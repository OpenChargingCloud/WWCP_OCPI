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

using System;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_1_1
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
        public Meter_Id    Id                      { get; }

        /// <summary>
        /// The optional model of the energy meter.
        /// </summary>
        [Optional]
        public String      Model                   { get; }

        /// <summary>
        /// The optional hardware version of the energy meter.
        /// </summary>
        [Optional]
        public String      HardwareVersion         { get; }

        /// <summary>
        /// The optional firmware version of the energy meter.
        /// </summary>
        [Optional]
        public String      FirmwareVersion         { get; }

        /// <summary>
        /// The optional vendor of the energy meter.
        /// </summary>
        [Optional]
        public String      Vendor                  { get; }

        /// <summary>
        /// The optional public key of the energy meter used for signeing the energy meter values.
        /// </summary>
        [Optional]
        public PublicKey?  PublicKey               { get; }

        /// <summary>
        /// One or multiple optional certificates for the public key of the energy meter.
        /// </summary>
        [Optional]
        public String      PublicKeyCertificate    { get; }

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
        /// <param name="PublicKeyCertificate">One or multiple optional certificates for the public key of the energy meter.</param>
        public EnergyMeter(Meter_Id    Id,
                           String      Model                  = null,
                           String      HardwareVersion        = null,
                           String      FirmwareVersion        = null,
                           String      Vendor                 = null,
                           PublicKey?  PublicKey              = null,
                           String      PublicKeyCertificate   = null)
        {

            this.Id                    = Id;
            this.Model                 = Model;
            this.HardwareVersion       = HardwareVersion;
            this.FirmwareVersion       = FirmwareVersion;
            this.Vendor                = Vendor;
            this.PublicKey             = PublicKey;
            this.PublicKeyCertificate  = PublicKeyCertificate;

        }

        #endregion


        #region (static) Parse   (JSON, CustomEnergyMeterParser = null)

        /// <summary>
        /// Parse the given JSON representation of an energy meter.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomEnergyMeterParser">A delegate to parse custom energy meter JSON objects.</param>
        public static EnergyMeter Parse(JObject                                   JSON,
                                        CustomJObjectParserDelegate<EnergyMeter>  CustomEnergyMeterParser   = null)
        {

            if (TryParse(JSON,
                         out EnergyMeter  energyMeter,
                         out String       ErrorResponse,
                         CustomEnergyMeterParser))
            {
                return energyMeter;
            }

            throw new ArgumentException("The given JSON representation of an energy meter is invalid: " + ErrorResponse, nameof(JSON));

        }

        #endregion

        #region (static) Parse   (Text, CustomEnergyMeterParser = null)

        /// <summary>
        /// Parse the given text representation of an energy meter.
        /// </summary>
        /// <param name="Text">The text to parse.</param>
        /// <param name="CustomEnergyMeterParser">A delegate to parse custom energy meter JSON objects.</param>
        public static EnergyMeter Parse(String                                    Text,
                                        CustomJObjectParserDelegate<EnergyMeter>  CustomEnergyMeterParser   = null)
        {

            if (TryParse(Text,
                         out EnergyMeter  energyMeter,
                         out String       ErrorResponse,
                         CustomEnergyMeterParser))
            {
                return energyMeter;
            }

            throw new ArgumentException("The given text representation of an energy meter is invalid: " + ErrorResponse, nameof(Text));

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
        public static Boolean TryParse(JObject          JSON,
                                       out EnergyMeter  EnergyMeter,
                                       out String       ErrorResponse)

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
        public static Boolean TryParse(JObject                                   JSON,
                                       out EnergyMeter                           EnergyMeter,
                                       out String                                ErrorResponse,
                                       CustomJObjectParserDelegate<EnergyMeter>  CustomEnergyMeterParser   = null)
        {

            try
            {

                EnergyMeter = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse Id                        [mandatory]

                if (!JSON.ParseMandatory("id",
                                         "energy meter identification",
                                         Meter_Id.TryParse,
                                         out Meter_Id Id,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Model                     [optional]

                var Model                 = JSON.GetString("model");

                #endregion

                #region Parse HardwareVersion           [optional]

                var HardwareVersion       = JSON.GetString("hardware_version");

                #endregion

                #region Parse FirmwareVersion           [optional]

                var FirmwareVersion       = JSON.GetString("firmware_version");

                #endregion

                #region Parse Vendor                    [optional]

                var Vendor                = JSON.GetString("vendor");

                #endregion

                #region Parse PublicKey                 [optional]

                if (JSON.ParseOptional("public_key",
                                       "energy meter public key",
                                       OCPIv2_1_1.PublicKey.TryParse,
                                       out PublicKey? PublicKey,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse PublicKeyCertificate      [optional]

                var PublicKeyCertificate = JSON.GetString("public_key_certificate");

                #endregion


                EnergyMeter = new EnergyMeter(Id,
                                              Model,
                                              HardwareVersion,
                                              FirmwareVersion,
                                              Vendor,
                                              PublicKey,
                                              PublicKeyCertificate);

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

        #region (static) TryParse(Text, out EnergyMeter, out ErrorResponse, CustomEnergyMeterParser = null)

        /// <summary>
        /// Try to parse the given text representation of an energy meter.
        /// </summary>
        /// <param name="Text">The text to parse.</param>
        /// <param name="EnergyMeter">The parsed energyMeter.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomEnergyMeterParser">A delegate to parse custom energy meter JSON objects.</param>
        public static Boolean TryParse(String                                    Text,
                                       out EnergyMeter                           EnergyMeter,
                                       out String                                ErrorResponse,
                                       CustomJObjectParserDelegate<EnergyMeter>  CustomEnergyMeterParser   = null)
        {

            try
            {

                return TryParse(JObject.Parse(Text),
                                out EnergyMeter,
                                out ErrorResponse,
                                CustomEnergyMeterParser);

            }
            catch (Exception e)
            {
                EnergyMeter    = default;
                ErrorResponse  = "The given text representation of an energy meter is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomEnergyMeterSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomEnergyMeterSerializer">A delegate to serialize custom energy meter JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<EnergyMeter> CustomEnergyMeterSerializer = null)
        {

            var JSON = JSONObject.Create(

                           new JProperty("id",                            Id.                  ToString()),

                           Model.IsNotNullOrEmpty()
                               ? new JProperty("model",                   Model.               ToString())
                               : null,

                           HardwareVersion.IsNotNullOrEmpty()
                               ? new JProperty("hardware_version",        HardwareVersion.     ToString())
                               : null,

                           FirmwareVersion.IsNotNullOrEmpty()
                               ? new JProperty("firmware_version",        FirmwareVersion.     ToString())
                               : null,

                           Vendor.IsNotNullOrEmpty()
                               ? new JProperty("vendor",                  Vendor.              ToString())
                               : null,

                           PublicKey.HasValue
                               ? new JProperty("public_key",              PublicKey.           ToString())
                               : null,

                           PublicKeyCertificate.IsNotNullOrEmpty()
                               ? new JProperty("public_key_certificate",  PublicKeyCertificate.ToString())
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
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is EnergyMeter energyMeter
                   ? CompareTo(energyMeter)
                   : throw new ArgumentException("The given object is not an energy meter!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(EnergyMeter)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergyMeter">An object to compare with.</param>
        public Int32 CompareTo(EnergyMeter EnergyMeter)
        {

            if (EnergyMeter is null)
                throw new ArgumentNullException(nameof(EnergyMeter), "The given energy meter must not be null!");

            var c = Id.CompareTo(EnergyMeter.Id);

            if (c == 0)
                c = Model.CompareTo(EnergyMeter.Model);

            if (c == 0)
                c = HardwareVersion.CompareTo(EnergyMeter.HardwareVersion);

            if (c == 0)
                c = FirmwareVersion.CompareTo(EnergyMeter.FirmwareVersion);

            if (c == 0)
                c = Vendor.CompareTo(EnergyMeter.Vendor);

            if (c == 0 && PublicKey.HasValue && EnergyMeter.PublicKey.HasValue)
                c = PublicKey.Value.CompareTo(EnergyMeter.PublicKey.Value);

            if (c == 0)
                c = PublicKeyCertificate.CompareTo(EnergyMeter.PublicKeyCertificate);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<EnergyMeter> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is EnergyMeter energyMeter &&
                   Equals(energyMeter);

        #endregion

        #region Equals(EnergyMeter)

        /// <summary>
        /// Compares two energy meters for equality.
        /// </summary>
        /// <param name="EnergyMeter">An energy meter to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(EnergyMeter EnergyMeter)

            => !(EnergyMeter is null) &&

               Id.Equals(EnergyMeter.Id) &&

               ((HardwareVersion.     IsNullOrEmpty()    &&  EnergyMeter.HardwareVersion.     IsNullOrEmpty()) ||
                (HardwareVersion.     IsNotNullOrEmpty() &&  EnergyMeter.HardwareVersion.     IsNotNullOrEmpty() && HardwareVersion.     Equals(EnergyMeter.HardwareVersion))) &&

               ((FirmwareVersion.     IsNullOrEmpty()    &&  EnergyMeter.FirmwareVersion.     IsNullOrEmpty()) ||
                (FirmwareVersion.     IsNotNullOrEmpty() &&  EnergyMeter.FirmwareVersion.     IsNotNullOrEmpty() && FirmwareVersion.     Equals(EnergyMeter.FirmwareVersion))) &&

               ((Vendor.              IsNullOrEmpty()    &&  EnergyMeter.Vendor.              IsNullOrEmpty()) ||
                (Vendor.              IsNotNullOrEmpty() &&  EnergyMeter.Vendor.              IsNotNullOrEmpty() && Vendor.              Equals(EnergyMeter.Vendor)))          &&

              ((!PublicKey.           HasValue           && !EnergyMeter.PublicKey.           HasValue) ||
               ( PublicKey.           HasValue           &&  EnergyMeter.PublicKey.           HasValue           && PublicKey.     Value.Equals(EnergyMeter.PublicKey.Value))) &&


               ((PublicKeyCertificate.IsNullOrEmpty()    &&  EnergyMeter.PublicKeyCertificate.IsNullOrEmpty()) ||
                (PublicKeyCertificate.IsNotNullOrEmpty() &&  EnergyMeter.PublicKeyCertificate.IsNotNullOrEmpty() && PublicKeyCertificate.Equals(EnergyMeter.PublicKeyCertificate)));

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

                return Id.GetHashCode()                          * 13 ^

                       (HardwareVersion.IsNotNullOrEmpty()
                            ? HardwareVersion.     GetHashCode() * 11
                            : 0) ^

                       (FirmwareVersion.IsNotNullOrEmpty()
                            ? FirmwareVersion.     GetHashCode() *  7
                            : 0) ^

                       (Vendor.IsNotNullOrEmpty()
                            ? Vendor.              GetHashCode() *  5
                            : 0) ^

                       (PublicKey.HasValue
                            ? PublicKey.           GetHashCode() *  3
                            : 0) ^

                       (PublicKeyCertificate.IsNotNullOrEmpty()
                            ? PublicKeyCertificate.GetHashCode()
                            : 0);

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(Id,
                             HardwareVersion.     IsNotNullOrEmpty() ? ", " + HardwareVersion                                   : "",
                             FirmwareVersion.     IsNotNullOrEmpty() ? ", " + FirmwareVersion                                   : "",
                             Vendor.              IsNotNullOrEmpty() ? ", " + Vendor                                            : "",
                             PublicKey.           HasValue           ? ", PublicKey: "   + PublicKey.ToString().SubstringMax(8) : "",
                             PublicKeyCertificate.IsNotNullOrEmpty() ? ", Certificate: " + PublicKeyCertificate.SubstringMax(8) : "");

        #endregion

    }

}
