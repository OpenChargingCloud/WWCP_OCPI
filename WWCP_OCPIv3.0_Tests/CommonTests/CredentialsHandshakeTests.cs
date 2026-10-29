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

using System.Text;
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
    /// The other side registering here, or putting its credentials anew:
    /// token C in place of token A in one line, and token A gone only once
    /// that line is in its file. Where the file refuses it, or no answer can
    /// be made, nothing has changed, and token A is still valid.
    /// </summary>
    /// <remarks>
    /// The other side is a stub with the two routes this API walks before it
    /// answers - the versions and the details of 3.0. The file is made
    /// unwritable the way that stops root as well: a directory where it would
    /// be. The next start is a Common API made anew on the same directory.
    /// </remarks>
    [TestFixture]
    public class CredentialsHandshakeTests
    {

        #region Data

        private static readonly RemoteParty_Id  id          = RemoteParty_Id.Parse("DE-BBB_EMSP");
        private static readonly AccessToken     tokenA      = AccessToken.Parse("their-token-a-of-ours");
        private static readonly AccessToken     theirToken  = AccessToken.Parse("their-token-for-us");
        private static readonly AccessToken     theirOld    = AccessToken.Parse("their-old-token-for-us");

        // How the other side is reached, beside its tokens.
        private static readonly DateTimeOffset  notAfter    = new (2036, 1, 1, 0, 0, 0, TimeSpan.Zero);
        private const           String          userAgent   = "Our OCPI client";
        private static readonly TimeSpan        timeout     = TimeSpan.FromSeconds(23);
        private const           UInt16          retries     = 2;

        private String       directory  = default!;
        private CommonAPI    api        = default!;
        private OtherSide    other      = default!;
        private HTTPServer?  listening;
        private UInt16       port;

        #endregion

        #region SetUp / TearDown

        [SetUp]
        public async Task SetUp()
        {

            directory  = Path.Combine(Path.GetTempPath(), $"WWCP_OCPI_Tests-{Guid.NewGuid():N}");

            Directory.CreateDirectory(directory);

            other      = await OtherSide.Start();

            await Listening();

        }

        [TearDown]
        public async Task TearDown()
        {

            await other.DisposeAsync();

            if (listening is not null)
                await listening.Stop();

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


        #region ARegistrationPutsTokenCInPlaceOfTokenA()

        /// <summary>
        /// A registration is answered with token C, which is from then on the
        /// one token of the other side here - token A is not let in any more -
        /// now and at the next start.
        /// </summary>
        [Test]
        public async Task ARegistrationPutsTokenCInPlaceOfTokenA()
        {

            await AddTheOtherSideToCome();

            var credentials                    = await CredentialsURL(tokenA);

            var (status, statusCode, answer)   = await Send(HTTPMethod.POST, credentials, tokenA);

            Assert.That(statusCode, Is.EqualTo(1000), $"The registration answered {status}: {answer}");

            var tokenC                         = AccessToken.Parse(answer?["data"]?.Value<String>("token") ?? "");

            var (withA, _, _)                  = await Send(HTTPMethod.GET, credentials, tokenA);
            // Token C as tokens are sent by default, Base64: what our local
            // token was set to is not taken over (OCPI-ISSUES.md O6).
            var (withC, withCCode, _)          = await Send(HTTPMethod.GET, credentials, tokenC, Base64: true);

            Assert.Multiple(() => {
                Assert.That(tokenC,              Is.Not.EqualTo(tokenA),             "The registration answered token A again.");
                Assert.That(LocalTokensOf(api),  Is.EqualTo(new[] { tokenC }),       "Token C is not the one token of the other side.");
                Assert.That(RemoteTokenOf(api),  Is.EqualTo(theirToken.ToString()),  "The token the other side sent is not the one it is called with.");
                Assert.That(withA,               Is.EqualTo(401),                    "Token A is still let in.");
                Assert.That(withCCode,           Is.EqualTo(1000),                   $"Token C is not let in: {withC}.");
            });

            var again = await NextStart();

            Assert.Multiple(() => {
                Assert.That(LocalTokensOf(again),  Is.EqualTo(new[] { tokenC }),       "The next start does not know token C alone.");
                Assert.That(RemoteTokenOf(again),  Is.EqualTo(theirToken.ToString()),  "The next start does not know the token the other side sent.");
            });

        }

        #endregion

        #region ARegistrationTheFileRefusesChangesNothing()

        /// <summary>
        /// A registration the file cannot take is answered OCPI 3000 with HTTP
        /// 500, and nothing changed: token A is still the one the other side
        /// has, now and at the next start.
        /// </summary>
        [Test]
        public async Task ARegistrationTheFileRefusesChangesNothing()
        {

            await AddTheOtherSideToCome();

            var credentials  = await CredentialsURL(tokenA);
            var told         = new List<String>();

            api.OnRemotePartyNotSaved += (timestamp, command, fileName, exception) => {
                lock (told)
                    told.Add(command);
                return Task.CompletedTask;
            };

            BlockTheFile();

            var (status, statusCode, answer) = await Send(HTTPMethod.POST, credentials, tokenA);

            Assert.Multiple(() => {
                Assert.That(status,              Is.EqualTo(500),                                         $"The registration the file refused is not answered 500: {answer}");
                Assert.That(statusCode,          Is.EqualTo(3000),                                        $"The registration the file refused is not answered OCPI 3000: {answer}");
                Assert.That(answer?["data"],     Is.Null,                                                 "The registration the file refused is answered with credentials.");
                Assert.That(LocalTokensOf(api),  Is.EqualTo(new[] { tokenA }),                            "Token A is not the one the other side has any more.");
                Assert.That(RemoteTokenOf(api),  Is.Null,                                                 "The token the other side sent is kept, though the file refused it.");
                Assert.That(told,                Is.EqualTo(new[] { CommonHTTPAPI.addOrUpdateRemoteParty }), "The line the file refused is not told to OnRemotePartyNotSaved.");
            });

            UnblockTheFile();

            var (_, again, _) = await Send(HTTPMethod.POST, credentials, tokenA);

            Assert.That(again, Is.EqualTo(1000), "Token A could not register once the file took lines again.");

            var next = await NextStart();

            Assert.That(LocalTokensOf(next), Has.None.EqualTo(tokenA), "The next start knows token A, though the registration went through.");

        }

        #endregion

        #region NewCredentialsKeepHowTheOtherSideIsReached()

        /// <summary>
        /// Credentials put anew by the other side change its versions URL and
        /// its token, and nothing else of how it is reached, nor the versions
        /// it sees.
        /// </summary>
        [Test]
        public async Task NewCredentialsKeepHowTheOtherSideIsReached()
        {

            var added = await api.AddRemoteParty(id,
                                                 TheirRoles,
                                                 tokenA,
                                                 other.VersionsURL,
                                                 theirOld,
                                                 HTTPUserAgent:                   userAgent,
                                                 RequestTimeout:                  timeout,
                                                 MaxNumberOfRetries:              retries,
                                                 RemoteAccessNotAfter:            notAfter,
                                                 LocalAccessTokenBase64Encoding:  false,
                                                 VisibleVersionIds:               [ Version.Id ]);

            Assert.That(added.IsSuccess, Is.True, $"The other side could not be added: {added.ErrorResponse}");

            var credentials                    = await CredentialsURL(tokenA);

            var (status, statusCode, answer)   = await Send(HTTPMethod.PUT, credentials, tokenA);

            Assert.That(statusCode, Is.EqualTo(1000), $"The new credentials answered {status}: {answer}");

            var reached = RemoteAccessOf(api);

            Assert.Multiple(() => {
                Assert.That(reached?.AccessToken.ToString(),  Is.EqualTo(theirToken.ToString()),  "The other side is not called with the token it sent.");
                Assert.That(reached?.VersionsURL,             Is.EqualTo(other.VersionsURL),      "The other side is not called where it said.");
                Assert.That(reached?.HTTPUserAgent,           Is.EqualTo(userAgent),              "The user agent the other side is called with is gone.");
                Assert.That(reached?.RequestTimeout,          Is.EqualTo(timeout),                "The timeout the other side is called with is gone.");
                Assert.That(reached?.MaxNumberOfRetries,      Is.EqualTo(retries),                "The retries the other side is called with are gone.");
                Assert.That(reached?.NotAfter,                Is.EqualTo(notAfter),               "When the token of the other side is not to be used after is gone.");
                Assert.That(VisibleVersionsOf(api),           Is.EqualTo(new[] { Version.Id }),   "The versions the other side sees are not those it saw.");
                Assert.That(CreatedOf(api),                   Is.EqualTo(added.Data!.Created),    "When the other side was added is not when it was added.");
            });

        }

        #endregion


        #region ARegistrationIsNotFollowedWhereTheAPIDoesNotDial()

        /// <summary>
        /// The versions URL a peer hands in is dialled only where the API
        /// allows (POI3-ISSUES.md S19): where it does not, nothing is asked
        /// there, the registration is refused, and token A is still valid.
        /// </summary>
        [Test]
        public async Task ARegistrationIsNotFollowedWhereTheAPIDoesNotDial()
        {

            await AddTheOtherSideToCome();

            var credentials                    = await CredentialsURL(tokenA);

            api.BaseAPI.RemoteNetworkAcl       = org.GraphDefined.Vanaheimr.Hermod.SSH.NetworkAcl.DenyByDefault();

            var (status, statusCode, answer)   = await Send(HTTPMethod.POST, credentials, tokenA);

            Assert.Multiple(() => {
                Assert.That(statusCode,          Is.Not.EqualTo(1000),            $"The registration was answered {status}, though the API may not dial its versions URL: {answer}");
                Assert.That(other.VersionsAsked, Is.Zero,                         "The versions of the other side were asked, though the API may not dial them.");
                Assert.That(LocalTokensOf(api),  Is.EqualTo(new[] { tokenA }),    "Token A is gone, though the registration was refused.");
                Assert.That(RemoteTokenOf(api),  Is.Null,                         "The token the other side sent is kept, though the registration was refused.");
            });

        }

        #endregion


        #region (private) Listening() / AddTheOtherSideToCome() / TheirRoles

        /// <summary>
        /// This test's Common API, listening on a port nobody was listening on
        /// a moment ago.
        /// </summary>
        private async Task Listening()
        {

            for (var attempt = 1; ; attempt++)
            {

                port      = OtherSide.FreePort();
                listening = new HTTPServer(IPAddress: IPv4Address.Localhost, TCPPort: IPPort.Parse(port));
                api       = ACommonAPI(listening, port);

                try
                {
                    await listening.Start();
                    break;
                }
                catch (SocketException) when (attempt < 5)
                { }

            }

        }

        /// <summary>
        /// The other side to come: token A of ours, handed out and not yet used.
        /// </summary>
        private async Task AddTheOtherSideToCome()
        {

            var added = await api.AddRemoteParty(id,
                                                 TheirRoles,
                                                 tokenA,
                                                 LocalAccessTokenBase64Encoding: false);

            Assert.That(added.IsSuccess, Is.True, $"The other side to come could not be added: {added.ErrorResponse}");

        }

        private static CredentialsRole[] TheirRoles

            => [
                   new CredentialsRole(
                       CountryCode.Parse("DE"),
                       Party_Id.   Parse("BBB"),
                       Role.EMSP,
                       new BusinessDetails("Their EMSP")
                   )
               ];

        #endregion

        #region (private) CredentialsURL(Token) / Send(Method, URL, Token)

        /// <summary>
        /// Where this version's credentials are, as the other side finds them
        /// with the given token: the details of this version, which the shared
        /// versions list does not list (OCPI-ISSUES.md O3).
        /// </summary>
        private async Task<String> CredentialsURL(AccessToken Token)
        {

            var (_, _, version)  = await Send(HTTPMethod.GET, $"http://127.0.0.1:{port}/ocpi/v3.0/versions/3.0", Token);
            var credentials      = (version?["data"]?["endpoints"] as JArray)?.FirstOrDefault(endpoint => endpoint.Value<String>("identifier") == "credentials")?.Value<String>("url");

            Assert.That(credentials, Is.Not.Null, $"This version has no credentials endpoint: {version}");

            return credentials!;

        }

        /// <summary>
        /// A request of the other side to this API, with the given token - its
        /// credentials as the body of a POST or a PUT - and what it was
        /// answered: the HTTP status, the OCPI status code, and the answer. The
        /// token as it is, unless Base64 encoded.
        /// </summary>
        private async Task<(Int32 Status, Int32? StatusCode, JObject? Answer)> Send(HTTPMethod   Method,
                                                                                   String       URL,
                                                                                   AccessToken  Token,
                                                                                   Boolean      Base64   = false)
        {

            using var http     = new HttpClient();
            using var request  = new HttpRequestMessage(new HttpMethod(Method.ToString()), URL);

            request.Headers.TryAddWithoutValidation("Authorization", Base64
                                                                         ? $"Token {Convert.ToBase64String(Encoding.UTF8.GetBytes(Token.ToString()))}"
                                                                         : $"Token {Token}");

            if (Method == HTTPMethod.POST || Method == HTTPMethod.PUT)
                request.Content = new StringContent(
                                      new Credentials(
                                          theirToken,
                                          other.VersionsURL,
                                          TheirRoles
                                      ).ToJSON().ToString(),
                                      Encoding.UTF8,
                                      "application/json"
                                  );

            using var response = await http.SendAsync(request);

            var text   = await response.Content.ReadAsStringAsync();
            var answer = text.StartsWith('{') ? JObject.Parse(text) : null;

            return ((Int32) response.StatusCode, answer?.Value<Int32?>("status_code"), answer);

        }

        #endregion

        #region (private) LocalTokensOf(API) / RemoteTokenOf(API) / RemoteAccessOf(API) / VisibleVersionsOf(API) / CreatedOf(API)

        /// <summary>
        /// The local tokens of the other side.
        /// </summary>
        private static AccessToken[] LocalTokensOf(CommonAPI API)

            => API.RemoteParties.
                   Where     (party => party.Id == id).
                   SelectMany(party => party.LocalAccessInfos.Select(info => info.AccessToken)).
                   ToArray();

        /// <summary>
        /// The token of the other side this API calls it with, or null.
        /// </summary>
        private static String? RemoteTokenOf(CommonAPI API)

            => RemoteAccessOf(API)?.AccessToken.ToString();

        /// <summary>
        /// How this API reaches the other side, or null.
        /// </summary>
        private static RemoteAccessInfo? RemoteAccessOf(CommonAPI API)

            => API.RemoteParties.
                   Where     (party => party.Id == id).
                   SelectMany(party => party.RemoteAccessInfos).
                   FirstOrDefault();

        /// <summary>
        /// The versions the other side sees, as this API has them.
        /// </summary>
        private static Version_Id[] VisibleVersionsOf(CommonAPI API)

            => API.RemoteParties.
                   Where     (party => party.Id == id).
                   SelectMany(party => party.VisibleVersionIds).
                   ToArray();

        /// <summary>
        /// When the other side was added, as this API has it, or null.
        /// </summary>
        private static DateTimeOffset? CreatedOf(CommonAPI API)

            => API.RemoteParties.
                   Where     (party => party.Id == id).
                   Select    (party => (DateTimeOffset?) party.Created).
                   FirstOrDefault();

        #endregion

        #region (private) NextStart() / ACommonAPI(Server, Port)

        /// <summary>
        /// The next start of a node: the Common API so far disposed, and one
        /// made anew on the same directory, which reads back what it wrote.
        /// </summary>
        private async Task<CommonAPI> NextStart()
        {

            await api.BaseAPI.DisposeAsync();

            return ACommonAPI(new HTTPServer(TCPPort: IPPort.Parse(3999)), 3999);

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
                                          HTTPAPI:              new HTTPExtAPI(
                                                                    HTTPServer: Server
                                                                ),
                                          OurBaseURL:           URL.Parse($"http://127.0.0.1:{Port}/ocpi"),
                                          OurVersionsURL:       URL.Parse($"http://127.0.0.1:{Port}/ocpi/versions"),
                                          RootPath:             HTTPPath.Parse("/ocpi"),
                                          LocationsAsOpenData:  false,
                                          DisableLogging:       true,
                                          LoggingPath:          directory
                                      ),

                   URLPathPrefix:       HTTPPath.Parse("/ocpi/v3.0"),
                   DatabaseFilePath:    directory,
                   DisableLogging:      true,
                   LoggingPath:         directory

               );

        #endregion

        #region (private) BlockTheFile() / UnblockTheFile()

        /// <summary>
        /// Make the file of the remote parties unwritable: a directory where it
        /// is. What it held is put aside.
        /// </summary>
        private void BlockTheFile()
        {

            if (File.Exists(api.RemotePartyDBFileName))
                File.Move(api.RemotePartyDBFileName, api.RemotePartyDBFileName + ".aside");

            Directory.CreateDirectory(api.RemotePartyDBFileName);

        }

        /// <summary>
        /// Give the file of the remote parties back what it held.
        /// </summary>
        private void UnblockTheFile()
        {

            Directory.Delete(api.RemotePartyDBFileName);

            if (File.Exists(api.RemotePartyDBFileName + ".aside"))
                File.Move(api.RemotePartyDBFileName + ".aside", api.RemotePartyDBFileName);

        }

        #endregion


        #region (private) OtherSide

        /// <summary>
        /// The other side of a registration, as this API walks it before it
        /// answers: the versions and the details of 3.0, on a port nobody was
        /// listening on a moment ago.
        /// </summary>
        private sealed class OtherSide : IAsyncDisposable
        {

            private readonly HTTPServer server;

            /// <summary>Where it says its versions are.</summary>
            public URL    VersionsURL     { get; }

            /// <summary>How often its versions were asked.</summary>
            public Int32  VersionsAsked   => versionsAsked;

            private Int32 versionsAsked;

            private OtherSide(HTTPServer Server, URL VersionsURL)
            {
                this.server       = Server;
                this.VersionsURL  = VersionsURL;
            }

            /// <summary>
            /// Started on a free port - made again on another where the one
            /// handed out was taken before it could be bound.
            /// </summary>
            public static async Task<OtherSide> Start()
            {

                for (var attempt = 1; ; attempt++)
                {

                    var port   = FreePort();
                    var server = new HTTPServer(IPAddress: IPv4Address.Localhost, TCPPort: IPPort.Parse(port));
                    var other  = Made(server, port);

                    try
                    {
                        await server.Start();
                        return other;
                    }
                    catch (SocketException) when (attempt < 5)
                    { }

                }

            }

            public static UInt16 FreePort()
            {

                var listener = new TcpListener(System.Net.IPAddress.Loopback, 0);

                listener.Start();

                var port = (UInt16) ((System.Net.IPEndPoint) listener.LocalEndpoint).Port;

                listener.Stop();

                return port;

            }

            private static OtherSide Made(HTTPServer Server, UInt16 Port)
            {

                var origin  = $"http://127.0.0.1:{Port}";
                var other   = new OtherSide(Server, URL.Parse($"{origin}/versions"));
                var api     = Server.AddHTTPAPI(HTTPPath.Root);

                api.AddHandler(
                    HTTPPath.Parse("/versions"),
                    request => {

                        Interlocked.Increment(ref other.versionsAsked);

                        return Task.FromResult(JSON(request, new JArray(
                                   new VersionInformation(
                                       Version.Id,
                                       URL.Parse($"{origin}/versions/3.0")
                                   ).ToJSON()
                               )));

                    },
                    HTTPMethod.GET
                );

                api.AddHandler(
                    HTTPPath.Parse("/versions/3.0"),
                    request => Task.FromResult(JSON(request, new VersionDetail(
                                   Version.Id,
                                   [
                                       new VersionEndpoint(
                                           Module_Id.Credentials,
                                           InterfaceRoles.RECEIVER,
                                           URL.Parse($"{origin}/3.0/credentials")
                                       )
                                   ]
                               ).ToJSON())),
                    HTTPMethod.GET
                );

                return other;

            }

            private static HTTPResponse JSON(HTTPRequest Request, JToken Data)

                => new HTTPResponse.Builder(Request) {
                       HTTPStatusCode  = HTTPStatusCode.OK,
                       ContentType     = HTTPContentType.Application.JSON_UTF8,
                       Content         = Encoding.UTF8.GetBytes(
                                             new JObject(
                                                 new JProperty("data",            Data),
                                                 new JProperty("status_code",     1000),
                                                 new JProperty("status_message",  "OK"),
                                                 new JProperty("timestamp",       DateTimeOffset.UtcNow.ToString("o"))
                                             ).ToString()
                                         ),
                       Connection      = ConnectionType.Close
                   }.AsImmutable;

            public async ValueTask DisposeAsync()
            {
                await server.Stop();
            }

        }

        #endregion

    }

}
