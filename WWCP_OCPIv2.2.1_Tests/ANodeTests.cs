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

using System.Collections.Concurrent;

using Newtonsoft.Json.Linq;

using NUnit.Framework;
using NUnit.Framework.Legacy;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.HTTPTest;

using cloud.charging.open.protocols.OCPI;
using cloud.charging.open.protocols.OCPIv2_2_1.HTTP;
using cloud.charging.open.protocols.OCPIv2_2_1.WebAPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2_1.UnitTests
{

    public static class TestHelpers
    {

        public static async Task<HTTPResponse<JObject?>> JSONRequest(URL     RemoteURL,
                                                                     String  Token)
        {

            var httpResponse  = await new HTTPClient(RemoteURL).
                                              Execute(client => client.CreateRequest(HTTPMethod.GET,
                                                                                     RemoteURL.Path,
                                                                                     RequestBuilder: requestBuilder => {
                                                                                         requestBuilder.Authorization  = HTTPTokenAuthentication.ParseHTTPHeader(Token);
                                                                                         requestBuilder.Accept.Add(HTTPContentType.Application.JSON_UTF8);
                                                                                         requestBuilder.Set("X-Request-ID",      "1234");
                                                                                         requestBuilder.Set("X-Correlation-ID",  "5678");
                                                                                     })).
                                              ConfigureAwait(false);

            return new HTTPResponse<JObject?>(
                       httpResponse,
                       httpResponse.HTTPBodyAsJSONObject
                   );

        }

        public static async Task<HTTPResponse<JObject?>> JSONRequest(HTTPMethod  Method,
                                                                     URL         RemoteURL,
                                                                     String      Token,
                                                                     JObject     JSON)
        {

            var httpResponse  = await new HTTPClient(RemoteURL).
                                              Execute(client => client.CreateRequest(Method,
                                                                                     RemoteURL.Path,
                                                                                     RequestBuilder: requestBuilder => {
                                                                                         requestBuilder.Authorization  = HTTPTokenAuthentication.ParseHTTPHeader(Token);
                                                                                         requestBuilder.ContentType    = HTTPContentType.Application.JSON_UTF8;
                                                                                         requestBuilder.Content        = JSON.ToUTF8Bytes();
                                                                                         requestBuilder.Accept.Add(HTTPContentType.Application.JSON_UTF8);
                                                                                         requestBuilder.Set("X-Request-ID",      "1234");
                                                                                         requestBuilder.Set("X-Correlation-ID",  "5678");
                                                                                     })).
                                              ConfigureAwait(false);

            return new HTTPResponse<JObject?>(
                       httpResponse,
                       httpResponse.HTTPBodyAsJSONObject
                   );

        }


    }


    /// <summary>
    /// OCPI v2.2 Node test defaults.
    /// </summary>
    public abstract class ANodeTests
    {

        #region Data

        protected       HTTPTestServerX?                                     cpoHTTPServer;
        protected       HTTPTestServerX?                                     emsp1HTTPServer;
        protected       HTTPTestServerX?                                     emsp2HTTPServer;

        protected       HTTPExtAPIX?                                         cpoHTTPAPI;
        protected       HTTPExtAPIX?                                         emsp1HTTPAPI;
        protected       HTTPExtAPIX?                                         emsp2HTTPAPI;

        //protected       HTTPAPI?                                             cpoHTTPAPI;
        protected       CommonAPI?                                           cpoCommonAPI;
        protected       OCPIWebAPI?                                          cpoWebAPI;
        protected       CPOAPI?                                              cpoCPOAPI;
        protected       OCPICSOAdapter?                                      cpoAdapter;
        protected       ConcurrentDictionary<DateTimeOffset, OCPIRequest>    cpoAPIRequestLogs;
        protected       ConcurrentDictionary<DateTimeOffset, OCPIResponse>   cpoAPIResponseLogs;

        //protected       HTTPAPI?                                             emsp1HTTPAPI;
        protected       CommonAPI?                                           emsp1CommonAPI;
        protected       OCPIWebAPI?                                          emsp1WebAPI;
        protected       EMSPAPI?                                             emsp1EMSPAPI;
        protected       OCPIEMPAdapter?                                      emsp1Adapter;
        protected       ConcurrentDictionary<DateTimeOffset, OCPIRequest>    emsp1APIRequestLogs;
        protected       ConcurrentDictionary<DateTimeOffset, OCPIResponse>   emsp1APIResponseLogs;

        //protected       HTTPAPI?                                             emsp2HTTPAPI;
        protected       CommonAPI?                                           emsp2CommonAPI;
        protected       OCPIWebAPI?                                          emsp2WebAPI;
        protected       EMSPAPI?                                             emsp2EMSPAPI;
        protected       OCPIEMPAdapter?                                      emsp2Adapter;
        protected       ConcurrentDictionary<DateTimeOffset, OCPIRequest>    emsp2APIRequestLogs;
        protected       ConcurrentDictionary<DateTimeOffset, OCPIResponse>   emsp2APIResponseLogs;

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

            #region Create cpo/emsp1/emsp2 HTTP Servers

            cpoHTTPServer         = new HTTPTestServerX(
                                        TCPPort:         IPPort.Parse(3301)
                                    );

            emsp1HTTPServer       = new HTTPTestServerX(
                                        TCPPort:         IPPort.Parse(3401)
                                    );

            emsp2HTTPServer       = new HTTPTestServerX(
                                        TCPPort:         IPPort.Parse(3402)
                                    );

            #endregion

            #region Create cpo/emsp1/emsp2 HTTP Servers

            cpoHTTPAPI            = new HTTPExtAPIX(
                                        HTTPServer:      cpoHTTPServer
                                    );

            emsp1HTTPAPI          = new HTTPExtAPIX(
                                        HTTPServer:      emsp1HTTPServer
                                    );

            emsp2HTTPAPI          = new HTTPExtAPIX(
                                        HTTPServer:      emsp2HTTPServer
                                    );

            #endregion

            var ocpiBaseAPI = new CommonHTTPAPI(

                                  HTTPAPI:                   cpoHTTPAPI,
                                  OurBaseURL:                URL.Parse("http://127.0.0.1:3201/ocpi"),
                                  OurVersionsURL:            URL.Parse("http://127.0.0.1:3201/ocpi/versions"),
                                  //OurBusinessDetails:        new OCPI.BusinessDetails(
                                  //                               "GraphDefined OCPI Services",
                                  //                               URL.Parse("https://www.graphdefined.com/ocpi")
                                  //                           ),
                                  //OurCountryCode:            OCPI.CountryCode.Parse("DE"),
                                  //OurPartyId:                OCPI.Party_Id.   Parse("BDO"),
                                  //OurRole:                   OCPI.Role.       CPO,

                                  AdditionalURLPathPrefix:   null,
                                  //KeepRemovedEVSEs:          null,
                                  LocationsAsOpenData:       true,
                                  AllowDowngrades:           null,

                                  ExternalDNSName:           null,
                                  HTTPServiceName:           null,
                                  BasePath:                  null,

                                  RootPath:                  HTTPPath.Parse("/ocpi"),
                                  APIVersionHashes:          null,

                                  IsDevelopment:             null,
                                  DevelopmentServers:        null,
                                  DisableLogging:            null,
                                  LoggingContext:            null,
                                  LoggingPath:               null,
                                  LogfileName:               null,
                                  LogfileCreator:            null

                              );

            ClassicAssert.IsNotNull(ocpiBaseAPI);


            #region Create cpo/emsp1/emsp2 OCPI Common API

            cpoVersionsAPIURL    = URL.Parse("http://127.0.0.1:3301/ocpi/v2.2/versions");

            cpoCommonAPI         = new CommonAPI(

                                       //OurBaseURL:                          URL.Parse("http://127.0.0.1:3301/ocpi/v2.2"),
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

                                       BaseAPI:                             ocpiBaseAPI,
                                       //HTTPServer:                          cpoHTTPAPI.HTTPServer,

                                       AdditionalURLPathPrefix:             null,
                                       KeepRemovedEVSEs:                    null,

                                       ExternalDNSName:                     null,
                                       HTTPServiceName:                     null,
                                       BasePath:                            null,

                                       URLPathPrefix:                       HTTPPath.Parse("/ocpi/v2.2"),
                                       APIVersionHashes:                    null,

                                       IsDevelopment:                       null,
                                       DevelopmentServers:                  null,
                                       DisableLogging:                      null,
                                       LoggingPath:                         null,
                                       LogfileName:                         null,
                                       LogfileCreator:                      null

                                   );

            await cpoCommonAPI.AddParty(
                      Party_Idv3.From(
                          CountryCode.Parse("DE"),
                          Party_Id.   Parse("GEF")
                      ),
                      Role.CPO,
                      new BusinessDetails(
                          "GraphDefined CSO Services",
                          URL.Parse("https://www.graphdefined.com/cso")
                      )
                  );


            emsp1VersionsAPIURL  = URL.Parse("http://127.0.0.1:3401/ocpi/v2.2/versions");

            emsp1CommonAPI       = new CommonAPI(

                                       //OurBaseURL:                          URL.Parse("http://127.0.0.1:3401/ocpi/v2.2"),
                                       //OurVersionsURL:                      emsp1VersionsAPIURL.Value,
                                       OurPartyData:                        [
                                                                                new PartyData(
                                                                                    Party_Idv3.From(
                                                                                        CountryCode.Parse("DE"),
                                                                                        Party_Id.   Parse("GDF")
                                                                                    ),
                                                                                    Role.CPO,
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

                                       BaseAPI:                             ocpiBaseAPI,

                                       AdditionalURLPathPrefix:             null,
                                       KeepRemovedEVSEs:                    null,

                                       ExternalDNSName:                     null,
                                       HTTPServiceName:                     null,
                                       BasePath:                            null,

                                       URLPathPrefix:                       HTTPPath.Parse("/ocpi/v2.2"),
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
            //          Role.       CPO,
            //          new BusinessDetails(
            //              "GraphDefined EMSP #1 Services",
            //              URL.Parse("https://www.graphdefined.com/emsp1")
            //          )
            //      );


            emsp2VersionsAPIURL  = URL.Parse("http://127.0.0.1:3402/ocpi/v2.2/versions");

            emsp2CommonAPI       = new CommonAPI(

                                       //OurBaseURL:                          URL.Parse("http://127.0.0.1:3402/ocpi/v2.2"),
                                       //OurVersionsURL:                      emsp2VersionsAPIURL.Value,
                                       OurPartyData:                        [
                                                                                new PartyData(
                                                                                    Party_Idv3.From(
                                                                                        CountryCode.Parse("DE"),
                                                                                        Party_Id.   Parse("GD2")
                                                                                    ),
                                                                                    Role.CPO,
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

                                       BaseAPI:                             ocpiBaseAPI,

                                       AdditionalURLPathPrefix:             null,
                                       KeepRemovedEVSEs:                    null,

                                       ExternalDNSName:                     null,
                                       HTTPServiceName:                     null,
                                       BasePath:                            null,

                                       URLPathPrefix:                       HTTPPath.Parse("/ocpi/v2.2"),
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
            //              Party_Id.   Parse("GD2")
            //          ),
            //          Role.       CPO,
            //          new BusinessDetails(
            //              "GraphDefined EMSP #2 Services",
            //              URL.Parse("https://www.graphdefined.com/emsp2")
            //          )
            //      );

            ClassicAssert.IsNotNull(cpoVersionsAPIURL);
            ClassicAssert.IsNotNull(emsp1VersionsAPIURL);
            ClassicAssert.IsNotNull(emsp2VersionsAPIURL);

            ClassicAssert.IsNotNull(cpoCommonAPI);
            ClassicAssert.IsNotNull(emsp1CommonAPI);
            ClassicAssert.IsNotNull(emsp2CommonAPI);

            #endregion

            #region Create cpo CPO API / emsp1 EMP API / emsp2 EMP API

            cpoCPOAPI            = new CPOAPI(

                                       CommonAPI:                           cpoCommonAPI,
                                       AllowDowngrades:                     null,

                                       ExternalDNSName:                     null,
                                       HTTPServiceName:                     null,
                                       BasePath:                            null,

                                       URLPathPrefix:                       HTTPPath.Parse("/ocpi/v2.2/v2.2.1/cpo"),
                                       APIVersionHashes:                    null,

                                       IsDevelopment:                       null,
                                       DevelopmentServers:                  null,
                                       DisableLogging:                      null,
                                       LoggingPath:                         null,
                                       LogfileName:                         null,
                                       LogfileCreator:                      null

                                   );

            emsp1EMSPAPI         = new EMSPAPI(

                                       CommonAPI:                           emsp1CommonAPI,
                                       AllowDowngrades:                     null,

                                       ExternalDNSName:                     null,
                                       HTTPServiceName:                     null,
                                       BasePath:                            null,

                                       URLPathPrefix:                       HTTPPath.Parse("/ocpi/v2.2/v2.2.1/emsp"),
                                       APIVersionHashes:                    null,

                                       IsDevelopment:                       null,
                                       DevelopmentServers:                  null,
                                       DisableLogging:                      null,
                                       LoggingPath:                         null,
                                       LogfileName:                         null,
                                       LogfileCreator:                      null

                                   );

            emsp2EMSPAPI         = new EMSPAPI(

                                       CommonAPI:                           emsp2CommonAPI,
                                       AllowDowngrades:                     null,

                                       ExternalDNSName:                     null,
                                       HTTPServiceName:                     null,
                                       BasePath:                            null,

                                       URLPathPrefix:                       HTTPPath.Parse("/ocpi/v2.2/v2.2.1/emsp"),
                                       APIVersionHashes:                    null,

                                       IsDevelopment:                       null,
                                       DevelopmentServers:                  null,
                                       DisableLogging:                      null,
                                       LoggingPath:                         null,
                                       LogfileName:                         null,
                                       LogfileCreator:                      null

                                   );

            ClassicAssert.IsNotNull(cpoCPOAPI);
            ClassicAssert.IsNotNull(emsp1EMSPAPI);
            ClassicAssert.IsNotNull(emsp2EMSPAPI);

            #endregion

            #region Add Remote Parties

            await cpoCommonAPI.AddRemoteParty  (Id:                          RemoteParty_Id.Parse(
                                                                                 emsp1CommonAPI.Parties.First().Id.CountryCode,
                                                                                 emsp1CommonAPI.Parties.First().Id.Party,
                                                                                 emsp1CommonAPI.Parties.First().Role
                                                                             ),
                                                CredentialsRoles:            [
                                                                                 new CredentialsRole(
                                                                                     CountryCode:       emsp1CommonAPI.Parties.First().Id.CountryCode,
                                                                                     PartyId:           emsp1CommonAPI.Parties.First().Id.Party,
                                                                                     Role:              Role.EMSP,
                                                                                     BusinessDetails:   emsp1CommonAPI.Parties.First().BusinessDetails,
                                                                                     AllowDowngrades:   false
                                                                                 )
                                                                             ],

                                                AccessToken:                 AccessToken.Parse("cso-2-emp1:token"),
                                                AccessStatus:                AccessStatus.ALLOWED,

                                                RemoteAccessToken:           AccessToken.Parse("emp1-2-cso:token"),
                                                RemoteVersionsURL:           URL.Parse($"http://localhost:{emsp1HTTPAPI.HTTPServer.TCPPort}/ocpi/v2.2/versions"),
                                                RemoteVersionIds:            null,
                                                AccessTokenBase64Encoding:   true,
                                                RemoteStatus:                RemoteAccessStatus.ONLINE,

                                                PartyStatus:                 PartyStatus.ENABLED);

            await cpoCommonAPI.AddRemoteParty  (Id:                          RemoteParty_Id.Parse(
                                                                                 emsp2CommonAPI.Parties.First().Id.CountryCode,
                                                                                 emsp2CommonAPI.Parties.First().Id.Party,
                                                                                 emsp2CommonAPI.Parties.First().Role
                                                                             ),
                                                CredentialsRoles:            [
                                                                                 new CredentialsRole(
                                                                                     CountryCode:       emsp2CommonAPI.Parties.First().Id.CountryCode,
                                                                                     PartyId:           emsp2CommonAPI.Parties.First().Id.Party,
                                                                                     Role:              Role.EMSP,
                                                                                     BusinessDetails:   emsp2CommonAPI.Parties.First().BusinessDetails,
                                                                                     AllowDowngrades:   false
                                                                                 )
                                                                             ],

                                                AccessToken:                 AccessToken.Parse("cso-2-emp2:token"),
                                                AccessStatus:                AccessStatus.ALLOWED,
                                                RemoteAccessToken:           AccessToken.Parse("emp2-2-cso:token"),
                                                RemoteVersionsURL:           URL.Parse($"http://localhost:{emsp2HTTPAPI.HTTPServer.TCPPort}/ocpi/v2.2/versions"),
                                                RemoteVersionIds:            null,
                                                AccessTokenBase64Encoding:   true,
                                                RemoteStatus:                RemoteAccessStatus.ONLINE,
                                                PartyStatus:                 PartyStatus.ENABLED);



            await emsp1CommonAPI.AddRemoteParty(Id:                          RemoteParty_Id.Parse(
                                                                                 cpoCommonAPI.Parties.First().Id.CountryCode,
                                                                                 cpoCommonAPI.Parties.First().Id.Party,
                                                                                 cpoCommonAPI.Parties.First().Role
                                                                             ),
                                                CredentialsRoles:            [
                                                                                 new CredentialsRole(
                                                                                     CountryCode:       cpoCommonAPI.Parties.First().Id.CountryCode,
                                                                                     PartyId:           cpoCommonAPI.Parties.First().Id.Party,
                                                                                     Role:              Role.CPO,
                                                                                     BusinessDetails:   cpoCommonAPI.Parties.First().BusinessDetails,
                                                                                     AllowDowngrades:   false
                                                                                 )
                                                                             ],

                                                AccessToken:                 AccessToken.Parse("emp1-2-cso:token"),
                                                AccessStatus:                AccessStatus.ALLOWED,

                                                RemoteAccessToken:           AccessToken.Parse("cso-2-emp1:token"),
                                                RemoteVersionsURL:           URL.Parse($"http://localhost:{cpoHTTPAPI.HTTPServer.TCPPort}/ocpi/v2.2/versions"),
                                                RemoteVersionIds:            null,
                                                AccessTokenBase64Encoding:   true,
                                                RemoteStatus:                RemoteAccessStatus.ONLINE,

                                                PartyStatus:                 PartyStatus.ENABLED);


            await emsp2CommonAPI.AddRemoteParty(Id:                          RemoteParty_Id.Parse(
                                                                                 cpoCommonAPI.Parties.First().Id.CountryCode,
                                                                                 cpoCommonAPI.Parties.First().Id.Party,
                                                                                 cpoCommonAPI.Parties.First().Role
                                                                             ),
                                                CredentialsRoles:            [
                                                                                 new CredentialsRole(
                                                                                     CountryCode:       cpoCommonAPI.Parties.First().Id.CountryCode,
                                                                                     PartyId:           cpoCommonAPI.Parties.First().Id.Party,
                                                                                     Role:              Role.CPO,
                                                                                     BusinessDetails:   cpoCommonAPI.Parties.First().BusinessDetails,
                                                                                     AllowDowngrades:   false
                                                                                 )
                                                                             ],

                                                AccessToken:                 AccessToken.Parse("emp2-2-cso:token"),
                                                AccessStatus:                AccessStatus.ALLOWED,

                                                RemoteAccessToken:           AccessToken.Parse("cso-2-emp2:token"),
                                                RemoteVersionsURL:           URL.Parse($"http://localhost:{cpoHTTPAPI.HTTPServer.TCPPort}/ocpi/v2.2/versions"),
                                                RemoteVersionIds:            null,
                                                AccessTokenBase64Encoding:   true,
                                                RemoteStatus:                RemoteAccessStatus.ONLINE,

                                                PartyStatus:                 PartyStatus.ENABLED);


            ClassicAssert.AreEqual(2, cpoCommonAPI.  RemoteParties.Count());
            ClassicAssert.AreEqual(1, emsp1CommonAPI.RemoteParties.Count());
            ClassicAssert.AreEqual(1, emsp2CommonAPI.RemoteParties.Count());

            #endregion

        }

        #endregion

        #region ShutdownEachTest()

        [TearDown]
        public virtual async Task ShutdownEachTest()
        {

            if (cpoHTTPServer   is not null)
                await cpoHTTPServer.  Stop();

            if (emsp1HTTPServer is not null)
                await emsp1HTTPServer.Stop();

            if (emsp2HTTPServer is not null)
                await emsp2HTTPServer.Stop();

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
