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

using org.GraphDefined.Vanaheimr.Aegir;
using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.JSON;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2
{

    /// <summary>
    /// The EVSE object describes the part that controls the power supply to a single EV in a single session.
    /// </summary>
    public class EVSE : IHasId<EVSE_Id>,
                        IEquatable<EVSE>,
                        IComparable<EVSE>,
                        IComparable,
                        IEnumerable<Connector>
    {

        #region Properties

        /// <summary>
        /// Uniquely identifies the EVSE within the CPOs platform.
        /// </summary>
        [Mandatory]
        public EVSE_UId                          UId                        { get; }

        /// <summary>
        /// Compliant with the following specification for EVSE ID from "eMI3 standard version V1.0".
        /// </summary>
        /// <remarks>Mandatory within this implementation.</remarks>
        [Mandatory]
        public EVSE_Id                           Id                         { get; }

        /// <summary>
        /// Indicates the current status of the EVSE.
        /// </summary>
        [Mandatory]
        public StatusTypes                       Status                     { get; }

        /// <summary>
        /// Indicates a planned status in the future of the EVSE.
        /// </summary>
        [Optional]
        public IEnumerable<StatusSchedule>       StatusSchedule             { get; }

        /// <summary>
        /// Enumeration of functionalities that the EVSE is capable of.
        /// </summary>
        [Optional]
        public IEnumerable<CapabilityTypes>      Capabilities               { get; }

        /// <summary>
        /// Enumeration of available connectors at this EVSE.
        /// </summary>
        [Mandatory]
        public IEnumerable<Connector>            Connectors                 { get; }

        /// <summary>
        /// The unique identifications of all connectors at this EVSE.
        /// </summary>
        [Optional]
        public IEnumerable<Connector_Id>         ConnectorIds
            => Connectors.SafeSelect(connector => connector.Id);

        /// <summary>
        /// Level on which the EVSE is located (in garage buildings) in the locally displayed numbering scheme.  // 4
        /// </summary>
        [Optional]
        public String                            FloorLevel                 { get; }

        /// <summary>
        /// An optional geographical location of this EVSE.
        /// </summary>
        [Optional]
        public GeoCoordinate?                    Coordinates                { get; }

        /// <summary>
        /// An optional number/string printed on the outside of the EVSE for visual identification. // 16
        /// </summary>
        [Optional]
        public String                            PhysicalReference          { get; }

        /// <summary>
        /// Optional multi-language human-readable directions when more detailed
        /// information on how to reach the EVSE from the location is required.
        /// </summary>
        [Optional]
        public I18NString                        Directions                 { get; }

        /// <summary>
        /// Optional restrictions that apply to the parking spot.
        /// </summary>
        [Optional]
        public IEnumerable<ParkingRestrictions>  ParkingRestrictions        { get; }

        /// <summary>
        /// Optional links to images related to the EVSE such as photos or logos.
        /// </summary>
        [Optional]
        public IEnumerable<Image>                Images                     { get; }

        /// <summary>
        /// Timestamp when this EVSE was last updated (or created).
        /// </summary>
        [Mandatory]
        public DateTime                          LastUpdated                { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// The EVSE object describes the part that controls the power supply to a single EV in a single session.
        /// </summary>
        /// <param name="UId">Uniquely identifies the EVSE within the CPOs platform.</param>
        /// <param name="Id">Compliant with the following specification for EVSE ID from "eMI3 standard version V1.0".</param>
        /// <param name="Status">Indicates the current status of the EVSE.</param>
        /// <param name="Connectors">Enumeration of available connectors at this EVSE.</param>
        /// <param name="StatusSchedule">Indicates a planned status in the future of the EVSE.</param>
        /// <param name="Capabilities">Enumeration of functionalities that the EVSE is capable of.</param>
        /// <param name="FloorLevel">Level on which the EVSE is located (in garage buildings) in the locally displayed numbering scheme.</param>
        /// <param name="Coordinates">An optional geographical location of this EVSE.</param>
        /// <param name="PhysicalReference">An optional number/string printed on the outside of the EVSE for visual identification.</param>
        /// <param name="Directions">Optional multi-language human-readable directions when more detailed information on how to reach the EVSE from the location is required.</param>
        /// <param name="ParkingRestrictions">Optional restrictions that apply to the parking spot.</param>
        /// <param name="Images">Optional links to images related to the EVSE such as photos or logos.</param>
        /// <param name="LastUpdated">Timestamp when this EVSE was last updated (or created).</param>
        public EVSE(EVSE_UId                          UId,
                    EVSE_Id                           Id,
                    StatusTypes                       Status,
                    IEnumerable<Connector>            Connectors,

                    IEnumerable<StatusSchedule>       StatusSchedule        = null,
                    IEnumerable<CapabilityTypes>      Capabilities          = null,
                    String                            FloorLevel            = null,
                    GeoCoordinate?                    Coordinates           = null,
                    String                            PhysicalReference     = null,
                    I18NString                        Directions            = null,
                    IEnumerable<ParkingRestrictions>  ParkingRestrictions   = null,
                    IEnumerable<Image>                Images                = null,

                    DateTime?                         LastUpdated           = null)

        {

            this.UId                  = UId;
            this.Id                   = Id;
            this.Status               = Status;
            this.Connectors           = Connectors;

            this.StatusSchedule       = StatusSchedule;
            this.Capabilities         = Capabilities;
            this.FloorLevel           = FloorLevel;
            this.Coordinates          = Coordinates;
            this.PhysicalReference    = PhysicalReference;
            this.Directions           = Directions;
            this.ParkingRestrictions   = ParkingRestrictions;
            this.Images               = Images;

            this.LastUpdated          = LastUpdated ?? DateTime.Now;

        }

        #endregion



        #region (static) Parse   (JSON, CustomEVSEParser = null)

        /// <summary>
        /// Parse the given JSON representation of an EVSE.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomEVSEParser">A delegate to parse custom EVSE JSON objects.</param>
        public static EVSE Parse(JObject                            JSON,
                                 CustomJObjectParserDelegate<EVSE>  CustomEVSEParser   = null)
        {

            if (TryParse(JSON,
                         out EVSE   evse,
                         out String ErrorResponse,
                         CustomEVSEParser))
            {
                return evse;
            }

            throw new ArgumentException("The given JSON representation of an EVSE is invalid: " + ErrorResponse, nameof(JSON));

        }

        #endregion

        #region (static) Parse   (Text, CustomEVSEParser = null)

        /// <summary>
        /// Parse the given text representation of an EVSE.
        /// </summary>
        /// <param name="Text">The text to parse.</param>
        /// <param name="CustomEVSEParser">A delegate to parse custom EVSE JSON objects.</param>
        public static EVSE Parse(String                             Text,
                                 CustomJObjectParserDelegate<EVSE>  CustomEVSEParser   = null)
        {

            if (TryParse(Text,
                         out EVSE   evse,
                         out String ErrorResponse,
                         CustomEVSEParser))
            {
                return evse;
            }

            throw new ArgumentException("The given text representation of an EVSE is invalid: " + ErrorResponse, nameof(Text));

        }

        #endregion

        #region (static) TryParse(JSON, out EVSE, out ErrorResponse, CustomEVSEParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of an EVSE.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="EVSE">The parsed connector.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomEVSEParser">A delegate to parse custom EVSE JSON objects.</param>
        public static Boolean TryParse(JObject                            JSON,
                                       out EVSE                           EVSE,
                                       out String                         ErrorResponse,
                                       CustomJObjectParserDelegate<EVSE>  CustomEVSEParser   = null)
        {

            try
            {

                EVSE = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse UId                [mandatory]

                if (!JSON.ParseMandatory("uid",
                                         "internal EVSE identification",
                                         EVSE_UId.TryParse,
                                         out EVSE_UId UId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Id                 [mandatory]

                if (!JSON.ParseMandatory("id",
                                         "offical EVSE identification",
                                         EVSE_Id.TryParse,
                                         out EVSE_Id Id,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Status             [mandatory]

                if (!JSON.ParseMandatoryEnum("status",
                                             "EVSE status",
                                             out StatusTypes Status,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Connectors         [mandatory]

                if (!JSON.ParseMandatoryJSON("connectors",
                                             "connectors",
                                             Connector.TryParse,
                                             out IEnumerable<Connector> Connectors,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse StatusSchedule     [mandatory]

                if (!JSON.ParseMandatoryJSON("status_schedule",
                                             "status schedule",
                                             OCPIv2_2.StatusSchedule.TryParse,
                                             out IEnumerable<StatusSchedule> StatusSchedule,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse MaxVoltage          [mandatory]

                if (!JSON.ParseMandatory("max_voltage",
                                                  "max voltage",
                                                  out UInt16 MaxVoltage,
                                                  out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse MaxAmperage         [mandatory]

                if (!JSON.ParseMandatory("max_amperage",
                                                  "max amperage",
                                                  out UInt16 MaxAmperage,
                                                  out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse MaxElectricPower    [optional]

                if (JSON.ParseOptional("max_electric_power",
                                                "max voltage",
                                                out UInt16? MaxElectricPower,
                                                out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse MaxElectricPower    [optional]

                if (JSON.ParseOptionalHashSet("tariff_ids",
                                                       "tariff identifications",
                                                       Tariff_Id.TryParse,
                                                       out HashSet <Tariff_Id> TariffIds,
                                                       out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                var TermsAndConditions = JSON.GetString("terms_and_conditions");

                #region Parse LastUpdated         [mandatory]

                if (!JSON.ParseMandatory("last_updated",
                                                  "last updated",
                                                  out DateTime LastUpdated,
                                                  out ErrorResponse))
                {
                    return false;
                }

                #endregion


                EVSE = new EVSE(UId,
                                Id,
                                Status,
                                Connectors,

                                StatusSchedule?.Distinct(),
                                Capabilities,
                                FloorLevel,
                                Coordinates,
                                PhysicalReference,
                                Directions,
                                ParkingRestrictions,
                                Images,

                                LastUpdated);


                if (CustomEVSEParser != null)
                    EVSE = CustomEVSEParser(JSON,
                                                      EVSE);

                return true;

            }
            catch (Exception e)
            {
                EVSE          = default;
                ErrorResponse = "The given JSON representation of an EVSE is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Text, out EVSE, out ErrorResponse, CustomEVSEParser = null)

        /// <summary>
        /// Try to parse the given text representation of an EVSE.
        /// </summary>
        /// <param name="Text">The text to parse.</param>
        /// <param name="EVSE">The parsed connector.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomEVSEParser">A delegate to parse custom EVSE JSON objects.</param>
        public static Boolean TryParse(String                             Text,
                                       out EVSE                           EVSE,
                                       out String                         ErrorResponse,
                                       CustomJObjectParserDelegate<EVSE>  CustomEVSEParser   = null)
        {

            try
            {

                return TryParse(JObject.Parse(Text),
                                out EVSE,
                                out ErrorResponse,
                                CustomEVSEParser);

            }
            catch (Exception e)
            {
                EVSE      = null;
                ErrorResponse  = "The given text representation of an EVSE is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomEVSESerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomEVSESerializer">A delegate to serialize custom EVSE JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<EVSE> CustomEVSESerializer = null)
        {

            var JSON = JSONObject.Create(

                           new JProperty("uid",                         UId.   ToString()),
                           new JProperty("evse_id",                     Id.    ToString()),
                           new JProperty("status",                      Status.ToString()),

                           StatusSchedule.SafeAny()
                               ? new JProperty("status_schedule",       new JArray(StatusSchedule.Select(status     => status.    ToJSON())))
                               : null,

                           Capabilities.SafeAny()
                               ? new JProperty("capabilities",          new JArray(Capabilities.  Select(capability => capability.ToString())))
                               : null,

                           Connectors.SafeAny()
                               ? new JProperty("connectors",            new JArray(Connectors.    Select(connector  => connector. ToJSON())))
                               : null,

                           FloorLevel.IsNotNullOrEmpty()
                               ? new JProperty("floor_level",           FloorLevel)
                               : null,

                           Coordinates.HasValue
                               ? new JProperty("coordinates",           new JObject(
                                                                            new JProperty("latitude",  Coordinates.Value.Latitude. Value.ToString()),
                                                                            new JProperty("longitude", Coordinates.Value.Longitude.Value.ToString())
                                                                        ))
                               : null,

                           PhysicalReference.IsNotNullOrEmpty()
                               ? new JProperty("physical_reference",    PhysicalReference)
                               : null,

                           Directions.IsNeitherNullNorEmpty()
                               ? new JProperty("directions",            Directions.ToJSON())
                               : null,

                           ParkingRestrictions.SafeAny()
                               ? new JProperty("parking_restrictions",  new JArray(ParkingRestrictions.Select(parking => parking.ToString())))
                               : null,

                           Images.SafeAny()
                               ? new JProperty("images",                new JArray(Images.Select(image => image.ToJSON())))
                               : null,

                           new JProperty("last_updated",                LastUpdated.ToIso8601())

                       );

            return CustomEVSESerializer != null
                       ? CustomEVSESerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region IEnumerable<Connectors> Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            => Connectors.GetEnumerator();

        public IEnumerator<Connector> GetEnumerator()
            => Connectors.GetEnumerator();

        #endregion


        #region IComparable<EVSE> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is EVSE evse
                   ? CompareTo(evse)
                   : throw new ArgumentException("The given object is not an EVSE!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(EVSE)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVSE">An EVSE to compare with.</param>
        public Int32 CompareTo(EVSE EVSE)
        {

            if (EVSE is null)
                throw new ArgumentNullException(nameof(EVSE), "The given EVSE must not be null!");

            return Id.CompareTo(EVSE.Id);

        }

        #endregion

        #endregion

        #region IEquatable<EVSE> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is EVSE evse &&
                   Equals(evse);

        #endregion

        #region Equals(EVSE)

        /// <summary>
        /// Compares two EVSEs for equality.
        /// </summary>
        /// <param name="EVSE">An EVSE to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(EVSE EVSE)

            => (!(EVSE is null)) &&
                   Id.Equals(EVSE.Id);

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
