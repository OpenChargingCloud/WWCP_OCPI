﻿/*
 * Copyright (c) 2015-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using System.Security.Cryptography;
using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPI;
using cloud.charging.open.protocols.OCPIv2_1_1.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_1_1
{

    /// <summary>
    /// The token object describes the charging session and its costs,
    /// how these costs are composed, etc.
    /// </summary>
    public class Token : IHasId<Token_Id>,
                         IEquatable<Token>,
                         IComparable<Token>,
                         IComparable
    {

        #region Data

        private readonly Lock patchLock = new();

        #endregion

        #region Properties

        /// <summary>
        /// The parent CommonAPI of this charging location.
        /// </summary>
        internal CommonAPI?      CommonAPI         { get; set; }

        /// <summary>
        /// The ISO-3166 alpha-2 country code of the charge point operator that 'owns' this token.
        /// </summary>
        [Mandatory]
        public   CountryCode     CountryCode       { get; }

        /// <summary>
        /// The party identification of the charge point operator that 'owns' this token (following the ISO-15118 standard).
        /// </summary>
        [Mandatory]
        public   Party_Id        PartyId           { get; }

        /// <summary>
        /// The unique identification of the token.
        /// </summary>
        [Mandatory]
        public   Token_Id        Id               { get; }

        /// <summary>
        /// The type of the token.
        /// </summary>
        [Mandatory]
        public   TokenType       Type             { get; }

        /// <summary>
        /// The unique identification of the EV driver contract token within the eMSP’s platform.
        /// </summary>
        [Mandatory]
        public   Auth_Id         AuthId           { get; }

        /// <summary>
        /// The token issuing company, most of the times the name of the company printed on
        /// the token (RFID card), not necessarily the eMSP.
        /// string(64)
        /// </summary>
        [Mandatory]
        public   String          Issuer           { get; }

        /// <summary>
        /// Whether this token is valid.
        /// </summary>
        [Mandatory]
        public   Boolean         IsValid          { get; }

        /// <summary>
        /// Indicates what type of white-listing is allowed.
        /// </summary>
        [Mandatory]
        public   WhitelistTypes  WhitelistType    { get; }

        /// <summary>
        /// The optional visual readable number/identification as printed on the token/RFID card.
        /// string(64)
        /// </summary>
        [Optional]
        public   String?         VisualNumber     { get; }

        /// <summary>
        /// The optional ISO 639-1 language code of the token owner’s preferred interface language.
        /// </summary>
        [Optional]
        public   Languages?      UILanguage       { get; }

        /// <summary>
        /// The timestamp when this token was created.
        /// </summary>
        [Mandatory, VendorExtension(VE.GraphDefined, VE.Pagination)]
        public   DateTime        Created          { get; }

        /// <summary>
        /// The timestamp when this token was last updated (or created).
        /// </summary>
        [Mandatory]
        public   DateTime        LastUpdated      { get; }

        /// <summary>
        /// The base64 encoded SHA256 hash of the JSON representation of this token.
        /// </summary>
        public   String          ETag             { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new token describing the charging session and its costs,
        /// how these costs are composed, etc.
        /// </summary>
        /// <param name="CountryCode">The ISO-3166 alpha-2 country code of the charge point operator that 'owns' this token.</param>
        /// <param name="PartyId">The party identification of the charge point operator that 'owns' this token (following the ISO-15118 standard).</param>
        /// <param name="Id">An unique identification of the token.</param>
        /// <param name="Type">The type of the token.</param>
        /// <param name="AuthId">An unique identification of the EV driver contract token within the eMSP’s platform.</param>
        /// <param name="Issuer">An token issuing company, most of the times the name of the company printed on the token (RFID card), not necessarily the eMSP.</param>
        /// <param name="IsValid">Whether this token is valid.</param>
        /// 
        /// <param name="WhitelistType">Indicates what type of white-listing is allowed.</param>
        /// <param name="VisualNumber">An optional visual readable number/identification as printed on the token/RFID card.</param>
        /// <param name="UILanguage">An optional ISO 639-1 language code of the token owner’s preferred interface language.</param>
        /// 
        /// <param name="Created">An optional timestamp when this charging token was created.</param>
        /// <param name="LastUpdated">An optional timestamp when this charging token was last updated (or created).</param>
        /// 
        /// <param name="CustomTokenSerializer">A delegate to serialize custom token JSON objects.</param>
        public Token(CountryCode                              CountryCode,
                     Party_Id                                 PartyId,
                     Token_Id                                 Id,
                     TokenType                                Type,
                     Auth_Id                                  AuthId,
                     String                                   Issuer,
                     Boolean                                  IsValid,

                     WhitelistTypes                           WhitelistType,
                     String?                                  VisualNumber            = null,
                     Languages?                               UILanguage              = null,

                     DateTime?                                Created                 = null,
                     DateTime?                                LastUpdated             = null,

                     CustomJObjectSerializerDelegate<Token>?  CustomTokenSerializer   = null)
        {

            if (Issuer.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Issuer), "The given issuer must not be null or empty!");

            this.CountryCode    = CountryCode;
            this.PartyId        = PartyId;
            this.Id             = Id;
            this.Type           = Type;
            this.AuthId         = AuthId;
            this.Issuer         = Issuer;
            this.IsValid        = IsValid;

            this.WhitelistType  = WhitelistType;
            this.VisualNumber   = VisualNumber;
            this.UILanguage     = UILanguage;

            this.Created        = Created     ?? LastUpdated ?? Timestamp.Now;
            this.LastUpdated    = LastUpdated ?? Created     ?? Timestamp.Now;

            this.ETag           = SHA256.HashData(ToJSON(true,
                                                         CustomTokenSerializer).ToUTF8Bytes()).ToBase64();

            unchecked
            {

                hashCode =  this.CountryCode.  GetHashCode()       * 37 ^
                            this.PartyId.      GetHashCode()       * 31 ^
                            this.Id.           GetHashCode()       * 29 ^
                            this.Type.         GetHashCode()       * 23 ^
                            this.AuthId.       GetHashCode()       * 19 ^
                            this.Issuer.       GetHashCode()       * 17 ^
                            this.IsValid.      GetHashCode()       * 13 ^
                            this.WhitelistType.GetHashCode()       * 11 ^
                            this.Created.      GetHashCode()       *  7 ^
                            this.LastUpdated.  GetHashCode()       *  5 ^
                           (this.VisualNumber?.GetHashCode() ?? 0) *  3 ^
                           (this.UILanguage?.  GetHashCode() ?? 0);

            }

        }

        #endregion


        #region (static) Parse   (JSON, CountryCodeURL = null, PartyIdURL = null, TokenIdURL = null, CustomTokenParser = null)

        /// <summary>
        /// Parse the given JSON representation of a token.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CountryCodeURL">An optional country code, e.g. from the HTTP URL.</param>
        /// <param name="PartyIdURL">An optional party identification, e.g. from the HTTP URL.</param>
        /// <param name="TokenIdURL">An optional token identification, e.g. from the HTTP URL.</param>
        /// <param name="CustomTokenParser">A delegate to parse custom token JSON objects.</param>
        public static Token Parse(JObject                              JSON,
                                  CountryCode?                         CountryCodeURL      = null,
                                  Party_Id?                            PartyIdURL          = null,
                                  Token_Id?                            TokenIdURL          = null,
                                  CustomJObjectParserDelegate<Token>?  CustomTokenParser   = null)
        {

            if (TryParse(JSON,
                         out var token,
                         out var errorResponse,
                         CountryCodeURL,
                         PartyIdURL,
                         TokenIdURL,
                         CustomTokenParser))
            {
                return token!;
            }

            throw new ArgumentException("The given JSON representation of a token is invalid: " + errorResponse,
                                        nameof(JSON));

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
        public static Boolean TryParse(JObject                           JSON,
                                       [NotNullWhen(true)]  out Token?   Token,
                                       [NotNullWhen(false)] out String?  ErrorResponse)

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
        /// <param name="TokenIdURL">An optional token identification, e.g. from the HTTP URL.</param>
        /// <param name="CustomTokenParser">A delegate to parse custom token JSON objects.</param>
        public static Boolean TryParse(JObject                              JSON,
                                       [NotNullWhen(true)]  out Token?      Token,
                                       [NotNullWhen(false)] out String?     ErrorResponse,
                                       CountryCode?                         CountryCodeURL      = null,
                                       Party_Id?                            PartyIdURL          = null,
                                       Token_Id?                            TokenIdURL          = null,
                                       CustomJObjectParserDelegate<Token>?  CustomTokenParser   = null)
        {

            try
            {

                Token = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse CountryCode      [optional, internal]

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

                #region Parse PartyIdURL       [optional, internal]

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

                #region Parse Id               [optional]

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

                #region Parse Type             [mandatory]

                if (!JSON.ParseMandatory("type",
                                         "token type",
                                         TokenType.TryParse,
                                         out TokenType Type,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse AuthId           [mandatory]

                if (!JSON.ParseMandatory("auth_id",
                                         "auth identification",
                                         Auth_Id.TryParse,
                                         out Auth_Id AuthId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Issuer           [mandatory]

                if (!JSON.ParseMandatoryText("issuer",
                                             "issuer",
                                             out String Issuer,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse IsValid          [mandatory]

                if (!JSON.ParseMandatory("valid",
                                         "token is valid",
                                         out Boolean IsValid,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse WhitelistType    [mandatory]

                if (!JSON.ParseMandatory("whitelist",
                                         "whitelist type",
                                         WhitelistTypesExtensions.TryParse,
                                         out WhitelistTypes WhitelistType,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse VisualNumber     [optional]

                var VisualNumber = JSON.GetString("visual_number");

                #endregion

                #region Parse UILanguage       [optional]

                if (JSON.ParseOptionalEnum("language",
                                           "user-interface language",
                                           out Languages? UILanguage,
                                           out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                }

                #endregion


                #region Parse Created          [optional, NonStandard]

                if (JSON.ParseOptional("created",
                                       "created",
                                       out DateTime? Created,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse LastUpdated      [mandatory]

                if (!JSON.ParseMandatory("last_updated",
                                         "last updated",
                                         out DateTime LastUpdated,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                Token = new Token(CountryCodeBody ?? CountryCodeURL!.Value,
                                  PartyIdBody     ?? PartyIdURL!.    Value,
                                  TokenIdBody     ?? TokenIdURL!.    Value,
                                  Type,
                                  AuthId,
                                  Issuer,
                                  IsValid,
                                  WhitelistType,
                                  VisualNumber,
                                  UILanguage,
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

        #region ToJSON(IncludeOwnerInformation = false, CustomTokenSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="IncludeOwnerInformation">Include optional owner information.</param>
        /// <param name="CustomTokenSerializer">A delegate to serialize custom token JSON objects.</param>
        public JObject ToJSON(Boolean                                  IncludeOwnerInformation   = false,
                              CustomJObjectSerializerDelegate<Token>?  CustomTokenSerializer     = null)
        {

            var json = JSONObject.Create(

                           IncludeOwnerInformation
                               ? new JProperty("country_code",    CountryCode.     ToString())
                               : null,

                           IncludeOwnerInformation
                               ? new JProperty("party_id",        PartyId.         ToString())
                               : null,

                                 new JProperty("uid",             Id.              ToString()),
                                 new JProperty("type",            Type.            ToString()),
                                 new JProperty("auth_id",         AuthId.          ToString()),

                           VisualNumber.IsNotNullOrEmpty()
                               ? new JProperty("visual_number",   VisualNumber)
                               : null,

                                 new JProperty("issuer",          Issuer),

                                 new JProperty("valid",           IsValid),
                                 new JProperty("whitelist",       WhitelistType.   AsText()),

                           UILanguage.HasValue
                               ? new JProperty("language",        UILanguage.Value.ToString())
                               : null,

                                 new JProperty("last_updated",    LastUpdated.     ToISO8601())

                       );

            return CustomTokenSerializer is not null
                       ? CustomTokenSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this object.
        /// </summary>
        public Token Clone()

            => new (
                   CountryCode. Clone(),
                   PartyId.     Clone(),
                   Id.          Clone(),
                   Type.        Clone(),
                   AuthId.      Clone(),
                   Issuer.      CloneString(),
                   IsValid,
                   WhitelistType,
                   VisualNumber.CloneNullableString(),
                   UILanguage,

                   Created,
                   LastUpdated
               );

        #endregion


        #region (private) TryPrivatePatch(JSON, Patch)

        private PatchResult<JObject> TryPrivatePatch(JObject           JSON,
                                                     JObject           Patch,
                                                     EventTracking_Id  EventTrackingId)
        {

            foreach (var property in Patch)
            {

                if (property.Key == "id")
                    return PatchResult<JObject>.Failed(EventTrackingId, JSON,
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
                            var patchResult = TryPrivatePatch(oldSubObject, subObject, EventTrackingId);

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

            return PatchResult<JObject>.Success(EventTrackingId, JSON);

        }

        #endregion

        #region TryPatch(TokenPatch, AllowDowngrades = false)

        /// <summary>
        /// Try to patch the JSON representation of this token.
        /// </summary>
        /// <param name="TokenPatch">The JSON merge patch.</param>
        /// <param name="AllowDowngrades">Allow to set the 'lastUpdated' timestamp to an earlier value.</param>
        public PatchResult<Token> TryPatch(JObject           TokenPatch,
                                           Boolean           AllowDowngrades   = false,
                                           EventTracking_Id? EventTrackingId   = null)
        {

            EventTrackingId ??= EventTracking_Id.New;

            if (!TokenPatch.HasValues)
                return PatchResult<Token>.Failed(EventTrackingId,
                                                 this,
                                                 "The given token patch must not be null or empty!");

            lock (patchLock)
            {

                if (TokenPatch["last_updated"] is null)
                    TokenPatch["last_updated"] = Timestamp.Now.ToISO8601();

                else if (AllowDowngrades == false &&
                        TokenPatch["last_updated"].Type == JTokenType.Date &&
                       (TokenPatch["last_updated"].Value<DateTime>().ToISO8601().CompareTo(LastUpdated.ToISO8601()) < 1))
                {
                    return PatchResult<Token>.Failed(EventTrackingId, this,
                                                     "The 'lastUpdated' timestamp of the token patch must be newer then the timestamp of the existing token!");
                }


                var patchResult = TryPrivatePatch(ToJSON(), TokenPatch, EventTrackingId);


                if (patchResult.IsFailed)
                    return PatchResult<Token>.Failed(EventTrackingId, this,
                                                     patchResult.ErrorResponse);

                if (TryParse(patchResult.PatchedData,
                             out var PatchedToken,
                             out var ErrorResponse))
                {

                    return PatchResult<Token>.Success(EventTrackingId, PatchedToken,
                                                      ErrorResponse);

                }

                else
                    return PatchResult<Token>.Failed(EventTrackingId, this,
                                                     "Invalid JSON merge patch of a token: " + ErrorResponse);

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
        public static Boolean operator == (Token? Token1,
                                           Token? Token2)
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
        public static Boolean operator != (Token? Token1,
                                           Token? Token2)

            => !(Token1 == Token2);

        #endregion

        #region Operator <  (Token1, Token2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Token1">A token.</param>
        /// <param name="Token2">Another token.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Token? Token1,
                                          Token? Token2)

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
        public static Boolean operator <= (Token? Token1,
                                           Token? Token2)

            => !(Token1 > Token2);

        #endregion

        #region Operator >  (Token1, Token2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Token1">A token.</param>
        /// <param name="Token2">Another token.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Token? Token1,
                                          Token? Token2)

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
        public static Boolean operator >= (Token? Token1,
                                           Token? Token2)

            => !(Token1 < Token2);

        #endregion

        #endregion

        #region IComparable<Token> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two tokens.
        /// </summary>
        /// <param name="Object">A token to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Token token
                   ? CompareTo(token)
                   : throw new ArgumentException("The given object is not a token!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(Token)

        /// <summary>
        /// Compares two tokens.
        /// </summary>
        /// <param name="Token">A token to compare with.</param>
        public Int32 CompareTo(Token? Token)
        {

            if (Token is null)
                throw new ArgumentNullException(nameof(Token), "The given token must not be null!");

            var c = CountryCode.  CompareTo(Token.CountryCode);

            if (c == 0)
                c = PartyId.      CompareTo(Token.PartyId);

            if (c == 0)
                c = Id.           CompareTo(Token.Id);

            if (c == 0)
                c = Type.         CompareTo(Token.Type);

            if (c == 0)
                c = AuthId.       CompareTo(Token.AuthId);

            if (c == 0)
                c = Issuer.       CompareTo(Token.Issuer);

            if (c == 0)
                c = IsValid.      CompareTo(Token.IsValid);

            if (c == 0)
                c = WhitelistType.CompareTo(Token.WhitelistType);

            if (c == 0)
                c = Created.      CompareTo(Token.Created);

            if (c == 0)
                c = LastUpdated.  CompareTo(Token.LastUpdated);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<Token> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two tokens for equality.
        /// </summary>
        /// <param name="Object">A token to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Token token &&
                   Equals(token);

        #endregion

        #region Equals(Token)

        /// <summary>
        /// Compares two tokens for equality.
        /// </summary>
        /// <param name="Token">A token to compare with.</param>
        public Boolean Equals(Token? Token)

            => Token is not null &&

               CountryCode.            Equals(Token.Id)                      &&
               PartyId.                Equals(Token.Id)                      &&
               Id.                     Equals(Token.Id)                      &&
               Type.                   Equals(Token.Type)                    &&
               AuthId.                 Equals(Token.AuthId)                  &&
               Issuer.                 Equals(Token.Issuer)                  &&
               IsValid.                Equals(Token.IsValid)                 &&
               WhitelistType.          Equals(Token.WhitelistType)           &&
               Created.    ToISO8601().Equals(Token.Created.    ToISO8601()) &&
               LastUpdated.ToISO8601().Equals(Token.LastUpdated.ToISO8601()) &&

             ((VisualNumber is     null &&  Token.VisualNumber is     null) ||
              (VisualNumber is not null &&  Token.VisualNumber is not null && VisualNumber.      Equals(Token.VisualNumber))) &&

            ((!UILanguage.  HasValue    && !Token.UILanguage.  HasValue)    ||
              (UILanguage.  HasValue    &&  Token.UILanguage.  HasValue    && UILanguage.  Value.Equals(Token.UILanguage.Value)));

        #endregion

        #endregion

        #region (override) GetHashCode()

        private readonly Int32 hashCode;

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        public override Int32 GetHashCode()
            => hashCode;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   Id,          " (",
                   CountryCode, "-",
                   PartyId,     ", ",
                   Type,        ", ",
                   AuthId,      ", ",
                   Issuer,      ", ",

                   IsValid
                       ? "valid"
                       : "not valid",

                   ") "

               );

        #endregion

    }

}
