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

using System.Runtime.CompilerServices;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_1_1.CPO.HTTP
{

    /// <summary>
    /// The CPO2EMSP client.
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

        public CPOAPI           CPOAPI      { get; }

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


        public CustomJObjectSerializerDelegate<CDR>?                         CustomCDRSerializer                          { get; set; }
        public CustomJObjectSerializerDelegate<Tariff>?                      CustomTariffSerializer                       { get; set; }
        public CustomJObjectSerializerDelegate<TariffElement>?               CustomTariffElementSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<PriceComponent>?              CustomPriceComponentSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<TariffRestrictions>?          CustomTariffRestrictionsSerializer           { get; set; }
        public CustomJObjectSerializerDelegate<ChargingPeriod>?              CustomChargingPeriodSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<CDRDimension>?                CustomCDRDimensionSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<CDRCostDetails>?              CustomCDRCostDetailsSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<SignedData>?                  CustomSignedDataSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<SignedValue>?                 CustomSignedValueSerializer                  { get; set; }

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
        /// <param name="CPOAPI">The CPOAPI.</param>
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

            this.CPOAPI      = CPOAPI;
            this.Counters    = new APICounters();

            base.HTTPLogger  = this.DisableLogging == false
                                   ? new Logger(
                                         this,
                                         LoggingPath,
                                         LoggingContext ?? DefaultLoggingContext,
                                         LogfileCreator
                                     )
                                   : null;

        }

        #endregion



        #region GetLocation    (LocationId, ...)

        /// <summary>
        /// Get the charging location specified by the given location identification from the remote API.
        /// </summary>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional cancellation token to cancel this request.</param>
        public async Task<OCPIResponse<Location>>

            GetLocation(Location_Id         LocationId,

                        Request_Id?         RequestId           = null,
                        Correlation_Id?     CorrelationId       = null,
                        Version_Id?         VersionId           = null,

                        EventTracking_Id?   EventTrackingId     = null,
                        TimeSpan?           RequestTimeout      = null,
                        CancellationToken   CancellationToken   = default)

        {

            #region Init

            var startTime        = Timestamp.Now;
            var requestId        = RequestId       ?? Request_Id.    NewRandom();
            var correlationId    = CorrelationId   ?? Correlation_Id.NewRandom();
            var eventTrackingId  = EventTrackingId ?? EventTracking_Id.New;
            var requestTimeout   = RequestTimeout  ?? this.RequestTimeout;

            Counters.GetLocation.IncRequests_OK();

            OCPIResponse<Location> response;

            #endregion

            #region Send OnGetLocationRequest event

            try
            {

                if (OnGetLocationRequest is not null)
                    await Task.WhenAll(OnGetLocationRequest.GetInvocationList().
                                       Cast<OnGetLocationRequestDelegate>().
                                       Select(e => e(startTime,
                                                     this,
                                                     requestId,
                                                     correlationId,

                                                     LocationId,

                                                     CancellationToken,
                                                     eventTrackingId,
                                                     requestTimeout))).
                                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(CPO2EMSPClient) + "." + nameof(OnGetLocationRequest));
            }

            #endregion


            try
            {

                var httpClient = await GetModuleHTTPClient(
                                           Module_Id.Locations,
                                           VersionId,
                                           eventTrackingId,
                                           CancellationToken
                                       );

                if (httpClient is not null)
                {

                    #region Upstream HTTP request...

                    var httpResponse = await httpClient.GET(
                                                 Path:                  httpClient.RemoteURL.Path + LocationId.ToString(),
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

            var endtime = Timestamp.Now;

            try
            {

                if (OnGetLocationResponse is not null)
                    await Task.WhenAll(OnGetLocationResponse.GetInvocationList().
                                       Cast<OnGetLocationResponseDelegate>().
                                       Select(e => e(endtime,
                                                     this,
                                                     requestId,
                                                     correlationId,

                                                     LocationId,

                                                     CancellationToken,
                                                     eventTrackingId,
                                                     requestTimeout,

                                                     response,
                                                     endtime - startTime))).
                                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(CPO2EMSPClient) + "." + nameof(OnGetLocationResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region PutLocation    (Location, ...)

        /// <summary>
        /// Put/store the given charging location on/within the remote API.
        /// </summary>
        /// <param name="Location">The charging location to store/put at/onto the remote API.</param>
        /// 
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional cancellation token to cancel this request.</param>
        public async Task<OCPIResponse<Location>>

            PutLocation(Location            Location,
                        EMSP_Id?            EMSPId              = null,

                        Request_Id?         RequestId           = null,
                        Correlation_Id?     CorrelationId       = null,
                        Version_Id?         VersionId           = null,

                        EventTracking_Id?   EventTrackingId     = null,
                        TimeSpan?           RequestTimeout      = null,
                        CancellationToken   CancellationToken   = default)

        {

            #region Init

            var startTime        = Timestamp.Now;
            var requestId        = RequestId       ?? Request_Id.    NewRandom();
            var correlationId    = CorrelationId   ?? Correlation_Id.NewRandom();
            var eventTrackingId  = EventTrackingId ?? EventTracking_Id.New;
            var requestTimeout   = RequestTimeout  ?? this.RequestTimeout;

            Counters.PutLocation.IncRequests_OK();

            OCPIResponse<Location> response;

            #endregion

            #region Send OnPutLocationRequest event

            try
            {

                if (OnPutLocationRequest is not null)
                    await Task.WhenAll(OnPutLocationRequest.GetInvocationList().
                                       Cast<OnPutLocationRequestDelegate>().
                                       Select(e => e(startTime,
                                                     this,
                                                     requestId,
                                                     correlationId,

                                                     Location,

                                                     CancellationToken,
                                                     eventTrackingId,
                                                     requestTimeout))).
                                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(CPO2EMSPClient) + "." + nameof(OnPutLocationRequest));
            }

            #endregion


            try
            {

                var httpClient = await GetModuleHTTPClient(
                                           Module_Id.Locations,
                                           VersionId,
                                           eventTrackingId,
                                           CancellationToken
                                       );

                if (httpClient is not null)
                {

                    #region Upstream HTTP request...

                    var httpResponse = await httpClient.PUT(
                                                 Path:                  httpClient.RemoteURL.Path + Location.Id.ToString(),
                                                 Content:               Location.ToJSON(
                                                                            false,
                                                                            false,
                                                                            EMSPId,
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
                                   json => Location.Parse(
                                               json,
                                               CommonAPI.OurCountryCode,
                                               CommonAPI.OurPartyId
                                           )
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

            var endtime = Timestamp.Now;

            try
            {

                if (OnPutLocationResponse is not null)
                    await Task.WhenAll(OnPutLocationResponse.GetInvocationList().
                                       Cast<OnPutLocationResponseDelegate>().
                                       Select(e => e(endtime,
                                                     this,
                                                     requestId,
                                                     correlationId,

                                                     Location,

                                                     CancellationToken,
                                                     eventTrackingId,
                                                     requestTimeout,

                                                     response,
                                                     endtime - startTime))).
                                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(CPO2EMSPClient) + "." + nameof(OnPutLocationResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region PatchLocation  (LocationId, LocationPatch, ...)

        /// <summary>
        /// Patch a location.
        /// </summary>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional cancellation token to cancel this request.</param>
        public async Task<OCPIResponse<Location>>

            PatchLocation(Location_Id         LocationId,
                          JObject             LocationPatch,

                          Request_Id?         RequestId           = null,
                          Correlation_Id?     CorrelationId       = null,
                          Version_Id?         VersionId           = null,

                          EventTracking_Id?   EventTrackingId     = null,
                          TimeSpan?           RequestTimeout      = null,
                          CancellationToken   CancellationToken   = default)

        {

            #region Initial checks

            if (!LocationPatch.HasValues)
                return OCPIResponse<Location>.Error("The given location patch must not be empty!");

            #endregion

            #region Init

            var startTime        = Timestamp.Now;
            var requestId        = RequestId       ?? Request_Id.    NewRandom();
            var correlationId    = CorrelationId   ?? Correlation_Id.NewRandom();
            var eventTrackingId  = EventTrackingId ?? EventTracking_Id.New;
            var requestTimeout   = RequestTimeout  ?? this.RequestTimeout;

            Counters.PatchLocation.IncRequests_OK();

            OCPIResponse<Location> response;

            #endregion

            #region Send OnPatchLocationRequest event

            try
            {

                if (OnPatchLocationRequest is not null)
                    await Task.WhenAll(OnPatchLocationRequest.GetInvocationList().
                                       Cast<OnPatchLocationRequestDelegate>().
                                       Select(e => e(startTime,
                                                     this,
                                                     requestId,
                                                     correlationId,

                                                     LocationId,
                                                     LocationPatch,

                                                     CancellationToken,
                                                     eventTrackingId,
                                                     requestTimeout))).
                                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(CPO2EMSPClient) + "." + nameof(OnPatchLocationRequest));
            }

            #endregion


            try
            {

                var httpClient = await GetModuleHTTPClient(
                                           Module_Id.Locations,
                                           VersionId,
                                           eventTrackingId,
                                           CancellationToken
                                       );

                if (httpClient is not null)
                {

                    #region Upstream HTTP request...

                    var httpResponse = await httpClient.PATCH(
                                                 Path:                  httpClient.RemoteURL.Path + LocationId.ToString(),
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

            var endtime = Timestamp.Now;

            try
            {

                if (OnPatchLocationResponse is not null)
                    await Task.WhenAll(OnPatchLocationResponse.GetInvocationList().
                                       Cast<OnPatchLocationResponseDelegate>().
                                       Select(e => e(endtime,
                                                     this,
                                                     requestId,
                                                     correlationId,

                                                     LocationId,
                                                     LocationPatch,

                                                     CancellationToken,
                                                     eventTrackingId,
                                                     requestTimeout,

                                                     response,
                                                     endtime - startTime))).
                                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(CPO2EMSPClient) + "." + nameof(OnPatchLocationResponse));
            }

            #endregion

            return response;

        }

        #endregion


        #region GetEVSE        (LocationId, EVSEUId, ...)

        /// <summary>
        /// Get the EVSE specified by the given EVSE unique identification from the remote API.
        /// </summary>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional cancellation token to cancel this request.</param>
        public async Task<OCPIResponse<EVSE>>

            GetEVSE(Location_Id         LocationId,
                    EVSE_UId            EVSEUId,

                    Request_Id?         RequestId           = null,
                    Correlation_Id?     CorrelationId       = null,
                    Version_Id?         VersionId           = null,

                    EventTracking_Id?   EventTrackingId     = null,
                    TimeSpan?           RequestTimeout      = null,
                    CancellationToken   CancellationToken   = default)

        {

            #region Init

            var startTime        = Timestamp.Now;
            var requestId        = RequestId       ?? Request_Id.    NewRandom();
            var correlationId    = CorrelationId   ?? Correlation_Id.NewRandom();
            var eventTrackingId  = EventTrackingId ?? EventTracking_Id.New;
            var requestTimeout   = RequestTimeout  ?? this.RequestTimeout;

            Counters.GetEVSE.IncRequests_OK();

            OCPIResponse<EVSE> response;

            #endregion

            #region Send OnGetEVSERequest event

            try
            {

                if (OnGetEVSERequest is not null)
                    await Task.WhenAll(OnGetEVSERequest.GetInvocationList().
                                       Cast<OnGetEVSERequestDelegate>().
                                       Select(e => e(startTime,
                                                     this,
                                                     requestId,
                                                     correlationId,

                                                     LocationId,
                                                     EVSEUId,

                                                     CancellationToken,
                                                     eventTrackingId,
                                                     requestTimeout))).
                                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(CPO2EMSPClient) + "." + nameof(OnGetEVSERequest));
            }

            #endregion


            try
            {

                var httpClient = await GetModuleHTTPClient(
                                           Module_Id.Locations,
                                           VersionId,
                                           eventTrackingId,
                                           CancellationToken
                                       );

                if (httpClient is not null)
                {

                    #region Upstream HTTP request...

                    var httpResponse = await httpClient.GET(
                                                 Path:                  httpClient.RemoteURL.Path + LocationId.ToString() +
                                                                                                    EVSEUId.   ToString(),
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

            var endtime = Timestamp.Now;

            try
            {

                if (OnGetEVSEResponse is not null)
                    await Task.WhenAll(OnGetEVSEResponse.GetInvocationList().
                                       Cast<OnGetEVSEResponseDelegate>().
                                       Select(e => e(endtime,
                                                     this,
                                                     requestId,
                                                     correlationId,

                                                     LocationId,
                                                     EVSEUId,

                                                     CancellationToken,
                                                     eventTrackingId,
                                                     requestTimeout,

                                                     response,
                                                     endtime - startTime))).
                                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(CPO2EMSPClient) + "." + nameof(OnPatchLocationResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region PutEVSE        (EVSE, ...)

        /// <summary>
        /// Put/store the given EVSE on/within the remote API.
        /// </summary>
        /// <param name="EVSE">The EVSE to store/put at/onto the remote API.</param>
        /// 
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional cancellation token to cancel this request.</param>
        public async Task<OCPIResponse<EVSE>>

            PutEVSE(EVSE                EVSE,
                    EMSP_Id?            EMSPId              = null,

                    Request_Id?         RequestId           = null,
                    Correlation_Id?     CorrelationId       = null,
                    Version_Id?         VersionId           = null,

                    EventTracking_Id?   EventTrackingId     = null,
                    TimeSpan?           RequestTimeout      = null,
                    CancellationToken   CancellationToken   = default)

        {

            if (EVSE.ParentLocation is null)
                return OCPIResponse<EVSE>.Error("The parent location of the given EVSE must not be null!");

            return await PutEVSE(
                             EVSE,
                             EVSE.ParentLocation.Id,
                             EMSPId,

                             RequestId,
                             CorrelationId,
                             VersionId,

                             EventTrackingId,
                             RequestTimeout,
                             CancellationToken
                         );

        }

        #endregion

        #region PutEVSE        (EVSE, LocationId, ...)

        /// <summary>
        /// Put/store the given EVSE on/within the remote API.
        /// </summary>
        /// <param name="EVSE">The EVSE to store/put at/onto the remote API.</param>
        /// <param name="LocationId">The identification of the location where to store the given EVSE.</param>
        /// 
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional cancellation token to cancel this request.</param>
        public async Task<OCPIResponse<EVSE>>

            PutEVSE(EVSE                EVSE,
                    Location_Id         LocationId,
                    EMSP_Id?            EMSPId              = null,

                    Request_Id?         RequestId           = null,
                    Correlation_Id?     CorrelationId       = null,
                    Version_Id?         VersionId           = null,

                    EventTracking_Id?   EventTrackingId     = null,
                    TimeSpan?           RequestTimeout      = null,
                    CancellationToken   CancellationToken   = default)

        {

            #region Init

            var startTime        = Timestamp.Now;
            var requestId        = RequestId       ?? Request_Id.    NewRandom();
            var correlationId    = CorrelationId   ?? Correlation_Id.NewRandom();
            var eventTrackingId  = EventTrackingId ?? EventTracking_Id.New;
            var requestTimeout   = RequestTimeout  ?? this.RequestTimeout;

            Counters.PutEVSE.IncRequests_OK();

            OCPIResponse<EVSE> response;

            #endregion

            #region Send OnPutEVSERequest event

            try
            {

                if (OnPutEVSERequest is not null)
                    await Task.WhenAll(OnPutEVSERequest.GetInvocationList().
                                       Cast<OnPutEVSERequestDelegate>().
                                       Select(e => e(startTime,
                                                     this,
                                                     requestId,
                                                     correlationId,

                                                     EVSE,
                                                     LocationId,

                                                     CancellationToken,
                                                     eventTrackingId,
                                                     requestTimeout))).
                                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(CPO2EMSPClient) + "." + nameof(OnPutEVSERequest));
            }

            #endregion


            try
            {

                var httpClient = await GetModuleHTTPClient(
                                           Module_Id.Locations,
                                           VersionId,
                                           eventTrackingId,
                                           CancellationToken
                                       );

                if (httpClient is not null)
                {

                    #region Upstream HTTP request...

                    var httpResponse = await httpClient.PUT(
                                                 Path:                  httpClient.RemoteURL.Path + LocationId.ToString() +
                                                                                                    EVSE.UId.  ToString(),
                                                 Content:               EVSE.ToJSON(
                                                                            EMSPId,
                                                                            false,
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

            var endtime = Timestamp.Now;

            try
            {

                if (OnPutEVSEResponse is not null)
                    await Task.WhenAll(OnPutEVSEResponse.GetInvocationList().
                                       Cast<OnPutEVSEResponseDelegate>().
                                       Select(e => e(endtime,
                                                     this,
                                                     requestId,
                                                     correlationId,

                                                     EVSE,
                                                     LocationId,

                                                     CancellationToken,
                                                     eventTrackingId,
                                                     requestTimeout,

                                                     response,
                                                     endtime - startTime))).
                                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(CPO2EMSPClient) + "." + nameof(OnPutEVSEResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region PatchEVSE      (LocationId, EVSEUId, EVSEPatch, ...)

        /// <summary>
        /// Patch a location.
        /// </summary>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional cancellation token to cancel this request.</param>
        public async Task<OCPIResponse<EVSE>>

            PatchEVSE(Location_Id         LocationId,
                      EVSE_UId            EVSEUId,
                      JObject             EVSEPatch,

                      Request_Id?         RequestId           = null,
                      Correlation_Id?     CorrelationId       = null,
                      Version_Id?         VersionId           = null,

                      EventTracking_Id?   EventTrackingId     = null,
                      TimeSpan?           RequestTimeout      = null,
                      CancellationToken   CancellationToken   = default)

        {

            #region Initial checks

            if (!EVSEPatch.HasValues)
                return OCPIResponse<EVSE>.Error(-1,
                                                "The given EVSE patch must not be empty!");

            #endregion

            #region Init

            var startTime        = Timestamp.Now;
            var requestId        = RequestId       ?? Request_Id.    NewRandom();
            var correlationId    = CorrelationId   ?? Correlation_Id.NewRandom();
            var eventTrackingId  = EventTrackingId ?? EventTracking_Id.New;
            var requestTimeout   = RequestTimeout  ?? this.RequestTimeout;

            Counters.PatchEVSE.IncRequests_OK();

            OCPIResponse<EVSE> response;

            #endregion

            #region Send OnPatchEVSERequest event

            try
            {

                if (OnPatchEVSERequest is not null)
                    await Task.WhenAll(OnPatchEVSERequest.GetInvocationList().
                                       Cast<OnPatchEVSERequestDelegate>().
                                       Select(e => e(startTime,
                                                     this,
                                                     requestId,
                                                     correlationId,

                                                     LocationId,
                                                     EVSEUId,
                                                     EVSEPatch,

                                                     CancellationToken,
                                                     eventTrackingId,
                                                     requestTimeout))).
                                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(CPO2EMSPClient) + "." + nameof(OnPatchEVSERequest));
            }

            #endregion


            try
            {

                var httpClient = await GetModuleHTTPClient(
                                           Module_Id.Locations,
                                           VersionId,
                                           eventTrackingId,
                                           CancellationToken
                                       );

                if (httpClient is not null)
                {

                    #region Upstream HTTP request...

                    var httpResponse = await httpClient.PATCH(
                                                 Path:                  httpClient.RemoteURL.Path + LocationId.ToString() +
                                                                                                    EVSEUId.   ToString(),
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

            var endtime = Timestamp.Now;

            try
            {

                if (OnPatchEVSEResponse is not null)
                    await Task.WhenAll(OnPatchEVSEResponse.GetInvocationList().
                                       Cast<OnPatchEVSEResponseDelegate>().
                                       Select(e => e(endtime,
                                                     this,
                                                     requestId,
                                                     correlationId,

                                                     LocationId,
                                                     EVSEUId,
                                                     EVSEPatch,

                                                     CancellationToken,
                                                     eventTrackingId,
                                                     requestTimeout,

                                                     response,
                                                     endtime - startTime))).
                                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(CPO2EMSPClient) + "." + nameof(OnPatchEVSEResponse));
            }

            #endregion

            return response;

        }

        #endregion


        #region GetConnector   (LocationId, EVSEUId, ConnectorId, ...)

        /// <summary>
        /// Get the connector specified by the given connector identification from the remote API.
        /// </summary>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional connector to cancel this request.</param>
        public async Task<OCPIResponse<Connector>>

            GetConnector(Location_Id         LocationId,
                         EVSE_UId            EVSEUId,
                         Connector_Id        ConnectorId,

                         Request_Id?         RequestId           = null,
                         Correlation_Id?     CorrelationId       = null,
                         Version_Id?         VersionId           = null,

                         EventTracking_Id?   EventTrackingId     = null,
                         TimeSpan?           RequestTimeout      = null,
                         CancellationToken   CancellationToken   = default)

        {

            #region Init

            var startTime        = Timestamp.Now;
            var requestId        = RequestId       ?? Request_Id.    NewRandom();
            var correlationId    = CorrelationId   ?? Correlation_Id.NewRandom();
            var eventTrackingId  = EventTrackingId ?? EventTracking_Id.New;
            var requestTimeout   = RequestTimeout  ?? this.RequestTimeout;

            Counters.GetConnector.IncRequests_OK();

            OCPIResponse<Connector> response;

            #endregion

            #region Send OnGetConnectorRequest event

            try
            {

                if (OnGetConnectorRequest is not null)
                    await Task.WhenAll(OnGetConnectorRequest.GetInvocationList().
                                       Cast<OnGetConnectorRequestDelegate>().
                                       Select(e => e(startTime,
                                                     this,
                                                     requestId,
                                                     correlationId,

                                                     LocationId,
                                                     EVSEUId,
                                                     ConnectorId,

                                                     CancellationToken,
                                                     eventTrackingId,
                                                     requestTimeout))).
                                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(CPO2EMSPClient) + "." + nameof(OnGetConnectorRequest));
            }

            #endregion


            try
            {

                var httpClient = await GetModuleHTTPClient(
                                           Module_Id.Locations,
                                           VersionId,
                                           eventTrackingId,
                                           CancellationToken
                                       );

                if (httpClient is not null)
                {

                    #region Upstream HTTP request...

                    var httpResponse = await httpClient.GET(
                                                 Path:                  httpClient.RemoteURL.Path + LocationId. ToString() +
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

            var endtime = Timestamp.Now;

            try
            {

                if (OnGetConnectorResponse is not null)
                    await Task.WhenAll(OnGetConnectorResponse.GetInvocationList().
                                       Cast<OnGetConnectorResponseDelegate>().
                                       Select(e => e(endtime,
                                                     this,
                                                     requestId,
                                                     correlationId,

                                                     LocationId,
                                                     EVSEUId,
                                                     ConnectorId,

                                                     CancellationToken,
                                                     eventTrackingId,
                                                     requestTimeout,

                                                     response,
                                                     endtime - startTime))).
                                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(CPO2EMSPClient) + "." + nameof(OnGetConnectorResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region PutConnector   (Connector, ...)

        /// <summary>
        /// Put/store the given charging connector on/within the remote API.
        /// </summary>
        /// <param name="Connector">The connector to store/put at/onto the remote API.</param>
        /// 
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional connector to cancel this request.</param>
        public async Task<OCPIResponse<Connector>>

            PutConnector(Connector           Connector,
                         EMSP_Id?            EMSPId              = null,

                         Request_Id?         RequestId           = null,
                         Correlation_Id?     CorrelationId       = null,
                         Version_Id?         VersionId           = null,

                         EventTracking_Id?   EventTrackingId     = null,
                         TimeSpan?           RequestTimeout      = null,
                         CancellationToken   CancellationToken   = default)

        {

            #region Initial checks

            if (Connector.ParentEVSE is null)
                return OCPIResponse<String, Connector>.Error("The parent EVSE of the connector must not be null!");

            if (Connector.ParentEVSE.ParentLocation is null)
                return OCPIResponse<String, Connector>.Error("The parent location of the connector must not be null!");

            #endregion

            #region Init

            var startTime        = Timestamp.Now;
            var requestId        = RequestId       ?? Request_Id.    NewRandom();
            var correlationId    = CorrelationId   ?? Correlation_Id.NewRandom();
            var eventTrackingId  = EventTrackingId ?? EventTracking_Id.New;
            var requestTimeout   = RequestTimeout  ?? this.RequestTimeout;

            Counters.PutConnector.IncRequests_OK();

            OCPIResponse<Connector> response;

            #endregion

            #region Send OnPutConnectorRequest event

            try
            {

                if (OnPutConnectorRequest is not null)
                    await Task.WhenAll(OnPutConnectorRequest.GetInvocationList().
                                       Cast<OnPutConnectorRequestDelegate>().
                                       Select(e => e(startTime,
                                                     this,
                                                     requestId,
                                                     correlationId,

                                                     Connector,

                                                     CancellationToken,
                                                     eventTrackingId,
                                                     requestTimeout))).
                                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(CPO2EMSPClient) + "." + nameof(OnPutConnectorRequest));
            }

            #endregion


            try
            {

                var httpClient = await GetModuleHTTPClient(
                                           Module_Id.Locations,
                                           VersionId,
                                           eventTrackingId,
                                           CancellationToken
                                       );

                if (httpClient is not null)
                {

                    #region Upstream HTTP request...

                    var httpResponse = await httpClient.PUT(
                                                 Path:                  httpClient.RemoteURL.Path + Connector.ParentEVSE.ParentLocation.Id. ToString() +
                                                                                                    Connector.ParentEVSE.               UId.ToString() +
                                                                                                    Connector.                          Id. ToString(),
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

                    response = OCPIResponse<Connector>.ParseJObject(httpResponse,
                                                                    requestId,
                                                                    correlationId,
                                                                    json => Connector.Parse(json));

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

            var endtime = Timestamp.Now;

            try
            {

                if (OnPutConnectorResponse is not null)
                    await Task.WhenAll(OnPutConnectorResponse.GetInvocationList().
                                       Cast<OnPutConnectorResponseDelegate>().
                                       Select(e => e(endtime,
                                                     this,
                                                     requestId,
                                                     correlationId,

                                                     Connector,

                                                     CancellationToken,
                                                     eventTrackingId,
                                                     requestTimeout,

                                                     response,
                                                     endtime - startTime))).
                                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(CPO2EMSPClient) + "." + nameof(OnPutConnectorResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region PatchConnector (LocationId, EVSEUId, ConnectorId, ConnectorPatch, ...)

        /// <summary>
        /// Patch a connector.
        /// </summary>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional connector to cancel this request.</param>
        public async Task<OCPIResponse<Connector>>

            PatchConnector(Location_Id         LocationId,
                           EVSE_UId            EVSEUId,
                           Connector_Id        ConnectorId,
                           JObject             ConnectorPatch,

                           Request_Id?         RequestId           = null,
                           Correlation_Id?     CorrelationId       = null,
                           Version_Id?         VersionId           = null,

                           EventTracking_Id?   EventTrackingId     = null,
                           TimeSpan?           RequestTimeout      = null,
                           CancellationToken   CancellationToken   = default)

        {

            #region Init

            var startTime        = Timestamp.Now;
            var requestId        = RequestId       ?? Request_Id.    NewRandom();
            var correlationId    = CorrelationId   ?? Correlation_Id.NewRandom();
            var eventTrackingId  = EventTrackingId ?? EventTracking_Id.New;
            var requestTimeout   = RequestTimeout  ?? this.RequestTimeout;

            Counters.PatchConnector.IncRequests_OK();

            OCPIResponse<Connector> response;

            #endregion

            #region Send OnPatchConnectorRequest event

            try
            {

                if (OnPatchConnectorRequest is not null)
                    await Task.WhenAll(OnPatchConnectorRequest.GetInvocationList().
                                       Cast<OnPatchConnectorRequestDelegate>().
                                       Select(e => e(startTime,
                                                     this,
                                                     requestId,
                                                     correlationId,

                                                     LocationId,
                                                     EVSEUId,
                                                     ConnectorId,
                                                     ConnectorPatch,

                                                     CancellationToken,
                                                     eventTrackingId,
                                                     requestTimeout))).
                                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(CPO2EMSPClient) + "." + nameof(OnPatchConnectorRequest));
            }

            #endregion


            try
            {

                var httpClient = await GetModuleHTTPClient(
                                           Module_Id.Locations,
                                           VersionId,
                                           eventTrackingId,
                                           CancellationToken
                                       );

                if (httpClient is not null)
                {

                    #region Upstream HTTP request...

                    var httpResponse = await httpClient.PATCH(
                                                 Path:                  httpClient.RemoteURL.Path + LocationId. ToString() +
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

            var endtime = Timestamp.Now;

            try
            {

                if (OnPatchConnectorResponse is not null)
                    await Task.WhenAll(OnPatchConnectorResponse.GetInvocationList().
                                       Cast<OnPatchConnectorResponseDelegate>().
                                       Select(e => e(endtime,
                                                     this,
                                                     requestId,
                                                     correlationId,

                                                     LocationId,
                                                     EVSEUId,
                                                     ConnectorId,
                                                     ConnectorPatch,

                                                     CancellationToken,
                                                     eventTrackingId,
                                                     requestTimeout,

                                                     response,
                                                     endtime - startTime))).
                                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(CPO2EMSPClient) + "." + nameof(OnPatchConnectorResponse));
            }

            #endregion

            return response;

        }

        #endregion


        #region GetTariff      (CountryCode, PartyId, TariffId, ...)

        /// <summary>
        /// Get the charging tariff specified by the given tariff identification from the remote API.
        /// </summary>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional tariff to cancel this request.</param>
        public async Task<OCPIResponse<Tariff>>

            GetTariff(CountryCode         CountryCode,
                      Party_Id            PartyId,
                      Tariff_Id           TariffId,

                      Request_Id?         RequestId           = null,
                      Correlation_Id?     CorrelationId       = null,
                      Version_Id?         VersionId           = null,

                      EventTracking_Id?   EventTrackingId     = null,
                      TimeSpan?           RequestTimeout      = null,
                      CancellationToken   CancellationToken   = default)

        {

            #region Init

            var startTime        = Timestamp.Now;
            var requestId        = RequestId       ?? Request_Id.    NewRandom();
            var correlationId    = CorrelationId   ?? Correlation_Id.NewRandom();
            var eventTrackingId  = EventTrackingId ?? EventTracking_Id.New;
            var requestTimeout   = RequestTimeout  ?? this.RequestTimeout;

            Counters.GetTariff.IncRequests_OK();

            OCPIResponse<Tariff> response;

            #endregion

            #region Send OnGetTariffRequest event

            try
            {

                if (OnGetTariffRequest is not null)
                    await Task.WhenAll(OnGetTariffRequest.GetInvocationList().
                                       Cast<OnGetTariffRequestDelegate>().
                                       Select(e => e(startTime,
                                                     this,
                                                     requestId,
                                                     correlationId,

                                                     CountryCode,
                                                     PartyId,
                                                     TariffId,

                                                     CancellationToken,
                                                     eventTrackingId,
                                                     requestTimeout))).
                                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(CPO2EMSPClient) + "." + nameof(OnGetTariffRequest));
            }

            #endregion


            try
            {

                var httpClient = await GetModuleHTTPClient(
                                           Module_Id.Tariffs,
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

            var endtime = Timestamp.Now;

            try
            {

                if (OnGetTariffResponse is not null)
                    await Task.WhenAll(OnGetTariffResponse.GetInvocationList().
                                       Cast<OnGetTariffResponseDelegate>().
                                       Select(e => e(endtime,
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
                                                     endtime - startTime))).
                                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(CPO2EMSPClient) + "." + nameof(OnGetTariffResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region PutTariff      (Tariff, ...)

        /// <summary>
        /// Put/store the given charging tariff on/within the remote API.
        /// </summary>
        /// <param name="Tariff">The charging tariff to store/put at/onto the remote API.</param>
        /// 
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional tariff to cancel this request.</param>
        public async Task<OCPIResponse<Tariff>>

            PutTariff(Tariff              Tariff,

                      Request_Id?         RequestId           = null,
                      Correlation_Id?     CorrelationId       = null,
                      Version_Id?         VersionId           = null,

                      EventTracking_Id?   EventTrackingId     = null,
                      TimeSpan?           RequestTimeout      = null,
                      CancellationToken   CancellationToken   = default)

        {

            #region Init

            var startTime        = Timestamp.Now;
            var requestId        = RequestId       ?? Request_Id.    NewRandom();
            var correlationId    = CorrelationId   ?? Correlation_Id.NewRandom();
            var eventTrackingId  = EventTrackingId ?? EventTracking_Id.New;
            var requestTimeout   = RequestTimeout  ?? this.RequestTimeout;

            Counters.PutTariff.IncRequests_OK();

            OCPIResponse<Tariff> response;

            #endregion

            #region Send OnPutTariffRequest event

            try
            {

                if (OnPutTariffRequest is not null)
                    await Task.WhenAll(OnPutTariffRequest.GetInvocationList().
                                       Cast<OnPutTariffRequestDelegate>().
                                       Select(e => e(startTime,
                                                     this,
                                                     requestId,
                                                     correlationId,

                                                     Tariff,

                                                     CancellationToken,
                                                     eventTrackingId,
                                                     requestTimeout))).
                                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(CPO2EMSPClient) + "." + nameof(OnPutTariffRequest));
            }

            #endregion


            try
            {

                var httpClient = await GetModuleHTTPClient(
                                           Module_Id.Tariffs,
                                           VersionId,
                                           eventTrackingId,
                                           CancellationToken
                                       );

                if (httpClient is not null)
                {

                    #region Upstream HTTP request...

                    var httpResponse = await httpClient.PUT(
                                                 Path:                  httpClient.RemoteURL.Path + Tariff.Id.ToString(),
                                                 Content:               Tariff.ToJSON().ToUTF8Bytes(JSONFormatting),
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

            var endtime = Timestamp.Now;

            try
            {

                if (OnPutTariffResponse is not null)
                    await Task.WhenAll(OnPutTariffResponse.GetInvocationList().
                                       Cast<OnPutTariffResponseDelegate>().
                                       Select(e => e(endtime,
                                                     this,
                                                     requestId,
                                                     correlationId,

                                                     Tariff,

                                                     CancellationToken,
                                                     eventTrackingId,
                                                     requestTimeout,

                                                     response,
                                                     endtime - startTime))).
                                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(CPO2EMSPClient) + "." + nameof(OnPutTariffResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region PatchTariff    (CountryCode, PartyId, TariffId, TariffPatch, ...)    [NonStandard]

        /// <summary>
        /// Patch a tariff.
        /// </summary>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional tariff to cancel this request.</param>
        public async Task<OCPIResponse<Tariff>>

            PatchTariff(CountryCode         CountryCode,
                        Party_Id            PartyId,
                        Tariff_Id           TariffId,
                        JObject             TariffPatch,

                        Request_Id?         RequestId           = null,
                        Correlation_Id?     CorrelationId       = null,
                        Version_Id?         VersionId           = null,

                        EventTracking_Id?   EventTrackingId     = null,
                        TimeSpan?           RequestTimeout      = null,
                        CancellationToken   CancellationToken   = default)

        {

            #region Initial checks

            if (!TariffPatch.HasValues)
                return OCPIResponse<Tariff>.Error("The given charging tariff patch must not be null!");

            #endregion

            #region Init

            var startTime        = Timestamp.Now;
            var requestId        = RequestId       ?? Request_Id.    NewRandom();
            var correlationId    = CorrelationId   ?? Correlation_Id.NewRandom();
            var eventTrackingId  = EventTrackingId ?? EventTracking_Id.New;
            var requestTimeout   = RequestTimeout  ?? this.RequestTimeout;

            Counters.PatchTariff.IncRequests_OK();

            OCPIResponse<Tariff> response;

            #endregion

            #region Send OnPatchTariffRequest event

            try
            {

                if (OnPatchTariffRequest is not null)
                    await Task.WhenAll(OnPatchTariffRequest.GetInvocationList().
                                       Cast<OnPatchTariffRequestDelegate>().
                                       Select(e => e(startTime,
                                                     this,
                                                     requestId,
                                                     correlationId,

                                                     CountryCode,
                                                     PartyId,
                                                     TariffId,
                                                     TariffPatch,

                                                     CancellationToken,
                                                     eventTrackingId,
                                                     requestTimeout))).
                                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(CPO2EMSPClient) + "." + nameof(OnPatchTariffRequest));
            }

            #endregion


            try
            {

                var httpClient = await GetModuleHTTPClient(
                                           Module_Id.Tariffs,
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

            var endtime = Timestamp.Now;

            try
            {

                if (OnPatchTariffResponse is not null)
                    await Task.WhenAll(OnPatchTariffResponse.GetInvocationList().
                                       Cast<OnPatchTariffResponseDelegate>().
                                       Select(e => e(endtime,
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
                                                     endtime - startTime))).
                                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(CPO2EMSPClient) + "." + nameof(OnPatchTariffResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region DeleteTariff   (CountryCode, PartyId, TariffId, ...)

        /// <summary>
        /// Delete the charging tariff specified by the given tariff identification from the remote API.
        /// </summary>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional tariff to cancel this request.</param>
        public async Task<OCPIResponse<Tariff>>

            DeleteTariff(CountryCode         CountryCode,
                         Party_Id            PartyId,
                         Tariff_Id           TariffId,

                         Request_Id?         RequestId           = null,
                         Correlation_Id?     CorrelationId       = null,
                         Version_Id?         VersionId           = null,

                         EventTracking_Id?   EventTrackingId     = null,
                         TimeSpan?           RequestTimeout      = null,
                         CancellationToken   CancellationToken   = default)

        {

            #region Init

            var startTime        = Timestamp.Now;
            var requestId        = RequestId       ?? Request_Id.    NewRandom();
            var correlationId    = CorrelationId   ?? Correlation_Id.NewRandom();
            var eventTrackingId  = EventTrackingId ?? EventTracking_Id.New;
            var requestTimeout   = RequestTimeout  ?? this.RequestTimeout;

            Counters.DeleteTariff.IncRequests_OK();

            OCPIResponse<Tariff> response;

            #endregion

            #region Send OnDeleteTariffRequest event

            try
            {

                if (OnDeleteTariffRequest is not null)
                    await Task.WhenAll(OnDeleteTariffRequest.GetInvocationList().
                                       Cast<OnDeleteTariffRequestDelegate>().
                                       Select(e => e(startTime,
                                                     this,
                                                     requestId,
                                                     correlationId,

                                                     CountryCode,
                                                     PartyId,
                                                     TariffId,

                                                     CancellationToken,
                                                     eventTrackingId,
                                                     requestTimeout))).
                                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(CPO2EMSPClient) + "." + nameof(OnDeleteTariffRequest));
            }

            #endregion


            try
            {

                var httpClient = await GetModuleHTTPClient(
                                           Module_Id.Tariffs,
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

            var endtime = Timestamp.Now;

            try
            {

                if (OnDeleteTariffResponse is not null)
                    await Task.WhenAll(OnDeleteTariffResponse.GetInvocationList().
                                       Cast<OnDeleteTariffResponseDelegate>().
                                       Select(e => e(endtime,
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
                                                     endtime - startTime))).
                                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(CPO2EMSPClient) + "." + nameof(OnDeleteTariffResponse));
            }

            #endregion

            return response;

        }

        #endregion


        #region GetSession     (CountryCode, PartyId, SessionId, ...)

        /// <summary>
        /// Get the charging session specified by the given session identification from the remote API.
        /// </summary>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional session to cancel this request.</param>
        public async Task<OCPIResponse<Session>>

            GetSession(CountryCode         CountryCode,
                       Party_Id            PartyId,
                       Session_Id          SessionId,

                       Request_Id?         RequestId           = null,
                       Correlation_Id?     CorrelationId       = null,
                       Version_Id?         VersionId           = null,

                       EventTracking_Id?   EventTrackingId     = null,
                       TimeSpan?           RequestTimeout      = null,
                       CancellationToken   CancellationToken   = default)

        {

            #region Init

            var startTime        = Timestamp.Now;
            var requestId        = RequestId       ?? Request_Id.    NewRandom();
            var correlationId    = CorrelationId   ?? Correlation_Id.NewRandom();
            var eventTrackingId  = EventTrackingId ?? EventTracking_Id.New;
            var requestTimeout   = RequestTimeout  ?? this.RequestTimeout;

            Counters.GetSession.IncRequests_OK();

            OCPIResponse<Session> response;

            #endregion

            #region Send OnGetSessionRequest event

            try
            {

                if (OnGetSessionRequest is not null)
                    await Task.WhenAll(OnGetSessionRequest.GetInvocationList().
                                       Cast<OnGetSessionRequestDelegate>().
                                       Select(e => e(startTime,
                                                     this,
                                                     requestId,
                                                     correlationId,

                                                     CountryCode,
                                                     PartyId,
                                                     SessionId,

                                                     CancellationToken,
                                                     eventTrackingId,
                                                     requestTimeout))).
                                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(CPO2EMSPClient) + "." + nameof(OnGetSessionRequest));
            }

            #endregion


            try
            {

                var httpClient = await GetModuleHTTPClient(
                                           Module_Id.Sessions,
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

            var endtime = Timestamp.Now;

            try
            {

                if (OnGetSessionResponse is not null)
                    await Task.WhenAll(OnGetSessionResponse.GetInvocationList().
                                       Cast<OnGetSessionResponseDelegate>().
                                       Select(e => e(endtime,
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
                                                     endtime - startTime))).
                                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(CPO2EMSPClient) + "." + nameof(OnGetSessionResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region PutSession     (Session, ...)

        /// <summary>
        /// Put/store the given charging session on/within the remote API.
        /// </summary>
        /// <param name="Session">The charging session to store/put at/onto the remote API.</param>
        /// 
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional session to cancel this request.</param>
        public async Task<OCPIResponse<Session>>

            PutSession(Session             Session,

                       Request_Id?         RequestId           = null,
                       Correlation_Id?     CorrelationId       = null,
                       Version_Id?         VersionId           = null,

                       EventTracking_Id?   EventTrackingId     = null,
                       TimeSpan?           RequestTimeout      = null,
                       CancellationToken   CancellationToken   = default)

        {

            #region Init

            var startTime        = Timestamp.Now;
            var requestId        = RequestId       ?? Request_Id.    NewRandom();
            var correlationId    = CorrelationId   ?? Correlation_Id.NewRandom();
            var eventTrackingId  = EventTrackingId ?? EventTracking_Id.New;
            var requestTimeout   = RequestTimeout  ?? this.RequestTimeout;

            Counters.PutSession.IncRequests_OK();

            OCPIResponse<Session> response;

            #endregion

            #region Send OnPutSessionRequest event

            try
            {

                if (OnPutSessionRequest is not null)
                    await Task.WhenAll(OnPutSessionRequest.GetInvocationList().
                                       Cast<OnPutSessionRequestDelegate>().
                                       Select(e => e(startTime,
                                                     this,
                                                     requestId,
                                                     correlationId,

                                                     Session,

                                                     CancellationToken,
                                                     eventTrackingId,
                                                     requestTimeout))).
                                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(CPO2EMSPClient) + "." + nameof(OnPutSessionRequest));
            }

            #endregion


            try
            {

                var httpClient = await GetModuleHTTPClient(
                                           Module_Id.Sessions,
                                           VersionId,
                                           eventTrackingId,
                                           CancellationToken
                                       );

                if (httpClient is not null)
                {

                    #region Upstream HTTP request...

                    var httpResponse = await httpClient.PUT(
                                                 Path:                  httpClient.RemoteURL.Path + Session.Id.ToString(),
                                                 Content:               Session.ToJSON().ToUTF8Bytes(JSONFormatting),
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

            try
            {

                if (OnPutSessionResponse is not null)
                    await Task.WhenAll(OnPutSessionResponse.GetInvocationList().
                                       Cast<OnPutSessionResponseDelegate>().
                                       Select(e => e(endtime,
                                                     this,
                                                     requestId,
                                                     correlationId,

                                                     Session,

                                                     CancellationToken,
                                                     eventTrackingId,
                                                     requestTimeout,

                                                     response,
                                                     endtime - startTime))).
                                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(CPO2EMSPClient) + "." + nameof(OnPutSessionResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region PatchSession   (CountryCode, PartyId, SessionId, SessionPatch, ...)

        /// <summary>
        /// Patch a session.
        /// </summary>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional session to cancel this request.</param>
        public async Task<OCPIResponse<Session>>

            PatchSession(CountryCode         CountryCode,
                         Party_Id            PartyId,
                         Session_Id          SessionId,
                         JObject             SessionPatch,

                         Request_Id?         RequestId           = null,
                         Correlation_Id?     CorrelationId       = null,
                         Version_Id?         VersionId           = null,

                         EventTracking_Id?   EventTrackingId     = null,
                         TimeSpan?           RequestTimeout      = null,
                         CancellationToken   CancellationToken   = default)

        {

            #region Initial checks

            if (!SessionPatch.HasValues)
                return OCPIResponse<Session>.Error("The given charging session patch must not be null!");

            #endregion

            #region Init

            var startTime        = Timestamp.Now;
            var requestId        = RequestId       ?? Request_Id.    NewRandom();
            var correlationId    = CorrelationId   ?? Correlation_Id.NewRandom();
            var eventTrackingId  = EventTrackingId ?? EventTracking_Id.New;
            var requestTimeout   = RequestTimeout  ?? this.RequestTimeout;

            Counters.PatchSession.IncRequests_OK();

            OCPIResponse<Session> response;

            #endregion

            #region Send OnPatchSessionRequest event

            try
            {

                if (OnPatchSessionRequest is not null)
                    await Task.WhenAll(OnPatchSessionRequest.GetInvocationList().
                                       Cast<OnPatchSessionRequestDelegate>().
                                       Select(e => e(startTime,
                                                     this,
                                                     requestId,
                                                     correlationId,

                                                     CountryCode,
                                                     PartyId,
                                                     SessionId,
                                                     SessionPatch,

                                                     CancellationToken,
                                                     eventTrackingId,
                                                     requestTimeout))).
                                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(CPO2EMSPClient) + "." + nameof(OnPatchSessionRequest));
            }

            #endregion


            try
            {

                var httpClient = await GetModuleHTTPClient(
                                           Module_Id.Sessions,
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

            var endtime = Timestamp.Now;

            try
            {

                if (OnPatchSessionResponse is not null)
                    await Task.WhenAll(OnPatchSessionResponse.GetInvocationList().
                                       Cast<OnPatchSessionResponseDelegate>().
                                       Select(e => e(endtime,
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
                                                     endtime - startTime))).
                                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(CPO2EMSPClient) + "." + nameof(OnPatchSessionResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region DeleteSession  (CountryCode, PartyId, SessionId, ...)    [NonStandard]

        /// <summary>
        /// Delete the charging session specified by the given session identification from the remote API.
        /// </summary>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional session to cancel this request.</param>
        public async Task<OCPIResponse<Session>>

            DeleteSession(CountryCode         CountryCode,
                          Party_Id            PartyId,
                          Session_Id          SessionId,

                          Request_Id?         RequestId           = null,
                          Correlation_Id?     CorrelationId       = null,
                          Version_Id?         VersionId           = null,

                          EventTracking_Id?   EventTrackingId     = null,
                          TimeSpan?           RequestTimeout      = null,
                          CancellationToken   CancellationToken   = default)

        {

            #region Init

            var startTime        = Timestamp.Now;
            var requestId        = RequestId       ?? Request_Id.    NewRandom();
            var correlationId    = CorrelationId   ?? Correlation_Id.NewRandom();
            var eventTrackingId  = EventTrackingId ?? EventTracking_Id.New;
            var requestTimeout   = RequestTimeout  ?? this.RequestTimeout;

            Counters.DeleteSession.IncRequests_OK();

            OCPIResponse<Session> response;

            #endregion

            #region Send OnDeleteSessionRequest event

            try
            {

                if (OnDeleteSessionRequest is not null)
                    await Task.WhenAll(OnDeleteSessionRequest.GetInvocationList().
                                       Cast<OnDeleteSessionRequestDelegate>().
                                       Select(e => e(startTime,
                                                     this,
                                                     requestId,
                                                     correlationId,

                                                     CountryCode,
                                                     PartyId,
                                                     SessionId,

                                                     CancellationToken,
                                                     eventTrackingId,
                                                     requestTimeout))).
                                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(CPO2EMSPClient) + "." + nameof(OnDeleteSessionRequest));
            }

            #endregion


            try
            {

                var httpClient = await GetModuleHTTPClient(
                                           Module_Id.Sessions,
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

            try
            {

                if (OnDeleteSessionResponse is not null)
                    await Task.WhenAll(OnDeleteSessionResponse.GetInvocationList().
                                       Cast<OnDeleteSessionResponseDelegate>().
                                       Select(e => e(endtime,
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
                                                     endtime - startTime))).
                                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(CPO2EMSPClient) + "." + nameof(OnDeleteSessionResponse));
            }

            #endregion

            return response;

        }

        #endregion


        #region PostCDR        (CDR, ...)

        /// <summary>
        /// Post/store the given charge detail record on/within the remote API.
        /// </summary>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional charge detail record to cancel this request.</param>
        public async Task<OCPIResponse>

            PostCDR(CDR                 CDR,
                    EMSP_Id?            EMSPId              = null,

                    Request_Id?         RequestId           = null,
                    Correlation_Id?     CorrelationId       = null,
                    Version_Id?         VersionId           = null,

                    EventTracking_Id?   EventTrackingId     = null,
                    TimeSpan?           RequestTimeout      = null,
                    CancellationToken   CancellationToken   = default)

        {

            #region Init

            var startTime        = Timestamp.Now;
            var requestId        = RequestId       ?? Request_Id.    NewRandom();
            var correlationId    = CorrelationId   ?? Correlation_Id.NewRandom();
            var eventTrackingId  = EventTrackingId ?? EventTracking_Id.New;
            var requestTimeout   = RequestTimeout  ?? this.RequestTimeout;

            Counters.PostCDR.IncRequests_OK();

            OCPIResponse response;

            #endregion

            #region Send OnPostCDRRequest event

            try
            {

                if (OnPostCDRRequest is not null)
                    await Task.WhenAll(OnPostCDRRequest.GetInvocationList().
                                       Cast<OnPostCDRRequestDelegate>().
                                       Select(e => e(startTime,
                                                     this,
                                                     requestId,
                                                     correlationId,

                                                     CDR,

                                                     CancellationToken,
                                                     eventTrackingId,
                                                     requestTimeout))).
                                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(CPO2EMSPClient) + "." + nameof(OnPostCDRRequest));
            }

            #endregion


            try
            {

                var httpClient = await GetModuleHTTPClient(
                                           Module_Id.CDRs,
                                           VersionId,
                                           eventTrackingId,
                                           CancellationToken
                                       );

                if (httpClient is not null)
                {

                    #region Upstream HTTP request...

                    var httpResponse = await httpClient.POST(
                                                 Path:                  httpClient.RemoteURL.Path,
                                                 Content:               CDR.ToJSON(
                                                                            false,
                                                                            false,
                                                                            false,
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

                    response = OCPIResponse.Parse(
                                   httpResponse,
                                   requestId,
                                   correlationId
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

            var endtime = Timestamp.Now;

            try
            {

                if (OnPostCDRResponse is not null)
                    await Task.WhenAll(OnPostCDRResponse.GetInvocationList().
                                       Cast<OnPostCDRResponseDelegate>().
                                       Select(e => e(endtime,
                                                     this,
                                                     requestId,
                                                     correlationId,

                                                     CDR,

                                                     CancellationToken,
                                                     eventTrackingId,
                                                     requestTimeout,

                                                     response,
                                                     endtime - startTime))).
                                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(CPO2EMSPClient) + "." + nameof(OnPostCDRResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region GetCDR         (CDRId, ...)   // The concrete URL is not specified by OCPI! m(

        /// <summary>
        /// Get the charge detail record specified by the given charge detail record identification from the remote API.
        /// </summary>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional charge detail record to cancel this request.</param>
        public async Task<OCPIResponse<CDR>>

            GetCDR(CDR_Id              CDRId,

                   Request_Id?         RequestId           = null,
                   Correlation_Id?     CorrelationId       = null,
                   Version_Id?         VersionId           = null,

                   EventTracking_Id?   EventTrackingId     = null,
                   TimeSpan?           RequestTimeout      = null,
                   CancellationToken   CancellationToken   = default)

        {

            #region Init

            var startTime        = Timestamp.Now;
            var requestId        = RequestId       ?? Request_Id.    NewRandom();
            var correlationId    = CorrelationId   ?? Correlation_Id.NewRandom();
            var eventTrackingId  = EventTrackingId ?? EventTracking_Id.New;
            var requestTimeout   = RequestTimeout  ?? this.RequestTimeout;

            Counters.GetCDR.IncRequests_OK();

            OCPIResponse<CDR> response;

            #endregion

            #region Send OnGetCDRRequest event

            try
            {

                if (OnGetCDRRequest is not null)
                    await Task.WhenAll(OnGetCDRRequest.GetInvocationList().
                                       Cast<OnGetCDRRequestDelegate>().
                                       Select(e => e(startTime,
                                                     this,
                                                     requestId,
                                                     correlationId,

                                                     CDRId,

                                                     CancellationToken,
                                                     eventTrackingId,
                                                     requestTimeout))).
                                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(CPO2EMSPClient) + "." + nameof(OnGetCDRRequest));
            }

            #endregion


            try
            {

                var httpClient = await GetModuleHTTPClient(
                                           Module_Id.CDRs,
                                           VersionId,
                                           eventTrackingId,
                                           CancellationToken
                                       );

                if (httpClient is not null)
                {

                    #region Upstream HTTP request...

                    var httpResponse = await httpClient.GET(
                                                 Path:                  httpClient.RemoteURL.Path + CDRId.ToString(),
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

            var endtime = Timestamp.Now;

            try
            {

                if (OnGetCDRResponse is not null)
                    await Task.WhenAll(OnGetCDRResponse.GetInvocationList().
                                       Cast<OnGetCDRResponseDelegate>().
                                       Select(e => e(endtime,
                                                     this,
                                                     requestId,
                                                     correlationId,

                                                     CDRId,

                                                     CancellationToken,
                                                     eventTrackingId,
                                                     requestTimeout,

                                                     response,
                                                     endtime - startTime))).
                                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(CPO2EMSPClient) + "." + nameof(OnGetCDRResponse));
            }

            #endregion

            return response;

        }

        #endregion


        #region GetTokens      (Offset = null, Limit = null, ...)

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
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<OCPIResponse<IEnumerable<Token>>>

            GetTokens(UInt64?             Offset              = null,
                      UInt64?             Limit               = null,

                      Version_Id?         VersionId           = null,
                      Request_Id?         RequestId           = null,
                      Correlation_Id?     CorrelationId       = null,

                      EventTracking_Id?   EventTrackingId     = null,
                      TimeSpan?           RequestTimeout      = null,
                      CancellationToken   CancellationToken   = default)

        {

            #region Init

            var startTime        = Timestamp.Now;
            var requestId        = RequestId       ?? Request_Id.    NewRandom();
            var correlationId    = CorrelationId   ?? Correlation_Id.NewRandom();
            var eventTrackingId  = EventTrackingId ?? EventTracking_Id.New;
            var requestTimeout   = RequestTimeout  ?? this.RequestTimeout;

            Counters.GetTokens.IncRequests_OK();

            OCPIResponse<IEnumerable<Token>> response;

            #endregion

            #region Send OnGetTokensRequest event

            try
            {

                if (OnGetTokensRequest is not null)
                    await Task.WhenAll(OnGetTokensRequest.GetInvocationList().
                                       Cast<OnGetTokensRequestDelegate>().
                                       Select(e => e(startTime,
                                                     this,
                                                     requestId,
                                                     correlationId,

                                                     Offset,
                                                     Limit,

                                                     CancellationToken,
                                                     eventTrackingId,
                                                     requestTimeout))).
                                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(CPO2EMSPClient) + "." + nameof(OnGetTokensRequest));
            }

            #endregion


            try
            {

                var httpClient = await GetModuleHTTPClient(
                                           Module_Id.Tokens,
                                           VersionId,
                                           eventTrackingId,
                                           CancellationToken
                                       );

                var offsetLimit    = "";

                if (Offset.HasValue)
                    offsetLimit += "&offset=" + Offset.Value;

                if (Limit.HasValue)
                    offsetLimit += "&limit="  + Limit. Value;

                if (offsetLimit.Length > 0)
                    offsetLimit = String.Concat("?", offsetLimit.AsSpan(1));


                if (httpClient is not null)
                {

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

            var endtime = Timestamp.Now;

            try
            {

                if (OnGetTokensResponse is not null)
                    await Task.WhenAll(OnGetTokensResponse.GetInvocationList().
                                       Cast<OnGetTokensResponseDelegate>().
                                       Select(e => e(endtime,
                                                     this,
                                                     requestId,
                                                     correlationId,

                                                     Offset,
                                                     Limit,

                                                     CancellationToken,
                                                     eventTrackingId,
                                                     requestTimeout,

                                                     response,
                                                     endtime - startTime))).
                                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(CPO2EMSPClient) + "." + nameof(OnGetTokensResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region PostToken      (TokenId, TokenType = null, ...)

        /// <summary>
        /// Post/store the given token identification on/within the remote API.
        /// </summary>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<OCPIResponse<AuthorizationInfo>>

            PostToken(Token_Id            TokenId,
                      TokenType?          TokenType           = null,
                      LocationReference?  LocationReference   = null,

                      Request_Id?         RequestId           = null,
                      Correlation_Id?     CorrelationId       = null,
                      Version_Id?         VersionId           = null,

                      EventTracking_Id?   EventTrackingId     = null,
                      TimeSpan?           RequestTimeout      = null,
                      CancellationToken   CancellationToken   = default)

        {

            #region Init

            var startTime        = Timestamp.Now;
            var requestId        = RequestId       ?? Request_Id.    NewRandom();
            var correlationId    = CorrelationId   ?? Correlation_Id.NewRandom();
            var eventTrackingId  = EventTrackingId ?? EventTracking_Id.New;
            var requestTimeout   = RequestTimeout  ?? this.RequestTimeout;

            Counters.PostToken.IncRequests_OK();

            OCPIResponse<AuthorizationInfo> response;

            #endregion

            #region Send OnPostTokenRequest event

            try
            {

                if (OnPostTokenRequest is not null)
                    await Task.WhenAll(OnPostTokenRequest.GetInvocationList().
                                       Cast<OnPostTokenRequestDelegate>().
                                       Select(e => e(startTime,
                                                     this,
                                                     requestId,
                                                     correlationId,

                                                     TokenId,
                                                     TokenType,
                                                     LocationReference,

                                                     CancellationToken,
                                                     eventTrackingId,
                                                     requestTimeout))).
                                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(CPO2EMSPClient) + "." + nameof(OnPostTokenRequest));
            }

            #endregion


            try
            {

                var httpClient = await GetModuleHTTPClient(
                                           Module_Id.Tokens,
                                           VersionId,
                                           eventTrackingId,
                                           CancellationToken
                                       );

                if (httpClient is not null)
                {

                    #region Upstream HTTP request...

                    var httpResponse = await httpClient.POST(
                                                 Path:                  httpClient.RemoteURL.Path + TokenId.ToString() + "authorize",
                                                 Content:               null,
                                                 Authentication:        TokenAuth,
                                                 RequestBuilder:        requestBuilder => {

                                                                            requestBuilder.Set("X-Request-ID",     requestId);
                                                                            requestBuilder.Set("X-Correlation-ID", correlationId);

                                                                            if (TokenType.HasValue)
                                                                                requestBuilder.QueryString.Add("type", TokenType.Value.ToString());

                                                                            if (LocationReference.HasValue)
                                                                            {
                                                                                requestBuilder.ContentType = HTTPContentType.Application.JSON_UTF8;
                                                                                requestBuilder.Content     = LocationReference.Value.ToJSON().ToUTF8Bytes(JSONFormatting);
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


            var endtime = Timestamp.Now;
            var runtime = endtime - startTime;

            if (response.Data is not null)
                response.Data.Runtime = runtime;


            #region Send OnPostTokenResponse event

            try
            {

                if (OnPostTokenResponse is not null)
                    await Task.WhenAll(OnPostTokenResponse.GetInvocationList().
                                       Cast<OnPostTokenResponseDelegate>().
                                       Select(e => e(endtime,
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
                                                     runtime))).
                                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(CPO2EMSPClient) + "." + nameof(OnPostTokenResponse));
            }

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
