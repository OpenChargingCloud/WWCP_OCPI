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

namespace cloud.charging.open.protocols.OCPIv3_0
{

    /// <summary>
    /// Extension methods for subscription renegotiation status.
    /// </summary>
    public static class SubscriptionRenegotiationStatusExtensions
    {

        /// <summary>
        /// Indicates whether this subscription renegotiation status is null or empty.
        /// </summary>
        /// <param name="SubscriptionRenegotiationStatus">A subscription renegotiation status.</param>
        public static Boolean IsNullOrEmpty(this SubscriptionRenegotiationStatus? SubscriptionRenegotiationStatus)
            => !SubscriptionRenegotiationStatus.HasValue || SubscriptionRenegotiationStatus.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this subscription renegotiation status is NOT null or empty.
        /// </summary>
        /// <param name="SubscriptionRenegotiationStatus">A subscription renegotiation status.</param>
        public static Boolean IsNotNullOrEmpty(this SubscriptionRenegotiationStatus? SubscriptionRenegotiationStatus)
            => SubscriptionRenegotiationStatus.HasValue && SubscriptionRenegotiationStatus.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A subscription renegotiation status.
    /// </summary>
    public readonly struct SubscriptionRenegotiationStatus : IId<SubscriptionRenegotiationStatus>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this subscription renegotiation status is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this subscription renegotiation status is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the subscription renegotiation status.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new subscription renegotiation status based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a subscription renegotiation status.</param>
        private SubscriptionRenegotiationStatus(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as a subscription renegotiation status.
        /// </summary>
        /// <param name="Text">A text representation of a subscription renegotiation status.</param>
        public static SubscriptionRenegotiationStatus Parse(String Text)
        {

            if (TryParse(Text, out var subscriptionRenegotiationStatus))
                return subscriptionRenegotiationStatus;

            throw new ArgumentException($"Invalid text representation of a subscription renegotiation status: '" + Text + "'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a subscription renegotiation status.
        /// </summary>
        /// <param name="Text">A text representation of a subscription renegotiation status.</param>
        public static SubscriptionRenegotiationStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var subscriptionRenegotiationStatus))
                return subscriptionRenegotiationStatus;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out SubscriptionRenegotiationStatus)

