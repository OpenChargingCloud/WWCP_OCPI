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

//using org.GraphDefined.WWCP;
using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2
{

    /// <summary>
    /// A Tariff Object consists of a list of one or more TariffElements,
    /// these elements can be used to create complex Tariff structures.
    /// When the list of elements contains more then 1 element, then the
    /// first tariff in the list with matching restrictions will be used.
    /// </summary>
    public class Tariff : IHasId<Tariff_Id>,
                          IEquatable<Tariff>,
                          IComparable<Tariff>,
                          IComparable
    {

        #region Properties

        public Tariff_Id Id { get; }

        /// <summary>
        /// ISO 4217 code of the currency used for this tariff.
        /// </summary>
        [Mandatory]
        public Currency                    Currency          { get; }

        /// <summary>
        /// List of multi-language alternative tariff info text.
        /// </summary>
        [Mandatory]
        public I18NString                  TariffText        { get; }

        /// <summary>
        /// Alternative URL to tariff info.
        /// </summary>
        [Optional]
        public String                      TariffUrl         { get; }

        /// <summary>
        /// An enumeration of tariff elements.
        /// </summary>
        [Mandatory]
        public IEnumerable<TariffElement>  TariffElements    { get; }

        /// <summary>
        /// The energy mix.
        /// </summary>
        [Optional]
        public EnergyMix                   EnergyMix         { get;  }

        #endregion

        #region Constructor(s)

        #region Tariff(Id, ..., params TariffElements)

        /// <summary>
        /// Create a new tariff object.
        /// </summary>
        /// <param name="Id">Uniquely identifies the Tariff within the CPOs platform (and suboperator platforms).</param>
        /// <param name="Currency">ISO 4217 code of the currency used for this tariff.</param>
        /// <param name="TariffElements">An enumeration of tariff elements.</param>
        /// <param name="TariffText">List of multi-language alternative tariff info text.</param>
        /// <param name="TariffUrl">Alternative URL to tariff info.</param>
        public Tariff(Tariff_Id               Id,
                      Currency                Currency,
                      I18NString              TariffText,
                      String                  TariffUrl,
                      params TariffElement[]  TariffElements)

            : this(Id,
                   Currency,
                   TariffElements,
                   TariffText,
                   TariffUrl)

        { }

        #endregion

        #region Tariff(Id, Currency, TariffElements, TariffText = null, TariffUrl = null, EnergyMix = null)

        /// <summary>
        /// Create a new tariff object.
        /// </summary>
        /// <param name="Id">Uniquely identifies the Tariff within the CPOs platform (and suboperator platforms).</param>
        /// <param name="Currency">ISO 4217 code of the currency used for this tariff.</param>
        /// <param name="TariffElements">An enumeration of tariff elements.</param>
        /// <param name="TariffText">List of multi-language alternative tariff info text.</param>
        /// <param name="TariffUrl">Alternative URL to tariff info.</param>
        /// <param name="EnergyMix">The energy mix.</param>
        public Tariff(Tariff_Id                   Id,
                      Currency                    Currency,
                      IEnumerable<TariffElement>  TariffElements,
                      I18NString                  TariffText   = null,
                      String                      TariffUrl    = null,
                      EnergyMix                   EnergyMix    = null)

        {

            #region Initial checks

            if (!TariffElements.SafeAny())
                throw new ArgumentNullException(nameof(TariffElements),  "The given enumeration must not be null or empty!");

            #endregion

            this.Currency        = Currency;
            this.TariffElements  = TariffElements;
            this.TariffText      = TariffText ?? new I18NString();
            this.TariffUrl       = TariffUrl;
            this.EnergyMix       = EnergyMix;

        }

        #endregion

        #endregion


        #region ToJSON()

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        public JObject ToJSON()

            => JSONObject.Create(new JProperty("id",        Id.ToString()),
                                 new JProperty("currency",  Currency.ToString()),
                                 TariffText?.Any() == true ? JSONHelper.ToJSON("tariff_alt_text", TariffText)      : null,
                                 TariffUrl         != null ? new JProperty("tariff_alt_url", TariffUrl.ToString()) : null,
                                 TariffElements.Any()
                                     ? new JProperty("elements",    new JArray(TariffElements.Select(TariffElement => TariffElement.ToJSON())))
                                     : null,
                                 EnergyMix != null
                                     ? new JProperty("energy_mix",  EnergyMix.ToJSON())
                                     : null);

        #endregion


        #region IComparable<Tariff> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is Tariff tariff
                   ? CompareTo(tariff)
                   : throw new ArgumentException("The given object is not a charging tariff!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(Tariff)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Tariff">An Tariff to compare with.</param>
        public Int32 CompareTo(Tariff Tariff)
        {

            if (Tariff is null)
                throw new ArgumentNullException(nameof(Tariff), "The given tariff must not be null!");

            return Id.CompareTo(Tariff.Id);

        }

        #endregion

        #endregion

        #region IEquatable<Tariff> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is Tariff tariff &&
                   Equals(tariff);

        #endregion

        #region Equals(Tariff)

        /// <summary>
        /// Compares two Tariffs for equality.
        /// </summary>
        /// <param name="Tariff">An Tariff to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(Tariff Tariff)

            => (!(Tariff is null)) &&
                   Id.Equals(Tariff.Id);

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Get the hashcode of this object.
        /// </summary>
        public override Int32 GetHashCode()

            => Id.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Get a string representation of this object.
        /// </summary>
        public override String ToString()

            => Id.ToString();

        #endregion

    }

}
