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
using cloud.charging.open.protocols.OCPIv3_0;

#endregion

namespace cloud.charging.open.protocols.OCPIv3_0
{

    /// <summary>
    /// A tariff association object describes which tariff applies at an EVSE
    /// for a certain audience from a certain time onward.
    /// </summary>
    public class TariffAssociation : APartyIssuedObject<TariffAssociation_Id>,
                                     IEquatable<TariffAssociation>,
                                     IComparable<TariffAssociation>,
                                     IComparable
    {

        #region Data

        /// <summary>
        /// The default JSON-LD context of tariffs.
        /// </summary>
        public static readonly JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/contexts/OCPI/3.0/tariffAssociation");

        #endregion

        #region Properties

        /// <summary>
        /// The timestamp at which this Tariff Association comes into (inclusive).
        /// </summary>
        [Mandatory]
        public   DateTimeOffset                   Start          { get; }

        /// <summary>
        /// The ID of the Tariff that is applied by this Tariff Association.
        /// </summary>
        [Mandatory]
        public   Tariff_Id                        TariffId       { get; }

        /// <summary>
        /// The enumeration of tariff elements.
        /// </summary>
        [Mandatory]
        public   IEnumerable<ConnectorReference>  Connectors     { get; }

        /// <summary>
        /// The audience (MSP contract holders, ad-hoc paying drivers, ...)
        /// that the tariff association applies a tariff for.
        /// </summary>
        [Mandatory]
        public   TariffAudience                   Audience       { get; }

        /// <summary>
        /// The timestamp when this tariff association was created.
        /// </summary>
        [Mandatory, VendorExtension(VE.GraphDefined, VE.Pagination)]
        public   DateTimeOffset                   Created        { get; }

        /// <summary>
        /// The timestamp when this tariff association was last updated (or created).
        /// </summary>
        [Mandatory]
        public   DateTimeOffset                   LastUpdated    { get; }

        /// <summary>
        /// The SHA256 hash of the JSON representation of this tariff association.
        /// </summary>
        public   String                           ETag           { get; private set; }

        #endregion

        #region Constructor(s)

        #region TariffAssociation(...)

        /// <summary>
        /// Create a new tariff.
        /// </summary>
        /// <param name="PartyId">The party identification of the party that issued this tariff.</param>
        /// <param name="Id">An identification of the tariff within the party.</param>
        /// <param name="VersionId">The version identification of the tariff.</param>
        /// 
        /// <param name="Start">A timestamp at which this tariff comes into (inclusive).</param>
        /// <param name="TariffId">A tariff identification.</param>
        /// <param name="Connectors">An enumeration of connector references.</param>
        /// <param name="Audience">The audience that the tariff applies for.</param>
        /// 
        /// <param name="Created">An optional timestamp when this tariff was created.</param>
        /// <param name="LastUpdated">An optional timestamp when this tariff was last updated (or created).</param>
        /// 
        /// <param name="CustomTariffAssociationSerializer">A delegate to serialize custom tariff JSON objects.</param>
        /// <param name="CustomConnectorReferenceSerializer">A delegate to serialize custom connector reference JSON objects.</param>
        public TariffAssociation(Party_Idv3                                            PartyId,
                                 TariffAssociation_Id                                  Id,
                                 UInt64                                                VersionId,

                                 DateTimeOffset                                        Start,
                                 Tariff_Id                                             TariffId,
                                 IEnumerable<ConnectorReference>                       Connectors,
                                 TariffAudience                                        Audience,

                                 DateTimeOffset?                                       Created                              = null,
                                 DateTimeOffset?                                       LastUpdated                          = null,

                                 CustomJObjectSerializerDelegate<TariffAssociation>?   CustomTariffAssociationSerializer    = null,
                                 CustomJObjectSerializerDelegate<ConnectorReference>?  CustomConnectorReferenceSerializer   = null)

            : this(null,
                   PartyId,
                   Id,
                   VersionId,

                   Start,
                   TariffId,
                   Connectors,
                   Audience,

                   Created,
                   LastUpdated,

                   CustomTariffAssociationSerializer,
                   CustomConnectorReferenceSerializer)

        { }

        #endregion

        #region (internal) TariffAssociation(CommonAPI, ...)

        /// <summary>
        /// Create a new tariff.
        /// </summary>
        /// <param name="CommonAPI">The OCPI Common API hosting this tariff.</param>
        /// <param name="PartyId">The party identification of the party that issued this tariff.</param>
        /// <param name="Id">An identification of the tariff within the party.</param>
        /// <param name="VersionId">The version identification of the tariff.</param>
        /// 
        /// <param name="Start">A timestamp at which this tariff comes into (inclusive).</param>
        /// <param name="TariffId">A tariff identification.</param>
        /// <param name="Connectors">An enumeration of connector references.</param>
        /// <param name="Audience">The audience that the tariff applies for.</param>
        /// 
        /// <param name="Created">An optional timestamp when this tariff was created.</param>
        /// <param name="LastUpdated">An optional timestamp when this tariff was last updated (or created).</param>
        /// 
        /// <param name="CustomTariffAssociationSerializer">A delegate to serialize custom tariff JSON objects.</param>
        /// <param name="CustomConnectorReferenceSerializer">A delegate to serialize custom connector reference JSON objects.</param>
        internal TariffAssociation(CommonAPI?                                            CommonAPI,
                                   Party_Idv3                                            PartyId,
                                   TariffAssociation_Id                                  Id,
                                   UInt64                                                VersionId,

                                   DateTimeOffset                                        Start,
                                   Tariff_Id                                             TariffId,
                                   IEnumerable<ConnectorReference>                       Connectors,
                                   TariffAudience                                        Audience,

                                   DateTimeOffset?                                       Created                              = null,
                                   DateTimeOffset?                                       LastUpdated                          = null,

                                   CustomJObjectSerializerDelegate<TariffAssociation>?   CustomTariffAssociationSerializer    = null,
                                   CustomJObjectSerializerDelegate<ConnectorReference>?  CustomConnectorReferenceSerializer   = null)

            : base(CommonAPI,
                   PartyId,
                   Id,
                   VersionId)

        {

            if (!Connectors.Any())
                throw new ArgumentNullException(nameof(Connectors),  "The given enumeration of connectors must not be null or empty!");

            this.Start        = Start;
            this.TariffId     = TariffId;
            this.Connectors   = Connectors.Distinct();
            this.Audience     = Audience;

            this.Created      = Created      ?? LastUpdated ?? Timestamp.Now;
            this.LastUpdated  = LastUpdated  ?? Created     ?? Timestamp.Now;

            this.ETag         = ETag = SHA256.HashData(
                                           ToJSON(
                                               true,
                                               true,
                                               true,
                                               true,
                                               CustomTariffAssociationSerializer,
                                               CustomConnectorReferenceSerializer
                                           ).ToUTF8Bytes(Newtonsoft.Json.Formatting.None)
                                       ).ToBase64();

            unchecked
            {

                hashCode = this.PartyId.    GetHashCode()  * 23 ^
                           this.Id.         GetHashCode()  * 19 ^
                           this.VersionId.  GetHashCode()  * 17 ^

                           this.Start.      GetHashCode()  * 13 ^
                           this.TariffId.   GetHashCode()  * 11 ^
                           this.Connectors. CalcHashCode() *  7 ^
                           this.Audience.   GetHashCode()  *  5 ^

                           this.Created.    GetHashCode()  *  3 ^
                           this.LastUpdated.GetHashCode();

            }

        }

        #endregion

        #endregion


        #region (static) Parse   (JSON, ...

        /// <summary>
        /// Parse the given JSON representation of a tariff association.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="PartyIdURL">An optional party identification, e.g. from the HTTP URL.</param>
        /// <param name="TariffAssociationIdURL">An optional tariff association identification, e.g. from the HTTP URL.</param>
        /// <param name="VersionIdURL">An optional version identification, e.g. from the HTTP URL.</param>
        /// <param name="CustomTariffAssociationParser">A delegate to parse custom tariff association JSON objects.</param>
        public static TariffAssociation Parse(JObject                                          JSON,
                                              Party_Idv3?                                      PartyIdURL                      = null,
                                              TariffAssociation_Id?                            TariffAssociationIdURL          = null,
                                              UInt64?                                          VersionIdURL                    = null,
                                              CustomJObjectParserDelegate<TariffAssociation>?  CustomTariffAssociationParser   = null)
        {

            if (TryParse(JSON,
                         out var tariffAssociation,
                         out var errorResponse,
                         PartyIdURL,
                         TariffAssociationIdURL,
                         VersionIdURL,
                         CustomTariffAssociationParser))
            {
                return tariffAssociation;
            }

            throw new ArgumentException("The given JSON representation of a tariff association is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out TariffAssociation, out ErrorResponse, TariffAssociationIdURL = null, CustomTariffAssociationParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a tariff.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="TariffAssociation">The parsed tariff.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                      JSON,
                                       [NotNullWhen(true)]  out TariffAssociation?  TariffAssociation,
                                       [NotNullWhen(false)] out String?             ErrorResponse)

            => TryParse(JSON,
                        out TariffAssociation,
                        out ErrorResponse,
                        null,
                        null,
                        null,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a tariff.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="TariffAssociation">The parsed tariff.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CountryCodeURL">An optional country code, e.g. from the HTTP URL.</param>
        /// <param name="PartyIdURL">An optional party identification, e.g. from the HTTP URL.</param>
        /// <param name="TariffAssociationIdURL">An optional tariff identification, e.g. from the HTTP URL.</param>
        /// <param name="CustomTariffAssociationParser">A delegate to parse custom tariff JSON objects.</param>
        public static Boolean TryParse(JObject                                          JSON,
                                       [NotNullWhen(true)]  out TariffAssociation?      TariffAssociation,
                                       [NotNullWhen(false)] out String?                 ErrorResponse,
                                       Party_Idv3?                                      PartyIdURL                      = null,
                                       TariffAssociation_Id?                            TariffAssociationIdURL          = null,
                                       UInt64?                                          VersionIdURL                    = null,
                                       CustomJObjectParserDelegate<TariffAssociation>?  CustomTariffAssociationParser   = null)
        {

            try
            {

                TariffAssociation = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse PartyId        [optional]

                if (JSON.ParseOptional("party_id",
                                       "party identification",
                                       Party_Idv3.TryParse,
                                       out Party_Idv3? PartyIdBody,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                if (!PartyIdURL.HasValue && !PartyIdBody.HasValue)
                {
                    ErrorResponse = "The party identification is missing!";
                    return false;
                }

                if (PartyIdURL.HasValue && PartyIdBody.HasValue && PartyIdURL.Value != PartyIdBody.Value)
                {
                    ErrorResponse = "The optional party identification given within the JSON body does not match the one given in the URL!";
                    return false;
                }

                #endregion

                #region Parse Id             [optional]

                if (JSON.ParseOptional("id",
                                       "tariff identification",
                                       TariffAssociation_Id.TryParse,
                                       out TariffAssociation_Id? TariffAssociationIdBody,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                if (!TariffAssociationIdURL.HasValue && !TariffAssociationIdBody.HasValue)
                {
                    ErrorResponse = "The tariff identification is missing!";
                    return false;
                }

                if (TariffAssociationIdURL.HasValue && TariffAssociationIdBody.HasValue && TariffAssociationIdURL.Value != TariffAssociationIdBody.Value)
                {
                    ErrorResponse = "The optional tariff identification given within the JSON body does not match the one given in the URL!";
                    return false;
                }

                #endregion

                #region Parse VersionId      [optional]

                if (JSON.ParseOptional("version",
                                       "version identification",
                                       out UInt64? VersionIdBody,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                if (!VersionIdURL.HasValue && !VersionIdBody.HasValue)
                {
                    ErrorResponse = "The version identification is missing!";
                    return false;
                }

                if (VersionIdURL.HasValue && VersionIdBody.HasValue && VersionIdURL.Value != VersionIdBody.Value)
                {
                    ErrorResponse = "The optional version identification given within the JSON body does not match the one given in the URL!";
                    return false;
                }

                #endregion


                #region Parse Start          [mandatory]

                if (!JSON.ParseMandatory("start_date_time",
                                         "start timestamp",
                                         out DateTimeOffset Start,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse TariffId       [mandatory]

                if (!JSON.ParseMandatory("tariff_id",
                                         "tariff identification",
                                         Tariff_Id.TryParse,
                                         out Tariff_Id TariffId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Connectors     [mandatory]

                if (!JSON.ParseMandatoryHashSet("connectors",
                                                "connector references",
                                                ConnectorReference.TryParse,
                                                out HashSet<ConnectorReference> Connectors,
                                                out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Audience       [mandatory]

                if (!JSON.ParseMandatory("audience",
                                         "tariff audience",
                                         TariffAudience.TryParse,
                                         out TariffAudience Audience,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                #region Parse Created        [optional, NonStandard]

                if (JSON.ParseOptional("created",
                                       "created",
                                       out DateTimeOffset? Created,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse LastUpdated    [mandatory]

                if (!JSON.ParseMandatory("last_updated",
                                         "last updated",
                                         out DateTimeOffset LastUpdated,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                TariffAssociation = new TariffAssociation(

                                        null,
                                        PartyIdBody             ?? PartyIdURL!.            Value,
                                        TariffAssociationIdBody ?? TariffAssociationIdURL!.Value,
                                        VersionIdBody           ?? VersionIdURL!.          Value,

                                        Start,
                                        TariffId,
                                        Connectors,
                                        Audience,

                                        Created,
                                        LastUpdated

                                    );

                if (CustomTariffAssociationParser is not null)
                    TariffAssociation = CustomTariffAssociationParser(JSON,
                                                                      TariffAssociation);

                return true;

            }
            catch (Exception e)
            {
                TariffAssociation  = default;
                ErrorResponse      = "The given JSON representation of a tariff association is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(IncludeOwnerInformation = false, IncludeExtensions = false, CustomTariffAssociationSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="IncludeOwnerInformation">Whether to include optional owner information.</param>
        /// <param name="IncludeVersionInformation">Whether to include version information.</param>
        /// <param name="IncludeExtensions">Whether to include optional data model extensions.</param>
        /// <param name="CustomTariffAssociationSerializer">A delegate to serialize custom tariff association JSON objects.</param>
        /// <param name="CustomConnectorReferenceSerializer">A delegate to serialize custom connector reference JSON objects.</param>
        public JObject ToJSON(Boolean                                               IncludeOwnerInformation              = true,
                              Boolean                                               IncludeVersionInformation            = true,
                              Boolean                                               IncludeCreatedTimestamp              = true,
                              Boolean                                               IncludeExtensions                    = true,
                              CustomJObjectSerializerDelegate<TariffAssociation>?   CustomTariffAssociationSerializer    = null,
                              CustomJObjectSerializerDelegate<ConnectorReference>?  CustomConnectorReferenceSerializer   = null)
        {

            var json = JSONObject.Create(

                           IncludeOwnerInformation
                               ? new JProperty("party_id",          PartyId.         ToString())
                               : null,

                                 new JProperty("id",                Id.              ToString()),

                           IncludeVersionInformation
                               ? new JProperty("version",           VersionId.       ToString())
                               : null,


                                 new JProperty("start_date_time",   Start.           ToISO8601()),
                                 new JProperty("tariff_id",         TariffId.        ToString()),
                                 new JProperty("connectors",        Connectors.Select(connectorReference => connectorReference.ToJSON(CustomConnectorReferenceSerializer))),
                                 new JProperty("audience",          Audience.        ToString()),

                           IncludeCreatedTimestamp
                               ? new JProperty("created",           Created.         ToISO8601())
                               : null,

                                 new JProperty("last_updated",      LastUpdated.     ToISO8601())

                       );

            return CustomTariffAssociationSerializer is not null
                       ? CustomTariffAssociationSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this tariff.
        /// </summary>
        public TariffAssociation Clone()

            => new (

                   CommonAPI,
                   PartyId.   Clone(),
                   Id.        Clone(),
                   VersionId,

                   Start,
                   TariffId.  Clone(),
                   Connectors.Select(connectorsReference => connectorsReference.Clone()),
                   Audience.  Clone(),

                   Created,
                   LastUpdated

               );

        #endregion


        #region (private) TryPrivatePatch(JSON, Patch)

        private PatchResult<JObject> TryPrivatePatch(JObject           JSON,
                                                     JObject           Patch,
                                                     EventTracking_Id  EventTrackingId)
        {

            foreach (var property in Patch)
            {

                if      (property.Key == "country_code")
                    return PatchResult<JObject>.Failed(EventTrackingId, JSON,
                                                       "Patching the 'country code' of a tariff is not allowed!");

                else if (property.Key == "party_id")
                    return PatchResult<JObject>.Failed(EventTrackingId, JSON,
                                                       "Patching the 'party identification' of a tariff is not allowed!");

                else if (property.Key == "id")
                    return PatchResult<JObject>.Failed(EventTrackingId, JSON,
                                                       "Patching the 'identification' of a tariff is not allowed!");

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

        #region TryPatch(TariffAssociationPatch, AllowDowngrades = false)

        /// <summary>
        /// Try to patch the JSON representation of this tariff.
        /// </summary>
        /// <param name="TariffAssociationPatch">The JSON merge patch.</param>
        /// <param name="AllowDowngrades">Allow to set the 'lastUpdated' timestamp to an earlier value.</param>
        public PatchResult<TariffAssociation> TryPatch(JObject           TariffAssociationPatch,
                                            Boolean           AllowDowngrades   = false,
                                            EventTracking_Id? EventTrackingId   = null)
        {

            EventTrackingId ??= EventTracking_Id.New;

            if (TariffAssociationPatch is null)
                return PatchResult<TariffAssociation>.Failed(EventTrackingId, this,
                                                  "The given tariff patch must not be null!");

            lock (patchLock)
            {

                if (TariffAssociationPatch["last_updated"] is null)
                    TariffAssociationPatch["last_updated"] = Timestamp.Now.ToISO8601();

                else if (AllowDowngrades == false &&
                        TariffAssociationPatch["last_updated"].Type == JTokenType.Date &&
                       (TariffAssociationPatch["last_updated"].Value<DateTime>().ToISO8601().CompareTo(LastUpdated.ToISO8601()) < 1))
                {
                    return PatchResult<TariffAssociation>.Failed(EventTrackingId, this,
                                                      "The 'lastUpdated' timestamp of the tariff patch must be newer then the timestamp of the existing tariff!");
                }


                var patchResult = TryPrivatePatch(ToJSON(), TariffAssociationPatch, EventTrackingId);


                if (patchResult.IsFailed)
                    return PatchResult<TariffAssociation>.Failed(EventTrackingId, this,
                                                      patchResult.ErrorResponse);

                if (TryParse(patchResult.PatchedData,
                             out var patchedTariffAssociation,
                             out var errorResponse))
                {

                    return PatchResult<TariffAssociation>.Success(EventTrackingId, patchedTariffAssociation,
                                                       errorResponse);

                }

                else
                    return PatchResult<TariffAssociation>.Failed(EventTrackingId, this,
                                                      "Invalid JSON merge patch of a tariff: " + errorResponse);

            }

        }

        #endregion


        #region Operator overloading

        #region Operator == (TariffAssociation1, TariffAssociation2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffAssociation1">A tariff.</param>
        /// <param name="TariffAssociation2">Another tariff.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (TariffAssociation? TariffAssociation1,
                                           TariffAssociation? TariffAssociation2)
        {

            if (Object.ReferenceEquals(TariffAssociation1, TariffAssociation2))
                return true;

            if (TariffAssociation1 is null || TariffAssociation2 is null)
                return false;

            return TariffAssociation1.Equals(TariffAssociation2);

        }

        #endregion

        #region Operator != (TariffAssociation1, TariffAssociation2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffAssociation1">A tariff.</param>
        /// <param name="TariffAssociation2">Another tariff.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (TariffAssociation? TariffAssociation1,
                                           TariffAssociation? TariffAssociation2)

            => !(TariffAssociation1 == TariffAssociation2);

        #endregion

        #region Operator <  (TariffAssociation1, TariffAssociation2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffAssociation1">A tariff.</param>
        /// <param name="TariffAssociation2">Another tariff.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (TariffAssociation? TariffAssociation1,
                                          TariffAssociation? TariffAssociation2)

            => TariffAssociation1 is null
                   ? throw new ArgumentNullException(nameof(TariffAssociation1), "The given tariff must not be null!")
                   : TariffAssociation1.CompareTo(TariffAssociation2) < 0;

        #endregion

        #region Operator <= (TariffAssociation1, TariffAssociation2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffAssociation1">A tariff.</param>
        /// <param name="TariffAssociation2">Another tariff.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (TariffAssociation? TariffAssociation1,
                                           TariffAssociation? TariffAssociation2)

            => !(TariffAssociation1 > TariffAssociation2);

        #endregion

        #region Operator >  (TariffAssociation1, TariffAssociation2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffAssociation1">A tariff.</param>
        /// <param name="TariffAssociation2">Another tariff.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (TariffAssociation? TariffAssociation1,
                                          TariffAssociation? TariffAssociation2)

            => TariffAssociation1 is null
                   ? throw new ArgumentNullException(nameof(TariffAssociation1), "The given tariff must not be null!")
                   : TariffAssociation1.CompareTo(TariffAssociation2) > 0;

        #endregion

        #region Operator >= (TariffAssociation1, TariffAssociation2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffAssociation1">A tariff.</param>
        /// <param name="TariffAssociation2">Another tariff.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (TariffAssociation? TariffAssociation1,
                                           TariffAssociation? TariffAssociation2)

            => !(TariffAssociation1 < TariffAssociation2);

        #endregion

        #endregion

        #region IComparable<TariffAssociation> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two tariff associations.
        /// </summary>
        /// <param name="Object">A tariff association to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is TariffAssociation tariffAssociation
                   ? CompareTo(tariffAssociation)
                   : throw new ArgumentException("The given object is not a tariff association!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(TariffAssociation)

        /// <summary>
        /// Compares two tariff associations.
        /// </summary>
        /// <param name="TariffAssociation">A tariff association to compare with.</param>
        public Int32 CompareTo(TariffAssociation? TariffAssociation)
        {

            if (TariffAssociation is null)
                throw new ArgumentNullException(nameof(TariffAssociation), "The given tariff association must not be null!");

            var c = PartyId.    CompareTo(TariffAssociation.PartyId);

            if (c == 0)
                c = Id.         CompareTo(TariffAssociation.Id);

            if (c == 0)
                c = VersionId.  CompareTo(TariffAssociation.VersionId);


            if (c == 0)
                c = Start.      CompareTo(TariffAssociation.Start);

            if (c == 0)
                c = TariffId.   CompareTo(TariffAssociation.TariffId);

            if (c == 0)
                c = Audience.   CompareTo(TariffAssociation.Audience);


            if (c == 0)
                c = Created.    CompareTo(TariffAssociation.Created);

            if (c == 0)
                c = LastUpdated.CompareTo(TariffAssociation.LastUpdated);

            // TariffAssociationElements
            // 
            // TariffAssociationAltText
            // TariffAssociationAltURL
            // MinPrice
            // MaxPrice
            // Start
            // End
            // EnergyMix

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<TariffAssociation> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two tariff associations for equality.
        /// </summary>
        /// <param name="Object">A tariff association to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is TariffAssociation tariffAssociation &&
                   Equals(tariffAssociation);

        #endregion

        #region Equals(TariffAssociation)

        /// <summary>
        /// Compares two tariff associations for equality.
        /// </summary>
        /// <param name="TariffAssociation">A tariff association to compare with.</param>
        public Boolean Equals(TariffAssociation? TariffAssociation)

            => TariffAssociation is not null &&

               PartyId.                Equals(TariffAssociation.PartyId)   &&
               Id.                     Equals(TariffAssociation.Id)        &&
               VersionId.              Equals(TariffAssociation.VersionId) &&

               Start.                  Equals(TariffAssociation.Start)     &&
               TariffId.               Equals(TariffAssociation.TariffId)  &&
               Audience.               Equals(TariffAssociation.Audience)  &&

               Created.    ToISO8601().Equals(TariffAssociation.Created.    ToISO8601()) &&
               LastUpdated.ToISO8601().Equals(TariffAssociation.LastUpdated.ToISO8601());

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

            => $"{PartyId}:{Id} ({VersionId}, {LastUpdated.ToISO8601()}) {TariffId} {Audience} {Start.ToISO8601()}: {Connectors.AggregateWith(", ")}";

        #endregion


    }

}
