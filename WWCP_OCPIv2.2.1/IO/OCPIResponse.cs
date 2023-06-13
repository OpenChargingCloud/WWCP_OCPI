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

using cloud.charging.open.protocols.OCPI;
using cloud.charging.open.protocols.OCPIv2_2_1.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2_1
{

    public class OCPIResponse
    {

        #region Properties

        public OCPIRequest?     Request                   { get; }

        public Int32            StatusCode                { get; }
        public String           StatusMessage             { get; }
        public String?          AdditionalInformation     { get; }
        public DateTime         Timestamp                 { get; }


        public HTTPResponse?    HTTPResponse              { get; }
        public Request_Id?      RequestId                 { get; }
        public Correlation_Id?  CorrelationId             { get; }
        public URL?             Location                  { get; }

        public Party_Id?        FromPartyId               { get; }
        public CountryCode?     FromCountryCode           { get; }
        public Party_Id?        ToPartyId                 { get; }
        public CountryCode?     ToCountryCode             { get; }

        #endregion

        #region Constructor(s)

        public OCPIResponse(OCPIRequest      Request,

                            Int32            StatusCode,
                            String           StatusMessage,
                            String?          AdditionalInformation   = null,
                            DateTime?        Timestamp               = null,

                            HTTPResponse?    HTTPResponse            = null,
                            Request_Id?      RequestId               = null,
                            Correlation_Id?  CorrelationId           = null,
                            URL?             Location                = null,

                            CountryCode?     FromCountryCode         = null,
                            Party_Id?        FromPartyId             = null,
                            CountryCode?     ToCountryCode           = null,
                            Party_Id?        ToPartyId               = null)

        {

            this.Request                = Request;

            this.StatusCode             = StatusCode;
            this.StatusMessage          = StatusMessage;
            this.AdditionalInformation  = AdditionalInformation;
            this.Timestamp              = Timestamp ?? org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            this.HTTPResponse           = HTTPResponse;
            this.RequestId              = RequestId;
            this.CorrelationId          = CorrelationId;
            this.Location               = Location;

            this.FromCountryCode        = FromCountryCode;
            this.FromPartyId            = FromPartyId;
            this.ToCountryCode          = ToCountryCode;
            this.ToPartyId              = ToPartyId;

        }

        public OCPIResponse(Int32            StatusCode,
                            String           StatusMessage,
                            String?          AdditionalInformation   = null,
                            DateTime?        Timestamp               = null,

                            HTTPResponse?    HTTPResponse            = null,
                            Request_Id?      RequestId               = null,
                            Correlation_Id?  CorrelationId           = null,
                            URL?             Location                = null,

                            CountryCode?     FromCountryCode         = null,
                            Party_Id?        FromPartyId             = null,
                            CountryCode?     ToCountryCode           = null,
                            Party_Id?        ToPartyId               = null)

        {

            this.StatusCode             = StatusCode;
            this.StatusMessage          = StatusMessage;
            this.AdditionalInformation  = AdditionalInformation;
            this.Timestamp              = Timestamp ?? org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            this.HTTPResponse           = HTTPResponse;
            this.RequestId              = RequestId;
            this.CorrelationId          = CorrelationId;
            this.Location               = Location;

            this.FromCountryCode        = FromCountryCode;
            this.FromPartyId            = FromPartyId;
            this.ToCountryCode          = ToCountryCode;
            this.ToPartyId              = ToPartyId;

        }

        #endregion


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

                           Location.HasValue
                               ? new JProperty("location",                Location.     Value.ToString())
                               : null,

                                 new JProperty("timestamp",               Timestamp.          ToIso8601())

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

                var RemoteRequestId      = HTTPResponse.TryParseHeaderField<Request_Id>    ("X-Request-ID",           Request_Id.    TryParse) ?? RequestId;
                var RemoteCorrelationId  = HTTPResponse.TryParseHeaderField<Correlation_Id>("X-Correlation-ID",       Correlation_Id.TryParse) ?? CorrelationId;
                var location             = HTTPResponse.TryParseHeaderField<URL>           ("Location",               URL.           TryParse);

                var fromPartyId          = HTTPResponse.TryParseHeaderField<Party_Id>      ("OCPI-from-party-id",     Party_Id.      TryParse);
                var fromCountryCode      = HTTPResponse.TryParseHeaderField<CountryCode>   ("OCPI-from-country-code", CountryCode.   TryParse);
                var toPartyId            = HTTPResponse.TryParseHeaderField<Party_Id>      ("OCPI-to-party-id",       Party_Id.      TryParse);
                var toCountryCode        = HTTPResponse.TryParseHeaderField<CountryCode>   ("OCPI-to-country-code",   CountryCode.   TryParse);

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

                    return new OCPIResponse(json["status_code"]?.   Value<Int32>()  ?? -1,
                                            json["status_message"]?.Value<String>() ?? String.Empty,
                                            json["data"]?.          Value<String>(),
                                            timestamp,
                                            HTTPResponse,
                                            RemoteRequestId,
                                            RemoteCorrelationId,
                                            location,

                                            fromCountryCode,
                                            fromPartyId,
                                            toCountryCode,
                                            toPartyId);

                }

                else
                    result = new OCPIResponse(-1,
                                              HTTPResponse.HTTPStatusCode.Code + " - " + HTTPResponse.HTTPStatusCode.Description,
                                              HTTPResponse.EntirePDU,
                                              HTTPResponse.Timestamp,

                                              HTTPResponse,
                                              RemoteRequestId,
                                              RemoteCorrelationId);

            }
            catch (Exception e)
            {

                result = new OCPIResponse(-1,
                                          e.Message,
                                          e.StackTrace,
                                          org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                          HTTPResponse,
                                          RequestId,
                                          CorrelationId);

            }

            result ??= new OCPIResponse(-1,
                                        String.Empty);

            return result;

        }


        public static OCPIResponse Error(String           StatusMessage,
                                         String?          AdditionalInformation   = null,
                                         DateTime?        Timestamp               = null,

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

        public static OCPIResponse Error(Int32            StatusCode,
                                         String           StatusMessage,
                                         String?          AdditionalInformation   = null,
                                         DateTime?        Timestamp               = null,

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

        public static OCPIResponse Exception(Exception        Exception,
                                             DateTime?        Timestamp               = null,

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



        public class Builder
        {

            #region Properties

            public OCPIRequest?          Request                   { get; }

            public JToken?               Data                      { get; set; }
            public Int32?                StatusCode                { get; set; }
            public String?               StatusMessage             { get; set; }

            public String?               AdditionalInformation     { get; set; }
            public DateTime?             Timestamp                 { get; set; }

            public Request_Id?           RequestId                 { get; set; }
            public Correlation_Id?       CorrelationId             { get; set; }
            public URL?                  Location                  { get; set; }

            public HTTPResponse.Builder  HTTPResponseBuilder       { get; set; }

            #endregion

            #region Constructor(s)

            public Builder(OCPIRequest      Request,
                           Int32?           StatusCode              = null,
                           String?          StatusMessage           = null,
                           String?          AdditionalInformation   = null,
                           DateTime?        Timestamp               = null,

                           Request_Id?      RequestId               = null,
                           Correlation_Id?  CorrelationId           = null,
                           URL?             Location                = null)
            {

                this.Request                = Request;
                this.StatusCode             = StatusCode;
                this.StatusMessage          = StatusMessage;
                this.AdditionalInformation  = AdditionalInformation;
                this.Timestamp              = Timestamp;

                this.RequestId              = RequestId;
                this.CorrelationId          = CorrelationId;
                this.Location               = Location;

            }

            #endregion


            public HTTPResponse.Builder ToHTTPResponseBuilder()
            {

                Timestamp                                      ??= org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

                HTTPResponseBuilder.Server                     ??= Request?.HTTPRequest.HTTPServer.DefaultServerName;
                HTTPResponseBuilder.Date                       ??= Timestamp.Value;
                HTTPResponseBuilder.AccessControlAllowOrigin   ??= "*";
                HTTPResponseBuilder.AccessControlAllowHeaders  ??= "Authorization";
                HTTPResponseBuilder.Vary                       ??= "Accept";
                HTTPResponseBuilder.Connection                 ??= "close";

                if (Request is not null)
                {

                    if (Request.HTTPRequest.HTTPMethod != HTTPMethod.OPTIONS)
                    {

                        HTTPResponseBuilder.ContentType = HTTPContentType.JSON_UTF8;

                        if (HTTPResponseBuilder.Content is null)
                            HTTPResponseBuilder.Content = JSONObject.Create(

                                                              Data is not null
                                                                  ? new JProperty("data",                    Data)
                                                                  : null,

                                                              new JProperty("status_code",                   StatusCode ?? 2000),

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

                                                              new JProperty("timestamp",                     Timestamp.    Value.ToIso8601())

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

        public OCPIResponse(TResponse?       Data,

                            Int32            StatusCode,
                            String           StatusMessage,
                            String?          AdditionalInformation   = null,
                            DateTime?        Timestamp               = null,

                            HTTPResponse?    HTTPResponse            = null,
                            Request_Id?      RequestId               = null,
                            Correlation_Id?  CorrelationId           = null,
                            URL?             Location                = null,

                            CountryCode?     FromCountryCode         = null,
                            Party_Id?        FromPartyId             = null,
                            CountryCode?     ToCountryCode           = null,
                            Party_Id?        ToPartyId               = null)

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

        public OCPIResponse(Int32            StatusCode,
                            String           StatusMessage,
                            String?          AdditionalInformation   = null,
                            DateTime?        Timestamp               = null,

                            HTTPResponse?    HTTPResponse            = null,
                            Request_Id?      RequestId               = null,
                            Correlation_Id?  CorrelationId           = null,
                            URL?             Location                = null,

                            CountryCode?     FromCountryCode         = null,
                            Party_Id?        FromPartyId             = null,
                            CountryCode?     ToCountryCode           = null,
                            Party_Id?        ToPartyId               = null)

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


        public new static OCPIResponse<TResponse> Error(String           StatusMessage,
                                                        String?          AdditionalInformation   = null,
                                                        DateTime?        Timestamp               = null,

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

        public new static OCPIResponse<TResponse> Error(Int32            StatusCode,
                                                        String           StatusMessage,
                                                        String?          AdditionalInformation   = null,
                                                        DateTime?        Timestamp               = null,

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

        public new static OCPIResponse<TResponse> Exception(Exception        Exception,
                                                            DateTime?        Timestamp               = null,

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



        public static JObject Create(TResponse                Data,
                                     Func<TResponse, JToken>  Serializer,
                                     Int32                    StatusCode,
                                     String                   StatusMessage,
                                     String?                  AdditionalInformation   = null,
                                     DateTime?                Timestamp               = null,

                                     HTTPResponse?            Response                = null,
                                     Request_Id?              RequestId               = null,
                                     Correlation_Id?          CorrelationId           = null,
                                     URL?                     Location                = null,

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

                           Location.HasValue
                               ? new JProperty("location",                Location.     Value.ToString())
                               : null,

                                 new JProperty("timestamp",               Timestamp.          ToIso8601())

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

                var remoteRequestId      = Response.TryParseHeaderField<Request_Id>    ("X-Request-ID",           Request_Id.    TryParse) ?? RequestId;
                var remoteCorrelationId  = Response.TryParseHeaderField<Correlation_Id>("X-Correlation-ID",       Correlation_Id.TryParse) ?? CorrelationId;
                var remoteLocation       = Response.TryParseHeaderField<URL>           ("Location",               URL.           TryParse);

                var fromCountryCode      = Response.TryParseHeaderField<CountryCode>   ("OCPI-from-country-code", CountryCode.   TryParse);
                var fromPartyId          = Response.TryParseHeaderField<Party_Id>      ("OCPI-from-party-id",     Party_Id.      TryParse);
                var toCountryCode        = Response.TryParseHeaderField<CountryCode>   ("OCPI-to-country-code",   CountryCode.   TryParse);
                var toPartyId            = Response.TryParseHeaderField<Party_Id>      ("OCPI-to-party-id",       Party_Id.      TryParse);

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
                                                                          Response.EntirePDU,
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
                                                                      -1,
                                                                      Response.HTTPStatusCode.Code + " - " + Response.HTTPStatusCode.Description,
                                                                      Response.EntirePDU,
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


        public static OCPIResponse<TResponse> ParseJObject(HTTPResponse              Response,
                                                           Request_Id                RequestId,
                                                           Correlation_Id            CorrelationId,
                                                           Func<JObject, TResponse>  Parser)
        {

            OCPIResponse<TResponse>? result = default;

            try
            {

                var remoteRequestId      = Response.TryParseHeaderField<Request_Id>    ("X-Request-ID",           Request_Id.    TryParse) ?? RequestId;
                var remoteCorrelationId  = Response.TryParseHeaderField<Correlation_Id>("X-Correlation-ID",       Correlation_Id.TryParse) ?? CorrelationId;
                var remoteLocation       = Response.TryParseHeaderField<URL>           ("Location",               URL.           TryParse);

                var fromCountryCode      = Response.TryParseHeaderField<CountryCode>   ("OCPI-from-country-code", CountryCode.   TryParse);
                var fromPartyId          = Response.TryParseHeaderField<Party_Id>      ("OCPI-from-party-id",     Party_Id.      TryParse);
                var toCountryCode        = Response.TryParseHeaderField<CountryCode>   ("OCPI-to-country-code",   CountryCode.   TryParse);
                var toPartyId            = Response.TryParseHeaderField<Party_Id>      ("OCPI-to-party-id",       Party_Id.      TryParse);

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
                                                                 remoteLocation,

                                                                 fromCountryCode,
                                                                 fromPartyId,
                                                                 toCountryCode,
                                                                 toPartyId);

                    }

                    else
                        result = new OCPIResponse<TResponse>(statusCode    ?? 3000,
                                                             statusMessage ?? String.Empty,
                                                             Response.EntirePDU,
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
                    result = new OCPIResponse<TResponse>(-1,
                                                         Response.HTTPStatusCode.Code + " - " + Response.HTTPStatusCode.Description,
                                                         Response.EntirePDU,
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

    }



    public class OCPIResponse<TRequest, TResponse> : OCPIResponse<TResponse>

        where TResponse : class

    {

        #region Properties

        public TRequest?  Request2    { get; }

        #endregion

        #region Constructor(s)

        public OCPIResponse(TRequest         Request,
                            TResponse?       Data,
                            Int32            StatusCode,
                            String           StatusMessage,
                            String?          AdditionalInformation   = null,
                            DateTime?        Timestamp               = null,

                            HTTPResponse?    HTTPResponse            = null,
                            Request_Id?      RequestId               = null,
                            Correlation_Id?  CorrelationId           = null,
                            URL?             Location                = null,

                            CountryCode?     FromCountryCode         = null,
                            Party_Id?        FromPartyId             = null,
                            CountryCode?     ToCountryCode           = null,
                            Party_Id?        ToPartyId               = null)

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



        public static OCPIResponse<TRequest, TResponse> Error(TRequest         Request,
                                                              Int32            StatusCode,
                                                              String           StatusMessage,
                                                              String?          AdditionalInformation   = null,
                                                              DateTime?        Timestamp               = null,

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


        public static OCPIResponse<TRequest, TResponse> Error(TRequest         Request,
                                                              String           StatusMessage,
                                                              String?          AdditionalInformation   = null,
                                                              DateTime?        Timestamp               = null,

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


        public static new OCPIResponse<TRequest, TResponse> Error(Int32            StatusCode,
                                                                  String           StatusMessage,
                                                                  String?          AdditionalInformation   = null,
                                                                  DateTime?        Timestamp               = null,

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


        public static new OCPIResponse<TRequest, TResponse> Error(String           StatusMessage,
                                                                  String?          AdditionalInformation   = null,
                                                                  DateTime?        Timestamp               = null,

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


        public static new OCPIResponse<TRequest, TResponse> Exception(Exception        Exception,
                                                                      DateTime?        Timestamp               = null,

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
                                                                      r.Location,

                                                                      r.FromCountryCode,
                                                                      r.FromPartyId,
                                                                      r.ToCountryCode,
                                                                      r.ToPartyId);

        }


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
                                                         r.Location,

                                                         r.FromCountryCode,
                                                         r.FromPartyId,
                                                         r.ToCountryCode,
                                                         r.ToPartyId);

        }

    }

}
