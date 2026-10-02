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

using System.Diagnostics;
using System.Net.Security;
using System.Collections.Concurrent;
using System.Security.Authentication;
using System.Runtime.CompilerServices;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography.X509Certificates;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2_1
{

    /// <summary>
    /// The assets an EMSP or a hub keeps of its remote CPOs - their locations,
    /// tariffs, sessions and charge detail records - read back from the file
    /// of the assets of its Common API, which they are written to.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The Common API's own replay passes over these lines: its parties are
    /// its own, and a remote CPO is none of them.
    /// </para>
    /// <para>
    /// The charge detail records of the remote CPOs were written with the
    /// Common API's own commands before. Their add, update and remove lines
    /// are taken as well, for a CDR of a remote CPO - the Common API passes
    /// over them, as it does not know that party. A removeAll line of before
    /// cannot be told from the Common API's own, and is left to it.
    /// </para>
    /// </remarks>
    internal static class RemoteCPOAssets
    {

        #region Read   (CommonAPI, RemoteCPOs, CancellationToken = default)

        /// <summary>
        /// Every remote CPO and every asset of one, as the file of the assets
        /// of the given Common API has them.
        /// </summary>
        /// <param name="CommonAPI">The Common API whose file of the assets is read.</param>
        /// <param name="RemoteCPOs">The remote CPOs, with their assets.</param>
        /// <param name="CancellationToken">An optional cancellation token.</param>
        public static async Task Read(CommonAPI                                    CommonAPI,
                                      ConcurrentDictionary<Party_Idv3, PartyData>  RemoteCPOs,
                                      CancellationToken                            CancellationToken   = default)
        {

            try
            {

                await foreach (var command in CommonAPI.BaseAPI.LoadCommandsFromDatabaseFile(
                                                  CommonAPI.AssetsDBFileName,
                                                  CancellationToken
                                              ))
                {
                    Process(command, RemoteCPOs);
                }

            }
            catch (FileNotFoundException)
            { }
            catch (DirectoryNotFoundException)
            { }

        }

        #endregion

        #region Process(Command, RemoteCPOs)

        /// <summary>
        /// One line of the file of the assets: what it says of a remote CPO.
        /// Every other line is the Common API's own, and is passed over.
        /// </summary>
        public static void Process(Command                                      command,
                                   ConcurrentDictionary<Party_Idv3, PartyData>  RemoteCPOs)
        {

            String?       errorResponse   = null;
            Location?     location;
            Tariff?       tariff;
            Session?      session;
            CDR?          cdr;

            var errorResponses = new List<Tuple<Command, String>>();

            switch (command.CommandName)
            {

                #region addRemoteCPO

                // AddRemoteCPO wrote down only the identification before: such
                // a line cannot make the remote CPO again, and is passed over.
                case CommonHTTPAPI.addRemoteCPO:
                    try
                    {
                        if (command.JSONObject is not null &&
                            CommonAPI.TryParsePartyJSON(
                                command.JSONObject,
                                out var partyData,
                                out errorResponse
                            ))
                        {
                            RemoteCPOs.TryAdd(partyData.Id, partyData);
                        }
                    }
                    catch (Exception e)
                    {
                        errorResponse ??= e.Message;
                    }
                    if (errorResponse is not null)
                        errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                    break;

                #endregion


                #region addRemoteLocation

                case CommonHTTPAPI.addRemoteLocation:
                    try
                    {
                        if (command.JSONObject is not null &&
                            Location.TryParse(
                                         command.JSONObject,
                                         out location,
                                         out errorResponse
                                     ) &&
                            RemoteCPOs. TryGetValue(
                                         Party_Idv3.From(
                                             location.CountryCode,
                                             location.PartyId
                                         ),
                                         out var party
                                     ))
                        {
                            party.Locations.TryAdd(
                                location.Id,
                                location
                            );
                        }
                    }
                    catch (Exception e)
                    {
                        errorResponse ??= e.Message;
                    }
                    if (errorResponse is not null)
                        errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                    break;

                #endregion

                #region addRemoteLocationIfNotExists

                case CommonHTTPAPI.addRemoteLocationIfNotExists:
                    try
                    {
                        if (command.JSONObject is not null &&
                            Location.TryParse(
                                         command.JSONObject,
                                         out location,
                                         out errorResponse
                                     ) &&
                            RemoteCPOs. TryGetValue(
                                         Party_Idv3.From(
                                             location.CountryCode,
                                             location.PartyId
                                         ),
                                         out var party
                                     ))
                        {
                            party.Locations.TryAdd(
                                location.Id,
                                location
                            );
                        }
                    }
                    catch (Exception e)
                    {
                        errorResponse ??= e.Message;
                    }
                    if (errorResponse is not null)
                        errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                    break;

                #endregion

                #region addOrUpdateRemoteLocation

                case CommonHTTPAPI.addOrUpdateRemoteLocation:
                    try
                    {
                        if (command.JSONObject is not null &&
                            Location.TryParse(
                                         command.JSONObject,
                                         out location,
                                         out errorResponse
                                     ) &&
                            RemoteCPOs. TryGetValue(
                                         Party_Idv3.From(
                                             location.CountryCode,
                                             location.PartyId
                                         ),
                                         out var party
                                     ))
                        {

                            if (party.Locations.ContainsKey(location.Id))
                                party.Locations.Remove(location.Id, out _);

                            party.Locations.TryAdd(location.Id, location);

                        }
                    }
                    catch (Exception e)
                    {
                        errorResponse ??= e.Message;
                    }
                    if (errorResponse is not null)
                        errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                    break;

                #endregion

                #region updateRemoteLocation

                case CommonHTTPAPI.updateRemoteLocation:
                    try
                    {
                        if (command.JSONObject is not null &&
                            Location.TryParse(
                                         command.JSONObject,
                                         out location,
                                         out errorResponse
                                     ) &&
                            RemoteCPOs. TryGetValue(
                                         Party_Idv3.From(
                                             location.CountryCode,
                                             location.PartyId
                                         ),
                                         out var party
                                     ))
                        {
                            party.Locations.Remove(location.Id, out _);
                            party.Locations.TryAdd(location.Id, location);
                        }
                    }
                    catch (Exception e)
                    {
                        errorResponse ??= e.Message;
                    }
                    if (errorResponse is not null)
                        errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                    break;

                #endregion

                #region removeRemoteLocation

                case CommonHTTPAPI.removeRemoteLocation:
                    try
                    {
                        if (command.JSONObject is not null &&
                            Location.TryParse(
                                         command.JSONObject,
                                         out location,
                                         out errorResponse
                                     ) &&
                            RemoteCPOs. TryGetValue(
                                         Party_Idv3.From(
                                             location.CountryCode,
                                             location.PartyId
                                         ),
                                         out var party
                                     ))
                        {
                            party.Locations.Remove(location.Id, out _);
                        }
                    }
                    catch (Exception e)
                    {
                        errorResponse ??= e.Message;
                    }
                    if (errorResponse is not null)
                        errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                    break;

                #endregion

                #region removeAllRemoteLocations

                case CommonHTTPAPI.removeAllRemoteLocations:
                    foreach (var party in RemoteCPOs.Values)
                        party.Locations.Clear();
                    break;

                #endregion


                #region addRemoteTariff

                case CommonHTTPAPI.addRemoteTariff:
                    try
                    {
                        if (command.JSONObject is not null &&
                            Tariff. TryParse(
                                        command.JSONObject,
                                        out tariff,
                                        out errorResponse
                                    ) &&
                            RemoteCPOs.TryGetValue(
                                        Party_Idv3.From(
                                            tariff.CountryCode,
                                            tariff.PartyId
                                        ),
                                        out var party
                                    ))
                        {
                            party.Tariffs.TryAdd(tariff.Id, tariff);
                        }
                    }
                    catch (Exception e)
                    {
                        errorResponse ??= e.Message;
                    }
                    if (errorResponse is not null)
                        errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                    break;

                #endregion

                #region addRemoteTariffIfNotExists

                case CommonHTTPAPI.addRemoteTariffIfNotExists:
                    try
                    {
                        if (command.JSONObject is not null &&
                            Tariff. TryParse(
                                        command.JSONObject,
                                        out tariff,
                                        out errorResponse
                                    ) &&
                            RemoteCPOs.TryGetValue(
                                        Party_Idv3.From(
                                            tariff.CountryCode,
                                            tariff.PartyId
                                        ),
                                        out var party
                                    ))
                        {
                            party.Tariffs.TryAdd(tariff.Id, tariff);
                        }
                    }
                    catch (Exception e)
                    {
                        errorResponse ??= e.Message;
                    }
                    if (errorResponse is not null)
                        errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                    break;

                #endregion

                #region addOrUpdateRemoteTariff

                case CommonHTTPAPI.addOrUpdateRemoteTariff:
                    try
                    {
                        if (command.JSONObject is not null &&
                            Tariff. TryParse(
                                        command.JSONObject,
                                        out tariff,
                                        out errorResponse
                                    ) &&
                            RemoteCPOs.TryGetValue(
                                        Party_Idv3.From(
                                            tariff.CountryCode,
                                            tariff.PartyId
                                        ),
                                        out var party
                                    ))
                        {

                            if (party.Tariffs.ContainsKey(tariff.Id))
                                party.Tariffs.Remove(tariff.Id);

                            party.Tariffs.TryAdd(tariff.Id, tariff);

                        }
                    }
                    catch (Exception e)
                    {
                        errorResponse ??= e.Message;
                    }
                    if (errorResponse is not null)
                        errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                    break;

                #endregion

                #region updateRemoteTariff

                case CommonHTTPAPI.updateRemoteTariff:
                    try
                    {
                        if (command.JSONObject is not null &&
                            Tariff. TryParse(
                                        command.JSONObject,
                                        out tariff,
                                        out errorResponse
                                    ) &&
                            RemoteCPOs.TryGetValue(
                                        Party_Idv3.From(
                                            tariff.CountryCode,
                                            tariff.PartyId
                                        ),
                                        out var party
                                    ))
                        {
                            party.Tariffs.Remove(tariff.Id);
                            party.Tariffs.TryAdd(tariff.Id, tariff);
                        }
                    }
                    catch (Exception e)
                    {
                        errorResponse ??= e.Message;
                    }
                    if (errorResponse is not null)
                        errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                    break;

                #endregion

                #region removeRemoteTariff

                case CommonHTTPAPI.removeRemoteTariff:
                    try
                    {
                        if (command.JSONObject is not null &&
                            Tariff. TryParse(
                                        command.JSONObject,
                                        out tariff,
                                        out errorResponse
                                    ) &&
                            RemoteCPOs.TryGetValue(
                                        Party_Idv3.From(
                                            tariff.CountryCode,
                                            tariff.PartyId
                                        ),
                                        out var party
                                    ))
                        {
                            party.Tariffs.Remove(tariff.Id);
                        }
                    }
                    catch (Exception e)
                    {
                        errorResponse ??= e.Message;
                    }
                    if (errorResponse is not null)
                        errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                    break;

                #endregion

                #region removeAllRemoteTariffs

                case CommonHTTPAPI.removeAllRemoteTariffs:
                    foreach (var party in RemoteCPOs.Values)
                        party.Tariffs.Clear();
                    break;

                #endregion


                #region addRemoteSession

                case CommonHTTPAPI.addRemoteSession:
                    try
                    {
                        if (command.JSONObject is not null &&
                            Session.TryParse(
                                        command.JSONObject,
                                        out session,
                                        out errorResponse
                                    ) &&
                            RemoteCPOs.TryGetValue(
                                        Party_Idv3.From(
                                            session.CountryCode,
                                            session.PartyId
                                        ),
                                        out var party
                                    ))
                        {
                            party.Sessions.TryAdd(session.Id, session);
                        }
                    }
                    catch (Exception e)
                    {
                        errorResponse ??= e.Message;
                    }
                    if (errorResponse is not null)
                        errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                    break;

                #endregion

                #region addRemoteSessionIfNotExists

                case CommonHTTPAPI.addRemoteSessionIfNotExists:
                    try
                    {
                        if (command.JSONObject is not null &&
                            Session.TryParse(
                                        command.JSONObject,
                                        out session,
                                        out errorResponse
                                    ) &&
                            RemoteCPOs.TryGetValue(
                                        Party_Idv3.From(
                                            session.CountryCode,
                                            session.PartyId
                                        ),
                                        out var party
                                    ))
                        {
                            party.Sessions.TryAdd(session.Id, session);
                        }
                    }
                    catch (Exception e)
                    {
                        errorResponse ??= e.Message;
                    }
                    if (errorResponse is not null)
                        errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                    break;

                #endregion

                #region addOrUpdateRemoteSession

                case CommonHTTPAPI.addOrUpdateRemoteSession:
                    try
                    {
                        if (command.JSONObject is not null &&
                            Session.TryParse(
                                        command.JSONObject,
                                        out session,
                                        out errorResponse
                                    ) &&
                            RemoteCPOs.TryGetValue(
                                        Party_Idv3.From(
                                            session.CountryCode,
                                            session.PartyId
                                        ),
                                        out var party
                                    ))
                        {

                            if (party.Sessions.ContainsKey(session.Id))
                                party.Sessions.Remove(session.Id, out _);

                            party.Sessions.TryAdd(session.Id, session);

                        }
                    }
                    catch (Exception e)
                    {
                        errorResponse ??= e.Message;
                    }
                    if (errorResponse is not null)
                        errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                    break;

                #endregion

                #region updateRemoteSession

                case CommonHTTPAPI.updateRemoteSession:
                    try
                    {
                        if (command.JSONObject is not null &&
                            Session.TryParse(
                                        command.JSONObject,
                                        out session,
                                        out errorResponse
                                    ) &&
                            RemoteCPOs.TryGetValue(
                                        Party_Idv3.From(
                                            session.CountryCode,
                                            session.PartyId
                                        ),
                                        out var party
                                    ))
                        {
                            party.Sessions.Remove(session.Id, out _);
                            party.Sessions.TryAdd(session.Id, session);
                        }
                    }
                    catch (Exception e)
                    {
                        errorResponse ??= e.Message;
                    }
                    if (errorResponse is not null)
                        errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                    break;

                #endregion

                #region removeRemoteSession

                case CommonHTTPAPI.removeRemoteSession:
                    try
                    {
                        if (command.JSONObject is not null &&
                            Session.TryParse(
                                        command.JSONObject,
                                        out session,
                                        out errorResponse
                                    ) &&
                            RemoteCPOs.TryGetValue(
                                        Party_Idv3.From(
                                            session.CountryCode,
                                            session.PartyId
                                        ),
                                        out var party
                                    ))
                        {
                            party.Sessions.Remove(session.Id, out _);
                        }
                    }
                    catch (Exception e)
                    {
                        errorResponse ??= e.Message;
                    }
                    if (errorResponse is not null)
                        errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                    break;

                #endregion

                #region removeAllRemoteSessions

                case CommonHTTPAPI.removeAllRemoteSessions:
                    foreach (var party in RemoteCPOs.Values)
                        party.Sessions.Clear();
                    break;

                #endregion


                #region addRemoteChargeDetailRecord

                case CommonHTTPAPI.addRemoteChargeDetailRecord:
                case CommonHTTPAPI.addChargeDetailRecord:
                    try
                    {
                        if (command.JSONObject is not null &&
                            CDR.    TryParse(
                                        command.JSONObject,
                                        out cdr,
                                        out errorResponse
                                    ) &&
                            RemoteCPOs.TryGetValue(
                                        Party_Idv3.From(
                                            cdr.CountryCode,
                                            cdr.PartyId
                                        ),
                                        out var party
                                    ))
                        {
                            party.CDRs.TryAdd(cdr.Id, cdr);
                        }
                    }
                    catch (Exception e)
                    {
                        errorResponse ??= e.Message;
                    }
                    if (errorResponse is not null)
                        errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                    break;

                #endregion

                #region addRemoteChargeDetailRecordIfNotExists

                case CommonHTTPAPI.addRemoteChargeDetailRecordIfNotExists:
                case CommonHTTPAPI.addChargeDetailRecordIfNotExists:
                    try
                    {
                        if (command.JSONObject is not null &&
                            CDR.    TryParse(
                                        command.JSONObject,
                                        out cdr,
                                        out errorResponse
                                    ) &&
                            RemoteCPOs.TryGetValue(
                                        Party_Idv3.From(
                                            cdr.CountryCode,
                                            cdr.PartyId
                                        ),
                                        out var party
                                    ))
                        {
                            party.CDRs.TryAdd(cdr.Id, cdr);
                        }
                    }
                    catch (Exception e)
                    {
                        errorResponse ??= e.Message;
                    }
                    if (errorResponse is not null)
                        errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                    break;

                #endregion

                #region addOrUpdateRemoteChargeDetailRecord

                case CommonHTTPAPI.addOrUpdateRemoteChargeDetailRecord:
                case CommonHTTPAPI.addOrUpdateChargeDetailRecord:
                    try
                    {
                        if (command.JSONObject is not null &&
                            CDR.    TryParse(
                                        command.JSONObject,
                                        out cdr,
                                        out errorResponse
                                    ) &&
                            RemoteCPOs.TryGetValue(
                                        Party_Idv3.From(
                                            cdr.CountryCode,
                                            cdr.PartyId
                                        ),
                                        out var party
                                    ))
                        {

                            if (party.CDRs.ContainsKey(cdr.Id))
                                party.CDRs.Remove(cdr.Id, out _);

                            party.CDRs.TryAdd(cdr.Id, cdr);

                        }
                    }
                    catch (Exception e)
                    {
                        errorResponse ??= e.Message;
                    }
                    if (errorResponse is not null)
                        errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                    break;

                #endregion

                #region updateRemoteChargeDetailRecord

                case CommonHTTPAPI.updateRemoteChargeDetailRecord:
                case CommonHTTPAPI.updateChargeDetailRecord:
                    try
                    {
                        if (command.JSONObject is not null &&
                            CDR.    TryParse(
                                        command.JSONObject,
                                        out cdr,
                                        out errorResponse
                                    ) &&
                            RemoteCPOs.TryGetValue(
                                        Party_Idv3.From(
                                            cdr.CountryCode,
                                            cdr.PartyId
                                        ),
                                        out var party
                                    ))
                        {
                            party.CDRs.Remove(cdr.Id, out _);
                            party.CDRs.TryAdd(cdr.Id, cdr);
                        }
                    }
                    catch (Exception e)
                    {
                        errorResponse ??= e.Message;
                    }
                    if (errorResponse is not null)
                        errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                    break;

                #endregion

                #region removeRemoteChargeDetailRecord

                case CommonHTTPAPI.removeRemoteChargeDetailRecord:
                case CommonHTTPAPI.removeChargeDetailRecord:
                    try
                    {
                        if (command.JSONObject is not null &&
                            CDR.    TryParse(
                                        command.JSONObject,
                                        out cdr,
                                        out errorResponse
                                    ) &&
                            RemoteCPOs.TryGetValue(
                                        Party_Idv3.From(
                                            cdr.CountryCode,
                                            cdr.PartyId
                                        ),
                                        out var party
                                    ))
                        {
                            party.CDRs.Remove(cdr.Id, out _);
                        }
                    }
                    catch (Exception e)
                    {
                        errorResponse ??= e.Message;
                    }
                    if (errorResponse is not null)
                        errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                    break;

                #endregion

                #region removeAllRemoteChargeDetailRecords

                case CommonHTTPAPI.removeAllRemoteChargeDetailRecords:
                    foreach (var party in RemoteCPOs.Values)
                        party.CDRs.Clear();
                    break;

                #endregion


            }

        }

        #endregion

    }

}
