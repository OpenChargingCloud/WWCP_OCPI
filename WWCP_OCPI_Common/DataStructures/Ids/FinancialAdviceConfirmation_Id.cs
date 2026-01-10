/*
 * Copyright (c) 2015-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPI <https://github.com/OpenChargingCloud/WWCP_OCPI>
 *
 * Licensed under the Apache License, FinancialAdviceConfirmation 2.0 (the "License");
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
    /// Extension methods for FinancialAdviceConfirmation identifications.
    /// </summary>
    public static class FinancialAdviceConfirmationIdExtensions
    {

        /// <summary>
        /// Indicates whether this FinancialAdviceConfirmation identification is null or empty.
        /// </summary>
        /// <param name="FinancialAdviceConfirmationId">A FinancialAdviceConfirmation identification.</param>
        public static Boolean IsNullOrEmpty(this FinancialAdviceConfirmation_Id? FinancialAdviceConfirmationId)
            => !FinancialAdviceConfirmationId.HasValue || FinancialAdviceConfirmationId.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this FinancialAdviceConfirmation identification is NOT null or empty.
        /// </summary>
        /// <param name="FinancialAdviceConfirmationId">A FinancialAdviceConfirmation identification.</param>
        public static Boolean IsNotNullOrEmpty(this FinancialAdviceConfirmation_Id? FinancialAdviceConfirmationId)
            => FinancialAdviceConfirmationId.HasValue && FinancialAdviceConfirmationId.Value.IsNotNullOrEmpty;


        #region Matches(FinancialAdviceConfirmationIds, Match, IgnoreCase = true)

        /// <summary>
        /// Checks whether the given enumeration of FinancialAdviceConfirmation identifications matches the given text.
        /// </summary>
        /// <param name="FinancialAdviceConfirmationIds">An enumeration of FinancialAdviceConfirmation identifications.</param>
        /// <param name="Match">A text to match.</param>
        /// <param name="IgnoreCase">Whether to ignore the case of the text.</param>
        public static Boolean Matches(this IEnumerable<FinancialAdviceConfirmation_Id>  FinancialAdviceConfirmationIds,
                                      String                         Match,
                                      Boolean                        IgnoreCase  = true)

            => FinancialAdviceConfirmationIds.Any(FinancialAdviceConfirmationId => IgnoreCase
                                                 ? FinancialAdviceConfirmationId.ToString().Contains(Match, StringComparison.OrdinalIgnoreCase)
                                                 : FinancialAdviceConfirmationId.ToString().Contains(Match, StringComparison.Ordinal));

        #endregion


    }


    /// <summary>
    /// The unique identification of a FinancialAdviceConfirmation.
    /// </summary>
    public readonly struct FinancialAdviceConfirmation_Id : IId<FinancialAdviceConfirmation_Id>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this FinancialAdviceConfirmation identification is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this FinancialAdviceConfirmation identification is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the FinancialAdviceConfirmation identification.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new FinancialAdviceConfirmation identification based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a FinancialAdviceConfirmation identification.</param>
        private FinancialAdviceConfirmation_Id(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) NewRandom(Length = 20)

        /// <summary>
        /// Create a new random FinancialAdviceConfirmation identification.
        /// </summary>
        /// <param name="Length">The expected length of the FinancialAdviceConfirmation identification.</param>
        public static FinancialAdviceConfirmation_Id NewRandom(Byte Length = 30)

            => new (RandomExtensions.RandomString(Length));

        #endregion

        #region (static) Parse    (Text)

        /// <summary>
        /// Parse the given text as a FinancialAdviceConfirmation identification.
        /// </summary>
        /// <param name="Text">A text representation of a FinancialAdviceConfirmation identification.</param>
        public static FinancialAdviceConfirmation_Id Parse(String Text)
        {

            if (TryParse(Text, out var financialAdviceConfirmationId))
                return financialAdviceConfirmationId;

            throw new ArgumentException($"Invalid text representation of a FinancialAdviceConfirmation identification: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse (Text)

        /// <summary>
        /// Try to parse the given text as a FinancialAdviceConfirmation identification.
        /// </summary>
        /// <param name="Text">A text representation of a FinancialAdviceConfirmation identification.</param>
        public static FinancialAdviceConfirmation_Id? TryParse(String Text)
        {

            if (TryParse(Text, out var financialAdviceConfirmationId))
                return financialAdviceConfirmationId;

            return null;

        }

        #endregion

        #region (static) TryParse (Text, out FinancialAdviceConfirmationId)

        /// <summary>
        /// Try to parse the given text as a FinancialAdviceConfirmation identification.
        /// </summary>
        /// <param name="Text">A text representation of a FinancialAdviceConfirmation identification.</param>
        /// <param name="FinancialAdviceConfirmationId">The parsed FinancialAdviceConfirmation identification.</param>
        public static Boolean TryParse(String Text, out FinancialAdviceConfirmation_Id FinancialAdviceConfirmationId)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    FinancialAdviceConfirmationId = new FinancialAdviceConfirmation_Id(Text);
                    return true;
                }
                catch
                { }
            }

            FinancialAdviceConfirmationId = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this FinancialAdviceConfirmation identification.
        /// </summary>
        public FinancialAdviceConfirmation_Id Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Operator overloading

        #region Operator == (FinancialAdviceConfirmationId1, FinancialAdviceConfirmationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="FinancialAdviceConfirmationId1">A FinancialAdviceConfirmation identification.</param>
        /// <param name="FinancialAdviceConfirmationId2">Another FinancialAdviceConfirmation identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (FinancialAdviceConfirmation_Id FinancialAdviceConfirmationId1,
                                           FinancialAdviceConfirmation_Id FinancialAdviceConfirmationId2)

            => FinancialAdviceConfirmationId1.Equals(FinancialAdviceConfirmationId2);

        #endregion

        #region Operator != (FinancialAdviceConfirmationId1, FinancialAdviceConfirmationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="FinancialAdviceConfirmationId1">A FinancialAdviceConfirmation identification.</param>
        /// <param name="FinancialAdviceConfirmationId2">Another FinancialAdviceConfirmation identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (FinancialAdviceConfirmation_Id FinancialAdviceConfirmationId1,
                                           FinancialAdviceConfirmation_Id FinancialAdviceConfirmationId2)

            => !FinancialAdviceConfirmationId1.Equals(FinancialAdviceConfirmationId2);

        #endregion

        #region Operator <  (FinancialAdviceConfirmationId1, FinancialAdviceConfirmationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="FinancialAdviceConfirmationId1">A FinancialAdviceConfirmation identification.</param>
        /// <param name="FinancialAdviceConfirmationId2">Another FinancialAdviceConfirmation identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (FinancialAdviceConfirmation_Id FinancialAdviceConfirmationId1,
                                          FinancialAdviceConfirmation_Id FinancialAdviceConfirmationId2)

            => FinancialAdviceConfirmationId1.CompareTo(FinancialAdviceConfirmationId2) < 0;

        #endregion

        #region Operator <= (FinancialAdviceConfirmationId1, FinancialAdviceConfirmationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="FinancialAdviceConfirmationId1">A FinancialAdviceConfirmation identification.</param>
        /// <param name="FinancialAdviceConfirmationId2">Another FinancialAdviceConfirmation identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (FinancialAdviceConfirmation_Id FinancialAdviceConfirmationId1,
                                           FinancialAdviceConfirmation_Id FinancialAdviceConfirmationId2)

            => FinancialAdviceConfirmationId1.CompareTo(FinancialAdviceConfirmationId2) <= 0;

        #endregion

        #region Operator >  (FinancialAdviceConfirmationId1, FinancialAdviceConfirmationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="FinancialAdviceConfirmationId1">A FinancialAdviceConfirmation identification.</param>
        /// <param name="FinancialAdviceConfirmationId2">Another FinancialAdviceConfirmation identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (FinancialAdviceConfirmation_Id FinancialAdviceConfirmationId1,
                                          FinancialAdviceConfirmation_Id FinancialAdviceConfirmationId2)

            => FinancialAdviceConfirmationId1.CompareTo(FinancialAdviceConfirmationId2) > 0;

        #endregion

        #region Operator >= (FinancialAdviceConfirmationId1, FinancialAdviceConfirmationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="FinancialAdviceConfirmationId1">A FinancialAdviceConfirmation identification.</param>
        /// <param name="FinancialAdviceConfirmationId2">Another FinancialAdviceConfirmation identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (FinancialAdviceConfirmation_Id FinancialAdviceConfirmationId1,
                                           FinancialAdviceConfirmation_Id FinancialAdviceConfirmationId2)

            => FinancialAdviceConfirmationId1.CompareTo(FinancialAdviceConfirmationId2) >= 0;

        #endregion

        #endregion

        #region IComparable<FinancialAdviceConfirmationId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two FinancialAdviceConfirmation identifications.
        /// </summary>
        /// <param name="Object">A FinancialAdviceConfirmation identification to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is FinancialAdviceConfirmation_Id financialAdviceConfirmationId
                   ? CompareTo(financialAdviceConfirmationId)
                   : throw new ArgumentException("The given object is not a FinancialAdviceConfirmation identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(FinancialAdviceConfirmationId)

        /// <summary>
        /// Compares two FinancialAdviceConfirmation identifications.
        /// </summary>
        /// <param name="FinancialAdviceConfirmationId">A FinancialAdviceConfirmation identification to compare with.</param>
        public Int32 CompareTo(FinancialAdviceConfirmation_Id FinancialAdviceConfirmationId)

            => String.Compare(InternalId,
                              FinancialAdviceConfirmationId.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<FinancialAdviceConfirmationId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two FinancialAdviceConfirmation identifications for equality.
        /// </summary>
        /// <param name="Object">A FinancialAdviceConfirmation identification to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is FinancialAdviceConfirmation_Id financialAdviceConfirmationId &&
                   Equals(financialAdviceConfirmationId);

        #endregion

        #region Equals(FinancialAdviceConfirmationId)

        /// <summary>
        /// Compares two FinancialAdviceConfirmation identifications for equality.
        /// </summary>
        /// <param name="FinancialAdviceConfirmationId">A FinancialAdviceConfirmation identification to compare with.</param>
        public Boolean Equals(FinancialAdviceConfirmation_Id FinancialAdviceConfirmationId)

            => String.Equals(InternalId,
                             FinancialAdviceConfirmationId.InternalId,
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
