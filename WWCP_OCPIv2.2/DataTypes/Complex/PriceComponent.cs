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

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2
{

    /// <summary>
    /// A price component defines the pricing of a tariff.
    /// </summary>
    public readonly struct PriceComponent : IEquatable<PriceComponent>,
                                            IComparable<PriceComponent>,
                                            IComparable
    {

        #region Properties

        /// <summary>
        /// Type of tariff dimension.
        /// </summary>
        public DimensionTypes  Type        { get; }

        /// <summary>
        /// Price per unit for this tariff dimension.
        /// </summary>
        public Decimal         Price       { get; }

        /// <summary>
        /// Minimum amount to be billed. This unit will be billed in this step_size blocks.
        /// </summary>
        /// <example>
        /// If type is time and step_size is 300, then time will be billed in blocks of 5 minutes,
        /// so if 6 minutes is used, 10 minutes (2 blocks of step_size) will be billed.
        /// </example>
        public UInt32          StepSize    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new price component defining the pricing of a tariff.
        /// </summary>
        /// <param name="Type">Type of tariff dimension.</param>
        /// <param name="Price">Price per unit for this tariff dimension.</param>
        /// <param name="StepSize">Minimum amount to be billed. This unit will be billed in this step_size blocks.</param>
        public PriceComponent(DimensionTypes  Type,
                              Decimal         Price,
                              UInt32          StepSize = 1)
        {

            this.Type      = Type;
            this.Price     = Price;
            this.StepSize  = StepSize;

        }

        #endregion


        #region Flat        (Price)

        /// <summary>
        /// Create a new flat rate price component.
        /// </summary>
        /// <param name="Price">Flat rate price.</param>
        public static PriceComponent FlatRate(Decimal  Price)

            => new PriceComponent(DimensionTypes.FLAT,
                                  Price,
                                  1);

        #endregion

        #region ChargingTime(Price, BillingIncrement)

        /// <summary>
        /// Create a new time-based charging price component.
        /// </summary>
        /// <param name="Price">Price per time span.</param>
        /// <param name="BillingIncrement">The minimum granularity of time in seconds that you will be billed.</param>
        public static PriceComponent ChargingTime(Decimal   Price,
                                                  TimeSpan  BillingIncrement)

            => new PriceComponent(DimensionTypes.TIME,
                                  Price,
                                  (UInt32) Math.Round(BillingIncrement.TotalSeconds, 0));

        #endregion

        #region ParkingTime (Price, BillingIncrement)

        /// <summary>
        /// Create a new time-based parking price component.
        /// </summary>
        /// <param name="Price">Price per time span.</param>
        /// <param name="BillingIncrement">The minimum granularity of time in seconds that you will be billed.</param>
        public static PriceComponent ParkingTime(Decimal   Price,
                                                 TimeSpan  BillingIncrement)

            => new PriceComponent(DimensionTypes.PARKING_TIME,
                                  Price,
                                  (UInt32) Math.Round(BillingIncrement.TotalSeconds, 0));

        #endregion


        #region ToJSON()

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        public JObject ToJSON()
        {

            return new JObject(new JProperty("type",       Type. ToString()),
                               new JProperty("price",      Price.ToString("0.00")),
                               new JProperty("step_size",  StepSize));

        }

        #endregion


        #region Operator overloading

        #region Operator == (PriceComponent1, PriceComponent2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PriceComponent1">A price component.</param>
        /// <param name="PriceComponent2">Another price component.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (PriceComponent PriceComponent1,
                                           PriceComponent PriceComponent2)

            => PriceComponent1.Equals(PriceComponent2);

        #endregion

        #region Operator != (PriceComponent1, PriceComponent2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PriceComponent1">A price component.</param>
        /// <param name="PriceComponent2">Another price component.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (PriceComponent PriceComponent1,
                                           PriceComponent PriceComponent2)

            => !(PriceComponent1 == PriceComponent2);

        #endregion

        #region Operator <  (PriceComponent1, PriceComponent2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PriceComponent1">A price component.</param>
        /// <param name="PriceComponent2">Another price component.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (PriceComponent PriceComponent1,
                                          PriceComponent PriceComponent2)

            => PriceComponent1.CompareTo(PriceComponent2) < 0;

        #endregion

        #region Operator <= (PriceComponent1, PriceComponent2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PriceComponent1">A price component.</param>
        /// <param name="PriceComponent2">Another price component.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (PriceComponent PriceComponent1,
                                           PriceComponent PriceComponent2)

            => !(PriceComponent1 > PriceComponent2);

        #endregion

        #region Operator >  (PriceComponent1, PriceComponent2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PriceComponent1">A price component.</param>
        /// <param name="PriceComponent2">Another price component.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (PriceComponent PriceComponent1,
                                          PriceComponent PriceComponent2)

            => PriceComponent1.CompareTo(PriceComponent2) > 0;

        #endregion

        #region Operator >= (PriceComponent1, PriceComponent2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PriceComponent1">A price component.</param>
        /// <param name="PriceComponent2">Another price component.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (PriceComponent PriceComponent1,
                                           PriceComponent PriceComponent2)

            => !(PriceComponent1 < PriceComponent2);

        #endregion

        #endregion

        #region IComparable<PriceComponent> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is PriceComponent priceComponent
                   ? CompareTo(priceComponent)
                   : throw new ArgumentException("The given object is not a price component!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(PriceComponent)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PriceComponent">An object to compare with.</param>
        public Int32 CompareTo(PriceComponent PriceComponent)
        {

            var c = Type.    CompareTo(PriceComponent.Type);

            if (c == 0)
                c = Price.   CompareTo(PriceComponent.Price);

            if (c == 0)
                c = StepSize.CompareTo(PriceComponent.StepSize);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<PriceComponent> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is PriceComponent priceComponent &&
                   Equals(priceComponent);

        #endregion

        #region Equals(PriceComponent)

        /// <summary>
        /// Compares two price components for equality.
        /// </summary>
        /// <param name="PriceComponent">A price component to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(PriceComponent PriceComponent)

            => Type.    Equals(PriceComponent.Type) &&
               Price.   Equals(PriceComponent.Price) &&
               StepSize.Equals(PriceComponent.StepSize);

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

                return Type.    GetHashCode() * 5 ^
                       Price.   GetHashCode() * 3 ^
                       StepSize.GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(Type,  "; ",
                             Price, "; ",
                             StepSize);

        #endregion

    }

}
