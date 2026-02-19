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

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;
using org.GraphDefined.Vanaheimr.Hermod;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_3_0
{

    /// <summary>
    /// A HTTP delegate.
    /// </summary>
    /// <param name="Request">The HTTP request.</param>
    /// <returns>A HTTP response task.</returns>
    public delegate Task<OCPIResponse.Builder> OCPIRequestDelegate(OCPIRequest Request);


    /// <summary>
    /// Extension methods for OCPI requests.
    /// </summary>
    public static class OCPIRequestExtensions
    {

        public static void AddOCPIMethod(this CommonAPI       CommonAPI,
                                         HTTPMethod           HTTPMethod,
                                         HTTPPath             URLTemplate,
                                         OCPIRequestDelegate  OCPIRequestHandler,
                                         URLReplacement       AllowReplacement   = URLReplacement.Fail)

            => AddOCPIMethod(

                   CommonAPI,
                   HTTPMethod,
                   URLTemplate,
                   OCPIRequestHandler,
                   HTTPMethod == HTTPMethod.OPTIONS
                       ? null
                       : HTTPContentType.Application.JSON_UTF8,

                   null, //OCPIRequestLogger,
                   null, //OCPIResponseLogger,

                   null, //DefaultErrorHandler,
                   AllowReplacement

               );


        public static void AddOCPIMethod(this CommonAPI          CommonAPI,
                                         HTTPMethod              HTTPMethod,
                                         HTTPPath                URLTemplate,
                                         OCPIRequestLogHandler   OCPIRequestLogger,
                                         OCPIResponseLogHandler  OCPIResponseLogger,
                                         OCPIRequestDelegate     OCPIRequestHandler,

                                         HTTPDelegate?           DefaultErrorHandler   = null,
                                         URLReplacement          AllowReplacement      = URLReplacement.Fail)

            => AddOCPIMethod(

                   CommonAPI,
                   HTTPMethod,
                   URLTemplate,
                   OCPIRequestHandler,
                   HTTPMethod == HTTPMethod.OPTIONS
                       ? null
                       : HTTPContentType.Application.JSON_UTF8,

                   OCPIRequestLogger,
                   OCPIResponseLogger,

                   DefaultErrorHandler,
                   AllowReplacement

               );


        #region AddOCPIMethod(CommonAPI, HTTPMethod, URLTemplate, HTTPContentType, OCPIRequestHandler, HTTPContentType, ...)

        /// <summary>
        /// Add a method callback for the given URL template.
        /// </summary>
        /// <param name="CommonAPI">The OCPI CommonAPI.</param>
        /// <param name="HTTPMethod">The HTTP method.</param>
        /// <param name="URLTemplate">The URL template.</param>
        /// <param name="HTTPContentType">The HTTP content type.</param>
        /// <param name="OCPIRequestLogger">A OCPI request logger.</param>
        /// <param name="OCPIResponseLogger">A OCPI response logger.</param>
        /// <param name="DefaultErrorHandler">The default error handler.</param>
        /// <param name="OCPIRequestHandler">The method to call.</param>
        public static void AddOCPIMethod(this CommonAPI           CommonAPI,
                                         HTTPMethod               HTTPMethod,
                                         HTTPPath                 URLTemplate,
                                         OCPIRequestDelegate      OCPIRequestHandler,
                                         HTTPContentType?         HTTPContentType,

                                         OCPIRequestLogHandler?   OCPIRequestLogger     = null,
                                         OCPIResponseLogHandler?  OCPIResponseLogger    = null,

                                         HTTPDelegate?            DefaultErrorHandler   = null,
                                         URLReplacement           AllowReplacement      = URLReplacement.Fail)

        {

            CommonAPI.HTTPBaseAPI.AddHandler(

                URLTemplate,
                async httpRequest => {

                    try
                    {

                        // When no OCPIRequestLogger was used!
                        httpRequest.SubprotocolRequest ??= OCPIRequest.Parse(
                                                               httpRequest, 
                                                               CommonAPI
                                                           );

                        if (httpRequest.SubprotocolRequest is OCPIRequest ocpiRequest)
                        {

                            if (ocpiRequest.AccessTokenErrorMessages.Any())
                            {

                                  //             AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                  //             Allow                      = [ HTTPMethod.OPTIONS, HTTPMethod.GET ],

                                return new HTTPResponse.Builder() {
                                           HTTPStatusCode             = HTTPStatusCode.Unauthorized,
                                           Server                     = CommonAPI.HTTPServiceName,
                                           Date                       = Timestamp.Now,
                                           AccessControlAllowOrigin   = "*",
                                           AccessControlAllowHeaders  = [ "Authorization" ],
                                           ContentType                = HTTPContentType.Application.JSON_UTF8,
                                           Content                    = new OCPIResponse(
                                                                           ocpiRequest,
                                                                           2000,
                                                                           ocpiRequest.AccessTokenErrorMessages.First(),
                                                                           ocpiRequest.AccessTokenErrorMessages.Count() > 1
                                                                               ? ocpiRequest.AccessTokenErrorMessages.Skip(1).AggregateWith(", ")
                                                                               : null,
                                                                           Timestamp.Now
                                                                       ).ToJSON().ToUTF8Bytes(),
                                           Connection                 = ConnectionType.KeepAlive,
                                           Vary                       = "Accept"
                                       };

                            }

                            var ocpiResponseBuilder  = await OCPIRequestHandler(ocpiRequest);

                            var httpResponseBuilder  = ocpiResponseBuilder.ToHTTPResponseBuilder();

                            httpResponseBuilder.SubprotocolResponse = new OCPIResponse(
                                                                          ocpiResponseBuilder.Request,
                                                                          ocpiResponseBuilder.StatusCode    ?? 3000,
                                                                          ocpiResponseBuilder.StatusMessage ?? "error!",
                                                                          ocpiResponseBuilder.AdditionalInformation,
                                                                          ocpiResponseBuilder.Timestamp     ?? Timestamp.Now,
                                                                          httpResponseBuilder.AsImmutable
                                                                      );

                            return httpResponseBuilder;

                        }

                        var ocpiResponseBuilderX  = new OCPIResponse.Builder(httpRequest.SubprotocolRequest as OCPIRequest) {
                                                        StatusCode    = 2001,
                                                        StatusMessage = "Invalid OCPI request!"
                                                    };
                        var httpResponseBuilderX  = new HTTPResponse.Builder();

                        httpResponseBuilderX.SubprotocolResponse = new OCPIResponse(
                                                                       ocpiResponseBuilderX.Request,
                                                                       ocpiResponseBuilderX.StatusCode    ?? 3000,
                                                                       ocpiResponseBuilderX.StatusMessage ?? "error!",
                                                                       ocpiResponseBuilderX.AdditionalInformation,
                                                                       ocpiResponseBuilderX.Timestamp     ?? Timestamp.Now,
                                                                       httpResponseBuilderX.AsImmutable
                                                                   );

                        return httpResponseBuilderX;

                    }
                    catch (Exception e)
                    {

                        return new HTTPResponse.Builder() {
                                   HTTPStatusCode  = HTTPStatusCode.InternalServerError,
                                   ContentType     = HTTPContentType.Application.JSON_UTF8,
                                   Content         = new OCPIResponse<JObject>(
                                                         JSONObject.Create(
                                                             new JProperty("description",  e.Message),
                                                             new JProperty("stacktrace",   new JArray(e.StackTrace?.Split(new[] { Environment.NewLine }, StringSplitOptions.None).ToArray() ?? [])),
                                                             new JProperty("source",       e.TargetSite?.Module.Name),
                                                             new JProperty("type",         e.TargetSite?.ReflectedType?.Name)
                                                         ),
                                                         2000,
                                                         e.Message,
                                                         null,
                                                         Timestamp.Now,
                                                         null,
                                                         (httpRequest.SubprotocolRequest as OCPIRequest)?.RequestId,
                                                         (httpRequest.SubprotocolRequest as OCPIRequest)?.CorrelationId
                                                     ).ToJSON(json => json).ToUTF8Bytes(),
                                   Connection      = ConnectionType.Close
                               };

                    }

                },
                HTTPMethod,
                HTTPContentType,

                null, //URLAuthentication,
                null, //HTTPMethodAuthentication,
                null, //ContentTypeAuthentication,

                (timestamp, httpAPI, httpRequest, ct) => {
                    return OCPIRequestLogger?.Invoke(
                               timestamp,
                               httpAPI,
                               OCPIRequest.Parse(
                                   httpRequest,
                                   CommonAPI
                               ),
                               ct
                           ) ?? Task.CompletedTask;
                },

                (timestamp, httpAPI, httpRequest, httpResponse, ct) => {

                    if (httpRequest.SubprotocolRequest is OCPIRequest ocpiRequest)
                        return OCPIResponseLogger?.Invoke(
                                   timestamp,
                                   httpAPI,
                                   ocpiRequest,
                                   (httpResponse.SubprotocolResponse as OCPIResponse)
                                       ?? new OCPIResponse(
                                                  ocpiRequest,
                                                  2000,
                                                  "OCPIResponse is null!",
                                                  httpResponse.HTTPBodyAsUTF8String,
                                                  httpResponse.Timestamp,
                                                  httpResponse
                                              ),
                                   ct
                               ) ?? Task.CompletedTask;

                    return Task.CompletedTask;

                },

                DefaultErrorHandler,
                null,

                AllowReplacement

            );

        }

        #endregion

    }


    /// <summary>
    /// An OCPI HTTP request.
    /// </summary>
    public class OCPIRequest // : HTTPRequest
    {

        #region DateAndPaginationFilters

        /// <summary>
        /// Date and pagination filters.
        /// </summary>
        /// <param name="From">An optional 'from' timestamp (inclusive).</param>
        /// <param name="To">An optional 'to' timestamp (exclusive).</param>
        /// <param name="Offset">An optional 'offset' within the result set.</param>
        /// <param name="Limit">An optional 'limit' of the result set.</param>
        public readonly struct DateAndPaginationFilters(DateTimeOffset?  From,
                                                        DateTimeOffset?  To,
                                                        UInt64?          Offset,
                                                        UInt64?          Limit)
        {

            #region Properties

            /// <summary>
            /// The optional 'from' timestamp (inclusive).
            /// </summary>
            public DateTimeOffset?  From      { get; } = From;

            /// <summary>
            /// The optional 'to' timestamp (exclusive).
            /// </summary>
            public DateTimeOffset?  To        { get; } = To;

            /// <summary>
            /// The optional 'offset' within the result set.
            /// </summary>
            public UInt64?          Offset    { get; } = Offset;

            /// <summary>
            /// The optional 'limit' of the result set.
            /// </summary>
            public UInt64?          Limit     { get; } = Limit;

            #endregion


            #region ToHTTPQueryString()

            /// <summary>
            /// Return a HTTP QueryString representation of this object.
            /// </summary>
            public String ToHTTPQueryString()

                => (From.  HasValue ||
                    To.    HasValue ||
                    Offset.HasValue ||
                    Limit. HasValue)

                    ? "?" + new String[] {
                          From.  HasValue ? "date_from=" + From.  Value.ToISO8601() : "",
                          To.    HasValue ? "date_to="   + To.    Value.ToISO8601() : "",
                          Offset.HasValue ? "offset="    + Offset.Value.ToString()  : "",
                          Limit. HasValue ? "limit="     + Limit. Value.ToString()  : ""
                      }.Where(text => text.IsNotNullOrEmpty()).
                        AggregateWith("&")

                    : "";

            #endregion

            #region (override) ToString()

            /// <summary>
            /// Return a text representation of this object.
            /// </summary>
            public override String ToString()

                => (From.  HasValue ||
                    To.    HasValue ||
                    Offset.HasValue ||
                    Limit. HasValue)

                    ? new String[] {
                          From.  HasValue ? "from: "   + From.  Value.ToString() : "",
                          To.    HasValue ? "to: "     + To.    Value.ToString() : "",
                          Offset.HasValue ? "offset: " + Offset.Value.ToString() : "",
                          Limit. HasValue ? "limit: "  + Limit. Value.ToString() : ""
                      }.Where(text => text.IsNotNullOrEmpty()).
                        AggregateWith(", ")

                    : "";

            #endregion


        }

        public DateAndPaginationFilters GetDateAndPaginationFilters()

            => new (HTTPRequest.QueryString.GetDateTime("date_from"),
                    HTTPRequest.QueryString.GetDateTime("date_to"),
                    HTTPRequest.QueryString.GetUInt64  ("offset"),
                    HTTPRequest.QueryString.GetUInt64  ("limit"));

        #endregion


        #region Properties

        public CommonAPI             CommonAPI                   { get; }

        public HTTPRequest           HTTPRequest                 { get; }

        public Request_Id?           RequestId                   { get; }
        public Correlation_Id?       CorrelationId               { get; }

        public Party_Idv3?           From                        { get; }
        public Party_Idv3?           To                          { get; }
        //public CountryCode?          ToCountryCode               { get; }
        //public Party_Id?             ToPartyId                   { get; }
        //public CountryCode?          FromCountryCode             { get; }
        //public Party_Id?             FromPartyId                 { get; }

        public AccessToken?          AccessToken                 { get; }

        public IEnumerable<String>   AccessTokenErrorMessages    { get; }

        public LocalAccessInfo2?     LocalAccessInfo             { get; }

        public RemoteParty?          RemoteParty                 { get; }

        public IEnumerable<CPO_Id>   CPOIds                      { get; } = [];

        public IEnumerable<EMSP_Id>  EMSPIds                     { get; } = [];

        public IEnumerable<EMSP_Id>  HUBIds                      { get; } = [];

        public CPO_Id?               CPOId                       { get; }

        public EMSP_Id?              EMSPId                      { get; }

        public EMSP_Id?              HUBId                       { get; }


        /// <summary>
        /// The HTTP query string.
        /// </summary>
        public HTTPHostname          Host
            => HTTPRequest.Host;

        /// <summary>
        /// The parsed URL parameters of the best matching URL template.
        /// Set by the HTTP server.
        /// </summary>
        public String[]              ParsedURLParameters
            => HTTPRequest.ParsedURLParameters;

        /// <summary>
        /// The HTTP query string.
        /// </summary>
        public QueryString           QueryString
            => HTTPRequest.QueryString;

        #endregion

        protected OCPIRequest(HTTPRequest  Request,
                              CommonAPI    CommonAPI)
        {

            this.HTTPRequest      = Request ?? throw new ArgumentNullException(nameof(Request), "The given HTTP request must not be null!");
            this.CommonAPI        = CommonAPI;

            this.RequestId        = Request.TryParseHeaderStruct               ("X-Request-ID",           Request_Id.    TryParse, Request_Id.    NewRandom(IsLocal: true));
            this.CorrelationId    = Request.TryParseHeaderStruct               ("X-Correlation-ID",       Correlation_Id.TryParse, Correlation_Id.NewRandom(IsLocal: true));

            var  ToCountryCode    = Request.TryParseHeaderStruct<CountryCode>  ("OCPI-to-country-code",   CountryCode.   TryParse);
            var  ToPartyId        = Request.TryParseHeaderStruct<Party_Id>     ("OCPI-to-party-id",       Party_Id.      TryParse);
            if (ToCountryCode.HasValue &&
                ToPartyId.    HasValue)
            {
                To    = Party_Idv3.From(
                           ToCountryCode.Value,
                           ToPartyId.    Value
                        );
            }

            var  FromCountryCode  = Request.TryParseHeaderStruct<CountryCode>  ("OCPI-from-country-code", CountryCode.   TryParse);
            var  FromPartyId      = Request.TryParseHeaderStruct<Party_Id>     ("OCPI-from-party-id",     Party_Id.      TryParse);
            if (FromCountryCode.HasValue &&
                FromPartyId.    HasValue)
            {
                From  = Party_Idv3.From(
                           FromCountryCode.Value,
                           FromPartyId.    Value
                        );
            }

            var  totp             = Request.TryParseHeaderField<TOTPHTTPHeader>("TOTP",                   TOTPHTTPHeader.TryParse);

            AccessToken?      accessTokenRAW                  = null;
            String?           accessTokenErrorMessageRAW      = null;
            AccessToken?      accessTokenBASE64               = null;
            String?           accessTokenErrorMessageBASE64   = null;
            LocalAccessInfo?  localAccessInfo                 = null;

            var accessTokenErrorMessages   = new HashSet<String>();
            this.AccessTokenErrorMessages  = accessTokenErrorMessages;

            if (Request.Authorization is HTTPTokenAuthentication tokenAuth)
            {

                if (OCPI.AccessToken.TryParse          (tokenAuth.Token, out var parsedAccessToken))
                    accessTokenRAW     = parsedAccessToken;

                if (OCPI.AccessToken.TryParseFromBASE64(tokenAuth.Token, out var decodedAccessToken))
                    accessTokenBASE64  = decodedAccessToken;

            }

            else if (Request.Authorization is HTTPBasicAuthentication basicAuth)
            {

                if (OCPI.AccessToken.TryParse          (basicAuth.Username, out var parsedAccessToken))
                    accessTokenRAW     = parsedAccessToken;

                if (OCPI.AccessToken.TryParseFromBASE64(basicAuth.Username, out var decodedAccessToken))
                    accessTokenBASE64  = decodedAccessToken;

                totp                   = TOTPHTTPHeader.Parse(basicAuth.Password);

            }

            if (accessTokenRAW.HasValue &&
                CommonAPI.TryGetRemoteParties(accessTokenRAW.Value,
                                              totp,
                                              null,
                                              out var partiesAccessInfosRAW,
                                              out     accessTokenErrorMessageRAW))
            {
                var tuple = partiesAccessInfosRAW.FirstOrDefault();
                if (tuple is not null)
                {
                    if (!tuple.Item2.AccessTokenIsBase64Encoded)
                    {
                        AccessToken      = accessTokenRAW;
                        RemoteParty      = tuple.Item1;
                        localAccessInfo  = tuple.Item2;
                    }
                }
            }

            if (accessTokenBASE64.HasValue &&
                CommonAPI.TryGetRemoteParties(accessTokenBASE64.Value,
                                              totp,
                                              null,
                                              out var partiesAccessInfosBASE64,
                                              out     accessTokenErrorMessageBASE64))
            {
                var tuple = partiesAccessInfosBASE64.FirstOrDefault();
                if (tuple is not null)
                {
                    if (tuple.Item2.AccessTokenIsBase64Encoded)
                    {
                        AccessToken      = accessTokenBASE64;
                        RemoteParty      = tuple.Item1;
                        localAccessInfo  = tuple.Item2;
                    }
                }
            }

            if (AccessToken.HasValue &&
                RemoteParty     is not null &&
                localAccessInfo is not null)
            {

                LocalAccessInfo  = new LocalAccessInfo2(
                                       AccessToken.    Value,
                                       localAccessInfo.Status,
                                       RemoteParty.    Roles,
                                       RemoteParty.LocalAccessInfos. FirstOrDefault()?.NotBefore,
                                       RemoteParty.LocalAccessInfos. FirstOrDefault()?.NotAfter,
                                       RemoteParty.RemoteAccessInfos.FirstOrDefault()?.VersionsURL
                                   );

                CPOIds           = [.. RemoteParty.Roles.Where (credentialsRole => credentialsRole.Role == Role.CPO).
                                                         Select(credentialsRole => CPO_Id. Parse($"{LocalAccessInfo.Roles.First().PartyId.CountryCode}*{LocalAccessInfo.Roles.First().PartyId.PartyId}")).
                                                         Distinct()];

                EMSPIds          = [.. RemoteParty.Roles.Where (credentialsRole => credentialsRole.Role == Role.EMSP).
                                                         Select(credentialsRole => EMSP_Id.Parse($"{LocalAccessInfo.Roles.First().PartyId.CountryCode}-{LocalAccessInfo.Roles.First().PartyId.PartyId}")).
                                                         Distinct()];

                HUBIds           = [.. RemoteParty.Roles.Where (credentialsRole => credentialsRole.Role == Role.HUB).
                                                         Select(credentialsRole => EMSP_Id.Parse($"{LocalAccessInfo.Roles.First().PartyId.CountryCode}-{LocalAccessInfo.Roles.First().PartyId.PartyId}")).
                                                         Distinct()];

                if (FromCountryCode.HasValue &&
                    FromPartyId.    HasValue)
                {

                    CPOId   = CPO_Id. Parse($"{FromCountryCode}*{FromPartyId}");
                    EMSPId  = EMSP_Id.Parse($"{FromCountryCode}-{FromPartyId}");
                    HUBId   = EMSP_Id.Parse($"{FromCountryCode}-{FromPartyId}");

                    if (CPOId. HasValue && !CPOIds. Contains(CPOId. Value))
                        CPOId   = null;

                    if (EMSPId.HasValue && !EMSPIds.Contains(EMSPId.Value))
                        EMSPId  = null;

                    if (HUBId. HasValue && !HUBIds. Contains(HUBId. Value))
                        HUBId   = null;

                }

                if (!FromCountryCode.HasValue &&
                    !FromPartyId.    HasValue)
                {

                    if (CPOIds. Any())
                        CPOId   = CPOIds. First();

                    if (EMSPIds.Any())
                        EMSPId  = EMSPIds.First();

                    if (HUBIds. Any())
                        HUBId   = HUBIds. First();

                }

                if (RemoteParty.IN?.RequestModifier is not null)
                    HTTPRequest = RemoteParty.IN.RequestModifier(HTTPRequest);

            }

            if (RemoteParty is null)
            {

                if (accessTokenErrorMessageRAW is not null)
                    accessTokenErrorMessages.Add(accessTokenErrorMessageRAW);

                if (accessTokenErrorMessageBASE64 is not null)
                    accessTokenErrorMessages.Add(accessTokenErrorMessageBASE64);

                if (accessTokenErrorMessages.Count > 1 &&
                    accessTokenErrorMessages.Contains("Unknown access token!"))
                {
                    accessTokenErrorMessages.Remove("Unknown access token!");
                }

                if (CommonAPI.BaseAPI.LocationsAsOpenData && accessTokenErrorMessages.Contains("Unknown access token!"))
                    accessTokenErrorMessages.Remove("Unknown access token!");

            }

            HTTPRequest.SubprotocolRequest = this;

        }


        public static OCPIRequest Parse(HTTPRequest  HTTPRequest,
                                        CommonAPI    CommonAPI)

            => new (HTTPRequest,
                    CommonAPI);



        #region TryParseJObjectRequestBody (out JSON, out OCPIResponseBuilder, ...)

        public Boolean TryParseJObjectRequestBody([NotNullWhen(true)]  out JObject?              JSON,
                                                  [NotNullWhen(false)] out OCPIResponse.Builder  OCPIResponseBuilder,
                                                  Boolean                                        AllowEmptyHTTPBody   = false,
                                                  String?                                        JSONLDContext        = null)
        {

            var result = HTTPRequest.TryParseJSONObjectRequestBody(
                             out JSON,
                             out var httpResponseBuilder,
                             AllowEmptyHTTPBody,
                             JSONLDContext
                         );

            if (httpResponseBuilder is not null)
            {

                if (RequestId.    HasValue)
                    httpResponseBuilder.Set("X-Request-ID",      RequestId);

                if (CorrelationId.HasValue)
                    httpResponseBuilder.Set("X-Correlation-ID",  CorrelationId);

            }

            OCPIResponseBuilder = new OCPIResponse.Builder(this) {
                                      StatusCode           = result ? 1000 : 2001,
                                      StatusMessage        = result ? ""   : "Could not parse JSON object in OCPI HTTP request body!",
                                      HTTPResponseBuilder  = httpResponseBuilder
                                  };

            return result;

        }

        #endregion

        #region TryParseJArrayRequestBody  (out JSON, out OCPIResponseBuilder, ...)

        public Boolean TryParseJArrayRequestBody([NotNullWhen(true)]  out JArray?               JSON,
                                                 [NotNullWhen(false)] out OCPIResponse.Builder  OCPIResponseBuilder,
                                                 Boolean                                        AllowEmptyHTTPBody   = false,
                                                 String?                                        JSONLDContext        = null)
        {

            var result = HTTPRequest.TryParseJSONArrayRequestBody(
                             out JSON,
                             out var httpResponseBuilder,
                             AllowEmptyHTTPBody,
                             JSONLDContext
                         );

            if (httpResponseBuilder is not null)
            {

                if (RequestId.    HasValue)
                    httpResponseBuilder.Set("X-Request-ID",      RequestId);

                if (CorrelationId.HasValue)
                    httpResponseBuilder.Set("X-Correlation-ID",  CorrelationId);

            }

            OCPIResponseBuilder = new OCPIResponse.Builder(this) {
                                      StatusCode           = result ? 1000 : 2001,
                                      StatusMessage        = result ? ""   : "Could not parse JSON array in OCPI HTTP request body!",
                                      HTTPResponseBuilder  = httpResponseBuilder
                                  };

            return result;

        }

        #endregion


    }

}
