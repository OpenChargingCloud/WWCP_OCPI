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

using System.Diagnostics;
using System.Runtime.CompilerServices;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2_1.CPO.HTTP
{

    /// <summary>
    /// The CPO2EMSP client is used by a CPO to talk to EMSPs (and SCSPs).
    /// </summary>
    public partial class CPO2EMSPClient : CommonClient
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
        public new const String  DefaultHTTPUserAgent    = $"GraphDefined OCPI {Version.String} {nameof(CPO2EMSPClient)}";

        /// <summary>
        /// The default logging context.
        /// </summary>
        public new const String  DefaultLoggingContext   = nameof(CPO2EMSPClient);

        #endregion

        #region Properties

        /// <summary>
        /// The EMSP identification of the remote party.
        /// </summary>
        public EMSP_Id          RemoteEMSPId    { get; }

        /// <summary>
        /// Our CPO API.
        /// </summary>
        public CPOAPI           CPOAPI          { get; }

        /// <summary>
        /// CPO client event counters.
        /// </summary>
        public new APICounters  Counters        { get; }

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
        /// Create a new CPO2EMSP client.
        /// </summary>
        /// <param name="CPOAPI">The CPO API.</param>
        /// <param name="RemoteParty">The remote party.</param>
        /// <param name="VirtualHostname">An optional HTTP virtual hostname.</param>
        /// <param name="Description">An optional description of this CPO client.</param>
        /// <param name="DisableLogging">Disable all logging.</param>
        /// <param name="LoggingPath">The logging path.</param>
        /// <param name="LoggingContext">An optional context for logging.</param>
        /// <param name="LogfileCreator">A delegate to create a log file from the given context and log file name.</param>
        /// <param name="DNSClient">The DNS client to use.</param>
        public CPO2EMSPClient(CPOAPI                       CPOAPI,
                              RemoteParty                  RemoteParty,
                              HTTPHostname?                VirtualHostname   = null,
                              I18NString?                  Description       = null,
                              HTTPClientLogger?            HTTPLogger        = null,

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

            this.RemoteEMSPId  = RemoteParty.Id.AsEMSPId;
            this.CPOAPI        = CPOAPI;
            this.Counters      = new APICounters();

            base.HTTPLogger    = this.DisableLogging == false
                                     ? new Logger(
                                           this,
                                           LoggingPath,
                                           LoggingContext,
                                           LogfileCreator
                                       )
                                     : null;

        }

        #endregion



        #region GetLocation        (CountryCode, PartyId, LocationId, ...)

        /// <summary>
        /// Get the charging location specified by the given location identification from the remote API.
        /// </summary>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional cancellation token to cancel this request.</param>
        public async Task<OCPIResponse<Location>>

            GetLocation(CountryCode        CountryCode,
                        Party_Id           PartyId,
                        Location_Id        LocationId,

                        Request_Id?        RequestId           = null,
                        Correlation_Id?    CorrelationId       = null,
                        Version_Id?        VersionId           = null,

                        DateTimeOffset?    RequestTimestamp    = null,
                        EventTracking_Id?  EventTrackingId     = null,
                        TimeSpan?          RequestTimeout      = null,
                        CancellationToken  CancellationToken   = default)

        {

            #region Init

            var requestId         = RequestId        ?? Request_Id.    NewRandom();
            var correlationId     = CorrelationId    ?? Correlation_Id.NewRandom();

            var requestTimestamp  = RequestTimestamp ?? Timestamp.Now;
            var eventTrackingId   = EventTrackingId  ?? EventTracking_Id.New;
            var requestTimeout    = RequestTimeout   ?? this.RequestTimeout;

            var startTime         = Timestamp.Now;
            var stopwatch         = Stopwatch.StartNew();

            Counters.GetLocation.IncRequests_OK();

            OCPIResponse<Location> response;

            #endregion

            #region Send OnGetLocationRequest event

            await LogEvent(
                      OnGetLocationRequest,
                      loggingDelegate => loggingDelegate.Invoke(
                          startTime,
                          this,
                          requestId,
                          correlationId,

                          CountryCode,
                          PartyId,
                          LocationId,

                          CancellationToken,
                          eventTrackingId,
                          requestTimeout
                      )
                  );

            #endregion


            try
            {

                var httpClient = await GetModuleHTTPClient(
                                           Module_Id.Locations,
                                           InterfaceRoles.RECEIVER,
                                           VersionId,
                                           eventTrackingId,
                                           CancellationToken
                                       );

                if (httpClient is not null)
                {

                    #region Upstream HTTP request...

                    var httpResponse = await httpClient.GET(
                                                 Path:                  httpClient.RemoteURL.Path + CountryCode.ToString() +
                                                                                                    PartyId.    ToString() +
                                                                                                    LocationId. ToString(),
                                                 Authentication:        TokenAuth,
                                                 RequestBuilder:        requestBuilder => {
                                                                            requestBuilder.Set("X-Request-ID",     requestId);
                                                                            requestBuilder.Set("X-Correlation-ID", correlationId);
                                                                        },
                                                 RequestLogDelegate:    OnGetLocationHTTPRequest,
                                                 ResponseLogDelegate:   OnGetLocationHTTPResponse,
                                                 EventTrackingId:       eventTrackingId,
                                                 //NumberOfRetry:         transmissionRetry,
                                                 RequestTimeout:        RequestTimeout ?? this.RequestTimeout,
                                                 CancellationToken:     CancellationToken).

                                             ConfigureAwait(false);

                    #endregion

                    response = OCPIResponse<Location>.ParseJObject(
                                   httpResponse,
                                   requestId,
                                   correlationId,
                                   json => Location.Parse(json)
                               );

                    Counters.GetLocation.IncResponses_OK();

                }

                else
                {
                    response = OCPIResponse<String, Location>.Error("No remote URL available!");
                    Counters.GetLocation.IncRequests_Error();
                }

            }

            catch (Exception e)
            {
                response = OCPIResponse<String, Location>.Exception(e);
                Counters.GetLocation.IncResponses_Error();
            }


            #region Send OnGetLocationResponse event

            stopwatch.Stop();
            var endtime = Timestamp.Now;

            await LogEvent(
                      OnGetLocationResponse,
                      loggingDelegate => loggingDelegate.Invoke(
                          endtime,
                          this,
                          requestId,
                          correlationId,

                          CountryCode,
                          PartyId,
                          LocationId,

                          CancellationToken,
                          eventTrackingId,
                          requestTimeout,

                          response,
                          stopwatch.Elapsed
                      )
                  );

            #endregion

            return response;

        }

        #endregion

        #region PutLocation        (Location, ...)

        /// <summary>
        /// Put/store the given charging location on/within the remote API.
        /// </summary>
        /// <param name="Location">The charging location to store/put at/onto the remote API.</param>
        /// 
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional cancellation token to cancel this request.</param>
        public async Task<OCPIResponse<Location>>

            PutLocation(Location           Location,
                        EMSP_Id?           EMSPId              = null,

                        Request_Id?        RequestId           = null,
                        Correlation_Id?    CorrelationId       = null,
                        Version_Id?        VersionId           = null,

                        DateTimeOffset?    RequestTimestamp    = null,
                        EventTracking_Id?  EventTrackingId     = null,
                        TimeSpan?          RequestTimeout      = null,
                        CancellationToken  CancellationToken   = default)

        {

            #region Init

            var requestId         = RequestId        ?? Request_Id.    NewRandom();
            var correlationId     = CorrelationId    ?? Correlation_Id.NewRandom();

            var requestTimestamp  = RequestTimestamp ?? Timestamp.Now;
            var eventTrackingId   = EventTrackingId  ?? EventTracking_Id.New;
            var requestTimeout    = RequestTimeout   ?? this.RequestTimeout;

            var startTime         = Timestamp.Now;
            var stopwatch         = Stopwatch.StartNew();

            Counters.PutLocation.IncRequests_OK();

            OCPIResponse<Location> response;

            #endregion

            #region Send OnPutLocationRequest event

            await LogEvent(
                      OnPutLocationRequest,
                      loggingDelegate => loggingDelegate.Invoke(
                          startTime,
                          this,
                          requestId,
                          correlationId,

                          Location,

                          CancellationToken,
                          eventTrackingId,
                          requestTimeout
                      )
                  );

            #endregion


            try
            {

                var httpClient = await GetModuleHTTPClient(
                                           Module_Id.Locations,
                                           InterfaceRoles.RECEIVER,
                                           VersionId,
                                           eventTrackingId,
                                           CancellationToken
                                       );

                if (httpClient is not null)
                {

                    #region Upstream HTTP request...

                    var httpResponse = await httpClient.PUT(
                                                 Path:                  httpClient.RemoteURL.Path + Location.CountryCode.ToString() +
                                                                                                    Location.PartyId.    ToString() +
                                                                                                    Location.Id.         ToString(),
                                                 Content:               Location.ToJSON(
                                                                            EMSPId,
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
                                                                        ).ToUTF8Bytes(JSONFormatting),
                                                 Authentication:        TokenAuth,
                                                 RequestBuilder:        requestBuilder => {
                                                                            requestBuilder.Set("X-Request-ID",     requestId);
                                                                            requestBuilder.Set("X-Correlation-ID", correlationId);
                                                                        },
                                                 RequestLogDelegate:    OnPutLocationHTTPRequest,
                                                 ResponseLogDelegate:   OnPutLocationHTTPResponse,
                                                 EventTrackingId:       eventTrackingId,
                                                 //NumberOfRetry:         transmissionRetry,
                                                 RequestTimeout:        RequestTimeout ?? this.RequestTimeout,
                                                 CancellationToken:     CancellationToken).

                                             ConfigureAwait(false);

                    #endregion

                    response = OCPIResponse<Location>.ParseJObject(
                                   httpResponse,
                                   requestId,
                                   correlationId,
                                   json => Location.Parse(json)
                               );

                    Counters.PutLocation.IncResponses_OK();

                }

                else
                {
                    response = OCPIResponse<String, Location>.Error("No remote URL available!");
                    Counters.PutLocation.IncRequests_Error();
                }

            }

            catch (Exception e)
            {
                response = OCPIResponse<String, Location>.Exception(e);
                Counters.PutLocation.IncResponses_Error();
            }


            #region Send OnPutLocationResponse event

            stopwatch.Stop();
            var endtime = Timestamp.Now;

            await LogEvent(
                      OnPutLocationResponse,
                      loggingDelegate => loggingDelegate.Invoke(
                          endtime,
                          this,
                          requestId,
                          correlationId,

                          Location,

                          CancellationToken,
                          eventTrackingId,
                          requestTimeout,

                          response,
                          stopwatch.Elapsed
                      )
                  );

            #endregion

            return response;

        }

        #endregion

        #region PatchLocation      (CountryCode, PartyId, LocationId, LocationPatch, ...)

        /// <summary>
        /// Patch a location.
        /// </summary>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional cancellation token to cancel this request.</param>
        public async Task<OCPIResponse<Location>>

            PatchLocation(CountryCode        CountryCode,
                          Party_Id           PartyId,
                          Location_Id        LocationId,
                          JObject            LocationPatch,

                          Request_Id?        RequestId           = null,
                          Correlation_Id?    CorrelationId       = null,
                          Version_Id?        VersionId           = null,

                          DateTimeOffset?    RequestTimestamp    = null,
                          EventTracking_Id?  EventTrackingId     = null,
                          TimeSpan?          RequestTimeout      = null,
                          CancellationToken  CancellationToken   = default)

        {

            #region Initial checks

            if (!LocationPatch.HasValues)
                return OCPIResponse<Location>.Error("The given location patch must not be empty!");

            #endregion

            #region Init

            var requestId         = RequestId        ?? Request_Id.    NewRandom();
            var correlationId     = CorrelationId    ?? Correlation_Id.NewRandom();

            var requestTimestamp  = RequestTimestamp ?? Timestamp.Now;
            var eventTrackingId   = EventTrackingId  ?? EventTracking_Id.New;
            var requestTimeout    = RequestTimeout   ?? this.RequestTimeout;

            var startTime         = Timestamp.Now;
            var stopwatch         = Stopwatch.StartNew();

            Counters.PatchLocation.IncRequests_OK();

            OCPIResponse<Location> response;

            #endregion

            #region Send OnPatchLocationRequest event

            await LogEvent(
                      OnPatchLocationRequest,
                      loggingDelegate => loggingDelegate.Invoke(
                          startTime,
                          this,
                          requestId,
                          correlationId,

                          LocationId,
                          LocationPatch,

                          CancellationToken,
                          eventTrackingId,
                          requestTimeout
                      )
                  );

            #endregion


            try
            {

                var httpClient = await GetModuleHTTPClient(
                                           Module_Id.Locations,
                                           InterfaceRoles.RECEIVER,
                                           VersionId,
                                           eventTrackingId,
                                           CancellationToken
                                       );

                if (httpClient is not null)
                {

                    #region Upstream HTTP request...

                    var httpResponse = await httpClient.PATCH(
                                                 Path:                  httpClient.RemoteURL.Path + CountryCode.ToString() +
                                                                                                    PartyId.    ToString() +
                                                                                                    LocationId. ToString(),
                                                 Content:               LocationPatch.ToUTF8Bytes(JSONFormatting),
                                                 Authentication:        TokenAuth,
                                                 RequestBuilder:        requestBuilder => {
                                                                            requestBuilder.Set("X-Request-ID",     requestId);
                                                                            requestBuilder.Set("X-Correlation-ID", correlationId);
                                                                        },
                                                 RequestLogDelegate:    OnPatchLocationHTTPRequest,
                                                 ResponseLogDelegate:   OnPatchLocationHTTPResponse,
                                                 EventTrackingId:       eventTrackingId,
                                                 //NumberOfRetry:         transmissionRetry,
                                                 RequestTimeout:        RequestTimeout ?? this.RequestTimeout,
                                                 CancellationToken:     CancellationToken).

                                             ConfigureAwait(false);

                    #endregion

                    response = OCPIResponse<Location>.ParseJObject(
                                   httpResponse,
                                   requestId,
                                   correlationId,
                                   json => Location.Parse(json)
                               );

                    Counters.PatchLocation.IncResponses_OK();

                }

                else
                {
                    response = OCPIResponse<String, Location>.Error("No remote URL available!");
                    Counters.PatchLocation.IncRequests_Error();
                }

            }

            catch (Exception e)
            {
                response = OCPIResponse<String, Location>.Exception(e);
                Counters.PatchLocation.IncResponses_Error();
            }


            #region Send OnPatchLocationResponse event

            stopwatch.Stop();
            var endtime = Timestamp.Now;

            await LogEvent(
                      OnPatchLocationResponse,
                      loggingDelegate => loggingDelegate.Invoke(
                          endtime,
                          this,
                          requestId,
                          correlationId,

                          LocationId,
                          LocationPatch,

                          CancellationToken,
                          eventTrackingId,
                          requestTimeout,

                          response,
                          stopwatch.Elapsed
                      )
                  );

            #endregion

            return response;

        }

        #endregion


        #region GetEVSE            (CountryCode, PartyId, LocationId, EVSEUId, ...)

        /// <summary>
        /// Get the EVSE specified by the given EVSE unique identification from the remote API.
        /// </summary>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional cancellation token to cancel this request.</param>
        public async Task<OCPIResponse<EVSE>>

            GetEVSE(CountryCode        CountryCode,
                    Party_Id           PartyId,
                    Location_Id        LocationId,
                    EVSE_UId           EVSEUId,

                    Request_Id?        RequestId           = null,
                    Correlation_Id?    CorrelationId       = null,
                    Version_Id?        VersionId           = null,

                    DateTimeOffset?    RequestTimestamp    = null,
                    EventTracking_Id?  EventTrackingId     = null,
                    TimeSpan?          RequestTimeout      = null,
                    CancellationToken  CancellationToken   = default)

        {

            #region Init

            var requestId         = RequestId        ?? Request_Id.    NewRandom();
            var correlationId     = CorrelationId    ?? Correlation_Id.NewRandom();

            var requestTimestamp  = RequestTimestamp ?? Timestamp.Now;
            var eventTrackingId   = EventTrackingId  ?? EventTracking_Id.New;
            var requestTimeout    = RequestTimeout   ?? this.RequestTimeout;

            var startTime         = Timestamp.Now;
            var stopwatch         = Stopwatch.StartNew();

            Counters.GetEVSE.IncRequests_OK();

            OCPIResponse<EVSE> response;

            #endregion

            #region Send OnGetEVSERequest event

            await LogEvent(
                      OnGetEVSERequest,
                      loggingDelegate => loggingDelegate.Invoke(
                          startTime,
                          this,
                          requestId,
                          correlationId,

                          LocationId,
                          EVSEUId,

                          CancellationToken,
                          eventTrackingId,
                          requestTimeout
                      )
                  );

            #endregion


            try
            {

                var httpClient = await GetModuleHTTPClient(
                                           Module_Id.Locations,
                                           InterfaceRoles.RECEIVER,
                                           VersionId,
                                           eventTrackingId,
                                           CancellationToken
                                       );

                if (httpClient is not null)
                {

                    #region Upstream HTTP request...

                    var httpResponse = await httpClient.GET(
                                                 Path:                  httpClient.RemoteURL.Path + CountryCode.ToString() +
                                                                                                    PartyId.    ToString() +
                                                                                                    LocationId. ToString() +
                                                                                                    EVSEUId.    ToString(),
                                                 Authentication:        TokenAuth,
                                                 RequestBuilder:        requestBuilder => {
                                                                            requestBuilder.Set("X-Request-ID",     requestId);
                                                                            requestBuilder.Set("X-Correlation-ID", correlationId);
                                                                        },
                                                 RequestLogDelegate:    OnGetEVSEHTTPRequest,
                                                 ResponseLogDelegate:   OnGetEVSEHTTPResponse,
                                                 EventTrackingId:       eventTrackingId,
                                                 //NumberOfRetry:         transmissionRetry,
                                                 RequestTimeout:        RequestTimeout ?? this.RequestTimeout,
                                                 CancellationToken:     CancellationToken).

                                             ConfigureAwait(false);

                    #endregion

                    response = OCPIResponse<EVSE>.ParseJObject(
                                   httpResponse,
                                   requestId,
                                   correlationId,
                                   json => EVSE.Parse(json)
                               );

                    Counters.GetEVSE.IncResponses_OK();

                }

                else
                {
                    response = OCPIResponse<String, EVSE>.Error("No remote URL available!");
                    Counters.GetEVSE.IncRequests_Error();
                }

            }

            catch (Exception e)
            {
                response = OCPIResponse<String, EVSE>.Exception(e);
                Counters.GetEVSE.IncResponses_Error();
            }


            #region Send OnGetEVSEResponse event

            stopwatch.Stop();
            var endtime = Timestamp.Now;

            await LogEvent(
                      OnGetEVSEResponse,
                      loggingDelegate => loggingDelegate.Invoke(
                          endtime,
                          this,
                          requestId,
                          correlationId,

                          LocationId,
                          EVSEUId,

                          CancellationToken,
                          eventTrackingId,
                          requestTimeout,

                          response,
                          stopwatch.Elapsed
                      )
                  );

            #endregion

            return response;

        }

        #endregion

        #region PutEVSE            (EVSE, ...)

        /// <summary>
        /// Put/store the given EVSE on/within the remote API.
        /// </summary>
        /// <param name="EVSE">The EVSE to store/put at/onto the remote API.</param>
        /// 
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional cancellation token to cancel this request.</param>
        public async Task<OCPIResponse<EVSE>>

            PutEVSE(EVSE               EVSE,
                    EMSP_Id?           EMSPId              = null,

                    Request_Id?        RequestId           = null,
                    Correlation_Id?    CorrelationId       = null,
                    Version_Id?        VersionId           = null,

                    DateTimeOffset?    RequestTimestamp    = null,
                    EventTracking_Id?  EventTrackingId     = null,
                    TimeSpan?          RequestTimeout      = null,
                    CancellationToken  CancellationToken   = default)

        {

            if (EVSE.ParentLocation is null)
                return OCPIResponse<EVSE>.Error("The parent location of the given EVSE must not be null!");

            return await PutEVSE(
                             EVSE,
                             EVSE.ParentLocation.CountryCode,
                             EVSE.ParentLocation.PartyId,
                             EVSE.ParentLocation.Id,
                             EMSPId,

                             RequestId,
                             CorrelationId,
                             VersionId,

                             RequestTimestamp,
                             EventTrackingId,
                             RequestTimeout,
                             CancellationToken
                         );

        }

        #endregion

        #region PutEVSE            (EVSE, CountryCode, PartyId, LocationId, ...)

        /// <summary>
        /// Put/store the given EVSE on/within the remote API.
        /// </summary>
        /// <param name="EVSE">The EVSE to store/put at/onto the remote API.</param>
        /// <param name="CountryCode">The country code of the location where to store the given EVSE.</param>
        /// <param name="PartyId">The party identification of the location where to store the given EVSE.</param>
        /// <param name="LocationId">The identification of the location where to store the given EVSE.</param>
        /// 
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional cancellation token to cancel this request.</param>
        public async Task<OCPIResponse<EVSE>>

            PutEVSE(EVSE               EVSE,
                    CountryCode        CountryCode,
                    Party_Id           PartyId,
                    Location_Id        LocationId,
                    EMSP_Id?           EMSPId              = null,

                    Request_Id?        RequestId           = null,
                    Correlation_Id?    CorrelationId       = null,
                    Version_Id?        VersionId           = null,

                    DateTimeOffset?    RequestTimestamp    = null,
                    EventTracking_Id?  EventTrackingId     = null,
                    TimeSpan?          RequestTimeout      = null,
                    CancellationToken  CancellationToken   = default)

        {

            #region Init

            var requestId         = RequestId        ?? Request_Id.    NewRandom();
            var correlationId     = CorrelationId    ?? Correlation_Id.NewRandom();

            var requestTimestamp  = RequestTimestamp ?? Timestamp.Now;
            var eventTrackingId   = EventTrackingId  ?? EventTracking_Id.New;
            var requestTimeout    = RequestTimeout   ?? this.RequestTimeout;

            var startTime         = Timestamp.Now;
            var stopwatch         = Stopwatch.StartNew();

            Counters.PutEVSE.IncRequests_OK();

            OCPIResponse<EVSE> response;

            #endregion

            #region Send OnPutEVSERequest event

            await LogEvent(
                      OnPutEVSERequest,
                      loggingDelegate => loggingDelegate.Invoke(
                          startTime,
                          this,
                          requestId,
                          correlationId,

                          EVSE,
                          CountryCode,
                          PartyId,
                          LocationId,

                          CancellationToken,
                          eventTrackingId,
                          requestTimeout
                      )
                  );

            #endregion


            try
            {

                var httpClient = await GetModuleHTTPClient(
                                           Module_Id.Locations,
                                           InterfaceRoles.RECEIVER,
                                           VersionId,
                                           eventTrackingId,
                                           CancellationToken
                                       );

                if (httpClient is not null)
                {

                    #region Upstream HTTP request...

                    var httpResponse = await httpClient.PUT(
                                                 Path:                  httpClient.RemoteURL.Path + CountryCode.ToString() +
                                                                                                    PartyId.    ToString() +
                                                                                                    LocationId. ToString() +
                                                                                                    EVSE.UId.   ToString(),
                                                 Content:               EVSE.ToJSON(
                                                                            EMSPId,
                                                                            CustomEVSESerializer,
                                                                            CustomStatusScheduleSerializer,
                                                                            CustomConnectorSerializer,
                                                                            CustomEVSEEnergyMeterSerializer,
                                                                            CustomTransparencySoftwareStatusSerializer,
                                                                            CustomTransparencySoftwareSerializer,
                                                                            CustomDisplayTextSerializer,
                                                                            CustomImageSerializer
                                                                        ).ToUTF8Bytes(JSONFormatting),
                                                 Authentication:        TokenAuth,
                                                 RequestBuilder:        requestBuilder => {
                                                                            requestBuilder.Set("X-Request-ID",     requestId);
                                                                            requestBuilder.Set("X-Correlation-ID", correlationId);
                                                                        },
                                                 RequestLogDelegate:    OnPutEVSEHTTPRequest,
                                                 ResponseLogDelegate:   OnPutEVSEHTTPResponse,
                                                 EventTrackingId:       eventTrackingId,
                                                 //NumberOfRetry:         transmissionRetry,
                                                 RequestTimeout:        RequestTimeout ?? this.RequestTimeout,
                                                 CancellationToken:     CancellationToken).

                                             ConfigureAwait(false);

                    #endregion

                    response = OCPIResponse<EVSE>.ParseJObject(
                                   httpResponse,
                                   requestId,
                                   correlationId,
                                   json => EVSE.Parse(json)
                               );

                    Counters.PutEVSE.IncResponses_OK();

                }

                else
                {
                    response = OCPIResponse<String, EVSE>.Error("No remote URL available!");
                    Counters.PutEVSE.IncRequests_Error();
                }

            }

            catch (Exception e)
            {
                response = OCPIResponse<String, EVSE>.Exception(e);
                Counters.PutEVSE.IncResponses_Error();
            }


            #region Send OnPutEVSEResponse event

            stopwatch.Stop();
            var endtime = Timestamp.Now;

            await LogEvent(
                      OnPutEVSEResponse,
                      loggingDelegate => loggingDelegate.Invoke(
                          endtime,
                          this,
                          requestId,
                          correlationId,

                          EVSE,
                          CountryCode,
                          PartyId,
                          LocationId,

                          CancellationToken,
                          eventTrackingId,
                          requestTimeout,

                          response,
                          stopwatch.Elapsed
                      )
                  );

            #endregion

            return response;

        }

        #endregion

        #region PatchEVSE          (CountryCode, PartyId, LocationId, EVSEUId, EVSEPatch, ...)

        /// <summary>
        /// Patch a location.
        /// </summary>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional cancellation token to cancel this request.</param>
        public async Task<OCPIResponse<EVSE>>

            PatchEVSE(CountryCode        CountryCode,
                      Party_Id           PartyId,
                      Location_Id        LocationId,
                      EVSE_UId           EVSEUId,
                      JObject            EVSEPatch,

                      Request_Id?        RequestId           = null,
                      Correlation_Id?    CorrelationId       = null,
                      Version_Id?        VersionId           = null,

                      DateTimeOffset?    RequestTimestamp    = null,
                      EventTracking_Id?  EventTrackingId     = null,
                      TimeSpan?          RequestTimeout      = null,
                      CancellationToken  CancellationToken   = default)

        {

            #region Initial checks

            if (!EVSEPatch.HasValues)
                return OCPIResponse<EVSE>.Error("The given EVSE patch must not be empty!");

            #endregion

            #region Init

            var requestId         = RequestId        ?? Request_Id.    NewRandom();
            var correlationId     = CorrelationId    ?? Correlation_Id.NewRandom();

            var requestTimestamp  = RequestTimestamp ?? Timestamp.Now;
            var eventTrackingId   = EventTrackingId  ?? EventTracking_Id.New;
            var requestTimeout    = RequestTimeout   ?? this.RequestTimeout;

            var startTime         = Timestamp.Now;
            var stopwatch         = Stopwatch.StartNew();

            Counters.PatchEVSE.IncRequests_OK();

            OCPIResponse<EVSE> response;

            #endregion

            #region Send OnPatchEVSERequest event

            await LogEvent(
                      OnPatchEVSERequest,
                      loggingDelegate => loggingDelegate.Invoke(
                          startTime,
                          this,
                          requestId,
                          correlationId,

                          CountryCode,
                          PartyId,
                          LocationId,
                          EVSEUId,
                          EVSEPatch,

                          CancellationToken,
                          eventTrackingId,
                          requestTimeout
                      )
                  );

            #endregion


            try
            {

                var httpClient = await GetModuleHTTPClient(
                                           Module_Id.Locations,
                                           InterfaceRoles.RECEIVER,
                                           VersionId,
                                           eventTrackingId,
                                           CancellationToken
                                       );

                if (httpClient is not null)
                {

                    #region Upstream HTTP request...

                    var httpResponse = await httpClient.PATCH(
                                                 Path:                  httpClient.RemoteURL.Path + CountryCode.ToString() +
                                                                                                    PartyId.    ToString() +
                                                                                                    LocationId. ToString() +
                                                                                                    EVSEUId.    ToString(),
                                                 Content:               EVSEPatch.ToUTF8Bytes(JSONFormatting),
                                                 Authentication:        TokenAuth,
                                                 RequestBuilder:        requestBuilder => {
                                                                            requestBuilder.Set("X-Request-ID",     requestId);
                                                                            requestBuilder.Set("X-Correlation-ID", correlationId);
                                                                        },
                                                 RequestLogDelegate:    OnPatchEVSEHTTPRequest,
                                                 ResponseLogDelegate:   OnPatchEVSEHTTPResponse,
                                                 EventTrackingId:       eventTrackingId,
                                                 //NumberOfRetry:         transmissionRetry,
                                                 RequestTimeout:        RequestTimeout ?? this.RequestTimeout,
                                                 CancellationToken:     CancellationToken).

                                             ConfigureAwait(false);

                    #endregion

                    response = OCPIResponse<EVSE>.ParseJObject(
                                   httpResponse,
                                   requestId,
                                   correlationId,
                                   json => EVSE.Parse(json)
                               );

                    Counters.PatchEVSE.IncResponses_OK();

                }

                else
                {
                    response = OCPIResponse<String, EVSE>.Error("No remote URL available!");
                    Counters.PatchEVSE.IncRequests_Error();
                }

            }

            catch (Exception e)
            {
                response = OCPIResponse<String, EVSE>.Exception(e);
                Counters.PatchEVSE.IncResponses_Error();
            }


            #region Send OnPatchEVSEResponse event

            stopwatch.Stop();
            var endtime = Timestamp.Now;

            await LogEvent(
                      OnPatchEVSEResponse,
                      loggingDelegate => loggingDelegate.Invoke(
                          endtime,
                          this,
                          requestId,
                          correlationId,

                          CountryCode,
                          PartyId,
                          LocationId,
                          EVSEUId,
                          EVSEPatch,

                          CancellationToken,
                          eventTrackingId,
                          requestTimeout,

                          response,
                          stopwatch.Elapsed
                      )
                  );

            #endregion

            return response;

        }

        #endregion


        #region GetConnector       (CountryCode, PartyId, LocationId, EVSEUId, ConnectorId, ...)

        /// <summary>
        /// Get the connector specified by the given connector identification from the remote API.
        /// </summary>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional cancellation token to cancel this request.</param>
        public async Task<OCPIResponse<Connector>>

            GetConnector(CountryCode        CountryCode,
                         Party_Id           PartyId,
                         Location_Id        LocationId,
                         EVSE_UId           EVSEUId,
                         Connector_Id       ConnectorId,

                         Request_Id?        RequestId           = null,
                         Correlation_Id?    CorrelationId       = null,
                         Version_Id?        VersionId           = null,

                         DateTimeOffset?    RequestTimestamp    = null,
                         EventTracking_Id?  EventTrackingId     = null,
                         TimeSpan?          RequestTimeout      = null,
                         CancellationToken  CancellationToken   = default)

        {

            #region Init

            var requestId         = RequestId        ?? Request_Id.    NewRandom();
            var correlationId     = CorrelationId    ?? Correlation_Id.NewRandom();

            var requestTimestamp  = RequestTimestamp ?? Timestamp.Now;
            var eventTrackingId   = EventTrackingId  ?? EventTracking_Id.New;
            var requestTimeout    = RequestTimeout   ?? this.RequestTimeout;

            var startTime         = Timestamp.Now;
            var stopwatch         = Stopwatch.StartNew();

            Counters.GetConnector.IncRequests_OK();

            OCPIResponse<Connector> response;

            #endregion

            #region Send OnGetConnectorRequest event

            await LogEvent(
                      OnGetConnectorRequest,
                      loggingDelegate => loggingDelegate.Invoke(
                          startTime,
                          this,
                          requestId,
                          correlationId,

                          CountryCode,
                          PartyId,
                          LocationId,
                          EVSEUId,
                          ConnectorId,

                          CancellationToken,
                          eventTrackingId,
                          requestTimeout
                      )
                  );

            #endregion


            try
            {

                var httpClient = await GetModuleHTTPClient(
                                           Module_Id.Locations,
                                           InterfaceRoles.RECEIVER,
                                           VersionId,
                                           eventTrackingId,
                                           CancellationToken
                                       );

                if (httpClient is not null)
                {

                    #region Upstream HTTP request...

                    var httpResponse = await httpClient.GET(
                                                 Path:                  httpClient.RemoteURL.Path + CountryCode.ToString() +
                                                                                                    PartyId.    ToString() +
                                                                                                    LocationId. ToString() +
                                                                                                    EVSEUId.    ToString() +
                                                                                                    ConnectorId.ToString(),
                                                 Authentication:        TokenAuth,
                                                 RequestBuilder:        requestBuilder => {
                                                                            requestBuilder.Set("X-Request-ID",     requestId);
                                                                            requestBuilder.Set("X-Correlation-ID", correlationId);
                                                                        },
                                                 RequestLogDelegate:    OnGetConnectorHTTPRequest,
                                                 ResponseLogDelegate:   OnGetConnectorHTTPResponse,
                                                 EventTrackingId:       eventTrackingId,
                                                 //NumberOfRetry:         transmissionRetry,
                                                 RequestTimeout:        RequestTimeout ?? this.RequestTimeout,
                                                 CancellationToken:     CancellationToken).

                                             ConfigureAwait(false);

                    #endregion

                    response = OCPIResponse<Connector>.ParseJObject(
                                   httpResponse,
                                   requestId,
                                   correlationId,
                                   json => Connector.Parse(json)
                               );

                    Counters.GetConnector.IncResponses_OK();

                }

                else
                {
                    response = OCPIResponse<String, Connector>.Error("No remote URL available!");
                    Counters.GetConnector.IncRequests_Error();
                }

            }

            catch (Exception e)
            {
                response = OCPIResponse<String, Connector>.Exception(e);
                Counters.GetConnector.IncResponses_Error();
            }


            #region Send OnGetConnectorResponse event

            stopwatch.Stop();
            var endtime = Timestamp.Now;

            await LogEvent(
                      OnGetConnectorResponse,
                      loggingDelegate => loggingDelegate.Invoke(
                          endtime,
                          this,
                          requestId,
                          correlationId,

                          CountryCode,
                          PartyId,
                          LocationId,
                          EVSEUId,
                          ConnectorId,

                          CancellationToken,
                          eventTrackingId,
                          requestTimeout,

                          response,
                          stopwatch.Elapsed
                      )
                  );

            #endregion

            return response;

        }

        #endregion

        #region PutConnector       (Connector, ...)

        /// <summary>
        /// Put/store the given charging connector on/within the remote API.
        /// </summary>
        /// <param name="Connector">The connector to store/put at/onto the remote API.</param>
        /// 
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional cancellation token to cancel this request.</param>
        public async Task<OCPIResponse<Connector>>

            PutConnector(Connector          Connector,
                         EMSP_Id?           EMSPId              = null,

                         Request_Id?        RequestId           = null,
                         Correlation_Id?    CorrelationId       = null,
                         Version_Id?        VersionId           = null,

                         DateTimeOffset?    RequestTimestamp    = null,
                         EventTracking_Id?  EventTrackingId     = null,
                         TimeSpan?          RequestTimeout      = null,
                         CancellationToken  CancellationToken   = default)

        {

            #region Initial checks

            if (Connector.ParentEVSE is null)
                return OCPIResponse<String, Connector>.Error("The parent EVSE of the connector must not be null!");

            if (Connector.ParentEVSE.ParentLocation is null)
                return OCPIResponse<String, Connector>.Error("The parent location of the connector must not be null!");

            #endregion

            #region Init

            var requestId         = RequestId        ?? Request_Id.    NewRandom();
            var correlationId     = CorrelationId    ?? Correlation_Id.NewRandom();

            var requestTimestamp  = RequestTimestamp ?? Timestamp.Now;
            var eventTrackingId   = EventTrackingId  ?? EventTracking_Id.New;
            var requestTimeout    = RequestTimeout   ?? this.RequestTimeout;

            var startTime         = Timestamp.Now;
            var stopwatch         = Stopwatch.StartNew();

            Counters.PutConnector.IncRequests_OK();

            OCPIResponse<Connector> response;

            #endregion

            #region Send OnPutConnectorRequest event

            await LogEvent(
                      OnPutConnectorRequest,
                      loggingDelegate => loggingDelegate.Invoke(
                          startTime,
                          this,
                          requestId,
                          correlationId,

                          Connector,

                          CancellationToken,
                          eventTrackingId,
                          requestTimeout
                      )
                  );

            #endregion


            try
            {

                var httpClient = await GetModuleHTTPClient(
                                           Module_Id.Locations,
                                           InterfaceRoles.RECEIVER,
                                           VersionId,
                                           eventTrackingId,
                                           CancellationToken
                                       );

                if (httpClient is not null)
                {

                    #region Upstream HTTP request...

                    var httpResponse = await httpClient.PUT(
                                                 Path:                  httpClient.RemoteURL.Path + Connector.ParentEVSE.ParentLocation.CountryCode.ToString() +
                                                                                                    Connector.ParentEVSE.ParentLocation.PartyId.    ToString() +
                                                                                                    Connector.ParentEVSE.ParentLocation.Id.         ToString() +
                                                                                                    Connector.ParentEVSE.               UId.        ToString() +
                                                                                                    Connector.                          Id.         ToString(),
                                                 Content:               Connector.ToJSON(
                                                                            EMSPId,
                                                                            CustomConnectorSerializer
                                                                        ).ToUTF8Bytes(JSONFormatting),
                                                 Authentication:        TokenAuth,
                                                 RequestBuilder:        requestBuilder => {
                                                                            requestBuilder.Set("X-Request-ID",     requestId);
                                                                            requestBuilder.Set("X-Correlation-ID", correlationId);
                                                                        },
                                                 RequestLogDelegate:    OnPutConnectorHTTPRequest,
                                                 ResponseLogDelegate:   OnPutConnectorHTTPResponse,
                                                 EventTrackingId:       eventTrackingId,
                                                 //NumberOfRetry:         transmissionRetry,
                                                 RequestTimeout:        RequestTimeout ?? this.RequestTimeout,
                                                 CancellationToken:     CancellationToken).

                                             ConfigureAwait(false);

                    #endregion

                    response = OCPIResponse<Connector>.ParseJObject(
                                   httpResponse,
                                   requestId,
                                   correlationId,
                                   json => Connector.Parse(json)
                               );

                    Counters.PutConnector.IncResponses_OK();

                }

                else
                {
                    response = OCPIResponse<String, Connector>.Error("No remote URL available!");
                    Counters.PutConnector.IncRequests_Error();
                }

            }

            catch (Exception e)
            {
                response = OCPIResponse<String, Connector>.Exception(e);
                Counters.PutConnector.IncResponses_Error();
            }


            #region Send OnPutConnectorResponse event

            stopwatch.Stop();
            var endtime = Timestamp.Now;

            await LogEvent(
                      OnPutConnectorResponse,
                      loggingDelegate => loggingDelegate.Invoke(
                          endtime,
                          this,
                          requestId,
                          correlationId,

                          Connector,

                          CancellationToken,
                          eventTrackingId,
                          requestTimeout,

                          response,
                          stopwatch.Elapsed
                      )
                  );

            #endregion

            return response;

        }

        #endregion

        #region PatchConnector     (CountryCode, PartyId, LocationId, EVSEUId, ConnectorId, ConnectorPatch, ...)

        /// <summary>
        /// Patch a connector.
        /// </summary>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional cancellation token to cancel this request.</param>
        public async Task<OCPIResponse<Connector>>

            PatchConnector(CountryCode        CountryCode,
                           Party_Id           PartyId,
                           Location_Id        LocationId,
                           EVSE_UId           EVSEUId,
                           Connector_Id       ConnectorId,
                           JObject            ConnectorPatch,

                           Request_Id?        RequestId           = null,
                           Correlation_Id?    CorrelationId       = null,
                           Version_Id?        VersionId           = null,

                           DateTimeOffset?    RequestTimestamp    = null,
                           EventTracking_Id?  EventTrackingId     = null,
                           TimeSpan?          RequestTimeout      = null,
                           CancellationToken  CancellationToken   = default)

        {

            #region Init

            var requestId         = RequestId        ?? Request_Id.    NewRandom();
            var correlationId     = CorrelationId    ?? Correlation_Id.NewRandom();

            var requestTimestamp  = RequestTimestamp ?? Timestamp.Now;
            var eventTrackingId   = EventTrackingId  ?? EventTracking_Id.New;
            var requestTimeout    = RequestTimeout   ?? this.RequestTimeout;

            var startTime         = Timestamp.Now;
            var stopwatch         = Stopwatch.StartNew();

            Counters.PatchConnector.IncRequests_OK();

            OCPIResponse<Connector> response;

            #endregion

            #region Send OnPatchConnectorRequest event

            await LogEvent(
                      OnPatchConnectorRequest,
                      loggingDelegate => loggingDelegate.Invoke(
                          startTime,
                          this,
                          requestId,
                          correlationId,

                          CountryCode,
                          PartyId,
                          LocationId,
                          EVSEUId,
                          ConnectorId,
                          ConnectorPatch,

                          CancellationToken,
                          eventTrackingId,
                          requestTimeout
                      )
                  );

            #endregion


            try
            {

                var httpClient = await GetModuleHTTPClient(
                                           Module_Id.Locations,
                                           InterfaceRoles.RECEIVER,
                                           VersionId,
                                           eventTrackingId,
                                           CancellationToken
                                       );

                if (httpClient is not null)
                {

                    #region Upstream HTTP request...

                    var httpResponse = await httpClient.PATCH(
                                                 Path:                  httpClient.RemoteURL.Path + CountryCode.ToString() +
                                                                                                    PartyId.    ToString() +
                                                                                                    LocationId. ToString() +
                                                                                                    EVSEUId.    ToString() +
                                                                                                    ConnectorId.ToString(),
                                                 Content:               ConnectorPatch.ToUTF8Bytes(JSONFormatting),
                                                 Authentication:        TokenAuth,
                                                 RequestBuilder:        requestBuilder => {
                                                                            requestBuilder.Set("X-Request-ID",     requestId);
                                                                            requestBuilder.Set("X-Correlation-ID", correlationId);
                                                                        },
                                                 RequestLogDelegate:    OnPatchConnectorHTTPRequest,
                                                 ResponseLogDelegate:   OnPatchConnectorHTTPResponse,
                                                 EventTrackingId:       eventTrackingId,
                                                 //NumberOfRetry:         transmissionRetry,
                                                 RequestTimeout:        RequestTimeout ?? this.RequestTimeout,
                                                 CancellationToken:     CancellationToken).

                                             ConfigureAwait(false);

                    #endregion

                    response = OCPIResponse<Connector>.ParseJObject(
                                   httpResponse,
                                   requestId,
                                   correlationId,
                                   json => Connector.Parse(json)
                               );

                    Counters.PatchConnector.IncResponses_OK();

                }

                else
                {
                    response = OCPIResponse<String, Connector>.Error("No remote URL available!");
                    Counters.PatchConnector.IncRequests_Error();
                }

            }

            catch (Exception e)
            {
                response = OCPIResponse<String, Connector>.Exception(e);
                Counters.PatchConnector.IncResponses_Error();
            }


            #region Send OnPatchConnectorResponse event

            stopwatch.Stop();
            var endtime = Timestamp.Now;

            await LogEvent(
                      OnPatchConnectorResponse,
                      loggingDelegate => loggingDelegate.Invoke(
                          endtime,
                          this,
                          requestId,
                          correlationId,

                          CountryCode,
                          PartyId,
                          LocationId,
                          EVSEUId,
                          ConnectorId,
                          ConnectorPatch,

                          CancellationToken,
                          eventTrackingId,
                          requestTimeout,

                          response,
                          stopwatch.Elapsed
                      )
                  );

            #endregion

            return response;

        }

        #endregion


        #region GetTariff          (CountryCode, PartyId, TariffId, ...)

        /// <summary>
        /// Get the charging tariff specified by the given tariff identification from the remote API.
        /// </summary>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional cancellation token to cancel this request.</param>
        public async Task<OCPIResponse<Tariff>>

            GetTariff(CountryCode        CountryCode,
                      Party_Id           PartyId,
                      Tariff_Id          TariffId,

                      Request_Id?        RequestId           = null,
                      Correlation_Id?    CorrelationId       = null,
                      Version_Id?        VersionId           = null,

                      DateTimeOffset?    RequestTimestamp    = null,
                      EventTracking_Id?  EventTrackingId     = null,
                      TimeSpan?          RequestTimeout      = null,
                      CancellationToken  CancellationToken   = default)

        {

            #region Init

            var requestId         = RequestId        ?? Request_Id.    NewRandom();
            var correlationId     = CorrelationId    ?? Correlation_Id.NewRandom();

            var requestTimestamp  = RequestTimestamp ?? Timestamp.Now;
            var eventTrackingId   = EventTrackingId  ?? EventTracking_Id.New;
            var requestTimeout    = RequestTimeout   ?? this.RequestTimeout;

            var startTime         = Timestamp.Now;
            var stopwatch         = Stopwatch.StartNew();

            Counters.GetTariff.IncRequests_OK();

            OCPIResponse<Tariff> response;

            #endregion

            #region Send OnGetTariffRequest event

            await LogEvent(
                      OnGetTariffRequest,
                      loggingDelegate => loggingDelegate.Invoke(
                          startTime,
                          this,
                          requestId,
                          correlationId,

                          CountryCode,
                          PartyId,
                          TariffId,

                          CancellationToken,
                          eventTrackingId,
                          requestTimeout
                      )
                  );

            #endregion


            try
            {

                var httpClient = await GetModuleHTTPClient(
                                           Module_Id.Tariffs,
                                           InterfaceRoles.RECEIVER,
                                           VersionId,
                                           eventTrackingId,
                                           CancellationToken
                                       );

                if (httpClient is not null)
                {

                    #region Upstream HTTP request...

                    var httpResponse = await httpClient.GET(
                                                 Path:                  httpClient.RemoteURL.Path + CountryCode.ToString() +
                                                                                                    PartyId.    ToString() +
                                                                                                    TariffId.   ToString(),
                                                 Authentication:        TokenAuth,
                                                 RequestBuilder:        requestBuilder => {
                                                                            requestBuilder.Set("X-Request-ID",     requestId);
                                                                            requestBuilder.Set("X-Correlation-ID", correlationId);
                                                                        },
                                                 RequestLogDelegate:    OnGetTariffHTTPRequest,
                                                 ResponseLogDelegate:   OnGetTariffHTTPResponse,
                                                 EventTrackingId:       eventTrackingId,
                                                 //NumberOfRetry:         transmissionRetry,
                                                 RequestTimeout:        RequestTimeout ?? this.RequestTimeout,
                                                 CancellationToken:     CancellationToken).

                                             ConfigureAwait(false);

                    #endregion

                    response = OCPIResponse<Tariff>.ParseJObject(
                                   httpResponse,
                                   requestId,
                                   correlationId,
                                   json => Tariff.Parse(json)
                               );

                    Counters.GetTariff.IncResponses_OK();

                }

                else
                {
                    response = OCPIResponse<String, Tariff>.Error("No remote URL available!");
                    Counters.GetTariff.IncRequests_Error();
                }

            }

            catch (Exception e)
            {
                response = OCPIResponse<String, Tariff>.Exception(e);
                Counters.GetTariff.IncResponses_Error();
            }


            #region Send OnGetTariffResponse event

            stopwatch.Stop();
            var endtime = Timestamp.Now;

            await LogEvent(
                      OnGetTariffResponse,
                      loggingDelegate => loggingDelegate.Invoke(
                          endtime,
                          this,
                          requestId,
                          correlationId,

                          CountryCode,
                          PartyId,
                          TariffId,

                          CancellationToken,
                          eventTrackingId,
                          requestTimeout,

                          response,
                          stopwatch.Elapsed
                      )
                  );

            #endregion

            return response;

        }

        #endregion

        #region PutTariff          (Tariff, ...)

        /// <summary>
        /// Put/store the given charging tariff on/within the remote API.
        /// </summary>
        /// <param name="Tariff">The charging tariff to store/put at/onto the remote API.</param>
        /// 
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional cancellation token to cancel this request.</param>
        public async Task<OCPIResponse<Tariff>>

            PutTariff(Tariff             Tariff,

                      Request_Id?        RequestId           = null,
                      Correlation_Id?    CorrelationId       = null,
                      Version_Id?        VersionId           = null,

                      DateTimeOffset?    RequestTimestamp    = null,
                      EventTracking_Id?  EventTrackingId     = null,
                      TimeSpan?          RequestTimeout      = null,
                      CancellationToken  CancellationToken   = default)

        {

            #region Init

            var requestId         = RequestId        ?? Request_Id.    NewRandom();
            var correlationId     = CorrelationId    ?? Correlation_Id.NewRandom();

            var requestTimestamp  = RequestTimestamp ?? Timestamp.Now;
            var eventTrackingId   = EventTrackingId  ?? EventTracking_Id.New;
            var requestTimeout    = RequestTimeout   ?? this.RequestTimeout;

            var startTime         = Timestamp.Now;
            var stopwatch         = Stopwatch.StartNew();

            Counters.PutTariff.IncRequests_OK();

            OCPIResponse<Tariff> response;

            #endregion

            #region Send OnPutTariffRequest event

            await LogEvent(
                      OnPutTariffRequest,
                      loggingDelegate => loggingDelegate.Invoke(
                          startTime,
                          this,
                          requestId,
                          correlationId,

                          Tariff,

                          CancellationToken,
                          eventTrackingId,
                          requestTimeout
                      )
                  );

            #endregion


            try
            {

                var httpClient = await GetModuleHTTPClient(
                                           Module_Id.Tariffs,
                                           InterfaceRoles.RECEIVER,
                                           VersionId,
                                           eventTrackingId,
                                           CancellationToken
                                       );

                if (httpClient is not null)
                {

                    #region Upstream HTTP request...

                    var httpResponse = await httpClient.PUT(
                                                 Path:                  httpClient.RemoteURL.Path + Tariff.CountryCode.ToString() +
                                                                                                    Tariff.PartyId.    ToString() +
                                                                                                    Tariff.Id.         ToString(),
                                                 Content:               Tariff.ToJSON(
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
                                                                        ).ToUTF8Bytes(JSONFormatting),
                                                 Authentication:        TokenAuth,
                                                 RequestBuilder:        requestBuilder => {
                                                                            requestBuilder.Set("X-Request-ID",     requestId);
                                                                            requestBuilder.Set("X-Correlation-ID", correlationId);
                                                                        },
                                                 RequestLogDelegate:    OnPutTariffHTTPRequest,
                                                 ResponseLogDelegate:   OnPutTariffHTTPResponse,
                                                 EventTrackingId:       eventTrackingId,
                                                 //NumberOfRetry:         transmissionRetry,
                                                 RequestTimeout:        RequestTimeout ?? this.RequestTimeout,
                                                 CancellationToken:     CancellationToken).

                                             ConfigureAwait(false);

                    #endregion

                    response = OCPIResponse<Tariff>.ParseJObject(
                                   httpResponse,
                                   requestId,
                                   correlationId,
                                   json => Tariff.Parse(json)
                               );

                    Counters.PutTariff.IncResponses_OK();

                }

                else
                {
                    response = OCPIResponse<String, Tariff>.Error("No remote URL available!");
                    Counters.PutTariff.IncRequests_Error();
                }

            }

            catch (Exception e)
            {
                response = OCPIResponse<String, Tariff>.Exception(e);
                Counters.PutTariff.IncResponses_Error();
            }


            #region Send OnPutTariffResponse event

            stopwatch.Stop();
            var endtime = Timestamp.Now;

            await LogEvent(
                      OnPutTariffResponse,
                      loggingDelegate => loggingDelegate.Invoke(
                          endtime,
                          this,
                          requestId,
                          correlationId,

                          Tariff,

                          CancellationToken,
                          eventTrackingId,
                          requestTimeout,

                          response,
                          stopwatch.Elapsed
                      )
                  );

            #endregion

            return response;

        }

        #endregion

        #region PatchTariff        (CountryCode, PartyId, TariffId, TariffPatch, ...)    [NonStandard]

        /// <summary>
        /// Patch a tariff.
        /// </summary>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional cancellation token to cancel this request.</param>
        public async Task<OCPIResponse<Tariff>>

            PatchTariff(CountryCode        CountryCode,
                        Party_Id           PartyId,
                        Tariff_Id          TariffId,
                        JObject            TariffPatch,

                        Request_Id?        RequestId           = null,
                        Correlation_Id?    CorrelationId       = null,
                        Version_Id?        VersionId           = null,

                        DateTimeOffset?    RequestTimestamp    = null,
                        EventTracking_Id?  EventTrackingId     = null,
                        TimeSpan?          RequestTimeout      = null,
                        CancellationToken  CancellationToken   = default)

        {

            #region Initial checks

            if (!TariffPatch.HasValues)
                return OCPIResponse<Tariff>.Error("The given charging tariff patch must not be null!");

            #endregion

            #region Init

            var requestId         = RequestId        ?? Request_Id.    NewRandom();
            var correlationId     = CorrelationId    ?? Correlation_Id.NewRandom();

            var requestTimestamp  = RequestTimestamp ?? Timestamp.Now;
            var eventTrackingId   = EventTrackingId  ?? EventTracking_Id.New;
            var requestTimeout    = RequestTimeout   ?? this.RequestTimeout;

            var startTime         = Timestamp.Now;
            var stopwatch         = Stopwatch.StartNew();

            OCPIResponse<Tariff> response;

            #endregion

            #region Send OnPatchTariffRequest event

            await LogEvent(
                      OnPatchTariffRequest,
                      loggingDelegate => loggingDelegate.Invoke(
                          startTime,
                          this,
                          requestId,
                          correlationId,

                          CountryCode,
                          PartyId,
                          TariffId,
                          TariffPatch,

                          CancellationToken,
                          eventTrackingId,
                          requestTimeout
                      )
                  );

            #endregion


            try
            {

                var httpClient = await GetModuleHTTPClient(
                                           Module_Id.Tariffs,
                                           InterfaceRoles.RECEIVER,
                                           VersionId,
                                           eventTrackingId,
                                           CancellationToken
                                       );

                if (httpClient is not null)
                {

                    #region Upstream HTTP request...

                    var httpResponse = await httpClient.PATCH(
                                                 Path:                  httpClient.RemoteURL.Path + CountryCode.ToString() +
                                                                                                    PartyId.    ToString() +
                                                                                                    TariffId.   ToString(),
                                                 Content:               TariffPatch.ToUTF8Bytes(JSONFormatting),
                                                 Authentication:        TokenAuth,
                                                 RequestBuilder:        requestBuilder => {
                                                                            requestBuilder.Set("X-Request-ID",     requestId);
                                                                            requestBuilder.Set("X-Correlation-ID", correlationId);
                                                                        },
                                                 RequestLogDelegate:    OnPatchTariffHTTPRequest,
                                                 ResponseLogDelegate:   OnPatchTariffHTTPResponse,
                                                 EventTrackingId:       eventTrackingId,
                                                 //NumberOfRetry:         transmissionRetry,
                                                 RequestTimeout:        RequestTimeout ?? this.RequestTimeout,
                                                 CancellationToken:     CancellationToken).

                                             ConfigureAwait(false);

                    #endregion

                    response = OCPIResponse<Tariff>.ParseJObject(
                                   httpResponse,
                                   requestId,
                                   correlationId,
                                   json => Tariff.Parse(json)
                               );

                    Counters.PatchTariff.IncResponses_OK();

                }

                else
                {
                    response = OCPIResponse<String, Tariff>.Error("No remote URL available!");
                    Counters.PatchTariff.IncRequests_Error();
                }

            }

            catch (Exception e)
            {
                response = OCPIResponse<String, Tariff>.Exception(e);
                Counters.PatchTariff.IncResponses_Error();
            }


            #region Send OnPatchTariffResponse event

            stopwatch.Stop();
            var endtime = Timestamp.Now;

            await LogEvent(
                      OnPatchTariffResponse,
                      loggingDelegate => loggingDelegate.Invoke(
                          endtime,
                          this,
                          requestId,
                          correlationId,

                          CountryCode,
                          PartyId,
                          TariffId,
                          TariffPatch,

                          CancellationToken,
                          eventTrackingId,
                          requestTimeout,

                          response,
                          stopwatch.Elapsed
                      )
                  );

            #endregion

            return response;

        }

        #endregion

        #region DeleteTariff       (CountryCode, PartyId, TariffId, ...)

        /// <summary>
        /// Delete the charging tariff specified by the given tariff identification from the remote API.
        /// </summary>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional cancellation token to cancel this request.</param>
        public async Task<OCPIResponse<Tariff>>

            DeleteTariff(CountryCode        CountryCode,
                         Party_Id           PartyId,
                         Tariff_Id          TariffId,

                         Request_Id?        RequestId           = null,
                         Correlation_Id?    CorrelationId       = null,
                         Version_Id?        VersionId           = null,

                         DateTimeOffset?    RequestTimestamp    = null,
                         EventTracking_Id?  EventTrackingId     = null,
                         TimeSpan?          RequestTimeout      = null,
                         CancellationToken  CancellationToken   = default)

        {

            #region Init

            var requestId         = RequestId        ?? Request_Id.    NewRandom();
            var correlationId     = CorrelationId    ?? Correlation_Id.NewRandom();

            var requestTimestamp  = RequestTimestamp ?? Timestamp.Now;
            var eventTrackingId   = EventTrackingId  ?? EventTracking_Id.New;
            var requestTimeout    = RequestTimeout   ?? this.RequestTimeout;

            var startTime         = Timestamp.Now;
            var stopwatch         = Stopwatch.StartNew();

            Counters.DeleteTariff.IncRequests_OK();

            OCPIResponse<Tariff> response;

            #endregion

            #region Send OnDeleteTariffRequest event

            await LogEvent(
                      OnDeleteTariffRequest,
                      loggingDelegate => loggingDelegate.Invoke(
                          startTime,
                          this,
                          requestId,
                          correlationId,

                          CountryCode,
                          PartyId,
                          TariffId,

                          CancellationToken,
                          eventTrackingId,
                          requestTimeout
                      )
                  );

            #endregion


            try
            {

                var httpClient = await GetModuleHTTPClient(
                                           Module_Id.Tariffs,
                                           InterfaceRoles.RECEIVER,
                                           VersionId,
                                           eventTrackingId,
                                           CancellationToken
                                       );

                if (httpClient is not null)
                {

                    #region Upstream HTTP request...

                    var httpResponse = await httpClient.DELETE(
                                                 Path:                  httpClient.RemoteURL.Path + CountryCode.ToString() +
                                                                                                    PartyId.    ToString() +
                                                                                                    TariffId.   ToString(),
                                                 Authentication:        TokenAuth,
                                                 RequestBuilder:        requestBuilder => {
                                                                            requestBuilder.Set("X-Request-ID",     requestId);
                                                                            requestBuilder.Set("X-Correlation-ID", correlationId);
                                                                        },
                                                 RequestLogDelegate:    OnDeleteTariffHTTPRequest,
                                                 ResponseLogDelegate:   OnDeleteTariffHTTPResponse,
                                                 EventTrackingId:       eventTrackingId,
                                                 //NumberOfRetry:         transmissionRetry,
                                                 RequestTimeout:        RequestTimeout ?? this.RequestTimeout,
                                                 CancellationToken:     CancellationToken).

                                             ConfigureAwait(false);

                    #endregion

                    response = OCPIResponse<Tariff>.ParseJObject(
                                   httpResponse,
                                   requestId,
                                   correlationId,
                                   json => Tariff.Parse(json)
                               );

                    Counters.DeleteTariff.IncResponses_OK();

                }

                else
                {
                    response = OCPIResponse<String, Tariff>.Error("No remote URL available!");
                    Counters.DeleteTariff.IncRequests_Error();
                }

            }

            catch (Exception e)
            {
                response = OCPIResponse<String, Tariff>.Exception(e);
                Counters.DeleteTariff.IncResponses_Error();
            }


            #region Send OnDeleteTariffResponse event

            stopwatch.Stop();
            var endtime = Timestamp.Now;

            await LogEvent(
                      OnDeleteTariffResponse,
                      loggingDelegate => loggingDelegate.Invoke(
                          endtime,
                          this,
                          requestId,
                          correlationId,

                          CountryCode,
                          PartyId,
                          TariffId,

                          CancellationToken,
                          eventTrackingId,
                          requestTimeout,

                          response,
                          stopwatch.Elapsed
                      )
                  );

            #endregion

            return response;

        }

        #endregion


        #region GetSession         (CountryCode, PartyId, SessionId, ...)

        /// <summary>
        /// Get the charging session specified by the given session identification from the remote API.
        /// </summary>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional cancellation token to cancel this request.</param>
        public async Task<OCPIResponse<Session>>

            GetSession(CountryCode        CountryCode,
                       Party_Id           PartyId,
                       Session_Id         SessionId,

                       Request_Id?        RequestId           = null,
                       Correlation_Id?    CorrelationId       = null,
                       Version_Id?        VersionId           = null,

                       DateTimeOffset?    RequestTimestamp    = null,
                       EventTracking_Id?  EventTrackingId     = null,
                       TimeSpan?          RequestTimeout      = null,
                       CancellationToken  CancellationToken   = default)

        {

            #region Init

            var requestId         = RequestId        ?? Request_Id.    NewRandom();
            var correlationId     = CorrelationId    ?? Correlation_Id.NewRandom();

            var requestTimestamp  = RequestTimestamp ?? Timestamp.Now;
            var eventTrackingId   = EventTrackingId  ?? EventTracking_Id.New;
            var requestTimeout    = RequestTimeout   ?? this.RequestTimeout;

            var startTime         = Timestamp.Now;
            var stopwatch         = Stopwatch.StartNew();

            Counters.GetSession.IncRequests_OK();

            OCPIResponse<Session> response;

            #endregion

            #region Send OnGetSessionRequest event

            await LogEvent(
                      OnGetSessionRequest,
                      loggingDelegate => loggingDelegate.Invoke(
                          startTime,
                          this,
                          requestId,
                          correlationId,

                          CountryCode,
                          PartyId,
                          SessionId,

                          CancellationToken,
                          eventTrackingId,
                          requestTimeout
                      )
                  );

            #endregion


            try
            {

                var httpClient = await GetModuleHTTPClient(
                                           Module_Id.Sessions,
                                           InterfaceRoles.RECEIVER,
                                           VersionId,
                                           eventTrackingId,
                                           CancellationToken
                                       );

                if (httpClient is not null)
                {

                    #region Upstream HTTP request...

                    var httpResponse = await httpClient.GET(
                                                 Path:                  httpClient.RemoteURL.Path + CountryCode.ToString() +
                                                                                                    PartyId.    ToString() +
                                                                                                    SessionId.  ToString(),
                                                 Authentication:        TokenAuth,
                                                 RequestBuilder:        requestBuilder => {
                                                                            requestBuilder.Set("X-Request-ID",     requestId);
                                                                            requestBuilder.Set("X-Correlation-ID", correlationId);
                                                                        },
                                                 RequestLogDelegate:    OnGetSessionHTTPRequest,
                                                 ResponseLogDelegate:   OnGetSessionHTTPResponse,
                                                 EventTrackingId:       eventTrackingId,
                                                 //NumberOfRetry:         transmissionRetry,
                                                 RequestTimeout:        RequestTimeout ?? this.RequestTimeout,
                                                 CancellationToken:     CancellationToken).

                                             ConfigureAwait(false);

                    #endregion

                    response = OCPIResponse<Session>.ParseJObject(
                                   httpResponse,
                                   requestId,
                                   correlationId,
                                   json => Session.Parse(json)
                               );

                    Counters.GetSession.IncResponses_OK();

                }

                else
                {
                    response = OCPIResponse<String, Session>.Error("No remote URL available!");
                    Counters.GetSession.IncRequests_Error();
                }

            }

            catch (Exception e)
            {
                response = OCPIResponse<String, Session>.Exception(e);
                Counters.GetSession.IncResponses_Error();
            }


            #region Send OnGetSessionResponse event

            stopwatch.Stop();
            var endtime = Timestamp.Now;

            await LogEvent(
                      OnGetSessionResponse,
                      loggingDelegate => loggingDelegate.Invoke(
                          endtime,
                          this,
                          requestId,
                          correlationId,

                          CountryCode,
                          PartyId,
                          SessionId,

                          CancellationToken,
                          eventTrackingId,
                          requestTimeout,

                          response,
                          stopwatch.Elapsed
                      )
                  );

            #endregion

            return response;

        }

        #endregion

        #region PutSession         (Session, ...)

        /// <summary>
        /// Put/store the given charging session on/within the remote API.
        /// </summary>
        /// <param name="Session">The charging session to store/put at/onto the remote API.</param>
        /// 
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional cancellation token to cancel this request.</param>
        public async Task<OCPIResponse<Session>>

            PutSession(Session            Session,

                       Request_Id?        RequestId           = null,
                       Correlation_Id?    CorrelationId       = null,
                       Version_Id?        VersionId           = null,

                       DateTimeOffset?    RequestTimestamp    = null,
                       EventTracking_Id?  EventTrackingId     = null,
                       TimeSpan?          RequestTimeout      = null,
                       CancellationToken  CancellationToken   = default)

        {

            #region Init

            var requestId         = RequestId        ?? Request_Id.    NewRandom();
            var correlationId     = CorrelationId    ?? Correlation_Id.NewRandom();

            var requestTimestamp  = RequestTimestamp ?? Timestamp.Now;
            var eventTrackingId   = EventTrackingId  ?? EventTracking_Id.New;
            var requestTimeout    = RequestTimeout   ?? this.RequestTimeout;

            var startTime         = Timestamp.Now;
            var stopwatch         = Stopwatch.StartNew();

            Counters.PutSession.IncRequests_OK();

            OCPIResponse<Session> response;

            #endregion

            #region Send OnPutSessionRequest event

            await LogEvent(
                      OnPutSessionRequest,
                      loggingDelegate => loggingDelegate.Invoke(
                          startTime,
                          this,
                          requestId,
                          correlationId,

                          Session,

                          CancellationToken,
                          eventTrackingId,
                          requestTimeout
                      )
                  );

            #endregion


            try
            {

                var httpClient = await GetModuleHTTPClient(
                                           Module_Id.Sessions,
                                           InterfaceRoles.RECEIVER,
                                           VersionId,
                                           eventTrackingId,
                                           CancellationToken
                                       );

                if (httpClient is not null)
                {

                    #region Upstream HTTP request...

                    var httpResponse = await httpClient.PUT(
                                                 Path:                  httpClient.RemoteURL.Path + Session.CountryCode.ToString() +
                                                                                                    Session.PartyId.    ToString() +
                                                                                                    Session.Id.         ToString(),
                                                 Content:               Session.ToJSON(
                                                                            CustomSessionSerializer,
                                                                            CustomCDRTokenSerializer,
                                                                            CustomChargingPeriodSerializer,
                                                                            CustomCDRDimensionSerializer,
                                                                            CustomPriceSerializer
                                                                        ).ToUTF8Bytes(JSONFormatting),
                                                 Authentication:        TokenAuth,
                                                 RequestBuilder:        requestBuilder => {
                                                                            requestBuilder.Set("X-Request-ID",     requestId);
                                                                            requestBuilder.Set("X-Correlation-ID", correlationId);
                                                                        },
                                                 RequestLogDelegate:    OnPutSessionHTTPRequest,
                                                 ResponseLogDelegate:   OnPutSessionHTTPResponse,
                                                 EventTrackingId:       eventTrackingId,
                                                 //NumberOfRetry:         transmissionRetry,
                                                 RequestTimeout:        RequestTimeout ?? this.RequestTimeout,
                                                 CancellationToken:     CancellationToken).

                                             ConfigureAwait(false);

                    #endregion

                    response = OCPIResponse<Session>.ParseJObject(
                                   httpResponse,
                                   requestId,
                                   correlationId,
                                   json => Session.Parse(json)
                               );

                    Counters.PutSession.IncResponses_OK();

                }

                else
                {
                    response = OCPIResponse<String, Session>.Error("No remote URL available!");
                    Counters.PutSession.IncRequests_Error();
                }

            }

            catch (Exception e)
            {
                response = OCPIResponse<String, Session>.Exception(e);
                Counters.PutSession.IncResponses_Error();
            }


            #region Send OnPutSessionResponse event

            var endtime = Timestamp.Now;

            await LogEvent(
                      OnPutSessionResponse,
                      loggingDelegate => loggingDelegate.Invoke(
                          endtime,
                          this,
                          requestId,
                          correlationId,

                          Session,

                          CancellationToken,
                          eventTrackingId,
                          requestTimeout,

                          response,
                          stopwatch.Elapsed
                      )
                  );

            #endregion

            return response;

        }

        #endregion

        #region PatchSession       (CountryCode, PartyId, SessionId, SessionPatch, ...)

        /// <summary>
        /// Patch a session.
        /// </summary>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional cancellation token to cancel this request.</param>
        public async Task<OCPIResponse<Session>>

            PatchSession(CountryCode        CountryCode,
                         Party_Id           PartyId,
                         Session_Id         SessionId,
                         JObject            SessionPatch,

                         Request_Id?        RequestId           = null,
                         Correlation_Id?    CorrelationId       = null,
                         Version_Id?        VersionId           = null,

                         DateTimeOffset?    RequestTimestamp    = null,
                         EventTracking_Id?  EventTrackingId     = null,
                         TimeSpan?          RequestTimeout      = null,
                         CancellationToken  CancellationToken   = default)

        {

            #region Initial checks

            if (!SessionPatch.HasValues)
                return OCPIResponse<Session>.Error("The given charging session patch must not be null!");

            #endregion

            #region Init

            var requestId         = RequestId        ?? Request_Id.    NewRandom();
            var correlationId     = CorrelationId    ?? Correlation_Id.NewRandom();

            var requestTimestamp  = RequestTimestamp ?? Timestamp.Now;
            var eventTrackingId   = EventTrackingId  ?? EventTracking_Id.New;
            var requestTimeout    = RequestTimeout   ?? this.RequestTimeout;

            var startTime         = Timestamp.Now;
            var stopwatch         = Stopwatch.StartNew();

            Counters.PatchSession.IncRequests_OK();

            OCPIResponse<Session> response;

            #endregion

            #region Send OnPatchSessionRequest event

            await LogEvent(
                      OnPatchSessionRequest,
                      loggingDelegate => loggingDelegate.Invoke(
                          startTime,
                          this,
                          requestId,
                          correlationId,

                          CountryCode,
                          PartyId,
                          SessionId,
                          SessionPatch,

                          CancellationToken,
                          eventTrackingId,
                          requestTimeout
                      )
                  );

            #endregion


            try
            {

                var httpClient = await GetModuleHTTPClient(
                                           Module_Id.Sessions,
                                           InterfaceRoles.RECEIVER,
                                           VersionId,
                                           eventTrackingId,
                                           CancellationToken
                                       );

                if (httpClient is not null)
                {

                    #region Upstream HTTP request...

                    var httpResponse = await httpClient.PATCH(
                                                 Path:                  httpClient.RemoteURL.Path + CountryCode.ToString() +
                                                                                                    PartyId.    ToString() +
                                                                                                    SessionId.  ToString(),
                                                 Content:               SessionPatch.ToUTF8Bytes(JSONFormatting),
                                                 Authentication:        TokenAuth,
                                                 RequestBuilder:        requestBuilder => {
                                                                            requestBuilder.Set("X-Request-ID",     requestId);
                                                                            requestBuilder.Set("X-Correlation-ID", correlationId);
                                                                        },
                                                 RequestLogDelegate:    OnPatchSessionHTTPRequest,
                                                 ResponseLogDelegate:   OnPatchSessionHTTPResponse,
                                                 EventTrackingId:       eventTrackingId,
                                                 //NumberOfRetry:         transmissionRetry,
                                                 RequestTimeout:        RequestTimeout ?? this.RequestTimeout,
                                                 CancellationToken:     CancellationToken).

                                             ConfigureAwait(false);

                    #endregion

                    response = OCPIResponse<Session>.ParseJObject(
                                   httpResponse,
                                   requestId,
                                   correlationId,
                                   json => Session.Parse(json)
                               );

                    Counters.PatchSession.IncResponses_OK();

                }

                else
                {
                    response = OCPIResponse<String, Session>.Error("No remote URL available!");
                    Counters.PatchSession.IncRequests_Error();
                }

            }

            catch (Exception e)
            {
                response = OCPIResponse<String, Session>.Exception(e);
                Counters.PatchSession.IncResponses_Error();
            }


            #region Send OnPatchSessionResponse event

            stopwatch.Stop();
            var endtime = Timestamp.Now;

            await LogEvent(
                      OnPatchSessionResponse,
                      loggingDelegate => loggingDelegate.Invoke(
                          endtime,
                          this,
                          requestId,
                          correlationId,

                          CountryCode,
                          PartyId,
                          SessionId,
                          SessionPatch,

                          CancellationToken,
                          eventTrackingId,
                          requestTimeout,

                          response,
                          stopwatch.Elapsed
                      )
                  );

            #endregion

            return response;

        }

        #endregion

        #region DeleteSession      (CountryCode, PartyId, SessionId, ...)    [NonStandard]

        /// <summary>
        /// Delete the charging session specified by the given session identification from the remote API.
        /// </summary>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional cancellation token to cancel this request.</param>
        public async Task<OCPIResponse<Session>>

            DeleteSession(CountryCode        CountryCode,
                          Party_Id           PartyId,
                          Session_Id         SessionId,

                          Request_Id?        RequestId           = null,
                          Correlation_Id?    CorrelationId       = null,
                          Version_Id?        VersionId           = null,

                          DateTimeOffset?    RequestTimestamp    = null,
                          EventTracking_Id?  EventTrackingId     = null,
                          TimeSpan?          RequestTimeout      = null,
                          CancellationToken  CancellationToken   = default)

        {

            #region Init

            var requestId         = RequestId        ?? Request_Id.    NewRandom();
            var correlationId     = CorrelationId    ?? Correlation_Id.NewRandom();

            var requestTimestamp  = RequestTimestamp ?? Timestamp.Now;
            var eventTrackingId   = EventTrackingId  ?? EventTracking_Id.New;
            var requestTimeout    = RequestTimeout   ?? this.RequestTimeout;

            var startTime         = Timestamp.Now;
            var stopwatch         = Stopwatch.StartNew();

            Counters.DeleteSession.IncRequests_OK();

            OCPIResponse<Session> response;

            #endregion

            #region Send OnDeleteSessionRequest event

            await LogEvent(
                      OnDeleteSessionRequest,
                      loggingDelegate => loggingDelegate.Invoke(
                          startTime,
                          this,
                          requestId,
                          correlationId,

                          CountryCode,
                          PartyId,
                          SessionId,

                          CancellationToken,
                          eventTrackingId,
                          requestTimeout
                      )
                  );

            #endregion


            try
            {

                var httpClient = await GetModuleHTTPClient(
                                           Module_Id.Sessions,
                                           InterfaceRoles.RECEIVER,
                                           VersionId,
                                           eventTrackingId,
                                           CancellationToken
                                       );

                if (httpClient is not null)
                {

                    #region Upstream HTTP request...

                    var httpResponse = await httpClient.DELETE(
                                                 Path:                  httpClient.RemoteURL.Path + CountryCode.ToString() +
                                                                                                    PartyId.    ToString() +
                                                                                                    SessionId.  ToString(),
                                                 Authentication:        TokenAuth,
                                                 RequestBuilder:        requestBuilder => {
                                                                            requestBuilder.Set("X-Request-ID",     requestId);
                                                                            requestBuilder.Set("X-Correlation-ID", correlationId);
                                                                        },
                                                 RequestLogDelegate:    OnDeleteSessionHTTPRequest,
                                                 ResponseLogDelegate:   OnDeleteSessionHTTPResponse,
                                                 EventTrackingId:       eventTrackingId,
                                                 //NumberOfRetry:         transmissionRetry,
                                                 RequestTimeout:        RequestTimeout ?? this.RequestTimeout,
                                                 CancellationToken:     CancellationToken).

                                             ConfigureAwait(false);

                    #endregion

                    response = OCPIResponse<Session>.ParseJObject(
                                   httpResponse,
                                   requestId,
                                   correlationId,
                                   json => Session.Parse(json)
                               );

                    Counters.DeleteSession.IncResponses_OK();

                }

                else
                {
                    response = OCPIResponse<String, Session>.Error("No remote URL available!");
                    Counters.DeleteSession.IncRequests_Error();
                }

            }

            catch (Exception e)
            {
                response = OCPIResponse<String, Session>.Exception(e);
                Counters.DeleteSession.IncResponses_Error();
            }


            #region Send OnDeleteSessionResponse event

            var endtime = Timestamp.Now;

            await LogEvent(
                      OnDeleteSessionResponse,
                      loggingDelegate => loggingDelegate.Invoke(
                          endtime,
                          this,
                          requestId,
                          correlationId,

                          CountryCode,
                          PartyId,
                          SessionId,

                          CancellationToken,
                          eventTrackingId,
                          requestTimeout,

                          response,
                          stopwatch.Elapsed
                      )
                  );

            #endregion

            return response;

        }

        #endregion


        #region PostCDR            (CDR, ...)

        /// <summary>
        /// Post/store the given charge detail record on/within the remote API.
        /// </summary>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional charge detail record to cancel this request.</param>
        public async Task<OCPIResponse<CDR>>

            PostCDR(CDR                CDR,

                    Request_Id?        RequestId           = null,
                    Correlation_Id?    CorrelationId       = null,
                    Version_Id?        VersionId           = null,

                    DateTimeOffset?    RequestTimestamp    = null,
                    EventTracking_Id?  EventTrackingId     = null,
                    TimeSpan?          RequestTimeout      = null,
                    CancellationToken  CancellationToken   = default)

        {

            #region Init

            var requestId         = RequestId        ?? Request_Id.    NewRandom();
            var correlationId     = CorrelationId    ?? Correlation_Id.NewRandom();

            var requestTimestamp  = RequestTimestamp ?? Timestamp.Now;
            var eventTrackingId   = EventTrackingId  ?? EventTracking_Id.New;
            var requestTimeout    = RequestTimeout   ?? this.RequestTimeout;

            var startTime         = Timestamp.Now;
            var stopwatch         = Stopwatch.StartNew();

            Counters.PostCDR.IncRequests_OK();

            OCPIResponse<CDR> response;

            #endregion

            #region Send OnPostCDRRequest event

            await LogEvent(
                      OnPostCDRRequest,
                      loggingDelegate => loggingDelegate.Invoke(
                          startTime,
                          this,
                          requestId,
                          correlationId,

                          CDR,

                          CancellationToken,
                          eventTrackingId,
                          requestTimeout
                      )
                  );

            #endregion


            try
            {

                var httpClient = await GetModuleHTTPClient(
                                           Module_Id.CDRs,
                                           InterfaceRoles.RECEIVER,
                                           VersionId,
                                           eventTrackingId,
                                           CancellationToken
                                       );

                if (httpClient is not null)
                {

                    #region Upstream HTTP request...

                    // The EMSP Id of the CDR might be different from the remote party identification,
                    // e.g. when the remote party is a hub!
                    var realEMSPId    = EMSP_Id.Parse(
                                            CDR.CountryCode,
                                            CDR.PartyId
                                        );

                    var httpResponse  = await httpClient.POST(
                                                  Path:                  httpClient.RemoteURL.Path + CDR.CountryCode.ToString() +    // <= Unclear if this URL is correct!
                                                                                                     CDR.PartyId.    ToString() +
                                                                                                     CDR.Id.         ToString(),
                                                  Content:               CDR.ToJSON(
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
                                                                         ).ToUTF8Bytes(JSONFormatting),
                                                  Authentication:        TokenAuth,
                                                  RequestBuilder:        requestBuilder => {
                                                                             requestBuilder.Set("X-Request-ID",     requestId);
                                                                             requestBuilder.Set("X-Correlation-ID", correlationId);
                                                                         },
                                                  RequestLogDelegate:    OnPostCDRHTTPRequest,
                                                  ResponseLogDelegate:   OnPostCDRHTTPResponse,
                                                  EventTrackingId:       eventTrackingId,
                                                  //NumberOfRetry:         transmissionRetry,
                                                  RequestTimeout:        RequestTimeout ?? this.RequestTimeout,
                                                  CancellationToken:     CancellationToken).

                                              ConfigureAwait(false);

                    #endregion

                    response = OCPIResponse<CDR>.ParseJObject(
                                   httpResponse,
                                   requestId,
                                   correlationId,
                                   json => CDR.Parse(json)
                               );

                    Counters.PostCDR.IncResponses_OK();

                }

                else
                {
                    response = OCPIResponse<String, CDR>.Error("No remote URL available!");
                    Counters.PostCDR.IncRequests_Error();
                }

            }

            catch (Exception e)
            {
                response = OCPIResponse<String, CDR>.Exception(e);
                Counters.PostCDR.IncResponses_Error();
            }


            #region Send OnPostCDRResponse event

            stopwatch.Stop();
            var endtime = Timestamp.Now;

            await LogEvent(
                      OnPostCDRResponse,
                      loggingDelegate => loggingDelegate.Invoke(
                          endtime,
                          this,
                          requestId,
                          correlationId,

                          CDR,

                          CancellationToken,
                          eventTrackingId,
                          requestTimeout,

                          response,
                          stopwatch.Elapsed
                      )
                  );

            #endregion

            return response;

        }

        #endregion

        #region GetCDR             (CountryCode, PartyId, CDRId, ...)   // The concrete URL is not specified by OCPI! m(

        /// <summary>
        /// Get the charge detail record specified by the given charge detail record identification from the remote API.
        /// </summary>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional charge detail record to cancel this request.</param>
        public async Task<OCPIResponse<CDR>>

            GetCDR(CountryCode        CountryCode,
                   Party_Id           PartyId,
                   CDR_Id             CDRId,

                   Request_Id?        RequestId           = null,
                   Correlation_Id?    CorrelationId       = null,
                   Version_Id?        VersionId           = null,

                   DateTimeOffset?    RequestTimestamp    = null,
                   EventTracking_Id?  EventTrackingId     = null,
                   TimeSpan?          RequestTimeout      = null,
                   CancellationToken  CancellationToken   = default)

        {

            #region Init

            var requestId         = RequestId        ?? Request_Id.    NewRandom();
            var correlationId     = CorrelationId    ?? Correlation_Id.NewRandom();

            var requestTimestamp  = RequestTimestamp ?? Timestamp.Now;
            var eventTrackingId   = EventTrackingId  ?? EventTracking_Id.New;
            var requestTimeout    = RequestTimeout   ?? this.RequestTimeout;

            var startTime         = Timestamp.Now;
            var stopwatch         = Stopwatch.StartNew();

            Counters.GetCDR.IncRequests_OK();

            OCPIResponse<CDR> response;

            #endregion

            #region Send OnGetCDRRequest event

            await LogEvent(
                      OnGetCDRRequest,
                      loggingDelegate => loggingDelegate.Invoke(
                          startTime,
                          this,
                          requestId,
                          correlationId,

                          CountryCode,
                          PartyId,
                          CDRId,

                          CancellationToken,
                          eventTrackingId,
                          requestTimeout
                      )
                  );

            #endregion


            try
            {

                var httpClient = await GetModuleHTTPClient(
                                           Module_Id.CDRs,
                                           InterfaceRoles.RECEIVER,
                                           VersionId,
                                           eventTrackingId,
                                           CancellationToken
                                       );

                if (httpClient is not null)
                {

                    #region Upstream HTTP request...

                    var httpResponse = await httpClient.GET(
                                                 Path:                  httpClient.RemoteURL.Path + CountryCode.ToString() +
                                                                                                    PartyId.    ToString() +
                                                                                                    CDRId.      ToString(),
                                                 Authentication:        TokenAuth,
                                                 RequestBuilder:        requestBuilder => {
                                                                            requestBuilder.Set("X-Request-ID",     requestId);
                                                                            requestBuilder.Set("X-Correlation-ID", correlationId);
                                                                        },
                                                 RequestLogDelegate:    OnGetCDRHTTPRequest,
                                                 ResponseLogDelegate:   OnGetCDRHTTPResponse,
                                                 EventTrackingId:       eventTrackingId,
                                                 //NumberOfRetry:         transmissionRetry,
                                                 RequestTimeout:        RequestTimeout ?? this.RequestTimeout,
                                                 CancellationToken:     CancellationToken).

                                             ConfigureAwait(false);

                    #endregion

                    response = OCPIResponse<CDR>.ParseJObject(
                                   httpResponse,
                                   requestId,
                                   correlationId,
                                   json => CDR.Parse(json)
                               );

                    Counters.GetCDR.IncResponses_OK();

                }

                else
                {
                    response = OCPIResponse<String, CDR>.Error("No remote URL available!");
                    Counters.GetCDR.IncRequests_Error();
                }

            }

            catch (Exception e)
            {
                response = OCPIResponse<String, CDR>.Exception(e);
                Counters.GetCDR.IncResponses_Error();
            }


            #region Send OnGetCDRResponse event

            stopwatch.Stop();
            var endtime = Timestamp.Now;

            await LogEvent(
                      OnGetCDRResponse,
                      loggingDelegate => loggingDelegate.Invoke(
                          endtime,
                          this,
                          requestId,
                          correlationId,

                          CountryCode,
                          PartyId,
                          CDRId,

                          CancellationToken,
                          eventTrackingId,
                          requestTimeout,

                          response,
                          stopwatch.Elapsed
                      )
                  );

            #endregion

            return response;

        }

        #endregion


        #region GetTokens          (Offset = null, Limit = null, ...)

        /// <summary>
        /// Get all tokens from the remote API.
        /// </summary>
        /// 
        /// <param name="VersionId">An optional OCPI version identification.</param>
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="CorrelationId">An optional request correlation identification.</param>
        /// 
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional cancellation token to cancel this request.</param>
        public async Task<OCPIResponse<IEnumerable<Token>>>

            GetTokens(UInt64?            Offset              = null,
                      UInt64?            Limit               = null,

                      Version_Id?        VersionId           = null,
                      Request_Id?        RequestId           = null,
                      Correlation_Id?    CorrelationId       = null,

                      DateTimeOffset?    RequestTimestamp    = null,
                      EventTracking_Id?  EventTrackingId     = null,
                      TimeSpan?          RequestTimeout      = null,
                      CancellationToken  CancellationToken   = default)

        {

            #region Init

            var requestId         = RequestId        ?? Request_Id.    NewRandom();
            var correlationId     = CorrelationId    ?? Correlation_Id.NewRandom();

            var requestTimestamp  = RequestTimestamp ?? Timestamp.Now;
            var eventTrackingId   = EventTrackingId  ?? EventTracking_Id.New;
            var requestTimeout    = RequestTimeout   ?? this.RequestTimeout;

            var startTime         = Timestamp.Now;
            var stopwatch         = Stopwatch.StartNew();

            Counters.GetTokens.IncRequests_OK();

            OCPIResponse<IEnumerable<Token>> response;

            #endregion

            #region Send OnGetTokensRequest event

            await LogEvent(
                      OnGetTokensRequest,
                      loggingDelegate => loggingDelegate.Invoke(
                          startTime,
                          this,
                          requestId,
                          correlationId,

                          Offset,
                          Limit,

                          CancellationToken,
                          eventTrackingId,
                          requestTimeout
                      )
                  );

            #endregion


            try
            {

                var httpClient = await GetModuleHTTPClient(
                                           Module_Id.Tokens,
                                           InterfaceRoles.RECEIVER,
                                           VersionId,
                                           eventTrackingId,
                                           CancellationToken
                                       );

                if (httpClient is not null)
                {

                    var offsetLimit    = "";

                    if (Offset.HasValue)
                        offsetLimit += "&offset=" + Offset.Value;

                    if (Limit.HasValue)
                        offsetLimit += "&limit="  + Limit. Value;

                    if (offsetLimit.Length > 0)
                        offsetLimit = String.Concat("?", offsetLimit.AsSpan(1));


                    #region Upstream HTTP request...

                    var httpResponse = await httpClient.GET(
                                                 Path:                  httpClient.RemoteURL.Path + offsetLimit,
                                                 Authentication:        TokenAuth,
                                                 RequestBuilder:        requestBuilder => {
                                                                            requestBuilder.Set("X-Request-ID",     requestId);
                                                                            requestBuilder.Set("X-Correlation-ID", correlationId);
                                                                        },
                                                 RequestLogDelegate:    OnGetTokensHTTPRequest,
                                                 ResponseLogDelegate:   OnGetTokensHTTPResponse,
                                                 EventTrackingId:       eventTrackingId,
                                                 //NumberOfRetry:         transmissionRetry,
                                                 RequestTimeout:        RequestTimeout ?? this.RequestTimeout,
                                                 CancellationToken:     CancellationToken).

                                             ConfigureAwait(false);

                    #endregion

                    response = OCPIResponse<Token>.ParseJArray(
                                   httpResponse,
                                   requestId,
                                   correlationId,
                                   json => Token.Parse(json)
                               );

                    Counters.GetTokens.IncResponses_OK();

                }

                else
                {
                    response = OCPIResponse<String, IEnumerable<Token>>.Error("No remote URL available!");
                    Counters.GetTokens.IncRequests_Error();
                }

            }

            catch (Exception e)
            {
                response = OCPIResponse<String, IEnumerable<Token>>.Exception(e);
                Counters.GetTokens.IncResponses_Error();
            }


            #region Send OnGetTokensResponse event

            stopwatch.Stop();
            var endtime = Timestamp.Now;

            await LogEvent(
                      OnGetTokensResponse,
                      loggingDelegate => loggingDelegate.Invoke(
                          endtime,
                          this,
                          requestId,
                          correlationId,

                          Offset,
                          Limit,

                          CancellationToken,
                          eventTrackingId,
                          requestTimeout,

                          response,
                          stopwatch.Elapsed
                      )
                  );

            #endregion

            return response;

        }

        #endregion

        #region PostToken          (TokenId, TokenType = null, ...)

        /// <summary>
        /// Post/store the given token identification on/within the remote API.
        /// </summary>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional cancellation token to cancel this request.</param>
        public async Task<OCPIResponse<AuthorizationInfo>>

            PostToken(Token_Id            TokenId,
                      TokenType?          TokenType           = null,
                      LocationReference?  LocationReference   = null,

                      Request_Id?         RequestId           = null,
                      Correlation_Id?     CorrelationId       = null,
                      Version_Id?         VersionId           = null,

                      DateTimeOffset?     RequestTimestamp    = null,
                      EventTracking_Id?   EventTrackingId     = null,
                      TimeSpan?           RequestTimeout      = null,
                      CancellationToken   CancellationToken   = default)

        {

            #region Init

            var requestId         = RequestId        ?? Request_Id.    NewRandom();
            var correlationId     = CorrelationId    ?? Correlation_Id.NewRandom();

            var requestTimestamp  = RequestTimestamp ?? Timestamp.Now;
            var eventTrackingId   = EventTrackingId  ?? EventTracking_Id.New;
            var requestTimeout    = RequestTimeout   ?? this.RequestTimeout;

            var startTime         = Timestamp.Now;
            var stopwatch         = Stopwatch.StartNew();

            Counters.PostToken.IncRequests_OK();

            OCPIResponse<AuthorizationInfo> response;

            #endregion

            #region Send OnPostTokenRequest event

            await LogEvent(
                      OnPostTokenRequest,
                      loggingDelegate => loggingDelegate.Invoke(
                          startTime,
                          this,
                          requestId,
                          correlationId,

                          TokenId,
                          TokenType,
                          LocationReference,

                          CancellationToken,
                          eventTrackingId,
                          requestTimeout
                      )
                  );

            #endregion


            try
            {

                var httpClient = await GetModuleHTTPClient(
                                           Module_Id.Tokens,
                                           InterfaceRoles.SENDER,
                                           VersionId,
                                           eventTrackingId,
                                           CancellationToken
                                       );

                if (httpClient is not null)
                {

                    #region Upstream HTTP request...

                    var httpResponse = await httpClient.POST(
                                                 Path:                  httpClient.RemoteURL.Path + TokenId.ToString() + "authorize",
                                                 Content:               [],
                                                 Authentication:        TokenAuth,
                                                 RequestBuilder:        requestBuilder => {

                                                                            requestBuilder.Set("X-Request-ID",     requestId);
                                                                            requestBuilder.Set("X-Correlation-ID", correlationId);

                                                                            if (TokenType.HasValue)
                                                                                requestBuilder.QueryString.Add("type", TokenType.Value.ToString());

                                                                            if (LocationReference.HasValue)
                                                                            {
                                                                                requestBuilder.Content     = LocationReference.Value.ToJSON(
                                                                                                                 CustomLocationReferenceSerializer
                                                                                                             ).ToUTF8Bytes(JSONFormatting);
                                                                                requestBuilder.ContentType = HTTPContentType.Application.JSON_UTF8;
                                                                            }

                                                                        },
                                                 RequestLogDelegate:    OnPostTokenHTTPRequest,
                                                 ResponseLogDelegate:   OnPostTokenHTTPResponse,
                                                 EventTrackingId:       eventTrackingId,
                                                 //NumberOfRetry:         transmissionRetry,
                                                 RequestTimeout:        RequestTimeout ?? this.RequestTimeout,
                                                 CancellationToken:     CancellationToken).

                                             ConfigureAwait(false);

                    #endregion

                    // {
                    //
                    //   "allowed":                  "ALLOWED",
                    //
                    //   "token": {
                    //       "country_code":                "DE",
                    //       "party_id":                    "GDF",
                    //       "uid":                         "aabbccdd",
                    //       "type":                        "RFID",
                    //       "contract_id":                 "C-aabbccdd",
                    //       "visual_number":               "visual:n/a",
                    //       "issuer":                      "DE-GDF1-issuer",
                    //       "valid":                        true,
                    //       "whitelist":                   "NEVER",
                    //       "last_updated":                "2021-11-11T04:48:36.913Z"
                    //   },
                    //
                    //   "authorization_reference":  "K5Cr29r53Q4v753nn49f8371CQA2Mh",
                    //
                    //   "info": {
                    //       "language":                    "en",
                    //       "text":                        "Charging allowed!"
                    //   }
                    //
                    // }

                    response = OCPIResponse<AuthorizationInfo>.ParseJObject(
                                   httpResponse,
                                   requestId,
                                   correlationId,
                                   json => AuthorizationInfo.Parse(json)
                               );

                    Counters.PostToken.IncResponses_OK();

                }

                else
                {
                    response = OCPIResponse<String, AuthorizationInfo>.Error("No remote URL available!");
                    Counters.PostToken.IncRequests_Error();
                }

            }

            catch (Exception e)
            {
                response = OCPIResponse<String, AuthorizationInfo>.Exception(e);
                Counters.PostToken.IncResponses_Error();
            }



            #region Send OnPostTokenResponse event

            stopwatch.Stop();
            var endtime = Timestamp.Now;

            response.Data?.Runtime = stopwatch.Elapsed;

            await LogEvent(
                      OnPostTokenResponse,
                      loggingDelegate => loggingDelegate.Invoke(
                          endtime,
                          this,
                          requestId,
                          correlationId,

                          TokenId,
                          TokenType,
                          LocationReference,

                          CancellationToken,
                          eventTrackingId,
                          requestTimeout,

                          response,
                          stopwatch.Elapsed
                      )
                  );

            #endregion

            return response;

        }

        #endregion


        // Commands

        // A CPO wants to send an update towards a EMSP or SCSP: https://github.com/ocpi/ocpi/blob/release-2.2.1-bugfixes/mod_charging_profiles.asciidoc

        #region SetChargingProfile (Token, ExpiryDate, ReservationId, LocationId, EVSEUId, AuthorizationReference, ...)

        /// <summary>
        /// Put/store the given token on/within the remote API.
        /// </summary>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional cancellation token to cancel this request.</param>
        public async Task<OCPIResponse<ChargingProfileResponse>>

            SetChargingProfile(Session_Id         SessionId,
                               ChargingProfile    ChargingProfile,

                               Request_Id?        RequestId           = null,
                               Correlation_Id?    CorrelationId       = null,
                               Version_Id?        VersionId           = null,

                               DateTimeOffset?    RequestTimestamp    = null,
                               EventTracking_Id?  EventTrackingId     = null,
                               TimeSpan?          RequestTimeout      = null,
                               CancellationToken  CancellationToken   = default)

        {

            #region Init

            var requestId         = RequestId        ?? Request_Id.    NewRandom();
            var correlationId     = CorrelationId    ?? Correlation_Id.NewRandom();

            var requestTimestamp  = RequestTimestamp ?? Timestamp.Now;
            var eventTrackingId   = EventTrackingId  ?? EventTracking_Id.New;
            var requestTimeout    = RequestTimeout   ?? this.RequestTimeout;

            var startTime         = Timestamp.Now;
            var stopwatch         = Stopwatch.StartNew();

            Counters.SetChargingProfile.IncRequests_OK();

            OCPIResponse<ChargingProfileResponse> response;

            #endregion

            #region Send OnSetChargingProfileRequest event

            await LogEvent(
                      OnSetChargingProfileRequest,
                      loggingDelegate => loggingDelegate.Invoke(
                          startTime,
                          this,
                          requestId,
                          correlationId,

                          SessionId,
                          ChargingProfile,

                          CancellationToken,
                          eventTrackingId,
                          requestTimeout
                      )
                  );

            #endregion


            try
            {

                var httpClient = await GetModuleHTTPClient(
                                           Module_Id.ChargingProfiles,
                                           InterfaceRoles.RECEIVER,
                                           VersionId,
                                           eventTrackingId,
                                           CancellationToken
                                       );

                if (httpClient is not null)
                {

                    var command = new SetChargingProfileCommand(
                                      ChargingProfile,
                                      CommonAPI.GetModuleURL(Module_Id.Commands) + "SET_CHARGING_PROFILE" + RandomExtensions.RandomString(50)
                                  );


                    #region Upstream HTTP request...

                    var httpResponse = await httpClient.PUT(
                                                 Path:                  httpClient.RemoteURL.Path + SessionId.ToString(),
                                                 Content:               command.ToJSON(
                                                                            CustomSetChargingProfileSerializer,
                                                                            CustomChargingProfileSerializer,
                                                                            CustomChargingProfilePeriodSerializer
                                                                        ).ToUTF8Bytes(JSONFormatting),
                                                 Authentication:        TokenAuth,
                                                 RequestBuilder:        requestBuilder => {
                                                                            requestBuilder.Set("X-Request-ID",     requestId);
                                                                            requestBuilder.Set("X-Correlation-ID", correlationId);
                                                                        },
                                                 RequestLogDelegate:    OnSetChargingProfileHTTPRequest,
                                                 ResponseLogDelegate:   OnSetChargingProfileHTTPResponse,
                                                 EventTrackingId:       eventTrackingId,
                                                 //NumberOfRetry:         transmissionRetry,
                                                 RequestTimeout:        RequestTimeout ?? this.RequestTimeout,
                                                 CancellationToken:     CancellationToken).

                                             ConfigureAwait(false);

                    #endregion

                    response = OCPIResponse<ChargingProfileResponse>.ParseJObject(
                                   httpResponse,
                                   requestId,
                                   correlationId,
                                   json => ChargingProfileResponse.Parse(json)
                               );

                    Counters.SetChargingProfile.IncResponses_OK();

                }

                else
                {
                    response = OCPIResponse<String, ChargingProfileResponse>.Error("No remote URL available!");
                    Counters.SetChargingProfile.IncRequests_Error();
                }

            }

            catch (Exception e)
            {
                response = OCPIResponse<String, ChargingProfileResponse>.Exception(e);
                Counters.PostToken.IncResponses_Error();
            }


            #region Send OnSetChargingProfileResponse event

            stopwatch.Stop();
            var endtime = Timestamp.Now;

            await LogEvent(
                      OnSetChargingProfileResponse,
                      loggingDelegate => loggingDelegate.Invoke(
                          endtime,
                          this,
                          requestId,
                          correlationId,

                          SessionId,
                          ChargingProfile,

                          CancellationToken,
                          eventTrackingId,
                          requestTimeout,

                          response,
                          stopwatch.Elapsed
                      )
                  );

            #endregion

            return response;

        }

        #endregion



        #region (private) LogEvent (Logger, LogHandler, ...)

        private Task LogEvent<TDelegate>(TDelegate?                                         Logger,
                                         Func<TDelegate, Task>                              LogHandler,
                                         [CallerArgumentExpression(nameof(Logger))] String  EventName     = "",
                                         [CallerMemberName()]                       String  OCPICommand   = "")

            where TDelegate : Delegate

            => LogEvent(
                   nameof(CPO2EMSPClient),
                   Logger,
                   LogHandler,
                   EventName,
                   OCPICommand
               );

        #endregion


        #region ToJSON()

        public override JObject ToJSON()
            => base.ToJSON(nameof(CPO2EMSPClient));

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

