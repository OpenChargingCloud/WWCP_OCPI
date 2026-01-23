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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using Hermod = org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;
using cloud.charging.open.protocols.OCPIv2_3_0;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_3_0
{

    public class OCPIResponse
    {

        #region Properties

        public OCPIRequest?      Request                   { get; }

        public Int32             StatusCode                { get; }
        public String            StatusMessage             { get; }
        public String?           AdditionalInformation     { get; }
        public DateTimeOffset    Timestamp                 { get; }


        public HTTPResponse?     HTTPResponse              { get; }
        public Request_Id?       RequestId                 { get; }
        public Correlation_Id?   CorrelationId             { get; }
        public Hermod.Location?  HTTPLocation              { get; }

        public Party_Id?         FromPartyId               { get; }
        public CountryCode?      FromCountryCode           { get; }
        public Party_Id?         ToPartyId                 { get; }
        public CountryCode?      ToCountryCode             { get; }

        #endregion

        #region Constructor(s)

        public OCPIResponse(OCPIRequest       Request,

                            Int32             StatusCode,
                            String            StatusMessage,
                            String?           AdditionalInformation   = null,
                            DateTimeOffset?   Timestamp               = null,

                            HTTPResponse?     HTTPResponse            = null,
                            Request_Id?       RequestId               = null,
                            Correlation_Id?   CorrelationId           = null,
                            Hermod.Location?  HTTPLocation            = null,

                            CountryCode?      FromCountryCode         = null,
                            Party_Id?         FromPartyId             = null,
                            CountryCode?      ToCountryCode           = null,
                            Party_Id?         ToPartyId               = null)

        {

            this.Request                = Request;

            this.StatusCode             = StatusCode;
            this.StatusMessage          = StatusMessage;
            this.AdditionalInformation  = AdditionalInformation;
            this.Timestamp              = Timestamp ?? org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            this.HTTPResponse           = HTTPResponse;
            this.RequestId              = RequestId;
            this.CorrelationId          = CorrelationId;
            this.HTTPLocation           = HTTPLocation;

            this.FromCountryCode        = FromCountryCode;
            this.FromPartyId            = FromPartyId;
            this.ToCountryCode          = ToCountryCode;
            this.ToPartyId              = ToPartyId;

        }

        public OCPIResponse(Int32             StatusCode,
                            String            StatusMessage,
                            String?           AdditionalInformation   = null,
                            DateTimeOffset?   Timestamp               = null,

                            HTTPResponse?     HTTPResponse            = null,
                            Request_Id?       RequestId               = null,
                            Correlation_Id?   CorrelationId           = null,
                            Hermod.Location?  HTTPLocation            = null,

                            CountryCode?      FromCountryCode         = null,
                            Party_Id?         FromPartyId             = null,
                            CountryCode?      ToCountryCode           = null,
                            Party_Id?         ToPartyId               = null)

        {

            this.StatusCode             = StatusCode;
            this.StatusMessage          = StatusMessage;
            this.AdditionalInformation  = AdditionalInformation;
            this.Timestamp              = Timestamp ?? org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            this.HTTPResponse           = HTTPResponse;
            this.RequestId              = RequestId;
            this.CorrelationId          = CorrelationId;
            this.HTTPLocation           = HTTPLocation;

            this.FromCountryCode        = FromCountryCode;
            this.FromPartyId            = FromPartyId;
            this.ToCountryCode          = ToCountryCode;
            this.ToPartyId              = ToPartyId;

        }

        #endregion


        #region ToJSON()

        public JObject ToJSON()
        {

            var json = JSONObject.Create(

                                 new JProperty("status_code",             StatusCode),

                           StatusMessage.IsNotNullOrEmpty()
                               ? new JProperty("status_message",          StatusMessage)
                               :  null,

                           AdditionalInformation.IsNotNullOrEmpty()
                               ? new JProperty("additionalInformation",   AdditionalInformation)
                               : null,

                           RequestId.HasValue
                               ? new JProperty("requestId",               RequestId.    Value.ToString())
                               : null,

                           CorrelationId.HasValue
                               ? new JProperty("correlationId",           CorrelationId.Value.ToString())
                               : null,

                           HTTPLocation.HasValue
                               ? new JProperty("httpLocation",            HTTPLocation. Value.ToString())
                               : null,

                                 new JProperty("timestamp",               Timestamp.          ToISO8601())

                       );

            return json;

        }

        #endregion

        #region (static) Parse(Response, RequestId, CorrelationId)

        public static OCPIResponse Parse(HTTPResponse    Response,
                                         Request_Id      RequestId,
                                         Correlation_Id  CorrelationId)
        {

            OCPIResponse? result = default;

            try
            {

                var RemoteRequestId      = Response.TryParseHeaderStruct                ("X-Request-ID",           Request_Id.     TryParse, RequestId);
                var RemoteCorrelationId  = Response.TryParseHeaderStruct                ("X-Correlation-ID",       Correlation_Id. TryParse, CorrelationId);
                var location             = Response.TryParseHeaderField<Hermod.Location>("Location",               Hermod.Location.TryParse);

                var fromPartyId          = Response.TryParseHeaderStruct<Party_Id>      ("OCPI-from-party-id",     Party_Id.       TryParse);
                var fromCountryCode      = Response.TryParseHeaderStruct<CountryCode>   ("OCPI-from-country-code", CountryCode.    TryParse);
                var toPartyId            = Response.TryParseHeaderStruct<Party_Id>      ("OCPI-to-party-id",       Party_Id.       TryParse);
                var toCountryCode        = Response.TryParseHeaderStruct<CountryCode>   ("OCPI-to-country-code",   CountryCode.    TryParse);

                if (Response.HTTPBody?.Length > 0)
                {

                    #region Documentation

                    // {
                    //     "status_code":      1000,
                    //     "status_message":  "hello world!",
                    //     "timestamp":       "2020-10-05T21:15:30.134Z"
                    // }

                    #endregion

                    var text  = Response.HTTPBodyAsUTF8String;
                    var json  = text is not null
                                    ? JObject.Parse(text)
                                    : null;

                    if (json is not null)
                    {

                        var timestamp  = json["timestamp"]?.Value<DateTime>();
                        if (timestamp.HasValue && timestamp.Value.Kind != DateTimeKind.Utc)
                            timestamp  = timestamp.Value.ToUniversalTime();

                        return new OCPIResponse(json["status_code"]?.   Value<Int32>()  ?? -1,
                                                json["status_message"]?.Value<String>() ?? String.Empty,
                                                json["data"]?.          Value<String>(),
                                                timestamp,
                                                Response,
                                                RemoteRequestId,
                                                RemoteCorrelationId,
                                                location,

                                                fromCountryCode,
                                                fromPartyId,
                                                toCountryCode,
                                                toPartyId);

                    }

                }

                result ??= new OCPIResponse(-1,
                                            Response.HTTPStatusCode.Code + " - " + Response.HTTPStatusCode.Description,
                                            null,
                                            Response.Timestamp,

                                            Response,
                                            RemoteRequestId,
                                            RemoteCorrelationId);

            }
            catch (Exception e)
            {

                result = new OCPIResponse(-1,
                                          e.Message,
                                          e.StackTrace,
                                          org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                          Response,
                                          RequestId,
                                          CorrelationId);

            }

            return result;

        }

        #endregion


        #region (static) Error    (StatusMessage,             AdditionalInformation   = null, Timestamp = null, ...)

        public static OCPIResponse Error(String           StatusMessage,
                                         String?          AdditionalInformation   = null,
                                         DateTimeOffset?  Timestamp               = null,

                                         HTTPResponse?    HTTPResponse            = null,
                                         Request_Id?      RequestId               = null,
                                         Correlation_Id?  CorrelationId           = null)

            => new(null,
                   -1,
                   StatusMessage,
                   AdditionalInformation,
                   Timestamp,

                   HTTPResponse,
                   RequestId,
                   CorrelationId);

        #endregion

        #region (static) Error    (StatusCode, StatusMessage, AdditionalInformation   = null, Timestamp = null, ...)

        public static OCPIResponse Error(Int32            StatusCode,
                                         String           StatusMessage,
                                         String?          AdditionalInformation   = null,
                                         DateTimeOffset?  Timestamp               = null,

                                         HTTPResponse?    HTTPResponse            = null,
                                         Request_Id?      RequestId               = null,
                                         Correlation_Id?  CorrelationId           = null)

            => new(null,
                   StatusCode,
                   StatusMessage,
                   AdditionalInformation,
                   Timestamp,

                   HTTPResponse,
                   RequestId,
                   CorrelationId);

        #endregion

        #region (static) Exception(Exception,                                                 Timestamp = null,...)

        public static OCPIResponse Exception(Exception        Exception,
                                             DateTimeOffset?  Timestamp               = null,

                                             HTTPResponse?    HTTPResponse            = null,
                                             Request_Id?      RequestId               = null,
                                             Correlation_Id?  CorrelationId           = null)

            => new(null,
                   -1,
                   Exception.Message,
                   Exception.StackTrace,
                   Timestamp,

                   HTTPResponse,
                   RequestId,
                   CorrelationId);

        #endregion


        public class Builder
        {

            #region Properties

            public OCPIRequest?           Request                  { get; }

            public JToken?                Data                     { get; set; }
            public Int32?                 StatusCode               { get; set; }
            public String?                StatusMessage            { get; set; }

            public String?                AdditionalInformation    { get; set; }
            public DateTimeOffset?        Timestamp                { get; set; }

            public Request_Id?            RequestId                { get; set; }
            public Correlation_Id?        CorrelationId            { get; set; }
            public Hermod.Location?       HTTPLocation             { get; set; }

            public HTTPResponse.Builder?  HTTPResponseBuilder      { get; set; }

            #endregion

            #region Constructor(s)

            public Builder(OCPIRequest       Request,
                           Int32?            StatusCode              = null,
                           String?           StatusMessage           = null,
                           String?           AdditionalInformation   = null,
                           DateTimeOffset?   Timestamp               = null,

                           Request_Id?       RequestId               = null,
                           Correlation_Id?   CorrelationId           = null,
                           Hermod.Location?  HTTPLocation            = null)
            {

                this.Request                = Request;
                this.StatusCode             = StatusCode;
                this.StatusMessage          = StatusMessage;
                this.AdditionalInformation  = AdditionalInformation;
                this.Timestamp              = Timestamp;

                this.RequestId              = RequestId;
                this.CorrelationId          = CorrelationId;
                this.HTTPLocation           = HTTPLocation;

            }

            #endregion


            #region ToHTTPResponseBuilder()

            public HTTPResponse.Builder ToHTTPResponseBuilder()
            {

                Timestamp                                      ??= org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

                HTTPResponseBuilder                            ??= new HTTPResponse.Builder(Request?.HTTPRequest);
                HTTPResponseBuilder.Server                     ??= Request?.HTTPRequest.HTTPServerX?.HTTPServerName;
                HTTPResponseBuilder.Date                       ??= Timestamp.Value;
                HTTPResponseBuilder.AccessControlAllowOrigin   ??= "*";
                HTTPResponseBuilder.AccessControlAllowHeaders  ??= [ "Authorization" ];
                HTTPResponseBuilder.Vary                       ??= "Accept";
                HTTPResponseBuilder.Connection                 ??= ConnectionType.Close;

                if (Request is not null)
                {

                    if (Request.HTTPRequest.HTTPMethod != HTTPMethod.OPTIONS)
                    {

                        HTTPResponseBuilder.ContentType = HTTPContentType.Application.JSON_UTF8;

                        if (HTTPResponseBuilder.Content is null)
                            HTTPResponseBuilder.Content = JSONObject.Create(

                                                              Data is not null
                                                                  ? new JProperty("data",                    Data)
                                                                  : null,

                                                                    new JProperty("status_code",             StatusCode ?? 2000),

                                                              StatusMessage.IsNotNullOrEmpty()
                                                                  ? new JProperty("status_message",          StatusMessage)
                                                                  :  null,

                                                              AdditionalInformation.IsNotNullOrEmpty()
                                                                  ? new JProperty("additionalInformation",   AdditionalInformation)
                                                                  : null,

                                                              RequestId.HasValue
                                                                  ? new JProperty("requestId",               RequestId.    Value.ToString())
                                                                  : null,

                                                              CorrelationId.HasValue
                                                                  ? new JProperty("correlationId",           CorrelationId.Value.ToString())
                                                                  : null,

                                                              HTTPLocation.HasValue
                                                                  ? new JProperty("httpLocation",            HTTPLocation. Value.ToString())
                                                                  : null,

                                                                    new JProperty("timestamp",               Timestamp.    Value.ToISO8601())

                                                          ).ToUTF8Bytes();

                    }

                    if (Request.RequestId.    HasValue)
                        HTTPResponseBuilder.Set("X-Request-ID",      Request.RequestId.    Value);

                    if (Request.CorrelationId.HasValue)
                        HTTPResponseBuilder.Set("X-Correlation-ID",  Request.CorrelationId.Value);

                }

                HTTPResponseBuilder.SubprotocolResponse = this;

                return HTTPResponseBuilder;

            }

            #endregion

            #region ToImmutable

            public OCPIResponse ToImmutable

                => new (Request,
                        StatusCode ?? 3000,
                        StatusMessage,
                        AdditionalInformation,
                        Timestamp ?? org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                        ToHTTPResponseBuilder().AsImmutable);

            #endregion

        }


    }



    public class OCPIResponse<TResponse> : OCPIResponse

        where TResponse : class

    {

        #region Properties

        public TResponse?  Data    { get; }

        #endregion

        #region Constructor(s)

        public OCPIResponse(TResponse?        Data,

                            Int32             StatusCode,
                            String            StatusMessage,
                            String?           AdditionalInformation   = null,
                            DateTimeOffset?   Timestamp               = null,

                            HTTPResponse?     HTTPResponse            = null,
                            Request_Id?       RequestId               = null,
                            Correlation_Id?   CorrelationId           = null,
                            Hermod.Location?  Location                = null,

                            CountryCode?      FromCountryCode         = null,
                            Party_Id?         FromPartyId             = null,
                            CountryCode?      ToCountryCode           = null,
                            Party_Id?         ToPartyId               = null)

            : base(StatusCode,
                   StatusMessage,
                   AdditionalInformation,
                   Timestamp,

                   HTTPResponse,
                   RequestId,
                   CorrelationId,
                   Location,

                   FromCountryCode,
                   FromPartyId,
                   ToCountryCode,
                   ToPartyId)

        {

            this.Data  = Data;

        }

        public OCPIResponse(Int32             StatusCode,
                            String            StatusMessage,
                            String?           AdditionalInformation   = null,
                            DateTimeOffset?   Timestamp               = null,

                            HTTPResponse?     HTTPResponse            = null,
                            Request_Id?       RequestId               = null,
                            Correlation_Id?   CorrelationId           = null,
                            Hermod.Location?  Location                = null,

                            CountryCode?      FromCountryCode         = null,
                            Party_Id?         FromPartyId             = null,
                            CountryCode?      ToCountryCode           = null,
                            Party_Id?         ToPartyId               = null)

            : this(null,

                   StatusCode,
                   StatusMessage,
                   AdditionalInformation,
                   Timestamp,

                   HTTPResponse,
                   RequestId,
                   CorrelationId,
                   Location,

                   FromCountryCode,
                   FromPartyId,
                   ToCountryCode,
                   ToPartyId)

        { }

        #endregion


        #region (static) Create   (Data, Serializer, StatusCode, StatusMessage, AdditionalInformation   = null, Timestamp = null, ...)

        public static JObject Create(TResponse                Data,
                                     Func<TResponse, JToken>  Serializer,
                                     Int32                    StatusCode,
                                     String                   StatusMessage,
                                     String?                  AdditionalInformation   = null,
                                     DateTimeOffset?          Timestamp               = null,

                                     HTTPResponse?            Response                = null,
                                     Request_Id?              RequestId               = null,
                                     Correlation_Id?          CorrelationId           = null,
                                     Hermod.Location?         Location                = null,

                                     CountryCode?             FromCountryCode         = null,
                                     Party_Id?                FromPartyId             = null,
                                     CountryCode?             ToCountryCode           = null,
                                     Party_Id?                ToPartyId               = null)

        {

            return new OCPIResponse<TResponse>(Data,
                                               StatusCode,
                                               StatusMessage,
                                               AdditionalInformation,
                                               Timestamp,

                                               Response,
                                               RequestId,
                                               CorrelationId,
                                               Location,

                                               FromCountryCode,
                                               FromPartyId,
                                               ToCountryCode,
                                               ToPartyId).ToJSON(Serializer);

        }

        #endregion

        #region (static) Error    (StatusMessage,                               AdditionalInformation   = null, Timestamp = null, ...)

        public new static OCPIResponse<TResponse> Error(String           StatusMessage,
                                                        String?          AdditionalInformation   = null,
                                                        DateTimeOffset?  Timestamp               = null,

                                                        HTTPResponse?    HTTPResponse            = null,
                                                        Request_Id?      RequestId               = null,
                                                        Correlation_Id?  CorrelationId           = null)

            => new(null,
                   -1,
                   StatusMessage,
                   AdditionalInformation,
                   Timestamp,

                   HTTPResponse,
                   RequestId,
                   CorrelationId);

        #endregion

        #region (static) Error    (                  StatusCode, StatusMessage, AdditionalInformation   = null, Timestamp = null, ...)

        public new static OCPIResponse<TResponse> Error(Int32            StatusCode,
                                                        String           StatusMessage,
                                                        String?          AdditionalInformation   = null,
                                                        DateTimeOffset?  Timestamp               = null,

                                                        HTTPResponse?    HTTPResponse            = null,
                                                        Request_Id?      RequestId               = null,
                                                        Correlation_Id?  CorrelationId           = null)

            => new(StatusCode,
                   StatusMessage,
                   AdditionalInformation,
                   Timestamp,

                   HTTPResponse,
                   RequestId,
                   CorrelationId);

        #endregion

        #region (static) Exception(Exception,                                                                   Timestamp = null,...)

        public new static OCPIResponse<TResponse> Exception(Exception        Exception,
                                                            DateTimeOffset?  Timestamp               = null,

                                                            HTTPResponse?    HTTPResponse            = null,
                                                            Request_Id?      RequestId               = null,
                                                            Correlation_Id?  CorrelationId           = null)

            => new(-1,
                   Exception.Message,
                   Exception.StackTrace,
                   Timestamp,

                   HTTPResponse,
                   RequestId,
                   CorrelationId);

        #endregion


        #region ToJSON(Serializer = null)

        public JObject ToJSON(Func<TResponse, JToken>? Serializer = null)

        {

            var json = JSONObject.Create(

                                 new JProperty("data",                    Serializer?.Invoke(Data)),
                                 new JProperty("status_code",             StatusCode),

                           StatusMessage.IsNotNullOrEmpty()
                               ? new JProperty("status_message",          StatusMessage)
                               :  null,

                           AdditionalInformation.IsNotNullOrEmpty()
                               ? new JProperty("additionalInformation",   AdditionalInformation)
                               : null,

                           RequestId.HasValue
                               ? new JProperty("requestId",               RequestId.    Value.ToString())
                               : null,

                           CorrelationId.HasValue
                               ? new JProperty("correlationId",           CorrelationId.Value.ToString())
                               : null,

                           HTTPLocation.HasValue
                               ? new JProperty("httpLocation",            HTTPLocation.     Value.ToString())
                               : null,

                                 new JProperty("timestamp",               Timestamp.          ToISO8601())

                       );

            return json;

        }

        #endregion


        #region (static) ParseJArray (Response, RequestId, CorrelationId, Parser)

        public static OCPIResponse<IEnumerable<TResponse>> ParseJArray<TElements>(HTTPResponse              Response,
                                                                                  Request_Id                RequestId,
                                                                                  Correlation_Id            CorrelationId,
                                                                                  Func<JObject, TElements>  Parser)
        {

            OCPIResponse<IEnumerable<TResponse>>? result = default;

            try
            {

                var remoteRequestId      = Response.TryParseHeaderStruct                ("X-Request-ID",           Request_Id.     TryParse, RequestId);
                var remoteCorrelationId  = Response.TryParseHeaderStruct                ("X-Correlation-ID",       Correlation_Id. TryParse, CorrelationId);
                var remoteLocation       = Response.TryParseHeaderField<Hermod.Location>("Location",               Hermod.Location.TryParse);

                var fromCountryCode      = Response.TryParseHeaderStruct<CountryCode>   ("OCPI-from-country-code", CountryCode.    TryParse);
                var fromPartyId          = Response.TryParseHeaderStruct<Party_Id>      ("OCPI-from-party-id",     Party_Id.       TryParse);
                var toCountryCode        = Response.TryParseHeaderStruct<CountryCode>   ("OCPI-to-country-code",   CountryCode.    TryParse);
                var toPartyId            = Response.TryParseHeaderStruct<Party_Id>      ("OCPI-to-party-id",       Party_Id.       TryParse);

                if (Response.HTTPBody?.Length > 0)
                {

                    var text  = Response.HTTPBodyAsUTF8String;
                    var json  = text is not null
                                    ? JObject.Parse(text)
                                    : null;

                    if (json is not null)
                    {

                        #region Documentation

                        // {
                        //   "data": [
                        //     {
                        //       "version": "2.2",
                        //       "url":     "https://example.com/ocpi/versions/2.2/"
                        //     }
                        //   ],
                        //   "status_code":     1000,
                        //   "status_message": "hello world!",
                        //   "timestamp":      "2020-10-05T21:15:30.134Z"
                        // }

                        #endregion

                        var statusCode     = json["status_code"]?.   Value<Int32>() ?? -1;
                        var statusMessage  = json["status_message"]?.Value<String>();
                        var timestamp      = json["timestamp"]?.     Value<DateTime>();
                        if (timestamp.HasValue && timestamp.Value.Kind != DateTimeKind.Utc)
                            timestamp      = timestamp.Value.ToUniversalTime();

                        if (Response.HTTPStatusCode == HTTPStatusCode.OK ||
                            Response.HTTPStatusCode == HTTPStatusCode.Created)
                        {

                            var items          = new List<TResponse>();
                            var exceptions     = new List<String>();

                            if (json["data"] is JArray JSONArray)
                            {
                                foreach (var item in JSONArray)
                                {
                                    try
                                    {
                                        items.Add((TResponse) (Object) Parser(item as JObject));
                                    }
                                    catch (Exception e)
                                    {
                                        exceptions.Add(e.Message);
                                    }
                                }
                            }

                            result = new OCPIResponse<IEnumerable<TResponse>>(items,
                                                                              statusCode,
                                                                              statusMessage ?? String.Empty,
                                                                              exceptions.Any() ? exceptions.AggregateWith(Environment.NewLine) : null,
                                                                              timestamp ?? org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,

                                                                              Response,
                                                                              remoteRequestId,
                                                                              remoteCorrelationId,
                                                                              remoteLocation,

                                                                              fromCountryCode,
                                                                              fromPartyId,
                                                                              toCountryCode,
                                                                              toPartyId);

                        }

                        else
                            result = new OCPIResponse<IEnumerable<TResponse>>(Array.Empty<TResponse>(),
                                                                              statusCode,
                                                                              statusMessage ?? String.Empty,
                                                                              null,
                                                                              timestamp ?? org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,

                                                                              Response,
                                                                              remoteRequestId,
                                                                              remoteCorrelationId,
                                                                              remoteLocation,

                                                                              fromCountryCode,
                                                                              fromPartyId,
                                                                              toCountryCode,
                                                                              toPartyId);

                    }

                }

                result ??= new OCPIResponse<IEnumerable<TResponse>>(Array.Empty<TResponse>(),
                                                                    -1,
                                                                    Response.HTTPStatusCode.Code + " - " + Response.HTTPStatusCode.Description,
                                                                    null,
                                                                    Response.Timestamp,

                                                                    Response,
                                                                    remoteRequestId,
                                                                    remoteCorrelationId,
                                                                    remoteLocation,

                                                                    fromCountryCode,
                                                                    fromPartyId,
                                                                    toCountryCode,
                                                                    toPartyId);

            }
            catch (Exception e)
            {

                result = new OCPIResponse<IEnumerable<TResponse>>(Array.Empty<TResponse>(),
                                                                  -1,
                                                                  e.Message,
                                                                  e.StackTrace,
                                                                  org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                                                  Response,
                                                                  RequestId,
                                                                  CorrelationId);

            }

            return result;

        }

        #endregion

        #region (static) ParseJObject(Response, RequestId, CorrelationId, Parser)

        public static OCPIResponse<TResponse> ParseJObject(HTTPResponse              Response,
                                                           Request_Id                RequestId,
                                                           Correlation_Id            CorrelationId,
                                                           Func<JObject, TResponse>  Parser)
        {

            OCPIResponse<TResponse>? result = default;

            try
            {

                var remoteRequestId      = Response.TryParseHeaderStruct                ("X-Request-ID",           Request_Id.     TryParse, RequestId);
                var remoteCorrelationId  = Response.TryParseHeaderStruct                ("X-Correlation-ID",       Correlation_Id. TryParse, CorrelationId);
                var remoteLocation       = Response.TryParseHeaderField<Hermod.Location>("Location",               Hermod.Location.TryParse);

                var fromCountryCode      = Response.TryParseHeaderStruct<CountryCode>   ("OCPI-from-country-code", CountryCode.    TryParse);
                var fromPartyId          = Response.TryParseHeaderStruct<Party_Id>      ("OCPI-from-party-id",     Party_Id.       TryParse);
                var toCountryCode        = Response.TryParseHeaderStruct<CountryCode>   ("OCPI-to-country-code",   CountryCode.    TryParse);
                var toPartyId            = Response.TryParseHeaderStruct<Party_Id>      ("OCPI-to-party-id",       Party_Id.       TryParse);

                if (Response.HTTPBody?.Length > 0)
                {

                    var text  = Response.HTTPBodyAsUTF8String;
                    var json  = text is not null
                                    ? JObject.Parse(text)
                                    : null;

                    if (json is not null)
                    {

                        var statusCode     = json["status_code"]?.   Value<Int32>();
                        var statusMessage  = json["status_message"]?.Value<String>();
                        var timestamp      = json["timestamp"]?.     Value<DateTime>();
                        if (timestamp.HasValue && timestamp.Value.Kind != DateTimeKind.Utc)
                            timestamp      = timestamp.Value.ToUniversalTime();

                        if ((Response.HTTPStatusCode == HTTPStatusCode.OK ||
                             Response.HTTPStatusCode == HTTPStatusCode.Created) &&
                            statusCode >= 1000 &&
                            statusCode <  2000)
                        {

                            if (json["data"] is JObject JSONObject)
                                result = new OCPIResponse<TResponse>(Parser(JSONObject),
                                                                     statusCode    ?? 3000,
                                                                     statusMessage ?? String.Empty,
                                                                     null,
                                                                     timestamp,

                                                                     Response,
                                                                     remoteRequestId,
                                                                     remoteCorrelationId,
                                                                     remoteLocation,

                                                                     fromCountryCode,
                                                                     fromPartyId,
                                                                     toCountryCode,
                                                                     toPartyId);

                        }

                        else
                            result = new OCPIResponse<TResponse>(statusCode    ?? 3000,
                                                                 statusMessage ?? String.Empty,
                                                                 null,
                                                                 timestamp,

                                                                 Response,
                                                                 remoteRequestId,
                                                                 remoteCorrelationId,
                                                                 remoteLocation,

                                                                 fromCountryCode,
                                                                 fromPartyId,
                                                                 toCountryCode,
                                                                 toPartyId);

                    }

                }

                result ??= new OCPIResponse<TResponse>(-1,
                                                       Response.HTTPStatusCode.Code + " - " + Response.HTTPStatusCode.Description,
                                                       null,
                                                       Response.Timestamp,

                                                       Response,
                                                       remoteRequestId,
                                                       remoteCorrelationId,
                                                       remoteLocation,

                                                       fromCountryCode,
                                                       fromPartyId,
                                                       toCountryCode,
                                                       toPartyId);

            }
            catch (Exception e)
            {

                result = new OCPIResponse<TResponse>(-1,
                                                     e.Message,
                                                     e.StackTrace);

            }

            result ??= new OCPIResponse<TResponse>(-1,
                                                   String.Empty);

            return result;

        }

        #endregion


    }



    public class OCPIResponse<TRequest, TResponse> : OCPIResponse<TResponse>

        where TResponse : class

    {

        #region Properties

        public TRequest?  Request2    { get; }

        #endregion

        #region Constructor(s)

        public OCPIResponse(TRequest          Request,
                            TResponse?        Data,
                            Int32             StatusCode,
                            String            StatusMessage,
                            String?           AdditionalInformation   = null,
                            DateTimeOffset?   Timestamp               = null,

                            HTTPResponse?     HTTPResponse            = null,
                            Request_Id?       RequestId               = null,
                            Correlation_Id?   CorrelationId           = null,
                            Hermod.Location?  Location                = null,

                            CountryCode?      FromCountryCode         = null,
                            Party_Id?         FromPartyId             = null,
                            CountryCode?      ToCountryCode           = null,
                            Party_Id?         ToPartyId               = null)

            : base(Data,
                   StatusCode,
                   StatusMessage,
                   AdditionalInformation,
                   Timestamp,

                   HTTPResponse,
                   RequestId,
                   CorrelationId,
                   Location,

                   FromCountryCode,
                   FromPartyId,
                   ToCountryCode,
                   ToPartyId)

        {

            this.Request2  = Request;

        }

        #endregion


        #region (static) Error    (Request, StatusCode, StatusMessage, AdditionalInformation = null, Timestamp = null, ...)

        public static OCPIResponse<TRequest, TResponse> Error(TRequest         Request,
                                                              Int32            StatusCode,
                                                              String           StatusMessage,
                                                              String?          AdditionalInformation   = null,
                                                              DateTimeOffset?  Timestamp               = null,

                                                              HTTPResponse?    HTTPResponse            = null,
                                                              Request_Id?      RequestId               = null,
                                                              Correlation_Id?  CorrelationId           = null)

            => new(Request,
                   null,
                   StatusCode,
                   StatusMessage,
                   AdditionalInformation,
                   Timestamp,

                   HTTPResponse,
                   RequestId,
                   CorrelationId);

        #endregion

        #region (static) Error    (Request,             StatusMessage, AdditionalInformation = null, Timestamp = null, ...)

        public static OCPIResponse<TRequest, TResponse> Error(TRequest         Request,
                                                              String           StatusMessage,
                                                              String?          AdditionalInformation   = null,
                                                              DateTimeOffset?  Timestamp               = null,

                                                              HTTPResponse?    HTTPResponse            = null,
                                                              Request_Id?      RequestId               = null,
                                                              Correlation_Id?  CorrelationId           = null)

            => new(Request,
                   null,
                   -1,
                   StatusMessage,
                   AdditionalInformation,
                   Timestamp,

                   HTTPResponse,
                   RequestId,
                   CorrelationId);

        #endregion

        #region (static) Error    (StatusCode,          StatusMessage, AdditionalInformation = null, Timestamp = null, ...)

        public static new OCPIResponse<TRequest, TResponse> Error(Int32            StatusCode,
                                                                  String           StatusMessage,
                                                                  String?          AdditionalInformation   = null,
                                                                  DateTimeOffset?  Timestamp               = null,

                                                                  HTTPResponse?    HTTPResponse            = null,
                                                                  Request_Id?      RequestId               = null,
                                                                  Correlation_Id?  CorrelationId           = null)

            => new(default,
                   null,
                   StatusCode,
                   StatusMessage,
                   AdditionalInformation,
                   Timestamp,

                   HTTPResponse,
                   RequestId,
                   CorrelationId);

        #endregion

        #region (static) Error    (StatusMessage,                      AdditionalInformation = null, Timestamp = null, ...)

        public static new OCPIResponse<TRequest, TResponse> Error(String           StatusMessage,
                                                                  String?          AdditionalInformation   = null,
                                                                  DateTimeOffset?  Timestamp               = null,

                                                                  HTTPResponse?    HTTPResponse            = null,
                                                                  Request_Id?      RequestId               = null,
                                                                  Correlation_Id?  CorrelationId           = null)

            => new(default,
                   null,
                   -1,
                   StatusMessage,
                   AdditionalInformation,
                   Timestamp,

                   HTTPResponse,
                   RequestId,
                   CorrelationId);

        #endregion

        #region (static) Exception(StatusCode,          StatusMessage, AdditionalInformation = null, Timestamp = null, ...)

        public static new OCPIResponse<TRequest, TResponse> Exception(Exception        Exception,
                                                                      DateTimeOffset?  Timestamp               = null,

                                                                      HTTPResponse?    HTTPResponse            = null,
                                                                      Request_Id?      RequestId               = null,
                                                                      Correlation_Id?  CorrelationId           = null)

            => new(default,
                   null,
                   -1,
                   Exception.Message,
                   Exception.StackTrace,
                   Timestamp,

                   HTTPResponse,
                   RequestId,
                   CorrelationId);

        #endregion


        #region (static) ParseJArray (Request, RequestId, CorrelationId, Response, Parser)

        public static OCPIResponse<TRequest, IEnumerable<TResponse>> ParseJArray(TRequest                  Request,
                                                                                 Request_Id                RequestId,
                                                                                 Correlation_Id            CorrelationId,
                                                                                 HTTPResponse              Response,
                                                                                 Func<JObject, TResponse>  Parser)
        {

            var r = ParseJArray(Response,
                                RequestId,
                                CorrelationId,
                                Parser);

            return new OCPIResponse<TRequest, IEnumerable<TResponse>>(Request,
                                                                      r.Data,
                                                                      r.StatusCode,
                                                                      r.StatusMessage,
                                                                      r.AdditionalInformation,
                                                                      r.Timestamp,

                                                                      r.HTTPResponse,
                                                                      r.RequestId,
                                                                      r.CorrelationId,
                                                                      r.HTTPLocation,

                                                                      r.FromCountryCode,
                                                                      r.FromPartyId,
                                                                      r.ToCountryCode,
                                                                      r.ToPartyId);

        }

        #endregion

        #region (static) ParseJObject(Request, RequestId, CorrelationId, Response, Parser)

        public static OCPIResponse<TRequest, TResponse> ParseJObject(TRequest                  Request,
                                                                     HTTPResponse              Response,
                                                                     Request_Id                RequestId,
                                                                     Correlation_Id            CorrelationId,
                                                                     Func<JObject, TResponse>  Parser)
        {

            var r = ParseJObject(Response,
                                 RequestId,
                                 CorrelationId,
                                 Parser);

            return new OCPIResponse<TRequest, TResponse>(Request,
                                                         r.Data,
                                                         r.StatusCode,
                                                         r.StatusMessage,
                                                         r.AdditionalInformation,
                                                         r.Timestamp,

                                                         r.HTTPResponse,
                                                         r.RequestId,
                                                         r.CorrelationId,
                                                         r.HTTPLocation,

                                                         r.FromCountryCode,
                                                         r.FromPartyId,
                                                         r.ToCountryCode,
                                                         r.ToPartyId);

        }

        #endregion

    }

}
