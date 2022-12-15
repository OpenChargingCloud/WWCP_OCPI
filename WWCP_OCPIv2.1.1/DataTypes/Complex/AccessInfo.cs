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

#region Usings

using System;
using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_1_1
{

    public readonly struct AccessInfo2
    {

        public AccessToken?  Token     { get; }
        public AccessStatus  Status    { get; }


        public AccessInfo2(AccessToken? Token,
                           AccessStatus Status)
        {

            this.Token   = Token;
            this.Status  = Status;

        }

        public JObject ToJSON()
        {
            return JSONObject.Create(
                       new JProperty("token",   Token. ToString()),
                       new JProperty("status",  Status.ToString())
                   );
        }

    }



    public struct AccessInfo
    {

        public AccessToken                  Token          { get; }

        public URL?                         VersionsURL    { get; }

        /// <summary>
        /// ISO-3166 alpha-2 country code of the country this party is operating in.
        /// </summary>
        [Mandatory]
        public CountryCode                  CountryCode        { get; }

        /// <summary>
        /// CPO, eMSP (or other role) ID of this party (following the ISO-15118 standard).
        /// </summary>
        [Mandatory]
        public Party_Id                     PartyId            { get; }

        /// <summary>
        /// Business details of this party.
        /// </summary>
        [Mandatory]
        public BusinessDetails              BusinessDetails    { get; }


        public AccessStatus                 Status         { get; set; }


        public AccessInfo(AccessToken                   Token,
                          AccessStatus                  Status,
                          CountryCode                   CountryCode,
                          Party_Id                      PartyId,
                          BusinessDetails               BusinessDetails,
                          URL?                          VersionsURL   = null)
        {

            this.Token            = Token;
            this.VersionsURL      = VersionsURL;
            this.CountryCode      = CountryCode;
            this.PartyId          = PartyId;
            this.BusinessDetails  = BusinessDetails;
            this.Status           = Status;

        }

        public JObject ToJSON()
        {

            return JSONObject.Create(
                       new JProperty("accesstoken",      Token.          ToString()),
                       new JProperty("versionsURL",      VersionsURL.    ToString()),
                       new JProperty("countryCode",      CountryCode.    ToString()),
                       new JProperty("partyId",          PartyId.        ToString()),
                       new JProperty("businessDetails",  BusinessDetails.ToJSON()),
                       new JProperty("status",           Status.         ToString())
                );

        }

        public Credentials AsCredentials()

            => new Credentials(Token,
                               VersionsURL.Value,
                               BusinessDetails,
                               CountryCode,
                               PartyId);

    }

}
