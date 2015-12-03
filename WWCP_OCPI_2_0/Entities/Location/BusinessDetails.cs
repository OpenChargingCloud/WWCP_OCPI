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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.WWCP.OCPI_2_0
{

    /// <summary>
    /// This class references business details of EVSE operators.
    /// </summary>
    public class BusinessDetails
    {

        #region Properties

        #region Name

        private readonly I18NString _Name;

        /// <summary>
        /// Name of the operator.
        /// </summary>
        public I18NString Name
        {
            get
            {
                return _Name;
            }
        }

        #endregion

        #region Website

        private readonly Uri _Website;

        /// <summary>
        /// Link to the operator's website.
        /// </summary>
        public Uri Website
        {
            get
            {
                return _Website;
            }
        }

        #endregion

        #region Logo

        private readonly Image _Logo;

        /// <summary>
        /// Image link to the operator's logo.
        /// </summary>
        public Image Logo
        {
            get
            {
                return _Logo;
            }
        }

        #endregion

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new business details for an EVSE operator.
        /// </summary>
        /// <param name="Name">Name of the operator.</param>
        /// <param name="Website">Link to the operator's website.</param>
        /// <param name="Logo">Image link to the operator's logo.</param>
        public BusinessDetails(I18NString  Name,
                               Uri         Website,
                               Image       Logo)
        {

            #region Initial checks

            if (Name == null)
                throw new ArgumentNullException("Name", "The given parameter must not be null!");

            #endregion

            this._Name     = Name;
            this._Website  = Website;
            this._Logo     = Logo;

        }

        #endregion

    }

}
