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

namespace cloud.charging.open.protocols.OCPIv2_1_1
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
                                         URLReplacement       AllowReplacement   = URLReplacement.Fail,
                                         Boolean              OpenData           = false)

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
                   AllowReplacement,
                   OpenData

               );


        public static void AddOCPIMethod(this CommonAPI          CommonAPI,
                                         HTTPMethod              HTTPMethod,
                                         HTTPPath                URLTemplate,
                                         OCPIRequestLogHandler   OCPIRequestLogger,
                                         OCPIResponseLogHandler  OCPIResponseLogger,
                                         OCPIRequestDelegate     OCPIRequestHandler,

                                         HTTPDelegate?           DefaultErrorHandler   = null,
                                         URLReplacement          AllowReplacement      = URLReplacement.Fail,
                                         Boolean                 OpenData              = false)

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
                   AllowReplacement,
                   OpenData

               );

        #region AddOCPIMethod(CommonAPI, HTTPMethod, URLTemplate, HTTPContentType, OCPIRequestHandler, HTTPContentType, ...)

        /// <summary>
        /// Add a method callback for the given URL template.
        /// </summary>
        /// <param name="CommonAPI">The OCPI Common API.</param>
        /// <param name="HTTPMethod">The HTTP method.</param>
        /// <param name="URLTemplate">The URL template.</param>
        /// <param name="HTTPContentType">The HTTP content type.</param>
        /// 
        /// <param name="OCPIRequestLogger">A OCPI request logger.</param>
        /// <param name="OCPIResponseLogger">A OCPI response logger.</param>
        /// 
        /// <param name="DefaultErrorHandler">The default error handler.</param>
        /// <param name="OCPIRequestHandler">The method to call.</param>
        /// <param name="AllowReplacement">Whether a handler of the same URL template may be replaced.</param>
        /// <param name="OpenData">Whether the route serves open data - locations, tariffs - and so takes a token of nobody here as no token, which it answers itself; every other route refuses one with 401 before it is asked.</param>
        public static void AddOCPIMethod(this CommonAPI           CommonAPI,
                                         HTTPMethod               HTTPMethod,
                                         HTTPPath                 URLTemplate,
                                         OCPIRequestDelegate      OCPIRequestHandler,
                                         HTTPContentType?         HTTPContentType,

                                         OCPIRequestLogHandler?   OCPIRequestLogger     = null,
                                         OCPIResponseLogHandler?  OCPIResponseLogger    = null,

                                         HTTPDelegate?            DefaultErrorHandler   = null,
                                         URLReplacement           AllowReplacement      = URLReplacement.Fail,
                                         Boolean                  OpenData              = false)

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

                            // A token of nobody here, or a TOTP that is not its party's:
                            // refused before the route, with why - not passed on as a
                            // request of nobody, which every route would have to refuse
                            // itself. Only a route of open data takes an unknown token,
                            // as no token at all.
                            if (ocpiRequest.IsRefusedBeforeTheRoute(OpenData))
                                return ocpiRequest.TokenRefused();

                            var ocpiResponseBuilder  = await OCPIRequestHandler(ocpiRequest);

                            var httpResponseBuilder  = ocpiResponseBuilder.ToHTTPResponseBuilder();

                            httpResponseBuilder.SubprotocolResponse = new OCPIResponse(
                                                                          ocpiResponseBuilder.Request,
                                                                          ocpiResponseBuilder.StatusCode    ?? StatusCode.ServerErrors.GenericServerError,
                                                                          ocpiResponseBuilder.StatusMessage ?? "error!",
                                                                          ocpiResponseBuilder.AdditionalInformation,
                                                                          ocpiResponseBuilder.Timestamp     ?? Timestamp.Now,
                                                                          httpResponseBuilder.AsImmutable
                                                                      );

                            return httpResponseBuilder;

                        }

                        var ocpiResponseBuilderX  = new OCPIResponse.Builder(httpRequest.SubprotocolRequest as OCPIRequest) {
                                                        StatusCode    = StatusCode.ClientErrors.InvalidOrMissingParameters,
                                                        StatusMessage = "Invalid OCPI request!"
                                                    };
                        var httpResponseBuilderX  = new HTTPResponse.Builder(httpRequest);

                        httpResponseBuilderX.SubprotocolResponse = new OCPIResponse(
                                                                       ocpiResponseBuilderX.Request,
                                                                       ocpiResponseBuilderX.StatusCode    ?? StatusCode.ServerErrors.GenericServerError,
                                                                       ocpiResponseBuilderX.StatusMessage ?? "error!",
                                                                       ocpiResponseBuilderX.AdditionalInformation,
                                                                       ocpiResponseBuilderX.Timestamp     ?? Timestamp.Now,
                                                                       httpResponseBuilderX.AsImmutable
                                                                   );

                        return httpResponseBuilderX;

                    }
                    catch (Exception e)
                    {

                        // What was thrown goes to whoever made this API, not
                        // to whoever asked: see CommonHTTPAPI.AnswerFailedRequest.
                        // Null where the request could not even be read.
                        var ocpiRequest   = httpRequest.SubprotocolRequest as OCPIRequest;

                        var httpResponse  = await CommonAPI.BaseAPI.AnswerFailedRequest(
                                                      httpRequest,
                                                      ocpiRequest?.RequestId,
                                                      ocpiRequest?.CorrelationId,
                                                      ocpiRequest?.RemoteParty?.Id,
                                                      e
                                                  );

                        // For the response logger, which hears of read requests only
                        if (ocpiRequest is not null)
                            httpResponse.SubprotocolResponse = new OCPIResponse(
                                                                   ocpiRequest,
                                                                   StatusCode.ServerErrors.GenericServerError,
                                                                   CommonHTTPAPI.FailedRequestMessage,
                                                                   null,
                                                                   httpResponse.Date,
                                                                   httpResponse.AsImmutable,
                                                                   ocpiRequest.RequestId,
                                                                   ocpiRequest.CorrelationId
                                                               );

                        return httpResponse;

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
                               httpAPI as HTTPExtAPI,
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
                                   httpAPI as HTTPExtAPI,
                                   ocpiRequest,
                                   (httpResponse.SubprotocolResponse as OCPIResponse)
                                       ?? new OCPIResponse(
                                              ocpiRequest,
                                              StatusCode.ClientErrors.GenericClientError,
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
    public class OCPIRequest
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

        public CommonAPI          CommonAPI           { get; }

        public HTTPRequest        HTTPRequest         { get; }

        public Request_Id?        RequestId           { get; }

        public Correlation_Id?    CorrelationId       { get; }

        public AccessToken?       AccessToken         { get; }

        public LocalAccessInfo2?  LocalAccessInfo     { get; }

        public RemoteParty?       RemoteParty         { get; }

        public EMSP_Id?           EMSPId              { get; }

        public CPO_Id?            CPOId               { get; }

        /// <summary>
        /// Why the token of this request makes it nobody's request: a token of
        /// nobody here, a TOTP that is not its party's. Empty where it is
        /// somebody's, or where there is no token.
        /// </summary>
        public IEnumerable<String> AccessTokenErrorMessages    { get; }

        /// <summary>
        /// What a token of nobody here is told.
        /// </summary>
        public const String UnknownAccessToken = "Unknown access token!";


        /// <summary>
        /// The HTTP query string.
        /// </summary>
        public HTTPHostname     Host
            => HTTPRequest.Host;

        /// <summary>
        /// The parsed URL parameters of the best matching URL template.
        /// Set by the HTTP server.
        /// </summary>
        public String[]         ParsedURLParameters
            => HTTPRequest.ParsedURLParameters;

        /// <summary>
        /// The HTTP query string.
        /// </summary>
        public QueryString      QueryString
            => HTTPRequest.QueryString;

        #endregion

        protected OCPIRequest(HTTPRequest  Request,
                              CommonAPI    CommonAPI)
        {

            this.HTTPRequest      = Request ?? throw new ArgumentNullException(nameof(Request), "The given HTTP request must not be null!");
            this.CommonAPI        = CommonAPI;

            this.RequestId        = Request.TryParseHeaderStruct               (HTTPHeaders.X_Request_ID,     Request_Id.    TryParse, Request_Id.    NewRandom(IsLocal: true));
            this.CorrelationId    = Request.TryParseHeaderStruct               (HTTPHeaders.X_Correlation_ID, Correlation_Id.TryParse, Correlation_Id.NewRandom(IsLocal: true));
            var  totp             = Request.TryParseHeaderField<TOTPHTTPHeader>("TOTP",                       TOTPHTTPHeader.TryParse);

            // The token is read as it was sent, and BASE64-decoded where it
            // reads as BASE64, and it is the token of a party in either reading -
            // as the versions list of the base API reads it. Read as it was sent
            // only, a token a party sends BASE64-encoded, as our own clients send
            // it unless told otherwise, was let in at the versions list and
            // refused at every route of 2.1.1; read only as the access of its
            // party says it is sent, a token a party sends as it is, as 2.1.1
            // has it, would be refused wherever that access says nothing.
            AccessToken?  accessTokenRAW     = null;
            AccessToken?  accessTokenBASE64  = null;

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

                // The password is a TOTP only where it reads as one. Anything
                // else, an empty one among them, is none - and not an exception
                // thrown at whoever sent it, before its token was looked at.
                if (TOTPHTTPHeader.TryParse(basicAuth.Password, out var totpOfThePassword, out _))
                    totp               = totpOfThePassword;

            }

            var accessTokenErrorMessages   = new List<String>();
            this.AccessTokenErrorMessages  = accessTokenErrorMessages;

            // As it was sent first, as 2.1.1 has it, and BASE64-decoded where
            // that reading is of nobody here. Where neither is, both say why.
            IEnumerable<Tuple<RemoteParty, LocalAccessInfo>>? partiesAccessInfos = null;

            foreach (var reading in new[] { accessTokenRAW, accessTokenBASE64 })
            {

                if (!reading.HasValue || partiesAccessInfos is not null)
                    continue;

                if (CommonAPI.TryGetRemoteParties(reading.Value,
                                                  totp,
                                                  null,
                                                  out var partiesOfThisReading,
                                                  out var accessTokenErrorMessage))
                {
                    this.AccessToken    = reading;
                    partiesAccessInfos  = partiesOfThisReading;
                }

                else if (!accessTokenErrorMessages.Contains(accessTokenErrorMessage ?? UnknownAccessToken))
                    accessTokenErrorMessages.Add(accessTokenErrorMessage ?? UnknownAccessToken);

            }

            if (partiesAccessInfos is not null)
                accessTokenErrorMessages.Clear();

            // A token of nobody here and a TOTP not its party's: the TOTP says
            // more, as 2.2.1 has it.
            else if (accessTokenErrorMessages.Count > 1)
                accessTokenErrorMessages.Remove(UnknownAccessToken);

            // Where the token reads as neither, it is taken as it was sent, as
            // before, though nobody has it.
            if (!this.AccessToken.HasValue)
                this.AccessToken = accessTokenRAW;

            if (partiesAccessInfos is not null)
            {

                if (partiesAccessInfos.Count() == 1)
                {

                    RemoteParty      = partiesAccessInfos.First().Item1;

                    var localAccess  = partiesAccessInfos.First().Item2;

                    LocalAccessInfo  = new LocalAccessInfo2(
                                           AccessToken!.Value,
                                           localAccess.Status,
                                           RemoteParty.CountryCode,
                                           RemoteParty.PartyId,
                                           RemoteParty.Role,
                                           RemoteParty.BusinessDetails,
                                           localAccess.NotBefore,
                                           localAccess.NotAfter,
                                           RemoteParty.RemoteAccessInfos.FirstOrDefault()?.VersionsURL
                                       );

                    CPOId            = RemoteParty.Role == Role.CPO
                                          ? CPO_Id. Parse($"{LocalAccessInfo.CountryCode}*{LocalAccessInfo.PartyId}")
                                          : null;

                    EMSPId           = RemoteParty.Role == Role.EMSP
                                          ? EMSP_Id.Parse($"{LocalAccessInfo.CountryCode}-{LocalAccessInfo.PartyId}")
                                          : null;

                }

                //else if (parties.Count() > 1      &&
                //         FromCountryCode.HasValue &&
                //         FromPartyId.    HasValue)
                //{

                //    var filteredParties = parties.Where(party => party.CountryCode == FromCountryCode.Value &&
                //                                                 party.PartyId     == FromPartyId.    Value).ToArray();

                //    if (filteredParties.Count() == 1)
                //    {

                //        this.AccessInfo   = new AccessInfo(AccessToken.Value,
                //                                           filteredParties.First().AccessInfo.First(accessInfo2 => accessInfo2.Token == AccessToken).Status,
                //                                           null,
                //                                           parties.First().BusinessDetails,
                //                                           parties.First().CountryCode,
                //                                           parties.First().PartyId);

                //        this.AccessInfo2  = filteredParties.First().AccessInfo.First(accessInfo2 => accessInfo2.Token == AccessToken);

                //        this.RemoteParty  = filteredParties.First();

                //    }

                //}


            //    if (CommonAPI.TryGetAccessInfo(this.AccessToken.Value, out AccessInfo accessInfo))
            //    {

            //        this.AccessInfo = accessInfo;

            ////        var allTheirCPORoles = this.AccessInfo.Value.Roles.Where(role => role.Role == Role.CPO).ToArray();

            ////        if (!FromCountryCode.HasValue && allTheirCPORoles.Length == 1)
            ////            this.FromCountryCode = allTheirCPORoles[0].CountryCode;

            ////        if (!FromPartyId.    HasValue && allTheirCPORoles.Length == 1)
            ////            this.FromPartyId     = allTheirCPORoles[0].PartyId;

            //    }

            }


            //var allMyCPORoles = this.AccessInfo.Value.Roles.Where(role => role.Role == Role.CPO).ToArray();

            //if (!ToCountryCode.HasValue && allMyCPORoles.Length == 1)
            //    this.ToCountryCode = allMyCPORoles[1].CountryCode;

            //if (!ToPartyId.HasValue && allMyCPORoles.Length == 1)
            //    this.ToPartyId = allMyCPORoles[1].PartyId;


            this.HTTPRequest.SubprotocolRequest = this;

            var requestModifier = RemoteParty?.LocalAccessInfos.FirstOrDefault()?.IN?.RequestModifier;
            if (requestModifier is not null)
            {
                try
                {
                    HTTPRequest = requestModifier(HTTPRequest);
                }
                catch (Exception e)
                {
                    //await CommonAPI.LogException(e);
                }
            }

        }


        public static OCPIRequest Parse(HTTPRequest  HTTPRequest,
                                        CommonAPI    CommonAPI)

            => new (HTTPRequest,
                    CommonAPI);


        #region IsRefusedBeforeTheRoute(OpenData) / TokenRefused()

        /// <summary>
        /// Whether this request is answered 401 before its route is asked: its
        /// token is of nobody here, or its TOTP is not its party's. A route of
        /// open data takes an unknown token as it takes no token, and a wrong
        /// TOTP never.
        /// </summary>
        /// <param name="OpenData">Whether the route serves open data.</param>
        public Boolean IsRefusedBeforeTheRoute(Boolean OpenData)

            => AccessTokenErrorMessages.Any(message => !OpenData || message != UnknownAccessToken);

        /// <summary>
        /// The 401 of a request refused before its route, with why.
        /// </summary>
        public HTTPResponse.Builder TokenRefused()

            => new (HTTPRequest) {
                   HTTPStatusCode             = HTTPStatusCode.Unauthorized,
                   Server                     = CommonAPI.HTTPServiceName,
                   Date                       = Timestamp.Now,
                   AccessControlAllowOrigin   = "*",
                   AccessControlAllowHeaders  = [ "Authorization" ],
                   ContentType                = HTTPContentType.Application.JSON_UTF8,
                   Content                    = new OCPIResponse(
                                                   this,
                                                   StatusCode.ClientErrors.GenericClientError,
                                                   AccessTokenErrorMessages.First(),
                                                   AccessTokenErrorMessages.Count() > 1
                                                       ? AccessTokenErrorMessages.Skip(1).AggregateWith(", ")
                                                       : null,
                                                   Timestamp.Now
                                               ).ToJSON().ToUTF8Bytes(),
                   Connection                 = ConnectionType.KeepAlive,
                   Vary                       = "Accept"
               };

        #endregion



        #region TryParseJObjectRequestBody (out JSON, out OCPIResponseBuilder, ...)

        public Boolean TryParseJObjectRequestBody([NotNullWhen(true)]  out JObject?               JSON,
                                                  [NotNullWhen(false)] out OCPIResponse.Builder?  OCPIResponseBuilder,
                                                  Boolean                                         AllowEmptyHTTPBody   = false,
                                                  String?                                         JSONLDContext        = null)
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
                    httpResponseBuilder.Set(HTTPHeaders.X_Request_ID,      RequestId.    Value);

                if (CorrelationId.HasValue)
                    httpResponseBuilder.Set(HTTPHeaders.X_Correlation_ID,  CorrelationId.Value);

            }

            OCPIResponseBuilder = new OCPIResponse.Builder(this) {
                                      StatusCode           = result ? StatusCode.Success : StatusCode.ClientErrors.InvalidOrMissingParameters,
                                      StatusMessage        = result ? ""                 : "Could not parse JSON object in HTTP request body!",
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
                    httpResponseBuilder.Set(HTTPHeaders.X_Request_ID,      RequestId.    Value);

                if (CorrelationId.HasValue)
                    httpResponseBuilder.Set(HTTPHeaders.X_Correlation_ID,  CorrelationId.Value);

            }

            OCPIResponseBuilder = new OCPIResponse.Builder(this) {
                                      StatusCode           = result ? StatusCode.Success : StatusCode.ClientErrors.InvalidOrMissingParameters,
                                      StatusMessage        = result ? ""                 : "Could not parse JSON array in HTTP request body!",
                                      HTTPResponseBuilder  = httpResponseBuilder
                                  };

            return result;

        }

        #endregion


    }

}
