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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPI
{

    /// <summary>
    /// Extension methods for official EVSE identifications.
    /// </summary>
    public static class EVSEIdExtensions
    {

        /// <summary>
        /// Indicates whether this official EVSE identification is null or empty.
        /// </summary>
        /// <param name="EVSEId">An official EVSE identification.</param>
        public static Boolean IsNullOrEmpty(this EVSE_Id? EVSEId)
            => !EVSEId.HasValue || EVSEId.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this official EVSE identification is NOT null or empty.
        /// </summary>
        /// <param name="EVSEId">An official EVSE identification.</param>
        public static Boolean IsNotNullOrEmpty(this EVSE_Id? EVSEId)
            => EVSEId.HasValue && EVSEId.Value.IsNotNullOrEmpty;


        #region Matches(EVSEIds, Match, IgnoreCase = true)

        /// <summary>
        /// Checks whether the given enumeration of EVSE identifications matches the given text.
        /// </summary>
        /// <param name="EVSEIds">An enumeration of EVSE identifications.</param>
        /// <param name="Match">A text to match.</param>
        /// <param name="IgnoreCase">Whether to ignore the case of the text.</param>
        public static Boolean Matches(this IEnumerable<EVSE_Id>  EVSEIds,
                                      String                     Match,
                                      Boolean                    IgnoreCase  = true)

            => EVSEIds.Any(evseId => IgnoreCase
                                          ? evseId.ToString().Contains(Match, StringComparison.OrdinalIgnoreCase)
                                          : evseId.ToString().Contains(Match, StringComparison.Ordinal));

        #endregion


    }


    /// <summary>
    /// The unique official identification of an EVSE.
    /// CiString(36)
    /// </summary>
    public readonly struct EVSE_Id : IId<EVSE_Id>
    {

        #region Data

        /// <summary>
        /// The official identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this official EVSE identification is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this official EVSE identification is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the official EVSE identification.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new official EVSE identification based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of an official EVSE identification.</param>
        private EVSE_Id(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as an official EVSE identification.
        /// </summary>
        /// <param name="Text">A text representation of an official EVSE identification.</param>
        public static EVSE_Id Parse(String Text)
        {

            if (TryParse(Text, out var evseId))
                return evseId;

            throw new ArgumentException($"Invalid text representation of an official EVSE identification: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as an official EVSE identification.
        /// </summary>
        /// <param name="Text">A text representation of an official EVSE identification.</param>
        public static EVSE_Id? TryParse(String Text)
        {

            if (TryParse(Text, out var evseId))
                return evseId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out EVSEId)

        /// <summary>
        /// Try to parse the given text as an official EVSE identification.
        /// </summary>
        /// <param name="Text">A text representation of an official EVSE identification.</param>
        /// <param name="EVSEId">The parsed official EVSE identification.</param>
        public static Boolean TryParse(String Text, out EVSE_Id EVSEId)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    EVSEId = new EVSE_Id(Text);
                    return true;
                }
                catch
                { }
            }

            EVSEId = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this official EVSE identification.
        /// </summary>
        public EVSE_Id Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Operator overloading

        #region Operator == (EVSEId1, EVSEId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVSEId1">An official EVSE identification.</param>
        /// <param name="EVSEId2">Another official EVSE identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (EVSE_Id EVSEId1,
                                           EVSE_Id EVSEId2)

            => EVSEId1.Equals(EVSEId2);

        #endregion

        #region Operator != (EVSEId1, EVSEId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVSEId1">An official EVSE identification.</param>
        /// <param name="EVSEId2">Another official EVSE identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (EVSE_Id EVSEId1,
                                           EVSE_Id EVSEId2)

            => !EVSEId1.Equals(EVSEId2);

        #endregion

        #region Operator <  (EVSEId1, EVSEId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVSEId1">An official EVSE identification.</param>
        /// <param name="EVSEId2">Another official EVSE identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (EVSE_Id EVSEId1,
                                          EVSE_Id EVSEId2)

            => EVSEId1.CompareTo(EVSEId2) < 0;

        #endregion

        #region Operator <= (EVSEId1, EVSEId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVSEId1">An official EVSE identification.</param>
        /// <param name="EVSEId2">Another official EVSE identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (EVSE_Id EVSEId1,
                                           EVSE_Id EVSEId2)

            => EVSEId1.CompareTo(EVSEId2) <= 0;

        #endregion

        #region Operator >  (EVSEId1, EVSEId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVSEId1">An official EVSE identification.</param>
        /// <param name="EVSEId2">Another official EVSE identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (EVSE_Id EVSEId1,
                                          EVSE_Id EVSEId2)

            => EVSEId1.CompareTo(EVSEId2) > 0;

        #endregion

        #region Operator >= (EVSEId1, EVSEId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVSEId1">An official EVSE identification.</param>
        /// <param name="EVSEId2">Another official EVSE identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (EVSE_Id EVSEId1,
                                           EVSE_Id EVSEId2)

            => EVSEId1.CompareTo(EVSEId2) >= 0;

        #endregion

        #endregion

        #region IComparable<EVSEId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two official EVSE identifications.
        /// </summary>
        /// <param name="Object">An official EVSE identification to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is EVSE_Id EVSEId
                   ? CompareTo(EVSEId)
                   : throw new ArgumentException("The given object is not an official EVSE identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(EVSEId)

        /// <summary>
        /// Compares two official EVSE identifications.
        /// </summary>
        /// <param name="EVSEId">An official EVSE identification to compare with.</param>
        public Int32 CompareTo(EVSE_Id EVSEId)

            => String.Compare(InternalId,
                              EVSEId.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<EVSEId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two official EVSE identifications for equality.
        /// </summary>
        /// <param name="Object">An official EVSE identification to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is EVSE_Id EVSEId &&
                   Equals(EVSEId);

        #endregion

        #region Equals(EVSEId)

        /// <summary>
        /// Compares two official EVSE identifications for equality.
        /// </summary>
        /// <param name="EVSEId">An official EVSE identification to compare with.</param>
        public Boolean Equals(EVSE_Id EVSEId)

            => String.Equals(InternalId,
                             EVSEId.InternalId,
                             StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()

            => InternalId?.ToLower().GetHashCode() ?? 0;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => InternalId ?? "";

        #endregion

    }

}
