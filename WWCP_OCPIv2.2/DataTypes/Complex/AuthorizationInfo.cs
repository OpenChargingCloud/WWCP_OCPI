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
using Newtonsoft.Json.Linq;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2
{

    /// <summary>
    /// An authorization information.
    /// </summary>
    public class AuthorizationInfo : IEquatable<AuthorizationInfo>
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


        #region (static) Parse   (JSON, CustomAuthorizationInfoParser = null)

        /// <summary>
        /// Parse the given JSON representation of an authorization information.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomAuthorizationInfoParser">A delegate to parse custom authorization information JSON objects.</param>
        public static AuthorizationInfo Parse(JObject                                         JSON,
                                              CustomJObjectParserDelegate<AuthorizationInfo>  CustomAuthorizationInfoParser   = null)
        {

            if (TryParse(JSON,
                         out AuthorizationInfo  authorizationInfo,
                         out String   ErrorResponse,
                         CustomAuthorizationInfoParser))
            {
                return authorizationInfo;
            }

            throw new ArgumentException("The given JSON representation of an authorization information is invalid: " + ErrorResponse, nameof(JSON));

        }

        #endregion

        #region (static) Parse   (Text, CustomAuthorizationInfoParser = null)

        /// <summary>
        /// Parse the given text representation of an authorization information.
        /// </summary>
        /// <param name="Text">The text to parse.</param>
        /// <param name="CustomAuthorizationInfoParser">A delegate to parse custom authorization information JSON objects.</param>
        public static AuthorizationInfo Parse(String                                Text,
                                    CustomJObjectParserDelegate<AuthorizationInfo>  CustomAuthorizationInfoParser   = null)
        {

            if (TryParse(Text,
                         out AuthorizationInfo  authorizationInfo,
                         out String   ErrorResponse,
                         CustomAuthorizationInfoParser))
            {
                return authorizationInfo;
            }

            throw new ArgumentException("The given text representation of an authorization information is invalid: " + ErrorResponse, nameof(Text));

        }

        #endregion

        #region (static) TryParse(JSON, out AuthorizationInfo, out ErrorResponse, CustomAuthorizationInfoParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of an authorization information.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="AuthorizationInfo">The parsed authorization information.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject      JSON,
                                       out AuthorizationInfo  AuthorizationInfo,
                                       out String   ErrorResponse)

            => TryParse(JSON,
                        out AuthorizationInfo,
                        out ErrorResponse,
                        null);

        /// <summary>
        /// Try to parse the given JSON representation of an authorization information.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="AuthorizationInfo">The parsed authorization information.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomAuthorizationInfoParser">A delegate to parse custom authorization information JSON objects.</param>
        public static Boolean TryParse(JObject                               JSON,
                                       out AuthorizationInfo                           AuthorizationInfo,
                                       out String                            ErrorResponse,
                                       CustomJObjectParserDelegate<AuthorizationInfo>  CustomAuthorizationInfoParser   = null)
        {

            try
            {

                AuthorizationInfo = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse Publish               [mandatory]

                if (!JSON.ParseMandatory("publish",
                                         "publish",
                                         out Boolean Publish,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                //#region Parse Address               [mandatory]

                //if (!JSON.ParseMandatoryText("address",
                //                             "address",
                //                             out String Address,
                //                             out ErrorResponse))
                //{
                //    return false;
                //}

                //#endregion

                //#region Parse City                  [mandatory]

                //if (!JSON.ParseMandatoryText("city",
                //                             "city",
                //                             out String City,
                //                             out ErrorResponse))
                //{
                //    return false;
                //}

                //#endregion

                //#region Parse Country               [mandatory]

                //if (!JSON.ParseMandatoryText("country",
                //                             "country",
                //                             out String Country,
                //                             out ErrorResponse))
                //{
                //    return false;
                //}

                //#endregion

                //#region Parse Coordinates           [mandatory]

                ////if (!JSON.ParseMandatoryJSON("coordinates",
                ////                             "geo coordinates",
                ////                             GeoCoordinate.TryParse,
                ////                             out GeoCoordinate Coordinates,
                ////                             out ErrorResponse))
                ////{
                ////    return false;
                ////}

                //#endregion

                //#region Parse TimeZone              [mandatory]

                //if (!JSON.ParseMandatoryText("time_zone",
                //                             "time zone",
                //                             out String TimeZone,
                //                             out ErrorResponse))
                //{
                //    return false;
                //}

                //#endregion


                //#region Parse PublishTokenTypes     [optional]

                //if (JSON.ParseOptionalJSON("publish_allowed_to",
                //                           "publish allowed to",
                //                           PublishTokenType.TryParse,
                //                           out IEnumerable<PublishTokenType> PublishTokenTypes,
                //                           out ErrorResponse))
                //{

                //    if (ErrorResponse != null)
                //        return false;

                //}

                //#endregion

                //#region Parse Name                  [optional]

                //var Name = JSON.GetString("name");

                //#endregion

                //#region Parse PostalCode            [optional]

                //var PostalCode = JSON.GetString("postal_code");

                //#endregion

                //#region Parse State                 [optional]

                //var State = JSON.GetString("state");

                //#endregion

                //#region Parse RelatedAuthorizationInfos      [optional]

                ////if (JSON.ParseOptionalJSON("related_authorizationInfos",
                ////                           "related authorizationInfos",
                ////                           AdditionalGeoAuthorizationInfo.TryParse,
                ////                           out IEnumerable<AdditionalGeoAuthorizationInfo> RelatedAuthorizationInfos,
                ////                           out ErrorResponse))
                ////{

                ////    if (ErrorResponse != null)
                ////        return false;

                ////}

                //#endregion

                //#region Parse ParkingType           [optional]

                //if (JSON.ParseOptionalEnum("parking_type",
                //                           "parking type",
                //                           out ParkingTypes? ParkingType,
                //                           out ErrorResponse))
                //{

                //    if (ErrorResponse != null)
                //        return false;

                //}

                //#endregion

                //#region Parse EVSEs                 [optional]

                //if (JSON.ParseOptionalJSON("evses",
                //                           "evses",
                //                           EVSE.TryParse,
                //                           out IEnumerable<EVSE> EVSEs,
                //                           out ErrorResponse))
                //{

                //    if (ErrorResponse != null)
                //        return false;

                //}

                //#endregion

                //#region Parse Directions            [optional]

                //if (JSON.ParseOptionalJSON("directions",
                //                           "multi-language directions",
                //                           DisplayText.TryParse,
                //                           out IEnumerable<DisplayText> Directions,
                //                           out ErrorResponse))
                //{

                //    if (ErrorResponse != null)
                //        return false;

                //}

                //#endregion

                //#region Parse Operator              [optional]

                //if (JSON.ParseOptionalJSON("operator",
                //                           "operator",
                //                           BusinessDetails.TryParse,
                //                           out BusinessDetails Operator,
                //                           out ErrorResponse))
                //{

                //    if (ErrorResponse != null)
                //        return false;

                //}

                //#endregion

                //#region Parse Suboperator           [optional]

                //if (JSON.ParseOptionalJSON("suboperator",
                //                           "suboperator",
                //                           BusinessDetails.TryParse,
                //                           out BusinessDetails Suboperator,
                //                           out ErrorResponse))
                //{

                //    if (ErrorResponse != null)
                //        return false;

                //}

                //#endregion

                //#region Parse Owner                 [optional]

                //if (JSON.ParseOptionalJSON("owner",
                //                           "owner",
                //                           BusinessDetails.TryParse,
                //                           out BusinessDetails Owner,
                //                           out ErrorResponse))
                //{

                //    if (ErrorResponse != null)
                //        return false;

                //}

                //#endregion

                //#region Parse Facilities            [optional]

                //if (JSON.ParseOptionalEnums("facilities",
                //                            "facilities",
                //                            out IEnumerable<Facilities> Facilities,
                //                            out ErrorResponse))
                //{

                //    if (ErrorResponse != null)
                //        return false;

                //}

                //#endregion

                //#region Parse OpeningTimes          [optional]

                //if (JSON.ParseOptionalJSON("opening_times",
                //                           "opening times",
                //                           Hours.TryParse,
                //                           out Hours OpeningTimes,
                //                           out ErrorResponse))
                //{

                //    if (ErrorResponse != null)
                //        return false;

                //}

                //#endregion

                //#region Parse ChargingWhenClosed    [optional]

                //if (JSON.ParseOptional("charging_when_closed",
                //                       "charging when closed",
                //                       out Boolean? ChargingWhenClosed,
                //                       out ErrorResponse))
                //{

                //    if (ErrorResponse != null)
                //        return false;

                //}

                //#endregion

                //#region Parse Images                [optional]

                //if (JSON.ParseOptionalJSON("images",
                //                           "images",
                //                           Image.TryParse,
                //                           out IEnumerable<Image> Images,
                //                           out ErrorResponse))
                //{

                //    if (ErrorResponse != null)
                //        return false;

                //}

                //#endregion

                //#region Parse EnergyMix             [optional]

                //if (JSON.ParseOptionalJSON("energy_mix",
                //                           "energy mix",
                //                           OCPIv2_2.EnergyMix.TryParse,
                //                           out EnergyMix EnergyMix,
                //                           out ErrorResponse))
                //{

                //    if (ErrorResponse != null)
                //        return false;

                //}

                //#endregion


                //#region Parse LastUpdated           [mandatory]

                //if (!JSON.ParseMandatory("last_updated",
                //                         "last updated",
                //                         out DateTime LastUpdated,
                //                         out ErrorResponse))
                //{
                //    return false;
                //}

                //#endregion


                //AuthorizationInfo = new AuthorizationInfo(CountryCodeBody ?? CountryCodeURL.Value,
                //                        PartyIdBody     ?? PartyIdURL.Value,
                //                        AuthorizationInfoIdBody  ?? AuthorizationInfoIdURL.Value,
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
                //                        RelatedAuthorizationInfos?.Distinct(),
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

                AuthorizationInfo = default;

                if (CustomAuthorizationInfoParser != null)
                    AuthorizationInfo = CustomAuthorizationInfoParser(JSON,
                                                  AuthorizationInfo);

                return true;

            }
            catch (Exception e)
            {
                AuthorizationInfo        = default;
                ErrorResponse  = "The given JSON representation of an authorization information is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Text, out AuthorizationInfo, out ErrorResponse, CustomAuthorizationInfoParser = null)

        /// <summary>
        /// Try to parse the given text representation of an authorizationInfo.
        /// </summary>
        /// <param name="Text">The text to parse.</param>
        /// <param name="AuthorizationInfo">The parsed authorizationInfo.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomAuthorizationInfoParser">A delegate to parse custom authorizationInfo JSON objects.</param>
        public static Boolean TryParse(String                                Text,
                                       out AuthorizationInfo                           AuthorizationInfo,
                                       out String                            ErrorResponse,
                                       CustomJObjectParserDelegate<AuthorizationInfo>  CustomAuthorizationInfoParser   = null)
        {

            try
            {

                return TryParse(JObject.Parse(Text),
                                out AuthorizationInfo,
                                out ErrorResponse,
                                CustomAuthorizationInfoParser);

            }
            catch (Exception e)
            {
                AuthorizationInfo        = null;
                ErrorResponse  = "The given text representation of an authorization information is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomAuthorizationInfoSerializer = null, CustomEVSESerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomAuthorizationInfoSerializer">A delegate to serialize custom authorizationInfo JSON objects.</param>
        /// <param name="CustomPublishTokenTypeSerializer">A delegate to serialize custom publish token type JSON objects.</param>
        /// <param name="CustomAdditionalGeoAuthorizationInfoSerializer">A delegate to serialize custom additional geo authorizationInfo JSON objects.</param>
        /// <param name="CustomEVSESerializer">A delegate to serialize custom EVSE JSON objects.</param>
        /// <param name="CustomStatusScheduleSerializer">A delegate to serialize custom status schedule JSON objects.</param>
        /// <param name="CustomConnectorSerializer">A delegate to serialize custom connector JSON objects.</param>
        /// <param name="CustomDisplayTextSerializer">A delegate to serialize custom multi-language text JSON objects.</param>
        /// <param name="CustomBusinessDetailsSerializer">A delegate to serialize custom business details JSON objects.</param>
        /// <param name="CustomHoursSerializer">A delegate to serialize custom hours JSON objects.</param>
        /// <param name="CustomImageSerializer">A delegate to serialize custom image JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<AuthorizationInfo>                CustomAuthorizationInfoSerializer                 = null,
                              CustomJObjectSerializerDelegate<PublishTokenType>       CustomPublishTokenTypeSerializer        = null,
                              CustomJObjectSerializerDelegate<AdditionalGeoLocation>  CustomAdditionalGeoAuthorizationInfoSerializer    = null,
                              CustomJObjectSerializerDelegate<EVSE>                   CustomEVSESerializer                    = null,
                              CustomJObjectSerializerDelegate<StatusSchedule>         CustomStatusScheduleSerializer          = null,
                              CustomJObjectSerializerDelegate<Connector>              CustomConnectorSerializer               = null,
                              CustomJObjectSerializerDelegate<DisplayText>            CustomDisplayTextSerializer             = null,
                              CustomJObjectSerializerDelegate<BusinessDetails>        CustomBusinessDetailsSerializer         = null,
                              CustomJObjectSerializerDelegate<Hours>                  CustomHoursSerializer                   = null,
                              CustomJObjectSerializerDelegate<Image>                  CustomImageSerializer                   = null)
        {

            var JSON = JSONObject.Create(

                           //new JProperty("country_code",                    CountryCode.ToString()),
                           //new JProperty("party_id",                        PartyId.    ToString()),
                           //new JProperty("id",                              Id.         ToString()),
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

                           //RelatedAuthorizationInfos.SafeAny()
                           //    ? new JProperty("related_authorizationInfos",         new JArray(RelatedAuthorizationInfos.Select(authorizationInfo => authorizationInfo.ToJSON(CustomAdditionalGeoAuthorizationInfoSerializer))))
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

                           //new JProperty("last_updated",                    LastUpdated.ToIso8601())

                       );

            return CustomAuthorizationInfoSerializer != null
                       ? CustomAuthorizationInfoSerializer(this, JSON)
                       : JSON;

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
        {

            if (Object.ReferenceEquals(AuthorizationInfo1, AuthorizationInfo2))
                return true;

            if (AuthorizationInfo1 is null || AuthorizationInfo2 is null)
                return false;

            return AuthorizationInfo1.Equals(AuthorizationInfo2);

        }

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

            => !(AuthorizationInfo is null) &&

               Allowed.               Equals(AuthorizationInfo.Allowed) &&
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
