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

        /// <summary>
        /// The ISO-3166 alpha-2 country code of the CPO that 'owns' this session.
        /// </summary>
        [Optional]
        public CountryCode                      CountryCode             { get; }

        /// <summary>
        /// The Id of the CPO that 'owns' this session (following the ISO-15118 standard).
        /// </summary>
        [Optional]
        public Party_Id                         PartyId                 { get; }

        /// <summary>
        /// The identification of the tariff within the CPOs platform (and suboperator platforms). 
        /// </summary>
        [Mandatory]
        public Tariff_Id                        Id                      { get; }

        /// <summary>
        /// ISO 4217 code of the currency used for this tariff.
        /// </summary>
        [Mandatory]
        public Currency                         Currency                { get; }

        /// <summary>
        /// Defines the type of the tariff. This allows for distinction in case of given
        /// charging preferences. When omitted, this tariff is valid for all charging sessions.
        /// </summary>
        [Optional]
        public TariffTypes?                     TariffType              { get; }

        /// <summary>
        /// Multi-language alternative tariff info text.
        /// </summary>
        [Mandatory]
        public I18NString                       TariffAltText           { get; }

        /// <summary>
        /// URL to a web page that contains an explanation of the tariff information
        /// in human readable form.
        /// </summary>
        [Optional]
        public String                           TariffAltUrl            { get; }

        /// <summary>
        /// When this field is set, a Charging Session with this tariff will at least cost
        /// this amount. This is different from a FLAT fee (Start Tariff, Transaction Fee),
        /// as a FLAT fee is a fixed amount that has to be paid for any Charging Session.
        /// A minimum price indicates that when the cost of a charging session is lower
        /// than this amount, the cost of the charging session will be equal to this amount.
        /// </summary>
        [Optional]
        public Price?                           MinPrice                { get; }

        /// <summary>
        /// When this field is set, a charging session with this tariff will NOT cost more
        /// than this amount.
        /// </summary>
        [Optional]
        public Price?                           MaxPrice                { get; }

        /// <summary>
        /// An enumeration of tariff elements.
        /// </summary>
        [Mandatory]
        public IEnumerable<TariffElement>       TariffElements          { get; }

        /// <summary>
        /// The time when this tariff becomes active, in UTC, time_zone field of the
        /// charging location can be used to convert to local time. Typically used for
        /// a new tariff that is already given with the charging location, before it
        /// becomes active.
        /// </summary>
        [Optional]
        public DateTime?                        Start                   { get; }

        /// <summary>
        /// The time after which this tariff is no longer valid, in UTC, time_zone field
        /// if the charging location can be used to convert to local time. Typically used
        /// when this tariff is going to be replaced with a different tariff in the near
        /// future.
        /// </summary>
        [Optional]
        public DateTime?                        End                     { get; }

        /// <summary>
        /// Details on the energy supplied with this tariff.
        /// </summary>
        [Optional]
        public EnergyMix                        EnergyMix               { get;  }

        /// <summary>
        /// Timestamp when this tariff was last updated (or created).
        /// </summary>
        [Mandatory]
        public DateTime                         LastUpdated             { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new chrging tariff.
        /// </summary>
        public Tariff(CountryCode                 CountryCode,
                      Party_Id                    PartyId,
                      Tariff_Id                   Id ,
                      Currency                    Currency,
                      IEnumerable<TariffElement>  TariffElements,

                      TariffTypes?                TariffType      = null,
                      I18NString                  TariffAltText   = null,
                      String                      TariffAltUrl    = null,
                      Price?                      MinPrice        = null,
                      Price?                      MaxPrice        = null,
                      DateTime?                   Start           = null,
                      DateTime?                   End             = null,
                      EnergyMix                   EnergyMix       = null,
                      DateTime?                   LastUpdated     = null)

        {

            #region Initial checks

            if (!TariffElements.SafeAny())
                throw new ArgumentNullException(nameof(TariffElements),  "The given enumeration of tariff elements must not be null or empty!");

            #endregion

            this.CountryCode      = CountryCode;
            this.PartyId          = PartyId;
            this.Id               = Id;
            this.Currency         = Currency;
            this.TariffElements   = TariffElements.Distinct();

            this.TariffType       = TariffType;
            this.TariffAltText    = TariffAltText;
            this.TariffAltUrl     = TariffAltUrl;
            this.MinPrice         = MinPrice;
            this.MaxPrice         = MaxPrice;
            this.Start            = Start;
            this.End              = End;
            this.EnergyMix        = EnergyMix;

            this.LastUpdated      = LastUpdated ?? DateTime.Now;

        }

        #endregion


        #region ToJSON()

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        public JObject ToJSON()

            => JSONObject.Create(new JProperty("id",        Id.ToString()),
                                 new JProperty("currency",  Currency.ToString()),
                                 //TariffText?.Any() == true ? JSONHelper.ToJSON("tariff_alt_text", TariffText)      : null,
                                 //TariffUrl         != null ? new JProperty("tariff_alt_url", TariffUrl.ToString()) : null,
                                 TariffElements.Any()
                                     ? new JProperty("elements",    new JArray(TariffElements.Select(TariffElement => TariffElement.ToJSON())))
                                     : null,
                                 EnergyMix != null
                                     ? new JProperty("energy_mix",  EnergyMix.ToJSON())
                                     : null);

        #endregion


        #region Operator overloading

        #region Operator == (Tariff1, Tariff2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Tariff1">A tariff.</param>
        /// <param name="Tariff2">Another tariff.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Tariff Tariff1,
                                           Tariff Tariff2)
        {

            if (Object.ReferenceEquals(Tariff1, Tariff2))
                return true;

            if (Tariff1 is null || Tariff2 is null)
                return false;

            return Tariff1.Equals(Tariff2);

        }

        #endregion

        #region Operator != (Tariff1, Tariff2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Tariff1">A tariff.</param>
        /// <param name="Tariff2">Another tariff.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Tariff Tariff1,
                                           Tariff Tariff2)

            => !(Tariff1 == Tariff2);

        #endregion

        #region Operator <  (Tariff1, Tariff2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Tariff1">A tariff.</param>
        /// <param name="Tariff2">Another tariff.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Tariff Tariff1,
                                          Tariff Tariff2)

            => Tariff1 is null
                   ? throw new ArgumentNullException(nameof(Tariff1), "The given tariff must not be null!")
                   : Tariff1.CompareTo(Tariff2) < 0;

        #endregion

        #region Operator <= (Tariff1, Tariff2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Tariff1">A tariff.</param>
        /// <param name="Tariff2">Another tariff.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Tariff Tariff1,
                                           Tariff Tariff2)

            => !(Tariff1 > Tariff2);

        #endregion

        #region Operator >  (Tariff1, Tariff2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Tariff1">A tariff.</param>
        /// <param name="Tariff2">Another tariff.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Tariff Tariff1,
                                          Tariff Tariff2)

            => Tariff1 is null
                   ? throw new ArgumentNullException(nameof(Tariff1), "The given tariff must not be null!")
                   : Tariff1.CompareTo(Tariff2) > 0;

        #endregion

        #region Operator >= (Tariff1, Tariff2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Tariff1">A tariff.</param>
        /// <param name="Tariff2">Another tariff.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Tariff Tariff1,
                                           Tariff Tariff2)

            => !(Tariff1 < Tariff2);

        #endregion

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

            => Tariff is null
                   ? throw new ArgumentNullException(nameof(Tariff), "The given charging tariff must not be null!")
                   : Id.CompareTo(Tariff.Id);

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

            => !(Tariff is null) &&
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
