/*
 * Copyright (c) 2015 GraphDefined GmbH
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

namespace org.GraphDefined.WWCP.OCPI_2_0
{

    /// <summary>
    /// The format of the connector, whether it is a socket or a plug.
    /// </summary>
    public enum ConnectorFormatType
    {

        /// <summary>
        /// The connector is a socket; the EV user needs to bring a fitting plug.
        /// </summary>
        SOCKET,

        /// <summary>
        /// The connector is a attached cable; the EV users car needs to have a fitting inlet.
        /// </summary>
        CABLE

    }

}
