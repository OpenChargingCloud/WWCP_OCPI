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

    public static class AccessInfoextentions
    {

        public static Boolean Is(this AccessInfo?  AccessInfo,
                                 Roles             Role)

            => AccessInfo.HasValue &&
               AccessInfo.Value.Is(Role);

        public static Boolean IsNot(this AccessInfo?  AccessInfo,
                                    Roles             Role)

            => !AccessInfo.HasValue ||
                AccessInfo.Value.IsNot(Role);

    }

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
            this.Roles        = Roles?.Distinct() ?? new CredentialsRole[0];
            this.Status       = Status;

        }

        public JObject ToJSON()
        {

            return JSONObject.Create(
                       new JProperty("accesstoken", Token.      ToString()),
                       new JProperty("versionsURL", VersionsURL.ToString()),
                       new JProperty("roles",       new JArray(Roles.Select(role => role.ToJSON()))),
                       new JProperty("status",      Status.     ToString())
                );

        }

        public Credentials AsCredentials()

            => new Credentials(Token,
                               VersionsURL.Value,
                               Roles.Select(role => new CredentialsRole(role.CountryCode,
                                                                        role.PartyId,
                                                                        role.Role,
                                                                        role.BusinessDetails)));


        public Boolean Is(Roles Role)
            => Roles.Any(role => role.Role == Role);

        public Boolean IsNot(Roles Role)
            => !Roles.Any(role => role.Role == Role);

    }

}
