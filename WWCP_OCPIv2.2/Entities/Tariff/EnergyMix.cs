/*
 * Copyright (c) 2015-2020 GraphDefined GmbH
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
using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;

#endregion

namespace org.GraphDefined.WWCP.OCPIv2_2
{

    /// <summary>
    /// The energy mix.
    /// </summary>
    public class EnergyMix
    {

        #region Properties

        /// <summary>
        /// The energy is green.
        /// </summary>
        public Boolean                           IsGreenEnergy          { get; }

        /// <summary>
        /// The energy sources.
        /// </summary>
        public IEnumerable<EnergySource>         EnergySources          { get; }

        /// <summary>
        /// The environmental impacts.
        /// </summary>
        public IEnumerable<EnvironmentalImpact>  EnvironmentalImpacts   { get; }

        /// <summary>
        /// The name of the energy supplier.
        /// </summary>
        public String                            SupplierName           { get; }

        /// <summary>
        /// The name of the energy product.
        /// </summary>
        public String                            EnergyProductName      { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// The energy mix.
        /// </summary>
        /// <param name="IsGreenEnergy">The energy is green.</param>
        /// <param name="EnergySources">The energy sources.</param>
        /// <param name="EnvironmentalImpacts">The environmental impacts.</param>
        /// <param name="SupplierName">The name of the energy supplier.</param>
        /// <param name="EnergyProductName">The name of the energy product.</param>
        public EnergyMix(Boolean                           IsGreenEnergy,
                         IEnumerable<EnergySource>         EnergySources,
                         IEnumerable<EnvironmentalImpact>  EnvironmentalImpacts,
                         String                            SupplierName,
                         String                            EnergyProductName)

        {

            this.IsGreenEnergy         = IsGreenEnergy;
            this.EnergySources         = EnergySources;
            this.EnvironmentalImpacts  = EnvironmentalImpacts;
            this.SupplierName          = SupplierName;
            this.EnergyProductName     = EnergyProductName;

        }

        #endregion


        #region ToJSON()

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        public JObject ToJSON()

            => JSONObject.Create(

                   new JProperty("is_green_energy", IsGreenEnergy),

                   new JProperty("energy_sources",  new JArray(
                       EnergySources.SafeSelect(energysource => energysource.ToJSON())
                   )),

                   new JProperty("environ_impact",  new JArray(
                       EnvironmentalImpacts.Select(environmentalimpact => environmentalimpact.ToJSON())
                   )),

                   new JProperty("supplier_name",        SupplierName),
                   new JProperty("energy_product_name",  EnergyProductName)

               );

        #endregion

    }

}
