/*
 * Copyright (c) 2015-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

namespace cloud.charging.open.protocols.OCPIv2_1_1
{

    /// <summary>
    /// Extension methods for status types.
    /// </summary>
    public static class StatusTypeExtensions
    {

        /// <summary>
        /// Indicates whether this status type is null or empty.
        /// </summary>
        /// <param name="StatusType">A status type.</param>
        public static Boolean IsNullOrEmpty(this StatusType? StatusType)
            => !StatusType.HasValue || StatusType.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this status type is NOT null or empty.
        /// </summary>
        /// <param name="StatusType">A status type.</param>
        public static Boolean IsNotNullOrEmpty(this StatusType? StatusType)
            => StatusType.HasValue && StatusType.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The status of an EVSE/connector.
    /// </summary>
    public readonly struct StatusType : IId<StatusType>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this status type is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this status type is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the status type.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new status type based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a status type.</param>
        private StatusType(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as a status type.
        /// </summary>
        /// <param name="Text">A text representation of a status type.</param>
        public static StatusType Parse(String Text)
        {

            if (TryParse(Text, out var statusType))
                return statusType;

            throw new ArgumentException($"Invalid text representation of a status type: '" + Text + "'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a status type.
        /// </summary>
        /// <param name="Text">A text representation of a status type.</param>
        public static StatusType? TryParse(String Text)
        {

            if (TryParse(Text, out var statusType))
                return statusType;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out StatusType)

        /// <summary>
        /// Try to parse the given text as a status type.
        /// </summary>
        /// <param name="Text">A text representation of a status type.</param>
        /// <param name="StatusType">The parsed status type.</param>
        public static Boolean TryParse(String Text, out StatusType StatusType)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    StatusType = new StatusType(Text);
                    return true;
                }
                catch
                { }
            }

            StatusType = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this status type.
        /// </summary>
        public StatusType Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// No status information available (also used when offline).
        /// </summary>
        public static StatusType UNKNOWN
            => new ("UNKNOWN");

        /// <summary>
        /// The EVSE/Connector is able to start a new charging session.
        /// </summary>
        public static StatusType AVAILABLE
            => new ("AVAILABLE");

        /// <summary>
        /// The EVSE/Connector is not accessible because of a physical barrier, i.e. a car.
        /// </summary>
        public static StatusType BLOCKED
            => new ("BLOCKED");

        /// <summary>
        /// The EVSE/Connector is in use.
        /// </summary>
        public static StatusType CHARGING
            => new ("CHARGING");

        /// <summary>
        /// The EVSE/Connector is not yet active or it is no longer available (deleted).
        /// </summary>
        public static StatusType INOPERATIVE
            => new ("INOPERATIVE");

        /// <summary>
        /// The EVSE/Connector is currently out of order.
        /// </summary>
        public static StatusType OUTOFORDER
            => new ("OUTOFORDER");

        /// <summary>
        /// The EVSE/Connector is planned, will be operating soon.
        /// </summary>
        public static StatusType PLANNED
            => new ("PLANNED");

        /// <summary>
        /// The EVSE/Connector was discontinued/removed.
        /// </summary>
        public static StatusType REMOVED
            => new ("REMOVED");

        /// <summary>
        /// The EVSE/Connector is reserved for a particular EV driver and is unavailable for other drivers.
        /// </summary>
        public static StatusType RESERVED
            => new ("RESERVED");

        #endregion


        #region Operator overloading

        #region Operator == (StatusType1, StatusType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="StatusType1">A status type.</param>
        /// <param name="StatusType2">Another status type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (StatusType StatusType1,
                                           StatusType StatusType2)

            => StatusType1.Equals(StatusType2);

        #endregion

        #region Operator != (StatusType1, StatusType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="StatusType1">A status type.</param>
        /// <param name="StatusType2">Another status type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (StatusType StatusType1,
                                           StatusType StatusType2)

            => !StatusType1.Equals(StatusType2);

        #endregion

        #region Operator <  (StatusType1, StatusType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="StatusType1">A status type.</param>
        /// <param name="StatusType2">Another status type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (StatusType StatusType1,
                                          StatusType StatusType2)

            => StatusType1.CompareTo(StatusType2) < 0;

        #endregion

        #region Operator <= (StatusType1, StatusType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="StatusType1">A status type.</param>
        /// <param name="StatusType2">Another status type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (StatusType StatusType1,
                                           StatusType StatusType2)

            => StatusType1.CompareTo(StatusType2) <= 0;

        #endregion

        #region Operator >  (StatusType1, StatusType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="StatusType1">A status type.</param>
        /// <param name="StatusType2">Another status type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (StatusType StatusType1,
                                          StatusType StatusType2)

            => StatusType1.CompareTo(StatusType2) > 0;

        #endregion

        #region Operator >= (StatusType1, StatusType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="StatusType1">A status type.</param>
        /// <param name="StatusType2">Another status type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (StatusType StatusType1,
                                           StatusType StatusType2)

            => StatusType1.CompareTo(StatusType2) >= 0;

        #endregion

        #endregion

        #region IComparable<StatusType> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two status types.
        /// </summary>
        /// <param name="Object">A status type to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is StatusType statusType
                   ? CompareTo(statusType)
                   : throw new ArgumentException("The given object is not a status type!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(StatusType)

        /// <summary>
        /// Compares two status types.
        /// </summary>
        /// <param name="StatusType">A status type to compare with.</param>
        public Int32 CompareTo(StatusType StatusType)

            => String.Compare(InternalId,
                              StatusType.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<StatusType> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two status types for equality.
        /// </summary>
        /// <param name="Object">A status type to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is StatusType statusType &&
                   Equals(statusType);

        #endregion

        #region Equals(StatusType)

        /// <summary>
        /// Compares two status types for equality.
        /// </summary>
        /// <param name="StatusType">A status type to compare with.</param>
        public Boolean Equals(StatusType StatusType)

            => String.Equals(InternalId,
                             StatusType.InternalId,
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
