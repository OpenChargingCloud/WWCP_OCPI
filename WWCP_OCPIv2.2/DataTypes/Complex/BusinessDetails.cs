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
        public I18NString  Name       { get; }

        /// <summary>
        /// Link to the operator's website.
        /// </summary>
        public Uri         Website    { get; }

        /// <summary>
        /// Image link to the operator's logo.
        /// </summary>
        public Image       Logo       { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new business details.
        /// </summary>
        /// <param name="Name">Name of the operator.</param>
        /// <param name="Website">Link to the operator's website.</param>
        /// <param name="Logo">Image link to the operator's logo.</param>
        public BusinessDetails(I18NString  Name,
                               Uri         Website,
                               Image       Logo)
        {

            #region Initial checks

            if (Name.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Name), "The given name must not be null or empty!");

            #endregion

            this.Name     = Name;
            this.Website  = Website;
            this.Logo     = Logo;

        }

        #endregion

    }

}
