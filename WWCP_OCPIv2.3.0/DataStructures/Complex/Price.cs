/*
 * Copyright (c) 2015-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
    /// A price.
    /// </summary>
    public class Price : IEquatable<Price>,
                         IComparable<Price>,
                         IComparable
    {

        #region Properties

        /// <summary>
        /// Price/Cost excluding taxes.
        /// </summary>
        [Mandatory]
        public Decimal                 BeforeTaxes    { get; }

        /// <summary>
        /// All taxes that are applicable to this price and relevant to the receiver of the Session or CDR.
        /// </summary>
        [Optional]
        public IEnumerable<TaxAmount>  Taxes          { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new price.
        /// </summary>
        /// <param name="BeforeTaxes">Price/Cost excluding taxes.</param>
        /// <param name="Taxes">All taxes that are applicable to this price and relevant to the receiver of the Session or CDR.</param>
        public Price(Decimal                  BeforeTaxes,
                     IEnumerable<TaxAmount>?  Taxes   = null)
        {

            this.BeforeTaxes  = BeforeTaxes;
            this.Taxes        = Taxes ?? [];

            unchecked
            {

                this.hashCode = this.BeforeTaxes.GetHashCode() * 3 ^
                                this.Taxes.      CalcHashCode();

            }

        }

        #endregion


        #region (static) Parse   (JSON, CustomPriceParser = null)

        /// <summary>
        /// Parse the given JSON representation of a price.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomPriceParser">A delegate to parse custom price JSON objects.</param>
        public static Price Parse(JObject                              JSON,
                                  CustomJObjectParserDelegate<Price>?  CustomPriceParser   = null)
        {

            if (TryParse(JSON,
                         out var price,
                         out var errorResponse,
                         CustomPriceParser))
            {
                return price;
            }

            throw new ArgumentException("The given JSON representation of a price is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out Price, out ErrorResponse, CustomPriceParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a price.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="Price">The parsed price.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                           JSON,
                                       [NotNullWhen(true)]  out Price?   Price,
                                       [NotNullWhen(false)] out String?  ErrorResponse)

            => TryParse(JSON,
                        out Price,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a price.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="Price">The parsed price.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomPriceParser">A delegate to parse custom price JSON objects.</param>
        public static Boolean TryParse(JObject                              JSON,
                                       [NotNullWhen(true)]  out Price?      Price,
                                       [NotNullWhen(false)] out String?     ErrorResponse,
                                       CustomJObjectParserDelegate<Price>?  CustomPriceParser   = null)
        {

            try
            {

                Price = default;

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

                #region Parse Taxes          [optional]

                if (JSON.ParseOptionalHashSet("taxes",
                                              "taxes",
                                              TaxAmount.TryParse,
                                              out HashSet<TaxAmount> Taxes,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                Price = new Price(
                            BeforeTaxes,
                            Taxes
                        );


                if (CustomPriceParser is not null)
                    Price = CustomPriceParser(JSON,
                                              Price);

                return true;

            }
            catch (Exception e)
            {
                Price          = default;
                ErrorResponse  = "The given JSON representation of a price is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomPriceSerializer = null, CustomTaxAmountSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomPriceSerializer">A delegate to serialize custom price JSON objects.</param>
        /// <param name="CustomTaxAmountSerializer">A delegate to serialize custom tax amount JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<Price>?      CustomPriceSerializer       = null,
                              CustomJObjectSerializerDelegate<TaxAmount>?  CustomTaxAmountSerializer   = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("before_taxes",   BeforeTaxes),

                           Taxes.Any()
                               ? new JProperty("taxes",          new JArray(Taxes.Select(taxes => taxes.ToJSON(CustomTaxAmountSerializer))))
                               : null

                       );

            return CustomPriceSerializer is not null
                       ? CustomPriceSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this object.
        /// </summary>
        public Price Clone()

            => new (
                   BeforeTaxes,
                   Taxes?.Select(taxes => taxes.Clone())
               );

        #endregion


        #region Operator overloading

        #region Operator == (Price1, Price2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Price1">A price.</param>
        /// <param name="Price2">Another price.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Price Price1,
                                           Price Price2)

            => Price1.Equals(Price2);

        #endregion

        #region Operator != (Price1, Price2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Price1">A price.</param>
        /// <param name="Price2">Another price.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Price Price1,
                                           Price Price2)

            => !Price1.Equals(Price2);

        #endregion

        #region Operator <  (Price1, Price2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Price1">A price.</param>
        /// <param name="Price2">Another price.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Price Price1,
                                          Price Price2)

            => Price1.CompareTo(Price2) < 0;

        #endregion

        #region Operator <= (Price1, Price2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Price1">A price.</param>
        /// <param name="Price2">Another price.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Price Price1,
                                           Price Price2)

            => Price1.CompareTo(Price2) <= 0;

        #endregion

        #region Operator >  (Price1, Price2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Price1">A price.</param>
        /// <param name="Price2">Another price.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Price Price1,
                                          Price Price2)

            => Price1.CompareTo(Price2) > 0;

        #endregion

        #region Operator >= (Price1, Price2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Price1">A price.</param>
        /// <param name="Price2">Another price.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Price Price1,
                                           Price Price2)

            => Price1.CompareTo(Price2) >= 0;

        #endregion

        #endregion

        #region IComparable<Price> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two prices.
        /// </summary>
        /// <param name="Object">A price to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Price price
                   ? CompareTo(price)
                   : throw new ArgumentException("The given object is not a price!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(Price)

        /// <summary>
        /// Compares two prices.
        /// </summary>
        /// <param name="Price">A price to compare with.</param>
        public Int32 CompareTo(Price? Price)
        {

            if (Price is null)
                throw new ArgumentNullException(nameof(Price), "The given price must not be null!");

            var c = BeforeTaxes.CompareTo(Price.BeforeTaxes);

            if (c == 0 && Taxes.Any() && Price.Taxes.Any())
            {
                var thisTaxes = Taxes.OrderBy(t => t.Name).ToList();
                var otherTaxes = Price.Taxes.OrderBy(t => t.Name).ToList();

                for (int i = 0; i < thisTaxes.Count && i < otherTaxes.Count; i++)
                {
                    c = thisTaxes[i].CompareTo(otherTaxes[i]);
                    if (c != 0)
                        break;
                }

                if (c == 0)
                    c = thisTaxes.Count.CompareTo(otherTaxes.Count);
            }

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<Price> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two prices for equality.
        /// </summary>
        /// <param name="Object">A price to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Price price &&
                   Equals(price);

        #endregion

        #region Equals(Price)

        /// <summary>
        /// Compares two prices for equality.
        /// </summary>
        /// <param name="Price">A price to compare with.</param>
        public Boolean Equals(Price? Price)

            => Price is not null &&

               BeforeTaxes.Equals(Price.BeforeTaxes) &&

            ((!Taxes.Any() && !Price.Taxes.Any()) ||
              (Taxes.Any() &&  Price.Taxes.Any() && Taxes.SequenceEqual(Price.Taxes)));

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

                   BeforeTaxes,

                   Taxes.Any()
                       ? ", " + Taxes.AggregateWith(", ")
                       : ""

               );

        #endregion

    }

}
