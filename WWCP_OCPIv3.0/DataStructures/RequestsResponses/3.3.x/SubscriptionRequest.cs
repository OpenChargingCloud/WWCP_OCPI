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

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv3_0
{

    /// <summary>
    /// A subscription request.
    /// </summary>
    public class SubscriptionRequest : IEquatable<SubscriptionRequest>,
                                       IComparable<SubscriptionRequest>
    {

        #region Data

        /// <summary>
        /// The default JSON-LD context of this object.
        /// </summary>
        public static readonly JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/contexts/OCPI/3.0/subscriptionRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The number of seconds that the sender platform SHOULD wait before retrying
        /// a request for this subscription the first time.
        /// </summary>
        [Mandatory]
        public TimeSpan  RetryInterval       { get; }

        /// <summary>
        /// The maximum number of requests that are not yet responded to and have not yet
        /// timed out that the sender platform is allowed to have made to the receiver 
        /// platform for this particular subscription at any one time.If this field is not
        /// given, the sender platform SHALL act as if the value of 1 was provided.
        /// </summary>
        [Optional]
        public UInt16?   ParallelismLimit    { get; }

        /// <summary>
        /// The maximum number of updates to Party Issued Objects that the sender promises
        /// to queue for the receiver for this particular subscription.
        /// </summary>
        [Mandatory]
        public UInt32    MaxQueueSize        { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new subscription request.
        /// </summary>
        /// <param name="RetryInterval">The proposed number of seconds that the sender platform SHOULD wait before retrying a request for this subscription the first time.</param>
        /// <param name="MaxQueueSize">The proposed maximum number of updates to Party Issued Objects that the sender will queue for the receiver for this particular subscription.</param>
        /// <param name="ParallelismLimit">The maximum number of requests that are not yet responded to and have not yet
        /// timed out that the sender platform is allowed to have made to the receiver 
        /// platform for this particular subscription at any one time.If this field is not
        /// given, the sender platform SHALL act as if the value of 1 was provided.</param>
        public SubscriptionRequest(TimeSpan  RetryInterval,
                                   UInt32    MaxQueueSize,
                                   UInt16?   ParallelismLimit = null)

        {

            this.RetryInterval     = RetryInterval;
            this.MaxQueueSize      = MaxQueueSize;
            this.ParallelismLimit  = ParallelismLimit;

            unchecked
            {

                hashCode = RetryInterval.    GetHashCode() * 5 ^
                           MaxQueueSize.     GetHashCode() * 3 ^
                           ParallelismLimit?.GetHashCode() ?? 0;

            }

        }

        #endregion


        #region (static) Parse   (JSON, ...)

        /// <summary>
        /// Parse the given JSON representation of subscription request.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomSubscriptionRequestParser">A delegate to parse custom subscription request JSON objects.</param>
        public static SubscriptionRequest Parse(JObject                                            JSON,
                                                CustomJObjectParserDelegate<SubscriptionRequest>?  CustomSubscriptionRequestParser   = null)
        {

            if (TryParse(JSON,
                         out var subscriptionRequest,
                         out var errorResponse,
                         CustomSubscriptionRequestParser))
            {
                return subscriptionRequest;
            }

            throw new ArgumentException("The given JSON representation of subscription request is invalid: " + errorResponse, nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out SubscriptionRequest, out ErrorResponse, ...)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of subscription request.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="SubscriptionRequest">The parsed subscription request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                        JSON,
                                       [NotNullWhen(true)]  out SubscriptionRequest?  SubscriptionRequest,
                                       [NotNullWhen(false)] out String?               ErrorResponse)

            => TryParse(JSON,
                        out SubscriptionRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of subscription request.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="SubscriptionRequest">The parsed subscription request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomSubscriptionRequestParser">A delegate to parse custom subscription request JSON objects.</param>
        public static Boolean TryParse(JObject                                            JSON,
                                       [NotNullWhen(true)]  out SubscriptionRequest?      SubscriptionRequest,
                                       [NotNullWhen(false)] out String?                   ErrorResponse,
                                       CustomJObjectParserDelegate<SubscriptionRequest>?  CustomSubscriptionRequestParser)
        {

            try
            {

                SubscriptionRequest = null;

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

                #region Parse MaxQueueSize        [mandatory]

                if (!JSON.ParseMandatory("max_queue_size",
                                         "max queue size",
                                         out UInt32 MaxQueueSize,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse ParallelismLimit    [optional]

                if (!JSON.ParseOptional("parallelism_limit",
                                        "parallelism limit",
                                        out UInt16? ParallelismLimit,
                                        out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                SubscriptionRequest = new SubscriptionRequest(
                                          RetryInterval,
                                          MaxQueueSize,
                                          ParallelismLimit
                                      );


                if (CustomSubscriptionRequestParser is not null)
                    SubscriptionRequest = CustomSubscriptionRequestParser(JSON,
                                                                          SubscriptionRequest);

                return true;

            }
            catch (Exception e)
            {
                SubscriptionRequest  = default;
                ErrorResponse        = "The given JSON representation of subscription request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSubscriptionRequestSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSubscriptionRequestSerializer">A delegate to serialize custom subscription requests.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SubscriptionRequest>? CustomSubscriptionRequestSerializer = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("retry_interval",      RetryInterval.TotalSeconds),
                                 new JProperty("max_queue_size",      MaxQueueSize),

                           ParallelismLimit.HasValue
                               ? new JProperty("parallelism_limit",   ParallelismLimit)
                               : null

                       );

            return CustomSubscriptionRequestSerializer is not null
                       ? CustomSubscriptionRequestSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this subscription request.
        /// </summary>
        public SubscriptionRequest Clone()

            => new (
                   RetryInterval,
                   MaxQueueSize,
                   ParallelismLimit
               );

        #endregion


        #region Operator overloading

        #region Operator == (SubscriptionRequest1, SubscriptionRequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SubscriptionRequest1">A subscription request.</param>
        /// <param name="SubscriptionRequest2">Another subscription request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (SubscriptionRequest SubscriptionRequest1,
                                           SubscriptionRequest SubscriptionRequest2)

            => SubscriptionRequest1.Equals(SubscriptionRequest2);

        #endregion

        #region Operator != (SubscriptionRequest1, SubscriptionRequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SubscriptionRequest1">A subscription request.</param>
        /// <param name="SubscriptionRequest2">Another subscription request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (SubscriptionRequest SubscriptionRequest1,
                                           SubscriptionRequest SubscriptionRequest2)

            => !SubscriptionRequest1.Equals(SubscriptionRequest2);

        #endregion

        #region Operator <  (SubscriptionRequest1, SubscriptionRequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SubscriptionRequest1">A subscription request.</param>
        /// <param name="SubscriptionRequest2">Another subscription request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (SubscriptionRequest SubscriptionRequest1,
                                          SubscriptionRequest SubscriptionRequest2)

            => SubscriptionRequest1.CompareTo(SubscriptionRequest2) < 0;

        #endregion

        #region Operator <= (SubscriptionRequest1, SubscriptionRequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SubscriptionRequest1">A subscription request.</param>
        /// <param name="SubscriptionRequest2">Another subscription request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (SubscriptionRequest SubscriptionRequest1,
                                           SubscriptionRequest SubscriptionRequest2)

            => SubscriptionRequest1.CompareTo(SubscriptionRequest2) <= 0;

        #endregion

        #region Operator >  (SubscriptionRequest1, SubscriptionRequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SubscriptionRequest1">A subscription request.</param>
        /// <param name="SubscriptionRequest2">Another subscription request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (SubscriptionRequest SubscriptionRequest1,
                                          SubscriptionRequest SubscriptionRequest2)

            => SubscriptionRequest1.CompareTo(SubscriptionRequest2) > 0;

        #endregion

        #region Operator >= (SubscriptionRequest1, SubscriptionRequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SubscriptionRequest1">A subscription request.</param>
        /// <param name="SubscriptionRequest2">Another subscription request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (SubscriptionRequest SubscriptionRequest1,
                                           SubscriptionRequest SubscriptionRequest2)

            => SubscriptionRequest1.CompareTo(SubscriptionRequest2) >= 0;

        #endregion

        #endregion

        #region IComparable<SubscriptionRequest> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two subscription requests.
        /// </summary>
        /// <param name="Object">A subscription request to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is SubscriptionRequest subscriptionRequest
                   ? CompareTo(subscriptionRequest)
                   : throw new ArgumentException("The given object is not a subscription request object!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(SubscriptionRequest)

        /// <summary>
        /// Compares two subscription requests.
        /// </summary>
        /// <param name="SubscriptionRequest">A subscription request to compare with.</param>
        public Int32 CompareTo(SubscriptionRequest? SubscriptionRequest)
        {

            if (SubscriptionRequest is null)
                throw new ArgumentNullException(nameof(SubscriptionRequest), "The given subscription request object must not be null!");

            var c = RetryInterval.CompareTo(SubscriptionRequest.RetryInterval);

            if (c == 0)
                c = MaxQueueSize.CompareTo(SubscriptionRequest.MaxQueueSize);

            if (c == 0)
                c = ParallelismLimit?.CompareTo(SubscriptionRequest.ParallelismLimit) ?? (SubscriptionRequest.ParallelismLimit is null ? 0 : -1);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<SubscriptionRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two subscription requests for equality.
        /// </summary>
        /// <param name="Object">A subscription request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SubscriptionRequest subscriptionRequest &&
                   Equals(subscriptionRequest);

        #endregion

        #region Equals(SubscriptionRequest)

        /// <summary>
        /// Compares two subscription requests for equality.
        /// </summary>
        /// <param name="SubscriptionRequest">A subscription request to compare with.</param>
        public Boolean Equals(SubscriptionRequest? SubscriptionRequest)

            => SubscriptionRequest is not null &&

               RetryInterval.Equals(SubscriptionRequest.RetryInterval) &&
               MaxQueueSize. Equals(SubscriptionRequest.MaxQueueSize)  &&

             ((ParallelismLimit is     null && SubscriptionRequest.ParallelismLimit is     null) ||
              (ParallelismLimit is not null && SubscriptionRequest.ParallelismLimit is not null && ParallelismLimit.Equals(SubscriptionRequest.ParallelismLimit)));

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

            => String.Concat(

                   $"{RetryInterval.TotalSeconds} sec., max {MaxQueueSize} items queued",

                   ParallelismLimit.HasValue
                       ? $", max {ParallelismLimit} parallel requests"
                       : ""

               );

        #endregion

    }

}
