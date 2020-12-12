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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2
{

    public class Party
    {

        #region Properties

        /// <summary>
        /// ISO-3166 alpha-2 country code of the country this party is operating in.
        /// </summary>
        [Mandatory]
        public CountryCode              CountryCode          { get; }

        /// <summary>
        /// CPO, eMSP (or other role) ID of this party (following the ISO-15118 standard).
        /// </summary>
        [Mandatory]
        public Party_Id                 PartyId              { get; }

        /// <summary>
        /// The type of the role.
        /// </summary>
        [Mandatory]
        public Roles                    Role                 { get; }

        /// <summary>
        /// Business details of this party.
        /// </summary>
        [Mandatory]
        public BusinessDetails          BusinessDetails      { get; }

        /// <summary>
        /// The current status of the party.
        /// </summary>
        public PartyStatus              PartyStatus          { get; }


        private readonly List<AccessInfo2> _AccessInfo;

        public IEnumerable<AccessInfo2> AccessInfo
            => _AccessInfo;


        private readonly List<RemoteAccessInfo> _RemoteAccessInfos;

        public IEnumerable<RemoteAccessInfo> RemoteAccessInfos
            => _RemoteAccessInfos;

        #endregion

        #region Constructor(s)

        public Party(CountryCode              CountryCode,
                     Party_Id                 PartyId,
                     Roles                    Role,
                     BusinessDetails          BusinessDetails,
                     PartyStatus              PartyStatus         = PartyStatus.ENABLED,

                     AccessToken?             AccessToken         = null,
                     AccessStatus             AccessStatus        = AccessStatus.ALLOWED,

                     AccessToken?             RemoteAccessToken   = null,
                     URL?                     RemoteVersionsURL   = null,
                     IEnumerable<Version_Id>  RemoteVersionIds    = null,
                     Version_Id?              SelectedVersionId   = null,
                     RemoteAccessStatus?      RemoteStatus        = RemoteAccessStatus.ONLINE)

        {

            this.CountryCode         = CountryCode;
            this.PartyId             = PartyId;
            this.Role                = Role;
            this.BusinessDetails     = BusinessDetails;
            this.PartyStatus         = PartyStatus;

            this._AccessInfo         = new List<AccessInfo2>();
            this._RemoteAccessInfos  = new List<RemoteAccessInfo>();

            if (AccessToken.HasValue)
            {
                _AccessInfo.Add(new AccessInfo2(AccessToken.Value,
                                                AccessStatus));
            }

            if (RemoteAccessToken.HasValue &&
                RemoteVersionsURL.HasValue)
            {
                _RemoteAccessInfos.Add(new RemoteAccessInfo(
                                           RemoteAccessToken.Value,
                                           RemoteVersionsURL.Value,
                                           RemoteVersionIds,
                                           SelectedVersionId,
                                           RemoteStatus
                                       ));
            }

        }

        public Party(CountryCode                    CountryCode,
                     Party_Id                       PartyId,
                     Roles                          Role,
                     BusinessDetails                BusinessDetails,

                     IEnumerable<AccessInfo2>       AccessInfos,
                     IEnumerable<RemoteAccessInfo>  RemoteAccessInfos,

                     PartyStatus                    PartyStatus   = PartyStatus.ENABLED)
        {

            this.CountryCode         = CountryCode;
            this.PartyId             = PartyId;
            this.Role                = Role;
            this.BusinessDetails     = BusinessDetails;
            this.PartyStatus         = PartyStatus;

            this._AccessInfo         = AccessInfos.      IsNullOrEmpty() ? new List<AccessInfo2>     (AccessInfos)       : new List<AccessInfo2>();
            this._RemoteAccessInfos  = RemoteAccessInfos.IsNullOrEmpty() ? new List<RemoteAccessInfo>(RemoteAccessInfos) : new List<RemoteAccessInfo>();

        }

        #endregion



        public JObject ToJSON()
        {

            var JSON = JSONObject.Create(

                           new JProperty("countryCode",              CountryCode.    ToString()),
                           new JProperty("partyId",                  PartyId.        ToString()),
                           new JProperty("role",                     Role.           ToString()),

                           BusinessDetails != null
                               ? new JProperty("businessDetails",    BusinessDetails.ToJSON())
                               : null,

                           new JProperty("partyStatus",              PartyStatus.    ToString()),

                           _AccessInfo.SafeAny()
                               ? new JProperty("accessInfos",        new JArray(_AccessInfo.       SafeSelect(accessInfo       => accessInfo.      ToJSON())))
                               : null,

                           _RemoteAccessInfos.SafeAny()
                               ? new JProperty("remoteAccessInfos",  new JArray(_RemoteAccessInfos.SafeSelect(remoteAccessInfo => remoteAccessInfo.ToJSON())))
                               : null

                       );

            return JSON;

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
