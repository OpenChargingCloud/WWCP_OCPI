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
    /// A charging tariff element.
    /// </summary>
    public readonly struct TariffElement : IEquatable<TariffElement>
    {

        #region Properties

        /// <summary>
        /// Enumeration of price components that make up the pricing of this tariff.
        /// </summary>
        [Mandatory]
        public IEnumerable<PriceComponent>      PriceComponents       { get; }

        /// <summary>
        /// Enumeration of tariff restrictions.
        /// </summary>
        [Optional]
        public IEnumerable<TariffRestrictions>  TariffRestrictions    { get;  }

        #endregion

        #region Constructor(s)

        #region TariffElement(params PriceComponents)

        /// <summary>
        /// Create a new charging tariff element.
        /// </summary>
        /// <param name="PriceComponents">An enumeration of price components that make up the pricing of this tariff.</param>
        public TariffElement(params PriceComponent[]  PriceComponents)

            : this(PriceComponents,
                   new TariffRestrictions[0])

        { }

        #endregion

        #region TariffElement(PriceComponents, TariffRestrictions = null)

        /// <summary>
        /// Create a new charging tariff element.
        /// </summary>
        /// <param name="PriceComponents">An enumeration of price components that make up the pricing of this tariff.</param>
        /// <param name="TariffRestrictions">An enumeration of tariff restrictions.</param>
        public TariffElement(IEnumerable<PriceComponent>      PriceComponents,
                             IEnumerable<TariffRestrictions>  TariffRestrictions = null)
        {

            #region Initial checks

            if (!PriceComponents.SafeAny())
                throw new ArgumentNullException(nameof(PriceComponents),  "The given enumeration of price components must not be null or empty!");

            #endregion

            this.PriceComponents     = PriceComponents.    Distinct();
            this.TariffRestrictions  = TariffRestrictions?.Distinct() ?? new TariffRestrictions[0];

        }

        #endregion

        #region TariffElement(PriceComponent,  TariffRestriction)

        /// <summary>
        /// Create a new charging tariff element.
        /// </summary>
        /// <param name="PriceComponent">A price component that makes up the pricing of this tariff.</param>
        /// <param name="TariffRestriction">A charging tariff restriction.</param>
        public TariffElement(PriceComponent      PriceComponent,
                             TariffRestrictions  TariffRestriction)
        {

            #region Initial checks

            if (PriceComponent    == null)
                throw new ArgumentNullException(nameof(PriceComponent),     "The given price component must not be null!");

            if (TariffRestriction == null)
                throw new ArgumentNullException(nameof(TariffRestriction),  "The given charging tariff restriction must not be null!");

            #endregion

            this.PriceComponents     = new PriceComponent[]     { PriceComponent };
            this.TariffRestrictions  = new TariffRestrictions[] { TariffRestriction };

        }

        #endregion

        #endregion


        #region ToJSON()

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        public JObject ToJSON()

            => JSONObject.Create(

                   new JProperty("price_components",    new JArray(PriceComponents.   SafeSelect(PriceComponent    => PriceComponent.   ToJSON()))),

                   TariffRestrictions.SafeAny()
                       ? new JProperty("restrictions",  new JArray(TariffRestrictions.SafeSelect(TariffRestriction => TariffRestriction.ToJSON())))
                       : null);

        #endregion


        #region Operator overloading

        #region Operator == (TariffElement1, TariffElement2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffElement1">A charging tariff element.</param>
        /// <param name="TariffElement2">Another charging tariff element.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (TariffElement TariffElement1,
                                           TariffElement TariffElement2)

            => TariffElement1.Equals(TariffElement2);

        #endregion

        #region Operator != (TariffElement1, TariffElement2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffElement1">A charging tariff element.</param>
        /// <param name="TariffElement2">Another charging tariff element.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (TariffElement TariffElement1,
                                           TariffElement TariffElement2)

            => !(TariffElement1 == TariffElement2);

        #endregion

        #endregion

        #region IEquatable<TariffElement> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is TariffElement tariffElement &&
                   Equals(tariffElement);

        #endregion

        #region Equals(TariffElement)

        /// <summary>
        /// Compares two TariffElements for equality.
        /// </summary>
        /// <param name="TariffElement">A TariffElement to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(TariffElement TariffElement)

            => PriceComponents.   Count().Equals(TariffElement.PriceComponents.   Count())        &&
               TariffRestrictions.Count().Equals(TariffElement.TariffRestrictions.Count())        &&
               PriceComponents.   All(price  => TariffElement.PriceComponents.   Contains(price)) &&
               TariffRestrictions.All(tariff => TariffElement.TariffRestrictions.Contains(tariff));

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

                return PriceComponents.   Aggregate(0, (hashCode, price)  => hashCode ^ price. GetHashCode()) ^
                       TariffRestrictions.Aggregate(0, (hashCode, tariff) => hashCode ^ tariff.GetHashCode());

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(PriceComponents?.   Count() ?? 0, " price components, ",
                             TariffRestrictions?.Count() ?? 0, " tariff restrictions");

        #endregion

    }

}
