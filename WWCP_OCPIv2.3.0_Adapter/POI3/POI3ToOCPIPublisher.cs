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

using System.Collections.Concurrent;
using System.Threading.Channels;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPI;

using POI = cloud.charging.open.protocols.WWCP.POI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_3_0.POI3
{

    /// <summary>
    /// Something could not be mapped or stored.
    /// </summary>
    /// <param name="Sender">The publisher or importer.</param>
    /// <param name="Description">What went wrong.</param>
    /// <param name="Warnings">What could not be mapped, if anything.</param>
    public delegate void OnPOI3OCPIErrorDelegate(Object                Sender,
                                                 String                Description,
                                                 IEnumerable<Warning>  Warnings);


    /// <summary>
    /// The CPO side: a POI3 history feeds an OCPI 2.3.0 CommonAPI, which serves its locations and tariffs to EMSPs.
    /// </summary>
    /// <remarks>
    /// At the start, every charging pool and charging tariff of the selected operators is stored as an OCPI location
    /// and tariff. Every new commit maps the head again: what changed is stored, what is gone is removed. Every change of the
    /// status or admin status of an EVSE updates its OCPI EVSE. The parties of the operators must be parties of the CommonAPI,
    /// and what it holds for them is the publisher's: at the start, what is no longer in the head is removed, e.g. what was
    /// removed while the publisher was not running.
    /// The charging tariff offers of the operators take the place of their charging tariffs while they apply: the head is
    /// published again when the status of an offer changes, and at the next time one begins or ends to apply. Where the
    /// tariffs of the eMSPs of the e-mobility providers an offer is for are others, the CommonAPI asks the publisher for
    /// them by eMSP (CommonAPI.GetTariffIdsDelegate).
    /// The work is done one after the other, in the order it came.
    /// </remarks>
    public sealed class POI3ToOCPIPublisher : IAsyncDisposable
    {

        #region Data

        private readonly Channel<Func<Task>>                                      work         = Channel.CreateUnbounded<Func<Task>>(new UnboundedChannelOptions { SingleReader = true });
        private readonly HashSet<(Party_Idv3 Party, Location_Id LocationId)>      locations    = [];
        private readonly HashSet<(Party_Idv3 Party, Tariff_Id   TariffId)>        tariffs      = [];
        private          Task?                                                    worker;
        private          Int32                                                    headQueued;

        /// <summary>
        /// The tariffs of the connectors whose tariffs differ by eMSP, as charging tariff offers make them: the CommonAPI
        /// asks for them (TariffIdsFor).
        /// </summary>
        private readonly ConcurrentDictionary<(Party_Idv3 Party, Location_Id Location, String EVSE, String Connector), OfferedTariffIds>  tariffsByEMSP  = [];

        /// <summary>
        /// The tariff identifications the CommonAPI asked for before the publisher started: asked for all others.
        /// </summary>
        private          GetTariffIds2_Delegate?                                  otherTariffIds;

        /// <summary>
        /// Wakes the publisher up when the next charging tariff offer begins or ends to apply.
        /// </summary>
        private readonly Timer                                                    offersTimer;

        /// <summary>
        /// The longest the timer of the offers waits at once: then it looks again.
        /// </summary>
        private static readonly TimeSpan                                          maxOffersWait  = TimeSpan.FromHours(1);

        #endregion

        #region Properties

        /// <summary>
        /// The history published.
        /// </summary>
        public POI.RoamingNetworkHistory                          History        { get; }

        /// <summary>
        /// The OCPI CommonAPI it is published to.
        /// </summary>
        public CommonAPI                                          CommonAPI      { get; }

        /// <summary>
        /// The operators published; all, unless given.
        /// </summary>
        public IReadOnlySet<POI.ChargingStationOperator_Id>?      OperatorIds    { get; }

        #endregion

        #region Events

        /// <summary>
        /// Something could not be mapped or stored.
        /// </summary>
        public event OnPOI3OCPIErrorDelegate?  OnError;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Publish the given history to the given OCPI CommonAPI.
        /// </summary>
        /// <param name="History">A history of a roaming network.</param>
        /// <param name="CommonAPI">An OCPI 2.3.0 CommonAPI of a CPO.</param>
        /// <param name="OperatorIds">The operators to publish; all, unless given.</param>
        public POI3ToOCPIPublisher(POI.RoamingNetworkHistory                      History,
                                   CommonAPI                                      CommonAPI,
                                   IEnumerable<POI.ChargingStationOperator_Id>?   OperatorIds   = null)
        {

            this.History      = History;
            this.CommonAPI    = CommonAPI;
            this.OperatorIds  = OperatorIds?.ToHashSet();
            this.offersTimer  = new Timer(_ => QueueHead(), null, Timeout.Infinite, Timeout.Infinite);

        }

        #endregion


        #region StartAsync()

        /// <summary>
        /// Publish the head, and every change from now on; returns once the head is published.
        /// </summary>
        public async Task StartAsync()
        {

            if (worker is not null)
                throw new InvalidOperationException("The publisher has been started already!");

            History.OnCommitAnnouncement           += Announced;
            History.RuntimeStatus.OnStatusUpdate   += StatusUpdated;
            History.RuntimeStatus.OnStatusPlanned  += StatusUpdated;

            // The tariffs of the connectors whose tariffs differ by eMSP: the publisher's, all others as before.
            otherTariffIds                   = CommonAPI.GetTariffIdsDelegate;
            CommonAPI.GetTariffIdsDelegate   = TariffIdsFor;

            worker = Task.Run(WorkAsync);

            var published = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

            work.Writer.TryWrite(async () => {
                AdoptStored();
                await PublishHeadAsync();
                published.TrySetResult();
            });

            await published.Task;

        }

        #endregion

        #region DisposeAsync()

        /// <summary>
        /// Stop publishing, after what is waiting.
        /// </summary>
        public async ValueTask DisposeAsync()
        {

            History.OnCommitAnnouncement           -= Announced;
            History.RuntimeStatus.OnStatusUpdate   -= StatusUpdated;
            History.RuntimeStatus.OnStatusPlanned  -= StatusUpdated;

            await offersTimer.DisposeAsync();

            work.Writer.TryComplete();

            if (worker is not null)
                await worker;

            if (CommonAPI.GetTariffIdsDelegate == TariffIdsFor)
                CommonAPI.GetTariffIdsDelegate = otherTariffIds;

        }

        #endregion


        #region (private) Announced(Announcement) / StatusUpdated(StatusUpdate)

        private void Announced(POI.RoamingNetworkCommitAnnouncement Announcement)

            => work.Writer.TryWrite(PublishHeadAsync);

        // Raised while the status schedule of the EVSE is locked: only queued here. A planned status as well: the
        // status schedule of the OCPI EVSE.
        private void StatusUpdated(POI.POIStatusUpdate StatusUpdate)
        {

            if (StatusUpdate.EntityType == nameof(POI.EVSE) &&
                StatusUpdate.Kind is POI.POIRuntimeStatusKind.Status or POI.POIRuntimeStatusKind.AdminStatus)
            {
                work.Writer.TryWrite(() => PublishEVSEStatusAsync(StatusUpdate.EntityId));
            }

            // New prices change tariffs and the locations naming them: published as the head is, once for all
            // price statuses reported until then, e.g. of every charging pool following the electricity market. So
            // does the status of a charging tariff offer, e.g. one becoming active at its notBefore.
            else if (StatusUpdate.Kind == POI.POIRuntimeStatusKind.PriceStatus ||
                     (StatusUpdate.Kind == POI.POIRuntimeStatusKind.Status && StatusUpdate.EntityType == nameof(POI.ChargingTariffOffer)))
            {
                QueueHead();
            }

        }

        #endregion

        #region (private) QueueHead()

        /// <summary>
        /// Publish the head once more, once for all asking for it until then.
        /// </summary>
        private void QueueHead()
        {
            if (Interlocked.Exchange(ref headQueued, 1) == 0)
                work.Writer.TryWrite(() => {
                    Interlocked.Exchange(ref headQueued, 0);
                    return PublishHeadAsync();
                });
        }

        #endregion

        #region (private) TariffIdsFor(Party, LocationId, EVSEId, ConnectorId, EMSP)

        /// <summary>
        /// The tariffs of a connector whose tariffs differ by eMSP, for the given eMSP; of any other one, as the
        /// CommonAPI asked for them before.
        /// </summary>
        private IEnumerable<Tariff_Id> TariffIdsFor(Party_Idv3       Party,
                                                    Location_Id?     LocationId,
                                                    EVSE_Id?         EVSEId,
                                                    Connector_Id?    ConnectorId,
                                                    RemoteParty_Id?  EMSP)

            => LocationId.HasValue && EVSEId.HasValue && ConnectorId.HasValue &&
               tariffsByEMSP.TryGetValue((Party, LocationId.Value, EVSEId.Value.ToString(), ConnectorId.Value.ToString()), out var offered)
                   ? offered.For(EMSP)
                   : otherTariffIds?.Invoke(Party, LocationId, EVSEId, ConnectorId, EMSP) ?? [];

        #endregion

        #region (private) WorkAsync()

        private async Task WorkAsync()
        {
            await foreach (var job in work.Reader.ReadAllAsync())
            {
                try
                {
                    await job();
                }
                catch (Exception e)
                {
                    RaiseError($"Publishing failed: {e.Message}", []);
                }
            }
        }

        #endregion

        #region (private) AdoptStored()

        /// <summary>
        /// Take what the CommonAPI holds for the parties of the selected operators as published before, so that the first
        /// publication removes what is no longer in the head. Without selected operators, those of the head: the locations
        /// and tariffs of an operator removed from the roaming network while the publisher was not running stay.
        /// </summary>
        private void AdoptStored()
        {

            var parties = (OperatorIds ?? History.Head.RoamingNetwork.ChargingStationOperators.Select(chargingStationOperator => chargingStationOperator.Id)).
                              Select(operatorId => operatorId.ToOCPIParty()).
                              ToHashSet();

            foreach (var party in parties)
            {

                foreach (var location in CommonAPI.GetLocations(party))
                    locations.Add((party, location.Id));

                foreach (var tariff in CommonAPI.GetTariffs(party))
                    tariffs.Add((party, tariff.Id));

            }

        }

        #endregion

        #region (private) PublishHeadAsync()

        /// <summary>
        /// Store what changed of the selected operators' charging pools and charging tariffs, and remove what is gone.
        /// </summary>
        private async Task PublishHeadAsync()
        {

            var head              = History.Head.RoamingNetwork;
            var now               = Timestamp.Now;
            var currentLocations  = new HashSet<(Party_Idv3, Location_Id)>();
            var currentTariffs    = new HashSet<(Party_Idv3, Tariff_Id)>();
            var currentByEMSP     = new HashSet<(Party_Idv3, Location_Id, String, String)>();
            var operators         = head.ChargingStationOperators.Where(op => OperatorIds is null || OperatorIds.Contains(op.Id)).ToArray();

            foreach (var chargingStationOperator in operators)
            {

                var party = chargingStationOperator.Id.ToOCPIParty();

                // With the properties their templates give its charging stations, and the charging tariff offers taking
                // the place of its charging tariffs now: mapped before the tariffs the offers make are known.
                var offers    = new OfferedTariffsAt(now);
                var mapped    = new List<(POI.ChargingPool ChargingPool, Location? Location, List<Warning> Warnings)>();

                foreach (var chargingPool in chargingStationOperator.EffectiveChargingPools)
                {
                    var warnings = new List<Warning>();
                    mapped.Add((chargingPool, chargingPool.ToOCPI(head, warnings, Offers: offers), warnings));
                }

                // Every charging tariff as it is at runtime, every one with the new prices in effect at an EVSE, a tariff
                // of its own, and every one the offers make: published before the locations naming them.
                var pricedTariffs = chargingStationOperator.EffectiveChargingTariffs.
                                        Select(chargingTariff => (chargingTariff, (Timestamped<POI.ChargingTariffPrices>?) null)).
                                        Concat(PricesInEffect(chargingStationOperator)).
                                        ToArray();

                foreach (var (chargingTariff, prices) in pricedTariffs)
                {

                    var warnings  = new List<Warning>();
                    var tariff    = chargingTariff.ToOCPI(warnings, Prices: prices);

                    if (tariff is null)
                    {
                        RaiseError($"The charging tariff '{chargingTariff.Id}' can not be published{(prices.HasValue ? " with its new prices" : "")}!", warnings);
                        continue;
                    }

                    currentTariffs.Add((party, tariff.Id));
                    tariffs.       Add((party, tariff.Id));

                    if (CommonAPI.TryGetTariff(party, tariff.Id, out var stored))
                    {

                        if (SameContent(stored.ToJSON(), tariff.ToJSON()))
                            continue;

                        if (tariff.LastUpdated <= stored.LastUpdated)
                            tariff = chargingTariff.ToOCPI(warnings, stored.LastUpdated.AddMilliseconds(1), prices)!;

                    }

                    var result = await CommonAPI.AddOrUpdateTariff(tariff);

                    if (!result.IsSuccess)
                        RaiseError($"The tariff '{tariff.Id}' can not be stored: {result.ErrorResponse}", warnings);

                }

                foreach (var offeredTariff in offers.Tariffs)
                {

                    var warnings  = new List<Warning>();
                    var tariff    = offeredTariff.ToOCPI(warnings);

                    if (tariff is null)
                    {
                        RaiseError($"The tariff the charging tariff offers '{String.Join("', '", offeredTariff.Offers.Select(offer => offer.Id))}' make of the charging tariff '{offeredTariff.ChargingTariff.Id}' can not be published!", warnings);
                        continue;
                    }

                    currentTariffs.Add((party, tariff.Id));
                    tariffs.       Add((party, tariff.Id));

                    if (CommonAPI.TryGetTariff(party, tariff.Id, out var stored))
                    {

                        if (SameContent(stored.ToJSON(), tariff.ToJSON()))
                            continue;

                        if (tariff.LastUpdated <= stored.LastUpdated)
                            tariff = offeredTariff.ToOCPI(warnings, stored.LastUpdated.AddMilliseconds(1))!;

                    }

                    var result = await CommonAPI.AddOrUpdateTariff(tariff);

                    if (!result.IsSuccess)
                        RaiseError($"The tariff '{tariff.Id}' can not be stored: {result.ErrorResponse}", warnings);

                }

                foreach (var (chargingPool, mappedLocation, warnings) in mapped)
                {

                    var location = mappedLocation;

                    if (location is null)
                    {
                        RaiseError($"The charging pool '{chargingPool.Id}' can not be published!", warnings);
                        continue;
                    }

                    currentLocations.Add((party, location.Id));
                    locations.       Add((party, location.Id));

                    // The connectors whose tariffs differ by eMSP: theirs the CommonAPI asks for, set before the location
                    // is stored, which it pushes to the eMSPs.
                    var byEMSP     = location.EVSEs.SelectMany(evse => evse.Connectors.Select(connector => (EVSE:       evse.EVSEId?.ToString() ?? evse.UId.ToString(),
                                                                                                         Connector:  connector.Id.ToString()))).
                                                    Where     (connector => offers.Connectors.TryGetValue(connector, out var offered) && offered.ByEMSP).
                                                    ToDictionary(connector => (party, location.Id, connector.EVSE, connector.Connector),
                                                                 connector => offers.Connectors[connector]);

                    var before     = tariffsByEMSP.Where(connector => connector.Key.Party == party && connector.Key.Location == location.Id).
                                                   ToDictionary(connector => connector.Key, connector => connector.Value);

                    var sameByEMSP = before.Count == byEMSP.Count &&
                                     byEMSP.All(connector => before.TryGetValue(connector.Key, out var told) && told.SameAs(connector.Value));

                    foreach (var key in before.Keys.Except(byEMSP.Keys))
                        tariffsByEMSP.TryRemove(key, out _);

                    foreach (var (key, offered) in byEMSP)
                    {
                        tariffsByEMSP[key] = offered;
                        currentByEMSP.Add(key);
                    }

                    if (CommonAPI.TryGetLocation(party, location.Id, out var stored))
                    {

                        if (sameByEMSP && SameContent(WithoutTariffIdsByEMSP(stored.  ToJSON(), byEMSP.Keys),
                                                      WithoutTariffIdsByEMSP(location.ToJSON(), byEMSP.Keys)))
                        {
                            continue;
                        }

                        if (location.LastUpdated <= stored.LastUpdated)
                            location = chargingPool.ToOCPI(head, warnings, stored.LastUpdated.AddMilliseconds(1), offers)!;

                    }

                    var result = await CommonAPI.AddOrUpdateLocation(location);

                    if (!result.IsSuccess)
                        RaiseError($"The location '{location.Id}' can not be stored: {result.ErrorResponse}", warnings);

                }

            }

            // The tariffs by eMSP of locations no longer published, or of connectors no longer differing.
            foreach (var key in tariffsByEMSP.Keys.Where(key => !currentByEMSP.Contains(key)).ToArray())
                tariffsByEMSP.TryRemove(key, out _);

            // Wake up when the next offer begins or ends to apply.
            var next = operators.Select(chargingStationOperator => POI3Mapper.NextOfferChange(chargingStationOperator, now)).
                                 Where (timestamp => timestamp.HasValue).
                                 Min();

            offersTimer.Change(next.HasValue
                                   ? TimeSpan.FromTicks(Math.Clamp((next.Value - Timestamp.Now + TimeSpan.FromMilliseconds(1)).Ticks, 0, maxOffersWait.Ticks))
                                   : Timeout.InfiniteTimeSpan,
                               Timeout.InfiniteTimeSpan);

            foreach (var (party, locationId) in locations.Except(currentLocations).ToArray())
            {
                await CommonAPI.RemoveLocation(party, locationId);
                locations.Remove((party, locationId));
            }

            foreach (var (party, tariffId) in tariffs.Except(currentTariffs).ToArray())
            {
                await CommonAPI.RemoveTariff(party, tariffId);
                tariffs.Remove((party, tariffId));
            }

        }

        #endregion

        #region (private static) PricesInEffect(Operator)

        /// <summary>
        /// The charging tariffs of the given operator with the new prices in effect at its EVSEs, each prices once:
        /// the same prices at many EVSEs are one tariff, since the earliest of them.
        /// </summary>
        private static IEnumerable<(POI.ChargingTariff, Timestamped<POI.ChargingTariffPrices>?)> PricesInEffect(POI.ChargingStationOperator Operator)
        {

            var pricesInEffect = new Dictionary<(POI.ChargingTariff_Id, String), Timestamped<POI.ChargingTariffPrices>>();

            foreach (var chargingPool in Operator.EffectiveChargingPools)
                foreach (var chargingStation in chargingPool.ChargingStations)
                    foreach (var evse in chargingStation.EVSEs)
                        foreach (var prices in POI3Mapper.PricesOf(evse, chargingStation, chargingPool, Operator))
                        {

                            var key = (prices.Value.ChargingTariffId, POI3Mapper.ElementsText(prices.Value.Elements));

                            if (!pricesInEffect.TryGetValue(key, out var known) || prices.Timestamp < known.Timestamp)
                                pricesInEffect[key] = prices;

                        }

            foreach (var ((chargingTariffId, _), prices) in pricesInEffect.OrderBy(entry => entry.Key.Item1.ToString(), StringComparer.Ordinal).
                                                                           ThenBy (entry => entry.Key.Item2,            StringComparer.Ordinal))
            {
                if (Operator.TryGetEffectiveChargingTariff(chargingTariffId, out var chargingTariff))
                    yield return (chargingTariff, prices);
            }

        }

        #endregion

        #region (private) PublishEVSEStatusAsync(EVSEId)

        /// <summary>
        /// Store the current status of the given EVSE in its OCPI EVSE.
        /// </summary>
        private async Task PublishEVSEStatusAsync(String EVSEId)
        {

            var head = History.Head.RoamingNetwork;

            if (!POI.EVSE_Id.TryParse(EVSEId, out var evseId) ||
                (OperatorIds is not null && !OperatorIds.Contains(evseId.OperatorId)) ||
                !head.TryGetChargingStationOperatorById(evseId.OperatorId, out var chargingStationOperator))
            {
                return;
            }

            // As its location tells it: with the properties their templates give its charging stations.
            foreach (var chargingPool in chargingStationOperator.EffectiveChargingPools)
            {
                foreach (var chargingStation in chargingPool.ChargingStations)
                {

                    if (!chargingStation.TryGetEVSE(evseId, out var evse))
                        continue;

                    var party       = chargingStationOperator.Id.ToOCPIParty();
                    var locationId  = chargingPool.Id.ToOCPILocationId();

                    if (!locationId.HasValue || !CommonAPI.TryGetLocation(party, locationId.Value, out var location))
                        return;

                    // With the charging tariff offers taking the place of its charging tariffs now: what they make of
                    // them changes only with the head, published again then.
                    var warnings    = new List<Warning>();
                    var offers      = new OfferedTariffsAt(Timestamp.Now);
                    var ocpiEVSE    = evse.ToOCPI(chargingStation, chargingPool, chargingStationOperator, warnings, Offers: offers);

                    if (ocpiEVSE is null)
                        return;

                    if (location.TryGetEVSE(ocpiEVSE.UId, out var stored))
                    {

                        if (stored.Status == ocpiEVSE.Status &&
                            JToken.DeepEquals(new JArray(stored.  StatusSchedule.Select(period => period.ToJSON())),
                                              new JArray(ocpiEVSE.StatusSchedule.Select(period => period.ToJSON()))))
                        {
                            return;
                        }

                        if (ocpiEVSE.LastUpdated <= stored.LastUpdated)
                            ocpiEVSE = evse.ToOCPI(chargingStation, chargingPool, chargingStationOperator, warnings, stored.LastUpdated.AddMilliseconds(1), offers)!;

                    }

                    var result = await CommonAPI.AddOrUpdateEVSE(location, ocpiEVSE);

                    if (!result.IsSuccess)
                        RaiseError($"The status of the EVSE '{evseId}' can not be stored: {result.ErrorResponse}", warnings);

                    return;

                }
            }

        }

        #endregion

        #region (private static) WithoutTariffIdsByEMSP(Location, Connectors)

        /// <summary>
        /// The given OCPI location without the tariff identifications of the given connectors, whose tariffs differ by
        /// eMSP: compared by what the publisher remembers of them.
        /// </summary>
        private static JObject WithoutTariffIdsByEMSP(JObject                                                                Location,
                                                      IEnumerable<(Party_Idv3 Party, Location_Id Location, String EVSE, String Connector)>  Connectors)
        {

            var connectors = Connectors.Select(connector => (connector.EVSE, connector.Connector)).ToHashSet();

            if (connectors.Count == 0)
                return Location;

            var copy = (JObject) Location.DeepClone();

            foreach (var evse in copy["evses"]?.OfType<JObject>() ?? [])
                foreach (var connector in evse["connectors"]?.OfType<JObject>() ?? [])
                    if (connectors.Contains((evse["evse_id"]?.Value<String>() ?? evse["uid"]?.Value<String>() ?? "", connector["id"]?.Value<String>() ?? "")))
                        connector.Remove("tariff_ids");

            return copy;

        }

        #endregion

        #region (internal static) SameContent(JSON1, JSON2)

        /// <summary>
        /// Whether the given OCPI objects say the same, but for their timestamps.
        /// </summary>
        internal static Boolean SameContent(JObject JSON1, JObject JSON2)

            => JToken.DeepEquals(WithoutTimestamps(JSON1), WithoutTimestamps(JSON2));

        private static JToken WithoutTimestamps(JToken JSON)
        {

            var copy = JSON.DeepClone();

            if (copy is JContainer container)
                foreach (var property in container.Descendants().OfType<JProperty>().Where(property => property.Name is "last_updated" or "created").ToArray())
                    property.Remove();

            return copy;

        }

        #endregion

        #region (private) RaiseError(Description, Warnings)

        private void RaiseError(String                Description,
                                IEnumerable<Warning>  Warnings)
        {
            try
            {
                OnError?.Invoke(this, Description, Warnings);
            }
            catch
            { }
        }

        #endregion

    }

}
