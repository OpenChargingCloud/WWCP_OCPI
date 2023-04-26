﻿/*
 * Copyright (c) 2015-2023 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

namespace cloud.charging.open.protocols.OCPIv2_2_1
{

    /// <summary>
    /// The remote access information.
    /// </summary>
    public class RemoteAccessInfo
    {

        #region Properties

        [Mandatory]
        public AccessToken              AccessToken                  { get; }

        [Mandatory]
        public URL                      VersionsURL                  { get; }

        [Optional]
        public IEnumerable<Version_Id>  VersionIds                   { get; }

        [Optional]
        public Version_Id?              SelectedVersionId            { get; }

        [Mandatory]
        public Boolean                  AccessTokenBase64Encoding    { get; }

        [Mandatory]
        public RemoteAccessStatus       Status                       { get; set; }

        [Mandatory]
        public DateTime                 LastUpdate                   { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new remote access information.
        /// </summary>
        /// <param name="AccessToken">A remote access token.</param>
        /// <param name="VersionsURL">An OCPI vesions URL.</param>
        /// <param name="VersionIds">An optional enumeration of version identifications.</param>
        /// <param name="SelectedVersionId">A optional selected version identification.</param>
        /// <param name="AccessTokenBase64Encoding">Whether the access token is base64 encoded or not.</param>
        /// <param name="Status">A remote access status.</param>
        public RemoteAccessInfo(AccessToken               AccessToken,
                                URL                       VersionsURL,
                                IEnumerable<Version_Id>?  VersionIds                  = null,
                                Version_Id?               SelectedVersionId           = null,
                                Boolean?                  AccessTokenBase64Encoding   = null,
                                RemoteAccessStatus?       Status                      = RemoteAccessStatus.ONLINE)
        {

            this.AccessToken                = AccessToken;
            this.VersionsURL                = VersionsURL;
            this.VersionIds                 = VersionIds?.Distinct() ?? Array.Empty<Version_Id>();
            this.SelectedVersionId          = SelectedVersionId;
            this.AccessTokenBase64Encoding  = AccessTokenBase64Encoding ?? true;
            this.Status                     = Status                 ?? RemoteAccessStatus.ONLINE;
            this.LastUpdate                 = Timestamp.Now;

        }

        #endregion


        #region ToJSON(CustomBusinessDetailsSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomRemoteAccessInfoSerializer">A delegate to serialize custom remote access information JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<RemoteAccessInfo>?  CustomRemoteAccessInfoSerializer   = null)
        {

            var JSON = JSONObject.Create(

                           new JProperty("token",                    AccessToken.      ToString()),
                           new JProperty("versionsURL",              VersionsURL.      ToString()),

                           VersionIds.IsNeitherNullNorEmpty()
                               ? new JProperty("versionIds",         new JArray(VersionIds.Select(versionId => versionId.ToString())))
                               : null,

                           SelectedVersionId.HasValue
                               ? new JProperty("selectedVersionId",  SelectedVersionId.ToString())
                               : null,

                           new JProperty("status",                   Status.           ToString()),
                           new JProperty("lastUpdate",               LastUpdate.       ToIso8601())

                       );

            return CustomRemoteAccessInfoSerializer is not null
                       ? CustomRemoteAccessInfoSerializer(this, JSON)
                       : JSON;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this object.
        /// </summary>
        public RemoteAccessInfo Clone()

            => new(AccessToken.Clone,
                   VersionsURL.Clone,
                   VersionIds.Select(versionId => versionId.Clone).ToArray(),
                   SelectedVersionId,
                   AccessTokenBase64Encoding,
                   Status);

        #endregion

    }

}