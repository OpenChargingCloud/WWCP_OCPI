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
using System.Collections.Generic;
using System.Linq;
using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2
{

    /// <summary>
    /// CDR token.
    /// </summary>
    public readonly struct CDRToken : IEquatable<CDRToken>,
                                      IComparable<CDRToken>,
                                      IComparable
    {

        #region Properties

        /// <summary>
        /// The unique identification by which this token can be identified.
        /// </summary>
        public Token_Id     UID           { get; }

        /// <summary>
        /// The type of the token.
        /// </summary>
        public TokenTypes   TokenType     { get; }

        /// <summary>
        /// Uniquely identifies the EV driver contract token within the eMSP’s
        /// platform (and suboperator platforms).
        /// </summary>
        public Contract_Id  ContractId    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// A charging period consists of a start timestamp and a
        /// list of possible values that influence this period.
        /// </summary>
        /// <param name="UID">The unique identification by which this token can be identified.</param>
        /// <param name="TokenType">The type of the token.</param>
        /// <param name="ContractId">Uniquely identifies the EV driver contract token within the eMSP’s platform (and suboperator platforms).</param>
        public CDRToken(Token_Id     UID,
                        TokenTypes   TokenType,
                        Contract_Id  ContractId)
        {

            this.UID         = UID;
            this.TokenType   = TokenType;
            this.ContractId  = ContractId;

        }

        #endregion


        #region Operator overloading

        #region Operator == (CDRToken1, CDRToken2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDRToken1">A charging period.</param>
        /// <param name="CDRToken2">Another charging period.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (CDRToken CDRToken1,
                                           CDRToken CDRToken2)

            => CDRToken1.Equals(CDRToken2);

        #endregion

        #region Operator != (CDRToken1, CDRToken2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDRToken1">A charging period.</param>
        /// <param name="CDRToken2">Another charging period.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (CDRToken CDRToken1,
                                           CDRToken CDRToken2)

            => !(CDRToken1 == CDRToken2);

        #endregion

        #region Operator <  (CDRToken1, CDRToken2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDRToken1">A charging period.</param>
        /// <param name="CDRToken2">Another charging period.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (CDRToken CDRToken1,
                                          CDRToken CDRToken2)

            => CDRToken1.CompareTo(CDRToken2) < 0;

        #endregion

        #region Operator <= (CDRToken1, CDRToken2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDRToken1">A charging period.</param>
        /// <param name="CDRToken2">Another charging period.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (CDRToken CDRToken1,
                                           CDRToken CDRToken2)

            => !(CDRToken1 > CDRToken2);

        #endregion

        #region Operator >  (CDRToken1, CDRToken2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDRToken1">A charging period.</param>
        /// <param name="CDRToken2">Another charging period.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (CDRToken CDRToken1,
                                          CDRToken CDRToken2)

            => CDRToken1.CompareTo(CDRToken2) > 0;

        #endregion

        #region Operator >= (CDRToken1, CDRToken2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDRToken1">A charging period.</param>
        /// <param name="CDRToken2">Another charging period.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (CDRToken CDRToken1,
                                           CDRToken CDRToken2)

            => !(CDRToken1 < CDRToken2);

        #endregion

        #endregion

        #region IComparable<CDRToken> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is CDRToken token
                   ? CompareTo(token)
                   : throw new ArgumentException("The given object is not a charging period!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(CDRToken)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDRToken">An object to compare with.</param>
        public Int32 CompareTo(CDRToken CDRToken)
        {

            var c = UID.       CompareTo(CDRToken.UID);

            if (c == 0)
                c = TokenType. CompareTo(CDRToken.TokenType);

            if (c == 0)
                c = ContractId.CompareTo(CDRToken.ContractId);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<CDRToken> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is CDRToken token &&
                   Equals(token);

        #endregion

        #region Equals(CDRToken)

        /// <summary>
        /// Compares two charging periods for equality.
        /// </summary>
        /// <param name="CDRToken">A charging period to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(CDRToken CDRToken)

            => UID.       Equals(CDRToken.UID)       &&
               TokenType. Equals(CDRToken.TokenType) &&
               ContractId.Equals(CDRToken.ContractId);

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

                return UID.       GetHashCode() * 5 ^
                       TokenType. GetHashCode() * 3 ^
                       ContractId.GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(UID,
                             " (", TokenType, ") => ",
                             ContractId);

        #endregion

    }

}
