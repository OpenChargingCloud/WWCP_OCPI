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
    /// Extension methods for the SCSP HTTP API.
    /// </summary>
    public static class SCSPAPIExtensions
    {

    }


    /// <summary>
    /// The HTTP API for e-mobility service providers.
    /// CPOs will connect to this API.
    /// </summary>
    public class SCSPAPI : HTTPAPI
    {

        #region Data

        /// <summary>
        /// The default HTTP server name.
        /// </summary>
        public new const           String    DefaultHTTPServerName     = "GraphDefined OCPI SCSP HTTP API v0.1";

        /// <summary>
        /// The default HTTP server name.
        /// </summary>
        public new const           String    DefaultHTTPServiceName    = "GraphDefined OCPI SCSP HTTP API v0.1";

        /// <summary>
        /// The default HTTP server TCP port.
        /// </summary>
        public new static readonly IPPort    DefaultHTTPServerPort     = IPPort.Parse(8080);

        /// <summary>
        /// The default HTTP URL path prefix.
        /// </summary>
        public new static readonly HTTPPath  DefaultURLPathPrefix      = HTTPPath.Parse("/emsp");

        protected Newtonsoft.Json.Formatting JSONFormat = Newtonsoft.Json.Formatting.Indented;

        #endregion

        #region Properties

        /// <summary>
        /// The CommonAPI.
        /// </summary>
        public CommonAPI       CommonAPI             { get; }

        /// <summary>
        /// The default country code to use.
        /// </summary>
        public CountryCode     DefaultCountryCode    { get; }

        /// <summary>
        /// The default party identification to use.
        /// </summary>
        public Party_Id        DefaultPartyId        { get; }

        /// <summary>
        /// (Dis-)allow PUTting of object having an earlier 'LastUpdated'-timestamp then already existing objects.
        /// OCPI v2.2 does not define any behaviour for this.
        /// </summary>
        public Boolean?        AllowDowngrades       { get; }

                /// <summary>
        /// The timeout for upstream requests.
        /// </summary>
        public TimeSpan        RequestTimeout        { get; set; }

        /// <summary>
        /// The SCSP API logger.
        /// </summary>
        public SCSPAPILogger?  SCSPAPILogger         { get; }

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

        #region Locations

        #region (protected internal) GetLocationsRequest    (Request)

        /// <summary>
        /// An event sent whenever a GET locations request was received.
        /// </summary>
        public OCPIRequestLogEvent OnGetLocationsRequest = new ();

        /// <summary>
        /// An event sent whenever a GET locations request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The SCSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task GetLocationsRequest(DateTimeOffset     Timestamp,
                                                    HTTPAPI      API,
                                                    OCPIRequest  Request)

            => OnGetLocationsRequest?.WhenAll(Timestamp,
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
        /// <param name="API">The SCSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task GetLocationsResponse(DateTimeOffset      Timestamp,
                                                     HTTPAPI       API,
                                                     OCPIRequest   Request,
                                                     OCPIResponse  Response)

            => OnGetLocationsResponse?.WhenAll(Timestamp,
                                               API ?? this,
                                               Request,
                                               Response);

        #endregion


        #region (protected internal) DeleteLocationsRequest (Request)

        /// <summary>
        /// An event sent whenever a delete locations request was received.
        /// </summary>
        public OCPIRequestLogEvent OnDeleteLocationsRequest = new ();

        /// <summary>
        /// An event sent whenever a delete locations request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The SCSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task DeleteLocationsRequest(DateTimeOffset     Timestamp,
                                                       HTTPAPI      API,
                                                       OCPIRequest  Request)

            => OnDeleteLocationsRequest?.WhenAll(Timestamp,
                                                 API ?? this,
                                                 Request);

        #endregion

        #region (protected internal) DeleteLocationsResponse(Response)

        /// <summary>
        /// An event sent whenever a delete locations response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnDeleteLocationsResponse = new ();

        /// <summary>
        /// An event sent whenever a delete locations response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The SCSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task DeleteLocationsResponse(DateTimeOffset      Timestamp,
                                                        HTTPAPI       API,
                                                        OCPIRequest   Request,
                                                        OCPIResponse  Response)

            => OnDeleteLocationsResponse?.WhenAll(Timestamp,
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
        /// <param name="API">The SCSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task GetLocationRequest(DateTimeOffset     Timestamp,
                                                   HTTPAPI      API,
                                                   OCPIRequest  Request)

            => OnGetLocationRequest?.WhenAll(Timestamp,
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
        /// <param name="API">The SCSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task GetLocationResponse(DateTimeOffset      Timestamp,
                                                    HTTPAPI       API,
                                                    OCPIRequest   Request,
                                                    OCPIResponse  Response)

            => OnGetLocationResponse?.WhenAll(Timestamp,
                                              API ?? this,
                                              Request,
                                              Response);

        #endregion


        #region (protected internal) PutLocationRequest    (Request)

        /// <summary>
        /// An event sent whenever a PUT location request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPutLocationRequest = new ();

        /// <summary>
        /// An event sent whenever a PUT location request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The SCSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PutLocationRequest(DateTimeOffset     Timestamp,
                                                   HTTPAPI      API,
                                                   OCPIRequest  Request)

            => OnPutLocationRequest?.WhenAll(Timestamp,
                                             API ?? this,
                                             Request);

        #endregion

        #region (protected internal) PutLocationResponse   (Response)

        /// <summary>
        /// An event sent whenever a PUT location response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnPutLocationResponse = new ();

        /// <summary>
        /// An event sent whenever a PUT location response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The SCSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PutLocationResponse(DateTimeOffset      Timestamp,
                                                    HTTPAPI       API,
                                                    OCPIRequest   Request,
                                                    OCPIResponse  Response)

            => OnPutLocationResponse?.WhenAll(Timestamp,
                                              API ?? this,
                                              Request,
                                              Response);

        #endregion


        #region (protected internal) PatchLocationRequest  (Request)

        /// <summary>
        /// An event sent whenever a PATCH location request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPatchLocationRequest = new ();

        /// <summary>
        /// An event sent whenever a PATCH location request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The SCSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PatchLocationRequest(DateTimeOffset     Timestamp,
                                                     HTTPAPI      API,
                                                     OCPIRequest  Request)

            => OnPatchLocationRequest?.WhenAll(Timestamp,
                                               API ?? this,
                                               Request);

        #endregion

        #region (protected internal) PatchLocationResponse (Response)

        /// <summary>
        /// An event sent whenever a PATCH location response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnPatchLocationResponse = new ();

        /// <summary>
        /// An event sent whenever a PATCH location response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The SCSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PatchLocationResponse(DateTimeOffset      Timestamp,
                                                      HTTPAPI       API,
                                                      OCPIRequest   Request,
                                                      OCPIResponse  Response)

            => OnPatchLocationResponse?.WhenAll(Timestamp,
                                                API ?? this,
                                                Request,
                                                Response);

        #endregion


        #region (protected internal) DeleteLocationRequest (Request)

        /// <summary>
        /// An event sent whenever a DELETE location request was received.
        /// </summary>
        public OCPIRequestLogEvent OnDeleteLocationRequest = new ();

        /// <summary>
        /// An event sent whenever a DELETE location request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The SCSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task DeleteLocationRequest(DateTimeOffset     Timestamp,
                                                      HTTPAPI      API,
                                                      OCPIRequest  Request)

            => OnDeleteLocationRequest?.WhenAll(Timestamp,
                                                API ?? this,
                                                Request);

        #endregion

        #region (protected internal) DeleteLocationResponse(Response)

        /// <summary>
        /// An event sent whenever a DELETE location response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnDeleteLocationResponse = new ();

        /// <summary>
        /// An event sent whenever a DELETE location response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The SCSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task DeleteLocationResponse(DateTimeOffset      Timestamp,
                                                       HTTPAPI       API,
                                                       OCPIRequest   Request,
                                                       OCPIResponse  Response)

            => OnDeleteLocationResponse?.WhenAll(Timestamp,
                                                 API ?? this,
                                                 Request,
                                                 Response);

        #endregion

        #endregion

        #region EVSEs

        #region (protected internal) GetEVSERequest    (Request)

        /// <summary>
        /// An event sent whenever a GET EVSE request was received.
        /// </summary>
        public OCPIRequestLogEvent OnGetEVSERequest = new ();

        /// <summary>
        /// An event sent whenever a GET EVSE request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The SCSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task GetEVSERequest(DateTimeOffset     Timestamp,
                                               HTTPAPI      API,
                                               OCPIRequest  Request)

            => OnGetEVSERequest?.WhenAll(Timestamp,
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
        /// <param name="API">The SCSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task GetEVSEResponse(DateTimeOffset      Timestamp,
                                                HTTPAPI       API,
                                                OCPIRequest   Request,
                                                OCPIResponse  Response)

            => OnGetEVSEResponse?.WhenAll(Timestamp,
                                          API ?? this,
                                          Request,
                                          Response);

        #endregion


        #region (protected internal) PutEVSERequest    (Request)

        /// <summary>
        /// An event sent whenever a PUT EVSE request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPutEVSERequest = new ();

        /// <summary>
        /// An event sent whenever a PUT EVSE request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The SCSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PutEVSERequest(DateTimeOffset     Timestamp,
                                               HTTPAPI      API,
                                               OCPIRequest  Request)

            => OnPutEVSERequest?.WhenAll(Timestamp,
                                         API ?? this,
                                         Request);

        #endregion

        #region (protected internal) PutEVSEResponse   (Response)

        /// <summary>
        /// An event sent whenever a PUT EVSE response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnPutEVSEResponse = new ();

        /// <summary>
        /// An event sent whenever a PUT EVSE response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The SCSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PutEVSEResponse(DateTimeOffset      Timestamp,
                                                HTTPAPI       API,
                                                OCPIRequest   Request,
                                                OCPIResponse  Response)

            => OnPutEVSEResponse?.WhenAll(Timestamp,
                                          API ?? this,
                                          Request,
                                          Response);

        #endregion


        #region (protected internal) PatchEVSERequest  (Request)

        /// <summary>
        /// An event sent whenever a PATCH EVSE request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPatchEVSERequest = new ();

        /// <summary>
        /// An event sent whenever a PATCH EVSE request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The SCSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PatchEVSERequest(DateTimeOffset     Timestamp,
                                                 HTTPAPI      API,
                                                 OCPIRequest  Request)

            => OnPatchEVSERequest?.WhenAll(Timestamp,
                                           API ?? this,
                                           Request);

        #endregion

        #region (protected internal) PatchEVSEResponse (Response)

        /// <summary>
        /// An event sent whenever a PATCH EVSE response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnPatchEVSEResponse = new ();

        /// <summary>
        /// An event sent whenever a PATCH EVSE response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The SCSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PatchEVSEResponse(DateTimeOffset      Timestamp,
                                                  HTTPAPI       API,
                                                  OCPIRequest   Request,
                                                  OCPIResponse  Response)

            => OnPatchEVSEResponse?.WhenAll(Timestamp,
                                            API ?? this,
                                            Request,
                                            Response);

        #endregion


        #region (protected internal) DeleteEVSERequest (Request)

        /// <summary>
        /// An event sent whenever a DELETE EVSE request was received.
        /// </summary>
        public OCPIRequestLogEvent OnDeleteEVSERequest = new ();

        /// <summary>
        /// An event sent whenever a DELETE EVSE request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The SCSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task DeleteEVSERequest(DateTimeOffset     Timestamp,
                                                  HTTPAPI      API,
                                                  OCPIRequest  Request)

            => OnDeleteEVSERequest?.WhenAll(Timestamp,
                                            API ?? this,
                                            Request);

        #endregion

        #region (protected internal) DeleteEVSEResponse(Response)

        /// <summary>
        /// An event sent whenever a DELETE EVSE response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnDeleteEVSEResponse = new ();

        /// <summary>
        /// An event sent whenever a DELETE EVSE response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The SCSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task DeleteEVSEResponse(DateTimeOffset      Timestamp,
                                                   HTTPAPI       API,
                                                   OCPIRequest   Request,
                                                   OCPIResponse  Response)

            => OnDeleteEVSEResponse?.WhenAll(Timestamp,
                                             API ?? this,
                                             Request,
                                             Response);

        #endregion



        #region (protected internal) OnPostEVSEStatusRequest    (Request)

        /// <summary>
        /// An event sent whenever a POST EVSE status request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPostEVSEStatusRequest = new ();

        /// <summary>
        /// An event sent whenever a POST EVSE status request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The SCSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PostEVSEStatusRequest(DateTimeOffset     Timestamp,
                                                      HTTPAPI      API,
                                                      OCPIRequest  Request)

            => OnPostEVSEStatusRequest?.WhenAll(Timestamp,
                                                API ?? this,
                                                Request);

        #endregion

        #region (protected internal) PostEVSEStatusResponse   (Response)

        /// <summary>
        /// An event sent whenever a POST EVSE status response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnPostEVSEStatusResponse = new ();

        /// <summary>
        /// An event sent whenever a POST EVSE status response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The SCSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PostEVSEStatusResponse(DateTimeOffset      Timestamp,
                                                       HTTPAPI       API,
                                                       OCPIRequest   Request,
                                                       OCPIResponse  Response)

            => OnPostEVSEStatusResponse?.WhenAll(Timestamp,
                                                 API ?? this,
                                                 Request,
                                                 Response);

        #endregion

        #endregion

        #region Connectors

        #region (protected internal) GetConnectorRequest    (Request)

        /// <summary>
        /// An event sent whenever a GET connector request was received.
        /// </summary>
        public OCPIRequestLogEvent OnGetConnectorRequest = new ();

        /// <summary>
        /// An event sent whenever a GET connector request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The SCSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task GetConnectorRequest(DateTimeOffset     Timestamp,
                                                    HTTPAPI      API,
                                                    OCPIRequest  Request)

            => OnGetConnectorRequest?.WhenAll(Timestamp,
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
        /// <param name="API">The SCSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task GetConnectorResponse(DateTimeOffset      Timestamp,
                                                     HTTPAPI       API,
                                                     OCPIRequest   Request,
                                                     OCPIResponse  Response)

            => OnGetConnectorResponse?.WhenAll(Timestamp,
                                               API ?? this,
                                               Request,
                                               Response);

        #endregion


        #region (protected internal) PutConnectorRequest    (Request)

        /// <summary>
        /// An event sent whenever a PUT connector request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPutConnectorRequest = new ();

        /// <summary>
        /// An event sent whenever a PUT connector request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The SCSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PutConnectorRequest(DateTimeOffset     Timestamp,
                                                    HTTPAPI      API,
                                                    OCPIRequest  Request)

            => OnPutConnectorRequest?.WhenAll(Timestamp,
                                              API ?? this,
                                              Request);

        #endregion

        #region (protected internal) PutConnectorResponse   (Response)

        /// <summary>
        /// An event sent whenever a PUT connector response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnPutConnectorResponse = new ();

        /// <summary>
        /// An event sent whenever a PUT connector response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The SCSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PutConnectorResponse(DateTimeOffset      Timestamp,
                                                     HTTPAPI       API,
                                                     OCPIRequest   Request,
                                                     OCPIResponse  Response)

            => OnPutConnectorResponse?.WhenAll(Timestamp,
                                               API ?? this,
                                               Request,
                                               Response);

        #endregion


        #region (protected internal) PatchConnectorRequest  (Request)

        /// <summary>
        /// An event sent whenever a PATCH connector request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPatchConnectorRequest = new ();

        /// <summary>
        /// An event sent whenever a PATCH connector request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The SCSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PatchConnectorRequest(DateTimeOffset     Timestamp,
                                                      HTTPAPI      API,
                                                      OCPIRequest  Request)

            => OnPatchConnectorRequest?.WhenAll(Timestamp,
                                                API ?? this,
                                                Request);

        #endregion

        #region (protected internal) PatchConnectorResponse (Response)

        /// <summary>
        /// An event sent whenever a PATCH connector response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnPatchConnectorResponse = new ();

        /// <summary>
        /// An event sent whenever a PATCH connector response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The SCSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PatchConnectorResponse(DateTimeOffset      Timestamp,
                                                       HTTPAPI       API,
                                                       OCPIRequest   Request,
                                                       OCPIResponse  Response)

            => OnPatchConnectorResponse?.WhenAll(Timestamp,
                                                 API ?? this,
                                                 Request,
                                                 Response);

        #endregion


        #region (protected internal) DeleteConnectorRequest (Request)

        /// <summary>
        /// An event sent whenever a DELETE connector request was received.
        /// </summary>
        public OCPIRequestLogEvent OnDeleteConnectorRequest = new ();

        /// <summary>
        /// An event sent whenever a DELETE connector request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The SCSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task DeleteConnectorRequest(DateTimeOffset     Timestamp,
                                                       HTTPAPI      API,
                                                       OCPIRequest  Request)

            => OnDeleteConnectorRequest?.WhenAll(Timestamp,
                                                 API ?? this,
                                                 Request);

        #endregion

        #region (protected internal) DeleteConnectorResponse(Response)

        /// <summary>
        /// An event sent whenever a DELETE connector response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnDeleteConnectorResponse = new ();

        /// <summary>
        /// An event sent whenever a DELETE connector response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The SCSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task DeleteConnectorResponse(DateTimeOffset      Timestamp,
                                                        HTTPAPI       API,
                                                        OCPIRequest   Request,
                                                        OCPIResponse  Response)

            => OnDeleteConnectorResponse?.WhenAll(Timestamp,
                                                  API ?? this,
                                                  Request,
                                                  Response);

        #endregion

        #endregion

        #region Tariffs

        #region (protected internal) GetTariffsRequest (Request)

        /// <summary>
        /// An event sent whenever a GET tariffs request was received.
        /// </summary>
        public OCPIRequestLogEvent OnGetTariffsRequest = new ();

        /// <summary>
        /// An event sent whenever a GET tariffs request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The SCSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task GetTariffsRequest(DateTimeOffset     Timestamp,
                                                  HTTPAPI      API,
                                                  OCPIRequest  Request)

            => OnGetTariffsRequest?.WhenAll(Timestamp,
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
        /// <param name="API">The SCSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task GetTariffsResponse(DateTimeOffset      Timestamp,
                                                   HTTPAPI       API,
                                                   OCPIRequest   Request,
                                                   OCPIResponse  Response)

            => OnGetTariffsResponse?.WhenAll(Timestamp,
                                             API ?? this,
                                             Request,
                                             Response);

        #endregion


        #region (protected internal) DeleteTariffsRequest (Request)

        /// <summary>
        /// An event sent whenever a DELETE tariffs request was received.
        /// </summary>
        public OCPIRequestLogEvent OnDeleteTariffsRequest = new ();

        /// <summary>
        /// An event sent whenever a DELETE tariffs request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The SCSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task DeleteTariffsRequest(DateTimeOffset     Timestamp,
                                                     HTTPAPI      API,
                                                     OCPIRequest  Request)

            => OnDeleteTariffsRequest?.WhenAll(Timestamp,
                                               API ?? this,
                                               Request);

        #endregion

        #region (protected internal) DeleteTariffsResponse(Response)

        /// <summary>
        /// An event sent whenever a DELETE tariffs response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnDeleteTariffsResponse = new ();

        /// <summary>
        /// An event sent whenever a DELETE tariffs response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The SCSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task DeleteTariffsResponse(DateTimeOffset      Timestamp,
                                                      HTTPAPI       API,
                                                      OCPIRequest   Request,
                                                      OCPIResponse  Response)

            => OnDeleteTariffsResponse?.WhenAll(Timestamp,
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
        /// <param name="API">The SCSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task GetTariffRequest(DateTimeOffset     Timestamp,
                                                 HTTPAPI      API,
                                                 OCPIRequest  Request)

            => OnGetTariffRequest?.WhenAll(Timestamp,
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
        /// <param name="API">The SCSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task GetTariffResponse(DateTimeOffset      Timestamp,
                                                  HTTPAPI       API,
                                                  OCPIRequest   Request,
                                                  OCPIResponse  Response)

            => OnGetTariffResponse?.WhenAll(Timestamp,
                                            API ?? this,
                                            Request,
                                            Response);

        #endregion


        #region (protected internal) PutTariffRequest    (Request)

        /// <summary>
        /// An event sent whenever a PUT tariff request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPutTariffRequest = new ();

        /// <summary>
        /// An event sent whenever a PUT tariff request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The SCSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PutTariffRequest(DateTimeOffset     Timestamp,
                                                 HTTPAPI      API,
                                                 OCPIRequest  Request)

            => OnPutTariffRequest?.WhenAll(Timestamp,
                                           API ?? this,
                                           Request);

        #endregion

        #region (protected internal) PutTariffResponse   (Response)

        /// <summary>
        /// An event sent whenever a PUT tariff response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnPutTariffResponse = new ();

        /// <summary>
        /// An event sent whenever a PUT tariff response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The SCSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PutTariffResponse(DateTimeOffset      Timestamp,
                                                  HTTPAPI       API,
                                                  OCPIRequest   Request,
                                                  OCPIResponse  Response)

            => OnPutTariffResponse?.WhenAll(Timestamp,
                                            API ?? this,
                                            Request,
                                            Response);

        #endregion


        #region (protected internal) PatchTariffRequest    (Request)

        /// <summary>
        /// An event sent whenever a PATCH tariff request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPatchTariffRequest = new ();

        /// <summary>
        /// An event sent whenever a PATCH tariff request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The SCSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PatchTariffRequest(DateTimeOffset     Timestamp,
                                                   HTTPAPI      API,
                                                   OCPIRequest  Request)

            => OnPatchTariffRequest?.WhenAll(Timestamp,
                                             API ?? this,
                                             Request);

        #endregion

        #region (protected internal) PatchTariffResponse   (Response)

        /// <summary>
        /// An event sent whenever a PATCH tariff response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnPatchTariffResponse = new ();

        /// <summary>
        /// An event sent whenever a PATCH tariff response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The SCSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PatchTariffResponse(DateTimeOffset      Timestamp,
                                                    HTTPAPI       API,
                                                    OCPIRequest   Request,
                                                    OCPIResponse  Response)

            => OnPatchTariffResponse?.WhenAll(Timestamp,
                                              API ?? this,
                                              Request,
                                              Response);

        #endregion


        #region (protected internal) DeleteTariffRequest (Request)

        /// <summary>
        /// An event sent whenever a DELETE tariff request was received.
        /// </summary>
        public OCPIRequestLogEvent OnDeleteTariffRequest = new ();

        /// <summary>
        /// An event sent whenever a DELETE tariff request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The SCSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task DeleteTariffRequest(DateTimeOffset     Timestamp,
                                                    HTTPAPI      API,
                                                    OCPIRequest  Request)

            => OnDeleteTariffRequest?.WhenAll(Timestamp,
                                              API ?? this,
                                              Request);

        #endregion

        #region (protected internal) DeleteTariffResponse(Response)

        /// <summary>
        /// An event sent whenever a DELETE tariff response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnDeleteTariffResponse = new ();

        /// <summary>
        /// An event sent whenever a DELETE tariff response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The SCSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task DeleteTariffResponse(DateTimeOffset      Timestamp,
                                                     HTTPAPI       API,
                                                     OCPIRequest   Request,
                                                     OCPIResponse  Response)

            => OnDeleteTariffResponse?.WhenAll(Timestamp,
                                               API ?? this,
                                               Request,
                                               Response);

        #endregion

        #endregion

        #region Sessions

        #region (protected internal) GetSessionsRequest (Request)

        /// <summary>
        /// An event sent whenever a GET sessions request was received.
        /// </summary>
        public OCPIRequestLogEvent OnGetSessionsRequest = new ();

        /// <summary>
        /// An event sent whenever a GET sessions request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The SCSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task GetSessionsRequest(DateTimeOffset     Timestamp,
                                                   HTTPAPI      API,
                                                   OCPIRequest  Request)

            => OnGetSessionsRequest?.WhenAll(Timestamp,
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
        /// <param name="API">The SCSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task GetSessionsResponse(DateTimeOffset      Timestamp,
                                                    HTTPAPI       API,
                                                    OCPIRequest   Request,
                                                    OCPIResponse  Response)

            => OnGetSessionsResponse?.WhenAll(Timestamp,
                                              API ?? this,
                                              Request,
                                              Response);

        #endregion


        #region (protected internal) DeleteSessionsRequest (Request)

        /// <summary>
        /// An event sent whenever a DELETE sessions request was received.
        /// </summary>
        public OCPIRequestLogEvent OnDeleteSessionsRequest = new ();

        /// <summary>
        /// An event sent whenever a DELETE sessions request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The SCSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task DeleteSessionsRequest(DateTimeOffset     Timestamp,
                                                      HTTPAPI      API,
                                                      OCPIRequest  Request)

            => OnDeleteSessionsRequest?.WhenAll(Timestamp,
                                                API ?? this,
                                                Request);

        #endregion

        #region (protected internal) DeleteSessionsResponse(Response)

        /// <summary>
        /// An event sent whenever a DELETE sessions response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnDeleteSessionsResponse = new ();

        /// <summary>
        /// An event sent whenever a DELETE sessions response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The SCSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task DeleteSessionsResponse(DateTimeOffset      Timestamp,
                                                       HTTPAPI       API,
                                                       OCPIRequest   Request,
                                                       OCPIResponse  Response)

            => OnDeleteSessionsResponse?.WhenAll(Timestamp,
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
        /// <param name="API">The SCSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task GetSessionRequest(DateTimeOffset     Timestamp,
                                                  HTTPAPI      API,
                                                  OCPIRequest  Request)

            => OnGetSessionRequest?.WhenAll(Timestamp,
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
        /// <param name="API">The SCSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task GetSessionResponse(DateTimeOffset      Timestamp,
                                                   HTTPAPI       API,
                                                   OCPIRequest   Request,
                                                   OCPIResponse  Response)

            => OnGetSessionResponse?.WhenAll(Timestamp,
                                             API ?? this,
                                             Request,
                                             Response);

        #endregion


        #region (protected internal) PutSessionRequest    (Request)

        /// <summary>
        /// An event sent whenever a PUT session request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPutSessionRequest = new ();

        /// <summary>
        /// An event sent whenever a PUT session request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The SCSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PutSessionRequest(DateTimeOffset     Timestamp,
                                                  HTTPAPI      API,
                                                  OCPIRequest  Request)

            => OnPutSessionRequest?.WhenAll(Timestamp,
                                            API ?? this,
                                            Request);

        #endregion

        #region (protected internal) PutSessionResponse   (Response)

        /// <summary>
        /// An event sent whenever a PUT session response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnPutSessionResponse = new ();

        /// <summary>
        /// An event sent whenever a PUT session response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The SCSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PutSessionResponse(DateTimeOffset      Timestamp,
                                                   HTTPAPI       API,
                                                   OCPIRequest   Request,
                                                   OCPIResponse  Response)

            => OnPutSessionResponse?.WhenAll(Timestamp,
                                             API ?? this,
                                             Request,
                                             Response);

        #endregion


        #region (protected internal) PatchSessionRequest  (Request)

        /// <summary>
        /// An event sent whenever a PATCH session request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPatchSessionRequest = new ();

        /// <summary>
        /// An event sent whenever a PATCH session request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The SCSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PatchSessionRequest(DateTimeOffset     Timestamp,
                                                    HTTPAPI      API,
                                                    OCPIRequest  Request)

            => OnPatchSessionRequest?.WhenAll(Timestamp,
                                              API ?? this,
                                              Request);

        #endregion

        #region (protected internal) PatchSessionResponse (Response)

        /// <summary>
        /// An event sent whenever a PATCH session response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnPatchSessionResponse = new ();

        /// <summary>
        /// An event sent whenever a PATCH session response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The SCSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PatchSessionResponse(DateTimeOffset      Timestamp,
                                                     HTTPAPI       API,
                                                     OCPIRequest   Request,
                                                     OCPIResponse  Response)

            => OnPatchSessionResponse?.WhenAll(Timestamp,
                                               API ?? this,
                                               Request,
                                               Response);

        #endregion


        #region (protected internal) DeleteSessionRequest (Request)

        /// <summary>
        /// An event sent whenever a DELETE session request was received.
        /// </summary>
        public OCPIRequestLogEvent OnDeleteSessionRequest = new ();

        /// <summary>
        /// An event sent whenever a DELETE session request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The SCSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task DeleteSessionRequest(DateTimeOffset     Timestamp,
                                                     HTTPAPI      API,
                                                     OCPIRequest  Request)

            => OnDeleteSessionRequest?.WhenAll(Timestamp,
                                               API ?? this,
                                               Request);

        #endregion

        #region (protected internal) DeleteSessionResponse(Response)

        /// <summary>
        /// An event sent whenever a DELETE session response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnDeleteSessionResponse = new ();

        /// <summary>
        /// An event sent whenever a DELETE session response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The SCSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task DeleteSessionResponse(DateTimeOffset      Timestamp,
                                                      HTTPAPI       API,
                                                      OCPIRequest   Request,
                                                      OCPIResponse  Response)

            => OnDeleteSessionResponse?.WhenAll(Timestamp,
                                                API ?? this,
                                                Request,
                                                Response);

        #endregion

        #endregion

        #region CDRs

        #region (protected internal) GetCDRsRequest (Request)

        /// <summary>
        /// An event sent whenever a GET CDRs request was received.
        /// </summary>
        public OCPIRequestLogEvent OnGetCDRsRequest = new ();

        /// <summary>
        /// An event sent whenever a GET CDRs request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The SCSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task GetCDRsRequest(DateTimeOffset     Timestamp,
                                               HTTPAPI      API,
                                               OCPIRequest  Request)

            => OnGetCDRsRequest?.WhenAll(Timestamp,
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
        /// <param name="API">The SCSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task GetCDRsResponse(DateTimeOffset      Timestamp,
                                                HTTPAPI       API,
                                                OCPIRequest   Request,
                                                OCPIResponse  Response)

            => OnGetCDRsResponse?.WhenAll(Timestamp,
                                          API ?? this,
                                          Request,
                                          Response);

        #endregion


        #region (protected internal) DeleteCDRsRequest (Request)

        /// <summary>
        /// An event sent whenever a DELETE CDRs request was received.
        /// </summary>
        public OCPIRequestLogEvent OnDeleteCDRsRequest = new ();

        /// <summary>
        /// An event sent whenever a DELETE CDRs request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The SCSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task DeleteCDRsRequest(DateTimeOffset     Timestamp,
                                                  HTTPAPI      API,
                                                  OCPIRequest  Request)

            => OnDeleteCDRsRequest?.WhenAll(Timestamp,
                                            API ?? this,
                                            Request);

        #endregion

        #region (protected internal) DeleteCDRsResponse(Response)

        /// <summary>
        /// An event sent whenever a DELETE CDRs response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnDeleteCDRsResponse = new ();

        /// <summary>
        /// An event sent whenever a DELETE CDRs response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The SCSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task DeleteCDRsResponse(DateTimeOffset      Timestamp,
                                                   HTTPAPI       API,
                                                   OCPIRequest   Request,
                                                   OCPIResponse  Response)

            => OnDeleteCDRsResponse?.WhenAll(Timestamp,
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
        /// <param name="API">The SCSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task GetCDRRequest(DateTimeOffset     Timestamp,
                                              HTTPAPI      API,
                                              OCPIRequest  Request)

            => OnGetCDRRequest?.WhenAll(Timestamp,
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
        /// <param name="API">The SCSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task GetCDRResponse(DateTimeOffset      Timestamp,
                                               HTTPAPI       API,
                                               OCPIRequest   Request,
                                               OCPIResponse  Response)

            => OnGetCDRResponse?.WhenAll(Timestamp,
                                         API ?? this,
                                         Request,
                                         Response);

        #endregion


        #region (protected internal) PostCDRRequest (Request)

        /// <summary>
        /// An event sent whenever a POST CDR request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPostCDRRequest = new ();

        /// <summary>
        /// An event sent whenever a POST CDR request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The SCSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PostCDRRequest(DateTimeOffset     Timestamp,
                                               HTTPAPI      API,
                                               OCPIRequest  Request)

            => OnPostCDRRequest?.WhenAll(Timestamp,
                                         API ?? this,
                                         Request);

        #endregion

        #region (protected internal) PostCDRResponse(Response)

        /// <summary>
        /// An event sent whenever a POST CDR response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnPostCDRResponse = new ();

        /// <summary>
        /// An event sent whenever a POST CDR response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The SCSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PostCDRResponse(DateTimeOffset      Timestamp,
                                                HTTPAPI       API,
                                                OCPIRequest   Request,
                                                OCPIResponse  Response)

            => OnPostCDRResponse?.WhenAll(Timestamp,
                                          API ?? this,
                                          Request,
                                          Response);

        #endregion


        #region (protected internal) DeleteCDRRequest (Request)

        /// <summary>
        /// An event sent whenever a DELETE CDR request was received.
        /// </summary>
        public OCPIRequestLogEvent OnDeleteCDRRequest = new ();

        /// <summary>
        /// An event sent whenever a DELETE CDR request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The SCSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task DeleteCDRRequest(DateTimeOffset     Timestamp,
                                                 HTTPAPI      API,
                                                 OCPIRequest  Request)

            => OnDeleteCDRRequest?.WhenAll(Timestamp,
                                           API ?? this,
                                           Request);

        #endregion

        #region (protected internal) DeleteCDRResponse(Response)

        /// <summary>
        /// An event sent whenever a DELETE CDR response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnDeleteCDRResponse = new ();

        /// <summary>
        /// An event sent whenever a DELETE CDR response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The SCSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task DeleteCDRResponse(DateTimeOffset      Timestamp,
                                                  HTTPAPI       API,
                                                  OCPIRequest   Request,
                                                  OCPIResponse  Response)

            => OnDeleteCDRResponse?.WhenAll(Timestamp,
                                            API ?? this,
                                            Request,
                                            Response);

        #endregion

        #endregion

        #region Tokens

        #region (protected internal) PostTokenRequest (Request)

        /// <summary>
        /// An event sent whenever a POST token request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPostTokenRequest = new ();

        /// <summary>
        /// An event sent whenever a POST token request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The SCSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PostTokenRequest(DateTimeOffset     Timestamp,
                                                 HTTPAPI      API,
                                                 OCPIRequest  Request)

            => OnPostTokenRequest?.WhenAll(Timestamp,
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
        /// <param name="API">The SCSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PostTokenResponse(DateTimeOffset      Timestamp,
                                                  HTTPAPI       API,
                                                  OCPIRequest   Request,
                                                  OCPIResponse  Response)

            => OnPostTokenResponse?.WhenAll(Timestamp,
                                            API ?? this,
                                            Request,
                                            Response);

        #endregion

        #endregion


        // Command callbacks

        #region (protected internal) ReserveNowCallbackRequest        (Request)

        /// <summary>
        /// An event sent whenever a reserve now callback was received.
        /// </summary>
        public OCPIRequestLogEvent OnReserveNowCallbackRequest = new ();

        /// <summary>
        /// An event sent whenever a reserve now callback was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the callback request.</param>
        /// <param name="API">The SCSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task ReserveNowCallbackRequest(DateTimeOffset     Timestamp,
                                                          HTTPAPI      API,
                                                          OCPIRequest  Request)

            => OnReserveNowCallbackRequest?.WhenAll(Timestamp,
                                                    API ?? this,
                                                    Request);

        #endregion

        #region (protected internal) ReserveNowCallbackResponse       (Response)

        /// <summary>
        /// An event sent whenever a reserve now callback response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnReserveNowCallbackResponse = new ();

        /// <summary>
        /// An event sent whenever a reserve now callback response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the callback response.</param>
        /// <param name="API">The SCSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task ReserveNowCallbackResponse(DateTimeOffset      Timestamp,
                                                           HTTPAPI       API,
                                                           OCPIRequest   Request,
                                                           OCPIResponse  Response)

            => OnReserveNowCallbackResponse?.WhenAll(Timestamp,
                                                     API ?? this,
                                                     Request,
                                                     Response);

        #endregion


        #region (protected internal) CancelReservationCallbackRequest (Request)

        /// <summary>
        /// An event sent whenever a cancel reservation callback was received.
        /// </summary>
        public OCPIRequestLogEvent OnCancelReservationCallbackRequest = new ();

        /// <summary>
        /// An event sent whenever a cancel reservation callback was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the callback request.</param>
        /// <param name="API">The SCSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task CancelReservationCallbackRequest(DateTimeOffset     Timestamp,
                                                                 HTTPAPI      API,
                                                                 OCPIRequest  Request)

            => OnCancelReservationCallbackRequest?.WhenAll(Timestamp,
                                                           API ?? this,
                                                           Request);

        #endregion

        #region (protected internal) CancelReservationCallbackResponse(Response)

        /// <summary>
        /// An event sent whenever a cancel reservation callback response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnCancelReservationCallbackResponse = new ();

        /// <summary>
        /// An event sent whenever a cancel reservation callback response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the callback response.</param>
        /// <param name="API">The SCSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task CancelReservationCallbackResponse(DateTimeOffset      Timestamp,
                                                                  HTTPAPI       API,
                                                                  OCPIRequest   Request,
                                                                  OCPIResponse  Response)

            => OnCancelReservationCallbackResponse?.WhenAll(Timestamp,
                                                            API ?? this,
                                                            Request,
                                                            Response);

        #endregion


        #region (protected internal) StartSessionCallbackRequest      (Request)

        /// <summary>
        /// An event sent whenever a start session callback was received.
        /// </summary>
        public OCPIRequestLogEvent OnStartSessionCallbackRequest = new ();

        /// <summary>
        /// An event sent whenever a start session callback was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the callback request.</param>
        /// <param name="API">The SCSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task StartSessionCallbackRequest(DateTimeOffset     Timestamp,
                                                            HTTPAPI      API,
                                                            OCPIRequest  Request)

            => OnStartSessionCallbackRequest?.WhenAll(Timestamp,
                                                      API ?? this,
                                                      Request);

        #endregion

        #region (protected internal) StartSessionCallbackResponse     (Response)

        /// <summary>
        /// An event sent whenever a start session callback response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnStartSessionCallbackResponse = new ();

        /// <summary>
        /// An event sent whenever a start session callback response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the callback response.</param>
        /// <param name="API">The SCSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task StartSessionCallbackResponse(DateTimeOffset      Timestamp,
                                                             HTTPAPI       API,
                                                             OCPIRequest   Request,
                                                             OCPIResponse  Response)

            => OnStartSessionCallbackResponse?.WhenAll(Timestamp,
                                                       API ?? this,
                                                       Request,
                                                       Response);

        #endregion


        #region (protected internal) StopSessionCallbackRequest       (Request)

        /// <summary>
        /// An event sent whenever a stop session callback was received.
        /// </summary>
        public OCPIRequestLogEvent OnStopSessionCallbackRequest = new ();

        /// <summary>
        /// An event sent whenever a stop session callback was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the callback request.</param>
        /// <param name="API">The SCSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task StopSessionCallbackRequest(DateTimeOffset     Timestamp,
                                                           HTTPAPI      API,
                                                           OCPIRequest  Request)

            => OnStopSessionCallbackRequest?.WhenAll(Timestamp,
                                                     API ?? this,
                                                     Request);

        #endregion

        #region (protected internal) StopSessionCallbackResponse      (Response)

        /// <summary>
        /// An event sent whenever a stop session callback response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnStopSessionCallbackResponse = new ();

        /// <summary>
        /// An event sent whenever a stop session callback response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the callback response.</param>
        /// <param name="API">The SCSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task StopSessionCallbackResponse(DateTimeOffset      Timestamp,
                                                            HTTPAPI       API,
                                                            OCPIRequest   Request,
                                                            OCPIResponse  Response)

            => OnStopSessionCallbackResponse?.WhenAll(Timestamp,
                                                      API ?? this,
                                                      Request,
                                                      Response);

        #endregion


        #region (protected internal) UnlockConnectorCallbackRequest   (Request)

        /// <summary>
        /// An event sent whenever a unlock connector callback was received.
        /// </summary>
        public OCPIRequestLogEvent OnUnlockConnectorCallbackRequest = new ();

        /// <summary>
        /// An event sent whenever a unlock connector callback was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the callback request.</param>
        /// <param name="API">The SCSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task UnlockConnectorCallbackRequest(DateTimeOffset     Timestamp,
                                                               HTTPAPI      API,
                                                               OCPIRequest  Request)

            => OnUnlockConnectorCallbackRequest?.WhenAll(Timestamp,
                                                         API ?? this,
                                                         Request);

        #endregion

        #region (protected internal) UnlockConnectorCallbackResponse  (Response)

        /// <summary>
        /// An event sent whenever a unlock connector callback response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnUnlockConnectorCallbackResponse = new ();

        /// <summary>
        /// An event sent whenever a unlock connector callback response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the callback response.</param>
        /// <param name="API">The SCSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task UnlockConnectorCallbackResponse(DateTimeOffset      Timestamp,
                                                                HTTPAPI       API,
                                                                OCPIRequest   Request,
                                                                OCPIResponse  Response)

            => OnUnlockConnectorCallbackResponse?.WhenAll(Timestamp,
                                                          API ?? this,
                                                          Request,
                                                          Response);

        #endregion

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create an instance of the HTTP API for e-mobility service providers
        /// using the given CommonAPI.
        /// </summary>
        /// <param name="CommonAPI">The OCPI common API.</param>
        /// <param name="DefaultCountryCode">The default country code to use.</param>
        /// <param name="DefaultPartyId">The default party identification to use.</param>
        /// <param name="AllowDowngrades">(Dis-)allow PUTting of object having an earlier 'LastUpdated'-timestamp then already existing objects.</param>
        /// 
        /// <param name="HTTPHostname">An optional HTTP hostname.</param>
        /// <param name="ExternalDNSName">The official URL/DNS name of this service, e.g. for sending e-mails.</param>
        /// <param name="URLPathPrefix">An optional URL path prefix.</param>
        /// <param name="BasePath">When the API is served from an optional subdirectory path.</param>
        /// <param name="HTTPServiceName">An optional name of the HTTP API service.</param>
        public SCSPAPI(CommonAPI                    CommonAPI,
                       CountryCode                  DefaultCountryCode,
                       Party_Id                     DefaultPartyId,
                       Boolean?                     AllowDowngrades           = null,

                       HTTPHostname?                HTTPHostname              = null,
                       String                       ExternalDNSName           = null,
                       HTTPPath?                    URLPathPrefix             = null,
                       HTTPPath?                    BasePath                  = null,
                       String                       HTTPServiceName           = DefaultHTTPServerName,

                       Boolean?                     IsDevelopment             = false,
                       IEnumerable<String>?         DevelopmentServers        = null,
                       Boolean?                     DisableLogging            = false,
                       String?                      LoggingContext            = null,
                       String?                      LoggingPath               = null,
                       String?                      LogfileName               = null,
                       OCPILogfileCreatorDelegate?  LogfileCreator            = null)

            : base(CommonAPI.HTTPServer,
                   HTTPHostname,
                   ExternalDNSName,
                   HTTPServiceName,
                   BasePath,

                   URLPathPrefix ?? DefaultURLPathPrefix,
                   null, // HTMLTemplate,
                   null, // APIVersionHashes,

                   null, // DisableMaintenanceTasks,
                   null, // MaintenanceInitialDelay,
                   null, // MaintenanceEvery,

                   null, // DisableWardenTasks,
                   null, // WardenInitialDelay,
                   null, // WardenCheckEvery,

                   IsDevelopment,
                   DevelopmentServers,
                   DisableLogging,
                   LoggingPath,
                   LogfileName,
                   LogfileCreator is not null
                       ? (loggingPath, context, logfileName) => LogfileCreator(loggingPath, null, context, logfileName)
                       : null,
                   false) // AutoStart

        {

            this.CommonAPI           = CommonAPI ?? throw new ArgumentNullException(nameof(CommonAPI), "The given CommonAPI must not be null!");
            this.DefaultCountryCode  = DefaultCountryCode;
            this.DefaultPartyId      = DefaultPartyId;
            this.AllowDowngrades     = AllowDowngrades;
            this.RequestTimeout      = TimeSpan.FromSeconds(60);

            this.SCSPAPILogger       = this.DisableLogging == false
                                           ? new SCSPAPILogger(
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

            #region GET    [/emsp] == /

            //HTTPServer.RegisterResourcesFolder(HTTPHostname.Any,
            //                                   URLPathPrefix + "/emsp", "cloud.charging.open.protocols.OCPIv2_2_1.HTTPAPI.SCSPAPI.HTTPRoot",
            //                                   Assembly.GetCallingAssembly());

            //CommonAPI.AddOCPIMethod(HTTPHostname.Any,
            //                             HTTPMethod.GET,
            //                             new HTTPPath[] {
            //                                 URLPathPrefix + "/emsp/index.html",
            //                                 URLPathPrefix + "/emsp/"
            //                             },
            //                             HTTPContentType.Text.HTML_UTF8,
            //                             OCPIRequest: async Request => {

            //                                 var _MemoryStream = new MemoryStream();
            //                                 typeof(SCSPAPI).Assembly.GetManifestResourceStream("cloud.charging.open.protocols.OCPIv2_2_1.HTTPAPI.SCSPAPI.HTTPRoot._header.html").SeekAndCopyTo(_MemoryStream, 3);
            //                                 typeof(SCSPAPI).Assembly.GetManifestResourceStream("cloud.charging.open.protocols.OCPIv2_2_1.HTTPAPI.SCSPAPI.HTTPRoot._footer.html").SeekAndCopyTo(_MemoryStream, 3);

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


            // Receiver Interface for eMSPs and NSPs

            #region ~/locations/{country_code}/{party_id}                               [NonStandard]

            #region OPTIONS  ~/locations/{country_code}/{party_id}      [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPHostname.Any,
                HTTPMethod.OPTIONS,
                URLPathPrefix + "locations/{country_code}/{party_id}",
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

            #region GET      ~/locations/{country_code}/{party_id}      [NonStandard]

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.GET,
                                    URLPathPrefix + "locations/{country_code}/{party_id}",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestLogger:   GetLocationsRequest,
                                    OCPIResponseLogger:  GetLocationsResponse,
                                    OCPIRequestHandler:  request => {

                                        #region Check access token

                                        if (request.LocalAccessInfo.IsNot(Role.CPO) ||
                                            request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                                        {

                                            return Task.FromResult(
                                                new OCPIResponse.Builder(request) {
                                                    StatusCode           = 2000,
                                                    StatusMessage        = "Invalid or blocked access token!",
                                                    HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                        AccessControlAllowMethods  = [ "OPTIONS", "GET", "DELETE" ],
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


                                        var filters            = request.GetDateAndPaginationFilters();

                                        var allLocations       = CommonAPI.GetLocations(partyId.Value).ToArray();

                                        var filteredLocations  = allLocations.Where(location => !filters.From.HasValue || location.LastUpdated >  filters.From.Value).
                                                                              Where(location => !filters.To.  HasValue || location.LastUpdated <= filters.To.  Value).
                                                                              ToArray();


                                        return Task.FromResult(
                                            new OCPIResponse.Builder(request) {
                                                   StatusCode           = 1000,
                                                   StatusMessage        = "Hello world!",
                                                   Data                 = new JArray(
                                                                              filteredLocations.
                                                                                  SkipTakeFilter(filters.Offset,
                                                                                                 filters.Limit).
                                                                                  Select(location => location.ToJSON(request.EMSPId,
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
                                                                          ),
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "DELETE" ],
                                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                                       //LastModified               = ?
                                                   }.
                                                   Set("X-Total-Count", allLocations.Length)
                                                   // X-Limit               The maximum number of objects that the server WILL return.
                                                   // Link                  Link to the 'next' page should be provided when this is NOT the last page.
                                            });

                                    });

            #endregion

            #region DELETE   ~/locations/{country_code}/{party_id}      [NonStandard]

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.DELETE,
                                    URLPathPrefix + "locations/{country_code}/{party_id}",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestLogger:   DeleteLocationsRequest,
                                    OCPIResponseLogger:  DeleteLocationsResponse,
                                    OCPIRequestHandler:  async request => {

                                        #region Check access token

                                        if (request.LocalAccessInfo.IsNot(Role.CPO) ||
                                            request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                                        {

                                            return new OCPIResponse.Builder(request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = "Invalid or blocked access token!",
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "DELETE" ],
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


                                        //ToDo: await...
                                        var result =  CommonAPI.RemoveAllLocations(partyId.Value);


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

            #region ~/locations/{country_code}/{party_id}/{locationId}

            #region OPTIONS  ~/locations/{country_code}/{party_id}/{locationId}      [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPHostname.Any,
                HTTPMethod.OPTIONS,
                URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}",
                OCPIRequestHandler: request =>

                    Task.FromResult(
                        new OCPIResponse.Builder(request) {
                            HTTPResponseBuilder = new HTTPResponse.Builder(request.HTTPRequest) {
                                HTTPStatusCode             = HTTPStatusCode.OK,
                                Allow                      = [ HTTPMethod.OPTIONS, HTTPMethod.GET, HTTPMethod.PUT, HTTPMethod.PATCH, HTTPMethod.DELETE ],
                                AcceptPatch                = [ HTTPContentType.Application.JSONMergePatch_UTF8 ],
                                AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                AccessControlAllowHeaders  = [ "Authorization" ]
                            }
                        })

            );

            #endregion

            #region GET      ~/locations/{country_code}/{party_id}/{locationId}

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.GET,
                                    URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestLogger:   GetLocationRequest,
                                    OCPIResponseLogger:  GetLocationResponse,
                                    OCPIRequestHandler:  request => {

                                        #region Check access token

                                        if (request.LocalAccessInfo.IsNot(Role.CPO) ||
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

                                        if (!request.ParseMandatoryLocation(CommonAPI,
                                                                            out var countryCode,
                                                                            out var partyId,
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
                                                                                          CustomDisplayTextSerializer,
                                                                                          CustomBusinessDetailsSerializer,
                                                                                          CustomHoursSerializer,
                                                                                          CustomImageSerializer,
                                                                                          CustomEnergyMixSerializer,
                                                                                          CustomEnergySourceSerializer,
                                                                                          CustomEnvironmentalImpactSerializer),
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                       AccessControlAllowHeaders  = [ "Authorization" ],
                                                       LastModified               = location.LastUpdated,
                                                       ETag                       = location.ETag
                                                   }
                                            });

                                    });

            #endregion

            #region PUT      ~/locations/{country_code}/{party_id}/{locationId}

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.PUT,
                                    URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestLogger:   PutLocationRequest,
                                    OCPIResponseLogger:  PutLocationResponse,
                                    OCPIRequestHandler:  async request => {

                                        #region Check access token

                                        if (request.LocalAccessInfo.IsNot(Role.CPO) ||
                                            request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                                        {

                                            return new OCPIResponse.Builder(request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = "Invalid or blocked access token!",
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                                       }
                                                   };

                                        }

                                        #endregion

                                        #region Check existing location

                                        if (!request.ParseOptionalLocation(CommonAPI,
                                                                           out var countryCode,
                                                                           out var partyId,
                                                                           out var locationId,
                                                                           out var existingLocation,
                                                                           out var ocpiResponseBuilder))
                                        {
                                            return ocpiResponseBuilder;
                                        }

                                        #endregion

                                        #region Parse new or updated location JSON

                                        if (!request.TryParseJObjectRequestBody(out var locationJSON, out ocpiResponseBuilder))
                                            return ocpiResponseBuilder;

                                        if (!Location.TryParse(locationJSON,
                                                               out var newOrUpdatedLocation,
                                                               out var errorResponse,
                                                               countryCode,
                                                               partyId,
                                                               locationId))
                                        {

                                            return new OCPIResponse.Builder(request) {
                                                       StatusCode           = 2001,
                                                       StatusMessage        = "Could not parse the given location JSON: " + errorResponse,
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                                       }
                                                   };

                                        }

                                        #endregion


                                        var addOrUpdateResult = await CommonAPI.AddOrUpdateLocation(
                                                                          newOrUpdatedLocation,
                                                                          AllowDowngrades ?? request.QueryString.GetBoolean("forceDowngrade")
                                                                      );


                                        if (addOrUpdateResult.IsSuccessAndDataNotNull(out var data))
                                        {

                                            return new OCPIResponse.Builder(request) {
                                                       StatusCode           = 1000,
                                                       StatusMessage        = "Hello world!",
                                                       Data                 = data.ToJSON(request.EMSPId,
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
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                                           HTTPStatusCode             = addOrUpdateResult.WasCreated == true
                                                                                            ? HTTPStatusCode.Created
                                                                                            : HTTPStatusCode.OK,
                                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                           AccessControlAllowHeaders  = [ "Authorization" ],
                                                           LastModified               = data.LastUpdated,
                                                           ETag                       = data.ETag
                                                       }
                                                   };

                                        }

                                        return new OCPIResponse.Builder(request) {
                                                   StatusCode           = 2000,
                                                   StatusMessage        = addOrUpdateResult.ErrorResponse,
                                                   Data                 = newOrUpdatedLocation.ToJSON(request.EMSPId,
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
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                                   }
                                               };

                                    });

            #endregion

            #region PATCH    ~/locations/{country_code}/{party_id}/{locationId}

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.PATCH,
                                    URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestLogger:   PatchLocationRequest,
                                    OCPIResponseLogger:  PatchLocationResponse,
                                    OCPIRequestHandler:  async request => {

                                        #region Check access token

                                        if (request.LocalAccessInfo.IsNot(Role.CPO) ||
                                            request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                                        {

                                            return new OCPIResponse.Builder(request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = "Invalid or blocked access token!",
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                                       }
                                                   };

                                        }

                                        #endregion

                                        #region Check location

                                        if (!request.ParseMandatoryLocation(CommonAPI,
                                                                            out var countryCode,
                                                                            out var partyId,
                                                                            out var locationId,
                                                                            out var existingLocation,
                                                                            out var ocpiResponseBuilder))
                                        {
                                            return ocpiResponseBuilder;
                                        }

                                        #endregion

                                        #region Parse location JSON patch

                                        if (!request.TryParseJObjectRequestBody(out var locationPatch, out ocpiResponseBuilder))
                                            return ocpiResponseBuilder;

                                        #endregion


                                        // Validation-Checks for PATCHes
                                        // (E-Tag, Timestamp, ...)

                                        var patchedLocation = await CommonAPI.TryPatchLocation(
                                                                        Party_Idv3.From(
                                                                            countryCode.Value,
                                                                            partyId.    Value
                                                                        ),
                                                                        locationId.Value,
                                                                        locationPatch
                                                                    );


                                        //ToDo: Handle update errors!
                                        if (patchedLocation.IsSuccessAndDataNotNull(out var data))
                                            return new OCPIResponse.Builder(request) {
                                                           StatusCode           = 1000,
                                                           StatusMessage        = "Hello world!",
                                                           Data                 = data.ToJSON(request.EMSPId,
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
                                                       StatusMessage        = patchedLocation.ErrorResponse,
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.OK,
                                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                                       }
                                                   };

                                    });

            #endregion

            #region DELETE   ~/locations/{country_code}/{party_id}/{locationId}      [NonStandard]

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.DELETE,
                                    URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestLogger:   DeleteLocationRequest,
                                    OCPIResponseLogger:  DeleteLocationResponse,
                                    OCPIRequestHandler:  async request => {

                                        #region Check access token

                                        if (request.LocalAccessInfo.IsNot(Role.CPO) ||
                                            request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                                        {

                                            return new OCPIResponse.Builder(request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = "Invalid or blocked access token!",
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                                       }
                                                   };

                                        }

                                        #endregion

                                        #region Check existing location

                                        if (!request.ParseMandatoryLocation(CommonAPI,
                                                                            out var countryCode,
                                                                            out var partyId,
                                                                            out var locationId,
                                                                            out var location,
                                                                            out var ocpiResponseBuilder))
                                        {
                                            return ocpiResponseBuilder;
                                        }

                                        #endregion


                                        //ToDo: await...
                                        CommonAPI.RemoveLocation(location);


                                        return new OCPIResponse.Builder(request) {
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
                                                                                              CustomDisplayTextSerializer,
                                                                                              CustomBusinessDetailsSerializer,
                                                                                              CustomHoursSerializer,
                                                                                              CustomImageSerializer,
                                                                                              CustomEnergyMixSerializer,
                                                                                              CustomEnergySourceSerializer,
                                                                                              CustomEnvironmentalImpactSerializer),
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.OK,
                                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                           AccessControlAllowHeaders  = [ "Authorization" ],
                                                           LastModified               = location.LastUpdated,
                                                           ETag                       = location.ETag
                                                       }
                                                   };

                                    });

            #endregion

            #endregion

            #region ~/locations/{country_code}/{party_id}/{locationId}/{evseId}

            #region OPTIONS  ~/locations/{country_code}/{party_id}/{locationId}/{evseId}      [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPHostname.Any,
                HTTPMethod.OPTIONS,
                URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}/{evseId}",
                OCPIRequestHandler: request =>

                    Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               HTTPResponseBuilder = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   Allow                      = [ HTTPMethod.OPTIONS, HTTPMethod.GET, HTTPMethod.PUT, HTTPMethod.PATCH, HTTPMethod.DELETE ],
                                   AcceptPatch                = [ HTTPContentType.Application.JSONMergePatch_UTF8 ],
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ]
                               }
                        })

            );

            #endregion

            #region GET      ~/locations/{country_code}/{party_id}/{locationId}/{evseId}

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.GET,
                                    URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}/{evseId}",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestLogger:   GetEVSERequest,
                                    OCPIResponseLogger:  GetEVSEResponse,
                                    OCPIRequestHandler:  request => {

                                        #region Check access token

                                        if (request.LocalAccessInfo.IsNot(Role.CPO) ||
                                            request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                                        {

                                            return Task.FromResult(
                                                new OCPIResponse.Builder(request) {
                                                    StatusCode           = 2000,
                                                    StatusMessage        = "Invalid or blocked access token!",
                                                    HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                        AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                        AccessControlAllowHeaders  = [ "Authorization" ]
                                                    }
                                                });

                                        }

                                        #endregion

                                        #region Check EVSE

                                        if (!request.ParseMandatoryLocationEVSE(CommonAPI,
                                                                                out var countryCode,
                                                                                out var partyId,
                                                                                out var locationId,
                                                                                out var location,
                                                                                out var evseUId,
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
                                                   Data                 = evse.ToJSON(request.EMSPId,
                                                                                      CustomEVSESerializer,
                                                                                      CustomStatusScheduleSerializer,
                                                                                      CustomConnectorSerializer,
                                                                                      CustomEVSEEnergyMeterSerializer,
                                                                                      CustomTransparencySoftwareStatusSerializer,
                                                                                      CustomTransparencySoftwareSerializer,
                                                                                      CustomDisplayTextSerializer,
                                                                                      CustomImageSerializer),
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                       AccessControlAllowHeaders  = [ "Authorization" ],
                                                       LastModified               = evse.LastUpdated,
                                                       ETag                       = evse.ETag
                                                   }
                                            });

                                    });

            #endregion

            #region PUT      ~/locations/{country_code}/{party_id}/{locationId}/{evseId}

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.PUT,
                                    URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}/{evseId}",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestLogger:   PutEVSERequest,
                                    OCPIResponseLogger:  PutEVSEResponse,
                                    OCPIRequestHandler:  async request => {

                                        #region Check access token

                                        if (request.LocalAccessInfo.IsNot(Role.CPO) ||
                                            request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                                        {

                                            return new OCPIResponse.Builder(request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = "Invalid or blocked access token!",
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                                       }
                                                   };

                                        }

                                        #endregion

                                        #region Check existing EVSE

                                        if (!request.ParseOptionalLocationEVSE(CommonAPI,
                                                                               out var countryCode,
                                                                               out var partyId,
                                                                               out var locationId,
                                                                               out var existingLocation,
                                                                               out var evseUId,
                                                                               out var existingEVSE,
                                                                               out var ocpiResponseBuilder))
                                        {
                                            return ocpiResponseBuilder;
                                        }

                                        #endregion

                                        #region Parse new or updated EVSE JSON

                                        if (!request.TryParseJObjectRequestBody(out var evseJSON, out ocpiResponseBuilder))
                                            return ocpiResponseBuilder;

                                        if (!EVSE.TryParse(evseJSON,
                                                           out var newOrUpdatedEVSE,
                                                           out var errorResponse,
                                                           evseUId))
                                        {

                                            return new OCPIResponse.Builder(request) {
                                                       StatusCode           = 2001,
                                                       StatusMessage        = "Could not parse the given EVSE JSON: " + errorResponse,
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                                       }
                                                   };

                                        }

                                        #endregion


                                        var addOrUpdateResult = await CommonAPI.AddOrUpdateEVSE(
                                                                          existingLocation,
                                                                          newOrUpdatedEVSE,
                                                                          AllowDowngrades ?? request.QueryString.GetBoolean("forceDowngrade")
                                                                      );


                                        if (addOrUpdateResult.IsSuccessAndDataNotNull(out var data))
                                            return new OCPIResponse.Builder(request) {
                                                       StatusCode           = 1000,
                                                       StatusMessage        = "Hello world!",
                                                       Data                 = data.ToJSON(request.EMSPId,
                                                                                          CustomEVSESerializer,
                                                                                          CustomStatusScheduleSerializer,
                                                                                          CustomConnectorSerializer,
                                                                                          CustomEVSEEnergyMeterSerializer,
                                                                                          CustomTransparencySoftwareStatusSerializer,
                                                                                          CustomTransparencySoftwareSerializer,
                                                                                          CustomDisplayTextSerializer,
                                                                                          CustomImageSerializer),
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                                           HTTPStatusCode             = addOrUpdateResult.WasCreated == true
                                                                                            ? HTTPStatusCode.Created
                                                                                            : HTTPStatusCode.OK,
                                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                           AccessControlAllowHeaders  = [ "Authorization" ],
                                                           LastModified               = data.LastUpdated,
                                                           ETag                       = data.ETag
                                                       }
                                                   };

                                        return new OCPIResponse.Builder(request) {
                                                   StatusCode           = 2000,
                                                   StatusMessage        = addOrUpdateResult.ErrorResponse,
                                                   Data                 = newOrUpdatedEVSE.ToJSON(request.EMSPId,
                                                                                                  CustomEVSESerializer,
                                                                                                  CustomStatusScheduleSerializer,
                                                                                                  CustomConnectorSerializer,
                                                                                                  CustomEVSEEnergyMeterSerializer,
                                                                                                  CustomTransparencySoftwareStatusSerializer,
                                                                                                  CustomTransparencySoftwareSerializer,
                                                                                                  CustomDisplayTextSerializer,
                                                                                                  CustomImageSerializer),
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                       AccessControlAllowHeaders  = [ "Authorization" ],
                                                       LastModified               = newOrUpdatedEVSE.LastUpdated,
                                                       ETag                       = newOrUpdatedEVSE.ETag
                                                   }
                                               };

                                    });

            #endregion

            #region PATCH    ~/locations/{country_code}/{party_id}/{locationId}/{evseId}

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.PATCH,
                                    URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}/{evseId}",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestLogger:   PatchEVSERequest,
                                    OCPIResponseLogger:  PatchEVSEResponse,
                                    OCPIRequestHandler:  async request => {

                                        #region Check access token

                                        if (request.LocalAccessInfo.IsNot(Role.CPO) ||
                                            request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                                        {

                                            return new OCPIResponse.Builder(request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = "Invalid or blocked access token!",
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                                       }
                                                   };

                                        }

                                        #endregion

                                        #region Check EVSE

                                        if (!request.ParseMandatoryLocationEVSE(CommonAPI,
                                                                                out var countryCode,
                                                                                out var partyId,
                                                                                out var locationId,
                                                                                out var existingLocation,
                                                                                out var evseUId,
                                                                                out var existingEVSE,
                                                                                out var ocpiResponseBuilder))
                                        {
                                            return ocpiResponseBuilder;
                                        }

                                        #endregion

                                        #region Parse EVSE JSON patch

                                        if (!request.TryParseJObjectRequestBody(out var evsePatch, out ocpiResponseBuilder))
                                            return ocpiResponseBuilder;

                                        #endregion


                                        var patchedEVSE = await CommonAPI.TryPatchEVSE(
                                                                    existingLocation,
                                                                    existingEVSE,
                                                                    evsePatch
                                                                );

                                        //ToDo: Handle update errors!
                                        if (patchedEVSE.IsSuccessAndDataNotNull(out var data))
                                            return new OCPIResponse.Builder(request) {
                                                           StatusCode           = 1000,
                                                           StatusMessage        = "Hello world!",
                                                           Data                 = data.ToJSON(request.EMSPId,
                                                                                              CustomEVSESerializer,
                                                                                              CustomStatusScheduleSerializer,
                                                                                              CustomConnectorSerializer,
                                                                                              CustomEVSEEnergyMeterSerializer,
                                                                                              CustomTransparencySoftwareStatusSerializer,
                                                                                              CustomTransparencySoftwareSerializer,
                                                                                              CustomDisplayTextSerializer,
                                                                                              CustomImageSerializer),
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
                                                       StatusMessage        = patchedEVSE.ErrorResponse,
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.OK,
                                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                                       }
                                                   };

                                    });

            #endregion

            #region DELETE   ~/locations/{country_code}/{party_id}/{locationId}/{evseId}      [NonStandard]

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.DELETE,
                                    URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}/{evseId}",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestLogger:   DeleteEVSERequest,
                                    OCPIResponseLogger:  DeleteEVSEResponse,
                                    OCPIRequestHandler:  async request => {

                                        #region Check access token

                                        if (request.LocalAccessInfo.IsNot(Role.CPO) ||
                                            request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                                        {

                                            return new OCPIResponse.Builder(request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = "Invalid or blocked access token!",
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                                       }
                                                   };

                                        }

                                        #endregion

                                        #region Check existing Location/EVSE(UId URI parameter)

                                        if (!request.ParseMandatoryLocationEVSE(CommonAPI,
                                                                                out var countryCode,
                                                                                out var partyId,
                                                                                out var locationId,
                                                                                out var existingLocation,
                                                                                out var evseUId,
                                                                                out var existingEVSE,
                                                                                out var ocpiResponseBuilder))
                                        {
                                            return ocpiResponseBuilder;
                                        }

                                        #endregion


                                        //CommonAPI.Remove(ExistingLocation, ExistingEVSE);


                                        return new OCPIResponse.Builder(request) {
                                                       StatusCode           = 1000,
                                                       StatusMessage        = "Hello world!",
                                                       Data                 = existingEVSE.ToJSON(request.EMSPId,
                                                                                                  CustomEVSESerializer,
                                                                                                  CustomStatusScheduleSerializer,
                                                                                                  CustomConnectorSerializer,
                                                                                                  CustomEVSEEnergyMeterSerializer,
                                                                                                  CustomTransparencySoftwareStatusSerializer,
                                                                                                  CustomTransparencySoftwareSerializer,
                                                                                                  CustomDisplayTextSerializer,
                                                                                                  CustomImageSerializer),
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.OK,
                                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                           AccessControlAllowHeaders  = [ "Authorization" ],
                                                           LastModified               = existingEVSE.LastUpdated,
                                                           ETag                       = existingEVSE.ETag
                                                       }
                                                   };

                                    });

            #endregion

            #endregion

            #region ~/locations/{country_code}/{party_id}/{locationId}/{evseId}/{connectorId}

            #region OPTIONS  ~/locations/{country_code}/{party_id}/{locationId}/{evseId}/{connectorId}      [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPHostname.Any,
                HTTPMethod.OPTIONS,
                URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}/{evseId}/{connectorId}",
                OCPIRequestHandler: request =>

                    Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               StatusCode           = 1000,
                               StatusMessage        = "Hello world!",
                               HTTPResponseBuilder = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   Allow                      = [ HTTPMethod.OPTIONS, HTTPMethod.GET, HTTPMethod.PUT, HTTPMethod.PATCH, HTTPMethod.DELETE ],
                                   AcceptPatch                = [ HTTPContentType.Application.JSONMergePatch_UTF8 ],
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ]
                               }
                        })

            );

            #endregion

            #region GET      ~/locations/{country_code}/{party_id}/{locationId}/{evseId}/{connectorId}

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.GET,
                                    URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}/{evseId}/{connectorId}",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestLogger:   GetConnectorRequest,
                                    OCPIResponseLogger:  GetConnectorResponse,
                                    OCPIRequestHandler:  request => {

                                        #region Check access token

                                        if (request.LocalAccessInfo.IsNot(Role.CPO) ||
                                            request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                                        {

                                            return Task.FromResult(
                                                new OCPIResponse.Builder(request) {
                                                    StatusCode           = 2000,
                                                    StatusMessage        = "Invalid or blocked access token!",
                                                    HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                        AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                        AccessControlAllowHeaders  = [ "Authorization" ]
                                                    }
                                                });

                                        }

                                        #endregion

                                        #region Check connector

                                        if (!request.ParseMandatoryLocationEVSEConnector(CommonAPI,
                                                                                         out var countryCode,
                                                                                         out var partyId,
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
                                                   Data                 = connector.ToJSON(request.EMSPId,
                                                                                           CustomConnectorSerializer),
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                       AccessControlAllowHeaders  = [ "Authorization" ],
                                                       LastModified               = connector.LastUpdated,
                                                       ETag                       = connector.ETag
                                                   }
                                            });

                                    });

            #endregion

            #region PUT      ~/locations/{country_code}/{party_id}/{locationId}/{evseId}/{connectorId}

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.PUT,
                                    URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}/{evseId}/{connectorId}",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestLogger:   PutConnectorRequest,
                                    OCPIResponseLogger:  PutConnectorResponse,
                                    OCPIRequestHandler:  async request => {

                                        #region Check access token

                                        if (request.LocalAccessInfo.IsNot(Role.CPO) ||
                                            request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                                        {

                                            return new OCPIResponse.Builder(request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = "Invalid or blocked access token!",
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                                       }
                                                   };

                                        }

                                        #endregion

                                        #region Check connector

                                        if (!request.ParseOptionalLocationEVSEConnector(CommonAPI,
                                                                                        out var countryCode,
                                                                                        out var partyId,
                                                                                        out var locationId,
                                                                                        out var existingLocation,
                                                                                        out var evseUId,
                                                                                        out var existingEVSE,
                                                                                        out var connectorId,
                                                                                        out var existingConnector,
                                                                                        out var ocpiResponseBuilder))
                                        {
                                            return ocpiResponseBuilder;
                                        }

                                        #endregion

                                        #region Parse new or updated connector JSON

                                        if (!request.TryParseJObjectRequestBody(out var connectorJSON, out ocpiResponseBuilder))
                                            return ocpiResponseBuilder;

                                        if (!Connector.TryParse(connectorJSON,
                                                                out var newOrUpdatedConnector,
                                                                out var errorResponse,
                                                                connectorId))
                                        {

                                            return new OCPIResponse.Builder(request) {
                                                       StatusCode           = 2001,
                                                       StatusMessage        = "Could not parse the given connector JSON: " + errorResponse,
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                                       }
                                                   };

                                        }

                                        #endregion


                                        var addOrUpdateResult = await CommonAPI.AddOrUpdateConnector(
                                                                          existingLocation,
                                                                          existingEVSE,
                                                                          newOrUpdatedConnector,
                                                                          AllowDowngrades ?? request.QueryString.GetBoolean("forceDowngrade")
                                                                      );


                                        if (addOrUpdateResult.IsSuccessAndDataNotNull(out var data))
                                            return new OCPIResponse.Builder(request) {
                                                       StatusCode           = 1000,
                                                       StatusMessage        = "Hello world!",
                                                       Data                 = data.ToJSON(request.EMSPId,
                                                                                          CustomConnectorSerializer),
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                                           HTTPStatusCode             = addOrUpdateResult.WasCreated == true
                                                                                            ? HTTPStatusCode.Created
                                                                                            : HTTPStatusCode.OK,
                                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                           AccessControlAllowHeaders  = [ "Authorization" ],
                                                           LastModified               = newOrUpdatedConnector.LastUpdated,
                                                           ETag                       = newOrUpdatedConnector.ETag
                                                       }
                                                   };

                                        return new OCPIResponse.Builder(request) {
                                                   StatusCode           = 2000,
                                                   StatusMessage        = addOrUpdateResult.ErrorResponse,
                                                   Data                 = newOrUpdatedConnector.ToJSON(request.EMSPId,
                                                                                                       CustomConnectorSerializer),
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                       AccessControlAllowHeaders  = [ "Authorization" ],
                                                       LastModified               = newOrUpdatedConnector.LastUpdated,
                                                       ETag                       = newOrUpdatedConnector.ETag
                                                   }
                                               };

                                    });

            #endregion

            #region PATCH    ~/locations/{country_code}/{party_id}/{locationId}/{evseId}/{connectorId}

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.PATCH,
                                    URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}/{evseId}/{connectorId}",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestLogger:   PatchConnectorRequest,
                                    OCPIResponseLogger:  PatchConnectorResponse,
                                    OCPIRequestHandler:  async request => {

                                        #region Check access token

                                        if (request.LocalAccessInfo.IsNot(Role.CPO) ||
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

                                        #region Check connector

                                        if (!request.ParseMandatoryLocationEVSEConnector(CommonAPI,
                                                                                         out var countryCode,
                                                                                         out var partyId,
                                                                                         out var locationId,
                                                                                         out var existingLocation,
                                                                                         out var evseUId,
                                                                                         out var existingEVSE,
                                                                                         out var connectorId,
                                                                                         out var existingConnector,
                                                                                         out var ocpiResponseBuilder))
                                        {
                                            return ocpiResponseBuilder;
                                        }

                                        #endregion

                                        #region Parse connector JSON patch

                                        if (!request.TryParseJObjectRequestBody(out var connectorPatch, out ocpiResponseBuilder))
                                            return ocpiResponseBuilder;

                                        #endregion


                                        var patchedConnector = await CommonAPI.TryPatchConnector(
                                                                         existingLocation,
                                                                         existingEVSE,
                                                                         existingConnector,
                                                                         connectorPatch
                                                                     );

                                        //ToDo: Handle update errors!
                                        if (patchedConnector.IsSuccessAndDataNotNull(out var data))
                                            return new OCPIResponse.Builder(request) {
                                                           StatusCode           = 1000,
                                                           StatusMessage        = "Hello world!",
                                                           Data                 = data.ToJSON(),
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
                                                       StatusMessage        = patchedConnector.ErrorResponse,
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.OK,
                                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                                       }
                                                   };

                                    });

            #endregion

            #region DELETE   ~/locations/{country_code}/{party_id}/{locationId}/{evseId}/{connectorId}      [NonStandard]

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.DELETE,
                                    URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}/{evseId}/{connectorId}",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestLogger:   DeleteConnectorRequest,
                                    OCPIResponseLogger:  DeleteConnectorResponse,
                                    OCPIRequestHandler:  async request => {

                                        #region Check access token

                                        if (request.LocalAccessInfo.IsNot(Role.CPO) ||
                                            request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                                        {

                                            return new OCPIResponse.Builder(request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = "Invalid or blocked access token!",
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                                       }
                                                   };

                                        }

                                        #endregion

                                        #region Check existing Location/EVSE/Connector(UId URI parameter)

                                        if (!request.ParseMandatoryLocationEVSEConnector(CommonAPI,
                                                                                         out var countryCode,
                                                                                         out var partyId,
                                                                                         out var locationId,
                                                                                         out var existingLocation,
                                                                                         out var evseUId,
                                                                                         out var existingEVSE,
                                                                                         out var connectorId,
                                                                                         out var existingConnector,
                                                                                         out var ocpiResponseBuilder))
                                        {
                                            return ocpiResponseBuilder;
                                        }

                                        #endregion


                                        //CommonAPI.Remove(ExistingLocation, ExistingEVSE);


                                        return new OCPIResponse.Builder(request) {
                                                       StatusCode           = 1000,
                                                       StatusMessage        = "Hello world!",
                                                       Data                 = existingConnector.ToJSON(request.EMSPId,
                                                                                                       CustomConnectorSerializer),
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.OK,
                                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                           AccessControlAllowHeaders  = [ "Authorization" ],
                                                           LastModified               = existingConnector.LastUpdated,
                                                           ETag                       = existingConnector.ETag
                                                       }
                                                   };

                                    });

            #endregion

            #endregion


            #region ~/locations/{country_code}/{party_id}/{locationId}/{evseId}/status  [NonStandard]

            #region OPTIONS  ~/locations/{country_code}/{party_id}/{locationId}/{evseId}/status     [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPHostname.Any,
                HTTPMethod.OPTIONS,
                URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}/{evseId}/status",
                OCPIRequestHandler: request =>

                    Task.FromResult(
                        new OCPIResponse.Builder(request) {
                            HTTPResponseBuilder = new HTTPResponse.Builder(request.HTTPRequest) {
                                HTTPStatusCode             = HTTPStatusCode.OK,
                                AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
                                Allow                      = [ HTTPMethod.OPTIONS, HTTPMethod.POST ],
                                AcceptPatch                = [ HTTPContentType.Application.JSONMergePatch_UTF8 ],
                                AccessControlAllowHeaders  = [ "Authorization" ]
                            }
                        })

            );

            #endregion

            #region POST     ~/locations/{country_code}/{party_id}/{locationId}/{evseId}/status     [NonStandard]

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.POST,
                                    URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}/{evseId}/status",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestLogger:   PutEVSERequest,
                                    OCPIResponseLogger:  PutEVSEResponse,
                                    OCPIRequestHandler:  async request => {

                                        #region Check access token

                                        if (request.LocalAccessInfo.IsNot(Role.CPO) ||
                                            request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                                        {

                                            return new OCPIResponse.Builder(request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = "Invalid or blocked access token!",
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                           AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
                                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                                       }
                                                   };

                                        }

                                        #endregion

                                        #region Check existing EVSE

                                        if (!request.ParseMandatoryLocationEVSE(CommonAPI,
                                                                                out var countryCode,
                                                                                out var partyId,
                                                                                out var locationId,
                                                                                out var existingLocation,
                                                                                out var evseUId,
                                                                                out var existingEVSE,
                                                                                out var ocpiResponseBuilder))
                                        {
                                            return ocpiResponseBuilder;
                                        }

                                        #endregion

                                        #region Parse EVSE status JSON

                                        if (!request.TryParseJObjectRequestBody(out var evseStatusJSON, out ocpiResponseBuilder))
                                            return ocpiResponseBuilder;

                                        //if (!EVSE.TryParse(EVSEJSON,
                                        //                   out EVSE    newOrUpdatedEVSE,
                                        //                   out String  ErrorResponse,
                                        //                   EVSEUId))
                                        //{

                                        //    return new OCPIResponse.Builder(Request) {
                                        //               StatusCode           = 2001,
                                        //               StatusMessage        = "Could not parse the given EVSE JSON: " + ErrorResponse,
                                        //               HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                        //                   HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                        //                   AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                        //                   AccessControlAllowHeaders  = [ "Authorization" ]
                                        //               }
                                        //           };

                                        //}

                                        #endregion


                                        //ToDo: Handle AddOrUpdate errors


                                        return new OCPIResponse.Builder(request) {
                                                       StatusCode           = 1000,
                                                       StatusMessage        = "Hello world!",
                                                       //Data                 = newOrUpdatedEVSE.ToJSON(),
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.OK,
                                                           AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
                                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                                       }
                                                   };

                                    });

            #endregion

            #endregion



            #region ~/sessions/{country_code}/{party_id}                                [NonStandard]

            #region OPTIONS  ~/sessions/{country_code}/{party_id}                [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPHostname.Any,
                HTTPMethod.OPTIONS,
                URLPathPrefix + "sessions/{country_code}/{party_id}",
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

            #region GET      ~/sessions                                          [NonStandard]

            // Return all charging session for the given access token roles

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.GET,
                                    URLPathPrefix + "sessions",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestLogger:   GetSessionsRequest,
                                    OCPIResponseLogger:  GetSessionsResponse,
                                    OCPIRequestHandler:  request => {

                                        #region Check access token

                                        if (request.LocalAccessInfo.IsNot(Role.CPO) ||
                                            request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                                        {

                                            return Task.FromResult(
                                                new OCPIResponse.Builder(request) {
                                                    StatusCode           = 2000,
                                                    StatusMessage        = "Invalid or blocked access token!",
                                                    HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                        AccessControlAllowMethods  = [ "OPTIONS", "GET", "DELETE" ],
                                                        AccessControlAllowHeaders  = [ "Authorization" ]
                                                    }
                                                });

                                        }

                                        #endregion


                                        var filters           = request.GetDateAndPaginationFilters();

                                        var allSessions       = CommonAPI.GetSessions(session => request.LocalAccessInfo.Roles.Any(role => role.CountryCode == session.CountryCode &&
                                                                                                                                           role.PartyId     == session.PartyId)).
                                                                          ToArray();

                                        var filteredSessions  = allSessions.Where(session => !filters.From.HasValue || session.LastUpdated >  filters.From.Value).
                                                                            Where(session => !filters.To.  HasValue || session.LastUpdated <= filters.To.  Value).
                                                                            ToArray();


                                        return Task.FromResult(
                                            new OCPIResponse.Builder(request) {
                                                   StatusCode           = 1000,
                                                   StatusMessage        = "Hello world!",
                                                   Data                 = new JArray(
                                                                              filteredSessions.
                                                                                  SkipTakeFilter(filters.Offset,
                                                                                                 filters.Limit).
                                                                                  Select(session => session.ToJSON(CustomSessionSerializer,
                                                                                                                   CustomCDRTokenSerializer,
                                                                                                                   CustomChargingPeriodSerializer,
                                                                                                                   CustomCDRDimensionSerializer,
                                                                                                                   CustomPriceSerializer))
                                                                          ),
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "DELETE" ],
                                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                                       //LastModified               = ?
                                                   }.
                                                   Set("X-Total-Count", allSessions.Length)
                                                   // X-Limit               The maximum number of objects that the server WILL return.
                                                   // Link                  Link to the 'next' page should be provided when this is NOT the last page.
                                            });

                                    });

            #endregion

            #region GET      ~/sessions/{country_code}/{party_id}                [NonStandard]

            // Return all charging session for the given country code and party identification

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.GET,
                                    URLPathPrefix + "sessions/{country_code}/{party_id}",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestLogger:   GetSessionsRequest,
                                    OCPIResponseLogger:  GetSessionsResponse,
                                    OCPIRequestHandler:  request => {

                                        #region Check access token

                                        if (request.LocalAccessInfo.IsNot(Role.CPO) ||
                                            request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                                        {

                                            return Task.FromResult(
                                                new OCPIResponse.Builder(request) {
                                                    StatusCode           = 2000,
                                                    StatusMessage        = "Invalid or blocked access token!",
                                                    HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                        AccessControlAllowMethods  = [ "OPTIONS", "GET", "DELETE" ],
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


                                        var filters           = request.GetDateAndPaginationFilters();

                                        var allSessions       = CommonAPI.GetSessions(partyId.Value).ToArray();

                                        var filteredSessions  = allSessions.Where(session => !filters.From.HasValue || session.LastUpdated >  filters.From.Value).
                                                                            Where(session => !filters.To.  HasValue || session.LastUpdated <= filters.To.  Value).
                                                                            ToArray();


                                        return Task.FromResult(
                                            new OCPIResponse.Builder(request) {
                                                   StatusCode           = 1000,
                                                   StatusMessage        = "Hello world!",
                                                   Data                 = new JArray(
                                                                              filteredSessions.
                                                                                  SkipTakeFilter(filters.Offset,
                                                                                                 filters.Limit).
                                                                                  Select(session => session.ToJSON(CustomSessionSerializer,
                                                                                                                   CustomCDRTokenSerializer,
                                                                                                                   CustomChargingPeriodSerializer,
                                                                                                                   CustomCDRDimensionSerializer,
                                                                                                                   CustomPriceSerializer))
                                                                          ),
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "DELETE" ],
                                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                                       //LastModified               = ?
                                                   }.
                                                   Set("X-Total-Count", allSessions.Length)
                                                   // X-Limit               The maximum number of objects that the server WILL return.
                                                   // Link                  Link to the 'next' page should be provided when this is NOT the last page.
                                            });

                                    });

            #endregion

            #region DELETE   ~/sessions                                          [NonStandard]

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.DELETE,
                                    URLPathPrefix + "sessions",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestLogger:   DeleteSessionsRequest,
                                    OCPIResponseLogger:  DeleteSessionsResponse,
                                    OCPIRequestHandler:  async request => {

                                        #region Check access token

                                        if (request.LocalAccessInfo.IsNot(Role.CPO) ||
                                            request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                                        {

                                            return new OCPIResponse.Builder(request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = "Invalid or blocked access token!",
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "DELETE" ],
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


                                        foreach (var role in request.LocalAccessInfo.Roles)
                                            await CommonAPI.RemoveAllSessions(
                                                      Party_Idv3.From(
                                                          role.CountryCode,
                                                          role.PartyId
                                                      )
                                                  );


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

            #region DELETE   ~/sessions/{country_code}/{party_id}                [NonStandard]

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.DELETE,
                                    URLPathPrefix + "sessions/{country_code}/{party_id}",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestLogger:   DeleteSessionsRequest,
                                    OCPIResponseLogger:  DeleteSessionsResponse,
                                    OCPIRequestHandler:  async request => {

                                        #region Check access token

                                        if (request.LocalAccessInfo.IsNot(Role.CPO) ||
                                            request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                                        {

                                            return new OCPIResponse.Builder(request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = "Invalid or blocked access token!",
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "DELETE" ],
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


                                        var result = await CommonAPI.RemoveAllSessions(partyId.Value);


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

            #region ~/sessions/{country_code}/{party_id}/{sessionId}

            #region OPTIONS  ~/sessions/{country_code}/{party_id}/{sessionId}    [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPHostname.Any,
                HTTPMethod.OPTIONS,
                URLPathPrefix + "sessions/{country_code}/{party_id}/{sessionId}",
                OCPIRequestHandler: request =>

                    Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               HTTPResponseBuilder = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   Allow                      = [ HTTPMethod.OPTIONS, HTTPMethod.GET, HTTPMethod.PUT, HTTPMethod.PATCH, HTTPMethod.DELETE ],
                                   AcceptPatch                = [ HTTPContentType.Application.JSONMergePatch_UTF8 ],
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ]
                               }
                        })

            );

            #endregion

            #region GET      ~/sessions/{country_code}/{party_id}/{sessionId}

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.GET,
                                    URLPathPrefix + "sessions/{country_code}/{party_id}/{sessionId}",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestLogger:   GetSessionRequest,
                                    OCPIResponseLogger:  GetSessionResponse,
                                    OCPIRequestHandler:  request => {

                                        #region Check access token

                                        if (request.LocalAccessInfo.IsNot(Role.CPO) ||
                                            request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                                        {

                                            return Task.FromResult(
                                                new OCPIResponse.Builder(request) {
                                                    StatusCode           = 2000,
                                                    StatusMessage        = "Invalid or blocked access token!",
                                                    HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                        AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                        AccessControlAllowHeaders  = [ "Authorization" ]
                                                    }
                                                });

                                        }

                                        #endregion

                                        #region Check session

                                        if (!request.ParseMandatorySession(CommonAPI,
                                                                           out var countryCode,
                                                                           out var partyId,
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
                                                Data                 = session.ToJSON(CustomSessionSerializer,
                                                                                      CustomCDRTokenSerializer,
                                                                                      CustomChargingPeriodSerializer,
                                                                                      CustomCDRDimensionSerializer,
                                                                                      CustomPriceSerializer),
                                                HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                                    HTTPStatusCode             = HTTPStatusCode.OK,
                                                    AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                    AccessControlAllowHeaders  = [ "Authorization" ],
                                                    LastModified               = session.LastUpdated,
                                                    ETag                       = session.ETag
                                                }
                                            });

                                    });

            #endregion

            #region PUT      ~/sessions/{country_code}/{party_id}/{sessionId}

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.PUT,
                                    URLPathPrefix + "sessions/{country_code}/{party_id}/{sessionId}",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestLogger:   PutSessionRequest,
                                    OCPIResponseLogger:  PutSessionResponse,
                                    OCPIRequestHandler:  async request => {

                                        #region Check access token

                                        if (request.LocalAccessInfo.IsNot(Role.CPO) ||
                                            request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                                        {

                                            return new OCPIResponse.Builder(request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = "Invalid or blocked access token!",
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                                       }
                                                   };

                                        }

                                        #endregion

                                        #region Check existing session

                                        if (!request.ParseOptionalSession(CommonAPI,
                                                                          out var countryCode,
                                                                          out var partyId,
                                                                          out var sessionId,
                                                                          out var existingSession,
                                                                          out var ocpiResponseBuilder))
                                        {
                                            return ocpiResponseBuilder;
                                        }

                                        #endregion

                                        #region Parse new or updated session

                                        if (!request.TryParseJObjectRequestBody(out var sessionJSON, out ocpiResponseBuilder))
                                            return ocpiResponseBuilder;

                                        if (!Session.TryParse(sessionJSON,
                                                              out var newOrUpdatedSession,
                                                              out var errorResponse,
                                                              countryCode,
                                                              partyId,
                                                              sessionId))
                                        {

                                            return new OCPIResponse.Builder(request) {
                                                       StatusCode           = 2001,
                                                       StatusMessage        = "Could not parse the given session JSON: " + errorResponse,
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                                       }
                                                   };

                                        }

                                        #endregion


                                        var addOrUpdateResult = await CommonAPI.AddOrUpdateSession(
                                                                          newOrUpdatedSession,
                                                                          AllowDowngrades ?? request.QueryString.GetBoolean("forceDowngrade")
                                                                      );


                                        if (addOrUpdateResult.IsSuccessAndDataNotNull(out var data))
                                            return new OCPIResponse.Builder(request) {
                                                       StatusCode           = 1000,
                                                       StatusMessage        = "Hello world!",
                                                       Data                 = data.ToJSON(CustomSessionSerializer,
                                                                                          CustomCDRTokenSerializer,
                                                                                          CustomChargingPeriodSerializer,
                                                                                          CustomCDRDimensionSerializer,
                                                                                          CustomPriceSerializer),
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                                           HTTPStatusCode             = addOrUpdateResult.WasCreated == true
                                                                                            ? HTTPStatusCode.Created
                                                                                            : HTTPStatusCode.OK,
                                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                           AccessControlAllowHeaders  = [ "Authorization" ],
                                                           LastModified               = data.LastUpdated,
                                                           ETag                       = data.ETag
                                                       }
                                                   };

                                        return new OCPIResponse.Builder(request) {
                                                   StatusCode           = 2000,
                                                   StatusMessage        = addOrUpdateResult.ErrorResponse,
                                                   Data                 = newOrUpdatedSession.ToJSON(CustomSessionSerializer,
                                                                                                     CustomCDRTokenSerializer,
                                                                                                     CustomChargingPeriodSerializer,
                                                                                                     CustomCDRDimensionSerializer,
                                                                                                     CustomPriceSerializer),
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                       AccessControlAllowHeaders  = [ "Authorization" ],
                                                       LastModified               = newOrUpdatedSession.LastUpdated,
                                                       ETag                       = newOrUpdatedSession.ETag
                                                   }
                                               };

                                    });

            #endregion

            #region PATCH    ~/sessions/{country_code}/{party_id}/{sessionId}

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.PATCH,
                                    URLPathPrefix + "sessions/{country_code}/{party_id}/{sessionId}",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestLogger:   PatchSessionRequest,
                                    OCPIResponseLogger:  PatchSessionResponse,
                                    OCPIRequestHandler:  async request => {

                                        #region Check access token

                                        if (request.LocalAccessInfo.IsNot(Role.CPO) ||
                                            request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                                        {

                                            return new OCPIResponse.Builder(request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = "Invalid or blocked access token!",
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                                       }
                                                   };

                                        }

                                        #endregion

                                        #region Check session

                                        if (!request.ParseMandatorySession(CommonAPI,
                                                                           out var countryCode,
                                                                           out var partyId,
                                                                           out var sessionId,
                                                                           out var existingSession,
                                                                           out var ocpiResponseBuilder))
                                        {
                                            return ocpiResponseBuilder;
                                        }

                                        #endregion

                                        #region Parse and apply Session JSON patch

                                        if (!request.TryParseJObjectRequestBody(out var sessionPatch, out ocpiResponseBuilder))
                                            return ocpiResponseBuilder;

                                        #endregion


                                        var patchedSession = await CommonAPI.TryPatchSession(
                                                                       Party_Idv3.From(
                                                                           countryCode.Value,
                                                                           partyId.    Value
                                                                       ),
                                                                       existingSession.Id,
                                                                       sessionPatch
                                                                   );


                                        //ToDo: Handle update errors!
                                        if (patchedSession.IsSuccessAndDataNotNull(out var data))
                                            return new OCPIResponse.Builder(request) {
                                                           StatusCode           = 1000,
                                                           StatusMessage        = "Hello world!",
                                                           Data                 = data.ToJSON(CustomSessionSerializer,
                                                                                              CustomCDRTokenSerializer,
                                                                                              CustomChargingPeriodSerializer,
                                                                                              CustomCDRDimensionSerializer,
                                                                                              CustomPriceSerializer),
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
                                                       StatusMessage        = patchedSession.ErrorResponse,
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.OK,
                                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                                       }
                                                   };

                                    });

            #endregion

            #region DELETE   ~/sessions/{country_code}/{party_id}/{sessionId}    [NonStandard]

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.DELETE,
                                    URLPathPrefix + "sessions/{country_code}/{party_id}/{sessionId}",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestLogger:   DeleteSessionRequest,
                                    OCPIResponseLogger:  DeleteSessionResponse,
                                    OCPIRequestHandler:  async request => {

                                        #region Check access token

                                        if (request.LocalAccessInfo.IsNot(Role.CPO) ||
                                            request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                                        {

                                            return new OCPIResponse.Builder(request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = "Invalid or blocked access token!",
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                                       }
                                                   };

                                        }

                                        #endregion

                                        #region Check existing session

                                        if (!request.ParseMandatorySession(CommonAPI,
                                                                           out var countryCode,
                                                                           out var partyId,
                                                                           out var sessionId,
                                                                           out var existingSession,
                                                                           out var ocpiResponseBuilder))
                                        {
                                            return ocpiResponseBuilder;
                                        }

                                        #endregion


                                        //ToDo: await...
                                        var result = await CommonAPI.RemoveSession(existingSession);


                                        return new OCPIResponse.Builder(request) {
                                                       StatusCode           = 1000,
                                                       StatusMessage        = "Hello world!",
                                                       Data                 = existingSession.ToJSON(CustomSessionSerializer,
                                                                                                     CustomCDRTokenSerializer,
                                                                                                     CustomChargingPeriodSerializer,
                                                                                                     CustomCDRDimensionSerializer,
                                                                                                     CustomPriceSerializer),
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



            // For EMSPs and SCSPs
            #region POST  ~/chargingprofiles/{session_id}

            // https://github.com/ocpi/ocpi/blob/release-2.2.1-bugfixes/mod_charging_profiles.asciidoc


            #region POST  ~/chargingprofiles/{session_id}/activeChargingProfile

            // ActiveChargingProfileResult
            // Result of the GET ActiveChargingProfile request, from the Charge Point.

            #endregion

            #region PUT   ~/chargingprofiles/{session_id}/activeChargingProfile

            // ActiveChargingProfile update

            #endregion


            #region POST  ~/chargingprofiles/{session_id}/chargingProfile

            // ChargingProfileResult
            // Result of the PUT ChargingProfile request, from the Charge Point.

            #endregion

            #region POST  ~/chargingprofiles/{session_id}/clearProfile

            // ClearProfileResult
            // Result of the DELETE ChargingProfile request, from the Charge Point.

            #endregion

            #endregion


        }

        #endregion


    }

}
