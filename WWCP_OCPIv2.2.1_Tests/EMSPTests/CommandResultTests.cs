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
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2_1.UnitTests.EMSPTests
{

    /// <summary>
    /// EMSP #1 sends the CPO a command, and the CPO sends the result of it
    /// where the command says: to EMSP #1, which takes it.
    /// </summary>
    /// <remarks>
    /// The URL a command gave for its result named the version without its
    /// "v" for some commands - "2.x/emsp/commands/…" where EMSP #1 is at
    /// "v2.x/emsp/commands/…" - and every result of those was answered 404.
    /// </remarks>
    [TestFixture]
    public class CommandResultTests : ANodeTests
    {

        #region Data

        private static readonly Party_Idv3      cpoId          = Party_Idv3.From(CountryCode.Parse("DE"), Party_Id.Parse("GEF"));
        private static readonly Location_Id     locationId     = Location_Id. Parse("LOC0001");
        private static readonly EVSE_UId        evseUId        = EVSE_UId.    Parse("DE*GEF*E*LOC0001*1");
        private static readonly Connector_Id    connectorId    = Connector_Id.Parse("1");
        private static readonly Reservation_Id  reservationId  = Reservation_Id.Parse("R0001");

        #endregion


        #region TheResultOfAReservationComesBack()

        [Test]
        public Task TheResultOfAReservationComesBack()

            => TheResultComesBack("RESERVE_NOW",
                                  (cpo, commandId) => cpo.ReserveNow(AToken(),
                                                                     Timestamp.Now + TimeSpan.FromHours(1),
                                                                     reservationId,
                                                                     locationId,
                                                                     evseUId,
                                                                     CommandId: commandId));

        #endregion

        #region TheResultOfACancelledReservationComesBack()

        [Test]
        public Task TheResultOfACancelledReservationComesBack()

            => TheResultComesBack("CANCEL_RESERVATION",
                                  (cpo, commandId) => cpo.CancelReservation(reservationId,
                                                                            CommandId: commandId));

        #endregion

        #region TheResultOfAStartedSessionComesBack()

        [Test]
        public Task TheResultOfAStartedSessionComesBack()

            => TheResultComesBack("START_SESSION",
                                  (cpo, commandId) => cpo.StartSession(AToken(),
                                                                       locationId,
                                                                       evseUId,
                                                                       connectorId,
                                                                       CommandId: commandId));

        #endregion

        #region TheResultOfAStoppedSessionComesBack()

        [Test]
        public Task TheResultOfAStoppedSessionComesBack()

            => TheResultComesBack("STOP_SESSION",
                                  (cpo, commandId) => cpo.StopSession(Session_Id.Parse("S0001"),
                                                                      CommandId: commandId));

        #endregion

        #region TheResultOfAnUnlockedConnectorComesBack()

        [Test]
        public Task TheResultOfAnUnlockedConnectorComesBack()

            => TheResultComesBack("UNLOCK_CONNECTOR",
                                  (cpo, commandId) => cpo.UnlockConnector(locationId,
                                                                          evseUId,
                                                                          connectorId,
                                                                          CommandId: commandId));

        #endregion


        #region (private) TheResultComesBack(Command, Send)

        /// <summary>
        /// EMSP #1 sends the given command, the CPO accepts it and sends its
        /// result to the URL the command gives: EMSP #1 is to have it there.
        /// </summary>
        private async Task TheResultComesBack(String                                                Command,
                                              Func<EMSP.HTTP.EMSP2CPO_HTTPClient, Command_Id, Task>  Send)
        {

            URL? responseURL = null;

            Task<CommandResult> Accept(IOCPICommand command)
            {
                responseURL = command.ResponseURL;
                return Task.FromResult(new CommandResult(CommandResultTypes.ACCEPTED));
            }

            cpoCPOAPI!.OnReserveNowCommand         += (remotePartyId, from, to, command) => Accept(command);
            cpoCPOAPI!.OnCancelReservationCommand  += (remotePartyId, from, to, command) => Accept(command);
            cpoCPOAPI!.OnStartSessionCommand       += (remotePartyId, from, to, command) => Accept(command);
            cpoCPOAPI!.OnStopSessionCommand        += (remotePartyId, from, to, command) => Accept(command);
            cpoCPOAPI!.OnUnlockConnectorCommand    += (remotePartyId, from, to, command) => Accept(command);

            var cpo        = emsp1EMSPAPI?.GetCPOClient(
                                 CountryCode: cpoId.CountryCode,
                                 PartyId:     cpoId.PartyId
                             );

            Assert.That(cpo, Is.Not.Null);

            var commandId  = Command_Id.NewRandom();

            await Send(cpo!, commandId);

            CommandResult? result = null;

            for (var waited = 0; waited < 100 && result is null; waited++)
            {

                if (emsp1CommonAPI!.CommandValueStore.TryGetValue(commandId, out var values))
                    result = values.Result;

                if (result is null)
                    await Task.Delay(100);

            }

            Assert.Multiple(() => {
                Assert.That(responseURL?.Path.ToString(),  Does.EndWith($"/v{Version.Id}/emsp/commands/{Command}/{commandId}"), "The command gives no URL of EMSP #1's for its result.");
                Assert.That(result?.Result,                Is.EqualTo(CommandResultTypes.ACCEPTED),                              $"EMSP #1 did not take the result of {Command} at {responseURL}.");
            });

        }

        #endregion

        #region (private) AToken()

        /// <summary>
        /// An RFID token of EMSP #1's.
        /// </summary>
        private static Token AToken()

            => new (
                   CountryCode.Parse("DE"),
                   Party_Id.   Parse("GDF"),
                   Token_Id.   Parse("11223344"),
                   TokenType.RFID,
                   Contract_Id.Parse("DE-GDF-C12345678-X"),
                   "GraphDefined CA",
                   true,
                   WhitelistType.NEVER
               );

        #endregion

    }

}
