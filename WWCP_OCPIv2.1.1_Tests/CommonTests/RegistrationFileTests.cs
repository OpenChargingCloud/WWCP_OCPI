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

namespace cloud.charging.open.protocols.OCPIv2_1_1.UnitTests.CommonTests
{

    /// <summary>
    /// A registration with the other side while the file the remote parties
    /// are kept in cannot be written: nothing sent where it cannot take the
    /// token the other side would call back with; kept where the other side
    /// accepted, because it uses the new tokens already, and written down
    /// once the file takes lines again.
    /// </summary>
    /// <remarks>
    /// The other side is a stub with the three routes a registration walks -
    /// the versions, the details of one, the credentials - which answers with
    /// credentials of its own, and makes the file unwritable on request just
    /// before. Unwritable the way that stops root as well: a directory where
    /// the file would be. The next start is a Common API made anew on the
    /// same directory.
    /// </remarks>
    [TestFixture]
    public class RegistrationFileTests
    {

        #region Data

        private static readonly RemoteParty_Id  id      = RemoteParty_Id.Parse("DE-BBB_EMSP");
        private const           String          tokenA  = "their-token-a";
        private const           String          tokenC  = "their-token-c";

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

            api        = ACommonAPI();
            other      = await OtherSide.Start();

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


        #region NothingIsSentWhereTheTokenToBeCalledBackWithCannotBeStored()

        /// <summary>
        /// The token the other side would call back with is stored before
        /// anything is sent; where the file refuses it, nothing is sent, and
        /// nothing has changed.
        /// </summary>
        [Test]
        public async Task NothingIsSentWhereTheTokenToBeCalledBackWithCannotBeStored()
        {

            var ourToken    = AccessToken.NewRandom();
            var party       = await AddTheOtherSide(ourToken);

            BlockTheFile();

            var registered  = await AClient(party).TryRegister();

            Assert.Multiple(() => {
                Assert.That(registered.NotSaved,          Is.True,                         "The file refused the token to be called back with, and the result does not say so.");
                Assert.That(registered.Reason,            Does.Contain(FileName),          "The result does not name the file that refused.");
                Assert.That(registered.Response.Data,     Is.Null,                         "The other side answered, though nothing was to be sent.");
                Assert.That(other.ReceivedCredentials,    Is.Null,                         "The credentials were sent, though the token to be called back with could not be stored.");
                Assert.That(LocalTokensOf(api),           Is.EqualTo(new[] { ourToken }),  "The token to be called back with is kept, though its file refused it.");
                Assert.That(api.UnsavedRemoteParties,     Is.Empty,                        "Something is said to be kept, though nothing was.");
            });

            UnblockTheFile();

            Assert.That(LocalTokensOf(ACommonAPI()), Is.EqualTo(new[] { ourToken }), "The next start knows a token its file refused.");

        }

        #endregion

        #region ARegistrationTheOtherSideAcceptedIsKept()

        /// <summary>
        /// Where the other side accepted and the file refuses what its answer
        /// changed, the change is kept - in effect, said to be unsaved - and
        /// written down once the file takes lines again.
        /// </summary>
        [Test]
        public async Task ARegistrationTheOtherSideAcceptedIsKept()
        {

            var party       = await AddTheOtherSide(AccessToken.NewRandom());

            other.WhenCredentialsArrive = BlockTheFile;

            var registered  = await AClient(party).TryRegister();
            var tokenB      = other.ReceivedCredentials?.Value<String>("token");

            Assert.Multiple(() => {
                Assert.That(registered.NotSaved,                          Is.True,                    "The file refused what the answer changed, and the result does not say so.");
                Assert.That(registered.Reason,                            Does.Contain(FileName),     "The result does not name the file that refused.");
                Assert.That(registered.Response.Data?.Token.ToString(),   Is.EqualTo(tokenC),         "The answer of the other side is not handed back.");
                Assert.That(LocalTokensOf(api).Select(token => token.ToString()), Is.EqualTo(new[] { tokenB }), "The token the other side calls back with is not the one in effect.");
                Assert.That(RemoteTokenOf(api),                           Is.EqualTo(tokenC),         "The token the other side handed out is not the one in effect.");
                Assert.That(api.UnsavedRemoteParties,                     Is.EqualTo(new[] { id }),   "The registration kept is not said to be unsaved.");
            });

            UnblockTheFile();

            Assert.That(await api.WriteDownUnsavedRemoteParties(), Is.Null, "The registration kept could not be written down, though its file takes lines again.");

            var again = ACommonAPI();

            Assert.Multiple(() => {
                Assert.That(LocalTokensOf(again).Select(token => token.ToString()), Is.EqualTo(new[] { tokenB }), "The next start does not know the token the other side calls back with.");
                Assert.That(RemoteTokenOf(again),                                   Is.EqualTo(tokenC),           "The next start does not know the token the other side handed out.");
            });

        }

