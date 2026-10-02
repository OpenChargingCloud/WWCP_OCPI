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
    /// A booking is there at the next start as it was last written down:
    /// added, changed, removed - or removed together with others.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Every change of a booking was written to the file of the assets, and
    /// the next start passed over every one of them: no booking was there
    /// any more once the process had ended.
    /// </para>
    /// <para>
    /// The next start is a Common API made anew on the same directory, which
    /// reads the files back as a node's start does - see RemovedAtOnceTests.
    /// Nothing here listens: the HTTP servers are made and never started.
    /// </para>
    /// </remarks>
    [TestFixture]
    public class BookingFileTests
    {

        #region Data

        private static readonly CountryCode     countryCode  = CountryCode.Parse("DE");
        private static readonly Party_Id        partyId      = Party_Id.   Parse("GEF");
        private static readonly Party_Idv3      ourPartyId   = Party_Idv3. From(countryCode, partyId);

        /// <summary>
        /// When the bookings were last updated, unless said otherwise: a time
        /// of their own, so that none of them reads the clock.
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


        #region ABookingAddedIsThereAtTheNextStart()

        [Test]
        public async Task ABookingAddedIsThereAtTheNextStart()
        {

            var booking  = ABooking("BOOKING0001");

            var added    = await api.AddBooking(booking);

            Assert.That(added.IsSuccess, Is.True, $"The booking could not be added: {added.ErrorResponse}");

            Assert.That(AsJSON((await NextStart()).GetBookings()), Is.EquivalentTo(AsJSON([ booking ])),
                        "The next start does not know the booking added.");

        }

        #endregion

        #region ABookingAddedIfNotThereIsThereAtTheNextStart()

        [Test]
        public async Task ABookingAddedIfNotThereIsThereAtTheNextStart()
        {

            var booking  = ABooking("BOOKING0001");

            var added    = await api.AddBookingIfNotExists(booking);

            Assert.That(added.IsSuccess, Is.True, $"The booking could not be added: {added.ErrorResponse}");

            Assert.That(AsJSON((await NextStart()).GetBookings()), Is.EquivalentTo(AsJSON([ booking ])),
                        "The next start does not know the booking added.");

        }

        #endregion

        #region ABookingAddedOrUpdatedIsThereAtTheNextStartAsLastWritten()

        [Test]
        public async Task ABookingAddedOrUpdatedIsThereAtTheNextStartAsLastWritten()
        {

            var booking  = ABooking("BOOKING0001");
            var changed  = ABooking("BOOKING0001", ReservationStatus.FULFILLED, booking.LastUpdated + TimeSpan.FromHours(1));

            var created  = await api.AddOrUpdateBooking(booking);
            var updated  = await api.AddOrUpdateBooking(changed);

            Assert.Multiple(() => {
                Assert.That(created.WasCreated, Is.True, $"The booking was not created: {created.ErrorResponse}");
                Assert.That(updated.WasUpdated, Is.True, $"The booking was not updated: {updated.ErrorResponse}");
            });

            Assert.That(AsJSON((await NextStart()).GetBookings()), Is.EquivalentTo(AsJSON([ changed ])),
                        "The next start does not know the booking as it was last written.");

        }

        #endregion

        #region ABookingUpdatedIsThereAtTheNextStartAsUpdated()

        [Test]
        public async Task ABookingUpdatedIsThereAtTheNextStartAsUpdated()
        {

            var booking  = ABooking("BOOKING0001");
            var changed  = ABooking("BOOKING0001", ReservationStatus.FULFILLED, booking.LastUpdated + TimeSpan.FromHours(1));

            var added    = await api.AddBooking   (booking);
            var updated  = await api.UpdateBooking(changed);

            Assert.Multiple(() => {
                Assert.That(added.  IsSuccess, Is.True, $"The booking could not be added: {added.ErrorResponse}");
                Assert.That(updated.IsSuccess, Is.True, $"The booking could not be updated: {updated.ErrorResponse}");
            });

            Assert.That(AsJSON((await NextStart()).GetBookings()), Is.EquivalentTo(AsJSON([ changed ])),
                        "The next start does not know the booking as it was updated.");

        }

        #endregion

        #region ABookingRemovedIsGoneAtTheNextStart()

        [Test]
        public async Task ABookingRemovedIsGoneAtTheNextStart()
        {

            var removed  = ABooking("BOOKING0001");
            var kept     = ABooking("BOOKING0002");

            var added1   = await api.AddBooking(removed);
            var added2   = await api.AddBooking(kept);
            var removal  = await api.RemoveBooking(ourPartyId, removed.Id);

            Assert.Multiple(() => {
                Assert.That(added1. IsSuccess, Is.True, $"The booking to remove could not be added: {added1.ErrorResponse}");
                Assert.That(added2. IsSuccess, Is.True, $"The booking to keep could not be added: {added2.ErrorResponse}");
                Assert.That(removal.IsSuccess, Is.True, $"The booking could not be removed: {removal.ErrorResponse}");
            });

            Assert.That(AsJSON((await NextStart()).GetBookings()), Is.EquivalentTo(AsJSON([ kept ])),
                        "The next start knows the booking removed, or not the one kept.");

        }

        #endregion

        #region TheBookingsRemovedAtOnceAreGoneAtTheNextStartAndNoOthers()

        /// <summary>
        /// The bookings a filter picked out are removed at once, and the next
        /// start knows those removed no more - and still knows the others.
        /// </summary>
        [Test]
        public async Task TheBookingsRemovedAtOnceAreGoneAtTheNextStartAndNoOthers()
        {

            var removed1  = ABooking("BOOKING0001");
            var kept      = ABooking("BOOKING0002");
            var removed2  = ABooking("BOOKING0003");

            foreach (var booking in new[] { removed1, kept, removed2 })
            {
                var added = await api.AddBooking(booking);
                Assert.That(added.IsSuccess, Is.True, $"The booking '{booking.Id}' could not be added: {added.ErrorResponse}");
            }

            var removal   = await api.RemoveAllBookings(booking => booking.Id != kept.Id);

            Assert.That(removal.Data?.Select(booking => booking.Id), Is.EquivalentTo(new[] { removed1.Id, removed2.Id }),
                        $"Not the bookings picked out were removed: {removal.ErrorResponse}");

            Assert.That(AsJSON((await NextStart()).GetBookings()), Is.EquivalentTo(AsJSON([ kept ])),
                        "The next start knows a booking removed at once, or not the one kept.");

        }

        #endregion


        #region ABookingTokensLicensePlateIsThereAtTheNextStart()

        /// <summary>
        /// A booking whose token carries a license plate is there at the next
        /// start with it: the plate was written down, and not read back.
        /// </summary>
        [Test]
        public async Task ABookingTokensLicensePlateIsThereAtTheNextStart()
        {

            var booking  = ABooking("BOOKING0001", LicensePlate: "J-GD 2026");

            var added    = await api.AddBooking(booking);

            Assert.That(added.IsSuccess, Is.True, $"The booking could not be added: {added.ErrorResponse}");

            Assert.That(AsJSON((await NextStart()).GetBookings()), Is.EquivalentTo(AsJSON([ booking ])),
                        "The next start does not know the license plate of the booking's token.");

        }

        #endregion


        #region (private static) ABooking(Id, Status = null, LastUpdated = null, LicensePlate = null)

        /// <summary>
        /// A booking of this Common API's own party, which it keeps bookings
        /// for: reserved, unless said otherwise.
        /// </summary>
        private static Booking ABooking(String              Id,
                                        ReservationStatus?  Status         = null,
                                        DateTimeOffset?     LastUpdated    = null,
                                        String?             LicensePlate   = null)

            => new (
                   Id:                      Booking_Id.            Parse(Id),
                   CountryCode:             countryCode,
                   PartyId:                 partyId,
                   RequestId:               Request_Id.            Parse($"REQUEST-{Id}"),
                   LocationId:              Location_Id.           Parse("LOCATION0001"),
                   BookingTokens:           [
                                                new BookingToken(
                                                    countryCode,
                                                    partyId,
                                                    Token_Id.   Parse("TOKEN0001"),
                                                    TokenType.  RFID,
                                                    Contract_Id.Parse("DE-GEF-C12345678-X"),
                                                    LicensePlate
                                                )
                                            ],
                   TariffIds:               [ Tariff_Id.Parse("TARIFF0001") ],
                   Period:                  new TimeSlot(
                                                start + TimeSpan.FromDays(1),
                                                start + TimeSpan.FromDays(1) + TimeSpan.FromHours(2)
                                            ),
                   ReservationStatus:       Status ?? ReservationStatus.RESERVED,
                   AuthorizationReference:  AuthorizationReference.Parse($"AUTH-{Id}"),
                   BookingTerms:            new BookingTerms(
                                                SupportedAccessMethods:  [ LocationAccess.TOKEN ],
                                                ChangeUntil:             TimeSpan.FromMinutes(60),
                                                CancelUntil:             TimeSpan.FromMinutes(30)
                                            ),
                   LastUpdated:             LastUpdated ?? start
               );

        #endregion

        #region (private static) AsJSON(Bookings)

        /// <summary>
        /// Bookings as their JSON, compared with all they say: two bookings are
        /// equal already when their identifications are.
        /// </summary>
        private static IEnumerable<String> AsJSON(IEnumerable<Booking> Bookings)

            => Bookings.Select(booking => booking.ToJSON().ToString(Newtonsoft.Json.Formatting.None));

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
