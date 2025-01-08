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

namespace cloud.charging.open.protocols.OCPI
{

    /// <summary>
    /// Extension methods for software licenses.
    /// </summary>
    public static class SoftwareLicenseExtensions
    {

        #region ToJSON(this SoftwareLicense)

        /// <summary>
        /// Return a JSON representation for the given enumeration of software licenses.
        /// </summary>
        /// <param name="SoftwareLicense">An enumeration of software licenses.</param>
        /// <param name="Skip">The optional number of software licenses to skip.</param>
        /// <param name="Take">The optional number of software licenses to return.</param>
        /// <param name="CustomSourceLicenseSerializer">A delegate to serialize custom data license JSON elements.</param>
        public static JArray ToJSON(this IEnumerable<SoftwareLicense>                  SoftwareLicense,
                                    UInt64?                                              Skip                            = null,
                                    UInt64?                                              Take                            = null,
                                    CustomJObjectSerializerDelegate<SoftwareLicense>?  CustomSourceLicenseSerializer   = null)

            => SoftwareLicense is null || !SoftwareLicense.Any()

                   ? []

                   : new JArray(SoftwareLicense.
                                    Where         (openSourceLicense => openSourceLicense is not null).
                                    OrderBy       (openSourceLicense => openSourceLicense.Id).
                                    SkipTakeFilter(Skip, Take).
                                    Select        (openSourceLicense => openSourceLicense.ToJSON(CustomSourceLicenseSerializer)));

        #endregion

    }


    /// <summary>
    /// A software license.
    /// </summary>
    public class SoftwareLicense : IEquatable<SoftwareLicense>,
                                   IComparable<SoftwareLicense>,
                                   IComparable
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of the object.
        /// </summary>
        public const String JSONLDContext = "https://open.charging.cloud/contexts/OCPI/softwareLicense";

        #endregion

        #region Properties

        /// <summary>
        /// The unique identification of the software license.
        /// </summary>
        public SoftwareLicense_Id  Id             { get; }

        /// <summary>
        /// The description of the software license.
        /// </summary>
        public DisplayTexts        Description    { get; }

        /// <summary>
        /// Optional URLs for more information on the software license.
        /// </summary>
        public IEnumerable<URL>    URLs           { get; }

        #endregion

        #region Constructor(s)

        #region SoftwareLicense(Id,              params URLs)

        /// <summary>
        /// Create a new software license.
        /// </summary>
        /// <param name="Id">The unique identification of the software license.</param>
        /// <param name="URLs">Optional URLs for more information on the software license.</param>
        public SoftwareLicense(SoftwareLicense_Id       Id,
                               params IEnumerable<URL>  URLs)
        {

            this.Id           = Id;
            this.Description  = DisplayTexts.Empty;
            this.URLs         = URLs?.Distinct().ToArray() ?? [];

        }

        #endregion

        #region SoftwareLicense(Id, Description, params URLs)

        /// <summary>
        /// Create a new software license.
        /// </summary>
        /// <param name="Id">The unique identification of the software license.</param>
        /// <param name="Description">The description of the software license.</param>
        /// <param name="URLs">Optional URLs for more information on the software license.</param>
        public SoftwareLicense(SoftwareLicense_Id       Id,
                               DisplayTexts             Description,
                               params IEnumerable<URL>  URLs)
        {

            this.Id           = Id;
            this.Description  = Description;
            this.URLs         = URLs?.Distinct().ToArray() ?? [];

        }

        #endregion

        #endregion


