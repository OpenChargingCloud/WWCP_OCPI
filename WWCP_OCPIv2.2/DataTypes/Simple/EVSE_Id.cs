/*
 * Copyright (c) 2014-2020 GraphDefined GmbH
 * This file is part of WWCP OCPI <https://github.com/OpenChargingCloud/WWCP_OCPI>
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
    /// The official unique identification of an EVSE.
    /// </summary>
    public readonly struct EVSE_Id : IId<EVSE_Id>
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
        /// The length of the EVSE.
        /// </summary>
        public UInt64 Length

            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new EVSE based on the given string.
        /// </summary>
        /// <param name="String">The string representation of the EVSE.</param>
        private EVSE_Id(String String)
        {
            this.InternalId  = String;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as an EVSE.
        /// </summary>
        /// <param name="Text">A text representation of an EVSE.</param>
        public static EVSE_Id Parse(String Text)
        {

            if (TryParse(Text, out EVSE_Id locationId))
                return locationId;

            if (Text.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Text), "The given text representation of an EVSE must not be null or empty!");

            throw new ArgumentException("The given text representation of an EVSE is invalid!", nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as an EVSE.
        /// </summary>
        /// <param name="Text">A text representation of an EVSE.</param>
        public static EVSE_Id? TryParse(String Text)
        {

            if (TryParse(Text, out EVSE_Id locationId))
                return locationId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out EVSEId)

        /// <summary>
        /// Try to parse the given text as an EVSE.
        /// </summary>
        /// <param name="Text">A text representation of an EVSE.</param>
        /// <param name="EVSEId">The parsed EVSE.</param>
        public static Boolean TryParse(String Text, out EVSE_Id EVSEId)
        {

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    EVSEId = new EVSE_Id(Text.Trim());
                    return true;
                }
                catch (Exception)
                { }
            }

            EVSEId = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this EVSE.
        /// </summary>
        public EVSE_Id Clone

            => new EVSE_Id(
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Operator overloading

        #region Operator == (EVSEId1, EVSEId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVSEId1">A EVSE.</param>
        /// <param name="EVSEId2">Another EVSE.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (EVSE_Id EVSEId1,
                                           EVSE_Id EVSEId2)

            => EVSEId1.Equals(EVSEId2);

        #endregion

        #region Operator != (EVSEId1, EVSEId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVSEId1">A EVSE.</param>
        /// <param name="EVSEId2">Another EVSE.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (EVSE_Id EVSEId1,
                                           EVSE_Id EVSEId2)

            => !(EVSEId1 == EVSEId2);

        #endregion

        #region Operator <  (EVSEId1, EVSEId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVSEId1">A EVSE.</param>
        /// <param name="EVSEId2">Another EVSE.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (EVSE_Id EVSEId1,
                                          EVSE_Id EVSEId2)

            => EVSEId1.CompareTo(EVSEId2) < 0;

        #endregion

        #region Operator <= (EVSEId1, EVSEId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVSEId1">A EVSE.</param>
        /// <param name="EVSEId2">Another EVSE.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (EVSE_Id EVSEId1,
                                           EVSE_Id EVSEId2)

            => !(EVSEId1 > EVSEId2);

        #endregion

        #region Operator >  (EVSEId1, EVSEId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVSEId1">A EVSE.</param>
        /// <param name="EVSEId2">Another EVSE.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (EVSE_Id EVSEId1,
                                          EVSE_Id EVSEId2)

            => EVSEId1.CompareTo(EVSEId2) > 0;

        #endregion

        #region Operator >= (EVSEId1, EVSEId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVSEId1">A EVSE.</param>
        /// <param name="EVSEId2">Another EVSE.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (EVSE_Id EVSEId1,
                                           EVSE_Id EVSEId2)

            => !(EVSEId1 < EVSEId2);

        #endregion

        #endregion

        #region IComparable<EVSEId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is EVSE_Id locationId
                   ? CompareTo(locationId)
                   : throw new ArgumentException("The given object is not an EVSE!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(EVSEId)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVSEId">An object to compare with.</param>
        public Int32 CompareTo(EVSE_Id EVSEId)

            => String.Compare(InternalId,
                              EVSEId.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<EVSEId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is EVSE_Id locationId &&
                   Equals(locationId);

        #endregion

        #region Equals(EVSEId)

        /// <summary>
        /// Compares two EVSEs for equality.
        /// </summary>
        /// <param name="EVSEId">An EVSE to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(EVSE_Id EVSEId)

            => String.Equals(InternalId,
                             EVSEId.InternalId,
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
