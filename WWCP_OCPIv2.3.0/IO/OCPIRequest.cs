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

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_3_0.HTTP
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
        /// <param name="CommonAPI">The OCPI CommonAPI.</param>
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
                                        false,
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

                                                return new HTTPResponse.Builder() {
                                                           HTTPStatusCode  = HTTPStatusCode.InternalServerError,
                                                           ContentType     = HTTPContentType.Application.JSON_UTF8,
                                                           Content         = new OCPIResponse<JObject>(
                                                                                 JSONObject.Create(
                                                                                     new JProperty("description",  e.Message),
                                                                                     new JProperty("stacktrace",   new JArray(e.StackTrace?.Split(new[] { Environment.NewLine }, StringSplitOptions.None).ToArray() ?? Array.Empty<String>())),
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
                                        AllowReplacement);

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
        public readonly struct DateAndPaginationFilters(DateTime?  From,
                                                        DateTime?  To,
                                                        UInt64?    Offset,
                                                        UInt64?    Limit)
        {

            #region Properties

            /// <summary>
            /// The optional 'from' timestamp (inclusive).
            /// </summary>
            public DateTime?  From      { get; } = From;

            /// <summary>
            /// The optional 'to' timestamp (exclusive).
            /// </summary>
            public DateTime?  To        { get; } = To;

            /// <summary>
            /// The optional 'offset' within the result set.
            /// </summary>
            public UInt64?    Offset    { get; } = Offset;

            /// <summary>
            /// The optional 'limit' of the result set.
            /// </summary>
            public UInt64?    Limit     { get; } = Limit;

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

        public CommonAPI             CommonAPI           { get; }

        public HTTPRequest           HTTPRequest         { get; }

        public Request_Id?           RequestId           { get; }
        public Correlation_Id?       CorrelationId       { get; }

        public CountryCode?          ToCountryCode       { get; }
        public Party_Id?             ToPartyId           { get; }
        public CountryCode?          FromCountryCode     { get; }
        public Party_Id?             FromPartyId         { get; }

        public AccessToken?          AccessToken         { get; }

        public LocalAccessInfo2?     LocalAccessInfo     { get; }

        public RemoteParty?          RemoteParty         { get; }

        public IEnumerable<EMSP_Id>  EMSPIds             { get; } = [];

        public IEnumerable<CPO_Id>   CPOIds              { get; } = [];

        public EMSP_Id?              EMSPId              { get; }

        public CPO_Id?               CPOId               { get; }


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

            this.RequestId        = Request.TryParseHeaderField<Request_Id>    ("X-Request-ID",           Request_Id.    TryParse) ?? Request_Id.    NewRandom(IsLocal: true);
            this.CorrelationId    = Request.TryParseHeaderField<Correlation_Id>("X-Correlation-ID",       Correlation_Id.TryParse) ?? Correlation_Id.NewRandom(IsLocal: true);
            this.ToCountryCode    = Request.TryParseHeaderField<CountryCode>   ("OCPI-to-country-code",   CountryCode.   TryParse);
            this.ToPartyId        = Request.TryParseHeaderField<Party_Id>      ("OCPI-to-party-id",       Party_Id.      TryParse);
            this.FromCountryCode  = Request.TryParseHeaderField<CountryCode>   ("OCPI-from-country-code", CountryCode.   TryParse);
            this.FromPartyId      = Request.TryParseHeaderField<Party_Id>      ("OCPI-from-party-id",     Party_Id.      TryParse);


            if (Request.Authorization is HTTPTokenAuthentication TokenAuth &&
                TokenAuth.Token.TryParseBASE64_UTF8(out var decodedToken, out var errorResponse) &&
                OCPI.AccessToken.TryParse(decodedToken, out var accessToken))
            {
                this.AccessToken = accessToken;
            }

            else if (Request.Authorization is HTTPBasicAuthentication BasicAuth &&
                OCPI.AccessToken.TryParse(BasicAuth.Username, out accessToken))
            {
                this.AccessToken = accessToken;
            }

            if (this.AccessToken.HasValue)
            {

                if (CommonAPI.TryGetRemoteParties(AccessToken.Value, out var parties))
                {

                    if (parties.Count() == 1)
                    {

                        RemoteParty      = parties.First();

                        LocalAccessInfo  = new LocalAccessInfo2(
                                               AccessToken.Value,
                                               RemoteParty.LocalAccessInfos.First(localAccessInfo => localAccessInfo.AccessToken == AccessToken).Status,
                                               RemoteParty.Roles,
                                               null,
                                               null,
                                               RemoteParty.RemoteAccessInfos.FirstOrDefault()?.VersionsURL
                                           );

                        CPOIds           = RemoteParty.Roles.Where (credentialsRole => credentialsRole.Role == Role.CPO).
                                                             Select(credentialsRole => CPO_Id. Parse($"{LocalAccessInfo.Roles.First().CountryCode}*{LocalAccessInfo.Roles.First().PartyId}")).
                                                             Distinct().
                                                             ToArray();

                        EMSPIds          = RemoteParty.Roles.Where (credentialsRole => credentialsRole.Role == Role.EMSP).
                                                             Select(credentialsRole => EMSP_Id.Parse($"{LocalAccessInfo.Roles.First().CountryCode}-{LocalAccessInfo.Roles.First().PartyId}")).
                                                             Distinct().
                                                             ToArray();

                        if (FromCountryCode.HasValue &&
                            FromPartyId.    HasValue)
                        {

                            CPOId            = CPO_Id. Parse($"{FromCountryCode}*{FromPartyId}");
                            EMSPId           = EMSP_Id.Parse($"{FromCountryCode}-{FromPartyId}");

                            if (CPOId. HasValue && !CPOIds. Contains(CPOId. Value))
                                CPOId   = null;

                            if (EMSPId.HasValue && !EMSPIds.Contains(EMSPId.Value))
                                EMSPId  = null;

                        }

                        if (!FromCountryCode.HasValue &&
                            !FromPartyId.    HasValue)
                        {

                            if (CPOIds. Any())
                                CPOId  = CPOIds. First();

                            if (EMSPIds.Any())
                                EMSPId = EMSPIds.First();

                        }

                    }

                    else if (parties.Count() > 1      &&
                             FromCountryCode.HasValue &&
                             FromPartyId.    HasValue)
                    {

                        var filteredParties = parties.Where(party => party.Roles.Any(credentialsRole => credentialsRole.CountryCode == FromCountryCode.Value) &&
                                                                     party.Roles.Any(credentialsRole => credentialsRole.PartyId     == FromPartyId.    Value)).
                                                      ToArray();

                        if (filteredParties.Length == 1)
                        {

                            this.LocalAccessInfo   = new LocalAccessInfo2(AccessToken.Value,
                                                                         filteredParties.First().LocalAccessInfos.First(accessInfo2 => accessInfo2.AccessToken == AccessToken).Status);

                            //this.AccessInfo2  = filteredParties.First().LocalAccessInfos.First(accessInfo2 => accessInfo2.AccessToken == AccessToken);

                            this.RemoteParty  = filteredParties.First();

                        }

                    }

                }


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

        }



        public Boolean TryParseJObjectRequestBody([NotNullWhen(true)]  out JObject?              JSON,
                                                  [NotNullWhen(false)] out OCPIResponse.Builder  OCPIResponseBuilder,
                                                  Boolean                                        AllowEmptyHTTPBody   = false,
                                                  String?                                        JSONLDContext        = null)
        {

            var result = HTTPRequest.TryParseJSONObjectRequestBody(out JSON,
                                                                   out var httpResponseBuilder,
                                                                   AllowEmptyHTTPBody,
                                                                   JSONLDContext);

            if (httpResponseBuilder is not null)
            {
                httpResponseBuilder.Set("X-Request-ID",      RequestId).
                                    Set("X-Correlation-ID",  CorrelationId);
            }

            OCPIResponseBuilder = new OCPIResponse.Builder(this) {
                StatusCode           = result ? 1000 : 2001,
                StatusMessage        = result ? ""   : "Could not parse JSON object in HTTP request body!",
                HTTPResponseBuilder  = httpResponseBuilder
            };

            return result;

        }

        public Boolean TryParseJArrayRequestBody([NotNullWhen(true)]  out JArray?               JSON,
                                                 [NotNullWhen(false)] out OCPIResponse.Builder  OCPIResponseBuilder,
                                                 Boolean                                        AllowEmptyHTTPBody   = false,
                                                 String?                                        JSONLDContext        = null)
        {

            var result = HTTPRequest.TryParseJSONArrayRequestBody(out JSON,
                                                                  out var HTTPResponseBuilder,
                                                                  AllowEmptyHTTPBody,
                                                                  JSONLDContext);

            if (HTTPResponseBuilder is not null)
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
