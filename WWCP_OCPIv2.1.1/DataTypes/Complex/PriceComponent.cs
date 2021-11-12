/*
 * Copyright (c) 2015-2021 GraphDefined GmbH
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
    /// A price component defines the pricing of a tariff.
    /// </summary>
    public readonly struct PriceComponent : IEquatable<PriceComponent>,
                                            IComparable<PriceComponent>,
                                            IComparable
    {

        #region Properties

        /// <summary>
        /// Type of tariff dimension.
        /// </summary>
        [Mandatory]
        public TariffDimensions  Type           { get; }

        /// <summary>
        /// Price per unit (excl. VAT) for this tariff dimension.
        /// </summary>
        [Mandatory]
        public Decimal           Price          { get; }

        /// <summary>
        /// Applicable VAT percentage for this tariff dimension. If omitted, no VAT is applicable.
        /// Not providing a VAT is different from 0% VAT, which would be a value of 0.0 here.
        /// </summary>
        [Optional]
        public Decimal?          VAT            { get; }

        /// <summary>
        /// Minimum amount to be billed. This unit will be billed in this step_size blocks.
        /// </summary>
        /// <example>
        /// If type is time and step_size is 300, then time will be billed in blocks of 5 minutes,
        /// so if 6 minutes is used, 10 minutes (2 blocks of step_size) will be billed.
        /// </example>
        [Mandatory]
        public UInt32            StepSize       { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new price component defining the pricing of a tariff.
        /// </summary>
        /// <param name="Type">Type of tariff dimension.</param>
        /// <param name="Price">Price per unit (excl. VAT) for this tariff dimension.</param>
        /// <param name="VAT_percentage">Applicable VAT percentage for this tariff dimension. If omitted, no VAT is applicable. Not providing a VAT is different from 0% VAT, which would be a value of 0.0 here.</param>
        /// <param name="StepSize">Minimum amount to be billed. This unit will be billed in this step_size blocks.</param>
        public PriceComponent(TariffDimensions  Type,
                              Decimal           Price,
                              Decimal?          VAT_percentage   = null,
                              UInt32            StepSize         = 1)
        {

            this.Type            = Type;
            this.Price           = Price;
            this.VAT  = VAT_percentage;
            this.StepSize        = StepSize;

        }

        #endregion


        #region Flat                          (Price, VAT_percentage = null)

        /// <summary>
        /// Create a new flat rate price component.
        /// </summary>
        /// <param name="Price">Flat rate price (excl. VAT).</param>
        /// <param name="VAT_percentage">Applicable VAT percentage for this tariff dimension. If omitted, no VAT is applicable. Not providing a VAT is different from 0% VAT, which would be a value of 0.0 here.</param>
        public static PriceComponent FlatRate(Decimal   Price,
                                              Decimal?  VAT_percentage = null)

            => new PriceComponent(TariffDimensions.FLAT,
                                  Price,
                                  VAT_percentage,
                                  1);

        #endregion

        #region ChargingTime(BillingIncrement, Price, VAT_percentage = null)

        /// <summary>
        /// Create a new time-based charging price component.
        /// </summary>
        /// <param name="BillingIncrement">The minimum granularity of time in seconds that you will be billed.</param>
        /// <param name="Price">Price per time span (excl. VAT).</param>
        /// <param name="VAT_percentage">Applicable VAT percentage for this tariff dimension. If omitted, no VAT is applicable. Not providing a VAT is different from 0% VAT, which would be a value of 0.0 here.</param>
        public static PriceComponent ChargingTime(TimeSpan  BillingIncrement,
                                                  Decimal   Price,
                                                  Decimal?  VAT_percentage = null)

            => new PriceComponent(TariffDimensions.TIME,
                                  Price,
                                  VAT_percentage,
                                  (UInt32) Math.Round(BillingIncrement.TotalSeconds, 0));

        #endregion

        #region ParkingTime (BillingIncrement, Price, VAT_percentage = null)

        /// <summary>
        /// Create a new time-based parking price component.
        /// </summary>
        /// <param name="BillingIncrement">The minimum granularity of time in seconds that you will be billed.</param>
        /// <param name="Price">Price per time span (excl. VAT).</param>
        /// <param name="VAT_percentage">Applicable VAT percentage for this tariff dimension. If omitted, no VAT is applicable. Not providing a VAT is different from 0% VAT, which would be a value of 0.0 here.</param>
        public static PriceComponent ParkingTime(TimeSpan  BillingIncrement,
                                                 Decimal   Price,
                                                 Decimal?  VAT_percentage = null)

            => new PriceComponent(TariffDimensions.PARKING_TIME,
                                  Price,
                                  VAT_percentage,
                                  (UInt32) Math.Round(BillingIncrement.TotalSeconds, 0));

        #endregion


        #region (static) Parse   (JSON, CustomPriceComponentParser = null)

        /// <summary>
        /// Parse the given JSON representation of a price component.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomPriceComponentParser">A delegate to parse custom price component JSON objects.</param>
        public static PriceComponent Parse(JObject                                      JSON,
                                           CustomJObjectParserDelegate<PriceComponent>  CustomPriceComponentParser   = null)
        {

            if (TryParse(JSON,
                         out PriceComponent  priceComponent,
                         out String          ErrorResponse,
                         CustomPriceComponentParser))
            {
                return priceComponent;
            }

            throw new ArgumentException("The given JSON representation of a price component is invalid: " + ErrorResponse, nameof(JSON));

        }

        #endregion

        #region (static) Parse   (Text, CustomPriceComponentParser = null)

        /// <summary>
        /// Parse the given text representation of a price component.
        /// </summary>
        /// <param name="Text">The text to parse.</param>
        /// <param name="CustomPriceComponentParser">A delegate to parse custom price component JSON objects.</param>
        public static PriceComponent Parse(String                                       Text,
                                           CustomJObjectParserDelegate<PriceComponent>  CustomPriceComponentParser   = null)
        {

            if (TryParse(Text,
                         out PriceComponent  priceComponent,
                         out String          ErrorResponse,
                         CustomPriceComponentParser))
            {
                return priceComponent;
            }

            throw new ArgumentException("The given text representation of a price component is invalid: " + ErrorResponse, nameof(Text));

        }

        #endregion

        #region (static) TryParse(JSON, CustomPriceComponentParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a price component.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomPriceComponentParser">A delegate to parse custom price component JSON objects.</param>
        public static PriceComponent? TryParse(JObject                                         JSON,
                                                  CustomJObjectParserDelegate<PriceComponent>  CustomPriceComponentParser   = null)
        {

            if (TryParse(JSON,
                         out PriceComponent  priceComponent,
                         out String          ErrorResponse,
                         CustomPriceComponentParser))
            {
                return priceComponent;
            }

            return default;

        }

        #endregion

        #region (static) TryParse(Text, CustomPriceComponentParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a price component.
        /// </summary>
        /// <param name="Text">The JSON to parse.</param>
        /// <param name="CustomPriceComponentParser">A delegate to parse custom price component JSON objects.</param>
        public static PriceComponent? TryParse(String                                       Text,
                                               CustomJObjectParserDelegate<PriceComponent>  CustomPriceComponentParser   = null)
        {

            if (TryParse(Text,
                         out PriceComponent  priceComponent,
                         out String          ErrorResponse,
                         CustomPriceComponentParser))
            {
                return priceComponent;
            }

            return default;

        }

        #endregion

        #region (static) TryParse(JSON, out PriceComponent, out ErrorResponse, CustomPriceComponentParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a price component.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="PriceComponent">The parsed price component.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject             JSON,
                                       out PriceComponent  PriceComponent,
                                       out String          ErrorResponse)

            => TryParse(JSON,
                        out PriceComponent,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a price component.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="PriceComponent">The parsed price component.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomPriceComponentParser">A delegate to parse custom price component JSON objects.</param>
        public static Boolean TryParse(JObject                                      JSON,
                                       out PriceComponent                           PriceComponent,
                                       out String                                   ErrorResponse,
                                       CustomJObjectParserDelegate<PriceComponent>  CustomPriceComponentParser   = null)
        {

            try
            {

                PriceComponent = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse Type          [mandatory]

                if (!JSON.ParseMandatoryEnum("type",
                                             "tariff dimension type",
                                             out TariffDimensions Type,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Price         [mandatory]

                if (!JSON.ParseMandatory("price",
                                         "price",
                                         out Decimal Price,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse VAT           [optional]

                if (JSON.ParseOptional("vat",
                                       "value added tax",
                                       out Decimal? VAT,
                                       out ErrorResponse))
                {
                    if (ErrorResponse != null)
                        return false;
                }

                #endregion

                #region Parse StepSize      [mandatory]

                if (!JSON.ParseMandatory("step_size",
                                         "step size",
                                         out UInt32 StepSize,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                PriceComponent = new PriceComponent(Type,
                                                    Price,
                                                    VAT,
                                                    StepSize);


                if (CustomPriceComponentParser != null)
                    PriceComponent = CustomPriceComponentParser(JSON,
                                                                PriceComponent);

                return true;

            }
            catch (Exception e)
            {
                PriceComponent  = default;
                ErrorResponse   = "The given JSON representation of a price component is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Text, out PriceComponent, out ErrorResponse, CustomPriceComponentParser = null)

        /// <summary>
        /// Try to parse the given text representation of a price component.
        /// </summary>
        /// <param name="Text">The text to parse.</param>
        /// <param name="PriceComponent">The parsed priceComponent.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomPriceComponentParser">A delegate to parse custom price component JSON objects.</param>
        public static Boolean TryParse(String                                       Text,
                                       out PriceComponent                           PriceComponent,
                                       out String                                   ErrorResponse,
                                       CustomJObjectParserDelegate<PriceComponent>  CustomPriceComponentParser   = null)
        {

            try
            {

                return TryParse(JObject.Parse(Text),
                                out PriceComponent,
                                out ErrorResponse,
                                CustomPriceComponentParser);

            }
            catch (Exception e)
            {
                PriceComponent  = default;
                ErrorResponse   = "The given text representation of a price component is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomPriceComponentSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomPriceComponentSerializer">A delegate to serialize custom price component JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<PriceComponent> CustomPriceComponentSerializer = null)
        {

            var JSON = JSONObject.Create(

                           new JProperty("type",       Type.ToString()),
                           new JProperty("price",      Price),

                           VAT.HasValue
                               ? new JProperty("vat",  VAT. Value)
                               : null,

                           new JProperty("step_size",  StepSize)

                       );

            return CustomPriceComponentSerializer != null
                       ? CustomPriceComponentSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Operator overloading

        #region Operator == (PriceComponent1, PriceComponent2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PriceComponent1">A price component.</param>
        /// <param name="PriceComponent2">Another price component.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (PriceComponent PriceComponent1,
                                           PriceComponent PriceComponent2)

            => PriceComponent1.Equals(PriceComponent2);

        #endregion

        #region Operator != (PriceComponent1, PriceComponent2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PriceComponent1">A price component.</param>
        /// <param name="PriceComponent2">Another price component.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (PriceComponent PriceComponent1,
                                           PriceComponent PriceComponent2)

            => !(PriceComponent1 == PriceComponent2);

        #endregion

        #region Operator <  (PriceComponent1, PriceComponent2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PriceComponent1">A price component.</param>
        /// <param name="PriceComponent2">Another price component.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (PriceComponent PriceComponent1,
                                          PriceComponent PriceComponent2)

            => PriceComponent1.CompareTo(PriceComponent2) < 0;

        #endregion

        #region Operator <= (PriceComponent1, PriceComponent2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PriceComponent1">A price component.</param>
        /// <param name="PriceComponent2">Another price component.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (PriceComponent PriceComponent1,
                                           PriceComponent PriceComponent2)

            => !(PriceComponent1 > PriceComponent2);

        #endregion

        #region Operator >  (PriceComponent1, PriceComponent2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PriceComponent1">A price component.</param>
        /// <param name="PriceComponent2">Another price component.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (PriceComponent PriceComponent1,
                                          PriceComponent PriceComponent2)

            => PriceComponent1.CompareTo(PriceComponent2) > 0;

        #endregion

        #region Operator >= (PriceComponent1, PriceComponent2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PriceComponent1">A price component.</param>
        /// <param name="PriceComponent2">Another price component.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (PriceComponent PriceComponent1,
                                           PriceComponent PriceComponent2)

            => !(PriceComponent1 < PriceComponent2);

        #endregion

        #endregion

        #region IComparable<PriceComponent> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is PriceComponent priceComponent
                   ? CompareTo(priceComponent)
                   : throw new ArgumentException("The given object is not a price component!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(PriceComponent)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PriceComponent">An object to compare with.</param>
        public Int32 CompareTo(PriceComponent PriceComponent)
        {

            var c = Type.    CompareTo(PriceComponent.Type);

            if (c == 0)
                c = Price.   CompareTo(PriceComponent.Price);

            if (c == 0 && VAT.HasValue && PriceComponent.VAT.HasValue)
                c = Price.CompareTo(PriceComponent.Price);

            if (c == 0)
                c = StepSize.CompareTo(PriceComponent.StepSize);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<PriceComponent> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is PriceComponent priceComponent &&
                   Equals(priceComponent);

        #endregion

        #region Equals(PriceComponent)

        /// <summary>
        /// Compares two price components for equality.
        /// </summary>
        /// <param name="PriceComponent">A price component to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(PriceComponent PriceComponent)

            => Type.    Equals(PriceComponent.Type)  &&
               Price.   Equals(PriceComponent.Price) &&
               VAT.     Equals(PriceComponent.VAT)   &&
               StepSize.Equals(PriceComponent.StepSize);

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

                return Type.    GetHashCode() * 7 ^
                       Price.   GetHashCode() * 5 ^
                       VAT.     GetHashCode() * 3 ^
                       StepSize.GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(Type,  "; ",
                             Price, "; ",
                             StepSize);

        #endregion

    }

}
