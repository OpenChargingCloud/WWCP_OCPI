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

using NUnit.Framework;
using NUnit.Framework.Legacy;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2_1.UnitTests.EMSPTests
{

    [TestFixture]
    public class Credentials : ANodeTests
    {

        #region EMSP_GetCredentials_Test1()

        /// <summary>
        /// EMSP GetCredentials Test 1.
        /// </summary>
        [Test]
        public async Task EMSP_GetCredentials_Test1()
        {

            var graphDefinedCPO = emsp1CommonAPI?.GetEMSPClient(
                                      CountryCode: CountryCode.Parse("DE"),
                                      PartyId:     Party_Id.   Parse("GEF")
                                  );

            ClassicAssert.IsNotNull(graphDefinedCPO);

            if (graphDefinedCPO is not null)
            {

                var response = await graphDefinedCPO.GetCredentials();

                // GET /2.2.1/credentials HTTP/1.1
                // Date:                          Mon, 26 Dec 2022 10:29:48 GMT
                // Accept:                        application/json; charset=utf-8;q=1
                // Host:                          127.0.0.1:7234
                // Authorization:                 Token xxxxxx
                // X-Request-ID:                  27ExnKK8fK3vhQvYA8dY7A2rx9KUtC
                // X-Correlation-ID:              325WrvpzKh6f238Mb4Ex17v1612365

                // HTTP/1.1 200 OK
                // Date:                          Mon, 26 Dec 2022 10:29:49 GMT
                // Access-Control-Allow-Methods:  OPTIONS, GET, POST, PUT, DELETE
                // Access-Control-Allow-Headers:  Authorization
                // Server:                        GraphDefined Hermod HTTP Server v1.0
                // Access-Control-Allow-Origin:   *
                // Connection:                    close
                // Content-Type:                  application/json; charset=utf-8
                // Content-Length:                296
                // X-Request-ID:                  27ExnKK8fK3vhQvYA8dY7A2rx9KUtC
                // X-Correlation-ID:              325WrvpzKh6f238Mb4Ex17v1612365
                // 
                // {
                //    "data": {
                //        "token":         "cso-2-emp1:token",
                //        "url":           "http://127.0.0.1:3301/ocpi/v2.2/versions",
                //        "business_details": {
                //            "name":           "GraphDefined CSO Services",
                //            "website":        "https://www.graphdefined.com/cso"
                //        },
                //        "country_code":  "DE",
                //        "party_id":      "GEF"
                //    },
                //    "status_code":      1000,
                //    "status_message":  "Hello world!",
                //    "timestamp":       "2022-12-26T10:29:49.143Z"
                //}

                ClassicAssert.IsNotNull(response);
                ClassicAssert.AreEqual (200,            response.HTTPResponse?.HTTPStatusCode.Code);
                ClassicAssert.AreEqual (1000,           response.StatusCode);
                ClassicAssert.AreEqual ("Hello world!", response.StatusMessage);
                ClassicAssert.IsTrue   (Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10));

                var credentials = response.Data;
                ClassicAssert.IsNotNull(credentials);

                if (credentials is not null)
                {

                    ClassicAssert.AreEqual ("cso-2-emp1:token",                           credentials.    Token.                    ToString());
                    ClassicAssert.AreEqual ("http://127.0.0.1:3301/ocpi/v2.2/versions",   credentials.    URL.                      ToString());
                    ClassicAssert.AreEqual ("DE",                                         credentials.    Roles.First().CountryCode.ToString());
                    ClassicAssert.AreEqual ("GEF",                                        credentials.    Roles.First().PartyId.    ToString());

                    var businessDetails = credentials.Roles.First().BusinessDetails;
                    ClassicAssert.IsNotNull(businessDetails);
                    ClassicAssert.AreEqual ("GraphDefined CSO Services",                  businessDetails.Name);
                    ClassicAssert.AreEqual ("https://www.graphdefined.com/cso",           businessDetails.Website.                  ToString());

                }

            }

        }

        #endregion

        #region EMSP_GetCredentials_Test2()

        /// <summary>
        /// EMSP GetCredentials Test 2.
        /// </summary>
        [Test]
        public async Task EMSP_GetCredentials_Test2()
        {

            var graphDefinedCPO = emsp2CommonAPI?.GetEMSPClient(
                                      CountryCode: CountryCode.Parse("DE"),
                                      PartyId:     Party_Id.   Parse("GEF")
                                  );

            ClassicAssert.IsNotNull(graphDefinedCPO);

            if (graphDefinedCPO is not null)
            {

                var response = await graphDefinedCPO.GetCredentials();

                // GET /2.2.1/credentials HTTP/1.1
                // Date:                          Mon, 26 Dec 2022 10:29:48 GMT
                // Accept:                        application/json; charset=utf-8;q=1
                // Host:                          127.0.0.1:7234
                // Authorization:                 Token xxxxxx
                // X-Request-ID:                  27ExnKK8fK3vhQvYA8dY7A2rx9KUtC
                // X-Correlation-ID:              325WrvpzKh6f238Mb4Ex17v1612365

                // HTTP/1.1 200 OK
                // Date:                          Mon, 26 Dec 2022 10:29:49 GMT
                // Access-Control-Allow-Methods:  OPTIONS, GET, POST, PUT, DELETE
                // Access-Control-Allow-Headers:  Authorization
                // Server:                        GraphDefined Hermod HTTP Server v1.0
                // Access-Control-Allow-Origin:   *
                // Connection:                    close
                // Content-Type:                  application/json; charset=utf-8
                // Content-Length:                296
                // X-Request-ID:                  27ExnKK8fK3vhQvYA8dY7A2rx9KUtC
                // X-Correlation-ID:              325WrvpzKh6f238Mb4Ex17v1612365
                // 
                // {
                //    "data": {
                //        "token":         "xxxxxx",
                //        "url":           "http://127.0.0.1:7234/versions",
                //        "business_details": {
                //            "name":           "GraphDefined CPO Services",
                //            "website":        "https://www.graphdefined.com/cpo"
                //        },
                //        "country_code":  "DE",
                //        "party_id":      "GEF"
                //    },
                //    "status_code":      1000,
                //    "status_message":  "Hello world!",
                //    "timestamp":       "2022-12-26T10:29:49.143Z"
                //}

                ClassicAssert.IsNotNull(response);
                ClassicAssert.AreEqual (200,            response.HTTPResponse?.HTTPStatusCode.Code);
                ClassicAssert.AreEqual (1000,           response.StatusCode);
                ClassicAssert.AreEqual ("Hello world!", response.StatusMessage);
                ClassicAssert.IsTrue   (Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10));

                var credentials = response.Data;
                ClassicAssert.IsNotNull(credentials);

                if (credentials is not null)
                {

                    ClassicAssert.AreEqual ("cso-2-emp2:token",                           credentials.    Token.                    ToString());
                    ClassicAssert.AreEqual ("http://127.0.0.1:3301/ocpi/v2.2/versions",   credentials.    URL.                      ToString());
                    ClassicAssert.AreEqual ("DE",                                         credentials.    Roles.First().CountryCode.ToString());
                    ClassicAssert.AreEqual ("GEF",                                        credentials.    Roles.First().PartyId.    ToString());

                    var businessDetails = credentials.Roles.First().BusinessDetails;
                    ClassicAssert.IsNotNull(businessDetails);
                    ClassicAssert.AreEqual ("GraphDefined CSO Services",                  businessDetails.Name);
                    ClassicAssert.AreEqual ("https://www.graphdefined.com/cso",           businessDetails.Website.                  ToString());

                }

            }

        }

        #endregion


    }

}