        #endregion

        #region ARegistrationItsFileTakesIsSaved()

        /// <summary>
        /// A registration the file takes is not said to be unsaved, and the
        /// next start knows it.
        /// </summary>
        [Test]
        public async Task ARegistrationItsFileTakesIsSaved()
        {

            var party       = await AddTheOtherSide(AccessToken.NewRandom());

            var registered  = await AClient(party).TryRegister();
            var tokenB      = other.ReceivedCredentials?.Value<String>("token");

            Assert.Multiple(() => {
                Assert.That(registered.NotSaved,                          Is.False,           $"The registration is said to be unsaved: {registered.Reason}");
                Assert.That(registered.Reason,                            Is.Null,            "The registration the file took has a reason it did not.");
                Assert.That(registered.Response.Data?.Token.ToString(),   Is.EqualTo(tokenC), $"The registration did not go through: {registered.Response.StatusMessage}");
                Assert.That(api.UnsavedRemoteParties,                     Is.Empty,           "Something is said to be kept, though the file took everything.");
            });

            var again = ACommonAPI();

            Assert.Multiple(() => {
                Assert.That(LocalTokensOf(again).Select(token => token.ToString()), Is.EqualTo(new[] { tokenB }), "The next start does not know the token the other side calls back with.");
                Assert.That(RemoteTokenOf(again),                                   Is.EqualTo(tokenC),           "The next start does not know the token the other side handed out.");
            });

        }

        #endregion


        #region TheOtherSideRegisteringWhileTheFileRefusesKeepsItsToken()

        /// <summary>
        /// The other side registering here while the file cannot take it is
        /// answered OCPI 3000 with HTTP 500, and nothing changed: the token it
        /// came with is still the one it has, now and at the next start.
        /// </summary>
        [Test]
        public async Task TheOtherSideRegisteringWhileTheFileRefusesKeepsItsToken()
        {

            var tokenOfOurs  = AccessToken.NewRandom();

            await Listening(tokenOfOurs);

            var credentials  = await CredentialsURL(tokenOfOurs);

            BlockTheFile();

            var (status, statusCode, _) = await Send(HTTPMethod.POST, credentials, tokenOfOurs);

            Assert.Multiple(() => {
                Assert.That(status,              Is.EqualTo(500),                    "The registration the file refused is not answered 500.");
                Assert.That(statusCode,          Is.EqualTo(3000),                   "The registration the file refused is not answered OCPI 3000.");
                Assert.That(LocalTokensOf(api),  Is.EqualTo(new[] { tokenOfOurs }),  "The token the other side came with is not the one it has any more.");
                Assert.That(RemoteTokenOf(api),  Is.Null,                            "The token the other side sent is kept, though the file refused it.");
            });

            UnblockTheFile();

            var again = ACommonAPI();

            Assert.Multiple(() => {
                Assert.That(LocalTokensOf(again),  Is.EqualTo(new[] { tokenOfOurs }),  "The next start does not know the token the other side came with.");
                Assert.That(RemoteTokenOf(again),  Is.Null,                            "The next start knows the token the other side sent, though the file refused it.");
            });

        }

        #endregion

        #region TheOtherSideUnregisteringWhileTheFileRefusesStaysRegistered()

