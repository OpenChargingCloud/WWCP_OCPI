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
    /// The modules the version details announce: those of the module APIs
    /// built on the Common API, at the paths they serve them at, and none of
    /// a module API that was not built, though the platform has its role -
    /// nor charging profiles, which no module API serves yet.
    /// </summary>
    [TestFixture]
    public class ModulesAnnouncedTests
    {

        #region Data

        private static readonly AccessToken  emspToken  = AccessToken.Parse("their-emsp-token-of-ours");

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
                                                emspToken,
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


        #region WithoutModuleAPIsOnlyTheCredentialsAreAnnounced()

        /// <summary>
        /// A platform that builds no CPO_HTTPAPI and no EMSP_HTTPAPI announces
        /// none of their modules: a peer would be sent to endpoints nobody
        /// answers.
        /// </summary>
        [Test]
        public async Task WithoutModuleAPIsOnlyTheCredentialsAreAnnounced()
        {

            var endpoints  = await Endpoints(emspToken);

            Assert.That(endpoints.Select(endpoint => endpoint.Identifier),
                        Is.EqualTo(new[] { "credentials" }),
                        "A module is announced that no module API serves, though the platform has its role.");

        }

        #endregion

        #region TheModulesOfTheModuleAPIsBuiltAreAnnouncedWhereTheyAreServed()

        /// <summary>
        /// A CPO_HTTPAPI and an EMSP_HTTPAPI are built, and the version details
        /// announce the modules they serve, where they serve them - and they
        /// answer there.
        /// </summary>
        [Test]
        public async Task TheModulesOfTheModuleAPIsBuiltAreAnnouncedWhereTheyAreServed()
        {

            var cpoAPI     = new CPO_HTTPAPI (api, DisableLogging: true, LoggingPath: directory);
            var emspAPI    = new EMSP_HTTPAPI(api, DisableLogging: true, LoggingPath: directory);

            var endpoints  = await Endpoints(emspToken);

            Assert.That(endpoints.Where(endpoint => endpoint.Identifier != "credentials"),
                        Is.EquivalentTo(new[] {
                            ("locations",  "SENDER",    "/cpo/locations"),
                            ("tariffs",    "SENDER",    "/cpo/tariffs"),
                            ("sessions",   "SENDER",    "/cpo/sessions"),
                            ("cdrs",       "SENDER",    "/cpo/cdrs"),
                            ("commands",   "RECEIVER",  "/cpo/commands"),
                            ("tokens",     "RECEIVER",  "/cpo/tokens"),
                            ("locations",  "RECEIVER",  "/emsp/locations"),
                            ("tariffs",    "RECEIVER",  "/emsp/tariffs"),
                            ("sessions",   "RECEIVER",  "/emsp/sessions"),
                            ("cdrs",       "RECEIVER",  "/emsp/cdrs"),
                            ("commands",   "SENDER",    "/emsp/commands"),
                            ("tokens",     "SENDER",    "/emsp/tokens")
                        }),
                        "The modules announced are not those the CPO_HTTPAPI and the EMSP_HTTPAPI serve, where they serve them.");

            var cpoPath    = $"http://127.0.0.1:{port}{cpoAPI. URLPathPrefix}".TrimEnd('/');
            var emspPath   = $"http://127.0.0.1:{port}{emspAPI.URLPathPrefix}".TrimEnd('/');
            var sender     = await Send($"{cpoPath}/locations",                   $"Token {emspToken}");
            var receiver   = await Send($"{emspPath}/locations/DE/CCC/LOC0001",   $"Token {emspToken}");

            // A route that is not there is a 404 without an OCPI answer; an OCPI
            // answer - a location nobody has, a party that may not - is a route
            // that is.
            Assert.Multiple(() => {
                Assert.That(IsServed(sender),   Is.True, $"The locations of the CPO_HTTPAPI are not served where they are announced: {sender.Status} {sender.Text}");
                Assert.That(IsServed(receiver), Is.True, $"The locations of the EMSP_HTTPAPI are not served where they are announced: {receiver.Status} {receiver.Text}");
            });

        }

        #endregion


        #region (private) Endpoints(Token)

        /// <summary>
        /// The endpoints the version details announce to the holder of the
        /// given token, as identifier, role and the path of their URL from
        /// "/cpo/" or "/emsp/" on.
        /// </summary>
        private async Task<List<(String Identifier, String Role, String URLEnd)>> Endpoints(AccessToken Token)
        {

            var versions  = await Send($"http://127.0.0.1:{port}/ocpi/versions", $"Token {Token}");
            var details   = (versions.JSON?["data"] as JArray)?.FirstOrDefault(version => version.Value<String>("version") == Version.Id.ToString())?.Value<String>("url");

            Assert.That(details, Is.Not.Null, $"The versions list does not list {Version.Id}: {versions.Text}");

            var version   = await Send(details!, $"Token {Token}");

            return ((version.JSON?["data"]?["endpoints"] as JArray) ?? []).
                       Select(endpoint => (endpoint.Value<String>("identifier") ?? "",
                                           endpoint.Value<String>("role")       ?? "",
                                           URLEnd(endpoint.Value<String>("url") ?? ""))).
                       ToList();

        }

        /// <summary>
        /// The part of the given URL from "/cpo/" or "/emsp/" on, or the whole
        /// of it.
        /// </summary>
        private static String URLEnd(String URL)
        {

            foreach (var side in new[] { "/cpo/", "/emsp/" })
            {
                var at = URL.IndexOf(side, StringComparison.Ordinal);
                if (at >= 0)
                    return URL[at..].TrimEnd('/');
            }

            return URL;

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
        /// A Common API on this test's directory and the given HTTP server,
        /// which says it is on the given port.
        /// </summary>
        private CommonAPI ACommonAPI(HTTPServer  Server,
                                     UInt16      Port)

            => new (

                   OurPartyData:        [
                                            new PartyData(
                                                Party_Idv3.From(CountryCode.Parse("DE"), Party_Id.Parse("GEF")),
                                                Role.CPO,
                                                new BusinessDetails("GraphDefined CSO")
                                            ),
                                            new PartyData(
                                                Party_Idv3.From(CountryCode.Parse("DE"), Party_Id.Parse("GDF")),
                                                Role.EMSP,
                                                new BusinessDetails("GraphDefined EMSP")
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
