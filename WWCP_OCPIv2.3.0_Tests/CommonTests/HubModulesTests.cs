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

namespace cloud.charging.open.protocols.OCPIv2_3_0.UnitTests.CommonTests
{

    /// <summary>
    /// The HUB modules: a HUB_HTTPAPI can be built - its receiver half of a
    /// module that has a sender half too is below "receiver/", where Hermod
    /// refused the two side by side as ambiguous - and the version details
    /// announce both halves where it is built, and none of them where it is
    /// not, though the platform has the HUB role.
    /// </summary>
    [TestFixture]
    public class HubModulesTests
    {

        #region Data

        private static readonly AccessToken  token  = AccessToken.Parse("their-token-of-ours");

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

            var added = await api.AddRemoteParty(RemoteParty_Id.Parse("DE-BBB_EMSP"),
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

            await api.BaseAPI.DisposeAsync();

            if (server is not null)
                await server.DisposeAsync();

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


        #region AHubAPIIsBuiltAndItsModulesAreAnnounced()

        /// <summary>
        /// A HUB_HTTPAPI is built, and the version details announce each
        /// module it serves: the sender half below "hub/", the receiver half
        /// below "hub/receiver/" - and both answer there.
        /// </summary>
        [Test]
        public async Task AHubAPIIsBuiltAndItsModulesAreAnnounced()
        {

            var hubAPI     = ABuiltHubAPI();

            var endpoints  = await Endpoints();

            Assert.Multiple(() => {
                foreach (var module in new[] { "locations", "tariffs", "sessions", "cdrs", "tokens" })
                {
                    Assert.That(endpoints, Does.Contain(($"{module}", "SENDER",   $"/hub/{module}")),          $"The sender half of {module} is not announced where it is served.");
                    Assert.That(endpoints, Does.Contain(($"{module}", "RECEIVER", $"/hub/receiver/{module}")), $"The receiver half of {module} is not announced where it is served.");
                }
                Assert.That(endpoints, Does.Contain(("commands", "RECEIVER", "/hub/commands")), "The commands are not announced where they are served.");
            });

            var hubPath   = $"http://127.0.0.1:{port}{hubAPI.URLPathPrefix}".TrimEnd('/');
            var sender    = await Send($"{hubPath}/locations",                          $"Token {token}");
            var receiver  = await Send($"{hubPath}/receiver/locations/DE/BBB/LOC0001",  $"Token {token}");

            // A route that is not there is a 404 without an OCPI answer; an OCPI
            // 404 - a location this hub does not have - is a route that is.
            Assert.Multiple(() => {
                Assert.That(IsServed(sender),   Is.True, $"The sender half of the locations is not served where it is announced: {sender.Status} {sender.Text}");
                Assert.That(IsServed(receiver), Is.True, $"The receiver half of the locations is not served where it is announced: {receiver.Status} {receiver.Text}");
            });

        }

        #endregion

        #region WithoutAHubAPINoneOfItsModulesIsAnnounced()

        /// <summary>
        /// A platform with the HUB role that builds no HUB_HTTPAPI - as the
        /// RoamingHub does - announces none of its modules: a peer would be sent
        /// to endpoints nobody answers.
        /// </summary>
        [Test]
        public async Task WithoutAHubAPINoneOfItsModulesIsAnnounced()
        {

            var endpoints = await Endpoints();

            Assert.That(endpoints.Where(endpoint => endpoint.URLEnd.Contains("/hub/")),
                        Is.Empty,
                        "A module of the HUB_HTTPAPI is announced, though none was built.");

        }

        #endregion


        #region (private) ABuiltHubAPI()

        /// <summary>
        /// A HUB_HTTPAPI on this test's Common API - which threw in its
        /// constructor while two parameter routes shared a path segment.
        /// </summary>
        private HUB_HTTPAPI ABuiltHubAPI()
        {

            HUB_HTTPAPI? hubAPI = null;

            Assert.That(() => hubAPI = new HUB_HTTPAPI(api,
                                                       DisableLogging:  true,
                                                       LoggingPath:     directory),
                        Throws.Nothing,
                        "The HUB_HTTPAPI could not be built.");

            return hubAPI!;

        }

        #endregion

        #region (private) Endpoints()

        /// <summary>
        /// The endpoints the version details announce, as identifier, role and
        /// the path of their URL below this platform.
        /// </summary>
        private async Task<List<(String Identifier, String Role, String URLEnd)>> Endpoints()
        {

            var versions  = await Send($"http://127.0.0.1:{port}/ocpi/versions", $"Token {token}");
            var details   = (versions.JSON?["data"] as JArray)?.FirstOrDefault(version => version.Value<String>("version") == Version.Id.ToString())?.Value<String>("url");

            Assert.That(details, Is.Not.Null, $"The versions list does not list {Version.Id}: {versions.Text}");

            var version   = await Send(details!, $"Token {token}");

            return ((version.JSON?["data"]?["endpoints"] as JArray) ?? []).
                       Select(endpoint => (endpoint.Value<String>("identifier") ?? "",
                                           endpoint.Value<String>("role")       ?? "",
                                           URLEnd(endpoint.Value<String>("url") ?? ""))).
                       ToList();

        }

        /// <summary>
        /// The part of the given URL from "/hub/" on, or the whole of it.
        /// </summary>
        private static String URLEnd(String URL)
        {
            var hub = URL.IndexOf("/hub/", StringComparison.Ordinal);
            return hub >= 0 ? URL[hub..].TrimEnd('/') : URL;
        }

        #endregion

        #region (private) IsServed(Answer)

        /// <summary>
        /// Whether a route answered: anything but a 404, or a 404 with an OCPI
        /// status in it.
        /// </summary>
        private static Boolean IsServed(Answer Answer)

            => Answer.Status != 404 ||
               Answer.JSON?["status_code"] is not null;

        #endregion

        #region (private) Send(URL, Authorization)

        /// <summary>
        /// A GET with the given Authorization header - and what it was answered.
        /// </summary>
        private static async Task<Answer> Send(String  URL,
                                               String  Authorization)
        {

            using var http     = new HttpClient();
            using var request  = new HttpRequestMessage(HttpMethod.Get, URL);

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
        /// A Common API of a hub on this test's directory and the given HTTP
        /// server, which says it is on the given port.
        /// </summary>
        private CommonAPI ACommonAPI(HTTPServer  Server,
                                     UInt16      Port)

            => new (

                   OurPartyData:        [
                                            new PartyData(
                                                Party_Idv3.From(CountryCode.Parse("DE"), Party_Id.Parse("GDH")),
                                                Role.HUB,
                                                new BusinessDetails("GraphDefined Hub")
                                            )
                                        ],
                   DefaultPartyId:      Party_Idv3.From(CountryCode.Parse("DE"), Party_Id.Parse("GDH")),

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

                   URLPathPrefix:       HTTPPath.Parse("/ocpi/v2.3.0"),
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
