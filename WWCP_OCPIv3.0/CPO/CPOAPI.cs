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
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv3_0.HTTP
{

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
        public CustomJObjectSerializerDelegate<Address>?                     CustomAddressSerializer                       { get; set; }
        public CustomJObjectSerializerDelegate<AdditionalGeoLocation>?       CustomAdditionalGeoLocationSerializer         { get; set; }
        public CustomJObjectSerializerDelegate<ChargingStation>?             CustomChargingStationSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<EVSE>?                        CustomEVSESerializer                          { get; set; }
        public CustomJObjectSerializerDelegate<StatusSchedule>?              CustomStatusScheduleSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<Connector>?                   CustomConnectorSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<Parking>?                     CustomParkingSerializer                       { get; set; }
        public CustomJObjectSerializerDelegate<ParkingRestriction>?          CustomParkingRestrictionSerializer            { get; set; }
        public CustomJObjectSerializerDelegate<EnergyMeter>?                 CustomEnergyMeterSerializer                   { get; set; }
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
        /// <param name="API">The CPO API.</param>
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
        /// <param name="API">The CPO API.</param>
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
        /// <param name="API">The CPO API.</param>
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
        /// <param name="API">The CPO API.</param>
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
        /// <param name="API">The CPO API.</param>
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
        /// <param name="API">The CPO API.</param>
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
        /// <param name="API">The CPO API.</param>
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
        /// <param name="API">The CPO API.</param>
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
        /// <param name="API">The CPO API.</param>
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
        /// <param name="API">The CPO API.</param>
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
        /// <param name="API">The CPO API.</param>
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
        /// <param name="API">The CPO API.</param>
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
        /// <param name="API">The CPO API.</param>
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
        /// <param name="API">The CPO API.</param>
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
        /// <param name="API">The CPO API.</param>
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
        /// <param name="API">The CPO API.</param>
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
        /// <param name="API">The CPO API.</param>
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
        /// <param name="API">The CPO API.</param>
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
        /// <param name="API">The CPO API.</param>
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
        /// <param name="API">The CPO API.</param>
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
        /// <param name="API">The CPO API.</param>
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
        /// <param name="API">The CPO API.</param>
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
        /// <param name="API">The CPO API.</param>
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
        /// <param name="API">The CPO API.</param>
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
        /// <param name="API">The CPO API.</param>
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
        /// <param name="API">The CPO API.</param>
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
        /// <param name="API">The CPO API.</param>
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
        /// <param name="API">The CPO API.</param>
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
        /// <param name="API">The CPO API.</param>
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
        /// <param name="API">The CPO API.</param>
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
        /// <param name="API">The CPO API.</param>
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
        /// <param name="API">The CPO API.</param>
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
        /// <param name="API">The CPO API.</param>
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
        /// <param name="API">The CPO API.</param>
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
        /// <param name="API">The CPO API.</param>
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
        /// <param name="API">The CPO API.</param>
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
        /// <param name="API">The CPO API.</param>
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
        /// <param name="API">The CPO API.</param>
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
        /// <param name="API">The CPO API.</param>
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
        /// <param name="API">The CPO API.</param>
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
        /// <param name="API">The CPO API.</param>
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
        /// <param name="API">The CPO API.</param>
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
        /// <param name="API">The CPO API.</param>
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
        /// <param name="API">The CPO API.</param>
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

            this.CommonAPI        = CommonAPI;
            this.AllowDowngrades  = AllowDowngrades;

            this.CPOAPILogger     = this.DisableLogging == false
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
            //                                   URLPathPrefix + "/cpo", "cloud.charging.open.protocols.OCPIv3_0.HTTPAPI.CPOAPI.HTTPRoot",
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
            //                                 typeof(CPOAPI).Assembly.GetManifestResourceStream("cloud.charging.open.protocols.OCPIv3_0.HTTPAPI.CPOAPI.HTTPRoot._header.html").SeekAndCopyTo(_MemoryStream, 3);
            //                                 typeof(CPOAPI).Assembly.GetManifestResourceStream("cloud.charging.open.protocols.OCPIv3_0.HTTPAPI.CPOAPI.HTTPRoot._footer.html").SeekAndCopyTo(_MemoryStream, 3);

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

                                        if ((Request.LocalAccessInfo is not null || CommonAPI.LocationsAsOpenData == false) &&
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
                                                                                              //location.Address.           Contains(pattern)         ||
                                                                                              //location.City.              Contains(pattern)         ||
                                                                                              //location.PostalCode.        Contains(pattern)         ||
                                                                                              //location.Country.ToString().Contains(pattern)         ||
                                                                                              location.Directions.        Matches (pattern)         ||
                                                                                              location.Operator?.   Name. Contains(pattern) == true ||
                                                                                              location.SubOperator?.Name. Contains(pattern) == true ||
                                                                                              location.Owner?.      Name. Contains(pattern) == true 
                                                                                              //location.Facilities.        Matches (pattern)         ||
                                                                                              //location.EVSEUIds.          Matches (pattern)         ||
                                                                                              //location.EVSEIds.           Matches (pattern)         
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
                                                                                      Select        (location => location.ToJSON(true,
                                                                                                                                 true,
                                                                                                                                 true,
                                                                                                                                 true,
                                                                                                                                 CustomLocationSerializer,
                                                                                                                                 CustomPublishTokenSerializer,
                                                                                                                                 CustomAddressSerializer,
                                                                                                                                 CustomAdditionalGeoLocationSerializer,
                                                                                                                                 CustomChargingStationSerializer,
                                                                                                                                 CustomEVSESerializer,
                                                                                                                                 CustomParkingSerializer,
                                                                                                                                 CustomParkingRestrictionSerializer,
                                                                                                                                 CustomStatusScheduleSerializer,
                                                                                                                                 CustomConnectorSerializer,
                                                                                                                                 CustomEnergyMeterSerializer,
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
                                                                   out var partyId,//CommonAPI.OurCredentialRoles.Select(credentialRole => new Tuple<CountryCode, Party_Id>(credentialRole.CountryCode, credentialRole.PartyId)),
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
                                                   Data                 = location.ToJSON(true,
                                                                                          true,
                                                                                          true,
                                                                                          true,
                                                                                          CustomLocationSerializer,
                                                                                          CustomPublishTokenSerializer,
                                                                                          CustomAddressSerializer,
                                                                                          CustomAdditionalGeoLocationSerializer,
                                                                                          CustomChargingStationSerializer,
                                                                                          CustomEVSESerializer,
                                                                                          CustomParkingSerializer,
                                                                                          CustomParkingRestrictionSerializer,
                                                                                          CustomStatusScheduleSerializer,
                                                                                          CustomConnectorSerializer,
                                                                                          CustomEnergyMeterSerializer,
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

                                        if (!Request.ParseLocationChargingStationEVSE(CommonAPI,
                                                                                      //Request.AccessInfo.Value.Roles.Select(role => new Tuple<CountryCode, Party_Id>(role.CountryCode, role.PartyId)),
                                                                                      out var partyId, //CommonAPI.OurCredentialRoles.Select(credentialRole => new Tuple<CountryCode, Party_Id>(credentialRole.CountryCode, credentialRole.PartyId)),
                                                                                      out var locationId,
                                                                                      out var location,
                                                                                      out var chargingStationId,
                                                                                      out var chargingStation,
                                                                                      out var evseId,
                                                                                      out var evse,
                                                                                      out var ocpiResponseBuilder,
                                                                                      FailOnMissingEVSE: true))
                                        {
                                            return Task.FromResult(ocpiResponseBuilder!);
                                        }

                                        #endregion


                                        return Task.FromResult(
                                            new OCPIResponse.Builder(Request) {
                                                   StatusCode           = 1000,
                                                   StatusMessage        = "Hello world!",
                                                   Data                 = evse.ToJSON(CustomEVSESerializer,
                                                                                      CustomConnectorSerializer,
                                                                                      CustomParkingSerializer,
                                                                                      CustomParkingRestrictionSerializer,
                                                                                      CustomImageSerializer,
                                                                                      CustomStatusScheduleSerializer,
                                                                                      CustomEnergyMeterSerializer,
                                                                                      CustomTransparencySoftwareStatusSerializer,
                                                                                      CustomTransparencySoftwareSerializer,
                                                                                      CustomDisplayTextSerializer),
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

                                        if (!Request.ParseLocationChargingStationEVSEConnector(CommonAPI,
                                                                                               //Request.AccessInfo.Value.Roles.Select(role => new Tuple<CountryCode, Party_Id>(role.CountryCode, role.PartyId)),
                                                                                               out var partyId, //CommonAPI.OurCredentialRoles.Select(credentialRole => new Tuple<CountryCode, Party_Id>(credentialRole.CountryCode, credentialRole.PartyId)),
                                                                                               out var locationId,
                                                                                               out var location,
                                                                                               out var chargingStationId,
                                                                                               out var chargingStation,
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
                                                   Data                 = connector.ToJSON(CustomConnectorSerializer),
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
                                                                         GetTariffs(tariff => CommonAPI.OurCredentialRoles.Any(credentialRole => //credentialRole.CountryCode == tariff.CountryCode &&
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
                                                                 out var partyId, //CommonAPI.OurCredentialRoles.Select(credentialRole => new Tuple<CountryCode, Party_Id>(credentialRole.CountryCode, credentialRole.PartyId)),
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



        }

        #endregion


    }

}
