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

namespace cloud.charging.open.protocols.OCPIv2_1_1
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

        private readonly Lock                             patchLock  = new();

        private readonly Tariff_Id?                       tariffId;
        private readonly Dictionary<EMSP_Id, Tariff_Id>?  emspTariffIds;

        #endregion

        #region Properties

        /// <summary>
        /// The parent EVSE of this connector.
        /// </summary>
        public EVSE?             ParentEVSE               { get; internal set; }

        /// <summary>
        /// The identification of the connector within the EVSE.
        /// Two connectors may have the same id as long as they do not belong to the same EVSE object.
        /// string(36)
        /// </summary>
        [Mandatory]
        public Connector_Id      Id                       { get; }

        /// <summary>
        /// The standard of the installed connector.
        /// </summary>
        [Mandatory]
        public ConnectorType     Standard                 { get; }

        /// <summary>
        /// The format (socket/cable) of the installed connector.
        /// </summary>
        [Mandatory]
        public ConnectorFormats  Format                   { get; }

        /// <summary>
        /// The type of power at the connector.
        /// </summary>
        [Mandatory]
        public PowerTypes        PowerType                { get; }

        /// <summary>
        /// The voltage of the connector (line to neutral for AC_3_PHASE).
        /// </summary>
        [Mandatory]
        public Volt              Voltage                  { get; }

        /// <summary>
        /// The maximum amperage of the connector.
        /// </summary>
        [Mandatory]
        public Ampere            Amperage                 { get; }

        /// <summary>
        /// The optional URL to the operator's terms and conditions.
        /// </summary>
        [Optional]
        public URL?              TermsAndConditionsURL    { get; }

        /// <summary>
        /// The timestamp when this connector was last updated (or created).
        /// </summary>
        [Mandatory]
        public DateTimeOffset    LastUpdated              { get; }

        /// <summary>
        /// The SHA256 hash of the JSON representation of this connector.
        /// </summary>
        public String            ETag                     { get; }


        //public CustomJObjectSerializerDelegate<Connector>? CustomConnectorSerializer { get; }

        #endregion

        #region Constructor(s)

        #region (internal) Connector(ParentEVSE, Id, Standard, ... TariffId = null, ...)

        /// <summary>
        /// A connector is the socket or cable available for the electric vehicle to make use of.
        /// </summary>
        /// <param name="ParentEVSE">The parent EVSE of this connector.</param>
        /// 
        /// <param name="Id">An identification of the connector within the EVSE.</param>
        /// <param name="Standard">The standard of the installed connector.</param>
        /// <param name="Format">The format (socket/cable) of the installed connector.</param>
        /// <param name="PowerType">The type of power at the connector.</param>
        /// <param name="Voltage">The voltage of the connector (line to neutral for AC_3_PHASE), in volt [V].</param>
        /// <param name="Amperage">The amperage of the connector, in ampere [A].</param>
        /// <param name="TariffId">The identification of the currently valid charging tariff.</param>
        /// <param name="TermsAndConditionsURL">An optional URL to the operator's terms and conditions.</param>
        /// 
        /// <param name="LastUpdated">A timestamp when this connector was last updated (or created).</param>
        internal Connector(EVSE?             ParentEVSE,

                           Connector_Id      Id,
                           ConnectorType     Standard,
                           ConnectorFormats  Format,
                           PowerTypes        PowerType,
                           Volt              Voltage,
                           Ampere            Amperage,
                           Tariff_Id         TariffId,
                           URL?              TermsAndConditionsURL   = null,

                           DateTimeOffset?   LastUpdated             = null)

        {

            this.ParentEVSE             = ParentEVSE;

            this.Id                     = Id;
            this.Standard               = Standard;
            this.Format                 = Format;
            this.PowerType              = PowerType;
            this.Voltage                = Voltage;
            this.Amperage               = Amperage;
            this.tariffId               = TariffId;
            this.emspTariffIds          = null;
            this.TermsAndConditionsURL  = TermsAndConditionsURL;

            this.LastUpdated            = LastUpdated ?? Timestamp.Now;

            this.ETag                   = SHA256.HashData(ToJSON().ToUTF8Bytes()).ToBase64();


            unchecked
            {

                hashCode = this.Id.                    GetHashCode()        * 27 ^
                           this.Standard.              GetHashCode()        * 23 ^
                           this.Format.                GetHashCode()        * 19 ^
                           this.PowerType.             GetHashCode()        * 17 ^
                           this.Voltage.               GetHashCode()        * 13 ^
                           this.Amperage.              GetHashCode()        * 11 ^
                          (this.tariffId?.             GetHashCode()  ?? 0) *  7 ^
                          (this.emspTariffIds?.        CalcHashCode() ?? 0) *  5 ^
                          (this.TermsAndConditionsURL?.GetHashCode()  ?? 0) *  3 ^
                           this.LastUpdated.           GetHashCode();

            }

        }

        #endregion

        #region (internal) Connector(ParentEVSE, Id, Standard, ... EMSPTariffIds,   ...)

        /// <summary>
        /// A connector is the socket or cable available for the electric vehicle to make use of.
        /// </summary>
        /// <param name="ParentEVSE">The parent EVSE of this connector.</param>
        /// 
        /// <param name="Id">An identification of the connector within the EVSE.</param>
        /// <param name="Standard">The standard of the installed connector.</param>
        /// <param name="Format">The format (socket/cable) of the installed connector.</param>
        /// <param name="PowerType">The type of power at the connector.</param>
        /// <param name="Voltage">The voltage of the connector (line to neutral for AC_3_PHASE), in volt [V].</param>
        /// <param name="Amperage">The amperage of the connector, in ampere [A].</param>
        /// <param name="EMSPTariffIds">The mapping of EMSP identifications to tariff identifications.</param>
        /// <param name="TermsAndConditionsURL">An optional URL to the operator's terms and conditions.</param>
        /// 
        /// <param name="LastUpdated">A timestamp when this connector was last updated (or created).</param>
        internal Connector(EVSE?                            ParentEVSE,

                           Connector_Id                     Id,
                           ConnectorType                    Standard,
                           ConnectorFormats                 Format,
                           PowerTypes                       PowerType,
                           Volt                             Voltage,
                           Ampere                           Amperage,
                           Dictionary<EMSP_Id, Tariff_Id>?  EMSPTariffIds           = null,
                           URL?                             TermsAndConditionsURL   = null,

                           DateTimeOffset?                  LastUpdated             = null)

        {

            this.ParentEVSE             = ParentEVSE;

            this.Id                     = Id;
            this.Standard               = Standard;
            this.Format                 = Format;
            this.PowerType              = PowerType;
            this.Voltage                = Voltage;
            this.Amperage               = Amperage;
            this.tariffId               = null;
            this.emspTariffIds          = EMSPTariffIds;
            this.TermsAndConditionsURL  = TermsAndConditionsURL;

            this.LastUpdated            = LastUpdated ?? Timestamp.Now;

            this.ETag                   = SHA256.HashData(ToJSON().ToUTF8Bytes()).ToBase64();


            unchecked
            {

                hashCode = this.Id.                    GetHashCode()        * 27 ^
                           this.Standard.              GetHashCode()        * 23 ^
                           this.Format.                GetHashCode()        * 19 ^
                           this.PowerType.             GetHashCode()        * 17 ^
                           this.Voltage.               GetHashCode()        * 13 ^
                           this.Amperage.              GetHashCode()        * 11 ^
                          (this.tariffId?.             GetHashCode()  ?? 0) *  7 ^
                          (this.emspTariffIds?.        CalcHashCode() ?? 0) *  5 ^
                          (this.TermsAndConditionsURL?.GetHashCode()  ?? 0) *  3 ^
                           this.LastUpdated.           GetHashCode();

            }

        }

        #endregion


        #region Connector(Id, Standard, ... TariffId = null, ...)

        /// <summary>
        /// A connector is the socket or cable available for the electric vehicle to make use of.
        /// </summary>
        /// <param name="Id">Identifier of the connector within the EVSE.</param>
        /// <param name="Standard">The standard of the installed connector.</param>
        /// <param name="Format">The format (socket/cable) of the installed connector.</param>
        /// <param name="PowerType">The type of powert at the connector.</param>
        /// <param name="Voltage">Voltage of the connector (line to neutral for AC_3_PHASE), in volt [V].</param>
        /// <param name="Amperage">Maximum amperage of the connector, in ampere [A].</param>
        /// <param name="TariffId">The identification of the currently valid charging tariff.</param>
        /// <param name="TermsAndConditionsURL">Optional URL to the operator's terms and conditions.</param>
        /// 
        /// <param name="LastUpdated">Timestamp when this connector was last updated (or created).</param>
        public Connector(Connector_Id      Id,
                         ConnectorType     Standard,
                         ConnectorFormats  Format,
                         PowerTypes        PowerType,
                         Volt              Voltage,
                         Ampere            Amperage,
                         Tariff_Id         TariffId,
                         URL?              TermsAndConditionsURL   = null,

                         DateTimeOffset?   LastUpdated             = null)

            : this(null,

                   Id,
                   Standard,
                   Format,
                   PowerType,
                   Voltage,
                   Amperage,
                   TariffId,
                   TermsAndConditionsURL,

                   LastUpdated)

        { }

        #endregion

        #region Connector(Id, Standard, ... EMSPTariffIds,   ...)

        /// <summary>
        /// A connector is the socket or cable available for the electric vehicle to make use of.
        /// </summary>
        /// <param name="Id">Identifier of the connector within the EVSE.</param>
        /// <param name="Standard">The standard of the installed connector.</param>
        /// <param name="Format">The format (socket/cable) of the installed connector.</param>
        /// <param name="PowerType">The type of powert at the connector.</param>
        /// <param name="Voltage">Voltage of the connector (line to neutral for AC_3_PHASE), in volt [V].</param>
        /// <param name="Amperage">Maximum amperage of the connector, in ampere [A].</param>
        /// <param name="EMSPTariffIds">The mapping of EMSP identifications to tariff identifications.</param>
        /// <param name="TermsAndConditionsURL">Optional URL to the operator's terms and conditions.</param>
        /// 
        /// <param name="LastUpdated">Timestamp when this connector was last updated (or created).</param>
        public Connector(Connector_Id                     Id,
                         ConnectorType                    Standard,
                         ConnectorFormats                 Format,
                         PowerTypes                       PowerType,
                         Volt                             Voltage,
                         Ampere                           Amperage,
                         Dictionary<EMSP_Id, Tariff_Id>?  EMSPTariffIds           = null,
                         URL?                             TermsAndConditionsURL   = null,

                         DateTimeOffset?                  LastUpdated             = null)

            : this(null,

                   Id,
                   Standard,
                   Format,
                   PowerType,
                   Voltage,
                   Amperage,
                   EMSPTariffIds,
                   TermsAndConditionsURL,

                   LastUpdated)

        { }

        #endregion

        #endregion


        #region GetTariffId(EMSPId = null)

        /// <summary>
        /// Returns the identification of the currently valid tariff.
        /// For a "Free of Charge" tariff this field should be set, and point to a defined "Free of Charge" tariff.
        /// </summary>
        /// <param name="EMSPId">An optional EMSP identification, e.g. for including the right tariff.</param>
        public Tariff_Id? GetTariffId(EMSP_Id? EMSPId = null)
        {

            var tariffIds = ParentEVSE?.GetTariffIds(Id, EMSPId) ?? [];
            if (tariffIds.Any())
            {

                var now      = Timestamp.Now;
                var tariffs  = new List<Tariff>();

                foreach (var tariffId in tariffIds)
                {

                    var tariff = ParentEVSE?.ParentLocation?.CommonAPI?.GetTariff(tariffId);

                    if (tariff is not null)
                    {
                        if (now >= tariff.NotBefore &&
                            now <  tariff.NotAfter)
                        {
                            tariffs.Add(tariff);
                        }
                    }

                }

                // When there are multiple tariffs...
                // prefer the one that is valid for the longest remaining time!
                return tariffs.OrderBy(tariff => now - tariff.NotAfter).FirstOrDefault()?.Id;

            }

            if (emspTariffIds is not null &&
                EMSPId.HasValue           &&
                emspTariffIds.TryGetValue(EMSPId.Value, out var emspTariffId))
            {
                return emspTariffId;
            }

            if (tariffId.HasValue)
                return tariffId;

            return null;

        }

        #endregion


        #region (static) Parse   (JSON, ConnectorIdURL = null, IgnoreTariffId = false, CustomConnectorParser = null)

        /// <summary>
        /// Parse the given JSON representation of a connector.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="ConnectorIdURL">An optional connector identification, e.g. from the HTTP URL.</param>
        /// <param name="IgnoreTariffId">Whether to ignore the 'tariff_id' property.</param>
        /// <param name="CustomConnectorParser">A delegate to parse custom connector JSON objects.</param>
        public static Connector Parse(JObject                                  JSON,
                                      Connector_Id?                            ConnectorIdURL          = null,
                                      Boolean                                  IgnoreTariffId          = false,
                                      CustomJObjectParserDelegate<Connector>?  CustomConnectorParser   = null)
        {

            if (TryParse(JSON,
                         out var connector,
                         out var errorResponse,
                         ConnectorIdURL,
                         IgnoreTariffId,
                         CustomConnectorParser))
            {
                return connector;
            }

            throw new ArgumentException("The given JSON representation of a connector is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out Connector, out ErrorResponse, ConnectorIdURL = null, IgnoreTariffId = false, CustomConnectorParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a connector.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="Connector">The parsed connector.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="ConnectorIdURL">An optional connector identification, e.g. from the HTTP URL.</param>
        /// <param name="IgnoreTariffId">Whether to ignore the 'tariff_id' property.</param>
        /// <param name="CustomConnectorParser">A delegate to parse custom connector JSON objects.</param>
        public static Boolean TryParse(JObject                                  JSON,
                                       [NotNullWhen(true)]  out Connector?      Connector,
                                       [NotNullWhen(false)] out String?         ErrorResponse,
                                       Connector_Id?                            ConnectorIdURL          = null,
                                       Boolean                                  IgnoreTariffId          = false,
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

                #region Parse Id                 [mandatory]

                Connector_Id ConnectorId;

                if (JSON.ParseOptional("id",
                                       "connector identification",
                                       Connector_Id.TryParse,
                                       out Connector_Id? ConnectorIdBody,
                                       out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                    if (!ConnectorIdBody.HasValue) {
                        ErrorResponse = $"The connector identification '{JSON["id"]?.Value<String>() ?? ""}' is invalid!";
                        return false;
                    }

                    ConnectorId = ConnectorIdBody.Value;

                }

                else if (ConnectorIdURL.HasValue)
                    ConnectorId = ConnectorIdURL.Value;

                else
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

                #region Parse Standard           [mandatory]

                if (!JSON.ParseMandatory("standard",
                                         "connector standard",
                                         ConnectorType.TryParse,
                                         out ConnectorType Standard,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Format             [mandatory]

                if (!JSON.ParseMandatory("format",
                                         "connector format",
                                         ConnectorFormatsExtensions.TryParse,
                                         out ConnectorFormats Format,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse PowerType          [mandatory]

                if (!JSON.ParseMandatory("power_type",
                                         "power type",
                                         PowerTypesExtensions.TryParse,
                                         out PowerTypes PowerType,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Voltage            [mandatory]

                if (!JSON.ParseMandatory("voltage",
                                         "voltage",
                                         Volt.TryParse,
                                         out Volt Voltage,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Amperage           [mandatory]

                if (!JSON.ParseMandatory("amperage",
                                         "amperage",
                                         Ampere.TryParse,
                                         out Ampere Amperage,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse TariffId           [optional]

                Tariff_Id? TariffId = null;

                if (!IgnoreTariffId)
                {
                    if (JSON.ParseOptional("tariff_id",
                                           "tariff identification",
                                           Tariff_Id.TryParse,
                                           out TariffId,
                                           out ErrorResponse))
                    {
                        if (ErrorResponse is not null)
                            return false;
                    }
                }

                #endregion

                #region Parse EMSPTariffIds      [optional]   // OCC OCPI Computer Science Extensions

                Dictionary<EMSP_Id, Tariff_Id>? EMSPTariffIds = null;

                if (!IgnoreTariffId)
                {

                    var emspTariffIds = JSON["emsp_tariff_ids"] as JObject;

                    if (emspTariffIds is not null &&
                        emspTariffIds.Count > 0)
                    {
                        foreach (var tariffKVP in emspTariffIds)
                        {
                            if (EMSP_Id.TryParse(tariffKVP.Key, out var emspId))
                            {
                                if (Tariff_Id.TryParse(tariffKVP.Value?.Value<String>() ?? "", out var tariffId))
                                {

                                    EMSPTariffIds ??= [];

                                    if (!EMSPTariffIds.TryAdd(emspId, tariffId))
                                    {
                                        ErrorResponse = $"Could not add EMSP tariff Id '{tariffId}' for EMSP '{emspId}'!";
                                        return false;
                                    }

                                }
                                else
                                {
                                    ErrorResponse = $"Could not parse EMSP tariff Id '{tariffKVP.Value?.Value<String>() ?? ""}' for EMSP '{tariffKVP.Key}'!";
                                    return false;
                                }
                            }
                            else
                            {
                                ErrorResponse = $"Could not parse EMSP identification '{tariffKVP.Key}'!";
                                return false;
                            }
                        }
                    }

                }

                #endregion

                #region TermsAndConditionsURL    [optional]

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

                #region Parse LastUpdated        [mandatory]

                if (!JSON.ParseMandatory("last_updated",
                                         "last updated",
                                         out DateTimeOffset LastUpdated,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                Connector = TariffId.HasValue

                                ? new Connector(
                                      ConnectorId,
                                      Standard,
                                      Format,
                                      PowerType,
                                      Voltage,
                                      Amperage,
                                      TariffId.Value,
                                      TermsAndConditionsURL,
                                      LastUpdated
                                  )

                                : new Connector(
                                      ConnectorId,
                                      Standard,
                                      Format,
                                      PowerType,
                                      Voltage,
                                      Amperage,
                                      EMSPTariffIds,
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

        #region ToJSON(EMSPId = null, CustomConnectorSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="EMSPId">The optional EMSP identification, e.g. for including the right charging tariff.</param>
        /// <param name="CustomConnectorSerializer">A delegate to serialize custom connector JSON objects.</param>
        public JObject ToJSON(EMSP_Id?                                     EMSPId                      = null,
                              CustomJObjectSerializerDelegate<Connector>?  CustomConnectorSerializer   = null)
        {

            var tariffId    = GetTariffId(EMSPId);

            var json        = JSONObject.Create(

                                        new JProperty("id",                     Id.                   ToString()),
                                        new JProperty("standard",               Standard.             ToString()),
                                        new JProperty("format",                 Format.               AsText()),
                                        new JProperty("power_type",             PowerType.            AsText()),
                                        new JProperty("voltage",                Voltage.              IntegerValue),
                                        new JProperty("amperage",               Amperage.             IntegerValue),

                                  tariffId.HasValue
                                      ? new JProperty("tariff_id",              tariffId.             Value.ToString())
                                      : null,

                                  emspTariffIds is not null && !tariffId.HasValue
                                      ? new JProperty("emsp_tariff_ids",        new JObject(
                                                                                    emspTariffIds.Select(
                                                                                        emspTariffId => new JProperty(
                                                                                                            emspTariffId.Key.  ToString(),
                                                                                                            emspTariffId.Value.ToString()
                                                                                                        )
                                                                                    )
                                                                                ))
                                      : null,

                                  TermsAndConditionsURL.HasValue
                                      ? new JProperty("terms_and_conditions",   TermsAndConditionsURL.ToString())
                                      : null,

                                        new JProperty("last_updated",           LastUpdated.          ToISO8601())

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

            => tariffId.HasValue

                   ? new (ParentEVSE,
                          Id,
                          Standard,
                          Format,
                          PowerType,
                          Voltage,
                          Amperage,
                          tariffId.Value,
                          TermsAndConditionsURL,
                          LastUpdated)

                   : new (ParentEVSE,

                          Id.                    Clone(),
                          Standard.              Clone(),
                          Format,
                          PowerType,
                          Voltage,
                          Amperage,
                          emspTariffIds?.ToDictionary(),
                          TermsAndConditionsURL?.Clone(),

                          LastUpdated);

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

            if (!ConnectorPatch.HasValues)
                return PatchResult<Connector>.Failed(EventTrackingId,
                                                     this,
                                                     "The given connector patch must not be null or empty!");

            lock (patchLock)
            {

                if (ConnectorPatch["last_updated"] is null)
                    ConnectorPatch["last_updated"] = Timestamp.Now.ToISO8601();

                else if (AllowDowngrades == false &&
                        ConnectorPatch["last_updated"]?.Type == JTokenType.Date &&
                       (ConnectorPatch["last_updated"]?.Value<DateTime>().ToISO8601().CompareTo(LastUpdated.ToISO8601()) < 1))
                {
                    return PatchResult<Connector>.Failed(EventTrackingId,
                                                         this,
                                                         "The 'lastUpdated' timestamp of the connector patch must be newer then the timestamp of the existing connector!");
                }


                var patchResult = TryPrivatePatch(ToJSON(), ConnectorPatch, EventTrackingId);

                if (patchResult.IsFailed ||
                    patchResult.PatchedData is null)
                {
                    return PatchResult<Connector>.Failed(EventTrackingId,
                                                         this,
                                                         patchResult.ErrorResponse ?? "Unknown error!");
                }

                if (TryParse(patchResult.PatchedData,
                             out var patchedConnector,
                             out var errorResponse) &&
                    patchedConnector is not null)
                {

                    return PatchResult<Connector>.Success(EventTrackingId,patchedConnector,
                                                          errorResponse);

                }

                else
                    return PatchResult<Connector>.Failed(EventTrackingId,this,
                                                         "Invalid JSON merge patch of a connector: " + errorResponse);

            }

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
        /// <param name="Object">A connector to compare with.</param>
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
        /// <param name="Connector">A connector to compare with.</param>
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
                c = Voltage.    CompareTo(Connector.Voltage);

            if (c == 0)
                c = Amperage.   CompareTo(Connector.Amperage);

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
        /// <param name="Object">A connector to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Connector connector &&
                   Equals(connector);

        #endregion

        #region Equals(Connector)

        /// <summary>
        /// Compares two connectors for equality.
        /// </summary>
        /// <param name="Connector">A connector to compare with.</param>
        public Boolean Equals(Connector? Connector)

        {

            var thisTariffId = GetTariffId();
            var thatTariffId = Connector?.GetTariffId();

            return Connector is not null &&

                   Id.                     Equals(Connector.Id)                      &&
                   Standard.               Equals(Connector.Standard)                &&
                   Format.                 Equals(Connector.Format)                  &&
                   PowerType.              Equals(Connector.PowerType)               &&
                   Voltage.                Equals(Connector.Voltage)                 &&
                   Amperage.               Equals(Connector.Amperage)                &&
                   LastUpdated.ToISO8601().Equals(Connector.LastUpdated.ToISO8601()) &&

                ((!thisTariffId.         HasValue && !thatTariffId.                   HasValue) ||
                  (thisTariffId.         HasValue &&  thatTariffId.                   HasValue && thisTariffId.         Value.Equals(thatTariffId.                   Value))) &&

                ((!TermsAndConditionsURL.HasValue && !Connector.TermsAndConditionsURL.HasValue) ||
                  (TermsAndConditionsURL.HasValue &&  Connector.TermsAndConditionsURL.HasValue && TermsAndConditionsURL.Value.Equals(Connector.TermsAndConditionsURL.Value)));

        }

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
        {

            var tariffId = GetTariffId();

            return String.Concat(

                       Id,
                       " (",
                       Standard,           ", ",
                       Format.   AsText(), ", ",
                       PowerType.AsText(), ", ",
                       Voltage,            " V, ",
                       Amperage,           "A",

                       tariffId.HasValue
                           ? ", tariff: " + tariffId.Value.ToString()
                           : "",

                       emspTariffIds is not null
                           ? ", tariffs: " + emspTariffIds.Select(tariffId => $"'{tariffId.Key}':'{tariffId.Value}'").AggregateWith(";")
                           : "",

                       ")"

                   );

        }

        #endregion


    }

}
