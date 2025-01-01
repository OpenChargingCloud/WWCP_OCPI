/*
 * Copyright (c) 2015-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPI
{

    /// <summary>
    /// Extension methods for certificate chains.
    /// </summary>
    public static class CertificateChainExtensions
    {

        /// <summary>
        /// Indicates whether this certificate chain is null or empty.
        /// </summary>
        /// <param name="CertificateChain">A certificate chain.</param>
        public static Boolean IsNullOrEmpty(this CertificateChain? CertificateChain)
            => !CertificateChain.HasValue || CertificateChain.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this certificate chain is NOT null or empty.
        /// </summary>
        /// <param name="CertificateChain">A certificate chain.</param>
        public static Boolean IsNotNullOrEmpty(this CertificateChain? CertificateChain)
            => CertificateChain.HasValue && CertificateChain.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The unique identification of a certificate chain.
    /// </summary>
    public readonly struct CertificateChain : IId<CertificateChain>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this certificate chain is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this certificate chain is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the certificate chain.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new certificate chain based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a certificate chain.</param>
        private CertificateChain(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as a certificate chain.
        /// </summary>
        /// <param name="Text">A text representation of a certificate chain.</param>
        public static CertificateChain Parse(String Text)
        {

            if (TryParse(Text, out var certificateChain))
                return certificateChain;

            throw new ArgumentException($"Invalid text representation of a certificate chain: '" + Text + "'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a certificate chain.
        /// </summary>
        /// <param name="Text">A text representation of a certificate chain.</param>
        public static CertificateChain? TryParse(String Text)
        {

            if (TryParse(Text, out var certificateChain))
                return certificateChain;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out CertificateChain)

        /// <summary>
        /// Try to parse the given text as a certificate chain.
        /// </summary>
        /// <param name="Text">A text representation of a certificate chain.</param>
        /// <param name="CertificateChain">The parsed certificate chain.</param>
        public static Boolean TryParse(String Text, out CertificateChain CertificateChain)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    CertificateChain = new CertificateChain(Text);
                    return true;
                }
                catch
                { }
            }

            CertificateChain = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this certificate chain.
        /// </summary>
        public CertificateChain Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Operator overloading

        #region Operator == (CertificateChain1, CertificateChain2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CertificateChain1">A certificate chain.</param>
        /// <param name="CertificateChain2">Another certificate chain.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (CertificateChain CertificateChain1,
                                           CertificateChain CertificateChain2)

            => CertificateChain1.Equals(CertificateChain2);

        #endregion

        #region Operator != (CertificateChain1, CertificateChain2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CertificateChain1">A certificate chain.</param>
        /// <param name="CertificateChain2">Another certificate chain.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (CertificateChain CertificateChain1,
                                           CertificateChain CertificateChain2)

            => !CertificateChain1.Equals(CertificateChain2);

        #endregion

        #region Operator <  (CertificateChain1, CertificateChain2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CertificateChain1">A certificate chain.</param>
        /// <param name="CertificateChain2">Another certificate chain.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (CertificateChain CertificateChain1,
                                          CertificateChain CertificateChain2)

            => CertificateChain1.CompareTo(CertificateChain2) < 0;

        #endregion

        #region Operator <= (CertificateChain1, CertificateChain2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CertificateChain1">A certificate chain.</param>
        /// <param name="CertificateChain2">Another certificate chain.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (CertificateChain CertificateChain1,
                                           CertificateChain CertificateChain2)

            => CertificateChain1.CompareTo(CertificateChain2) <= 0;

        #endregion

        #region Operator >  (CertificateChain1, CertificateChain2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CertificateChain1">A certificate chain.</param>
        /// <param name="CertificateChain2">Another certificate chain.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (CertificateChain CertificateChain1,
                                          CertificateChain CertificateChain2)

            => CertificateChain1.CompareTo(CertificateChain2) > 0;

        #endregion

        #region Operator >= (CertificateChain1, CertificateChain2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CertificateChain1">A certificate chain.</param>
        /// <param name="CertificateChain2">Another certificate chain.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (CertificateChain CertificateChain1,
                                           CertificateChain CertificateChain2)

            => CertificateChain1.CompareTo(CertificateChain2) >= 0;

        #endregion

        #endregion

        #region IComparable<CertificateChain> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two certificate chains.
        /// </summary>
        /// <param name="Object">A certificate chain to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is CertificateChain certificateChain
                   ? CompareTo(certificateChain)
                   : throw new ArgumentException("The given object is not a certificate chain!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(CertificateChain)

        /// <summary>
        /// Compares two certificate chains.
        /// </summary>
        /// <param name="CertificateChain">A certificate chain to compare with.</param>
        public Int32 CompareTo(CertificateChain CertificateChain)

            => String.Compare(InternalId,
                              CertificateChain.InternalId,
                              StringComparison.Ordinal);

        #endregion

        #endregion

        #region IEquatable<CertificateChain> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two certificate chains for equality.
        /// </summary>
        /// <param name="Object">A certificate chain to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is CertificateChain certificateChain &&
                   Equals(certificateChain);

        #endregion

        #region Equals(CertificateChain)

        /// <summary>
        /// Compares two certificate chains for equality.
        /// </summary>
        /// <param name="CertificateChain">A certificate chain to compare with.</param>
        public Boolean Equals(CertificateChain CertificateChain)

            => String.Equals(InternalId,
                             CertificateChain.InternalId,
                             StringComparison.Ordinal);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()

            => InternalId?.GetHashCode() ?? 0;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => InternalId ?? "";

        #endregion

    }

}
