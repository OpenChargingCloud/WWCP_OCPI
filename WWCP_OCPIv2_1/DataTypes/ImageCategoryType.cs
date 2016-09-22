/*
 * Copyright (c) 2015-2016 GraphDefined GmbH
 * This file is part of WWCP OCPI <https://github.com/GraphDefined/WWCP_OCPI>
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

namespace org.GraphDefined.WWCP.OCPIv2_1
{

    /// <summary>
    /// The category of an image to obtain the correct usage in an user presentation. Has to be set accordingly to the image content in order to guaranty the right usage.
    /// </summary>
    public enum ImageCategoryType
    {

        /// <summary>
        /// Photo of the physical device that contains one or more EVSEs.
        /// </summary>
        Charger     = 0,

        /// <summary>
        /// Location entrance photo. Should show the car entrance to the location from street side.
        /// </summary>
        Entrance    = 1,

        /// <summary>
        /// Location overview photo.
        /// </summary>
        Location    = 2,

        /// <summary>
        /// Logo of a associated roaming network to be displayed with the EVSE for example in lists, maps and detailed information view.
        /// </summary>
        Network     = 3,

        /// <summary>
        /// Logo of the charge points operator, for example a municipal, to be displayed with the EVSEs detailed information view or in lists and maps, if no networkLogo is present.
        /// </summary>
        Operator    = 4,

        /// <summary>
        /// Other.
        /// </summary>
        Other       = 5,

        /// <summary>
        /// Logo of the charge points owner, for example a local store, to be displayed with the EVSEs detailed information view.
        /// </summary>
        Owner       = 6

    }

}
