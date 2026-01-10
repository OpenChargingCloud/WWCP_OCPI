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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_3_0
{

    /// <summary>
    /// Extension methods for EVSE positions.
    /// </summary>
    public static class EVSEPositionExtensions
    {

        /// <summary>
        /// Indicates whether this EVSE position is null or empty.
        /// </summary>
        /// <param name="EVSEPosition">An EVSE position.</param>
        public static Boolean IsNullOrEmpty(this EVSEPosition? EVSEPosition)
            => !EVSEPosition.HasValue || EVSEPosition.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this EVSE position is NOT null or empty.
        /// </summary>
        /// <param name="EVSEPosition">An EVSE position.</param>
        public static Boolean IsNotNullOrEmpty(this EVSEPosition? EVSEPosition)
            => EVSEPosition.HasValue && EVSEPosition.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The EVSE position.
    /// </summary>
    public readonly struct EVSEPosition : IId<EVSEPosition>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this EVSE position is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this EVSE position is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the EVSE position.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new EVSE position based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of an EVSE position.</param>
        private EVSEPosition(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as an EVSE position.
        /// </summary>
        /// <param name="Text">A text representation of an EVSE position.</param>
        public static EVSEPosition Parse(String Text)
        {

            if (TryParse(Text, out var evsePosition))
                return evsePosition;

            throw new ArgumentException($"Invalid text representation of an EVSE position: '" + Text + "'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as an EVSE position.
        /// </summary>
        /// <param name="Text">A text representation of an EVSE position.</param>
        public static EVSEPosition? TryParse(String Text)
        {

            if (TryParse(Text, out var evsePosition))
                return evsePosition;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out EVSEPosition)

        /// <summary>
        /// Try to parse the given text as an EVSE position.
        /// </summary>
        /// <param name="Text">A text representation of an EVSE position.</param>
        /// <param name="EVSEPosition">The parsed EVSE position.</param>
        public static Boolean TryParse(String Text, out EVSEPosition EVSEPosition)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    EVSEPosition = new EVSEPosition(Text);
                    return true;
                }
                catch
                { }
            }

            EVSEPosition = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this EVSE position.
        /// </summary>
        public EVSEPosition Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// The EVSE is to the left of the vehicle.
        /// For streetside parking, the CPO can assume the vehicle is facing the same way as traffic
        /// on the side of the road that the EVSE is on.This means that LEFT is used for all streetside
        /// parking in locales with left-hand traffic.
        /// For parking bays leading sideways from a roadway, the CPO can assume the vehicle is
        /// parking with the nose away from the roadway (that is, entering the parking bay driving forward).
        /// </summary>
        public static EVSEPosition  LEFT      { get; }
            = new ("LEFT");

        /// <summary>
        /// The EVSE is to the right of the vehicle when parked.
        /// For streetside parking, the CPO can assume the vehicle is facing the same way as traffic
        /// on the side of the road that the EVSE is on.This means that RIGHT is used for all
        /// streetside parking in locales with right-hand traffic.
        /// For parking bays leading sideways from a roadway, the CPO can assume the vehicle is
        /// parking with the nose away from the roadway (that is, entering the parking bay driving forward).
        /// </summary>
        public static EVSEPosition  RIGHT     { get; }
            = new ("RIGHT");

        /// <summary>
        /// The EVSE is at the center of the impassable narrow end of a parking bay.
        /// </summary>
        public static EVSEPosition  CENTER    { get; }
            = new ("CENTER");

        #endregion


        #region Operator overloading

        #region Operator == (EVSEPosition1, EVSEPosition2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVSEPosition1">An EVSE position.</param>
        /// <param name="EVSEPosition2">Another EVSE position.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (EVSEPosition EVSEPosition1,
                                           EVSEPosition EVSEPosition2)

            => EVSEPosition1.Equals(EVSEPosition2);

        #endregion

        #region Operator != (EVSEPosition1, EVSEPosition2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVSEPosition1">An EVSE position.</param>
        /// <param name="EVSEPosition2">Another EVSE position.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (EVSEPosition EVSEPosition1,
                                           EVSEPosition EVSEPosition2)

            => !EVSEPosition1.Equals(EVSEPosition2);

        #endregion

        #region Operator <  (EVSEPosition1, EVSEPosition2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVSEPosition1">An EVSE position.</param>
        /// <param name="EVSEPosition2">Another EVSE position.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (EVSEPosition EVSEPosition1,
                                          EVSEPosition EVSEPosition2)

            => EVSEPosition1.CompareTo(EVSEPosition2) < 0;

        #endregion

        #region Operator <= (EVSEPosition1, EVSEPosition2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVSEPosition1">An EVSE position.</param>
        /// <param name="EVSEPosition2">Another EVSE position.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (EVSEPosition EVSEPosition1,
                                           EVSEPosition EVSEPosition2)

            => EVSEPosition1.CompareTo(EVSEPosition2) <= 0;

        #endregion

        #region Operator >  (EVSEPosition1, EVSEPosition2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVSEPosition1">An EVSE position.</param>
        /// <param name="EVSEPosition2">Another EVSE position.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (EVSEPosition EVSEPosition1,
                                          EVSEPosition EVSEPosition2)

            => EVSEPosition1.CompareTo(EVSEPosition2) > 0;

        #endregion

        #region Operator >= (EVSEPosition1, EVSEPosition2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVSEPosition1">An EVSE position.</param>
        /// <param name="EVSEPosition2">Another EVSE position.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (EVSEPosition EVSEPosition1,
                                           EVSEPosition EVSEPosition2)

            => EVSEPosition1.CompareTo(EVSEPosition2) >= 0;

        #endregion

        #endregion

        #region IComparable<EVSEPosition> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two EVSE positions.
        /// </summary>
        /// <param name="Object">An EVSE position to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is EVSEPosition evsePosition
                   ? CompareTo(evsePosition)
                   : throw new ArgumentException("The given object is not an EVSE position!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(EVSEPosition)

        /// <summary>
        /// Compares two EVSE positions.
        /// </summary>
        /// <param name="EVSEPosition">An EVSE position to compare with.</param>
        public Int32 CompareTo(EVSEPosition EVSEPosition)

            => String.Compare(InternalId,
                              EVSEPosition.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<EVSEPosition> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two EVSE positions for equality.
        /// </summary>
        /// <param name="Object">An EVSE position to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is EVSEPosition evsePosition &&
                   Equals(evsePosition);

        #endregion

        #region Equals(EVSEPosition)

        /// <summary>
        /// Compares two EVSE positions for equality.
        /// </summary>
        /// <param name="EVSEPosition">An EVSE position to compare with.</param>
        public Boolean Equals(EVSEPosition EVSEPosition)

            => String.Equals(InternalId,
                             EVSEPosition.InternalId,
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
