/*
 * Copyright (c) 2015-2023 GraphDefined GmbH
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

using NUnit.Framework;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_1_1.UnitTests.EMSPTests
{

    [TestFixture]
    public class Credentials_Tests : ANodeTests
    {

        #region EMSP_GetCredentials_RegisteredToken_Test1()

        /// <summary>
        /// EMSP GetCredentials Test (EMSP 1).
        /// </summary>
        [Test]
        public async Task EMSP_GetCredentials_RegisteredToken_Test1()
        {

            var graphDefinedCPO = emsp1CommonAPI?.GetEMSPClient(
                                      CountryCode: CountryCode.Parse("DE"),
                                      PartyId:     Party_Id.   Parse("GEF")
                                  );

            Assert.IsNotNull(graphDefinedCPO);

            if (graphDefinedCPO is not null)
            {

                var response = await graphDefinedCPO.GetCredentials();

                // GET /ocpi/v2.1/v2.1.1/credentials HTTP/1.1
                // Date:                            Sun, 30 Apr 2023 08:03:44 GMT
                // Accept:                          application/json; charset=utf-8;q=1
                // Host:                            localhost:3301
                // Authorization:                   Token emp1-accessing-cso::token
                // X-Request-ID:                    ACjnj252Gvr647rW86KYxWYMCdjv6U
                // X-Correlation-ID:                jM2KMp89jK575MfMf67Kjfr2CM5EW7

                // HTTP/1.1 200 OK
                // Date:                            Sun, 30 Apr 2023 08:03:44 GMT
                // Access-Control-Allow-Methods:    OPTIONS, GET, POST, PUT, DELETE
                // Access-Control-Allow-Headers:    Authorization
                // Server:                          GraphDefined HTTP API
                // Access-Control-Allow-Origin:     *
                // Vary:                            Accept
                // Connection:                      close
                // Content-Type:                    application/json; charset=utf-8
                // Content-Length:                  323
                // X-Request-ID:                    ACjnj252Gvr647rW86KYxWYMCdjv6U
                // X-Correlation-ID:                jM2KMp89jK575MfMf67Kjfr2CM5EW7
                // 
                // {
                //     "data": {
                //         "token":         "emp1-accessing-cso::token",
                //         "url":           "http://127.0.0.1:3301/ocpi/v2.1/versions",
                //         "business_details": {
                //             "name":          "GraphDefined CSO Services",
                //             "website":       "https://www.graphdefined.com/cso"
                //         },
                //         "country_code":  "DE",
                //         "party_id":      "GEF"
                //     },
                //     "status_code":        1000,
                //     "status_message":    "Hello world!",
                //     "timestamp":         "2023-04-30T08:03:44.581Z"
                // }

                Assert.IsNotNull(response);
                Assert.AreEqual (200,            response.HTTPResponse?.HTTPStatusCode.Code);
                Assert.AreEqual (1000,           response.StatusCode);
                Assert.AreEqual ("Hello world!", response.StatusMessage);
                Assert.IsTrue   (Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10));

                Assert.IsNotNull(response.RequestId);
                Assert.IsNotNull(response.CorrelationId);

                var credentials = response.Data;
                Assert.IsNotNull(credentials);

                if (credentials is not null)
                {

                    Assert.AreEqual (emp1_accessing_cso__token,                    credentials.    Token.      ToString());
                    Assert.AreEqual ("http://127.0.0.1:3301/ocpi/v2.1/versions",   credentials.    URL.        ToString());
                    Assert.AreEqual ("DE",                                         credentials.    CountryCode.ToString());
                    Assert.AreEqual ("GEF",                                        credentials.    PartyId.    ToString());

                    var businessDetails = credentials.BusinessDetails;
                    Assert.IsNotNull(businessDetails);
                    Assert.AreEqual ("GraphDefined CSO Services",                  businessDetails.Name);
                    Assert.AreEqual ("https://www.graphdefined.com/cso",           businessDetails.Website.    ToString());

                }

            }

        }

        #endregion

        #region EMSP_GetCredentials_RegisteredToken_Test2()

        /// <summary>
        /// EMSP GetCredentials Test (EMSP 2).
        /// </summary>
        [Test]
        public async Task EMSP_GetCredentials_RegisteredToken_Test2()
        {

            var graphDefinedCPO = emsp2CommonAPI?.GetEMSPClient(
                                      CountryCode: CountryCode.Parse("DE"),
                                      PartyId:     Party_Id.   Parse("GEF")
                                  );

            Assert.IsNotNull(graphDefinedCPO);

            if (graphDefinedCPO is not null)
            {

                var response = await graphDefinedCPO.GetCredentials();

                // GET /ocpi/v2.1/v2.1.1/credentials HTTP/1.1
                // Date:                            Sun, 30 Apr 2023 08:07:42 GMT
                // Accept:                          application/json; charset=utf-8;q=1
                // Host:                            localhost:3301
                // Authorization:                   Token emp2-accessing-cso::token
                // X-Request-ID:                    xt167nt9WCjzKpfp5btn3CG4bzM3dS
                // X-Correlation-ID:                732phS8A686tMb4hE82982ftYbp38S

                // HTTP/1.1 200 OK
                // Date:                            Sun, 30 Apr 2023 08:07:42 GMT
                // Access-Control-Allow-Methods:    OPTIONS, GET, POST, PUT, DELETE
                // Access-Control-Allow-Headers:    Authorization
                // Server:                          GraphDefined HTTP API
                // Access-Control-Allow-Origin:     *
                // Vary:                            Accept
                // Connection:                      close
                // Content-Type:                    application/json; charset=utf-8
                // Content-Length:                  323
                // X-Request-ID:                    xt167nt9WCjzKpfp5btn3CG4bzM3dS
                // X-Correlation-ID:                732phS8A686tMb4hE82982ftYbp38S
                // 
                // {
                //     "data": {
                //         "token":         "emp2-accessing-cso::token",
                //         "url":           "http://127.0.0.1:3301/ocpi/v2.1/versions",
                //         "business_details": {
                //             "name":          "GraphDefined CSO Services",
                //             "website":       "https://www.graphdefined.com/cso"
                //         },
                //         "country_code":  "DE",
                //         "party_id":      "GEF"
                //     },
                //     "status_code":        1000,
                //     "status_message":    "Hello world!",
                //     "timestamp":         "2023-04-30T08:07:42.354Z"
                // }

                Assert.IsNotNull(response);
                Assert.AreEqual (200,            response.HTTPResponse?.HTTPStatusCode.Code);
                Assert.AreEqual (1000,           response.StatusCode);
                Assert.AreEqual ("Hello world!", response.StatusMessage);
                Assert.IsTrue   (Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10));

                Assert.IsNotNull(response.RequestId);
                Assert.IsNotNull(response.CorrelationId);

                var credentials = response.Data;
                Assert.IsNotNull(credentials);

                if (credentials is not null)
                {

                    Assert.AreEqual (emp2_accessing_cso__token,                    credentials.    Token.      ToString());
                    Assert.AreEqual ("http://127.0.0.1:3301/ocpi/v2.1/versions",   credentials.    URL.        ToString());
                    Assert.AreEqual ("DE",                                         credentials.    CountryCode.ToString());
                    Assert.AreEqual ("GEF",                                        credentials.    PartyId.    ToString());

                    var businessDetails = credentials.BusinessDetails;
                    Assert.IsNotNull(businessDetails);
                    Assert.AreEqual ("GraphDefined CSO Services",                  businessDetails.Name);
                    Assert.AreEqual ("https://www.graphdefined.com/cso",           businessDetails.Website.    ToString());

                }

            }

        }

        #endregion


        #region EMSP_DeleteCredentials_RegisteredToken_Test1()

        /// <summary>
        /// EMSP GetCredentials Test (EMSP 1).
        /// </summary>
        [Test]
        public async Task EMSP_DeleteCredentials_RegisteredToken_Test1()
        {

            var graphDefinedCPO = emsp1CommonAPI?.GetEMSPClient(
                                      CountryCode: CountryCode.Parse("DE"),
                                      PartyId:     Party_Id.   Parse("GEF")
                                  );

            Assert.IsNotNull(graphDefinedCPO);

            if (graphDefinedCPO is not null &&
                cpoCommonAPI is not null)
            {

                Func<RemoteParty?> remotePartyAtCPO = () => cpoCommonAPI.RemoteParties.FirstOrDefault(remoteParty => remoteParty.LocalAccessInfos.Any(localAccessInfo => localAccessInfo.AccessToken.ToString() == emp1_accessing_cso__token));

                var remotePartyAtCPO_before  = remotePartyAtCPO();
                Assert.IsNotNull(remotePartyAtCPO_before);

                var response                 = await graphDefinedCPO.DeleteCredentials();

                // DELETE /ocpi/v2.1/v2.1.1/credentials HTTP/1.1
                // Date:                            Fri, 05 May 2023 12:31:07 GMT
                // Accept:                          application/json; charset=utf-8; q=1
                // Host:                            127.0.0.1:3301
                // Authorization:                   Token emp1-accessing-cso::token
                // X-Request-ID:                    1234
                // X-Correlation-ID:                5678

                // HTTP/1.1 200 OK
                // Date:                            Fri, 05 May 2023 12:31:07 GMT
                // Access-Control-Allow-Methods:    OPTIONS, GET, POST, PUT, DELETE
                // Access-Control-Allow-Headers:    Authorization
                // Server:                          GraphDefined HTTP API
                // Access-Control-Allow-Origin:     *
                // Vary:                            Accept
                // Connection:                      close
                // Content-Type:                    application/json; charset=utf-8
                // Content-Length:                  142
                // X-Request-ID:                    1234
                // X-Correlation-ID:                5678
                // 
                // {
                //     "status_code":      1000,
                //     "status_message":  "The given access token 'emp1-accessing-cso::token' was deleted!",
                //     "timestamp":       "2023-05-05T12:31:07.181Z"
                // }

                Assert.IsNotNull(response);
                Assert.AreEqual (200,                                                                 response.HTTPResponse?.HTTPStatusCode.Code);
                Assert.AreEqual (1000,                                                                response.StatusCode);
                Assert.AreEqual ("The given access token 'emp1-accessing-cso::token' was deleted!",   response.StatusMessage);
                Assert.IsTrue   (Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10));

                Assert.IsNotNull(response.RequestId);
                Assert.IsNotNull(response.CorrelationId);


                // Validate, that the remote party was deleted!
                var remotePartyAtCPO_after  = remotePartyAtCPO();
                Assert.IsNull(remotePartyAtCPO_after);

            }

        }

        #endregion


        // HTTP client JSON tests

        #region EMSP_GetCredentials_JSON_NoToken_Test()

        /// <summary>
        /// EMSP GetCredentials JSON without an access token.
        /// </summary>
        [Test]
        public async Task EMSP_GetCredentials_JSON_NoToken_Test()
        {

            if (cpoVersionsAPIURL.HasValue)
            {

                var response1       = await TestHelpers.GetJSONRequest(cpoVersionsAPIURL.Value);
                var versionURL      = URL.Parse(response1.Content["data"]?[0]?["url"]?.Value<String>()!);
                var response2       = await TestHelpers.GetJSONRequest(versionURL);
                var credentialsURL  = URL.Parse(response2.Content["data"]?["endpoints"]?[0]?["url"]?.Value<String>()!);
                var response3       = await TestHelpers.GetJSONRequest(credentialsURL);

                // HTTP/1.1 200 OK
                // Date:                            Sun, 30 Apr 2023 10:02:02 GMT
                // Access-Control-Allow-Methods:    OPTIONS, GET, POST, PUT, DELETE
                // Access-Control-Allow-Headers:    Authorization
                // Server:                          GraphDefined HTTP API
                // Access-Control-Allow-Origin:     *
                // Vary:                            Accept
                // Connection:                      close
                // Content-Type:                    application/json; charset=utf-8
                // Content-Length:                  303
                // X-Request-ID:                    1234
                // X-Correlation-ID:                5678
                // 
                // {
                //     "data": {
                //         "token":         "<any>",
                //         "url":           "http://127.0.0.1:3301/ocpi/v2.1/versions",
                //         "business_details": {
                //             "name":          "GraphDefined CSO Services",
                //             "website":       "https://www.graphdefined.com/cso"
                //         },
                //         "country_code":  "DE",
                //         "party_id":      "GEF"
                //     },
                //     "status_code":        1000,
                //     "status_message":    "Hello world!",
                //     "timestamp":         "2023-04-30T10:02:02.264Z"
                // }

                Assert.IsNotNull(response3);
                Assert.AreEqual (200,            response3.HTTPStatusCode.Code);
                Assert.IsTrue   (Timestamp.Now - response3.Timestamp < TimeSpan.FromSeconds(10));

                var json        = response3.Content;
                Assert.IsNotNull(json);

                Assert.AreEqual ("<any>",                                      json["data"]?["token"]?.                       Value<String>());
                Assert.AreEqual ("http://127.0.0.1:3301/ocpi/v2.1/versions",   json["data"]?["url"]?.                         Value<String>());

                Assert.AreEqual ("GraphDefined CSO Services",                  json["data"]?["business_details"]?["name"]?.   Value<String>());
                Assert.AreEqual ("https://www.graphdefined.com/cso",           json["data"]?["business_details"]?["website"]?.Value<String>());

                Assert.AreEqual ("DE",                                         json["data"]?["country_code"]?.                Value<String>());
                Assert.AreEqual ("GEF",                                        json["data"]?["party_id"]?.                    Value<String>());

                Assert.AreEqual (1000,                                         json["status_code"]?.                          Value<UInt32>());
                Assert.AreEqual ("Hello world!",                               json["status_message"]?.                       Value<String>());

                var timestamp   = json["timestamp"]?.Value<DateTime>();
                Assert.IsNotNull(timestamp);
                Assert.IsTrue(Timestamp.Now - timestamp < TimeSpan.FromSeconds(10));

            }

        }

        #endregion

        #region EMSP_GetCredentials_JSON_UnknownToken_Test()

        /// <summary>
        /// EMSP GetCredentials JSON using an unknown access token.
        /// </summary>
        [Test]
        public async Task EMSP_GetCredentials_JSON_UnknownToken_Test()
        {

            if (cpoVersionsAPIURL.HasValue)
            {

                var response1       = await TestHelpers.GetJSONRequest(cpoVersionsAPIURL.Value);
                var versionURL      = URL.Parse(response1.Content["data"]?[0]?["url"]?.Value<String>()!);
                var response2       = await TestHelpers.GetJSONRequest(versionURL);
                var credentialsURL  = URL.Parse(response2.Content["data"]?["endpoints"]?[0]?["url"]?.Value<String>()!);
                var response3       = await TestHelpers.GetJSONRequest(credentialsURL, UnknownToken);

                // HTTP/1.1 200 OK
                // Date:                            Sun, 30 Apr 2023 10:02:02 GMT
                // Access-Control-Allow-Methods:    OPTIONS, GET, POST, PUT, DELETE
                // Access-Control-Allow-Headers:    Authorization
                // Server:                          GraphDefined HTTP API
                // Access-Control-Allow-Origin:     *
                // Vary:                            Accept
                // Connection:                      close
                // Content-Type:                    application/json; charset=utf-8
                // Content-Length:                  303
                // X-Request-ID:                    1234
                // X-Correlation-ID:                5678
                // 
                // {
                //     "data": {
                //         "token":         "<any>",
                //         "url":           "http://127.0.0.1:3301/ocpi/v2.1/versions",
                //         "business_details": {
                //             "name":          "GraphDefined CSO Services",
                //             "website":       "https://www.graphdefined.com/cso"
                //         },
                //         "country_code":  "DE",
                //         "party_id":      "GEF"
                //     },
                //     "status_code":        1000,
                //     "status_message":    "Hello world!",
                //     "timestamp":         "2023-04-30T10:02:02.264Z"
                // }

                Assert.IsNotNull(response3);
                Assert.AreEqual (200,            response3.HTTPStatusCode.Code);
                Assert.IsTrue   (Timestamp.Now - response3.Timestamp < TimeSpan.FromSeconds(10));

                var json        = response3.Content;
                Assert.IsNotNull(json);

                Assert.AreEqual ("<any>",                                      json["data"]?["token"]?.                       Value<String>());
                Assert.AreEqual ("http://127.0.0.1:3301/ocpi/v2.1/versions",   json["data"]?["url"]?.                         Value<String>());

                Assert.AreEqual ("GraphDefined CSO Services",                  json["data"]?["business_details"]?["name"]?.   Value<String>());
                Assert.AreEqual ("https://www.graphdefined.com/cso",           json["data"]?["business_details"]?["website"]?.Value<String>());

                Assert.AreEqual ("DE",                                         json["data"]?["country_code"]?.                Value<String>());
                Assert.AreEqual ("GEF",                                        json["data"]?["party_id"]?.                    Value<String>());

                Assert.AreEqual (1000,                                         json["status_code"]?.                          Value<UInt32>());
                Assert.AreEqual ("Hello world!",                               json["status_message"]?.                       Value<String>());

                var timestamp   = json["timestamp"]?.Value<DateTime>();
                Assert.IsNotNull(timestamp);
                Assert.IsTrue(Timestamp.Now - timestamp < TimeSpan.FromSeconds(10));

            }

        }

        #endregion

        #region EMSP_GetCredentials_JSON_BlockedToken_Test()

        /// <summary>
        /// EMSP GetCredentials JSON using a blocked access token.
        /// </summary>
        [Test]
        public async Task EMSP_GetCredentials_JSON_BlockedToken_Test()
        {

            if (cpoVersionsAPIURL.HasValue)
            {

                var response1       = await TestHelpers.GetJSONRequest(cpoVersionsAPIURL.Value);
                var versionURL      = URL.Parse(response1.Content["data"]?[0]?["url"]?.Value<String>()!);
                var response2       = await TestHelpers.GetJSONRequest(versionURL);
                var credentialsURL  = URL.Parse(response2.Content["data"]?["endpoints"]?[0]?["url"]?.Value<String>()!);
                var response3       = await TestHelpers.GetJSONRequest(credentialsURL, BlockedEMSPToken);

                // HTTP/1.1 403 Forbidden
                // Date:                            Sun, 30 Apr 2023 10:14:01 GMT
                // Access-Control-Allow-Methods:    OPTIONS, GET
                // Access-Control-Allow-Headers:    Authorization
                // Server:                          GraphDefined HTTP API
                // Access-Control-Allow-Origin:     *
                // Vary:                            Accept
                // Connection:                      close
                // Content-Type:                    application/json; charset=utf-8
                // Content-Length:                  111
                // X-Request-ID:                    1234
                // X-Correlation-ID:                5678
                // 
                // {
                //     "status_code":      2000,
                //     "status_message":  "Invalid or blocked access token!",
                //     "timestamp":       "2023-04-30T10:14:01.041Z"
                // }

                Assert.IsNotNull(response3);
                Assert.AreEqual (403,            response3.HTTPStatusCode.Code);
                Assert.IsTrue   (Timestamp.Now - response3.Timestamp < TimeSpan.FromSeconds(10));

                var json        = response3.Content;
                Assert.IsNotNull(json);

                Assert.AreEqual (2000,                                         json["status_code"]?.                          Value<UInt32>());
                Assert.AreEqual ("Invalid or blocked access token!",           json["status_message"]?.                       Value<String>());

                var timestamp   = json["timestamp"]?.Value<DateTime>();
                Assert.IsNotNull(timestamp);
                Assert.IsTrue   (Timestamp.Now - timestamp < TimeSpan.FromSeconds(10));

            }

        }

        #endregion

        #region EMSP_GetCredentials_JSON_RegisteredToken_Test()

        /// <summary>
        /// EMSP GetCredentials JSON using a registered access token.
        /// </summary>
        [Test]
        public async Task EMSP_GetCredentials_JSON_RegisteredToken_Test()
        {

            if (cpoVersionsAPIURL.HasValue)
            {

                var response1       = await TestHelpers.GetJSONRequest(cpoVersionsAPIURL.Value);
                var versionURL      = URL.Parse(response1.Content["data"]?[0]?["url"]?.Value<String>()!);
                var response2       = await TestHelpers.GetJSONRequest(versionURL);
                var credentialsURL  = URL.Parse(response2.Content["data"]?["endpoints"]?[0]?["url"]?.Value<String>()!);
                var response3       = await TestHelpers.GetJSONRequest(credentialsURL, emp1_accessing_cso__token);

                // HTTP/1.1 200 OK
                // Date:                            Sun, 30 Apr 2023 10:24:16 GMT
                // Access-Control-Allow-Methods:    OPTIONS, GET, POST, PUT, DELETE
                // Access-Control-Allow-Headers:    Authorization
                // Server:                          GraphDefined HTTP API
                // Access-Control-Allow-Origin:     *
                // Vary:                            Accept
                // Connection:                      close
                // Content-Type:                    application/json; charset=utf-8
                // Content-Length:                  323
                // X-Request-ID:                    1234
                // X-Correlation-ID:                5678
                // 
                // {
                //     "data": {
                //         "token":         "emp1-accessing-cso::token",
                //         "url":           "http://127.0.0.1:3301/ocpi/v2.1/versions",
                //         "business_details": {
                //             "name":          "GraphDefined CSO Services",
                //             "website":       "https://www.graphdefined.com/cso"
                //         },
                //         "country_code":  "DE",
                //         "party_id":      "GEF"
                //     },
                //     "status_code":        1000,
                //     "status_message":    "Hello world!",
                //     "timestamp":         "2023-04-30T10:24:16.022Z"
                // }

                Assert.IsNotNull(response3);
                Assert.AreEqual (200,            response3.HTTPStatusCode.Code);
                Assert.IsTrue   (Timestamp.Now - response3.Timestamp < TimeSpan.FromSeconds(10));

                var json        = response3.Content;
                Assert.IsNotNull(json);

                Assert.AreEqual (emp1_accessing_cso__token,                    json["data"]?["token"]?.                       Value<String>());
                Assert.AreEqual ("http://127.0.0.1:3301/ocpi/v2.1/versions",   json["data"]?["url"]?.                         Value<String>());

                Assert.AreEqual ("GraphDefined CSO Services",                  json["data"]?["business_details"]?["name"]?.   Value<String>());
                Assert.AreEqual ("https://www.graphdefined.com/cso",           json["data"]?["business_details"]?["website"]?.Value<String>());

                Assert.AreEqual ("DE",                                         json["data"]?["country_code"]?.                Value<String>());
                Assert.AreEqual ("GEF",                                        json["data"]?["party_id"]?.                    Value<String>());

                Assert.AreEqual (1000,                                         json["status_code"]?.                          Value<UInt32>());
                Assert.AreEqual ("Hello world!",                               json["status_message"]?.                       Value<String>());

                var timestamp   = json["timestamp"]?.Value<DateTime>();
                Assert.IsNotNull(timestamp);
                Assert.IsTrue(Timestamp.Now - timestamp < TimeSpan.FromSeconds(10));

            }

        }

        #endregion


        #region EMSP_OptionsCredentials_JSON_NoToken_Test()

        /// <summary>
        /// EMSP PutCredentials JSON without an access token.
        /// </summary>
        [Test]
        public async Task EMSP_OptionsCredentials_JSON_NoToken_Test()
        {

            if (cpoVersionsAPIURL.HasValue)
            {

                var response1       = await TestHelpers.GetJSONRequest(cpoVersionsAPIURL.Value);
                var versionURL      = URL.Parse(response1.Content["data"]?[0]?["url"]?.Value<String>()!);
                var response2       = await TestHelpers.GetJSONRequest(versionURL);
                var credentialsURL  = URL.Parse(response2.Content["data"]?["endpoints"]?[0]?["url"]?.Value<String>()!);

                var response3       = await TestHelpers.OptionsRequest(credentialsURL);

                // OPTIONS /ocpi/v2.1/v2.1.1/credentials HTTP/1.1
                // Date:                            Mon, 01 May 2023 18:36:39 GMT
                // Host:                            127.0.0.1:3301
                // X-Request-ID:                    1234
                // X-Correlation-ID:                5678

                // HTTP/1.1 200 OK
                // Date: Mon, 01 May 2023 18:36:39 GMT
                // Access-Control-Allow-Methods:    OPTIONS, GET
                // Allow:                           System.Collections.Generic.List`1[org.GraphDefined.Vanaheimr.Hermod.HTTP.HTTPMethod]
                // Access-Control-Allow-Headers:    Authorization
                // Server:                          GraphDefined HTTP API
                // Access-Control-Allow-Origin:     *
                // Vary:                            Accept
                // Connection:                      close
                // X-Request-ID:                    1234
                // X-Correlation-ID:                5678

                Assert.IsNotNull(response3);
                Assert.AreEqual (200,                      response3.HTTPStatusCode.Code);
                Assert.IsTrue   (Timestamp.Now -           response3.Timestamp < TimeSpan.FromSeconds(10));

                Assert.IsNotNull(response3.AccessControlAllowMethods);
                if (response3.AccessControlAllowMethods is not null)
                {
                    Assert.AreEqual (2,                    response3.AccessControlAllowMethods.Count());
                    Assert.IsTrue   (response3.AccessControlAllowMethods?.Contains("OPTIONS"));
                    Assert.IsTrue   (response3.AccessControlAllowMethods?.Contains("GET"));
                }

                Assert.IsNotNull(response3.Allow);
                if (response3.Allow is not null)
                {
                    Assert.AreEqual (2,                    response3.Allow.Count);
                    Assert.Contains (HTTPMethod.OPTIONS,   response3.Allow);
                    Assert.Contains (HTTPMethod.GET,       response3.Allow);
                }

            }

        }

        #endregion


        #region EMSP_PutCredentials_JSON_NoToken_Test()

        /// <summary>
        /// EMSP PutCredentials JSON without an access token.
        /// </summary>
        [Test]
        public async Task EMSP_PutCredentials_JSON_NoToken_Test()
        {

            if (cpoVersionsAPIURL.HasValue)
            {

                var response1       = await TestHelpers.GetJSONRequest(cpoVersionsAPIURL.Value);
                var versionURL      = URL.Parse(response1.Content["data"]?[0]?["url"]?.Value<String>()!);
                var response2       = await TestHelpers.GetJSONRequest(versionURL);
                var credentialsURL  = URL.Parse(response2.Content["data"]?["endpoints"]?[0]?["url"]?.Value<String>()!);

                var response3       = await TestHelpers.PutJSONRequest(URL.Parse(credentialsURL.ToString()),
                                                                       new Credentials(
                                                                           AccessToken.Parse("1234"),
                                                                           URL.Parse("http://localhost/versions"),
                                                                           new BusinessDetails(
                                                                               "GraphDefined",
                                                                               URL.Parse("https://www.graphdefined.com")
                                                                           ),
                                                                           CountryCode.Parse("DE"),
                                                                           Party_Id.   Parse("GEF")
                                                                       ).ToJSON());


                // PUT /ocpi/v2.1/v2.1.1/credentials HTTP/1.1
                // Date:                            Mon, 01 May 2023 04:03:30 GMT
                // Accept:                          application/json; charset=utf-8;q=1
                // Host:                            127.0.0.1:3301
                // Content-Type:                    application/json; charset=utf-8
                // Content-Length:                  171
                // X-Request-ID:                    1234
                // X-Correlation-ID:                5678
                // 
                // {
                //     "token":         "1234",
                //     "url":           "http://localhost/versions",
                //     "business_details": {
                //         "name":      "GraphDefined",
                //         "website":   "https://www.graphdefined.com"
                //     },
                //     "country_code":  "DE",
                //     "party_id":      "GEF"
                // }

                // HTTP/1.1 405 Method Not Allowed
                // Date:                            Mon, 01 May 2023 04:03:51 GMT
                // Access-Control-Allow-Methods:    OPTIONS, GET
                // Access-Control-Allow-Headers:    Authorization
                // Server:                          GraphDefined HTTP API
                // Access-Control-Allow-Origin:     *
                // Vary:                            Accept
                // Connection:                      close
                // Content-Type:                    application/json; charset=utf-8
                // Content-Length:                  151
                // X-Request-ID:                    1234
                // X-Correlation-ID:                5678
                // 
                // {
                //     "status_code":      2000,
                //     "status_message":  "You need to be registered before trying to invoke this protected method!",
                //     "timestamp":       "2023-05-01T04:04:02.642Z"
                // }

                Assert.IsNotNull(response3);
                Assert.AreEqual (405, response3.HTTPStatusCode.Code);
                Assert.AreEqual (2,   response3.AccessControlAllowMethods?.Count());
                Assert.IsTrue   (     response3.AccessControlAllowMethods?.Contains("OPTIONS"));
                Assert.IsTrue   (     response3.AccessControlAllowMethods?.Contains("GET"));
                Assert.IsTrue   (Timestamp.Now - response3.Timestamp < TimeSpan.FromSeconds(10));

                var json        = response3.Content;
                Assert.IsNotNull(json);

                Assert.AreEqual (2000,                                                                         json["status_code"]?.   Value<UInt32>());
                Assert.AreEqual ("You need to be registered before trying to invoke this protected method!",   json["status_message"]?.Value<String>());

                var timestamp   = json["timestamp"]?.Value<DateTime>();
                Assert.IsNotNull(timestamp);
                Assert.IsTrue(Timestamp.Now - timestamp < TimeSpan.FromSeconds(10));

            }

        }

        #endregion

        #region EMSP_PutCredentials_JSON_UnknownToken_Test()

        /// <summary>
        /// EMSP PutCredentials JSON using an unknown access token.
        /// </summary>
        [Test]
        public async Task EMSP_PutCredentials_JSON_UnknownToken_Test()
        {

            if (cpoVersionsAPIURL.HasValue)
            {

                var response1       = await TestHelpers.GetJSONRequest(cpoVersionsAPIURL.Value);
                var versionURL      = URL.Parse(response1.Content["data"]?[0]?["url"]?.Value<String>()!);
                var response2       = await TestHelpers.GetJSONRequest(versionURL);
                var credentialsURL  = URL.Parse(response2.Content["data"]?["endpoints"]?[0]?["url"]?.Value<String>()!);

                var response3       = await TestHelpers.PutJSONRequest(URL.Parse(credentialsURL.ToString()),
                                                                       new Credentials(
                                                                           AccessToken.Parse("1234"),
                                                                           URL.Parse("http://localhost/versions"),
                                                                           new BusinessDetails(
                                                                               "GraphDefined",
                                                                               URL.Parse("https://www.graphdefined.com")
                                                                           ),
                                                                           CountryCode.Parse("DE"),
                                                                           Party_Id.   Parse("GEF")
                                                                       ).ToJSON(),
                                                                       UnknownToken);


                // PUT /ocpi/v2.1/v2.1.1/credentials HTTP/1.1
                // Date:                            Wed, 03 May 2023 21:29:12 GMT
                // Accept:                          application/json; charset=utf-8;q=1
                // Host:                            127.0.0.1:3301
                // Authorization:                   Token UnknownUnknownUnknownToken
                // Content-Type:                    application/json; charset=utf-8
                // Content-Length:                  171
                // X-Request-ID:                    1234
                // X-Correlation-ID:                5678
                // 
                // {
                //     "token":         "1234",
                //     "url":           "http://localhost/versions",
                //     "business_details": {
                //         "name":      "GraphDefined",
                //         "website":   "https://www.graphdefined.com"
                //     },
                //     "country_code":  "DE",
                //     "party_id":      "GEF"
                // }

                // HTTP/1.1 405 Method Not Allowed
                // Date:                            Wed, 03 May 2023 21:29:12 GMT
                // Access-Control-Allow-Methods:    OPTIONS, GET
                // Access-Control-Allow-Headers:    Authorization
                // Server:                          GraphDefined HTTP API
                // Access-Control-Allow-Origin:     *
                // Vary:                            Accept
                // Connection:                      close
                // Content-Type:                    application/json; charset=utf-8
                // Content-Length:                  151
                // X-Request-ID:                    1234
                // X-Correlation-ID:                5678
                // 
                // {
                //     "status_code":      2000,
                //     "status_message":  "You need to be registered before trying to invoke this protected method!",
                //     "timestamp":       "2023-05-03T21:29:12.612Z"}
                // }

                Assert.IsNotNull(response3);
                Assert.AreEqual (405, response3.HTTPStatusCode.Code);
                Assert.AreEqual (2,   response3.AccessControlAllowMethods?.Count());
                Assert.IsTrue   (     response3.AccessControlAllowMethods?.Contains("OPTIONS"));
                Assert.IsTrue   (     response3.AccessControlAllowMethods?.Contains("GET"));
                Assert.IsTrue   (Timestamp.Now - response3.Timestamp < TimeSpan.FromSeconds(10));

                var json        = response3.Content;
                Assert.IsNotNull(json);

                Assert.AreEqual (2000,                                                                         json["status_code"]?.   Value<UInt32>());
                Assert.AreEqual ("You need to be registered before trying to invoke this protected method!",   json["status_message"]?.Value<String>());

                var timestamp   = json["timestamp"]?.Value<DateTime>();
                Assert.IsNotNull(timestamp);
                Assert.IsTrue(Timestamp.Now - timestamp < TimeSpan.FromSeconds(10));

            }

        }

        #endregion

        #region EMSP_PutCredentials_JSON_BlockedToken_Test()

        /// <summary>
        /// EMSP PutCredentials JSON using a blocked access token.
        /// </summary>
        [Test]
        public async Task EMSP_PutCredentials_JSON_BlockedToken_Test()
        {

            if (cpoVersionsAPIURL.HasValue)
            {

                var response1       = await TestHelpers.GetJSONRequest(cpoVersionsAPIURL.Value);
                var versionURL      = URL.Parse(response1.Content["data"]?[0]?["url"]?.Value<String>()!);
                var response2       = await TestHelpers.GetJSONRequest(versionURL);
                var credentialsURL  = URL.Parse(response2.Content["data"]?["endpoints"]?[0]?["url"]?.Value<String>()!);

                var response3       = await TestHelpers.PutJSONRequest(URL.Parse(credentialsURL.ToString()),
                                                                       new Credentials(
                                                                           AccessToken.Parse("1234"),
                                                                           URL.Parse("http://localhost/versions"),
                                                                           new BusinessDetails(
                                                                               "GraphDefined",
                                                                               URL.Parse("https://www.graphdefined.com")
                                                                           ),
                                                                           CountryCode.Parse("DE"),
                                                                           Party_Id.   Parse("GEF")
                                                                       ).ToJSON(),
                                                                       BlockedEMSPToken);


                // PUT /ocpi/v2.1/v2.1.1/credentials HTTP/1.1
                // Date:                            Wed, 03 May 2023 21:29:12 GMT
                // Accept:                          application/json; charset=utf-8;q=1
                // Host:                            127.0.0.1:3301
                // Authorization:                   Token blocked-emsp
                // Content-Type:                    application/json; charset=utf-8
                // Content-Length:                  171
                // X-Request-ID:                    1234
                // X-Correlation-ID:                5678
                // 
                // {
                //     "token":         "1234",
                //     "url":           "http://localhost/versions",
                //     "business_details": {
                //         "name":      "GraphDefined",
                //         "website":   "https://www.graphdefined.com"
                //     },
                //     "country_code":  "DE",
                //     "party_id":      "GEF"
                // }

                // HTTP/1.1 403 Forbidden
                // Date:                            Sun, 30 Apr 2023 10:14:01 GMT
                // Access-Control-Allow-Methods:    OPTIONS, GET
                // Access-Control-Allow-Headers:    Authorization
                // Server:                          GraphDefined HTTP API
                // Access-Control-Allow-Origin:     *
                // Vary:                            Accept
                // Connection:                      close
                // Content-Type:                    application/json; charset=utf-8
                // Content-Length:                  111
                // X-Request-ID:                    1234
                // X-Correlation-ID:                5678
                // 
                // {
                //     "status_code":      2000,
                //     "status_message":  "The given access token 'blocked-emsp' is blocked!",
                //     "timestamp":       "2023-04-30T10:14:01.041Z"
                // }

                Assert.IsNotNull(response3);
                Assert.AreEqual (403, response3.HTTPStatusCode.Code);
                Assert.AreEqual (2,   response3.AccessControlAllowMethods?.Count());
                Assert.IsTrue   (     response3.AccessControlAllowMethods?.Contains("OPTIONS"));
                Assert.IsTrue   (     response3.AccessControlAllowMethods?.Contains("GET"));
                Assert.IsTrue   (Timestamp.Now - response3.Timestamp < TimeSpan.FromSeconds(10));

                var json        = response3.Content;
                Assert.IsNotNull(json);

                Assert.AreEqual (2000,                                                  json["status_code"]?.   Value<UInt32>());
                Assert.AreEqual ("The given access token 'blocked-emsp' is blocked!",   json["status_message"]?.Value<String>());

                var timestamp   = json["timestamp"]?.Value<DateTime>();
                Assert.IsNotNull(timestamp);
                Assert.IsTrue(Timestamp.Now - timestamp < TimeSpan.FromSeconds(10));

            }

        }

        #endregion

        #region EMSP_PutCredentials_JSON_RegisteredToken_Test()

        /// <summary>
        /// EMSP PutCredentials JSON using a registered access token.
        /// </summary>
        [Test]
        public async Task EMSP_PutCredentials_JSON_RegisteredToken_Test()
        {

            if (cpoVersionsAPIURL.HasValue &&
                emsp1HTTPAPI   is not null &&
                cpoCommonAPI   is not null &&
                emsp1CommonAPI is not null)
            {

                var newAccessToken  = AccessToken.Parse("updatedAccessToken");

                var response1       = await TestHelpers.GetJSONRequest(cpoVersionsAPIURL.Value);
                var versionURL      = URL.Parse(response1.Content["data"]?[0]?["url"]?.Value<String>()!);
                var response2       = await TestHelpers.GetJSONRequest(versionURL);
                var credentialsURL  = URL.Parse(response2.Content["data"]?["endpoints"]?[0]?["url"]?.Value<String>()!);

                var response3       = await TestHelpers.PutJSONRequest(URL.Parse(credentialsURL.ToString()),
                                                                       new Credentials(
                                                                           newAccessToken,
                                                                           URL.Parse($"http://localhost:{emsp1HTTPAPI.HTTPServer.IPPorts.First()}/ocpi/v2.1/versions"),
                                                                           new BusinessDetails(
                                                                               "GraphDefined EMSP #1 (NEW)",
                                                                               URL.Parse("https://www.graphdefined.com/emsp/new")
                                                                           ),
                                                                           CountryCode.Parse("DE"),
                                                                           Party_Id.   Parse("GDF")
                                                                       ).ToJSON(),
                                                                       emp1_accessing_cso__token);


                // PUT /ocpi/v2.1/v2.1.1/credentials HTTP/1.1
                // Date:                            Thu, 04 May 2023 04:35:10 GMT
                // Accept:                          application/json; charset=utf-8; q=1
                // Host:                            127.0.0.1:3301
                // Authorization:                   Token emp1-accessing-cso::token
                // Content-Type:                    application/json; charset=utf-8
                // Content-Length:                  223
                // X-Request-ID:                    1234
                // X-Correlation-ID:                5678
                // 
                // {
                //     "token":         "updatedAccessToken",
                //     "url":           "http://localhost:3401/ocpi/v2.1/versions",
                //     "business_details": {
                //         "name":           "GraphDefined EMSP #1 (NEW)",
                //         "website":        "https://www.graphdefined.com/emsp/new"
                //     },
                //     "country_code":  "DE",
                //     "party_id":      "GDF"
                // }

                // HTTP/1.1 200 OK
                // Date:                            Thu, 04 May 2023 04:35:16 GMT
                // Access-Control-Allow-Methods:    OPTIONS, GET, POST, PUT, DELETE
                // Access-Control-Allow-Headers:    Authorization
                // Server:                          GraphDefined HTTP API
                // Access-Control-Allow-Origin:     *
                // Vary:                            Accept
                // Connection:                      close
                // Content-Type:                    application/json; charset=utf-8
                // Content-Length:                  348
                // X-Request-ID:                    1234
                // X-Correlation-ID:                5678
                // 
                // {
                //     "data": {
                //         "token":           "rMGz58G464SS2A9Y76A6G9CrjKEQdMn5pxdh1Wt2d2hMv6Epzx",
                //         "url":             "http://127.0.0.1:3301/ocpi/v2.1/versions",
                //         "business_details": {
                //             "name":            "GraphDefined CSO Services",
                //             "website":         "https://www.graphdefined.com/cso"
                //         },
                //         "country_code":    "DE",
                //         "party_id":        "GEF"
                //     },
                //     "status_code":      1000,
                //     "status_message":  "Hello world!",
                //     "timestamp":       "2023-05-04T04:35:16.712Z"
                // }

                Assert.IsNotNull(response3);
                Assert.AreEqual (200, response3.HTTPStatusCode.Code);
                Assert.AreEqual (5,   response3.AccessControlAllowMethods?.Count());
                Assert.IsTrue   (     response3.AccessControlAllowMethods?.Contains("OPTIONS"));
                Assert.IsTrue   (     response3.AccessControlAllowMethods?.Contains("GET"));
                Assert.IsTrue   (     response3.AccessControlAllowMethods?.Contains("POST"));
                Assert.IsTrue   (     response3.AccessControlAllowMethods?.Contains("PUT"));
                Assert.IsTrue   (     response3.AccessControlAllowMethods?.Contains("DELETE"));
                Assert.IsTrue   (Timestamp.Now - response3.Timestamp < TimeSpan.FromSeconds(10));

                var json        = response3.Content;
                Assert.IsNotNull(json);

                var token       = json["data"]?["token"]?.Value<String>();
                Assert.IsNotNull(token);
                var accessToken = AccessToken.Parse(token!);

                Assert.AreEqual ("http://127.0.0.1:3301/ocpi/v2.1/versions",    json["data"]?["url"]?.                         Value<String>());

                Assert.AreEqual ("GraphDefined CSO Services",                   json["data"]?["business_details"]?["name"]?.   Value<String>());
                Assert.AreEqual ("https://www.graphdefined.com/cso",            json["data"]?["business_details"]?["website"]?.Value<String>());

                Assert.AreEqual ("DE",                                          json["data"]?["country_code"]?.                Value<String>());
                Assert.AreEqual ("GEF",                                         json["data"]?["party_id"]?.                    Value<String>());

                Assert.AreEqual (1000,                                          json["status_code"]?.                          Value<UInt32>());
                Assert.AreEqual ("Hello world!",                                json["status_message"]?.                       Value<String>());

                var timestamp   = json["timestamp"]?.Value<DateTime>();
                Assert.IsNotNull(timestamp);
                Assert.IsTrue(Timestamp.Now - timestamp < TimeSpan.FromSeconds(10));


                // Validate, that the remote party was updated!
                var remotePartyAtCPO   = cpoCommonAPI.RemoteParties.FirstOrDefault(remoteParty => remoteParty.RemoteAccessInfos.Any(remoteAccessInfo => remoteAccessInfo.AccessToken == newAccessToken));
                Assert.IsNotNull(remotePartyAtCPO);

                if (remotePartyAtCPO is not null)
                {
                    Assert.AreEqual ("GraphDefined EMSP #1 (NEW)",              remotePartyAtCPO.BusinessDetails.Name);
                    Assert.AreEqual ("https://www.graphdefined.com/emsp/new",   remotePartyAtCPO.BusinessDetails.Website.ToString());
                    Assert.AreEqual ("DE",                                      remotePartyAtCPO.CountryCode.            ToString());
                    Assert.AreEqual ("GDF",                                     remotePartyAtCPO.PartyId.                ToString());
                    Assert.AreEqual (accessToken,                               remotePartyAtCPO.LocalAccessInfos.FirstOrDefault().AccessToken);
                }

            }

        }

        #endregion

        #region EMSP_PutCredentials_JSON_RegisteredToken_InvalidCountryCode_Test()

        /// <summary>
        /// EMSP PutCredentials JSON using a registered access token and
        /// trying to update the country code.
        /// </summary>
        [Test]
        public async Task EMSP_PutCredentials_JSON_RegisteredToken_InvalidCountryCode_Test()
        {

            if (cpoVersionsAPIURL.HasValue &&
                emsp1HTTPAPI   is not null &&
                cpoCommonAPI   is not null &&
                emsp1CommonAPI is not null)
            {

                var newAccessToken  = AccessToken.Parse("updatedAccessToken");

                var response1       = await TestHelpers.GetJSONRequest(cpoVersionsAPIURL.Value);
                var versionURL      = URL.Parse(response1.Content["data"]?[0]?["url"]?.Value<String>()!);
                var response2       = await TestHelpers.GetJSONRequest(versionURL);
                var credentialsURL  = URL.Parse(response2.Content["data"]?["endpoints"]?[0]?["url"]?.Value<String>()!);

                var response3       = await TestHelpers.PutJSONRequest(URL.Parse(credentialsURL.ToString()),
                                                                       new Credentials(
                                                                           newAccessToken,
                                                                           URL.Parse($"http://localhost:{emsp1HTTPAPI.HTTPServer.IPPorts.First()}/ocpi/v2.1/versions"),
                                                                           new BusinessDetails(
                                                                               "GraphDefined EMSP #1 (NEW)",
                                                                               URL.Parse("https://www.graphdefined.com/emsp/new")
                                                                           ),
                                                                           CountryCode.Parse("FR"),
                                                                           Party_Id.   Parse("GDF")
                                                                       ).ToJSON(),
                                                                       emp1_accessing_cso__token);


                // PUT /ocpi/v2.1/v2.1.1/credentials HTTP/1.1
                // Date:                            Thu, 04 May 2023 05:04:31 GMT
                // Accept:                          application/json; charset=utf-8; q=1
                // Host:                            127.0.0.1:3301
                // Authorization:                   Token emp1-accessing-cso::token
                // Content-Type:                    application/json; charset=utf-8
                // Content-Length:                  223
                // X-Request-ID:                    1234
                // X-Correlation-ID:                5678
                // 
                // {
                //     "token":         "updatedAccessToken",
                //     "url":           "http://localhost:3401/ocpi/v2.1/versions",
                //     "business_details": {
                //         "name":           "GraphDefined EMSP #1 (NEW)",
                //         "website":        "https://www.graphdefined.com/emsp/new"
                //     },
                //     "country_code":  "FR",
                //     "party_id":      "GDF"
                // }

                // HTTP/1.1 400 Bad Request
                // Date:                            Thu, 04 May 2023 05:04:32 GMT
                // Access-Control-Allow-Methods:    OPTIONS, GET, POST, PUT, DELETE
                // Access-Control-Allow-Headers:    Authorization
                // Server:                          GraphDefined HTTP API
                // Access-Control-Allow-Origin:     *
                // Vary:                            Accept
                // Connection:                      close
                // Content-Type:                    application/json; charset=utf-8
                // Content-Length:                  138
                // X-Request-ID:                    1234
                // X-Correlation-ID:                5678
                // 
                // {
                //     "status_code":      2000,
                //     "status_message":  "Updating the country code from 'DE' to 'FR' is not allowed!",
                //     "timestamp":       "2023-05-04T05:04:32.071Z"
                // }

                Assert.IsNotNull(response3);
                Assert.AreEqual (400, response3.HTTPStatusCode.Code);
                Assert.AreEqual (5,   response3.AccessControlAllowMethods?.Count());
                Assert.IsTrue   (     response3.AccessControlAllowMethods?.Contains("OPTIONS"));
                Assert.IsTrue   (     response3.AccessControlAllowMethods?.Contains("GET"));
                Assert.IsTrue   (     response3.AccessControlAllowMethods?.Contains("POST"));
                Assert.IsTrue   (     response3.AccessControlAllowMethods?.Contains("PUT"));
                Assert.IsTrue   (     response3.AccessControlAllowMethods?.Contains("DELETE"));
                Assert.IsTrue   (Timestamp.Now - response3.Timestamp < TimeSpan.FromSeconds(10));

                var json              = response3.Content;
                Assert.IsNotNull(json);

                Assert.AreEqual (2000,                                                            json["status_code"]?.   Value<UInt32>());
                Assert.AreEqual ("Updating the country code from 'DE' to 'FR' is not allowed!",   json["status_message"]?.Value<String>());

                var timestamp         = json["timestamp"]?.Value<DateTime>();
                Assert.IsNotNull(timestamp);
                Assert.IsTrue(Timestamp.Now - timestamp < TimeSpan.FromSeconds(10));


                // Validate, that the remote party was not updated!
                var remotePartyAtCPO  = cpoCommonAPI.RemoteParties.FirstOrDefault(remoteParty => remoteParty.LocalAccessInfos.Any(localAccessInfo => localAccessInfo.AccessToken.ToString() == emp1_accessing_cso__token));
                Assert.IsNotNull(remotePartyAtCPO);

                if (remotePartyAtCPO is not null)
                {
                    Assert.AreEqual ("GraphDefined EMSP #1 Services",                             remotePartyAtCPO.BusinessDetails.Name);
                    Assert.AreEqual ("https://www.graphdefined.com/emsp1",                        remotePartyAtCPO.BusinessDetails.Website.ToString());
                    Assert.AreEqual ("DE",                                                        remotePartyAtCPO.CountryCode.            ToString());
                    Assert.AreEqual ("GDF",                                                       remotePartyAtCPO.PartyId.                ToString());
                }

            }

        }

        #endregion

        #region EMSP_PutCredentials_JSON_RegisteredToken_InvalidPartyId_Test()

        /// <summary>
        /// EMSP PutCredentials JSON using a registered access token and
        /// trying to update the party identification.
        /// </summary>
        [Test]
        public async Task EMSP_PutCredentials_JSON_RegisteredToken_InvalidPartyId_Test()
        {

            if (cpoVersionsAPIURL.HasValue &&
                emsp1HTTPAPI   is not null &&
                cpoCommonAPI   is not null &&
                emsp1CommonAPI is not null)
            {

                var newAccessToken  = AccessToken.Parse("updatedAccessToken");

                var response1       = await TestHelpers.GetJSONRequest(cpoVersionsAPIURL.Value);
                var versionURL      = URL.Parse(response1.Content["data"]?[0]?["url"]?.Value<String>()!);
                var response2       = await TestHelpers.GetJSONRequest(versionURL);
                var credentialsURL  = URL.Parse(response2.Content["data"]?["endpoints"]?[0]?["url"]?.Value<String>()!);

                var response3       = await TestHelpers.PutJSONRequest(URL.Parse(credentialsURL.ToString()),
                                                                       new Credentials(
                                                                           newAccessToken,
                                                                           URL.Parse($"http://localhost:{emsp1HTTPAPI.HTTPServer.IPPorts.First()}/ocpi/v2.1/versions"),
                                                                           new BusinessDetails(
                                                                               "GraphDefined EMSP #1 (NEW)",
                                                                               URL.Parse("https://www.graphdefined.com/emsp/new")
                                                                           ),
                                                                           CountryCode.Parse("DE"),
                                                                           Party_Id.   Parse("GXF")
                                                                       ).ToJSON(),
                                                                       emp1_accessing_cso__token);


                // PUT /ocpi/v2.1/v2.1.1/credentials HTTP/1.1
                // Date:                            Thu, 04 May 2023 05:04:31 GMT
                // Accept:                          application/json; charset=utf-8; q=1
                // Host:                            127.0.0.1:3301
                // Authorization:                   Token emp1-accessing-cso::token
                // Content-Type:                    application/json; charset=utf-8
                // Content-Length:                  223
                // X-Request-ID:                    1234
                // X-Correlation-ID:                5678
                // 
                // {
                //     "token":         "updatedAccessToken",
                //     "url":           "http://localhost:3401/ocpi/v2.1/versions",
                //     "business_details": {
                //         "name":           "GraphDefined EMSP #1 (NEW)",
                //         "website":        "https://www.graphdefined.com/emsp/new"
                //     },
                //     "country_code":  "DE",
                //     "party_id":      "GXF"
                // }

                // HTTP/1.1 400 Bad Request
                // Date:                            Thu, 04 May 2023 05:04:32 GMT
                // Access-Control-Allow-Methods:    OPTIONS, GET, POST, PUT, DELETE
                // Access-Control-Allow-Headers:    Authorization
                // Server:                          GraphDefined HTTP API
                // Access-Control-Allow-Origin:     *
                // Vary:                            Accept
                // Connection:                      close
                // Content-Type:                    application/json; charset=utf-8
                // Content-Length:                  148
                // X-Request-ID:                    1234
                // X-Correlation-ID:                5678
                // 
                // {
                //     "status_code":      2000,
                //     "status_message":  "Updating the party identification from 'GDF' to 'GXF' is not allowed!",
                //     "timestamp":       "2023-05-04T05:04:32.071Z"
                // }

                Assert.IsNotNull(response3);
                Assert.AreEqual (400, response3.HTTPStatusCode.Code);
                Assert.AreEqual (5,   response3.AccessControlAllowMethods?.Count());
                Assert.IsTrue   (     response3.AccessControlAllowMethods?.Contains("OPTIONS"));
                Assert.IsTrue   (     response3.AccessControlAllowMethods?.Contains("GET"));
                Assert.IsTrue   (     response3.AccessControlAllowMethods?.Contains("POST"));
                Assert.IsTrue   (     response3.AccessControlAllowMethods?.Contains("PUT"));
                Assert.IsTrue   (     response3.AccessControlAllowMethods?.Contains("DELETE"));
                Assert.IsTrue   (Timestamp.Now - response3.Timestamp < TimeSpan.FromSeconds(10));

                var json              = response3.Content;
                Assert.IsNotNull(json);

                Assert.AreEqual (2000,                                                                      json["status_code"]?.   Value<UInt32>());
                Assert.AreEqual ("Updating the party identification from 'GDF' to 'GXF' is not allowed!",   json["status_message"]?.Value<String>());

                var timestamp         = json["timestamp"]?.Value<DateTime>();
                Assert.IsNotNull(timestamp);
                Assert.IsTrue(Timestamp.Now - timestamp < TimeSpan.FromSeconds(10));


                // Validate, that the remote party was not updated!
                var remotePartyAtCPO  = cpoCommonAPI.RemoteParties.FirstOrDefault(remoteParty => remoteParty.LocalAccessInfos.Any(localAccessInfo => localAccessInfo.AccessToken.ToString() == emp1_accessing_cso__token));
                Assert.IsNotNull(remotePartyAtCPO);

                if (remotePartyAtCPO is not null)
                {
                    Assert.AreEqual ("GraphDefined EMSP #1 Services",                                       remotePartyAtCPO.BusinessDetails.Name);
                    Assert.AreEqual ("https://www.graphdefined.com/emsp1",                                  remotePartyAtCPO.BusinessDetails.Website.ToString());
                    Assert.AreEqual ("DE",                                                                  remotePartyAtCPO.CountryCode.            ToString());
                    Assert.AreEqual ("GDF",                                                                 remotePartyAtCPO.PartyId.                ToString());
                }

            }

        }

        #endregion

        #region EMSP_PutCredentials_JSON_RegisteredToken_NotFullyRegistered_Test()

        /// <summary>
        /// EMSP PutCredentials JSON using a registered but not yet fully registered access token.
        /// </summary>
        [Test]
        public async Task EMSP_PutCredentials_JSON_RegisteredToken_NotFullyRegistered_Test()
        {

            if (cpoVersionsAPIURL.HasValue &&
                emsp1HTTPAPI   is not null &&
                cpoCommonAPI   is not null &&
                emsp1CommonAPI is not null)
            {

                cpoCommonAPI.RemoveAllRemoteParties();

                cpoCommonAPI.AddRemoteParty(CountryCode:                 emsp1CommonAPI.OurCountryCode,
                                            PartyId:                     emsp1CommonAPI.OurPartyId,
                                            Role:                        Roles.EMSP,
                                            BusinessDetails:             emsp1CommonAPI.OurBusinessDetails,

                                            AccessToken:                 AccessToken.Parse(emp1_accessing_cso__token),
                                            AccessStatus:                AccessStatus.ALLOWED,

                                            //RemoteAccessToken:           AccessToken.Parse("emp1-2-cso:token"),
                                            //RemoteVersionsURL:           null, // 
                                            //RemoteVersionIds:            new[] { Version.Id },
                                            //SelectedVersionId:           Version.Id,
                                            //AccessTokenBase64Encoding:   false,
                                            //RemoteStatus:                RemoteAccessStatus.ONLINE,

                                            PartyStatus:                 PartyStatus.ENABLED);


                var newAccessToken  = AccessToken.Parse("updatedAccessToken");

                var response1       = await TestHelpers.GetJSONRequest(cpoVersionsAPIURL.Value);
                var versionURL      = URL.Parse(response1.Content["data"]?[0]?["url"]?.Value<String>()!);
                var response2       = await TestHelpers.GetJSONRequest(versionURL);
                var credentialsURL  = URL.Parse(response2.Content["data"]?["endpoints"]?[0]?["url"]?.Value<String>()!);

                var response3       = await TestHelpers.PutJSONRequest(URL.Parse(credentialsURL.ToString()),
                                                                       new Credentials(
                                                                           newAccessToken,
                                                                           URL.Parse($"http://localhost:{emsp1HTTPAPI.HTTPServer.IPPorts.First()}/ocpi/v2.1/versions"),
                                                                           new BusinessDetails(
                                                                               "GraphDefined EMSP #1 (NEW)",
                                                                               URL.Parse("https://www.graphdefined.com/emsp/new")
                                                                           ),
                                                                           CountryCode.Parse("DE"),
                                                                           Party_Id.   Parse("GDF")
                                                                       ).ToJSON(),
                                                                       emp1_accessing_cso__token);


                // PUT /ocpi/v2.1/v2.1.1/credentials HTTP/1.1
                // Date:                            Fri, 05 May 2023 10:19:29 GMT
                // Accept:                          application/json; charset=utf-8; q=1
                // Host:                            127.0.0.1:3301
                // Authorization:                   Token emp1-accessing-cso::token
                // Content-Type:                    application/json; charset=utf-8
                // Content-Length:                  223
                // X-Request-ID:                    1234
                // X-Correlation-ID:                5678
                // 
                // {
                //     "token":         "updatedAccessToken",
                //     "url":           "http://localhost:3401/ocpi/v2.1/versions",
                //     "business_details": {
                //         "name":           "GraphDefined EMSP #1 (NEW)",
                //         "website":        "https://www.graphdefined.com/emsp/new"
                //     },
                //     "country_code":  "DE",
                //     "party_id":      "GXF"
                // }

                // HTTP/1.1 405 Method Not Allowed
                // Date:                            Fri, 05 May 2023 10:19:29 GMT
                // Access-Control-Allow-Methods:    OPTIONS, GET, POST
                // Access-Control-Allow-Headers:    Authorization
                // Server:                          GraphDefined HTTP API
                // Access-Control-Allow-Origin:     *
                // Vary:                            Accept
                // Connection:                      close
                // Content-Type:                    application/json; charset=utf-8
                // Content-Length:                  152
                // X-Request-ID:                    1234
                // X-Correlation-ID:                5678
                // 
                // {
                //     "status_code":      2000,
                //     "status_message":  "The given access token 'emp1-accessing-cso::token' is not yet registered!",
                //     "timestamp":       "2023-05-05T10:19:29.735Z"
                // }

                Assert.IsNotNull(response3);
                Assert.AreEqual (405, response3.HTTPStatusCode.Code);
                Assert.AreEqual (3,   response3.AccessControlAllowMethods?.Count());
                Assert.IsTrue   (     response3.AccessControlAllowMethods?.Contains("OPTIONS"));
                Assert.IsTrue   (     response3.AccessControlAllowMethods?.Contains("GET"));
                Assert.IsTrue   (     response3.AccessControlAllowMethods?.Contains("POST"));
                Assert.IsTrue   (Timestamp.Now - response3.Timestamp < TimeSpan.FromSeconds(10));

                var json              = response3.Content;
                Assert.IsNotNull(json);

                Assert.AreEqual (2000,                                                                          json["status_code"]?.   Value<UInt32>());
                Assert.AreEqual ("The given access token 'emp1-accessing-cso::token' is not yet registered!",   json["status_message"]?.Value<String>());

                var timestamp         = json["timestamp"]?.Value<DateTime>();
                Assert.IsNotNull(timestamp);
                Assert.IsTrue(Timestamp.Now - timestamp < TimeSpan.FromSeconds(10));


                // Validate, that the remote party was not updated!
                var remotePartyAtCPO  = cpoCommonAPI.RemoteParties.FirstOrDefault(remoteParty => remoteParty.LocalAccessInfos.Any(localAccessInfo => localAccessInfo.AccessToken.ToString() == emp1_accessing_cso__token));
                Assert.IsNotNull(remotePartyAtCPO);

                if (remotePartyAtCPO is not null)
                {
                    Assert.AreEqual ("GraphDefined EMSP #1 Services",                                           remotePartyAtCPO.BusinessDetails.Name);
                    Assert.AreEqual ("https://www.graphdefined.com/emsp1",                                      remotePartyAtCPO.BusinessDetails.Website.ToString());
                    Assert.AreEqual ("DE",                                                                      remotePartyAtCPO.CountryCode.            ToString());
                    Assert.AreEqual ("GDF",                                                                     remotePartyAtCPO.PartyId.                ToString());
                }

            }

        }

        #endregion


        #region EMSP_DeleteCredentials_JSON_RegisteredToken_Test()

        /// <summary>
        /// EMSP DeleteCredentials JSON using a registered access token.
        /// </summary>
        [Test]
        public async Task EMSP_DeleteCredentials_JSON_RegisteredToken_Test()
        {

            if (cpoVersionsAPIURL.HasValue &&
                emsp1HTTPAPI   is not null &&
                cpoCommonAPI   is not null &&
                emsp1CommonAPI is not null)
            {

                Func<RemoteParty?> remotePartyAtCPO = () => cpoCommonAPI.RemoteParties.FirstOrDefault(remoteParty => remoteParty.LocalAccessInfos.Any(localAccessInfo => localAccessInfo.AccessToken.ToString() == emp1_accessing_cso__token));

                var remotePartyAtCPO_before  = remotePartyAtCPO();
                Assert.IsNotNull(remotePartyAtCPO_before);

                var response1                = await TestHelpers.GetJSONRequest(cpoVersionsAPIURL.Value);
                var versionURL               = URL.Parse(response1.Content["data"]?[0]?["url"]?.Value<String>()!);
                var response2                = await TestHelpers.GetJSONRequest(versionURL);
                var credentialsURL           = URL.Parse(response2.Content["data"]?["endpoints"]?[0]?["url"]?.Value<String>()!);

                var response3                = await TestHelpers.DeleteJSONRequest(URL.Parse(credentialsURL.ToString()),
                                                                                   emp1_accessing_cso__token);


                // DELETE /ocpi/v2.1/v2.1.1/credentials HTTP/1.1
                // Date:                            Fri, 05 May 2023 12:31:07 GMT
                // Accept:                          application/json; charset=utf-8; q=1
                // Host:                            127.0.0.1:3301
                // Authorization:                   Token emp1-accessing-cso::token
                // X-Request-ID:                    1234
                // X-Correlation-ID:                5678

                // HTTP/1.1 200 OK
                // Date:                            Fri, 05 May 2023 12:31:07 GMT
                // Access-Control-Allow-Methods:    OPTIONS, GET, POST, PUT, DELETE
                // Access-Control-Allow-Headers:    Authorization
                // Server:                          GraphDefined HTTP API
                // Access-Control-Allow-Origin:     *
                // Vary:                            Accept
                // Connection:                      close
                // Content-Type:                    application/json; charset=utf-8
                // Content-Length:                  142
                // X-Request-ID:                    1234
                // X-Correlation-ID:                5678
                // 
                // {
                //     "status_code":      1000,
                //     "status_message":  "The given access token 'emp1-accessing-cso::token' was deleted!",
                //     "timestamp":       "2023-05-05T12:31:07.181Z"
                // }

                Assert.IsNotNull(response3);
                Assert.AreEqual (200, response3.HTTPStatusCode.Code);
                Assert.AreEqual (5,   response3.AccessControlAllowMethods?.Count());
                Assert.IsTrue   (     response3.AccessControlAllowMethods?.Contains("OPTIONS"));
                Assert.IsTrue   (     response3.AccessControlAllowMethods?.Contains("GET"));
                Assert.IsTrue   (     response3.AccessControlAllowMethods?.Contains("POST"));
                Assert.IsTrue   (     response3.AccessControlAllowMethods?.Contains("PUT"));
                Assert.IsTrue   (     response3.AccessControlAllowMethods?.Contains("DELETE"));
                Assert.IsTrue   (Timestamp.Now - response3.Timestamp < TimeSpan.FromSeconds(10));

                var json                     = response3.Content;
                Assert.IsNotNull(json);

                Assert.AreEqual (1000,                                                                json["status_code"]?.   Value<UInt32>());
                Assert.AreEqual ("The given access token 'emp1-accessing-cso::token' was deleted!",   json["status_message"]?.Value<String>());

                var timestamp                = json["timestamp"]?.Value<DateTime>();
                Assert.IsNotNull(timestamp);
                Assert.IsTrue(Timestamp.Now - timestamp < TimeSpan.FromSeconds(10));


                // Validate, that the remote party was deleted!
                var remotePartyAtCPO_after   = remotePartyAtCPO();
                Assert.IsNull(remotePartyAtCPO_after);

            }

        }

        #endregion

    }

}
