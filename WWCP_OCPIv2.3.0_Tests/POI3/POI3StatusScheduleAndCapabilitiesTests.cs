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

using NUnit.Framework;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Aegir;
using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;
using cloud.charging.open.protocols.OCPIv2_3_0.POI3;
using cloud.charging.open.protocols.WWCP.POI.tests;

using POI = cloud.charging.open.protocols.WWCP.POI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_3_0.UnitTests.POI3
{

    /// <summary>
    /// The planned status of POI3 EVSEs as the OCPI status schedule, and the capabilities of OCPI EVSEs imported onto
    /// existing charging pools.
    /// </summary>
    [TestFixture]
    public class POI3StatusScheduleAndCapabilitiesTests
    {

        #region Data

        private static readonly DateTimeOffset                  Created    = DateTimeOffset.Parse("2026-01-01T00:00:00Z");

        private static readonly POI.ChargingStationOperator_Id  GEF        = POI.ChargingStationOperator_Id.Parse("DE*GEF");
        private static readonly POI.ChargingPool_Id             P1         = POI.ChargingPool_Id.           Parse("DE*GEF*P1");
        private static readonly POI.ChargingStation_Id          S1         = POI.ChargingStation_Id.        Parse("DE*GEF*S1");
        private static readonly POI.EVSE_Id                     E1         = POI.EVSE_Id.                   Parse("DE*GEF*E1*1");

        private static readonly Party_Idv3                      Party      = Party_Idv3.From(CountryCode.Parse("DE"), Party_Id.Parse("GEF"));
        private static readonly Location_Id                     Location1  = Location_Id.Parse("1");

        private String     directory  = default!;
        private CommonAPI  api        = default!;

        #endregion

        #region SetUp / TearDown

        [SetUp]
        public void SetUp()
        {
            directory  = Path.Combine(Path.GetTempPath(), $"WWCP_OCPI_POI3_ScheduleTests-{Guid.NewGuid():N}");
            Directory.CreateDirectory(directory);
            api        = new CommonAPI(
                             OurPartyData:      [ new PartyData(Party, Role.CPO, new BusinessDetails("GraphDefined CSO")) ],
                             DefaultPartyId:    Party,
                             BaseAPI:           new CommonHTTPAPI(
                                                    HTTPAPI:         new HTTPExtAPI(HTTPServer: new HTTPServer(TCPPort: IPPort.Parse(3996))),
                                                    OurBaseURL:      URL.Parse("http://127.0.0.1:3996/ocpi"),
                                                    OurVersionsURL:  URL.Parse("http://127.0.0.1:3996/ocpi/versions"),
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

        #region (private static) ANetwork()

        /// <summary>
        /// A pool authenticating by RFID with its card types, remotely and by credit card, which it also takes as a
        /// payment, and offering a QR code; its station is reservable.
        /// </summary>
        private static POI.RoamingNetwork ANetwork()

            => new POI.RoamingNetwork(POI.RoamingNetwork_Id.Parse("rn1"),
                                      Created:                   Created,
                                      ChargingStationOperators:  [ new POI.ChargingStationOperator(GEF,
                                                                                                   Name:           I18NString.Create(Languages.en, "GraphDefined"),
                                                                                                   ChargingPools:  [ new POI.ChargingPool(P1,
                                                                                                                                          I18NString.Create(Languages.en, "Biberweg"),
                                                                                                                                          Address:              new Address("Biberweg", "07749", I18NString.Create(Languages.de, "Jena"), Country.Germany, HouseNumber: "18"),
                                                                                                                                          GeoLocation:          GeoCoordinate.Parse(50.92, 11.58),
                                                                                                                                          TimeZone:             Time_Zone.Parse("Europe/Berlin"),
                                                                                                                                          AuthenticationModes:  [ POI.AuthenticationMode.RFID([ POI.RFIDCardTypes.MifareClassic ]),
                                                                                                                                                                  POI.AuthenticationMode.REMOTE,
                                                                                                                                                                  POI.AuthenticationMode.CreditCard ],
                                                                                                                                          PaymentOptions:       [ POI.PaymentOptions.CreditCard ],
                                                                                                                                          UIFeatures:           [ POI.UIFeatures.CreditCard ],
                                                                                                                                          ChargingStations:     [ new POI.ChargingStation(S1,
                                                                                                                                                                                          Features:  [ POI.ChargingStationFeature.Reservable ],
                                                                                                                                                                                          EVSEs:     [ new POI.EVSE(E1,
                                                                                                                                                                                                                    CurrentTypes:        POI.CurrentTypes.AC_ThreePhases,
                                                                                                                                                                                                                    MaxPower:            Watt.FromW(22000),
                                                                                                                                                                                                                    ChargingConnectors:  [ new POI.ChargingConnector(POI.ChargingConnector_Id.Parse("1"), POI.ChargingConnectorType.IEC_62196_T2) ],
                                                                                                                                                                                                                    Created:             Created) ],
                                                                                                                                                                                          Created:   Created) ],
                                                                                                                                          Created:              Created) ],
                                                                                                   Created:        Created) ]).WithKeys();

        #endregion

        #region (private static) Eventually(Condition) / Plan(History, Status, At)

        private static async Task<Boolean> Eventually(Func<Boolean> Condition)
        {

            for (var i = 0; i < 200; i++)
            {
                if (Condition())
                    return true;
                await Task.Delay(25);
            }

            return Condition();

        }

        private static void Plan(POI.RoamingNetworkHistory  History,
                                 String                     Status,
                                 DateTimeOffset             At)
        {
            var result = History.RuntimeStatus.TryUpdate(POI.POIRuntimeStatusKind.Status, nameof(POI.EVSE), E1.ToString(), Status, At, out _, out var errorResponse);
            Assert.That(result, Is.Not.EqualTo(POI.POIStatusUpdateResult.Refused), errorResponse);
        }

        private static (DateTimeOffset, String)[] PlannedOf(POI.RoamingNetworkHistory History)

            => [.. History.Head.RoamingNetwork.GetChargingStationOperatorById(GEF)!.EVSEs.Single().
                       StatusSchedule(timestamp => timestamp > Timestamp.Now).
                       Select (status => (status.Timestamp, status.Value.ToString())).
                       OrderBy(status => status.Timestamp) ];

        #endregion


        #region PlannedStatusIsTheStatusSchedule()

        [Test]
        public void PlannedStatusIsTheStatusSchedule()
        {

            var history  = POI.RoamingNetworkHistory.Start(ANetwork());
            var now      = Timestamp.Now;
            var t1       = new DateTimeOffset(now.Year, now.Month, now.Day, now.Hour, 0, 0, TimeSpan.Zero).AddDays(1);

            // Inoperative for two hours, then available again, blocked from the evening on.
            Plan(history, "inoperative", t1);
            Plan(history, "available",   t1.AddHours(2));
            Plan(history, "blocked",     t1.AddHours(6));

            var head      = history.Head.RoamingNetwork;
            var warnings  = new List<Warning>();
            var location  = head.GetChargingStationOperatorById(GEF)!.EffectiveChargingPools.Single().ToOCPI(head, warnings)!;
            var schedule  = location.EVSEs.Single().StatusSchedule.ToArray();

            Assert.Multiple(() => {
                Assert.That(location.EVSEs.Single().Status,                       Is.EqualTo(StatusType.AVAILABLE));
                Assert.That(schedule.Select(period => (period.Status, period.Begin, period.End)),
                            Is.EqualTo(new (StatusType, DateTimeOffset, DateTimeOffset?)[] {
                                           (StatusType.INOPERATIVE, t1,              t1.AddHours(2)),
                                           (StatusType.BLOCKED,     t1.AddHours(6),  null)
                                       }));
            });

        }

        #endregion

        #region TheStatusScheduleIsPlannedAndPublishedBack()

        [Test]
        public async Task TheStatusScheduleIsPlannedAndPublishedBack()
        {

            var history  = POI.RoamingNetworkHistory.Start(ANetwork());
            var errors   = new List<String>();
            var now      = Timestamp.Now;
            var t1       = new DateTimeOffset(now.Year, now.Month, now.Day, now.Hour, 0, 0, TimeSpan.Zero).AddDays(1);

            void OnError(Object Sender, String Description, IEnumerable<Warning> Warnings)
            {
                lock (errors)
                    errors.Add(Description + " " + String.Join(" ", Warnings.Select(warning => warning.Text.FirstText())));
            }

            // A hub publishing and importing the same history.
            await using var importer  = new OCPIToPOI3Importer(api, history, changeSet => changeSet.SignedByAll(history.Head.RoamingNetwork));
            importer.OnError += OnError;
            importer.Start();

            await using var publisher = new POI3ToOCPIPublisher(history, api);
            publisher.OnError += OnError;
            await publisher.StartAsync();

            // What a CPO tells: inoperative for two hours tomorrow.
            Assert.That(api.TryGetLocation(Party, Location1, out var location), Is.True);

            var evse     = location!.EVSEs.Single();
            var planned  = new EVSE(evse.UId,
                                    evse.Status,
                                    evse.Connectors,
                                    evse.EVSEId,
                                    StatusSchedule:  [ new StatusSchedule(StatusType.INOPERATIVE, t1, t1.AddHours(2)) ],
                                    Capabilities:    evse.Capabilities,
                                    LastUpdated:     evse.LastUpdated.AddMinutes(1));

            Assert.That((await api.AddOrUpdateEVSE(location, planned)).IsSuccess);

            Assert.That(await Eventually(() => PlannedOf(history).Length == 2), Is.True, String.Join("\n", errors));

            // Published back as it was told.
            Assert.That(await Eventually(() => api.TryGetLocation(Party, Location1, out var published) &&
                                               published.EVSEs.Single().StatusSchedule.Count() == 1),
                        Is.True);

            api.TryGetLocation(Party, Location1, out var stored);
            await importer.ImportAsync(stored!);

            Assert.Multiple(() => {
                Assert.That(PlannedOf(history),  Is.EqualTo(new[] { (t1, POI.EVSEStatusType.Inoperative.ToString()), (t1.AddHours(2), POI.EVSEStatusType.Available.ToString()) }));
                Assert.That(stored!.EVSEs.Single().StatusSchedule.Select(period => (period.Status, period.Begin, period.End)),
                            Is.EqualTo(new (StatusType, DateTimeOffset, DateTimeOffset?)[] { (StatusType.INOPERATIVE, t1, t1.AddHours(2)) }));
            });

            lock (errors)
                Assert.That(errors, Is.Empty);

        }

        #endregion

        #region CapabilitiesOfExistingPoolsAreImported()

        [Test]
        public async Task CapabilitiesOfExistingPoolsAreImported()
        {

            var network   = ANetwork();
            var history   = POI.RoamingNetworkHistory.Start(network);
            var revision  = history.Head.Commit.Revision;
            var errors    = new List<String>();

            await using var importer = new OCPIToPOI3Importer(api, history, changeSet => changeSet.SignedByAll(history.Head.RoamingNetwork));
            importer.OnError += (sender, description, warnings) => {
                lock (errors)
                    errors.Add(description + " " + String.Join(" ", warnings.Select(warning => warning.Text.FirstText())));
            };

            importer.Start();

            var warnings  = new List<Warning>();
            var json      = network.GetChargingStationOperatorById(GEF)!.ChargingPools.Single().ToOCPI(network, warnings)!.ToJSON();

            Location WithCapabilities(params String[] Capabilities)
            {
                var changed = (JObject) json.DeepClone();
                changed["evses"]![0]!["capabilities"] = new JArray(Capabilities);
                changed["last_updated"]               = DateTimeOffset.Parse("2026-10-17T12:00:00Z").ToISO8601();
                return Location.Parse(changed);
            }

            var told      = Location.Parse(json).EVSEs.Single().Capabilities.Select(capability => capability.ToString()).ToArray();

            // As it was published: nothing changes.
            await importer.ImportAsync(Location.Parse(json));

            Assert.That(history.Head.Commit.Revision, Is.EqualTo(revision), String.Join("\n", errors));

            // No longer reservable nor payable by credit card, but with a PIN pad and unlockable by the CSO.
            await importer.ImportAsync(WithCapabilities([ .. told.Where(capability => capability is not "RESERVABLE" and not "CREDIT_CARD_PAYABLE"), "PED_TERMINAL", "UNLOCK_CAPABLE" ]));

            var pool      = history.Head.RoamingNetwork.GetChargingStationOperatorById(GEF)!.ChargingPools.Single();

            Assert.Multiple(() => {

                Assert.That(told,                                                          Is.EquivalentTo(new[] { "RFID_READER", "REMOTE_START_STOP_CAPABLE", "CREDIT_CARD_PAYABLE", "RESERVABLE" }));
                Assert.That(history.Head.Commit.Revision,                                  Is.EqualTo(revision + 1), String.Join("\n", errors));

                // The RFID mode keeps its card types; what gave the credit card capability is gone.
                Assert.That(pool.AuthenticationModes.Select(mode => mode.Type),            Is.EquivalentTo(new[] { POI.AuthenticationMode.RFIDType, POI.AuthenticationMode.REMOTEType, POI.AuthenticationMode.PINPADType }));
                Assert.That(pool.AuthenticationModes.Single(mode => mode.Type == POI.AuthenticationMode.RFIDType).ToJSON().ToString(),
                            Is.EqualTo(POI.AuthenticationMode.RFID([ POI.RFIDCardTypes.MifareClassic ]).ToJSON().ToString()));
                Assert.That(pool.PaymentOptions,                                           Is.Empty);
                Assert.That(pool.UIFeatures,                                               Is.Empty);
                Assert.That(pool.ChargingStations.Single().Features,                       Is.EqualTo(new[] { POI.ChargingStationFeature.CSOUnlockSupported }));

            });

            // Told as before: nothing changes.
            await importer.ImportAsync(WithCapabilities([ .. told.Where(capability => capability is not "RESERVABLE" and not "CREDIT_CARD_PAYABLE"), "PED_TERMINAL", "UNLOCK_CAPABLE" ]));

            Assert.That(history.Head.Commit.Revision, Is.EqualTo(revision + 1));

            lock (errors)
                Assert.That(errors, Is.Empty);

        }

        #endregion

    }

}
