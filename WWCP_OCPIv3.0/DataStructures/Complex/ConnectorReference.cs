/*
 * Copyright (c) 2015-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv3_0
{

    /// <summary>
    /// The connector reference uniquely identifies a connector
    /// that a tariff association applies a tariff for.
    /// </summary>
    public readonly struct ConnectorReference : IEquatable<ConnectorReference>,
                                                IComparable<ConnectorReference>,
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
        public ConnectorReference(EVSE_UId      EVSEUId,
                                Connector_Id  ConnectorId)
        {

            this.EVSEUId      = EVSEUId;
            this.ConnectorId  = ConnectorId;

        }

        #endregion


        #region (static) Parse   (JSON, CustomConnectorReferenceParser = null)

        /// <summary>
        /// Parse the given JSON representation of a connector reference.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomConnectorReferenceParser">A delegate to parse custom connector reference JSON objects.</param>
        public static ConnectorReference Parse(JObject                                           JSON,
                                               CustomJObjectParserDelegate<ConnectorReference>?  CustomConnectorReferenceParser   = null)
        {

            if (TryParse(JSON,
                         out var connectorReference,
                         out var errorResponse,
                         CustomConnectorReferenceParser))
            {
                return connectorReference;
            }

            throw new ArgumentException("The given JSON representation of a connector reference is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, CustomConnectorReferenceParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a connector reference.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomConnectorReferenceParser">A delegate to parse custom connector reference JSON objects.</param>
        public static ConnectorReference? TryParse(JObject                                           JSON,
                                                   CustomJObjectParserDelegate<ConnectorReference>?  CustomConnectorReferenceParser   = null)
        {

            if (TryParse(JSON,
                         out var connectorReference,
                         out var errorResponse,
                         CustomConnectorReferenceParser))
            {
                return connectorReference;
            }

            return default;

        }

        #endregion

        #region (static) TryParse(JSON, out ConnectorReference, out ErrorResponse, CustomConnectorReferenceParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a connector reference.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="ConnectorReference">The parsed connector reference.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                      JSON,
                                       [NotNullWhen(true)]  out ConnectorReference  ConnectorReference,
                                       [NotNullWhen(false)] out String?             ErrorResponse)

            => TryParse(JSON,
                        out ConnectorReference,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a connector reference.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="ConnectorReference">The parsed connector reference.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomConnectorReferenceParser">A delegate to parse custom connector reference JSON objects.</param>
        public static Boolean TryParse(JObject                                           JSON,
                                       [NotNullWhen(true)]  out ConnectorReference       ConnectorReference,
                                       [NotNullWhen(false)] out String?                  ErrorResponse,
                                       CustomJObjectParserDelegate<ConnectorReference>?  CustomConnectorReferenceParser   = null)
        {

            try
            {

                ConnectorReference = default;

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
                                         "connector reference identification",
                                         Connector_Id.TryParse,
                                         out Connector_Id ConnectorId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                ConnectorReference = new ConnectorReference(
                                         EVSEUId,
                                         ConnectorId
                                     );


                if (CustomConnectorReferenceParser is not null)
                    ConnectorReference = CustomConnectorReferenceParser(JSON,
                                                                        ConnectorReference);

                return true;

            }
            catch (Exception e)
            {
                ConnectorReference  = default;
                ErrorResponse       = "The given JSON representation of a connector reference is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomConnectorReferenceSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomConnectorReferenceSerializer">A delegate to serialize custom connector reference JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ConnectorReference>? CustomConnectorReferenceSerializer = null)
        {

            var json = JSONObject.Create(

                           new JProperty("evse_uid",       EVSEUId.    ToString()),

                           new JProperty("connector_id",   ConnectorId.ToString())

                       );

            return CustomConnectorReferenceSerializer is not null
                       ? CustomConnectorReferenceSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this connector reference.
        /// </summary>
        public ConnectorReference Clone()

            => new (
                   EVSEUId.    Clone(),
                   ConnectorId.Clone()
               );

        #endregion


        #region Operator overloading

        #region Operator == (ConnectorReference1, ConnectorReference2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConnectorReference1">A connector reference.</param>
        /// <param name="ConnectorReference2">Another connector reference.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ConnectorReference ConnectorReference1,
                                           ConnectorReference ConnectorReference2)

            => ConnectorReference1.Equals(ConnectorReference2);

        #endregion

        #region Operator != (ConnectorReference1, ConnectorReference2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConnectorReference1">A connector reference.</param>
        /// <param name="ConnectorReference2">Another connector reference.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ConnectorReference ConnectorReference1,
                                           ConnectorReference ConnectorReference2)

            => !ConnectorReference1.Equals(ConnectorReference2);

        #endregion

        #region Operator <  (ConnectorReference1, ConnectorReference2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConnectorReference1">A connector reference.</param>
        /// <param name="ConnectorReference2">Another connector reference.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ConnectorReference ConnectorReference1,
                                          ConnectorReference ConnectorReference2)

            => ConnectorReference1.CompareTo(ConnectorReference2) < 0;

        #endregion

        #region Operator <= (ConnectorReference1, ConnectorReference2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConnectorReference1">A connector reference.</param>
        /// <param name="ConnectorReference2">Another connector reference.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ConnectorReference ConnectorReference1,
                                           ConnectorReference ConnectorReference2)

            => ConnectorReference1.CompareTo(ConnectorReference2) <= 0;

        #endregion

        #region Operator >  (ConnectorReference1, ConnectorReference2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConnectorReference1">A connector reference.</param>
        /// <param name="ConnectorReference2">Another connector reference.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ConnectorReference ConnectorReference1,
                                          ConnectorReference ConnectorReference2)

            => ConnectorReference1.CompareTo(ConnectorReference2) > 0;

        #endregion

        #region Operator >= (ConnectorReference1, ConnectorReference2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConnectorReference1">A connector reference.</param>
        /// <param name="ConnectorReference2">Another connector reference.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ConnectorReference ConnectorReference1,
                                           ConnectorReference ConnectorReference2)

            => ConnectorReference1.CompareTo(ConnectorReference2) >= 0;

        #endregion

        #endregion

        #region IComparable<ConnectorReference> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two connector references.
        /// </summary>
        /// <param name="Object">A connector reference to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is ConnectorReference connectorReference
                   ? CompareTo(connectorReference)
                   : throw new ArgumentException("The given object is not a connector reference!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(ConnectorReference)

        /// <summary>
        /// Compares two connector references.
        /// </summary>
        /// <param name="ConnectorReference">A connector reference to compare with.</param>
        public Int32 CompareTo(ConnectorReference ConnectorReference)
        {

            var c = EVSEUId.    CompareTo(ConnectorReference.EVSEUId);

            if (c == 0)
                c = ConnectorId.CompareTo(ConnectorReference.ConnectorId);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<ConnectorReference> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two connector references for equality.
        /// </summary>
        /// <param name="Object">A connector reference to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ConnectorReference connectorReference &&
                   Equals(connectorReference);

        #endregion

        #region Equals(ConnectorReference)

        /// <summary>
        /// Compares two connector references for equality.
        /// </summary>
        /// <param name="ConnectorReference">A connector reference to compare with.</param>
        public Boolean Equals(ConnectorReference ConnectorReference)

            => EVSEUId.    Equals(ConnectorReference.EVSEUId) &&
               ConnectorId.Equals(ConnectorReference.ConnectorId);

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
