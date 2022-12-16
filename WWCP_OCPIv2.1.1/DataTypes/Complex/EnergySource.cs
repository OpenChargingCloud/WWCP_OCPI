/*
 * Copyright (c) 2015-2022 GraphDefined GmbH
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
    /// The energy source.
    /// </summary>
    public readonly struct EnergySource : IEquatable<EnergySource>,
                                          IComparable<EnergySource>,
                                          IComparable
    {

        #region Properties

        /// <summary>
        /// The energy source.
        /// </summary>
        [Mandatory]
        public EnergySourceCategories  Source        { get; }

        /// <summary>
        /// The percentage of this energy source.
        /// </summary>
        [Mandatory]
        public Double                  Percentage    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// A single energy source.
        /// </summary>
        /// <param name="Source">The energy source.</param>
        /// <param name="Percentage">The percentage of this energy source.</param>
        public EnergySource(EnergySourceCategories  Source,
                            Double                  Percentage)
        {

            this.Source      = Source;
            this.Percentage  = Percentage;

        }

        #endregion


        #region (static) Parse   (JSON, CustomEnergySourceParser = null)

        /// <summary>
        /// Parse the given JSON representation of an energy source.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomEnergySourceParser">A delegate to parse custom energy source JSON objects.</param>
        public static EnergySource Parse(JObject                                    JSON,
                                         CustomJObjectParserDelegate<EnergySource>  CustomEnergySourceParser   = null)
        {

            if (TryParse(JSON,
                         out EnergySource energySource,
                         out String       ErrorResponse,
                         CustomEnergySourceParser))
            {
                return energySource;
            }

            throw new ArgumentException("The given JSON representation of an energy source is invalid: " + ErrorResponse, nameof(JSON));

        }

        #endregion

        #region (static) Parse   (Text, CustomEnergySourceParser = null)

        /// <summary>
        /// Parse the given text representation of an energy source.
        /// </summary>
        /// <param name="Text">The text to parse.</param>
        /// <param name="CustomEnergySourceParser">A delegate to parse custom energy source JSON objects.</param>
        public static EnergySource Parse(String                                     Text,
                                         CustomJObjectParserDelegate<EnergySource>  CustomEnergySourceParser   = null)
        {

            if (TryParse(Text,
                         out EnergySource energySource,
                         out String       ErrorResponse,
                         CustomEnergySourceParser))
            {
                return energySource;
            }

            throw new ArgumentException("The given text representation of an energy source is invalid: " + ErrorResponse, nameof(Text));

        }

        #endregion

        #region (static) TryParse(JSON, out EnergySource, out ErrorResponse, CustomEnergySourceParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of an energy source.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="EnergySource">The parsed connector.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject           JSON,
                                       out EnergySource  EnergySource,
                                       out String        ErrorResponse)

            => TryParse(JSON,
                        out EnergySource,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of an energy source.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="EnergySource">The parsed connector.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomEnergySourceParser">A delegate to parse custom energy source JSON objects.</param>
        public static Boolean TryParse(JObject                                    JSON,
                                       out EnergySource                           EnergySource,
                                       out String                                 ErrorResponse,
                                       CustomJObjectParserDelegate<EnergySource>  CustomEnergySourceParser)
        {

            try
            {

                EnergySource = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse EnergySourceCategory    [mandatory]

                if (!JSON.ParseMandatoryEnum("source",
                                             "energy source",
                                             out EnergySourceCategories EnergySourceCategory,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Percentage              [mandatory]

                if (!JSON.ParseMandatory("percentage",
                                         "percentage",
                                         out Double Percentage,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                EnergySource = new EnergySource(EnergySourceCategory,
                                                Percentage);


                if (CustomEnergySourceParser is not null)
                    EnergySource = CustomEnergySourceParser(JSON,
                                                            EnergySource);

                return true;

            }
            catch (Exception e)
            {
                EnergySource   = default;
                ErrorResponse  = "The given JSON representation of an energy source is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Text, out EnergySource, out ErrorResponse, CustomEnergySourceParser = null)

        /// <summary>
        /// Try to parse the given text representation of an energy source.
        /// </summary>
        /// <param name="Text">The text to parse.</param>
        /// <param name="EnergySource">The parsed connector.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomEnergySourceParser">A delegate to parse custom energy source JSON objects.</param>
        public static Boolean TryParse(String                                     Text,
                                       out EnergySource                           EnergySource,
                                       out String                                 ErrorResponse,
                                       CustomJObjectParserDelegate<EnergySource>  CustomEnergySourceParser   = null)
        {

            try
            {

                return TryParse(JObject.Parse(Text),
                                out EnergySource,
                                out ErrorResponse,
                                CustomEnergySourceParser);

            }
            catch (Exception e)
            {
                EnergySource = default;
                ErrorResponse  = "The given text representation of an energy source is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomEnergySourceSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomEnergySourceSerializer">A delegate to serialize custom energy source JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<EnergySource> CustomEnergySourceSerializer = null)
        {

            var JSON = JSONObject.Create(
                           new JProperty("source",      Source.ToString()),
                           new JProperty("percentage",  Percentage)
                       );

            return CustomEnergySourceSerializer is not null
                       ? CustomEnergySourceSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Operator overloading

        #region Operator == (EnergySource1, EnergySource2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergySource1">An energy source.</param>
        /// <param name="EnergySource2">Another energy source.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (EnergySource EnergySource1,
                                           EnergySource EnergySource2)

            => EnergySource1.Equals(EnergySource2);

        #endregion

        #region Operator != (EnergySource1, EnergySource2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergySource1">An energy source.</param>
        /// <param name="EnergySource2">Another energy source.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (EnergySource EnergySource1,
                                           EnergySource EnergySource2)

            => !(EnergySource1 == EnergySource2);

        #endregion

        #region Operator <  (EnergySource1, EnergySource2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergySource1">An energy source.</param>
        /// <param name="EnergySource2">Another energy source.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (EnergySource EnergySource1,
                                          EnergySource EnergySource2)

            => EnergySource1.CompareTo(EnergySource2) < 0;

        #endregion

        #region Operator <= (EnergySource1, EnergySource2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergySource1">An energy source.</param>
        /// <param name="EnergySource2">Another energy source.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (EnergySource EnergySource1,
                                           EnergySource EnergySource2)

            => !(EnergySource1 > EnergySource2);

        #endregion

        #region Operator >  (EnergySource1, EnergySource2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergySource1">An energy source.</param>
        /// <param name="EnergySource2">Another energy source.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (EnergySource EnergySource1,
                                          EnergySource EnergySource2)

            => EnergySource1.CompareTo(EnergySource2) > 0;

        #endregion

        #region Operator >= (EnergySource1, EnergySource2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergySource1">An energy source.</param>
        /// <param name="EnergySource2">Another energy source.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (EnergySource EnergySource1,
                                           EnergySource EnergySource2)

            => !(EnergySource1 < EnergySource2);

        #endregion

        #endregion

        #region IComparable<EnergySource> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is EnergySource energySource
                   ? CompareTo(energySource)
                   : throw new ArgumentException("The given object is not an energy source!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(EnergySource)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergySource">An object to compare with.</param>
        public Int32 CompareTo(EnergySource EnergySource)
        {

            var c = Source.CompareTo(EnergySource.Source);

            if (c == 0)
                c = Percentage.CompareTo(EnergySource.Percentage);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<EnergySource> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is EnergySource energySource &&
                   Equals(energySource);

        #endregion

        #region Equals(EnergySource)

        /// <summary>
        /// Compares two energy sources for equality.
        /// </summary>
        /// <param name="EnergySource">A energy source to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(EnergySource EnergySource)

            => Source.    Equals(EnergySource.Source) &&
               Percentage.Equals(EnergySource.Percentage);

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

                return Source.    GetHashCode() * 3 ^
                       Percentage.GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(Percentage, "% ",
                             Source);

        #endregion

    }

}
