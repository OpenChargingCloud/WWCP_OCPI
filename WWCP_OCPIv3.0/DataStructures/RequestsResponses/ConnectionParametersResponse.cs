/*
 * Copyright (c) 2015-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPI <https://github.com/OpenChargingCloud/WWCP_OCPI>
 *
 * Licensed under the Apache License, Auth 2.0 (the "License");
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
    /// A connection parameters response.
    /// </summary>
    public class ConnectionParametersResponse
    {

        #region Properties

        /// <summary>
        /// A timeout, in milliseconds, for Platform B to use when making OCPI HTTP requests to platform A.
        /// That is, after this many milliseconds have elapsed after Platform B made a request without Platform B receiving a response to it,
        /// Platform B SHOULD not expect to receive a response from Platform A anymore.
        /// </summary>
        public TimeSpan  RequestTimeout    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new connection parameters response.
        /// </summary>
        /// <param name="RequestTimeout">A timeout, in milliseconds, for Platform B to use when making OCPI HTTP requests to platform A.
        /// That is, after this many milliseconds have elapsed after Platform B made a request without Platform B receiving a response to it,
        /// Platform B SHOULD not expect to receive a response from Platform A anymore.</param>
        public ConnectionParametersResponse(TimeSpan RequestTimeout)
        {

            this.RequestTimeout  = RequestTimeout;

        }

        #endregion


        #region (static) Parse   (JSON, ...)

        /// <summary>
        /// Parse the given JSON as a connection parameters response.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        public static ConnectionParametersResponse Parse(JObject                                                     JSON,
                                                         CustomJObjectParserDelegate<ConnectionParametersResponse>?  CustomConnectionParametersResponseParser   = null)
        {

            if (TryParse(JSON,
                         out var connectionParametersRequest,
                         out var errorResponse,
                         CustomConnectionParametersResponseParser))
            {
                return connectionParametersRequest;
            }

            throw new ArgumentException("The given JSON representation of a connection parameters response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out ConnectionParametersResponse, out ErrorResponse, ...)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a connection parameters response.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="ConnectionParametersResponse">The parsed connection parameters response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                                 JSON,
                                       [NotNullWhen(true)]  out ConnectionParametersResponse?  ConnectionParametersResponse,
                                       [NotNullWhen(false)] out String?                        ErrorResponse)

            => TryParse(JSON,
                        out ConnectionParametersResponse,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given text as a connection parameters response.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="ConnectionParametersResponse">The parsed connection parameters response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomConnectionParametersResponseParser">A delegate to parse custom connection parameters response JSON objects.</param>
        public static Boolean TryParse(JObject                                                     JSON,
                                       [NotNullWhen(true)]  out ConnectionParametersResponse?      ConnectionParametersResponse,
                                       [NotNullWhen(false)] out String?                            ErrorResponse,
                                       CustomJObjectParserDelegate<ConnectionParametersResponse>?  CustomConnectionParametersResponseParser)
        {

            try
            {

                ConnectionParametersResponse = null;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse RequestTimeout    [mandatory]

                if (!JSON.ParseMandatory("request_timeout",
                                         "request timeout",
                                         out TimeSpan RequestTimeout,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                ConnectionParametersResponse = new ConnectionParametersResponse(
                                                   RequestTimeout
                                               );


                if (CustomConnectionParametersResponseParser is not null)
                    ConnectionParametersResponse = CustomConnectionParametersResponseParser(JSON,
                                                                                            ConnectionParametersResponse);

                return true;

            }
            catch (Exception e)
            {
                ConnectionParametersResponse  = null;
                ErrorResponse                 = "The given JSON representation of a connection parameters response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomConnectionParametersRequestSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomConnectionParametersResponseSerializer">A delegate to serialize custom subscription cancellation responses.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ConnectionParametersResponse>? CustomConnectionParametersResponseSerializer = null)
        {

            var json = JSONObject.Create(
                           new JProperty("request_timeout",  (Int32) RequestTimeout.TotalSeconds)
                       );

            return CustomConnectionParametersResponseSerializer is not null
                       ? CustomConnectionParametersResponseSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this connection parameters response.
        /// </summary>
        public ConnectionParametersResponse Clone()

            => new (
                   RequestTimeout
               );

        #endregion


        #region Operator overloading

        #region Operator == (ConnectionParametersResponse1, ConnectionParametersResponse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConnectionParametersResponse1">A connection parameters response.</param>
        /// <param name="ConnectionParametersResponse2">Another connection parameters response.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ConnectionParametersResponse ConnectionParametersResponse1,
                                           ConnectionParametersResponse ConnectionParametersResponse2)

            => ConnectionParametersResponse1.Equals(ConnectionParametersResponse2);

        #endregion

        #region Operator != (ConnectionParametersResponse1, ConnectionParametersResponse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConnectionParametersResponse1">A connection parameters response.</param>
        /// <param name="ConnectionParametersResponse2">Another connection parameters response.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ConnectionParametersResponse ConnectionParametersResponse1,
                                           ConnectionParametersResponse ConnectionParametersResponse2)

            => !ConnectionParametersResponse1.Equals(ConnectionParametersResponse2);

        #endregion

        #region Operator <  (ConnectionParametersResponse1, ConnectionParametersResponse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConnectionParametersResponse1">A connection parameters response.</param>
        /// <param name="ConnectionParametersResponse2">Another connection parameters response.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ConnectionParametersResponse ConnectionParametersResponse1,
                                          ConnectionParametersResponse ConnectionParametersResponse2)

            => ConnectionParametersResponse1.CompareTo(ConnectionParametersResponse2) < 0;

        #endregion

        #region Operator <= (ConnectionParametersResponse1, ConnectionParametersResponse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConnectionParametersResponse1">A connection parameters response.</param>
        /// <param name="ConnectionParametersResponse2">Another connection parameters response.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ConnectionParametersResponse ConnectionParametersResponse1,
                                           ConnectionParametersResponse ConnectionParametersResponse2)

            => ConnectionParametersResponse1.CompareTo(ConnectionParametersResponse2) <= 0;

        #endregion

        #region Operator >  (ConnectionParametersResponse1, ConnectionParametersResponse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConnectionParametersResponse1">A connection parameters response.</param>
        /// <param name="ConnectionParametersResponse2">Another connection parameters response.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ConnectionParametersResponse ConnectionParametersResponse1,
                                          ConnectionParametersResponse ConnectionParametersResponse2)

            => ConnectionParametersResponse1.CompareTo(ConnectionParametersResponse2) > 0;

        #endregion

        #region Operator >= (ConnectionParametersResponse1, ConnectionParametersResponse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConnectionParametersResponse1">A connection parameters response.</param>
        /// <param name="ConnectionParametersResponse2">Another connection parameters response.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ConnectionParametersResponse ConnectionParametersResponse1,
                                           ConnectionParametersResponse ConnectionParametersResponse2)

            => ConnectionParametersResponse1.CompareTo(ConnectionParametersResponse2) >= 0;

        #endregion

        #endregion

        #region IComparable<ConnectionParametersResponse> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two connection parameters responses.
        /// </summary>
        /// <param name="Object">A connection parameters response to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is ConnectionParametersResponse connectionParametersRequest
                   ? CompareTo(connectionParametersRequest)
                   : throw new ArgumentException("The given object is not a connection parameters response!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(ConnectionParametersResponse)

        /// <summary>
        /// Compares two connection parameters responses.
        /// </summary>
        /// <param name="ConnectionParametersResponse">A connection parameters response to compare with.</param>
        public Int32 CompareTo(ConnectionParametersResponse? ConnectionParametersResponse)
        {

            if (ConnectionParametersResponse is null)
                throw new ArgumentNullException(nameof(ConnectionParametersResponse), "The given connection parameters response must not be null!");

            return RequestTimeout.CompareTo(ConnectionParametersResponse.RequestTimeout);

        }

        #endregion

        #endregion

        #region IEquatable<ConnectionParametersResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two connection parameters responses for equality.
        /// </summary>
        /// <param name="Object">A connection parameters response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ConnectionParametersResponse connectionParametersRequest &&
                   Equals(connectionParametersRequest);

        #endregion

        #region Equals(ConnectionParametersResponse)

        /// <summary>
        /// Compares two connection parameters responses for equality.
        /// </summary>
        /// <param name="ConnectionParametersResponse">A connection parameters response to compare with.</param>
        public Boolean Equals(ConnectionParametersResponse? ConnectionParametersResponse)

            => ConnectionParametersResponse is not null &&
               RequestTimeout.Equals(ConnectionParametersResponse.RequestTimeout);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()

            => RequestTimeout.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"{RequestTimeout.TotalSeconds} sec.";

        #endregion

    }

}
