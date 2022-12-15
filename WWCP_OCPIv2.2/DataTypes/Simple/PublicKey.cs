/*
 * Copyright (c) 2015-2022 GraphDefined GmbH
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

namespace cloud.charging.open.protocols.OCPIv2_2
{

    /// <summary>
    /// Extension methods for public keys.
    /// </summary>
    public static class PublicKeyExtensions
    {

        /// <summary>
        /// Indicates whether this public key is null or empty.
        /// </summary>
        /// <param name="PublicKey">A public key.</param>
        public static Boolean IsNullOrEmpty(this PublicKey? PublicKey)
            => !PublicKey.HasValue || PublicKey.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this public key is NOT null or empty.
        /// </summary>
        /// <param name="PublicKey">A public key.</param>
        public static Boolean IsNotNullOrEmpty(this PublicKey? PublicKey)
            => PublicKey.HasValue && PublicKey.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The unique identification of a public key.
    /// </summary>
    public readonly struct PublicKey : IId<PublicKey>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this public key is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this public key is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the public key.
        /// </summary>
        public UInt64 Length
            => (UInt64) InternalId.Length;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new public key based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a public key.</param>
        private PublicKey(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as a public key.
        /// </summary>
        /// <param name="Text">A text representation of a public key.</param>
        public static PublicKey Parse(String Text)
        {

            if (TryParse(Text, out var publicKey))
                return publicKey;

            throw new ArgumentException("Invalid text representation of a public key: '" + Text + "'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a public key.
        /// </summary>
        /// <param name="Text">A text representation of a public key.</param>
        public static PublicKey? TryParse(String Text)
        {

            if (TryParse(Text, out var publicKey))
                return publicKey;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out PublicKey)

        /// <summary>
        /// Try to parse the given text as a public key.
        /// </summary>
        /// <param name="Text">A text representation of a public key.</param>
        /// <param name="PublicKey">The parsed public key.</param>
        public static Boolean TryParse(String Text, out PublicKey PublicKey)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    PublicKey = new PublicKey(Text);
                    return true;
                }
                catch (Exception)
                { }
            }

            PublicKey = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this public key.
        /// </summary>
        public PublicKey Clone

            => new (
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Operator overloading

        #region Operator == (PublicKey1, PublicKey2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PublicKey1">A public key.</param>
        /// <param name="PublicKey2">Another public key.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (PublicKey PublicKey1,
                                           PublicKey PublicKey2)

            => PublicKey1.Equals(PublicKey2);

        #endregion

        #region Operator != (PublicKey1, PublicKey2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PublicKey1">A public key.</param>
        /// <param name="PublicKey2">Another public key.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (PublicKey PublicKey1,
                                           PublicKey PublicKey2)

            => !PublicKey1.Equals(PublicKey2);

        #endregion

        #region Operator <  (PublicKey1, PublicKey2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PublicKey1">A public key.</param>
        /// <param name="PublicKey2">Another public key.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (PublicKey PublicKey1,
                                          PublicKey PublicKey2)

            => PublicKey1.CompareTo(PublicKey2) < 0;

        #endregion

        #region Operator <= (PublicKey1, PublicKey2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PublicKey1">A public key.</param>
        /// <param name="PublicKey2">Another public key.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (PublicKey PublicKey1,
                                           PublicKey PublicKey2)

            => PublicKey1.CompareTo(PublicKey2) <= 0;

        #endregion

        #region Operator >  (PublicKey1, PublicKey2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PublicKey1">A public key.</param>
        /// <param name="PublicKey2">Another public key.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (PublicKey PublicKey1,
                                          PublicKey PublicKey2)

            => PublicKey1.CompareTo(PublicKey2) > 0;

        #endregion

        #region Operator >= (PublicKey1, PublicKey2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PublicKey1">A public key.</param>
        /// <param name="PublicKey2">Another public key.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (PublicKey PublicKey1,
                                           PublicKey PublicKey2)

            => PublicKey1.CompareTo(PublicKey2) >= 0;

        #endregion

        #endregion

        #region IComparable<PublicKey> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two public keys.
        /// </summary>
        /// <param name="Object">A public key to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is PublicKey publicKey
                   ? CompareTo(publicKey)
                   : throw new ArgumentException("The given object is not a public key!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(PublicKey)

        /// <summary>
        /// Compares two public keys.
        /// </summary>
        /// <param name="PublicKey">A public key to compare with.</param>
        public Int32 CompareTo(PublicKey PublicKey)

            => String.Compare(InternalId,
                              PublicKey.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<PublicKey> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two public keys for equality.
        /// </summary>
        /// <param name="Object">A public key to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is PublicKey publicKey &&
                   Equals(publicKey);

        #endregion

        #region Equals(PublicKey)

        /// <summary>
        /// Compares two public keys for equality.
        /// </summary>
        /// <param name="PublicKey">A public key to compare with.</param>
        public Boolean Equals(PublicKey PublicKey)

            => String.Equals(InternalId,
                             PublicKey.InternalId,
                             StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()

            => InternalId?.ToLower().GetHashCode() ?? 0;

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
