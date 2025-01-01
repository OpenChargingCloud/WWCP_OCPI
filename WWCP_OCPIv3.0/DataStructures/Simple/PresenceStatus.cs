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
    /// Extension methods for presence statuss.
    /// </summary>
    public static class PresenceStatusExtensions
    {

        /// <summary>
        /// Indicates whether this presence status is null or empty.
        /// </summary>
        /// <param name="PresenceStatus">A presence status.</param>
        public static Boolean IsNullOrEmpty(this PresenceStatus? PresenceStatus)
            => !PresenceStatus.HasValue || PresenceStatus.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this presence status is NOT null or empty.
        /// </summary>
        /// <param name="PresenceStatus">A presence status.</param>
        public static Boolean IsNotNullOrEmpty(this PresenceStatus? PresenceStatus)
            => PresenceStatus.HasValue && PresenceStatus.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The presence status.
    /// </summary>
    public readonly struct PresenceStatus : IId<PresenceStatus>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this presence status is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this presence status is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the presence status.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new presence status based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a presence status.</param>
        private PresenceStatus(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as a presence status.
        /// </summary>
        /// <param name="Text">A text representation of a presence status.</param>
        public static PresenceStatus Parse(String Text)
        {

            if (TryParse(Text, out var presenceStatus))
                return presenceStatus;

            throw new ArgumentException($"Invalid text representation of a presence status: '" + Text + "'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a presence status.
        /// </summary>
        /// <param name="Text">A text representation of a presence status.</param>
        public static PresenceStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var presenceStatus))
                return presenceStatus;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out PresenceStatus)

        /// <summary>
        /// Try to parse the given text as a presence status.
        /// </summary>
        /// <param name="Text">A text representation of a presence status.</param>
        /// <param name="PresenceStatus">The parsed presence status.</param>
        public static Boolean TryParse(String Text, out PresenceStatus PresenceStatus)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    PresenceStatus = new PresenceStatus(Text);
                    return true;
                }
                catch
                { }
            }

            PresenceStatus = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this presence status.
        /// </summary>
        public PresenceStatus Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// The EVSE is currently present and whether it is currently usable can be indicated using the EVSE Status module.
        /// </summary>
        public static PresenceStatus  PRESENT    { get; }
            = new ("PRESENT");

        /// <summary>
        /// The EVSE is not currently present but it is planned for the future.
        /// </summary>
        public static PresenceStatus  PLANNED    { get; }
            = new ("PLANNED");

        /// <summary>
        /// The EVSE is not currently present but it is used to be present in the past.
        /// </summary>
        public static PresenceStatus  REMOVED    { get; }
            = new ("REMOVED");

        #endregion


        #region Operator overloading

        #region Operator == (PresenceStatus1, PresenceStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PresenceStatus1">A presence status.</param>
        /// <param name="PresenceStatus2">Another presence status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (PresenceStatus PresenceStatus1,
                                           PresenceStatus PresenceStatus2)

            => PresenceStatus1.Equals(PresenceStatus2);

        #endregion

        #region Operator != (PresenceStatus1, PresenceStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PresenceStatus1">A presence status.</param>
        /// <param name="PresenceStatus2">Another presence status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (PresenceStatus PresenceStatus1,
                                           PresenceStatus PresenceStatus2)

            => !PresenceStatus1.Equals(PresenceStatus2);

        #endregion

        #region Operator <  (PresenceStatus1, PresenceStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PresenceStatus1">A presence status.</param>
        /// <param name="PresenceStatus2">Another presence status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (PresenceStatus PresenceStatus1,
                                          PresenceStatus PresenceStatus2)

            => PresenceStatus1.CompareTo(PresenceStatus2) < 0;

        #endregion

        #region Operator <= (PresenceStatus1, PresenceStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PresenceStatus1">A presence status.</param>
        /// <param name="PresenceStatus2">Another presence status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (PresenceStatus PresenceStatus1,
                                           PresenceStatus PresenceStatus2)

            => PresenceStatus1.CompareTo(PresenceStatus2) <= 0;

        #endregion

        #region Operator >  (PresenceStatus1, PresenceStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PresenceStatus1">A presence status.</param>
        /// <param name="PresenceStatus2">Another presence status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (PresenceStatus PresenceStatus1,
                                          PresenceStatus PresenceStatus2)

            => PresenceStatus1.CompareTo(PresenceStatus2) > 0;

        #endregion

        #region Operator >= (PresenceStatus1, PresenceStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PresenceStatus1">A presence status.</param>
        /// <param name="PresenceStatus2">Another presence status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (PresenceStatus PresenceStatus1,
                                           PresenceStatus PresenceStatus2)

            => PresenceStatus1.CompareTo(PresenceStatus2) >= 0;

        #endregion

        #endregion

        #region IComparable<PresenceStatus> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two presence statuss.
        /// </summary>
        /// <param name="Object">A presence status to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is PresenceStatus presenceStatus
                   ? CompareTo(presenceStatus)
                   : throw new ArgumentException("The given object is not a presence status!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(PresenceStatus)

        /// <summary>
        /// Compares two presence statuss.
        /// </summary>
        /// <param name="PresenceStatus">A presence status to compare with.</param>
        public Int32 CompareTo(PresenceStatus PresenceStatus)

            => String.Compare(InternalId,
                              PresenceStatus.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<PresenceStatus> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two presence statuss for equality.
        /// </summary>
        /// <param name="Object">A presence status to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is PresenceStatus presenceStatus &&
                   Equals(presenceStatus);

        #endregion

        #region Equals(PresenceStatus)

        /// <summary>
        /// Compares two presence statuss for equality.
        /// </summary>
        /// <param name="PresenceStatus">A presence status to compare with.</param>
        public Boolean Equals(PresenceStatus PresenceStatus)

            => String.Equals(InternalId,
                             PresenceStatus.InternalId,
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
