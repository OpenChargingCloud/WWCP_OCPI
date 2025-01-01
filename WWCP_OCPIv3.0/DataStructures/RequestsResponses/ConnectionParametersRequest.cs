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
    /// A connection parameters request.
    /// </summary>
    public class ConnectionParametersRequest
    {

        #region Properties

        /// <summary>
        /// The OCPI version that Platform B requests to use with Platform A.
        /// </summary>
        public String    Version           { get; }

        /// <summary>
        /// A timeout, in milliseconds, for Platform A to use when making OCPI HTTP requests to platform B.
        /// That is, after this many milliseconds have elapsed after Platform A made a request without Platform A receiving a response to it,
        /// Platform A SHOULD not expect to receive a response from Platform B to that request anymore.
        /// </summary>
        public TimeSpan  RequestTimeout    { get; }

        /// <summary>
        /// Platform B’s base URL.
        /// </summary>
        public URL       BaseURL           { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new connection parameters request.
        /// </summary>
        /// <param name="Version">The OCPI version that Platform B requests to use with Platform A.</param>
        /// <param name="RequestTimeout">A timeout, in milliseconds, for Platform A to use when making OCPI HTTP requests to platform B.
        /// That is, after this many milliseconds have elapsed after Platform A made a request without Platform A receiving a response to it,
        /// Platform A SHOULD not expect to receive a response from Platform B to that request anymore.</param>
        /// <param name="BaseURL">Platform B’s base URL.</param>
        public ConnectionParametersRequest(String    Version,
                                           TimeSpan  RequestTimeout,
                                           URL       BaseURL)
        {

            this.Version         = Version;
            this.RequestTimeout  = RequestTimeout;
            this.BaseURL         = BaseURL;

            unchecked
            {

                hashCode = this.Version.       GetHashCode() * 5 ^
                           this.RequestTimeout.GetHashCode() * 3 ^
                           this.BaseURL.       GetHashCode();

            }

        }

        #endregion


        #region (static) Parse   (JSON, ...)

        /// <summary>
        /// Parse the given JSON as a connection parameters request.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        public static ConnectionParametersRequest Parse(JObject                                                    JSON,
                                                        CustomJObjectParserDelegate<ConnectionParametersRequest>?  CustomConnectionParametersRequestParser   = null)
        {

            if (TryParse(JSON,
                         out var connectionParametersRequest,
                         out var errorResponse,
                         CustomConnectionParametersRequestParser))
            {
                return connectionParametersRequest;
            }

            throw new ArgumentException("The given JSON representation of a connection parameters request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out ConnectionParametersRequest, out ErrorResponse, ...)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a connection parameters request.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="ConnectionParametersRequest">The parsed connection parameters request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                                JSON,
                                       [NotNullWhen(true)]  out ConnectionParametersRequest?  ConnectionParametersRequest,
                                       [NotNullWhen(false)] out String?                       ErrorResponse)

            => TryParse(JSON,
                        out ConnectionParametersRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given text as a connection parameters request.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="ConnectionParametersRequest">The parsed connection parameters request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomConnectionParametersRequestParser">A delegate to parse custom connection parameters request JSON objects.</param>
        public static Boolean TryParse(JObject                                                    JSON,
                                       [NotNullWhen(true)]  out ConnectionParametersRequest?      ConnectionParametersRequest,
                                       [NotNullWhen(false)] out String?                           ErrorResponse,
                                       CustomJObjectParserDelegate<ConnectionParametersRequest>?  CustomConnectionParametersRequestParser)
        {

            try
            {

                ConnectionParametersRequest = null;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse Version           [mandatory]

                if (!JSON.ParseMandatoryText("version",
                                             "version",
                                             out String? Version,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse RequestTimeout    [mandatory]

                if (!JSON.ParseMandatory("request_timeout",
                                         "request timeout",
                                         out TimeSpan RequestTimeout,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse BaseURL           [mandatory]

                if (!JSON.ParseMandatory("base_url",
                                         "base URL",
                                         URL.TryParse,
                                         out URL BaseURL,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                ConnectionParametersRequest = new ConnectionParametersRequest(
                                                  Version,
                                                  RequestTimeout,
                                                  BaseURL
                                              );


                if (CustomConnectionParametersRequestParser is not null)
                    ConnectionParametersRequest = CustomConnectionParametersRequestParser(JSON,
                                                                                          ConnectionParametersRequest);

                return true;

            }
            catch (Exception e)
            {
                ConnectionParametersRequest  = null;
                ErrorResponse                = "The given JSON representation of a connection parameters request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomConnectionParametersRequestSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomConnectionParametersRequestSerializer">A delegate to serialize custom connection parameters requests.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ConnectionParametersRequest>? CustomConnectionParametersRequestSerializer = null)
        {

            var json = JSONObject.Create(

                           new JProperty("version",           Version),
                           new JProperty("request_timeout",   (Int32) RequestTimeout.TotalSeconds),
                           new JProperty("base_url",          BaseURL.ToString())

                       );

            return CustomConnectionParametersRequestSerializer is not null
                       ? CustomConnectionParametersRequestSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this connection parameters request.
        /// </summary>
        public ConnectionParametersRequest Clone()

            => new (
                   Version.CloneString(),
                   RequestTimeout,
                   BaseURL.Clone()
               );

        #endregion


        #region Operator overloading

        #region Operator == (ConnectionParametersRequest1, ConnectionParametersRequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConnectionParametersRequest1">A connection parameters request.</param>
        /// <param name="ConnectionParametersRequest2">Another connection parameters request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ConnectionParametersRequest ConnectionParametersRequest1,
                                           ConnectionParametersRequest ConnectionParametersRequest2)

            => ConnectionParametersRequest1.Equals(ConnectionParametersRequest2);

        #endregion

        #region Operator != (ConnectionParametersRequest1, ConnectionParametersRequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConnectionParametersRequest1">A connection parameters request.</param>
        /// <param name="ConnectionParametersRequest2">Another connection parameters request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ConnectionParametersRequest ConnectionParametersRequest1,
                                           ConnectionParametersRequest ConnectionParametersRequest2)

            => !ConnectionParametersRequest1.Equals(ConnectionParametersRequest2);

        #endregion

        #region Operator <  (ConnectionParametersRequest1, ConnectionParametersRequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConnectionParametersRequest1">A connection parameters request.</param>
        /// <param name="ConnectionParametersRequest2">Another connection parameters request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ConnectionParametersRequest ConnectionParametersRequest1,
                                          ConnectionParametersRequest ConnectionParametersRequest2)

            => ConnectionParametersRequest1.CompareTo(ConnectionParametersRequest2) < 0;

        #endregion

        #region Operator <= (ConnectionParametersRequest1, ConnectionParametersRequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConnectionParametersRequest1">A connection parameters request.</param>
        /// <param name="ConnectionParametersRequest2">Another connection parameters request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ConnectionParametersRequest ConnectionParametersRequest1,
                                           ConnectionParametersRequest ConnectionParametersRequest2)

            => ConnectionParametersRequest1.CompareTo(ConnectionParametersRequest2) <= 0;

        #endregion

        #region Operator >  (ConnectionParametersRequest1, ConnectionParametersRequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConnectionParametersRequest1">A connection parameters request.</param>
        /// <param name="ConnectionParametersRequest2">Another connection parameters request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ConnectionParametersRequest ConnectionParametersRequest1,
                                          ConnectionParametersRequest ConnectionParametersRequest2)

            => ConnectionParametersRequest1.CompareTo(ConnectionParametersRequest2) > 0;

        #endregion

        #region Operator >= (ConnectionParametersRequest1, ConnectionParametersRequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConnectionParametersRequest1">A connection parameters request.</param>
        /// <param name="ConnectionParametersRequest2">Another connection parameters request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ConnectionParametersRequest ConnectionParametersRequest1,
                                           ConnectionParametersRequest ConnectionParametersRequest2)

            => ConnectionParametersRequest1.CompareTo(ConnectionParametersRequest2) >= 0;

        #endregion

        #endregion

        #region IComparable<ConnectionParametersRequest> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two connection parameters requests.
        /// </summary>
        /// <param name="Object">A connection parameters request to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is ConnectionParametersRequest connectionParametersRequest
                   ? CompareTo(connectionParametersRequest)
                   : throw new ArgumentException("The given object is not a connection parameters request!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(ConnectionParametersRequest)

        /// <summary>
        /// Compares two connection parameters requests.
        /// </summary>
        /// <param name="ConnectionParametersRequest">A connection parameters request to compare with.</param>
        public Int32 CompareTo(ConnectionParametersRequest? ConnectionParametersRequest)
        {

            if (ConnectionParametersRequest is null)
                throw new ArgumentNullException(nameof(ConnectionParametersRequest), "The given connection parameters request must not be null!");

            var c = String.Compare(
                        Version,
                        ConnectionParametersRequest.Version,
                        StringComparison.OrdinalIgnoreCase
                    );

            if (c == 0)
                c = RequestTimeout.CompareTo(ConnectionParametersRequest.RequestTimeout);

            if (c == 0)
                c = BaseURL.CompareTo(ConnectionParametersRequest.BaseURL);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<ConnectionParametersRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two connection parameters requests for equality.
        /// </summary>
        /// <param name="Object">A connection parameters request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ConnectionParametersRequest connectionParametersRequest &&
                   Equals(connectionParametersRequest);

        #endregion

        #region Equals(ConnectionParametersRequest)

        /// <summary>
        /// Compares two connection parameters requests for equality.
        /// </summary>
        /// <param name="ConnectionParametersRequest">A connection parameters request to compare with.</param>
        public Boolean Equals(ConnectionParametersRequest? ConnectionParametersRequest)

            => ConnectionParametersRequest is not null &&

               Version.       Equals(ConnectionParametersRequest.Version, StringComparison.OrdinalIgnoreCase) &&
               RequestTimeout.Equals(ConnectionParametersRequest.RequestTimeout) &&
               BaseURL.       Equals(ConnectionParametersRequest.BaseURL);

        #endregion

        #endregion

        #region (override) GetHashCode()

        private readonly Int32 hashCode;

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()
            => hashCode;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"{Version} @ {BaseURL} ({RequestTimeout.TotalSeconds} sec.)";

        #endregion

    }

}
