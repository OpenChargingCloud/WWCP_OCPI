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

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv3_0
{

    /// <summary>
    /// Location Max Power
    /// </summary>
    public readonly struct LocationMaxPower : IEquatable<LocationMaxPower>,
                                              IComparable<LocationMaxPower>,
                                              IComparable
    {

        #region Properties

        /// <summary>
        /// The maximum power or current that the Location can draw.
        /// </summary>
        [Mandatory]
        public Decimal            Value    { get; }

        /// <summary>
        /// The unit in which the maximum draw is expressed.
        /// </summary>
        [Mandatory]
        public ChargingRateUnits  Unit     { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new Location Max Power.
        /// </summary>
        /// <param name="Value">The maximum power or current that the Location can draw.</param>
        /// <param name="Unit">The unit in which the maximum draw is expressed.</param>
        public LocationMaxPower(Decimal            Value,
                                ChargingRateUnits  Unit)
        {

            this.Value  = Value;
            this.Unit   = Unit;

        }

        #endregion


        #region (static) Parse   (JSON, CustomLocationMaxPowerParser = null)

        /// <summary>
        /// Parse the given JSON representation of a location max power.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomLocationMaxPowerParser">A delegate to parse custom location max power JSON objects.</param>
        public static LocationMaxPower Parse(JObject                                         JSON,
                                             CustomJObjectParserDelegate<LocationMaxPower>?  CustomLocationMaxPowerParser   = null)
        {

            if (TryParse(JSON,
                         out var locationMaxPower,
                         out var errorResponse,
                         CustomLocationMaxPowerParser))
            {
                return locationMaxPower;
            }

            throw new ArgumentException("The given JSON representation of a location max power is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, CustomLocationMaxPowerParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a location max power.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomLocationMaxPowerParser">A delegate to parse custom location max power JSON objects.</param>
        public static LocationMaxPower? TryParse(JObject                                         JSON,
                                                 CustomJObjectParserDelegate<LocationMaxPower>?  CustomLocationMaxPowerParser   = null)
        {

            if (TryParse(JSON,
                         out var locationMaxPower,
                         out var errorResponse,
                         CustomLocationMaxPowerParser))
            {
                return locationMaxPower;
            }

            return default;

        }

        #endregion

        #region (static) TryParse(JSON, out LocationMaxPower, out ErrorResponse, CustomLocationMaxPowerParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a location max power.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="LocationMaxPower">The parsed location max power.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                    JSON,
                                       [NotNullWhen(true)]  out LocationMaxPower  LocationMaxPower,
                                       [NotNullWhen(false)] out String?           ErrorResponse)

            => TryParse(JSON,
                        out LocationMaxPower,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a location max power.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="LocationMaxPower">The parsed location max power.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomLocationMaxPowerParser">A delegate to parse custom location max power JSON objects.</param>
        public static Boolean TryParse(JObject                                         JSON,
                                       [NotNullWhen(true)]  out LocationMaxPower       LocationMaxPower,
                                       [NotNullWhen(false)] out String?                ErrorResponse,
                                       CustomJObjectParserDelegate<LocationMaxPower>?  CustomLocationMaxPowerParser   = null)
        {

            try
            {

                LocationMaxPower = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse Value    [mandatory]

                if (!JSON.ParseMandatory("value",
                                         "value",
                                         out Decimal Value,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Unit     [mandatory]

                if (!JSON.ParseMandatory("unit",
                                         "unit",
                                         ChargingRateUnits.TryParse,
                                         out ChargingRateUnits Unit,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                LocationMaxPower = new LocationMaxPower(
                                       Value,
                                       Unit
                                   );


                if (CustomLocationMaxPowerParser is not null)
                    LocationMaxPower = CustomLocationMaxPowerParser(JSON,
                                                                    LocationMaxPower);

                return true;

            }
            catch (Exception e)
            {
                LocationMaxPower  = default;
                ErrorResponse     = "The given JSON representation of a location max power is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomLocationMaxPowerSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomLocationMaxPowerSerializer">A delegate to serialize custom location max power JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<LocationMaxPower>? CustomLocationMaxPowerSerializer = null)
        {

            var json = JSONObject.Create(
                           new JProperty("value",  Value),
                           new JProperty("unit",   Unit.ToString())
                       );

            return CustomLocationMaxPowerSerializer is not null
                       ? CustomLocationMaxPowerSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this object.
        /// </summary>
        public LocationMaxPower Clone()

            => new (
                   Value,
                   Unit
               );

        #endregion


        #region Operator overloading

        #region Operator == (LocationMaxPower1, LocationMaxPower2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LocationMaxPower1">A location max power.</param>
        /// <param name="LocationMaxPower2">Another location max power.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (LocationMaxPower LocationMaxPower1,
                                           LocationMaxPower LocationMaxPower2)

            => LocationMaxPower1.Equals(LocationMaxPower2);

        #endregion

        #region Operator != (LocationMaxPower1, LocationMaxPower2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LocationMaxPower1">A location max power.</param>
        /// <param name="LocationMaxPower2">Another location max power.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (LocationMaxPower LocationMaxPower1,
                                           LocationMaxPower LocationMaxPower2)

            => !LocationMaxPower1.Equals(LocationMaxPower2);

        #endregion

        #region Operator <  (LocationMaxPower1, LocationMaxPower2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LocationMaxPower1">A location max power.</param>
        /// <param name="LocationMaxPower2">Another location max power.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (LocationMaxPower LocationMaxPower1,
                                          LocationMaxPower LocationMaxPower2)

            => LocationMaxPower1.CompareTo(LocationMaxPower2) < 0;

        #endregion

        #region Operator <= (LocationMaxPower1, LocationMaxPower2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LocationMaxPower1">A location max power.</param>
        /// <param name="LocationMaxPower2">Another location max power.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (LocationMaxPower LocationMaxPower1,
                                           LocationMaxPower LocationMaxPower2)

            => LocationMaxPower1.CompareTo(LocationMaxPower2) <= 0;

        #endregion

        #region Operator >  (LocationMaxPower1, LocationMaxPower2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LocationMaxPower1">A location max power.</param>
        /// <param name="LocationMaxPower2">Another location max power.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (LocationMaxPower LocationMaxPower1,
                                          LocationMaxPower LocationMaxPower2)

            => LocationMaxPower1.CompareTo(LocationMaxPower2) > 0;

        #endregion

        #region Operator >= (LocationMaxPower1, LocationMaxPower2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LocationMaxPower1">A location max power.</param>
        /// <param name="LocationMaxPower2">Another location max power.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (LocationMaxPower LocationMaxPower1,
                                           LocationMaxPower LocationMaxPower2)

            => LocationMaxPower1.CompareTo(LocationMaxPower2) >= 0;

        #endregion

        #endregion

        #region IComparable<LocationMaxPower> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two location max powers.
        /// </summary>
        /// <param name="Object">A location max power to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is LocationMaxPower locationMaxPower
                   ? CompareTo(locationMaxPower)
                   : throw new ArgumentException("The given object is not a location max power!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(LocationMaxPower)

        /// <summary>
        /// Compares two location max powers.
        /// </summary>
        /// <param name="LocationMaxPower">A location max power to compare with.</param>
        public Int32 CompareTo(LocationMaxPower LocationMaxPower)
        {

            var c = Value.CompareTo(LocationMaxPower.Value);

            if (c == 0)
                c = Unit. CompareTo(LocationMaxPower.Unit);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<LocationMaxPower> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two location max powers for equality.
        /// </summary>
        /// <param name="Object">A location max power to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is LocationMaxPower locationMaxPower &&
                   Equals(locationMaxPower);

        #endregion

        #region Equals(LocationMaxPower)

        /// <summary>
        /// Compares two location max powers for equality.
        /// </summary>
        /// <param name="LocationMaxPower">A location max power to compare with.</param>
        public Boolean Equals(LocationMaxPower LocationMaxPower)

            => Value.Equals(LocationMaxPower.Value) &&
               Unit. Equals(LocationMaxPower.Unit);

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

                return Value.GetHashCode() * 3 ^
                       Unit. GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"Max: {Value} {Unit}";

        #endregion

    }

}
