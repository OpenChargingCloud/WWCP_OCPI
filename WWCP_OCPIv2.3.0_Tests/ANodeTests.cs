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

using System.Collections.Concurrent;

using Newtonsoft.Json.Linq;

using NUnit.Framework;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.Logging;

using cloud.charging.open.protocols.OCPI;
using cloud.charging.open.protocols.OCPIv2_3_0.WebAPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_3_0.UnitTests
{

    public static class TestHelpers
    {

        public static async Task<HTTPResponse<JObject>> JSONRequest(URL     RemoteURL,
                                                                    String  Token)
        {

            var httpResponse  = await new HTTPClient(RemoteURL).GET(
                                          RemoteURL.Path,
                                          RequestBuilder: requestBuilder => {
                                              requestBuilder.Authorization  = HTTPTokenAuthentication.ParseHTTPHeader(Token);
                                              requestBuilder.Accept.Add(HTTPContentType.Application.JSON_UTF8);
                                              requestBuilder.Set("X-Request-ID",      "1234");
                                              requestBuilder.Set("X-Correlation-ID",  "5678");
                                          }
                                      ).ConfigureAwait(false);

            return new HTTPResponse<JObject>(httpResponse,
                                             JObject.Parse(httpResponse.HTTPBodyAsUTF8String));

        }

        public static async Task<HTTPResponse<JObject>> JSONRequest(HTTPMethod  Method,
                                                                    URL         RemoteURL,
                                                                    String      Token,
                                                                    JObject     JSON)
        {

            var httpResponse  = await new HTTPClient(RemoteURL).
                                          RunRequest(
                                              Method,
                                              RemoteURL.Path,
                                              RequestBuilder: requestBuilder => {
                                                  requestBuilder.Authorization  = HTTPTokenAuthentication.ParseHTTPHeader(Token);
                                                  requestBuilder.ContentType    = HTTPContentType.Application.JSON_UTF8;
                                                  requestBuilder.Content        = JSON.ToUTF8Bytes();
                                                  requestBuilder.Accept.Add(HTTPContentType.Application.JSON_UTF8);
                                                  requestBuilder.Set("X-Request-ID",      "1234");
                                                  requestBuilder.Set("X-Correlation-ID",  "5678");
                                              }
                                          ).ConfigureAwait(false);

            return new HTTPResponse<JObject>(httpResponse,
                                             JObject.Parse(httpResponse.HTTPBodyAsUTF8String));

        }


    }


    /// <summary>
    /// OCPI v2.3 Node test defaults.
    /// </summary>
    public abstract class ANodeTests
    {

        #region Data

        protected       HTTPServer?                                          cpoHTTPServer;
        protected       HTTPServer?                                          emsp1HTTPServer;
        protected       HTTPServer?                                          emsp2HTTPServer;

        protected       HTTPExtAPI?                                          cpoHTTPAPI;
        protected       HTTPExtAPI?                                          emsp1HTTPAPI;
        protected       HTTPExtAPI?                                          emsp2HTTPAPI;

        //protected       HTTPAPI?                                             cpoHTTPAPI;
        protected       CommonAPI?                                           cpoCommonAPI;
        protected       OCPIWebAPI?                                          cpoWebAPI;
        protected       CPO_HTTPAPI?                                         cpoCPOAPI;
        protected       OCPICSOAdapter?                                      cpoAdapter;
        protected       ConcurrentDictionary<DateTimeOffset, OCPIRequest>    cpoAPIRequestLogs;
        protected       ConcurrentDictionary<DateTimeOffset, OCPIResponse>   cpoAPIResponseLogs;

        //protected       HTTPAPI?                                             emsp1HTTPAPI;
        protected       CommonAPI?                                           emsp1CommonAPI;
        protected       OCPIWebAPI?                                          emsp1WebAPI;
        protected       EMSP_HTTPAPI?                                        emsp1EMSPAPI;
        protected       OCPIEMPAdapter?                                      emsp1Adapter;
        protected       ConcurrentDictionary<DateTimeOffset, OCPIRequest>    emsp1APIRequestLogs;
        protected       ConcurrentDictionary<DateTimeOffset, OCPIResponse>   emsp1APIResponseLogs;

        //protected       HTTPAPI?                                             emsp2HTTPAPI;
        protected       CommonAPI?                                           emsp2CommonAPI;
        protected       OCPIWebAPI?                                          emsp2WebAPI;
        protected       EMSP_HTTPAPI?                                        emsp2EMSPAPI;
        protected       OCPIEMPAdapter?                                      emsp2Adapter;
        protected       ConcurrentDictionary<DateTimeOffset, OCPIRequest>    emsp2APIRequestLogs;
        protected       ConcurrentDictionary<DateTimeOffset, OCPIResponse>   emsp2APIResponseLogs;

        /// <summary>
        /// The directory of this test's nodes' files - their remote parties and
        /// assets - so that no test reads what another one wrote.
        /// </summary>
        /// <summary>
        /// Whether the CPO and EMSP APIs come from adapters a subclass makes,
        /// rather than from this rig.
        /// </summary>
        protected virtual Boolean ModuleAPIsByAdapters
            => false;

        protected       String                                               nodesDirectory  = default!;

        public          URL?                                                 cpoVersionsAPIURL;
        public          URL?                                                 emsp1VersionsAPIURL;
        public          URL?                                                 emsp2VersionsAPIURL;

        protected const String                                               cpo_accessing_emsp1__token  = "cpo_accessing_emsp1++token";
        protected const String                                               cpo_accessing_emsp2__token  = "cpo_accessing_emsp2++token";

        protected const String                                               emsp1_accessing_cpo__token  = "emsp1_accessing_cpo++token";
        protected const String                                               emsp2_accessing_cpo__token  = "emsp2_accessing_cpo++token";

        protected const String                                               UnknownToken                = "UnknownUnknownUnknownToken";

        protected const String                                               BlockedCPOToken             = "blocked-cpo";
        protected const String                                               BlockedEMSPToken            = "blocked-emsp";

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
        public async virtual Task SetupEachTest()
        {

            Timestamp.Reset();

            nodesDirectory = Path.Combine(Path.GetTempPath(), $"WWCP_OCPI_Tests-{Guid.NewGuid():N}");
            foreach (var node in new[] { "cpo", "emsp1", "emsp2" })
                Directory.CreateDirectory(Path.Combine(nodesDirectory, node));

            #region Create cpo/emsp1/emsp2 HTTP Servers

            cpoHTTPServer         = new HTTPServer(
                                        IPAddress:         IPv4Address.Localhost,
                                        TCPPort:         IPPort.Parse(FreePort())
                                    );

            emsp1HTTPServer       = new HTTPServer(
                                        IPAddress:         IPv4Address.Localhost,
                                        TCPPort:         IPPort.Parse(FreePort())
                                    );

            emsp2HTTPServer       = new HTTPServer(
                                        IPAddress:         IPv4Address.Localhost,
                                        TCPPort:         IPPort.Parse(FreePort())
                                    );

            #endregion

            #region Create cpo/emsp1/emsp2 HTTP Servers

            cpoHTTPAPI            = new HTTPExtAPI(
                                        HTTPServer:      cpoHTTPServer
                                    );

            emsp1HTTPAPI          = new HTTPExtAPI(
                                        HTTPServer:      emsp1HTTPServer
                                    );

            emsp2HTTPAPI          = new HTTPExtAPI(
                                        HTTPServer:      emsp2HTTPServer
                                    );

            #endregion

            // Each node is a platform of its own: its own HTTP server, its own
            // Common HTTP API, its own Common API. All three shared the CPO's
            // Common HTTP API, and the second Common API failed to register the
            // routes the first one had - every test of this rig failed in its
            // SetUp, from the day CommonHTTPAPI came.
            var cpoBaseAPI    = ABaseAPI(cpoHTTPAPI);
            var emsp1BaseAPI  = ABaseAPI(emsp1HTTPAPI);
            var emsp2BaseAPI  = ABaseAPI(emsp2HTTPAPI);


            #region Create cpo/emsp1/emsp2 OCPI Common API

            cpoVersionsAPIURL    = cpoBaseAPI.OurVersionsURL;

            cpoCommonAPI         = new CommonAPI(

                                       //OurBaseURL:                          URL.Parse("http://127.0.0.1:3301/ocpi/v2.3"),
                                       //OurVersionsURL:                      cpoVersionsAPIURL.Value,
                                       OurPartyData:                        [
                                                                                new PartyData(
                                                                                    Party_Idv3.From(
                                                                                        CountryCode.Parse("DE"),
                                                                                        Party_Id.   Parse("GEF")
                                                                                    ),
                                                                                    Role.CPO,
                                                                                    new BusinessDetails(
                                                                                        "GraphDefined CSO Services",
                                                                                        URL.Parse("https://www.graphdefined.com/cso")
                                                                                    )
                                                                                )
                                                                            ],
                                       DefaultPartyId:                      Party_Idv3.From(
                                                                                CountryCode.Parse("DE"),
                                                                                Party_Id.   Parse("GEF")
                                                                            ),

                                       BaseAPI:                             cpoBaseAPI,
                                       DatabaseFilePath:                    Path.Combine(nodesDirectory, "cpo"),

                                       AdditionalURLPathPrefix:             null,
                                       KeepRemovedEVSEs:                    null,

                                       ExternalDNSName:                     null,
                                       HTTPServiceName:                     null,
                                       BasePath:                            null,

                                       URLPathPrefix:                       null,  // the default, as a platform makes it
                                       APIVersionHashes:                    null,

                                       IsDevelopment:                       null,
                                       DevelopmentServers:                  null,
                                       DisableLogging:                      null,
                                       LoggingPath:                         null,
                                       LogfileName:                         null,
                                       LogfileCreator:                      null

                                   );

            //await cpoCommonAPI.AddParty(
            //          Party_Idv3.From(
            //              CountryCode.Parse("DE"),
            //              Party_Id.   Parse("GEF")
            //          ),
            //          Role.CPO,
            //          new BusinessDetails(
            //              "GraphDefined CSO Services",
            //              URL.Parse("https://www.graphdefined.com/cso")
            //          )
            //      );


            emsp1VersionsAPIURL  = emsp1BaseAPI.OurVersionsURL;

            emsp1CommonAPI       = new CommonAPI(

                                       //OurBaseURL:                          URL.Parse("http://127.0.0.1:3401/ocpi/v2.3"),
                                       //OurVersionsURL:                      emsp1VersionsAPIURL.Value,
                                       OurPartyData:                        [
                                                                                new PartyData(
                                                                                    Party_Idv3.From(
                                                                                        CountryCode.Parse("DE"),
                                                                                        Party_Id.   Parse("GDF")
                                                                                    ),
                                                                                    Role.EMSP,
                                                                                    new BusinessDetails(
                                                                                        "GraphDefined EMSP #1 Services",
                                                                                        URL.Parse("https://www.graphdefined.com/emsp1")
                                                                                    )
                                                                                )
                                                                            ],
                                       DefaultPartyId:                      Party_Idv3.From(
                                                                                CountryCode.Parse("DE"),
                                                                                Party_Id.   Parse("GDF")
                                                                            ),

                                       BaseAPI:                             emsp1BaseAPI,
                                       DatabaseFilePath:                    Path.Combine(nodesDirectory, "emsp1"),
                                       //HTTPServer:                          emsp1HTTPAPI.HTTPServer,

                                       AdditionalURLPathPrefix:             null,
                                       KeepRemovedEVSEs:                    null,

                                       ExternalDNSName:                     null,
                                       HTTPServiceName:                     null,
                                       BasePath:                            null,

                                       URLPathPrefix:                       null,  // the default, as a platform makes it
                                       APIVersionHashes:                    null,

                                       IsDevelopment:                       null,
                                       DevelopmentServers:                  null,
                                       DisableLogging:                      null,
                                       LoggingPath:                         null,
                                       LogfileName:                         null,
                                       LogfileCreator:                      null

                                   );

            //await emsp1CommonAPI.AddParty(
            //          Party_Idv3.From(
            //              CountryCode.Parse("DE"),
            //              Party_Id.   Parse("GDF")
            //          ),
            //          Role.EMSP,
            //          new BusinessDetails(
            //              "GraphDefined EMSP #1 Services",
            //              URL.Parse("https://www.graphdefined.com/emsp1")
            //          )
            //      );


            emsp2VersionsAPIURL  = emsp2BaseAPI.OurVersionsURL;

            emsp2CommonAPI       = new CommonAPI(

                                       //OurBaseURL:                          URL.Parse("http://127.0.0.1:3402/ocpi/v2.3"),
                                       //OurVersionsURL:                      emsp2VersionsAPIURL.Value,
                                       OurPartyData:                        [
                                                                                new PartyData(
                                                                                    Party_Idv3.From(
                                                                                        CountryCode.Parse("DE"),
                                                                                        Party_Id.   Parse("GD2")
                                                                                    ),
                                                                                    Role.EMSP,
                                                                                    new BusinessDetails(
                                                                                        "GraphDefined EMSP #2 Services",
                                                                                        URL.Parse("https://www.graphdefined.com/emsp2")
                                                                                    )
                                                                                )
                                                                            ],
                                       DefaultPartyId:                      Party_Idv3.From(
                                                                                CountryCode.Parse("DE"),
                                                                                Party_Id.   Parse("GD2")
                                                                            ),

                                       BaseAPI:                             emsp2BaseAPI,
                                       DatabaseFilePath:                    Path.Combine(nodesDirectory, "emsp2"),

                                       AdditionalURLPathPrefix:             null,
                                       KeepRemovedEVSEs:                    null,

                                       ExternalDNSName:                     null,
                                       HTTPServiceName:                     null,
                                       BasePath:                            null,

                                       URLPathPrefix:                       null,  // the default, as a platform makes it
                                       APIVersionHashes:                    null,

                                       IsDevelopment:                       null,
                                       DevelopmentServers:                  null,
                                       DisableLogging:                      null,
                                       LoggingPath:                         null,
                                       LogfileName:                         null,
                                       LogfileCreator:                      null

                                   );

            //await emsp2CommonAPI.AddParty(
            //          Party_Idv3.From(
            //              CountryCode.Parse("DE"),
            //              Party_Id.   Parse("GD2")
            //          ),
            //          Role.EMSP,
            //          new BusinessDetails(
            //              "GraphDefined EMSP #2 Services",
            //              URL.Parse("https://www.graphdefined.com/emsp2")
            //          )
            //      );


            Assert.That(cpoVersionsAPIURL,   Is.Not.Null);
            Assert.That(emsp1VersionsAPIURL, Is.Not.Null);
            Assert.That(emsp2VersionsAPIURL, Is.Not.Null);

            Assert.That(cpoCommonAPI,   Is.Not.Null);
            Assert.That(emsp1CommonAPI, Is.Not.Null);
            Assert.That(emsp2CommonAPI, Is.Not.Null);

            #endregion

            #region Create cpo CPO API / emsp1 EMP API / emsp2 EMP API

            // Not where a subclass makes adapters: each adapter makes its own, on
            // the same Common API, and a second one there fails to register the
            // routes the first one has.
            if (!ModuleAPIsByAdapters)
            {

                cpoCPOAPI            = new CPO_HTTPAPI(

                                           CommonAPI:                           cpoCommonAPI,
                                           AllowDowngrades:                     null,

                                           ExternalDNSName:                     null,
                                           HTTPServiceName:                     null,
                                           BasePath:                            null,

                                           URLPathPrefix:                       null,  // the default, where the version details announce it
                                           APIVersionHashes:                    null,

                                           IsDevelopment:                       null,
                                           DevelopmentServers:                  null,
                                           DisableLogging:                      null,
                                           LoggingPath:                         null,
                                           LogfileName:                         null,
                                           LogfileCreator:                      null

                                       );

                emsp1EMSPAPI         = new EMSP_HTTPAPI(

                                           CommonAPI:                           emsp1CommonAPI,
                                           AllowDowngrades:                     null,

                                           ExternalDNSName:                     null,
                                           HTTPServiceName:                     null,
                                           BasePath:                            null,

                                           URLPathPrefix:                       null,  // the default, where the version details announce it
                                           APIVersionHashes:                    null,

                                           IsDevelopment:                       null,
                                           DevelopmentServers:                  null,
                                           DisableLogging:                      null,
                                           LoggingPath:                         null,
                                           LogfileName:                         null,
                                           LogfileCreator:                      null

                                       );

                emsp2EMSPAPI         = new EMSP_HTTPAPI(

                                           CommonAPI:                           emsp2CommonAPI,
                                           AllowDowngrades:                     null,

                                           ExternalDNSName:                     null,
                                           HTTPServiceName:                     null,
                                           BasePath:                            null,

                                           URLPathPrefix:                       null,  // the default, where the version details announce it
                                           APIVersionHashes:                    null,

                                           IsDevelopment:                       null,
                                           DevelopmentServers:                  null,
                                           DisableLogging:                      null,
                                           LoggingPath:                         null,
                                           LogfileName:                         null,
                                           LogfileCreator:                      null

                                       );

                Assert.That(cpoCPOAPI,    Is.Not.Null);
                Assert.That(emsp1EMSPAPI, Is.Not.Null);
                Assert.That(emsp2EMSPAPI, Is.Not.Null);

            }

            #endregion

            #region Add Remote Parties

            await cpoCommonAPI.AddRemoteParty  (Id:                                RemoteParty_Id.From(
                                                                                       emsp1CommonAPI.Parties.First().Id.CountryCode,
                                                                                       emsp1CommonAPI.Parties.First().Id.PartyId,
                                                                                       emsp1CommonAPI.Parties.First().Role
                                                                                   ),
                                                CredentialsRoles:                  [
                                                                                       new CredentialsRole(
                                                                                           CountryCode:       emsp1CommonAPI.Parties.First().Id.CountryCode,
                                                                                           PartyId:           emsp1CommonAPI.Parties.First().Id.PartyId,
                                                                                           Role:              Role.EMSP,
                                                                                           BusinessDetails:   emsp1CommonAPI.Parties.First().BusinessDetails,
                                                                                           AllowDowngrades:   false
                                                                                       )
                                                                                   ],

                                                LocalAccessToken:                  AccessToken.Parse("cso-2-emp1:token"),
                                                LocalAccessStatus:                 AccessStatus.ALLOWED,

                                                RemoteAccessToken:                 AccessToken.Parse("emp1-2-cso:token"),
                                                RemoteVersionsURL:                 emsp1VersionsAPIURL.Value,
                                                RemoteVersionIds:                  null,
                                                RemoteAccessTokenBase64Encoding:   true,
                                                RemoteStatus:                      RemoteAccessStatus.ONLINE,

                                                Status:                            PartyStatus.ENABLED);

            await cpoCommonAPI.AddRemoteParty  (Id:                                RemoteParty_Id.From(
                                                                                       emsp2CommonAPI.Parties.First().Id.CountryCode,
                                                                                       emsp2CommonAPI.Parties.First().Id.PartyId,
                                                                                       emsp2CommonAPI.Parties.First().Role
                                                                                   ),
                                                CredentialsRoles:                  [
                                                                                       new CredentialsRole(
                                                                                           CountryCode:       emsp2CommonAPI.Parties.First().Id.CountryCode,
                                                                                           PartyId:           emsp2CommonAPI.Parties.First().Id.PartyId,
                                                                                           Role:              Role.EMSP,
                                                                                           BusinessDetails:   emsp2CommonAPI.Parties.First().BusinessDetails,
                                                                                           AllowDowngrades:   false
                                                                                       )
                                                                                   ],

                                                LocalAccessToken:                  AccessToken.Parse("cso-2-emp2:token"),
                                                LocalAccessStatus:                 AccessStatus.ALLOWED,
                                                RemoteAccessToken:                 AccessToken.Parse("emp2-2-cso:token"),
                                                RemoteVersionsURL:                 emsp2VersionsAPIURL.Value,
                                                RemoteVersionIds:                  null,
                                                RemoteAccessTokenBase64Encoding:   true,
                                                RemoteStatus:                      RemoteAccessStatus.ONLINE,
                                                Status:                            PartyStatus.ENABLED);



            await emsp1CommonAPI.AddRemoteParty(Id:                                RemoteParty_Id.From(
                                                                                       cpoCommonAPI.Parties.First().Id.CountryCode,
                                                                                       cpoCommonAPI.Parties.First().Id.PartyId,
                                                                                       cpoCommonAPI.Parties.First().Role
                                                                                   ),
                                                CredentialsRoles:                  [
                                                                                       new CredentialsRole(
                                                                                           CountryCode:       cpoCommonAPI.Parties.First().Id.CountryCode,
                                                                                           PartyId:           cpoCommonAPI.Parties.First().Id.PartyId,
                                                                                           Role:              Role.CPO,
                                                                                           BusinessDetails:   cpoCommonAPI.Parties.First().BusinessDetails,
                                                                                           AllowDowngrades:   false
                                                                                       )
                                                                                   ],

                                                LocalAccessToken:                  AccessToken.Parse("emp1-2-cso:token"),
                                                LocalAccessStatus:                 AccessStatus.ALLOWED,

                                                RemoteAccessToken:                 AccessToken.Parse("cso-2-emp1:token"),
                                                RemoteVersionsURL:                 cpoVersionsAPIURL.Value,
                                                RemoteVersionIds:                  null,
                                                RemoteAccessTokenBase64Encoding:   true,
                                                RemoteStatus:                      RemoteAccessStatus.ONLINE,

                                                Status:                            PartyStatus.ENABLED);


            await emsp2CommonAPI.AddRemoteParty(Id:                                RemoteParty_Id.From(
                                                                                       cpoCommonAPI.Parties.First().Id.CountryCode,
                                                                                       cpoCommonAPI.Parties.First().Id.PartyId,
                                                                                       cpoCommonAPI.Parties.First().Role
                                                                                   ),
                                                CredentialsRoles:                  [
                                                                                       new CredentialsRole(
                                                                                           CountryCode:       cpoCommonAPI.Parties.First().Id.CountryCode,
                                                                                           PartyId:           cpoCommonAPI.Parties.First().Id.PartyId,
                                                                                           Role:              Role.CPO,
                                                                                           BusinessDetails:   cpoCommonAPI.Parties.First().BusinessDetails,
                                                                                           AllowDowngrades:   false
                                                                                       )
                                                                                   ],

                                                LocalAccessToken:                  AccessToken.Parse("emp2-2-cso:token"),
                                                LocalAccessStatus:                 AccessStatus.ALLOWED,

                                                RemoteAccessToken:                 AccessToken.Parse("cso-2-emp2:token"),
                                                RemoteVersionsURL:                 cpoVersionsAPIURL.Value,
                                                RemoteVersionIds:                  null,
                                                RemoteAccessTokenBase64Encoding:   true,
                                                RemoteStatus:                      RemoteAccessStatus.ONLINE,

                                                Status:                            PartyStatus.ENABLED);


            Assert.That(cpoCommonAPI.  RemoteParties.Count(), Is.EqualTo(2));
            Assert.That(emsp1CommonAPI.RemoteParties.Count(), Is.EqualTo(1));
            Assert.That(emsp2CommonAPI.RemoteParties.Count(), Is.EqualTo(1));

            #endregion

            await cpoHTTPServer.  Start();
            await emsp1HTTPServer.Start();
            await emsp2HTTPServer.Start();

        }

        #endregion

        #region (private) FreePort() / ABaseAPI(HTTPAPI)

        /// <summary>
        /// A port nobody was listening on a moment ago.
        /// </summary>
        private static UInt16 FreePort()
        {

            var listener = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Loopback, 0);

            listener.Start();

            var port = (UInt16) ((System.Net.IPEndPoint) listener.LocalEndpoint).Port;

            listener.Stop();

            return port;

        }

        /// <summary>
        /// The Common HTTP API of a node on the given HTTP API: its URLs are
        /// the ones its server listens on.
        /// </summary>
        private static CommonHTTPAPI ABaseAPI(HTTPExtAPI HTTPAPI)

            => new (
                   HTTPAPI:              HTTPAPI,
                   OurBaseURL:           URL.Parse($"http://127.0.0.1:{HTTPAPI.HTTPServer.TCPPort}/ocpi"),
                   OurVersionsURL:       URL.Parse($"http://127.0.0.1:{HTTPAPI.HTTPServer.TCPPort}/ocpi/versions"),
                   LocationsAsOpenData:  true,
                   RootPath:             HTTPPath.Parse("/ocpi")
               );

        #endregion

        #region ShutdownEachTest()

        [TearDown]
        public virtual async Task ShutdownEachTest()
        {

            if (cpoHTTPServer   is not null)
                await cpoHTTPServer.  DisposeAsync();

            if (emsp1HTTPServer is not null)
                await emsp1HTTPServer.DisposeAsync();

            if (emsp2HTTPServer is not null)
                await emsp2HTTPServer.DisposeAsync();

            // Their base APIs write out what their files still wait for, and
            // then the files go.
            foreach (var commonAPI in new[] { cpoCommonAPI, emsp1CommonAPI, emsp2CommonAPI })
                if (commonAPI is not null)
                    await commonAPI.BaseAPI.DisposeAsync();

            try
            {
                Directory.Delete(nodesDirectory, true);
            }
            catch (IOException)
            { }
            catch (UnauthorizedAccessException)
            { }

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
