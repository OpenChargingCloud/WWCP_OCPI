/*
 * Copyright (c) 2015-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPI <https://github.com/OpenChargingCloud/WWCP_OCPI>
 *
 * Licensed under the Apache License, Auth 2.0 (the "License");
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

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv3_0
{

    /// <summary>
    /// A certificate renewal request.
    /// </summary>
    public class CertificateRenewalRequest
    {

        #region Properties

        /// <summary>
        /// The certificate signing request according to RFC 2986 ([CSR]), encoded according to PEM([PEM]).
        /// </summary>
        public String  CertificateSigningRequest    { get; }

        /// <summary>
        /// The callback Id for the other party to create a URL to post the signed certificate.
        /// </summary>
        public String  CallbackId                   { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new certificate renewal request.
        /// </summary>
        /// <param name="CertificateSigningRequest">The certificate signing request according to RFC 2986 ([CSR]), encoded according to PEM([PEM]).</param>
        /// <param name="CallbackId">The callback Id for the other party to create a URL to post the signed certificate.</param>
        public CertificateRenewalRequest(String CertificateSigningRequest,
                                         String CallbackId)
        {

            this.CertificateSigningRequest  = CertificateSigningRequest;
            this.CallbackId                 = CallbackId;

            unchecked
            {

                hashCode = this.CertificateSigningRequest.GetHashCode() * 3 ^
                           this.CallbackId.               GetHashCode();

            }

        }

        #endregion


        #region (static) Parse   (JSON, ...)

        /// <summary>
        /// Parse the given JSON as a certificate renewal request.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        public static CertificateRenewalRequest Parse(JObject                                                  JSON,
                                                      CustomJObjectParserDelegate<CertificateRenewalRequest>?  CustomCertificateRenewalRequestParser   = null)
        {

            if (TryParse(JSON,
                         out var certificateRenewalRequest,
                         out var errorResponse,
                         CustomCertificateRenewalRequestParser))
            {
                return certificateRenewalRequest;
            }

            throw new ArgumentException("The given JSON representation of a certificate renewal request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out CertificateRenewalRequest, out ErrorResponse, ...)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a certificate renewal request.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CertificateRenewalRequest">The parsed certificate renewal request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                              JSON,
                                       [NotNullWhen(true)]  out CertificateRenewalRequest?  CertificateRenewalRequest,
                                       [NotNullWhen(false)] out String?                     ErrorResponse)

            => TryParse(JSON,
                        out CertificateRenewalRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given text as a certificate renewal request.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CertificateRenewalRequest">The parsed certificate renewal request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomCertificateRenewalRequestParser">A delegate to parse custom certificate renewal request JSON objects.</param>
        public static Boolean TryParse(JObject                                                  JSON,
                                       [NotNullWhen(true)]  out CertificateRenewalRequest?      CertificateRenewalRequest,
                                       [NotNullWhen(false)] out String?                         ErrorResponse,
                                       CustomJObjectParserDelegate<CertificateRenewalRequest>?  CustomCertificateRenewalRequestParser)
        {

            try
            {

                CertificateRenewalRequest = null;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse CertificateSigningRequest    [mandatory]

                if (!JSON.ParseMandatoryText("csr",
                                             "certificate aigning request",
                                             out String? CertificateSigningRequest,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse CallbackId                   [mandatory]

                if (!JSON.ParseMandatoryText("callbackId",
                                             "callback identification",
                                             out String? CallbackId,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion


                CertificateRenewalRequest = new CertificateRenewalRequest(
                                                CertificateSigningRequest,
                                                CallbackId
                                            );


                if (CustomCertificateRenewalRequestParser is not null)
                    CertificateRenewalRequest = CustomCertificateRenewalRequestParser(JSON,
                                                                                      CertificateRenewalRequest);

                return true;

            }
            catch (Exception e)
            {
                CertificateRenewalRequest  = null;
                ErrorResponse              = "The given JSON representation of a certificate renewal request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomCertificateRenewalRequestSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomCertificateRenewalRequestSerializer">A delegate to serialize custom subscription cancellation requests.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<CertificateRenewalRequest>? CustomCertificateRenewalRequestSerializer = null)
        {

            var json = JSONObject.Create(

                           new JProperty("csr",          CertificateSigningRequest),

                           new JProperty("callbackId",   CallbackId)

                       );

            return CustomCertificateRenewalRequestSerializer is not null
                       ? CustomCertificateRenewalRequestSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this certificate renewal request.
        /// </summary>
        public CertificateRenewalRequest Clone()

            => new (
                   CertificateSigningRequest.CloneString(),
                   CallbackId.               CloneString()
               );

        #endregion


        #region Operator overloading

        #region Operator == (CertificateRenewalRequest1, CertificateRenewalRequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CertificateRenewalRequest1">A certificate renewal request.</param>
        /// <param name="CertificateRenewalRequest2">Another certificate renewal request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (CertificateRenewalRequest CertificateRenewalRequest1,
                                           CertificateRenewalRequest CertificateRenewalRequest2)

            => CertificateRenewalRequest1.Equals(CertificateRenewalRequest2);

        #endregion

        #region Operator != (CertificateRenewalRequest1, CertificateRenewalRequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CertificateRenewalRequest1">A certificate renewal request.</param>
        /// <param name="CertificateRenewalRequest2">Another certificate renewal request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (CertificateRenewalRequest CertificateRenewalRequest1,
                                           CertificateRenewalRequest CertificateRenewalRequest2)

            => !CertificateRenewalRequest1.Equals(CertificateRenewalRequest2);

        #endregion

        #region Operator <  (CertificateRenewalRequest1, CertificateRenewalRequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CertificateRenewalRequest1">A certificate renewal request.</param>
        /// <param name="CertificateRenewalRequest2">Another certificate renewal request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (CertificateRenewalRequest CertificateRenewalRequest1,
                                          CertificateRenewalRequest CertificateRenewalRequest2)

            => CertificateRenewalRequest1.CompareTo(CertificateRenewalRequest2) < 0;

        #endregion

        #region Operator <= (CertificateRenewalRequest1, CertificateRenewalRequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CertificateRenewalRequest1">A certificate renewal request.</param>
        /// <param name="CertificateRenewalRequest2">Another certificate renewal request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (CertificateRenewalRequest CertificateRenewalRequest1,
                                           CertificateRenewalRequest CertificateRenewalRequest2)

            => CertificateRenewalRequest1.CompareTo(CertificateRenewalRequest2) <= 0;

        #endregion

        #region Operator >  (CertificateRenewalRequest1, CertificateRenewalRequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CertificateRenewalRequest1">A certificate renewal request.</param>
        /// <param name="CertificateRenewalRequest2">Another certificate renewal request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (CertificateRenewalRequest CertificateRenewalRequest1,
                                          CertificateRenewalRequest CertificateRenewalRequest2)

            => CertificateRenewalRequest1.CompareTo(CertificateRenewalRequest2) > 0;

        #endregion

        #region Operator >= (CertificateRenewalRequest1, CertificateRenewalRequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CertificateRenewalRequest1">A certificate renewal request.</param>
        /// <param name="CertificateRenewalRequest2">Another certificate renewal request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (CertificateRenewalRequest CertificateRenewalRequest1,
                                           CertificateRenewalRequest CertificateRenewalRequest2)

            => CertificateRenewalRequest1.CompareTo(CertificateRenewalRequest2) >= 0;

        #endregion

        #endregion

        #region IComparable<CertificateRenewalRequest> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two certificate renewal requests.
        /// </summary>
        /// <param name="Object">A certificate renewal request to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is CertificateRenewalRequest certificateRenewalRequest
                   ? CompareTo(certificateRenewalRequest)
                   : throw new ArgumentException("The given object is not a certificate renewal request!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(CertificateRenewalRequest)

        /// <summary>
        /// Compares two certificate renewal requests.
        /// </summary>
        /// <param name="CertificateRenewalRequest">A certificate renewal request to compare with.</param>
        public Int32 CompareTo(CertificateRenewalRequest? CertificateRenewalRequest)
        {

            if (CertificateRenewalRequest is null)
                throw new ArgumentNullException(nameof(CertificateRenewalRequest), "The given certificate renewal request must not be null!");

            var c = String.Compare(
                        CertificateSigningRequest,
                        CertificateRenewalRequest.CertificateSigningRequest,
                        StringComparison.Ordinal
                    );

            if (c == 0)
                c = String.Compare(
                        CallbackId,
                        CertificateRenewalRequest.CallbackId,
                        StringComparison.Ordinal
                    );

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<CertificateRenewalRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two certificate renewal requests for equality.
        /// </summary>
        /// <param name="Object">A certificate renewal request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is CertificateRenewalRequest certificateRenewalRequest &&
                   Equals(certificateRenewalRequest);

        #endregion

        #region Equals(CertificateRenewalRequest)

        /// <summary>
        /// Compares two certificate renewal requests for equality.
        /// </summary>
        /// <param name="CertificateRenewalRequest">A certificate renewal request to compare with.</param>
        public Boolean Equals(CertificateRenewalRequest? CertificateRenewalRequest)

            => CertificateRenewalRequest is not null &&

               CertificateSigningRequest.Equals(CertificateRenewalRequest.CertificateSigningRequest, StringComparison.Ordinal) &&
               CallbackId.               Equals(CertificateRenewalRequest.CallbackId,                StringComparison.Ordinal);

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

            => $"{CertificateSigningRequest.SubstringMax(32)} for '{CallbackId}'";

        #endregion

    }

}
