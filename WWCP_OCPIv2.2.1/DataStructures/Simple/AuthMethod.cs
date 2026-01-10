/*
 * Copyright (c) 2015-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2_1
{

    /// <summary>
    /// Extension methods for AuthMethods.
    /// </summary>
    public static class AuthMethodExtensions
    {

        /// <summary>
        /// Indicates whether this AuthMethod is null or empty.
        /// </summary>
        /// <param name="AuthMethod">An AuthMethod.</param>
        public static Boolean IsNullOrEmpty(this AuthMethod? AuthMethod)
            => !AuthMethod.HasValue || AuthMethod.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this AuthMethod is NOT null or empty.
        /// </summary>
        /// <param name="AuthMethod">An AuthMethod.</param>
        public static Boolean IsNotNullOrEmpty(this AuthMethod? AuthMethod)
            => AuthMethod.HasValue && AuthMethod.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// An auth method.
    /// </summary>
    public readonly struct AuthMethod : IId<AuthMethod>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this AuthMethod is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this AuthMethod is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the AuthMethod.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new AuthMethod based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of an AuthMethod.</param>
        private AuthMethod(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as an AuthMethod.
        /// </summary>
        /// <param name="Text">A text representation of an AuthMethod.</param>
        public static AuthMethod Parse(String Text)
        {

            if (TryParse(Text, out var authMethod))
                return authMethod;

            throw new ArgumentException($"Invalid text representation of an AuthMethod: '" + Text + "'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as an AuthMethod.
        /// </summary>
        /// <param name="Text">A text representation of an AuthMethod.</param>
        public static AuthMethod? TryParse(String Text)
        {

            if (TryParse(Text, out var authMethod))
                return authMethod;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out AuthMethod)

        /// <summary>
        /// Try to parse the given text as an AuthMethod.
        /// </summary>
        /// <param name="Text">A text representation of an AuthMethod.</param>
        /// <param name="AuthMethod">The parsed AuthMethod.</param>
        public static Boolean TryParse(String Text, out AuthMethod AuthMethod)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    AuthMethod = new AuthMethod(Text);
                    return true;
                }
                catch
                { }
            }

            AuthMethod = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this AuthMethod.
        /// </summary>
        public AuthMethod Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// An authentication request has been sent to the eMSP.
        /// </summary>
        public static AuthMethod  AUTH_REQUEST    { get; }
            = new ("AUTH_REQUEST");

        /// <summary>
        /// Command like 'StartSession' or 'ReserveNow' used to start the charging session.
        /// The token provided within the command was used as authorization.
        /// </summary>
        public static AuthMethod  COMMAND         { get; }
            = new ("COMMAND");

        /// <summary>
        /// A whitelist was used for authentication, no request to the eMSP has been performed.
        /// </summary>
        public static AuthMethod  WHITELIST       { get; }
            = new ("WHITELIST");

        #endregion


        #region Operator overloading

        #region Operator == (AuthMethod1, AuthMethod2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthMethod1">An AuthMethod.</param>
        /// <param name="AuthMethod2">Another AuthMethod.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (AuthMethod AuthMethod1,
                                           AuthMethod AuthMethod2)

            => AuthMethod1.Equals(AuthMethod2);

        #endregion

        #region Operator != (AuthMethod1, AuthMethod2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthMethod1">An AuthMethod.</param>
        /// <param name="AuthMethod2">Another AuthMethod.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (AuthMethod AuthMethod1,
                                           AuthMethod AuthMethod2)

            => !AuthMethod1.Equals(AuthMethod2);

        #endregion

        #region Operator <  (AuthMethod1, AuthMethod2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthMethod1">An AuthMethod.</param>
        /// <param name="AuthMethod2">Another AuthMethod.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (AuthMethod AuthMethod1,
                                          AuthMethod AuthMethod2)

            => AuthMethod1.CompareTo(AuthMethod2) < 0;

        #endregion

        #region Operator <= (AuthMethod1, AuthMethod2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthMethod1">An AuthMethod.</param>
        /// <param name="AuthMethod2">Another AuthMethod.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (AuthMethod AuthMethod1,
                                           AuthMethod AuthMethod2)

            => AuthMethod1.CompareTo(AuthMethod2) <= 0;

        #endregion

        #region Operator >  (AuthMethod1, AuthMethod2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthMethod1">An AuthMethod.</param>
        /// <param name="AuthMethod2">Another AuthMethod.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (AuthMethod AuthMethod1,
                                          AuthMethod AuthMethod2)

            => AuthMethod1.CompareTo(AuthMethod2) > 0;

        #endregion

        #region Operator >= (AuthMethod1, AuthMethod2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthMethod1">An AuthMethod.</param>
        /// <param name="AuthMethod2">Another AuthMethod.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (AuthMethod AuthMethod1,
                                           AuthMethod AuthMethod2)

            => AuthMethod1.CompareTo(AuthMethod2) >= 0;

        #endregion

        #endregion

        #region IComparable<AuthMethod> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two energy source categories.
        /// </summary>
        /// <param name="Object">An AuthMethod to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is AuthMethod authMethod
                   ? CompareTo(authMethod)
                   : throw new ArgumentException("The given object is not an AuthMethod!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(AuthMethod)

        /// <summary>
        /// Compares two energy source categories.
        /// </summary>
        /// <param name="AuthMethod">An AuthMethod to compare with.</param>
        public Int32 CompareTo(AuthMethod AuthMethod)

            => String.Compare(InternalId,
                              AuthMethod.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<AuthMethod> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two energy source categories for equality.
        /// </summary>
        /// <param name="Object">An AuthMethod to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is AuthMethod authMethod &&
                   Equals(authMethod);

        #endregion

        #region Equals(AuthMethod)

        /// <summary>
        /// Compares two energy source categories for equality.
        /// </summary>
        /// <param name="AuthMethod">An AuthMethod to compare with.</param>
        public Boolean Equals(AuthMethod AuthMethod)

            => String.Equals(InternalId,
                             AuthMethod.InternalId,
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
