/*
 * Copyright (c) 2015-2022 GraphDefined GmbH
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
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_1_1
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

        #region Data

        private readonly Object patchLock = new Object();

        #endregion

        #region Properties

        /// <summary>
        /// The ISO-3166 alpha-2 country code of the CPO that 'owns' this token.
        /// </summary>
        [Mandatory]
        public CountryCode                  CountryCode                 { get; }

        /// <summary>
        /// The Id of the CPO that 'owns' this token (following the ISO-15118 standard).
        /// </summary>
        [Mandatory]
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
        public TokenTypes                   Type                        { get; }

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
        public Boolean                      IsValid                     { get; }

        /// <summary>
        /// Indicates what type of white-listing is allowed.
        /// </summary>
        [Mandatory]
        public WhitelistTypes               WhitelistType               { get; }

        /// <summary>
        /// Optional language Code ISO 639-1. This optional field indicates the token
        /// owner’s preferred interface language. If the language is not provided or
        /// not supported then the CPO is free to choose its own language.
        /// </summary>
        [Optional]
        public Languages?                   UILanguage                  { get; }

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
        public EnergyContract?              EnergyContract            { get; }

        /// <summary>
        /// Timestamp when this token was last updated (or created).
        /// </summary>
        [Mandatory]
        public DateTime                     LastUpdated                 { get; }

        /// <summary>
        /// The SHA256 hash of the JSON representation of this tokenf.
        /// </summary>
        public String                       SHA256Hash                  { get; private set; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new token describing the charging session and its costs,
        /// how these costs are composed, etc.
        /// </summary>
        public Token(CountryCode      CountryCode,
                     Party_Id         PartyId,
                     Token_Id         Id,
                     TokenTypes       Type,
                     Contract_Id      ContractId,
                     String           Issuer,
                     Boolean          IsValid,
                     WhitelistTypes   WhitelistType,

                     String           VisualNumber     = null,
                     Group_Id?        GroupId          = null,
                     Languages?       UILanguage       = null,
                     ProfileTypes?    DefaultProfile   = null,
                     EnergyContract?  EnergyContract   = null,

                     DateTime?        LastUpdated      = null)

        {

            if (Issuer.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Issuer), "The given issuer must not be null or empty!");

            this.CountryCode     = CountryCode;
            this.PartyId         = PartyId;
            this.Id              = Id;
            this.Type            = Type;
            this.ContractId      = ContractId;
            this.Issuer          = Issuer;
            this.IsValid         = IsValid;
            this.WhitelistType   = WhitelistType;

            this.VisualNumber    = VisualNumber;
            this.GroupId         = GroupId;
            this.UILanguage      = UILanguage;
            this.DefaultProfile  = DefaultProfile;
            this.EnergyContract  = EnergyContract;

            this.LastUpdated     = LastUpdated ?? DateTime.Now;

            CalcSHA256Hash();

        }

        #endregion


        #region (static) Parse   (JSON, TokenIdURL = null, CustomTokenParser = null)

        /// <summary>
        /// Parse the given JSON representation of a token.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="TokenIdURL">An optional token identification, e.g. from the HTTP URL.</param>
        /// <param name="CustomTokenParser">A delegate to parse custom token JSON objects.</param>
        public static Token Parse(JObject                             JSON,
                                  CountryCode?                        CountryCodeURL      = null,
                                  Party_Id?                           PartyIdURL          = null,
                                  Token_Id?                           TokenIdURL          = null,
                                  CustomJObjectParserDelegate<Token>  CustomTokenParser   = null)
        {

            if (TryParse(JSON,
                         out Token   token,
                         out String  ErrorResponse,
                         CountryCodeURL,
                         PartyIdURL,
                         TokenIdURL,
                         CustomTokenParser))
            {
                return token;
            }

            throw new ArgumentException("The given JSON representation of a token is invalid: " + ErrorResponse, nameof(JSON));

        }

        #endregion

        #region (static) Parse   (Text, TokenIdURL = null, CustomTokenParser = null)

        /// <summary>
        /// Parse the given text representation of a token.
        /// </summary>
        /// <param name="Text">The text to parse.</param>
        /// <param name="TokenIdURL">An optional token identification, e.g. from the HTTP URL.</param>
        /// <param name="CustomTokenParser">A delegate to parse custom token JSON objects.</param>
        public static Token Parse(String                              Text,
                                  CountryCode?                        CountryCodeURL      = null,
                                  Party_Id?                           PartyIdURL          = null,
                                  Token_Id?                           TokenIdURL          = null,
                                  CustomJObjectParserDelegate<Token>  CustomTokenParser   = null)
        {

            if (TryParse(Text,
                         out Token   token,
                         out String  ErrorResponse,
                         CountryCodeURL,
                         PartyIdURL,
                         TokenIdURL,
                         CustomTokenParser))
            {
                return token;
            }

            throw new ArgumentException("The given text representation of a token is invalid: " + ErrorResponse, nameof(Text));

        }

        #endregion

        #region (static) TryParse(JSON, out Token, out ErrorResponse, TokenIdURL = null, CustomTokenParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a token.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="Token">The parsed token.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject     JSON,
                                       out Token   Token,
                                       out String  ErrorResponse)

            => TryParse(JSON,
                        out Token,
                        out ErrorResponse,
                        null,
                        null,
                        null,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a token.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="Token">The parsed token.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CountryCodeURL">An optional country code, e.g. from the HTTP URL.</param>
        /// <param name="PartyIdURL">An optional party identification, e.g. from the HTTP URL.</param>
        /// <param name="TokenIdURL">An optional token identification, e.g. from the HTTP URL.</param>
        /// <param name="CustomTokenParser">A delegate to parse custom token JSON objects.</param>
        public static Boolean TryParse(JObject                             JSON,
                                       out Token                           Token,
                                       out String                          ErrorResponse,
                                       CountryCode?                        CountryCodeURL      = null,
                                       Party_Id?                           PartyIdURL          = null,
                                       Token_Id?                           TokenIdURL          = null,
                                       CustomJObjectParserDelegate<Token>  CustomTokenParser   = null)
        {

            try
            {

                Token = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse CountryCode           [optional]

                if (JSON.ParseOptional("country_code",
                                       "country code",
                                       CountryCode.TryParse,
                                       out CountryCode? CountryCodeBody,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                if (!CountryCodeURL.HasValue && !CountryCodeBody.HasValue)
                {
                    ErrorResponse = "The country code is missing!";
                    return false;
                }

                if (CountryCodeURL.HasValue && CountryCodeBody.HasValue && CountryCodeURL.Value != CountryCodeBody.Value)
                {
                    ErrorResponse = "The optional country code given within the JSON body does not match the one given in the URL!";
                    return false;
                }

                #endregion

                #region Parse PartyIdURL            [optional]

                if (JSON.ParseOptional("party_id",
                                       "party identification",
                                       Party_Id.TryParse,
                                       out Party_Id? PartyIdBody,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                if (!PartyIdURL.HasValue && !PartyIdBody.HasValue)
                {
                    ErrorResponse = "The party identification is missing!";
                    return false;
                }

                if (PartyIdURL.HasValue && PartyIdBody.HasValue && PartyIdURL.Value != PartyIdBody.Value)
                {
                    ErrorResponse = "The optional party identification given within the JSON body does not match the one given in the URL!";
                    return false;
                }

                #endregion

                #region Parse Id                    [optional]

                if (JSON.ParseOptional("uid",
                                       "token identification",
                                       Token_Id.TryParse,
                                       out Token_Id? TokenIdBody,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                if (!TokenIdURL.HasValue && !TokenIdBody.HasValue)
                {
                    ErrorResponse = "The token identification is missing!";
                    return false;
                }

                if (TokenIdURL.HasValue && TokenIdBody.HasValue && TokenIdURL.Value != TokenIdBody.Value)
                {
                    ErrorResponse = "The optional token identification given within the JSON body does not match the one given in the URL!";
                    return false;
                }

                #endregion

                #region Parse Type                  [mandatory]

                if (!JSON.ParseMandatoryEnum("type",
                                             "token type",
                                             out TokenTypes Type,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse ContractId            [mandatory]

                if (!JSON.ParseMandatory("contract_id",
                                         "contract identification",
                                         Contract_Id.TryParse,
                                         out Contract_Id ContractId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse VisualNumber          [optional]

                var VisualNumber = JSON.GetString("visual_number");

                #endregion

                #region Parse Issuer                [mandatory]

                if (!JSON.ParseMandatoryText("issuer",
                                             "issuer",
                                             out String Issuer,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse GroupId               [optional]

                if (JSON.ParseOptional("group_id",
                                       "group identification",
                                       Group_Id.TryParse,
                                       out Group_Id? GroupId,
                                       out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                }

                #endregion

                #region Parse IsValid               [mandatory]

                if (!JSON.ParseMandatory("valid",
                                         "token is valid",
                                         out Boolean IsValid,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse WhitelistType         [mandatory]

                if (!JSON.ParseMandatoryEnum("whitelist",
                                             "whitelist type",
                                             out WhitelistTypes WhitelistType,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse UILanguage            [optional]

                if (JSON.ParseOptionalEnum("language",
                                           "user-interface language",
                                           out Languages? UILanguage,
                                           out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                }

                #endregion

                #region Parse DefaultProfile        [optional]

                if (JSON.ParseOptionalEnum("default_profile_type",
                                           "user-interface language",
                                           out ProfileTypes? DefaultProfile,
                                           out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                }

                #endregion

                #region Parse EnergyContract        [optional]

                if (JSON.ParseOptionalJSON("energy_contract",
                                           "energy contract",
                                           OCPIv2_1_1.EnergyContract.TryParse,
                                           out EnergyContract EnergyContract,
                                           out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                }

                #endregion

                #region Parse LastUpdated           [mandatory]

                if (!JSON.ParseMandatory("last_updated",
                                         "last updated",
                                         out DateTime LastUpdated,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                Token = new Token(CountryCodeBody ?? CountryCodeURL.Value,
                                  PartyIdBody     ?? PartyIdURL.Value,
                                  TokenIdBody     ?? TokenIdURL.Value,
                                  Type,
                                  ContractId,
                                  Issuer,
                                  IsValid,
                                  WhitelistType,
                                  VisualNumber,
                                  GroupId,
                                  UILanguage,
                                  DefaultProfile,
                                  EnergyContract,
                                  LastUpdated);


                if (CustomTokenParser is not null)
                    Token = CustomTokenParser(JSON,
                                              Token);

                return true;

            }
            catch (Exception e)
            {
                Token          = default;
                ErrorResponse  = "The given JSON representation of a token is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Text, out Token, out ErrorResponse, TokenIdURL = null, CustomTokenParser = null)

        /// <summary>
        /// Try to parse the given text representation of a token.
        /// </summary>
        /// <param name="Text">The text to parse.</param>
        /// <param name="Token">The parsed token.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="TokenIdURL">An optional token identification, e.g. from the HTTP URL.</param>
        /// <param name="CustomTokenParser">A delegate to parse custom token JSON objects.</param>
        public static Boolean TryParse(String                              Text,
                                       out Token                           Token,
                                       out String                          ErrorResponse,
                                       CountryCode?                        CountryCodeURL      = null,
                                       Party_Id?                           PartyIdURL          = null,
                                       Token_Id?                           TokenIdURL          = null,
                                       CustomJObjectParserDelegate<Token>  CustomTokenParser   = null)
        {

            try
            {

                return TryParse(JObject.Parse(Text),
                                out Token,
                                out ErrorResponse,
                                CountryCodeURL,
                                PartyIdURL,
                                TokenIdURL,
                                CustomTokenParser);

            }
            catch (Exception e)
            {
                Token          = null;
                ErrorResponse  = "The given text representation of a token is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomTokenSerializer = null, CustomEnergyContractSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomTokenSerializer">A delegate to serialize custom token JSON objects.</param>
        /// <param name="CustomEnergyContractSerializer">A delegate to serialize custom energy contract JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<Token>           CustomTokenSerializer            = null,
                              CustomJObjectSerializerDelegate<EnergyContract>  CustomEnergyContractSerializer   = null)
        {

            var JSON = JSONObject.Create(

                           new JProperty("country_code",                CountryCode.         ToString()),
                           new JProperty("party_id",                    PartyId.             ToString()),
                           new JProperty("uid",                         Id.                  ToString()),
                           new JProperty("type",                        Type.                ToString()),
                           new JProperty("contract_id",                 ContractId.          ToString()),

                           VisualNumber.IsNotNullOrEmpty()
                               ? new JProperty("visual_number",         VisualNumber)
                               : null,

                           new JProperty("issuer",                      Issuer),

                           GroupId.HasValue
                               ? new JProperty("group_id",              GroupId.             ToString())
                               : null,

                           new JProperty("valid",                       IsValid),
                           new JProperty("whitelist",                   WhitelistType.       ToString()),

                           UILanguage.HasValue
                               ? new JProperty("language",              UILanguage.          ToString())
                               : null,

                           DefaultProfile.HasValue
                               ? new JProperty("default_profile_type",  DefaultProfile.      ToString())
                               : null,

                           EnergyContract.HasValue
                               ? new JProperty("energy_contract",       EnergyContract.Value.ToJSON(CustomEnergyContractSerializer))
                               : null,

                           new JProperty("last_updated",                LastUpdated.         ToIso8601())

                       );

            return CustomTokenSerializer is not null
                       ? CustomTokenSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region (private) TryPrivatePatch(JSON, Patch)

        private PatchResult<JObject> TryPrivatePatch(JObject  JSON,
                                                     JObject  Patch)
        {

            foreach (var property in Patch)
            {

                if (property.Key == "id")
                    return PatchResult<JObject>.Failed(JSON,
                                                       "Patching the 'unique identification' of a token is not allowed!");

                else if (property.Value is null)
                    JSON.Remove(property.Key);

                else if (property.Value is JObject subObject)
                {

                    if (JSON.ContainsKey(property.Key))
                    {

                        if (JSON[property.Key] is JObject oldSubObject)
                        {

                            //ToDo: Perhaps use a more generic JSON patch here!
                            // PatchObject.Apply(ToJSON(), EVSEPatch),
                            var patchResult = TryPrivatePatch(oldSubObject, subObject);

                            if (patchResult.IsSuccess)
                                JSON[property.Key] = patchResult.PatchedData;

                        }

                        else
                            JSON[property.Key] = subObject;

                    }

                    else
                        JSON.Add(property.Key, subObject);

                }

                //else if (property.Value is JArray subArray)
                //{
                //}

                else
                    JSON[property.Key] = property.Value;

            }

            return PatchResult<JObject>.Success(JSON);

        }

        #endregion

        #region TryPatch(TokenPatch, AllowDowngrades = false)

        /// <summary>
        /// Try to patch the JSON representaion of this token.
        /// </summary>
        /// <param name="TokenPatch">The JSON merge patch.</param>
        /// <param name="AllowDowngrades">Allow to set the 'lastUpdated' timestamp to an earlier value.</param>
        public PatchResult<Token> TryPatch(JObject  TokenPatch,
                                           Boolean  AllowDowngrades = false)
        {

            if (TokenPatch == null)
                return PatchResult<Token>.Failed(this,
                                                 "The given token patch must not be null!");

            lock (patchLock)
            {

                if (TokenPatch["last_updated"] is null)
                    TokenPatch["last_updated"] = Timestamp.Now.ToIso8601();

                else if (AllowDowngrades == false &&
                        TokenPatch["last_updated"].Type == JTokenType.Date &&
                       (TokenPatch["last_updated"].Value<DateTime>().ToIso8601().CompareTo(LastUpdated.ToIso8601()) < 1))
                {
                    return PatchResult<Token>.Failed(this,
                                                     "The 'lastUpdated' timestamp of the token patch must be newer then the timestamp of the existing token!");
                }


                var patchResult = TryPrivatePatch(ToJSON(), TokenPatch);


                if (patchResult.IsFailed)
                    return PatchResult<Token>.Failed(this,
                                                     patchResult.ErrorResponse);

                if (TryParse(patchResult.PatchedData,
                             out Token   PatchedToken,
                             out String  ErrorResponse))
                {

                    return PatchResult<Token>.Success(PatchedToken,
                                                      ErrorResponse);

                }

                else
                    return PatchResult<Token>.Failed(this,
                                                     "Invalid JSON merge patch of a token: " + ErrorResponse);

            }

        }

        #endregion


        #region CalcSHA256Hash(CustomTokenSerializer = null, CustomEnergyContractSerializer = null, ...)

        /// <summary>
        /// Calculate the SHA256 hash of the JSON representation of this charging tariff in HEX.
        /// </summary>
        /// <param name="CustomTokenSerializer">A delegate to serialize custom token JSON objects.</param>
        /// <param name="CustomEnergyContractSerializer">A delegate to serialize custom energy contract JSON objects.</param>
        public String CalcSHA256Hash(CustomJObjectSerializerDelegate<Token>           CustomTokenSerializer            = null,
                                     CustomJObjectSerializerDelegate<EnergyContract>  CustomEnergyContractSerializer   = null)
        {

            using (var SHA256 = new SHA256Managed())
            {

                return SHA256Hash = "0x" + SHA256.ComputeHash(Encoding.Unicode.GetBytes(
                                                                  ToJSON(CustomTokenSerializer,
                                                                         CustomEnergyContractSerializer).
                                                                  ToString(Newtonsoft.Json.Formatting.None)
                                                              )).
                                                  Select(value => String.Format("{0:x2}", value)).
                                                  Aggregate();

            }

        }

        #endregion


        #region Operator overloading

        #region Operator == (Token1, Token2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Token1">A token.</param>
        /// <param name="Token2">Another token.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Token Token1,
                                           Token Token2)
        {

            if (Object.ReferenceEquals(Token1, Token2))
                return true;

            if (Token1 is null || Token2 is null)
                return false;

            return Token1.Equals(Token2);

        }

        #endregion

        #region Operator != (Token1, Token2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Token1">A token.</param>
        /// <param name="Token2">Another token.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Token Token1,
                                           Token Token2)

            => !(Token1 == Token2);

        #endregion

        #region Operator <  (Token1, Token2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Token1">A token.</param>
        /// <param name="Token2">Another token.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Token Token1,
                                          Token Token2)

            => Token1 is null
                   ? throw new ArgumentNullException(nameof(Token1), "The given token must not be null!")
                   : Token1.CompareTo(Token2) < 0;

        #endregion

        #region Operator <= (Token1, Token2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Token1">A token.</param>
        /// <param name="Token2">Another token.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Token Token1,
                                           Token Token2)

            => !(Token1 > Token2);

        #endregion

        #region Operator >  (Token1, Token2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Token1">A token.</param>
        /// <param name="Token2">Another token.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Token Token1,
                                          Token Token2)

            => Token1 is null
                   ? throw new ArgumentNullException(nameof(Token1), "The given token must not be null!")
                   : Token1.CompareTo(Token2) > 0;

        #endregion

        #region Operator >= (Token1, Token2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Token1">A token.</param>
        /// <param name="Token2">Another token.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Token Token1,
                                           Token Token2)

            => !(Token1 < Token2);

        #endregion

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

            => Token is null
                   ? throw new ArgumentNullException(nameof(Token), "The given token must not be null!")
                   : Id.CompareTo(Token.Id);

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

            => !(Token is null) &&
                   Id.Equals(Token.Id);

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
        /// Get a text representation of this object.
        /// </summary>
        public override String ToString()

            => Id.ToString();

        #endregion

    }

}
