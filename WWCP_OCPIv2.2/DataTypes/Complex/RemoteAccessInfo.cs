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
using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2
{

    public class RemoteAccessInfo
    {

        /// <summary>
        /// ISO-3166 alpha-2 country code of the country this party is operating in.
        /// </summary>
        [Mandatory]
        public CountryCode              CountryCode         { get; }

        /// <summary>
        /// CPO, eMSP (or other role) ID of this party (following the ISO-15118 standard).
        /// </summary>
        [Mandatory]
        public Party_Id                 PartyId             { get; }

        /// <summary>
        /// The type of the role.
        /// </summary>
        [Mandatory]
        public Roles                    Role                { get; }

        /// <summary>
        /// Business details of this party.
        /// </summary>
        [Mandatory]
        public BusinessDetails          BusinessDetails     { get; }

        [Mandatory]
        public AccessToken              Token               { get; }

        [Mandatory]
        public URL?                     VersionsURL         { get; }


        public IEnumerable<Version_Id>  VersionIds          { get; }

        [Mandatory]
        public RemoteAccessStatus       Status              { get; set; }


        public RemoteAccessInfo(CountryCode              CountryCode,
                                Party_Id                 PartyId,
                                Roles                    Role,
                                BusinessDetails          BusinessDetails,
                                AccessToken              Token,
                                URL?                     VersionsURL,
                                IEnumerable<Version_Id>  VersionIds   = null,
                                RemoteAccessStatus?      Status       = RemoteAccessStatus.ONLINE)
        {

            this.CountryCode      = CountryCode;
            this.PartyId          = PartyId;
            this.Role             = Role;
            this.BusinessDetails  = BusinessDetails;
            this.Token            = Token;
            this.VersionsURL      = VersionsURL;
            this.VersionIds       = VersionIds;
            this.Status           = Status ?? RemoteAccessStatus.ONLINE;

        }



        //public Credentials AsCredentials()

        //    => new Credentials(Token,
        //                       VersionsURL.Value,
        //                       Roles.Select(role => new CredentialsRole(role.CountryCode,
        //                                                                role.PartyId,
        //                                                                role.Role,
        //                                                                role.BusinessDetails)));

    }

}
