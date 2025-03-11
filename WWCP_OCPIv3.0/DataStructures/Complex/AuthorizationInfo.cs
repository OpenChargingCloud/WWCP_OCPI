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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv3_0
{

    /// <summary>
    /// The authorization information.
    /// </summary>
    public class AuthorizationInfo : IEquatable<AuthorizationInfo>
    {

        // ToDo: Implement RemoteParty and EMSPId correctly!

        #region Properties

        /// <summary>
        /// The status of the token, and whether charging is allowed at the optionally given charging location.
        /// </summary>
        [Mandatory]
        public AllowedType              Allowed                   { get; }

        /// <summary>
        /// The optional complete token object for which this authorization was requested.
        /// </summary>
        [Optional]
        public Token?                   Token                     { get; }

        /// <summary>
        /// The optional reference to the location if it was included in the request, and if
        /// the EV driver is allowed to charge at that location. Only the EVSEs the EV
        /// driver is allowed to charge at are returned.
        /// </summary>
        [Optional]
        public LocationReference?       Location                  { get; }

        /// <summary>
        /// The optional reference to the authorization given by the eMSP, when given, this reference
        /// will be provided in the relevant charging session and/or charge detail record.
        /// </summary>
        [Optional]
        public AuthorizationReference?  AuthorizationReference    { get; }

        /// <summary>
        /// Optional additional information to the EV driver.
        /// </summary>
        [Optional]
        public DisplayText?             Info                      { get; }

        /// <summary>
        ///  The remote party.
        /// </summary>
        [Optional, VendorExtension(VE.GraphDefined)]
        public RemoteParty?             RemoteParty               { get; }

        /// <summary>
        ///  The EMSP identification.
        /// </summary>
        [Optional, VendorExtension(VE.GraphDefined)]
        public EMSP_Id?                 EMSPId                    { get; }

        /// <summary>
        /// The runtime of the authorization.
        /// </summary>
        [Optional, VendorExtension(VE.GraphDefined)]
        public TimeSpan                 Runtime                   { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new authorization information.
        /// </summary>
        /// <param name="Allowed">A status for the token, and whether charging is allowed at the optionally given charging location.</param>
        /// <param name="Token">An optional complete token object for which this authorization was requested.</param>
        /// <param name="Location">An optional reference to the location if it was included in the request, and if the EV driver is allowed to charge at that location. Only the EVSEs the EV driver is allowed to charge at are returned.</param>
        /// <param name="AuthorizationReference">An optional reference to the authorization given by the eMSP, when given, this reference will be provided in the relevant charging session and/or charge detail record.</param>
        /// <param name="Info">An optional additional information to the EV driver.</param>
        /// <param name="RemoteParty">The remote party.</param>
        /// <param name="EMSPId">The EMSP identification.</param>
        /// <param name="Runtime">The runtime of the authorization.</param>
        public AuthorizationInfo(AllowedType              Allowed,
                                 Token?                   Token                    = null,
                                 LocationReference?       Location                 = null,
                                 AuthorizationReference?  AuthorizationReference   = null,
                                 DisplayText?             Info                     = null,
                                 RemoteParty?             RemoteParty              = null,
                                 EMSP_Id?                 EMSPId                   = null,
                                 TimeSpan?                Runtime                  = null)
        {

            this.Allowed                 = Allowed;
            this.Token                   = Token;
            this.Location                = Location;
            this.AuthorizationReference  = AuthorizationReference;
            this.Info                    = Info;
            this.RemoteParty             = RemoteParty;
            this.EMSPId                  = EMSPId;
            this.Runtime                 = Runtime ?? TimeSpan.Zero;

        }

        #endregion


        #region (static) Parse   (JSON, CustomAuthorizationInfoParser = null)

        /// <summary>
        /// Parse the given JSON representation of an authorization information.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomAuthorizationInfoParser">A delegate to parse custom authorization information JSON objects.</param>
        public static AuthorizationInfo Parse(JObject                                          JSON,
                                              RemoteParty?                                     RemoteParty                     = null,
                                              EMSP_Id?                                         EMSPId                          = null,
                                              CustomJObjectParserDelegate<AuthorizationInfo>?  CustomAuthorizationInfoParser   = null)
        {

            if (TryParse(JSON,
                         out var authorizationInfo,
                         out var errorResponse,
                         RemoteParty,
                         EMSPId,
                         CustomAuthorizationInfoParser))
            {
                return authorizationInfo!;
            }

            throw new ArgumentException("The given JSON representation of an authorization information is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out AuthorizationInfo, out ErrorResponse, CustomAuthorizationInfoParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of an authorization information.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="AuthorizationInfo">The parsed authorization information.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                 JSON,
                                       out AuthorizationInfo?  AuthorizationInfo,
                                       out String?             ErrorResponse)

            => TryParse(JSON,
                        out AuthorizationInfo,
                        out ErrorResponse,
                        null,
                        null,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of an authorization information.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="AuthorizationInfo">The parsed authorization information.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomAuthorizationInfoParser">A delegate to parse custom authorization information JSON objects.</param>
        public static Boolean TryParse(JObject                                          JSON,
                                       out AuthorizationInfo?                           AuthorizationInfo,
                                       out String?                                      ErrorResponse,
                                       RemoteParty?                                     RemoteParty                     = null,
                                       EMSP_Id?                                         EMSPId                          = null,
                                       CustomJObjectParserDelegate<AuthorizationInfo>?  CustomAuthorizationInfoParser   = null)
        {

            try
            {

                AuthorizationInfo = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse Allowed                   [mandatory]

                if (!JSON.ParseMandatory("allowed",
                                         "allowed",
                                         AllowedType.TryParse,
                                         out AllowedType Allowed,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Token                     [optional]

                if (JSON.ParseOptionalJSON("token",
                                           "token",
                                           OCPIv3_0.Token.TryParse,
                                           out Token? Token,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse LocationReference         [optional]

                if (JSON.ParseOptionalJSON("location",
                                           "location reference",
                                           OCPIv3_0.LocationReference.TryParse,
                                           out LocationReference? LocationReference,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse AuthorizationReference    [optional]

                if (JSON.ParseOptional("authorization_reference",
                                       "authorization reference",
                                       OCPIv3_0.AuthorizationReference.TryParse,
                                       out AuthorizationReference? AuthorizationReference,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse Info                      [optional]

                if (JSON.ParseOptionalJSON("info",
                                           "multi-language information",
                                           DisplayText.TryParse,
                                           out DisplayText? Info,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                AuthorizationInfo = new AuthorizationInfo(Allowed,
                                                          Token,
                                                          LocationReference,
                                                          AuthorizationReference,
                                                          Info,
                                                          RemoteParty,
                                                          EMSPId);


                if (CustomAuthorizationInfoParser is not null)
                    AuthorizationInfo = CustomAuthorizationInfoParser(JSON,
                                                                      AuthorizationInfo);

                return true;

            }
            catch (Exception e)
            {
                AuthorizationInfo  = default;
                ErrorResponse      = "The given JSON representation of an authorization information is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomAuthorizationInfoSerializer = null, CustomTokenSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomAuthorizationInfoSerializer">A delegate to serialize custom authorizationInfo JSON objects.</param>
        /// <param name="CustomTokenSerializer">A delegate to serialize custom token JSON objects.</param>
        /// <param name="CustomEnergyContractSerializer">A delegate to serialize custom energy contract JSON objects.</param>
        /// <param name="CustomLocationReferenceSerializer">A delegate to serialize custom location reference JSON objects.</param>
        /// <param name="CustomDisplayTextSerializer">A delegate to serialize custom multi-language text JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<AuthorizationInfo>?  CustomAuthorizationInfoSerializer   = null,
                              CustomJObjectSerializerDelegate<Token>?              CustomTokenSerializer               = null,
                              CustomJObjectSerializerDelegate<EnergyContract>?     CustomEnergyContractSerializer      = null,
                              CustomJObjectSerializerDelegate<LocationReference>?  CustomLocationReferenceSerializer   = null,
                              CustomJObjectSerializerDelegate<DisplayText>?        CustomDisplayTextSerializer         = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("allowed",                   Allowed.                     ToString()),

                           Token is not null
                               ? new JProperty("token",                     Token.                       ToJSON(true,
                                                                                                                true,
                                                                                                                true,
                                                                                                                true,
                                                                                                                CustomTokenSerializer,
                                                                                                                CustomEnergyContractSerializer))
                               : null,

                           Location.HasValue
                               ? new JProperty("location",                  Location.              Value.ToJSON(CustomLocationReferenceSerializer))
                               : null,

                           AuthorizationReference.HasValue
                               ? new JProperty("authorization_reference",   AuthorizationReference.Value.ToString())
                               : null,

                           Info.HasValue
                               ? new JProperty("info",                      Info.                  Value.ToJSON(CustomDisplayTextSerializer))
                               : null

                       );

            return CustomAuthorizationInfoSerializer is not null
                       ? CustomAuthorizationInfoSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this object.
        /// </summary>
        public AuthorizationInfo Clone()

            => new (
                   Allowed.                Clone(),
                   Token?.                 Clone(),
                   Location?.              Clone(),
                   AuthorizationReference?.Clone(),
                   Info?.                  Clone(),
                   RemoteParty,
                   EMSPId,
                   Runtime
               );

        #endregion


        #region Operator overloading

        #region Operator == (AuthorizationInfo1, AuthorizationInfo2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthorizationInfo1">An authorization information.</param>
        /// <param name="AuthorizationInfo2">Another authorization information.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (AuthorizationInfo AuthorizationInfo1,
                                           AuthorizationInfo AuthorizationInfo2)
        {

            if (Object.ReferenceEquals(AuthorizationInfo1, AuthorizationInfo2))
                return true;

            if (AuthorizationInfo1 is null || AuthorizationInfo2 is null)
                return false;

            return AuthorizationInfo1.Equals(AuthorizationInfo2);

        }

        #endregion

        #region Operator != (AuthorizationInfo1, AuthorizationInfo2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthorizationInfo1">An authorization information.</param>
        /// <param name="AuthorizationInfo2">Another authorization information.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (AuthorizationInfo AuthorizationInfo1,
                                           AuthorizationInfo AuthorizationInfo2)

            => !(AuthorizationInfo1 == AuthorizationInfo2);

        #endregion

        #endregion

        #region IEquatable<AuthorizationInfo> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two authorization information for equality.
        /// </summary>
        /// <param name="Object">An authorization information to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is AuthorizationInfo authorizationReference &&
                   Equals(authorizationReference);

        #endregion

        #region Equals(AuthorizationInfo)

        /// <summary>
        /// Compares two authorization information for equality.
        /// </summary>
        /// <param name="AuthorizationInfo">An authorization information to compare with.</param>
        public Boolean Equals(AuthorizationInfo? AuthorizationInfo)

            => AuthorizationInfo is not null &&

               Allowed.Equals(AuthorizationInfo.Allowed) &&

             ((Token is     null               &&  AuthorizationInfo.Token is     null)               ||
              (Token is not null               &&  AuthorizationInfo.Token is not null                && Token.                       Equals(AuthorizationInfo.Token                       ))) &&

            ((!Location.              HasValue && !AuthorizationInfo.Location.              HasValue) ||
              (Location.              HasValue &&  AuthorizationInfo.Location.              HasValue  && Location.              Value.Equals(AuthorizationInfo.Location.              Value))) &&

            ((!AuthorizationReference.HasValue && !AuthorizationInfo.AuthorizationReference.HasValue) ||
              (AuthorizationReference.HasValue &&  AuthorizationInfo.AuthorizationReference.HasValue  && AuthorizationReference.Value.Equals(AuthorizationInfo.AuthorizationReference.Value))) &&

            ((!Info.                  HasValue && !AuthorizationInfo.Info.                  HasValue) ||
              (Info.                  HasValue &&  AuthorizationInfo.Info.                  HasValue  && Info.                  Value.Equals(AuthorizationInfo.Info.                  Value)));

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

                return Allowed.                GetHashCode()       * 11 ^
                      (Token?.                 GetHashCode() ?? 0) *  7 ^
                      (Location?.              GetHashCode() ?? 0) *  5 ^
                      (AuthorizationReference?.GetHashCode() ?? 0) *  3^
                       Info?.                  GetHashCode() ?? 0;

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   Token?.Id.ToString() ?? "-",
                   " => ",
                   Allowed,

                   Location.HasValue
                       ? " @ " + Location.Value.LocationId + " / " + Location.Value.EVSEUIds.AggregateWith(", ")
                       : "",

                   Info.HasValue
                       ? ", info: " + Info.Value.Text
                       : "",

                   AuthorizationReference.HasValue
                       ? ", ref: " + AuthorizationReference.Value
                       : ""

               );

        #endregion

    }

}
