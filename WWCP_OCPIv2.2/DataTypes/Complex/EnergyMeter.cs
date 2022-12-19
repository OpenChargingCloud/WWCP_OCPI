/*
 * Copyright (c) 2015-2022 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2
{

    /// <summary>
    /// An energy meter. This information will e.g. be used for the German calibration law.
    /// </summary>
    [NonStandard]
    public class EnergyMeter : IEquatable<EnergyMeter>,
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
        /// The optional vendor of the energy meter.
        /// </summary>
        [Optional]
        public String?                                  Vendor                        { get; }

        /// <summary>
        /// The optional public key of the energy meter used for signeing the energy meter values.
        /// </summary>
        [Optional]
        public PublicKey?                               PublicKey                     { get; }

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

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new energy meter.
        /// </summary>
        /// <param name="Id">The identification of the energy meter.</param>
        /// <param name="Model">The optional model of the energy meter.</param>
        /// <param name="HardwareVersion">The optional hardware version of the energy meter.</param>
        /// <param name="FirmwareVersion">The optional firmware version of the energy meter.</param>
        /// <param name="Vendor">The optional vendor of the energy meter.</param>
        /// <param name="PublicKey">The optional public key of the energy meter used for signeing the energy meter values.</param>
        /// <param name="PublicKeyCertificateChain">One or multiple optional certificates for the public key of the energy meter.</param>
        /// <param name="TransparencySoftwares">An enumeration of transparency softwares and their legal status, which can be used to validate the charging session data.</param>
        /// 
        public EnergyMeter(Meter_Id                                  Id,
                           String?                                   Model                       = null,
                           String?                                   HardwareVersion             = null,
                           String?                                   FirmwareVersion             = null,
                           String?                                   Vendor                      = null,
                           PublicKey?                                PublicKey                   = null,
                           CertificateChain?                         PublicKeyCertificateChain   = null,
                           IEnumerable<TransparencySoftwareStatus>?  TransparencySoftwares       = null)
        {

            this.Id                         = Id;
            this.Model                      = Model;
            this.HardwareVersion            = HardwareVersion;
            this.FirmwareVersion            = FirmwareVersion;
            this.Vendor                     = Vendor;
            this.PublicKey                  = PublicKey;
            this.PublicKeyCertificateChain  = PublicKeyCertificateChain;
            this.TransparencySoftwares      = TransparencySoftwares?.Distinct() ?? Array.Empty<TransparencySoftwareStatus>();

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

                #region Parse HardwareVersion               [optional]

                var HardwareVersion       = JSON.GetString("hardware_version");

                #endregion

                #region Parse FirmwareVersion               [optional]

                var FirmwareVersion       = JSON.GetString("firmware_version");

                #endregion

                #region Parse Vendor                        [optional]

                var Vendor                = JSON.GetString("vendor");

                #endregion

                #region Parse PublicKey                     [optional]

                if (JSON.ParseOptional("public_key",
                                       "energy meter public key",
                                       OCPIv2_2.PublicKey.TryParse,
                                       out PublicKey? PublicKey,
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


                EnergyMeter = new EnergyMeter(Id,
                                              Model,
                                              HardwareVersion,
                                              FirmwareVersion,
                                              Vendor,
                                              PublicKey,
                                              PublicKeyCertificateChain,
                                              TransparencySoftwares);

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

            var JSON = JSONObject.Create(

                           new JProperty("id",                                   Id.       ToString()),

                           Model is not null
                               ? new JProperty("model",                          Model)
                               : null,

                           HardwareVersion is not null
                               ? new JProperty("hardware_version",               HardwareVersion)
                               : null,

                           FirmwareVersion is not null
                               ? new JProperty("firmware_version",               FirmwareVersion)
                               : null,

                           Vendor is not null
                               ? new JProperty("vendor",                         Vendor)
                               : null,

                           PublicKey.HasValue
                               ? new JProperty("public_key",                     PublicKey.ToString())
                               : null,

                           PublicKeyCertificateChain.HasValue
                               ? new JProperty("public_key_certificate_chain",   PublicKeyCertificateChain)
                               : null,

                           TransparencySoftwares.Any()
                               ? new JProperty("transparency_softwares",         new JArray(TransparencySoftwares.Select(transparencySoftwareStatus => transparencySoftwareStatus.ToJSON(CustomTransparencySoftwareStatusSerializer,
                                                                                                                                                                                         CustomTransparencySoftwareSerializer))))
                               : null

                       );

            return CustomEnergyMeterSerializer is not null
                       ? CustomEnergyMeterSerializer(this, JSON)
                       : JSON;

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

             ((HardwareVersion           is     null &&  EnergyMeter.HardwareVersion           is     null) ||
              (HardwareVersion           is not null &&  EnergyMeter.HardwareVersion           is not null && HardwareVersion.                Equals(EnergyMeter.HardwareVersion)))                 &&

             ((FirmwareVersion           is     null &&  EnergyMeter.FirmwareVersion           is     null) ||
              (FirmwareVersion           is not null &&  EnergyMeter.FirmwareVersion           is not null && FirmwareVersion.                Equals(EnergyMeter.FirmwareVersion)))                 &&

             ((Vendor                    is     null &&  EnergyMeter.Vendor                    is     null) ||
              (Vendor                    is not null &&  EnergyMeter.Vendor                    is not null && Vendor.                         Equals(EnergyMeter.Vendor)))                          &&

            ((!PublicKey.                HasValue    && !EnergyMeter.PublicKey.                HasValue)    ||
             ( PublicKey.                HasValue    &&  EnergyMeter.PublicKey.                HasValue    && PublicKey.                Value.Equals(EnergyMeter.PublicKey.                Value))) &&

            ((!PublicKeyCertificateChain.HasValue    &&  EnergyMeter.PublicKeyCertificateChain.HasValue)    ||
              (PublicKeyCertificateChain.HasValue    &&  EnergyMeter.PublicKeyCertificateChain.HasValue    && PublicKeyCertificateChain.Value.Equals(EnergyMeter.PublicKeyCertificateChain.Value))) &&

               TransparencySoftwares.Count().Equals(EnergyMeter.TransparencySoftwares.Count()) &&
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

                return Id.                        GetHashCode()       * 17 ^
                      (HardwareVersion?.          GetHashCode() ?? 0) * 13 ^
                      (FirmwareVersion?.          GetHashCode() ?? 0) * 11 ^
                      (Vendor?.                   GetHashCode() ?? 0) *  7 ^
                      (PublicKey?.                GetHashCode() ?? 0) *  5 ^
                      (PublicKeyCertificateChain?.GetHashCode() ?? 0) *  3 ^
                       TransparencySoftwares.      CalcHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   Id,

                   HardwareVersion      is not null
                       ? ", " + HardwareVersion
                       : "",

                   FirmwareVersion      is not null
                       ? ", " + FirmwareVersion
                       : "",

                   Vendor               is not null
                       ? ", " + Vendor
                       : "",

                   PublicKey.HasValue
                       ? ", publicKey: "         + PublicKey.                Value.ToString().SubstringMax(20)
                       : "",

                   PublicKeyCertificateChain is not null
                       ? ", certificate chain: " + PublicKeyCertificateChain.Value.ToString().SubstringMax(20)
                       : "",

                   TransparencySoftwares.Count(),
                   " transparency software(s)"

               );

        #endregion

    }

}
