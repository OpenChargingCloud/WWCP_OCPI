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
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPIv2_1_1.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_1_1.UnitTests
{

    public static class TestHelpers
    {

        public static async Task<HTTPResponse<JObject>> JSONRequest(URL     RemoteURL,
                                                                    String  Token)
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
        protected          HTTPServer?                                                 emspHTTPServer1;
        protected          HTTPServer?                                                 emspHTTPServer2;

        protected          WebAPI.OCPIWebAPI?                                          cpoWebAPI;
        protected          WebAPI.OCPIWebAPI?                                          emspWebAPI1;
        protected          WebAPI.OCPIWebAPI?                                          emspWebAPI2;

        public             URL                                                         cpoVersionsAPIURL   = URL.Parse("http://127.0.0.1:7134/versions");
        public             URL                                                         emspVersionsAPIURL1 = URL.Parse("http://127.0.0.1:7135/versions");
        public             URL                                                         emspVersionsAPIURL2 = URL.Parse("http://127.0.0.1:7136/versions");

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

            cpoHTTPServer    = new HTTPServer(
                                   IPPort.Parse(7134),
                                   Autostart: true
                               );

            emspHTTPServer1  = new HTTPServer(
                                   IPPort.Parse(7135),
                                   Autostart: true
                               );

            emspHTTPServer2  = new HTTPServer(
                                   IPPort.Parse(7136),
                                   Autostart: true
                               );

            Assert.IsNotNull(cpoHTTPServer);
            Assert.IsNotNull(emspHTTPServer1);
            Assert.IsNotNull(emspHTTPServer2);


            cpoWebAPI       = new WebAPI.OCPIWebAPI(

                                  cpoHTTPServer,

                                  new CommonAPI(

                                      cpoVersionsAPIURL,
                                      new BusinessDetails(
                                          "GraphDefined CPO Services",
                                          URL.Parse("https://www.graphdefined.com/cpo")
                                      ),
                                      CountryCode.Parse("DE"),
                                      Party_Id.   Parse("GEF"),
                                      Roles.      CPO,

                                      HTTPServer:            cpoHTTPServer,
                                      HTTPHostname:          HTTPHostname.Any,
                                      Disable_RootServices:  false

                                  ),

                                  RequestTimeout: TimeSpan.FromSeconds(20)

                              );

            emspWebAPI1     = new WebAPI.OCPIWebAPI(

                                  emspHTTPServer1,

                                  new CommonAPI(

                                      emspVersionsAPIURL1,
                                      new BusinessDetails(
                                          "GraphDefined EMSP #1 Services",
                                          URL.Parse("https://www.graphdefined.com/emsp1")
                                      ),
                                      CountryCode.Parse("DE"),
                                      Party_Id.   Parse("GDF"),
                                      Roles.      EMSP,

                                      HTTPServer:            emspHTTPServer1,
                                      HTTPHostname:          HTTPHostname.Any,
                                      Disable_RootServices:  false

                                  ),

                                  RequestTimeout: TimeSpan.FromSeconds(10)

                              );

            emspWebAPI2     = new WebAPI.OCPIWebAPI(

                                  emspHTTPServer2,

                                  new CommonAPI(

                                      emspVersionsAPIURL2,
                                      new BusinessDetails(
                                          "GraphDefined EMSP #2 Services",
                                          URL.Parse("https://www.graphdefined.com/emsp2")
                                      ),
                                      CountryCode.Parse("DE"),
                                      Party_Id.   Parse("GD2"),
                                      Roles.      EMSP,

                                      HTTPServer:            emspHTTPServer2,
                                      HTTPHostname:          HTTPHostname.Any,
                                      Disable_RootServices:  false

                                  ),

                                  RequestTimeout: TimeSpan.FromSeconds(10)

                              );

            Assert.IsNotNull(cpoWebAPI);
            Assert.IsNotNull(emspWebAPI1);
            Assert.IsNotNull(emspWebAPI2);



            var cpoAPI           = new CPOAPI(
                                       cpoWebAPI.CommonAPI,
                                       CountryCode.Parse("DE"),
                                       Party_Id.   Parse("GEF"),
                                       URLPathPrefix:    HTTPPath.Parse($"{Version.Id}/cpo"),
                                       //LoggingPath:      Path.Combine(OpenChargingCloudAPIPath, "CPOAPI"),
                                       AllowDowngrades:  false
                                   );

            var emspAPI1         = new EMSPAPI(
                                       emspWebAPI1.CommonAPI,
                                       CountryCode.Parse("DE"),
                                       Party_Id.   Parse("GDF"),
                                       URLPathPrefix:    HTTPPath.Parse($"{Version.Id}/emsp1"),
                                       //LoggingPath:      Path.Combine(OpenChargingCloudAPIPath, "CPOAPI"),
                                       AllowDowngrades:  false
                                   );

            var emspAPI2         = new EMSPAPI(
                                       emspWebAPI2.CommonAPI,
                                       CountryCode.Parse("DE"),
                                       Party_Id.   Parse("GD2"),
                                       URLPathPrefix:    HTTPPath.Parse($"{Version.Id}/emsp2"),
                                       //LoggingPath:      Path.Combine(OpenChargingCloudAPIPath, "CPOAPI"),
                                       AllowDowngrades:  false
                                   );



            var addEMSPResult1 = cpoWebAPI.CommonAPI.AddRemoteParty(
                CountryCode:         CountryCode.Parse("DE"),
                PartyId:             Party_Id.   Parse("GDF"),
                Role:                Roles.      EMSP,
                BusinessDetails:     new BusinessDetails("GraphDefined EMSP #1 Services"),
                AccessInfoStatus:    new[] {
                                         new AccessInfoStatus(
                                             AccessToken.Parse("xxxxxx"),
                                             AccessStatus.ALLOWED
                                         )
                                     },
                RemoteAccessInfos:   new[] {
                                         new RemoteAccessInfo(
                                             AccessToken:        AccessToken.Parse("yyyyyy"),
                                             VersionsURL:        emspVersionsAPIURL1,
                                             VersionIds:         new[] {
                                                                     Version.Id
                                                                 },
                                             SelectedVersionId:  Version.Id,
                                             Status:             RemoteAccessStatus.ONLINE
                                         )
                                     },
                Status:              PartyStatus.ENABLED
            );

            var addEMSPResult2 = cpoWebAPI.CommonAPI.AddRemoteParty(
                CountryCode:         CountryCode.Parse("DE"),
                PartyId:             Party_Id.   Parse("GD2"),
                Role:                Roles.      EMSP,
                BusinessDetails:     new BusinessDetails("GraphDefined EMSP #2 Services"),
                AccessInfoStatus:    new[] {
                                         new AccessInfoStatus(
                                             AccessToken.Parse("eeeeee"),
                                             AccessStatus.ALLOWED
                                         )
                                     },
                RemoteAccessInfos:   new[] {
                                         new RemoteAccessInfo(
                                             AccessToken:        AccessToken.Parse("ffffff"),
                                             VersionsURL:        emspVersionsAPIURL2,
                                             VersionIds:         new[] {
                                                                     Version.Id
                                                                 },
                                             SelectedVersionId:  Version.Id,
                                             Status:             RemoteAccessStatus.ONLINE
                                         )
                                     },
                Status:              PartyStatus.ENABLED
            );

            Assert.IsTrue(addEMSPResult1);
            Assert.IsTrue(addEMSPResult2);



            var addCPOResult1 = emspWebAPI1.CommonAPI.AddRemoteParty(
                CountryCode:         CountryCode.Parse("DE"),
                PartyId:             Party_Id.   Parse("GEF"),
                Role:                Roles.      CPO,
                BusinessDetails:     new BusinessDetails("GraphDefined CPO Services"),
                AccessInfoStatus:    new[] {
                                         new AccessInfoStatus(
                                             AccessToken.Parse("yyyyyy"),
                                             AccessStatus.ALLOWED
                                         )
                                         //new AccessInfo2(
                                         //    AccessToken.Parse("eeeeee"),
                                         //    AccessStatus.BLOCKED
                                         //)
                                     },
                RemoteAccessInfos:   new[] {
                                         new RemoteAccessInfo(
                                             AccessToken:        AccessToken.Parse("xxxxxx"),
                                             VersionsURL:        cpoVersionsAPIURL,
                                             VersionIds:         new[] {
                                                                     Version.Id
                                                                 },
                                             SelectedVersionId:  Version.Id,
                                             Status:             RemoteAccessStatus.ONLINE
                                         )
                                     },
                Status:              PartyStatus.ENABLED
            );

            var addCPOResult2 = emspWebAPI2.CommonAPI.AddRemoteParty(
                CountryCode:         CountryCode.Parse("DE"),
                PartyId:             Party_Id.   Parse("GEF"),
                Role:                Roles.      CPO,
                BusinessDetails:     new BusinessDetails("GraphDefined CPO Services"),
                AccessInfoStatus:    new[] {
                                         new AccessInfoStatus(
                                             AccessToken.Parse("ffffff"),
                                             AccessStatus.ALLOWED
                                         )
                                         //new AccessInfo2(
                                         //    AccessToken.Parse("eeeeee"),
                                         //    AccessStatus.BLOCKED
                                         //)
                                     },
                RemoteAccessInfos:   new[] {
                                         new RemoteAccessInfo(
                                             AccessToken:        AccessToken.Parse("eeeeee"),
                                             VersionsURL:        cpoVersionsAPIURL,
                                             VersionIds:         new[] {
                                                                     Version.Id
                                                                 },
                                             SelectedVersionId:  Version.Id,
                                             Status:             RemoteAccessStatus.ONLINE
                                         )
                                     },
                Status:              PartyStatus.ENABLED
            );

            Assert.IsTrue(addCPOResult1);
            Assert.IsTrue(addCPOResult2);

        }

        #endregion

        #region ShutdownEachTest()

        [TearDown]
        public void ShutdownEachTest()
        {

            cpoWebAPI?.     Shutdown();
            cpoHTTPServer?. Shutdown();

            emspWebAPI1?.    Shutdown();
            emspHTTPServer1?.Shutdown();

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
