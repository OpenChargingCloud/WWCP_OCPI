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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv3_0
{

    /// <summary>
    /// Extension methods for meter reading units.
    /// </summary>
    public static class MeterReadingUnitExtensions
    {

        /// <summary>
        /// Indicates whether this meter reading unit is null or empty.
        /// </summary>
        /// <param name="MeterReadingUnit">A meter reading unit.</param>
        public static Boolean IsNullOrEmpty(this MeterReadingUnit? MeterReadingUnit)
            => !MeterReadingUnit.HasValue || MeterReadingUnit.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this meter reading unit is NOT null or empty.
        /// </summary>
        /// <param name="MeterReadingUnit">A meter reading unit.</param>
        public static Boolean IsNotNullOrEmpty(this MeterReadingUnit? MeterReadingUnit)
            => MeterReadingUnit.HasValue && MeterReadingUnit.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// Capabilities or functionalities of an EVSE.
    /// </summary>
    public readonly struct MeterReadingUnit : IId<MeterReadingUnit>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this meter reading unit is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this meter reading unit is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the meter reading unit.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new meter reading unit based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a meter reading unit.</param>
        private MeterReadingUnit(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as a meter reading unit.
        /// </summary>
        /// <param name="Text">A text representation of a meter reading unit.</param>
        public static MeterReadingUnit Parse(String Text)
        {

            if (TryParse(Text, out var meterReadingUnit))
                return meterReadingUnit;

            throw new ArgumentException($"Invalid text representation of a meter reading unit: '" + Text + "'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a meter reading unit.
        /// </summary>
        /// <param name="Text">A text representation of a meter reading unit.</param>
        public static MeterReadingUnit? TryParse(String Text)
        {

            if (TryParse(Text, out var meterReadingUnit))
                return meterReadingUnit;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out MeterReadingUnit)

        /// <summary>
        /// Try to parse the given text as a meter reading unit.
        /// </summary>
        /// <param name="Text">A text representation of a meter reading unit.</param>
        /// <param name="MeterReadingUnit">The parsed meter reading unit.</param>
        public static Boolean TryParse(String Text, out MeterReadingUnit MeterReadingUnit)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    MeterReadingUnit = new MeterReadingUnit(Text);
                    return true;
                }
                catch
                { }
            }

            MeterReadingUnit = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this meter reading unit.
        /// </summary>
        public MeterReadingUnit Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// Amperes
        /// </summary>
        public static MeterReadingUnit  A          { get; }
            = new ("A");

        /// <summary>
        /// Hertz
        /// </summary>
        public static MeterReadingUnit  Hz         { get; }
            = new ("Hz");

        /// <summary>
        /// Percentage
        /// </summary>
        public static MeterReadingUnit  PERCENT    { get; }
            = new ("PERCENT");

        /// <summary>
        /// Watts
        /// </summary>
        public static MeterReadingUnit  W          { get; }
            = new ("W");

        #endregion


        #region Operator overloading

        #region Operator == (MeterReadingUnit1, MeterReadingUnit2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MeterReadingUnit1">A meter reading unit.</param>
        /// <param name="MeterReadingUnit2">Another meter reading unit.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (MeterReadingUnit MeterReadingUnit1,
                                           MeterReadingUnit MeterReadingUnit2)

            => MeterReadingUnit1.Equals(MeterReadingUnit2);

        #endregion

        #region Operator != (MeterReadingUnit1, MeterReadingUnit2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MeterReadingUnit1">A meter reading unit.</param>
        /// <param name="MeterReadingUnit2">Another meter reading unit.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (MeterReadingUnit MeterReadingUnit1,
                                           MeterReadingUnit MeterReadingUnit2)

            => !MeterReadingUnit1.Equals(MeterReadingUnit2);

        #endregion

        #region Operator <  (MeterReadingUnit1, MeterReadingUnit2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MeterReadingUnit1">A meter reading unit.</param>
        /// <param name="MeterReadingUnit2">Another meter reading unit.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (MeterReadingUnit MeterReadingUnit1,
                                          MeterReadingUnit MeterReadingUnit2)

            => MeterReadingUnit1.CompareTo(MeterReadingUnit2) < 0;

        #endregion

        #region Operator <= (MeterReadingUnit1, MeterReadingUnit2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MeterReadingUnit1">A meter reading unit.</param>
        /// <param name="MeterReadingUnit2">Another meter reading unit.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (MeterReadingUnit MeterReadingUnit1,
                                           MeterReadingUnit MeterReadingUnit2)

            => MeterReadingUnit1.CompareTo(MeterReadingUnit2) <= 0;

        #endregion

        #region Operator >  (MeterReadingUnit1, MeterReadingUnit2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MeterReadingUnit1">A meter reading unit.</param>
        /// <param name="MeterReadingUnit2">Another meter reading unit.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (MeterReadingUnit MeterReadingUnit1,
                                          MeterReadingUnit MeterReadingUnit2)

            => MeterReadingUnit1.CompareTo(MeterReadingUnit2) > 0;

        #endregion

        #region Operator >= (MeterReadingUnit1, MeterReadingUnit2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MeterReadingUnit1">A meter reading unit.</param>
        /// <param name="MeterReadingUnit2">Another meter reading unit.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (MeterReadingUnit MeterReadingUnit1,
                                           MeterReadingUnit MeterReadingUnit2)

            => MeterReadingUnit1.CompareTo(MeterReadingUnit2) >= 0;

        #endregion

        #endregion

        #region IComparable<MeterReadingUnit> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two meter reading units.
        /// </summary>
        /// <param name="Object">A meter reading unit to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is MeterReadingUnit meterReadingUnit
                   ? CompareTo(meterReadingUnit)
                   : throw new ArgumentException("The given object is not a meter reading unit!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(MeterReadingUnit)

        /// <summary>
        /// Compares two meter reading units.
        /// </summary>
        /// <param name="MeterReadingUnit">A meter reading unit to compare with.</param>
        public Int32 CompareTo(MeterReadingUnit MeterReadingUnit)

            => String.Compare(InternalId,
                              MeterReadingUnit.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<MeterReadingUnit> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two meter reading units for equality.
        /// </summary>
        /// <param name="Object">A meter reading unit to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is MeterReadingUnit meterReadingUnit &&
                   Equals(meterReadingUnit);

        #endregion

        #region Equals(MeterReadingUnit)

        /// <summary>
        /// Compares two meter reading units for equality.
        /// </summary>
        /// <param name="MeterReadingUnit">A meter reading unit to compare with.</param>
        public Boolean Equals(MeterReadingUnit MeterReadingUnit)

            => String.Equals(InternalId,
                             MeterReadingUnit.InternalId,
                             StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()

            => InternalId?.ToUpper().GetHashCode() ?? 0;

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
