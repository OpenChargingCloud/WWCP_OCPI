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
    /// A subscription response.
    /// </summary>
    public class SubscriptionResponse
    {

        #region Properties

        /// <summary>
        /// The maximum number of updates to Party Issued Objects that the sender
        /// promises to queue for the receiver for this particular subscription.
        /// </summary>
        public UInt32  MaxQueueSize    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new subscription response.
        /// </summary>
        /// <param name="MaxQueueSize">The maximum number of updates to Party Issued Objects that the sender promises to queue for the receiver for this particular subscription.</param>
        public SubscriptionResponse(UInt32 MaxQueueSize)
        {

            this.MaxQueueSize = MaxQueueSize;

        }

        #endregion


        #region (static) Parse   (JSON, ...)

        /// <summary>
        /// Parse the given JSON as a subscription response.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        public static SubscriptionResponse Parse(JObject                                             JSON,
                                                 CustomJObjectParserDelegate<SubscriptionResponse>?  CustomSubscriptionResponseParser   = null)
        {

            if (TryParse(JSON,
                         out var subscriptionResponse,
                         out var errorResponse,
                         CustomSubscriptionResponseParser))
            {
                return subscriptionResponse;
            }

            throw new ArgumentException("The given JSON representation of a subscription response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out SubscriptionResponse, out ErrorResponse, ...)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a subscription response.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="SubscriptionResponse">The parsed subscription response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                         JSON,
                                       [NotNullWhen(true)]  out SubscriptionResponse?  SubscriptionResponse,
                                       [NotNullWhen(false)] out String?                ErrorResponse)

            => TryParse(JSON,
                        out SubscriptionResponse,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given text as a subscription response.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="SubscriptionResponse">The parsed subscription response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomSubscriptionResponseParser">A delegate to parse custom subscription response JSON objects.</param>
        public static Boolean TryParse(JObject                                             JSON,
                                       [NotNullWhen(true)]  out SubscriptionResponse?      SubscriptionResponse,
                                       [NotNullWhen(false)] out String?                    ErrorResponse,
                                       CustomJObjectParserDelegate<SubscriptionResponse>?  CustomSubscriptionResponseParser)
        {

            try
            {

                SubscriptionResponse = null;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse MaxQueueSize    [mandatory]

                if (!JSON.ParseMandatory("max_queue_size",
                                         "max queue size",
                                         out UInt32 MaxQueueSize,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                SubscriptionResponse = new SubscriptionResponse(
                                           MaxQueueSize
                                       );


                if (CustomSubscriptionResponseParser is not null)
                    SubscriptionResponse = CustomSubscriptionResponseParser(JSON,
                                                                            SubscriptionResponse);

                return true;

            }
            catch (Exception e)
            {
                SubscriptionResponse  = null;
                ErrorResponse         = "The given JSON representation of a subscription response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSubscriptionResponseSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSubscriptionResponseSerializer">A delegate to serialize custom subscription cancellation JSON responses.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SubscriptionResponse>?  CustomSubscriptionResponseSerializer   = null)
        {

            var json = JSONObject.Create(
                           new JProperty("max_queue_size", MaxQueueSize)
                       );

            return CustomSubscriptionResponseSerializer is not null
                       ? CustomSubscriptionResponseSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this subscription response.
        /// </summary>
        public SubscriptionResponse Clone()

            => new (
                   MaxQueueSize
               );

        #endregion


        #region Operator overloading

        #region Operator == (SubscriptionResponse1, SubscriptionResponse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SubscriptionResponse1">A subscription response.</param>
        /// <param name="SubscriptionResponse2">Another subscription response.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (SubscriptionResponse SubscriptionResponse1,
                                           SubscriptionResponse SubscriptionResponse2)

            => SubscriptionResponse1.Equals(SubscriptionResponse2);

        #endregion

        #region Operator != (SubscriptionResponse1, SubscriptionResponse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SubscriptionResponse1">A subscription response.</param>
        /// <param name="SubscriptionResponse2">Another subscription response.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (SubscriptionResponse SubscriptionResponse1,
                                           SubscriptionResponse SubscriptionResponse2)

            => !SubscriptionResponse1.Equals(SubscriptionResponse2);

        #endregion

        #region Operator <  (SubscriptionResponse1, SubscriptionResponse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SubscriptionResponse1">A subscription response.</param>
        /// <param name="SubscriptionResponse2">Another subscription response.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (SubscriptionResponse SubscriptionResponse1,
                                          SubscriptionResponse SubscriptionResponse2)

            => SubscriptionResponse1.CompareTo(SubscriptionResponse2) < 0;

        #endregion

        #region Operator <= (SubscriptionResponse1, SubscriptionResponse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SubscriptionResponse1">A subscription response.</param>
        /// <param name="SubscriptionResponse2">Another subscription response.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (SubscriptionResponse SubscriptionResponse1,
                                           SubscriptionResponse SubscriptionResponse2)

            => SubscriptionResponse1.CompareTo(SubscriptionResponse2) <= 0;

        #endregion

        #region Operator >  (SubscriptionResponse1, SubscriptionResponse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SubscriptionResponse1">A subscription response.</param>
        /// <param name="SubscriptionResponse2">Another subscription response.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (SubscriptionResponse SubscriptionResponse1,
                                          SubscriptionResponse SubscriptionResponse2)

            => SubscriptionResponse1.CompareTo(SubscriptionResponse2) > 0;

        #endregion

        #region Operator >= (SubscriptionResponse1, SubscriptionResponse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SubscriptionResponse1">A subscription response.</param>
        /// <param name="SubscriptionResponse2">Another subscription response.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (SubscriptionResponse SubscriptionResponse1,
                                           SubscriptionResponse SubscriptionResponse2)

            => SubscriptionResponse1.CompareTo(SubscriptionResponse2) >= 0;

        #endregion

        #endregion

        #region IComparable<SubscriptionResponse> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two subscription responses.
        /// </summary>
        /// <param name="Object">A subscription response to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is SubscriptionResponse subscriptionResponse
                   ? CompareTo(subscriptionResponse)
                   : throw new ArgumentException("The given object is not a subscription response!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(SubscriptionResponse)

        /// <summary>
        /// Compares two subscription responses.
        /// </summary>
        /// <param name="SubscriptionResponse">A subscription response to compare with.</param>
        public Int32 CompareTo(SubscriptionResponse? SubscriptionResponse)
        {

            if (SubscriptionResponse is null)
                throw new ArgumentNullException(nameof(SubscriptionResponse), "The given subscription response must not be null!");

            return MaxQueueSize.CompareTo(SubscriptionResponse.MaxQueueSize);

        }

        #endregion

        #endregion

        #region IEquatable<SubscriptionResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two subscription responses for equality.
        /// </summary>
        /// <param name="Object">A subscription response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SubscriptionResponse subscriptionResponse &&
                   Equals(subscriptionResponse);

        #endregion

        #region Equals(SubscriptionResponse)

        /// <summary>
        /// Compares two subscription responses for equality.
        /// </summary>
        /// <param name="SubscriptionResponse">A subscription response to compare with.</param>
        public Boolean Equals(SubscriptionResponse? SubscriptionResponse)

            => SubscriptionResponse is not null &&
               MaxQueueSize.Equals(SubscriptionResponse.MaxQueueSize);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()

            => MaxQueueSize.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"{MaxQueueSize} items";

        #endregion

    }

}
