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

using System.Threading.Channels;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPI;

using POI = cloud.charging.open.protocols.WWCP.POI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_3_0.POI3
{

    /// <summary>
    /// Signs a change set of the import with the keys of the operators it changes, e.g. of a hub
    /// that received their data and is trusted by them.
    /// </summary>
    /// <param name="ChangeSet">A change set of the import, unsigned.</param>
    public delegate POI.RoamingNetworkChangeSet SignPOI3ChangeSetDelegate(POI.RoamingNetworkChangeSet ChangeSet);


    /// <summary>
    /// The EMSP or hub side: what an OCPI 2.3.0 CommonAPI receives, e.g. locations and tariffs pushed by CPOs,
    /// is committed to a POI3 history.
    /// </summary>
    /// <remarks>
    /// Every location or tariff added or changed becomes a change set against the head: the charging pool or charging
    /// tariff as OCPI tells it, onto the one there is, with what only POI3 knows kept. It is signed by the given delegate,
    /// as the operator owns its data, and committed; nothing changed, nothing is committed. A location or tariff removed
    /// removes its charging pool or charging tariff. The status of an EVSE goes into the runtime status of the history.
    /// The operators must be in the roaming network already: adding them is up to its operator.
    /// The work is done one after the other, in the order it came.
    /// </remarks>
    public sealed class OCPIToPOI3Importer : IAsyncDisposable
    {

        #region Data

        private readonly Channel<Func<Task>>  work  = Channel.CreateUnbounded<Func<Task>>(new UnboundedChannelOptions { SingleReader = true });
        private          Task?                worker;

        #endregion

        #region Properties

        /// <summary>
        /// The OCPI CommonAPI imported from.
        /// </summary>
        public CommonAPI                                   CommonAPI                    { get; }

        /// <summary>
        /// The history imported into.
        /// </summary>
        public POI.RoamingNetworkHistory                   History                      { get; }

        /// <summary>
        /// Signs the change sets of the import.
        /// </summary>
        public SignPOI3ChangeSetDelegate                   Sign                         { get; }

        /// <summary>
        /// The language of texts OCPI gives without one.
        /// </summary>
        public Languages                                   Language                     { get; }

        /// <summary>
        /// An optional additional verifier of change set signatures, given to the history when committing.
        /// </summary>
        public POI.VerifyChangeSetSignatureDelegate?       VerifyChangeSetSignature     { get; }

        #endregion

        #region Events

        /// <summary>
        /// Something could not be mapped or committed.
        /// </summary>
        public event OnPOI3OCPIErrorDelegate?  OnError;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Import what the given OCPI CommonAPI receives into the given history.
        /// </summary>
        /// <param name="CommonAPI">An OCPI 2.3.0 CommonAPI, e.g. of an EMSP or a hub.</param>
        /// <param name="History">A history of a roaming network.</param>
        /// <param name="Sign">Signs the change sets of the import with the keys of the operators.</param>
        /// <param name="Language">The language of texts OCPI gives without one; English by default.</param>
        /// <param name="VerifyChangeSetSignature">An optional additional verifier of change set signatures.</param>
        public OCPIToPOI3Importer(CommonAPI                               CommonAPI,
                                  POI.RoamingNetworkHistory               History,
                                  SignPOI3ChangeSetDelegate               Sign,
                                  Languages?                              Language                   = null,
                                  POI.VerifyChangeSetSignatureDelegate?   VerifyChangeSetSignature   = null)
        {

            this.CommonAPI                 = CommonAPI;
            this.History                   = History;
            this.Sign                      = Sign;
            this.Language                  = Language ?? Languages.en;
            this.VerifyChangeSetSignature  = VerifyChangeSetSignature;

        }

        #endregion


        #region Start()

        /// <summary>
        /// Import every change of the CommonAPI from now on.
        /// </summary>
        public void Start()
        {

            if (worker is not null)
                throw new InvalidOperationException("The importer has been started already!");

            CommonAPI.OnLocationAdded      += LocationChanged;
            CommonAPI.OnLocationChanged    += LocationChanged;
            CommonAPI.OnLocationRemoved    += LocationRemoved;
            CommonAPI.OnEVSEAdded          += EVSEChanged;
            CommonAPI.OnEVSEChanged        += EVSEChanged;
            CommonAPI.OnEVSERemoved        += EVSEChanged;
            CommonAPI.OnEVSEStatusChanged  += EVSEStatusChanged;
            CommonAPI.OnTariffAdded        += TariffChanged;
            CommonAPI.OnTariffChanged      += TariffChanged;
            CommonAPI.OnTariffRemoved      += TariffsRemoved;

            worker = Task.Run(WorkAsync);

        }

        #endregion

        #region ImportAsync(Location) / ImportAsync(Tariff)

        /// <summary>
        /// Import the given location now, e.g. one the CommonAPI held before the import started.
        /// </summary>
        public Task ImportAsync(Location Location)
            => Enqueue(() => ImportLocation(Location));

        /// <summary>
        /// Import the given tariff now, e.g. one the CommonAPI held before the import started.
        /// </summary>
        public Task ImportAsync(Tariff Tariff)
            => Enqueue(() => ImportTariff(Tariff));

        #endregion

        #region DisposeAsync()

        /// <summary>
        /// Stop importing, after what is waiting.
        /// </summary>
        public async ValueTask DisposeAsync()
        {

            CommonAPI.OnLocationAdded      -= LocationChanged;
            CommonAPI.OnLocationChanged    -= LocationChanged;
            CommonAPI.OnLocationRemoved    -= LocationRemoved;
            CommonAPI.OnEVSEAdded          -= EVSEChanged;
            CommonAPI.OnEVSEChanged        -= EVSEChanged;
            CommonAPI.OnEVSERemoved        -= EVSEChanged;
            CommonAPI.OnEVSEStatusChanged  -= EVSEStatusChanged;
            CommonAPI.OnTariffAdded        -= TariffChanged;
            CommonAPI.OnTariffChanged      -= TariffChanged;
            CommonAPI.OnTariffRemoved      -= TariffsRemoved;

            work.Writer.TryComplete();

            if (worker is not null)
                await worker;

        }

        #endregion


        #region (private) Events of the CommonAPI

        private Task LocationChanged(Location Location)
        {
            work.Writer.TryWrite(() => { ImportLocation(Location); return Task.CompletedTask; });
            return Task.CompletedTask;
        }

        private Task LocationRemoved(Location Location)
        {
            work.Writer.TryWrite(() => { RemoveLocation(Location); return Task.CompletedTask; });
            return Task.CompletedTask;
        }

        // The location as the CommonAPI holds it now, with the EVSE.
        private Task EVSEChanged(EVSE EVSE)
        {

            if (EVSE.ParentLocation is Location parent)
                work.Writer.TryWrite(() => {
                    if (CommonAPI.TryGetLocation(Party_Idv3.From(parent.CountryCode, parent.PartyId), parent.Id, out var location))
                        ImportLocation(location);
                    return Task.CompletedTask;
                });

            return Task.CompletedTask;

        }

        private Task EVSEStatusChanged(DateTimeOffset  Timestamp,
                                       EVSE            EVSE,
                                       StatusType      NewStatus,
                                       StatusType?     OldStatus)
        {
            // A removed EVSE is gone with the location as it is stored now.
            if (NewStatus != StatusType.REMOVED)
                work.Writer.TryWrite(() => { ImportStatus(EVSE, NewStatus, Timestamp); return Task.CompletedTask; });
            return Task.CompletedTask;
        }

        private Task TariffChanged(Tariff Tariff)
        {
            work.Writer.TryWrite(() => { ImportTariff(Tariff); return Task.CompletedTask; });
            return Task.CompletedTask;
        }

        private Task TariffsRemoved(IEnumerable<Tariff> Tariffs)
        {
            foreach (var tariff in Tariffs)
                work.Writer.TryWrite(() => { RemoveTariff(tariff); return Task.CompletedTask; });
            return Task.CompletedTask;
        }

        #endregion

        #region (private) WorkAsync() / Enqueue(Job)

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
                    RaiseError($"Importing failed: {e.Message}", []);
                }
            }
        }

        private Task Enqueue(Action Job)
        {

            var done = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

            work.Writer.TryWrite(() => {
                try
                {
                    Job();
                    done.TrySetResult();
                }
                catch (Exception e)
                {
                    done.TrySetException(e);
                }
                return Task.CompletedTask;
            });

            return done.Task;

        }

        #endregion


        #region (private) ImportLocation(Location)

        private void ImportLocation(Location Location)
        {

            var operatorId = POI3Mapper.ToPOI3Operator(Location.CountryCode, Location.PartyId);

            if (!operatorId.HasValue)
            {
                RaiseError($"The party '{Location.CountryCode}*{Location.PartyId}' of the location '{Location.Id}' is no operator!", []);
                return;
            }

            ChangeOperator(operatorId.Value,
                           $"OCPI location {Location.Id}",
                           Location.LastUpdated,
                           (head, operatorJSON, warnings) => {

                               var pools      = POI3Mapper.ArrayOf(operatorJSON, "chargingPools");

                               var poolId     = POI3Mapper.ToPOI3PoolId(operatorId.Value, Location.Id);
                               var existing   = pools.OfType<JObject>().FirstOrDefault(pool => pool["@id"]?.Value<String>() == poolId?.ToString());

                               // The existing charging pool as it is at runtime, with the properties its templates and those
                               // of its EVSEs give them, and as OCPI tells it: what the location tells alike is kept, what
                               // the templates give stays in the templates.
                               var effective  = poolId.HasValue &&
                                                head.TryGetChargingStationOperatorById(operatorId.Value, out var chargingStationOperator) &&
                                                chargingStationOperator.TryGetEffectiveChargingPool(poolId.Value, out var chargingPool)
                                                    ? chargingPool
                                                    : null;

                               var unchanged  = effective?.ToOCPI(head, []);

                               var desired    = Location.ToPOI3(operatorId.Value, existing, Language, warnings, unchanged, effective?.ToJSON());

                               if (desired is null)
                                   return false;

                               ImportParking(Location, head, operatorId.Value, operatorJSON, desired, warnings);

                               if (existing is not null)
                                   existing.Replace(desired);
                               else
                                   pools.Add(desired);

                               return true;

                           });

            // The status of every EVSE, e.g. of new ones, which start as available, and the status planned.
            foreach (var evse in Location.EVSEs.Where(evse => evse.Status != StatusType.REMOVED))
            {
                ImportStatus        (evse, evse.Status, evse.LastUpdated);
                ImportStatusSchedule(evse);
            }

            ImportPrices(Location, operatorId.Value, Location.LastUpdated);

        }

        #endregion

        #region (private) ImportParking(Location, Head, OperatorId, OperatorJSON, Pool, Warnings)

        /// <summary>
        /// The parking places of the given location as parking spaces, and the parking of its EVSEs as the parking spaces
        /// their charging stations name.
        /// </summary>
        /// <remarks>
        /// A parking place a parking operator already defines is named, not changed: the parking operator owns it.
        /// Every other one is a parking space of the charging station operator, added or changed, with what only POI3
        /// knows of it kept, and what OCPI tells as it would tell it as well. One another charging station operator
        /// defines is left out, with a warning. Parking spaces no location names any more stay.
        /// </remarks>
        private static void ImportParking(Location                        Location,
                                          POI.RoamingNetwork              Head,
                                          POI.ChargingStationOperator_Id  OperatorId,
                                          JObject                         OperatorJSON,
                                          JObject                         Pool,
                                          ICollection<Warning>            Warnings)
        {

            Head.TryGetChargingStationOperatorById(OperatorId, out var chargingStationOperator);

            var spaces    = OperatorJSON["parkingSpaces"] as JArray;
            var nameable  = new HashSet<String>();

            #region The parking places

            foreach (var parking in Location.ParkingPlaces)
            {

                var mapped = parking.ToPOI3(Location.Created, Location.LastUpdated, Warnings);

                if (mapped is null)
                    continue;

                // One of a parking operator, or of another charging station operator.
                if (Head.ParkingOperators.Any(parkingOperator => parkingOperator.TryGetParkingSpace(mapped.Id, out _)))
                {
                    nameable.Add(mapped.Id.ToString());
                    continue;
                }

                if (Head.ChargingStationOperators.Any(other => other.Id != OperatorId && other.TryGetParkingSpace(mapped.Id, out _)))
                {
                    Warnings.Add(Warning.Create($"The parking '{parking.Id}' of the location '{Location.Id}' is a parking space of another charging station operator, and is left out!"));
                    continue;
                }

                var mappedJSON  = mapped.ToJSON();
                var existing    = spaces?.OfType<JObject>().FirstOrDefault(space => POI.ParkingSpace_Id.TryParse(space["@id"]?.Value<String>() ?? "", out var id) && id == mapped.Id);

                if (existing is null)
                {
                    spaces ??= POI3Mapper.ArrayOf(OperatorJSON, "parkingSpaces");
                    spaces.Add(mappedJSON);
                }

                else
                {

                    // The existing parking space as OCPI tells it: what the parking tells alike is kept.
                    var unchanged = chargingStationOperator is not null && chargingStationOperator.TryGetParkingSpace(mapped.Id, out var existingSpace)
                                        ? existingSpace.ToOCPI().ToPOI3(Location.Created, Location.LastUpdated, [])?.ToJSON()
                                        : null;

                    POI3Mapper.Overlay(existing, mappedJSON, POI3Mapper.ParkingSpaceProperties.Where(property => !POI3Mapper.SameValue(unchanged, mappedJSON, property)));

                }

                nameable.Add(mapped.Id.ToString());

            }

            #endregion

            #region The parking spaces of the charging stations: those of their EVSEs at OCPI

            // Of the EVSEs that are there, as ToPOI3 takes them: a REMOVED one may have the EVSE ID of the one that
            // replaced it, which OCPI allows. Of two of the same identification the last one.
            var parkingOfEVSEs = new Dictionary<String, String[]>();

            foreach (var evse in Location.EVSEs.Where(evse => evse.Status != StatusType.REMOVED))
                parkingOfEVSEs[evse.EVSEId?.ToString() ?? evse.UId.ToString()] = [.. evse.Parking.Select(parking => parking.ParkingId.ToString())];

            foreach (var station in (Pool["chargingStations"] as JArray ?? []).OfType<JObject>())
            {

                var named = (station["EVSEs"] as JArray ?? []).OfType<JObject>().
                                SelectMany(evse => parkingOfEVSEs.GetValueOrDefault(evse["@id"]?.Value<String>() ?? "") ?? []).
                                ToHashSet();

                foreach (var parkingId in named.Where(parkingId => !nameable.Contains(parkingId) &&
                                                                   !(POI.ParkingSpace_Id.TryParse(parkingId, out var id) && Head.TryGetParkingSpaceById(id, out _))).ToArray())
                {
                    Warnings.Add(Warning.Create($"The parking '{parkingId}' of the location '{Location.Id}' is no parking space, and is left out!"));
                    named.Remove(parkingId);
                }

                // Named as before, also as written before, or as it names them with its templates: those its templates give it
                // stay theirs, a charging station template's parking spaces being those of its EVSEs at OCPI as well.
                var before     = (station["parkingSpaceIds"] as JArray ?? []).Values<String>().OfType<String>().ToHashSet();

                var effective  = chargingStationOperator is not null &&
                                 POI.ChargingStation_Id.TryParse(station["@id"]?.Value<String>() ?? "", out var stationId) &&
                                 chargingStationOperator.TryGetEffectiveChargingStation(stationId, out var effectiveStation)
                                     ? effectiveStation.ParkingSpaceIds.Select(parkingSpaceId => parkingSpaceId.ToString()).ToHashSet()
                                     : before;

                if (named.SetEquals(before) || named.SetEquals(effective))
                    continue;

                named.ExceptWith(effective.Except(before));

                if (named.SetEquals(before))
                    continue;

                if (named.Count > 0)
                    station["parkingSpaceIds"] = new JArray(named.Order(StringComparer.Ordinal));
                else
                    station.Remove("parkingSpaceIds");

            }

            #endregion

        }

        #endregion

        #region (private) RemoveLocation(Location)

        private void RemoveLocation(Location Location)
        {

            var operatorId = POI3Mapper.ToPOI3Operator(Location.CountryCode, Location.PartyId);

            if (!operatorId.HasValue)
                return;

            ChangeOperator(operatorId.Value,
                           $"OCPI location {Location.Id} removed",
                           Timestamp.Now,
                           (head, operatorJSON, warnings) => {

                               var poolId    = POI3Mapper.ToPOI3PoolId(operatorId.Value, Location.Id)?.ToString();
                               var existing  = (operatorJSON["chargingPools"] as JArray ?? []).OfType<JObject>().FirstOrDefault(pool => pool["@id"]?.Value<String>() == poolId);

                               existing?.Remove();
                               return existing is not null;

                           });

        }

        #endregion

        #region (private) ImportTariff(Tariff)

        private void ImportTariff(Tariff Tariff)
        {

            var operatorId = POI3Mapper.ToPOI3Operator(Tariff.CountryCode, Tariff.PartyId);

            if (!operatorId.HasValue)
            {
                RaiseError($"The party '{Tariff.CountryCode}*{Tariff.PartyId}' of the tariff '{Tariff.Id}' is no operator!", []);
                return;
            }

            // A tariff charging tariff offers make of a charging tariff, taking its place while they apply: neither a
            // charging tariff nor its prices, the offers are POI3's.
            if (Tariff.Id.IsOffered())
                return;

            // A tariff with new prices of a charging tariff: the price status of the locations naming it.
            if (Tariff.Id.HasPrices())
            {
                ImportPricesOf(Tariff, operatorId.Value);
                return;
            }

            ChangeOperator(operatorId.Value,
                           $"OCPI tariff {Tariff.Id}",
                           Tariff.LastUpdated,
                           (head, operatorJSON, warnings) => {

                               var chargingTariff = Tariff.ToPOI3(operatorId.Value, Language, warnings);

                               if (chargingTariff is null)
                                   return false;

                               var tariffs    = POI3Mapper.ArrayOf(operatorJSON, "chargingTariffs");

                               var existing   = tariffs.OfType<JObject>().FirstOrDefault(tariff => tariff["@id"]?.Value<String>() == chargingTariff.Id.ToString());
                               var desired    = chargingTariff.ToJSON();

                               if (existing is not null)
                               {

                                   // The existing charging tariff as OCPI tells it: what the tariff tells alike is kept,
                                   // e.g. a name and a description OCPI tells as one text.
                                   var unchanged  = head.TryGetChargingStationOperatorById(operatorId.Value, out var chargingStationOperator) &&
                                                    chargingStationOperator.TryGetChargingTariff(chargingTariff.Id, out var existingTariff)
                                                        ? existingTariff.ToOCPI([])?.ToPOI3(operatorId.Value, Language, [])?.ToJSON()
                                                        : null;

                                   // What only POI3 knows is kept.
                                   var merged     = (JObject) existing.DeepClone();

                                   POI3Mapper.Overlay(merged,
                                                      desired,
                                                      desired.Properties().
                                                              Select(property => property.Name).
                                                              Concat([ "name", "description", "tariffType", "tariffURL", "minPrice", "maxPrice", "validFrom", "validUntil", "energyMix" ]).
                                                              Distinct().
                                                              Where(property => property is not "created" and not "lastChange" &&
                                                                                !POI3Mapper.SameValue(unchanged, desired, property)));

                                   // Only new prices are runtime data, e.g. ad hoc prices following the electricity
                                   // market: the price status of the locations naming the tariff, not a commit.
                                   if (JToken.DeepEquals(WithoutElements(merged), WithoutElements(existing)))
                                       merged["elements"] = existing["elements"]?.DeepClone();

                                   existing.Replace(merged);

                               }
                               else
                                   tariffs.Add(desired);

                               return true;

                           });

            ImportPricesOf(Tariff, operatorId.Value);

        }

        private static JObject WithoutElements(JObject ChargingTariff)
        {
            var copy = (JObject) ChargingTariff.DeepClone();
            copy.Remove("elements");
            return copy;
        }

        #endregion

        #region (private) ImportPricesOf(Tariff, OperatorId) / ImportPrices(Location, OperatorId, Timestamp)

        /// <summary>
        /// The prices of the given tariff at the locations naming it, also with other prices.
        /// </summary>
        private void ImportPricesOf(Tariff                          Tariff,
                                    POI.ChargingStationOperator_Id  OperatorId)
        {

            var chargingTariffId = POI3Mapper.ToPOI3TariffIdOf(OperatorId, Tariff.Id);

            if (!chargingTariffId.HasValue)
                return;

            foreach (var location in CommonAPI.GetLocations(Party_Idv3.From(Tariff.CountryCode, Tariff.PartyId)))
                if (location.EVSEs.Any(evse => evse.Connectors.Any(connector => connector.TariffIds.Any(tariffId => POI3Mapper.ToPOI3TariffIdOf(OperatorId, tariffId) == chargingTariffId))))
                    ImportPrices(location, OperatorId, Tariff.LastUpdated);

        }

        /// <summary>
        /// The prices OCPI tells at the EVSEs of the given location as the price status of its charging pool and EVSEs:
        /// for every charging tariff, the elements of the tariff the connectors name, where they are not the tariff's.
        /// </summary>
        /// <remarks>
        /// Prices the same at every EVSE of the location naming a charging tariff are the charging pool's, others the
        /// EVSE's own. The price status of a charging station is followed, never set: an EVSE whose station has other
        /// prices gets its own. A tariff the CommonAPI does not hold (yet) tells nothing: its prices stay as they are.
        /// </remarks>
        private void ImportPrices(Location                        Location,
                                  POI.ChargingStationOperator_Id  OperatorId,
                                  DateTimeOffset                  Timestamp)
        {

            var poolId = POI3Mapper.ToPOI3PoolId(OperatorId, Location.Id);

            if (!poolId.HasValue ||
                !History.Head.RoamingNetwork.TryGetChargingStationOperatorById(OperatorId, out var chargingStationOperator) ||
                !chargingStationOperator.TryGetEffectiveChargingPool(poolId.Value, out var chargingPool))
            {
                return;
            }

            var party  = Party_Idv3.From(Location.CountryCode, Location.PartyId);
            var told   = new Dictionary<(POI.EVSE_Id EVSEId, POI.ChargingTariff_Id TariffId), POI.ChargingTariffPrices>();

            foreach (var evse in Location.EVSEs.Where(evse => evse.Status != StatusType.REMOVED))
            {

                if (!POI.EVSE_Id.TryParse(evse.EVSEId?.ToString() ?? evse.UId.ToString(), out var evseId))
                    continue;

                // A tariff charging tariff offers make tells no prices: those of its charging tariff stay as they are.
                foreach (var tariffId in evse.Connectors.SelectMany(connector => connector.TariffIds).Where(tariffId => !tariffId.IsOffered()).Distinct())
                {

                    var chargingTariffId = POI3Mapper.ToPOI3TariffIdOf(OperatorId, tariffId);

                    if (chargingTariffId.HasValue &&
                        chargingStationOperator.TryGetChargingTariff(chargingTariffId.Value, out _) &&
                        CommonAPI.TryGetTariff(party, tariffId, out var tariff) &&
                        tariff.ToPOI3Elements() is { Count: > 0 } elements)
                    {
                        told[(evseId, chargingTariffId.Value)] = new POI.ChargingTariffPrices(chargingTariffId.Value, elements);
                    }

                }

            }

            String ElementsOf(POI.ChargingTariff_Id ChargingTariffId)
                => chargingStationOperator.TryGetChargingTariff(ChargingTariffId, out var chargingTariff)
                       ? POI3Mapper.ElementsText(chargingTariff.Elements)
                       : "";

            #region The charging pool: the prices the same at all its EVSEs naming a tariff

            var poolPrices = PricesOf(chargingPool.PriceStatus);

            foreach (var byTariff in told.GroupBy(entry => entry.Key.TariffId))
            {

                var prices = byTariff.Select(entry => entry.Value).DistinctBy(price => POI3Mapper.ElementsText(price.Elements)).ToArray();

                // Other prices at other EVSEs: theirs.
                if (prices.Length != 1)
                    continue;

                if (POI3Mapper.ElementsText(prices[0].Elements) == ElementsOf(byTariff.Key))
                    poolPrices.Remove(byTariff.Key);
                else
                    poolPrices[byTariff.Key] = prices[0];

            }

            ImportPriceStatus(nameof(POI.ChargingPool), chargingPool.Id.ToString(), chargingPool.PriceStatus, poolPrices, Timestamp);

            #endregion

            #region The EVSEs: what the charging pool and their charging station do not tell

            foreach (var chargingStation in chargingPool.ChargingStations)
            {

                var stationPrices = PricesOf(chargingStation.PriceStatus);

                foreach (var evse in chargingStation.EVSEs)
                {

                    var evsePrices = PricesOf(evse.PriceStatus);

                    foreach (var ((_, chargingTariffId), prices) in told.Where(entry => entry.Key.EVSEId == evse.Id))
                    {

                        var withoutOwn = stationPrices.TryGetValue(chargingTariffId, out var stationPrice) ? POI3Mapper.ElementsText(stationPrice.Elements)
                                       : poolPrices.   TryGetValue(chargingTariffId, out var poolPrice)    ? POI3Mapper.ElementsText(poolPrice.   Elements)
                                       : ElementsOf(chargingTariffId);

                        if (POI3Mapper.ElementsText(prices.Elements) == withoutOwn)
                            evsePrices.Remove(chargingTariffId);
                        else
                            evsePrices[chargingTariffId] = prices;

                    }

                    ImportPriceStatus(nameof(POI.EVSE), evse.Id.ToString(), evse.PriceStatus, evsePrices, Timestamp);

                }

            }

            #endregion

        }

        private static Dictionary<POI.ChargingTariff_Id, POI.ChargingTariffPrices> PricesOf(Timestamped<POI.ChargingPrices>? PriceStatus)

            => PriceStatus?.Value.Prices.ToDictionary(price => price.ChargingTariffId) ?? [];

        #endregion

        #region (private) RemoveTariff(Tariff)

        private void RemoveTariff(Tariff Tariff)
        {

            var operatorId = POI3Mapper.ToPOI3Operator(Tariff.CountryCode, Tariff.PartyId);

            if (!operatorId.HasValue)
                return;

            ChangeOperator(operatorId.Value,
                           $"OCPI tariff {Tariff.Id} removed",
                           Timestamp.Now,
                           (head, operatorJSON, warnings) => {

                               var tariffId  = POI3Mapper.ToPOI3TariffId(operatorId.Value, Tariff.Id)?.ToString();
                               var existing  = (operatorJSON["chargingTariffs"] as JArray ?? []).OfType<JObject>().FirstOrDefault(tariff => tariff["@id"]?.Value<String>() == tariffId);

                               existing?.Remove();
                               return existing is not null;

                           });

        }

        #endregion

        #region (private) ImportStatus(EVSE, Status, Timestamp)

        private void ImportStatus(EVSE            EVSE,
                                  StatusType      Status,
                                  DateTimeOffset  Timestamp)
        {

            var evseId  = EVSE.EVSEId?.ToString() ?? EVSE.UId.ToString();
            var value   = Status.ToPOI3Status().ToString();

            // The CommonAPI keeps an EVSE only with a later 'last updated': what it tells is the current status.
            // A later one here, e.g. the default status of an EVSE just imported, is followed right after.
            if (History.RuntimeStatus.TryGet(POI.POIRuntimeStatusKind.Status, nameof(POI.EVSE), evseId, out var current) &&
                current.Timestamp >= Timestamp &&
                !String.Equals(current.Value, value, StringComparison.OrdinalIgnoreCase))
            {
                Timestamp = current.Timestamp.AddMilliseconds(1);
            }

            var result = History.RuntimeStatus.TryUpdate(POI.POIRuntimeStatusKind.Status,
                                                         nameof(POI.EVSE),
                                                         evseId,
                                                         value,
                                                         Timestamp,
                                                         out _,
                                                         out var errorResponse,
                                                         DataSource: "OCPI");

            if (result == POI.POIStatusUpdateResult.Refused)
                RaiseError($"The status of the EVSE '{evseId}' can not be taken: {errorResponse}", []);

        }

        #endregion

        #region (private) ImportPriceStatus(EntityType, EntityId, Current, Prices, Timestamp)

        /// <summary>
        /// The given prices as the price status of the given entity, unless it has these already.
        /// </summary>
        private void ImportPriceStatus(String                                                       EntityType,
                                       String                                                       EntityId,
                                       Timestamped<POI.ChargingPrices>?                             Current,
                                       IReadOnlyDictionary<POI.ChargingTariff_Id, POI.ChargingTariffPrices>  Prices,
                                       DateTimeOffset                                               Timestamp)
        {

            var priceStatus = new POI.ChargingPrices(Prices.Values);

            if (priceStatus.Text == (Current?.Value ?? new POI.ChargingPrices()).Text)
                return;

            // What OCPI tells is the current price status, also where the one here is later.
            if (Current.HasValue && Current.Value.Timestamp >= Timestamp)
                Timestamp = Current.Value.Timestamp.AddMilliseconds(1);

            var result = History.RuntimeStatus.TryUpdate(POI.POIRuntimeStatusKind.PriceStatus,
                                                         EntityType,
                                                         EntityId,
                                                         priceStatus.Text,
                                                         Timestamp,
                                                         out _,
                                                         out var errorResponse,
                                                         DataSource: "OCPI");

            if (result == POI.POIStatusUpdateResult.Refused)
                RaiseError($"The prices of the {EntityType} '{EntityId}' can not be taken: {errorResponse}", []);

        }

        #endregion

        #region (private) ImportStatusSchedule(EVSE)

        /// <summary>
        /// The status schedule of the given EVSE as planned status: every period beginning later than now is planned
        /// at its begin, and its end, where no other period begins then, plans the current status of the EVSE again.
        /// A status planned before and no longer told stays planned.
        /// </summary>
        private void ImportStatusSchedule(EVSE EVSE)
        {

            var now      = Timestamp.Now;
            var evseId   = EVSE.EVSEId?.ToString() ?? EVSE.UId.ToString();
            var periods  = EVSE.StatusSchedule.Where(period => period.Begin > now).ToArray();

            if (periods.Length == 0 || !POI.EVSE_Id.TryParse(evseId, out var id))
                return;

            // What is planned already is not planned again.
            var planned  = History.Head.RoamingNetwork.TryGetChargingStationOperatorById(id.OperatorId, out var chargingStationOperator) &&
                           chargingStationOperator.TryGetEVSE(id, out var poiEVSE)
                               ? poiEVSE.StatusSchedule(timestamp => timestamp > now).Select(status => (status.Timestamp, status.Value.ToString())).ToHashSet()
                               : [];

            var plans    = periods.Select(period => (period.Begin, period.Status)).
                                   Concat(periods.Where (period => period.End.HasValue && !periods.Any(other => other.Begin == period.End)).
                                                  Select(period => (period.End!.Value, EVSE.Status)));

            foreach (var (at, status) in plans)
            {

                var value = status.ToPOI3Status().ToString();

                if (planned.Contains((at, value)))
                    continue;

                var result = History.RuntimeStatus.TryUpdate(POI.POIRuntimeStatusKind.Status,
                                                             nameof(POI.EVSE),
                                                             evseId,
                                                             value,
                                                             at,
                                                             out _,
                                                             out var errorResponse,
                                                             DataSource: "OCPI");

                if (result == POI.POIStatusUpdateResult.Refused)
                    RaiseError($"The status planned for the EVSE '{evseId}' at {at:o} can not be taken: {errorResponse}", []);

            }

        }

        #endregion

        #region (private) ChangeOperator(OperatorId, Description, CreatedAt, Change)

        /// <summary>
        /// Change the JSON of the given operator in the head, and commit the difference, signed.
        /// </summary>
        private void ChangeOperator(POI.ChargingStationOperator_Id                                      OperatorId,
                                    String                                                              Description,
                                    DateTimeOffset                                                      CreatedAt,
                                    Func<POI.RoamingNetwork, JObject, ICollection<Warning>, Boolean>    Change)
        {

            var head          = History.Head.RoamingNetwork;
            var networkJSON   = head.ToJSON(IncludeETags: false);   // changed, so without the ETags of the head
            var operatorJSON  = (networkJSON["chargingStationOperators"] as JArray ?? []).
                                    OfType<JObject>().
                                    FirstOrDefault(op => op["@id"]?.Value<String>() == OperatorId.ToString());

            if (operatorJSON is null)
            {
                RaiseError($"The operator '{OperatorId}' is not in the roaming network: adding it is up to its operator!", []);
                return;
            }

            var warnings = new List<Warning>();

            if (!Change(head, operatorJSON, warnings))
            {
                if (warnings.Count > 0)
                    RaiseError($"{Description} can not be imported!", warnings);
                return;
            }

            if (!POI.RoamingNetwork.TryParse(networkJSON, out var desired, out var parseError))
            {
                RaiseError($"{Description} is no valid roaming network data: {parseError}", warnings);
                return;
            }

            // The difference: a merge whose left side is its base.
            if (!POI.RoamingNetwork.TryMerge(head, head, desired, POI.ChangeSet_Id.NewRandom(), out _, out var difference, out var conflicts))
            {
                RaiseError($"{Description} can not be imported: {String.Join(" ", conflicts.Select(conflict => conflict.Description))}", warnings);
                return;
            }

            if (difference.Changes.Length == 0)
                return;

            var changeSet  = head.CreateChangeSet(difference.Changes,
                                                  new ImmutableI18NString(I18NString.Create(Languages.en, Description)),
                                                  CreatedAt: CreatedAt);

            var result     = History.Commit(Sign(changeSet), VerifyChangeSetSignature);

            if (!result.IsCommitted)
                RaiseError($"{Description} can not be committed: {result.ErrorResponse}", warnings);

            else if (warnings.Count > 0)
                RaiseError($"{Description} was imported partly.", warnings);

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
