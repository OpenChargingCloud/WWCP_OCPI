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
    /// Specifies one exceptional period for opening or access hours.
    /// </summary>
    public class ExceptionalPeriod
    {

        #region Properties

        #region Begin

        private readonly DateTime _Begin;

        /// <summary>
        /// Begin of the opening or access hours exception.
        /// </summary>
        public DateTime Begin
        {
            get
            {
                return _Begin;
            }
        }

        #endregion

        #region End

        private readonly DateTime _End;

        /// <summary>
        /// End of the opening or access hours exception.
        /// </summary>
        public DateTime End
        {
            get
            {
                return _End;
            }
        }

        #endregion

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new exceptional period for opening or access hours.
        /// </summary>
        /// <param name="Begin">Begin of the opening or access hours exception.</param>
        /// <param name="End">End of the opening or access hours exception.</param>
        public ExceptionalPeriod(DateTime  Begin,
                                 DateTime  End)
        {

            this._Begin  = Begin;
            this._End    = End;

        }

        #endregion

    }

}
