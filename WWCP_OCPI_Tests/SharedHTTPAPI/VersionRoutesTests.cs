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

using v2_2_1 = cloud.charging.open.protocols.OCPIv2_2_1;
using v3_0   = cloud.charging.open.protocols.OCPIv3_0;

#endregion

namespace cloud.charging.open.protocols.OCPI.UnitTests.SharedHTTPAPI
{

    /// <summary>
    /// OCPI 2.2.1 and 3.0 on one HTTP API and below one path, as a platform
    /// serving both versions has them: each version is answered at its own
    /// routes by its own Common API, and a version served by neither by none -
    /// both given no path of their own, as the RoamingHub gives none, and both
    /// given the same one, which each puts below the path of the base API.
    /// </summary>
    /// <remarks>
    /// 3.0 registered its routes as templates of the version id,
    /// "versions/{versionId}" and "{versionId}/credentials", where 2.x
    /// register their own version: they matched every version's routes, and
    /// where they came before 2.2.1's, a 2.2.1 peer was answered by 3.0 - and
    /// refused, its token being of nobody there.
    /// And 3.0 took a path given as a whole path, where 2.x put that of the
    /// base API before it: given none, 3.0 was served at the root of the HTTP
    /// API, and given one, elsewhere than 2.2.1 given the same.
    /// </remarks>
    [TestFixture("")]
    [TestFixture("/cso")]
    public class VersionRoutesTests(String OwnPath)
    {

        #region Data

        private static readonly AccessToken  token2_2_1  = AccessToken.Parse("their-2.2.1-token-of-ours");
        private static readonly AccessToken  token3_0    = AccessToken.Parse("their-3.0-token-of-ours");

        /// <summary>
        /// The path both versions are given, below that of the base API, or none.
        /// </summary>
        private readonly        HTTPPath?    ownPathPrefix  = OwnPath.Length > 0 ? HTTPPath.Parse(OwnPath) : null;

        private String               directory  = default!;
        private CommonHTTPAPI        baseAPI    = default!;
        private v2_2_1.CommonAPI     api2_2_1   = default!;
        private v3_0.  CommonAPI     api3_0     = default!;
        private HTTPServer?          server;
        private UInt16               port;

        #endregion

        #region SetUp / TearDown

        [SetUp]
        public async Task SetUp()
        {

            directory  = Path.Combine(Path.GetTempPath(), $"WWCP_OCPI_Tests-{Guid.NewGuid():N}");

            Directory.CreateDirectory(directory);

            for (var attempt = 1; ; attempt++)
            {

                port      = FreePort();
                server    = new HTTPServer(IPAddress: IPv4Address.Localhost, TCPPort: IPPort.Parse(port));
                baseAPI   = ABaseAPI(server, port);
                api2_2_1  = ACommonAPIv2_2_1(baseAPI);
                api3_0    = ACommonAPIv3_0  (baseAPI);

                try
                {
                    await server.Start();
                    break;
                }
                catch (SocketException) when (attempt < 5)
                { }

            }

            var added2_2_1 = await api2_2_1.AddRemoteParty(RemoteParty_Id.Parse("DE-BBB_EMSP"),
                                                          [
                                                              new CredentialsRole(
                                                                  CountryCode.Parse("DE"),
                                                                  Party_Id.   Parse("BBB"),
                                                                  Role.EMSP,
                                                                  new BusinessDetails("Their 2.2.1 EMSP")
                                                              )
                                                          ],
                                                          token2_2_1,
                                                          LocalAccessTokenBase64Encoding: false);

            var added3_0   = await api3_0.  AddRemoteParty(RemoteParty_Id.Parse("DE-CCC_EMSP"),
                                                          [
                                                              new CredentialsRole(
                                                                  CountryCode.Parse("DE"),
                                                                  Party_Id.   Parse("CCC"),
                                                                  Role.EMSP,
                                                                  new BusinessDetails("Their 3.0 EMSP")
                                                              )
                                                          ],
                                                          token3_0,
                                                          LocalAccessTokenBase64Encoding: false);

            Assert.Multiple(() => {
                Assert.That(added2_2_1.IsSuccess,  Is.True,  $"The 2.2.1 peer could not be added: {added2_2_1.ErrorResponse}");
                Assert.That(added3_0.  IsSuccess,  Is.True,  $"The 3.0 peer could not be added: {added3_0.ErrorResponse}");
            });

        }

