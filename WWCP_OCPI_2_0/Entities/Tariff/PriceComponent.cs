/*
 * Copyright (c) 2015 GraphDefined GmbH
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

namespace org.GraphDefined.WWCP.OCPI_2_0
{

    /// <summary>
    /// A price component defines the pricing of a tariff.
    /// </summary>
    public struct PriceComponent
    {

        #region Properties

        #region Type

        private readonly DimensionType _Type;

        /// <summary>
        /// Type of tariff dimension.
        /// </summary>
        public DimensionType Type
        {
            get
            {
                return _Type;
            }
        }

        #endregion

        #region Price

        private readonly Decimal _Price;

        /// <summary>
        /// Price per unit for this tariff dimension.
        /// </summary>
        public Decimal Price
        {
            get
            {
                return _Price;
            }
        }

        #endregion

        #region StepSize

        private readonly UInt32 _StepSize;

        /// <summary>
        /// Minimum amount to be billed. This unit will be billed in this step_size blocks.
        /// </summary>
        /// <example>
        /// If type is time and step_size is 300, then time will be billed in blocks of 5 minutes,
        /// so if 6 minutes is used, 10 minutes (2 blocks of step_size) will be billed.
        /// </example>
        public UInt32 StepSize
        {
            get
            {
                return _StepSize;
            }
        }

        #endregion

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
                              UInt32         StepSize)
        {

            this._Type      = Type;
            this._Price     = Price;
            this._StepSize  = StepSize;

        }

        #endregion


        #region ToJSON()

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        public JObject ToJSON()
        {

            return new JObject(new JProperty("type",       _Type. ToString()),
                               new JProperty("price",      _Price.ToString()),
                               new JProperty("step_size",  _StepSize));

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
                return _Type.GetHashCode() * 23 ^ _Price.GetHashCode() * 17 ^ _StepSize.GetHashCode();
            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Get a string representation of this object.
        /// </summary>
        public override String ToString()
        {
            return String.Concat("type: ", _Type.ToString(), ", price: ", _Price.ToString(), ", step size:", _StepSize.ToString());
        }

        #endregion

    }

}
