/*
 * Copyright (c) 2014-2020 GraphDefined GmbH
 * This file is part of WWCP Core <https://github.com/OpenChargingCloud/WWCP_Core>
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      *     http://www.apache.org/licenses/LICENSE-2.0
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

namespace cloud.charging.open.protocols
{

    /// <summary>
    /// The unique identification of an encoding method for the German Eichrecht.
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
        /// Indicates whether this identification is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty

            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// The length of the encoding method.
        /// </summary>
        public UInt64 Length

            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new encoding method for the German Eichrecht based on the given string.
        /// </summary>
        /// <param name="String">The string representation of the encoding method.</param>
        private EncodingMethod(String String)
        {
            this.InternalId  = String;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as an encoding method.
        /// </summary>
        /// <param name="Text">A text representation of an encoding method.</param>
        public static EncodingMethod Parse(String Text)
        {

            if (TryParse(Text, out EncodingMethod encodingMethod))
                return encodingMethod;

            if (Text.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Text), "The given text representation of an encoding method must not be null or empty!");

            throw new ArgumentException("The given text representation of an encoding method is invalid!", nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as an encoding method.
        /// </summary>
        /// <param name="Text">A text representation of an encoding method.</param>
        public static EncodingMethod? TryParse(String Text)
        {

            if (TryParse(Text, out EncodingMethod encodingMethod))
                return encodingMethod;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out AuthId)

        /// <summary>
        /// Try to parse the given text as an encoding method.
        /// </summary>
        /// <param name="Text">A text representation of an encoding method.</param>
        /// <param name="AuthId">The parsed encoding method.</param>
        public static Boolean TryParse(String Text, out EncodingMethod AuthId)
        {

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    AuthId = new EncodingMethod(Text.Trim());
                    return true;
                }
                catch (Exception)
                { }
            }

            AuthId = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this encoding method.
        /// </summary>
        public EncodingMethod Clone

            => new EncodingMethod(
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Static defaults

        /// <summary>
        /// Proposed by SAFE.
        /// </summary>
        public static EncodingMethod OCMF           = EncodingMethod.Parse("OCMF");

        /// <summary>
        /// Alfen Eichrecht encoding / implementation.
        /// </summary>
        public static EncodingMethod Alfen          = EncodingMethod.Parse("Alfen Eichrecht");

        /// <summary>
        /// eBee smart technologies implementation.
        /// </summary>
        public static EncodingMethod eBee           = EncodingMethod.Parse("EDL40 E-Mobility Extension");

        /// <summary>
        /// Mennekes implementation.
        /// </summary>
        public static EncodingMethod Mennekes       = EncodingMethod.Parse("EDL40 Mennekes");

        /// <summary>
        /// chargeIT Mobility implementation.
        /// </summary>
        public static EncodingMethod ChargeIT       = EncodingMethod.Parse("chargeIT Mobility");

        /// <summary>
        /// ChargePoint implementation.
        /// </summary>
        public static EncodingMethod ChargePoint    = EncodingMethod.Parse("ChargePoint");

        /// <summary>
        /// Porsche implementation.
        /// </summary>
        public static EncodingMethod Porsche        = EncodingMethod.Parse("Porsche");

        #endregion


        #region Operator overloading

        #region Operator == (AuthId1, AuthId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthId1">A encoding method.</param>
        /// <param name="AuthId2">Another encoding method.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (EncodingMethod AuthId1,
                                           EncodingMethod AuthId2)

            => AuthId1.Equals(AuthId2);

        #endregion

        #region Operator != (AuthId1, AuthId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthId1">A encoding method.</param>
        /// <param name="AuthId2">Another encoding method.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (EncodingMethod AuthId1,
                                           EncodingMethod AuthId2)

            => !(AuthId1 == AuthId2);

        #endregion

        #region Operator <  (AuthId1, AuthId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthId1">A encoding method.</param>
        /// <param name="AuthId2">Another encoding method.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (EncodingMethod AuthId1,
                                          EncodingMethod AuthId2)

            => AuthId1.CompareTo(AuthId2) < 0;

        #endregion

        #region Operator <= (AuthId1, AuthId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthId1">A encoding method.</param>
        /// <param name="AuthId2">Another encoding method.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (EncodingMethod AuthId1,
                                           EncodingMethod AuthId2)

            => !(AuthId1 > AuthId2);

        #endregion

        #region Operator >  (AuthId1, AuthId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthId1">A encoding method.</param>
        /// <param name="AuthId2">Another encoding method.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (EncodingMethod AuthId1,
                                          EncodingMethod AuthId2)

            => AuthId1.CompareTo(AuthId2) > 0;

        #endregion

        #region Operator >= (AuthId1, AuthId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthId1">A encoding method.</param>
        /// <param name="AuthId2">Another encoding method.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (EncodingMethod AuthId1,
                                           EncodingMethod AuthId2)

            => !(AuthId1 < AuthId2);

        #endregion

        #endregion

        #region IComparable<AuthId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is EncodingMethod encodingMethod
                   ? CompareTo(encodingMethod)
                   : throw new ArgumentException("The given object is not an encoding method!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(AuthId)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthId">An object to compare with.</param>
        public Int32 CompareTo(EncodingMethod AuthId)

            => String.Compare(InternalId,
                              AuthId.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<AuthId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is EncodingMethod encodingMethod &&
                   Equals(encodingMethod);

        #endregion

        #region Equals(AuthId)

        /// <summary>
        /// Compares two encoding methods for equality.
        /// </summary>
        /// <param name="AuthId">An encoding method to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(EncodingMethod AuthId)

            => String.Equals(InternalId,
                             AuthId.InternalId,
                             StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
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
