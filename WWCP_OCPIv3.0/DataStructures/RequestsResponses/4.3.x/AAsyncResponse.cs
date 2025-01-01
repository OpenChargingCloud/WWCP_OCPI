/*
 * Copyright (c) 2015-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv3_0
{

    /// <summary>
    /// A generic async response.
    /// </summary>
    public abstract class AAsyncResponse<TRequest, TResponse> //: IEquatable<AsyncResponse>,
                                                             //  IComparable<AsyncResponse>

        where TRequest  : class
        where TResponse : class

    {

        #region Properties

        /// <summary>
        /// The request leading to this response.
        /// </summary>
        [Mandatory]
        public TRequest         Request       { get; }

        /// <summary>
        /// The completion status of the requested asynchronous remote procedure call.
        /// </summary>
        [Mandatory]
        public AsyncResultType  ResultType    { get; }

        ///// <summary>
        ///// The error that occurred during the execution of the asynchronous remote procedure call, if any.
        ///// </summary>
        //[Optional]
        //public JToken?          Error         { get; }

        ///// <summary>
        ///// The result of the asynchronous remote procedure call, if any.
        ///// </summary>
        //[Optional]
        //public JToken?          Payload       { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new async response.
        /// </summary>
        /// <param name="ResultType">The completion status of the requested asynchronous remote procedure call.</param>
        /// <param name="Error">The error that occurred during the execution of the asynchronous remote procedure call, if any.</param>
        /// <param name="Payload">The result of the asynchronous remote procedure call, if any.</param>
        public AAsyncResponse(TRequest         Request,
                              AsyncResultType  ResultType)
                             //JToken?          Error     = null,
                             //JToken?          Payload   = null)

        {

            this.Request     = Request;
            this.ResultType  = ResultType;
            //this.Error       = Error;
            //this.Payload     = Payload;

        }

        #endregion



        #region ToJSON(CustomAsyncResponseSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomAsyncResponseSerializer">A delegate to serialize custom async responses.</param>
        public JObject ToJSON(JToken? Payload,
                              JToken? Error,
                              CustomJObjectSerializerDelegate<AAsyncResponse<TRequest, TResponse>>? CustomAsyncResponseSerializer = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("result_type",   ResultType.ToString()),

                           Error is not null
                               ? new JProperty("error",         Error)
                               : null,

                           Payload is not null
                               ? new JProperty("payload",       Payload)
                               : null

                       );

            return CustomAsyncResponseSerializer is not null
                       ? CustomAsyncResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   ResultType

                   //Error is not null
                   //    ? $", error: {Error?.ToString().SubstringMax(32)}"
                   //    : "",

                   //Payload is not null
                   //    ? $", payload: {Payload?.ToString().SubstringMax(32)}"
                   //    : ""

               );

        #endregion

    }

}
