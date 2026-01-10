/*
 * Copyright (c) 2015-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPI <https://github.com/OpenChargingCloud/WWCP_OCPI>
 *
 * Licensed under the Affero GPL license, Version 3.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.gnu.org/licenses/agpl.html
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#region Usings

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv3_0
{

    /// <summary>
    /// A generic async request.
    /// </summary>
    public abstract class AAsyncRequest<TRequest>// : IEquatable<AAsyncRequest>,
                                                   // IComparable<AAsyncRequest>

        where TRequest : class

    {

        #region Properties

        /// <summary>
        /// An identifier to relate a later asynchronous response to this request.
        /// </summary>
        [Mandatory]
        public String   CallbackId    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new async request.
        /// </summary>
        /// <param name="CallbackId">An identifier to relate a later asynchronous response to this request.</param>
        public AAsyncRequest(String CallbackId)

        {

            this.CallbackId  = CallbackId;

        }

        #endregion


        #region ToJSON(CustomAAsyncRequestSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomAAsyncRequestSerializer">A delegate to serialize custom async requests.</param>
        public JObject ToJSON(JToken Payload,
                              CustomJObjectSerializerDelegate<AAsyncRequest<TRequest>>? CustomAAsyncRequestSerializer = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("callback_id",   CallbackId),

                           Payload is not null
                               ? new JProperty("payload",       Payload)
                               : null

                       );

            return CustomAAsyncRequestSerializer is not null
                       ? CustomAAsyncRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   CallbackId

               );

        #endregion

    }

}
