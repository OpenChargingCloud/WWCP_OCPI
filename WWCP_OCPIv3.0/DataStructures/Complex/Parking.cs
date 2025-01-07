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

#endregion

namespace cloud.charging.open.protocols.OCPIv3_0
{

    /// <summary>
    /// The restrictions on parking that is available for an EVSE.
    /// </summary>
    public class Parking : IEquatable<Parking>,
                           IComparable<Parking>,
                           IComparable
    {

        #region Properties

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
        /// The length of the parking bay, in centimeters. A value for this field should be provided unless the value of the
        /// vehicle_types field contains no values other than PERSONAL_VEHICLE or MOTORCYCLE.
        /// </summary>
        [Optional]
        public Meter?                           ParkingBayLength         { get; }

        /// <summary>
        /// The width of the parking bay, in centimeters. A value for this field should be provided unless the value of the
        /// vehicle_types field contains no values other than PERSONAL_VEHICLE or MOTORCYCLE.
        /// </summary>
        [Optional]
        public Meter?                           ParkingBayWidth          { get; }

        /// <summary>
        /// Whether vehicles loaded with dangerous substances are allowed to park at the EVSE.A value for this field should
        /// be provided unless the value of the vehicle_types field contains no values other than PERSONAL_VEHICLE or MOTORCYCLE.
        /// </summary>
        [Optional]
        public Boolean?                         DangerousGoodsAllowed    { get; }

        /// <summary>
        /// The position of the EVSE relative to the parking space.
        /// </summary>
        [Mandatory]
        public EVSEPosition                     EVSEPosition             { get; }

        /// <summary>
        /// The direction in which the vehicle is to be parked next to this EVSE.
        /// </summary>
        [Mandatory]
        public ParkingDirection                 Direction                { get; }

        /// <summary>
        /// Whether it is forbidden for vehicles of a type not listed in vehicle_types to park at this EVSE,
        /// even if they can physically park there safely.
        /// </summary>
        [Mandatory]
        public Boolean                          RestrictedToType         { get; }

        /// <summary>
        /// All applicable restrictions on who can park at this EVSE, apart from those related to the vehicle type.
        /// </summary>
        [Optional]
        public IEnumerable<ParkingRestriction>  ParkingRestrictions      { get; }

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
        /// A list of standards that the parking space conforms to, e.g.PAS 1899 for parking for people with disabilities.
        /// </summary>
        [Optional]
        public IEnumerable<Standard>            Standards                  { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new parking.
        /// </summary>
        /// <param name="VehicleTypes">The vehicle types that the EVSE is intended for and that the associated parking is designed to accomodate.</param>
        /// <param name="EVSEPosition">The position of the EVSE relative to the parking space.</param>
        /// <param name="Direction">The direction in which the vehicle is to be parked next to this EVSE.</param>
        /// <param name="RestrictedToType">Whether it is forbidden for vehicles of a type not listed in vehicle_types to park at this EVSE, even if they can physically park there safely.</param>
        /// <param name="ReservationRequired">Whether a reservation is required for parking at the EVSE.</param>
        /// 
        /// <param name="MaxVehicleWeight">The maximum vehicle weight that can park at the EVSE, in kilograms.A value for this field should be provided unless the value of the vehicle_types field contains no values other than PERSONAL_VEHICLE or MOTORCYCLE.</param>
        /// <param name="MaxVehicleHeight">The maximum vehicle height that can park at the EVSE, in centimeters.A value for this field should be provided unless the value of the vehicle_types field contains no values other than PERSONAL_VEHICLE or MOTORCYCLE.</param>
        /// <param name="MaxVehicleLength">The maximum vehicle length that can park at the EVSE, in centimeters.A value for this field should be provided unless the value of the vehicle_types field contains no values other than PERSONAL_VEHICLE or MOTORCYCLE.</param>
        /// <param name="MaxVehicleWidth">The maximum vehicle width that can park at the EVSE, in centimeters.A value for this field should be provided unless the value of the vehicle_types field contains no values other than PERSONAL_VEHICLE or MOTORCYCLE.</param>
        /// <param name="ParkingBayLength">The length of the parking bay, in centimeters. A value for this field should be provided unless the value of the vehicle_types field contains no values other than PERSONAL_VEHICLE or MOTORCYCLE.</param>
        /// <param name="ParkingBayWidth">The width of the parking bay, in centimeters. A value for this field should be provided unless the value of the vehicle_types field contains no values other than PERSONAL_VEHICLE or MOTORCYCLE.</param>
        /// <param name="DangerousGoodsAllowed">Whether vehicles loaded with dangerous substances are allowed to park at the EVSE.A value for this field should be provided unless the value of the vehicle_types field contains no values other than PERSONAL_VEHICLE or MOTORCYCLE.</param>
        /// <param name="ParkingRestrictions">All applicable restrictions on who can park at this EVSE, apart from those related to the vehicle type.</param>
        /// <param name="TimeLimit">A time limit. If this field is present, vehicles may not park in this parking longer than this number of minutes.</param>
        /// <param name="Roofed">Whether the vehicle will be parked under a roof while charging.</param>
        /// <param name="Images">Photos of the parking space at the EVSE. At least one photograph should be provided if the value of vehicle_types includes the DISABLED vehicle type.</param>
        /// <param name="Lighting">Whether the parking space for the EVSE is lit by artificial lighting.</param>
        /// <param name="Standards">A list of standards that the parking space conforms to, e.g.PAS 1899 for parking for people with disabilities.</param>
        public Parking(IEnumerable<VehicleType>          VehicleTypes,
                       EVSEPosition                      EVSEPosition,
                       ParkingDirection                  Direction,
                       Boolean                           RestrictedToType,
                       Boolean                           ReservationRequired,

                       Kilogram?                         MaxVehicleWeight        = null,
                       Meter?                            MaxVehicleHeight        = null,
                       Meter?                            MaxVehicleLength        = null,
                       Meter?                            MaxVehicleWidth         = null,
                       Meter?                            ParkingBayLength        = null,
                       Meter?                            ParkingBayWidth         = null,
                       Boolean?                          DangerousGoodsAllowed   = null,
                       IEnumerable<ParkingRestriction>?  ParkingRestrictions     = null,
                       TimeSpan?                         TimeLimit               = null,
                       Boolean?                          Roofed                  = null,
                       IEnumerable<Image>?               Images                  = null,
                       Boolean?                          Lighting                = null,
                       IEnumerable<Standard>?            Standards               = null)
        {

            this.VehicleTypes           = VehicleTypes;
            this.EVSEPosition           = EVSEPosition;
            this.Direction              = Direction;
            this.RestrictedToType       = RestrictedToType;
            this.ReservationRequired    = ReservationRequired;

            this.MaxVehicleWeight       = MaxVehicleWeight;
            this.MaxVehicleHeight       = MaxVehicleHeight;
            this.MaxVehicleLength       = MaxVehicleLength;
            this.MaxVehicleWidth        = MaxVehicleWidth;
            this.ParkingBayLength       = ParkingBayLength;
            this.ParkingBayWidth        = ParkingBayWidth;
            this.DangerousGoodsAllowed  = DangerousGoodsAllowed;
            this.ParkingRestrictions    = ParkingRestrictions?.Distinct() ?? [];
            this.TimeLimit              = TimeLimit;
            this.Roofed                 = Roofed;
            this.Images                 = Images?.             Distinct() ?? [];
            this.Lighting               = Lighting;
            this.Standards              = Standards?.          Distinct() ?? [];

            unchecked
            {

                hashCode = this.VehicleTypes.          CalcHashCode()      * 11 ^
                           this.EVSEPosition.          GetHashCode()       *  7 ^
                           this.Direction.             GetHashCode()       * 5 ^
                           this.RestrictedToType.      GetHashCode()       * 11 ^
                           this.ReservationRequired.   GetHashCode()       *  7 ^

                          (this.MaxVehicleWeight?.     GetHashCode() ?? 0) *  5 ^
                          (this.MaxVehicleHeight?.     GetHashCode() ?? 0) *  3 ^
                          (this.MaxVehicleLength?.     GetHashCode() ?? 0) *  5 ^
                          (this.MaxVehicleWidth?.      GetHashCode() ?? 0) *  3 ^
                          (this.ParkingBayLength?.     GetHashCode() ?? 0) *  5 ^
                          (this.ParkingBayWidth?.      GetHashCode() ?? 0) *  3 ^
                          (this.DangerousGoodsAllowed?.GetHashCode() ?? 0) *  5 ^
                           this.ParkingRestrictions.   CalcHashCode()      *  3 ^
                          (this.TimeLimit?.            GetHashCode() ?? 0) *  5 ^
                          (this.Roofed?.               GetHashCode() ?? 0) *  3 ^
                           this.Images.                CalcHashCode()      *  5 ^
                          (this.Lighting?.             GetHashCode() ?? 0) *  3 ^
                           this.Standards.             CalcHashCode();

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

                #region Parse EVSEPosition             [mandatory]

                if (!JSON.ParseMandatory("evse_position",
                                         "EVSE position",
                                         OCPIv3_0.EVSEPosition.TryParse,
                                         out EVSEPosition EVSEPosition,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Direction                [mandatory]

                if (!JSON.ParseMandatory("direction",
                                         "charging rate unit",
                                         OCPIv3_0.ParkingDirection.TryParse,
                                         out ParkingDirection Direction,
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
                                       Meter.TryParse,
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
                                       Meter.TryParse,
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
                                       Meter.TryParse,
                                       out Meter? MaxVehicleWidth,
                                       out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                }

                #endregion

                #region Parse ParkingBayLength         [optional]

                if (JSON.ParseOptional("parking_bay_length",
                                       "parking bay length",
                                       Meter.TryParse,
                                       out Meter? ParkingBayLength,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse ParkingBayWidth          [optional]

                if (JSON.ParseOptional("parking_bay_width",
                                       "parking bay width",
                                       Meter.TryParse,
                                       out Meter? ParkingBayWidth,
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

                #region Parse ParkingRestrictions      [optional]

                if (JSON.ParseOptionalHashSet("parking_restrictions",
                                              "parking restrictions",
                                              ParkingRestriction.TryParse,
                                              out HashSet<ParkingRestriction>? ParkingRestrictions,
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


                Parking = new Parking(

                              VehicleTypes,
                              EVSEPosition,
                              Direction,
                              RestrictedToType,
                              ReservationRequired,

                              MaxVehicleWeight,
                              MaxVehicleHeight,
                              MaxVehicleLength,
                              MaxVehicleWidth,
                              ParkingBayLength,
                              ParkingBayWidth,
                              DangerousGoodsAllowed,
                              ParkingRestrictions,
                              TimeLimit,
                              Roofed,
                              Images,
                              Lighting,
                              Standards

                          );

                if (CustomParkingParser is not null)
                    Parking = CustomParkingParser(JSON,
                                                                  Parking);

                return true;

            }
            catch (Exception e)
            {
                Parking  = default;
                ErrorResponse    = "The given JSON representation of a parking is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomParkingSerializer = null, CustomParkingRestrictionSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomParkingSerializer">A delegate to serialize custom parking JSON objects.</param>
        /// <param name="CustomParkingRestrictionSerializer">A delegate to serialize custom parking restriction JSON objects.</param>
        /// <param name="CustomImageSerializer">A delegate to serialize custom image JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<Parking>?             CustomParkingSerializer              = null,
                              CustomJObjectSerializerDelegate<ParkingRestriction>?  CustomParkingRestrictionSerializer   = null,
                              CustomJObjectSerializerDelegate<Image>?               CustomImageSerializer                = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("vehicle_types",             new JArray(VehicleTypes.Select(vehicleType => vehicleType.ToString()))),
                                 new JProperty("evse_position",             EVSEPosition.ToString()),
                                 new JProperty("direction",                 Direction.   ToString()),
                                 new JProperty("restricted_to_type",        RestrictedToType),
                                 new JProperty("reservation_required",      ReservationRequired),

                           MaxVehicleWeight.     HasValue
                               ? new JProperty("max_vehicle_weight",        MaxVehicleWeight.Value.Value)
                               : null,

                           MaxVehicleHeight.     HasValue
                               ? new JProperty("max_vehicle_height",        MaxVehicleHeight.Value.Value)
                               : null,

                           MaxVehicleLength.     HasValue
                               ? new JProperty("max_vehicle_length",        MaxVehicleLength.Value.Value)
                               : null,

                           MaxVehicleWidth.      HasValue
                               ? new JProperty("max_vehicle_width",         MaxVehicleWidth. Value.Value)
                               : null,

                           ParkingBayLength.     HasValue
                               ? new JProperty("parking_bay_length",        ParkingBayLength.Value.Value)
                               : null,

                           ParkingBayWidth.      HasValue
                               ? new JProperty("parking_bay_width",         ParkingBayWidth. Value.Value)
                               : null,

                           DangerousGoodsAllowed.HasValue
                               ? new JProperty("dangerous_goods_allowed",   DangerousGoodsAllowed.Value)
                               : null,

                           ParkingRestrictions.Any()
                               ? new JProperty("parking_restrictions",      new JArray(ParkingRestrictions.Select(parkingRestriction => parkingRestriction.ToJSON(CustomParkingRestrictionSerializer))))
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

                           Standards.Any()
                               ? new JProperty("standards",                 new JArray(Standards.Select(standard => standard.ToString())))
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

                   VehicleTypes.       Select(vehicleType => vehicleType.Clone()),
                   EVSEPosition.       Clone(),
                   Direction.          Clone(),
                   RestrictedToType,
                   ReservationRequired,

                   MaxVehicleWeight?.  Clone(),
                   MaxVehicleHeight?.  Clone(),
                   MaxVehicleLength?.  Clone(),
                   MaxVehicleWidth?.   Clone(),
                   ParkingBayLength?.  Clone(),
                   ParkingBayWidth?.   Clone(),
                   DangerousGoodsAllowed,
                   ParkingRestrictions.Select(parkingRestriction => parkingRestriction.Clone()),
                   TimeLimit,
                   Roofed,
                   Images.             Select(image => image.Clone()),
                   Lighting,
                   Standards.          Select(standard => standard.Clone())

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

            var c = VehicleTypes.Count().CompareTo(Parking.VehicleTypes.Count());
            if (c != 0) return c;

            c = EVSEPosition.CompareTo(Parking.EVSEPosition);
            if (c != 0) return c;

            c = Direction.CompareTo(Parking.Direction);
            if (c != 0) return c;

            c = RestrictedToType.CompareTo(Parking.RestrictedToType);
            if (c != 0) return c;

            c = ReservationRequired.CompareTo(Parking.ReservationRequired);
            if (c != 0) return c;

            c = MaxVehicleWeight?.CompareTo(Parking.MaxVehicleWeight) ?? (Parking.MaxVehicleWeight.HasValue ? -1 : 0);
            if (c != 0) return c;

            c = MaxVehicleHeight?.CompareTo(Parking.MaxVehicleHeight) ?? (Parking.MaxVehicleHeight.HasValue ? -1 : 0);
            if (c != 0) return c;

            c = MaxVehicleLength?.CompareTo(Parking.MaxVehicleLength) ?? (Parking.MaxVehicleLength.HasValue ? -1 : 0);
            if (c != 0) return c;

            c = MaxVehicleWidth?.CompareTo(Parking.MaxVehicleWidth) ?? (Parking.MaxVehicleWidth.HasValue ? -1 : 0);
            if (c != 0) return c;

            c = ParkingBayLength?.CompareTo(Parking.ParkingBayLength) ?? (Parking.ParkingBayLength.HasValue ? -1 : 0);
            if (c != 0) return c;

            c = ParkingBayWidth?.CompareTo(Parking.ParkingBayWidth) ?? (Parking.ParkingBayWidth.HasValue ? -1 : 0);
            if (c != 0) return c;

            c = DangerousGoodsAllowed?.CompareTo(Parking.DangerousGoodsAllowed) ?? (Parking.DangerousGoodsAllowed.HasValue ? -1 : 0);
            if (c != 0) return c;

            c = ParkingRestrictions.Count().CompareTo(Parking.ParkingRestrictions.Count());
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

               VehicleTypes.       SequenceEqual(Parking.VehicleTypes)        &&
               EVSEPosition.       Equals       (Parking.EVSEPosition)        &&
               Direction.          Equals       (Parking.Direction)           &&
               RestrictedToType.   Equals       (Parking.RestrictedToType)    &&
               ReservationRequired.Equals       (Parking.ReservationRequired) &&

              ((!MaxVehicleWeight.     HasValue && !Parking.MaxVehicleWeight.HasValue) ||
                (MaxVehicleWeight.     HasValue &&  Parking.MaxVehicleWeight.HasValue && MaxVehicleWeight.Value.Equals(Parking.MaxVehicleWeight.Value))) &&

              ((!MaxVehicleHeight.     HasValue && !Parking.MaxVehicleHeight.HasValue) ||
                (MaxVehicleHeight.     HasValue &&  Parking.MaxVehicleHeight.HasValue && MaxVehicleHeight.Value.Equals(Parking.MaxVehicleHeight.Value))) &&

              ((!MaxVehicleLength.     HasValue && !Parking.MaxVehicleLength.HasValue) ||
                (MaxVehicleLength.     HasValue &&  Parking.MaxVehicleLength.HasValue && MaxVehicleLength.Value.Equals(Parking.MaxVehicleLength.Value))) &&

              ((!MaxVehicleWidth.      HasValue  && !Parking.MaxVehicleWidth.HasValue) ||
                (MaxVehicleWidth.      HasValue  &&  Parking.MaxVehicleWidth.HasValue && MaxVehicleWidth.Value.Equals(Parking.MaxVehicleWidth.Value))) &&

              ((!ParkingBayLength.     HasValue && !Parking.ParkingBayLength.HasValue) ||
                (ParkingBayLength.     HasValue &&  Parking.ParkingBayLength.HasValue && ParkingBayLength.Value.Equals(Parking.ParkingBayLength.Value))) &&

              ((!ParkingBayWidth.      HasValue && !Parking.ParkingBayWidth.HasValue) ||
                (ParkingBayWidth.      HasValue &&  Parking.ParkingBayWidth.HasValue && ParkingBayWidth.Value.Equals(Parking.ParkingBayWidth.Value))) &&

              ((!DangerousGoodsAllowed.HasValue && !Parking.DangerousGoodsAllowed.HasValue) ||
                (DangerousGoodsAllowed.HasValue &&  Parking.DangerousGoodsAllowed.HasValue && DangerousGoodsAllowed.Value.Equals(Parking.DangerousGoodsAllowed.Value))) &&

               ParkingRestrictions.SequenceEqual(Parking.ParkingRestrictions) &&

              ((!TimeLimit.HasValue && !Parking.TimeLimit.HasValue) ||
                (TimeLimit.HasValue && Parking.TimeLimit.HasValue && TimeLimit.Value.Equals(Parking.TimeLimit.Value))) &&

              ((!Roofed.HasValue && !Parking.Roofed.HasValue) ||
                (Roofed.HasValue && Parking.Roofed.HasValue && Roofed.Value.Equals(Parking.Roofed.Value))) &&

               Images.SequenceEqual(Parking.Images) &&

              ((!Lighting.HasValue && !Parking.Lighting.HasValue) ||
                (Lighting.HasValue && Parking.Lighting.HasValue && Lighting.Value.Equals(Parking.Lighting.Value))) &&

               Standards.SequenceEqual(Parking.Standards);

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

                   VehicleTypes.AggregateWith(", "),

                   ", ", EVSEPosition

               );

        #endregion

    }

}
