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

        // The status and 'last updated' of the EVSE itself: AddOrUpdateEVSE gives the old status as the new one,
        // and the time of the call rather than of the status.
        private Task EVSEStatusChanged(DateTimeOffset  Timestamp,
                                       EVSE            EVSE,
                                       StatusType      NewStatus,
                                       StatusType?     OldStatus)
        {
            work.Writer.TryWrite(() => { ImportStatus(EVSE, EVSE.Status, EVSE.LastUpdated); return Task.CompletedTask; });
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
                           (operatorJSON, warnings) => {

                               var pools     = POI3Mapper.ArrayOf(operatorJSON, "chargingPools");

                               var poolId    = POI3Mapper.ToPOI3PoolId(operatorId.Value, Location.Id)?.ToString();
                               var existing  = pools.OfType<JObject>().FirstOrDefault(pool => pool["@id"]?.Value<String>() == poolId);
                               var desired   = Location.ToPOI3(operatorId.Value, existing, Language, warnings);

                               if (desired is null)
                                   return false;

                               if (existing is not null)
                                   existing.Replace(desired);
                               else
                                   pools.Add(desired);

                               return true;

                           });

            // The status of every EVSE, e.g. of new ones, which start as available.
            foreach (var evse in Location.EVSEs)
                ImportStatus(evse, evse.Status, evse.LastUpdated);

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
                           (operatorJSON, warnings) => {

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

            ChangeOperator(operatorId.Value,
                           $"OCPI tariff {Tariff.Id}",
                           Tariff.LastUpdated,
                           (operatorJSON, warnings) => {

                               var chargingTariff = Tariff.ToPOI3(operatorId.Value, Language, warnings);

                               if (chargingTariff is null)
                                   return false;

                               var tariffs   = POI3Mapper.ArrayOf(operatorJSON, "chargingTariffs");

                               var existing  = tariffs.OfType<JObject>().FirstOrDefault(tariff => tariff["@id"]?.Value<String>() == chargingTariff.Id.ToString());
                               var desired   = chargingTariff.ToJSON();

                               if (existing is not null)
                               {
                                   // What only POI3 knows is kept.
                                   var merged = (JObject) existing.DeepClone();
                                   foreach (var property in desired.Properties().Where(property => property.Name is not "created" and not "lastChange"))
                                       merged[property.Name] = property.Value.DeepClone();
                                   POI3Mapper.Overlay(merged, desired, [ "name", "description", "tariffURL", "energyMix" ]);
                                   existing.Replace(merged);
                               }
                               else
                                   tariffs.Add(desired);

                               return true;

                           });

        }

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
                           (operatorJSON, warnings) => {

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

        #region (private) ChangeOperator(OperatorId, Description, CreatedAt, Change)

        /// <summary>
        /// Change the JSON of the given operator in the head, and commit the difference, signed.
        /// </summary>
        private void ChangeOperator(POI.ChargingStationOperator_Id                  OperatorId,
                                    String                                          Description,
                                    DateTimeOffset                                  CreatedAt,
                                    Func<JObject, ICollection<Warning>, Boolean>    Change)
        {

            var head          = History.Head.RoamingNetwork;
            var networkJSON   = head.ToJSON();
            var operatorJSON  = (networkJSON["chargingStationOperators"] as JArray ?? []).
                                    OfType<JObject>().
                                    FirstOrDefault(op => op["@id"]?.Value<String>() == OperatorId.ToString());

            if (operatorJSON is null)
            {
                RaiseError($"The operator '{OperatorId}' is not in the roaming network: adding it is up to its operator!", []);
                return;
            }

            var warnings = new List<Warning>();

            if (!Change(operatorJSON, warnings))
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
