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
using System.Collections.Concurrent;

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
    /// An OCPI request whose handling throws: its caller is answered OCPI
    /// 3000 with HTTP 500, a message that says nothing of this server, and
    /// the request and correlation ids it came with. What was thrown is said
    /// to OnRequestFailed instead.
    /// </summary>
    /// <remarks>
    /// Two ways to throw: a route of this test's own whose handler throws,
    /// and a remote party whose TOTP secret is too short to make a password
    /// with, which makes reading a request with its token throw - the request
    /// itself, before any handler, in this version and in the versions list.
    /// </remarks>
    [TestFixture]
    public class FailedRequestTests
    {

        #region Data

        private const           String          marker         = "C0FFEE-only-the-log-may-know-this";
        private const           String          requestId      = "a-request-id-of-the-caller";
        private const           String          correlationId  = "a-correlation-id-of-the-caller";
        private const           String          throwsPath     = "/ocpi/v2.3.0/throws";

        private static readonly RemoteParty_Id  id             = RemoteParty_Id.Parse("DE-BBB_EMSP");
        private static readonly AccessToken     token          = AccessToken.Parse("their-token-of-ours");
        private static readonly AccessToken     brokenToken    = AccessToken.Parse("their-token-with-a-broken-totp");

        private String       directory  = default!;
        private CommonAPI    api        = default!;
        private HTTPServer?  server;
        private UInt16       port;

        private readonly ConcurrentQueue<Report>        reports  = new();
        private readonly ConcurrentQueue<OCPIResponse>  heard    = new();

        #endregion

        #region SetUp / TearDown

        [SetUp]
        public async Task SetUp()
        {

            directory  = Path.Combine(Path.GetTempPath(), $"WWCP_OCPI_Tests-{Guid.NewGuid():N}");

            Directory.CreateDirectory(directory);

            reports.Clear();
            heard.  Clear();

            for (var attempt = 1; ; attempt++)
            {

                port    = FreePort();
                server  = new HTTPServer(IPAddress: IPv4Address.Localhost, TCPPort: IPPort.Parse(port));
                api     = ACommonAPI(server, port);

                api.AddOCPIMethod(
                    HTTPMethod.GET,
                    HTTPPath.Parse(throwsPath),
                    request => throw new InvalidOperationException(marker)
                );

                api.AddOCPIMethod(
                    HTTPMethod.GET,
                    HTTPPath.Parse(throwsPath + "-logged"),
                    OCPIRequestLogger:   (timestamp, ocpiAPI, request, cancellationToken) => Task.CompletedTask,
                    OCPIResponseLogger:  (timestamp, ocpiAPI, request, response, cancellationToken) => {
                                             heard.Enqueue(response);
                                             return Task.CompletedTask;
                                         },
                    OCPIRequestHandler:  request => throw new InvalidOperationException(marker)
                );

                try
                {
                    await server.Start();
                    break;
                }
                catch (SocketException) when (attempt < 5)
                { }

            }

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


        #region AThrowingHandlerTellsItsCallerNoMoreThanItsIds()

        /// <summary>
        /// A handler that throws: its caller, whom no token makes known, gets
        /// OCPI 3000 with HTTP 500, the message for it, and the ids it came
        /// with, and nothing of what was thrown, where or how.
        /// </summary>
        [Test]
        public async Task AThrowingHandlerTellsItsCallerNoMoreThanItsIds()
        {

            var answer = await Send(ThrowsURL, Authorization: null);

            SaysNothingButItsIds(answer, "The request whose handler threw");

        }

        #endregion

        #region AThrowingHandlerIsSaidToOnRequestFailed()

        /// <summary>
        /// What the caller is not told goes to OnRequestFailed: what was
        /// thrown, the request, the ids the caller was answered with, and who
        /// it was.
        /// </summary>
        [Test]
        public async Task AThrowingHandlerIsSaidToOnRequestFailed()
        {

            await AddTheOtherSide();

            Listen();

            var answer = await Send(ThrowsURL, $"Token {token}");

            SaysNothingButItsIds(answer, "The request whose handler threw");

            Assert.That(reports, Has.Count.EqualTo(1), "The request whose handler threw is not said to OnRequestFailed once.");

            var report = reports.Single();

            Assert.Multiple(() => {
                Assert.That(report.Exception,                  Is.InstanceOf<InvalidOperationException>(),  "What the handler threw is not what is said.");
                Assert.That(report.Exception.Message,          Is.EqualTo(marker),                          "What the handler threw is not what is said.");
                Assert.That(report.Request.Path.ToString(),    Is.EqualTo(throwsPath),                      "The request said is not the one that failed.");
                Assert.That(report.RequestId.ToString(),       Is.EqualTo(requestId),                       "The request id said is not the one the caller was answered with.");
                Assert.That(report.CorrelationId.ToString(),   Is.EqualTo(correlationId),                   "The correlation id said is not the one the caller was answered with.");
                Assert.That(report.RemotePartyId,              Is.EqualTo(id),                              "Who sent the request is not said, though its token was known.");
            });

        }

        #endregion

        #region ASubscriberThatThrowsChangesNothingForTheCaller()

        /// <summary>
        /// A subscriber of OnRequestFailed that throws changes nothing of the
        /// answer, and keeps no other subscriber from hearing of it.
        /// </summary>
        [Test]
        public async Task ASubscriberThatThrowsChangesNothingForTheCaller()
        {

            api.BaseAPI.OnRequestFailed += (_, _, _, _, _, _) =>
                throw new InvalidOperationException("A subscriber that throws.");

            Listen();

            var answer = await Send(ThrowsURL, Authorization: null);

            SaysNothingButItsIds(answer, "The request whose handler threw, said to a subscriber that throws,");

            Assert.That(reports, Has.Count.EqualTo(1), "The subscriber after the one that threw did not hear of the request.");

        }

        #endregion

        #region AResponseLoggerHearsWhatTheCallerWasAnswered()

        /// <summary>
        /// A response logger of the route hears what the caller was answered:
        /// OCPI 3000, the message for it and the ids - and not that there was
        /// no OCPI response at all.
        /// </summary>
        [Test]
        public async Task AResponseLoggerHearsWhatTheCallerWasAnswered()
        {

            var answer = await Send(ThrowsURL + "-logged", Authorization: null);

            SaysNothingButItsIds(answer, "The request whose handler threw");

            Assert.That(heard, Has.Count.EqualTo(1), "The response logger did not hear of the request once.");

            var response = heard.Single();

            Assert.Multiple(() => {
                Assert.That(response.StatusCode.Value,           Is.EqualTo(3000),                                "The response logger heard another status code than the caller was answered.");
                Assert.That(response.StatusMessage,              Is.EqualTo(CommonHTTPAPI.FailedRequestMessage),  "The response logger heard another message than the caller was answered.");
                Assert.That(response.RequestId?.    ToString(),  Is.EqualTo(requestId),                           "The response logger heard another request id than the caller was answered.");
                Assert.That(response.CorrelationId?.ToString(),  Is.EqualTo(correlationId),                       "The response logger heard another correlation id than the caller was answered.");
            });

        }

        #endregion

        #region ARequestWithoutIdsIsAnsweredWithTheOnesTheLogGets()

        /// <summary>
        /// A caller that sent no ids gets the ones made up for its request -
        /// the same ones the log gets, so that it has something to quote.
        /// </summary>
        [Test]
        public async Task ARequestWithoutIdsIsAnsweredWithTheOnesTheLogGets()
        {

            Listen();

            var answer = await Send(ThrowsURL, Authorization: null, WithIds: false);

            Assert.That(reports, Has.Count.EqualTo(1), "The request whose handler threw is not said to OnRequestFailed once.");

            var report = reports.Single();

            Assert.Multiple(() => {
                Assert.That(answer.Status,                                  Is.EqualTo(500),                         "The request whose handler threw is not answered 500.");
                Assert.That(answer.JSON?.Value<Int32?>("status_code"),      Is.EqualTo(3000),                        "The request whose handler threw is not answered OCPI 3000.");
                Assert.That(answer.RequestIdHeader,                         Is.EqualTo(report.RequestId.    ToString()),  "The request id answered is not the one the log gets.");
                Assert.That(answer.CorrelationIdHeader,                     Is.EqualTo(report.CorrelationId.ToString()),  "The correlation id answered is not the one the log gets.");
                Assert.That(answer.JSON?.Value<String>("requestId"),        Is.EqualTo(report.RequestId.    ToString()),  "The request id in the body is not the one the log gets.");
                Assert.That(answer.JSON?.Value<String>("correlationId"),    Is.EqualTo(report.CorrelationId.ToString()),  "The correlation id in the body is not the one the log gets.");
            });

        }

        #endregion

        #region ARequestThatCannotBeReadIsAnsweredWithTheIdsItCameWith()

        /// <summary>
        /// A request that cannot even be read - here, because the remote party
        /// its token names has a TOTP secret too short to make a password with -
        /// is answered as well: with the ids it came with, taken from its
        /// headers, and nothing of why.
        /// </summary>
        [Test]
        public async Task ARequestThatCannotBeReadIsAnsweredWithTheIdsItCameWith()
        {

            await AddAPartyWithABrokenTOTP();

            Listen();

            var answer = await Send(ThrowsURL, $"Token {brokenToken}");

            SaysNothingButItsIds(answer, "The request that could not be read");

            Assert.That(answer.Text,  Does.Not.Contain("secret"),            "The caller is told why its request could not be read.");
            Assert.That(reports,      Has.Count.EqualTo(1),                  "The request that could not be read is not said to OnRequestFailed once.");

            var report = reports.Single();

            Assert.Multiple(() => {
                Assert.That(report.Exception,                  Is.InstanceOf<ArgumentException>(),  "What is said is not why the request could not be read.");
                Assert.That(report.Exception.Message,          Does.Contain("shared secret"),       "What is said is not why the request could not be read.");
                Assert.That(report.RequestId.ToString(),       Is.EqualTo(requestId),               "The request id said is not the one the caller sent.");
                Assert.That(report.CorrelationId.ToString(),   Is.EqualTo(correlationId),           "The correlation id said is not the one the caller sent.");
                Assert.That(report.RemotePartyId,              Is.Null,                             "A remote party is said, though the request could not be read.");
            });

        }

        #endregion

        #region TheVersionsListTellsItsCallerNoMoreThanItsIds()

        /// <summary>
        /// The same for the versions list, which the API every version hangs
        /// off serves, and which reads the request on its own.
        /// </summary>
        [Test]
        public async Task TheVersionsListTellsItsCallerNoMoreThanItsIds()
        {

            await AddAPartyWithABrokenTOTP();

            Listen();

            var answer = await Send($"http://127.0.0.1:{port}/ocpi/versions", $"Token {brokenToken}");

            SaysNothingButItsIds(answer, "The versions list that could not be read");

            Assert.That(answer.Text,  Does.Not.Contain("secret"),  "The caller is told why its request could not be read.");
            Assert.That(reports,      Has.Count.EqualTo(1),        "The versions list that could not be read is not said to OnRequestFailed once.");

            var report = reports.Single();

            Assert.Multiple(() => {
                Assert.That(report.Exception.Message,          Does.Contain("shared secret"),  "What is said is not why the request could not be read.");
                Assert.That(report.Request.Path.ToString(),    Is.EqualTo("/ocpi/versions"),   "The request said is not the one that failed.");
                Assert.That(report.RequestId.ToString(),       Is.EqualTo(requestId),          "The request id said is not the one the caller sent.");
            });

        }

        #endregion


        #region (private) SaysNothingButItsIds(Answer, What)

        /// <summary>
        /// The answer to a request whose handling threw: OCPI 3000 with HTTP
        /// 500, the message for it, and the ids the caller sent - as headers
        /// and in the body - and nothing of what was thrown.
        /// </summary>
        private static void SaysNothingButItsIds(Answer  Answer,
                                                 String  What)
        {

            Assert.Multiple(() => {

                Assert.That(Answer.Status,                                Is.EqualTo(500),                                 $"{What} is not answered 500: {Answer.Text}");
                Assert.That(Answer.JSON?.Value<Int32?>("status_code"),    Is.EqualTo(3000),                                $"{What} is not answered OCPI 3000: {Answer.Text}");
                Assert.That(Answer.JSON?.Value<String>("status_message"), Is.EqualTo(CommonHTTPAPI.FailedRequestMessage),  $"{What} is not answered with the message for it.");
                Assert.That(Answer.JSON?["data"],                         Is.Null,                                         $"{What} is answered with data.");

                Assert.That(Answer.RequestIdHeader,                       Is.EqualTo(requestId),                           $"{What} is not answered with the request id it came with.");
                Assert.That(Answer.CorrelationIdHeader,                   Is.EqualTo(correlationId),                       $"{What} is not answered with the correlation id it came with.");
                Assert.That(Answer.JSON?.Value<String>("requestId"),      Is.EqualTo(requestId),                           $"{What} does not say the request id it came with.");
                Assert.That(Answer.JSON?.Value<String>("correlationId"),  Is.EqualTo(correlationId),                       $"{What} does not say the correlation id it came with.");

                Assert.That(Answer.Text,  Does.Not.Contain(marker),                   $"{What} is told what was thrown.");
                Assert.That(Answer.Text,  Does.Not.Contain("stacktrace").IgnoreCase,  $"{What} is told a stack trace.");
                Assert.That(Answer.Text,  Does.Not.Contain(" at "),                   $"{What} is told where it was thrown.");
                Assert.That(Answer.Text,  Does.Not.Contain(".cs"),                    $"{What} is told a source file.");
                Assert.That(Answer.Text,  Does.Not.Contain("Exception"),              $"{What} is told what was thrown.");

            });

        }

        #endregion

        #region (private) Listen()

        /// <summary>
        /// Every request said to OnRequestFailed, into the reports.
        /// </summary>
        private void Listen()
        {
            api.BaseAPI.OnRequestFailed += (timestamp, request, requestIdSaid, correlationIdSaid, remotePartyId, exception) => {
                reports.Enqueue(new Report(request, requestIdSaid, correlationIdSaid, remotePartyId, exception));
                return Task.CompletedTask;
            };
        }

        #endregion

        #region (private) AddTheOtherSide() / AddAPartyWithABrokenTOTP()

        /// <summary>
        /// The other side as a remote party, known by its token.
        /// </summary>
        private async Task AddTheOtherSide()
        {

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

        /// <summary>
        /// A remote party whose TOTP secret is too short to make a password
        /// with: reading a request with its token throws.
        /// </summary>
        private async Task AddAPartyWithABrokenTOTP()
        {

            var added = await api.AddRemoteParty(RemoteParty_Id.Parse("DE-CCC_EMSP"),
                                                [
                                                    new CredentialsRole(
                                                        CountryCode.Parse("DE"),
                                                        Party_Id.   Parse("CCC"),
                                                        Role.EMSP,
                                                        new BusinessDetails("An EMSP with a broken TOTP")
                                                    )
                                                ],
                                                brokenToken,
                                                LocalAccessTokenBase64Encoding: false,
                                                LocalTOTPConfig:                new TOTPConfig("too-short"));

            Assert.That(added.IsSuccess, Is.True, $"The party with a broken TOTP could not be added: {added.ErrorResponse}");

        }

        #endregion

        #region (private) ThrowsURL / Send(URL, Authorization, WithIds = true)

        /// <summary>
        /// The route of this test whose handler throws.
        /// </summary>
        private String ThrowsURL
            => $"http://127.0.0.1:{port}{throwsPath}";

        /// <summary>
        /// A GET with the given Authorization header, if any, and the ids of
        /// the caller unless told otherwise - and what it was answered.
        /// </summary>
        private static async Task<Answer> Send(String   URL,
                                               String?  Authorization,
                                               Boolean  WithIds   = true)
        {

            using var http     = new HttpClient();
            using var request  = new HttpRequestMessage(HttpMethod.Get, URL);

            if (Authorization is not null)
                request.Headers.TryAddWithoutValidation("Authorization",     Authorization);

            if (WithIds)
            {
                request.Headers.TryAddWithoutValidation("X-Request-ID",      requestId);
                request.Headers.TryAddWithoutValidation("X-Correlation-ID",  correlationId);
            }

            using var response = await http.SendAsync(request);

            var text = await response.Content.ReadAsStringAsync();

            return new Answer(
                       (Int32) response.StatusCode,
                       text.StartsWith('{') ? JObject.Parse(text) : null,
                       text,
                       response.Headers.TryGetValues("X-Request-ID",      out var requestIds)      ? requestIds.     FirstOrDefault() : null,
                       response.Headers.TryGetValues("X-Correlation-ID",  out var correlationIds)  ? correlationIds. FirstOrDefault() : null
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

        #region (private) Answer / Report

        /// <summary>
        /// What a request was answered: the HTTP status, the body, and the ids
        /// in the headers.
        /// </summary>
        private sealed record Answer(Int32     Status,
                                     JObject?  JSON,
                                     String    Text,
                                     String?   RequestIdHeader,
                                     String?   CorrelationIdHeader);

        /// <summary>
        /// What OnRequestFailed said.
        /// </summary>
        private sealed record Report(HTTPRequest      Request,
                                     Request_Id       RequestId,
                                     Correlation_Id   CorrelationId,
                                     RemoteParty_Id?  RemotePartyId,
                                     Exception        Exception);

        #endregion

    }

}
