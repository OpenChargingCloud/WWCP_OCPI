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

using System.Collections.Concurrent;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_3_0
{

    public class PartyData(Party_Idv3       Id,
                           Role             Role,
                           BusinessDetails  BusinessDetails,
                           Boolean?         AllowDowngrades   = null)
    {

        public Party_Idv3       Id                 { get; } = Id;
        public Role             Role               { get; } = Role;
        public BusinessDetails  BusinessDetails    { get; } = BusinessDetails;
        public Boolean          AllowDowngrades    { get; } = AllowDowngrades ?? false;


        public ConcurrentDictionary<Location_Id,        Location>         Locations          { get; } =  [];
        public ConcurrentDictionary<Terminal_Id,        Terminal>         PaymentTerminals   { get; } =  [];
        public TimeRangeDictionary <Tariff_Id,          Tariff>           Tariffs            { get; } =  [];
        public ConcurrentDictionary<Session_Id,         Session>          Sessions           { get; } =  [];
        public ConcurrentDictionary<Token_Id,           TokenStatus>      Tokens             { get; } =  [];
        public ConcurrentDictionary<CDR_Id,             CDR>              CDRs               { get; } =  [];
        public ConcurrentDictionary<Booking_Id,         Booking>          Bookings           { get; } =  [];
        public ConcurrentDictionary<BookingLocation_Id, BookingLocation>  BookingLocations   { get; } =  [];

    }

}
