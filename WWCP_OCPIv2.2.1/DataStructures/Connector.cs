/*
 * Copyright (c) 2015-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPI <https://github.com/OpenChargingCloud/WWCP_OCPI>
 *
 * Licensed under the Affero GPL license, Version 3.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.gnu.org/licenses/agpl.html
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#region Usings

using System.Security.Cryptography;
using System.Diagnostics.CodeAnalysis;

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

        private readonly Lock patchLock = new();

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

                return ParentEVSE?.GetTariffIds(Id, EMSPId) ?? [];

            }
        }

        /// <summary>
        /// The optional URL to the operator's terms and conditions.
        /// </summary>
        [Optional]
        public URL?                    TermsAndConditionsURL    { get; }


        /// <summary>
        /// The timestamp when this connector was created.
        /// </summary>
        [Mandatory, VendorExtension(VE.GraphDefined, VE.Pagination)]
        public DateTimeOffset          Created                  { get; }

        /// <summary>
        /// The timestamp when this connector was last updated (or created).
        /// </summary>
        [Mandatory]
        public DateTimeOffset          LastUpdated              { get; }

        /// <summary>
        /// The SHA256 hash of the JSON representation of this connector.
        /// </summary>
        public String                  ETag                     { get; }

        #endregion

        #region Constructor(s)

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
        /// <param name="Created">The optional timestamp when this connector was created.</param>
        /// <param name="LastUpdated">A timestamp when this connector was last updated (or created).</param>
        /// 
        /// <param name="ParentEVSE">The parent EVSE of this connector.</param>
        /// 
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

                         DateTimeOffset?                              Created                     = null,
                         DateTimeOffset?                              LastUpdated                 = null,
                         String?                                      ETag                        = null,

                         EVSE?                                        ParentEVSE                  = null,
                         EMSP_Id?                                     EMSPId                      = null,
                         CustomJObjectSerializerDelegate<Connector>?  CustomConnectorSerializer   = null)

        {

            this.Id                     = Id;
            this.Standard               = Standard;
            this.Format                 = Format;
            this.PowerType              = PowerType;
            this.MaxVoltage             = MaxVoltage;
            this.MaxAmperage            = MaxAmperage;
            this.MaxElectricPower       = MaxElectricPower;
            this.tariffIds              = TariffIds?.Distinct() ?? [];
            this.TermsAndConditionsURL  = TermsAndConditionsURL;

            var created                 = Created     ?? LastUpdated ?? Timestamp.Now;
            this.Created                = created;
            this.LastUpdated            = LastUpdated ?? created;

            this.ParentEVSE             = ParentEVSE;

            this.ETag                   = ETag        ?? SHA256.HashData(
                                                             ToJSON(
                                                                 true,
                                                                 true,
                                                                 EMSPId,
                                                                 CustomConnectorSerializer
                                                             ).ToUTF8Bytes()
                                                         ).ToBase64();

            unchecked
            {

                hashCode = this.Id.                    GetHashCode()       * 31 ^
                           this.Standard.              GetHashCode()       * 29 ^
                           this.Format.                GetHashCode()       * 23 ^
                           this.PowerType.             GetHashCode()       * 19 ^
                           this.MaxVoltage.            GetHashCode()       * 17 ^
                           this.MaxAmperage.           GetHashCode()       * 13 ^
                           this.TariffIds.             CalcHashCode()      * 11 ^
                          (this.MaxElectricPower?.     GetHashCode() ?? 0) *  7 ^
                          (this.TermsAndConditionsURL?.GetHashCode() ?? 0) *  5 ^
                           this.Created.               GetHashCode()       *  3 ^
                           this.LastUpdated.           GetHashCode();

            }

        }

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
                return connector;
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
        /// <param name="connectorIdURL">An optional connector identification, e.g. from the HTTP URL.</param>
        /// <param name="CustomConnectorParser">A delegate to parse custom connector JSON objects.</param>
        public static Boolean TryParse(JObject                                  JSON,
                                       [NotNullWhen(true)]  out Connector?      Connector,
                                       [NotNullWhen(false)] out String?         ErrorResponse,
                                       Connector_Id?                            connectorIdURL          = null,
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
                                       out Connector_Id? connectorIdBody,
                                       out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                }

                if (!connectorIdURL.HasValue && !connectorIdBody.HasValue)
                {
                    ErrorResponse = "The connector identification is missing!";
                    return false;
                }

                if (connectorIdURL.HasValue && connectorIdBody.HasValue && connectorIdURL.Value != connectorIdBody.Value)
                {
                    ErrorResponse = "The optional connector identification given within the JSON body does not match the one given in the URL!";
                    return false;
                }

                #endregion

                #region Parse Standard            [mandatory]

                if (!JSON.ParseMandatory("standard",
                                         "connector standard",
                                         ConnectorType.TryParse,
                                         out ConnectorType standard,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Format              [mandatory]

                if (!JSON.ParseMandatory("format",
                                         "connector format",
                                         ConnectorFormatsExtensions.TryParse,
                                         out ConnectorFormats format,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse PowerType           [mandatory]

                if (!JSON.ParseMandatory("power_type",
                                         "power type",
                                         PowerTypesExtensions.TryParse,
                                         out PowerTypes powerType,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse MaxVoltage          [mandatory]

                if (!JSON.ParseMandatory("max_voltage",
                                         "max voltage",
                                         Volt.TryParse,
                                         out Volt maxVoltage,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse MaxAmperage         [mandatory]

                if (!JSON.ParseMandatory("max_amperage",
                                         "max amperage",
                                         Ampere.TryParse,
                                         out Ampere maxAmperage,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse MaxElectricPower    [optional]

                if (JSON.ParseOptional("max_electric_power",
                                       "max electric power",
                                       Watt.TryParse,
                                       out Watt? maxElectricPower,
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
                                           out IEnumerable<Tariff_Id> tariffIds,
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
                                       out URL? termsAndConditionsURL,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                #region Parse Created             [optional, VendorExtension]

                if (JSON.ParseOptional("created",
                                       "created",
                                       out DateTimeOffset? created,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse LastUpdated         [mandatory]

                if (!JSON.ParseMandatory("last_updated",
                                         "last updated",
                                         out DateTimeOffset lastUpdated,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                Connector = new Connector(

                                connectorIdBody ?? connectorIdURL!.Value,
                                standard,
                                format,
                                powerType,
                                maxVoltage,
                                maxAmperage,

                                maxElectricPower,
                                tariffIds,
                                termsAndConditionsURL,

                                created,
                                lastUpdated

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
        public JObject ToJSON(Boolean                                      IncludeCreatedTimestamp     = true,
                              Boolean                                      IncludeExtensions           = true,
                              EMSP_Id?                                     EMSPId                      = null,
                              CustomJObjectSerializerDelegate<Connector>?  CustomConnectorSerializer   = null)
        {

            var tariffIds  = this.tariffIds;

            if (!tariffIds.Any())
                tariffIds  = GetTariffIds(EMSPId);

            var json       = JSONObject.Create(

                                       new JProperty("id",                     Id.                    ToString()),
                                       new JProperty("standard",               Standard.              ToString()),
                                       new JProperty("format",                 Format.                AsText()),
                                       new JProperty("power_type",             PowerType.             AsText()),
                                       new JProperty("max_voltage",            MaxVoltage.            IntegerValue),
                                       new JProperty("max_amperage",           MaxAmperage.           IntegerValue),

                                 MaxElectricPower.HasValue
                                     ? new JProperty("max_electric_power",     MaxElectricPower.Value.IntegerValue)
                                     : null,

                                 tariffIds is not null && tariffIds.Any()
                                     ? new JProperty("tariff_ids",             new JArray(tariffIds.Select(tariffId => tariffId.ToString())))
                                     : null,

                                 TermsAndConditionsURL.HasValue
                                     ? new JProperty("terms_and_conditions",   TermsAndConditionsURL. ToString())
                                     : null,

                                 IncludeCreatedTimestamp
                                     ? new JProperty("created",                Created.               ToISO8601())
                                     : null,

                                       new JProperty("last_updated",           LastUpdated.           ToISO8601())

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

            => new (

                   Id.                    Clone(),
                   Standard.              Clone(),
                   Format,
                   PowerType,
                   MaxVoltage,
                   MaxAmperage,
                   MaxElectricPower,
                   [.. TariffIds],
                   TermsAndConditionsURL?.Clone(),

                   Created,
                   LastUpdated,

                   ETag.                  CloneString(),
                   ParentEVSE

               );

        #endregion


        #region (private) TryPrivatePatch(JSON, Patch)

        private PatchResult<JObject> TryPrivatePatch(JObject           JSON,
                                                     JObject           Patch,
                                                     EventTracking_Id  EventTrackingId)
        {

            foreach (var property in Patch)
            {

                if (property.Key == "id")
                    return PatchResult<JObject>.Failed(EventTrackingId, JSON,
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
                            var patchResult = TryPrivatePatch(oldSubObject, subObject, EventTrackingId);

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

            return PatchResult<JObject>.Success(EventTrackingId, JSON);

        }

        #endregion

        #region TryPatch(ConnectorPatch, AllowDowngrades = false))

        /// <summary>
        /// Try to patch the JSON representation of this connector.
        /// </summary>
        /// <param name="ConnectorPatch">The JSON merge patch.</param>
        /// <param name="AllowDowngrades">Allow to set the 'lastUpdated' timestamp to an earlier value.</param>
        public PatchResult<Connector> TryPatch(JObject           ConnectorPatch,
                                               Boolean           AllowDowngrades   = false,
                                               EventTracking_Id? EventTrackingId   = null)
        {

            EventTrackingId ??= EventTracking_Id.New;
            if (ConnectorPatch is null)
                return PatchResult<Connector>.Failed(EventTrackingId, this,
                                                     "The given connector patch must not be null!");

            lock (patchLock)
            {

                if (ConnectorPatch["last_updated"] is null)
                    ConnectorPatch["last_updated"] = Timestamp.Now.ToISO8601();

                else if (AllowDowngrades == false &&
                        ConnectorPatch["last_updated"].Type == JTokenType.Date &&
                       (ConnectorPatch["last_updated"].Value<DateTime>().ToISO8601().CompareTo(LastUpdated.ToISO8601()) < 1))
                {
                    return PatchResult<Connector>.Failed(EventTrackingId, this,
                                                         "The 'lastUpdated' timestamp of the connector patch must be newer then the timestamp of the existing connector!");
                }


                var patchResult = TryPrivatePatch(ToJSON(), ConnectorPatch, EventTrackingId);


                if (patchResult.IsFailed)
                    return PatchResult<Connector>.Failed(EventTrackingId, this,
                                                         patchResult.ErrorResponse);

                if (TryParse(patchResult.PatchedData,
                             out var patchedConnector,
                             out var errorResponse))
                {

                    return PatchResult<Connector>.Success(EventTrackingId, patchedConnector,
                                                          errorResponse);

                }

                else
                    return PatchResult<Connector>.Failed(EventTrackingId, this,
                                                         "Invalid JSON merge patch of a connector: " + errorResponse);

            }

        }

        #endregion


        #region GetTariffId(EMSPId = null)

        /// <summary>
        /// Returns the identification of the currently valid tariff.
        /// For a "Free of Charge" tariff this field should be set, and point to a defined "Free of Charge" tariff.
        /// </summary>
        /// <param name="EMSPId">An optional EMSP identification, e.g. for including the right tariff.</param>
        public IEnumerable<Tariff_Id> GetTariffIds(EMSP_Id? EMSPId = null)
        {

            var tariffIds = ParentEVSE?.GetTariffIds(Id, EMSPId) ?? [];
            //if (tariffIds.Any())
            //{

            //    var now      = Timestamp.Now;
            //    var tariffs  = new List<Tariff>();

            //    foreach (var tariffId in tariffIds)
            //    {

            //        var tariff = ParentEVSE?.ParentLocation?.CommonAPI?.GetTariff(tariffId);

            //        if (tariff is not null)
            //        {
            //            if (now >= tariff.NotBefore &&
            //                now <  tariff.NotAfter)
            //            {
            //                tariffs.Add(tariff);
            //            }
            //        }

            //    }

            //    // When there are multiple tariffs...
            //    // prefer the one that is valid for the longest remaining time!
            //    return tariffs.OrderBy(tariff => now - tariff.NotAfter).FirstOrDefault()?.Id;

            //}

            //if (emspTariffIds is not null &&
            //    EMSPId.HasValue           &&
            //    emspTariffIds.TryGetValue(EMSPId.Value, out var emspTariffId))
            //{
            //    return emspTariffId;
            //}

            //if (tariffId.HasValue)
            //    return tariffId;

            return tariffIds;

        }

        #endregion


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
               LastUpdated.ToISO8601().Equals(Connector.LastUpdated.ToISO8601()) &&

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
