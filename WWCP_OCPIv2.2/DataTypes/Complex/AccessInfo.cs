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

using System.Linq;
using System.Collections.Generic;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2
{

    public struct AccessInfo
    {

        public AccessToken                   Token          { get; }

        public URL?                          VersionsURL    { get; }

        public IEnumerable<CredentialsRole>  Roles          { get; }

        public AccessStatus                  Status         { get; set; }


        public AccessInfo(AccessToken                   Token,
                          AccessStatus                  Status,
                          URL?                          VersionsURL   = null,
                          IEnumerable<CredentialsRole>  Roles         = null)
        {

            this.Token        = Token;
            this.VersionsURL  = VersionsURL;
            this.Roles        = Roles?.Distinct();
            this.Status       = Status;

        }



        public Credentials AsCredentials()

            => new Credentials(Token,
                               VersionsURL.Value,
                               Roles.Select(role => new CredentialsRole(role.CountryCode,
                                                                        role.PartyId,
                                                                        role.Role,
                                                                        role.BusinessDetails)));

    }

}
