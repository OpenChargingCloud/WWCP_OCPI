/*
 * Copyright (c) 2015-2023 GraphDefined GmbH
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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_1_1
{

    public struct AccessInfo
    {

        public AccessToken       Token              { get; }
        public AccessStatus      Status             { get; set; }
        public URL?              VersionsURL        { get; }
        public BusinessDetails?  BusinessDetails    { get; }
        public CountryCode       CountryCode        { get; }
        public Party_Id          PartyId            { get; }
        public Roles             Role               { get; }

        public AccessInfo(AccessToken       Token,
                          AccessStatus      Status,
                          CountryCode       CountryCode,
                          Party_Id          PartyId,
                          Roles             Role,
                          URL?              VersionsURL       = null,
                          BusinessDetails?  BusinessDetails   = null)
        {

            this.Token            = Token;
            this.Status           = Status;
            this.CountryCode      = CountryCode;
            this.PartyId          = PartyId;
            this.Role             = Role;
            this.VersionsURL      = VersionsURL;
            this.BusinessDetails  = BusinessDetails;

        }

        public JObject ToJSON()

            => JSONObject.Create(

                         new JProperty("accesstoken",      Token.          ToString()),
                         new JProperty("status",           Status.         ToString()),
                         new JProperty("countryCode",      CountryCode.    ToString()),
                         new JProperty("partyId",          PartyId.        ToString()),
                         new JProperty("role",             Role.           AsText  ()),

                   VersionsURL.HasValue
                       ? new JProperty("versionsURL",      VersionsURL.    ToString())
                       : null,

                   BusinessDetails is not null
                       ? new JProperty("businessDetails",  BusinessDetails.ToJSON())
                       : null

               );

        public Credentials? AsCredentials

            => VersionsURL.    HasValue &&
               BusinessDetails is not null

                   ? new Credentials(Token,
                                     VersionsURL.Value,
                                     BusinessDetails,
                                     CountryCode,
                                     PartyId)

                   : null;

    }

}
