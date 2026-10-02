/*
 * Copyright (c) 2015-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPI <https://github.com/OpenChargingCloud/WWCP_OCPI>
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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPI
{

    /// <summary>
    /// An optional URL of a JSON object, refused while it is there but not
    /// valid.
    /// </summary>
    /// <remarks>
    /// ParseOptional of Illias with URL.TryParse takes it as a mapper, as
    /// URL.TryParse(String) returns a URL? - and a mapper's null is no URL at
    /// all, not one that is not valid: a URL not valid was passed over, and
    /// the rest read as if it were not there.
    /// </remarks>
    public static class OptionalURLExtensions
    {

        #region ParseOptionalURL(this JSON, PropertyName, PropertyDescription, out URL, out ErrorResponse)

        /// <summary>
        /// Parse the optional URL of the given property: true while it is
        /// there - with an error response while it is not valid - and false
        /// while it is not, as the other ParseOptional methods do.
        /// </summary>
        /// <param name="JSON">The JSON object.</param>
        /// <param name="PropertyName">The name of the property.</param>
        /// <param name="PropertyDescription">The description of the property, for the error response.</param>
        /// <param name="URL">The URL, while it is there and valid.</param>
        /// <param name="ErrorResponse">Why the URL is not valid.</param>
        public static Boolean ParseOptionalURL(this JObject  JSON,
                                               String        PropertyName,
                                               String        PropertyDescription,
                                               out URL?      URL,
                                               out String?   ErrorResponse)
        {

            URL            = null;
            ErrorResponse  = null;

            if (!JSON.TryGetValue(PropertyName, out var token) ||
                token.Type == JTokenType.Null)
            {
                return false;
            }

            if (token.Type == JTokenType.String &&
                org.GraphDefined.Vanaheimr.Hermod.HTTP.URL.TryParse(token.Value<String>() ?? "", out var url))
            {
                URL = url;
            }

            else
                ErrorResponse = $"The value '{token}' is not valid for JSON property '{PropertyDescription}'!";

            return true;

        }

        #endregion

    }

}
