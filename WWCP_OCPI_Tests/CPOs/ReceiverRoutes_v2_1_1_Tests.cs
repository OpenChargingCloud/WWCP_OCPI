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

using cloud.charging.open.protocols.OCPI.UnitTests;

#endregion

namespace cloud.charging.open.protocols.OCPI.CPO.UnitTests
{

    /// <summary>
    /// An EMSP's receiver routes of OCPI 2.1.1 name the CPO's party, as the
    /// specification has them: /locations/{country_code}/{party_id}/{location_id},
    /// and so on for EVSEs, connectors, tariffs and sessions.
    /// </summary>
    /// <remarks>
    /// They named none - /locations/{location_id} - and the library's own
    /// client sent none, so between two nodes of this library it worked; a
    /// CPO that sent what the specification says hit the EVSE route with the
    /// country code as the location id, and was told the location is unknown.
    /// </remarks>
    [TestFixture]
    public class ReceiverRoutes_v2_1_1_Tests : A_2CPOs2EMSPs_TestDefaults
    {

        #region ALocationIsPutUnderTheCPOsParty()

        [Test]
        public async Task ALocationIsPutUnderTheCPOsParty()
        {

            var response = await EMSP1().PutLocation(ALocation("GEF", "LOC0101"));

            Assert.Multiple(() => {
                Assert.That(response.StatusCode.Value,                         Is.EqualTo(1000), response.StatusMessage);
                Assert.That(response.HTTPResponse?.HTTPStatusCode.Code,        Is.EqualTo(201));
                Assert.That(response.HTTPResponse?.HTTPRequest?.Path.ToString(), Does.EndWith("/locations/DE/GEF/LOC0101"),
                            "The location was not put under the CPO's party.");
            });

        }

        #endregion

        #region ALocationOfAnotherPartyIsRefused()

        [Test]
        public async Task ALocationOfAnotherPartyIsRefused()
        {

            // The CPO's client sends the location under its own party: here
            // one that is not the CPO's.
            var response = await EMSP1().PutLocation(ALocation("XYZ", "LOC0102"));

            Assert.Multiple(() => {
                Assert.That(response.HTTPResponse?.HTTPRequest?.Path.ToString(), Does.EndWith("/locations/DE/XYZ/LOC0102"));
                Assert.That(response.HTTPResponse?.HTTPStatusCode.Code,          Is.EqualTo(403));
                Assert.That(response.StatusMessage,                              Is.EqualTo("The party 'DEXYZ' the URL names is not the caller's!"));
            });

        }

        #endregion


        #region TwoCPOsMayPutALocationOfTheSameId()

        /// <summary>
        /// The EMSP kept the locations by their id alone: the second CPO's
        /// location of an id replaced the first one's.
        /// </summary>
        [Test]
        public async Task TwoCPOsMayPutALocationOfTheSameId()
        {

            var cpo2 = cpo2CPOAPI_v2_1_1?.GetEMSPClient(
                           CountryCode: CountryCode.Parse("DE"),
                           PartyId:     Party_Id.   Parse("GDF")
                       );

            Assert.That(cpo2, Is.Not.Null);

            var response1 = await EMSP1().PutLocation(ALocation("GEF", "LOC0103"));
            var response2 = await cpo2!. PutLocation(ALocation("GE2", "LOC0103"));

            Assert.Multiple(() => {

                Assert.That(response1.StatusCode.Value, Is.EqualTo(1000), response1.StatusMessage);
                Assert.That(response2.StatusCode.Value, Is.EqualTo(1000), response2.StatusMessage);

                Assert.That(emsp1CommonAPI_v2_1_1!.GetLocations(CountryCode.Parse("DE"), Party_Id.Parse("GEF")).Select(location => location.Id.ToString()), Does.Contain("LOC0103"),
                            "CPO #1's location is gone.");

                Assert.That(emsp1CommonAPI_v2_1_1!.GetLocations(CountryCode.Parse("DE"), Party_Id.Parse("GE2")).Select(location => location.Id.ToString()), Does.Contain("LOC0103"),
                            "CPO #2's location is not there.");

            });

        }

        #endregion


        #region TwoCPOsMayPutASessionOfTheSameId()

        /// <summary>
        /// The EMSP kept the sessions by their id alone: the second CPO's
        /// session of an id replaced the first one's.
        /// </summary>
        [Test]
        public async Task TwoCPOsMayPutASessionOfTheSameId()
        {

            var cpo2 = cpo2CPOAPI_v2_1_1?.GetEMSPClient(
                           CountryCode: CountryCode.Parse("DE"),
                           PartyId:     Party_Id.   Parse("GDF")
                       );

            Assert.That(cpo2, Is.Not.Null);

            var response1 = await EMSP1().PutSession(ASession("GEF", "SESSION0103"));
            var response2 = await cpo2!. PutSession(ASession("GE2", "SESSION0103"));

            Assert.Multiple(() => {

                Assert.That(response1.StatusCode.Value, Is.EqualTo(1000), response1.StatusMessage);
                Assert.That(response2.StatusCode.Value, Is.EqualTo(1000), response2.StatusMessage);

                Assert.That(emsp1CommonAPI_v2_1_1!.GetSessions(CountryCode.Parse("DE"), Party_Id.Parse("GEF")).Select(session => session.Id.ToString()), Does.Contain("SESSION0103"),
                            "CPO #1's session is gone.");

                Assert.That(emsp1CommonAPI_v2_1_1!.GetSessions(CountryCode.Parse("DE"), Party_Id.Parse("GE2")).Select(session => session.Id.ToString()), Does.Contain("SESSION0103"),
                            "CPO #2's session is not there.");

            });

        }

