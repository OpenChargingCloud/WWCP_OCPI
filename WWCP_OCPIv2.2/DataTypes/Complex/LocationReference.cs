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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

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
        /// Optional enumeration of EVSE identifiers within the CPO’s platform.
        /// </summary>
        [Optional]
        public IEnumerable<EVSE_UId>  EVSEUIds      { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new references to location details.
        /// </summary>
        /// <param name="LocationId">Unique identifier for the location..</param>
        /// <param name="EVSEUIds">Optional enumeration of EVSE identifiers within the CPO’s platform.</param>
        public LocationReference(Location_Id            LocationId,
                                 IEnumerable<EVSE_UId>  EVSEUIds   = null)
        {

            this.LocationId  = LocationId;
            this.EVSEUIds    = EVSEUIds?.Distinct().OrderBy(evseUId => evseUId).ToArray() ?? new EVSE_UId[0];

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
                         out String             ErrorResponse,
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
        public static Boolean TryParse(JObject                JSON,
                                       out LocationReference  LocationReference,
                                       out String             ErrorResponse)

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
        public static Boolean TryParse(JObject                                         JSON,
                                       out LocationReference                           LocationReference,
                                       out String                                      ErrorResponse,
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

                #region Parse LocationId        [mandatory]

                if (!JSON.ParseMandatory("location_id",
                                         "location identification",
                                         Location_Id.TryParse,
                                         out Location_Id LocationId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse EVSEUIds          [optional]

                if (JSON.ParseOptionalJSON("evse_uids",
                                           "EVSE identifications",
                                           EVSE_UId.TryParse,
                                           out IEnumerable<EVSE_UId> EVSEUIds,
                                           out ErrorResponse))
                {
                    if (ErrorResponse != null)
                        return false;
                }

                #endregion


                LocationReference = new LocationReference(LocationId,
                                                          EVSEUIds);


                if (CustomLocationReferenceParser != null)
                    LocationReference = CustomLocationReferenceParser(JSON,
                                                                      LocationReference);

                return true;

            }
            catch (Exception e)
            {
                LocationReference  = default;
                ErrorResponse      = "The given JSON representation of a location reference is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Text, out LocationReference, out ErrorResponse, CustomLocationReferenceParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given text representation of a location reference.
        /// </summary>
        /// <param name="Text">The text to parse.</param>
        /// <param name="LocationReference">The parsed location reference.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(String                 Text,
                                       out LocationReference  LocationReference,
                                       out String             ErrorResponse)

            => TryParse(Text,
                        out LocationReference,
                        out ErrorResponse,
                        null);


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

        #region ToJSON(CustomLocationReferenceSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomLocationReferenceSerializer">A delegate to serialize custom location reference JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<LocationReference> CustomLocationReferenceSerializer = null)
        {

            var JSON = JSONObject.Create(

                           new JProperty("location_id",      LocationId.ToString()),

                           EVSEUIds.SafeAny()
                               ? new JProperty("evse_uids",  EVSEUIds.Select(evseUId => evseUId.ToString()))
                               : null

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
        {

            var c = LocationId.CompareTo(LocationReference.LocationId);

            if (c == 0 && EVSEUIds.SafeAny() && LocationReference.EVSEUIds.SafeAny())
                c = EVSEUIds.AggregateWith("-").CompareTo(LocationReference.EVSEUIds.AggregateWith("|"));

            return c;

        }

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

            => LocationId.Equals(LocationReference.LocationId) &&

               ((!EVSEUIds.SafeAny() && !LocationReference.EVSEUIds.SafeAny()) ||
                ( EVSEUIds.SafeAny() &&  LocationReference.EVSEUIds.SafeAny() &&
                  EVSEUIds.Count() == LocationReference.EVSEUIds.Count() &&
                  EVSEUIds.All(evseUId => LocationReference.EVSEUIds.Contains(evseUId))));

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

                return LocationId.GetHashCode() * 3 ^
                       EVSEUIds.  GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(LocationId,
                             EVSEUIds.SafeAny()
                                 ? " -> " + EVSEUIds.OrderBy(evse_uid => evse_uid).
                                                     AggregateWith(", ")
                                 : "");

        #endregion

    }

}
