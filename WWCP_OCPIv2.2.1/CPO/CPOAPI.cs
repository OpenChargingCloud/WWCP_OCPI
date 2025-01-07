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
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2_1.HTTP
{

    /// <summary>
    /// Extension methods for the CPO HTTP API.
    /// </summary>
    public static class CPOAPIExtensions
    {

        //#region ParseCountryCodeAndPartyId (this Request, CPOAPI, out CountryCode, out PartyId,                                                        out HTTPResponse)

        ///// <summary>
        ///// Parse the given HTTP request and return the location identification
        ///// for the given HTTP hostname and HTTP query parameter
        ///// or an HTTP error response.
        ///// </summary>
        ///// <param name="Request">A HTTP request.</param>
        ///// <param name="CPOAPI">The CPO API.</param>
        ///// <param name="CountryCode">The parsed country code.</param>
        ///// <param name="PartyId">The parsed party identification.</param>
        ///// <param name="OCPIResponseBuilder">An OCPI response builder.</param>
        ///// <returns>True, when user identification was found; false else.</returns>
        //public static Boolean ParseCountryCodeAndPartyId(this OCPIRequest           Request,
        //                                                 CPOAPI                     CPOAPI,
        //                                                 out CountryCode?           CountryCode,
        //                                                 out Party_Id?              PartyId,
        //                                                 out OCPIResponse.Builder?  OCPIResponseBuilder)
        //{

        //    #region Initial checks

        //    if (Request is null)
        //        throw new ArgumentNullException(nameof(Request),  "The given HTTP request must not be null!");

        //    if (CPOAPI is null)
        //        throw new ArgumentNullException(nameof(CPOAPI),   "The given CPO API must not be null!");

        //    #endregion

        //    CountryCode          = default;
        //    PartyId              = default;
        //    OCPIResponseBuilder  = default;

        //    if (Request.ParsedURLParameters.Length < 2)
        //    {

        //        OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
        //            StatusCode           = 2001,
        //            StatusMessage        = "Missing country code and/or party identification!",
        //            HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
        //                HTTPStatusCode             = HTTPStatusCode.BadRequest,
        //                //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
        //                AccessControlAllowHeaders  = [ "Authorization" ]
        //            }
        //        };

        //        return false;

        //    }

        //    CountryCode = OCPI.CountryCode.TryParse(Request.ParsedURLParameters[0]);

        //    if (!CountryCode.HasValue)
        //    {

        //        OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
        //            StatusCode           = 2001,
        //            StatusMessage        = "Invalid country code!",
        //            HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
        //                HTTPStatusCode             = HTTPStatusCode.BadRequest,
        //                //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
        //                AccessControlAllowHeaders  = [ "Authorization" ]
        //            }
        //        };

        //        return false;

        //    }

        //    PartyId = Party_Id.TryParse(Request.ParsedURLParameters[1]);

        //    if (!PartyId.HasValue)
        //    {

        //        OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
        //            StatusCode           = 2001,
        //            StatusMessage        = "Invalid party identification!",
        //            HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
        //                HTTPStatusCode             = HTTPStatusCode.BadRequest,
        //                //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
        //                AccessControlAllowHeaders  = [ "Authorization" ]
        //            }
        //        };

        //        return false;

        //    }

        //    return true;

        //}

        //#endregion















        //#region ParseCommandId             (this Request, CPOAPI, out CommandId,                                                                       out HTTPResponse)

        ///// <summary>
        ///// Parse the given HTTP request and return the command identification
        ///// for the given HTTP hostname and HTTP query parameter
        ///// or an HTTP error response.
        ///// </summary>
        ///// <param name="Request">A HTTP request.</param>
        ///// <param name="CPOAPI">The CPO API.</param>
        ///// <param name="CommandId">The parsed unique command identification.</param>
        ///// <param name="OCPIResponseBuilder">An OCPI response builder.</param>
        ///// <returns>True, when user identification was found; false else.</returns>
        //public static Boolean ParseCommandId(this OCPIRequest           Request,
        //                                     CPOAPI                     CPOAPI,
        //                                     out Command_Id?            CommandId,
        //                                     out OCPIResponse.Builder?  OCPIResponseBuilder)
        //{

        //    #region Initial checks

        //    if (Request is null)
        //        throw new ArgumentNullException(nameof(Request),  "The given HTTP request must not be null!");

        //    if (CPOAPI  is null)
        //        throw new ArgumentNullException(nameof(CPOAPI),   "The given CPO API must not be null!");

        //    #endregion

        //    CommandId            = default;
        //    OCPIResponseBuilder  = default;

        //    if (Request.ParsedURLParameters.Length < 1)
        //    {

        //        OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
        //            StatusCode           = 2001,
        //            StatusMessage        = "Missing command identification!",
        //            HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
        //                HTTPStatusCode             = HTTPStatusCode.BadRequest,
        //                //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
        //                AccessControlAllowHeaders  = [ "Authorization" ]
        //            }
        //        };

        //        return false;

        //    }

        //    CommandId = Command_Id.TryParse(Request.ParsedURLParameters[0]);

        //    if (!CommandId.HasValue)
        //    {

        //        OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
        //            StatusCode           = 2001,
        //            StatusMessage        = "Invalid command identification!",
        //            HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
        //                HTTPStatusCode             = HTTPStatusCode.BadRequest,
        //                //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
        //                AccessControlAllowHeaders  = [ "Authorization" ]
        //            }
        //        };

        //        return false;

        //    }

        //    return true;

        //}

        //#endregion

    }


    /// <summary>
    /// The HTTP API for charge point operators.
    /// EMSPs will connect to this API.
    /// </summary>
    public class CPOAPI : HTTPAPI
    {

        #region Data

        /// <summary>
        /// The default HTTP server name.
        /// </summary>
        public new const           String    DefaultHTTPServiceName   = "GraphDefined OCPI CPO HTTP API v0.1";

        /// <summary>
        /// The default HTTP server TCP port.
        /// </summary>
        public new static readonly IPPort    DefaultHTTPServerPort    = IPPort.Parse(8080);

        /// <summary>
        /// The default HTTP URL path prefix.
        /// </summary>
        public new static readonly HTTPPath  DefaultURLPathPrefix     = HTTPPath.Parse("cpo/");

        /// <summary>
        /// The default CPO API logfile name.
        /// </summary>
        public     static readonly String    DefaultLogfileName       = $"OCPI{Version.Id}_CPOAPI.log";


        protected Newtonsoft.Json.Formatting JSONFormat               = Newtonsoft.Json.Formatting.Indented;

        #endregion

        #region Properties

        /// <summary>
        /// The CommonAPI.
        /// </summary>
        public CommonAPI      CommonAPI             { get; }

        /// <summary>
        /// (Dis-)allow PUTting of object having an earlier 'LastUpdated'-timestamp then already existing objects.
        /// OCPI v2.2 does not define any behaviour for this.
        /// </summary>
        public Boolean?       AllowDowngrades       { get; }

        /// <summary>
        /// The CPO API logger.
        /// </summary>
        public CPOAPILogger?  CPOAPILogger          { get; }

        #endregion

        #region Custom JSON parsers

        #endregion

        #region Custom JSON serializers

        public CustomJObjectSerializerDelegate<Location>?                    CustomLocationSerializer                      { get; set; }
        public CustomJObjectSerializerDelegate<PublishToken>?                CustomPublishTokenSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<AdditionalGeoLocation>?       CustomAdditionalGeoLocationSerializer         { get; set; }
        public CustomJObjectSerializerDelegate<EVSE>?                        CustomEVSESerializer                          { get; set; }
        public CustomJObjectSerializerDelegate<StatusSchedule>?              CustomStatusScheduleSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<Connector>?                   CustomConnectorSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<EnergyMeter<Location>>?       CustomLocationEnergyMeterSerializer           { get; set; }
        public CustomJObjectSerializerDelegate<EnergyMeter<EVSE>>?           CustomEVSEEnergyMeterSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<TransparencySoftwareStatus>?  CustomTransparencySoftwareStatusSerializer    { get; set; }
        public CustomJObjectSerializerDelegate<TransparencySoftware>?        CustomTransparencySoftwareSerializer          { get; set; }
        public CustomJObjectSerializerDelegate<DisplayText>?                 CustomDisplayTextSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<BusinessDetails>?             CustomBusinessDetailsSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<Hours>?                       CustomHoursSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<Image>?                       CustomImageSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<EnergyMix>?                   CustomEnergyMixSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<EnergySource>?                CustomEnergySourceSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<EnvironmentalImpact>?         CustomEnvironmentalImpactSerializer           { get; set; }


        public CustomJObjectSerializerDelegate<Tariff>?                      CustomTariffSerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<Price>?                       CustomPriceSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<TariffElement>?               CustomTariffElementSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<PriceComponent>?              CustomPriceComponentSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<TariffRestrictions>?          CustomTariffRestrictionsSerializer            { get; set; }


        public CustomJObjectSerializerDelegate<Session>?                     CustomSessionSerializer                       { get; set; }
        public CustomJObjectSerializerDelegate<CDRToken>?                    CustomCDRTokenSerializer                      { get; set; }
        public CustomJObjectSerializerDelegate<ChargingPeriod>?              CustomChargingPeriodSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<CDRDimension>?                CustomCDRDimensionSerializer                  { get; set; }


        public CustomJObjectSerializerDelegate<CDR>?                         CustomCDRSerializer                           { get; set; }
        public CustomJObjectSerializerDelegate<CDRLocation>?                 CustomCDRLocationSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<SignedData>?                  CustomSignedDataSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<SignedValue>?                 CustomSignedValueSerializer                   { get; set; }


        public CustomJObjectSerializerDelegate<Token>?                       CustomTokenSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<EnergyContract>?              CustomEnergyContractSerializer                { get; set; }

        #endregion

        #region Events

        #region Location(s)

        #region (protected internal) GetLocationsRequest    (Request)

        /// <summary>
        /// An event sent whenever a GET locations request was received.
        /// </summary>
        public OCPIRequestLogEvent OnGetLocationsRequest = new ();

        /// <summary>
        /// An event sent whenever a GET locations request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task GetLocationsRequest(DateTime     Timestamp,
                                                    HTTPAPI      API,
                                                    OCPIRequest  Request)

            => OnGetLocationsRequest.WhenAll(Timestamp,
                                             API ?? this,
                                             Request);

        #endregion

        #region (protected internal) GetLocationsResponse   (Response)

        /// <summary>
        /// An event sent whenever a GET locations response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnGetLocationsResponse = new ();

        /// <summary>
        /// An event sent whenever a GET locations response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task GetLocationsResponse(DateTime      Timestamp,
                                                     HTTPAPI       API,
                                                     OCPIRequest   Request,
                                                     OCPIResponse  Response)

            => OnGetLocationsResponse.WhenAll(Timestamp,
                                              API ?? this,
                                              Request,
                                              Response);

        #endregion


        #region (protected internal) GetLocationRequest    (Request)

        /// <summary>
        /// An event sent whenever a GET location request was received.
        /// </summary>
        public OCPIRequestLogEvent OnGetLocationRequest = new ();

        /// <summary>
        /// An event sent whenever a GET location request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task GetLocationRequest(DateTime     Timestamp,
                                                   HTTPAPI      API,
                                                   OCPIRequest  Request)

            => OnGetLocationRequest.WhenAll(Timestamp,
                                            API ?? this,
                                            Request);

        #endregion

        #region (protected internal) GetLocationResponse   (Response)

        /// <summary>
        /// An event sent whenever a GET location response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnGetLocationResponse = new ();

        /// <summary>
        /// An event sent whenever a GET location response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task GetLocationResponse(DateTime      Timestamp,
                                                    HTTPAPI       API,
                                                    OCPIRequest   Request,
                                                    OCPIResponse  Response)

            => OnGetLocationResponse.WhenAll(Timestamp,
                                             API ?? this,
                                             Request,
                                             Response);

        #endregion

        #endregion

        #region EVSE

        #region (protected internal) GetEVSERequest    (Request)

        /// <summary>
        /// An event sent whenever a GET EVSE request was received.
        /// </summary>
        public OCPIRequestLogEvent OnGetEVSERequest = new ();

        /// <summary>
        /// An event sent whenever a GET EVSE request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task GetEVSERequest(DateTime     Timestamp,
                                               HTTPAPI      API,
                                               OCPIRequest  Request)

            => OnGetEVSERequest.WhenAll(Timestamp,
                                        API ?? this,
                                        Request);

        #endregion

        #region (protected internal) GetEVSEResponse   (Response)

        /// <summary>
        /// An event sent whenever a GET EVSE response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnGetEVSEResponse = new ();

        /// <summary>
        /// An event sent whenever a GET EVSE response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task GetEVSEResponse(DateTime      Timestamp,
                                                HTTPAPI       API,
                                                OCPIRequest   Request,
                                                OCPIResponse  Response)

            => OnGetEVSEResponse.WhenAll(Timestamp,
                                         API ?? this,
                                         Request,
                                         Response);

        #endregion

        #endregion

        #region Connector

        #region (protected internal) GetConnectorRequest    (Request)

        /// <summary>
        /// An event sent whenever a GET connector request was received.
        /// </summary>
        public OCPIRequestLogEvent OnGetConnectorRequest = new ();

        /// <summary>
        /// An event sent whenever a GET connector request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task GetConnectorRequest(DateTime     Timestamp,
                                                    HTTPAPI      API,
                                                    OCPIRequest  Request)

            => OnGetConnectorRequest.WhenAll(Timestamp,
                                             API ?? this,
                                             Request);

        #endregion

        #region (protected internal) GetConnectorResponse   (Response)

        /// <summary>
        /// An event sent whenever a GET connector response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnGetConnectorResponse = new ();

        /// <summary>
        /// An event sent whenever a GET connector response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task GetConnectorResponse(DateTime      Timestamp,
                                                     HTTPAPI       API,
                                                     OCPIRequest   Request,
                                                     OCPIResponse  Response)

            => OnGetConnectorResponse.WhenAll(Timestamp,
                                              API ?? this,
                                              Request,
                                              Response);

        #endregion

        #endregion

        #region Tariff(s)

        #region (protected internal) GetTariffsRequest (Request)

        /// <summary>
        /// An event sent whenever a GET tariffs request was received.
        /// </summary>
        public OCPIRequestLogEvent OnGetTariffsRequest = new ();

        /// <summary>
        /// An event sent whenever a GET tariffs request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task GetTariffsRequest(DateTime     Timestamp,
                                                  HTTPAPI      API,
                                                  OCPIRequest  Request)

            => OnGetTariffsRequest.WhenAll(Timestamp,
                                           API ?? this,
                                           Request);

        #endregion

        #region (protected internal) GetTariffsResponse(Response)

        /// <summary>
        /// An event sent whenever a GET tariffs response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnGetTariffsResponse = new ();

        /// <summary>
        /// An event sent whenever a GET tariffs response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task GetTariffsResponse(DateTime      Timestamp,
                                                   HTTPAPI       API,
                                                   OCPIRequest   Request,
                                                   OCPIResponse  Response)

            => OnGetTariffsResponse.WhenAll(Timestamp,
                                            API ?? this,
                                            Request,
                                            Response);

        #endregion


        #region (protected internal) GetTariffRequest    (Request)

        /// <summary>
        /// An event sent whenever a GET tariff request was received.
        /// </summary>
        public OCPIRequestLogEvent OnGetTariffRequest = new ();

        /// <summary>
        /// An event sent whenever a GET tariff request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task GetTariffRequest(DateTime     Timestamp,
                                                 HTTPAPI      API,
                                                 OCPIRequest  Request)

            => OnGetTariffRequest.WhenAll(Timestamp,
                                          API ?? this,
                                          Request);

        #endregion

        #region (protected internal) GetTariffResponse   (Response)

        /// <summary>
        /// An event sent whenever a GET tariff response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnGetTariffResponse = new ();

        /// <summary>
        /// An event sent whenever a GET tariff response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task GetTariffResponse(DateTime      Timestamp,
                                                  HTTPAPI       API,
                                                  OCPIRequest   Request,
                                                  OCPIResponse  Response)

            => OnGetTariffResponse.WhenAll(Timestamp,
                                           API ?? this,
                                           Request,
                                           Response);

        #endregion

        #endregion

        #region Session(s)

        #region (protected internal) GetSessionsRequest (Request)

        /// <summary>
        /// An event sent whenever a GET sessions request was received.
        /// </summary>
        public OCPIRequestLogEvent OnGetSessionsRequest = new ();

        /// <summary>
        /// An event sent whenever a GET sessions request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task GetSessionsRequest(DateTime     Timestamp,
                                                   HTTPAPI      API,
                                                   OCPIRequest  Request)

            => OnGetSessionsRequest.WhenAll(Timestamp,
                                            API ?? this,
                                            Request);

        #endregion

        #region (protected internal) GetSessionsResponse(Response)

        /// <summary>
        /// An event sent whenever a GET sessions response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnGetSessionsResponse = new ();

        /// <summary>
        /// An event sent whenever a GET sessions response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task GetSessionsResponse(DateTime      Timestamp,
                                                    HTTPAPI       API,
                                                    OCPIRequest   Request,
                                                    OCPIResponse  Response)

            => OnGetSessionsResponse.WhenAll(Timestamp,
                                             API ?? this,
                                             Request,
                                             Response);

        #endregion


        #region (protected internal) GetSessionRequest    (Request)

        /// <summary>
        /// An event sent whenever a GET session request was received.
        /// </summary>
        public OCPIRequestLogEvent OnGetSessionRequest = new ();

        /// <summary>
        /// An event sent whenever a GET session request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task GetSessionRequest(DateTime     Timestamp,
                                                  HTTPAPI      API,
                                                  OCPIRequest  Request)

            => OnGetSessionRequest.WhenAll(Timestamp,
                                           API ?? this,
                                           Request);

        #endregion

        #region (protected internal) GetSessionResponse   (Response)

        /// <summary>
        /// An event sent whenever a GET session response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnGetSessionResponse = new ();

        /// <summary>
        /// An event sent whenever a GET session response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task GetSessionResponse(DateTime      Timestamp,
                                                   HTTPAPI       API,
                                                   OCPIRequest   Request,
                                                   OCPIResponse  Response)

            => OnGetSessionResponse.WhenAll(Timestamp,
                                            API ?? this,
                                            Request,
                                            Response);

        #endregion

        #endregion

        #region CDR(s)

        #region (protected internal) GetCDRsRequest (Request)

        /// <summary>
        /// An event sent whenever a GET CDRs request was received.
        /// </summary>
        public OCPIRequestLogEvent OnGetCDRsRequest = new ();

        /// <summary>
        /// An event sent whenever a GET CDRs request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task GetCDRsRequest(DateTime     Timestamp,
                                               HTTPAPI      API,
                                               OCPIRequest  Request)

            => OnGetCDRsRequest.WhenAll(Timestamp,
                                        API ?? this,
                                        Request);

        #endregion

        #region (protected internal) GetCDRsResponse(Response)

        /// <summary>
        /// An event sent whenever a GET CDRs response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnGetCDRsResponse = new ();

        /// <summary>
        /// An event sent whenever a GET CDRs response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task GetCDRsResponse(DateTime      Timestamp,
                                                HTTPAPI       API,
                                                OCPIRequest   Request,
                                                OCPIResponse  Response)

            => OnGetCDRsResponse.WhenAll(Timestamp,
                                         API ?? this,
                                         Request,
                                         Response);

        #endregion


        #region (protected internal) GetCDRRequest (Request)

        /// <summary>
        /// An event sent whenever a GET CDR request was received.
        /// </summary>
        public OCPIRequestLogEvent OnGetCDRRequest = new ();

        /// <summary>
        /// An event sent whenever a GET CDR request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task GetCDRRequest(DateTime     Timestamp,
                                              HTTPAPI      API,
                                              OCPIRequest  Request)

            => OnGetCDRRequest.WhenAll(Timestamp,
                                       API ?? this,
                                       Request);

        #endregion

        #region (protected internal) GetCDRResponse(Response)

        /// <summary>
        /// An event sent whenever a GET CDR response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnGetCDRResponse = new ();

        /// <summary>
        /// An event sent whenever a GET CDR response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task GetCDRResponse(DateTime      Timestamp,
                                               HTTPAPI       API,
                                               OCPIRequest   Request,
                                               OCPIResponse  Response)

            => OnGetCDRResponse.WhenAll(Timestamp,
                                        API ?? this,
                                        Request,
                                        Response);

        #endregion

        #endregion

        #region Token(s)

        #region (protected internal) GetTokensRequest (Request)

        /// <summary>
        /// An event sent whenever a GET Tokens request was received.
        /// </summary>
        public OCPIRequestLogEvent OnGetTokensRequest = new ();

        /// <summary>
        /// An event sent whenever a GET Tokens request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task GetTokensRequest(DateTime     Timestamp,
                                                 HTTPAPI      API,
                                                 OCPIRequest  Request)

            => OnGetTokensRequest.WhenAll(Timestamp,
                                          API ?? this,
                                          Request);

        #endregion

        #region (protected internal) GetTokensResponse(Response)

        /// <summary>
        /// An event sent whenever a GET Tokens response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnGetTokensResponse = new ();

        /// <summary>
        /// An event sent whenever a GET Tokens response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task GetTokensResponse(DateTime      Timestamp,
                                                  HTTPAPI       API,
                                                  OCPIRequest   Request,
                                                  OCPIResponse  Response)

            => OnGetTokensResponse.WhenAll(Timestamp,
                                           API ?? this,
                                           Request,
                                           Response);

        #endregion


        #region (protected internal) DeleteTokensRequest (Request)

        /// <summary>
        /// An event sent whenever a DELETE Tokens request was received.
        /// </summary>
        public OCPIRequestLogEvent OnDeleteTokensRequest = new ();

        /// <summary>
        /// An event sent whenever a DELETE Tokens request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task DeleteTokensRequest(DateTime     Timestamp,
                                                    HTTPAPI      API,
                                                    OCPIRequest  Request)

            => OnDeleteTokensRequest.WhenAll(Timestamp,
                                             API ?? this,
                                             Request);

        #endregion

        #region (protected internal) DeleteTokensResponse(Response)

        /// <summary>
        /// An event sent whenever a DELETE Tokens response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnDeleteTokensResponse = new ();

        /// <summary>
        /// An event sent whenever a DELETE Tokens response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task DeleteTokensResponse(DateTime      Timestamp,
                                                     HTTPAPI       API,
                                                     OCPIRequest   Request,
                                                     OCPIResponse  Response)

            => OnDeleteTokensResponse.WhenAll(Timestamp,
                                              API ?? this,
                                              Request,
                                              Response);

        #endregion


        // Token

        #region (protected internal) GetTokenRequest (Request)

        /// <summary>
        /// An event sent whenever a GET Token request was received.
        /// </summary>
        public OCPIRequestLogEvent OnGetTokenRequest = new ();

        /// <summary>
        /// An event sent whenever a GET Token request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task GetTokenRequest(DateTime     Timestamp,
                                                HTTPAPI      API,
                                                OCPIRequest  Request)

            => OnGetTokenRequest.WhenAll(Timestamp,
                                         API ?? this,
                                         Request);

        #endregion

        #region (protected internal) GetTokenResponse(Response)

        /// <summary>
        /// An event sent whenever a GET Token response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnGetTokenResponse = new ();

        /// <summary>
        /// An event sent whenever a GET Token response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task GetTokenResponse(DateTime      Timestamp,
                                                 HTTPAPI       API,
                                                 OCPIRequest   Request,
                                                 OCPIResponse  Response)

            => OnGetTokenResponse.WhenAll(Timestamp,
                                          API ?? this,
                                          Request,
                                          Response);

        #endregion


        #region (protected internal) PostTokenRequest (Request)

        /// <summary>
        /// An event sent whenever a POST token request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPostTokenRequest = new ();

        /// <summary>
        /// An event sent whenever a POST token request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PostTokenRequest(DateTime     Timestamp,
                                                 HTTPAPI      API,
                                                 OCPIRequest  Request)

            => OnPostTokenRequest.WhenAll(Timestamp,
                                          API ?? this,
                                          Request);

        #endregion

        #region (protected internal) PostTokenResponse(Response)

        /// <summary>
        /// An event sent whenever a POST token response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnPostTokenResponse = new ();

        /// <summary>
        /// An event sent whenever a POST token response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PostTokenResponse(DateTime      Timestamp,
                                                  HTTPAPI       API,
                                                  OCPIRequest   Request,
                                                  OCPIResponse  Response)

            => OnPostTokenResponse.WhenAll(Timestamp,
                                           API ?? this,
                                           Request,
                                           Response);

        #endregion


        #region (protected internal) PutTokenRequest    (Request)

        /// <summary>
        /// An event sent whenever a put token request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPutTokenRequest = new OCPIRequestLogEvent();

        /// <summary>
        /// An event sent whenever a put token request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PutTokenRequest(DateTime     Timestamp,
                                                HTTPAPI      API,
                                                OCPIRequest  Request)

            => OnPutTokenRequest.WhenAll(Timestamp,
                                         API ?? this,
                                         Request);

        #endregion

        #region (protected internal) PutTokenResponse   (Response)

        /// <summary>
        /// An event sent whenever a put token response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnPutTokenResponse = new OCPIResponseLogEvent();

        /// <summary>
        /// An event sent whenever a put token response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PutTokenResponse(DateTime      Timestamp,
                                                 HTTPAPI       API,
                                                 OCPIRequest   Request,
                                                 OCPIResponse  Response)

            => OnPutTokenResponse.WhenAll(Timestamp,
                                          API ?? this,
                                          Request,
                                          Response);

        #endregion


        #region (protected internal) PatchTokenRequest  (Request)

        /// <summary>
        /// An event sent whenever a patch token request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPatchTokenRequest = new OCPIRequestLogEvent();

        /// <summary>
        /// An event sent whenever a patch token request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PatchTokenRequest(DateTime     Timestamp,
                                                  HTTPAPI      API,
                                                  OCPIRequest  Request)

            => OnPatchTokenRequest.WhenAll(Timestamp,
                                           API ?? this,
                                           Request);

        #endregion

        #region (protected internal) PatchTokenResponse (Response)

        /// <summary>
        /// An event sent whenever a patch token response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnPatchTokenResponse = new OCPIResponseLogEvent();

        /// <summary>
        /// An event sent whenever a patch token response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PatchTokenResponse(DateTime      Timestamp,
                                                   HTTPAPI       API,
                                                   OCPIRequest   Request,
                                                   OCPIResponse  Response)

            => OnPatchTokenResponse.WhenAll(Timestamp,
                                            API ?? this,
                                            Request,
                                            Response);

        #endregion


        #region (protected internal) DeleteTokenRequest (Request)

        /// <summary>
        /// An event sent whenever a delete token request was received.
        /// </summary>
        public OCPIRequestLogEvent OnDeleteTokenRequest = new OCPIRequestLogEvent();

        /// <summary>
        /// An event sent whenever a delete token request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task DeleteTokenRequest(DateTime     Timestamp,
                                                   HTTPAPI      API,
                                                   OCPIRequest  Request)

            => OnDeleteTokenRequest.WhenAll(Timestamp,
                                            API ?? this,
                                            Request);

        #endregion

        #region (protected internal) DeleteTokenResponse(Response)

        /// <summary>
        /// An event sent whenever a delete token response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnDeleteTokenResponse = new OCPIResponseLogEvent();

        /// <summary>
        /// An event sent whenever a delete token response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task DeleteTokenResponse(DateTime      Timestamp,
                                                    HTTPAPI       API,
                                                    OCPIRequest   Request,
                                                    OCPIResponse  Response)

            => OnDeleteTokenResponse.WhenAll(Timestamp,
                                             API ?? this,
                                             Request,
                                             Response);

        #endregion

        #endregion


        // Commands

        #region (protected internal) ReserveNowRequest        (Request)

        /// <summary>
        /// An event sent whenever a reserve now command was received.
        /// </summary>
        public OCPIRequestLogEvent OnReserveNowRequest = new ();

        /// <summary>
        /// An event sent whenever a reserve now command was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the command request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task ReserveNowRequest(DateTime     Timestamp,
                                                  HTTPAPI      API,
                                                  OCPIRequest  Request)

            => OnReserveNowRequest.WhenAll(Timestamp,
                                           API ?? this,
                                           Request);

        #endregion

        #region OnReserveNowCommand

        public delegate Task<CommandResponse> OnReserveNowCommandDelegate(EMSP_Id            EMSPId,
                                                                          ReserveNowCommand  ReserveNowCommand);

        public event OnReserveNowCommandDelegate? OnReserveNowCommand;

        #endregion

        #region (protected internal) ReserveNowResponse       (Response)

        /// <summary>
        /// An event sent whenever a reserve now command response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnReserveNowResponse = new ();

        /// <summary>
        /// An event sent whenever a reserve now command response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the command response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task ReserveNowResponse(DateTime      Timestamp,
                                                   HTTPAPI       API,
                                                   OCPIRequest   Request,
                                                   OCPIResponse  Response)

            => OnReserveNowResponse.WhenAll(Timestamp,
                                            API ?? this,
                                            Request,
                                            Response);

        #endregion


        #region (protected internal) CancelReservationRequest (Request)

        /// <summary>
        /// An event sent whenever a cancel reservation command was received.
        /// </summary>
        public OCPIRequestLogEvent OnCancelReservationRequest = new ();

        /// <summary>
        /// An event sent whenever a cancel reservation command was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the command request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task CancelReservationRequest(DateTime     Timestamp,
                                                         HTTPAPI      API,
                                                         OCPIRequest  Request)

            => OnCancelReservationRequest.WhenAll(Timestamp,
                                                  API ?? this,
                                                  Request);

        #endregion

        #region OnCancelReservationCommand

        public delegate Task<CommandResponse> OnCancelReservationCommandDelegate(EMSP_Id                   EMSPId,
                                                                                 CancelReservationCommand  CancelReservationCommand);

        public event OnCancelReservationCommandDelegate? OnCancelReservationCommand;

        #endregion

        #region (protected internal) CancelReservationResponse(Response)

        /// <summary>
        /// An event sent whenever a cancel reservation command response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnCancelReservationResponse = new ();

        /// <summary>
        /// An event sent whenever a cancel reservation command response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the command response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task CancelReservationResponse(DateTime      Timestamp,
                                                          HTTPAPI       API,
                                                          OCPIRequest   Request,
                                                          OCPIResponse  Response)

            => OnCancelReservationResponse.WhenAll(Timestamp,
                                                   API ?? this,
                                                   Request,
                                                   Response);

        #endregion


        #region (protected internal) StartSessionRequest      (Request)

        /// <summary>
        /// An event sent whenever a start session command was received.
        /// </summary>
        public OCPIRequestLogEvent OnStartSessionRequest = new ();

        /// <summary>
        /// An event sent whenever a start session command was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the command request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task StartSessionRequest(DateTime     Timestamp,
                                                    HTTPAPI      API,
                                                    OCPIRequest  Request)

            => OnStartSessionRequest.WhenAll(Timestamp,
                                             API ?? this,
                                             Request);

        #endregion

        #region OnStartSessionCommand

        public delegate Task<CommandResponse> OnStartSessionCommandDelegate(EMSP_Id              EMSPId,
                                                                            StartSessionCommand  StartSessionCommand);

        public event OnStartSessionCommandDelegate? OnStartSessionCommand;

        #endregion

        #region (protected internal) StartSessionResponse     (Response)

        /// <summary>
        /// An event sent whenever a start session command response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnStartSessionResponse = new ();

        /// <summary>
        /// An event sent whenever a start session command response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the command response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task StartSessionResponse(DateTime      Timestamp,
                                                     HTTPAPI       API,
                                                     OCPIRequest   Request,
                                                     OCPIResponse  Response)

            => OnStartSessionResponse.WhenAll(Timestamp,
                                              API ?? this,
                                              Request,
                                              Response);

        #endregion


        #region (protected internal) StopSessionRequest       (Request)

        /// <summary>
        /// An event sent whenever a stop session command was received.
        /// </summary>
        public OCPIRequestLogEvent OnStopSessionRequest = new ();

        /// <summary>
        /// An event sent whenever a stop session command was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the command request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task StopSessionRequest(DateTime     Timestamp,
                                                   HTTPAPI      API,
                                                   OCPIRequest  Request)

            => OnStopSessionRequest.WhenAll(Timestamp,
                                            API ?? this,
                                            Request);

        #endregion

        #region OnStopSessionCommand

        public delegate Task<CommandResponse> OnStopSessionCommandDelegate(EMSP_Id             EMSPId,
                                                                           StopSessionCommand  StopSessionCommand);

        public event OnStopSessionCommandDelegate? OnStopSessionCommand;

        #endregion

        #region (protected internal) StopSessionResponse      (Response)

        /// <summary>
        /// An event sent whenever a stop session command response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnStopSessionResponse = new ();

        /// <summary>
        /// An event sent whenever a stop session command response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the command response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task StopSessionResponse(DateTime      Timestamp,
                                                    HTTPAPI       API,
                                                    OCPIRequest   Request,
                                                    OCPIResponse  Response)

            => OnStopSessionResponse.WhenAll(Timestamp,
                                             API ?? this,
                                             Request,
                                             Response);

        #endregion


        #region (protected internal) UnlockConnectorRequest   (Request)

        /// <summary>
        /// An event sent whenever a unlock connector command was received.
        /// </summary>
        public OCPIRequestLogEvent OnUnlockConnectorRequest = new ();

        /// <summary>
        /// An event sent whenever a unlock connector command was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the command request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task UnlockConnectorRequest(DateTime     Timestamp,
                                                       HTTPAPI      API,
                                                       OCPIRequest  Request)

            => OnUnlockConnectorRequest.WhenAll(Timestamp,
                                                API ?? this,
                                                Request);

        #endregion

        #region OnUnlockConnectorCommand

        public delegate Task<CommandResponse> OnUnlockConnectorCommandDelegate(EMSP_Id                 EMSPId,
                                                                               UnlockConnectorCommand  UnlockConnectorCommand);

        public event OnUnlockConnectorCommandDelegate? OnUnlockConnectorCommand;

        #endregion

        #region (protected internal) UnlockConnectorResponse  (Response)

        /// <summary>
        /// An event sent whenever a unlock connector command response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnUnlockConnectorResponse = new ();

        /// <summary>
        /// An event sent whenever a unlock connector command response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the command response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task UnlockConnectorResponse(DateTime      Timestamp,
                                                        HTTPAPI       API,
                                                        OCPIRequest   Request,
                                                        OCPIResponse  Response)

            => OnUnlockConnectorResponse.WhenAll(Timestamp,
                                                 API ?? this,
                                                 Request,
                                                 Response);

        #endregion

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create an instance of the HTTP API for charge point operators
        /// using the given CommonAPI.
        /// </summary>
        /// <param name="CommonAPI">The OCPI common API.</param>
        /// <param name="AllowDowngrades">(Dis-)allow PUTting of object having an earlier 'LastUpdated'-timestamp then already existing objects.</param>
        /// 
        /// <param name="HTTPHostname">An optional HTTP hostname.</param>
        /// <param name="ExternalDNSName">The offical URL/DNS name of this service, e.g. for sending e-mails.</param>
        /// <param name="URLPathPrefix">An optional URL path prefix.</param>
        /// <param name="BasePath">When the API is served from an optional subdirectory path.</param>
        /// <param name="HTTPServiceName">An optional name of the HTTP API service.</param>
        public CPOAPI(CommonAPI                    CommonAPI,
                      Boolean?                     AllowDowngrades           = null,

                      HTTPHostname?                HTTPHostname              = null,
                      String?                      ExternalDNSName           = "",
                      String?                      HTTPServiceName           = DefaultHTTPServiceName,
                      HTTPPath?                    BasePath                  = null,

                      HTTPPath?                    URLPathPrefix             = null,
                      JObject?                     APIVersionHashes          = null,

                      Boolean?                     DisableMaintenanceTasks   = false,
                      TimeSpan?                    MaintenanceInitialDelay   = null,
                      TimeSpan?                    MaintenanceEvery          = null,

                      Boolean?                     DisableWardenTasks        = false,
                      TimeSpan?                    WardenInitialDelay        = null,
                      TimeSpan?                    WardenCheckEvery          = null,

                      Boolean?                     IsDevelopment             = false,
                      IEnumerable<String>?         DevelopmentServers        = null,
                      Boolean?                     DisableLogging            = false,
                      String?                      LoggingContext            = null,
                      String?                      LoggingPath               = null,
                      String?                      LogfileName               = null,
                      OCPILogfileCreatorDelegate?  LogfileCreator            = null,
                      Boolean                      AutoStart                 = false)

            : base(CommonAPI.HTTPServer,
                   HTTPHostname,
                   ExternalDNSName,
                   HTTPServiceName ?? DefaultHTTPServiceName,
                   BasePath,

                   URLPathPrefix   ?? DefaultURLPathPrefix,
                   null, //HTMLTemplate,
                   APIVersionHashes,

                   DisableMaintenanceTasks,
                   MaintenanceInitialDelay,
                   MaintenanceEvery,

                   DisableWardenTasks,
                   WardenInitialDelay,
                   WardenCheckEvery,

                   IsDevelopment,
                   DevelopmentServers,
                   DisableLogging,
                   LoggingPath,
                   LogfileName     ?? DefaultLogfileName,
                   LogfileCreator is not null
                       ? (loggingPath, context, logfileName) => LogfileCreator(loggingPath, null, context, logfileName)
                       : null,
                   AutoStart)

        {

            this.CommonAPI           = CommonAPI;
            this.AllowDowngrades     = AllowDowngrades;

            this.CPOAPILogger        = this.DisableLogging == false
                                           ? new CPOAPILogger(
                                                 this,
                                                 LoggingContext,
                                                 LoggingPath,
                                                 LogfileCreator
                                             )
                                           : null;

            RegisterURLTemplates();

        }

        #endregion


        #region (private) RegisterURLTemplates()

        private void RegisterURLTemplates()
        {

            #region GET    [/cpo] == /

            //HTTPServer.RegisterResourcesFolder(HTTPHostname.Any,
            //                                   URLPathPrefix + "/cpo", "cloud.charging.open.protocols.OCPIv2_2_1.HTTPAPI.CPOAPI.HTTPRoot",
            //                                   Assembly.GetCallingAssembly());

            //CommonAPI.AddOCPIMethod(HTTPHostname.Any,
            //                             HTTPMethod.GET,
            //                             new HTTPPath[] {
            //                                 URLPathPrefix + "/cpo/index.html",
            //                                 URLPathPrefix + "/cpo/"
            //                             },
            //                             HTTPContentType.Text.HTML_UTF8,
            //                             OCPIRequest: async Request => {

            //                                 var _MemoryStream = new MemoryStream();
            //                                 typeof(CPOAPI).Assembly.GetManifestResourceStream("cloud.charging.open.protocols.OCPIv2_2_1.HTTPAPI.CPOAPI.HTTPRoot._header.html").SeekAndCopyTo(_MemoryStream, 3);
            //                                 typeof(CPOAPI).Assembly.GetManifestResourceStream("cloud.charging.open.protocols.OCPIv2_2_1.HTTPAPI.CPOAPI.HTTPRoot._footer.html").SeekAndCopyTo(_MemoryStream, 3);

            //                                 return new HTTPResponse.Builder(Request.HTTPRequest) {
            //                                     HTTPStatusCode  = HTTPStatusCode.OK,
            //                                     Server          = DefaultHTTPServerName,
            //                                     Date            = Timestamp.Now,
            //                                     ContentType     = HTTPContentType.Text.HTML_UTF8,
            //                                     Content         = _MemoryStream.ToArray(),
            //                                     Connection      = ConnectionType.Close
            //                                 };

            //                             });

            #endregion


            #region ~/locations

            #region OPTIONS  ~/locations

            CommonAPI.AddOCPIMethod(

                HTTPHostname.Any,
                HTTPMethod.OPTIONS,
                URLPathPrefix + "locations",
                OCPIRequestHandler: request =>

                    Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               HTTPResponseBuilder = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode              = HTTPStatusCode.OK,
                                   Allow                       = [ HTTPMethod.OPTIONS, HTTPMethod.GET ],
                                   AccessControlAllowMethods   = [ "OPTIONS", "GET" ],
                                   AccessControlAllowHeaders   = [ "Authorization" ],
                                   AccessControlExposeHeaders  = [ "X-Request-ID", "X-Correlation-ID", "Link", "X-Total-Count", "X-Filtered-Count" ]
                               }
                        })

            );

            #endregion

            #region GET      ~/locations

            // https://example.com/ocpi/2.2/cpo/locations/?date_from=2019-01-28T12:00:00&date_to=2019-01-29T12:00:00&offset=50&limit=100
            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.GET,
                                    URLPathPrefix + "locations",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestLogger:   GetLocationsRequest,
                                    OCPIResponseLogger:  GetLocationsResponse,
                                    OCPIRequestHandler:  Request => {

                                        #region Check access token

                                        if ((Request.LocalAccessInfo is not null || CommonAPI.BaseAPI.LocationsAsOpenData == false) &&
                                            (Request.LocalAccessInfo?.Status            != AccessStatus.ALLOWED ||
                                             Request.LocalAccessInfo?.IsNot(Roles.EMSP) == true))
                                        {


                                        //if (Request.LocalAccessInfo?.IsNot(Roles.EMSP) == true ||
                                        //    Request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                                        //{

                                            return Task.FromResult(
                                                new OCPIResponse.Builder(Request) {
                                                    StatusCode           = 2000,
                                                    StatusMessage        = "Invalid or blocked access token!",
                                                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                        HTTPStatusCode              = HTTPStatusCode.Forbidden,
                                                        AccessControlAllowMethods   = [ "OPTIONS", "GET", "DELETE" ],
                                                        AccessControlAllowHeaders   = [ "Authorization" ],
                                                        AccessControlExposeHeaders  = ["X-Request-ID", "X-Correlation-ID", "Link", "X-Total-Count", "X-Filtered-Count"]
                                                    }
                                                });

                                        }

                                        #endregion


                                        //var emspId               = Request.LocalAccessInfo is not null
                                        //                               ? EMSP_Id.Parse(Request.LocalAccessInfo.CountryCode, Request.LocalAccessInfo.PartyId)
                                        //                               : new EMSP_Id?();

                                        var withExtensions       = Request.QueryString.GetBoolean ("withExtensions", false);


                                        var filters              = Request.GetDateAndPaginationFilters();
                                        var matchFilter          = Request.QueryString.CreateStringFilter<Location>(
                                                                       "match",
                                                                       (location, pattern) => location.Id.     ToString().Contains(pattern)         ||
                                                                                              location.Name?.             Contains(pattern) == true ||
                                                                                              location.Address.           Contains(pattern)         ||
                                                                                              location.City.              Contains(pattern)         ||
                                                                                              location.PostalCode.        Contains(pattern)         ||
                                                                                              location.Country.ToString().Contains(pattern)         ||
                                                                                              location.Directions.        Matches (pattern)         ||
                                                                                              location.Operator?.   Name. Contains(pattern) == true ||
                                                                                              location.SubOperator?.Name. Contains(pattern) == true ||
                                                                                              location.Owner?.      Name. Contains(pattern) == true ||
                                                                                              //location.Facilities.        Matches (pattern)         ||
                                                                                              location.EVSEUIds.          Matches (pattern)         ||
                                                                                              location.EVSEIds.           Matches (pattern)         
                                                                                              //location.EVSEs.Any(evse => evse.Connectors.Any(connector => connector?.GetTariffId(emspId).ToString()?.Contains(pattern) == true))
                                                                   );

                                                                                          //ToDo: Filter to NOT show all locations to everyone!
                                        var allLocations       = CommonAPI.GetLocations().//location => Request.AccessInfo.Value.Roles.Any(role => role.CountryCode == location.CountryCode &&
                                                                                          //                                                       role.PartyId     == location.PartyId)).
                                                                           ToArray();

                                        var filteredLocations  = allLocations.Where(matchFilter).
                                                                              Where(location => !filters.From.HasValue || location.LastUpdated >  filters.From.Value).
                                                                              Where(location => !filters.To.  HasValue || location.LastUpdated <= filters.To.  Value).
                                                                              ToArray();


                                        var httpResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                                       HTTPStatusCode              = HTTPStatusCode.OK,
                                                                       Server                      = DefaultHTTPServerName,
                                                                       Date                        = Timestamp.Now,
                                                                       AccessControlAllowMethods   = [ "OPTIONS", "GET" ],
                                                                       AccessControlAllowHeaders   = [ "Authorization" ],
                                                                       AccessControlExposeHeaders  = [ "X-Request-ID", "X-Correlation-ID", "Link", "X-Total-Count", "X-Filtered-Count" ]
                                                                   }.

                                                                   // The overall number of locations
                                                                   Set("X-Total-Count",     allLocations.     Length).

                                                                   // The number of locations matching search filters
                                                                   Set("X-Filtered-Count",  filteredLocations.Length).

                                                                   // The maximum number of locations that the server WILL return within a single request
                                                                   Set("X-Limit",           allLocations.     Length);


                                        #region When the limit query parameter was set & this is not the last pagination page...

                                        if (filters.Limit.HasValue &&
                                            allLocations.ULongLength() > ((filters.Offset ?? 0) + (filters.Limit ?? 0)))
                                        {

                                            // The new query parameters for the "next" page of pagination within the HTTP Link header
                                            var queryParameters    = new List<String?>() {
                                                                         filters.From. HasValue ? $"from={filters.From.Value}" :                             null,
                                                                         filters.To.   HasValue ? $"to={filters.To.Value}" :                                 null,
                                                                         filters.Limit.HasValue ? $"offset={(filters.Offset ?? 0) + (filters.Limit ?? 0)}" : null,
                                                                         filters.Limit.HasValue ? $"limit={filters.Limit ?? 0}" :                            null
                                                                     }.Where(queryParameter => queryParameter is not null).
                                                                       AggregateWith("&");

                                            if (queryParameters.Length > 0)
                                                queryParameters = "?" + queryParameters;

                                            // Link to the 'next' page should be provided when this is NOT the last page, e.g.:
                                            //   - Link: <https://www.server.com/ocpi/cpo/2.0/cdrs/?offset=150&limit=50>; rel="next"
                                            httpResponseBuilder.Set("Link", $"<{(ExternalDNSName.IsNotNullOrEmpty()
                                                                                     ? $"https://{ExternalDNSName}"
                                                                                     : $"http://127.0.0.1:{HTTPServer.IPPorts.First()}")}{URLPathPrefix}/locations{queryParameters}>; rel=\"next\"");

                                        }

                                        #endregion


                                        return Task.FromResult(
                                                   new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 1000,
                                                       StatusMessage        = "Hello world!",
                                                       HTTPResponseBuilder  = httpResponseBuilder,
                                                       Data                 = new JArray(
                                                                                  filteredLocations.
                                                                                      OrderBy       (location => location.Created).
                                                                                      SkipTakeFilter(filters.Offset,
                                                                                                     filters.Limit).
                                                                                      Select        (location => location.ToJSON(Request.EMSPId,
                                                                                                                                 CustomLocationSerializer,
                                                                                                                                 CustomPublishTokenSerializer,
                                                                                                                                 CustomAdditionalGeoLocationSerializer,
                                                                                                                                 CustomEVSESerializer,
                                                                                                                                 CustomStatusScheduleSerializer,
                                                                                                                                 CustomConnectorSerializer,
                                                                                                                                 CustomLocationEnergyMeterSerializer,
                                                                                                                                 CustomEVSEEnergyMeterSerializer,
                                                                                                                                 CustomTransparencySoftwareStatusSerializer,
                                                                                                                                 CustomTransparencySoftwareSerializer,
                                                                                                                                 CustomDisplayTextSerializer,
                                                                                                                                 CustomBusinessDetailsSerializer,
                                                                                                                                 CustomHoursSerializer,
                                                                                                                                 CustomImageSerializer,
                                                                                                                                 CustomEnergyMixSerializer,
                                                                                                                                 CustomEnergySourceSerializer,
                                                                                                                                 CustomEnvironmentalImpactSerializer))
                                                                              )
                                                   }
                                               );

                                    });

                                    //    return Task.FromResult(
                                    //        new OCPIResponse.Builder(Request) {
                                    //                StatusCode           = 1000,
                                    //                StatusMessage        = "Hello world!",
                                    //                Data                 = new JArray(filteredLocations.SkipTakeFilter(filters.Offset,
                                    //                                                                                   filters.Limit).
                                    //                                                                    SafeSelect(location => location.ToJSON(Request.EMSPId,
                                                                                                                                               
                                    //                HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                    //                    HTTPStatusCode             = HTTPStatusCode.OK,
                                    //                    AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                    //                    AccessControlAllowHeaders  = [ "Authorization" ]
                                    //                    //LastModified               = ?
                                    //                }.
                                    //                Set("X-Total-Count", allLocations.Length)
                                    //                // X-Limit               The maximum number of objects that the server WILL return.
                                    //                // Link                  Link to the 'next' page should be provided when this is NOT the last page.
                                    //        });

                                    //});

            #endregion

            #endregion

            #region ~/locations/{locationId}

            #region OPTIONS  ~/locations/{locationId}

            CommonAPI.AddOCPIMethod(

                HTTPHostname.Any,
                HTTPMethod.OPTIONS,
                URLPathPrefix + "locations/{locationId}",
                OCPIRequestHandler: request =>

                    Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               HTTPResponseBuilder = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   Allow                      = [ HTTPMethod.OPTIONS, HTTPMethod.GET ],
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ]
                               }
                        })

            );

            #endregion

            #region GET      ~/locations/{locationId}

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.GET,
                                    URLPathPrefix + "locations/{locationId}",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestLogger:   GetLocationRequest,
                                    OCPIResponseLogger:  GetLocationResponse,
                                    OCPIRequestHandler:  Request => {

                                        #region Check access token

                                        if (Request.LocalAccessInfo?.IsNot(Roles.EMSP) == true ||
                                            Request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                                        {

                                            return Task.FromResult(
                                                new OCPIResponse.Builder(Request) {
                                                    StatusCode           = 2000,
                                                    StatusMessage        = "Invalid or blocked access token!",
                                                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                        AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                                        AccessControlAllowHeaders  = [ "Authorization" ]
                                                    }
                                                });

                                        }

                                        #endregion

                                        #region Check location

                                        if (!Request.ParseLocation(CommonAPI,
                                                                   //Request.AccessInfo.Value.Roles.Select(role => new Tuple<CountryCode, Party_Id>(role.CountryCode, role.PartyId)),
                                                                   CommonAPI.OurCredentialRoles.Select(credentialRole => new Tuple<CountryCode, Party_Id>(credentialRole.CountryCode, credentialRole.PartyId)),
                                                                   out var locationId,
                                                                   out var location,
                                                                   out var ocpiResponseBuilder,
                                                                   FailOnMissingLocation: true) ||
                                             location is null)
                                        {
                                            return Task.FromResult(ocpiResponseBuilder!);
                                        }

                                        #endregion


                                        return Task.FromResult(
                                            new OCPIResponse.Builder(Request) {
                                                   StatusCode           = 1000,
                                                   StatusMessage        = "Hello world!",
                                                   Data                 = location.ToJSON(Request.EMSPId,
                                                                                          CustomLocationSerializer,
                                                                                          CustomPublishTokenSerializer,
                                                                                          CustomAdditionalGeoLocationSerializer,
                                                                                          CustomEVSESerializer,
                                                                                          CustomStatusScheduleSerializer,
                                                                                          CustomConnectorSerializer,
                                                                                          CustomLocationEnergyMeterSerializer,
                                                                                          CustomEVSEEnergyMeterSerializer,
                                                                                          CustomTransparencySoftwareStatusSerializer,
                                                                                          CustomTransparencySoftwareSerializer,
                                                                                          CustomDisplayTextSerializer,
                                                                                          CustomBusinessDetailsSerializer,
                                                                                          CustomHoursSerializer,
                                                                                          CustomImageSerializer,
                                                                                          CustomEnergyMixSerializer,
                                                                                          CustomEnergySourceSerializer,
                                                                                          CustomEnvironmentalImpactSerializer),
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                                       AccessControlAllowHeaders  = [ "Authorization" ],
                                                       LastModified               = location.LastUpdated,
                                                       ETag                       = location.ETag
                                                   }
                                            });

                                    });

            #endregion

            #endregion

            #region ~/locations/{locationId}/{evseId}

            #region OPTIONS  ~/locations/{locationId}/{evseId}

            CommonAPI.AddOCPIMethod(

                HTTPHostname.Any,
                HTTPMethod.OPTIONS,
                URLPathPrefix + "locations/{locationId}/{evseId}",
                OCPIRequestHandler: request =>

                    Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               HTTPResponseBuilder = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   Allow                      = [ HTTPMethod.OPTIONS, HTTPMethod.GET ],
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ]
                               }
                        })

            );

            #endregion

            #region GET      ~/locations/{locationId}/{evseId}

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.GET,
                                    URLPathPrefix + "locations/{locationId}/{evseId}",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestHandler: Request => {

                                        #region Check access token

                                        if (Request.LocalAccessInfo?.IsNot(Roles.EMSP) == true ||
                                            Request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                                        {

                                            return Task.FromResult(
                                                new OCPIResponse.Builder(Request) {
                                                    StatusCode           = 2000,
                                                    StatusMessage        = "Invalid or blocked access token!",
                                                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                        AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                                        AccessControlAllowHeaders  = [ "Authorization" ]
                                                    }
                                                });

                                        }

                                        #endregion

                                        #region Check EVSE

                                        if (!Request.ParseLocationEVSE(CommonAPI,
                                                                       //Request.AccessInfo.Value.Roles.Select(role => new Tuple<CountryCode, Party_Id>(role.CountryCode, role.PartyId)),
                                                                       CommonAPI.OurCredentialRoles.Select(credentialRole => new Tuple<CountryCode, Party_Id>(credentialRole.CountryCode, credentialRole.PartyId)),
                                                                       out var locationId,
                                                                       out var location,
                                                                       out var evseId,
                                                                       out var evse,
                                                                       out var ocpiResponseBuilder) ||
                                             evse is null)
                                        {
                                            return Task.FromResult(ocpiResponseBuilder!);
                                        }

                                        #endregion


                                        return Task.FromResult(
                                            new OCPIResponse.Builder(Request) {
                                                   StatusCode           = 1000,
                                                   StatusMessage        = "Hello world!",
                                                   Data                 = evse.ToJSON(Request.EMSPId,
                                                                                      CustomEVSESerializer,
                                                                                      CustomStatusScheduleSerializer,
                                                                                      CustomConnectorSerializer,
                                                                                      CustomEVSEEnergyMeterSerializer,
                                                                                      CustomTransparencySoftwareStatusSerializer,
                                                                                      CustomTransparencySoftwareSerializer,
                                                                                      CustomDisplayTextSerializer,
                                                                                      CustomImageSerializer),
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                                       AccessControlAllowHeaders  = [ "Authorization" ],
                                                       LastModified               = evse.LastUpdated,
                                                       ETag                       = evse.ETag
                                                   }
                                            });

                                    });

            #endregion

            #endregion

            #region ~/locations/{locationId}/{evseId}/{connectorId}

            #region OPTIONS  ~/locations/{locationId}/{evseId}/{connectorId}

            CommonAPI.AddOCPIMethod(

                HTTPHostname.Any,
                HTTPMethod.OPTIONS,
                URLPathPrefix + "locations/{locationId}/{evseId}/{connectorId}",
                OCPIRequestHandler: request =>

                    Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               HTTPResponseBuilder = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   Allow                      = [ HTTPMethod.OPTIONS, HTTPMethod.GET ],
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ]
                               }
                        })

            );

            #endregion

            #region GET      ~/locations/{locationId}/{evseId}/{connectorId}

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.GET,
                                    URLPathPrefix + "locations/{locationId}/{evseId}/{connectorId}",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestHandler: Request => {

                                        #region Check access token

                                        if (Request.LocalAccessInfo?.IsNot(Roles.EMSP) == true ||
                                            Request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                                        {

                                            return Task.FromResult(
                                                new OCPIResponse.Builder(Request) {
                                                    StatusCode           = 2000,
                                                    StatusMessage        = "Invalid or blocked access token!",
                                                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                        AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                                        AccessControlAllowHeaders  = [ "Authorization" ]
                                                    }
                                                });

                                        }

                                        #endregion

                                        #region Check connector

                                        if (!Request.ParseLocationEVSEConnector(CommonAPI,
                                                                                //Request.AccessInfo.Value.Roles.Select(role => new Tuple<CountryCode, Party_Id>(role.CountryCode, role.PartyId)),
                                                                                CommonAPI.OurCredentialRoles.Select(credentialRole => new Tuple<CountryCode, Party_Id>(credentialRole.CountryCode, credentialRole.PartyId)),
                                                                                out var locationId,
                                                                                out var location,
                                                                                out var evseId,
                                                                                out var evse,
                                                                                out var connectorId,
                                                                                out var connector,
                                                                                out var ocpiResponseBuilder) ||
                                             connector is null)
                                        {
                                            return Task.FromResult(ocpiResponseBuilder!);
                                        }

                                        #endregion


                                        return Task.FromResult(
                                            new OCPIResponse.Builder(Request) {
                                                   StatusCode           = 1000,
                                                   StatusMessage        = "Hello world!",
                                                   Data                 = connector.ToJSON(Request.EMSPId,
                                                                                           CustomConnectorSerializer),
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                                       AccessControlAllowHeaders  = [ "Authorization" ],
                                                       LastModified               = connector.LastUpdated,
                                                       ETag                       = connector.ETag
                                                   }
                                            });

                                    });

            #endregion

            #endregion


            #region ~/tariffs

            #region OPTIONS  ~/tariffs

            CommonAPI.AddOCPIMethod(

                HTTPHostname.Any,
                HTTPMethod.OPTIONS,
                URLPathPrefix + "tariffs",
                OCPIRequestHandler: request =>

                    Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               HTTPResponseBuilder = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode              = HTTPStatusCode.OK,
                                   Allow                       = [ HTTPMethod.OPTIONS, HTTPMethod.GET ],
                                   AccessControlAllowMethods   = [ "OPTIONS", "GET" ],
                                   AccessControlAllowHeaders   = [ "Authorization" ],
                                   AccessControlExposeHeaders  = [ "X-Request-ID", "X-Correlation-ID", "Link", "X-Total-Count", "X-Filtered-Count" ]
                               }
                        })

            );

            #endregion

            #region GET      ~/tariffs

            // https://example.com/ocpi/2.2/cpo/tariffs/?date_from=2019-01-28T12:00:00&date_to=2019-01-29T12:00:00&offset=50&limit=100
            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.GET,
                                    URLPathPrefix + "tariffs",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestHandler: Request => {

                                        #region Check access token

                                        if (Request.LocalAccessInfo?.IsNot(Roles.EMSP) == true ||
                                            Request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                                        {

                                            return Task.FromResult(
                                                new OCPIResponse.Builder(Request) {
                                                    StatusCode           = 2000,
                                                    StatusMessage        = "Invalid or blocked access token!",
                                                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                        AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                                        AccessControlAllowHeaders  = [ "Authorization" ]
                                                    }
                                                });

                                        }

                                        #endregion


                                        var filters          = Request.GetDateAndPaginationFilters();

                                        var allTariffs       = CommonAPI.//GetTariffs(tariff => Request.AccessInfo.Value.Roles.Any(role => role.CountryCode == tariff.CountryCode &&
                                                                         //                                                                role.PartyId     == tariff.PartyId)).
                                                                         GetTariffs(tariff => CommonAPI.OurCredentialRoles.Any(credentialRole => credentialRole.CountryCode == tariff.CountryCode &&
                                                                                                                                                 credentialRole.PartyId     == tariff.PartyId)).
                                                                         ToArray();

                                        var filteredTariffs  = allTariffs.Where(tariff => !filters.From.HasValue || tariff.LastUpdated >  filters.From.Value).
                                                                          Where(tariff => !filters.To.  HasValue || tariff.LastUpdated <= filters.To.  Value).
                                                                          ToArray();


                                        var httpResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                                       Server                     = DefaultHTTPServerName,
                                                                       Date                       = Timestamp.Now,
                                                                       AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                                                   }.

                                                                   // The overall number of tariffs
                                                                   Set("X-Total-Count",  allTariffs.Length).

                                                                   // The maximum number of tariffs that the server WILL return within a single request
                                                                   Set("X-Limit",        allTariffs.Length);


                                        #region When the limit query parameter was set & this is not the last pagination page...

                                        if (filters.Limit.HasValue &&
                                            allTariffs.ULongLength() > ((filters.Offset ?? 0) + (filters.Limit ?? 0)))
                                        {

                                            // The new query parameters for the "next" page of pagination within the HTTP Link header
                                            var queryParameters    = new List<String?>() {
                                                                         filters.From. HasValue ? $"from={filters.From.Value}" :                             null,
                                                                         filters.To.   HasValue ? $"to={filters.To.Value}" :                                 null,
                                                                         filters.Limit.HasValue ? $"offset={(filters.Offset ?? 0) + (filters.Limit ?? 0)}" : null,
                                                                         filters.Limit.HasValue ? $"limit={filters.Limit ?? 0}" :                            null
                                                                     }.Where(queryParameter => queryParameter is not null).
                                                                       AggregateWith("&");

                                            if (queryParameters.Length > 0)
                                                queryParameters = "?" + queryParameters;

                                            // Link to the 'next' page should be provided when this is NOT the last page, e.g.:
                                            //   - Link: <https://www.server.com/ocpi/cpo/2.0/cdrs/?offset=150&limit=50>; rel="next"
                                            httpResponseBuilder.Set("Link", $"<{(ExternalDNSName.IsNotNullOrEmpty()
                                                                                     ? $"https://{ExternalDNSName}"
                                                                                     : $"http://127.0.0.1:{HTTPServer.IPPorts.First()}")}{URLPathPrefix}/tariffs{queryParameters}>; rel=\"next\"");

                                        }

                                        #endregion

                                        return Task.FromResult(
                                                   new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 1000,
                                                       StatusMessage        = "Hello world!",
                                                       HTTPResponseBuilder  = httpResponseBuilder,
                                                       Data                 = new JArray(
                                                                                  filteredTariffs.
                                                                                  OrderBy       (tariff => tariff.Created).
                                                                                  SkipTakeFilter(filters.Offset,
                                                                                                 filters.Limit).
                                                                                  Select        (tariff => tariff.ToJSON(true,
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
                                                                              )
                                                   }
                                               );

                                    });

            #endregion

            #endregion

            #region ~/tariffs/{tariffId}        [NonStandard]

            #region OPTIONS  ~/tariffs/{tariffId}

            CommonAPI.AddOCPIMethod(

                HTTPHostname.Any,
                HTTPMethod.OPTIONS,
                URLPathPrefix + "tariffs/{tariffId}",
                OCPIRequestHandler: request =>

                    Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               HTTPResponseBuilder = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   Allow                      = [ HTTPMethod.OPTIONS, HTTPMethod.GET ],
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ]
                               }
                        })

            );

            #endregion

            #region GET      ~/tariffs/{tariffId}

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.GET,
                                    URLPathPrefix + "tariffs/{tariffId}",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestHandler: Request => {

                                        #region Check access token

                                        if (Request.LocalAccessInfo?.IsNot(Roles.EMSP) == true ||
                                            Request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                                        {

                                            return Task.FromResult(
                                                new OCPIResponse.Builder(Request) {
                                                    StatusCode           = 2000,
                                                    StatusMessage        = "Invalid or blocked access token!",
                                                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                        AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                                        AccessControlAllowHeaders  = [ "Authorization" ]
                                                    }
                                                });

                                        }

                                        #endregion

                                        #region Check tariff

                                        if (!Request.ParseTariff(CommonAPI,
                                                                 //Request.AccessInfo.Value.Roles.Select(role => new Tuple<CountryCode, Party_Id>(role.CountryCode, role.PartyId)),
                                                                 CommonAPI.OurCredentialRoles.Select(credentialRole => new Tuple<CountryCode, Party_Id>(credentialRole.CountryCode, credentialRole.PartyId)),
                                                                 out var tariffId,
                                                                 out var tariff,
                                                                 out var ocpiResponseBuilder) ||
                                             tariff is null)
                                        {
                                            return Task.FromResult(ocpiResponseBuilder!);
                                        }

                                        #endregion


                                        return Task.FromResult(
                                            new OCPIResponse.Builder(Request) {
                                                   StatusCode           = 1000,
                                                   StatusMessage        = "Hello world!",
                                                   Data                 = tariff.ToJSON(),
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                                       AccessControlAllowHeaders  = [ "Authorization" ],
                                                       LastModified               = tariff.LastUpdated,
                                                       ETag                       = tariff.ETag
                                                   }
                                            });

                                    });

            #endregion

            #endregion


            #region ~/sessions

            #region OPTIONS  ~/sessions

            CommonAPI.AddOCPIMethod(

                HTTPHostname.Any,
                HTTPMethod.OPTIONS,
                URLPathPrefix + "sessions",
                OCPIRequestHandler: request =>

                    Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               HTTPResponseBuilder = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode              = HTTPStatusCode.OK,
                                   Allow                       = [ HTTPMethod.OPTIONS, HTTPMethod.GET ],
                                   AccessControlAllowMethods   = [ "OPTIONS", "GET" ],
                                   AccessControlAllowHeaders   = [ "Authorization" ],
                                   AccessControlExposeHeaders  = [ "X-Request-ID", "X-Correlation-ID", "Link", "X-Total-Count", "X-Filtered-Count" ]
                               }
                        })

            );

            #endregion

            #region GET      ~/sessions

            // https://example.com/ocpi/2.2/cpo/sessions/?date_from=2019-01-28T12:00:00&date_to=2019-01-29T12:00:00&offset=50&limit=100
            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.GET,
                                    URLPathPrefix + "sessions",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestHandler: Request => {

                                        #region Check access token

                                        if (Request.LocalAccessInfo?.IsNot(Roles.EMSP) == true ||
                                            Request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                                        {

                                            return Task.FromResult(
                                                new OCPIResponse.Builder(Request) {
                                                    StatusCode           = 2000,
                                                    StatusMessage        = "Invalid or blocked access token!",
                                                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                        AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                                        AccessControlAllowHeaders  = [ "Authorization" ]
                                                    }
                                                });

                                        }

                                        #endregion


                                        var filters           = Request.GetDateAndPaginationFilters();

                                        var allSessions       = CommonAPI.//GetSessions(session => Request.AccessInfo.Value.Roles.Any(role => role.CountryCode == session.CountryCode &&
                                                                          //                                                                  role.PartyId     == session.PartyId)).
                                                                          GetSessions(session => CommonAPI.OurCredentialRoles.Any(credentialRole => credentialRole.CountryCode == session.CountryCode &&
                                                                                                                                                    credentialRole.PartyId     == session.PartyId)).
                                                                          ToArray();

                                        var filteredSessions  = allSessions.Where(session => !filters.From.HasValue || session.LastUpdated >  filters.From.Value).
                                                                            Where(session => !filters.To.  HasValue || session.LastUpdated <= filters.To.  Value).
                                                                            ToArray();


                                        var httpResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                                       Server                     = DefaultHTTPServerName,
                                                                       Date                       = Timestamp.Now,
                                                                       AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                                                   }.

                                                                   // The overall number of sessions
                                                                   Set("X-Total-Count",  allSessions.Length).

                                                                   // The maximum number of sessions that the server WILL return within a single request
                                                                   Set("X-Limit",        allSessions.Length);


                                        #region When the limit query parameter was set & this is not the last pagination page...

                                        if (filters.Limit.HasValue &&
                                            allSessions.ULongLength() > ((filters.Offset ?? 0) + (filters.Limit ?? 0)))
                                        {

                                            // The new query parameters for the "next" page of pagination within the HTTP Link header
                                            var queryParameters    = new List<String?>() {
                                                                         filters.From. HasValue ? $"from={filters.From.Value}" :                             null,
                                                                         filters.To.   HasValue ? $"to={filters.To.Value}" :                                 null,
                                                                         filters.Limit.HasValue ? $"offset={(filters.Offset ?? 0) + (filters.Limit ?? 0)}" : null,
                                                                         filters.Limit.HasValue ? $"limit={filters.Limit ?? 0}" :                            null
                                                                     }.Where(queryParameter => queryParameter is not null).
                                                                       AggregateWith("&");

                                            if (queryParameters.Length > 0)
                                                queryParameters = "?" + queryParameters;

                                            // Link to the 'next' page should be provided when this is NOT the last page, e.g.:
                                            //   - Link: <https://www.server.com/ocpi/cpo/2.0/cdrs/?offset=150&limit=50>; rel="next"
                                            httpResponseBuilder.Set("Link", $"<{(ExternalDNSName.IsNotNullOrEmpty()
                                                                                     ? $"https://{ExternalDNSName}"
                                                                                     : $"http://127.0.0.1:{HTTPServer.IPPorts.First()}")}{URLPathPrefix}/sessions{queryParameters}>; rel=\"next\"");

                                        }

                                        #endregion

                                        return Task.FromResult(
                                                   new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 1000,
                                                       StatusMessage        = "Hello world!",
                                                       HTTPResponseBuilder  = httpResponseBuilder,
                                                       Data                 = new JArray(
                                                                                  filteredSessions.
                                                                                      OrderBy       (session => session.Created).
                                                                                      SkipTakeFilter(filters.Offset,
                                                                                                     filters.Limit).
                                                                                      Select        (session => session.ToJSON(CustomSessionSerializer,
                                                                                                                               CustomCDRTokenSerializer,
                                                                                                                               CustomChargingPeriodSerializer,
                                                                                                                               CustomCDRDimensionSerializer,
                                                                                                                               CustomPriceSerializer))
                                                                              )
                                                   }
                                               );

                                    });

            #endregion

            #endregion

            #region ~/sessions/{sessionId}      [NonStandard]

            #region OPTIONS  ~/sessions/{sessionId}

            CommonAPI.AddOCPIMethod(

                HTTPHostname.Any,
                HTTPMethod.OPTIONS,
                URLPathPrefix + "sessions/{sessionId}",
                OCPIRequestHandler: request =>

                    Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               HTTPResponseBuilder = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   Allow                      = [ HTTPMethod.OPTIONS, HTTPMethod.GET ],
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ]
                               }
                        })

            );

            #endregion

            #region GET      ~/sessions/{sessionId}

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.GET,
                                    URLPathPrefix + "sessions/{sessionId}",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestHandler: Request => {

                                        #region Check access token

                                        if (Request.LocalAccessInfo?.IsNot(Roles.EMSP) == true ||
                                            Request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                                        {

                                            return Task.FromResult(
                                                new OCPIResponse.Builder(Request) {
                                                    StatusCode           = 2000,
                                                    StatusMessage        = "Invalid or blocked access token!",
                                                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                        AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                                        AccessControlAllowHeaders  = [ "Authorization" ]
                                                    }
                                                });

                                        }

                                        #endregion

                                        #region Check session

                                        if (!Request.ParseSession(CommonAPI,
                                                                  //Request.AccessInfo.Value.Roles.Select(role => new Tuple<CountryCode, Party_Id>(role.CountryCode, role.PartyId)),
                                                                  CommonAPI.OurCredentialRoles.Select(credentialRole => new Tuple<CountryCode, Party_Id>(credentialRole.CountryCode, credentialRole.PartyId)),
                                                                  out var sessionId,
                                                                  out var session,
                                                                  out var ocpiResponseBuilder) ||
                                             session is null)
                                        {
                                            return Task.FromResult(ocpiResponseBuilder!);
                                        }

                                        #endregion


                                        return Task.FromResult(
                                            new OCPIResponse.Builder(Request) {
                                                   StatusCode           = 1000,
                                                   StatusMessage        = "Hello world!",
                                                   Data                 = session.ToJSON(CustomSessionSerializer,
                                                                                         CustomCDRTokenSerializer,
                                                                                         CustomChargingPeriodSerializer,
                                                                                         CustomCDRDimensionSerializer,
                                                                                         CustomPriceSerializer),
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                                       AccessControlAllowHeaders  = [ "Authorization" ],
                                                       LastModified               = session.LastUpdated,
                                                       ETag                       = session.ETag
                                                   }
                                            });

                                    });

            #endregion

            #endregion


            // For CPOs, but also for EMSPs, as SCSPs might talk to EMSPs!
            #region ~/chargingprofiles/{session_id}

            // https://github.com/ocpi/ocpi/blob/release-2.2.1-bugfixes/mod_charging_profiles.asciidoc

            #region GET     ~/chargingprofiles/{session_id}?duration={duration}&response_url=https://client.com/12345/

            // 1. GET will just return a ChargingProfileResponse (result=ACCEPTED).
            // 2. The resposeURL will be called with a ActiveProfileResult object (result=ACCEPTED, ActiveChargingProfile).

            // NOTE: This GET requests introduces state and thus is a VIOLATION of HTTP semantics!

            #endregion

            #region PUT     ~/chargingprofiles/{session_id}?response_url=https://client.com/12345/

            // 1. PUT (with a resposeURL): SetChargingProfile object
            // 2. The resposeURL will be called later, e.g. POST https://client.com/12345/ with a ChargingProfileResult object.

            #endregion

            #region DELETE  ~/chargingprofiles/{session_id}?response_url=https://client.com/12345/

            // 1. DELETE will just return a ChargingProfileResponse (result=ACCEPTED).
            // 2. The resposeURL will be called with a ClearProfileResult object (result=ACCEPTED).

            #endregion

            #endregion


            #region ~/sessions/{session_id}/charging_preferences <= Yet to do!

            //ToDo: Implement ~/sessions/{session_id}/charging_preferences!

            #region PUT     ~/sessions/{session_id}/charging_preferences

            // https://example.com/ocpi/2.2/cpo/sessions/12454/charging_preferences

            #endregion

            #endregion


            #region ~/cdrs

            #region OPTIONS  ~/cdrs

            CommonAPI.AddOCPIMethod(

                HTTPHostname.Any,
                HTTPMethod.OPTIONS,
                URLPathPrefix + "CDRs",
                OCPIRequestHandler: request =>

                    Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               HTTPResponseBuilder = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode              = HTTPStatusCode.OK,
                                   Allow                       = [ HTTPMethod.OPTIONS, HTTPMethod.GET ],
                                   AccessControlAllowMethods   = [ "OPTIONS", "GET" ],
                                   AccessControlAllowHeaders   = [ "Authorization" ],
                                   AccessControlExposeHeaders  = [ "X-Request-ID", "X-Correlation-ID", "Link", "X-Total-Count", "X-Filtered-Count" ]
                               }
                        })

            );

            #endregion

            #region GET      ~/cdrs

            // https://example.com/ocpi/2.2/cpo/cdrs/?date_from=2019-01-28T12:00:00&date_to=2019-01-29T12:00:00&offset=50&limit=100
            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.GET,
                                    URLPathPrefix + "cdrs",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestHandler: Request => {

                                        #region Check access token

                                        if (Request.LocalAccessInfo?.IsNot(Roles.EMSP) == true ||
                                            Request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                                        {

                                            return Task.FromResult(
                                                new OCPIResponse.Builder(Request) {
                                                    StatusCode           = 2000,
                                                    StatusMessage        = "Invalid or blocked access token!",
                                                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                        AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                                        AccessControlAllowHeaders  = [ "Authorization" ]
                                                    }
                                                });

                                        }

                                        #endregion


                                        var filters            = Request.GetDateAndPaginationFilters();

                                        var allCDRs            = CommonAPI.//GetCDRs(cdr => Request.AccessInfo.Value.Roles.Any(role => role.CountryCode == cdr.CountryCode &&
                                                                           //                                                          role.PartyId     == cdr.PartyId)).
                                                                           GetCDRs(cdr => CommonAPI.OurCredentialRoles.Any(credentialRole => credentialRole.CountryCode == cdr.CountryCode &&
                                                                                                                                             credentialRole.PartyId     == cdr.PartyId)).
                                                                           ToArray();

                                        var filteredCDRs       = allCDRs.Where(CDR => !filters.From.HasValue || CDR.LastUpdated >  filters.From.Value).
                                                                         Where(CDR => !filters.To.  HasValue || CDR.LastUpdated <= filters.To.  Value).
                                                                         ToArray();


                                        var httpResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                                       Server                     = DefaultHTTPServerName,
                                                                       Date                       = Timestamp.Now,
                                                                       AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                                                   }.

                                                                   // The overall number of CDRs
                                                                   Set("X-Total-Count",  allCDRs.Length).

                                                                   // The maximum number of CDRs that the server WILL return within a single request
                                                                   Set("X-Limit",        allCDRs.Length);


                                        #region When the limit query parameter was set & this is not the last pagination page...

                                        if (filters.Limit.HasValue &&
                                            allCDRs.ULongLength() > ((filters.Offset ?? 0) + (filters.Limit ?? 0)))
                                        {

                                            // The new query parameters for the "next" page of pagination within the HTTP Link header
                                            var queryParameters    = new List<String?>() {
                                                                         filters.From. HasValue ? $"from={filters.From.Value}" :                             null,
                                                                         filters.To.   HasValue ? $"to={filters.To.Value}" :                                 null,
                                                                         filters.Limit.HasValue ? $"offset={(filters.Offset ?? 0) + (filters.Limit ?? 0)}" : null,
                                                                         filters.Limit.HasValue ? $"limit={filters.Limit ?? 0}" :                            null
                                                                     }.Where(queryParameter => queryParameter is not null).
                                                                       AggregateWith("&");

                                            if (queryParameters.Length > 0)
                                                queryParameters = "?" + queryParameters;

                                            // Link to the 'next' page should be provided when this is NOT the last page, e.g.:
                                            //   - Link: <https://www.server.com/ocpi/cpo/2.0/cdrs/?offset=150&limit=50>; rel="next"
                                            httpResponseBuilder.Set("Link", $"<{(ExternalDNSName.IsNotNullOrEmpty()
                                                                                     ? $"https://{ExternalDNSName}"
                                                                                     : $"http://127.0.0.1:{HTTPServer.IPPorts.First()}")}{URLPathPrefix}/cdrs{queryParameters}>; rel=\"next\"");

                                        }

                                        #endregion

                                        return Task.FromResult(
                                                   new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 1000,
                                                       StatusMessage        = "Hello world!",
                                                       HTTPResponseBuilder  = httpResponseBuilder,
                                                       Data                 = new JArray(
                                                                                  filteredCDRs.
                                                                                  OrderBy       (cdr => cdr.Created).
                                                                                  SkipTakeFilter(filters.Offset,
                                                                                                 filters.Limit).
                                                                                  Select        (cdr => cdr.ToJSON(CustomCDRSerializer,
                                                                                                                   CustomCDRTokenSerializer,
                                                                                                                   CustomCDRLocationSerializer,
                                                                                                                   CustomEVSEEnergyMeterSerializer,
                                                                                                                   CustomTransparencySoftwareSerializer,
                                                                                                                   CustomTariffSerializer,
                                                                                                                   CustomDisplayTextSerializer,
                                                                                                                   CustomPriceSerializer,
                                                                                                                   CustomTariffElementSerializer,
                                                                                                                   CustomPriceComponentSerializer,
                                                                                                                   CustomTariffRestrictionsSerializer,
                                                                                                                   CustomEnergyMixSerializer,
                                                                                                                   CustomEnergySourceSerializer,
                                                                                                                   CustomEnvironmentalImpactSerializer,
                                                                                                                   CustomChargingPeriodSerializer,
                                                                                                                   CustomCDRDimensionSerializer,
                                                                                                                   CustomSignedDataSerializer,
                                                                                                                   CustomSignedValueSerializer))
                                                                              )
                                                   }
                                               );

                                    });

            #endregion

            #endregion

            #region ~/cdrs/{CDRId}

            #region OPTIONS  ~/cdrs/{CDRId}

            CommonAPI.AddOCPIMethod(

                HTTPHostname.Any,
                HTTPMethod.OPTIONS,
                URLPathPrefix + "cdrs/{CDRId}",
                OCPIRequestHandler: request =>

                    Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               HTTPResponseBuilder = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   Allow                      = [ HTTPMethod.OPTIONS, HTTPMethod.GET ],
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ]
                               }
                        })

            );

            #endregion

            #region GET      ~/cdrs/{CDRId}     // The concrete URL is not specified by OCPI! m(

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.GET,
                                    URLPathPrefix + "cdrs/{CDRId}",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestHandler: Request => {

                                        #region Check access token

                                        if (Request.LocalAccessInfo?.IsNot(Roles.EMSP) == true ||
                                            Request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                                        {

                                            return Task.FromResult(
                                                new OCPIResponse.Builder(Request) {
                                                    StatusCode           = 2000,
                                                    StatusMessage        = "Invalid or blocked access token!",
                                                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                        AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                                        AccessControlAllowHeaders  = [ "Authorization" ]
                                                    }
                                                });

                                        }

                                        #endregion

                                        #region Check CDR

                                        if (!Request.ParseCDR(CommonAPI,
                                                              //Request.AccessInfo.Value.Roles.Select(role => new Tuple<CountryCode, Party_Id>(role.CountryCode, role.PartyId)),
                                                              CommonAPI.OurCredentialRoles.Select(credentialRole => new Tuple<CountryCode, Party_Id>(credentialRole.CountryCode, credentialRole.PartyId)),
                                                              out var cdrId,
                                                              out var cdr,
                                                              out var ocpiResponseBuilder) ||
                                             cdr is null)
                                        {
                                            return Task.FromResult(ocpiResponseBuilder!);
                                        }

                                        #endregion


                                        return Task.FromResult(
                                            new OCPIResponse.Builder(Request) {
                                                   StatusCode           = 1000,
                                                   StatusMessage        = "Hello world!",
                                                   Data                 = cdr.ToJSON(CustomCDRSerializer,
                                                                                     CustomCDRTokenSerializer,
                                                                                     CustomCDRLocationSerializer,
                                                                                     CustomEVSEEnergyMeterSerializer,
                                                                                     CustomTransparencySoftwareSerializer,
                                                                                     CustomTariffSerializer,
                                                                                     CustomDisplayTextSerializer,
                                                                                     CustomPriceSerializer,
                                                                                     CustomTariffElementSerializer,
                                                                                     CustomPriceComponentSerializer,
                                                                                     CustomTariffRestrictionsSerializer,
                                                                                     CustomEnergyMixSerializer,
                                                                                     CustomEnergySourceSerializer,
                                                                                     CustomEnvironmentalImpactSerializer,
                                                                                     CustomChargingPeriodSerializer,
                                                                                     CustomCDRDimensionSerializer,
                                                                                     CustomSignedDataSerializer,
                                                                                     CustomSignedValueSerializer),
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                                       AccessControlAllowHeaders  = [ "Authorization" ],
                                                       LastModified               = cdr.LastUpdated,
                                                       ETag                       = cdr.ETag
                                                   }
                                            });

                                    });

            #endregion

            #endregion


            #region ~/tokens/{country_code}/{party_id}       [NonStandard]

            #region OPTIONS  ~/tokens/{country_code}/{party_id}      [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPHostname.Any,
                HTTPMethod.OPTIONS,
                URLPathPrefix + "tokens/{country_code}/{party_id}",
                OCPIRequestHandler: request =>

                    Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               HTTPResponseBuilder = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode              = HTTPStatusCode.OK,
                                   Allow                       = [ HTTPMethod.OPTIONS, HTTPMethod.GET, HTTPMethod.DELETE ],
                                   AccessControlAllowMethods   = [ "OPTIONS", "GET", "DELETE" ],
                                   AccessControlAllowHeaders   = [ "Authorization" ],
                                   AccessControlExposeHeaders  = [ "X-Request-ID", "X-Correlation-ID", "Link", "X-Total-Count", "X-Filtered-Count" ]
                               }
                        })

            );

            #endregion

            #region GET      ~/tokens/{country_code}/{party_id}      [NonStandard]

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.GET,
                                    URLPathPrefix + "tokens/{country_code}/{party_id}",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestHandler: Request => {

                                        #region Check access token

                                        if (Request.LocalAccessInfo?.IsNot(Roles.EMSP) == true ||
                                            Request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                                        {

                                            return Task.FromResult(
                                                new OCPIResponse.Builder(Request) {
                                                    StatusCode           = 2000,
                                                    StatusMessage        = "Invalid or blocked access token!",
                                                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                        AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                                        AccessControlAllowHeaders  = [ "Authorization" ]
                                                    }
                                                });

                                        }

                                        #endregion

                                        #region Check country code and party identification

                                        if (!Request.ParseCountryCodeAndPartyId(CommonAPI,
                                                                                out var countryCode,
                                                                                out var partyId,
                                                                                out var ocpiResponseBuilder) ||
                                            !countryCode.HasValue ||
                                            !partyId.HasValue)
                                        {
                                            return Task.FromResult(ocpiResponseBuilder!);
                                        }

                                        #endregion


                                        var filters         = Request.GetDateAndPaginationFilters();

                                        var allTokens       = CommonAPI.GetTokens(countryCode,
                                                                                  partyId).
                                                                        ToArray();

                                        var filteredTokens  = allTokens.Select(tokenStatus => tokenStatus.Token).
                                                                        Where (token       => !filters.From.HasValue || token.LastUpdated >  filters.From.Value).
                                                                        Where (token       => !filters.To.  HasValue || token.LastUpdated <= filters.To.  Value).
                                                                        ToArray();


                                        var httpResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                                       Server                     = DefaultHTTPServerName,
                                                                       Date                       = Timestamp.Now,
                                                                       AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                                                   }.

                                                                   // The overall number of tokens
                                                                   Set("X-Total-Count",  allTokens.Length).

                                                                   // The maximum number of tokens that the server WILL return within a single request
                                                                   Set("X-Limit",        allTokens.Length);


                                        #region When the limit query parameter was set & this is not the last pagination page...

                                        if (filters.Limit.HasValue &&
                                            allTokens.ULongLength() > ((filters.Offset ?? 0) + (filters.Limit ?? 0)))
                                        {

                                            // The new query parameters for the "next" page of pagination within the HTTP Link header
                                            var queryParameters    = new List<String?>() {
                                                                         filters.From. HasValue ? $"from={filters.From.Value}" :                             null,
                                                                         filters.To.   HasValue ? $"to={filters.To.Value}" :                                 null,
                                                                         filters.Limit.HasValue ? $"offset={(filters.Offset ?? 0) + (filters.Limit ?? 0)}" : null,
                                                                         filters.Limit.HasValue ? $"limit={filters.Limit ?? 0}" :                            null
                                                                     }.Where(queryParameter => queryParameter is not null).
                                                                       AggregateWith("&");

                                            if (queryParameters.Length > 0)
                                                queryParameters = "?" + queryParameters;

                                            // Link to the 'next' page should be provided when this is NOT the last page, e.g.:
                                            //   - Link: <https://www.server.com/ocpi/cpo/2.0/cdrs/?offset=150&limit=50>; rel="next"
                                            httpResponseBuilder.Set("Link", $"<{(ExternalDNSName.IsNotNullOrEmpty()
                                                                                     ? $"https://{ExternalDNSName}"
                                                                                     : $"http://127.0.0.1:{HTTPServer.IPPorts.First()}")}{URLPathPrefix}/tokens/{countryCode}/{partyId}{queryParameters}>; rel=\"next\"");

                                        }

                                        #endregion

                                        return Task.FromResult(
                                                   new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 1000,
                                                       StatusMessage        = "Hello world!",
                                                       HTTPResponseBuilder  = httpResponseBuilder,
                                                       Data                 = new JArray(
                                                                                  filteredTokens.
                                                                                      OrderBy       (token => token.Created).
                                                                                      SkipTakeFilter(filters.Offset,
                                                                                                     filters.Limit).
                                                                                      Select        (token => token.ToJSON(CustomTokenSerializer,
                                                                                                                           CustomEnergyContractSerializer))
                                                                              )
                                                   }
                                               );

                                    });

            #endregion

            #region DELETE   ~/tokens/{country_code}/{party_id}      [NonStandard]

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.DELETE,
                                    URLPathPrefix + "tokens/{country_code}/{party_id}",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestHandler: async Request => {

                                        #region Check access token

                                        if (Request.LocalAccessInfo?.IsNot(Roles.EMSP) == true ||
                                            Request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = "Invalid or blocked access token!",
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                           AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                                       }
                                                   };

                                        }

                                        #endregion

                                        #region Check country code and party identification

                                        if (!Request.ParseCountryCodeAndPartyId(CommonAPI,
                                                                                out var CountryCode,
                                                                                out var PartyId,
                                                                                out var OCPIResponseBuilder))
                                        {
                                            return OCPIResponseBuilder;
                                        }

                                        #endregion


                                        CommonAPI.RemoveAllTokens(CountryCode.Value,
                                                                  PartyId.    Value);


                                        return new OCPIResponse.Builder(Request) {
                                                   StatusCode           = 1000,
                                                   StatusMessage        = "Hello world!",
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "DELETE" },
                                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                                   }
                                               };

                                    });

            #endregion

            #endregion

            #region ~/tokens/{country_code}/{party_id}/{tokenId}

            #region OPTIONS  ~/tokens/{country_code}/{party_id}/{tokenId}      [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPHostname.Any,
                HTTPMethod.OPTIONS,
                URLPathPrefix + "tokens/{country_code}/{party_id}/{tokenId}",
                OCPIRequestHandler: request =>

                    Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               HTTPResponseBuilder = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   Allow                      = [ HTTPMethod.OPTIONS, HTTPMethod.GET, HTTPMethod.PUT, HTTPMethod.PATCH, HTTPMethod.DELETE ],
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE"],
                                   AccessControlAllowHeaders  = [ "Authorization" ],
                                   AcceptPatch                = [ HTTPContentType.Application.JSONMergePatch_UTF8 ]
                               }
                        })

            );

            #endregion

            #region GET     ~/tokens/{country_code}/{party_id}/{tokenId}?type={type}

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.GET,
                                    URLPathPrefix + "tokens/{country_code}/{party_id}/{tokenId}",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestLogger:   GetTokenRequest,
                                    OCPIResponseLogger:  GetTokenResponse,
                                    OCPIRequestHandler:  Request => {

                                        #region Check access token

                                        if (Request.LocalAccessInfo is null ||
                                            Request.LocalAccessInfo?.IsNot(Roles.EMSP) == true ||
                                            Request.LocalAccessInfo.Status != AccessStatus.ALLOWED)
                                        {

                                            return Task.FromResult(
                                                new OCPIResponse.Builder(Request) {
                                                    StatusCode           = 2000,
                                                    StatusMessage        = "Invalid or blocked access token!",
                                                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                        AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                                        AccessControlAllowHeaders  = [ "Authorization" ]
                                                    }
                                                });

                                        }

                                        #endregion

                                        #region Check token

                                        if (!Request.ParseToken(CommonAPI,
                                                                Request.LocalAccessInfo.Roles.Select(role => new Tuple<CountryCode, Party_Id>(role.CountryCode, role.PartyId)),
                                                                out var countryCode,
                                                                out var partyId,
                                                                out var tokenId,
                                                                out var tokenStatus,
                                                                out var ocpiResponseBuilder) ||
                                             tokenStatus.Token is null)
                                        {
                                            return Task.FromResult(ocpiResponseBuilder!);
                                        }

                                        #endregion


                                        //ToDo: What exactly to do with this information?
                                        var tokenType = Request.QueryString.TryParseEnum<TokenType>("type") ?? TokenType.RFID;


                                        return Task.FromResult(
                                            new OCPIResponse.Builder(Request) {
                                                   StatusCode           = 1000,
                                                   StatusMessage        = "Hello world!",
                                                   Data                 = tokenStatus.Token.ToJSON(CustomTokenSerializer,
                                                                                                   CustomEnergyContractSerializer),
                                                HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "PUT", "PATCH", "DELETE" },
                                                       AccessControlAllowHeaders  = [ "Authorization" ],
                                                       LastModified               = tokenStatus.Token.LastUpdated,
                                                       ETag                       = tokenStatus.Token.ETag
                                                   }
                                            });

                                    });

            #endregion

            #region PUT     ~/tokens/{country_code}/{party_id}/{tokenId}?type={type}

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.PUT,
                                    URLPathPrefix + "tokens/{country_code}/{party_id}/{tokenId}",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestLogger:   PutTokenRequest,
                                    OCPIResponseLogger:  PutTokenResponse,
                                    OCPIRequestHandler:  async Request => {

                                        #region Check access token

                                        if (Request.LocalAccessInfo is null ||
                                            Request.LocalAccessInfo?.IsNot(Roles.EMSP) == true ||
                                            Request.LocalAccessInfo.Status != AccessStatus.ALLOWED)
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = "Invalid or blocked access token!",
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                           AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                                       }
                                                   };

                                        }

                                        #endregion

                                        #region Check token

                                        if (!Request.ParseTokenId(CommonAPI,
                                                                  out var countryCode,
                                                                  out var partyId,
                                                                  out var tokenId,
                                                                  out var ocpiResponseBuilder))
                                        {
                                            return ocpiResponseBuilder!;
                                        }

                                        #endregion

                                        #region Parse new or updated token JSON

                                        if (!Request.TryParseJObjectRequestBody(out var tokenJSON, out ocpiResponseBuilder))
                                            return ocpiResponseBuilder;

                                        if (!Token.TryParse(tokenJSON,
                                                            out var newOrUpdatedToken,
                                                            out var errorResponse,
                                                            countryCode,
                                                            partyId,
                                                            tokenId) ||
                                             newOrUpdatedToken is null)
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                   StatusCode           = 2001,
                                                   StatusMessage        = "Could not parse the given token JSON: " + errorResponse,
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                       AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "PUT", "PATCH", "DELETE" },
                                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                                   }
                                               };

                                        }

                                        #endregion


                                        //ToDo: What exactly to do with this information?
                                        var TokenType          = Request.QueryString.TryParseEnum<TokenType>("type") ?? OCPIv2_2_1.TokenType.RFID;

                                        var addOrUpdateResult  = await CommonAPI.AddOrUpdateToken(newOrUpdatedToken,
                                                                                                  AllowDowngrades: AllowDowngrades ?? Request.QueryString.GetBoolean("forceDowngrade"));


                                        if (addOrUpdateResult.IsSuccess &&
                                            addOrUpdateResult.Data is not null)
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 1000,
                                                       StatusMessage        = "Hello world!",
                                                       Data                 = addOrUpdateResult.Data.ToJSON(CustomTokenSerializer,
                                                                                                            CustomEnergyContractSerializer),
                                                HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = addOrUpdateResult.WasCreated == true
                                                                                            ? HTTPStatusCode.Created
                                                                                            : HTTPStatusCode.OK,
                                                           AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "PUT", "PATCH", "DELETE" },
                                                           AccessControlAllowHeaders  = [ "Authorization" ],
                                                           LastModified               = addOrUpdateResult.Data.LastUpdated,
                                                           ETag                       = addOrUpdateResult.Data.ETag
                                                       }
                                                   };

                                        }

                                        return new OCPIResponse.Builder(Request) {
                                                   StatusCode           = 2000,
                                                   StatusMessage        = addOrUpdateResult.ErrorResponse,
                                                   Data                 = newOrUpdatedToken.ToJSON(CustomTokenSerializer,
                                                                                                   CustomEnergyContractSerializer),
                                            HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                       AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "PUT", "PATCH", "DELETE" },
                                                       AccessControlAllowHeaders  = [ "Authorization" ],
                                                       LastModified               = addOrUpdateResult.Data.LastUpdated,
                                                       ETag                       = addOrUpdateResult.Data.ETag
                                                   }
                                               };

                                    });

            #endregion

            #region PATCH   ~/tokens/{country_code}/{party_id}/{tokenId}?type={type}

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.PATCH,
                                    URLPathPrefix + "tokens/{country_code}/{party_id}/{tokenId}",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestLogger:   PatchTokenRequest,
                                    OCPIResponseLogger:  PatchTokenResponse,
                                    OCPIRequestHandler:   async Request => {

                                        #region Check access token

                                        if (Request.LocalAccessInfo is null ||
                                            Request.LocalAccessInfo?.IsNot(Roles.EMSP) == true ||
                                            Request.LocalAccessInfo.Status != AccessStatus.ALLOWED)
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = "Invalid or blocked access token!",
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                           AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                                       }
                                                   };

                                        }

                                        #endregion

                                        #region Check token

                                        if (!Request.ParseToken(CommonAPI,
                                                                Request.LocalAccessInfo.Roles.Select(role => new Tuple<CountryCode, Party_Id>(role.CountryCode, role.PartyId)),
                                                                out var countryCode,
                                                                out var partyId,
                                                                out var tokenId,
                                                                out var existingTokenStatus,
                                                                out var ocpiResponseBuilder) ||
                                             existingTokenStatus.Token is null)
                                        {
                                            return ocpiResponseBuilder!;
                                        }

                                        #endregion

                                        #region Parse token JSON patch

                                        if (!Request.TryParseJObjectRequestBody(out var tokenPatch, out ocpiResponseBuilder))
                                            return ocpiResponseBuilder;

                                        #endregion


                                        //ToDo: What exactly to do with this information?
                                        var tokenType     = Request.QueryString.TryParseEnum<TokenType>("type") ?? TokenType.RFID;


                                        //ToDo: Validation-Checks for PATCHes (E-Tag, Timestamp, ...)
                                        var patchedToken  = await CommonAPI.TryPatchToken(existingTokenStatus.Token,
                                                                                          tokenPatch,
                                                                                          AllowDowngrades ?? Request.QueryString.GetBoolean("forceDowngrade"));


                                        if (patchedToken.IsSuccess)
                                            return new OCPIResponse.Builder(Request) {
                                                           StatusCode           = 1000,
                                                           StatusMessage        = "Hello world!",
                                                           Data                 = patchedToken.PatchedData.ToJSON(CustomTokenSerializer,
                                                                                                                  CustomEnergyContractSerializer),
                                                HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                               HTTPStatusCode             = HTTPStatusCode.OK,
                                                               AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "PUT", "PATCH", "DELETE" },
                                                               AccessControlAllowHeaders  = [ "Authorization" ],
                                                               LastModified               = patchedToken.PatchedData.LastUpdated,
                                                               ETag                       = patchedToken.PatchedData.ETag
                                                           }
                                                       };

                                        return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = patchedToken.ErrorResponse,
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.OK,
                                                           AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "PUT", "PATCH", "DELETE" },
                                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                                       }
                                                   };

                                    });

            #endregion

            #region DELETE  ~/tokens/{country_code}/{party_id}/{tokenId}      [NonStandard]

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.DELETE,
                                    URLPathPrefix + "tokens/{country_code}/{party_id}/{tokenId}",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestLogger:   DeleteTokenRequest,
                                    OCPIResponseLogger:  DeleteTokenResponse,
                                    OCPIRequestHandler:   async Request => {

                                        #region Check access token

                                        if (Request.LocalAccessInfo is null ||
                                            Request.LocalAccessInfo?.IsNot(Roles.EMSP) == true ||
                                            Request.LocalAccessInfo.Status != AccessStatus.ALLOWED)
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = "Invalid or blocked access token!",
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                           AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                                       }
                                                   };

                                        }

                                        #endregion

                                        #region Check token (status)

                                        if (!Request.ParseToken(CommonAPI,
                                                                Request.LocalAccessInfo.Roles.Select(role => new Tuple<CountryCode, Party_Id>(role.CountryCode, role.PartyId)),
                                                                out var countryCode,
                                                                out var partyId,
                                                                out var tokenId,
                                                                out var existingTokenStatus,
                                                                out var ocpiResponseBuilder) ||
                                             existingTokenStatus.Token is null)
                                        {
                                            return ocpiResponseBuilder!;
                                        }

                                        #endregion


                                        //ToDo: What exactly to do with this information?
                                        var tokenType = Request.QueryString.TryParseEnum<TokenType>("type") ?? TokenType.RFID;


                                        if (CommonAPI.RemoveToken(existingTokenStatus.Token))
                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 1000,
                                                       StatusMessage        = "Hello world!",
                                                       Data                 = existingTokenStatus.Token.ToJSON(CustomTokenSerializer,
                                                                                                               CustomEnergyContractSerializer),
                                                HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.OK,
                                                           AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "PUT", "PATCH", "DELETE" },
                                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                                           //LastModified               = Timestamp.Now.ToIso8601()
                                                       }
                                                   };

                                        else
                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = "Hello world!",
                                                       Data                 = existingTokenStatus.Token.ToJSON(CustomTokenSerializer,
                                                                                                               CustomEnergyContractSerializer),
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.OK,
                                                           AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "PUT", "PATCH", "DELETE" },
                                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                                           //LastModified               = Timestamp.Now.ToIso8601()
                                                       }
                                                   };

                                    });

            #endregion

            #endregion



            // Commands

            #region ~/commands/RESERVE_NOW

            #region OPTIONS  ~/commands/RESERVE_NOW

            CommonAPI.AddOCPIMethod(

                HTTPHostname.Any,
                HTTPMethod.OPTIONS,
                URLPathPrefix + "commands/RESERVE_NOW",
                HTTPContentType.Application.JSON_UTF8,
                OCPIRequestHandler: request =>

                    Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               HTTPResponseBuilder = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   Allow                      = [ HTTPMethod.OPTIONS, HTTPMethod.POST],
                                   AccessControlAllowMethods  = [ "OPTIONS", "POST"],
                                   AccessControlAllowHeaders  = [ "Authorization" ]
                               }
                        })

            );

            #endregion

            #region POST     ~/commands/RESERVE_NOW

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.POST,
                                    URLPathPrefix + "commands/RESERVE_NOW",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestLogger:   ReserveNowRequest,
                                    OCPIResponseLogger:  ReserveNowResponse,
                                    OCPIRequestHandler:  async Request => {

                                        #region Check access token

                                        if (Request.RemoteParty is null ||
                                            Request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = "Invalid or blocked access token!",
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                           AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                                       }
                                                   };

                                        }

                                        #endregion

                                        #region Parse ReserveNow command JSON

                                        if (!Request.TryParseJObjectRequestBody(out var reserveNowJSON, out var ocpiResponse))
                                            return ocpiResponse;

                                        if (!ReserveNowCommand.TryParse(reserveNowJSON,
                                                                        out var reserveNowCommand,
                                                                        out var errorResponse) ||
                                             reserveNowCommand is null)
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2001,
                                                       StatusMessage        = "Could not parse the given 'RESERVE_NOW' command JSON: " + errorResponse,
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                           AccessControlAllowMethods  = new[] { "OPTIONS", "POST" },
                                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                                       }
                                                   };

                                        }

                                        #endregion


                                        CommandResponse? commandResponse = null;

                                        if (OnReserveNowCommand is not null)
                                            commandResponse = await OnReserveNowCommand.Invoke(EMSP_Id.From(Request.RemoteParty.Id),
                                                                                               reserveNowCommand);

                                        commandResponse ??= new CommandResponse(
                                                                reserveNowCommand,
                                                                CommandResponseTypes.NOT_SUPPORTED,
                                                                Timeout: TimeSpan.FromSeconds(15),
                                                                Message: new[] {
                                                                             new DisplayText(Languages.en, "Not supported!")
                                                                         }
                                                            );


                                        return new OCPIResponse.Builder(Request) {
                                                   StatusCode           = 1000,
                                                   StatusMessage        = "Hello world!",
                                                   Data                 = commandResponse.ToJSON(),
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = new[] { "OPTIONS", "POST" },
                                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                                   }
                                               };

                                    });

            #endregion

            #endregion

            #region ~/commands/CANCEL_RESERVATION

            #region OPTIONS  ~/commands/CANCEL_RESERVATION

            CommonAPI.AddOCPIMethod(

                HTTPHostname.Any,
                HTTPMethod.OPTIONS,
                URLPathPrefix + "commands/CANCEL_RESERVATION",
                HTTPContentType.Application.JSON_UTF8,
                OCPIRequestHandler: request =>

                    Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               HTTPResponseBuilder = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   Allow                      = [ HTTPMethod.OPTIONS, HTTPMethod.POST],
                                   AccessControlAllowMethods  = [ "OPTIONS", "POST"],
                                   AccessControlAllowHeaders  = [ "Authorization" ]
                               }
                        })

            );

            #endregion

            #region POST     ~/commands/CANCEL_RESERVATION

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.POST,
                                    URLPathPrefix + "commands/CANCEL_RESERVATION",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestLogger:   CancelReservationRequest,
                                    OCPIResponseLogger:  CancelReservationResponse,
                                    OCPIRequestHandler:  async Request => {

                                        #region Check access token

                                        if (Request.RemoteParty is null ||
                                            Request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = "Invalid or blocked access token!",
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                           AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                                       }
                                                   };

                                        }

                                        #endregion

                                        #region Parse CancelReservation command JSON

                                        if (!Request.TryParseJObjectRequestBody(out var cancelReservationJSON, out var ocpiResponse))
                                            return ocpiResponse;

                                        if (!CancelReservationCommand.TryParse(cancelReservationJSON,
                                                                               out var cancelReservationCommand,
                                                                               out var errorResponse) ||
                                             cancelReservationCommand is null)
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2001,
                                                       StatusMessage        = "Could not parse the given 'CANCEL_RESERVATION' command JSON: " + errorResponse,
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                           AccessControlAllowMethods  = new[] { "OPTIONS", "POST" },
                                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                                       }
                                                   };

                                        }

                                        #endregion


                                        CommandResponse? commandResponse = null;

                                        if (OnCancelReservationCommand is not null)
                                            commandResponse = await OnCancelReservationCommand.Invoke(EMSP_Id.From(Request.RemoteParty.Id),
                                                                                                      cancelReservationCommand);

                                        commandResponse ??= new CommandResponse(
                                                                cancelReservationCommand,
                                                                CommandResponseTypes.NOT_SUPPORTED,
                                                                Timeout: TimeSpan.FromSeconds(15),
                                                                Message: new[] {
                                                                             new DisplayText(Languages.en, "Not supported!")
                                                                         }
                                                            );


                                        return new OCPIResponse.Builder(Request) {
                                                   StatusCode           = 1000,
                                                   StatusMessage        = "Hello world!",
                                                   Data                 = commandResponse.ToJSON(),
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = new[] { "OPTIONS", "POST" },
                                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                                   }
                                               };

                                    });

            #endregion

            #endregion

            #region ~/commands/START_SESSION

            #region OPTIONS  ~/commands/START_SESSION

            CommonAPI.AddOCPIMethod(

                HTTPHostname.Any,
                HTTPMethod.OPTIONS,
                URLPathPrefix + "commands/START_SESSION",
                HTTPContentType.Application.JSON_UTF8,
                OCPIRequestHandler: request =>

                    Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               HTTPResponseBuilder = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   Allow                      = [ HTTPMethod.OPTIONS, HTTPMethod.POST],
                                   AccessControlAllowMethods  = [ "OPTIONS", "POST"],
                                   AccessControlAllowHeaders  = [ "Authorization" ]
                               }
                        })

            );

            #endregion

            #region POST     ~/commands/START_SESSION

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.POST,
                                    URLPathPrefix + "commands/START_SESSION",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestLogger:   StartSessionRequest,
                                    OCPIResponseLogger:  StartSessionResponse,
                                    OCPIRequestHandler:  async Request => {

                                        #region Check access token

                                        if (Request.RemoteParty is null ||
                                            Request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = "Invalid or blocked access token!",
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                           AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                                       }
                                                   };

                                        }

                                        #endregion

                                        #region Parse StartSession command JSON

                                        if (!Request.TryParseJObjectRequestBody(out var startSessionJSON, out var ocpiResponse))
                                            return ocpiResponse;

                                        if (!StartSessionCommand.TryParse(startSessionJSON,
                                                                          out var startSessionCommand,
                                                                          out var errorResponse) ||
                                             startSessionCommand is null)
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2001,
                                                       StatusMessage        = "Could not parse the given 'START_SESSION' command JSON: " + errorResponse,
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                           AccessControlAllowMethods  = new[] { "OPTIONS", "POST" },
                                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                                       }
                                                   };

                                        }

                                        #endregion


                                        CommandResponse? commandResponse = null;

                                        if (OnStartSessionCommand is not null)
                                            commandResponse = await OnStartSessionCommand.Invoke(EMSP_Id.From(Request.RemoteParty.Id),
                                                                                                 startSessionCommand);

                                        commandResponse ??= new CommandResponse(
                                                                startSessionCommand,
                                                                CommandResponseTypes.NOT_SUPPORTED,
                                                                Timeout: TimeSpan.FromSeconds(15),
                                                                Message: new[] {
                                                                             new DisplayText(Languages.en, "Not supported!")
                                                                         }
                                                            );


                                        return new OCPIResponse.Builder(Request) {
                                                   StatusCode           = 1000,
                                                   StatusMessage        = "Hello world!",
                                                   Data                 = commandResponse.ToJSON(),
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = new[] { "OPTIONS", "POST" },
                                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                                   }
                                               };

                                    });

            #endregion

            #endregion

            #region ~/commands/STOP_SESSION

            #region OPTIONS  ~/commands/STOP_SESSION

            CommonAPI.AddOCPIMethod(

                HTTPHostname.Any,
                HTTPMethod.OPTIONS,
                URLPathPrefix + "commands/STOP_SESSION",
                HTTPContentType.Application.JSON_UTF8,
                OCPIRequestHandler: request =>

                    Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               HTTPResponseBuilder = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   Allow                      = [ HTTPMethod.OPTIONS, HTTPMethod.POST],
                                   AccessControlAllowMethods  = [ "OPTIONS", "POST"],
                                   AccessControlAllowHeaders  = [ "Authorization" ]
                               }
                        })

            );

            #endregion

            #region POST     ~/commands/STOP_SESSION

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.POST,
                                    URLPathPrefix + "commands/STOP_SESSION",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestLogger:   StopSessionRequest,
                                    OCPIResponseLogger:  StopSessionResponse,
                                    OCPIRequestHandler:  async Request => {

                                        #region Check access token

                                        if (Request.RemoteParty is null ||
                                            Request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = "Invalid or blocked access token!",
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                           AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                                       }
                                                   };

                                        }

                                        #endregion

                                        #region Parse StopSession command JSON

                                        if (!Request.TryParseJObjectRequestBody(out var stopSessionJSON, out var ocpiResponse))
                                            return ocpiResponse;

                                        if (!StopSessionCommand.TryParse(stopSessionJSON,
                                                                          out var stopSessionCommand,
                                                                          out var errorResponse) ||
                                             stopSessionCommand is null)
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2001,
                                                       StatusMessage        = "Could not parse the given 'STOP_SESSION' command JSON: " + errorResponse,
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                           AccessControlAllowMethods  = new[] { "OPTIONS", "POST" },
                                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                                       }
                                                   };

                                        }

                                        #endregion


                                        CommandResponse? commandResponse = null;

                                        if (OnStopSessionCommand is not null)
                                            commandResponse = await OnStopSessionCommand.Invoke(EMSP_Id.From(Request.RemoteParty.Id),
                                                                                                stopSessionCommand);

                                        commandResponse ??= new CommandResponse(
                                                                stopSessionCommand,
                                                                CommandResponseTypes.NOT_SUPPORTED,
                                                                Timeout: TimeSpan.FromSeconds(15),
                                                                Message: new[] {
                                                                             new DisplayText(Languages.en, "Not supported!")
                                                                         }
                                                            );


                                        return new OCPIResponse.Builder(Request) {
                                                   StatusCode           = 1000,
                                                   StatusMessage        = "Hello world!",
                                                   Data                 = commandResponse.ToJSON(),
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = new[] { "OPTIONS", "POST" },
                                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                                   }
                                               };

                                    });

            #endregion

            #endregion

            #region ~/commands/UNLOCK_CONNECTOR

            #region OPTIONS  ~/commands/UNLOCK_CONNECTOR

            CommonAPI.AddOCPIMethod(

                HTTPHostname.Any,
                HTTPMethod.OPTIONS,
                URLPathPrefix + "commands/UNLOCK_CONNECTOR",
                HTTPContentType.Application.JSON_UTF8,
                OCPIRequestHandler: request =>

                    Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               HTTPResponseBuilder = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   Allow                      = [ HTTPMethod.OPTIONS, HTTPMethod.POST],
                                   AccessControlAllowMethods  = [ "OPTIONS", "POST"],
                                   AccessControlAllowHeaders  = [ "Authorization" ]
                               }
                        })

            );

            #endregion

            #region POST     ~/commands/UNLOCK_CONNECTOR

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.POST,
                                    URLPathPrefix + "commands/UNLOCK_CONNECTOR",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestLogger:   UnlockConnectorRequest,
                                    OCPIResponseLogger:  UnlockConnectorResponse,
                                    OCPIRequestHandler:  async Request => {

                                        #region Check access token

                                        if (Request.RemoteParty is null ||
                                            Request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = "Invalid or blocked access token!",
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                           AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                                       }
                                                   };

                                        }

                                        #endregion

                                        #region Parse UnlockConnector command JSON

                                        if (!Request.TryParseJObjectRequestBody(out var unlockConnectorJSON, out var ocpiResponse))
                                            return ocpiResponse;

                                        if (!UnlockConnectorCommand.TryParse(unlockConnectorJSON,
                                                                             out var unlockConnectorCommand,
                                                                             out var errorResponse) ||
                                             unlockConnectorCommand is null)
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2001,
                                                       StatusMessage        = "Could not parse the given 'UNLOCK_CONNECTOR' command JSON: " + errorResponse,
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                           AccessControlAllowMethods  = new[] { "OPTIONS", "POST" },
                                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                                       }
                                                   };

                                        }

                                        #endregion


                                        CommandResponse? commandResponse = null;

                                        if (OnUnlockConnectorCommand is not null)
                                            commandResponse = await OnUnlockConnectorCommand.Invoke(EMSP_Id.From(Request.RemoteParty.Id),
                                                                                                    unlockConnectorCommand);

                                        commandResponse ??= new CommandResponse(
                                                                unlockConnectorCommand,
                                                                CommandResponseTypes.NOT_SUPPORTED,
                                                                Timeout: TimeSpan.FromSeconds(15),
                                                                Message: new[] {
                                                                             new DisplayText(Languages.en, "Not supported!")
                                                                         }
                                                            );


                                        return new OCPIResponse.Builder(Request) {
                                                   StatusCode           = 1000,
                                                   StatusMessage        = "Hello world!",
                                                   Data                 = commandResponse.ToJSON(),
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = new[] { "OPTIONS", "POST" },
                                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                                   }
                                               };

                                    });

            #endregion

            #endregion


        }

        #endregion


    }

}
