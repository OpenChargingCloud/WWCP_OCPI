/*
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

using System.Text;
using System.Security.Cryptography;
using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Aegir;
using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPI;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_3_0
{

    /// <summary>
    /// The Electric Vehicle Supply Equipment (EVSE) is the part that controls
    /// the power supply to a single electric vehicle.
    /// </summary>
    public class EVSE : //IHasId<EVSE_Id>,
                        IEquatable<EVSE>,
                        IComparable<EVSE>,
                        IComparable,
                        IEnumerable<Connector>
    {

        #region Data

        /// <summary>
        /// The default JSON-LD context of locations.
        /// </summary>
        public static readonly JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/contexts/OCPI/2.3/evse");

        private readonly Lock patchLock = new();

        #endregion

        #region Properties

        /// <summary>
        /// The parent location of this EVSE.
        /// </summary>
        public Location?                        ParentLocation             { get; internal set; }

        /// <summary>
        /// The unique identification of the EVSE within the CPOs platform.
        /// For interoperability please make sure, that the internal EVSE UId has the same value as the official EVSE Id!
        /// </summary>
        [Mandatory]
        public EVSE_UId                         UId                        { get; }

        /// <summary>
        /// The official unique identification of the EVSE.
        /// For interoperability please make sure, that the internal EVSE UId has the same value as the official EVSE Id!
        /// </summary>
        [Optional]
        public EVSE_Id?                         EVSEId                     { get; }

        /// <summary>
        /// The current status of the EVSE.
        /// </summary>
        [Mandatory]
        public StatusType                       Status                     { get; }

        /// <summary>
        /// The enumeration of planned future status of the EVSE.
        /// </summary>
        [Optional]
        public IEnumerable<StatusSchedule>      StatusSchedule             { get; }

        /// <summary>
        /// The enumeration of functionalities that the EVSE is capable of.
        /// </summary>
        [Optional]
        public IEnumerable<Capability>          Capabilities               { get; }

        /// <summary>
        /// The optional energy meter, e.g. for the German calibration law.
        /// </summary>
        [Optional, VendorExtension(VE.GraphDefined, VE.Eichrecht)]
        public EnergyMeter<EVSE>?               EnergyMeter                { get; }

        /// <summary>
        /// The enumeration of available connectors attached to this EVSE.
        /// </summary>
        [Mandatory]
        public IEnumerable<Connector>           Connectors                 { get; private set; }

        /// <summary>
        /// The enumeration of connector identifications attached to this EVSE.
        /// </summary>
        [Optional]
        public IEnumerable<Connector_Id>        ConnectorIds
            => Connectors.Select(connector => connector.Id);

        /// <summary>
        /// The optional floor level on which the EVSE is located (in garage buildings)
        /// in the locally displayed numbering scheme.
        /// string(4)
        /// </summary>
        [Optional]
        public String?                          FloorLevel                 { get; }

        /// <summary>
        /// The optional geographical location of the EVSE.
        /// </summary>
        [Optional]
        public GeoCoordinate?                   Coordinates                { get; }

        /// <summary>
        /// The optional number/string printed on the outside of the EVSE for visual identification.
        /// string(16)
        /// </summary>
        [Optional]
        public String?                          PhysicalReference          { get; }

        /// <summary>
        /// The optional multi-language human-readable directions when more detailed
        /// information on how to reach the EVSE from the location is required.
        /// </summary>
        [Optional]
        public IEnumerable<DisplayText>         Directions                 { get; }

        /// <summary>
        /// Optional applicable restrictions on who can charge at the EVSE, apart from those related to the vehicle type.
        /// </summary>
        [Optional]
        public IEnumerable<ParkingRestriction>  ParkingRestrictions        { get; }

        /// <summary>
        /// Optional references to the parking space or spaces that can be used by vehicles charging at this EVSE.
        /// </summary>
        [Optional]
        public IEnumerable<EVSEParking>         Parking                    { get; }

        /// <summary>
        /// The optional enumeration of images related to the EVSE such as photos or logos.
        /// </summary>
        [Optional]
        public IEnumerable<Image>               Images                     { get; }

        /// <summary>
        /// The optional enumeration of eMSPs offering contract-based payment options that are accepted at this EVSE.
        /// </summary>
        [Optional]
        public IEnumerable<EMSP_Id>             AcceptedServiceProviders { get; }


        /// <summary>
        /// The timestamp when this EVSE was created.
        /// </summary>
        [Mandatory, VendorExtension(VE.GraphDefined, VE.Pagination)]
        public DateTime                         Created                    { get; }

        /// <summary>
        /// Timestamp when this EVSE was last updated (or created).
        /// </summary>
        [Mandatory]
        public DateTime                         LastUpdated                { get; }

        /// <summary>
        /// The SHA256 hash of the JSON representation of this EVSE used as HTTP ETag.
        /// </summary>
        [Mandatory, VendorExtension(VE.GraphDefined)]
        public String                           ETag                       { get; private set; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new EVSE.
        /// </summary>
        /// <param name="ParentLocation">The parent location of this EVSE.</param>
        /// 
        /// <param name="UId">An unique identification of the EVSE within the CPOs platform. For interoperability please make sure, that the internal EVSE UId has the same value as the official EVSE Id!</param>
        /// <param name="Status">A current status of the EVSE.</param>
        /// <param name="Connectors">The enumeration of available connectors attached to this EVSE.</param>
        /// 
        /// <param name="EVSEId">The official unique identification of the EVSE. For interoperability please make sure, that the internal EVSE UId has the same value as the official EVSE Id!</param>
        /// <param name="StatusSchedule">An enumeration of planned future status of the EVSE.</param>
        /// <param name="Capabilities">An enumeration of functionalities that the EVSE is capable of.</param>
        /// <param name="EnergyMeter">An optional energy meter, e.g. for the German calibration law.</param>
        /// <param name="FloorLevel">An optional floor level on which the EVSE is located (in garage buildings) in the locally displayed numbering scheme.</param>
        /// <param name="Coordinates">An optional geographical location of the EVSE.</param>
        /// <param name="PhysicalReference">An optional number/string printed on the outside of the EVSE for visual identification.</param>
        /// <param name="Directions">An optional multi-language human-readable directions when more detailed information on how to reach the EVSE from the location is required.</param>
        /// <param name="Parking">An optional reference to the parking space or spaces that can be used by vehicles charging at this EVSE.</param>
        /// <param name="Images">An optional enumeration of images related to the EVSE such as photos or logos.</param>
        /// <param name="AcceptedServiceProviders">An optional enumeration of eMSPs offering contract-based payment options that are accepted at this EVSE.</param>
        /// 
        /// <param name="Created">The optional timestamp when this EVSE was created.</param>
        /// <param name="LastUpdated">The optional timestamp when this EVSE was last updated (or created).</param>
        /// 
        /// <param name="CustomEVSESerializer">A delegate to serialize custom EVSE JSON objects.</param>
        /// <param name="CustomStatusScheduleSerializer">A delegate to serialize custom status schedule JSON objects.</param>
        /// <param name="CustomConnectorSerializer">A delegate to serialize custom connector JSON objects.</param>
        /// <param name="CustomEVSEEnergyMeterSerializer">A delegate to serialize custom EVSE energy meter JSON objects.</param>
        /// <param name="CustomTransparencySoftwareStatusSerializer">A delegate to serialize custom transparency software status JSON objects.</param>
        /// <param name="CustomTransparencySoftwareSerializer">A delegate to serialize custom transparency software JSON objects.</param>
        /// <param name="CustomDisplayTextSerializer">A delegate to serialize custom multi-language text JSON objects.</param>
        /// <param name="CustomImageSerializer">A delegate to serialize custom image JSON objects.</param>
        public EVSE(EVSE_UId                                                      UId,
                    StatusType                                                    Status,
                    IEnumerable<Connector>                                        Connectors,

                    EVSE_Id?                                                      EVSEId                                       = null,
                    IEnumerable<StatusSchedule>?                                  StatusSchedule                               = null,
                    IEnumerable<Capability>?                                      Capabilities                                 = null,
                    EnergyMeter<EVSE>?                                            EnergyMeter                                  = null,
                    String?                                                       FloorLevel                                   = null,
                    GeoCoordinate?                                                Coordinates                                  = null,
                    String?                                                       PhysicalReference                            = null,
                    IEnumerable<DisplayText>?                                     Directions                                   = null,
                    IEnumerable<ParkingRestriction>?                              ParkingRestrictions                          = null,
                    IEnumerable<EVSEParking>?                                     Parking                                      = null,
                    IEnumerable<Image>?                                           Images                                       = null,
                    IEnumerable<EMSP_Id>?                                         AcceptedServiceProviders                     = null,

                    DateTime?                                                     Created                                      = null,
                    DateTime?                                                     LastUpdated                                  = null,
                    String?                                                       ETag                                         = null,

                    Location?                                                     ParentLocation                               = null,
                    EMSP_Id?                                                      EMSPId                                       = null,
                    CustomJObjectSerializerDelegate<EVSE>?                        CustomEVSESerializer                         = null,
                    CustomJObjectSerializerDelegate<StatusSchedule>?              CustomStatusScheduleSerializer               = null,
                    CustomJObjectSerializerDelegate<Connector>?                   CustomConnectorSerializer                    = null,
                    CustomJObjectSerializerDelegate<EnergyMeter<EVSE>>?           CustomEVSEEnergyMeterSerializer              = null,
                    CustomJObjectSerializerDelegate<TransparencySoftwareStatus>?  CustomTransparencySoftwareStatusSerializer   = null,
                    CustomJObjectSerializerDelegate<TransparencySoftware>?        CustomTransparencySoftwareSerializer         = null,
                    CustomJObjectSerializerDelegate<DisplayText>?                 CustomDisplayTextSerializer                  = null,
                    CustomJObjectSerializerDelegate<EVSEParking>?                 CustomEVSEParkingSerializer                  = null,
                    CustomJObjectSerializerDelegate<Image>?                       CustomImageSerializer                        = null)

        {

            this.ParentLocation            = ParentLocation;

            this.UId                       = UId;
            this.Status                    = Status;
            this.Connectors                = Connectors.               Distinct();

            if (!this.Connectors.  Any())
                throw new ArgumentNullException(nameof(Connectors),  "The given enumeration of connectors must not be null or empty!");

            this.EVSEId                    = EVSEId;
            this.StatusSchedule            = StatusSchedule?.          Distinct() ?? [];
            this.Capabilities              = Capabilities?.            Distinct() ?? [];
            this.EnergyMeter               = EnergyMeter;
            this.FloorLevel                = FloorLevel?.              Trim();
            this.Coordinates               = Coordinates;
            this.PhysicalReference         = PhysicalReference?.       Trim();
            this.Directions                = Directions?.              Distinct() ?? [];
            this.ParkingRestrictions       = ParkingRestrictions?.     Distinct() ?? [];
            this.Parking                   = Parking?.                 Distinct() ?? [];
            this.Images                    = Images?.                  Distinct() ?? [];
            this.AcceptedServiceProviders  = AcceptedServiceProviders?.Distinct() ?? [];

            var created                    = Created     ?? LastUpdated ?? Timestamp.Now;
            this.Created                   = created;
            this.LastUpdated               = LastUpdated ?? created;

            foreach (var connector in this.Connectors)
                connector.ParentEVSE = this;

            this.ETag                      = ETag ?? CalcSHA256Hash(
                                                         EMSPId,
                                                         CustomEVSESerializer,
                                                         CustomStatusScheduleSerializer,
                                                         CustomConnectorSerializer,
                                                         CustomEVSEEnergyMeterSerializer,
                                                         CustomTransparencySoftwareStatusSerializer,
                                                         CustomTransparencySoftwareSerializer,
                                                         CustomDisplayTextSerializer,
                                                         CustomEVSEParkingSerializer,
                                                         CustomImageSerializer
                                                     );

        }

        #endregion


        #region (static) Parse   (JSON, EVSEUIdURL = null, CustomEVSEParser = null)

        /// <summary>
        /// Parse the given JSON representation of an EVSE.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="EVSEUIdURL">An optional EVSE identification, e.g. from the HTTP URL.</param>
        /// <param name="CustomEVSEParser">A delegate to parse custom EVSE JSON objects.</param>
        public static EVSE Parse(JObject                             JSON,
                                 EVSE_UId?                           EVSEUIdURL         = null,
                                 CustomJObjectParserDelegate<EVSE>?  CustomEVSEParser   = null)
        {

            if (TryParse(JSON,
                         out var evse,
                         out var errorResponse,
                         EVSEUIdURL,
                         CustomEVSEParser))
            {
                return evse;
            }

            throw new ArgumentException("The given JSON representation of an EVSE is invalid: " + errorResponse,
                                        nameof(JSON));

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
        public static Boolean TryParse(JObject                           JSON,
                                       [NotNullWhen(true)]  out EVSE?    EVSE,
                                       [NotNullWhen(false)] out String?  ErrorResponse)

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
        public static Boolean TryParse(JObject                             JSON,
                                       [NotNullWhen(true)]  out EVSE?      EVSE,
                                       [NotNullWhen(false)] out String?    ErrorResponse,
                                       EVSE_UId?                           EVSEUIdURL,
                                       CustomJObjectParserDelegate<EVSE>?  CustomEVSEParser   = null)
        {

            try
            {

                EVSE = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse UId                         [optional]

                if (JSON.ParseOptional("uid",
                                       "internal EVSE identification",
                                       EVSE_UId.TryParse,
                                       out EVSE_UId? EVSEUIdBody,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
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

                #region Parse Status                      [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "EVSE status",
                                         StatusType.TryParse,
                                         out StatusType Status,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Connectors                  [mandatory]

                if (!JSON.ParseMandatoryJSON<Connector, Connector_Id>("connectors",
                                                                      "connectors",
                                                                      Connector.TryParse,
                                                                      out IEnumerable<Connector> Connectors,
                                                                      out ErrorResponse))
                {
                    return false;
                }

                #endregion


                #region Parse EVSEId                      [optional]

                if (JSON.ParseOptional("evse_id",
                                       "official EVSE identification",
                                       EVSE_Id.TryParse,
                                       out EVSE_Id? EVSEId,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse StatusSchedule              [optional]

                if (JSON.ParseOptionalJSON("status_schedule",
                                           "status schedule",
                                           OCPIv2_3_0.StatusSchedule.TryParse,
                                           out IEnumerable<StatusSchedule> StatusSchedule,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse Capabilities                [optional]

                if (JSON.ParseOptionalHashSet("capabilities",
                                              "capabilities",
                                              Capability.TryParse,
                                              out HashSet<Capability> Capabilities,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse EnergyMeter                 [optional]

                if (JSON.ParseOptionalJSON("energy_meter",
                                           "energy meter",
                                           EnergyMeter<EVSE>.TryParse,
                                           out EnergyMeter<EVSE>? EnergyMeter,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse FloorLevel                  [optional]

                var FloorLevel = JSON.GetString("floor_level");

                #endregion

                #region Parse Coordinates                 [optional]

                if (JSON.ParseOptionalJSON("coordinates",
                                           "geo coordinates",
                                           GeoCoordinate.TryParse,
                                           out GeoCoordinate? Coordinates,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse PhysicalReference           [optional]

                var PhysicalReference = JSON.GetString("physical_reference");

                #endregion

                #region Parse Directions                  [optional]

                if (JSON.ParseOptionalJSON("directions",
                                           "directions",
                                           DisplayText.TryParse,
                                           out IEnumerable<DisplayText> Directions,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse ParkingRestrictions         [optional]

                if (JSON.ParseOptionalHashSet("parking_restrictions",
                                              "parking restrictions",
                                              ParkingRestriction.TryParse,
                                              out HashSet<ParkingRestriction> parkingRestrictions,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse Parking                     [optional]

                if (JSON.ParseOptionalHashSet("parking",
                                              "EVSE parking",
                                              EVSEParking.TryParse,
                                              out HashSet<EVSEParking> parking,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse Images                      [optional]

                if (JSON.ParseOptionalJSON("images",
                                           "images",
                                           Image.TryParse,
                                           out IEnumerable<Image> Images,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse AcceptedServiceProviders    [optional]

                if (JSON.ParseOptionalHashSet("accepted_service_providers",
                                              "accepted e-mobility service providers",
                                              EMSP_Id.TryParse,
                                              out HashSet<EMSP_Id> AcceptedServiceProviders,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                #region Parse Created                     [optional, VendorExtension]

                if (JSON.ParseOptional("created",
                                       "created",
                                       out DateTime? Created,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse LastUpdated                 [mandatory]

                if (!JSON.ParseMandatory("last_updated",
                                         "last updated",
                                         out DateTime LastUpdated,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                EVSE = new EVSE(

                           EVSEUIdBody ?? EVSEUIdURL!.Value,
                           Status,
                           Connectors,

                           EVSEId,
                           StatusSchedule,
                           Capabilities,
                           EnergyMeter,
                           FloorLevel,
                           Coordinates,
                           PhysicalReference,
                           Directions,
                           parkingRestrictions,
                           parking,
                           Images,
                           AcceptedServiceProviders,

                           Created,
                           LastUpdated

                       );


                if (CustomEVSEParser is not null)
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

        #region ToJSON(CustomEVSESerializer = null, CustomStatusScheduleSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="IncludeCreatedTimestamp">Whether to include the created timestamp in the JSON representation.</param>
        /// <param name="IncludeEnergyMeter">Whether to include the energy meter in the JSON representation.</param>
        /// <param name="IncludeExtensions">Whether to include optional data model extensions.</param>
        /// <param name="CustomEVSESerializer">A delegate to serialize custom EVSE JSON objects.</param>
        /// <param name="CustomStatusScheduleSerializer">A delegate to serialize custom status schedule JSON objects.</param>
        /// <param name="CustomConnectorSerializer">A delegate to serialize custom connector JSON objects.</param>
        /// <param name="CustomEVSEEnergyMeterSerializer">A delegate to serialize custom EVSE energy meter JSON objects.</param>
        /// <param name="CustomTransparencySoftwareStatusSerializer">A delegate to serialize custom transparency software status JSON objects.</param>
        /// <param name="CustomTransparencySoftwareSerializer">A delegate to serialize custom transparency software JSON objects.</param>
        /// <param name="CustomDisplayTextSerializer">A delegate to serialize custom multi-language text JSON objects.</param>
        /// <param name="CustomEVSEParkingSerializer">A delegate to serialize custom EVSE parking JSON objects.</param>
        /// <param name="CustomImageSerializer">A delegate to serialize custom image JSON objects.</param>
        public JObject ToJSON(Boolean                                                       IncludeCreatedTimestamp                      = true,
                              Boolean                                                       IncludeEnergyMeter                           = true,
                              Boolean                                                       IncludeExtensions                            = true,
                              EMSP_Id?                                                      EMSPId                                       = null,
                              CustomJObjectSerializerDelegate<EVSE>?                        CustomEVSESerializer                         = null,
                              CustomJObjectSerializerDelegate<StatusSchedule>?              CustomStatusScheduleSerializer               = null,
                              CustomJObjectSerializerDelegate<Connector>?                   CustomConnectorSerializer                    = null,
                              CustomJObjectSerializerDelegate<EnergyMeter<EVSE>>?           CustomEVSEEnergyMeterSerializer              = null,
                              CustomJObjectSerializerDelegate<TransparencySoftwareStatus>?  CustomTransparencySoftwareStatusSerializer   = null,
                              CustomJObjectSerializerDelegate<TransparencySoftware>?        CustomTransparencySoftwareSerializer         = null,
                              CustomJObjectSerializerDelegate<DisplayText>?                 CustomDisplayTextSerializer                  = null,
                              CustomJObjectSerializerDelegate<EVSEParking>?                 CustomEVSEParkingSerializer                  = null,
                              CustomJObjectSerializerDelegate<Image>?                       CustomImageSerializer                        = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("uid",                          UId.         ToString()),

                           EVSEId.HasValue
                               ? new JProperty("evse_id",                      EVSEId.Value.ToString())
                               : null,

                                 new JProperty("status",                       Status.      ToString()),

                           StatusSchedule.Any()
                               ? new JProperty("status_schedule",              new JArray(StatusSchedule.Select (statusSchedule => statusSchedule.ToJSON(CustomStatusScheduleSerializer))))
                               : null,

                           Capabilities.Any()
                               ? new JProperty("capabilities",                 new JArray(Capabilities.  Select (capability     => capability.    ToString())))
                               : null,

                           Connectors.Any()
                               ? new JProperty("connectors",                   new JArray(Connectors.    OrderBy(connector      => connector.Id).
                                                                                                         Select (connector      => connector.     ToJSON(true,
                                                                                                                                                         true,
                                                                                                                                                         EMSPId,
                                                                                                                                                         CustomConnectorSerializer))))
                               : null,

                           IncludeEnergyMeter && EnergyMeter is not null
                               ? new JProperty("energy_meter",                 EnergyMeter. ToJSON(CustomEVSEEnergyMeterSerializer,
                                                                                                   CustomTransparencySoftwareStatusSerializer,
                                                                                                   CustomTransparencySoftwareSerializer))
                               : null,

                           FloorLevel.IsNotNullOrEmpty()
                               ? new JProperty("floor_level",                  FloorLevel)
                               : null,

                           Coordinates.HasValue
                               ? new JProperty("coordinates",                  new JObject(
                                                                                   new JProperty("latitude",   Coordinates.Value.Latitude. Value.ToString("0.00000##").Replace(",", ".")),
                                                                                   new JProperty("longitude",  Coordinates.Value.Longitude.Value.ToString("0.00000##").Replace(",", "."))
                                                                               ))
                               : null,

                           PhysicalReference.IsNotNullOrEmpty()
                               ? new JProperty("physical_reference",           PhysicalReference)
                               : null,

                           Directions.              Any()
                               ? new JProperty("directions",                   new JArray(Directions.              Select (displayText        => displayText.       ToJSON(CustomDisplayTextSerializer))))
                               : null,

                           ParkingRestrictions.     Any()
                               ? new JProperty("parking_restrictions",         new JArray(ParkingRestrictions.     Select (parkingRestriction => parkingRestriction.ToString())))
                               : null,

                           Parking.                 Any()
                               ? new JProperty("parking",                      new JArray(Parking.                 Select (parking            => parking.           ToJSON(CustomEVSEParkingSerializer))))
                               : null,

                           Images.                  Any()
                               ? new JProperty("images",                       new JArray(Images.                  Select (image              => image.             ToJSON(CustomImageSerializer))))
                               : null,

                           AcceptedServiceProviders.Any()
                               ? new JProperty("accepted_service_providers",   new JArray(AcceptedServiceProviders.Select (emsp               => emsp.              ToString())))
                               : null,


                           IncludeCreatedTimestamp
                               ? new JProperty("created",                      Created.    ToISO8601())
                               : null,

                                 new JProperty("last_updated",                 LastUpdated.ToISO8601())

                       );

            return CustomEVSESerializer is not null
                       ? CustomEVSESerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this EVSE.
        /// </summary>
        public EVSE Clone()

            => new (

                   UId.                     Clone(),
                   Status.                  Clone(),
                   Connectors.              Select(connector          => connector.         Clone()),

                   EVSEId?.                 Clone(),
                   StatusSchedule.          Select(statusSchedule     => statusSchedule.    Clone()),
                   Capabilities.            Select(capability         => capability.        Clone()),
                   EnergyMeter?.            Clone(),
                   FloorLevel.              CloneNullableString(),
                   Coordinates?.            Clone(),
                   PhysicalReference.       CloneNullableString(),
                   Directions.              Select(displayText        => displayText.       Clone()),
                   ParkingRestrictions.     Select(parkingRestriction => parkingRestriction.Clone()),
                   Parking.                 Select(parking            => parking.           Clone()),
                   Images.                  Select(image              => image.             Clone()),
                   AcceptedServiceProviders.Select(emspId             => emspId.            Clone()),

                   Created,
                   LastUpdated,
                   ETag.                    CloneString(),

                   ParentLocation

               );

        #endregion


        #region (private) TryPrivatePatch(JSON, Patch)

        private PatchResult<JObject> TryPrivatePatch(JObject  JSON,
                                                     JObject  Patch,
                                                     EventTracking_Id  EventTrackingId)
        {

            foreach (var property in Patch)
            {

                if (property.Key == "uid")
                    return PatchResult<JObject>.Failed(EventTrackingId, JSON,
                                                       "Patching the 'unique identification' of an EVSE is not allowed!");

                else if (property.Key == "connectors")
                    return PatchResult<JObject>.Failed(EventTrackingId, JSON,
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

        #region TryPatch(EVSEPatch, AllowDowngrades = false)

        /// <summary>
        /// Try to patch the JSON representation of this EVSE.
        /// </summary>
        /// <param name="EVSEPatch">The JSON merge patch.</param>
        /// <param name="AllowDowngrades">Allow to set the 'lastUpdated' timestamp to an earlier value.</param>
        public PatchResult<EVSE> TryPatch(JObject            EVSEPatch,
                                          Boolean            AllowDowngrades   = false,
                                          EventTracking_Id?  EventTrackingId   = null)
        {

            EventTrackingId ??= EventTracking_Id.New;

            if (EVSEPatch is null)
                return PatchResult<EVSE>.Failed(EventTrackingId, this,
                                                "The given EVSE patch must not be null!");

            lock (patchLock)
            {

                if (EVSEPatch["last_updated"] is null)
                    EVSEPatch["last_updated"] = Timestamp.Now.ToISO8601();

                else if (AllowDowngrades == false &&
                        EVSEPatch["last_updated"].Type == JTokenType.Date &&
                       (EVSEPatch["last_updated"].Value<DateTime>().ToISO8601().CompareTo(LastUpdated.ToISO8601()) < 1))
                {
                    return PatchResult<EVSE>.Failed(EventTrackingId, this,
                                                    "The 'lastUpdated' timestamp of the EVSE patch must be newer then the timestamp of the existing EVSE!");
                }


                var patchResult = TryPrivatePatch(ToJSON(), EVSEPatch, EventTrackingId);


                if (patchResult.IsFailed)
                    return PatchResult<EVSE>.Failed(EventTrackingId, this,
                                                    patchResult.ErrorResponse);

                if (TryParse(patchResult.PatchedData,
                             out var patchedEVSE,
                             out var errorResponse))
                {

                    return PatchResult<EVSE>.Success(EventTrackingId, patchedEVSE,
                                                     errorResponse);

                }

                else
                    return PatchResult<EVSE>.Failed(EventTrackingId, this,
                                                    "Invalid JSON merge patch of an EVSE: " + errorResponse);

            }

        }

        #endregion


        internal IEnumerable<Tariff_Id> GetTariffIds(Connector_Id?  ConnectorId   = null,
                                                     EMSP_Id?       EMSPId        = null)

            => ParentLocation?.GetTariffIds(UId,
                                            ConnectorId,
                                            EMSPId) ?? [];


        #region (internal) UpdateConnector(Connector)

        internal void UpdateConnector(Connector Connector)
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

        #region CalcSHA256Hash(CustomEVSESerializer = null, CustomStatusScheduleSerializer = null, ...)

        /// <summary>
        /// Calculate the SHA256 hash of the JSON representation of this location in HEX.
        /// </summary>
        /// <param name="CustomEVSESerializer">A delegate to serialize custom EVSE JSON objects.</param>
        /// <param name="CustomStatusScheduleSerializer">A delegate to serialize custom status schedule JSON objects.</param>
        /// <param name="CustomConnectorSerializer">A delegate to serialize custom connector JSON objects.</param>
        /// <param name="CustomEVSEEnergyMeterSerializer">A delegate to serialize custom EVSE energy meter JSON objects.</param>
        /// <param name="CustomTransparencySoftwareStatusSerializer">A delegate to serialize custom transparency software status JSON objects.</param>
        /// <param name="CustomTransparencySoftwareSerializer">A delegate to serialize custom transparency software JSON objects.</param>
        /// <param name="CustomDisplayTextSerializer">A delegate to serialize custom multi-language text JSON objects.</param>
        /// <param name="CustomImageSerializer">A delegate to serialize custom image JSON objects.</param>
        public String CalcSHA256Hash(EMSP_Id?                                                      EMSPId                                       = null,
                                     CustomJObjectSerializerDelegate<EVSE>?                        CustomEVSESerializer                         = null,
                                     CustomJObjectSerializerDelegate<StatusSchedule>?              CustomStatusScheduleSerializer               = null,
                                     CustomJObjectSerializerDelegate<Connector>?                   CustomConnectorSerializer                    = null,
                                     CustomJObjectSerializerDelegate<EnergyMeter<EVSE>>?           CustomEVSEEnergyMeterSerializer              = null,
                                     CustomJObjectSerializerDelegate<TransparencySoftwareStatus>?  CustomTransparencySoftwareStatusSerializer   = null,
                                     CustomJObjectSerializerDelegate<TransparencySoftware>?        CustomTransparencySoftwareSerializer         = null,
                                     CustomJObjectSerializerDelegate<DisplayText>?                 CustomDisplayTextSerializer                  = null,
                                     CustomJObjectSerializerDelegate<EVSEParking>?                 CustomEVSEParkingSerializer                  = null,
                                     CustomJObjectSerializerDelegate<Image>?                       CustomImageSerializer                        = null)
        {

            ETag = SHA256.HashData(
                       ToJSON(
                           true,
                           true,
                           true,
                           EMSPId,
                           CustomEVSESerializer,
                           CustomStatusScheduleSerializer,
                           CustomConnectorSerializer,
                           CustomEVSEEnergyMeterSerializer,
                           CustomTransparencySoftwareStatusSerializer,
                           CustomTransparencySoftwareSerializer,
                           CustomDisplayTextSerializer,
                           CustomEVSEParkingSerializer,
                           CustomImageSerializer
                       ).ToUTF8Bytes()
                   ).ToBase64();

            return ETag;

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

        #region GetConnector   (ConnectorId)

        /// <summary>
        /// Return the connector having the given connector identification.
        /// </summary>
        /// <param name="ConnectorId">A connector identification.</param>
        public Connector? GetConnector(Connector_Id ConnectorId)
        {

            if (TryGetConnector(ConnectorId, out var connector))
                return connector;

            return null;

        }

        #endregion

        #region TryGetConnector(ConnectorId, out Connector)

        /// <summary>
        /// Try to return the connector having the given connector identification.
        /// </summary>
        /// <param name="ConnectorId">A connector identification.</param>
        /// <param name="Connector">The connector having the given connector identification.</param>
        public Boolean TryGetConnector(Connector_Id    ConnectorId,
                                       out Connector?  Connector)
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


        #region Operator overloading

        #region Operator == (EVSE1, EVSE2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVSE1">An EVSE.</param>
        /// <param name="EVSE2">Another EVSE.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (EVSE? EVSE1,
                                           EVSE? EVSE2)
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
        public static Boolean operator != (EVSE? EVSE1,
                                           EVSE? EVSE2)

            => !(EVSE1 == EVSE2);

        #endregion

        #region Operator <  (EVSE1, EVSE2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVSE1">An EVSE.</param>
        /// <param name="EVSE2">Another EVSE.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (EVSE? EVSE1,
                                          EVSE? EVSE2)

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
        public static Boolean operator <= (EVSE? EVSE1,
                                           EVSE? EVSE2)

            => !(EVSE1 > EVSE2);

        #endregion

        #region Operator >  (EVSE1, EVSE2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVSE1">An EVSE.</param>
        /// <param name="EVSE2">Another EVSE.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (EVSE? EVSE1,
                                          EVSE? EVSE2)

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
        public static Boolean operator >= (EVSE? EVSE1,
                                           EVSE? EVSE2)

            => !(EVSE1 < EVSE2);

        #endregion

        #endregion

        #region IComparable<EVSE> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two EVSEs.
        /// </summary>
        /// <param name="Object">An EVSE to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is EVSE evse
                   ? CompareTo(evse)
                   : throw new ArgumentException("The given object is not an EVSE!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(EVSE)

        /// <summary>
        /// Compares two EVSEs.
        /// </summary>
        /// <param name="EVSE">An EVSE to compare with.</param>
        public Int32 CompareTo(EVSE? EVSE)
        {

            if (EVSE is null)
                throw new ArgumentNullException(nameof(EVSE), "The given EVSE must not be null!");

            var c = UId.        CompareTo(EVSE.UId);

            if (c == 0)
                c = Status.     CompareTo(EVSE.Status);

            if (c == 0)
                c = LastUpdated.CompareTo(EVSE.LastUpdated);

            if (c == 0)
                c = ETag.       CompareTo(EVSE.ETag);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<EVSE> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two EVSEs for equality.
        /// </summary>
        /// <param name="Object">An EVSE to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is EVSE evse &&
                   Equals(evse);

        #endregion

        #region Equals(EVSE)

        /// <summary>
        /// Compares two EVSEs for equality.
        /// </summary>
        /// <param name="EVSE">An EVSE to compare with.</param>
        public Boolean Equals(EVSE? EVSE)

            => EVSE is not null &&

               UId.                    Equals(EVSE.UId)                     &&
               Status.                 Equals(EVSE.Status)                  &&
               LastUpdated.ToISO8601().Equals(EVSE.LastUpdated.ToISO8601()) &&

            ((!EVSEId.           HasValue    && !EVSE.EVSEId.           HasValue)    ||
              (EVSEId.           HasValue    &&  EVSE.EVSEId.           HasValue    && EVSEId.     Value.Equals(EVSE.EVSEId.     Value))) &&

            ((!Coordinates.      HasValue    && !EVSE.Coordinates.      HasValue)    ||
              (Coordinates.      HasValue    &&  EVSE.Coordinates.      HasValue    && Coordinates.Value.Equals(EVSE.Coordinates.Value))) &&

             ((EnergyMeter       is     null &&  EVSE.EnergyMeter       is     null) ||
              (EnergyMeter       is not null &&  EVSE.EnergyMeter       is not null && EnergyMeter.      Equals(EVSE.EnergyMeter)))       &&

             ((FloorLevel        is     null &&  EVSE.FloorLevel        is     null) ||
              (FloorLevel        is not null &&  EVSE.FloorLevel        is not null && FloorLevel.       Equals(EVSE.FloorLevel)))        &&

             ((PhysicalReference is     null &&  EVSE.PhysicalReference is     null) ||
              (PhysicalReference is not null &&  EVSE.PhysicalReference is not null && PhysicalReference.Equals(EVSE.PhysicalReference))) &&

             ((Parking           is     null &&  EVSE.Parking           is     null) ||
              (Parking           is not null &&  EVSE.Parking           is not null && Parking.          Equals(EVSE.Parking)))           &&

               Connectors.              Count().Equals(EVSE.Connectors.              Count()) &&
               StatusSchedule.          Count().Equals(EVSE.StatusSchedule.          Count()) &&
               Capabilities.            Count().Equals(EVSE.Capabilities.            Count()) &&
               Directions.              Count().Equals(EVSE.Directions.              Count()) &&
               Images.                  Count().Equals(EVSE.Images.                  Count()) &&
               AcceptedServiceProviders.Count().Equals(EVSE.AcceptedServiceProviders.Count()) &&

               Connectors.              All(connector               => EVSE.Connectors.              Contains(connector))          &&
               StatusSchedule.          All(statusSchedule          => EVSE.StatusSchedule.          Contains(statusSchedule))     &&
               Capabilities.            All(capabilityType          => EVSE.Capabilities.            Contains(capabilityType))     &&
               Directions.              All(displayText             => EVSE.Directions.              Contains(displayText))        &&
               Images.                  All(image                   => EVSE.Images.                  Contains(image))              &&
               AcceptedServiceProviders.All(acceptedServiceProvider => EVSE.AcceptedServiceProviders.Contains(acceptedServiceProvider));

        #endregion

        #endregion

        #region (override) GetHashCode()

        private Int32? cachedHashCode;

        private readonly Object hashSync = new ();

        /// <summary>
        /// Get the hash code of this object.
        /// </summary>
        public override Int32 GetHashCode()
        {

            if (cachedHashCode.HasValue)
                return cachedHashCode.Value;

            lock (hashSync)
            {

                unchecked
                {

                    cachedHashCode = UId.                     GetHashCode()       * 53 ^
                                     Status.                  GetHashCode()       * 47 ^
                                     Connectors.              CalcHashCode()      * 43 ^
                                    (EVSEId?.                 GetHashCode() ?? 0) * 41 ^
                                     StatusSchedule.          GetHashCode()       * 37 ^
                                     Capabilities.            CalcHashCode()      * 31 ^
                                    (EnergyMeter?.            GetHashCode() ?? 0) * 23 ^
                                    (FloorLevel?.             GetHashCode() ?? 0) * 29 ^
                                    (Coordinates?.            GetHashCode() ?? 0) * 19 ^
                                    (PhysicalReference?.      GetHashCode() ?? 0) * 17 ^
                                     Directions.              CalcHashCode()      * 13 ^
                                    (Parking?.                GetHashCode() ?? 0) * 11 ^
                                     Images.                  CalcHashCode()      *  7 ^
                                     AcceptedServiceProviders.CalcHashCode()      *  5 ^

                                     Created.                 GetHashCode()       *  3 ^
                                     LastUpdated.             GetHashCode();

                    return cachedHashCode.Value;

                }

            }

        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   UId,

                   EVSEId.HasValue
                       ? $" ({EVSEId.Value})"
                       : "",

                   ", ",
                   $"{Connectors.Count()} connector(s), ",

                   LastUpdated.ToISO8601()

               );

        #endregion


        #region ToBuilder(NewEVSEUId = null)

        /// <summary>
        /// Return a builder for this EVSE.
        /// </summary>
        /// <param name="NewEVSEUId">An optional new EVSE identification.</param>
        public Builder ToBuilder(EVSE_UId? NewEVSEUId = null)

            => new (

                   ParentLocation,

                   NewEVSEUId ?? UId,
                   Status,
                   Connectors,

                   EVSEId,
                   StatusSchedule,
                   Capabilities,
                   EnergyMeter,
                   FloorLevel,
                   Coordinates,
                   PhysicalReference,
                   Directions,
                   ParkingRestrictions,
                   Parking,
                   Images,
                   AcceptedServiceProviders,

                   Created,
                   LastUpdated

               );

        #endregion

        #region (class) Builder

        /// <summary>
        /// An EVSE builder.
        /// </summary>
        public class Builder
        {

            #region Properties

            /// <summary>
            /// The parent location of this EVSE.
            /// </summary>
            public Location?                        ParentLocation              { get; set; }

            /// <summary>
            /// The unique identification of the EVSE within the CPOs platform.
            /// For interoperability please make sure, that the EVSE UId has the same value as the official EVSE Id!
            /// </summary>
            [Mandatory]
            public EVSE_UId?                        UId                         { get; set; }

            /// <summary>
            /// The official unique identification of the EVSE.
            /// For interoperability please make sure, that the official EVSE Id has the same value as the internal EVSE UId!
            /// </summary>
            [Optional]
            public EVSE_Id?                         EVSEId                      { get; set; }

            /// <summary>
            /// The current status of the EVSE.
            /// </summary>
            [Mandatory]
            public StatusType?                      Status                      { get; set; }

            /// <summary>
            /// The enumeration of planned future status of the EVSE.
            /// </summary>
            [Optional]
            public HashSet<StatusSchedule>          StatusSchedule              { get; }

            /// <summary>
            /// The enumeration of functionalities that the EVSE is capable of.
            /// </summary>
            [Optional]
            public HashSet<Capability>              Capabilities                { get; }

            /// <summary>
            /// The enumeration of available connectors attached to this EVSE.
            /// </summary>
            [Mandatory]
            public HashSet<Connector>               Connectors                  { get; }

            /// <summary>
            /// The enumeration of connector identifications attached to this EVSE.
            /// </summary>
            [Optional]
            public IEnumerable<Connector_Id>        ConnectorIds
                => Connectors.Select(connector => connector.Id);

            /// <summary>
            /// The optional energy meter, e.g. for the German calibration law.
            /// </summary>
            [Optional, VendorExtension(VE.GraphDefined, VE.Eichrecht)]
            public EnergyMeter<EVSE>?               EnergyMeter                 { get; set; }

            /// <summary>
            /// The optional floor level on which the EVSE is located (in garage buildings)
            /// in the locally displayed numbering scheme.
            /// string(4)
            /// </summary>
            [Optional]
            public String?                          FloorLevel                  { get; set; }

            /// <summary>
            /// The optional geographical location of the EVSE.
            /// </summary>
            [Optional]
            public GeoCoordinate?                   Coordinates                 { get; set; }

            /// <summary>
            /// The optional number/string printed on the outside of the EVSE for visual identification.
            /// string(16)
            /// </summary>
            [Optional]
            public String?                          PhysicalReference           { get; set; }

            /// <summary>
            /// Optional multi-language human-readable directions when more detailed
            /// information on how to reach the EVSE from the location is required.
            /// </summary>
            [Optional]
            public HashSet<DisplayText>             Directions                  { get; }

            /// <summary>
            /// Optional applicable restrictions on who can charge at the EVSE, apart from those related to the vehicle type.
            /// </summary>
            [Optional]
            public HashSet<ParkingRestriction>      ParkingRestrictions        { get; }

            /// <summary>
            /// Optional references to the parking space or spaces that can be used by vehicles charging at this EVSE.
            /// </summary>
            [Optional]
            public HashSet<EVSEParking>             Parking                    { get; }

            /// <summary>
            /// Optional images related to the EVSE such as photos or logos.
            /// </summary>
            [Optional]
            public HashSet<Image>                   Images                      { get; }

            /// <summary>
            /// The optional enumeration of eMSPs offering contract-based payment options that are accepted at this EVSE.
            /// </summary>
            [Optional]
            public IEnumerable<EMSP_Id>             AcceptedServiceProviders    { get; }


            /// <summary>
            /// The timestamp when this EVSE was created.
            /// </summary>
            [Mandatory, VendorExtension(VE.GraphDefined, VE.Pagination)]
            public DateTime                         Created                     { get; set; }

            /// <summary>
            /// Timestamp when this EVSE was last updated (or created).
            /// </summary>
            [Mandatory]
            public DateTime                         LastUpdated                 { get; set; }

            #endregion

            #region Constructor(s)

            /// <summary>
            /// Create a new EVSE builder.
            /// </summary>
            /// <param name="ParentLocation">The parent location of this EVSE.</param>
            /// 
            /// <param name="UId">An unique identification of the EVSE within the CPOs platform. For interoperability please make sure, that the EVSE UId has the same value as the official EVSE Id!</param>
            /// <param name="Status">A current status of the EVSE.</param>
            /// <param name="Connectors">An enumeration of available connectors attached to this EVSE.</param>
            /// 
            /// <param name="EVSEId">The official unique identification of the EVSE. For interoperability please make sure, that the official EVSE Id has the same value as the internal EVSE UId!</param>
            /// <param name="StatusSchedule">An enumeration of planned future status of the EVSE.</param>
            /// <param name="Capabilities">An enumeration of functionalities that the EVSE is capable of.</param>
            /// <param name="EnergyMeter">An optional energy meter, e.g. for the German calibration law.</param>
            /// <param name="FloorLevel">An optional floor level on which the EVSE is located (in garage buildings) in the locally displayed numbering scheme.</param>
            /// <param name="Coordinates">An optional geographical location of the EVSE.</param>
            /// <param name="PhysicalReference">An optional number/string printed on the outside of the EVSE for visual identification.</param>
            /// <param name="Directions">An optional multi-language human-readable directions when more detailed information on how to reach the EVSE from the location is required.</param>
            /// <param name="Parking">An optional reference to the parking space or spaces that can be used by vehicles charging at this EVSE.</param>
            /// <param name="Images">An optional enumeration of images related to the EVSE such as photos or logos.</param>
            /// <param name="AcceptedServiceProviders">An enumeration of the service providers that are accepted by this EVSE.</param>
            /// 
            /// <param name="Created">The optional timestamp when this EVSE was created.</param>
            /// <param name="LastUpdated">The optional timestamp when this EVSE was last updated (or created).</param>
            internal Builder(Location?                         ParentLocation             = null,

                             EVSE_UId?                         UId                        = null,
                             StatusType?                       Status                     = null,
                             IEnumerable<Connector>?           Connectors                 = null,

                             EVSE_Id?                          EVSEId                     = null,
                             IEnumerable<StatusSchedule>?      StatusSchedule             = null,
                             IEnumerable<Capability>?          Capabilities               = null,
                             EnergyMeter<EVSE>?                EnergyMeter                = null,
                             String?                           FloorLevel                 = null,
                             GeoCoordinate?                    Coordinates                = null,
                             String?                           PhysicalReference          = null,
                             IEnumerable<DisplayText>?         Directions                 = null,
                             IEnumerable<ParkingRestriction>?  ParkingRestrictions        = null,
                             IEnumerable<EVSEParking>?         Parking                    = null,
                             IEnumerable<Image>?               Images                     = null,
                             IEnumerable<EMSP_Id>?             AcceptedServiceProviders   = null,

                             DateTime?                         Created                    = null,
                             DateTime?                         LastUpdated                = null)

            {

                this.ParentLocation            = ParentLocation;

                this.UId                       = UId;
                this.Status                    = Status;
                this.Connectors                = Connectors               is not null ? [.. Connectors]               : [];

                this.EVSEId                    = EVSEId;
                this.StatusSchedule            = StatusSchedule           is not null ? [.. StatusSchedule]           : [];
                this.Capabilities              = Capabilities             is not null ? [.. Capabilities]             : [];
                this.EnergyMeter               = EnergyMeter;
                this.FloorLevel                = FloorLevel;
                this.Coordinates               = Coordinates;
                this.PhysicalReference         = PhysicalReference;
                this.Directions                = Directions               is not null ? [.. Directions]               : [];
                this.ParkingRestrictions       = ParkingRestrictions      is not null ? [.. ParkingRestrictions]      : [];
                this.Parking                   = Parking                  is not null ? [.. Parking]                  : [];
                this.Images                    = Images                   is not null ? [.. Images]                   : [];
                this.AcceptedServiceProviders  = AcceptedServiceProviders is not null ? [.. AcceptedServiceProviders] : [];

                this.Created                   = Created     ?? LastUpdated ?? Timestamp.Now;
                this.LastUpdated               = LastUpdated ?? Created     ?? Timestamp.Now;

            }

            #endregion


            public Builder SetConnector(Connector Connector)
            {

                // EVSE.UpdateConnector(newOrUpdatedConnector);
                var newConnectors = Connectors.Where(connector => connector.Id != Connector.Id).ToHashSet();
                Connectors.Clear();

                foreach (var newConnector in newConnectors)
                    Connectors.Add(newConnector);

                Connectors.Add(Connector);

                return this;

            }


            #region ToImmutable

            /// <summary>
            /// Return an immutable version of the EVSE.
            /// </summary>
            public static implicit operator EVSE?(Builder? Builder)

                => Builder?.ToImmutable(out _);


            /// <summary>
            /// Return an immutable version of the EVSE.
            /// </summary>
            /// <param name="Warnings"></param>
            public EVSE? ToImmutable(out IEnumerable<Warning> Warnings)
            {

                var warnings = new List<Warning>();

                if (!UId.   HasValue)
                    warnings.Add(Warning.Create("The unique identification must not be null or empty!"));

                if (!Status.HasValue)
                    warnings.Add(Warning.Create("The status must not be null or empty!"));

                Warnings = warnings;

                return warnings.Count != 0

                           ? null

                           : new EVSE(

                                 UId.   Value,
                                 Status.Value,
                                 Connectors,

                                 EVSEId,
                                 StatusSchedule,
                                 Capabilities,
                                 EnergyMeter,
                                 FloorLevel,
                                 Coordinates,
                                 PhysicalReference,
                                 Directions,
                                 ParkingRestrictions,
                                 Parking,
                                 Images,
                                 AcceptedServiceProviders,

                                 Created,
                                 LastUpdated,
                                 null,

                                 ParentLocation

                             );

            }

            #endregion

        }

        #endregion


    }

}