        #region (static) Parse   (JSON, CustomSoftwareLicenseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a software license.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomSoftwareLicenseParser">A delegate to parse custom software license JSON objects.</param>
        public static SoftwareLicense Parse(JObject                                        JSON,
                                            CustomJObjectParserDelegate<SoftwareLicense>?  CustomSoftwareLicenseParser   = null)
        {

            if (TryParse(JSON,
                         out var openSourceLicense,
                         out var errorResponse,
                         CustomSoftwareLicenseParser))
            {
                return openSourceLicense;
            }

            throw new ArgumentException("The given JSON representation of a software license is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out SoftwareLicense, out ErrorResponse, CustomSoftwareLicenseParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a software license.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="SoftwareLicense">The parsed software license.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                    JSON,
                                       [NotNullWhen(true)]  out SoftwareLicense?  SoftwareLicense,
                                       [NotNullWhen(false)] out String?           ErrorResponse)

            => TryParse(JSON,
                        out SoftwareLicense,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a software license.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="SoftwareLicense">The parsed software license.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomSoftwareLicenseParser">A delegate to parse custom software license JSON objects.</param>
        public static Boolean TryParse(JObject                                        JSON,
                                       [NotNullWhen(true)]  out SoftwareLicense?      SoftwareLicense,
                                       [NotNullWhen(false)] out String?               ErrorResponse,
                                       CustomJObjectParserDelegate<SoftwareLicense>?  CustomSoftwareLicenseParser   = null)
        {

            try
            {

                SoftwareLicense = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse Id             [mandatory]

                if (!JSON.ParseMandatory("id",
                                         "software license identification",
                                         SoftwareLicense_Id.TryParse,
                                         out SoftwareLicense_Id Id,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Description    [optional]

                if (JSON.ParseOptionalJSONArray("description",
                                                "software license description",
                                                DisplayTexts.TryParse,
                                                out DisplayTexts Description,
                                                out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse URLs           [optional]

                if (JSON.ParseOptionalHashSet("urls",
                                              "software license URLs",
                                              URL.TryParse,
                                              out HashSet<URL> URLs,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                SoftwareLicense = new SoftwareLicense(
                                      Id,
                                      Description,
                                      [.. URLs]
                                  );

                if (CustomSoftwareLicenseParser is not null)
                    SoftwareLicense = CustomSoftwareLicenseParser(JSON,
                                                                  SoftwareLicense);

                return true;

            }
            catch (Exception e)
            {
                SoftwareLicense  = default;
                ErrorResponse    = "The given JSON representation of a software license is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSourceLicenseSerializer = null, CustomDisplayTextsSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of the given data license.
        /// </summary>
        /// <param name="CustomSourceLicenseSerializer">A delegate to serialize custom data license JSON elements.</param>
        /// <param name="CustomDisplayTextsSerializer">A delegate to serialize custom display texts JSON objects.</param>
        /// <param name="CustomDisplayTextSerializer">A delegate to serialize custom platform party JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SoftwareLicense>?  CustomSourceLicenseSerializer   = null,
                              CustomJArraySerializerDelegate<DisplayTexts>?      CustomDisplayTextsSerializer    = null,
                              CustomJObjectSerializerDelegate<DisplayText>?      CustomDisplayTextSerializer     = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("id",            Id.         ToString()),

                           Description.Any()
                               ? new JProperty("description",   Description.ToJSON(CustomDisplayTextsSerializer,
                                                                                   CustomDisplayTextSerializer))
                               : null,

                                 new JProperty("urls",          new JArray(URLs.Select(url => url.ToString())))

                       );

            return CustomSourceLicenseSerializer is not null
                       ? CustomSourceLicenseSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this software license.
        /// </summary>
        public SoftwareLicense Clone()

            => new (
                   Id.         Clone(),
                   Description.Clone(),
                   URLs.       Select(url => url.Clone())
               );

        #endregion


        #region Static Definitions

        /// <summary>
        /// No license.
        /// </summary>
        public static readonly SoftwareLicense None               = new (SoftwareLicense_Id.Parse("None"),
                                                                           DisplayTexts.Create(Languages.en, "None - do not use!"));

        /// <summary>
        /// The software etc.pp is not software.
        /// </summary>
        public static readonly SoftwareLicense ClosedSource       = new (SoftwareLicense_Id.Parse("ClosedSource"),
                                                                           DisplayTexts.Create(Languages.en, "Closed Source - do not use!"));


        /// <summary>
        /// Apache License v2.0
        /// </summary>
        public static readonly SoftwareLicense Apache2_0          = new (SoftwareLicense_Id.Parse("Apache-2.0"),
                                                                           DisplayTexts.Create(Languages.en, "Apache License v2.0"),
                                                                           URL.Parse("https://www.apache.org/licenses/LICENSE-2.0.txt"),
                                                                           URL.Parse("https://www.apache.org/licenses/LICENSE-2.0"),
                                                                           URL.Parse("https://opensource.org/license/apache-2-0/"));

        /// <summary>
        /// The MIT License
        /// </summary>
        public static readonly SoftwareLicense MIT                = new (SoftwareLicense_Id.Parse("MIT"),
                                                                           DisplayTexts.Create(Languages.en, "The MIT License"),
                                                                           URL.Parse("https://opensource.org/license/mit/"));

        /// <summary>
        /// The 2-Clause BSD License
        /// </summary>
        public static readonly SoftwareLicense BSD2               = new (SoftwareLicense_Id.Parse("BSD-2-Clause"),
                                                                           DisplayTexts.Create(Languages.en, "The 2-Clause BSD License"),
                                                                           URL.Parse("https://opensource.org/license/bsd-2-clause/"));

        /// <summary>
        /// The 3-Clause BSD License
        /// </summary>
        public static readonly SoftwareLicense BSD3               = new (SoftwareLicense_Id.Parse("BSD-3-Clause"),
                                                                           DisplayTexts.Create(Languages.en, "The 3-Clause BSD License"),
                                                                           URL.Parse("https://opensource.org/license/bsd-3-clause/"));


        /// <summary>
        /// GNU General Public License version 2
        /// </summary>
        public static readonly SoftwareLicense GPL2               = new (SoftwareLicense_Id.Parse("GPL-2.0"),
                                                                           DisplayTexts.Create(Languages.en, "GNU General Public License version 2"),
                                                                           URL.Parse("https://www.gnu.org/licenses/gpl-2.0.html"),
                                                                           URL.Parse("https://opensource.org/license/gpl-2-0/"));

        /// <summary>
        /// GNU General Public License version 3
        /// </summary>
        public static readonly SoftwareLicense GPL3               = new (SoftwareLicense_Id.Parse("GPL-3.0"),
                                                                           DisplayTexts.Create(Languages.en, "GNU General Public License version 3"),
                                                                           URL.Parse("https://www.gnu.org/licenses/gpl-3.0.html"),
                                                                           URL.Parse("https://opensource.org/license/gpl-3-0/"));

        /// <summary>
        /// GNU Affero General Public License version 3
        /// </summary>
        public static readonly SoftwareLicense AGPL3              = new (SoftwareLicense_Id.Parse("AGPL-3.0"),
                                                                           DisplayTexts.Create(Languages.en, "GNU Affero General Public License version 3"),
                                                                           URL.Parse("https://www.gnu.org/licenses/agpl-3.0.html"),
                                                                           URL.Parse("https://opensource.org/license/agpl-v3/"));

        /// <summary>
        /// GNU Lesser General Public License version 3
        /// </summary>
        public static readonly SoftwareLicense LGPL3              = new (SoftwareLicense_Id.Parse("LGPL-3.0"),
                                                                           DisplayTexts.Create(Languages.en, "GNU Lesser General Public License version 3"),
                                                                           URL.Parse("https://www.gnu.org/licenses/lgpl-3.0.html"),
                                                                           URL.Parse("https://opensource.org/license/lgpl-v3/"));


        /// <summary>
        /// European Union Public License, version 1.2 (EUPL-1.2)
        /// </summary>
        public static readonly SoftwareLicense EUPLv1_2           = new (SoftwareLicense_Id.Parse("EUPL-1.2"),
                                                                           DisplayTexts.Create(Languages.en, "European Union Public License, version 1.2 (EUPL-1.2)"),
                                                                           URL.Parse("https://joinup.ec.europa.eu/collection/eupl/news/understanding-eupl-v12"),
                                                                           URL.Parse("https://opensource.org/license/eupl-1-2/"));

        #endregion


        #region Operator overloading

        #region Operator == (SoftwareLicense1, SoftwareLicense2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SoftwareLicense1">A software license.</param>
        /// <param name="SoftwareLicense2">Another software license.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (SoftwareLicense SoftwareLicense1,
                                           SoftwareLicense SoftwareLicense2)

            => SoftwareLicense1.Equals(SoftwareLicense2);

        #endregion

        #region Operator != (SoftwareLicense1, SoftwareLicense2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SoftwareLicense1">A software license.</param>
        /// <param name="SoftwareLicense2">Another software license.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (SoftwareLicense SoftwareLicense1,
                                           SoftwareLicense SoftwareLicense2)

            => !(SoftwareLicense1 == SoftwareLicense2);

        #endregion

        #region Operator <  (SoftwareLicense1, SoftwareLicense2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SoftwareLicense1">A software license.</param>
        /// <param name="SoftwareLicense2">Another software license.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (SoftwareLicense SoftwareLicense1,
                                          SoftwareLicense SoftwareLicense2)

            => SoftwareLicense1.CompareTo(SoftwareLicense2) < 0;

        #endregion

        #region Operator <= (SoftwareLicense1, SoftwareLicense2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SoftwareLicense1">A software license.</param>
        /// <param name="SoftwareLicense2">Another software license.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (SoftwareLicense SoftwareLicense1,
                                           SoftwareLicense SoftwareLicense2)

            => SoftwareLicense1.CompareTo(SoftwareLicense2) <= 0;

        #endregion

        #region Operator >  (SoftwareLicense1, SoftwareLicense2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SoftwareLicense1">A software license.</param>
        /// <param name="SoftwareLicense2">Another software license.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (SoftwareLicense SoftwareLicense1,
                                          SoftwareLicense SoftwareLicense2)

            => SoftwareLicense1.CompareTo(SoftwareLicense2) > 0;

        #endregion

        #region Operator >= (SoftwareLicense1, SoftwareLicense2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SoftwareLicense1">A software license.</param>
        /// <param name="SoftwareLicense2">Another software license.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (SoftwareLicense SoftwareLicense1,
                                           SoftwareLicense SoftwareLicense2)

            => SoftwareLicense1.CompareTo(SoftwareLicense2) >= 0;

        #endregion

        #endregion

        #region IComparable<SoftwareLicense> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two software licenses.
        /// </summary>
        /// <param name="Object">A software license to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is SoftwareLicense openSourceLicense
                   ? CompareTo(openSourceLicense)
                   : throw new ArgumentException("The given object is not a software license!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(SoftwareLicense)

        /// <summary>
        /// Compares two software licenses.
        /// </summary>
        /// <param name="SoftwareLicense">A software license to compare with.</param>
        public Int32 CompareTo(SoftwareLicense? SoftwareLicense)

            => SoftwareLicense is not null
                   ? Id.CompareTo(SoftwareLicense.Id)
                   : throw new ArgumentNullException(nameof(SoftwareLicense),
                                                     "The given software license must not be null!");

        #endregion

        #endregion

        #region IEquatable<SoftwareLicense> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two software licenses for equality.
        /// </summary>
        /// <param name="Object">A software license to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SoftwareLicense openSourceLicense &&
                   Equals(openSourceLicense);

        #endregion

        #region Equals(SoftwareLicense)

        /// <summary>
        /// Compares two software licenses for equality.
        /// </summary>
        /// <param name="SoftwareLicense">A software license to compare with.</param>
        public Boolean Equals(SoftwareLicense? SoftwareLicense)

            => SoftwareLicense is not null &&

               Id.Equals(SoftwareLicense.Id) &&

               Description.Count().Equals(SoftwareLicense.Description.Count()) &&
               Description.All(displayText => SoftwareLicense.Description.Contains(displayText)) &&

               URLs.       Count().Equals(SoftwareLicense.URLs.       Count()) &&
               URLs.       All(url         => SoftwareLicense.URLs.       Contains(url));

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

                   Id.ToString(),

                   Description.Any()
                       ? ": " + Description.Select(displayText => $"[{displayText.Language}] {displayText.Text.SubstringMax(20)}").AggregateWith(", ")
                       : "",

                   URLs.Any()
                       ? ": " + URLs.       Select(url         => url.ToString().SubstringMax(15)).AggregateWith(", ")
                       : ""

               );

        #endregion

    }

}
