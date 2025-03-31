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

#endregion

namespace cloud.charging.open.protocols.OCPIv2_3_0
{

    /// <summary>
    /// A charging dimension.
    /// </summary>
    public readonly struct CDRDimension : IEquatable<CDRDimension>,
                                          IComparable<CDRDimension>,
                                          IComparable
    {

        #region Properties

        /// <summary>
        /// The charging dimension type.
        /// </summary>
        [Mandatory]
        public CDRDimensionType  Type      { get; }

        /// <summary>
        /// Volume of the dimension consumed, measured according to the dimension type.
        /// </summary>
        [Mandatory]
        public Decimal           Volume    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new charging dimension.
        /// </summary>
        /// <param name="Type">The charging dimension type.</param>
        /// <param name="Volume">Volume of the dimension consumed, measured according to the dimension type.</param>
        private CDRDimension(CDRDimensionType  Type,
                             Decimal           Volume)
        {

            this.Type    = Type;
            this.Volume  = Volume;

        }

        #endregion


        #region (static) Create  (Type, Volume)

        /// <summary>
        /// Create new charging dimension.
        /// </summary>
        /// <param name="Type">The charging dimension type.</param>
        /// <param name="Volume">Volume of the dimension consumed, measured according to the dimension type.</param>
        public static CDRDimension Create(CDRDimensionType  Type,
                                          Decimal           Volume)

            => new (Type,
                    Volume);

        #endregion

        #region (static) Parse   (JSON, CustomCDRDimensionParser = null)

        /// <summary>
        /// Parse the given JSON representation of a charging dimension.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomCDRDimensionParser">A delegate to parse custom charging dimension JSON objects.</param>
        public static CDRDimension Parse(JObject                                     JSON,
                                         CustomJObjectParserDelegate<CDRDimension>?  CustomCDRDimensionParser   = null)
        {

            if (TryParse(JSON,
                         out var cdrDimension,
                         out var errorResponse,
                         CustomCDRDimensionParser))
            {
                return cdrDimension;
            }

            throw new ArgumentException("The given JSON representation of a charging dimension is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, CustomCDRDimensionParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a charging dimension.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomCDRDimensionParser">A delegate to parse custom charging dimension JSON objects.</param>
        public static CDRDimension? TryParse(JObject                                     JSON,
                                             CustomJObjectParserDelegate<CDRDimension>?  CustomCDRDimensionParser   = null)
        {

            if (TryParse(JSON,
                         out var cdrDimension,
                         out var errorResponse,
                         CustomCDRDimensionParser))
            {
                return cdrDimension;
            }

            return default;

        }

        #endregion

        #region (static) TryParse(JSON, out CDRDimension, out ErrorResponse, CustomCDRDimensionParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a charging dimension.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CDRDimension">The parsed charging dimension.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                JSON,
                                       [NotNullWhen(true)]  out CDRDimension  CDRDimension,
                                       [NotNullWhen(false)] out String?       ErrorResponse)

            => TryParse(JSON,
                        out CDRDimension,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a charging dimension.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CDRDimension">The parsed charging dimension.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomCDRDimensionParser">A delegate to parse custom charging dimension JSON objects.</param>
        public static Boolean TryParse(JObject                                     JSON,
                                       [NotNullWhen(true)]  out CDRDimension       CDRDimension,
                                       [NotNullWhen(false)] out String?            ErrorResponse,
                                       CustomJObjectParserDelegate<CDRDimension>?  CustomCDRDimensionParser   = null)
        {

            try
            {

                CDRDimension = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse Type      [mandatory]

                if (!JSON.ParseMandatory("type",
                                         "charging dimension type",
                                         CDRDimensionType.TryParse,
                                         out CDRDimensionType Type,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Volume    [mandatory]

                if (!JSON.ParseMandatory("volume",
                                         "volume of the dimension consumed",
                                         out Decimal Volume,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                CDRDimension = new CDRDimension(
                                   Type,
                                   Volume
                               );


                if (CustomCDRDimensionParser is not null)
                    CDRDimension = CustomCDRDimensionParser(JSON,
                                                            CDRDimension);

                return true;

            }
            catch (Exception e)
            {
                CDRDimension   = default;
                ErrorResponse  = "The given JSON representation of a charging dimension is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomCDRDimensionSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomCDRDimensionSerializer">A delegate to serialize custom charging dimension JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<CDRDimension>? CustomCDRDimensionSerializer = null)
        {

            var json = JSONObject.Create(
                           new JProperty("type",    Type.ToString()),
                           new JProperty("volume",  Volume)
                       );

            return CustomCDRDimensionSerializer is not null
                       ? CustomCDRDimensionSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this charging dimension.
        /// </summary>
        public CDRDimension Clone()

            => new (
                   Type.Clone(),
                   Volume
               );

        #endregion


        #region Static definitions

        #region CURRENT          (Volume)

        /// <summary>
        /// Average charging current during this ChargingPeriod: defined in A (Ampere).
        /// When negative, the current is flowing from the EV to the grid.
        /// </summary>
        public static CDRDimension CURRENT(Ampere Volume)        // Session only!

            => new (CDRDimensionType.CURRENT,
                    Volume.Value);

        #endregion

        #region ENERGY           (Volume)

        /// <summary>
        /// Total amount of energy (dis-)charged during this ChargingPeriod: defined in kWh.
        /// When negative, more energy was feed into the grid then charged into the EV.
        /// Default step_size is 1.
        /// </summary>
        public static CDRDimension ENERGY(WattHour Volume)

            => new (CDRDimensionType.ENERGY,
                    Volume.kWh);

        #endregion

        #region ENERGY_EXPORT    (Volume)

        /// <summary>
        /// Total amount of energy feed back into the grid: defined in kWh.
        /// </summary>
        public static CDRDimension ENERGY_EXPORT(WattHour Volume)   // Session only!

            => new (CDRDimensionType.ENERGY_EXPORT,
                    Volume.kWh);

        #endregion

        #region ENERGY_IMPORT    (Volume)

        /// <summary>
        /// Total amount of energy charged, defined in kWh.
        /// </summary>
        public static CDRDimension ENERGY_IMPORT(WattHour Volume)   // Session only!

            => new (CDRDimensionType.ENERGY_IMPORT,
                    Volume.kWh);

        #endregion

        #region MAX_CURRENT      (Volume)

        /// <summary>
        /// Sum of the maximum current over all phases, reached during this ChargingPeriod: defined in A (Ampere).
        /// </summary>
        public static CDRDimension MAX_CURRENT(Ampere Volume)

            => new (CDRDimensionType.MAX_CURRENT,
                    Volume.Value);

        #endregion

        #region MIN_CURRENT      (Volume)

        /// <summary>
        /// Sum of the minimum current over all phases, reached during this ChargingPeriod,
        /// when negative, current has flowed from the EV to the grid. Defined in A (Ampere).
        /// </summary>
        public static CDRDimension MIN_CURRENT(Ampere Volume)

            => new (CDRDimensionType.MIN_CURRENT,
                    Volume.Value);

        #endregion

        #region MAX_POWER        (Volume)

        /// <summary>
        /// Maximum power reached during this ChargingPeriod: defined in kW (Kilowatt).
        /// </summary>
        public static CDRDimension MAX_POWER(Watt Volume)

            => new (CDRDimensionType.MAX_POWER,
                    Volume.kW);

        #endregion

        #region MIN_POWER        (Volume)

        /// <summary>
        /// Minimum power reached during this ChargingPeriod: defined in kW (Kilowatt), when negative, the power has flowed from the EV to the grid.
        /// </summary>
        public static CDRDimension MIN_POWER(Watt Volume)

            => new (CDRDimensionType.MIN_POWER,
                    Volume.kW);

        #endregion

        #region PARKING_TIME     (Volume)

        /// <summary>
        /// Time during this ChargingPeriod not charging: defined in hours, default step_size multiplier is 1 second.
        /// </summary>
        public static CDRDimension PARKING_TIME(TimeSpan Volume)

            => new (CDRDimensionType.PARKING_TIME,
                    (Decimal) Volume.TotalHours);

        #endregion

        #region POWER            (Volume)

        /// <summary>
        /// Average power during this ChargingPeriod: defined in kW (Kilowatt). When negative, the power is flowing from the EV to the grid.
        /// </summary>
        public static CDRDimension POWER(Watt Volume)          // Session only!

            => new (CDRDimensionType.POWER,
                    Volume.kW);

        #endregion

        #region RESERVATION_TIME (Volume)

        /// <summary>
        /// Time during this ChargingPeriod Charge Point has been reserved and not yet been
        /// in use for this customer: defined in hours, default step_size multiplier is 1 second.
        /// </summary>
        public static CDRDimension RESERVATION_TIME(TimeSpan Volume)

            => new (CDRDimensionType.RESERVATION_TIME,
                    (Decimal) Volume.TotalHours);

        #endregion

        #region STATE_OF_CHARGE  (Volume)

        /// <summary>
        /// Current state of charge of the EV, in percentage, values allowed: 0 to 100. See note below.
        /// </summary>
        public static CDRDimension STATE_OF_CHARGE(Decimal Volume)            // Session only!

            => new (CDRDimensionType.STATE_OF_CHARGE,
                    Volume);

        #endregion

        #region TIME             (Volume)

        /// <summary>
        /// Time charging during this ChargingPeriod: defined in hours, default step_size multiplier is 1 second.
        /// </summary>
        public static CDRDimension TIME(TimeSpan Volume)

            => new (CDRDimensionType.TIME,
                    (Decimal) Volume.TotalHours);

        #endregion

        #endregion


        #region Operator overloading

        #region Operator == (CDRDimension1, CDRDimension2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDRDimension1">A charging dimension.</param>
        /// <param name="CDRDimension2">Another charging dimension.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (CDRDimension CDRDimension1,
                                           CDRDimension CDRDimension2)

            => CDRDimension1.Equals(CDRDimension2);

        #endregion

        #region Operator != (CDRDimension1, CDRDimension2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDRDimension1">A charging dimension.</param>
        /// <param name="CDRDimension2">Another charging dimension.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (CDRDimension CDRDimension1,
                                           CDRDimension CDRDimension2)

            => !CDRDimension1.Equals(CDRDimension2);

        #endregion

        #region Operator <  (CDRDimension1, CDRDimension2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDRDimension1">A charging dimension.</param>
        /// <param name="CDRDimension2">Another charging dimension.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (CDRDimension CDRDimension1,
                                          CDRDimension CDRDimension2)

            => CDRDimension1.CompareTo(CDRDimension2) < 0;

        #endregion

        #region Operator <= (CDRDimension1, CDRDimension2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDRDimension1">A charging dimension.</param>
        /// <param name="CDRDimension2">Another charging dimension.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (CDRDimension CDRDimension1,
                                           CDRDimension CDRDimension2)

            => CDRDimension1.CompareTo(CDRDimension2) <= 0;

        #endregion

        #region Operator >  (CDRDimension1, CDRDimension2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDRDimension1">A charging dimension.</param>
        /// <param name="CDRDimension2">Another charging dimension.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (CDRDimension CDRDimension1,
                                          CDRDimension CDRDimension2)

            => CDRDimension1.CompareTo(CDRDimension2) > 0;

        #endregion

        #region Operator >= (CDRDimension1, CDRDimension2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDRDimension1">A charging dimension.</param>
        /// <param name="CDRDimension2">Another charging dimension.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (CDRDimension CDRDimension1,
                                           CDRDimension CDRDimension2)

            => CDRDimension1.CompareTo(CDRDimension2) >= 0;

        #endregion

        #endregion

        #region IComparable<CDRDimension> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two charging dimensions.
        /// </summary>
        /// <param name="Object">A charging dimension to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is CDRDimension CDRDimension
                   ? CompareTo(CDRDimension)
                   : throw new ArgumentException("The given object is not a charging dimension!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(CDRDimension)

        /// <summary>
        /// Compares two charging dimensions.
        /// </summary>
        /// <param name="CDRDimension">A charging dimension to compare with.</param>
        public Int32 CompareTo(CDRDimension CDRDimension)

            => Type.CompareTo(CDRDimension.Type);

        #endregion

        #endregion

        #region IEquatable<CDRDimension> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two charging dimensions for equality.
        /// </summary>
        /// <param name="Object">A charging dimension to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is CDRDimension CDRDimension &&
                   Equals(CDRDimension);

        #endregion

        #region Equals(CDRDimension)

        /// <summary>
        /// Compares two charging dimensions for equality.
        /// </summary>
        /// <param name="CDRDimension">A charging dimension to compare with.</param>
        public Boolean Equals(CDRDimension CDRDimension)

            => Type.  Equals(CDRDimension.Type) &&
               Volume.Equals(CDRDimension.Volume);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return Type.  GetHashCode() * 3 ^
                       Volume.GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   Volume,
                   " ",
                   Type

               );

        #endregion

    }

}
