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

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv3_0
{

    /// <summary>
    /// The Electric Vehicle Supply Equipment (EVSE) is the part that controls
    /// the power supply to a single electric vehicle.
    /// </summary>
    public class EVSE : APartyIssuedObject3<EVSE_UId, ChargingStation>,
                        IEquatable<EVSE>,
                        IComparable<EVSE>,
                        IComparable,
                        IEnumerable<Connector>
    {

        #region Data

        private readonly Lock patchLock = new ();

        #endregion

        #region Properties

        /// <summary>
        /// The parent charging station of this EVSE.
        /// </summary>
        public ChargingStation?                 ParentChargingStation
        {
            get
            {
                return Parent;
            }
            internal set
            {
                Parent = value;
            }
        }

        /// <summary>
        /// The unique identification of the EVSE within the CPOs platform.
        /// For interoperability please make sure, that the internal EVSE UId has the same value as the official EVSE Id!
        /// </summary>
        [Mandatory]
        public EVSE_UId                         UId
            => base.Id;

        /// <summary>
        /// The official unique identification of the EVSE.
        /// For interoperability please make sure, that the internal EVSE UId has the same value as the official EVSE Id!
        /// </summary>
        [Optional]
        public EVSE_Id?                         EVSEId                     { get; }

        /// <summary>
        /// The optional current status of the EVSE.
        /// </summary>
        /// <remarks>Since OCPI v3.0 this is a vendor extension!</remarks>
        [Optional]
        public StatusType?                      Status                     { get; }

        /// <summary>
        /// Whether this EVSE is currently physically present, or only planned for the future, or already removed.
        /// </summary>
        [Mandatory]
        public PresenceStatus                   Presence                   { get; }

        /// <summary>
        /// The enumeration of planned future status of the EVSE.
        /// </summary>
        [Optional]
        public IEnumerable<StatusSchedule>      StatusSchedule             { get; }

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
        /// The optional number/string printed on the outside of the EVSE for visual identification.
        /// string(16)
        /// </summary>
        [Optional]
        public String?                          PhysicalReference          { get; }

        /// <summary>
        /// The description of the available parking for the EVSE.
        /// </summary>
        [Mandatory]
        public Parking                          Parking                    { get; }

        /// <summary>
        /// The optional enumeration of images related to the EVSE such as photos or logos.
        /// </summary>
        [Optional]
        public IEnumerable<Image>               Images                     { get; }

        /// <summary>
        /// The optional URL where certificates, identifiers and public keys related to the calibration
        /// of meters in this EVSE can be found.
        /// </summary>
        public URL?                             CalibrationInfoURL         { get; }

        /// <summary>
        /// The optional energy meter, e.g. for the German calibration law.
        /// </summary>
        [Optional, VendorExtension(VE.GraphDefined, VE.Eichrecht)]
        public EnergyMeter<EVSE>?               EnergyMeter                { get; }


        public JObject                          CustomData                 { get; }
        public UserDefinedDictionary            InternalData               { get; }


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
        /// The SHA256 hash of the JSON representation of this EVSE.
        /// </summary>
        public String                           ETag                       { get; private set; }

        #endregion

        #region Constructor(s)

        #region EVSE(...)

        /// <summary>
        /// Create a new EVSE.
        /// </summary>
        /// <param name="UId">An unique identification of the EVSE within the CPOs platform. For interoperability please make sure, that the internal EVSE UId has the same value as the official EVSE Id!</param>
        /// <param name="Presence">Whether this EVSE is currently physically present, or only planned for the future, or already removed.</param>
        /// <param name="Connectors">An enumeration of available connectors attached to this EVSE.</param>
        /// <param name="Parking">The description of the available parking for the EVSE.</param>
        /// 
        /// <param name="EVSEId">The official unique identification of the EVSE. For interoperability please make sure, that the internal EVSE UId has the same value as the official EVSE Id!</param>
        /// <param name="Status">The optional current status of the EVSE. Since OCPI v3.0 this is a vendor extension!</param>
        /// <param name="StatusSchedule">An enumeration of planned future status of the EVSE.</param>
        /// <param name="PhysicalReference">An optional number/string printed on the outside of the EVSE for visual identification.</param>
        /// <param name="Images">An optional enumeration of images related to the EVSE such as photos or logos.</param>
        /// <param name="CalibrationInfoURL"></param>
        /// <param name="EnergyMeter">An optional energy meter, e.g. for the German calibration law.</param>
        /// 
        /// <param name="Created">The optional timestamp when this EVSE was created.</param>
        /// <param name="LastUpdated">The optional timestamp when this EVSE was last updated (or created).</param>
        /// 
        /// <param name="CustomEVSESerializer">A delegate to serialize custom EVSE JSON objects.</param>
        /// <param name="CustomConnectorSerializer">A delegate to serialize custom connector JSON objects.</param>
        /// <param name="CustomParkingSerializer">A delegate to serialize custom parking JSON objects.</param>
        /// <param name="CustomParkingRestrictionSerializer">A delegate to serialize custom parking restriction JSON objects.</param>
        /// <param name="CustomImageSerializer">A delegate to serialize custom image JSON objects.</param>
        /// <param name="CustomStatusScheduleSerializer">A delegate to serialize custom status schedule JSON objects.</param>
        /// <param name="CustomTransparencySoftwareStatusSerializer">A delegate to serialize custom transparency software status JSON objects.</param>
        /// <param name="CustomTransparencySoftwareSerializer">A delegate to serialize custom transparency software JSON objects.</param>
        /// <param name="CustomDisplayTextSerializer">A delegate to serialize custom multi-language text JSON objects.</param>
        /// <param name="CustomEVSEEnergyMeterSerializer">A delegate to serialize custom EVSE energy meter JSON objects.</param>
        public EVSE(EVSE_UId                                                      UId,
                    PresenceStatus                                                Presence,
                    IEnumerable<Connector>                                        Connectors,
                    Parking                                                       Parking,

                    EVSE_Id?                                                      EVSEId                                       = null,
                    StatusType?                                                   Status                                       = null,
                    IEnumerable<StatusSchedule>?                                  StatusSchedule                               = null,
                    String?                                                       PhysicalReference                            = null,
                    IEnumerable<Image>?                                           Images                                       = null,
                    URL?                                                          CalibrationInfoURL                           = null,
                    EnergyMeter<EVSE>?                                            EnergyMeter                                  = null,

                    JObject?                                                      CustomData                                   = null,
                    UserDefinedDictionary?                                        InternalData                                 = null,

                    DateTime?                                                     Created                                      = null,
                    DateTime?                                                     LastUpdated                                  = null,

                    CustomJObjectSerializerDelegate<EVSE>?                        CustomEVSESerializer                         = null,
                    CustomJObjectSerializerDelegate<Connector>?                   CustomConnectorSerializer                    = null,
                    CustomJObjectSerializerDelegate<Parking>?                     CustomParkingSerializer                      = null,
                    CustomJObjectSerializerDelegate<ParkingRestriction>?          CustomParkingRestrictionSerializer           = null,
                    CustomJObjectSerializerDelegate<Image>?                       CustomImageSerializer                        = null,
                    CustomJObjectSerializerDelegate<StatusSchedule>?              CustomStatusScheduleSerializer               = null,
                    CustomJObjectSerializerDelegate<EnergyMeter<EVSE>>?           CustomEVSEEnergyMeterSerializer              = null,
                    CustomJObjectSerializerDelegate<TransparencySoftwareStatus>?  CustomTransparencySoftwareStatusSerializer   = null,
                    CustomJObjectSerializerDelegate<TransparencySoftware>?        CustomTransparencySoftwareSerializer         = null,
                    CustomJObjectSerializerDelegate<DisplayText>?                 CustomDisplayTextSerializer                  = null)

            : this(null,
                   UId,
                   Presence,
                   Connectors,
                   Parking,

                   EVSEId,
                   Status,
                   StatusSchedule,
                   PhysicalReference,
                   Images,
                   CalibrationInfoURL,
                   EnergyMeter,

                   CustomData,
                   InternalData,

                   Created,
                   LastUpdated,

                   CustomEVSESerializer,
                   CustomConnectorSerializer,
                   CustomParkingSerializer,
                   CustomParkingRestrictionSerializer,
                   CustomImageSerializer,
                   CustomStatusScheduleSerializer,
                   CustomEVSEEnergyMeterSerializer,
                   CustomTransparencySoftwareStatusSerializer,
                   CustomTransparencySoftwareSerializer,
                   CustomDisplayTextSerializer)

            { }

        #endregion

        #region (internal) EVSE(ParentLocation, ... )

        /// <summary>
        /// Create a new EVSE.
        /// </summary>
        /// <param name="ParentChargingStation">The parent location of this EVSE.</param>
        /// 
        /// <param name="UId">An unique identification of the EVSE within the CPOs platform. For interoperability please make sure, that the internal EVSE UId has the same value as the official EVSE Id!</param>
        /// <param name="Presence">Whether this EVSE is currently physically present, or only planned for the future, or already removed.</param>
        /// <param name="Connectors">An enumeration of available connectors attached to this EVSE.</param>
        /// <param name="Parking">The description of the available parking for the EVSE.</param>
        /// 
        /// <param name="EVSEId">The official unique identification of the EVSE. For interoperability please make sure, that the internal EVSE UId has the same value as the official EVSE Id!</param>
        /// <param name="Status">The optional current status of the EVSE. Since OCPI v3.0 this is a vendor extension!</param>
        /// <param name="StatusSchedule">An enumeration of planned future status of the EVSE.</param>
        /// <param name="PhysicalReference">An optional number/string printed on the outside of the EVSE for visual identification.</param>
        /// <param name="Images">An optional enumeration of images related to the EVSE such as photos or logos.</param>
        /// <param name="CalibrationInfoURL"></param>
        /// <param name="EnergyMeter">An optional energy meter, e.g. for the German calibration law.</param>
        /// 
        /// <param name="Created">The optional timestamp when this EVSE was created.</param>
        /// <param name="LastUpdated">The optional timestamp when this EVSE was last updated (or created).</param>
        /// 
        /// <param name="CustomEVSESerializer">A delegate to serialize custom EVSE JSON objects.</param>
        /// <param name="CustomConnectorSerializer">A delegate to serialize custom connector JSON objects.</param>
        /// <param name="CustomParkingSerializer">A delegate to serialize custom parking JSON objects.</param>
        /// <param name="CustomStatusScheduleSerializer">A delegate to serialize custom status schedule JSON objects.</param>
        /// <param name="CustomImageSerializer">A delegate to serialize custom image JSON objects.</param>
        /// <param name="CustomEVSEEnergyMeterSerializer">A delegate to serialize custom EVSE energy meter JSON objects.</param>
        /// <param name="CustomTransparencySoftwareStatusSerializer">A delegate to serialize custom transparency software status JSON objects.</param>
        /// <param name="CustomTransparencySoftwareSerializer">A delegate to serialize custom transparency software JSON objects.</param>
        /// <param name="CustomDisplayTextSerializer">A delegate to serialize custom multi-language text JSON objects.</param>
        internal EVSE(ChargingStation?                                              ParentChargingStation,

                      EVSE_UId                                                      UId,
                      PresenceStatus                                                Presence,
                      IEnumerable<Connector>                                        Connectors,
                      Parking                                                       Parking,

                      EVSE_Id?                                                      EVSEId                                       = null,
                      StatusType?                                                   Status                                       = null,
                      IEnumerable<StatusSchedule>?                                  StatusSchedule                               = null,
                      String?                                                       PhysicalReference                            = null,
                      IEnumerable<Image>?                                           Images                                       = null,
                      URL?                                                          CalibrationInfoURL                           = null,
                      EnergyMeter<EVSE>?                                            EnergyMeter                                  = null,

                      JObject?                                                      CustomData                                   = null,
                      UserDefinedDictionary?                                        InternalData                                 = null,

                      DateTime?                                                     Created                                      = null,
                      DateTime?                                                     LastUpdated                                  = null,

                      CustomJObjectSerializerDelegate<EVSE>?                        CustomEVSESerializer                         = null,
                      CustomJObjectSerializerDelegate<Connector>?                   CustomConnectorSerializer                    = null,
                      CustomJObjectSerializerDelegate<Parking>?                     CustomParkingSerializer                      = null,
                      CustomJObjectSerializerDelegate<ParkingRestriction>?          CustomParkingRestrictionSerializer           = null,
                      CustomJObjectSerializerDelegate<Image>?                       CustomImageSerializer                        = null,
                      CustomJObjectSerializerDelegate<StatusSchedule>?              CustomStatusScheduleSerializer               = null,
                      CustomJObjectSerializerDelegate<EnergyMeter<EVSE>>?           CustomEVSEEnergyMeterSerializer              = null,
                      CustomJObjectSerializerDelegate<TransparencySoftwareStatus>?  CustomTransparencySoftwareStatusSerializer   = null,
                      CustomJObjectSerializerDelegate<TransparencySoftware>?        CustomTransparencySoftwareSerializer         = null,
                      CustomJObjectSerializerDelegate<DisplayText>?                 CustomDisplayTextSerializer                  = null)

            : base(ParentChargingStation,
                   UId)

        {

            this.ParentChargingStation  = ParentChargingStation;

            this.Presence               = Presence;
            this.Connectors             = Connectors?.    Distinct() ?? [];
            this.Parking                = Parking;

            this.EVSEId                 = EVSEId;
            this.Status                 = Status;
            this.StatusSchedule         = StatusSchedule?.Distinct() ?? [];
            this.PhysicalReference      = PhysicalReference?.Trim();
            this.Images                 = Images?.        Distinct() ?? [];
            this.CalibrationInfoURL     = CalibrationInfoURL;
            this.EnergyMeter            = EnergyMeter;

            this.CustomData             = CustomData                 ?? [];
            this.InternalData           = InternalData               ?? new UserDefinedDictionary();

            this.Created                = Created                    ?? LastUpdated ?? Timestamp.Now;
            this.LastUpdated            = LastUpdated                ?? Created     ?? Timestamp.Now;

            foreach (var connector in this.Connectors)
                connector.ParentEVSE = this;

            this.ETag                   = CalcSHA256Hash(
                                              CustomEVSESerializer,
                                              CustomConnectorSerializer,
                                              CustomParkingSerializer,
                                              CustomParkingRestrictionSerializer,
                                              CustomImageSerializer,
                                              CustomStatusScheduleSerializer,
                                              CustomEVSEEnergyMeterSerializer,
                                              CustomTransparencySoftwareStatusSerializer,
                                              CustomTransparencySoftwareSerializer,
                                              CustomDisplayTextSerializer
                                          );

        }

        #endregion

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

                #region Parse UId                    [optional]

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

                #region Parse Presence               [mandatory]

                if (!JSON.ParseMandatory("presence",
                                         "presence status",
                                         PresenceStatus.TryParse,
                                         out PresenceStatus Presence,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Connectors             [mandatory]

                if (!JSON.ParseMandatoryJSON<Connector, Connector_Id>("connectors",
                                                                      "connectors",
                                                                      Connector.TryParse,
                                                                      out IEnumerable<Connector> Connectors,
                                                                      out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Parking                [mandatory]

                if (!JSON.ParseMandatoryJSON("parking",
                                             "parking",
                                             OCPIv3_0.Parking.TryParse,
                                             out Parking? Parking,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion


                #region Parse EVSEId                 [optional]

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

                #region Parse Status                 [optional]

                if (JSON.ParseOptional("status",
                                       "EVSE status",
                                       StatusType.TryParse,
                                       out StatusType? Status,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse StatusSchedule         [optional]

                if (JSON.ParseOptionalJSON("status_schedule",
                                           "status schedule",
                                           OCPIv3_0.StatusSchedule.TryParse,
                                           out IEnumerable<StatusSchedule> StatusSchedule,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse PhysicalReference      [optional]

                var PhysicalReference = JSON.GetString("physical_reference");

                #endregion

                #region Parse Images                 [optional]

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

                #region Parse CalibrationInfoURL     [optional]

                if (JSON.ParseOptional("calibration_info_url",
                                       "calibration info URL",
                                       URL.TryParse,
                                       out URL? CalibrationInfoURL,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse EnergyMeter            [optional]

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


                #region Parse Created                [optional, NonStandard]

                if (JSON.ParseOptional("created",
                                       "created",
                                       out DateTime? Created,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse LastUpdated            [mandatory]

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
                           Presence,
                           Connectors,
                           Parking,

                           EVSEId,
                           Status,
                           StatusSchedule,
                           PhysicalReference,
                           Images,
                           CalibrationInfoURL,
                           EnergyMeter,

                           null,
                           null,

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

        #region ToJSON(CustomEVSESerializer = null, CustomConnectorSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="IncludeCreatedTimestamp">Whether to include the created timestamp in the JSON representation.</param>
        /// <param name="IncludeExtensions">Whether to include optional data model extensions.</param>
        /// <param name="CustomEVSESerializer">A delegate to serialize custom EVSE JSON objects.</param>
        /// <param name="CustomConnectorSerializer">A delegate to serialize custom connector JSON objects.</param>
        /// <param name="CustomParkingSerializer">A delegate to serialize custom parking JSON objects.</param>
        /// <param name="CustomParkingRestrictionSerializer">A delegate to serialize custom parking restriction JSON objects.</param>
        /// <param name="CustomImageSerializer">A delegate to serialize custom image JSON objects.</param>
        /// <param name="CustomStatusScheduleSerializer">A delegate to serialize custom status schedule JSON objects.</param>
        /// <param name="CustomEVSEEnergyMeterSerializer">A delegate to serialize custom EVSE energy meter JSON objects.</param>
        /// <param name="CustomTransparencySoftwareStatusSerializer">A delegate to serialize custom transparency software status JSON objects.</param>
        /// <param name="CustomTransparencySoftwareSerializer">A delegate to serialize custom transparency software JSON objects.</param>
        /// <param name="CustomDisplayTextSerializer">A delegate to serialize custom multi-language text JSON objects.</param>
        public JObject ToJSON(Boolean                                                       IncludeCreatedTimestamp                      = true,
                              Boolean                                                       IncludeExtensions                            = true,
                              CustomJObjectSerializerDelegate<EVSE>?                        CustomEVSESerializer                         = null,
                              CustomJObjectSerializerDelegate<Connector>?                   CustomConnectorSerializer                    = null,
                              CustomJObjectSerializerDelegate<Parking>?                     CustomParkingSerializer                      = null,
                              CustomJObjectSerializerDelegate<ParkingRestriction>?          CustomParkingRestrictionSerializer           = null,
                              CustomJObjectSerializerDelegate<Image>?                       CustomImageSerializer                        = null,
                              CustomJObjectSerializerDelegate<StatusSchedule>?              CustomStatusScheduleSerializer               = null,
                              CustomJObjectSerializerDelegate<EnergyMeter<EVSE>>?           CustomEVSEEnergyMeterSerializer              = null,
                              CustomJObjectSerializerDelegate<TransparencySoftwareStatus>?  CustomTransparencySoftwareStatusSerializer   = null,
                              CustomJObjectSerializerDelegate<TransparencySoftware>?        CustomTransparencySoftwareSerializer         = null,
                              CustomJObjectSerializerDelegate<DisplayText>?                 CustomDisplayTextSerializer                  = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("uid",                    UId.                     ToString()),
                                 new JProperty("presence",               Presence.                ToString()),

                           Connectors.Any()
                               ? new JProperty("connectors",             new JArray(Connectors.    OrderBy(connector     => connector.Id).
                                                                                                   Select (connector     => connector.     ToJSON(true,
                                                                                                                                                  true,
                                                                                                                                                  CustomConnectorSerializer))))
                               : null,

                                 new JProperty("parking",                Parking .                ToJSON(CustomParkingSerializer,
                                                                                                         CustomParkingRestrictionSerializer,
                                                                                                         CustomImageSerializer)),


                           EVSEId.HasValue
                               ? new JProperty("evse_id",                EVSEId.Value.            ToString())
                               : null,

                           Status.HasValue
                               ? new JProperty("status",                 Status.                  ToString())
                               : null,

                           StatusSchedule.Any()
                               ? new JProperty("status_schedule",        new JArray(StatusSchedule.Select(statusSchedule => statusSchedule.ToJSON(CustomStatusScheduleSerializer))))
                               : null,

                           PhysicalReference.IsNotNullOrEmpty()
                               ? new JProperty("physical_reference",     PhysicalReference)
                               : null,

                           Images.Any()
                               ? new JProperty("images",                 new JArray(Images.        Select(image          => image.         ToJSON(CustomImageSerializer))))
                               : null,

                           CalibrationInfoURL.HasValue
                               ? new JProperty("calibration_info_url",   CalibrationInfoURL.Value.ToString())
                               : null,

                           EnergyMeter is not null
                               ? new JProperty("energy_meter",           EnergyMeter.             ToJSON(CustomEVSEEnergyMeterSerializer,
                                                                                                         CustomTransparencySoftwareStatusSerializer,
                                                                                                         CustomTransparencySoftwareSerializer))
                               : null,

                           IncludeCreatedTimestamp
                               ? new JProperty("created",                Created.                 ToISO8601())
                               : null,

                                 new JProperty("last_updated",           LastUpdated.             ToISO8601())

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

                   ParentChargingStation,

                   UId.                Clone(),
                   Presence.           Clone(),
                   Connectors.         Select(connector      => connector.     Clone()).ToArray(),
                   Parking.            Clone(),

                   EVSEId?.            Clone(),
                   Status?.            Clone(),
                   StatusSchedule.     Select(statusSchedule => statusSchedule.Clone()).ToArray(),
                   PhysicalReference.CloneNullableString(),
                   Images.             Select(image          => image.         Clone()).ToArray(),
                   CalibrationInfoURL?.Clone(),
                   EnergyMeter?.       Clone(),

                   CustomData,
                   InternalData,

                   Created,
                   LastUpdated

               );

        #endregion


        #region (private) TryPrivatePatch(JSON, Patch)

        private PatchResult<JObject> TryPrivatePatch(JObject  JSON,
                                                     JObject  Patch,
                                                     EventTracking_Id EventTrackingId)
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

        #region CalcSHA256Hash(CustomEVSESerializer = null, CustomConnectorSerializer = null, ...)

        /// <summary>
        /// Calculate the SHA256 hash of the JSON representation of this location in HEX.
        /// </summary>
        /// <param name="CustomEVSESerializer">A delegate to serialize custom EVSE JSON objects.</param>
        /// <param name="CustomConnectorSerializer">A delegate to serialize custom connector JSON objects.</param>
        /// <param name="CustomParkingSerializer">A delegate to serialize custom parking JSON objects.</param>
        /// <param name="CustomParkingRestrictionSerializer">A delegate to serialize custom parking restriction JSON objects.</param>
        /// <param name="CustomImageSerializer">A delegate to serialize custom image JSON objects.</param>
        /// <param name="CustomStatusScheduleSerializer">A delegate to serialize custom status schedule JSON objects.</param>
        /// <param name="CustomEVSEEnergyMeterSerializer">A delegate to serialize custom EVSE energy meter JSON objects.</param>
        /// <param name="CustomTransparencySoftwareStatusSerializer">A delegate to serialize custom transparency software status JSON objects.</param>
        /// <param name="CustomTransparencySoftwareSerializer">A delegate to serialize custom transparency software JSON objects.</param>
        /// <param name="CustomDisplayTextSerializer">A delegate to serialize custom multi-language text JSON objects.</param>
        public String CalcSHA256Hash(CustomJObjectSerializerDelegate<EVSE>?                        CustomEVSESerializer                         = null,
                                     CustomJObjectSerializerDelegate<Connector>?                   CustomConnectorSerializer                    = null,
                                     CustomJObjectSerializerDelegate<Parking>?                     CustomParkingSerializer                      = null,
                                     CustomJObjectSerializerDelegate<ParkingRestriction>?          CustomParkingRestrictionSerializer           = null,
                                     CustomJObjectSerializerDelegate<Image>?                       CustomImageSerializer                        = null,
                                     CustomJObjectSerializerDelegate<StatusSchedule>?              CustomStatusScheduleSerializer               = null,
                                     CustomJObjectSerializerDelegate<EnergyMeter<EVSE>>?           CustomEVSEEnergyMeterSerializer              = null,
                                     CustomJObjectSerializerDelegate<TransparencySoftwareStatus>?  CustomTransparencySoftwareStatusSerializer   = null,
                                     CustomJObjectSerializerDelegate<TransparencySoftware>?        CustomTransparencySoftwareSerializer         = null,
                                     CustomJObjectSerializerDelegate<DisplayText>?                 CustomDisplayTextSerializer                  = null)
        {

            ETag = SHA256.HashData(
                       ToJSON(
                           true,
                           true,
                           CustomEVSESerializer,
                           CustomConnectorSerializer,
                           CustomParkingSerializer,
                           CustomParkingRestrictionSerializer,
                           CustomImageSerializer,
                           CustomStatusScheduleSerializer,
                           CustomEVSEEnergyMeterSerializer,
                           CustomTransparencySoftwareStatusSerializer,
                           CustomTransparencySoftwareSerializer,
                           CustomDisplayTextSerializer
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

            //if (c == 0)
            //    c = Status.     CompareTo(EVSE.Status);

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

            //((!Coordinates.      HasValue    && !EVSE.Coordinates.      HasValue)    ||
            //  (Coordinates.      HasValue    &&  EVSE.Coordinates.      HasValue    && Coordinates.Value.Equals(EVSE.Coordinates.Value))) &&

            // ((EnergyMeter       is     null &&  EVSE.EnergyMeter       is     null) ||
            //  (EnergyMeter       is not null &&  EVSE.EnergyMeter       is not null && EnergyMeter.      Equals(EVSE.EnergyMeter)))       &&

            // ((FloorLevel        is     null &&  EVSE.FloorLevel        is     null) ||
            //  (FloorLevel        is not null &&  EVSE.FloorLevel        is not null && FloorLevel.       Equals(EVSE.FloorLevel)))        &&

            // ((PhysicalReference is     null &&  EVSE.PhysicalReference is     null) ||
            //  (PhysicalReference is not null &&  EVSE.PhysicalReference is not null && PhysicalReference.Equals(EVSE.PhysicalReference))) &&

            //   Connectors.         Count().Equals(EVSE.Connectors.         Count()) &&
            //   StatusSchedule.     Count().Equals(EVSE.StatusSchedule.     Count()) &&
            //   Capabilities.       Count().Equals(EVSE.Capabilities.       Count()) &&
            //   Directions.         Count().Equals(EVSE.Directions.         Count()) &&
            //   ParkingRestrictions.Count().Equals(EVSE.ParkingRestrictions.Count()) &&
            //   Images.             Count().Equals(EVSE.Images.             Count()) &&

            //   Connectors.         All(connector          => EVSE.Connectors.         Contains(connector))          &&
            //   StatusSchedule.     All(statusSchedule     => EVSE.StatusSchedule.     Contains(statusSchedule))     &&
            //   Capabilities.       All(capabilityType     => EVSE.Capabilities.       Contains(capabilityType))     &&
            //   Directions.         All(displayText        => EVSE.Directions.         Contains(displayText))        &&
            //   ParkingRestrictions.All(parkingRestriction => EVSE.ParkingRestrictions.Contains(parkingRestriction)) &&
               Images.             All(image              => EVSE.Images.             Contains(image));

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

                    cachedHashCode = UId.                GetHashCode()       * 43 ^
                                     Status.             GetHashCode()       * 41 ^
                                     Connectors.         CalcHashCode()      * 37 ^
                                    (EVSEId?.            GetHashCode() ?? 0) * 31 ^
                                     StatusSchedule.     GetHashCode()       * 29 ^
                                    // Capabilities.       CalcHashCode()      * 23 ^
                                    //(EnergyMeter?.       GetHashCode() ?? 0) * 19 ^
                                    //(FloorLevel?.        GetHashCode() ?? 0) * 17 ^
                                    //(Coordinates?.       GetHashCode() ?? 0) * 13 ^
                                    //(PhysicalReference?. GetHashCode() ?? 0) * 11 ^
                                    // Directions.         CalcHashCode()      *  7 ^
                                    // ParkingRestrictions.CalcHashCode()      *  5 ^
                                     Images.             CalcHashCode()      *  3 ^
                                     LastUpdated.        GetHashCode();

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
                       ? " (" + EVSEId.Value + ")"
                       : "",

                   ", ",
                   Connectors.Count(), " connector(s), ",

                   LastUpdated.ToISO8601()

               );

        #endregion


        #region ToBuilder(NewEVSEUId = null, NewVersionId = null)

        /// <summary>
        /// Return a builder for this EVSE.
        /// </summary>
        /// <param name="NewEVSEUId">An optional new EVSE identification.</param>
        /// <param name="NewVersionId">An optional new version identification.</param>
        public Builder ToBuilder(EVSE_UId?  NewEVSEUId     = null,
                                 UInt64?    NewVersionId   = null)

            => new (

                   ParentChargingStation,
                   //PartyId,
                   NewEVSEUId ?? UId,
                   //VersionId,

                   Presence,
                   Connectors,
                   Parking,

                   EVSEId,
                   Status,
                   StatusSchedule,
                   PhysicalReference,
                   Images,
                   CalibrationInfoURL,
                   EnergyMeter,

                   CustomData,
                   InternalData,

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
            /// The parent charging station of this EVSE.
            /// </summary>
            public ChargingStation?                 ParentChargingStation      { get; set; }

            /// <summary>
            /// The unique identification of the EVSE within the CPOs platform.
            /// For interoperability please make sure, that the internal EVSE UId has the same value as the official EVSE Id!
            /// </summary>
            [Mandatory]
            public EVSE_UId?                        UId                        { get; set; }

            /// <summary>
            /// The official unique identification of the EVSE.
            /// For interoperability please make sure, that the internal EVSE UId has the same value as the official EVSE Id!
            /// </summary>
            [Optional]
            public EVSE_Id?                         EVSEId                     { get; set; }

            /// <summary>
            /// The optional current status of the EVSE.
            /// </summary>
            /// <remarks>Since OCPI v3.0 this is a vendor extension!</remarks>
            [Optional]
            public StatusType?                      Status                     { get; set; }

            /// <summary>
            /// Whether this EVSE is currently physically present, or only planned for the future, or already removed.
            /// </summary>
            [Mandatory]
            public PresenceStatus?                  Presence                   { get; set; }

            /// <summary>
            /// The enumeration of planned future status of the EVSE.
            /// </summary>
            [Optional]
            public HashSet<StatusSchedule>          StatusSchedule             { get; }

            /// <summary>
            /// The enumeration of available connectors attached to this EVSE.
            /// </summary>
            [Mandatory]
            public HashSet<Connector>               Connectors                 { get; }

            /// <summary>
            /// The enumeration of connector identifications attached to this EVSE.
            /// </summary>
            [Optional]
            public IEnumerable<Connector_Id>        ConnectorIds
                => Connectors.Select(connector => connector.Id);

            /// <summary>
            /// The optional number/string printed on the outside of the EVSE for visual identification.
            /// string(16)
            /// </summary>
            [Optional]
            public String?                          PhysicalReference          { get; set; }

            /// <summary>
            /// The description of the available parking for the EVSE.
            /// </summary>
            [Mandatory]
            public Parking?                         Parking                    { get; set; }

            /// <summary>
            /// The optional enumeration of images related to the EVSE such as photos or logos.
            /// </summary>
            [Optional]
            public HashSet<Image>                   Images                     { get; }

            /// <summary>
            /// The optional URL where certificates, identifiers and public keys related to the calibration
            /// of meters in this EVSE can be found.
            /// </summary>
            public URL?                             CalibrationInfoURL         { get; set; }

            /// <summary>
            /// The optional energy meter, e.g. for the German calibration law.
            /// </summary>
            [Optional, VendorExtension(VE.GraphDefined, VE.Eichrecht)]
            public EnergyMeter<EVSE>?               EnergyMeter                { get; set; }


            public JObject                          CustomData                 { get; }
            public UserDefinedDictionary            InternalData               { get; }


            /// <summary>
            /// The timestamp when this EVSE was created.
            /// </summary>
            [Mandatory, VendorExtension(VE.GraphDefined, VE.Pagination)]
            public DateTime?                        Created                    { get; set; }

            /// <summary>
            /// Timestamp when this EVSE was last updated (or created).
            /// </summary>
            [Mandatory]
            public DateTime?                        LastUpdated                { get; set; }

            #endregion

            #region Constructor(s)

            /// <summary>
            /// Create a new EVSE builder.
            /// </summary>
            /// <param name="ParentChargingStation">The parent charging station of this EVSE.</param>
            /// <param name="PartyId">The party identification of the party that issued this charging station.</param>
            /// <param name="UId">An unique identification of the EVSE within the CPOs platform. For interoperability please make sure, that the internal EVSE UId has the same value as the official EVSE Id!</param>
            /// <param name="VersionId">The version identification of the charging station.</param>
            /// 
            /// <param name="Presence">Whether this EVSE is currently physically present, or only planned for the future, or already removed.</param>
            /// <param name="Connectors">An enumeration of available connectors attached to this EVSE.</param>
            /// <param name="Parking">The description of the available parking for the EVSE.</param>
            /// 
            /// <param name="EVSEId">The official unique identification of the EVSE. For interoperability please make sure, that the internal EVSE UId has the same value as the official EVSE Id!</param>
            /// <param name="Status">The optional current status of the EVSE. Since OCPI v3.0 this is a vendor extension!</param>
            /// <param name="StatusSchedule">An enumeration of planned future status of the EVSE.</param>
            /// <param name="PhysicalReference">An optional number/string printed on the outside of the EVSE for visual identification.</param>
            /// <param name="Images">An optional enumeration of images related to the EVSE such as photos or logos.</param>
            /// <param name="CalibrationInfoURL"></param>
            /// <param name="EnergyMeter">An optional energy meter, e.g. for the German calibration law.</param>
            /// 
            /// <param name="Created">The optional timestamp when this EVSE was created.</param>
            /// <param name="LastUpdated">The optional timestamp when this EVSE was last updated (or created).</param>
            internal Builder(ChargingStation?              ParentChargingStation   = null,
                        //     Party_Idv3?                   PartyId                 = null,
                             EVSE_UId?                     UId                     = null,
                        //     UInt64?                       VersionId               = null,

                             PresenceStatus?               Presence                = null,
                             IEnumerable<Connector>?       Connectors              = null,
                             Parking?                      Parking                 = null,

                             EVSE_Id?                      EVSEId                  = null,
                             StatusType?                   Status                  = null,
                             IEnumerable<StatusSchedule>?  StatusSchedule          = null,
                             String?                       PhysicalReference       = null,
                             IEnumerable<Image>?           Images                  = null,
                             URL?                          CalibrationInfoURL      = null,
                             EnergyMeter<EVSE>?            EnergyMeter             = null,

                             JObject?                      CustomData              = null,
                             UserDefinedDictionary?        InternalData            = null,

                             DateTime?                     Created                 = null,
                             DateTime?                     LastUpdated             = null)

                //: base(PartyId,
                //       Id,
                //       VersionId)

            {

                this.ParentChargingStation  = ParentChargingStation;

                this.UId                    = UId;
                this.Presence               = Presence;
                this.Connectors             = Connectors     is not null ? [.. Connectors]     : [];
                this.Parking                = Parking;

                this.EVSEId                 = EVSEId;
                this.Status                 = Status;
                this.StatusSchedule         = StatusSchedule is not null ? [.. StatusSchedule] : [];
                this.PhysicalReference      = PhysicalReference;
                this.Images                 = Images         is not null ? [.. Images]         : [];
                this.CalibrationInfoURL     = CalibrationInfoURL;
                this.EnergyMeter            = EnergyMeter;

                this.CustomData             = CustomData   ?? [];
                this.InternalData           = InternalData ?? new UserDefinedDictionary();

                this.Created                = Created      ?? LastUpdated ?? Timestamp.Now;
                this.LastUpdated            = LastUpdated  ?? Created     ?? Timestamp.Now;

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

                if (!UId.     HasValue)
                    warnings.Add(Warning.Create("The unique identification must not be null or empty!"));

                if (!Presence.HasValue)
                    warnings.Add(Warning.Create("The presence must not be null or empty!"));

                if (Parking is null)
                    warnings.Add(Warning.Create("The parking must not be null or empty!"));

                Warnings = warnings;

                return warnings.Count != 0

                           ? null

                           : new EVSE(

                                 ParentChargingStation,

                                 UId.     Value,
                                 Presence.Value,
                                 Connectors,
                                 Parking,

                                 EVSEId,
                                 Status,
                                 StatusSchedule,
                                 PhysicalReference,
                                 Images,
                                 CalibrationInfoURL,
                                 EnergyMeter,

                                 CustomData,
                                 InternalData,

                                 Created,
                                 LastUpdated

                             );

            }

            #endregion

        }

        #endregion


    }

}
