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

namespace cloud.charging.open.protocols.OCPIv2_1_1.UnitTests.CommonTests
{

    /// <summary>
    /// The locations two CPOs push to an EMSP are each one's own, also where
    /// they have the same id, and are there at the next start.
    /// </summary>
    /// <remarks>
    /// The Common API kept its locations by their id alone: the second CPO's
    /// location of an id replaced the first one's, and a PATCH or a removal
    /// of one CPO's location could reach the other's.
    ///
    /// The next start is a Common API made anew on the same directory, which
    /// reads back what it wrote, as a node's start does. Nothing here listens:
    /// the HTTP server is made and never started.
    /// </remarks>
    [TestFixture]
    public class RemoteLocationsFileTests
    {

        #region Data

        private static readonly CountryCode     de          = CountryCode.Parse("DE");
        private static readonly Party_Id        cpoA        = Party_Id.Parse("AAA");
        private static readonly Party_Id        cpoB        = Party_Id.Parse("BBB");
        private static readonly Location_Id     id          = Location_Id.Parse("LOC0001");

        /// <summary>
        /// When the locations were last updated: a time of their own, so that
        /// they do not read the clock.
        /// </summary>
        private static readonly DateTimeOffset  start       = new (2026, 10, 1, 8, 0, 0, TimeSpan.Zero);

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


        #region TheLocationsOfTwoCPOsOfOneIdAreKeptApartAndAreThereAtTheNextStart()

        [Test]
        public async Task TheLocationsOfTwoCPOsOfOneIdAreKeptApartAndAreThereAtTheNextStart()
        {

            var pushedA = await api.AddOrUpdateLocation(ALocation(cpoA, "Jena"));
            var pushedB = await api.AddOrUpdateLocation(ALocation(cpoB, "Erfurt"));

            Assert.Multiple(() => {
                Assert.That(pushedA.IsSuccess, Is.True, $"CPO A's location was refused: {pushedA.ErrorResponse}");
                Assert.That(pushedB.IsSuccess, Is.True, $"CPO B's location was refused: {pushedB.ErrorResponse}");
            });

            await api.BaseAPI.DisposeAsync();

            api = ACommonAPI();

            Assert.Multiple(() => {

                Assert.That(api.GetLocations(de, cpoA).Select(location => $"{location.Id} {location.City}"), Is.EquivalentTo(new[] { "LOC0001 Jena" }),
                            "At the next start CPO A's location is not as it was pushed.");

                Assert.That(api.GetLocations(de, cpoB).Select(location => $"{location.Id} {location.City}"), Is.EquivalentTo(new[] { "LOC0001 Erfurt" }),
                            "At the next start CPO B's location is not as it was pushed.");

                Assert.That(api.GetLocations().Count(), Is.EqualTo(2), "The locations in all are not both.");

                Assert.That(api.TryGetLocation(de, cpoA, id, out var a) ? a.City : "none", Is.EqualTo("Jena"));
                Assert.That(api.TryGetLocation(de, cpoB, id, out var b) ? b.City : "none", Is.EqualTo("Erfurt"));

            });

        }

        #endregion

        #region APatchOfOneCPOsLocationLeavesTheOthersOfTheSameId()

        [Test]
        public async Task APatchOfOneCPOsLocationLeavesTheOthersOfTheSameId()
        {

            await api.AddOrUpdateLocation(ALocation(cpoA, "Jena"));
            await api.AddOrUpdateLocation(ALocation(cpoB, "Erfurt"));

            var patched = await api.TryPatchLocation(
                                    de,
                                    cpoB,
                                    id,
                                    new JObject(
                                        new JProperty("name",          "Patched"),
                                        new JProperty("last_updated",  (start + TimeSpan.FromHours(1)).ToISO8601())
                                    )
                                );

            Assert.Multiple(() => {
                Assert.That(patched.IsSuccess, Is.True, $"CPO B's location could not be patched: {patched.ErrorResponse}");
                Assert.That(api.TryGetLocation(de, cpoA, id, out var a) ? a.Name : "none", Is.Null,          "CPO A's location was patched with CPO B's.");
                Assert.That(api.TryGetLocation(de, cpoB, id, out var b) ? b.Name : "none", Is.EqualTo("Patched"), "CPO B's location was not patched.");
            });

        }

        #endregion

        #region ALocationRemovedIsOnlyThatPartys()

        [Test]
        public async Task ALocationRemovedIsOnlyThatPartys()
        {

            await api.AddOrUpdateLocation(ALocation(cpoA, "Jena"));
            await api.AddOrUpdateLocation(ALocation(cpoB, "Erfurt"));

            var removed = await api.RemoveLocation(ALocation(cpoA, "Jena"));

            await api.BaseAPI.DisposeAsync();

            api = ACommonAPI();

            Assert.Multiple(() => {
                Assert.That(removed.IsSuccess,                    Is.True,  $"CPO A's location could not be removed: {removed.ErrorResponse}");
                Assert.That(api.LocationExists(de, cpoA, id),     Is.False, "CPO A's location is still there.");
                Assert.That(api.LocationExists(de, cpoB, id),     Is.True,  "CPO B's location went with CPO A's.");
            });

        }

        #endregion


        #region (private static) ALocation(PartyId, City)

        /// <summary>
        /// A location of the given party, of the one id of these tests.
        /// </summary>
        private static Location ALocation(Party_Id PartyId, String City)

            => new (
                   de,
                   PartyId,
                   id,
                   LocationType.PARKING_LOT,
                   "Biberweg 18",
                   City,
                   "07749",
                   Country.Germany,
                   GeoCoordinate.Parse(50.9, 11.6),
                   LastUpdated: start
               );

        #endregion

        #region (private) ACommonAPI()

        /// <summary>
        /// The Common API of an EMSP on this test's directory. Made anew, it
        /// reads back what the one before it wrote, as the next start of a
        /// node does.
        /// </summary>
        private CommonAPI ACommonAPI()

            => new (

                   OurBusinessDetails:  new BusinessDetails("GraphDefined EMSP"),
                   OurCountryCode:      CountryCode.Parse("DE"),
                   OurPartyId:          Party_Id.   Parse("GDF"),
                   OurRole:             Role.EMSP,

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

                   URLPathPrefix:       HTTPPath.Parse("/ocpi/v2.1.1"),
                   DatabaseFilePath:    directory,
                   DisableLogging:      true,
                   LoggingPath:         directory

               );

        #endregion

    }

}
