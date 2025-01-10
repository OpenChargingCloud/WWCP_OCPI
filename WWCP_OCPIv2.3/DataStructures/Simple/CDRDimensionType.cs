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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_3
{

    /// <summary>
    /// Extension methods for CDR dimension types.
    /// </summary>
    public static class CDRDimensionTypeExtensions
    {

        /// <summary>
        /// Indicates whether this CDR dimension type is null or empty.
        /// </summary>
        /// <param name="CDRDimensionType">A CDR dimension type.</param>
        public static Boolean IsNullOrEmpty(this CDRDimensionType? CDRDimensionType)
            => !CDRDimensionType.HasValue || CDRDimensionType.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this CDR dimension type is NOT null or empty.
        /// </summary>
        /// <param name="CDRDimensionType">A CDR dimension type.</param>
        public static Boolean IsNotNullOrEmpty(this CDRDimensionType? CDRDimensionType)
            => CDRDimensionType.HasValue && CDRDimensionType.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The CDR dimension type.
    /// </summary>
    public readonly struct CDRDimensionType : IId<CDRDimensionType>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this CDR dimension type is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this CDR dimension type is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the CDR dimension type.
        /// </summary>
        public UInt64 Length
            => (UInt64)InternalId.Length;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new CDR dimension type based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a CDR dimension type.</param>
        private CDRDimensionType(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as a CDR dimension type.
        /// </summary>
        /// <param name="Text">A text representation of a CDR dimension type.</param>
        public static CDRDimensionType Parse(String Text)
        {

            if (TryParse(Text, out var cdrDimensionType))
                return cdrDimensionType;

            throw new ArgumentException($"Invalid text representation of a CDR dimension type: '" + Text + "'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a CDR dimension type.
        /// </summary>
        /// <param name="Text">A text representation of a CDR dimension type.</param>
        public static CDRDimensionType? TryParse(String Text)
        {

            if (TryParse(Text, out var cdrDimensionType))
                return cdrDimensionType;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out CDRDimensionType)

        /// <summary>
        /// Try to parse the given text as a CDR dimension type.
        /// </summary>
        /// <param name="Text">A text representation of a CDR dimension type.</param>
        /// <param name="CDRDimensionType">The parsed CDR dimension type.</param>
        public static Boolean TryParse(String Text, out CDRDimensionType CDRDimensionType)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    CDRDimensionType = new CDRDimensionType(Text);
                    return true;
                }
                catch
                { }
            }

            CDRDimensionType = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this CDR dimension type.
        /// </summary>
        public CDRDimensionType Clone

            => new(
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// Average charging current during this ChargingPeriod: defined in A (Ampere).
        /// When negative, the current is flowing from the EV to the grid.
        /// </summary>
        public static CDRDimensionType CURRENT             { get; }      // Session only!
            = new ("CURRENT");

        /// <summary>
        /// Total amount of energy (dis-)charged during this ChargingPeriod: defined in kWh. When negative, more energy was feed into the grid then charged into the EV. Default step_size is 1.
        /// </summary>
        public static CDRDimensionType ENERGY              { get; }
            = new ("ENERGY");

        /// <summary>
        /// Total amount of energy feed back into the grid: defined in kWh.
        /// </summary>
        public static CDRDimensionType ENERGY_EXPORT       { get; }  // Session only!
            = new ("ENERGY_EXPORT");

        /// <summary>
        /// Total amount of energy charged, defined in kWh.
        /// </summary>
        public static CDRDimensionType ENERGY_IMPORT       { get; }  // Session only!
            = new ("ENERGY_IMPORT");

        /// <summary>
        /// Sum of the maximum current over all phases, reached during this ChargingPeriod: defined in A (Ampere).
        /// </summary>
        public static CDRDimensionType MAX_CURRENT         { get; }
            = new ("MAX_CURRENT");

        /// <summary>
        /// Sum of the minimum current over all phases, reached during this ChargingPeriod, when negative, current has flowed from the EV to the grid. Defined in A (Ampere).
        /// </summary>
        public static CDRDimensionType MIN_CURRENT         { get; }
            = new ("MIN_CURRENT");

        /// <summary>
        /// Maximum power reached during this ChargingPeriod: defined in kW (Kilowatt).
        /// </summary>
        public static CDRDimensionType MAX_POWER           { get; }
            = new ("MAX_POWER");

        /// <summary>
        /// Minimum power reached during this ChargingPeriod: defined in kW (Kilowatt),
        /// when negative, the power has flowed from the EV to the grid.
        /// </summary>
        public static CDRDimensionType MIN_POWER           { get; }
            = new ("MIN_POWER");

        /// <summary>
        /// Time during this ChargingPeriod not charging: defined in hours, default step_size multiplier is 1 second.
        /// </summary>
        public static CDRDimensionType PARKING_TIME        { get; }
            = new ("PARKING_TIME");

        /// <summary>
        /// Average power during this ChargingPeriod: defined in kW (Kilowatt).
        /// When negative, the power is flowing from the EV to the grid.
        /// </summary>
        public static CDRDimensionType POWER               { get; }        // Session only!
            = new ("POWER");

        /// <summary>
        /// Time during this ChargingPeriod Charge Point has been reserved and
        /// not yet been in use for this customer:
        /// defined in hours, default step_size multiplier is 1 second.
        /// </summary>
        public static CDRDimensionType RESERVATION_TIME    { get; }
            = new ("RESERVATION_TIME");

        /// <summary>
        /// Current state of charge of the EV, in percentage, values allowed:
        /// 0 to 100.
        /// </summary>
        public static CDRDimensionType STATE_OF_CHARGE     { get; }    // Session only!
            = new ("STATE_OF_CHARGE");

        /// <summary>
        /// Time charging during this ChargingPeriod: defined in hours,
        /// default step_size multiplier is 1 second.
        /// </summary>
        public static CDRDimensionType TIME                { get; }
            = new ("TIME");

        #endregion


        #region Operator overloading

        #region Operator == (CDRDimensionType1, CDRDimensionType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDRDimensionType1">A CDR dimension type.</param>
        /// <param name="CDRDimensionType2">Another CDR dimension type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (CDRDimensionType CDRDimensionType1,
                                           CDRDimensionType CDRDimensionType2)

            => CDRDimensionType1.Equals(CDRDimensionType2);

        #endregion

        #region Operator != (CDRDimensionType1, CDRDimensionType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDRDimensionType1">A CDR dimension type.</param>
        /// <param name="CDRDimensionType2">Another CDR dimension type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (CDRDimensionType CDRDimensionType1,
                                           CDRDimensionType CDRDimensionType2)

            => !CDRDimensionType1.Equals(CDRDimensionType2);

        #endregion

        #region Operator <  (CDRDimensionType1, CDRDimensionType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDRDimensionType1">A CDR dimension type.</param>
        /// <param name="CDRDimensionType2">Another CDR dimension type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (CDRDimensionType CDRDimensionType1,
                                          CDRDimensionType CDRDimensionType2)

            => CDRDimensionType1.CompareTo(CDRDimensionType2) < 0;

        #endregion

        #region Operator <= (CDRDimensionType1, CDRDimensionType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDRDimensionType1">A CDR dimension type.</param>
        /// <param name="CDRDimensionType2">Another CDR dimension type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (CDRDimensionType CDRDimensionType1,
                                           CDRDimensionType CDRDimensionType2)

            => CDRDimensionType1.CompareTo(CDRDimensionType2) <= 0;

        #endregion

        #region Operator >  (CDRDimensionType1, CDRDimensionType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDRDimensionType1">A CDR dimension type.</param>
        /// <param name="CDRDimensionType2">Another CDR dimension type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (CDRDimensionType CDRDimensionType1,
                                          CDRDimensionType CDRDimensionType2)

            => CDRDimensionType1.CompareTo(CDRDimensionType2) > 0;

        #endregion

        #region Operator >= (CDRDimensionType1, CDRDimensionType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDRDimensionType1">A CDR dimension type.</param>
        /// <param name="CDRDimensionType2">Another CDR dimension type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (CDRDimensionType CDRDimensionType1,
                                           CDRDimensionType CDRDimensionType2)

            => CDRDimensionType1.CompareTo(CDRDimensionType2) >= 0;

        #endregion

        #endregion

        #region IComparable<CDRDimensionType> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two CDR dimension types.
        /// </summary>
        /// <param name="Object">A CDR dimension type to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is CDRDimensionType cdrDimensionType
                   ? CompareTo(cdrDimensionType)
                   : throw new ArgumentException("The given object is not a CDR dimension type!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(CDRDimensionType)

        /// <summary>
        /// Compares two CDR dimension types.
        /// </summary>
        /// <param name="CDRDimensionType">A CDR dimension type to compare with.</param>
        public Int32 CompareTo(CDRDimensionType CDRDimensionType)

            => String.Compare(InternalId,
                              CDRDimensionType.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<CDRDimensionType> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two CDR dimension types for equality.
        /// </summary>
        /// <param name="Object">A CDR dimension type to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is CDRDimensionType cdrDimensionType &&
                   Equals(cdrDimensionType);

        #endregion

        #region Equals(CDRDimensionType)

        /// <summary>
        /// Compares two CDR dimension types for equality.
        /// </summary>
        /// <param name="CDRDimensionType">A CDR dimension type to compare with.</param>
        public Boolean Equals(CDRDimensionType CDRDimensionType)

            => String.Equals(InternalId,
                             CDRDimensionType.InternalId,
                             StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()

            => InternalId?.ToLower().GetHashCode() ?? 0;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => InternalId ?? "";

        #endregion

    }

}
