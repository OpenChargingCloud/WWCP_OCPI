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

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv3_0
{

    /// <summary>
    /// The connector where a charging session is/was happening.
    /// </summary>
    public readonly struct SessionConnector : IEquatable<SessionConnector>,
                                              IComparable<SessionConnector>,
                                              IComparable
    {

        #region Properties

        /// <summary>
        /// The identification of the EVSE where a charging session is/was happening.
        /// </summary>
        [Mandatory]
        public EVSE_UId      EVSEUId        { get; }

        /// <summary>
        /// The identification of the connector where a charging session is/was happening.
        /// </summary>
        [Mandatory]
        public Connector_Id  ConnectorId    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new connector where a charging session is/was happening.
        /// </summary>
        /// <param name="EVSEUId">An identification of the EVSE where a charging session is/was happening.</param>
        /// <param name="ConnectorId">An identification of the connector where a charging session is/was happening.</param>
        public SessionConnector(EVSE_UId      EVSEUId,
                                Connector_Id  ConnectorId)
        {

            this.EVSEUId      = EVSEUId;
            this.ConnectorId  = ConnectorId;

        }

        #endregion


        #region (static) Parse   (JSON, CustomSessionConnectorParser = null)

        /// <summary>
        /// Parse the given JSON representation of a session connector.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomSessionConnectorParser">A delegate to parse custom session connector JSON objects.</param>
        public static SessionConnector Parse(JObject                                         JSON,
                                             CustomJObjectParserDelegate<SessionConnector>?  CustomSessionConnectorParser   = null)
        {

            if (TryParse(JSON,
                         out var sessionConnector,
                         out var errorResponse,
                         CustomSessionConnectorParser))
            {
                return sessionConnector;
            }

            throw new ArgumentException("The given JSON representation of a session connector is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, CustomSessionConnectorParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a session connector.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomSessionConnectorParser">A delegate to parse custom session connector JSON objects.</param>
        public static SessionConnector? TryParse(JObject                                         JSON,
                                                 CustomJObjectParserDelegate<SessionConnector>?  CustomSessionConnectorParser   = null)
        {

            if (TryParse(JSON,
                         out var sessionConnector,
                         out var errorResponse,
                         CustomSessionConnectorParser))
            {
                return sessionConnector;
            }

            return default;

        }

        #endregion

        #region (static) TryParse(JSON, out SessionConnector, out ErrorResponse, CustomSessionConnectorParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a session connector.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="SessionConnector">The parsed session connector.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                    JSON,
                                       [NotNullWhen(true)]  out SessionConnector  SessionConnector,
                                       [NotNullWhen(false)] out String?           ErrorResponse)

            => TryParse(JSON,
                        out SessionConnector,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a session connector.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="SessionConnector">The parsed session connector.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomSessionConnectorParser">A delegate to parse custom session connector JSON objects.</param>
        public static Boolean TryParse(JObject                                         JSON,
                                       [NotNullWhen(true)]  out SessionConnector       SessionConnector,
                                       [NotNullWhen(false)] out String?                ErrorResponse,
                                       CustomJObjectParserDelegate<SessionConnector>?  CustomSessionConnectorParser   = null)
        {

            try
            {

                SessionConnector = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse EVSEUId        [mandatory]

                if (!JSON.ParseMandatory("evse_uid",
                                         "session EVSE unique identification",
                                         EVSE_UId.TryParse,
                                         out EVSE_UId EVSEUId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse ConnectorId    [mandatory]

                if (!JSON.ParseMandatory("connector_id",
                                         "session connector identification",
                                         Connector_Id.TryParse,
                                         out Connector_Id ConnectorId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                SessionConnector = new SessionConnector(
                                       EVSEUId,
                                       ConnectorId
                                   );


                if (CustomSessionConnectorParser is not null)
                    SessionConnector = CustomSessionConnectorParser(JSON,
                                                                    SessionConnector);

                return true;

            }
            catch (Exception e)
            {
                SessionConnector  = default;
                ErrorResponse     = "The given JSON representation of a session connector is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSessionConnectorSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSessionConnectorSerializer">A delegate to serialize custom session connector JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SessionConnector>? CustomSessionConnectorSerializer = null)
        {

            var json = JSONObject.Create(

                           new JProperty("evse_uid",       EVSEUId.    ToString()),

                           new JProperty("connector_id",   ConnectorId.ToString())

                       );

            return CustomSessionConnectorSerializer is not null
                       ? CustomSessionConnectorSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this session connector.
        /// </summary>
        public SessionConnector Clone()

            => new (
                   EVSEUId.    Clone(),
                   ConnectorId.Clone()
               );

        #endregion


        #region Operator overloading

        #region Operator == (SessionConnector1, SessionConnector2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SessionConnector1">A session connector.</param>
        /// <param name="SessionConnector2">Another session connector.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (SessionConnector SessionConnector1,
                                           SessionConnector SessionConnector2)

            => SessionConnector1.Equals(SessionConnector2);

        #endregion

        #region Operator != (SessionConnector1, SessionConnector2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SessionConnector1">A session connector.</param>
        /// <param name="SessionConnector2">Another session connector.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (SessionConnector SessionConnector1,
                                           SessionConnector SessionConnector2)

            => !SessionConnector1.Equals(SessionConnector2);

        #endregion

        #region Operator <  (SessionConnector1, SessionConnector2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SessionConnector1">A session connector.</param>
        /// <param name="SessionConnector2">Another session connector.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (SessionConnector SessionConnector1,
                                          SessionConnector SessionConnector2)

            => SessionConnector1.CompareTo(SessionConnector2) < 0;

        #endregion

        #region Operator <= (SessionConnector1, SessionConnector2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SessionConnector1">A session connector.</param>
        /// <param name="SessionConnector2">Another session connector.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (SessionConnector SessionConnector1,
                                           SessionConnector SessionConnector2)

            => SessionConnector1.CompareTo(SessionConnector2) <= 0;

        #endregion

        #region Operator >  (SessionConnector1, SessionConnector2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SessionConnector1">A session connector.</param>
        /// <param name="SessionConnector2">Another session connector.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (SessionConnector SessionConnector1,
                                          SessionConnector SessionConnector2)

            => SessionConnector1.CompareTo(SessionConnector2) > 0;

        #endregion

        #region Operator >= (SessionConnector1, SessionConnector2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SessionConnector1">A session connector.</param>
        /// <param name="SessionConnector2">Another session connector.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (SessionConnector SessionConnector1,
                                           SessionConnector SessionConnector2)

            => SessionConnector1.CompareTo(SessionConnector2) >= 0;

        #endregion

        #endregion

        #region IComparable<SessionConnector> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two session connectors.
        /// </summary>
        /// <param name="Object">A session connector to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is SessionConnector sessionConnector
                   ? CompareTo(sessionConnector)
                   : throw new ArgumentException("The given object is not a session connector!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(SessionConnector)

        /// <summary>
        /// Compares two session connectors.
        /// </summary>
        /// <param name="SessionConnector">A session connector to compare with.</param>
        public Int32 CompareTo(SessionConnector SessionConnector)
        {

            var c = EVSEUId.    CompareTo(SessionConnector.EVSEUId);

            if (c == 0)
                c = ConnectorId.CompareTo(SessionConnector.ConnectorId);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<SessionConnector> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two session connectors for equality.
        /// </summary>
        /// <param name="Object">A session connector to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SessionConnector sessionConnector &&
                   Equals(sessionConnector);

        #endregion

        #region Equals(SessionConnector)

        /// <summary>
        /// Compares two session connectors for equality.
        /// </summary>
        /// <param name="SessionConnector">A session connector to compare with.</param>
        public Boolean Equals(SessionConnector SessionConnector)

            => EVSEUId.    Equals(SessionConnector.EVSEUId) &&
               ConnectorId.Equals(SessionConnector.ConnectorId);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return EVSEUId.    GetHashCode() * 3 ^
                       ConnectorId.GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"{EVSEUId} / {ConnectorId}";

        #endregion

    }

}
