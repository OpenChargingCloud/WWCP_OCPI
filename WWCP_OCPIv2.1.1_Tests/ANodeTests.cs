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

using NUnit.Framework;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Aegir;
using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPIv2_1_1.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_1_1.UnitTests
{

    public static class TestHelpers
    {

        public static async Task<HTTPResponse<JObject>> JSONRequest(URL         RemoteURL,
                                                                    String      Token)
        {

            var httpResponse  = await new HTTPClient(RemoteURL).
                                              Execute(client => client.CreateRequest(HTTPMethod.GET,
                                                                                     RemoteURL.Path,
                                                                                     requestbuilder => {
                                                                                         requestbuilder.Authorization  = new HTTPTokenAuthentication(Token);
                                                                                         requestbuilder.Accept.Add(HTTPContentType.JSON_UTF8);
                                                                                         requestbuilder.Set("X-Request-ID",      "1234");
                                                                                         requestbuilder.Set("X-Correlation-ID",  "5678");
                                                                                     })).
                                              ConfigureAwait(false);

            return new HTTPResponse<JObject>(httpResponse,
                                             JObject.Parse(httpResponse.HTTPBodyAsUTF8String));

        }

        public static async Task<HTTPResponse<JObject>> JSONRequest(HTTPMethod  Method,
                                                                    URL         RemoteURL,
                                                                    String      Token,
                                                                    JObject     JSON)
        {

            var httpResponse  = await new HTTPClient(RemoteURL).
                                              Execute(client => client.CreateRequest(Method,
                                                                                     RemoteURL.Path,
                                                                                     requestbuilder => {
                                                                                         requestbuilder.Authorization  = new HTTPTokenAuthentication(Token);
                                                                                         requestbuilder.ContentType    = HTTPContentType.JSON_UTF8;
                                                                                         requestbuilder.Content        = JSON.ToUTF8Bytes();
                                                                                         requestbuilder.Accept.Add(HTTPContentType.JSON_UTF8);
                                                                                         requestbuilder.Set("X-Request-ID",      "1234");
                                                                                         requestbuilder.Set("X-Correlation-ID",  "5678");
                                                                                     })).
                                              ConfigureAwait(false);

            return new HTTPResponse<JObject>(httpResponse,
                                             JObject.Parse(httpResponse.HTTPBodyAsUTF8String));

        }


    }


    /// <summary>
    /// OCPI v2.1.1 Node test defaults.
    /// </summary>
    public abstract class ANodeTests
    {

        #region Data

        protected          HTTPServer?                                                 cpoHTTPServer;
        protected          HTTPServer?                                                 emspHTTPServer;

        protected          WebAPI.OCPIWebAPI?                                          cpoWebAPI;
        protected          WebAPI.OCPIWebAPI?                                          emspWebAPI;

        public             URL                                                         cpoVersionsAPIURL  = URL.Parse("http://127.0.0.1:7134/versions");
        public             URL                                                         emspVersionsAPIURL = URL.Parse("http://127.0.0.1:7135/versions");

        //protected readonly Dictionary<Operator_Id, HashSet<EVSEDataRecord>>            EVSEDataRecords;
        //protected readonly Dictionary<Operator_Id, HashSet<EVSEStatusRecord>>          EVSEStatusRecords;
        //protected readonly Dictionary<Operator_Id, HashSet<PricingProductDataRecord>>  PricingProductData;
        //protected readonly Dictionary<Operator_Id, HashSet<EVSEPricing>>               EVSEPricings;
        //protected readonly Dictionary<Operator_Id, HashSet<ChargeDetailRecord>>        ChargeDetailRecords;

        #endregion

        #region Constructor(s)

        public ANodeTests()
        {

            //this.EVSEDataRecords      = new Dictionary<Operator_Id, HashSet<EVSEDataRecord>>();
            //this.EVSEStatusRecords    = new Dictionary<Operator_Id, HashSet<EVSEStatusRecord>>();
            //this.PricingProductData   = new Dictionary<Operator_Id, HashSet<PricingProductDataRecord>>();
            //this.EVSEPricings         = new Dictionary<Operator_Id, HashSet<EVSEPricing>>();
            //this.ChargeDetailRecords  = new Dictionary<Operator_Id, HashSet<ChargeDetailRecord>>();

        }

        #endregion


        #region SetupOnce()

        [OneTimeSetUp]
        public void SetupOnce()
        {

        }

        #endregion

        #region SetupEachTest()

        [SetUp]
        public void SetupEachTest()
        {

            Timestamp.Reset();

            cpoHTTPServer   = new HTTPServer(
                                  IPPort.Parse(7134),
                                  Autostart: true
                              );

            emspHTTPServer  = new HTTPServer(
                                  IPPort.Parse(7135),
                                  Autostart: true
                              );

            Assert.IsNotNull(cpoHTTPServer);
            Assert.IsNotNull(emspHTTPServer);


            cpoWebAPI = new WebAPI.OCPIWebAPI(

                                  cpoHTTPServer,

                                  new CommonAPI(

                                      cpoVersionsAPIURL,
                                      new BusinessDetails(
                                          "GraphDefined CPO Services",
                                          URL.Parse("https://www.graphdefined.com/cpo")
                                      ),
                                      CountryCode.Parse("DE"),
                                      Party_Id.   Parse("GEF"),

                                      HTTPServer:            cpoHTTPServer,
                                      HTTPHostname:          HTTPHostname.Any,
                                      Disable_RootServices:  false

                                  ),

                                  RequestTimeout: TimeSpan.FromSeconds(910)

                              );

            emspWebAPI      = new WebAPI.OCPIWebAPI(

                                  emspHTTPServer,

                                  new CommonAPI(

                                      emspVersionsAPIURL,
                                      new BusinessDetails(
                                          "GraphDefined EMSP Services",
                                          URL.Parse("https://www.graphdefined.com/emsp")
                                      ),
                                      CountryCode.Parse("DE"),
                                      Party_Id.   Parse("GDF"),

                                      HTTPServer:            emspHTTPServer,
                                      HTTPHostname:          HTTPHostname.Any,
                                      Disable_RootServices:  false

                                  ),

                                  RequestTimeout: TimeSpan.FromSeconds(10)

                              );

            Assert.IsNotNull(cpoWebAPI);
            Assert.IsNotNull(emspWebAPI);



            //var cpoAPI           = new CPOAPI(
            //                           webAPI.CommonAPI,
            //                           CountryCode.Parse("DE"),
            //                           Party_Id.   Parse("GEF"),
            //                           URLPathPrefix:    HTTPPath.Parse("2.1.1/cpo"),
            //                           //LoggingPath:      Path.Combine(OpenChargingCloudAPIPath, "CPOAPI"),
            //                           AllowDowngrades:  false
            //                       );

            var emspAPI          = new EMSPAPI(
                                       emspWebAPI.CommonAPI,
                                       CountryCode.Parse("DE"),
                                       Party_Id.   Parse("GDF"),
                                       URLPathPrefix:    HTTPPath.Parse("2.1.1/emsp"),
                                       //LoggingPath:      Path.Combine(OpenChargingCloudAPIPath, "CPOAPI"),
                                       AllowDowngrades:  false
                                   );



            var addEMSPResult = cpoWebAPI.CommonAPI.AddRemoteParty(
                CountryCode:        CountryCode.Parse("DE"),
                PartyId:            Party_Id.   Parse("GDF"),
                Role:               Roles.      EMSP,
                BusinessDetails:    new BusinessDetails("GraphDefined EMSP Services"),
                AccessInfoStatus:        new AccessInfoStatus[] {
                                        new AccessInfoStatus(
                                            AccessToken.Parse("xxxxxx"),
                                            AccessStatus.ALLOWED
                                        )
                                    },
                RemoteAccessInfos:  new RemoteAccessInfo[] {
                                        new RemoteAccessInfo(
                                            AccessToken:        AccessToken.Parse("yyyyyy"),
                                            VersionsURL:        emspVersionsAPIURL,
                                            VersionIds:         new Version_Id[] {
                                                                    Version_Id.Parse("2.1.1")
                                                                },
                                            SelectedVersionId:  Version_Id.Parse("2.1.1"),
                                            Status:             RemoteAccessStatus.ONLINE
                                        )
                                    },
                Status:             PartyStatus.ENABLED
            );

            Assert.IsTrue(addEMSPResult);


            var addCPOResult = emspWebAPI.CommonAPI.AddRemoteParty(
                CountryCode:        CountryCode.Parse("DE"),
                PartyId:            Party_Id.   Parse("GEF"),
                Role:               Roles.      CPO,
                BusinessDetails:    new BusinessDetails("GraphDefined CPO Services"),
                AccessInfoStatus:        new AccessInfoStatus[] {
                                        new AccessInfoStatus(
                                            AccessToken.Parse("yyyyyy"),
                                            AccessStatus.ALLOWED
                                        )
                                        //new AccessInfo2(
                                        //    AccessToken.Parse("eeeeee"),
                                        //    AccessStatus.BLOCKED
                                        //)
                                    },
                RemoteAccessInfos:  new RemoteAccessInfo[] {
                                        new RemoteAccessInfo(
                                            AccessToken:        AccessToken.Parse("xxxxxx"),
                                            VersionsURL:        emspVersionsAPIURL,
                                            VersionIds:         new Version_Id[] {
                                                                    Version_Id.Parse("2.1.1")
                                                                },
                                            SelectedVersionId:  Version_Id.Parse("2.1.1"),
                                            Status:             RemoteAccessStatus.ONLINE
                                        )
                                    },
                Status:             PartyStatus.ENABLED
            );

            Assert.IsTrue(addCPOResult);

        }

        #endregion

        #region ShutdownEachTest()

        [TearDown]
        public void ShutdownEachTest()
        {

            cpoWebAPI?.     Shutdown();
            cpoHTTPServer?. Shutdown();

            emspWebAPI?.    Shutdown();
            emspHTTPServer?.Shutdown();

        }

        #endregion

        #region ShutdownOnce()

        [OneTimeTearDown]
        public void ShutdownOnce()
        {

        }

        #endregion


    }

}
