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
    /// Extension methods for (Open) Data licenses.
    /// </summary>
    public static class DataLicenseExtensions
    {

        #region ToJSON(this DataLicense)

        /// <summary>
        /// Return a JSON representation for the given enumeration of (Open) Data licenses.
        /// </summary>
        /// <param name="DataLicense">An enumeration of (Open) Data licenses.</param>
        /// <param name="Skip">The optional number of (Open) Data licenses to skip.</param>
        /// <param name="Take">The optional number of (Open) Data licenses to return.</param>
        /// <param name="CustomDataLicenseSerializer">A delegate to serialize custom data license JSON elements.</param>
        public static JArray ToJSON(this IEnumerable<DataLicense>                  DataLicense,
                                    UInt64?                                            Skip                          = null,
                                    UInt64?                                            Take                          = null,
                                    CustomJObjectSerializerDelegate<DataLicense>?  CustomDataLicenseSerializer   = null)

            => DataLicense is null || !DataLicense.Any()

                   ? new JArray()

                   : new JArray(DataLicense.
                                    Where         (openDataLicense => openDataLicense is not null).
                                    OrderBy       (openDataLicense => openDataLicense.Id).
                                    SkipTakeFilter(Skip, Take).
                                    Select        (openDataLicense => openDataLicense.ToJSON(CustomDataLicenseSerializer)));

        #endregion

    }


    /// <summary>
    /// An (Open) Data license.
    /// </summary>
    public class DataLicense : IEquatable<DataLicense>,
                               IComparable<DataLicense>,
                               IComparable
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of the object.
        /// </summary>
        public const String JSONLDContext = "https://open.charging.cloud/contexts/OCPI/openDataLicense";

        #endregion

        #region Properties

        /// <summary>
        /// The unique identification of the (Open) Data license.
        /// </summary>
        public DataLicense_Id  Id             { get; }

        /// <summary>
        /// The description of the (Open) Data license.
        /// </summary>
        public DisplayTexts        Description    { get; }

        /// <summary>
        /// Optional URLs for more information on the (Open) Data license.
        /// </summary>
        public IEnumerable<URL>    URLs           { get; }

        #endregion

        #region Constructor(s)

        #region DataLicense(Id,              params URLs)

        /// <summary>
        /// Create a new (Open) Data license.
        /// </summary>
        /// <param name="Id">The unique identification of the (Open) Data license.</param>
        /// <param name="URLs">Optional URLs for more information on the (Open) Data license.</param>
        public DataLicense(DataLicense_Id           Id,
                           params IEnumerable<URL>  URLs)

            : this(Id,
                   DisplayTexts.Empty,
                   URLs)

        { }

        #endregion

        #region DataLicense(Id, Description, params URLs)

        /// <summary>
        /// Create a new (Open) Data license.
        /// </summary>
        /// <param name="Id">The unique identification of the (Open) Data license.</param>
        /// <param name="Description">The description of the (Open) Data license.</param>
        /// <param name="URLs">Optional URLs for more information on the (Open) Data license.</param>
        public DataLicense(DataLicense_Id           Id,
                           DisplayTexts             Description,
                           params IEnumerable<URL>  URLs)
        {

            this.Id           = Id;
            this.Description  = Description;
            this.URLs         = URLs?.Distinct().ToArray() ?? [];

            unchecked
            {

                hashCode = this.Id.         GetHashCode() * 5 ^
                           this.Description.GetHashCode() * 3 ^
                           this.URLs.       CalcHashCode();

            }

        }

        #endregion

        #endregion


        #region (static) Parse   (JSON, CustomDataLicenseParser = null)

        /// <summary>
        /// Parse the given JSON representation of an (Open) Data license.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomDataLicenseParser">A delegate to parse custom (Open) Data license JSON objects.</param>
        public static DataLicense Parse(JObject                                        JSON,
                                            CustomJObjectParserDelegate<DataLicense>?  CustomDataLicenseParser   = null)
        {

            if (TryParse(JSON,
                         out var openDataLicense,
                         out var errorResponse,
                         CustomDataLicenseParser))
            {
                return openDataLicense;
            }

            throw new ArgumentException("The given JSON representation of an (Open) Data license is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out DataLicense, out ErrorResponse, CustomDataLicenseParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of an (Open) Data license.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="DataLicense">The parsed (Open) Data license.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                    JSON,
                                       [NotNullWhen(true)]  out DataLicense?  DataLicense,
                                       [NotNullWhen(false)] out String?           ErrorResponse)

            => TryParse(JSON,
                        out DataLicense,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of an (Open) Data license.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="DataLicense">The parsed (Open) Data license.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomDataLicenseParser">A delegate to parse custom (Open) Data license JSON objects.</param>
        public static Boolean TryParse(JObject                                    JSON,
                                       [NotNullWhen(true)]  out DataLicense?      DataLicense,
                                       [NotNullWhen(false)] out String?           ErrorResponse,
                                       CustomJObjectParserDelegate<DataLicense>?  CustomDataLicenseParser   = null)
        {

            try
            {

                DataLicense = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse Id             [mandatory]

                if (!JSON.ParseMandatory("id",
                                         "(Open) Data license identification",
                                         DataLicense_Id.TryParse,
                                         out DataLicense_Id Id,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Description    [optional]

                if (JSON.ParseOptionalJSONArray("description",
                                                "(Open) Data license description",
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
                                              "(Open) Data license URLs",
                                              URL.TryParse,
                                              out HashSet<URL> URLs,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                DataLicense = new DataLicense(
                                  Id,
                                  Description,
                                  [.. URLs]
                              );

                if (CustomDataLicenseParser is not null)
                    DataLicense = CustomDataLicenseParser(JSON,
                                                          DataLicense);

                return true;

            }
            catch (Exception e)
            {
                DataLicense  = default;
                ErrorResponse    = "The given JSON representation of an (Open) Data license is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSourceLicenseSerializer = null, CustomDisplayTextsSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of the given data license.
        /// </summary>
        /// <param name="CustomDataLicenseSerializer">A delegate to serialize custom data license JSON elements.</param>
        /// <param name="CustomDisplayTextsSerializer">A delegate to serialize custom display texts JSON objects.</param>
        /// <param name="CustomDisplayTextSerializer">A delegate to serialize custom platform party JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<DataLicense>?  CustomDataLicenseSerializer    = null,
                              CustomJArraySerializerDelegate<DisplayTexts>?  CustomDisplayTextsSerializer   = null,
                              CustomJObjectSerializerDelegate<DisplayText>?  CustomDisplayTextSerializer    = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("id",            Id.         ToString()),

                           Description.Any()
                               ? new JProperty("description",   Description.ToJSON(CustomDisplayTextsSerializer,
                                                                                   CustomDisplayTextSerializer))
                               : null,

                                 new JProperty("urls",          new JArray(URLs.Select(url => url.ToString())))

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
        public DataLicense Clone()

            => new (
                   Id.         Clone(),
                   Description.Clone(),
                   URLs.       Select(url => url.Clone())
               );

        #endregion


        #region Static Definitions

        /// <summary>
        /// No license, ask the data source for more details.
        /// </summary>
        public static readonly DataLicense None                              = new (DataLicense_Id.Parse("None"),
                                                                                    DisplayTexts.Create(Languages.en, "None"));


        // (Open) Data licenses

        /// <summary>
        /// (Open) Data Commons: Public Domain Dedication and License (PDDL)
        /// </summary>
        public static readonly DataLicense PublicDomainDedicationAndLicense  = new (DataLicense_Id.Parse("PDDL"),
                                                                                    DisplayTexts.Create(Languages.en, "(Open) Data Commons: Public Domain Dedication and License"),
                                                                                    URL.Parse("http://opendatacommons.org/licenses/pddl/"));

        /// <summary>
        /// (Open) Data Commons: Attribution License (ODC-By)
        /// </summary>
        public static readonly DataLicense AttributionLicense                = new (DataLicense_Id.Parse("ODC-By"),
                                                                                    DisplayTexts.Create(Languages.en, "(Open) Data Commons: Attribution License"),
                                                                                    URL.Parse("http://opendatacommons.org/licenses/by/"));

        /// <summary>
        /// (Open) Data Commons: (Open) Data Commons (Open) Database License (ODbL)
        /// Attribution and Share-Alike for Data/Databases
        /// </summary>
        public static readonly DataLicense OpenDatabaseLicense               = new (DataLicense_Id.Parse("ODbL"),
                                                                                    DisplayTexts.Create(Languages.en, "(Open) Data Commons: (Open) Data Commons (Open) Database License"),
                                                                                    URL.Parse("http://opendatacommons.org/licenses/odbl/"),
                                                                                    URL.Parse("http://opendatacommons.org/licenses/odbl/summary/"),
                                                                                    URL.Parse("http://opendatacommons.org/licenses/odbl/1.0/"));




        // Special German licenses

        /// <summary>
        /// Datenlizenz Deutschland – Namensnennung – Version 2.0
        /// </summary>
        public static readonly DataLicense DatenlizenzDeutschland_BY_2       = new (DataLicense_Id.Parse("dl-de/by-2-0"),
                                                                                    DisplayTexts.Create(Languages.de, "Datenlizenz Deutschland – Namensnennung – Version 2.0"),
                                                                                    URL.Parse("https://www.govdata.de/dl-de/by-2-0"));

        /// <summary>
        /// Datenlizenz Deutschland – Namensnennung – Version 2.0
        /// </summary>
        public static readonly DataLicense DatenlizenzDeutschland_Zero_2     = new (DataLicense_Id.Parse("dl-de/zero-2-0"),
                                                                                    DisplayTexts.Create(Languages.de, "Datenlizenz Deutschland – Namensnennung – Version 2.0"),
                                                                                    URL.Parse("https://www.govdata.de/dl-de/zero-2-0"));

        /// <summary>
        /// GeoLizenz V1.3 – Open
        /// </summary>
        public static readonly DataLicense GeoLizenz_OpenData_1_3_1          = new (DataLicense_Id.Parse("GeoLizenz_V1.3"),
                                                                                    DisplayTexts.Create(Languages.de, "GeoLizenz V1.3 – Open"),
                                                                                    URL.Parse("https://www.geolizenz.org/index/page.php?p=GL/opendata"),
                                                                                    URL.Parse("https://www.geolizenz.org/modules/geolizenz/docs/1.3.1/GeoLizenz_V1.3_Open_050615_V1.pdf"),
                                                                                    URL.Parse("https://www.geolizenz.org/modules/geolizenz/docs/1.3.1/Erl%C3%A4uterungen_GeoLizenzV1.3_Open_06.06.2015_V1.pdf"));




        // Creative Commons licenses

        /// <summary>
        /// Creative Commons Attribution 4.0 International (CC BY 4.0)
        /// </summary>
        public static readonly DataLicense CreativeCommons_BY_4              = new (DataLicense_Id.Parse("CC BY 4.0"),
                                                                                    DisplayTexts.Create(Languages.en, "Creative Commons Attribution 4.0 International"),
                                                                                    URL.Parse("http://creativecommons.org/licenses/by/4.0/"),
                                                                                    URL.Parse("http://creativecommons.org/licenses/by/4.0/legalcode"));

        /// <summary>
        /// Creative Commons Attribution-ShareAlike 4.0 International (CC BY-SA 4.0)
        /// </summary>
        public static readonly DataLicense CreativeCommons_BY_SA_4           = new (DataLicense_Id.Parse("CC BY-SA 4.0"),
                                                                                    DisplayTexts.Create(Languages.en, "Creative Commons Attribution-ShareAlike 4.0 International"),
                                                                                    URL.Parse("http://creativecommons.org/licenses/by-sa/4.0/"),
                                                                                    URL.Parse("http://creativecommons.org/licenses/by-sa/4.0/legalcode"));

        /// <summary>
        /// Creative Commons Attribution-NoDerivs 4.0 International (CC BY-ND 4.0)
        /// </summary>
        public static readonly DataLicense CreativeCommons_BY_ND_4           = new (DataLicense_Id.Parse("CC BY-ND 4.0"),
                                                                                    DisplayTexts.Create(Languages.en, "Creative Commons Attribution-NoDerivs 4.0 International"),
                                                                                    URL.Parse("http://creativecommons.org/licenses/by-nd/4.0/"),
                                                                                    URL.Parse("http://creativecommons.org/licenses/by-nd/4.0/legalcode"));

        /// <summary>
        /// Creative Commons Attribution-NonCommercial 4.0 International (CC BY-NC 4.0)
        /// </summary>
        public static readonly DataLicense CreativeCommons_BY_NC_4           = new (DataLicense_Id.Parse("CC BY-NC 4.0"),
                                                                                    DisplayTexts.Create(Languages.en, "Creative Commons Attribution-NonCommercial 4.0 International"),
                                                                                    URL.Parse("http://creativecommons.org/licenses/by-nc/4.0/"),
                                                                                    URL.Parse("http://creativecommons.org/licenses/by-nc/4.0/legalcode"));

        /// <summary>
        /// Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International (CC BY-NC-SA 4.0)
        /// </summary>
        public static readonly DataLicense CreativeCommons_BY_NC_SA_4        = new (DataLicense_Id.Parse("CC BY-NC-SA 4.0"),
                                                                                    DisplayTexts.Create(Languages.en, "Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International"),
                                                                                    URL.Parse("http://creativecommons.org/licenses/by-nc-sa/4.0/"),
                                                                                    URL.Parse("http://creativecommons.org/licenses/by-nc-sa/4.0/legalcode"));

        /// <summary>
        /// Creative Commons Attribution-NonCommercial-NoDerivs 4.0 International (CC BY-NC-ND 4.0)
        /// </summary>
        public static readonly DataLicense CreativeCommons_BY_NC_ND_4        = new (DataLicense_Id.Parse("CC BY-NC-ND 4.0"),
                                                                                    DisplayTexts.Create(Languages.en, "Creative Commons Attribution-NonCommercial-NoDerivs 4.0 International"),
                                                                                    URL.Parse("http://creativecommons.org/licenses/by-nc-nd/4.0/"),
                                                                                    URL.Parse("http://creativecommons.org/licenses/by-nc-nd/4.0/legalcode"));

        #endregion


        #region Operator overloading

        #region Operator == (DataLicense1, DataLicense2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DataLicense1">An (Open) Data license.</param>
        /// <param name="DataLicense2">Another (Open) Data license.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (DataLicense DataLicense1,
                                           DataLicense DataLicense2)

            => DataLicense1.Equals(DataLicense2);

        #endregion

        #region Operator != (DataLicense1, DataLicense2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DataLicense1">An (Open) Data license.</param>
        /// <param name="DataLicense2">Another (Open) Data license.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (DataLicense DataLicense1,
                                           DataLicense DataLicense2)

            => !(DataLicense1 == DataLicense2);

        #endregion

        #region Operator <  (DataLicense1, DataLicense2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DataLicense1">An (Open) Data license.</param>
        /// <param name="DataLicense2">Another (Open) Data license.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (DataLicense DataLicense1,
                                          DataLicense DataLicense2)

            => DataLicense1.CompareTo(DataLicense2) < 0;

        #endregion

        #region Operator <= (DataLicense1, DataLicense2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DataLicense1">An (Open) Data license.</param>
        /// <param name="DataLicense2">Another (Open) Data license.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (DataLicense DataLicense1,
                                           DataLicense DataLicense2)

            => DataLicense1.CompareTo(DataLicense2) <= 0;

        #endregion

        #region Operator >  (DataLicense1, DataLicense2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DataLicense1">An (Open) Data license.</param>
        /// <param name="DataLicense2">Another (Open) Data license.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (DataLicense DataLicense1,
                                          DataLicense DataLicense2)

            => DataLicense1.CompareTo(DataLicense2) > 0;

        #endregion

        #region Operator >= (DataLicense1, DataLicense2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DataLicense1">An (Open) Data license.</param>
        /// <param name="DataLicense2">Another (Open) Data license.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (DataLicense DataLicense1,
                                           DataLicense DataLicense2)

            => DataLicense1.CompareTo(DataLicense2) >= 0;

        #endregion

        #endregion

        #region IComparable<DataLicense> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two (Open) Data licenses.
        /// </summary>
        /// <param name="Object">An (Open) Data license to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is DataLicense openDataLicense
                   ? CompareTo(openDataLicense)
                   : throw new ArgumentException("The given object is not an (Open) Data license!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(DataLicense)

        /// <summary>
        /// Compares two (Open) Data licenses.
        /// </summary>
        /// <param name="DataLicense">An (Open) Data license to compare with.</param>
        public Int32 CompareTo(DataLicense? DataLicense)

            => DataLicense is not null
                   ? Id.CompareTo(DataLicense.Id)
                   : throw new ArgumentNullException(nameof(DataLicense),
                                                     "The given (Open) Data license must not be null!");

        #endregion

        #endregion

        #region IEquatable<DataLicense> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two (Open) Data licenses for equality.
        /// </summary>
        /// <param name="Object">An (Open) Data license to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is DataLicense openDataLicense &&
                   Equals(openDataLicense);

        #endregion

        #region Equals(DataLicense)

        /// <summary>
        /// Compares two (Open) Data licenses for equality.
        /// </summary>
        /// <param name="DataLicense">An (Open) Data license to compare with.</param>
        public Boolean Equals(DataLicense? DataLicense)

            => DataLicense is not null &&

               Id.Equals(DataLicense.Id) &&

               Description.Count().Equals(DataLicense.Description.Count()) &&
               Description.All(displayText => DataLicense.Description.Contains(displayText)) &&

               URLs.       Count().Equals(DataLicense.URLs.       Count()) &&
               URLs.       All(url         => DataLicense.URLs.       Contains(url));

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
