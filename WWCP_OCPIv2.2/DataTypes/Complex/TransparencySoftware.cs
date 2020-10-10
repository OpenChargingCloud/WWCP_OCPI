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
    /// A transparency software. This information will e.g. be used for the German calibration law.
    /// </summary>
    [NonStandard]
    public class TransparencySoftware : IEquatable<TransparencySoftware>,
                                        IComparable<TransparencySoftware>,
                                        IComparable
    {

        #region Properties

        /// <summary>
        /// The name of the transparency software.
        /// </summary>
        [Mandatory]
        public String              Name                     { get; }

        /// <summary>
        /// The version of the transparency software.
        /// </summary>
        [Mandatory]
        public String              Version                  { get; }

        /// <summary>
        /// The legal status of the transparency software.
        /// </summary>
        [Mandatory]
        public LegalStatus         LegalStatus              { get; }

        /// <summary>
        /// The Open Source license of the transparency software.
        /// </summary>
        [Mandatory]
        public OpenSourceLicenses  OpenSourceLicense        { get; }

        /// <summary>
        /// The vendor of the transparency software.
        /// </summary>
        [Mandatory]
        public String              Vendor                   { get; }

        /// <summary>
        /// An optional URL where to find a small logo of the transparency software.
        /// </summary>
        [Optional]
        public URL?                Logo                     { get; }

        /// <summary>
        /// An optional URL where to find a manual how to use the transparency software.
        /// </summary>
        [Optional]
        public URL?                HowToUse                 { get; }

        /// <summary>
        /// An optional URL where to find more information about the transparency software.
        /// </summary>
        [Optional]
        public URL?                MoreInformation          { get; }

        /// <summary>
        /// An optional URL where to find the source code of the transparency software.
        /// </summary>
        [Optional]
        public URL?                SourceCodeRepository     { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new transparency software.
        /// </summary>
        /// <param name="Name">The name of the transparency software.</param>
        /// <param name="Version">The version of the transparency software.</param>
        /// <param name="LegalStatus">The legal status of the transparency software.</param>
        /// <param name="OpenSourceLicense">The Open Source license of the transparency software.</param>
        /// <param name="Vendor">The vendor of the transparency software.</param>
        /// 
        /// <param name="Logo">An optional URL where to find a small logo of the transparency software.</param>
        /// <param name="HowToUse">An optional URL where to find a manual how to use the transparency software.</param>
        /// <param name="MoreInformation">An optional URL where to find more information about the transparency software.</param>
        /// <param name="SourceCodeRepository">An optional URL where to find the source code of the transparency software.</param>
        public TransparencySoftware(String              Name,
                                    String              Version,
                                    LegalStatus         LegalStatus,
                                    OpenSourceLicenses  OpenSourceLicense,
                                    String              Vendor,

                                    URL?                Logo                   = null,
                                    URL?                HowToUse               = null,
                                    URL?                MoreInformation        = null,
                                    URL?                SourceCodeRepository   = null)
        {

            this.Name                  = Name;
            this.Version               = Version;
            this.LegalStatus           = LegalStatus;
            this.OpenSourceLicense     = OpenSourceLicense;
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
        public static TransparencySoftware Parse(JObject                                            JSON,
                                                 CustomJObjectParserDelegate<TransparencySoftware>  CustomTransparencySoftwareParser   = null)
        {

            if (TryParse(JSON,
                         out TransparencySoftware  transparencySoftware,
                         out String                ErrorResponse,
                         CustomTransparencySoftwareParser))
            {
                return transparencySoftware;
            }

            throw new ArgumentException("The given JSON representation of a transparency software is invalid: " + ErrorResponse, nameof(JSON));

        }

        #endregion

        #region (static) Parse   (Text, CustomTransparencySoftwareParser = null)

        /// <summary>
        /// Parse the given text representation of a transparency software.
        /// </summary>
        /// <param name="Text">The text to parse.</param>
        /// <param name="CustomTransparencySoftwareParser">A delegate to parse custom transparency software JSON objects.</param>
        public static TransparencySoftware Parse(String                                             Text,
                                                 CustomJObjectParserDelegate<TransparencySoftware>  CustomTransparencySoftwareParser   = null)
        {

            if (TryParse(Text,
                         out TransparencySoftware  transparencySoftware,
                         out String                ErrorResponse,
                         CustomTransparencySoftwareParser))
            {
                return transparencySoftware;
            }

            throw new ArgumentException("The given text representation of a transparency software is invalid: " + ErrorResponse, nameof(Text));

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
        public static Boolean TryParse(JObject                   JSON,
                                       out TransparencySoftware  TransparencySoftware,
                                       out String                ErrorResponse)

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
        public static Boolean TryParse(JObject                                            JSON,
                                       out TransparencySoftware                           TransparencySoftware,
                                       out String                                         ErrorResponse,
                                       CustomJObjectParserDelegate<TransparencySoftware>  CustomTransparencySoftwareParser   = null)
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

                if (!JSON.ParseMandatoryText("name",
                                             "name",
                                             out String Name,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Version                   [mandatory]

                if (!JSON.ParseMandatoryText("version",
                                             "version",
                                             out String Version,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse LegalStatus               [mandatory]

                if (!JSON.ParseMandatoryEnum("legal_status",
                                             "legal status",
                                             out LegalStatus LegalStatus,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse OpenSourceLicense         [mandatory]

                if (!JSON.ParseMandatoryEnum("open_source_license",
                                             "legal status",
                                             out OpenSourceLicenses OpenSourceLicense,
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

                if (JSON.ParseOptional("logo",
                                       "logo",
                                       URL.TryParse,
                                       out URL? Logo,
                                       out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse HowToUse                  [optional]

                if (JSON.ParseOptional("how_to_use",
                                       "how to use",
                                       URL.TryParse,
                                       out URL? HowToUse,
                                       out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse MoreInformation           [optional]

                if (JSON.ParseOptional("more_information",
                                       "more information",
                                       URL.TryParse,
                                       out URL? MoreInformation,
                                       out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse SourceCodeRepository      [optional]

                if (JSON.ParseOptional("source_code_repository",
                                       "source code repository",
                                       URL.TryParse,
                                       out URL? SourceCodeRepository,
                                       out ErrorResponse))
                {
                    return false;
                }

                #endregion


                TransparencySoftware = new TransparencySoftware(Name,
                                                                Version,
                                                                LegalStatus,
                                                                OpenSourceLicense,
                                                                Vendor,
                                                                Logo,
                                                                HowToUse,
                                                                MoreInformation,
                                                                SourceCodeRepository);

                if (CustomTransparencySoftwareParser != null)
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

        #region (static) TryParse(Text, out TransparencySoftware, out ErrorResponse, CustomTransparencySoftwareParser = null)

        /// <summary>
        /// Try to parse the given text representation of a transparency software.
        /// </summary>
        /// <param name="Text">The text to parse.</param>
        /// <param name="TransparencySoftware">The parsed transparencySoftware.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomTransparencySoftwareParser">A delegate to parse custom transparency software JSON objects.</param>
        public static Boolean TryParse(String                                             Text,
                                       out TransparencySoftware                           TransparencySoftware,
                                       out String                                         ErrorResponse,
                                       CustomJObjectParserDelegate<TransparencySoftware>  CustomTransparencySoftwareParser   = null)
        {

            try
            {

                return TryParse(JObject.Parse(Text),
                                out TransparencySoftware,
                                out ErrorResponse,
                                CustomTransparencySoftwareParser);

            }
            catch (Exception e)
            {
                TransparencySoftware  = default;
                ErrorResponse         = "The given text representation of a transparency software is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomTransparencySoftwareSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomTransparencySoftwareSerializer">A delegate to serialize custom transparency software JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<TransparencySoftware> CustomTransparencySoftwareSerializer = null)
        {

            var JSON = JSONObject.Create(

                           new JProperty("name",                 Name),
                           new JProperty("version",              Version),
                           new JProperty("legal_status",         LegalStatus.      ToString()),
                           new JProperty("open_source_license",  OpenSourceLicense.ToString()),
                           new JProperty("vendor",               Vendor),

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

            return CustomTransparencySoftwareSerializer != null
                       ? CustomTransparencySoftwareSerializer(this, JSON)
                       : JSON;

        }

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
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is TransparencySoftware transparencySoftware
                   ? CompareTo(transparencySoftware)
                   : throw new ArgumentException("The given object is not a transparency software!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(TransparencySoftware)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TransparencySoftware">An object to compare with.</param>
        public Int32 CompareTo(TransparencySoftware TransparencySoftware)
        {

            if (TransparencySoftware is null)
                throw new ArgumentNullException(nameof(TransparencySoftware), "The give transparency software must not be null!");

            var c = Name.             CompareTo(TransparencySoftware.Name);

            if (c == 0)
                c = Version.          CompareTo(TransparencySoftware.Version);

            if (c == 0)
                c = LegalStatus.      CompareTo(TransparencySoftware.LegalStatus);

            if (c == 0)
                c = OpenSourceLicense.CompareTo(TransparencySoftware.OpenSourceLicense);

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

        #endregion

        #region IEquatable<TransparencySoftware> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is TransparencySoftware transparencySoftware &&
                   Equals(transparencySoftware);

        #endregion

        #region Equals(TransparencySoftware)

        /// <summary>
        /// Compares two transparency softwares for equality.
        /// </summary>
        /// <param name="TransparencySoftware">A transparency software to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(TransparencySoftware TransparencySoftware)

            => !(TransparencySoftware is null) &&

               Name.             Equals(TransparencySoftware.Name)              &&
               Version.          Equals(TransparencySoftware.Version)           &&
               LegalStatus.      Equals(TransparencySoftware.LegalStatus)       &&
               OpenSourceLicense.Equals(TransparencySoftware.OpenSourceLicense) &&
               Vendor.           Equals(TransparencySoftware.Vendor)            &&

            ((!Logo.                HasValue && !TransparencySoftware.Logo.                HasValue) ||
              (Logo.                HasValue &&  TransparencySoftware.Logo.                HasValue && Logo.                Value.Equals(TransparencySoftware.Logo.                Value))) &&

            ((!HowToUse.            HasValue && !TransparencySoftware.HowToUse.            HasValue) ||
              (HowToUse.            HasValue &&  TransparencySoftware.HowToUse.            HasValue && HowToUse.            Value.Equals(TransparencySoftware.HowToUse.            Value))) &&

            ((!MoreInformation.     HasValue && !TransparencySoftware.MoreInformation.     HasValue) ||
              (MoreInformation.     HasValue &&  TransparencySoftware.MoreInformation.     HasValue && HowToUse.            Value.Equals(TransparencySoftware.MoreInformation.     Value))) &&

            ((!SourceCodeRepository.HasValue && !TransparencySoftware.SourceCodeRepository.HasValue) ||
              (SourceCodeRepository.HasValue &&  TransparencySoftware.SourceCodeRepository.HasValue && SourceCodeRepository.Value.Equals(TransparencySoftware.SourceCodeRepository.Value)));

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

                return Name.             GetHashCode() * 21 ^
                       Version.          GetHashCode() * 19 ^
                       LegalStatus.      GetHashCode() * 17 ^
                       OpenSourceLicense.GetHashCode() * 13 ^
                       Vendor.           GetHashCode() * 11 ^

                       (Logo.                HasValue
                           ? Logo.                GetHashCode() * 7
                           : 0) ^

                       (HowToUse.            HasValue
                           ? HowToUse.            GetHashCode() * 5
                           : 0) ^

                       (MoreInformation.     HasValue
                           ? MoreInformation.     GetHashCode() * 3
                           : 0) ^

                       (SourceCodeRepository.HasValue
                           ? SourceCodeRepository.GetHashCode()
                           : 0);

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(Name, " ", Version,
                             " [", LegalStatus, "] ",
                             Vendor, " ",
                             OpenSourceLicense);

        #endregion

    }

}