        /// <summary>
        /// Try to parse the given text as a subscription renegotiation status.
        /// </summary>
        /// <param name="Text">A text representation of a subscription renegotiation status.</param>
        /// <param name="SubscriptionRenegotiationStatus">The parsed subscription renegotiation status.</param>
        public static Boolean TryParse(String Text, out SubscriptionRenegotiationStatus SubscriptionRenegotiationStatus)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    SubscriptionRenegotiationStatus = new SubscriptionRenegotiationStatus(Text);
                    return true;
                }
                catch
                { }
            }

            SubscriptionRenegotiationStatus = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this subscription renegotiation status.
        /// </summary>
        public SubscriptionRenegotiationStatus Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// The proposed subscription parameters have been accepted and will take effect for subsequent Party Issued Object updates in the subscription.
        /// </summary>
        public static SubscriptionRenegotiationStatus  ACCEPTED    { get; }
            = new ("ACCEPTED");

        /// <summary>
        /// The proposed subscription parameters have been rejected and will not take effect for subsequent Party Issued Object updates in the subscription.
        /// </summary>
        public static SubscriptionRenegotiationStatus  REJECTED    { get; }
            = new ("REJECTED");

        #endregion


        #region Operator overloading

        #region Operator == (SubscriptionRenegotiationStatus1, SubscriptionRenegotiationStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SubscriptionRenegotiationStatus1">A subscription renegotiation status.</param>
        /// <param name="SubscriptionRenegotiationStatus2">Another subscription renegotiation status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (SubscriptionRenegotiationStatus SubscriptionRenegotiationStatus1,
                                           SubscriptionRenegotiationStatus SubscriptionRenegotiationStatus2)

            => SubscriptionRenegotiationStatus1.Equals(SubscriptionRenegotiationStatus2);

        #endregion

        #region Operator != (SubscriptionRenegotiationStatus1, SubscriptionRenegotiationStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SubscriptionRenegotiationStatus1">A subscription renegotiation status.</param>
        /// <param name="SubscriptionRenegotiationStatus2">Another subscription renegotiation status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (SubscriptionRenegotiationStatus SubscriptionRenegotiationStatus1,
                                           SubscriptionRenegotiationStatus SubscriptionRenegotiationStatus2)

            => !SubscriptionRenegotiationStatus1.Equals(SubscriptionRenegotiationStatus2);

        #endregion

        #region Operator <  (SubscriptionRenegotiationStatus1, SubscriptionRenegotiationStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SubscriptionRenegotiationStatus1">A subscription renegotiation status.</param>
        /// <param name="SubscriptionRenegotiationStatus2">Another subscription renegotiation status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (SubscriptionRenegotiationStatus SubscriptionRenegotiationStatus1,
                                          SubscriptionRenegotiationStatus SubscriptionRenegotiationStatus2)

            => SubscriptionRenegotiationStatus1.CompareTo(SubscriptionRenegotiationStatus2) < 0;

        #endregion

        #region Operator <= (SubscriptionRenegotiationStatus1, SubscriptionRenegotiationStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SubscriptionRenegotiationStatus1">A subscription renegotiation status.</param>
        /// <param name="SubscriptionRenegotiationStatus2">Another subscription renegotiation status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (SubscriptionRenegotiationStatus SubscriptionRenegotiationStatus1,
                                           SubscriptionRenegotiationStatus SubscriptionRenegotiationStatus2)

            => SubscriptionRenegotiationStatus1.CompareTo(SubscriptionRenegotiationStatus2) <= 0;

        #endregion

        #region Operator >  (SubscriptionRenegotiationStatus1, SubscriptionRenegotiationStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SubscriptionRenegotiationStatus1">A subscription renegotiation status.</param>
        /// <param name="SubscriptionRenegotiationStatus2">Another subscription renegotiation status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (SubscriptionRenegotiationStatus SubscriptionRenegotiationStatus1,
                                          SubscriptionRenegotiationStatus SubscriptionRenegotiationStatus2)

            => SubscriptionRenegotiationStatus1.CompareTo(SubscriptionRenegotiationStatus2) > 0;

        #endregion

        #region Operator >= (SubscriptionRenegotiationStatus1, SubscriptionRenegotiationStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SubscriptionRenegotiationStatus1">A subscription renegotiation status.</param>
        /// <param name="SubscriptionRenegotiationStatus2">Another subscription renegotiation status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (SubscriptionRenegotiationStatus SubscriptionRenegotiationStatus1,
                                           SubscriptionRenegotiationStatus SubscriptionRenegotiationStatus2)

            => SubscriptionRenegotiationStatus1.CompareTo(SubscriptionRenegotiationStatus2) >= 0;

        #endregion

        #endregion

        #region IComparable<SubscriptionRenegotiationStatus> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two subscription renegotiation status.
        /// </summary>
        /// <param name="Object">A subscription renegotiation status to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is SubscriptionRenegotiationStatus subscriptionRenegotiationStatus
                   ? CompareTo(subscriptionRenegotiationStatus)
                   : throw new ArgumentException("The given object is not a subscription renegotiation status!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(SubscriptionRenegotiationStatus)

        /// <summary>
        /// Compares two subscription renegotiation status.
        /// </summary>
        /// <param name="SubscriptionRenegotiationStatus">A subscription renegotiation status to compare with.</param>
        public Int32 CompareTo(SubscriptionRenegotiationStatus SubscriptionRenegotiationStatus)

            => String.Compare(InternalId,
                              SubscriptionRenegotiationStatus.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<SubscriptionRenegotiationStatus> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two subscription renegotiation status for equality.
        /// </summary>
        /// <param name="Object">A subscription renegotiation status to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SubscriptionRenegotiationStatus subscriptionRenegotiationStatus &&
                   Equals(subscriptionRenegotiationStatus);

        #endregion

        #region Equals(SubscriptionRenegotiationStatus)

        /// <summary>
        /// Compares two subscription renegotiation status for equality.
        /// </summary>
        /// <param name="SubscriptionRenegotiationStatus">A subscription renegotiation status to compare with.</param>
        public Boolean Equals(SubscriptionRenegotiationStatus SubscriptionRenegotiationStatus)

            => String.Equals(InternalId,
                             SubscriptionRenegotiationStatus.InternalId,
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
