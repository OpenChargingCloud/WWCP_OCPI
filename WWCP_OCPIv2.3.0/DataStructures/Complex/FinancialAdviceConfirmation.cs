/*
 * Copyright (c) 2015-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_3_0
{

    /// <summary>
    /// The Financial Advice Confirmation object is utilized to encapsulate the
    /// financial details of transactions processed at payment terminals.
    /// </summary>
    public class FinancialAdviceConfirmation : IEquatable<FinancialAdviceConfirmation>,
                                               IComparable<FinancialAdviceConfirmation>,
                                               IComparable
    {

        #region Properties

        /// <summary>
        /// The unique identification of the financial advice confirmation.
        /// </summary>
        [Mandatory]
        public FinancialAdviceConfirmation_Id  Id                        { get; }

        /// <summary>
        /// The reference to the authorization given by the PTP in the Commands.StartSession.
        /// </summary>
        [Mandatory]
        public AuthorizationReference          AuthorizationReference    { get; }

        /// <summary>
        /// The real amount that was captured at the PSP.
        /// This is a consumer price with VAT.
        /// </summary>
        [Mandatory]
        public Price                           TotalCosts                { get; }

        /// <summary>
        /// The currency of this financial advice confirmation.
        /// </summary>
        [Mandatory]
        public Currency                        Currency                  { get; }

        /// <summary>
        /// The enumeration of invoice relevant data from the direct payment.
        /// </summary>
        [Mandatory]
        public IEnumerable<String>             EFTData                   { get; }

        /// <summary>
        /// The message about about a possible error at the financial advice.
        /// </summary>
        [Mandatory]
        public Capture_StatusCode              CaptureStatusCode         { get; }

        /// <summary>
        /// Message about any error at the financial advice.
        /// </summary>
        [Optional]
        public String?                         CaptureStatusMessage      { get; }


        /// <summary>
        /// The timestamp when this financial advice confirmation was created.
        /// </summary>
        [Mandatory, VendorExtension(VE.GraphDefined)]
        public DateTimeOffset                  Created                   { get; }

        /// <summary>
        /// Timestamp when this financial advice confirmation was last updated (or created).
        /// </summary>
        [Optional]
        public DateTimeOffset                  LastUpdated               { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new Financial Advice Confirmation.
        /// </summary>
        /// <param name="Id">An unique identification of the financial advice confirmation.</param>
        /// <param name="AuthorizationReference">A reference to the authorization given by the PTP in the Commands.StartSession.</param>
        /// <param name="TotalCosts">The real amount that was captured at the PSP. This is a consumer price with VAT.</param>
        /// <param name="Currency">The currency of this financial advice confirmation.</param>
        /// <param name="EFTData">An enumeration of invoice relevant data from the direct payment.</param>
        /// <param name="CaptureStatusCode">A message about about a possible error at the financial advice.</param>
        /// <param name="CaptureStatusMessage"></param>
        /// <param name="Created"></param>
        /// <param name="LastUpdated"></param>
        public FinancialAdviceConfirmation(FinancialAdviceConfirmation_Id  Id,
                                           AuthorizationReference          AuthorizationReference,
                                           Price                           TotalCosts,
                                           Currency                        Currency,
                                           IEnumerable<String>             EFTData,
                                           Capture_StatusCode              CaptureStatusCode,
                                           String?                         CaptureStatusMessage   = null,
                                           DateTimeOffset?                 Created                = null,
                                           DateTimeOffset?                 LastUpdated            = null)
        {

            this.Id                      = Id;
            this.AuthorizationReference  = AuthorizationReference;
            this.TotalCosts              = TotalCosts;
            this.Currency                = Currency;
            this.EFTData                 = EFTData;
            this.CaptureStatusCode       = CaptureStatusCode;
            this.CaptureStatusMessage    = CaptureStatusMessage;

            this.Created                 = Created     ?? LastUpdated ?? Timestamp.Now;
            this.LastUpdated             = LastUpdated ?? Created     ?? Timestamp.Now;

            unchecked
            {

                hashCode = this.Id.                    GetHashCode()       * 23 ^
                           this.AuthorizationReference.GetHashCode()       * 19 ^
                           this.TotalCosts.            GetHashCode()       * 17 ^
                           this.Currency.              GetHashCode()       * 13 ^
                           this.EFTData.               GetHashCode()       * 11 ^
                           this.CaptureStatusCode.     GetHashCode()       *  7 ^
                          (this.CaptureStatusMessage?. GetHashCode() ?? 0) *  5 ^
                           this.Created.               GetHashCode()       *  3 ^
                           this.LastUpdated.           GetHashCode();

            }

        }

        #endregion


        #region (static) Parse   (JSON, CustomFinancialAdviceConfirmationParser = null)

        /// <summary>
        /// Parse the given JSON representation of a Financial Advice Confirmation.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomFinancialAdviceConfirmationParser">A delegate to parse custom Financial Advice Confirmation JSON objects.</param>
        public static FinancialAdviceConfirmation Parse(JObject                                                    JSON,
                                                        CustomJObjectParserDelegate<FinancialAdviceConfirmation>?  CustomFinancialAdviceConfirmationParser   = null)
        {

            if (TryParse(JSON,
                         out var financialAdviceConfirmation,
                         out var errorResponse,
                         CustomFinancialAdviceConfirmationParser))
            {
                return financialAdviceConfirmation;
            }

            throw new ArgumentException("The given JSON representation of a Financial Advice Confirmation is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out FinancialAdviceConfirmation, out ErrorResponse, CustomFinancialAdviceConfirmationParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a Financial Advice Confirmation.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="FinancialAdviceConfirmation">The parsed Financial Advice Confirmation.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                                JSON,
                                       [NotNullWhen(true)]  out FinancialAdviceConfirmation?  FinancialAdviceConfirmation,
                                       [NotNullWhen(false)] out String?                       ErrorResponse)

            => TryParse(JSON,
                        out FinancialAdviceConfirmation,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a Financial Advice Confirmation.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="FinancialAdviceConfirmation">The parsed Financial Advice Confirmation.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomFinancialAdviceConfirmationParser">A delegate to parse custom Financial Advice Confirmation JSON objects.</param>
        public static Boolean TryParse(JObject                                                    JSON,
                                       [NotNullWhen(true)]  out FinancialAdviceConfirmation?      FinancialAdviceConfirmation,
                                       [NotNullWhen(false)] out String?                           ErrorResponse,
                                       CustomJObjectParserDelegate<FinancialAdviceConfirmation>?  CustomFinancialAdviceConfirmationParser   = null)
        {

            try
            {

                FinancialAdviceConfirmation = null;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse Id                        [mandatory]

                if (!JSON.ParseMandatory("id",
                                         "financial advice confirmation identification",
                                         FinancialAdviceConfirmation_Id.TryParse,
                                         out FinancialAdviceConfirmation_Id id,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse AuthorizationReference    [mandatory]

                if (!JSON.ParseMandatory("authorization_reference",
                                         "authorization reference",
                                         AuthorizationReference.TryParse,
                                         out AuthorizationReference authorizationReference,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse TotalCosts                [mandatory]

                if (!JSON.ParseMandatoryJSON("total_costs",
                                             "total costs",
                                             Price.TryParse,
                                             out Price? totalCosts,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Currency                  [mandatory]

                if (!JSON.ParseMandatory("currency",
                                         "currency",
                                         Currency.TryParseISO,
                                         out Currency currency,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse EFTData                   [mandatory]

                if (!JSON.ParseMandatory("eft_data",
                                         "eft data",
                                         out IEnumerable<String> eftData,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse CaptureStatusCode         [mandatory]

                if (!JSON.ParseMandatory("capture_status_code",
                                         "capture status code",
                                         Capture_StatusCode.TryParse,
                                         out Capture_StatusCode captureStatusCode,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse CaptureStatusMessage      [optional]

                var captureStatusMessage = JSON["capture_status_message"]?.Value<String>();

                #endregion

                #region Parse Created                   [optional, VendorExtension]

                if (JSON.ParseOptional("created",
                                       "created",
                                       out DateTimeOffset? created,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse LastUpdated               [mandatory]

                if (!JSON.ParseMandatory("last_updated",
                                         "last updated",
                                         out DateTimeOffset lastUpdated,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                FinancialAdviceConfirmation = new FinancialAdviceConfirmation(
                                                  id,
                                                  authorizationReference,
                                                  totalCosts,
                                                  currency,
                                                  eftData,
                                                  captureStatusCode,
                                                  captureStatusMessage,
                                                  created,
                                                  lastUpdated
                                              );

                if (CustomFinancialAdviceConfirmationParser is not null)
                    FinancialAdviceConfirmation = CustomFinancialAdviceConfirmationParser(JSON,
                                                                  FinancialAdviceConfirmation);

                return true;

            }
            catch (Exception e)
            {
                FinancialAdviceConfirmation  = default;
                ErrorResponse    = "The given JSON representation of a Financial Advice Confirmation is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomFinancialAdviceConfirmationSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomFinancialAdviceConfirmationSerializer">A delegate to serialize custom Financial Advice Confirmation JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<FinancialAdviceConfirmation>?  CustomFinancialAdviceConfirmationSerializer   = null,
                              Boolean                                                        IncludeCreatedTimestamp                       = true)
        {

            var json = JSONObject.Create(

                                 new JProperty("id",                        Id.                    ToString()),
                                 new JProperty("authorization_reference",   AuthorizationReference.ToString()),
                                 new JProperty("total_costs",               TotalCosts.            ToJSON()),
                                 new JProperty("currency",                  Currency.              ISOCode),
                                 new JProperty("eft_data",                  new JArray(EFTData)),
                                 new JProperty("capture_status_code",       CaptureStatusCode.     ToString()),

                           CaptureStatusMessage.IsNotNullOrEmpty()
                               ? new JProperty("capture_status_message",    CaptureStatusMessage)
                               : null,

                           IncludeCreatedTimestamp
                               ? new JProperty("created",                   Created.               ToISO8601())
                               : null,

                                 new JProperty("last_updated",              LastUpdated.           ToISO8601())

                       );

            return CustomFinancialAdviceConfirmationSerializer is not null
                       ? CustomFinancialAdviceConfirmationSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (FinancialAdviceConfirmation1, FinancialAdviceConfirmation2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="FinancialAdviceConfirmation1">A Financial Advice Confirmation.</param>
        /// <param name="FinancialAdviceConfirmation2">Another Financial Advice Confirmation.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (FinancialAdviceConfirmation FinancialAdviceConfirmation1,
                                           FinancialAdviceConfirmation FinancialAdviceConfirmation2)

            => FinancialAdviceConfirmation1.Equals(FinancialAdviceConfirmation2);

        #endregion

        #region Operator != (FinancialAdviceConfirmation1, FinancialAdviceConfirmation2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="FinancialAdviceConfirmation1">A Financial Advice Confirmation.</param>
        /// <param name="FinancialAdviceConfirmation2">Another Financial Advice Confirmation.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (FinancialAdviceConfirmation FinancialAdviceConfirmation1,
                                           FinancialAdviceConfirmation FinancialAdviceConfirmation2)

            => !(FinancialAdviceConfirmation1 == FinancialAdviceConfirmation2);

        #endregion

        #region Operator <  (FinancialAdviceConfirmation1, FinancialAdviceConfirmation2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="FinancialAdviceConfirmation1">A Financial Advice Confirmation.</param>
        /// <param name="FinancialAdviceConfirmation2">Another Financial Advice Confirmation.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (FinancialAdviceConfirmation FinancialAdviceConfirmation1,
                                          FinancialAdviceConfirmation FinancialAdviceConfirmation2)

            => FinancialAdviceConfirmation1.CompareTo(FinancialAdviceConfirmation2) < 0;

        #endregion

        #region Operator <= (FinancialAdviceConfirmation1, FinancialAdviceConfirmation2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="FinancialAdviceConfirmation1">A Financial Advice Confirmation.</param>
        /// <param name="FinancialAdviceConfirmation2">Another Financial Advice Confirmation.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (FinancialAdviceConfirmation FinancialAdviceConfirmation1,
                                           FinancialAdviceConfirmation FinancialAdviceConfirmation2)

            => !(FinancialAdviceConfirmation1 > FinancialAdviceConfirmation2);

        #endregion

        #region Operator >  (FinancialAdviceConfirmation1, FinancialAdviceConfirmation2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="FinancialAdviceConfirmation1">A Financial Advice Confirmation.</param>
        /// <param name="FinancialAdviceConfirmation2">Another Financial Advice Confirmation.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (FinancialAdviceConfirmation FinancialAdviceConfirmation1,
                                          FinancialAdviceConfirmation FinancialAdviceConfirmation2)

            => FinancialAdviceConfirmation1.CompareTo(FinancialAdviceConfirmation2) > 0;

        #endregion

        #region Operator >= (FinancialAdviceConfirmation1, FinancialAdviceConfirmation2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="FinancialAdviceConfirmation1">A Financial Advice Confirmation.</param>
        /// <param name="FinancialAdviceConfirmation2">Another Financial Advice Confirmation.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (FinancialAdviceConfirmation FinancialAdviceConfirmation1,
                                           FinancialAdviceConfirmation FinancialAdviceConfirmation2)

            => !(FinancialAdviceConfirmation1 < FinancialAdviceConfirmation2);

        #endregion

        #endregion

        #region IComparable<FinancialAdviceConfirmation> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two Financial Advice Confirmations.
        /// </summary>
        /// <param name="Object">A Financial Advice Confirmation to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is FinancialAdviceConfirmation financialAdviceConfirmation
                   ? CompareTo(financialAdviceConfirmation)
                   : throw new ArgumentException("The given object is not a Financial Advice Confirmation!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(FinancialAdviceConfirmation)

        /// <summary>
        /// Compares two Financial Advice Confirmations.
        /// </summary>
        /// <param name="FinancialAdviceConfirmation">A Financial Advice Confirmation to compare with.</param>
        public Int32 CompareTo(FinancialAdviceConfirmation? FinancialAdviceConfirmation)
        {

            if (FinancialAdviceConfirmation is null)
                throw new ArgumentNullException(nameof(FinancialAdviceConfirmation), "The given Financial Advice Confirmation must not be null!");

            var c = Id.CompareTo(FinancialAdviceConfirmation.Id);

            if (c == 0)
                c = AuthorizationReference. CompareTo(FinancialAdviceConfirmation.AuthorizationReference);

            if (c == 0)
                c = TotalCosts.             CompareTo(FinancialAdviceConfirmation.TotalCosts);

            if (c == 0)
                c = Currency.               CompareTo(FinancialAdviceConfirmation.Currency);

            if (c == 0)
                c = Created.    ToISO8601().CompareTo(FinancialAdviceConfirmation.Created.    ToISO8601());

            if (c == 0)
                c = LastUpdated.ToISO8601().CompareTo(FinancialAdviceConfirmation.LastUpdated.ToISO8601());

            //if (c == 0 && MinChargingRate.HasValue && FinancialAdviceConfirmation.MinChargingRate.HasValue)
            //    c = MinChargingRate.Value.CompareTo(FinancialAdviceConfirmation.MinChargingRate.Value);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<FinancialAdviceConfirmation> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two Financial Advice Confirmations for equality.
        /// </summary>
        /// <param name="Object">A Financial Advice Confirmation to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is FinancialAdviceConfirmation financialAdviceConfirmation &&
                   Equals(financialAdviceConfirmation);

        #endregion

        #region Equals(FinancialAdviceConfirmation)

        /// <summary>
        /// Compares two Financial Advice Confirmations for equality.
        /// </summary>
        /// <param name="FinancialAdviceConfirmation">A Financial Advice Confirmation to compare with.</param>
        public Boolean Equals(FinancialAdviceConfirmation? FinancialAdviceConfirmation)

            => FinancialAdviceConfirmation is not null &&

               Id.                     Equals(FinancialAdviceConfirmation.Id)                      &&
               AuthorizationReference. Equals(FinancialAdviceConfirmation.AuthorizationReference)  &&
               TotalCosts.             Equals(FinancialAdviceConfirmation.TotalCosts)              &&
               Currency.               Equals(FinancialAdviceConfirmation.Currency)                &&
               CaptureStatusCode.      Equals(FinancialAdviceConfirmation.CaptureStatusCode)       &&
               // CaptureStatusMessage
               Created.    ToISO8601().Equals(FinancialAdviceConfirmation.Created.    ToISO8601()) &&
               LastUpdated.ToISO8601().Equals(FinancialAdviceConfirmation.LastUpdated.ToISO8601()) &&

               EFTData.Count().Equals(FinancialAdviceConfirmation.EFTData.Count()) &&
               EFTData.ToHashSet().SetEquals(FinancialAdviceConfirmation.EFTData.ToHashSet());

        #endregion

        #endregion

        #region (override) GetHashCode()

        private readonly Int32 hashCode;

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()
            => hashCode;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"{Id} ({AuthorizationReference}) for {TotalCosts} {Currency}: {CaptureStatusCode} / {LastUpdated}";

        #endregion

    }

}
