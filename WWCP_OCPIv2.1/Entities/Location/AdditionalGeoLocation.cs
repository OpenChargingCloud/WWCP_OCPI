/*
 * Copyright (c) 2015-2016 GraphDefined GmbH
 * This file is part of WWCP OCPI <https://github.com/GraphDefined/WWCP_OCPI>
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

using org.GraphDefined.Vanaheimr.Aegir;
using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.WWCP.OCPIv2_1
{

    /// <summary>
    /// This class defines a geo location. The geodetic system to be used is WGS 84.
    /// </summary>
    public class AdditionalGeoLocation : GeoCoordinate
    {

        #region Properties

        #region Name

        private readonly I18NString _Name;

        /// <summary>
        /// An optional name for this geo location.
        /// </summary>
        /// <example>The street name of a parking lot entrance or it's number.</example>
        public I18NString Name
        {
            get
            {
                return _Name;
            }
        }

        #endregion

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new geographical coordinate or position on a map.
        /// </summary>
        /// <param name="Latitude">The Latitude (south to nord).</param>
        /// <param name="Longitude">The Longitude (parallel to equator).</param>
        /// <param name="Altitude">The (optional) Altitude.</param>
        /// <param name="Name">An optional name for this geo location.</param>
        public AdditionalGeoLocation(Latitude    Latitude,
                                     Longitude   Longitude,
                                     Altitude?   Altitude  = null,
                                     I18NString  Name      = null)

            : base(Latitude, Longitude, Altitude)

        {

            this._Name = Name != null ? Name : new I18NString();

        }

        #endregion

    }

}
