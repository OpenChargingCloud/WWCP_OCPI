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
    /// A charging dimension.
    /// </summary>
    public readonly struct CDRDimension : IEquatable<CDRDimension>,
                                          IComparable<CDRDimension>,
                                          IComparable
    {

        #region Properties

        /// <summary>
        /// The charging dimension.
        /// </summary>
        [Mandatory]
        public CDRDimensions  Type      { get; }

        /// <summary>
        /// Volume of the dimension consumed, measured according to the dimension type.
        /// </summary>
        [Mandatory]
        public Decimal        Volume    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new charging dimension.
        /// </summary>
        /// <param name="Type">The charging dimension.</param>
        /// <param name="Volume">Volume of the dimension consumed, measured according to the dimension type.</param>
        public CDRDimension(CDRDimensions  Type,
                            Decimal        Volume)
        {

            this.Type    = Type;
            this.Volume  = Volume;

        }

        #endregion


        #region (static) Parse   (JSON, CustomCDRDimensionParser = null)

        /// <summary>
        /// Parse the given JSON representation of a charging dimension.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomCDRDimensionParser">A delegate to parse custom charging dimension JSON objects.</param>
        public static CDRDimension Parse(JObject                                    JSON,
                                         CustomJObjectParserDelegate<CDRDimension>  CustomCDRDimensionParser   = null)
        {

            if (TryParse(JSON,
                         out CDRDimension  cdrDimension,
                         out String        ErrorResponse,
                         CustomCDRDimensionParser))
            {
                return cdrDimension;
            }

            throw new ArgumentException("The given JSON representation of a charging dimension is invalid: " + ErrorResponse, nameof(JSON));

        }

        #endregion

        #region (static) Parse   (Text, CustomCDRDimensionParser = null)

        /// <summary>
        /// Parse the given text representation of a charging dimension.
        /// </summary>
        /// <param name="Text">The text to parse.</param>
        /// <param name="CustomCDRDimensionParser">A delegate to parse custom charging dimension JSON objects.</param>
        public static CDRDimension Parse(String                                     Text,
                                         CustomJObjectParserDelegate<CDRDimension>  CustomCDRDimensionParser   = null)
        {

            if (TryParse(Text,
                         out CDRDimension  cdrDimension,
                         out String        ErrorResponse,
                         CustomCDRDimensionParser))
            {
                return cdrDimension;
            }

            throw new ArgumentException("The given text representation of a charging dimension is invalid: " + ErrorResponse, nameof(Text));

        }

        #endregion

        #region (static) TryParse(JSON, CustomCDRDimensionParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a charging dimension.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomCDRDimensionParser">A delegate to parse custom charging dimension JSON objects.</param>
        public static CDRDimension? TryParse(JObject                                    JSON,
                                             CustomJObjectParserDelegate<CDRDimension>  CustomCDRDimensionParser   = null)
        {

            if (TryParse(JSON,
                         out CDRDimension  cdrDimension,
                         out String        ErrorResponse,
                         CustomCDRDimensionParser))
            {
                return cdrDimension;
            }

            return default;

        }

        #endregion

        #region (static) TryParse(Text, CustomCDRDimensionParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a charging dimension.
        /// </summary>
        /// <param name="Text">The JSON to parse.</param>
        /// <param name="CustomCDRDimensionParser">A delegate to parse custom charging dimension JSON objects.</param>
        public static CDRDimension? TryParse(String                                     Text,
                                             CustomJObjectParserDelegate<CDRDimension>  CustomCDRDimensionParser   = null)
        {

            if (TryParse(Text,
                         out CDRDimension  cdrDimension,
                         out String        ErrorResponse,
                         CustomCDRDimensionParser))
            {
                return cdrDimension;
            }

            return default;

        }

        #endregion

        #region (static) TryParse(JSON, out CDRDimension, out ErrorResponse, CustomCDRDimensionParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a charging dimension.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CDRDimension">The parsed charging dimension.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject           JSON,
                                       out CDRDimension  CDRDimension,
                                       out String        ErrorResponse)

            => TryParse(JSON,
                        out CDRDimension,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a charging dimension.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CDRDimension">The parsed charging dimension.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomCDRDimensionParser">A delegate to parse custom charging dimension JSON objects.</param>
        public static Boolean TryParse(JObject                                    JSON,
                                       out CDRDimension                           CDRDimension,
                                       out String                                 ErrorResponse,
                                       CustomJObjectParserDelegate<CDRDimension>  CustomCDRDimensionParser   = null)
        {

            try
            {

                CDRDimension = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse Type      [mandatory]

                if (!JSON.ParseMandatoryEnum("type",
                                             "charging dimension type",
                                             out CDRDimensions Type,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Volume    [mandatory]

                if (!JSON.ParseMandatory("volume",
                                         "volume of the dimension consumed",
                                         out Decimal Volume,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                CDRDimension = new CDRDimension(Type,
                                                Volume);


                if (CustomCDRDimensionParser is not null)
                    CDRDimension = CustomCDRDimensionParser(JSON,
                                                            CDRDimension);

                return true;

            }
            catch (Exception e)
            {
                CDRDimension   = default;
                ErrorResponse  = "The given JSON representation of a charging dimension is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Text, out CDRDimension, out ErrorResponse, CustomCDRDimensionParser = null)

        /// <summary>
        /// Try to parse the given text representation of an cdrDimension.
        /// </summary>
        /// <param name="Text">The text to parse.</param>
        /// <param name="CDRDimension">The parsed cdrDimension.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomCDRDimensionParser">A delegate to parse custom charging dimension JSON objects.</param>
        public static Boolean TryParse(String                                     Text,
                                       out CDRDimension                           CDRDimension,
                                       out String                                 ErrorResponse,
                                       CustomJObjectParserDelegate<CDRDimension>  CustomCDRDimensionParser   = null)
        {

            try
            {

                return TryParse(JObject.Parse(Text),
                                out CDRDimension,
                                out ErrorResponse,
                                CustomCDRDimensionParser);

            }
            catch (Exception e)
            {
                CDRDimension   = default;
                ErrorResponse  = "The given text representation of a charging dimension is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomCDRDimensionSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomCDRDimensionSerializer">A delegate to serialize custom charging dimension JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<CDRDimension> CustomCDRDimensionSerializer = null)
        {

            var JSON = JSONObject.Create(
                           new JProperty("type",    Type.ToString()),
                           new JProperty("volume",  Volume)
                       );

            return CustomCDRDimensionSerializer is not null
                       ? CustomCDRDimensionSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Operator overloading

        #region Operator == (CDRDimension1, CDRDimension2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDRDimension1">A specification of a charging dimension.</param>
        /// <param name="CDRDimension2">Another specification of a charging dimension.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (CDRDimension CDRDimension1,
                                           CDRDimension CDRDimension2)

            => CDRDimension1.Equals(CDRDimension2);

        #endregion

        #region Operator != (CDRDimension1, CDRDimension2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDRDimension1">A specification of a charging dimension.</param>
        /// <param name="CDRDimension2">Another specification of a charging dimension.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (CDRDimension CDRDimension1,
                                           CDRDimension CDRDimension2)

            => !(CDRDimension1 == CDRDimension2);

        #endregion

        #region Operator <  (CDRDimension1, CDRDimension2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDRDimension1">A specification of a charging dimension.</param>
        /// <param name="CDRDimension2">Another specification of a charging dimension.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (CDRDimension CDRDimension1,
                                          CDRDimension CDRDimension2)

            => CDRDimension1.CompareTo(CDRDimension2) < 0;

        #endregion

        #region Operator <= (CDRDimension1, CDRDimension2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDRDimension1">A specification of a charging dimension.</param>
        /// <param name="CDRDimension2">Another specification of a charging dimension.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (CDRDimension CDRDimension1,
                                           CDRDimension CDRDimension2)

            => !(CDRDimension1 > CDRDimension2);

        #endregion

        #region Operator >  (CDRDimension1, CDRDimension2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDRDimension1">A specification of a charging dimension.</param>
        /// <param name="CDRDimension2">Another specification of a charging dimension.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (CDRDimension CDRDimension1,
                                          CDRDimension CDRDimension2)

            => CDRDimension1.CompareTo(CDRDimension2) > 0;

        #endregion

        #region Operator >= (CDRDimension1, CDRDimension2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDRDimension1">A specification of a charging dimension.</param>
        /// <param name="CDRDimension2">Another specification of a charging dimension.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (CDRDimension CDRDimension1,
                                           CDRDimension CDRDimension2)

            => !(CDRDimension1 < CDRDimension2);

        #endregion

        #endregion

        #region IComparable<CDRDimension> Members

        #region CompareTo(Object)d

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is CDRDimension CDRDimension
                   ? CompareTo(CDRDimension)
                   : throw new ArgumentException("The given object is not a charging dimension!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(CDRDimension)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDRDimension">An object to compare with.</param>
        public Int32 CompareTo(CDRDimension CDRDimension)

            => Type.CompareTo(CDRDimension.Type);

        #endregion

        #endregion

        #region IEquatable<CDRDimension> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is CDRDimension CDRDimension &&
                   Equals(CDRDimension);

        #endregion

        #region Equals(CDRDimension)

        /// <summary>
        /// Compares two CDRDimensions for equality.
        /// </summary>
        /// <param name="CDRDimension">A CDRDimension to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(CDRDimension CDRDimension)

            => Type.  Equals(CDRDimension.Type) &&
               Volume.Equals(CDRDimension.Volume);

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

                return Type.  GetHashCode() * 3 ^
                       Volume.GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(Volume,
                             " ",
                             Type);

        #endregion

    }

}
