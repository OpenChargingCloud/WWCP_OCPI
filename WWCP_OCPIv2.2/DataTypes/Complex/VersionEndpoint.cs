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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2
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
        public ModuleIDs       Identifier    { get; }

        /// <summary>
        /// The interface role.
        /// </summary>
        [Mandatory]
        public InterfaceRoles  Role          { get; }

        /// <summary>
        /// The URL of the module.
        /// </summary>
        [Mandatory]
        public String          URL           { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new version endpoint information.
        /// </summary>
        /// <param name="Identifier">The module identifier.</param>
        /// <param name="Role">The interface role.</param>
        /// <param name="URL">The URL of the module.</param>
        public VersionEndpoint(ModuleIDs       Identifier,
                               InterfaceRoles  Role,
                               String          URL)
        {

            if (URL.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(URL), "The given version URL must not be null or empty!");

            this.Identifier  = Identifier;
            this.Role        = Role;
            this.URL         = URL?.Trim();

        }

        #endregion


        #region (static) Parse   (JSON, CustomVersionEndpointParser = null)

        /// <summary>
        /// Parse the given JSON representation of a version endpoint.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomVersionEndpointParser">A delegate to parse custom version endpoint JSON objects.</param>
        public static VersionEndpoint Parse(JObject                                       JSON,
                                            CustomJObjectParserDelegate<VersionEndpoint>  CustomVersionEndpointParser   = null)
        {

            if (TryParse(JSON,
                         out VersionEndpoint  versionEndpoint,
                         out String           ErrorResponse,
                         CustomVersionEndpointParser))
            {
                return versionEndpoint;
            }

            throw new ArgumentException("The given JSON representation of a version endpoint is invalid: " + ErrorResponse, nameof(JSON));

        }

        #endregion

        #region (static) Parse   (Text, CustomVersionEndpointParser = null)

        /// <summary>
        /// Parse the given text representation of a version endpoint.
        /// </summary>
        /// <param name="Text">The text to parse.</param>
        /// <param name="CustomVersionEndpointParser">A delegate to parse custom version endpoint JSON objects.</param>
        public static VersionEndpoint Parse(String                               Text,
                                   CustomJObjectParserDelegate<VersionEndpoint>  CustomVersionEndpointParser   = null)
        {

            if (TryParse(Text,
                         out VersionEndpoint  versionEndpoint,
                         out String           ErrorResponse,
                         CustomVersionEndpointParser))
            {
                return versionEndpoint;
            }

            throw new ArgumentException("The given text representation of a version endpoint is invalid: " + ErrorResponse, nameof(Text));

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
                                       out String           ErrorResponse)

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
        public static Boolean TryParse(JObject                                       JSON,
                                       out VersionEndpoint                           VersionEndpoint,
                                       out String                                    ErrorResponse,
                                       CustomJObjectParserDelegate<VersionEndpoint>  CustomVersionEndpointParser)
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

                if (!JSON.ParseMandatoryEnum("identifier",
                                             "endpoint identifier",
                                             out ModuleIDs Identifier,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Role           [mandatory]

                if (!JSON.ParseMandatoryEnum("role",
                                             "interface role",
                                             out InterfaceRoles Role,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse URL            [mandatory]

                if (!JSON.ParseMandatoryText("url",
                                             "version URL",
                                             out String URL,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion


                VersionEndpoint = new VersionEndpoint(Identifier,
                                                      Role,
                                                      URL);


                if (CustomVersionEndpointParser != null)
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

        #region (static) TryParse(Text, out VersionEndpoint, out ErrorResponse, CustomVersionEndpointParser = null)

        /// <summary>
        /// Try to parse the given text representation of a version.
        /// </summary>
        /// <param name="Text">The text to parse.</param>
        /// <param name="VersionEndpoint">The parsed version.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomVersionEndpointParser">A delegate to parse custom version JSON objects.</param>
        public static Boolean TryParse(String                                        Text,
                                       out VersionEndpoint                           VersionEndpoint,
                                       out String                                    ErrorResponse,
                                       CustomJObjectParserDelegate<VersionEndpoint>  CustomVersionEndpointParser   = null)
        {

            try
            {

                return TryParse(JObject.Parse(Text),
                                out VersionEndpoint,
                                out ErrorResponse,
                                CustomVersionEndpointParser);

            }
            catch (Exception e)
            {
                VersionEndpoint  = default;
                ErrorResponse    = "The given text representation of a version is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomVersionEndpointSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomVersionEndpointSerializer">A delegate to serialize custom version JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<VersionEndpoint> CustomVersionEndpointSerializer   = null)
        {

            var JSON = JSONObject.Create(
                           new JProperty("identifier",  Identifier.ToString().ToLower()),
                           new JProperty("role",        Role.      ToString()),
                           new JProperty("url",         URL)
                       );

            return CustomVersionEndpointSerializer != null
                       ? CustomVersionEndpointSerializer(this, JSON)
                       : JSON;

        }

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

            => !(VersionEndpoint1 == VersionEndpoint2);

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

            => !(VersionEndpoint1 > VersionEndpoint2);

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

            => !(VersionEndpoint1 < VersionEndpoint2);

        #endregion

        #endregion

        #region IComparable<VersionEndpoint> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is VersionEndpoint versionEndpoint
                   ? CompareTo(versionEndpoint)
                   : throw new ArgumentException("The given object is not a version!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(VersionEndpoint)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VersionEndpoint">An object to compare with.</param>
        public Int32 CompareTo(VersionEndpoint VersionEndpoint)
        {

            var c = Identifier.CompareTo(VersionEndpoint.Identifier);

            if (c == 0)
                c = Role.CompareTo(VersionEndpoint.Role);

            if (c == 0)
                c = URL.CompareTo(VersionEndpoint.URL);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<VersionEndpoint> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is VersionEndpoint versionEndpoint &&
                   Equals(versionEndpoint);

        #endregion

        #region Equals(VersionEndpoint)

        /// <summary>
        /// Compares two version endpoint informations for equality.
        /// </summary>
        /// <param name="VersionEndpoint">A version endpoint information to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(VersionEndpoint VersionEndpoint)

            => Identifier.Equals(VersionEndpoint.Identifier) &&
               Role.      Equals(VersionEndpoint.Role)       &&
               URL.       Equals(VersionEndpoint.URL);

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
                             " -> ", URL);

        #endregion

    }

}
