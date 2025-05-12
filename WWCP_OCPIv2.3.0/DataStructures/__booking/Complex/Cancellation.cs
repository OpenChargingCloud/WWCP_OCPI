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

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_3_0
{

    /// <summary>
    /// A booking cancellation
    /// </summary>
    public class Cancellation : IEquatable<Cancellation>,
                                IComparable<Cancellation>,
                                IComparable
    {

        #region Properties

        /// <summary>
        /// The reason why the booking is canceled.
        /// </summary>
        [Mandatory]
        public CanceledReason  CancellationReason    { get; }

        /// <summary>
        /// Who canceled the booking.
        /// </summary>
        [Mandatory]
        public Role            WhoCanceled           { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new booking cancellation.
        /// </summary>
        /// <param name="CancellationReason">The reason why the booking is canceled.</param>
        /// <param name="WhoCanceled">Who canceled the booking.</param>
        public Cancellation(CanceledReason  CancellationReason,
                            Role            WhoCanceled)
        {

            this.CancellationReason  = CancellationReason;
            this.WhoCanceled         = WhoCanceled;

            unchecked
            {

                hashCode = this.CancellationReason.GetHashCode() * 3 ^
                           this.WhoCanceled.       GetHashCode();

            }

        }

        #endregion


        #region (static) Parse   (JSON, CustomCancellationParser = null)

        /// <summary>
        /// Parse the given JSON representation of a booking cancellation.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomCancellationParser">A delegate to parse custom booking cancellation JSON objects.</param>
        public static Cancellation Parse(JObject                                     JSON,
                                         CustomJObjectParserDelegate<Cancellation>?  CustomCancellationParser   = null)
        {

            if (TryParse(JSON,
                         out var bookingCancellation,
                         out var errorResponse,
                         CustomCancellationParser))
            {
                return bookingCancellation;
            }

            throw new ArgumentException("The given JSON representation of a booking cancellation is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out Cancellation, out ErrorResponse, CustomCancellationParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a booking cancellation.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="Cancellation">The parsed connector.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                 JSON,
                                       [NotNullWhen(true)]  out Cancellation?  Cancellation,
                                       [NotNullWhen(false)] out String?        ErrorResponse)

            => TryParse(JSON,
                        out Cancellation,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a booking cancellation.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="Cancellation">The parsed connector.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomCancellationParser">A delegate to parse custom booking cancellation JSON objects.</param>
        public static Boolean TryParse(JObject                                     JSON,
                                       [NotNullWhen(true)]  out Cancellation?      Cancellation,
                                       [NotNullWhen(false)] out String?            ErrorResponse,
                                       CustomJObjectParserDelegate<Cancellation>?  CustomCancellationParser)
        {

            try
            {

                Cancellation = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse CancellationReason    [mandatory]

                if (!JSON.ParseMandatory("cancellation_reason",
                                         "booking cancellation reason",
                                         CanceledReason.TryParse,
                                         out CanceledReason cancellationReason,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse WhoCanceled           [mandatory]

                if (!JSON.ParseMandatory("who_canceled",
                                         "who canceled",
                                         Role.TryParse,
                                         out Role whoCanceled,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                Cancellation = new Cancellation(
                                   cancellationReason,
                                   whoCanceled
                               );

                if (CustomCancellationParser is not null)
                    Cancellation = CustomCancellationParser(JSON,
                                                            Cancellation);

                return true;

            }
            catch (Exception e)
            {
                Cancellation   = default;
                ErrorResponse  = "The given JSON representation of a booking cancellation is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomCancellationSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomCancellationSerializer">A delegate to serialize custom booking cancellation JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<Cancellation>? CustomCancellationSerializer = null)
        {

            var json = JSONObject.Create(
                           new JProperty("cancellation_reason",   CancellationReason),
                           new JProperty("who_canceled",          WhoCanceled.ToString())
                       );

            return CustomCancellationSerializer is not null
                       ? CustomCancellationSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this booking cancellation.
        /// </summary>
        public Cancellation Clone()

            => new (
                   CancellationReason.Clone(),
                   WhoCanceled.       Clone()
               );

        #endregion


        #region Operator overloading

        #region Operator == (Cancellation1, Cancellation2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Cancellation1">Cancellation.</param>
        /// <param name="Cancellation2">Other booking cancellation.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Cancellation Cancellation1,
                                           Cancellation Cancellation2)
        {

            if (Object.ReferenceEquals(Cancellation1, Cancellation2))
                return true;

            if (Cancellation1 is null || Cancellation2 is null)
                return false;

            return Cancellation1.Equals(Cancellation2);

        }

        #endregion

        #region Operator != (Cancellation1, Cancellation2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Cancellation1">Cancellation.</param>
        /// <param name="Cancellation2">Other booking cancellation.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Cancellation Cancellation1,
                                           Cancellation Cancellation2)

            => !(Cancellation1 == Cancellation2);

        #endregion

        #region Operator <  (Cancellation1, Cancellation2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Cancellation1">A booking cancellation.</param>
        /// <param name="Cancellation2">Another booking cancellation.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Cancellation? Cancellation1,
                                          Cancellation? Cancellation2)

            => Cancellation1 is null
                   ? throw new ArgumentNullException(nameof(Cancellation1), "The given booking cancellation must not be null!")
                   : Cancellation1.CompareTo(Cancellation2) < 0;

        #endregion

        #region Operator <= (Cancellation1, Cancellation2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Cancellation1">A booking cancellation.</param>
        /// <param name="Cancellation2">Another booking cancellation.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Cancellation? Cancellation1,
                                           Cancellation? Cancellation2)

            => !(Cancellation1 > Cancellation2);

        #endregion

        #region Operator >  (Cancellation1, Cancellation2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Cancellation1">A booking cancellation.</param>
        /// <param name="Cancellation2">Another booking cancellation.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Cancellation? Cancellation1,
                                          Cancellation? Cancellation2)

            => Cancellation1 is null
                   ? throw new ArgumentNullException(nameof(Cancellation1), "The given booking cancellation must not be null!")
                   : Cancellation1.CompareTo(Cancellation2) > 0;

        #endregion

        #region Operator >= (Cancellation1, Cancellation2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Cancellation1">A booking cancellation.</param>
        /// <param name="Cancellation2">Another booking cancellation.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Cancellation? Cancellation1,
                                           Cancellation? Cancellation2)

            => !(Cancellation1 < Cancellation2);

        #endregion

        #endregion

        #region IComparable<Cancellation> Members

        #region CompareTo(Object)

        /// <summary>s
        /// Compares two booking cancellations.
        /// </summary>
        /// <param name="Object">A booking cancellation to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Cancellation cancellation
                   ? CompareTo(cancellation)
                   : throw new ArgumentException("The given object is not a booking cancellation!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(Cancellation)

        /// <summary>s
        /// Compares two booking cancellations.
        /// </summary>
        /// <param name="Object">A booking cancellation to compare with.</param>
        public Int32 CompareTo(Cancellation? Cancellation)
        {

            if (Cancellation is null)
                throw new ArgumentNullException(nameof(Cancellation), "The given booking cancellation must not be null!");

            var c = WhoCanceled.       CompareTo(Cancellation.WhoCanceled);

            if (c == 0)
                c = CancellationReason.CompareTo(Cancellation.CancellationReason);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<Cancellation> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two booking cancellation for equality.
        /// </summary>
        /// <param name="Object">Cancellation to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Cancellation cancellation &&
                   Equals(cancellation);

        #endregion

        #region Equals(Cancellation)

        /// <summary>
        /// Compares two booking cancellation for equality.
        /// </summary>
        /// <param name="Cancellation">Cancellation to compare with.</param>
        public Boolean Equals(Cancellation? Cancellation)

            => Cancellation is not null &&

               CancellationReason.Equals(Cancellation.CancellationReason) &&
               WhoCanceled.       Equals(Cancellation.WhoCanceled);

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

            => $"{WhoCanceled} canceled for '{CancellationReason}'";

        #endregion

    }

}
