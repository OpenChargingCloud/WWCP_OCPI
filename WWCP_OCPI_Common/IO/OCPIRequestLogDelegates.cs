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

using org.GraphDefined.Vanaheimr.Hermod.HTTPTest;

#endregion

namespace cloud.charging.open.protocols.OCPI
{

    public delegate Task OCPIRequestLoggerDelegate(String       LoggingPath,
                                                   String       Context,
                                                   String       LogEventName,
                                                   OCPIRequest  Request);

    /// <summary>
    /// The delegate for OCPI/HTTP request logs.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the incoming request.</param>
    /// <param name="OCPIAPI">The sending OCPI/HTTP API.</param>
    /// <param name="Request">The incoming request.</param>
    public delegate Task OCPIRequestLogHandler(DateTimeOffset     Timestamp,
                                               HTTPAPIX           OCPIAPI,
                                               OCPIRequest        Request,
                                               CancellationToken  CancellationToken);

}