        #endregion


        #region TwoCPOsMayPutATariffOfTheSameId()

        /// <summary>
        /// The EMSP kept the tariffs by their id alone: the second CPO's
        /// tariff of an id replaced the first one's. And the CPO's client
        /// read the tariff it got back without the party it named, so that
        /// every answer as OCPI has it failed.
        /// </summary>
        [Test]
        public async Task TwoCPOsMayPutATariffOfTheSameId()
        {

            var cpo2 = cpo2CPOAPI_v2_1_1?.GetEMSPClient(
                           CountryCode: CountryCode.Parse("DE"),
                           PartyId:     Party_Id.   Parse("GDF")
                       );

            Assert.That(cpo2, Is.Not.Null);

            var response1 = await EMSP1().PutTariff(ATariff("GEF", "TARIFF0103"));
            var response2 = await cpo2!. PutTariff(ATariff("GE2", "TARIFF0103"));

            Assert.Multiple(() => {

                Assert.That(response1.StatusCode.Value, Is.EqualTo(1000), response1.StatusMessage);
                Assert.That(response2.StatusCode.Value, Is.EqualTo(1000), response2.StatusMessage);

                Assert.That(emsp1CommonAPI_v2_1_1!.GetTariffs(CountryCode.Parse("DE"), Party_Id.Parse("GEF")).Select(tariff => tariff.Id.ToString()), Does.Contain("TARIFF0103"),
                            "CPO #1's tariff is gone.");

                Assert.That(emsp1CommonAPI_v2_1_1!.GetTariffs(CountryCode.Parse("DE"), Party_Id.Parse("GE2")).Select(tariff => tariff.Id.ToString()), Does.Contain("TARIFF0103"),
                            "CPO #2's tariff is not there.");

            });

        }

        #endregion


        #region (private) EMSP1() / ALocation(PartyId, LocationId) / ASession(PartyId, SessionId) / ATariff(PartyId, TariffId)

        /// <summary>
        /// CPO #1's client of EMSP #1, on OCPI 2.1.1.
        /// </summary>
        private OCPIv2_1_1.CPO.HTTP.CPO2EMSP_HTTPClient EMSP1()
        {

            var client = cpo1CPOAPI_v2_1_1?.GetEMSPClient(
                             CountryCode: CountryCode.Parse("DE"),
                             PartyId:     Party_Id.   Parse("GDF")
                         );

            Assert.That(client, Is.Not.Null);

            return client!;

        }

        /// <summary>
        /// A location of the given party.
        /// </summary>
        private static OCPIv2_1_1.Location ALocation(String PartyId, String LocationId)

            => new (
                   CountryCode.Parse("DE"),
                   Party_Id.   Parse(PartyId),
                   Location_Id.Parse(LocationId),
                   OCPIv2_1_1.LocationType.PARKING_LOT,
                   "Biberweg 18",
                   "Jena",
                   "07749",
                   Country.Germany,
                   GeoCoordinate.Parse(50.9, 11.6)
               );

        /// <summary>
        /// An active session of the given party, at a location of its own.
        /// </summary>
        private static OCPIv2_1_1.Session ASession(String PartyId, String SessionId)

            => new (
                   CountryCode:  CountryCode.Parse("DE"),
                   PartyId:      Party_Id.   Parse(PartyId),
                   Id:           Session_Id.Parse(SessionId),
                   Start:        Timestamp.Now - TimeSpan.FromMinutes(5),
                   kWh:          WattHour.FromKWh(1.11M),
                   AuthId:       Auth_Id.Parse("1234"),
                   AuthMethod:   OCPIv2_1_1.AuthMethods.AUTH_REQUEST,
                   Location:     ALocation(PartyId, "LOC0104"),
                   Currency:     Currency.EUR,
                   Status:       OCPIv2_1_1.SessionStatusTypes.ACTIVE
               );

        /// <summary>
        /// A tariff of the given party.
        /// </summary>
        private static OCPIv2_1_1.Tariff ATariff(String PartyId, String TariffId)

            => new (
                   CountryCode:     CountryCode.Parse("DE"),
                   PartyId:         Party_Id.   Parse(PartyId),
                   Id:              Tariff_Id.  Parse(TariffId),
                   Currency:        Currency.EUR,
                   TariffElements:  [
                                        new OCPIv2_1_1.TariffElement(
                                            [
                                                OCPIv2_1_1.PriceComponent.ChargingTime(
                                                    2.5M,
                                                    TimeSpan.FromSeconds(300)
                                                )
                                            ]
                                        )
                                    ]
               );

        #endregion

    }

}
