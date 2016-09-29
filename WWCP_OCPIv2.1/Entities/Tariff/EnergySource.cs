/*
 * Copyright (c) 2015-2016 GraphDefined GmbH
 * This file is part of WWCP OCPI <https://github.com/GraphDefined/WWCP_OCPI>
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

using System;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Hermod;

#endregion

namespace org.GraphDefined.WWCP.OCPIv2_1
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
        public EnergySourceCategory  Source        { get; }

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
        public EnergySource(EnergySourceCategory  Source,
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
        {

            return JSONObject.Create(new JProperty("source",      Source.    ToString()),
                                     new JProperty("percentage",  Percentage.ToString()));

        }

        #endregion

    }

}
