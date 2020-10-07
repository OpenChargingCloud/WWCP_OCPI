/*
 * Copyright (c) 2015-2020 GraphDefined GmbH
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
using System.Linq;
using System.Collections.Generic;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2
{

    /// <summary>
    /// An authorization information.
    /// </summary>
    public readonly struct AuthorizationInfo : IEquatable<AuthorizationInfo>
    {

        #region Properties

        /// <summary>
        /// Status of the token, and whether charging is allowed at the optionally given
        /// charging location.
        /// </summary>
        public AllowedTypes              Allowed                   { get; }

        /// <summary>
        /// The complete Token object for which this authorization was requested.
        /// </summary>
        public Token                     Token                     { get; }

        /// <summary>
        /// Optional reference to the location if it was included in the request, and if
        /// the EV driver is allowed to charge at that location. Only the EVSEs the EV
        /// driver is allowed to charge at are returned.
        /// </summary>
        public LocationReference?        Location                  { get; }

        /// <summary>
        /// Reference to the authorization given by the eMSP, when given, this reference
        /// will be provided in the relevant charging session and/or charge detail record.
        /// </summary>
        public AuthorizationReference?   AuthorizationReference    { get; }

        /// <summary>
        /// Optional display text, additional information to the EV driver.
        /// </summary>
        public IEnumerable<DisplayText>  Info                      { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// An authorization information consists of a start timestamp and a
        /// list of possible values that influence this period.
        /// </summary>
        public AuthorizationInfo(AllowedTypes              Allowed,
                                 Token                     Token,
                                 LocationReference?        Location                 = null,
                                 AuthorizationReference?   AuthorizationReference   = null,
                                 IEnumerable<DisplayText>  Info                     = null)
        {

            this.Allowed                 = Allowed;
            this.Token                   = Token;
            this.Location                = Location;
            this.AuthorizationReference  = AuthorizationReference;
            this.Info                    = Info;

        }

        #endregion


        #region Operator overloading

        #region Operator == (AuthorizationInfo1, AuthorizationInfo2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthorizationInfo1">An authorization information.</param>
        /// <param name="AuthorizationInfo2">Another authorization information.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (AuthorizationInfo AuthorizationInfo1,
                                           AuthorizationInfo AuthorizationInfo2)

            => AuthorizationInfo1.Equals(AuthorizationInfo2);

        #endregion

        #region Operator != (AuthorizationInfo1, AuthorizationInfo2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthorizationInfo1">An authorization information.</param>
        /// <param name="AuthorizationInfo2">Another authorization information.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (AuthorizationInfo AuthorizationInfo1,
                                           AuthorizationInfo AuthorizationInfo2)

            => !(AuthorizationInfo1 == AuthorizationInfo2);

        #endregion

        #endregion

        #region IEquatable<AuthorizationInfo> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is AuthorizationInfo authorizationReference &&
                   Equals(authorizationReference);

        #endregion

        #region Equals(AuthorizationInfo)

        /// <summary>
        /// Compares two authorization informations for equality.
        /// </summary>
        /// <param name="AuthorizationInfo">An authorization information to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(AuthorizationInfo AuthorizationInfo)

            => Allowed.               Equals(AuthorizationInfo.Allowed) &&
               Token.                 Equals(AuthorizationInfo.Token)   &&

               Location.              HasValue && AuthorizationInfo.Location.              HasValue && Location.              Value.Equals(AuthorizationInfo.Location.              Value) &&
               AuthorizationReference.HasValue && AuthorizationInfo.AuthorizationReference.HasValue && AuthorizationReference.Value.Equals(AuthorizationInfo.AuthorizationReference.Value) &&

               Info.SafeAny()                  && AuthorizationInfo.Info.SafeAny()                  && Info.Count().                Equals(AuthorizationInfo.Info.Count()) &&
               Info.SafeAll(info => AuthorizationInfo.Info.Contains(info));

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return Allowed.                            GetHashCode() * 11 ^
                       Token.                              GetHashCode() *  7 ^

                       (Location.HasValue
                            ? Location.              Value.GetHashCode() *  5
                            : 0) ^

                       (AuthorizationReference.HasValue
                            ? AuthorizationReference.Value.GetHashCode() *  3
                            : 0) ^

                       (Info.SafeAny()
                            ? Info.GetHashCode()
                            : 0);

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(Token,
                             " -> ",
                             Allowed);

        #endregion

    }

}
