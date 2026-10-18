/*
 * Copyright (c) 2015-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPI <https://github.com/OpenChargingCloud/WWCP_OCPI>
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#region Usings

using System.Globalization;

using NUnit.Framework;

using org.GraphDefined.Vanaheimr.Aegir;
using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using Newtonsoft.Json.Linq;

using cloud.charging.open.protocols.OCPI;
using cloud.charging.open.protocols.OCPIv2_3_0.POI3;
using cloud.charging.open.protocols.WWCP.POI.tests;

using POI = cloud.charging.open.protocols.WWCP.POI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_3_0.UnitTests.POI3
{

    /// <summary>
    /// The charging tariff offers of an operator as OCPI 2.3.0 tariffs taking the place of its charging tariffs while
    /// they apply: of the sessions they are for, within their regular openings, for the eMSPs of the e-mobility providers
    /// they name; and not imported back.
    /// </summary>
    [TestFixture]
    public class POI3OfferTests
    {

        #region Data

        private static readonly DateTimeOffset                      Created   = DateTimeOffset.Parse("2026-01-01T00:00:00Z");
        private static readonly DateTimeOffset                      T0        = DateTimeOffset.Parse("2026-10-12T12:00:00Z");

        private static readonly POI.ChargingStationOperator_Id      GEF       = POI.ChargingStationOperator_Id.Parse("DE*GEF");
        private static readonly POI.EMobilityProvider_Id            GDF       = POI.EMobilityProvider_Id.      Parse("DE-GDF");
        private static readonly POI.ChargingPool_Id                 P1        = POI.ChargingPool_Id.           Parse("DE*GEF*P1");
        private static readonly POI.ChargingStation_Id              S1        = POI.ChargingStation_Id.        Parse("DE*GEF*S1");
        private static readonly POI.EVSE_Id                         E1        = POI.EVSE_Id.                   Parse("DE*GEF*E1*1");
        private static readonly POI.EVSE_Id                         E2        = POI.EVSE_Id.                   Parse("DE*GEF*E1*2");
        private static readonly POI.ChargingTariff_Id               T1        = POI.ChargingTariff_Id.         Parse("DE*GEF*T1");
        private static readonly POI.ChargingTariff_Id               T2        = POI.ChargingTariff_Id.         Parse("DE*GEF*T2");
        private static readonly POI.ChargingTariff_Id               T3        = POI.ChargingTariff_Id.         Parse("DE*GEF*T3");
        private static readonly POI.ChargingStationGroup_Id         SG1       = POI.ChargingStationGroup_Id.   Parse("DE*GEF*SG1");

        private static readonly Party_Idv3                          Party     = Party_Idv3.From(CountryCode.Parse("DE"), Party_Id.Parse("GEF"));

        private String     directory  = default!;
        private CommonAPI  api        = default!;

        #endregion

        #region SetUp / TearDown

        [SetUp]
        public void SetUp()
        {
            directory  = Path.Combine(Path.GetTempPath(), $"WWCP_OCPI_POI3_OfferTests-{Guid.NewGuid():N}");
            Directory.CreateDirectory(directory);
            api        = new CommonAPI(
                             OurPartyData:      [ new PartyData(Party, Role.CPO, new BusinessDetails("GraphDefined CSO")) ],
                             DefaultPartyId:    Party,
                             BaseAPI:           new CommonHTTPAPI(
                                                    HTTPAPI:         new HTTPExtAPI(HTTPServer: new HTTPServer(TCPPort: IPPort.Parse(3995))),
                                                    OurBaseURL:      URL.Parse("http://127.0.0.1:3995/ocpi"),
                                                    OurVersionsURL:  URL.Parse("http://127.0.0.1:3995/ocpi/versions"),
                                                    RootPath:        HTTPPath.Parse("/ocpi"),
                                                    DisableLogging:  true,
                                                    LoggingPath:     directory
                                                ),
                             URLPathPrefix:     HTTPPath.Parse("/ocpi/v2.3.0"),
                             DatabaseFilePath:  directory,
                             DisableLogging:    true,
                             LoggingPath:       directory
                         );
        }

        [TearDown]
        public async Task TearDown()
        {

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

        #region (private static) Energy(Price) / Daily(From, To) / Offers / ANetwork(Offers)

        private static POI.ChargingTariffElement[] Energy(Decimal Price)

            => [ new POI.ChargingTariffElement([ new POI.ChargingPriceComponent(POI.ChargingDimensionTypes.ENERGY, Price, VAT: 19, StepSize: 1) ]) ];

        /// <summary>
        /// Every day from the given hour to the other one, local time; and what else is given.
        /// </summary>
        private static ImmutableOpeningTimes Daily(Byte From, Byte To, Action<OpeningTimes>? More = null)
        {

            var openingTimes = new OpeningTimes();

            foreach (var dayOfWeek in Enum.GetValues<DayOfWeek>())
                openingTimes.AddRegularOpening(dayOfWeek, new HourMin(From, 0), new HourMin(To, 0));

            More?.Invoke(openingTimes);

            return new ImmutableOpeningTimes(openingTimes);

        }

        /// <summary>
        /// 20 % less at night, from 18:00 to 06:00, at the station of the pool, for every session.
        /// </summary>
        private static POI.ChargingTariffOffer Night()

            => new (POI.ChargingTariffOffer_Id.Parse("gef-night"),
                    I18NString.Create(Languages.en, "Night discount"),
                    Discount:                 new POI.ChargingTariffDiscount(20),
                    ChargingStationGroupIds:  [ SG1 ],
                    OpeningTimes:             Daily(18, 6),
                    Created:                  Created);

        /// <summary>
        /// 29 ct/kWh by day, from 06:00 to 18:00, for the contracts of DE-GDF only.
        /// </summary>
        private static POI.ChargingTariffOffer DayForGDF()

            => new (POI.ChargingTariffOffer_Id.Parse("gef-day-gdf"),
                    ChargingTariff:              new POI.ChargingTariffOfferPrice(Currency.EUR, Energy(0.29m)),
                    ChargingStationOperatorIds:  [ GEF ],
                    OpeningTimes:                Daily(6, 18),
                    Sessions:                    POI.ChargingTariffOfferSessions.contract,
                    EMobilityProviderIds:        [ GDF ],
                    Created:                     Created);

        /// <summary>
        /// 25 ct/kWh at any time, for a day around T0, for every session; with the given opening times.
        /// </summary>
        private static POI.ChargingTariffOffer Always(ImmutableOpeningTimes? OpeningTimes = null)

            => new (POI.ChargingTariffOffer_Id.Parse("gef-always"),
                    I18NString.Create(Languages.en, "Always"),
                    ChargingTariff:              new POI.ChargingTariffOfferPrice(Currency.EUR, Energy(0.25m)),
                    ChargingStationOperatorIds:  [ GEF ],
                    NotBefore:                   T0.AddDays(-1),
                    NotAfter:                    T0.AddDays(1),
                    OpeningTimes:                OpeningTimes,
                    Created:                     Created);

        private static POI.EVSE AnEVSE(POI.EVSE_Id Id, params POI.ChargingTariff_Id[] TariffIds)

            => new (Id,
                    CurrentTypes:        POI.CurrentTypes.AC_ThreePhases,
                    MaxVoltage:          Volt.  FromV(400),
                    MaxCurrent:          Ampere.FromA(32),
                    MaxPower:            Watt.  FromW(22000),
                    ChargingConnectors:  [ new POI.ChargingConnector(POI.ChargingConnector_Id.Parse("1"), POI.ChargingConnectorType.IEC_62196_T2) ],
                    ChargingTariffIds:   TariffIds,
                    Created:             Created);

        /// <summary>
        /// DE*GEF with a tariff for every session, one for ad hoc sessions and one for contracts; a pool in Berlin with
        /// one station, its first EVSE taking the first tariff, its second the others; and the given offers.
        /// </summary>
        private static POI.RoamingNetwork ANetwork(params POI.ChargingTariffOffer[] Offers)

            => new POI.RoamingNetwork(POI.RoamingNetwork_Id.Parse("rn1"),
                                      Created:                   Created,
                                      ChargingStationOperators:  [ new POI.ChargingStationOperator(GEF,
                                                                                                   Name:                   I18NString.Create(Languages.en, "GraphDefined"),
                                                                                                   ChargingTariffs:        [ new POI.ChargingTariff(T1, Currency.EUR, Energy(0.39m), I18NString.Create(Languages.en, "Standard"), Created: Created),
                                                                                                                             new POI.ChargingTariff(T2, Currency.EUR, Energy(0.59m), TariffType: POI.ChargingTariffType.AD_HOC_PAYMENT, Created: Created),
                                                                                                                             new POI.ChargingTariff(T3, Currency.EUR, Energy(0.49m), TariffType: POI.ChargingTariffType.REGULAR,        Created: Created) ],
                                                                                                   ChargingPools:          [ new POI.ChargingPool(P1,
                                                                                                                                                  I18NString.Create(Languages.en, "Biberweg"),
                                                                                                                                                  Address:           new Address("Biberweg", "07749", I18NString.Create(Languages.de, "Jena"), Country.Germany, HouseNumber: "18"),
                                                                                                                                                  GeoLocation:       GeoCoordinate.Parse(50.92, 11.58),
                                                                                                                                                  TimeZone:          Time_Zone.Parse("Europe/Berlin"),
                                                                                                                                                  ChargingStations:  [ new POI.ChargingStation(S1, EVSEs: [ AnEVSE(E1, T1), AnEVSE(E2, T2, T3) ], Created: Created) ],
                                                                                                                                                  Created:           Created) ],
                                                                                                   ChargingStationGroups:  [ new POI.ChargingStationGroup(SG1, ChargingStationIds: [ S1 ], Created: Created) ],
                                                                                                   ChargingTariffOffers:   Offers,
                                                                                                   Created:                Created) ]).WithKeys();

        #endregion

        #region (private static) OfferedAt(Network, EVSE, At) / Describe(Tariff) / TariffOf(Offered, Id)

        private static OfferedTariffIds OfferedAt(POI.RoamingNetwork       Network,
                                                  POI.EVSE_Id              EVSEId,
                                                  DateTimeOffset           At,
                                                  params POI.ChargingTariff_Id[] TariffIds)

            => POI3Mapper.OfferedTariffIdsOf(EVSEId, TariffIds, [], Network.GetChargingStationOperatorById(GEF)!, At, []);

        private static String[] IdsOf(IEnumerable<Tariff_Id> TariffIds)

            => [ .. TariffIds.Select(tariffId => tariffId.ToString()) ];

        /// <summary>
        /// The elements of the given tariff, each as its prices, its times and its days.
        /// </summary>
        private static String[] Describe(Tariff Tariff)

            => [ .. Tariff.TariffElements.Select(element => String.Join(" ",
                                                     new[] { String.Join(",", element.PriceComponents.Select(priceComponent => $"{priceComponent.Type} {priceComponent.Price.ToString("0.###", CultureInfo.InvariantCulture)}")) }.
                                                     Concat(element.TariffRestrictions?.StartTime is Time startTime ? [ $"{startTime}-{element.TariffRestrictions.EndTime}" ] : []).
                                                     Concat(element.TariffRestrictions?.DayOfWeek.Any() == true      ? [ String.Join(",", element.TariffRestrictions.DayOfWeek) ] : []))) ];

        private static Tariff TariffOf(OfferedTariffIds  Offered,
                                       String            TariffId)

            => Offered.Tariffs.Single(tariff => tariff.Id.ToString() == TariffId).ToOCPI([])!;

        #endregion

        #region (private static) Eventually(Condition) / TariffIdsOf(Location) / TariffIdsOf(LocationJSON, EVSE)

        private static async Task<Boolean> Eventually(Func<Boolean> Condition)
        {

            for (var i = 0; i < 400; i++)
            {
                if (Condition())
                    return true;
                await Task.Delay(25);
            }

            return Condition();

        }

        private static String[] TariffIdsOf(Location Location)

            => [ .. Location.EVSEs.OrderBy(evse => evse.UId.ToString()).Select(evse => String.Join(",", evse.Connectors.SelectMany(connector => connector.TariffIds))) ];

        /// <summary>
        /// The tariffs the connector of the given EVSE names in the given JSON of a location, as an eMSP gets it.
        /// </summary>
        private static String[] TariffIdsOf(JObject      Location,
                                            POI.EVSE_Id  EVSEId)

            => [ .. Location["evses"]!.OfType<JObject>().Single(evse => evse["uid"]!.Value<String>() == EVSEId.ToString())["connectors"]![0]!["tariff_ids"]?.Values<String>().OfType<String>() ?? [] ];

        #endregion


        #region AnOfferTakesThePlaceOfTheTariffsOfTheSessionsItIsFor()

        /// <summary>
        /// The night discount for every session takes the place of the tariff for every session, of the tariff for ad
        /// hoc sessions and of the one for contracts: each a tariff of its own, its identification the tariff's and the
        /// offer's.
        /// </summary>
        [Test]
        public void AnOfferTakesThePlaceOfTheTariffsOfTheSessionsItIsFor()
        {

            var network  = ANetwork(Night());
            var first    = OfferedAt(network, E1, T0, T1);
            var second   = OfferedAt(network, E2, T0, T2, T3);

            Assert.Multiple(() => {

                Assert.That(IdsOf(first. TariffIds),                                        Is.EqualTo(new[] { "1+gef-night" }));
                Assert.That(IdsOf(second.TariffIds),                                        Is.EqualTo(new[] { "2+gef-night", "3+gef-night" }));
                Assert.That(first.ByEMSP,                                                   Is.False);

                // Its type is the tariff's.
                Assert.That(TariffOf(first,  "1+gef-night").TariffType,                     Is.Null);
                Assert.That(TariffOf(second, "2+gef-night").TariffType,                     Is.EqualTo(TariffType.AD_HOC_PAYMENT));
                Assert.That(TariffOf(second, "3+gef-night").TariffType,                     Is.EqualTo(TariffType.REGULAR));

                // The tariff and its charging tariff, also of one with new prices.
                Assert.That(Tariff_Id.Parse("1+gef-night").IsOffered(),                     Is.True);
                Assert.That(Tariff_Id.Parse("1~0123abcd").IsOffered(),                      Is.False);
                Assert.That(Tariff_Id.Parse("1~0123abcd+gef-night").HasPrices(),            Is.True);
                Assert.That(POI3Mapper.ToPOI3TariffIdOf(GEF, Tariff_Id.Parse("1+gef-night")),           Is.EqualTo(T1));
                Assert.That(POI3Mapper.ToPOI3TariffIdOf(GEF, Tariff_Id.Parse("1~0123abcd+gef-night")),  Is.EqualTo(T1));

            });

        }

        #endregion

        #region ADiscountWithinRegularOpeningsIsTheTariffReducedThenAndAsItIsOtherwise()

        /// <summary>
        /// The night discount: the elements of the tariff reduced by 20 %, from 18:00 to 06:00 local time, split at
        /// midnight as OCPI says it; the tariff as it is at the other times.
        /// </summary>
        [Test]
        public void ADiscountWithinRegularOpeningsIsTheTariffReducedThenAndAsItIsOtherwise()
        {

            var tariff = TariffOf(OfferedAt(ANetwork(Night()), E1, T0, T1), "1+gef-night");

            Assert.Multiple(() => {

                Assert.That(Describe(tariff),                                Is.EqualTo(new[] { "ENERGY 0.312 00:00-06:00",
                                                                                                "ENERGY 0.312 18:00-00:00",
                                                                                                "ENERGY 0.39" }));
                Assert.That(tariff.Currency,                                 Is.EqualTo(Currency.EUR));
                Assert.That(tariff.TariffAltText.Select(text => text.Text),  Is.EqualTo(new[] { "Night discount", "Standard" }));
                Assert.That(tariff.Start,                                    Is.Null);
                Assert.That(tariff.End,                                      Is.Null);

            });

        }

        #endregion

        #region AnOfferForTheContractsOfAProviderIsTheirsOnly()

        /// <summary>
        /// By day 29 ct/kWh for the contracts of DE-GDF: its eMSP gets other tariffs, those of the night discount and of
        /// this offer for its contracts beside the tariff for every session; of the tariff for contracts, one of both
        /// offers. Its price applies within its openings, priced 0 for every dimension the tariff prices else, so that
        /// none of the tariff's applies then.
        /// </summary>
        [Test]
        public void AnOfferForTheContractsOfAProviderIsTheirsOnly()
        {

            var network  = ANetwork(Night(), DayForGDF());
            var first    = OfferedAt(network, E1, T0, T1);
            var second   = OfferedAt(network, E2, T0, T2, T3);

            var gdf      = RemoteParty_Id.Parse("DE-GDF_EMSP");
            var other    = RemoteParty_Id.Parse("DE-XYZ_EMSP");

            Assert.Multiple(() => {

                // Every other eMSP: the night discount only.
                Assert.That(IdsOf(first.TariffIds),                          Is.EqualTo(new[] { "1+gef-night" }));
                Assert.That(IdsOf(first.For(other)),                         Is.EqualTo(new[] { "1+gef-night" }));
                Assert.That(IdsOf(first.For(null)),                          Is.EqualTo(new[] { "1+gef-night" }));

                // DE-GDF: the tariff, beside it the night discount for ad hoc sessions and both offers for contracts.
                Assert.That(first.ByEMSP,                                    Is.True);
                Assert.That(IdsOf(first.For(gdf)),                           Has.Length.EqualTo(3));
                Assert.That(IdsOf(first.For(gdf))[0],                        Is.EqualTo("1"));
                Assert.That(IdsOf(first.For(gdf))[1],                        Does.Match("^1\\+[0-9a-f]{8}$"));
                Assert.That(IdsOf(first.For(gdf))[2],                        Does.Match("^1\\+[0-9a-f]{8}$"));

                Assert.That(TariffOf(first, IdsOf(first.For(gdf))[1]).TariffType,  Is.EqualTo(TariffType.AD_HOC_PAYMENT));
                Assert.That(TariffOf(first, IdsOf(first.For(gdf))[2]).TariffType,  Is.EqualTo(TariffType.REGULAR));
                // Both offers, in the order of their identifications, then the tariff.
                Assert.That(Describe(TariffOf(first, IdsOf(first.For(gdf))[2])),   Is.EqualTo(new[] { "ENERGY 0.29 06:00-18:00",
                                                                                                      "ENERGY 0 06:00-18:00",
                                                                                                      "ENERGY 0.312 00:00-06:00",
                                                                                                      "ENERGY 0.312 18:00-00:00",
                                                                                                      "ENERGY 0.39" }));

                // The tariff for ad hoc sessions as for every eMSP; the one for contracts of both offers.
                Assert.That(IdsOf(second.For(gdf))[0],                       Is.EqualTo("2+gef-night"));
                Assert.That(IdsOf(second.For(gdf))[1],                       Does.Match("^3\\+[0-9a-f]{8}$"));

            });

        }

        #endregion

        #region APriceAtAnyTimeIsItsOwnWhileItIsActiveAndNotClosed()

        /// <summary>
        /// 25 ct/kWh at any time for a day: its price, currency and times, as long as it is active; not during an
        /// exceptional closing; and when its status changes next.
        /// </summary>
        [Test]
        public void APriceAtAnyTimeIsItsOwnWhileItIsActiveAndNotClosed()
        {

            var network  = ANetwork(Always());
            var offered  = OfferedAt(network, E1, T0, T1);
            var tariff   = TariffOf(offered, "1+gef-always");

            var closed   = ANetwork(Always(Daily(0, 0, openingTimes => openingTimes.AddExceptionalClosing(T0.AddHours(-1), T0.AddHours(1)))));

            Assert.Multiple(() => {

                Assert.That(IdsOf(offered.TariffIds),                                         Is.EqualTo(new[] { "1+gef-always" }));
                Assert.That(Describe(tariff),                                                 Is.EqualTo(new[] { "ENERGY 0.25" }));
                Assert.That(tariff.Start,                                                     Is.EqualTo(T0.AddDays(-1)));
                Assert.That(tariff.End,                                                       Is.EqualTo(T0.AddDays(1)));
                Assert.That(tariff.TariffAltText.Select(text => text.Text),                   Is.EqualTo(new[] { "Always", "Standard" }));

                // Planned, expired: the tariff.
                Assert.That(IdsOf(OfferedAt(network, E1, T0.AddDays(-2), T1).TariffIds),     Is.EqualTo(new[] { "1" }));
                Assert.That(IdsOf(OfferedAt(network, E1, T0.AddDays( 2), T1).TariffIds),     Is.EqualTo(new[] { "1" }));

                // During its exceptional closing the tariff, after it the offer.
                Assert.That(IdsOf(OfferedAt(closed,  E1, T0,              T1).TariffIds),     Is.EqualTo(new[] { "1" }));
                Assert.That(IdsOf(OfferedAt(closed,  E1, T0.AddHours(2),  T1).TariffIds),     Is.EqualTo(new[] { "1+gef-always" }));

                // Next: right after its notAfter; of the closed one, the end of its closing.
                Assert.That(POI3Mapper.NextOfferChange(network.GetChargingStationOperatorById(GEF)!, T0),  Is.EqualTo(T0.AddDays(1).AddTicks(1)));
                Assert.That(POI3Mapper.NextOfferChange(closed. GetChargingStationOperatorById(GEF)!, T0),  Is.EqualTo(T0.AddHours(1)));

            });

        }

        #endregion

        #region ThePublisherPublishesTheTariffsOfOffersWhileTheyApply()

        /// <summary>
        /// An offer for every session, ending two seconds from now: while it applies, the connectors name its tariffs,
        /// one of the tariff for every session, one of each of the others; after it, the tariffs again, and its tariffs
        /// are gone.
        /// </summary>
        [Test]
        public async Task ThePublisherPublishesTheTariffsOfOffersWhileTheyApply()
        {

            var now      = Timestamp.Now;
            var flash    = new POI.ChargingTariffOffer(POI.ChargingTariffOffer_Id.Parse("gef-flash"),
                                                       ChargingTariff:              new POI.ChargingTariffOfferPrice(Currency.EUR, Energy(0.25m)),
                                                       ChargingStationOperatorIds:  [ GEF ],
                                                       NotBefore:                   now.AddMinutes(-1),
                                                       NotAfter:                    now.AddSeconds(2),
                                                       Created:                     Created);

            var history  = POI.RoamingNetworkHistory.Start(ANetwork(flash));
            var errors   = new List<String>();

            await using var publisher = new POI3ToOCPIPublisher(history, api);
            publisher.OnError += (sender, description, warnings) => { lock (errors) errors.Add(description); };

            await publisher.StartAsync();

            Assert.Multiple(() => {
                Assert.That(api.TryGetLocation(Party, Location_Id.Parse("1"), out var location) ? TariffIdsOf(location) : null,  Is.EqualTo(new[] { "1+gef-flash", "2+gef-flash,3+gef-flash" }));
                Assert.That(api.TryGetTariff(Party, Tariff_Id.Parse("1+gef-flash"), out var tariff) ? Describe(tariff) : null,      Is.EqualTo(new[] { "ENERGY 0.25" }));
                Assert.That(api.TryGetTariff(Party, Tariff_Id.Parse("1"),           out var regular) ? Describe(regular) : null,    Is.EqualTo(new[] { "ENERGY 0.39" }));
            });

            Assert.That(await Eventually(() => api.TryGetLocation(Party, Location_Id.Parse("1"), out var location) &&
                                               TariffIdsOf(location).SequenceEqual([ "1", "2,3" ]) &&
                                               !api.TryGetTariff(Party, Tariff_Id.Parse("1+gef-flash"), out _)),
                        Is.True, "The tariffs were not published again after the offer.");

            lock (errors)
                Assert.That(errors, Is.Empty);

        }

        #endregion

        #region TheTariffsOfAnOfferForTheContractsOfAProviderAreToldToItsEMSPOnly()

        /// <summary>
        /// 29 ct/kWh for the contracts of DE-GDF: its eMSP gets the location with the tariffs of the offer, every other
        /// eMSP with the charging tariffs; the tariffs of the offer are stored.
        /// </summary>
        [Test]
        public async Task TheTariffsOfAnOfferForTheContractsOfAProviderAreToldToItsEMSPOnly()
        {

            var forGDF   = new POI.ChargingTariffOffer(POI.ChargingTariffOffer_Id.Parse("gef-gdf"),
                                                       ChargingTariff:              new POI.ChargingTariffOfferPrice(Currency.EUR, Energy(0.29m)),
                                                       ChargingStationOperatorIds:  [ GEF ],
                                                       Sessions:                    POI.ChargingTariffOfferSessions.contract,
                                                       EMobilityProviderIds:        [ GDF ],
                                                       Created:                     Created);

            var history  = POI.RoamingNetworkHistory.Start(ANetwork(forGDF));

            await using var publisher = new POI3ToOCPIPublisher(history, api);
            await publisher.StartAsync();

            Assert.That(api.TryGetLocation(Party, Location_Id.Parse("1"), out var location), Is.True);

            var ofGDF    = location!.ToJSON(RemoteParty_Id.Parse("DE-GDF_EMSP"));
            var ofOther  = location. ToJSON(RemoteParty_Id.Parse("DE-XYZ_EMSP"));

            Assert.Multiple(() => {

                // The tariff for every session, beside it one of the offer for contracts; of the tariff for contracts, the offer's.
                Assert.That(TariffIdsOf(ofGDF,   E1)[0],                                   Is.EqualTo("1"));
                Assert.That(TariffIdsOf(ofGDF,   E1)[1],                                   Does.Match("^1\\+[0-9a-f]{8}$"));
                Assert.That(TariffIdsOf(ofGDF,   E2),                                      Is.EqualTo(new[] { "2", "3+gef-gdf" }));

                Assert.That(TariffIdsOf(ofOther, E1),                                      Is.EqualTo(new[] { "1" }));
                Assert.That(TariffIdsOf(ofOther, E2),                                      Is.EqualTo(new[] { "2", "3" }));

                Assert.That(api.TryGetTariff(Party, Tariff_Id.Parse("3+gef-gdf"), out _),  Is.True);
                Assert.That(api.TryGetTariff(Party, Tariff_Id.Parse(TariffIdsOf(ofGDF, E1)[1]), out var offered) ? offered.TariffType : null,
                            Is.EqualTo(TariffType.REGULAR));

            });

        }

        #endregion

        #region TheImporterTakesNoTariffOfOffers()

        /// <summary>
        /// A tariff charging tariff offers make, pushed by a CPO, is no charging tariff: nothing is committed.
        /// </summary>
        [Test]
        public async Task TheImporterTakesNoTariffOfOffers()
        {

            var network   = ANetwork(Night());
            var history   = POI.RoamingNetworkHistory.Start(network);
            var revision  = history.Head.Commit.Revision;
            var errors    = new List<String>();

            await using var importer = new OCPIToPOI3Importer(api, history, changeSet => changeSet.SignedByAll(history.Head.RoamingNetwork));
            importer.OnError += (sender, description, warnings) => { lock (errors) errors.Add(description); };
            importer.Start();

            var tariff    = TariffOf(OfferedAt(network, E1, T0, T1), "1+gef-night");

            Assert.That((await api.AddOrUpdateTariff(tariff)).IsSuccess);
            await importer.ImportAsync(tariff);

            Assert.Multiple(() => {
                Assert.That(history.Head.Commit.Revision,                                                      Is.EqualTo(revision));
                Assert.That(history.Head.RoamingNetwork.GetChargingStationOperatorById(GEF)!.ChargingTariffs.Count(),  Is.EqualTo(3));
                lock (errors)
                    Assert.That(errors,                                                                        Is.Empty);
            });

        }

        #endregion

    }

}
