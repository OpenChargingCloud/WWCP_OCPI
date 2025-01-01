/*
 * Copyright (c) 2015-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPI <https://github.com/OpenChargingCloud/WWCP_OCPI>
 *
 * Licensed under the Apache License, Auth 2.0 (the "License");
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

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv3_0
{

    /// <summary>
    /// An enumeration of OCPI Versions.
    /// </summary>
    public class OCPIVersions
    {

        #region Properties

        /// <summary>
        /// The enumeration of all OCPI versions supported by the platform.
        /// </summary>
        public IEnumerable<OCPIVersion>  Versions    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new enumeration of OCPI Versions.
        /// </summary>
        /// <param name="Versions">An enumeration of all OCPI versions supported by the platform.</param>
        public OCPIVersions(IEnumerable<OCPIVersion> Versions)
        {

            if (!Versions.Any())
                throw new ArgumentNullException(nameof(Versions), "The given enumeration of OCPI versions must not be empty!");

            this.Versions = Versions;

            unchecked
            {
                hashCode = this.Versions.CalcHashCode();
            }

        }

        #endregion


        #region (static) Parse   (JSON, ...)

        /// <summary>
        /// Parse the given JSON as an enumeration of OCPI versions.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        public static OCPIVersions Parse(JObject                                                  JSON,
                                         CustomJObjectParserDelegate<OCPIVersions>?  CustomOCPIVersionsParser   = null)
        {

            if (TryParse(JSON,
                         out var ocpiVersions,
                         out var errorResponse,
                         CustomOCPIVersionsParser))
            {
                return ocpiVersions;
            }

            throw new ArgumentException("The given JSON representation of OCPI versions is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out OCPIVersions, out ErrorResponse, ...)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of OCPI versions.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="OCPIVersions">The parsed OCPI versions.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                 JSON,
                                       [NotNullWhen(true)]  out OCPIVersions?  OCPIVersions,
                                       [NotNullWhen(false)] out String?        ErrorResponse)

            => TryParse(JSON,
                        out OCPIVersions,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given text as an OCPI versions object.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="OCPIVersions">The parsed OCPI versions.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomOCPIVersionsParser">A delegate to parse custom OCPI versions JSON objects.</param>
        public static Boolean TryParse(JObject                                     JSON,
                                       [NotNullWhen(true)]  out OCPIVersions?      OCPIVersions,
                                       [NotNullWhen(false)] out String?            ErrorResponse,
                                       CustomJObjectParserDelegate<OCPIVersions>?  CustomOCPIVersionsParser)
        {

            try
            {

                OCPIVersions = null;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse Versions    [mandatory]

                if (!JSON.ParseMandatoryHashSet("versions",
                                                "OCPI versions",
                                                OCPIVersion.TryParse,
                                                out HashSet<OCPIVersion> Versions,
                                                out ErrorResponse))
                {
                    return false;
                }

                #endregion


                OCPIVersions = new OCPIVersions(
                                   Versions
                               );


                if (CustomOCPIVersionsParser is not null)
                    OCPIVersions = CustomOCPIVersionsParser(JSON,
                                                            OCPIVersions);

                return true;

            }
            catch (Exception e)
            {
                OCPIVersions   = null;
                ErrorResponse  = "The given JSON representation of OCPI versions is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomOCPIVersionsSerializer = null, CustomOCPIVersionSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomOCPIVersionsSerializer">A delegate to serialize custom OCPI Version JSON objects.</param>
        /// <param name="CustomOCPIVersionSerializer">A delegate to serialize custom OCPI Version JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<OCPIVersions>?  CustomOCPIVersionsSerializer   = null,
                              CustomJObjectSerializerDelegate<OCPIVersion>?   CustomOCPIVersionSerializer    = null)
        {

            var json = JSONObject.Create(
                           new JProperty("versions",  new JArray(Versions.Select(version => version.ToJSON(CustomOCPIVersionSerializer))))
                       );

            return CustomOCPIVersionsSerializer is not null
                       ? CustomOCPIVersionsSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this OCPI versions.
        /// </summary>
        public OCPIVersions Clone()

            => new (
                   Versions.Select(version => version.Clone())
               );

        #endregion


        #region Operator overloading

        #region Operator == (OCPIVersions1, OCPIVersions2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OCPIVersions1">An OCPI versions object.</param>
        /// <param name="OCPIVersions2">Another OCPI versions object.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (OCPIVersions OCPIVersions1,
                                           OCPIVersions OCPIVersions2)

            => OCPIVersions1.Equals(OCPIVersions2);

        #endregion

        #region Operator != (OCPIVersions1, OCPIVersions2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OCPIVersions1">An OCPI versions object.</param>
        /// <param name="OCPIVersions2">Another OCPI versions object.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (OCPIVersions OCPIVersions1,
                                           OCPIVersions OCPIVersions2)

            => !OCPIVersions1.Equals(OCPIVersions2);

        #endregion

        #region Operator <  (OCPIVersions1, OCPIVersions2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OCPIVersions1">An OCPI versions object.</param>
        /// <param name="OCPIVersions2">Another OCPI versions object.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (OCPIVersions OCPIVersions1,
                                          OCPIVersions OCPIVersions2)

            => OCPIVersions1.CompareTo(OCPIVersions2) < 0;

        #endregion

        #region Operator <= (OCPIVersions1, OCPIVersions2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OCPIVersions1">An OCPI versions object.</param>
        /// <param name="OCPIVersions2">Another OCPI versions object.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (OCPIVersions OCPIVersions1,
                                           OCPIVersions OCPIVersions2)

            => OCPIVersions1.CompareTo(OCPIVersions2) <= 0;

        #endregion

        #region Operator >  (OCPIVersions1, OCPIVersions2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OCPIVersions1">An OCPI versions object.</param>
        /// <param name="OCPIVersions2">Another OCPI versions object.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (OCPIVersions OCPIVersions1,
                                          OCPIVersions OCPIVersions2)

            => OCPIVersions1.CompareTo(OCPIVersions2) > 0;

        #endregion

        #region Operator >= (OCPIVersions1, OCPIVersions2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OCPIVersions1">An OCPI versions object.</param>
        /// <param name="OCPIVersions2">Another OCPI versions object.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (OCPIVersions OCPIVersions1,
                                           OCPIVersions OCPIVersions2)

            => OCPIVersions1.CompareTo(OCPIVersions2) >= 0;

        #endregion

        #endregion

        #region IComparable<OCPIVersions> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two OCPI versionss.
        /// </summary>
        /// <param name="Object">An OCPI versions object to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is OCPIVersions ocpiVersions
                   ? CompareTo(ocpiVersions)
                   : throw new ArgumentException("The given object is not an OCPI versions object!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(OCPIVersions)

        /// <summary>
        /// Compares two OCPI versionss.
        /// </summary>
        /// <param name="OCPIVersions">An OCPI versions object to compare with.</param>
        public Int32 CompareTo(OCPIVersions? OCPIVersions)
        {

            if (OCPIVersions is null)
                throw new ArgumentNullException(nameof(OCPIVersions), "The given OCPI versions must not be null!");

            var thisVersions   =              Versions.OrderBy(v => v.Version).ToArray();
            var otherVersions  = OCPIVersions.Versions.OrderBy(v => v.Version).ToArray();

            for (var i = 0; i < Math.Min(thisVersions.Length, otherVersions.Length); i++)
            {
                var comparison = thisVersions[i].CompareTo(otherVersions[i]);
                if (comparison != 0)
                    return comparison;
            }

            return thisVersions.Length.CompareTo(otherVersions.Length);

        }

        #endregion

        #endregion

        #region IEquatable<OCPIVersions> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two OCPI versionss for equality.
        /// </summary>
        /// <param name="Object">An OCPI versions object to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is OCPIVersions ocpiVersions &&
                   Equals(ocpiVersions);

        #endregion

        #region Equals(OCPIVersions)

        /// <summary>
        /// Compares two OCPI versionss for equality.
        /// </summary>
        /// <param name="OCPIVersions">An OCPI versions object to compare with.</param>
        public Boolean Equals(OCPIVersions? OCPIVersions)
        {

            if (OCPIVersions is null)
                return false;

            var thisVersions   =              Versions.OrderBy(v => v.Version).ToArray();
            var otherVersions  = OCPIVersions.Versions.OrderBy(v => v.Version).ToArray();

            if (thisVersions.Length != otherVersions.Length)
                return false;

            for (var i = 0; i < thisVersions.Length; i++)
            {
                if (!thisVersions[i].Equals(otherVersions[i]))
                    return false;
            }

            return true;

        }

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

            => $"{Versions.Count()} versions";

        #endregion

    }

}
