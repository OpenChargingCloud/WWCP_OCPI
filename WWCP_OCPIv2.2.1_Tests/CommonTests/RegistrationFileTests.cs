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
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

using Newtonsoft.Json.Linq;

using NUnit.Framework;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2_1.UnitTests.CommonTests
{

    /// <summary>
    /// A registration with the other side - or its renewal - while the file
    /// the remote parties are kept in cannot be written: nothing sent where it
    /// cannot take the token the other side would call back with; kept where
    /// the other side accepted, because it uses the new tokens already, and
    /// written down once the file takes lines again.
    /// </summary>
    /// <remarks>
    /// The other side is a stub with the three routes a registration walks -
    /// the versions, the details of one, the credentials - which answers with
    /// credentials of its own, and makes the file unwritable on request just
    /// before. Unwritable the way that stops root as well: a directory where
    /// the file would be. A renewal it calls back before it answers, with the
    /// token it was sent, as this library's own receiver does. The next start
    /// is a Common API made anew on the same directory.
    /// </remarks>
    [TestFixture]
    public class RegistrationFileTests
    {

        #region Data

        private static readonly RemoteParty_Id  id      = RemoteParty_Id.Parse("DE-BBB_CPO");
        private const           String          tokenA  = "their-token-a";
        private const           String          tokenC  = "their-token-c";

        // How the other side is reached, beside its tokens.
        private static readonly DateTimeOffset  notAfter    = new (2036, 1, 1, 0, 0, 0, TimeSpan.Zero);
        private const           String          userAgent   = "Our OCPI client";
        private static readonly TimeSpan        timeout     = TimeSpan.FromSeconds(23);
        private const           UInt16          retries     = 2;
        private const           SslProtocols    tls         = SslProtocols.Tls12 | SslProtocols.Tls13;
        private static readonly HTTPContentType contentType = HTTPContentType.Application.JSON_UTF8;
        private static readonly AcceptTypes     accept      = AcceptTypes.FromHTTPContentTypes(HTTPContentType.Application.JSON_UTF8);

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


        #region ARenewalItsFileTakesIsSaved()

        /// <summary>
        /// A renewal - our credentials put onto the other side once more, with
        /// a new token - which the other side calls back with that token before
        /// it answers: the token opens our door already, the renewal goes
        /// through, and the next start knows the new tokens of both sides.
        /// </summary>
        [Test]
        public async Task ARenewalItsFileTakesIsSaved()
        {

            var newToken  = AccessToken.NewRandom();

            await Listening();

            var party     = await AddTheOtherSide(AccessToken.NewRandom());

            var renewed   = await AClient(party).TryPutCredentials(OurCredentials(newToken));

            Assert.Multiple(() => {
                Assert.That(other.CalledBack?.StatusCode,                 Is.EqualTo(1000),               $"The other side called back with the new token, and was answered {other.CalledBack}.");
                Assert.That(renewed.Response.Data?.Token.ToString(),      Is.EqualTo(tokenC),             $"The renewal did not go through: {renewed.Response.StatusMessage}");
                Assert.That(renewed.NotSaved,                             Is.False,                       $"The renewal is said to be unsaved: {renewed.Reason}");
                Assert.That(LocalTokensOf(api),                           Is.EqualTo(new[] { newToken }), "The token the other side calls back with is not the new one alone.");
                Assert.That(RemoteTokenOf(api),                           Is.EqualTo(tokenC),             "The token the other side handed out is not the one in effect.");
                Assert.That(api.UnsavedRemoteParties,                     Is.Empty,                       "Something is said to be kept, though the file took everything.");
            });

            var again = ACommonAPI();

            Assert.Multiple(() => {
                Assert.That(LocalTokensOf(again),  Is.EqualTo(new[] { newToken }),  "The next start does not know the token the other side calls back with.");
                Assert.That(RemoteTokenOf(again),  Is.EqualTo(tokenC),              "The next start does not know the token the other side handed out.");
            });

        }

        #endregion

        #region NothingIsSentWhereTheNewTokenOfARenewalCannotBeStored()

        /// <summary>
        /// The new token of a renewal is stored before anything is sent; where
        /// the file refuses it, nothing is sent, and nothing has changed.
        /// </summary>
        [Test]
        public async Task NothingIsSentWhereTheNewTokenOfARenewalCannotBeStored()
        {

            var ourToken  = AccessToken.NewRandom();
            var party     = await AddTheOtherSide(ourToken);

            BlockTheFile();

            var renewed   = await AClient(party).TryPutCredentials(OurCredentials(AccessToken.NewRandom()));

            Assert.Multiple(() => {
                Assert.That(renewed.NotSaved,          Is.True,                         "The file refused the new token, and the result does not say so.");
                Assert.That(renewed.Reason,            Does.Contain(FileName),          "The result does not name the file that refused.");
                Assert.That(renewed.Response.Data,     Is.Null,                         "The other side answered, though nothing was to be sent.");
                Assert.That(other.ReceivedCredentials, Is.Null,                         "The credentials were sent, though their new token could not be stored.");
                Assert.That(LocalTokensOf(api),        Is.EqualTo(new[] { ourToken }),  "The new token is kept, though its file refused it.");
                Assert.That(RemoteTokenOf(api),        Is.EqualTo(tokenA),              "The token the other side has is not the one in effect any more.");
                Assert.That(api.UnsavedRemoteParties,  Is.Empty,                        "Something is said to be kept, though nothing was.");
            });

            UnblockTheFile();

            Assert.That(LocalTokensOf(ACommonAPI()), Is.EqualTo(new[] { ourToken }), "The next start knows a token its file refused.");

        }

        #endregion

        #region ARenewalTheOtherSideAcceptedIsKept()

        /// <summary>
        /// Where the other side accepted a renewal and the file refuses what
        /// its answer changed, the change is kept - in effect, said to be
        /// unsaved - and written down once the file takes lines again.
        /// </summary>
        [Test]
        public async Task ARenewalTheOtherSideAcceptedIsKept()
        {

            var newToken  = AccessToken.NewRandom();

            await Listening();

            var party     = await AddTheOtherSide(AccessToken.NewRandom());

            other.WhenCredentialsArrive = BlockTheFile;

            var renewed   = await AClient(party).TryPutCredentials(OurCredentials(newToken));

            Assert.Multiple(() => {
                Assert.That(renewed.NotSaved,                             Is.True,                        "The file refused what the answer changed, and the result does not say so.");
                Assert.That(renewed.Reason,                               Does.Contain(FileName),         "The result does not name the file that refused.");
                Assert.That(renewed.Response.Data?.Token.ToString(),      Is.EqualTo(tokenC),             $"The answer of the other side is not handed back: {renewed.Response.StatusMessage}");
                Assert.That(LocalTokensOf(api),                           Is.EqualTo(new[] { newToken }), "The token the other side calls back with is not the one in effect.");
                Assert.That(RemoteTokenOf(api),                           Is.EqualTo(tokenC),             "The token the other side handed out is not the one in effect.");
                Assert.That(api.UnsavedRemoteParties,                     Is.EqualTo(new[] { id }),       "The renewal kept is not said to be unsaved.");
            });

            UnblockTheFile();

            Assert.That(await api.WriteDownUnsavedRemoteParties(), Is.Null, "The renewal kept could not be written down, though its file takes lines again.");

            var again = ACommonAPI();

            Assert.Multiple(() => {
                Assert.That(LocalTokensOf(again),  Is.EqualTo(new[] { newToken }),  "The next start does not know the token the other side calls back with.");
                Assert.That(RemoteTokenOf(again),  Is.EqualTo(tokenC),              "The next start does not know the token the other side handed out.");
            });

        }

        #endregion

        #region ARenewalTheOtherSideRefusedLeavesTheTokensInEffect()

        /// <summary>
        /// A renewal the other side refused after it called back - as this
        /// library's own receiver refuses one its file cannot take - changed
        /// nothing there: the token it holds still opens our door, now and at
        /// the next start, and the one it handed out is still the one we call
        /// it with.
        /// </summary>
        [Test]
        public async Task ARenewalTheOtherSideRefusedLeavesTheTokensInEffect()
        {

            var ourToken  = AccessToken.NewRandom();

            await Listening();

            var party     = await AddTheOtherSide(ourToken);

            other.RefusesRenewals = true;

            var renewed   = await AClient(party).TryPutCredentials(OurCredentials(AccessToken.NewRandom()));

            Assert.Multiple(() => {
                Assert.That(other.CalledBack?.StatusCode,        Is.EqualTo(1000),          $"The other side called back with the new token, and was answered {other.CalledBack}.");
                Assert.That(renewed.Response.StatusCode.Value,   Is.EqualTo(3000),          $"The refusal of the other side is not handed back: {renewed.Response.StatusMessage}");
                Assert.That(renewed.NotSaved,                    Is.False,                  $"The renewal is said to be unsaved: {renewed.Reason}");
                Assert.That(LocalTokensOf(api),                  Does.Contain(ourToken),    "The token the other side still holds does not open our door any more.");
                Assert.That(RemoteTokenOf(api),                  Is.EqualTo(tokenA),        "The token the other side still has is not the one we call it with any more.");
            });

            var again = ACommonAPI();

            Assert.Multiple(() => {
                Assert.That(LocalTokensOf(again),  Does.Contain(ourToken),  "The next start does not know the token the other side still holds.");
                Assert.That(RemoteTokenOf(again),  Is.EqualTo(tokenA),      "The next start does not know the token the other side still has.");
            });

        }

        #endregion

        #region ARenewalSentOnceMoreAddsItsTokenOnce()

        /// <summary>
        /// A renewal sent once more - its first answer a gateway timeout, a
        /// reason to send it again - adds its new token once, not once for
        /// every time it was sent.
        /// </summary>
        [Test]
        public async Task ARenewalSentOnceMoreAddsItsTokenOnce()
        {

            var ourToken  = AccessToken.NewRandom();
            var newToken  = AccessToken.NewRandom();

            await Listening();

            var party     = await AddTheOtherSide(ourToken);

            other.TimesOutOnce     = true;
            other.RefusesRenewals  = true;

            var renewed   = await AClient(party).TryPutCredentials(OurCredentials(newToken));

            Assert.Multiple(() => {
                Assert.That(other.Renewals,                     Is.EqualTo(2),                             "The renewal was not sent once more after the gateway timed out.");
                Assert.That(renewed.Response.StatusCode.Value,  Is.EqualTo(3000),                          $"The refusal of the other side is not handed back: {renewed.Response.StatusMessage}");
                Assert.That(LocalTokensOf(api),                 Is.EqualTo(new[] { ourToken, newToken }),  "The new token is not there once, beside the one the other side holds.");
            });

        }

        #endregion

        #region ARenewalKeepsHowTheOtherSideIsReached()

        /// <summary>
        /// A renewal changes the tokens, and nothing else of how the other side
        /// is reached: its client certificate - without which a peer that asks
        /// for one would shut us out - the TLS versions, IPv4 first, the
        /// timeout, the retries, the user agent and until when its token may be
        /// used are as they were, and so are when it was added and the versions
        /// it sees. The next start knows what its file reads back of them.
        /// </summary>
        [Test]
        public async Task ARenewalKeepsHowTheOtherSideIsReached()
        {

            var       newToken     = AccessToken.NewRandom();
            using var certificate  = AClientCertificate();

            await Listening();

            var party    = await AddTheOtherSide(AccessToken.NewRandom(), certificate);

            var renewed  = await AClient(party).TryPutCredentials(OurCredentials(newToken));

            Assert.That(renewed.Response.Data?.Token.ToString(), Is.EqualTo(tokenC), $"The renewal did not go through: {renewed.Response.StatusMessage}");
            Assert.That(LocalTokensOf(api), Is.EqualTo(new[] { newToken }), "The token the other side calls back with is not the new one alone.");

            ReachedAsItWas(party, certificate);

        }

        #endregion

        #region ARegistrationKeepsHowTheOtherSideIsReached()

        /// <summary>
        /// A registration changes the tokens, and nothing else of how the other
        /// side is reached: its client certificate - without which a peer that
        /// asks for one would shut us out - the TLS versions, IPv4 first, the
        /// timeout, the retries, the user agent and until when its token may be
        /// used are as they were, and so are when it was added and the versions
        /// it sees. The next start knows what its file reads back of them.
        /// </summary>
        [Test]
        public async Task ARegistrationKeepsHowTheOtherSideIsReached()
        {

            using var certificate  = AClientCertificate();

            var party       = await AddTheOtherSide(AccessToken.NewRandom(), certificate);

            var registered  = await AClient(party).TryRegister();
            var tokenB      = other.ReceivedCredentials?.Value<String>("token");

            Assert.That(registered.Response.Data?.Token.ToString(), Is.EqualTo(tokenC), $"The registration did not go through: {registered.Response.StatusMessage}");
            Assert.That(LocalTokensOf(api).Select(token => token.ToString()), Is.EqualTo(new[] { tokenB }), "The token the other side calls back with is not token B alone.");

            ReachedAsItWas(party, certificate);

        }

        #endregion

        #region TheTokenHandedOutIsSentAsTheNextStartSendsIt(Renewal, Base64Encoded)

        /// <summary>
        /// The token the other side hands out - at a registration, or at a
        /// renewal - is sent as its party says it is to be: raw where it says
        /// so, Base64 where it says nothing; as the client the next start makes
        /// sends it, so that the other side is not asked one way now and
        /// another after a restart.
        /// </summary>
        [TestCase(false, false)]
        [TestCase(false, null)]
        [TestCase(true,  false)]
        [TestCase(true,  null)]
        public async Task TheTokenHandedOutIsSentAsTheNextStartSendsIt(Boolean   Renewal,
                                                                       Boolean?  Base64Encoded)
        {

            await Listening();

            var client    = AClient(await AddTheOtherSide(AccessToken.NewRandom(), Base64Encoded));

            var answer    = Renewal
                                ? (await client.TryPutCredentials(OurCredentials(AccessToken.NewRandom()))).Response
                                : (await client.TryRegister()).Response;

            Assert.That(answer.Data?.Token.ToString(), Is.EqualTo(tokenC), $"The other side did not answer with its token: {answer.StatusMessage}");

            await client.GetVersions();

            var again     = ACommonAPI();

            await new CommonHTTPClient(
                      CommonAPI:    again,
                      RemoteParty:  again.RemoteParties.First(party => party.Id == id)
                  ).GetVersions();

            var expected  = Base64Encoded == false
                                ? tokenC
                                : Convert.ToBase64String(Encoding.UTF8.GetBytes(tokenC));

            Assert.That(other.VersionsAskedWith.TakeLast(2),
                        Is.EqualTo(new[] { expected, expected }),
                        "The token the other side handed out is not sent as its party says, now and after the next start.");

        }

        #endregion

        #region ARefusedRenewalLeavesTheRestOfThePartyAsItWas()

        /// <summary>
        /// A renewal the other side refused leaves the rest of the party as it
        /// was, though its new token was stored before it was sent: when it was
        /// added, and the versions it sees.
        /// </summary>
        [Test]
        public async Task ARefusedRenewalLeavesTheRestOfThePartyAsItWas()
        {

            await Listening();

            var party    = await AddTheOtherSide(AccessToken.NewRandom());

            other.RefusesRenewals = true;

            var renewed  = await AClient(party).TryPutCredentials(OurCredentials(AccessToken.NewRandom()));

            Assert.Multiple(() => {
                Assert.That(renewed.Response.StatusCode.Value,  Is.EqualTo(3000),                  $"The refusal of the other side is not handed back: {renewed.Response.StatusMessage}");
                Assert.That(CreatedOf(api),                     Is.EqualTo(party.Created),         "The other side is said to have been added when its renewal was sent.");
                Assert.That(VisibleVersionsOf(api),             Is.EqualTo(new[] { Version.Id }),  "The other side sees other versions than it saw.");
            });

        }

        #endregion

        #region ASecondRenewalTheOtherSideRefusedLeavesTheTokensOfTheFirstInEffect()

        /// <summary>
        /// A second renewal through the same client - made before the first,
        /// as a cached client is - which the other side refused after it called
        /// back: the tokens of the first renewal are still in effect, not those
        /// the client found when it was made.
        /// </summary>
        [Test]
        public async Task ASecondRenewalTheOtherSideRefusedLeavesTheTokensOfTheFirstInEffect()
        {

            var firstToken  = AccessToken.NewRandom();

            await Listening();

            var client      = AClient(await AddTheOtherSide(AccessToken.NewRandom()));

            var first       = await client.TryPutCredentials(OurCredentials(firstToken));

            Assert.That(first.Response.Data?.Token.ToString(), Is.EqualTo(tokenC), $"The first renewal did not go through: {first.Response.StatusMessage}");

            other.RefusesRenewals = true;

            var second      = await client.TryPutCredentials(OurCredentials(AccessToken.NewRandom()));

            Assert.Multiple(() => {
                Assert.That(second.Response.StatusCode.Value,  Is.EqualTo(3000),          $"The refusal of the other side is not handed back: {second.Response.StatusMessage}");
                Assert.That(LocalTokensOf(api),                Does.Contain(firstToken),  "The token the other side holds since the first renewal does not open our door any more.");
                Assert.That(RemoteTokenOf(api),                Is.EqualTo(tokenC),        "The token the other side handed out at the first renewal is not the one we call it with any more.");
            });

            var again = ACommonAPI();

            Assert.Multiple(() => {
                Assert.That(LocalTokensOf(again),  Does.Contain(firstToken),  "The next start does not know the token the other side holds since the first renewal.");
                Assert.That(RemoteTokenOf(again),  Is.EqualTo(tokenC),        "The next start does not know the token the other side handed out at the first renewal.");
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


        #region (private) Listening() / Listening(TokenOfOurs) / CredentialsURL(Token) / Send(Method, URL, Token)

        /// <summary>
        /// This test's Common API made anew, listening on a port nobody was
        /// listening on a moment ago.
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
        /// The same, with the other side to come: the given token of ours,
        /// handed out and not yet used.
        /// </summary>
        private async Task Listening(AccessToken TokenOfOurs)
        {

            await Listening();

            var added = await api.AddRemoteParty(id,
                                                [
                                                    new CredentialsRole(
                                                        CountryCode.Parse("DE"),
                                                        Party_Id.   Parse("BBB"),
                                                        Role.CPO,
                                                        new BusinessDetails("Their CPO")
                                                    )
                                                ],
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
            var details           = (versions?["data"] as JArray)?.First(version => version.Value<String>("version") == "2.2.1").Value<String>("url");

            Assert.That(details, Is.Not.Null, $"This API does not list 2.2.1: {versions}");

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

            request.Headers.TryAddWithoutValidation("Authorization", $"Token {Convert.ToBase64String(Encoding.UTF8.GetBytes(Token.ToString()))}");

            if (Method == HTTPMethod.POST)
                request.Content = new StringContent(
                                      new JObject(
                                          new JProperty("token",  tokenC),
                                          new JProperty("url",    other.VersionsURL.ToString()),
                                          new JProperty("roles",  new JArray(
                                              new JObject(
                                                  new JProperty("role",              "CPO"),
                                                  new JProperty("party_id",          "BBB"),
                                                  new JProperty("country_code",      "DE"),
                                                  new JProperty("business_details",  new JObject(
                                                      new JProperty("name",  "Their CPO")
                                                  ))
                                              )
                                          ))
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

        #region (private) AddTheOtherSide(OurToken[, ClientCertificate]) / AClientCertificate() / AClient(Party) / OurCredentials(Token)

        /// <summary>
        /// The other side as a remote party: the given token of ours, and its
        /// versions URL and token A to start the registration with - and, as
        /// one may be kept to it, this version the only one it sees. Its token
        /// encoded as given: Base64 where nothing is given.
        /// </summary>
        private async Task<RemoteParty> AddTheOtherSide(AccessToken  OurToken,
                                                        Boolean?     Base64Encoded   = null)
        {

            var added = await api.AddRemoteParty(id,
                                                [
                                                    new CredentialsRole(
                                                        CountryCode.Parse("DE"),
                                                        Party_Id.   Parse("BBB"),
                                                        Role.CPO,
                                                        new BusinessDetails("Their CPO")
                                                    )
                                                ],
                                                OurToken,
                                                other.VersionsURL,
                                                AccessToken.Parse(tokenA),
                                                RemoteAccessTokenBase64Encoding:  Base64Encoded,
                                                VisibleVersionIds:                [ Version.Id ]);

            Assert.That(added.IsSuccess, Is.True, $"The other side could not be added: {added.ErrorResponse}");

            return added.Data!;

        }

        /// <summary>
        /// The same, reached as a peer may ask to be: with the given client
        /// certificate, over the TLS versions it takes, IPv4 first, with a
        /// timeout, retries, a content type, the types it accepts and a user
        /// agent of its own, and its token not to be used after a given time.
        /// </summary>
        private async Task<RemoteParty> AddTheOtherSide(AccessToken       OurToken,
                                                        X509Certificate2  ClientCertificate)
        {

            var added = await api.AddRemoteParty(id,
                                                [
                                                    new CredentialsRole(
                                                        CountryCode.Parse("DE"),
                                                        Party_Id.   Parse("BBB"),
                                                        Role.CPO,
                                                        new BusinessDetails("Their CPO")
                                                    )
                                                ],
                                                OurToken,
                                                other.VersionsURL,
                                                AccessToken.Parse(tokenA),
                                                PreferIPv4:             IPVersionPreference.PreferIPv4,
                                                ClientCertificates:     [ ClientCertificate ],
                                                TLSProtocols:           tls,
                                                ContentType:            contentType,
                                                Accept:                 accept,
                                                HTTPUserAgent:          userAgent,
                                                RequestTimeout:         timeout,
                                                MaxNumberOfRetries:     retries,
                                                RemoteAccessNotAfter:   notAfter,
                                                VisibleVersionIds:      [ Version.Id ]);

            Assert.That(added.IsSuccess, Is.True, $"The other side could not be added: {added.ErrorResponse}");

            return added.Data!;

        }

        /// <summary>
        /// A client certificate of ours, made for the test, with its private
        /// key, as the file of the remote parties keeps one.
        /// </summary>
        private static X509Certificate2 AClientCertificate()
        {

            using var key = ECDsa.Create(ECCurve.NamedCurves.nistP256);

            return new CertificateRequest("CN=Our OCPI client", key, HashAlgorithmName.SHA256).
                       CreateSelfSigned(DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow.AddYears(1));

        }

        /// <summary>
        /// A client to the other side, for the given remote party.
        /// </summary>
        private CommonHTTPClient AClient(RemoteParty Party)

            => new (
                   CommonAPI:    api,
                   RemoteParty:  Party
               );

        /// <summary>
        /// Our credentials, as a renewal puts them onto the other side: the
        /// given new token, and where our versions are.
        /// </summary>
        private Credentials OurCredentials(AccessToken Token)

            => new (
                   Token,
                   api.BaseAPI.OurVersionsURL,
                   [
                       new CredentialsRole(
                           CountryCode.Parse("DE"),
                           Party_Id.   Parse("GEF"),
                           Role.CPO,
                           new BusinessDetails("GraphDefined CSO")
                       )
                   ]
               );

        #endregion

        #region (private) ReachedAsItWas(Party, ClientCertificate)

        /// <summary>
        /// Everything but the tokens of how this API reaches the other side is
        /// as it was when the given party was added with the given client
        /// certificate, and the other side has handed out token C - now, and at
        /// the next start for what its file reads back.
        /// </summary>
        private void ReachedAsItWas(RemoteParty       Party,
                                    X509Certificate2  ClientCertificate)
        {

            var now = RemoteAccessOf(api);

            Assert.Multiple(() => {
                Assert.That(now?.AccessToken?.ToString(),                          Is.EqualTo(tokenC),                               "The token the other side handed out is not the one in effect.");
                Assert.That(now?.ClientCertificates.Select(c => c.Thumbprint),     Is.EqualTo(new[] { ClientCertificate.Thumbprint }), "The client certificate the other side asks for is gone.");
                Assert.That(now?.TLSProtocols,                                     Is.EqualTo(tls),                                  "The TLS versions are not the ones they were.");
                Assert.That(now?.PreferIPv4,                                       Is.EqualTo(IPVersionPreference.PreferIPv4),       "IPv4 is not preferred any more.");
                Assert.That(now?.RequestTimeout,                                   Is.EqualTo(timeout),                              "The timeout is not the one it was.");
                Assert.That(now?.MaxNumberOfRetries,                               Is.EqualTo(retries),                              "The retries are not the ones they were.");
                Assert.That(now?.HTTPUserAgent,                                    Is.EqualTo(userAgent),                            "The user agent is not the one it was.");
                Assert.That(now?.ContentType?.ToString(),                          Is.EqualTo(contentType.ToString()),               "The content type is not the one it was.");
                Assert.That(now?.Accept?.ToString(),                               Is.EqualTo(accept.ToString()),                    "The types it accepts are not the ones they were.");
                Assert.That(now?.VersionIds,                                       Is.EqualTo(new[] { Version.Id }),                 "The versions of the other side are not the one it speaks.");
                Assert.That(now?.NotAfter,                                         Is.EqualTo(notAfter),                             "The token of the other side may be used for another time than it might.");
                Assert.That(CreatedOf(api),                                        Is.EqualTo(Party.Created),                        "The other side is said to have been added anew.");
                Assert.That(VisibleVersionsOf(api),                                Is.EqualTo(new[] { Version.Id }),                 "The other side sees other versions than it saw.");
            });

            var again = RemoteAccessOf(ACommonAPI());

            Assert.Multiple(() => {
                Assert.That(again?.AccessToken?.ToString(),                        Is.EqualTo(tokenC),                               "The next start does not know the token the other side handed out.");
                Assert.That(again?.ClientCertificates.Select(c => c.Thumbprint),   Is.EqualTo(new[] { ClientCertificate.Thumbprint }), "The next start does not know the client certificate the other side asks for.");
                Assert.That(again?.TLSProtocols,                                   Is.EqualTo(tls),                                  "The next start does not know the TLS versions.");
                Assert.That(again?.PreferIPv4,                                     Is.EqualTo(IPVersionPreference.PreferIPv4),       "The next start does not prefer IPv4.");
                Assert.That(again?.RequestTimeout,                                 Is.EqualTo(timeout),                              "The next start does not know the timeout.");
                Assert.That(again?.MaxNumberOfRetries,                             Is.EqualTo(retries),                              "The next start does not know the retries.");
                Assert.That(again?.HTTPUserAgent,                                  Is.EqualTo(userAgent),                            "The next start does not know the user agent.");
                Assert.That(again?.ContentType?.ToString(),                        Is.EqualTo(contentType.ToString()),               "The next start does not know the content type.");
                Assert.That(again?.Accept?.ToString(),                             Is.EqualTo(accept.ToString()),                    "The next start does not know the types it accepts.");
                Assert.That(again?.VersionIds,                                     Is.EqualTo(new[] { Version.Id }),                 "The next start does not know the versions of the other side.");
                Assert.That(again?.NotAfter,                                       Is.EqualTo(notAfter),                             "The next start does not know until when the token of the other side may be used.");
            });

        }

        #endregion

        #region (private) FileName / LocalTokensOf(API) / RemoteTokenOf(API) / CreatedOf(API) / VisibleVersionsOf(API) / RemoteAccessOf(API)

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

        /// <summary>
        /// When the other side was added, as this API has it, or null.
        /// </summary>
        private static DateTimeOffset? CreatedOf(CommonAPI API)

            => API.RemoteParties.
                   Where     (party => party.Id == id).
                   Select    (party => (DateTimeOffset?) party.Created).
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
        /// How this API reaches the other side, or null.
        /// </summary>
        private static RemoteAccessInfo? RemoteAccessOf(CommonAPI API)

            => API.RemoteParties.
                   Where     (party => party.Id == id).
                   SelectMany(party => party.RemoteAccessInfos).
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
        /// port. A token it does not know is refused, as a hub refuses it by
        /// default - not let through with the locations as open data.
        /// </summary>
        private CommonAPI ACommonAPI(HTTPServer  Server,
                                     UInt16      Port)

            => new (

                   OurPartyData:      [
                                          new PartyData(
                                              Party_Idv3.From(CountryCode.Parse("DE"), Party_Id.Parse("GEF")),
                                              Role.CPO,
                                              new BusinessDetails("GraphDefined CSO")
                                          )
                                      ],
                   DefaultPartyId:    Party_Idv3.From(CountryCode.Parse("DE"), Party_Id.Parse("GEF")),

                   BaseAPI:           new CommonHTTPAPI(
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

                   URLPathPrefix:     HTTPPath.Parse("/ocpi/v2.2.1"),
                   DatabaseFilePath:  directory,
                   DisableLogging:    true,
                   LoggingPath:       directory

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
        /// 2.2.1 and the credentials, on a port nobody was listening on a
        /// moment ago. It records the credentials it was sent, and answers
        /// with its own - a renewal only once it has called back, as this
        /// library's own receiver does: the versions of the sender, fetched
        /// with the token it sent.
        /// </summary>
        private sealed class OtherSide : IAsyncDisposable
        {

            private readonly HTTPServer server;

            /// <summary>Where it says its versions are.</summary>
            public URL                                 VersionsURL            { get; }

            /// <summary>The credentials it was sent, if any.</summary>
            public JObject?                            ReceivedCredentials    { get; private set; }

            /// <summary>Something to do once credentials have arrived, before the answer goes back.</summary>
            public Action?                             WhenCredentialsArrive  { get; set; }

            /// <summary>What the versions of the sender of a renewal answered the token it sent: the HTTP status and the OCPI status code.</summary>
            public (Int32 Status, Int32? StatusCode)?  CalledBack             { get; private set; }

            /// <summary>Whether a renewal is refused once it has called back - OCPI 3000 with HTTP 500, as this library's own receiver refuses one its file cannot take: nothing changed there.</summary>
            public Boolean                             RefusesRenewals        { get; set; }

            /// <summary>Whether the first renewal is answered as a gateway answers one that took too long: HTTP 504, a reason to send it once more.</summary>
            public Boolean                             TimesOutOnce           { get; set; }

            /// <summary>How many renewals arrived.</summary>
            public Int32                               Renewals               { get; private set; }

            /// <summary>The tokens its versions were asked with, as they arrived.</summary>
            public List<String?>                       VersionsAskedWith      { get; } = [];

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

                        other.VersionsAskedWith.Add((request.Authorization as HTTPTokenAuthentication)?.Token);

                        return Task.FromResult(JSON(request, new JArray(
                                   new JObject(
                                       new JProperty("version",  "2.2.1"),
                                       new JProperty("url",      $"{origin}/versions/2.2.1")
                                   )
                               )));

                    },
                    HTTPMethod.GET
                );

                api.AddHandler(
                    HTTPPath.Parse("/versions/2.2.1"),
                    request => Task.FromResult(JSON(request, new JObject(
                                   new JProperty("version",    "2.2.1"),
                                   new JProperty("endpoints",  new JArray(
                                       new JObject(
                                           new JProperty("identifier",  "credentials"),
                                           new JProperty("role",        "RECEIVER"),
                                           new JProperty("url",         $"{origin}/2.2.1/credentials")
                                       )
                                   ))
                               ))),
                    HTTPMethod.GET
                );

                api.AddHandler(
                    HTTPPath.Parse("/2.2.1/credentials"),
                    request => {

                        other.ReceivedCredentials = JObject.Parse(request.HTTPBodyAsUTF8String ?? "{}");
                        other.WhenCredentialsArrive?.Invoke();

                        return Task.FromResult(JSON(request, TheirCredentials(origin)));

                    },
                    HTTPMethod.POST
                );

                api.AddHandler(
                    HTTPPath.Parse("/2.2.1/credentials"),
                    async request => {

                        other.ReceivedCredentials = JObject.Parse(request.HTTPBodyAsUTF8String ?? "{}");

                        if (++other.Renewals == 1 && other.TimesOutOnce)
                            return Refused(request, HTTPStatusCode.GatewayTimeout,       3000, "The gateway timed out.");

                        other.WhenCredentialsArrive?.Invoke();

                        other.CalledBack = await CallBack(other.ReceivedCredentials);

                        if (other.CalledBack.Value.StatusCode != 1000)
                            return Refused(request, HTTPStatusCode.MethodNotAllowed,     2000, "Could not fetch VERSIONS information!");

                        if (other.RefusesRenewals)
                            return Refused(request, HTTPStatusCode.InternalServerError,  3000, "The credentials could not be stored here, and nothing was changed.");

                        return JSON(request, TheirCredentials(origin));

                    },
                    HTTPMethod.PUT
                );

                return other;

            }

            /// <summary>
            /// The credentials it answers with: token C, and where its versions
            /// are.
            /// </summary>
            private static JObject TheirCredentials(String Origin)

                => new (
                       new JProperty("token",  tokenC),
                       new JProperty("url",    $"{Origin}/versions"),
                       new JProperty("roles",  new JArray(
                           new JObject(
                               new JProperty("role",              "CPO"),
                               new JProperty("party_id",          "BBB"),
                               new JProperty("country_code",      "DE"),
                               new JProperty("business_details",  new JObject(
                                   new JProperty("name",  "Their CPO")
                               ))
                           )
                       ))
                   );

            /// <summary>
            /// The versions of the sender of the given credentials, fetched with
            /// the token they carry - and what they answered: the HTTP status and
            /// the OCPI status code, none where nobody answered.
            /// </summary>
            private static async Task<(Int32 Status, Int32? StatusCode)> CallBack(JObject Credentials)
            {

                try
                {

                    using var http     = new HttpClient();
                    using var request  = new HttpRequestMessage(HttpMethod.Get, Credentials.Value<String>("url"));

                    request.Headers.TryAddWithoutValidation("Authorization", $"Token {Convert.ToBase64String(Encoding.UTF8.GetBytes(Credentials.Value<String>("token") ?? ""))}");

                    using var response = await http.SendAsync(request);

                    var text   = await response.Content.ReadAsStringAsync();
                    var answer = text.StartsWith('{') ? JObject.Parse(text) : null;

                    return ((Int32) response.StatusCode, answer?.Value<Int32?>("status_code"));

                }
                catch (HttpRequestException)
                {
                    return (0, null);
                }

            }

            private static HTTPResponse Refused(HTTPRequest     Request,
                                                HTTPStatusCode  Status,
                                                Int32           StatusCode,
                                                String          Message)

                => new HTTPResponse.Builder(Request) {
                       HTTPStatusCode  = Status,
                       ContentType     = HTTPContentType.Application.JSON_UTF8,
                       Content         = Encoding.UTF8.GetBytes(
                                             new JObject(
                                                 new JProperty("status_code",     StatusCode),
                                                 new JProperty("status_message",  Message),
                                                 new JProperty("timestamp",       DateTimeOffset.UtcNow.ToString("o"))
                                             ).ToString()
                                         ),
                       Connection      = ConnectionType.Close
                   }.AsImmutable;

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
