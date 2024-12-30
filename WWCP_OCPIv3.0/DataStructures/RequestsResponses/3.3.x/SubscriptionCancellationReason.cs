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

using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv3_0
{

    /// <summary>
    /// Extension methods for subscription cancellation reasons.
    /// </summary>
    public static class SubscriptionCancellationReasonExtensions
    {

        /// <summary>
        /// Indicates whether this subscription cancellation reason is null or empty.
        /// </summary>
        /// <param name="SubscriptionCancellationReason">A subscription cancellation reason.</param>
        public static Boolean IsNullOrEmpty(this SubscriptionCancellationReason? SubscriptionCancellationReason)
            => !SubscriptionCancellationReason.HasValue || SubscriptionCancellationReason.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this subscription cancellation reason is NOT null or empty.
        /// </summary>
        /// <param name="SubscriptionCancellationReason">A subscription cancellation reason.</param>
        public static Boolean IsNotNullOrEmpty(this SubscriptionCancellationReason? SubscriptionCancellationReason)
            => SubscriptionCancellationReason.HasValue && SubscriptionCancellationReason.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A subscription cancellation reason.
    /// </summary>
    public readonly struct SubscriptionCancellationReason : IId<SubscriptionCancellationReason>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this subscription cancellation reason is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this subscription cancellation reason is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the subscription cancellation reason.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new subscription cancellation reason based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a subscription cancellation reason.</param>
        private SubscriptionCancellationReason(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as a subscription cancellation reason.
        /// </summary>
        /// <param name="Text">A text representation of a subscription cancellation reason.</param>
        public static SubscriptionCancellationReason Parse(String Text)
        {

            if (TryParse(Text, out var subscriptionCancellationReason))
                return subscriptionCancellationReason;

            throw new ArgumentException($"Invalid text representation of a subscription cancellation reason: '" + Text + "'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a subscription cancellation reason.
        /// </summary>
        /// <param name="Text">A text representation of a subscription cancellation reason.</param>
        public static SubscriptionCancellationReason? TryParse(String Text)
        {

            if (TryParse(Text, out var subscriptionCancellationReason))
                return subscriptionCancellationReason;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out SubscriptionCancellationReason)

        /// <summary>
        /// Try to parse the given text as a subscription cancellation reason.
        /// </summary>
        /// <param name="Text">A text representation of a subscription cancellation reason.</param>
        /// <param name="SubscriptionCancellationReason">The parsed subscription cancellation reason.</param>
        public static Boolean TryParse(String Text, out SubscriptionCancellationReason SubscriptionCancellationReason)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    SubscriptionCancellationReason = new SubscriptionCancellationReason(Text);
                    return true;
                }
                catch
                { }
            }

            SubscriptionCancellationReason = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this subscription cancellation reason.
        /// </summary>
        public SubscriptionCancellationReason Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// The number of queued updates has exceeded the maximum size of the persistent queue on the data sender platform.
        /// </summary>
        public static SubscriptionCancellationReason  QUEUE_OVERFLOW              { get; }
            = new ("QUEUE_OVERFLOW");

        /// <summary>
        /// The party that was subscribed to is no longer hosted on the Platform that was subscribed to,
        /// or they are no longer acting as a sender for the module that was subscribed to.
        /// </summary>
        public static SubscriptionCancellationReason  DATA_NO_LONGER_AVAILABLE    { get; }
            = new ("DATA_NO_LONGER_AVAILABLE");

        /// <summary>
        /// A generic code for any other reason for the data sender to cancel the subscription.
        /// </summary>
        public static SubscriptionCancellationReason  OTHER                       { get; }
            = new ("OTHER");

        #endregion


        #region Operator overloading

        #region Operator == (SubscriptionCancellationReason1, SubscriptionCancellationReason2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SubscriptionCancellationReason1">A subscription cancellation reason.</param>
        /// <param name="SubscriptionCancellationReason2">Another subscription cancellation reason.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (SubscriptionCancellationReason SubscriptionCancellationReason1,
                                           SubscriptionCancellationReason SubscriptionCancellationReason2)

            => SubscriptionCancellationReason1.Equals(SubscriptionCancellationReason2);

        #endregion

        #region Operator != (SubscriptionCancellationReason1, SubscriptionCancellationReason2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SubscriptionCancellationReason1">A subscription cancellation reason.</param>
        /// <param name="SubscriptionCancellationReason2">Another subscription cancellation reason.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (SubscriptionCancellationReason SubscriptionCancellationReason1,
                                           SubscriptionCancellationReason SubscriptionCancellationReason2)

            => !SubscriptionCancellationReason1.Equals(SubscriptionCancellationReason2);

        #endregion

        #region Operator <  (SubscriptionCancellationReason1, SubscriptionCancellationReason2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SubscriptionCancellationReason1">A subscription cancellation reason.</param>
        /// <param name="SubscriptionCancellationReason2">Another subscription cancellation reason.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (SubscriptionCancellationReason SubscriptionCancellationReason1,
                                          SubscriptionCancellationReason SubscriptionCancellationReason2)

            => SubscriptionCancellationReason1.CompareTo(SubscriptionCancellationReason2) < 0;

        #endregion

        #region Operator <= (SubscriptionCancellationReason1, SubscriptionCancellationReason2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SubscriptionCancellationReason1">A subscription cancellation reason.</param>
        /// <param name="SubscriptionCancellationReason2">Another subscription cancellation reason.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (SubscriptionCancellationReason SubscriptionCancellationReason1,
                                           SubscriptionCancellationReason SubscriptionCancellationReason2)

            => SubscriptionCancellationReason1.CompareTo(SubscriptionCancellationReason2) <= 0;

        #endregion

        #region Operator >  (SubscriptionCancellationReason1, SubscriptionCancellationReason2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SubscriptionCancellationReason1">A subscription cancellation reason.</param>
        /// <param name="SubscriptionCancellationReason2">Another subscription cancellation reason.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (SubscriptionCancellationReason SubscriptionCancellationReason1,
                                          SubscriptionCancellationReason SubscriptionCancellationReason2)

            => SubscriptionCancellationReason1.CompareTo(SubscriptionCancellationReason2) > 0;

        #endregion

        #region Operator >= (SubscriptionCancellationReason1, SubscriptionCancellationReason2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SubscriptionCancellationReason1">A subscription cancellation reason.</param>
        /// <param name="SubscriptionCancellationReason2">Another subscription cancellation reason.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (SubscriptionCancellationReason SubscriptionCancellationReason1,
                                           SubscriptionCancellationReason SubscriptionCancellationReason2)

            => SubscriptionCancellationReason1.CompareTo(SubscriptionCancellationReason2) >= 0;

        #endregion

        #endregion

        #region IComparable<SubscriptionCancellationReason> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two subscription cancellation reasons.
        /// </summary>
        /// <param name="Object">A subscription cancellation reason to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is SubscriptionCancellationReason subscriptionCancellationReason
                   ? CompareTo(subscriptionCancellationReason)
                   : throw new ArgumentException("The given object is not a subscription cancellation reason!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(SubscriptionCancellationReason)

        /// <summary>
        /// Compares two subscription cancellation reasons.
        /// </summary>
        /// <param name="SubscriptionCancellationReason">A subscription cancellation reason to compare with.</param>
        public Int32 CompareTo(SubscriptionCancellationReason SubscriptionCancellationReason)

            => String.Compare(InternalId,
                              SubscriptionCancellationReason.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<SubscriptionCancellationReason> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two subscription cancellation reasons for equality.
        /// </summary>
        /// <param name="Object">A subscription cancellation reason to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SubscriptionCancellationReason subscriptionCancellationReason &&
                   Equals(subscriptionCancellationReason);

        #endregion

        #region Equals(SubscriptionCancellationReason)

        /// <summary>
        /// Compares two subscription cancellation reasons for equality.
        /// </summary>
        /// <param name="SubscriptionCancellationReason">A subscription cancellation reason to compare with.</param>
        public Boolean Equals(SubscriptionCancellationReason SubscriptionCancellationReason)

            => String.Equals(InternalId,
                             SubscriptionCancellationReason.InternalId,
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
