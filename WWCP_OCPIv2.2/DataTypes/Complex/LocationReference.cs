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
    /// Reference to location details.
    /// </summary>
    public readonly struct LocationReference : IEquatable<LocationReference>,
                                               IComparable<LocationReference>,
                                               IComparable
    {

        #region Properties

        /// <summary>
        /// Unique identifier for the location.
        /// </summary>
        [Mandatory]
        public Location_Id            LocationId    { get; }

        /// <summary>
        /// Optional enumeration of EVSE identifiers within the CPO’s platform within the EVSE within the given location.
        /// </summary>
        [Optional]
        public IEnumerable<EVSE_UId>  EVSEUIds      { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new references to location details.
        /// </summary>
        /// <param name="LocationId">Unique identifier for the location..</param>
        /// <param name="EVSEUIds">Optional enumeration of EVSE identifiers within the CPO’s platform within the given location.</param>
        public LocationReference(Location_Id            LocationId,
                                 IEnumerable<EVSE_UId>  EVSEUIds)
        {

            this.LocationId  = LocationId;
            this.EVSEUIds    = EVSEUIds?.Distinct() ?? new EVSE_UId[0];

        }

        #endregion


        #region (static) Parse   (JSON, CustomLocationReferenceParser = null)

        /// <summary>
        /// Parse the given JSON representation of a location reference.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomLocationReferenceParser">A delegate to parse custom location reference JSON objects.</param>
        public static LocationReference Parse(JObject                                         JSON,
                                              CustomJObjectParserDelegate<LocationReference>  CustomLocationReferenceParser   = null)
        {

            if (TryParse(JSON,
                         out LocationReference  locationReference,
                         out String   ErrorResponse,
                         CustomLocationReferenceParser))
            {
                return locationReference;
            }

            throw new ArgumentException("The given JSON representation of a location reference is invalid: " + ErrorResponse, nameof(JSON));

        }

        #endregion

        #region (static) Parse   (Text, CustomLocationReferenceParser = null)

        /// <summary>
        /// Parse the given text representation of a location reference.
        /// </summary>
        /// <param name="Text">The text to parse.</param>
        /// <param name="CustomLocationReferenceParser">A delegate to parse custom location reference JSON objects.</param>
        public static LocationReference Parse(String                                          Text,
                                              CustomJObjectParserDelegate<LocationReference>  CustomLocationReferenceParser   = null)
        {

            if (TryParse(Text,
                         out LocationReference  locationReference,
                         out String             ErrorResponse,
                         CustomLocationReferenceParser))
            {
                return locationReference;
            }

            throw new ArgumentException("The given text representation of a location reference is invalid: " + ErrorResponse, nameof(Text));

        }

        #endregion

        #region (static) TryParse(JSON, CustomLocationReferenceParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a location reference.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomLocationReferenceParser">A delegate to parse custom location reference JSON objects.</param>
        public static LocationReference? TryParse(JObject                                         JSON,
                                                  CustomJObjectParserDelegate<LocationReference>  CustomLocationReferenceParser   = null)
        {

            if (TryParse(JSON,
                         out LocationReference  locationReference,
                         out String             ErrorResponse,
                         CustomLocationReferenceParser))
            {
                return locationReference;
            }

            return default;

        }

        #endregion

        #region (static) TryParse(Text, CustomLocationReferenceParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a location reference.
        /// </summary>
        /// <param name="Text">The JSON to parse.</param>
        /// <param name="CustomLocationReferenceParser">A delegate to parse custom location reference JSON objects.</param>
        public static LocationReference? TryParse(String                                          Text,
                                                  CustomJObjectParserDelegate<LocationReference>  CustomLocationReferenceParser   = null)
        {

            if (TryParse(Text,
                         out LocationReference  locationReference,
                         out String             ErrorResponse,
                         CustomLocationReferenceParser))
            {
                return locationReference;
            }

            return default;

        }

        #endregion

        #region (static) TryParse(JSON, out LocationReference, out ErrorResponse, CustomLocationReferenceParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a location reference.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="LocationReference">The parsed location reference.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject      JSON,
                                       out LocationReference  LocationReference,
                                       out String   ErrorResponse)

            => TryParse(JSON,
                        out LocationReference,
                        out ErrorResponse,
                        null);

        /// <summary>
        /// Try to parse the given JSON representation of a location reference.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="LocationReference">The parsed location reference.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomLocationReferenceParser">A delegate to parse custom location reference JSON objects.</param>
        public static Boolean TryParse(JObject                               JSON,
                                       out LocationReference                           LocationReference,
                                       out String                            ErrorResponse,
                                       CustomJObjectParserDelegate<LocationReference>  CustomLocationReferenceParser   = null)
        {

            try
            {

                LocationReference = default;

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

                //#region Parse RelatedLocationReferences      [optional]

                ////if (JSON.ParseOptionalJSON("related_locationReferences",
                ////                           "related locationReferences",
                ////                           AdditionalGeoLocationReference.TryParse,
                ////                           out IEnumerable<AdditionalGeoLocationReference> RelatedLocationReferences,
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


                //LocationReference = new LocationReference(CountryCodeBody ?? CountryCodeURL.Value,
                //                        PartyIdBody     ?? PartyIdURL.Value,
                //                        LocationReferenceIdBody  ?? LocationReferenceIdURL.Value,
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
                //                        RelatedLocationReferences?.Distinct(),
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

                LocationReference = default;

                if (CustomLocationReferenceParser != null)
                    LocationReference = CustomLocationReferenceParser(JSON,
                                                  LocationReference);

                return true;

            }
            catch (Exception e)
            {
                LocationReference        = default;
                ErrorResponse  = "The given JSON representation of a location reference is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Text, out LocationReference, out ErrorResponse, CustomLocationReferenceParser = null)

        /// <summary>
        /// Try to parse the given text representation of an locationReference.
        /// </summary>
        /// <param name="Text">The text to parse.</param>
        /// <param name="LocationReference">The parsed locationReference.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomLocationReferenceParser">A delegate to parse custom locationReference JSON objects.</param>
        public static Boolean TryParse(String                                          Text,
                                       out LocationReference                           LocationReference,
                                       out String                                      ErrorResponse,
                                       CustomJObjectParserDelegate<LocationReference>  CustomLocationReferenceParser   = null)
        {

            try
            {

                return TryParse(JObject.Parse(Text),
                                out LocationReference,
                                out ErrorResponse,
                                CustomLocationReferenceParser);

            }
            catch (Exception e)
            {
                LocationReference  = default;
                ErrorResponse      = "The given text representation of a location reference is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomLocationReferenceSerializer = null, CustomEVSESerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomLocationReferenceSerializer">A delegate to serialize custom locationReference JSON objects.</param>
        /// <param name="CustomPublishTokenTypeSerializer">A delegate to serialize custom publish token type JSON objects.</param>
        /// <param name="CustomAdditionalGeoLocationReferenceSerializer">A delegate to serialize custom additional geo locationReference JSON objects.</param>
        /// <param name="CustomEVSESerializer">A delegate to serialize custom EVSE JSON objects.</param>
        /// <param name="CustomStatusScheduleSerializer">A delegate to serialize custom status schedule JSON objects.</param>
        /// <param name="CustomConnectorSerializer">A delegate to serialize custom connector JSON objects.</param>
        /// <param name="CustomDisplayTextSerializer">A delegate to serialize custom multi-language text JSON objects.</param>
        /// <param name="CustomBusinessDetailsSerializer">A delegate to serialize custom business details JSON objects.</param>
        /// <param name="CustomHoursSerializer">A delegate to serialize custom hours JSON objects.</param>
        /// <param name="CustomImageSerializer">A delegate to serialize custom image JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<LocationReference>                CustomLocationReferenceSerializer                 = null,
                              CustomJObjectSerializerDelegate<PublishTokenType>       CustomPublishTokenTypeSerializer        = null,
                              CustomJObjectSerializerDelegate<AdditionalGeoLocation>  CustomAdditionalGeoLocationReferenceSerializer    = null,
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

                           //RelatedLocationReferences.SafeAny()
                           //    ? new JProperty("related_locationReferences",         new JArray(RelatedLocationReferences.Select(locationReference => locationReference.ToJSON(CustomAdditionalGeoLocationReferenceSerializer))))
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

            return CustomLocationReferenceSerializer != null
                       ? CustomLocationReferenceSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Operator overloading

        #region Operator == (LocationReference1, LocationReference2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LocationReference1">A location reference.</param>
        /// <param name="LocationReference2">Another location reference.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (LocationReference LocationReference1,
                                           LocationReference LocationReference2)

            => LocationReference1.Equals(LocationReference2);

        #endregion

        #region Operator != (LocationReference1, LocationReference2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LocationReference1">A location reference.</param>
        /// <param name="LocationReference2">Another location reference.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (LocationReference LocationReference1,
                                           LocationReference LocationReference2)

            => !(LocationReference1 == LocationReference2);

        #endregion

        #region Operator <  (LocationReference1, LocationReference2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LocationReference1">A location reference.</param>
        /// <param name="LocationReference2">Another location reference.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (LocationReference LocationReference1,
                                          LocationReference LocationReference2)

            => LocationReference1.CompareTo(LocationReference2) < 0;

        #endregion

        #region Operator <= (LocationReference1, LocationReference2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LocationReference1">A location reference.</param>
        /// <param name="LocationReference2">Another location reference.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (LocationReference LocationReference1,
                                           LocationReference LocationReference2)

            => !(LocationReference1 > LocationReference2);

        #endregion

        #region Operator >  (LocationReference1, LocationReference2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LocationReference1">A location reference.</param>
        /// <param name="LocationReference2">Another location reference.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (LocationReference LocationReference1,
                                          LocationReference LocationReference2)

            => LocationReference1.CompareTo(LocationReference2) > 0;

        #endregion

        #region Operator >= (LocationReference1, LocationReference2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LocationReference1">A location reference.</param>
        /// <param name="LocationReference2">Another location reference.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (LocationReference LocationReference1,
                                           LocationReference LocationReference2)

            => !(LocationReference1 < LocationReference2);

        #endregion

        #endregion

        #region IComparable<LocationReference> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is LocationReference locationReference
                   ? CompareTo(locationReference)
                   : throw new ArgumentException("The given object is not a location reference!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(LocationReference)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LocationReference">An object to compare with.</param>
        public Int32 CompareTo(LocationReference LocationReference)

            => LocationId.CompareTo(LocationReference.LocationId);

        #endregion

        #endregion

        #region IEquatable<LocationReference> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is LocationReference locationReference &&
                   Equals(locationReference);

        #endregion

        #region Equals(LocationReference)

        /// <summary>
        /// Compares two location references for equality.
        /// </summary>
        /// <param name="LocationReference">A location reference to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(LocationReference LocationReference)

            => LocationId.Equals(LocationReference.LocationId);

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
                return LocationId.GetHashCode();
            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(LocationId,
                             " -> ",
                             EVSEUIds.OrderBy(evse_uid => evse_uid).
                                      AggregateWith(", "));

        #endregion

    }

}
