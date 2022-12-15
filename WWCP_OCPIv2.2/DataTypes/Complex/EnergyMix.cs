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
using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2
{

    /// <summary>
    /// The energy mix.
    /// </summary>
    public class EnergyMix : IEquatable<EnergyMix>
    {

        #region Properties

        /// <summary>
        /// The energy is green.
        /// </summary>
        [Mandatory]
        public Boolean                           IsGreenEnergy           { get; }

        /// <summary>
        /// The energy mixs.
        /// </summary>
        [Optional]
        public IEnumerable<EnergySource>         EnergySources           { get; }

        /// <summary>
        /// The environmental impacts.
        /// </summary>
        [Optional]
        public IEnumerable<EnvironmentalImpact>  EnvironmentalImpacts    { get; }

        /// <summary>
        /// The name of the energy supplier.
        /// </summary>
        [Optional]
        public String                            SupplierName            { get; }

        /// <summary>
        /// The name of the energy product.
        /// </summary>
        [Optional]
        public String                            EnergyProductName       { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// The energy mix.
        /// </summary>
        /// <param name="IsGreenEnergy">The energy is green.</param>
        /// <param name="EnergySources">The optional energy sources.</param>
        /// <param name="EnvironmentalImpacts">The optional environmental impacts.</param>
        /// <param name="SupplierName">The optional name of the energy supplier.</param>
        /// <param name="EnergyProductName">The optional name of the energy product.</param>
        public EnergyMix(Boolean                           IsGreenEnergy,
                         IEnumerable<EnergySource>         EnergySources          = null,
                         IEnumerable<EnvironmentalImpact>  EnvironmentalImpacts   = null,
                         String                            SupplierName           = null,
                         String                            EnergyProductName      = null)

        {

            this.IsGreenEnergy         = IsGreenEnergy;
            this.EnergySources         = EnergySources?.       Distinct() ?? new EnergySource[0];
            this.EnvironmentalImpacts  = EnvironmentalImpacts?.Distinct() ?? new EnvironmentalImpact[0];
            this.SupplierName          = SupplierName;
            this.EnergyProductName     = EnergyProductName;

        }

        #endregion


        #region (static) Parse   (JSON, CustomEnergyMixParser = null)

        /// <summary>
        /// Parse the given JSON representation of an energy mix.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomEnergyMixParser">A delegate to parse custom energy mix JSON objects.</param>
        public static EnergyMix Parse(JObject                                 JSON,
                                      CustomJObjectParserDelegate<EnergyMix>  CustomEnergyMixParser   = null)
        {

            if (TryParse(JSON,
                         out EnergyMix energyMix,
                         out String    ErrorResponse,
                         CustomEnergyMixParser))
            {
                return energyMix;
            }

            throw new ArgumentException("The given JSON representation of an energy mix is invalid: " + ErrorResponse, nameof(JSON));

        }

        #endregion

        #region (static) Parse   (Text, CustomEnergyMixParser = null)

        /// <summary>
        /// Parse the given text representation of an energy mix.
        /// </summary>
        /// <param name="Text">The text to parse.</param>
        /// <param name="CustomEnergyMixParser">A delegate to parse custom energy mix JSON objects.</param>
        public static EnergyMix Parse(String                                         Text,
                                             CustomJObjectParserDelegate<EnergyMix>  CustomEnergyMixParser   = null)
        {

            if (TryParse(Text,
                         out EnergyMix energyMix,
                         out String    ErrorResponse,
                         CustomEnergyMixParser))
            {
                return energyMix;
            }

            throw new ArgumentException("The given text representation of an energy mix is invalid: " + ErrorResponse, nameof(Text));

        }

        #endregion

        #region (static) TryParse(JSON, out EnergyMix, out ErrorResponse, CustomEnergyMixParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of an energy mix.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="EnergyMix">The parsed connector.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject        JSON,
                                       out EnergyMix  EnergyMix,
                                       out String     ErrorResponse)

            => TryParse(JSON,
                        out EnergyMix,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of an energy mix.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="EnergyMix">The parsed connector.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomEnergyMixParser">A delegate to parse custom energy mix JSON objects.</param>
        public static Boolean TryParse(JObject                                 JSON,
                                       out EnergyMix                           EnergyMix,
                                       out String                              ErrorResponse,
                                       CustomJObjectParserDelegate<EnergyMix>  CustomEnergyMixParser)
        {

            try
            {

                EnergyMix = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse IsGreenEnergy           [mandatory]

                if (!JSON.ParseMandatory("is_green_energy",
                                         "is green energy",
                                         out Boolean IsGreenEnergy,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse EnergySources           [optional]

                if (JSON.ParseOptionalJSON("energy_sources",
                                           "energy sources",
                                           EnergySource.TryParse,
                                           out IEnumerable<EnergySource> EnergySources,
                                           out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                }

                #endregion

                #region Parse EnvironmentalImpacts    [optional]

                if (JSON.ParseOptionalJSON("environ_impact",
                                           "environmental impacts",
                                           EnvironmentalImpact.TryParse,
                                           out IEnumerable<EnvironmentalImpact> EnvironmentalImpacts,
                                           out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                }

                #endregion

                #region Parse SupplierName            [optional]

                var SupplierName      = JSON.GetString("supplier_name");

                #endregion

                #region Parse EnergyProductName       [optional]

                var EnergyProductName = JSON.GetString("energy_product_name");

                #endregion


                EnergyMix = new EnergyMix(IsGreenEnergy,
                                          EnergySources,
                                          EnvironmentalImpacts,
                                          SupplierName,
                                          EnergyProductName);


                if (CustomEnergyMixParser is not null)
                    EnergyMix = CustomEnergyMixParser(JSON,
                                              EnergyMix);

                return true;

            }
            catch (Exception e)
            {
                EnergyMix          = default;
                ErrorResponse  = "The given JSON representation of an energy mix is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Text, out EnergyMix, out ErrorResponse, CustomEnergyMixParser = null)

        /// <summary>
        /// Try to parse the given text representation of an energy mix.
        /// </summary>
        /// <param name="Text">The text to parse.</param>
        /// <param name="EnergyMix">The parsed connector.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomEnergyMixParser">A delegate to parse custom energy mix JSON objects.</param>
        public static Boolean TryParse(String                                  Text,
                                       out EnergyMix                           EnergyMix,
                                       out String                              ErrorResponse,
                                       CustomJObjectParserDelegate<EnergyMix>  CustomEnergyMixParser   = null)
        {

            try
            {

                return TryParse(JObject.Parse(Text),
                                out EnergyMix,
                                out ErrorResponse,
                                CustomEnergyMixParser);

            }
            catch (Exception e)
            {
                EnergyMix          = default;
                ErrorResponse  = "The given text representation of an energy mix is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomEnergyMixSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomEnergyMixSerializer">A delegate to serialize custom hours JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<EnergyMix> CustomEnergyMixSerializer = null)
        {

            var JSON = JSONObject.Create(

                           new JProperty("is_green_energy",            IsGreenEnergy),

                           EnergySources.       SafeAny()
                               ? new JProperty("energy_sources",       new JArray(EnergySources.       Select(energysource        => energysource.       ToJSON())))
                               : null,

                           EnvironmentalImpacts.SafeAny()
                               ? new JProperty("environ_impact",       new JArray(EnvironmentalImpacts.Select(environmentalimpact => environmentalimpact.ToJSON())))
                               : null,

                           SupplierName.IsNotNullOrEmpty()
                               ? new JProperty("supplier_name",        SupplierName)
                               : null,

                           EnergyProductName.IsNotNullOrEmpty()
                               ? new JProperty("energy_product_name",  EnergyProductName)
                               : null

                       );

            return CustomEnergyMixSerializer is not null
                       ? CustomEnergyMixSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Operator overloading

        #region Operator == (EnergyMix1, EnergyMix2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergyMix1">An energy mix.</param>
        /// <param name="EnergyMix2">Another energy mix.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (EnergyMix EnergyMix1,
                                           EnergyMix EnergyMix2)
        {

            if (Object.ReferenceEquals(EnergyMix1, EnergyMix2))
                return true;

            if (EnergyMix1 is null || EnergyMix2 is null)
                return false;

            return EnergyMix1.Equals(EnergyMix2);

        }

        #endregion

        #region Operator != (EnergyMix1, EnergyMix2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergyMix1">An energy mix.</param>
        /// <param name="EnergyMix2">Another energy mix.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (EnergyMix EnergyMix1,
                                           EnergyMix EnergyMix2)

            => !(EnergyMix1 == EnergyMix2);

        #endregion

        #endregion

        #region IEquatable<EnergyMix> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is EnergyMix energyMix &&
                   Equals(energyMix);

        #endregion

        #region Equals(EnergyMix)

        /// <summary>
        /// Compares two energy mixs for equality.
        /// </summary>
        /// <param name="EnergyMix">A energy mix to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(EnergyMix EnergyMix)

            => !(EnergyMix is null) &&

               IsGreenEnergy.Equals(EnergyMix.IsGreenEnergy) &&

               EnergySources.       Count().Equals(EnergyMix.EnergySources.Count())                 &&
               EnvironmentalImpacts.Count().Equals(EnergyMix.EnvironmentalImpacts.Count())          &&
               EnergySources.       All(source =>  EnergyMix.EnergySources.       Contains(source)) &&
               EnvironmentalImpacts.All(impact =>  EnergyMix.EnvironmentalImpacts.Contains(impact)) &&

               ((SupplierName.     IsNullOrEmpty()         && EnergyMix.SupplierName.     IsNullOrEmpty()) ||
                (SupplierName.     IsNeitherNullNorEmpty() && EnergyMix.SupplierName.     IsNeitherNullNorEmpty() && SupplierName.     Equals(EnergyMix.SupplierName))) &&

               ((EnergyProductName.IsNullOrEmpty()         && EnergyMix.EnergyProductName.IsNullOrEmpty()) ||
                (EnergyProductName.IsNeitherNullNorEmpty() && EnergyMix.EnergyProductName.IsNeitherNullNorEmpty() && EnergyProductName.Equals(EnergyMix.EnergyProductName)));

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

                return IsGreenEnergy.      GetHashCode()       * 5 ^

                       EnergySources.       Aggregate(0, (hashCode, energySource)        => hashCode ^ energySource.       GetHashCode()) ^
                       EnvironmentalImpacts.Aggregate(0, (hashCode, environmentalImpact) => hashCode ^ environmentalImpact.GetHashCode()) ^

                       (SupplierName?.     GetHashCode() ?? 0) * 3 ^
                       (EnergyProductName?.GetHashCode() ?? 0);

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(IsGreenEnergy
                                 ? "Green energy"
                                 : "No green energy",
                             EnergyProductName.IsNotNullOrEmpty()
                                 ? " (" + EnergyProductName + ")"
                                 : "",
                             SupplierName.IsNotNullOrEmpty()
                                 ? " from " + SupplierName
                                 : "");

        #endregion

    }

}
