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

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_3_0
{

    /// <summary>
    /// A link between an EVSE and a parking object.
    /// The presence of an EVSEParking object in an EVSE indicates that a certain
    /// parking space can be used when charging at that EVSE.
    /// </summary>
    public class EVSEParking : IEquatable<EVSEParking>,
                               IComparable<EVSEParking>,
                               IComparable
    {

        #region Properties

        /// <summary>
        /// The identification of the parking space.
        /// It refers to a parking object from the containing location’s parking_places field.
        /// </summary>
        [Mandatory]
        public Parking_Id     ParkingId       { get; }

        /// <summary>
        /// The optional position of the EVSE relative to the parking space.
        /// </summary>
        [Optional]
        public EVSEPosition?  EVSEPosition    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new EVSEParking.
        /// </summary>
        /// <param name="ParkingId">The identification of the parking space.</param>
        /// <param name="EVSEPosition">The optional position of the EVSE relative to the parking space.</param>
        public EVSEParking(Parking_Id     ParkingId,
                           EVSEPosition?  EVSEPosition   = null)

        {

            this.ParkingId     = ParkingId;
            this.EVSEPosition  = EVSEPosition;

        }

        #endregion


        #region (static) Parse   (JSON, CustomEVSEParkingParser = null)

        /// <summary>
        /// Parse the given JSON representation of an EVSEParking.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomEVSEParkingParser">A delegate to parse custom EVSEParking JSON objects.</param>
        public static EVSEParking Parse(JObject                                    JSON,
                                        CustomJObjectParserDelegate<EVSEParking>?  CustomEVSEParkingParser   = null)
        {

            if (TryParse(JSON,
                         out var evseParking,
                         out var errorResponse,
                         CustomEVSEParkingParser))
            {
                return evseParking;
            }

            throw new ArgumentException("The given JSON representation of an EVSEParking is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out EVSEParking, out ErrorResponse, EVSEParkingIdURL = null, CustomEVSEParkingParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of an EVSEParking.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="EVSEParking">The parsed EVSEParking.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                JSON,
                                       [NotNullWhen(true)]  out EVSEParking?  EVSEParking,
                                       [NotNullWhen(false)] out String?       ErrorResponse)

            => TryParse(JSON,
                        out EVSEParking,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of an EVSEParking.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="EVSEParking">The parsed EVSEParking.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomEVSEParkingParser">A delegate to parse custom EVSEParking JSON objects.</param>
        public static Boolean TryParse(JObject                                    JSON,
                                       [NotNullWhen(true)]  out EVSEParking?      EVSEParking,
                                       [NotNullWhen(false)] out String?           ErrorResponse,
                                       CustomJObjectParserDelegate<EVSEParking>?  CustomEVSEParkingParser   = null)
        {

            try
            {

                EVSEParking = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse ParkingId       [mandatory]

                if (!JSON.ParseMandatory("parking_id",
                                         "parking identification",
                                         Parking_Id.TryParse,
                                         out Parking_Id parkingId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse EVSEPosition    [optional]

                if (JSON.ParseOptional("evse_position",
                                       "EVSE position",
                                       OCPIv2_3_0.EVSEPosition.TryParse,
                                       out EVSEPosition evsePosition,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                EVSEParking = new EVSEParking(
                                  parkingId,
                                  evsePosition
                              );


                if (CustomEVSEParkingParser is not null)
                    EVSEParking = CustomEVSEParkingParser(JSON,
                                                          EVSEParking);

                return true;

            }
            catch (Exception e)
            {
                EVSEParking    = default;
                ErrorResponse  = "The given JSON representation of an EVSEParking is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomEVSEParkingSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomEVSEParkingSerializer">A delegate to serialize custom EVSEParking JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<EVSEParking>? CustomEVSEParkingSerializer = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("parking_id",      ParkingId.   ToString()),

                           EVSEPosition.HasValue
                               ? new JProperty("evse_position",   EVSEPosition.ToString())
                               : null

                       );

            return CustomEVSEParkingSerializer is not null
                       ? CustomEVSEParkingSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this EVSEParking.
        /// </summary>
        public EVSEParking Clone()

            => new (
                   ParkingId.    Clone(),
                   EVSEPosition?.Clone()
               );

        #endregion


        #region Operator overloading

        #region Operator == (EVSEParking1, EVSEParking2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVSEParking1">An EVSEParking.</param>
        /// <param name="EVSEParking2">Another EVSEParking.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (EVSEParking EVSEParking1,
                                           EVSEParking EVSEParking2)
        {

            if (Object.ReferenceEquals(EVSEParking1, EVSEParking2))
                return true;

            if (EVSEParking1 is null || EVSEParking2 is null)
                return false;

            return EVSEParking1.Equals(EVSEParking2);

        }

        #endregion

        #region Operator != (EVSEParking1, EVSEParking2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVSEParking1">An EVSEParking.</param>
        /// <param name="EVSEParking2">Another EVSEParking.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (EVSEParking EVSEParking1,
                                           EVSEParking EVSEParking2)

            => !(EVSEParking1 == EVSEParking2);

        #endregion

        #region Operator <  (EVSEParking1, EVSEParking2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVSEParking1">An EVSEParking.</param>
        /// <param name="EVSEParking2">Another EVSEParking.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (EVSEParking EVSEParking1,
                                          EVSEParking EVSEParking2)

            => EVSEParking1 is null
                   ? throw new ArgumentNullException(nameof(EVSEParking1), "The given EVSEParking must not be null!")
                   : EVSEParking1.CompareTo(EVSEParking2) < 0;

        #endregion

        #region Operator <= (EVSEParking1, EVSEParking2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVSEParking1">An EVSEParking.</param>
        /// <param name="EVSEParking2">Another EVSEParking.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (EVSEParking EVSEParking1,
                                           EVSEParking EVSEParking2)

            => !(EVSEParking1 > EVSEParking2);

        #endregion

        #region Operator >  (EVSEParking1, EVSEParking2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVSEParking1">An EVSEParking.</param>
        /// <param name="EVSEParking2">Another EVSEParking.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (EVSEParking EVSEParking1,
                                          EVSEParking EVSEParking2)

            => EVSEParking1 is null
                   ? throw new ArgumentNullException(nameof(EVSEParking1), "The given EVSEParking must not be null!")
                   : EVSEParking1.CompareTo(EVSEParking2) > 0;

        #endregion

        #region Operator >= (EVSEParking1, EVSEParking2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVSEParking1">An EVSEParking.</param>
        /// <param name="EVSEParking2">Another EVSEParking.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (EVSEParking EVSEParking1,
                                           EVSEParking EVSEParking2)

            => !(EVSEParking1 < EVSEParking2);

        #endregion

        #endregion

        #region IComparable<EVSEParking> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two EVSEParkings.
        /// </summary>
        /// <param name="Object">An EVSEParking to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is EVSEParking evseParking
                   ? CompareTo(evseParking)
                   : throw new ArgumentException("The given object is not an EVSEParking!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(EVSEParking)

        /// <summary>
        /// Compares two EVSEParkings.
        /// </summary>
        /// <param name="EVSEParking">An EVSEParking to compare with.</param>
        public Int32 CompareTo(EVSEParking? EVSEParking)
        {

            if (EVSEParking is null)
                throw new ArgumentNullException(nameof(EVSEParking), "The given EVSEParking must not be null!");

            var c = ParkingId.CompareTo(EVSEParking.ParkingId);

            if (c == 0 && EVSEPosition.HasValue && EVSEParking.EVSEPosition.HasValue)
                c = EVSEPosition.Value.CompareTo(EVSEParking.EVSEPosition.Value);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<EVSEParking> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two EVSEParkings for equality.
        /// </summary>
        /// <param name="Object">An EVSEParking to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is EVSEParking evseParking &&
                   Equals(evseParking);

        #endregion

        #region Equals(EVSEParking)

        /// <summary>
        /// Compares two EVSEParkings for equality.
        /// </summary>
        /// <param name="EVSEParking">An EVSEParking to compare with.</param>
        public Boolean Equals(EVSEParking? EVSEParking)

            => EVSEParking is not null &&

               ParkingId.Equals(EVSEParking.ParkingId) &&

            ((!EVSEPosition.HasValue && !EVSEParking.EVSEPosition.HasValue) ||
              (EVSEPosition.HasValue &&  EVSEParking.EVSEPosition.HasValue && EVSEPosition.Equals(EVSEParking.EVSEPosition)));

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Get the hash code of this object.
        /// </summary>
        public override Int32 GetHashCode()

            => ParkingId.    GetHashCode() * 3 ^
               EVSEPosition?.GetHashCode() ?? 0;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   $"'{ParkingId}'",

                   EVSEPosition.HasValue
                       ? $", {EVSEPosition}"
                       : ""

               );

        #endregion

    }

}
