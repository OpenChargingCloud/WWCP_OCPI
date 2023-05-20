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

using System.Security.Cryptography;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;
using cloud.charging.open.protocols.OCPIv2_1_1.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_1_1
{

    /// <summary>
    /// The charging tariff consists of a list of one or more chrging tariff elements,
    /// these elements can be used to create complex tariff structures.
    /// When the list of elements contains more then 1 element, then the
    /// first tariff in the list with matching restrictions will be used.
    /// </summary>
    public class Tariff : IHasId<Tariff_Id>,
                          IEquatable<Tariff>,
                          IComparable<Tariff>,
                          IComparable
    {

        #region Data

        private readonly Object patchLock = new Object();

        #endregion

        #region Properties

        /// <summary>
        /// The parent CommonAPI of this charging tariff.
        /// </summary>
        internal CommonAPI?                  CommonAPI            { get; set; }

        /// <summary>
        /// The ISO-3166 alpha-2 country code of the charge point operator that 'owns' this session.
        /// </summary>
        [Optional]
        public   CountryCode                 CountryCode          { get; }

        /// <summary>
        /// The identification of the charge point operator that 'owns' this session (following the ISO-15118 standard).
        /// </summary>
        [Optional]
        public   Party_Id                    PartyId              { get; }

        /// <summary>
        /// The identification of the tariff within the CPOs platform (and suboperator platforms). 
        /// </summary>
        [Mandatory]
        public   Tariff_Id                   Id                   { get; }

        /// <summary>
        /// The ISO 4217 code of the currency used for this tariff.
        /// </summary>
        [Mandatory]
        public   OCPI.Currency               Currency             { get; }

        /// <summary>
        /// The optional multi-language alternative tariff info text.
        /// </summary>
        [Optional]
        public   IEnumerable<DisplayText>    TariffAltText        { get; }

        /// <summary>
        /// The optional URL to a web page that contains an explanation of the tariff information
        /// in human readable form.
        /// </summary>
        [Optional]
        public   URL?                        TariffAltURL         { get; }

        /// <summary>
        /// The enumeration of tariff elements.
        /// </summary>
        [Mandatory]
        public   IEnumerable<TariffElement>  TariffElements       { get; }

        /// <summary>
        /// Optional details on the energy supplied with this tariff.
        /// </summary>
        [Optional]
        public   EnergyMix?                  EnergyMix            { get;  }

        /// <summary>
        /// The timestamp when this tariff was last updated (or created).
        /// </summary>
        [Mandatory]
        public   DateTime                    LastUpdated          { get; }

        /// <summary>
        /// The SHA256 hash of the JSON representation of this charging tariff.
        /// </summary>
        public   String                      ETag                 { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new charging tariff.
        /// </summary>
        /// <param name="CountryCode">An ISO-3166 alpha-2 country code of the charge point operator that 'owns' this session.</param>
        /// <param name="PartyId">An identification of the charge point operator that 'owns' this session (following the ISO-15118 standard).</param>
        /// <param name="Id">An identification of the tariff within the CPOs platform (and suboperator platforms).</param>
        /// <param name="Currency">An ISO 4217 code of the currency used for this tariff.</param>
        /// <param name="TariffElements">An enumeration of tariff elements.</param>
        /// 
        /// <param name="TariffAltText">An optional multi-language alternative tariff info text.</param>
        /// <param name="TariffAltURL">An optional URL to a web page that contains an explanation of the tariff information in human readable form.</param>
        /// <param name="EnergyMix">Optional details on the energy supplied with this tariff.</param>
        /// 
        /// <param name="LastUpdated">A timestamp when this tariff was last updated (or created).</param>
        /// <param name="CustomDisplayTextSerializer">A delegate to serialize custom multi-language text JSON objects.</param>
        /// <param name="CustomTariffSerializer">A delegate to serialize custom tariff JSON objects.</param>
        /// <param name="CustomTariffElementSerializer">A delegate to serialize custom tariff element JSON objects.</param>
        /// <param name="CustomPriceComponentSerializer">A delegate to serialize custom price component JSON objects.</param>
        /// <param name="CustomTariffRestrictionsSerializer">A delegate to serialize custom tariff restrictions JSON objects.</param>
        /// <param name="CustomEnergyMixSerializer">A delegate to serialize custom hours JSON objects.</param>
        /// <param name="CustomEnergySourceSerializer">A delegate to serialize custom energy source JSON objects.</param>
        /// <param name="CustomEnvironmentalImpactSerializer">A delegate to serialize custom environmental impact JSON objects.</param>
        public Tariff(CountryCode                                            CountryCode,
                      Party_Id                                               PartyId,
                      Tariff_Id                                              Id ,
                      OCPI.Currency                                          Currency,
                      IEnumerable<TariffElement>                             TariffElements,

                      IEnumerable<DisplayText>?                              TariffAltText                         = null,
                      URL?                                                   TariffAltURL                          = null,
                      EnergyMix?                                             EnergyMix                             = null,

                      DateTime?                                              LastUpdated                           = null,
                      CustomJObjectSerializerDelegate<Tariff>?               CustomTariffSerializer                = null,
                      CustomJObjectSerializerDelegate<DisplayText>?          CustomDisplayTextSerializer           = null,
                      CustomJObjectSerializerDelegate<TariffElement>?        CustomTariffElementSerializer         = null,
                      CustomJObjectSerializerDelegate<PriceComponent>?       CustomPriceComponentSerializer        = null,
                      CustomJObjectSerializerDelegate<TariffRestrictions>?   CustomTariffRestrictionsSerializer    = null,
                      CustomJObjectSerializerDelegate<EnergyMix>?            CustomEnergyMixSerializer             = null,
                      CustomJObjectSerializerDelegate<EnergySource>?         CustomEnergySourceSerializer          = null,
                      CustomJObjectSerializerDelegate<EnvironmentalImpact>?  CustomEnvironmentalImpactSerializer   = null)

            : this(null,
                   CountryCode,
                   PartyId,
                   Id ,
                   Currency,
                   TariffElements,

                   TariffAltText,
                   TariffAltURL,
                   EnergyMix,

                   LastUpdated,
                   CustomTariffSerializer,
                   CustomDisplayTextSerializer,
                   CustomTariffElementSerializer,
                   CustomPriceComponentSerializer,
                   CustomTariffRestrictionsSerializer,
                   CustomEnergyMixSerializer,
                   CustomEnergySourceSerializer,
                   CustomEnvironmentalImpactSerializer)

        { }


        /// <summary>
        /// Create a new charging tariff.
        /// </summary>
        /// <param name="CountryCode">An ISO-3166 alpha-2 country code of the charge point operator that 'owns' this session.</param>
        /// <param name="PartyId">An identification of the charge point operator that 'owns' this session (following the ISO-15118 standard).</param>
        /// <param name="Id">An identification of the tariff within the CPOs platform (and suboperator platforms).</param>
        /// <param name="Currency">An ISO 4217 code of the currency used for this tariff.</param>
        /// <param name="TariffElements">An enumeration of tariff elements.</param>
        /// 
        /// <param name="TariffAltText">An optional multi-language alternative tariff info text.</param>
        /// <param name="TariffAltURL">An optional URL to a web page that contains an explanation of the tariff information in human readable form.</param>
        /// <param name="EnergyMix">Optional details on the energy supplied with this tariff.</param>
        /// 
        /// <param name="LastUpdated">A timestamp when this tariff was last updated (or created).</param>
        /// <param name="CustomDisplayTextSerializer">A delegate to serialize custom multi-language text JSON objects.</param>
        /// <param name="CustomTariffSerializer">A delegate to serialize custom tariff JSON objects.</param>
        /// <param name="CustomTariffElementSerializer">A delegate to serialize custom tariff element JSON objects.</param>
        /// <param name="CustomPriceComponentSerializer">A delegate to serialize custom price component JSON objects.</param>
        /// <param name="CustomTariffRestrictionsSerializer">A delegate to serialize custom tariff restrictions JSON objects.</param>
        /// <param name="CustomEnergyMixSerializer">A delegate to serialize custom hours JSON objects.</param>
        /// <param name="CustomEnergySourceSerializer">A delegate to serialize custom energy source JSON objects.</param>
        /// <param name="CustomEnvironmentalImpactSerializer">A delegate to serialize custom environmental impact JSON objects.</param>
        public Tariff(CommonAPI?                                             CommonAPI,
                      CountryCode                                            CountryCode,
                      Party_Id                                               PartyId,
                      Tariff_Id                                              Id ,
                      OCPI.Currency                                          Currency,
                      IEnumerable<TariffElement>                             TariffElements,

                      IEnumerable<DisplayText>?                              TariffAltText                         = null,
                      URL?                                                   TariffAltURL                          = null,
                      EnergyMix?                                             EnergyMix                             = null,

                      DateTime?                                              LastUpdated                           = null,
                      CustomJObjectSerializerDelegate<Tariff>?               CustomTariffSerializer                = null,
                      CustomJObjectSerializerDelegate<DisplayText>?          CustomDisplayTextSerializer           = null,
                      CustomJObjectSerializerDelegate<TariffElement>?        CustomTariffElementSerializer         = null,
                      CustomJObjectSerializerDelegate<PriceComponent>?       CustomPriceComponentSerializer        = null,
                      CustomJObjectSerializerDelegate<TariffRestrictions>?   CustomTariffRestrictionsSerializer    = null,
                      CustomJObjectSerializerDelegate<EnergyMix>?            CustomEnergyMixSerializer             = null,
                      CustomJObjectSerializerDelegate<EnergySource>?         CustomEnergySourceSerializer          = null,
                      CustomJObjectSerializerDelegate<EnvironmentalImpact>?  CustomEnvironmentalImpactSerializer   = null)

        {

            if (!TariffElements.Any())
                throw new ArgumentNullException(nameof(TariffElements),  "The given enumeration of tariff elements must not be null or empty!");

            this.CommonAPI       = CommonAPI;
            this.CountryCode     = CountryCode;
            this.PartyId         = PartyId;
            this.Id              = Id;
            this.Currency        = Currency;
            this.TariffElements  = TariffElements.Distinct();

            this.TariffAltText   = TariffAltText?.Distinct() ?? Array.Empty<DisplayText>();
            this.TariffAltURL    = TariffAltURL;
            this.EnergyMix       = EnergyMix;

            this.LastUpdated     = LastUpdated ?? Timestamp.Now;

            this.ETag            = SHA256.HashData(ToJSON(true,
                                                   CustomTariffSerializer,
                                                   CustomDisplayTextSerializer,
                                                   CustomTariffElementSerializer,
                                                   CustomPriceComponentSerializer,
                                                   CustomTariffRestrictionsSerializer,
                                                   CustomEnergyMixSerializer,
                                                   CustomEnergySourceSerializer,
                                                   CustomEnvironmentalImpactSerializer).ToUTF8Bytes()).ToBase64();

            unchecked
            {

                hashCode = this.CountryCode.   GetHashCode()        * 23 ^
                           this.PartyId.       GetHashCode()        * 19 ^
                           this.Id.            GetHashCode()        * 17 ^
                           this.Currency.      GetHashCode()        * 13 ^
                           this.TariffElements.CalcHashCode()       * 11 ^
                           this.LastUpdated.   GetHashCode()        *  7 ^
                           this.TariffAltText. CalcHashCode()       *  5 ^
                          (this.TariffAltURL?. GetHashCode()  ?? 0) *  3 ^
                           this.EnergyMix?.    GetHashCode()  ?? 0;

            }

        }

        #endregion


        #region (static) Parse   (JSON, CountryCodeURL = null, PartyIdURL = null, TariffIdURL = null, CustomTariffParser = null)

        /// <summary>
        /// Parse the given JSON representation of a charging tariff.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CountryCodeURL">An optional country code, e.g. from the HTTP URL.</param>
        /// <param name="PartyIdURL">An optional party identification, e.g. from the HTTP URL.</param>
        /// <param name="TariffIdURL">An optional tariff identification, e.g. from the HTTP URL.</param>
        /// <param name="CustomTariffParser">A delegate to parse custom tariff JSON objects.</param>
        public static Tariff Parse(JObject                               JSON,
                                   CountryCode?                          CountryCodeURL       = null,
                                   Party_Id?                             PartyIdURL           = null,
                                   Tariff_Id?                            TariffIdURL          = null,
                                   CustomJObjectParserDelegate<Tariff>?  CustomTariffParser   = null)
        {

            if (TryParse(JSON,
                         out var tariff,
                         out var errorResponse,
                         CountryCodeURL,
                         PartyIdURL, 
                         TariffIdURL,
                         CustomTariffParser))
            {
                return tariff!;
            }

            throw new ArgumentException("The given JSON representation of a charging tariff is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out Tariff, out ErrorResponse, TariffIdURL = null, CustomTariffParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a charging tariff.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="Tariff">The parsed tariff.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject      JSON,
                                       out Tariff?  Tariff,
                                       out String?  ErrorResponse)

            => TryParse(JSON,
                        out Tariff,
                        out ErrorResponse,
                        null,
                        null,
                        null,
                        null);

        /// <summary>
        /// Try to parse the given JSON representation of a charging tariff.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="Tariff">The parsed tariff.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CountryCodeURL">An optional country code, e.g. from the HTTP URL.</param>
        /// <param name="PartyIdURL">An optional party identification, e.g. from the HTTP URL.</param>
        /// <param name="TariffIdURL">An optional tariff identification, e.g. from the HTTP URL.</param>
        /// <param name="CustomTariffParser">A delegate to parse custom tariff JSON objects.</param>
        public static Boolean TryParse(JObject                               JSON,
                                       out Tariff?                           Tariff,
                                       out String?                           ErrorResponse,
                                       CountryCode?                          CountryCodeURL       = null,
                                       Party_Id?                             PartyIdURL           = null,
                                       Tariff_Id?                            TariffIdURL          = null,
                                       CustomJObjectParserDelegate<Tariff>?  CustomTariffParser   = null)
        {

            try
            {

                Tariff = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse CountryCode           [optional, internal]

                if (JSON.ParseOptional("country_code",
                                       "country code",
                                       CountryCode.TryParse,
                                       out CountryCode? CountryCodeBody,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                if (!CountryCodeURL.HasValue && !CountryCodeBody.HasValue)
                {
                    ErrorResponse = "The country code is missing!";
                    return false;
                }

                if (CountryCodeURL.HasValue && CountryCodeBody.HasValue && CountryCodeURL.Value != CountryCodeBody.Value)
                {
                    ErrorResponse = "The optional country code given within the JSON body does not match the one given in the URL!";
                    return false;
                }

                #endregion

                #region Parse PartyIdURL            [optional, internal]

                if (JSON.ParseOptional("party_id",
                                       "party identification",
                                       Party_Id.TryParse,
                                       out Party_Id? PartyIdBody,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                if (!PartyIdURL.HasValue && !PartyIdBody.HasValue)
                {
                    ErrorResponse = "The party identification is missing!";
                    return false;
                }

                if (PartyIdURL.HasValue && PartyIdBody.HasValue && PartyIdURL.Value != PartyIdBody.Value)
                {
                    ErrorResponse = "The optional party identification given within the JSON body does not match the one given in the URL!";
                    return false;
                }

                #endregion

                #region Parse Id                    [optional]

                if (JSON.ParseOptional("id",
                                       "tariff identification",
                                       Tariff_Id.TryParse,
                                       out Tariff_Id? TariffIdBody,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                if (!TariffIdURL.HasValue && !TariffIdBody.HasValue)
                {
                    ErrorResponse = "The tariff identification is missing!";
                    return false;
                }

                if (TariffIdURL.HasValue && TariffIdBody.HasValue && TariffIdURL.Value != TariffIdBody.Value)
                {
                    ErrorResponse = "The optional tariff identification given within the JSON body does not match the one given in the URL!";
                    return false;
                }

                #endregion

                #region Parse Currency              [mandatory]

                if (!JSON.ParseMandatory("currency",
                                         "currency",
                                         OCPI.Currency.TryParse,
                                         out OCPI.Currency Currency,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse TariffAltText         [optional]

                if (JSON.ParseOptionalJSON("tariff_alt_text",
                                           "tariff alternative text",
                                           DisplayText.TryParse,
                                           out IEnumerable<DisplayText> TariffAltText,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse TariffAltURL          [optional]

                if (JSON.ParseOptional("tariff_alt_url",
                                       "tariff alternative URL",
                                       URL.TryParse,
                                       out URL? TariffAltURL,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }


                #endregion

                #region Parse TariffElements        [mandatory]

                if (JSON.ParseMandatoryJSON("elements",
                                            "tariff elements",
                                            TariffElement.TryParse,
                                            out IEnumerable<TariffElement> TariffElements,
                                            out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse EnergyMix             [optional]

                if (JSON.ParseOptionalJSON("energy_mix",
                                           "energy mix",
                                           OCPIv2_1_1.EnergyMix.TryParse,
                                           out EnergyMix EnergyMix,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse LastUpdated           [mandatory]

                if (!JSON.ParseMandatory("last_updated",
                                         "last updated",
                                         out DateTime LastUpdated,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                Tariff = new Tariff(CountryCodeBody ?? CountryCodeURL!.Value,
                                    PartyIdBody     ?? PartyIdURL!.    Value,
                                    TariffIdBody    ?? TariffIdURL!.   Value,
                                    Currency,
                                    TariffElements,

                                    TariffAltText,
                                    TariffAltURL,
                                    EnergyMix,
                                    LastUpdated);

                if (CustomTariffParser is not null)
                    Tariff = CustomTariffParser(JSON,
                                                Tariff);

                return true;

            }
            catch (Exception e)
            {
                Tariff         = default;
                ErrorResponse  = "The given JSON representation of a charging tariff is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(IncludeOwnerInformation = false, CustomTariffSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="IncludeOwnerInformation">Include optional owner information.</param>
        /// <param name="CustomTariffSerializer">A delegate to serialize custom tariff JSON objects.</param>
        /// <param name="CustomDisplayTextSerializer">A delegate to serialize custom multi-language text JSON objects.</param>
        /// <param name="CustomTariffElementSerializer">A delegate to serialize custom tariff element JSON objects.</param>
        /// <param name="CustomPriceComponentSerializer">A delegate to serialize custom price component JSON objects.</param>
        /// <param name="CustomTariffRestrictionsSerializer">A delegate to serialize custom tariff restrictions JSON objects.</param>
        /// <param name="CustomEnergyMixSerializer">A delegate to serialize custom hours JSON objects.</param>
        /// <param name="CustomEnergySourceSerializer">A delegate to serialize custom energy source JSON objects.</param>
        /// <param name="CustomEnvironmentalImpactSerializer">A delegate to serialize custom environmental impact JSON objects.</param>
        public JObject ToJSON(Boolean                                                IncludeOwnerInformation               = false,
                              CustomJObjectSerializerDelegate<Tariff>?               CustomTariffSerializer                = null,
                              CustomJObjectSerializerDelegate<DisplayText>?          CustomDisplayTextSerializer           = null,
                              CustomJObjectSerializerDelegate<TariffElement>?        CustomTariffElementSerializer         = null,
                              CustomJObjectSerializerDelegate<PriceComponent>?       CustomPriceComponentSerializer        = null,
                              CustomJObjectSerializerDelegate<TariffRestrictions>?   CustomTariffRestrictionsSerializer    = null,
                              CustomJObjectSerializerDelegate<EnergyMix>?            CustomEnergyMixSerializer             = null,
                              CustomJObjectSerializerDelegate<EnergySource>?         CustomEnergySourceSerializer          = null,
                              CustomJObjectSerializerDelegate<EnvironmentalImpact>?  CustomEnvironmentalImpactSerializer   = null)
        {

            var JSON = JSONObject.Create(

                           IncludeOwnerInformation
                               ? new JProperty("country_code",      CountryCode. ToString())
                               : null,

                           IncludeOwnerInformation
                               ? new JProperty("party_id",          PartyId.     ToString())
                               : null,

                                 new JProperty("id",                Id.          ToString()),

                                 new JProperty("currency",          Currency.    ToString()),

                           TariffAltText.SafeAny()
                               ? new JProperty("tariff_alt_text",   new JArray(TariffAltText.Select(tariffAltText => tariffAltText.ToJSON(CustomDisplayTextSerializer))))
                               : null,

                           TariffAltURL.HasValue
                               ? new JProperty("tariff_alt_url",    TariffAltURL.ToString())
                               : null,

                           TariffElements.SafeAny()
                               ? new JProperty("elements",          new JArray(TariffElements.Select(tariffElement => tariffElement.ToJSON(CustomTariffElementSerializer,
                                                                                                                                           CustomPriceComponentSerializer,
                                                                                                                                           CustomTariffRestrictionsSerializer))))
                               : null,

                           EnergyMix is not null
                               ? new JProperty("energy_mix",        EnergyMix.   ToJSON(CustomEnergyMixSerializer,
                                                                                        CustomEnergySourceSerializer,
                                                                                        CustomEnvironmentalImpactSerializer))
                               : null,

                                 new JProperty("last_updated",      LastUpdated. ToIso8601())

                       );

            return CustomTariffSerializer is not null
                       ? CustomTariffSerializer(this, JSON)
                       : JSON;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this object.
        /// </summary>
        public Tariff Clone()

            => new (CountryCode.  Clone,
                    PartyId.      Clone,
                    Id.           Clone,
                    Currency.     Clone,
                    TariffElements.Select(tariffElement => tariffElement.Clone()).ToArray(),

                    TariffAltText. Select(displayText   => displayText.  Clone()).ToArray(),
                    TariffAltURL?.Clone,
                    EnergyMix?.   Clone(),

                    LastUpdated);

        #endregion


        #region (private) TryPrivatePatch(JSON, Patch)

        private PatchResult<JObject> TryPrivatePatch(JObject  JSON,
                                                     JObject  Patch)
        {

            foreach (var property in Patch)
            {

                if      (property.Key == "country_code")
                    return PatchResult<JObject>.Failed(JSON,
                                                       "Patching the 'country code' of a charging tariff is not allowed!");

                else if (property.Key == "party_id")
                    return PatchResult<JObject>.Failed(JSON,
                                                       "Patching the 'party identification' of a charging tariff is not allowed!");

                else if (property.Key == "id")
                    return PatchResult<JObject>.Failed(JSON,
                                                       "Patching the 'identification' of a charging tariff is not allowed!");

                else if (property.Value is null)
                    JSON.Remove(property.Key);

                else if (property.Value is JObject subObject)
                {

                    if (JSON.ContainsKey(property.Key))
                    {

                        if (JSON[property.Key] is JObject oldSubObject)
                        {

                            //ToDo: Perhaps use a more generic JSON patch here!
                            // PatchObject.Apply(ToJSON(), EVSEPatch),
                            var patchResult = TryPrivatePatch(oldSubObject, subObject);

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

            return PatchResult<JObject>.Success(JSON);

        }

        #endregion

        #region TryPatch(TariffPatch, AllowDowngrades = false)

        /// <summary>
        /// Try to patch the JSON representaion of this charging tariff.
        /// </summary>
        /// <param name="TariffPatch">The JSON merge patch.</param>
        /// <param name="AllowDowngrades">Allow to set the 'lastUpdated' timestamp to an earlier value.</param>
        public PatchResult<Tariff> TryPatch(JObject  TariffPatch,
                                            Boolean  AllowDowngrades = false)
        {

            if (TariffPatch is null)
                return PatchResult<Tariff>.Failed(this,
                                                  "The given charging tariff patch must not be null!");

            lock (patchLock)
            {

                if (TariffPatch["last_updated"] is null)
                    TariffPatch["last_updated"] = Timestamp.Now.ToIso8601();

                else if (AllowDowngrades == false &&
                        TariffPatch["last_updated"].Type == JTokenType.Date &&
                       (TariffPatch["last_updated"].Value<DateTime>().ToIso8601().CompareTo(LastUpdated.ToIso8601()) < 1))
                {
                    return PatchResult<Tariff>.Failed(this,
                                                      "The 'lastUpdated' timestamp of the charging tariff patch must be newer then the timestamp of the existing charging tariff!");
                }


                var patchResult = TryPrivatePatch(ToJSON(), TariffPatch);


                if (patchResult.IsFailed)
                    return PatchResult<Tariff>.Failed(this,
                                                      patchResult.ErrorResponse);

                if (TryParse(patchResult.PatchedData,
                             out var patchedTariff,
                             out var errorResponse))
                {

                    return PatchResult<Tariff>.Success(patchedTariff,
                                                       errorResponse);

                }

                else
                    return PatchResult<Tariff>.Failed(this,
                                                      "Invalid JSON merge patch of a charging tariff: " + errorResponse);

            }

        }

        #endregion


        #region Operator overloading

        #region Operator == (Tariff1, Tariff2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Tariff1">A charging tariff.</param>
        /// <param name="Tariff2">Another charging tariff.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Tariff Tariff1,
                                           Tariff Tariff2)
        {

            if (Object.ReferenceEquals(Tariff1, Tariff2))
                return true;

            if (Tariff1 is null || Tariff2 is null)
                return false;

            return Tariff1.Equals(Tariff2);

        }

        #endregion

        #region Operator != (Tariff1, Tariff2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Tariff1">A charging tariff.</param>
        /// <param name="Tariff2">Another charging tariff.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Tariff Tariff1,
                                           Tariff Tariff2)

            => !(Tariff1 == Tariff2);

        #endregion

        #region Operator <  (Tariff1, Tariff2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Tariff1">A charging tariff.</param>
        /// <param name="Tariff2">Another charging tariff.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Tariff Tariff1,
                                          Tariff Tariff2)

            => Tariff1 is null
                   ? throw new ArgumentNullException(nameof(Tariff1), "The given tariff must not be null!")
                   : Tariff1.CompareTo(Tariff2) < 0;

        #endregion

        #region Operator <= (Tariff1, Tariff2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Tariff1">A charging tariff.</param>
        /// <param name="Tariff2">Another charging tariff.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Tariff Tariff1,
                                           Tariff Tariff2)

            => !(Tariff1 > Tariff2);

        #endregion

        #region Operator >  (Tariff1, Tariff2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Tariff1">A charging tariff.</param>
        /// <param name="Tariff2">Another charging tariff.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Tariff Tariff1,
                                          Tariff Tariff2)

            => Tariff1 is null
                   ? throw new ArgumentNullException(nameof(Tariff1), "The given tariff must not be null!")
                   : Tariff1.CompareTo(Tariff2) > 0;

        #endregion

        #region Operator >= (Tariff1, Tariff2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Tariff1">A charging tariff.</param>
        /// <param name="Tariff2">Another charging tariff.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Tariff Tariff1,
                                           Tariff Tariff2)

            => !(Tariff1 < Tariff2);

        #endregion

        #endregion

        #region IComparable<Tariff> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two charging tariffs.
        /// </summary>
        /// <param name="Object">A charging tariff to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Tariff tariff
                   ? CompareTo(tariff)
                   : throw new ArgumentException("The given object is not a charging tariff!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(Tariff)

        /// <summary>
        /// Compares two charging tariffs.
        /// </summary>
        /// <param name="Object">A charging tariff to compare with.</param>
        public Int32 CompareTo(Tariff? Tariff)
        {

            if (Tariff is null)
                throw new ArgumentNullException(nameof(Tariff), "The given charging tariff must not be null!");

            var c = CountryCode.CompareTo(Tariff.CountryCode);

            if (c == 0)
                c = PartyId.    CompareTo(Tariff.PartyId);

            if (c == 0)
                c = Id.         CompareTo(Tariff.Id);

            if (c == 0)
                c = Currency.   CompareTo(Tariff.Currency);

            if (c == 0)
                c = LastUpdated.CompareTo(Tariff.LastUpdated);

            // TariffElements
            // 
            // TariffAltText
            // TariffAltURL
            // EnergyMix

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<Tariff> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two charging tariffs for equality.
        /// </summary>
        /// <param name="Object">A charging tariff to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Tariff tariff &&
                   Equals(tariff);

        #endregion

        #region Equals(Tariff)

        /// <summary>
        /// Compares two charging tariffs for equality.
        /// </summary>
        /// <param name="Tariff">A charging tariff to compare with.</param>
        public Boolean Equals(Tariff? Tariff)

            => Tariff is not null &&

               CountryCode.            Equals(Tariff.CountryCode) &&
               PartyId.                Equals(Tariff.PartyId)     &&
               Id.                     Equals(Tariff.Id)          &&
               Currency.               Equals(Tariff.Currency)                &&
               LastUpdated.ToIso8601().Equals(Tariff.LastUpdated.ToIso8601()) &&

            ((!TariffAltURL.HasValue    && !Tariff.TariffAltURL.HasValue)    ||
              (TariffAltURL.HasValue    &&  Tariff.TariffAltURL.HasValue && TariffAltURL.Value.Equals(Tariff.TariffAltURL.Value))) &&

             ((EnergyMix    is     null &&  Tariff.EnergyMix    is     null) ||
              (EnergyMix    is not null &&  Tariff.EnergyMix    is not null && EnergyMix.      Equals(Tariff.EnergyMix)))          &&

               TariffElements.Count().Equals(Tariff.TariffElements.Count()) &&
               TariffAltText. Count().Equals(Tariff.TariffAltText. Count()) &&

               TariffElements.All(tariffElement => Tariff.TariffElements.Contains(tariffElement)) &&
               TariffAltText. All(displayText   => Tariff.TariffAltText. Contains(displayText));

        #endregion

        #endregion

        #region GetHashCode()

        private readonly Int32 hashCode;

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        public override Int32 GetHashCode()
            => hashCode;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Get a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   Id,          " (",
                   CountryCode, "-",
                   PartyId,     ") ",
                   Currency,    ", ",

                   TariffElements.Count(), " tariff element(s), ",

                   TariffAltText.Any()
                       ? "text: " + TariffAltText.First().Text + ", "
                       : "",

                   TariffAltURL.HasValue
                       ? "url: "  + TariffAltURL.Value + ", "
                       : "",

                   EnergyMix is not null
                       ? "energy mix: " + EnergyMix + ", "
                       : "",

                   "last updated: " + LastUpdated.ToIso8601()

               );

        #endregion

    }

}
