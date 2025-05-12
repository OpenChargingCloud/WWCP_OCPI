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
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_3_0
{

    /// <summary>
    /// Bookable
    /// </summary>
    public class Bookable : IEquatable<Bookable>,
                            IComparable<Bookable>,
                            IComparable
    {

        #region Properties

        /// <summary>
        /// Whether a reservation is required.
        /// </summary>
        [Mandatory]
        public Boolean  ReservationRequired    { get; }

        /// <summary>
        /// The optional number of ad hoc charging options available.
        /// </summary>
        [Optional]
        public Byte?    AdHoc                  { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new bookable.
        /// </summary>
        /// <param name="ReservationRequired">Whether a reservation is required.</param>
        /// <param name="AdHoc">An optional number of ad hoc charging options available.</param>
        public Bookable(Boolean  ReservationRequired,
                        Byte?    AdHoc   = null)
        {

            this.ReservationRequired  = ReservationRequired;
            this.AdHoc                = AdHoc;

            unchecked
            {

                hashCode = this.ReservationRequired.GetHashCode() * 3 ^
                           this.AdHoc?.             GetHashCode() ?? 0;

            }

        }

        #endregion


        #region (static) Parse   (JSON, CustomBookableParser = null)

        /// <summary>
        /// Parse the given JSON representation of a bookable.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomBookableParser">A delegate to parse custom bookable JSON objects.</param>
        public static Bookable Parse(JObject                                 JSON,
                                     CustomJObjectParserDelegate<Bookable>?  CustomBookableParser   = null)
        {

            if (TryParse(JSON,
                         out var bookable,
                         out var errorResponse,
                         CustomBookableParser))
            {
                return bookable;
            }

            throw new ArgumentException("The given JSON representation of a bookable is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out Bookable, out ErrorResponse, CustomBookableParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a bookable.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="Bookable">The parsed connector.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                             JSON,
                                       [NotNullWhen(true)]  out Bookable?  Bookable,
                                       [NotNullWhen(false)] out String?    ErrorResponse)

            => TryParse(JSON,
                        out Bookable,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a bookable.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="Bookable">The parsed connector.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomBookableParser">A delegate to parse custom bookable JSON objects.</param>
        public static Boolean TryParse(JObject                                 JSON,
                                       [NotNullWhen(true)]  out Bookable?      Bookable,
                                       [NotNullWhen(false)] out String?        ErrorResponse,
                                       CustomJObjectParserDelegate<Bookable>?  CustomBookableParser)
        {

            try
            {

                Bookable = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse ReservationRequired    [mandatory]

                if (!JSON.ParseMandatory("reservation_required",
                                         "reservation required",
                                         out Boolean reservationRequired,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse AdHoc                  [optional]

                if (!JSON.ParseOptional("ad_hoc",
                                        "ad hoc",
                                        out Byte? adHoc,
                                        out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                Bookable = new Bookable(
                               reservationRequired,
                               adHoc
                           );

                if (CustomBookableParser is not null)
                    Bookable = CustomBookableParser(JSON,
                                                    Bookable);

                return true;

            }
            catch (Exception e)
            {
                Bookable       = default;
                ErrorResponse  = "The given JSON representation of a bookable is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomBookableSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomBookableSerializer">A delegate to serialize custom bookable JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<Bookable>? CustomBookableSerializer = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("reservation_required",   ReservationRequired),

                           AdHoc.HasValue
                               ? new JProperty("ad_hoc",                 AdHoc.Value)
                               : null

                       );

            return CustomBookableSerializer is not null
                       ? CustomBookableSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this bookable.
        /// </summary>
        public Bookable Clone()

            => new (
                   ReservationRequired,
                   AdHoc
               );

        #endregion


        #region Operator overloading

        #region Operator == (Bookable1, Bookable2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Bookable1">Bookable.</param>
        /// <param name="Bookable2">Other bookable.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Bookable Bookable1,
                                           Bookable Bookable2)
        {

            if (Object.ReferenceEquals(Bookable1, Bookable2))
                return true;

            if (Bookable1 is null || Bookable2 is null)
                return false;

            return Bookable1.Equals(Bookable2);

        }

        #endregion

        #region Operator != (Bookable1, Bookable2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Bookable1">Bookable.</param>
        /// <param name="Bookable2">Other bookable.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Bookable Bookable1,
                                           Bookable Bookable2)

            => !(Bookable1 == Bookable2);

        #endregion

        #region Operator <  (Bookable1, Bookable2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Bookable1">A booking location.</param>
        /// <param name="Bookable2">Another booking location.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Bookable? Bookable1,
                                          Bookable? Bookable2)

            => Bookable1 is null
                   ? throw new ArgumentNullException(nameof(Bookable1), "The given bookingLocation must not be null!")
                   : Bookable1.CompareTo(Bookable2) < 0;

        #endregion

        #region Operator <= (Bookable1, Bookable2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Bookable1">A booking location.</param>
        /// <param name="Bookable2">Another booking location.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Bookable? Bookable1,
                                           Bookable? Bookable2)

            => !(Bookable1 > Bookable2);

        #endregion

        #region Operator >  (Bookable1, Bookable2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Bookable1">A booking location.</param>
        /// <param name="Bookable2">Another booking location.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Bookable? Bookable1,
                                          Bookable? Bookable2)

            => Bookable1 is null
                   ? throw new ArgumentNullException(nameof(Bookable1), "The given bookingLocation must not be null!")
                   : Bookable1.CompareTo(Bookable2) > 0;

        #endregion

        #region Operator >= (Bookable1, Bookable2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Bookable1">A booking location.</param>
        /// <param name="Bookable2">Another booking location.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Bookable? Bookable1,
                                           Bookable? Bookable2)

            => !(Bookable1 < Bookable2);

        #endregion

        #endregion

        #region IComparable<Bookable> Members

        #region CompareTo(Object)

        /// <summary>s
        /// Compares two bookables.
        /// </summary>
        /// <param name="Object">A bookable to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Bookable bookable
                   ? CompareTo(bookable)
                   : throw new ArgumentException("The given object is not a bookable!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(Bookable)

        /// <summary>s
        /// Compares two bookables.
        /// </summary>
        /// <param name="Object">A bookable to compare with.</param>
        public Int32 CompareTo(Bookable? Bookable)
        {

            if (Bookable is null)
                throw new ArgumentNullException(nameof(Bookable), "The given bookable must not be null!");

            var c = ReservationRequired.CompareTo(Bookable.ReservationRequired);

            if (c == 0 && AdHoc.HasValue && Bookable.AdHoc.HasValue)
                c = AdHoc.Value.CompareTo(Bookable.AdHoc.Value);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<Bookable> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two bookable for equality.
        /// </summary>
        /// <param name="Object">Bookable to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Bookable bookable &&
                   Equals(bookable);

        #endregion

        #region Equals(Bookable)

        /// <summary>
        /// Compares two bookable for equality.
        /// </summary>
        /// <param name="Bookable">Bookable to compare with.</param>
        public Boolean Equals(Bookable? Bookable)

            => Bookable is not null &&

               ReservationRequired.Equals(Bookable.ReservationRequired) &&

            ((!AdHoc.HasValue && !Bookable.AdHoc.HasValue) ||
              (AdHoc.HasValue &&  Bookable.AdHoc.HasValue && AdHoc.Value.Equals(Bookable.AdHoc.Value)));

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

                   ReservationRequired
                       ? "Reservation required"
                       : "Reservation not required",

                   AdHoc.HasValue
                       ? ", AdHoc: " + AdHoc.Value
                       : ""

               );

        #endregion

    }

}
