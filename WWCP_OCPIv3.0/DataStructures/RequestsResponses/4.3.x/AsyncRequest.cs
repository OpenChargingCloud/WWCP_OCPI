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
    /// An async request.
    /// </summary>
    public class AsyncRequest : IEquatable<AsyncRequest>,
                                IComparable<AsyncRequest>
    {

        #region Data

        /// <summary>
        /// The default JSON-LD context of this object.
        /// </summary>
        public static readonly JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/contexts/OCPI/3.0/asyncRequest");

        #endregion

        #region Properties

        /// <summary>
        /// An identifier to relate a later asynchronous response to this request.
        /// </summary>
        [Mandatory]
        public String   CallbackId    { get; }

        /// <summary>
        /// The parameters to the request to which the request sender expects an asynchronous response.
        /// </summary>
        [Optional]
        public JToken?  Payload       { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new async request.
        /// </summary>
        /// <param name="CallbackId">An identifier to relate a later asynchronous response to this request.</param>
        /// <param name="Payload">The parameters to the request to which the request sender expects an asynchronous response.</param>
        public AsyncRequest(String   CallbackId,
                            JToken?  Payload   = null)

        {

            this.CallbackId  = CallbackId;
            this.Payload     = Payload;

            unchecked
            {

                hashCode = CallbackId.GetHashCode() * 3 ^
                           Payload?.  GetHashCode() ?? 0;

            }

        }

        #endregion


        #region (static) Parse   (JSON, ...)

        /// <summary>
        /// Parse the given JSON representation of an async request.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomAsyncRequestParser">A delegate to parse custom async request JSON objects.</param>
        public static AsyncRequest Parse(JObject                                     JSON,
                                         CustomJObjectParserDelegate<AsyncRequest>?  CustomAsyncRequestParser   = null)
        {

            if (TryParse(JSON,
                         out var asyncRequest,
                         out var errorResponse,
                         CustomAsyncRequestParser))
            {
                return asyncRequest;
            }

            throw new ArgumentException("The given JSON representation of an async request is invalid: " + errorResponse, nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out AsyncRequest, out ErrorResponse, ...)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of an async request.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="AsyncRequest">The parsed async request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                 JSON,
                                       [NotNullWhen(true)]  out AsyncRequest?  AsyncRequest,
                                       [NotNullWhen(false)] out String?        ErrorResponse)

            => TryParse(JSON,
                        out AsyncRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of an async request.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="AsyncRequest">The parsed async request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomAsyncRequestParser">A delegate to parse custom async request JSON objects.</param>
        public static Boolean TryParse(JObject                                     JSON,
                                       [NotNullWhen(true)]  out AsyncRequest?      AsyncRequest,
                                       [NotNullWhen(false)] out String?            ErrorResponse,
                                       CustomJObjectParserDelegate<AsyncRequest>?  CustomAsyncRequestParser)
        {

            try
            {

                AsyncRequest = null;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse CallbackId    [mandatory]

                if (!JSON.ParseMandatoryText("callback_id",
                                             "callback identification",
                                             out String? CallbackId,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Payload       [optional]

                var Payload = JSON["payload"];

                #endregion


                AsyncRequest = new AsyncRequest(
                                   CallbackId,
                                   Payload
                               );


                if (CustomAsyncRequestParser is not null)
                    AsyncRequest = CustomAsyncRequestParser(JSON,
                                                            AsyncRequest);

                return true;

            }
            catch (Exception e)
            {
                AsyncRequest   = default;
                ErrorResponse  = "The given JSON representation of an async request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomAsyncRequestSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomAsyncRequestSerializer">A delegate to serialize custom async requests.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<AsyncRequest>? CustomAsyncRequestSerializer = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("callback_id",   CallbackId),

                           Payload is not null
                               ? new JProperty("payload",       Payload)
                               : null

                       );

            return CustomAsyncRequestSerializer is not null
                       ? CustomAsyncRequestSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this async request.
        /// </summary>
        public AsyncRequest Clone()

            => new (
                   CallbackId.CloneString(),
                   Payload is not null ? JToken.Parse(Payload.ToString()) : null
               );

        #endregion


        #region Operator overloading

        #region Operator == (AsyncRequest1, AsyncRequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AsyncRequest1">A async request.</param>
        /// <param name="AsyncRequest2">Another async request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (AsyncRequest AsyncRequest1,
                                           AsyncRequest AsyncRequest2)

            => AsyncRequest1.Equals(AsyncRequest2);

        #endregion

        #region Operator != (AsyncRequest1, AsyncRequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AsyncRequest1">A async request.</param>
        /// <param name="AsyncRequest2">Another async request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (AsyncRequest AsyncRequest1,
                                           AsyncRequest AsyncRequest2)

            => !AsyncRequest1.Equals(AsyncRequest2);

        #endregion

        #region Operator <  (AsyncRequest1, AsyncRequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AsyncRequest1">A async request.</param>
        /// <param name="AsyncRequest2">Another async request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (AsyncRequest AsyncRequest1,
                                          AsyncRequest AsyncRequest2)

            => AsyncRequest1.CompareTo(AsyncRequest2) < 0;

        #endregion

        #region Operator <= (AsyncRequest1, AsyncRequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AsyncRequest1">A async request.</param>
        /// <param name="AsyncRequest2">Another async request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (AsyncRequest AsyncRequest1,
                                           AsyncRequest AsyncRequest2)

            => AsyncRequest1.CompareTo(AsyncRequest2) <= 0;

        #endregion

        #region Operator >  (AsyncRequest1, AsyncRequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AsyncRequest1">A async request.</param>
        /// <param name="AsyncRequest2">Another async request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (AsyncRequest AsyncRequest1,
                                          AsyncRequest AsyncRequest2)

            => AsyncRequest1.CompareTo(AsyncRequest2) > 0;

        #endregion

        #region Operator >= (AsyncRequest1, AsyncRequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AsyncRequest1">A async request.</param>
        /// <param name="AsyncRequest2">Another async request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (AsyncRequest AsyncRequest1,
                                           AsyncRequest AsyncRequest2)

            => AsyncRequest1.CompareTo(AsyncRequest2) >= 0;

        #endregion

        #endregion

        #region IComparable<AsyncRequest> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two async requests.
        /// </summary>
        /// <param name="Object">An async request to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is AsyncRequest asyncRequest
                   ? CompareTo(asyncRequest)
                   : throw new ArgumentException("The given object is not an async request object!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(AsyncRequest)

        /// <summary>
        /// Compares two async requests.
        /// </summary>
        /// <param name="AsyncRequest">An async request to compare with.</param>
        public Int32 CompareTo(AsyncRequest? AsyncRequest)
        {

            if (AsyncRequest is null)
                throw new ArgumentNullException(nameof(AsyncRequest), "The given async request object must not be null!");

            var c = CallbackId.CompareTo(AsyncRequest.CallbackId);

            if (c == 0)
                c = Payload?.ToString().CompareTo(AsyncRequest.Payload?.ToString()) ?? (AsyncRequest.Payload is null ? 0 : -1);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<AsyncRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two async requests for equality.
        /// </summary>
        /// <param name="Object">An async request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is AsyncRequest asyncRequest &&
                   Equals(asyncRequest);

        #endregion

        #region Equals(AsyncRequest)

        /// <summary>
        /// Compares two async requests for equality.
        /// </summary>
        /// <param name="AsyncRequest">An async request to compare with.</param>
        public Boolean Equals(AsyncRequest? AsyncRequest)

            => AsyncRequest is not null &&

               CallbackId.Equals(AsyncRequest.CallbackId) &&

             ((Payload is     null && AsyncRequest.Payload is     null) ||
              (Payload is not null && AsyncRequest.Payload is not null && Payload.ToString().Equals(AsyncRequest.Payload.ToString())));

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

                   CallbackId,

                   Payload is not null
                       ? $", {Payload?.ToString().SubstringMax(32)}"
                       : ""

               );

        #endregion

    }

}
