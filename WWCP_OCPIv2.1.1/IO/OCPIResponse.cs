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
using Hermod = org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;
using cloud.charging.open.protocols.OCPIv2_1_1;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_1_1
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
                            Hermod.Location?  HTTPLocation            = null)

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

        }

        public OCPIResponse(Int32             StatusCode,
                            String            StatusMessage,
                            String?           AdditionalInformation   = null,
                            DateTimeOffset?   Timestamp               = null,

                            HTTPResponse?     HTTPResponse            = null,
                            Request_Id?       RequestId               = null,
                            Correlation_Id?   CorrelationId           = null,
                            Hermod.Location?  HTTPLocation            = null)

        {

            this.Request                = null;

            this.StatusCode             = StatusCode;
            this.StatusMessage          = StatusMessage;
            this.AdditionalInformation  = AdditionalInformation;
            this.Timestamp              = Timestamp ?? org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            this.HTTPResponse           = HTTPResponse;
            this.RequestId              = RequestId;
            this.CorrelationId          = CorrelationId;
            this.HTTPLocation           = HTTPLocation;

        }

        #endregion


        public JObject ToJSON()
        {

            var json = JSONObject.Create(

                                 new JProperty("status_code",             StatusCode),

                           StatusMessage.IsNotNullOrEmpty()
                               ? new JProperty("status_message",          StatusMessage)
                               : null,

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


        public static OCPIResponse Parse(HTTPResponse    HTTPResponse,
                                         Request_Id      RequestId,
                                         Correlation_Id  CorrelationId)
        {

            OCPIResponse? result = default;

            try
            {

                var remoteRequestId      = HTTPResponse.TryParseHeaderStruct                ("X-Request-ID",     Request_Id.     TryParse, RequestId);
                var remoteCorrelationId  = HTTPResponse.TryParseHeaderStruct                ("X-Correlation-ID", Correlation_Id. TryParse, CorrelationId);
                var location             = HTTPResponse.TryParseHeaderField<Hermod.Location>("Location",         Hermod.Location.TryParse);

                if (HTTPResponse.HTTPBody?.Length > 0)
                {

                    #region Documentation

                    // {
                    //     "status_code":      1000,
                    //     "status_message":  "hello world!",
                    //     "timestamp":       "2020-10-05T21:15:30.134Z"
                    // }

                    #endregion

                    var json       = JObject.Parse(HTTPResponse.HTTPBodyAsUTF8String);

                    var timestamp  = json["timestamp"]?.Value<DateTime>();
                    if (timestamp.HasValue && timestamp.Value.Kind != DateTimeKind.Utc)
                        timestamp  = timestamp.Value.ToUniversalTime();

                    return new OCPIResponse(
                               json["status_code"]?.   Value<Int32>()  ?? -1,
                               json["status_message"]?.Value<String>() ?? String.Empty,
                               json["data"]?.ToString(),
                               timestamp,
                               HTTPResponse,
                               remoteRequestId,
                               remoteCorrelationId,
                               location
                           );

                }

                else
                    result = new OCPIResponse(
                                 -1,
                                 HTTPResponse.HTTPStatusCode.Code + " - " + HTTPResponse.HTTPStatusCode.Description,
                                 HTTPResponse.EntirePDU,
                                 HTTPResponse.Timestamp,

                                 HTTPResponse,
                                 remoteRequestId,
                                 remoteCorrelationId
                             );

            }
            catch (Exception e)
            {

                result = new OCPIResponse(
                             -1,
                             e.Message,
                             e.StackTrace,
                             org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                             HTTPResponse,
                             RequestId,
                             CorrelationId
                         );

            }

            result ??= new OCPIResponse(
                           -1,
                           String.Empty
                       );

            return result;

        }

        public static Boolean TryParse(HTTPResponse                           HTTPResponse,
                                       Request_Id                             RequestId,
                                       Correlation_Id                         CorrelationId,
                                       [NotNullWhen(true)] out OCPIResponse?  OCPIResponse)
        {

            OCPIResponse = default;

            try
            {

                var remoteRequestId      = HTTPResponse.TryParseHeaderStruct                ("X-Request-ID",     Request_Id.     TryParse, RequestId);
                var remoteCorrelationId  = HTTPResponse.TryParseHeaderStruct                ("X-Correlation-ID", Correlation_Id. TryParse, CorrelationId);
                var location             = HTTPResponse.TryParseHeaderField<Hermod.Location>("Location",         Hermod.Location.TryParse);

                var httpBody             = HTTPResponse.HTTPBodyAsUTF8String;

                if (httpBody?.Length > 0)
                {

                    #region Documentation

                    // {
                    //     "status_code":      1000,
                    //     "status_message":  "hello world!",
                    //     "timestamp":       "2020-10-05T21:15:30.134Z"
                    // }

                    #endregion

                    var json       = JObject.Parse(httpBody ?? "");

                    var timestamp  = json["timestamp"]?.Value<DateTime>();
                    if (timestamp.HasValue && timestamp.Value.Kind != DateTimeKind.Utc)
                        timestamp  = timestamp.Value.ToUniversalTime();

                    OCPIResponse = new OCPIResponse(
                                       json["status_code"]?.   Value<Int32>()  ?? -1,
                                       json["status_message"]?.Value<String>() ?? String.Empty,
                                       json["data"]?.ToString(),
                                       timestamp,
                                       HTTPResponse,
                                       remoteRequestId,
                                       remoteCorrelationId,
                                       location
                                   );

                    return true;

                }

                else
                    OCPIResponse = new OCPIResponse(
                                       -1,
                                       $"{HTTPResponse.HTTPStatusCode.Code} - {HTTPResponse.HTTPStatusCode.Description}",
                                       HTTPResponse.EntirePDU,
                                       HTTPResponse.Timestamp,

                                       HTTPResponse,
                                       remoteRequestId,
                                       remoteCorrelationId
                                   );

            }
            catch (Exception e)
            {

                OCPIResponse = new OCPIResponse(
                                   -1,
                                   e.Message,
                                   e.StackTrace,
                                   org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                   HTTPResponse,
                                   RequestId,
                                   CorrelationId
                               );

            }

            OCPIResponse ??= new OCPIResponse(
                                 -1,
                                 String.Empty
                             );

            return true;

        }


        public static OCPIResponse Error(String           StatusMessage,
                                         String?          AdditionalInformation   = null,
                                         DateTimeOffset?  Timestamp               = null,

                                         HTTPResponse?    HTTPResponse            = null,
                                         Request_Id?      RequestId               = null,
                                         Correlation_Id?  CorrelationId           = null)

            => new(-1,
                   StatusMessage,
                   AdditionalInformation,
                   Timestamp,

                   HTTPResponse,
                   RequestId,
                   CorrelationId);

        public static OCPIResponse Error(Int32            StatusCode,
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

        public static OCPIResponse Exception(Exception        Exception,
                                             DateTimeOffset?  Timestamp       = null,

                                             HTTPResponse?    HTTPResponse    = null,
                                             Request_Id?      RequestId       = null,
                                             Correlation_Id?  CorrelationId   = null)

            => new(-1,
                   Exception.Message,
                   Exception.StackTrace,
                   Timestamp,

                   HTTPResponse,
                   RequestId,
                   CorrelationId);


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
            public URL?                   HTTPLocation             { get; set; }

            public HTTPResponse.Builder?  HTTPResponseBuilder      { get; set; }

            #endregion

            #region Constructor(s)

            public Builder(OCPIRequest      Request,
                           Int32?           StatusCode              = null,
                           String?          StatusMessage           = null,
                           String?          AdditionalInformation   = null,
                           DateTimeOffset?  Timestamp               = null,

                           Request_Id?      RequestId               = null,
                           Correlation_Id?  CorrelationId           = null,
                           URL?             HTTPLocation            = null)
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


            public OCPIResponse ToImmutable

                => new (Request,
                        StatusCode ?? 3000,
                        StatusMessage,
                        AdditionalInformation,
                        Timestamp ?? org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                        ToHTTPResponseBuilder().AsImmutable);


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
                            Hermod.Location?  Location                = null)

            : base(StatusCode,
                   StatusMessage,
                   AdditionalInformation,
                   Timestamp,

                   HTTPResponse,
                   RequestId,
                   CorrelationId,
                   Location)

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
                            Hermod.Location?  Location                = null)

            : this(null,
                   StatusCode,
                   StatusMessage,
                   AdditionalInformation,
                   Timestamp,

                   HTTPResponse,
                   RequestId,
                   CorrelationId,
                   Location)

        { }

        #endregion


        public new static OCPIResponse<TResponse> Error(String           StatusMessage,
                                                        String?          AdditionalInformation   = null,
                                                        DateTimeOffset?  Timestamp               = null,

                                                        HTTPResponse?    HTTPResponse            = null,
                                                        Request_Id?      RequestId               = null,
                                                        Correlation_Id?  CorrelationId           = null)

            => new (-1,
                    StatusMessage,
                    AdditionalInformation,
                    Timestamp,

                    HTTPResponse,
                    RequestId,
                    CorrelationId);

        public new static OCPIResponse<TResponse> Error(Int32            StatusCode,
                                                        String           StatusMessage,
                                                        String?          AdditionalInformation   = null,
                                                        DateTimeOffset?  Timestamp               = null,

                                                        HTTPResponse?    HTTPResponse            = null,
                                                        Request_Id?      RequestId               = null,
                                                        Correlation_Id?  CorrelationId           = null)

            => new (StatusCode,
                    StatusMessage,
                    AdditionalInformation,
                    Timestamp,

                    HTTPResponse,
                    RequestId,
                    CorrelationId);


        public new static OCPIResponse<TResponse> Exception(Exception        Exception,
                                                            DateTimeOffset?  Timestamp               = null,

                                                            HTTPResponse?    HTTPResponse            = null,
                                                            Request_Id?      RequestId               = null,
                                                            Correlation_Id?  CorrelationId           = null)

            => new (-1,
                    Exception.Message,
                    Exception.StackTrace,
                    Timestamp,

                    HTTPResponse,
                    RequestId,
                    CorrelationId);


        public static JObject Create(TResponse                Data,
                                     Func<TResponse, JToken>  Serializer,
                                     Int32                    StatusCode,
                                     String                   StatusMessage,
                                     String?                  AdditionalInformation   = null,
                                     DateTimeOffset?          Timestamp               = null,

                                     HTTPResponse?            Response                = null,
                                     Request_Id?              RequestId               = null,
                                     Correlation_Id?          CorrelationId           = null,
                                     Hermod.Location?         Location                = null)

        {

            return new OCPIResponse<TResponse>(
                       Data,
                       StatusCode,
                       StatusMessage,
                       AdditionalInformation,
                       Timestamp,

                       Response,
                       RequestId,
                       CorrelationId,
                       Location
                   ).ToJSON(Serializer);

        }





        //public HTTPResponse.Builder CreateHTTPResonse(HTTPRequest Request)
        //{
        //    return new HTTPResponse.Builder(Request);
        //}



        public JObject ToJSON(Func<TResponse, JToken>? Serializer = null)

        {

            var json = JSONObject.Create(

                                 new JProperty("data",                    Serializer?.Invoke(Data)),
                                 new JProperty("status_code",             StatusCode),

                           StatusMessage.IsNotNullOrEmpty()
                               ? new JProperty("status_message",          StatusMessage)
                               : null,

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


        public static OCPIResponse<IEnumerable<TResponse>> ParseJArray<TElements>(HTTPResponse              Response,
                                                                                  Request_Id                RequestId,
                                                                                  Correlation_Id            CorrelationId,
                                                                                  Func<JObject, TElements>  Parser)
        {

            OCPIResponse<IEnumerable<TResponse>>? result = default;

            try
            {

                var remoteRequestId      = Response.TryParseHeaderStruct                ("X-Request-ID",     Request_Id.     TryParse, RequestId);
                var remoteCorrelationId  = Response.TryParseHeaderStruct                ("X-Correlation-ID", Correlation_Id. TryParse, CorrelationId);
                var remoteLocation       = Response.TryParseHeaderField<Hermod.Location>("Location",         Hermod.Location.TryParse);

                if (Response.HTTPBody?.Length > 0)
                {

                    var json           = JObject.Parse(Response.HTTPBodyAsUTF8String);

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

                        result = new OCPIResponse<IEnumerable<TResponse>>(
                                     items,
                                     statusCode,
                                     statusMessage ?? String.Empty,
                                     exceptions.Count != 0 ? exceptions.AggregateWith(Environment.NewLine) : null,
                                     timestamp ?? org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,

                                     Response,
                                     remoteRequestId,
                                     remoteCorrelationId,
                                     remoteLocation
                                 );

                    }

                    else
                        result = new OCPIResponse<IEnumerable<TResponse>>(
                                     [],
                                     statusCode,
                                     statusMessage ?? String.Empty,
                                     Response.EntirePDU,
                                     timestamp ?? org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,

                                     Response,
                                     remoteRequestId,
                                     remoteCorrelationId,
                                     remoteLocation
                                 );

                }

                else
                    result = new OCPIResponse<IEnumerable<TResponse>>(
                                 [],
                                 -1,
                                 Response.HTTPStatusCode.Code + " - " + Response.HTTPStatusCode.Description,
                                 Response.EntirePDU,
                                 Response.Timestamp,

                                 Response,
                                 remoteRequestId,
                                 remoteCorrelationId,
                                 remoteLocation
                             );

            }
            catch (Exception e)
            {

                result = new OCPIResponse<IEnumerable<TResponse>>(
                             [],
                             -1,
                             e.Message,
                             e.StackTrace,
                             org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                             Response,
                             RequestId,
                             CorrelationId
                         );

            }

            return result;

        }


        public static OCPIResponse<TResponse> ParseJObject(HTTPResponse              Response,
                                                           Request_Id                RequestId,
                                                           Correlation_Id            CorrelationId,
                                                           Func<JObject, TResponse>  Parser)
        {

            OCPIResponse<TResponse>? result = default;

            try
            {

                var remoteRequestId      = Response.TryParseHeaderStruct                ("X-Request-ID",     Request_Id.     TryParse, RequestId);
                var remoteCorrelationId  = Response.TryParseHeaderStruct                ("X-Correlation-ID", Correlation_Id. TryParse, CorrelationId);
                var remoteLocation       = Response.TryParseHeaderField<Hermod.Location>("Location",         Hermod.Location.TryParse);

                if (Response.HTTPBody?.Length > 0)
                {

                    var json           = JObject.Parse(Response.HTTPBodyAsUTF8String);

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
                                                                 remoteLocation);

                    }

                    else
                        result = new OCPIResponse<TResponse>(statusCode    ?? 3000,
                                                             statusMessage ?? String.Empty,
                                                             Response.EntirePDU,
                                                             timestamp,

                                                             Response,
                                                             remoteRequestId,
                                                             remoteCorrelationId,
                                                             remoteLocation);

                }

                else
                    result = new OCPIResponse<TResponse>(-1,
                                                         Response.HTTPStatusCode.Code + " - " + Response.HTTPStatusCode.Description,
                                                         Response.EntirePDU,
                                                         Response.Timestamp,

                                                         Response,
                                                         remoteRequestId,
                                                         remoteCorrelationId,
                                                         remoteLocation);

            }
            catch (Exception e)
            {

                result = new OCPIResponse<TResponse>(
                             -1,
                             e.Message,
                             e.StackTrace
                         );

            }

            result ??= new OCPIResponse<TResponse>(
                           -1,
                           String.Empty
                       );

            return result;

        }

    }



    public class OCPIResponse<TRequest, TResponse> : OCPIResponse<TResponse>

        where TResponse : class

    {

        #region Properties

        public TRequest  Request2    { get; }

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
                            Hermod.Location?  Location                = null)

            : base(Data,
                   StatusCode,
                   StatusMessage,
                   AdditionalInformation,
                   Timestamp,

                   HTTPResponse,
                   RequestId,
                   CorrelationId,
                   Location)

        {

            this.Request2  = Request;

        }

        #endregion



        public static OCPIResponse<TRequest, TResponse> Error(TRequest         Request,
                                                              Int32            StatusCode,
                                                              String           StatusMessage,
                                                              String?          AdditionalInformation   = null,
                                                              DateTimeOffset?  Timestamp               = null,

                                                              HTTPResponse?    HTTPResponse            = null,
                                                              Request_Id?      RequestId               = null,
                                                              Correlation_Id?  CorrelationId           = null)

            => new (Request,
                    null,
                    StatusCode,
                    StatusMessage,
                    AdditionalInformation,
                    Timestamp,

                    HTTPResponse,
                    RequestId,
                    CorrelationId);


        public static OCPIResponse<TRequest, TResponse> Error(TRequest         Request,
                                                              String           StatusMessage,
                                                              String?          AdditionalInformation   = null,
                                                              DateTimeOffset?  Timestamp               = null,

                                                              HTTPResponse?    HTTPResponse            = null,
                                                              Request_Id?      RequestId               = null,
                                                              Correlation_Id?  CorrelationId           = null)

            => new (Request,
                    null,
                    -1,
                    StatusMessage,
                    AdditionalInformation,
                    Timestamp,

                    HTTPResponse,
                    RequestId,
                    CorrelationId);


        public static new OCPIResponse<TRequest, TResponse> Error(Int32            StatusCode,
                                                                  String           StatusMessage,
                                                                  String?          AdditionalInformation   = null,
                                                                  DateTimeOffset?  Timestamp               = null,

                                                                  HTTPResponse?    HTTPResponse            = null,
                                                                  Request_Id?      RequestId               = null,
                                                                  Correlation_Id?  CorrelationId           = null)

            => new (default,
                    null,
                    StatusCode,
                    StatusMessage,
                    AdditionalInformation,
                    Timestamp,

                    HTTPResponse,
                    RequestId,
                    CorrelationId);


        public static new OCPIResponse<TRequest, TResponse> Error(String           StatusMessage,
                                                                  String?          AdditionalInformation   = null,
                                                                  DateTimeOffset?  Timestamp               = null,

                                                                  HTTPResponse?    HTTPResponse            = null,
                                                                  Request_Id?      RequestId               = null,
                                                                  Correlation_Id?  CorrelationId           = null)

            => new (default,
                    null,
                    -1,
                    StatusMessage,
                    AdditionalInformation,
                    Timestamp,

                    HTTPResponse,
                    RequestId,
                    CorrelationId);


        public static new OCPIResponse<TRequest, TResponse> Exception(Exception        Exception,
                                                                      DateTimeOffset?  Timestamp       = null,

                                                                      HTTPResponse?    HTTPResponse    = null,
                                                                      Request_Id?      RequestId       = null,
                                                                      Correlation_Id?  CorrelationId   = null)

            => new (default,
                    null,
                    -1,
                    Exception.Message,
                    Exception.StackTrace,
                    Timestamp,

                    HTTPResponse,
                    RequestId,
                    CorrelationId);


        public static OCPIResponse<TRequest, IEnumerable<TResponse>> ParseJArray(TRequest                  Request,
                                                                                 Request_Id                RequestId,
                                                                                 Correlation_Id            CorrelationId,
                                                                                 HTTPResponse              Response,
                                                                                 Func<JObject, TResponse>  Parser)
        {

            var r = ParseJArray(
                        Response,
                        RequestId,
                        CorrelationId,
                        Parser
                    );

            return new OCPIResponse<TRequest, IEnumerable<TResponse>>(
                       Request,
                       r.Data,
                       r.StatusCode,
                       r.StatusMessage,
                       r.AdditionalInformation,
                       r.Timestamp,

                       r.HTTPResponse,
                       r.RequestId,
                       r.CorrelationId,
                       r.HTTPLocation
                   );

        }


        public static OCPIResponse<TRequest, TResponse> ParseJObject(TRequest                  Request,
                                                                     HTTPResponse              Response,
                                                                     Request_Id                RequestId,
                                                                     Correlation_Id            CorrelationId,
                                                                     Func<JObject, TResponse>  Parser)
        {

            var r = ParseJObject(
                        Response,
                        RequestId,
                        CorrelationId,
                        Parser
                    );

            return new OCPIResponse<TRequest, TResponse>(
                       Request,
                       r.Data,
                       r.StatusCode,
                       r.StatusMessage,
                       r.AdditionalInformation,
                       r.Timestamp,

                       r.HTTPResponse,
                       r.RequestId,
                       r.CorrelationId,
                       r.HTTPLocation
                   );

        }

    }

}
