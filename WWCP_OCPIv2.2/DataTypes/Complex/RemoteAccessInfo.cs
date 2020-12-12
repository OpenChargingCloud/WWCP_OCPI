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

    public class RemoteAccessInfo
    {

        [Mandatory]
        public AccessToken              AccessToken          { get; }

        [Mandatory]
        public URL                      VersionsURL          { get; }


        public IEnumerable<Version_Id>  VersionIds           { get; }


        public Version_Id?              SelectedVersionId    { get; }

        [Mandatory]
        public RemoteAccessStatus       Status               { get; set; }



        public RemoteAccessInfo(AccessToken              AccessToken,
                                URL                      VersionsURL,
                                IEnumerable<Version_Id>  VersionIds          = null,
                                Version_Id?              SelectedVersionId   = null,
                                RemoteAccessStatus?      Status              = RemoteAccessStatus.ONLINE)
        {

            this.AccessToken        = AccessToken;
            this.VersionsURL        = VersionsURL;
            this.VersionIds         = VersionIds;
            this.SelectedVersionId  = SelectedVersionId;
            this.Status             = Status ?? RemoteAccessStatus.ONLINE;

        }

        public JObject ToJSON()
        {

            return JSONObject.Create(
                       new JProperty("accesstoken",        AccessToken.      ToString()),
                       new JProperty("versionsURL",        VersionsURL.      ToString()),
                       new JProperty("versionIds",         new JArray(VersionIds.Select(versionId => versionId.ToString()))),
                       new JProperty("selectedVersionId",  SelectedVersionId.ToString()),
                       new JProperty("status",             Status.           ToString())
                   );

        }


    }

}
