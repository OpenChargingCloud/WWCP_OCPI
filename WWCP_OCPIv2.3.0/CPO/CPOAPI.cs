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

using System.Collections.Concurrent;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;
using cloud.charging.open.protocols.OCPIv2_3_0.CPO.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.HTTPTest;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_3_0
{

    /// <summary>
    /// The HTTP API for charge point operators.
    /// EMSPs will connect to this API.
    /// </summary>
    public class CPOAPI : AHTTPExtAPIXExtension2<CommonAPI, HTTPExtAPIX>
    {

        #region Data

        /// <summary>
        /// The default HTTP server name.
        /// </summary>
        public new const           String    DefaultHTTPServiceName   = $"GraphDefined OCPI {Version.String} CPO HTTP API";

        /// <summary>
        /// The default HTTP URL path prefix.
        /// </summary>
        public     static readonly HTTPPath  DefaultURLPathPrefix     = HTTPPath.Parse($"{Version.String}/cpo/");

        /// <summary>
        /// The default CPO API logfile name.
        /// </summary>
        public     const           String    DefaultLogfileName       = $"OCPI{Version.String}_CPOAPI.log";

        #endregion

        #region Properties

        /// <summary>
        /// The OCPI CommonAPI.
        /// </summary>
        public CommonAPI      CommonAPI
            => HTTPBaseAPI;

        /// <summary>
        /// (Dis-)allow PUTting of object having an earlier 'LastUpdated'-timestamp then already existing objects.
        /// OCPI v2.2 does not define any behaviour for this.
        /// </summary>
        public Boolean?       AllowDowngrades    { get; }

        /// <summary>
        /// The CPO API logger.
        /// </summary>
        public CPOAPILogger?  Logger             { get; set; }

        #endregion

        #region Custom JSON parsers

        #endregion

        #region Custom JSON serializers

        public CustomJObjectSerializerDelegate<Terminal>?                    CustomTerminalSerializer                      { get; set; }
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
        public CustomJObjectSerializerDelegate<Parking>?                     CustomParkingSerializer                       { get; set; }
        public CustomJObjectSerializerDelegate<BusinessDetails>?             CustomBusinessDetailsSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<Hours>?                       CustomHoursSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<EVSEParking>?                 CustomEVSEParkingSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<Image>?                       CustomImageSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<EnergyMix>?                   CustomEnergyMixSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<EnergySource>?                CustomEnergySourceSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<EnvironmentalImpact>?         CustomEnvironmentalImpactSerializer           { get; set; }


        public CustomJObjectSerializerDelegate<Tariff>?                      CustomTariffSerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<Price>?                       CustomPriceSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<PriceLimit>?                  CustomPriceLimitSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<TariffElement>?               CustomTariffElementSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<PriceComponent>?              CustomPriceComponentSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<TaxAmount>?                   CustomTaxAmountSerializer                     { get; set; }
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
        public OCPIRequestLogEvent OnGetLocationsRequest = new();

        /// <summary>
        /// An event sent whenever a GET locations request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task GetLocationsRequest(DateTimeOffset     Timestamp,
                                                    HTTPAPIX           API,
                                                    OCPIRequest        Request,
                                                    CancellationToken  CancellationToken)

            => OnGetLocationsRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) GetLocationsResponse   (Response)

        /// <summary>
        /// An event sent whenever a GET locations response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnGetLocationsResponse = new();

        /// <summary>
        /// An event sent whenever a GET locations response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task GetLocationsResponse(DateTimeOffset     Timestamp,
                                                     HTTPAPIX           API,
                                                     OCPIRequest        Request,
                                                     OCPIResponse       Response,
                                                     CancellationToken  CancellationToken)

            => OnGetLocationsResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion


        #region (protected internal) GetLocationRequest    (Request)

        /// <summary>
        /// An event sent whenever a GET location request was received.
        /// </summary>
        public OCPIRequestLogEvent OnGetLocationRequest = new();

        /// <summary>
        /// An event sent whenever a GET location request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task GetLocationRequest(DateTimeOffset     Timestamp,
                                                   HTTPAPIX           API,
                                                   OCPIRequest        Request,
                                                   CancellationToken  CancellationToken)

            => OnGetLocationRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) GetLocationResponse   (Response)

        /// <summary>
        /// An event sent whenever a GET location response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnGetLocationResponse = new();

        /// <summary>
        /// An event sent whenever a GET location response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task GetLocationResponse(DateTimeOffset     Timestamp,
                                                    HTTPAPIX           API,
                                                    OCPIRequest        Request,
                                                    OCPIResponse       Response,
                                                    CancellationToken  CancellationToken)

            => OnGetLocationResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion

        #endregion

        #region EVSE

        #region (protected internal) GetEVSERequest    (Request)

        /// <summary>
        /// An event sent whenever a GET EVSE request was received.
        /// </summary>
        public OCPIRequestLogEvent OnGetEVSERequest = new();

        /// <summary>
        /// An event sent whenever a GET EVSE request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task GetEVSERequest(DateTimeOffset     Timestamp,
                                               HTTPAPIX           API,
                                               OCPIRequest        Request,
                                               CancellationToken  CancellationToken)

            => OnGetEVSERequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) GetEVSEResponse   (Response)

        /// <summary>
        /// An event sent whenever a GET EVSE response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnGetEVSEResponse = new();

        /// <summary>
        /// An event sent whenever a GET EVSE response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task GetEVSEResponse(DateTimeOffset     Timestamp,
                                                HTTPAPIX           API,
                                                OCPIRequest        Request,
                                                OCPIResponse       Response,
                                                CancellationToken  CancellationToken)

            => OnGetEVSEResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion

        #endregion

        #region Connector

        #region (protected internal) GetConnectorRequest    (Request)

        /// <summary>
        /// An event sent whenever a GET connector request was received.
        /// </summary>
        public OCPIRequestLogEvent OnGetConnectorRequest = new();

        /// <summary>
        /// An event sent whenever a GET connector request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task GetConnectorRequest(DateTimeOffset     Timestamp,
                                                    HTTPAPIX           API,
                                                    OCPIRequest        Request,
                                                    CancellationToken  CancellationToken)

            => OnGetConnectorRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) GetConnectorResponse   (Response)

        /// <summary>
        /// An event sent whenever a GET connector response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnGetConnectorResponse = new();

        /// <summary>
        /// An event sent whenever a GET connector response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task GetConnectorResponse(DateTimeOffset     Timestamp,
                                                     HTTPAPIX           API,
                                                     OCPIRequest        Request,
                                                     OCPIResponse       Response,
                                                     CancellationToken  CancellationToken)

            => OnGetConnectorResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion

        #endregion

        #region Tariff(s)

        #region (protected internal) GetTariffsRequest (Request)

        /// <summary>
        /// An event sent whenever a GET tariffs request was received.
        /// </summary>
        public OCPIRequestLogEvent OnGetTariffsRequest = new();

        /// <summary>
        /// An event sent whenever a GET tariffs request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task GetTariffsRequest(DateTimeOffset     Timestamp,
                                                  HTTPAPIX           API,
                                                  OCPIRequest        Request,
                                                  CancellationToken  CancellationToken)

            => OnGetTariffsRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) GetTariffsResponse(Response)

        /// <summary>
        /// An event sent whenever a GET tariffs response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnGetTariffsResponse = new();

        /// <summary>
        /// An event sent whenever a GET tariffs response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task GetTariffsResponse(DateTimeOffset     Timestamp,
                                                   HTTPAPIX           API,
                                                   OCPIRequest        Request,
                                                   OCPIResponse       Response,
                                                   CancellationToken  CancellationToken)

            => OnGetTariffsResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion


        #region (protected internal) GetTariffRequest    (Request)

        /// <summary>
        /// An event sent whenever a GET tariff request was received.
        /// </summary>
        public OCPIRequestLogEvent OnGetTariffRequest = new();

        /// <summary>
        /// An event sent whenever a GET tariff request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task GetTariffRequest(DateTimeOffset     Timestamp,
                                                 HTTPAPIX           API,
                                                 OCPIRequest        Request,
                                                 CancellationToken  CancellationToken)

            => OnGetTariffRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) GetTariffResponse   (Response)

        /// <summary>
        /// An event sent whenever a GET tariff response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnGetTariffResponse = new();

        /// <summary>
        /// An event sent whenever a GET tariff response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task GetTariffResponse(DateTimeOffset     Timestamp,
                                                  HTTPAPIX           API,
                                                  OCPIRequest        Request,
                                                  OCPIResponse       Response,
                                                  CancellationToken  CancellationToken)

            => OnGetTariffResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion

        #endregion

        #region Session(s)

        #region (protected internal) GetSessionsRequest (Request)

        /// <summary>
        /// An event sent whenever a GET sessions request was received.
        /// </summary>
        public OCPIRequestLogEvent OnGetSessionsRequest = new();

        /// <summary>
        /// An event sent whenever a GET sessions request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task GetSessionsRequest(DateTimeOffset     Timestamp,
                                                   HTTPAPIX           API,
                                                   OCPIRequest        Request,
                                                   CancellationToken  CancellationToken)

            => OnGetSessionsRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) GetSessionsResponse(Response)

        /// <summary>
        /// An event sent whenever a GET sessions response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnGetSessionsResponse = new();

        /// <summary>
        /// An event sent whenever a GET sessions response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task GetSessionsResponse(DateTimeOffset     Timestamp,
                                                    HTTPAPIX           API,
                                                    OCPIRequest        Request,
                                                    OCPIResponse       Response,
                                                    CancellationToken  CancellationToken)

            => OnGetSessionsResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion


        #region (protected internal) GetSessionRequest    (Request)

        /// <summary>
        /// An event sent whenever a GET session request was received.
        /// </summary>
        public OCPIRequestLogEvent OnGetSessionRequest = new();

        /// <summary>
        /// An event sent whenever a GET session request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task GetSessionRequest(DateTimeOffset     Timestamp,
                                                  HTTPAPIX           API,
                                                  OCPIRequest        Request,
                                                  CancellationToken  CancellationToken)

            => OnGetSessionRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) GetSessionResponse   (Response)

        /// <summary>
        /// An event sent whenever a GET session response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnGetSessionResponse = new();

        /// <summary>
        /// An event sent whenever a GET session response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task GetSessionResponse(DateTimeOffset     Timestamp,
                                                   HTTPAPIX           API,
                                                   OCPIRequest        Request,
                                                   OCPIResponse       Response,
                                                   CancellationToken  CancellationToken)

            => OnGetSessionResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion

        #endregion

        #region CDR(s)

        #region (protected internal) GetCDRsRequest (Request)

        /// <summary>
        /// An event sent whenever a GET CDRs request was received.
        /// </summary>
        public OCPIRequestLogEvent OnGetCDRsRequest = new();

        /// <summary>
        /// An event sent whenever a GET CDRs request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task GetCDRsRequest(DateTimeOffset     Timestamp,
                                               HTTPAPIX           API,
                                               OCPIRequest        Request,
                                               CancellationToken  CancellationToken)

            => OnGetCDRsRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) GetCDRsResponse(Response)

        /// <summary>
        /// An event sent whenever a GET CDRs response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnGetCDRsResponse = new();

        /// <summary>
        /// An event sent whenever a GET CDRs response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task GetCDRsResponse(DateTimeOffset     Timestamp,
                                                HTTPAPIX           API,
                                                OCPIRequest        Request,
                                                OCPIResponse       Response,
                                                CancellationToken  CancellationToken)

            => OnGetCDRsResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion


        #region (protected internal) GetCDRRequest (Request)

        /// <summary>
        /// An event sent whenever a GET CDR request was received.
        /// </summary>
        public OCPIRequestLogEvent OnGetCDRRequest = new();

        /// <summary>
        /// An event sent whenever a GET CDR request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task GetCDRRequest(DateTimeOffset     Timestamp,
                                              HTTPAPIX           API,
                                              OCPIRequest        Request,
                                              CancellationToken  CancellationToken)

            => OnGetCDRRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) GetCDRResponse(Response)

        /// <summary>
        /// An event sent whenever a GET CDR response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnGetCDRResponse = new();

        /// <summary>
        /// An event sent whenever a GET CDR response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task GetCDRResponse(DateTimeOffset     Timestamp,
                                               HTTPAPIX           API,
                                               OCPIRequest        Request,
                                               OCPIResponse       Response,
                                               CancellationToken  CancellationToken)

            => OnGetCDRResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion

        #endregion

        #region Token(s)

        #region (protected internal) GetTokensRequest (Request)

        /// <summary>
        /// An event sent whenever a GET Tokens request was received.
        /// </summary>
        public OCPIRequestLogEvent OnGetTokensRequest = new();

        /// <summary>
        /// An event sent whenever a GET Tokens request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task GetTokensRequest(DateTimeOffset     Timestamp,
                                                 HTTPAPIX           API,
                                                 OCPIRequest        Request,
                                                 CancellationToken  CancellationToken)

            => OnGetTokensRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) GetTokensResponse(Response)

        /// <summary>
        /// An event sent whenever a GET Tokens response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnGetTokensResponse = new();

        /// <summary>
        /// An event sent whenever a GET Tokens response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task GetTokensResponse(DateTimeOffset     Timestamp,
                                                  HTTPAPIX           API,
                                                  OCPIRequest        Request,
                                                  OCPIResponse       Response,
                                                  CancellationToken  CancellationToken)

            => OnGetTokensResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion


        #region (protected internal) DeleteTokensRequest (Request)

        /// <summary>
        /// An event sent whenever a DELETE Tokens request was received.
        /// </summary>
        public OCPIRequestLogEvent OnDeleteTokensRequest = new();

        /// <summary>
        /// An event sent whenever a DELETE Tokens request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task DeleteTokensRequest(DateTimeOffset     Timestamp,
                                                    HTTPAPIX           API,
                                                    OCPIRequest        Request,
                                                    CancellationToken  CancellationToken)

            => OnDeleteTokensRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) DeleteTokensResponse(Response)

        /// <summary>
        /// An event sent whenever a DELETE Tokens response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnDeleteTokensResponse = new();

        /// <summary>
        /// An event sent whenever a DELETE Tokens response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task DeleteTokensResponse(DateTimeOffset     Timestamp,
                                                     HTTPAPIX           API,
                                                     OCPIRequest        Request,
                                                     OCPIResponse       Response,
                                                     CancellationToken  CancellationToken)

            => OnDeleteTokensResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion


        // Token

        #region (protected internal) GetTokenRequest (Request)

        /// <summary>
        /// An event sent whenever a GET Token request was received.
        /// </summary>
        public OCPIRequestLogEvent OnGetTokenRequest = new();

        /// <summary>
        /// An event sent whenever a GET Token request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task GetTokenRequest(DateTimeOffset     Timestamp,
                                                HTTPAPIX           API,
                                                OCPIRequest        Request,
                                                CancellationToken  CancellationToken)

            => OnGetTokenRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) GetTokenResponse(Response)

        /// <summary>
        /// An event sent whenever a GET Token response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnGetTokenResponse = new();

        /// <summary>
        /// An event sent whenever a GET Token response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task GetTokenResponse(DateTimeOffset     Timestamp,
                                                 HTTPAPIX           API,
                                                 OCPIRequest        Request,
                                                 OCPIResponse       Response,
                                                 CancellationToken  CancellationToken)

            => OnGetTokenResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion


        #region (protected internal) PostTokenRequest (Request)

        /// <summary>
        /// An event sent whenever a POST token request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPostTokenRequest = new();

        /// <summary>
        /// An event sent whenever a POST token request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PostTokenRequest(DateTimeOffset     Timestamp,
                                                 HTTPAPIX           API,
                                                 OCPIRequest        Request,
                                                 CancellationToken  CancellationToken)

            => OnPostTokenRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) PostTokenResponse(Response)

        /// <summary>
        /// An event sent whenever a POST token response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnPostTokenResponse = new();

        /// <summary>
        /// An event sent whenever a POST token response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PostTokenResponse(DateTimeOffset     Timestamp,
                                                  HTTPAPIX           API,
                                                  OCPIRequest        Request,
                                                  OCPIResponse       Response,
                                                  CancellationToken  CancellationToken)

            => OnPostTokenResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion


        #region (protected internal) PutTokenRequest    (Request)

        /// <summary>
        /// An event sent whenever a put token request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPutTokenRequest = new();

        /// <summary>
        /// An event sent whenever a put token request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PutTokenRequest(DateTimeOffset     Timestamp,
                                                HTTPAPIX           API,
                                                OCPIRequest        Request,
                                                CancellationToken  CancellationToken)

            => OnPutTokenRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) PutTokenResponse   (Response)

        /// <summary>
        /// An event sent whenever a put token response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnPutTokenResponse = new();

        /// <summary>
        /// An event sent whenever a put token response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PutTokenResponse(DateTimeOffset     Timestamp,
                                                 HTTPAPIX           API,
                                                 OCPIRequest        Request,
                                                 OCPIResponse       Response,
                                                 CancellationToken  CancellationToken)

            => OnPutTokenResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion


        #region (protected internal) PatchTokenRequest  (Request)

        /// <summary>
        /// An event sent whenever a patch token request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPatchTokenRequest = new();

        /// <summary>
        /// An event sent whenever a patch token request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PatchTokenRequest(DateTimeOffset     Timestamp,
                                                  HTTPAPIX           API,
                                                  OCPIRequest        Request,
                                                  CancellationToken  CancellationToken)

            => OnPatchTokenRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) PatchTokenResponse (Response)

        /// <summary>
        /// An event sent whenever a patch token response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnPatchTokenResponse = new();

        /// <summary>
        /// An event sent whenever a patch token response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PatchTokenResponse(DateTimeOffset     Timestamp,
                                                   HTTPAPIX           API,
                                                   OCPIRequest        Request,
                                                   OCPIResponse       Response,
                                                   CancellationToken  CancellationToken)

            => OnPatchTokenResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion


        #region (protected internal) DeleteTokenRequest (Request)

        /// <summary>
        /// An event sent whenever a delete token request was received.
        /// </summary>
        public OCPIRequestLogEvent OnDeleteTokenRequest = new();

        /// <summary>
        /// An event sent whenever a delete token request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task DeleteTokenRequest(DateTimeOffset     Timestamp,
                                                   HTTPAPIX           API,
                                                   OCPIRequest        Request,
                                                   CancellationToken  CancellationToken)

            => OnDeleteTokenRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) DeleteTokenResponse(Response)

        /// <summary>
        /// An event sent whenever a delete token response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnDeleteTokenResponse = new();

        /// <summary>
        /// An event sent whenever a delete token response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task DeleteTokenResponse(DateTimeOffset     Timestamp,
                                                    HTTPAPIX           API,
                                                    OCPIRequest        Request,
                                                    OCPIResponse       Response,
                                                    CancellationToken  CancellationToken)

            => OnDeleteTokenResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion

        #endregion

        #region Terminal(s)

        #region (protected internal) GetTerminalRequest    (Request)

        /// <summary>
        /// An event sent whenever a GET terminal request was received.
        /// </summary>
        public OCPIRequestLogEvent OnGetTerminalRequest = new();

        /// <summary>
        /// An event sent whenever a GET terminal request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The PTP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task GetTerminalRequest(DateTimeOffset     Timestamp,
                                                   HTTPAPIX           API,
                                                   OCPIRequest        Request,
                                                   CancellationToken  CancellationToken)

            => OnGetTerminalRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) GetTerminalResponse   (Response)

        /// <summary>
        /// An event sent whenever a GET terminal response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnGetTerminalResponse = new();

        /// <summary>
        /// An event sent whenever a GET terminal response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The PTP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task GetTerminalResponse(DateTimeOffset     Timestamp,
                                                    HTTPAPIX           API,
                                                    OCPIRequest        Request,
                                                    OCPIResponse       Response,
                                                    CancellationToken  CancellationToken)

            => OnGetTerminalResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion


        #region (protected internal) PostTerminalRequest    (Request)

        /// <summary>
        /// An event sent whenever a POST terminal request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPostTerminalRequest = new();

        /// <summary>
        /// An event sent whenever a POST terminal request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The PTP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PostTerminalRequest(DateTimeOffset     Timestamp,
                                                    HTTPAPIX           API,
                                                    OCPIRequest        Request,
                                                    CancellationToken  CancellationToken)

            => OnPostTerminalRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) PostTerminalResponse   (Response)

        /// <summary>
        /// An event sent whenever a POST terminal response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnPostTerminalResponse = new();

        /// <summary>
        /// An event sent whenever a POST terminal response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The PTP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PostTerminalResponse(DateTimeOffset     Timestamp,
                                                     HTTPAPIX           API,
                                                     OCPIRequest        Request,
                                                     OCPIResponse       Response,
                                                     CancellationToken  CancellationToken)

            => OnPostTerminalResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion

        #endregion


        // Commands

        #region (protected internal) ReserveNowRequest        (Request)

        /// <summary>
        /// An event sent whenever a reserve now command was received.
        /// </summary>
        public OCPIRequestLogEvent OnReserveNowRequest = new();

        /// <summary>
        /// An event sent whenever a reserve now command was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the command request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task ReserveNowRequest(DateTimeOffset     Timestamp,
                                                  HTTPAPIX           API,
                                                  OCPIRequest        Request,
                                                  CancellationToken  CancellationToken)

            => OnReserveNowRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

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
        public OCPIResponseLogEvent OnReserveNowResponse = new();

        /// <summary>
        /// An event sent whenever a reserve now command response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the command response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task ReserveNowResponse(DateTimeOffset     Timestamp,
                                                   HTTPAPIX           API,
                                                   OCPIRequest        Request,
                                                   OCPIResponse       Response,
                                                   CancellationToken  CancellationToken)

            => OnReserveNowResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion


        #region (protected internal) CancelReservationRequest (Request)

        /// <summary>
        /// An event sent whenever a cancel reservation command was received.
        /// </summary>
        public OCPIRequestLogEvent OnCancelReservationRequest = new();

        /// <summary>
        /// An event sent whenever a cancel reservation command was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the command request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task CancelReservationRequest(DateTimeOffset     Timestamp,
                                                         HTTPAPIX           API,
                                                         OCPIRequest        Request,
                                                         CancellationToken  CancellationToken)

            => OnCancelReservationRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

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
        public OCPIResponseLogEvent OnCancelReservationResponse = new();

        /// <summary>
        /// An event sent whenever a cancel reservation command response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the command response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task CancelReservationResponse(DateTimeOffset     Timestamp,
                                                          HTTPAPIX           API,
                                                          OCPIRequest        Request,
                                                          OCPIResponse       Response,
                                                          CancellationToken  CancellationToken)

            => OnCancelReservationResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion


        #region (protected internal) StartSessionRequest      (Request)

        /// <summary>
        /// An event sent whenever a start session command was received.
        /// </summary>
        public OCPIRequestLogEvent OnStartSessionRequest = new();

        /// <summary>
        /// An event sent whenever a start session command was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the command request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task StartSessionRequest(DateTimeOffset     Timestamp,
                                                    HTTPAPIX           API,
                                                    OCPIRequest        Request,
                                                    CancellationToken  CancellationToken)

            => OnStartSessionRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

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
        public OCPIResponseLogEvent OnStartSessionResponse = new();

        /// <summary>
        /// An event sent whenever a start session command response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the command response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task StartSessionResponse(DateTimeOffset     Timestamp,
                                                     HTTPAPIX           API,
                                                     OCPIRequest        Request,
                                                     OCPIResponse       Response,
                                                     CancellationToken  CancellationToken)

            => OnStartSessionResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion


        #region (protected internal) StopSessionRequest       (Request)

        /// <summary>
        /// An event sent whenever a stop session command was received.
        /// </summary>
        public OCPIRequestLogEvent OnStopSessionRequest = new();

        /// <summary>
        /// An event sent whenever a stop session command was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the command request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task StopSessionRequest(DateTimeOffset     Timestamp,
                                                   HTTPAPIX           API,
                                                   OCPIRequest        Request,
                                                   CancellationToken  CancellationToken)

            => OnStopSessionRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

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
        public OCPIResponseLogEvent OnStopSessionResponse = new();

        /// <summary>
        /// An event sent whenever a stop session command response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the command response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task StopSessionResponse(DateTimeOffset     Timestamp,
                                                    HTTPAPIX           API,
                                                    OCPIRequest        Request,
                                                    OCPIResponse       Response,
                                                    CancellationToken  CancellationToken)

            => OnStopSessionResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion


        #region (protected internal) UnlockConnectorRequest   (Request)

        /// <summary>
        /// An event sent whenever a unlock connector command was received.
        /// </summary>
        public OCPIRequestLogEvent OnUnlockConnectorRequest = new();

        /// <summary>
        /// An event sent whenever a unlock connector command was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the command request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task UnlockConnectorRequest(DateTimeOffset     Timestamp,
                                                       HTTPAPIX           API,
                                                       OCPIRequest        Request,
                                                       CancellationToken  CancellationToken)

            => OnUnlockConnectorRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

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
        public OCPIResponseLogEvent OnUnlockConnectorResponse = new();

        /// <summary>
        /// An event sent whenever a unlock connector command response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the command response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task UnlockConnectorResponse(DateTimeOffset     Timestamp,
                                                        HTTPAPIX           API,
                                                        OCPIRequest        Request,
                                                        OCPIResponse       Response,
                                                        CancellationToken  CancellationToken)

            => OnUnlockConnectorResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

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
        /// <param name="ExternalDNSName">The official URL/DNS name of this service, e.g. for sending e-mails.</param>
        /// <param name="URLPathPrefix">An optional URL path prefix.</param>
        /// <param name="BasePath">When the API is served from an optional subdirectory path.</param>
        /// <param name="HTTPServiceName">An optional name of the HTTP API service.</param>
        public CPOAPI(CommonAPI                    CommonAPI,
                      I18NString?                  Description          = null,
                      Boolean?                     AllowDowngrades      = null,

                      HTTPPath?                    BasePath             = null,
                      HTTPPath?                    URLPathPrefix        = null,

                      String?                      ExternalDNSName      = null,
                      String?                      HTTPServerName       = DefaultHTTPServerName,
                      String?                      HTTPServiceName      = DefaultHTTPServiceName,
                      String?                      APIVersionHash       = null,
                      JObject?                     APIVersionHashes     = null,

                      Boolean?                     IsDevelopment        = false,
                      IEnumerable<String>?         DevelopmentServers   = null,
                      Boolean?                     DisableLogging       = false,
                      String?                      LoggingContext       = null,
                      String?                      LoggingPath          = null,
                      String?                      LogfileName          = null,
                      OCPILogfileCreatorDelegate?  LogfileCreator       = null)

            : base(CommonAPI,
                   CommonAPI.URLPathPrefix + (URLPathPrefix ?? DefaultURLPathPrefix),
                   BasePath,

                   Description     ?? I18NString.Create($"OCPI{Version.String} CPO API"),

                   ExternalDNSName,
                   HTTPServerName  ?? DefaultHTTPServerName,
                   HTTPServiceName ?? DefaultHTTPServiceName,
                   APIVersionHash,
                   APIVersionHashes,

                   IsDevelopment,
                   DevelopmentServers,
                   DisableLogging,
                   LoggingPath,
                   LogfileName     ?? DefaultLogfileName,
                   LogfileCreator is not null
                       ? (loggingPath, context, logfileName) => LogfileCreator(loggingPath, null, context, logfileName)
                       : null)

        {

            this.AllowDowngrades  = AllowDowngrades;

            this.Logger           = this.DisableLogging == false
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


        #region CPO-2-EMSP Clients

        private readonly ConcurrentDictionary<EMSP_Id, CPO2EMSPClient> cpo2emspClients = new();

        /// <summary>
        /// Return an enumeration of all CPO2EMSP clients.
        /// </summary>
        public IEnumerable<CPO2EMSPClient> CPO2EMSPClients
            => cpo2emspClients.Values;


        #region GetEMSPClient(CountryCode, PartyId, Description = null, AllowCachedClients = true)

        /// <summary>
        /// As a CPO create a client to access e.g. a remote EMSP.
        /// </summary>
        /// <param name="CountryCode">The country code of the remote EMSP.</param>
        /// <param name="PartyId">The party identification of the remote EMSP.</param>
        /// <param name="Description">A description for the OCPI client.</param>
        /// <param name="AllowCachedClients">Whether to allow to return cached CPO clients.</param>
        public CPO2EMSPClient? GetEMSPClient(CountryCode  CountryCode,
                                             Party_Id     PartyId,
                                             I18NString?  Description          = null,
                                             Boolean      AllowCachedClients   = true)
        {

            var emspId         = EMSP_Id.       Parse(CountryCode, PartyId);
            var remotePartyId  = RemoteParty_Id.From (emspId);

            if (AllowCachedClients &&
                cpo2emspClients.TryGetValue(emspId, out var cachedCPOClient))
            {
                return cachedCPOClient;
            }

            if (CommonAPI.TryGetRemoteParty(remotePartyId, out var remoteParty) &&
                remoteParty?.RemoteAccessInfos?.Any() == true)
            {

                var cpoClient = new CPO2EMSPClient(
                                    this,
                                    remoteParty,
                                    null,
                                    Description ?? CommonAPI.BaseAPI.ClientConfigurations.Description?.Invoke(remotePartyId),
                                    null,
                                    CommonAPI.BaseAPI.ClientConfigurations.DisableLogging?.Invoke(remotePartyId),
                                    CommonAPI.BaseAPI.ClientConfigurations.LoggingPath?.   Invoke(remotePartyId),
                                    CommonAPI.BaseAPI.ClientConfigurations.LoggingContext?.Invoke(remotePartyId),
                                    CommonAPI.BaseAPI.ClientConfigurations.LogfileCreator,
                                    CommonAPI.HTTPBaseAPI.HTTPServer.DNSClient
                                );

                cpo2emspClients.TryAdd(emspId, cpoClient);

                return cpoClient;

            }

            return null;

        }

        #endregion

        #region GetEMSPClient(RemoteParty,          Description = null, AllowCachedClients = true)

        /// <summary>
        /// As a CPO create a client to access e.g. a remote EMSP.
        /// </summary>
        /// <param name="RemoteParty">A remote party.</param>
        /// <param name="Description">A description for the OCPI client.</param>
        /// <param name="AllowCachedClients">Whether to allow to return cached CPO clients.</param>
        public CPO2EMSPClient? GetEMSPClient(RemoteParty  RemoteParty,
                                             I18NString?  Description          = null,
                                             Boolean      AllowCachedClients   = true)
        {

            var emspId = EMSP_Id.From(RemoteParty.Id);

            if (AllowCachedClients &&
                cpo2emspClients.TryGetValue(emspId, out var cachedCPOClient))
            {
                return cachedCPOClient;
            }

            if (RemoteParty?.RemoteAccessInfos?.Any() == true)
            {

                var cpoClient = new CPO2EMSPClient(
                                    this,
                                    RemoteParty,
                                    null,
                                    Description ?? CommonAPI.BaseAPI.ClientConfigurations.Description?.Invoke(RemoteParty.Id),
                                    null,
                                    CommonAPI.BaseAPI.ClientConfigurations.DisableLogging?.Invoke(RemoteParty.Id),
                                    CommonAPI.BaseAPI.ClientConfigurations.LoggingPath?.   Invoke(RemoteParty.Id),
                                    CommonAPI.BaseAPI.ClientConfigurations.LoggingContext?.Invoke(RemoteParty.Id),
                                    CommonAPI.BaseAPI.ClientConfigurations.LogfileCreator,
                                    CommonAPI.HTTPBaseAPI.HTTPServer.DNSClient
                                );

                cpo2emspClients.TryAdd(emspId, cpoClient);

                return cpoClient;

            }

            return null;

        }

        #endregion

        #region GetEMSPClient(RemotePartyId,        Description = null, AllowCachedClients = true)

        /// <summary>
        /// As a CPO create a client to access e.g. a remote EMSP.
        /// </summary>
        /// <param name="RemotePartyId">A remote party identification.</param>
        /// <param name="Description">A description for the OCPI client.</param>
        /// <param name="AllowCachedClients">Whether to allow to return cached CPO clients.</param>
        public CPO2EMSPClient? GetEMSPClient(RemoteParty_Id  RemotePartyId,
                                             I18NString?     Description          = null,
                                             Boolean         AllowCachedClients   = true)
        {

            var emspId = EMSP_Id.From(RemotePartyId);

            if (AllowCachedClients &&
                cpo2emspClients.TryGetValue(emspId, out var cachedCPOClient))
            {
                return cachedCPOClient;
            }

            if (CommonAPI.TryGetRemoteParty(RemotePartyId, out var remoteParty) &&
                remoteParty?.RemoteAccessInfos?.Any() == true)
            {

                var cpoClient = new CPO2EMSPClient(
                                    this,
                                    remoteParty,
                                    null,
                                    Description ?? CommonAPI.BaseAPI.ClientConfigurations.Description?.Invoke(RemotePartyId),
                                    null,
                                    CommonAPI.BaseAPI.ClientConfigurations.DisableLogging?.Invoke(RemotePartyId),
                                    CommonAPI.BaseAPI.ClientConfigurations.LoggingPath?.   Invoke(RemotePartyId),
                                    CommonAPI.BaseAPI.ClientConfigurations.LoggingContext?.Invoke(RemotePartyId),
                                    CommonAPI.BaseAPI.ClientConfigurations.LogfileCreator,
                                    CommonAPI.HTTPBaseAPI.HTTPServer.DNSClient
                                );

                cpo2emspClients.TryAdd(emspId, cpoClient);

                return cpoClient;

            }

            return null;

        }

        #endregion

        #endregion

        #region CPO-2-PTP  Clients

        private readonly ConcurrentDictionary<PTP_Id, CPO2PTPClient> cpo2ptpClients = new();

        /// <summary>
        /// Return an enumeration of all CPO2PTP clients.
        /// </summary>
        public IEnumerable<CPO2PTPClient> CPO2PTPClients
            => cpo2ptpClients.Values;


        #region GetPTPClient(CountryCode, PartyId, Description = null, AllowCachedClients = true)

        /// <summary>
        /// As a CPO create a client to access e.g. a remote PTP.
        /// </summary>
        /// <param name="CountryCode">The country code of the remote PTP.</param>
        /// <param name="PartyId">The party identification of the remote PTP.</param>
        /// <param name="Description">A description for the OCPI client.</param>
        /// <param name="AllowCachedClients">Whether to allow to return cached CPO clients.</param>
        public CPO2PTPClient? GetPTPClient(CountryCode  CountryCode,
                                           Party_Id     PartyId,
                                           I18NString?  Description          = null,
                                           Boolean      AllowCachedClients   = true)
        {

            var ptpId          = PTP_Id.        Parse(CountryCode, PartyId);
            var remotePartyId  = RemoteParty_Id.From (ptpId);

            if (AllowCachedClients &&
                cpo2ptpClients.TryGetValue(ptpId, out var cachedCPOClient))
            {
                return cachedCPOClient;
            }

            if (CommonAPI.TryGetRemoteParty(remotePartyId, out var remoteParty) &&
                remoteParty?.RemoteAccessInfos?.Any() == true)
            {

                var cpoClient = new CPO2PTPClient(
                                    this,
                                    remoteParty,
                                    null,
                                    Description ?? CommonAPI.BaseAPI.ClientConfigurations.Description?.Invoke(remotePartyId),
                                    null,
                                    CommonAPI.BaseAPI.ClientConfigurations.DisableLogging?.Invoke(remotePartyId),
                                    CommonAPI.BaseAPI.ClientConfigurations.LoggingPath?.   Invoke(remotePartyId),
                                    CommonAPI.BaseAPI.ClientConfigurations.LoggingContext?.Invoke(remotePartyId),
                                    CommonAPI.BaseAPI.ClientConfigurations.LogfileCreator,
                                    CommonAPI.HTTPBaseAPI.HTTPServer.DNSClient
                                );

                cpo2ptpClients.TryAdd(ptpId, cpoClient);

                return cpoClient;

            }

            return null;

        }

        #endregion

        #region GetPTPClient(RemoteParty,          Description = null, AllowCachedClients = true)

        /// <summary>
        /// As a CPO create a client to access e.g. a remote PTP.
        /// </summary>
        /// <param name="RemoteParty">A remote party.</param>
        /// <param name="Description">A description for the OCPI client.</param>
        /// <param name="AllowCachedClients">Whether to allow to return cached CPO clients.</param>
        public CPO2PTPClient? GetPTPClient(RemoteParty  RemoteParty,
                                           I18NString?  Description          = null,
                                           Boolean      AllowCachedClients   = true)
        {

            var ptpId = PTP_Id.From(RemoteParty.Id);

            if (AllowCachedClients &&
                cpo2ptpClients.TryGetValue(ptpId, out var cachedCPOClient))
            {
                return cachedCPOClient;
            }

            if (RemoteParty?.RemoteAccessInfos?.Any() == true)
            {

                var cpoClient = new CPO2PTPClient(
                                    this,
                                    RemoteParty,
                                    null,
                                    Description ?? CommonAPI.BaseAPI.ClientConfigurations.Description?.Invoke(RemoteParty.Id),
                                    null,
                                    CommonAPI.BaseAPI.ClientConfigurations.DisableLogging?.Invoke(RemoteParty.Id),
                                    CommonAPI.BaseAPI.ClientConfigurations.LoggingPath?.   Invoke(RemoteParty.Id),
                                    CommonAPI.BaseAPI.ClientConfigurations.LoggingContext?.Invoke(RemoteParty.Id),
                                    CommonAPI.BaseAPI.ClientConfigurations.LogfileCreator,
                                    CommonAPI.HTTPBaseAPI.HTTPServer.DNSClient
                                );

                cpo2ptpClients.TryAdd(ptpId, cpoClient);

                return cpoClient;

            }

            return null;

        }

        #endregion

        #region GetPTPClient(RemotePartyId,        Description = null, AllowCachedClients = true)

        /// <summary>
        /// As a CPO create a client to access e.g. a remote PTP.
        /// </summary>
        /// <param name="RemotePartyId">A remote party identification.</param>
        /// <param name="Description">A description for the OCPI client.</param>
        /// <param name="AllowCachedClients">Whether to allow to return cached CPO clients.</param>
        public CPO2PTPClient? GetPTPClient(RemoteParty_Id  RemotePartyId,
                                           I18NString?     Description          = null,
                                           Boolean         AllowCachedClients   = true)
        {

            var ptpId = PTP_Id.From(RemotePartyId);

            if (AllowCachedClients &&
                cpo2ptpClients.TryGetValue(ptpId, out var cachedCPOClient))
            {
                return cachedCPOClient;
            }

            if (CommonAPI.TryGetRemoteParty(RemotePartyId, out var remoteParty) &&
                remoteParty?.RemoteAccessInfos?.Any() == true)
            {

                var cpoClient = new CPO2PTPClient(
                                    this,
                                    remoteParty,
                                    null,
                                    Description ?? CommonAPI.BaseAPI.ClientConfigurations.Description?.Invoke(RemotePartyId),
                                    null,
                                    CommonAPI.BaseAPI.ClientConfigurations.DisableLogging?.Invoke(RemotePartyId),
                                    CommonAPI.BaseAPI.ClientConfigurations.LoggingPath?.   Invoke(RemotePartyId),
                                    CommonAPI.BaseAPI.ClientConfigurations.LoggingContext?.Invoke(RemotePartyId),
                                    CommonAPI.BaseAPI.ClientConfigurations.LogfileCreator,
                                    CommonAPI.HTTPBaseAPI.HTTPServer.DNSClient
                                );

                cpo2ptpClients.TryAdd(ptpId, cpoClient);

                return cpoClient;

            }

            return null;

        }

        #endregion

        #endregion


        #region (private) RegisterURLTemplates()

        private void RegisterURLTemplates()
        {

            #region ~/locations

            #region OPTIONS  ~/locations

            CommonAPI.AddOCPIMethod(

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
            CommonAPI.AddOCPIMethod(

                HTTPMethod.GET,
                URLPathPrefix + "locations",
                HTTPContentType.Application.JSON_UTF8,
                OCPIRequestLogger:   GetLocationsRequest,
                OCPIResponseLogger:  GetLocationsResponse,
                OCPIRequestHandler:  request => {

                    #region Check access token

                    if ((request.LocalAccessInfo is not null || CommonAPI.BaseAPI.LocationsAsOpenData == false) &&
                        (request.LocalAccessInfo?.Status            != AccessStatus.ALLOWED ||
                         request.LocalAccessInfo.IsNot(Role.EMSP) == true))
                    {


                    //if (request.LocalAccessInfo.IsNot(Role.EMSP) == true ||
                    //    Request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                    //{

                        return Task.FromResult(
                            new OCPIResponse.Builder(request) {
                                StatusCode           = 2000,
                                StatusMessage        = "Invalid or blocked access token!",
                                HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                    HTTPStatusCode              = HTTPStatusCode.Forbidden,
                                    AccessControlAllowMethods   = [ "OPTIONS", "GET", "DELETE" ],
                                    AccessControlAllowHeaders   = [ "Authorization" ],
                                    AccessControlExposeHeaders  = [ "X-Request-ID", "X-Correlation-ID", "Link", "X-Total-Count", "X-Filtered-Count" ]
                                }
                            });

                    }

                    #endregion


                    var withExtensions       = request.QueryString.GetBoolean ("withExtensions", false);


                    var filters              = request.GetDateAndPaginationFilters();
                    var matchFilter          = request.QueryString.CreateStringFilter<Location>(
                                                   "match",
                                                   (location, pattern) => location.Id.     ToString().Contains(pattern)         ||
                                                                          location.Name?.             Contains(pattern) == true ||
                                                                          location.Address.           Contains(pattern)         ||
                                                                          location.City.              Contains(pattern)         ||
                                                                         (location.PostalCode ?? ""). Contains(pattern)         ||
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


                    var httpResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
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
                                                                                     : $"http://127.0.0.1:{CommonAPI.BaseAPI.HTTPBaseAPI.HTTPServer.TCPPort}")}{URLPathPrefix}/locations{queryParameters}>; rel=\"next\"");

                    }

                    #endregion


                    return Task.FromResult(
                               new OCPIResponse.Builder(request) {
                                   StatusCode           = 1000,
                                   StatusMessage        = "Hello world!",
                                   HTTPResponseBuilder  = httpResponseBuilder,
                                   Data                 = new JArray(
                                                              filteredLocations.
                                                                  OrderBy       (location => location.Created).
                                                                  SkipTakeFilter(filters.Offset,
                                                                                 filters.Limit).
                                                                  Select        (location => location.ToJSON(request.EMSPId,
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
                                                                                                             CustomParkingSerializer,
                                                                                                             CustomDisplayTextSerializer,
                                                                                                             CustomBusinessDetailsSerializer,
                                                                                                             CustomHoursSerializer,
                                                                                                             CustomEVSEParkingSerializer,
                                                                                                             CustomImageSerializer,
                                                                                                             CustomEnergyMixSerializer,
                                                                                                             CustomEnergySourceSerializer,
                                                                                                             CustomEnvironmentalImpactSerializer))
                                                          )
                               }
                           );

                });

            #endregion

            #endregion

            #region ~/locations/{locationId}

            #region OPTIONS  ~/locations/{locationId}

            CommonAPI.AddOCPIMethod(

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

            CommonAPI.AddOCPIMethod(

                HTTPMethod.GET,
                URLPathPrefix + "locations/{locationId}",
                HTTPContentType.Application.JSON_UTF8,
                OCPIRequestLogger:   GetLocationRequest,
                OCPIResponseLogger:  GetLocationResponse,
                OCPIRequestHandler:  request => {

                    #region Check access token

                    if (request.LocalAccessInfo.IsNot(Role.EMSP) == true ||
                        request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                    {

                        return Task.FromResult(
                            new OCPIResponse.Builder(request) {
                                StatusCode           = 2000,
                                StatusMessage        = "Invalid or blocked access token!",
                                HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                    HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                    AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                    AccessControlAllowHeaders  = [ "Authorization" ]
                                }
                            });

                    }

                    #endregion

                    #region Check location

                    if (!request.ParseLocation(CommonAPI,
                                               //Request.AccessInfo.Value.Roles.Select(role => new Tuple<CountryCode, Party_Id>(role.CountryCode, role.PartyId)),
                                               CommonAPI.Parties.Select(partyData => new Tuple<CountryCode, Party_Id>(partyData.Id.CountryCode, partyData.Id.Party)),
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
                        new OCPIResponse.Builder(request) {
                               StatusCode           = 1000,
                               StatusMessage        = "Hello world!",
                               Data                 = location.ToJSON(request.EMSPId,
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
                                                                      CustomParkingSerializer,
                                                                      CustomDisplayTextSerializer,
                                                                      CustomBusinessDetailsSerializer,
                                                                      CustomHoursSerializer,
                                                                      CustomEVSEParkingSerializer,
                                                                      CustomImageSerializer,
                                                                      CustomEnergyMixSerializer,
                                                                      CustomEnergySourceSerializer,
                                                                      CustomEnvironmentalImpactSerializer),
                               HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
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

            CommonAPI.AddOCPIMethod(

                HTTPMethod.GET,
                URLPathPrefix + "locations/{locationId}/{evseId}",
                HTTPContentType.Application.JSON_UTF8,
                OCPIRequestHandler: request => {

                    #region Check access token

                    if (request.LocalAccessInfo.IsNot(Role.EMSP) == true ||
                        request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                    {

                        return Task.FromResult(
                            new OCPIResponse.Builder(request) {
                                StatusCode           = 2000,
                                StatusMessage        = "Invalid or blocked access token!",
                                HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                    HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                    AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                    AccessControlAllowHeaders  = [ "Authorization" ]
                                }
                            });

                    }

                    #endregion

                    #region Check EVSE

                    if (!request.ParseLocationEVSE(CommonAPI,
                                                   //Request.AccessInfo.Value.Roles.Select(role => new Tuple<CountryCode, Party_Id>(role.CountryCode, role.PartyId)),
                                                   CommonAPI.Parties.Select(partyData => new Tuple<CountryCode, Party_Id>(partyData.Id.CountryCode, partyData.Id.Party)),
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
                        new OCPIResponse.Builder(request) {
                               StatusCode           = 1000,
                               StatusMessage        = "Hello world!",
                               Data                 = evse.ToJSON(true,
                                                                  true,
                                                                  true,
                                                                  request.EMSPId,
                                                                  CustomEVSESerializer,
                                                                  CustomStatusScheduleSerializer,
                                                                  CustomConnectorSerializer,
                                                                  CustomEVSEEnergyMeterSerializer,
                                                                  CustomTransparencySoftwareStatusSerializer,
                                                                  CustomTransparencySoftwareSerializer,
                                                                  CustomDisplayTextSerializer,
                                                                  CustomEVSEParkingSerializer,
                                                                  CustomImageSerializer),
                               HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
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

            CommonAPI.AddOCPIMethod(

                HTTPMethod.GET,
                URLPathPrefix + "locations/{locationId}/{evseId}/{connectorId}",
                HTTPContentType.Application.JSON_UTF8,
                OCPIRequestHandler: request => {

                    #region Check access token

                    if (request.LocalAccessInfo.IsNot(Role.EMSP) == true ||
                        request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                    {

                        return Task.FromResult(
                            new OCPIResponse.Builder(request) {
                                StatusCode           = 2000,
                                StatusMessage        = "Invalid or blocked access token!",
                                HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                    HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                    AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                    AccessControlAllowHeaders  = [ "Authorization" ]
                                }
                            });

                    }

                    #endregion

                    #region Check connector

                    if (!request.ParseLocationEVSEConnector(CommonAPI,
                                                            //Request.AccessInfo.Value.Roles.Select(role => new Tuple<CountryCode, Party_Id>(role.CountryCode, role.PartyId)),
                                                            CommonAPI.Parties.Select(partyData => new Tuple<CountryCode, Party_Id>(partyData.Id.CountryCode, partyData.Id.Party)),
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
                        new OCPIResponse.Builder(request) {
                               StatusCode           = 1000,
                               StatusMessage        = "Hello world!",
                               Data                 = connector.ToJSON(true,
                                                                       true,
                                                                       request.EMSPId,
                                                                       CustomConnectorSerializer),
                               HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
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
            CommonAPI.AddOCPIMethod(

                HTTPMethod.GET,
                URLPathPrefix + "tariffs",
                HTTPContentType.Application.JSON_UTF8,
                OCPIRequestHandler: request => {

                    #region Check access token

                    if (request.LocalAccessInfo.IsNot(Role.EMSP) == true ||
                        request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                    {

                        return Task.FromResult(
                            new OCPIResponse.Builder(request) {
                                StatusCode           = 2000,
                                StatusMessage        = "Invalid or blocked access token!",
                                HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                    HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                    AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                    AccessControlAllowHeaders  = [ "Authorization" ]
                                }
                            });

                    }

                    #endregion


                    var filters          = request.GetDateAndPaginationFilters();

                    var allTariffs       = CommonAPI.//GetTariffs(tariff => Request.AccessInfo.Value.Roles.Any(role => role.CountryCode == tariff.CountryCode &&
                                                     //                                                                role.PartyId     == tariff.PartyId)).
                                                     GetTariffs(tariff => CommonAPI.Parties.Any(partyData => partyData.Id.CountryCode == tariff.CountryCode &&
                                                                                                             partyData.Id.Party       == tariff.PartyId)).
                                                     ToArray();

                    var filteredTariffs  = allTariffs.Where(tariff => !filters.From.HasValue || tariff.LastUpdated >  filters.From.Value).
                                                      Where(tariff => !filters.To.  HasValue || tariff.LastUpdated <= filters.To.  Value).
                                                      ToArray();


                    var httpResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
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
                                                                                     : $"http://127.0.0.1:{CommonAPI.BaseAPI.HTTPBaseAPI.HTTPServer.TCPPort}")}{URLPathPrefix}/tariffs{queryParameters}>; rel=\"next\"");

                    }

                    #endregion

                    return Task.FromResult(
                               new OCPIResponse.Builder(request) {
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
                                                                                                     CustomPriceLimitSerializer,
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

            CommonAPI.AddOCPIMethod(

                HTTPMethod.GET,
                URLPathPrefix + "tariffs/{tariffId}",
                HTTPContentType.Application.JSON_UTF8,
                OCPIRequestHandler: request => {

                    #region Check access token

                    if (request.LocalAccessInfo.IsNot(Role.EMSP) == true ||
                        request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                    {

                        return Task.FromResult(
                            new OCPIResponse.Builder(request) {
                                StatusCode           = 2000,
                                StatusMessage        = "Invalid or blocked access token!",
                                HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                    HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                    AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                    AccessControlAllowHeaders  = [ "Authorization" ]
                                }
                            });

                    }

                    #endregion

                    #region Check tariff

                    if (!request.ParseTariff(CommonAPI,
                                             //Request.AccessInfo.Value.Roles.Select(role => new Tuple<CountryCode, Party_Id>(role.CountryCode, role.PartyId)),
                                             CommonAPI.Parties.Select(partyData => new Tuple<CountryCode, Party_Id>(partyData.Id.CountryCode, partyData.Id.Party)),
                                             out var tariffId,
                                             out var tariff,
                                             out var ocpiResponseBuilder) ||
                         tariff is null)
                    {
                        return Task.FromResult(ocpiResponseBuilder!);
                    }

                    #endregion


                    return Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               StatusCode           = 1000,
                               StatusMessage        = "Hello world!",
                               Data                 = tariff.ToJSON(),
                               HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
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
            CommonAPI.AddOCPIMethod(

                HTTPMethod.GET,
                URLPathPrefix + "sessions",
                HTTPContentType.Application.JSON_UTF8,
                OCPIRequestHandler: request => {

                    #region Check access token

                    if (request.LocalAccessInfo.IsNot(Role.EMSP) == true ||
                        request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                    {

                        return Task.FromResult(
                            new OCPIResponse.Builder(request) {
                                StatusCode           = 2000,
                                StatusMessage        = "Invalid or blocked access token!",
                                HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                    HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                    AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                    AccessControlAllowHeaders  = [ "Authorization" ]
                                }
                            });

                    }

                    #endregion


                    var filters           = request.GetDateAndPaginationFilters();

                    var allSessions       = CommonAPI.//GetSessions(session => Request.AccessInfo.Value.Roles.Any(role => role.CountryCode == session.CountryCode &&
                                                      //                                                                  role.PartyId     == session.PartyId)).
                                                      GetSessions(session => CommonAPI.Parties.Any(partyData => partyData.Id.CountryCode == session.CountryCode &&
                                                                                                                partyData.Id.Party       == session.PartyId)).
                                                      ToArray();

                    var filteredSessions  = allSessions.Where(session => !filters.From.HasValue || session.LastUpdated >  filters.From.Value).
                                                        Where(session => !filters.To.  HasValue || session.LastUpdated <= filters.To.  Value).
                                                        ToArray();


                    var httpResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
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
                                                                                     : $"http://127.0.0.1:{CommonAPI.BaseAPI.HTTPBaseAPI.HTTPServer.TCPPort}")}{URLPathPrefix}/sessions{queryParameters}>; rel=\"next\"");

                    }

                    #endregion

                    return Task.FromResult(
                               new OCPIResponse.Builder(request) {
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

            CommonAPI.AddOCPIMethod(

                HTTPMethod.GET,
                URLPathPrefix + "sessions/{sessionId}",
                HTTPContentType.Application.JSON_UTF8,
                OCPIRequestHandler: request => {

                    #region Check access token

                    if (request.LocalAccessInfo.IsNot(Role.EMSP) == true ||
                        request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                    {

                        return Task.FromResult(
                            new OCPIResponse.Builder(request) {
                                StatusCode           = 2000,
                                StatusMessage        = "Invalid or blocked access token!",
                                HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                    HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                    AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                    AccessControlAllowHeaders  = [ "Authorization" ]
                                }
                            });

                    }

                    #endregion

                    #region Check session

                    if (!request.ParseSession(CommonAPI,
                                              //Request.AccessInfo.Value.Roles.Select(role => new Tuple<CountryCode, Party_Id>(role.CountryCode, role.PartyId)),
                                              CommonAPI.Parties.Select(partyData => new Tuple<CountryCode, Party_Id>(partyData.Id.CountryCode, partyData.Id.Party)),
                                              out var sessionId,
                                              out var session,
                                              out var ocpiResponseBuilder) ||
                         session is null)
                    {
                        return Task.FromResult(ocpiResponseBuilder!);
                    }

                    #endregion


                    return Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               StatusCode           = 1000,
                               StatusMessage        = "Hello world!",
                               Data                 = session.ToJSON(CustomSessionSerializer,
                                                                     CustomCDRTokenSerializer,
                                                                     CustomChargingPeriodSerializer,
                                                                     CustomCDRDimensionSerializer,
                                                                     CustomPriceSerializer),
                               HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
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

                HTTPMethod.OPTIONS,
                URLPathPrefix + "cdrs",
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
            CommonAPI.AddOCPIMethod(

                HTTPMethod.GET,
                URLPathPrefix + "cdrs",
                HTTPContentType.Application.JSON_UTF8,
                OCPIRequestHandler: request => {

                    #region Check access token

                    if (request.LocalAccessInfo.IsNot(Role.EMSP) == true ||
                        request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                    {

                        return Task.FromResult(
                            new OCPIResponse.Builder(request) {
                                StatusCode           = 2000,
                                StatusMessage        = "Invalid or blocked access token!",
                                HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                    HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                    AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                    AccessControlAllowHeaders  = [ "Authorization" ]
                                }
                            });

                    }

                    #endregion


                    var filters            = request.GetDateAndPaginationFilters();

                    var allCDRs            = CommonAPI.//GetCDRs(cdr => Request.AccessInfo.Value.Roles.Any(role => role.CountryCode == cdr.CountryCode &&
                                                       //                                                          role.PartyId     == cdr.PartyId)).
                                                       GetCDRs(cdr => CommonAPI.Parties.Any(partyData => partyData.Id.CountryCode == cdr.CountryCode &&
                                                                                                         partyData.Id.Party       == cdr.PartyId)).
                                                       ToArray();

                    var filteredCDRs       = allCDRs.Where(CDR => !filters.From.HasValue || CDR.LastUpdated >  filters.From.Value).
                                                     Where(CDR => !filters.To.  HasValue || CDR.LastUpdated <= filters.To.  Value).
                                                     ToArray();


                    var httpResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
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
                                                    : $"http://127.0.0.1:{CommonAPI.BaseAPI.HTTPBaseAPI.HTTPServer.TCPPort}")}{URLPathPrefix}/cdrs{queryParameters}>; rel=\"next\"");

                    }

                    #endregion

                    return Task.FromResult(
                               new OCPIResponse.Builder(request) {
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
                                                                                               CustomPriceLimitSerializer,
                                                                                               CustomTariffElementSerializer,
                                                                                               CustomPriceComponentSerializer,
                                                                                               CustomTaxAmountSerializer,
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

            CommonAPI.AddOCPIMethod(

                HTTPMethod.GET,
                URLPathPrefix + "cdrs/{CDRId}",
                HTTPContentType.Application.JSON_UTF8,
                OCPIRequestHandler: request => {

                    #region Check access token

                    if (request.LocalAccessInfo.IsNot(Role.EMSP) == true ||
                        request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                    {

                        return Task.FromResult(
                            new OCPIResponse.Builder(request) {
                                StatusCode           = 2000,
                                StatusMessage        = "Invalid or blocked access token!",
                                HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                    HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                    AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                    AccessControlAllowHeaders  = [ "Authorization" ]
                                }
                            });

                    }

                    #endregion

                    #region Check CDR

                    if (!request.ParseCDR(CommonAPI,
                                          //Request.AccessInfo.Value.Roles.Select(role => new Tuple<CountryCode, Party_Id>(role.CountryCode, role.PartyId)),
                                          CommonAPI.Parties.Select(partyData => new Tuple<CountryCode, Party_Id>(partyData.Id.CountryCode, partyData.Id.Party)),
                                          out var cdrId,
                                          out var cdr,
                                          out var ocpiResponseBuilder) ||
                         cdr is null)
                    {
                        return Task.FromResult(ocpiResponseBuilder!);
                    }

                    #endregion


                    return Task.FromResult(
                        new OCPIResponse.Builder(request) {
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
                                                                 CustomPriceLimitSerializer,
                                                                 CustomTariffElementSerializer,
                                                                 CustomPriceComponentSerializer,
                                                                 CustomTaxAmountSerializer,
                                                                 CustomTariffRestrictionsSerializer,
                                                                 CustomEnergyMixSerializer,
                                                                 CustomEnergySourceSerializer,
                                                                 CustomEnvironmentalImpactSerializer,
                                                                 CustomChargingPeriodSerializer,
                                                                 CustomCDRDimensionSerializer,
                                                                 CustomSignedDataSerializer,
                                                                 CustomSignedValueSerializer),
                               HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
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

            CommonAPI.AddOCPIMethod(

                HTTPMethod.GET,
                URLPathPrefix + "tokens/{country_code}/{party_id}",
                HTTPContentType.Application.JSON_UTF8,
                OCPIRequestHandler: request => {

                    #region Check access token

                    if (request.LocalAccessInfo.IsNot(Role.EMSP) == true ||
                        request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                    {

                        return Task.FromResult(
                            new OCPIResponse.Builder(request) {
                                StatusCode           = 2000,
                                StatusMessage        = "Invalid or blocked access token!",
                                HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                    HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                    AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                    AccessControlAllowHeaders  = [ "Authorization" ]
                                }
                            });

                    }

                    #endregion

                    #region Check party identification

                    if (!request.ParsePartyId(CommonAPI,
                                              out var partyId,
                                              out var ocpiResponseBuilder))
                    {
                        return Task.FromResult(ocpiResponseBuilder);
                    }

                    #endregion


                    var filters         = request.GetDateAndPaginationFilters();

                    var allTokens       = CommonAPI.GetTokenStatus(partyId.Value).ToArray();

                    var filteredTokens  = allTokens.Select(tokenStatus => tokenStatus.Token).
                                                    Where (token       => !filters.From.HasValue || token.LastUpdated >  filters.From.Value).
                                                    Where (token       => !filters.To.  HasValue || token.LastUpdated <= filters.To.  Value).
                                                    ToArray();


                    var httpResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
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
                                                                 : $"http://127.0.0.1:{CommonAPI.BaseAPI.HTTPBaseAPI.HTTPServer.TCPPort}")}{URLPathPrefix}/tokens/{partyId.Value.CountryCode}/{partyId.Value.Party}{queryParameters}>; rel=\"next\"");

                    }

                    #endregion

                    return Task.FromResult(
                               new OCPIResponse.Builder(request) {
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

            CommonAPI.AddOCPIMethod(

                HTTPMethod.DELETE,
                URLPathPrefix + "tokens/{country_code}/{party_id}",
                HTTPContentType.Application.JSON_UTF8,
                OCPIRequestHandler: async request => {

                    #region Check access token

                    if (request.LocalAccessInfo.IsNot(Role.EMSP) == true ||
                        request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                    {

                        return new OCPIResponse.Builder(request) {
                                   StatusCode           = 2000,
                                   StatusMessage        = "Invalid or blocked access token!",
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                       AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                   }
                               };

                    }

                    #endregion

                    #region Check party identification

                    if (!request.ParsePartyId(CommonAPI,
                                              out var partyId,
                                              out var ocpiResponseBuilder))
                    {
                        return ocpiResponseBuilder;
                    }

                    #endregion


                    await CommonAPI.RemoveAllTokens(partyId.Value);


                    return new OCPIResponse.Builder(request) {
                               StatusCode           = 1000,
                               StatusMessage        = "Hello world!",
                               HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET", "DELETE" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ]
                               }
                           };

                });

            #endregion

            #endregion

            #region ~/tokens/{country_code}/{party_id}/{tokenId}

            #region OPTIONS  ~/tokens/{country_code}/{party_id}/{tokenId}      [NonStandard]

            CommonAPI.AddOCPIMethod(

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

            CommonAPI.AddOCPIMethod(

                HTTPMethod.GET,
                URLPathPrefix + "tokens/{country_code}/{party_id}/{tokenId}",
                HTTPContentType.Application.JSON_UTF8,
                OCPIRequestLogger:   GetTokenRequest,
                OCPIResponseLogger:  GetTokenResponse,
                OCPIRequestHandler:  request => {

                    #region Check access token

                    if (request.LocalAccessInfo is null ||
                        request.LocalAccessInfo.IsNot(Role.EMSP) == true ||
                        request.LocalAccessInfo.Status != AccessStatus.ALLOWED)
                    {

                        return Task.FromResult(
                            new OCPIResponse.Builder(request) {
                                StatusCode           = 2000,
                                StatusMessage        = "Invalid or blocked access token!",
                                HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                    HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                    AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                    AccessControlAllowHeaders  = [ "Authorization" ]
                                }
                            });

                    }

                    #endregion

                    #region Check token

                    if (!request.ParseMandatoryToken(CommonAPI,
                                                     request.LocalAccessInfo.Roles.Select(role => new Tuple<CountryCode, Party_Id>(role.CountryCode, role.PartyId)),
                                                     out var countryCode,
                                                     out var partyId,
                                                     out var tokenId,
                                                     out var tokenStatus,
                                                     out var ocpiResponseBuilder))
                    {
                        return Task.FromResult(ocpiResponseBuilder);
                    }

                    #endregion


                    //ToDo: What exactly to do with this information?
                    var tokenType = request.QueryString.TryParseEnum<TokenType>("type") ?? TokenType.RFID;


                    return Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               StatusCode           = 1000,
                               StatusMessage        = "Hello world!",
                               Data                 = tokenStatus.Token.ToJSON(CustomTokenSerializer,
                                                                               CustomEnergyContractSerializer),
                            HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ],
                                   LastModified               = tokenStatus.Token.LastUpdated,
                                   ETag                       = tokenStatus.Token.ETag
                               }
                        });

                });

            #endregion

            #region PUT     ~/tokens/{country_code}/{party_id}/{tokenId}?type={type}

            CommonAPI.AddOCPIMethod(

                HTTPMethod.PUT,
                URLPathPrefix + "tokens/{country_code}/{party_id}/{tokenId}",
                HTTPContentType.Application.JSON_UTF8,
                OCPIRequestLogger:   PutTokenRequest,
                OCPIResponseLogger:  PutTokenResponse,
                OCPIRequestHandler:  async request => {

                    #region Check access token

                    if (request.LocalAccessInfo is null ||
                        request.LocalAccessInfo.IsNot(Role.EMSP) == true ||
                        request.LocalAccessInfo.Status != AccessStatus.ALLOWED)
                    {

                        return new OCPIResponse.Builder(request) {
                                   StatusCode           = 2000,
                                   StatusMessage        = "Invalid or blocked access token!",
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                       AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                   }
                               };

                    }

                    #endregion

                    #region Check token

                    if (!request.ParseTokenId(CommonAPI,
                                              out var countryCode,
                                              out var partyId,
                                              out var tokenId,
                                              out var ocpiResponseBuilder))
                    {
                        return ocpiResponseBuilder!;
                    }

                    #endregion

                    #region Parse new or updated token JSON

                    if (!request.TryParseJObjectRequestBody(out var tokenJSON, out ocpiResponseBuilder))
                        return ocpiResponseBuilder;

                    if (!Token.TryParse(tokenJSON,
                                        out var newOrUpdatedToken,
                                        out var errorResponse,
                                        countryCode,
                                        partyId,
                                        tokenId) ||
                         newOrUpdatedToken is null)
                    {

                        return new OCPIResponse.Builder(request) {
                               StatusCode           = 2001,
                               StatusMessage        = "Could not parse the given token JSON: " + errorResponse,
                               HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ]
                               }
                           };

                    }

                    #endregion


                    //ToDo: What exactly to do with this information?
                    var TokenType          = request.QueryString.TryParseEnum<TokenType>("type") ?? OCPI.TokenType.RFID;

                    var addOrUpdateResult  = await CommonAPI.AddOrUpdateToken(
                                                       newOrUpdatedToken,
                                                       AllowDowngrades: AllowDowngrades ?? request.QueryString.GetBoolean("forceDowngrade")
                                                   );


                    if (addOrUpdateResult.IsSuccessAndDataNotNull(out var data))
                    {

                        return new OCPIResponse.Builder(request) {
                                   StatusCode           = 1000,
                                   StatusMessage        = "Hello world!",
                                   Data                 = data.Token.ToJSON(CustomTokenSerializer,
                                                                            CustomEnergyContractSerializer),
                            HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = addOrUpdateResult.WasCreated == true
                                                                        ? HTTPStatusCode.Created
                                                                        : HTTPStatusCode.OK,
                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ],
                                       LastModified               = data.Token.LastUpdated,
                                       ETag                       = data.Token.ETag
                                   }
                               };

                    }

                    return new OCPIResponse.Builder(request) {
                               StatusCode           = 2000,
                               StatusMessage        = addOrUpdateResult.ErrorResponse,
                               Data                 = newOrUpdatedToken.ToJSON(CustomTokenSerializer,
                                                                               CustomEnergyContractSerializer),
                        HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ],
                                   LastModified               = newOrUpdatedToken.LastUpdated,
                                   ETag                       = newOrUpdatedToken.ETag
                               }
                           };

                });

            #endregion

            #region PATCH   ~/tokens/{country_code}/{party_id}/{tokenId}?type={type}

            CommonAPI.AddOCPIMethod(

                HTTPMethod.PATCH,
                URLPathPrefix + "tokens/{country_code}/{party_id}/{tokenId}",
                HTTPContentType.Application.JSON_UTF8,
                OCPIRequestLogger:   PatchTokenRequest,
                OCPIResponseLogger:  PatchTokenResponse,
                OCPIRequestHandler:  async request => {

                    #region Check access token

                    if (request.LocalAccessInfo is null ||
                        request.LocalAccessInfo.IsNot(Role.EMSP) == true ||
                        request.LocalAccessInfo.Status != AccessStatus.ALLOWED)
                    {

                        return new OCPIResponse.Builder(request) {
                                   StatusCode           = 2000,
                                   StatusMessage        = "Invalid or blocked access token!",
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                       AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                   }
                               };

                    }

                    #endregion

                    #region Check token

                    if (!request.ParseMandatoryToken(CommonAPI,
                                                     request.LocalAccessInfo.Roles.Select(role => new Tuple<CountryCode, Party_Id>(role.CountryCode, role.PartyId)),
                                                     out var countryCode,
                                                     out var partyId,
                                                     out var tokenId,
                                                     out var existingTokenStatus,
                                                     out var ocpiResponseBuilder))
                    {
                        return ocpiResponseBuilder;
                    }

                    #endregion

                    #region Parse token JSON patch

                    if (!request.TryParseJObjectRequestBody(out var tokenPatch, out ocpiResponseBuilder))
                        return ocpiResponseBuilder;

                    #endregion


                    //ToDo: What exactly to do with this information?
                    var tokenType     = request.QueryString.TryParseEnum<TokenType>("type") ?? TokenType.RFID;


                    //ToDo: Validation-Checks for PATCHes (E-Tag, Timestamp, ...)
                    var patchedToken  = await CommonAPI.TryPatchToken(
                                                  Party_Idv3.From(
                                                      existingTokenStatus.Token.CountryCode,
                                                      existingTokenStatus.Token.PartyId
                                                  ),
                                                  existingTokenStatus.Token.Id,
                                                  tokenPatch,
                                                  AllowDowngrades ?? request.QueryString.GetBoolean("forceDowngrade")
                                              );


                    if (patchedToken.IsSuccessAndDataNotNull(out var data))
                        return new OCPIResponse.Builder(request) {
                                       StatusCode           = 1000,
                                       StatusMessage        = "Hello world!",
                                       Data                 = data.ToJSON(CustomTokenSerializer,
                                                                          CustomEnergyContractSerializer),
                            HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                           HTTPStatusCode             = HTTPStatusCode.OK,
                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                           AccessControlAllowHeaders  = [ "Authorization" ],
                                           LastModified               = data.LastUpdated,
                                           ETag                       = data.ETag
                                       }
                                   };

                    return new OCPIResponse.Builder(request) {
                                   StatusCode           = 2000,
                                   StatusMessage        = patchedToken.ErrorResponse,
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                   }
                               };

                });

            #endregion

            #region DELETE  ~/tokens/{country_code}/{party_id}/{tokenId}      [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPMethod.DELETE,
                URLPathPrefix + "tokens/{country_code}/{party_id}/{tokenId}",
                HTTPContentType.Application.JSON_UTF8,
                OCPIRequestLogger:   DeleteTokenRequest,
                OCPIResponseLogger:  DeleteTokenResponse,
                OCPIRequestHandler:   async request => {

                    #region Check access token

                    if (request.LocalAccessInfo.IsNot(Role.EMSP) == true ||
                        request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                    {

                        return new OCPIResponse.Builder(request) {
                                   StatusCode           = 2000,
                                   StatusMessage        = "Invalid or blocked access token!",
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                       AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                   }
                               };

                    }

                    #endregion

                    #region Check token (status)

                    if (!request.ParseMandatoryToken(CommonAPI,
                                                     request.LocalAccessInfo.Roles.Select(role => new Tuple<CountryCode, Party_Id>(role.CountryCode, role.PartyId)),
                                                     out var countryCode,
                                                     out var partyId,
                                                     out var tokenId,
                                                     out var existingTokenStatus,
                                                     out var ocpiResponseBuilder))
                    {
                        return ocpiResponseBuilder;
                    }

                    #endregion


                    //ToDo: What exactly to do with this information?
                    var tokenType = request.QueryString.TryParseEnum<TokenType>("type") ?? TokenType.RFID;

                    var result = await CommonAPI.RemoveToken(existingTokenStatus.Token);

                    if (result.IsSuccessAndDataNotNull(out var data))
                        return new OCPIResponse.Builder(request) {
                                   StatusCode           = 1000,
                                   StatusMessage        = "Hello world!",
                                   Data                 = data.ToJSON(CustomTokenSerializer,
                                                                      CustomEnergyContractSerializer),
                            HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                       //LastModified               = Timestamp.Now.ToISO8601()
                                   }
                               };

                    else
                        return new OCPIResponse.Builder(request) {
                                   StatusCode           = 2000,
                                   StatusMessage        = "Hello world!",
                                   Data                 = existingTokenStatus.Token.ToJSON(CustomTokenSerializer,
                                                                                           CustomEnergyContractSerializer),
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                       //LastModified               = Timestamp.Now.ToISO8601()
                                   }
                               };

                });

            #endregion

            #endregion


            #region ~/bookings

            #region OPTIONS  ~/bookings

            CommonAPI.AddOCPIMethod(

                HTTPMethod.OPTIONS,
                URLPathPrefix + "bookings",
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

            #region GET      ~/bookings

            // https://example.com/ocpi/2.2/cpo/bookings/?date_from=2019-01-28T12:00:00&date_to=2019-01-29T12:00:00&offset=50&limit=100
            CommonAPI.AddOCPIMethod(

                HTTPMethod.GET,
                URLPathPrefix + "bookings",
                HTTPContentType.Application.JSON_UTF8,
                OCPIRequestHandler: request => {

                    #region Check access token

                    if (request.LocalAccessInfo.IsNot(Role.EMSP) == true ||
                        request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                    {

                        return Task.FromResult(
                            new OCPIResponse.Builder(request) {
                                StatusCode           = 2000,
                                StatusMessage        = "Invalid or blocked access token!",
                                HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                    HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                    AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                    AccessControlAllowHeaders  = [ "Authorization" ]
                                }
                            });

                    }

                    #endregion


                    var filters              = request.GetDateAndPaginationFilters();

                    var allBookings          = CommonAPI.//Getbookings(booking => Request.AccessInfo.Value.Roles.Any(role => role.CountryCode == booking.CountryCode &&
                                                     //                                                          role.PartyId     == booking.PartyId)).
                                                     GetBookings(booking => CommonAPI.Parties.Any(partyData => partyData.Id.CountryCode == booking.CountryCode &&
                                                                                                  partyData.Id.Party       == booking.PartyId)).
                                                     ToArray();

                    var filteredBookings     = allBookings.Where(booking => !filters.From.HasValue || booking.LastUpdated >  filters.From.Value).
                                                   Where(booking => !filters.To.  HasValue || booking.LastUpdated <= filters.To.  Value).
                                                   ToArray();


                    var httpResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                                   Server                     = DefaultHTTPServerName,
                                                   Date                       = Timestamp.Now,
                                                   AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                                   AccessControlAllowHeaders  = [ "Authorization" ]
                                               }.

                                               // The overall number of bookings
                                               Set("X-Total-Count",  allBookings.Length).

                                               // The maximum number of bookings that the server WILL return within a single request
                                               Set("X-Limit",        allBookings.Length);


                    #region When the limit query parameter was set & this is not the last pagination page...

                    if (filters.Limit.HasValue &&
                        allBookings.ULongLength() > ((filters.Offset ?? 0) + (filters.Limit ?? 0)))
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
                        //   - Link: <https://www.server.com/ocpi/cpo/2.0/bookings/?offset=150&limit=50>; rel="next"
                        httpResponseBuilder.Set("Link", $"<{(ExternalDNSName.IsNotNullOrEmpty()
                                                    ? $"https://{ExternalDNSName}"
                                                    : $"http://127.0.0.1:{CommonAPI.BaseAPI.HTTPBaseAPI.HTTPServer.TCPPort}")}{URLPathPrefix}/bookings{queryParameters}>; rel=\"next\"");

                    }

                    #endregion

                    return Task.FromResult(
                               new OCPIResponse.Builder(request) {
                                   StatusCode           = 1000,
                                   StatusMessage        = "Hello world!",
                                   HTTPResponseBuilder  = httpResponseBuilder,
                                   Data                 = new JArray(
                                                              filteredBookings.
                                                              OrderBy       (booking => booking.Created).
                                                              SkipTakeFilter(filters.Offset,
                                                                             filters.Limit).
                                                              Select        (booking => booking.ToJSON(
                                                                                               //CustombookingSerializer,
                                                                                               //CustombookingTokenSerializer,
                                                                                               //CustombookingLocationSerializer,
                                                                                               //CustomEVSEEnergyMeterSerializer,
                                                                                               //CustomTransparencySoftwareSerializer,
                                                                                               //CustomTariffSerializer,
                                                                                               //CustomDisplayTextSerializer,
                                                                                               //CustomPriceSerializer,
                                                                                               //CustomPriceLimitSerializer,
                                                                                               //CustomTariffElementSerializer,
                                                                                               //CustomPriceComponentSerializer,
                                                                                               //CustomTaxAmountSerializer,
                                                                                               //CustomTariffRestrictionsSerializer,
                                                                                               //CustomEnergyMixSerializer,
                                                                                               //CustomEnergySourceSerializer,
                                                                                               //CustomEnvironmentalImpactSerializer,
                                                                                               //CustomChargingPeriodSerializer,
                                                                                               //CustombookingDimensionSerializer,
                                                                                               //CustomSignedDataSerializer,
                                                                                               //CustomSignedValueSerializer
                                                                                               ))
                                                          )
                               }
                           );

                });

            #endregion

            #region POST     ~/bookings

            CommonAPI.AddOCPIMethod(

                HTTPMethod.POST,
                URLPathPrefix + "bookings",
                HTTPContentType.Application.JSON_UTF8,
                OCPIRequestHandler: request => {

                    #region Check access token

                    if (request.LocalAccessInfo.IsNot(Role.EMSP) == true ||
                        request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                    {

                        return Task.FromResult(
                            new OCPIResponse.Builder(request) {
                                StatusCode           = 2000,
                                StatusMessage        = "Invalid or blocked access token!",
                                HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                    HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                    AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                    AccessControlAllowHeaders  = [ "Authorization" ]
                                }
                            });

                    }

                    #endregion


                    return Task.FromResult(
                               new OCPIResponse.Builder(request) {
                                   StatusCode           = 1000,
                                   StatusMessage        = "Hello world!",
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                                              HTTPStatusCode             = HTTPStatusCode.OK,
                                                              Server                     = DefaultHTTPServerName,
                                                              Date                       = Timestamp.Now,
                                                              AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                                              AccessControlAllowHeaders  = [ "Authorization" ]
                                                          },
                                   Data                 = new JArray(
                                                              
                                                          )
                               }
                           );

                });

            #endregion

            #endregion

            #region ~/bookings/booking_locations

            #region OPTIONS  ~/bookings/booking_locations
            #endregion

            #region GET      ~/bookings/booking_locations
            #endregion

            #endregion

            #region ~/bookings/booking_locations/{booking_location_id}

            #endregion

            #region ~/bookings/booking_locations/{booking_location_id}/{calendar_id}

            #region OPTIONS  ~/bookings/booking_locations/{booking_location_id}/{calendar_id}
            #endregion

            #region GET      ~/bookings/booking_locations/{booking_location_id}/{calendar_id}
            #endregion

            #endregion


            // Commands

            #region ~/commands/RESERVE_NOW

            #region OPTIONS  ~/commands/RESERVE_NOW

            CommonAPI.AddOCPIMethod(

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

            CommonAPI.AddOCPIMethod(

                HTTPMethod.POST,
                URLPathPrefix + "commands/RESERVE_NOW",
                HTTPContentType.Application.JSON_UTF8,
                OCPIRequestLogger:   ReserveNowRequest,
                OCPIResponseLogger:  ReserveNowResponse,
                OCPIRequestHandler:  async request => {

                    #region Check access token

                    if (request.RemoteParty is null ||
                        request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                    {

                        return new OCPIResponse.Builder(request) {
                                   StatusCode           = 2000,
                                   StatusMessage        = "Invalid or blocked access token!",
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                       AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                   }
                               };

                    }

                    #endregion

                    #region Parse ReserveNow command JSON

                    if (!request.TryParseJObjectRequestBody(out var reserveNowJSON, out var ocpiResponse))
                        return ocpiResponse;

                    if (!ReserveNowCommand.TryParse(reserveNowJSON,
                                                    out var reserveNowCommand,
                                                    out var errorResponse) ||
                         reserveNowCommand is null)
                    {

                        return new OCPIResponse.Builder(request) {
                                   StatusCode           = 2001,
                                   StatusMessage        = "Could not parse the given 'RESERVE_NOW' command JSON: " + errorResponse,
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                       AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                   }
                               };

                    }

                    #endregion


                    CommandResponse? commandResponse = null;

                    if (OnReserveNowCommand is not null)
                        commandResponse = await OnReserveNowCommand.Invoke(EMSP_Id.From(request.RemoteParty.Id),
                                                                           reserveNowCommand);

                    commandResponse ??= new CommandResponse(
                                            reserveNowCommand,
                                            CommandResponseTypes.NOT_SUPPORTED,
                                            Timeout: TimeSpan.FromSeconds(15),
                                            Message: [ new DisplayText(Languages.en, "Not supported!") ]
                                        );


                    return new OCPIResponse.Builder(request) {
                               StatusCode           = 1000,
                               StatusMessage        = "Hello world!",
                               Data                 = commandResponse.ToJSON(),
                               HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ]
                               }
                           };

                });

            #endregion

            #endregion

            #region ~/commands/CANCEL_RESERVATION

            #region OPTIONS  ~/commands/CANCEL_RESERVATION

            CommonAPI.AddOCPIMethod(

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

            CommonAPI.AddOCPIMethod(

                HTTPMethod.POST,
                URLPathPrefix + "commands/CANCEL_RESERVATION",
                HTTPContentType.Application.JSON_UTF8,
                OCPIRequestLogger:   CancelReservationRequest,
                OCPIResponseLogger:  CancelReservationResponse,
                OCPIRequestHandler:  async request => {

                    #region Check access token

                    if (request.RemoteParty is null ||
                        request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                    {

                        return new OCPIResponse.Builder(request) {
                                   StatusCode           = 2000,
                                   StatusMessage        = "Invalid or blocked access token!",
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                       AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                   }
                               };

                    }

                    #endregion

                    #region Parse CancelReservation command JSON

                    if (!request.TryParseJObjectRequestBody(out var cancelReservationJSON, out var ocpiResponse))
                        return ocpiResponse;

                    if (!CancelReservationCommand.TryParse(cancelReservationJSON,
                                                           out var cancelReservationCommand,
                                                           out var errorResponse) ||
                         cancelReservationCommand is null)
                    {

                        return new OCPIResponse.Builder(request) {
                                   StatusCode           = 2001,
                                   StatusMessage        = "Could not parse the given 'CANCEL_RESERVATION' command JSON: " + errorResponse,
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                       AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                   }
                               };

                    }

                    #endregion


                    CommandResponse? commandResponse = null;

                    if (OnCancelReservationCommand is not null)
                        commandResponse = await OnCancelReservationCommand.Invoke(EMSP_Id.From(request.RemoteParty.Id),
                                                                                  cancelReservationCommand);

                    commandResponse ??= new CommandResponse(
                                            cancelReservationCommand,
                                            CommandResponseTypes.NOT_SUPPORTED,
                                            Timeout: TimeSpan.FromSeconds(15),
                                            Message: [ new DisplayText(Languages.en, "Not supported!") ]
                                        );


                    return new OCPIResponse.Builder(request) {
                               StatusCode           = 1000,
                               StatusMessage        = "Hello world!",
                               Data                 = commandResponse.ToJSON(),
                               HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ]
                               }
                           };

                });

            #endregion

            #endregion

            #region ~/commands/START_SESSION

            #region OPTIONS  ~/commands/START_SESSION

            CommonAPI.AddOCPIMethod(

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

            CommonAPI.AddOCPIMethod(

                HTTPMethod.POST,
                URLPathPrefix + "commands/START_SESSION",
                HTTPContentType.Application.JSON_UTF8,
                OCPIRequestLogger:   StartSessionRequest,
                OCPIResponseLogger:  StartSessionResponse,
                OCPIRequestHandler:  async request => {

                    #region Check access token

                    if (request.RemoteParty is null ||
                        request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                    {

                        return new OCPIResponse.Builder(request) {
                                   StatusCode           = 2000,
                                   StatusMessage        = "Invalid or blocked access token!",
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                       AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                   }
                               };

                    }

                    #endregion

                    #region Parse StartSession command JSON

                    if (!request.TryParseJObjectRequestBody(out var startSessionJSON, out var ocpiResponse))
                        return ocpiResponse;

                    if (!StartSessionCommand.TryParse(startSessionJSON,
                                                      out var startSessionCommand,
                                                      out var errorResponse) ||
                         startSessionCommand is null)
                    {

                        return new OCPIResponse.Builder(request) {
                                   StatusCode           = 2001,
                                   StatusMessage        = "Could not parse the given 'START_SESSION' command JSON: " + errorResponse,
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                       AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                   }
                               };

                    }

                    #endregion


                    CommandResponse? commandResponse = null;

                    if (OnStartSessionCommand is not null)
                        commandResponse = await OnStartSessionCommand.Invoke(EMSP_Id.From(request.RemoteParty.Id),
                                                                             startSessionCommand);

                    commandResponse ??= new CommandResponse(
                                            startSessionCommand,
                                            CommandResponseTypes.NOT_SUPPORTED,
                                            Timeout: TimeSpan.FromSeconds(15),
                                            Message: [ new DisplayText(Languages.en, "Not supported!") ]
                                        );


                    return new OCPIResponse.Builder(request) {
                               StatusCode           = 1000,
                               StatusMessage        = "Hello world!",
                               Data                 = commandResponse.ToJSON(),
                               HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ]
                               }
                           };

                });

            #endregion

            #endregion

            #region ~/commands/STOP_SESSION

            #region OPTIONS  ~/commands/STOP_SESSION

            CommonAPI.AddOCPIMethod(

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

            CommonAPI.AddOCPIMethod(

                HTTPMethod.POST,
                URLPathPrefix + "commands/STOP_SESSION",
                HTTPContentType.Application.JSON_UTF8,
                OCPIRequestLogger:   StopSessionRequest,
                OCPIResponseLogger:  StopSessionResponse,
                OCPIRequestHandler:  async request => {

                    #region Check access token

                    if (request.RemoteParty is null ||
                        request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                    {

                        return new OCPIResponse.Builder(request) {
                                   StatusCode           = 2000,
                                   StatusMessage        = "Invalid or blocked access token!",
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                       AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                   }
                               };

                    }

                    #endregion

                    #region Parse StopSession command JSON

                    if (!request.TryParseJObjectRequestBody(out var stopSessionJSON, out var ocpiResponse))
                        return ocpiResponse;

                    if (!StopSessionCommand.TryParse(stopSessionJSON,
                                                      out var stopSessionCommand,
                                                      out var errorResponse) ||
                         stopSessionCommand is null)
                    {

                        return new OCPIResponse.Builder(request) {
                                   StatusCode           = 2001,
                                   StatusMessage        = "Could not parse the given 'STOP_SESSION' command JSON: " + errorResponse,
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                       AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                   }
                               };

                    }

                    #endregion


                    CommandResponse? commandResponse = null;

                    if (OnStopSessionCommand is not null)
                        commandResponse = await OnStopSessionCommand.Invoke(EMSP_Id.From(request.RemoteParty.Id),
                                                                            stopSessionCommand);

                    commandResponse ??= new CommandResponse(
                                            stopSessionCommand,
                                            CommandResponseTypes.NOT_SUPPORTED,
                                            Timeout: TimeSpan.FromSeconds(15),
                                            Message: [ new DisplayText(Languages.en, "Not supported!") ]
                                        );


                    return new OCPIResponse.Builder(request) {
                               StatusCode           = 1000,
                               StatusMessage        = "Hello world!",
                               Data                 = commandResponse.ToJSON(),
                               HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ]
                               }
                           };

                });

            #endregion

            #endregion

            #region ~/commands/UNLOCK_CONNECTOR

            #region OPTIONS  ~/commands/UNLOCK_CONNECTOR

            CommonAPI.AddOCPIMethod(

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

            CommonAPI.AddOCPIMethod(

                HTTPMethod.POST,
                URLPathPrefix + "commands/UNLOCK_CONNECTOR",
                HTTPContentType.Application.JSON_UTF8,
                OCPIRequestLogger:   UnlockConnectorRequest,
                OCPIResponseLogger:  UnlockConnectorResponse,
                OCPIRequestHandler:  async request => {

                    #region Check access token

                    if (request.RemoteParty is null ||
                        request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                    {

                        return new OCPIResponse.Builder(request) {
                                   StatusCode           = 2000,
                                   StatusMessage        = "Invalid or blocked access token!",
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                       AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                   }
                               };

                    }

                    #endregion

                    #region Parse UnlockConnector command JSON

                    if (!request.TryParseJObjectRequestBody(out var unlockConnectorJSON, out var ocpiResponse))
                        return ocpiResponse;

                    if (!UnlockConnectorCommand.TryParse(unlockConnectorJSON,
                                                         out var unlockConnectorCommand,
                                                         out var errorResponse) ||
                         unlockConnectorCommand is null)
                    {

                        return new OCPIResponse.Builder(request) {
                                   StatusCode           = 2001,
                                   StatusMessage        = "Could not parse the given 'UNLOCK_CONNECTOR' command JSON: " + errorResponse,
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                       AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                   }
                               };

                    }

                    #endregion


                    CommandResponse? commandResponse = null;

                    if (OnUnlockConnectorCommand is not null)
                        commandResponse = await OnUnlockConnectorCommand.Invoke(EMSP_Id.From(request.RemoteParty.Id),
                                                                                unlockConnectorCommand);

                    commandResponse ??= new CommandResponse(
                                            unlockConnectorCommand,
                                            CommandResponseTypes.NOT_SUPPORTED,
                                            Timeout: TimeSpan.FromSeconds(15),
                                            Message: [ new DisplayText(Languages.en, "Not supported!") ]
                                        );


                    return new OCPIResponse.Builder(request) {
                               StatusCode           = 1000,
                               StatusMessage        = "Hello world!",
                               Data                 = commandResponse.ToJSON(),
                               HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ]
                               }
                           };

                });

            #endregion

            #endregion



            // Payment Terminals

            #region ~/payments/terminals/{terminalId}

            #region OPTIONS  ~/payments/terminals/{terminalId}

            CommonAPI.AddOCPIMethod(

                HTTPMethod.OPTIONS,
                URLPathPrefix + "payments/terminals/{terminalId}",
                OCPIRequestHandler: request =>

                    Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               HTTPResponseBuilder = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   Allow                      = [ HTTPMethod.OPTIONS, HTTPMethod.GET ],
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ]
                               }
                        })

            );

            #endregion

            #region GET      ~/payments/terminals/{terminalId}

            CommonAPI.AddOCPIMethod(

                HTTPMethod.GET,
                URLPathPrefix + "payments/terminals/{terminalId}",
                HTTPContentType.Application.JSON_UTF8,
                OCPIRequestLogger:   GetTerminalRequest,
                OCPIResponseLogger:  GetTerminalResponse,
                OCPIRequestHandler:  request => {

                    #region Check access token

                    if (request.LocalAccessInfo.IsNot(Role.EMSP) == true ||
                        request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                    {

                        return Task.FromResult(
                            new OCPIResponse.Builder(request) {
                                StatusCode           = 2000,
                                StatusMessage        = "Invalid or blocked access token!",
                                HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                    HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                    AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                    AccessControlAllowHeaders  = [ "Authorization" ]
                                }
                            });

                    }

                    #endregion

                    #region Check terminal

                    if (!request.ParsePaymentTerminal(CommonAPI,
                                               //Request.AccessInfo.Value.Roles.Select(role => new Tuple<CountryCode, Party_Id>(role.CountryCode, role.PartyId)),
                                               CommonAPI.Parties.Select(partyData => new Tuple<CountryCode, Party_Id>(partyData.Id.CountryCode, partyData.Id.Party)),
                                               out var terminalId,
                                               out var terminal,
                                               out var ocpiResponseBuilder,
                                               FailOnMissingTerminal: true) ||
                         terminal is null)
                    {
                        return Task.FromResult(ocpiResponseBuilder!);
                    }

                    #endregion


                    return Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               StatusCode           = 1000,
                               StatusMessage        = "Hello world!",
                               Data                 = terminal.ToJSON(true,
                                                                      true,
                                                                      CustomTerminalSerializer,
                                                                      CustomDisplayTextSerializer,
                                                                      CustomImageSerializer),
                               HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ],
                                   LastModified               = terminal.LastUpdated,
                                   ETag                       = terminal.ETag
                               }
                        });

                });

            #endregion

            #region POST     ~/payments/terminals/{terminalId}

            CommonAPI.AddOCPIMethod(

                HTTPMethod.POST,
                URLPathPrefix + "payments/terminals/{terminalId}",
                HTTPContentType.Application.JSON_UTF8,
                OCPIRequestLogger:   PostTerminalRequest,
                OCPIResponseLogger:  PostTerminalResponse,
                OCPIRequestHandler:  async Request => {

                    #region Check access token

                    if (Request.LocalAccessInfo.IsNot(Role.CPO) ||
                        Request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                    {

                        return new OCPIResponse.Builder(Request) {
                                   StatusCode           = 2000,
                                   StatusMessage        = "Invalid or blocked access token!",
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PATCH" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                   }
                               };

                    }

                    #endregion

                    #region Check existing location

                    if (!Request.ParsePaymentTerminal(CommonAPI,
                                               out var terminalId,
                                               out var existingTerminal,
                                               out var ocpiResponseBuilder,
                                               FailOnMissingTerminal: false))
                    {
                        return ocpiResponseBuilder;
                    }

                    #endregion

                    #region Parse new or updated location JSON

                    if (!Request.TryParseJObjectRequestBody(out var locationJSON, out ocpiResponseBuilder))
                        return ocpiResponseBuilder;

                    if (!Terminal.TryParse(locationJSON,
                                           out var newOrUpdatedTerminal,
                                           out var errorResponse,
                                           terminalId))
                    {

                        return new OCPIResponse.Builder(Request) {
                                   StatusCode           = 2001,
                                   StatusMessage        = "Could not parse the given location JSON: " + errorResponse,
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PATCH" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                   }
                               };

                    }

                    #endregion


                    var addOrUpdateResult = await CommonAPI.AddOrUpdatePaymentTerminal(
                                                      newOrUpdatedTerminal,
                                                      AllowDowngrades ?? Request.QueryString.GetBoolean("forceDowngrade"),
                                                      false, //SkipNotifications
                                                      Request.HTTPRequest.EventTrackingId,
                                                      CurrentUserId: null
                                                  );

                    if (addOrUpdateResult.IsSuccess &&
                        addOrUpdateResult.Data is not null)
                    {

                        return new OCPIResponse.Builder(Request) {
                                   StatusCode           = 1000,
                                   StatusMessage        = "Hello world!",
                                   Data                 = addOrUpdateResult.Data.ToJSON(true,
                                                                                        true,
                                                                                        CustomTerminalSerializer,
                                                                                        CustomDisplayTextSerializer,
                                                                                        CustomImageSerializer),
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                       HTTPStatusCode             = addOrUpdateResult.WasCreated == true
                                                                        ? HTTPStatusCode.Created
                                                                        : HTTPStatusCode.OK,
                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PATCH" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ],
                                       LastModified               = addOrUpdateResult.Data.LastUpdated,
                                       ETag                       = addOrUpdateResult.Data.ETag
                                   }
                               };

                    }

                    return new OCPIResponse.Builder(Request) {
                               StatusCode           = 2000,
                               StatusMessage        = addOrUpdateResult.ErrorResponse,
                               Data                 = newOrUpdatedTerminal.ToJSON(true,
                                                                                  true,
                                                                                  CustomTerminalSerializer,
                                                                                  CustomDisplayTextSerializer,
                                                                                  CustomImageSerializer),
                        HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PATCH" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ]
                               }
                           };

                }

            );

            #endregion

            #endregion

            #region ~/payments/financial-advice-confirmations

            #region OPTIONS  ~/payments/financial-advice-confirmations

            CommonAPI.AddOCPIMethod(

                HTTPMethod.OPTIONS,
                URLPathPrefix + "payments/financial-advice-confirmations",
                OCPIRequestHandler: request =>

                    Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               HTTPResponseBuilder = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   Allow                      = [ HTTPMethod.OPTIONS, HTTPMethod.POST ],
                                   AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ]
                               }
                        })

            );

            #endregion

            #region POST     ~/payments/financial-advice-confirmations


            #endregion

            #endregion

            #region ~/payments/financial-advice-confirmations/{financial_advice_confirmation_id}

            #region OPTIONS  ~/payments/financial-advice-confirmations/{financial_advice_confirmation_id}

            CommonAPI.AddOCPIMethod(

                HTTPMethod.OPTIONS,
                URLPathPrefix + "payments/financial-advice-confirmations/{financial_advice_confirmation_id}",
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

            #region GET      ~/payments/financial-advice-confirmations/{financial_advice_confirmation_id}


            #endregion

            #endregion

        }

        #endregion


    }

}
