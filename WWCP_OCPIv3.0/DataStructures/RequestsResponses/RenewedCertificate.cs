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

#endregion

namespace cloud.charging.open.protocols.OCPIv3_0
{

    /// <summary>
    /// A renewed certificate (chain).
    /// </summary>
    public class RenewedCertificate
    {

        #region Properties

        /// <summary>
        /// The certificate chain for the receiving platform to identify itself.
        /// The certificate SHOULD be a X.509 certificate([X509]), encoded according to PEM ([PEM]).
        /// The PEM bundle MAY also contain sub CA certificates. In case it does, the certificates SHOULD be ordered in the bundle from the leaf certificate to the root certificate.
        /// </summary>
        public String  CertificateChain    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new renewed certificate (chain).
        /// </summary>
        /// <param name="RequestTimeout">The certificate chain for the receiving platform to identify itself.
        /// The certificate SHOULD be a X.509 certificate([X509]), encoded according to PEM ([PEM]).
        /// The PEM bundle MAY also contain sub CA certificates. In case it does, the certificates SHOULD be ordered in the bundle from the leaf certificate to the root certificate.</param>
        public RenewedCertificate(String CertificateChain)
        {

            this.CertificateChain = CertificateChain;

        }

        #endregion


        #region (static) Parse   (JSON, ...)

        /// <summary>
        /// Parse the given JSON as a renewed certificate (chain).
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        public static RenewedCertificate Parse(JObject                                           JSON,
                                               CustomJObjectParserDelegate<RenewedCertificate>?  CustomRenewedCertificateParser   = null)
        {

            if (TryParse(JSON,
                         out var renewedCertificate,
                         out var errorResponse,
                         CustomRenewedCertificateParser))
            {
                return renewedCertificate;
            }

            throw new ArgumentException("The given JSON representation of a renewed certificate (chain) is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out RenewedCertificate, out ErrorResponse, ...)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a renewed certificate (chain).
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="RenewedCertificate">The parsed renewed certificate (chain).</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                       JSON,
                                       [NotNullWhen(true)]  out RenewedCertificate?  RenewedCertificate,
                                       [NotNullWhen(false)] out String?              ErrorResponse)

            => TryParse(JSON,
                        out RenewedCertificate,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given text as a renewed certificate (chain).
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="RenewedCertificate">The parsed renewed certificate (chain).</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomRenewedCertificateParser">A delegate to parse custom renewed certificate (chain) JSON objects.</param>
        public static Boolean TryParse(JObject                                           JSON,
                                       [NotNullWhen(true)]  out RenewedCertificate?      RenewedCertificate,
                                       [NotNullWhen(false)] out String?                  ErrorResponse,
                                       CustomJObjectParserDelegate<RenewedCertificate>?  CustomRenewedCertificateParser)
        {

            try
            {

                RenewedCertificate = null;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse CertificateChain    [mandatory]

                if (!JSON.ParseMandatoryText("certificateChain",
                                             "certificate chain",
                                             out String? CertificateChain,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion


                RenewedCertificate = new RenewedCertificate(
                                         CertificateChain
                                     );


                if (CustomRenewedCertificateParser is not null)
                    RenewedCertificate = CustomRenewedCertificateParser(JSON,
                                                                        RenewedCertificate);

                return true;

            }
            catch (Exception e)
            {
                RenewedCertificate  = null;
                ErrorResponse       = "The given JSON representation of a renewed certificate (chain) is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this renewed certificate (chain).
        /// </summary>
        public RenewedCertificate Clone()

            => new (
                   CertificateChain
               );

        #endregion


        #region Operator overloading

        #region Operator == (RenewedCertificate1, RenewedCertificate2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RenewedCertificate1">A renewed certificate (chain).</param>
        /// <param name="RenewedCertificate2">Another renewed certificate (chain).</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (RenewedCertificate RenewedCertificate1,
                                           RenewedCertificate RenewedCertificate2)

            => RenewedCertificate1.Equals(RenewedCertificate2);

        #endregion

        #region Operator != (RenewedCertificate1, RenewedCertificate2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RenewedCertificate1">A renewed certificate (chain).</param>
        /// <param name="RenewedCertificate2">Another renewed certificate (chain).</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (RenewedCertificate RenewedCertificate1,
                                           RenewedCertificate RenewedCertificate2)

            => !RenewedCertificate1.Equals(RenewedCertificate2);

        #endregion

        #region Operator <  (RenewedCertificate1, RenewedCertificate2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RenewedCertificate1">A renewed certificate (chain).</param>
        /// <param name="RenewedCertificate2">Another renewed certificate (chain).</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (RenewedCertificate RenewedCertificate1,
                                          RenewedCertificate RenewedCertificate2)

            => RenewedCertificate1.CompareTo(RenewedCertificate2) < 0;

        #endregion

        #region Operator <= (RenewedCertificate1, RenewedCertificate2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RenewedCertificate1">A renewed certificate (chain).</param>
        /// <param name="RenewedCertificate2">Another renewed certificate (chain).</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (RenewedCertificate RenewedCertificate1,
                                           RenewedCertificate RenewedCertificate2)

            => RenewedCertificate1.CompareTo(RenewedCertificate2) <= 0;

        #endregion

        #region Operator >  (RenewedCertificate1, RenewedCertificate2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RenewedCertificate1">A renewed certificate (chain).</param>
        /// <param name="RenewedCertificate2">Another renewed certificate (chain).</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (RenewedCertificate RenewedCertificate1,
                                          RenewedCertificate RenewedCertificate2)

            => RenewedCertificate1.CompareTo(RenewedCertificate2) > 0;

        #endregion

        #region Operator >= (RenewedCertificate1, RenewedCertificate2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RenewedCertificate1">A renewed certificate (chain).</param>
        /// <param name="RenewedCertificate2">Another renewed certificate (chain).</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (RenewedCertificate RenewedCertificate1,
                                           RenewedCertificate RenewedCertificate2)

