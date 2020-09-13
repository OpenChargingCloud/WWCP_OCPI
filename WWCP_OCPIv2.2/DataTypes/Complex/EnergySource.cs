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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2
{

    /// <summary>
    /// The energy mix.
    /// </summary>
    public readonly struct EnergySource : IEquatable<EnergySource>,
                                          IComparable<EnergySource>,
                                          IComparable
    {

        #region Properties

        /// <summary>
        /// The energy source.
        /// </summary>
        public EnergySourceCategories  Source        { get; }

        /// <summary>
        /// The percentage of this energy source.
        /// </summary>
        public Single                  Percentage    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// A single energy source.
        /// </summary>
        /// <param name="Source">The energy source.</param>
        /// <param name="Percentage">The percentage of this energy source.</param>
        public EnergySource(EnergySourceCategories  Source,
                            Single                  Percentage)
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
                   new JProperty("percentage",  Percentage)
               );

        #endregion


        #region Operator overloading

        #region Operator == (EnergySource1, EnergySource2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergySource1">An energy source.</param>
        /// <param name="EnergySource2">Another energy source.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (EnergySource EnergySource1,
                                           EnergySource EnergySource2)

            => EnergySource1.Equals(EnergySource2);

        #endregion

        #region Operator != (EnergySource1, EnergySource2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergySource1">An energy source.</param>
        /// <param name="EnergySource2">Another energy source.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (EnergySource EnergySource1,
                                           EnergySource EnergySource2)

            => !(EnergySource1 == EnergySource2);

        #endregion

        #region Operator <  (EnergySource1, EnergySource2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergySource1">An energy source.</param>
        /// <param name="EnergySource2">Another energy source.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (EnergySource EnergySource1,
                                          EnergySource EnergySource2)

            => EnergySource1.CompareTo(EnergySource2) < 0;

        #endregion

        #region Operator <= (EnergySource1, EnergySource2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergySource1">An energy source.</param>
        /// <param name="EnergySource2">Another energy source.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (EnergySource EnergySource1,
                                           EnergySource EnergySource2)

            => !(EnergySource1 > EnergySource2);

        #endregion

        #region Operator >  (EnergySource1, EnergySource2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergySource1">An energy source.</param>
        /// <param name="EnergySource2">Another energy source.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (EnergySource EnergySource1,
                                          EnergySource EnergySource2)

            => EnergySource1.CompareTo(EnergySource2) > 0;

        #endregion

        #region Operator >= (EnergySource1, EnergySource2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergySource1">An energy source.</param>
        /// <param name="EnergySource2">Another energy source.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (EnergySource EnergySource1,
                                           EnergySource EnergySource2)

            => !(EnergySource1 < EnergySource2);

        #endregion

        #endregion

        #region IComparable<EnergySource> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is EnergySource energySource
                   ? CompareTo(energySource)
                   : throw new ArgumentException("The given object is not an energy source!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(EnergySource)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergySource">An object to compare with.</param>
        public Int32 CompareTo(EnergySource EnergySource)
        {

            var c = Source.CompareTo(EnergySource.Source);

            if (c == 0)
                c = Percentage.CompareTo(EnergySource.Percentage);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<EnergySource> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is EnergySource energySource &&
                   Equals(energySource);

        #endregion

        #region Equals(EnergySource)

        /// <summary>
        /// Compares two energy sources for equality.
        /// </summary>
        /// <param name="EnergySource">A energy source to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(EnergySource EnergySource)

            => Source.    Equals(EnergySource.Source) &&
               Percentage.Equals(EnergySource.Percentage);

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

                return Source.    GetHashCode() * 3 ^
                       Percentage.GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(Percentage, "% ",
                             Source);

        #endregion

    }

}
