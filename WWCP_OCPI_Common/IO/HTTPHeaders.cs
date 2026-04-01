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

using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPI
{

    /// <summary>
    /// Common OCPI HTTP header field names.
    /// </summary>
    public static class HTTPHeaders
    {

        public const String X_Request_ID            = "X-Request-ID";
        public const String X_Correlation_ID        = "X-Correlation-ID";

        public const String OCPI_From_Country_Code  = "ocpi-from-country-code";
        public const String OCPI_From_PartyId       = "ocpi-from-party-id";
        public const String OCPI_To_Country_Code    = "ocpi-to-country-code";
        public const String OCPI_To_PartyId         = "ocpi-to-party-id";

    }

}
