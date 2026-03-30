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

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using Hermod = org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPI
{

    public static class StatusCodes
    {

        public static readonly Int32  Success                       = 1000;
        // 19xx – Reserviert für custom success codes (1900–1999)


        // Client errors
        public static readonly Int32  GenericClientError            = 2000;
        public static readonly Int32  InvalidOrMissingParameters    = 2001; // (e.g. missing last_updated field in a PATCH request)
        public static readonly Int32  NotEnoughInformation          = 2002; // (e.g. too little information in an authorization request)
        public static readonly Int32  UnknownLocation               = 2003; // (e.g. START_SESSION with an unknown location)
        public static readonly Int32  UnknownToken                  = 2004; // (e.g. real-time authorization of an unknown token)
        // 29xx – Reserviert für custom client errors (2900–2999)


        // Server errors
        public static readonly Int32  GenericServerError            = 3000;
        public static readonly Int32  UnableToUseTheClientsAPI      = 3001; // (e.g. during credentials registration, if a GET request fails)
        public static readonly Int32  UnsupportedVersion            = 3002;
        public static readonly Int32  NoMatchingEndpoints           = 3003; // (during registration, if no common modules are available)
        // 39xx – Reserviert für custom server errors (3900–3999)


        // Hub errors
        public static readonly Int32  GenericHubError               = 4000;
        public static readonly Int32  UnknownReceiver               = 4001; // (TO-Adress unknown)
        public static readonly Int32  TimeoutOnForwardedRequest     = 4002;
        public static readonly Int32  ConnectionProblem             = 4003; // (Receiver not connected)
        // 49xx – Reserviert für custom hub errors (4900–4999)


        // Custom-Codes (19xx/29xx/39xx/49xx) must not be used in the standard OCPI modules to avoid conflicts.

    }

}
