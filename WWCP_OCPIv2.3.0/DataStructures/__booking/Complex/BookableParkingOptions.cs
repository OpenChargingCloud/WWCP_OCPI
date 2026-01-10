/*
 * Copyright (c) 2015-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

#endregion

namespace cloud.charging.open.protocols.OCPIv2_3_0
{

    /// <summary>
    /// Bookable Parking Options.
    /// </summary>
    public class BookableParkingOptions : IEquatable<BookableParkingOptions>,
                                          IComparable<BookableParkingOptions>,
                                          IComparable
    {

        #region Properties

        /// <summary>
        /// The optional position of the EVSE relative to the parking space.
        /// </summary>
        [Optional]
        public EVSEPosition?             EVSEPosition             { get; }

        /// <summary>
        /// The enumeration of vehicle types that the parking is designed to accommodate.
        /// </summary>
        [Mandatory]
        public IEnumerable<VehicleType>  VehicleTypes             { get; }

        /// <summary>
        /// The optional format (socket/cable) of the installed connector.
        /// </summary>
        [Mandatory]
        public ConnectorFormats          Format                   { get; }

        /// <summary>
        /// The optional maximum vehicle weight that can park at the EVSE, in kilograms.
        /// A value for this field should be provided unless the value of the vehicle_types field
        /// contains no values other than PERSONAL_VEHICLE or MOTORCYCLE.
        /// </summary>
        [Optional]
        public Kilogram?                 MaxVehicleWeight         { get; }

        /// <summary>
        /// The optional maximum vehicle height  that can park at the EVSE, in centimeters.
        /// A value for this field should be provided unless the value of the vehicle_types field
        /// contains no values other than PERSONAL_VEHICLE or MOTORCYCLE.
        /// </summary>
        [Optional]
        public Meter?                    MaxVehicleHeight         { get; }

        /// <summary>
        /// The optional maximum vehicle height that can park at the EVSE, in centimeters.
        /// A value for this field should be provided unless the value of the vehicle_types field
        /// contains no values other than PERSONAL_VEHICLE or MOTORCYCLE.
        /// </summary>
        [Optional]
        public Meter?                    MaxVehicleLength         { get; }

        /// <summary>
        /// The optional maximum vehicle length that can park at the EVSE, in centimeters.
        /// A value for this field should be provided unless the value of the vehicle_types field
        /// contains no values other than PERSONAL_VEHICLE or MOTORCYCLE.
        /// </summary>
        [Optional]
        public Meter?                    MaxVehicleWidth          { get; }

        /// <summary>
        /// The optional maximum vehicle width that can park at the EVSE, in centimeters.
        /// A value for this field should be provided unless the value of the vehicle_types field
        /// contains no values other than PERSONAL_VEHICLE or MOTORCYCLE.
        /// </summary>
        [Optional]
        public Meter?                    ParkingSpaceLength       { get; }

        /// <summary>
        /// The optional width of the parking space, in centimeters.
        /// A value for this field should be provided unless the value of the vehicle_types field
        /// contains no values other than PERSONAL_VEHICLE or MOTORCYCLE.
        /// </summary>
        [Optional]
        public Meter?                    ParkingSpaceWidth        { get; }

        /// <summary>
        /// The optional indication whether vehicles loaded with dangerous substances are allowed
        /// to park at the EVSE.
        /// A value for this field should be provided unless the value of the vehicle_types field
        /// contains no values other than PERSONAL_VEHICLE or MOTORCYCLE.
        /// </summary>
        [Optional]
        public Boolean?                  DangerousGoodsAllowed    { get; }

        /// <summary>
        /// The optional indication whether a vehicle can stop, charge, and proceed without reversing
        /// into or out of a parking space.
        /// This should only be set to true if driving through is possible for all vehicle types
        /// listed in the vehicle_types field.
        /// </summary>
        [Optional]
        public Boolean?                  DriveThrough             { get; }

        /// <summary>
        /// The optional indication whether it is forbidden for vehicles of a type not listed in
        /// vehicle_types to park at the EVSE,
        /// even if they can physically park there safely.
        /// </summary>
        [Optional]
        public Boolean?                  RestrictedToType         { get; }

        /// <summary>
        /// The optional indication whether a power outlet is available to power a transport
        /// truck’s load refrigeration while the vehicle is parked.
        /// </summary>
        [Optional]
        public Boolean?                  RefrigerationOutlet      { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new Bookable Parking Options.
        /// </summary>
        /// <param name="Format">A format (socket/cable) of the installed connector.</param>
        /// <param name="VehicleTypes">An enumeration of vehicle types that the parking is designed to accommodate.</param>
        /// <param name="EVSEPosition">An optional position of the EVSE relative to the parking space.</param>
        /// <param name="MaxVehicleWeight">An optional maximum vehicle weight that can park at the EVSE, in kilograms.</param>
        /// <param name="MaxVehicleHeight">An optional maximum vehicle height that can park at the EVSE, in centimeters.</param>
        /// <param name="MaxVehicleLength">An optional maximum vehicle height that can park at the EVSE, in centimeters.</param>
        /// <param name="MaxVehicleWidth">An optional maximum vehicle length that can park at the EVSE, in centimeters.</param>
        /// <param name="ParkingSpaceLength">An optional maximum vehicle width that can park at the EVSE, in centimeters.</param>
        /// <param name="ParkingSpaceWidth">An optional width of the parking space, in centimeters.</param>
        /// <param name="DangerousGoodsAllowed">An optional indication whether vehicles loaded with dangerous substances are allowed to park at the EVSE.</param>
        /// <param name="DriveThrough">An optional indication whether a vehicle can stop, charge, and proceed without reversing into or out of a parking space.</param>
        /// <param name="RestrictedToType">An optional indication whether it is forbidden for vehicles of a type not listed in vehicle_types to park at the EVSE.</param>
        /// <param name="RefrigerationOutlet">An optional indication whether a power outlet is available to power a transport truck’s load refrigeration while the vehicle is parked.</param>
        public BookableParkingOptions(ConnectorFormats          Format,
                                      IEnumerable<VehicleType>  VehicleTypes,
                                      EVSEPosition?             EVSEPosition            = null,
                                      Kilogram?                 MaxVehicleWeight        = null,
                                      Meter?                    MaxVehicleHeight        = null,
                                      Meter?                    MaxVehicleLength        = null,
                                      Meter?                    MaxVehicleWidth         = null,
                                      Meter?                    ParkingSpaceLength      = null,
                                      Meter?                    ParkingSpaceWidth       = null,
                                      Boolean?                  DangerousGoodsAllowed   = null,
                                      Boolean?                  DriveThrough            = null,
                                      Boolean?                  RestrictedToType        = null,
                                      Boolean?                  RefrigerationOutlet     = null)
        {

            this.Format                 = Format;
            this.VehicleTypes           = VehicleTypes?.Distinct() ?? [];
            this.EVSEPosition           = EVSEPosition;
            this.MaxVehicleWeight       = MaxVehicleWeight;
            this.MaxVehicleHeight       = MaxVehicleHeight;
            this.MaxVehicleLength       = MaxVehicleLength;
            this.MaxVehicleWidth        = MaxVehicleWidth;
            this.ParkingSpaceLength     = ParkingSpaceLength;
            this.ParkingSpaceWidth      = ParkingSpaceWidth;
            this.DangerousGoodsAllowed  = DangerousGoodsAllowed;
            this.DriveThrough           = DriveThrough;
            this.RestrictedToType       = RestrictedToType;
            this.RefrigerationOutlet    = RefrigerationOutlet;

            unchecked
            {

                hashCode = this.Format.                GetHashCode()       * 41 ^
                           this.VehicleTypes.          CalcHashCode()      * 37 ^
                          (this.EVSEPosition?.         GetHashCode() ?? 0) * 31 ^
                          (this.MaxVehicleWeight?.     GetHashCode() ?? 0) * 29 ^
                          (this.MaxVehicleHeight?.     GetHashCode() ?? 0) * 23 ^
                          (this.MaxVehicleLength?.     GetHashCode() ?? 0) * 19 ^
                          (this.MaxVehicleWidth?.      GetHashCode() ?? 0) * 17 ^
                          (this.ParkingSpaceLength?.   GetHashCode() ?? 0) * 13 ^
                          (this.ParkingSpaceWidth?.    GetHashCode() ?? 0) * 11 ^
                          (this.DangerousGoodsAllowed?.GetHashCode() ?? 0) *  7 ^
                          (this.DriveThrough?.         GetHashCode() ?? 0) *  5 ^
                          (this.RestrictedToType?.     GetHashCode() ?? 0) *  3 ^
                           this.RefrigerationOutlet?.  GetHashCode() ?? 0;

            }

        }

        #endregion


        #region (static) Parse   (JSON, CustomBookableParkingOptionsParser = null)

        /// <summary>
        /// Parse the given JSON representation of Bookable Parking Options.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomBookableParkingOptionsParser">A delegate to parse custom Bookable Parking Options JSON objects.</param>
        public static BookableParkingOptions Parse(JObject                                               JSON,
                                                   CustomJObjectParserDelegate<BookableParkingOptions>?  CustomBookableParkingOptionsParser   = null)
        {

            if (TryParse(JSON,
                         out var bookableParkingOptions,
                         out var errorResponse,
                         CustomBookableParkingOptionsParser))
            {
                return bookableParkingOptions;
            }

            throw new ArgumentException("The given JSON representation of Bookable Parking Options is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out BookableParkingOptions, out ErrorResponse, CustomBookableParkingOptionsParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of Bookable Parking Options.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="BookableParkingOptions">The parsed Bookable Parking Options.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                           JSON,
                                       [NotNullWhen(true)]  out BookableParkingOptions?  BookableParkingOptions,
                                       [NotNullWhen(false)] out String?                  ErrorResponse)

            => TryParse(JSON,
                        out BookableParkingOptions,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of Bookable Parking Options.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="BookableParkingOptions">The parsed Bookable Parking Options.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomBookableParkingOptionsParser">A delegate to parse custom Bookable Parking Options JSON objects.</param>
        public static Boolean TryParse(JObject                                               JSON,
                                       [NotNullWhen(true)]  out BookableParkingOptions?      BookableParkingOptions,
                                       [NotNullWhen(false)] out String?                      ErrorResponse,
                                       CustomJObjectParserDelegate<BookableParkingOptions>?  CustomBookableParkingOptionsParser   = null)
        {

            try
            {

                BookableParkingOptions = null;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse Format                   [mandatory]

                if (!JSON.ParseMandatory("format",
                                         "connector formats",
                                         ConnectorFormatsExtensions.TryParse,
                                         out ConnectorFormats format,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse VehicleType              [mandatory]

                if (!JSON.ParseMandatoryHashSet("vehicle_types",
                                                "vehicle types",
                                                OCPIv2_3_0.VehicleType.TryParse,
                                                out HashSet<VehicleType> vehicleTypes,
                                                out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse EVSEPosition             [optional]

                if (JSON.ParseOptional("evse_position",
                                       "relative EVSE position",
                                       OCPIv2_3_0.EVSEPosition.TryParse,
                                       out EVSEPosition evsePosition,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse MaxVehicleWeight         [optional]

                if (JSON.ParseOptional("max_vehicle_weight",
                                       "max vehicle weight",
                                       Kilogram.TryParse,
                                       out Kilogram? maxVehicleWeight,
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
                                       out Meter? maxVehicleHeight,
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
                                       out Meter? maxVehicleLength,
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
                                       out Meter? maxVehicleWidth,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse ParkingSpaceLength       [optional]

                if (JSON.ParseOptional("parking_space_length",
                                       "max vehicle length",
                                       Meter.TryParseCM,
                                       out Meter? parkingSpaceLength,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse ParkingSpaceWidth        [optional]

                if (JSON.ParseOptional("parking_space_width",
                                       "max vehicle width",
                                       Meter.TryParseCM,
                                       out Meter? parkingSpaceWidth,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse DangerousGoodsAllowed    [optional]

                if (JSON.ParseOptional("dangerous_goods_allowed",
                                       "dangerous goods allowed",
                                       out Boolean? dangerousGoodsAllowed,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse DriveThrough             [optional]

                if (JSON.ParseOptional("drive_through",
                                       "drive through",
                                       out Boolean? driveThrough,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse RestrictedToType         [optional]

                if (JSON.ParseOptional("restricted_to_type",
                                       "restricted to type",
                                       out Boolean? restrictedToType,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse RefrigerationOutlet      [optional]

                if (JSON.ParseOptional("refrigeration_outlet",
                                       "refrigeration outlet",
                                       out Boolean? refrigerationOutlet,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                BookableParkingOptions = new BookableParkingOptions(
                                             format,
                                             vehicleTypes,
                                             evsePosition,
                                             maxVehicleWeight,
                                             maxVehicleHeight,
                                             maxVehicleLength,
                                             maxVehicleWidth,
                                             parkingSpaceLength,
                                             parkingSpaceWidth,
                                             dangerousGoodsAllowed,
                                             driveThrough,
                                             restrictedToType,
                                             refrigerationOutlet
                                         );


                if (CustomBookableParkingOptionsParser is not null)
                    BookableParkingOptions = CustomBookableParkingOptionsParser(JSON,
                                                                                BookableParkingOptions);

                return true;

            }
            catch (Exception e)
            {
                BookableParkingOptions  = default;
                ErrorResponse           = "The given JSON representation of Bookable Parking Options is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomBookableParkingOptionsSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomBookableParkingOptionsSerializer">A delegate to serialize custom Bookable Parking Options JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<BookableParkingOptions>? CustomBookableParkingOptionsSerializer = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("format",                    Format.ToString()),
                                 new JProperty("vehicle_types",             new JArray(VehicleTypes.Select(vehicleType => vehicleType.ToString()))),

                           EVSEPosition.         HasValue
                               ? new JProperty("evse_position",             EVSEPosition.         Value.ToString())
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

                           ParkingSpaceLength.   HasValue
                               ? new JProperty("parking_space_length",      ParkingSpaceLength.   Value.CM)
                               : null,

                           ParkingSpaceWidth.    HasValue
                               ? new JProperty("parking_space_width",       ParkingSpaceWidth.    Value.CM)
                               : null,

                           DangerousGoodsAllowed.HasValue
                               ? new JProperty("dangerous_goods_allowed",   DangerousGoodsAllowed.Value)
                               : null,

                           DriveThrough.         HasValue
                               ? new JProperty("drive_through",             DriveThrough.         Value)
                               : null,

                           RestrictedToType.     HasValue
                               ? new JProperty("restricted_to_type",        RestrictedToType.     Value)
                               : null,

                           RefrigerationOutlet.  HasValue
                               ? new JProperty("refrigeration_outlet",      RefrigerationOutlet.  Value)
                               : null

                       );

            return CustomBookableParkingOptionsSerializer is not null
                       ? CustomBookableParkingOptionsSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone these Bookable Parking Options.
        /// </summary>
        public BookableParkingOptions Clone()

            => new (
                   Format,
                   VehicleTypes.Select(vehicleType => vehicleType.Clone()),
                   EVSEPosition?.      Clone(),
                   MaxVehicleWeight?.  Clone(),
                   MaxVehicleHeight?.  Clone(),
                   MaxVehicleLength?.  Clone(),
                   MaxVehicleWidth?.   Clone(),
                   ParkingSpaceLength?.Clone(),
                   ParkingSpaceWidth?. Clone(),
                   DangerousGoodsAllowed,
                   DriveThrough,
                   RestrictedToType,
                   RefrigerationOutlet
               );

        #endregion


        #region Operator overloading

        #region Operator == (BookableParkingOptions1, BookableParkingOptions2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BookableParkingOptions1">A Bookable Parking Options.</param>
        /// <param name="BookableParkingOptions2">Another Bookable Parking Options.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (BookableParkingOptions? BookableParkingOptions1,
                                           BookableParkingOptions? BookableParkingOptions2)
        {

            if (Object.ReferenceEquals(BookableParkingOptions1, BookableParkingOptions2))
                return true;

            if (BookableParkingOptions1 is null || BookableParkingOptions2 is null)
                return false;

            return BookableParkingOptions1.Equals(BookableParkingOptions2);

        }

        #endregion

        #region Operator != (BookableParkingOptions1, BookableParkingOptions2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BookableParkingOptions1">A Bookable Parking Options.</param>
        /// <param name="BookableParkingOptions2">Another Bookable Parking Options.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (BookableParkingOptions? BookableParkingOptions1,
                                           BookableParkingOptions? BookableParkingOptions2)

            => !(BookableParkingOptions1 == BookableParkingOptions2);

        #endregion

        #region Operator <  (BookableParkingOptions1, BookableParkingOptions2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BookableParkingOptions1">A Bookable Parking Options.</param>
        /// <param name="BookableParkingOptions2">Another Bookable Parking Options.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (BookableParkingOptions? BookableParkingOptions1,
                                          BookableParkingOptions? BookableParkingOptions2)

            => BookableParkingOptions1 is null
                   ? throw new ArgumentNullException(nameof(BookableParkingOptions1), "The given Bookable Parking Options must not be null!")
                   : BookableParkingOptions1.CompareTo(BookableParkingOptions2) < 0;

        #endregion

        #region Operator <= (BookableParkingOptions1, BookableParkingOptions2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BookableParkingOptions1">A Bookable Parking Options.</param>
        /// <param name="BookableParkingOptions2">Another Bookable Parking Options.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (BookableParkingOptions? BookableParkingOptions1,
                                           BookableParkingOptions? BookableParkingOptions2)

            => !(BookableParkingOptions1 > BookableParkingOptions2);

        #endregion

        #region Operator >  (BookableParkingOptions1, BookableParkingOptions2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BookableParkingOptions1">A Bookable Parking Options.</param>
        /// <param name="BookableParkingOptions2">Another Bookable Parking Options.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (BookableParkingOptions? BookableParkingOptions1,
                                          BookableParkingOptions? BookableParkingOptions2)

            => BookableParkingOptions1 is null
                   ? throw new ArgumentNullException(nameof(BookableParkingOptions1), "The given Bookable Parking Options must not be null!")
                   : BookableParkingOptions1.CompareTo(BookableParkingOptions2) > 0;

        #endregion

        #region Operator >= (BookableParkingOptions1, BookableParkingOptions2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BookableParkingOptions1">A Bookable Parking Options.</param>
        /// <param name="BookableParkingOptions2">Another Bookable Parking Options.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (BookableParkingOptions? BookableParkingOptions1,
                                           BookableParkingOptions? BookableParkingOptions2)

            => !(BookableParkingOptions1 < BookableParkingOptions2);

        #endregion

        #endregion

        #region IComparable<BookableParkingOptions> Members

        #region CompareTo(Object)

        /// <summary>s
        /// Compares two Bookable Parking Optionss.
        /// </summary>
        /// <param name="Object">A Bookable Parking Options to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is BookableParkingOptions bookableParkingOptions
                   ? CompareTo(bookableParkingOptions)
                   : throw new ArgumentException("The given object is not Bookable Parking Options!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(BookableParkingOptions)

        /// <summary>s
        /// Compares two Bookable Parking Optionss.
        /// </summary>
        /// <param name="Object">A Bookable Parking Options to compare with.</param>
        public Int32 CompareTo(BookableParkingOptions? BookableParkingOptions)
        {

            if (BookableParkingOptions is null)
                throw new ArgumentNullException(nameof(BookableParkingOptions), "The given Bookable Parking Options must not be null!");

            var c = Format. CompareTo(BookableParkingOptions.Format);

            if (c == 0)
                c = VehicleTypes.Order().Select(vehicleType => vehicleType.ToString()).AggregateWith(",").CompareTo(
                        BookableParkingOptions.VehicleTypes.Order().Select(vehicleType => vehicleType.ToString()).AggregateWith(",")
                    );

            if (c == 0 && EVSEPosition.         HasValue && BookableParkingOptions.EVSEPosition.         HasValue)
                c = EVSEPosition.         Value.CompareTo(BookableParkingOptions.EVSEPosition.         Value);

            if (c == 0 && MaxVehicleWeight.     HasValue && BookableParkingOptions.MaxVehicleWeight.     HasValue)
                c = MaxVehicleWeight.     Value.CompareTo(BookableParkingOptions.MaxVehicleWeight.     Value);

            if (c == 0 && MaxVehicleHeight.     HasValue && BookableParkingOptions.MaxVehicleHeight.     HasValue)
                c = MaxVehicleHeight.     Value.CompareTo(BookableParkingOptions.MaxVehicleHeight.     Value);

            if (c == 0 && MaxVehicleLength.     HasValue && BookableParkingOptions.MaxVehicleLength.     HasValue)
                c = MaxVehicleLength.     Value.CompareTo(BookableParkingOptions.MaxVehicleLength.     Value);

            if (c == 0 && MaxVehicleWidth.      HasValue && BookableParkingOptions.MaxVehicleWidth.      HasValue)
                c = MaxVehicleWidth.      Value.CompareTo(BookableParkingOptions.MaxVehicleWidth.      Value);

            if (c == 0 && ParkingSpaceLength.   HasValue && BookableParkingOptions.ParkingSpaceLength.   HasValue)
                c = ParkingSpaceLength.   Value.CompareTo(BookableParkingOptions.ParkingSpaceLength.   Value);

            if (c == 0 && ParkingSpaceWidth.    HasValue && BookableParkingOptions.ParkingSpaceWidth.    HasValue)
                c = ParkingSpaceWidth.    Value.CompareTo(BookableParkingOptions.ParkingSpaceWidth.    Value);

            if (c == 0 && DangerousGoodsAllowed.HasValue && BookableParkingOptions.DangerousGoodsAllowed.HasValue)
                c = DangerousGoodsAllowed.Value.CompareTo(BookableParkingOptions.DangerousGoodsAllowed.Value);

            if (c == 0 && DriveThrough.         HasValue && BookableParkingOptions.DriveThrough.         HasValue)
                c = DriveThrough.         Value.CompareTo(BookableParkingOptions.DriveThrough.         Value);

            if (c == 0 && RestrictedToType.     HasValue && BookableParkingOptions.RestrictedToType.     HasValue)
                c = RestrictedToType.     Value.CompareTo(BookableParkingOptions.RestrictedToType.     Value);

            if (c == 0 && RefrigerationOutlet.  HasValue && BookableParkingOptions.RefrigerationOutlet.  HasValue)
                c = RefrigerationOutlet.  Value.CompareTo(BookableParkingOptions.RefrigerationOutlet.  Value);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<BookableParkingOptions> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two Bookable Parking Optionss for equality.
        /// </summary>
        /// <param name="Object">A Bookable Parking Options to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is BookableParkingOptions bookableParkingOptions &&
                   Equals(bookableParkingOptions);

        #endregion

        #region Equals(BookableParkingOptions)

        /// <summary>
        /// Compares two Bookable Parking Optionss for equality.
        /// </summary>
        /// <param name="BookableParkingOptions">A Bookable Parking Options to compare with.</param>
        public Boolean Equals(BookableParkingOptions? BookableParkingOptions)

            => BookableParkingOptions is not null &&

               Format.Equals(BookableParkingOptions.Format) &&
               VehicleTypes.Order().SequenceEqual(BookableParkingOptions.VehicleTypes.Order()) &&

            ((!EVSEPosition.         HasValue && !BookableParkingOptions.EVSEPosition.         HasValue) ||
              (EVSEPosition.         HasValue &&  BookableParkingOptions.EVSEPosition.         HasValue && EVSEPosition.         Value.Equals(BookableParkingOptions.EVSEPosition.         Value))) &&

            ((!MaxVehicleWeight.     HasValue && !BookableParkingOptions.MaxVehicleWeight.     HasValue) ||
              (MaxVehicleWeight.     HasValue &&  BookableParkingOptions.MaxVehicleWeight.     HasValue && MaxVehicleWeight.     Value.Equals(BookableParkingOptions.MaxVehicleWeight.     Value))) &&

            ((!MaxVehicleHeight.     HasValue && !BookableParkingOptions.MaxVehicleHeight.     HasValue) ||
              (MaxVehicleHeight.     HasValue &&  BookableParkingOptions.MaxVehicleHeight.     HasValue && MaxVehicleHeight.     Value.Equals(BookableParkingOptions.MaxVehicleHeight.     Value))) &&

            ((!MaxVehicleLength.     HasValue && !BookableParkingOptions.MaxVehicleLength.     HasValue) ||
              (MaxVehicleLength.     HasValue &&  BookableParkingOptions.MaxVehicleLength.     HasValue && MaxVehicleLength.     Value.Equals(BookableParkingOptions.MaxVehicleLength.     Value))) &&

            ((!MaxVehicleWidth.      HasValue && !BookableParkingOptions.MaxVehicleWidth.      HasValue) ||
              (MaxVehicleWidth.      HasValue &&  BookableParkingOptions.MaxVehicleWidth.      HasValue && MaxVehicleWidth.      Value.Equals(BookableParkingOptions.MaxVehicleWidth.      Value))) &&

            ((!ParkingSpaceLength.   HasValue && !BookableParkingOptions.ParkingSpaceLength.   HasValue) ||
              (ParkingSpaceLength.   HasValue &&  BookableParkingOptions.ParkingSpaceLength.   HasValue && ParkingSpaceLength.   Value.Equals(BookableParkingOptions.ParkingSpaceLength.   Value))) &&

            ((!ParkingSpaceWidth.    HasValue && !BookableParkingOptions.ParkingSpaceWidth.    HasValue) ||
              (ParkingSpaceWidth.    HasValue &&  BookableParkingOptions.ParkingSpaceWidth.    HasValue && ParkingSpaceWidth.    Value.Equals(BookableParkingOptions.ParkingSpaceWidth.    Value))) &&

            ((!DangerousGoodsAllowed.HasValue && !BookableParkingOptions.DangerousGoodsAllowed.HasValue) ||
              (DangerousGoodsAllowed.HasValue &&  BookableParkingOptions.DangerousGoodsAllowed.HasValue && DangerousGoodsAllowed.Value.Equals(BookableParkingOptions.DangerousGoodsAllowed.Value))) &&

            ((!DriveThrough.         HasValue && !BookableParkingOptions.DriveThrough.         HasValue) ||
              (DriveThrough.         HasValue &&  BookableParkingOptions.DriveThrough.         HasValue && DriveThrough.         Value.Equals(BookableParkingOptions.DriveThrough.         Value))) &&

            ((!RestrictedToType.     HasValue && !BookableParkingOptions.RestrictedToType.     HasValue) ||
              (RestrictedToType.     HasValue &&  BookableParkingOptions.RestrictedToType.     HasValue && RestrictedToType.     Value.Equals(BookableParkingOptions.RestrictedToType.     Value))) &&

            ((!RefrigerationOutlet.  HasValue && !BookableParkingOptions.RefrigerationOutlet.  HasValue) ||
              (RefrigerationOutlet.  HasValue &&  BookableParkingOptions.RefrigerationOutlet.  HasValue && RefrigerationOutlet.  Value.Equals(BookableParkingOptions.RefrigerationOutlet.  Value)));

        #endregion

        #endregion

        #region (override) GetHashCode()

        private readonly Int32 hashCode;

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        public override Int32 GetHashCode()
            => hashCode;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   $"{Format} '{VehicleTypes.AggregateWith(", ")}'",

                   EVSEPosition.HasValue
                       ? $", EVSE position: {EVSEPosition.Value}"
                       : ""

               );

        #endregion

    }

}