        /// <summary>
        /// The other side unregistering while the file cannot take it is
        /// answered OCPI 3000 with HTTP 500, and stays registered, its token
        /// valid, now and at the next start.
        /// </summary>
        [Test]
        public async Task TheOtherSideUnregisteringWhileTheFileRefusesStaysRegistered()
        {

            var tokenOfOurs  = AccessToken.NewRandom();

            await Listening(tokenOfOurs);

            var credentials  = await CredentialsURL(tokenOfOurs);

            var (registered, registeredCode, answer) = await Send(HTTPMethod.POST, credentials, tokenOfOurs);

            Assert.That(registeredCode, Is.EqualTo(1000), $"The registration answered {registered}: {answer}");

            var tokenNow     = AccessToken.Parse(answer?["data"]?.Value<String>("token") ?? "");

            BlockTheFile();

            var (status, statusCode, _) = await Send(HTTPMethod.DELETE, credentials, tokenNow);

            Assert.Multiple(() => {
                Assert.That(status,              Is.EqualTo(500),                  "The unregistration the file refused is not answered 500.");
                Assert.That(statusCode,          Is.EqualTo(3000),                 "The unregistration the file refused is not answered OCPI 3000.");
                Assert.That(LocalTokensOf(api),  Is.EqualTo(new[] { tokenNow }),   "The token of the other side is gone, though the file refused its unregistration.");
                Assert.That(RemoteTokenOf(api),  Is.EqualTo(tokenC),               "The token the other side sent is gone, though the file refused its unregistration.");
            });

            UnblockTheFile();

            var again = ACommonAPI();

            Assert.Multiple(() => {
                Assert.That(LocalTokensOf(again),  Is.EqualTo(new[] { tokenNow }),  "The next start does not know the token of the other side.");
                Assert.That(RemoteTokenOf(again),  Is.EqualTo(tokenC),              "The next start does not know the token the other side sent.");
            });

        }

        #endregion


        #region (private) Listening(TokenOfOurs) / CredentialsURL(Token) / Send(Method, URL, Token)

        /// <summary>
        /// This test's Common API made anew, listening on a port nobody was
        /// listening on a moment ago, with the other side to come: the given
        /// token of ours, handed out and not yet used.
        /// </summary>
        private async Task Listening(AccessToken TokenOfOurs)
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

            var added = await api.AddRemoteParty(CountryCode.Parse("DE"),
                                                Party_Id.   Parse("BBB"),
                                                Role.EMSP,
                                                new BusinessDetails("Their EMSP"),
                                                TokenOfOurs);

            Assert.That(added.IsSuccess, Is.True, $"The other side to come could not be added: {added.ErrorResponse}");

        }

        /// <summary>
        /// Where this API's credentials endpoint is, as the other side finds
        /// it with the given token: the versions, then the details of this
        /// version.
        /// </summary>
        private async Task<String> CredentialsURL(AccessToken Token)
        {

            var (_, _, versions)  = await Send(HTTPMethod.GET, $"http://127.0.0.1:{port}/ocpi/versions", Token);
            var details           = (versions?["data"] as JArray)?.First(version => version.Value<String>("version") == "2.1.1").Value<String>("url");

            Assert.That(details, Is.Not.Null, $"This API does not list 2.1.1: {versions}");

            var (_, _, version)   = await Send(HTTPMethod.GET, details!, Token);
            var credentials       = (version?["data"]?["endpoints"] as JArray)?.First(endpoint => endpoint.Value<String>("identifier") == "credentials").Value<String>("url");

            Assert.That(credentials, Is.Not.Null, $"This API has no credentials endpoint: {version}");

            return credentials!;

        }

        /// <summary>
        /// A request of the other side to this API, with the given token -
        /// its credentials as the body of a POST - and what it was answered:
        /// the HTTP status, the OCPI status code, and the answer.
        /// </summary>
        private async Task<(Int32 Status, Int32? StatusCode, JObject? Answer)> Send(HTTPMethod   Method,
                                                                                   String       URL,
                                                                                   AccessToken  Token)
        {

            using var http     = new HttpClient();
            using var request  = new HttpRequestMessage(new HttpMethod(Method.ToString()), URL);

            request.Headers.TryAddWithoutValidation("Authorization", $"Token {Token}");

            if (Method == HTTPMethod.POST)
                request.Content = new StringContent(
                                      new JObject(
                                          new JProperty("token",  tokenC),
                                          new JProperty("url",    other.VersionsURL.ToString()),
                                          new JProperty("business_details",  new JObject(
                                              new JProperty("name",  "Their EMSP")
                                          )),
                                          new JProperty("party_id",          "BBB"),
                                          new JProperty("country_code",      "DE")
                                      ).ToString(),
                                      Encoding.UTF8,
                                      "application/json"
                                  );

            using var response = await http.SendAsync(request);

            var text   = await response.Content.ReadAsStringAsync();
            var answer = text.StartsWith('{') ? JObject.Parse(text) : null;

            return ((Int32) response.StatusCode, answer?.Value<Int32?>("status_code"), answer);

        }

        #endregion

        #region (private) AddTheOtherSide(OurToken) / AClient(Party)

