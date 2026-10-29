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

using System.Net.Sockets;

using Newtonsoft.Json.Linq;

using NUnit.Framework;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv3_0.UnitTests.CommonTests
{

    /// <summary>
    /// A 3.0 platform as the other side finds it: in the versions list every
    /// version hangs off, with its peers' tokens known there, and with
    /// credentials that name the parties it was made with.
    /// </summary>
    /// <remarks>
    /// The Common API of 3.0 kept its version and its peers to itself, and
    /// left the parties it was made with out: the versions list did not list
    /// it and knew no 3.0 peer's token, and its credentials could not be made
    /// unless a party was added by hand.
    /// </remarks>
    [TestFixture]
    public class VersionsListTests
    {

        #region Data

        private static readonly RemoteParty_Id  id       = RemoteParty_Id.Parse("DE-BBB_EMSP");
        private static readonly AccessToken     token    = AccessToken.Parse("their-token-of-ours");
        private static readonly AccessToken     unknown  = AccessToken.Parse("a-token-of-nobody-here");

        private String       directory  = default!;
        private CommonAPI    api        = default!;
        private HTTPServer?  server;
        private UInt16       port;

        #endregion

        #region SetUp / TearDown

        [SetUp]
        public async Task SetUp()
        {

            directory  = Path.Combine(Path.GetTempPath(), $"WWCP_OCPI_Tests-{Guid.NewGuid():N}");

            Directory.CreateDirectory(directory);

            for (var attempt = 1; ; attempt++)
            {

                port    = FreePort();
                server  = new HTTPServer(IPAddress: IPv4Address.Localhost, TCPPort: IPPort.Parse(port));
                api     = ACommonAPI(server, port);

                try
                {
                    await server.Start();
                    break;
                }
                catch (SocketException) when (attempt < 5)
                { }

            }

            var added = await api.AddRemoteParty(id,
                                                [
                                                    new CredentialsRole(
                                                        CountryCode.Parse("DE"),
                                                        Party_Id.   Parse("BBB"),
                                                        Role.EMSP,
                                                        new BusinessDetails("Their EMSP")
                                                    )
                                                ],
                                                token,
                                                LocalAccessTokenBase64Encoding: false);

            Assert.That(added.IsSuccess, Is.True, $"The other side could not be added: {added.ErrorResponse}");

        }

        [TearDown]
        public async Task TearDown()
        {

            if (server is not null)
                await server.Stop();

            try
            {
                Directory.Delete(directory, true);
            }
            catch (IOException)
            { }
            catch (UnauthorizedAccessException)
            { }

        }

        #endregion


        #region TheVersionsListListsThisVersionToItsPeer()

        /// <summary>
        /// A peer of this 3.0 platform is let in at the versions list with its
        /// token, finds 3.0 in it, and the details of 3.0 where it says.
        /// </summary>
        [Test]
        public async Task TheVersionsListListsThisVersionToItsPeer()
        {

            var versions  = await Send($"http://127.0.0.1:{port}/ocpi/versions", $"Token {token}");
            var details   = (versions.JSON?["data"] as JArray)?.FirstOrDefault(version => version.Value<String>("version") == "3.0")?.Value<String>("url");

            Assert.Multiple(() => {
                Assert.That(versions.Status,                               Is.EqualTo(200),   versions.Text);
                Assert.That(versions.JSON?.Value<Int32?>("status_code"),   Is.EqualTo(1000),  versions.Text);
                Assert.That(details,                                       Does.EndWith($":{port}/ocpi/v3.0/versions/3.0"), $"The versions list does not list 3.0 where it is: {versions.Text}");
            });

            var version = await Send(details!, $"Token {token}");

            Assert.Multiple(() => {
                Assert.That(version.Status,                                Is.EqualTo(200),   version.Text);
                Assert.That(version.JSON?.Value<Int32?>("status_code"),    Is.EqualTo(1000),  version.Text);
            });

        }

        #endregion

        #region TheVersionsListRefusesAnUnknownToken()

        /// <summary>
        /// A token of nobody here is refused at the versions list as before:
        /// that a 3.0 peer's token is known there does not open it to others.
        /// </summary>
        [Test]
        public async Task TheVersionsListRefusesAnUnknownToken()
        {

            var versions = await Send($"http://127.0.0.1:{port}/ocpi/versions", $"Token {unknown}");

            Assert.Multiple(() => {
                Assert.That(versions.Status,                                   Is.EqualTo(401),                     versions.Text);
                Assert.That(versions.JSON?.Value<String>("status_message"),    Is.EqualTo("Unknown access token!"), versions.Text);
            });

        }

        #endregion

        #region TheCredentialsNameThePartiesThisPlatformWasMadeWith()

        /// <summary>
        /// The credentials name the party this platform was made with, none
        /// added by hand.
        /// </summary>
        [Test]
        public async Task TheCredentialsNameThePartiesThisPlatformWasMadeWith()
        {

            var version      = await Send($"http://127.0.0.1:{port}/ocpi/v3.0/versions/3.0", $"Token {token}");
            var url          = (version.JSON?["data"]?["endpoints"] as JArray)?.FirstOrDefault(endpoint => endpoint.Value<String>("identifier") == "credentials")?.Value<String>("url");

            Assert.That(url, Is.Not.Null, $"This version has no credentials endpoint: {version.Text}");

            var credentials  = await Send(url!, $"Token {token}");
            var roles        = credentials.JSON?["data"]?["roles"] as JArray;

            Assert.Multiple(() => {
                Assert.That(credentials.Status,                              Is.EqualTo(200),   credentials.Text);
                Assert.That(credentials.JSON?.Value<Int32?>("status_code"),  Is.EqualTo(1000),  credentials.Text);
                Assert.That(roles,                                           Has.Count.EqualTo(1), credentials.Text);
                Assert.That(roles?.First().ToString(),                       Does.Contain("GEF").And.Contain("CPO").And.Contain("GraphDefined CSO"), credentials.Text);
            });

        }

        #endregion


        #region TheVersionDetailsAnnounceNoModuleNoModuleAPIServes()

        /// <summary>
        /// OCPI 3.0 has no module API serving the CPO or the EMSP modules yet:
        /// the version details announce none of them to a peer, though this
        /// platform is a CPO and the peer an EMSP - nor charging profiles - but
        /// the credentials alone.
        /// </summary>
        [Test]
        public async Task TheVersionDetailsAnnounceNoModuleNoModuleAPIServes()
        {

            var version      = await Send($"http://127.0.0.1:{port}/ocpi/v3.0/versions/3.0", $"Token {token}");
            var identifiers  = ((version.JSON?["data"]?["endpoints"] as JArray) ?? []).Select(endpoint => endpoint.Value<String>("identifier"));

            Assert.Multiple(() => {
                Assert.That(version.Status,  Is.EqualTo(200),                    version.Text);
                Assert.That(identifiers,     Is.EqualTo(new[] { "credentials" }), $"A module is announced that no module API serves: {version.Text}");
            });

        }

        #endregion

        #region TheRequestNamesTheValidityOfTheTokenItCameWith()

        /// <summary>
        /// A party with two tokens of ours, each valid for a time of its own -
        /// as every registration and renewal leaves one pending beside the one
        /// in effect for a while - is let in with the second: the request
        /// names when that one may be used, not when the first may.
        /// </summary>
        [Test]
        public async Task TheRequestNamesTheValidityOfTheTokenItCameWith()
        {

            var first   = AccessToken.Parse("their-first-token-of-ours");
            var second  = AccessToken.Parse("their-second-token-of-ours");

            var added   = await api.AddRemoteParty(RemoteParty_Id.Parse("DE-DDD_EMSP"),
                                                  [
                                                      new CredentialsRole(
                                                          CountryCode.Parse("DE"),
                                                          Party_Id.   Parse("DDD"),
                                                          Role.EMSP,
                                                          new BusinessDetails("An EMSP with two tokens of ours")
                                                      )
                                                  ],

                                                  [
                                                      new LocalAccessInfo(first,  NotBefore: new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero), NotAfter: new DateTimeOffset(2034, 1, 1, 0, 0, 0, TimeSpan.Zero), AccessTokenIsBase64Encoded: false),
                                                      new LocalAccessInfo(second, NotBefore: new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero), NotAfter: new DateTimeOffset(2035, 1, 1, 0, 0, 0, TimeSpan.Zero), AccessTokenIsBase64Encoded: false)
                                                  ],
                                                  []);

            Assert.That(added.IsSuccess, Is.True, $"The party with two tokens could not be added: {added.ErrorResponse}");

            var version = await Send($"http://127.0.0.1:{port}/ocpi/v3.0/versions/3.0", $"Token {token}");
            var url     = (version.JSON?["data"]?["endpoints"] as JArray)?.FirstOrDefault(endpoint => endpoint.Value<String>("identifier") == "credentials")?.Value<String>("url");

            Assert.That(url, Is.Not.Null, $"This version has no credentials endpoint: {version.Text}");

            LocalAccessInfo2? named = null;

            api.OnGetCredentialsRequest += (timestamp, httpAPI, request, cancellationToken) => {
                named = request.LocalAccessInfo;
                return Task.CompletedTask;
            };

            var answer  = await Send(url!, $"Token {second}");

            Assert.Multiple(() => {
                Assert.That(answer.JSON?.Value<Int32?>("status_code"),  Is.EqualTo(1000),                                                     answer.Text);
                Assert.That(named?.AccessToken,                         Is.EqualTo(second),                                                   "The request names another token than it came with.");
                Assert.That(named?.NotBefore,                           Is.EqualTo(new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero)),  "The request names when another token of the party may be used from.");
                Assert.That(named?.NotAfter,                            Is.EqualTo(new DateTimeOffset(2035, 1, 1, 0, 0, 0, TimeSpan.Zero)),  "The request names until when another token of the party may be used.");
            });

        }

        #endregion


        #region (private) Send(URL, Authorization)

        /// <summary>
        /// A GET with the given Authorization header, if any - and what it was
        /// answered.
        /// </summary>
        private static async Task<Answer> Send(String   URL,
                                               String?  Authorization)
        {

            using var http     = new HttpClient();
            using var request  = new HttpRequestMessage(HttpMethod.Get, URL);

            if (Authorization is not null)
                request.Headers.TryAddWithoutValidation("Authorization", Authorization);

            using var response = await http.SendAsync(request);

            var text = await response.Content.ReadAsStringAsync();

            return new Answer(
                       (Int32) response.StatusCode,
                       text.StartsWith('{') ? JObject.Parse(text) : null,
                       text
                   );

        }

        #endregion

        #region (private) FreePort() / ACommonAPI(Server, Port)

        /// <summary>
        /// A port nobody was listening on a moment ago.
        /// </summary>
        private static UInt16 FreePort()
        {

            var listener = new TcpListener(System.Net.IPAddress.Loopback, 0);

            listener.Start();

            var port = (UInt16) ((System.Net.IPEndPoint) listener.LocalEndpoint).Port;

            listener.Stop();

            return port;

        }

        /// <summary>
        /// A Common API on this test's directory and the given HTTP server,
        /// which says it is on the given port.
        /// </summary>
        private CommonAPI ACommonAPI(HTTPServer  Server,
                                     UInt16      Port)

            => new (

                   OurCredentialRoles:  [
                                            new CredentialsRole(
                                                Party_Idv3.From(CountryCode.Parse("DE"), Party_Id.Parse("GEF")),
                                                Role.CPO,
                                                new BusinessDetails("GraphDefined CSO")
                                            )
                                        ],
                   DefaultPartyId:      Party_Idv3.From(CountryCode.Parse("DE"), Party_Id.Parse("GEF")),

                   BaseAPI:             new CommonHTTPAPI(
                                          HTTPAPI:          new HTTPExtAPI(
                                                                HTTPServer: Server
                                                            ),
                                          OurBaseURL:       URL.Parse($"http://127.0.0.1:{Port}/ocpi"),
                                          OurVersionsURL:   URL.Parse($"http://127.0.0.1:{Port}/ocpi/versions"),
                                          RootPath:         HTTPPath.Parse("/ocpi"),
                                          DisableLogging:   true,
                                          LoggingPath:      directory
                                      ),

                   URLPathPrefix:       HTTPPath.Parse("/ocpi/v3.0"),
                   DatabaseFilePath:    directory,
                   DisableLogging:      true,
                   LoggingPath:         directory

               );

        #endregion

        #region (private) Answer

        /// <summary>
        /// What a request was answered: the HTTP status and the body.
        /// </summary>
        private sealed record Answer(Int32     Status,
                                     JObject?  JSON,
                                     String    Text);

        #endregion

    }

}
