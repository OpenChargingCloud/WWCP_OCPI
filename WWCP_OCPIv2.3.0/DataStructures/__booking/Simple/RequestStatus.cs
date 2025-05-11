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
    /// Extension methods for request status.
    /// </summary>
    public static class RequestStatusExtensions
    {

        /// <summary>
        /// Indicates whether this request status is null or empty.
        /// </summary>
        /// <param name="RequestStatus">A request status.</param>
        public static Boolean IsNullOrEmpty(this RequestStatus? RequestStatus)
            => !RequestStatus.HasValue || RequestStatus.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this request status is NOT null or empty.
        /// </summary>
        /// <param name="RequestStatus">A request status.</param>
        public static Boolean IsNotNullOrEmpty(this RequestStatus? RequestStatus)
            => RequestStatus.HasValue && RequestStatus.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The request status.
    /// </summary>
    public readonly struct RequestStatus : IId<RequestStatus>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this request status is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this request status is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the request status.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new request status based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a request status.</param>
        private RequestStatus(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as a request status.
        /// </summary>
        /// <param name="Text">A text representation of a request status.</param>
        public static RequestStatus Parse(String Text)
        {

            if (TryParse(Text, out var requestStatus))
                return requestStatus;

            throw new ArgumentException($"Invalid text representation of a request status: '" + Text + "'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a request status.
        /// </summary>
        /// <param name="Text">A text representation of a request status.</param>
        public static RequestStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var requestStatus))
                return requestStatus;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out RequestStatus)

        /// <summary>
        /// Try to parse the given text as a request status.
        /// </summary>
        /// <param name="Text">A text representation of a request status.</param>
        /// <param name="RequestStatus">The parsed request status.</param>
        public static Boolean TryParse(String Text, out RequestStatus RequestStatus)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    RequestStatus = new RequestStatus(Text);
                    return true;
                }
                catch
                { }
            }

            RequestStatus = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this request status.
        /// </summary>
        public RequestStatus Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// Booking request pending processing by the CPO.
        /// </summary>
        public static RequestStatus  PENDING     { get; }
            = new ("PENDING");

        /// <summary>
        /// Booking request accepted by the CPO.
        /// </summary>
        public static RequestStatus  ACCEPTED    { get; }
            = new ("ACCEPTED");

        /// <summary>
        /// Booking request declined by the CPO.
        /// </summary>
        public static RequestStatus  DECLINED    { get; }
            = new ("DECLINED");

        /// <summary>
        /// Request for booking failed (error).
        /// </summary>
        public static RequestStatus  FAILED      { get; }
            = new ("FAILED");

        #endregion


        #region Operator overloading

        #region Operator == (RequestStatus1, RequestStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RequestStatus1">A request status.</param>
        /// <param name="RequestStatus2">Another request status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (RequestStatus RequestStatus1,
                                           RequestStatus RequestStatus2)

            => RequestStatus1.Equals(RequestStatus2);

        #endregion

        #region Operator != (RequestStatus1, RequestStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RequestStatus1">A request status.</param>
        /// <param name="RequestStatus2">Another request status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (RequestStatus RequestStatus1,
                                           RequestStatus RequestStatus2)

            => !RequestStatus1.Equals(RequestStatus2);

        #endregion

        #region Operator <  (RequestStatus1, RequestStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RequestStatus1">A request status.</param>
        /// <param name="RequestStatus2">Another request status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (RequestStatus RequestStatus1,
                                          RequestStatus RequestStatus2)

            => RequestStatus1.CompareTo(RequestStatus2) < 0;

        #endregion

        #region Operator <= (RequestStatus1, RequestStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RequestStatus1">A request status.</param>
        /// <param name="RequestStatus2">Another request status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (RequestStatus RequestStatus1,
                                           RequestStatus RequestStatus2)

            => RequestStatus1.CompareTo(RequestStatus2) <= 0;

        #endregion

        #region Operator >  (RequestStatus1, RequestStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RequestStatus1">A request status.</param>
        /// <param name="RequestStatus2">Another request status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (RequestStatus RequestStatus1,
                                          RequestStatus RequestStatus2)

            => RequestStatus1.CompareTo(RequestStatus2) > 0;

        #endregion

        #region Operator >= (RequestStatus1, RequestStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RequestStatus1">A request status.</param>
        /// <param name="RequestStatus2">Another request status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (RequestStatus RequestStatus1,
                                           RequestStatus RequestStatus2)

            => RequestStatus1.CompareTo(RequestStatus2) >= 0;

        #endregion

        #endregion

        #region IComparable<RequestStatus> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two request status.
        /// </summary>
        /// <param name="Object">A request status to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is RequestStatus requestStatus
                   ? CompareTo(requestStatus)
                   : throw new ArgumentException("The given object is not a request status!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(RequestStatus)

        /// <summary>
        /// Compares two request status.
        /// </summary>
        /// <param name="RequestStatus">A request status to compare with.</param>
        public Int32 CompareTo(RequestStatus RequestStatus)

            => String.Compare(InternalId,
                              RequestStatus.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<RequestStatus> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two request status for equality.
        /// </summary>
        /// <param name="Object">A request status to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is RequestStatus requestStatus &&
                   Equals(requestStatus);

        #endregion

        #region Equals(RequestStatus)

        /// <summary>
        /// Compares two request status for equality.
        /// </summary>
        /// <param name="RequestStatus">A request status to compare with.</param>
        public Boolean Equals(RequestStatus RequestStatus)

            => String.Equals(InternalId,
                             RequestStatus.InternalId,
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
