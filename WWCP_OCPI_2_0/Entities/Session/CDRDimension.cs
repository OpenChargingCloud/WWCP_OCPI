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

#endregion

namespace org.GraphDefined.WWCP.OCPI_2_0
{

    /// <summary>
    /// A CDR dimension.
    /// </summary>
    public struct CDRDimension
    {

        #region Properties

        #region Type

        private readonly DimensionType _Type;

        /// <summary>
        /// Type of cdr dimension.
        /// </summary>
        public DimensionType Type
        {
            get
            {
                return _Type;
            }
        }

        #endregion

        #region Volume

        private readonly Decimal _Volume;

        /// <summary>
        /// Volume of the dimension consumed, measured according to the dimension type.
        /// </summary>
        public Decimal Volume
        {
            get
            {
                return _Volume;
            }
        }

        #endregion

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new CDR dimension object.
        /// </summary>
        /// <param name="Type">Type of cdr dimension.</param>
        /// <param name="Volume">Volume of the dimension consumed, measured according to the dimension type.</param>
        public CDRDimension(DimensionType  Type,
                            Decimal        Volume)
        {

            this._Type    = Type;
            this._Volume  = Volume;

        }

        #endregion

    }

}
