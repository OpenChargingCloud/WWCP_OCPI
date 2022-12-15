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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2
{

    /// <summary>
    /// Extension methods for internal EVSE identifications.
    /// </summary>
    public static class EVSEUIdExtensions
    {

        /// <summary>
        /// Indicates whether this internal EVSE identification is null or empty.
        /// </summary>
        /// <param name="EVSEUId">An internal EVSE identification.</param>
        public static Boolean IsNullOrEmpty(this EVSE_UId? EVSEUId)
            => !EVSEUId.HasValue || EVSEUId.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this internal EVSE identification is NOT null or empty.
        /// </summary>
        /// <param name="EVSEUId">An internal EVSE identification.</param>
        public static Boolean IsNotNullOrEmpty(this EVSE_UId? EVSEUId)
            => EVSEUId.HasValue && EVSEUId.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The unique internal identification of an EVSE.
    /// CiString(36)
    /// </summary>
    public readonly struct EVSE_UId : IId<EVSE_UId>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this internal EVSE identification is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this internal EVSE identification is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the internal EVSE identification.
        /// </summary>
        public UInt64 Length
            => (UInt64) InternalId.Length;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new internal EVSE identification based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of an internal EVSE identification.</param>
        private EVSE_UId(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as an internal EVSE identification.
        /// </summary>
        /// <param name="Text">A text representation of an internal EVSE identification.</param>
        public static EVSE_UId Parse(String Text)
        {

            if (TryParse(Text, out var evseUId))
                return evseUId;

            throw new ArgumentException("Invalid text representation of an internal EVSE identification: '" + Text + "'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as an internal EVSE identification.
        /// </summary>
        /// <param name="Text">A text representation of an internal EVSE identification.</param>
        public static EVSE_UId? TryParse(String Text)
        {

            if (TryParse(Text, out var evseUId))
                return evseUId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out EVSEUId)

        /// <summary>
        /// Try to parse the given text as an internal EVSE identification.
        /// </summary>
        /// <param name="Text">A text representation of an internal EVSE identification.</param>
        /// <param name="EVSEUId">The parsed internal EVSE identification.</param>
        public static Boolean TryParse(String Text, out EVSE_UId EVSEUId)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    EVSEUId = new EVSE_UId(Text);
                    return true;
                }
                catch (Exception)
                { }
            }

            EVSEUId = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this internal EVSE identification.
        /// </summary>
        public EVSE_UId Clone

            => new (
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Operator overloading

        #region Operator == (EVSEUId1, EVSEUId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVSEUId1">An internal EVSE identification.</param>
        /// <param name="EVSEUId2">Another internal EVSE identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (EVSE_UId EVSEUId1,
                                           EVSE_UId EVSEUId2)

            => EVSEUId1.Equals(EVSEUId2);

        #endregion

        #region Operator != (EVSEUId1, EVSEUId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVSEUId1">An internal EVSE identification.</param>
        /// <param name="EVSEUId2">Another internal EVSE identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (EVSE_UId EVSEUId1,
                                           EVSE_UId EVSEUId2)

            => !EVSEUId1.Equals(EVSEUId2);

        #endregion

        #region Operator <  (EVSEUId1, EVSEUId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVSEUId1">An internal EVSE identification.</param>
        /// <param name="EVSEUId2">Another internal EVSE identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (EVSE_UId EVSEUId1,
                                          EVSE_UId EVSEUId2)

            => EVSEUId1.CompareTo(EVSEUId2) < 0;

        #endregion

        #region Operator <= (EVSEUId1, EVSEUId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVSEUId1">An internal EVSE identification.</param>
        /// <param name="EVSEUId2">Another internal EVSE identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (EVSE_UId EVSEUId1,
                                           EVSE_UId EVSEUId2)

            => EVSEUId1.CompareTo(EVSEUId2) <= 0;

        #endregion

        #region Operator >  (EVSEUId1, EVSEUId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVSEUId1">An internal EVSE identification.</param>
        /// <param name="EVSEUId2">Another internal EVSE identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (EVSE_UId EVSEUId1,
                                          EVSE_UId EVSEUId2)

            => EVSEUId1.CompareTo(EVSEUId2) > 0;

        #endregion

        #region Operator >= (EVSEUId1, EVSEUId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVSEUId1">An internal EVSE identification.</param>
        /// <param name="EVSEUId2">Another internal EVSE identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (EVSE_UId EVSEUId1,
                                           EVSE_UId EVSEUId2)

            => EVSEUId1.CompareTo(EVSEUId2) >= 0;

        #endregion

        #endregion

        #region IComparable<EVSEUId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two internal EVSE identifications.
        /// </summary>
        /// <param name="Object">An internal EVSE identification to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is EVSE_UId EVSEUId
                   ? CompareTo(EVSEUId)
                   : throw new ArgumentException("The given object is not an internal EVSE identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(EVSEUId)

        /// <summary>
        /// Compares two internal EVSE identifications.
        /// </summary>
        /// <param name="EVSEUId">An internal EVSE identification to compare with.</param>
        public Int32 CompareTo(EVSE_UId EVSEUId)

            => String.Compare(InternalId,
                              EVSEUId.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<EVSEUId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two internal EVSE identifications for equality.
        /// </summary>
        /// <param name="Object">An internal EVSE identification to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is EVSE_UId EVSEUId &&
                   Equals(EVSEUId);

        #endregion

        #region Equals(EVSEUId)

        /// <summary>
        /// Compares two internal EVSE identifications for equality.
        /// </summary>
        /// <param name="EVSEUId">An internal EVSE identification to compare with.</param>
        public Boolean Equals(EVSE_UId EVSEUId)

            => String.Equals(InternalId,
                             EVSEUId.InternalId,
                             StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region GetHashCode()

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
