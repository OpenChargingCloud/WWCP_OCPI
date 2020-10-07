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
using Newtonsoft.Json.Linq;

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
                         out Token  token,
                         out String   ErrorResponse,
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

                if (JSON.ParseOptionalStruct("country_code",
                                             "country code",
                                             CountryCode.TryParse,
                                             out CountryCode? CountryCodeBody,
                                             out ErrorResponse))
                {

                    if (ErrorResponse != null)
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

                if (JSON.ParseOptionalStruct("party_id",
                                             "party identification",
                                             Party_Id.TryParse,
                                             out Party_Id? PartyIdBody,
                                             out ErrorResponse))
                {

                    if (ErrorResponse != null)
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

                if (JSON.ParseOptionalStruct("id",
                                             "token identification",
                                             Token_Id.TryParse,
                                             out Token_Id? TokenIdBody,
                                             out ErrorResponse))
                {

                    if (ErrorResponse != null)
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

                #region Parse Publish               [mandatory]

                if (!JSON.ParseMandatory("publish",
                                         "publish",
                                         out Boolean Publish,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Address               [mandatory]

                if (!JSON.ParseMandatoryText("address",
                                             "address",
                                             out String Address,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse City                  [mandatory]

                if (!JSON.ParseMandatoryText("city",
                                             "city",
                                             out String City,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Country               [mandatory]

                if (!JSON.ParseMandatoryText("country",
                                             "country",
                                             out String Country,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Coordinates           [mandatory]

                //if (!JSON.ParseMandatoryJSON("coordinates",
                //                             "geo coordinates",
                //                             GeoCoordinate.TryParse,
                //                             out GeoCoordinate Coordinates,
                //                             out ErrorResponse))
                //{
                //    return false;
                //}

                #endregion

                #region Parse TimeZone              [mandatory]

                if (!JSON.ParseMandatoryText("time_zone",
                                             "time zone",
                                             out String TimeZone,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion


                #region Parse PublishTokenTypes     [optional]

                if (JSON.ParseOptionalJSON("publish_allowed_to",
                                           "publish allowed to",
                                           PublishTokenType.TryParse,
                                           out IEnumerable<PublishTokenType> PublishTokenTypes,
                                           out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse Name                  [optional]

                var Name = JSON.GetString("name");

                #endregion

                #region Parse PostalCode            [optional]

                var PostalCode = JSON.GetString("postal_code");

                #endregion

                #region Parse State                 [optional]

                var State = JSON.GetString("state");

                #endregion

                #region Parse RelatedTokens      [optional]

                //if (JSON.ParseOptionalJSON("related_tokens",
                //                           "related tokens",
                //                           AdditionalGeoToken.TryParse,
                //                           out IEnumerable<AdditionalGeoToken> RelatedTokens,
                //                           out ErrorResponse))
                //{

                //    if (ErrorResponse != null)
                //        return false;

                //}

                #endregion

                #region Parse ParkingType           [optional]

                if (JSON.ParseOptionalEnum("parking_type",
                                           "parking type",
                                           out ParkingTypes? ParkingType,
                                           out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse EVSEs                 [optional]

                if (JSON.ParseOptionalJSON("evses",
                                           "evses",
                                           EVSE.TryParse,
                                           out IEnumerable<EVSE> EVSEs,
                                           out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse Directions            [optional]

                if (JSON.ParseOptionalJSON("directions",
                                           "multi-language directions",
                                           DisplayText.TryParse,
                                           out IEnumerable<DisplayText> Directions,
                                           out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse Operator              [optional]

                if (JSON.ParseOptionalJSON("operator",
                                           "operator",
                                           BusinessDetails.TryParse,
                                           out BusinessDetails Operator,
                                           out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse Suboperator           [optional]

                if (JSON.ParseOptionalJSON("suboperator",
                                           "suboperator",
                                           BusinessDetails.TryParse,
                                           out BusinessDetails Suboperator,
                                           out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse Owner                 [optional]

                if (JSON.ParseOptionalJSON("owner",
                                           "owner",
                                           BusinessDetails.TryParse,
                                           out BusinessDetails Owner,
                                           out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse Facilities            [optional]

                if (JSON.ParseOptionalEnums("facilities",
                                            "facilities",
                                            out IEnumerable<Facilities> Facilities,
                                            out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse OpeningTimes          [optional]

                if (JSON.ParseOptionalJSON("opening_times",
                                           "opening times",
                                           Hours.TryParse,
                                           out Hours OpeningTimes,
                                           out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse ChargingWhenClosed    [optional]

                if (JSON.ParseOptional("charging_when_closed",
                                       "charging when closed",
                                       out Boolean? ChargingWhenClosed,
                                       out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse Images                [optional]

                if (JSON.ParseOptionalJSON("images",
                                           "images",
                                           Image.TryParse,
                                           out IEnumerable<Image> Images,
                                           out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse EnergyMix             [optional]

                if (JSON.ParseOptionalJSON("energy_mix",
                                           "energy mix",
                                           OCPIv2_2.EnergyMix.TryParse,
                                           out EnergyMix EnergyMix,
                                           out ErrorResponse))
                {

                    if (ErrorResponse != null)
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


                //Token = new Token(CountryCodeBody ?? CountryCodeURL.Value,
                //                        PartyIdBody     ?? PartyIdURL.Value,
                //                        TokenIdBody  ?? TokenIdURL.Value,
                //                        Publish,
                //                        Address?.   Trim(),
                //                        City?.      Trim(),
                //                        Country?.   Trim(),
                //                        Coordinates,
                //                        TimeZone?.  Trim(),

                //                        PublishTokenTypes,
                //                        Name?.      Trim(),
                //                        PostalCode?.Trim(),
                //                        State?.     Trim(),
                //                        RelatedTokens?.Distinct(),
                //                        ParkingType,
                //                        EVSEs?.           Distinct(),
                //                        Directions?.      Distinct(),
                //                        Operator,
                //                        Suboperator,
                //                        Owner,
                //                        Facilities?.      Distinct(),
                //                        OpeningTimes,
                //                        ChargingWhenClosed,
                //                        Images?.          Distinct(),
                //                        EnergyMix,
                //                        LastUpdated);

                Token = null;

                if (CustomTokenParser != null)
                    Token = CustomTokenParser(JSON,
                                                  Token);

                return true;

            }
            catch (Exception e)
            {
                Token        = default;
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
                Token        = null;
                ErrorResponse  = "The given text representation of a token is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomTokenSerializer = null, CustomEVSESerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomTokenSerializer">A delegate to serialize custom token JSON objects.</param>
        /// <param name="CustomPublishTokenTypeSerializer">A delegate to serialize custom publish token type JSON objects.</param>
        /// <param name="CustomAdditionalGeoTokenSerializer">A delegate to serialize custom additional geo token JSON objects.</param>
        /// <param name="CustomEVSESerializer">A delegate to serialize custom EVSE JSON objects.</param>
        /// <param name="CustomStatusScheduleSerializer">A delegate to serialize custom status schedule JSON objects.</param>
        /// <param name="CustomConnectorSerializer">A delegate to serialize custom connector JSON objects.</param>
        /// <param name="CustomDisplayTextSerializer">A delegate to serialize custom multi-language text JSON objects.</param>
        /// <param name="CustomBusinessDetailsSerializer">A delegate to serialize custom business details JSON objects.</param>
        /// <param name="CustomHoursSerializer">A delegate to serialize custom hours JSON objects.</param>
        /// <param name="CustomImageSerializer">A delegate to serialize custom image JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<Token>                  CustomTokenSerializer                = null,
                              CustomJObjectSerializerDelegate<PublishTokenType>       CustomPublishTokenTypeSerializer     = null,
                              CustomJObjectSerializerDelegate<AdditionalGeoLocation>  CustomAdditionalGeoTokenSerializer   = null,
                              CustomJObjectSerializerDelegate<EVSE>                   CustomEVSESerializer                 = null,
                              CustomJObjectSerializerDelegate<StatusSchedule>         CustomStatusScheduleSerializer       = null,
                              CustomJObjectSerializerDelegate<Connector>              CustomConnectorSerializer            = null,
                              CustomJObjectSerializerDelegate<DisplayText>            CustomDisplayTextSerializer          = null,
                              CustomJObjectSerializerDelegate<BusinessDetails>        CustomBusinessDetailsSerializer      = null,
                              CustomJObjectSerializerDelegate<Hours>                  CustomHoursSerializer                = null,
                              CustomJObjectSerializerDelegate<Image>                  CustomImageSerializer                = null)
        {

            var JSON = JSONObject.Create(

                           new JProperty("country_code",                    CountryCode.ToString()),
                           new JProperty("party_id",                        PartyId.    ToString()),
                           new JProperty("id",                              Id.         ToString()),
                           //new JProperty("publish",                         Publish),

                           //Publish == false && PublishAllowedTo.SafeAny()
                           //    ? new JProperty("publish_allowed_to",        new JArray(PublishAllowedTo.Select(publishAllowedTo => publishAllowedTo.ToJSON(CustomPublishTokenTypeSerializer))))
                           //    : null,

                           //Name.IsNotNullOrEmpty()
                           //    ? new JProperty("name",                      Name)
                           //    : null,

                           //new JProperty("address",                         Address),
                           //new JProperty("city",                            City),

                           //PostalCode.IsNotNullOrEmpty()
                           //    ? new JProperty("postal_code",               PostalCode)
                           //    : null,

                           //State.IsNotNullOrEmpty()
                           //    ? new JProperty("state",                     State)
                           //    : null,

                           //new JProperty("country",                         Country),
                           //new JProperty("coordinates",                     new JObject(
                           //                                                     new JProperty("latitude",  Coordinates.Latitude. Value.ToString()),
                           //                                                     new JProperty("longitude", Coordinates.Longitude.Value.ToString())
                           //                                                 )),

                           //RelatedTokens.SafeAny()
                           //    ? new JProperty("related_tokens",         new JArray(RelatedTokens.Select(token => token.ToJSON(CustomAdditionalGeoTokenSerializer))))
                           //    : null,

                           //ParkingType.HasValue
                           //    ? new JProperty("parking_type",              ParkingType.Value.ToString())
                           //    : null,

                           //EVSEs.SafeAny()
                           //    ? new JProperty("evses",                     new JArray(EVSEs.Select(evse => evse.ToJSON(CustomEVSESerializer,
                           //                                                                                             CustomStatusScheduleSerializer,
                           //                                                                                             CustomConnectorSerializer))))
                           //    : null,

                           //Directions.SafeAny()
                           //    ? new JProperty("directions",                new JArray(Directions.Select(evse => evse.ToJSON(CustomDisplayTextSerializer))))
                           //    : null,

                           //Operator != null
                           //    ? new JProperty("operator",                  Operator.   ToJSON(CustomBusinessDetailsSerializer))
                           //    : null,

                           //SubOperator != null
                           //    ? new JProperty("suboperator",               SubOperator.ToJSON(CustomBusinessDetailsSerializer))
                           //    : null,

                           //Owner != null
                           //    ? new JProperty("owner",                     Owner.      ToJSON(CustomBusinessDetailsSerializer))
                           //    : null,

                           //Facilities.SafeAny()
                           //    ? new JProperty("facilities",                new JArray(Facilities.Select(facility => facility.ToString())))
                           //    : null,

                           //new JProperty("time_zone",                       Timezone),

                           //OpeningTimes != null
                           //    ? new JProperty("opening_times",             OpeningTimes.ToJSON(CustomHoursSerializer))
                           //    : null,

                           //ChargingWhenClosed.HasValue
                           //    ? new JProperty("charging_when_closed",      ChargingWhenClosed.Value)
                           //    : null,

                           //Images.SafeAny()
                           //    ? new JProperty("images",                    new JArray(Images.Select(image => image.ToJSON(CustomImageSerializer))))
                           //    : null,

                           //EnergyMix != null
                           //    ? new JProperty("energy_mix",                EnergyMix.ToJSON())
                           //    : null,

                           new JProperty("last_updated",                    LastUpdated.ToIso8601())

                       );

            return CustomTokenSerializer != null
                       ? CustomTokenSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Patch(TokenPatch)

        public Token Patch(JObject TokenPatch)
        {

            return this;

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
        /// Get a string representation of this object.
        /// </summary>
        public override String ToString()

            => Id.ToString();

        #endregion

    }

}
