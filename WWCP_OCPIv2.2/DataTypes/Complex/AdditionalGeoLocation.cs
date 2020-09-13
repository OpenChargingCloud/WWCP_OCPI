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

using org.GraphDefined.Vanaheimr.Aegir;
using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2
{

    /// <summary>
    /// This class defines a geo location. The geodetic system to be used is WGS 84.
    /// </summary>
    public readonly struct AdditionalGeoLocation : IEquatable<AdditionalGeoLocation>
    {

        #region Properties

        /// <summary>
        /// The geo location.
        /// </summary>
        public GeoCoordinate  GeoLocation    { get; }

        /// <summary>
        /// An optional name for this geo location.
        /// </summary>
        /// <example>The street name of a parking lot entrance or it's number.</example>
        public I18NString     Name           { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new geographical coordinate or position on a map.
        /// </summary>
        /// <param name="GeoLocation">The geo location.</param>
        /// <param name="Name">An optional name for this geo location.</param>
        public AdditionalGeoLocation(GeoCoordinate  GeoLocation,
                                     I18NString     Name   = null)
        {

            this.GeoLocation  = GeoLocation;
            this.Name         = Name ?? new I18NString();

        }

        /// <summary>
        /// Create a new geographical coordinate or position on a map.
        /// </summary>
        /// <param name="Latitude">The Latitude (south to nord).</param>
        /// <param name="Longitude">The Longitude (parallel to equator).</param>
        /// <param name="Altitude">The (optional) Altitude.</param>
        /// <param name="Name">An optional name for this geo location.</param>
        public AdditionalGeoLocation(Latitude    Latitude,
                                     Longitude   Longitude,
                                     Altitude?   Altitude   = null,
                                     I18NString  Name       = null)

            : this(new GeoCoordinate(Latitude,
                                     Longitude,
                                     Altitude),
                   Name)

        { }

        #endregion


        #region Operator overloading

        #region Operator == (AdditionalGeoLocation1, AdditionalGeoLocation2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AdditionalGeoLocation1">An additional geo location.</param>
        /// <param name="AdditionalGeoLocation2">Another additional geo location.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (AdditionalGeoLocation AdditionalGeoLocation1,
                                           AdditionalGeoLocation AdditionalGeoLocation2)

            => AdditionalGeoLocation1.Equals(AdditionalGeoLocation2);

        #endregion

        #region Operator != (AdditionalGeoLocation1, AdditionalGeoLocation2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AdditionalGeoLocation1">An additional geo location.</param>
        /// <param name="AdditionalGeoLocation2">Another additional geo location.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (AdditionalGeoLocation AdditionalGeoLocation1,
                                           AdditionalGeoLocation AdditionalGeoLocation2)

            => !(AdditionalGeoLocation1 == AdditionalGeoLocation2);

        #endregion

        #endregion

        #region IEquatable<AdditionalGeoLocation> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is AdditionalGeoLocation AdditionalGeoLocation &&
                   Equals(AdditionalGeoLocation);

        #endregion

        #region Equals(AdditionalGeoLocation)

        /// <summary>
        /// Compares two AdditionalGeoLocations for equality.
        /// </summary>
        /// <param name="AdditionalGeoLocation">A AdditionalGeoLocation to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(AdditionalGeoLocation AdditionalGeoLocation)

            => GeoLocation.Equals(AdditionalGeoLocation.GeoLocation) &&

               ((Name.IsNullOrEmpty()         && AdditionalGeoLocation.Name.IsNullOrEmpty()) ||
                (Name.IsNeitherNullNorEmpty() && AdditionalGeoLocation.Name.IsNeitherNullNorEmpty() && Name.Equals(AdditionalGeoLocation.Name)));

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

                return GeoLocation.GetHashCode() * 3 ^

                       (Name.IsNeitherNullNorEmpty()
                            ? Name.GetHashCode()
                            : 0);

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(GeoLocation.Latitude,
                             " / ",
                             GeoLocation.Longitude,
                             Name.IsNeitherNullNorEmpty()
                                 ? " : Name = " + Name.ToString()
                                 : "");

        #endregion

    }

}
