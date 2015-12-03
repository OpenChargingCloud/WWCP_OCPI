/*
 * Copyright (c) 2015 GraphDefined GmbH
 * This file is part of WWCP OCPI <https://github.com/GraphDefined/WWCP_OCPI>
 *
 * Licensed under the Affero GPL license Version 3.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.gnu.org/licenses/agpl.html
 *
 * Unless required by applicable law or agreed to in writing software
 * distributed under the License is distributed on an "AS IS" BASIS
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#region Usings

using System;
using System.Linq;
using System.Collections.Generic;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.WWCP.OCPI_2_0
{

    /// <summary>
    /// A Tariff Object consists of a list of one or more TariffElements,
    /// these elements can be used to create complex Tariff structures.
    /// When the list of elements contains more then 1 element, then the
    /// first tariff in the list with matching restrictions will be used.
    /// </summary>
    public class Tariff : AEMobilityEntity<Tariff_Id>,
                          IEquatable<Tariff>, IComparable<Tariff>, IComparable
    {

        #region Properties

        #region Currency

        private readonly Currency _Currency;

        /// <summary>
        /// ISO 4217 code of the currency used for this tariff.
        /// </summary>
        [Mandatory]
        public Currency Currency
        {
            get
            {
                return _Currency;
            }
        }

        #endregion

        #region TariffText

        private readonly I18NString _TariffText;

        /// <summary>
        /// List of multi-language alternative tariff info text.
        /// </summary>
        [Mandatory]
        public I18NString TariffText
        {
            get
            {
                return _TariffText;
            }
        }

        #endregion

        #region TariffUrl

        private readonly Uri _TariffUrl;

        /// <summary>
        /// Alternative URL to tariff info.
        /// </summary>
        [Optional]
        public Uri TariffUrl
        {
            get
            {
                return _TariffUrl;
            }
        }

        #endregion

        #region TariffElements

        private readonly IEnumerable<TariffElement> _TariffElements;

        /// <summary>
        /// An enumeration of tariff elements.
        /// </summary>
        [Mandatory]
        public IEnumerable<TariffElement> TariffElements
        {
            get
            {
                return _TariffElements;
            }
        }

        #endregion

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new tariff object.
        /// </summary>
        /// <param name="Id">Uniquely identifies the Tariff within the CPOs platform (and suboperator platforms).</param>
        /// <param name="Currency">ISO 4217 code of the currency used for this tariff.</param>
        /// <param name="TariffText">List of multi-language alternative tariff info text.</param>
        /// <param name="TariffUrl">Alternative URL to tariff info.</param>
        /// <param name="TariffElements">An enumeration of tariff elements.</param>
        public Tariff(Tariff_Id                   Id,
                      Currency                    Currency,
                      I18NString                  TariffText,
                      Uri                         TariffUrl,
                      IEnumerable<TariffElement>  TariffElements)

            : base(Id)

        {

            #region Initial checks

            if (TariffElements == null)
                throw new ArgumentNullException("TariffElements", "The given parameter must not be null!");

            if (!TariffElements.Any())
                throw new ArgumentNullException("TariffElements", "The given enumeration must not be empty!");

            #endregion

            #region Init data and properties

            this._Currency        = Currency;
            this._TariffText      = TariffText != null ? TariffText : new I18NString();
            this._TariffUrl       = TariffUrl;
            this._TariffElements  = TariffElements;

            #endregion

        }

        #endregion


        #region IComparable<Tariff> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)
        {

            if (Object == null)
                throw new ArgumentNullException("The given object must not be null!");

            // Check if the given object is an Tariff.
            var Tariff = Object as Tariff;
            if ((Object) Tariff == null)
                throw new ArgumentException("The given object is not an Tariff!");

            return CompareTo(Tariff);

        }

        #endregion

        #region CompareTo(Tariff)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Tariff">An Tariff to compare with.</param>
        public Int32 CompareTo(Tariff Tariff)
        {

            if ((Object) Tariff == null)
                throw new ArgumentNullException("The given Tariff must not be null!");

            return _Id.CompareTo(Tariff._Id);

        }

        #endregion

        #endregion

        #region IEquatable<Tariff> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)
        {

            if (Object == null)
                return false;

            // Check if the given object is an Tariff.
            var Tariff = Object as Tariff;
            if ((Object) Tariff == null)
                return false;

            return this.Equals(Tariff);

        }

        #endregion

        #region Equals(Tariff)

        /// <summary>
        /// Compares two Tariffs for equality.
        /// </summary>
        /// <param name="Tariff">An Tariff to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(Tariff Tariff)
        {

            if ((Object) Tariff == null)
                return false;

            return _Id.Equals(Tariff._Id);

        }

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Get the hashcode of this object.
        /// </summary>
        public override Int32 GetHashCode()
        {
            return _Id.GetHashCode();
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Get a string representation of this object.
        /// </summary>
        public override String ToString()
        {
            return _Id.ToString();
        }

        #endregion

    }

}
