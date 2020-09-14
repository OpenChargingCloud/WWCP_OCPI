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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2
{

    /// <summary>
    /// The Token object describes the charging session and its costs,
    /// how these costs are composed, etc.
    /// </summary>
    public class Token : IHasId<Token_Id>,
                         IEquatable<Token>,
                         IComparable<Token>,
                         IComparable
    {

        #region Properties

        /// <summary>
        /// The ISO-3166 alpha-2 country code of the CPO that 'owns' this token.
        /// </summary>
        [Optional]
        public CountryCode                  CountryCode                 { get; }

        /// <summary>
        /// The Id of the CPO that 'owns' this token (following the ISO-15118 standard).
        /// </summary>
        [Optional]
        public Party_Id                     PartyId                     { get; }

        /// <summary>
        /// Unique ID by which this Token can be identified.
        /// </summary>
        [Mandatory]
        public Token_Id                     Id                          { get; }

        /// <summary>
        /// Type of the token.
        /// </summary>
        [Mandatory]
        public TokenTypes                   Types                       { get; }

        /// <summary>
        /// Uniquely identifies the EV Driver contract token within the eMSP’s platform
        /// (and suboperator platforms). 
        /// </summary>
        [Mandatory]
        public Contract_Id                  ContractId                  { get; }

        /// <summary>
        /// Visual readable number/identification as printed on the Token (RFID card),
        /// might be equal to the contract_id.
        /// </summary>
        [Optional]
        public String                       VisualNumber                { get; }

        /// <summary>
        /// Issuing company, most of the times the name of the company printed on
        /// the token (RFID card), not necessarily the eMSP.
        /// </summary>
        [Mandatory]
        public String                       Issuer                      { get; }

        /// <summary>
        /// This identification groups a couple of tokens. This can be used to make two or
        /// more tokens work as one, so that a session can be started with one token and
        /// stopped with another, handy when a card and key-fob are given to the EV-driver.
        /// </summary>
        [Optional]
        public Group_Id?                    GroupId                     { get; }

        /// <summary>
        /// Is this Token valid.
        /// </summary>
        [Mandatory]
        public Boolean                      Valid                       { get; }

        /// <summary>
        /// Indicates what type of white-listing is allowed.
        /// </summary>
        [Mandatory]
        public WhitelistTypes               Whitelist                   { get; }

        /// <summary>
        /// Optional language Code ISO 639-1. This optional field indicates the token
        /// owner’s preferred interface language. If the language is not provided or
        /// not supported then the CPO is free to choose its own language.
        /// </summary>
        [Optional]
        public String                       Language                    { get; }

        /// <summary>
        /// The default charging preference. When this is provided, and a charging session
        /// is started on an EVSE that support preference base smart charging and support
        /// this profile, the EVSE can start using this profile, without this having to be
        /// set via: SetChargingPreferences.
        /// </summary>
        [Optional]
        public ProfileTypes?                DefaultProfile              { get; }

        /// <summary>
        /// When the EVSE supports using your own energy supplier/contract, information about
        /// the energy supplier/contract is needed so the charging station operator knows
        /// which energy supplier to use.
        /// </summary>
        [Optional]
        public EnergyContract?              EnergyContract              { get; }

        /// <summary>
        /// Timestamp when this token was last updated (or created).
        /// </summary>
        [Mandatory]
        public DateTime                     LastUpdated                 { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new token describing the charging session and its costs,
        /// how these costs are composed, etc.
        /// </summary>
        public Token(CountryCode      CountryCode,
                     Party_Id         PartyId,
                     Token_Id         Id,
                     TokenTypes       Types,
                     Contract_Id      ContractId,
                     String           Issuer,
                     Boolean          Valid,
                     WhitelistTypes   Whitelist,

                     String           VisualNumber     = null,
                     Group_Id?        GroupId          = null,
                     String           Language         = null,
                     ProfileTypes?    DefaultProfile   = null,
                     EnergyContract?  EnergyContract   = null,

                     DateTime?        LastUpdated      = null)

        {

            #region Initial checks

            if (Issuer.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Issuer), "The given issuer must not be null or empty!");

            #endregion

            this.CountryCode      = CountryCode;
            this.PartyId          = PartyId;
            this.Id               = Id;
            this.Types            = Types;
            this.ContractId       = ContractId;
            this.Issuer           = Issuer;
            this.Valid            = Valid;
            this.Whitelist        = Whitelist;

            this.VisualNumber     = VisualNumber;
            this.GroupId          = GroupId;
            this.Language         = Language;
            this.DefaultProfile   = DefaultProfile;
            this.EnergyContract   = EnergyContract;

            this.LastUpdated      = LastUpdated ?? DateTime.Now;

        }

        #endregion


        #region IComparable<Token> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is Token token
                   ? CompareTo(token)
                   : throw new ArgumentException("The given object is not a token!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(Token)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Token">An Token to compare with.</param>
        public Int32 CompareTo(Token Token)
        {

            if (Token is null)
                throw new ArgumentNullException(nameof(Token),  "The given token must not be null!");

            return Id.CompareTo(Token.Id);

        }

        #endregion

        #endregion

        #region IEquatable<Token> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is Token token &&
                   Equals(token);

        #endregion

        #region Equals(Token)

        /// <summary>
        /// Compares two Tokens for equality.
        /// </summary>
        /// <param name="Token">An Token to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(Token Token)
        {

            if (Token is null)
                return false;

            return Id.Equals(Token.Id);

        }

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Get the hashcode of this object.
        /// </summary>
        public override Int32 GetHashCode()

            => Id.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Get a string representation of this object.
        /// </summary>
        public override String ToString()

            => Id.ToString();

        #endregion

    }

}
