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

namespace cloud.charging.open.protocols.OCPIv2_1_1
{

    /// <summary>
    /// Extension methods for energy meters.
    /// </summary>
    public static partial class EnergyMeterExtensions
    {

        #region ToJSON(this EnergyMeters, Skip = null, Take = null, ...)

        /// <summary>
        /// Return a JSON representation for the given enumeration of EnergyMeters.
        /// </summary>
        /// <param name="EnergyMeters">An enumeration of smart energy meters.</param>
        /// <param name="Skip">The optional number of smart energy meters to skip.</param>
        /// <param name="Take">The optional number of smart energy meters to return.</param>
        public static JArray ToJSON(this IEnumerable<EnergyMeter>                                 EnergyMeters,
                                    UInt64?                                                       Skip                                         = null,
                                    UInt64?                                                       Take                                         = null,
                                    CustomJObjectSerializerDelegate<EnergyMeter>?                 CustomEnergyMeterSerializer                  = null,
                                    CustomJObjectSerializerDelegate<TransparencySoftwareStatus>?  CustomTransparencySoftwareStatusSerializer   = null,
                                    CustomJObjectSerializerDelegate<TransparencySoftware>?        CustomTransparencySoftwareSerializer         = null)


            => EnergyMeters?.Any() == true

                   ? new JArray(EnergyMeters.Where         (energyMeter => energyMeter is not null).
                                             OrderBy       (energyMeter => energyMeter.Id).
                                             SkipTakeFilter(Skip, Take).
                                             SafeSelect    (energyMeter => energyMeter.ToJSON(CustomEnergyMeterSerializer,
                                                                                              CustomTransparencySoftwareStatusSerializer,
                                                                                              CustomTransparencySoftwareSerializer)).
                                             Where         (energyMeter => energyMeter is not null))

                   : new JArray();

        #endregion

    }


