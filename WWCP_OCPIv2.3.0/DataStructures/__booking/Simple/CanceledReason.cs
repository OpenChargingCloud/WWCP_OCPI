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

namespace cloud.charging.open.protocols.OCPIv2_3_0
{

    /// <summary>
    /// Extension methods for canceled reasons.
    /// </summary>
    public static class CanceledReasonExtensions
    {

        /// <summary>
        /// Indicates whether this canceled reason is null or empty.
        /// </summary>
        /// <param name="CanceledReason">A canceled reason.</param>
        public static Boolean IsNullOrEmpty(this CanceledReason? CanceledReason)
            => !CanceledReason.HasValue || CanceledReason.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this canceled reason is NOT null or empty.
        /// </summary>
        /// <param name="CanceledReason">A canceled reason.</param>
        public static Boolean IsNotNullOrEmpty(this CanceledReason? CanceledReason)
            => CanceledReason.HasValue && CanceledReason.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The canceled reason.
    /// </summary>
    public readonly struct CanceledReason : IId<CanceledReason>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this canceled reason is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this canceled reason is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the canceled reason.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new canceled reason based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a canceled reason.</param>
        private CanceledReason(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as a canceled reason.
        /// </summary>
        /// <param name="Text">A text representation of a canceled reason.</param>
        public static CanceledReason Parse(String Text)
        {

            if (TryParse(Text, out var canceledReason))
                return canceledReason;

            throw new ArgumentException($"Invalid text representation of a canceled reason: '" + Text + "'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a canceled reason.
        /// </summary>
        /// <param name="Text">A text representation of a canceled reason.</param>
        public static CanceledReason? TryParse(String Text)
        {

            if (TryParse(Text, out var canceledReason))
                return canceledReason;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out CanceledReason)

        /// <summary>
        /// Try to parse the given text as a canceled reason.
        /// </summary>
        /// <param name="Text">A text representation of a canceled reason.</param>
        /// <param name="CanceledReason">The parsed canceled reason.</param>
        public static Boolean TryParse(String Text, out CanceledReason CanceledReason)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    CanceledReason = new CanceledReason(Text);
                    return true;
                }
                catch
                { }
            }

            CanceledReason = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this canceled reason.
        /// </summary>
        public CanceledReason Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// Any other status / unknown status.
        /// </summary>
        public static CanceledReason  UNKNOWN           { get; }
            = new ("UNKNOWN");

        /// <summary>
        /// No power available at the site, set by the CPO.
        /// </summary>
        public static CanceledReason  POWER_OUTAGE      { get; }
            = new ("POWER_OUTAGE");

        /// <summary>
        /// The charger is broken and charging is not possible, set by the CPO.
        /// </summary>
        public static CanceledReason  BROKEN_CHARGER    { get; }
            = new ("BROKEN_CHARGER");

        /// <summary>
        /// The chargers are full, because someone isn’t leaving, set by the CPO.
        /// </summary>
        public static CanceledReason  FULL              { get; }
            = new ("FULL");

        /// <summary>
        /// The reserved charger isn’t physically reachable.
        /// </summary>
        public static CanceledReason  BLOCKED           { get; }
            = new ("BLOCKED");

        /// <summary>
        /// The vehicle can’t come in time because of traffic, set by the MSP.
        /// </summary>
        public static CanceledReason  TRAFFIC           { get; }
            = new ("TRAFFIC");

        /// <summary>
        /// The vehicle broke down and can’t make the reservation, set by the MSP.
        /// </summary>
        public static CanceledReason  BROKEN_VEHICLE    { get; }
            = new ("BROKEN_VEHICLE");

        /// <summary>
        /// The driver didn’t communicate a reason for canceling, set by rhe MSP.
        /// </summary>
        public static CanceledReason  NO_CANCELED       { get; }
            = new ("NO_CANCELED");

        #endregion


        #region Operator overloading

        #region Operator == (CanceledReason1, CanceledReason2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CanceledReason1">A canceled reason.</param>
        /// <param name="CanceledReason2">Another canceled reason.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (CanceledReason CanceledReason1,
                                           CanceledReason CanceledReason2)

            => CanceledReason1.Equals(CanceledReason2);

        #endregion

        #region Operator != (CanceledReason1, CanceledReason2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CanceledReason1">A canceled reason.</param>
        /// <param name="CanceledReason2">Another canceled reason.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (CanceledReason CanceledReason1,
                                           CanceledReason CanceledReason2)

            => !CanceledReason1.Equals(CanceledReason2);

        #endregion

        #region Operator <  (CanceledReason1, CanceledReason2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CanceledReason1">A canceled reason.</param>
        /// <param name="CanceledReason2">Another canceled reason.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (CanceledReason CanceledReason1,
                                          CanceledReason CanceledReason2)

            => CanceledReason1.CompareTo(CanceledReason2) < 0;

        #endregion

        #region Operator <= (CanceledReason1, CanceledReason2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CanceledReason1">A canceled reason.</param>
        /// <param name="CanceledReason2">Another canceled reason.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (CanceledReason CanceledReason1,
                                           CanceledReason CanceledReason2)

            => CanceledReason1.CompareTo(CanceledReason2) <= 0;

        #endregion

        #region Operator >  (CanceledReason1, CanceledReason2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CanceledReason1">A canceled reason.</param>
        /// <param name="CanceledReason2">Another canceled reason.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (CanceledReason CanceledReason1,
                                          CanceledReason CanceledReason2)

            => CanceledReason1.CompareTo(CanceledReason2) > 0;

        #endregion

        #region Operator >= (CanceledReason1, CanceledReason2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CanceledReason1">A canceled reason.</param>
        /// <param name="CanceledReason2">Another canceled reason.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (CanceledReason CanceledReason1,
                                           CanceledReason CanceledReason2)

            => CanceledReason1.CompareTo(CanceledReason2) >= 0;

        #endregion

        #endregion

        #region IComparable<CanceledReason> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two canceled reasons.
        /// </summary>
        /// <param name="Object">A canceled reason to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is CanceledReason canceledReason
                   ? CompareTo(canceledReason)
                   : throw new ArgumentException("The given object is not a canceled reason!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(CanceledReason)

        /// <summary>
        /// Compares two canceled reasons.
        /// </summary>
        /// <param name="CanceledReason">A canceled reason to compare with.</param>
        public Int32 CompareTo(CanceledReason CanceledReason)

            => String.Compare(InternalId,
                              CanceledReason.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<CanceledReason> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two canceled reasons for equality.
        /// </summary>
        /// <param name="Object">A canceled reason to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is CanceledReason canceledReason &&
                   Equals(canceledReason);

        #endregion

        #region Equals(CanceledReason)

        /// <summary>
        /// Compares two canceled reasons for equality.
        /// </summary>
        /// <param name="CanceledReason">A canceled reason to compare with.</param>
        public Boolean Equals(CanceledReason CanceledReason)

            => String.Equals(InternalId,
                             CanceledReason.InternalId,
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
