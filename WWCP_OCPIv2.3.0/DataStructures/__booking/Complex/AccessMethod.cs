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

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_3_0
{

    /// <summary>
    /// An access method.
    /// </summary>
    public class AccessMethod : IEquatable<AccessMethod>
    {

        #region Properties

        /// <summary>
        /// The location access method.
        /// </summary>
        [Mandatory]
        public LocationAccess  LocationAccess    { get; }

        /// <summary>
        /// The optional value for the given location access option.
        /// For a license plate it would be: ABC12D, or for an access code: 1224
        /// </summary>
        [Optional]
        public String?         Value             { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new access method.
        /// </summary>
        /// <param name="LocationAccess">A location access method.</param>
        /// <param name="Value">An optional value for the given location access option.</param>
        public AccessMethod(LocationAccess  LocationAccess,
                            String?         Value   = null)
        {

            this.LocationAccess  = LocationAccess;
            this.Value           = Value;

            unchecked
            {

                hashCode = this.LocationAccess.GetHashCode() * 3 ^
                           this.Value?.        GetHashCode() ?? 0;

            }

        }

        #endregion


        #region (static) Parse   (JSON, CustomAccessMethodParser = null)

        /// <summary>
        /// Parse the given JSON representation of an access method.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomAccessMethodParser">A delegate to parse custom access method JSON objects.</param>
        public static AccessMethod Parse(JObject                                     JSON,
                                         CustomJObjectParserDelegate<AccessMethod>?  CustomAccessMethodParser   = null)
        {

            if (TryParse(JSON,
                         out var accessMethod,
                         out var errorResponse,
                         CustomAccessMethodParser))
            {
                return accessMethod;
            }

            throw new ArgumentException("The given JSON representation of an access method is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out AccessMethod, out ErrorResponse, CustomAccessMethodParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of an access method.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="AccessMethod">The parsed connector.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                 JSON,
                                       [NotNullWhen(true)]  out AccessMethod?  AccessMethod,
                                       [NotNullWhen(false)] out String?        ErrorResponse)

            => TryParse(JSON,
                        out AccessMethod,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of an access method.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="AccessMethod">The parsed connector.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomAccessMethodParser">A delegate to parse custom access method JSON objects.</param>
        public static Boolean TryParse(JObject                                     JSON,
                                       [NotNullWhen(true)]  out AccessMethod?      AccessMethod,
                                       [NotNullWhen(false)] out String?            ErrorResponse,
                                       CustomJObjectParserDelegate<AccessMethod>?  CustomAccessMethodParser)
        {

            try
            {

                AccessMethod = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse LocationAccess    [mandatory]

                if (!JSON.ParseMandatory("location_access",
                                         "location access",
                                         LocationAccess.TryParse,
                                         out LocationAccess locationAccess,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Value             [optional]

                if (!JSON.ParseOptionalText("value",
                                            "location access value",
                                            out String? value,
                                            out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                AccessMethod = new AccessMethod(
                                   locationAccess,
                                   value
                               );

                if (CustomAccessMethodParser is not null)
                    AccessMethod = CustomAccessMethodParser(JSON,
                                                            AccessMethod);

                return true;

            }
            catch (Exception e)
            {
                AccessMethod   = default;
                ErrorResponse  = "The given JSON representation of an access method is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomAccessMethodSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomAccessMethodSerializer">A delegate to serialize custom access method JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<AccessMethod>? CustomAccessMethodSerializer = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("location_access",   LocationAccess.ToString()),

                           Value.IsNotNullOrEmpty()
                               ? new JProperty("value",             Value)
                               : null

                       );

            return CustomAccessMethodSerializer is not null
                       ? CustomAccessMethodSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this access method.
        /// </summary>
        public AccessMethod Clone()

            => new (
                   LocationAccess.Clone(),
                   Value?.        CloneString()
               );

        #endregion


        #region Operator overloading

        #region Operator == (AccessMethod1, AccessMethod2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AccessMethod1">AccessMethod.</param>
        /// <param name="AccessMethod2">Other access method.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (AccessMethod AccessMethod1,
                                           AccessMethod AccessMethod2)
        {

            if (Object.ReferenceEquals(AccessMethod1, AccessMethod2))
                return true;

            if (AccessMethod1 is null || AccessMethod2 is null)
                return false;

            return AccessMethod1.Equals(AccessMethod2);

        }

        #endregion

        #region Operator != (AccessMethod1, AccessMethod2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AccessMethod1">AccessMethod.</param>
        /// <param name="AccessMethod2">Other access method.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (AccessMethod AccessMethod1,
                                           AccessMethod AccessMethod2)

            => !(AccessMethod1 == AccessMethod2);

        #endregion

        #endregion

        #region IEquatable<AccessMethod> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two access method for equality.
        /// </summary>
        /// <param name="Object">AccessMethod to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is AccessMethod accessMethod &&
                   Equals(accessMethod);

        #endregion

        #region Equals(AccessMethod)

        /// <summary>
        /// Compares two access method for equality.
        /// </summary>
        /// <param name="AccessMethod">AccessMethod to compare with.</param>
        public Boolean Equals(AccessMethod? AccessMethod)

            => AccessMethod is not null &&

               LocationAccess.Equals(AccessMethod.LocationAccess) &&

             ((Value is     null && AccessMethod.Value is     null)  ||
              (Value is not null && AccessMethod.Value is not null && Value.Equals(AccessMethod.Value)));

        #endregion

        #endregion

        #region (override) GetHashCode()

        private readonly Int32 hashCode;

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        public override Int32 GetHashCode()
            => hashCode;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   LocationAccess.ToString(),

                   Value.IsNotNullOrEmpty()
                       ? $": {Value}"
                       : ""

               );

        #endregion

    }

}
