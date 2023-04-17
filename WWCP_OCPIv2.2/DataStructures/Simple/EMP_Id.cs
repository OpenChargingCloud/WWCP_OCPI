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

namespace cloud.charging.open.protocols.OCPIv2_2
{

    /// <summary>
    /// Extension methods for EMP identifications.
    /// </summary>
    public static class EMPIdExtensions
    {

        /// <summary>
        /// Indicates whether this EMP identification is null or empty.
        /// </summary>
        /// <param name="EMPId">An EMP identification.</param>
        public static Boolean IsNullOrEmpty(this EMP_Id? EMPId)
            => !EMPId.HasValue || EMPId.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this EMP identification is NOT null or empty.
        /// </summary>
        /// <param name="EMPId">An EMP identification.</param>
        public static Boolean IsNotNullOrEmpty(this EMP_Id? EMPId)
            => EMPId.HasValue && EMPId.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The unique identification of an EMP.
    /// CiString(3)
    /// </summary>
    public readonly struct EMP_Id : IId<EMP_Id>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this EMP identification is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this EMP identification is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the EMP identification.
        /// </summary>
        public UInt64 Length
            => (UInt64) InternalId.Length;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new EMP identification based on the given text.
        /// </summary>
        /// <param name="Text">A text representation of an EMP identification.</param>
        private EMP_Id(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as an EMP identification.
        /// </summary>
        /// <param name="Text">A text representation of an EMP identification.</param>
        public static EMP_Id Parse(String Text)
        {

            if (TryParse(Text, out var EMPId))
                return EMPId;

            throw new ArgumentException("Invalid text representation of an EMP identification: '" + Text + "'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as an EMP identification.
        /// </summary>
        /// <param name="Text">A text representation of an EMP identification.</param>
        public static EMP_Id? TryParse(String Text)
        {

            if (TryParse(Text, out var EMPId))
                return EMPId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out EMPId)

        /// <summary>
        /// Try to parse the given text as an EMP identification.
        /// </summary>
        /// <param name="Text">A text representation of an EMP identification.</param>
        /// <param name="EMPId">The parsed EMP identification.</param>
        public static Boolean TryParse(String Text, out EMP_Id EMPId)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty() &&
                Text.Length >= 1        &&
                Text.Length <= 3)
            {
                try
                {
                    EMPId = new EMP_Id(Text);
                    return true;
                }
                catch (Exception)
                { }
            }

            EMPId = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this EMP identification.
        /// </summary>
        public EMP_Id Clone

            => new (
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Operator overloading

        #region Operator == (EMPId1, EMPId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EMPId1">An EMP identification.</param>
        /// <param name="EMPId2">Another EMP identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (EMP_Id EMPId1,
                                           EMP_Id EMPId2)

            => EMPId1.Equals(EMPId2);

        #endregion

        #region Operator != (EMPId1, EMPId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EMPId1">An EMP identification.</param>
        /// <param name="EMPId2">Another EMP identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (EMP_Id EMPId1,
                                           EMP_Id EMPId2)

            => !EMPId1.Equals(EMPId2);

        #endregion

        #region Operator <  (EMPId1, EMPId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EMPId1">An EMP identification.</param>
        /// <param name="EMPId2">Another EMP identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (EMP_Id EMPId1,
                                          EMP_Id EMPId2)

            => EMPId1.CompareTo(EMPId2) < 0;

        #endregion

        #region Operator <= (EMPId1, EMPId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EMPId1">An EMP identification.</param>
        /// <param name="EMPId2">Another EMP identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (EMP_Id EMPId1,
                                           EMP_Id EMPId2)

            => EMPId1.CompareTo(EMPId2) <= 0;

        #endregion

        #region Operator >  (EMPId1, EMPId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EMPId1">An EMP identification.</param>
        /// <param name="EMPId2">Another EMP identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (EMP_Id EMPId1,
                                          EMP_Id EMPId2)

            => EMPId1.CompareTo(EMPId2) > 0;

        #endregion

        #region Operator >= (EMPId1, EMPId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EMPId1">An EMP identification.</param>
        /// <param name="EMPId2">Another EMP identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (EMP_Id EMPId1,
                                           EMP_Id EMPId2)

            => EMPId1.CompareTo(EMPId2) >= 0;

        #endregion

        #endregion

        #region IComparable<EMPId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two EMP identifications.
        /// </summary>
        /// <param name="Object">An EMP identification to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is EMP_Id EMPId
                   ? CompareTo(EMPId)
                   : throw new ArgumentException("The given object is not an EMP identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(EMPId)

        /// <summary>
        /// Compares two EMP identifications.
        /// </summary>
        /// <param name="EMPId">An EMP identification to compare with.</param>
        public Int32 CompareTo(EMP_Id EMPId)

            => String.Compare(InternalId,
                              EMPId.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<EMPId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two EMP identifications for equality.
        /// </summary>
        /// <param name="Object">An EMP identification to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is EMP_Id EMPId &&
                   Equals(EMPId);

        #endregion

        #region Equals(EMPId)

        /// <summary>
        /// Compares two EMP identifications for equality.
        /// </summary>
        /// <param name="EMPId">An EMP identification to compare with.</param>
        public Boolean Equals(EMP_Id EMPId)

            => String.Equals(InternalId,
                             EMPId.InternalId,
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
