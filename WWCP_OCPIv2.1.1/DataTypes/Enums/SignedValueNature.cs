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

using System;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_1_1
{

    /// <summary>
    /// A signed value nature.
    /// </summary>
    public readonly struct SignedValueNature : IId<SignedValueNature>

    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this identification is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty

            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// The length of the signed value nature.
        /// </summary>
        public UInt64 Length

            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new signed value nature based on the given string.
        /// </summary>
        /// <param name="String">The string representation of the signed value nature.</param>
        private SignedValueNature(String String)
        {
            this.InternalId  = String;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a signed value nature.
        /// </summary>
        /// <param name="Text">A text representation of a signed value nature.</param>
        public static SignedValueNature Parse(String Text)
        {

            if (TryParse(Text, out SignedValueNature signedValueNature))
                return signedValueNature;

            if (Text.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Text), "The given text representation of a signed value nature must not be null or empty!");

            throw new ArgumentException("The given text representation of a signed value nature is invalid!", nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a signed value nature.
        /// </summary>
        /// <param name="Text">A text representation of a signed value nature.</param>
        public static SignedValueNature? TryParse(String Text)
        {

            if (TryParse(Text, out SignedValueNature signedValueNature))
                return signedValueNature;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out SignedValueNature)

        /// <summary>
        /// Try to parse the given text as a signed value nature.
        /// </summary>
        /// <param name="Text">A text representation of a signed value nature.</param>
        /// <param name="SignedValueNature">The parsed signed value nature.</param>
        public static Boolean TryParse(String Text, out SignedValueNature SignedValueNature)
        {

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    SignedValueNature = new SignedValueNature(Text.Trim());
                    return true;
                }
                catch (Exception)
                { }
            }

            SignedValueNature = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this signed value nature.
        /// </summary>
        public SignedValueNature Clone

            => new SignedValueNature(
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        /// <summary>
        /// Signed value at the start of the charging session.
        /// </summary>
        public static SignedValueNature START         = Parse("START");

        /// <summary>
        /// Signed values take during the charging session, after start, before end.
        /// </summary>
        public static SignedValueNature INTERMEDIATE  = Parse("INTERMEDIATE");

        /// <summary>
        /// Signed value at the end of the charging session.
        /// </summary>
        public static SignedValueNature END           = Parse("END");


        #region Operator overloading

        #region Operator == (SignedValueNature1, SignedValueNature2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SignedValueNature1">A signed value nature.</param>
        /// <param name="SignedValueNature2">Another signed value nature.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (SignedValueNature SignedValueNature1,
                                           SignedValueNature SignedValueNature2)

            => SignedValueNature1.Equals(SignedValueNature2);

        #endregion

        #region Operator != (SignedValueNature1, SignedValueNature2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SignedValueNature1">A signed value nature.</param>
        /// <param name="SignedValueNature2">Another signed value nature.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (SignedValueNature SignedValueNature1,
                                           SignedValueNature SignedValueNature2)

            => !(SignedValueNature1 == SignedValueNature2);

        #endregion

        #region Operator <  (SignedValueNature1, SignedValueNature2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SignedValueNature1">A signed value nature.</param>
        /// <param name="SignedValueNature2">Another signed value nature.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (SignedValueNature SignedValueNature1,
                                          SignedValueNature SignedValueNature2)

            => SignedValueNature1.CompareTo(SignedValueNature2) < 0;

        #endregion

        #region Operator <= (SignedValueNature1, SignedValueNature2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SignedValueNature1">A signed value nature.</param>
        /// <param name="SignedValueNature2">Another signed value nature.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (SignedValueNature SignedValueNature1,
                                           SignedValueNature SignedValueNature2)

            => !(SignedValueNature1 > SignedValueNature2);

        #endregion

        #region Operator >  (SignedValueNature1, SignedValueNature2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SignedValueNature1">A signed value nature.</param>
        /// <param name="SignedValueNature2">Another signed value nature.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (SignedValueNature SignedValueNature1,
                                          SignedValueNature SignedValueNature2)

            => SignedValueNature1.CompareTo(SignedValueNature2) > 0;

        #endregion

        #region Operator >= (SignedValueNature1, SignedValueNature2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SignedValueNature1">A signed value nature.</param>
        /// <param name="SignedValueNature2">Another signed value nature.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (SignedValueNature SignedValueNature1,
                                           SignedValueNature SignedValueNature2)

            => !(SignedValueNature1 < SignedValueNature2);

        #endregion

        #endregion

        #region IComparable<SignedValueNature> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)
        {

            if (Object is SignedValueNature signedValueNature)
                return CompareTo(signedValueNature);

            throw new ArgumentException("The given object is not a signed value nature!",
                                        nameof(Object));

        }

        #endregion

        #region CompareTo(SignedValueNature)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SignedValueNature">An object to compare with.</param>
        public Int32 CompareTo(SignedValueNature SignedValueNature)

            => String.Compare(InternalId,
                              SignedValueNature.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<SignedValueNature> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)
        {

            if (Object is SignedValueNature signedValueNature)
                return Equals(signedValueNature);

            return false;

        }

        #endregion

        #region Equals(SignedValueNature)

        /// <summary>
        /// Compares two signed value natures for equality.
        /// </summary>
        /// <param name="SignedValueNature">An signed value nature to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(SignedValueNature SignedValueNature)

            => String.Equals(InternalId,
                             SignedValueNature.InternalId,
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
