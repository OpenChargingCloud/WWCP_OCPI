/*
 * Copyright (c) 2015-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

namespace cloud.charging.open.protocols.OCPIv2_1_1
{

    /// <summary>
    /// Reservation restriction types.
    /// </summary>
    public enum ReservationRestrictionTypes
    {

        /// <summary>
        /// Used in tariff elements to describe costs for a reservation.
        /// </summary>
        RESERVATION,

        /// <summary>
        /// Used in tariff elements to describe costs for a reservation that expires
        /// (i.e. driver does not start a charging session before expiry_date of the reservation).
        /// </summary>
        RESERVATION_EXPIRES

    }

}