            => RenewedCertificate1.CompareTo(RenewedCertificate2) >= 0;

        #endregion

        #endregion

        #region IComparable<RenewedCertificate> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two renewed certificate (chain)s.
        /// </summary>
        /// <param name="Object">A renewed certificate (chain) to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is RenewedCertificate renewedCertificate
                   ? CompareTo(renewedCertificate)
                   : throw new ArgumentException("The given object is not a renewed certificate (chain)!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(RenewedCertificate)

        /// <summary>
        /// Compares two renewed certificate (chain)s.
        /// </summary>
        /// <param name="RenewedCertificate">A renewed certificate (chain) to compare with.</param>
        public Int32 CompareTo(RenewedCertificate? RenewedCertificate)
        {

            if (RenewedCertificate is null)
                throw new ArgumentNullException(nameof(RenewedCertificate), "The given renewed certificate (chain) must not be null!");

            return String.Compare(CertificateChain,
                                  RenewedCertificate.CertificateChain,
                                  StringComparison.Ordinal);

        }

        #endregion

        #endregion

        #region IEquatable<RenewedCertificate> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two renewed certificate (chain)s for equality.
        /// </summary>
        /// <param name="Object">A renewed certificate (chain) to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is RenewedCertificate renewedCertificate &&
                   Equals(renewedCertificate);

        #endregion

        #region Equals(RenewedCertificate)

        /// <summary>
        /// Compares two renewed certificate (chain)s for equality.
        /// </summary>
        /// <param name="RenewedCertificate">A renewed certificate (chain) to compare with.</param>
        public Boolean Equals(RenewedCertificate? RenewedCertificate)

            => RenewedCertificate is not null &&
               CertificateChain.Equals(RenewedCertificate.CertificateChain);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()

            => CertificateChain.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"{CertificateChain.SubstringMax(32)} sec.";

        #endregion

    }

}
