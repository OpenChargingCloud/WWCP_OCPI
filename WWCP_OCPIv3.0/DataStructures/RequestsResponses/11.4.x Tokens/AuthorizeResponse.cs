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

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv3_0
{

    /// <summary>
    /// An authorize response.
    /// </summary>
    public class AuthorizeResponse : AAsyncResponse<AuthorizeRequest, AuthorizeResponse>,
                                     IEquatable<AuthorizeResponse>,
                                     IComparable<AuthorizeResponse>
    {

        #region Data

        /// <summary>
        /// The default JSON-LD context of this object.
        /// </summary>
        public static readonly JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/contexts/OCPI/3.0/locations/authorizeResponse");

        #endregion

        #region Properties

        /// <summary>
        /// Status of the Token, and whether charging is allowed at the Location given in the
        /// corresponding AuthorizationRequest, possibly restricted to the EVSEs given in the
        /// AuthorizationRequest.
        /// </summary>
        [Mandatory]
        public AllowedType              Allowed                   { get; }

        /// <summary>
        /// The complete Token object for which this authorization was requested.
        /// </summary>
        [Mandatory]
        public Token                    Token                     { get; }

        /// <summary>
        /// The complete Token object for which this authorization was requested.
        /// </summary>
        [Optional]
        public AuthorizationReference?  AuthorizationReference    { get; }

        /// <summary>
        /// The time from which the Token is authorized to charge at the Location given in
        /// the request.The eMSP MAY dispute the resulting CDR if the CPO uses the
        /// authorization reference for a Session that started before this timestamp.
        /// </summary>
        [Mandatory]
        public DateTimeOffset           AuthorizationTimestamp    { get; }

        /// <summary>
        /// The time until which the Token is authorized to charge at the location given
        /// in the request. The eMSP MAY dispute the resulting CDR if the CPO uses the
        /// authorization reference for a session that started at or after this timestamp.
        /// </summary>
        [Mandatory]
        public DateTimeOffset           AuthorizedUntil           { get; }

        /// <summary>
        /// If given, this is the maximum amount of energy that the eMSP is authorizing
        /// the CPO to let the Driver charge.If the CPO starts a Charging Session for
        /// this authorization, and sends the eMSP a CDR for that Session with more
        /// energy consumed than the amount of kWh given in this field, then the eMSP
        /// MAY dispute the CDR.
        /// This field is meant to facilitate pre-paid eMSP models. It is typically left
        /// unfilled in other scenarios.
        /// </summary>
        [Optional]
        public WattHour?                MaxEnergy                 { get; }

        /// <summary>
        /// If given, this is the maximum amount of time that the eMSP is authorizing
        /// the CPO to let the Driver charge for. If the CPO starts a Charging Session
        /// for this authorization, and sends the eMSP a CDR for that Session than
        /// lasted longer than the amount of hours given in this field, then the eMSP
        /// MAY dispute the CDR.
        /// This field is meant to facilitate pre-paid eMSP models. It is typically
        /// left unfilled in other scenarios.
        /// </summary>
        [Optional]
        public TimeSpan?                MaxTime                   { get; }

        /// <summary>
        /// Optional display text, additional information to the EV driver.
        /// </summary>
        [Optional]
        public DisplayText?             Info                      { get; }

        /// <summary>
        /// The Tariff that will be charged by the eMSP to the Driver, to be displayed
        /// on the Charging Station. This added because regulations in the US State of
        /// California require that a Driver see on the Charging Station what they will
        /// be paying when they start a Charging Session.
        /// This field can also be used in combination with IEC 15118 to provide
        /// pricing information from the Charging Station to the vehicle.
        /// Where these two use cases do not apply, this field may be left empty.
        /// </summary>
        [Optional]
        public Tariff?                  DisplayTariff             { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new authorize response.
        /// </summary>
        /// <param name="Request">The request leading to this response.</param>
        /// <param name="ResultType">The completion status of the requested asynchronous remote procedure call.</param>
        /// 
        /// <param name="Allowed">Status of the Token, and whether charging is allowed at the Location given in the corresponding AuthorizationRequest, possibly restricted to the EVSEs given in the AuthorizationRequest.</param>
        /// <param name="Token">The complete Token object for which this authorization was requested.</param>
        /// <param name="AuthorizationTimestamp">The time from which the Token is authorized to charge at the Location given in the request.The eMSP MAY dispute the resulting CDR if the CPO uses the authorization reference for a Session that started before this timestamp.</param>
        /// <param name="AuthorizedUntil">The time until which the Token is authorized to charge at the location given in the request. The eMSP MAY dispute the resulting CDR if the CPO uses the authorization reference for a session that started at or after this timestamp.</param>
        /// 
        /// <param name="AuthorizationReference">The complete Token object for which this authorization was requested.</param>
        /// <param name="MaxEnergy">If given, this is the maximum amount of energy that the eMSP is authorizing the CPO to let the Driver charge.If the CPO starts a Charging Session for this authorization, and sends the eMSP a CDR for that Session with more energy consumed than the amount of kWh given in this field, then the eMSP MAY dispute the CDR. This field is meant to facilitate pre-paid eMSP models. It is typically left unfilled in other scenarios.</param>
        /// <param name="MaxTime">If given, this is the maximum amount of time that the eMSP is authorizing the CPO to let the Driver charge for. If the CPO starts a Charging Session for this authorization, and sends the eMSP a CDR for that Session than lasted longer than the amount of hours given in this field, then the eMSP MAY dispute the CDR. This field is meant to facilitate pre-paid eMSP models. It is typically left unfilled in other scenarios.</param>
        /// <param name="Info">Optional display text, additional information to the EV driver.</param>
        /// <param name="DisplayTariff">The Tariff that will be charged by the eMSP to the Driver, to be displayed on the Charging Station. This added because regulations in the US State of California require that a Driver see on the Charging Station what they will be paying when they start a Charging Session. This field can also be used in combination with IEC 15118 to provide pricing information from the Charging Station to the vehicle. Where these two use cases do not apply, this field may be left empty.</param>
        public AuthorizeResponse(AuthorizeRequest         Request,
                                 AsyncResultType          ResultType,

                                 AllowedType              Allowed,
                                 Token                    Token,
                                 DateTimeOffset           AuthorizationTimestamp,
                                 DateTimeOffset           AuthorizedUntil,

                                 AuthorizationReference?  AuthorizationReference   = null,
                                 WattHour?                MaxEnergy                = null,
                                 TimeSpan?                MaxTime                  = null,
                                 DisplayText?             Info                     = null,
                                 Tariff?                  DisplayTariff            = null)

            : base(Request,
                   ResultType)

        {

            this.Allowed                 = Allowed;
            this.Token                   = Token;
            this.AuthorizationTimestamp  = AuthorizationTimestamp;
            this.AuthorizedUntil         = AuthorizedUntil;

            this.AuthorizationReference  = AuthorizationReference;
            this.MaxEnergy               = MaxEnergy;
            this.MaxTime                 = MaxTime;
            this.Info                    = Info;
            this.DisplayTariff           = DisplayTariff;

            unchecked
            {

                hashCode = this.Request.                GetHashCode()       * 31 ^
                           this.ResultType.             GetHashCode()       * 29 ^

                           this.Allowed.                GetHashCode()       * 23 ^
                           this.Token.                  GetHashCode()       * 19 ^
                           this.AuthorizationTimestamp. GetHashCode()       * 17 ^
                           this.AuthorizedUntil.        GetHashCode()       * 13 ^

                          (this.AuthorizationReference?.GetHashCode() ?? 0) * 11 ^
                          (this.MaxEnergy?.             GetHashCode() ?? 0) *  7 ^
                          (this.MaxTime?.               GetHashCode() ?? 0) *  5 ^
                          (this.Info?.                  GetHashCode() ?? 0) *  3 ^
                           this.DisplayTariff?.         GetHashCode() ?? 0;

            }

        }

        #endregion


        #region (static) Parse   (Request, JSON, ...)

        /// <summary>
        /// Parse the given JSON representation of an authorize response.
        /// </summary>
        /// <param name="Request">The request leading to this response.</param>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomAuthorizeResponseParser">A delegate to parse custom authorize response JSON objects.</param>
        public static AuthorizeResponse Parse(AuthorizeRequest                                 Request,
                                              JObject                                          JSON,
                                              CustomJObjectParserDelegate<AuthorizeResponse>?  CustomAuthorizeResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var authorizeResponse,
                         out var errorResponse,
                         CustomAuthorizeResponseParser))
            {
                return authorizeResponse;
            }

            throw new ArgumentException("The given JSON representation of an authorize response is invalid: " + errorResponse, nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out AuthorizeResponse, out ErrorResponse, ...)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of an authorize response.
        /// </summary>
        /// <param name="Request">The request leading to this response.</param>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="AuthorizeResponse">The parsed authorize response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(AuthorizeRequest                             Request,
                                       JObject                                      JSON,
                                       [NotNullWhen(true)]  out AuthorizeResponse?  AuthorizeResponse,
                                       [NotNullWhen(false)] out String?             ErrorResponse)

            => TryParse(Request,
                        JSON,
                        out AuthorizeResponse,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of an authorize response.
        /// </summary>
        /// <param name="Request">The request leading to this response.</param>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="AuthorizeResponse">The parsed authorize response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomAuthorizeResponseParser">A delegate to parse custom authorize response JSON objects.</param>
        public static Boolean TryParse(AuthorizeRequest                                 Request,
                                       JObject                                          JSON,
                                       [NotNullWhen(true)]  out AuthorizeResponse?      AuthorizeResponse,
                                       [NotNullWhen(false)] out String?                 ErrorResponse,
                                       CustomJObjectParserDelegate<AuthorizeResponse>?  CustomAuthorizeResponseParser)
        {

            try
            {
                AuthorizeResponse = null;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse ResultType                [mandatory]

                if (!JSON.ParseMandatory("result_type",
                                         "result type",
                                         AsyncResultType.TryParse,
                                         out AsyncResultType ResultType,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Payload                   [mandatory]

                if (JSON["payload"] is not JObject payload)
                {
                    ErrorResponse = "The async response did not contain a payload!";
                    return false;
                }

                #endregion

                #region Parse Error                     [mandatory]

                if (JSON["error"] is not JObject error)
                {
                    ErrorResponse = "The async response did not contain an error payload!";
                    return false;
                }

                #endregion


                #region Parse Allowed                   [mandatory]

                if (!error.ParseMandatory("allowed",
                                          "allowed",
                                          AllowedType.TryParse,
                                          out AllowedType Allowed,
                                          out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Token                     [mandatory]

                if (!error.ParseMandatoryJSON("token",
                                              "token",
                                              OCPIv3_0.Token.TryParse,
                                              out Token? Token,
                                              out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse AuthorizationTimestamp    [mandatory]

                if (!error.ParseMandatory("authorization_timestamp",
                                          "authorization_timestamp",
                                          out DateTimeOffset AuthorizationTimestamp,
                                          out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse AuthorizedUntil           [mandatory]

                if (!error.ParseMandatory("authorized_until",
                                          "authorized until",
                                          out DateTimeOffset AuthorizedUntil,
                                          out ErrorResponse))
                {
                    return false;
                }

                #endregion


                #region Parse AuthorizationReference    [optional]

                if (error.ParseOptional("authorization_reference",
                                        "authorization reference",
                                        OCPIv3_0.AuthorizationReference.TryParse,
                                        out AuthorizationReference AuthorizationReference,
                                        out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse MaxEnergy                 [optional]

                if (error.ParseOptional("max_energy",
                                        "max energy",
                                        WattHour.TryParseKWh,
                                        out WattHour? MaxEnergy,
                                        out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse MaxTime                   [optional]

                if (error.ParseOptional("max_time",
                                        "max time",
                                        out TimeSpan? MaxTime,
                                        out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse Info                      [optional]

                if (error.ParseOptionalJSON("info",
                                            "user info",
                                            DisplayText.TryParse,
                                            out DisplayText? Info,
                                            out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse DisplayTariff             [optional]

                if (error.ParseOptionalJSON("display_tariff",
                                            "display tariff",
                                            Tariff.TryParse,
                                            out Tariff? DisplayTariff,
                                            out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                AuthorizeResponse = new AuthorizeResponse(

                                        Request,
                                        ResultType,

                                        Allowed,
                                        Token,
                                        AuthorizationTimestamp,
                                        AuthorizedUntil,

                                        AuthorizationReference,
                                        MaxEnergy,
                                        MaxTime,
                                        Info,
                                        DisplayTariff

                                    );


                if (CustomAuthorizeResponseParser is not null)
                    AuthorizeResponse = CustomAuthorizeResponseParser(JSON,
                                                                      AuthorizeResponse);

                return true;

            }
            catch (Exception e)
            {
                AuthorizeResponse  = default;
                ErrorResponse      = "The given JSON representation of an authorize response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomAuthorizeResponseSerializer = null, CustomTokenSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomAuthorizeResponseSerializer">A delegate to serialize custom authorize responses.</param>
        /// <param name="CustomTokenSerializer">A delegate to serialize custom token JSON objects.</param>
        /// <param name="CustomEnergyContractSerializer">A delegate to serialize custom energy contract JSON objects.</param>
        /// <param name="CustomTariffSerializer">A delegate to serialize custom tariff JSON objects.</param>
        /// <param name="CustomDisplayTextSerializer">A delegate to serialize custom multi-language text JSON objects.</param>
        /// <param name="CustomPriceSerializer">A delegate to serialize custom price JSON objects.</param>
        /// <param name="CustomTariffElementSerializer">A delegate to serialize custom tariff element JSON objects.</param>
        /// <param name="CustomPriceComponentSerializer">A delegate to serialize custom price component JSON objects.</param>
        /// <param name="CustomTariffRestrictionsSerializer">A delegate to serialize custom tariff restrictions JSON objects.</param>
        /// <param name="CustomEnergyMixSerializer">A delegate to serialize custom hours JSON objects.</param>
        /// <param name="CustomEnergySourceSerializer">A delegate to serialize custom energy source JSON objects.</param>
        /// <param name="CustomEnvironmentalImpactSerializer">A delegate to serialize custom environmental impact JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<AuthorizeResponse>?    CustomAuthorizeResponseSerializer     = null,
                              CustomJObjectSerializerDelegate<Token>?                CustomTokenSerializer                 = null,
                              CustomJObjectSerializerDelegate<EnergyContract>?       CustomEnergyContractSerializer        = null,
                              CustomJObjectSerializerDelegate<Tariff>?               CustomTariffSerializer                = null,
                              CustomJObjectSerializerDelegate<DisplayText>?          CustomDisplayTextSerializer           = null,
                              CustomJObjectSerializerDelegate<Price>?                CustomPriceSerializer                 = null,
                              CustomJObjectSerializerDelegate<TariffElement>?        CustomTariffElementSerializer         = null,
                              CustomJObjectSerializerDelegate<PriceComponent>?       CustomPriceComponentSerializer        = null,
                              CustomJObjectSerializerDelegate<TariffRestrictions>?   CustomTariffRestrictionsSerializer    = null,
                              CustomJObjectSerializerDelegate<EnergyMix>?            CustomEnergyMixSerializer             = null,
                              CustomJObjectSerializerDelegate<EnergySource>?         CustomEnergySourceSerializer          = null,
                              CustomJObjectSerializerDelegate<EnvironmentalImpact>?  CustomEnvironmentalImpactSerializer   = null)
        {

            var json = ToJSON(
                           Payload:  null,
                           Error:    JSONObject.Create(

                                               new JProperty("allowed",                   Allowed.                     ToString()),
                                               new JProperty("token",                     Token.                       ToJSON(true,
                                                                                                                              true,
                                                                                                                              true,
                                                                                                                              true,
                                                                                                                              CustomTokenSerializer,
                                                                                                                              CustomEnergyContractSerializer)),
                                               new JProperty("authorization_timestamp",   AuthorizationTimestamp.      ToISO8601()),
                                               new JProperty("authorized_until",          AuthorizedUntil.             ToISO8601()),

                                         AuthorizationReference.HasValue
                                             ? new JProperty("authorization_reference",   AuthorizationReference.Value.ToString())
                                             : null,

                                         MaxEnergy.             HasValue
                                             ? new JProperty("max_energy",                MaxEnergy.             Value.ToString())
                                             : null,

                                         MaxTime.               HasValue
                                             ? new JProperty("max_time",                  MaxTime.               Value.ToString())
                                             : null,

                                         Info.                  HasValue
                                             ? new JProperty("info",                      Info.                  Value.ToString())
                                             : null,

                                         DisplayTariff is not null
                                             ? new JProperty("display_tariff",            DisplayTariff.               ToJSON(true,
                                                                                                                              true,
                                                                                                                              true,
                                                                                                                              true,
                                                                                                                              CustomTariffSerializer,
                                                                                                                              CustomDisplayTextSerializer,
                                                                                                                              CustomPriceSerializer,
                                                                                                                              CustomTariffElementSerializer,
                                                                                                                              CustomPriceComponentSerializer,
                                                                                                                              CustomTariffRestrictionsSerializer,
                                                                                                                              CustomEnergyMixSerializer,
                                                                                                                              CustomEnergySourceSerializer,
                                                                                                                              CustomEnvironmentalImpactSerializer))
                                             : null

                                     )
                       );

            return CustomAuthorizeResponseSerializer is not null
                       ? CustomAuthorizeResponseSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this authorize response.
        /// </summary>
        public AuthorizeResponse Clone()

            => new (

                   Request.                Clone(),
                   ResultType.             Clone(),

                   Allowed.                Clone(),
                   Token.                  Clone(),
                   AuthorizationTimestamp,
                   AuthorizedUntil,

                   AuthorizationReference?.Clone(),
                   MaxEnergy?.             Clone(),
                   MaxTime,
                   Info?.                  Clone(),
                   DisplayTariff?.         Clone()

               );

        #endregion


        #region Operator overloading

        #region Operator == (AuthorizeResponse1, AuthorizeResponse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthorizeResponse1">An authorize response.</param>
        /// <param name="AuthorizeResponse2">Another authorize response.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (AuthorizeResponse AuthorizeResponse1,
                                           AuthorizeResponse AuthorizeResponse2)

            => AuthorizeResponse1.Equals(AuthorizeResponse2);

        #endregion

        #region Operator != (AuthorizeResponse1, AuthorizeResponse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthorizeResponse1">An authorize response.</param>
        /// <param name="AuthorizeResponse2">Another authorize response.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (AuthorizeResponse AuthorizeResponse1,
                                           AuthorizeResponse AuthorizeResponse2)

            => !AuthorizeResponse1.Equals(AuthorizeResponse2);

        #endregion

        #region Operator <  (AuthorizeResponse1, AuthorizeResponse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthorizeResponse1">An authorize response.</param>
        /// <param name="AuthorizeResponse2">Another authorize response.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (AuthorizeResponse AuthorizeResponse1,
                                          AuthorizeResponse AuthorizeResponse2)

            => AuthorizeResponse1.CompareTo(AuthorizeResponse2) < 0;

        #endregion

        #region Operator <= (AuthorizeResponse1, AuthorizeResponse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthorizeResponse1">An authorize response.</param>
        /// <param name="AuthorizeResponse2">Another authorize response.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (AuthorizeResponse AuthorizeResponse1,
                                           AuthorizeResponse AuthorizeResponse2)

            => AuthorizeResponse1.CompareTo(AuthorizeResponse2) <= 0;

        #endregion

        #region Operator >  (AuthorizeResponse1, AuthorizeResponse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthorizeResponse1">An authorize response.</param>
        /// <param name="AuthorizeResponse2">Another authorize response.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (AuthorizeResponse AuthorizeResponse1,
                                          AuthorizeResponse AuthorizeResponse2)

            => AuthorizeResponse1.CompareTo(AuthorizeResponse2) > 0;

        #endregion

        #region Operator >= (AuthorizeResponse1, AuthorizeResponse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthorizeResponse1">An authorize response.</param>
        /// <param name="AuthorizeResponse2">Another authorize response.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (AuthorizeResponse AuthorizeResponse1,
                                           AuthorizeResponse AuthorizeResponse2)

            => AuthorizeResponse1.CompareTo(AuthorizeResponse2) >= 0;

        #endregion

        #endregion

        #region IComparable<AuthorizeResponse> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two authorize responses.
        /// </summary>
        /// <param name="Object">An authorize response to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is AuthorizeResponse authorizeResponse
                   ? CompareTo(authorizeResponse)
                   : throw new ArgumentException("The given object is not an authorize response!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(AuthorizeResponse)

        /// <summary>
        /// Compares two authorize responses.
        /// </summary>
        /// <param name="AuthorizeResponse">An authorize response to compare with.</param>
        public Int32 CompareTo(AuthorizeResponse? AuthorizeResponse)
        {

            if (AuthorizeResponse is null)
                throw new ArgumentNullException(nameof(AuthorizeResponse), "The given authorize response must not be null!");

            var c = ResultType.                  CompareTo(AuthorizeResponse.ResultType);

            if (c == 0)
                c = Allowed.                     CompareTo(AuthorizeResponse.Allowed);

            if (c == 0)
                c = Token.                       CompareTo(AuthorizeResponse.Token);

            if (c == 0)
                c = AuthorizationTimestamp.      CompareTo(AuthorizeResponse.AuthorizationTimestamp);

            if (c == 0)
                c = AuthorizedUntil.             CompareTo(AuthorizeResponse.AuthorizedUntil);

            if (c == 0 && AuthorizationReference.HasValue && AuthorizeResponse.AuthorizationReference.HasValue)
                c = AuthorizationReference.Value.CompareTo(AuthorizeResponse.AuthorizationReference.Value);

            if (c == 0 && MaxEnergy.HasValue && AuthorizeResponse.MaxEnergy.HasValue)
                c = MaxEnergy.             Value.CompareTo(AuthorizeResponse.MaxEnergy.Value);

            if (c == 0 && MaxTime.HasValue && AuthorizeResponse.MaxTime.HasValue)
                c = MaxTime.               Value.CompareTo(AuthorizeResponse.MaxTime.Value);

            if (c == 0 && Info.HasValue && AuthorizeResponse.Info.HasValue)
                c = Info.                  Value.CompareTo(AuthorizeResponse.Info.Value);

            if (c == 0 && DisplayTariff is not null && AuthorizeResponse.DisplayTariff is not null)
                c = DisplayTariff.               CompareTo(AuthorizeResponse.DisplayTariff);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<AuthorizeResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two authorize responses for equality.
        /// </summary>
        /// <param name="Object">An authorize response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is AuthorizeResponse authorizeResponse &&
                   Equals(authorizeResponse);

        #endregion

        #region Equals(AuthorizeResponse)

        /// <summary>
        /// Compares two authorize responses for equality.
        /// </summary>
        /// <param name="AuthorizeResponse">An authorize response to compare with.</param>
        public Boolean Equals(AuthorizeResponse? AuthorizeResponse)

            => AuthorizeResponse is not null &&

               Allowed.               Equals(AuthorizeResponse.Allowed)                &&
               Token.                 Equals(AuthorizeResponse.Token)                  &&
               AuthorizationTimestamp.Equals(AuthorizeResponse.AuthorizationTimestamp) &&
               AuthorizedUntil.       Equals(AuthorizeResponse.AuthorizedUntil)        &&

            ((!AuthorizationReference.HasValue    && !AuthorizeResponse.AuthorizationReference.HasValue) ||
              (AuthorizationReference.HasValue    &&  AuthorizeResponse.AuthorizationReference.HasValue && AuthorizationReference.Value.Equals(AuthorizeResponse.AuthorizationReference.Value))) &&

            ((!MaxEnergy.             HasValue    && !AuthorizeResponse.MaxEnergy.             HasValue) ||
              (MaxEnergy.             HasValue    &&  AuthorizeResponse.MaxEnergy.             HasValue && MaxEnergy.             Value.Equals(AuthorizeResponse.MaxEnergy.             Value))) &&

            ((!MaxTime.               HasValue    && !AuthorizeResponse.MaxTime.               HasValue) ||
              (MaxTime.               HasValue    &&  AuthorizeResponse.MaxTime.               HasValue && MaxTime.               Value.Equals(AuthorizeResponse.MaxTime.               Value))) &&

            ((!Info.                  HasValue    && !AuthorizeResponse.Info.                  HasValue) ||
              (Info.                  HasValue    &&  AuthorizeResponse.Info.                  HasValue && Info.                  Value.Equals(AuthorizeResponse.Info.                  Value))) &&

             ((DisplayTariff          is null     &&  AuthorizeResponse.DisplayTariff          is null)  ||
              (DisplayTariff          is not null &&  AuthorizeResponse.DisplayTariff          is not null && DisplayTariff.            Equals(AuthorizeResponse.DisplayTariff)));

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

                   $"{ResultType} / {Allowed}",

                   MaxEnergy.HasValue
                       ? $", max '{MaxEnergy.Value.kWh}' kWh"
                       : "",

                   MaxTime.HasValue
                       ? $", max '{MaxTime.Value.TotalMinutes}' minutes"
                       : "",

                   DisplayTariff is not null
                       ? $", tariff id '{DisplayTariff.Id}'"
                       : "",

                   Info is not null
                       ? $", '{Info}'"
                       : ""

               );

        #endregion

    }

}
