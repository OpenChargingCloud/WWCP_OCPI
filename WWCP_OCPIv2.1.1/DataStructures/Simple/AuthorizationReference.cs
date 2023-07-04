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
    /// Extension methods for authorization references.
    /// </summary>
    public static class AuthorizationReferenceExtensions
    {

        /// <summary>
        /// Indicates whether this authorization reference is null or empty.
        /// </summary>
        /// <param name="AuthorizationReference">An authorization reference.</param>
        public static Boolean IsNullOrEmpty(this AuthorizationReference? AuthorizationReference)
            => !AuthorizationReference.HasValue || AuthorizationReference.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this authorization reference is NOT null or empty.
        /// </summary>
        /// <param name="AuthorizationReference">An authorization reference.</param>
        public static Boolean IsNotNullOrEmpty(this AuthorizationReference? AuthorizationReference)
            => AuthorizationReference.HasValue && AuthorizationReference.Value.IsNotNullOrEmpty;

    }


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

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this authorization reference is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this authorization reference is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the authorization reference.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new authorization reference based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of an authorization reference.</param>
        private AuthorizationReference(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) NewRandom(Length = 30)

        /// <summary>
        /// Create a new random authorization reference.
        /// </summary>
        /// <param name="Length">The expected length of the authorization reference.</param>
        public static AuthorizationReference NewRandom(Byte Length = 30)

            => new (RandomExtensions.RandomString(Length));

        #endregion

        #region (static) Parse    (Text)

        /// <summary>
        /// Parse the given text as an authorization reference.
        /// </summary>
        /// <param name="Text">A text representation of an authorization reference.</param>
        public static AuthorizationReference Parse(String Text)
        {

            if (TryParse(Text, out var authorizationReference))
                return authorizationReference;

            throw new ArgumentException($"Invalid text representation of an authorization reference: '" + Text + "'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse (Text)

        /// <summary>
        /// Try to parse the given text as an authorization reference.
        /// </summary>
        /// <param name="Text">A text representation of an authorization reference.</param>
        public static AuthorizationReference? TryParse(String Text)
        {

            if (TryParse(Text, out var authorizationReference))
                return authorizationReference;

            return null;

        }

        #endregion

        #region (static) TryParse (Text, out AuthorizationReference)

        /// <summary>
        /// Try to parse the given text as an authorization reference.
        /// </summary>
        /// <param name="Text">A text representation of an authorization reference.</param>
        /// <param name="AuthorizationReference">The parsed authorization reference.</param>
        public static Boolean TryParse(String Text, out AuthorizationReference AuthorizationReference)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    AuthorizationReference = new AuthorizationReference(Text);
                    return true;
                }
                catch
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

            => new (
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Operator overloading

        #region Operator == (AuthorizationReference1, AuthorizationReference2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthorizationReference1">An authorization reference.</param>
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
        /// <param name="AuthorizationReference1">An authorization reference.</param>
        /// <param name="AuthorizationReference2">Another authorization reference.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (AuthorizationReference AuthorizationReference1,
                                           AuthorizationReference AuthorizationReference2)

            => !AuthorizationReference1.Equals(AuthorizationReference2);

        #endregion

        #region Operator <  (AuthorizationReference1, AuthorizationReference2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthorizationReference1">An authorization reference.</param>
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
        /// <param name="AuthorizationReference1">An authorization reference.</param>
        /// <param name="AuthorizationReference2">Another authorization reference.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (AuthorizationReference AuthorizationReference1,
                                           AuthorizationReference AuthorizationReference2)

            => AuthorizationReference1.CompareTo(AuthorizationReference2) <= 0;

        #endregion

        #region Operator >  (AuthorizationReference1, AuthorizationReference2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthorizationReference1">An authorization reference.</param>
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
        /// <param name="AuthorizationReference1">An authorization reference.</param>
        /// <param name="AuthorizationReference2">Another authorization reference.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (AuthorizationReference AuthorizationReference1,
                                           AuthorizationReference AuthorizationReference2)

            => AuthorizationReference1.CompareTo(AuthorizationReference2) >= 0;

        #endregion

        #endregion

        #region IComparable<AuthorizationReference> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two authorization references.
        /// </summary>
        /// <param name="Object">An authorization reference to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is AuthorizationReference authorizationReference
                   ? CompareTo(authorizationReference)
                   : throw new ArgumentException("The given object is not an authorization reference!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(AuthorizationReference)

        /// <summary>
        /// Compares two authorization references.
        /// </summary>
        /// <param name="AuthorizationReference">An authorization reference to compare with.</param>
        public Int32 CompareTo(AuthorizationReference AuthorizationReference)

            => String.Compare(InternalId,
                              AuthorizationReference.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<AuthorizationReference> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two authorization references for equality.
        /// </summary>
        /// <param name="Object">An authorization reference to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is AuthorizationReference authorizationReference &&
                   Equals(authorizationReference);

        #endregion

        #region Equals(AuthorizationReference)

        /// <summary>
        /// Compares two authorization references for equality.
        /// </summary>
        /// <param name="AuthorizationReference">An authorization reference to compare with.</param>
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
