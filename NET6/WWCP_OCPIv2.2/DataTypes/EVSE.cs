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

using org.GraphDefined.Vanaheimr.Aegir;
using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2
{

    /// <summary>
    /// An Electric Vehicle Supply Equipment (EVSE) is the part that controls
    /// the power supply to a single electric vehicle.
    /// </summary>
    public class EVSE : //IHasId<EVSE_Id>,
                        IEquatable<EVSE>,
                        IComparable<EVSE>,
                        IComparable,
                        IEnumerable<Connector>
    {

        #region Data

        private readonly Object patchLock = new Object();

        #endregion

        #region Properties

        /// <summary>
        /// The parent location of this EVSE.
        /// </summary>
        public Location                          ParentLocation             { get; internal set; }

        /// <summary>
        /// Uniquely identifies the EVSE within the CPOs platform.
        /// </summary>
        [Mandatory]
        public EVSE_UId                          UId                        { get; }

        /// <summary>
        /// Compliant with the following specification for EVSE ID from "eMI3 standard version V1.0".
        /// </summary>
        [Optional]
        public EVSE_Id?                          EVSEId                     { get; }

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
        public IEnumerable<Connector>            Connectors                 { get; private set; }

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
        public IEnumerable<DisplayText>          Directions                 { get; }

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

        /// <summary>
        /// The SHA256 hash of the JSON representation of this EVSE.
        /// </summary>
        public String                            SHA256Hash                 { get; private set; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new EVSE.
        /// </summary>
        /// <param name="ParentLocation">The parent location of this EVSE.</param>
        /// 
        /// <param name="UId">Uniquely identifies the EVSE within the CPOs platform.</param>
        /// <param name="Status">Indicates the current status of the EVSE.</param>
        /// <param name="Connectors">Enumeration of available connectors at this EVSE.</param>
        /// 
        /// <param name="EVSEId">Compliant with the following specification for EVSE ID from "eMI3 standard version V1.0".</param>
        /// <param name="StatusSchedule">Indicates a planned status in the future of the EVSE.</param>
        /// <param name="Capabilities">Enumeration of functionalities that the EVSE is capable of.</param>
        /// <param name="FloorLevel">Level on which the EVSE is located (in garage buildings) in the locally displayed numbering scheme.</param>
        /// <param name="Coordinates">An optional geographical location of this EVSE.</param>
        /// <param name="PhysicalReference">An optional number/string printed on the outside of the EVSE for visual identification.</param>
        /// <param name="Directions">Optional multi-language human-readable directions when more detailed information on how to reach the EVSE from the location is required.</param>
        /// <param name="ParkingRestrictions">Optional restrictions that apply to the parking spot.</param>
        /// <param name="Images">Optional links to images related to the EVSE such as photos or logos.</param>
        /// 
        /// <param name="LastUpdated">Timestamp when this EVSE was last updated (or created).</param>
        internal EVSE(Location                          ParentLocation,

                      EVSE_UId                          UId,
                      StatusTypes                       Status,
                      IEnumerable<Connector>            Connectors,

                      EVSE_Id?                          EVSEId                = null,
                      IEnumerable<StatusSchedule>       StatusSchedule        = null,
                      IEnumerable<CapabilityTypes>      Capabilities          = null,
                      String                            FloorLevel            = null,
                      GeoCoordinate?                    Coordinates           = null,
                      String                            PhysicalReference     = null,
                      IEnumerable<DisplayText>          Directions            = null,
                      IEnumerable<ParkingRestrictions>  ParkingRestrictions   = null,
                      IEnumerable<Image>                Images                = null,

                      DateTime?                         LastUpdated           = null)

        {

            this.ParentLocation        = ParentLocation;

            this.UId                   = UId;
            this.Status                = Status;
            this.Connectors            = Connectors?.         Distinct() ?? new Connector[0];

            this.EVSEId                = EVSEId;
            this.StatusSchedule        = StatusSchedule?.     Distinct() ?? new StatusSchedule[0];
            this.Capabilities          = Capabilities?.       Distinct() ?? new CapabilityTypes[0];
            this.FloorLevel            = FloorLevel;
            this.Coordinates           = Coordinates;
            this.PhysicalReference     = PhysicalReference;
            this.Directions            = Directions?.         Distinct() ?? new DisplayText[0];
            this.ParkingRestrictions   = ParkingRestrictions?.Distinct() ?? new ParkingRestrictions[0];
            this.Images                = Images?.             Distinct() ?? new Image[0];

            this.LastUpdated           = LastUpdated ?? DateTime.Now;

            if (Connectors != null)
                foreach (var connector in Connectors)
                    connector.ParentEVSE = this;

            CalcSHA256Hash();

        }


        /// <summary>
        /// Create a new EVSE.
        /// </summary>
        /// <param name="UId">Uniquely identifies the EVSE within the CPOs platform.</param>
        /// <param name="Status">Indicates the current status of the EVSE.</param>
        /// <param name="Connectors">Enumeration of available connectors at this EVSE.</param>
        /// 
        /// <param name="EVSEId">Compliant with the following specification for EVSE ID from "eMI3 standard version V1.0".</param>
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
                    StatusTypes                       Status,
                    IEnumerable<Connector>            Connectors,

                    EVSE_Id?                          EVSEId                = null,
                    IEnumerable<StatusSchedule>       StatusSchedule        = null,
                    IEnumerable<CapabilityTypes>      Capabilities          = null,
                    String                            FloorLevel            = null,
                    GeoCoordinate?                    Coordinates           = null,
                    String                            PhysicalReference     = null,
                    IEnumerable<DisplayText>          Directions            = null,
                    IEnumerable<ParkingRestrictions>  ParkingRestrictions   = null,
                    IEnumerable<Image>                Images                = null,

                    DateTime?                         LastUpdated           = null)

            : this(null,

                   UId,
                   Status,
                   Connectors,

                   EVSEId,
                   StatusSchedule,
                   Capabilities,
                   FloorLevel,
                   Coordinates,
                   PhysicalReference,
                   Directions,
                   ParkingRestrictions,
                   Images,

                   LastUpdated)

            { }

        #endregion


        #region (static) Parse   (JSON, EVSEUIdURL = null, CustomEVSEParser = null)

        /// <summary>
        /// Parse the given JSON representation of an EVSE.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="EVSEUIdURL">An optional EVSE identification, e.g. from the HTTP URL.</param>
        /// <param name="CustomEVSEParser">A delegate to parse custom EVSE JSON objects.</param>
        public static EVSE Parse(JObject                            JSON,
                                 EVSE_UId?                          EVSEUIdURL         = null,
                                 CustomJObjectParserDelegate<EVSE>  CustomEVSEParser   = null)
        {

            if (TryParse(JSON,
                         out EVSE   evse,
                         out String ErrorResponse,
                         EVSEUIdURL,
                         CustomEVSEParser))
            {
                return evse;
            }

            throw new ArgumentException("The given JSON representation of an EVSE is invalid: " + ErrorResponse, nameof(JSON));

        }

        #endregion

        #region (static) Parse   (Text, EVSEUIdURL = null, CustomEVSEParser = null)

        /// <summary>
        /// Parse the given text representation of an EVSE.
        /// </summary>
        /// <param name="Text">The text to parse.</param>
        /// <param name="EVSEUIdURL">An optional EVSE identification, e.g. from the HTTP URL.</param>
        /// <param name="CustomEVSEParser">A delegate to parse custom EVSE JSON objects.</param>
        public static EVSE Parse(String                             Text,
                                 EVSE_UId?                          EVSEUIdURL         = null,
                                 CustomJObjectParserDelegate<EVSE>  CustomEVSEParser   = null)
        {

            if (TryParse(Text,
                         out EVSE   evse,
                         out String ErrorResponse,
                         EVSEUIdURL,
                         CustomEVSEParser))
            {
                return evse;
            }

            throw new ArgumentException("The given text representation of an EVSE is invalid: " + ErrorResponse, nameof(Text));

        }

        #endregion

        #region (static) TryParse(JSON, out EVSE, out ErrorResponse, EVSEUIdURL = null, CustomEVSEParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of an EVSE.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="EVSE">The parsed EVSE.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject     JSON,
                                       out EVSE    EVSE,
                                       out String  ErrorResponse)

            => TryParse(JSON,
                        out EVSE,
                        out ErrorResponse,
                        null,
                        null);

        /// <summary>
        /// Try to parse the given JSON representation of an EVSE.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="EVSE">The parsed EVSE.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="EVSEUIdURL">An optional EVSE identification, e.g. from the HTTP URL.</param>
        /// <param name="CustomEVSEParser">A delegate to parse custom EVSE JSON objects.</param>
        public static Boolean TryParse(JObject                            JSON,
                                       out EVSE                           EVSE,
                                       out String                         ErrorResponse,
                                       EVSE_UId?                          EVSEUIdURL,
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

                #region Parse UId                       [optional]

                if (JSON.ParseOptionalStruct("uid",
                                             "internal EVSE identification",
                                             EVSE_UId.TryParse,
                                             out EVSE_UId? EVSEUIdBody,
                                             out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                if (!EVSEUIdURL.HasValue && !EVSEUIdBody.HasValue)
                {
                    ErrorResponse = "The EVSE identification is missing!";
                    return false;
                }

                if (EVSEUIdURL.HasValue && EVSEUIdBody.HasValue && EVSEUIdURL.Value != EVSEUIdBody.Value)
                {
                    ErrorResponse = "The optional EVSE identification given within the JSON body does not match the one given in the URL!";
                    return false;
                }

                #endregion

                #region Parse Status                    [mandatory]

                if (!JSON.ParseMandatoryEnum("status",
                                             "EVSE status",
                                             out StatusTypes Status,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Connectors                [mandatory]

                if (!JSON.ParseMandatoryJSON<Connector, Connector_Id>("connectors",
                                                                      "connectors",
                                                                      Connector.TryParse,
                                                                      out IEnumerable<Connector> Connectors,
                                                                      out ErrorResponse))
                {
                    return false;
                }

                #endregion


                #region Parse EVSEId                    [optional]

                if (JSON.ParseOptional("evse_id",
                                       "offical EVSE identification",
                                       EVSE_Id.TryParse,
                                       out EVSE_Id? EVSEId,
                                       out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse StatusSchedule            [optional]

                if (JSON.ParseOptionalJSON("status_schedule",
                                           "status schedule",
                                           OCPIv2_2.StatusSchedule.TryParse,
                                           out IEnumerable<StatusSchedule> StatusSchedule,
                                           out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse Capabilities              [optional]

                if (JSON.ParseOptionalEnums("capabilities",
                                            "capabilities",
                                            out IEnumerable<CapabilityTypes> Capabilities,
                                            out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse FloorLevel                [optional]

                var FloorLevel = JSON.GetString("floor_level");

                #endregion

                #region Parse Coordinates               [optional]

                if (JSON.ParseOptionalJSON("coordinates",
                                           "geo coordinates",
                                           GeoCoordinate.TryParse,
                                           out GeoCoordinate? Coordinates,
                                           out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse PhysicalReference         [optional]

                var PhysicalReference = JSON.GetString("physical_reference");

                #endregion

                #region Parse Directions                [optional]

                if (JSON.ParseOptionalJSON("directions",
                                           "directions",
                                           DisplayText.TryParse,
                                           out IEnumerable<DisplayText> Directions,
                                           out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse ParkingRestrictions       [optional]

                if (JSON.ParseOptionalEnums("parking_restrictions",
                                            "parking restrictions",
                                            out IEnumerable<ParkingRestrictions> ParkingRestrictions,
                                            out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse Images                    [optional]

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


                #region Parse LastUpdated               [mandatory]

                if (!JSON.ParseMandatory("last_updated",
                                         "last updated",
                                         out DateTime LastUpdated,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                EVSE = new EVSE(EVSEUIdBody ?? EVSEUIdURL.Value,
                                Status,
                                Connectors?.         Distinct(),

                                EVSEId,
                                StatusSchedule?.     Distinct(),
                                Capabilities?.       Distinct(),
                                FloorLevel?.         Trim(),
                                Coordinates,
                                PhysicalReference?.  Trim(),
                                Directions?.         Distinct(),
                                ParkingRestrictions?.Distinct(),
                                Images?.             Distinct(),

                                LastUpdated);


                if (CustomEVSEParser != null)
                    EVSE = CustomEVSEParser(JSON,
                                            EVSE);

                return true;

            }
            catch (Exception e)
            {
                EVSE           = default;
                ErrorResponse  = "The given JSON representation of an EVSE is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Text, out EVSE, out ErrorResponse, EVSEUIdURL = null, CustomEVSEParser = null)

        /// <summary>
        /// Try to parse the given text representation of an EVSE.
        /// </summary>
        /// <param name="Text">The text to parse.</param>
        /// <param name="EVSE">The parsed EVSE.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="EVSEUIdURL">An optional EVSE identification, e.g. from the HTTP URL.</param>
        /// <param name="CustomEVSEParser">A delegate to parse custom EVSE JSON objects.</param>
        public static Boolean TryParse(String                             Text,
                                       out EVSE                           EVSE,
                                       out String                         ErrorResponse,
                                       EVSE_UId?                          EVSEUIdURL         = null,
                                       CustomJObjectParserDelegate<EVSE>  CustomEVSEParser   = null)
        {

            try
            {

                return TryParse(JObject.Parse(Text),
                                out EVSE,
                                out ErrorResponse,
                                EVSEUIdURL,
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

        #region ToJSON(CustomEVSESerializer = null, CustomStatusScheduleSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomEVSESerializer">A delegate to serialize custom EVSE JSON objects.</param>
        /// <param name="CustomStatusScheduleSerializer">A delegate to serialize custom status schedule JSON objects.</param>
        /// <param name="CustomConnectorSerializer">A delegate to serialize custom connector JSON objects.</param>
        /// <param name="CustomDisplayTextSerializer">A delegate to serialize custom multi-language text JSON objects.</param>
        /// <param name="CustomImageSerializer">A delegate to serialize custom image JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<EVSE>            CustomEVSESerializer             = null,
                              CustomJObjectSerializerDelegate<StatusSchedule>  CustomStatusScheduleSerializer   = null,
                              CustomJObjectSerializerDelegate<Connector>       CustomConnectorSerializer        = null,
                              CustomJObjectSerializerDelegate<DisplayText>     CustomDisplayTextSerializer      = null,
                              CustomJObjectSerializerDelegate<Image>           CustomImageSerializer            = null)
        {

            var JSON = JSONObject.Create(

                           new JProperty("uid",                         UId.   ToString()),

                           EVSEId.HasValue
                               ? new JProperty("evse_id",               EVSEId.ToString())
                               : null,

                           new JProperty("status",                      Status.ToString()),

                           StatusSchedule.SafeAny()
                               ? new JProperty("status_schedule",       new JArray(StatusSchedule.Select(status     => status.    ToJSON(CustomStatusScheduleSerializer))))
                               : null,

                           Capabilities.SafeAny()
                               ? new JProperty("capabilities",          new JArray(Capabilities.  Select(capability => capability.ToString())))
                               : null,

                           Connectors.SafeAny()
                               ? new JProperty("connectors",            new JArray(Connectors.    Select(connector  => connector. ToJSON(CustomConnectorSerializer))))
                               : null,

                           FloorLevel.IsNotNullOrEmpty()
                               ? new JProperty("floor_level",           FloorLevel)
                               : null,

                           Coordinates.HasValue
                               ? new JProperty("coordinates",           new JObject(
                                                                            new JProperty("latitude",  Coordinates.Value.Latitude. Value.ToString("0.00000##").Replace(",", ".")),
                                                                            new JProperty("longitude", Coordinates.Value.Longitude.Value.ToString("0.00000##").Replace(",", "."))
                                                                        ))
                               : null,

                           PhysicalReference.IsNotNullOrEmpty()
                               ? new JProperty("physical_reference",    PhysicalReference)
                               : null,

                           Directions.SafeAny()
                               ? new JProperty("directions",            new JArray(Directions.         Select(direction => direction.ToJSON(CustomDisplayTextSerializer))))
                               : null,

                           ParkingRestrictions.SafeAny()
                               ? new JProperty("parking_restrictions",  new JArray(ParkingRestrictions.Select(parking   => parking.  ToString())))
                               : null,

                           Images.SafeAny()
                               ? new JProperty("images",                new JArray(Images.             Select(image     => image.    ToJSON(CustomImageSerializer))))
                               : null,

                           new JProperty("last_updated",                LastUpdated.ToIso8601())

                       );

            return CustomEVSESerializer != null
                       ? CustomEVSESerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region (private) TryPrivatePatch(JSON, Patch)

        private PatchResult<JObject> TryPrivatePatch(JObject  JSON,
                                                     JObject  Patch)
        {

            foreach (var property in Patch)
            {

                if (property.Key == "uid")
                    return PatchResult<JObject>.Failed(JSON,
                                                       "Patching the 'unique identification' of an EVSE is not allowed!");

                else if (property.Key == "connectors")
                    return PatchResult<JObject>.Failed(JSON,
                                                       "Patching the 'connectors' array of an EVSE is not allowed!");
                //{

                //    if (property.Value == null)
                //        return PatchResult<JObject>.Failed(JSON,
                //                                           "Patching the 'connectors' array of a location to 'null' is not allowed!");

                //    else if (property.Value is JArray ConnectorsArray)
                //    {

                //        if (ConnectorsArray.Count == 0)
                //            return PatchResult<JObject>.Failed(JSON,
                //                                               "Patching the 'connectors' array of a location to '[]' is not allowed!");

                //        else
                //        {
                //            foreach (var connector in ConnectorsArray)
                //            {

                //                //ToDo: What to do with multiple EVSE objects having the same EVSEUId?
                //                if (connector is JObject ConnectorObject)
                //                {

                //                    if (ConnectorObject.ParseMandatory("id",
                //                                                       "connector identification",
                //                                                       Connector_Id.TryParse,
                //                                                       out Connector_Id  ConnectorId,
                //                                                       out String        ErrorResponse))
                //                    {

                //                        return PatchResult<JObject>.Failed(JSON,
                //                                                           "Patching the 'connectors' array of a location led to an error: " + ErrorResponse);

                //                    }

                //                    if (TryGetConnector(ConnectorId, out Connector Connector))
                //                    {
                //                        //Connector.Patch(ConnectorObject);
                //                    }
                //                    else
                //                    {

                //                        //ToDo: Create this "new" Connector!
                //                        return PatchResult<JObject>.Failed(JSON,
                //                                                           "Unknown connector identification!");

                //                    }

                //                }
                //                else
                //                {
                //                    return PatchResult<JObject>.Failed(JSON,
                //                                                       "Invalid JSON merge patch for 'connectors' array of a location: Data within the 'connectors' array is not a valid connector object!");
                //                }

                //            }
                //        }
                //    }

                //    else
                //    {
                //        return PatchResult<JObject>.Failed(JSON,
                //                                           "Invalid JSON merge patch for 'connectors' array of a location: JSON property 'connectors' is not an array!");
                //    }

                //}

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

        #region TryPatch(EVSEPatch, AllowDowngrades = false)

        /// <summary>
        /// Try to patch the JSON representaion of this EVSE.
        /// </summary>
        /// <param name="EVSEPatch">The JSON merge patch.</param>
        /// <param name="AllowDowngrades">Allow to set the 'lastUpdated' timestamp to an earlier value.</param>
        public PatchResult<EVSE> TryPatch(JObject  EVSEPatch,
                                          Boolean  AllowDowngrades = false)
        {

            if (EVSEPatch == null)
                return PatchResult<EVSE>.Failed(this,
                                                "The given EVSE patch must not be null!");

            lock (patchLock)
            {

                if (EVSEPatch["last_updated"] is null)
                    EVSEPatch["last_updated"] = DateTime.UtcNow.ToIso8601();

                else if (AllowDowngrades == false &&
                        EVSEPatch["last_updated"].Type == JTokenType.Date &&
                       (EVSEPatch["last_updated"].Value<DateTime>().ToIso8601().CompareTo(LastUpdated.ToIso8601()) < 1))
                {
                    return PatchResult<EVSE>.Failed(this,
                                                    "The 'lastUpdated' timestamp of the EVSE patch must be newer then the timestamp of the existing EVSE!");
                }


                var patchResult = TryPrivatePatch(ToJSON(), EVSEPatch);


                if (patchResult.IsFailed)
                    return PatchResult<EVSE>.Failed(this,
                                                    patchResult.ErrorResponse);

                if (TryParse(patchResult.PatchedData,
                             out EVSE    PatchedEVSE,
                             out String  ErrorResponse))
                {

                    return PatchResult<EVSE>.Success(PatchedEVSE,
                                                     ErrorResponse);

                }

                else
                    return PatchResult<EVSE>.Failed(this,
                                                    "Invalid JSON merge patch of an EVSE: " + ErrorResponse);

            }

        }

        #endregion


        #region (internal) SetConnector(Connector)

        internal void SetConnector(Connector Connector)
        {

            if (Connector is null)
                return;

            lock (Connectors)
            {

                Connectors = Connectors.
                                 Where (connector => connector.Id != Connector.Id).
                                 Concat(new Connector[] { Connector });

            }

        }

        #endregion


        #region ConnectorExists(ConnectorId)

        /// <summary>
        /// Checks whether any connector having the given connector identification exists.
        /// </summary>
        /// <param name="ConnectorId">A connector identification.</param>
        public Boolean ConnectorExists(Connector_Id ConnectorId)
        {

            lock (Connectors)
            {
                foreach (var connector in Connectors)
                {
                    if (connector.Id == ConnectorId)
                        return true;
                }
            }

            return false;

        }

        #endregion

        #region TryGetConnector(ConnectorId, out Connector)

        /// <summary>
        /// Try to return the connector having the given connector identification.
        /// </summary>
        /// <param name="ConnectorId">A connector identification.</param>
        /// <param name="Connector">The connector having the given connector identification.</param>
        public Boolean TryGetConnector(Connector_Id   ConnectorId,
                                       out Connector  Connector)
        {

            lock (Connectors)
            {
                foreach (var connector in Connectors)
                {
                    if (connector.Id == ConnectorId)
                    {
                        Connector = connector;
                        return true;
                    }
                }
            }

            Connector = null;
            return false;

        }

        #endregion

        #region IEnumerable<Connectors> Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            => Connectors.GetEnumerator();

        public IEnumerator<Connector> GetEnumerator()
            => Connectors.GetEnumerator();

        #endregion


        #region CalcSHA256Hash(CustomEVSESerializer = null, CustomStatusScheduleSerializer = null, ...)

        /// <summary>
        /// Calculate the SHA256 hash of the JSON representation of this EVSE in HEX.
        /// </summary>
        /// <param name="CustomEVSESerializer">A delegate to serialize custom EVSE JSON objects.</param>
        /// <param name="CustomStatusScheduleSerializer">A delegate to serialize custom status schedule JSON objects.</param>
        /// <param name="CustomConnectorSerializer">A delegate to serialize custom connector JSON objects.</param>
        /// <param name="CustomDisplayTextSerializer">A delegate to serialize custom multi-language text JSON objects.</param>
        /// <param name="CustomImageSerializer">A delegate to serialize custom image JSON objects.</param>
        public String CalcSHA256Hash(CustomJObjectSerializerDelegate<EVSE>            CustomEVSESerializer             = null,
                                     CustomJObjectSerializerDelegate<StatusSchedule>  CustomStatusScheduleSerializer   = null,
                                     CustomJObjectSerializerDelegate<Connector>       CustomConnectorSerializer        = null,
                                     CustomJObjectSerializerDelegate<DisplayText>     CustomDisplayTextSerializer      = null,
                                     CustomJObjectSerializerDelegate<Image>           CustomImageSerializer            = null)
        {

            using (var SHA256 = new SHA256Managed())
            {

                return SHA256Hash = "0x" + SHA256.ComputeHash(Encoding.Unicode.GetBytes(
                                                                  ToJSON(CustomEVSESerializer,
                                                                         CustomStatusScheduleSerializer,
                                                                         CustomConnectorSerializer,
                                                                         CustomDisplayTextSerializer,
                                                                         CustomImageSerializer).
                                                                  ToString(Newtonsoft.Json.Formatting.None)
                                                              )).
                                                  Select(value => String.Format("{0:x2}", value)).
                                                  Aggregate();

            }

        }

        #endregion


        #region Operator overloading

        #region Operator == (EVSE1, EVSE2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVSE1">An EVSE.</param>
        /// <param name="EVSE2">Another EVSE.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (EVSE EVSE1,
                                           EVSE EVSE2)
        {

            if (Object.ReferenceEquals(EVSE1, EVSE2))
                return true;

            if (EVSE1 is null || EVSE2 is null)
                return false;

            return EVSE1.Equals(EVSE2);

        }

        #endregion

        #region Operator != (EVSE1, EVSE2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVSE1">An EVSE.</param>
        /// <param name="EVSE2">Another EVSE.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (EVSE EVSE1,
                                           EVSE EVSE2)

            => !(EVSE1 == EVSE2);

        #endregion

        #region Operator <  (EVSE1, EVSE2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVSE1">An EVSE.</param>
        /// <param name="EVSE2">Another EVSE.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (EVSE EVSE1,
                                          EVSE EVSE2)

            => EVSE1 is null
                   ? throw new ArgumentNullException(nameof(EVSE1), "The given EVSE must not be null!")
                   : EVSE1.CompareTo(EVSE2) < 0;

        #endregion

        #region Operator <= (EVSE1, EVSE2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVSE1">An EVSE.</param>
        /// <param name="EVSE2">Another EVSE.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (EVSE EVSE1,
                                           EVSE EVSE2)

            => !(EVSE1 > EVSE2);

        #endregion

        #region Operator >  (EVSE1, EVSE2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVSE1">An EVSE.</param>
        /// <param name="EVSE2">Another EVSE.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (EVSE EVSE1,
                                          EVSE EVSE2)

            => EVSE1 is null
                   ? throw new ArgumentNullException(nameof(EVSE1), "The given EVSE must not be null!")
                   : EVSE1.CompareTo(EVSE2) > 0;

        #endregion

        #region Operator >= (EVSE1, EVSE2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVSE1">An EVSE.</param>
        /// <param name="EVSE2">Another EVSE.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (EVSE EVSE1,
                                           EVSE EVSE2)

            => !(EVSE1 < EVSE2);

        #endregion

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

            => EVSE is null
                   ? throw new ArgumentNullException(nameof(EVSE), "The given EVSE must not be null!")
                   : UId.CompareTo(EVSE.UId);

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

            => !(EVSE is null) &&
                   UId.Equals(EVSE.UId);

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Get the hashcode of this object.
        /// </summary>
        public override Int32 GetHashCode()

            => EVSEId.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Get a string representation of this object.
        /// </summary>
        public override String ToString()

            => EVSEId.ToString();

        #endregion


        #region ToBuilder(NewEVSEUId = null)

        /// <summary>
        /// Return a builder for this EVSE.
        /// </summary>
        /// <param name="NewEVSEUId">An optional new EVSE identification.</param>
        public Builder ToBuilder(EVSE_UId? NewEVSEUId = null)

            => new Builder(ParentLocation,

                           NewEVSEUId ?? UId,
                           Status,
                           Connectors,

                           EVSEId,
                           StatusSchedule,
                           Capabilities,
                           FloorLevel,
                           Coordinates,
                           PhysicalReference,
                           Directions,
                           ParkingRestrictions,
                           Images,

                           LastUpdated);

        #endregion

        #region (class) Builder

        /// <summary>
        /// A EVSE builder.
        /// </summary>
        public class Builder
        {

            #region Properties

            /// <summary>
            /// The parent location of this EVSE.
            /// </summary>
            public Location                      ParentLocation             { get; internal set; }

            /// <summary>
            /// Uniquely identifies the EVSE within the CPOs platform.
            /// </summary>
            public EVSE_UId?                     UId                        { get; set; }

            /// <summary>
            /// Compliant with the following specification for EVSE ID from "eMI3 standard version V1.0".
            /// </summary>
            public EVSE_Id?                      EVSEId                     { get; set; }

            /// <summary>
            /// Indicates the current status of the EVSE.
            /// </summary>
            public StatusTypes?                  Status                     { get; set; }

            /// <summary>
            /// Indicates a planned status in the future of the EVSE.
            /// </summary>
            public HashSet<StatusSchedule>       StatusSchedule             { get; }

            /// <summary>
            /// Enumeration of functionalities that the EVSE is capable of.
            /// </summary>
            public HashSet<CapabilityTypes>      Capabilities               { get; }

            /// <summary>
            /// Enumeration of available connectors at this EVSE.
            /// </summary>
            public HashSet<Connector>            Connectors                 { get; }

            /// <summary>
            /// The unique identifications of all connectors at this EVSE.
            /// </summary>
            public IEnumerable<Connector_Id>     ConnectorIds
                => Connectors.SafeSelect(connector => connector.Id);

            /// <summary>
            /// Level on which the EVSE is located (in garage buildings) in the locally displayed numbering scheme.  // 4
            /// </summary>
            public String                        FloorLevel                 { get; set; }

            /// <summary>
            /// An optional geographical location of this EVSE.
            /// </summary>
            public GeoCoordinate?                Coordinates                { get; set; }

            /// <summary>
            /// An optional number/string printed on the outside of the EVSE for visual identification. // 16
            /// </summary>
            public String                        PhysicalReference          { get; set; }

            /// <summary>
            /// Optional multi-language human-readable directions when more detailed
            /// information on how to reach the EVSE from the location is required.
            /// </summary>
            public HashSet<DisplayText>          Directions                 { get; }

            /// <summary>
            /// Optional restrictions that apply to the parking spot.
            /// </summary>
            public HashSet<ParkingRestrictions>  ParkingRestrictions        { get; }

            /// <summary>
            /// Optional links to images related to the EVSE such as photos or logos.
            /// </summary>
            public HashSet<Image>                Images                     { get; }

            /// <summary>
            /// Timestamp when this EVSE was last updated (or created).
            /// </summary>
            public DateTime?                     LastUpdated                { get; set; }

            #endregion

            #region Constructor(s)

            /// <summary>
            /// Create a new EVSE.
            /// </summary>
            /// <param name="ParentLocation">The parent location of this EVSE.</param>
            /// 
            /// <param name="UId">Uniquely identifies the EVSE within the CPOs platform.</param>
            /// <param name="Status">Indicates the current status of the EVSE.</param>
            /// <param name="Connectors">Enumeration of available connectors at this EVSE.</param>
            /// 
            /// <param name="EVSEId">Compliant with the following specification for EVSE ID from "eMI3 standard version V1.0".</param>
            /// <param name="StatusSchedule">Indicates a planned status in the future of the EVSE.</param>
            /// <param name="Capabilities">Enumeration of functionalities that the EVSE is capable of.</param>
            /// <param name="FloorLevel">Level on which the EVSE is located (in garage buildings) in the locally displayed numbering scheme.</param>
            /// <param name="Coordinates">An optional geographical location of this EVSE.</param>
            /// <param name="PhysicalReference">An optional number/string printed on the outside of the EVSE for visual identification.</param>
            /// <param name="Directions">Optional multi-language human-readable directions when more detailed information on how to reach the EVSE from the location is required.</param>
            /// <param name="ParkingRestrictions">Optional restrictions that apply to the parking spot.</param>
            /// <param name="Images">Optional links to images related to the EVSE such as photos or logos.</param>
            /// 
            /// <param name="LastUpdated">Timestamp when this EVSE was last updated (or created).</param>
            internal Builder(Location                          ParentLocation,

                             EVSE_UId?                         UId                   = null,
                             StatusTypes?                      Status                = null,
                             IEnumerable<Connector>            Connectors            = null,

                             EVSE_Id?                          EVSEId                = null,
                             IEnumerable<StatusSchedule>       StatusSchedule        = null,
                             IEnumerable<CapabilityTypes>      Capabilities          = null,
                             String                            FloorLevel            = null,
                             GeoCoordinate?                    Coordinates           = null,
                             String                            PhysicalReference     = null,
                             IEnumerable<DisplayText>          Directions            = null,
                             IEnumerable<ParkingRestrictions>  ParkingRestrictions   = null,
                             IEnumerable<Image>                Images                = null,

                             DateTime?                         LastUpdated           = null)

            {

                this.ParentLocation        = ParentLocation;

                this.UId                   = UId;
                this.Status                = Status;
                this.Connectors            = Connectors          != null ? new HashSet<Connector>          (Connectors)          : new HashSet<Connector>();

                this.EVSEId                = EVSEId;
                this.StatusSchedule        = StatusSchedule      != null ? new HashSet<StatusSchedule>     (StatusSchedule)      : new HashSet<StatusSchedule>();
                this.Capabilities          = Capabilities        != null ? new HashSet<CapabilityTypes>    (Capabilities)        : new HashSet<CapabilityTypes>();
                this.FloorLevel            = FloorLevel;
                this.Coordinates           = Coordinates;
                this.PhysicalReference     = PhysicalReference;
                this.Directions            = Directions          != null ? new HashSet<DisplayText>        (Directions)          : new HashSet<DisplayText>();
                this.ParkingRestrictions   = ParkingRestrictions != null ? new HashSet<ParkingRestrictions>(ParkingRestrictions) : new HashSet<ParkingRestrictions>();
                this.Images                = Images              != null ? new HashSet<Image>              (Images)              : new HashSet<Image>();

                this.LastUpdated           = LastUpdated;

            }

            #endregion

            #region ToImmutable

            /// <summary>
            /// Return an immutable version of the EVSE.
            /// </summary>
            public static implicit operator EVSE(Builder Builder)

                => Builder?.ToImmutable;


            /// <summary>
            /// Return an immutable version of the EVSE.
            /// </summary>
            public EVSE ToImmutable
            {
                get
                {

                    if (!UId.HasValue)
                        throw new ArgumentNullException(nameof(UId),     "The unique identification must not be null or empty!");

                    if (!Status.HasValue)
                        throw new ArgumentNullException(nameof(Status),  "The status must not be null or empty!");


                    return new EVSE(ParentLocation,

                                    UId.   Value,
                                    Status.Value,
                                    Connectors,

                                    EVSEId,
                                    StatusSchedule,
                                    Capabilities,
                                    FloorLevel,
                                    Coordinates,
                                    PhysicalReference,
                                    Directions,
                                    ParkingRestrictions,
                                    Images,

                                    LastUpdated);

                }
            }

            #endregion

        }

        #endregion

    }

}
