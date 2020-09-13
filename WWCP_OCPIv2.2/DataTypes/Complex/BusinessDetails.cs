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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2
{

    /// <summary>
    /// This class references business details.
    /// </summary>
    public class BusinessDetails : IEquatable<BusinessDetails>,
                                   IComparable<BusinessDetails>,
                                   IComparable
    {

        #region Properties

        /// <summary>
        /// Name of the operator.
        /// </summary>
        public String  Name       { get; }

        /// <summary>
        /// Optinal link to the operator's website.
        /// </summary>
        public String  Website    { get; }

        /// <summary>
        /// Optinal image link to the operator's logo.
        /// </summary>
        public Image   Logo       { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new business details.
        /// </summary>
        /// <param name="Name">Name of the operator.</param>
        /// <param name="Website">Optinal link to the operator's website.</param>
        /// <param name="Logo">Optinal image link to the operator's logo.</param>
        public BusinessDetails(String  Name,
                               String  Website   = null,
                               Image   Logo      = null)
        {

            #region Initial checks

            if (Name.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Name), "The given name must not be null or empty!");

            #endregion

            this.Name     = Name?.   Trim();
            this.Website  = Website?.Trim();
            this.Logo     = Logo;

        }

        #endregion


        #region Operator overloading

        #region Operator == (BusinessDetails1, BusinessDetails2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BusinessDetails1">A business detail.</param>
        /// <param name="BusinessDetails2">Another business detail.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (BusinessDetails BusinessDetails1,
                                           BusinessDetails BusinessDetails2)

            => BusinessDetails1.Equals(BusinessDetails2);

        #endregion

        #region Operator != (BusinessDetails1, BusinessDetails2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BusinessDetails1">A business detail.</param>
        /// <param name="BusinessDetails2">Another business detail.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (BusinessDetails BusinessDetails1,
                                           BusinessDetails BusinessDetails2)

            => !(BusinessDetails1 == BusinessDetails2);

        #endregion

        #region Operator <  (BusinessDetails1, BusinessDetails2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BusinessDetails1">A business detail.</param>
        /// <param name="BusinessDetails2">Another business detail.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (BusinessDetails BusinessDetails1,
                                          BusinessDetails BusinessDetails2)

            => BusinessDetails1.CompareTo(BusinessDetails2) < 0;

        #endregion

        #region Operator <= (BusinessDetails1, BusinessDetails2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BusinessDetails1">A business detail.</param>
        /// <param name="BusinessDetails2">Another business detail.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (BusinessDetails BusinessDetails1,
                                           BusinessDetails BusinessDetails2)

            => !(BusinessDetails1 > BusinessDetails2);

        #endregion

        #region Operator >  (BusinessDetails1, BusinessDetails2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BusinessDetails1">A business detail.</param>
        /// <param name="BusinessDetails2">Another business detail.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (BusinessDetails BusinessDetails1,
                                          BusinessDetails BusinessDetails2)

            => BusinessDetails1.CompareTo(BusinessDetails2) > 0;

        #endregion

        #region Operator >= (BusinessDetails1, BusinessDetails2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BusinessDetails1">A business detail.</param>
        /// <param name="BusinessDetails2">Another business detail.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (BusinessDetails BusinessDetails1,
                                           BusinessDetails BusinessDetails2)

            => !(BusinessDetails1 < BusinessDetails2);

        #endregion

        #endregion

        #region IComparable<BusinessDetails> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is BusinessDetails chargingPeriod
                   ? CompareTo(chargingPeriod)
                   : throw new ArgumentException("The given object is not a business detail!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(BusinessDetails)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BusinessDetails">An object to compare with.</param>
        public Int32 CompareTo(BusinessDetails BusinessDetails)
        {

            var c = Name.   CompareTo(BusinessDetails.Name);

            if (c == 0)
                c = Website.IsNotNullOrEmpty() && BusinessDetails.Website.IsNotNullOrEmpty()
                        ? 0
                        : Website.IsNotNullOrEmpty()
                              ? -1
                              : Website.CompareTo(BusinessDetails.Website);

            if (c == 0)
                c = Logo == null && BusinessDetails.Logo == null
                        ? 0
                        : Logo == null
                              ? -1
                              : Logo.CompareTo(BusinessDetails.Logo);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<BusinessDetails> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is BusinessDetails BusinessDetails &&
                   Equals(BusinessDetails);

        #endregion

        #region Equals(BusinessDetails)

        /// <summary>
        /// Compares two business details for equality.
        /// </summary>
        /// <param name="BusinessDetails">A business detail to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(BusinessDetails BusinessDetails)

            =>   Name.Equals(BusinessDetails.Name) &&

               ((Website.IsNullOrEmpty()         && BusinessDetails.Website.IsNullOrEmpty()) ||
                (Website.IsNeitherNullNorEmpty() && BusinessDetails.Website.IsNeitherNullNorEmpty() && Website.Equals(BusinessDetails.Website))) &&

               ((Logo == null                    && BusinessDetails.Logo == null) ||
                (Logo != null                    && BusinessDetails.Logo != null                    && Logo.   Equals(BusinessDetails.Logo)));

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

                return        Name.   GetHashCode() * 5 ^

                       (Website.IsNotNullOrEmpty()
                            ? Website.GetHashCode() * 3
                            : 0) ^

                       (Logo    != null
                            ? Logo.   GetHashCode()
                            : 0);

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(Name,
                             Website.IsNotNullOrEmpty()
                                 ? "; " + Website
                                 : "",
                             Logo != null
                                 ? "; " + Logo
                                 : "");

        #endregion

    }

}
