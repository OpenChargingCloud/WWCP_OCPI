/*
 * Copyright (c) 2015-2020 GraphDefined GmbH
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

using System;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.DNS;

using org.GraphDefined.WWCP;

using cloud.charging.open.protocols.OCPIv2_2.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2
{

    public class OCPIResponse
    {

        #region Properties

        public OCPIRequest      Request                   { get; }

        public Int32?           StatusCode                { get; }
        public String           StatusMessage             { get; }

        public String           AdditionalInformation     { get; }
        public DateTime         Timestamp                 { get; }


        public Request_Id?      RequestId                 { get; }
        public Correlation_Id?  CorrelationId             { get; }

        public HTTPResponse     HTTPResponse              { get; }

        #endregion

        #region Constructor(s)

        public OCPIResponse(OCPIRequest   Request,

                            Int32?        StatusCode,
                            String        StatusMessage,
                            String        AdditionalInformation   = null,
                            DateTime?     Timestamp               = null,

                            HTTPResponse  HTTPResponse            = null)

        {

            this.Request                = Request;

            this.StatusCode             = StatusCode;
            this.StatusMessage          = StatusMessage;
            this.AdditionalInformation  = AdditionalInformation;
            this.Timestamp              = Timestamp ?? DateTime.UtcNow;

            this.RequestId              = RequestId;
            this.CorrelationId          = CorrelationId;
            this.HTTPResponse           = HTTPResponse;

        }



        public OCPIResponse(Int32?           StatusCode,
                            String           StatusMessage,
                            String           AdditionalInformation   = null,
                            DateTime?        Timestamp               = null,

                            Request_Id?      RequestId               = null,
                            Correlation_Id?  CorrelationId           = null,
                            HTTPResponse     Response                = null)

        {

            this.StatusCode             = StatusCode;
            this.StatusMessage          = StatusMessage;
            this.AdditionalInformation  = AdditionalInformation;
            this.Timestamp              = Timestamp ?? DateTime.UtcNow;

            this.RequestId              = RequestId;
            this.CorrelationId          = CorrelationId;
            this.HTTPResponse           = Response;

        }

        #endregion


        public static JObject Create(Int32?           StatusCode,
                                     String           StatusMessage,
                                     String           AdditionalInformation   = null,
                                     DateTime?        Timestamp               = null,

                                     Request_Id?      RequestId               = null,
                                     Correlation_Id?  CorrelationId           = null,
                                     HTTPResponse     Response                = null)

        {

            return new OCPIResponse(StatusCode,
                                    StatusMessage,
                                    AdditionalInformation,
                                    Timestamp,
                                    RequestId,
                                    CorrelationId,
                                    Response).ToJSON();

        }


        //public HTTPResponse.Builder CreateHTTPResonse(HTTPRequest Request)
        //{
        //    return new HTTPResponse.Builder(Request);
        //}


        public JObject ToJSON()
        {

            var JSON = JSONObject.Create(

                           new JProperty("status_code",           StatusCode),

                           StatusMessage.IsNotNullOrEmpty()
                               ? new JProperty("status_message",  StatusMessage)
                               :  null,

                           new JProperty("timestamp",             Timestamp.ToIso8601())

                       );

            return JSON;

        }

        public static JObject ToJSON(Int32            StatusCode,
                                     String           StatusMessage           = null,
                                     JToken           Data                    = null,
                                     String           AdditionalInformation   = null,
                                     DateTime?        Timestamp               = null)
        {

            var JSON = JSONObject.Create(

                           Data != null
                               ? new JProperty("data",            Data)
                               : null,

                           new JProperty("status_code",           StatusCode),

                           StatusMessage.IsNotNullOrEmpty()
                               ? new JProperty("status_message",  StatusMessage)
                               :  null,

                           new JProperty("timestamp",             (Timestamp ?? DateTime.UtcNow).ToIso8601())

                       );

            return JSON;

        }



        public class Builder
        {

            #region Properties

            public OCPIRequest           Request                   { get; }

            public JToken                Data                      { get; set; }
            public Int32?                StatusCode                { get; set; }
            public String                StatusMessage             { get; set; }

            public String                AdditionalInformation     { get; set; }
            public DateTime?             Timestamp                 { get; set; }

            public HTTPResponse.Builder  HTTPResponseBuilder       { get; set; }

            #endregion

            #region Constructor(s)

            public Builder(OCPIRequest Request)
            {
                this.Request = Request;
            }

            #endregion


            public HTTPResponse.Builder UpdateHTTPResponseBuilder()
            {

                if (!Timestamp.HasValue)
                    Timestamp = DateTime.UtcNow;

                HTTPResponseBuilder.Server                    = Request.HTTPRequest.HTTPServer.DefaultServerName;
                HTTPResponseBuilder.Date                      = Timestamp.Value;
                HTTPResponseBuilder.AccessControlAllowOrigin  = "*";
                HTTPResponseBuilder.Connection                = "close";

                if (Request.HTTPRequest.HTTPMethod != HTTPMethod.OPTIONS)
                {

                    HTTPResponseBuilder.ContentType = HTTPContentType.JSON_UTF8;

                    if (HTTPResponseBuilder.Content == null)
                        HTTPResponseBuilder.Content = ToJSON(
                                                          StatusCode ?? 2000,
                                                          StatusMessage,
                                                          Data,
                                                          AdditionalInformation,
                                                          Timestamp
                                                      ).ToUTF8Bytes();

                }

                HTTPResponseBuilder.Set("X-Request-ID",      Request.RequestId).
                                    Set("X-Correlation-ID",  Request.CorrelationId);

                return HTTPResponseBuilder;

            }


            public OCPIResponse ToImmutable

                => new OCPIResponse(Request,
                                    StatusCode,
                                    StatusMessage,
                                    AdditionalInformation,
                                    Timestamp ?? DateTime.UtcNow,
                                    UpdateHTTPResponseBuilder().AsImmutable);


        }


    }



    public class OCPIResponse<TResponse> : OCPIResponse
    {

        #region Properties

        public TResponse        Data                      { get; }

        #endregion

        #region Constructor(s)

        public OCPIResponse(TResponse        Data,
                            Int32?           StatusCode,
                            String           StatusMessage,
                            String           AdditionalInformation   = null,
                            DateTime?        Timestamp               = null,

                            Request_Id?      RequestId               = null,
                            Correlation_Id?  CorrelationId           = null,
                            HTTPResponse     Response                = null)

            : base(StatusCode,
                   StatusMessage,
                   AdditionalInformation,
                   Timestamp,
                   RequestId,
                   CorrelationId,
                   Response)

        {

            this.Data  = Data;

        }

        #endregion


        public static JObject Create(TResponse                Data,
                                     Func<TResponse, JToken>  Serializer,
                                     Int32?                   StatusCode,
                                     String                   StatusMessage,
                                     String                   AdditionalInformation   = null,
                                     DateTime?                Timestamp               = null,

                                     Request_Id?              RequestId               = null,
                                     Correlation_Id?          CorrelationId           = null,
                                     HTTPResponse             Response                = null)

        {

            return new OCPIResponse<TResponse>(Data,
                                               StatusCode,
                                               StatusMessage,
                                               AdditionalInformation,
                                               Timestamp,
                                               RequestId,
                                               CorrelationId,
                                               Response).ToJSON(Serializer);

        }


        //public HTTPResponse.Builder CreateHTTPResonse(HTTPRequest Request)
        //{
        //    return new HTTPResponse.Builder(Request);
        //}



        public JObject ToJSON(Func<TResponse, JToken> Serializer = null)

        {

            var JSON = JSONObject.Create(

                           new JProperty("data",                  Serializer?.Invoke(Data)),
                           new JProperty("status_code",           StatusCode),

                           StatusMessage.IsNotNullOrEmpty()
                               ? new JProperty("status_message",  StatusMessage)
                               :  null,

                           new JProperty("timestamp",             Timestamp.ToIso8601())

                       );

            return JSON;

        }


        public static OCPIResponse<IEnumerable<TResponse>> ParseJArray(HTTPResponse              Response,
                                                                       Request_Id                RequestId,
                                                                       Correlation_Id            CorrelationId,
                                                                       Func<JObject, TResponse>  Parser)
        {

            OCPIResponse<IEnumerable<TResponse>> result = default;

            try
            {

                var RemoteRequestId      = Response.TryParseHeaderField<Request_Id>    ("X-Request-ID",     Request_Id.    TryParse) ?? RequestId;
                var RemoteCorrelationId  = Response.TryParseHeaderField<Correlation_Id>("X-Correlation-ID", Correlation_Id.TryParse) ?? CorrelationId;

                if (Response.HTTPBody.Length > 0)
                {

                    var JSON           = JObject.Parse(Response.HTTPBody?.ToUTF8String());

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

                    var statusCode     = JSON["status_code"].    Value<Int32>();
                    var statusMessage  = JSON["status_message"]?.Value<String>();
                    var timestamp      = JSON["timestamp"]?.     Value<DateTime>();
                    if (timestamp.HasValue && timestamp.Value.Kind != DateTimeKind.Utc)
                        timestamp      = timestamp.Value.ToUniversalTime();

                    if (Response.HTTPStatusCode == HTTPStatusCode.OK)
                    {

                        var Items          = new List<TResponse>();
                        var exceptions     = new List<String>();

                        if (JSON["data"] is JArray JSONArray)
                        {
                            foreach (JObject item in JSONArray)
                            {
                                try
                                {
                                    Items.Add(Parser(item));
                                }
                                catch (Exception e)
                                {
                                    exceptions.Add(e.Message);
                                }
                            }
                        }

                        result = new OCPIResponse<IEnumerable<TResponse>>(Items,
                                                                          statusCode,
                                                                          statusMessage,
                                                                          exceptions.Any() ? exceptions.AggregateWith(Environment.NewLine) : null,
                                                                          timestamp ?? DateTime.UtcNow,
                                                                          RemoteRequestId,
                                                                          RemoteCorrelationId,
                                                                          Response);

                    }

                    else
                        result = new OCPIResponse<IEnumerable<TResponse>>(new TResponse[0],
                                                                          statusCode,
                                                                          statusMessage,
                                                                          Response.EntirePDU,
                                                                          timestamp ?? DateTime.UtcNow,
                                                                          RemoteRequestId,
                                                                          RemoteCorrelationId,
                                                                          Response);

                }

                else
                    result = new OCPIResponse<IEnumerable<TResponse>>(new TResponse[0],
                                                                      -1,
                                                                      Response.HTTPStatusCode.Code + " - " + Response.HTTPStatusCode.Description,
                                                                      Response.EntirePDU,
                                                                      Response.Timestamp,
                                                                      RemoteRequestId,
                                                                      RemoteCorrelationId,
                                                                      Response);

            }
            catch (Exception e)
            {

                result = new OCPIResponse<IEnumerable<TResponse>>(new TResponse[0],
                                                                  -1,
                                                                  e.Message,
                                                                  e.StackTrace,
                                                                  DateTime.UtcNow,
                                                                  RequestId,
                                                                  CorrelationId,
                                                                  Response);

            }

            return result;

        }


        public static OCPIResponse<TResponse> ParseJObject(HTTPResponse              Response,
                                                           Request_Id                RequestId,
                                                           Correlation_Id            CorrelationId,
                                                           Func<JObject, TResponse>  Parser)
        {

            OCPIResponse<TResponse> result = default;

            try
            {

                var RemoteRequestId      = Response.TryParseHeaderField<Request_Id>    ("X-Request-ID",     Request_Id.    TryParse) ?? RequestId;
                var RemoteCorrelationId  = Response.TryParseHeaderField<Correlation_Id>("X-Correlation-ID", Correlation_Id.TryParse) ?? CorrelationId;

                if (Response.HTTPBody.Length > 0)
                {

                    var JSON           = JObject.Parse(Response.HTTPBody?.ToUTF8String());

                    var statusCode     = JSON["status_code"].    Value<Int32>();
                    var statusMessage  = JSON["status_message"]?.Value<String>();
                    var timestamp      = JSON["timestamp"]?.     Value<DateTime>();
                    if (timestamp.HasValue && timestamp.Value.Kind != DateTimeKind.Utc)
                        timestamp      = timestamp.Value.ToUniversalTime();

                    if (Response.HTTPStatusCode == HTTPStatusCode.OK)
                    {

                        if (JSON["data"] is JObject JSONObject)
                            result = new OCPIResponse<TResponse>(Parser(JSONObject),
                                                                 statusCode,
                                                                 statusMessage,
                                                                 null,
                                                                 timestamp,
                                                                 RemoteRequestId,
                                                                 RemoteCorrelationId,
                                                                 Response);

                    }

                    else
                        result = new OCPIResponse<TResponse>(default,
                                                             statusCode,
                                                             statusMessage,
                                                             Response.EntirePDU,
                                                             timestamp,
                                                             RemoteRequestId,
                                                             RemoteCorrelationId,
                                                             Response);

                }

                else
                    result = new OCPIResponse<TResponse>(default,
                                                         -1,
                                                         Response.HTTPStatusCode.Code + " - " + Response.HTTPStatusCode.Description,
                                                         Response.EntirePDU,
                                                         Response.Timestamp,
                                                         RemoteRequestId,
                                                         RemoteCorrelationId,
                                                         Response);

            }
            catch (Exception e)
            {

                result = new OCPIResponse<TResponse>(default,
                                                     -1,
                                                     e.Message,
                                                     e.StackTrace);

            }

            return result;

        }

    }



    public class OCPIResponse<TRequest, TResponse> : OCPIResponse<TResponse>
    {

        #region Data


        #endregion

        #region Properties

        public TRequest   Request                   { get; }

        #endregion

        #region Constructor(s)

        public OCPIResponse(TRequest         Request,
                            TResponse        Data,
                            Int32?           StatusCode,
                            String           StatusMessage,
                            String           AdditionalInformation   = null,
                            DateTime?        Timestamp               = null,

                            Request_Id?      RequestId               = null,
                            Correlation_Id?  CorrelationId           = null)

            : base(Data,
                   StatusCode,
                   StatusMessage,
                   AdditionalInformation,
                   Timestamp,

                   RequestId,
                   CorrelationId)

        {

            this.Request  = Request;

        }

        #endregion



        public static OCPIResponse<TRequest, IEnumerable<TResponse>> ParseJArray(TRequest                  Request,
                                                                                 Request_Id                RequestId,
                                                                                 Correlation_Id            CorrelationId,
                                                                                 HTTPResponse              Response,
                                                                                 Func<JObject, TResponse>  Parser)
        {

            var r = OCPIResponse <TResponse>.ParseJArray(Response,
                                                         RequestId,
                                                         CorrelationId,
                                                         Parser);

            return new OCPIResponse<TRequest, IEnumerable<TResponse>>(Request,
                                                                      r.Data,
                                                                      r.StatusCode,
                                                                      r.StatusMessage,
                                                                      r.AdditionalInformation,
                                                                      r.Timestamp,
                                                                      r.RequestId,
                                                                      r.CorrelationId);

        }


        public static OCPIResponse<TRequest, TResponse> ParseJObject(TRequest                  Request,
                                                                     Request_Id                RequestId,
                                                                     Correlation_Id            CorrelationId,
                                                                     HTTPResponse              Response,
                                                                     Func<JObject, TResponse>  Parser)
        {

            var r = OCPIResponse<TResponse>.ParseJObject(Response,
                                                         RequestId,
                                                         CorrelationId,
                                                         Parser);

            return new OCPIResponse<TRequest, TResponse>(Request,
                                                         r.Data,
                                                         r.StatusCode,
                                                         r.StatusMessage,
                                                         r.AdditionalInformation,
                                                         r.Timestamp,
                                                         r.RequestId,
                                                         r.CorrelationId);

        }


    }

}
