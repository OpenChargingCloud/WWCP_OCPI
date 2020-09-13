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
    /// The environmental impact.
    /// </summary>
    public readonly struct EnvironmentalImpact : IEquatable<EnvironmentalImpact>,
                                                 IComparable<EnvironmentalImpact>,
                                                 IComparable
    {

        #region Properties

        /// <summary>
        /// The environmental impact.
        /// </summary>
        public EnergySourceCategories  Source    { get; }

        /// <summary>
        /// The amount of this environmental impact.
        /// </summary>
        public Double                  Amount    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// The environmental impact.
        /// </summary>
        /// <param name="Source">The environmental impact.</param>
        /// <param name="Amount">The amount of this environmental impact.</param>
        public EnvironmentalImpact(EnergySourceCategories  Source,
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


        #region Operator overloading

        #region Operator == (EnvironmentalImpact1, EnvironmentalImpact2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnvironmentalImpact1">An environmental impact.</param>
        /// <param name="EnvironmentalImpact2">Another environmental impact.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (EnvironmentalImpact EnvironmentalImpact1,
                                           EnvironmentalImpact EnvironmentalImpact2)

            => EnvironmentalImpact1.Equals(EnvironmentalImpact2);

        #endregion

        #region Operator != (EnvironmentalImpact1, EnvironmentalImpact2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnvironmentalImpact1">An environmental impact.</param>
        /// <param name="EnvironmentalImpact2">Another environmental impact.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (EnvironmentalImpact EnvironmentalImpact1,
                                           EnvironmentalImpact EnvironmentalImpact2)

            => !(EnvironmentalImpact1 == EnvironmentalImpact2);

        #endregion

        #region Operator <  (EnvironmentalImpact1, EnvironmentalImpact2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnvironmentalImpact1">An environmental impact.</param>
        /// <param name="EnvironmentalImpact2">Another environmental impact.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (EnvironmentalImpact EnvironmentalImpact1,
                                          EnvironmentalImpact EnvironmentalImpact2)

            => EnvironmentalImpact1.CompareTo(EnvironmentalImpact2) < 0;

        #endregion

        #region Operator <= (EnvironmentalImpact1, EnvironmentalImpact2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnvironmentalImpact1">An environmental impact.</param>
        /// <param name="EnvironmentalImpact2">Another environmental impact.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (EnvironmentalImpact EnvironmentalImpact1,
                                           EnvironmentalImpact EnvironmentalImpact2)

            => !(EnvironmentalImpact1 > EnvironmentalImpact2);

        #endregion

        #region Operator >  (EnvironmentalImpact1, EnvironmentalImpact2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnvironmentalImpact1">An environmental impact.</param>
        /// <param name="EnvironmentalImpact2">Another environmental impact.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (EnvironmentalImpact EnvironmentalImpact1,
                                          EnvironmentalImpact EnvironmentalImpact2)

            => EnvironmentalImpact1.CompareTo(EnvironmentalImpact2) > 0;

        #endregion

        #region Operator >= (EnvironmentalImpact1, EnvironmentalImpact2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnvironmentalImpact1">An environmental impact.</param>
        /// <param name="EnvironmentalImpact2">Another environmental impact.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (EnvironmentalImpact EnvironmentalImpact1,
                                           EnvironmentalImpact EnvironmentalImpact2)

            => !(EnvironmentalImpact1 < EnvironmentalImpact2);

        #endregion

        #endregion

        #region IComparable<EnvironmentalImpact> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is EnvironmentalImpact environmentalImpact
                   ? CompareTo(environmentalImpact)
                   : throw new ArgumentException("The given object is not an environmental impact!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(EnvironmentalImpact)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnvironmentalImpact">An object to compare with.</param>
        public Int32 CompareTo(EnvironmentalImpact EnvironmentalImpact)
        {

            var c = Source.CompareTo(EnvironmentalImpact.Source);

            if (c == 0)
                c = Amount.CompareTo(EnvironmentalImpact.Amount);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<EnvironmentalImpact> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is EnvironmentalImpact environmentalImpact &&
                   Equals(environmentalImpact);

        #endregion

        #region Equals(EnvironmentalImpact)

        /// <summary>
        /// Compares two EnvironmentalImpacts for equality.
        /// </summary>
        /// <param name="EnvironmentalImpact">A EnvironmentalImpact to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(EnvironmentalImpact EnvironmentalImpact)

            => Source.Equals(EnvironmentalImpact.Source) &&
               Amount.Equals(EnvironmentalImpact.Amount);

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

                return Source.GetHashCode() * 3 ^
                       Amount.GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(Source,
                             " - ",
                             Amount);

        #endregion

    }

}
