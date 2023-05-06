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
    /// Extension methods for CPO identifications.
    /// </summary>
    public static class CPOIdExtensions
    {

        /// <summary>
        /// Indicates whether this CPO identification is null or empty.
        /// </summary>
        /// <param name="CPOId">A CPO identification.</param>
        public static Boolean IsNullOrEmpty(this CPO_Id? CPOId)
            => !CPOId.HasValue || CPOId.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this CPO identification is NOT null or empty.
        /// </summary>
        /// <param name="CPOId">A CPO identification.</param>
        public static Boolean IsNotNullOrEmpty(this CPO_Id? CPOId)
            => CPOId.HasValue && CPOId.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The unique identification of a CPO.
    /// CiString(3)
    /// </summary>
    public readonly struct CPO_Id : IId<CPO_Id>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this CPO identification is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this CPO identification is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the CPO identification.
        /// </summary>
        public UInt64 Length
            => (UInt64) InternalId.Length;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new CPO identification based on the given text.
        /// </summary>
        /// <param name="Text">A text representation of a CPO identification.</param>
        private CPO_Id(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as a CPO identification.
        /// </summary>
        /// <param name="Text">A text representation of a CPO identification.</param>
        public static CPO_Id Parse(String Text)
        {

            if (TryParse(Text, out var CPOId))
                return CPOId;

            throw new ArgumentException("Invalid text representation of a CPO identification: '" + Text + "'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a CPO identification.
        /// </summary>
        /// <param name="Text">A text representation of a CPO identification.</param>
        public static CPO_Id? TryParse(String Text)
        {

            if (TryParse(Text, out var CPOId))
                return CPOId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out CPOId)

        /// <summary>
        /// Try to parse the given text as a CPO identification.
        /// </summary>
        /// <param name="Text">A text representation of a CPO identification.</param>
        /// <param name="CPOId">The parsed CPO identification.</param>
        public static Boolean TryParse(String Text, out CPO_Id CPOId)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    CPOId = new CPO_Id(Text);
                    return true;
                }
                catch (Exception)
                { }
            }

            CPOId = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this CPO identification.
        /// </summary>
        public CPO_Id Clone

            => new (
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Operator overloading

        #region Operator == (CPOId1, CPOId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CPOId1">A CPO identification.</param>
        /// <param name="CPOId2">Another CPO identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (CPO_Id CPOId1,
                                           CPO_Id CPOId2)

            => CPOId1.Equals(CPOId2);

        #endregion

        #region Operator != (CPOId1, CPOId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CPOId1">A CPO identification.</param>
        /// <param name="CPOId2">Another CPO identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (CPO_Id CPOId1,
                                           CPO_Id CPOId2)

            => !CPOId1.Equals(CPOId2);

        #endregion

        #region Operator <  (CPOId1, CPOId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CPOId1">A CPO identification.</param>
        /// <param name="CPOId2">Another CPO identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (CPO_Id CPOId1,
                                          CPO_Id CPOId2)

            => CPOId1.CompareTo(CPOId2) < 0;

        #endregion

        #region Operator <= (CPOId1, CPOId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CPOId1">A CPO identification.</param>
        /// <param name="CPOId2">Another CPO identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (CPO_Id CPOId1,
                                           CPO_Id CPOId2)

            => CPOId1.CompareTo(CPOId2) <= 0;

        #endregion

        #region Operator >  (CPOId1, CPOId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CPOId1">A CPO identification.</param>
        /// <param name="CPOId2">Another CPO identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (CPO_Id CPOId1,
                                          CPO_Id CPOId2)

            => CPOId1.CompareTo(CPOId2) > 0;

        #endregion

        #region Operator >= (CPOId1, CPOId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CPOId1">A CPO identification.</param>
        /// <param name="CPOId2">Another CPO identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (CPO_Id CPOId1,
                                           CPO_Id CPOId2)

            => CPOId1.CompareTo(CPOId2) >= 0;

        #endregion

        #endregion

        #region IComparable<CPOId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two CPO identifications.
        /// </summary>
        /// <param name="Object">A CPO identification to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is CPO_Id CPOId
                   ? CompareTo(CPOId)
                   : throw new ArgumentException("The given object is not a CPO identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(CPOId)

        /// <summary>
        /// Compares two CPO identifications.
        /// </summary>
        /// <param name="CPOId">A CPO identification to compare with.</param>
        public Int32 CompareTo(CPO_Id CPOId)

            => String.Compare(InternalId,
                              CPOId.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<CPOId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two CPO identifications for equality.
        /// </summary>
        /// <param name="Object">A CPO identification to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is CPO_Id CPOId &&
                   Equals(CPOId);

        #endregion

        #region Equals(CPOId)

        /// <summary>
        /// Compares two CPO identifications for equality.
        /// </summary>
        /// <param name="CPOId">A CPO identification to compare with.</param>
        public Boolean Equals(CPO_Id CPOId)

            => String.Equals(InternalId,
                             CPOId.InternalId,
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
