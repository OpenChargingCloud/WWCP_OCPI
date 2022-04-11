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
    /// Specifies one exceptional period for opening or access hours.
    /// </summary>
    public readonly struct ExceptionalPeriod : IEquatable<ExceptionalPeriod>,
                                               IComparable<ExceptionalPeriod>,
                                               IComparable
    {

        #region Properties

        /// <summary>
        /// Begin of the opening or access hours exception.
        /// </summary>
        [Mandatory]
        public DateTime  Begin    { get; }

        /// <summary>
        /// End of the opening or access hours exception.
        /// </summary>
        [Mandatory]
        public DateTime  End      { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new exceptional period for opening or access hours.
        /// </summary>
        /// <param name="Begin">Begin of the opening or access hours exception.</param>
        /// <param name="End">End of the opening or access hours exception.</param>
        public ExceptionalPeriod(DateTime  Begin,
                                 DateTime  End)
        {

            this.Begin  = Begin;
            this.End    = End;

        }

        #endregion


        #region (static) Parse   (JSON, CustomExceptionalPeriodParser = null)

        /// <summary>
        /// Parse the given JSON representation of an exceptional period.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomExceptionalPeriodParser">A delegate to parse custom exceptional period JSON objects.</param>
        public static ExceptionalPeriod Parse(JObject                                         JSON,
                                              CustomJObjectParserDelegate<ExceptionalPeriod>  CustomExceptionalPeriodParser   = null)
        {

            if (TryParse(JSON,
                         out ExceptionalPeriod  exceptionalPeriod,
                         out String             ErrorResponse,
                         CustomExceptionalPeriodParser))
            {
                return exceptionalPeriod;
            }

            throw new ArgumentException("The given JSON representation of an exceptional period is invalid: " + ErrorResponse, nameof(JSON));

        }

        #endregion

        #region (static) Parse   (Text, CustomExceptionalPeriodParser = null)

        /// <summary>
        /// Parse the given text representation of an exceptional period.
        /// </summary>
        /// <param name="Text">The text to parse.</param>
        /// <param name="CustomExceptionalPeriodParser">A delegate to parse custom exceptional period JSON objects.</param>
        public static ExceptionalPeriod Parse(String                                          Text,
                                              CustomJObjectParserDelegate<ExceptionalPeriod>  CustomExceptionalPeriodParser   = null)
        {

            if (TryParse(Text,
                         out ExceptionalPeriod  exceptionalPeriod,
                         out String             ErrorResponse,
                         CustomExceptionalPeriodParser))
            {
                return exceptionalPeriod;
            }

            throw new ArgumentException("The given text representation of an exceptional period is invalid: " + ErrorResponse, nameof(Text));

        }

        #endregion

        #region (static) TryParse(JSON, out ExceptionalPeriod, out ErrorResponse, CustomExceptionalPeriodParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of an exceptional period.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="ExceptionalPeriod">The parsed connector.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                JSON,
                                       out ExceptionalPeriod  ExceptionalPeriod,
                                       out String             ErrorResponse)

            => TryParse(JSON,
                        out ExceptionalPeriod,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of an exceptional period.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="ExceptionalPeriod">The parsed connector.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomExceptionalPeriodParser">A delegate to parse custom exceptional period JSON objects.</param>
        public static Boolean TryParse(JObject                                         JSON,
                                       out ExceptionalPeriod                           ExceptionalPeriod,
                                       out String                                      ErrorResponse,
                                       CustomJObjectParserDelegate<ExceptionalPeriod>  CustomExceptionalPeriodParser)
        {

            try
            {

                ExceptionalPeriod = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse PeriodBegin    [mandatory]

                if (!JSON.ParseMandatory("period_begin",
                                         "period begin",
                                         out DateTime PeriodBegin,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse PeriodEnd      [mandatory]

                if (!JSON.ParseMandatory("period_end",
                                         "period end",
                                         out DateTime PeriodEnd,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                ExceptionalPeriod = new ExceptionalPeriod(PeriodBegin,
                                                          PeriodEnd);


                if (CustomExceptionalPeriodParser != null)
                    ExceptionalPeriod = CustomExceptionalPeriodParser(JSON,
                                                                      ExceptionalPeriod);

                return true;

            }
            catch (Exception e)
            {
                ExceptionalPeriod   = default;
                ErrorResponse  = "The given JSON representation of an exceptional period is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Text, out ExceptionalPeriod, out ErrorResponse, CustomExceptionalPeriodParser = null)

        /// <summary>
        /// Try to parse the given text representation of an exceptional period.
        /// </summary>
        /// <param name="Text">The text to parse.</param>
        /// <param name="ExceptionalPeriod">The parsed connector.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomExceptionalPeriodParser">A delegate to parse custom exceptional period JSON objects.</param>
        public static Boolean TryParse(String                                          Text,
                                       out ExceptionalPeriod                           ExceptionalPeriod,
                                       out String                                      ErrorResponse,
                                       CustomJObjectParserDelegate<ExceptionalPeriod>  CustomExceptionalPeriodParser   = null)
        {

            try
            {

                return TryParse(JObject.Parse(Text),
                                out ExceptionalPeriod,
                                out ErrorResponse,
                                CustomExceptionalPeriodParser);

            }
            catch (Exception e)
            {
                ExceptionalPeriod  = default;
                ErrorResponse      = "The given text representation of an exceptional period is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomExceptionalPeriodSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomExceptionalPeriodSerializer">A delegate to serialize custom regular hours JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ExceptionalPeriod> CustomExceptionalPeriodSerializer = null)
        {

            var JSON = JSONObject.Create(
                           new JProperty("period_begin",  Begin.ToIso8601()),
                           new JProperty("period_end",    End.  ToIso8601())
                       );

            return CustomExceptionalPeriodSerializer != null
                       ? CustomExceptionalPeriodSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Operator overloading

        #region Operator == (ExceptionalPeriod1, ExceptionalPeriod2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ExceptionalPeriod1">An exceptional period for opening or access hours.</param>
        /// <param name="ExceptionalPeriod2">Another exceptional period for opening or access hours.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ExceptionalPeriod ExceptionalPeriod1,
                                           ExceptionalPeriod ExceptionalPeriod2)

            => ExceptionalPeriod1.Equals(ExceptionalPeriod2);

        #endregion

        #region Operator != (ExceptionalPeriod1, ExceptionalPeriod2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ExceptionalPeriod1">An exceptional period for opening or access hours.</param>
        /// <param name="ExceptionalPeriod2">Another exceptional period for opening or access hours.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ExceptionalPeriod ExceptionalPeriod1,
                                           ExceptionalPeriod ExceptionalPeriod2)

            => !(ExceptionalPeriod1 == ExceptionalPeriod2);

        #endregion

        #region Operator <  (ExceptionalPeriod1, ExceptionalPeriod2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ExceptionalPeriod1">An exceptional period for opening or access hours.</param>
        /// <param name="ExceptionalPeriod2">Another exceptional period for opening or access hours.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ExceptionalPeriod ExceptionalPeriod1,
                                          ExceptionalPeriod ExceptionalPeriod2)

            => ExceptionalPeriod1.CompareTo(ExceptionalPeriod2) < 0;

        #endregion

        #region Operator <= (ExceptionalPeriod1, ExceptionalPeriod2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ExceptionalPeriod1">An exceptional period for opening or access hours.</param>
        /// <param name="ExceptionalPeriod2">Another exceptional period for opening or access hours.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ExceptionalPeriod ExceptionalPeriod1,
                                           ExceptionalPeriod ExceptionalPeriod2)

            => !(ExceptionalPeriod1 > ExceptionalPeriod2);

        #endregion

        #region Operator >  (ExceptionalPeriod1, ExceptionalPeriod2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ExceptionalPeriod1">An exceptional period for opening or access hours.</param>
        /// <param name="ExceptionalPeriod2">Another exceptional period for opening or access hours.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ExceptionalPeriod ExceptionalPeriod1,
                                          ExceptionalPeriod ExceptionalPeriod2)

            => ExceptionalPeriod1.CompareTo(ExceptionalPeriod2) > 0;

        #endregion

        #region Operator >= (ExceptionalPeriod1, ExceptionalPeriod2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ExceptionalPeriod1">An exceptional period for opening or access hours.</param>
        /// <param name="ExceptionalPeriod2">Another exceptional period for opening or access hours.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ExceptionalPeriod ExceptionalPeriod1,
                                           ExceptionalPeriod ExceptionalPeriod2)

            => !(ExceptionalPeriod1 < ExceptionalPeriod2);

        #endregion

        #endregion

        #region IComparable<ExceptionalPeriod> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is ExceptionalPeriod exceptionalPeriod
                   ? CompareTo(exceptionalPeriod)
                   : throw new ArgumentException("The given object is not an exceptional period!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(ExceptionalPeriod)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ExceptionalPeriod">An object to compare with.</param>
        public Int32 CompareTo(ExceptionalPeriod ExceptionalPeriod)
        {

            var c = Begin.CompareTo(ExceptionalPeriod.Begin);

            if (c == 0)
                c = End.CompareTo(ExceptionalPeriod.End);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<ExceptionalPeriod> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is ExceptionalPeriod exceptionalPeriod &&
                   Equals(exceptionalPeriod);

        #endregion

        #region Equals(ExceptionalPeriod)

        /// <summary>
        /// Compares two ExceptionalPeriods for equality.
        /// </summary>
        /// <param name="ExceptionalPeriod">A ExceptionalPeriod to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(ExceptionalPeriod ExceptionalPeriod)

            => Begin.Equals(ExceptionalPeriod.Begin) &&
               End.  Equals(ExceptionalPeriod.End);

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

                return Begin.GetHashCode() * 3 ^
                       End.  GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(Begin,
                             " - ",
                             End);

        #endregion

    }

}
