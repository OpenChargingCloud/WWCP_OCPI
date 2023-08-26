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

namespace cloud.charging.open.protocols.OCPIv2_1_1
{

    /// <summary>
    /// Extension methods for encoding methods.
    /// </summary>
    public static class EncodingMethodExtensions
    {

        /// <summary>
        /// Indicates whether this encoding method is null or empty.
        /// </summary>
        /// <param name="EncodingMethod">An encoding method.</param>
        public static Boolean IsNullOrEmpty(this EncodingMethod? EncodingMethod)
            => !EncodingMethod.HasValue || EncodingMethod.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this encoding method is NOT null or empty.
        /// </summary>
        /// <param name="EncodingMethod">An encoding method.</param>
        public static Boolean IsNotNullOrEmpty(this EncodingMethod? EncodingMethod)
            => EncodingMethod.HasValue && EncodingMethod.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The unique identification of a version.
    /// </summary>
    public readonly struct EncodingMethod : IId<EncodingMethod>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this encoding method is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this encoding method is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the encoding method.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new encoding method based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of an encoding method.</param>
        private EncodingMethod(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as an encoding method.
        /// </summary>
        /// <param name="Text">A text representation of an encoding method.</param>
        public static EncodingMethod Parse(String Text)
        {

            if (TryParse(Text, out var encodingMethod))
                return encodingMethod;

            throw new ArgumentException($"Invalid text representation of an encoding method: '" + Text + "'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as an encoding method.
        /// </summary>
        /// <param name="Text">A text representation of an encoding method.</param>
        public static EncodingMethod? TryParse(String Text)
        {

            if (TryParse(Text, out var encodingMethod))
                return encodingMethod;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out EncodingMethod)

        /// <summary>
        /// Try to parse the given text as an encoding method.
        /// </summary>
        /// <param name="Text">A text representation of an encoding method.</param>
        /// <param name="EncodingMethod">The parsed encoding method.</param>
        public static Boolean TryParse(String Text, out EncodingMethod EncodingMethod)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    EncodingMethod = new EncodingMethod(Text);
                    return true;
                }
                catch
                { }
            }

            EncodingMethod = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this encoding method.
        /// </summary>
        public EncodingMethod Clone

            => new (
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Static definitions

#pragma warning disable IDE1006 // Naming Styles

        /// <summary>
        /// Proposed by SAFE.
        /// </summary>
        public static EncodingMethod OCMF
            => new ("OCMF");

        /// <summary>
        /// Alfen Eichrecht encoding / implementation.
        /// </summary>
        public static EncodingMethod Alfen
            => new ("Alfen Eichrecht");

        /// <summary>
        /// eBee smart technologies implementation.
        /// </summary>

        public static EncodingMethod eBee
            => new ("EDL40 E-Mobility Extension");

        /// <summary>
        /// Mennekes implementation.
        /// </summary>
        public static EncodingMethod Mennekes
            => new ("EDL40 Mennekes");

        /// <summary>
        /// GraphDefined implementation.
        /// </summary>
        public static EncodingMethod GraphDefined
            => new ("GraphDefined");

        /// <summary>
        /// chargeIT Mobility implementation.
        /// </summary>
        public static EncodingMethod chargeIT
            => new ("chargeIT Mobility");

        /// <summary>
        /// ChargePoint implementation.
        /// </summary>
        public static EncodingMethod ChargePoint
            => new ("ChargePoint");

        /// <summary>
        /// Porsche implementation.
        /// </summary>
        public static EncodingMethod Porsche
            => new ("Porsche");

#pragma warning restore IDE1006 // Naming Styles

        #endregion


        #region Operator overloading

        #region Operator == (EncodingMethod1, EncodingMethod2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EncodingMethod1">An encoding method.</param>
        /// <param name="EncodingMethod2">Another encoding method.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (EncodingMethod EncodingMethod1,
                                           EncodingMethod EncodingMethod2)

            => EncodingMethod1.Equals(EncodingMethod2);

        #endregion

        #region Operator != (EncodingMethod1, EncodingMethod2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EncodingMethod1">An encoding method.</param>
        /// <param name="EncodingMethod2">Another encoding method.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (EncodingMethod EncodingMethod1,
                                           EncodingMethod EncodingMethod2)

            => !EncodingMethod1.Equals(EncodingMethod2);

        #endregion

        #region Operator <  (EncodingMethod1, EncodingMethod2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EncodingMethod1">An encoding method.</param>
        /// <param name="EncodingMethod2">Another encoding method.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (EncodingMethod EncodingMethod1,
                                          EncodingMethod EncodingMethod2)

            => EncodingMethod1.CompareTo(EncodingMethod2) < 0;

        #endregion

        #region Operator <= (EncodingMethod1, EncodingMethod2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EncodingMethod1">An encoding method.</param>
        /// <param name="EncodingMethod2">Another encoding method.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (EncodingMethod EncodingMethod1,
                                           EncodingMethod EncodingMethod2)

            => EncodingMethod1.CompareTo(EncodingMethod2) <= 0;

        #endregion

        #region Operator >  (EncodingMethod1, EncodingMethod2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EncodingMethod1">An encoding method.</param>
        /// <param name="EncodingMethod2">Another encoding method.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (EncodingMethod EncodingMethod1,
                                          EncodingMethod EncodingMethod2)

            => EncodingMethod1.CompareTo(EncodingMethod2) > 0;

        #endregion

        #region Operator >= (EncodingMethod1, EncodingMethod2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EncodingMethod1">An encoding method.</param>
        /// <param name="EncodingMethod2">Another encoding method.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (EncodingMethod EncodingMethod1,
                                           EncodingMethod EncodingMethod2)

            => EncodingMethod1.CompareTo(EncodingMethod2) >= 0;

        #endregion

        #endregion

        #region IComparable<EncodingMethod> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two encoding methods.
        /// </summary>
        /// <param name="Object">An encoding method to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is EncodingMethod encodingMethod
                   ? CompareTo(encodingMethod)
                   : throw new ArgumentException("The given object is not an encoding method!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(EncodingMethod)

        /// <summary>
        /// Compares two encoding methods.
        /// </summary>
        /// <param name="EncodingMethod">An encoding method to compare with.</param>
        public Int32 CompareTo(EncodingMethod EncodingMethod)

            => String.Compare(InternalId,
                              EncodingMethod.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<EncodingMethod> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two encoding methods for equality.
        /// </summary>
        /// <param name="Object">An encoding method to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is EncodingMethod encodingMethod &&
                   Equals(encodingMethod);

        #endregion

        #region Equals(EncodingMethod)

        /// <summary>
        /// Compares two encoding methods for equality.
        /// </summary>
        /// <param name="EncodingMethod">An encoding method to compare with.</param>
        public Boolean Equals(EncodingMethod EncodingMethod)

            => String.Equals(InternalId,
                             EncodingMethod.InternalId,
                             StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region (override) GetHashCode()

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
