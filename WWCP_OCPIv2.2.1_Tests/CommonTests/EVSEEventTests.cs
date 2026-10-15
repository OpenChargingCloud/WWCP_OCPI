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
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2_1.UnitTests.CommonTests
{

    /// <summary>
    /// The EVSE events of the Common API tell the EVSE as it is now, its new status as the new one,
    /// and the time of the status rather than of the call.
    /// </summary>
    /// <remarks>
    /// AddOrUpdateEVSE gave the old status as the new one and the time of the call. AddOrUpdateLocation
    /// gave the EVSE as it was before, never told of EVSEs added to a location stored before, and of no
    /// change of status at all. An EVSE stored as REMOVED was removed from a location no longer stored.
    /// Removed EVSEs are not kept here, which the Common API does by default.
    /// </remarks>
    [TestFixture]
    public class EVSEEventTests
    {

        #region Data

        private static readonly Party_Idv3      party     = Party_Idv3.From(CountryCode.Parse("DE"), Party_Id.Parse("GEF"));
        private static readonly Location_Id     id        = Location_Id.Parse("1");
        private static readonly EVSE_UId        uid1      = EVSE_UId.   Parse("DE*GEF*E1*1");
        private static readonly EVSE_UId        uid2      = EVSE_UId.   Parse("DE*GEF*E1*2");
        private static readonly DateTimeOffset  t0        = DateTimeOffset.Parse("2026-10-15T08:00:00Z");

        private String     directory  = default!;
        private CommonAPI  api        = default!;

        private readonly List<(DateTimeOffset Timestamp, EVSE EVSE, StatusType New, StatusType? Old)>  statusChanges  = [];
        private readonly List<EVSE>                                                                       changed        = [];
        private readonly List<EVSE>                                                                       added          = [];
        private readonly List<EVSE>                                                                       removed        = [];

        #endregion

        #region SetUp / TearDown

        [SetUp]
        public void SetUp()
        {

            directory  = Path.Combine(Path.GetTempPath(), $"WWCP_OCPI_Tests-{Guid.NewGuid():N}");

            Directory.CreateDirectory(directory);

            api        = new CommonAPI(
                             OurPartyData:      [ new PartyData(party, Role.CPO, new BusinessDetails("GraphDefined CSO")) ],
                             DefaultPartyId:    party,
                             BaseAPI:           new CommonHTTPAPI(
                                                    HTTPAPI:         new HTTPExtAPI(HTTPServer: new HTTPServer(TCPPort: IPPort.Parse(3998))),
                                                    OurBaseURL:      URL.Parse("http://127.0.0.1:3998/ocpi"),
                                                    OurVersionsURL:  URL.Parse("http://127.0.0.1:3998/ocpi/versions"),
                                                    RootPath:        HTTPPath.Parse("/ocpi"),
                                                    DisableLogging:  true,
                                                    LoggingPath:     directory
                                                ),
                             URLPathPrefix:     HTTPPath.Parse("/ocpi/v2.2.1"),
                             KeepRemovedEVSEs:  evse => false,
                             DatabaseFilePath:  directory,
                             DisableLogging:    true,
                             LoggingPath:       directory
                         );

            statusChanges.Clear();
            changed.      Clear();
            added.        Clear();
            removed.      Clear();

            api.OnEVSEStatusChanged += (timestamp, evse, newStatus, oldStatus) => { statusChanges.Add((timestamp, evse, newStatus, oldStatus)); return Task.CompletedTask; };
            api.OnEVSEChanged       += evse => { changed.Add(evse); return Task.CompletedTask; };
            api.OnEVSEAdded         += evse => { added.  Add(evse); return Task.CompletedTask; };
            api.OnEVSERemoved       += evse => { removed.Add(evse); return Task.CompletedTask; };

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

        #region (private static) AnEVSE(UId, Status, LastUpdated, MaxPower) / ALocation(LastUpdated, EVSEs)

        private static EVSE AnEVSE(EVSE_UId        UId,
                                   StatusType      Status,
                                   DateTimeOffset  LastUpdated,
                                   Int32           MaxPower = 22000)

            => new (UId,
                    Status,
                    [ new Connector(Connector_Id.Parse("1"),
                                    ConnectorType.IEC_62196_T2,
                                    ConnectorFormats.SOCKET,
                                    PowerTypes.AC_3_PHASE,
                                    Volt.  FromV(400),
                                    Ampere.FromA(32),
                                    Watt.  FromW(MaxPower),
                                    LastUpdated: LastUpdated) ],
                    EVSE_Id.Parse(UId.ToString()),
                    Created:      t0,
                    LastUpdated:  LastUpdated);

        private static Location ALocation(DateTimeOffset  LastUpdated,
                                          params EVSE[]   EVSEs)

            => new (party.CountryCode,
                    party.PartyId,
                    id,
                    true,
                    "Biberweg 18",
                    "Jena",
                    Country.Germany,
                    GeoCoordinate.Parse(50.92, 11.58),
                    "Europe/Berlin",
                    EVSEs:        EVSEs,
                    Created:      t0,
                    LastUpdated:  LastUpdated);

        private Location Stored()
            => api.TryGetLocation(party, id, out var location) ? location : throw new Exception("The location is not stored!");

        #endregion


        #region AddOrUpdateEVSE_TellsTheNewStatusAndItsTime()

        [Test]
        public async Task AddOrUpdateEVSE_TellsTheNewStatusAndItsTime()
        {

            Assert.That((await api.AddOrUpdateLocation(ALocation(t0, AnEVSE(uid1, StatusType.AVAILABLE, t0)))).IsSuccess);

            statusChanges.Clear();

            var charging = AnEVSE(uid1, StatusType.CHARGING, t0.AddMinutes(1));

            Assert.That((await api.AddOrUpdateEVSE(Stored(), charging)).IsSuccess);

            Assert.Multiple(() => {
                Assert.That(statusChanges,                                   Has.Count.EqualTo(1));
                Assert.That(statusChanges.Single().Timestamp,                Is.EqualTo(t0.AddMinutes(1)));
                Assert.That(statusChanges.Single().New,                      Is.EqualTo(StatusType.CHARGING));
                Assert.That(statusChanges.Single().Old,                      Is.EqualTo(StatusType.AVAILABLE));
                Assert.That(statusChanges.Single().EVSE.Status,              Is.EqualTo(StatusType.CHARGING));
                Assert.That(changed.Last().Status,                           Is.EqualTo(StatusType.CHARGING));
                Assert.That(Stored().GetEVSE(uid1)?.Status,                  Is.EqualTo(StatusType.CHARGING));
                Assert.That(Stored().LastUpdated,                            Is.EqualTo(t0.AddMinutes(1)));
            });

        }

        #endregion

        #region AddOrUpdateEVSE_RemovesARemovedEVSEFromTheStoredLocation()

        [Test]
        public async Task AddOrUpdateEVSE_RemovesARemovedEVSEFromTheStoredLocation()
        {

            Assert.That((await api.AddOrUpdateLocation(ALocation(t0, AnEVSE(uid1, StatusType.AVAILABLE, t0),
                                                                     AnEVSE(uid2, StatusType.AVAILABLE, t0)))).IsSuccess);

            Assert.That((await api.AddOrUpdateEVSE(Stored(), AnEVSE(uid2, StatusType.REMOVED, t0.AddMinutes(1)))).IsSuccess);

            Assert.Multiple(() => {
                Assert.That(Stored().EVSEs.Select(evse => evse.UId),         Is.EqualTo(new[] { uid1 }));
                Assert.That(removed.Select(evse => evse.UId),                Is.EqualTo(new[] { uid2 }));
                Assert.That(changed,                                         Is.Empty);
            });

        }

        #endregion

        #region AddOrUpdateEVSE_NeverTurnsTheLocationBack()

        [Test]
        public async Task AddOrUpdateEVSE_NeverTurnsTheLocationBack()
        {

            Assert.That((await api.AddOrUpdateLocation(ALocation(t0.AddMinutes(5), AnEVSE(uid1, StatusType.AVAILABLE, t0),
                                                                                   AnEVSE(uid2, StatusType.AVAILABLE, t0.AddMinutes(5))))).IsSuccess);

            Assert.That((await api.AddOrUpdateEVSE(Stored(), AnEVSE(uid1, StatusType.CHARGING, t0.AddMinutes(1)))).IsSuccess);

            Assert.Multiple(() => {
                Assert.That(Stored().GetEVSE(uid1)?.Status,                  Is.EqualTo(StatusType.CHARGING));
                Assert.That(Stored().LastUpdated,                            Is.EqualTo(t0.AddMinutes(5)));
            });

        }

        #endregion

        #region AddOrUpdateLocation_TellsTheEVSEsAsTheyAreNow()

        [Test]
        public async Task AddOrUpdateLocation_TellsTheEVSEsAsTheyAreNow()
        {

            Assert.That((await api.AddOrUpdateLocation(ALocation(t0, AnEVSE(uid1, StatusType.AVAILABLE, t0)))).IsSuccess);

            statusChanges.Clear();
            added.        Clear();

            Assert.That((await api.AddOrUpdateLocation(ALocation(t0.AddMinutes(1), AnEVSE(uid1, StatusType.CHARGING,  t0.AddMinutes(1), MaxPower: 11000),
                                                                                   AnEVSE(uid2, StatusType.AVAILABLE, t0.AddMinutes(1))))).IsSuccess);

            Assert.Multiple(() => {

                Assert.That(changed.Select(evse => evse.UId),                Is.EqualTo(new[] { uid1 }));
                Assert.That(changed.Single().Status,                         Is.EqualTo(StatusType.CHARGING));
                Assert.That(changed.Single().Connectors.Single().MaxElectricPower?.Value, Is.EqualTo(11000));

                Assert.That(added.Select(evse => evse.UId),                  Is.EqualTo(new[] { uid2 }));

                Assert.That(statusChanges.Select(change => (change.EVSE.UId, change.New, change.Old, change.Timestamp)),
                            Is.EquivalentTo(new (EVSE_UId, StatusType, StatusType?, DateTimeOffset)[] {
                                (uid1, StatusType.CHARGING,  StatusType.AVAILABLE, t0.AddMinutes(1)),
                                (uid2, StatusType.AVAILABLE, null,                 t0.AddMinutes(1))
                            }));

            });

        }

        #endregion

        #region TryPatchEVSE_TellsTheNewStatusAndItsTime()

        [Test]
        public async Task TryPatchEVSE_TellsTheNewStatusAndItsTime()
        {

            Assert.That((await api.AddOrUpdateLocation(ALocation(t0, AnEVSE(uid1, StatusType.AVAILABLE, t0)))).IsSuccess);

            statusChanges.Clear();

            var stored = Stored();
            var result = await api.TryPatchEVSE(stored,
                                                stored.GetEVSE(uid1)!,
                                                new Newtonsoft.Json.Linq.JObject(
                                                    new Newtonsoft.Json.Linq.JProperty("status",       "CHARGING"),
                                                    new Newtonsoft.Json.Linq.JProperty("last_updated", t0.AddMinutes(1).ToISO8601())
                                                ));

            Assert.That(result.IsSuccess, result.ErrorResponse);

            Assert.Multiple(() => {
                Assert.That(statusChanges,                                   Has.Count.EqualTo(1));
                Assert.That(statusChanges.Single().Timestamp,                Is.EqualTo(t0.AddMinutes(1)));
                Assert.That(statusChanges.Single().New,                      Is.EqualTo(StatusType.CHARGING));
                Assert.That(statusChanges.Single().Old,                      Is.EqualTo(StatusType.AVAILABLE));
                Assert.That(statusChanges.Single().EVSE.Status,              Is.EqualTo(StatusType.CHARGING));
                Assert.That(Stored().GetEVSE(uid1)?.Status,                  Is.EqualTo(StatusType.CHARGING));
            });

        }

        #endregion

    }

}
