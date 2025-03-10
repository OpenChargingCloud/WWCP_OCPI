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

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPI;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_3_0
{

    /// <summary>
    /// The restrictions on parking that is available for an EVSE.
    /// </summary>
    public class Parking : IHasId<Parking_Id>,
                           IEquatable<Parking>,
                           IComparable<Parking>,
                           IComparable
    {

        #region Properties

        /// <summary>
        /// The identification of the parking space within the charge point operator's platform (and suboperator platforms).
        /// </summary>
        [Mandatory]
        public Parking_Id                       Id                       { get; }

        /// <summary>
        /// A string identifier for the parking place that is physically visible on-site to drivers using the parking space.
        /// This could be a short identifier painted on the surface of a parking place in a parking garage for example.
        /// </summary>
        [Optional]
        public String?                          PhysicalReference          { get; }

        /// <summary>
        /// The vehicle types that the EVSE is intended for and that the associated parking is designed to accomodate.
        /// </summary>
        [Mandatory]
        public IEnumerable<VehicleType>         VehicleTypes             { get; }

        /// <summary>
        /// The maximum vehicle weight that can park at the EVSE, in kilograms.A value for this field should be provided
        /// unless the value of the vehicle_types field contains no values other than PERSONAL_VEHICLE or MOTORCYCLE.
        /// </summary>
        [Optional]
        public Kilogram?                        MaxVehicleWeight         { get; }

        /// <summary>
        /// The maximum vehicle height that can park at the EVSE, in centimeters.A value for this field should be provided
        /// unless the value of the vehicle_types field contains no values other than PERSONAL_VEHICLE or MOTORCYCLE.
        /// </summary>
        [Optional]
        public Meter?                           MaxVehicleHeight         { get; }

        /// <summary>
        /// The maximum vehicle length that can park at the EVSE, in centimeters.A value for this field should be provided
        /// unless the value of the vehicle_types field contains no values other than PERSONAL_VEHICLE or MOTORCYCLE.
        /// </summary>
        [Optional]
        public Meter?                           MaxVehicleLength         { get; }

        /// <summary>
        /// The maximum vehicle width that can park at the EVSE, in centimeters.A value for this field should be provided
        /// unless the value of the vehicle_types field contains no values other than PERSONAL_VEHICLE or MOTORCYCLE.
        /// </summary>
        [Optional]
        public Meter?                           MaxVehicleWidth          { get; }

        /// <summary>
        /// The length of the parking space, in centimeters. A value for this field should be provided unless the value of the
        /// vehicle_types field contains no values other than PERSONAL_VEHICLE or MOTORCYCLE.
        /// </summary>
        [Optional]
        public Meter?                           ParkingSpaceLength       { get; }

        /// <summary>
        /// The width of the parking space, in centimeters. A value for this field should be provided unless the value of the
        /// vehicle_types field contains no values other than PERSONAL_VEHICLE or MOTORCYCLE.
        /// </summary>
        [Optional]
        public Meter?                           ParkingSpaceWidth        { get; }

        /// <summary>
        /// Whether vehicles loaded with dangerous substances are allowed to park at the EVSE.A value for this field should
        /// be provided unless the value of the vehicle_types field contains no values other than PERSONAL_VEHICLE or MOTORCYCLE.
        /// </summary>
        [Optional]
        public Boolean?                         DangerousGoodsAllowed    { get; }

        /// <summary>
        /// The position of the EVSE relative to the parking space.
        /// </summary>
        [Optional]
        public EVSEPosition?                    EVSEPosition             { get; }

        /// <summary>
        /// The direction in which the vehicle is to be parked next to the EVSE.
        /// </summary>
        [Optional]
        public ParkingDirection?                Direction                { get; }

        /// <summary>
        /// The position of the EVSE relative to the parking space.
        /// </summary>
        [Optional]
        public Boolean?                         DriveThrough             { get; }

        /// <summary>
        /// Whether it is forbidden for vehicles of a type not listed in vehicle_types to park at this EVSE,
        /// even if they can physically park there safely.
        /// </summary>
        [Mandatory]
        public Boolean                          RestrictedToType         { get; }

        /// <summary>
        /// Whether a reservation is required for parking at the EVSE.
        /// </summary>
        [Mandatory]
        public Boolean                          ReservationRequired      { get; }

        /// <summary>
        /// A time limit. If this field is present, vehicles may not park in this parking longer than this number of minutes.
        /// </summary>
        [Optional]
        public TimeSpan?                        TimeLimit                { get; }

        /// <summary>
        /// Whether the vehicle will be parked under a roof while charging.
        /// </summary>
        [Optional]
        public Boolean?                         Roofed                   { get; }

        /// <summary>
        /// Photos of the parking space at the EVSE. At least one photograph should be provided if the value of
        /// vehicle_types includes the DISABLED vehicle type.
        /// </summary>
        [Optional]
        public IEnumerable<Image>               Images                     { get; }

        /// <summary>
        /// Whether the parking space for the EVSE is lit by artificial lighting.
        /// </summary>
        [Optional]
        public Boolean?                         Lighting                   { get; }

        /// <summary>
        /// Whether a power outlet is available to power a transport truck’s load refrigeration while the vehicle is parked.
        /// </summary>
        [Optional]
        public Boolean?                         RefrigerationOutlet        { get; }

        /// <summary>
        /// A list of standards that the parking space conforms to, e.g.PAS 1899 for parking for people with disabilities.
        /// </summary>
        [Optional]
        public IEnumerable<Standard>            Standards                  { get; }

        /// <summary>
        /// Reference to an Alliance for Parking Data Standards (APDS) element describing this parking.
        /// The referenced element may be a Place, Space or other hierarchy element defined by APDS.
        /// </summary>
        [Optional]
        public String?                          APDSReference              { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new parking.
        /// </summary>
        /// <param name="Id">The identification of the parking space within the charge point operator's platform (and suboperator platforms).</param>
        /// <param name="VehicleTypes">The vehicle types that the EVSE is intended for and that the associated parking is designed to accomodate.</param>
        /// <param name="RestrictedToType">Whether it is forbidden for vehicles of a type not listed in vehicle_types to park at the EVSE, even if they can physically park there safely.</param>
        /// <param name="ReservationRequired">Whether a reservation is required for parking at the EVSE.</param>
        /// 
        /// <param name="PhysicalReference">A string identifier for the parking place that is physically visible on-site to drivers using the parking space. This could be a short identifier painted on the surface of a parking place in a parking garage for example.</param>
        /// <param name="MaxVehicleWeight">The maximum vehicle weight that can park at the EVSE, in kilograms.A value for this field should be provided unless the value of the vehicle_types field contains no values other than PERSONAL_VEHICLE or MOTORCYCLE.</param>
        /// <param name="MaxVehicleHeight">The maximum vehicle height that can park at the EVSE, in centimeters.A value for this field should be provided unless the value of the vehicle_types field contains no values other than PERSONAL_VEHICLE or MOTORCYCLE.</param>
        /// <param name="MaxVehicleLength">The maximum vehicle length that can park at the EVSE, in centimeters.A value for this field should be provided unless the value of the vehicle_types field contains no values other than PERSONAL_VEHICLE or MOTORCYCLE.</param>
        /// <param name="MaxVehicleWidth">The maximum vehicle width that can park at the EVSE, in centimeters.A value for this field should be provided unless the value of the vehicle_types field contains no values other than PERSONAL_VEHICLE or MOTORCYCLE.</param>
        /// <param name="ParkingSpaceLength">The length of the parking space, in centimeters. A value for this field should be provided unless the value of the vehicle_types field contains no values other than PERSONAL_VEHICLE or MOTORCYCLE.</param>
        /// <param name="ParkingSpaceWidth">The width of the parking space, in centimeters. A value for this field should be provided unless the value of the vehicle_types field contains no values other than PERSONAL_VEHICLE or MOTORCYCLE.</param>
        /// <param name="DangerousGoodsAllowed">Whether vehicles loaded with dangerous substances are allowed to park at the EVSE.A value for this field should be provided unless the value of the vehicle_types field contains no values other than PERSONAL_VEHICLE or MOTORCYCLE.</param>
        /// <param name="EVSEPosition">The position of the EVSE relative to the parking space.</param>
        /// <param name="Direction">The direction in which the vehicle is to be parked next to the EVSE.</param>
        /// <param name="DriveThrough">The position of the EVSE relative to the parking space.</param>
        /// <param name="TimeLimit">A time limit. If this field is present, vehicles may not park in this parking longer than this number of minutes.</param>
        /// <param name="Roofed">Whether the vehicle will be parked under a roof while charging.</param>
        /// <param name="Images">Photos of the parking space at the EVSE. At least one photograph should be provided if the value of vehicle_types includes the DISABLED vehicle type.</param>
        /// <param name="Lighting">Whether the parking space for the EVSE is lit by artificial lighting.</param>
        /// <param name="RefrigerationOutlet">Whether a power outlet is available to power a transport truck’s load refrigeration while the vehicle is parked.</param>
        /// <param name="Standards">A list of standards that the parking space conforms to, e.g.PAS 1899 for parking for people with disabilities.</param>
        /// <param name="APDSReference">Reference to an Alliance for Parking Data Standards (APDS) element describing this parking. The referenced element may be a Place, Space or other hierarchy element defined by APDS.</param>
        public Parking(Parking_Id                        Id,
                       IEnumerable<VehicleType>          VehicleTypes,
                       Boolean                           RestrictedToType,
                       Boolean                           ReservationRequired,

                       String?                           PhysicalReference       = null,
                       Kilogram?                         MaxVehicleWeight        = null,
                       Meter?                            MaxVehicleHeight        = null,
                       Meter?                            MaxVehicleLength        = null,
                       Meter?                            MaxVehicleWidth         = null,
                       Meter?                            ParkingSpaceLength      = null,
                       Meter?                            ParkingSpaceWidth       = null,
                       Boolean?                          DangerousGoodsAllowed   = null,
                       EVSEPosition?                     EVSEPosition            = null,
                       ParkingDirection?                 Direction               = null,
                       Boolean?                          DriveThrough            = null,
                       TimeSpan?                         TimeLimit               = null,
                       Boolean?                          Roofed                  = null,
                       IEnumerable<Image>?               Images                  = null,
                       Boolean?                          Lighting                = null,
                       Boolean?                          RefrigerationOutlet     = null,
                       IEnumerable<Standard>?            Standards               = null,
                       String?                           APDSReference           = null)
        {

            this.Id                     = Id;
            this.VehicleTypes           = VehicleTypes;
            this.RestrictedToType       = RestrictedToType;
            this.ReservationRequired    = ReservationRequired;

            this.PhysicalReference      = PhysicalReference?.Trim();
            this.MaxVehicleWeight       = MaxVehicleWeight;
            this.MaxVehicleHeight       = MaxVehicleHeight;
            this.MaxVehicleLength       = MaxVehicleLength;
            this.MaxVehicleWidth        = MaxVehicleWidth;
            this.ParkingSpaceLength     = ParkingSpaceLength;
            this.ParkingSpaceWidth      = ParkingSpaceWidth;
            this.DangerousGoodsAllowed  = DangerousGoodsAllowed;
            this.EVSEPosition           = EVSEPosition;
            this.Direction              = Direction;
            this.DriveThrough           = DriveThrough;
            this.TimeLimit              = TimeLimit;
            this.Roofed                 = Roofed;
            this.Images                 = Images?.             Distinct() ?? [];
            this.Lighting               = Lighting;
            this.RefrigerationOutlet    = RefrigerationOutlet;
            this.Standards              = Standards?.          Distinct() ?? [];
            this.APDSReference          = APDSReference;

            unchecked
            {

                hashCode = this.Id.                    GetHashCode()       *  89 ^
                           this.VehicleTypes.          CalcHashCode()      *  83 ^
                           this.RestrictedToType.      GetHashCode()       *  79 ^
                           this.ReservationRequired.   GetHashCode()       *  73 ^

                          (this.PhysicalReference?.    GetHashCode() ?? 0) *  71 ^
                          (this.MaxVehicleWeight?.     GetHashCode() ?? 0) *  67 ^
                          (this.MaxVehicleHeight?.     GetHashCode() ?? 0) *  61 ^
                          (this.MaxVehicleLength?.     GetHashCode() ?? 0) *  59 ^
                          (this.MaxVehicleWidth?.      GetHashCode() ?? 0) *  53 ^
                          (this.ParkingSpaceLength?.   GetHashCode() ?? 0) *  47 ^
                          (this.ParkingSpaceWidth?.    GetHashCode() ?? 0) *  43 ^
                          (this.DangerousGoodsAllowed?.GetHashCode() ?? 0) *  41 ^
                          (this.EVSEPosition?.         GetHashCode() ?? 0) *  37 ^
                          (this.Direction?.            GetHashCode() ?? 0) *  31 ^
                          (this.DriveThrough?.         GetHashCode() ?? 0) *  29 ^
//                           this.ParkingRestrictions.   CalcHashCode()      *  23 ^
                          (this.TimeLimit?.            GetHashCode() ?? 0) *  19 ^
                          (this.Roofed?.               GetHashCode() ?? 0) *  13 ^
                           this.Images.                CalcHashCode()      *  11 ^
                          (this.Lighting?.             GetHashCode() ?? 0) *   7 ^
                          (this.RefrigerationOutlet?.  GetHashCode() ?? 0) *   5 ^
                           this.Standards.             CalcHashCode()      *   3 ^
                           this.APDSReference?.        GetHashCode() ?? 0;

            }

        }

        #endregion


        #region (static) Parse   (JSON, CustomParkingParser = null)

        /// <summary>
        /// Parse the given JSON representation of a parking.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomParkingParser">A delegate to parse custom parking JSON objects.</param>
        public static Parking Parse(JObject                                JSON,
                                    CustomJObjectParserDelegate<Parking>?  CustomParkingParser   = null)
        {

            if (TryParse(JSON,
                         out var parking,
                         out var errorResponse,
                         CustomParkingParser))
            {
                return parking!;
            }

            throw new ArgumentException("The given JSON representation of a parking is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out Parking, out ErrorResponse, CustomParkingParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a parking.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="Parking">The parsed parking.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                            JSON,
                                       [NotNullWhen(true)]  out Parking?  Parking,
                                       [NotNullWhen(false)] out String?   ErrorResponse)

            => TryParse(JSON,
                        out Parking,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a parking.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="Parking">The parsed parking.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomParkingParser">A delegate to parse custom parking JSON objects.</param>
        public static Boolean TryParse(JObject                                JSON,
                                       [NotNullWhen(true)]  out Parking?      Parking,
                                       [NotNullWhen(false)] out String?       ErrorResponse,
                                       CustomJObjectParserDelegate<Parking>?  CustomParkingParser   = null)
        {

            try
            {

                Parking = null;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse Id                       [mandatory]

                if (!JSON.ParseMandatory("id",
                                         "parking identification",
                                         Parking_Id.TryParse,
                                         out Parking_Id Id,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse VehicleTypes             [mandatory]

                if (!JSON.ParseMandatoryHashSet("vehicle_types",
                                                "vehicle types",
                                                VehicleType.TryParse,
                                                out HashSet<VehicleType> VehicleTypes,
                                                out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse RestrictedToType         [mandatory]

                if (!JSON.ParseMandatory("restricted_to_type",
                                         "restricted to vehicle type",
                                         out Boolean RestrictedToType,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse ReservationRequired      [mandatory]

                if (!JSON.ParseMandatory("reservation_required",
                                         "reservation required",
                                         out Boolean ReservationRequired,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                #region Parse PhysicalReference        [optional]

                var PhysicalReference = JSON["physical_reference"]?.Value<String>();

                #endregion

                #region Parse MaxVehicleWeight         [optional]

                if (JSON.ParseOptional("max_vehicle_weight",
                                       "max vehicle weight",
                                       Kilogram.TryParseKilogram,
                                       out Kilogram? MaxVehicleWeight,
                                       out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                }

                #endregion

                #region Parse MaxVehicleHeight         [optional]

                if (JSON.ParseOptional("max_vehicle_height",
                                       "max vehicle height",
                                       Meter.TryParseCM,
                                       out Meter? MaxVehicleHeight,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse MaxVehicleLength         [optional]

                if (JSON.ParseOptional("max_vehicle_length",
                                       "max vehicle length",
                                       Meter.TryParseCM,
                                       out Meter? MaxVehicleLength,
                                       out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                }

                #endregion

                #region Parse MaxVehicleWidth          [optional]

                if (JSON.ParseOptional("max_vehicle_width",
                                       "max vehicle width",
                                       Meter.TryParseCM,
                                       out Meter? MaxVehicleWidth,
                                       out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                }

                #endregion

                #region Parse ParkingSpaceLength       [optional]

                if (JSON.ParseOptional("parking_space_length",
                                       "parking space length",
                                       Meter.TryParseCM,
                                       out Meter? ParkingSpaceLength,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse ParkingSpaceWidth        [optional]

                if (JSON.ParseOptional("parking_space_width",
                                       "parking space width",
                                       Meter.TryParseCM,
                                       out Meter? ParkingSpaceWidth,
                                       out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                }

                #endregion

                #region Parse DangerousGoodsAllowed    [optional]

                if (JSON.ParseOptional("dangerous_goods_allowed",
                                       "dangerous goods allowed",
                                       out Boolean? DangerousGoodsAllowed,
                                       out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                }

                #endregion

                #region Parse EVSEPosition             [optional]

                if (JSON.ParseOptional("evse_position",
                                       "EVSE position",
                                       OCPIv2_3_0.EVSEPosition.TryParse,
                                       out EVSEPosition? EVSEPosition,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse Direction                [optional]

                if (JSON.ParseOptional("direction",
                                       "charging rate unit",
                                       ParkingDirection.TryParse,
                                       out ParkingDirection? Direction,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse DriveThrough             [optional]

                if (JSON.ParseOptional("drive_through",
                                       "drive through",
                                       out Boolean? DriveThrough,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse TimeLimit                [optional]

                if (JSON.ParseOptional("time_limit",
                                       "time limit",
                                       TimeSpanExtensions.TryParseMinutes,
                                       out TimeSpan? TimeLimit,
                                       out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                }

                #endregion

                #region Parse Roofed                   [optional]

                if (JSON.ParseOptional("roofed",
                                       "roofed",
                                       out Boolean? Roofed,
                                       out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                }

                #endregion

                #region Parse Images                   [optional]

                if (JSON.ParseOptionalHashSet("images",
                                              "images",
                                              Image.TryParse,
                                              out HashSet<Image>? Images,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse Lighting                 [optional]

                if (JSON.ParseOptional("lighting",
                                       "lighting",
                                       out Boolean? Lighting,
                                       out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                }

                #endregion

                #region Parse RefrigerationOutlet      [optional]

                if (JSON.ParseOptional("refrigeration_outlet",
                                       "refrigeration outlet",
                                       out Boolean? RefrigerationOutlet,
                                       out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                }

                #endregion

                #region Parse Standards                [optional]

                if (JSON.ParseOptionalHashSet("standards",
                                              "standards",
                                              Standard.TryParse,
                                              out HashSet<Standard>? Standards,
                                              out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                }

                #endregion

                #region Parse RefrigerationOutlet      [optional]

                var APDSReference = JSON["apds_reference"]?.Value<String>();

                #endregion


                Parking = new Parking(

                              Id,
                              VehicleTypes,
                              RestrictedToType,
                              ReservationRequired,

                              PhysicalReference,
                              MaxVehicleWeight,
                              MaxVehicleHeight,
                              MaxVehicleLength,
                              MaxVehicleWidth,
                              ParkingSpaceLength,
                              ParkingSpaceWidth,
                              DangerousGoodsAllowed,
                              EVSEPosition,
                              Direction,
                              DriveThrough,
                              TimeLimit,
                              Roofed,
                              Images,
                              Lighting,
                              RefrigerationOutlet,
                              Standards,
                              APDSReference

                          );

                if (CustomParkingParser is not null)
                    Parking = CustomParkingParser(JSON,
                                                  Parking);

                return true;

            }
            catch (Exception e)
            {
                Parking        = default;
                ErrorResponse  = "The given JSON representation of a parking is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomParkingSerializer = null, CustomImageSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomParkingSerializer">A delegate to serialize custom parking JSON objects.</param>
        /// <param name="CustomImageSerializer">A delegate to serialize custom image JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<Parking>?  CustomParkingSerializer   = null,
                              CustomJObjectSerializerDelegate<Image>?    CustomImageSerializer     = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("id",                        Id.ToString()),
                                 new JProperty("vehicle_types",             new JArray(VehicleTypes.Select(vehicleType => vehicleType.ToString()))),
                                 new JProperty("restricted_to_type",        RestrictedToType),
                                 new JProperty("reservation_required",      ReservationRequired),

                           PhysicalReference.IsNotNullOrEmpty()
                               ? new JProperty("physical_reference",        PhysicalReference)
                               : null,

                           MaxVehicleWeight.     HasValue
                               ? new JProperty("max_vehicle_weight",        MaxVehicleWeight.     Value.Value)
                               : null,

                           MaxVehicleHeight.     HasValue
                               ? new JProperty("max_vehicle_height",        MaxVehicleHeight.     Value.CM)
                               : null,

                           MaxVehicleLength.     HasValue
                               ? new JProperty("max_vehicle_length",        MaxVehicleLength.     Value.CM)
                               : null,

                           MaxVehicleWidth.      HasValue
                               ? new JProperty("max_vehicle_width",         MaxVehicleWidth.      Value.CM)
                               : null,

                           ParkingSpaceLength.     HasValue
                               ? new JProperty("parking_space_length",      ParkingSpaceLength.   Value.CM)
                               : null,

                           ParkingSpaceWidth.      HasValue
                               ? new JProperty("parking_space_width",       ParkingSpaceWidth.    Value.CM)
                               : null,

                           DangerousGoodsAllowed.HasValue
                               ? new JProperty("dangerous_goods_allowed",   DangerousGoodsAllowed.Value)
                               : null,

                           EVSEPosition.HasValue
                               ? new JProperty("evse_position",             EVSEPosition.         Value.ToString())
                               : null,

                           Direction.HasValue
                               ? new JProperty("direction",                 Direction.            Value.ToString())
                               : null,

                           DriveThrough.HasValue
                               ? new JProperty("drive_through",             DriveThrough.         Value)
                               : null,

                           TimeLimit.            HasValue
                               ? new JProperty("time_limit",                TimeLimit.Value.TotalMinutes)
                               : null,

                           Roofed.               HasValue
                               ? new JProperty("roofed",                    Roofed.Value)
                               : null,

                           Images.Any()
                               ? new JProperty("images",                    new JArray(Images.Select(image => image.ToJSON(CustomImageSerializer))))
                               : null,

                           Lighting.             HasValue
                               ? new JProperty("lighting",                  Lighting.Value)
                               : null,

                           RefrigerationOutlet.  HasValue
                               ? new JProperty("refrigeration_outlet",      RefrigerationOutlet.         Value)
                               : null,

                           Standards.Any()
                               ? new JProperty("standards",                 new JArray(Standards.Select(standard => standard.ToString())))
                               : null,

                           APDSReference.IsNotNullOrEmpty()
                               ? new JProperty("apds_reference",            APDSReference)
                               : null

                       );

            return CustomParkingSerializer is not null
                       ? CustomParkingSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this parking.
        /// </summary>
        public Parking Clone()

            => new (

                   Id.                 Clone(),
                   VehicleTypes.       Select(vehicleType        => vehicleType.       Clone()),
                   RestrictedToType,
                   ReservationRequired,

                   PhysicalReference?. CloneString(),
                   MaxVehicleWeight?.  Clone(),
                   MaxVehicleHeight?.  Clone(),
                   MaxVehicleLength?.  Clone(),
                   MaxVehicleWidth?.   Clone(),
                   ParkingSpaceLength?.  Clone(),
                   ParkingSpaceWidth?.   Clone(),
                   DangerousGoodsAllowed,
                   EVSEPosition?.      Clone(),
                   Direction?.         Clone(),
                   DriveThrough,
                   TimeLimit,
                   Roofed,
                   Images.             Select(image              => image.             Clone()),
                   Lighting,
                   RefrigerationOutlet,
                   Standards.          Select(standard           => standard.          Clone()),
                   APDSReference?.     CloneString()

               );

        #endregion


        #region Operator overloading

        #region Operator == (Parking1, Parking2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Parking1">A parking.</param>
        /// <param name="Parking2">Another parking.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Parking Parking1,
                                           Parking Parking2)

            => Parking1.Equals(Parking2);

        #endregion

        #region Operator != (Parking1, Parking2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Parking1">A parking.</param>
        /// <param name="Parking2">Another parking.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Parking Parking1,
                                           Parking Parking2)

            => !(Parking1 == Parking2);

        #endregion

        #region Operator <  (Parking1, Parking2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Parking1">A parking.</param>
        /// <param name="Parking2">Another parking.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Parking Parking1,
                                          Parking Parking2)

            => Parking1.CompareTo(Parking2) < 0;

        #endregion

        #region Operator <= (Parking1, Parking2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Parking1">A parking.</param>
        /// <param name="Parking2">Another parking.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Parking Parking1,
                                           Parking Parking2)

            => !(Parking1 > Parking2);

        #endregion

        #region Operator >  (Parking1, Parking2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Parking1">A parking.</param>
        /// <param name="Parking2">Another parking.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Parking Parking1,
                                          Parking Parking2)

            => Parking1.CompareTo(Parking2) > 0;

        #endregion

        #region Operator >= (Parking1, Parking2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Parking1">A parking.</param>
        /// <param name="Parking2">Another parking.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Parking Parking1,
                                           Parking Parking2)

            => !(Parking1 < Parking2);

        #endregion

        #endregion

        #region IComparable<Parking> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two parking objects.
        /// </summary>
        /// <param name="Object">A parking object to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Parking parking
                   ? CompareTo(parking)
                   : throw new ArgumentException("The given object is not a parking object!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(Parking)

        /// <summary>
        /// Compares two parking objects.
        /// </summary>
        /// <param name="Parking">A parking object to compare with.</param>
        public Int32 CompareTo(Parking? Parking)
        {

            if (Parking is null)
                throw new ArgumentNullException(nameof(Parking), "The given parking object must not be null!");

            var c = Id.CompareTo(Parking.Id);
            if (c != 0) return c;

            c = VehicleTypes.Count().CompareTo(Parking.VehicleTypes.Count());
            if (c != 0) return c;

            c = RestrictedToType.CompareTo(Parking.RestrictedToType);
            if (c != 0) return c;

            c = ReservationRequired.CompareTo(Parking.ReservationRequired);
            if (c != 0) return c;

            c = PhysicalReference?.CompareTo(Parking.PhysicalReference) ?? (Parking.PhysicalReference.IsNotNullOrEmpty() ? -1 : 0);
            if (c != 0) return c;

            c = MaxVehicleWeight?.CompareTo(Parking.MaxVehicleWeight) ?? (Parking.MaxVehicleWeight.HasValue ? -1 : 0);
            if (c != 0) return c;

            c = MaxVehicleHeight?.CompareTo(Parking.MaxVehicleHeight) ?? (Parking.MaxVehicleHeight.HasValue ? -1 : 0);
            if (c != 0) return c;

            c = MaxVehicleLength?.CompareTo(Parking.MaxVehicleLength) ?? (Parking.MaxVehicleLength.HasValue ? -1 : 0);
            if (c != 0) return c;

            c = MaxVehicleWidth?.CompareTo(Parking.MaxVehicleWidth) ?? (Parking.MaxVehicleWidth.HasValue ? -1 : 0);
            if (c != 0) return c;

            c = ParkingSpaceLength?.CompareTo(Parking.ParkingSpaceLength) ?? (Parking.ParkingSpaceLength.HasValue ? -1 : 0);
            if (c != 0) return c;

            c = ParkingSpaceWidth?.CompareTo(Parking.ParkingSpaceWidth) ?? (Parking.ParkingSpaceWidth.HasValue ? -1 : 0);
            if (c != 0) return c;

            c = DangerousGoodsAllowed?.CompareTo(Parking.DangerousGoodsAllowed) ?? (Parking.DangerousGoodsAllowed.HasValue ? -1 : 0);
            if (c != 0) return c;

            c = EVSEPosition?.CompareTo(Parking.EVSEPosition) ?? (Parking.EVSEPosition.HasValue ? -1 : 0);
            if (c != 0) return c;

            c = Direction?.CompareTo(Parking.Direction) ?? (Parking.Direction.HasValue ? -1 : 0);
            if (c != 0) return c;

            c = TimeLimit?.CompareTo(Parking.TimeLimit) ?? (Parking.TimeLimit.HasValue ? -1 : 0);
            if (c != 0) return c;

            c = Roofed?.CompareTo(Parking.Roofed) ?? (Parking.Roofed.HasValue ? -1 : 0);
            if (c != 0) return c;

            c = Images.Count().CompareTo(Parking.Images.Count());
            if (c != 0) return c;

            c = Lighting?.CompareTo(Parking.Lighting) ?? (Parking.Lighting.HasValue ? -1 : 0);
            if (c != 0) return c;

            c = Standards.Count().CompareTo(Parking.Standards.Count());
            if (c != 0) return c;

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<Parking> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two parking objects for equality.
        /// </summary>
        /// <param name="Object">A parking object to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Parking parking &&
                   Equals(parking);

        #endregion

        #region Equals(Parking)

        /// <summary>
        /// Compares two parkings for equality.
        /// </summary>
        /// <param name="Parking">A parking to compare with.</param>
        public Boolean Equals(Parking? Parking)

            => Parking is not null &&

               Id.                 Equals       (Parking.Id)                  &&
               VehicleTypes.       SequenceEqual(Parking.VehicleTypes)        &&
               RestrictedToType.   Equals       (Parking.RestrictedToType)    &&
               ReservationRequired.Equals       (Parking.ReservationRequired) &&

             ((PhysicalReference is     null  && Parking.PhysicalReference is     null) ||
              (PhysicalReference is not null  && Parking.PhysicalReference is not null && PhysicalReference.Equals(Parking.PhysicalReference))) &&

            ((!MaxVehicleWeight.     HasValue && !Parking.MaxVehicleWeight.HasValue) ||
              (MaxVehicleWeight.     HasValue &&  Parking.MaxVehicleWeight.HasValue && MaxVehicleWeight.Value.Equals(Parking.MaxVehicleWeight.Value))) &&

            ((!MaxVehicleHeight.     HasValue && !Parking.MaxVehicleHeight.HasValue) ||
              (MaxVehicleHeight.     HasValue &&  Parking.MaxVehicleHeight.HasValue && MaxVehicleHeight.Value.Equals(Parking.MaxVehicleHeight.Value))) &&

            ((!MaxVehicleLength.     HasValue && !Parking.MaxVehicleLength.HasValue) ||
              (MaxVehicleLength.     HasValue &&  Parking.MaxVehicleLength.HasValue && MaxVehicleLength.Value.Equals(Parking.MaxVehicleLength.Value))) &&

            ((!MaxVehicleWidth.      HasValue  && !Parking.MaxVehicleWidth.HasValue) ||
              (MaxVehicleWidth.      HasValue  &&  Parking.MaxVehicleWidth.HasValue && MaxVehicleWidth.Value.Equals(Parking.MaxVehicleWidth.Value))) &&

            ((!ParkingSpaceLength.     HasValue && !Parking.ParkingSpaceLength.HasValue) ||
              (ParkingSpaceLength.     HasValue &&  Parking.ParkingSpaceLength.HasValue && ParkingSpaceLength.Value.Equals(Parking.ParkingSpaceLength.Value))) &&

            ((!ParkingSpaceWidth.      HasValue && !Parking.ParkingSpaceWidth.HasValue) ||
              (ParkingSpaceWidth.      HasValue &&  Parking.ParkingSpaceWidth.HasValue && ParkingSpaceWidth.Value.Equals(Parking.ParkingSpaceWidth.Value))) &&

            ((!DangerousGoodsAllowed.HasValue && !Parking.DangerousGoodsAllowed.HasValue) ||
              (DangerousGoodsAllowed.HasValue &&  Parking.DangerousGoodsAllowed.HasValue && DangerousGoodsAllowed.Value.Equals(Parking.DangerousGoodsAllowed.Value))) &&

            ((!EVSEPosition.         HasValue && !Parking.EVSEPosition.         HasValue) ||
              (EVSEPosition.         HasValue &&  Parking.EVSEPosition.         HasValue && EVSEPosition.Value.Equals(Parking.EVSEPosition.Value))) &&

            ((!Direction.            HasValue && !Parking.Direction.            HasValue) ||
              (Direction.            HasValue &&  Parking.Direction.            HasValue && Direction.   Value.Equals(Parking.Direction.   Value))) &&

            ((!DriveThrough.         HasValue && !Parking.DriveThrough.         HasValue) ||
              (DriveThrough.         HasValue &&  Parking.DriveThrough.         HasValue && DriveThrough.Value.Equals(Parking.DriveThrough.Value))) &&

            ((!TimeLimit.            HasValue && !Parking.TimeLimit.            HasValue) ||
              (TimeLimit.            HasValue &&  Parking.TimeLimit.            HasValue && TimeLimit.Value.Equals(Parking.TimeLimit.Value))) &&

            ((!Roofed.               HasValue && !Parking.Roofed.               HasValue) ||
              (Roofed.               HasValue &&  Parking.Roofed.               HasValue && Roofed.Value.Equals(Parking.Roofed.Value))) &&

               Images.SequenceEqual(Parking.Images) &&

            ((!Lighting.             HasValue && !Parking.Lighting.             HasValue) ||
              (Lighting.             HasValue &&  Parking.Lighting.             HasValue && Lighting.           Value.Equals(Parking.Lighting.           Value))) &&

            ((!RefrigerationOutlet.  HasValue && !Parking.RefrigerationOutlet.  HasValue) ||
              (RefrigerationOutlet.  HasValue &&  Parking.RefrigerationOutlet.  HasValue && RefrigerationOutlet.Value.Equals(Parking.RefrigerationOutlet.Value))) &&

               Standards.SequenceEqual(Parking.Standards) &&

             ((APDSReference is     null && Parking.APDSReference is     null) ||
              (APDSReference is not null && Parking.APDSReference is not null && APDSReference.Equals(Parking.APDSReference)));

        #endregion

        #endregion

        #region (override) GetHashCode()

        private readonly Int32 hashCode;

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()
            => hashCode;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   Id,

                   PhysicalReference.IsNotNullOrEmpty()
                       ? $" ({PhysicalReference})"
                       : "",

                   ": ",

                   VehicleTypes.AggregateWith(", "),

                   ", ", EVSEPosition

               );

        #endregion

    }

}
