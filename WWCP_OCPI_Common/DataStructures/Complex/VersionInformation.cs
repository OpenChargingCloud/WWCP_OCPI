﻿/*
 * Copyright (c) 2015-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

namespace cloud.charging.open.protocols.OCPI
{

    /// <summary>
    /// Version information.
    /// </summary>
    public class VersionInformation : IEquatable<VersionInformation>,
                                      IComparable<VersionInformation>,
                                      IComparable
    {

        #region Properties

        /// <summary>
        /// The version information identification.
        /// </summary>
        [Mandatory]
        public Version_Id  Id     { get; }

        /// <summary>
        /// The URL of the version information.
        /// </summary>
        [Mandatory]
        public URL         URL    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new version information information.
        /// </summary>
        /// <param name="Id">The version information identification.</param>
        /// <param name="URL">The URL of the version information.</param>
        public VersionInformation(Version_Id  Id,
                                  URL         URL)
        {

            this.Id   = Id;
            this.URL  = URL;

        }

        #endregion


        #region (static) Parse   (JSON, CustomVersionInformationParser = null)

        /// <summary>
        /// Parse the given JSON representation of a version information.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomVersionInformationParser">A delegate to parse custom version information JSON objects.</param>
        public static VersionInformation Parse(JObject                                           JSON,
                                               CustomJObjectParserDelegate<VersionInformation>?  CustomVersionInformationParser   = null)
        {

            if (TryParse(JSON,
                         out var versionInformation,
                         out var errorResponse,
                         CustomVersionInformationParser))
            {
                return versionInformation;
            }

            throw new ArgumentException("The given JSON representation of a version information is invalid: " + errorResponse, nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out Version, out ErrorResponse, CustomVersionInformationParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a version information.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="Version">The parsed version information.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                       JSON,
                                       [NotNullWhen(true)]  out VersionInformation?  Version,
                                       [NotNullWhen(false)] out String?              ErrorResponse)

            => TryParse(JSON,
                        out Version,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a version information.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="Version">The parsed version information.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomVersionInformationParser">A delegate to parse custom version information JSON objects.</param>
        public static Boolean TryParse(JObject                                           JSON,
                                       [NotNullWhen(true)]  out VersionInformation?      Version,
                                       [NotNullWhen(false)] out String?                  ErrorResponse,
                                       CustomJObjectParserDelegate<VersionInformation>?  CustomVersionInformationParser)
        {

            try
            {

                Version = null;

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


                Version = new VersionInformation(
                              Id,
                              URL
                          );


                if (CustomVersionInformationParser is not null)
                    Version = CustomVersionInformationParser(JSON,
                                                             Version);

                return true;

            }
            catch (Exception e)
            {
                Version        = default;
                ErrorResponse  = "The given JSON representation of a version information is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomVersionInformationSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomVersionInformationSerializer">A delegate to serialize custom version information JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<VersionInformation>? CustomVersionInformationSerializer = null)
        {

            var json = JSONObject.Create(
                           new JProperty("version",  Id. ToString()),
                           new JProperty("url",      URL.ToString())
                       );

            return CustomVersionInformationSerializer is not null
                       ? CustomVersionInformationSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this version information.
        /// </summary>
        public VersionInformation Clone()

            => new (
                   Id. Clone(),
                   URL.Clone()
               );

        #endregion


        #region Operator overloading

        #region Operator == (VersionInformation1, VersionInformation2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VersionInformation1">A version information.</param>
        /// <param name="VersionInformation2">Another version information.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (VersionInformation? VersionInformation1,
                                           VersionInformation? VersionInformation2)
        {

            if (Object.ReferenceEquals(VersionInformation1, VersionInformation2))
                return true;

            if (VersionInformation1 is null || VersionInformation2 is null)
                return false;

            return VersionInformation1.Equals(VersionInformation2);

        }

        #endregion

        #region Operator != (VersionInformation1, VersionInformation2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VersionInformation1">A version information.</param>
        /// <param name="VersionInformation2">Another version information.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (VersionInformation? VersionInformation1,
                                           VersionInformation? VersionInformation2)

            => !(VersionInformation1 == VersionInformation2);

        #endregion

        #region Operator <  (VersionInformation1, VersionInformation2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VersionInformation1">A version information.</param>
        /// <param name="VersionInformation2">Another version information.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (VersionInformation? VersionInformation1,
                                          VersionInformation? VersionInformation2)

            => VersionInformation1 is null
                   ? throw new ArgumentNullException(nameof(VersionInformation1), "The given version information must not be null!")
                   : VersionInformation1.CompareTo(VersionInformation2) < 0;

        #endregion

        #region Operator <= (VersionInformation1, VersionInformation2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VersionInformation1">A version information.</param>
        /// <param name="VersionInformation2">Another version information.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (VersionInformation? VersionInformation1,
                                           VersionInformation? VersionInformation2)

            => !(VersionInformation1 > VersionInformation2);

        #endregion

        #region Operator >  (VersionInformation1, VersionInformation2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VersionInformation1">A version information.</param>
        /// <param name="VersionInformation2">Another version information.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (VersionInformation? VersionInformation1,
                                          VersionInformation? VersionInformation2)

            => VersionInformation1 is null
                   ? throw new ArgumentNullException(nameof(VersionInformation1), "The given version information must not be null!")
                   : VersionInformation1.CompareTo(VersionInformation2) > 0;

        #endregion

        #region Operator >= (VersionInformation1, VersionInformation2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VersionInformation1">A version information.</param>
        /// <param name="VersionInformation2">Another version information.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (VersionInformation? VersionInformation1,
                                           VersionInformation? VersionInformation2)

            => !(VersionInformation1 < VersionInformation2);

        #endregion

        #endregion

        #region IComparable<Version> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two version information.
        /// </summary>
        /// <param name="Object">A version information to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is VersionInformation versionInformation
                   ? CompareTo(versionInformation)
                   : throw new ArgumentException("The given object is not a version information!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(Version)

        /// <summary>
        /// Compares two version information.
        /// </summary>
        /// <param name="VersionInformation">A version information to compare with.</param>
        public Int32 CompareTo(VersionInformation? VersionInformation)
        {

            if (VersionInformation is null)
                throw new ArgumentNullException(nameof(VersionInformation), "The given version information must not be null!");

            var c = Id. CompareTo(VersionInformation.Id);

            if (c == 0)
                c = URL.CompareTo(VersionInformation.URL);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<Version> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two version information for equality.
        /// </summary>
        /// <param name="Object">A version information to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is VersionInformation versionInformation &&
                   Equals(versionInformation);

        #endregion

        #region Equals(Version)

        /// <summary>
        /// Compares two version information for equality.
        /// </summary>
        /// <param name="VersionInformation">A version information to compare with.</param>
        public Boolean Equals(VersionInformation? VersionInformation)

            => VersionInformation is not null &&

               Id. Equals(VersionInformation.Id) &&
               URL.Equals(VersionInformation.URL);

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
