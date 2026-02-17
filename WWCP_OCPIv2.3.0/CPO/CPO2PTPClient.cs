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

using System.Runtime.CompilerServices;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;
using cloud.charging.open.protocols.OCPIv2_3_0;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_3_0.CPO.HTTP
{

    /// <summary>
    /// The CPO2PTP Client is used by CPOs to talk to PTPs.
    /// </summary>
    public partial class CPO2PTPClient : CommonHTTPClient
    {

        #region (class) APICounters

        public class APICounters : CommonAPICounters
        {

            #region Properties

            public APICounterValues  GetLocation           { get; }
            public APICounterValues  PutLocation           { get; }
            public APICounterValues  PatchLocation         { get; }

            public APICounterValues  GetEVSE               { get; }
            public APICounterValues  PutEVSE               { get; }
            public APICounterValues  PatchEVSE             { get; }

            public APICounterValues  GetConnector          { get; }
            public APICounterValues  PutConnector          { get; }
            public APICounterValues  PatchConnector        { get; }

            public APICounterValues  GetTariff             { get; }
            public APICounterValues  PutTariff             { get; }
            public APICounterValues  PatchTariff           { get; }
            public APICounterValues  DeleteTariff          { get; }

            public APICounterValues  GetSession            { get; }
            public APICounterValues  PutSession            { get; }
            public APICounterValues  PatchSession          { get; }
            public APICounterValues  DeleteSession         { get; }

            public APICounterValues  PostCDR               { get; }
            public APICounterValues  GetCDR                { get; }

            public APICounterValues  GetTokens             { get; }
            public APICounterValues  PostToken             { get; }

            public APICounterValues  SetChargingProfile    { get; }

            #endregion

            #region Constructor(s)

            public APICounters(APICounterValues?  GetVersions          = null,
                               APICounterValues?  Register             = null,

                               APICounterValues?  GetLocation          = null,
                               APICounterValues?  PutLocation          = null,
                               APICounterValues?  PatchLocation        = null,

                               APICounterValues?  GetEVSE              = null,
                               APICounterValues?  PutEVSE              = null,
                               APICounterValues?  PatchEVSE            = null,

                               APICounterValues?  GetConnector         = null,
                               APICounterValues?  PutConnector         = null,
                               APICounterValues?  PatchConnector       = null,

                               APICounterValues?  GetTariff            = null,
                               APICounterValues?  PutTariff            = null,
                               APICounterValues?  PatchTariff          = null,
                               APICounterValues?  DeleteTariff         = null,

                               APICounterValues?  GetSession           = null,
                               APICounterValues?  PutSession           = null,
                               APICounterValues?  PatchSession         = null,
                               APICounterValues?  DeleteSession        = null,

                               APICounterValues?  PostCDR              = null,
                               APICounterValues?  GetCDR               = null,

                               APICounterValues?  GetTokens            = null,
                               APICounterValues?  PostToken            = null,

                               APICounterValues?  SetChargingProfile   = null)

                : base(GetVersions,
                       Register)

            {

                this.GetLocation         = GetLocation        ?? new APICounterValues();
                this.PutLocation         = PutLocation        ?? new APICounterValues();
                this.PatchLocation       = PatchLocation      ?? new APICounterValues();

                this.GetEVSE             = GetEVSE            ?? new APICounterValues();
                this.PutEVSE             = PutEVSE            ?? new APICounterValues();
                this.PatchEVSE           = PatchEVSE          ?? new APICounterValues();

                this.GetConnector        = GetConnector       ?? new APICounterValues();
                this.PutConnector        = PutConnector       ?? new APICounterValues();
                this.PatchConnector      = PatchConnector     ?? new APICounterValues();

                this.GetTariff           = GetTariff          ?? new APICounterValues();
                this.PutTariff           = PutTariff          ?? new APICounterValues();
                this.PatchTariff         = PatchTariff        ?? new APICounterValues();
                this.DeleteTariff        = DeleteTariff       ?? new APICounterValues();

                this.GetSession          = GetSession         ?? new APICounterValues();
                this.PutSession          = PutSession         ?? new APICounterValues();
                this.PatchSession        = PatchSession       ?? new APICounterValues();
                this.DeleteSession       = DeleteSession      ?? new APICounterValues();

                this.PostCDR             = PostCDR            ?? new APICounterValues();
                this.GetCDR              = GetCDR             ?? new APICounterValues();

                this.GetTokens           = GetTokens          ?? new APICounterValues();
                this.PostToken           = PostToken          ?? new APICounterValues();

                this.SetChargingProfile  = SetChargingProfile ?? new APICounterValues();

            }

            #endregion


            #region ToJSON()

            public override JObject ToJSON()
            {

                var json = base.ToJSON();

                json.Add(new JProperty("getLocation",         GetLocation.       ToJSON()));
                json.Add(new JProperty("putLocation",         PutLocation.       ToJSON()));
                json.Add(new JProperty("patchLocation",       PatchLocation.     ToJSON()));

                json.Add(new JProperty("getEVSE",             GetEVSE.           ToJSON()));
                json.Add(new JProperty("putEVSE",             PutEVSE.           ToJSON()));
                json.Add(new JProperty("patchEVSE",           PatchEVSE.         ToJSON()));

                json.Add(new JProperty("getConnector",        GetConnector.      ToJSON()));
                json.Add(new JProperty("putConnector",        PutConnector.      ToJSON()));
                json.Add(new JProperty("patchConnector",      PatchConnector.    ToJSON()));

                json.Add(new JProperty("getTariff",           GetTariff.         ToJSON()));
                json.Add(new JProperty("putTariff",           PutTariff.         ToJSON()));
                json.Add(new JProperty("patchTariff",         PatchTariff.       ToJSON()));
                json.Add(new JProperty("deleteTariff",        DeleteTariff.      ToJSON()));

                json.Add(new JProperty("getSession",          GetSession.        ToJSON()));
                json.Add(new JProperty("putSession",          PutSession.        ToJSON()));
                json.Add(new JProperty("patchSession",        PatchSession.      ToJSON()));
                json.Add(new JProperty("deleteSession",       DeleteSession.     ToJSON()));

                json.Add(new JProperty("postCDR",             PostCDR.           ToJSON()));
                json.Add(new JProperty("getCDR",              GetCDR.            ToJSON()));

                json.Add(new JProperty("getTokens",           GetTokens.         ToJSON()));
                json.Add(new JProperty("postToken",           PostToken.         ToJSON()));

                json.Add(new JProperty("setChargingProfile",  SetChargingProfile.ToJSON()));

                return json;

            }

            #endregion

        }

        #endregion


        #region Data

        /// <summary>
        /// The default HTTP user agent.
        /// </summary>
        public new const String  DefaultHTTPUserAgent    = $"GraphDefined OCPI {Version.String} {nameof(CPO2PTPClient)}";

        /// <summary>
        /// The default logging context.
        /// </summary>
        public new const String  DefaultLoggingContext   = nameof(CPO2PTPClient);

        #endregion

        #region Properties

        public CPO_HTTPAPI           CPOAPI      { get; }

        /// <summary>
        /// CPO client event counters.
        /// </summary>
        public new APICounters  Counters    { get; }

        /// <summary>
        /// The attached HTTP client logger.
        /// </summary>
        public new Logger       HTTPLogger
        {
            get
            {
                return base.HTTPLogger as Logger;
            }
            set
            {
                base.HTTPLogger = value;
            }
        }

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
        public CustomJObjectSerializerDelegate<Parking>?                     CustomParkingSerializer                       { get; set; }
        public CustomJObjectSerializerDelegate<BusinessDetails>?             CustomBusinessDetailsSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<Hours>?                       CustomHoursSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<EVSEParking>?                 CustomEVSEParkingSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<Image>?                       CustomImageSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<EnergyMix>?                   CustomEnergyMixSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<EnergySource>?                CustomEnergySourceSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<EnvironmentalImpact>?         CustomEnvironmentalImpactSerializer           { get; set; }
        public CustomJObjectSerializerDelegate<Tariff>?                      CustomTariffSerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<PriceLimit>?                  CustomPriceLimitSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<TariffElement>?               CustomTariffElementSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<PriceComponent>?              CustomPriceComponentSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<TariffRestrictions>?          CustomTariffRestrictionsSerializer            { get; set; }
        public CustomJObjectSerializerDelegate<Session>?                     CustomSessionSerializer                       { get; set; }
        public CustomJObjectSerializerDelegate<CDRToken>?                    CustomCDRTokenSerializer                      { get; set; }
        public CustomJObjectSerializerDelegate<ChargingPeriod>?              CustomChargingPeriodSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<CDRDimension>?                CustomCDRDimensionSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<Price>?                       CustomPriceSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<TaxAmount>?                   CustomTaxAmountSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<CDR>?                         CustomCDRSerializer                           { get; set; }
        public CustomJObjectSerializerDelegate<CDRLocation>?                 CustomCDRLocationSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<SignedData>?                  CustomSignedDataSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<SignedValue>?                 CustomSignedValueSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<LocationReference>?           CustomLocationReferenceSerializer             { get; set; }
        public CustomJObjectSerializerDelegate<SetChargingProfileCommand>?   CustomSetChargingProfileSerializer            { get; set; }
        public CustomJObjectSerializerDelegate<ChargingProfile>?             CustomChargingProfileSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<ChargingProfilePeriod>?       CustomChargingProfilePeriodSerializer         { get; set; }

        #endregion

        #region Events

        #region OnGetLocationRequest/-Response

        /// <summary>
        /// An event fired whenever a request getting a location will be send.
        /// </summary>
        public event OnGetLocationRequestDelegate?   OnGetLocationRequest;

        /// <summary>
        /// An event fired whenever a HTTP request getting a location will be send.
        /// </summary>
        public event ClientRequestLogHandler?        OnGetLocationHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a getting a location HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler?       OnGetLocationHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a getting a location request had been received.
        /// </summary>
        public event OnGetLocationResponseDelegate?  OnGetLocationResponse;

        #endregion

        #region OnPutLocationRequest/-Response

        /// <summary>
        /// An event fired whenever a request putting a location will be send.
        /// </summary>
        public event OnPutLocationRequestDelegate?   OnPutLocationRequest;

        /// <summary>
        /// An event fired whenever a HTTP request putting a location will be send.
        /// </summary>
        public event ClientRequestLogHandler?        OnPutLocationHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a putting a location HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler?       OnPutLocationHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a putting a location request had been received.
        /// </summary>
        public event OnPutLocationResponseDelegate?  OnPutLocationResponse;

        #endregion

        #region OnPatchLocationRequest/-Response

        /// <summary>
        /// An event fired whenever a request patching a location will be send.
        /// </summary>
        public event OnPatchLocationRequestDelegate?   OnPatchLocationRequest;

        /// <summary>
        /// An event fired whenever a HTTP request patching a location will be send.
        /// </summary>
        public event ClientRequestLogHandler?          OnPatchLocationHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a patching a location HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler?         OnPatchLocationHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a patching a location request had been received.
        /// </summary>
        public event OnPatchLocationResponseDelegate?  OnPatchLocationResponse;

        #endregion


        #region OnGetEVSERequest/-Response

        /// <summary>
        /// An event fired whenever a request getting an EVSE will be send.
        /// </summary>
        public event OnGetEVSERequestDelegate?   OnGetEVSERequest;

        /// <summary>
        /// An event fired whenever a HTTP request getting an EVSE will be send.
        /// </summary>
        public event ClientRequestLogHandler?    OnGetEVSEHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a getting an EVSE HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler?   OnGetEVSEHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a getting an EVSE request had been received.
        /// </summary>
        public event OnGetEVSEResponseDelegate?  OnGetEVSEResponse;

        #endregion

        #region OnPutEVSERequest/-Response

        /// <summary>
        /// An event fired whenever a request putting an EVSE will be send.
        /// </summary>
        public event OnPutEVSERequestDelegate?   OnPutEVSERequest;

        /// <summary>
        /// An event fired whenever a HTTP request putting an EVSE will be send.
        /// </summary>
        public event ClientRequestLogHandler?    OnPutEVSEHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a putting an EVSE HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler?   OnPutEVSEHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a putting an EVSE request had been received.
        /// </summary>
        public event OnPutEVSEResponseDelegate?  OnPutEVSEResponse;

        #endregion

        #region OnPatchEVSERequest/-Response

        /// <summary>
        /// An event fired whenever a request patching an EVSE will be send.
        /// </summary>
        public event OnPatchEVSERequestDelegate?   OnPatchEVSERequest;

        /// <summary>
        /// An event fired whenever a HTTP request patching an EVSE will be send.
        /// </summary>
        public event ClientRequestLogHandler?      OnPatchEVSEHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a patching an EVSE HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler?     OnPatchEVSEHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a patching an EVSE request had been received.
        /// </summary>
        public event OnPatchEVSEResponseDelegate?  OnPatchEVSEResponse;

        #endregion


        #region OnGetConnectorRequest/-Response

        /// <summary>
        /// An event fired whenever a request getting a connector will be send.
        /// </summary>
        public event OnGetConnectorRequestDelegate?   OnGetConnectorRequest;

        /// <summary>
        /// An event fired whenever a HTTP request getting a connector will be send.
        /// </summary>
        public event ClientRequestLogHandler?         OnGetConnectorHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a getting a connector HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler?        OnGetConnectorHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a getting a connector request had been received.
        /// </summary>
        public event OnGetConnectorResponseDelegate?  OnGetConnectorResponse;

        #endregion

        #region OnPutConnectorRequest/-Response

        /// <summary>
        /// An event fired whenever a request putting a connector will be send.
        /// </summary>
        public event OnPutConnectorRequestDelegate?   OnPutConnectorRequest;

        /// <summary>
        /// An event fired whenever a HTTP request putting a connector will be send.
        /// </summary>
        public event ClientRequestLogHandler?         OnPutConnectorHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a putting a connector HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler?        OnPutConnectorHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a putting a connector request had been received.
        /// </summary>
        public event OnPutConnectorResponseDelegate?  OnPutConnectorResponse;

        #endregion

        #region OnPatchConnectorRequest/-Response

        /// <summary>
        /// An event fired whenever a request patching a connector will be send.
        /// </summary>
        public event OnPatchConnectorRequestDelegate?   OnPatchConnectorRequest;

        /// <summary>
        /// An event fired whenever a HTTP request patching a connector will be send.
        /// </summary>
        public event ClientRequestLogHandler?           OnPatchConnectorHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a patching a connector HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler?          OnPatchConnectorHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a patching a connector request had been received.
        /// </summary>
        public event OnPatchConnectorResponseDelegate?  OnPatchConnectorResponse;

        #endregion



        #region OnGetTariffRequest/-Response

        /// <summary>
        /// An event fired whenever a request getting a tariff will be send.
        /// </summary>
        public event OnGetTariffRequestDelegate?   OnGetTariffRequest;

        /// <summary>
        /// An event fired whenever a HTTP request getting a tariff will be send.
        /// </summary>
        public event ClientRequestLogHandler?      OnGetTariffHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a getting a tariff HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler?     OnGetTariffHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a getting a tariff request had been received.
        /// </summary>
        public event OnGetTariffResponseDelegate?  OnGetTariffResponse;

        #endregion

        #region OnPutTariffRequest/-Response

        /// <summary>
        /// An event fired whenever a request putting a tariff will be send.
        /// </summary>
        public event OnPutTariffRequestDelegate?   OnPutTariffRequest;

        /// <summary>
        /// An event fired whenever a HTTP request putting a tariff will be send.
        /// </summary>
        public event ClientRequestLogHandler?      OnPutTariffHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a putting a tariff HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler?     OnPutTariffHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a putting a tariff request had been received.
        /// </summary>
        public event OnPutTariffResponseDelegate?  OnPutTariffResponse;

        #endregion

        #region OnPatchTariffRequest/-Response

        /// <summary>
        /// An event fired whenever a request patching a tariff will be send.
        /// </summary>
        public event OnPatchTariffRequestDelegate?   OnPatchTariffRequest;

        /// <summary>
        /// An event fired whenever a HTTP request patching a tariff will be send.
        /// </summary>
        public event ClientRequestLogHandler?        OnPatchTariffHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a patching a tariff HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler?       OnPatchTariffHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a patching a tariff request had been received.
        /// </summary>
        public event OnPatchTariffResponseDelegate?  OnPatchTariffResponse;

        #endregion

        #region OnDeleteTariffRequest/-Response

        /// <summary>
        /// An event fired whenever a request deleting a tariff will be send.
        /// </summary>
        public event OnDeleteTariffRequestDelegate?   OnDeleteTariffRequest;

        /// <summary>
        /// An event fired whenever a HTTP request deleting a tariff will be send.
        /// </summary>
        public event ClientRequestLogHandler?         OnDeleteTariffHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a deleting a tariff HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler?        OnDeleteTariffHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a deleting a tariff request had been received.
        /// </summary>
        public event OnDeleteTariffResponseDelegate?  OnDeleteTariffResponse;

        #endregion



        #region OnGetSessionRequest/-Response

        /// <summary>
        /// An event fired whenever a request getting a session will be send.
        /// </summary>
        public event OnGetSessionRequestDelegate?   OnGetSessionRequest;

        /// <summary>
        /// An event fired whenever a HTTP request getting a session will be send.
        /// </summary>
        public event ClientRequestLogHandler?        OnGetSessionHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a getting a session HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler?       OnGetSessionHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a getting a session request had been received.
        /// </summary>
        public event OnGetSessionResponseDelegate?  OnGetSessionResponse;

        #endregion

        #region OnPutSessionRequest/-Response

        /// <summary>
        /// An event fired whenever a request putting a session will be send.
        /// </summary>
        public event OnPutSessionRequestDelegate?   OnPutSessionRequest;

        /// <summary>
        /// An event fired whenever a HTTP request putting a session will be send.
        /// </summary>
        public event ClientRequestLogHandler?       OnPutSessionHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a putting a session HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler?      OnPutSessionHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a putting a session request had been received.
        /// </summary>
        public event OnPutSessionResponseDelegate?  OnPutSessionResponse;

        #endregion

        #region OnPatchSessionRequest/-Response

        /// <summary>
        /// An event fired whenever a request patching a session will be send.
        /// </summary>
        public event OnPatchSessionRequestDelegate?   OnPatchSessionRequest;

        /// <summary>
        /// An event fired whenever a HTTP request patching a session will be send.
        /// </summary>
        public event ClientRequestLogHandler?         OnPatchSessionHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a patching a session HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler?        OnPatchSessionHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a patching a session request had been received.
        /// </summary>
        public event OnPatchSessionResponseDelegate?  OnPatchSessionResponse;

        #endregion

        #region OnDeleteSessionRequest/-Response

        /// <summary>
        /// An event fired whenever a request deleting a session will be send.
        /// </summary>
        public event OnDeleteSessionRequestDelegate?   OnDeleteSessionRequest;

        /// <summary>
        /// An event fired whenever a HTTP request deleting a session will be send.
        /// </summary>
        public event ClientRequestLogHandler?          OnDeleteSessionHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a deleting a session HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler?         OnDeleteSessionHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a deleting a session request had been received.
        /// </summary>
        public event OnDeleteSessionResponseDelegate?  OnDeleteSessionResponse;

        #endregion



        #region OnGetCDRRequest/-Response

        /// <summary>
        /// An event fired whenever a request getting a charge detail record will be send.
        /// </summary>
        public event OnGetCDRRequestDelegate?   OnGetCDRRequest;

        /// <summary>
        /// An event fired whenever a HTTP request getting a charge detail record will be send.
        /// </summary>
        public event ClientRequestLogHandler?   OnGetCDRHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a getting a charge detail record HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler?  OnGetCDRHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a getting a charge detail record request had been received.
        /// </summary>
        public event OnGetCDRResponseDelegate?  OnGetCDRResponse;

        #endregion

        #region OnPostCDRRequest/-Response

        /// <summary>
        /// An event fired whenever a request posting a charge detail record will be send.
        /// </summary>
        public event OnPostCDRRequestDelegate?   OnPostCDRRequest;

        /// <summary>
        /// An event fired whenever a HTTP request posting a charge detail record will be send.
        /// </summary>
        public event ClientRequestLogHandler?    OnPostCDRHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a posting a charge detail record HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler?   OnPostCDRHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a posting a charge detail record request had been received.
        /// </summary>
        public event OnPostCDRResponseDelegate?  OnPostCDRResponse;

        #endregion



        #region OnGetTokensRequest/-Response

        /// <summary>
        /// An event fired whenever a request getting tokens will be send.
        /// </summary>
        public event OnGetTokensRequestDelegate?   OnGetTokensRequest;

        /// <summary>
        /// An event fired whenever a HTTP request getting tokens will be send.
        /// </summary>
        public event ClientRequestLogHandler?      OnGetTokensHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a getting tokens HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler?     OnGetTokensHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a getting tokens request had been received.
        /// </summary>
        public event OnGetTokensResponseDelegate?  OnGetTokensResponse;

        #endregion

        #region OnPostTokenRequest/-Response

        /// <summary>
        /// An event fired whenever a request posting a token will be send.
        /// </summary>
        public event OnPostTokenRequestDelegate?   OnPostTokenRequest;

        /// <summary>
        /// An event fired whenever a HTTP request posting a token will be send.
        /// </summary>
        public event ClientRequestLogHandler?      OnPostTokenHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a posting a token HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler?     OnPostTokenHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a posting a token request had been received.
        /// </summary>
        public event OnPostTokenResponseDelegate?  OnPostTokenResponse;

        #endregion


        #region OnSetChargingProfileRequest/-Response

        /// <summary>
        /// An event fired whenever a request setting a charging profile will be send.
        /// </summary>
        public event OnSetChargingProfileRequestDelegate?   OnSetChargingProfileRequest;

        /// <summary>
        /// An event fired whenever a HTTP request setting a charging profile will be send.
        /// </summary>
        public event ClientRequestLogHandler?               OnSetChargingProfileHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a set charging profile HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler?              OnSetChargingProfileHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a set charging profile request had been received.
        /// </summary>
        public event OnSetChargingProfileResponseDelegate?  OnSetChargingProfileResponse;

        #endregion

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new CPO2PTP client.
        /// </summary>
        /// <param name="CPOAPI">The CPOAPI.</param>
        /// <param name="RemoteParty">The remote party.</param>
        /// <param name="VirtualHostname">An optional HTTP virtual hostname.</param>
        /// <param name="Description">An optional description of this CPO client.</param>
        /// <param name="DisableLogging">Disable all logging.</param>
        /// <param name="LoggingPath">The logging path.</param>
        /// <param name="LoggingContext">An optional context for logging.</param>
        /// <param name="LogfileCreator">A delegate to create a log file from the given context and log file name.</param>
        /// <param name="DNSClient">The DNS client to use.</param>
        public CPO2PTPClient(CPO_HTTPAPI                       CPOAPI,
                             RemoteParty                  RemoteParty,
                             HTTPHostname?                VirtualHostname   = null,
                             I18NString?                  Description       = null,
                             org.GraphDefined.Vanaheimr.Hermod.HTTP.HTTPClientLogger?            HTTPLogger        = null,

                             Boolean?                     DisableLogging    = false,
                             String?                      LoggingPath       = null,
                             String?                      LoggingContext    = null,
                             OCPILogfileCreatorDelegate?  LogfileCreator    = null,
                             IDNSClient?                  DNSClient         = null)

            : base(CPOAPI.CommonAPI,
                   RemoteParty,
                   VirtualHostname,
                   Description,
                   HTTPLogger,

                   DisableLogging,
                   LoggingPath,
                   LoggingContext,
                   LogfileCreator,
                   DNSClient)

        {

            this.CPOAPI      = CPOAPI;
            this.Counters    = new APICounters();

            base.HTTPLogger  = this.DisableLogging == false
                                   ? new Logger(
                                         this,
                                         LoggingPath,
                                         LoggingContext,
                                         LogfileCreator
                                     )
                                   : null;

        }

        #endregion


        public override JObject ToJSON()
            => base.ToJSON(nameof(CPO2PTPClient));






        #region (private) LogEvent (Logger, LogHandler, ...)

        private Task LogEvent<TDelegate>(TDelegate?                                         Logger,
                                         Func<TDelegate, Task>                              LogHandler,
                                         [CallerArgumentExpression(nameof(Logger))] String  EventName     = "",
                                         [CallerMemberName()]                       String  OCPICommand   = "")

            where TDelegate : Delegate

            => LogEvent(
                   nameof(CPO2PTPClient),
                   Logger,
                   LogHandler,
                   EventName,
                   OCPICommand
               );

        #endregion


        #region Dispose()

        /// <summary>
        /// Dispose this object.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
        }

        #endregion


    }

}
