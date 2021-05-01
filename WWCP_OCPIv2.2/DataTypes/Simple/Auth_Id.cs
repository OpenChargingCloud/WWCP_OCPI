/*
 * Copyright (c) 2014--2021 GraphDefined GmbH
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

namespace cloud.charging.open.protocols.OCPIv2_2
{

    /// <summary>
    /// The unique identification of an authentication credential.
    /// </summary>
    public readonly struct Auth_Id : IId<Auth_Id>
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
        /// The length of the authentication credential.
        /// </summary>
        public UInt64 Length

            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new authentication credential based on the given string.
        /// </summary>
        /// <param name="String">The string representation of the authentication credential.</param>
        private Auth_Id(String String)
        {
            this.InternalId  = String;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as an authentication credential.
        /// </summary>
        /// <param name="Text">A text representation of an authentication credential.</param>
        public static Auth_Id Parse(String Text)
        {

            if (TryParse(Text, out Auth_Id authId))
                return authId;

            if (Text.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Text), "The given text representation of an authentication credential must not be null or empty!");

            throw new ArgumentException("The given text representation of an authentication credential is invalid!", nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as an authentication credential.
        /// </summary>
        /// <param name="Text">A text representation of an authentication credential.</param>
        public static Auth_Id? TryParse(String Text)
        {

            if (TryParse(Text, out Auth_Id authId))
                return authId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out AuthId)

        /// <summary>
        /// Try to parse the given text as an authentication credential.
        /// </summary>
        /// <param name="Text">A text representation of an authentication credential.</param>
        /// <param name="AuthId">The parsed authentication credential.</param>
        public static Boolean TryParse(String Text, out Auth_Id AuthId)
        {

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    AuthId = new Auth_Id(Text.Trim());
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
        /// Clone this authentication credential.
        /// </summary>
        public Auth_Id Clone

            => new Auth_Id(
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Operator overloading

        #region Operator == (AuthId1, AuthId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthId1">A authentication credential.</param>
        /// <param name="AuthId2">Another authentication credential.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Auth_Id AuthId1,
                                           Auth_Id AuthId2)

            => AuthId1.Equals(AuthId2);

        #endregion

        #region Operator != (AuthId1, AuthId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthId1">A authentication credential.</param>
        /// <param name="AuthId2">Another authentication credential.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Auth_Id AuthId1,
                                           Auth_Id AuthId2)

            => !(AuthId1 == AuthId2);

        #endregion

        #region Operator <  (AuthId1, AuthId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthId1">A authentication credential.</param>
        /// <param name="AuthId2">Another authentication credential.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Auth_Id AuthId1,
                                          Auth_Id AuthId2)

            => AuthId1.CompareTo(AuthId2) < 0;

        #endregion

        #region Operator <= (AuthId1, AuthId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthId1">A authentication credential.</param>
        /// <param name="AuthId2">Another authentication credential.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Auth_Id AuthId1,
                                           Auth_Id AuthId2)

            => !(AuthId1 > AuthId2);

        #endregion

        #region Operator >  (AuthId1, AuthId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthId1">A authentication credential.</param>
        /// <param name="AuthId2">Another authentication credential.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Auth_Id AuthId1,
                                          Auth_Id AuthId2)

            => AuthId1.CompareTo(AuthId2) > 0;

        #endregion

        #region Operator >= (AuthId1, AuthId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthId1">A authentication credential.</param>
        /// <param name="AuthId2">Another authentication credential.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Auth_Id AuthId1,
                                           Auth_Id AuthId2)

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

            => Object is Auth_Id authId
                   ? CompareTo(authId)
                   : throw new ArgumentException("The given object is not an authentication credential!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(AuthId)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthId">An object to compare with.</param>
        public Int32 CompareTo(Auth_Id AuthId)

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

            => Object is Auth_Id authId &&
                   Equals(authId);

        #endregion

        #region Equals(AuthId)

        /// <summary>
        /// Compares two authentication credentials for equality.
        /// </summary>
        /// <param name="AuthId">An authentication credential to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(Auth_Id AuthId)

            => String.Equals(InternalId,
                             AuthId.InternalId,
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
