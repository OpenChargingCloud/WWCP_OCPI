/*
 * Copyright (c) 2015-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2_1
{

    /// <summary>
    /// Version endpoint information.
    /// </summary>
    public readonly struct VersionEndpoint : IEquatable<VersionEndpoint>,
                                             IComparable<VersionEndpoint>,
                                             IComparable
    {

        #region Properties

        /// <summary>
        /// The module identifier.
        /// </summary>
        [Mandatory]
        public Module_Id       Identifier    { get; }

        /// <summary>
        /// The interface role.
        /// </summary>
        [Mandatory]
        public InterfaceRoles  Role          { get; }

        /// <summary>
        /// The URL of the module.
        /// </summary>
        [Mandatory]
        public URL             URL           { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new version endpoint information.
        /// </summary>
        /// <param name="Identifier">The module identifier.</param>
        /// <param name="Role">The interface role.</param>
        /// <param name="URL">The URL of the module.</param>
        public VersionEndpoint(Module_Id       Identifier,
                               InterfaceRoles  Role,
                               URL             URL)
        {

            this.Identifier  = Identifier;
            this.Role        = Role;
            this.URL         = URL;

        }

        #endregion


        #region (static) Parse   (JSON, CustomVersionEndpointParser = null)

        /// <summary>
        /// Parse the given JSON representation of a version endpoint.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomVersionEndpointParser">A delegate to parse custom version endpoint JSON objects.</param>
        public static VersionEndpoint Parse(JObject                                        JSON,
                                            CustomJObjectParserDelegate<VersionEndpoint>?  CustomVersionEndpointParser   = null)
        {

            if (TryParse(JSON,
                         out var versionEndpoint,
                         out var errorResponse,
                         CustomVersionEndpointParser))
            {
                return versionEndpoint;
            }

            throw new ArgumentException("The given JSON representation of a version endpoint is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out VersionEndpoint, out ErrorResponse, CustomVersionEndpointParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a version endpoint.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="VersionEndpoint">The parsed version endpoint endpoint.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject              JSON,
                                       out VersionEndpoint  VersionEndpoint,
                                       out String?          ErrorResponse)

            => TryParse(JSON,
                        out VersionEndpoint,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a version endpoint.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="VersionEndpoint">The parsed version endpoint.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomVersionEndpointParser">A delegate to parse custom version JSON objects.</param>
        public static Boolean TryParse(JObject                                        JSON,
                                       out VersionEndpoint                            VersionEndpoint,
                                       out String?                                    ErrorResponse,
                                       CustomJObjectParserDelegate<VersionEndpoint>?  CustomVersionEndpointParser)
        {

            try
            {

                VersionEndpoint = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse Identifier     [mandatory]

                if (!JSON.ParseMandatory("identifier",
                                         "endpoint identifier",
                                         Module_Id.TryParse,
                                         out Module_Id Identifier,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Role           [mandatory]

                if (!JSON.ParseMandatory("role",
                                         "interface role",
                                         InterfaceRolesExtensions.TryParse,
                                         out InterfaceRoles Role,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse URL            [mandatory]

                if (!JSON.ParseMandatory("url",
                                         "version URL",
                                         org.GraphDefined.Vanaheimr.Hermod.HTTP.URL.TryParse,
                                         out URL URL,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                VersionEndpoint = new VersionEndpoint(Identifier,
                                                      Role,
                                                      URL);


                if (CustomVersionEndpointParser is not null)
                    VersionEndpoint = CustomVersionEndpointParser(JSON,
                                                                  VersionEndpoint);

                return true;

            }
            catch (Exception e)
            {
                VersionEndpoint  = default;
                ErrorResponse    = "The given JSON representation of a version is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomVersionEndpointSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomVersionEndpointSerializer">A delegate to serialize custom version JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<VersionEndpoint>?  CustomVersionEndpointSerializer   = null)
        {

            var json = JSONObject.Create(
                           new JProperty("identifier",  Identifier.ToString()),
                           new JProperty("role",        Role.      AsText()),
                           new JProperty("url",         URL.       ToString())
                       );

            return CustomVersionEndpointSerializer is not null
                       ? CustomVersionEndpointSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this object.
        /// </summary>
        public VersionEndpoint Clone()

            => new (Identifier.Clone,
                    Role,
                    URL.       Clone);

        #endregion


        #region Operator overloading

        #region Operator == (VersionEndpoint1, VersionEndpoint2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VersionEndpoint1">A version endpoint information.</param>
        /// <param name="VersionEndpoint2">Another version endpoint information.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (VersionEndpoint VersionEndpoint1,
                                           VersionEndpoint VersionEndpoint2)

            => VersionEndpoint1.Equals(VersionEndpoint2);

        #endregion

        #region Operator != (VersionEndpoint1, VersionEndpoint2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VersionEndpoint1">A version endpoint information.</param>
        /// <param name="VersionEndpoint2">Another version endpoint information.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (VersionEndpoint VersionEndpoint1,
                                           VersionEndpoint VersionEndpoint2)

            => !VersionEndpoint1.Equals(VersionEndpoint2);

        #endregion

        #region Operator <  (VersionEndpoint1, VersionEndpoint2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VersionEndpoint1">A version endpoint information.</param>
        /// <param name="VersionEndpoint2">Another version endpoint information.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (VersionEndpoint VersionEndpoint1,
                                          VersionEndpoint VersionEndpoint2)

            => VersionEndpoint1.CompareTo(VersionEndpoint2) < 0;

        #endregion

        #region Operator <= (VersionEndpoint1, VersionEndpoint2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VersionEndpoint1">A version endpoint information.</param>
        /// <param name="VersionEndpoint2">Another version endpoint information.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (VersionEndpoint VersionEndpoint1,
                                           VersionEndpoint VersionEndpoint2)

            => VersionEndpoint1.CompareTo(VersionEndpoint2) <= 0;

        #endregion

        #region Operator >  (VersionEndpoint1, VersionEndpoint2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VersionEndpoint1">A version endpoint information.</param>
        /// <param name="VersionEndpoint2">Another version endpoint information.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (VersionEndpoint VersionEndpoint1,
                                          VersionEndpoint VersionEndpoint2)

            => VersionEndpoint1.CompareTo(VersionEndpoint2) > 0;

        #endregion

        #region Operator >= (VersionEndpoint1, VersionEndpoint2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VersionEndpoint1">A version endpoint information.</param>
        /// <param name="VersionEndpoint2">Another version endpoint information.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (VersionEndpoint VersionEndpoint1,
                                           VersionEndpoint VersionEndpoint2)

            => VersionEndpoint1.CompareTo(VersionEndpoint2) >= 0;

        #endregion

        #endregion

        #region IComparable<VersionEndpoint> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two version endpoint information.
        /// </summary>
        /// <param name="Object">A version endpoint information to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is VersionEndpoint versionEndpoint
                   ? CompareTo(versionEndpoint)
                   : throw new ArgumentException("The given object is not a version endpoint!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(VersionEndpoint)

        /// <summary>
        /// Compares two version endpoint information.
        /// </summary>
        /// <param name="VersionEndpoint">A version endpoint information to compare with.</param>
        public Int32 CompareTo(VersionEndpoint VersionEndpoint)
        {

            var c = Identifier.CompareTo(VersionEndpoint.Identifier);

            if (c == 0)
                c = Role.      CompareTo(VersionEndpoint.Role);

            if (c == 0)
                c = URL.       CompareTo(VersionEndpoint.URL);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<VersionEndpoint> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two version endpoint information for equality.
        /// </summary>
        /// <param name="Object">A version endpoint information to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is VersionEndpoint versionEndpoint &&
                   Equals(versionEndpoint);

        #endregion

        #region Equals(VersionEndpoint)

        /// <summary>
        /// Compares two version endpoint information for equality.
        /// </summary>
        /// <param name="VersionEndpoint">A version endpoint information to compare with.</param>
        public Boolean Equals(VersionEndpoint VersionEndpoint)

            => Identifier.Equals(VersionEndpoint.Identifier) &&
               Role.      Equals(VersionEndpoint.Role)       &&
               URL.       Equals(VersionEndpoint.URL);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return Identifier.GetHashCode() * 5 ^
                       Role.      GetHashCode() * 3 ^
                       URL.       GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(Identifier, ", ", Role,
                             " -> ",
                             URL);

        #endregion

    }

}
