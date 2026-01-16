/*
 * Copyright (c) 2015-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

using Newtonsoft.Json.Linq;

using NUnit.Framework;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.PKI;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.HTTPTest;
using org.GraphDefined.Vanaheimr.Hermod.Logging;

using cloud.charging.open.protocols.OCPI;
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
    public class RegistrationProcessTLSv1_2ClientCertificates_Tests()

        : A_2CPOs2EMSPs_TestDefaults(

              // This will enable client certificates for TLS authentication!
              AllowedTLSProtocols:     SslProtocols.Tls12,
              ECCAlgorithm:           "secp256r1",
              AutoWireRemoteParties:   false

          )

    {

        #region RegistrationProcess_TLSv1_2__v2_2_1_Test1()

        /// <summary>
        /// EMSP #1 starting a registration process with CPO #1 via OCPI v2.2.1!
        /// </summary>
        [Test]
        public async Task RegistrationProcess_TLSv1_2__v2_2_1_Test1()
        {

            #region Setup

            if (cpo1CommonAPI_v2_2_1 is null)
            {
                Assert.Fail("CPO #1 CommonAPI v2.2.1 not initialized!");
                return;
            }

            if (cpo1CPOAPI_v2_2_1    is null)
            {
                Assert.Fail("CPO #1 CPO API v2.2.1 not initialized!");
                return;
            }

            if (cpo1HTTPServer        is null)
            {
                Assert.Fail("CPO #1 HTTP server not initialized!");
                return;
            }

            await cpo1CommonAPI_v2_2_1. RemoveAllRemoteParties();
            await cpo1CPOAPI_v2_2_1.    CloseAllClients();


            if (emsp1CommonAPI_v2_2_1 is null)
            {
                Assert.Fail("EMSP #1 CommonAPI v2.2.1 not initialized!");
                return;
            }

            if (emsp1EMSPAPI_v2_2_1   is null)
            {
                Assert.Fail("EMSP #1 EMSP API v2.2.1 not initialized!");
                return;
            }

            await emsp1CommonAPI_v2_2_1.RemoveAllRemoteParties();
            await emsp1EMSPAPI_v2_2_1.  CloseAllClients();

            #endregion

            #region CPO #1  -> EMSP #1

            var cpo1_2_emsp1_v2_2_1 = await cpo1CommonAPI_v2_2_1.AddRemotePartyIfNotExists(

                                                Id:                                RemoteParty_Id.From(
                                                                                       emsp1CommonAPI_v2_2_1.DefaultPartyId,
                                                                                       Role.EMSP
                                                                                   ),
                                                CredentialsRoles:                  [
                                                                                       emsp1CommonAPI_v2_2_1.Parties.First().ToCredentialsRole()
                                                                                   ],

                                                LocalAccessToken:                  AccessToken.Parse(emsp1_accessing_cpo1__token),
                                                LocalAccessStatus:                 AccessStatus.ALLOWED,
                                                LocalTOTPConfig:                   TOTPValidityTime.HasValue ? new TOTPConfig(emsp1_accessing_cpo1__token, TOTPValidityTime) : null,
                                                LocalAccessTokenBase64Encoding:    true,

                                                //RemoteAccessToken:                 AccessToken.Parse(cpo1_accessing_emsp1__token),
                                                //RemoteTOTPConfig:                  TOTPValidityTime.HasValue ? new TOTPConfig(cpo1_accessing_emsp1__token, TOTPValidityTime) : null,
                                                //RemoteVersionsURL:                 URL.Parse($"http{(emsp1TLSServerCertificate is not null ? "s" : "")}://localhost:{emsp1HTTPServer.TCPPort}/ocpi/versions"),
                                                //RemoteVersionIds:                  [ OCPIv2_2_1.Version.Id ],
                                                //SelectedVersionId:                 OCPIv2_2_1.Version.Id,
                                                //RemoteAccessTokenBase64Encoding:   true,
                                                //RemoteStatus:                      RemoteAccessStatus.ONLINE,

                                                RemoteCertificateValidator:        emsp1TLSServerCertificate is not null
                                                                                       ? (sender, serverCertificate, certificateChain, tlsClient, sslPolicyErrors) => {

                                                                                             if (serverCertificate is null)
                                                                                                 return (false, [ "The server certificate is null!" ]);

                                                                                             var chainReport = PKIFactory.ValidateServerChain(
                                                                                                                   serverCertificate,
                                                                                                                   serverCACertificate2!,
                                                                                                                   rootCACertificate2!
                                                                                                               );

                                                                                             return (chainReport.IsValid,
                                                                                                     chainReport.Status.Select(chainStatus => chainStatus.Status.ToString()));

                                                                                       }
                                                                                       : null,
                                                LocalCertificateSelector:          null,
                                                ClientCertificates:                cpo1emsp1TLSClientCertificate is not null
                                                                                       ? [ cpo1emsp1TLSClientCertificate ]
                                                                                       : null,
                                                ClientCertificateContext:          cpo1emsp1TLSClientCertificate is not null && clientCACertificate2 is not null && UseClientCertificateChains
                                                                                       ? SslStreamCertificateContext.Create(
                                                                                             cpo1emsp1TLSClientCertificate,
                                                                                             new X509Certificate2Collection(clientCACertificate2)
                                                                                         )
                                                                                       : null,
                                                ClientCertificateChain:            null,
                                                TLSProtocols:                      SslProtocols.Tls12,

                                                Status:                            PartyStatus.ENABLED

                                            );

            Assert.That(cpo1_2_emsp1_v2_2_1.IsSuccess,  Is.True);
            Assert.That(cpo1_2_emsp1_v2_2_1.Data,       Is.Not.Null);

            var aa = cpo1_2_emsp1_v2_2_1.Data.ToJSON(false);
            var a1 = (aa["remoteAccessInfos"]  as JArray).First();
            var a2 = (a1["clientCertificates"] as JArray).First()?.Value<String>();
            Assert.That(a2.Contains("-----BEGIN CERTIFICATE-----"),  Is.True);
            Assert.That(a2.Contains("-----BEGIN PRIVATE KEY-----"),  Is.True);

            var bb = RemoteParty.Parse(aa);
            Assert.That(bb.RemoteAccessInfos.First().TLSProtocols,                              Is.EqualTo(SslProtocols.Tls12));
            Assert.That(bb.RemoteAccessInfos.First().ClientCertificates.First(),                Is.Not.Null);
            Assert.That(bb.RemoteAccessInfos.First().ClientCertificates.First().HasPrivateKey,  Is.True);

            var cc = 1;

            #endregion

            #region EMSP #1 -> CPO #1

            var emsp1_2_cpo1_v2_2_1 = await emsp1CommonAPI_v2_2_1.AddRemotePartyIfNotExists(

                                                Id:                                RemoteParty_Id.From(
                                                                                       cpo1CommonAPI_v2_2_1.DefaultPartyId,
                                                                                       Role.CPO
                                                                                   ),
                                                CredentialsRoles:                  [
                                                                                        cpo1CommonAPI_v2_2_1.Parties.First().ToCredentialsRole()
                                                                                   ],
                                                //LocalAccessToken:                  AccessToken.Parse(cpo1_accessing_emsp1__token),
                                                //LocalAccessStatus:                 AccessStatus.ALLOWED,
                                                //LocalTOTPConfig:                   TOTPValidityTime.HasValue ? new TOTPConfig(cpo1_accessing_emsp1__token, TOTPValidityTime) : null,
                                                //LocalAccessTokenBase64Encoding:    true,

                                                RemoteAccessToken:                 AccessToken.Parse(emsp1_accessing_cpo1__token),
                                                RemoteTOTPConfig:                  TOTPValidityTime.HasValue ? new TOTPConfig(emsp1_accessing_cpo1__token, TOTPValidityTime) : null,
                                                RemoteVersionsURL:                 URL.Parse($"http{(cpo1TLSServerCertificate is not null ? "s" : "")}://localhost:{cpo1HTTPServer.TCPPort}/ocpi/versions"),
                                                RemoteVersionIds:                  [ OCPIv2_2_1.Version.Id ],
                                                SelectedVersionId:                 OCPIv2_2_1.Version.Id,
                                                RemoteAccessTokenBase64Encoding:   true,
                                                RemoteStatus:                      RemoteAccessStatus.ONLINE,

                                                RemoteCertificateValidator:        cpo1TLSServerCertificate is not null
                                                                                        ? (sender, serverCertificate, certificateChain, tlsClient, sslPolicyErrors) => {

                                                                                                if (serverCertificate is null)
                                                                                                    return (false, [ "The server certificate is null!" ]);

                                                                                                var chainReport = PKIFactory.ValidateServerChain(
                                                                                                                    serverCertificate,
                                                                                                                    serverCACertificate2!,
                                                                                                                    rootCACertificate2!
                                                                                                                );

                                                                                                return (chainReport.IsValid,
                                                                                                        chainReport.Status.Select(chainStatus => chainStatus.Status.ToString()));

                                                                                        }
                                                                                        : null,
                                                LocalCertificateSelector:          null,
                                                ClientCertificates:                emsp1cpo1TLSClientCertificate is not null
                                                                                        ? [ emsp1cpo1TLSClientCertificate ]
                                                                                        : null,
                                                ClientCertificateContext:          emsp1cpo1TLSClientCertificate is not null && clientCACertificate2 is not null && UseClientCertificateChains
                                                                                        ? SslStreamCertificateContext.Create(
                                                                                                emsp1cpo1TLSClientCertificate,
                                                                                                new X509Certificate2Collection(clientCACertificate2)
                                                                                            )
                                                                                        : null,
                                                ClientCertificateChain:            null,
                                                TLSProtocols:                      SslProtocols.Tls12,

                                                Status:                            PartyStatus.ENABLED

                                            );

            Assert.That(emsp1_2_cpo1_v2_2_1.IsSuccess,  Is.True);
            Assert.That(emsp1_2_cpo1_v2_2_1.Data,       Is.Not.Null);

            #endregion


            var cpoClient = emsp1EMSPAPI_v2_2_1?.GetCPOClient(
                                cpo1CommonAPI_v2_2_1.DefaultPartyId.CountryCode,
                                cpo1CommonAPI_v2_2_1.DefaultPartyId.Party
                            );

            Assert.That(cpoClient, Is.Not.Null);

            if (cpoClient is not null)
            {

                var response = await cpoClient.Register(
                                         VersionId:  OCPIv2_2_1.Version.Id
                                     );

                Assert.That(response,                                                       Is.Not.Null);
                Assert.That(response.HTTPResponse?.HTTPStatusCode.Code,                     Is.EqualTo(200),  response.HTTPResponse?.HTTPBodyAsUTF8String);
                Assert.That(response.StatusCode,                                            Is.EqualTo(1000), response.StatusMessage);
                Assert.That(response.StatusMessage,                                         Is.EqualTo("Hello world!"));
                Assert.That(Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10),  Is.True, "The response was too slow!");

                Assert.That(response.Data,                                                  Is.Not.Null);
                if (response.Data is not null)
                {
                    //Assert.That(response.Data.Roles,                                        Is.EqualTo(OCPIv2_1_1.Version.Id));
                    Assert.That(response.Data.URL.  ToString(),                             Is.EqualTo($"https://localhost:{cpo1HTTPServer.TCPPort}/ocpi/versions"));
                    //Assert.That(response.Data.Token.ToString(),                             Is.EqualTo(OCPIv2_1_1.Version.Id));
                }

            }

        }

        #endregion

        #region RegistrationProcess_TLSv1_3__v2_2_1_Test1()

        /// <summary>
        /// EMSP #1 starting a registration process with CPO #1 via OCPI v2.2.1
        /// using TLSv1.3 and theirfore will fail!
        /// </summary>
        [Test]
        public async Task RegistrationProcess_TLSv1_3__v2_2_1_Test1()
        {

            #region Setup

            if (cpo1CommonAPI_v2_2_1 is null)
            {
                Assert.Fail("CPO #1 CommonAPI v2.2.1 not initialized!");
                return;
            }

            if (cpo1CPOAPI_v2_2_1    is null)
            {
                Assert.Fail("CPO #1 CPO API v2.2.1 not initialized!");
                return;
            }

            if (cpo1HTTPServer        is null)
            {
                Assert.Fail("CPO #1 HTTP server not initialized!");
                return;
            }

            await cpo1CommonAPI_v2_2_1. RemoveAllRemoteParties();
            await cpo1CPOAPI_v2_2_1.    CloseAllClients();


            if (emsp1CommonAPI_v2_2_1 is null)
            {
                Assert.Fail("EMSP #1 CommonAPI v2.2.1 not initialized!");
                return;
            }

            if (emsp1EMSPAPI_v2_2_1   is null)
            {
                Assert.Fail("EMSP #1 EMSP API v2.2.1 not initialized!");
                return;
            }

            await emsp1CommonAPI_v2_2_1.RemoveAllRemoteParties();
            await emsp1EMSPAPI_v2_2_1.  CloseAllClients();

            #endregion

            #region CPO #1  -> EMSP #1

            var cpo1_2_emsp1_v2_2_1 = await cpo1CommonAPI_v2_2_1.AddRemotePartyIfNotExists(

                                                Id:                                RemoteParty_Id.From(
                                                                                       emsp1CommonAPI_v2_2_1.DefaultPartyId,
                                                                                       Role.EMSP
                                                                                   ),
                                                CredentialsRoles:                  [
                                                                                       emsp1CommonAPI_v2_2_1.Parties.First().ToCredentialsRole()
                                                                                   ],

                                                LocalAccessToken:                  AccessToken.Parse(emsp1_accessing_cpo1__token),
                                                LocalAccessStatus:                 AccessStatus.ALLOWED,
                                                LocalTOTPConfig:                   TOTPValidityTime.HasValue ? new TOTPConfig(emsp1_accessing_cpo1__token, TOTPValidityTime) : null,
                                                LocalAccessTokenBase64Encoding:    true,

                                                //RemoteAccessToken:                 AccessToken.Parse(cpo1_accessing_emsp1__token),
                                                //RemoteTOTPConfig:                  TOTPValidityTime.HasValue ? new TOTPConfig(cpo1_accessing_emsp1__token, TOTPValidityTime) : null,
                                                //RemoteVersionsURL:                 URL.Parse($"http{(emsp1TLSServerCertificate is not null ? "s" : "")}://localhost:{emsp1HTTPServer.TCPPort}/ocpi/versions"),
                                                //RemoteVersionIds:                  [ OCPIv2_2_1.Version.Id ],
                                                //SelectedVersionId:                 OCPIv2_2_1.Version.Id,
                                                //RemoteAccessTokenBase64Encoding:   true,
                                                //RemoteStatus:                      RemoteAccessStatus.ONLINE,

                                                RemoteCertificateValidator:        emsp1TLSServerCertificate is not null
                                                                                       ? (sender, serverCertificate, certificateChain, tlsClient, sslPolicyErrors) => {

                                                                                             if (serverCertificate is null)
                                                                                                 return (false, [ "The server certificate is null!" ]);

                                                                                             var chainReport = PKIFactory.ValidateServerChain(
                                                                                                                   serverCertificate,
                                                                                                                   serverCACertificate2!,
                                                                                                                   rootCACertificate2!
                                                                                                               );

                                                                                             return (chainReport.IsValid,
                                                                                                     chainReport.Status.Select(chainStatus => chainStatus.Status.ToString()));

                                                                                       }
                                                                                       : null,
                                                LocalCertificateSelector:          null,
                                                ClientCertificates:                cpo1emsp1TLSClientCertificate is not null
                                                                                       ? [ cpo1emsp1TLSClientCertificate ]
                                                                                       : null,
                                                ClientCertificateContext:          cpo1emsp1TLSClientCertificate is not null && clientCACertificate2 is not null && UseClientCertificateChains
                                                                                       ? SslStreamCertificateContext.Create(
                                                                                             cpo1emsp1TLSClientCertificate,
                                                                                             new X509Certificate2Collection(clientCACertificate2)
                                                                                         )
                                                                                       : null,
                                                ClientCertificateChain:            null,
                                                TLSProtocols:                      SslProtocols.Tls13,

                                                Status:                            PartyStatus.ENABLED

                                            );

            Assert.That(cpo1_2_emsp1_v2_2_1.IsSuccess,  Is.True);
            Assert.That(cpo1_2_emsp1_v2_2_1.Data,       Is.Not.Null);

            #endregion

            #region EMSP #1 -> CPO #1

            var emsp1_2_cpo1_v2_2_1 = await emsp1CommonAPI_v2_2_1.AddRemotePartyIfNotExists(

                                                Id:                                RemoteParty_Id.From(
                                                                                       cpo1CommonAPI_v2_2_1.DefaultPartyId,
                                                                                       Role.CPO
                                                                                   ),
                                                CredentialsRoles:                  [
                                                                                        cpo1CommonAPI_v2_2_1.Parties.First().ToCredentialsRole()
                                                                                   ],
                                                //LocalAccessToken:                  AccessToken.Parse(cpo1_accessing_emsp1__token),
                                                //LocalAccessStatus:                 AccessStatus.ALLOWED,
                                                //LocalTOTPConfig:                   TOTPValidityTime.HasValue ? new TOTPConfig(cpo1_accessing_emsp1__token, TOTPValidityTime) : null,
                                                //LocalAccessTokenBase64Encoding:    true,

                                                RemoteAccessToken:                 AccessToken.Parse(emsp1_accessing_cpo1__token),
                                                RemoteTOTPConfig:                  TOTPValidityTime.HasValue ? new TOTPConfig(emsp1_accessing_cpo1__token, TOTPValidityTime) : null,
                                                RemoteVersionsURL:                 URL.Parse($"http{(cpo1TLSServerCertificate is not null ? "s" : "")}://localhost:{cpo1HTTPServer.TCPPort}/ocpi/versions"),
                                                RemoteVersionIds:                  [ OCPIv2_2_1.Version.Id ],
                                                SelectedVersionId:                 OCPIv2_2_1.Version.Id,
                                                RemoteAccessTokenBase64Encoding:   true,
                                                RemoteStatus:                      RemoteAccessStatus.ONLINE,

                                                RemoteCertificateValidator:        cpo1TLSServerCertificate is not null
                                                                                        ? (sender, serverCertificate, certificateChain, tlsClient, sslPolicyErrors) => {

                                                                                                if (serverCertificate is null)
                                                                                                    return (false, [ "The server certificate is null!" ]);

                                                                                                var chainReport = PKIFactory.ValidateServerChain(
                                                                                                                    serverCertificate,
                                                                                                                    serverCACertificate2!,
                                                                                                                    rootCACertificate2!
                                                                                                                );

                                                                                                return (chainReport.IsValid,
                                                                                                        chainReport.Status.Select(chainStatus => chainStatus.Status.ToString()));

                                                                                        }
                                                                                        : null,
                                                LocalCertificateSelector:          null,
                                                ClientCertificates:                emsp1cpo1TLSClientCertificate is not null
                                                                                        ? [ emsp1cpo1TLSClientCertificate ]
                                                                                        : null,
                                                ClientCertificateContext:          emsp1cpo1TLSClientCertificate is not null && clientCACertificate2 is not null && UseClientCertificateChains
                                                                                        ? SslStreamCertificateContext.Create(
                                                                                                emsp1cpo1TLSClientCertificate,
                                                                                                new X509Certificate2Collection(clientCACertificate2)
                                                                                            )
                                                                                        : null,
                                                ClientCertificateChain:            null,
                                                TLSProtocols:                      SslProtocols.Tls13,

                                                Status:                            PartyStatus.ENABLED

                                            );

            Assert.That(emsp1_2_cpo1_v2_2_1.IsSuccess,  Is.True);
            Assert.That(emsp1_2_cpo1_v2_2_1.Data,       Is.Not.Null);

            #endregion


            var cpoClient = emsp1EMSPAPI_v2_2_1?.GetCPOClient(
                                cpo1CommonAPI_v2_2_1.DefaultPartyId.CountryCode,
                                cpo1CommonAPI_v2_2_1.DefaultPartyId.Party
                            );

            Assert.That(cpoClient, Is.Not.Null);

            if (cpoClient is not null)
            {

                var response = await cpoClient.Register(
                                         VersionId:  OCPIv2_2_1.Version.Id
                                     );

                Assert.That(response,                                                       Is.Not.Null);
                Assert.That(response.HTTPResponse?.HTTPStatusCode.Code,                     Is.Null);
                Assert.That(response.StatusCode,                                            Is.EqualTo(-1));
                Assert.That(response.StatusMessage,                                         Is.EqualTo("No remote URL available!"));
                Assert.That(response.Data,                                                  Is.Null);
                Assert.That(Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10),  Is.True, "The response was too slow!");

            }

        }

        #endregion

    }

}
