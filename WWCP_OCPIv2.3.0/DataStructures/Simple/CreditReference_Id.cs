/*
 * Copyright (c) 2015-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPI <https://github.com/OpenChargingCloud/WWCP_OCPI>
 *
 * Licensed under the Apache License, CreditReference 2.0 (the "License");
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

namespace cloud.charging.open.protocols.OCPIv2_3_0
{

    /// <summary>
    /// Extension methods for credit reference identifications.
    /// </summary>
    public static class CreditReferenceIdExtensions
    {

        /// <summary>
        /// Indicates whether this credit reference identification is null or empty.
        /// </summary>
        /// <param name="CreditReferenceId">A credit reference identification.</param>
        public static Boolean IsNullOrEmpty(this CreditReference_Id? CreditReferenceId)
            => !CreditReferenceId.HasValue || CreditReferenceId.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this credit reference identification is NOT null or empty.
        /// </summary>
        /// <param name="CreditReferenceId">A credit reference identification.</param>
        public static Boolean IsNotNullOrEmpty(this CreditReference_Id? CreditReferenceId)
            => CreditReferenceId.HasValue && CreditReferenceId.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The unique identification of a credit reference.
    /// CiString(39)
    /// </summary>
    public readonly struct CreditReference_Id : IId<CreditReference_Id>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this credit reference identification is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this credit reference identification is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the credit reference identification.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new credit reference identification based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a credit reference identification.</param>
        private CreditReference_Id(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as a credit reference identification.
        /// </summary>
        /// <param name="Text">A text representation of a credit reference identification.</param>
        public static CreditReference_Id Parse(String Text)
        {

            if (TryParse(Text, out var creditReferenceId))
                return creditReferenceId;

            throw new ArgumentException($"Invalid text representation of a credit reference identification: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a credit reference identification.
        /// </summary>
        /// <param name="Text">A text representation of a credit reference identification.</param>
        public static CreditReference_Id? TryParse(String Text)
        {

            if (TryParse(Text, out var creditReferenceId))
                return creditReferenceId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out CreditReferenceId)

        /// <summary>
        /// Try to parse the given text as a credit reference identification.
        /// </summary>
        /// <param name="Text">A text representation of a credit reference identification.</param>
        /// <param name="CreditReferenceId">The parsed credit reference identification.</param>
        public static Boolean TryParse(String Text, out CreditReference_Id CreditReferenceId)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    CreditReferenceId = new CreditReference_Id(Text);
                    return true;
                }
                catch
                { }
            }

            CreditReferenceId = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this credit reference identification.
        /// </summary>
        public CreditReference_Id Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Operator overloading

        #region Operator == (CreditReferenceId1, CreditReferenceId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CreditReferenceId1">A credit reference identification.</param>
        /// <param name="CreditReferenceId2">Another credit reference identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (CreditReference_Id CreditReferenceId1,
                                           CreditReference_Id CreditReferenceId2)

            => CreditReferenceId1.Equals(CreditReferenceId2);

        #endregion

        #region Operator != (CreditReferenceId1, CreditReferenceId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CreditReferenceId1">A credit reference identification.</param>
        /// <param name="CreditReferenceId2">Another credit reference identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (CreditReference_Id CreditReferenceId1,
                                           CreditReference_Id CreditReferenceId2)

            => !CreditReferenceId1.Equals(CreditReferenceId2);

        #endregion

        #region Operator <  (CreditReferenceId1, CreditReferenceId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CreditReferenceId1">A credit reference identification.</param>
        /// <param name="CreditReferenceId2">Another credit reference identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (CreditReference_Id CreditReferenceId1,
                                          CreditReference_Id CreditReferenceId2)

            => CreditReferenceId1.CompareTo(CreditReferenceId2) < 0;

        #endregion

        #region Operator <= (CreditReferenceId1, CreditReferenceId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CreditReferenceId1">A credit reference identification.</param>
        /// <param name="CreditReferenceId2">Another credit reference identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (CreditReference_Id CreditReferenceId1,
                                           CreditReference_Id CreditReferenceId2)

            => CreditReferenceId1.CompareTo(CreditReferenceId2) <= 0;

        #endregion

        #region Operator >  (CreditReferenceId1, CreditReferenceId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CreditReferenceId1">A credit reference identification.</param>
        /// <param name="CreditReferenceId2">Another credit reference identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (CreditReference_Id CreditReferenceId1,
                                          CreditReference_Id CreditReferenceId2)

            => CreditReferenceId1.CompareTo(CreditReferenceId2) > 0;

        #endregion

        #region Operator >= (CreditReferenceId1, CreditReferenceId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CreditReferenceId1">A credit reference identification.</param>
        /// <param name="CreditReferenceId2">Another credit reference identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (CreditReference_Id CreditReferenceId1,
                                           CreditReference_Id CreditReferenceId2)

            => CreditReferenceId1.CompareTo(CreditReferenceId2) >= 0;

        #endregion

        #endregion

        #region IComparable<CreditReferenceId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two credit reference identifications.
        /// </summary>
        /// <param name="Object">A credit reference identification to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is CreditReference_Id creditReferenceId
                   ? CompareTo(creditReferenceId)
                   : throw new ArgumentException("The given object is not a credit reference identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(CreditReferenceId)

        /// <summary>
        /// Compares two credit reference identifications.
        /// </summary>
        /// <param name="CreditReferenceId">A credit reference identification to compare with.</param>
        public Int32 CompareTo(CreditReference_Id CreditReferenceId)

            => String.Compare(InternalId,
                              CreditReferenceId.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<CreditReferenceId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two credit reference identifications for equality.
        /// </summary>
        /// <param name="Object">A credit reference identification to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is CreditReference_Id creditReferenceId &&
                   Equals(creditReferenceId);

        #endregion

        #region Equals(CreditReferenceId)

        /// <summary>
        /// Compares two credit reference identifications for equality.
        /// </summary>
        /// <param name="CreditReferenceId">A credit reference identification to compare with.</param>
        public Boolean Equals(CreditReference_Id CreditReferenceId)

            => String.Equals(InternalId,
                             CreditReferenceId.InternalId,
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
