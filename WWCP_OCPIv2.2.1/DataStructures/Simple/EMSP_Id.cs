/*
 * Copyright (c) 2015-2023 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

namespace cloud.charging.open.protocols.OCPIv2_2_1
{

    /// <summary>
    /// Extension methods for EMSP identifications.
    /// </summary>
    public static class EMSPIdExtensions
    {

        /// <summary>
        /// Indicates whether this EMSP identification is null or empty.
        /// </summary>
        /// <param name="EMSPId">An EMSP identification.</param>
        public static Boolean IsNullOrEmpty(this EMSP_Id? EMSPId)
            => !EMSPId.HasValue || EMSPId.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this EMSP identification is NOT null or empty.
        /// </summary>
        /// <param name="EMSPId">An EMSP identification.</param>
        public static Boolean IsNotNullOrEmpty(this EMSP_Id? EMSPId)
            => EMSPId.HasValue && EMSPId.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The unique identification of an EMSP.
    /// CiString(3)
    /// </summary>
    public readonly struct EMSP_Id : IId<EMSP_Id>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this EMSP identification is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this EMSP identification is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the EMSP identification.
        /// </summary>
        public UInt64 Length
            => (UInt64) InternalId.Length;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new EMSP identification based on the given text.
        /// </summary>
        /// <param name="Text">A text representation of an EMSP identification.</param>
        private EMSP_Id(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as an EMSP identification.
        /// </summary>
        /// <param name="Text">A text representation of an EMSP identification.</param>
        public static EMSP_Id Parse(String Text)
        {

            if (TryParse(Text, out var EMSPId))
                return EMSPId;

            throw new ArgumentException("Invalid text representation of an EMSP identification: '" + Text + "'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as an EMSP identification.
        /// </summary>
        /// <param name="Text">A text representation of an EMSP identification.</param>
        public static EMSP_Id? TryParse(String Text)
        {

            if (TryParse(Text, out var EMSPId))
                return EMSPId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out EMSPId)

        /// <summary>
        /// Try to parse the given text as an EMSP identification.
        /// </summary>
        /// <param name="Text">A text representation of an EMSP identification.</param>
        /// <param name="EMSPId">The parsed EMSP identification.</param>
        public static Boolean TryParse(String Text, out EMSP_Id EMSPId)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    EMSPId = new EMSP_Id(Text);
                    return true;
                }
                catch (Exception)
                { }
            }

            EMSPId = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this EMSP identification.
        /// </summary>
        public EMSP_Id Clone

            => new (
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Operator overloading

        #region Operator == (EMSPId1, EMSPId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EMSPId1">An EMSP identification.</param>
        /// <param name="EMSPId2">Another EMSP identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (EMSP_Id EMSPId1,
                                           EMSP_Id EMSPId2)

            => EMSPId1.Equals(EMSPId2);

        #endregion

        #region Operator != (EMSPId1, EMSPId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EMSPId1">An EMSP identification.</param>
        /// <param name="EMSPId2">Another EMSP identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (EMSP_Id EMSPId1,
                                           EMSP_Id EMSPId2)

            => !EMSPId1.Equals(EMSPId2);

        #endregion

        #region Operator <  (EMSPId1, EMSPId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EMSPId1">An EMSP identification.</param>
        /// <param name="EMSPId2">Another EMSP identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (EMSP_Id EMSPId1,
                                          EMSP_Id EMSPId2)

            => EMSPId1.CompareTo(EMSPId2) < 0;

        #endregion

        #region Operator <= (EMSPId1, EMSPId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EMSPId1">An EMSP identification.</param>
        /// <param name="EMSPId2">Another EMSP identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (EMSP_Id EMSPId1,
                                           EMSP_Id EMSPId2)

            => EMSPId1.CompareTo(EMSPId2) <= 0;

        #endregion

        #region Operator >  (EMSPId1, EMSPId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EMSPId1">An EMSP identification.</param>
        /// <param name="EMSPId2">Another EMSP identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (EMSP_Id EMSPId1,
                                          EMSP_Id EMSPId2)

            => EMSPId1.CompareTo(EMSPId2) > 0;

        #endregion

        #region Operator >= (EMSPId1, EMSPId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EMSPId1">An EMSP identification.</param>
        /// <param name="EMSPId2">Another EMSP identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (EMSP_Id EMSPId1,
                                           EMSP_Id EMSPId2)

            => EMSPId1.CompareTo(EMSPId2) >= 0;

        #endregion

        #endregion

        #region IComparable<EMSPId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two EMSP identifications.
        /// </summary>
        /// <param name="Object">An EMSP identification to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is EMSP_Id EMSPId
                   ? CompareTo(EMSPId)
                   : throw new ArgumentException("The given object is not an EMSP identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(EMSPId)

        /// <summary>
        /// Compares two EMSP identifications.
        /// </summary>
        /// <param name="EMSPId">An EMSP identification to compare with.</param>
        public Int32 CompareTo(EMSP_Id EMSPId)

            => String.Compare(InternalId,
                              EMSPId.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<EMSPId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two EMSP identifications for equality.
        /// </summary>
        /// <param name="Object">An EMSP identification to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is EMSP_Id EMSPId &&
                   Equals(EMSPId);

        #endregion

        #region Equals(EMSPId)

        /// <summary>
        /// Compares two EMSP identifications for equality.
        /// </summary>
        /// <param name="EMSPId">An EMSP identification to compare with.</param>
        public Boolean Equals(EMSP_Id EMSPId)

            => String.Equals(InternalId,
                             EMSPId.InternalId,
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
