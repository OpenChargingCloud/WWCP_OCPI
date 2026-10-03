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
    /// Who is given this platform's credentials: a caller its token makes
    /// known, and nobody else - not a caller without a token, not one with a
    /// token of nobody here, and not one that sends the TOTP of a party with
    /// a token that is not that party's.
    /// </summary>
    /// <remarks>
    /// The Common API is made as a platform makes it, with its locations as
    /// open data: an unknown token is let through to the routes then, and
    /// the credentials are the one route that must still say no.
    /// </remarks>
    [TestFixture]
    public class CredentialsAccessTests
    {

        #region Data

        private const           String       totpSecret  = "a-shared-secret-of-ours-and-theirs-long-enough-for-totp";

        private static readonly AccessToken  token       = AccessToken.Parse("their-token-of-ours");
        private static readonly AccessToken  totpToken   = AccessToken.Parse("their-token-that-wants-a-totp");
        private static readonly AccessToken  unknown     = AccessToken.Parse("a-token-of-nobody-here");

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

            var addedWithTOTP = await api.AddRemoteParty(RemoteParty_Id.Parse("DE-CCC_EMSP"),
                                                        [
                                                            new CredentialsRole(
                                                                CountryCode.Parse("DE"),
                                                                Party_Id.   Parse("CCC"),
                                                                Role.EMSP,
                                                                new BusinessDetails("An EMSP that signs in with a TOTP")
                                                            )
                                                        ],
                                                        totpToken,
                                                        LocalAccessTokenBase64Encoding: false,
                                                        LocalTOTPConfig:                new TOTPConfig(totpSecret));

            Assert.That(addedWithTOTP.IsSuccess, Is.True, $"The party with a TOTP could not be added: {addedWithTOTP.ErrorResponse}");

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


        #region TheCredentialsAreGivenToAKnownToken()

        /// <summary>
        /// A caller its token makes known is given the credentials, with that
        /// token in them.
        /// </summary>
        [Test]
        public async Task TheCredentialsAreGivenToAKnownToken()
        {

            var answer = await Send(await CredentialsURL(), $"Token {token}");

            Assert.Multiple(() => {
                Assert.That(answer.Status,                                     Is.EqualTo(200),            answer.Text);
                Assert.That(answer.JSON?.Value<Int32?>("status_code"),         Is.EqualTo(1000),           answer.Text);
                Assert.That(answer.JSON?["data"]?.Value<String>("token"),      Is.EqualTo(token.ToString()),
                            "The credentials do not hold the token they were asked with.");
            });

        }

        #endregion

        #region TheCredentialsAreRefusedWithoutAToken()

        /// <summary>
        /// A caller without a token is answered 401 and told nothing of this
        /// platform: no versions URL, no business details, no token.
        /// </summary>
        [Test]
        public async Task TheCredentialsAreRefusedWithoutAToken()
        {

            var answer = await Send(await CredentialsURL(), Authorization: null);

            IsRefused(answer, "The credentials asked for without a token");

        }

        #endregion

        #region TheCredentialsAreRefusedToAnUnknownToken()

        /// <summary>
        /// A caller with a token of nobody here is answered 401, as without
        /// one - the locations being open data does not make the credentials
        /// open.
        /// </summary>
        [Test]
        public async Task TheCredentialsAreRefusedToAnUnknownToken()
        {

            var answer = await Send(await CredentialsURL(), $"Token {unknown}");

            IsRefused(answer, "The credentials asked for with a token of nobody here");

        }

        #endregion

        #region ATOTPDoesNotMakeAnotherTokenThatOfItsParty()

        /// <summary>
        /// The TOTP of the party that signs in with one, sent with a token
        /// that is not that party's, makes the caller neither that party nor
        /// anybody else: it is refused as the token alone is.
        /// </summary>
        [Test]
        public async Task ATOTPDoesNotMakeAnotherTokenThatOfItsParty()
        {

            var answer = await Send(await CredentialsURL(), $"Token {unknown}", TOTP: CurrentTOTP());

            IsRefused(answer, "The credentials asked for with the TOTP of a party and a token of nobody here");

        }

        #endregion

        #region ATOTPDoesNotMakeTheTokenOfAnotherPartyThatOfItsParty()

        /// <summary>
        /// The TOTP of the party that signs in with one, sent with the token
        /// of another party, makes the caller that other party - the one its
        /// token names - and not the party of the TOTP.
        /// </summary>
        [Test]
        public async Task ATOTPDoesNotMakeTheTokenOfAnotherPartyThatOfItsParty()
        {

            var answer = await Send(await CredentialsURL(), $"Token {token}", TOTP: CurrentTOTP());

            Assert.Multiple(() => {
                Assert.That(answer.Status,                                     Is.EqualTo(200),            answer.Text);
                Assert.That(answer.JSON?["data"]?.Value<String>("token"),      Is.EqualTo(token.ToString()),
                            "The token of one party and the TOTP of another made the caller the party of the TOTP.");
            });

        }

        #endregion

        #region APartyWithATOTPIsLetInWithItsTokenAndItsTOTP()

        /// <summary>
        /// The party that signs in with a TOTP is let in with its token and its
        /// TOTP, and not with its token alone.
        /// </summary>
        [Test]
        public async Task APartyWithATOTPIsLetInWithItsTokenAndItsTOTP()
        {

            var url      = await CredentialsURL();
            var withIt   = await Send(url, $"Token {totpToken}", TOTP: CurrentTOTP());
            var without  = await Send(url, $"Token {totpToken}");

            Assert.Multiple(() => {
                Assert.That(withIt.Status,                                     Is.EqualTo(200),            withIt.Text);
                Assert.That(withIt.JSON?["data"]?.Value<String>("token"),      Is.EqualTo(totpToken.ToString()));
            });

            IsRefused(without, "The credentials asked for with the token of a party that signs in with a TOTP, but without it",
                      "Invalid Time-based One-Time Password (TOTP)!");

        }

        #endregion


        #region (private) IsRefused(Answer, What, Why = "Unknown access token!")

        /// <summary>
        /// Answered 401 with OCPI 2000 and the given message, and nothing of
        /// this platform in it.
        /// </summary>
        private void IsRefused(Answer  Answer,
                               String  What,
                               String  Why   = "Unknown access token!")
        {

            Assert.Multiple(() => {
                Assert.That(Answer.Status,                                Is.EqualTo(401),     $"{What} is not answered 401: {Answer.Text}");
                Assert.That(Answer.JSON?.Value<Int32?>("status_code"),    Is.EqualTo(2000),    $"{What} is not answered OCPI 2000: {Answer.Text}");
                Assert.That(Answer.JSON?.Value<String>("status_message"), Is.EqualTo(Why),     $"{What} is not told why.");
                Assert.That(Answer.JSON?["data"],                         Is.Null,             $"{What} is answered with data.");
                Assert.That(Answer.Text,  Does.Not.Contain("GraphDefined CSO"),       $"{What} is told this platform's business details.");
                Assert.That(Answer.Text,  Does.Not.Contain($":{port}/ocpi/versions"), $"{What} is told this platform's versions URL.");
            });

        }

        #endregion

        #region (private) CurrentTOTP()

        /// <summary>
        /// The TOTP of the party that signs in with one, as it is now.
        /// </summary>
        private static String CurrentTOTP()
        {

            var config = new TOTPConfig(totpSecret);

            var (_, current, _, _, _) = TOTPGenerator.GenerateTOTPs(config.SharedSecret,
                                                                     config.ValidityTime,
                                                                     config.Length,
                                                                     config.Alphabet);

            return current;

        }

        #endregion

        #region (private) CredentialsURL()

        /// <summary>
        /// Where this version's credentials are, as the other side finds them
        /// with its token: the versions, then the details of this version.
        /// </summary>
        private async Task<String> CredentialsURL()
        {

            var versions     = await Send($"http://127.0.0.1:{port}/ocpi/versions", $"Token {token}");
            var details      = (versions.JSON?["data"] as JArray)?.FirstOrDefault(version => version.Value<String>("version") == "2.3.0")?.Value<String>("url");

            Assert.That(details,      Is.Not.Null, $"The versions list does not list 2.3.0: {versions.Text}");

            var version      = await Send(details!, $"Token {token}");
            var credentials  = (version.JSON?["data"]?["endpoints"] as JArray)?.FirstOrDefault(endpoint => endpoint.Value<String>("identifier") == "credentials")?.Value<String>("url");

            Assert.That(credentials,  Is.Not.Null, $"This version has no credentials endpoint: {version.Text}");

            return credentials!;

        }

        #endregion

        #region (private) Send(URL, Authorization, TOTP = null)

        /// <summary>
        /// A GET with the given Authorization header and TOTP, if any - and what
        /// it was answered.
        /// </summary>
        private static async Task<Answer> Send(String   URL,
                                               String?  Authorization,
                                               String?  TOTP   = null)
        {

            using var http     = new HttpClient();
            using var request  = new HttpRequestMessage(HttpMethod.Get, URL);

            if (Authorization is not null)
                request.Headers.TryAddWithoutValidation("Authorization", Authorization);

            if (TOTP is not null)
                request.Headers.TryAddWithoutValidation("TOTP",          $"{(Byte) TOTPHTTPHeaderType.RAW} {TOTP}");

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
