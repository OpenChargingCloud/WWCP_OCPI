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

    public class OCPIResponse<TResponse>
    {

        #region Properties

        public TResponse  Data                      { get; }

        public Int32?     StatusCode                { get; }
        public String     StatusMessage             { get; }

        public String     AdditionalInformation     { get; }
        public DateTime   Timestamp                 { get; }

        #endregion

        #region Constructor(s)

        public OCPIResponse(TResponse  Data,
                            Int32?     StatusCode,
                            String     StatusMessage,
                            String     AdditionalInformation   = null,
                            DateTime?  Timestamp               = null)
        {

            this.Data                   = Data;
            this.StatusCode             = StatusCode;
            this.StatusMessage          = StatusMessage;
            this.AdditionalInformation  = AdditionalInformation;
            this.Timestamp              = Timestamp ?? DateTime.UtcNow;

        }


        public static JObject Create(TResponse                Data,
                                     Func<TResponse, JToken>  Serializer,
                                     Int32?                   StatusCode,
                                     String                   StatusMessage,
                                     String                   AdditionalInformation   = null,
                                     DateTime?                Timestamp               = null)
        {

            return new OCPIResponse<TResponse>(Data,
                                               StatusCode,
                                               StatusMessage,
                                               AdditionalInformation,
                                               Timestamp).ToJSON(Serializer);

        }

        #endregion



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
                                                                       Func<JObject, TResponse>  Parser)
        {

            OCPIResponse<IEnumerable<TResponse>> result = default;

            try
            {

                if (Response.HTTPStatusCode == HTTPStatusCode.OK)
                {

                    var JSON  = JObject.Parse(Response.HTTPBody?.ToUTF8String());
                    var Items = new List<TResponse>();

                    if (JSON["data"] is JArray JSONArray)
                    {
                        foreach (JObject item in JSONArray)
                        {
                            Items.Add(Parser(item));
                        }
                    }

                    result = new OCPIResponse<IEnumerable<TResponse>>(Items,
                                                                      1000,
                                                                      String.Empty,
                                                                      String.Empty);

                }

                else
                    result = new OCPIResponse<IEnumerable<TResponse>>(default,
                                                                      -1,
                                                                      Response.HTTPStatusCode.Code + " - " + Response.HTTPStatusCode.Description,
                                                                      Response.EntirePDU);

            }
            catch (Exception e)
            {

                result = new OCPIResponse<IEnumerable<TResponse>>(default,
                                                                  -1,
                                                                  e.Message,
                                                                  e.StackTrace);

            }

            return result;

        }


        public static OCPIResponse<TResponse> ParseJObject(HTTPResponse              Response,
                                                           Func<JObject, TResponse>  Parser)
        {

            OCPIResponse<TResponse> result = default;

            try
            {

                if (Response.HTTPStatusCode == HTTPStatusCode.OK)
                {

                    var JSON  = JObject.Parse(Response.HTTPBody?.ToUTF8String());

                    if (JSON["data"] is JObject JSONObject)
                        result = new OCPIResponse<TResponse>(Parser(JSONObject),
                                                             1000,
                                                             String.Empty,
                                                             String.Empty);

                }

                else
                    result = new OCPIResponse<TResponse>(default,
                                                         -1,
                                                         Response.HTTPStatusCode.Code + " - " + Response.HTTPStatusCode.Description,
                                                         Response.EntirePDU);

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

        public OCPIResponse(TRequest   Request,
                            TResponse  Data,
                            Int32?     StatusCode,
                            String     StatusMessage,
                            String     AdditionalInformation,
                            DateTime?  Timestamp = null)

            : base(Data,
                   StatusCode,
                   StatusMessage,
                   AdditionalInformation,
                   Timestamp)

        {

            this.Request  = Request;

        }

        #endregion



        public static OCPIResponse<TRequest, IEnumerable<TResponse>> ParseJArray(TRequest                  Request,
                                                                                 HTTPResponse              Response,
                                                                                 Func<JObject, TResponse>  Parser)
        {

            OCPIResponse<TRequest, IEnumerable<TResponse>> result = default;

            try
            {

                if (Response.HTTPStatusCode == HTTPStatusCode.OK)
                {

                    var JSON  = JObject.Parse(Response.HTTPBody?.ToUTF8String());
                    var Items = new List<TResponse>();

                    if (JSON["data"] is JArray JSONArray)
                    {
                        foreach (JObject item in JSONArray)
                        {
                            Items.Add(Parser(item));
                        }
                    }

                    result = new OCPIResponse<TRequest, IEnumerable<TResponse>>(Request,
                                                                                Items,
                                                                                1000,
                                                                                String.Empty,
                                                                                String.Empty);

                }

                else
                    result = new OCPIResponse<TRequest, IEnumerable<TResponse>>(Request,
                                                                                default,
                                                                                -1,
                                                                                Response.HTTPStatusCode.Code + " - " + Response.HTTPStatusCode.Description,
                                                                                Response.EntirePDU);

            }
            catch (Exception e)
            {

                result = new OCPIResponse<TRequest, IEnumerable<TResponse>>(Request,
                                                                            default,
                                                                            -1,
                                                                            e.Message,
                                                                            e.StackTrace);

            }

            return result;

        }


        public static OCPIResponse<TRequest, TResponse> ParseJObject(TRequest                  Request,
                                                                     HTTPResponse              Response,
                                                                     Func<JObject, TResponse>  Parser)
        {

            OCPIResponse<TRequest, TResponse> result = default;

            try
            {

                if (Response.HTTPStatusCode == HTTPStatusCode.OK)
                {

                    var JSON  = JObject.Parse(Response.HTTPBody?.ToUTF8String());

                    if (JSON["data"] is JObject JSONObject)
                        result = new OCPIResponse<TRequest, TResponse>(Request,
                                                                       Parser(JSONObject),
                                                                       1000,
                                                                       String.Empty,
                                                                       String.Empty);

                }

                else
                    result = new OCPIResponse<TRequest, TResponse>(Request,
                                                                   default,
                                                                   -1,
                                                                   Response.HTTPStatusCode.Code + " - " + Response.HTTPStatusCode.Description,
                                                                   Response.EntirePDU);

            }
            catch (Exception e)
            {

                result = new OCPIResponse<TRequest, TResponse>(Request,
                                                               default,
                                                               -1,
                                                               e.Message,
                                                               e.StackTrace);

            }

            return result;

        }



    }

}
