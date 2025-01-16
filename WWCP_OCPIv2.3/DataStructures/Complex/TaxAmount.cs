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
    /// A tax amount.
    /// </summary>
    public class TaxAmount : IEquatable<TaxAmount>,
                             IComparable<TaxAmount>,
                             IComparable
    {

        #region Properties

        /// <summary>
        /// A description of the tax. In countries where a tax name is required like Canada this can be something like "QST".
        /// In countries where this is not required, this can be something more generic like "VAT" or "General Sales Tax".
        /// </summary>
        [Mandatory]
        public String    Name             { get; }

        /// <summary>
        /// The amount of money of this tax that is due.
        /// </summary>
        [Mandatory]
        public Decimal   Amount           { get; }

        /// <summary>
        /// Optional Tax Account Number of the business entity remitting these taxes.
        /// Optional as this is not required in all countries.
        /// </summary>
        [Optional]
        public String?   AccountNumber    { get; }

        /// <summary>
        /// Tax percentage. Optional as this is not required in all countries.
        /// </summary>
        [Optional]
        public Decimal?  Percentage       { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new tax amount.
        /// </summary>
        /// <param name="Name">A description of the tax. In countries where a tax name is required like Canada this can be something like "QST". In countries where this is not required, this can be something more generic like "VAT" or "General Sales Tax".</param>
        /// <param name="Amount">The amount of money of this tax that is due.</param>
        /// <param name="AccountNumber">Optional Tax Account Number of the business entity remitting these taxes. Optional as this is not required in all countries.</param>
        /// <param name="Percentage">Tax percentage. Optional as this is not required in all countries.</param>
        public TaxAmount(String    Name,
                         Decimal   Amount,
                         String?   AccountNumber   = null,
                         Decimal?  Percentage      = null)
        {

            this.Name           = Name;
            this.Amount         = Amount;
            this.AccountNumber  = AccountNumber;
            this.Percentage     = Percentage;

            unchecked
            {

                this.hashCode = this.Name.          GetHashCode()       * 7 ^
                                this.Amount.        GetHashCode()       * 5 ^
                               (this.AccountNumber?.GetHashCode() ?? 0) * 3 ^
                                this.Percentage?.   GetHashCode() ?? 0;

            }

        }

        #endregion


        #region (static) Parse   (JSON, CustomTaxAmountParser = null)

        /// <summary>
        /// Parse the given JSON representation of a tax amount.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomTaxAmountParser">A delegate to parse custom tax amount JSON objects.</param>
        public static TaxAmount Parse(JObject                                       JSON,
                                           CustomJObjectParserDelegate<TaxAmount>?  CustomTaxAmountParser   = null)
        {

            if (TryParse(JSON,
                         out var taxAmount,
                         out var errorResponse,
                         CustomTaxAmountParser))
            {
                return taxAmount;
            }

            throw new ArgumentException("The given JSON representation of a tax amount is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out TaxAmount, out ErrorResponse, CustomTaxAmountParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a tax amount.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="TaxAmount">The parsed tax amount.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                              JSON,
                                       [NotNullWhen(true)]  out TaxAmount?  TaxAmount,
                                       [NotNullWhen(false)] out String?     ErrorResponse)

            => TryParse(JSON,
                        out TaxAmount,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a tax amount.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="TaxAmount">The parsed tax amount.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomTaxAmountParser">A delegate to parse custom tax amount JSON objects.</param>
        public static Boolean TryParse(JObject                                  JSON,
                                       [NotNullWhen(true)]  out TaxAmount?      TaxAmount,
                                       [NotNullWhen(false)] out String?         ErrorResponse,
                                       CustomJObjectParserDelegate<TaxAmount>?  CustomTaxAmountParser   = null)
        {

            try
            {

                TaxAmount = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse Name             [mandatory]

                if (!JSON.ParseMandatoryText("name",
                                             "tax name",
                                             out String? Name,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Amount           [mandatory]

                if (!JSON.ParseMandatory("amount",
                                         "tax amount",
                                         out Decimal Amount,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse AccountNumber    [optional]

                var AccountNumber = JSON.GetString("account_number");

                #endregion

                #region Parse Percentage       [optional]

                if (JSON.ParseOptional("percentage",
                                       "tax percentage",
                                       out Decimal? Percentage,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                TaxAmount = new TaxAmount(
                                Name,
                                Amount,
                                AccountNumber,
                                Percentage
                            );


                if (CustomTaxAmountParser is not null)
                    TaxAmount = CustomTaxAmountParser(JSON,
                                                      TaxAmount);

                return true;

            }
            catch (Exception e)
            {
                TaxAmount      = default;
                ErrorResponse  = "The given JSON representation of a tax amount is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomTaxAmountSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomTaxAmountSerializer">A delegate to serialize custom tax amount JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<TaxAmount>? CustomTaxAmountSerializer = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("name",             Name),
                                 new JProperty("amount",           Amount),

                           AccountNumber.IsNotNullOrEmpty()
                               ? new JProperty("account_number",   AccountNumber)
                               : null,

                           Percentage.HasValue
                               ? new JProperty("percentage",       Percentage.Value)
                               : null

                       );

            return CustomTaxAmountSerializer is not null
                       ? CustomTaxAmountSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this object.
        /// </summary>
        public TaxAmount Clone()

            => new (
                   new String(Name.ToCharArray()),
                   Amount,
                   AccountNumber is not null
                       ? new String(AccountNumber.ToCharArray())
                       : null,
                   Percentage
               );

        #endregion


        #region Operator overloading

        #region Operator == (TaxAmount1, TaxAmount2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TaxAmount1">A tax amount.</param>
        /// <param name="TaxAmount2">Another tax amount.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (TaxAmount TaxAmount1,
                                           TaxAmount TaxAmount2)

            => TaxAmount1.Equals(TaxAmount2);

        #endregion

        #region Operator != (TaxAmount1, TaxAmount2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TaxAmount1">A tax amount.</param>
        /// <param name="TaxAmount2">Another tax amount.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (TaxAmount TaxAmount1,
                                           TaxAmount TaxAmount2)

            => !TaxAmount1.Equals(TaxAmount2);

        #endregion

        #region Operator <  (TaxAmount1, TaxAmount2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TaxAmount1">A tax amount.</param>
        /// <param name="TaxAmount2">Another tax amount.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (TaxAmount TaxAmount1,
                                          TaxAmount TaxAmount2)

            => TaxAmount1.CompareTo(TaxAmount2) < 0;

        #endregion

        #region Operator <= (TaxAmount1, TaxAmount2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TaxAmount1">A tax amount.</param>
        /// <param name="TaxAmount2">Another tax amount.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (TaxAmount TaxAmount1,
                                           TaxAmount TaxAmount2)

            => TaxAmount1.CompareTo(TaxAmount2) <= 0;

        #endregion

        #region Operator >  (TaxAmount1, TaxAmount2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TaxAmount1">A tax amount.</param>
        /// <param name="TaxAmount2">Another tax amount.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (TaxAmount TaxAmount1,
                                          TaxAmount TaxAmount2)

            => TaxAmount1.CompareTo(TaxAmount2) > 0;

        #endregion

        #region Operator >= (TaxAmount1, TaxAmount2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TaxAmount1">A tax amount.</param>
        /// <param name="TaxAmount2">Another tax amount.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (TaxAmount TaxAmount1,
                                           TaxAmount TaxAmount2)

            => TaxAmount1.CompareTo(TaxAmount2) >= 0;

        #endregion

        #endregion

        #region IComparable<TaxAmount> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two tax amounts.
        /// </summary>
        /// <param name="Object">A tax amount to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is TaxAmount taxAmount
                   ? CompareTo(taxAmount)
                   : throw new ArgumentException("The given object is not a tax amount!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(TaxAmount)

        /// <summary>
        /// Compares two tax amounts.
        /// </summary>
        /// <param name="TaxAmount">A tax amount to compare with.</param>
        public Int32 CompareTo(TaxAmount? TaxAmount)
        {

            if (TaxAmount is null)
                throw new ArgumentNullException(nameof(TaxAmount), "The given tax amount must not be null!");

            var c = Name.CompareTo(TaxAmount.Name);

            if (c == 0)
                c = Amount.CompareTo(TaxAmount.Amount);

            if (c == 0)
                c = AccountNumber?.CompareTo(TaxAmount.AccountNumber) ?? (TaxAmount.AccountNumber is null ? 0 : -1);

            if (c == 0 && Percentage.HasValue && TaxAmount.Percentage.HasValue)
                c = Percentage.Value.CompareTo(TaxAmount.Percentage.Value);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<TaxAmount> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two tax amounts for equality.
        /// </summary>
        /// <param name="Object">A tax amount to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is TaxAmount taxAmount &&
                   Equals(taxAmount);

        #endregion

        #region Equals(TaxAmount)

        /// <summary>
        /// Compares two tax amounts for equality.
        /// </summary>
        /// <param name="TaxAmount">A tax amount to compare with.</param>
        public Boolean Equals(TaxAmount? TaxAmount)

           => TaxAmount is not null &&
              Name.          Equals(TaxAmount.Name)   &&
              Amount.        Equals(TaxAmount.Amount) &&
             (AccountNumber?.Equals(TaxAmount.AccountNumber) ?? TaxAmount.AccountNumber is null) &&
             (Percentage?.   Equals(TaxAmount.Percentage)    ?? TaxAmount.Percentage    is null);

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

                   Name,   ", ",
                   Amount, ", ",

                   AccountNumber.IsNotNullOrEmpty()
                       ? ", " + AccountNumber
                       : "",

                   Percentage.HasValue
                       ? ", " + Percentage.Value + "%"
                       : ""

               );

        #endregion

    }

}