        [TearDown]
        public async Task TearDown()
        {

            await baseAPI.DisposeAsync();

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


        #region EachVersionIsDetailedByItsOwnCommonAPI()

        /// <summary>
        /// The details of 2.2.1 and of 3.0, asked for where the versions list
        /// says they are, are each those of its version, given to its peer.
        /// </summary>
        [Test]
        public async Task EachVersionIsDetailedByItsOwnCommonAPI()
        {

            var versions2_2_1  = await Send($"http://127.0.0.1:{port}/ocpi/versions", $"Token {token2_2_1}");
            var versions3_0    = await Send($"http://127.0.0.1:{port}/ocpi/versions", $"Token {token3_0}");

            var url2_2_1       = DetailsURL(versions2_2_1, "2.2.1");
            var url3_0         = DetailsURL(versions3_0,   "3.0");

            Assert.Multiple(() => {
                Assert.That(url2_2_1,  Does.EndWith($":{port}/ocpi{OwnPath}/versions/2.2.1"),  $"The versions list does not list 2.2.1 where it is: {versions2_2_1.Text}");
                Assert.That(url3_0,    Does.EndWith($":{port}/ocpi{OwnPath}/versions/3.0"),    $"The versions list does not list 3.0 where it is: {versions3_0.Text}");
            });

            var details2_2_1   = await Send(url2_2_1!, $"Token {token2_2_1}");
            var details3_0     = await Send(url3_0!,   $"Token {token3_0}");

            Assert.Multiple(() => {

                Assert.That(details2_2_1.Status,                                     Is.EqualTo(200),      details2_2_1.Text);
                Assert.That(details2_2_1.JSON?.Value<Int32?> ("status_code"),        Is.EqualTo(1000),     details2_2_1.Text);
                Assert.That(details2_2_1.JSON?["data"]?.Value<String>("version"),    Is.EqualTo("2.2.1"),  details2_2_1.Text);

                Assert.That(details3_0.  Status,                                     Is.EqualTo(200),      details3_0.Text);
                Assert.That(details3_0.  JSON?.Value<Int32?> ("status_code"),        Is.EqualTo(1000),     details3_0.Text);
                Assert.That(details3_0.  JSON?["data"]?.Value<String>("version"),    Is.EqualTo("3.0"),    details3_0.Text);

            });

        }

        #endregion

        #region EachVersionsCredentialsAreThoseOfItsOwnCommonAPI()

        /// <summary>
        /// The credentials of 2.2.1 and of 3.0, asked for where the details of
        /// each version announce them, name the party of that version, each to
        /// its own peer.
        /// </summary>
        [Test]
        public async Task EachVersionsCredentialsAreThoseOfItsOwnCommonAPI()
        {

            var url2_2_1          = CredentialsURL(await Send($"http://127.0.0.1:{port}/ocpi{OwnPath}/versions/2.2.1", $"Token {token2_2_1}"));
            var url3_0            = CredentialsURL(await Send($"http://127.0.0.1:{port}/ocpi{OwnPath}/versions/3.0",   $"Token {token3_0}"));

            Assert.Multiple(() => {
                Assert.That(url2_2_1,  Does.EndWith($":{port}/ocpi{OwnPath}/v2.2.1/credentials"),  "2.2.1 announces its credentials elsewhere.");
                Assert.That(url3_0,    Does.EndWith($":{port}/ocpi{OwnPath}/v3.0/credentials"),    "3.0 announces its credentials elsewhere.");
            });

            var credentials2_2_1  = await Send(url2_2_1!, $"Token {token2_2_1}");
            var credentials3_0    = await Send(url3_0!,   $"Token {token3_0}");

            Assert.Multiple(() => {

                Assert.That(credentials2_2_1.Status,                               Is.EqualTo(200),   credentials2_2_1.Text);
                Assert.That(credentials2_2_1.JSON?.Value<Int32?>("status_code"),   Is.EqualTo(1000),  credentials2_2_1.Text);
                Assert.That(credentials2_2_1.JSON?["data"]?["roles"]?.ToString(), Does.Contain("GraphDefined CSO 2.2.1").And.Not.Contain("GraphDefined CSO 3.0"), credentials2_2_1.Text);

                Assert.That(credentials3_0.  Status,                               Is.EqualTo(200),   credentials3_0.Text);
                Assert.That(credentials3_0.  JSON?.Value<Int32?>("status_code"),   Is.EqualTo(1000),  credentials3_0.Text);
                Assert.That(credentials3_0.  JSON?["data"]?["roles"]?.ToString(), Does.Contain("GraphDefined CSO 3.0").And.Not.Contain("GraphDefined CSO 2.2.1"), credentials3_0.Text);

            });

        }

        #endregion

        #region AVersionServedByNoneIsAnsweredByNone()

        /// <summary>
        /// The details and the credentials of a version neither Common API
        /// serves are not there: 3.0 does not answer them as if they were its
        /// own.
        /// </summary>
        [Test]
        public async Task AVersionServedByNoneIsAnsweredByNone()
        {

            var details      = await Send($"http://127.0.0.1:{port}/ocpi{OwnPath}/versions/2.1.1",  $"Token {token3_0}");
            var credentials  = await Send($"http://127.0.0.1:{port}/ocpi{OwnPath}/v2.1.1/credentials", $"Token {token3_0}");

            Assert.Multiple(() => {
                Assert.That(details.    Status,  Is.EqualTo(404),  $"The details of 2.1.1, served by none, were answered: {details.Text}");
                Assert.That(credentials.Status,  Is.EqualTo(404),  $"The credentials of 2.1.1, served by none, were answered: {credentials.Text}");
            });

        }

        #endregion


        #region (private) DetailsURL(Versions, Version) / CredentialsURL(Details)

        /// <summary>
        /// Where the versions list says the given version is detailed.
        /// </summary>
        private static String? DetailsURL(Answer  Versions,
                                          String  Version)

            => (Versions.JSON?["data"] as JArray)?.
                   FirstOrDefault(version => version.Value<String>("version") == Version)?.
                   Value<String>("url");

        /// <summary>
        /// Where the details of a version say its credentials are.
        /// </summary>
        private static String? CredentialsURL(Answer Details)

            => (Details.JSON?["data"]?["endpoints"] as JArray)?.
                   FirstOrDefault(endpoint => endpoint.Value<String>("identifier") == "credentials")?.
                   Value<String>("url");

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

        #region (private) FreePort() / ABaseAPI(Server, Port) / ACommonAPIv2_2_1(BaseAPI) / ACommonAPIv3_0(BaseAPI)

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
        /// The base API both versions hang off, on the given HTTP server, which
        /// says it is on the given port.
        /// </summary>
        private CommonHTTPAPI ABaseAPI(HTTPServer  Server,
                                       UInt16      Port)

            => new (
                   HTTPAPI:         new HTTPExtAPI(
                                        HTTPServer: Server
                                    ),
                   OurBaseURL:      URL.Parse($"http://127.0.0.1:{Port}/ocpi"),
                   OurVersionsURL:  URL.Parse($"http://127.0.0.1:{Port}/ocpi/versions"),
                   RootPath:        HTTPPath.Parse("/ocpi"),
                   DisableLogging:  true,
                   LoggingPath:     directory
               );

        /// <summary>
        /// A 2.2.1 Common API below the path both versions share: that of the
        /// base API, which 2.x put before the path they are given.
        /// </summary>
        private v2_2_1.CommonAPI ACommonAPIv2_2_1(CommonHTTPAPI BaseAPI)

            => new (

                   OurPartyData:      [
                                          new v2_2_1.PartyData(
                                              Party_Idv3.From(CountryCode.Parse("DE"), Party_Id.Parse("GEF")),
                                              Role.CPO,
                                              new BusinessDetails("GraphDefined CSO 2.2.1")
                                          )
                                      ],
                   DefaultPartyId:    Party_Idv3.From(CountryCode.Parse("DE"), Party_Id.Parse("GEF")),

                   BaseAPI:           BaseAPI,

                   URLPathPrefix:     ownPathPrefix,
                   DatabaseFilePath:  directory,
                   DisableLogging:    true,
                   LoggingPath:       directory

               );

        /// <summary>
        /// A 3.0 Common API below the path both versions share: that of the
        /// base API, which 3.0 puts before the path it is given, as 2.x do.
        /// </summary>
        private v3_0.CommonAPI ACommonAPIv3_0(CommonHTTPAPI BaseAPI)

            => new (

                   OurCredentialRoles:  [
                                            new CredentialsRole(
                                                Party_Idv3.From(CountryCode.Parse("DE"), Party_Id.Parse("GEF")),
                                                Role.CPO,
                                                new BusinessDetails("GraphDefined CSO 3.0")
                                            )
                                        ],
                   DefaultPartyId:      Party_Idv3.From(CountryCode.Parse("DE"), Party_Id.Parse("GEF")),

                   BaseAPI:             BaseAPI,

                   URLPathPrefix:       ownPathPrefix,
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
