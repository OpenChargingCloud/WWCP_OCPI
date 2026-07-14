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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPI
{

    /// <summary>
    /// Extension methods for the EVSE status.
    /// </summary>
    public static class EVSEStatusExtensions
    {

        public static JObject ToJSON(this IEnumerable<EVSEStatus>  StatusList,
                                     UInt64?                       Skip   = null,
                                     UInt64?                       Take   = null)

            => StatusList.ToJSON(
                   Skip,
                   Take
               );


    }


    /// <summary>
    /// The current status of an EVSE.
    /// </summary>
    public readonly struct EVSEStatus : IEquatable <EVSEStatus>,
                                        IComparable<EVSEStatus>,
                                        IComparable
    {

        #region Properties

        /// <summary>
        /// The unique identification of the EVSE.
        /// </summary>
        public EVSE_UId        UId          { get; }

        /// <summary>
        /// The current status of the EVSE.
        /// </summary>
        public StatusType      Status       { get; }

        /// <summary>
        /// The timestamp of the current status of the EVSE.
        /// </summary>
        public DateTimeOffset  Timestamp    { get; }

        /// <summary>
        /// The optional roaming identification of the EVSE.
        /// </summary>
        public EVSE_Id?        Id           { get; }

        /// <summary>
        /// An optional data source or context for this EVSE status.
        /// </summary>
        public Context?        Context      { get; }

        #endregion

        #region Constructor(s)

        #region EVSEStatus(UId, Status,            Id = null, Context = null)

        /// <summary>
        /// Create a new EVSE status.
        /// </summary>
        /// <param name="UId">The unique identification of the EVSE.</param>
        /// <param name="Status">The current timestamped status of the EVSE.</param>
        /// <param name="Id">The optional roaming identification of the EVSE.</param>
        /// <param name="Context">An optional data source or context for the EVSE status.</param>
        public EVSEStatus(EVSE_UId                 UId,
                          Timestamped<StatusType>  Status,
                          EVSE_Id?                 Id        = null,
                          Context?                 Context   = null)

            : this(UId,
                   Status.Value,
                   Status.Timestamp,
                   Id,
                   Context)

        { }

        #endregion

        #region EVSEStatus(UId, Status, Timestamp, Id = null, Context = null)

        /// <summary>
        /// Create a new EVSE status.
        /// </summary>
        /// <param name="UId">The unique identification of the EVSE.</param>
        /// <param name="Status">The current status of the EVSE.</param>
        /// <param name="Timestamp">The timestamp of the status change of the EVSE.</param>
        /// <param name="Id">The optional roaming identification of the EVSE.</param>
        /// <param name="Context">An optional data source or context for the EVSE status.</param>
        public EVSEStatus(EVSE_UId        UId,
                          StatusType      Status,
                          DateTimeOffset  Timestamp,
                          EVSE_Id?        Id        = null,
                          Context?        Context   = null)
        {

            this.UId        = UId;
            this.Status     = Status;
            this.Timestamp  = Timestamp;
            this.Id         = Id;
            this.Context    = Context;

            unchecked
            {

                hashCode = UId.      GetHashCode()       * 11 ^
                           Status.   GetHashCode()       *  7 ^
                           Timestamp.GetHashCode()       *  5 ^
                          (Id?.      GetHashCode() ?? 0) *  3 ^
                          (Context?. GetHashCode() ?? 0);

            }

        }

        #endregion

        #endregion


        #region (static) Snapshot(EVSE)

        /// <summary>
        /// Take a snapshot of the current EVSE status.
        /// </summary>
        /// <param name="EVSE">An EVSE.</param>
        public static EVSEStatus Snapshot(IEVSE EVSE)

            => new (
                   EVSE.UId,
                   EVSE.Status,
                   EVSE.LastUpdated,
                   EVSE.EVSEId
               );

        #endregion


        #region Operator overloading

        #region Operator == (EVSEStatus1, EVSEStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVSEStatus1">An EVSE status.</param>
        /// <param name="EVSEStatus2">Another EVSE status.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (EVSEStatus EVSEStatus1,
                                           EVSEStatus EVSEStatus2)

            => EVSEStatus1.Equals(EVSEStatus2);

        #endregion

        #region Operator != (EVSEStatus1, EVSEStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVSEStatus1">An EVSE status.</param>
        /// <param name="EVSEStatus2">Another EVSE status.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (EVSEStatus EVSEStatus1,
                                           EVSEStatus EVSEStatus2)

            => !EVSEStatus1.Equals(EVSEStatus2);

        #endregion

        #region Operator <  (EVSEStatus1, EVSEStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVSEStatus1">An EVSE status.</param>
        /// <param name="EVSEStatus2">Another EVSE status.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator < (EVSEStatus EVSEStatus1,
                                          EVSEStatus EVSEStatus2)

            => EVSEStatus1.CompareTo(EVSEStatus2) < 0;

        #endregion

        #region Operator <= (EVSEStatus1, EVSEStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVSEStatus1">An EVSE status.</param>
        /// <param name="EVSEStatus2">Another EVSE status.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator <= (EVSEStatus EVSEStatus1,
                                           EVSEStatus EVSEStatus2)

            => EVSEStatus1.CompareTo(EVSEStatus2) <= 0;

        #endregion

        #region Operator >  (EVSEStatus1, EVSEStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVSEStatus1">An EVSE status.</param>
        /// <param name="EVSEStatus2">Another EVSE status.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator > (EVSEStatus EVSEStatus1,
                                          EVSEStatus EVSEStatus2)

            => EVSEStatus1.CompareTo(EVSEStatus2) > 0;

        #endregion

        #region Operator >= (EVSEStatus1, EVSEStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVSEStatus1">An EVSE status.</param>
        /// <param name="EVSEStatus2">Another EVSE status.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator >= (EVSEStatus EVSEStatus1,
                                           EVSEStatus EVSEStatus2)

            => EVSEStatus1.CompareTo(EVSEStatus2) >= 0;

        #endregion

        #endregion

        #region IComparable<EVSEStatus> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two EVSE status.
        /// </summary>
        /// <param name="Object">An EVSE status to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is EVSEStatus evseStatus
                   ? CompareTo(evseStatus)
                   : throw new ArgumentException("The given object is not an EVSE status!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(EVSEStatus)

        /// <summary>
        /// Compares two EVSE status.
        /// </summary>
        /// <param name="EVSEStatus">An EVSE status to compare with.</param>
        public Int32 CompareTo(EVSEStatus EVSEStatus)
        {

            var c = UId.                  CompareTo(EVSEStatus.UId);

            if (c == 0)
                c = Status.               CompareTo(EVSEStatus.Status);

            if (c == 0)
                c = Timestamp.ToISO8601().CompareTo(EVSEStatus.Timestamp.ToISO8601());

            if (c == 0 && Id.HasValue         && EVSEStatus.Id.HasValue)
                c = Id.Value.             CompareTo(EVSEStatus.Id.Value);

            if (c == 0 && Context is not null && EVSEStatus.Context is not null)
                c = Context.              CompareTo(EVSEStatus.Context);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<EVSEStatus> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two EVSE status for equality.
        /// </summary>
        /// <param name="Object">An EVSE status to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is EVSEStatus evseStatus &&
                   Equals(evseStatus);

        #endregion

        #region Equals(EVSEStatus)

        /// <summary>
        /// Compares two EVSE status for equality.
        /// </summary>
        /// <param name="EVSEStatus">An EVSE status to compare with.</param>
        public Boolean Equals(EVSEStatus EVSEStatus)

            => UId.                  Equals(EVSEStatus.UId)                   &&
               Status.               Equals(EVSEStatus.Status)                &&
               Timestamp.ToISO8601().Equals(EVSEStatus.Timestamp.ToISO8601()) &&

            ((!Id.HasValue         && !EVSEStatus.Id.HasValue)     ||
              (Id.HasValue         &&  EVSEStatus.Id.HasValue         && Id.Value.Equals(EVSEStatus.Id.Value))) &&

             ((Context is null     &&  EVSEStatus.Context is null) ||
              (Context is not null &&  EVSEStatus.Context is not null && Context. Equals(EVSEStatus.Context)));

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

            => $"{Id}{(Id.HasValue ? $" ({Id.Value})" : "")} -> '{Status}' since {Timestamp.ToISO8601()}";

        #endregion

    }

}
