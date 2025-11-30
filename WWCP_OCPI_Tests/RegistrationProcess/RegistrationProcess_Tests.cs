/*
 * Copyright (c) 2015-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPI <https://github.com/OpenChargingCloud/WWCP_OCPI>
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
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
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPIv2_1_1;
using cloud.charging.open.protocols.OCPIv2_2_1;
using cloud.charging.open.protocols.OCPIv2_3_0;
using cloud.charging.open.protocols.OCPIv3_0;

#endregion

namespace cloud.charging.open.protocols.OCPI.UnitTests
{

    /// <summary>
    /// Testing the OCPI Registration Process.
    /// </summary>
    [TestFixture]
    public class RegistrationProcess_Tests : A_2CPOs2EMSPs_TestDefaults
    {

        #region Constructor(s)

        public RegistrationProcess_Tests()

            : base(AutoWireRemoteParties:  false)

        {

        }

        #endregion


        #region RegistrationProcess_Test1()

        /// <summary>
        /// CPO #1 asking EMSP #1 for its OCPI version details via OCPI v2.1.1!
        /// </summary>
        [Test]
        public async Task RegistrationProcess_v2_2_1_Test1()
        {

            if (cpo1CommonAPI_v2_2_1 is null)
            {
                Assert.Fail("CPO #1 CommonAPI v2.2.1 not initialized!");
                return;
            }

            if (emsp1CommonAPI_v2_2_1 is null)
            {
                Assert.Fail("EMSP #1 CommonAPI v2.2.1 not initialized!");
                return;
            }

            if (cpo1HTTPServer is null)
            {
                Assert.Fail("CPO #1 HTTP server not initialized!");
                return;
            }


            #region EMSP #1 -> CPO #1

            Assert.That(
                    await cpo1CommonAPI_v2_2_1.AddRemotePartyIfNotExists(
                              Id:                                RemoteParty_Id.Parse(
                                                                     emsp1CommonAPI_v2_2_1.DefaultPartyId,
                                                                     Role.EMSP
                                                                 ),
                              CredentialsRoles:                  [
                                                                     emsp1CommonAPI_v2_2_1.Parties.First().ToCredentialsRole()
                                                                 ],

                              LocalAccessToken:                  AccessToken.Parse(emsp1_accessing_cpo1__token),
                              LocalAccessStatus:                 AccessStatus.ALLOWED,
                              //LocalTOTPConfig:                   TOTPValidityTime.HasValue ? new TOTPConfig(emsp1_accessing_cpo1__token, TOTPValidityTime) : null,
                              LocalAccessTokenBase64Encoding:    true,

                              //RemoteAccessToken:                 AccessToken.Parse(cpo1_accessing_emsp1__token),
                              //RemoteTOTPConfig:                  TOTPValidityTime.HasValue ? new TOTPConfig(cpo1_accessing_emsp1__token, TOTPValidityTime) : null,
                              //RemoteVersionsURL:                 URL.Parse($"http://localhost:{emsp1HTTPServer.TCPPort}/ocpi/versions"),
                              //RemoteVersionIds:                  [ OCPIv2_2_1.Version.Id ],
                              //SelectedVersionId:                 OCPIv2_2_1.Version.Id,
                              //RemoteAccessTokenBase64Encoding:   true,
                              //RemoteStatus:                      RemoteAccessStatus.ONLINE,

                              //ClientCertificate:                 cpo1emsp1TLSClientKeyPair is not null
                              //                                       ? null
                              //                                       : null,

                              Status:                            PartyStatus.ENABLED
                          ),
                    Is.True
                );

            #endregion



           // var remotePartyId  = 
           //                      
           //                      
           //                      

            var addResult      = await emsp1CommonAPI_v2_2_1.AddRemoteParty(

                                           RemoteParty_Id.Parse(
                                               cpo1CommonAPI_v2_2_1.DefaultPartyId,
                                               Role.CPO
                                           ),
                                           [],

                                           RemoteVersionsURL:                 URL.Parse($"http://localhost:{cpo1HTTPServer.TCPPort}/ocpi/versions"),
                                           RemoteAccessToken:                 AccessToken.Parse(emsp1_accessing_cpo1__token),
                                           Status:                            PartyStatus.PreRegistration,

                                           RemoteAccessTokenBase64Encoding:   null,
                                           RemoteTOTPConfig:                  null,
                                           RemoteAccessNotBefore:             null,
                                           RemoteAccessNotAfter:              null,
                                           RemoteStatus:                      null,
                                           RemoteVersionIds:                  null,
                                           SelectedVersionId:                 null,
                                           RemoteAllowDowngrades:             null,

                                           PreferIPv4:                        null,
                                           RemoteCertificateValidator:        null,
                                           LocalCertificateSelector:          null,
                                           ClientCertificates:                null,
                                           ClientCertificateContext:          null,
                                           ClientCertificateChain:            null,
                                           TLSProtocols:                      null,
                                           ContentType:                       null,
                                           Accept:                            null,
                                           HTTPUserAgent:                     null,
                                           RequestTimeout:                    null,
                                           TransmissionRetryDelay:            null,
                                           MaxNumberOfRetries:                null,
                                           InternalBufferSize:                null,
                                           UseHTTPPipelining:                 null,

                                           Created:                           null,
                                           LastUpdated:                       null

                                       );

            Assert.That(addResult.IsSuccess,  Is.True);

            //     var commonClient = new OCPIv2_2_1.CommonClient(
            //
            //                            emsp1CommonAPI_v2_2_1,
            //                            RemoteParty_Id.Parse(
            //                                cpo1CommonAPI_v2_2_1.DefaultPartyId,
            //                                Role.CPO
            //                            ),
            //
            //                            RemoteVersionsURL:                 URL.Parse($"http://localhost:{cpo1HTTPServer.TCPPort}/ocpi/versions"),
            //                            RemoteAccessToken:                 AccessToken.Parse(emsp1_accessing_cpo1__token),
            //                            RemoteAccessTokenBase64Encoding:   true,
            //                            TOTPConfig:                        null,
            //
            //                            VirtualHostname:                   null,
            //                            Description:                       null,
            //                            PreferIPv4:                        null,
            //                            RemoteCertificateValidator:        null,
            //                            LocalCertificateSelector:          null,
            //                            ClientCertificate:                 null,
            //                            TLSProtocols:                      null,
            //                            ContentType:                       null,
            //                            Accept:                            null,
            //                            HTTPUserAgent:                     null,
            //                            RequestTimeout:                    null,
            //                            TransmissionRetryDelay:            null,
            //                            MaxNumberOfRetries:                null,
            //                            InternalBufferSize:                null,
            //                            UseHTTPPipelining:                 null,
            //                            HTTPLogger:                        null,
            //
            //                            DisableLogging:                    null,
            //                            LoggingPath:                       null,
            //                            LoggingContext:                    null,
            //                            LogfileCreator:                    null,
            //                            DNSClient:                         null
            //
            //                        );

            var remoteParty   = addResult.Data;
            Assert.That(remoteParty,   Is.Not.Null);

            var commonClient  = emsp1EMSPAPI_v2_2_1?.GetCPOClient(remoteParty!.Id);
            Assert.That(commonClient,  Is.Not.Null);

            var response      = await commonClient!.Register(
                                          VersionId:  OCPIv2_2_1.Version.Id
                                      );

            Assert.That(response,                                                       Is.Not.Null);
            Assert.That(response.HTTPResponse?.HTTPStatusCode.Code,                     Is.EqualTo(200), response.HTTPResponse?.HTTPBodyAsUTF8String);
            Assert.That(response.StatusCode,                                            Is.EqualTo(1000));
            Assert.That(response.StatusMessage,                                         Is.EqualTo("Hello world!"));
            Assert.That(Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10),  Is.True);

            if (response.Data is not null)
            {

                //Assert.That(response.Data.VersionId,                                        Is.EqualTo(OCPIv2_1_1.Version.Id));

                //var endpoints = response.Data.Endpoints.ToDictionary(endpoint => endpoint.Identifier);

                //Assert.That(endpoints,                                                      Is.Not.Null);
                //Assert.That(endpoints.Count,                                                Is.EqualTo(7));

                //Assert.That(endpoints[OCPIv2_1_1.Module_Id.Credentials].URL,                Is.EqualTo(URL.Parse("http://localhost:3401/ocpi/v2.1.1/credentials")));
                //Assert.That(endpoints[OCPIv2_1_1.Module_Id.Locations].  URL,                Is.EqualTo(URL.Parse("http://localhost:3401/ocpi/v2.1.1/emsp/locations")));
                //Assert.That(endpoints[OCPIv2_1_1.Module_Id.Tariffs].    URL,                Is.EqualTo(URL.Parse("http://localhost:3401/ocpi/v2.1.1/emsp/tariffs")));
                //Assert.That(endpoints[OCPIv2_1_1.Module_Id.Sessions].   URL,                Is.EqualTo(URL.Parse("http://localhost:3401/ocpi/v2.1.1/emsp/sessions")));
                //Assert.That(endpoints[OCPIv2_1_1.Module_Id.CDRs].       URL,                Is.EqualTo(URL.Parse("http://localhost:3401/ocpi/v2.1.1/emsp/cdrs")));
                //Assert.That(endpoints[OCPIv2_1_1.Module_Id.Commands].   URL,                Is.EqualTo(URL.Parse("http://localhost:3401/ocpi/v2.1.1/emsp/commands")));
                //Assert.That(endpoints[OCPIv2_1_1.Module_Id.Tokens].     URL,                Is.EqualTo(URL.Parse("http://localhost:3401/ocpi/v2.1.1/emsp/tokens")));

            }









                //,
                //new RemoteParty(
                //    RemoteParty_Id.Parse(""),
                //    [
                //        new CredentialsRole(
                //            cpo1CommonAPI_v2_2_1.DefaultPartyId.CountryCode,
                //            cpo1CommonAPI_v2_2_1.DefaultPartyId.Party,
                //            //CountryCode.Parse("DE"),
                //            //Party_Id.   Parse("GEF"),
                //            Role.CPO,
                //            emsp1CommonAPI_v2_2_1.Parties.First().BusinessDetails
                //        )
                //    ],
                //    new LocalAccessInfo[0],
                //    new RemoteAccessInfo[0],


                //    )



            //var graphDefinedEMSP1 = cpo1CPOAPI_v2_2_1?.GetEMSPClient(
            //                            CountryCode: CountryCode.Parse("DE"),
            //                            PartyId:     Party_Id.   Parse("GDF")
            //                        );

            //Assert.That(graphDefinedEMSP1, Is.Not.Null);

            //if (graphDefinedEMSP1 is not null)
            //{

            //    var response = await graphDefinedEMSP1.GetVersionDetails(OCPIv2_2_1.Version.Id);

            //    // GET /ocpi/versions/2.1.1 HTTP/1.1
            //    // Accept:                        application/json; charset=utf-8; q=1
            //    // Host:                          localhost:3401
            //    // User-Agent:                    GraphDefined OCPI v2.1.1 CommonClient
            //    // Authorization:                 Token cpo1_accessing_emsp1++token
            //    // Connection:                    close
            //    // X-Request-ID:                  S1Q64pQ9nf24C3tA64xjhvWnz983jt
            //    // X-Correlation-ID:              945f2bEj63npK7fMSU48UdfnCt8722

            //    // HTTP/1.1 200 OK
            //    // Date:                          Fri, 10 Jan 2025 06:01:52 GMT
            //    // Server:                        GraphDefined OCPI v2.1.1 Common HTTP API
            //    // Access-Control-Allow-Origin:   *
            //    // Access-Control-Allow-Methods:  OPTIONS, GET
            //    // Access-Control-Allow-Headers:  Authorization
            //    // Allow:                         OPTIONS, GET
            //    // Vary:                          Accept
            //    // Content-Type:                  application/json; charset=utf-8
            //    // Content-Length:                695
            //    // X-Request-ID:                  S1Q64pQ9nf24C3tA64xjhvWnz983jt
            //    // X-Correlation-ID:              945f2bEj63npK7fMSU48UdfnCt8722
            //    // 
            //    // {
            //    //     "data": {
            //    //         "version": "2.1.1",
            //    //         "endpoints": [
            //    //             {
            //    //                 "identifier": "credentials",
            //    //                 "url":        "http://localhost:3401/ocpi/v2.1.1/credentials"
            //    //             },
            //    //             {
            //    //                 "identifier": "locations",
            //    //                 "url":        "http://localhost:3401/ocpi/v2.1.1/emsp/locations"
            //    //             },
            //    //             {
            //    //                 "identifier": "tariffs",
            //    //                 "url":        "http://localhost:3401/ocpi/v2.1.1/emsp/tariffs"
            //    //             },
            //    //             {
            //    //                 "identifier": "sessions",
            //    //                 "url":        "http://localhost:3401/ocpi/v2.1.1/emsp/sessions"
            //    //             },
            //    //             {
            //    //                 "identifier": "cdrs",
            //    //                 "url":        "http://localhost:3401/ocpi/v2.1.1/emsp/cdrs"
            //    //             },
            //    //             {
            //    //                 "identifier": "commands",
            //    //                 "url":        "http://localhost:3401/ocpi/v2.1.1/emsp/commands"
            //    //             },
            //    //             {
            //    //                 "identifier": "tokens",
            //    //                 "url":        "http://localhost:3401/ocpi/v2.1.1/emsp/tokens"
            //    //             }
            //    //         ]
            //    //     },
            //    //     "status_code":     1000,
            //    //     "status_message": "Hello world!",
            //    //     "timestamp":      "2025-01-10T06:01:52.727Z"
            //    // }

            //    Assert.That(response,                                                       Is.Not.Null);
            //    Assert.That(response.HTTPResponse?.HTTPStatusCode.Code,                     Is.EqualTo(200), response.HTTPResponse?.HTTPBodyAsUTF8String);
            //    Assert.That(response.StatusCode,                                            Is.EqualTo(1000));
            //    Assert.That(response.StatusMessage,                                         Is.EqualTo("Hello world!"));
            //    Assert.That(Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10),  Is.True);

            //    if (response.Data is not null)
            //    {

            //        Assert.That(response.Data.VersionId,                                        Is.EqualTo(OCPIv2_1_1.Version.Id));

            //        var endpoints = response.Data.Endpoints.ToDictionary(endpoint => endpoint.Identifier);

            //        //Assert.That(endpoints,                                                      Is.Not.Null);
            //        //Assert.That(endpoints.Count,                                                Is.EqualTo(7));

            //        //Assert.That(endpoints[OCPIv2_1_1.Module_Id.Credentials].URL,                Is.EqualTo(URL.Parse("http://localhost:3401/ocpi/v2.1.1/credentials")));
            //        //Assert.That(endpoints[OCPIv2_1_1.Module_Id.Locations].  URL,                Is.EqualTo(URL.Parse("http://localhost:3401/ocpi/v2.1.1/emsp/locations")));
            //        //Assert.That(endpoints[OCPIv2_1_1.Module_Id.Tariffs].    URL,                Is.EqualTo(URL.Parse("http://localhost:3401/ocpi/v2.1.1/emsp/tariffs")));
            //        //Assert.That(endpoints[OCPIv2_1_1.Module_Id.Sessions].   URL,                Is.EqualTo(URL.Parse("http://localhost:3401/ocpi/v2.1.1/emsp/sessions")));
            //        //Assert.That(endpoints[OCPIv2_1_1.Module_Id.CDRs].       URL,                Is.EqualTo(URL.Parse("http://localhost:3401/ocpi/v2.1.1/emsp/cdrs")));
            //        //Assert.That(endpoints[OCPIv2_1_1.Module_Id.Commands].   URL,                Is.EqualTo(URL.Parse("http://localhost:3401/ocpi/v2.1.1/emsp/commands")));
            //        //Assert.That(endpoints[OCPIv2_1_1.Module_Id.Tokens].     URL,                Is.EqualTo(URL.Parse("http://localhost:3401/ocpi/v2.1.1/emsp/tokens")));

            //    }

            //}

        }

        #endregion


    }

}
