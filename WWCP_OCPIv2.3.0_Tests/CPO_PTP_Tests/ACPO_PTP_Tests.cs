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

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.HTTPTest;

using cloud.charging.open.protocols.OCPI;
using cloud.charging.open.protocols.OCPIv2_3_0.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_3_0.UnitTests.CPO_PTP
{

    /// <summary>
    /// OCPI CPO <=> Payment Terminal Provider test defaults.
    /// </summary>
    public abstract class ACPO_PTP_Tests
    {

        #region Data

        protected  HTTPTestServerX?  cpoHTTPServer;
        protected  HTTPExtAPIX       cpoHTTPAPI;
        protected  CommonHTTPAPI?    cpoBaseAPI;
        protected  CommonAPI?        cpoCommonAPI;
        protected  CPOAPI?           cpoAPI;

        protected  HTTPTestServerX?  ptpHTTPServer;
        protected  HTTPExtAPIX       ptpHTTPAPI;
        protected  CommonHTTPAPI?    ptpBaseAPI;
        protected  CommonAPI?        ptpCommonAPI;
        protected  PTPAPI?           ptpAPI;

        #endregion

        #region Constructor(s)

        public ACPO_PTP_Tests()
        {

        }

        #endregion


        #region SetupOnce()

        [OneTimeSetUp]
        public async virtual Task SetupOnce()
        {

            #region Create CPO APIs

            cpoHTTPServer        = new HTTPTestServerX(
                                       TCPPort:                   IPPort.Parse(3701)
                                   );

            Assert.That(cpoHTTPServer,  Is.Not.Null);


            cpoHTTPAPI           = new HTTPExtAPIX(
                                       HTTPServer:                cpoHTTPServer
                                   );

            Assert.That(cpoHTTPAPI,     Is.Not.Null);


            cpoBaseAPI           = new CommonHTTPAPI(

                                       HTTPAPI:                   cpoHTTPAPI,
                                       OurBaseURL:                URL.Parse($"http://localhost:{cpoHTTPServer.TCPPort}/ocpi"),
                                       OurVersionsURL:            URL.Parse($"http://localhost:{cpoHTTPServer.TCPPort}/ocpi/versions"),

                                       AdditionalURLPathPrefix:   null,
                                       LocationsAsOpenData:       true,
                                       AllowDowngrades:           null,

                                       ExternalDNSName:           null,
                                       HTTPServiceName:           null,//"XXX",
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

            Assert.That(cpoBaseAPI,  Is.Not.Null);

            cpoCommonAPI         = new CommonAPI(

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

                                       AdditionalURLPathPrefix:             null,
                                       KeepRemovedEVSEs:                    null,

                                       ExternalDNSName:                     null,
                                       HTTPServiceName:                     null,
                                       BasePath:                            null,

                                       URLPathPrefix:                       HTTPPath.Parse("/ocpi"),
                                       APIVersionHashes:                    null,

                                       IsDevelopment:                       null,
                                       DevelopmentServers:                  null,
                                       DisableLogging:                      null,
                                       LoggingPath:                         null,
                                       LogfileName:                         null,
                                       LogfileCreator:                      null

                                   );

            Assert.That(cpoCommonAPI,  Is.Not.Null);

            cpoAPI               = new CPOAPI(

                                       CommonAPI:                           cpoCommonAPI,
                                       AllowDowngrades:                     null,

                                       ExternalDNSName:                     null,
                                       HTTPServiceName:                     null,
                                       BasePath:                            null,

                                       //URLPathPrefix:                       HTTPPath.Parse("/ocpi/v2.3.0/cpo"),
                                       APIVersionHashes:                    null,

                                       IsDevelopment:                       null,
                                       DevelopmentServers:                  null,
                                       DisableLogging:                      null,
                                       LoggingPath:                         null,
                                       LogfileName:                         null,
                                       LogfileCreator:                      null

                                   );

            Assert.That(cpoAPI,  Is.Not.Null);

            #endregion

            #region Create PTP APIs

            ptpHTTPServer        = new HTTPTestServerX(
                                       TCPPort:                   IPPort.Parse(3801)
                                   );

            Assert.That(ptpHTTPServer,  Is.Not.Null);


            ptpHTTPAPI           = new HTTPExtAPIX(
                                       HTTPServer:                ptpHTTPServer
                                   );

            Assert.That(ptpHTTPAPI,     Is.Not.Null);


            ptpBaseAPI           = new CommonHTTPAPI(

                                       HTTPAPI:                   ptpHTTPAPI,
                                       OurBaseURL:                URL.Parse($"http://localhost:{ptpHTTPServer.TCPPort}/ocpi"),
                                       OurVersionsURL:            URL.Parse($"http://localhost:{ptpHTTPServer.TCPPort}/ocpi/versions"),

                                       AdditionalURLPathPrefix:   null,
                                       LocationsAsOpenData:       true,
                                       AllowDowngrades:           null,

                                       ExternalDNSName:           null,
                                       HTTPServiceName:           null,//"YYY",
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

            Assert.That(ptpBaseAPI,  Is.Not.Null);

            ptpCommonAPI         = new CommonAPI(

                                       OurPartyData:                        [
                                                                                new PartyData(
                                                                                    Party_Idv3.From(
                                                                                        CountryCode.Parse("DE"),
                                                                                        Party_Id.   Parse("GDF")
                                                                                    ),
                                                                                    Role.CPO,
                                                                                    new BusinessDetails(
                                                                                        "GraphDefined PTP #1 Services",
                                                                                        URL.Parse("https://www.graphdefined.com/ptp")
                                                                                    )
                                                                                )
                                                                            ],
                                       DefaultPartyId:                      Party_Idv3.From(
                                                                                CountryCode.Parse("DE"),
                                                                                Party_Id.   Parse("GDF")
                                                                            ),

                                       BaseAPI:                             ptpBaseAPI,

                                       AdditionalURLPathPrefix:             null,
                                       KeepRemovedEVSEs:                    null,

                                       ExternalDNSName:                     null,
                                       HTTPServiceName:                     "aaa",//null,
                                       BasePath:                            null,

                                       URLPathPrefix:                       HTTPPath.Parse("/ocpi"),
                                       APIVersionHashes:                    null,

                                       IsDevelopment:                       null,
                                       DevelopmentServers:                  null,
                                       DisableLogging:                      null,
                                       LoggingPath:                         null,
                                       LogfileName:                         null,
                                       LogfileCreator:                      null

                                   );

            Assert.That(ptpCommonAPI,  Is.Not.Null);

            ptpAPI               = new PTPAPI(

                                       CommonAPI:                           ptpCommonAPI,
                                       AllowDowngrades:                     null,

                                       ExternalDNSName:                     null,
                                       HTTPServiceName:                     null,
                                       BasePath:                            null,

                                       //URLPathPrefix:                       HTTPPath.Parse("/ocpi/v2.3.0/ptp"),
                                       APIVersionHashes:                    null,

                                       IsDevelopment:                       null,
                                       DevelopmentServers:                  null,
                                       DisableLogging:                      null,
                                       LoggingPath:                         null,
                                       LogfileName:                         null,
                                       LogfileCreator:                      null

                                   );

            Assert.That(ptpAPI,  Is.Not.Null);

            #endregion

            #region Add Remote Parties

            await cpoCommonAPI.AddRemoteParty(

                      Id:                          RemoteParty_Id.Parse(
                                                       ptpCommonAPI.Parties.First().Id.CountryCode,
                                                       ptpCommonAPI.Parties.First().Id.Party,
                                                       ptpCommonAPI.Parties.First().Role
                                                   ),

                      CredentialsRoles:            [
                                                       new CredentialsRole(
                                                           CountryCode:       ptpCommonAPI.Parties.First().Id.CountryCode,
                                                           PartyId:           ptpCommonAPI.Parties.First().Id.Party,
                                                           Role:              Role.PTP,
                                                           BusinessDetails:   ptpCommonAPI.Parties.First().BusinessDetails,
                                                           AllowDowngrades:   false
                                                       )
                                                   ],

                      AccessToken:                 AccessToken.Parse("cpo-2-ptp:token"),
                      AccessStatus:                AccessStatus.ALLOWED,

                      RemoteAccessToken:           AccessToken.Parse("ptp-2-cpo:token"),
                      RemoteVersionsURL:           ptpCommonAPI.BaseAPI.OurVersionsURL,
                      RemoteVersionIds:            null,
                      AccessTokenBase64Encoding:   true,
                      RemoteStatus:                RemoteAccessStatus.ONLINE,

                      PartyStatus:                 PartyStatus.ENABLED

                  );


            await ptpCommonAPI.AddRemoteParty(

                      Id:                          RemoteParty_Id.Parse(
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

                      AccessToken:                 AccessToken.Parse("ptp-2-cpo:token"),
                      AccessStatus:                AccessStatus.ALLOWED,

                      RemoteAccessToken:           AccessToken.Parse("cpo-2-ptp:token"),
                      RemoteVersionsURL:           cpoCommonAPI.BaseAPI.OurVersionsURL,
                      RemoteVersionIds:            null,
                      AccessTokenBase64Encoding:   true,
                      RemoteStatus:                RemoteAccessStatus.ONLINE,

                      PartyStatus:                 PartyStatus.ENABLED

                  );


            Assert.That(cpoCommonAPI.RemoteParties.Count(),  Is.EqualTo(1));
            Assert.That(ptpCommonAPI.RemoteParties.Count(),  Is.EqualTo(1));

            #endregion

        }

        #endregion

        #region SetupEachTest()

        [SetUp]
        public async virtual Task SetupEachTest()
        {

            Timestamp.Reset();

        }

        #endregion

        #region ShutdownEachTest()

        [TearDown]
        public async virtual Task ShutdownEachTest()
        {

            

        }

        #endregion

        #region ShutdownOnce()

        [OneTimeTearDown]
        public async virtual Task ShutdownOnce()
        {

            if (cpoHTTPServer is not null)
                await cpoHTTPServer.Stop();

            if (ptpHTTPServer is not null)
                await ptpHTTPServer.Stop();

        }

        #endregion


    }

}
