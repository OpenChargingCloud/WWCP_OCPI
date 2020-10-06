/*
 * Copyright (c) 2015-2020 GraphDefined GmbH
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

#region Usings

using System;
using System.Linq;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2
{

    public static class JSON_IO
    {

        //public static JObject CreateResponse(JArray     Data,
        //                                     Int32      StatusCode,
        //                                     String     StatusMessage,
        //                                     DateTime?  Timestamp  = null)

        //    => JSONObject.Create(
        //           new JProperty("data",            Data),
        //           new JProperty("status_code",     StatusCode),
        //           new JProperty("status_message",  StatusMessage),
        //           new JProperty("timestamp",       (Timestamp ?? DateTime.UtcNow).ToIso8601())
        //       );

        //public static JObject CreateResponse(JObject    Data,
        //                                     Int32      StatusCode,
        //                                     String     StatusMessage,
        //                                     DateTime?  Timestamp  = null)

        //    => JSONObject.Create(
        //           new JProperty("data",            Data),
        //           new JProperty("status_code",     StatusCode),
        //           new JProperty("status_message",  StatusMessage),
        //           new JProperty("timestamp",       (Timestamp ?? DateTime.UtcNow).ToIso8601())
        //       );

    }

}
