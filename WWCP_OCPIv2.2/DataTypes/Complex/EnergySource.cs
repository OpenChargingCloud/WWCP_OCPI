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

using System;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2
{

    /// <summary>
    /// The energy mix.
    /// </summary>
    public struct EnergySource
    {

        #region Properties

        /// <summary>
        /// The energy source.
        /// </summary>
        public EnergySourceCategories  Source        { get; }

        /// <summary>
        /// The percentage of this energy source.
        /// </summary>
        public Single                Percentage    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// A single energy source.
        /// </summary>
        /// <param name="Source">The energy source.</param>
        /// <param name="Percentage">The percentage of this energy source.</param>
        public EnergySource(EnergySourceCategories  Source,
                            Single                Percentage)
        {

            this.Source      = Source;
            this.Percentage  = Percentage;

        }

        #endregion


        #region ToJSON()

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        public JObject ToJSON()

            => JSONObject.Create(
                   new JProperty("source",      Source.    ToString()),
                   new JProperty("percentage",  Percentage.ToString())
               );

        #endregion

    }

}
