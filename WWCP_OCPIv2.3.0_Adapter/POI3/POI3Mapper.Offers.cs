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

using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPI;

using POI = cloud.charging.open.protocols.WWCP.POI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_3_0.POI3
{

    /// <summary>
    /// A tariff the charging tariff offers of an operator make of one of its charging tariffs at an EVSE, taking its place
    /// while they apply (see POI3Mapper.OfferedTariffIdsOf): of an offer with a price of its own, its elements; of a
    /// discount, the elements of the charging tariff with the reduced prices. Where the offers apply within their regular
    /// openings, their elements are restricted to these, local times of the location, and the elements of the charging
    /// tariff follow for the other times.
    /// </summary>
    public sealed class OfferedTariff
    {

        #region Properties

        /// <summary>
        /// The charging tariff whose place it takes, as it is at runtime.
        /// </summary>
        public POI.ChargingTariff                         ChargingTariff       { get; }

        /// <summary>
        /// The new prices of the charging tariff in effect at the EVSE, if any, with since when.
        /// </summary>
        public Timestamped<POI.ChargingTariffPrices>?     Prices               { get; }

        /// <summary>
        /// The offers, in the order of their identifications.
        /// </summary>
        public IReadOnlyList<POI.ChargingTariffOffer>     Offers               { get; }

        /// <summary>
        /// Whether the offers apply within their regular openings only, else at any time: without opening times, open
        /// 24 hours or during an exceptional opening.
        /// </summary>
        public Boolean                                    ByRegularOpenings    { get; }

        /// <summary>
        /// The type of the tariff: the one of the charging tariff, or of the sessions the offers are for, where the
        /// charging tariff is for both.
        /// </summary>
        public TariffType?                                TariffType           { get; }

        /// <summary>
        /// Its identification: the one of the charging tariff, '+' and the offer, else the first 8 hex digits of the
        /// SHA-256 of the offers and the type (see POI3Mapper.OfferSeparator).
        /// </summary>
        public Tariff_Id?                                 Id                   { get; }

        #endregion

        #region Constructor(s)

        internal OfferedTariff(POI.ChargingTariff                       ChargingTariff,
                               Timestamped<POI.ChargingTariffPrices>?   Prices,
                               IReadOnlyList<POI.ChargingTariffOffer>   Offers,
                               Boolean                                  ByRegularOpenings,
                               TariffType?                              TariffType)
        {

            this.ChargingTariff     = ChargingTariff;
            this.Prices             = Prices;
            this.Offers             = Offers;
            this.ByRegularOpenings  = ByRegularOpenings;
            this.TariffType         = TariffType;

            var regularId           = ChargingTariff.Id.ToOCPITariffId(Prices?.Value);
            var ownType             = ChargingTariff.TariffType.HasValue ? OCPI.TariffType.Parse(ChargingTariff.TariffType.Value.ToString()) : (TariffType?) null;

            if (regularId.HasValue)
            {

                var readable = Offers.Count == 1 && TariffType == ownType ? Offers[0].Id.ToString() : null;

                this.Id = Tariff_Id.TryParse(readable is not null &&
                                             ReadableOffer.IsMatch(readable) &&
                                             regularId.Value.ToString().Length + 1 + readable.Length <= MaxTariffIdLength
                                                 ? $"{regularId}{POI3Mapper.OfferSeparator}{readable}"
                                                 : $"{regularId}{POI3Mapper.OfferSeparator}{OffersHash(Offers, TariffType)}");

            }

        }

        #endregion


        #region ToOCPI(Warnings, NotBefore = null)

        /// <summary>
        /// The OCPI tariff, or null when it can not be one, e.g. of an offer with a price in another currency than the
        /// charging tariff's, applying within its regular openings only.
        /// </summary>
        /// <param name="Warnings">Where to tell what could not be mapped.</param>
        /// <param name="NotBefore">The earliest 'last updated' of the tariff, e.g. after the one published before.</param>
        public Tariff? ToOCPI(ICollection<Warning>  Warnings,
                              DateTimeOffset?       NotBefore   = null)
        {

            if (!Id.HasValue)
            {
                Warnings.Add(Warning.Create($"The charging tariff identification '{ChargingTariff.Id}' is no OCPI tariff identification!"));
                return null;
            }

            var of            = $"charging tariff '{ChargingTariff.Id}' with the charging tariff offers '{String.Join("', '", Offers.Select(offer => offer.Id))}'";
            var baseElements  = Prices?.Value.Elements ?? ChargingTariff.Elements;
            var currency      = ChargingTariff.Currency;
            var minPrice      = ChargingTariff.MinPrice;
            var maxPrice      = ChargingTariff.MaxPrice;
            var tariffURL     = ChargingTariff.TariffURL;
            var energyMix     = ChargingTariff.EnergyMix;
            var elements      = new List<POI.ChargingTariffElement>();

            if (!ByRegularOpenings)
            {

                // At any time: the price of the offer, or the charging tariff's reduced.
                var offer = Offers[0];

                if (offer.ChargingTariff is POI.ChargingTariffOfferPrice price)
                {
                    currency   = price.Currency;
                    minPrice   = price.MinPrice;
                    maxPrice   = price.MaxPrice;
                    tariffURL  = price.TariffURL ?? tariffURL;
                    energyMix  = price.EnergyMix ?? energyMix;
                    elements.AddRange(price.Elements);
                }

                else if (offer.Discount is POI.ChargingTariffDiscount discount)
                {

                    elements.AddRange(POI3Mapper.Reduced(baseElements, discount));

                    // Without dimensions, its minimum and maximum price as well.
                    if (discount.Dimensions.IsDefaultOrEmpty)
                    {
                        minPrice = POI3Mapper.Reduced(minPrice, discount.Percent);
                        maxPrice = POI3Mapper.Reduced(maxPrice, discount.Percent);
                    }

                }

            }

            else
            {

                // The dimensions the charging tariff prices: priced 0 within the openings of an offer with a price of its
                // own where its elements price them not, so that none of the charging tariff's applies then.
                var dimensions = baseElements.SelectMany(element => element.PriceComponents.Select(priceComponent => priceComponent.Type)).Distinct().ToArray();

                foreach (var offer in Offers)
                {

                    var periods = POI3Mapper.RegularPeriodsOf(offer.OpeningTimes!);

                    if (offer.ChargingTariff is POI.ChargingTariffOfferPrice price)
                    {

                        if (price.Currency != ChargingTariff.Currency)
                        {
                            Warnings.Add(Warning.Create($"The price of the charging tariff offer '{offer.Id}' is in another currency than the charging tariff '{ChargingTariff.Id}', whose elements apply outside its opening times: it is left out!"));
                            continue;
                        }

                        foreach (var period in periods)
                            foreach (var element in price.Elements)
                                elements.AddRange(POI3Mapper.Restricted(element, period));

                        if (dimensions.Length > 0)
                            foreach (var period in periods)
                                elements.AddRange(POI3Mapper.Restricted(new POI.ChargingTariffElement(dimensions.Select(dimension => new POI.ChargingPriceComponent(dimension,
                                                                                                                                                                       0,
                                                                                                                                                                       StepSize: dimension == POI.ChargingDimensionTypes.FLAT ? null : 1))),
                                                                        period));

                    }

                    else if (offer.Discount is POI.ChargingTariffDiscount discount)
                        foreach (var period in periods)
                            foreach (var element in POI3Mapper.Reduced(baseElements, discount))
                                elements.AddRange(POI3Mapper.Restricted(element, period));

                }

                // Outside the openings: the charging tariff.
                elements.AddRange(baseElements);

            }

            var notBefores  = Offers.Where(offer => offer.NotBefore.HasValue).Select(offer => offer.NotBefore!.Value).ToArray();
            var notAfters   = Offers.Where(offer => offer.NotAfter. HasValue).Select(offer => offer.NotAfter! .Value).ToArray();

            return POI3Mapper.TariffOf(ChargingTariff.Id.OperatorId,
                                       Id.Value,
                                       of,
                                       currency,
                                       elements,
                                       TariffType,
                                       Offers.SelectMany(offer => POI3Mapper.ToDisplayTexts(offer.Name).Concat(POI3Mapper.ToDisplayTexts(offer.Description))).
                                              Concat(POI3Mapper.ToDisplayTexts(ChargingTariff.Name)).
                                              Concat(POI3Mapper.ToDisplayTexts(ChargingTariff.Description)),
                                       tariffURL,
                                       minPrice,
                                       maxPrice,
                                       notBefores.Length > 0 ? notBefores.Max() : null,
                                       notAfters. Length > 0 ? notAfters. Min() : null,
                                       energyMix,
                                       ChargingTariff.Created,
                                       new[] { ChargingTariff.LastChangeDate, Prices?.Timestamp ?? DateTimeOffset.MinValue, NotBefore ?? DateTimeOffset.MinValue }.
                                           Concat(Offers.Select(offer => offer.LastChangeDate)).
                                           Max(),
                                       Warnings);

        }

        #endregion


        #region (private) Data

        /// <summary>
        /// The most characters of an OCPI tariff identification.
        /// </summary>
        private const Int32 MaxTariffIdLength = 36;

        /// <summary>
        /// An offer identification its tariff identification tells as it is.
        /// </summary>
        private static readonly Regex ReadableOffer = new ("^[A-Za-z0-9_.-]+$");

        private static String OffersHash(IEnumerable<POI.ChargingTariffOffer>  Offers,
                                         TariffType?                           TariffType)

            => Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(String.Join("\n", Offers.Select(offer => offer.Id.ToString())) + "|" + TariffType)))[..8];

        #endregion

    }


    /// <summary>
    /// The tariffs the connectors of an EVSE name while charging tariff offers apply: for every eMSP, and for each of the
    /// e-mobility providers the offers are for, where theirs are others; and the tariffs the offers make.
    /// </summary>
    /// <param name="TariffIds">The tariffs for every eMSP.</param>
    /// <param name="ByProvider">The tariffs for the e-mobility providers whose are others.</param>
    /// <param name="Tariffs">The tariffs the offers make, which some of them name.</param>
    /// <param name="Since">Since when the offers apply as they do, as far as their times and changes tell.</param>
    public sealed record OfferedTariffIds(IReadOnlyList<Tariff_Id>                                                 TariffIds,
                                          IReadOnlyDictionary<POI.EMobilityProvider_Id, IReadOnlyList<Tariff_Id>>  ByProvider,
                                          IReadOnlyList<OfferedTariff>                                             Tariffs,
                                          DateTimeOffset?                                                          Since)
    {

        /// <summary>
        /// Whether the eMSPs of the e-mobility providers some of the offers are for get other tariffs.
        /// </summary>
        public Boolean ByEMSP
            => ByProvider.Count > 0;

        /// <summary>
        /// The tariffs for the given eMSP: of the e-mobility provider it is, where its tariffs are others.
        /// </summary>
        public IReadOnlyList<Tariff_Id> For(RemoteParty_Id? EMSP)

            => EMSP is RemoteParty_Id emsp &&
               POI.EMobilityProvider_Id.TryParse($"{emsp.CountryCode}-{emsp.PartyId}", out var eMobilityProviderId) &&
               ByProvider.TryGetValue(eMobilityProviderId, out var tariffIds)
                   ? tariffIds
                   : TariffIds;

        /// <summary>
        /// Whether the given ones tell the same tariffs, for every eMSP and for each of the e-mobility providers.
        /// </summary>
        public Boolean SameAs(OfferedTariffIds? Other)

            => Other is not null &&
               TariffIds.SequenceEqual(Other.TariffIds) &&
               ByProvider.Count == Other.ByProvider.Count &&
               ByProvider.All(byProvider => Other.ByProvider.TryGetValue(byProvider.Key, out var other) && byProvider.Value.SequenceEqual(other));

    }


    /// <summary>
    /// What the charging tariff offers of an operator make of the tariffs of the connectors at a time, as the mapping of
    /// its charging pools finds it (POI3Mapper.ToOCPI(ChargingPool, …, Offers)): by the EVSE and the connector, and the
    /// tariffs they make.
    /// </summary>
    public sealed class OfferedTariffsAt
    {

        private readonly Dictionary<(String EVSE, String Connector), OfferedTariffIds> connectors = [];

        /// <summary>
        /// The time.
        /// </summary>
        public DateTimeOffset  At    { get; }

        /// <summary>
        /// What the offers make of the tariffs of the connectors at the given time.
        /// </summary>
        public OfferedTariffsAt(DateTimeOffset At)
        {
            this.At = At;
        }

        /// <summary>
        /// What the offers make of the tariffs of the connectors, by their EVSE and connector.
        /// </summary>
        public IReadOnlyDictionary<(String EVSE, String Connector), OfferedTariffIds>  Connectors
            => connectors;

        /// <summary>
        /// The tariffs the offers make, each once, in the order of their identifications.
        /// </summary>
        public IEnumerable<OfferedTariff> Tariffs

            => connectors.Values.SelectMany(offered => offered.Tariffs).
                                 DistinctBy(tariff  => tariff.Id.ToString()).
                                 OrderBy   (tariff  => tariff.Id.ToString(), StringComparer.Ordinal);

        /// <summary>
        /// Remember what the offers make of the tariffs of the given connector of the given EVSE.
        /// </summary>
        public void Add(String            EVSE,
                        String            Connector,
                        OfferedTariffIds  Offered)
        {
            connectors[(EVSE, Connector)] = Offered;
        }

    }


    public static partial class POI3Mapper
    {

        #region OfferSeparator / IsOffered(TariffId)

        /// <summary>
        /// Between the tariff of a charging tariff and the charging tariff offers taking its place: no charging tariff
        /// identification has it.
        /// </summary>
        public const Char OfferSeparator = '+';

        /// <summary>
        /// Whether the given tariff is one charging tariff offers make of a charging tariff (see OfferedTariff).
        /// </summary>
        public static Boolean IsOffered(this Tariff_Id TariffId)

            => TariffId.ToString().IndexOf(OfferSeparator) > 0;

        #endregion

        #region OfferedTariffIdsOf(EVSEId, ChargingTariffIds, Prices, Operator, At, Warnings)

        /// <summary>
        /// The tariffs a connector of the given EVSE taking the given charging tariffs, with the given new prices in effect
        /// at it, names at the given time, with the charging tariff offers of its operator covering the EVSE taking the
        /// place of its charging tariffs where they apply: active then, without a maximum of sessions per contract, and
        /// not during an exceptional closing.
        /// </summary>
        /// <remarks>
        /// A charging tariff of the type AD_HOC_PAYMENT is for ad hoc sessions, one of another type for sessions with a
        /// contract, one without a type for both. Where offers apply to all the sessions of a charging tariff, a tariff of
        /// theirs takes its place; to only the ad hoc sessions, or only those with a contract, of one without a type, a
        /// tariff of theirs of the type AD_HOC_PAYMENT or REGULAR comes beside it. The contracts an offer is for are those
        /// of the e-mobility providers it names, else of all: the eMSPs of the providers it names get other tariffs.
        /// </remarks>
        public static OfferedTariffIds OfferedTariffIdsOf(POI.EVSE_Id                                           EVSEId,
                                                          IEnumerable<POI.ChargingTariff_Id>                    ChargingTariffIds,
                                                          IReadOnlyList<Timestamped<POI.ChargingTariffPrices>>  Prices,
                                                          POI.ChargingStationOperator                           Operator,
                                                          DateTimeOffset                                        At,
                                                          ICollection<Warning>                                  Warnings)
        {

            Timestamped<POI.ChargingTariffPrices>? PricesOf(POI.ChargingTariff_Id ChargingTariffId)
                => Prices.Where (price => price.Value.ChargingTariffId == ChargingTariffId).
                          Select(price => (Timestamped<POI.ChargingTariffPrices>?) price).
                          FirstOrDefault();

            var offers   = Operator.OffersCovering(EVSEId).
                               Where (offer => offer.MaxSessionsPerContract is null &&
                                               offer.StatusAt(At) == POI.ChargingTariffOfferStatusType.Active).
                               Select(offer => (Offer: offer, Openness: OpennessOf(offer, At))).
                               Where (offer => offer.Openness != OfferOpenness.Closed).
                               ToArray();

            if (offers.Length == 0)
                return new OfferedTariffIds([ .. ChargingTariffIds.Select(tariffId => tariffId.ToOCPITariffId(PricesOf(tariffId)?.Value)).
                                                                   Where (tariffId => tariffId.HasValue).
                                                                   Select(tariffId => tariffId!.Value) ],
                                            new Dictionary<POI.EMobilityProvider_Id, IReadOnlyList<Tariff_Id>>(),
                                            [],
                                            null);

            // The charging tariffs it takes, as they are at runtime, with the new prices in effect at it.
            var regular  = ChargingTariffIds.
                               Select(tariffId => Operator.TryGetEffectiveChargingTariff(tariffId, out var chargingTariff)
                                                      ? (ChargingTariff: chargingTariff, Prices: PricesOf(tariffId))
                                                      : (null, null)).
                               Where (tariff => tariff.ChargingTariff is not null).
                               ToArray();

            var tariffs  = new Dictionary<String, OfferedTariff>();

            IReadOnlyList<Tariff_Id> TariffIdsFor(POI.EMobilityProvider_Id? Provider)
            {

                var tariffIds  = new List<Tariff_Id>();
                var adHoc      = offers.Where(offer => offer.Offer.Sessions is null or POI.ChargingTariffOfferSessions.adHoc).ToArray();
                var contract   = offers.Where(offer => offer.Offer.Sessions is null or POI.ChargingTariffOfferSessions.contract &&
                                                       (offer.Offer.EMobilityProviderIds.IsDefaultOrEmpty ||
                                                        (Provider.HasValue && offer.Offer.EMobilityProviderIds.Contains(Provider.Value)))).ToArray();

                foreach (var (chargingTariff, tariffPrices) in regular)
                {

                    if (chargingTariff!.Id.ToOCPITariffId(tariffPrices?.Value) is not Tariff_Id regularId)
                        continue;

                    Tariff_Id? Offered((POI.ChargingTariffOffer Offer, OfferOpenness Openness)[] Of, TariffType? Type)
                    {

                        if (Of.Length == 0)
                            return null;

                        var open     = Of.Where(offer => offer.Openness == OfferOpenness.Open).Take(1).ToArray();
                        var offered  = new OfferedTariff(chargingTariff, tariffPrices, [ .. (open.Length > 0 ? open : Of).Select(offer => offer.Offer) ], open.Length == 0, Type);

                        if (offered.Id is not Tariff_Id offeredId || offered.ToOCPI(Warnings) is null)
                            return null;

                        tariffs.TryAdd(offeredId.ToString(), offered);
                        return offeredId;

                    }

                    switch (chargingTariff.TariffType)
                    {

                        case POI.ChargingTariffType.AD_HOC_PAYMENT:
                            tariffIds.Add(Offered(adHoc,    TariffType.AD_HOC_PAYMENT) ?? regularId);
                            break;

                        case POI.ChargingTariffType type:
                            tariffIds.Add(Offered(contract, TariffType.Parse(type.ToString())) ?? regularId);
                            break;

                        // For both: one tariff of the offers for both alike, else beside it one for each.
                        default:

                            if (adHoc.Length > 0 && adHoc.SequenceEqual(contract) && Offered(adHoc, null) is Tariff_Id forBoth)
                                tariffIds.Add(forBoth);

                            else
                            {

                                tariffIds.Add(regularId);

                                if (Offered(adHoc, TariffType.AD_HOC_PAYMENT) is Tariff_Id forAdHoc)
                                    tariffIds.Add(forAdHoc);

                                if (Offered(contract, TariffType.REGULAR) is Tariff_Id forContracts)
                                    tariffIds.Add(forContracts);

                            }

                            break;

                    }

                }

                return [ .. tariffIds.Distinct() ];

            }

            var forAll      = TariffIdsFor(null);
            var byProvider  = new Dictionary<POI.EMobilityProvider_Id, IReadOnlyList<Tariff_Id>>();

            foreach (var provider in offers.SelectMany(offer => offer.Offer.EMobilityProviderIds).Distinct())
                if (TariffIdsFor(provider) is var forProvider && !forProvider.SequenceEqual(forAll))
                    byProvider[provider] = forProvider;

            return new OfferedTariffIds(forAll,
                                        byProvider,
                                        [ .. tariffs.Values.OrderBy(tariff => tariff.Id.ToString(), StringComparer.Ordinal) ],
                                        offers.Select(offer => new[] { offer.Offer.LastChangeDate, offer.Offer.NotBefore ?? offer.Offer.Created }.Max()).Max());

        }

        #endregion

        #region NextOfferChange(Operator, At)

        /// <summary>
        /// When the tariffs the charging tariff offers of the given operator make, or where they apply, change next after
        /// the given time by their times: at the notBefore of an offer, right after its notAfter, and at the beginning
        /// and the end of an exceptional opening or closing. Null when none does any more.
        /// </summary>
        public static DateTimeOffset? NextOfferChange(POI.ChargingStationOperator  Operator,
                                                      DateTimeOffset               At)

            => Operator.ChargingTariffOffers.
                   Where     (offer => offer.MaxSessionsPerContract is null).
                   SelectMany(offer => new[] {
                                           offer.NotBefore,
                                           offer.NotAfter.HasValue && offer.NotAfter.Value < DateTimeOffset.MaxValue ? offer.NotAfter.Value.AddTicks(1) : null
                                       }.
                                       Concat((offer.OpeningTimes?.ExceptionalOpenings ?? []).
                                                  Concat(offer.OpeningTimes?.ExceptionalClosings ?? []).
                                                  SelectMany(period => new DateTimeOffset?[] { period.Begin, period.End }))).
                   Where     (timestamp => timestamp > At).
                   Min();

        #endregion


        #region (private) OfferOpenness / OpennessOf(Offer, At)

        /// <summary>
        /// Whether an offer is closed at a time by an exceptional closing, open by an exceptional opening or at any time,
        /// else open by its regular openings.
        /// </summary>
        private enum OfferOpenness
        {
            Closed,
            Open,
            Regularly
        }

        private static OfferOpenness OpennessOf(POI.ChargingTariffOffer  Offer,
                                                DateTimeOffset           At)
        {

            if (Offer.OpeningTimes is not ImmutableOpeningTimes openingTimes)
                return OfferOpenness.Open;

            if (openingTimes.ExceptionalClosings.Any(period => period.Begin <= At && At < period.End))
                return OfferOpenness.Closed;

            if (openingTimes.IsOpen24Hours ||
                openingTimes.ExceptionalOpenings.Any(period => period.Begin <= At && At < period.End))
            {
                return OfferOpenness.Open;
            }

            return OfferOpenness.Regularly;

        }

        #endregion

        #region (internal) RegularPeriodsOf(OpeningTimes)

        /// <summary>
        /// A period of the day, from and to minutes since midnight, on the given days.
        /// </summary>
        internal sealed record DayPeriod(Int32                       From,
                                         Int32                       To,
                                         IReadOnlyList<DayOfWeek>    Days);

        /// <summary>
        /// The regular openings of the given opening times as periods of the day, each with the days it is on: a period
        /// ending at midnight ends at the end of its day, one ending before it begins the next day.
        /// </summary>
        internal static IReadOnlyList<DayPeriod> RegularPeriodsOf(ImmutableOpeningTimes OpeningTimes)
        {

            var periods = new List<(Int32 From, Int32 To, DayOfWeek Day)>();

            foreach (var (dayOfWeek, regularHours) in OpeningTimes.RegularOpenings)
                foreach (var hours in regularHours)
                {

                    var from  = hours.PeriodBegin.Hour * 60 + hours.PeriodBegin.Minute;
                    var to    = hours.PeriodEnd.  Hour * 60 + hours.PeriodEnd.  Minute;

                    if (to <= from)
                        to += 1440;

                    periods.Add((from, Math.Min(to, 1440), dayOfWeek));

                    if (to > 1440)
                        periods.Add((0, to - 1440, (DayOfWeek) (((Int32) dayOfWeek + 1) % 7)));

                }

            return [ .. periods.GroupBy (period => (period.From, period.To)).
                                OrderBy (period => period.Key.From).
                                ThenBy  (period => period.Key.To).
                                Select  (period => new DayPeriod(period.Key.From,
                                                                 period.Key.To,
                                                                 [ .. period.Select(day => day.Day).Distinct().OrderBy(day => ((Int32) day + 6) % 7) ])) ];

        }

        #endregion

        #region (internal) Restricted(Element, Period)

        /// <summary>
        /// The given charging tariff element within the given period of the day on its days: its restrictions of times
        /// and days intersected with them, each part of the day an element of its own; none, where they have nothing in
        /// common. An end at midnight is 00:00, as OCPI says it.
        /// </summary>
        internal static IEnumerable<POI.ChargingTariffElement> Restricted(POI.ChargingTariffElement  Element,
                                                                          DayPeriod                  Period)
        {

            var restrictions  = Element.Restrictions;

            var days          = restrictions is not null && !restrictions.DaysOfWeek.IsDefaultOrEmpty
                                    ? [ .. Period.Days.Intersect(restrictions.DaysOfWeek) ]
                                    : Period.Days;

            if (days.Count == 0)
                yield break;

            // The times of the element: one ending before it begins wraps around to the next day.
            var from          = restrictions?.StartTime is TimeOnly startTime ? startTime.Hour * 60 + startTime.Minute : 0;
            var to            = restrictions?.EndTime   is TimeOnly endTime   ? endTime.  Hour * 60 + endTime.  Minute : 1440;

            if (to == 0)
                to = 1440;

            (Int32 From, Int32 To)[] times = to > from
                                                 ? [ (from, to) ]
                                                 : [ (from, 1440), (0, to) ];

            foreach (var (timeFrom, timeTo) in times)
            {

                var begin  = Math.Max(timeFrom, Period.From);
                var end    = Math.Min(timeTo,   Period.To);

                if (begin >= end)
                    continue;

                var wholeDay = begin == 0 && end == 1440;

                yield return new POI.ChargingTariffElement(
                                 Element.PriceComponents,
                                 new POI.ChargingTariffRestrictions(
                                     StartTime:    wholeDay ? null : new TimeOnly(begin / 60, begin % 60),
                                     EndTime:      wholeDay ? null : end == 1440 ? new TimeOnly(0, 0) : new TimeOnly(end / 60, end % 60),
                                     StartDate:    restrictions?.StartDate,
                                     EndDate:      restrictions?.EndDate,
                                     MinEnergy:    restrictions?.MinEnergy,
                                     MaxEnergy:    restrictions?.MaxEnergy,
                                     MinCurrent:   restrictions?.MinCurrent,
                                     MaxCurrent:   restrictions?.MaxCurrent,
                                     MinPower:     restrictions?.MinPower,
                                     MaxPower:     restrictions?.MaxPower,
                                     MinDuration:  restrictions?.MinDuration,
                                     MaxDuration:  restrictions?.MaxDuration,
                                     DaysOfWeek:   days.Count == 7 ? null : days,
                                     Reservation:  restrictions?.Reservation
                                 ));

            }

        }

        #endregion

        #region (internal) Reduced(Elements, Discount) / Reduced(PriceLimit, Percent)

        /// <summary>
        /// The given charging tariff elements with the prices of the dimensions the discount names, or of all, reduced
        /// by its percentage, rounded to six decimal places.
        /// </summary>
        internal static IEnumerable<POI.ChargingTariffElement> Reduced(IEnumerable<POI.ChargingTariffElement>  Elements,
                                                                       POI.ChargingTariffDiscount              Discount)

            => Elements.Select(element => new POI.ChargingTariffElement(
                                              element.PriceComponents.Select(priceComponent => Discount.Dimensions.IsDefaultOrEmpty || Discount.Dimensions.Contains(priceComponent.Type)
                                                                                                   ? priceComponent with { Price = Reduced(priceComponent.Price, Discount.Percent) }
                                                                                                   : priceComponent),
                                              element.Restrictions));

        /// <summary>
        /// The given price limit reduced by the given percentage, both its prices.
        /// </summary>
        internal static POI.ChargingPriceLimit? Reduced(POI.ChargingPriceLimit?  PriceLimit,
                                                        Decimal                  Percent)

            => PriceLimit is null
                   ? null
                   : new POI.ChargingPriceLimit(Reduced(PriceLimit.ExcludingVAT, Percent),
                                                PriceLimit.IncludingVAT.HasValue ? Reduced(PriceLimit.IncludingVAT.Value, Percent) : null);

        private static Decimal Reduced(Decimal  Price,
                                       Decimal  Percent)

            => Math.Round(Price * (100 - Percent) / 100, 6);

        #endregion

    }

}
