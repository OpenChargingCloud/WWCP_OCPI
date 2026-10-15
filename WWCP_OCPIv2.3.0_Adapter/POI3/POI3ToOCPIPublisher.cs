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
    /// The work is done one after the other, in the order it came.
    /// </remarks>
    public sealed class POI3ToOCPIPublisher : IAsyncDisposable
    {

        #region Data

        private readonly Channel<Func<Task>>                                      work         = Channel.CreateUnbounded<Func<Task>>(new UnboundedChannelOptions { SingleReader = true });
        private readonly HashSet<(Party_Idv3 Party, Location_Id LocationId)>      locations    = [];
        private readonly HashSet<(Party_Idv3 Party, Tariff_Id   TariffId)>        tariffs      = [];
        private          Task?                                                    worker;

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

            History.OnCommitAnnouncement          += Announced;
            History.RuntimeStatus.OnStatusUpdate  += StatusUpdated;

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

            History.OnCommitAnnouncement          -= Announced;
            History.RuntimeStatus.OnStatusUpdate  -= StatusUpdated;

            work.Writer.TryComplete();

            if (worker is not null)
                await worker;

        }

        #endregion


        #region (private) Announced(Announcement) / StatusUpdated(StatusUpdate)

        private void Announced(POI.RoamingNetworkCommitAnnouncement Announcement)

            => work.Writer.TryWrite(PublishHeadAsync);

        // Raised while the status schedule of the EVSE is locked: only queued here.
        private void StatusUpdated(POI.POIStatusUpdate StatusUpdate)
        {
            if (StatusUpdate.EntityType == nameof(POI.EVSE) &&
                StatusUpdate.Kind is POI.POIRuntimeStatusKind.Status or POI.POIRuntimeStatusKind.AdminStatus)
            {
                work.Writer.TryWrite(() => PublishEVSEStatusAsync(StatusUpdate.EntityId));
            }
        }

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
            var currentLocations  = new HashSet<(Party_Idv3, Location_Id)>();
            var currentTariffs    = new HashSet<(Party_Idv3, Tariff_Id)>();

            foreach (var chargingStationOperator in head.ChargingStationOperators.Where(op => OperatorIds is null || OperatorIds.Contains(op.Id)))
            {

                var party = chargingStationOperator.Id.ToOCPIParty();

                foreach (var chargingTariff in chargingStationOperator.ChargingTariffs)
                {

                    var warnings  = new List<Warning>();
                    var tariff    = chargingTariff.ToOCPI(warnings);

                    if (tariff is null)
                    {
                        RaiseError($"The charging tariff '{chargingTariff.Id}' can not be published!", warnings);
                        continue;
                    }

                    currentTariffs.Add((party, tariff.Id));
                    tariffs.       Add((party, tariff.Id));

                    if (CommonAPI.TryGetTariff(party, tariff.Id, out var stored))
                    {

                        if (SameContent(stored.ToJSON(), tariff.ToJSON()))
                            continue;

                        if (tariff.LastUpdated <= stored.LastUpdated)
                            tariff = chargingTariff.ToOCPI(warnings, stored.LastUpdated.AddMilliseconds(1))!;

                    }

                    var result = await CommonAPI.AddOrUpdateTariff(tariff);

                    if (!result.IsSuccess)
                        RaiseError($"The tariff '{tariff.Id}' can not be stored: {result.ErrorResponse}", warnings);

                }

                foreach (var chargingPool in chargingStationOperator.ChargingPools)
                {

                    var warnings  = new List<Warning>();
                    var location  = chargingPool.ToOCPI(head, warnings);

                    if (location is null)
                    {
                        RaiseError($"The charging pool '{chargingPool.Id}' can not be published!", warnings);
                        continue;
                    }

                    currentLocations.Add((party, location.Id));
                    locations.       Add((party, location.Id));

                    if (CommonAPI.TryGetLocation(party, location.Id, out var stored))
                    {

                        if (SameContent(stored.ToJSON(), location.ToJSON()))
                            continue;

                        if (location.LastUpdated <= stored.LastUpdated)
                            location = chargingPool.ToOCPI(head, warnings, stored.LastUpdated.AddMilliseconds(1))!;

                    }

                    var result = await CommonAPI.AddOrUpdateLocation(location);

                    if (!result.IsSuccess)
                        RaiseError($"The location '{location.Id}' can not be stored: {result.ErrorResponse}", warnings);

                }

            }

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

            foreach (var chargingPool in chargingStationOperator.ChargingPools)
            {
                foreach (var chargingStation in chargingPool.ChargingStations)
                {

                    if (!chargingStation.TryGetEVSE(evseId, out var evse))
                        continue;

                    var party       = chargingStationOperator.Id.ToOCPIParty();
                    var locationId  = chargingPool.Id.ToOCPILocationId();

                    if (!locationId.HasValue || !CommonAPI.TryGetLocation(party, locationId.Value, out var location))
                        return;

                    var warnings    = new List<Warning>();
                    var ocpiEVSE    = evse.ToOCPI(chargingStation, chargingPool, chargingStationOperator, warnings);

                    if (ocpiEVSE is null)
                        return;

                    if (location.TryGetEVSE(ocpiEVSE.UId, out var stored))
                    {

                        if (stored.Status == ocpiEVSE.Status)
                            return;

                        if (ocpiEVSE.LastUpdated <= stored.LastUpdated)
                            ocpiEVSE = evse.ToOCPI(chargingStation, chargingPool, chargingStationOperator, warnings, stored.LastUpdated.AddMilliseconds(1))!;

                    }

                    var result = await CommonAPI.AddOrUpdateEVSE(location, ocpiEVSE);

                    if (!result.IsSuccess)
                        RaiseError($"The status of the EVSE '{evseId}' can not be stored: {result.ErrorResponse}", warnings);

                    return;

                }
            }

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
