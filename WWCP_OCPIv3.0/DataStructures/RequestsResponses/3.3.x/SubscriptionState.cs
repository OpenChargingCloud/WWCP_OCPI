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

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv3_0
{

    /// <summary>
    /// The subscription state
    /// </summary>
    public readonly struct SubscriptionState : IEquatable<SubscriptionState>,
                                               IComparable<SubscriptionState>
    {

        #region Data

        /// <summary>
        /// The default JSON-LD context of this object.
        /// </summary>
        public static readonly JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/contexts/OCPI/3.0/subscriptionState");

        #endregion

        #region Properties

        /// <summary>
        /// The number of seconds that the sender Platform waits before retrying a request
        /// for this subscription the first time.
        /// </summary>
        [Mandatory]
        public TimeSpan  RetryInterval       { get; }

        /// <summary>
        /// The number of updates to Party Issued Objects that the sender is storing in its
        /// persistent queue for this particular subscription at the time that this response
        /// is generated.
        /// </summary>
        [Mandatory]
        public UInt32    CurrentQueueSize    { get; }

        /// <summary>
        /// The maximum number of updates to Party Issued Objects that the sender promises
        /// to queue for the receiver for this particular subscription.
        /// </summary>
        [Mandatory]
        public UInt32    MaxQueueSize        { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new subscription state.
        /// </summary>
        /// <param name="RetryInterval">The number of seconds that the sender Platform waits before retrying a request for this subscription the first time.</param>
        /// <param name="CurrentQueueSize">The number of updates to Party Issued Objects that the sender is storing in its persistent queue for this particular subscription at the time that this response is generated.</param>
        /// <param name="MaxQueueSize">The maximum number of updates to Party Issued Objects that the sender promises to queue for the receiver for this particular subscription.</param>
        public SubscriptionState(TimeSpan  RetryInterval,
                                 UInt32    CurrentQueueSize,
                                 UInt32    MaxQueueSize)

        {

            this.RetryInterval     = RetryInterval;
            this.CurrentQueueSize  = CurrentQueueSize;
            this.MaxQueueSize      = MaxQueueSize;

            unchecked
            {

                hashCode = RetryInterval.   GetHashCode() * 5 ^
                           CurrentQueueSize.GetHashCode() * 3 ^
                           MaxQueueSize.    GetHashCode();

            }

        }

        #endregion


        #region (static) Parse   (JSON, ...)

        /// <summary>
        /// Parse the given JSON representation of a subscription state.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomSubscriptionStateParser">A delegate to parse custom subscription state JSON objects.</param>
        public static SubscriptionState Parse(JObject                                          JSON,
                                              CustomJObjectParserDelegate<SubscriptionState>?  CustomSubscriptionStateParser   = null)
        {

            if (TryParse(JSON,
                         out var subscriptionState,
                         out var errorResponse,
                         CustomSubscriptionStateParser))
            {
                return subscriptionState;
            }

            throw new ArgumentException("The given JSON representation of a subscription state is invalid: " + errorResponse, nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out SubscriptionState, out ErrorResponse, ...)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a subscription state.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="SubscriptionState">The parsed subscription state.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                     JSON,
                                       [NotNullWhen(true)]  out SubscriptionState  SubscriptionState,
                                       [NotNullWhen(false)] out String?            ErrorResponse)

            => TryParse(JSON,
                        out SubscriptionState,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a subscription state.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="SubscriptionState">The parsed subscription state.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomSubscriptionStateParser">A delegate to parse custom subscription state JSON objects.</param>
        public static Boolean TryParse(JObject                                          JSON,
                                       [NotNullWhen(true)]  out SubscriptionState       SubscriptionState,
                                       [NotNullWhen(false)] out String?                 ErrorResponse,
                                       CustomJObjectParserDelegate<SubscriptionState>?  CustomSubscriptionStateParser)
        {

            try
            {

                SubscriptionState = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse RetryInterval       [mandatory]

                if (!JSON.ParseMandatory("retry_interval",
                                         "retry interval",
                                         out TimeSpan RetryInterval,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse CurrentQueueSize    [mandatory]

                if (!JSON.ParseMandatory("current_queue_size",
                                         "current queue size",
                                         out UInt32 CurrentQueueSize,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse MaxQueueSize        [mandatory]

                if (!JSON.ParseMandatory("max_queue_size",
                                         "max queue size",
                                         out UInt32 MaxQueueSize,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                SubscriptionState = new SubscriptionState(
                                        RetryInterval,
                                        CurrentQueueSize,
                                        MaxQueueSize
                                    );


                if (CustomSubscriptionStateParser is not null)
                    SubscriptionState = CustomSubscriptionStateParser(JSON,
                                                                      SubscriptionState);

                return true;

            }
            catch (Exception e)
            {
                SubscriptionState  = default;
                ErrorResponse      = "The given JSON representation of a subscription state is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSubscriptionStateSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSubscriptionStateSerializer">A delegate to serialize custom subscription state JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SubscriptionState>?  CustomSubscriptionStateSerializer   = null)
        {

            var json = JSONObject.Create(
                           new JProperty("retry_interval",       (UInt32) RetryInterval.TotalSeconds),
                           new JProperty("current_queue_size",   CurrentQueueSize),
                           new JProperty("max_queue_size",       MaxQueueSize)
                       );

            return CustomSubscriptionStateSerializer is not null
                       ? CustomSubscriptionStateSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this subscription state.
        /// </summary>
        public SubscriptionState Clone()

            => new (
                   RetryInterval,
                   CurrentQueueSize,
                   MaxQueueSize
               );

        #endregion


        #region Operator overloading

        #region Operator == (SubscriptionState1, SubscriptionState2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SubscriptionState1">A subscription state.</param>
        /// <param name="SubscriptionState2">Another subscription state.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (SubscriptionState SubscriptionState1,
                                           SubscriptionState SubscriptionState2)

            => SubscriptionState1.Equals(SubscriptionState2);

        #endregion

        #region Operator != (SubscriptionState1, SubscriptionState2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SubscriptionState1">A subscription state.</param>
        /// <param name="SubscriptionState2">Another subscription state.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (SubscriptionState SubscriptionState1,
                                           SubscriptionState SubscriptionState2)

            => !SubscriptionState1.Equals(SubscriptionState2);

        #endregion

        #region Operator <  (SubscriptionState1, SubscriptionState2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SubscriptionState1">A subscription state.</param>
        /// <param name="SubscriptionState2">Another subscription state.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (SubscriptionState SubscriptionState1,
                                          SubscriptionState SubscriptionState2)

            => SubscriptionState1.CompareTo(SubscriptionState2) < 0;

        #endregion

        #region Operator <= (SubscriptionState1, SubscriptionState2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SubscriptionState1">A subscription state.</param>
        /// <param name="SubscriptionState2">Another subscription state.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (SubscriptionState SubscriptionState1,
                                           SubscriptionState SubscriptionState2)

            => SubscriptionState1.CompareTo(SubscriptionState2) <= 0;

        #endregion

        #region Operator >  (SubscriptionState1, SubscriptionState2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SubscriptionState1">A subscription state.</param>
        /// <param name="SubscriptionState2">Another subscription state.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (SubscriptionState SubscriptionState1,
                                          SubscriptionState SubscriptionState2)

            => SubscriptionState1.CompareTo(SubscriptionState2) > 0;

        #endregion

        #region Operator >= (SubscriptionState1, SubscriptionState2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SubscriptionState1">A subscription state.</param>
        /// <param name="SubscriptionState2">Another subscription state.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (SubscriptionState SubscriptionState1,
                                           SubscriptionState SubscriptionState2)

            => SubscriptionState1.CompareTo(SubscriptionState2) >= 0;

        #endregion

        #endregion

        #region IComparable<SubscriptionState> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two subscription states.
        /// </summary>
        /// <param name="Object">A subscription state to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is SubscriptionState subscriptionState
                   ? CompareTo(subscriptionState)
                   : throw new ArgumentException("The given object is not a subscription state!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(SubscriptionState)

        /// <summary>
        /// Compares two subscription states.
        /// </summary>
        /// <param name="SubscriptionState">A subscription state to compare with.</param>
        public Int32 CompareTo(SubscriptionState SubscriptionState)
        {

            var c = RetryInterval.   CompareTo(SubscriptionState.RetryInterval);

            if (c == 0)
                c = CurrentQueueSize.CompareTo(SubscriptionState.CurrentQueueSize);

            if (c == 0)
                c = MaxQueueSize.    CompareTo(SubscriptionState.MaxQueueSize);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<SubscriptionState> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two subscription states for equality.
        /// </summary>
        /// <param name="Object">A subscription state to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SubscriptionState subscriptionState &&
                   Equals(subscriptionState);

        #endregion

        #region Equals(SubscriptionState)

        /// <summary>
        /// Compares two subscription states for equality.
        /// </summary>
        /// <param name="SubscriptionState">A subscription state to compare with.</param>
        public Boolean Equals(SubscriptionState SubscriptionState)

            => RetryInterval.   Equals(SubscriptionState.RetryInterval)    &&
               CurrentQueueSize.Equals(SubscriptionState.CurrentQueueSize) &&
               MaxQueueSize.    Equals(SubscriptionState.MaxQueueSize);

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

            => $"{RetryInterval.TotalSeconds} sec., {CurrentQueueSize}/{MaxQueueSize}";

        #endregion

    }

}
