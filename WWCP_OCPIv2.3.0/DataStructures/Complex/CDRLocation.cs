/*
 * Copyright (c) 2015-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_3_0
{

    /// <summary>
    /// The charge detail record location is a copy of the location object, but contains
    /// only the relevant information needed for creating a charge detail record.
    /// </summary>
    public class CDRLocation : IHasId<Location_Id>,
                               IEquatable<CDRLocation>,
                               IComparable<CDRLocation>,
                               IComparable
    {

        #region Properties

        /// <summary>
        /// The unique identification of the location within the charge point operator's platform (and suboperator platforms).
        /// </summary>
        [Mandatory]
        public Location_Id                         Id                       { get; }

        /// <summary>
        /// The optional display name of the location.
        /// string(255)
        /// </summary>
        [Optional]
        public String?                             Name                     { get; }

        /// <summary>
        /// The address (street/block name and house number if available) of the location.
        /// string(45)
        /// </summary>
        [Mandatory]
        public String                              Address                  { get; }

        /// <summary>
        /// The city or town of the location.
        /// string(45)
        /// </summary>
        [Mandatory]
        public String                              City                     { get; }

        /// <summary>
        /// The optional postal code of the location.
        /// string(10)
        /// </summary>
        [Optional]
        public String?                             PostalCode               { get; }

        /// <summary>
        /// The optional state of the location.
        /// string(20)
        /// </summary>
        [Optional]
        public String?                             State                    { get; }

        /// <summary>
        /// The ISO 3166-1 alpha-3 code of the country of the location.
        /// </summary>
        [Mandatory]
        public Country                             Country                  { get; }

        /// <summary>
        /// The geographical location of this location.
        [Mandatory]
        public GeoCoordinate                       Coordinates              { get; }

        /// <summary>
        /// The internal identification of the Electric Vehicle Supply Equipment (EVSE)
        /// within the charge point operator's platform.
        /// For interoperability please make sure, that the internal EVSE UId has the same value as the official EVSE Id!
        /// </summary>
        [Mandatory]
        public EVSE_UId                            EVSEUId                  { get; }

        /// <summary>
        /// The official unique identification of the Electric Vehicle Supply Equipment (EVSE).
        /// For interoperability please make sure, that the internal EVSE UId has the same value as the official EVSE Id!
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
        public ConnectorType                       ConnectorStandard        { get; }

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
        /// <param name="Id">The unique identification of the location within the charge point operator's platform (and suboperator platforms).</param>
        /// <param name="Address">The optional display name of the location.</param>
        /// <param name="City">The city or town of the location.</param>
        /// <param name="Country">The ISO 3166-1 alpha-3 code of the country of the location.</param>
        /// <param name="Coordinates">The geographical location of this location.</param>
        /// <param name="EVSEUId">The internal identification of the Electric Vehicle Supply Equipment (EVSE) within the charge point operator's platform. For interoperability please make sure, that the internal EVSE UId has the same value as the official EVSE Id!</param>
        /// <param name="EVSEId">The official unique identification of the Electric Vehicle Supply Equipment (EVSE). For interoperability please make sure, that the internal EVSE UId has the same value as the official EVSE Id!</param>
        /// <param name="ConnectorId">The official identification of the connector within the EVSE.</param>
        /// <param name="ConnectorStandard">The standard of the installed connector.</param>
        /// <param name="ConnectorFormat">The format (socket/cable) of the installed connector.</param>
        /// <param name="ConnectorPowerType">The power type of the installed connector.</param>
        /// 
        /// <param name="Name">The optional display name of the location.</param>
        /// <param name="PostalCode">The optional postal code of the location.</param>
        /// <param name="State">The optional state of the location.</param>
        public CDRLocation(Location_Id       Id,
                           String            Address,
                           String            City,
                           Country           Country,
                           GeoCoordinate     Coordinates,
                           EVSE_UId          EVSEUId,
                           EVSE_Id           EVSEId,
                           Connector_Id      ConnectorId,
                           ConnectorType     ConnectorStandard,
                           ConnectorFormats  ConnectorFormat,
                           PowerTypes        ConnectorPowerType,

                           String?           Name         = null,
                           String?           PostalCode   = null,
                           String?           State        = null)

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
            this.State                = State;

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

            throw new ArgumentException("The given JSON representation of a charge detail record location is invalid: " + errorResponse,
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

                #region Parse State                     [optional]

                var State = JSON.GetString("state");

                #endregion

                #region Parse Country                   [mandatory]

                if (!JSON.ParseMandatory("country",
                                         "country",
                                         org.GraphDefined.Vanaheimr.Illias.Country.TryParse,
                                         out Country Country,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Coordinates               [mandatory]

                if (!JSON.ParseMandatoryJSON("coordinates",
                                             "geo coordinates",
                                             GeoCoordinate.TryParse,
                                             out GeoCoordinate? Coordinates,
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

                if (!JSON.ParseMandatory("connector_standard",
                                         "connector standard/type",
                                         ConnectorType.TryParse,
                                         out ConnectorType ConnectorStandard,
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


                CDRLocation = new CDRLocation(
                                  Id,
                                  Address,
                                  City,
                                  Country,
                                  Coordinates.Value,
                                  EVSEUId,
                                  EVSEId,
                                  ConnectorId,
                                  ConnectorStandard,
                                  ConnectorFormat,
                                  ConnectorPowerType,
                                  Name,
                                  PostalCode,
                                  State
                              );


                if (CustomCDRLocationParser is not null)
                    CDRLocation = CustomCDRLocationParser(JSON,
                                                          CDRLocation);

                return true;

            }
            catch (Exception e)
            {
                CDRLocation    = default;
                ErrorResponse  = "The given JSON representation of a charge detail record location is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomCDRLocationSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomCDRLocationSerializer">A delegate to serialize custom location JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<CDRLocation>? CustomCDRLocationSerializer = null)
        {

            var json = JSONObject.Create(

                           new JProperty("id",                    Id.                ToString()),

                           Name.IsNotNullOrEmpty()
                               ? new JProperty("name",            Name)
                               : null,

                           new JProperty("address",               Address),
                           new JProperty("city",                  City),

                           PostalCode.IsNotNullOrEmpty()
                               ? new JProperty("postal_code",     PostalCode)
                               : null,

                           State.IsNotNullOrEmpty()
                               ? new JProperty("state",           State)
                               : null,

                           new JProperty("country",               Country.Alpha3Code),

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
                       ? CustomCDRLocationSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this object.
        /// </summary>
        public CDRLocation Clone()

            => new (

                   Id.               Clone(),
                   Address.          CloneString(),
                   City.             CloneString(),
                   Country.          Clone(),
                   Coordinates.      Clone(),
                   EVSEUId.          Clone(),
                   EVSEId.           Clone(),
                   ConnectorId.      Clone(),
                   ConnectorStandard.Clone(),
                   ConnectorFormat,
                   ConnectorPowerType,

                   Name.             CloneNullableString(),
                   PostalCode.       CloneNullableString(),
                   State.            CloneNullableString()

               );

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
        /// Compares two charge detail record locations.
        /// </summary>
        /// <param name="Object">A charge detail record location to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is CDRLocation location
                   ? CompareTo(location)
                   : throw new ArgumentException("The given object is not a charge detail record location!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(CDRLocation)

        /// <summary>
        /// Compares two charge detail record locations.
        /// </summary>
        /// <param name="CDRLocation">A charge detail record location to compare with.</param>
        public Int32 CompareTo(CDRLocation? CDRLocation)
        {

            if (CDRLocation is null)
                throw new ArgumentNullException(nameof(CDRLocation), "The given charge detail record location must not be null!");

            var c = Id.                CompareTo(CDRLocation.Id);

            if (c == 0)
                c = EVSEUId.           CompareTo(CDRLocation.EVSEUId);

            if (c == 0)
                c = EVSEId.            CompareTo(CDRLocation.EVSEId);

            if (c == 0)
                c = ConnectorId.       CompareTo(CDRLocation.ConnectorId);

            if (c == 0)
                c = ConnectorStandard. CompareTo(CDRLocation.ConnectorStandard);

            if (c == 0)
                c = ConnectorFormat.   CompareTo(CDRLocation.ConnectorFormat);

            if (c == 0)
                c = ConnectorPowerType.CompareTo(CDRLocation.ConnectorPowerType);

            // Address
            // City
            // Country
            // Coordinates

            // Name
            // PostalCode
            // State

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<CDRLocation> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two charge detail record locations for equality.
        /// </summary>
        /// <param name="Object">A charge detail record location to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is CDRLocation location &&
                   Equals(location);

        #endregion

        #region Equals(CDRLocation)

        /// <summary>
        /// Compares two charge detail record locations for equality.
        /// </summary>
        /// <param name="CDRLocation">A charge detail record location to compare with.</param>
        public Boolean Equals(CDRLocation? CDRLocation)

            => CDRLocation is not null &&

               Id.                Equals(CDRLocation.Id)                 &&
               Address.           Equals(CDRLocation.Address)            &&
               City.              Equals(CDRLocation.City)               &&
               Country.           Equals(CDRLocation.Country)            &&
               Coordinates.       Equals(CDRLocation.Coordinates)        &&
               EVSEUId.           Equals(CDRLocation.EVSEUId)            &&
               EVSEId.            Equals(CDRLocation.EVSEId)             &&
               ConnectorId.       Equals(CDRLocation.ConnectorId)        &&
               ConnectorStandard. Equals(CDRLocation.ConnectorStandard)  &&
               ConnectorFormat.   Equals(CDRLocation.ConnectorFormat)    &&
               ConnectorPowerType.Equals(CDRLocation.ConnectorPowerType) &&

             ((Name       is     null &&  CDRLocation.Name       is     null) ||
              (Name       is not null &&  CDRLocation.Name       is not null && Name.      Equals(CDRLocation.Name)))       &&

             ((PostalCode is     null &&  CDRLocation.PostalCode is     null) ||
              (PostalCode is not null &&  CDRLocation.PostalCode is not null && PostalCode.Equals(CDRLocation.PostalCode))) &&

             ((State      is     null &&  CDRLocation.State      is     null) ||
              (State      is not null &&  CDRLocation.State      is not null && State.     Equals(CDRLocation.State)));

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Get the hashcode of this object.
        /// </summary>
        public override Int32 GetHashCode()

            => Id.                GetHashCode()       * 43 ^
               Address.           GetHashCode()       * 41 ^
               City.              GetHashCode()       * 37 ^
               Country.           GetHashCode()       * 31 ^
               Coordinates.       GetHashCode()       * 29 ^
               EVSEUId.           GetHashCode()       * 23 ^
               EVSEId.            GetHashCode()       * 19 ^
               ConnectorId.       GetHashCode()       * 17 ^
               ConnectorStandard. GetHashCode()       * 13 ^
               ConnectorFormat.   GetHashCode()       * 11 ^
               ConnectorPowerType.GetHashCode()       *  7 ^
              (Name?.             GetHashCode() ?? 0) *  5 ^
              (PostalCode?.       GetHashCode() ?? 0) *  3 ^
               State?.            GetHashCode() ?? 0;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   Id,                ", ",

                   Name is not null
                       ? Name +       ", "
                       : "",

                   EVSEUId,           ", ",
                   EVSEId,            ", ",
                   ConnectorId,       ", ",
                   ConnectorStandard, ", ",
                   ConnectorFormat,   ", ",
                   ConnectorPowerType

               );

        // Address,
        // City,
        // Country,
        // Coordinates,
        // PostalCode  
        // State       

        #endregion

    }

}
