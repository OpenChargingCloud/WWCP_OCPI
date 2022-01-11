/*
 * Copyright (c) 2015-2022 GraphDefined GmbH
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

namespace cloud.charging.open.protocols.OCPIv2_1_1
{

    /// <summary>
    /// Reflects the general type of the charge points location. May be used for user information.
    /// </summary>
    public enum ParkingTypes
    {

        /// <summary>
        /// Parking location type is not known by the operator (default).
        /// </summary>
        UNKNOWN,

        /// <summary>
        /// Location on a parking facility/rest area along a motorway, freeway, interstate, highway etc.
        /// </summary>
        ALONG_MOTORWAY,

        /// <summary>
        /// Multistorey car park.
        /// </summary>
        PARKING_GARAGE,

        /// <summary>
        /// A cleared area that is intended for parking vehicles, i.e.at super markets, bars, etc.
        /// </summary>
        PARKING_LOT,

        /// <summary>
        /// Location is on the driveway of a house/building.
        /// </summary>
        ON_DRIVEWAY,

        /// <summary>
        /// Parking in public space.
        /// </summary>
        ON_STREET,

        /// <summary>
        /// Multistorey car park, mainly underground.
        /// </summary>
        UNDERGROUND_GARAGE

    }

}
