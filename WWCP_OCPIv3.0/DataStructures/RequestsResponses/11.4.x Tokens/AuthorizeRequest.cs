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
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv3_0
{

    /// <summary>
    /// An Authorize request.
    /// </summary>
    public class AuthorizeRequest : AAsyncRequest<AuthorizeRequest>,
                                    IEquatable<AuthorizeRequest>,
                                    IComparable<AuthorizeRequest>
    {

        #region Data

        /// <summary>
        /// The default JSON-LD context of this object.
        /// </summary>
        public static readonly JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/contexts/OCPI/3.0/locations/authorizeRequest");

        #endregion

        #region Properties

        /// <summary>
        /// Token UID of the Token for which authorization is requested.
        /// </summary>
        [Mandatory]
        public Token_Id               TokenUId                 { get; }

        /// <summary>
        /// Type of the Token that authorization is requested for. By default this is RFID.s
        /// </summary>
        [Optional]
        public TokenType?             Type                     { get; }

        /// <summary>
        /// The time at which the CPO first learned of the authorization request with this token.
        /// If possible, this could be the timestamp at which a token was presented to the Charging
        /// Station on site, like when a physical RFID token was presented to an RFID reader, or or
        /// when a contract certificate was read from a vehicle. If the Charging Station does not
        /// provide the CPO with such timestamps, the CPO can use the timestamp of when they received
        /// the authorization request from the Charging Station.
        /// </summary>
        [Mandatory]
        public DateTime               PresentationTimestamp    { get; }

        /// <summary>
        /// The ID of the Location on which the CPO is starting the Charge Session that it is
        /// seeking authorization for.
        /// </summary>
        [Mandatory]
        public Location_Id            LocationId               { get; }

        /// <summary>
        /// A list of UIDs of EVSEs. If this is set, it means that the CPO is seeking authorization
        /// to start a Session on one of these EVSEs. The idea behind this field is that if a CPO
        /// receives an OCPP 1.6 Authorize request from a Charging Station, it knows that the
        /// session will happen on one of the EVSEs of that Charging Station but it will not be
        /// sure which one of those EVSEs it will be. This field allows the CPO to share its
        /// knowledge of which EVSEs the authorization request applies to with the eMSP.
        /// OCPI is using a list of EVSE UIDs here instead of a ChargingStation ID because OCPP 1.6
        /// Charge Points are not necessarily mapped one-to-one to OCPI Charging Stations.
        /// </summary>
        [Optional]
        public IEnumerable<EVSE_UId>  EVSEUIds                 { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new Authorize request.
        /// </summary>
        /// <param name="CallbackId">An identifier to relate a later asynchronous response to this request.</param>
        /// 
        /// <param name="TokenUId">Token UID of the Token for which authorization is requested.</param>
        /// <param name="PresentationTimestamp">The time at which the CPO first learned of the authorization request with this token.</param>
        /// <param name="LocationId">The ID of the Location on which the CPO is starting the Charge Session that it is seeking authorization for.</param>
        /// <param name="Type">Type of the Token that authorization is requested for. By default this is RFID.</param>
        /// <param name="EVSEUIds">A list of UIDs of EVSEs. If this is set, it means that the CPO is seeking authorization to start a Session on one of these EVSEs.</param>
        public AuthorizeRequest(String                  CallbackId,

                                Token_Id                TokenUId,
                                DateTime                PresentationTimestamp,
                                Location_Id             LocationId,
                                TokenType?              Type       = null,
                                IEnumerable<EVSE_UId>?  EVSEUIds   = null)

            : base(CallbackId)

        {

            this.TokenUId               = TokenUId;
            this.PresentationTimestamp  = PresentationTimestamp;
            this.LocationId             = LocationId;
            this.Type                   = Type;
            this.EVSEUIds               = EVSEUIds?.Distinct() ?? [];

            unchecked
            {

                hashCode = this.CallbackId.           GetHashCode()       * 13 ^
                           this.TokenUId.             GetHashCode()       * 11 ^
                           this.PresentationTimestamp.GetHashCode()       *  7 ^
                           this.LocationId.           GetHashCode()       *  5 ^
                          (this.Type?.                GetHashCode() ?? 0) *  3 ^
                           this.EVSEUIds.             CalcHashCode();

            }

        }

        #endregion


        #region (static) Parse   (JSON, ...)

        /// <summary>
        /// Parse the given JSON representation of an Authorize request.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomAuthorizeRequestParser">A delegate to parse custom Authorize request JSON objects.</param>
        public static AuthorizeRequest Parse(JObject                                         JSON,
                                             CustomJObjectParserDelegate<AuthorizeRequest>?  CustomAuthorizeRequestParser   = null)
        {

            if (TryParse(JSON,
                         out var authorizeRequest,
                         out var errorResponse,
                         CustomAuthorizeRequestParser))
            {
                return authorizeRequest;
            }

            throw new ArgumentException("The given JSON representation of an Authorize request is invalid: " + errorResponse, nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out AuthorizeRequest, out ErrorResponse, ...)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of an Authorize request.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="AuthorizeRequest">The parsed Authorize request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                     JSON,
                                       [NotNullWhen(true)]  out AuthorizeRequest?  AuthorizeRequest,
                                       [NotNullWhen(false)] out String?            ErrorResponse)

            => TryParse(JSON,
                        out AuthorizeRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of an Authorize request.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="AuthorizeRequest">The parsed Authorize request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomAuthorizeRequestParser">A delegate to parse custom Authorize request JSON objects.</param>
        public static Boolean TryParse(JObject                                         JSON,
                                       [NotNullWhen(true)]  out AuthorizeRequest?      AuthorizeRequest,
                                       [NotNullWhen(false)] out String?                ErrorResponse,
                                       CustomJObjectParserDelegate<AuthorizeRequest>?  CustomAuthorizeRequestParser)
        {

            try
            {

                AuthorizeRequest = null;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse CallbackId               [mandatory]

                if (!JSON.ParseMandatoryText("callback_id",
                                             "callback identification",
                                             out String? CallbackId,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion


                #region Parse TokenUId                 [mandatory]

                if (!JSON.ParseMandatory("token_uid",
                                         "token unique identification",
                                         Token_Id.TryParse,
                                         out Token_Id TokenUId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse PresentationTimestamp    [mandatory]

                if (!JSON.ParseMandatory("presentation_timestamp",
                                         "presentation timestamp",
                                         out DateTime PresentationTimestamp,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse LocationId               [mandatory]

                if (!JSON.ParseMandatory("location_id",
                                         "location identification",
                                         Location_Id.TryParse,
                                         out Location_Id LocationId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Type                     [optional]

                if (JSON.ParseOptional("type",
                                       "token type",
                                       TokenType.TryParse,
                                       out TokenType Type,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse EVSEUIds                 [optional]

                if (JSON.ParseOptionalHashSet("evse_uids",
                                              "EVSE unique identifications",
                                              EVSE_UId.TryParse,
                                              out HashSet<EVSE_UId> EVSEUIds,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                AuthorizeRequest = new AuthorizeRequest(

                                       CallbackId,

                                       TokenUId,
                                       PresentationTimestamp,
                                       LocationId,
                                       Type,
                                       EVSEUIds

                                   );


                if (CustomAuthorizeRequestParser is not null)
                    AuthorizeRequest = CustomAuthorizeRequestParser(JSON,
                                                                    AuthorizeRequest);

                return true;

            }
            catch (Exception e)
            {
                AuthorizeRequest  = default;
                ErrorResponse     = "The given JSON representation of an Authorize request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomAuthorizeRequestSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomAuthorizeRequestSerializer">A delegate to serialize custom Authorize requests.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<AuthorizeRequest>? CustomAuthorizeRequestSerializer = null)
        {

            var json = ToJSON(
                           JSONObject.Create(

                                     new JProperty("token_uid",                TokenUId.             ToString()),
                                     new JProperty("presentation_timestamp",   PresentationTimestamp.ToIso8601()),
                                     new JProperty("location_id",              LocationId.           ToString()),

                               Type.HasValue
                                   ? new JProperty("type",                     Type.           Value.ToString())
                                   : null,

                               EVSEUIds.Any()
                                   ? new JProperty("evse_uids",                new JArray(EVSEUIds.Select(evseUId => evseUId.ToString())))
                                   : null

                           )
                       );

            return CustomAuthorizeRequestSerializer is not null
                       ? CustomAuthorizeRequestSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this Authorize request.
        /// </summary>
        public AuthorizeRequest Clone()

            => new (

                   CallbackId.CloneString(),

                   TokenUId.  Clone(),
                   PresentationTimestamp,
                   LocationId.Clone(),
                   Type?.     Clone(),
                   EVSEUIds.  Select(evseUId => evseUId.Clone())

               );

        #endregion


        #region Operator overloading

        #region Operator == (AuthorizeRequest1, AuthorizeRequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthorizeRequest1">An Authorize request.</param>
        /// <param name="AuthorizeRequest2">Another Authorize request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (AuthorizeRequest AuthorizeRequest1,
                                           AuthorizeRequest AuthorizeRequest2)

            => AuthorizeRequest1.Equals(AuthorizeRequest2);

        #endregion

        #region Operator != (AuthorizeRequest1, AuthorizeRequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthorizeRequest1">An Authorize request.</param>
        /// <param name="AuthorizeRequest2">Another Authorize request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (AuthorizeRequest AuthorizeRequest1,
                                           AuthorizeRequest AuthorizeRequest2)

            => !AuthorizeRequest1.Equals(AuthorizeRequest2);

        #endregion

        #region Operator <  (AuthorizeRequest1, AuthorizeRequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthorizeRequest1">An Authorize request.</param>
        /// <param name="AuthorizeRequest2">Another Authorize request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (AuthorizeRequest AuthorizeRequest1,
                                          AuthorizeRequest AuthorizeRequest2)

            => AuthorizeRequest1.CompareTo(AuthorizeRequest2) < 0;

        #endregion

        #region Operator <= (AuthorizeRequest1, AuthorizeRequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthorizeRequest1">An Authorize request.</param>
        /// <param name="AuthorizeRequest2">Another Authorize request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (AuthorizeRequest AuthorizeRequest1,
                                           AuthorizeRequest AuthorizeRequest2)

            => AuthorizeRequest1.CompareTo(AuthorizeRequest2) <= 0;

        #endregion

        #region Operator >  (AuthorizeRequest1, AuthorizeRequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthorizeRequest1">An Authorize request.</param>
        /// <param name="AuthorizeRequest2">Another Authorize request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (AuthorizeRequest AuthorizeRequest1,
                                          AuthorizeRequest AuthorizeRequest2)

            => AuthorizeRequest1.CompareTo(AuthorizeRequest2) > 0;

        #endregion

        #region Operator >= (AuthorizeRequest1, AuthorizeRequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthorizeRequest1">An Authorize request.</param>
        /// <param name="AuthorizeRequest2">Another Authorize request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (AuthorizeRequest AuthorizeRequest1,
                                           AuthorizeRequest AuthorizeRequest2)

            => AuthorizeRequest1.CompareTo(AuthorizeRequest2) >= 0;

        #endregion

        #endregion

        #region IComparable<AuthorizeRequest> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two Authorize requests.
        /// </summary>
        /// <param name="Object">An Authorize request to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is AuthorizeRequest authorizeRequest
                   ? CompareTo(authorizeRequest)
                   : throw new ArgumentException("The given object is not an Authorize request object!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(AuthorizeRequest)

        /// <summary>
        /// Compares two Authorize requests.
        /// </summary>
        /// <param name="AuthorizeRequest">An Authorize request to compare with.</param>
        public Int32 CompareTo(AuthorizeRequest? AuthorizeRequest)
        {

            if (AuthorizeRequest is null)
                throw new ArgumentNullException(nameof(AuthorizeRequest), "The given Authorize request object must not be null!");

            var c = TokenUId.             CompareTo(AuthorizeRequest.TokenUId);

            if (c == 0)
                c = PresentationTimestamp.CompareTo(AuthorizeRequest.PresentationTimestamp);

            if (c == 0)
                c = LocationId.           CompareTo(AuthorizeRequest.LocationId);

            if (c == 0 && Type.HasValue && AuthorizeRequest.Type.HasValue)
                c = Type.Value.CompareTo(AuthorizeRequest.Type.Value);

            if (c == 0 && EVSEUIds is not null && AuthorizeRequest.EVSEUIds is not null)
                c = EVSEUIds.SequenceEqual(AuthorizeRequest.EVSEUIds) ? 0 : 1;

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<AuthorizeRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two Authorize requests for equality.
        /// </summary>
        /// <param name="Object">An Authorize request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is AuthorizeRequest authorizeRequest &&
                   Equals(authorizeRequest);

        #endregion

        #region Equals(AuthorizeRequest)

        /// <summary>
        /// Compares two Authorize requests for equality.
        /// </summary>
        /// <param name="AuthorizeRequest">An Authorize request to compare with.</param>
        public Boolean Equals(AuthorizeRequest? AuthorizeRequest)

            => AuthorizeRequest is not null &&

               TokenUId.             Equals(AuthorizeRequest.TokenUId)              &&
               PresentationTimestamp.Equals(AuthorizeRequest.PresentationTimestamp) &&
               LocationId.           Equals(AuthorizeRequest.LocationId)            &&

            ((!Type.    HasValue    && !AuthorizeRequest.Type.    HasValue)   ||
              (Type.    HasValue    &&  AuthorizeRequest.Type.    HasValue    && Type.Value.Equals(AuthorizeRequest.Type.Value))) &&

              (EVSEUIds is     null &&  AuthorizeRequest.EVSEUIds is     null ||
               EVSEUIds is not null &&  AuthorizeRequest.EVSEUIds is not null && EVSEUIds.SequenceEqual(AuthorizeRequest.EVSEUIds));

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

                   TokenUId.ToString(),

                   Type.HasValue
                       ? $"/{Type}"
                       : "",

                   $" @ {LocationId}",

                   EVSEUIds.Any()
                       ? $"/{EVSEUIds.AggregateWith(",")}"
                       : ""

               );

        #endregion

    }

}
