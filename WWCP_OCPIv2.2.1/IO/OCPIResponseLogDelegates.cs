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

#region Usings

using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2_1.HTTP
{

    public delegate Task OCPIResponseLoggerDelegate(String        LoggingPath,
                                                    String        Context,
                                                    String        LogEventName,
                                                    OCPIRequest   Request,
                                                    OCPIResponse  Response);

    /// <summary>
    /// The delegate for OCPI/HTTP response logs.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the incoming request.</param>
    /// <param name="OCPIAPI">The sending OCPI/HTTP API.</param>
    /// <param name="Request">The incoming OCPI/HTTP request.</param>
    /// <param name="Response">The outgoing OCPI/HTTP response.</param>
    public delegate Task OCPIResponseLogHandler(DateTimeOffset  Timestamp,
                                                HTTPAPI         OCPIAPI,
                                                OCPIRequest     Request,
                                                OCPIResponse    Response);

}
