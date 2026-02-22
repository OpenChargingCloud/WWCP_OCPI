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

using System.Collections.Concurrent;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.HTTPTest;

using cloud.charging.open.protocols.OCPI;
using cloud.charging.open.protocols.OCPIv2_2_1.CPO.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2_1
{

    /// <summary>
    /// The HTTP API for charge point operators.
    /// EMSPs will connect to this API.
    /// </summary>
    public class CPO_HTTPAPI : AHTTPExtAPIXExtension2<CommonAPI, HTTPExtAPIX>
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
        public CommonAPI            CommonAPI
            => HTTPBaseAPI;

        /// <summary>
        /// (Dis-)allow PUTting of object having an earlier 'LastUpdated'-timestamp then already existing objects.
        /// OCPI v2.2.x does not define any behaviour for this.
        /// </summary>
        public Boolean?             AllowDowngrades    { get; }

        /// <summary>
        /// The CPO HTTP API logger.
        /// </summary>
        public CPO_HTTPAPI_Logger?  HTTPLogger         { get; set; }


        public Action<CPO2EMSP_HTTPClient>?  DefaultCPO2EMSP_HTTPClientConfigurator    { get; set; }

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
        public CustomJObjectSerializerDelegate<CommandResponse>?             CustomCommandResponseSerializer               { get; set; }


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

        public HTTP_Events  HTTPEvents    { get; } = new HTTP_Events();

        public class HTTP_Events
        {

            #region Location(s)

            #region (protected internal) GetLocationsHTTPRequest  (Request)

            /// <summary>
            /// An event sent whenever a GET locations HTTP request was received.
            /// </summary>
            public OCPIRequestLogEvent OnGetLocationsHTTPRequest = new();

            /// <summary>
            /// An event sent whenever a GET locations HTTP request was received.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The CPO HTTP API.</param>
            /// <param name="Request">The OCPI request.</param>
            protected internal Task GetLocationsHTTPRequest(DateTimeOffset     Timestamp,
                                                            HTTPAPIX           API,
                                                            OCPIRequest        Request,
                                                            CancellationToken  CancellationToken)

                => OnGetLocationsHTTPRequest.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       CancellationToken
                   );

            #endregion

            #region (protected internal) GetLocationsHTTPResponse (Response)

            /// <summary>
            /// An event sent whenever a GET locations HTTP response was sent.
            /// </summary>
            public OCPIResponseLogEvent OnGetLocationsHTTPResponse = new();

            /// <summary>
            /// An event sent whenever a GET locations HTTP response was sent.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The CPO HTTP API.</param>
            /// <param name="Request">The OCPI request.</param>
            /// <param name="Response">The OCPI response.</param>
            protected internal Task GetLocationsHTTPResponse(DateTimeOffset     Timestamp,
                                                             HTTPAPIX           API,
                                                             OCPIRequest        Request,
                                                             OCPIResponse       Response,
                                                             CancellationToken  CancellationToken)

                => OnGetLocationsHTTPResponse.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       Response,
                       CancellationToken
                   );

            #endregion


            #region (protected internal) GetLocationHTTPRequest   (Request)

            /// <summary>
            /// An event sent whenever a GET location HTTP request was received.
            /// </summary>
            public OCPIRequestLogEvent OnGetLocationHTTPRequest = new();

            /// <summary>
            /// An event sent whenever a GET location HTTP request was received.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The CPO HTTP API.</param>
            /// <param name="Request">The OCPI request.</param>
            protected internal Task GetLocationHTTPRequest(DateTimeOffset     Timestamp,
                                                           HTTPAPIX           API,
                                                           OCPIRequest        Request,
                                                           CancellationToken  CancellationToken)

                => OnGetLocationHTTPRequest.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       CancellationToken
                   );

            #endregion

            #region (protected internal) GetLocationHTTPResponse  (Response)

            /// <summary>
            /// An event sent whenever a GET location HTTP response was sent.
            /// </summary>
            public OCPIResponseLogEvent OnGetLocationHTTPResponse = new();

            /// <summary>
            /// An event sent whenever a GET location HTTP response was sent.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The CPO HTTP API.</param>
            /// <param name="Request">The OCPI request.</param>
            /// <param name="Response">The OCPI response.</param>
            protected internal Task GetLocationHTTPResponse(DateTimeOffset     Timestamp,
                                                            HTTPAPIX           API,
                                                            OCPIRequest        Request,
                                                            OCPIResponse       Response,
                                                            CancellationToken  CancellationToken)

                => OnGetLocationHTTPResponse.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       Response,
                       CancellationToken
                   );

            #endregion

            #endregion

            #region EVSE

            #region (protected internal) GetEVSEHTTPRequest  (Request)

            /// <summary>
            /// An event sent whenever a GET EVSE HTTP request was received.
            /// </summary>
            public OCPIRequestLogEvent OnGetEVSEHTTPRequest = new();

            /// <summary>
            /// An event sent whenever a GET EVSE HTTP request was received.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The CPO HTTP API.</param>
            /// <param name="Request">The OCPI request.</param>
            protected internal Task GetEVSEHTTPRequest(DateTimeOffset     Timestamp,
                                                       HTTPAPIX           API,
                                                       OCPIRequest        Request,
                                                       CancellationToken  CancellationToken)

                => OnGetEVSEHTTPRequest.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       CancellationToken
                   );

            #endregion

            #region (protected internal) GetEVSEHTTPResponse (Response)

            /// <summary>
            /// An event sent whenever a GET EVSE HTTP response was sent.
            /// </summary>
            public OCPIResponseLogEvent OnGetEVSEHTTPResponse = new();

            /// <summary>
            /// An event sent whenever a GET EVSE HTTP response was sent.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The CPO HTTP API.</param>
            /// <param name="Request">The OCPI request.</param>
            /// <param name="Response">The OCPI response.</param>
            protected internal Task GetEVSEHTTPResponse(DateTimeOffset     Timestamp,
                                                        HTTPAPIX           API,
                                                        OCPIRequest        Request,
                                                        OCPIResponse       Response,
                                                        CancellationToken  CancellationToken)

                => OnGetEVSEHTTPResponse.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       Response,
                       CancellationToken
                   );

            #endregion

            #endregion

            #region Connector

            #region (protected internal) GetConnectorHTTPRequest  (Request)

            /// <summary>
            /// An event sent whenever a GET connector HTTP request was received.
            /// </summary>
            public OCPIRequestLogEvent OnGetConnectorHTTPRequest = new();

            /// <summary>
            /// An event sent whenever a GET connector HTTP request was received.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The CPO HTTP API.</param>
            /// <param name="Request">The OCPI request.</param>
            protected internal Task GetConnectorHTTPRequest(DateTimeOffset     Timestamp,
                                                            HTTPAPIX           API,
                                                            OCPIRequest        Request,
                                                            CancellationToken  CancellationToken)

                => OnGetConnectorHTTPRequest.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       CancellationToken
                   );

            #endregion

            #region (protected internal) GetConnectorHTTPResponse (Response)

            /// <summary>
            /// An event sent whenever a GET connector HTTP response was sent.
            /// </summary>
            public OCPIResponseLogEvent OnGetConnectorHTTPResponse = new();

            /// <summary>
            /// An event sent whenever a GET connector HTTP response was sent.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The CPO HTTP API.</param>
            /// <param name="Request">The OCPI request.</param>
            /// <param name="Response">The OCPI response.</param>
            protected internal Task GetConnectorHTTPResponse(DateTimeOffset     Timestamp,
                                                             HTTPAPIX           API,
                                                             OCPIRequest        Request,
                                                             OCPIResponse       Response,
                                                             CancellationToken  CancellationToken)

                => OnGetConnectorHTTPResponse.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       Response,
                       CancellationToken
                   );

            #endregion

            #endregion

            #region Tariff(s)

            #region (protected internal) GetTariffsHTTPRequest  (Request)

            /// <summary>
            /// An event sent whenever a GET tariffs HTTP request was received.
            /// </summary>
            public OCPIRequestLogEvent OnGetTariffsHTTPRequest = new();

            /// <summary>
            /// An event sent whenever a GET tariffs HTTP request was received.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The CPO HTTP API.</param>
            /// <param name="Request">The OCPI request.</param>
            protected internal Task GetTariffsHTTPRequest(DateTimeOffset     Timestamp,
                                                          HTTPAPIX           API,
                                                          OCPIRequest        Request,
                                                          CancellationToken  CancellationToken)

                => OnGetTariffsHTTPRequest.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       CancellationToken
                   );

            #endregion

            #region (protected internal) GetTariffsHTTPResponse (Response)

            /// <summary>
            /// An event sent whenever a GET tariffs HTTP response was sent.
            /// </summary>
            public OCPIResponseLogEvent OnGetTariffsHTTPResponse = new();

            /// <summary>
            /// An event sent whenever a GET tariffs HTTP response was sent.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The CPO HTTP API.</param>
            /// <param name="Request">The OCPI request.</param>
            /// <param name="Response">The OCPI response.</param>
            protected internal Task GetTariffsHTTPResponse(DateTimeOffset     Timestamp,
                                                           HTTPAPIX           API,
                                                           OCPIRequest        Request,
                                                           OCPIResponse       Response,
                                                           CancellationToken  CancellationToken)

                => OnGetTariffsHTTPResponse.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       Response,
                       CancellationToken
                   );

            #endregion


            #region (protected internal) GetTariffHTTPRequest   (Request)

            /// <summary>
            /// An event sent whenever a GET tariff HTTP request was received.
            /// </summary>
            public OCPIRequestLogEvent OnGetTariffHTTPRequest = new();

            /// <summary>
            /// An event sent whenever a GET tariff HTTP request was received.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The CPO HTTP API.</param>
            /// <param name="Request">The OCPI request.</param>
            protected internal Task GetTariffHTTPRequest(DateTimeOffset     Timestamp,
                                                         HTTPAPIX           API,
                                                         OCPIRequest        Request,
                                                         CancellationToken  CancellationToken)

                => OnGetTariffHTTPRequest.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       CancellationToken
                   );

            #endregion

            #region (protected internal) GetTariffHTTPResponse  (Response)

            /// <summary>
            /// An event sent whenever a GET tariff HTTP response was sent.
            /// </summary>
            public OCPIResponseLogEvent OnGetTariffHTTPResponse = new();

            /// <summary>
            /// An event sent whenever a GET tariff HTTP response was sent.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The CPO HTTP API.</param>
            /// <param name="Request">The OCPI request.</param>
            /// <param name="Response">The OCPI response.</param>
            protected internal Task GetTariffHTTPResponse(DateTimeOffset     Timestamp,
                                                          HTTPAPIX           API,
                                                          OCPIRequest        Request,
                                                          OCPIResponse       Response,
                                                          CancellationToken  CancellationToken)

                => OnGetTariffHTTPResponse.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       Response,
                       CancellationToken
                   );

            #endregion

            #endregion

            #region Session(s)

            #region (protected internal) GetSessionsHTTPRequest  (Request)

            /// <summary>
            /// An event sent whenever a GET sessions HTTP request was received.
            /// </summary>
            public OCPIRequestLogEvent OnGetSessionsHTTPRequest = new();

            /// <summary>
            /// An event sent whenever a GET sessions HTTP request was received.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The CPO HTTP API.</param>
            /// <param name="Request">The OCPI request.</param>
            protected internal Task GetSessionsHTTPRequest(DateTimeOffset     Timestamp,
                                                           HTTPAPIX           API,
                                                           OCPIRequest        Request,
                                                           CancellationToken  CancellationToken)

                => OnGetSessionsHTTPRequest.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       CancellationToken
                   );

            #endregion

            #region (protected internal) GetSessionsHTTPResponse (Response)

            /// <summary>
            /// An event sent whenever a GET sessions HTTP response was sent.
            /// </summary>
            public OCPIResponseLogEvent OnGetSessionsHTTPResponse = new();

            /// <summary>
            /// An event sent whenever a GET sessions HTTP response was sent.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The CPO HTTP API.</param>
            /// <param name="Request">The OCPI request.</param>
            /// <param name="Response">The OCPI response.</param>
            protected internal Task GetSessionsHTTPResponse(DateTimeOffset     Timestamp,
                                                            HTTPAPIX           API,
                                                            OCPIRequest        Request,
                                                            OCPIResponse       Response,
                                                            CancellationToken  CancellationToken)

                => OnGetSessionsHTTPResponse.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       Response,
                       CancellationToken
                   );

            #endregion


            #region (protected internal) GetSessionHTTPRequest   (Request)

            /// <summary>
            /// An event sent whenever a GET session HTTP request was received.
            /// </summary>
            public OCPIRequestLogEvent OnGetSessionHTTPRequest = new();

            /// <summary>
            /// An event sent whenever a GET session HTTP request was received.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The CPO HTTP API.</param>
            /// <param name="Request">The OCPI request.</param>
            protected internal Task GetSessionHTTPRequest(DateTimeOffset     Timestamp,
                                                          HTTPAPIX           API,
                                                          OCPIRequest        Request,
                                                          CancellationToken  CancellationToken)

                => OnGetSessionHTTPRequest.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       CancellationToken
                   );

            #endregion

            #region (protected internal) GetSessionHTTPResponse  (Response)

            /// <summary>
            /// An event sent whenever a GET session HTTP response was sent.
            /// </summary>
            public OCPIResponseLogEvent OnGetSessionHTTPResponse = new();

            /// <summary>
            /// An event sent whenever a GET session HTTP response was sent.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The CPO HTTP API.</param>
            /// <param name="Request">The OCPI request.</param>
            /// <param name="Response">The OCPI response.</param>
            protected internal Task GetSessionHTTPResponse(DateTimeOffset     Timestamp,
                                                           HTTPAPIX           API,
                                                           OCPIRequest        Request,
                                                           OCPIResponse       Response,
                                                           CancellationToken  CancellationToken)

                => OnGetSessionHTTPResponse.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       Response,
                       CancellationToken
                   );

            #endregion

            #endregion

            #region CDR(s)

            #region (protected internal) GetCDRsHTTPRequest  (Request)

            /// <summary>
            /// An event sent whenever a GET CDRs HTTP request was received.
            /// </summary>
            public OCPIRequestLogEvent OnGetCDRsHTTPRequest = new();

            /// <summary>
            /// An event sent whenever a GET CDRs HTTP request was received.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The CPO HTTP API.</param>
            /// <param name="Request">The OCPI request.</param>
            protected internal Task GetCDRsHTTPRequest(DateTimeOffset     Timestamp,
                                                       HTTPAPIX           API,
                                                       OCPIRequest        Request,
                                                       CancellationToken  CancellationToken)

                => OnGetCDRsHTTPRequest.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       CancellationToken
                   );

            #endregion

            #region (protected internal) GetCDRsHTTPResponse (Response)

            /// <summary>
            /// An event sent whenever a GET CDRs HTTP response was sent.
            /// </summary>
            public OCPIResponseLogEvent OnGetCDRsHTTPResponse = new();

            /// <summary>
            /// An event sent whenever a GET CDRs HTTP response was sent.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The CPO HTTP API.</param>
            /// <param name="Request">The OCPI request.</param>
            /// <param name="Response">The OCPI response.</param>
            protected internal Task GetCDRsHTTPResponse(DateTimeOffset     Timestamp,
                                                        HTTPAPIX           API,
                                                        OCPIRequest        Request,
                                                        OCPIResponse       Response,
                                                        CancellationToken  CancellationToken)

                => OnGetCDRsHTTPResponse.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       Response,
                       CancellationToken
                   );

            #endregion


            #region (protected internal) GetCDRHTTPRequest   (Request)

            /// <summary>
            /// An event sent whenever a GET CDR HTTP request was received.
            /// </summary>
            public OCPIRequestLogEvent OnGetCDRHTTPRequest = new();

            /// <summary>
            /// An event sent whenever a GET CDR HTTP request was received.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The CPO HTTP API.</param>
            /// <param name="Request">The OCPI request.</param>
            protected internal Task GetCDRHTTPRequest(DateTimeOffset     Timestamp,
                                                      HTTPAPIX           API,
                                                      OCPIRequest        Request,
                                                      CancellationToken  CancellationToken)

                => OnGetCDRHTTPRequest.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       CancellationToken
                   );

            #endregion

            #region (protected internal) GetCDRHTTPResponse  (Response)

            /// <summary>
            /// An event sent whenever a GET CDR HTTP response was sent.
            /// </summary>
            public OCPIResponseLogEvent OnGetCDRHTTPResponse = new();

            /// <summary>
            /// An event sent whenever a GET CDR HTTP response was sent.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The CPO HTTP API.</param>
            /// <param name="Request">The OCPI request.</param>
            /// <param name="Response">The OCPI response.</param>
            protected internal Task GetCDRHTTPResponse(DateTimeOffset     Timestamp,
                                                       HTTPAPIX           API,
                                                       OCPIRequest        Request,
                                                       OCPIResponse       Response,
                                                       CancellationToken  CancellationToken)

                => OnGetCDRHTTPResponse.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       Response,
                       CancellationToken
                   );

            #endregion

            #endregion

            #region Token(s)

            #region (protected internal) GetTokensHTTPRequest     (Request)

            /// <summary>
            /// An event sent whenever a GET Tokens HTTP request was received.
            /// </summary>
            public OCPIRequestLogEvent OnGetTokensHTTPRequest = new();

            /// <summary>
            /// An event sent whenever a GET Tokens HTTP request was received.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The CPO HTTP API.</param>
            /// <param name="Request">The OCPI request.</param>
            protected internal Task GetTokensHTTPRequest(DateTimeOffset     Timestamp,
                                                         HTTPAPIX           API,
                                                         OCPIRequest        Request,
                                                         CancellationToken  CancellationToken)

                => OnGetTokensHTTPRequest.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       CancellationToken
                   );

            #endregion

            #region (protected internal) GetTokensHTTPResponse    (Response)

            /// <summary>
            /// An event sent whenever a GET Tokens HTTP response was sent.
            /// </summary>
            public OCPIResponseLogEvent OnGetTokensHTTPResponse = new();

            /// <summary>
            /// An event sent whenever a GET Tokens HTTP response was sent.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The CPO HTTP API.</param>
            /// <param name="Request">The OCPI request.</param>
            /// <param name="Response">The OCPI response.</param>
            protected internal Task GetTokensHTTPResponse(DateTimeOffset     Timestamp,
                                                          HTTPAPIX           API,
                                                          OCPIRequest        Request,
                                                          OCPIResponse       Response,
                                                          CancellationToken  CancellationToken)

                => OnGetTokensHTTPResponse.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       Response,
                       CancellationToken
                   );

            #endregion


            #region (protected internal) DeleteTokensHTTPRequest  (Request)

            /// <summary>
            /// An event sent whenever a DELETE Tokens HTTP request was received.
            /// </summary>
            public OCPIRequestLogEvent OnDeleteTokensHTTPRequest = new();

            /// <summary>
            /// An event sent whenever a DELETE Tokens HTTP request was received.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The CPO HTTP API.</param>
            /// <param name="Request">The OCPI request.</param>
            protected internal Task DeleteTokensHTTPRequest(DateTimeOffset     Timestamp,
                                                            HTTPAPIX           API,
                                                            OCPIRequest        Request,
                                                            CancellationToken  CancellationToken)

                => OnDeleteTokensHTTPRequest.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       CancellationToken
                   );

            #endregion

            #region (protected internal) DeleteTokensHTTPResponse (Response)

            /// <summary>
            /// An event sent whenever a DELETE Tokens HTTP response was sent.
            /// </summary>
            public OCPIResponseLogEvent OnDeleteTokensHTTPResponse = new();

            /// <summary>
            /// An event sent whenever a DELETE Tokens HTTP response was sent.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The CPO HTTP API.</param>
            /// <param name="Request">The OCPI request.</param>
            /// <param name="Response">The OCPI response.</param>
            protected internal Task DeleteTokensHTTPResponse(DateTimeOffset     Timestamp,
                                                             HTTPAPIX           API,
                                                             OCPIRequest        Request,
                                                             OCPIResponse       Response,
                                                             CancellationToken  CancellationToken)

                => OnDeleteTokensHTTPResponse.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       Response,
                       CancellationToken
                   );

            #endregion


            // Token

            #region (protected internal) GetTokenHTTPRequest      (Request)

            /// <summary>
            /// An event sent whenever a GET Token HTTP request was received.
            /// </summary>
            public OCPIRequestLogEvent OnGetTokenHTTPRequest = new();

            /// <summary>
            /// An event sent whenever a GET Token HTTP request was received.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The CPO HTTP API.</param>
            /// <param name="Request">The OCPI request.</param>
            protected internal Task GetTokenHTTPRequest(DateTimeOffset     Timestamp,
                                                        HTTPAPIX           API,
                                                        OCPIRequest        Request,
                                                        CancellationToken  CancellationToken)

                => OnGetTokenHTTPRequest.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       CancellationToken
                   );

            #endregion

            #region (protected internal) GetTokenHTTPResponse     (Response)

            /// <summary>
            /// An event sent whenever a GET Token HTTP response was sent.
            /// </summary>
            public OCPIResponseLogEvent OnGetTokenHTTPResponse = new();

            /// <summary>
            /// An event sent whenever a GET Token HTTP response was sent.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The CPO HTTP API.</param>
            /// <param name="Request">The OCPI request.</param>
            /// <param name="Response">The OCPI response.</param>
            protected internal Task GetTokenHTTPResponse(DateTimeOffset     Timestamp,
                                                         HTTPAPIX           API,
                                                         OCPIRequest        Request,
                                                         OCPIResponse       Response,
                                                         CancellationToken  CancellationToken)

                => OnGetTokenHTTPResponse.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       Response,
                       CancellationToken
                   );

            #endregion


            #region (protected internal) PostTokenHTTPRequest     (Request)

            /// <summary>
            /// An event sent whenever a POST token HTTP request was received.
            /// </summary>
            public OCPIRequestLogEvent OnPostTokenHTTPRequest = new();

            /// <summary>
            /// An event sent whenever a POST token HTTP request was received.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The CPO HTTP API.</param>
            /// <param name="Request">The OCPI request.</param>
            protected internal Task PostTokenHTTPRequest(DateTimeOffset     Timestamp,
                                                         HTTPAPIX           API,
                                                         OCPIRequest        Request,
                                                         CancellationToken  CancellationToken)

                => OnPostTokenHTTPRequest.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       CancellationToken
                   );

            #endregion

            #region (protected internal) PostTokenHTTPResponse    (Response)

            /// <summary>
            /// An event sent whenever a POST token HTTP response was sent.
            /// </summary>
            public OCPIResponseLogEvent OnPostTokenHTTPResponse = new();

            /// <summary>
            /// An event sent whenever a POST token HTTP response was sent.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The CPO HTTP API.</param>
            /// <param name="Request">The OCPI request.</param>
            /// <param name="Response">The OCPI response.</param>
            protected internal Task PostTokenHTTPResponse(DateTimeOffset     Timestamp,
                                                          HTTPAPIX           API,
                                                          OCPIRequest        Request,
                                                          OCPIResponse       Response,
                                                          CancellationToken  CancellationToken)

                => OnPostTokenHTTPResponse.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       Response,
                       CancellationToken
                   );

            #endregion


            #region (protected internal) PutTokenHTTPRequest      (Request)

            /// <summary>
            /// An event sent whenever a put token HTTP request was received.
            /// </summary>
            public OCPIRequestLogEvent OnPutTokenHTTPRequest = new();

            /// <summary>
            /// An event sent whenever a put token HTTP request was received.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The CPO HTTP API.</param>
            /// <param name="Request">The OCPI request.</param>
            protected internal Task PutTokenHTTPRequest(DateTimeOffset     Timestamp,
                                                        HTTPAPIX           API,
                                                        OCPIRequest        Request,
                                                        CancellationToken  CancellationToken)

                => OnPutTokenHTTPRequest.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       CancellationToken
                   );

            #endregion

            #region (protected internal) PutTokenHTTPResponse     (Response)

            /// <summary>
            /// An event sent whenever a put token HTTP response was sent.
            /// </summary>
            public OCPIResponseLogEvent OnPutTokenHTTPResponse = new();

            /// <summary>
            /// An event sent whenever a put token HTTP response was sent.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The CPO HTTP API.</param>
            /// <param name="Request">The OCPI request.</param>
            /// <param name="Response">The OCPI response.</param>
            protected internal Task PutTokenHTTPResponse(DateTimeOffset     Timestamp,
                                                         HTTPAPIX           API,
                                                         OCPIRequest        Request,
                                                         OCPIResponse       Response,
                                                         CancellationToken  CancellationToken)

                => OnPutTokenHTTPResponse.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       Response,
                       CancellationToken
                   );

            #endregion


            #region (protected internal) PatchTokenHTTPRequest    (Request)

            /// <summary>
            /// An event sent whenever a patch token HTTP request was received.
            /// </summary>
            public OCPIRequestLogEvent OnPatchTokenHTTPRequest = new();

            /// <summary>
            /// An event sent whenever a patch token HTTP request was received.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The CPO HTTP API.</param>
            /// <param name="Request">The OCPI request.</param>
            protected internal Task PatchTokenHTTPRequest(DateTimeOffset     Timestamp,
                                                          HTTPAPIX           API,
                                                          OCPIRequest        Request,
                                                          CancellationToken  CancellationToken)

                => OnPatchTokenHTTPRequest.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       CancellationToken
                   );

            #endregion

            #region (protected internal) PatchTokenHTTPResponse   (Response)

            /// <summary>
            /// An event sent whenever a patch token HTTP response was sent.
            /// </summary>
            public OCPIResponseLogEvent OnPatchTokenHTTPResponse = new();

            /// <summary>
            /// An event sent whenever a patch token HTTP response was sent.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The CPO HTTP API.</param>
            /// <param name="Request">The OCPI request.</param>
            /// <param name="Response">The OCPI response.</param>
            protected internal Task PatchTokenHTTPResponse(DateTimeOffset     Timestamp,
                                                           HTTPAPIX           API,
                                                           OCPIRequest        Request,
                                                           OCPIResponse       Response,
                                                           CancellationToken  CancellationToken)

                => OnPatchTokenHTTPResponse.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       Response,
                       CancellationToken
                   );

            #endregion


            #region (protected internal) DeleteTokenHTTPRequest   (Request)

            /// <summary>
            /// An event sent whenever a delete token HTTP request was received.
            /// </summary>
            public OCPIRequestLogEvent OnDeleteTokenHTTPRequest = new();

            /// <summary>
            /// An event sent whenever a delete token HTTP request was received.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The CPO HTTP API.</param>
            /// <param name="Request">The OCPI request.</param>
            protected internal Task DeleteTokenHTTPRequest(DateTimeOffset     Timestamp,
                                                           HTTPAPIX           API,
                                                           OCPIRequest        Request,
                                                           CancellationToken  CancellationToken)

                => OnDeleteTokenHTTPRequest.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       CancellationToken
                   );

            #endregion

            #region (protected internal) DeleteTokenHTTPResponse  (Response)

            /// <summary>
            /// An event sent whenever a delete token HTTP response was sent.
            /// </summary>
            public OCPIResponseLogEvent OnDeleteTokenHTTPResponse = new();

            /// <summary>
            /// An event sent whenever a delete token HTTP response was sent.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The CPO HTTP API.</param>
            /// <param name="Request">The OCPI request.</param>
            /// <param name="Response">The OCPI response.</param>
            protected internal Task DeleteTokenHTTPResponse(DateTimeOffset     Timestamp,
                                                            HTTPAPIX           API,
                                                            OCPIRequest        Request,
                                                            OCPIResponse       Response,
                                                            CancellationToken  CancellationToken)

                => OnDeleteTokenHTTPResponse.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       Response,
                       CancellationToken
                   );

            #endregion

            #endregion


            // Commands

            #region (protected internal) ReserveNowHTTPRequest         (Request)

            /// <summary>
            /// An event sent whenever a reserve now command was received.
            /// </summary>
            public OCPIRequestLogEvent OnReserveNowHTTPRequest = new();

            /// <summary>
            /// An event sent whenever a reserve now command was received.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the command request.</param>
            /// <param name="API">The CPO HTTP API.</param>
            /// <param name="Request">The OCPI request.</param>
            protected internal Task ReserveNowHTTPRequest(DateTimeOffset     Timestamp,
                                                          HTTPAPIX           API,
                                                          OCPIRequest        Request,
                                                          CancellationToken  CancellationToken)

                => OnReserveNowHTTPRequest.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       CancellationToken
                   );

            #endregion

            #region (protected internal) ReserveNowHTTPResponse        (Response)

            /// <summary>
            /// An event sent whenever a reserve now command HTTP response was sent.
            /// </summary>
            public OCPIResponseLogEvent OnReserveNowHTTPResponse = new();

            /// <summary>
            /// An event sent whenever a reserve now command HTTP response was sent.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the command response.</param>
            /// <param name="API">The CPO HTTP API.</param>
            /// <param name="Request">The OCPI request.</param>
            /// <param name="Response">The OCPI response.</param>
            protected internal Task ReserveNowHTTPResponse(DateTimeOffset     Timestamp,
                                                           HTTPAPIX           API,
                                                           OCPIRequest        Request,
                                                           OCPIResponse       Response,
                                                           CancellationToken  CancellationToken)

                => OnReserveNowHTTPResponse.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       Response,
                       CancellationToken
                   );

            #endregion


            #region (protected internal) CancelReservationHTTPRequest  (Request)

            /// <summary>
            /// An event sent whenever a cancel reservation command was received.
            /// </summary>
            public OCPIRequestLogEvent OnCancelReservationHTTPRequest = new();

            /// <summary>
            /// An event sent whenever a cancel reservation command was received.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the command request.</param>
            /// <param name="API">The CPO HTTP API.</param>
            /// <param name="Request">The OCPI request.</param>
            protected internal Task CancelReservationHTTPRequest(DateTimeOffset     Timestamp,
                                                                 HTTPAPIX           API,
                                                                 OCPIRequest        Request,
                                                                 CancellationToken  CancellationToken)

                => OnCancelReservationHTTPRequest.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       CancellationToken
                   );

            #endregion

            #region (protected internal) CancelReservationHTTPResponse (Response)

            /// <summary>
            /// An event sent whenever a cancel reservation command HTTP response was sent.
            /// </summary>
            public OCPIResponseLogEvent OnCancelReservationHTTPResponse = new();

            /// <summary>
            /// An event sent whenever a cancel reservation command HTTP response was sent.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the command response.</param>
            /// <param name="API">The CPO HTTP API.</param>
            /// <param name="Request">The OCPI request.</param>
            /// <param name="Response">The OCPI response.</param>
            protected internal Task CancelReservationHTTPResponse(DateTimeOffset     Timestamp,
                                                                  HTTPAPIX           API,
                                                                  OCPIRequest        Request,
                                                                  OCPIResponse       Response,
                                                                  CancellationToken  CancellationToken)

                => OnCancelReservationHTTPResponse.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       Response,
                       CancellationToken
                   );

            #endregion


            #region (protected internal) StartSessionHTTPRequest       (Request)

            /// <summary>
            /// An event sent whenever a start session command was received.
            /// </summary>
            public OCPIRequestLogEvent OnStartSessionHTTPRequest = new();

            /// <summary>
            /// An event sent whenever a start session command was received.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the command request.</param>
            /// <param name="API">The CPO HTTP API.</param>
            /// <param name="Request">The OCPI request.</param>
            protected internal Task StartSessionHTTPRequest(DateTimeOffset     Timestamp,
                                                            HTTPAPIX           API,
                                                            OCPIRequest        Request,
                                                            CancellationToken  CancellationToken)

                => OnStartSessionHTTPRequest.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       CancellationToken
                   );

            #endregion

            #region (protected internal) StartSessionHTTPResponse      (Response)

            /// <summary>
            /// An event sent whenever a start session command HTTP response was sent.
            /// </summary>
            public OCPIResponseLogEvent OnStartSessionHTTPResponse = new();

            /// <summary>
            /// An event sent whenever a start session command HTTP response was sent.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the command response.</param>
            /// <param name="API">The CPO HTTP API.</param>
            /// <param name="Request">The OCPI request.</param>
            /// <param name="Response">The OCPI response.</param>
            protected internal Task StartSessionHTTPResponse(DateTimeOffset     Timestamp,
                                                             HTTPAPIX           API,
                                                             OCPIRequest        Request,
                                                             OCPIResponse       Response,
                                                             CancellationToken  CancellationToken)

                => OnStartSessionHTTPResponse.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       Response,
                       CancellationToken
                   );

            #endregion


            #region (protected internal) StopSessionHTTPRequest        (Request)

            /// <summary>
            /// An event sent whenever a stop session command was received.
            /// </summary>
            public OCPIRequestLogEvent OnStopSessionHTTPRequest = new();

            /// <summary>
            /// An event sent whenever a stop session command was received.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the command request.</param>
            /// <param name="API">The CPO HTTP API.</param>
            /// <param name="Request">The OCPI request.</param>
            protected internal Task StopSessionHTTPRequest(DateTimeOffset     Timestamp,
                                                           HTTPAPIX           API,
                                                           OCPIRequest        Request,
                                                           CancellationToken  CancellationToken)

                => OnStopSessionHTTPRequest.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       CancellationToken
                   );

            #endregion

            #region (protected internal) StopSessionHTTPResponse       (Response)

            /// <summary>
            /// An event sent whenever a stop session command HTTP response was sent.
            /// </summary>
            public OCPIResponseLogEvent OnStopSessionHTTPResponse = new();

            /// <summary>
            /// An event sent whenever a stop session command HTTP response was sent.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the command response.</param>
            /// <param name="API">The CPO HTTP API.</param>
            /// <param name="Request">The OCPI request.</param>
            /// <param name="Response">The OCPI response.</param>
            protected internal Task StopSessionHTTPResponse(DateTimeOffset     Timestamp,
                                                            HTTPAPIX           API,
                                                            OCPIRequest        Request,
                                                            OCPIResponse       Response,
                                                            CancellationToken  CancellationToken)

                => OnStopSessionHTTPResponse.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       Response,
                       CancellationToken
                   );

            #endregion


            #region (protected internal) UnlockConnectorHTTPRequest    (Request)

            /// <summary>
            /// An event sent whenever a unlock connector command was received.
            /// </summary>
            public OCPIRequestLogEvent OnUnlockConnectorHTTPRequest = new();

            /// <summary>
            /// An event sent whenever a unlock connector command was received.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the command request.</param>
            /// <param name="API">The CPO HTTP API.</param>
            /// <param name="Request">The OCPI request.</param>
            protected internal Task UnlockConnectorHTTPRequest(DateTimeOffset     Timestamp,
                                                               HTTPAPIX           API,
                                                               OCPIRequest        Request,
                                                               CancellationToken  CancellationToken)

                => OnUnlockConnectorHTTPRequest.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       CancellationToken
                   );

            #endregion

            #region (protected internal) UnlockConnectorHTTPResponse   (Response)

            /// <summary>
            /// An event sent whenever a unlock connector command HTTP response was sent.
            /// </summary>
            public OCPIResponseLogEvent OnUnlockConnectorHTTPResponse = new();

            /// <summary>
            /// An event sent whenever a unlock connector command HTTP response was sent.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the command response.</param>
            /// <param name="API">The CPO HTTP API.</param>
            /// <param name="Request">The OCPI request.</param>
            /// <param name="Response">The OCPI response.</param>
            protected internal Task UnlockConnectorHTTPResponse(DateTimeOffset     Timestamp,
                                                                HTTPAPIX           API,
                                                                OCPIRequest        Request,
                                                                OCPIResponse       Response,
                                                                CancellationToken  CancellationToken)

                => OnUnlockConnectorHTTPResponse.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       Response,
                       CancellationToken
                   );

                #endregion


        }


        // Commands

        #region OnReserveNowCommand

        public delegate Task<CommandResponse> OnReserveNowCommandDelegate(RemoteParty_Id     RemotePartyId,
                                                                          EMSP_Id?           From,
                                                                          CPO_Id?            To,
                                                                          ReserveNowCommand  ReserveNowCommand);

        public event OnReserveNowCommandDelegate? OnReserveNowCommand;

        #endregion

        #region OnCancelReservationCommand

        public delegate Task<CommandResponse> OnCancelReservationCommandDelegate(RemoteParty_Id            RemotePartyId,
                                                                                 EMSP_Id?                  From,
                                                                                 CPO_Id?                   To,
                                                                                 CancelReservationCommand  CancelReservationCommand);

        public event OnCancelReservationCommandDelegate? OnCancelReservationCommand;

        #endregion

        #region OnStartSessionCommand

        public delegate Task<CommandResponse> OnStartSessionCommandDelegate(RemoteParty_Id       RemotePartyId,
                                                                            EMSP_Id?             From,
                                                                            CPO_Id?              To,
                                                                            StartSessionCommand  StartSessionCommand);

        public event OnStartSessionCommandDelegate? OnStartSessionCommand;

        #endregion

        #region OnStopSessionCommand

        public delegate Task<CommandResponse> OnStopSessionCommandDelegate(RemoteParty_Id      RemotePartyId,
                                                                           EMSP_Id?            From,
                                                                           CPO_Id?             To,
                                                                           StopSessionCommand  StopSessionCommand);

        public event OnStopSessionCommandDelegate? OnStopSessionCommand;

        #endregion

        #region OnUnlockConnectorCommand

        public delegate Task<CommandResponse> OnUnlockConnectorCommandDelegate(RemoteParty_Id          RemotePartyId,
                                                                               EMSP_Id?                From,
                                                                               CPO_Id?                 To,
                                                                               UnlockConnectorCommand  UnlockConnectorCommand);

        public event OnUnlockConnectorCommandDelegate? OnUnlockConnectorCommand;

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
        /// <param name="ExternalDNSName">The official URL/DNS name of this service, e.g. for sending e-mails.</param>
        /// <param name="URLPathPrefix">An optional URL path prefix.</param>
        /// <param name="BasePath">When the API is served from an optional subdirectory path.</param>
        /// <param name="HTTPServiceName">An optional name of the HTTP API service.</param>
        public CPO_HTTPAPI(CommonAPI                    CommonAPI,
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

                   Description     ?? I18NString.Create($"OCPI{Version.String} CPO HTTP API"),

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

            //this.Counters         = new APICounters();

            this.HTTPLogger       = this.DisableLogging == false
                                        ? new CPO_HTTPAPI_Logger(
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

        private readonly ConcurrentDictionary<EMSP_Id, CPO2EMSP_HTTPClient> cpo2emspClients = new();

        /// <summary>
        /// Return an enumeration of all CPO2EMSP clients.
        /// </summary>
        public IEnumerable<CPO2EMSP_HTTPClient> CPO2EMSPClients
            => cpo2emspClients.Values;


        #region GetEMSPClient (CountryCode, PartyId,   Description = null, AllowCachedClients = true)

        /// <summary>
        /// As a CPO create a client to access e.g. a remote EMSP.
        /// </summary>
        /// <param name="CountryCode">The country code of the remote EMSP.</param>
        /// <param name="PartyId">The party identification of the remote EMSP.</param>
        /// <param name="Description">A description for the OCPI client.</param>
        /// <param name="AllowCachedClients">Whether to allow to return cached CPO clients.</param>
        public CPO2EMSP_HTTPClient? GetEMSPClient(CountryCode  CountryCode,
                                                  Party_Id     PartyId,
                                                  I18NString?  Description          = null,
                                                  Boolean      AllowCachedClients   = true)

            => GetEMSPClient(
                   RemoteParty_Id.From(
                       CountryCode,
                       PartyId,
                       Role.EMSP
                   ),
                   Description,
                   AllowCachedClients
               );

        #endregion

        #region GetEMSPClient (             PartyIdv3, Description = null, AllowCachedClients = true)

        /// <summary>
        /// As a CPO create a client to access e.g. a remote EMSP.
        /// </summary>
        /// <param name="v">The party identification of the remote EMSP.</param>
        /// <param name="Description">A description for the OCPI client.</param>
        /// <param name="AllowCachedClients">Whether to allow to return cached CPO clients.</param>
        public CPO2EMSP_HTTPClient? GetEMSPClient(Party_Idv3   PartyIdv3,
                                                  I18NString?  Description          = null,
                                                  Boolean      AllowCachedClients   = true)

            => GetEMSPClient(
                   RemoteParty_Id.From(
                       PartyIdv3,
                       Role.EMSP
                   ),
                   Description,
                   AllowCachedClients
               );

        #endregion

        #region GetEMSPClient (RemoteParty,            Description = null, AllowCachedClients = true)

        /// <summary>
        /// As a CPO create a client to access e.g. a remote EMSP.
        /// </summary>
        /// <param name="RemoteParty">A remote party.</param>
        /// <param name="Description">A description for the OCPI client.</param>
        /// <param name="AllowCachedClients">Whether to allow to return cached CPO clients.</param>
        public CPO2EMSP_HTTPClient? GetEMSPClient(RemoteParty  RemoteParty,
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

                var cpo2EMSPClient = new CPO2EMSP_HTTPClient(
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

                cpo2emspClients.TryAdd(emspId, cpo2EMSPClient);

                DefaultCPO2EMSP_HTTPClientConfigurator?.Invoke(cpo2EMSPClient);

                return cpo2EMSPClient;

            }

            return null;

        }

        #endregion

        #region GetEMSPClient (RemotePartyId,          Description = null, AllowCachedClients = true)

        /// <summary>
        /// As a CPO create a client to access e.g. a remote EMSP.
        /// </summary>
        /// <param name="RemotePartyId">A remote party identification.</param>
        /// <param name="Description">A description for the OCPI client.</param>
        /// <param name="AllowCachedClients">Whether to allow to return cached CPO clients.</param>
        public CPO2EMSP_HTTPClient? GetEMSPClient(RemoteParty_Id  RemotePartyId,
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

                var cpo2EMSPClient = new CPO2EMSP_HTTPClient(
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

                cpo2emspClients.TryAdd(emspId, cpo2EMSPClient);

                DefaultCPO2EMSP_HTTPClientConfigurator?.Invoke(cpo2EMSPClient);

                return cpo2EMSPClient;

            }

            return null;

        }

        #endregion


        public Task CloseAllClients()
        {

            foreach (var client in cpo2emspClients.Values)
            {
                client.Close();
            }

            cpo2emspClients.Clear();

            return Task.CompletedTask;

        }

        #endregion


        #region (private) RegisterURLTemplates()

        private void RegisterURLTemplates()
        {

            #region ~/locations

            #region OPTIONS  ~/locations

            CommonAPI.AddOCPIMethod(

                HTTPMethod.OPTIONS,
                URLPathPrefix + "locations",
                request =>

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

            // https://example.com/ocpi/2.2.1/cpo/locations/?date_from=2019-01-28T12:00:00&date_to=2019-01-29T12:00:00&offset=50&limit=100
            CommonAPI.AddOCPIMethod(

                HTTPMethod.GET,
                URLPathPrefix + "locations",
                HTTPEvents.GetLocationsHTTPRequest,
                HTTPEvents.GetLocationsHTTPResponse,
                request => {

                    #region Check access token

                    if ((request.LocalAccessInfo is not null || CommonAPI.BaseAPI.LocationsAsOpenData == false) &&
                        (request.LocalAccessInfo?.Status != AccessStatus.ALLOWED ||
                         request.LocalAccessInfo.IsNot(Role.EMSP, Role.HUB) == true))
                    {

                        return Task.FromResult(
                            new OCPIResponse.Builder(request) {
                                StatusCode           = 2000,
                                StatusMessage        = "Invalid or blocked access token!",
                                HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                    HTTPStatusCode              = HTTPStatusCode.Forbidden,
                                    AccessControlAllowMethods   = [ "OPTIONS", "GET" ],
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
                    var allLocations         = CommonAPI.
                                                   GetLocations().//location => Request.AccessInfo.Value.Roles.Any(role => role.CountryCode == location.CountryCode &&
                                                                  //                                                       role.PartyId     == location.PartyId)).
                                                   ToArray();

                    var filteredLocations    = allLocations.
                                                   Where(matchFilter).
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
                                                                  Select        (location => location.ToJSON(
                                                                                                 request.EMSPId ?? request.HUBId,
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
                                                                                                 CustomEnvironmentalImpactSerializer
                                                                                             ))
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

                HTTPMethod.OPTIONS,
                URLPathPrefix + "locations/{locationId}",
                request =>

                    Task.FromResult(
                        new OCPIResponse.Builder(request) {
                            HTTPResponseBuilder = new HTTPResponse.Builder(request.HTTPRequest) {
                                HTTPStatusCode              = HTTPStatusCode.OK,
                                Allow                       = [ HTTPMethod.OPTIONS, HTTPMethod.GET ],
                                AccessControlAllowMethods   = [ "OPTIONS", "GET" ],
                                AccessControlAllowHeaders   = [ "Authorization" ],
                                AccessControlExposeHeaders  = [ "X-Request-ID", "X-Correlation-ID" ]
                            }
                        })

            );

            #endregion

            #region GET      ~/locations/{locationId}

            CommonAPI.AddOCPIMethod(

                HTTPMethod.GET,
                URLPathPrefix + "locations/{locationId}",
                HTTPEvents.GetLocationHTTPRequest,
                HTTPEvents.GetLocationHTTPResponse,
                request => {

                    #region Check access token

                    if ((request.LocalAccessInfo is not null || CommonAPI.BaseAPI.LocationsAsOpenData == false) &&
                        (request.LocalAccessInfo?.Status != AccessStatus.ALLOWED ||
                         request.LocalAccessInfo.IsNot(Role.EMSP, Role.HUB) == true))
                    {

                        return Task.FromResult(
                            new OCPIResponse.Builder(request) {
                                StatusCode           = 2000,
                                StatusMessage        = "Invalid or blocked access token!",
                                HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                    HTTPStatusCode              = HTTPStatusCode.Forbidden,
                                    AccessControlAllowMethods   = [ "OPTIONS", "GET" ],
                                    AccessControlAllowHeaders   = [ "Authorization" ],
                                    AccessControlExposeHeaders  = [ "X-Request-ID", "X-Correlation-ID" ]
                                }
                            });

                    }

                    #endregion

                    #region Check location

                    if (!request.ParseMandatoryLocation(CommonAPI,
                                                        //Request.AccessInfo.Value.Roles.Select(role => new Tuple<CountryCode, Party_Id>(role.CountryCode, role.PartyId)),
                                                        CommonAPI.Parties.Select(partyData => new Tuple<CountryCode, Party_Id>(partyData.Id.CountryCode, partyData.Id.PartyId)),
                                                        out var locationId,
                                                        out var location,
                                                        out var ocpiResponseBuilder))
                    {
                        return Task.FromResult(ocpiResponseBuilder);
                    }

                    #endregion


                    return Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               StatusCode           = 1000,
                               StatusMessage        = "Hello world!",
                               Data                 = location.ToJSON(
                                                          request.EMSPId ?? request.HUBId,
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
                                                          CustomEnvironmentalImpactSerializer
                                                      ),
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
                request =>

                    Task.FromResult(
                        new OCPIResponse.Builder(request) {
                            HTTPResponseBuilder = new HTTPResponse.Builder(request.HTTPRequest) {
                                HTTPStatusCode              = HTTPStatusCode.OK,
                                Allow                       = [ HTTPMethod.OPTIONS, HTTPMethod.GET ],
                                AccessControlAllowMethods   = [ "OPTIONS", "GET" ],
                                AccessControlAllowHeaders   = [ "Authorization" ],
                                AccessControlExposeHeaders  = [ "X-Request-ID", "X-Correlation-ID" ]
                            }
                        })

            );

            #endregion

            #region GET      ~/locations/{locationId}/{evseId}

            CommonAPI.AddOCPIMethod(

                HTTPMethod.GET,
                URLPathPrefix + "locations/{locationId}/{evseId}",
                HTTPEvents.GetEVSEHTTPRequest,
                HTTPEvents.GetEVSEHTTPResponse,
                request => {

                    #region Check access token

                    if ((request.LocalAccessInfo is not null || CommonAPI.BaseAPI.LocationsAsOpenData == false) &&
                        (request.LocalAccessInfo?.Status != AccessStatus.ALLOWED ||
                         request.LocalAccessInfo.IsNot(Role.EMSP, Role.HUB) == true))
                    {

                        return Task.FromResult(
                            new OCPIResponse.Builder(request) {
                                StatusCode           = 2000,
                                StatusMessage        = "Invalid or blocked access token!",
                                HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                    HTTPStatusCode              = HTTPStatusCode.Forbidden,
                                    AccessControlAllowMethods   = [ "OPTIONS", "GET" ],
                                    AccessControlAllowHeaders   = [ "Authorization" ],
                                    AccessControlExposeHeaders  = [ "X-Request-ID", "X-Correlation-ID" ]
                                }
                            });

                    }

                    #endregion

                    #region Check EVSE

                    if (!request.ParseMandatoryLocationEVSE(CommonAPI,
                                                            //Request.AccessInfo.Value.Roles.Select(role => new Tuple<CountryCode, Party_Id>(role.CountryCode, role.PartyId)),
                                                            CommonAPI.Parties.Select(partyData => new Tuple<CountryCode, Party_Id>(partyData.Id.CountryCode, partyData.Id.PartyId)),
                                                            out var locationId,
                                                            out var location,
                                                            out var evseId,
                                                            out var evse,
                                                            out var ocpiResponseBuilder))
                    {
                        return Task.FromResult(ocpiResponseBuilder);
                    }

                    #endregion


                    return Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               StatusCode           = 1000,
                               StatusMessage        = "Hello world!",
                               Data                 = evse.ToJSON(
                                                          request.EMSPId ?? request.HUBId,
                                                          CustomEVSESerializer,
                                                          CustomStatusScheduleSerializer,
                                                          CustomConnectorSerializer,
                                                          CustomEVSEEnergyMeterSerializer,
                                                          CustomTransparencySoftwareStatusSerializer,
                                                          CustomTransparencySoftwareSerializer,
                                                          CustomDisplayTextSerializer,
                                                          CustomImageSerializer
                                                      ),
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
                request =>

                    Task.FromResult(
                        new OCPIResponse.Builder(request) {
                            HTTPResponseBuilder = new HTTPResponse.Builder(request.HTTPRequest) {
                                HTTPStatusCode              = HTTPStatusCode.OK,
                                Allow                       = [ HTTPMethod.OPTIONS, HTTPMethod.GET ],
                                AccessControlAllowMethods   = [ "OPTIONS", "GET" ],
                                AccessControlAllowHeaders   = [ "Authorization" ],
                                AccessControlExposeHeaders  = [ "X-Request-ID", "X-Correlation-ID" ]
                            }
                        })

            );

            #endregion

            #region GET      ~/locations/{locationId}/{evseId}/{connectorId}

            CommonAPI.AddOCPIMethod(

                HTTPMethod.GET,
                URLPathPrefix + "locations/{locationId}/{evseId}/{connectorId}",
                HTTPEvents.GetConnectorHTTPRequest,
                HTTPEvents.GetConnectorHTTPResponse,
                request => {

                    #region Check access token

                    if ((request.LocalAccessInfo is not null || CommonAPI.BaseAPI.LocationsAsOpenData == false) &&
                        (request.LocalAccessInfo?.Status != AccessStatus.ALLOWED ||
                         request.LocalAccessInfo.IsNot(Role.EMSP, Role.HUB) == true))
                    {

                        return Task.FromResult(
                            new OCPIResponse.Builder(request) {
                                StatusCode           = 2000,
                                StatusMessage        = "Invalid or blocked access token!",
                                HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                    HTTPStatusCode              = HTTPStatusCode.Forbidden,
                                    AccessControlAllowMethods   = [ "OPTIONS", "GET" ],
                                    AccessControlAllowHeaders   = [ "Authorization" ],
                                    AccessControlExposeHeaders  = [ "X-Request-ID", "X-Correlation-ID" ]
                                }
                            });

                    }

                    #endregion

                    #region Check connector

                    if (!request.ParseMandatoryLocationEVSEConnector(CommonAPI,
                                                                     //Request.AccessInfo.Value.Roles.Select(role => new Tuple<CountryCode, Party_Id>(role.CountryCode, role.PartyId)),
                                                                     CommonAPI.Parties.Select(partyData => new Tuple<CountryCode, Party_Id>(partyData.Id.CountryCode, partyData.Id.PartyId)),
                                                                     out var locationId,
                                                                     out var location,
                                                                     out var evseId,
                                                                     out var evse,
                                                                     out var connectorId,
                                                                     out var connector,
                                                                     out var ocpiResponseBuilder))
                    {
                        return Task.FromResult(ocpiResponseBuilder);
                    }

                    #endregion


                    return Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               StatusCode           = 1000,
                               StatusMessage        = "Hello world!",
                               Data                 = connector.ToJSON(
                                                          true,
                                                          true,
                                                          request.EMSPId ?? request.HUBId,
                                                          CustomConnectorSerializer
                                                      ),
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
                request =>

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

            // https://example.com/ocpi/2.2.1/cpo/tariffs/?date_from=2019-01-28T12:00:00&date_to=2019-01-29T12:00:00&offset=50&limit=100
            CommonAPI.AddOCPIMethod(

                HTTPMethod.GET,
                URLPathPrefix + "tariffs",
                HTTPEvents.GetTariffsHTTPRequest,
                HTTPEvents.GetTariffsHTTPResponse,
                request => {

                    #region Check access token

                    if ((request.LocalAccessInfo is not null || CommonAPI.BaseAPI.TariffsAsOpenData == false) &&
                        (request.LocalAccessInfo?.Status != AccessStatus.ALLOWED ||
                         request.LocalAccessInfo.IsNot(Role.EMSP, Role.HUB) == true))
                    {

                        return Task.FromResult(
                            new OCPIResponse.Builder(request) {
                                StatusCode           = 2000,
                                StatusMessage        = "Invalid or blocked access token!",
                                HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                    HTTPStatusCode              = HTTPStatusCode.Forbidden,
                                    AccessControlAllowMethods   = [ "OPTIONS", "GET" ],
                                    AccessControlAllowHeaders   = [ "Authorization" ],
                                    AccessControlExposeHeaders  = [ "X-Request-ID", "X-Correlation-ID", "Link", "X-Total-Count", "X-Filtered-Count" ]
                                }
                            });

                    }

                    #endregion


                    var filters          = request.GetDateAndPaginationFilters();

                    var allTariffs       = CommonAPI.//GetTariffs(tariff => Request.AccessInfo.Value.Roles.Any(role => role.CountryCode == tariff.CountryCode &&
                                                     //                                                                role.PartyId     == tariff.PartyId)).
                                                     GetTariffs(tariff => CommonAPI.Parties.Any(partyData => partyData.Id.CountryCode == tariff.CountryCode &&
                                                                                                             partyData.Id.PartyId       == tariff.PartyId)).
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
                                                              Select        (tariff => tariff.ToJSON(
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
                                                                                           CustomEnvironmentalImpactSerializer
                                                                                       ))
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
                request =>

                    Task.FromResult(
                        new OCPIResponse.Builder(request) {
                            HTTPResponseBuilder = new HTTPResponse.Builder(request.HTTPRequest) {
                                HTTPStatusCode              = HTTPStatusCode.OK,
                                Allow                       = [ HTTPMethod.OPTIONS, HTTPMethod.GET ],
                                AccessControlAllowMethods   = [ "OPTIONS", "GET" ],
                                AccessControlAllowHeaders   = [ "Authorization" ],
                                AccessControlExposeHeaders  = [ "X-Request-ID", "X-Correlation-ID" ]
                            }
                        })

            );

            #endregion

            #region GET      ~/tariffs/{tariffId}

            CommonAPI.AddOCPIMethod(

                HTTPMethod.GET,
                URLPathPrefix + "tariffs/{tariffId}",
                HTTPEvents.GetTariffHTTPRequest,
                HTTPEvents.GetTariffHTTPResponse,
                request => {

                    #region Check access token

                    if ((request.LocalAccessInfo is not null || CommonAPI.BaseAPI.TariffsAsOpenData == false) &&
                        (request.LocalAccessInfo?.Status != AccessStatus.ALLOWED ||
                         request.LocalAccessInfo.IsNot(Role.EMSP, Role.HUB) == true))
                    {

                        return Task.FromResult(
                            new OCPIResponse.Builder(request) {
                                StatusCode           = 2000,
                                StatusMessage        = "Invalid or blocked access token!",
                                HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                    HTTPStatusCode              = HTTPStatusCode.Forbidden,
                                    AccessControlAllowMethods   = [ "OPTIONS", "GET" ],
                                    AccessControlAllowHeaders   = [ "Authorization" ],
                                    AccessControlExposeHeaders  = [ "X-Request-ID", "X-Correlation-ID" ]
                                }
                            });

                    }

                    #endregion

                    #region Check tariff

                    if (!request.ParseMandatoryTariff(CommonAPI,
                                                      //Request.AccessInfo.Value.Roles.Select(role => new Tuple<CountryCode, Party_Id>(role.CountryCode, role.PartyId)),
                                                      CommonAPI.Parties.Select(partyData => new Tuple<CountryCode, Party_Id>(partyData.Id.CountryCode, partyData.Id.PartyId)),
                                                      out var tariffId,
                                                      out var tariff,
                                                      out var ocpiResponseBuilder))
                    {
                        return Task.FromResult(ocpiResponseBuilder);
                    }

                    #endregion


                    return Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               StatusCode           = 1000,
                               StatusMessage        = "Hello world!",
                               Data                 = tariff.ToJSON(
                                                          false, //IncludeOwnerInformation
                                                          false, //IncludeExtensions
                                                          CustomTariffSerializer,
                                                          CustomDisplayTextSerializer,
                                                          CustomPriceSerializer,
                                                          CustomTariffElementSerializer,
                                                          CustomPriceComponentSerializer,
                                                          CustomTariffRestrictionsSerializer,
                                                          CustomEnergyMixSerializer,
                                                          CustomEnergySourceSerializer,
                                                          CustomEnvironmentalImpactSerializer
                                                      ),
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
                request =>

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
                HTTPEvents.GetSessionsHTTPRequest,
                HTTPEvents.GetSessionsHTTPResponse,
                request => {

                    #region Check access token

                    if (request.LocalAccessInfo is null ||
                        request.LocalAccessInfo.Status != AccessStatus.ALLOWED ||
                        request.LocalAccessInfo.IsNot(Role.EMSP, Role.HUB) == true)
                    {

                        return Task.FromResult(
                            new OCPIResponse.Builder(request) {
                                StatusCode           = 2000,
                                StatusMessage        = "Invalid or blocked access token!",
                                HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                    HTTPStatusCode              = HTTPStatusCode.Forbidden,
                                    AccessControlAllowMethods   = [ "OPTIONS", "GET" ],
                                    AccessControlAllowHeaders   = [ "Authorization" ],
                                    AccessControlExposeHeaders  = [ "X-Request-ID", "X-Correlation-ID" ]
                                }
                            });

                    }

                    #endregion


                    var filters              = request.GetDateAndPaginationFilters();

                    var allSessions          = CommonAPI.//GetSessions(session => Request.AccessInfo.Value.Roles.Any(role => role.CountryCode == session.CountryCode &&
                                                         //                                                                  role.PartyId     == session.PartyId)).
                                                         GetSessions(session => CommonAPI.Parties.Any(partyData => partyData.Id.CountryCode == session.CountryCode &&
                                                                                                                   partyData.Id.PartyId       == session.PartyId)).
                                                         ToArray();

                    var filteredSessions     = allSessions.Where(session => !filters.From.HasValue || session.LastUpdated >  filters.From.Value).
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
                                                                  Select        (session => session.ToJSON(
                                                                                                CustomSessionSerializer,
                                                                                                CustomCDRTokenSerializer,
                                                                                                CustomChargingPeriodSerializer,
                                                                                                CustomCDRDimensionSerializer,
                                                                                                CustomPriceSerializer
                                                                                            ))
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
                request =>

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
                HTTPEvents.GetSessionHTTPRequest,
                HTTPEvents.GetSessionHTTPResponse,
                request => {

                    #region Check access token

                    if (request.LocalAccessInfo is null ||
                        request.LocalAccessInfo.Status != AccessStatus.ALLOWED ||
                        request.LocalAccessInfo.IsNot(Role.EMSP, Role.HUB) == true)
                    {

                        return Task.FromResult(
                            new OCPIResponse.Builder(request) {
                                StatusCode           = 2000,
                                StatusMessage        = "Invalid or blocked access token!",
                                HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                    HTTPStatusCode              = HTTPStatusCode.Forbidden,
                                    AccessControlAllowMethods   = [ "OPTIONS", "GET" ],
                                    AccessControlAllowHeaders   = [ "Authorization" ],
                                    AccessControlExposeHeaders  = [ "X-Request-ID", "X-Correlation-ID" ]
                                }
                            });

                    }

                    #endregion

                    #region Check session

                    if (!request.ParseMandatorySession(CommonAPI,
                                                       //Request.AccessInfo.Value.Roles.Select(role => new Tuple<CountryCode, Party_Id>(role.CountryCode, role.PartyId)),
                                                       CommonAPI.Parties.Select(partyData => new Tuple<CountryCode, Party_Id>(partyData.Id.CountryCode, partyData.Id.PartyId)),
                                                       out var sessionId,
                                                       out var session,
                                                       out var ocpiResponseBuilder))
                    {
                        return Task.FromResult(ocpiResponseBuilder);
                    }

                    #endregion


                    return Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               StatusCode           = 1000,
                               StatusMessage        = "Hello world!",
                               Data                 = session.ToJSON(
                                                          CustomSessionSerializer,
                                                          CustomCDRTokenSerializer,
                                                          CustomChargingPeriodSerializer,
                                                          CustomCDRDimensionSerializer,
                                                          CustomPriceSerializer
                                                      ),
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

            #region GET      ~/chargingprofiles/{session_id}?duration={duration}&response_url=https://client.com/12345/

            // 1. GET will just return a ChargingProfileResponse (result=ACCEPTED).
            // 2. The resposeURL will be called with a ActiveProfileResult object (result=ACCEPTED, ActiveChargingProfile).

            // NOTE: This GET requests introduces state and thus is a VIOLATION of HTTP semantics!

            #endregion

            #region PUT      ~/chargingprofiles/{session_id}?response_url=https://client.com/12345/

            // 1. PUT (with a resposeURL): SetChargingProfile object
            // 2. The resposeURL will be called later, e.g. POST https://client.com/12345/ with a ChargingProfileResult object.

            #endregion

            #region DELETE   ~/chargingprofiles/{session_id}?response_url=https://client.com/12345/

            // 1. DELETE will just return a ChargingProfileResponse (result=ACCEPTED).
            // 2. The resposeURL will be called with a ClearProfileResult object (result=ACCEPTED).

            #endregion

            #endregion


            #region ~/sessions/{session_id}/charging_preferences <= Yet to do!

            //ToDo: Implement ~/sessions/{session_id}/charging_preferences!

            #region PUT      ~/sessions/{session_id}/charging_preferences

            // https://example.com/ocpi/2.2/cpo/sessions/12454/charging_preferences

            #endregion

            #endregion


            #region ~/cdrs

            #region OPTIONS  ~/cdrs

            CommonAPI.AddOCPIMethod(

                HTTPMethod.OPTIONS,
                URLPathPrefix + "CDRs",
                request =>

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
                HTTPEvents.GetCDRsHTTPRequest,
                HTTPEvents.GetCDRsHTTPResponse,
                request => {

                    #region Check access token

                    if (request.LocalAccessInfo is null ||
                        request.LocalAccessInfo.Status != AccessStatus.ALLOWED ||
                        request.LocalAccessInfo.IsNot(Role.EMSP, Role.HUB) == true)
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

                    var allCDRs              = CommonAPI.//GetCDRs(cdr => Request.AccessInfo.Value.Roles.Any(role => role.CountryCode == cdr.CountryCode &&
                                                         //                                                          role.PartyId     == cdr.PartyId)).
                                                         GetCDRs(cdr => CommonAPI.Parties.Any(partyData => partyData.Id.CountryCode == cdr.CountryCode &&
                                                                                                           partyData.Id.PartyId       == cdr.PartyId)).
                                                         ToArray();

                    var filteredCDRs         = allCDRs.Where(CDR => !filters.From.HasValue || CDR.LastUpdated >  filters.From.Value).
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
                                                              Select        (cdr => cdr.ToJSON(
                                                                                        CustomCDRSerializer,
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
                                                                                        CustomSignedValueSerializer
                                                                                    ))
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
                request =>

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
                HTTPEvents.GetCDRHTTPRequest,
                HTTPEvents.GetCDRHTTPResponse,
                request => {

                    #region Check access token

                    if (request.LocalAccessInfo is null ||
                        request.LocalAccessInfo.Status != AccessStatus.ALLOWED ||
                        request.LocalAccessInfo.IsNot(Role.EMSP, Role.HUB) == true)
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

                    if (!request.ParseMandatoryCDR(CommonAPI,
                                                   //Request.AccessInfo.Value.Roles.Select(role => new Tuple<CountryCode, Party_Id>(role.CountryCode, role.PartyId)),
                                                   CommonAPI.Parties.Select(partyData => new Tuple<CountryCode, Party_Id>(partyData.Id.CountryCode, partyData.Id.PartyId)),
                                                   out var cdrId,
                                                   out var cdr,
                                                   out var ocpiResponseBuilder))
                    {
                        return Task.FromResult(ocpiResponseBuilder);
                    }

                    #endregion


                    return Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               StatusCode           = 1000,
                               StatusMessage        = "Hello world!",
                               Data                 = cdr.ToJSON(
                                                          CustomCDRSerializer,
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
                                                          CustomSignedValueSerializer
                                                      ),
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
                request =>

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
                HTTPEvents.GetTokensHTTPRequest,
                HTTPEvents.GetTokensHTTPResponse,
                request => {

                    #region Check access token

                    if (request.LocalAccessInfo is null ||
                        request.LocalAccessInfo.Status != AccessStatus.ALLOWED ||
                        request.LocalAccessInfo.IsNot(Role.EMSP, Role.HUB) == true)
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


                    var filters              = request.GetDateAndPaginationFilters();

                    var allTokens            = CommonAPI.GetTokenStatus(partyId.Value).ToArray();

                    var filteredTokens       = allTokens.Select(tokenStatus => tokenStatus.Token).
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
                                                                 : $"http://127.0.0.1:{CommonAPI.BaseAPI.HTTPBaseAPI.HTTPServer.TCPPort}")}{URLPathPrefix}/tokens/{partyId.Value.CountryCode}/{partyId.Value.PartyId}{queryParameters}>; rel=\"next\"");

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
                                                                  Select        (token => token.ToJSON(
                                                                                              CustomTokenSerializer,
                                                                                              CustomEnergyContractSerializer
                                                                                          ))
                                                          )
                               }
                           );

                });

            #endregion

            #region DELETE   ~/tokens/{country_code}/{party_id}      [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPMethod.DELETE,
                URLPathPrefix + "tokens/{country_code}/{party_id}",
                HTTPEvents.DeleteTokensHTTPRequest,
                HTTPEvents.DeleteTokensHTTPResponse,
                async request => {

                    #region Check access token

                    if (request.LocalAccessInfo is null ||
                        request.LocalAccessInfo.Status != AccessStatus.ALLOWED ||
                        request.LocalAccessInfo.IsNot(Role.EMSP, Role.HUB) == true)
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
                request =>

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

            #region GET      ~/tokens/{country_code}/{party_id}/{tokenId}?type={type}

            CommonAPI.AddOCPIMethod(

                HTTPMethod.GET,
                URLPathPrefix + "tokens/{country_code}/{party_id}/{tokenId}",
                HTTPEvents.GetTokenHTTPRequest,
                HTTPEvents.GetTokenHTTPResponse,
                request => {

                    #region Check access token

                    if (request.LocalAccessInfo is null ||
                        request.LocalAccessInfo.Status != AccessStatus.ALLOWED ||
                        request.LocalAccessInfo.IsNot(Role.EMSP, Role.HUB) == true)
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
                                                     request.LocalAccessInfo.Roles.Select(role => role.PartyId),
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
                               Data                 = tokenStatus.Token.ToJSON(
                                                          CustomTokenSerializer,
                                                          CustomEnergyContractSerializer
                                                      ),
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

            #region PUT      ~/tokens/{country_code}/{party_id}/{tokenId}?type={type}

            CommonAPI.AddOCPIMethod(

                HTTPMethod.PUT,
                URLPathPrefix + "tokens/{country_code}/{party_id}/{tokenId}",
                HTTPEvents.PutTokenHTTPRequest,
                HTTPEvents.PutTokenHTTPResponse,
                async request => {

                    #region Check access token

                    if (request.LocalAccessInfo is null ||
                        request.LocalAccessInfo.Status != AccessStatus.ALLOWED ||
                        request.LocalAccessInfo.IsNot(Role.EMSP, Role.HUB) == true)
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

                    if (!request.ParseTokenId(out var countryCode,
                                              out var partyId,
                                              out var tokenId,
                                              out var ocpiResponseBuilder))
                    {
                        return ocpiResponseBuilder;
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
                                        tokenId))
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


                    if (addOrUpdateResult.IsSuccessAndDataNotNull(out var tokenData))
                        return new OCPIResponse.Builder(request) {
                                   StatusCode           = 1000,
                                   StatusMessage        = "Hello world!",
                                   Data                 = tokenData.Token.ToJSON(
                                                              CustomTokenSerializer,
                                                              CustomEnergyContractSerializer
                                                          ),
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                                              HTTPStatusCode             = addOrUpdateResult.WasCreated == true
                                                                                               ? HTTPStatusCode.Created
                                                                                               : HTTPStatusCode.OK,
                                                              AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                              AccessControlAllowHeaders  = [ "Authorization" ],
                                                              LastModified               = tokenData.Token.LastUpdated,
                                                              ETag                       = tokenData.Token.ETag
                                                          }
                               };

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

            #region PATCH    ~/tokens/{country_code}/{party_id}/{tokenId}?type={type}

            CommonAPI.AddOCPIMethod(

                HTTPMethod.PATCH,
                URLPathPrefix + "tokens/{country_code}/{party_id}/{tokenId}",
                HTTPEvents.PatchTokenHTTPRequest,
                HTTPEvents.PatchTokenHTTPResponse,
                async request => {

                    #region Check access token

                    if (request.LocalAccessInfo is null ||
                        request.LocalAccessInfo.Status != AccessStatus.ALLOWED ||
                        request.LocalAccessInfo.IsNot(Role.EMSP, Role.HUB) == true)
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
                                                     request.LocalAccessInfo.Roles.Select(role => role.PartyId),
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


                    if (patchedToken.IsSuccessAndDataNotNull(out var tokenData))
                        return new OCPIResponse.Builder(request) {
                                       StatusCode           = 1000,
                                       StatusMessage        = "Hello world!",
                                       Data                 = tokenData.ToJSON(
                                                                  CustomTokenSerializer,
                                                                  CustomEnergyContractSerializer
                                                              ),
                            HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                           HTTPStatusCode             = HTTPStatusCode.OK,
                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                           AccessControlAllowHeaders  = [ "Authorization" ],
                                           LastModified               = tokenData.LastUpdated,
                                           ETag                       = tokenData.ETag
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

            #region DELETE   ~/tokens/{country_code}/{party_id}/{tokenId}      [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPMethod.DELETE,
                URLPathPrefix + "tokens/{country_code}/{party_id}/{tokenId}",
                HTTPEvents.DeleteTokenHTTPRequest,
                HTTPEvents.DeleteTokenHTTPResponse,
                async request => {

                    #region Check access token

                    if (request.LocalAccessInfo is null ||
                        request.LocalAccessInfo.Status != AccessStatus.ALLOWED ||
                        request.LocalAccessInfo.IsNot(Role.EMSP, Role.HUB) == true)
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
                                                     request.LocalAccessInfo.Roles.Select(role => role.PartyId),
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
                    var tokenType  = request.QueryString.TryParseEnum<TokenType>("type") ?? TokenType.RFID;

                    var result     = await CommonAPI.RemoveToken(existingTokenStatus.Token);

                    if (result.IsSuccessAndDataNotNull(out var tokenStatus))
                        return new OCPIResponse.Builder(request) {
                                   StatusCode           = 1000,
                                   StatusMessage        = "Hello world!",
                                   Data                 = tokenStatus.Token.ToJSON(
                                                              CustomTokenSerializer,
                                                              CustomEnergyContractSerializer
                                                          ),
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
                                   Data                 = existingTokenStatus.Token.ToJSON(
                                                              CustomTokenSerializer,
                                                              CustomEnergyContractSerializer
                                                          ),
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



            // Commands

            #region ~/commands/RESERVE_NOW

            #region OPTIONS  ~/commands/RESERVE_NOW

            CommonAPI.AddOCPIMethod(

                HTTPMethod.OPTIONS,
                URLPathPrefix + "commands/RESERVE_NOW",
                request =>

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
                HTTPEvents.ReserveNowHTTPRequest,
                HTTPEvents.ReserveNowHTTPResponse,
                async request => {

                    #region Check access token

                    if (request.RemoteParty     is null ||
                        request.LocalAccessInfo is null ||
                        request.LocalAccessInfo.Status != AccessStatus.ALLOWED ||
                        request.LocalAccessInfo.IsNot(Role.EMSP, Role.HUB) == true)
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
                                                    out var errorResponse))
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
                        commandResponse = await OnReserveNowCommand.Invoke(
                                                    request.RemoteParty.Id,
                                                    request.From.AsEMSPId(),
                                                    request.To.  AsCPOId(),
                                                    reserveNowCommand
                                                );

                    commandResponse ??= new CommandResponse(
                                            reserveNowCommand,
                                            CommandResponseTypes.NOT_SUPPORTED,
                                            Timeout: TimeSpan.FromSeconds(15),
                                            Message: [ new DisplayText(Languages.en, "Not supported!") ]
                                        );


                    return new OCPIResponse.Builder(request) {
                               StatusCode           = 1000,
                               StatusMessage        = "Hello world!",
                               Data                 = commandResponse.ToJSON(
                                                          CustomCommandResponseSerializer,
                                                          CustomDisplayTextSerializer
                                                      ),
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
                request =>

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
                HTTPEvents.CancelReservationHTTPRequest,
                HTTPEvents.CancelReservationHTTPResponse,
                async request => {

                    #region Check access token

                    if (request.RemoteParty     is null ||
                        request.LocalAccessInfo is null ||
                        request.LocalAccessInfo.Status != AccessStatus.ALLOWED ||
                        request.LocalAccessInfo.IsNot(Role.EMSP, Role.HUB) == true)
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
                                                           out var errorResponse))
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
                        commandResponse = await OnCancelReservationCommand.Invoke(
                                                    request.RemoteParty.Id,
                                                    request.From.AsEMSPId(),
                                                    request.To.  AsCPOId(),
                                                    cancelReservationCommand
                                                );

                    commandResponse ??= new CommandResponse(
                                            cancelReservationCommand,
                                            CommandResponseTypes.NOT_SUPPORTED,
                                            Timeout: TimeSpan.FromSeconds(15),
                                            Message: [ new DisplayText(Languages.en, "Not supported!") ]
                                        );


                    return new OCPIResponse.Builder(request) {
                               StatusCode           = 1000,
                               StatusMessage        = "Hello world!",
                               Data                 = commandResponse.ToJSON(
                                                          CustomCommandResponseSerializer,
                                                          CustomDisplayTextSerializer
                                                      ),
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
                request =>

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
                HTTPEvents.StartSessionHTTPRequest,
                HTTPEvents.StartSessionHTTPResponse,
                async request => {

                    #region Check access token

                    if (request.RemoteParty     is null ||
                        request.LocalAccessInfo is null ||
                        request.LocalAccessInfo.Status != AccessStatus.ALLOWED ||
                        request.LocalAccessInfo.IsNot(Role.EMSP, Role.HUB) == true)
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
                                                      out var errorResponse))
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
                        commandResponse = await OnStartSessionCommand.Invoke(
                                                    request.RemoteParty.Id,
                                                    request.From.AsEMSPId(),
                                                    request.To.  AsCPOId(),
                                                    startSessionCommand
                                                );

                    commandResponse ??= new CommandResponse(
                                            startSessionCommand,
                                            CommandResponseTypes.NOT_SUPPORTED,
                                            Timeout: TimeSpan.FromSeconds(15),
                                            Message: [ new DisplayText(Languages.en, "Not supported!") ]
                                        );


                    return new OCPIResponse.Builder(request) {
                               StatusCode           = 1000,
                               StatusMessage        = "Hello world!",
                               Data                 = commandResponse.ToJSON(
                                                          CustomCommandResponseSerializer,
                                                          CustomDisplayTextSerializer
                                                      ),
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
                request =>

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
                HTTPEvents.StopSessionHTTPRequest,
                HTTPEvents.StopSessionHTTPResponse,
                async request => {

                    #region Check access token

                    if (request.RemoteParty     is null ||
                        request.LocalAccessInfo is null ||
                        request.LocalAccessInfo.Status != AccessStatus.ALLOWED ||
                        request.LocalAccessInfo.IsNot(Role.EMSP, Role.HUB) == true)
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
                                                     out var errorResponse))
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
                        commandResponse = await OnStopSessionCommand.Invoke(
                                                    request.RemoteParty.Id,
                                                    request.From.AsEMSPId(),
                                                    request.To.  AsCPOId(),
                                                    stopSessionCommand
                                                );

                    commandResponse ??= new CommandResponse(
                                            stopSessionCommand,
                                            CommandResponseTypes.NOT_SUPPORTED,
                                            Timeout: TimeSpan.FromSeconds(15),
                                            Message: [ new DisplayText(Languages.en, "Not supported!") ]
                                        );


                    return new OCPIResponse.Builder(request) {
                               StatusCode           = 1000,
                               StatusMessage        = "Hello world!",
                               Data                 = commandResponse.ToJSON(
                                                          CustomCommandResponseSerializer,
                                                          CustomDisplayTextSerializer
                                                      ),
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
                request =>

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
                HTTPEvents.UnlockConnectorHTTPRequest,
                HTTPEvents.UnlockConnectorHTTPResponse,
                async request => {

                    #region Check access token

                    if (request.RemoteParty     is null ||
                        request.LocalAccessInfo is null ||
                        request.LocalAccessInfo.Status != AccessStatus.ALLOWED ||
                        request.LocalAccessInfo.IsNot(Role.EMSP, Role.HUB) == true)
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
                                                         out var errorResponse))
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
                        commandResponse = await OnUnlockConnectorCommand.Invoke(
                                                    request.RemoteParty.Id,
                                                    request.From.AsEMSPId(),
                                                    request.To.  AsCPOId(),
                                                    unlockConnectorCommand
                                                );

                    commandResponse ??= new CommandResponse(
                                            unlockConnectorCommand,
                                            CommandResponseTypes.NOT_SUPPORTED,
                                            Timeout: TimeSpan.FromSeconds(15),
                                            Message: [ new DisplayText(Languages.en, "Not supported!") ]
                                        );


                    return new OCPIResponse.Builder(request) {
                               StatusCode           = 1000,
                               StatusMessage        = "Hello world!",
                               Data                 = commandResponse.ToJSON(
                                                          CustomCommandResponseSerializer,
                                                          CustomDisplayTextSerializer
                                                      ),
                               HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
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
