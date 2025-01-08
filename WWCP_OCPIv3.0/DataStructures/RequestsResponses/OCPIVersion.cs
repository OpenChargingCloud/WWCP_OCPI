/*
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

namespace cloud.charging.open.protocols.OCPIv3_0
{

    /// <summary>
    /// An OCPI Version.
    /// </summary>
    public class OCPIVersion
    {

        #region Properties

        /// <summary>
        /// The version number of the OCPI version being described in this record.
        /// </summary>
        public String  Version    { get; }

        /// <summary>
        /// The OCPI base URL for this platform for version 3.0.
        /// For earlier versions of OCPI, the URL to the endpoint containing version specific information.
        /// </summary>
        public URL     URL        { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new OCPI version.
        /// </summary>
        /// <param name="Version">The version number of the OCPI version being described in this record.</param>
        /// <param name="URL">The OCPI base URL for this platform for version 3.0. For earlier versions of OCPI, the URL to the endpoint containing version specific information.</param>
        public OCPIVersion(String  Version,
                           URL     URL)
        {

            this.Version  = Version;
            this.URL      = URL;

            unchecked
            {

                hashCode = this.Version.GetHashCode() * 3 ^
                           this.URL.    GetHashCode();

            }

        }

        #endregion


        #region (static) Parse   (JSON, ...)

        /// <summary>
        /// Parse the given JSON as an OCPI version.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        public static OCPIVersion Parse(JObject                                    JSON,
                                        CustomJObjectParserDelegate<OCPIVersion>?  CustomOCPIVersionParser   = null)
        {

            if (TryParse(JSON,
                         out var ocpiVersion,
                         out var errorResponse,
                         CustomOCPIVersionParser))
            {
                return ocpiVersion;
            }

            throw new ArgumentException("The given JSON representation of an OCPI version is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out OCPIVersion, out ErrorResponse, ...)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of an OCPI version.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="OCPIVersion">The parsed OCPI version.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                JSON,
                                       [NotNullWhen(true)]  out OCPIVersion?  OCPIVersion,
                                       [NotNullWhen(false)] out String?       ErrorResponse)

            => TryParse(JSON,
                        out OCPIVersion,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given text as an OCPI version.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="OCPIVersion">The parsed OCPI version.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomOCPIVersionParser">A delegate to parse custom OCPI version JSON objects.</param>
        public static Boolean TryParse(JObject                                    JSON,
                                       [NotNullWhen(true)]  out OCPIVersion?      OCPIVersion,
                                       [NotNullWhen(false)] out String?           ErrorResponse,
                                       CustomJObjectParserDelegate<OCPIVersion>?  CustomOCPIVersionParser)
        {

            try
            {

                OCPIVersion = null;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse Version    [mandatory]

                if (!JSON.ParseMandatoryText("version",
                                             "OCPI version",
                                             out String? Version,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse URL        [mandatory]

                if (!JSON.ParseMandatory("url",
                                         "platform URL",
                                         org.GraphDefined.Vanaheimr.Hermod.HTTP.URL.TryParse,
                                         out URL URL,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                OCPIVersion = new OCPIVersion(
                                  Version,
                                  URL
                              );


                if (CustomOCPIVersionParser is not null)
                    OCPIVersion = CustomOCPIVersionParser(JSON,
                                                          OCPIVersion);

                return true;

            }
            catch (Exception e)
            {
                OCPIVersion    = null;
                ErrorResponse  = "The given JSON representation of an OCPI version is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomOCPIVersionSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomOCPIVersionSerializer">A delegate to serialize custom OCPI Version JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<OCPIVersion>? CustomOCPIVersionSerializer = null)
        {

            var json = JSONObject.Create(
                           new JProperty("version",   Version),
                           new JProperty("url",       URL)
                       );

            return CustomOCPIVersionSerializer is not null
                       ? CustomOCPIVersionSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this OCPI version.
        /// </summary>
        public OCPIVersion Clone()

            => new (
                   Version.CloneString(),
                   URL.    Clone()
               );

        #endregion


        #region Operator overloading

        #region Operator == (OCPIVersion1, OCPIVersion2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OCPIVersion1">An OCPI version.</param>
        /// <param name="OCPIVersion2">Another OCPI version.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (OCPIVersion OCPIVersion1,
                                           OCPIVersion OCPIVersion2)

            => OCPIVersion1.Equals(OCPIVersion2);

        #endregion

        #region Operator != (OCPIVersion1, OCPIVersion2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OCPIVersion1">An OCPI version.</param>
        /// <param name="OCPIVersion2">Another OCPI version.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (OCPIVersion OCPIVersion1,
                                           OCPIVersion OCPIVersion2)

            => !OCPIVersion1.Equals(OCPIVersion2);

        #endregion

        #region Operator <  (OCPIVersion1, OCPIVersion2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OCPIVersion1">An OCPI version.</param>
        /// <param name="OCPIVersion2">Another OCPI version.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (OCPIVersion OCPIVersion1,
                                          OCPIVersion OCPIVersion2)

            => OCPIVersion1.CompareTo(OCPIVersion2) < 0;

        #endregion

        #region Operator <= (OCPIVersion1, OCPIVersion2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OCPIVersion1">An OCPI version.</param>
        /// <param name="OCPIVersion2">Another OCPI version.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (OCPIVersion OCPIVersion1,
                                           OCPIVersion OCPIVersion2)

            => OCPIVersion1.CompareTo(OCPIVersion2) <= 0;

        #endregion

        #region Operator >  (OCPIVersion1, OCPIVersion2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OCPIVersion1">An OCPI version.</param>
        /// <param name="OCPIVersion2">Another OCPI version.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (OCPIVersion OCPIVersion1,
                                          OCPIVersion OCPIVersion2)

            => OCPIVersion1.CompareTo(OCPIVersion2) > 0;

        #endregion

        #region Operator >= (OCPIVersion1, OCPIVersion2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OCPIVersion1">An OCPI version.</param>
        /// <param name="OCPIVersion2">Another OCPI version.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (OCPIVersion OCPIVersion1,
                                           OCPIVersion OCPIVersion2)

            => OCPIVersion1.CompareTo(OCPIVersion2) >= 0;

        #endregion

        #endregion

        #region IComparable<OCPIVersion> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two OCPI versions.
        /// </summary>
        /// <param name="Object">An OCPI version to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is OCPIVersion ocpiVersion
                   ? CompareTo(ocpiVersion)
                   : throw new ArgumentException("The given object is not an OCPI version!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(OCPIVersion)

        /// <summary>
        /// Compares two OCPI versions.
        /// </summary>
        /// <param name="OCPIVersion">An OCPI version to compare with.</param>
        public Int32 CompareTo(OCPIVersion? OCPIVersion)
        {

            if (OCPIVersion is null)
                throw new ArgumentNullException(nameof(OCPIVersion), "The given OCPI version must not be null!");

            var c = String.Compare(
                        Version,
                        OCPIVersion.Version,
                        StringComparison.OrdinalIgnoreCase
                    );

            if (c == 0)
                c = URL.CompareTo(OCPIVersion.URL);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<OCPIVersion> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two OCPI versions for equality.
        /// </summary>
        /// <param name="Object">An OCPI version to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is OCPIVersion ocpiVersion &&
                   Equals(ocpiVersion);

        #endregion

        #region Equals(OCPIVersion)

        /// <summary>
        /// Compares two OCPI versions for equality.
        /// </summary>
        /// <param name="OCPIVersion">An OCPI version to compare with.</param>
        public Boolean Equals(OCPIVersion? OCPIVersion)

            => OCPIVersion is not null &&

               Version.Equals(OCPIVersion.Version, StringComparison.OrdinalIgnoreCase) &&
               URL.    Equals(OCPIVersion.URL);

        #endregion

        #endregion

        #region (override) GetHashCode()

        private readonly Int32 hashCode;

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()
            => hashCode;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"{Version} @ {URL}";

        #endregion

    }

}
