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

using NUnit.Framework;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Aegir;
using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv3_0.UnitTests.CommonTests
{

    /// <summary>
    /// The tokens, sessions and charge detail records removed - one, or
    /// several at once - are still removed at the next start, and only they
    /// are.
    /// </summary>
    /// <remarks>
    /// <para>
    /// RemoveAllTokens, RemoveAllSessions and RemoveAllCDRs write down what
    /// they removed - with a filter or for one party, only some - and the next
    /// start removed every one of its kind instead. RemoveAllTokens with a
    /// filter, or with none, wrote its line as "removeAllSessions": the next
    /// start removed every session, and the tokens removed were there again.
    /// RemoveToken writes the token removed, and the next start read it as a
    /// token status, which it is not, and passed over it.
    /// </para>
    /// <para>
    /// The next start is a Common API made anew on the same directory, which
    /// reads the files back as a node's start does - see PartyFileTests.
    /// Nothing here listens: the HTTP servers are made and never started.
    /// </para>
    /// </remarks>
    [TestFixture]
    public class RemovedAtOnceTests
    {

        #region Data

        private static readonly Party_Idv3      ourPartyId  = Party_Idv3.From(CountryCode.Parse("DE"), Party_Id.Parse("GEF"));

        /// <summary>
        /// When things began and were last updated: a time of their own, so
        /// that none of them reads the clock.
        /// </summary>
        private static readonly DateTimeOffset  start       = new (2026, 10, 1, 8, 0, 0, TimeSpan.Zero);

        private String     directory  = default!;
        private CommonAPI  api        = default!;

        #endregion

        #region SetUp / TearDown

        [SetUp]
        public async Task SetUp()
        {

            directory  = Path.Combine(Path.GetTempPath(), $"WWCP_OCPI_Tests-{Guid.NewGuid():N}");

            Directory.CreateDirectory(directory);

            api        = ACommonAPI();

            // Tokens, sessions and CDRs are filed under their party, and in 3.0
            // no party is given at the start: every one is added, and is there
            // at the next start since 2bf8aa31.
            var party  = await api.AddParty(ourPartyId, Role.EMSP, new BusinessDetails("GraphDefined EMSP"));

            Assert.That(party.IsSuccess, Is.True, $"Our party could not be added: {party.ErrorResponse}");

        }

        [TearDown]
        public async Task TearDown()
        {

            // Its queue written out, before its directory goes.
            await api.BaseAPI.DisposeAsync();

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


        #region ATokenRemovedIsGoneAtTheNextStart()

        [Test]
        public async Task ATokenRemovedIsGoneAtTheNextStart()
        {

            var added1   = await api.AddToken(AToken("TOKEN0001"));
            var added2   = await api.AddToken(AToken("TOKEN0002"));
            var removal  = await api.RemoveToken(ourPartyId, Token_Id.Parse("TOKEN0001"));

            Assert.Multiple(() => {
                Assert.That(added1. IsSuccess, Is.True, $"The token to remove could not be added: {added1.ErrorResponse}");
                Assert.That(added2. IsSuccess, Is.True, $"The token to keep could not be added: {added2.ErrorResponse}");
                Assert.That(removal.IsSuccess, Is.True, $"The token could not be removed: {removal.ErrorResponse}");
            });

            Assert.That(IdsOf((await NextStart()).GetTokens()), Is.EquivalentTo(new[] { "TOKEN0002" }),
                        "The next start knows the token removed, or not the one kept.");

        }

        #endregion

        #region EveryTokenRemovedAtOnceIsGoneAndTheSessionsAreThere()

        /// <summary>
        /// Every token removed at once is gone at the next start, and the
        /// sessions are all there.
        /// </summary>
        [Test]
        public async Task EveryTokenRemovedAtOnceIsGoneAndTheSessionsAreThere()
        {

            var added1   = await api.AddToken  (AToken  ("TOKEN0001"));
            var added2   = await api.AddToken  (AToken  ("TOKEN0002"));
            var added3   = await api.AddSession(ASession("SESSION0001"));
            var removal  = await api.RemoveAllTokens();

            Assert.Multiple(() => {
                Assert.That(added1. IsSuccess, Is.True, $"The first token could not be added: {added1.ErrorResponse}");
                Assert.That(added2. IsSuccess, Is.True, $"The second token could not be added: {added2.ErrorResponse}");
                Assert.That(added3. IsSuccess, Is.True, $"The session could not be added: {added3.ErrorResponse}");
                Assert.That(removal.IsSuccess, Is.True, $"The tokens could not be removed: {removal.ErrorResponse}");
            });

            var next     = await NextStart();

            Assert.Multiple(() => {

                Assert.That(IdsOf(next.GetTokens()),   Is.Empty,                                   "The next start knows a token removed at once.");
                Assert.That(IdsOf(next.GetSessions()), Is.EquivalentTo(new[] { "SESSION0001" }), "The next start does not know the session.");

                Assert.That(CommandsIn(next.AssetsDBFileName), Does.Contain(CommonHTTPAPI.removeAllTokens).And.Not.Contain(CommonHTTPAPI.removeAllSessions),
                            "The tokens removed at once are not written down as tokens removed.");

            });

        }

        #endregion

        #region EveryTokenMatchedIsGoneAtTheNextStart()

        /// <summary>
        /// Every token a filter matched is gone at the next start, and the one
        /// it did not match is there.
        /// </summary>
        [Test]
        public async Task EveryTokenMatchedIsGoneAtTheNextStart()
        {

            var added1   = await api.AddToken(AToken("TOKEN0001"));
            var added2   = await api.AddToken(AToken("TOKEN0002"));
            var added3   = await api.AddToken(AToken("TOKEN0003"));
            var removal  = await api.RemoveAllTokens((Token token) => token.Id.ToString() != "TOKEN0003");

            Assert.Multiple(() => {
                Assert.That(added1. IsSuccess, Is.True, $"The first token could not be added: {added1.ErrorResponse}");
                Assert.That(added2. IsSuccess, Is.True, $"The second token could not be added: {added2.ErrorResponse}");
                Assert.That(added3. IsSuccess, Is.True, $"The third token could not be added: {added3.ErrorResponse}");
                Assert.That(removal.IsSuccess, Is.True, $"The tokens matched could not be removed: {removal.ErrorResponse}");
            });

            Assert.That(IdsOf((await NextStart()).GetTokens()), Is.EquivalentTo(new[] { "TOKEN0003" }),
                        "The next start knows a token matched and removed, or not the one kept.");

        }

        #endregion

        #region TokensRemovedAsBeforeAreGoneAndTheSessionsAreThere()

        /// <summary>
        /// Tokens removed at once, their line written as RemoveAllTokens wrote
        /// it before - as "removeAllSessions" - are gone at the next start as
        /// well, and the sessions are all there.
        /// </summary>
        [Test]
        public async Task TokensRemovedAsBeforeAreGoneAndTheSessionsAreThere()
        {

            var removed  = AToken("TOKEN0001");

            var added1   = await api.AddToken  (removed);
            var added2   = await api.AddToken  (AToken  ("TOKEN0002"));
            var added3   = await api.AddSession(ASession("SESSION0001"));

            Assert.Multiple(() => {
                Assert.That(added1.IsSuccess, Is.True, $"The token to remove could not be added: {added1.ErrorResponse}");
                Assert.That(added2.IsSuccess, Is.True, $"The token to keep could not be added: {added2.ErrorResponse}");
                Assert.That(added3.IsSuccess, Is.True, $"The session could not be added: {added3.ErrorResponse}");
            });

            await api.LogAsset(CommonHTTPAPI.removeAllSessions, new JArray(removed.ToJSON(true, true, true, true)), EventTracking_Id.New);

            var next     = await NextStart();

            Assert.Multiple(() => {
                Assert.That(IdsOf(next.GetTokens()),   Is.EquivalentTo(new[] { "TOKEN0002" }),   "The next start knows the token removed before, or not the one kept.");
                Assert.That(IdsOf(next.GetSessions()), Is.EquivalentTo(new[] { "SESSION0001" }), "The next start does not know the session.");
            });

        }

        #endregion

        #region EverySessionMatchedIsGoneAtTheNextStart()

        /// <summary>
        /// Every session a filter matched is gone at the next start, and the
        /// one it did not match is there.
        /// </summary>
        [Test]
        public async Task EverySessionMatchedIsGoneAtTheNextStart()
        {

            var added1   = await api.AddSession(ASession("SESSION0001"));
            var added2   = await api.AddSession(ASession("SESSION0002"));
            var added3   = await api.AddSession(ASession("SESSION0003"));
            var removal  = await api.RemoveAllSessions((Session session) => session.Id.ToString() != "SESSION0003");

            Assert.Multiple(() => {
                Assert.That(added1. IsSuccess, Is.True, $"The first session could not be added: {added1.ErrorResponse}");
                Assert.That(added2. IsSuccess, Is.True, $"The second session could not be added: {added2.ErrorResponse}");
                Assert.That(added3. IsSuccess, Is.True, $"The third session could not be added: {added3.ErrorResponse}");
                Assert.That(removal.IsSuccess, Is.True, $"The sessions matched could not be removed: {removal.ErrorResponse}");
            });

            Assert.That(IdsOf((await NextStart()).GetSessions()), Is.EquivalentTo(new[] { "SESSION0003" }),
                        "The next start knows a session matched and removed, or not the one kept.");

        }

        #endregion

        #region EverySessionRemovedAtOnceIsGoneAtTheNextStart()

        [Test]
        public async Task EverySessionRemovedAtOnceIsGoneAtTheNextStart()
        {

            var added1   = await api.AddSession(ASession("SESSION0001"));
            var added2   = await api.AddSession(ASession("SESSION0002"));
            var removal  = await api.RemoveAllSessions();

            Assert.Multiple(() => {
                Assert.That(added1. IsSuccess, Is.True, $"The first session could not be added: {added1.ErrorResponse}");
                Assert.That(added2. IsSuccess, Is.True, $"The second session could not be added: {added2.ErrorResponse}");
                Assert.That(removal.IsSuccess, Is.True, $"The sessions could not be removed: {removal.ErrorResponse}");
            });

            Assert.That(IdsOf((await NextStart()).GetSessions()), Is.Empty,
                        "The next start knows a session removed at once.");

        }

        #endregion

        #region EveryCDRMatchedIsGoneAtTheNextStart()

        /// <summary>
        /// Every CDR a filter matched is gone at the next start, and the one it
        /// did not match is there.
        /// </summary>
        [Test]
        public async Task EveryCDRMatchedIsGoneAtTheNextStart()
        {

            var added1   = await api.AddCDR(ACDR("CDR0001"));
            var added2   = await api.AddCDR(ACDR("CDR0002"));
            var added3   = await api.AddCDR(ACDR("CDR0003"));
            var removal  = await api.RemoveAllCDRs((CDR cdr) => cdr.Id.ToString() != "CDR0003");

            Assert.Multiple(() => {
                Assert.That(added1. IsSuccess, Is.True, $"The first CDR could not be added: {added1.ErrorResponse}");
                Assert.That(added2. IsSuccess, Is.True, $"The second CDR could not be added: {added2.ErrorResponse}");
                Assert.That(added3. IsSuccess, Is.True, $"The third CDR could not be added: {added3.ErrorResponse}");
                Assert.That(removal.IsSuccess, Is.True, $"The CDRs matched could not be removed: {removal.ErrorResponse}");
            });

            Assert.That(IdsOf((await NextStart()).GetCDRs()), Is.EquivalentTo(new[] { "CDR0003" }),
                        "The next start knows a CDR matched and removed, or not the one kept.");

        }

        #endregion

        #region EveryCDRRemovedAtOnceIsGoneAtTheNextStart()

        [Test]
        public async Task EveryCDRRemovedAtOnceIsGoneAtTheNextStart()
        {

            var added1   = await api.AddCDR(ACDR("CDR0001"));
            var added2   = await api.AddCDR(ACDR("CDR0002"));
            var removal  = await api.RemoveAllCDRs();

            Assert.Multiple(() => {
                Assert.That(added1. IsSuccess, Is.True, $"The first CDR could not be added: {added1.ErrorResponse}");
                Assert.That(added2. IsSuccess, Is.True, $"The second CDR could not be added: {added2.ErrorResponse}");
                Assert.That(removal.IsSuccess, Is.True, $"The CDRs could not be removed: {removal.ErrorResponse}");
            });

            Assert.That(IdsOf((await NextStart()).GetCDRs()), Is.Empty,
                        "The next start knows a CDR removed at once.");

        }

        #endregion


        #region (private static) AToken / ASession / ACDR / ACDRToken

        /// <summary>
        /// A token of our party.
        /// </summary>
        private static Token AToken(String Id)

            => new (
                   PartyId:      ourPartyId,
                   Id:           Token_Id.   Parse(Id),
                   VersionId:    1,
                   Type:         TokenType.RFID,
                   ContractId:   Contract_Id.Parse("DE-GEF-C12345678-X"),
                   Issuer:       "GraphDefined CA",
                   ValidFrom:    start,
                   Whitelist:    WhitelistType.NEVER,
                   LastUpdated:  start
               );

        /// <summary>
        /// A session filed under our party.
        /// </summary>
        private static Session ASession(String Id)

            => new (
                   PartyId:              ourPartyId,
                   Id:                   Session_Id.Parse(Id),
                   VersionId:            1,
                   Start:                start,
                   Energy:               WattHour.FromKWh(1.11M),
                   CDRToken:             ACDRToken(),
                   AuthMethod:           AuthMethod.AUTH_REQUEST,
                   LocationId:           Location_Id.Parse("LOCATION0001"),
                   Currency:             Currency.EUR,
                   TariffAssociationId:  TariffAssociation_Id.Parse("TA0001"),
                   TariffId:             Tariff_Id.Parse("TARIFF0001"),
                   Status:               SessionStatus.ACTIVE,
                   LastUpdated:          start
               );

        /// <summary>
        /// A charge detail record filed under our party.
        /// </summary>
        private static CDR ACDR(String Id)

            => new (
                   PartyId:              ourPartyId,
                   Id:                   CDR_Id.Parse(Id),
                   VersionId:            1,
                   Start:                start,
                   End:                  start + TimeSpan.FromMinutes(30),
                   CDRToken:             ACDRToken(),
                   AuthMethod:           AuthMethod.AUTH_REQUEST,
                   CDRLocation:          new CDRLocation(
                                             Location_Id.     Parse("LOCATION0001"),
                                             "Biberweg 18",
                                             "Jena",
                                             Country.Germany,
                                             GeoCoordinate.   Parse(50.9, 11.6),
                                             EVSE_UId.        Parse("EVSE0001"),
                                             EVSE_Id.         Parse("DE*GEF*E0001*1"),
                                             Connector_Id.    Parse("1"),
                                             ConnectorType.   IEC_62196_T2,
                                             ConnectorFormats.SOCKET,
                                             PowerTypes.      AC_3_PHASE
                                         ),
                   Currency:             Currency.EUR,
                   TariffAssociationId:  TariffAssociation_Id.Parse("TA0001"),
                   TariffId:             Tariff_Id.Parse("TARIFF0001"),
                   ChargingPeriods:      [
                                             ChargingPeriod.Create(
                                                 start,
                                                 [ CDRDimension.Create(CDRDimensionType.ENERGY, 1.33M) ]
                                             )
                                         ],
                   TotalCosts:           new Price(10.5M, 11.6M),
                   TotalEnergy:          WattHour.FromKWh(1.33M),
                   TotalTime:            TimeSpan.FromMinutes(30),
                   LastUpdated:          start
               );

        /// <summary>
        /// The token a session or a CDR was charged with.
        /// </summary>
        private static CDRToken ACDRToken()

            => new (
                   ourPartyId.CountryCode,
                   ourPartyId,
                   Token_Id.   Parse("TOKEN0001"),
                   TokenType.  RFID,
                   Contract_Id.Parse("DE-GEF-C12345678-X")
               );

        #endregion

        #region (private static) IdsOf(...)

        /// <summary>
        /// The identifications of the given tokens, sessions or CDRs, as text.
        /// </summary>
        private static IEnumerable<String> IdsOf(IEnumerable<Token>    Tokens)    => Tokens.  Select(token   => token.  Id.ToString()).ToArray();
        private static IEnumerable<String> IdsOf(IEnumerable<Session>  Sessions)  => Sessions.Select(session => session.Id.ToString()).ToArray();
        private static IEnumerable<String> IdsOf(IEnumerable<CDR>      CDRs)      => CDRs.    Select(cdr     => cdr.    Id.ToString()).ToArray();

        #endregion

        #region (private static) CommandsIn(FileName)

        /// <summary>
        /// The commands of the lines in a file, in the order they are in it.
        /// </summary>
        private static IEnumerable<String> CommandsIn(String FileName)

            => File.ReadLines(FileName).
                    Where (line => line.StartsWith('{')).
                    Select(line => JObject.Parse(line).Properties().First().Name).
                    ToArray();

        #endregion

        #region (private) NextStart()

        /// <summary>
        /// The next start of a node: the Common API so far disposed - its assets
        /// go through a queue, and are all in their file once it is - and one
        /// made anew on the same directory, which reads back what it wrote.
        /// </summary>
        private async Task<CommonAPI> NextStart()
        {

            await api.BaseAPI.DisposeAsync();

            api = ACommonAPI();

            return api;

        }

        #endregion

        #region (private) ACommonAPI()

        /// <summary>
        /// A Common API on this test's directory. Made anew, it reads back what
        /// the one before it wrote, as the next start of a node does.
        /// </summary>
        private CommonAPI ACommonAPI()

            => new (

                   OurCredentialRoles:  [
                                            new CredentialsRole(
                                                ourPartyId,
                                                Role.EMSP,
                                                new BusinessDetails("GraphDefined EMSP")
                                            )
                                        ],
                   DefaultPartyId:      ourPartyId,

                   BaseAPI:             new CommonHTTPAPI(
                                          HTTPAPI:          new HTTPExtAPI(
                                                                HTTPServer: new HTTPServer(TCPPort: IPPort.Parse(3999))
                                                            ),
                                          OurBaseURL:       URL.Parse("http://127.0.0.1:3999/ocpi"),
                                          OurVersionsURL:   URL.Parse("http://127.0.0.1:3999/ocpi/versions"),
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

    }

}
