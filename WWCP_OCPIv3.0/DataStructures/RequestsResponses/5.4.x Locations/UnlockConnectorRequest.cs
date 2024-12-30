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

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv3_0
{

    /// <summary>
    /// An UnlockConnector request.
    /// </summary>
    public class UnlockConnectorRequest : AAsyncRequest<UnlockConnectorRequest>,
                                          IEquatable<UnlockConnectorRequest>,
                                          IComparable<UnlockConnectorRequest>
    {

        #region Data

        /// <summary>
        /// The default JSON-LD context of this object.
        /// </summary>
        public static readonly JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/contexts/OCPI/3.0/locations/unlockConnectorRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The ID of the location at which a connector is to be unlocked.
        /// </summary>
        [Mandatory]
        public Location_Id   LocationId     { get; }

        /// <summary>
        /// The value of the uid field of the EVSE of this location of which it is requested to unlock the connector.
        /// </summary>
        [Mandatory]
        public EVSE_UId      EVSEUId        { get; }

        /// <summary>
        /// Identifier of the connector of this location of which it is requested to unlock.
        /// This is the identifier found in the id field of the connector object.
        /// </summary>
        [Mandatory]
        public Connector_Id  ConnectorId    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new UnlockConnector request.
        /// </summary>
        /// <param name="CallbackId">An identifier to relate a later asynchronous response to this request.</param>
        /// 
        /// <param name="LocationId">The ID of the location at which a connector is to be unlocked.</param>
        /// <param name="EVSEUId">The value of the uid field of the EVSE of this location of which it is requested to unlock the connector.</param>
        /// <param name="ConnectorId">Identifier of the connector of this location of which it is requested to unlock. This is the identifier found in the id field of the connector object.</param>
        public UnlockConnectorRequest(String        CallbackId,

                                      Location_Id   LocationId,
                                      EVSE_UId      EVSEUId,
                                      Connector_Id  ConnectorId)

            : base(CallbackId)

        {

            this.LocationId   = LocationId;
            this.EVSEUId      = EVSEUId;
            this.ConnectorId  = ConnectorId;

            unchecked
            {

                hashCode = this.LocationId. GetHashCode() * 5 ^
                           this.EVSEUId.    GetHashCode() * 3 ^
                           this.ConnectorId.GetHashCode();

            }

        }

        #endregion


        #region (static) Parse   (JSON, ...)

        /// <summary>
        /// Parse the given JSON representation of an UnlockConnector request.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomUnlockConnectorRequestParser">A delegate to parse custom UnlockConnector request JSON objects.</param>
        public static UnlockConnectorRequest Parse(JObject                                               JSON,
                                                   CustomJObjectParserDelegate<UnlockConnectorRequest>?  CustomUnlockConnectorRequestParser   = null)
        {

            if (TryParse(JSON,
                         out var unlockConnectorRequest,
                         out var errorResponse,
                         CustomUnlockConnectorRequestParser))
            {
                return unlockConnectorRequest;
            }

            throw new ArgumentException("The given JSON representation of an UnlockConnector request is invalid: " + errorResponse, nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out UnlockConnectorRequest, out ErrorResponse, ...)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of an UnlockConnector request.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="UnlockConnectorRequest">The parsed UnlockConnector request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                           JSON,
                                       [NotNullWhen(true)]  out UnlockConnectorRequest?  UnlockConnectorRequest,
                                       [NotNullWhen(false)] out String?                  ErrorResponse)

            => TryParse(JSON,
                        out UnlockConnectorRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of an UnlockConnector request.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="UnlockConnectorRequest">The parsed UnlockConnector request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomUnlockConnectorRequestParser">A delegate to parse custom UnlockConnector request JSON objects.</param>
        public static Boolean TryParse(JObject                                               JSON,
                                       [NotNullWhen(true)]  out UnlockConnectorRequest?      UnlockConnectorRequest,
                                       [NotNullWhen(false)] out String?                      ErrorResponse,
                                       CustomJObjectParserDelegate<UnlockConnectorRequest>?  CustomUnlockConnectorRequestParser)
        {

            try
            {

                UnlockConnectorRequest = null;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse CallbackId                [mandatory]

                if (!JSON.ParseMandatoryText("callback_id",
                                             "callback identification",
                                             out String? CallbackId,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion


                #region Parse LocationId                [mandatory]

                if (!JSON.ParseMandatory("location_id",
                                         "location identification",
                                         Location_Id.TryParse,
                                         out Location_Id LocationId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse EVSEUId                   [mandatory]

                if (!JSON.ParseMandatory("evse_uid",
                                         "EVSE unique identification",
                                         EVSE_UId.TryParse,
                                         out EVSE_UId EVSEUId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse ConnectorId               [mandatory]

                if (!JSON.ParseMandatory("connector_id",
                                         "connector identification",
                                         Connector_Id.TryParse,
                                         out Connector_Id ConnectorId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                UnlockConnectorRequest = new UnlockConnectorRequest(

                                             CallbackId,

                                             LocationId,
                                             EVSEUId,
                                             ConnectorId

                                         );


                if (CustomUnlockConnectorRequestParser is not null)
                    UnlockConnectorRequest = CustomUnlockConnectorRequestParser(JSON,
                                                                                UnlockConnectorRequest);

                return true;

            }
            catch (Exception e)
            {
                UnlockConnectorRequest  = default;
                ErrorResponse           = "The given JSON representation of an UnlockConnector request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomUnlockConnectorRequestSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomUnlockConnectorRequestSerializer">A delegate to serialize custom UnlockConnector requests.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<UnlockConnectorRequest>? CustomUnlockConnectorRequestSerializer = null)
        {

            var json = ToJSON(
                           JSONObject.Create(

                               new JProperty("location_id",    LocationId. ToString()),
                               new JProperty("evse_uid",       EVSEUId.    ToString()),
                               new JProperty("connector_id",   ConnectorId.ToString())

                           )
                       );

            return CustomUnlockConnectorRequestSerializer is not null
                       ? CustomUnlockConnectorRequestSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this UnlockConnector request.
        /// </summary>
        public UnlockConnectorRequest Clone()

            => new (

                   CallbackId. CloneString(),

                   LocationId. Clone(),
                   EVSEUId.    Clone(),
                   ConnectorId.Clone()

               );

        #endregion


        #region Operator overloading

        #region Operator == (UnlockConnectorRequest1, UnlockConnectorRequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="UnlockConnectorRequest1">An UnlockConnector request.</param>
        /// <param name="UnlockConnectorRequest2">Another UnlockConnector request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (UnlockConnectorRequest UnlockConnectorRequest1,
                                           UnlockConnectorRequest UnlockConnectorRequest2)

            => UnlockConnectorRequest1.Equals(UnlockConnectorRequest2);

        #endregion

        #region Operator != (UnlockConnectorRequest1, UnlockConnectorRequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="UnlockConnectorRequest1">An UnlockConnector request.</param>
        /// <param name="UnlockConnectorRequest2">Another UnlockConnector request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (UnlockConnectorRequest UnlockConnectorRequest1,
                                           UnlockConnectorRequest UnlockConnectorRequest2)

            => !UnlockConnectorRequest1.Equals(UnlockConnectorRequest2);

        #endregion

        #region Operator <  (UnlockConnectorRequest1, UnlockConnectorRequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="UnlockConnectorRequest1">An UnlockConnector request.</param>
        /// <param name="UnlockConnectorRequest2">Another UnlockConnector request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (UnlockConnectorRequest UnlockConnectorRequest1,
                                          UnlockConnectorRequest UnlockConnectorRequest2)

            => UnlockConnectorRequest1.CompareTo(UnlockConnectorRequest2) < 0;

        #endregion

        #region Operator <= (UnlockConnectorRequest1, UnlockConnectorRequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="UnlockConnectorRequest1">An UnlockConnector request.</param>
        /// <param name="UnlockConnectorRequest2">Another UnlockConnector request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (UnlockConnectorRequest UnlockConnectorRequest1,
                                           UnlockConnectorRequest UnlockConnectorRequest2)

            => UnlockConnectorRequest1.CompareTo(UnlockConnectorRequest2) <= 0;

        #endregion

        #region Operator >  (UnlockConnectorRequest1, UnlockConnectorRequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="UnlockConnectorRequest1">An UnlockConnector request.</param>
        /// <param name="UnlockConnectorRequest2">Another UnlockConnector request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (UnlockConnectorRequest UnlockConnectorRequest1,
                                          UnlockConnectorRequest UnlockConnectorRequest2)

            => UnlockConnectorRequest1.CompareTo(UnlockConnectorRequest2) > 0;

        #endregion

        #region Operator >= (UnlockConnectorRequest1, UnlockConnectorRequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="UnlockConnectorRequest1">An UnlockConnector request.</param>
        /// <param name="UnlockConnectorRequest2">Another UnlockConnector request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (UnlockConnectorRequest UnlockConnectorRequest1,
                                           UnlockConnectorRequest UnlockConnectorRequest2)

            => UnlockConnectorRequest1.CompareTo(UnlockConnectorRequest2) >= 0;

        #endregion

        #endregion

        #region IComparable<UnlockConnectorRequest> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two UnlockConnector requests.
        /// </summary>
        /// <param name="Object">An UnlockConnector request to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is UnlockConnectorRequest unlockConnectorRequest
                   ? CompareTo(unlockConnectorRequest)
                   : throw new ArgumentException("The given object is not an UnlockConnector request object!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(UnlockConnectorRequest)

        /// <summary>
        /// Compares two UnlockConnector requests.
        /// </summary>
        /// <param name="UnlockConnectorRequest">An UnlockConnector request to compare with.</param>
        public Int32 CompareTo(UnlockConnectorRequest? UnlockConnectorRequest)
        {

            if (UnlockConnectorRequest is null)
                throw new ArgumentNullException(nameof(UnlockConnectorRequest), "The given UnlockConnector request object must not be null!");

            var c = LocationId. CompareTo(UnlockConnectorRequest.LocationId);

            if (c == 0)
                c = EVSEUId.    CompareTo(UnlockConnectorRequest.EVSEUId);

            if (c == 0)
                c = ConnectorId.CompareTo(UnlockConnectorRequest.ConnectorId);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<UnlockConnectorRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two UnlockConnector requests for equality.
        /// </summary>
        /// <param name="Object">An UnlockConnector request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is UnlockConnectorRequest unlockConnectorRequest &&
                   Equals(unlockConnectorRequest);

        #endregion

        #region Equals(UnlockConnectorRequest)

        /// <summary>
        /// Compares two UnlockConnector requests for equality.
        /// </summary>
        /// <param name="UnlockConnectorRequest">An UnlockConnector request to compare with.</param>
        public Boolean Equals(UnlockConnectorRequest? UnlockConnectorRequest)

            => UnlockConnectorRequest is not null &&

               LocationId. Equals(UnlockConnectorRequest.LocationId)  &&
               EVSEUId.    Equals(UnlockConnectorRequest.EVSEUId)     &&
               ConnectorId.Equals(UnlockConnectorRequest.ConnectorId);

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

            => $"{LocationId} / {EVSEUId} / {ConnectorId}";

        #endregion

    }

}
