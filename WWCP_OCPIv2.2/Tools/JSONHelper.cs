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
using System.Linq;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2
{

    public static class JSONHelper
    {

        #region ToJSON(this I18N)

        /// <summary>
        /// Create a JSON representation of the given internationalized string.
        /// </summary>
        /// <param name="I18N">An internationalized string.</param>
        public static JArray ToJSON(I18NString I18N)
        {

            return new JArray(I18N.SafeSelect(i18n => new JObject(new JProperty("language", i18n.Language.ToString()),
                                                                  new JProperty("text",     i18n.Text))));

        }

        #endregion

        #region ToJSON(JPropertyKey, this I18N)

        /// <summary>
        /// Create a JSON representation of the given internationalized string.
        /// </summary>
        /// <param name="JPropertyKey">The name of the JSON property key.</param>
        /// <param name="I18N">An internationalized string.</param>
        public static JProperty ToJSON(String JPropertyKey, I18NString I18N)
        {

            if (I18N.Any())
                return new JProperty(JPropertyKey, JSONHelper.ToJSON(I18N));
            else
                return null;

        }

        #endregion

    }

}
