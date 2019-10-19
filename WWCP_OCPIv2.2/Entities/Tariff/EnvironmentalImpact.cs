/*
 * Copyright (c) 2015-2016 GraphDefined GmbH
 * This file is part of WWCP OCPI <https://github.com/GraphDefined/WWCP_OCPI>
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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.WWCP.OCPIv2_2
{

    /// <summary>
    /// The environmental impact.
    /// </summary>
    public struct EnvironmentalImpact
    {

        #region Properties

        /// <summary>
        /// The environmental impact.
        /// </summary>
        public EnergySourceCategory  Source    { get; }

        /// <summary>
        /// The amount of this environmental impact.
        /// </summary>
        public Double                Amount    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// The environmental impact.
        /// </summary>
        /// <param name="Source">The environmental impact.</param>
        /// <param name="Amount">The amount of this environmental impact.</param>
        public EnvironmentalImpact(EnergySourceCategory  Source,
                                   Double                Amount)
        {

            this.Source  = Source;
            this.Amount  = Amount;

        }

        #endregion


        #region ToJSON()

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        public JObject ToJSON()

            => JSONObject.Create(
                   new JProperty("source",  Source.ToString()),
                   new JProperty("amount",  Amount.ToString())
               );

        #endregion

    }

}
