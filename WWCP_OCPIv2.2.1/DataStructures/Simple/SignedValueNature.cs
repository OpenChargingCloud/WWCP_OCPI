/*
 * Copyright (c) 2015-2023 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

namespace cloud.charging.open.protocols.OCPIv2_2_1
{

    /// <summary>
    /// Extension methods for signed value natures.
    /// </summary>
    public static class SignedValueNatureExtensions
    {

        /// <summary>
        /// Indicates whether this signed value nature is null or empty.
        /// </summary>
        /// <param name="SignedValueNature">A signed value nature.</param>
        public static Boolean IsNullOrEmpty(this SignedValueNature? SignedValueNature)
            => !SignedValueNature.HasValue || SignedValueNature.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this signed value nature is NOT null or empty.
        /// </summary>
        /// <param name="SignedValueNature">A signed value nature.</param>
        public static Boolean IsNotNullOrEmpty(this SignedValueNature? SignedValueNature)
            => SignedValueNature.HasValue && SignedValueNature.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The unique identification of a meter.
    /// string(255)
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
        /// Indicates whether this signed value nature is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this signed value nature is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the signed value nature.
        /// </summary>
        public UInt64 Length
            => (UInt64) InternalId.Length;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new signed value nature based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a signed value nature.</param>
        private SignedValueNature(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as a signed value nature.
        /// </summary>
        /// <param name="Text">A text representation of a signed value nature.</param>
        public static SignedValueNature Parse(String Text)
        {

            if (TryParse(Text, out var signedValueNature))
                return signedValueNature;

            throw new ArgumentException("Invalid text representation of a signed value nature: '" + Text + "'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a signed value nature.
        /// </summary>
        /// <param name="Text">A text representation of a signed value nature.</param>
        public static SignedValueNature? TryParse(String Text)
        {

            if (TryParse(Text, out var signedValueNature))
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

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    SignedValueNature = new SignedValueNature(Text);
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

            => new (
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Static Definitions

        /// <summary>
        /// Signed value at the start of the charging session.
        /// </summary>
        public static SignedValueNature START
            => new ("START");

        /// <summary>
        /// Signed values take during the charging session, after start, before end.
        /// </summary>
        public static SignedValueNature INTERMEDIATE
            => new ("INTERMEDIATE");

        /// <summary>
        /// Signed value at the end of the charging session.
        /// </summary>
        public static SignedValueNature END
            => new ("END");

        #endregion


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

            => !SignedValueNature1.Equals(SignedValueNature2);

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

            => SignedValueNature1.CompareTo(SignedValueNature2) <= 0;

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

            => SignedValueNature1.CompareTo(SignedValueNature2) >= 0;

        #endregion

        #endregion

        #region IComparable<SignedValueNature> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two signed value natures.
        /// </summary>
        /// <param name="Object">A signed value nature to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is SignedValueNature signedValueNature
                   ? CompareTo(signedValueNature)
                   : throw new ArgumentException("The given object is not a signed value nature!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(SignedValueNature)

        /// <summary>
        /// Compares two signed value natures.
        /// </summary>
        /// <param name="SignedValueNature">A signed value nature to compare with.</param>
        public Int32 CompareTo(SignedValueNature SignedValueNature)

            => String.Compare(InternalId,
                              SignedValueNature.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<SignedValueNature> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two signed value natures for equality.
        /// </summary>
        /// <param name="Object">A signed value nature to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SignedValueNature signedValueNature &&
                   Equals(signedValueNature);

        #endregion

        #region Equals(SignedValueNature)

        /// <summary>
        /// Compares two signed value natures for equality.
        /// </summary>
        /// <param name="SignedValueNature">A signed value nature to compare with.</param>
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
