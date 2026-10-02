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

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_3_0.UnitTests.CommonTests
{

    /// <summary>
    /// A booking location is there at the next start as it was last written
    /// down: added, changed, removed - or removed together with others.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Every change of a booking location was written to the file of the
    /// assets, and the next start passed over every one of them. Read back,
    /// it would not have been the same either: its parking options, booking
    /// terms and calendars were written as text, where objects are read; a
    /// calendar read its end as the time it was last updated; and parking
    /// options without an EVSE position came back with an empty one.
    /// </para>
    /// <para>
    /// The next start is a Common API made anew on the same directory, which
    /// reads the files back as a node's start does - see RemovedAtOnceTests.
    /// Nothing here listens: the HTTP servers are made and never started.
    /// </para>
    /// </remarks>
    [TestFixture]
    public class BookingLocationFileTests
    {

        #region Data

        private static readonly CountryCode     countryCode  = CountryCode.Parse("DE");
        private static readonly Party_Id        partyId      = Party_Id.   Parse("GEF");
        private static readonly Party_Idv3      ourPartyId   = Party_Idv3. From(countryCode, partyId);

        /// <summary>
        /// When the booking locations were last updated, unless said otherwise:
        /// a time of their own, so that none of them reads the clock.
        /// </summary>
        private static readonly DateTimeOffset  start        = new (2026, 10, 1, 8, 0, 0, TimeSpan.Zero);

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


        #region ABookingLocationAddedIsThereAtTheNextStart()

        [Test]
        public async Task ABookingLocationAddedIsThereAtTheNextStart()
        {

            var bookingLocation  = ABookingLocation("BOOKINGLOCATION0001");

            var added            = await api.AddBookingLocation(bookingLocation);

            Assert.That(added.IsSuccess, Is.True, $"The booking location could not be added: {added.ErrorResponse}");

            Assert.That(AsJSON((await NextStart()).GetBookingLocations()), Is.EquivalentTo(AsJSON([ bookingLocation ])),
                        "The next start does not know the booking location added.");

        }

        #endregion

        #region ABookingLocationAddedIfNotThereIsThereAtTheNextStart()

        [Test]
        public async Task ABookingLocationAddedIfNotThereIsThereAtTheNextStart()
        {

            var bookingLocation  = ABookingLocation("BOOKINGLOCATION0001");

            var added            = await api.AddBookingLocationIfNotExists(bookingLocation);

            Assert.That(added.IsSuccess, Is.True, $"The booking location could not be added: {added.ErrorResponse}");

            Assert.That(AsJSON((await NextStart()).GetBookingLocations()), Is.EquivalentTo(AsJSON([ bookingLocation ])),
                        "The next start does not know the booking location added.");

        }

        #endregion

        #region ABookingLocationAddedOrUpdatedIsThereAtTheNextStartAsLastWritten()

        [Test]
        public async Task ABookingLocationAddedOrUpdatedIsThereAtTheNextStartAsLastWritten()
        {

            var bookingLocation  = ABookingLocation("BOOKINGLOCATION0001");
            var changed          = ABookingLocation("BOOKINGLOCATION0001", "LOCATION0002", bookingLocation.LastUpdated + TimeSpan.FromHours(1));

            var created          = await api.AddOrUpdateBookingLocation(bookingLocation);
            var updated          = await api.AddOrUpdateBookingLocation(changed);

            Assert.Multiple(() => {
                Assert.That(created.WasCreated, Is.True, $"The booking location was not created: {created.ErrorResponse}");
                Assert.That(updated.WasUpdated, Is.True, $"The booking location was not updated: {updated.ErrorResponse}");
            });

            Assert.That(AsJSON((await NextStart()).GetBookingLocations()), Is.EquivalentTo(AsJSON([ changed ])),
                        "The next start does not know the booking location as it was last written.");

        }

        #endregion

        #region ABookingLocationUpdatedIsThereAtTheNextStartAsUpdated()

        [Test]
        public async Task ABookingLocationUpdatedIsThereAtTheNextStartAsUpdated()
        {

            var bookingLocation  = ABookingLocation("BOOKINGLOCATION0001");
            var changed          = ABookingLocation("BOOKINGLOCATION0001", "LOCATION0002", bookingLocation.LastUpdated + TimeSpan.FromHours(1));

            var added            = await api.AddBookingLocation   (bookingLocation);
            var updated          = await api.UpdateBookingLocation(changed);

            Assert.Multiple(() => {
                Assert.That(added.  IsSuccess, Is.True, $"The booking location could not be added: {added.ErrorResponse}");
                Assert.That(updated.IsSuccess, Is.True, $"The booking location could not be updated: {updated.ErrorResponse}");
            });

            Assert.That(AsJSON((await NextStart()).GetBookingLocations()), Is.EquivalentTo(AsJSON([ changed ])),
                        "The next start does not know the booking location as it was updated.");

        }

        #endregion

        #region ABookingLocationRemovedIsGoneAtTheNextStart()

        [Test]
        public async Task ABookingLocationRemovedIsGoneAtTheNextStart()
        {

            var removed  = ABookingLocation("BOOKINGLOCATION0001");
            var kept     = ABookingLocation("BOOKINGLOCATION0002");

            var added1   = await api.AddBookingLocation(removed);
            var added2   = await api.AddBookingLocation(kept);
            var removal  = await api.RemoveBookingLocation(ourPartyId, removed.Id);

            Assert.Multiple(() => {
                Assert.That(added1. IsSuccess, Is.True, $"The booking location to remove could not be added: {added1.ErrorResponse}");
                Assert.That(added2. IsSuccess, Is.True, $"The booking location to keep could not be added: {added2.ErrorResponse}");
                Assert.That(removal.IsSuccess, Is.True, $"The booking location could not be removed: {removal.ErrorResponse}");
            });

            Assert.That(AsJSON((await NextStart()).GetBookingLocations()), Is.EquivalentTo(AsJSON([ kept ])),
                        "The next start knows the booking location removed, or not the one kept.");

        }

        #endregion

        #region TheBookingLocationsRemovedAtOnceAreGoneAtTheNextStartAndNoOthers()

        /// <summary>
        /// The booking locations a filter picked out are removed at once, and
        /// the next start knows those removed no more - and still knows the
        /// others.
        /// </summary>
        [Test]
        public async Task TheBookingLocationsRemovedAtOnceAreGoneAtTheNextStartAndNoOthers()
        {

            var removed1  = ABookingLocation("BOOKINGLOCATION0001");
            var kept      = ABookingLocation("BOOKINGLOCATION0002");
            var removed2  = ABookingLocation("BOOKINGLOCATION0003");

            foreach (var bookingLocation in new[] { removed1, kept, removed2 })
            {
                var added = await api.AddBookingLocation(bookingLocation);
                Assert.That(added.IsSuccess, Is.True, $"The booking location '{bookingLocation.Id}' could not be added: {added.ErrorResponse}");
            }

            var removal   = await api.RemoveAllBookingLocations(bookingLocation => bookingLocation.Id != kept.Id);

            Assert.That(removal.Data?.Select(bookingLocation => bookingLocation.Id), Is.EquivalentTo(new[] { removed1.Id, removed2.Id }),
                        $"Not the booking locations picked out were removed: {removal.ErrorResponse}");

            Assert.That(AsJSON((await NextStart()).GetBookingLocations()), Is.EquivalentTo(AsJSON([ kept ])),
                        "The next start knows a booking location removed at once, or not the one kept.");

        }

        #endregion


        #region (private static) ABookingLocation(Id, LocationId = null, LastUpdated = null)

        /// <summary>
        /// A booking location of this Common API's own party, which it keeps
        /// booking locations for, with all a booking location can say: its
        /// parking options, booking terms and calendar among them.
        /// </summary>
        private static BookingLocation ABookingLocation(String           Id,
                                                        String?          LocationId    = null,
                                                        DateTimeOffset?  LastUpdated   = null)

            => new (
                   CountryCode:             countryCode,
                   PartyId:                 partyId,
                   Id:                      BookingLocation_Id.Parse(Id),
                   LocationId:              Location_Id.       Parse(LocationId ?? "LOCATION0001"),
                   EVSEUIds:                [ EVSE_UId.Parse("DE*GEF*E0001*1") ],
                   BookableParkingOptions:  [
                                                new BookableParkingOptions(
                                                    Format:        ConnectorFormats.CABLE,
                                                    VehicleTypes:  [ VehicleType.PERSONAL_VEHICLE ],
                                                    DriveThrough:  true
                                                )
                                            ],
                   Bookable:                new Bookable(
                                                ReservationRequired:  true
                                            ),
                   TariffIds:               [ Tariff_Id.Parse("TARIFF0001") ],
                   BookingTerms:            [
                                                new BookingTerms(
                                                    SupportedAccessMethods:  [ LocationAccess.TOKEN ],
                                                    ChangeUntil:             TimeSpan.FromMinutes(60),
                                                    CancelUntil:             TimeSpan.FromMinutes(30)
                                                )
                                            ],
                   Calendar:                [
                                                new Calendar(
                                                    Id:                  Calendar_Id.Parse("CALENDAR0001"),
                                                    BeginFrom:           start + TimeSpan.FromDays(1),
                                                    EndBefore:           start + TimeSpan.FromDays(2),
                                                    AvailableTimeSlots:  [
                                                                             new TimeSlot(
                                                                                 start + TimeSpan.FromDays(1) + TimeSpan.FromHours(8),
                                                                                 start + TimeSpan.FromDays(1) + TimeSpan.FromHours(10)
                                                                             )
                                                                         ],
                                                    LastUpdated:         start
                                                )
                                            ],
                   LastUpdated:             LastUpdated ?? start
               );

        #endregion

        #region (private static) AsJSON(BookingLocations)

        /// <summary>
        /// Booking locations as their JSON, compared with all they say: two
        /// booking locations are equal already when their identifications are.
        /// </summary>
        private static IEnumerable<String> AsJSON(IEnumerable<BookingLocation> BookingLocations)

            => BookingLocations.Select(bookingLocation => bookingLocation.ToJSON().ToString(Newtonsoft.Json.Formatting.None));

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

                   OurPartyData:      [
                                          new PartyData(
                                              ourPartyId,
                                              Role.CPO,
                                              new BusinessDetails("GraphDefined CSO")
                                          )
                                      ],
                   DefaultPartyId:    ourPartyId,

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

    }

}
