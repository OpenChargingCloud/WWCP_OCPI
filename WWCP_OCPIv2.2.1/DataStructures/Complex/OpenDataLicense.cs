/*
 * Copyright (c) 2015-2023 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
    /// Extension methods for Open Data licenses.
    /// </summary>
    public static class OpenDataLicenseExtensions
    {

        #region ToJSON(this OpenDataLicense)

        /// <summary>
        /// Return a JSON representation for the given enumeration of Open Data licenses.
        /// </summary>
        /// <param name="OpenDataLicense">An enumeration of Open Data licenses.</param>
        /// <param name="Skip">The optional number of Open Data licenses to skip.</param>
        /// <param name="Take">The optional number of Open Data licenses to return.</param>
        /// <param name="CustomDataLicenseSerializer">A delegate to serialize custom data license JSON elements.</param>
        public static JArray ToJSON(this IEnumerable<OpenDataLicense>                  OpenDataLicense,
                                    UInt64?                                            Skip                          = null,
                                    UInt64?                                            Take                          = null,
                                    CustomJObjectSerializerDelegate<OpenDataLicense>?  CustomDataLicenseSerializer   = null)

            => OpenDataLicense is null || !OpenDataLicense.Any()

                   ? new JArray()

                   : new JArray(OpenDataLicense.
                                    Where         (openDataLicense => openDataLicense is not null).
                                    OrderBy       (openDataLicense => openDataLicense.Id).
                                    SkipTakeFilter(Skip, Take).
                                    Select        (openDataLicense => openDataLicense.ToJSON(CustomDataLicenseSerializer)));

        #endregion

    }


    /// <summary>
    /// An Open Data license.
    /// </summary>
    public class OpenDataLicense : IEquatable<OpenDataLicense>,
                                   IComparable<OpenDataLicense>,
                                   IComparable
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of the object.
        /// </summary>
        public const String JSONLDContext = "https://opendata.social/contexts/wwcp+json/openDataLicenses";

        #endregion

        #region Properties

        /// <summary>
        /// The unique identification of the Open Data license.
        /// </summary>
        public OpenDataLicense_Id        Id             { get; }

        /// <summary>
        /// The description of the Open Data license.
        /// </summary>
        public IEnumerable<DisplayText>  Description    { get; }

        /// <summary>
        /// Optional URLs for more information on the Open Data license.
        /// </summary>
        public IEnumerable<URL>          URLs           { get; }

        #endregion

        #region Constructor(s)

        #region OpenDataLicense(Id,              params URLs)

        /// <summary>
        /// Create a new Open Data license.
        /// </summary>
        /// <param name="Id">The unique identification of the Open Data license.</param>
        /// <param name="URLs">Optional URLs for more information on the Open Data license.</param>
        public OpenDataLicense(OpenDataLicense_Id  Id,
                               params URL[]        URLs)
        {

            this.Id           = Id;
            this.Description  = Array.Empty<DisplayText>();
            this.URLs         = URLs?.Distinct().ToArray() ?? Array.Empty<URL>();

        }

        #endregion

        #region OpenDataLicense(Id, Description, params URLs)

        /// <summary>
        /// Create a new Open Data license.
        /// </summary>
        /// <param name="Id">The unique identification of the Open Data license.</param>
        /// <param name="Description">The description of the Open Data license.</param>
        /// <param name="URLs">Optional URLs for more information on the Open Data license.</param>
        public OpenDataLicense(OpenDataLicense_Id        Id,
                               IEnumerable<DisplayText>  Description,
                               params URL[]              URLs)
        {

            this.Id           = Id;
            this.Description  = Description?.Distinct().ToArray() ?? Array.Empty<DisplayText>();
            this.URLs         = URLs?.       Distinct().ToArray() ?? Array.Empty<URL>();

        }

        #endregion

        #endregion


        #region (static) Parse   (JSON, CustomOpenDataLicenseParser = null)

        /// <summary>
        /// Parse the given JSON representation of an Open Data license.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomOpenDataLicenseParser">A delegate to parse custom Open Data license JSON objects.</param>
        public static OpenDataLicense Parse(JObject                                        JSON,
                                            CustomJObjectParserDelegate<OpenDataLicense>?  CustomOpenDataLicenseParser   = null)
        {

            if (TryParse(JSON,
                         out var openDataLicense,
                         out var errorResponse,
                         CustomOpenDataLicenseParser))
            {
                return openDataLicense!;
            }

            throw new ArgumentException("The given JSON representation of an Open Data license is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out OpenDataLicense, out ErrorResponse, CustomOpenDataLicenseParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of an Open Data license.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="OpenDataLicense">The parsed Open Data license.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject               JSON,
                                       out OpenDataLicense?  OpenDataLicense,
                                       out String?           ErrorResponse)

            => TryParse(JSON,
                        out OpenDataLicense,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of an Open Data license.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="OpenDataLicense">The parsed Open Data license.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomOpenDataLicenseParser">A delegate to parse custom Open Data license JSON objects.</param>
        public static Boolean TryParse(JObject                                        JSON,
                                       out OpenDataLicense?                           OpenDataLicense,
                                       out String?                                    ErrorResponse,
                                       CustomJObjectParserDelegate<OpenDataLicense>?  CustomOpenDataLicenseParser   = null)
        {

            try
            {

                OpenDataLicense = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse Id             [mandatory]

                if (!JSON.ParseMandatory("id",
                                         "Open Data license identification",
                                         OpenDataLicense_Id.TryParse,
                                         out OpenDataLicense_Id Id,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Description    [optional]

                if (JSON.ParseOptionalHashSet("description",
                                              "Open Source license description",
                                              DisplayText.TryParse,
                                              out HashSet<DisplayText> Description,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }


                #endregion

                #region Parse URLs           [optional]

                if (JSON.ParseOptionalHashSet("urls",
                                              "Open Data license URLs",
                                              URL.TryParse,
                                              out HashSet<URL> URLs,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                OpenDataLicense = new OpenDataLicense(Id,
                                                      Description,
                                                      URLs.ToArray());

                if (CustomOpenDataLicenseParser is not null)
                    OpenDataLicense = CustomOpenDataLicenseParser(JSON,
                                                                  OpenDataLicense);

                return true;

            }
            catch (Exception e)
            {
                OpenDataLicense  = default;
                ErrorResponse    = "The given JSON representation of an Open Data license is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSourceLicenseSerializer = null)

        /// <summary>
        /// Return a JSON representation of the given data license.
        /// </summary>
        /// <param name="CustomDataLicenseSerializer">A delegate to serialize custom data license JSON elements.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<OpenDataLicense>?  CustomDataLicenseSerializer   = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("id",            Id.         ToString()),

                           Description.Any()
                               ? new JProperty("description",   new JArray(Description.Select(displayText => displayText.ToJSON())))
                               : null,

                                 new JProperty("urls",          new JArray(URLs.       Select(url         => url.        ToString())))

                       );

            return CustomDataLicenseSerializer is not null
                       ? CustomDataLicenseSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this object.
        /// </summary>
        public OpenDataLicense Clone()

            => new (Id.         Clone,
                    Description.Select(displayText => displayText.Clone()).ToArray(),
                    URLs.       Select(url         => url.        Clone).  ToArray());

        #endregion


        #region Static Definitions

        /// <summary>
        /// No license, ask the data source for more details.
        /// </summary>
        public static readonly OpenDataLicense None                              = new (OpenDataLicense_Id.Parse("None"),
                                                                                        DisplayText.CreateSet(Languages.en, "None"));


        // Open Data licenses

        /// <summary>
        /// Open Data Commons: Public Domain Dedication and License (PDDL)
        /// </summary>
        public static readonly OpenDataLicense PublicDomainDedicationAndLicense  = new (OpenDataLicense_Id.Parse("PDDL"),
                                                                                        DisplayText.CreateSet(Languages.en, "Open Data Commons: Public Domain Dedication and License"),
                                                                                        URL.Parse("http://opendatacommons.org/licenses/pddl/"));

        /// <summary>
        /// Open Data Commons: Attribution License (ODC-By)
        /// </summary>
        public static readonly OpenDataLicense AttributionLicense                = new (OpenDataLicense_Id.Parse("ODC-By"),
                                                                                        DisplayText.CreateSet(Languages.en, "Open Data Commons: Attribution License"),
                                                                                        URL.Parse("http://opendatacommons.org/licenses/by/"));

        /// <summary>
        /// Open Data Commons: Open Data Commons Open Database License (ODbL)
        /// Attribution and Share-Alike for Data/Databases
        /// </summary>
        public static readonly OpenDataLicense OpenDatabaseLicense               = new (OpenDataLicense_Id.Parse("ODbL"),
                                                                                        DisplayText.CreateSet(Languages.en, "Open Data Commons: Open Data Commons Open Database License"),
                                                                                        URL.Parse("http://opendatacommons.org/licenses/odbl/"),
                                                                                        URL.Parse("http://opendatacommons.org/licenses/odbl/summary/"),
                                                                                        URL.Parse("http://opendatacommons.org/licenses/odbl/1.0/"));




        // Special German licenses

        /// <summary>
        /// Datenlizenz Deutschland – Namensnennung – Version 2.0
        /// </summary>
        public static readonly OpenDataLicense DatenlizenzDeutschland_BY_2       = new (OpenDataLicense_Id.Parse("dl-de/by-2-0"),
                                                                                        DisplayText.CreateSet(Languages.de, "Datenlizenz Deutschland – Namensnennung – Version 2.0"),
                                                                                        URL.Parse("https://www.govdata.de/dl-de/by-2-0"));

        /// <summary>
        /// Datenlizenz Deutschland – Namensnennung – Version 2.0
        /// </summary>
        public static readonly OpenDataLicense DatenlizenzDeutschland_Zero_2     = new (OpenDataLicense_Id.Parse("dl-de/zero-2-0"),
                                                                                        DisplayText.CreateSet(Languages.de, "Datenlizenz Deutschland – Namensnennung – Version 2.0"),
                                                                                        URL.Parse("https://www.govdata.de/dl-de/zero-2-0"));

        /// <summary>
        /// GeoLizenz V1.3 – Open
        /// </summary>
        public static readonly OpenDataLicense GeoLizenz_OpenData_1_3_1          = new (OpenDataLicense_Id.Parse("GeoLizenz_V1.3"),
                                                                                        DisplayText.CreateSet(Languages.de, "GeoLizenz V1.3 – Open"),
                                                                                        URL.Parse("https://www.geolizenz.org/index/page.php?p=GL/opendata"),
                                                                                        URL.Parse("https://www.geolizenz.org/modules/geolizenz/docs/1.3.1/GeoLizenz_V1.3_Open_050615_V1.pdf"),
                                                                                        URL.Parse("https://www.geolizenz.org/modules/geolizenz/docs/1.3.1/Erl%C3%A4uterungen_GeoLizenzV1.3_Open_06.06.2015_V1.pdf"));




        // Creative Commons licenses

        /// <summary>
        /// Creative Commons Attribution 4.0 International (CC BY 4.0)
        /// </summary>
        public static readonly OpenDataLicense CreativeCommons_BY_4              = new (OpenDataLicense_Id.Parse("CC BY 4.0"),
                                                                                        DisplayText.CreateSet(Languages.en, "Creative Commons Attribution 4.0 International"),
                                                                                        URL.Parse("http://creativecommons.org/licenses/by/4.0/"),
                                                                                        URL.Parse("http://creativecommons.org/licenses/by/4.0/legalcode"));

        /// <summary>
        /// Creative Commons Attribution-ShareAlike 4.0 International (CC BY-SA 4.0)
        /// </summary>
        public static readonly OpenDataLicense CreativeCommons_BY_SA_4           = new (OpenDataLicense_Id.Parse("CC BY-SA 4.0"),
                                                                                        DisplayText.CreateSet(Languages.en, "Creative Commons Attribution-ShareAlike 4.0 International"),
                                                                                        URL.Parse("http://creativecommons.org/licenses/by-sa/4.0/"),
                                                                                        URL.Parse("http://creativecommons.org/licenses/by-sa/4.0/legalcode"));

        /// <summary>
        /// Creative Commons Attribution-NoDerivs 4.0 International (CC BY-ND 4.0)
        /// </summary>
        public static readonly OpenDataLicense CreativeCommons_BY_ND_4           = new (OpenDataLicense_Id.Parse("CC BY-ND 4.0"),
                                                                                        DisplayText.CreateSet(Languages.en, "Creative Commons Attribution-NoDerivs 4.0 International"),
                                                                                        URL.Parse("http://creativecommons.org/licenses/by-nd/4.0/"),
                                                                                        URL.Parse("http://creativecommons.org/licenses/by-nd/4.0/legalcode"));

        /// <summary>
        /// Creative Commons Attribution-NonCommercial 4.0 International (CC BY-NC 4.0)
        /// </summary>
        public static readonly OpenDataLicense CreativeCommons_BY_NC_4           = new (OpenDataLicense_Id.Parse("CC BY-NC 4.0"),
                                                                                        DisplayText.CreateSet(Languages.en, "Creative Commons Attribution-NonCommercial 4.0 International"),
                                                                                        URL.Parse("http://creativecommons.org/licenses/by-nc/4.0/"),
                                                                                        URL.Parse("http://creativecommons.org/licenses/by-nc/4.0/legalcode"));

        /// <summary>
        /// Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International (CC BY-NC-SA 4.0)
        /// </summary>
        public static readonly OpenDataLicense CreativeCommons_BY_NC_SA_4        = new (OpenDataLicense_Id.Parse("CC BY-NC-SA 4.0"),
                                                                                        DisplayText.CreateSet(Languages.en, "Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International"),
                                                                                        URL.Parse("http://creativecommons.org/licenses/by-nc-sa/4.0/"),
                                                                                        URL.Parse("http://creativecommons.org/licenses/by-nc-sa/4.0/legalcode"));

        /// <summary>
        /// Creative Commons Attribution-NonCommercial-NoDerivs 4.0 International (CC BY-NC-ND 4.0)
        /// </summary>
        public static readonly OpenDataLicense CreativeCommons_BY_NC_ND_4        = new (OpenDataLicense_Id.Parse("CC BY-NC-ND 4.0"),
                                                                                        DisplayText.CreateSet(Languages.en, "Creative Commons Attribution-NonCommercial-NoDerivs 4.0 International"),
                                                                                        URL.Parse("http://creativecommons.org/licenses/by-nc-nd/4.0/"),
                                                                                        URL.Parse("http://creativecommons.org/licenses/by-nc-nd/4.0/legalcode"));

        #endregion


        #region Operator overloading

        #region Operator == (OpenDataLicense1, OpenDataLicense2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OpenDataLicense1">An Open Data license.</param>
        /// <param name="OpenDataLicense2">Another Open Data license.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (OpenDataLicense OpenDataLicense1,
                                           OpenDataLicense OpenDataLicense2)

            => OpenDataLicense1.Equals(OpenDataLicense2);

        #endregion

        #region Operator != (OpenDataLicense1, OpenDataLicense2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OpenDataLicense1">An Open Data license.</param>
        /// <param name="OpenDataLicense2">Another Open Data license.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (OpenDataLicense OpenDataLicense1,
                                           OpenDataLicense OpenDataLicense2)

            => !(OpenDataLicense1 == OpenDataLicense2);

        #endregion

        #region Operator <  (OpenDataLicense1, OpenDataLicense2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OpenDataLicense1">An Open Data license.</param>
        /// <param name="OpenDataLicense2">Another Open Data license.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (OpenDataLicense OpenDataLicense1,
                                          OpenDataLicense OpenDataLicense2)

            => OpenDataLicense1.CompareTo(OpenDataLicense2) < 0;

        #endregion

        #region Operator <= (OpenDataLicense1, OpenDataLicense2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OpenDataLicense1">An Open Data license.</param>
        /// <param name="OpenDataLicense2">Another Open Data license.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (OpenDataLicense OpenDataLicense1,
                                           OpenDataLicense OpenDataLicense2)

            => OpenDataLicense1.CompareTo(OpenDataLicense2) <= 0;

        #endregion

        #region Operator >  (OpenDataLicense1, OpenDataLicense2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OpenDataLicense1">An Open Data license.</param>
        /// <param name="OpenDataLicense2">Another Open Data license.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (OpenDataLicense OpenDataLicense1,
                                          OpenDataLicense OpenDataLicense2)

            => OpenDataLicense1.CompareTo(OpenDataLicense2) > 0;

        #endregion

        #region Operator >= (OpenDataLicense1, OpenDataLicense2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OpenDataLicense1">An Open Data license.</param>
        /// <param name="OpenDataLicense2">Another Open Data license.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (OpenDataLicense OpenDataLicense1,
                                           OpenDataLicense OpenDataLicense2)

            => OpenDataLicense1.CompareTo(OpenDataLicense2) >= 0;

        #endregion

        #endregion

        #region IComparable<OpenDataLicense> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two Open Data licenses.
        /// </summary>
        /// <param name="Object">An Open Data license to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is OpenDataLicense openDataLicense
                   ? CompareTo(openDataLicense)
                   : throw new ArgumentException("The given object is not an Open Data license!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(OpenDataLicense)

        /// <summary>
        /// Compares two Open Data licenses.
        /// </summary>
        /// <param name="OpenDataLicense">An Open Data license to compare with.</param>
        public Int32 CompareTo(OpenDataLicense? OpenDataLicense)

            => OpenDataLicense is not null
                   ? Id.CompareTo(OpenDataLicense.Id)
                   : throw new ArgumentNullException(nameof(OpenDataLicense),
                                                     "The given Open Data license must not be null!");

        #endregion

        #endregion

        #region IEquatable<OpenDataLicense> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two Open Data licenses for equality.
        /// </summary>
        /// <param name="Object">An Open Data license to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is OpenDataLicense openDataLicense &&
                   Equals(openDataLicense);

        #endregion

        #region Equals(OpenDataLicense)

        /// <summary>
        /// Compares two Open Data licenses for equality.
        /// </summary>
        /// <param name="OpenDataLicense">An Open Data license to compare with.</param>
        public Boolean Equals(OpenDataLicense? OpenDataLicense)

            => OpenDataLicense is not null &&

               Id.Equals(OpenDataLicense.Id) &&

               Description.Count().Equals(OpenDataLicense.Description.Count()) &&
               Description.All(displayText => OpenDataLicense.Description.Contains(displayText)) &&

               URLs.       Count().Equals(OpenDataLicense.URLs.       Count()) &&
               URLs.       All(url         => OpenDataLicense.URLs.       Contains(url));

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

                return Id.         GetHashCode()  * 5 ^
                       Description.CalcHashCode() * 3 ^
                       URLs.       CalcHashCode();

            }
        }

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
                       : String.Empty,

                   URLs.Any()
                       ? ": " + URLs.       Select(url         => url.ToString().SubstringMax(15)).AggregateWith(", ")
                       : String.Empty

               );

        #endregion

    }

}
