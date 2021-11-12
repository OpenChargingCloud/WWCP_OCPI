﻿/*
 * Copyright (c) 2015-2021 GraphDefined GmbH
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
    /// The category of an image to obtain the correct usage in an user presentation. Has to be set accordingly to the image content in order to guaranty the right usage.
    /// </summary>
    public enum ImageCategories
    {

        /// <summary>
        /// Photo of the physical device that contains one or more EVSEs.
        /// </summary>
        CHARGER,

        /// <summary>
        /// Location entrance photo. Should show the car entrance to the location from street side.
        /// </summary>
        ENTRANCE,

        /// <summary>
        /// Location overview photo.
        /// </summary>
        LOCATION,

        /// <summary>
        /// Logo of a associated roaming network to be displayed with the EVSE for example in lists, maps and detailed information view.
        /// </summary>
        NETWORK,

        /// <summary>
        /// Logo of the charge points operator, for example a municipal, to be displayed with the EVSEs detailed information view or in lists and maps, if no networkLogo is present.
        /// </summary>
        OPERATOR,

        /// <summary>
        /// Other.
        /// </summary>
        OTHER,

        /// <summary>
        /// Logo of the charge points owner, for example a local store, to be displayed with the EVSEs detailed information view.
        /// </summary>
        OWNER

    }

}
