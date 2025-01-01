/*
 * Copyright (c) 2015-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

namespace cloud.charging.open.protocols.OCPIv3_0
{


    /// <summary>
    /// Extension methods for tariff audiences.
    /// </summary>
    public static class TariffAudienceExtensions
    {

        /// <summary>
        /// Indicates whether this tariff audience is null or empty.
        /// </summary>
        /// <param name="TariffAudience">A tariff audience.</param>
        public static Boolean IsNullOrEmpty(this TariffAudience? TariffAudience)
            => !TariffAudience.HasValue || TariffAudience.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this tariff audience is NOT null or empty.
        /// </summary>
        /// <param name="TariffAudience">A tariff audience.</param>
        public static Boolean IsNotNullOrEmpty(this TariffAudience? TariffAudience)
            => TariffAudience.HasValue && TariffAudience.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The unique identification of a tariff audience.
    /// </summary>
    public readonly struct TariffAudience : IId<TariffAudience>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this tariff audience is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this tariff audience is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the tariff audience.
        /// </summary>
        public UInt64 Length
            => (UInt64)InternalId.Length;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new tariff audience based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a tariff audience.</param>
        private TariffAudience(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as a tariff audience.
        /// </summary>
        /// <param name="Text">A text representation of a tariff audience.</param>
        public static TariffAudience Parse(String Text)
        {

            if (TryParse(Text, out var tariffAudience))
                return tariffAudience;

            throw new ArgumentException($"Invalid text representation of a tariff audience: '" + Text + "'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a tariff audience.
        /// </summary>
        /// <param name="Text">A text representation of a tariff audience.</param>
        public static TariffAudience? TryParse(String Text)
        {

            if (TryParse(Text, out var tariffAudience))
                return tariffAudience;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out TariffAudience)

        /// <summary>
        /// Try to parse the given text as a tariff audience.
        /// </summary>
        /// <param name="Text">A text representation of a tariff audience.</param>
        /// <param name="TariffAudience">The parsed tariff audience.</param>
        public static Boolean TryParse(String Text, out TariffAudience TariffAudience)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    TariffAudience = new TariffAudience(Text);
                    return true;
                }
                catch
                { }
            }

            TariffAudience = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this tariff audience.
        /// </summary>
        public TariffAudience Clone()

            => new(
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// Used to describe that a tariff is valid when ad-hoc payment is used at the charging station
        /// (for example: Debit or credit card payment terminal).
        /// </summary>
        public static TariffAudience  AD_HOC_PAYMENT    { get; }
            = new ("AD_HOC_PAYMENT");

        /// <summary>
        /// Used to describe that a tariff is valid when charging preference:
        /// CHEAP is set for the charging session.
        /// </summary>
        public static TariffAudience  PROFILE_CHEAP     { get; }
            = new ("PROFILE_CHEAP");

        /// <summary>
        /// Used to describe that a tariff is valid when charging preference:
        /// FAST is set for the charging session.
        /// </summary>
        public static TariffAudience  PROFILE_FAST      { get; }
            = new ("PROFILE_FAST");

        /// <summary>
        /// Used to describe that a tariff is valid when charging preference:
        /// GREEN is set for the charging session.
        /// </summary>
        public static TariffAudience  PROFILE_GREEN     { get; }
            = new("PROFILE_GREEN");

        /// <summary>
        /// Used to describe that a tariff is valid when using an RFID, without
        /// any charging preference, or when Charging Preference:
        /// REGULAR is set for the charging session.
        /// </summary>
        public static TariffAudience  REGULAR           { get; }
            = new ("REGULAR");

        #endregion


        #region Operator overloading

        #region Operator == (TariffAudience1, TariffAudience2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffAudience1">A tariff audience.</param>
        /// <param name="TariffAudience2">Another tariff audience.</param>
        /// <returns>true|false</returns>
        public static Boolean operator ==(TariffAudience TariffAudience1,
                                           TariffAudience TariffAudience2)

            => TariffAudience1.Equals(TariffAudience2);

        #endregion

        #region Operator != (TariffAudience1, TariffAudience2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffAudience1">A tariff audience.</param>
        /// <param name="TariffAudience2">Another tariff audience.</param>
        /// <returns>true|false</returns>
        public static Boolean operator !=(TariffAudience TariffAudience1,
                                           TariffAudience TariffAudience2)

            => !TariffAudience1.Equals(TariffAudience2);

        #endregion

        #region Operator <  (TariffAudience1, TariffAudience2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffAudience1">A tariff audience.</param>
        /// <param name="TariffAudience2">Another tariff audience.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <(TariffAudience TariffAudience1,
                                          TariffAudience TariffAudience2)

            => TariffAudience1.CompareTo(TariffAudience2) < 0;

        #endregion

        #region Operator <= (TariffAudience1, TariffAudience2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffAudience1">A tariff audience.</param>
        /// <param name="TariffAudience2">Another tariff audience.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <=(TariffAudience TariffAudience1,
                                           TariffAudience TariffAudience2)

            => TariffAudience1.CompareTo(TariffAudience2) <= 0;

        #endregion

        #region Operator >  (TariffAudience1, TariffAudience2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffAudience1">A tariff audience.</param>
        /// <param name="TariffAudience2">Another tariff audience.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >(TariffAudience TariffAudience1,
                                          TariffAudience TariffAudience2)

            => TariffAudience1.CompareTo(TariffAudience2) > 0;

        #endregion

        #region Operator >= (TariffAudience1, TariffAudience2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffAudience1">A tariff audience.</param>
        /// <param name="TariffAudience2">Another tariff audience.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >=(TariffAudience TariffAudience1,
                                           TariffAudience TariffAudience2)

            => TariffAudience1.CompareTo(TariffAudience2) >= 0;

        #endregion

        #endregion

        #region IComparable<TariffAudience> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two tariff audiences.
        /// </summary>
        /// <param name="Object">A tariff audience to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is TariffAudience tariffAudience
                   ? CompareTo(tariffAudience)
                   : throw new ArgumentException("The given object is not a tariff audience!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(TariffAudience)

        /// <summary>
        /// Compares two tariff audiences.
        /// </summary>
        /// <param name="TariffAudience">A tariff audience to compare with.</param>
        public Int32 CompareTo(TariffAudience TariffAudience)

            => String.Compare(InternalId,
                              TariffAudience.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<TariffAudience> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two tariff audiences for equality.
        /// </summary>
        /// <param name="Object">A tariff audience to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is TariffAudience tariffAudience &&
                   Equals(tariffAudience);

        #endregion

        #region Equals(TariffAudience)

        /// <summary>
        /// Compares two tariff audiences for equality.
        /// </summary>
        /// <param name="TariffAudience">A tariff audience to compare with.</param>
        public Boolean Equals(TariffAudience TariffAudience)

            => String.Equals(InternalId,
                             TariffAudience.InternalId,
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
