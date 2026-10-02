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

using org.GraphDefined.Vanaheimr.Aegir;
using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv3_0.UnitTests.CommonTests
{

    /// <summary>
    /// A session and a charge detail record are there at the next start as
    /// they were added.
    /// </summary>
    /// <remarks>
    /// <para>
    /// No session came back from the file of the assets: Session.TryParse
    /// gave up on a session without a connector, which is optional, and said
    /// nothing. And no CDR could be made at all: CDR.ToJSON, which its
    /// constructor calls, wrote its total energy as a WattHour, which JSON
    /// does not know, and threw.
    /// </para>
    /// <para>
    /// The next start is a Common API made anew on the same directory, which
    /// reads the files back as a node's start does - see PartyFileTests.
    /// Nothing here listens: the HTTP servers are made and never started.
    /// </para>
    /// </remarks>
    [TestFixture]
    public class SessionCDRFileTests
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

            // Sessions and CDRs are filed under their party, and in 3.0
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


        #region ASessionAddedIsThereAtTheNextStart()

        [Test]
        public async Task ASessionAddedIsThereAtTheNextStart()
        {

            var session  = ASession("SESSION0001");
            var added    = await api.AddSession(session);

            Assert.That(added.IsSuccess, Is.True, $"The session could not be added: {added.ErrorResponse}");

            Assert.That((await NextStart()).GetSessions().Select(AsJSON), Is.EquivalentTo(new[] { AsJSON(session) }),
                        "The next start does not know the session added.");

        }

        #endregion

        #region ACDRAddedIsThereAtTheNextStart()

        [Test]
        public async Task ACDRAddedIsThereAtTheNextStart()
        {

            var cdr    = ACDR("CDR0001");
            var added  = await api.AddCDR(cdr);

            Assert.That(added.IsSuccess, Is.True, $"The CDR could not be added: {added.ErrorResponse}");

            Assert.That((await NextStart()).GetCDRs().Select(AsJSON), Is.EquivalentTo(new[] { AsJSON(cdr) }),
                        "The next start does not know the CDR added.");

        }

        #endregion


        #region (private static) ASession / ACDR / ACDRToken

        /// <summary>
        /// A session filed under our party. Its energy has no zero at its end,
        /// which an energy read back does not keep: JSON compares it as text.
        /// </summary>
        private static Session ASession(String Id)

            => new (
                   PartyId:              ourPartyId,
                   Id:                   Session_Id.Parse(Id),
                   VersionId:            1,
                   Start:                start,
                   Energy:               WattHour.ParseWh("1115.5"),
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

        #region (private static) AsJSON(...)

        /// <summary>
        /// A session or a CDR as its JSON, compared with all it says.
        /// </summary>
        private static String AsJSON(Session  Session)  => Session.ToJSON(true, true, true, true).ToString(Newtonsoft.Json.Formatting.None);
        private static String AsJSON(CDR      CDR)      => CDR.    ToJSON(true, true, true, true).ToString(Newtonsoft.Json.Formatting.None);

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
