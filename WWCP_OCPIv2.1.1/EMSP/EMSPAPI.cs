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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_1_1.HTTP
{

    /// <summary>
    /// The HTTP API for e-mobility service providers.
    /// CPOs will connect to this API.
    /// </summary>
    public class EMSPAPI : HTTPAPI
    {

        #region Data

        /// <summary>
        /// The default HTTP server name.
        /// </summary>
        public new const           String    DefaultHTTPServiceName    = "GraphDefined OCPI EMSP HTTP API v0.1";

        /// <summary>
        /// The default HTTP server TCP port.
        /// </summary>
        public new static readonly IPPort    DefaultHTTPServerPort     = IPPort.Parse(8080);

        /// <summary>
        /// The default HTTP URL path prefix.
        /// </summary>
        public new static readonly HTTPPath  DefaultURLPathPrefix      = HTTPPath.Parse("/emsp");

        /// <summary>
        /// The default EMSP API logfile name.
        /// </summary>
        public  const              String    DefaultLogfileName       = "OCPI_EMSPAPI.log";

        protected Newtonsoft.Json.Formatting JSONFormat = Newtonsoft.Json.Formatting.Indented;

        #endregion

        #region Properties

        /// <summary>
        /// The CommonAPI.
        /// </summary>
        public CommonAPI       CommonAPI             { get; }

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
        /// The EMSP API logger.
        /// </summary>
        public EMSPAPILogger?  EMSPAPILogger         { get; }

        #endregion

        #region Custom JSON parsers

        #endregion

        #region Custom JSON serializers

        public CustomJObjectSerializerDelegate<Location>?                    CustomLocationSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<AdditionalGeoLocation>?       CustomAdditionalGeoLocationSerializer        { get; set; }
        public CustomJObjectSerializerDelegate<EVSE>?                        CustomEVSESerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<StatusSchedule>?              CustomStatusScheduleSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<Connector>?                   CustomConnectorSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<EnergyMeter<Location>>?       CustomLocationEnergyMeterSerializer          { get; set; }
        public CustomJObjectSerializerDelegate<EnergyMeter<EVSE>>?           CustomEVSEEnergyMeterSerializer              { get; set; }
        public CustomJObjectSerializerDelegate<TransparencySoftwareStatus>?  CustomTransparencySoftwareStatusSerializer   { get; set; }
        public CustomJObjectSerializerDelegate<TransparencySoftware>?        CustomTransparencySoftwareSerializer         { get; set; }
        public CustomJObjectSerializerDelegate<DisplayText>?                 CustomDisplayTextSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<BusinessDetails>?             CustomBusinessDetailsSerializer              { get; set; }
        public CustomJObjectSerializerDelegate<Hours>?                       CustomHoursSerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<Image>?                       CustomImageSerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<EnergyMix>?                   CustomEnergyMixSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<EnergySource>?                CustomEnergySourceSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<EnvironmentalImpact>?         CustomEnvironmentalImpactSerializer          { get; set; }


        public CustomJObjectSerializerDelegate<Tariff>?                      CustomTariffSerializer                       { get; set; }
        public CustomJObjectSerializerDelegate<TariffElement>?               CustomTariffElementSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<PriceComponent>?              CustomPriceComponentSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<TariffRestrictions>?          CustomTariffRestrictionsSerializer           { get; set; }

        public Boolean                                                       IncludeSessionOwnerInformation               { get; set; }
        public CustomJObjectSerializerDelegate<Session>?                     CustomSessionSerializer                      { get; set; }

        public CustomJObjectSerializerDelegate<CDR>?                         CustomCDRSerializer                          { get; set; }
        public CustomJObjectSerializerDelegate<ChargingPeriod>?              CustomChargingPeriodSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<CDRDimension>?                CustomCDRDimensionSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<CDRCostDetails>?              CustomCDRCostDetailsSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<SignedData>?                  CustomSignedDataSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<SignedValue>?                 CustomSignedValueSerializer                  { get; set; }


        public CustomJObjectSerializerDelegate<Token>?                       CustomTokenSerializer                        { get; set; }


        public CustomJObjectSerializerDelegate<AuthorizationInfo>?           CustomAuthorizationInfoSerializer            { get; set; }
        public CustomJObjectSerializerDelegate<LocationReference>?           CustomLocationReferenceSerializer            { get; set; }

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


        #region (protected internal) DeleteLocationsRequest (Request)

        /// <summary>
        /// An event sent whenever a delete locations request was received.
        /// </summary>
        public OCPIRequestLogEvent OnDeleteLocationsRequest = new ();

        /// <summary>
        /// An event sent whenever a delete locations request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task DeleteLocationsRequest(DateTime     Timestamp,
                                                       HTTPAPI      API,
                                                       OCPIRequest  Request)

            => OnDeleteLocationsRequest.WhenAll(Timestamp,
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
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task DeleteLocationsResponse(DateTime      Timestamp,
                                                        HTTPAPI       API,
                                                        OCPIRequest   Request,
                                                        OCPIResponse  Response)

            => OnDeleteLocationsResponse.WhenAll(Timestamp,
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


        #region (protected internal) PutLocationRequest    (Request)

        /// <summary>
        /// An event sent whenever a PUT location request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPutLocationRequest = new ();

        /// <summary>
        /// An event sent whenever a PUT location request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PutLocationRequest(DateTime     Timestamp,
                                                   HTTPAPI      API,
                                                   OCPIRequest  Request)

            => OnPutLocationRequest.WhenAll(Timestamp,
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
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PutLocationResponse(DateTime      Timestamp,
                                                    HTTPAPI       API,
                                                    OCPIRequest   Request,
                                                    OCPIResponse  Response)

            => OnPutLocationResponse.WhenAll(Timestamp,
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
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PatchLocationRequest(DateTime     Timestamp,
                                                     HTTPAPI      API,
                                                     OCPIRequest  Request)

            => OnPatchLocationRequest.WhenAll(Timestamp,
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
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PatchLocationResponse(DateTime      Timestamp,
                                                      HTTPAPI       API,
                                                      OCPIRequest   Request,
                                                      OCPIResponse  Response)

            => OnPatchLocationResponse.WhenAll(Timestamp,
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
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task DeleteLocationRequest(DateTime     Timestamp,
                                                      HTTPAPI      API,
                                                      OCPIRequest  Request)

            => OnDeleteLocationRequest.WhenAll(Timestamp,
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
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task DeleteLocationResponse(DateTime      Timestamp,
                                                       HTTPAPI       API,
                                                       OCPIRequest   Request,
                                                       OCPIResponse  Response)

            => OnDeleteLocationResponse.WhenAll(Timestamp,
                                                API ?? this,
                                                Request,
                                                Response);

        #endregion

        #endregion

        #region EVSE/EVSE status

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


        #region (protected internal) PutEVSERequest    (Request)

        /// <summary>
        /// An event sent whenever a PUT EVSE request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPutEVSERequest = new ();

        /// <summary>
        /// An event sent whenever a PUT EVSE request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PutEVSERequest(DateTime     Timestamp,
                                               HTTPAPI      API,
                                               OCPIRequest  Request)

            => OnPutEVSERequest.WhenAll(Timestamp,
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
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PutEVSEResponse(DateTime      Timestamp,
                                                HTTPAPI       API,
                                                OCPIRequest   Request,
                                                OCPIResponse  Response)

            => OnPutEVSEResponse.WhenAll(Timestamp,
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
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PatchEVSERequest(DateTime     Timestamp,
                                                 HTTPAPI      API,
                                                 OCPIRequest  Request)

            => OnPatchEVSERequest.WhenAll(Timestamp,
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
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PatchEVSEResponse(DateTime      Timestamp,
                                                  HTTPAPI       API,
                                                  OCPIRequest   Request,
                                                  OCPIResponse  Response)

            => OnPatchEVSEResponse.WhenAll(Timestamp,
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
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task DeleteEVSERequest(DateTime     Timestamp,
                                                  HTTPAPI      API,
                                                  OCPIRequest  Request)

            => OnDeleteEVSERequest.WhenAll(Timestamp,
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
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task DeleteEVSEResponse(DateTime      Timestamp,
                                                   HTTPAPI       API,
                                                   OCPIRequest   Request,
                                                   OCPIResponse  Response)

            => OnDeleteEVSEResponse.WhenAll(Timestamp,
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
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PostEVSEStatusRequest(DateTime     Timestamp,
                                                      HTTPAPI      API,
                                                      OCPIRequest  Request)

            => OnPostEVSEStatusRequest.WhenAll(Timestamp,
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
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PostEVSEStatusResponse(DateTime      Timestamp,
                                                       HTTPAPI       API,
                                                       OCPIRequest   Request,
                                                       OCPIResponse  Response)

            => OnPostEVSEStatusResponse.WhenAll(Timestamp,
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


        #region (protected internal) PutConnectorRequest    (Request)

        /// <summary>
        /// An event sent whenever a PUT connector request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPutConnectorRequest = new ();

        /// <summary>
        /// An event sent whenever a PUT connector request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PutConnectorRequest(DateTime     Timestamp,
                                                    HTTPAPI      API,
                                                    OCPIRequest  Request)

            => OnPutConnectorRequest.WhenAll(Timestamp,
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
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PutConnectorResponse(DateTime      Timestamp,
                                                     HTTPAPI       API,
                                                     OCPIRequest   Request,
                                                     OCPIResponse  Response)

            => OnPutConnectorResponse.WhenAll(Timestamp,
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
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PatchConnectorRequest(DateTime     Timestamp,
                                                      HTTPAPI      API,
                                                      OCPIRequest  Request)

            => OnPatchConnectorRequest.WhenAll(Timestamp,
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
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PatchConnectorResponse(DateTime      Timestamp,
                                                       HTTPAPI       API,
                                                       OCPIRequest   Request,
                                                       OCPIResponse  Response)

            => OnPatchConnectorResponse.WhenAll(Timestamp,
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
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task DeleteConnectorRequest(DateTime     Timestamp,
                                                       HTTPAPI      API,
                                                       OCPIRequest  Request)

            => OnDeleteConnectorRequest.WhenAll(Timestamp,
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
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task DeleteConnectorResponse(DateTime      Timestamp,
                                                        HTTPAPI       API,
                                                        OCPIRequest   Request,
                                                        OCPIResponse  Response)

            => OnDeleteConnectorResponse.WhenAll(Timestamp,
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


        #region (protected internal) DeleteTariffsRequest (Request)

        /// <summary>
        /// An event sent whenever a DELETE tariffs request was received.
        /// </summary>
        public OCPIRequestLogEvent OnDeleteTariffsRequest = new ();

        /// <summary>
        /// An event sent whenever a DELETE tariffs request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task DeleteTariffsRequest(DateTime     Timestamp,
                                                     HTTPAPI      API,
                                                     OCPIRequest  Request)

            => OnDeleteTariffsRequest.WhenAll(Timestamp,
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
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task DeleteTariffsResponse(DateTime      Timestamp,
                                                      HTTPAPI       API,
                                                      OCPIRequest   Request,
                                                      OCPIResponse  Response)

            => OnDeleteTariffsResponse.WhenAll(Timestamp,
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


        #region (protected internal) PutTariffRequest    (Request)

        /// <summary>
        /// An event sent whenever a PUT tariff request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPutTariffRequest = new ();

        /// <summary>
        /// An event sent whenever a PUT tariff request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PutTariffRequest(DateTime     Timestamp,
                                                 HTTPAPI      API,
                                                 OCPIRequest  Request)

            => OnPutTariffRequest.WhenAll(Timestamp,
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
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PutTariffResponse(DateTime      Timestamp,
                                                  HTTPAPI       API,
                                                  OCPIRequest   Request,
                                                  OCPIResponse  Response)

            => OnPutTariffResponse.WhenAll(Timestamp,
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
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PatchTariffRequest(DateTime     Timestamp,
                                                   HTTPAPI      API,
                                                   OCPIRequest  Request)

            => OnPatchTariffRequest.WhenAll(Timestamp,
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
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PatchTariffResponse(DateTime      Timestamp,
                                                    HTTPAPI       API,
                                                    OCPIRequest   Request,
                                                    OCPIResponse  Response)

            => OnPatchTariffResponse.WhenAll(Timestamp,
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
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task DeleteTariffRequest(DateTime     Timestamp,
                                                    HTTPAPI      API,
                                                    OCPIRequest  Request)

            => OnDeleteTariffRequest.WhenAll(Timestamp,
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
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task DeleteTariffResponse(DateTime      Timestamp,
                                                     HTTPAPI       API,
                                                     OCPIRequest   Request,
                                                     OCPIResponse  Response)

            => OnDeleteTariffResponse.WhenAll(Timestamp,
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


        #region (protected internal) DeleteSessionsRequest (Request)

        /// <summary>
        /// An event sent whenever a DELETE sessions request was received.
        /// </summary>
        public OCPIRequestLogEvent OnDeleteSessionsRequest = new ();

        /// <summary>
        /// An event sent whenever a DELETE sessions request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task DeleteSessionsRequest(DateTime     Timestamp,
                                                      HTTPAPI      API,
                                                      OCPIRequest  Request)

            => OnDeleteSessionsRequest.WhenAll(Timestamp,
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
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task DeleteSessionsResponse(DateTime      Timestamp,
                                                       HTTPAPI       API,
                                                       OCPIRequest   Request,
                                                       OCPIResponse  Response)

            => OnDeleteSessionsResponse.WhenAll(Timestamp,
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


        #region (protected internal) PutSessionRequest    (Request)

        /// <summary>
        /// An event sent whenever a PUT session request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPutSessionRequest = new ();

        /// <summary>
        /// An event sent whenever a PUT session request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PutSessionRequest(DateTime     Timestamp,
                                                  HTTPAPI      API,
                                                  OCPIRequest  Request)

            => OnPutSessionRequest.WhenAll(Timestamp,
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
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PutSessionResponse(DateTime      Timestamp,
                                                   HTTPAPI       API,
                                                   OCPIRequest   Request,
                                                   OCPIResponse  Response)

            => OnPutSessionResponse.WhenAll(Timestamp,
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
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PatchSessionRequest(DateTime     Timestamp,
                                                    HTTPAPI      API,
                                                    OCPIRequest  Request)

            => OnPatchSessionRequest.WhenAll(Timestamp,
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
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PatchSessionResponse(DateTime      Timestamp,
                                                     HTTPAPI       API,
                                                     OCPIRequest   Request,
                                                     OCPIResponse  Response)

            => OnPatchSessionResponse.WhenAll(Timestamp,
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
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task DeleteSessionRequest(DateTime     Timestamp,
                                                     HTTPAPI      API,
                                                     OCPIRequest  Request)

            => OnDeleteSessionRequest.WhenAll(Timestamp,
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
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task DeleteSessionResponse(DateTime      Timestamp,
                                                      HTTPAPI       API,
                                                      OCPIRequest   Request,
                                                      OCPIResponse  Response)

            => OnDeleteSessionResponse.WhenAll(Timestamp,
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


        #region (protected internal) DeleteCDRsRequest (Request)

        /// <summary>
        /// An event sent whenever a DELETE CDRs request was received.
        /// </summary>
        public OCPIRequestLogEvent OnDeleteCDRsRequest = new ();

        /// <summary>
        /// An event sent whenever a DELETE CDRs request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task DeleteCDRsRequest(DateTime     Timestamp,
                                                  HTTPAPI      API,
                                                  OCPIRequest  Request)

            => OnDeleteCDRsRequest.WhenAll(Timestamp,
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
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task DeleteCDRsResponse(DateTime      Timestamp,
                                                   HTTPAPI       API,
                                                   OCPIRequest   Request,
                                                   OCPIResponse  Response)

            => OnDeleteCDRsResponse.WhenAll(Timestamp,
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


        #region (protected internal) PostCDRRequest (Request)

        /// <summary>
        /// An event sent whenever a POST CDR request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPostCDRRequest = new ();

        /// <summary>
        /// An event sent whenever a POST CDR request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PostCDRRequest(DateTime     Timestamp,
                                               HTTPAPI      API,
                                               OCPIRequest  Request)

            => OnPostCDRRequest.WhenAll(Timestamp,
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
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PostCDRResponse(DateTime      Timestamp,
                                                HTTPAPI       API,
                                                OCPIRequest   Request,
                                                OCPIResponse  Response)

            => OnPostCDRResponse.WhenAll(Timestamp,
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
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task DeleteCDRRequest(DateTime     Timestamp,
                                                 HTTPAPI      API,
                                                 OCPIRequest  Request)

            => OnDeleteCDRRequest.WhenAll(Timestamp,
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
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task DeleteCDRResponse(DateTime      Timestamp,
                                                  HTTPAPI       API,
                                                  OCPIRequest   Request,
                                                  OCPIResponse  Response)

            => OnDeleteCDRResponse.WhenAll(Timestamp,
                                           API ?? this,
                                           Request,
                                           Response);

        #endregion

        #endregion

        #region Tokens

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

        #endregion


        public delegate Task<AuthorizationInfo> OnRFIDAuthTokenDelegate(CountryCode         CountryCode,
                                                                        Party_Id            PartyId,
                                                                        Token_Id            TokenId,
                                                                        LocationReference?  LocationReference);

        public event OnRFIDAuthTokenDelegate? OnRFIDAuthToken;



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
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task ReserveNowCallbackRequest(DateTime     Timestamp,
                                                          HTTPAPI      API,
                                                          OCPIRequest  Request)

            => OnReserveNowCallbackRequest.WhenAll(Timestamp,
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
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task ReserveNowCallbackResponse(DateTime      Timestamp,
                                                           HTTPAPI       API,
                                                           OCPIRequest   Request,
                                                           OCPIResponse  Response)

            => OnReserveNowCallbackResponse.WhenAll(Timestamp,
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
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task CancelReservationCallbackRequest(DateTime     Timestamp,
                                                                 HTTPAPI      API,
                                                                 OCPIRequest  Request)

            => OnCancelReservationCallbackRequest.WhenAll(Timestamp,
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
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task CancelReservationCallbackResponse(DateTime      Timestamp,
                                                                  HTTPAPI       API,
                                                                  OCPIRequest   Request,
                                                                  OCPIResponse  Response)

            => OnCancelReservationCallbackResponse.WhenAll(Timestamp,
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
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task StartSessionCallbackRequest(DateTime     Timestamp,
                                                            HTTPAPI      API,
                                                            OCPIRequest  Request)

            => OnStartSessionCallbackRequest.WhenAll(Timestamp,
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
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task StartSessionCallbackResponse(DateTime      Timestamp,
                                                             HTTPAPI       API,
                                                             OCPIRequest   Request,
                                                             OCPIResponse  Response)

            => OnStartSessionCallbackResponse.WhenAll(Timestamp,
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
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task StopSessionCallbackRequest(DateTime     Timestamp,
                                                           HTTPAPI      API,
                                                           OCPIRequest  Request)

            => OnStopSessionCallbackRequest.WhenAll(Timestamp,
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
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task StopSessionCallbackResponse(DateTime      Timestamp,
                                                            HTTPAPI       API,
                                                            OCPIRequest   Request,
                                                            OCPIResponse  Response)

            => OnStopSessionCallbackResponse.WhenAll(Timestamp,
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
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task UnlockConnectorCallbackRequest(DateTime     Timestamp,
                                                               HTTPAPI      API,
                                                               OCPIRequest  Request)

            => OnUnlockConnectorCallbackRequest.WhenAll(Timestamp,
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
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task UnlockConnectorCallbackResponse(DateTime      Timestamp,
                                                                HTTPAPI       API,
                                                                OCPIRequest   Request,
                                                                OCPIResponse  Response)

            => OnUnlockConnectorCallbackResponse.WhenAll(Timestamp,
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
        /// <param name="CommonAPI">The OCPI CommonAPI.</param>
        /// <param name="AllowDowngrades">(Dis-)allow PUTting of object having an earlier 'LastUpdated'-timestamp then already existing objects.</param>
        /// 
        /// <param name="HTTPHostname">An optional HTTP hostname.</param>
        /// <param name="ExternalDNSName">The offical URL/DNS name of this service, e.g. for sending e-mails.</param>
        /// <param name="HTTPServiceName">An optional name of the HTTP API service.</param>
        /// <param name="BasePath">When the API is served from an optional subdirectory path.</param>
        /// 
        /// <param name="URLPathPrefix">An optional URL path prefix, used when defining URL templates.</param>
        /// <param name="APIVersionHashes">The API version hashes (git commit hash values).</param>
        /// 
        /// <param name="DisableMaintenanceTasks">Disable all maintenance tasks.</param>
        /// <param name="MaintenanceInitialDelay">The initial delay of the maintenance tasks.</param>
        /// <param name="MaintenanceEvery">The maintenance intervall.</param>
        /// 
        /// <param name="DisableWardenTasks">Disable all warden tasks.</param>
        /// <param name="WardenInitialDelay">The initial delay of the warden tasks.</param>
        /// <param name="WardenCheckEvery">The warden intervall.</param>
        /// 
        /// <param name="IsDevelopment">This HTTP API runs in development mode.</param>
        /// <param name="DevelopmentServers">An enumeration of server names which will imply to run this service in development mode.</param>
        /// <param name="DisableLogging">Disable the log file.</param>
        /// <param name="LoggingPath">The path for all logfiles.</param>
        /// <param name="LogfileName">The name of the logfile.</param>
        /// <param name="LogfileCreator">A delegate for creating the name of the logfile for this API.</param>
        /// <param name="AutoStart">Whether to start the API automatically.</param>
        public EMSPAPI(CommonAPI                    CommonAPI,
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
                       String?                      LogfileName               = DefaultLogfileName,
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
            this.RequestTimeout   = TimeSpan.FromSeconds(30);

            this.EMSPAPILogger    = this.DisableLogging == false
                                        ? new EMSPAPILogger(
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
            //                                   URLPathPrefix + "/emsp", "cloud.charging.open.protocols.OCPIv2_1_1.HTTPAPI.EMSPAPI.HTTPRoot",
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
            //                                 typeof(EMSPAPI).Assembly.GetManifestResourceStream("cloud.charging.open.protocols.OCPIv2_1_1.HTTPAPI.EMSPAPI.HTTPRoot._header.html").SeekAndCopyTo(_MemoryStream, 3);
            //                                 typeof(EMSPAPI).Assembly.GetManifestResourceStream("cloud.charging.open.protocols.OCPIv2_1_1.HTTPAPI.EMSPAPI.HTTPRoot._footer.html").SeekAndCopyTo(_MemoryStream, 3);

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

            #region ~/locations                               [NonStandard]

            #region OPTIONS  ~/locations      [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPHostname.Any,
                HTTPMethod.OPTIONS,
                URLPathPrefix + "locations",
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

            #region GET      ~/locations      [NonStandard]

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.GET,
                                    URLPathPrefix + "locations",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestLogger:   GetLocationsRequest,
                                    OCPIResponseLogger:  GetLocationsResponse,
                                    OCPIRequestHandler:  Request => {

                                        #region Check access token

                                        if (Request.LocalAccessInfo is null ||
                                            Request.LocalAccessInfo.Status != AccessStatus.ALLOWED ||
                                            Request.LocalAccessInfo.Role   != Roles.CPO)
                                        {

                                            return Task.FromResult(
                                                new OCPIResponse.Builder(Request) {
                                                    StatusCode           = 2000,
                                                    StatusMessage        = "Invalid or blocked access token!",
                                                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                        AccessControlAllowMethods  = [ "OPTIONS", "GET", "DELETE" ],
                                                        AccessControlAllowHeaders  = [ "Authorization" ]
                                                    }
                                                });

                                        }

                                        #endregion


                                        var filters            = Request.GetDateAndPaginationFilters();

                                        var allLocations       = CommonAPI.GetLocations(Request.LocalAccessInfo.CountryCode,
                                                                                        Request.LocalAccessInfo.PartyId).
                                                                           ToArray();

                                        var filteredLocations  = allLocations.Where(location => !filters.From.HasValue || location.LastUpdated >  filters.From.Value).
                                                                              Where(location => !filters.To.  HasValue || location.LastUpdated <= filters.To.  Value).
                                                                              ToArray();


                                        return Task.FromResult(
                                            new OCPIResponse.Builder(Request) {
                                                   StatusCode           = 1000,
                                                   StatusMessage        = "Hello world!",
                                                   Data                 = new JArray(
                                                                              filteredLocations.
                                                                                  SkipTakeFilter(filters.Offset,
                                                                                                 filters.Limit).
                                                                                  Select(location => location.ToJSON(false,
                                                                                                                     false,
                                                                                                                     Request.EMSPId,
                                                                                                                     CustomLocationSerializer,
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
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
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

            #region DELETE   ~/locations      [NonStandard]

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.DELETE,
                                    URLPathPrefix + "locations",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestLogger:   DeleteLocationsRequest,
                                    OCPIResponseLogger:  DeleteLocationsResponse,
                                    OCPIRequestHandler:  async Request => {

                                        #region Check access token

                                        if (Request.LocalAccessInfo?.Status != AccessStatus.ALLOWED ||
                                            Request.LocalAccessInfo?.Role   != Roles.CPO)
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = "Invalid or blocked access token!",
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "DELETE" ],
                                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                                       }
                                                   };

                                        }

                                        #endregion


                                        //ToDo: await...
                                        await CommonAPI.RemoveAllLocations();


                                        return new OCPIResponse.Builder(Request) {
                                                   StatusCode           = 1000,
                                                   StatusMessage        = "Hello world!",
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "DELETE" ],
                                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                                   }
                                               };

                                    });

            #endregion

            #endregion

            #region ~/locations/{locationId}

            #region OPTIONS  ~/locations/{locationId}      [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPHostname.Any,
                HTTPMethod.OPTIONS,
                URLPathPrefix + "locations/{locationId}",
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

            #region GET      ~/locations/{locationId}

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.GET,
                                    URLPathPrefix + "locations/{locationId}",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestLogger:   GetLocationRequest,
                                    OCPIResponseLogger:  GetLocationResponse,
                                    OCPIRequestHandler:  Request => {

                                        #region Check access token

                                        if (Request.LocalAccessInfo is null ||
                                            Request.LocalAccessInfo.Status != AccessStatus.ALLOWED ||
                                            Request.LocalAccessInfo.Role   != Roles.CPO)
                                        {

                                            return Task.FromResult(
                                                new OCPIResponse.Builder(Request) {
                                                    StatusCode           = 2000,
                                                    StatusMessage        = "Invalid or blocked access token!",
                                                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                        AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                        AccessControlAllowHeaders  = [ "Authorization" ]
                                                    }
                                                });

                                        }

                                        #endregion

                                        #region Check location

                                        if (!Request.ParseLocation(CommonAPI,
                                                                   Request.LocalAccessInfo.CountryCode,
                                                                   Request.LocalAccessInfo.PartyId,
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
                                                   Data                 = location.ToJSON(false,
                                                                                          false,
                                                                                          Request.EMSPId,
                                                                                          CustomLocationSerializer,
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
                                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                       AccessControlAllowHeaders  = [ "Authorization" ],
                                                       LastModified               = location.LastUpdated,
                                                       ETag                       = location.ETag
                                                   }
                                            });

                                    });

            #endregion

            #region PUT      ~/locations/{locationId}

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.PUT,
                                    URLPathPrefix + "locations/{locationId}",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestLogger:   PutLocationRequest,
                                    OCPIResponseLogger:  PutLocationResponse,
                                    OCPIRequestHandler:  async Request => {

                                        #region Check access token

                                        if (Request.LocalAccessInfo is null ||
                                            Request.LocalAccessInfo.Status != AccessStatus.ALLOWED ||
                                            Request.LocalAccessInfo.Role   != Roles.CPO)
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = "Invalid or blocked access token!",
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                                       }
                                                   };

                                        }

                                        #endregion

                                        #region Check existing location

                                        if (!Request.ParseLocation(CommonAPI,
                                                                   Request.LocalAccessInfo.CountryCode,
                                                                   Request.LocalAccessInfo.PartyId,
                                                                   out var locationId,
                                                                   out var existingLocation,
                                                                   out var ocpiResponseBuilder,
                                                                   FailOnMissingLocation: false))
                                        {
                                            return ocpiResponseBuilder!;
                                        }

                                        #endregion

                                        #region Parse new or updated location JSON

                                        if (!Request.TryParseJObjectRequestBody(out var locationJSON, out ocpiResponseBuilder))
                                            return ocpiResponseBuilder;

                                        if (!Location.TryParse(locationJSON,
                                                               out var newOrUpdatedLocation,
                                                               out var errorResponse,
                                                               Request.LocalAccessInfo.CountryCode,
                                                               Request.LocalAccessInfo.PartyId,
                                                               locationId) ||
                                             newOrUpdatedLocation is null)
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2001,
                                                       StatusMessage        = "Could not parse the given location JSON: " + errorResponse,
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                                       }
                                                   };

                                        }

                                        #endregion


                                        var addOrUpdateResult = await CommonAPI.AddOrUpdateLocation(newOrUpdatedLocation,
                                                                                                    AllowDowngrades ?? Request.QueryString.GetBoolean("forceDowngrade"));


                                        if (addOrUpdateResult.IsSuccess &&
                                            addOrUpdateResult.Data is not null)
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 1000,
                                                       StatusMessage        = "Hello world!",
                                                       Data                 = addOrUpdateResult.Data.ToJSON(false,
                                                                                                            false,
                                                                                                            Request.EMSPId,
                                                                                                            CustomLocationSerializer,
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
                                                           HTTPStatusCode             = addOrUpdateResult.WasCreated == true
                                                                                            ? HTTPStatusCode.Created
                                                                                            : HTTPStatusCode.OK,
                                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                           AccessControlAllowHeaders  = [ "Authorization" ],
                                                           LastModified               = addOrUpdateResult.Data.LastUpdated,
                                                           ETag                       = addOrUpdateResult.Data.ETag
                                                       }
                                                   };

                                        }

                                        return new OCPIResponse.Builder(Request) {
                                                   StatusCode           = 2000,
                                                   StatusMessage        = addOrUpdateResult.ErrorResponse,
                                                   Data                 = newOrUpdatedLocation.ToJSON(false,
                                                                                                      false,
                                                                                                      Request.EMSPId,
                                                                                                      CustomLocationSerializer,
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
                                                       HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                       AccessControlAllowHeaders  = [ "Authorization" ],
                                                       LastModified               = addOrUpdateResult.Data.LastUpdated,
                                                       ETag                       = addOrUpdateResult.Data.ETag
                                                   }
                                               };

                                    });

            #endregion

            #region PATCH    ~/locations/{locationId}

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.PATCH,
                                    URLPathPrefix + "locations/{locationId}",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestLogger:   PatchLocationRequest,
                                    OCPIResponseLogger:  PatchLocationResponse,
                                    OCPIRequestHandler:  async Request => {

                                        #region Check access token

                                        if (Request.LocalAccessInfo is null ||
                                            Request.LocalAccessInfo.Status != AccessStatus.ALLOWED ||
                                            Request.LocalAccessInfo.Role   != Roles.CPO)
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = "Invalid or blocked access token!",
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                                       }
                                                   };

                                        }

                                        #endregion

                                        #region Check location

                                        if (!Request.ParseLocation(CommonAPI,
                                                                   Request.LocalAccessInfo.CountryCode,
                                                                   Request.LocalAccessInfo.PartyId,
                                                                   out var locationId,
                                                                   out var location,
                                                                   out var ocpiResponseBuilder,
                                                                   FailOnMissingLocation: true))
                                        {
                                            return ocpiResponseBuilder;
                                        }

                                        #endregion

                                        #region Parse location JSON patch

                                        if (!Request.TryParseJObjectRequestBody(out var locationPatch, out ocpiResponseBuilder))
                                            return ocpiResponseBuilder;

                                        #endregion


                                        // Validation-Checks for PATCHes
                                        // (E-Tag, Timestamp, ...)

                                        var result = await CommonAPI.TryPatchLocation(
                                                               locationId.Value,
                                                               locationPatch
                                                           );

                                        //ToDo: Handle update errors!
                                        if (result.IsSuccess)
                                            return new OCPIResponse.Builder(Request) {
                                                           StatusCode           = 1000,
                                                           StatusMessage        = "Hello world!",
                                                           Data                 = result.PatchedData.ToJSON(false,
                                                                                                            false,
                                                                                                            Request.EMSPId,
                                                                                                            CustomLocationSerializer,
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
                                                               AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                               AccessControlAllowHeaders  = [ "Authorization" ],
                                                               LastModified               = result.PatchedData.LastUpdated,
                                                               ETag                       = result.PatchedData.ETag
                                                           }
                                                       };

                                        return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = result.ErrorResponse,
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.OK,
                                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                                       }
                                                   };

                                    });

            #endregion

            #region DELETE   ~/locations/{locationId}      [NonStandard]

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.DELETE,
                                    URLPathPrefix + "locations/{locationId}",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestLogger:   DeleteLocationRequest,
                                    OCPIResponseLogger:  DeleteLocationResponse,
                                    OCPIRequestHandler:  async Request => {

                                        #region Check access token

                                        if (Request.LocalAccessInfo is null ||
                                            Request.LocalAccessInfo.Status != AccessStatus.ALLOWED ||
                                            Request.LocalAccessInfo.Role   != Roles.CPO)
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = "Invalid or blocked access token!",
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                                       }
                                                   };

                                        }

                                        #endregion

                                        #region Check existing location

                                        if (!Request.ParseLocation(CommonAPI,
                                                                   Request.LocalAccessInfo.CountryCode,
                                                                   Request.LocalAccessInfo.PartyId,
                                                                   out var locationId,
                                                                   out var location,
                                                                   out var ocpiResponseBuilder) ||
                                             location is null)
                                        {
                                            return ocpiResponseBuilder!;
                                        }

                                        #endregion


                                        //ToDo: await...
                                        await CommonAPI.RemoveLocation(location!);


                                        return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 1000,
                                                       StatusMessage        = "Hello world!",
                                                       Data                 = location.ToJSON(false,
                                                                                              false,
                                                                                              Request.EMSPId,
                                                                                              CustomLocationSerializer,
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
                                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                           AccessControlAllowHeaders  = [ "Authorization" ],
                                                           LastModified               = location.LastUpdated,
                                                           ETag                       = location.ETag
                                                       }
                                                   };

                                    });

            #endregion

            #endregion

            #region ~/locations/{locationId}/{evseId}

            #region OPTIONS  ~/locations/{locationId}/{evseId}      [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPHostname.Any,
                HTTPMethod.OPTIONS,
                URLPathPrefix + "locations/{locationId}/{evseId}",
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

            #region GET      ~/locations/{locationId}/{evseId}

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.GET,
                                    URLPathPrefix + "locations/{locationId}/{evseId}",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestLogger:   GetEVSERequest,
                                    OCPIResponseLogger:  GetEVSEResponse,
                                    OCPIRequestHandler:  Request => {

                                        #region Check access token

                                        if (Request.LocalAccessInfo is null ||
                                            Request.LocalAccessInfo.Status != AccessStatus.ALLOWED ||
                                            Request.LocalAccessInfo.Role   != Roles.CPO)
                                        {

                                            return Task.FromResult(
                                                new OCPIResponse.Builder(Request) {
                                                    StatusCode           = 2000,
                                                    StatusMessage        = "Invalid or blocked access token!",
                                                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                        AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                        AccessControlAllowHeaders  = [ "Authorization" ]
                                                    }
                                                });

                                        }

                                        #endregion

                                        #region Check EVSE

                                        if (!Request.ParseLocationEVSE(CommonAPI,
                                                                       Request.LocalAccessInfo.CountryCode,
                                                                       Request.LocalAccessInfo.PartyId,
                                                                       out var locationId,
                                                                       out var location,
                                                                       out var evseUId,
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
                                                                                      false,
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
                                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                       AccessControlAllowHeaders  = [ "Authorization" ],
                                                       LastModified               = evse.LastUpdated,
                                                       ETag                       = evse.ETag
                                                   }
                                            });

                                    });

            #endregion

            #region PUT      ~/locations/{locationId}/{evseId}

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.PUT,
                                    URLPathPrefix + "locations/{locationId}/{evseId}",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestLogger:   PutEVSERequest,
                                    OCPIResponseLogger:  PutEVSEResponse,
                                    OCPIRequestHandler:  async Request => {

                                        #region Check access token

                                        if (Request.LocalAccessInfo is null ||
                                            Request.LocalAccessInfo.Status != AccessStatus.ALLOWED ||
                                            Request.LocalAccessInfo.Role   != Roles.CPO)
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = "Invalid or blocked access token!",
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                                       }
                                                   };

                                        }

                                        #endregion

                                        #region Check existing EVSE

                                        if (!Request.ParseLocationEVSE(CommonAPI,
                                                                       Request.LocalAccessInfo.CountryCode,
                                                                       Request.LocalAccessInfo.PartyId,
                                                                       out var locationId,
                                                                       out var existingLocation,
                                                                       out var evseUId,
                                                                       out var existingEVSE,
                                                                       out var ocpiResponseBuilder,
                                                                       FailOnMissingEVSE: false) ||
                                             existingLocation is null)
                                        {
                                            return ocpiResponseBuilder!;
                                        }

                                        #endregion

                                        #region Parse new or updated EVSE JSON

                                        if (!Request.TryParseJObjectRequestBody(out var evseJSON, out ocpiResponseBuilder))
                                            return ocpiResponseBuilder;

                                        if (!EVSE.TryParse(evseJSON,
                                                           out var newOrUpdatedEVSE,
                                                           out var errorResponse,
                                                           evseUId) ||
                                             newOrUpdatedEVSE is null)
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2001,
                                                       StatusMessage        = "Could not parse the given EVSE JSON: " + errorResponse,
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                                       }
                                                   };

                                        }

                                        #endregion


                                        var addOrUpdateResult = await CommonAPI.AddOrUpdateEVSE(existingLocation,
                                                                                                newOrUpdatedEVSE,
                                                                                                AllowDowngrades ?? Request.QueryString.GetBoolean("forceDowngrade"));


                                        if (addOrUpdateResult.IsSuccess)
                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 1000,
                                                       StatusMessage        = "Hello world!",
                                                       Data                 = addOrUpdateResult.Data.ToJSON(Request.EMSPId,
                                                                                                            false,
                                                                                                            CustomEVSESerializer,
                                                                                                            CustomStatusScheduleSerializer,
                                                                                                            CustomConnectorSerializer,
                                                                                                            CustomEVSEEnergyMeterSerializer,
                                                                                                            CustomTransparencySoftwareStatusSerializer,
                                                                                                            CustomTransparencySoftwareSerializer,
                                                                                                            CustomDisplayTextSerializer,
                                                                                                            CustomImageSerializer),
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = addOrUpdateResult.WasCreated == true
                                                                                            ? HTTPStatusCode.Created
                                                                                            : HTTPStatusCode.OK,
                                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                           AccessControlAllowHeaders  = [ "Authorization" ],
                                                           LastModified               = newOrUpdatedEVSE.LastUpdated,
                                                           ETag                       = newOrUpdatedEVSE.ETag
                                                       }
                                                   };

                                        return new OCPIResponse.Builder(Request) {
                                                   StatusCode           = 2000,
                                                   StatusMessage        = addOrUpdateResult.ErrorResponse,
                                                   Data                 = newOrUpdatedEVSE.ToJSON(Request.EMSPId,
                                                                                                  false,
                                                                                                  CustomEVSESerializer,
                                                                                                  CustomStatusScheduleSerializer,
                                                                                                  CustomConnectorSerializer,
                                                                                                  CustomEVSEEnergyMeterSerializer,
                                                                                                  CustomTransparencySoftwareStatusSerializer,
                                                                                                  CustomTransparencySoftwareSerializer,
                                                                                                  CustomDisplayTextSerializer,
                                                                                                  CustomImageSerializer),
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                       AccessControlAllowHeaders  = [ "Authorization" ],
                                                       LastModified               = newOrUpdatedEVSE.LastUpdated,
                                                       ETag                       = newOrUpdatedEVSE.ETag
                                                   }
                                               };

                                    });

            #endregion

            #region PATCH    ~/locations/{locationId}/{evseId}

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.PATCH,
                                    URLPathPrefix + "locations/{locationId}/{evseId}",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestLogger:   PatchEVSERequest,
                                    OCPIResponseLogger:  PatchEVSEResponse,
                                    OCPIRequestHandler:   async Request => {

                                        #region Check access token

                                        if (Request.LocalAccessInfo is null ||
                                            Request.LocalAccessInfo.Status != AccessStatus.ALLOWED ||
                                            Request.LocalAccessInfo.Role   != Roles.CPO)
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = "Invalid or blocked access token!",
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                                       }
                                                   };

                                        }

                                        #endregion

                                        #region Check EVSE

                                        if (!Request.ParseLocationEVSE(CommonAPI,
                                                                       Request.LocalAccessInfo.CountryCode,
                                                                       Request.LocalAccessInfo.PartyId,
                                                                       out var locationId,
                                                                       out var existingLocation,
                                                                       out var eVSEUId,
                                                                       out var existingEVSE,
                                                                       out var ocpiResponseBuilder,
                                                                       FailOnMissingEVSE: true) ||
                                             existingLocation is null ||
                                             existingEVSE     is null)
                                        {
                                            return ocpiResponseBuilder!;
                                        }

                                        #endregion

                                        #region Parse EVSE JSON patch

                                        if (!Request.TryParseJObjectRequestBody(out var evsePatch, out ocpiResponseBuilder))
                                            return ocpiResponseBuilder;

                                        #endregion


                                        var patchedEVSE = await CommonAPI.TryPatchEVSE(existingLocation,
                                                                                       existingEVSE,
                                                                                       evsePatch);

                                        //ToDo: Handle update errors!
                                        if (patchedEVSE.IsSuccess)
                                            return new OCPIResponse.Builder(Request) {
                                                           StatusCode           = 1000,
                                                           StatusMessage        = "Hello world!",
                                                           Data                 = patchedEVSE.PatchedData.ToJSON(),
                                                           HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                               HTTPStatusCode             = HTTPStatusCode.OK,
                                                               AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                               AccessControlAllowHeaders  = [ "Authorization" ],
                                                               LastModified               = patchedEVSE.PatchedData.LastUpdated,
                                                               ETag                       = patchedEVSE.PatchedData.ETag
                                                           }
                                                       };

                                        return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = patchedEVSE.ErrorResponse,
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.OK,
                                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                                       }
                                                   };

                                    });

            #endregion

            #region DELETE   ~/locations/{locationId}/{evseId}      [NonStandard]

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.DELETE,
                                    URLPathPrefix + "locations/{locationId}/{evseId}",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestLogger:   DeleteEVSERequest,
                                    OCPIResponseLogger:  DeleteEVSEResponse,
                                    OCPIRequestHandler:  async Request => {

                                        #region Check access token

                                        if (Request.LocalAccessInfo is null ||
                                            Request.LocalAccessInfo.Status != AccessStatus.ALLOWED ||
                                            Request.LocalAccessInfo.Role   != Roles.CPO)
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = "Invalid or blocked access token!",
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                                       }
                                                   };

                                        }

                                        #endregion

                                        #region Check existing Location/EVSE(UId URI parameter)

                                        if (!Request.ParseLocationEVSE(CommonAPI,
                                                                       Request.LocalAccessInfo.CountryCode,
                                                                       Request.LocalAccessInfo.PartyId,
                                                                       out var locationId,
                                                                       out var existingLocation,
                                                                       out var eVSEUId,
                                                                       out var existingEVSE,
                                                                       out var ocpiResponseBuilder) ||
                                             existingEVSE is null)
                                        {
                                            return ocpiResponseBuilder!;
                                        }

                                        #endregion


                                        //CommonAPI.Remove(ExistingLocation, ExistingEVSE);


                                        return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 1000,
                                                       StatusMessage        = "Hello world!",
                                                       Data                 = existingEVSE.ToJSON(Request.EMSPId,
                                                                                                  false,
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
                                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                           AccessControlAllowHeaders  = [ "Authorization" ],
                                                           LastModified               = existingEVSE.LastUpdated,
                                                           ETag                       = existingEVSE.ETag
                                                       }
                                                   };

                                    });

            #endregion

            #endregion

            #region ~/locations/{locationId}/{evseId}/{connectorId}

            #region OPTIONS  ~/locations/{locationId}/{evseId}/{connectorId}      [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPHostname.Any,
                HTTPMethod.OPTIONS,
                URLPathPrefix + "locations/{locationId}/{evseId}/{connectorId}",
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

            #region GET      ~/locations/{locationId}/{evseId}/{connectorId}

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.GET,
                                    URLPathPrefix + "locations/{locationId}/{evseId}/{connectorId}",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestLogger:   GetConnectorRequest,
                                    OCPIResponseLogger:  GetConnectorResponse,
                                    OCPIRequestHandler:  Request => {

                                        #region Check access token

                                        if (Request.LocalAccessInfo is null ||
                                            Request.LocalAccessInfo.Status != AccessStatus.ALLOWED ||
                                            Request.LocalAccessInfo.Role   != Roles.CPO)
                                        {

                                            return Task.FromResult(
                                                new OCPIResponse.Builder(Request) {
                                                    StatusCode           = 2000,
                                                    StatusMessage        = "Invalid or blocked access token!",
                                                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                        AccessControlAllowMethods  = new[] { "OPTIONS", "GET" },
                                                        AccessControlAllowHeaders  = [ "Authorization" ]
                                                    }
                                                });

                                        }

                                        #endregion

                                        #region Check connector

                                        if (!Request.ParseLocationEVSEConnector(CommonAPI,
                                                                                Request.LocalAccessInfo.CountryCode,
                                                                                Request.LocalAccessInfo.PartyId,
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
                                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                       AccessControlAllowHeaders  = [ "Authorization" ],
                                                       LastModified               = connector.LastUpdated,
                                                       ETag                       = connector.ETag
                                                   }
                                            });

                                    });

            #endregion

            #region PUT      ~/locations/{locationId}/{evseId}/{connectorId}

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.PUT,
                                    URLPathPrefix + "locations/{locationId}/{evseId}/{connectorId}",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestLogger:   PutConnectorRequest,
                                    OCPIResponseLogger:  PutConnectorResponse,
                                    OCPIRequestHandler:  async Request => {

                                        #region Check access token

                                        if (Request.LocalAccessInfo is null ||
                                            Request.LocalAccessInfo.Status != AccessStatus.ALLOWED ||
                                            Request.LocalAccessInfo.Role   != Roles.CPO)
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = "Invalid or blocked access token!",
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                           AccessControlAllowMethods  = new[] { "OPTIONS", "GET" },
                                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                                       }
                                                   };

                                        }

                                        #endregion

                                        #region Check connector

                                        if (!Request.ParseLocationEVSEConnector(CommonAPI,
                                                                                Request.LocalAccessInfo.CountryCode,
                                                                                Request.LocalAccessInfo.PartyId,
                                                                                out var locationId,
                                                                                out var existingLocation,
                                                                                out var eVSEUId,
                                                                                out var existingEVSE,
                                                                                out var connectorId,
                                                                                out var existingConnector,
                                                                                out var ocpiResponseBuilder,
                                                                                FailOnMissingConnector: false) ||
                                             existingLocation is null ||
                                             existingEVSE     is null)
                                        {
                                            return ocpiResponseBuilder!;
                                        }

                                        #endregion

                                        #region Parse new or updated connector JSON

                                        if (!Request.TryParseJObjectRequestBody(out var connectorJSON, out ocpiResponseBuilder))
                                            return ocpiResponseBuilder;

                                        if (!Connector.TryParse(connectorJSON,
                                                                out var newOrUpdatedConnector,
                                                                out var errorResponse,
                                                                connectorId) ||
                                             newOrUpdatedConnector is null)
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2001,
                                                       StatusMessage        = "Could not parse the given connector JSON: " + errorResponse,
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                                       }
                                                   };

                                        }

                                        #endregion


                                        var addOrUpdateResult = await CommonAPI.AddOrUpdateConnector(existingLocation,
                                                                                                     existingEVSE,
                                                                                                     newOrUpdatedConnector,
                                                                                                     AllowDowngrades ?? Request.QueryString.GetBoolean("forceDowngrade"));


                                        if (addOrUpdateResult.IsSuccess)
                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 1000,
                                                       StatusMessage        = "Hello world!",
                                                       Data                 = addOrUpdateResult.Data?.ToJSON(),
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = addOrUpdateResult.WasCreated == true
                                                                                            ? HTTPStatusCode.Created
                                                                                            : HTTPStatusCode.OK,
                                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                           AccessControlAllowHeaders  = [ "Authorization" ],
                                                           LastModified               = newOrUpdatedConnector.LastUpdated,
                                                           ETag                       = newOrUpdatedConnector.ETag
                                                       }
                                                   };

                                        return new OCPIResponse.Builder(Request) {
                                                   StatusCode           = 2000,
                                                   StatusMessage        = addOrUpdateResult.ErrorResponse,
                                                   Data                 = newOrUpdatedConnector.ToJSON(),
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                       AccessControlAllowHeaders  = [ "Authorization" ],
                                                       LastModified               = newOrUpdatedConnector.LastUpdated,
                                                       ETag                       = newOrUpdatedConnector.ETag
                                                   }
                                               };

                                    });

            #endregion

            #region PATCH    ~/locations/{locationId}/{evseId}/{connectorId}

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.PATCH,
                                    URLPathPrefix + "locations/{locationId}/{evseId}/{connectorId}",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestLogger:   PatchConnectorRequest,
                                    OCPIResponseLogger:  PatchConnectorResponse,
                                    OCPIRequestHandler:  async Request => {

                                        #region Check access token

                                        if (Request.LocalAccessInfo is null ||
                                            Request.LocalAccessInfo.Status != AccessStatus.ALLOWED ||
                                            Request.LocalAccessInfo.Role   != Roles.CPO)
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = "Invalid or blocked access token!",
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                           AccessControlAllowMethods  = new[] { "OPTIONS", "GET" },
                                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                                       }
                                                   };

                                        }

                                        #endregion

                                        #region Check connector

                                        if (!Request.ParseLocationEVSEConnector(CommonAPI,
                                                                                Request.LocalAccessInfo.CountryCode,
                                                                                Request.LocalAccessInfo.PartyId,
                                                                                out var locationId,
                                                                                out var existingLocation,
                                                                                out var eVSEUId,
                                                                                out var existingEVSE,
                                                                                out var connectorId,
                                                                                out var existingConnector,
                                                                                out var ocpiResponseBuilder,
                                                                                FailOnMissingConnector: true) ||
                                             existingLocation  is null ||
                                             existingEVSE      is null ||
                                             existingConnector is null)
                                        {
                                            return ocpiResponseBuilder!;
                                        }

                                        #endregion

                                        #region Parse connector JSON patch

                                        if (!Request.TryParseJObjectRequestBody(out var connectorPatch, out ocpiResponseBuilder))
                                            return ocpiResponseBuilder;

                                        #endregion


                                        var patchedConnector = await CommonAPI.TryPatchConnector(existingLocation,
                                                                                                 existingEVSE,
                                                                                                 existingConnector,
                                                                                                 connectorPatch);

                                        //ToDo: Handle update errors!
                                        if (patchedConnector.IsSuccess)
                                            return new OCPIResponse.Builder(Request) {
                                                           StatusCode           = 1000,
                                                           StatusMessage        = "Hello world!",
                                                           Data                 = patchedConnector.PatchedData.ToJSON(),
                                                           HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                               HTTPStatusCode             = HTTPStatusCode.OK,
                                                               AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                               AccessControlAllowHeaders  = [ "Authorization" ],
                                                               LastModified               = patchedConnector.PatchedData.LastUpdated,
                                                               ETag                       = patchedConnector.PatchedData.ETag
                                                           }
                                                       };

                                        return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = patchedConnector.ErrorResponse,
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.OK,
                                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                                       }
                                                   };

                                    });

            #endregion

            #region DELETE   ~/locations/{locationId}/{evseId}/{connectorId}      [NonStandard]

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.DELETE,
                                    URLPathPrefix + "locations/{locationId}/{evseId}/{connectorId}",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestLogger:   DeleteConnectorRequest,
                                    OCPIResponseLogger:  DeleteConnectorResponse,
                                    OCPIRequestHandler:  async Request => {

                                        #region Check access token

                                        if (Request.LocalAccessInfo is null ||
                                            Request.LocalAccessInfo.Status != AccessStatus.ALLOWED ||
                                            Request.LocalAccessInfo.Role   != Roles.CPO)
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = "Invalid or blocked access token!",
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                                       }
                                                   };

                                        }

                                        #endregion

                                        #region Check existing Location/EVSE/Connector(UId URI parameter)

                                        if (!Request.ParseLocationEVSEConnector(CommonAPI,
                                                                                Request.LocalAccessInfo.CountryCode,
                                                                                Request.LocalAccessInfo.PartyId,
                                                                                out var locationId,
                                                                                out var existingLocation,
                                                                                out var eVSEUId,
                                                                                out var existingEVSE,
                                                                                out var connectorId,
                                                                                out var existingConnector,
                                                                                out var ocpiResponseBuilder) ||
                                             existingConnector is null)
                                        {
                                            return ocpiResponseBuilder!;
                                        }

                                        #endregion


                                        //CommonAPI.Remove(ExistingLocation, ExistingEVSE);


                                        return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 1000,
                                                       StatusMessage        = "Hello world!",
                                                       Data                 = existingConnector.ToJSON(Request.EMSPId,
                                                                                                       CustomConnectorSerializer),
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
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


            #region ~/locations/{locationId}/{evseId}/status  [NonStandard]

            #region OPTIONS  ~/locations/{locationId}/{evseId}/status     [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPHostname.Any,
                HTTPMethod.OPTIONS,
                URLPathPrefix + "locations/{locationId}/{evseId}/status",
                OCPIRequestHandler: request =>

                    Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               HTTPResponseBuilder = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   Allow                      = [ HTTPMethod.OPTIONS, HTTPMethod.POST ],
                                   AcceptPatch                = [ HTTPContentType.Application.JSONMergePatch_UTF8 ],
                                   AccessControlAllowHeaders  = [ "Authorization" ],
                                   AccessControlAllowMethods  = [ "OPTIONS", "POST" ]
                               }
                        })

            );

            #endregion

            #region POST     ~/locations/{locationId}/{evseId}/status     [NonStandard]

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.POST,
                                    URLPathPrefix + "locations/{locationId}/{evseId}/status",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestLogger:   PostEVSEStatusRequest,
                                    OCPIResponseLogger:  PostEVSEStatusResponse,
                                    OCPIRequestHandler:  async Request => {

                                        #region Check access token

                                        if (Request.LocalAccessInfo is null ||
                                            Request.LocalAccessInfo.Status != AccessStatus.ALLOWED ||
                                            Request.LocalAccessInfo.Role   != Roles.CPO)
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = "Invalid or blocked access token!",
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                           AccessControlAllowMethods  = new[] { "OPTIONS", "GET" },
                                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                                       }
                                                   };

                                        }

                                        #endregion

                                        #region Check existing EVSE

                                        if (!Request.ParseLocationEVSE(CommonAPI,
                                                                       Request.LocalAccessInfo.CountryCode,
                                                                       Request.LocalAccessInfo.PartyId,
                                                                       out var locationId,
                                                                       out var existingLocation,
                                                                       out var eVSEUId,
                                                                       out var existingEVSE,
                                                                       out var ocpiResponseBuilder,
                                                                       FailOnMissingEVSE: false))
                                        {
                                            return ocpiResponseBuilder!;
                                        }

                                        #endregion

                                        #region Parse EVSE status JSON

                                        if (!Request.TryParseJObjectRequestBody(out var evseStatusJSON, out ocpiResponseBuilder))
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


                                        return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 1000,
                                                       StatusMessage        = "Hello world!",
                                                       //Data                 = newOrUpdatedEVSE.ToJSON(),
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.OK,
                                                           AccessControlAllowMethods  = new[] { "OPTIONS", "POST" },
                                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                                       }
                                                   };

                                    });

            #endregion

            #endregion



            #region ~/tariffs                                 [NonStandard]

            #region OPTIONS  ~/tariffs            [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPHostname.Any,
                HTTPMethod.OPTIONS,
                URLPathPrefix + "tariffs",
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

            #region GET      ~/tariffs            [NonStandard]

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.GET,
                                    URLPathPrefix + "tariffs",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestLogger:   GetTariffsRequest,
                                    OCPIResponseLogger:  GetTariffsResponse,
                                    OCPIRequestHandler:  Request => {

                                        #region Check access token

                                        if (Request.LocalAccessInfo is null ||
                                            Request.LocalAccessInfo.Status != AccessStatus.ALLOWED ||
                                            Request.LocalAccessInfo.Role   != Roles.CPO)
                                        {

                                            return Task.FromResult(
                                                new OCPIResponse.Builder(Request) {
                                                    StatusCode           = 2000,
                                                    StatusMessage        = "Invalid or blocked access token!",
                                                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                        AccessControlAllowMethods  = new[] { "OPTIONS", "GET" },
                                                        AccessControlAllowHeaders  = [ "Authorization" ]
                                                    }
                                                });

                                        }

                                        #endregion


                                        var withExtensions   = Request.QueryString.GetBoolean("withExtensions") ?? false;

                                        var filters          = Request.GetDateAndPaginationFilters();

                                        var allTariffs       = CommonAPI.GetTariffs(Request.LocalAccessInfo.CountryCode,
                                                                                    Request.LocalAccessInfo.PartyId).
                                                                         ToArray();

                                        var filteredTariffs  = allTariffs.Where(tariff => !filters.From.HasValue || tariff.LastUpdated >  filters.From.Value).
                                                                          Where(tariff => !filters.To.  HasValue || tariff.LastUpdated <= filters.To.  Value).
                                                                          ToArray();


                                        return Task.FromResult(
                                            new OCPIResponse.Builder(Request) {
                                                   StatusCode           = 1000,
                                                   StatusMessage        = "Hello world!",
                                                   Data                 = new JArray(
                                                                              filteredTariffs.
                                                                                  SkipTakeFilter(filters.Offset,
                                                                                                 filters.Limit).
                                                                                  Select(tariff => tariff.ToJSON(false,
                                                                                                                 withExtensions,
                                                                                                                 CustomTariffSerializer,
                                                                                                                 CustomDisplayTextSerializer,
                                                                                                                 CustomTariffElementSerializer,
                                                                                                                 CustomPriceComponentSerializer,
                                                                                                                 CustomTariffRestrictionsSerializer,
                                                                                                                 CustomEnergyMixSerializer,
                                                                                                                 CustomEnergySourceSerializer,
                                                                                                                 CustomEnvironmentalImpactSerializer))
                                                                          ),
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "DELETE" ],
                                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                                       //LastModified               = ?
                                                   }.
                                                   Set("X-Total-Count", allTariffs.Length)
                                                   // X-Limit               The maximum number of objects that the server WILL return.
                                                   // Link                  Link to the 'next' page should be provided when this is NOT the last page.
                                            });

                                    });

            #endregion

            #region DELETE   ~/tariffs            [NonStandard]

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.DELETE,
                                    URLPathPrefix + "tariffs",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestLogger:   DeleteTariffsRequest,
                                    OCPIResponseLogger:  DeleteTariffsResponse,
                                    OCPIRequestHandler:  async Request => {

                                        #region Check access token

                                        if (Request.LocalAccessInfo?.Status != AccessStatus.ALLOWED ||
                                            Request.LocalAccessInfo?.Role   != Roles.CPO)
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = "Invalid or blocked access token!",
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                           AccessControlAllowMethods  = new[] { "OPTIONS", "GET" },
                                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                                       }
                                                   };

                                        }

                                        #endregion


                                        CommonAPI.RemoveAllTariffs();


                                        return new OCPIResponse.Builder(Request) {
                                                   StatusCode           = 1000,
                                                   StatusMessage        = "Hello world!",
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "DELETE" ],
                                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                                   }
                                               };

                                    });

            #endregion

            #endregion

            #region ~/tariffs/{tariffId}

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
                                   Allow                      = [ HTTPMethod.OPTIONS, HTTPMethod.GET, HTTPMethod.PUT, HTTPMethod.PATCH, HTTPMethod.DELETE ],
                                   AcceptPatch                = [ HTTPContentType.Application.JSONMergePatch_UTF8 ],
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
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
                                    OCPIRequestLogger:   GetTariffRequest,
                                    OCPIResponseLogger:  GetTariffResponse,
                                    OCPIRequestHandler:  Request => {

                                        #region Check access token

                                        if (Request.LocalAccessInfo is null ||
                                            Request.LocalAccessInfo.Status != AccessStatus.ALLOWED ||
                                            Request.LocalAccessInfo.Role   != Roles.CPO)
                                        {

                                            return Task.FromResult(
                                                new OCPIResponse.Builder(Request) {
                                                    StatusCode           = 2000,
                                                    StatusMessage        = "Invalid or blocked access token!",
                                                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                        AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                        AccessControlAllowHeaders  = [ "Authorization" ]
                                                    }
                                                });

                                        }

                                        #endregion

                                        #region Check tariff

                                        if (!Request.ParseTariff(CommonAPI,
                                                                 Request.LocalAccessInfo.CountryCode,
                                                                 Request.LocalAccessInfo.PartyId,
                                                                 out var tariffId,
                                                                 out var tariff,
                                                                 out var ocpiResponseBuilder) ||
                                             tariff is null)
                                        {
                                            return Task.FromResult(ocpiResponseBuilder!);
                                        }

                                        #endregion

                                        var withExtensions = Request.QueryString.GetBoolean("withExtensions") ?? false;


                                        return Task.FromResult(
                                            new OCPIResponse.Builder(Request) {
                                                   StatusCode           = 1000,
                                                   StatusMessage        = "Hello world!",
                                                   Data                 = tariff.ToJSON(false,
                                                                                        withExtensions,
                                                                                        CustomTariffSerializer,
                                                                                        CustomDisplayTextSerializer,
                                                                                        CustomTariffElementSerializer,
                                                                                        CustomPriceComponentSerializer,
                                                                                        CustomTariffRestrictionsSerializer,
                                                                                        CustomEnergyMixSerializer,
                                                                                        CustomEnergySourceSerializer,
                                                                                        CustomEnvironmentalImpactSerializer),
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                       AccessControlAllowHeaders  = [ "Authorization" ],
                                                       LastModified               = tariff.LastUpdated,
                                                       ETag                       = tariff.ETag
                                                   }
                                            });

                                    });

            #endregion

            #region PUT      ~/tariffs/{tariffId}

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.PUT,
                                    URLPathPrefix + "tariffs/{tariffId}",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestLogger:   PutTariffRequest,
                                    OCPIResponseLogger:  PutTariffResponse,
                                    OCPIRequestHandler:  async Request => {

                                        #region Check access token

                                        if (Request.LocalAccessInfo is null ||
                                            Request.LocalAccessInfo.Status != AccessStatus.ALLOWED ||
                                            Request.LocalAccessInfo.Role   != Roles.CPO)
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = "Invalid or blocked access token!",
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                                       }
                                                   };

                                        }

                                        #endregion

                                        #region Check existing tariff

                                        if (!Request.ParseTariff(CommonAPI,
                                                                 Request.LocalAccessInfo.CountryCode,
                                                                 Request.LocalAccessInfo.PartyId,
                                                                 out var tariffId,
                                                                 out var existingTariff,
                                                                 out var ocpiResponseBuilder,
                                                                 FailOnMissingTariff: false))
                                        {
                                            return ocpiResponseBuilder!;
                                        }

                                        #endregion

                                        #region Parse new or updated tariff

                                        if (!Request.TryParseJObjectRequestBody(out var tariffJSON, out ocpiResponseBuilder))
                                            return ocpiResponseBuilder;

                                        if (!Tariff.TryParse(tariffJSON,
                                                             out var newOrUpdatedTariff,
                                                             out var errorResponse,
                                                             Request.LocalAccessInfo.CountryCode,
                                                             Request.LocalAccessInfo.PartyId,
                                                             tariffId) ||
                                             newOrUpdatedTariff is null)
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2001,
                                                       StatusMessage        = "Could not parse the given tariff JSON: " + errorResponse,
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                           AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "PUT", "DELETE" },
                                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                                       }
                                                   };

                                        }

                                        #endregion


                                        var addOrUpdateResult = await CommonAPI.AddOrUpdateTariff(newOrUpdatedTariff,
                                                                                                  AllowDowngrades ?? Request.QueryString.GetBoolean("forceDowngrade"));


                                        if (addOrUpdateResult.IsSuccess)
                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 1000,
                                                       StatusMessage        = "Hello world!",
                                                       Data                 = addOrUpdateResult.Data?.ToJSON(),
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = addOrUpdateResult.WasCreated == true
                                                                                            ? HTTPStatusCode.Created
                                                                                            : HTTPStatusCode.OK,
                                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                           AccessControlAllowHeaders  = [ "Authorization" ],
                                                           LastModified               = addOrUpdateResult.Data?.LastUpdated,
                                                           ETag                       = addOrUpdateResult.Data.ETag
                                                       }
                                                   };

                                        return new OCPIResponse.Builder(Request) {
                                                   StatusCode           = 2000,
                                                   StatusMessage        = addOrUpdateResult.ErrorResponse,
                                                   Data                 = newOrUpdatedTariff.ToJSON(false,
                                                                                                    false,
                                                                                                    CustomTariffSerializer,
                                                                                                    CustomDisplayTextSerializer,
                                                                                                    CustomTariffElementSerializer,
                                                                                                    CustomPriceComponentSerializer,
                                                                                                    CustomTariffRestrictionsSerializer,
                                                                                                    CustomEnergyMixSerializer,
                                                                                                    CustomEnergySourceSerializer,
                                                                                                    CustomEnvironmentalImpactSerializer),
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                                   }
                                               };

                                    });

            #endregion

            #region PATCH    ~/tariffs/{tariffId}      [NonStandard]

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.PATCH,
                                    URLPathPrefix + "tariffs/{tariffId}",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestLogger:   PatchTariffRequest,
                                    OCPIResponseLogger:  PatchTariffResponse,
                                    OCPIRequestHandler:  async Request => {

                                        #region Check access token

                                        if (Request.LocalAccessInfo is null ||
                                            Request.LocalAccessInfo.Status != AccessStatus.ALLOWED ||
                                            Request.LocalAccessInfo.Role   != Roles.CPO)
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = "Invalid or blocked access token!",
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                                       }
                                                   };

                                        }

                                        #endregion

                                        #region Check tariff

                                        if (!Request.ParseTariff(CommonAPI,
                                                                 Request.LocalAccessInfo.CountryCode,
                                                                 Request.LocalAccessInfo.PartyId,
                                                                 out var tariffId,
                                                                 out var existingTariff,
                                                                 out var ocpiResponseBuilder,
                                                                 FailOnMissingTariff: true) ||
                                             existingTariff is null)
                                        {
                                            return ocpiResponseBuilder!;
                                        }

                                        #endregion

                                        #region Parse and apply Tariff JSON patch

                                        if (!Request.TryParseJObjectRequestBody(out var tariffPatch, out ocpiResponseBuilder))
                                            return ocpiResponseBuilder;

                                        #endregion


                                        // Validation-Checks for PATCHes
                                        // (E-Tag, Timestamp, ...)

                                        var patchedTariff = await CommonAPI.TryPatchTariff(
                                                                      existingTariff,
                                                                      tariffPatch
                                                                  );

                                        //ToDo: Handle update errors!
                                        if (patchedTariff.IsSuccessAndDataNotNull(out var patchedData))
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                           StatusCode           = 1000,
                                                           StatusMessage        = "Hello world!",
                                                           Data                 = patchedData.ToJSON(),
                                                           HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                               HTTPStatusCode             = HTTPStatusCode.OK,
                                                               AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                               AccessControlAllowHeaders  = [ "Authorization" ],
                                                               LastModified               = patchedData.LastUpdated,
                                                               ETag                       = patchedData.ETag
                                                           }
                                                       };

                                        }

                                        else
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                           StatusCode           = 2000,
                                                           StatusMessage        = patchedTariff.ErrorResponse,
                                                           HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                               HTTPStatusCode             = HTTPStatusCode.OK,
                                                               AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                               AccessControlAllowHeaders  = [ "Authorization" ]
                                                           }
                                                       };

                                        }

                                    });

            #endregion

            #region DELETE   ~/tariffs/{tariffId}

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.DELETE,
                                    URLPathPrefix + "tariffs/{tariffId}",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestLogger:   DeleteTariffRequest,
                                    OCPIResponseLogger:  DeleteTariffResponse,
                                    OCPIRequestHandler:  async Request => {

                                        #region Check access token

                                        if (Request.LocalAccessInfo is null ||
                                            Request.LocalAccessInfo.Status != AccessStatus.ALLOWED ||
                                            Request.LocalAccessInfo.Role   != Roles.CPO)
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = "Invalid or blocked access token!",
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                                       }
                                                   };

                                        }

                                        #endregion

                                        #region Check existing tariff

                                        if (!Request.ParseTariff(CommonAPI,
                                                                 Request.LocalAccessInfo.CountryCode,
                                                                 Request.LocalAccessInfo.PartyId,
                                                                 out var tariffId,
                                                                 out var existingTariff,
                                                                 out var ocpiResponseBuilder,
                                                                 FailOnMissingTariff: true) ||
                                             existingTariff is null)
                                        {
                                            return ocpiResponseBuilder!;
                                        }

                                        #endregion


                                        var result = await CommonAPI.RemoveTariff(existingTariff);

                                        return result.IsSuccess

                                                   ? new OCPIResponse.Builder(Request) {
                                                         StatusCode           = 1000,
                                                         StatusMessage        = "Hello world!",
                                                         Data                 = existingTariff.ToJSON(true,
                                                                                                      true,
                                                                                                      CustomTariffSerializer,
                                                                                                      CustomDisplayTextSerializer,
                                                                                                      CustomTariffElementSerializer,
                                                                                                      CustomPriceComponentSerializer,
                                                                                                      CustomTariffRestrictionsSerializer,
                                                                                                      CustomEnergyMixSerializer,
                                                                                                      CustomEnergySourceSerializer,
                                                                                                      CustomEnvironmentalImpactSerializer),
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                             HTTPStatusCode             = HTTPStatusCode.OK,
                                                             AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                             AccessControlAllowHeaders  = [ "Authorization" ],
                                                             LastModified               = existingTariff.LastUpdated,
                                                             ETag                       = existingTariff.ETag
                                                         }
                                                     }

                                                   : new OCPIResponse.Builder(Request) {
                                                         StatusCode           = 2000,
                                                         StatusMessage        = "Failed!",
                                                         Data                 = existingTariff.ToJSON(true,
                                                                                                      true,
                                                                                                      CustomTariffSerializer,
                                                                                                      CustomDisplayTextSerializer,
                                                                                                      CustomTariffElementSerializer,
                                                                                                      CustomPriceComponentSerializer,
                                                                                                      CustomTariffRestrictionsSerializer,
                                                                                                      CustomEnergyMixSerializer,
                                                                                                      CustomEnergySourceSerializer,
                                                                                                      CustomEnvironmentalImpactSerializer),
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                             HTTPStatusCode             = HTTPStatusCode.OK,
                                                             AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                             AccessControlAllowHeaders  = [ "Authorization" ],
                                                             LastModified               = existingTariff.LastUpdated,
                                                             ETag                       = existingTariff.ETag
                                                         }
                                                     };

                                    });

            #endregion

            #endregion



            #region ~/sessions                                [NonStandard]

            #region OPTIONS  ~/sessions                [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPHostname.Any,
                HTTPMethod.OPTIONS,
                URLPathPrefix + "sessions",
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

            #region GET      ~/sessions                [NonStandard]

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.GET,
                                    URLPathPrefix + "sessions",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestLogger:   GetSessionsRequest,
                                    OCPIResponseLogger:  GetSessionsResponse,
                                    OCPIRequestHandler:  Request => {

                                        #region Check access token

                                        if (Request.LocalAccessInfo is null ||
                                            Request.LocalAccessInfo.Status != AccessStatus.ALLOWED ||
                                            Request.LocalAccessInfo.Role   != Roles.CPO)
                                        {

                                            return Task.FromResult(
                                                new OCPIResponse.Builder(Request) {
                                                    StatusCode           = 2000,
                                                    StatusMessage        = "Invalid or blocked access token!",
                                                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                        AccessControlAllowMethods  = [ "OPTIONS", "GET", "DELETE" ],
                                                        AccessControlAllowHeaders  = [ "Authorization" ]
                                                    }
                                                });

                                        }

                                        #endregion


                                        var filters           = Request.GetDateAndPaginationFilters();

                                        var allSessions       = CommonAPI.GetSessions(Request.LocalAccessInfo.CountryCode,
                                                                                      Request.LocalAccessInfo.PartyId).
                                                                          ToArray();

                                        var filteredSessions  = allSessions.Where(session => !filters.From.HasValue || session.LastUpdated >  filters.From.Value).
                                                                            Where(session => !filters.To.  HasValue || session.LastUpdated <= filters.To.  Value).
                                                                            ToArray();


                                        return Task.FromResult(
                                            new OCPIResponse.Builder(Request) {
                                                   StatusCode           = 1000,
                                                   StatusMessage        = "Hello world!",
                                                   Data                 = new JArray(
                                                                              filteredSessions.
                                                                                  SkipTakeFilter(filters.Offset,
                                                                                                 filters.Limit).
                                                                                  Select(session => session.ToJSON(IncludeSessionOwnerInformation,
                                                                                                                   false,
                                                                                                                   Request.EMSPId,
                                                                                                                   CustomSessionSerializer,
                                                                                                                   CustomLocationSerializer,
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
                                                                                                                   CustomEnvironmentalImpactSerializer,
                                                                                                                   CustomChargingPeriodSerializer,
                                                                                                                   CustomCDRDimensionSerializer))
                                                                          ),
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
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

            #region DELETE   ~/sessions                [NonStandard]

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.DELETE,
                                    URLPathPrefix + "sessions",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestLogger:   DeleteSessionsRequest,
                                    OCPIResponseLogger:  DeleteSessionsResponse,
                                    OCPIRequestHandler:  async Request => {

                                        #region Check access token

                                        if (Request.LocalAccessInfo is null ||
                                            Request.LocalAccessInfo.Status != AccessStatus.ALLOWED ||
                                            Request.LocalAccessInfo.Role   != Roles.CPO)
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = "Invalid or blocked access token!",
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "DELETE" ],
                                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                                       }
                                                   };

                                        }

                                    #endregion


                                        CommonAPI.RemoveAllSessions(session => Request.LocalAccessInfo.CountryCode == session.CountryCode &&
                                                                               Request.LocalAccessInfo.PartyId     == session.PartyId);


                                        return new OCPIResponse.Builder(Request) {
                                                   StatusCode           = 1000,
                                                   StatusMessage        = "Hello world!",
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "DELETE" ],
                                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                                   }
                                               };

                                    });

            #endregion

            #endregion

            #region ~/sessions/{sessionId}

            #region OPTIONS  ~/sessions/{sessionId}    [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPHostname.Any,
                HTTPMethod.OPTIONS,
                URLPathPrefix + "sessions/{sessionId}",
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

            #region GET      ~/sessions/{sessionId}

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.GET,
                                    URLPathPrefix + "sessions/{sessionId}",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestLogger:   GetSessionRequest,
                                    OCPIResponseLogger:  GetSessionResponse,
                                    OCPIRequestHandler:  Request => {

                                        #region Check access token

                                        if (Request.LocalAccessInfo is null ||
                                            Request.LocalAccessInfo.Status != AccessStatus.ALLOWED ||
                                            Request.LocalAccessInfo.Role   != Roles.CPO)
                                        {

                                            return Task.FromResult(
                                                new OCPIResponse.Builder(Request) {
                                                    StatusCode           = 2000,
                                                    StatusMessage        = "Invalid or blocked access token!",
                                                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                        AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                        AccessControlAllowHeaders  = [ "Authorization" ]
                                                    }
                                                });

                                        }

                                        #endregion

                                        #region Check session

                                        if (!Request.ParseSession(CommonAPI,
                                                                  Request.LocalAccessInfo.CountryCode,
                                                                  Request.LocalAccessInfo.PartyId,
                                                                  out var sessionId,
                                                                  out var session,
                                                                  out var ocpiResponseBuilder,
                                                                  FailOnMissingSession: true) ||
                                             session is null)
                                        {
                                            return Task.FromResult(ocpiResponseBuilder!);
                                        }

                                        #endregion


                                        return Task.FromResult(
                                            new OCPIResponse.Builder(Request) {
                                                StatusCode           = 1000,
                                                StatusMessage        = "Hello world!",
                                                Data                 = session.ToJSON(false,
                                                                                      false,
                                                                                      Request.EMSPId,
                                                                                      CustomSessionSerializer,
                                                                                      CustomLocationSerializer,
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
                                                                                      CustomEnvironmentalImpactSerializer,
                                                                                      CustomChargingPeriodSerializer,
                                                                                      CustomCDRDimensionSerializer),
                                                HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                    HTTPStatusCode             = HTTPStatusCode.OK,
                                                    AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                    AccessControlAllowHeaders  = [ "Authorization" ],
                                                    LastModified               = session.LastUpdated,
                                                    ETag                       = session.ETag
                                                }
                                            });

                                    });

            #endregion

            #region PUT      ~/sessions/{sessionId}

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.PUT,
                                    URLPathPrefix + "sessions/{sessionId}",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestLogger:   PutSessionRequest,
                                    OCPIResponseLogger:  PutSessionResponse,
                                    OCPIRequestHandler:  async Request => {

                                        #region Check access token

                                        if (Request.LocalAccessInfo is null ||
                                            Request.LocalAccessInfo.Status != AccessStatus.ALLOWED ||
                                            Request.LocalAccessInfo.Role   != Roles.CPO)
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = "Invalid or blocked access token!",
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                                       }
                                                   };

                                        }

                                        #endregion

                                        #region Check existing session

                                        if (!Request.ParseSession(CommonAPI,
                                                                  Request.LocalAccessInfo.CountryCode,
                                                                  Request.LocalAccessInfo.PartyId,
                                                                  out var sessionId,
                                                                  out var existingSession,
                                                                  out var ocpiResponseBuilder,
                                                                  FailOnMissingSession: false))
                                        {
                                            return ocpiResponseBuilder!;
                                        }

                                        #endregion

                                        #region Parse new or updated session

                                        if (!Request.TryParseJObjectRequestBody(out var sessionJSON, out ocpiResponseBuilder))
                                            return ocpiResponseBuilder;

                                        if (!Session.TryParse(sessionJSON,
                                                              out var newOrUpdatedSession,
                                                              out var errorResponse,
                                                              Request.LocalAccessInfo.CountryCode,
                                                              Request.LocalAccessInfo.PartyId,
                                                              sessionId) ||
                                             newOrUpdatedSession is null)
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2001,
                                                       StatusMessage        = "Could not parse the given session JSON: " + errorResponse,
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                                       }
                                                   };

                                        }

                                        #endregion


                                        var addOrUpdateResult = await CommonAPI.AddOrUpdateSession(newOrUpdatedSession,
                                                                                                   AllowDowngrades ?? Request.QueryString.GetBoolean("forceDowngrade"));


                                        if (addOrUpdateResult.IsSuccess)
                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 1000,
                                                       StatusMessage        = "Hello world!",
                                                       Data                 = addOrUpdateResult.Data.ToJSON(false,
                                                                                                            false,
                                                                                                            Request.EMSPId,
                                                                                                            CustomSessionSerializer,
                                                                                                            CustomLocationSerializer,
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
                                                                                                            CustomEnvironmentalImpactSerializer,
                                                                                                            CustomChargingPeriodSerializer,
                                                                                                            CustomCDRDimensionSerializer),
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = addOrUpdateResult.WasCreated == true
                                                                                            ? HTTPStatusCode.Created
                                                                                            : HTTPStatusCode.OK,
                                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                           AccessControlAllowHeaders  = [ "Authorization" ],
                                                           LastModified               = addOrUpdateResult.Data.LastUpdated,
                                                           ETag                       = addOrUpdateResult.Data.ETag
                                                       }
                                                   };

                                        return new OCPIResponse.Builder(Request) {
                                                   StatusCode           = 2000,
                                                   StatusMessage        = addOrUpdateResult.ErrorResponse,
                                                   Data                 = newOrUpdatedSession.ToJSON(false,
                                                                                                     false,
                                                                                                     Request.EMSPId,
                                                                                                     CustomSessionSerializer,
                                                                                                     CustomLocationSerializer,
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
                                                                                                     CustomEnvironmentalImpactSerializer,
                                                                                                     CustomChargingPeriodSerializer,
                                                                                                     CustomCDRDimensionSerializer),
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                       AccessControlAllowHeaders  = [ "Authorization" ],
                                                       LastModified               = addOrUpdateResult.Data.LastUpdated,
                                                       ETag                       = addOrUpdateResult.Data.ETag
                                                   }
                                               };

                                    });

            #endregion

            #region PATCH    ~/sessions/{sessionId}

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.PATCH,
                                    URLPathPrefix + "sessions/{sessionId}",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestLogger:   PatchSessionRequest,
                                    OCPIResponseLogger:  PatchSessionResponse,
                                    OCPIRequestHandler:  async Request => {

                                        #region Check access token

                                        if (Request.LocalAccessInfo is null ||
                                            Request.LocalAccessInfo.Status != AccessStatus.ALLOWED ||
                                            Request.LocalAccessInfo.Role   != Roles.CPO)
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = "Invalid or blocked access token!",
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                                       }
                                                   };

                                        }

                                        #endregion

                                        #region Check session

                                        if (!Request.ParseSession(CommonAPI,
                                                                  Request.LocalAccessInfo.CountryCode,
                                                                  Request.LocalAccessInfo.PartyId, 
                                                                  out var sessionId,
                                                                  out var existingSession,
                                                                  out var ocpiResponseBuilder,
                                                                  FailOnMissingSession: true) ||
                                             existingSession is null)
                                        {
                                            return ocpiResponseBuilder!;
                                        }

                                        #endregion

                                        #region Parse and apply Session JSON patch

                                        if (!Request.TryParseJObjectRequestBody(out var sessionPatch, out ocpiResponseBuilder))
                                            return ocpiResponseBuilder;

                                        #endregion


                                        var patchedSession = await CommonAPI.TryPatchSession(existingSession,
                                                                                             sessionPatch);


                                        //ToDo: Handle update errors!
                                        if (patchedSession.IsSuccess)
                                            return new OCPIResponse.Builder(Request) {
                                                           StatusCode           = 1000,
                                                           StatusMessage        = "Hello world!",
                                                           Data                 = patchedSession.PatchedData.ToJSON(false,
                                                                                                                    false,
                                                                                                                    Request.EMSPId,
                                                                                                                    CustomSessionSerializer,
                                                                                                                    CustomLocationSerializer,
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
                                                                                                                    CustomEnvironmentalImpactSerializer,
                                                                                                                    CustomChargingPeriodSerializer,
                                                                                                                    CustomCDRDimensionSerializer),
                                                           HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                               HTTPStatusCode             = HTTPStatusCode.OK,
                                                               AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                               AccessControlAllowHeaders  = [ "Authorization" ],
                                                               LastModified               = patchedSession.PatchedData.LastUpdated,
                                                               ETag                       = patchedSession.PatchedData.ETag
                                                           }
                                                       };

                                        return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = patchedSession.ErrorResponse,
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.OK,
                                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                                       }
                                                   };

                                    });

            #endregion

            #region DELETE   ~/sessions/{sessionId}    [NonStandard]

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.DELETE,
                                    URLPathPrefix + "sessions/{sessionId}",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestLogger:   DeleteSessionRequest,
                                    OCPIResponseLogger:  DeleteSessionResponse,
                                    OCPIRequestHandler:  async Request => {

                                        #region Check access token

                                        if (Request.LocalAccessInfo is null ||
                                            Request.LocalAccessInfo.Status != AccessStatus.ALLOWED ||
                                            Request.LocalAccessInfo.Role   != Roles.CPO)
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = "Invalid or blocked access token!",
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                                       }
                                                   };

                                        }

                                        #endregion

                                        #region Check existing session

                                        if (!Request.ParseSession(CommonAPI,
                                                                  Request.LocalAccessInfo.CountryCode,
                                                                  Request.LocalAccessInfo.PartyId, 
                                                                  out var sessionId,
                                                                  out var existingSession,
                                                                  out var ocpiResponseBuilder,
                                                                  FailOnMissingSession: true) ||
                                             existingSession is null)
                                        {
                                            return ocpiResponseBuilder!;
                                        }

                                        #endregion


                                        await CommonAPI.RemoveSession(existingSession);


                                        return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 1000,
                                                       StatusMessage        = "Hello world!",
                                                       Data                 = existingSession.ToJSON(false,
                                                                                                     false,
                                                                                                     Request.EMSPId,
                                                                                                     CustomSessionSerializer,
                                                                                                     CustomLocationSerializer,
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
                                                                                                     CustomEnvironmentalImpactSerializer,
                                                                                                     CustomChargingPeriodSerializer,
                                                                                                     CustomCDRDimensionSerializer),
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.OK,
                                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                                           //LastModified               = Timestamp.Now.ToIso8601()
                                                       }
                                                   };

                                    });

            #endregion

            #endregion



            #region ~/cdrs                                    [NonStandard]

            #region OPTIONS  ~/cdrs           [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPHostname.Any,
                HTTPMethod.OPTIONS,
                URLPathPrefix + "cdrs",
                OCPIRequestHandler: request =>

                    Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               HTTPResponseBuilder = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode              = HTTPStatusCode.OK,
                                   Allow                       = [ HTTPMethod.OPTIONS, HTTPMethod.GET, HTTPMethod.POST, HTTPMethod.DELETE ],
                                   AccessControlAllowMethods   = [ "OPTIONS", "GET", "POST", "DELETE"],
                                   AccessControlAllowHeaders   = [ "Authorization" ],
                                   AccessControlExposeHeaders  = [ "X-Request-ID", "X-Correlation-ID", "Link", "X-Total-Count", "X-Filtered-Count" ]
                               }
                        })

            );

            #endregion

            #region GET      ~/cdrs           [NonStandard]

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.GET,
                                    URLPathPrefix + "cdrs",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestLogger:   GetCDRsRequest,
                                    OCPIResponseLogger:  GetCDRsResponse,
                                    OCPIRequestHandler:  Request => {

                                        #region Check access token

                                        if (Request.LocalAccessInfo is null ||
                                            Request.LocalAccessInfo.Status != AccessStatus.ALLOWED ||
                                            Request.LocalAccessInfo.Role   != Roles.CPO)
                                        {

                                            return Task.FromResult(
                                                new OCPIResponse.Builder(Request) {
                                                    StatusCode           = 2000,
                                                    StatusMessage        = "Invalid or blocked access token!",
                                                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                        AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "DELETE" ],
                                                        AccessControlAllowHeaders  = [ "Authorization" ]
                                                    }
                                                });

                                        }

                                        #endregion


                                        var includeOwnerInformation  = Request.QueryString.GetBoolean("includeOwnerInformation") ?? false;
                                        var includeEnergyMeter       = Request.QueryString.GetBoolean("includeEnergyMeter")      ?? false;
                                        var includeCostDetails       = Request.QueryString.GetBoolean("includeCostDetails")      ?? false;

                                        var filters                  = Request.GetDateAndPaginationFilters();

                                        var allCDRs                  = CommonAPI.GetCDRs(Request.LocalAccessInfo.CountryCode,
                                                                                         Request.LocalAccessInfo.PartyId).
                                                                                 ToArray();

                                        var filteredCDRs             = allCDRs.Where(cdr => !filters.From.HasValue || cdr.LastUpdated >  filters.From.Value).
                                                                               Where(cdr => !filters.To.  HasValue || cdr.LastUpdated <= filters.To.  Value).
                                                                               ToArray();


                                        return Task.FromResult(
                                            new OCPIResponse.Builder(Request) {
                                                   StatusCode           = 1000,
                                                   StatusMessage        = "Hello world!",
                                                   Data                 = new JArray(
                                                                              filteredCDRs.
                                                                                  SkipTakeFilter(filters.Offset,
                                                                                                 filters.Limit).
                                                                                  Select(cdr => cdr.ToJSON(includeOwnerInformation,
                                                                                                           includeEnergyMeter,
                                                                                                           includeCostDetails,
                                                                                                           null,
                                                                                                           CustomCDRSerializer,
                                                                                                           CustomLocationSerializer,
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
                                                                                                           CustomEnvironmentalImpactSerializer,
                                                                                                           CustomTariffSerializer,
                                                                                                           CustomTariffElementSerializer,
                                                                                                           CustomPriceComponentSerializer,
                                                                                                           CustomTariffRestrictionsSerializer,
                                                                                                           CustomChargingPeriodSerializer,
                                                                                                           CustomCDRDimensionSerializer,
                                                                                                           CustomCDRCostDetailsSerializer,
                                                                                                           CustomSignedDataSerializer,
                                                                                                           CustomSignedValueSerializer))
                                                                          ),
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "DELETE" ],
                                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                                       //LastModified               = ?
                                                   }.
                                                   Set("X-Total-Count", allCDRs.Length)
                                                   // X-Limit               The maximum number of objects that the server WILL return.
                                                   // Link                  Link to the 'next' page should be provided when this is NOT the last page.
                                            });

                                    });

            #endregion

            #region POST     ~/cdrs       <= Unclear if this URL is correct!

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.POST,
                                    URLPathPrefix + "cdrs",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestLogger:   PostCDRRequest,
                                    OCPIResponseLogger:  PostCDRResponse,
                                    OCPIRequestHandler:  async Request => {

                                        #region Check access token

                                        if (Request.LocalAccessInfo?.Status != AccessStatus.ALLOWED ||
                                            Request.LocalAccessInfo?.Role   != Roles.CPO)
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = "Invalid or blocked access token!",
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "DELETE" ],
                                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                                       }
                                                   };

                                        }

                                        #endregion

                                        #region Parse newCDR JSON

                                        if (!Request.TryParseJObjectRequestBody(out var jsonCDR, out var ocpiResponseBuilder))
                                            return ocpiResponseBuilder;

                                        if (!CDR.TryParse(jsonCDR,
                                                          out var newCDR,
                                                          out var errorResponse,
                                                          Request.RemoteParty?.CountryCode,
                                                          Request.RemoteParty?.PartyId) ||
                                             newCDR is null)
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2001,
                                                       StatusMessage        = "Could not parse the given charge detail record JSON: " + errorResponse,
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "DELETE" ],
                                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                                       }
                                                   };

                                        }

                                        #endregion


                                        //ToDo: How do we verify, that this CPO does not send CDRs for other CPOs?


                                        var addResult = await CommonAPI.AddCDR(newCDR);


                                        // https://github.com/ocpi/ocpi/blob/release-2.1.1-bugfixes/mod_cdrs.md#response-headers
                                        // The response should contain the URL to the just created CDR object in the eMSP system.
                                        //
                                        // Parameter    Location
                                        // Datatype     URL
                                        // Required     yes
                                        // Description  URL to the newly created CDR in the eMSP system, can be used by the CPO system to do a GET on of the same CDR.
                                        // Example      Location: /ocpi/emsp/2.0/cdrs/123456

                                        if (addResult.IsSuccess)
                                            return new OCPIResponse.Builder(Request) {
                                                           StatusCode           = 1000,
                                                           StatusMessage        = "Hello world!",
                                                           Data                 = newCDR.ToJSON(),
                                                           HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                               HTTPStatusCode             = HTTPStatusCode.Created,
                                                               Location                   = org.GraphDefined.Vanaheimr.Hermod.HTTP.Location.From(URLPathPrefix + "cdrs" + newCDR.Id.ToString()),
                                                               AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "DELETE" ],
                                                               AccessControlAllowHeaders  = [ "Authorization" ],
                                                               LastModified               = newCDR.LastUpdated,
                                                               ETag                       = newCDR.ETag
                                                           }
                                                       };


                                        return new OCPIResponse.Builder(Request) {
                                                   StatusCode           = 2000,
                                                   StatusMessage        = addResult.ErrorResponse,
                                                   Data                 = newCDR.ToJSON(true,
                                                                                        true,
                                                                                        true,
                                                                                        null,
                                                                                        CustomCDRSerializer,
                                                                                        CustomLocationSerializer,
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
                                                                                        CustomEnvironmentalImpactSerializer,
                                                                                        CustomTariffSerializer,
                                                                                        CustomTariffElementSerializer,
                                                                                        CustomPriceComponentSerializer,
                                                                                        CustomTariffRestrictionsSerializer,
                                                                                        CustomChargingPeriodSerializer,
                                                                                        CustomCDRDimensionSerializer,
                                                                                        CustomCDRCostDetailsSerializer,
                                                                                        CustomSignedDataSerializer,
                                                                                        CustomSignedValueSerializer),
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                                   }
                                               };

                                    });

            #endregion

            #region DELETE   ~/cdrs           [NonStandard]

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.DELETE,
                                    URLPathPrefix + "cdrs",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestLogger:   DeleteCDRsRequest,
                                    OCPIResponseLogger:  DeleteCDRsResponse,
                                    OCPIRequestHandler:  async Request => {

                                        #region Check access token

                                        if (Request.LocalAccessInfo is null ||
                                            Request.LocalAccessInfo.Status != AccessStatus.ALLOWED ||
                                            Request.LocalAccessInfo.Role   != Roles.CPO)
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = "Invalid or blocked access token!",
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "DELETE" ],
                                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                                       }
                                                   };

                                        }

                                        #endregion


                                        var deleteResult = await CommonAPI.RemoveAllCDRs(Request.LocalAccessInfo.CountryCode,
                                                                                         Request.LocalAccessInfo.PartyId);


                                        return new OCPIResponse.Builder(Request) {
                                                   StatusCode           = 1000,
                                                   StatusMessage        = "Hello world!",
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "DELETE" ],
                                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                                   }
                                               };

                                    });

            #endregion

            #endregion

            #region ~/cdrs/{cdrId}

            #region OPTIONS  ~/cdrs/{cdrId}    [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPHostname.Any,
                HTTPMethod.OPTIONS,
                URLPathPrefix + "cdrs/{cdrId}",
                OCPIRequestHandler: request =>

                    Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               HTTPResponseBuilder = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   Allow                      = [ HTTPMethod.OPTIONS, HTTPMethod.GET, HTTPMethod.DELETE ],
                                   AcceptPatch                = [ HTTPContentType.Application.JSONMergePatch_UTF8 ],
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET", "DELETE" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ]
                               }
                        })

            );

            #endregion

            #region GET      ~/cdrs/{cdrId}       // The concrete URL is not specified by OCPI! m(

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.GET,
                                    URLPathPrefix + "cdrs/{cdrId}",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestLogger:   GetCDRRequest,
                                    OCPIResponseLogger:  GetCDRResponse,
                                    OCPIRequestHandler:  Request => {

                                        #region Check access token

                                        if (Request.LocalAccessInfo is null ||
                                            Request.LocalAccessInfo.Status != AccessStatus.ALLOWED ||
                                            Request.LocalAccessInfo.Role   != Roles.CPO)
                                        {

                                            return Task.FromResult(
                                                new OCPIResponse.Builder(Request) {
                                                    StatusCode           = 2000,
                                                    StatusMessage        = "Invalid or blocked access token!",
                                                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                        AccessControlAllowMethods  = [ "OPTIONS", "GET", "DELETE" ],
                                                        AccessControlAllowHeaders  = [ "Authorization" ]
                                                    }
                                                });

                                        }

                                        #endregion

                                        #region Check existing CDR

                                        if (!Request.ParseCDR(CommonAPI,
                                                              Request.LocalAccessInfo.CountryCode,
                                                              Request.LocalAccessInfo.PartyId,
                                                              out var cdrId,
                                                              out var cdr,
                                                              out var ocpiResponseBuilder,
                                                              FailOnMissingCDR: true) ||
                                             cdr is null)
                                        {
                                            return Task.FromResult(ocpiResponseBuilder!);
                                        }

                                        #endregion


                                        var includeOwnerInformation  = Request.QueryString.GetBoolean("includeOwnerInformation") ?? false;
                                        var includeEnergyMeter       = Request.QueryString.GetBoolean("includeEnergyMeter")      ?? false;
                                        var includeCostDetails       = Request.QueryString.GetBoolean("includeCostDetails")      ?? false;

                                        return Task.FromResult(
                                            new OCPIResponse.Builder(Request) {
                                                   StatusCode           = 1000,
                                                   StatusMessage        = "Hello world!",
                                                   Data                 = cdr.ToJSON(includeOwnerInformation,
                                                                                     includeEnergyMeter,
                                                                                     includeCostDetails,
                                                                                     null,
                                                                                     CustomCDRSerializer,
                                                                                     CustomLocationSerializer,
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
                                                                                     CustomEnvironmentalImpactSerializer,
                                                                                     CustomTariffSerializer,
                                                                                     CustomTariffElementSerializer,
                                                                                     CustomPriceComponentSerializer,
                                                                                     CustomTariffRestrictionsSerializer,
                                                                                     CustomChargingPeriodSerializer,
                                                                                     CustomCDRDimensionSerializer,
                                                                                     CustomCDRCostDetailsSerializer,
                                                                                     CustomSignedDataSerializer,
                                                                                     CustomSignedValueSerializer),
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "DELETE" ],
                                                       AccessControlAllowHeaders  = [ "Authorization" ],
                                                       LastModified               = cdr.LastUpdated,
                                                       ETag                       = cdr.ETag
                                                   }
                                            });

                                    });

            #endregion

            #region DELETE   ~/cdrs/{cdrId}    [NonStandard]

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.DELETE,
                                    URLPathPrefix + "cdrs/{cdrId}",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestLogger:   DeleteCDRRequest,
                                    OCPIResponseLogger:  DeleteCDRResponse,
                                    OCPIRequestHandler:  async Request => {

                                        #region Check access token

                                        if (Request.LocalAccessInfo is null ||
                                            Request.LocalAccessInfo.Status != AccessStatus.ALLOWED ||
                                            Request.LocalAccessInfo.Role   != Roles.CPO)
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = "Invalid or blocked access token!",
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "DELETE" ],
                                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                                       }
                                                   };

                                        }

                                        #endregion

                                        #region Check existing CDR

                                        if (!Request.ParseCDR(CommonAPI,
                                                              Request.LocalAccessInfo.CountryCode,
                                                              Request.LocalAccessInfo.PartyId, 
                                                              out var cdrId,
                                                              out var existingCDR,
                                                              out var ocpiResponseBuilder,
                                                              FailOnMissingCDR: true) ||
                                             existingCDR is null)
                                        {
                                            return ocpiResponseBuilder!;
                                        }

                                        #endregion


                                        var deleteResult = await CommonAPI.RemoveCDR(existingCDR);


                                        if (deleteResult.IsSuccess)
                                            return new OCPIResponse.Builder(Request) {
                                                           StatusCode           = 1000,
                                                           StatusMessage        = "Hello world!",
                                                           Data                 = existingCDR.ToJSON(true,
                                                                                                     true,
                                                                                                     true,
                                                                                                     null,
                                                                                                     CustomCDRSerializer,
                                                                                                     CustomLocationSerializer,
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
                                                                                                     CustomEnvironmentalImpactSerializer,
                                                                                                     CustomTariffSerializer,
                                                                                                     CustomTariffElementSerializer,
                                                                                                     CustomPriceComponentSerializer,
                                                                                                     CustomTariffRestrictionsSerializer,
                                                                                                     CustomChargingPeriodSerializer,
                                                                                                     CustomCDRDimensionSerializer,
                                                                                                     CustomCDRCostDetailsSerializer,
                                                                                                     CustomSignedDataSerializer,
                                                                                                     CustomSignedValueSerializer),
                                                           HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                               HTTPStatusCode             = HTTPStatusCode.OK,
                                                               AccessControlAllowMethods  = [ "OPTIONS", "GET", "DELETE" ],
                                                               AccessControlAllowHeaders  = [ "Authorization" ],
                                                               LastModified               = existingCDR.LastUpdated,
                                                               ETag                       = existingCDR.ETag
                                                           }
                                                       };


                                        return new OCPIResponse.Builder(Request) {
                                                   StatusCode           = 2000,
                                                   StatusMessage        = deleteResult.ErrorResponse,
                                                   Data                 = existingCDR.ToJSON(true,
                                                                                             true,
                                                                                             true,
                                                                                             null,
                                                                                             CustomCDRSerializer,
                                                                                             CustomLocationSerializer,
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
                                                                                             CustomEnvironmentalImpactSerializer,
                                                                                             CustomTariffSerializer,
                                                                                             CustomTariffElementSerializer,
                                                                                             CustomPriceComponentSerializer,
                                                                                             CustomTariffRestrictionsSerializer,
                                                                                             CustomChargingPeriodSerializer,
                                                                                             CustomCDRDimensionSerializer,
                                                                                             CustomCDRCostDetailsSerializer,
                                                                                             CustomSignedDataSerializer,
                                                                                             CustomSignedValueSerializer),
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "DELETE" ],
                                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                                   }
                                               };

                                    });

            #endregion

            #endregion



            #region ~/tokens

            #region OPTIONS  ~/tokens

            // https://example.com/ocpi/2.2/cpo/tokens/?date_from=2019-01-28T12:00:00&date_to=2019-01-29T12:00:00&offset=50&limit=100
            CommonAPI.AddOCPIMethod(

                HTTPHostname.Any,
                HTTPMethod.OPTIONS,
                URLPathPrefix + "tokens",
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

            #region GET      ~/tokens

            // https://example.com/ocpi/2.2/cpo/tokens/?date_from=2019-01-28T12:00:00&date_to=2019-01-29T12:00:00&offset=50&limit=100
            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.GET,
                                    URLPathPrefix + "tokens",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestLogger:   GetTokensRequest,
                                    OCPIResponseLogger:  GetTokensResponse,
                                    OCPIRequestHandler:  Request => {

                                        #region Check access token

                                        if (Request.LocalAccessInfo?.Status != AccessStatus.ALLOWED ||
                                            Request.LocalAccessInfo?.Role   != Roles.CPO)
                                        {

                                            return Task.FromResult(
                                                new OCPIResponse.Builder(Request) {
                                                    StatusCode           = 2000,
                                                    StatusMessage        = "Invalid or blocked access token!",
                                                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                        AccessControlAllowMethods  = new[] { "OPTIONS", "GET" },
                                                        AccessControlAllowHeaders  = [ "Authorization" ]
                                                    }
                                                });

                                        }

                                        #endregion


                                        var filters         = Request.GetDateAndPaginationFilters();

                                        var allTokens       = CommonAPI.GetTokens().
                                                                        Select(tokenStatus => tokenStatus.Token).
                                                                        ToArray();

                                        var filteredTokens  = allTokens.Where(token => !filters.From.HasValue || token.LastUpdated >  filters.From.Value).
                                                                        Where(token => !filters.To.  HasValue || token.LastUpdated <= filters.To.  Value).
                                                                        ToArray();


                                        return Task.FromResult(
                                            new OCPIResponse.Builder(Request) {
                                                   StatusCode           = 1000,
                                                   StatusMessage        = "Hello world!",
                                                   Data                 = new JArray(
                                                                              filteredTokens.
                                                                                  SkipTakeFilter(filters.Offset,
                                                                                                 filters.Limit).
                                                                                  Select(token => token.ToJSON(false,
                                                                                                               CustomTokenSerializer))
                                                                          ),
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = new[] { "OPTIONS", "GET" },
                                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                                       //LastModified               = ?
                                                   }.
                                                   Set("X-Total-Count", allTokens.Length)
                                                   // X-Limit               The maximum number of objects that the server WILL return.
                                                   // Link                  Link to the 'next' page should be provided when this is NOT the last page.
                                            });

                                    });

            #endregion

            #endregion

            #region ~/tokens/{token_id}/authorize

            #region OPTIONS  ~/tokens/{token_id}/authorize

            CommonAPI.AddOCPIMethod(

                HTTPHostname.Any,
                HTTPMethod.OPTIONS,
                URLPathPrefix + "tokens/{token_id}/authorize",
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

            #region POST     ~/tokens/{token_id}/authorize?type=RFID

            // A real-time authorization request
            // https://example.com/ocpi/2.2/emsp/tokens/012345678/authorize?type=RFID
            // curl -X POST http://127.0.0.1:3000/2.2/emsp/tokens/012345678/authorize?type=RFID
            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.POST,
                                    URLPathPrefix + "tokens/{token_id}/authorize",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestLogger:   PostTokenRequest,
                                    OCPIResponseLogger:  PostTokenResponse,
                                    OCPIRequestHandler:  async Request => {

                                        #region Check access token

                                        if (Request.LocalAccessInfo is null ||
                                            Request.LocalAccessInfo.Status != AccessStatus.ALLOWED ||
                                            Request.LocalAccessInfo.Role   != Roles.CPO)
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = "Invalid or blocked access token!",
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                           AccessControlAllowMethods  = new[] { "OPTIONS", "POST" },
                                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                                       }
                                                   };

                                        }

                                        #endregion

                                        #region Check TokenId URI parameter

                                        if (!Request.ParseTokenId(CommonAPI,
                                                                  out var tokenId,
                                                                  out var ocpiResponseBuilder) ||
                                             !tokenId.HasValue)
                                        {
                                            return ocpiResponseBuilder!;
                                        }

                                        #endregion

                                        var requestedTokenType  = Request.QueryString.Map("type", TokenType.TryParse) ?? TokenType.RFID;

                                        #region Parse optional LocationReference JSON

                                        LocationReference? locationReference = null;

                                        if (Request.TryParseJObjectRequestBody(out var locationReferenceJSON,
                                                                               out ocpiResponseBuilder,
                                                                               AllowEmptyHTTPBody: true))
                                        {

                                            if (!LocationReference.TryParse(locationReferenceJSON,
                                                                            out var _locationReference,
                                                                            out var errorResponse))
                                            {

                                                return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2001,
                                                       StatusMessage        = "Could not parse the given location reference JSON: " + errorResponse,
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                           AccessControlAllowMethods  = new[] { "OPTIONS", "POST" },
                                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                                       }
                                                   };

                                            }

                                            locationReference = _locationReference;

                                        }

                                        #endregion


                                        AuthorizationInfo? authorizationInfo = null;

                                        var onRFIDAuthTokenLocal = OnRFIDAuthToken;
                                        if (onRFIDAuthTokenLocal is not null)
                                        {

                                            try
                                            {

                                                var result = onRFIDAuthTokenLocal(Request.LocalAccessInfo.CountryCode,
                                                                                  Request.LocalAccessInfo.PartyId,
                                                                                  tokenId.Value,
                                                                                  locationReference).Result;

                                                authorizationInfo = result;

                                            }
                                            catch (Exception e)
                                            {

                                            }

                                        }

                                        else
                                        {

                                            #region Check existing token

                                            if (!CommonAPI.TryGetToken(tokenId.Value, out var _tokenStatus) ||
                                                (_tokenStatus.Token.Type != requestedTokenType))
                                            {

                                                return new OCPIResponse.Builder(Request) {
                                                           StatusCode           = 2004,
                                                           StatusMessage        = "Unknown token!",
                                                           HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                               HTTPStatusCode             = HTTPStatusCode.NotFound,
                                                               AccessControlAllowMethods  = new[] { "OPTIONS", "POST" },
                                                               AccessControlAllowHeaders  = [ "Authorization" ]
                                                           }
                                                       };

                                            }

                                            #endregion

                                            authorizationInfo = new AuthorizationInfo(
                                                                      _tokenStatus.Status,
                                                                      _tokenStatus.LocationReference
                                                                      //new DisplayText(
                                                                      //    _tokenStatus.Token.UILanguage ?? Languages.en,
                                                                      //    responseText
                                                                      //)
                                                                );

                                            #region Parse optional LocationReference JSON

                                            if (locationReference.HasValue)
                                            {

                                                if (!CommonAPI.TryGetLocation(locationReference.Value.LocationId,
                                                                              out var validLocation))
                                                {

                                                    return new OCPIResponse.Builder(Request) {
                                                        StatusCode           = 2001,
                                                        StatusMessage        = "The given location is unknown!",
                                                        Data                 = new AuthorizationInfo(
                                                                                   AllowedType.NOT_ALLOWED,
                                                                                   locationReference.Value,
                                                                                   new DisplayText(Languages.en,
                                                                                                   "The given location is unknown!")
                                                                               ).ToJSON(),
                                                        HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                            HTTPStatusCode             = HTTPStatusCode.NotFound,
                                                            AccessControlAllowMethods  = new[] { "OPTIONS", "POST" },
                                                            AccessControlAllowHeaders  = [ "Authorization" ]
                                                        }
                                                    };

                                                }

                                                else
                                                {

                                                    //if (Request.AccessInfo.Value.Roles.Where(role => role.Role == Roles.CPO).Count() != 1)
                                                    //{

                                                    //    return new OCPIResponse.Builder(Request) {
                                                    //        StatusCode           = 2001,
                                                    //        StatusMessage        = "Could not determine the country code and party identification of the given location!",
                                                    //        Data                 = new AuthorizationInfo(
                                                    //                                   AllowedTypes.NOT_ALLOWED,
                                                    //                                   _tokenStatus.Token,
                                                    //                                   locationReference.Value
                                                    //                               ).ToJSON(),
                                                    //        HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                    //            HTTPStatusCode             = HTTPStatusCode.NotFound,
                                                    //            AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST" },
                                                    //            AccessControlAllowHeaders  = [ "Authorization" ]
                                                    //        }
                                                    //    };

                                                    //}

                                                    //var allTheirCPORoles = Request.AccessInfo.Value.Roles.Where(role => role.Role == Roles.CPO).ToArray();

                                                    //if (!CommonAPI.TryGetLocation(allTheirCPORoles[0].CountryCode,
                                                    //                              allTheirCPORoles[0].PartyId,
                                                    //                              locationReference.Value.LocationId,
                                                    //                              out validLocation))
                                                    //{

                                                    //    return new OCPIResponse.Builder(Request) {
                                                    //        StatusCode           = 2001,
                                                    //        StatusMessage        = "The given location is unknown!",
                                                    //        Data                 = new AuthorizationInfo(
                                                    //                                   AllowedTypes.NOT_ALLOWED,
                                                    //                                   _tokenStatus.Token,
                                                    //                                   locationReference.Value,
                                                    //                                   null,
                                                    //                                   new DisplayText(Languages.en, "The given location is unknown!")
                                                    //                               ).ToJSON(),
                                                    //        HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                    //            HTTPStatusCode             = HTTPStatusCode.NotFound,
                                                    //            AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST" },
                                                    //            AccessControlAllowHeaders  = [ "Authorization" ]
                                                    //        }
                                                    //    };

                                                    //}

                                                }


                                                //ToDo: Add a event/delegate for addditional location filters!


                                                if (locationReference.Value.EVSEUIds.SafeAny())
                                                {

                                                    locationReference = new LocationReference(locationReference.Value.LocationId,
                                                                                              locationReference.Value.EVSEUIds.
                                                                                                                      Where(evseuid => validLocation.EVSEExists(evseuid)));

                                                    if (!locationReference.Value.EVSEUIds.SafeAny())
                                                    {

                                                        return new OCPIResponse.Builder(Request) {
                                                            StatusCode           = 2001,
                                                            StatusMessage        = locationReference.Value.EVSEUIds.Count() == 1
                                                                                       ? "The EVSE at the given location is unknown!"
                                                                                       : "The EVSEs at the given location are unknown!",
                                                            Data                 = new AuthorizationInfo(
                                                                                       AllowedType.NOT_ALLOWED,
                                                                                       locationReference.Value,
                                                                                       new DisplayText(
                                                                                           Languages.en,
                                                                                           locationReference.Value.EVSEUIds.Count() == 1
                                                                                               ? "The EVSE at the given location is unknown!"
                                                                                               : "The EVSEs at the given location are unknown!"
                                                                                       )
                                                                                   ).ToJSON(),
                                                            HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                                HTTPStatusCode             = HTTPStatusCode.NotFound,
                                                                AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST" },
                                                                AccessControlAllowHeaders  = [ "Authorization" ]
                                                            }
                                                        };

                                                    }



                                                    //ToDo: Add a event/delegate for addditional EVSE filters!

                                                }

                                            }

                                            #endregion

                                        }

                                        authorizationInfo ??= new AuthorizationInfo(
                                                                  AllowedType.BLOCKED
                                                              );


                                        // too little information like e.g. no LocationReferences provided:
                                        //   => status_code 2002


                                        #region Set a user-friendly response message for the ev driver

                                        var responseText = "An error occured!";

                                        if (!authorizationInfo.Info.HasValue)
                                        {

                                            if (authorizationInfo.Allowed == AllowedType.ALLOWED)
                                                responseText = "Charging allowed!";

                                            else if (authorizationInfo.Allowed == AllowedType.BLOCKED)
                                                responseText = "Sorry, your token is blocked!";

                                            else if (authorizationInfo.Allowed == AllowedType.EXPIRED)
                                                responseText = "Sorry, your token has expired!";

                                            else if (authorizationInfo.Allowed == AllowedType.NO_CREDIT)
                                                responseText = "Sorry, your have not enough credits for charging!";

                                            else if (authorizationInfo.Allowed == AllowedType.NOT_ALLOWED)
                                                responseText = "Sorry, charging is not allowed!";

                                        }

                                        #endregion


                                        return new OCPIResponse.Builder(Request) {
                                                   StatusCode           = 1000,
                                                   StatusMessage        = "Hello world!",
                                                   Data                 = new AuthorizationInfo(
                                                                              authorizationInfo.Allowed,
                                                                              authorizationInfo.Location,
                                                                              authorizationInfo.Info ?? new DisplayText(
                                                                                                            Languages.en,
                                                                                                            responseText
                                                                                                        )
                                                                          ).ToJSON(CustomAuthorizationInfoSerializer,
                                                                                   CustomLocationReferenceSerializer,
                                                                                   CustomDisplayTextSerializer),
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = new[] { "OPTIONS", "POST" },
                                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                                   }
                                               };

                                    });

            #endregion

            #endregion



            // Command result callbacks

            #region ~/commands/RESERVE_NOW/{commandId}

            #region OPTIONS  ~/commands/RESERVE_NOW/{commandId}

            CommonAPI.AddOCPIMethod(

                HTTPHostname.Any,
                HTTPMethod.OPTIONS,
                URLPathPrefix + "commands/RESERVE_NOW/{commandId}",
                OCPIRequestHandler: request =>

                    Task.FromResult(
                        new OCPIResponse.Builder(request) {
                            HTTPResponseBuilder = new HTTPResponse.Builder(request.HTTPRequest) {
                                HTTPStatusCode             = HTTPStatusCode.OK,
                                AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
                                AccessControlAllowHeaders  = [ "Authorization" ]
                            }
                        })

            );

            #endregion

            #region POST     ~/commands/RESERVE_NOW/{commandId}

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.POST,
                                    URLPathPrefix + "commands/RESERVE_NOW/{commandId}",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestLogger:   ReserveNowCallbackRequest,
                                    OCPIResponseLogger:  ReserveNowCallbackResponse,
                                    OCPIRequestHandler:  Request => {

                                        #region Check command identification

                                        if (!Request.ParseCommandId(CommonAPI,
                                                                    out var commandId,
                                                                    out var ocpiResponseBuilder) ||
                                            !commandId.HasValue)
                                        {
                                            return Task.FromResult(ocpiResponseBuilder!);
                                        }

                                        #endregion

                                        #region Parse command result JSON

                                        if (!Request.TryParseJObjectRequestBody(out var json, out ocpiResponseBuilder))
                                            return Task.FromResult(ocpiResponseBuilder);

                                        if (!CommandResult.TryParse(json,
                                                                    out var commandResult,
                                                                    out var errorResponse))
                                        {

                                            return Task.FromResult(
                                                       new OCPIResponse.Builder(Request) {
                                                           StatusCode           = 2001,
                                                           StatusMessage        = "Could not parse the given 'RESERVE NOW' command result JSON: " + errorResponse,
                                                           HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                               HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                               AccessControlAllowMethods  = new[] { "OPTIONS", "POST" },
                                                               AccessControlAllowHeaders  = [ "Authorization" ]
                                                           }
                                                       }
                                                   );

                                        }

                                        #endregion


                                        if (CommonAPI.CommandValueStore.TryGetValue(commandId.Value, out var commandValues))
                                        {

                                            commandValues.Result = commandResult;

                                            return Task.FromResult(
                                                       new OCPIResponse.Builder(Request) {
                                                           StatusCode           = 1000,
                                                           HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                               HTTPStatusCode             = HTTPStatusCode.Accepted,
                                                               AccessControlAllowMethods  = new[] { "OPTIONS", "POST" },
                                                               AccessControlAllowHeaders  = [ "Authorization" ]
                                                           }
                                                       }
                                                   );

                                        }

                                        return Task.FromResult(
                                                   new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = "Unknown 'RESERVE NOW' command identification!",
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.OK,
                                                           AccessControlAllowMethods  = new[] { "OPTIONS", "POST" },
                                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                                       }
                                                   }
                                               );

                                    });

            #endregion

            #endregion

            #region ~/commands/CANCEL_RESERVATION/{commandId}

            #region OPTIONS  ~/commands/CANCEL_RESERVATION/{commandId}

            CommonAPI.AddOCPIMethod(

                HTTPHostname.Any,
                HTTPMethod.OPTIONS,
                URLPathPrefix + "commands/CANCEL_RESERVATION/{commandId}",
                OCPIRequestHandler: request =>

                    Task.FromResult(
                        new OCPIResponse.Builder(request) {
                            HTTPResponseBuilder = new HTTPResponse.Builder(request.HTTPRequest) {
                                HTTPStatusCode             = HTTPStatusCode.OK,
                                AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
                                AccessControlAllowHeaders  = [ "Authorization" ]
                            }
                        })

            );

            #endregion

            #region POST     ~/commands/CANCEL_RESERVATION/{commandId}

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.POST,
                                    URLPathPrefix + "commands/CANCEL_RESERVATION/{commandId}",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestLogger:   CancelReservationCallbackRequest,
                                    OCPIResponseLogger:  CancelReservationCallbackResponse,
                                    OCPIRequestHandler:  Request => {

                                        #region Check command identification

                                        if (!Request.ParseCommandId(CommonAPI,
                                                                    out var commandId,
                                                                    out var ocpiResponseBuilder) ||
                                            !commandId.HasValue)
                                        {
                                            return Task.FromResult(ocpiResponseBuilder!);
                                        }

                                        #endregion

                                        #region Parse command result JSON

                                        if (!Request.TryParseJObjectRequestBody(out var json, out ocpiResponseBuilder))
                                            return Task.FromResult(ocpiResponseBuilder);

                                        if (!CommandResult.TryParse(json,
                                                                    out var commandResult,
                                                                    out var errorResponse))
                                        {

                                            return Task.FromResult(
                                                       new OCPIResponse.Builder(Request) {
                                                           StatusCode           = 2001,
                                                           StatusMessage        = "Could not parse the given 'CANCEL RESERVATION' command result JSON: " + errorResponse,
                                                           HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                               HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                               AccessControlAllowMethods  = new[] { "OPTIONS", "POST" },
                                                               AccessControlAllowHeaders  = [ "Authorization" ]
                                                           }
                                                       }
                                                   );

                                        }

                                        #endregion


                                        if (CommonAPI.CommandValueStore.TryGetValue(commandId.Value, out var commandValues))
                                        {

                                            commandValues.Result = commandResult;

                                            return Task.FromResult(
                                                       new OCPIResponse.Builder(Request) {
                                                           StatusCode           = 1000,
                                                           HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                               HTTPStatusCode             = HTTPStatusCode.Accepted,
                                                               AccessControlAllowMethods  = new[] { "OPTIONS", "POST" },
                                                               AccessControlAllowHeaders  = [ "Authorization" ]
                                                           }
                                                       }
                                                   );

                                        }

                                        return Task.FromResult(
                                                   new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = "Unknown 'CANCEL RESERVATION' command identification!",
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.OK,
                                                           AccessControlAllowMethods  = new[] { "OPTIONS", "POST" },
                                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                                       }
                                                   }
                                               );

                                    });

            #endregion

            #endregion

            #region ~/commands/START_SESSION/{commandId}

            #region OPTIONS  ~/commands/START_SESSION/{commandId}

            CommonAPI.AddOCPIMethod(

                HTTPHostname.Any,
                HTTPMethod.OPTIONS,
                URLPathPrefix + "commands/START_SESSION/{commandId}",
                OCPIRequestHandler: request =>

                    Task.FromResult(
                        new OCPIResponse.Builder(request) {
                            HTTPResponseBuilder = new HTTPResponse.Builder(request.HTTPRequest) {
                                HTTPStatusCode             = HTTPStatusCode.OK,
                                AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
                                AccessControlAllowHeaders  = [ "Authorization" ]
                            }
                        })

            );

            #endregion

            #region POST     ~/commands/START_SESSION/{commandId}

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.POST,
                                    URLPathPrefix + "commands/START_SESSION/{commandId}",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestLogger:   StartSessionCallbackRequest,
                                    OCPIResponseLogger:  StartSessionCallbackResponse,
                                    OCPIRequestHandler:  Request => {

                                        #region Check command identification

                                        if (!Request.ParseCommandId(CommonAPI,
                                                                    out var commandId,
                                                                    out var ocpiResponseBuilder) ||
                                            !commandId.HasValue)
                                        {
                                            return Task.FromResult(ocpiResponseBuilder!);
                                        }

                                        #endregion

                                        #region Parse command result JSON

                                        if (!Request.TryParseJObjectRequestBody(out var json, out ocpiResponseBuilder))
                                            return Task.FromResult(ocpiResponseBuilder);

                                        if (!CommandResult.TryParse(json,
                                                                    out var commandResult,
                                                                    out var errorResponse))
                                        {

                                            return Task.FromResult(
                                                       new OCPIResponse.Builder(Request) {
                                                           StatusCode           = 2001,
                                                           StatusMessage        = "Could not parse the given 'START SESSION' command result JSON: " + errorResponse,
                                                           HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                               HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                               AccessControlAllowMethods  = new[] { "OPTIONS", "POST" },
                                                               AccessControlAllowHeaders  = [ "Authorization" ]
                                                           }
                                                       }
                                                   );

                                        }

                                        #endregion


                                        if (CommonAPI.CommandValueStore.TryGetValue(commandId.Value, out var commandValues))
                                        {

                                            commandValues.Result = commandResult;

                                            return Task.FromResult(
                                                       new OCPIResponse.Builder(Request) {
                                                           StatusCode           = 1000,
                                                           HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                               HTTPStatusCode             = HTTPStatusCode.Accepted,
                                                               AccessControlAllowMethods  = new[] { "OPTIONS", "POST" },
                                                               AccessControlAllowHeaders  = [ "Authorization" ]
                                                           }
                                                       }
                                                   );

                                        }

                                        return Task.FromResult(
                                                   new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = "Unknown 'START SESSION' command identification!",
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.OK,
                                                           AccessControlAllowMethods  = new[] { "OPTIONS", "POST" },
                                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                                       }
                                                   }
                                               );

                                    });

            #endregion

            #endregion

            #region ~/commands/STOP_SESSION/{commandId}

            #region OPTIONS  ~/commands/STOP_SESSION/{commandId}

            CommonAPI.AddOCPIMethod(

                HTTPHostname.Any,
                HTTPMethod.OPTIONS,
                URLPathPrefix + "commands/STOP_SESSION/{commandId}",
                OCPIRequestHandler: request =>

                    Task.FromResult(
                        new OCPIResponse.Builder(request) {
                            HTTPResponseBuilder = new HTTPResponse.Builder(request.HTTPRequest) {
                                HTTPStatusCode             = HTTPStatusCode.OK,
                                AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
                                AccessControlAllowHeaders  = [ "Authorization" ]
                            }
                        })

            );

            #endregion

            #region POST     ~/commands/STOP_SESSION/{commandId}

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.POST,
                                    URLPathPrefix + "commands/STOP_SESSION/{commandId}",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestLogger:   StopSessionCallbackRequest,
                                    OCPIResponseLogger:  StopSessionCallbackResponse,
                                    OCPIRequestHandler:  Request => {

                                        #region Check command identification

                                        if (!Request.ParseCommandId(CommonAPI,
                                                                    out var commandId,
                                                                    out var ocpiResponseBuilder) ||
                                            !commandId.HasValue)
                                        {
                                            return Task.FromResult(ocpiResponseBuilder!);
                                        }

                                        #endregion

                                        #region Parse command result JSON

                                        if (!Request.TryParseJObjectRequestBody(out var json, out ocpiResponseBuilder))
                                            return Task.FromResult(ocpiResponseBuilder);

                                        if (!CommandResult.TryParse(json,
                                                                    out var commandResult,
                                                                    out var errorResponse))
                                        {

                                            return Task.FromResult(
                                                       new OCPIResponse.Builder(Request) {
                                                           StatusCode           = 2001,
                                                           StatusMessage        = "Could not parse the given 'STOP SESSION' command result JSON: " + errorResponse,
                                                           HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                               HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                               AccessControlAllowMethods  = new[] { "OPTIONS", "POST" },
                                                               AccessControlAllowHeaders  = [ "Authorization" ]
                                                           }
                                                       }
                                                   );

                                        }

                                        #endregion


                                        if (CommonAPI.CommandValueStore.TryGetValue(commandId.Value, out var commandValues))
                                        {

                                            commandValues.Result = commandResult;

                                            return Task.FromResult(
                                                       new OCPIResponse.Builder(Request) {
                                                           StatusCode           = 1000,
                                                           HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                               HTTPStatusCode             = HTTPStatusCode.Accepted,
                                                               AccessControlAllowMethods  = new[] { "OPTIONS", "POST" },
                                                               AccessControlAllowHeaders  = [ "Authorization" ]
                                                           }
                                                       }
                                                   );

                                        }

                                        return Task.FromResult(
                                                   new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = "Unknown 'STOP SESSION' command identification!",
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.OK,
                                                           AccessControlAllowMethods  = new[] { "OPTIONS", "POST" },
                                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                                       }
                                                   }
                                               );

                                    });

            #endregion

            #endregion

            #region ~/commands/UNLOCK_CONNECTOR/{commandId}

            #region OPTIONS  ~/commands/UNLOCK_CONNECTOR/{commandId}

            CommonAPI.AddOCPIMethod(

                HTTPHostname.Any,
                HTTPMethod.OPTIONS,
                URLPathPrefix + "commands/UNLOCK_CONNECTOR/{commandId}",
                OCPIRequestHandler: request =>

                    Task.FromResult(
                        new OCPIResponse.Builder(request) {
                            HTTPResponseBuilder = new HTTPResponse.Builder(request.HTTPRequest) {
                                HTTPStatusCode             = HTTPStatusCode.OK,
                                AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
                                AccessControlAllowHeaders  = [ "Authorization" ]
                            }
                        })

            );

            #endregion

            #region POST     ~/commands/UNLOCK_CONNECTOR/{commandId}

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.POST,
                                    URLPathPrefix + "commands/UNLOCK_CONNECTOR/{commandId}",
                                    HTTPContentType.Application.JSON_UTF8,
                                    OCPIRequestLogger:   UnlockConnectorCallbackRequest,
                                    OCPIResponseLogger:  UnlockConnectorCallbackResponse,
                                    OCPIRequestHandler:  Request => {

                                        #region Check command identification

                                        if (!Request.ParseCommandId(CommonAPI,
                                                                    out var commandId,
                                                                    out var ocpiResponseBuilder) ||
                                            !commandId.HasValue)
                                        {
                                            return Task.FromResult(ocpiResponseBuilder!);
                                        }

                                        #endregion

                                        #region Parse command result JSON

                                        if (!Request.TryParseJObjectRequestBody(out var json, out ocpiResponseBuilder))
                                            return Task.FromResult(ocpiResponseBuilder);

                                        if (!CommandResult.TryParse(json,
                                                                    out var commandResult,
                                                                    out var errorResponse))
                                        {

                                            return Task.FromResult(
                                                       new OCPIResponse.Builder(Request) {
                                                           StatusCode           = 2001,
                                                           StatusMessage        = "Could not parse the given 'UNLOCK CONNECTOR' command result JSON: " + errorResponse,
                                                           HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                               HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                               AccessControlAllowMethods  = new[] { "OPTIONS", "POST" },
                                                               AccessControlAllowHeaders  = [ "Authorization" ]
                                                           }
                                                       }
                                                   );

                                        }

                                        #endregion


                                        if (CommonAPI.CommandValueStore.TryGetValue(commandId.Value, out var commandValues))
                                        {

                                            commandValues.Result = commandResult;

                                            return Task.FromResult(
                                                       new OCPIResponse.Builder(Request) {
                                                           StatusCode           = 1000,
                                                           HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                               HTTPStatusCode             = HTTPStatusCode.Accepted,
                                                               AccessControlAllowMethods  = new[] { "OPTIONS", "POST" },
                                                               AccessControlAllowHeaders  = [ "Authorization" ]
                                                           }
                                                       }
                                                   );

                                        }

                                        return Task.FromResult(
                                                   new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = "Unknown 'UNLOCK CONNECTOR' command identification!",
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.OK,
                                                           AccessControlAllowMethods  = new[] { "OPTIONS", "POST" },
                                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                                       }
                                                   }
                                               );

                                    });

            #endregion

            #endregion

        }

        #endregion


    }

}
