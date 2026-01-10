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

namespace cloud.charging.open.protocols.OCPIv3_0
{

    /// <summary>
    /// A parking restriction.
    /// </summary>
    public readonly struct ParkingRestriction : IEquatable<ParkingRestriction>,
                                                IComparable<ParkingRestriction>,
                                                IComparable
    {

        #region Properties

        /// <summary>
        /// One or more groups that drivers have to be in to be allowed to park here.
        /// </summary>
        [Mandatory]
        public IEnumerable<ParkingRestrictionGroup>  Groups                        { get; }

        /// <summary>
        /// Whether the restriction applies also outside opening hours of the establishment that the Location belongs to.
        /// This field can be used for example to create a ParkingRestriction that signals that a certain EVSE is for
        /// employees only during opening hours, but can be used by everyone outside those opening hours.
        /// </summary>
        [Mandatory]
        public Boolean                               AppliesOutsideOpeningHours    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new parking restriction.
        /// </summary>
        /// <param name="Groups">One or more groups that drivers have to be in to be allowed to park here.</param>
        /// <param name="AppliesOutsideOpeningHours">Whether the restriction applies also outside opening hours of the establishment that the Location belongs to. This field can be used for example to create a ParkingRestriction that signals that a certain EVSE is for employees only during opening hours, but can be used by everyone outside those opening hours.</param>
        public ParkingRestriction(IEnumerable<ParkingRestrictionGroup>  Groups,
                                  Boolean                               AppliesOutsideOpeningHours)
        {

            this.Groups                      = Groups;
            this.AppliesOutsideOpeningHours  = AppliesOutsideOpeningHours;

            unchecked
            {

                hashCode = this.Groups.                    CalcHashCode() * 3 ^
                           this.AppliesOutsideOpeningHours.GetHashCode();

            }

        }

        #endregion


        #region (static) Parse   (JSON, CustomParkingRestrictionParser = null)

        /// <summary>
        /// Parse the given JSON representation of a parking restriction.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomParkingRestrictionParser">A delegate to parse custom parking restriction JSON objects.</param>
        public static ParkingRestriction Parse(JObject                                           JSON,
                                               CustomJObjectParserDelegate<ParkingRestriction>?  CustomParkingRestrictionParser   = null)
        {

            if (TryParse(JSON,
                         out var parkingRestriction,
                         out var errorResponse,
                         CustomParkingRestrictionParser))
            {
                return parkingRestriction;
            }

            throw new ArgumentException("The given JSON representation of a parking restriction is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, CustomParkingRestrictionParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a parking restriction.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomParkingRestrictionParser">A delegate to parse custom parking restriction JSON objects.</param>
        public static ParkingRestriction? TryParse(JObject                                           JSON,
                                                   CustomJObjectParserDelegate<ParkingRestriction>?  CustomParkingRestrictionParser   = null)
        {

            if (TryParse(JSON,
                         out var parkingRestriction,
                         out var errorResponse,
                         CustomParkingRestrictionParser))
            {
                return parkingRestriction;
            }

            return default;

        }

        #endregion

        #region (static) TryParse(JSON, out ParkingRestriction, out ErrorResponse, CustomParkingRestrictionParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a parking restriction.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="ParkingRestriction">The parsed parking restriction.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                      JSON,
                                       [NotNullWhen(true)]  out ParkingRestriction  ParkingRestriction,
                                       [NotNullWhen(false)] out String?             ErrorResponse)

            => TryParse(JSON,
                        out ParkingRestriction,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a parking restriction.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="ParkingRestriction">The parsed parking restriction.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomParkingRestrictionParser">A delegate to parse custom parking restriction JSON objects.</param>
        public static Boolean TryParse(JObject                                           JSON,
                                       [NotNullWhen(true)]  out ParkingRestriction       ParkingRestriction,
                                       [NotNullWhen(false)] out String?                  ErrorResponse,
                                       CustomJObjectParserDelegate<ParkingRestriction>?  CustomParkingRestrictionParser   = null)
        {

            try
            {

                ParkingRestriction = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse Groups                        [mandatory]

                if (!JSON.ParseMandatoryHashSet("group",
                                                "parking restriction groups",
                                                ParkingRestrictionGroup.TryParse,
                                                out HashSet<ParkingRestrictionGroup> Groups,
                                                out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse AppliesOutsideOpeningHours    [mandatory]

                if (!JSON.ParseMandatory("applies_outside_opening_hours",
                                         "applies outside opening hours",
                                         out Boolean AppliesOutsideOpeningHours,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                ParkingRestriction = new ParkingRestriction(
                                         Groups,
                                         AppliesOutsideOpeningHours
                                     );


                if (CustomParkingRestrictionParser is not null)
                    ParkingRestriction = CustomParkingRestrictionParser(JSON,
                                                                        ParkingRestriction);

                return true;

            }
            catch (Exception e)
            {
                ParkingRestriction  = default;
                ErrorResponse       = "The given JSON representation of a parking restriction is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomParkingRestrictionSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomParkingRestrictionSerializer">A delegate to serialize custom parking restriction JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ParkingRestriction>? CustomParkingRestrictionSerializer = null)
        {

            var json = JSONObject.Create(

                           new JProperty("group",                           new JArray(Groups.Select(parkingRestrictionGroup => parkingRestrictionGroup.ToString()))),

                           new JProperty("applies_outside_opening_hours",   AppliesOutsideOpeningHours)

                       );

            return CustomParkingRestrictionSerializer is not null
                       ? CustomParkingRestrictionSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this parking restriction.
        /// </summary>
        public ParkingRestriction Clone()

            => new (
                   Groups.Select(parkingRestrictionGroup => parkingRestrictionGroup.Clone()),
                   AppliesOutsideOpeningHours
               );

        #endregion


        #region Operator overloading

        #region Operator == (ParkingRestriction1, ParkingRestriction2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ParkingRestriction1">An parking restriction.</param>
        /// <param name="ParkingRestriction2">Another parking restriction.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ParkingRestriction ParkingRestriction1,
                                           ParkingRestriction ParkingRestriction2)

            => ParkingRestriction1.Equals(ParkingRestriction2);

        #endregion

        #region Operator != (ParkingRestriction1, ParkingRestriction2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ParkingRestriction1">An parking restriction.</param>
        /// <param name="ParkingRestriction2">Another parking restriction.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ParkingRestriction ParkingRestriction1,
                                           ParkingRestriction ParkingRestriction2)

            => !ParkingRestriction1.Equals(ParkingRestriction2);

        #endregion

        #region Operator <  (ParkingRestriction1, ParkingRestriction2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ParkingRestriction1">An parking restriction.</param>
        /// <param name="ParkingRestriction2">Another parking restriction.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ParkingRestriction ParkingRestriction1,
                                          ParkingRestriction ParkingRestriction2)

            => ParkingRestriction1.CompareTo(ParkingRestriction2) < 0;

        #endregion

        #region Operator <= (ParkingRestriction1, ParkingRestriction2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ParkingRestriction1">An parking restriction.</param>
        /// <param name="ParkingRestriction2">Another parking restriction.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ParkingRestriction ParkingRestriction1,
                                           ParkingRestriction ParkingRestriction2)

            => ParkingRestriction1.CompareTo(ParkingRestriction2) <= 0;

        #endregion

        #region Operator >  (ParkingRestriction1, ParkingRestriction2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ParkingRestriction1">An parking restriction.</param>
        /// <param name="ParkingRestriction2">Another parking restriction.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ParkingRestriction ParkingRestriction1,
                                          ParkingRestriction ParkingRestriction2)

            => ParkingRestriction1.CompareTo(ParkingRestriction2) > 0;

        #endregion

        #region Operator >= (ParkingRestriction1, ParkingRestriction2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ParkingRestriction1">An parking restriction.</param>
        /// <param name="ParkingRestriction2">Another parking restriction.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ParkingRestriction ParkingRestriction1,
                                           ParkingRestriction ParkingRestriction2)

            => ParkingRestriction1.CompareTo(ParkingRestriction2) >= 0;

        #endregion

        #endregion

        #region IComparable<ParkingRestriction> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two parking restrictions.
        /// </summary>
        /// <param name="Object">An parking restriction to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is ParkingRestriction parkingRestriction
                   ? CompareTo(parkingRestriction)
                   : throw new ArgumentException("The given object is not a parking restriction!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(ParkingRestriction)

        /// <summary>
        /// Compares two parking restrictions.
        /// </summary>
        /// <param name="ParkingRestriction">An parking restriction to compare with.</param>
        public Int32 CompareTo(ParkingRestriction ParkingRestriction)
        {

            var c = Groups.Count().CompareTo(ParkingRestriction.Groups.Count());

            if (c == 0)
                c = AppliesOutsideOpeningHours.CompareTo(ParkingRestriction.AppliesOutsideOpeningHours);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<ParkingRestriction> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two parking restrictions for equality.
        /// </summary>
        /// <param name="Object">An parking restriction to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ParkingRestriction parkingRestriction &&
                   Equals(parkingRestriction);

        #endregion

        #region Equals(ParkingRestriction)

        /// <summary>
        /// Compares two parking restrictions for equality.
        /// </summary>
        /// <param name="ParkingRestriction">An parking restriction to compare with.</param>
        public Boolean Equals(ParkingRestriction ParkingRestriction)

            => Groups.                    SequenceEqual(ParkingRestriction.Groups) &&
               AppliesOutsideOpeningHours.Equals       (ParkingRestriction.AppliesOutsideOpeningHours);

        #endregion

        #endregion

        #region (override) GetHashCode()

        private readonly Int32 hashCode;

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()
            => hashCode;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"'{Groups.AggregateWith(", ")}', {(AppliesOutsideOpeningHours ? "outside opening hours" : "NOT outside opening hours")}";

        #endregion

    }

}
