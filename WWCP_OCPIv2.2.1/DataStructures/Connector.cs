/*
 * Copyright (c) 2015-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPI <https://github.com/OpenChargingCloud/WWCP_OCPI>
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#region Usings

using System.Security.Cryptography;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2_1
{

    /// <summary>
    /// A connector is the socket or cable available for the electric vehicle to make use of.
    /// </summary>
    public class Connector : IHasId<Connector_Id>,
                             IEquatable<Connector>,
                             IComparable<Connector>,
                             IComparable
    {

        #region Data

        private readonly Object patchLock = new();

        #endregion

        #region Properties

        /// <summary>
        /// The parent EVSE of this connector.
        /// </summary>
        public EVSE?                   ParentEVSE               { get; internal set; }

        /// <summary>
        /// The identification of the connector within the EVSE.
        /// Two connectors may have the same id as long as they do not belong to the same EVSE object.
        /// string(36)
        /// </summary>
        [Mandatory]
        public Connector_Id            Id                       { get; }

        /// <summary>
        /// The standard of the installed connector.
        /// </summary>
        [Mandatory]
        public ConnectorType           Standard                 { get; }

        /// <summary>
        /// The format (socket/cable) of the installed connector.
        /// </summary>
        [Mandatory]
        public ConnectorFormats        Format                   { get; }

        /// <summary>
        /// The type of power at the connector.
        /// </summary>
        [Mandatory]
        public PowerTypes              PowerType                { get; }

        /// <summary>
        /// The maximum voltage of the connector (line to neutral for AC_3_PHASE).
        /// </summary>
        [Mandatory]
        public Volt                    MaxVoltage               { get; }

        /// <summary>
        /// The maximum amperage of the connector.
        /// </summary>
        [Mandatory]
        public Ampere                  MaxAmperage              { get; }

        /// <summary>
        /// The maximum electric power that can be delivered by this connector.
        /// When the maximum electric power is lower than the calculated value from voltage
        /// and amperage, this value should be set.
        /// </summary>
        [Optional]
        public Watt?                   MaxElectricPower         { get; }

        public EMSP_Id?                EMSPId                   { get; }

        private readonly IEnumerable<Tariff_Id> tariffIds;

        /// <summary>
        /// The enumeration of currently valid charging tariffs identifiers.
        /// Multiple tariffs are possible, but only one of each tariff.type can be active at the
        /// same time. Tariffs with the same type are only allowed if they are not active at the
        /// same time: start_date_time and end_date_time period not overlapping.
        /// </summary>
        [Optional]
        public IEnumerable<Tariff_Id>  TariffIds
        {
            get
            {

                if (tariffIds.Any())
                    return tariffIds;

                return ParentEVSE?.GetTariffIds(Id, EMSPId) ?? Array.Empty<Tariff_Id>();

            }
        }

        /// <summary>
        /// The optional URL to the operator's terms and conditions.
        /// </summary>
        [Optional]
        public URL?                    TermsAndConditionsURL    { get; }

        /// <summary>
        /// The timestamp when this connector was last updated (or created).
        /// </summary>
        [Mandatory]
        public DateTime                LastUpdated              { get; }

        /// <summary>
        /// The SHA256 hash of the JSON representation of this connector.
        /// </summary>
        public String                  ETag                     { get; }

        #endregion

        #region Constructor(s)

        #region (internal) Connector(ParentEVSE, Id, Standard, ... )

        /// <summary>
        /// A connector is the socket or cable available for the electric vehicle to make use of.
        /// </summary>
        /// <param name="ParentEVSE">The parent EVSE of this connector.</param>
        /// 
        /// <param name="Id">Identifier of the connector within the EVSE.</param>
        /// <param name="Standard">The standard of the installed connector.</param>
        /// <param name="Format">The format (socket/cable) of the installed connector.</param>
        /// <param name="PowerType">The type of power at the connector.</param>
        /// <param name="MaxVoltage">The maximum voltage of the connector (line to neutral for AC_3_PHASE).</param>
        /// <param name="MaxAmperage">The maximum amperage of the connector.</param>
        /// <param name="MaxElectricPower">The maximum electric power that can be delivered by this connector.</param>
        /// <param name="TariffIds">An enumeration of currently valid charging tariffs identifiers.</param>
        /// <param name="TermsAndConditionsURL">An optional URL to the operator's terms and conditions.</param>
        /// 
        /// <param name="LastUpdated">A timestamp when this connector was last updated (or created).</param>
        /// <param name="CustomConnectorSerializer">A delegate to serialize custom connector JSON objects.</param>
        internal Connector(EVSE?                                        ParentEVSE,

                           Connector_Id                                 Id,
                           ConnectorType                                Standard,
                           ConnectorFormats                             Format,
                           PowerTypes                                   PowerType,
                           Volt                                         MaxVoltage,
                           Ampere                                       MaxAmperage,
                           Watt?                                        MaxElectricPower            = null,
                           IEnumerable<Tariff_Id>?                      TariffIds                   = null,
                           URL?                                         TermsAndConditionsURL       = null,

                           DateTime?                                    LastUpdated                 = null,
                           EMSP_Id?                                     EMSPId                      = null,
                           CustomJObjectSerializerDelegate<Connector>?  CustomConnectorSerializer   = null)

        {

            this.ParentEVSE             = ParentEVSE;

            this.Id                     = Id;
            this.Standard               = Standard;
            this.Format                 = Format;
            this.PowerType              = PowerType;
            this.MaxVoltage             = MaxVoltage;
            this.MaxAmperage            = MaxAmperage;
            this.MaxElectricPower       = MaxElectricPower;
            this.tariffIds              = TariffIds?.Distinct() ?? Array.Empty<Tariff_Id>();
            this.TermsAndConditionsURL  = TermsAndConditionsURL;

            this.LastUpdated            = LastUpdated ?? Timestamp.Now;

            this.ETag                   = SHA256.HashData(ToJSON(EMSPId, CustomConnectorSerializer).ToUTF8Bytes()).ToBase64();

            unchecked
            {

                hashCode = this.Id.                    GetHashCode()       * 29 ^
                           this.Standard.              GetHashCode()       * 23 ^
                           this.Format.                GetHashCode()       * 19 ^
                           this.PowerType.             GetHashCode()       * 17 ^
                           this.MaxVoltage.            GetHashCode()       * 13 ^
                           this.MaxAmperage.           GetHashCode()       * 11 ^
                           this.TariffIds.             CalcHashCode()      *  7 ^
                          (this.MaxElectricPower?.     GetHashCode() ?? 0) *  5 ^
                          (this.TermsAndConditionsURL?.GetHashCode() ?? 0) *  3 ^
                           this.LastUpdated.           GetHashCode();

            }

        }

        #endregion

        #region Connector(Id, Standard, ... )

        /// <summary>
        /// A connector is the socket or cable available for the electric vehicle to make use of.
        /// </summary>
        /// <param name="Id">Identifier of the connector within the EVSE.</param>
        /// <param name="Standard">The standard of the installed connector.</param>
        /// <param name="Format">The format (socket/cable) of the installed connector.</param>
        /// <param name="PowerType">The type of power at the connector.</param>
        /// <param name="MaxVoltage">The maximum voltage of the connector (line to neutral for AC_3_PHASE).</param>
        /// <param name="MaxAmperage">The maximum amperage of the connector.</param>
        /// <param name="MaxElectricPower">The maximum electric power that can be delivered by this connector.</param>
        /// <param name="TariffIds">An enumeration of currently valid charging tariffs identifiers.</param>
        /// <param name="TermsAndConditionsURL">An optional URL to the operator's terms and conditions.</param>
        /// 
        /// <param name="LastUpdated">A timestamp when this connector was last updated (or created).</param>
        /// <param name="CustomConnectorSerializer">A delegate to serialize custom connector JSON objects.</param>
        public Connector(Connector_Id                                 Id,
                         ConnectorType                                Standard,
                         ConnectorFormats                             Format,
                         PowerTypes                                   PowerType,
                         Volt                                         MaxVoltage,
                         Ampere                                       MaxAmperage,
                         Watt?                                        MaxElectricPower            = null,
                         IEnumerable<Tariff_Id>?                      TariffIds                   = null,
                         URL?                                         TermsAndConditionsURL       = null,

                         DateTime?                                    LastUpdated                 = null,
                         EMSP_Id?                                     EMSPId                      = null,
                         CustomJObjectSerializerDelegate<Connector>?  CustomConnectorSerializer   = null)

            : this(null,

                   Id,
                   Standard,
                   Format,
                   PowerType,
                   MaxVoltage,
                   MaxAmperage,
                   MaxElectricPower,
                   TariffIds,
                   TermsAndConditionsURL,

                   LastUpdated,
                   EMSPId,
                   CustomConnectorSerializer)

        { }

        #endregion

        #endregion


        #region (static) Parse   (JSON, ConnectorIdURL = null, CustomConnectorParser = null)

        /// <summary>
        /// Parse the given JSON representation of a connector.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="ConnectorIdURL">An optional connector identification, e.g. from the HTTP URL.</param>
        /// <param name="CustomConnectorParser">A delegate to parse custom connector JSON objects.</param>
        public static Connector Parse(JObject                                  JSON,
                                      Connector_Id?                            ConnectorIdURL          = null,
                                      CustomJObjectParserDelegate<Connector>?  CustomConnectorParser   = null)
        {

            if (TryParse(JSON,
                         out var connector,
                         out var errorResponse,
                         ConnectorIdURL,
                         CustomConnectorParser))
            {
                return connector!;
            }

            throw new ArgumentException("The given JSON representation of a connector is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out Connector, out ErrorResponse, ConnectorIdURL = null, CustomConnectorParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a connector.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="Connector">The parsed connector.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="ConnectorIdURL">An optional connector identification, e.g. from the HTTP URL.</param>
        /// <param name="CustomConnectorParser">A delegate to parse custom connector JSON objects.</param>
        public static Boolean TryParse(JObject                                  JSON,
                                       out Connector?                           Connector,
                                       out String?                              ErrorResponse,
                                       Connector_Id?                            ConnectorIdURL          = null,
                                       CustomJObjectParserDelegate<Connector>?  CustomConnectorParser   = null)
        {

            try
            {

                Connector = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse Id                  [mandatory]

                if (JSON.ParseOptional("id",
                                       "connector identification",
                                       Connector_Id.TryParse,
                                       out Connector_Id? ConnectorIdBody,
                                       out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                }

                if (!ConnectorIdURL.HasValue && !ConnectorIdBody.HasValue)
                {
                    ErrorResponse = "The connector identification is missing!";
                    return false;
                }

                if (ConnectorIdURL.HasValue && ConnectorIdBody.HasValue && ConnectorIdURL.Value != ConnectorIdBody.Value)
                {
                    ErrorResponse = "The optional connector identification given within the JSON body does not match the one given in the URL!";
                    return false;
                }

                #endregion

                #region Parse Standard            [mandatory]

                if (!JSON.ParseMandatory("standard",
                                         "connector standard",
                                         ConnectorType.TryParse,
                                         out ConnectorType Standard,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Format              [mandatory]

                if (!JSON.ParseMandatory("format",
                                         "connector format",
                                         ConnectorFormatsExtensions.TryParse,
                                         out ConnectorFormats Format,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse PowerType           [mandatory]

                if (!JSON.ParseMandatory("power_type",
                                         "power type",
                                         PowerTypesExtensions.TryParse,
                                         out PowerTypes PowerType,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse MaxVoltage          [mandatory]

                if (!JSON.ParseMandatory("max_voltage",
                                         "max voltage",
                                         Volt.TryParse,
                                         out Volt MaxVoltage,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse MaxAmperage         [mandatory]

                if (!JSON.ParseMandatory("max_amperage",
                                         "max amperage",
                                         Ampere.TryParse,
                                         out Ampere MaxAmperage,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse MaxElectricPower    [optional]

                if (JSON.ParseOptional("max_electric_power",
                                       "max electric power",
                                       Watt.TryParse,
                                       out Watt? MaxElectricPower,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse TariffIds           [optional]

                if (JSON.ParseOptionalJSON("tariff_ids",
                                           "tariff identifications",
                                           Tariff_Id.TryParse,
                                           out IEnumerable<Tariff_Id> TariffIds,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region TermsAndConditionsURL     [optional]

                if (JSON.ParseOptional("terms_and_conditions",
                                       "terms and conditions",
                                       URL.TryParse,
                                       out URL? TermsAndConditionsURL,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse LastUpdated         [mandatory]

                if (!JSON.ParseMandatory("last_updated",
                                         "last updated",
                                         out DateTime LastUpdated,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                Connector = new Connector(
                                ConnectorIdBody ?? ConnectorIdURL!.Value,
                                Standard,
                                Format,
                                PowerType,
                                MaxVoltage,
                                MaxAmperage,

                                MaxElectricPower,
                                TariffIds,
                                TermsAndConditionsURL,

                                LastUpdated
                            );


                if (CustomConnectorParser is not null)
                    Connector = CustomConnectorParser(JSON,
                                                      Connector);

                return true;

            }
            catch (Exception e)
            {
                Connector      = default;
                ErrorResponse  = "The given JSON representation of a connector is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomConnectorSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomConnectorSerializer">A delegate to serialize custom connector JSON objects.</param>
        public JObject ToJSON(EMSP_Id?                                     EMSPId                      = null,
                              CustomJObjectSerializerDelegate<Connector>?  CustomConnectorSerializer   = null)
        {

            var tariffIds  = this.tariffIds;

            if (!tariffIds.Any())
                tariffIds  = GetTariffIds(EMSPId);

            var json       = JSONObject.Create(

                                       new JProperty("id",                     Id.                   ToString()),
                                       new JProperty("standard",               Standard.             ToString()),
                                       new JProperty("format",                 Format.               AsText()),
                                       new JProperty("power_type",             PowerType.            AsText()),
                                       new JProperty("max_voltage",            MaxVoltage),
                                       new JProperty("max_amperage",           MaxAmperage),

                                 MaxElectricPower.HasValue
                                     ? new JProperty("max_electric_power",     MaxElectricPower.Value)
                                     : null,

                                 tariffIds is not null && tariffIds.Any()
                                     ? new JProperty("tariff_ids",             new JArray(tariffIds.Select(tariffId => tariffId.ToString())))
                                     : null,

                                 TermsAndConditionsURL.HasValue
                                     ? new JProperty("terms_and_conditions",   TermsAndConditionsURL.ToString())
                                     : null,

                                       new JProperty("last_updated",           LastUpdated.          ToIso8601())

                             );

            return CustomConnectorSerializer is not null
                       ? CustomConnectorSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this object.
        /// </summary>
        public Connector Clone()

            => new (ParentEVSE,

                    Id.                    Clone,
                    Standard.              Clone,
                    Format,
                    PowerType,
                    MaxVoltage,
                    MaxAmperage,
                    MaxElectricPower,
                    TariffIds.Select(tariffId => tariffId.Clone).ToArray(),
                    TermsAndConditionsURL?.Clone,

                    LastUpdated);

        #endregion


        #region (private) TryPrivatePatch(JSON, Patch)

        private PatchResult<JObject> TryPrivatePatch(JObject  JSON,
                                                     JObject  Patch)
        {

            foreach (var property in Patch)
            {

                if (property.Key == "id")
                    return PatchResult<JObject>.Failed(JSON,
                                                       "Patching the 'identification' of a connector is not allowed!");

                else if (property.Value is null)
                    JSON.Remove(property.Key);

                else if (property.Value is JObject subObject)
                {

                    if (JSON.ContainsKey(property.Key))
                    {

                        if (JSON[property.Key] is JObject oldSubObject)
                        {

                            //ToDo: Perhaps use a more generic JSON patch here!
                            // PatchObject.Apply(ToJSON(), EVSEPatch),
                            var patchResult = TryPrivatePatch(oldSubObject, subObject);

                            if (patchResult.IsSuccess)
                                JSON[property.Key] = patchResult.PatchedData;

                        }

                        else
                            JSON[property.Key] = subObject;

                    }

                    else
                        JSON.Add(property.Key, subObject);

                }

                //else if (property.Value is JArray subArray)
                //{
                //}

                else
                    JSON[property.Key] = property.Value;

            }

            return PatchResult<JObject>.Success(JSON);

        }

        #endregion

        #region TryPatch(ConnectorPatch, AllowDowngrades = false))

        /// <summary>
        /// Try to patch the JSON representaion of this connector.
        /// </summary>
        /// <param name="ConnectorPatch">The JSON merge patch.</param>
        /// <param name="AllowDowngrades">Allow to set the 'lastUpdated' timestamp to an earlier value.</param>
        public PatchResult<Connector> TryPatch(JObject  ConnectorPatch,
                                               Boolean  AllowDowngrades = false)
        {

            if (ConnectorPatch is null)
                return PatchResult<Connector>.Failed(this,
                                                     "The given connector patch must not be null!");

            lock (patchLock)
            {

                if (ConnectorPatch["last_updated"] is null)
                    ConnectorPatch["last_updated"] = Timestamp.Now.ToIso8601();

                else if (AllowDowngrades == false &&
                        ConnectorPatch["last_updated"].Type == JTokenType.Date &&
                       (ConnectorPatch["last_updated"].Value<DateTime>().ToIso8601().CompareTo(LastUpdated.ToIso8601()) < 1))
                {
                    return PatchResult<Connector>.Failed(this,
                                                         "The 'lastUpdated' timestamp of the connector patch must be newer then the timestamp of the existing connector!");
                }


                var patchResult = TryPrivatePatch(ToJSON(), ConnectorPatch);


                if (patchResult.IsFailed)
                    return PatchResult<Connector>.Failed(this,
                                                         patchResult.ErrorResponse);

                if (TryParse(patchResult.PatchedData,
                             out var patchedConnector,
                             out var errorResponse))
                {

                    return PatchResult<Connector>.Success(patchedConnector,
                                                          errorResponse);

                }

                else
                    return PatchResult<Connector>.Failed(this,
                                                         "Invalid JSON merge patch of a connector: " + errorResponse);

            }

        }

        #endregion


        internal IEnumerable<Tariff_Id> GetTariffIds(EMSP_Id? EMSPId = null)

            => ParentEVSE?.GetTariffIds(Id,
                                        EMSPId) ?? Array.Empty<Tariff_Id>();


        #region Operator overloading

        #region Operator == (Connector1, Connector2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Connector1">A connector.</param>
        /// <param name="Connector2">Another connector.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Connector? Connector1,
                                           Connector? Connector2)
        {

            if (Object.ReferenceEquals(Connector1, Connector2))
                return true;

            if (Connector1 is null || Connector2 is null)
                return false;

            return Connector1.Equals(Connector2);

        }

        #endregion

        #region Operator != (Connector1, Connector2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Connector1">A connector.</param>
        /// <param name="Connector2">Another connector.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Connector? Connector1,
                                           Connector? Connector2)

            => !(Connector1 == Connector2);

        #endregion

        #region Operator <  (Connector1, Connector2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Connector1">A connector.</param>
        /// <param name="Connector2">Another connector.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Connector? Connector1,
                                          Connector? Connector2)

            => Connector1 is null
                   ? throw new ArgumentNullException(nameof(Connector1), "The given connector must not be null!")
                   : Connector1.CompareTo(Connector2) < 0;

        #endregion

        #region Operator <= (Connector1, Connector2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Connector1">A connector.</param>
        /// <param name="Connector2">Another connector.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Connector? Connector1,
                                           Connector? Connector2)

            => !(Connector1 > Connector2);

        #endregion

        #region Operator >  (Connector1, Connector2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Connector1">A connector.</param>
        /// <param name="Connector2">Another connector.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Connector? Connector1,
                                          Connector? Connector2)

            => Connector1 is null
                   ? throw new ArgumentNullException(nameof(Connector1), "The given connector must not be null!")
                   : Connector1.CompareTo(Connector2) > 0;

        #endregion

        #region Operator >= (Connector1, Connector2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Connector1">A connector.</param>
        /// <param name="Connector2">Another connector.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Connector? Connector1,
                                           Connector? Connector2)

            => !(Connector1 < Connector2);

        #endregion

        #endregion

        #region IComparable<Connector> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two connectors.
        /// </summary>
        /// <param name="Object">An connector to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Connector connector
                   ? CompareTo(connector)
                   : throw new ArgumentException("The given object is not a connector!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(Connector)

        /// <summary>
        /// Compares two connectors.
        /// </summary>
        /// <param name="Object">An connector to compare with.</param>
        public Int32 CompareTo(Connector? Connector)
        {

            if (Connector is null)
                throw new ArgumentNullException(nameof(Connector), "The given connector must not be null!");

            var c = Id.         CompareTo(Connector.Id);

            if (c == 0)
                c = Standard.   CompareTo(Connector.Standard);

            if (c == 0)
                c = Format.     CompareTo(Connector.Format);

            if (c == 0)
                c = PowerType.  CompareTo(Connector.PowerType);

            if (c == 0)
                c = MaxVoltage. CompareTo(Connector.MaxVoltage);

            if (c == 0)
                c = MaxAmperage.CompareTo(Connector.MaxAmperage);

            if (c == 0)
                c = LastUpdated.CompareTo(Connector.LastUpdated);

            if (c == 0)
                c = ETag.       CompareTo(Connector.ETag);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<Connector> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two connectors for equality.
        /// </summary>
        /// <param name="Object">An connector to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Connector connector &&
                   Equals(connector);

        #endregion

        #region Equals(Connector)

        /// <summary>
        /// Compares two connectors for equality.
        /// </summary>
        /// <param name="Connector">An connector to compare with.</param>
        public Boolean Equals(Connector? Connector)

            => Connector is not null &&

               Id.                     Equals(Connector.Id)                      &&
               Standard.               Equals(Connector.Standard)                &&
               Format.                 Equals(Connector.Format)                  &&
               PowerType.              Equals(Connector.PowerType)               &&
               MaxVoltage.             Equals(Connector.MaxVoltage)              &&
               MaxAmperage.            Equals(Connector.MaxAmperage)             &&
               MaxElectricPower.       Equals(Connector.MaxElectricPower)        &&
               LastUpdated.ToIso8601().Equals(Connector.LastUpdated.ToIso8601()) &&

               TariffIds.Count().Equals(Connector.TariffIds.Count()) &&
               TariffIds.All(tariffId => Connector.TariffIds.Contains(tariffId)) &&

            ((!TermsAndConditionsURL.HasValue && !Connector.TermsAndConditionsURL.HasValue) ||
              (TermsAndConditionsURL.HasValue &&  Connector.TermsAndConditionsURL.HasValue && TermsAndConditionsURL.Value.Equals(Connector.TermsAndConditionsURL.Value)));

        #endregion

        #endregion

        #region (override) GetHashCode()

        private readonly Int32 hashCode;

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        public override Int32 GetHashCode()
            => hashCode;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   Id,
                   " (",
                   Standard,           ", ",
                   Format.   AsText(), ", ",
                   PowerType.AsText(), ", ",
                   MaxVoltage,         " V, ",
                   MaxAmperage,        " A",

                   MaxElectricPower.HasValue
                       ? ", " + MaxElectricPower.Value + " W"
                       : "",

                   TariffIds.Any()
                       ? ", " + TariffIds.Count() + " tariff id(s)"
                       : "",

                   ")"

               );

        #endregion


    }

}
