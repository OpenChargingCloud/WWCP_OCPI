/*
 * Copyright (c) 2015-2022 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Aegir;
using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2
{

    /// <summary>
    /// The CdrLocation class contains only the relevant information from the
    /// location object that is needed in a CDR.
    /// </summary>
    public class CDRLocation : IHasId<Location_Id>,
                               IEquatable<CDRLocation>,
                               IComparable<CDRLocation>,
                               IComparable
    {

        #region Properties

        /// <summary>
        /// The identification of the location within the CPOs platform (and suboperator platforms). 
        /// </summary>
        [Mandatory]
        public Location_Id                         Id                       { get; }

        /// <summary>
        /// Display name of the location. // 255
        /// </summary>
        [Optional]
        public String                              Name                     { get; }

        /// <summary>
        /// Address of the location. // 45
        /// </summary>
        [Mandatory]
        public String                              Address                  { get; }

        /// <summary>
        /// Address of the location. // 45
        /// </summary>
        [Mandatory]
        public String                              City                     { get; }

        /// <summary>
        /// Address of the location. // 10
        /// </summary>
        [Optional]
        public String                              PostalCode               { get; }

        /// <summary>
        /// Address of the location. // 3
        /// </summary>
        [Mandatory]
        public String                              Country                  { get; }

        /// <summary>
        /// The geographical location of this location.
        [Mandatory]
        public GeoCoordinate                       Coordinates              { get; }

        /// <summary>
        /// The internal identification of the Electric Vehicle Supply Equipment (EVSE).
        /// </summary>
        [Mandatory]
        public EVSE_UId                            EVSEUId                  { get; }

        /// <summary>
        /// The official identification of the Electric Vehicle Supply Equipment (EVSE).
        /// </summary>
        [Mandatory]
        public EVSE_Id                             EVSEId                   { get; }

        /// <summary>
        /// The official identification of the connector within the EVSE.
        /// </summary>
        [Mandatory]
        public Connector_Id                        ConnectorId              { get; }

        /// <summary>
        /// The standard of the installed connector.
        /// </summary>
        public ConnectorTypes                      ConnectorStandard        { get; }

        /// <summary>
        /// The format (socket/cable) of the installed connector.
        /// </summary>
        public ConnectorFormats                    ConnectorFormat          { get; }

        /// <summary>
        /// The power type of the installed connector.
        /// </summary>
        public PowerTypes                          ConnectorPowerType       { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new charge detail record location containing only the relevant
        /// information from the official location.
        /// </summary>
        public CDRLocation(Location_Id       Id,
                           String            Address,
                           String            City,
                           String            Country,
                           GeoCoordinate     Coordinates,
                           EVSE_UId          EVSEUId,
                           EVSE_Id           EVSEId,
                           Connector_Id      ConnectorId,
                           ConnectorTypes    ConnectorStandard,
                           ConnectorFormats  ConnectorFormat,
                           PowerTypes        ConnectorPowerType,

                           String            Name         = null,
                           String            PostalCode   = null)

        {

            this.Id                   = Id;
            this.Address              = Address;
            this.City                 = City;
            this.Country              = Country;
            this.Coordinates          = Coordinates;
            this.EVSEUId              = EVSEUId;
            this.EVSEId               = EVSEId;
            this.ConnectorId          = ConnectorId;
            this.ConnectorStandard    = ConnectorStandard;
            this.ConnectorFormat      = ConnectorFormat;
            this.ConnectorPowerType   = ConnectorPowerType;

            this.Name                 = Name;
            this.PostalCode           = PostalCode;

        }

        #endregion


        #region (static) Parse   (JSON, CustomCDRLocationParser = null)

        /// <summary>
        /// Parse the given JSON representation of a charge detail record location.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomCDRLocationParser">A delegate to parse custom location JSON objects.</param>
        public static CDRLocation Parse(JObject                                    JSON,
                                        CustomJObjectParserDelegate<CDRLocation>?  CustomCDRLocationParser   = null)
        {

            if (TryParse(JSON,
                         out var location,
                         out var errorResponse,
                         CustomCDRLocationParser))
            {
                return location!;
            }

            throw new ArgumentException("The given JSON representation of a cdr location is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out CDRLocation, out ErrorResponse, CDRLocationIdURL = null, CustomCDRLocationParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a charge detail record location.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CDRLocation">The parsed location.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject           JSON,
                                       out CDRLocation?  CDRLocation,
                                       out String?       ErrorResponse)

            => TryParse(JSON,
                        out CDRLocation,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a charge detail record location.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CDRLocation">The parsed location.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomCDRLocationParser">A delegate to parse custom location JSON objects.</param>
        public static Boolean TryParse(JObject                                    JSON,
                                       out CDRLocation?                           CDRLocation,
                                       out String?                                ErrorResponse,
                                       CustomJObjectParserDelegate<CDRLocation>?  CustomCDRLocationParser   = null)
        {

            try
            {

                CDRLocation = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse Id                        [mandatory]

                if (!JSON.ParseMandatory("id",
                                         "location identification",
                                         Location_Id.TryParse,
                                         out Location_Id Id,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Name                      [optional]

                var Name = JSON.GetString("name");

                #endregion

                #region Parse Address                   [mandatory]

                if (!JSON.ParseMandatoryText("address",
                                             "address",
                                             out String Address,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse City                      [mandatory]

                if (!JSON.ParseMandatoryText("city",
                                             "city",
                                             out String City,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse PostalCode                [optional]

                var PostalCode = JSON.GetString("postal_code");

                #endregion

                #region Parse Country                   [mandatory]

                if (!JSON.ParseMandatoryText("country",
                                             "country",
                                             out String Country,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Coordinates               [mandatory]

                if (!JSON.ParseMandatoryJSON("coordinates",
                                             "geo coordinates",
                                             GeoCoordinate.TryParse,
                                             out GeoCoordinate Coordinates,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse EVSEUId                   [mandatory]

                if (!JSON.ParseMandatory("evse_uid",
                                         "EVSE unique identification",
                                         EVSE_UId.TryParse,
                                         out EVSE_UId EVSEUId,
                                         out ErrorResponse))
                {
                     return false;
                }

                #endregion

                #region Parse EVSEId                    [mandatory]

                if (!JSON.ParseMandatory("evse_id",
                                         "EVSE identification",
                                         EVSE_Id.TryParse,
                                         out EVSE_Id EVSEId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse ConnectorId               [mandatory]

                if (!JSON.ParseMandatory("connector_id",
                                         "connector identification",
                                         Connector_Id.TryParse,
                                         out Connector_Id ConnectorId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse ConnectorStandard         [mandatory]

                if (!JSON.ParseMandatoryEnum("connector_standard",
                                             "connector standard/type",
                                             out ConnectorTypes ConnectorStandard,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse ConnectorFormat           [mandatory]

                if (!JSON.ParseMandatory("connector_format",
                                         "connector format",
                                         ConnectorFormatsExtensions.TryParse,
                                         out ConnectorFormats ConnectorFormat,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse ConnectorPowerType        [mandatory]

                if (!JSON.ParseMandatory("connector_power_type",
                                         "connector power type",
                                         PowerTypesExtensions.TryParse,
                                         out PowerTypes ConnectorPowerType,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                CDRLocation = new CDRLocation(Id,
                                              Address,
                                              City,
                                              Country,
                                              Coordinates,
                                              EVSEUId,
                                              EVSEId,
                                              ConnectorId,
                                              ConnectorStandard,
                                              ConnectorFormat,
                                              ConnectorPowerType,
                                              Name,
                                              PostalCode);


                if (CustomCDRLocationParser is not null)
                    CDRLocation = CustomCDRLocationParser(JSON,
                                                          CDRLocation);

                return true;

            }
            catch (Exception e)
            {
                CDRLocation    = default;
                ErrorResponse  = "The given JSON representation of a cdr location is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomCDRLocationSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomCDRLocationSerializer">A delegate to serialize custom location JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<CDRLocation> CustomCDRLocationSerializer = null)
        {

            var JSON = JSONObject.Create(

                           new JProperty("id",                    Id.                ToString()),

                           Name.IsNotNullOrEmpty()
                               ? new JProperty("name",            Name)
                               : null,

                           new JProperty("address",               Address),
                           new JProperty("city",                  City),

                           PostalCode.IsNotNullOrEmpty()
                               ? new JProperty("postal_code",     PostalCode)
                               : null,

                           new JProperty("country",               Country),

                           new JProperty("coordinates",           new JObject(
                                                                      new JProperty("latitude",  Coordinates.Latitude. Value.ToString("0.00000##").Replace(",", ".")),
                                                                      new JProperty("longitude", Coordinates.Longitude.Value.ToString("0.00000##").Replace(",", "."))
                                                                  )),

                           new JProperty("evse_uid",              EVSEUId.           ToString()),
                           new JProperty("evse_id",               EVSEId.            ToString()),
                           new JProperty("connector_id",          ConnectorId.       ToString()),
                           new JProperty("connector_standard",    ConnectorStandard. ToString()),
                           new JProperty("connector_format",      ConnectorFormat.   ToString()),
                           new JProperty("connector_power_type",  ConnectorPowerType.ToString())

                       );

            return CustomCDRLocationSerializer is not null
                       ? CustomCDRLocationSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Operator overloading

        #region Operator == (CDRLocation1, CDRLocation2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDRLocation1">A charge detail record location.</param>
        /// <param name="CDRLocation2">Another charge detail record location.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (CDRLocation CDRLocation1,
                                           CDRLocation CDRLocation2)
        {

            if (Object.ReferenceEquals(CDRLocation1, CDRLocation2))
                return true;

            if (CDRLocation1 is null || CDRLocation2 is null)
                return false;

            return CDRLocation1.Equals(CDRLocation2);

        }

        #endregion

        #region Operator != (CDRLocation1, CDRLocation2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDRLocation1">A charge detail record location.</param>
        /// <param name="CDRLocation2">Another charge detail record location.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (CDRLocation CDRLocation1,
                                           CDRLocation CDRLocation2)

            => !(CDRLocation1 == CDRLocation2);

        #endregion

        #region Operator <  (CDRLocation1, CDRLocation2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDRLocation1">A charge detail record location.</param>
        /// <param name="CDRLocation2">Another charge detail record location.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (CDRLocation CDRLocation1,
                                          CDRLocation CDRLocation2)

            => CDRLocation1 is null
                   ? throw new ArgumentNullException(nameof(CDRLocation1), "The given charge detail record location must not be null!")
                   : CDRLocation1.CompareTo(CDRLocation2) < 0;

        #endregion

        #region Operator <= (CDRLocation1, CDRLocation2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDRLocation1">A charge detail record location.</param>
        /// <param name="CDRLocation2">Another charge detail record location.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (CDRLocation CDRLocation1,
                                           CDRLocation CDRLocation2)

            => !(CDRLocation1 > CDRLocation2);

        #endregion

        #region Operator >  (CDRLocation1, CDRLocation2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDRLocation1">A charge detail record location.</param>
        /// <param name="CDRLocation2">Another charge detail record location.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (CDRLocation CDRLocation1,
                                          CDRLocation CDRLocation2)

            => CDRLocation1 is null
                   ? throw new ArgumentNullException(nameof(CDRLocation1), "The given charge detail record location must not be null!")
                   : CDRLocation1.CompareTo(CDRLocation2) > 0;

        #endregion

        #region Operator >= (CDRLocation1, CDRLocation2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDRLocation1">A charge detail record location.</param>
        /// <param name="CDRLocation2">Another charge detail record location.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (CDRLocation CDRLocation1,
                                           CDRLocation CDRLocation2)

            => !(CDRLocation1 < CDRLocation2);

        #endregion

        #endregion

        #region IComparable<CDRLocation> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is CDRLocation location
                   ? CompareTo(location)
                   : throw new ArgumentException("The given object is not a charging location!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(CDRLocation)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDRLocation">An CDRLocation to compare with.</param>
        public Int32 CompareTo(CDRLocation CDRLocation)

            => CDRLocation is null
                   ? throw new ArgumentNullException(nameof(CDRLocation), "The given charging location must not be null!")
                   : Id.CompareTo(CDRLocation.Id);

        #endregion

        #endregion

        #region IEquatable<CDRLocation> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is CDRLocation location &&
                   Equals(location);

        #endregion

        #region Equals(CDRLocation)

        /// <summary>
        /// Compares two locations for equality.
        /// </summary>
        /// <param name="CDRLocation">A location to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(CDRLocation CDRLocation)

            => !(CDRLocation is null) &&
                   Id.Equals(CDRLocation.Id);

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
