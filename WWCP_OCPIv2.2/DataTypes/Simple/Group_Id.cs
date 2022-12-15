/*
 * Copyright (c) 2015-2022 GraphDefined GmbH
 * This file is part of WWCP OCPI <https://github.com/OpenChargingCloud/WWCP_OCPI>
 *
 * Licensed under the Apache License, Group 2.0 (the "License");
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
    /// Extension methods for group identifications.
    /// </summary>
    public static class GroupIdExtensions
    {

        /// <summary>
        /// Indicates whether this group identification is null or empty.
        /// </summary>
        /// <param name="GroupId">A group identification.</param>
        public static Boolean IsNullOrEmpty(this Group_Id? GroupId)
            => !GroupId.HasValue || GroupId.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this group identification is NOT null or empty.
        /// </summary>
        /// <param name="GroupId">A group identification.</param>
        public static Boolean IsNotNullOrEmpty(this Group_Id? GroupId)
            => GroupId.HasValue && GroupId.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The unique identification of a group.
    /// CiString(36)
    /// </summary>
    public readonly struct Group_Id : IId<Group_Id>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this group identification is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this group identification is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the group identification.
        /// </summary>
        public UInt64 Length
            => (UInt64) InternalId.Length;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new group identification based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a group identification.</param>
        private Group_Id(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) NewRandom

        /// <summary>
        /// Create a new random group identification.
        /// </summary>
        public static Group_Id NewRandom

            => Parse(Guid.NewGuid().ToString());

        #endregion

        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as a group identification.
        /// </summary>
        /// <param name="Text">A text representation of a group identification.</param>
        public static Group_Id Parse(String Text)
        {

            if (TryParse(Text, out var groupId))
                return groupId;

            throw new ArgumentException("Invalid text representation of a group identification: '" + Text + "'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a group identification.
        /// </summary>
        /// <param name="Text">A text representation of a group identification.</param>
        public static Group_Id? TryParse(String Text)
        {

            if (TryParse(Text, out var groupId))
                return groupId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out GroupId)

        /// <summary>
        /// Try to parse the given text as a group identification.
        /// </summary>
        /// <param name="Text">A text representation of a group identification.</param>
        /// <param name="GroupId">The parsed group identification.</param>
        public static Boolean TryParse(String Text, out Group_Id GroupId)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    GroupId = new Group_Id(Text);
                    return true;
                }
                catch (Exception)
                { }
            }

            GroupId = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this group identification.
        /// </summary>
        public Group_Id Clone

            => new (
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Operator overloading

        #region Operator == (GroupId1, GroupId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="GroupId1">A group identification.</param>
        /// <param name="GroupId2">Another group identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Group_Id GroupId1,
                                           Group_Id GroupId2)

            => GroupId1.Equals(GroupId2);

        #endregion

        #region Operator != (GroupId1, GroupId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="GroupId1">A group identification.</param>
        /// <param name="GroupId2">Another group identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Group_Id GroupId1,
                                           Group_Id GroupId2)

            => !GroupId1.Equals(GroupId2);

        #endregion

        #region Operator <  (GroupId1, GroupId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="GroupId1">A group identification.</param>
        /// <param name="GroupId2">Another group identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Group_Id GroupId1,
                                          Group_Id GroupId2)

            => GroupId1.CompareTo(GroupId2) < 0;

        #endregion

        #region Operator <= (GroupId1, GroupId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="GroupId1">A group identification.</param>
        /// <param name="GroupId2">Another group identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Group_Id GroupId1,
                                           Group_Id GroupId2)

            => GroupId1.CompareTo(GroupId2) <= 0;

        #endregion

        #region Operator >  (GroupId1, GroupId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="GroupId1">A group identification.</param>
        /// <param name="GroupId2">Another group identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Group_Id GroupId1,
                                          Group_Id GroupId2)

            => GroupId1.CompareTo(GroupId2) > 0;

        #endregion

        #region Operator >= (GroupId1, GroupId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="GroupId1">A group identification.</param>
        /// <param name="GroupId2">Another group identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Group_Id GroupId1,
                                           Group_Id GroupId2)

            => GroupId1.CompareTo(GroupId2) >= 0;

        #endregion

        #endregion

        #region IComparable<GroupId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two group identifications.
        /// </summary>
        /// <param name="Object">A group identification to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Group_Id groupId
                   ? CompareTo(groupId)
                   : throw new ArgumentException("The given object is not a group identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(GroupId)

        /// <summary>
        /// Compares two group identifications.
        /// </summary>
        /// <param name="GroupId">A group identification to compare with.</param>
        public Int32 CompareTo(Group_Id GroupId)

            => String.Compare(InternalId,
                              GroupId.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<GroupId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two group identifications for equality.
        /// </summary>
        /// <param name="Object">A group identification to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Group_Id groupId &&
                   Equals(groupId);

        #endregion

        #region Equals(GroupId)

        /// <summary>
        /// Compares two group identifications for equality.
        /// </summary>
        /// <param name="GroupId">A group identification to compare with.</param>
        public Boolean Equals(Group_Id GroupId)

            => String.Equals(InternalId,
                             GroupId.InternalId,
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
