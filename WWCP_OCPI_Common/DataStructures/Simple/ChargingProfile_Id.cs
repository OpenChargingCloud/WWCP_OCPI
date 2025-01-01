/*
 * Copyright (c) 2015-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPI <https://github.com/OpenChargingCloud/WWCP_OCPI>
 *
 * Licensed under the Apache License, ChargingProfile 2.0 (the "License");
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

namespace cloud.charging.open.protocols.OCPI
{

    /// <summary>
    /// Extension methods for charging profile identifications.
    /// </summary>
    public static class ChargingProfileIdExtensions
    {

        /// <summary>
        /// Indicates whether this charging profile identification is null or empty.
        /// </summary>
        /// <param name="ChargingProfileId">A charging profile identification.</param>
        public static Boolean IsNullOrEmpty(this ChargingProfile_Id? ChargingProfileId)
            => !ChargingProfileId.HasValue || ChargingProfileId.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this charging profile identification is NOT null or empty.
        /// </summary>
        /// <param name="ChargingProfileId">A charging profile identification.</param>
        public static Boolean IsNotNullOrEmpty(this ChargingProfile_Id? ChargingProfileId)
            => ChargingProfileId.HasValue && ChargingProfileId.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The unique identification of a charging profile.
    /// </summary>
    public readonly struct ChargingProfile_Id : IId<ChargingProfile_Id>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this charging profile identification is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this charging profile identification is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the charging profile identification.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new charging profile identification based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a charging profile identification.</param>
        private ChargingProfile_Id(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) NewRandom

        /// <summary>
        /// Create a new random charging profile identification.
        /// </summary>
        public static ChargingProfile_Id NewRandom

            => Parse(Guid.NewGuid().ToString());

        #endregion

        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as a charging profile identification.
        /// </summary>
        /// <param name="Text">A text representation of a charging profile identification.</param>
        public static ChargingProfile_Id Parse(String Text)
        {

            if (TryParse(Text, out var chargingProfileId))
                return chargingProfileId;

            throw new ArgumentException($"Invalid text representation of a charging profile identification: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a charging profile identification.
        /// </summary>
        /// <param name="Text">A text representation of a charging profile identification.</param>
        public static ChargingProfile_Id? TryParse(String Text)
        {

            if (TryParse(Text, out var chargingProfileId))
                return chargingProfileId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out ChargingProfileId)

        /// <summary>
        /// Try to parse the given text as a charging profile identification.
        /// </summary>
        /// <param name="Text">A text representation of a charging profile identification.</param>
        /// <param name="ChargingProfileId">The parsed charging profile identification.</param>
        public static Boolean TryParse(String Text, out ChargingProfile_Id ChargingProfileId)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    ChargingProfileId = new ChargingProfile_Id(Text);
                    return true;
                }
                catch
                { }
            }

            ChargingProfileId = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this charging profile identification.
        /// </summary>
        public ChargingProfile_Id Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Operator overloading

        #region Operator == (ChargingProfileId1, ChargingProfileId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingProfileId1">A charging profile identification.</param>
        /// <param name="ChargingProfileId2">Another charging profile identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ChargingProfile_Id ChargingProfileId1,
                                           ChargingProfile_Id ChargingProfileId2)

            => ChargingProfileId1.Equals(ChargingProfileId2);

        #endregion

        #region Operator != (ChargingProfileId1, ChargingProfileId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingProfileId1">A charging profile identification.</param>
        /// <param name="ChargingProfileId2">Another charging profile identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ChargingProfile_Id ChargingProfileId1,
                                           ChargingProfile_Id ChargingProfileId2)

            => !ChargingProfileId1.Equals(ChargingProfileId2);

        #endregion

        #region Operator <  (ChargingProfileId1, ChargingProfileId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingProfileId1">A charging profile identification.</param>
        /// <param name="ChargingProfileId2">Another charging profile identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ChargingProfile_Id ChargingProfileId1,
                                          ChargingProfile_Id ChargingProfileId2)

            => ChargingProfileId1.CompareTo(ChargingProfileId2) < 0;

        #endregion

        #region Operator <= (ChargingProfileId1, ChargingProfileId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingProfileId1">A charging profile identification.</param>
        /// <param name="ChargingProfileId2">Another charging profile identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ChargingProfile_Id ChargingProfileId1,
                                           ChargingProfile_Id ChargingProfileId2)

            => ChargingProfileId1.CompareTo(ChargingProfileId2) <= 0;

        #endregion

        #region Operator >  (ChargingProfileId1, ChargingProfileId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingProfileId1">A charging profile identification.</param>
        /// <param name="ChargingProfileId2">Another charging profile identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ChargingProfile_Id ChargingProfileId1,
                                          ChargingProfile_Id ChargingProfileId2)

            => ChargingProfileId1.CompareTo(ChargingProfileId2) > 0;

        #endregion

        #region Operator >= (ChargingProfileId1, ChargingProfileId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingProfileId1">A charging profile identification.</param>
        /// <param name="ChargingProfileId2">Another charging profile identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ChargingProfile_Id ChargingProfileId1,
                                           ChargingProfile_Id ChargingProfileId2)

            => ChargingProfileId1.CompareTo(ChargingProfileId2) >= 0;

        #endregion

        #endregion

        #region IComparable<ChargingProfileId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two charging profile identifications.
        /// </summary>
        /// <param name="Object">A charging profile identification to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is ChargingProfile_Id chargingProfileId
                   ? CompareTo(chargingProfileId)
                   : throw new ArgumentException("The given object is not a charging profile identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(ChargingProfileId)

        /// <summary>
        /// Compares two charging profile identifications.
        /// </summary>
        /// <param name="ChargingProfileId">A charging profile identification to compare with.</param>
        public Int32 CompareTo(ChargingProfile_Id ChargingProfileId)

            => String.Compare(InternalId,
                              ChargingProfileId.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<ChargingProfileId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two charging profile identifications for equality.
        /// </summary>
        /// <param name="Object">A charging profile identification to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ChargingProfile_Id chargingProfileId &&
                   Equals(chargingProfileId);

        #endregion

        #region Equals(ChargingProfileId)

        /// <summary>
        /// Compares two charging profile identifications for equality.
        /// </summary>
        /// <param name="ChargingProfileId">A charging profile identification to compare with.</param>
        public Boolean Equals(ChargingProfile_Id ChargingProfileId)

            => String.Equals(InternalId,
                             ChargingProfileId.InternalId,
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
