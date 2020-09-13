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
using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2
{

    /// <summary>
    /// The energy mix.
    /// </summary>
    public class EnergyMix : IEquatable<EnergyMix>
    {

        #region Properties

        /// <summary>
        /// The energy is green.
        /// </summary>
        [Mandatory]
        public Boolean                           IsGreenEnergy           { get; }

        /// <summary>
        /// The energy mixs.
        /// </summary>
        [Optional]
        public IEnumerable<EnergySource>         EnergySources           { get; }

        /// <summary>
        /// The environmental impacts.
        /// </summary>
        [Optional]
        public IEnumerable<EnvironmentalImpact>  EnvironmentalImpacts    { get; }

        /// <summary>
        /// The name of the energy supplier.
        /// </summary>
        [Optional]
        public String                            SupplierName            { get; }

        /// <summary>
        /// The name of the energy product.
        /// </summary>
        [Optional]
        public String                            EnergyProductName       { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// The energy mix.
        /// </summary>
        /// <param name="IsGreenEnergy">The energy is green.</param>
        /// <param name="EnergySources">The optional energy sources.</param>
        /// <param name="EnvironmentalImpacts">The optional environmental impacts.</param>
        /// <param name="SupplierName">The optional name of the energy supplier.</param>
        /// <param name="EnergyProductName">The optional name of the energy product.</param>
        public EnergyMix(Boolean                           IsGreenEnergy,
                         IEnumerable<EnergySource>         EnergySources          = null,
                         IEnumerable<EnvironmentalImpact>  EnvironmentalImpacts   = null,
                         String                            SupplierName           = null,
                         String                            EnergyProductName      = null)

        {

            this.IsGreenEnergy         = IsGreenEnergy;
            this.EnergySources         = EnergySources?.       Distinct() ?? new EnergySource[0];
            this.EnvironmentalImpacts  = EnvironmentalImpacts?.Distinct() ?? new EnvironmentalImpact[0];
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

                   new JProperty("is_green_energy",            IsGreenEnergy),

                   EnergySources.       SafeAny()
                       ? new JProperty("energy_sources",       new JArray(EnergySources.       SafeSelect(energysource        => energysource.       ToJSON())))
                       : null,

                   EnvironmentalImpacts.SafeAny()
                       ? new JProperty("environ_impact",       new JArray(EnvironmentalImpacts.SafeSelect(environmentalimpact => environmentalimpact.ToJSON())))
                       : null,

                   SupplierName.IsNotNullOrEmpty()
                       ? new JProperty("supplier_name",        SupplierName)
                       : null,

                   EnergyProductName.IsNotNullOrEmpty()
                       ? new JProperty("energy_product_name",  EnergyProductName)
                       : null

               );

        #endregion


        #region Operator overloading

        #region Operator == (EnergyMix1, EnergyMix2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergyMix1">An energy mix.</param>
        /// <param name="EnergyMix2">Another energy mix.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (EnergyMix EnergyMix1,
                                           EnergyMix EnergyMix2)

            => EnergyMix1.Equals(EnergyMix2);

        #endregion

        #region Operator != (EnergyMix1, EnergyMix2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergyMix1">An energy mix.</param>
        /// <param name="EnergyMix2">Another energy mix.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (EnergyMix EnergyMix1,
                                           EnergyMix EnergyMix2)

            => !(EnergyMix1 == EnergyMix2);

        #endregion

        #endregion

        #region IEquatable<EnergyMix> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is EnergyMix energyMix &&
                   Equals(energyMix);

        #endregion

        #region Equals(EnergyMix)

        /// <summary>
        /// Compares two energy mixs for equality.
        /// </summary>
        /// <param name="EnergyMix">A energy mix to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(EnergyMix EnergyMix)

            => IsGreenEnergy.Equals(EnergyMix.IsGreenEnergy) &&

               EnergySources.       Count().Equals(EnergyMix.EnergySources.Count())                &&
               EnvironmentalImpacts.Count().Equals(EnergyMix.EnvironmentalImpacts.Count())         &&
               EnergySources.       All(source => EnergyMix.EnergySources.       Contains(source)) &&
               EnvironmentalImpacts.All(impact => EnergyMix.EnvironmentalImpacts.Contains(impact)) &&

               ((SupplierName.     IsNullOrEmpty()         && EnergyMix.SupplierName.     IsNullOrEmpty()) ||
                (SupplierName.     IsNeitherNullNorEmpty() && EnergyMix.SupplierName.     IsNeitherNullNorEmpty() && SupplierName.     Equals(EnergyMix.SupplierName))) &&

               ((EnergyProductName.IsNullOrEmpty()         && EnergyMix.EnergyProductName.IsNullOrEmpty()) ||
                (EnergyProductName.IsNeitherNullNorEmpty() && EnergyMix.EnergyProductName.IsNeitherNullNorEmpty() && EnergyProductName.Equals(EnergyMix.EnergyProductName)));

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return IsGreenEnergy.           GetHashCode() * 5 ^

                       EnergySources.       Aggregate(0, (hashCode, source) => hashCode ^ source.GetHashCode()) ^
                       EnvironmentalImpacts.Aggregate(0, (hashCode, impact) => hashCode ^ impact.GetHashCode()) ^

                       (SupplierName.     IsNotNullOrEmpty()
                            ? SupplierName.     GetHashCode() * 3
                            : 0) ^

                       (EnergyProductName.IsNotNullOrEmpty()
                            ? EnergyProductName.GetHashCode()
                            : 0);

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(IsGreenEnergy
                                 ? "Green energy"
                                 : "No green energy",
                             EnergyProductName.IsNotNullOrEmpty()
                                 ? " (" + EnergyProductName + ")"
                                 : "",
                             SupplierName.IsNotNullOrEmpty()
                                 ? " from " + SupplierName
                                 : "");

        #endregion

    }

}
