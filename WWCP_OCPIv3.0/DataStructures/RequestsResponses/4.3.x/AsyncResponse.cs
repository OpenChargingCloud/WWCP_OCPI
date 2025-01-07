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

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv3_0
{

    /// <summary>
    /// An async response.
    /// </summary>
    public class AsyncResponse : IEquatable<AsyncResponse>,
                                 IComparable<AsyncResponse>
    {

        #region Data

        /// <summary>
        /// The default JSON-LD context of this object.
        /// </summary>
        public static readonly JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/contexts/OCPI/3.0/asyncResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The completion status of the requested asynchronous remote procedure call.
        /// </summary>
        [Mandatory]
        public AsyncResultType  ResultType    { get; }

        /// <summary>
        /// The error that occurred during the execution of the asynchronous remote procedure call, if any.
        /// </summary>
        [Optional]
        public JToken?          Error         { get; }

        /// <summary>
        /// The result of the asynchronous remote procedure call, if any.
        /// </summary>
        [Optional]
        public JToken?          Payload       { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new async response.
        /// </summary>
        /// <param name="ResultType">The completion status of the requested asynchronous remote procedure call.</param>
        /// <param name="Error">The error that occurred during the execution of the asynchronous remote procedure call, if any.</param>
        /// <param name="Payload">The result of the asynchronous remote procedure call, if any.</param>
        public AsyncResponse(AsyncResultType  ResultType,
                             JToken?          Error     = null,
                             JToken?          Payload   = null)

        {

            this.ResultType  = ResultType;
            this.Error       = Error;
            this.Payload     = Payload;

            unchecked
            {

                hashCode = ResultType.GetHashCode()       * 5 ^
                          (Error?.    GetHashCode() ?? 0) * 3 ^
                           Payload?.  GetHashCode() ?? 0;

            }

        }

        #endregion


        #region (static) Parse   (JSON, ...)

        /// <summary>
        /// Parse the given JSON representation of an async response.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomAsyncResponseParser">A delegate to parse custom async response JSON objects.</param>
        public static AsyncResponse Parse(JObject                                      JSON,
                                          CustomJObjectParserDelegate<AsyncResponse>?  CustomAsyncResponseParser   = null)
        {

            if (TryParse(JSON,
                         out var asyncResponse,
                         out var errorResponse,
                         CustomAsyncResponseParser))
            {
                return asyncResponse;
            }

            throw new ArgumentException("The given JSON representation of an async response is invalid: " + errorResponse, nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out AsyncResponse, out ErrorResponse, ...)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of an async response.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="AsyncResponse">The parsed async response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                  JSON,
                                       [NotNullWhen(true)]  out AsyncResponse?  AsyncResponse,
                                       [NotNullWhen(false)] out String?         ErrorResponse)

            => TryParse(JSON,
                        out AsyncResponse,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of an async response.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="AsyncResponse">The parsed async response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomAsyncResponseParser">A delegate to parse custom async response JSON objects.</param>
        public static Boolean TryParse(JObject                                      JSON,
                                       [NotNullWhen(true)]  out AsyncResponse?      AsyncResponse,
                                       [NotNullWhen(false)] out String?             ErrorResponse,
                                       CustomJObjectParserDelegate<AsyncResponse>?  CustomAsyncResponseParser)
        {

            try
            {

                AsyncResponse = null;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse ResultType    [mandatory]

                if (!JSON.ParseMandatory("result_type",
                                         "result type",
                                         AsyncResultType.TryParse,
                                         out AsyncResultType ResultType,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Payload       [optional]

                var Error   = JSON["error"];

                #endregion

                #region Parse Payload       [optional]

                var Payload = JSON["payload"];

                #endregion



                AsyncResponse = new AsyncResponse(
                                    ResultType,
                                    Error,
                                    Payload
                                );


                if (CustomAsyncResponseParser is not null)
                    AsyncResponse = CustomAsyncResponseParser(JSON,
                                                              AsyncResponse);

                return true;

            }
            catch (Exception e)
            {
                AsyncResponse  = default;
                ErrorResponse  = "The given JSON representation of an async response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomAsyncResponseSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomAsyncResponseSerializer">A delegate to serialize custom async responses.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<AsyncResponse>? CustomAsyncResponseSerializer = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("result_type",   ResultType.ToString()),

                           Error is not null
                               ? new JProperty("error",         Error)
                               : null,

                           Payload is not null
                               ? new JProperty("payload",       Payload)
                               : null

                       );

            return CustomAsyncResponseSerializer is not null
                       ? CustomAsyncResponseSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this async response.
        /// </summary>
        public AsyncResponse Clone()

            => new (
                   ResultType.Clone(),
                   Error   is not null ? JToken.Parse(Error.  ToString()) : null,
                   Payload is not null ? JToken.Parse(Payload.ToString()) : null
               );

        #endregion


        #region Operator overloading

        #region Operator == (AsyncResponse1, AsyncResponse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AsyncResponse1">A async response.</param>
        /// <param name="AsyncResponse2">Another async response.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (AsyncResponse AsyncResponse1,
                                           AsyncResponse AsyncResponse2)

            => AsyncResponse1.Equals(AsyncResponse2);

        #endregion

        #region Operator != (AsyncResponse1, AsyncResponse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AsyncResponse1">A async response.</param>
        /// <param name="AsyncResponse2">Another async response.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (AsyncResponse AsyncResponse1,
                                           AsyncResponse AsyncResponse2)

            => !AsyncResponse1.Equals(AsyncResponse2);

        #endregion

        #region Operator <  (AsyncResponse1, AsyncResponse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AsyncResponse1">A async response.</param>
        /// <param name="AsyncResponse2">Another async response.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (AsyncResponse AsyncResponse1,
                                          AsyncResponse AsyncResponse2)

            => AsyncResponse1.CompareTo(AsyncResponse2) < 0;

        #endregion

        #region Operator <= (AsyncResponse1, AsyncResponse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AsyncResponse1">A async response.</param>
        /// <param name="AsyncResponse2">Another async response.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (AsyncResponse AsyncResponse1,
                                           AsyncResponse AsyncResponse2)

            => AsyncResponse1.CompareTo(AsyncResponse2) <= 0;

        #endregion

        #region Operator >  (AsyncResponse1, AsyncResponse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AsyncResponse1">A async response.</param>
        /// <param name="AsyncResponse2">Another async response.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (AsyncResponse AsyncResponse1,
                                          AsyncResponse AsyncResponse2)

            => AsyncResponse1.CompareTo(AsyncResponse2) > 0;

        #endregion

        #region Operator >= (AsyncResponse1, AsyncResponse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AsyncResponse1">A async response.</param>
        /// <param name="AsyncResponse2">Another async response.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (AsyncResponse AsyncResponse1,
                                           AsyncResponse AsyncResponse2)

            => AsyncResponse1.CompareTo(AsyncResponse2) >= 0;

        #endregion

        #endregion

        #region IComparable<AsyncResponse> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two async responses.
        /// </summary>
        /// <param name="Object">An async response to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is AsyncResponse asyncResponse
                   ? CompareTo(asyncResponse)
                   : throw new ArgumentException("The given object is not an async response object!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(AsyncResponse)

        /// <summary>
        /// Compares two async responses.
        /// </summary>
        /// <param name="AsyncResponse">An async response to compare with.</param>
        public Int32 CompareTo(AsyncResponse? AsyncResponse)
        {

            if (AsyncResponse is null)
                throw new ArgumentNullException(nameof(AsyncResponse), "The given async response object must not be null!");

            var c = ResultType.CompareTo(AsyncResponse.ResultType);

            if (c == 0)
                c = Error?.  ToString().CompareTo(AsyncResponse.Error?.  ToString()) ?? (AsyncResponse.Error   is null ? 0 : -1);

            if (c == 0)
                c = Payload?.ToString().CompareTo(AsyncResponse.Payload?.ToString()) ?? (AsyncResponse.Payload is null ? 0 : -1);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<AsyncResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two async responses for equality.
        /// </summary>
        /// <param name="Object">An async response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is AsyncResponse asyncResponse &&
                   Equals(asyncResponse);

        #endregion

        #region Equals(AsyncResponse)

        /// <summary>
        /// Compares two async responses for equality.
        /// </summary>
        /// <param name="AsyncResponse">An async response to compare with.</param>
        public Boolean Equals(AsyncResponse? AsyncResponse)

            => AsyncResponse is not null &&

               ResultType.Equals(AsyncResponse.ResultType) &&

             ((Error   is     null && AsyncResponse.Error   is     null) ||
              (Error   is not null && AsyncResponse.Error   is not null && Error.  ToString().Equals(AsyncResponse.Error.  ToString()))) &&

             ((Payload is     null && AsyncResponse.Payload is     null) ||
              (Payload is not null && AsyncResponse.Payload is not null && Payload.ToString().Equals(AsyncResponse.Payload.ToString())));

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

                   ResultType,

                   Error is not null
                       ? $", error: {Error?.ToString().SubstringMax(32)}"
                       : "",

                   Payload is not null
                       ? $", payload: {Payload?.ToString().SubstringMax(32)}"
                       : ""

               );

        #endregion

    }

}
