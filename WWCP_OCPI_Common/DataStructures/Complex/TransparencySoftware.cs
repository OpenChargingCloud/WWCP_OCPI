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

namespace cloud.charging.open.protocols.OCPI
{

    /// <summary>
    /// The charging transparency software.
    /// This information will e.g. be used for the German calibration law.
    /// </summary>
    [VendorExtension(VE.GraphDefined, VE.Eichrecht)]
    public class TransparencySoftware : IEquatable<TransparencySoftware>,
                                        IComparable<TransparencySoftware>,
                                        IComparable
    {

        #region Properties

        /// <summary>
        /// The multi-language name of the transparency software.
        /// </summary>
        [Mandatory]
        public IEnumerable<DisplayText>      Name                     { get; }

        /// <summary>
        /// The version of the transparency software.
        /// </summary>
        [Mandatory]
        public String                        Version                  { get; }

        /// <summary>
        /// The Open Source licenses of the transparency software, at least one.
        /// </summary>
        [Mandatory]
        public IEnumerable<SoftwareLicense>  OpenSourceLicenses       { get; }

        /// <summary>
        /// The vendor of the transparency software.
        /// </summary>
        [Mandatory]
        public String                        Vendor                   { get; }

        /// <summary>
        /// The optional URL where to find a small logo of the transparency software.
        /// </summary>
        [Optional]
        public URL?                          Logo                     { get; }

        /// <summary>
        /// The optional URL where to find a manual how to use the transparency software.
        /// </summary>
        [Optional]
        public URL?                          HowToUse                 { get; }

        /// <summary>
        /// The optional URL where to find more information about the transparency software.
        /// </summary>
        [Optional]
        public URL?                          MoreInformation          { get; }

        /// <summary>
        /// The optional URL where to find the source code of the transparency software.
        /// </summary>
        [Optional]
        public URL?                          SourceCodeRepository     { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new charging transparency software.
        /// </summary>
        /// <param name="Name">The multi-language name of the transparency software.</param>
        /// <param name="Version">The version of the transparency software.</param>
        /// <param name="OpenSourceLicenses">The Open Source licenses of the transparency software, at least one.</param>
        /// <param name="Vendor">The vendor of the transparency software.</param>
        /// 
        /// <param name="Logo">An optional URL where to find a small logo of the transparency software.</param>
        /// <param name="HowToUse">An optional URL where to find a manual how to use the transparency software.</param>
        /// <param name="MoreInformation">An optional URL where to find more information about the transparency software.</param>
        /// <param name="SourceCodeRepository">An optional URL where to find the source code of the transparency software.</param>
        public TransparencySoftware(IEnumerable<DisplayText>      Name,
                                    String                        Version,
                                    IEnumerable<SoftwareLicense>  OpenSourceLicenses,
                                    String                        Vendor,

                                    URL?                          Logo                   = null,
                                    URL?                          HowToUse               = null,
                                    URL?                          MoreInformation        = null,
                                    URL?                          SourceCodeRepository   = null)
        {

            var names     = (Name               ?? []).Distinct().Order().ToArray();
            var licenses  = (OpenSourceLicenses ?? []).Distinct().Order().ToArray();

            if (names.Length == 0 || names.Any(name => String.IsNullOrWhiteSpace(name.Text)))
                throw new ArgumentException("A name with a nonempty text in each language is required!", nameof(Name));

            if (names.Select(name => name.Language).Distinct().Count() != names.Length)
                throw new ArgumentException("The name must have only one text per language!", nameof(Name));

            if (licenses.Length == 0)
                throw new ArgumentException("At least one Open Source license is required!", nameof(OpenSourceLicenses));

            if (licenses.Select(license => license.Id).Distinct().Count() != licenses.Length)
                throw new ArgumentException("An Open Source license must not be given twice!", nameof(OpenSourceLicenses));

            this.Name                  = names;
            this.Version               = Version;
            this.OpenSourceLicenses    = licenses;
            this.Vendor                = Vendor;

            this.Logo                  = Logo;
            this.HowToUse              = HowToUse;
            this.MoreInformation       = MoreInformation;
            this.SourceCodeRepository  = SourceCodeRepository;

        }

        #endregion


        #region (static) Parse   (JSON, CustomTransparencySoftwareParser = null)

        /// <summary>
        /// Parse the given JSON representation of a transparency software.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomTransparencySoftwareParser">A delegate to parse custom transparency software JSON objects.</param>
        public static TransparencySoftware Parse(JObject                                             JSON,
                                                 CustomJObjectParserDelegate<TransparencySoftware>?  CustomTransparencySoftwareParser   = null)
        {

            if (TryParse(JSON,
                         out var transparencySoftware,
                         out var errorResponse,
                         CustomTransparencySoftwareParser))
            {
                return transparencySoftware;
            }

            throw new ArgumentException("The given JSON representation of a transparency software is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out TransparencySoftware, out ErrorResponse, CustomTransparencySoftwareParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a transparency software.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="TransparencySoftware">The parsed transparency software.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                         JSON,
                                       [NotNullWhen(true)]  out TransparencySoftware?  TransparencySoftware,
                                       [NotNullWhen(false)] out String?                ErrorResponse)

            => TryParse(JSON,
                        out TransparencySoftware,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a transparency software.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="TransparencySoftware">The parsed transparency software.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomTransparencySoftwareParser">A delegate to parse custom transparency software JSON objects.</param>
        public static Boolean TryParse(JObject                                             JSON,
                                       [NotNullWhen(true)]  out TransparencySoftware?      TransparencySoftware,
                                       [NotNullWhen(false)] out String?                    ErrorResponse,
                                       CustomJObjectParserDelegate<TransparencySoftware>?  CustomTransparencySoftwareParser   = null)
        {

            try
            {

                TransparencySoftware = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse Name                      [mandatory]

                if (!JSON.ParseMandatoryHashSet("name",
                                                "name",
                                                DisplayText.TryParse,
                                                out HashSet<DisplayText> Name,
                                                out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Version                   [mandatory]

                if (!JSON.ParseMandatoryText("version",
                                             "version",
                                             out String? Version,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse OpenSourceLicenses        [mandatory]

                if (!JSON.ParseMandatoryHashSet("open_source_licenses",
                                                "Open Source licenses",
                                                OCPI.SoftwareLicense.TryParse,
                                                out HashSet<SoftwareLicense> OpenSourceLicenses,
                                                out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Vendor                    [mandatory]

                if (!JSON.ParseMandatoryText("vendor",
                                             "vendor",
                                             out String Vendor,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Logo                      [optional]

                if (JSON.ParseOptionalURL("logo",
                                          "logo",
                                          out URL? Logo,
                                          out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse HowToUse                  [optional]

                if (JSON.ParseOptionalURL("how_to_use",
                                          "how to use",
                                          out URL? HowToUse,
                                          out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse MoreInformation           [optional]

                if (JSON.ParseOptionalURL("more_information",
                                          "more information",
                                          out URL? MoreInformation,
                                          out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse SourceCodeRepository      [optional]

                if (JSON.ParseOptionalURL("source_code_repository",
                                          "source code repository",
                                          out URL? SourceCodeRepository,
                                          out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                TransparencySoftware = new TransparencySoftware(
                                           Name,
                                           Version,
                                           OpenSourceLicenses,
                                           Vendor,
                                           Logo,
                                           HowToUse,
                                           MoreInformation,
                                           SourceCodeRepository
                                       );

                if (CustomTransparencySoftwareParser is not null)
                    TransparencySoftware = CustomTransparencySoftwareParser(JSON,
                                                                            TransparencySoftware);

                return true;

            }
            catch (Exception e)
            {
                TransparencySoftware  = default;
                ErrorResponse         = "The given JSON representation of a transparency software is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomTransparencySoftwareSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomTransparencySoftwareSerializer">A delegate to serialize custom transparency software JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<TransparencySoftware>? CustomTransparencySoftwareSerializer = null)
        {

            var JSON = JSONObject.Create(

                                 new JProperty("name",                    new JArray(Name.              Select(name    => name.   ToJSON()))),
                                 new JProperty("version",                 Version),
                                 new JProperty("open_source_licenses",    new JArray(OpenSourceLicenses.Select(license => license.ToJSON()))),
                                 new JProperty("vendor",                  Vendor),

                           Logo.                HasValue
                               ? new JProperty("logo",                    Logo.                ToString())
                               : null,

                           HowToUse.            HasValue
                               ? new JProperty("how_to_use",              HowToUse.            ToString())
                               : null,

                           MoreInformation.     HasValue
                               ? new JProperty("more_information",        MoreInformation.     ToString())
                               : null,

                           SourceCodeRepository.HasValue
                               ? new JProperty("source_code_repository",  SourceCodeRepository.ToString())
                               : null

                       );

            return CustomTransparencySoftwareSerializer is not null
                       ? CustomTransparencySoftwareSerializer(this, JSON)
                       : JSON;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this object.
        /// </summary>
        public TransparencySoftware Clone()

            => new (
                   Name.                 Select(name    => name.   Clone()),
                   Version.              CloneString(),
                   OpenSourceLicenses.   Select(license => license.Clone()),
                   Vendor.               CloneString(),
                   Logo?.                Clone(),
                   HowToUse?.            Clone(),
                   MoreInformation?.     Clone(),
                   SourceCodeRepository?.Clone()
               );

        #endregion


        #region Operator overloading

        #region Operator == (TransparencySoftware1, TransparencySoftware2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TransparencySoftware1">A transparency software.</param>
        /// <param name="TransparencySoftware2">Another transparency software.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (TransparencySoftware TransparencySoftware1,
                                           TransparencySoftware TransparencySoftware2)
        {

            if (Object.ReferenceEquals(TransparencySoftware1, TransparencySoftware2))
                return true;

            if (TransparencySoftware1 is null || TransparencySoftware2 is null)
                return false;

            return TransparencySoftware1.Equals(TransparencySoftware2);

        }

        #endregion

        #region Operator != (TransparencySoftware1, TransparencySoftware2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TransparencySoftware1">A transparency software.</param>
        /// <param name="TransparencySoftware2">Another transparency software.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (TransparencySoftware TransparencySoftware1,
                                           TransparencySoftware TransparencySoftware2)

            => !(TransparencySoftware1 == TransparencySoftware2);

        #endregion

        #region Operator <  (TransparencySoftware1, TransparencySoftware2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TransparencySoftware1">A transparency software.</param>
        /// <param name="TransparencySoftware2">Another transparency software.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (TransparencySoftware TransparencySoftware1,
                                          TransparencySoftware TransparencySoftware2)

            => TransparencySoftware1 is null
                   ? throw new ArgumentNullException(nameof(TransparencySoftware1), "The give transparency software must not be null!")
                   : TransparencySoftware1.CompareTo(TransparencySoftware2) < 0;

        #endregion

        #region Operator <= (TransparencySoftware1, TransparencySoftware2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TransparencySoftware1">A transparency software.</param>
        /// <param name="TransparencySoftware2">Another transparency software.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (TransparencySoftware TransparencySoftware1,
                                           TransparencySoftware TransparencySoftware2)

            => !(TransparencySoftware1 > TransparencySoftware2);

        #endregion

        #region Operator >  (TransparencySoftware1, TransparencySoftware2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TransparencySoftware1">A transparency software.</param>
        /// <param name="TransparencySoftware2">Another transparency software.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (TransparencySoftware TransparencySoftware1,
                                          TransparencySoftware TransparencySoftware2)

            => TransparencySoftware1 is null
                   ? throw new ArgumentNullException(nameof(TransparencySoftware1), "The give transparency software must not be null!")
                   : TransparencySoftware1.CompareTo(TransparencySoftware2) > 0;

        #endregion

        #region Operator >= (TransparencySoftware1, TransparencySoftware2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TransparencySoftware1">A transparency software.</param>
        /// <param name="TransparencySoftware2">Another transparency software.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (TransparencySoftware TransparencySoftware1,
                                           TransparencySoftware TransparencySoftware2)

            => !(TransparencySoftware1 < TransparencySoftware2);

        #endregion

        #endregion

        #region IComparable<TransparencySoftware> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two transparency software.
        /// </summary>
        /// <param name="Object">A transparency software to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is TransparencySoftware transparencySoftware
                   ? CompareTo(transparencySoftware)
                   : throw new ArgumentException("The given object is not a transparency software!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(TransparencySoftware)

        /// <summary>
        /// Compares two transparency software.
        /// </summary>
        /// <param name="TransparencySoftware">A transparency software to compare with.</param>
        public Int32 CompareTo(TransparencySoftware? TransparencySoftware)
        {

            if (TransparencySoftware is null)
                throw new ArgumentNullException(nameof(TransparencySoftware), "The give transparency software must not be null!");

            var c = Compare(Name, TransparencySoftware.Name);

            if (c == 0)
                c = Version.          CompareTo(TransparencySoftware.Version);

            if (c == 0)
                c = Compare(OpenSourceLicenses, TransparencySoftware.OpenSourceLicenses);

            if (c == 0)
                c = Vendor.           CompareTo(TransparencySoftware.Vendor);

            if (c == 0 && Logo.                HasValue && TransparencySoftware.Logo.                HasValue)
                c = Logo.                Value.CompareTo(TransparencySoftware.Logo.                Value);

            if (c == 0 && HowToUse.            HasValue && TransparencySoftware.HowToUse.            HasValue)
                c = HowToUse.            Value.CompareTo(TransparencySoftware.HowToUse.            Value);

            if (c == 0 && MoreInformation.     HasValue && TransparencySoftware.MoreInformation.     HasValue)
                c = MoreInformation.     Value.CompareTo(TransparencySoftware.MoreInformation.     Value);

            if (c == 0 && SourceCodeRepository.HasValue && TransparencySoftware.SourceCodeRepository.HasValue)
                c = SourceCodeRepository.Value.CompareTo(TransparencySoftware.SourceCodeRepository.Value);

            return c;

        }

        #endregion

        #region (private static) Compare(First, Second)

        /// <summary>
        /// Compares two sorted sequences element by element, then by their length.
        /// </summary>
        private static Int32 Compare<T>(IEnumerable<T> First, IEnumerable<T> Second)
            where T : IComparable<T>
        {

            using var first  = First. GetEnumerator();
            using var second = Second.GetEnumerator();

            while (true)
            {

                var hasFirst   = first. MoveNext();
                var hasSecond  = second.MoveNext();

                if (!hasFirst || !hasSecond)
                    return hasFirst.CompareTo(hasSecond);

                var c = first.Current.CompareTo(second.Current);
                if (c != 0)
                    return c;

            }

        }

        #endregion

        #endregion

        #region IEquatable<TransparencySoftware> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two transparency software for equality.
        /// </summary>
        /// <param name="Object">A transparency software to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is TransparencySoftware transparencySoftware &&
                   Equals(transparencySoftware);

        #endregion

        #region Equals(TransparencySoftware)

        /// <summary>
        /// Compares two transparency software for equality.
        /// </summary>
        /// <param name="TransparencySoftware">A transparency software to compare with.</param>
        public Boolean Equals(TransparencySoftware? TransparencySoftware)

            => TransparencySoftware is not null &&

               Name.              SequenceEqual(TransparencySoftware.Name)               &&
               Version.           Equals       (TransparencySoftware.Version)            &&
               OpenSourceLicenses.SequenceEqual(TransparencySoftware.OpenSourceLicenses) &&
               Vendor.           Equals(TransparencySoftware.Vendor)            &&

            ((!Logo.                HasValue && !TransparencySoftware.Logo.                HasValue) ||
              (Logo.                HasValue &&  TransparencySoftware.Logo.                HasValue && Logo.                Value.Equals(TransparencySoftware.Logo.                Value))) &&

            ((!HowToUse.            HasValue && !TransparencySoftware.HowToUse.            HasValue) ||
              (HowToUse.            HasValue &&  TransparencySoftware.HowToUse.            HasValue && HowToUse.            Value.Equals(TransparencySoftware.HowToUse.            Value))) &&

            ((!MoreInformation.     HasValue && !TransparencySoftware.MoreInformation.     HasValue) ||
              (MoreInformation.     HasValue &&  TransparencySoftware.MoreInformation.     HasValue && MoreInformation.     Value.Equals(TransparencySoftware.MoreInformation.     Value))) &&

            ((!SourceCodeRepository.HasValue && !TransparencySoftware.SourceCodeRepository.HasValue) ||
              (SourceCodeRepository.HasValue &&  TransparencySoftware.SourceCodeRepository.HasValue && SourceCodeRepository.Value.Equals(TransparencySoftware.SourceCodeRepository.Value)));

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

                return Name.              Aggregate(0, (hash, name)    => hash * 31 ^ name.   GetHashCode()) * 23 ^
                       Version.              GetHashCode()       * 19 ^
                       OpenSourceLicenses.Aggregate(0, (hash, license) => hash * 31 ^ license.GetHashCode()) * 13 ^
                       Vendor.               GetHashCode()       * 11 ^
                      (Logo?.                GetHashCode() ?? 0) * 7 ^
                      (HowToUse?.            GetHashCode() ?? 0) * 5 ^
                      (MoreInformation?.     GetHashCode() ?? 0) * 3 ^
                      (SourceCodeRepository?.GetHashCode() ?? 0);

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   Name.FirstOrDefault().Text, ", ",
                   Version,                    ", ",
                   Vendor,                     ", ",
                   OpenSourceLicenses.Select(license => license.Id).AggregateWith(" / ")

               );

        #endregion

    }

}
