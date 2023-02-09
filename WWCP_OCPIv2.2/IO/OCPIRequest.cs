/*
 * Copyright (c) 2015-2023 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2.HTTP
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

        #region AddOCPIMethod(CommonAPI, Hostname, HTTPMethod, URLTemplate,  HTTPContentType = null, URLAuthentication = false, HTTPMethodAuthentication = false, ContentTypeAuthentication = false, HTTPDelegate = null)

        /// <summary>
        /// Add a method callback for the given URL template.
        /// </summary>
        /// <param name="CommonAPI">The OCPI Common API.</param>
        /// <param name="Hostname">The HTTP hostname.</param>
        /// <param name="HTTPMethod">The HTTP method.</param>
        /// <param name="URLTemplate">The URL template.</param>
        /// <param name="HTTPContentType">The HTTP content type.</param>
        /// <param name="URLAuthentication">Whether this method needs explicit uri authentication or not.</param>
        /// <param name="HTTPMethodAuthentication">Whether this method needs explicit HTTP method authentication or not.</param>
        /// <param name="ContentTypeAuthentication">Whether this method needs explicit HTTP content type authentication or not.</param>
        /// <param name="OCPIRequestLogger">A OCPI request logger.</param>
        /// <param name="OCPIResponseLogger">A OCPI response logger.</param>
        /// <param name="DefaultErrorHandler">The default error handler.</param>
        /// <param name="OCPIRequestHandler">The method to call.</param>
        public static void AddOCPIMethod(this CommonAPI           CommonAPI,
                                         HTTPHostname             Hostname,
                                         HTTPMethod               HTTPMethod,
                                         HTTPPath                 URLTemplate,
                                         HTTPContentType?         HTTPContentType             = null,
                                         HTTPAuthentication?      URLAuthentication           = null,
                                         HTTPAuthentication?      HTTPMethodAuthentication    = null,
                                         HTTPAuthentication?      ContentTypeAuthentication   = null,
                                         OCPIRequestLogHandler?   OCPIRequestLogger           = null,
                                         OCPIResponseLogHandler?  OCPIResponseLogger          = null,
                                         HTTPDelegate?            DefaultErrorHandler         = null,
                                         OCPIRequestDelegate?     OCPIRequestHandler          = null,
                                         URLReplacement           AllowReplacement            = URLReplacement.Fail)

        {

            CommonAPI.HTTPServer.
                      AddMethodCallback(CommonAPI,
                                        Hostname,
                                        HTTPMethod,
                                        URLTemplate,
                                        HTTPContentType,
                                        URLAuthentication,
                                        HTTPMethodAuthentication,
                                        ContentTypeAuthentication,
                                        (timestamp, httpAPI, httpRequest)               => OCPIRequestLogger?. Invoke(timestamp, null, OCPIRequest.Parse(httpRequest, CommonAPI)),
                                        (timestamp, httpAPI, httpRequest, httpResponse) => OCPIResponseLogger?.Invoke(timestamp, null, httpRequest. SubprotocolRequest  as OCPIRequest,
                                                                                                                                      (httpResponse.SubprotocolResponse as OCPIResponse) 
                                                                                                                                           ?? new OCPIResponse(httpRequest.SubprotocolRequest as OCPIRequest,
                                                                                                                                                               2000,
                                                                                                                                                               "OCPIResponse is null!",
                                                                                                                                                               httpResponse.HTTPBodyAsUTF8String,
                                                                                                                                                               httpResponse.Timestamp,
                                                                                                                                                               httpResponse)),
                                        DefaultErrorHandler,
                                        async httpRequest => {

                                            try
                                            {

                                                // When no OCPIRequestLogger was used!
                                                httpRequest.SubprotocolRequest ??= OCPIRequest.Parse(httpRequest, CommonAPI);

                                                var OCPIResponseBuilder = await OCPIRequestHandler(httpRequest.SubprotocolRequest as OCPIRequest);
                                                var httpResponseBuilder = OCPIResponseBuilder.ToHTTPResponseBuilder();

                                                httpResponseBuilder.SubprotocolResponse = new OCPIResponse(OCPIResponseBuilder.Request,
                                                                                                           OCPIResponseBuilder.StatusCode ?? 3000,
                                                                                                           OCPIResponseBuilder.StatusMessage,
                                                                                                           OCPIResponseBuilder.AdditionalInformation,
                                                                                                           OCPIResponseBuilder.Timestamp  ?? Timestamp.Now,
                                                                                                           httpResponseBuilder.AsImmutable);

                                                return httpResponseBuilder;

                                            }
                                            catch (Exception e)
                                            {

                                                return new HTTPResponse.Builder(HTTPStatusCode.InternalServerError) {
                                                           ContentType  = HTTPContentType.JSON_UTF8,
                                                           Content      = new OCPIResponse<JObject>(
                                                                              JSONObject.Create(
                                                                                  new JProperty("description", e.Message),
                                                                                  new JProperty("stacktrace",  e.StackTrace.Split(new String[] { Environment.NewLine }, StringSplitOptions.None).ToArray()),
                                                                                  new JProperty("source",      e.TargetSite.Module.Name),
                                                                                  new JProperty("type",        e.TargetSite.ReflectedType.Name)
                                                                              ),
                                                                              2000,
                                                                              e.Message,
                                                                              null,
                                                                              Timestamp.Now,
                                                                              null,
                                                                              (httpRequest.SubprotocolRequest as OCPIRequest)?.RequestId,
                                                                              (httpRequest.SubprotocolRequest as OCPIRequest)?.CorrelationId
                                                                          ).ToJSON(json => json).ToUTF8Bytes(),
                                                           Connection   = "close"
                                                       };

                                            }

                                        },
                                        AllowReplacement);

        }

        #endregion

    }


    /// <summary>
    /// An OCPI HTTP request.
    /// </summary>
    public class OCPIRequest // : HTTPRequest
    {

        public readonly struct DateAndPaginationFilters
        {

            public DateTime?  From     { get; }
            public DateTime?  To       { get; }
            public UInt64?    Offset   { get; }
            public UInt64?    Limit    { get; }


            public DateAndPaginationFilters(DateTime?  From,
                                            DateTime?  To,
                                            UInt64?    Offset,
                                            UInt64?    Limit)
            {

                this.From    = From;
                this.To      = To;
                this.Offset  = Offset;
                this.Limit   = Limit;

            }

        }


        #region Properties

        public CommonAPI        CommonAPI           { get; }

        public HTTPRequest      HTTPRequest         { get; }

        public Request_Id?      RequestId           { get; }
        public Correlation_Id?  CorrelationId       { get; }
        public CountryCode?     ToCountryCode       { get; }
        public Party_Id?        ToPartyId           { get; }
        public CountryCode?     FromCountryCode     { get; }
        public Party_Id?        FromPartyId         { get; }

        public AccessToken?     AccessToken         { get; }

        public AccessInfo?      AccessInfo          { get; }

        public AccessInfoStatus?     AccessInfo2         { get; }

        public RemoteParty      RemoteParty         { get; }


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

            this.HTTPRequest      = Request ?? throw new ArgumentNullException(nameof(HTTPRequest), "The given HTTP request must not be null!");
            this.CommonAPI        = CommonAPI;

            this.RequestId        = Request.TryParseHeaderField<Request_Id>    ("X-Request-ID",           Request_Id.    TryParse) ?? Request_Id.    NewRandom(IsLocal: true);
            this.CorrelationId    = Request.TryParseHeaderField<Correlation_Id>("X-Correlation-ID",       Correlation_Id.TryParse) ?? Correlation_Id.NewRandom(IsLocal: true);
            this.ToCountryCode    = Request.TryParseHeaderField<CountryCode>   ("OCPI-to-country-code",   CountryCode.   TryParse);
            this.ToPartyId        = Request.TryParseHeaderField<Party_Id>      ("OCPI-to-party-id",       Party_Id.      TryParse);
            this.FromCountryCode  = Request.TryParseHeaderField<CountryCode>   ("OCPI-from-country-code", CountryCode.   TryParse);
            this.FromPartyId      = Request.TryParseHeaderField<Party_Id>      ("OCPI-from-party-id",     Party_Id.      TryParse);


            if (Request.Authorization is HTTPTokenAuthentication TokenAuth &&
                TokenAuth.Token.TryBase64Decode_UTF8(out String DecodedToken)   &&
                OCPIv2_2.AccessToken.TryParse(DecodedToken, out var accessToken))
            {
                this.AccessToken = accessToken;
            }

            else if (Request.Authorization is HTTPBasicAuthentication BasicAuth &&
                OCPIv2_2.AccessToken.TryParse(BasicAuth.Username, out accessToken))
            {
                this.AccessToken = accessToken;
            }

            if (this.AccessToken.HasValue)
            {

                if (CommonAPI.TryGetRemoteParties(AccessToken.Value, out var parties))
                {

                    if (parties.Count() == 1)
                    {

                        this.AccessInfo   = new AccessInfo(
                                                AccessToken.Value,
                                                parties.First().AccessInfoStatus.First(accessInfo2 => accessInfo2.Token == AccessToken).Status,
                                                null,
                                                new CredentialsRole[] {
                                                    new CredentialsRole(
                                                        parties.First().CountryCode,
                                                        parties.First().PartyId,
                                                        parties.First().Role,
                                                        parties.First().BusinessDetails,
                                                        null
                                                    )
                                                }
                                            );

                        this.AccessInfo2  = parties.First().AccessInfoStatus.First(accessInfo2 => accessInfo2.Token == AccessToken);

                        this.RemoteParty  = parties.First();

                    }

                    else if (parties.Count() > 1      &&
                             FromCountryCode.HasValue &&
                             FromPartyId.    HasValue)
                    {

                        var filteredParties = parties.Where(party => party.CountryCode == FromCountryCode.Value &&
                                                                     party.PartyId     == FromPartyId.    Value).ToArray();

                        if (filteredParties.Count() == 1)
                        {

                            this.AccessInfo   = new AccessInfo(AccessToken.Value,
                                                               filteredParties.First().AccessInfoStatus.First(accessInfo2 => accessInfo2.Token == AccessToken).Status);

                            this.AccessInfo2  = filteredParties.First().AccessInfoStatus.First(accessInfo2 => accessInfo2.Token == AccessToken);

                            this.RemoteParty  = filteredParties.First();

                        }

                    }

                }


            //    if (CommonAPI.TryGetAccessInfo(this.AccessToken.Value, out AccessInfo accessInfo))
            //    {

            //        this.AccessInfo = accessInfo;

            ////        var allTheirCPORoles = this.AccessInfo.Value.Roles.Where(role => role.Role == Roles.CPO).ToArray();

            ////        if (!FromCountryCode.HasValue && allTheirCPORoles.Length == 1)
            ////            this.FromCountryCode = allTheirCPORoles[0].CountryCode;

            ////        if (!FromPartyId.    HasValue && allTheirCPORoles.Length == 1)
            ////            this.FromPartyId     = allTheirCPORoles[0].PartyId;

            //    }

            }


            //var allMyCPORoles = this.AccessInfo.Value.Roles.Where(role => role.Role == Roles.CPO).ToArray();

            //if (!ToCountryCode.HasValue && allMyCPORoles.Length == 1)
            //    this.ToCountryCode = allMyCPORoles[1].CountryCode;

            //if (!ToPartyId.HasValue && allMyCPORoles.Length == 1)
            //    this.ToPartyId = allMyCPORoles[1].PartyId;


            this.HTTPRequest.SubprotocolRequest = this;

        }

        public DateAndPaginationFilters GetDateAndPaginationFilters()

            => new (HTTPRequest.QueryString.GetDateTime("date_from"),
                    HTTPRequest.QueryString.GetDateTime("date_to"),
                    HTTPRequest.QueryString.GetUInt64  ("offset"),
                    HTTPRequest.QueryString.GetUInt64  ("limit"));




        public Boolean TryParseJObjectRequestBody(out JObject               JSON,
                                                  out OCPIResponse.Builder  OCPIResponseBuilder,
                                                  Boolean                   AllowEmptyHTTPBody   = false,
                                                  String?                   JSONLDContext        = null)
        {

            var result = HTTPRequest.TryParseJObjectRequestBody(out JSON,
                                                                out var HTTPResponseBuilder,
                                                                AllowEmptyHTTPBody,
                                                                JSONLDContext);

            if (HTTPResponseBuilder != null)
            {
                HTTPResponseBuilder.Set("X-Request-ID",      RequestId).
                                    Set("X-Correlation-ID",  CorrelationId);
            }

            OCPIResponseBuilder = new OCPIResponse.Builder(this) {
                StatusCode           = result ? 1000 : 2001,
                StatusMessage        = result ? ""   : "Could not parse JSON object in HTTP request body!",
                HTTPResponseBuilder  = HTTPResponseBuilder
            };

            return result;

        }

        public Boolean TryParseJArrayRequestBody(out JArray                JSON,
                                                 out OCPIResponse.Builder  OCPIResponseBuilder,
                                                 Boolean                   AllowEmptyHTTPBody   = false,
                                                 String?                   JSONLDContext        = null)
        {

            var result = HTTPRequest.TryParseJArrayRequestBody(out JSON,
                                                               out var HTTPResponseBuilder,
                                                               AllowEmptyHTTPBody,
                                                               JSONLDContext);

            if (HTTPResponseBuilder != null)
            {
                HTTPResponseBuilder.Set("X-Request-ID",      RequestId).
                                    Set("X-Correlation-ID",  CorrelationId);
            }

            OCPIResponseBuilder = new OCPIResponse.Builder(this) {
                StatusCode           = result ? 1000 : 2001,
                StatusMessage        = result ? ""   : "Could not parse JSON array in HTTP request body!",
                HTTPResponseBuilder  = HTTPResponseBuilder
            };

            return result;

        }


        public static OCPIRequest Parse(HTTPRequest  HTTPRequest,
                                        CommonAPI    CommonAPI)

            => new (HTTPRequest,
                    CommonAPI);


    }

}
