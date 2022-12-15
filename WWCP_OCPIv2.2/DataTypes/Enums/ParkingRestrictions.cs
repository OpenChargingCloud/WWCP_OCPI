/*
 * Copyright (c) 2015-2022 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

namespace cloud.charging.open.protocols.OCPIv2_2
{

    /// <summary>
    /// This value, if provided, represents the restriction to the parking spot for different purposes.
    /// </summary>
    public enum ParkingRestrictions
    {

        /// <summary>
        /// Reserved parking spot for electric vehicles.
        /// </summary>
        EV_ONLY,

        /// <summary>
        /// Parking allowed only while plugged in (charging).
        /// </summary>
        PLUGGED,

        /// <summary>
        /// Reserved parking spot for disabled people with valid ID.
        /// </summary>
        DISABLED,

        /// <summary>
        /// Parking spot for customers/guests only, for example in case of a hotel or shop.
        /// </summary>
        CUSTOMERS,

        /// <summary>
        /// Parking spot only suitable for (electric) motorcycles or scooters.
        /// </summary>
        MOTORCYCLES

    }

}
