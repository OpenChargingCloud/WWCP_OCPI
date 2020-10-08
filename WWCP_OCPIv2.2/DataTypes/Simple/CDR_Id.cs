/*
 * Copyright (c) 2014-2020 GraphDefined GmbH
 * This file is part of WWCP Core <https://github.com/OpenChargingCloud/WWCP_Core>
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#region Usings

using System;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2
{

    /// <summary>
    /// The unique identification of a charge detail record.
    /// </summary>
    public readonly struct CDR_Id : IId<CDR_Id>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this identification is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty

            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// The length of the charge detail record identification.
        /// </summary>
        public UInt64 Length

            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new charge detail record identification based on the given string.
        /// </summary>
        /// <param name="String">The string representation of the charge detail record identification.</param>
        private CDR_Id(String String)
        {
            this.InternalId  = String;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a charge detail record identification.
        /// </summary>
        /// <param name="Text">A text representation of a charge detail record identification.</param>
        public static CDR_Id Parse(String Text)
        {

            if (TryParse(Text, out CDR_Id cdrId))
                return cdrId;

            if (Text.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Text), "The given text representation of a charge detail record identification must not be null or empty!");

            throw new ArgumentException("The given text representation of a charge detail record identification is invalid!", nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a charge detail record identification.
        /// </summary>
        /// <param name="Text">A text representation of a charge detail record identification.</param>
        public static CDR_Id? TryParse(String Text)
        {

            if (TryParse(Text, out CDR_Id cdrId))
                return cdrId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out CDRId)

        /// <summary>
        /// Try to parse the given text as a charge detail record identification.
        /// </summary>
        /// <param name="Text">A text representation of a charge detail record identification.</param>
        /// <param name="CDRId">The parsed charge detail record identification.</param>
        public static Boolean TryParse(String Text, out CDR_Id CDRId)
        {

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    CDRId = new CDR_Id(Text.Trim());
                    return true;
                }
                catch (Exception)
                { }
            }

            CDRId = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this charge detail record identification.
        /// </summary>
        public CDR_Id Clone

            => new CDR_Id(
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Operator overloading

        #region Operator == (CDRId1, CDRId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDRId1">A charge detail record identification.</param>
        /// <param name="CDRId2">Another charge detail record identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (CDR_Id CDRId1,
                                           CDR_Id CDRId2)

            => CDRId1.Equals(CDRId2);

        #endregion

        #region Operator != (CDRId1, CDRId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDRId1">A charge detail record identification.</param>
        /// <param name="CDRId2">Another charge detail record identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (CDR_Id CDRId1,
                                           CDR_Id CDRId2)

            => !(CDRId1 == CDRId2);

        #endregion

        #region Operator <  (CDRId1, CDRId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDRId1">A charge detail record identification.</param>
        /// <param name="CDRId2">Another charge detail record identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (CDR_Id CDRId1,
                                          CDR_Id CDRId2)

            => CDRId1.CompareTo(CDRId2) < 0;

        #endregion

        #region Operator <= (CDRId1, CDRId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDRId1">A charge detail record identification.</param>
        /// <param name="CDRId2">Another charge detail record identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (CDR_Id CDRId1,
                                           CDR_Id CDRId2)

            => !(CDRId1 > CDRId2);

        #endregion

        #region Operator >  (CDRId1, CDRId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDRId1">A charge detail record identification.</param>
        /// <param name="CDRId2">Another charge detail record identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (CDR_Id CDRId1,
                                          CDR_Id CDRId2)

            => CDRId1.CompareTo(CDRId2) > 0;

        #endregion

        #region Operator >= (CDRId1, CDRId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDRId1">A charge detail record identification.</param>
        /// <param name="CDRId2">Another charge detail record identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (CDR_Id CDRId1,
                                           CDR_Id CDRId2)

            => !(CDRId1 < CDRId2);

        #endregion

        #endregion

        #region IComparable<CDRId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)
        {

            if (Object is CDR_Id cdrId)
                return CompareTo(cdrId);

            throw new ArgumentException("The given object is not a charge detail record identification!",
                                        nameof(Object));

        }

        #endregion

        #region CompareTo(CDRId)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDRId">An object to compare with.</param>
        public Int32 CompareTo(CDR_Id CDRId)

            => String.Compare(InternalId,
                              CDRId.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<CDRId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)
        {

            if (Object is CDR_Id cdrId)
                return Equals(cdrId);

            return false;

        }

        #endregion

        #region Equals(CDRId)

        /// <summary>
        /// Compares two charge detail record identifications for equality.
        /// </summary>
        /// <param name="CDRId">An charge detail record identification to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(CDR_Id CDRId)

            => String.Equals(InternalId,
                             CDRId.InternalId,
                             StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
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
