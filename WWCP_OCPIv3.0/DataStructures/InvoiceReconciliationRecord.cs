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

using System.Text;
using System.Security.Cryptography;
using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPI;
using cloud.charging.open.protocols.OCPIv3_0.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv3_0
{

    /// <summary>
    /// The invoice reconciliation record describes the charging session and its costs,
    /// how these costs are composed, etc.
    /// </summary>
    public class InvoiceReconciliationRecord : APartyIssuedObject<Invoice_Id>,
                                               IEquatable<InvoiceReconciliationRecord>,
                                               IComparable<InvoiceReconciliationRecord>,
                                               IComparable
    {

        #region Data

        /// <summary>
        /// The default JSON-LD context of locations.
        /// </summary>
        public static readonly JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/contexts/OCPI/3.0/invoiceReconciliationRecord");

        #endregion

        #region Properties

        /// <summary>
        /// The unique CDR identifiers of the CDRs that are invoiced by the
        /// invoice identified by the value of the invoice identification.
        /// </summary>
        [Mandatory]
        public   IEnumerable<CDR_Id>  CDRIds         { get; }


        /// <summary>
        /// The timestamp when this invoice reconciliation record was created.
        /// </summary>
        [Mandatory, NonStandard("Pagination")]
        public   DateTime             Created        { get; }

        /// <summary>
        /// The timestamp when this invoice reconciliation record was last updated (or created).
        /// </summary>
        [Mandatory]
        public   DateTime             LastUpdated    { get; }

        /// <summary>
        /// The SHA256 hash of the JSON representation of this invoice reconciliation record.
        /// </summary>
        public   String               ETag           { get; }

        #endregion

        #region Constructor(s)

        #region InvoiceReconciliationRecord(...)

        /// <summary>
        /// Create a new invoice reconciliation record.
        /// </summary>
        /// <param name="PartyId">The party identification of the party that issued this invoice reconciliation record.</param>
        /// <param name="Id">An identification of the invoice reconciliation record within the charge point operator's platform (and suboperator platforms).</param>
        /// <param name="VersionId">The version identification of the invoice reconciliation record.</param>
        /// 
        /// <param name="CDRIds">The unique CDR identifiers of the CDRs that are invoiced by the invoice identified by the value of the invoice identification.</param>
        /// 
        /// <param name="Created">An optional timestamp when this invoice reconciliation record was created.</param>
        /// <param name="LastUpdated">An optional timestamp when this invoice reconciliation record was last updated (or created).</param>
        /// 
        /// <param name="CustomInvoiceReconciliationRecordSerializer">A delegate to serialize custom invoice reconciliation record JSON objects.</param>
        public InvoiceReconciliationRecord(Party_Idv3                                                     PartyId,
                                           Invoice_Id                                                     Id,
                                           UInt64                                                         VersionId,

                                           IEnumerable<CDR_Id>                                            CDRIds,

                                           DateTime?                                                      Created                                       = null,
                                           DateTime?                                                      LastUpdated                                   = null,

                                           CustomJObjectSerializerDelegate<InvoiceReconciliationRecord>?  CustomInvoiceReconciliationRecordSerializer   = null)

            : this(null,
                   PartyId,
                   Id,
                   VersionId,

                   CDRIds,

                   Created,
                   LastUpdated,

                   CustomInvoiceReconciliationRecordSerializer)

        { }

        #endregion

        #region (internal) InvoiceReconciliationRecord(CommonAPI, ...)

        /// <summary>
        /// Create a new invoice reconciliation record.
        /// </summary>
        /// <param name="CommonAPI">The OCPI Common API hosting this invoice reconciliation record.</param>
        /// <param name="PartyId">The party identification of the party that issued this invoice reconciliation record.</param>
        /// <param name="Id">An identification of the invoice reconciliation record within the charge point operator's platform (and suboperator platforms).</param>
        /// <param name="VersionId">The version identification of the invoice reconciliation record.</param>
        /// 
        /// <param name="CDRIds">The unique CDR identifiers of the CDRs that are invoiced by the invoice identified by the value of the invoice identification.</param>
        /// 
        /// <param name="Created">An optional timestamp when this invoice reconciliation record was created.</param>
        /// <param name="LastUpdated">An optional timestamp when this invoice reconciliation record was last updated (or created).</param>
        /// 
        /// <param name="CustomInvoiceReconciliationRecordSerializer">A delegate to serialize custom invoice reconciliation record JSON objects.</param>
        public InvoiceReconciliationRecord(CommonAPI?                                                     CommonAPI,
                                           Party_Idv3                                                     PartyId,
                                           Invoice_Id                                                     Id,
                                           UInt64                                                         VersionId,

                                           IEnumerable<CDR_Id>                                            CDRIds,

                                           DateTime?                                                      Created                                       = null,
                                           DateTime?                                                      LastUpdated                                   = null,

                                           CustomJObjectSerializerDelegate<InvoiceReconciliationRecord>?  CustomInvoiceReconciliationRecordSerializer   = null)

            : base(CommonAPI,
                   PartyId,
                   Id,
                   VersionId)

        {

            if (!CDRIds.Any())
                throw new ArgumentNullException(nameof(CDRIds),  "The given enumeration of CDR identifications must not be empty!");

            this.CDRIds       = CDRIds.Distinct();

            this.Created      = Created               ?? LastUpdated ?? Timestamp.Now;
            this.LastUpdated  = LastUpdated           ?? Created     ?? Timestamp.Now;

            this.ETag         = SHA256.HashData(
                                    ToJSON(
                                        true,
                                        true,
                                        true,
                                        true,
                                        CustomInvoiceReconciliationRecordSerializer
                                    ).ToUTF8Bytes(Newtonsoft.Json.Formatting.None)
                                ).ToBase64();

            unchecked
            {

                hashCode = this.PartyId.    GetHashCode()  * 17 ^
                           this.Id.         GetHashCode()  * 13 ^
                           this.VersionId.  GetHashCode()  * 11 ^

                           this.CDRIds.     CalcHashCode() *  7 ^

                           this.Created.    GetHashCode()  *  5 ^
                           this.LastUpdated.GetHashCode()  *  3;

            }

        }

        #endregion

        #endregion


        #region (static) Parse   (JSON, ...)

        /// <summary>
        /// Parse the given JSON representation of a InvoiceReconciliationRecord.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="PartyIdURL">An optional party identification, e.g. from the HTTP URL.</param>
        /// <param name="InvoiceIdURL">An optional invoice identification, e.g. from the HTTP URL.</param>
        /// <param name="VersionIdURL">An optional version identification, e.g. from the HTTP URL.</param>
        /// <param name="CustomInvoiceReconciliationRecordParser">A delegate to parse custom invoice reconciliation record JSON objects.</param>
        public static InvoiceReconciliationRecord Parse(JObject                                                    JSON,
                                                        Party_Idv3?                                                PartyIdURL                                = null,
                                                        Invoice_Id?                                                InvoiceIdURL                              = null,
                                                        UInt64?                                                    VersionIdURL                              = null,
                                                        CustomJObjectParserDelegate<InvoiceReconciliationRecord>?  CustomInvoiceReconciliationRecordParser   = null)
        {

            if (TryParse(JSON,
                         out var InvoiceReconciliationRecord,
                         out var errorResponse,
                         PartyIdURL,
                         InvoiceIdURL,
                         VersionIdURL,
                         CustomInvoiceReconciliationRecordParser))
            {
                return InvoiceReconciliationRecord;
            }

            throw new ArgumentException("The given JSON representation of an invoice reconciliation record is invalid: " + errorResponse, nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out InvoiceReconciliationRecord, out ErrorResponse, ...)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a InvoiceReconciliationRecord.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="InvoiceReconciliationRecord">The parsed InvoiceReconciliationRecord.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                                JSON,
                                       [NotNullWhen(true)]  out InvoiceReconciliationRecord?  InvoiceReconciliationRecord,
                                       [NotNullWhen(false)] out String?                       ErrorResponse)

            => TryParse(JSON,
                        out InvoiceReconciliationRecord,
                        out ErrorResponse,
                        null,
                        null,
                        null,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a InvoiceReconciliationRecord.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="InvoiceReconciliationRecord">The parsed InvoiceReconciliationRecord.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="PartyIdURL">An optional party identification, e.g. from the HTTP URL.</param>
        /// <param name="InvoiceIdURL">An optional invoice identification, e.g. from the HTTP URL.</param>
        /// <param name="VersionIdURL">An optional version identification, e.g. from the HTTP URL.</param>
        /// <param name="CustomInvoiceReconciliationRecordParser">A delegate to parse custom InvoiceReconciliationRecord JSON objects.</param>
        public static Boolean TryParse(JObject                                                    JSON,
                                       [NotNullWhen(true)]  out InvoiceReconciliationRecord?      InvoiceReconciliationRecord,
                                       [NotNullWhen(false)] out String?                           ErrorResponse,
                                       Party_Idv3?                                                PartyIdURL                                = null,
                                       Invoice_Id?                                                InvoiceIdURL                              = null,
                                       UInt64?                                                    VersionIdURL                              = null,
                                       CustomJObjectParserDelegate<InvoiceReconciliationRecord>?  CustomInvoiceReconciliationRecordParser   = null)
        {

            try
            {

                InvoiceReconciliationRecord = default;

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
                                       "Invoice identification",
                                       Invoice_Id.TryParse,
                                       out Invoice_Id? InvoiceIdBody,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                if (!InvoiceIdURL.HasValue && !InvoiceIdBody.HasValue)
                {
                    ErrorResponse = "The Invoice identification is missing!";
                    return false;
                }

                if (InvoiceIdURL.HasValue && InvoiceIdBody.HasValue && InvoiceIdURL.Value != InvoiceIdBody.Value)
                {
                    ErrorResponse = "The optional Invoice identification given within the JSON body does not match the one given in the URL!";
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


                #region Parse CDRIds         [optional]

                if (!JSON.ParseMandatoryHashSet("invoiceReconciliationRecord_ids",
                                                "charge detail record identifications",
                                                CDR_Id.TryParse,
                                                out HashSet<CDR_Id> CDRIds,
                                                out ErrorResponse))
                {
                    return false;
                }

                #endregion


                #region Parse Created        [optional, NonStandard]

                if (JSON.ParseOptional("created",
                                       "created",
                                       out DateTime? Created,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse LastUpdated    [mandatory]

                if (!JSON.ParseMandatory("last_updated",
                                         "last updated",
                                         out DateTime LastUpdated,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                InvoiceReconciliationRecord = new InvoiceReconciliationRecord(

                                                  null,
                                                  PartyIdBody    ?? PartyIdURL!.  Value,
                                                  InvoiceIdBody  ?? InvoiceIdURL!.Value,
                                                  VersionIdBody  ?? VersionIdURL!.Value,

                                                  CDRIds,

                                                  Created,
                                                  LastUpdated

                                              );


                if (CustomInvoiceReconciliationRecordParser is not null)
                    InvoiceReconciliationRecord = CustomInvoiceReconciliationRecordParser(JSON,
                                                                                          InvoiceReconciliationRecord);

                return true;

            }
            catch (Exception e)
            {
                InvoiceReconciliationRecord  = default;
                ErrorResponse                = "The given JSON representation of a InvoiceReconciliationRecord is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomInvoiceReconciliationRecordSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="IncludeOwnerInformation">Whether to include optional owner information.</param>
        /// <param name="IncludeVersionInformation">Whether to include version information.</param>
        /// <param name="IncludeCreatedTimestamp">Whether to include a timestamp of when this location was created.</param>
        /// <param name="IncludeExtensions">Whether to include optional data model extensions.</param>
        /// <param name="CustomInvoiceReconciliationRecordSerializer">A delegate to serialize custom invoice reconciliation record JSON objects.</param>
        public JObject ToJSON(Boolean                                                        IncludeOwnerInformation                       = true,
                              Boolean                                                        IncludeVersionInformation                     = true,
                              Boolean                                                        IncludeCreatedTimestamp                       = true,
                              Boolean                                                        IncludeExtensions                             = true,
                              CustomJObjectSerializerDelegate<InvoiceReconciliationRecord>?  CustomInvoiceReconciliationRecordSerializer   = null)
        {

            var json = JSONObject.Create(

                           IncludeOwnerInformation
                               ? new JProperty("party_id",       PartyId.    ToString())
                               : null,

                                 new JProperty("id",             Id.         ToString()),

                           IncludeVersionInformation
                               ? new JProperty("version",        VersionId.  ToString())
                               : null,


                                 new JProperty("invoiceReconciliationRecord_ids",        new JArray(CDRIds.Select(invoiceReconciliationRecordId => invoiceReconciliationRecordId.ToString()))),


                           IncludeCreatedTimestamp
                               ? new JProperty("created",        Created.    ToIso8601())
                               : null,

                                 new JProperty("last_updated",   LastUpdated.ToIso8601())

                       );

            return CustomInvoiceReconciliationRecordSerializer is not null
                       ? CustomInvoiceReconciliationRecordSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this invoice reconciliation record.
        /// </summary>
        public InvoiceReconciliationRecord Clone()

            => new (

                   CommonAPI,
                   PartyId.Clone(),
                   Id.     Clone(),
                   VersionId,

                   CDRIds.Select(invoiceReconciliationRecordId => invoiceReconciliationRecordId.Clone()),

                   Created,
                   LastUpdated

               );

        #endregion


        #region (private) TryPrivatePatch(JSON, Patch)

        private PatchResult<JObject> TryPrivatePatch(JObject  JSON,
                                                     JObject  Patch,
                                                     EventTracking_Id  EventTrackingId)
        {

            foreach (var property in Patch)
            {

                if      (property.Key == "country_code")
                    return PatchResult<JObject>.Failed(EventTrackingId, JSON,
                                                       "Patching the 'country code' of an invoice reconciliation record is not allowed!");

                else if (property.Key == "party_id")
                    return PatchResult<JObject>.Failed(EventTrackingId, JSON,
                                                       "Patching the 'party identification' of an invoice reconciliation record is not allowed!");

                else if (property.Key == "id")
                    return PatchResult<JObject>.Failed(EventTrackingId, JSON,
                                                       "Patching the 'identification' of an invoice reconciliation record is not allowed!");

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

        #region TryPatch(InvoiceReconciliationRecordPatch, AllowDowngrades = false)

        /// <summary>
        /// Try to patch the JSON representaion of this invoice reconciliation record.
        /// </summary>
        /// <param name="InvoiceReconciliationRecordPatch">The JSON merge patch.</param>
        /// <param name="AllowDowngrades">Allow to set the 'lastUpdated' timestamp to an earlier value.</param>
        public PatchResult<InvoiceReconciliationRecord> TryPatch(JObject           InvoiceReconciliationRecordPatch,
                                         Boolean           AllowDowngrades   = false,
                                         EventTracking_Id? EventTrackingId   = null)
        {

            EventTrackingId ??= EventTracking_Id.New;

            if (InvoiceReconciliationRecordPatch is null)
                return PatchResult<InvoiceReconciliationRecord>.Failed(EventTrackingId, this,
                                                  "The given invoice reconciliation record patch must not be null!");

            lock (patchLock)
            {

                if (InvoiceReconciliationRecordPatch["last_updated"] is null)
                    InvoiceReconciliationRecordPatch["last_updated"] = Timestamp.Now.ToIso8601();

                else if (AllowDowngrades == false &&
                        InvoiceReconciliationRecordPatch["last_updated"].Type == JTokenType.Date &&
                       (InvoiceReconciliationRecordPatch["last_updated"].Value<DateTime>().ToIso8601().CompareTo(LastUpdated.ToIso8601()) < 1))
                {
                    return PatchResult<InvoiceReconciliationRecord>.Failed(EventTrackingId, this,
                                                      "The 'lastUpdated' timestamp of the invoice reconciliation record patch must be newer then the timestamp of the existing invoice reconciliation record!");
                }


                var patchResult = TryPrivatePatch(ToJSON(), InvoiceReconciliationRecordPatch, EventTrackingId);


                if (patchResult.IsFailed)
                    return PatchResult<InvoiceReconciliationRecord>.Failed(EventTrackingId, this,
                                                      patchResult.ErrorResponse);

                if (TryParse(patchResult.PatchedData,
                             out var patchedInvoiceReconciliationRecord,
                             out var errorResponse))
                {

                    return PatchResult<InvoiceReconciliationRecord>.Success(EventTrackingId, patchedInvoiceReconciliationRecord,
                                                       errorResponse);

                }

                else
                    return PatchResult<InvoiceReconciliationRecord>.Failed(EventTrackingId, this,
                                                      "Invalid JSON merge patch of an invoice reconciliation record: " + errorResponse);

            }

        }

        #endregion


        #region Operator overloading

        #region Operator == (InvoiceReconciliationRecord1, InvoiceReconciliationRecord2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="InvoiceReconciliationRecord1">An invoice reconciliation record.</param>
        /// <param name="InvoiceReconciliationRecord2">Another invoice reconciliation record.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (InvoiceReconciliationRecord? InvoiceReconciliationRecord1,
                                           InvoiceReconciliationRecord? InvoiceReconciliationRecord2)
        {

            if (Object.ReferenceEquals(InvoiceReconciliationRecord1, InvoiceReconciliationRecord2))
                return true;

            if (InvoiceReconciliationRecord1 is null || InvoiceReconciliationRecord2 is null)
                return false;

            return InvoiceReconciliationRecord1.Equals(InvoiceReconciliationRecord2);

        }

        #endregion

        #region Operator != (InvoiceReconciliationRecord1, InvoiceReconciliationRecord2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="InvoiceReconciliationRecord1">An invoice reconciliation record.</param>
        /// <param name="InvoiceReconciliationRecord2">Another invoice reconciliation record.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (InvoiceReconciliationRecord? InvoiceReconciliationRecord1,
                                           InvoiceReconciliationRecord? InvoiceReconciliationRecord2)

            => !(InvoiceReconciliationRecord1 == InvoiceReconciliationRecord2);

        #endregion

        #region Operator <  (InvoiceReconciliationRecord1, InvoiceReconciliationRecord2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="InvoiceReconciliationRecord1">An invoice reconciliation record.</param>
        /// <param name="InvoiceReconciliationRecord2">Another invoice reconciliation record.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (InvoiceReconciliationRecord? InvoiceReconciliationRecord1,
                                          InvoiceReconciliationRecord? InvoiceReconciliationRecord2)

            => InvoiceReconciliationRecord1 is null
                   ? throw new ArgumentNullException(nameof(InvoiceReconciliationRecord1), "The given invoice reconciliation record must not be null!")
                   : InvoiceReconciliationRecord1.CompareTo(InvoiceReconciliationRecord2) < 0;

        #endregion

        #region Operator <= (InvoiceReconciliationRecord1, InvoiceReconciliationRecord2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="InvoiceReconciliationRecord1">An invoice reconciliation record.</param>
        /// <param name="InvoiceReconciliationRecord2">Another invoice reconciliation record.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (InvoiceReconciliationRecord? InvoiceReconciliationRecord1,
                                           InvoiceReconciliationRecord? InvoiceReconciliationRecord2)

            => !(InvoiceReconciliationRecord1 > InvoiceReconciliationRecord2);

        #endregion

        #region Operator >  (InvoiceReconciliationRecord1, InvoiceReconciliationRecord2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="InvoiceReconciliationRecord1">An invoice reconciliation record.</param>
        /// <param name="InvoiceReconciliationRecord2">Another invoice reconciliation record.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (InvoiceReconciliationRecord? InvoiceReconciliationRecord1,
                                          InvoiceReconciliationRecord? InvoiceReconciliationRecord2)

            => InvoiceReconciliationRecord1 is null
                   ? throw new ArgumentNullException(nameof(InvoiceReconciliationRecord1), "The given invoice reconciliation record must not be null!")
                   : InvoiceReconciliationRecord1.CompareTo(InvoiceReconciliationRecord2) > 0;

        #endregion

        #region Operator >= (InvoiceReconciliationRecord1, InvoiceReconciliationRecord2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="InvoiceReconciliationRecord1">An invoice reconciliation record.</param>
        /// <param name="InvoiceReconciliationRecord2">Another invoice reconciliation record.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (InvoiceReconciliationRecord? InvoiceReconciliationRecord1,
                                           InvoiceReconciliationRecord? InvoiceReconciliationRecord2)

            => !(InvoiceReconciliationRecord1 < InvoiceReconciliationRecord2);

        #endregion

        #endregion

        #region IComparable<InvoiceReconciliationRecord> Members

        #region CompareTo(Object)

        /// <summary>s
        /// Compares two invoice reconciliation records.
        /// </summary>
        /// <param name="Object">An invoice reconciliation record to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is InvoiceReconciliationRecord invoiceReconciliationRecord
                   ? CompareTo(invoiceReconciliationRecord)
                   : throw new ArgumentException("The given object is not an invoice reconciliation record!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(InvoiceReconciliationRecord)

        /// <summary>s
        /// Compares two invoice reconciliation records.
        /// </summary>
        /// <param name="InvoiceReconciliationRecord">An invoice reconciliation record to compare with.</param>
        public Int32 CompareTo(InvoiceReconciliationRecord? InvoiceReconciliationRecord)
        {

            if (InvoiceReconciliationRecord is null)
                throw new ArgumentNullException(nameof(InvoiceReconciliationRecord), "The given invoice reconciliation record must not be null!");

            var c = PartyId.    CompareTo(InvoiceReconciliationRecord.PartyId);

            if (c == 0)
                c = Id.         CompareTo(InvoiceReconciliationRecord.Id);

            if (c == 0)
                c = VersionId.  CompareTo(InvoiceReconciliationRecord.VersionId);


            if (c == 0)
                c = Created.    CompareTo(InvoiceReconciliationRecord.Created);

            if (c == 0)
                c = LastUpdated.CompareTo(InvoiceReconciliationRecord.LastUpdated);

            if (c == 0)
                c = ETag.       CompareTo(InvoiceReconciliationRecord.ETag);


            if (c == 0)
                c = CDRIds.     SequenceEqual(InvoiceReconciliationRecord.CDRIds) ? 0 : 1;

            return c;

        }



        #endregion

        #endregion

        #region IEquatable<InvoiceReconciliationRecord> Members

        #region Equals(Object)

        /// <summary>s
        /// Compares two invoice reconciliation records for equality.
        /// </summary>
        /// <param name="Object">An invoice reconciliation record to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is InvoiceReconciliationRecord invoiceReconciliationRecord &&
                   Equals(invoiceReconciliationRecord);

        #endregion

        #region Equals(InvoiceReconciliationRecord)

        /// <summary>s
        /// Compares two invoice reconciliation records for equality.
        /// </summary>
        /// <param name="InvoiceReconciliationRecord">An invoice reconciliation record to compare with.</param>
        public Boolean Equals(InvoiceReconciliationRecord? InvoiceReconciliationRecord)

            => InvoiceReconciliationRecord is not null &&

               PartyId.                Equals       (InvoiceReconciliationRecord.PartyId)   &&
               Id.                     Equals       (InvoiceReconciliationRecord.Id)        &&
               VersionId.              Equals       (InvoiceReconciliationRecord.VersionId) &&

               CDRIds.                 SequenceEqual(InvoiceReconciliationRecord.CDRIds)    &&

               Created.    ToIso8601().Equals       (InvoiceReconciliationRecord.Created.    ToIso8601()) &&
               LastUpdated.ToIso8601().Equals       (InvoiceReconciliationRecord.LastUpdated.ToIso8601());

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

                   $"{PartyId}:{Id} ({VersionId}, {LastUpdated.ToIso8601()})",

                   CDRIds.Count(), " CDR Ids, ",
                   CDRIds.AggregateWith(", "),

                   LastUpdated.ToIso8601()

               );

        #endregion


        #region ToBuilder(NewInvoiceId = null, NewVersionId = null)

        /// <summary>
        /// Return a builder for this invoice reconciliation record.
        /// </summary>
        /// <param name="NewInvoiceId">An optional new invoice reconciliation record identification.</param>
        /// <param name="NewVersionId">An optional new version identification.</param>
        public Builder ToBuilder(Invoice_Id?  NewInvoiceId   = null,
                                 UInt64?      NewVersionId   = null)

            => new (

                   CommonAPI,
                   PartyId,
                   NewInvoiceId ?? Id,
                   NewVersionId ?? VersionId,

                   CDRIds,

                   Created,
                   LastUpdated

               );

        #endregion

        #region (class) Builder

        /// <summary>
        /// An invoice reconciliation record builder.
        /// </summary>
        public class Builder : ABuilder
        {

            #region Properties

            /// <summary>
            /// The unique CDR identifiers of the CDRs that are invoiced by the
            /// invoice identified by the value of the invoice identification.
            /// </summary>
            [Mandatory]
            public   HashSet<CDR_Id>  CDRIds         { get; }

            /// <summary>
            /// The timestamp when this invoice reconciliation record was created.
            /// </summary>
            [Mandatory, NonStandard("Pagination")]
            public   DateTime?        Created        { get; set; }

            /// <summary>
            /// The timestamp when this invoice reconciliation record was last updated (or created).
            /// </summary>
            [Mandatory]
            public   DateTime?        LastUpdated    { get; set; }

            #endregion

            #region Constructor(s)

            /// <summary>
            /// Create a new invoice reconciliation record builder.
            /// </summary>
            /// <param name="CommonAPI">The OCPI Common API hosting this invoice reconciliation record.</param>
            /// <param name="PartyId">The party identification of the party that issued this invoice reconciliation record.</param>
            /// <param name="Id">An identification of the invoice reconciliation record within the charge point operator's platform (and suboperator platforms).</param>
            /// <param name="VersionId">The version identification of the invoice reconciliation record.</param>
            /// 
            /// <param name="CDRIds">The unique CDR identifiers of the CDRs that are invoiced by the invoice identified by the value of the invoice identification.</param>
            /// 
            /// <param name="Created">An optional timestamp when this invoice reconciliation record was created.</param>
            /// <param name="LastUpdated">An optional timestamp when this invoice reconciliation record was last updated (or created).</param>
            internal Builder(CommonAPI?            CommonAPI     = null,
                             Party_Idv3?           PartyId       = null,
                             Invoice_Id?           Id            = null,
                             UInt64?               VersionId     = null,

                             IEnumerable<CDR_Id>?  CDRIds        = null,

                             DateTime?             Created       = null,
                             DateTime?             LastUpdated   = null)

                : base(CommonAPI,
                       PartyId,
                       Id,
                       VersionId)

            {

                this.CDRIds       = CDRIds is not null ? new HashSet<CDR_Id>(CDRIds) : [];

                this.Created      = Created     ?? LastUpdated;
                this.LastUpdated  = LastUpdated ?? Created;

            }

            #endregion

            #region ToImmutable

            /// <summary>
            /// Return an immutable version of the invoice reconciliation record.
            /// </summary>
            public static implicit operator InvoiceReconciliationRecord?(Builder? Builder)

                => Builder?.ToImmutable(out _);


            /// <summary>
            /// Return an immutable version of the invoice reconciliation record.
            /// </summary>
            /// <param name="Warnings"></param>
            public InvoiceReconciliationRecord? ToImmutable(out IEnumerable<Warning> Warnings)
            {

                var warnings = new List<Warning>();

                //if (!PartyId.    HasValue)
                //    throw new ArgumentNullException(nameof(PartyId),    "The party identification of the charging station must not be null or empty!");

                //if (!Id.         HasValue)
                //    throw new ArgumentNullException(nameof(Id),         "The charging station identification must not be null or empty!");

                //if (!VersionId.  HasValue)
                //    throw new ArgumentNullException(nameof(VersionId),  "The version identification of the charging station must not be null or empty!");

                if (!PartyId.  HasValue)
                    warnings.Add(Warning.Create("The party identification of the charging station must not be null or empty!"));

                if (!Id.       HasValue)
                    warnings.Add(Warning.Create("The charging station identification must not be null or empty!"));

                if (!VersionId.HasValue)
                    warnings.Add(Warning.Create("The version identification of the charging station must not be null or empty!"));

                Warnings = warnings;

                return warnings.Count != 0

                           ? null

                           : new InvoiceReconciliationRecord(

                                 null,
                                 PartyId.            Value,
                                 Id.                 Value,
                                 VersionId.          Value,

                                 CDRIds,

                                 Created     ?? Timestamp.Now,
                                 LastUpdated ?? Timestamp.Now

                             );

            }

            #endregion

        }

        #endregion


    }

}
