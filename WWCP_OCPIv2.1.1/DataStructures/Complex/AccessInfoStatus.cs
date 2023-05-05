///*
// * Copyright (c) 2015-2023 GraphDefined GmbH
// * This file is part of WWCP OCPI <https://github.com/OpenChargingCloud/WWCP_OCPI>
// *
// * Licensed under the Apache License, Version 2.0 (the "License");
// * you may not use this file except in compliance with the License.
// * You may obtain a copy of the License at
// *
// *     http://www.apache.org/licenses/LICENSE-2.0
// *
// * Unless required by applicable law or agreed to in writing, software
// * distributed under the License is distributed on an "AS IS" BASIS,
// * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// * See the License for the specific language governing permissions and
// * limitations under the License.
// */

//#region Usings

//using Newtonsoft.Json.Linq;

//using org.GraphDefined.Vanaheimr.Illias;

//#endregion

//namespace cloud.charging.open.protocols.OCPIv2_1_1
//{

//    public readonly struct AccessInfoStatus
//    {

//        #region Properties

//        /// <summary>
//        /// The access token.
//        /// </summary>
//        [Mandatory]
//        public AccessToken   AccessToken                   { get; }

//        /// <summary>
//        /// Whether the access token is base64 encoded or not.
//        /// </summary>
//        [Mandatory]
//        public Boolean       AccessTokenIsBase64Encoded    { get; }

//        /// <summary>
//        /// The access status.
//        /// </summary>
//        [Mandatory]
//        public AccessStatus  Status                        { get; }

//        #endregion

//        #region Properties

//        /// <summary>
//        /// Create a new access info status.
//        /// </summary>
//        /// <param name="AccessToken">An access token.</param>
//        /// <param name="Status">An access status.</param>
//        public AccessInfoStatus(AccessToken   AccessToken,
//                                AccessStatus  Status)

//            : this (AccessToken,
//                    true,
//                    Status)

//        { }


//        /// <summary>
//        /// Create a new access info status.
//        /// </summary>
//        /// <param name="AccessToken">An access token.</param>
//        /// <param name="AccessTokenIsBase64Encoded">Whether the access token is base64 encoded or not.</param>
//        /// <param name="Status">An access status.</param>
//        public AccessInfoStatus(AccessToken   AccessToken,
//                                Boolean       AccessTokenIsBase64Encoded,
//                                AccessStatus  Status)
//        {

//            this.AccessToken                 = AccessToken;
//            this.AccessTokenIsBase64Encoded  = AccessTokenIsBase64Encoded;
//            this.Status                      = Status;

//        }

//        #endregion



//        #region ToJSON(CustomAccessInfoStatusSerializer = null)

//        /// <summary>
//        /// Return a JSON representation of this object.
//        /// </summary>
//        /// <param name="CustomAccessInfoStatusSerializer">A delegate to serialize custom access information status JSON objects.</param>
//        public JObject ToJSON(CustomJObjectSerializerDelegate<AccessInfoStatus>? CustomAccessInfoStatusSerializer = null)
//        {

//            var json = JSONObject.Create(
//                           new JProperty("accessToken",                 AccessToken.ToString()),
//                           new JProperty("accessTokenIsBase64Encoded",  AccessTokenIsBase64Encoded),
//                           new JProperty("status",                      Status.     ToString())
//                       );

//            return CustomAccessInfoStatusSerializer is not null
//                       ? CustomAccessInfoStatusSerializer(this, json)
//                       : json;

//        }

//        #endregion




//        #region Clone()

//        /// <summary>
//        /// Clone this object.
//        /// </summary>
//        public AccessInfoStatus Clone()

//            => new (AccessToken.Clone,
//                    AccessTokenIsBase64Encoded,
//                    Status);

//        #endregion


//    }

//}