        /// <summary>
        /// The other side as a remote party: the given token of ours, and its
        /// versions URL and token A to start the registration with.
        /// </summary>
        private async Task<RemoteParty> AddTheOtherSide(AccessToken OurToken)
        {

            var added = await api.AddRemoteParty(CountryCode.Parse("DE"),
                                                Party_Id.   Parse("BBB"),
                                                Role.EMSP,
                                                new BusinessDetails("Their EMSP"),
                                                OurToken,
                                                other.VersionsURL,
                                                AccessToken.Parse(tokenA));

            Assert.That(added.IsSuccess, Is.True, $"The other side could not be added: {added.ErrorResponse}");

            return added.Data!;

        }

        /// <summary>
        /// A client to the other side, for the given remote party.
        /// </summary>
        private CommonHTTPClient AClient(RemoteParty Party)

            => new (
                   CommonAPI:    api,
                   RemoteParty:  Party
               );

        #endregion

        #region (private) FileName / LocalTokensOf(API) / RemoteTokenOf(API)

        /// <summary>
        /// The name of the file of the remote parties, which a refusal names.
        /// </summary>
        private String FileName
            => Path.GetFileName(api.RemotePartyDBFileName);

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

            => API.RemoteParties.
                   Where     (party => party.Id == id).
                   SelectMany(party => party.RemoteAccessInfos.Select(info => info.AccessToken.ToString())).
                   FirstOrDefault();

        #endregion

        #region (private) ACommonAPI()

        /// <summary>
        /// A Common API on this test's directory. Made anew, it reads back what
        /// the one before it wrote, as the next start of a node does.
        /// </summary>
        private CommonAPI ACommonAPI()

            => ACommonAPI(new HTTPServer(TCPPort: IPPort.Parse(3999)), 3999);

        /// <summary>
        /// The same on the given HTTP server, which says it is on the given
        /// port.
        /// </summary>
        private CommonAPI ACommonAPI(HTTPServer  Server,
                                     UInt16      Port)

            => new (

                   OurBusinessDetails:  new BusinessDetails("GraphDefined CSO"),
                   OurCountryCode:      CountryCode.Parse("DE"),
                   OurPartyId:          Party_Id.   Parse("GEF"),
                   OurRole:             Role.CPO,

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

                   URLPathPrefix:       HTTPPath.Parse("/ocpi/v2.1.1"),
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
        /// The other side of a registration: the versions, the details of
        /// 2.1.1 and the credentials, on a port nobody was listening on a
        /// moment ago. It records the credentials it was sent, and answers
        /// with its own.
        /// </summary>
        private sealed class OtherSide : IAsyncDisposable
        {

            private readonly HTTPServer server;

            /// <summary>Where it says its versions are.</summary>
            public URL       VersionsURL            { get; }

            /// <summary>The credentials it was sent, if any.</summary>
            public JObject?  ReceivedCredentials    { get; private set; }

            /// <summary>Something to do once credentials have arrived, before the answer goes back.</summary>
            public Action?   WhenCredentialsArrive  { get; set; }

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
                    request => Task.FromResult(JSON(request, new JArray(
                                   new JObject(
                                       new JProperty("version",  "2.1.1"),
                                       new JProperty("url",      $"{origin}/versions/2.1.1")
                                   )
                               ))),
                    HTTPMethod.GET
                );

                api.AddHandler(
                    HTTPPath.Parse("/versions/2.1.1"),
                    request => Task.FromResult(JSON(request, new JObject(
                                   new JProperty("version",    "2.1.1"),
                                   new JProperty("endpoints",  new JArray(
                                       new JObject(
                                           new JProperty("identifier",  "credentials"),
                                           new JProperty("url",         $"{origin}/2.1.1/credentials")
                                       )
                                   ))
                               ))),
                    HTTPMethod.GET
                );

                api.AddHandler(
                    HTTPPath.Parse("/2.1.1/credentials"),
                    request => {

                        other.ReceivedCredentials = JObject.Parse(request.HTTPBodyAsUTF8String ?? "{}");
                        other.WhenCredentialsArrive?.Invoke();

                        return Task.FromResult(JSON(request, new JObject(
                                   new JProperty("token",  tokenC),
                                   new JProperty("url",    $"{origin}/versions"),
                                   new JProperty("business_details",  new JObject(
                                       new JProperty("name",  "Their EMSP")
                                   )),
                                   new JProperty("party_id",      "BBB"),
                                   new JProperty("country_code",  "DE")
                               )));

                    },
                    HTTPMethod.POST
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
