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

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2
{

    /// <summary>
    /// A charge detail record dimension.
    /// </summary>
    public readonly struct CDRDimension : IEquatable<CDRDimension>,
                                          IComparable<CDRDimension>,
                                          IComparable
    {

        #region Properties

        /// <summary>
        /// Type of cdr dimension.
        /// </summary>
        public TariffDimensionTypes  Type      { get; }

        /// <summary>
        /// Volume of the dimension consumed, measured according to the dimension type.
        /// </summary>
        public Decimal         Volume    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new charge detail record dimension.
        /// </summary>
        /// <param name="Type">Type of charge detail record dimension.</param>
        /// <param name="Volume">Volume of the dimension consumed, measured according to the dimension type.</param>
        public CDRDimension(TariffDimensionTypes  Type,
                            Decimal         Volume)
        {

            this.Type    = Type;
            this.Volume  = Volume;

        }

        #endregion


        #region Operator overloading

        #region Operator == (CDRDimension1, CDRDimension2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDRDimension1">A specification of a charge detail record dimension.</param>
        /// <param name="CDRDimension2">Another specification of a charge detail record dimension.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (CDRDimension CDRDimension1,
                                           CDRDimension CDRDimension2)

            => CDRDimension1.Equals(CDRDimension2);

        #endregion

        #region Operator != (CDRDimension1, CDRDimension2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDRDimension1">A specification of a charge detail record dimension.</param>
        /// <param name="CDRDimension2">Another specification of a charge detail record dimension.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (CDRDimension CDRDimension1,
                                           CDRDimension CDRDimension2)

            => !(CDRDimension1 == CDRDimension2);

        #endregion

        #region Operator <  (CDRDimension1, CDRDimension2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDRDimension1">A specification of a charge detail record dimension.</param>
        /// <param name="CDRDimension2">Another specification of a charge detail record dimension.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (CDRDimension CDRDimension1,
                                          CDRDimension CDRDimension2)

            => CDRDimension1.CompareTo(CDRDimension2) < 0;

        #endregion

        #region Operator <= (CDRDimension1, CDRDimension2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDRDimension1">A specification of a charge detail record dimension.</param>
        /// <param name="CDRDimension2">Another specification of a charge detail record dimension.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (CDRDimension CDRDimension1,
                                           CDRDimension CDRDimension2)

            => !(CDRDimension1 > CDRDimension2);

        #endregion

        #region Operator >  (CDRDimension1, CDRDimension2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDRDimension1">A specification of a charge detail record dimension.</param>
        /// <param name="CDRDimension2">Another specification of a charge detail record dimension.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (CDRDimension CDRDimension1,
                                          CDRDimension CDRDimension2)

            => CDRDimension1.CompareTo(CDRDimension2) > 0;

        #endregion

        #region Operator >= (CDRDimension1, CDRDimension2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDRDimension1">A specification of a charge detail record dimension.</param>
        /// <param name="CDRDimension2">Another specification of a charge detail record dimension.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (CDRDimension CDRDimension1,
                                           CDRDimension CDRDimension2)

            => !(CDRDimension1 < CDRDimension2);

        #endregion

        #endregion

        #region IComparable<CDRDimension> Members

        #region CompareTo(Object)d

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is CDRDimension CDRDimension
                   ? CompareTo(CDRDimension)
                   : throw new ArgumentException("The given object is not a charge detail record dimension!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(CDRDimension)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDRDimension">An object to compare with.</param>
        public Int32 CompareTo(CDRDimension CDRDimension)

            => Type.CompareTo(CDRDimension.Type);

        #endregion

        #endregion

        #region IEquatable<CDRDimension> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is CDRDimension CDRDimension &&
                   Equals(CDRDimension);

        #endregion

        #region Equals(CDRDimension)

        /// <summary>
        /// Compares two CDRDimensions for equality.
        /// </summary>
        /// <param name="CDRDimension">A CDRDimension to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(CDRDimension CDRDimension)

            => Type.  Equals(CDRDimension.Type) &&
               Volume.Equals(CDRDimension.Volume);

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
                return Type.  GetHashCode() * 3 ^
                       Volume.GetHashCode();
            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(Volume,
                             " ",
                             Type);

        #endregion

    }

}
