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
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.HTTPTest;

using cloud.charging.open.protocols.OCPI;
using cloud.charging.open.protocols.OCPIv2_2_1.EMSP.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2_1
{

    /// <summary>
    /// The HTTP API for e-mobility service providers.
    /// CPOs will connect to this API.
    /// </summary>
    public class EMSPAPI : AHTTPExtAPIXExtension2<CommonAPI, HTTPExtAPIX>
    {

        #region Data

        /// <summary>
        /// The default HTTP server name.
        /// </summary>
        public new const           String    DefaultHTTPServiceName   = $"GraphDefined OCPI {Version.String} EMSP HTTP API";

        /// <summary>
        /// The default HTTP URL path prefix.
        /// </summary>
        public     static readonly HTTPPath  DefaultURLPathPrefix     = HTTPPath.Parse($"{Version.String}/cpo/");

        /// <summary>
        /// The default EMSP API logfile name.
        /// </summary>
        public     const           String    DefaultLogfileName       = $"OCPI{Version.String}_EMSPAPI.log";

        #endregion

        #region Properties

        /// <summary>
        /// The OCPI CommonAPI.
        /// </summary>
        public CommonAPI       CommonAPI
            => HTTPBaseAPI;

        /// <summary>
        /// (Dis-)allow PUTting of object having an earlier 'LastUpdated'-timestamp then already existing objects.
        /// OCPI v2.2 does not define any behaviour for this.
        /// </summary>
        public Boolean?        AllowDowngrades    { get; }

                /// <summary>
        /// The timeout for upstream requests.
        /// </summary>
        public TimeSpan        RequestTimeout     { get; set; }

        /// <summary>
        /// The EMSP API logger.
        /// </summary>
        public EMSPAPILogger?  Logger             { get; set; }

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

        public CustomJObjectSerializerDelegate<AuthorizationInfo>?           CustomAuthorizationInfoSerializer             { get; set; }
        public CustomJObjectSerializerDelegate<LocationReference>?           CustomLocationReferenceSerializer             { get; set; }

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


        #region (protected internal) DeleteLocationsRequest (Request)

        /// <summary>
        /// An event sent whenever a delete locations request was received.
        /// </summary>
        public OCPIRequestLogEvent OnDeleteLocationsRequest = new();

        /// <summary>
        /// An event sent whenever a delete locations request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task DeleteLocationsRequest(DateTimeOffset     Timestamp,
                                                       HTTPAPIX           API,
                                                       OCPIRequest        Request,
                                                       CancellationToken  CancellationToken)

            => OnDeleteLocationsRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) DeleteLocationsResponse(Response)

        /// <summary>
        /// An event sent whenever a delete locations response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnDeleteLocationsResponse = new();

        /// <summary>
        /// An event sent whenever a delete locations response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task DeleteLocationsResponse(DateTimeOffset     Timestamp,
                                                        HTTPAPIX           API,
                                                        OCPIRequest        Request,
                                                        OCPIResponse       Response,
                                                        CancellationToken  CancellationToken)

            => OnDeleteLocationsResponse.WhenAll(
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


        #region (protected internal) PutLocationRequest    (Request)

        /// <summary>
        /// An event sent whenever a PUT location request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPutLocationRequest = new();

        /// <summary>
        /// An event sent whenever a PUT location request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PutLocationRequest(DateTimeOffset     Timestamp,
                                                   HTTPAPIX           API,
                                                   OCPIRequest        Request,
                                                   CancellationToken  CancellationToken)

            => OnPutLocationRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) PutLocationResponse   (Response)

        /// <summary>
        /// An event sent whenever a PUT location response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnPutLocationResponse = new();

        /// <summary>
        /// An event sent whenever a PUT location response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PutLocationResponse(DateTimeOffset     Timestamp,
                                                    HTTPAPIX           API,
                                                    OCPIRequest        Request,
                                                    OCPIResponse       Response,
                                                    CancellationToken  CancellationToken)

            => OnPutLocationResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion


        #region (protected internal) PatchLocationRequest  (Request)

        /// <summary>
        /// An event sent whenever a PATCH location request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPatchLocationRequest = new();

        /// <summary>
        /// An event sent whenever a PATCH location request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PatchLocationRequest(DateTimeOffset     Timestamp,
                                                     HTTPAPIX           API,
                                                     OCPIRequest        Request,
                                                     CancellationToken  CancellationToken)

            => OnPatchLocationRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) PatchLocationResponse (Response)

        /// <summary>
        /// An event sent whenever a PATCH location response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnPatchLocationResponse = new();

        /// <summary>
        /// An event sent whenever a PATCH location response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PatchLocationResponse(DateTimeOffset     Timestamp,
                                                      HTTPAPIX           API,
                                                      OCPIRequest        Request,
                                                      OCPIResponse       Response,
                                                      CancellationToken  CancellationToken)

            => OnPatchLocationResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion


        #region (protected internal) DeleteLocationRequest (Request)

        /// <summary>
        /// An event sent whenever a DELETE location request was received.
        /// </summary>
        public OCPIRequestLogEvent OnDeleteLocationRequest = new();

        /// <summary>
        /// An event sent whenever a DELETE location request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task DeleteLocationRequest(DateTimeOffset     Timestamp,
                                                      HTTPAPIX           API,
                                                      OCPIRequest        Request,
                                                      CancellationToken  CancellationToken)

            => OnDeleteLocationRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) DeleteLocationResponse(Response)

        /// <summary>
        /// An event sent whenever a DELETE location response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnDeleteLocationResponse = new();

        /// <summary>
        /// An event sent whenever a DELETE location response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task DeleteLocationResponse(DateTimeOffset     Timestamp,
                                                       HTTPAPIX           API,
                                                       OCPIRequest        Request,
                                                       OCPIResponse       Response,
                                                       CancellationToken  CancellationToken)

            => OnDeleteLocationResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion

        #endregion

        #region EVSE/EVSE status

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


        #region (protected internal) PutEVSERequest    (Request)

        /// <summary>
        /// An event sent whenever a PUT EVSE request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPutEVSERequest = new();

        /// <summary>
        /// An event sent whenever a PUT EVSE request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PutEVSERequest(DateTimeOffset     Timestamp,
                                               HTTPAPIX           API,
                                               OCPIRequest        Request,
                                               CancellationToken  CancellationToken)

            => OnPutEVSERequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) PutEVSEResponse   (Response)

        /// <summary>
        /// An event sent whenever a PUT EVSE response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnPutEVSEResponse = new();

        /// <summary>
        /// An event sent whenever a PUT EVSE response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PutEVSEResponse(DateTimeOffset     Timestamp,
                                                HTTPAPIX           API,
                                                OCPIRequest        Request,
                                                OCPIResponse       Response,
                                                CancellationToken  CancellationToken)

            => OnPutEVSEResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion


        #region (protected internal) PatchEVSERequest  (Request)

        /// <summary>
        /// An event sent whenever a PATCH EVSE request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPatchEVSERequest = new();

        /// <summary>
        /// An event sent whenever a PATCH EVSE request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PatchEVSERequest(DateTimeOffset     Timestamp,
                                                 HTTPAPIX           API,
                                                 OCPIRequest        Request,
                                                 CancellationToken  CancellationToken)

            => OnPatchEVSERequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) PatchEVSEResponse (Response)

        /// <summary>
        /// An event sent whenever a PATCH EVSE response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnPatchEVSEResponse = new();

        /// <summary>
        /// An event sent whenever a PATCH EVSE response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PatchEVSEResponse(DateTimeOffset     Timestamp,
                                                  HTTPAPIX           API,
                                                  OCPIRequest        Request,
                                                  OCPIResponse       Response,
                                                  CancellationToken  CancellationToken)

            => OnPatchEVSEResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion


        #region (protected internal) DeleteEVSERequest (Request)

        /// <summary>
        /// An event sent whenever a DELETE EVSE request was received.
        /// </summary>
        public OCPIRequestLogEvent OnDeleteEVSERequest = new();

        /// <summary>
        /// An event sent whenever a DELETE EVSE request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task DeleteEVSERequest(DateTimeOffset     Timestamp,
                                                  HTTPAPIX           API,
                                                  OCPIRequest        Request,
                                                  CancellationToken  CancellationToken)

            => OnDeleteEVSERequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) DeleteEVSEResponse(Response)

        /// <summary>
        /// An event sent whenever a DELETE EVSE response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnDeleteEVSEResponse = new();

        /// <summary>
        /// An event sent whenever a DELETE EVSE response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task DeleteEVSEResponse(DateTimeOffset     Timestamp,
                                                   HTTPAPIX           API,
                                                   OCPIRequest        Request,
                                                   OCPIResponse       Response,
                                                   CancellationToken  CancellationToken)

            => OnDeleteEVSEResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion



        #region (protected internal) OnPostEVSEStatusRequest    (Request)

        /// <summary>
        /// An event sent whenever a POST EVSE status request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPostEVSEStatusRequest = new();

        /// <summary>
        /// An event sent whenever a POST EVSE status request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PostEVSEStatusRequest(DateTimeOffset     Timestamp,
                                                      HTTPAPIX           API,
                                                      OCPIRequest        Request,
                                                      CancellationToken  CancellationToken)

            => OnPostEVSEStatusRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) PostEVSEStatusResponse   (Response)

        /// <summary>
        /// An event sent whenever a POST EVSE status response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnPostEVSEStatusResponse = new();

        /// <summary>
        /// An event sent whenever a POST EVSE status response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PostEVSEStatusResponse(DateTimeOffset     Timestamp,
                                                       HTTPAPIX           API,
                                                       OCPIRequest        Request,
                                                       OCPIResponse       Response,
                                                       CancellationToken  CancellationToken)

            => OnPostEVSEStatusResponse.WhenAll(
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


        #region (protected internal) PutConnectorRequest    (Request)

        /// <summary>
        /// An event sent whenever a PUT connector request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPutConnectorRequest = new();

        /// <summary>
        /// An event sent whenever a PUT connector request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PutConnectorRequest(DateTimeOffset     Timestamp,
                                                    HTTPAPIX           API,
                                                    OCPIRequest        Request,
                                                    CancellationToken  CancellationToken)

            => OnPutConnectorRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) PutConnectorResponse   (Response)

        /// <summary>
        /// An event sent whenever a PUT connector response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnPutConnectorResponse = new();

        /// <summary>
        /// An event sent whenever a PUT connector response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PutConnectorResponse(DateTimeOffset     Timestamp,
                                                     HTTPAPIX           API,
                                                     OCPIRequest        Request,
                                                     OCPIResponse       Response,
                                                     CancellationToken  CancellationToken)

            => OnPutConnectorResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion


        #region (protected internal) PatchConnectorRequest  (Request)

        /// <summary>
        /// An event sent whenever a PATCH connector request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPatchConnectorRequest = new();

        /// <summary>
        /// An event sent whenever a PATCH connector request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PatchConnectorRequest(DateTimeOffset     Timestamp,
                                                      HTTPAPIX           API,
                                                      OCPIRequest        Request,
                                                      CancellationToken  CancellationToken)

            => OnPatchConnectorRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) PatchConnectorResponse (Response)

        /// <summary>
        /// An event sent whenever a PATCH connector response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnPatchConnectorResponse = new();

        /// <summary>
        /// An event sent whenever a PATCH connector response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PatchConnectorResponse(DateTimeOffset     Timestamp,
                                                       HTTPAPIX           API,
                                                       OCPIRequest        Request,
                                                       OCPIResponse       Response,
                                                       CancellationToken  CancellationToken)

            => OnPatchConnectorResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion


        #region (protected internal) DeleteConnectorRequest (Request)

        /// <summary>
        /// An event sent whenever a DELETE connector request was received.
        /// </summary>
        public OCPIRequestLogEvent OnDeleteConnectorRequest = new();

        /// <summary>
        /// An event sent whenever a DELETE connector request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task DeleteConnectorRequest(DateTimeOffset     Timestamp,
                                                       HTTPAPIX           API,
                                                       OCPIRequest        Request,
                                                       CancellationToken  CancellationToken)

            => OnDeleteConnectorRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) DeleteConnectorResponse(Response)

        /// <summary>
        /// An event sent whenever a DELETE connector response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnDeleteConnectorResponse = new();

        /// <summary>
        /// An event sent whenever a DELETE connector response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task DeleteConnectorResponse(DateTimeOffset     Timestamp,
                                                        HTTPAPIX           API,
                                                        OCPIRequest        Request,
                                                        OCPIResponse       Response,
                                                        CancellationToken  CancellationToken)

            => OnDeleteConnectorResponse.WhenAll(
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


        #region (protected internal) DeleteTariffsRequest (Request)

        /// <summary>
        /// An event sent whenever a DELETE tariffs request was received.
        /// </summary>
        public OCPIRequestLogEvent OnDeleteTariffsRequest = new();

        /// <summary>
        /// An event sent whenever a DELETE tariffs request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task DeleteTariffsRequest(DateTimeOffset     Timestamp,
                                                     HTTPAPIX           API,
                                                     OCPIRequest        Request,
                                                     CancellationToken  CancellationToken)

            => OnDeleteTariffsRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) DeleteTariffsResponse(Response)

        /// <summary>
        /// An event sent whenever a DELETE tariffs response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnDeleteTariffsResponse = new();

        /// <summary>
        /// An event sent whenever a DELETE tariffs response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task DeleteTariffsResponse(DateTimeOffset     Timestamp,
                                                      HTTPAPIX           API,
                                                      OCPIRequest        Request,
                                                      OCPIResponse       Response,
                                                      CancellationToken  CancellationToken)

            => OnDeleteTariffsResponse.WhenAll(
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


        #region (protected internal) PutTariffRequest    (Request)

        /// <summary>
        /// An event sent whenever a PUT tariff request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPutTariffRequest = new();

        /// <summary>
        /// An event sent whenever a PUT tariff request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PutTariffRequest(DateTimeOffset     Timestamp,
                                                 HTTPAPIX           API,
                                                 OCPIRequest        Request,
                                                 CancellationToken  CancellationToken)

            => OnPutTariffRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) PutTariffResponse   (Response)

        /// <summary>
        /// An event sent whenever a PUT tariff response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnPutTariffResponse = new();

        /// <summary>
        /// An event sent whenever a PUT tariff response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PutTariffResponse(DateTimeOffset     Timestamp,
                                                  HTTPAPIX           API,
                                                  OCPIRequest        Request,
                                                  OCPIResponse       Response,
                                                  CancellationToken  CancellationToken)

            => OnPutTariffResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion


        #region (protected internal) PatchTariffRequest    (Request)

        /// <summary>
        /// An event sent whenever a PATCH tariff request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPatchTariffRequest = new();

        /// <summary>
        /// An event sent whenever a PATCH tariff request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PatchTariffRequest(DateTimeOffset     Timestamp,
                                                   HTTPAPIX           API,
                                                   OCPIRequest        Request,
                                                   CancellationToken  CancellationToken)

            => OnPatchTariffRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) PatchTariffResponse   (Response)

        /// <summary>
        /// An event sent whenever a PATCH tariff response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnPatchTariffResponse = new();

        /// <summary>
        /// An event sent whenever a PATCH tariff response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PatchTariffResponse(DateTimeOffset     Timestamp,
                                                    HTTPAPIX           API,
                                                    OCPIRequest        Request,
                                                    OCPIResponse       Response,
                                                    CancellationToken  CancellationToken)

            => OnPatchTariffResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion


        #region (protected internal) DeleteTariffRequest (Request)

        /// <summary>
        /// An event sent whenever a DELETE tariff request was received.
        /// </summary>
        public OCPIRequestLogEvent OnDeleteTariffRequest = new();

        /// <summary>
        /// An event sent whenever a DELETE tariff request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task DeleteTariffRequest(DateTimeOffset     Timestamp,
                                                    HTTPAPIX           API,
                                                    OCPIRequest        Request,
                                                    CancellationToken  CancellationToken)

            => OnDeleteTariffRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) DeleteTariffResponse(Response)

        /// <summary>
        /// An event sent whenever a DELETE tariff response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnDeleteTariffResponse = new();

        /// <summary>
        /// An event sent whenever a DELETE tariff response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task DeleteTariffResponse(DateTimeOffset     Timestamp,
                                                     HTTPAPIX           API,
                                                     OCPIRequest        Request,
                                                     OCPIResponse       Response,
                                                     CancellationToken  CancellationToken)

            => OnDeleteTariffResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion

        #endregion

        #region Sessions

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


        #region (protected internal) DeleteSessionsRequest (Request)

        /// <summary>
        /// An event sent whenever a DELETE sessions request was received.
        /// </summary>
        public OCPIRequestLogEvent OnDeleteSessionsRequest = new();

        /// <summary>
        /// An event sent whenever a DELETE sessions request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task DeleteSessionsRequest(DateTimeOffset     Timestamp,
                                                      HTTPAPIX           API,
                                                      OCPIRequest        Request,
                                                      CancellationToken  CancellationToken)

            => OnDeleteSessionsRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) DeleteSessionsResponse(Response)

        /// <summary>
        /// An event sent whenever a DELETE sessions response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnDeleteSessionsResponse = new();

        /// <summary>
        /// An event sent whenever a DELETE sessions response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task DeleteSessionsResponse(DateTimeOffset     Timestamp,
                                                       HTTPAPIX           API,
                                                       OCPIRequest        Request,
                                                       OCPIResponse       Response,
                                                       CancellationToken  CancellationToken)

            => OnDeleteSessionsResponse.WhenAll(
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


        #region (protected internal) PutSessionRequest    (Request)

        /// <summary>
        /// An event sent whenever a PUT session request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPutSessionRequest = new();

        /// <summary>
        /// An event sent whenever a PUT session request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PutSessionRequest(DateTimeOffset     Timestamp,
                                                  HTTPAPIX           API,
                                                  OCPIRequest        Request,
                                                  CancellationToken  CancellationToken)

            => OnPutSessionRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) PutSessionResponse   (Response)

        /// <summary>
        /// An event sent whenever a PUT session response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnPutSessionResponse = new();

        /// <summary>
        /// An event sent whenever a PUT session response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PutSessionResponse(DateTimeOffset     Timestamp,
                                                   HTTPAPIX           API,
                                                   OCPIRequest        Request,
                                                   OCPIResponse       Response,
                                                   CancellationToken  CancellationToken)

            => OnPutSessionResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion


        #region (protected internal) PatchSessionRequest  (Request)

        /// <summary>
        /// An event sent whenever a PATCH session request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPatchSessionRequest = new();

        /// <summary>
        /// An event sent whenever a PATCH session request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PatchSessionRequest(DateTimeOffset     Timestamp,
                                                    HTTPAPIX           API,
                                                    OCPIRequest        Request,
                                                    CancellationToken  CancellationToken)

            => OnPatchSessionRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) PatchSessionResponse (Response)

        /// <summary>
        /// An event sent whenever a PATCH session response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnPatchSessionResponse = new();

        /// <summary>
        /// An event sent whenever a PATCH session response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PatchSessionResponse(DateTimeOffset     Timestamp,
                                                     HTTPAPIX           API,
                                                     OCPIRequest        Request,
                                                     OCPIResponse       Response,
                                                     CancellationToken  CancellationToken)

            => OnPatchSessionResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion


        #region (protected internal) DeleteSessionRequest (Request)

        /// <summary>
        /// An event sent whenever a DELETE session request was received.
        /// </summary>
        public OCPIRequestLogEvent OnDeleteSessionRequest = new();

        /// <summary>
        /// An event sent whenever a DELETE session request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task DeleteSessionRequest(DateTimeOffset     Timestamp,
                                                     HTTPAPIX           API,
                                                     OCPIRequest        Request,
                                                     CancellationToken  CancellationToken)

            => OnDeleteSessionRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) DeleteSessionResponse(Response)

        /// <summary>
        /// An event sent whenever a DELETE session response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnDeleteSessionResponse = new();

        /// <summary>
        /// An event sent whenever a DELETE session response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task DeleteSessionResponse(DateTimeOffset     Timestamp,
                                                      HTTPAPIX           API,
                                                      OCPIRequest        Request,
                                                      OCPIResponse       Response,
                                                      CancellationToken  CancellationToken)

            => OnDeleteSessionResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion

        #endregion

        #region CDRs

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


        #region (protected internal) DeleteCDRsRequest (Request)

        /// <summary>
        /// An event sent whenever a DELETE CDRs request was received.
        /// </summary>
        public OCPIRequestLogEvent OnDeleteCDRsRequest = new();

        /// <summary>
        /// An event sent whenever a DELETE CDRs request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task DeleteCDRsRequest(DateTimeOffset     Timestamp,
                                                  HTTPAPIX           API,
                                                  OCPIRequest        Request,
                                                  CancellationToken  CancellationToken)

            => OnDeleteCDRsRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) DeleteCDRsResponse(Response)

        /// <summary>
        /// An event sent whenever a DELETE CDRs response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnDeleteCDRsResponse = new();

        /// <summary>
        /// An event sent whenever a DELETE CDRs response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task DeleteCDRsResponse(DateTimeOffset     Timestamp,
                                                   HTTPAPIX           API,
                                                   OCPIRequest        Request,
                                                   OCPIResponse       Response,
                                                   CancellationToken  CancellationToken)

            => OnDeleteCDRsResponse.WhenAll(
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


        #region (protected internal) PostCDRRequest (Request)

        /// <summary>
        /// An event sent whenever a POST CDR request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPostCDRRequest = new();

        /// <summary>
        /// An event sent whenever a POST CDR request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PostCDRRequest(DateTimeOffset     Timestamp,
                                               HTTPAPIX           API,
                                               OCPIRequest        Request,
                                               CancellationToken  CancellationToken)

            => OnPostCDRRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) PostCDRResponse(Response)

        /// <summary>
        /// An event sent whenever a POST CDR response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnPostCDRResponse = new();

        /// <summary>
        /// An event sent whenever a POST CDR response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PostCDRResponse(DateTimeOffset     Timestamp,
                                                HTTPAPIX           API,
                                                OCPIRequest        Request,
                                                OCPIResponse       Response,
                                                CancellationToken  CancellationToken)

            => OnPostCDRResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion


        #region (protected internal) DeleteCDRRequest (Request)

        /// <summary>
        /// An event sent whenever a DELETE CDR request was received.
        /// </summary>
        public OCPIRequestLogEvent OnDeleteCDRRequest = new();

        /// <summary>
        /// An event sent whenever a DELETE CDR request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task DeleteCDRRequest(DateTimeOffset     Timestamp,
                                                 HTTPAPIX           API,
                                                 OCPIRequest        Request,
                                                 CancellationToken  CancellationToken)

            => OnDeleteCDRRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) DeleteCDRResponse(Response)

        /// <summary>
        /// An event sent whenever a DELETE CDR response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnDeleteCDRResponse = new();

        /// <summary>
        /// An event sent whenever a DELETE CDR response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task DeleteCDRResponse(DateTimeOffset     Timestamp,
                                                  HTTPAPIX           API,
                                                  OCPIRequest        Request,
                                                  OCPIResponse       Response,
                                                  CancellationToken  CancellationToken)

            => OnDeleteCDRResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion

        #endregion

        #region Tokens

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

        #endregion


        public delegate Task<AuthorizationInfo> OnRFIDAuthTokenDelegate(CountryCode         From_CountryCode,
                                                                        Party_Id            From_PartyId,
                                                                        CountryCode         To_CountryCode,
                                                                        Party_Id            To_PartyId,
                                                                        Token_Id            TokenId,
                                                                        LocationReference?  LocationReference);

        public event OnRFIDAuthTokenDelegate? OnRFIDAuthToken;



        // Command callbacks

        #region (protected internal) ReserveNowCallbackRequest        (Request)

        /// <summary>
        /// An event sent whenever a reserve now callback was received.
        /// </summary>
        public OCPIRequestLogEvent OnReserveNowCallbackRequest = new();

        /// <summary>
        /// An event sent whenever a reserve now callback was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the callback request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task ReserveNowCallbackRequest(DateTimeOffset     Timestamp,
                                                          HTTPAPIX           API,
                                                          OCPIRequest        Request,
                                                          CancellationToken  CancellationToken)

            => OnReserveNowCallbackRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) ReserveNowCallbackResponse       (Response)

        /// <summary>
        /// An event sent whenever a reserve now callback response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnReserveNowCallbackResponse = new();

        /// <summary>
        /// An event sent whenever a reserve now callback response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the callback response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task ReserveNowCallbackResponse(DateTimeOffset     Timestamp,
                                                           HTTPAPIX           API,
                                                           OCPIRequest        Request,
                                                           OCPIResponse       Response,
                                                           CancellationToken  CancellationToken)

            => OnReserveNowCallbackResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion


        #region (protected internal) CancelReservationCallbackRequest (Request)

        /// <summary>
        /// An event sent whenever a cancel reservation callback was received.
        /// </summary>
        public OCPIRequestLogEvent OnCancelReservationCallbackRequest = new();

        /// <summary>
        /// An event sent whenever a cancel reservation callback was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the callback request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task CancelReservationCallbackRequest(DateTimeOffset     Timestamp,
                                                                 HTTPAPIX           API,
                                                                 OCPIRequest        Request,
                                                                 CancellationToken  CancellationToken)

            => OnCancelReservationCallbackRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) CancelReservationCallbackResponse(Response)

        /// <summary>
        /// An event sent whenever a cancel reservation callback response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnCancelReservationCallbackResponse = new();

        /// <summary>
        /// An event sent whenever a cancel reservation callback response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the callback response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task CancelReservationCallbackResponse(DateTimeOffset     Timestamp,
                                                                  HTTPAPIX           API,
                                                                  OCPIRequest        Request,
                                                                  OCPIResponse       Response,
                                                                  CancellationToken  CancellationToken)

            => OnCancelReservationCallbackResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion


        #region (protected internal) StartSessionCallbackRequest      (Request)

        /// <summary>
        /// An event sent whenever a start session callback was received.
        /// </summary>
        public OCPIRequestLogEvent OnStartSessionCallbackRequest = new();

        /// <summary>
        /// An event sent whenever a start session callback was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the callback request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task StartSessionCallbackRequest(DateTimeOffset     Timestamp,
                                                            HTTPAPIX           API,
                                                            OCPIRequest        Request,
                                                            CancellationToken  CancellationToken)

            => OnStartSessionCallbackRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) StartSessionCallbackResponse     (Response)

        /// <summary>
        /// An event sent whenever a start session callback response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnStartSessionCallbackResponse = new();

        /// <summary>
        /// An event sent whenever a start session callback response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the callback response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task StartSessionCallbackResponse(DateTimeOffset     Timestamp,
                                                             HTTPAPIX           API,
                                                             OCPIRequest        Request,
                                                             OCPIResponse       Response,
                                                             CancellationToken  CancellationToken)

            => OnStartSessionCallbackResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion


        #region (protected internal) StopSessionCallbackRequest       (Request)

        /// <summary>
        /// An event sent whenever a stop session callback was received.
        /// </summary>
        public OCPIRequestLogEvent OnStopSessionCallbackRequest = new();

        /// <summary>
        /// An event sent whenever a stop session callback was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the callback request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task StopSessionCallbackRequest(DateTimeOffset     Timestamp,
                                                           HTTPAPIX           API,
                                                           OCPIRequest        Request,
                                                           CancellationToken  CancellationToken)

            => OnStopSessionCallbackRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) StopSessionCallbackResponse      (Response)

        /// <summary>
        /// An event sent whenever a stop session callback response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnStopSessionCallbackResponse = new();

        /// <summary>
        /// An event sent whenever a stop session callback response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the callback response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task StopSessionCallbackResponse(DateTimeOffset     Timestamp,
                                                            HTTPAPIX           API,
                                                            OCPIRequest        Request,
                                                            OCPIResponse       Response,
                                                            CancellationToken  CancellationToken)

            => OnStopSessionCallbackResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion


        #region (protected internal) UnlockConnectorCallbackRequest   (Request)

        /// <summary>
        /// An event sent whenever a unlock connector callback was received.
        /// </summary>
        public OCPIRequestLogEvent OnUnlockConnectorCallbackRequest = new();

        /// <summary>
        /// An event sent whenever a unlock connector callback was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the callback request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task UnlockConnectorCallbackRequest(DateTimeOffset     Timestamp,
                                                               HTTPAPIX           API,
                                                               OCPIRequest        Request,
                                                               CancellationToken  CancellationToken)

            => OnUnlockConnectorCallbackRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) UnlockConnectorCallbackResponse  (Response)

        /// <summary>
        /// An event sent whenever a unlock connector callback response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnUnlockConnectorCallbackResponse = new();

        /// <summary>
        /// An event sent whenever a unlock connector callback response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the callback response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task UnlockConnectorCallbackResponse(DateTimeOffset     Timestamp,
                                                                HTTPAPIX           API,
                                                                OCPIRequest        Request,
                                                                OCPIResponse       Response,
                                                                CancellationToken  CancellationToken)

            => OnUnlockConnectorCallbackResponse.WhenAll(
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
        /// Create an instance of the HTTP API for e-mobility service providers
        /// using the given CommonAPI.
        /// </summary>
        /// <param name="CommonAPI">The OCPI common API.</param>
        /// <param name="AllowDowngrades">(Dis-)allow PUTting of object having an earlier 'LastUpdated'-timestamp then already existing objects.</param>
        /// 
        /// <param name="ExternalDNSName">The official URL/DNS name of this service, e.g. for sending e-mails.</param>
        /// <param name="URLPathPrefix">An optional URL path prefix.</param>
        /// <param name="BasePath">When the API is served from an optional subdirectory path.</param>
        /// <param name="HTTPServiceName">An optional name of the HTTP API service.</param>
        public EMSPAPI(CommonAPI                    CommonAPI,
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

                   Description     ?? I18NString.Create($"OCPI{Version.String} EMSP API"),

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
            this.RequestTimeout   = TimeSpan.FromSeconds(30);

            this.Logger           = this.DisableLogging == false
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


        #region EMSP-2-CPO Clients

        private readonly ConcurrentDictionary<CPO_Id, EMSP2CPOClient> emsp2cpoClients = new();

        /// <summary>
        /// Return an enumeration of all EMSP2CPO clients.
        /// </summary>
        public IEnumerable<EMSP2CPOClient> EMSP2CPOClients
            => emsp2cpoClients.Values;


        #region GetCPOClient (CountryCode, PartyId,   Description = null, AllowCachedClients = true)

        /// <summary>
        /// As a EMSP create a client to access e.g. a remote CPO.
        /// </summary>
        /// <param name="CountryCode">The country code of the remote CPO.</param>
        /// <param name="PartyId">The party identification of the remote CPO.</param>
        /// <param name="Description">A description for the OCPI client.</param>
        /// <param name="AllowCachedClients">Whether to allow to return cached CPO clients.</param>
        public EMSP2CPOClient? GetCPOClient(CountryCode  CountryCode,
                                            Party_Id     PartyId,
                                            I18NString?  Description          = null,
                                            Boolean      AllowCachedClients   = true)

            => GetCPOClient(
                   RemoteParty_Id.From(
                       CountryCode,
                       PartyId,
                       Role.CPO
                   ),
                   Description,
                   AllowCachedClients
               );

        #endregion

        #region GetCPOClient (             PartyIdv3, Description = null, AllowCachedClients = true)

        /// <summary>
        /// As a EMSP create a client to access e.g. a remote CPO.
        /// </summary>
        /// <param name="PartyId">The party identification of the remote CPO.</param>
        /// <param name="Description">A description for the OCPI client.</param>
        /// <param name="AllowCachedClients">Whether to allow to return cached CPO clients.</param>
        public EMSP2CPOClient? GetCPOClient(Party_Idv3   PartyId,
                                            I18NString?  Description          = null,
                                            Boolean      AllowCachedClients   = true)

            => GetCPOClient(
                   RemoteParty_Id.From(
                       PartyId,
                       Role.CPO
                   ),
                   Description,
                   AllowCachedClients
               );

        #endregion

        #region GetCPOClient (RemoteParty,            Description = null, AllowCachedClients = true)

        /// <summary>
        /// As a EMSP create a client to access e.g. a remote CPO.
        /// </summary>
        /// <param name="RemoteParty">A remote party.</param>
        /// <param name="Description">A description for the OCPI client.</param>
        /// <param name="AllowCachedClients">Whether to allow to return cached EMSP clients.</param>
        public EMSP2CPOClient? GetCPOClient(RemoteParty  RemoteParty,
                                            I18NString?  Description          = null,
                                            Boolean      AllowCachedClients   = true)
        {

            var cpoId = CPO_Id.From(RemoteParty.Id);

            if (AllowCachedClients &&
                emsp2cpoClients.TryGetValue(cpoId, out var cachedEMSPClient))
            {
                return cachedEMSPClient;
            }

            if (RemoteParty?.RemoteAccessInfos?.Any() == true)
            {

                var emsp2CPOClient = new EMSP2CPOClient(
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

                emsp2cpoClients.TryAdd(cpoId, emsp2CPOClient);

                return emsp2CPOClient;

            }

            return null;

        }

        #endregion

        #region GetCPOClient (RemotePartyId,          Description = null, AllowCachedClients = true)

        /// <summary>
        /// As a EMSP create a client to access e.g. a remote CPO.
        /// </summary>
        /// <param name="RemotePartyId">A remote party identification.</param>
        /// <param name="Description">A description for the OCPI client.</param>
        /// <param name="AllowCachedClients">Whether to allow to return cached EMSP clients.</param>
        public EMSP2CPOClient? GetCPOClient(RemoteParty_Id  RemotePartyId,
                                            I18NString?     Description          = null,
                                            Boolean         AllowCachedClients   = true)
        {

            var cpoId = CPO_Id.From(RemotePartyId);

            if (AllowCachedClients &&
                emsp2cpoClients.TryGetValue(cpoId, out var cachedEMSPClient))
            {
                return cachedEMSPClient;
            }

            if (CommonAPI.TryGetRemoteParty(RemotePartyId, out var remoteParty) &&
                remoteParty?.RemoteAccessInfos?.Any() == true)
            {

                var emsp2CPOClient = new EMSP2CPOClient(
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

                emsp2cpoClients.TryAdd(cpoId, emsp2CPOClient);

                return emsp2CPOClient;

            }

            return null;

        }

        #endregion

        #endregion

        #region CloseAllClients()

        public Task CloseAllClients()
        {

            foreach (var client in emsp2cpoClients.Values)
            {
                client.Close();
            }

            emsp2cpoClients.Clear();

            return Task.CompletedTask;

        }

        #endregion


        #region (private) RegisterURLTemplates()

        private void RegisterURLTemplates()
        {

            // Receiver Interface for eMSPs and NSPs

            #region ~/locations/{country_code}/{party_id}                               [NonStandard]

            #region OPTIONS  ~/locations/{country_code}/{party_id}      [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPMethod.OPTIONS,
                URLPathPrefix + "locations/{country_code}/{party_id}",
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

            #region GET      ~/locations/{country_code}/{party_id}      [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPMethod.GET,
                URLPathPrefix + "locations/{country_code}/{party_id}",
                GetLocationsRequest,
                GetLocationsResponse,
                request => {

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
                                                              Select(location => location.ToJSON(
                                                                                     request.EMSPId,
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

                }
            );

            #endregion

            #region DELETE   ~/locations/{country_code}/{party_id}      [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPMethod.DELETE,
                URLPathPrefix + "locations/{country_code}/{party_id}",
                DeleteLocationsRequest,
                DeleteLocationsResponse,
                async request => {

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
                    var result = await CommonAPI.RemoveAllLocations(partyId.Value);


                    return new OCPIResponse.Builder(request) {
                               StatusCode           = 1000,
                               StatusMessage        = "Hello world!",
                               HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET", "DELETE" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ]
                               }
                           };

                }
            );

            #endregion

            #endregion

            #region ~/locations/{country_code}/{party_id}/{locationId}

            #region OPTIONS  ~/locations/{country_code}/{party_id}/{locationId}      [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPMethod.OPTIONS,
                URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}",
                request =>

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

            CommonAPI.AddOCPIMethod(

                HTTPMethod.GET,
                URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}",
                GetLocationRequest,
                GetLocationResponse,
                request => {

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
                               Data                 = location.ToJSON(
                                                          request.EMSPId,
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
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ],
                                   LastModified               = location.LastUpdated,
                                   ETag                       = location.ETag
                               }
                        });

                }
            );

            #endregion

            #region PUT      ~/locations/{country_code}/{party_id}/{locationId}

            CommonAPI.AddOCPIMethod(

                HTTPMethod.PUT,
                URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}",
                PutLocationRequest,
                PutLocationResponse,
                async request => {

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


                    if (addOrUpdateResult.IsSuccessAndDataNotNull(out var locationData))
                    {

                        return new OCPIResponse.Builder(request) {
                                   StatusCode           = 1000,
                                   StatusMessage        = "Hello world!",
                                   Data                 = locationData.ToJSON(
                                                              request.EMSPId,
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
                                       HTTPStatusCode             = addOrUpdateResult.WasCreated == true
                                                                        ? HTTPStatusCode.Created
                                                                        : HTTPStatusCode.OK,
                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ],
                                       LastModified               = locationData.LastUpdated,
                                       ETag                       = locationData.ETag
                                   }
                               };

                    }

                    return new OCPIResponse.Builder(request) {
                               StatusCode           = 2000,
                               StatusMessage        = addOrUpdateResult.ErrorResponse,
                               Data                 = newOrUpdatedLocation.ToJSON(
                                                          request.EMSPId,
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
                                   HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ]
                               }
                           };

                }
            );

            #endregion

            #region PATCH    ~/locations/{country_code}/{party_id}/{locationId}

            CommonAPI.AddOCPIMethod(

                HTTPMethod.PATCH,
                URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}",
                PatchLocationRequest,
                PatchLocationResponse,
                async request => {

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
                    if (patchedLocation.IsSuccessAndDataNotNull(out var locationData))
                        return new OCPIResponse.Builder(request) {
                                       StatusCode           = 1000,
                                       StatusMessage        = "Hello world!",
                                       Data                 = locationData.ToJSON(
                                                                  request.EMSPId,
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
                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                           AccessControlAllowHeaders  = [ "Authorization" ],
                                           LastModified               = locationData.LastUpdated,
                                           ETag                       = locationData.ETag
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

                }
            );

            #endregion

            #region DELETE   ~/locations/{country_code}/{party_id}/{locationId}      [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPMethod.DELETE,
                URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}",
                DeleteLocationRequest,
                DeleteLocationResponse,
                async request => {

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
                    var result = await CommonAPI.RemoveLocation(location);


                    return new OCPIResponse.Builder(request) {
                                   StatusCode           = 1000,
                                   StatusMessage        = "Hello world!",
                                   Data                 = location.ToJSON(
                                                              request.EMSPId,
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
                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ],
                                       LastModified               = location.LastUpdated,
                                       ETag                       = location.ETag
                                   }
                               };

                }
            );

            #endregion

            #endregion

            #region ~/locations/{country_code}/{party_id}/{locationId}/{evseUId}

            #region OPTIONS  ~/locations/{country_code}/{party_id}/{locationId}/{evseUId}      [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPMethod.OPTIONS,
                URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}/{evseUId}",
                request =>

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

            #region GET      ~/locations/{country_code}/{party_id}/{locationId}/{evseUId}

            CommonAPI.AddOCPIMethod(

                HTTPMethod.GET,
                URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}/{evseUId}",
                GetEVSERequest,
                GetEVSEResponse,
                request => {

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
                               Data                 = evse.ToJSON(
                                                          request.EMSPId,
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
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ],
                                   LastModified               = evse.LastUpdated,
                                   ETag                       = evse.ETag
                               }
                        });

                }
            );

            #endregion

            #region PUT      ~/locations/{country_code}/{party_id}/{locationId}/{evseUId}

            CommonAPI.AddOCPIMethod(

                HTTPMethod.PUT,
                URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}/{evseUId}",
                PutEVSERequest,
                PutEVSEResponse,
                async request => {

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


                    if (addOrUpdateResult.IsSuccessAndDataNotNull(out var evseData))
                        return new OCPIResponse.Builder(request) {
                                   StatusCode           = 1000,
                                   StatusMessage        = "Hello world!",
                                   Data                 = evseData.ToJSON(
                                                              request.EMSPId,
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
                                       HTTPStatusCode             = addOrUpdateResult.WasCreated == true
                                                                        ? HTTPStatusCode.Created
                                                                        : HTTPStatusCode.OK,
                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ],
                                       LastModified               = evseData.LastUpdated,
                                       ETag                       = evseData.ETag
                                   }
                               };

                    return new OCPIResponse.Builder(request) {
                               StatusCode           = 2000,
                               StatusMessage        = addOrUpdateResult.ErrorResponse,
                               Data                 = newOrUpdatedEVSE.ToJSON(
                                                          request.EMSPId,
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
                                   HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ],
                                   LastModified               = newOrUpdatedEVSE.LastUpdated,
                                   ETag                       = newOrUpdatedEVSE.ETag
                               }
                           };

                });

            #endregion

            #region PATCH    ~/locations/{country_code}/{party_id}/{locationId}/{evseUId}

            CommonAPI.AddOCPIMethod(

                HTTPMethod.PATCH,
                URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}/{evseUId}",
                PatchEVSERequest,
                PatchEVSEResponse,
                async request => {

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
                    if (patchedEVSE.IsSuccessAndDataNotNull(out var evseData))
                        return new OCPIResponse.Builder(request) {
                                       StatusCode           = 1000,
                                       StatusMessage        = "Hello world!",
                                       Data                 = evseData.ToJSON(
                                                                  request.EMSPId,
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
                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                           AccessControlAllowHeaders  = [ "Authorization" ],
                                           LastModified               = evseData.LastUpdated,
                                           ETag                       = evseData.ETag
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

                }
            );

            #endregion

            #region DELETE   ~/locations/{country_code}/{party_id}/{locationId}/{evseUId}      [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPMethod.DELETE,
                URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}/{evseUId}",
                DeleteEVSERequest,
                DeleteEVSEResponse,
                async request => {

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
                                   Data                 = existingEVSE.ToJSON(
                                                              request.EMSPId,
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
                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ],
                                       LastModified               = existingEVSE.LastUpdated,
                                       ETag                       = existingEVSE.ETag
                                   }
                               };

                }
            );

            #endregion

            #endregion

            #region ~/locations/{country_code}/{party_id}/{locationId}/{evseUId}/{connectorId}

            #region OPTIONS  ~/locations/{country_code}/{party_id}/{locationId}/{evseUId}/{connectorId}      [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPMethod.OPTIONS,
                URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}/{evseUId}/{connectorId}",
                request =>

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

            #region GET      ~/locations/{country_code}/{party_id}/{locationId}/{evseUId}/{connectorId}

            CommonAPI.AddOCPIMethod(

                HTTPMethod.GET,
                URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}/{evseUId}/{connectorId}",
                GetConnectorRequest,
                GetConnectorResponse,
                request => {

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
                               Data                 = connector.ToJSON(
                                                          request.EMSPId,
                                                          CustomConnectorSerializer
                                                      ),
                               HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ],
                                   LastModified               = connector.LastUpdated,
                                   ETag                       = connector.ETag
                               }
                        });

                }
            );

            #endregion

            #region PUT      ~/locations/{country_code}/{party_id}/{locationId}/{evseUId}/{connectorId}

            CommonAPI.AddOCPIMethod(

                HTTPMethod.PUT,
                URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}/{evseUId}/{connectorId}",
                PutConnectorRequest,
                PutConnectorResponse,
                async request => {

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


                    if (addOrUpdateResult.IsSuccessAndDataNotNull(out var connectorData))
                        return new OCPIResponse.Builder(request) {
                                   StatusCode           = 1000,
                                   StatusMessage        = "Hello world!",
                                   Data                 = connectorData.ToJSON(
                                                              request.EMSPId,
                                                              CustomConnectorSerializer
                                                          ),
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
                               Data                 = newOrUpdatedConnector.ToJSON(
                                                          request.EMSPId,
                                                          CustomConnectorSerializer
                                                      ),
                               HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ],
                                   LastModified               = newOrUpdatedConnector.LastUpdated,
                                   ETag                       = newOrUpdatedConnector.ETag
                               }
                           };

                }
            );

            #endregion

            #region PATCH    ~/locations/{country_code}/{party_id}/{locationId}/{evseUId}/{connectorId}

            CommonAPI.AddOCPIMethod(

                HTTPMethod.PATCH,
                URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}/{evseUId}/{connectorId}",
                PatchConnectorRequest,
                PatchConnectorResponse,
                async request => {

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
                    if (patchedConnector.IsSuccessAndDataNotNull(out var connectorData))
                        return new OCPIResponse.Builder(request) {
                                       StatusCode           = 1000,
                                       StatusMessage        = "Hello world!",
                                       Data                 = connectorData.ToJSON(
                                                                  request.EMSPId,
                                                                  CustomConnectorSerializer
                                                              ),
                                       HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                           HTTPStatusCode             = HTTPStatusCode.OK,
                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                           AccessControlAllowHeaders  = [ "Authorization" ],
                                           LastModified               = connectorData.LastUpdated,
                                           ETag                       = connectorData.ETag
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

                }
            );

            #endregion

            #region DELETE   ~/locations/{country_code}/{party_id}/{locationId}/{evseUId}/{connectorId}      [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPMethod.DELETE,
                URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}/{evseUId}/{connectorId}",
                DeleteConnectorRequest,
                DeleteConnectorResponse,
                async request => {

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

                }
            );

            #endregion

            #endregion


            #region ~/locations/{country_code}/{party_id}/{locationId}/{evseUId}/status  [NonStandard]

            #region OPTIONS  ~/locations/{country_code}/{party_id}/{locationId}/{evseUId}/status     [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPMethod.OPTIONS,
                URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}/{evseUId}/status",
                request =>

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

            #region POST     ~/locations/{country_code}/{party_id}/{locationId}/{evseUId}/status     [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPMethod.POST,
                URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}/{evseUId}/status",
                PutEVSERequest,
                PutEVSEResponse,
                async request => {

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

                }
            );

            #endregion

            #endregion



            #region ~/tariffs/{country_code}/{party_id}                                 [NonStandard]

            #region OPTIONS  ~/tariffs/{country_code}/{party_id}            [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPMethod.OPTIONS,
                URLPathPrefix + "tariffs/{country_code}/{party_id}",
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

            #region GET      ~/tariffs/{country_code}/{party_id}            [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPMethod.GET,
                URLPathPrefix + "tariffs/{country_code}/{party_id}",
                GetTariffsRequest,
                GetTariffsResponse,
                request => {

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


                    var filters          = request.GetDateAndPaginationFilters();

                    var allTariffs       = CommonAPI.GetTariffs(partyId.Value).ToArray();

                    var filteredTariffs  = allTariffs.Where(tariff => !filters.From.HasValue || tariff.LastUpdated >  filters.From.Value).
                                                      Where(tariff => !filters.To.  HasValue || tariff.LastUpdated <= filters.To.  Value).
                                                      ToArray();


                    return Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               StatusCode           = 1000,
                               StatusMessage        = "Hello world!",
                               Data                 = new JArray(
                                                          filteredTariffs.
                                                              SkipTakeFilter(filters.Offset,
                                                                             filters.Limit).
                                                              Select(tariff => tariff.ToJSON(
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
                                                      ),
                               HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET", "DELETE" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ]
                                   //LastModified               = ?
                               }.
                               Set("X-Total-Count", allTariffs.Length)
                               // X-Limit               The maximum number of objects that the server WILL return.
                               // Link                  Link to the 'next' page should be provided when this is NOT the last page.
                        });

                }
            );

            #endregion

            #region DELETE   ~/tariffs/{country_code}/{party_id}            [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPMethod.DELETE,
                URLPathPrefix + "tariffs/{country_code}/{party_id}",
                DeleteTariffsRequest,
                DeleteTariffsResponse,
                async request => {

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


                    var result = await CommonAPI.RemoveAllTariffs(partyId.Value);


                    return new OCPIResponse.Builder(request) {
                               StatusCode           = 1000,
                               StatusMessage        = "Hello world!",
                               HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET", "DELETE" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ]
                               }
                           };

                }
            );

            #endregion

            #endregion

            #region ~/tariffs/{country_code}/{party_id}/{tariffId}

            #region OPTIONS  ~/tariffs/{country_code}/{party_id}/{tariffId}

            CommonAPI.AddOCPIMethod(

                HTTPMethod.OPTIONS,
                URLPathPrefix + "tariffs/{country_code}/{party_id}/{tariffId}",
                request =>

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

            #region GET      ~/tariffs/{country_code}/{party_id}/{tariffId}

            CommonAPI.AddOCPIMethod(

                HTTPMethod.GET,
                URLPathPrefix + "tariffs/{country_code}/{party_id}/{tariffId}",
                GetTariffRequest,
                GetTariffResponse,
                request => {

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
                                    AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "PUT", "DELETE" },
                                    AccessControlAllowHeaders  = [ "Authorization" ]
                                }
                            });

                    }

                    #endregion

                    #region Check tariff

                    if (!request.ParseMandatoryTariff(CommonAPI,
                                                      out var countryCode,
                                                      out var partyId,
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
                                                      ),
                               HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   AccessControlAllowMethods  = ["OPTIONS", "GET", "PUT", "PATCH", "DELETE"],
                                   AccessControlAllowHeaders  = [ "Authorization" ],
                                   LastModified               = tariff.LastUpdated,
                                   ETag                       = tariff.ETag
                               }
                        });

                }
            );

            #endregion

            #region PUT      ~/tariffs/{country_code}/{party_id}/{tariffId}

            CommonAPI.AddOCPIMethod(

                HTTPMethod.PUT,
                URLPathPrefix + "tariffs/{country_code}/{party_id}/{tariffId}",
                PutTariffRequest,
                PutTariffResponse,
                async request => {

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

                    #region Check existing tariff

                    if (!request.ParseOptionalTariff(CommonAPI,
                                                     out var countryCode,
                                                     out var partyId,
                                                     out var tariffId,
                                                     out var existingTariff,
                                                     out var ocpiResponseBuilder))
                    {
                        return ocpiResponseBuilder;
                    }

                    #endregion

                    #region Parse new or updated tariff

                    if (!request.TryParseJObjectRequestBody(out var tariffJSON, out ocpiResponseBuilder))
                        return ocpiResponseBuilder;

                    if (!Tariff.TryParse(tariffJSON,
                                         out var newOrUpdatedTariff,
                                         out var errorResponse,
                                         countryCode,
                                         partyId,
                                         tariffId))
                    {

                        return new OCPIResponse.Builder(request) {
                                   StatusCode           = 2001,
                                   StatusMessage        = "Could not parse the given tariff JSON: " + errorResponse,
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                   }
                               };

                    }

                    #endregion


                    var addOrUpdateResult = await CommonAPI.AddOrUpdateTariff(
                                                      newOrUpdatedTariff,
                                                      AllowDowngrades ?? request.QueryString.GetBoolean("forceDowngrade")
                                                  );


                    if (addOrUpdateResult.IsSuccessAndDataNotNull(out var data))
                        return new OCPIResponse.Builder(request) {
                                   StatusCode           = 1000,
                                   StatusMessage        = "Hello world!",
                                   Data                 = data.ToJSON(
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
                                                          ),
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
                               Data                 = newOrUpdatedTariff.ToJSON(
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
                                                      ),
                               HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ],
                                   LastModified               = newOrUpdatedTariff.LastUpdated,
                                   ETag                       = newOrUpdatedTariff.ETag
                               }
                           };

                }
            );

            #endregion

            #region PATCH    ~/tariffs/{country_code}/{party_id}/{tariffId}      [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPMethod.PATCH,
                URLPathPrefix + "tariffs/{country_code}/{party_id}/{tariffId}",
                PatchTariffRequest,
                PatchTariffResponse,
                async request => {

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

                    #region Check tariff

                    if (!request.ParseMandatoryTariff(CommonAPI,
                                                      out var countryCode,
                                                      out var partyId,
                                                      out var tariffId,
                                                      out var existingTariff,
                                                      out var ocpiResponseBuilder))
                    {
                        return ocpiResponseBuilder;
                    }

                    #endregion

                    #region Parse and apply Tariff JSON patch

                    if (!request.TryParseJObjectRequestBody(out var tariffPatch, out ocpiResponseBuilder))
                        return ocpiResponseBuilder;

                    #endregion


                    // Validation-Checks for PATCHes
                    // (E-Tag, Timestamp, ...)

                    var patchedTariff = await CommonAPI.TryPatchTariff(
                                                  Party_Idv3.From(
                                                      existingTariff.CountryCode,
                                                      existingTariff.PartyId
                                                  ),
                                                  existingTariff.Id,
                                                  tariffPatch
                                              );

                    //ToDo: Handle update errors!
                    if (patchedTariff.IsSuccessAndDataNotNull(out var patchedData))
                        return new OCPIResponse.Builder(request) {
                                       StatusCode           = 1000,
                                       StatusMessage        = "Hello world!",
                                       Data                 = patchedData.ToJSON(),
                                       HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                           HTTPStatusCode             = HTTPStatusCode.OK,
                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                           AccessControlAllowHeaders  = [ "Authorization" ],
                                           LastModified               = patchedData.LastUpdated,
                                           ETag                       = patchedData.ETag
                                       }
                                   };

                    return new OCPIResponse.Builder(request) {
                                   StatusCode           = 2000,
                                   StatusMessage        = patchedTariff.ErrorResponse,
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                   }
                               };

                }
            );

            #endregion

            #region DELETE   ~/tariffs/{country_code}/{party_id}/{tariffId}

            CommonAPI.AddOCPIMethod(

                HTTPMethod.DELETE,
                URLPathPrefix + "tariffs/{country_code}/{party_id}/{tariffId}",
                DeleteTariffRequest,
                DeleteTariffResponse,
                async request => {

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

                    #region Check existing tariff

                    if (!request.ParseMandatoryTariff(CommonAPI,
                                                      out var countryCode,
                                                      out var partyId,
                                                      out var tariffId,
                                                      out var existingTariff,
                                                      out var ocpiResponseBuilder))
                    {
                        return ocpiResponseBuilder;
                    }

                    #endregion


                    var result = await CommonAPI.RemoveTariff(existingTariff);


                    if (result.IsSuccessAndDataNotNull(out var tariffData))
                        return new OCPIResponse.Builder(request) {
                                   StatusCode           = 1000,
                                   StatusMessage        = "Hello world!",
                                   Data                 = existingTariff.ToJSON(
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
                                                          ),
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ],
                                       LastModified               = existingTariff.LastUpdated,
                                       ETag                       = existingTariff.ETag
                                   }
                               };

                    return new OCPIResponse.Builder(request) {
                               StatusCode           = 2000,
                               StatusMessage        = "Failed!",
                               Data                 = existingTariff.ToJSON(
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
                                                      ),
                               HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ],
                                   LastModified               = existingTariff.LastUpdated,
                                   ETag                       = existingTariff.ETag
                               }
                           };

                }
            );

            #endregion

            #endregion



            #region ~/sessions/{country_code}/{party_id}                                [NonStandard]

            #region OPTIONS  ~/sessions/{country_code}/{party_id}                [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPMethod.OPTIONS,
                URLPathPrefix + "sessions/{country_code}/{party_id}",
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

            #region GET      ~/sessions                                          [NonStandard]

            // Return all charging session for the given access token roles

            CommonAPI.AddOCPIMethod(

                HTTPMethod.GET,
                URLPathPrefix + "sessions",
                GetSessionsRequest,
                GetSessionsResponse,
                request => {

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

                    var allSessions       = CommonAPI.GetSessions(session => request.LocalAccessInfo.Roles.Any(role => role.PartyId.CountryCode == session.CountryCode &&
                                                                                                                       role.PartyId.Party       == session.PartyId)).
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
                                                              Select(session => session.ToJSON(
                                                                                    CustomSessionSerializer,
                                                                                    CustomCDRTokenSerializer,
                                                                                    CustomChargingPeriodSerializer,
                                                                                    CustomCDRDimensionSerializer,
                                                                                    CustomPriceSerializer
                                                                                ))
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

                }
            );

            #endregion

            #region GET      ~/sessions/{country_code}/{party_id}                [NonStandard]

            // Return all charging session for the given country code and party identification

            CommonAPI.AddOCPIMethod(

                HTTPMethod.GET,
                URLPathPrefix + "sessions/{country_code}/{party_id}",
                GetSessionsRequest,
                GetSessionsResponse,
                request => {

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
                                                              Select(session => session.ToJSON(
                                                                                    CustomSessionSerializer,
                                                                                    CustomCDRTokenSerializer,
                                                                                    CustomChargingPeriodSerializer,
                                                                                    CustomCDRDimensionSerializer,
                                                                                    CustomPriceSerializer
                                                                                ))
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

                }
            );

            #endregion

            #region DELETE   ~/sessions                                          [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPMethod.DELETE,
                URLPathPrefix + "sessions",
                DeleteSessionsRequest,
                DeleteSessionsResponse,
                async request => {

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
                        await CommonAPI.RemoveAllSessions(role.PartyId);


                    return new OCPIResponse.Builder(request) {
                               StatusCode           = 1000,
                               StatusMessage        = "Hello world!",
                               HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET", "DELETE" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ]
                               }
                           };

                }
            );

            #endregion

            #region DELETE   ~/sessions/{country_code}/{party_id}                [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPMethod.DELETE,
                URLPathPrefix + "sessions/{country_code}/{party_id}",
                DeleteSessionsRequest,
                DeleteSessionsResponse,
                async request => {

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

                }
            );

            #endregion

            #endregion

            #region ~/sessions/{country_code}/{party_id}/{session_id}

            #region OPTIONS  ~/sessions/{country_code}/{party_id}/{session_id}    [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPMethod.OPTIONS,
                URLPathPrefix + "sessions/{country_code}/{party_id}/{session_id}",
                request =>

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

            #region GET      ~/sessions/{country_code}/{party_id}/{session_id}

            CommonAPI.AddOCPIMethod(

                HTTPMethod.GET,
                URLPathPrefix + "sessions/{country_code}/{party_id}/{session_id}",
                GetSessionRequest,
                GetSessionResponse,
                request => {

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
                                                       out var session_id,
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
                                AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                AccessControlAllowHeaders  = [ "Authorization" ],
                                LastModified               = session.LastUpdated,
                                ETag                       = session.ETag
                            }
                        });

                }
            );

            #endregion

            #region PUT      ~/sessions/{country_code}/{party_id}/{session_id}

            CommonAPI.AddOCPIMethod(

                HTTPMethod.PUT,
                URLPathPrefix + "sessions/{country_code}/{party_id}/{session_id}",
                PutSessionRequest,
                PutSessionResponse,
                async request => {

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

                    #region Parse session identification

                    if (!request.ParseSessionId(CommonAPI,
                                                out var countryCode,
                                                out var partyId,
                                                out var session_id,
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
                                          session_id))
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


                    if (addOrUpdateResult.IsSuccessAndDataNotNull(out var sessionData))
                        return new OCPIResponse.Builder(request) {
                                   StatusCode           = 1000,
                                   StatusMessage        = "Hello world!",
                                   Data                 = sessionData.ToJSON(
                                                              CustomSessionSerializer,
                                                              CustomCDRTokenSerializer,
                                                              CustomChargingPeriodSerializer,
                                                              CustomCDRDimensionSerializer,
                                                              CustomPriceSerializer
                                                          ),
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = addOrUpdateResult.WasCreated == true
                                                                        ? HTTPStatusCode.Created
                                                                        : HTTPStatusCode.OK,
                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ],
                                       LastModified               = sessionData.LastUpdated,
                                       ETag                       = sessionData.ETag
                                   }
                               };

                    return new OCPIResponse.Builder(request) {
                               StatusCode           = 2000,
                               StatusMessage        = addOrUpdateResult.ErrorResponse,
                               Data                 = newOrUpdatedSession.ToJSON(
                                                          CustomSessionSerializer,
                                                          CustomCDRTokenSerializer,
                                                          CustomChargingPeriodSerializer,
                                                          CustomCDRDimensionSerializer,
                                                          CustomPriceSerializer
                                                      ),
                               HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ],
                                   LastModified               = newOrUpdatedSession.LastUpdated,
                                   ETag                       = newOrUpdatedSession.ETag
                               }
                           };

                }
            );

            #endregion

            #region PATCH    ~/sessions/{country_code}/{party_id}/{session_id}

            CommonAPI.AddOCPIMethod(

                HTTPMethod.PATCH,
                URLPathPrefix + "sessions/{country_code}/{party_id}/{session_id}",
                PatchSessionRequest,
                PatchSessionResponse,
                async request => {

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
                                                       out var session_id,
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
                    if (patchedSession.IsSuccessAndDataNotNull(out var patchedSessionData))
                        return new OCPIResponse.Builder(request) {
                                       StatusCode           = 1000,
                                       StatusMessage        = "Hello world!",
                                       Data                 = patchedSessionData.ToJSON(
                                                                  CustomSessionSerializer,
                                                                  CustomCDRTokenSerializer,
                                                                  CustomChargingPeriodSerializer,
                                                                  CustomCDRDimensionSerializer,
                                                                  CustomPriceSerializer
                                                              ),
                                       HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                           HTTPStatusCode             = HTTPStatusCode.OK,
                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                           AccessControlAllowHeaders  = [ "Authorization" ],
                                           LastModified               = patchedSessionData.LastUpdated,
                                           ETag                       = patchedSessionData.ETag
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

                }
            );

            #endregion

            #region DELETE   ~/sessions/{country_code}/{party_id}/{session_id}    [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPMethod.DELETE,
                URLPathPrefix + "sessions/{country_code}/{party_id}/{session_id}",
                DeleteSessionRequest,
                DeleteSessionResponse,
                async request => {

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
                                                       out var session_id,
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
                                   Data                 = existingSession.ToJSON(
                                                              CustomSessionSerializer,
                                                              CustomCDRTokenSerializer,
                                                              CustomChargingPeriodSerializer,
                                                              CustomCDRDimensionSerializer,
                                                              CustomPriceSerializer
                                                          ),
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                       //LastModified               = Timestamp.Now.ToISO8601()
                                   }
                               };

                }
            );

            #endregion

            #endregion



            #region ~/cdrs/{country_code}/{party_id}                                    [NonStandard]

            #region OPTIONS  ~/cdrs/{country_code}/{party_id}           [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPMethod.OPTIONS,
                URLPathPrefix + "cdrs/{country_code}/{party_id}",
                request =>

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

            #region GET      ~/cdrs                                     [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPMethod.GET,
                URLPathPrefix + "cdrs",
                GetCDRsRequest,
                GetCDRsResponse,
                request => {

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

                    #region Check party identification

                    if (!request.ParsePartyId(CommonAPI,
                                              out var partyId,
                                              out var ocpiResponseBuilder))
                    {
                        return Task.FromResult(ocpiResponseBuilder);
                    }

                    #endregion


                    var filters       = request.GetDateAndPaginationFilters();

                    var allCDRs       = CommonAPI.GetCDRs(session => request.LocalAccessInfo.Roles.Any(role => role.PartyId.CountryCode == session.CountryCode &&
                                                                                                               role.PartyId.Party       == session.PartyId)).
                                                  ToArray();

                    var filteredCDRs  = allCDRs.Where(cdr => !filters.From.HasValue || cdr.LastUpdated >  filters.From.Value).
                                                Where(cdr => !filters.To.  HasValue || cdr.LastUpdated <= filters.To.  Value).
                                                ToArray();


                    return Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               StatusCode           = 1000,
                               StatusMessage        = "Hello world!",
                               Data                 = new JArray(
                                                          filteredCDRs.
                                                              SkipTakeFilter(filters.Offset,
                                                                             filters.Limit).
                                                              Select(cdr => cdr.ToJSON(
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
                                                      ),
                               HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "DELETE" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ]
                                   //LastModified               = ?
                               }.
                               Set("X-Total-Count", allCDRs.Length)
                               // X-Limit               The maximum number of objects that the server WILL return.
                               // Link                  Link to the 'next' page should be provided when this is NOT the last page.
                        });

                }
            );

            #endregion

            #region GET      ~/cdrs/{country_code}/{party_id}           [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPMethod.GET,
                URLPathPrefix + "cdrs/{country_code}/{party_id}",
                GetCDRsRequest,
                GetCDRsResponse,
                request => {

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

                    #region Check party identification

                    if (!request.ParsePartyId(CommonAPI,
                                              out var partyId,
                                              out var ocpiResponseBuilder))
                    {
                        return Task.FromResult(ocpiResponseBuilder);
                    }

                    #endregion


                    var filters       = request.GetDateAndPaginationFilters();

                    var allCDRs       = CommonAPI.GetCDRs(partyId.Value).ToArray();

                    var filteredCDRs  = CommonAPI.GetCDRs().
                                                  Where(cdr => !filters.From.HasValue || cdr.LastUpdated >  filters.From.Value).
                                                  Where(cdr => !filters.To.  HasValue || cdr.LastUpdated <= filters.To.  Value).
                                                  ToArray();


                    return Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               StatusCode           = 1000,
                               StatusMessage        = "Hello world!",
                               Data                 = new JArray(
                                                          filteredCDRs.
                                                              SkipTakeFilter(filters.Offset,
                                                                             filters.Limit).
                                                              Select(cdr => cdr.ToJSON(
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
                                                      ),
                               HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "DELETE" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ]
                                   //LastModified               = ?
                               }.
                               Set("X-Total-Count", allCDRs.Length)
                               // X-Limit               The maximum number of objects that the server WILL return.
                               // Link                  Link to the 'next' page should be provided when this is NOT the last page.
                        });

                }
            );

            #endregion

            #region POST     ~/cdrs               ///{country_code}/{party_id}       <= Unclear if this URL is correct!

            CommonAPI.AddOCPIMethod(

                HTTPMethod.POST,
                URLPathPrefix + "cdrs",///{country_code}/{party_id}",
                PostCDRRequest,
                PostCDRResponse,
                async request => {

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

                    #region Check party identification

                    //if (!request.ParsePartyId(CommonAPI,
                    //                          out var partyId,
                    //                          out var ocpiResponseBuilder))
                    //{
                    //    return ocpiResponseBuilder;
                    //}

                    #endregion

                    #region Parse newCDR JSON

                    if (!request.TryParseJObjectRequestBody(out var jsonCDR, out var ocpiResponseBuilder))
                        return ocpiResponseBuilder;

                    if (!CDR.TryParse(jsonCDR,
                                      out var newCDR,
                                      out var errorResponse
                                      //partyId.Value.CountryCode,
                                      //partyId.Value.Party
                                      ))
                    {

                        return new OCPIResponse.Builder(request) {
                                   StatusCode           = 2001,
                                   StatusMessage        = "Could not parse the given charge detail record JSON: " + errorResponse,
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "DELETE" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                   }
                               };

                    }

                    #endregion


                    // ToDo: What kind of error might happen here?
                    var result = await CommonAPI.AddCDR(newCDR);


                    // https://github.com/ocpi/ocpi/blob/release-2.2-bugfixes/mod_cdrs.asciidoc#mod_cdrs_post_method
                    // The response should contain the URL to the just created CDR object in the eMSP’s system.
                    //
                    // Parameter    Location
                    // Datatype     URL
                    // Required     yes
                    // Description  URL to the newly created CDR in the eMSP’s system, can be used by the CPO system to perform a GET on the same CDR.
                    // Example      https://www.server.com/ocpi/emsp/2.2/cdrs/123456

                    return new OCPIResponse.Builder(request) {
                                   StatusCode           = 1000,
                                   StatusMessage        = "Hello world!",
                                   Data                 = newCDR.ToJSON(
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
                                       HTTPStatusCode             = HTTPStatusCode.Created,
                                       Location                   = org.GraphDefined.Vanaheimr.Hermod.HTTP.Location.From(URLPathPrefix + "cdrs" + newCDR.CountryCode.ToString() + newCDR.PartyId.ToString() + newCDR.Id.ToString()),
                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ],
                                       LastModified               = newCDR.LastUpdated,
                                       ETag                       = newCDR.ETag
                                   }
                               };

                }
            );

            #endregion

            #region DELETE   ~/cdrs                                     [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPMethod.DELETE,
                URLPathPrefix + "cdrs",
                DeleteCDRsRequest,
                DeleteCDRsResponse,
                async request => {

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


                    foreach (var role in request.LocalAccessInfo.Roles)
                        await CommonAPI.RemoveAllCDRs(role.PartyId);


                    return new OCPIResponse.Builder(request) {
                               StatusCode           = 1000,
                               StatusMessage        = "Hello world!",
                               HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "DELETE" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ]
                               }
                           };

                }
            );

            #endregion

            #region DELETE   ~/cdrs/{country_code}/{party_id}           [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPMethod.DELETE,
                URLPathPrefix + "cdrs/{country_code}/{party_id}",
                DeleteCDRsRequest,
                DeleteCDRsResponse,
                async request => {

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

                    #region Check party identification

                    if (!request.ParsePartyId(CommonAPI,
                                              out var partyId,
                                              out var ocpiResponseBuilder))
                    {
                        return ocpiResponseBuilder;
                    }

                    #endregion


                    var result = await CommonAPI.RemoveAllCDRs(partyId.Value);


                    return new OCPIResponse.Builder(request) {
                               StatusCode           = 1000,
                               StatusMessage        = "Hello world!",
                               HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "DELETE" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ]
                               }
                           };

                }
            );

            #endregion

            #endregion

            #region ~/cdrs/{country_code}/{party_id}/{cdrId}

            #region OPTIONS  ~/cdrs/{country_code}/{party_id}/{cdrId}    [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPMethod.OPTIONS,
                URLPathPrefix + "cdrs/{country_code}/{party_id}/{cdrId}",
                request =>

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

            #region GET      ~/cdrs/{country_code}/{party_id}/{cdrId}       // The concrete URL is not specified by OCPI! m(

            CommonAPI.AddOCPIMethod(

                HTTPMethod.GET,
                URLPathPrefix + "cdrs/{country_code}/{party_id}/{cdrId}",
                GetCDRRequest,
                GetCDRResponse,
                request => {

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

                    #region Check existing CDR

                    if (!request.ParseMandatoryCDR(CommonAPI,
                                                   out var countryCode,
                                                   out var partyId,
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
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET", "DELETE" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ],
                                   LastModified               = cdr.LastUpdated,
                                   ETag                       = cdr.ETag
                               }
                        });

                }
            );

            #endregion

            #region DELETE   ~/cdrs/{country_code}/{party_id}/{cdrId}    [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPMethod.DELETE,
                URLPathPrefix + "cdrs/{country_code}/{party_id}/{cdrId}",
                DeleteCDRRequest,
                DeleteCDRResponse,
                async request => {

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

                    #region Check existing CDR

                    if (!request.ParseMandatoryCDR(CommonAPI,
                                                   out var countryCode,
                                                   out var partyId,
                                                   out var cdrId,
                                                   out var existingCDR,
                                                   out var ocpiResponseBuilder))
                    {
                        return ocpiResponseBuilder;
                    }

                    #endregion


                    //ToDo: await...
                    await CommonAPI.RemoveCDR(existingCDR);


                    return new OCPIResponse.Builder(request) {
                               StatusCode           = 1000,
                               StatusMessage        = "Hello world!",
                               Data                 = existingCDR.ToJSON(
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
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET", "DELETE" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ],
                                   LastModified               = existingCDR.LastUpdated,
                                   ETag                       = existingCDR.ETag
                               }
                           };

                }
            );

            #endregion

            #endregion



            #region ~/tokens

            #region OPTIONS  ~/tokens

            // https://example.com/ocpi/2.2/cpo/tokens/?date_from=2019-01-28T12:00:00&date_to=2019-01-29T12:00:00&offset=50&limit=100
            CommonAPI.AddOCPIMethod(

                HTTPMethod.OPTIONS,
                URLPathPrefix + "tokens",
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

            #region GET      ~/tokens

            // https://example.com/ocpi/2.2/cpo/tokens/?date_from=2019-01-28T12:00:00&date_to=2019-01-29T12:00:00&offset=50&limit=100
            CommonAPI.AddOCPIMethod(

                HTTPMethod.GET,
                URLPathPrefix + "tokens",
                GetTokensRequest,
                GetTokensResponse,
                request => {

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


                    var filters         = request.GetDateAndPaginationFilters();

                    var allTokens       = CommonAPI.GetTokens().ToArray();

                    var filteredTokens  = allTokens.Where(token => !filters.From.HasValue || token.LastUpdated >  filters.From.Value).
                                                    Where(token => !filters.To.  HasValue || token.LastUpdated <= filters.To.  Value).
                                                    ToArray();


                    return Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               StatusCode           = 1000,
                               StatusMessage        = "Hello world!",
                               Data                 = new JArray(
                                                          filteredTokens.SkipTakeFilter(filters.Offset,
                                                                                        filters.Limit).
                                                                         Select        (token => token.ToJSON(
                                                                                                     CustomTokenSerializer,
                                                                                                     CustomEnergyContractSerializer
                                                                                                 ))
                                                      ),
                               HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ]
                                   //LastModified               = ?
                               }.
                               Set("X-Total-Count", allTokens.Length)
                               // X-Limit               The maximum number of objects that the server WILL return.
                               // Link                  Link to the 'next' page should be provided when this is NOT the last page.
                        });

                }
            );

            #endregion

            #endregion

            #region ~/tokens/{token_id}/authorize

            #region OPTIONS  ~/tokens/{token_id}/authorize

            CommonAPI.AddOCPIMethod(

                HTTPMethod.OPTIONS,
                URLPathPrefix + "tokens/{token_id}/authorize",
                request =>

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
            CommonAPI.AddOCPIMethod(

                HTTPMethod.POST,
                URLPathPrefix + "tokens/{token_id}/authorize",
                PostTokenRequest,
                PostTokenResponse,
                async request => {

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

                    #region Check TokenId URI parameter

                    if (!request.ParseTokenId(out var tokenId,
                                              out var ocpiResponseBuilder))
                    {
                        return ocpiResponseBuilder;
                    }

                    #endregion

                    var requestedTokenType  = request.QueryString.Map("type", TokenType.TryParse) ?? TokenType.RFID;

                    #region Parse optional LocationReference JSON

                    LocationReference? locationReference = null;

                    if (request.TryParseJObjectRequestBody(out var locationReferenceJSON,
                                                           out ocpiResponseBuilder,
                                                           AllowEmptyHTTPBody: true))
                    {

                        if (!LocationReference.TryParse(locationReferenceJSON,
                                                        out var _locationReference,
                                                        out var errorResponse))
                        {

                            return new OCPIResponse.Builder(request) {
                                   StatusCode           = 2001,
                                   StatusMessage        = "Could not parse the given location reference JSON: " + errorResponse,
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST" ],
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

                            var result = await onRFIDAuthTokenLocal(
                                                   request.FromCountryCode ?? CommonAPI.DefaultPartyId.CountryCode,
                                                   request.FromPartyId     ?? CommonAPI.DefaultPartyId.Party,
                                                   request.ToCountryCode   ?? CommonAPI.DefaultPartyId.CountryCode,
                                                   request.ToPartyId       ?? CommonAPI.DefaultPartyId.Party,
                                                   tokenId.Value,
                                                   locationReference
                                               );

                            authorizationInfo = result;

                        }
                        catch (Exception e)
                        {
                            DebugX.LogException(e, "Could not do an RFID auth!");
                        }

                    }

                    else
                    {

                        #region Check existing token

                        if (!CommonAPI.TryGetTokenStatus(
                                Party_Idv3.From(
                                    request.ToCountryCode,
                                    request.ToPartyId
                                ) ?? CommonAPI.DefaultPartyId,
                                tokenId.Value,
                                out var _tokenStatus) ||
                            (_tokenStatus.Token.Type != requestedTokenType))
                        {

                            return new OCPIResponse.Builder(request) {
                                       StatusCode           = 2004,
                                       StatusMessage        = "Unknown token!",
                                       HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                           HTTPStatusCode             = HTTPStatusCode.NotFound,
                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST" ],
                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                       }
                                   };

                        }

                        #endregion

                        authorizationInfo = new AuthorizationInfo(
                                                  _tokenStatus.Status,
                                                  _tokenStatus.Token,
                                                  _tokenStatus.LocationReference,
                                                  AuthorizationReference.NewRandom()
                                                  //new DisplayText(
                                                  //    _tokenStatus.Token.UILanguage ?? Languages.en,
                                                  //    responseText
                                                  //)
                                            );

                        #region Parse optional LocationReference JSON

                        if (locationReference.HasValue)
                        {

                            Location? validLocation = null;

                            if (request.FromCountryCode.HasValue && request.FromPartyId.HasValue)
                            {

                                if (!CommonAPI.TryGetLocation(
                                    Party_Idv3.From(
                                        request.FromCountryCode.Value,
                                        request.FromPartyId.    Value
                                    ),
                                    locationReference.Value.LocationId,
                                    out validLocation))
                                {

                                    return new OCPIResponse.Builder(request) {
                                        StatusCode           = 2001,
                                        StatusMessage        = "The given location is unknown!",
                                        Data                 = new AuthorizationInfo(
                                                                   AllowedType.NOT_ALLOWED,
                                                                   _tokenStatus.Token,
                                                                   locationReference.Value,
                                                                   null,
                                                                   new DisplayText(Languages.en, "The given location is unknown!")
                                                               ).ToJSON(),
                                        HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                            HTTPStatusCode             = HTTPStatusCode.NotFound,
                                            AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST" ],
                                            AccessControlAllowHeaders  = [ "Authorization" ]
                                        }
                                    };

                                }

                            }

                            else
                            {

                                if (request.LocalAccessInfo.Roles.Where(role => role.Role == Role.CPO).Count() != 1)
                                {

                                    return new OCPIResponse.Builder(request) {
                                        StatusCode           = 2001,
                                        StatusMessage        = "Could not determine the country code and party identification of the given location!",
                                        Data                 = new AuthorizationInfo(
                                                                   AllowedType.NOT_ALLOWED,
                                                                   _tokenStatus.Token,
                                                                   locationReference.Value
                                                               ).ToJSON(),
                                        HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                            HTTPStatusCode             = HTTPStatusCode.NotFound,
                                            AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST" ],
                                            AccessControlAllowHeaders  = [ "Authorization" ]
                                        }
                                    };

                                }

                                var allTheirCPORoles = request.LocalAccessInfo.Roles.Where(role => role.Role == Role.CPO).ToArray();

                                if (!CommonAPI.TryGetLocation(
                                    allTheirCPORoles[0].PartyId,
                                    locationReference.Value.LocationId,
                                    out validLocation))
                                {

                                    return new OCPIResponse.Builder(request) {
                                        StatusCode           = 2001,
                                        StatusMessage        = "The given location is unknown!",
                                        Data                 = new AuthorizationInfo(
                                                                   AllowedType.NOT_ALLOWED,
                                                                   _tokenStatus.Token,
                                                                   locationReference.Value,
                                                                   null,
                                                                   new DisplayText(Languages.en, "The given location is unknown!")
                                                               ).ToJSON(),
                                        HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                            HTTPStatusCode             = HTTPStatusCode.NotFound,
                                            AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST" ],
                                            AccessControlAllowHeaders  = [ "Authorization" ]
                                        }
                                    };

                                }

                            }


                            //ToDo: Add a event/delegate for additional location filters!


                            if (locationReference.Value.EVSEUIds.SafeAny())
                            {

                                locationReference = new LocationReference(locationReference.Value.LocationId,
                                                                          locationReference.Value.EVSEUIds.
                                                                                                  Where(evseuid => validLocation.EVSEExists(evseuid)));

                                if (!locationReference.Value.EVSEUIds.SafeAny())
                                {

                                    return new OCPIResponse.Builder(request) {
                                        StatusCode           = 2001,
                                        StatusMessage        = locationReference.Value.EVSEUIds.Count() == 1
                                                                   ? "The EVSE at the given location is unknown!"
                                                                   : "The EVSEs at the given location are unknown!",
                                        Data                 = new AuthorizationInfo(
                                                                   AllowedType.NOT_ALLOWED,
                                                                   _tokenStatus.Token,
                                                                   locationReference.Value,
                                                                   null,
                                                                   new DisplayText(
                                                                       Languages.en,
                                                                       locationReference.Value.EVSEUIds.Count() == 1
                                                                           ? "The EVSE at the given location is unknown!"
                                                                           : "The EVSEs at the given location are unknown!"
                                                                   )
                                                               ).ToJSON(),
                                        HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                            HTTPStatusCode             = HTTPStatusCode.NotFound,
                                            AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST" ],
                                            AccessControlAllowHeaders  = [ "Authorization" ]
                                        }
                                    };

                                }



                                //ToDo: Add a event/delegate for additional EVSE filters!

                            }

                        }

                        #endregion

                    }

                    authorizationInfo ??= new AuthorizationInfo(
                                              AllowedType.BLOCKED,
                                              new Token(
                                                  CountryCode.Parse("DE"),
                                                  Party_Id.Parse("XXX"),
                                                  tokenId.Value,
                                                  requestedTokenType,
                                                  Contract_Id.Parse("DE-XXX-" + tokenId.ToString()),
                                                  "Error!",
                                                  false,
                                                  WhitelistTypes.NEVER
                                              )
                                          );


                    // too little information like e.g. no LocationReferences provided:
                    //   => status_code 2002


                    #region Set a user-friendly response message for the ev driver

                    var responseText = "An error occurred!";

                    if (!authorizationInfo.Info.HasValue)
                    {

                        #region ALLOWED

                        if (authorizationInfo.Allowed == AllowedType.ALLOWED)
                        {

                            responseText = "Charging allowed!";

                            if (authorizationInfo.Token?.UILanguage.HasValue == true)
                            {
                                switch (authorizationInfo.Token.UILanguage.Value)
                                {
                                    case Languages.de:
                                        responseText = "Der Ladevorgang wird gestartet!";
                                        break;
                                }
                            }

                        }

                        #endregion

                        #region BLOCKED

                        else if (authorizationInfo.Allowed == AllowedType.BLOCKED)
                        {

                            responseText = "Sorry, your token is blocked!";

                            if (authorizationInfo.Token?.UILanguage.HasValue == true)
                            {
                                switch (authorizationInfo.Token.UILanguage.Value)
                                {
                                    case Languages.de:
                                        responseText = "Autorisierung fehlgeschlagen!";
                                        break;
                                }
                            }

                        }

                        #endregion

                        #region EXPIRED

                        else if (authorizationInfo.Allowed == AllowedType.EXPIRED)
                        {

                            responseText = "Sorry, your token has expired!";

                            if (authorizationInfo.Token?.UILanguage.HasValue == true)
                            {
                                switch (authorizationInfo.Token.UILanguage.Value)
                                {
                                    case Languages.de:
                                        responseText = "Autorisierungstoken ungültig!";
                                        break;
                                }
                            }

                        }

                        #endregion

                        #region NO_CREDIT

                        else if (authorizationInfo.Allowed == AllowedType.NO_CREDIT)
                        {

                            responseText = "Sorry, your have not enough credits for charging!";

                            if (authorizationInfo.Token?.UILanguage.HasValue == true)
                            {
                                switch (authorizationInfo.Token.UILanguage.Value)
                                {
                                    case Languages.de:
                                        responseText = "Nicht genügend Ladeguthaben!";
                                        break;
                                }
                            }

                        }

                        #endregion

                        #region NOT_ALLOWED

                        else if (authorizationInfo.Allowed == AllowedType.NOT_ALLOWED)
                        {

                            responseText = "Sorry, charging is not allowed!";

                            if (authorizationInfo.Token?.UILanguage.HasValue == true)
                            {
                                switch (authorizationInfo.Token.UILanguage.Value)
                                {
                                    case Languages.de:
                                        responseText = "Autorisierung abgelehnt!";
                                        break;
                                }
                            }

                        }

                        #endregion

                        #region default

                        else
                        {

                            responseText = "An error occurred!";

                            if (authorizationInfo.Token?.UILanguage.HasValue == true)
                            {
                                switch (authorizationInfo.Token.UILanguage.Value)
                                {
                                    case Languages.de:
                                        responseText = "Ein Fehler ist aufgetreten!";
                                        break;
                                }
                            }

                        }

                        #endregion

                    }

                    #endregion


                    return new OCPIResponse.Builder(request) {
                               StatusCode           = 1000,
                               StatusMessage        = "Hello world!",
                               Data                 = new AuthorizationInfo(
                                                          authorizationInfo.Allowed,
                                                          authorizationInfo.Token,
                                                          authorizationInfo.Location,
                                                          authorizationInfo.AuthorizationReference ?? AuthorizationReference.NewRandom(),
                                                          authorizationInfo.Info                   ?? new DisplayText(
                                                                                                          authorizationInfo.Token?.UILanguage ?? Languages.en,
                                                                                                          responseText
                                                                                                      ),
                                                          authorizationInfo.RemoteParty,
                                                          authorizationInfo.EMSPId,
                                                          authorizationInfo.Runtime
                                                      ).ToJSON(
                                                            CustomAuthorizationInfoSerializer,
                                                            CustomTokenSerializer,
                                                            CustomLocationReferenceSerializer,
                                                            CustomDisplayTextSerializer
                                                        ),
                               HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ]
                               }
                           };

                }
            );

            #endregion

            #endregion



            // Command result callbacks

            #region ~/commands/RESERVE_NOW/{command_id}

            #region OPTIONS  ~/commands/RESERVE_NOW/{command_id}

            CommonAPI.AddOCPIMethod(

                HTTPMethod.OPTIONS,
                URLPathPrefix + "commands/RESERVE_NOW/{command_id}",
                request =>

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

            #region POST     ~/commands/RESERVE_NOW/{command_id}

            CommonAPI.AddOCPIMethod(

                HTTPMethod.POST,
                URLPathPrefix + "commands/RESERVE_NOW/{command_id}",
                ReserveNowCallbackRequest,
                ReserveNowCallbackResponse,
                request => {

                    #region Check command identification

                    if (!request.ParseCommandId(CommonAPI,
                                                out var commandId,
                                                out var ocpiResponseBuilder))
                    {
                        return Task.FromResult(ocpiResponseBuilder);
                    }

                    #endregion

                    #region Parse command result JSON

                    if (!request.TryParseJObjectRequestBody(out var json, out ocpiResponseBuilder))
                        return Task.FromResult(ocpiResponseBuilder);

                    if (!CommandResult.TryParse(json,
                                                out var commandResult,
                                                out var errorResponse))
                    {

                        return Task.FromResult(
                                   new OCPIResponse.Builder(request) {
                                       StatusCode           = 2001,
                                       StatusMessage        = "Could not parse the given 'RESERVE NOW' command result JSON: " + errorResponse,
                                       HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                           HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                           AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
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
                                   new OCPIResponse.Builder(request) {
                                       StatusCode           = 1000,
                                       HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                           HTTPStatusCode             = HTTPStatusCode.Accepted,
                                           AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                       }
                                   }
                               );

                    }

                    return Task.FromResult(
                               new OCPIResponse.Builder(request) {
                                   StatusCode           = 2000,
                                   StatusMessage        = "Unknown 'RESERVE NOW' command identification!",
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                       AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                   }
                               }
                           );

                }
            );

            #endregion

            #endregion

            #region ~/commands/CANCEL_RESERVATION/{command_id}

            #region OPTIONS  ~/commands/CANCEL_RESERVATION/{command_id}

            CommonAPI.AddOCPIMethod(

                HTTPMethod.OPTIONS,
                URLPathPrefix + "commands/CANCEL_RESERVATION/{command_id}",
                request =>

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

            #region POST     ~/commands/CANCEL_RESERVATION/{command_id}

            CommonAPI.AddOCPIMethod(

                HTTPMethod.POST,
                URLPathPrefix + "commands/CANCEL_RESERVATION/{command_id}",
                CancelReservationCallbackRequest,
                CancelReservationCallbackResponse,
                request => {

                    #region Check command identification

                    if (!request.ParseCommandId(CommonAPI,
                                                out var commandId,
                                                out var ocpiResponseBuilder))
                    {
                        return Task.FromResult(ocpiResponseBuilder);
                    }

                    #endregion

                    #region Parse command result JSON

                    if (!request.TryParseJObjectRequestBody(out var json, out ocpiResponseBuilder))
                        return Task.FromResult(ocpiResponseBuilder);

                    if (!CommandResult.TryParse(json,
                                                out var commandResult,
                                                out var errorResponse))
                    {

                        return Task.FromResult(
                                   new OCPIResponse.Builder(request) {
                                       StatusCode           = 2001,
                                       StatusMessage        = "Could not parse the given 'CANCEL RESERVATION' command result JSON: " + errorResponse,
                                       HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                           HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                           AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
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
                                   new OCPIResponse.Builder(request) {
                                       StatusCode           = 1000,
                                       HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                           HTTPStatusCode             = HTTPStatusCode.Accepted,
                                           AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                       }
                                   }
                               );

                    }

                    return Task.FromResult(
                               new OCPIResponse.Builder(request) {
                                   StatusCode           = 2000,
                                   StatusMessage        = "Unknown 'CANCEL RESERVATION' command identification!",
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                       AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                   }
                               }
                           );

                }
            );

            #endregion

            #endregion

            #region ~/commands/START_SESSION/{command_id}

            #region OPTIONS  ~/commands/START_SESSION/{command_id}

            CommonAPI.AddOCPIMethod(

                HTTPMethod.OPTIONS,
                URLPathPrefix + "commands/START_SESSION/{command_id}",
                request =>

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

            #region POST     ~/commands/START_SESSION/{command_id}

            CommonAPI.AddOCPIMethod(

                HTTPMethod.POST,
                URLPathPrefix + "commands/START_SESSION/{command_id}",
                StartSessionCallbackRequest,
                StartSessionCallbackResponse,
                request => {

                    #region Check command identification

                    if (!request.ParseCommandId(CommonAPI,
                                                out var commandId,
                                                out var ocpiResponseBuilder))
                    {
                        return Task.FromResult(ocpiResponseBuilder);
                    }

                    #endregion

                    #region Parse command result JSON

                    if (!request.TryParseJObjectRequestBody(out var json, out ocpiResponseBuilder))
                        return Task.FromResult(ocpiResponseBuilder);

                    if (!CommandResult.TryParse(json,
                                                out var commandResult,
                                                out var errorResponse))
                    {

                        return Task.FromResult(
                                   new OCPIResponse.Builder(request) {
                                       StatusCode           = 2001,
                                       StatusMessage        = "Could not parse the given 'START SESSION' command result JSON: " + errorResponse,
                                       HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                           HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                           AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
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
                                   new OCPIResponse.Builder(request) {
                                       StatusCode           = 1000,
                                       HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                           HTTPStatusCode             = HTTPStatusCode.Accepted,
                                           AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                       }
                                   }
                               );

                    }

                    return Task.FromResult(
                               new OCPIResponse.Builder(request) {
                                   StatusCode           = 2000,
                                   StatusMessage        = "Unknown 'START SESSION' command identification!",
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                       AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                   }
                               }
                           );

                }
            );

            #endregion

            #endregion

            #region ~/commands/STOP_SESSION/{command_id}

            #region OPTIONS  ~/commands/STOP_SESSION/{command_id}

            CommonAPI.AddOCPIMethod(

                HTTPMethod.OPTIONS,
                URLPathPrefix + "commands/STOP_SESSION/{command_id}",
                request =>

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

            #region POST     ~/commands/STOP_SESSION/{command_id}

            CommonAPI.AddOCPIMethod(

                HTTPMethod.POST,
                URLPathPrefix + "commands/STOP_SESSION/{command_id}",
                StopSessionCallbackRequest,
                StopSessionCallbackResponse,
                request => {

                    #region Check command identification

                    if (!request.ParseCommandId(CommonAPI,
                                                out var commandId,
                                                out var ocpiResponseBuilder))
                    {
                        return Task.FromResult(ocpiResponseBuilder);
                    }

                    #endregion

                    #region Parse command result JSON

                    if (!request.TryParseJObjectRequestBody(out var json, out ocpiResponseBuilder))
                        return Task.FromResult(ocpiResponseBuilder);

                    if (!CommandResult.TryParse(json,
                                                out var commandResult,
                                                out var errorResponse))
                    {

                        return Task.FromResult(
                                   new OCPIResponse.Builder(request) {
                                       StatusCode           = 2001,
                                       StatusMessage        = "Could not parse the given 'STOP SESSION' command result JSON: " + errorResponse,
                                       HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                           HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                           AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
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
                                   new OCPIResponse.Builder(request) {
                                       StatusCode           = 1000,
                                       HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                           HTTPStatusCode             = HTTPStatusCode.Accepted,
                                           AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                       }
                                   }
                               );

                    }

                    return Task.FromResult(
                               new OCPIResponse.Builder(request) {
                                   StatusCode           = 2000,
                                   StatusMessage        = "Unknown 'STOP SESSION' command identification!",
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                       AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                   }
                               }
                           );

                }
            );

            #endregion

            #endregion

            #region ~/commands/UNLOCK_CONNECTOR/{command_id}

            #region OPTIONS  ~/commands/UNLOCK_CONNECTOR/{command_id}

            CommonAPI.AddOCPIMethod(

                HTTPMethod.OPTIONS,
                URLPathPrefix + "commands/UNLOCK_CONNECTOR/{command_id}",
                request =>

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

            #region POST     ~/commands/UNLOCK_CONNECTOR/{command_id}

            CommonAPI.AddOCPIMethod(

                HTTPMethod.POST,
                URLPathPrefix + "commands/UNLOCK_CONNECTOR/{command_id}",
                UnlockConnectorCallbackRequest,
                UnlockConnectorCallbackResponse,
                request => {

                    #region Check command identification

                    if (!request.ParseCommandId(CommonAPI,
                                                out var commandId,
                                                out var ocpiResponseBuilder))
                    {
                        return Task.FromResult(ocpiResponseBuilder);
                    }

                    #endregion

                    #region Parse command result JSON

                    if (!request.TryParseJObjectRequestBody(out var json, out ocpiResponseBuilder))
                        return Task.FromResult(ocpiResponseBuilder);

                    if (!CommandResult.TryParse(json,
                                                out var commandResult,
                                                out var errorResponse))
                    {

                        return Task.FromResult(
                                   new OCPIResponse.Builder(request) {
                                       StatusCode           = 2001,
                                       StatusMessage        = "Could not parse the given 'UNLOCK CONNECTOR' command result JSON: " + errorResponse,
                                       HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                           HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                           AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
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
                                   new OCPIResponse.Builder(request) {
                                       StatusCode           = 1000,
                                       HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                           HTTPStatusCode             = HTTPStatusCode.Accepted,
                                           AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                       }
                                   }
                               );

                    }

                    return Task.FromResult(
                               new OCPIResponse.Builder(request) {
                                   StatusCode           = 2000,
                                   StatusMessage        = "Unknown 'UNLOCK CONNECTOR' command identification!",
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                       AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                   }
                               }
                           );

                }
            );

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
