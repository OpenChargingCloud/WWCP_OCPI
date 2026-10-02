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

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv3_0.UnitTests.CommonTests
{

    /// <summary>
    /// A line of a database file that is not read at the start is told to the
    /// Common HTTP API's OnDatabaseLineNotRead, with its file and why; a line
    /// that is read, or one of a command the reader does not know, is not.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A line that was no command was told to the debug log alone, and one
    /// whose command could not be made - a remote party, an asset - was put
    /// into a list that was thrown away: the remote party or the asset was
    /// gone after the start, and nobody heard of it.
    /// </para>
    /// <para>
    /// The next start is a Common API made anew on the same directory, which
    /// reads the files back as a node's start does - see PartyFileTests. Its
    /// Common HTTP API is made first, and subscribed to before. Nothing here
    /// listens: the HTTP servers are made and never started.
    /// </para>
    /// </remarks>
    [TestFixture]
    public class LinesNotReadTests
    {

        #region Data

        private static readonly Party_Idv3      ourPartyId   = Party_Idv3.From(CountryCode.Parse("DE"), Party_Id.Parse("GEF"));

        /// <summary>
        /// When the tariffs were last updated: a time of their own, so that
        /// none of them reads the clock.
        /// </summary>
        private static readonly DateTimeOffset  start        = new (2026, 10, 1, 8, 0, 0, TimeSpan.Zero);

        private String     directory  = default!;
        private CommonAPI  api        = default!;

        /// <summary>
        /// What OnDatabaseLineNotRead was told: the file, the line and why.
        /// </summary>
        private readonly List<(String FileName, String Line, String Reason)> told = [];

        #endregion

        #region SetUp / TearDown

        [SetUp]
        public async Task SetUp()
        {

            directory  = Path.Combine(Path.GetTempPath(), $"WWCP_OCPI_Tests-{Guid.NewGuid():N}");

            Directory.CreateDirectory(directory);

            told.Clear();

            api        = ACommonAPI();

            // A tariff is filed under its party, and in 3.0 no party is given
            // at the start: every one is added, and is there at the next
            // start since 2bf8aa31.
            var party  = await api.AddParty(ourPartyId, Role.CPO, new BusinessDetails("GraphDefined CSO"));

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


        #region ALineThatIsNoCommandIsTold()

        [Test]
        public async Task ALineThatIsNoCommandIsTold()
        {

            var file = api.AssetsDBFileName;

            await NextStartWith(file, "this is no JSON");

            Assert.That(told.Select(AsText), Is.EqualTo(new[] { $"{file} | this is no JSON" }),
                        "A line that is no command is not told, or not so.");

            Assert.That(told[0].Reason, Is.Not.Empty, "Why the line is not read is not told.");

        }

        #endregion

        #region AnAssetLineThatCannotBeReadIsTold()

        [Test]
        public async Task AnAssetLineThatCannotBeReadIsTold()
        {

            var file = api.AssetsDBFileName;
            var line = ALine(CommonHTTPAPI.addTariff, new JObject(new JProperty("id", "TARIFF0001")));

            await NextStartWith(file, line);

            Assert.That(told.Select(AsText), Is.EqualTo(new[] { $"{file} | {line}" }),
                        "An asset that cannot be made is not told, or not so.");

            Assert.That(told[0].Reason, Is.Not.Empty, "Why the line is not read is not told.");

        }

        #endregion

        #region ARemotePartyLineThatCannotBeReadIsTold()

        [Test]
        public async Task ARemotePartyLineThatCannotBeReadIsTold()
        {

            var file = api.RemotePartyDBFileName;
            var line = ALine(CommonHTTPAPI.addRemoteParty, new JObject(new JProperty("id", "DE-XYZ_CPO")));

            await NextStartWith(file, line);

            Assert.That(told.Select(AsText), Is.EqualTo(new[] { $"{file} | {line}" }),
                        "A remote party that cannot be made is not told, or not so.");

            Assert.That(told[0].Reason, Is.Not.Empty, "Why the line is not read is not told.");

        }

        #endregion

        #region ALineOfACommandNotKnownIsNotTold()

        /// <summary>
        /// A line of a command the Common API does not know may be another
        /// reader's of the same file, and is not told.
        /// </summary>
        [Test]
        public async Task ALineOfACommandNotKnownIsNotTold()
        {

            await NextStartWith(api.AssetsDBFileName, ALine("addSomethingElse", new JObject(new JProperty("id", "X"))));

            Assert.That(told.Select(AsText), Is.Empty, "A line of a command not known is told.");

        }

        #endregion

        #region LinesThatAreReadAreNotTold()

        [Test]
        public async Task LinesThatAreReadAreNotTold()
        {

            var added = await api.AddTariff(ATariff("TARIFF0001"));

            Assert.That(added.IsSuccess, Is.True, $"The tariff could not be added: {added.ErrorResponse}");

            var next  = await NextStart();

            Assert.Multiple(() => {
                Assert.That(next.GetTariffs().Select(tariff => tariff.Id.ToString()), Is.EqualTo(new[] { "TARIFF0001" }), "The tariff is not read.");
                Assert.That(told.Select(AsText), Is.Empty, "A line that is read is told.");
            });

        }

        #endregion


        #region (private static) ATariff(Id) / ALine(Command, Data) / AsText(Told)

        /// <summary>
        /// A tariff of this Common API's own party.
        /// </summary>
        private static Tariff ATariff(String Id)

            => new (
                   PartyId:         ourPartyId,
                   Id:              Tariff_Id.Parse(Id),
                   VersionId:       1,
                   Currency:        Currency.EUR,
                   TariffElements:  [
                                        new TariffElement(
                                            PriceComponent.ChargingTime(
                                                2.5M,
                                                0.1M,
                                                TimeSpan.FromSeconds(300)
                                            )
                                        )
                                    ],
                   LastUpdated:     start
               );

        /// <summary>
        /// A line of a database file: the command and its data.
        /// </summary>
        private static String ALine(String Command, JToken Data)

            => new JObject(new JProperty(Command, Data)).ToString(Newtonsoft.Json.Formatting.None);

        /// <summary>
        /// The file and the line of what was told, as one text.
        /// </summary>
        private static String AsText((String FileName, String Line, String Reason) Told)

            => $"{Told.FileName} | {Told.Line}";

        #endregion

        #region (private) NextStart() / NextStartWith(FileName, Line)

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

        /// <summary>
        /// The next start, with the given line added to the given file before.
        /// </summary>
        private async Task<CommonAPI> NextStartWith(String FileName, String Line)
        {

            await api.BaseAPI.DisposeAsync();

            File.AppendAllLines(FileName, [ Line ]);

            api = ACommonAPI();

            return api;

        }

        #endregion

        #region (private) ACommonAPI() / ACommonHTTPAPI()

        /// <summary>
        /// A Common API on this test's directory. Made anew, it reads back what
        /// the one before it wrote, as the next start of a node does - and tells
        /// what it does not read to its Common HTTP API, made and subscribed to
        /// before.
        /// </summary>
        private CommonAPI ACommonAPI()

            => new (

                   OurCredentialRoles:  [
                                            new CredentialsRole(
                                                ourPartyId,
                                                Role.CPO,
                                                new BusinessDetails("GraphDefined CSO")
                                            )
                                        ],
                   DefaultPartyId:      ourPartyId,

                   BaseAPI:             ACommonHTTPAPI(),

                   URLPathPrefix:       HTTPPath.Parse("/ocpi/v3.0"),
                   DatabaseFilePath:    directory,
                   DisableLogging:      true,
                   LoggingPath:         directory

               );

        /// <summary>
        /// A Common HTTP API on this test's directory, whose lines not read are
        /// put into told.
        /// </summary>
        private CommonHTTPAPI ACommonHTTPAPI()
        {

            var baseAPI = new CommonHTTPAPI(
                              HTTPAPI:          new HTTPExtAPI(
                                                    HTTPServer: new HTTPServer(TCPPort: IPPort.Parse(3999))
                                                ),
                              OurBaseURL:       URL.Parse("http://127.0.0.1:3999/ocpi"),
                              OurVersionsURL:   URL.Parse("http://127.0.0.1:3999/ocpi/versions"),
                              RootPath:         HTTPPath.Parse("/ocpi"),
                              DisableLogging:   true,
                              LoggingPath:      directory
                          );

            baseAPI.OnDatabaseLineNotRead += (timestamp, sender, fileName, line, reason) => told.Add((fileName, line, reason));

            return baseAPI;

        }

        #endregion

    }

}
