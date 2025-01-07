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

using System.Collections;
using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv3_0
{

    /// <summary>
    /// Display texts.
    /// </summary>
    public class DisplayTexts : IEquatable<DisplayTexts>,
                                IComparable<DisplayTexts>,
                                IEnumerable<DisplayText>
    {

        #region Data

        /// <summary>
        /// The default JSON-LD context of this object.
        /// </summary>
        public static readonly JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/contexts/OCPI/displayTexts");

        #endregion

        #region Properties

        /// <summary>
        /// The enumeration of display texts.
        /// </summary>
        [Mandatory]
        public IEnumerable<DisplayText>  Texts    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new enumeration of display texts.
        /// </summary>
        /// <param name="Texts">An enumeration of of display texts.</param>
        public DisplayTexts(IEnumerable<DisplayText> Texts)

        {

            this.Texts = Texts.Distinct() ?? [];

            unchecked
            {
                hashCode = Texts.CalcHashCode();
            }

        }

        #endregion


        #region (static) Parse   (JSON, ...)

        /// <summary>
        /// Parse the given JSON representation of display texts.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomDisplayTextsParser">A delegate to parse custom display texts JSON objects.</param>
        public static DisplayTexts Parse(JArray                                     JSON,
                                         CustomJArrayParserDelegate<DisplayTexts>?  CustomDisplayTextsParser   = null)
        {

            if (TryParse(JSON,
                         out var displayTexts,
                         out var errorResponse,
                         CustomDisplayTextsParser))
            {
                return displayTexts;
            }

            throw new ArgumentException("The given JSON representation of display texts is invalid: " + errorResponse, nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out DisplayTexts, out ErrorResponse, ...)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of display texts.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="DisplayTexts">The parsed display texts.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JArray                                  JSON,
                                       [NotNullWhen(true)]  out DisplayTexts?  DisplayTexts,
                                       [NotNullWhen(false)] out String?        ErrorResponse)

            => TryParse(JSON,
                        out DisplayTexts,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of display texts.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="DisplayTexts">The parsed display texts.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomDisplayTextsParser">A delegate to parse custom display texts JSON objects.</param>
        public static Boolean TryParse(JArray                                     JSON,
                                       [NotNullWhen(true)]  out DisplayTexts?     DisplayTexts,
                                       [NotNullWhen(false)] out String?           ErrorResponse,
                                       CustomJArrayParserDelegate<DisplayTexts>?  CustomDisplayTextsParser)
        {

            try
            {

                DisplayTexts   = null;
                ErrorResponse  = null;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                var Texts = new HashSet<DisplayText>();

                foreach (var json in JSON)
                {

                    if (json is not JObject jObject)
                    {
                        ErrorResponse = "Could not parse the given JSON object!";
                        return false;
                    }

                    if (!DisplayText.TryParse(jObject,
                                              out var displayText,
                                              out ErrorResponse))
                    {
                        return false;
                    }

                    Texts.Add(displayText);

                }

                DisplayTexts = new DisplayTexts(
                                   Texts
                               );

                if (CustomDisplayTextsParser is not null)
                    DisplayTexts = CustomDisplayTextsParser(JSON,
                                                            DisplayTexts);

                return true;

            }
            catch (Exception e)
            {
                DisplayTexts   = default;
                ErrorResponse  = "The given JSON representation of display texts is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomDisplayTextsSerializer = null, CustomDisplayTextSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomDisplayTextsSerializer">A delegate to serialize custom display texts JSON objects.</param>
        /// <param name="CustomDisplayTextSerializer">A delegate to serialize custom platform party JSON objects.</param>
        public JArray ToJSON(CustomJArraySerializerDelegate<DisplayTexts>?  CustomDisplayTextsSerializer   = null,
                             CustomJObjectSerializerDelegate<DisplayText>?  CustomDisplayTextSerializer    = null)
        {

            var json = new JArray(Texts.Select(displayText => displayText.ToJSON(CustomDisplayTextSerializer)));

            return CustomDisplayTextsSerializer is not null
                       ? CustomDisplayTextsSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this display texts.
        /// </summary>
        public DisplayTexts Clone()

            => new (
                   Texts.Select(displayText => displayText.Clone())
               );

        #endregion


        #region (static) Create(Language, Text)
        public static DisplayTexts Create(Languages  Language,
                                          String     Text)

            => new ([ new DisplayText(Language, Text) ]);

        #endregion

        #region (static) Empty

        public static DisplayTexts Empty
            => new ([]);

        #endregion


        #region IEnumerable<DisplayText> Members

        public IEnumerator<DisplayText> GetEnumerator()
            => Texts.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => Texts.GetEnumerator();

        #endregion


        #region Operator overloading

        #region Operator == (DisplayTexts1, DisplayTexts2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DisplayTexts1">A display texts.</param>
        /// <param name="DisplayTexts2">Another display texts.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (DisplayTexts DisplayTexts1,
                                           DisplayTexts DisplayTexts2)

            => DisplayTexts1.Equals(DisplayTexts2);

        #endregion

        #region Operator != (DisplayTexts1, DisplayTexts2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DisplayTexts1">A display texts.</param>
        /// <param name="DisplayTexts2">Another display texts.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (DisplayTexts DisplayTexts1,
                                           DisplayTexts DisplayTexts2)

            => !DisplayTexts1.Equals(DisplayTexts2);

        #endregion

        #region Operator <  (DisplayTexts1, DisplayTexts2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DisplayTexts1">A display texts.</param>
        /// <param name="DisplayTexts2">Another display texts.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (DisplayTexts DisplayTexts1,
                                          DisplayTexts DisplayTexts2)

            => DisplayTexts1.CompareTo(DisplayTexts2) < 0;

        #endregion

        #region Operator <= (DisplayTexts1, DisplayTexts2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DisplayTexts1">A display texts.</param>
        /// <param name="DisplayTexts2">Another display texts.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (DisplayTexts DisplayTexts1,
                                           DisplayTexts DisplayTexts2)

            => DisplayTexts1.CompareTo(DisplayTexts2) <= 0;

        #endregion

        #region Operator >  (DisplayTexts1, DisplayTexts2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DisplayTexts1">A display texts.</param>
        /// <param name="DisplayTexts2">Another display texts.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (DisplayTexts DisplayTexts1,
                                          DisplayTexts DisplayTexts2)

            => DisplayTexts1.CompareTo(DisplayTexts2) > 0;

        #endregion

        #region Operator >= (DisplayTexts1, DisplayTexts2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DisplayTexts1">A display texts.</param>
        /// <param name="DisplayTexts2">Another display texts.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (DisplayTexts DisplayTexts1,
                                           DisplayTexts DisplayTexts2)

            => DisplayTexts1.CompareTo(DisplayTexts2) >= 0;

        #endregion

        #endregion

        #region IComparable<DisplayTexts> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two display texts.
        /// </summary>
        /// <param name="Object">A display texts to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is DisplayTexts displayTexts
                   ? CompareTo(displayTexts)
                   : throw new ArgumentException("The given object is not a display texts object!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(DisplayTexts)

        /// <summary>
        /// Compares two display texts.
        /// </summary>
        /// <param name="DisplayTexts">A display texts to compare with.</param>
        public Int32 CompareTo(DisplayTexts? DisplayTexts)
        {

            if (DisplayTexts is null)
                throw new ArgumentNullException(nameof(DisplayTexts), "The given display texts object must not be null!");

            return Texts.OrderBy(displayText => displayText).SequenceEqual(DisplayTexts.Texts.OrderBy(displayText => displayText)) ? 0 : 1;

        }

        #endregion

        #endregion

        #region IEquatable<DisplayTexts> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two display texts for equality.
        /// </summary>
        /// <param name="Object">A display texts to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is DisplayTexts displayTexts &&
                   Equals(displayTexts);

        #endregion

        #region Equals(DisplayTexts)

        /// <summary>
        /// Compares two display texts for equality.
        /// </summary>
        /// <param name="DisplayTexts">A display texts to compare with.</param>
        public Boolean Equals(DisplayTexts? DisplayTexts)

            => DisplayTexts is not null &&
               Texts.OrderBy(displayText => displayText).SequenceEqual(DisplayTexts.Texts.OrderBy(displayText => displayText));

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

            => $"{Texts.Select(displayText => displayText.ToString()).AggregateWith("; ")}";

        #endregion

    }

}
