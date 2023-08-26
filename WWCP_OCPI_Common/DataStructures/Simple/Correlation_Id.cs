/*
 * Copyright (c) 2015-2023 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPI <https://github.com/OpenChargingCloud/WWCP_OCPI>
 *
 * Licensed under the Apache License, Correlation 2.0 (the "License");
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
    /// Extension methods for correlation identifications.
    /// </summary>
    public static class CorrelationIdExtensions
    {

        /// <summary>
        /// Indicates whether this correlation identification is null or empty.
        /// </summary>
        /// <param name="CorrelationId">A correlation identification.</param>
        public static Boolean IsNullOrEmpty(this Correlation_Id? CorrelationId)
            => !CorrelationId.HasValue || CorrelationId.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this correlation identification is NOT null or empty.
        /// </summary>
        /// <param name="CorrelationId">A correlation identification.</param>
        public static Boolean IsNotNullOrEmpty(this Correlation_Id? CorrelationId)
            => CorrelationId.HasValue && CorrelationId.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The unique identification of a request correlation.
    /// </summary>
    public readonly struct Correlation_Id : IId<Correlation_Id>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this correlation identification is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this correlation identification is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the correlation identification.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new correlation identification based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a correlation identification.</param>
        private Correlation_Id(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) NewRandom(Length = 30, IsLocal = false)

        /// <summary>
        /// Create a new random request correlation identification.
        /// </summary>
        /// <param name="Length">The expected length of the request correlation identification.</param>
        /// <param name="IsLocal">The request correlation identification was generated locally and not received via network.</param>
        public static Correlation_Id NewRandom(Byte      Length    = 30,
                                               Boolean?  IsLocal   = false)

            => new ((IsLocal == true ? "Local:" : "") +
                    RandomExtensions.RandomString(Length));

        #endregion

        #region (static) Parse    (Text)

        /// <summary>
        /// Parse the given text as a correlation identification.
        /// </summary>
        /// <param name="Text">A text representation of a correlation identification.</param>
        public static Correlation_Id Parse(String Text)
        {

            if (TryParse(Text, out var correlationId))
                return correlationId;

            throw new ArgumentException($"Invalid text representation of a correlation identification: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse (Text)

        /// <summary>
        /// Try to parse the given text as a correlation identification.
        /// </summary>
        /// <param name="Text">A text representation of a correlation identification.</param>
        public static Correlation_Id? TryParse(String Text)
        {

            if (TryParse(Text, out var correlationId))
                return correlationId;

            return null;

        }

        #endregion

        #region (static) TryParse (Text, out CorrelationId)

        /// <summary>
        /// Try to parse the given text as a correlation identification.
        /// </summary>
        /// <param name="Text">A text representation of a correlation identification.</param>
        /// <param name="CorrelationId">The parsed correlation identification.</param>
        public static Boolean TryParse(String Text, out Correlation_Id CorrelationId)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    CorrelationId = new Correlation_Id(Text);
                    return true;
                }
                catch
                { }
            }

            CorrelationId = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this correlation identification.
        /// </summary>
        public Correlation_Id Clone

            => new (
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Operator overloading

        #region Operator == (CorrelationId1, CorrelationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CorrelationId1">A correlation identification.</param>
        /// <param name="CorrelationId2">Another correlation identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Correlation_Id CorrelationId1,
                                           Correlation_Id CorrelationId2)

            => CorrelationId1.Equals(CorrelationId2);

        #endregion

        #region Operator != (CorrelationId1, CorrelationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CorrelationId1">A correlation identification.</param>
        /// <param name="CorrelationId2">Another correlation identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Correlation_Id CorrelationId1,
                                           Correlation_Id CorrelationId2)

            => !CorrelationId1.Equals(CorrelationId2);

        #endregion

        #region Operator <  (CorrelationId1, CorrelationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CorrelationId1">A correlation identification.</param>
        /// <param name="CorrelationId2">Another correlation identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Correlation_Id CorrelationId1,
                                          Correlation_Id CorrelationId2)

            => CorrelationId1.CompareTo(CorrelationId2) < 0;

        #endregion

        #region Operator <= (CorrelationId1, CorrelationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CorrelationId1">A correlation identification.</param>
        /// <param name="CorrelationId2">Another correlation identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Correlation_Id CorrelationId1,
                                           Correlation_Id CorrelationId2)

            => CorrelationId1.CompareTo(CorrelationId2) <= 0;

        #endregion

        #region Operator >  (CorrelationId1, CorrelationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CorrelationId1">A correlation identification.</param>
        /// <param name="CorrelationId2">Another correlation identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Correlation_Id CorrelationId1,
                                          Correlation_Id CorrelationId2)

            => CorrelationId1.CompareTo(CorrelationId2) > 0;

        #endregion

        #region Operator >= (CorrelationId1, CorrelationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CorrelationId1">A correlation identification.</param>
        /// <param name="CorrelationId2">Another correlation identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Correlation_Id CorrelationId1,
                                           Correlation_Id CorrelationId2)

            => CorrelationId1.CompareTo(CorrelationId2) >= 0;

        #endregion

        #endregion

        #region IComparable<CorrelationId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two correlation identifications.
        /// </summary>
        /// <param name="Object">A correlation identification to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Correlation_Id correlationId
                   ? CompareTo(correlationId)
                   : throw new ArgumentException("The given object is not a correlation identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(CorrelationId)

        /// <summary>
        /// Compares two correlation identifications.
        /// </summary>
        /// <param name="CorrelationId">A correlation identification to compare with.</param>
        public Int32 CompareTo(Correlation_Id CorrelationId)

            => String.Compare(InternalId,
                              CorrelationId.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<CorrelationId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two correlation identifications for equality.
        /// </summary>
        /// <param name="Object">A correlation identification to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Correlation_Id correlationId &&
                   Equals(correlationId);

        #endregion

        #region Equals(CorrelationId)

        /// <summary>
        /// Compares two correlation identifications for equality.
        /// </summary>
        /// <param name="CorrelationId">A correlation identification to compare with.</param>
        public Boolean Equals(Correlation_Id CorrelationId)

            => String.Equals(InternalId,
                             CorrelationId.InternalId,
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
