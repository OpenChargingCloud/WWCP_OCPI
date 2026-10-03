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

using System.Collections.Concurrent;
using System.Threading.Channels;

using NUnit.Framework;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_3_0.UnitTests.CommonTests
{

    /// <summary>
    /// The assets are written through a queue, and disposing the Common HTTP
    /// API writes out what the queue still holds: every asset handed over
    /// before is in its file then, or said not to be.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Nothing disposed the queue, and it went down with the process: what it
    /// still held was never written, and the next start did not know it. A
    /// line its file refused was told to the debug log alone, which a Release
    /// build does not have.
    /// </para>
    /// <para>
    /// Nothing here listens: the HTTP servers are made and never started.
    /// </para>
    /// </remarks>
    [TestFixture]
    public class AssetFileTests
    {

        #region Data

        private static readonly CountryCode     countryCode  = CountryCode.Parse("DE");
        private static readonly Party_Id        partyId      = Party_Id.   Parse("BBB");
        private static readonly RemoteParty_Id  id           = RemoteParty_Id.Parse("DE-BBB_CPO");

        private String     directory  = default!;
        private CommonAPI  api        = default!;

        #endregion

        #region SetUp / TearDown

        [SetUp]
        public void SetUp()
        {

            directory  = Path.Combine(Path.GetTempPath(), $"WWCP_OCPI_Tests-{Guid.NewGuid():N}");

            Directory.CreateDirectory(directory);

            api        = ACommonAPI();

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


        #region EveryAssetHandedOverIsInItsFileOnceTheAPIIsDisposed()

        /// <summary>
        /// Two thousand assets handed over at once, as an import does, and the
        /// API disposed right after them: every one is in the file, in order -
        /// and disposing it a second time does no harm.
        /// </summary>
        [Test]
        public async Task EveryAssetHandedOverIsInItsFileOnceTheAPIIsDisposed()
        {

            for (var number = 1; number <= 2000; number++)
                await api.LogAsset("probe", number, EventTracking_Id.New);

            await api.BaseAPI.DisposeAsync();

            Assert.That(ProbesIn(api.AssetsDBFileName), Is.EqualTo(Enumerable.Range(1, 2000).Select(number => (Int64) number)),
                        "Not every asset handed over before the API was disposed is in its file.");

            await Assert.DoesNotThrowAsync(async () => await api.BaseAPI.DisposeAsync(),
                                           "Disposing the API a second time threw.");

        }

        #endregion

        #region AnAssetHandedOverOnceTheAPIIsDisposedIsRefused()

        /// <summary>
        /// An asset handed over after the queue was written out is refused to
        /// the caller, rather than taken and never written.
        /// </summary>
        [Test]
        public async Task AnAssetHandedOverOnceTheAPIIsDisposedIsRefused()
        {

            await api.BaseAPI.DisposeAsync();

            await Assert.ThrowsAsync<ChannelClosedException>(async () => await api.LogAsset("probe", 1, EventTracking_Id.New),
                                                             "An asset handed over once the API was disposed was taken.");

        }

        #endregion

        #region AnAssetItsFileRefusesIsSaidToOnDatabaseLineNotWritten()

        /// <summary>
        /// An asset its file cannot take is said to OnDatabaseLineNotWritten,
        /// with the file and the line: the call that handed it over returned
        /// long before, and nobody else hears of it.
        /// </summary>
        /// <remarks>
        /// The file is made unwritable the way that stops root as well: a
        /// directory where it would be.
        /// </remarks>
        [Test]
        public async Task AnAssetItsFileRefusesIsSaidToOnDatabaseLineNotWritten()
        {

            var reports = new ConcurrentQueue<(String FileName, String Line, Exception Exception)>();

            api.BaseAPI.OnDatabaseLineNotWritten += (timestamp, sender, fileName, line, exception) => reports.Enqueue((fileName, line, exception));

            if (File.Exists(api.AssetsDBFileName))
                File.Move(api.AssetsDBFileName, api.AssetsDBFileName + ".aside");

            Directory.CreateDirectory(api.AssetsDBFileName);

            await api.LogAsset("probe", 42, EventTracking_Id.New);

            await api.BaseAPI.DisposeAsync();

            Assert.That(reports, Has.Count.EqualTo(1), "The asset its file refused was not said to OnDatabaseLineNotWritten.");

            var report = reports.Single();

            Assert.Multiple(() => {
                Assert.That(report.FileName,                       Is.EqualTo(api.AssetsDBFileName));
                Assert.That(JObject.Parse(report.Line)["probe"]?.Value<Int64>(), Is.EqualTo(42));
                Assert.That(report.Exception,                      Is.Not.Null);
            });

        }

        #endregion

        #region AnotherAPIOnTheSameDirectoryStillWritesOnceOneIsDisposed()

        /// <summary>
        /// Disposing one API leaves the locks of the database files alone: they
        /// belong to the process, and a node made anew on the same directory -
        /// as the tests of every node do - still writes its remote parties.
        /// </summary>
        [Test]
        public async Task AnotherAPIOnTheSameDirectoryStillWritesOnceOneIsDisposed()
        {

            var other = ACommonAPI();

            try
            {

                var before  = await other.AddRemoteParty(RemoteParty_Id.Parse("DE-CCC_CPO"), Roles, AccessToken.NewRandom());

                await api.BaseAPI.DisposeAsync();

                var after   = await other.AddRemoteParty(id, Roles, AccessToken.NewRandom());

                Assert.Multiple(() => {
                    Assert.That(before.IsSuccess,  Is.True,  $"The remote party could not be added: {before.ErrorResponse}");
                    Assert.That(after. IsSuccess,  Is.True,  $"Once the other API was disposed, a remote party could not be added: {after.ErrorResponse}");
                });

                Assert.That(File.ReadAllLines(other.RemotePartyDBFileName).Count(line => line.Contains(CommonHTTPAPI.addRemoteParty)), Is.EqualTo(2),
                            "Not every remote party added is in its file.");

            }
            finally
            {
                await other.BaseAPI.DisposeAsync();
            }

        }

        #endregion


        #region (private static) Roles

        /// <summary>
        /// Who the remote party is.
        /// </summary>
        private static IEnumerable<CredentialsRole> Roles

            => [
                   new CredentialsRole(
                       countryCode,
                       partyId,
                       Role.CPO,
                       new BusinessDetails("Test CPO")
                   )
               ];

        #endregion

        #region (private) ACommonAPI()

        /// <summary>
        /// A Common API on this test's directory. Made anew, it reads back what
        /// the one before it wrote, as the next start of a node does.
        /// </summary>
        private CommonAPI ACommonAPI()

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
                                          HTTPAPI:          new HTTPExtAPI(
                                                                HTTPServer: new HTTPServer(TCPPort: IPPort.Parse(3999))
                                                            ),
                                          OurBaseURL:       URL.Parse("http://127.0.0.1:3999/ocpi"),
                                          OurVersionsURL:   URL.Parse("http://127.0.0.1:3999/ocpi/versions"),
                                          RootPath:         HTTPPath.Parse("/ocpi"),
                                          DisableLogging:   true,
                                          LoggingPath:      directory
                                      ),

                   URLPathPrefix:     HTTPPath.Parse("/ocpi/v2.3.0"),
                   DatabaseFilePath:  directory,
                   DisableLogging:    true,
                   LoggingPath:       directory

               );

        #endregion

        #region (private static) ProbesIn(FileName)

        /// <summary>
        /// The numbers of the "probe" assets in a file, in the order they are
        /// in it: none where it is not there. Read the way a file the queue may
        /// still be writing to can be read.
        /// </summary>
        private static IEnumerable<Int64> ProbesIn(String FileName)
        {

            if (!File.Exists(FileName))
                return [];

            using var stream  = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
            using var reader  = new StreamReader(stream);

            var probes = new List<Int64>();

            while (reader.ReadLine() is String line)
                if (line.StartsWith("{\"probe\":"))
                    probes.Add(JObject.Parse(line)["probe"]!.Value<Int64>());

            return probes;

        }

        #endregion

    }

}
