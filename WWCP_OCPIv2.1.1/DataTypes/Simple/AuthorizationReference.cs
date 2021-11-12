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

namespace cloud.charging.open.protocols.OCPIv2_1_1
{

    /// <summary>
    /// The unique identification of an authorization reference.
    /// </summary>
    public readonly struct AuthorizationReference : IId<AuthorizationReference>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        private static readonly Random random = new Random();

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this identification is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty

            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// The length of the authorization reference.
        /// </summary>
        public UInt64 Length

            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new authorization reference based on the given string.
        /// </summary>
        /// <param name="String">The string representation of the authorization reference.</param>
        private AuthorizationReference(String String)
        {
            this.InternalId  = String;
        }

        #endregion


        #region (static) Random  (Length = 30)

        /// <summary>
        /// Create a new random authorization reference.
        /// </summary>
        /// <param name="Length">The expected length of the authorization reference.</param>
        public static AuthorizationReference Random(Byte Length = 30)

            => new AuthorizationReference(random.RandomString(Length));

        #endregion

        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as an authorization reference.
        /// </summary>
        /// <param name="Text">A text representation of an authorization reference.</param>
        public static AuthorizationReference Parse(String Text)
        {

            if (TryParse(Text, out AuthorizationReference authorizationReference))
                return authorizationReference;

            if (Text.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Text), "The given text representation of an authorization reference must not be null or empty!");

            throw new ArgumentException("The given text representation of an authorization reference is invalid!", nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as an authorization reference.
        /// </summary>
        /// <param name="Text">A text representation of an authorization reference.</param>
        public static AuthorizationReference? TryParse(String Text)
        {

            if (TryParse(Text, out AuthorizationReference authorizationReference))
                return authorizationReference;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out AuthorizationReference)

        /// <summary>
        /// Try to parse the given text as an authorization reference.
        /// </summary>
        /// <param name="Text">A text representation of an authorization reference.</param>
        /// <param name="AuthorizationReference">The parsed authorization reference.</param>
        public static Boolean TryParse(String Text, out AuthorizationReference AuthorizationReference)
        {

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    AuthorizationReference = new AuthorizationReference(Text.Trim());
                    return true;
                }
                catch (Exception)
                { }
            }

            AuthorizationReference = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this authorization reference.
        /// </summary>
        public AuthorizationReference Clone

            => new AuthorizationReference(
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Operator overloading

        #region Operator == (AuthorizationReference1, AuthorizationReference2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthorizationReference1">A authorization reference.</param>
        /// <param name="AuthorizationReference2">Another authorization reference.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (AuthorizationReference AuthorizationReference1,
                                           AuthorizationReference AuthorizationReference2)

            => AuthorizationReference1.Equals(AuthorizationReference2);

        #endregion

        #region Operator != (AuthorizationReference1, AuthorizationReference2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthorizationReference1">A authorization reference.</param>
        /// <param name="AuthorizationReference2">Another authorization reference.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (AuthorizationReference AuthorizationReference1,
                                           AuthorizationReference AuthorizationReference2)

            => !(AuthorizationReference1 == AuthorizationReference2);

        #endregion

        #region Operator <  (AuthorizationReference1, AuthorizationReference2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthorizationReference1">A authorization reference.</param>
        /// <param name="AuthorizationReference2">Another authorization reference.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (AuthorizationReference AuthorizationReference1,
                                          AuthorizationReference AuthorizationReference2)

            => AuthorizationReference1.CompareTo(AuthorizationReference2) < 0;

        #endregion

        #region Operator <= (AuthorizationReference1, AuthorizationReference2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthorizationReference1">A authorization reference.</param>
        /// <param name="AuthorizationReference2">Another authorization reference.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (AuthorizationReference AuthorizationReference1,
                                           AuthorizationReference AuthorizationReference2)

            => !(AuthorizationReference1 > AuthorizationReference2);

        #endregion

        #region Operator >  (AuthorizationReference1, AuthorizationReference2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthorizationReference1">A authorization reference.</param>
        /// <param name="AuthorizationReference2">Another authorization reference.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (AuthorizationReference AuthorizationReference1,
                                          AuthorizationReference AuthorizationReference2)

            => AuthorizationReference1.CompareTo(AuthorizationReference2) > 0;

        #endregion

        #region Operator >= (AuthorizationReference1, AuthorizationReference2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthorizationReference1">A authorization reference.</param>
        /// <param name="AuthorizationReference2">Another authorization reference.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (AuthorizationReference AuthorizationReference1,
                                           AuthorizationReference AuthorizationReference2)

            => !(AuthorizationReference1 < AuthorizationReference2);

        #endregion

        #endregion

        #region IComparable<AuthorizationReference> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is AuthorizationReference authorizationReference
                   ? CompareTo(authorizationReference)
                   : throw new ArgumentException("The given object is not an authorization reference!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(AuthorizationReference)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthorizationReference">An object to compare with.</param>
        public Int32 CompareTo(AuthorizationReference AuthorizationReference)

            => String.Compare(InternalId,
                              AuthorizationReference.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<AuthorizationReference> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is AuthorizationReference authorizationReference &&
                   Equals(authorizationReference);

        #endregion

        #region Equals(AuthorizationReference)

        /// <summary>
        /// Compares two authorization references for equality.
        /// </summary>
        /// <param name="AuthorizationReference">An authorization reference to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(AuthorizationReference AuthorizationReference)

            => String.Equals(InternalId,
                             AuthorizationReference.InternalId,
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
