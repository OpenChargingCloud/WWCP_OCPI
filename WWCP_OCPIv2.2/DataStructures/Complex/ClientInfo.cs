/*
 * Copyright (c) 2015-2023 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using System;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2_1
{

    /// <summary>
    /// A client info.
    /// </summary>
    public readonly struct ClientInfo : IEquatable<ClientInfo>,
                                        IComparable<ClientInfo>,
                                        IComparable
    {

        #region Properties

        /// <summary>
        /// Country code of the country this party is operating in, as used in the credentials exchange.
        /// </summary>
        [Mandatory]
        public CountryCode       CountryCode    { get; }

        /// <summary>
        /// CPO or eMSP ID of this party (following the 15118 ISO standard), as used in the credentials exchange.
        /// </summary>
        [Mandatory]
        public Party_Id          PartyId        { get; }

        /// <summary>
        /// The role of the connected party.
        /// </summary>
        [Mandatory]
        public Roles             Role           { get; }

        /// <summary>
        /// Status of the connection to the party.
        /// </summary>
        [Mandatory]
        public ConnectionStatus  Status         { get; }

        /// <summary>
        /// Timestamp when this client info was last updated.
        /// </summary>
        [Mandatory]
        public DateTime          LastUpdated    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new client info.
        /// </summary>
        /// <param name="CountryCode">Country code of the country this party is operating in, as used in the credentials exchange.</param>
        /// <param name="PartyId">CPO or eMSP ID of this party (following the 15118 ISO standard), as used in the credentials exchange.</param>
        /// <param name="Role">The role of the connected party.</param>
        /// <param name="Status">Status of the connection to the party.</param>
        /// <param name="LastUpdated">Optional timestamp when this client info was last updated.</param>
        public ClientInfo(CountryCode       CountryCode,
                          Party_Id          PartyId,
                          Roles             Role,
                          ConnectionStatus  Status,
                          DateTime?         LastUpdated = null)
        {

            this.CountryCode  = CountryCode;
            this.PartyId      = PartyId;
            this.Role         = Role;
            this.Status       = Status;
            this.LastUpdated  = LastUpdated ?? Timestamp.Now;
        }

        #endregion


        #region (static) Parse   (JSON, CustomClientInfoParser = null)

        /// <summary>
        /// Parse the given JSON representation of a client info.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomClientInfoParser">A delegate to parse custom energy contract JSON objects.</param>
        public static ClientInfo Parse(JObject                                  JSON,
                                       CustomJObjectParserDelegate<ClientInfo>  CustomClientInfoParser   = null)
        {

            if (TryParse(JSON,
                         out ClientInfo  clientInfo,
                         out String      ErrorResponse,
                         CustomClientInfoParser))
            {
                return clientInfo;
            }

            throw new ArgumentException("The given JSON representation of a client info is invalid: " + ErrorResponse, nameof(JSON));

        }

        #endregion

        #region (static) Parse   (Text, CustomClientInfoParser = null)

        /// <summary>
        /// Parse the given text representation of a client info.
        /// </summary>
        /// <param name="Text">The text to parse.</param>
        /// <param name="CustomClientInfoParser">A delegate to parse custom energy contract JSON objects.</param>
        public static ClientInfo Parse(String                                   Text,
                                       CustomJObjectParserDelegate<ClientInfo>  CustomClientInfoParser   = null)
        {

            if (TryParse(Text,
                         out ClientInfo  clientInfo,
                         out String      ErrorResponse,
                         CustomClientInfoParser))
            {
                return clientInfo;
            }

            throw new ArgumentException("The given text representation of a client info is invalid: " + ErrorResponse, nameof(Text));

        }

        #endregion

        #region (static) TryParse(JSON, CustomClientInfoParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a energy contract.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomClientInfoParser">A delegate to parse custom energy contract JSON objects.</param>
        public static ClientInfo? TryParse(JObject                                  JSON,
                                           CustomJObjectParserDelegate<ClientInfo>  CustomClientInfoParser   = null)
        {

            if (TryParse(JSON,
                         out ClientInfo  clientInfo,
                         out String      ErrorResponse,
                         CustomClientInfoParser))
            {
                return clientInfo;
            }

            return default;

        }

        #endregion

        #region (static) TryParse(Text, CustomClientInfoParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a energy contract.
        /// </summary>
        /// <param name="Text">The JSON to parse.</param>
        /// <param name="CustomClientInfoParser">A delegate to parse custom energy contract JSON objects.</param>
        public static ClientInfo? TryParse(String                                   Text,
                                           CustomJObjectParserDelegate<ClientInfo>  CustomClientInfoParser   = null)
        {

            if (TryParse(Text,
                         out ClientInfo  clientInfo,
                         out String      ErrorResponse,
                         CustomClientInfoParser))
            {
                return clientInfo;
            }

            return default;

        }

        #endregion

        #region (static) TryParse(JSON, out ClientInfo, out ErrorResponse, CustomClientInfoParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a client info.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="ClientInfo">The parsed energy contract.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject         JSON,
                                       out ClientInfo  ClientInfo,
                                       out String      ErrorResponse)

            => TryParse(JSON,
                        out ClientInfo,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a client info.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="ClientInfo">The parsed energy contract.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomClientInfoParser">A delegate to parse custom energy contract JSON objects.</param>
        public static Boolean TryParse(JObject                                  JSON,
                                       out ClientInfo                           ClientInfo,
                                       out String                               ErrorResponse,
                                       CustomJObjectParserDelegate<ClientInfo>  CustomClientInfoParser   = null)
        {

            try
            {

                ClientInfo = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse CountryCode       [mandatory]

                if (!JSON.ParseMandatory("country_code",
                                         "country code",
                                         OCPIv2_2_1.CountryCode.TryParse,
                                         out CountryCode CountryCode,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse PartyId           [mandatory]

                if (!JSON.ParseMandatory("party_id",
                                         "party identification",
                                         Party_Id.TryParse,
                                         out Party_Id PartyId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Role              [mandatory]

                if (!JSON.ParseMandatoryEnum("role",
                                             "client role",
                                             out Roles Role,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Status            [mandatory]

                if (!JSON.ParseMandatoryEnum("status",
                                             "client status",
                                             out ConnectionStatus Status,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse LastUpdated       [mandatory]

                if (!JSON.ParseMandatory("last_updated",
                                         "last updated",
                                         out DateTime LastUpdated,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                ClientInfo = new ClientInfo(CountryCode,
                                            PartyId,
                                            Role,
                                            Status,
                                            LastUpdated);

                if (CustomClientInfoParser is not null)
                    ClientInfo = CustomClientInfoParser(JSON,
                                                        ClientInfo);

                return true;

            }
            catch (Exception e)
            {
                ClientInfo     = default;
                ErrorResponse  = "The given JSON representation of a client info is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Text, out ClientInfo, out ErrorResponse, CustomClientInfoParser = null)

        /// <summary>
        /// Try to parse the given text representation of a client info.
        /// </summary>
        /// <param name="Text">The text to parse.</param>
        /// <param name="ClientInfo">The parsed clientInfo.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomClientInfoParser">A delegate to parse custom energy contract JSON objects.</param>
        public static Boolean TryParse(String                                   Text,
                                       out ClientInfo                           ClientInfo,
                                       out String                               ErrorResponse,
                                       CustomJObjectParserDelegate<ClientInfo>  CustomClientInfoParser   = null)
        {

            try
            {

                return TryParse(JObject.Parse(Text),
                                out ClientInfo,
                                out ErrorResponse,
                                CustomClientInfoParser);

            }
            catch (Exception e)
            {
                ClientInfo     = default;
                ErrorResponse  = "The given text representation of a client info is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomClientInfoSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomClientInfoSerializer">A delegate to serialize custom energy contract JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ClientInfo> CustomClientInfoSerializer = null)
        {

            var JSON = JSONObject.Create(
                           new JProperty("country_code",  CountryCode.ToString()),
                           new JProperty("party_id",      PartyId.    ToString()),
                           new JProperty("role",          Role.       ToString()),
                           new JProperty("status",        Status.     ToString()),
                           new JProperty("last_updated",  LastUpdated.ToIso8601())
                       );

            return CustomClientInfoSerializer is not null
                       ? CustomClientInfoSerializer(this, JSON)
                       : JSON;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this object.
        /// </summary>
        public ClientInfo Clone()

            => new (CountryCode.Clone,
                    PartyId.    Clone,
                    Role,
                    Status,
                    LastUpdated);

        #endregion


        #region Operator overloading

        #region Operator == (ClientInfo1, ClientInfo2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ClientInfo1">A client info.</param>
        /// <param name="ClientInfo2">Another client info.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ClientInfo ClientInfo1,
                                           ClientInfo ClientInfo2)

            => ClientInfo1.Equals(ClientInfo2);

        #endregion

        #region Operator != (ClientInfo1, ClientInfo2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ClientInfo1">A client info.</param>
        /// <param name="ClientInfo2">Another client info.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ClientInfo ClientInfo1,
                                           ClientInfo ClientInfo2)

            => !(ClientInfo1 == ClientInfo2);

        #endregion

        #region Operator <  (ClientInfo1, ClientInfo2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ClientInfo1">A client info.</param>
        /// <param name="ClientInfo2">Another client info.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ClientInfo ClientInfo1,
                                          ClientInfo ClientInfo2)

            => ClientInfo1.CompareTo(ClientInfo2) < 0;

        #endregion

        #region Operator <= (ClientInfo1, ClientInfo2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ClientInfo1">A client info.</param>
        /// <param name="ClientInfo2">Another client info.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ClientInfo ClientInfo1,
                                           ClientInfo ClientInfo2)

            => !(ClientInfo1 > ClientInfo2);

        #endregion

        #region Operator >  (ClientInfo1, ClientInfo2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ClientInfo1">A client info.</param>
        /// <param name="ClientInfo2">Another client info.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ClientInfo ClientInfo1,
                                          ClientInfo ClientInfo2)

            => ClientInfo1.CompareTo(ClientInfo2) > 0;

        #endregion

        #region Operator >= (ClientInfo1, ClientInfo2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ClientInfo1">A client info.</param>
        /// <param name="ClientInfo2">Another client info.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ClientInfo ClientInfo1,
                                           ClientInfo ClientInfo2)

            => !(ClientInfo1 < ClientInfo2);

        #endregion

        #endregion

        #region IComparable<ClientInfo> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is ClientInfo clientInfo
                   ? CompareTo(clientInfo)
                   : throw new ArgumentException("The given object is not a client info!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(ClientInfo)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ClientInfo">An object to compare with.</param>
        public Int32 CompareTo(ClientInfo ClientInfo)
        {

            var c = CountryCode.CompareTo(ClientInfo.CountryCode);

            if (c == 0)
                c = PartyId.    CompareTo(ClientInfo.PartyId);

            if (c == 0)
                c = Role.       CompareTo(ClientInfo.Role);

            if (c == 0)
                c = Status.     CompareTo(ClientInfo.Status);

            if (c == 0)
                c = LastUpdated.CompareTo(ClientInfo.LastUpdated);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<ClientInfo> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is ClientInfo clientInfo &&
                   Equals(clientInfo);

        #endregion

        #region Equals(ClientInfo)

        /// <summary>
        /// Compares two client infos for equality.
        /// </summary>
        /// <param name="ClientInfo">A client info to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(ClientInfo ClientInfo)

            => CountryCode.Equals(ClientInfo.CountryCode) &&
               PartyId.    Equals(ClientInfo.PartyId)     &&
               Role.       Equals(ClientInfo.Role)        &&
               Status.     Equals(ClientInfo.Status)      &&
               LastUpdated.Equals(ClientInfo.LastUpdated);

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return CountryCode.GetHashCode() * 11 ^
                       PartyId.    GetHashCode() *  7 ^
                       Role.       GetHashCode() *  5 ^
                       Status.     GetHashCode() *  3 ^
                       LastUpdated.GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(CountryCode, "*" , PartyId, ": ",
                             Role, " / ", Status,
                             " (", LastUpdated, ")");

        #endregion

    }

}
