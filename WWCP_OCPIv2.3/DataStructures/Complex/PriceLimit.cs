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

#endregion

namespace cloud.charging.open.protocols.OCPIv2_3_0
{

    /// <summary>
    /// A price limit.
    /// </summary>
    /// <param name="BeforeTaxes">Maximum or minimum cost excluding taxes.</param>
    /// <param name="AfterTaxes">Optional maximum or minimum cost including taxes.</param>
    public readonly struct PriceLimit(Decimal   BeforeTaxes,
                                      Decimal?  AfterTaxes) : IEquatable<PriceLimit>,
                                                              IComparable<PriceLimit>,
                                                              IComparable
    {

        #region Properties

        /// <summary>
        /// Maximum or minimum cost excluding taxes.
        /// </summary>
        [Mandatory]
        public Decimal   BeforeTaxes    { get; } = BeforeTaxes;

        /// <summary>
        /// Optional maximum or minimum cost including taxes.
        /// </summary>
        [Optional]
        public Decimal?  AfterTaxes     { get; } = AfterTaxes;

        #endregion


        #region (static) Parse   (JSON, CustomPriceLimitParser = null)

        /// <summary>
        /// Parse the given JSON representation of a price limit.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomPriceLimitParser">A delegate to parse custom price limit JSON objects.</param>
        public static PriceLimit Parse(JObject                                   JSON,
                                       CustomJObjectParserDelegate<PriceLimit>?  CustomPriceLimitParser   = null)
        {

            if (TryParse(JSON,
                         out var priceLimit,
                         out var errorResponse,
                         CustomPriceLimitParser))
            {
                return priceLimit;
            }

            throw new ArgumentException("The given JSON representation of a price limit is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, CustomPriceLimitParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a price limit.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomPriceLimitParser">A delegate to parse custom price limit JSON objects.</param>
        public static PriceLimit? TryParse(JObject                                   JSON,
                                           CustomJObjectParserDelegate<PriceLimit>?  CustomPriceLimitParser   = null)
        {

            if (TryParse(JSON,
                         out var priceLimit,
                         out var errorResponse,
                         CustomPriceLimitParser))
            {
                return priceLimit;
            }

            return default;

        }

        #endregion

        #region (static) TryParse(JSON, out PriceLimit, out ErrorResponse, CustomPriceLimitParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a price limit.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="PriceLimit">The parsed price limit.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                              JSON,
                                       [NotNullWhen(true)]  out PriceLimit  PriceLimit,
                                       [NotNullWhen(false)] out String?     ErrorResponse)

            => TryParse(JSON,
                        out PriceLimit,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a price limit.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="PriceLimit">The parsed price limit.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomPriceLimitParser">A delegate to parse custom price limit JSON objects.</param>
        public static Boolean TryParse(JObject                                   JSON,
                                       [NotNullWhen(true)]  out PriceLimit       PriceLimit,
                                       [NotNullWhen(false)] out String?          ErrorResponse,
                                       CustomJObjectParserDelegate<PriceLimit>?  CustomPriceLimitParser   = null)
        {

            try
            {

                PriceLimit = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse BeforeTaxes    [mandatory]

                if (!JSON.ParseMandatory("before_taxes",
                                         "before taxes",
                                         out Decimal BeforeTaxes,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse AfterTaxes     [optional]

                if (JSON.ParseOptional("after_taxes",
                                       "after taxes",
                                       out Decimal? AfterTaxes,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                PriceLimit = new PriceLimit(
                                 BeforeTaxes,
                                 AfterTaxes
                             );


                if (CustomPriceLimitParser is not null)
                    PriceLimit = CustomPriceLimitParser(JSON,
                                                        PriceLimit);

                return true;

            }
            catch (Exception e)
            {
                PriceLimit     = default;
                ErrorResponse  = "The given JSON representation of a price limit is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomPriceLimitSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomPriceLimitSerializer">A delegate to serialize custom price limit JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<PriceLimit>? CustomPriceLimitSerializer = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("before_taxes",   BeforeTaxes),

                           AfterTaxes.HasValue
                               ? new JProperty("after_taxes",    AfterTaxes.Value)
                               : null

                       );

            return CustomPriceLimitSerializer is not null
                       ? CustomPriceLimitSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this object.
        /// </summary>
        public PriceLimit Clone()

            => new (
                   BeforeTaxes,
                   AfterTaxes
               );

        #endregion


        #region Operator overloading

        #region Operator == (PriceLimit1, PriceLimit2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PriceLimit1">A price limit.</param>
        /// <param name="PriceLimit2">Another price limit.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (PriceLimit PriceLimit1,
                                           PriceLimit PriceLimit2)

            => PriceLimit1.Equals(PriceLimit2);

        #endregion

        #region Operator != (PriceLimit1, PriceLimit2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PriceLimit1">A price limit.</param>
        /// <param name="PriceLimit2">Another price limit.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (PriceLimit PriceLimit1,
                                           PriceLimit PriceLimit2)

            => !PriceLimit1.Equals(PriceLimit2);

        #endregion

        #region Operator <  (PriceLimit1, PriceLimit2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PriceLimit1">A price limit.</param>
        /// <param name="PriceLimit2">Another price limit.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (PriceLimit PriceLimit1,
                                          PriceLimit PriceLimit2)

            => PriceLimit1.CompareTo(PriceLimit2) < 0;

        #endregion

        #region Operator <= (PriceLimit1, PriceLimit2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PriceLimit1">A price limit.</param>
        /// <param name="PriceLimit2">Another price limit.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (PriceLimit PriceLimit1,
                                           PriceLimit PriceLimit2)

            => PriceLimit1.CompareTo(PriceLimit2) <= 0;

        #endregion

        #region Operator >  (PriceLimit1, PriceLimit2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PriceLimit1">A price limit.</param>
        /// <param name="PriceLimit2">Another price limit.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (PriceLimit PriceLimit1,
                                          PriceLimit PriceLimit2)

            => PriceLimit1.CompareTo(PriceLimit2) > 0;

        #endregion

        #region Operator >= (PriceLimit1, PriceLimit2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PriceLimit1">A price limit.</param>
        /// <param name="PriceLimit2">Another price limit.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (PriceLimit PriceLimit1,
                                           PriceLimit PriceLimit2)

            => PriceLimit1.CompareTo(PriceLimit2) >= 0;

        #endregion

        #endregion

        #region IComparable<PriceLimit> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two price limits.
        /// </summary>
        /// <param name="Object">A price limit to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is PriceLimit priceLimit
                   ? CompareTo(priceLimit)
                   : throw new ArgumentException("The given object is not a price limit!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(PriceLimit)

        /// <summary>
        /// Compares two price limits.
        /// </summary>
        /// <param name="PriceLimit">A price limit to compare with.</param>
        public Int32 CompareTo(PriceLimit PriceLimit)
        {

            var c = BeforeTaxes.CompareTo(PriceLimit.BeforeTaxes);

            if (c == 0 && AfterTaxes.HasValue && PriceLimit.AfterTaxes.HasValue)
                c = AfterTaxes.Value.CompareTo(PriceLimit.AfterTaxes.Value);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<PriceLimit> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two price limits for equality.
        /// </summary>
        /// <param name="Object">A price limit to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is PriceLimit priceLimit &&
                   Equals(priceLimit);

        #endregion

        #region Equals(PriceLimit)

        /// <summary>
        /// Compares two price limits for equality.
        /// </summary>
        /// <param name="PriceLimit">A price limit to compare with.</param>
        public Boolean Equals(PriceLimit PriceLimit)

            => BeforeTaxes.Equals(PriceLimit.BeforeTaxes) &&

            ((!AfterTaxes.HasValue && !PriceLimit.AfterTaxes.HasValue) ||
              (AfterTaxes.HasValue &&  PriceLimit.AfterTaxes.HasValue && AfterTaxes.Equals(PriceLimit.AfterTaxes)));

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return BeforeTaxes.GetHashCode() * 3 ^
                       AfterTaxes?.GetHashCode() ?? 0;

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   BeforeTaxes,

                   AfterTaxes.HasValue
                       ? ", " + AfterTaxes.Value
                       : ""

               );

        #endregion

    }

}
