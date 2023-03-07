﻿/*
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

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2
{

    /// <summary>
    /// A multi-language text.
    /// </summary>
    public readonly struct DisplayText : IEquatable<DisplayText>,
                                         IComparable<DisplayText>,
                                         IComparable
    {

        #region Properties

        /// <summary>
        /// The language of the text.
        /// </summary>
        [Mandatory]
        public Languages  Language    { get; }

        /// <summary>
        /// The text.
        /// </summary>
        [Mandatory]
        public String     Text        { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// A multi-language text.
        /// </summary>
        /// <param name="Language">The language of the text.</param>
        /// <param name="Text">The text.</param>
        public DisplayText(Languages  Language,
                           String     Text)
        {

            if (Text.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Text), "The given multi-language text must not be null or empty!");

            this.Language  = Language;
            this.Text      = Text.Trim();

        }

        #endregion


        #region (static) Create  (Language, Text)

        /// <summary>
        /// Create a new multi-language text.
        /// </summary>
        /// <param name="Language">The language of the text.</param>
        /// <param name="Text">The text.</param>
        public static DisplayText Create(Languages  Language,
                                         String     Text)

            => new (Language,
                    Text);

        #endregion

        #region (static) Parse   (JSON, CustomDisplayTextParser = null)

        /// <summary>
        /// Parse the given JSON representation of a multi-language text.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomDisplayTextParser">A delegate to parse custom multi-language text JSON objects.</param>
        public static DisplayText Parse(JObject                                    JSON,
                                        CustomJObjectParserDelegate<DisplayText>?  CustomDisplayTextParser   = null)
        {

            if (TryParse(JSON,
                         out var displayText,
                         out var errorResponse,
                         CustomDisplayTextParser))
            {
                return displayText;
            }

            throw new ArgumentException("The given JSON representation of a multi-language text is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out DisplayText, out ErrorResponse, CustomDisplayTextParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a displayText.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="DisplayText">The parsed multi-language text.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject          JSON,
                                       out DisplayText  DisplayText,
                                       out String?      ErrorResponse)

            => TryParse(JSON,
                        out DisplayText,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a displayText.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="DisplayText">The parsed multi-language text.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomDisplayTextParser">A delegate to parse custom multi-language text JSON objects.</param>
        public static Boolean TryParse(JObject                                    JSON,
                                       out DisplayText                            DisplayText,
                                       out String?                                ErrorResponse,
                                       CustomJObjectParserDelegate<DisplayText>?  CustomDisplayTextParser)
        {

            try
            {

                DisplayText = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse Language    [mandatory]

                if (!JSON.ParseMandatoryEnum("language",
                                             "language",
                                             out Languages Language,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Text        [mandatory]

                if (!JSON.ParseMandatoryText("text",
                                             "text",
                                             out String Text,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion


                DisplayText = new DisplayText(Language,
                                              Text);


                if (CustomDisplayTextParser is not null)
                    DisplayText = CustomDisplayTextParser(JSON,
                                                          DisplayText);

                return true;

            }
            catch (Exception e)
            {
                DisplayText    = default;
                ErrorResponse  = "The given JSON representation of a multi-language text is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomDisplayTextSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomDisplayTextSerializer">A delegate to serialize custom multi-language text JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<DisplayText>? CustomDisplayTextSerializer = null)
        {

            var JSON = JSONObject.Create(
                           new JProperty("language",  Language.ToString()),
                           new JProperty("text",      Text)
                       );

            return CustomDisplayTextSerializer is not null
                       ? CustomDisplayTextSerializer(this, JSON)
                       : JSON;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this object.
        /// </summary>
        public DisplayText Clone()

            => new (Language,
                    new String(Text.ToCharArray()));

        #endregion


        #region Operator overloading

        #region Operator == (DisplayText1, DisplayText2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DisplayText1">A display text.</param>
        /// <param name="DisplayText2">Another display text.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (DisplayText DisplayText1,
                                           DisplayText DisplayText2)

            => DisplayText1.Equals(DisplayText2);

        #endregion

        #region Operator != (DisplayText1, DisplayText2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DisplayText1">A display text.</param>
        /// <param name="DisplayText2">Another display text.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (DisplayText DisplayText1,
                                           DisplayText DisplayText2)

            => !(DisplayText1 == DisplayText2);

        #endregion

        #region Operator <  (DisplayText1, DisplayText2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DisplayText1">A display text.</param>
        /// <param name="DisplayText2">Another display text.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (DisplayText DisplayText1,
                                          DisplayText DisplayText2)

            => DisplayText1.CompareTo(DisplayText2) < 0;

        #endregion

        #region Operator <= (DisplayText1, DisplayText2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DisplayText1">A display text.</param>
        /// <param name="DisplayText2">Another display text.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (DisplayText DisplayText1,
                                           DisplayText DisplayText2)

            => DisplayText1.CompareTo(DisplayText2) <= 0;

        #endregion

        #region Operator >  (DisplayText1, DisplayText2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DisplayText1">A display text.</param>
        /// <param name="DisplayText2">Another display text.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (DisplayText DisplayText1,
                                          DisplayText DisplayText2)

            => DisplayText1.CompareTo(DisplayText2) > 0;

        #endregion

        #region Operator >= (DisplayText1, DisplayText2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DisplayText1">A display text.</param>
        /// <param name="DisplayText2">Another display text.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (DisplayText DisplayText1,
                                           DisplayText DisplayText2)

            => DisplayText1.CompareTo(DisplayText2) >= 0;

        #endregion

        #endregion

        #region IComparable<DisplayText> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two display texts.
        /// </summary>
        /// <param name="Object">A display text to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is DisplayText displayText
                   ? CompareTo(displayText)
                   : throw new ArgumentException("The given object is not a display text!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(DisplayText)

        /// <summary>
        /// Compares two display texts.
        /// </summary>
        /// <param name="DisplayText">A display text to compare with.</param>
        public Int32 CompareTo(DisplayText DisplayText)
        {

            var c = Language.CompareTo(DisplayText.Language);

            if (c == 0)
                c = Text.    CompareTo(DisplayText.Text);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<DisplayText> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two display texts for equality.
        /// </summary>
        /// <param name="Object">A display text to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is DisplayText displayText &&
                   Equals(displayText);

        #endregion

        #region Equals(DisplayText)

        /// <summary>
        /// Compares two display texts for equality.
        /// </summary>
        /// <param name="DisplayText">A display text to compare with.</param>
        public Boolean Equals(DisplayText DisplayText)

            => Language.Equals(DisplayText.Language) &&
               Text.    Equals(DisplayText.Text);

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

                return Language.GetHashCode() * 3 ^
                       Text.    GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   Language,
                   " -> ",
                   Text

               );

        #endregion

    }

}
