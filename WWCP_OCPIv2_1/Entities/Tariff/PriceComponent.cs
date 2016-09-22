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

#endregion

namespace org.GraphDefined.WWCP.OCPIv2_1
{

    /// <summary>
    /// A price component defines the pricing of a tariff.
    /// </summary>
    public struct PriceComponent
    {

        #region Properties

        /// <summary>
        /// Type of tariff dimension.
        /// </summary>
        public DimensionType  Type      { get; }

        /// <summary>
        /// Price per unit for this tariff dimension.
        /// </summary>
        public Decimal        Price     { get; }

        /// <summary>
        /// Minimum amount to be billed. This unit will be billed in this step_size blocks.
        /// </summary>
        /// <example>
        /// If type is time and step_size is 300, then time will be billed in blocks of 5 minutes,
        /// so if 6 minutes is used, 10 minutes (2 blocks of step_size) will be billed.
        /// </example>
        public UInt32         StepSize  { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new price component defining the pricing of a tariff.
        /// </summary>
        /// <param name="Type">Type of tariff dimension.</param>
        /// <param name="Price">Price per unit for this tariff dimension.</param>
        /// <param name="StepSize">Minimum amount to be billed. This unit will be billed in this step_size blocks.</param>
        public PriceComponent(DimensionType  Type,
                              Decimal        Price,
                              UInt32         StepSize = 1)
        {

            this.Type      = Type;
            this.Price     = Price;
            this.StepSize  = StepSize;

        }

        #endregion


        #region Flat(Price, BillingIncrement)

        /// <summary>
        /// Create a new flat rate price component.
        /// </summary>
        /// <param name="Price">Flat rate price.</param>
        public static PriceComponent FlatRate(Decimal  Price)
        {

            return new PriceComponent(DimensionType.FLAT,
                                      Price,
                                      1);

        }

        #endregion

        #region ChargingTime(Price, BillingIncrement)

        /// <summary>
        /// Create a new time-based charging price component.
        /// </summary>
        /// <param name="Price">Price per time span.</param>
        /// <param name="BillingIncrement">The minimum granularity of time in seconds that you will be billed.</param>
        public static PriceComponent ChargingTime(Decimal   Price,
                                                  TimeSpan  BillingIncrement)
        {

            return new PriceComponent(DimensionType.TIME,
                                      Price,
                                      (UInt32) Math.Round(BillingIncrement.TotalSeconds, 0));

        }

        #endregion

        #region ParkingTime(Price, BillingIncrement)

        /// <summary>
        /// Create a new time-based parking price component.
        /// </summary>
        /// <param name="Price">Price per time span.</param>
        /// <param name="BillingIncrement">The minimum granularity of time in seconds that you will be billed.</param>
        public static PriceComponent ParkingTime(Decimal   Price,
                                                 TimeSpan  BillingIncrement)
        {

            return new PriceComponent(DimensionType.PARKING_TIME,
                                      Price,
                                      (UInt32) Math.Round(BillingIncrement.TotalSeconds, 0));

        }

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


        #region GetHashCode()

        /// <summary>
        /// Get the hashcode of this object.
        /// </summary>
        public override Int32 GetHashCode()
        {
            unchecked
            {
                return Type.GetHashCode() * 23 ^ Price.GetHashCode() * 17 ^ StepSize.GetHashCode();
            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Get a string representation of this object.
        /// </summary>
        public override String ToString()
        {
            return String.Concat("type: ", Type.ToString(), ", price: ", Price.ToString(), ", step size:", StepSize.ToString());
        }

        #endregion

    }

}
