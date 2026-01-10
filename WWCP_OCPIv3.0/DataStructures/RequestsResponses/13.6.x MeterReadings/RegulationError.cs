/*
 * Copyright (c) 2015-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPI <https://github.com/OpenChargingCloud/WWCP_OCPI>
 *
 * Licensed under the Affero GPL license, Version 3.0 (the "License");
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

namespace cloud.charging.open.protocols.OCPIv3_0
{

    /// <summary>
    /// Extension methods for regulation errors.
    /// </summary>
    public static class RegulationErrorExtensions
    {

        /// <summary>
        /// Indicates whether this regulation error is null or empty.
        /// </summary>
        /// <param name="RegulationError">A regulation error.</param>
        public static Boolean IsNullOrEmpty(this RegulationError? RegulationError)
            => !RegulationError.HasValue || RegulationError.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this regulation error is NOT null or empty.
        /// </summary>
        /// <param name="RegulationError">A regulation error.</param>
        public static Boolean IsNotNullOrEmpty(this RegulationError? RegulationError)
            => RegulationError.HasValue && RegulationError.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The unique identification of a regulation error.
    /// </summary>
    public readonly struct RegulationError : IId<RegulationError>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this regulation error is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this regulation error is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the regulation error.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new regulation error based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a regulation error.</param>
        private RegulationError(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as a regulation error.
        /// </summary>
        /// <param name="Text">A text representation of a regulation error.</param>
        public static RegulationError Parse(String Text)
        {

            if (TryParse(Text, out var regulationError))
                return regulationError;

            throw new ArgumentException($"Invalid text representation of a regulation error: '" + Text + "'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a regulation error.
        /// </summary>
        /// <param name="Text">A text representation of a regulation error.</param>
        public static RegulationError? TryParse(String Text)
        {

            if (TryParse(Text, out var regulationError))
                return regulationError;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out RegulationError)

        /// <summary>
        /// Try to parse the given text as a regulation error.
        /// </summary>
        /// <param name="Text">A text representation of a regulation error.</param>
        /// <param name="RegulationError">The parsed regulation error.</param>
        public static Boolean TryParse(String Text, out RegulationError RegulationError)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    RegulationError = new RegulationError(Text);
                    return true;
                }
                catch
                { }
            }

            RegulationError = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this regulation error.
        /// </summary>
        public RegulationError Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// The Charging Profile request was rejected by the CPO because requests are sent more often than the CPO allows.
        /// </summary>
        public static RegulationError  TOO_OFTEN           { get; }
            = new ("TOO_OFTEN");

        /// <summary>
        /// The Session in the request command is not known by this CPO.
        /// </summary>
        public static RegulationError  UNKNOWN_SESSION     { get; }
            = new ("UNKNOWN_SESSION");

        /// <summary>
        /// An EVSE mentioned in the request is not known by this CPO.
        /// </summary>
        public static RegulationError  UNKNOWN_EVSE        { get; }
            = new ("UNKNOWN_EVSE");

        #endregion


        #region Operator overloading

        #region Operator == (RegulationError1, RegulationError2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RegulationError1">A regulation error.</param>
        /// <param name="RegulationError2">Another regulation error.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (RegulationError RegulationError1,
                                           RegulationError RegulationError2)

            => RegulationError1.Equals(RegulationError2);

        #endregion

        #region Operator != (RegulationError1, RegulationError2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RegulationError1">A regulation error.</param>
        /// <param name="RegulationError2">Another regulation error.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (RegulationError RegulationError1,
                                           RegulationError RegulationError2)

            => !RegulationError1.Equals(RegulationError2);

        #endregion

        #region Operator <  (RegulationError1, RegulationError2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RegulationError1">A regulation error.</param>
        /// <param name="RegulationError2">Another regulation error.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (RegulationError RegulationError1,
                                          RegulationError RegulationError2)

            => RegulationError1.CompareTo(RegulationError2) < 0;

        #endregion

        #region Operator <= (RegulationError1, RegulationError2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RegulationError1">A regulation error.</param>
        /// <param name="RegulationError2">Another regulation error.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (RegulationError RegulationError1,
                                           RegulationError RegulationError2)

            => RegulationError1.CompareTo(RegulationError2) <= 0;

        #endregion

        #region Operator >  (RegulationError1, RegulationError2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RegulationError1">A regulation error.</param>
        /// <param name="RegulationError2">Another regulation error.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (RegulationError RegulationError1,
                                          RegulationError RegulationError2)

            => RegulationError1.CompareTo(RegulationError2) > 0;

        #endregion

        #region Operator >= (RegulationError1, RegulationError2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RegulationError1">A regulation error.</param>
        /// <param name="RegulationError2">Another regulation error.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (RegulationError RegulationError1,
                                           RegulationError RegulationError2)

            => RegulationError1.CompareTo(RegulationError2) >= 0;

        #endregion

        #endregion

        #region IComparable<RegulationError> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two regulation errors.
        /// </summary>
        /// <param name="Object">A regulation error to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is RegulationError regulationError
                   ? CompareTo(regulationError)
                   : throw new ArgumentException("The given object is not a regulation error!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(RegulationError)

        /// <summary>
        /// Compares two regulation errors.
        /// </summary>
        /// <param name="RegulationError">A regulation error to compare with.</param>
        public Int32 CompareTo(RegulationError RegulationError)

            => String.Compare(InternalId,
                              RegulationError.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<RegulationError> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two regulation errors for equality.
        /// </summary>
        /// <param name="Object">A regulation error to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is RegulationError regulationError &&
                   Equals(regulationError);

        #endregion

        #region Equals(RegulationError)

        /// <summary>
        /// Compares two regulation errors for equality.
        /// </summary>
        /// <param name="RegulationError">A regulation error to compare with.</param>
        public Boolean Equals(RegulationError RegulationError)

            => String.Equals(InternalId,
                             RegulationError.InternalId,
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
