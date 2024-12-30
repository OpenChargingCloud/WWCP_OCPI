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
    /// A OCPI subscription cancellation.
    /// </summary>
    public readonly struct SubscriptionCancellation : IEquatable<SubscriptionCancellation>,
                                                      IComparable<SubscriptionCancellation>
    {

        #region Data

        /// <summary>
        /// The default JSON-LD context of this object.
        /// </summary>
        public static readonly JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/contexts/OCPI/3.0/subscriptionCancellation");

        #endregion

        #region Properties

        /// <summary>
        /// The reason for cancelling the subscription.
        /// </summary>
        [Mandatory]
        public SubscriptionCancellationReason  Reason    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new OCPI subscription cancellation.
        /// </summary>
        /// <param name="Reason">A reason for cancelling the subscription.</param>
        public SubscriptionCancellation(SubscriptionCancellationReason Reason)
        {
            this.Reason = Reason;
        }

        #endregion


        #region (static) Parse   (JSON, ...)

        /// <summary>
        /// Parse the given JSON representation of a subscription cancellation.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomSubscriptionCancellationParser">A delegate to parse custom subscription cancellation JSON objects.</param>
        public static SubscriptionCancellation Parse(JObject                                  JSON,
                                      CustomJObjectParserDelegate<SubscriptionCancellation>?  CustomSubscriptionCancellationParser   = null)
        {

            if (TryParse(JSON,
                         out var subscriptionCancellation,
                         out var errorResponse,
                         CustomSubscriptionCancellationParser))
            {
                return subscriptionCancellation;
            }

            throw new ArgumentException("The given JSON representation of a subscription cancellation is invalid: " + errorResponse, nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out SubscriptionCancellation, out ErrorResponse, ...)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a subscription cancellation.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="SubscriptionCancellation">The parsed subscription cancellation.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                             JSON,
                                       [NotNullWhen(true)]  out SubscriptionCancellation  SubscriptionCancellation,
                                       [NotNullWhen(false)] out String?    ErrorResponse)

            => TryParse(JSON,
                        out SubscriptionCancellation,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a subscription cancellation.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="SubscriptionCancellation">The parsed subscription cancellation.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomSubscriptionCancellationParser">A delegate to parse custom subscription cancellation JSON objects.</param>
        public static Boolean TryParse(JObject                                  JSON,
                                       [NotNullWhen(true)]  out SubscriptionCancellation       SubscriptionCancellation,
                                       [NotNullWhen(false)] out String?         ErrorResponse,
                                       CustomJObjectParserDelegate<SubscriptionCancellation>?  CustomSubscriptionCancellationParser)
        {

            try
            {

                SubscriptionCancellation = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse Reason    [mandatory]

                if (!JSON.ParseMandatory("reason",
                                         "cancellation reason",
                                         SubscriptionCancellationReason.TryParse,
                                         out SubscriptionCancellationReason Reason,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                SubscriptionCancellation = new SubscriptionCancellation(
                                               Reason
                                           );


                if (CustomSubscriptionCancellationParser is not null)
                    SubscriptionCancellation = CustomSubscriptionCancellationParser(JSON,
                                                                                    SubscriptionCancellation);

                return true;

            }
            catch (Exception e)
            {
                SubscriptionCancellation  = default;
                ErrorResponse             = "The given JSON representation of a subscription cancellation is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSubscriptionCancellationSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSubscriptionCancellationSerializer">A delegate to serialize custom subscription cancellation JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SubscriptionCancellation>?  CustomSubscriptionCancellationSerializer   = null)
        {

            var json = JSONObject.Create(
                           new JProperty("reason",  Reason.ToString())
                       );

            return CustomSubscriptionCancellationSerializer is not null
                       ? CustomSubscriptionCancellationSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this object.
        /// </summary>
        public SubscriptionCancellation Clone()

            => new (
                   Reason.Clone()
               );

        #endregion


        #region Operator overloading

        #region Operator == (SubscriptionCancellation1, SubscriptionCancellation2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SubscriptionCancellation1">A subscription cancellation.</param>
        /// <param name="SubscriptionCancellation2">Another subscription cancellation.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (SubscriptionCancellation SubscriptionCancellation1,
                                           SubscriptionCancellation SubscriptionCancellation2)

            => SubscriptionCancellation1.Equals(SubscriptionCancellation2);

        #endregion

        #region Operator != (SubscriptionCancellation1, SubscriptionCancellation2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SubscriptionCancellation1">A subscription cancellation.</param>
        /// <param name="SubscriptionCancellation2">Another subscription cancellation.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (SubscriptionCancellation SubscriptionCancellation1,
                                           SubscriptionCancellation SubscriptionCancellation2)

            => !SubscriptionCancellation1.Equals(SubscriptionCancellation2);

        #endregion

        #region Operator <  (SubscriptionCancellation1, SubscriptionCancellation2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SubscriptionCancellation1">A subscription cancellation.</param>
        /// <param name="SubscriptionCancellation2">Another subscription cancellation.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (SubscriptionCancellation SubscriptionCancellation1,
                                          SubscriptionCancellation SubscriptionCancellation2)

            => SubscriptionCancellation1.CompareTo(SubscriptionCancellation2) < 0;

        #endregion

        #region Operator <= (SubscriptionCancellation1, SubscriptionCancellation2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SubscriptionCancellation1">A subscription cancellation.</param>
        /// <param name="SubscriptionCancellation2">Another subscription cancellation.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (SubscriptionCancellation SubscriptionCancellation1,
                                           SubscriptionCancellation SubscriptionCancellation2)

            => SubscriptionCancellation1.CompareTo(SubscriptionCancellation2) <= 0;

        #endregion

        #region Operator >  (SubscriptionCancellation1, SubscriptionCancellation2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SubscriptionCancellation1">A subscription cancellation.</param>
        /// <param name="SubscriptionCancellation2">Another subscription cancellation.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (SubscriptionCancellation SubscriptionCancellation1,
                                          SubscriptionCancellation SubscriptionCancellation2)

            => SubscriptionCancellation1.CompareTo(SubscriptionCancellation2) > 0;

        #endregion

        #region Operator >= (SubscriptionCancellation1, SubscriptionCancellation2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SubscriptionCancellation1">A subscription cancellation.</param>
        /// <param name="SubscriptionCancellation2">Another subscription cancellation.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (SubscriptionCancellation SubscriptionCancellation1,
                                           SubscriptionCancellation SubscriptionCancellation2)

            => SubscriptionCancellation1.CompareTo(SubscriptionCancellation2) >= 0;

        #endregion

        #endregion

        #region IComparable<SubscriptionCancellation> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two subscription cancellations.
        /// </summary>
        /// <param name="Object">A subscription cancellation to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is SubscriptionCancellation subscriptionCancellation
                   ? CompareTo(subscriptionCancellation)
                   : throw new ArgumentException("The given object is not a subscription cancellation!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(SubscriptionCancellation)

        /// <summary>
        /// Compares two subscription cancellations.
        /// </summary>
        /// <param name="SubscriptionCancellation">A subscription cancellation to compare with.</param>
        public Int32 CompareTo(SubscriptionCancellation SubscriptionCancellation)

            => Reason.CompareTo(SubscriptionCancellation.Reason);

        #endregion

        #endregion

        #region IEquatable<SubscriptionCancellation> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two subscription cancellations for equality.
        /// </summary>
        /// <param name="Object">A subscription cancellation to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SubscriptionCancellation subscriptionCancellation &&
                   Equals(subscriptionCancellation);

        #endregion

        #region Equals(SubscriptionCancellation)

        /// <summary>
        /// Compares two subscription cancellations for equality.
        /// </summary>
        /// <param name="SubscriptionCancellation">A subscription cancellation to compare with.</param>
        public Boolean Equals(SubscriptionCancellation SubscriptionCancellation)

            => Reason.Equals(SubscriptionCancellation.Reason);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        public override Int32 GetHashCode()
            => Reason.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => Reason.ToString();

        #endregion

    }

}
