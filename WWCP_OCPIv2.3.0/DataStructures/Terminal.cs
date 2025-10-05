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

using System.Text;
using System.Security.Cryptography;
using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Aegir;
using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;
using cloud.charging.open.protocols.OCPIv2_3_0;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_3_0
{

    /// <summary>
    /// The Terminal object describes one physical payment terminal.
    /// It is designed primarily to establish a mapping between charge points (locations and/or EVSEs) and payment terminals.
    /// The object facilitates the configuration of necessary payment-related data,
    /// such as customer reference identifiers and invoice URLs
    /// </summary>
    public class Terminal : IHasId<Terminal_Id>,
                            IEquatable<Terminal>,
                            IComparable<Terminal>,
                            IComparable
    {

        #region Data

        /// <summary>
        /// The default JSON-LD context of locations.
        /// </summary>
        public static readonly JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/contexts/OCPI/2.3/terminal");

        private readonly Lock patchLock = new();

        #endregion

        #region Properties

        /// <summary>
        /// The parent CommonAPI of the terminal.
        /// </summary>
        internal CommonAPI?                       CommonAPI            { get; set; }

        /// <summary>
        /// The unique identification of the terminal.
        /// </summary>
        [Mandatory]
        public   Terminal_Id                      Id                   { get; }

        /// <summary>
        /// This optional reference will be used to link the terminal to a CSMS.
        /// The reference might also be provided via the order process.
        /// </summary>
        [Optional]
        public   Customer_Reference?              CustomerReference    { get; }

        /// <summary>
        /// The optional country code as an alternative to the customer reference.
        /// </summary>
        [Optional]
        public   CountryCode?                     CountryCode          { get; }

        /// <summary>
        /// The optional party identification as an alternative to the customer reference.
        /// </summary>
        [Optional]
        public   Party_Id?                        PartyId              { get; }

        /// <summary>
        /// The optional street/block name and house number of the terminal.
        /// </summary>
        [Optional]
        public   String?                          Address              { get; }

        /// <summary>
        /// The optional city or town of the terminal.
        /// </summary>
        [Optional]
        public   String?                          City                 { get; }

        /// <summary>
        /// The optional postal code of the terminal.
        /// </summary>
        [Optional]
        public   String?                          PostalCode           { get; }

        /// <summary>
        /// The optional state or province of the terminal.
        /// </summary>
        [Optional]
        public   String?                          State                { get; }

        /// <summary>
        /// The country of the terminal.
        /// </summary>
        [Optional]
        public   Country?                         Country              { get; }

        /// <summary>
        /// The optional floor level on which the terminal is located (in garage buildings)
        /// in the locally displayed numbering scheme.
        /// </summary>
        [Optional, VendorExtension(VE.GraphDefined)]
        public   String?                          FloorLevel           { get; }

        /// <summary>
        /// The optional number/string printed on the outside of the terminal for visual identification.
        /// </summary>
        [Optional, VendorExtension(VE.GraphDefined)]
        public   String?                          PhysicalReference    { get; }

        /// <summary>
        /// The optional geographical location of the terminal.
        /// </summary>
        [Optional]
        public   GeoCoordinate?                   Coordinates          { get; }

        /// <summary>
        /// The optional multi-language human-readable directions when more detailed
        /// information on how to reach the terminal.
        /// </summary>
        [Optional, VendorExtension(VE.GraphDefined)]
        public   IEnumerable<DisplayText>         Directions           { get; }

        /// <summary>
        /// Optional enumeration of images related to the terminal such as photos or logos.
        /// </summary>
        [Optional]
        public   IEnumerable<Image>               Images               { get; }

        /// <summary>
        /// Optional enumeration of all functionalities of the terminal.
        /// </summary>
        [Optional, VendorExtension(VE.GraphDefined)]
        public   IEnumerable<TerminalCapability>  Capabilities         { get; }

        /// <summary>
        /// Optional enumeration of all supported payment methods.
        /// </summary>
        [Optional, VendorExtension(VE.GraphDefined)]
        public   IEnumerable<PaymentMethod>       PaymentMethods       { get; }

        /// <summary>
        /// Optional enumeration of all supported payment brands.
        /// </summary>
        [Optional, VendorExtension(VE.GraphDefined)]
        public   IEnumerable<PaymentBrand>        PaymentBrands        { get; }

        /// <summary>
        /// The optional BaseURL for downloading invoices.
        /// </summary>
        [Optional]
        public   URL?                             InvoiceBaseURL       { get; }

        /// <summary>
        /// Describes which party creates the invoice for the EV driver.
        /// </summary>
        [Optional]
        public   Invoice_Creator?                 InvoiceCreator       { get; }

        /// <summary>
        /// Mapping value as issued by the PTP (e.g serial number).
        /// </summary>
        [Optional]
        public   String?                          Reference            { get; }

        /// <summary>
        /// List of all locations assigned to that terminal.
        /// </summary>
        [Optional]
        public   IEnumerable<Location_Id>         LocationIds          { get; }

        /// <summary>
        /// List of all EVSEs assigned to that terminal.
        /// </summary>
        [Optional]
        public   IEnumerable<EVSE_UId>            EVSEUIds             { get; }


        /// <summary>
        /// The timestamp when this terminal was created.
        /// </summary>
        [Mandatory, VendorExtension(VE.GraphDefined, VE.Pagination)]
        public   DateTimeOffset                   Created              { get; }

        /// <summary>
        /// Timestamp when this terminal was last updated (or created).
        /// </summary>
        [Mandatory]
        public   DateTimeOffset                   LastUpdated          { get; }

        /// <summary>
        /// The SHA256 hash of the JSON representation of this terminal used as HTTP ETag.
        /// </summary>
        [Mandatory, VendorExtension(VE.GraphDefined)]
        public   String                           ETag                 { get; private set; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new Terminal.
        /// </summary>
        /// <param name="Id">The unique identification of the Terminal.</param>
        /// <param name="CustomerReference">This optional reference will be used to link the terminal to a CSMS. The reference might also be provided via the order process.</param>
        /// <param name="CountryCode">An optional country code as an alternative to the customer reference.</param>
        /// <param name="PartyId">An optional party identification as an alternative to the customer reference.</param>
        /// <param name="Address">An optional street/block name and house number of the terminal.</param>
        /// <param name="City">An optional city or town of the terminal.</param>
        /// <param name="PostalCode">An optional postal code of the terminal.</param>
        /// <param name="State">An optional state or province of the terminal.</param>
        /// <param name="Country">An optional country of the terminal.</param>
        /// <param name="FloorLevel">An optional floor level on which the terminal is located (in garage buildings) in the locally displayed numbering scheme.</param>
        /// <param name="PhysicalReference">An optional number/string printed on the outside of the terminal for visual identification.</param>
        /// <param name="Coordinates">An optional geographical location of the terminal.</param>
        /// <param name="Directions">An optional multi-language human-readable directions when more detailed information on how to reach the terminal.</param>
        /// <param name="Images">An optional enumeration of images related to the terminal such as photos or logos.</param>
        /// <param name="Capabilities">An optional enumeration of all functionalities of the terminal.</param>
        /// <param name="PaymentMethods">An optional enumeration of all supported payment methods.</param>
        /// <param name="PaymentBrands">An optional enumeration of all supported payment brands.</param>
        /// <param name="InvoiceBaseURL">An optional BaseURL for downloading invoices.</param>
        /// <param name="InvoiceCreator">An optional enumeration of all supported payment brands.</param>
        /// <param name="Reference">An optional mapping value as issued by the PTP (e.g serial number).</param>
        /// <param name="LocationIds">An optional list of all locations assigned to that terminal.</param>
        /// <param name="EVSEUIds">An optional list of all EVSEs assigned to that terminal.</param>
        /// 
        /// <param name="Created">An optional timestamp when this terminal was created.</param>
        /// <param name="LastUpdated">An optional timestamp when this terminal was last updated (or created).</param>
        /// 
        /// <param name="CommonAPI">An optional CommonAPI of this terminal.</param>
        /// <param name="CustomTerminalSerializer">A delegate to serialize custom Terminal JSON objects.</param>
        /// <param name="CustomDisplayTextSerializer">A delegate to serialize custom multi-language text JSON objects.</param>
        /// <param name="CustomImageSerializer">A delegate to serialize custom image JSON objects.</param>
        public Terminal(Terminal_Id                                    Id,
                        Customer_Reference?                            CustomerReference             = null,
                        CountryCode?                                   CountryCode                   = null,
                        Party_Id?                                      PartyId                       = null,
                        String?                                        Address                       = null,
                        String?                                        City                          = null,
                        String?                                        PostalCode                    = null,
                        String?                                        State                         = null,
                        Country?                                       Country                       = null,
                        String?                                        FloorLevel                    = null,
                        String?                                        PhysicalReference             = null,
                        GeoCoordinate?                                 Coordinates                   = null,
                        IEnumerable<DisplayText>?                      Directions                    = null,
                        IEnumerable<Image>?                            Images                        = null,
                        IEnumerable<TerminalCapability>?               Capabilities                  = null,
                        IEnumerable<PaymentMethod>?                    PaymentMethods                = null,
                        IEnumerable<PaymentBrand>?                     PaymentBrands                 = null,
                        URL?                                           InvoiceBaseURL                = null,
                        Invoice_Creator?                               InvoiceCreator                = null,
                        String?                                        Reference                     = null,
                        IEnumerable<Location_Id>?                      LocationIds                   = null,
                        IEnumerable<EVSE_UId>?                         EVSEUIds                      = null,

                        DateTimeOffset?                                Created                       = null,
                        DateTimeOffset?                                LastUpdated                   = null,
                        String?                                        ETag                          = null,

                        CommonAPI?                                     CommonAPI                     = null,
                        CustomJObjectSerializerDelegate<Terminal>?     CustomTerminalSerializer      = null,
                        CustomJObjectSerializerDelegate<DisplayText>?  CustomDisplayTextSerializer   = null,
                        CustomJObjectSerializerDelegate<Image>?        CustomImageSerializer         = null)

        {

            this.Id                 = Id;
            this.CustomerReference  = CustomerReference;
            this.CountryCode        = CountryCode;
            this.PartyId            = PartyId;
            this.Address            = Address;
            this.City               = City;
            this.PostalCode         = PostalCode;
            this.State              = State;
            this.Country            = Country;
            this.FloorLevel         = FloorLevel;
            this.PhysicalReference  = PhysicalReference;
            this.Coordinates        = Coordinates;
            this.Directions         = Directions?.    Distinct() ?? [];
            this.Images             = Images?.        Distinct() ?? [];
            this.Capabilities       = Capabilities?.  Distinct() ?? [];
            this.PaymentMethods     = PaymentMethods?.Distinct() ?? [];
            this.PaymentBrands      = PaymentBrands?. Distinct() ?? [];
            this.InvoiceBaseURL     = InvoiceBaseURL;
            this.InvoiceCreator     = InvoiceCreator;
            this.Reference          = Reference;
            this.LocationIds        = LocationIds?.   Distinct() ?? [];
            this.EVSEUIds           = EVSEUIds?.      Distinct() ?? [];

            var created             = Created     ?? LastUpdated ?? Timestamp.Now;
            this.Created            = created;
            this.LastUpdated        = LastUpdated ?? created;

            this.CommonAPI          = CommonAPI;

            this.ETag               = ETag ?? CalcSHA256Hash(
                                                  CustomTerminalSerializer,
                                                  CustomDisplayTextSerializer,
                                                  CustomImageSerializer
                                              );

            unchecked
            {

                hashCode = this.Id.                GetHashCode()       * 89 ^
                          (this.CustomerReference?.GetHashCode() ?? 0) * 83 ^
                          (this.CountryCode?.      GetHashCode() ?? 0) * 79 ^
                          (this.PartyId?.          GetHashCode() ?? 0) * 73 ^
                          (this.Address?.          GetHashCode() ?? 0) * 71 ^
                          (this.City?.             GetHashCode() ?? 0) * 67 ^
                          (this.PostalCode?.       GetHashCode() ?? 0) * 61 ^
                          (this.State?.            GetHashCode() ?? 0) * 59 ^
                          (this.Country?.          GetHashCode() ?? 0) * 53 ^
                          (this.FloorLevel?.       GetHashCode() ?? 0) * 47 ^
                          (this.PhysicalReference?.GetHashCode() ?? 0) * 43 ^
                          (this.Coordinates?.      GetHashCode() ?? 0) * 41 ^
                           this.Directions.        CalcHashCode()      * 37 ^
                           this.Images.            CalcHashCode()      * 31 ^
                           this.Capabilities.      CalcHashCode()      * 29 ^
                           this.PaymentMethods.    CalcHashCode()      * 23 ^
                           this.PaymentBrands.     CalcHashCode()      * 19 ^
                          (this.InvoiceBaseURL?.   GetHashCode() ?? 0) * 17 ^
                          (this.InvoiceCreator?.   GetHashCode() ?? 0) * 13 ^
                          (this.Reference?.        GetHashCode() ?? 0) * 11 ^
                           this.LocationIds.       CalcHashCode()      *  7 ^
                           this.EVSEUIds.          CalcHashCode()      *  5 ^

                           this.Created.           GetHashCode()       *  3 ^
                           this.LastUpdated.       GetHashCode();

            }

        }

        #endregion


        #region (static) Parse   (JSON, TerminalIdURL = null, CustomTerminalParser = null)

        /// <summary>
        /// Parse the given JSON representation of a Terminal.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="TerminalIdURL">An optional Terminal identification, e.g. from the HTTP URL.</param>
        /// <param name="CustomTerminalParser">A delegate to parse custom Terminal JSON objects.</param>
        public static Terminal Parse(JObject                                 JSON,
                                     Terminal_Id?                            TerminalIdURL          = null,
                                     CustomJObjectParserDelegate<Terminal>?  CustomTerminalParser   = null)
        {

            if (TryParse(JSON,
                         out var terminal,
                         out var errorResponse,
                         TerminalIdURL,
                         false,
                         CustomTerminalParser))
            {
                return terminal;
            }

            throw new ArgumentException("The given JSON representation of a Terminal is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out Terminal, out ErrorResponse, TerminalUIdURL = null, CustomTerminalParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a Terminal.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="Terminal">The parsed Terminal.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                             JSON,
                                       [NotNullWhen(true)]  out Terminal?  Terminal,
                                       [NotNullWhen(false)] out String?    ErrorResponse)

            => TryParse(JSON,
                        out Terminal,
                        out ErrorResponse,
                        null,
                        false,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a Terminal.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="Terminal">The parsed Terminal.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="TerminalIdURL">An optional Terminal identification, e.g. from the HTTP URL.</param>
        /// <param name="CustomTerminalParser">A delegate to parse custom Terminal JSON objects.</param>
        public static Boolean TryParse(JObject                                 JSON,
                                       [NotNullWhen(true)]  out Terminal?      Terminal,
                                       [NotNullWhen(false)] out String?        ErrorResponse,
                                       Terminal_Id?                            TerminalIdURL               = null,
                                       Boolean                                 GenerateMissingTerminalId   = false,
                                       CustomJObjectParserDelegate<Terminal>?  CustomTerminalParser        = null)
        {

            try
            {

                Terminal = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse TerminalId           [mandatory]

                if (JSON.ParseOptional("terminal_id",
                                       "internal Terminal identification",
                                       Terminal_Id.TryParse,
                                       out Terminal_Id? terminalIdBody,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                if (!TerminalIdURL.HasValue && !terminalIdBody.HasValue)
                {
                    ErrorResponse = "The Terminal identification is missing!";
                    return false;
                }

                if (TerminalIdURL.HasValue && terminalIdBody.HasValue && TerminalIdURL.Value != terminalIdBody.Value)
                {
                    ErrorResponse = "The optional Terminal identification given within the JSON body does not match the one given in the URL!";
                    return false;
                }

                if (!terminalIdBody.HasValue && GenerateMissingTerminalId)
                    terminalIdBody = Terminal_Id.Random();

                #endregion

                #region Parse CustomerReference    [optional]

                if (JSON.ParseOptional("customer_reference",
                                       "customer reference",
                                       Customer_Reference.TryParse,
                                       out Customer_Reference? customerReference,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse CountryCode          [optional]

                if (JSON.ParseOptional("country_code",
                                       "country code",
                                       OCPI.CountryCode.TryParse,
                                       out CountryCode? countryCode,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse PartyId              [optional]

                if (JSON.ParseOptional("party_id",
                                       "party identification",
                                       Party_Id.TryParse,
                                       out Party_Id? partyId,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse Address              [optional]

                var address = JSON.GetString("address");

                #endregion

                #region Parse City                 [optional]

                var city = JSON.GetString("city");

                #endregion

                #region Parse PostalCode           [optional]

                var postalCode = JSON.GetString("postal_code");

                #endregion

                #region Parse State                [optional]

                var state = JSON.GetString("state");

                #endregion

                #region Parse Country              [optional]

                if (JSON.ParseOptional("country",
                                       "country",
                                       Country.TryParse,
                                       out Country? country,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse FloorLevel           [optional, VendorExtension]

                var floorLevel = JSON.GetString("floor_level");

                #endregion

                #region Parse PhysicalReference    [optional, VendorExtension]

                var physicalReference = JSON.GetString("physical_reference");

                #endregion

                #region Parse Coordinates          [optional]

                if (JSON.ParseOptionalJSON("coordinates",
                                           "geo coordinates",
                                           GeoCoordinate.TryParse,
                                           out GeoCoordinate? coordinates,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse Directions           [optional, VendorExtension]

                if (JSON.ParseOptionalHashSet("directions",
                                              "multi-language directions",
                                              DisplayText.TryParse,
                                              out HashSet<DisplayText> directions,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse Images               [optional, VendorExtension]

                if (JSON.ParseOptionalJSON("images",
                                           "images",
                                           Image.TryParse,
                                           out IEnumerable<Image> images,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse Capabilities         [optional, VendorExtension]

                if (JSON.ParseOptionalHashSet("capabilities",
                                              "capabilities",
                                              TerminalCapability.TryParse,
                                              out HashSet<TerminalCapability> capabilities,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse PaymentMethods       [optional, VendorExtension]

                if (JSON.ParseOptionalHashSet("payment_methods",
                                              "payment methods",
                                              PaymentMethod.TryParse,
                                              out HashSet<PaymentMethod> paymentMethods,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse PaymentBrands        [optional, VendorExtension]

                if (JSON.ParseOptionalHashSet("payment_brands",
                                              "payment brands",
                                              PaymentBrand.TryParse,
                                              out HashSet<PaymentBrand> paymentBrands,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse InvoiceBaseURL       [optional]

                if (JSON.ParseOptional("invoice_base_url",
                                       "invoice base URL",
                                       URL.TryParse,
                                       out URL? invoiceBaseURL,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse InvoiceCreator       [optional]

                if (JSON.ParseOptional("invoice_creator",
                                       "invoice creator",
                                       Invoice_Creator.TryParse,
                                       out Invoice_Creator? invoiceCreator,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse Reference            [optional]

                var reference = JSON.GetString("reference");

                #endregion

                #region Parse LocationIds          [optional]

                if (JSON.ParseOptionalHashSet("location_ids",
                                              "location identifications",
                                              Location_Id.TryParse,
                                              out HashSet<Location_Id> locationIds,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse EVSEUIds             [optional]

                if (JSON.ParseOptionalHashSet("evse_uids",
                                              "EVSE unique identifications",
                                              EVSE_UId.TryParse,
                                              out HashSet<EVSE_UId> evseUIds,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                #region Parse Created              [optional, VendorExtension]

                if (JSON.ParseOptional("created",
                                       "created",
                                       out DateTime? Created,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse LastUpdated          [mandatory]

                if (!JSON.ParseMandatory("last_updated",
                                         "last updated",
                                         out DateTime LastUpdated,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                Terminal = new Terminal(

                               terminalIdBody ?? TerminalIdURL!.Value,
                               customerReference,
                               countryCode,
                               partyId,
                               address,
                               city,
                               postalCode,
                               state,
                               country,
                               floorLevel,
                               physicalReference,
                               coordinates,
                               directions,
                               images,
                               capabilities,
                               paymentMethods,
                               paymentBrands,
                               invoiceBaseURL,
                               invoiceCreator,
                               reference,
                               locationIds,
                               evseUIds,

                               Created,
                               LastUpdated

                           );


                if (CustomTerminalParser is not null)
                    Terminal = CustomTerminalParser(JSON,
                                                    Terminal);

                return true;

            }
            catch (Exception e)
            {
                Terminal       = default;
                ErrorResponse  = "The given JSON representation of a Terminal is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomTerminalSerializer = null, CustomDisplayTextSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="IncludeCreatedTimestamp">Whether to include the created timestamp in the JSON representation.</param>
        /// <param name="IncludeExtensions">Whether to include optional data model extensions.</param>
        /// <param name="CustomTerminalSerializer">A delegate to serialize custom Terminal JSON objects.</param>
        /// <param name="CustomDisplayTextSerializer">A delegate to serialize custom multi-language text JSON objects.</param>
        /// <param name="CustomImageSerializer">A delegate to serialize custom image JSON objects.</param>
        public JObject ToJSON(Boolean                                        IncludeCreatedTimestamp       = true,
                              Boolean                                        IncludeExtensions             = true,
                              CustomJObjectSerializerDelegate<Terminal>?     CustomTerminalSerializer      = null,
                              CustomJObjectSerializerDelegate<DisplayText>?  CustomDisplayTextSerializer   = null,
                              CustomJObjectSerializerDelegate<Image>?        CustomImageSerializer         = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("terminal_id",          Id.                     ToString()),

                           CustomerReference.HasValue
                               ? new JProperty("customer_reference",   CustomerReference.Value.ToString())
                               : null,

                           CountryCode.HasValue
                               ? new JProperty("country_code",         CountryCode.      Value.ToString())
                               : null,

                           PartyId.HasValue
                               ? new JProperty("party_id",             PartyId.          Value.ToString())
                               : null,

                           Address.IsNotNullOrEmpty()
                               ? new JProperty("address",              Address)
                               : null,

                           City.IsNotNullOrEmpty()
                               ? new JProperty("city",                 City)
                               : null,

                           PostalCode.IsNotNullOrEmpty()
                               ? new JProperty("postal_code",          PostalCode)
                               : null,

                           State.IsNotNullOrEmpty()
                               ? new JProperty("state",                State)
                               : null,

                           Country is not null
                               ? new JProperty("country",              Country.Alpha3Code)
                               : null,

                           FloorLevel.IsNotNullOrEmpty()
                               ? new JProperty("floor_level",          FloorLevel)
                               : null,

                           PhysicalReference.IsNotNullOrEmpty()
                               ? new JProperty("physical_reference",   PhysicalReference)
                               : null,

                           Directions.Any()
                               ? new JProperty("directions",           new JArray(Directions.    Select(displayText        => displayText.       ToJSON(CustomDisplayTextSerializer))))
                               : null,

                           Images.Any()
                               ? new JProperty("images",               new JArray(Images.        Select(image              => image.             ToJSON(CustomImageSerializer))))
                               : null,

                           Capabilities.Any()
                               ? new JProperty("capabilities",         new JArray(Capabilities.  Select(terminalCapability => terminalCapability.ToString())))
                               : null,

                           PaymentMethods.Any()
                               ? new JProperty("payment_methods",      new JArray(PaymentMethods.Select(paymentMethod      => paymentMethod.     ToString())))
                               : null,

                           PaymentBrands.Any()
                               ? new JProperty("payment_brands",       new JArray(PaymentBrands. Select(paymentBrand       => paymentBrand.      ToString())))
                               : null,

                           InvoiceBaseURL.HasValue
                               ? new JProperty("invoice_base_url",     InvoiceBaseURL.   Value.ToString())
                               : null,

                           InvoiceCreator.HasValue
                               ? new JProperty("invoice_creator",      InvoiceCreator.   Value.ToString())
                               : null,

                           Reference.IsNotNullOrEmpty()
                               ? new JProperty("reference",            Reference)
                               : null,

                           LocationIds.Any()
                               ? new JProperty("location_ids",         new JArray(LocationIds.   Select(locationId         => locationId.        ToString())))
                               : null,

                           EVSEUIds.Any()
                               ? new JProperty("evse_uids",            new JArray(EVSEUIds.      Select(evseUId            => evseUId.           ToString())))
                               : null,


                           IncludeCreatedTimestamp
                               ? new JProperty("created",              Created.                ToISO8601())
                               : null,

                                 new JProperty("last_updated",         LastUpdated.            ToISO8601())

                       );

            return CustomTerminalSerializer is not null
                       ? CustomTerminalSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this terminal.
        /// </summary>
        public Terminal Clone()

            => new (

                   Id.                  Clone(),
                   CustomerReference?.  Clone(),
                   CountryCode?.        Clone(),
                   PartyId?.            Clone(),
                   Address?.            CloneString(),
                   City?.               CloneString(),
                   PostalCode?.         CloneString(),
                   State?.              CloneString(),
                   Country?.            Clone(),
                   FloorLevel?.         CloneString(),
                   PhysicalReference?.  CloneString(),
                   Coordinates?.        Clone(),
                   Directions.          Select(displayText   => displayText.  Clone()),
                   Images.              Select(image         => image.        Clone()),
                   Capabilities.        Select(capability    => capability.   Clone()),
                   PaymentMethods.      Select(paymentMethod => paymentMethod.Clone()),
                   PaymentBrands.       Select(paymentBrand  => paymentBrand. Clone()),
                   InvoiceBaseURL?.     Clone(),
                   InvoiceCreator?.     Clone(),
                   Reference?.          CloneString(),
                   LocationIds.         Select(locationId    => locationId.   Clone()),
                   EVSEUIds.            Select(evseUId       => evseUId.      Clone()),

                   Created,
                   LastUpdated,
                   ETag.                CloneString(),

                   CommonAPI

               );

        #endregion


        #region (private) TryPrivatePatch(JSON, Patch)

        private PatchResult<JObject> TryPrivatePatch(JObject  JSON,
                                                     JObject  Patch,
                                                     EventTracking_Id  EventTrackingId)
        {

            foreach (var property in Patch)
            {

                if (property.Key == "uid")
                    return PatchResult<JObject>.Failed(EventTrackingId, JSON,
                                                       "Patching the 'unique identification' of a Terminal is not allowed!");

                else if (property.Key == "connectors")
                    return PatchResult<JObject>.Failed(EventTrackingId, JSON,
                                                       "Patching the 'connectors' array of a Terminal is not allowed!");
                //{

                //    if (property.Value is null)
                //        return PatchResult<JObject>.Failed(JSON,
                //                                           "Patching the 'connectors' array of a location to 'null' is not allowed!");

                //    else if (property.Value is JArray ConnectorsArray)
                //    {

                //        if (ConnectorsArray.Count == 0)
                //            return PatchResult<JObject>.Failed(JSON,
                //                                               "Patching the 'connectors' array of a location to '[]' is not allowed!");

                //        else
                //        {
                //            foreach (var connector in ConnectorsArray)
                //            {

                //                //ToDo: What to do with multiple Terminal objects having the same TerminalUId?
                //                if (connector is JObject ConnectorObject)
                //                {

                //                    if (ConnectorObject.ParseMandatory("id",
                //                                                       "connector identification",
                //                                                       Connector_Id.TryParse,
                //                                                       out Connector_Id  ConnectorId,
                //                                                       out String        ErrorResponse))
                //                    {

                //                        return PatchResult<JObject>.Failed(JSON,
                //                                                           "Patching the 'connectors' array of a location led to an error: " + ErrorResponse);

                //                    }

                //                    if (TryGetConnector(ConnectorId, out Connector Connector))
                //                    {
                //                        //Connector.Patch(ConnectorObject);
                //                    }
                //                    else
                //                    {

                //                        //ToDo: Create this "new" Connector!
                //                        return PatchResult<JObject>.Failed(JSON,
                //                                                           "Unknown connector identification!");

                //                    }

                //                }
                //                else
                //                {
                //                    return PatchResult<JObject>.Failed(JSON,
                //                                                       "Invalid JSON merge patch for 'connectors' array of a location: Data within the 'connectors' array is not a valid connector object!");
                //                }

                //            }
                //        }
                //    }

                //    else
                //    {
                //        return PatchResult<JObject>.Failed(JSON,
                //                                           "Invalid JSON merge patch for 'connectors' array of a location: JSON property 'connectors' is not an array!");
                //    }

                //}

                else if (property.Value is null)
                    JSON.Remove(property.Key);

                else if (property.Value is JObject subObject)
                {

                    if (JSON.ContainsKey(property.Key))
                    {

                        if (JSON[property.Key] is JObject oldSubObject)
                        {

                            //ToDo: Perhaps use a more generic JSON patch here!
                            // PatchObject.Apply(ToJSON(), TerminalPatch),
                            var patchResult = TryPrivatePatch(oldSubObject, subObject, EventTrackingId);

                            if (patchResult.IsSuccess)
                                JSON[property.Key] = patchResult.PatchedData;

                        }

                        else
                            JSON[property.Key] = subObject;

                    }

                    else
                        JSON.Add(property.Key, subObject);

                }

                //else if (property.Value is JArray subArray)
                //{
                //}

                else
                    JSON[property.Key] = property.Value;

            }

            return PatchResult<JObject>.Success(EventTrackingId, JSON);

        }

        #endregion

        #region TryPatch(TerminalPatch, AllowDowngrades = false)

        /// <summary>
        /// Try to patch the JSON representation of this Terminal.
        /// </summary>
        /// <param name="TerminalPatch">The JSON merge patch.</param>
        /// <param name="AllowDowngrades">Allow to set the 'lastUpdated' timestamp to an earlier value.</param>
        public PatchResult<Terminal> TryPatch(JObject            TerminalPatch,
                                              Boolean            AllowDowngrades   = false,
                                              EventTracking_Id?  EventTrackingId   = null)
        {

            EventTrackingId ??= EventTracking_Id.New;

            if (TerminalPatch is null)
                return PatchResult<Terminal>.Failed(EventTrackingId, this,
                                                "The given Terminal patch must not be null!");

            lock (patchLock)
            {

                if (TerminalPatch["last_updated"] is null)
                    TerminalPatch["last_updated"] = Timestamp.Now.ToISO8601();

                else if (AllowDowngrades == false &&
                        TerminalPatch["last_updated"].Type == JTokenType.Date &&
                       (TerminalPatch["last_updated"].Value<DateTime>().ToISO8601().CompareTo(LastUpdated.ToISO8601()) < 1))
                {
                    return PatchResult<Terminal>.Failed(EventTrackingId, this,
                                                    "The 'lastUpdated' timestamp of the Terminal patch must be newer then the timestamp of the existing Terminal!");
                }


                var patchResult = TryPrivatePatch(ToJSON(), TerminalPatch, EventTrackingId);


                if (patchResult.IsFailed)
                    return PatchResult<Terminal>.Failed(EventTrackingId, this,
                                                    patchResult.ErrorResponse);

                if (TryParse(patchResult.PatchedData,
                             out var patchedTerminal,
                             out var errorResponse))
                {

                    return PatchResult<Terminal>.Success(EventTrackingId, patchedTerminal,
                                                     errorResponse);

                }

                else
                    return PatchResult<Terminal>.Failed(EventTrackingId, this,
                                                    "Invalid JSON merge patch of a Terminal: " + errorResponse);

            }

        }

        #endregion


        #region CalcSHA256Hash(CustomTerminalSerializer = null, CustomDisplayTextSerializer = null, ...)

        /// <summary>
        /// Calculate the SHA256 hash of the JSON representation of this location in HEX.
        /// </summary>
        /// <param name="CustomTerminalSerializer">A delegate to serialize custom Terminal JSON objects.</param>
        /// <param name="CustomDisplayTextSerializer">A delegate to serialize custom multi-language text JSON objects.</param>
        /// <param name="CustomImageSerializer">A delegate to serialize custom image JSON objects.</param>
        public String CalcSHA256Hash(CustomJObjectSerializerDelegate<Terminal>?     CustomTerminalSerializer      = null,
                                     CustomJObjectSerializerDelegate<DisplayText>?  CustomDisplayTextSerializer   = null,
                                     CustomJObjectSerializerDelegate<Image>?        CustomImageSerializer         = null)
        {

            ETag = SHA256.HashData(
                       ToJSON(
                           true,
                           true,
                           CustomTerminalSerializer,
                           CustomDisplayTextSerializer,
                           CustomImageSerializer
                       ).ToUTF8Bytes()
                   ).ToBase64();

            return ETag;

        }

        #endregion


        #region Operator overloading

        #region Operator == (Terminal1, Terminal2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Terminal1">A Terminal.</param>
        /// <param name="Terminal2">Another Terminal.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Terminal? Terminal1,
                                           Terminal? Terminal2)
        {

            if (Object.ReferenceEquals(Terminal1, Terminal2))
                return true;

            if (Terminal1 is null || Terminal2 is null)
                return false;

            return Terminal1.Equals(Terminal2);

        }

        #endregion

        #region Operator != (Terminal1, Terminal2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Terminal1">A Terminal.</param>
        /// <param name="Terminal2">Another Terminal.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Terminal? Terminal1,
                                           Terminal? Terminal2)

            => !(Terminal1 == Terminal2);

        #endregion

        #region Operator <  (Terminal1, Terminal2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Terminal1">A Terminal.</param>
        /// <param name="Terminal2">Another Terminal.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Terminal? Terminal1,
                                          Terminal? Terminal2)

            => Terminal1 is null
                   ? throw new ArgumentNullException(nameof(Terminal1), "The given Terminal must not be null!")
                   : Terminal1.CompareTo(Terminal2) < 0;

        #endregion

        #region Operator <= (Terminal1, Terminal2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Terminal1">A Terminal.</param>
        /// <param name="Terminal2">Another Terminal.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Terminal? Terminal1,
                                           Terminal? Terminal2)

            => !(Terminal1 > Terminal2);

        #endregion

        #region Operator >  (Terminal1, Terminal2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Terminal1">A Terminal.</param>
        /// <param name="Terminal2">Another Terminal.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Terminal? Terminal1,
                                          Terminal? Terminal2)

            => Terminal1 is null
                   ? throw new ArgumentNullException(nameof(Terminal1), "The given Terminal must not be null!")
                   : Terminal1.CompareTo(Terminal2) > 0;

        #endregion

        #region Operator >= (Terminal1, Terminal2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Terminal1">A Terminal.</param>
        /// <param name="Terminal2">Another Terminal.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Terminal? Terminal1,
                                           Terminal? Terminal2)

            => !(Terminal1 < Terminal2);

        #endregion

        #endregion

        #region IComparable<Terminal> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two Terminals.
        /// </summary>
        /// <param name="Object">A Terminal to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Terminal terminal
                   ? CompareTo(terminal)
                   : throw new ArgumentException("The given object is not a Terminal!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(Terminal)

        /// <summary>
        /// Compares two Terminals.
        /// </summary>
        /// <param name="Terminal">A Terminal to compare with.</param>
        public Int32 CompareTo(Terminal? Terminal)
        {

            if (Terminal is null)
                throw new ArgumentNullException(nameof(Terminal), "The given Terminal must not be null!");

            var c = Id.                     CompareTo(Terminal.Id);

            if (c == 0)
                c = LastUpdated.ToISO8601().CompareTo(Terminal.LastUpdated.ToISO8601());

            if (c == 0)
                c = ETag.                   CompareTo(Terminal.ETag);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<Terminal> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two Terminals for equality.
        /// </summary>
        /// <param name="Object">A Terminal to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Terminal terminal &&
                   Equals(terminal);

        #endregion

        #region Equals(Terminal)

        /// <summary>
        /// Compares two Terminals for equality.
        /// </summary>
        /// <param name="Terminal">A Terminal to compare with.</param>
        public Boolean Equals(Terminal? Terminal)

            => Terminal is not null &&

               Id.                     Equals(Terminal.Id)                      &&
               LastUpdated.ToISO8601().Equals(Terminal.LastUpdated.ToISO8601()) &&

            ((!Coordinates.      HasValue    && !Terminal.Coordinates.      HasValue)    ||
              (Coordinates.      HasValue    &&  Terminal.Coordinates.      HasValue    && Coordinates.Value.Equals(Terminal.Coordinates.Value))) &&

             ((FloorLevel        is     null &&  Terminal.FloorLevel        is     null) ||
              (FloorLevel        is not null &&  Terminal.FloorLevel        is not null && FloorLevel.       Equals(Terminal.FloorLevel)))        &&

             ((PhysicalReference is     null &&  Terminal.PhysicalReference is     null) ||
              (PhysicalReference is not null &&  Terminal.PhysicalReference is not null && PhysicalReference.Equals(Terminal.PhysicalReference))) &&

               Capabilities.            Count().Equals(Terminal.Capabilities.            Count()) &&
               Directions.              Count().Equals(Terminal.Directions.              Count()) &&
               Images.                  Count().Equals(Terminal.Images.                  Count()) &&

               Capabilities.            All(capabilityType          => Terminal.Capabilities.            Contains(capabilityType))     &&
               Directions.              All(displayText             => Terminal.Directions.              Contains(displayText))        &&
               Images.                  All(image                   => Terminal.Images.                  Contains(image));

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

                   Id,

                   LastUpdated.ToISO8601()

               );

        #endregion


        #region ToBuilder(NewId = null)

        /// <summary>
        /// Return a builder for this Terminal.
        /// </summary>
        /// <param name="NewId">An optional new Terminal identification.</param>
        public Builder ToBuilder(Terminal_Id? NewId = null)

            => new (

                   Id,
                   CustomerReference,
                   CountryCode,
                   PartyId,
                   Address,
                   City,
                   PostalCode,
                   State,
                   Country,
                   FloorLevel,
                   PhysicalReference,
                   Coordinates,
                   Directions,
                   Images,
                   Capabilities,
                   PaymentMethods,
                   PaymentBrands,
                   InvoiceBaseURL,
                   InvoiceCreator,
                   Reference,
                   LocationIds,
                   EVSEUIds,

                   Created,
                   LastUpdated,

                   CommonAPI

               );

        #endregion

        #region (class) Builder

        /// <summary>
        /// A Terminal builder.
        /// </summary>
        public class Builder
        {

            #region Properties

            /// <summary>
            /// The parent CommonAPI of the terminal.
            /// </summary>
            internal CommonAPI?                       CommonAPI            { get; set; }

            /// <summary>
            /// The unique identification of the terminal.
            /// </summary>
            [Mandatory]
            public   Terminal_Id?                     Id                   { get; set; }

            /// <summary>
            /// This optional reference will be used to link the terminal to a CSMS.
            /// The reference might also be provided via the order process.
            /// </summary>
            [Optional]
            public   Customer_Reference?              CustomerReference    { get; set; }

            /// <summary>
            /// The optional country code as an alternative to the customer reference.
            /// </summary>
            [Optional]
            public   CountryCode?                     CountryCode          { get; set; }

            /// <summary>
            /// The optional party identification as an alternative to the customer reference.
            /// </summary>
            [Optional]
            public   Party_Id?                        PartyId              { get; set; }

            /// <summary>
            /// The optional street/block name and house number of the terminal.
            /// </summary>
            [Optional]
            public   String?                          Address              { get; set; }

            /// <summary>
            /// The optional city or town of the terminal.
            /// </summary>
            [Optional]
            public   String?                          City                 { get; set; }

            /// <summary>
            /// The optional postal code of the terminal.
            /// </summary>
            [Optional]
            public   String?                          PostalCode           { get; set; }

            /// <summary>
            /// The optional state or province of the terminal.
            /// </summary>
            [Optional]
            public   String?                          State                { get; set; }

            /// <summary>
            /// The country of the terminal.
            /// </summary>
            [Optional]
            public   Country?                         Country              { get; set; }

            /// <summary>
            /// The optional floor level on which the terminal is located (in garage buildings)
            /// in the locally displayed numbering scheme.
            /// </summary>
            [Optional, VendorExtension(VE.GraphDefined)]
            public   String?                          FloorLevel           { get; set; }

            /// <summary>
            /// The optional number/string printed on the outside of the terminal for visual identification.
            /// </summary>
            [Optional, VendorExtension(VE.GraphDefined)]
            public   String?                          PhysicalReference    { get; set; }

            /// <summary>
            /// The optional geographical location of the terminal.
            /// </summary>
            [Optional]
            public   GeoCoordinate?                   Coordinates          { get; set; }

            /// <summary>
            /// The optional multi-language human-readable directions when more detailed
            /// information on how to reach the terminal.
            /// </summary>
            [Optional, VendorExtension(VE.GraphDefined)]
            public   HashSet<DisplayText>             Directions           { get; }

            /// <summary>
            /// Optional enumeration of images related to the terminal such as photos or logos.
            /// </summary>
            [Optional]
            public   HashSet<Image>                   Images               { get; }

            /// <summary>
            /// Optional enumeration of all functionalities of the terminal.
            /// </summary>
            [Optional, VendorExtension(VE.GraphDefined)]
            public HashSet<TerminalCapability>        Capabilities         { get; }

            /// <summary>
            /// Optional enumeration of all supported payment methods.
            /// </summary>
            [Optional, VendorExtension(VE.GraphDefined)]
            public   HashSet<PaymentMethod>           PaymentMethods       { get; }

            /// <summary>
            /// Optional enumeration of all supported payment brands.
            /// </summary>
            [Optional, VendorExtension(VE.GraphDefined)]
            public   HashSet<PaymentBrand>            PaymentBrands        { get; }

            /// <summary>
            /// The optional BaseURL for downloading invoices.
            /// </summary>
            [Optional]
            public   URL?                             InvoiceBaseURL       { get; set; }

            /// <summary>
            /// Describes which party creates the invoice for the EV driver.
            /// </summary>
            [Optional]
            public   Invoice_Creator?                 InvoiceCreator       { get; set; }

            /// <summary>
            /// Mapping value as issued by the PTP (e.g serial number).
            /// </summary>
            [Optional]
            public   String?                          Reference            { get; set; }

            /// <summary>
            /// List of all locations assigned to that terminal.
            /// </summary>
            [Optional]
            public   HashSet<Location_Id>             LocationIds          { get; }

            /// <summary>
            /// List of all EVSEs assigned to that terminal.
            /// </summary>
            [Optional]
            public   HashSet<EVSE_UId>                EVSEUIds             { get; }


            /// <summary>
            /// The timestamp when this terminal was created.
            /// </summary>
            [Mandatory, VendorExtension(VE.GraphDefined, VE.Pagination)]
            public   DateTimeOffset                   Created              { get; set; }

            /// <summary>
            /// Timestamp when this terminal was last updated (or created).
            /// </summary>
            [Mandatory]
            public   DateTimeOffset                   LastUpdated          { get; set; }

            #endregion

            #region Constructor(s)

            /// <summary>
            /// Create a new Terminal builder.
            /// </summary>
            /// <param name="Id">The unique identification of the Terminal.</param>
            /// <param name="CustomerReference">This optional reference will be used to link the terminal to a CSMS. The reference might also be provided via the order process.</param>
            /// <param name="CountryCode">An optional country code as an alternative to the customer reference.</param>
            /// <param name="PartyId">An optional party identification as an alternative to the customer reference.</param>
            /// <param name="Address">An optional street/block name and house number of the terminal.</param>
            /// <param name="City">An optional city or town of the terminal.</param>
            /// <param name="PostalCode">An optional postal code of the terminal.</param>
            /// <param name="State">An optional state or province of the terminal.</param>
            /// <param name="Country">An optional country of the terminal.</param>
            /// <param name="FloorLevel">An optional floor level on which the terminal is located (in garage buildings) in the locally displayed numbering scheme.</param>
            /// <param name="PhysicalReference">An optional number/string printed on the outside of the terminal for visual identification.</param>
            /// <param name="Coordinates">An optional geographical location of the terminal.</param>
            /// <param name="Directions">An optional multi-language human-readable directions when more detailed information on how to reach the terminal.</param>
            /// <param name="Images">An optional enumeration of images related to the terminal such as photos or logos.</param>
            /// <param name="Capabilities">An optional enumeration of all functionalities of the terminal.</param>
            /// <param name="PaymentMethods">An optional enumeration of all supported payment methods.</param>
            /// <param name="PaymentBrands">An optional enumeration of all supported payment brands.</param>
            /// <param name="InvoiceBaseURL">An optional BaseURL for downloading invoices.</param>
            /// <param name="InvoiceCreator">An optional enumeration of all supported payment brands.</param>
            /// <param name="Reference">An optional mapping value as issued by the PTP (e.g serial number).</param>
            /// <param name="LocationIds">An optional list of all locations assigned to that terminal.</param>
            /// <param name="EVSEUIds">An optional list of all EVSEs assigned to that terminal.</param>
            /// 
            /// <param name="Created">An optional timestamp when this terminal was created.</param>
            /// <param name="LastUpdated">An optional timestamp when this terminal was last updated (or created).</param>
            /// 
            /// <param name="CommonAPI">An optional CommonAPI of this terminal.</param>
            internal Builder(Terminal_Id?                                   Id                            = null,
                             Customer_Reference?                            CustomerReference             = null,
                             CountryCode?                                   CountryCode                   = null,
                             Party_Id?                                      PartyId                       = null,
                             String?                                        Address                       = null,
                             String?                                        City                          = null,
                             String?                                        PostalCode                    = null,
                             String?                                        State                         = null,
                             Country?                                       Country                       = null,
                             String?                                        FloorLevel                    = null,
                             String?                                        PhysicalReference             = null,
                             GeoCoordinate?                                 Coordinates                   = null,
                             IEnumerable<DisplayText>?                      Directions                    = null,
                             IEnumerable<Image>?                            Images                        = null,
                             IEnumerable<TerminalCapability>?               Capabilities                  = null,
                             IEnumerable<PaymentMethod>?                    PaymentMethods                = null,
                             IEnumerable<PaymentBrand>?                     PaymentBrands                 = null,
                             URL?                                           InvoiceBaseURL                = null,
                             Invoice_Creator?                               InvoiceCreator                = null,
                             String?                                        Reference                     = null,
                             IEnumerable<Location_Id>?                      LocationIds                   = null,
                             IEnumerable<EVSE_UId>?                         EVSEUIds                      = null,

                             DateTimeOffset?                                Created                       = null,
                             DateTimeOffset?                                LastUpdated                   = null,

                             CommonAPI?                                     CommonAPI                     = null)

            {

                this.Id                 = Id;
                this.CustomerReference  = CustomerReference;
                this.CountryCode        = CountryCode;
                this.PartyId            = PartyId;
                this.Address            = Address;
                this.City               = City;
                this.PostalCode         = PostalCode;
                this.State              = State;
                this.Country            = Country;
                this.FloorLevel         = FloorLevel;
                this.PhysicalReference  = PhysicalReference;
                this.Coordinates        = Coordinates;
                this.Directions         = Directions     is not null ? [.. Directions]     : [];
                this.Images             = Images         is not null ? [.. Images]         : [];
                this.Capabilities       = Capabilities   is not null ? [.. Capabilities]   : [];
                this.PaymentMethods     = PaymentMethods is not null ? [.. PaymentMethods] : [];
                this.PaymentBrands      = PaymentBrands  is not null ? [.. PaymentBrands]  : [];
                this.InvoiceBaseURL     = InvoiceBaseURL;
                this.InvoiceCreator     = InvoiceCreator;
                this.Reference          = Reference;
                this.LocationIds        = LocationIds    is not null ? [.. LocationIds]    : [];
                this.EVSEUIds           = EVSEUIds       is not null ? [.. EVSEUIds]       : [];

                this.Created            = Created     ?? LastUpdated ?? Timestamp.Now;
                this.LastUpdated        = LastUpdated ?? Created     ?? Timestamp.Now;

                this.CommonAPI          = CommonAPI;

            }

            #endregion


            #region ToImmutable

            /// <summary>
            /// Return an immutable version of the Terminal.
            /// </summary>
            public static implicit operator Terminal?(Builder? Builder)

                => Builder?.ToImmutable(out _);


            /// <summary>
            /// Return an immutable version of the Terminal.
            /// </summary>
            /// <param name="Warnings"></param>
            public Terminal? ToImmutable(out IEnumerable<Warning> Warnings)
            {

                var warnings = new List<Warning>();

                if (!Id.   HasValue)
                    warnings.Add(Warning.Create("The identification must not be null or empty!"));

                Warnings = warnings;

                return warnings.Count != 0

                           ? null

                           : new Terminal(

                                 Id.   Value,
                                 CustomerReference,
                                 CountryCode,
                                 PartyId,
                                 Address,
                                 City,
                                 PostalCode,
                                 State,
                                 Country,
                                 FloorLevel,
                                 PhysicalReference,
                                 Coordinates,
                                 Directions,
                                 Images,
                                 Capabilities,
                                 PaymentMethods,
                                 PaymentBrands,
                                 InvoiceBaseURL,
                                 InvoiceCreator,
                                 Reference,
                                 LocationIds,
                                 EVSEUIds,

                                 Created,
                                 LastUpdated,
                                 null,

                                 CommonAPI

                             );

            }

            #endregion

        }

        #endregion


    }

}
