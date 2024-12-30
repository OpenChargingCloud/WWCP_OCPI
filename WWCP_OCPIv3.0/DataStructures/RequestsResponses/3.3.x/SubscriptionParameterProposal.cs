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
    /// A subscription parameter proposal.
    /// </summary>
    public class SubscriptionParameterProposal : IEquatable<SubscriptionParameterProposal>,
                                                 IComparable<SubscriptionParameterProposal>
    {

        #region Data

        /// <summary>
        /// The default JSON-LD context of this object.
        /// </summary>
        public static readonly JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/contexts/OCPI/3.0/subscriptionParameterProposal");

        #endregion

        #region Properties

        /// <summary>
        /// The proposed number of seconds that the sender platform SHOULD wait before
        /// retrying a request for this subscription the first time.
        /// </summary>
        [Mandatory]
        public TimeSpan  RetryInterval       { get; }

        /// <summary>
        /// The proposed maximum number of requests that are not yet responded to and
        /// have not yet timed out that the sender platform is allowed to have made to
        /// the receiver Platform for this particular subscription at any one time.
        /// If this field is not given, the sender platform SHALL act as if the value
        /// of 1 was provided.
        /// </summary>
        [Optional]
        public UInt16?   ParallelismLimit    { get; }

        /// <summary>
        /// The proposed maximum number of updates to Party Issued Objects that the
        /// sender will queue for the receiver for this particular subscription.
        /// </summary>
        [Mandatory]
        public UInt32    MaxQueueSize        { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new subscription parameter proposal.
        /// </summary>
        /// <param name="RetryInterval">The proposed number of seconds that the sender platform SHOULD wait before retrying a request for this subscription the first time.</param>
        /// <param name="MaxQueueSize">The proposed maximum number of updates to Party Issued Objects that the sender will queue for the receiver for this particular subscription.</param>
        /// <param name="ParallelismLimit">The proposed maximum number of requests that are not yet responded to and
        /// have not yet timed out that the sender platform is allowed to have made to
        /// the receiver Platform for this particular subscription at any one time.
        /// If this field is not given, the sender platform SHALL act as if the value
        /// of 1 was provided.</param>
        public SubscriptionParameterProposal(TimeSpan  RetryInterval,
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
        /// Parse the given JSON representation of subscription parameter proposal.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomSubscriptionParameterProposalParser">A delegate to parse custom subscription parameter proposal JSON objects.</param>
        public static SubscriptionParameterProposal Parse(JObject                                                      JSON,
                                                          CustomJObjectParserDelegate<SubscriptionParameterProposal>?  CustomSubscriptionParameterProposalParser   = null)
        {

            if (TryParse(JSON,
                         out var subscriptionParameterProposal,
                         out var errorResponse,
                         CustomSubscriptionParameterProposalParser))
            {
                return subscriptionParameterProposal;
            }

            throw new ArgumentException("The given JSON representation of subscription parameter proposal is invalid: " + errorResponse, nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out SubscriptionParameterProposal, out ErrorResponse, ...)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of subscription parameter proposal.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="SubscriptionParameterProposal">The parsed subscription parameter proposal.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                                  JSON,
                                       [NotNullWhen(true)]  out SubscriptionParameterProposal?  SubscriptionParameterProposal,
                                       [NotNullWhen(false)] out String?                         ErrorResponse)

            => TryParse(JSON,
                        out SubscriptionParameterProposal,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of subscription parameter proposal.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="SubscriptionParameterProposal">The parsed subscription parameter proposal.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomSubscriptionParameterProposalParser">A delegate to parse custom subscription parameter proposal JSON objects.</param>
        public static Boolean TryParse(JObject                                                      JSON,
                                       [NotNullWhen(true)]  out SubscriptionParameterProposal?      SubscriptionParameterProposal,
                                       [NotNullWhen(false)] out String?                             ErrorResponse,
                                       CustomJObjectParserDelegate<SubscriptionParameterProposal>?  CustomSubscriptionParameterProposalParser)
        {

            try
            {

                SubscriptionParameterProposal = null;

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


                SubscriptionParameterProposal = new SubscriptionParameterProposal(
                                                    RetryInterval,
                                                    MaxQueueSize,
                                                    ParallelismLimit
                                                );


                if (CustomSubscriptionParameterProposalParser is not null)
                    SubscriptionParameterProposal = CustomSubscriptionParameterProposalParser(JSON,
                                                                                              SubscriptionParameterProposal);

                return true;

            }
            catch (Exception e)
            {
                SubscriptionParameterProposal  = default;
                ErrorResponse                  = "The given JSON representation of subscription parameter proposal is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSubscriptionParameterProposalSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSubscriptionParameterProposalSerializer">A delegate to serialize custom subscription parameter proposal JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SubscriptionParameterProposal>? CustomSubscriptionParameterProposalSerializer = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("retry_interval",      RetryInterval.TotalSeconds),
                                 new JProperty("max_queue_size",      MaxQueueSize),

                           ParallelismLimit.HasValue
                               ? new JProperty("parallelism_limit",   ParallelismLimit)
                               : null

                       );

            return CustomSubscriptionParameterProposalSerializer is not null
                       ? CustomSubscriptionParameterProposalSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this subscription parameter proposal.
        /// </summary>
        public SubscriptionParameterProposal Clone()

            => new (
                   RetryInterval,
                   MaxQueueSize,
                   ParallelismLimit
               );

        #endregion


        #region Operator overloading

        #region Operator == (SubscriptionParameterProposal1, SubscriptionParameterProposal2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SubscriptionParameterProposal1">A subscription parameter proposal.</param>
        /// <param name="SubscriptionParameterProposal2">Another subscription parameter proposal.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (SubscriptionParameterProposal SubscriptionParameterProposal1,
                                           SubscriptionParameterProposal SubscriptionParameterProposal2)

            => SubscriptionParameterProposal1.Equals(SubscriptionParameterProposal2);

        #endregion

        #region Operator != (SubscriptionParameterProposal1, SubscriptionParameterProposal2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SubscriptionParameterProposal1">A subscription parameter proposal.</param>
        /// <param name="SubscriptionParameterProposal2">Another subscription parameter proposal.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (SubscriptionParameterProposal SubscriptionParameterProposal1,
                                           SubscriptionParameterProposal SubscriptionParameterProposal2)

            => !SubscriptionParameterProposal1.Equals(SubscriptionParameterProposal2);

        #endregion

        #region Operator <  (SubscriptionParameterProposal1, SubscriptionParameterProposal2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SubscriptionParameterProposal1">A subscription parameter proposal.</param>
        /// <param name="SubscriptionParameterProposal2">Another subscription parameter proposal.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (SubscriptionParameterProposal SubscriptionParameterProposal1,
                                          SubscriptionParameterProposal SubscriptionParameterProposal2)

            => SubscriptionParameterProposal1.CompareTo(SubscriptionParameterProposal2) < 0;

        #endregion

        #region Operator <= (SubscriptionParameterProposal1, SubscriptionParameterProposal2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SubscriptionParameterProposal1">A subscription parameter proposal.</param>
        /// <param name="SubscriptionParameterProposal2">Another subscription parameter proposal.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (SubscriptionParameterProposal SubscriptionParameterProposal1,
                                           SubscriptionParameterProposal SubscriptionParameterProposal2)

            => SubscriptionParameterProposal1.CompareTo(SubscriptionParameterProposal2) <= 0;

        #endregion

        #region Operator >  (SubscriptionParameterProposal1, SubscriptionParameterProposal2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SubscriptionParameterProposal1">A subscription parameter proposal.</param>
        /// <param name="SubscriptionParameterProposal2">Another subscription parameter proposal.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (SubscriptionParameterProposal SubscriptionParameterProposal1,
                                          SubscriptionParameterProposal SubscriptionParameterProposal2)

            => SubscriptionParameterProposal1.CompareTo(SubscriptionParameterProposal2) > 0;

        #endregion

        #region Operator >= (SubscriptionParameterProposal1, SubscriptionParameterProposal2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SubscriptionParameterProposal1">A subscription parameter proposal.</param>
        /// <param name="SubscriptionParameterProposal2">Another subscription parameter proposal.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (SubscriptionParameterProposal SubscriptionParameterProposal1,
                                           SubscriptionParameterProposal SubscriptionParameterProposal2)

            => SubscriptionParameterProposal1.CompareTo(SubscriptionParameterProposal2) >= 0;

        #endregion

        #endregion

        #region IComparable<SubscriptionParameterProposal> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two subscription parameter proposals.
        /// </summary>
        /// <param name="Object">A subscription parameter proposal to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is SubscriptionParameterProposal subscriptionParameterProposal
                   ? CompareTo(subscriptionParameterProposal)
                   : throw new ArgumentException("The given object is not a subscription parameter proposal object!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(SubscriptionParameterProposal)

        /// <summary>
        /// Compares two subscription parameter proposals.
        /// </summary>
        /// <param name="SubscriptionParameterProposal">A subscription parameter proposal to compare with.</param>
        public Int32 CompareTo(SubscriptionParameterProposal? SubscriptionParameterProposal)
        {

            if (SubscriptionParameterProposal is null)
                throw new ArgumentNullException(nameof(SubscriptionParameterProposal), "The given subscription parameter proposal object must not be null!");

            var c = RetryInterval.CompareTo(SubscriptionParameterProposal.RetryInterval);

            if (c == 0)
                c = MaxQueueSize.CompareTo(SubscriptionParameterProposal.MaxQueueSize);

            if (c == 0)
                c = ParallelismLimit?.CompareTo(SubscriptionParameterProposal.ParallelismLimit) ?? (SubscriptionParameterProposal.ParallelismLimit is null ? 0 : -1);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<SubscriptionParameterProposal> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two subscription parameter proposals for equality.
        /// </summary>
        /// <param name="Object">A subscription parameter proposal to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SubscriptionParameterProposal subscriptionParameterProposal &&
                   Equals(subscriptionParameterProposal);

        #endregion

        #region Equals(SubscriptionParameterProposal)

        /// <summary>
        /// Compares two subscription parameter proposals for equality.
        /// </summary>
        /// <param name="SubscriptionParameterProposal">A subscription parameter proposal to compare with.</param>
        public Boolean Equals(SubscriptionParameterProposal? SubscriptionParameterProposal)

            => SubscriptionParameterProposal is not null &&

               RetryInterval.Equals(SubscriptionParameterProposal.RetryInterval) &&
               MaxQueueSize. Equals(SubscriptionParameterProposal.MaxQueueSize)  &&

             ((ParallelismLimit is     null && SubscriptionParameterProposal.ParallelismLimit is     null) ||
              (ParallelismLimit is not null && SubscriptionParameterProposal.ParallelismLimit is not null && ParallelismLimit.Equals(SubscriptionParameterProposal.ParallelismLimit)));

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