    /// <summary>
    /// An energy meter.
    /// </summary>
    [NonStandard]
    public class EnergyMeter : AInternalData,
                               IEquatable<EnergyMeter>,
                               IComparable<EnergyMeter>,
                               IComparable
    {

        #region Properties

        /// <summary>
        /// The identification of the energy meter.
        /// </summary>
        [Mandatory]
        public Meter_Id                                 Id                            { get; }

        /// <summary>
        /// The optional model of the energy meter.
        /// </summary>
        [Optional]
        public String?                                  Model                         { get; }

        /// <summary>
        /// The optional URL to the model of the energy meter.
        /// </summary>
        [Optional]
        public URL?                                     ModelURL                      { get; }

        /// <summary>
        /// The optional hardware version of the energy meter.
        /// </summary>
        [Optional]
        public String?                                  HardwareVersion               { get; }

        /// <summary>
        /// The optional firmware version of the energy meter.
        /// </summary>
        [Optional]
        public String?                                  FirmwareVersion               { get; }

        /// <summary>
        /// The optional manufacturer of the energy meter.
        /// </summary>
        [Optional]
        public String?                                  Manufacturer                  { get; }

        /// <summary>
        /// The optional URL to the manufacturer of the energy meter.
        /// </summary>
        [Optional]
        public URL?                                     ManufacturerURL               { get; }

        /// <summary>
        /// The optional enumeration of public keys used for signing the energy meter values.
        /// </summary>
        [Optional]
        public IEnumerable<PublicKey>                   PublicKeys                    { get; }

        /// <summary>
        /// One or multiple optional certificates for the public key of the energy meter.
        /// </summary>
        [Optional]
        public CertificateChain?                        PublicKeyCertificateChain     { get; }

        /// <summary>
        /// The enumeration of transparency softwares and their legal status,
        /// which can be used to validate the charging session data.
        /// </summary>
        [Optional]
        public IEnumerable<TransparencySoftwareStatus>  TransparencySoftwares         { get; }

        /// <summary>
        /// The multi-language description of the energy meter.
        /// </summary>
        [Optional]
        public I18NString?                              Description                   { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new energy meter.
        /// </summary>
        /// <param name="Id">The identification of the energy meter.</param>
        /// <param name="Model">An optional model of the energy meter.</param>
        /// <param name="ModelURL">An optional URL to the model of the energy meter.</param>
        /// <param name="HardwareVersion">An optional hardware version of the energy meter.</param>
        /// <param name="FirmwareVersion">An optional firmware version of the energy meter.</param>
        /// <param name="Manufacturer">An optional manufacturer of the energy meter.</param>
        /// <param name="ManufacturerURL">An optional URL to the manufacturer of the energy meter.</param>
        /// <param name="PublicKeys">The optional public key of the energy meter used for signeing the energy meter values.</param>
        /// <param name="PublicKeyCertificateChain">One or multiple optional certificates for the public key of the energy meter.</param>
        /// <param name="TransparencySoftwares">An enumeration of transparency softwares and their legal status, which can be used to validate the charging session data.</param>
        /// <param name="Description">An multi-language description of the energy meter.</param>
        public EnergyMeter(Meter_Id                                  Id,
                           String?                                   Model                       = null,
                           URL?                                      ModelURL                    = null,
                           String?                                   HardwareVersion             = null,
                           String?                                   FirmwareVersion             = null,
                           String?                                   Manufacturer                = null,
                           URL?                                      ManufacturerURL             = null,
                           IEnumerable<PublicKey>?                   PublicKeys                  = null,
                           CertificateChain?                         PublicKeyCertificateChain   = null,
                           IEnumerable<TransparencySoftwareStatus>?  TransparencySoftwares       = null,
                           I18NString?                               Description                 = null,

                           JObject?                                  CustomData                  = null,
                           UserDefinedDictionary?                    InternalData                = null)

            : base(CustomData,
                   InternalData)

        {

            this.Id                         = Id;
            this.Model                      = Model;
            this.ModelURL                   = ModelURL;
            this.HardwareVersion            = HardwareVersion;
            this.FirmwareVersion            = FirmwareVersion;
            this.Manufacturer               = Manufacturer;
            this.ManufacturerURL            = ManufacturerURL;
            this.PublicKeys                 = PublicKeys?.           Distinct() ?? Array.Empty<PublicKey>();
            this.PublicKeyCertificateChain  = PublicKeyCertificateChain;
            this.TransparencySoftwares      = TransparencySoftwares?.Distinct() ?? Array.Empty<TransparencySoftwareStatus>();
            this.Description                = Description                       ?? I18NString.Empty;

        }

        #endregion


        #region (static) Parse   (JSON, CustomEnergyMeterParser = null)

        /// <summary>
        /// Parse the given JSON representation of an energy meter.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomEnergyMeterParser">A delegate to parse custom energy meter JSON objects.</param>
        public static EnergyMeter Parse(JObject                                    JSON,
                                        CustomJObjectParserDelegate<EnergyMeter>?  CustomEnergyMeterParser   = null)
        {

            if (TryParse(JSON,
                         out var energyMeter,
                         out var errorResponse,
                         CustomEnergyMeterParser))
            {
                return energyMeter!;
            }

            throw new ArgumentException("The given JSON representation of an energy meter is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out EnergyMeter, out ErrorResponse, CustomEnergyMeterParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of an energy meter.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="EnergyMeter">The parsed energy meter.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject           JSON,
                                       out EnergyMeter?  EnergyMeter,
                                       out String?       ErrorResponse)

            => TryParse(JSON,
                        out EnergyMeter,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of an energy meter.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="EnergyMeter">The parsed energy meter.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomEnergyMeterParser">A delegate to parse custom energy meter JSON objects.</param>
        public static Boolean TryParse(JObject                                    JSON,
                                       out EnergyMeter?                           EnergyMeter,
                                       out String?                                ErrorResponse,
                                       CustomJObjectParserDelegate<EnergyMeter>?  CustomEnergyMeterParser   = null)
        {

            try
            {

                EnergyMeter = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse Id                            [mandatory]

                if (!JSON.ParseMandatory("id",
                                         "energy meter identification",
                                         Meter_Id.TryParse,
                                         out Meter_Id Id,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Model                         [optional]

                var Model                 = JSON.GetString("model");

                #endregion

                #region Parse ModelURL                      [optional]

                if (JSON.ParseOptional("model_url",
                                       "energy meter model URL",
                                       URL.TryParse,
                                       out URL? ModelURL,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse HardwareVersion               [optional]

                var HardwareVersion       = JSON.GetString("hardware_version");

                #endregion

                #region Parse FirmwareVersion               [optional]

                var FirmwareVersion       = JSON.GetString("firmware_version");

                #endregion

                #region Parse Vendor                        [optional]

                var Manufacturer          = JSON.GetString("manufacturer");

                #endregion

                #region Parse ManufacturerURL               [optional]

                if (JSON.ParseOptional("manufacturer_url",
                                       "energy meter manufacturer URL",
                                       URL.TryParse,
                                       out URL? ManufacturerURL,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse PublicKeys                    [optional]

                if (JSON.ParseOptionalHashSet("public_keys",
                                              "energy meter public keys",
                                              PublicKey.TryParse,
                                              out HashSet<PublicKey> PublicKeys,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse PublicKeyCertificateChain     [optional]

                if (JSON.ParseOptional("public_key_certificate_chain",
                                       "energy meter public key certificate chain",
                                       CertificateChain.TryParse,
                                       out CertificateChain? PublicKeyCertificateChain,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse TransparencySoftwareStatus    [optional]

                if (JSON.ParseOptionalHashSet("transparency_softwares",
                                              "transparency softwares",
                                              TransparencySoftwareStatus.TryParse,
                                              out HashSet<TransparencySoftwareStatus> TransparencySoftwares,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse Description                   [optional]

                if (JSON.ParseOptional("description",
                                       "energy meter description",
                                       I18NString.TryParse,
                                       out I18NString? Description,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                EnergyMeter = new EnergyMeter(Id,
                                              Model,
                                              ModelURL,
                                              HardwareVersion,
                                              FirmwareVersion,
                                              Manufacturer,
                                              ManufacturerURL,
                                              PublicKeys,
                                              PublicKeyCertificateChain,
                                              TransparencySoftwares,
                                              Description);

                if (CustomEnergyMeterParser is not null)
                    EnergyMeter = CustomEnergyMeterParser(JSON,
                                                          EnergyMeter);

                return true;

            }
            catch (Exception e)
            {
                EnergyMeter    = default;
                ErrorResponse  = "The given JSON representation of an energy meter is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomEnergyMeterSerializer = null, CustomTransparencySoftwareStatusSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomEnergyMeterSerializer">A delegate to serialize custom energy meter JSON objects.</param>
        /// <param name="CustomTransparencySoftwareStatusSerializer">A delegate to serialize custom transparency software status JSON objects.</param>
        /// <param name="CustomTransparencySoftwareSerializer">A delegate to serialize custom transparency software JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<EnergyMeter>?                 CustomEnergyMeterSerializer                  = null,
                              CustomJObjectSerializerDelegate<TransparencySoftwareStatus>?  CustomTransparencySoftwareStatusSerializer   = null,
                              CustomJObjectSerializerDelegate<TransparencySoftware>?        CustomTransparencySoftwareSerializer         = null)
        {

            var json = JSONObject.Create(

                           new JProperty("id",                                   Id.                             ToString()),

                           Model is not null
                               ? new JProperty("model",                          Model)
                               : null,

                           ModelURL.HasValue
                               ? new JProperty("model_url",                      ModelURL.                 Value.ToString())
                               : null,

                           HardwareVersion is not null
                               ? new JProperty("hardware_version",               HardwareVersion)
                               : null,

                           FirmwareVersion is not null
                               ? new JProperty("firmware_version",               FirmwareVersion)
                               : null,

                           Manufacturer is not null
                               ? new JProperty("manufacturer",                   Manufacturer)
                               : null,

                           ManufacturerURL.HasValue
                               ? new JProperty("manufacturer_url",               ManufacturerURL.          Value.ToString())
                               : null,

                           PublicKeys.Any()
                               ? new JProperty("public_keys",                    new JArray(PublicKeys.Select(publicKey => publicKey.ToString())))
                               : null,

                           PublicKeyCertificateChain.HasValue
                               ? new JProperty("public_key_certificate_chain",   PublicKeyCertificateChain.Value.ToString())
                               : null,

                           TransparencySoftwares.Any()
                               ? new JProperty("transparency_softwares",         new JArray(TransparencySoftwares.Select(transparencySoftwareStatus => transparencySoftwareStatus.ToJSON(CustomTransparencySoftwareStatusSerializer,
                                                                                                                                                                                         CustomTransparencySoftwareSerializer))))
                               : null,

                           Description is not null && Description.IsNeitherNullNorEmpty()
                               ? new JProperty("description",                    Description.ToJSON())
                               : null

                       );

            return CustomEnergyMeterSerializer is not null
                       ? CustomEnergyMeterSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (EnergyMeter1, EnergyMeter2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergyMeter1">An energy meter.</param>
        /// <param name="EnergyMeter2">Another energy meter.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (EnergyMeter EnergyMeter1,
                                           EnergyMeter EnergyMeter2)
        {

            if (Object.ReferenceEquals(EnergyMeter1, EnergyMeter2))
                return true;

            if (EnergyMeter1 is null || EnergyMeter2 is null)
                return false;

            return EnergyMeter1.Equals(EnergyMeter2);

        }

        #endregion

        #region Operator != (EnergyMeter1, EnergyMeter2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergyMeter1">An energy meter.</param>
        /// <param name="EnergyMeter2">Another energy meter.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (EnergyMeter EnergyMeter1,
                                           EnergyMeter EnergyMeter2)

            => !(EnergyMeter1 == EnergyMeter2);

        #endregion

        #region Operator <  (EnergyMeter1, EnergyMeter2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergyMeter1">An energy meter.</param>
        /// <param name="EnergyMeter2">Another energy meter.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (EnergyMeter EnergyMeter1,
                                          EnergyMeter EnergyMeter2)

            => EnergyMeter1 is null
                   ? throw new ArgumentNullException(nameof(EnergyMeter1), "The given energy meter must not be null!")
                   : EnergyMeter1.CompareTo(EnergyMeter2) < 0;

        #endregion

        #region Operator <= (EnergyMeter1, EnergyMeter2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergyMeter1">An energy meter.</param>
        /// <param name="EnergyMeter2">Another energy meter.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (EnergyMeter EnergyMeter1,
                                           EnergyMeter EnergyMeter2)

            => !(EnergyMeter1 > EnergyMeter2);

        #endregion

        #region Operator >  (EnergyMeter1, EnergyMeter2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergyMeter1">An energy meter.</param>
        /// <param name="EnergyMeter2">Another energy meter.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (EnergyMeter EnergyMeter1,
                                          EnergyMeter EnergyMeter2)

            => EnergyMeter1 is null
                   ? throw new ArgumentNullException(nameof(EnergyMeter1), "The given energy meter must not be null!")
                   : EnergyMeter1.CompareTo(EnergyMeter2) > 0;

        #endregion

        #region Operator >= (EnergyMeter1, EnergyMeter2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergyMeter1">An energy meter.</param>
        /// <param name="EnergyMeter2">Another energy meter.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (EnergyMeter EnergyMeter1,
                                           EnergyMeter EnergyMeter2)

            => !(EnergyMeter1 < EnergyMeter2);

        #endregion

        #endregion

        #region IComparable<EnergyMeter> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two energy meters.
        /// </summary>
        /// <param name="Object">An energy meter to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is EnergyMeter energyMeter
                   ? CompareTo(energyMeter)
                   : throw new ArgumentException("The given object is not an energy meter!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(EnergyMeter)

        /// <summary>
        /// Compares two energy meters.
        /// </summary>
        /// <param name="EnergyMeter">An energy meter to compare with.</param>
        public Int32 CompareTo(EnergyMeter? EnergyMeter)
        {

            if (EnergyMeter is null)
                throw new ArgumentNullException(nameof(EnergyMeter), "The given energy meter must not be null!");

            var c = Id.CompareTo(EnergyMeter.Id);

            // Model
            // HardwareVersion
            // FirmwareVersion
            // Vendor
            // PublicKey
            // PublicKeyCertificateChain
            // TransparencySoftware

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<EnergyMeter> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two energy meters for equality.
        /// </summary>
        /// <param name="Object">An energy meter to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is EnergyMeter energyMeter &&
                   Equals(energyMeter);

        #endregion

        #region Equals(EnergyMeter)

        /// <summary>
        /// Compares two energy meters for equality.
        /// </summary>
        /// <param name="EnergyMeter">An energy meter to compare with.</param>
        public Boolean Equals(EnergyMeter? EnergyMeter)

            => EnergyMeter is not null &&

               Id.Equals(EnergyMeter.Id) &&

             ((Model                     is     null &&  EnergyMeter.Model                     is     null) ||
              (Model                     is not null &&  EnergyMeter.Model                     is not null && Model.                          Equals(EnergyMeter.Model)))                           &&

            ((!ModelURL.                 HasValue    && !EnergyMeter.ModelURL.                 HasValue)    ||
             ( ModelURL.                 HasValue    &&  EnergyMeter.ModelURL.                 HasValue    && ModelURL.                 Value.Equals(EnergyMeter.ModelURL.                 Value))) &&

             ((HardwareVersion           is     null &&  EnergyMeter.HardwareVersion           is     null) ||
              (HardwareVersion           is not null &&  EnergyMeter.HardwareVersion           is not null && HardwareVersion.                Equals(EnergyMeter.HardwareVersion)))                 &&

             ((FirmwareVersion           is     null &&  EnergyMeter.FirmwareVersion           is     null) ||
              (FirmwareVersion           is not null &&  EnergyMeter.FirmwareVersion           is not null && FirmwareVersion.                Equals(EnergyMeter.FirmwareVersion)))                 &&

             ((Manufacturer              is     null &&  EnergyMeter.Manufacturer              is     null) ||
              (Manufacturer              is not null &&  EnergyMeter.Manufacturer              is not null && Manufacturer.                   Equals(EnergyMeter.Manufacturer)))                    &&

            ((!ManufacturerURL.          HasValue    && !EnergyMeter.ManufacturerURL.          HasValue)    ||
             ( ManufacturerURL.          HasValue    &&  EnergyMeter.ManufacturerURL.          HasValue    && ManufacturerURL.          Value.Equals(EnergyMeter.ManufacturerURL.          Value))) &&

            ((!PublicKeyCertificateChain.HasValue    &&  EnergyMeter.PublicKeyCertificateChain.HasValue)    ||
              (PublicKeyCertificateChain.HasValue    &&  EnergyMeter.PublicKeyCertificateChain.HasValue    && PublicKeyCertificateChain.Value.Equals(EnergyMeter.PublicKeyCertificateChain.Value))) &&

             ((Description               is     null &&  EnergyMeter.Description               is     null) ||
              (Description               is not null &&  EnergyMeter.Description               is not null && Description.                    Equals(EnergyMeter.Description)))                     &&

               PublicKeys.           Count().Equals(EnergyMeter.PublicKeys.           Count())                          &&
               PublicKeys.           All(publicKey            => EnergyMeter.PublicKeys.           Contains(publicKey)) &&

               TransparencySoftwares.Count().Equals(EnergyMeter.TransparencySoftwares.Count())                          &&
               TransparencySoftwares.All(transparencySoftware => EnergyMeter.TransparencySoftwares.Contains(transparencySoftware));

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

                return Id.                        GetHashCode()        * 29 ^
                      (Model?.                    GetHashCode()  ?? 0) * 27 ^
                      (ModelURL?.                 GetHashCode()  ?? 0) * 23 ^
                      (HardwareVersion?.          GetHashCode()  ?? 0) * 19 ^
                      (FirmwareVersion?.          GetHashCode()  ?? 0) * 17 ^
                      (Manufacturer?.             GetHashCode()  ?? 0) * 13 ^
                      (ManufacturerURL?.          GetHashCode()  ?? 0) * 11 ^
                      (PublicKeys?.               CalcHashCode() ?? 0) *  7 ^
                      (PublicKeyCertificateChain?.GetHashCode()  ?? 0) *  5 ^
                      (TransparencySoftwares?.    CalcHashCode() ?? 0) *  3 ^
                       Description?.              GetHashCode()  ?? 0;

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => new String[] {

                   $"Id: {Id}",

                   Model.IsNotNullOrEmpty()
                       ? $"Model: {Model}"
                       : String.Empty,

                   ModelURL.HasValue
                       ? $"Model URL: {ModelURL}"
                       : String.Empty,

                   HardwareVersion.IsNotNullOrEmpty()
                       ? $"Hardware version: {HardwareVersion}"
                       : String.Empty,

                   FirmwareVersion.IsNotNullOrEmpty()
                       ? $"Firmware version: {FirmwareVersion}"
                       : String.Empty,

                   Manufacturer.IsNotNullOrEmpty()
                       ? $"Manufacturer: {Manufacturer}"
                       : String.Empty,

                   ManufacturerURL.HasValue
                       ? $"Manufacturer URL: {ManufacturerURL}"
                       : String.Empty,

                   PublicKeys.Any()
                       ? "public keys: " + PublicKeys.Select(publicKey => publicKey.ToString().SubstringMax(20)).AggregateWith(", ")
                       : String.Empty,

                   PublicKeyCertificateChain.HasValue
                       ? $"public key certificate chain: {PublicKeyCertificateChain.Value.ToString().SubstringMax(20)}"
                       : String.Empty,

                   TransparencySoftwares.Any()
                       ? $"{TransparencySoftwares.Count()} transparency software(s)"
                       : String.Empty,

                   Description is not null && Description.IsNeitherNullNorEmpty()
                       ? $"Description: {Description}"
                       : String.Empty

            }.Where(_ => _.IsNotNullOrEmpty()).
              AggregateWith(", ");

        #endregion

    }

}
