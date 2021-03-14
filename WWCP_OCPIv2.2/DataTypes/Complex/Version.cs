/*
 * Copyright (c) 2015-2021 GraphDefined GmbH
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
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2
{

    /// <summary>
    /// Version informations.
    /// </summary>
    public readonly struct Version : IEquatable<Version>,
                                     IComparable<Version>,
                                     IComparable
    {

        #region Properties

        /// <summary>
        /// The version identification.
        /// </summary>
        [Mandatory]
        public Version_Id  Id     { get; }

        /// <summary>
        /// The URL of the version.
        /// </summary>
        [Mandatory]
        public URL         URL    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new version information.
        /// </summary>
        /// <param name="Id">The version identification.</param>
        /// <param name="URL">The URL of the version.</param>
        public Version(Version_Id  Id,
                       URL         URL)
        {

            if (Id. IsNullOrEmpty)
                throw new ArgumentNullException(nameof(Id),   "The given version identification must not be null or empty!");

            if (URL.IsNullOrEmpty)
                throw new ArgumentNullException(nameof(URL),  "The given version URL must not be null or empty!");


            this.Id   = Id;
            this.URL  = URL;

        }

        #endregion


        #region (static) Parse   (JSON, CustomVersionParser = null)

        /// <summary>
        /// Parse the given JSON representation of a version.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomVersionParser">A delegate to parse custom version JSON objects.</param>
        public static Version Parse(JObject                               JSON,
                                    CustomJObjectParserDelegate<Version>  CustomVersionParser   = null)
        {

            if (TryParse(JSON,
                         out Version  version,
                         out String   ErrorResponse,
                         CustomVersionParser))
            {
                return version;
            }

            throw new ArgumentException("The given JSON representation of a version is invalid: " + ErrorResponse, nameof(JSON));

        }

        #endregion

        #region (static) Parse   (Text, CustomVersionParser = null)

        /// <summary>
        /// Parse the given text representation of a version.
        /// </summary>
        /// <param name="Text">The text to parse.</param>
        /// <param name="CustomVersionParser">A delegate to parse custom version JSON objects.</param>
        public static Version Parse(String                               Text,
                                   CustomJObjectParserDelegate<Version>  CustomVersionParser   = null)
        {

            if (TryParse(Text,
                         out Version  version,
                         out String   ErrorResponse,
                         CustomVersionParser))
            {
                return version;
            }

            throw new ArgumentException("The given text representation of a version is invalid: " + ErrorResponse, nameof(Text));

        }

        #endregion

        #region (static) TryParse(JSON, out Version, out ErrorResponse, CustomVersionParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a version.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="Version">The parsed version.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject      JSON,
                                       out Version  Version,
                                       out String   ErrorResponse)

            => TryParse(JSON,
                        out Version,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a version.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="Version">The parsed version.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomVersionParser">A delegate to parse custom version JSON objects.</param>
        public static Boolean TryParse(JObject                               JSON,
                                       out Version                           Version,
                                       out String                            ErrorResponse,
                                       CustomJObjectParserDelegate<Version>  CustomVersionParser)
        {

            try
            {

                Version = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse VersionId     [mandatory]

                if (!JSON.ParseMandatory("version",
                                         "version identification",
                                         Version_Id.TryParse,
                                         out Version_Id Id,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse URL           [mandatory]

                if (!JSON.ParseMandatory("url",
                                         "version URL",
                                         org.GraphDefined.Vanaheimr.Hermod.HTTP.URL.TryParse,
                                         out URL URL,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                Version = new Version(Id,
                                      URL);


                if (CustomVersionParser != null)
                    Version = CustomVersionParser(JSON,
                                                  Version);

                return true;

            }
            catch (Exception e)
            {
                Version        = default;
                ErrorResponse  = "The given JSON representation of a version is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Text, out Version, out ErrorResponse, CustomVersionParser = null)

        /// <summary>
        /// Try to parse the given text representation of a version.
        /// </summary>
        /// <param name="Text">The text to parse.</param>
        /// <param name="Version">The parsed version.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomVersionParser">A delegate to parse custom version JSON objects.</param>
        public static Boolean TryParse(String                                Text,
                                       out Version                           Version,
                                       out String                            ErrorResponse,
                                       CustomJObjectParserDelegate<Version>  CustomVersionParser   = null)
        {

            try
            {

                return TryParse(JObject.Parse(Text),
                                out Version,
                                out ErrorResponse,
                                CustomVersionParser);

            }
            catch (Exception e)
            {
                Version        = default;
                ErrorResponse  = "The given text representation of a version is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomVersionSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomVersionSerializer">A delegate to serialize custom version JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<Version> CustomVersionSerializer   = null)
        {

            var JSON = JSONObject.Create(
                           new JProperty("version",  Id. ToString()),
                           new JProperty("url",      URL.ToString())
                       );

            return CustomVersionSerializer != null
                       ? CustomVersionSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Operator overloading

        #region Operator == (Version1, Version2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Version1">A version.</param>
        /// <param name="Version2">Another version.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Version Version1,
                                           Version Version2)

            => Version1.Equals(Version2);

        #endregion

        #region Operator != (Version1, Version2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Version1">A version.</param>
        /// <param name="Version2">Another version.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Version Version1,
                                           Version Version2)

            => !(Version1 == Version2);

        #endregion

        #region Operator <  (Version1, Version2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Version1">A version.</param>
        /// <param name="Version2">Another version.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Version Version1,
                                          Version Version2)

            => Version1.CompareTo(Version2) < 0;

        #endregion

        #region Operator <= (Version1, Version2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Version1">A version.</param>
        /// <param name="Version2">Another version.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Version Version1,
                                           Version Version2)

            => !(Version1 > Version2);

        #endregion

        #region Operator >  (Version1, Version2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Version1">A version.</param>
        /// <param name="Version2">Another version.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Version Version1,
                                          Version Version2)

            => Version1.CompareTo(Version2) > 0;

        #endregion

        #region Operator >= (Version1, Version2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Version1">A version.</param>
        /// <param name="Version2">Another version.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Version Version1,
                                           Version Version2)

            => !(Version1 < Version2);

        #endregion

        #endregion

        #region IComparable<Version> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is Version version
                   ? CompareTo(version)
                   : throw new ArgumentException("The given object is not a version!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(Version)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Version">An object to compare with.</param>
        public Int32 CompareTo(Version Version)
        {

            var result = Id.CompareTo(Version.Id);

            if (result == 0)
                result = URL.CompareTo(Version.URL);

            return result;

        }

        #endregion

        #endregion

        #region IEquatable<Version> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is Version version &&
                   Equals(version);

        #endregion

        #region Equals(Version)

        /// <summary>
        /// Compares two versions for equality.
        /// </summary>
        /// <param name="Version">A version to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(Version Version)

            => Id. Equals(Version.Id) &&
               URL.Equals(Version.URL);

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return Id. GetHashCode() * 3 ^
                       URL.GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(Id,
                             " -> ",
                             URL);

        #endregion

    }

}
