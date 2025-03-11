/*
 * Copyright (c) 2015-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

namespace cloud.charging.open.protocols.OCPI
{

    /// <summary>
    /// Extension methods for CaptureStatusCodes.
    /// </summary>
    public static class CaptureStatusCodeExtensions
    {

        /// <summary>
        /// Indicates whether this CaptureStatusCode is null or empty.
        /// </summary>
        /// <param name="CaptureStatusCode">A CaptureStatusCode.</param>
        public static Boolean IsNullOrEmpty(this Capture_StatusCode? CaptureStatusCode)
            => !CaptureStatusCode.HasValue || CaptureStatusCode.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this CaptureStatusCode is NOT null or empty.
        /// </summary>
        /// <param name="CaptureStatusCode">A CaptureStatusCode.</param>
        public static Boolean IsNotNullOrEmpty(this Capture_StatusCode? CaptureStatusCode)
            => CaptureStatusCode.HasValue && CaptureStatusCode.Value.IsNotNullOrEmpty;


        #region Matches(CaptureStatusCodes, Match, IgnoreCase = true)

        /// <summary>
        /// Checks whether the given enumeration of EVSE identifications matches the given text.
        /// </summary>
        /// <param name="CaptureStatusCodes">An enumeration of EVSE identifications.</param>
        /// <param name="Match">A text to match.</param>
        /// <param name="IgnoreCase">Whether to ignore the case of the text.</param>
        public static Boolean Matches(this IEnumerable<Capture_StatusCode>  CaptureStatusCodes,
                                      String                                Match,
                                      Boolean                               IgnoreCase  = true)

            => CaptureStatusCodes.Any(captureStatusCode => IgnoreCase
                                          ? captureStatusCode.ToString().Contains(Match, StringComparison.OrdinalIgnoreCase)
                                          : captureStatusCode.ToString().Contains(Match, StringComparison.Ordinal));

        #endregion


    }


    /// <summary>
    /// This enumeration describes the status of the payment capture process following a transaction at an EV charging station.
    /// It helps determine the outcome of the transaction and facilitates accurate financial reporting and customer billing.
    /// </summary>
    public readonly struct Capture_StatusCode : IId<Capture_StatusCode>
    {

        #region Data

        /// <summary>
        /// The official identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this CaptureStatusCode is null or empty.
        /// </summary>
        public Boolean  IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this CaptureStatusCode is NOT null or empty.
        /// </summary>
        public Boolean  IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the CaptureStatusCode.
        /// </summary>
        public UInt64   Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new CaptureStatusCode based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a CaptureStatusCode.</param>
        private Capture_StatusCode(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as a CaptureStatusCode.
        /// </summary>
        /// <param name="Text">A text representation of a CaptureStatusCode.</param>
        public static Capture_StatusCode Parse(String Text)
        {

            if (TryParse(Text, out var captureStatusCode))
                return captureStatusCode;

            throw new ArgumentException($"Invalid text representation of a CaptureStatusCode: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a CaptureStatusCode.
        /// </summary>
        /// <param name="Text">A text representation of a CaptureStatusCode.</param>
        public static Capture_StatusCode? TryParse(String Text)
        {

            if (TryParse(Text, out var captureStatusCode))
                return captureStatusCode;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out CaptureStatusCode)

        /// <summary>
        /// Try to parse the given text as a CaptureStatusCode.
        /// </summary>
        /// <param name="Text">A text representation of a CaptureStatusCode.</param>
        /// <param name="CaptureStatusCode">The parsed CaptureStatusCode.</param>
        public static Boolean TryParse(String Text, out Capture_StatusCode CaptureStatusCode)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    CaptureStatusCode = new Capture_StatusCode(Text);
                    return true;
                }
                catch
                { }
            }

            CaptureStatusCode = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this CaptureStatusCode.
        /// </summary>
        public Capture_StatusCode Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// Indicates that the payment capture was completed successfully without any issues.
        /// Funds were secured and will be settled according to the payment processor’s timeline.
        /// This status confirms that all checks (e.g., fraud, card validation) passed and the transaction was approved.
        /// </summary>
        public static Capture_StatusCode  SUCCESS            { get; }
            = new ("SUCCESS");

        /// <summary>
        /// Used when only part of the transaction amount was approved or when certain conditions of the payment were altered during processing.
        /// This might occur in scenarios where the available balance was insufficient for the full requested amount,
        /// or specific transaction limits were enforced by the card issuer.
        /// </summary>
        public static Capture_StatusCode  PARTIAL_SUCCESS    { get; }
            = new ("PARTIAL_SUCCESS");

        /// <summary>
        /// Indicates that the payment capture attempt was unsuccessful.
        /// This failure can be due to various reasons such as insufficient funds, card expiration,
        /// network issues, or refusal by the card issuer.
        /// </summary>
        public static Capture_StatusCode  FAILED             { get; }
            = new ("FAILED");

        #endregion


        #region Operator overloading

        #region Operator == (CaptureStatusCode1, CaptureStatusCode2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CaptureStatusCode1">A CaptureStatusCode.</param>
        /// <param name="CaptureStatusCode2">Another CaptureStatusCode.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Capture_StatusCode CaptureStatusCode1,
                                           Capture_StatusCode CaptureStatusCode2)

            => CaptureStatusCode1.Equals(CaptureStatusCode2);

        #endregion

        #region Operator != (CaptureStatusCode1, CaptureStatusCode2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CaptureStatusCode1">A CaptureStatusCode.</param>
        /// <param name="CaptureStatusCode2">Another CaptureStatusCode.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Capture_StatusCode CaptureStatusCode1,
                                           Capture_StatusCode CaptureStatusCode2)

            => !CaptureStatusCode1.Equals(CaptureStatusCode2);

        #endregion

        #region Operator <  (CaptureStatusCode1, CaptureStatusCode2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CaptureStatusCode1">A CaptureStatusCode.</param>
        /// <param name="CaptureStatusCode2">Another CaptureStatusCode.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Capture_StatusCode CaptureStatusCode1,
                                          Capture_StatusCode CaptureStatusCode2)

            => CaptureStatusCode1.CompareTo(CaptureStatusCode2) < 0;

        #endregion

        #region Operator <= (CaptureStatusCode1, CaptureStatusCode2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CaptureStatusCode1">A CaptureStatusCode.</param>
        /// <param name="CaptureStatusCode2">Another CaptureStatusCode.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Capture_StatusCode CaptureStatusCode1,
                                           Capture_StatusCode CaptureStatusCode2)

            => CaptureStatusCode1.CompareTo(CaptureStatusCode2) <= 0;

        #endregion

        #region Operator >  (CaptureStatusCode1, CaptureStatusCode2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CaptureStatusCode1">A CaptureStatusCode.</param>
        /// <param name="CaptureStatusCode2">Another CaptureStatusCode.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Capture_StatusCode CaptureStatusCode1,
                                          Capture_StatusCode CaptureStatusCode2)

            => CaptureStatusCode1.CompareTo(CaptureStatusCode2) > 0;

        #endregion

        #region Operator >= (CaptureStatusCode1, CaptureStatusCode2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CaptureStatusCode1">A CaptureStatusCode.</param>
        /// <param name="CaptureStatusCode2">Another CaptureStatusCode.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Capture_StatusCode CaptureStatusCode1,
                                           Capture_StatusCode CaptureStatusCode2)

            => CaptureStatusCode1.CompareTo(CaptureStatusCode2) >= 0;

        #endregion

        #endregion

        #region IComparable<CaptureStatusCode> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two CaptureStatusCodes.
        /// </summary>
        /// <param name="Object">A CaptureStatusCode to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Capture_StatusCode captureStatusCode
                   ? CompareTo(captureStatusCode)
                   : throw new ArgumentException("The given object is not a CaptureStatusCode!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(CaptureStatusCode)

        /// <summary>
        /// Compares two CaptureStatusCodes.
        /// </summary>
        /// <param name="CaptureStatusCode">A CaptureStatusCode to compare with.</param>
        public Int32 CompareTo(Capture_StatusCode CaptureStatusCode)

            => String.Compare(InternalId,
                              CaptureStatusCode.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<CaptureStatusCode> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two CaptureStatusCodes for equality.
        /// </summary>
        /// <param name="Object">A CaptureStatusCode to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Capture_StatusCode captureStatusCode &&
                   Equals(captureStatusCode);

        #endregion

        #region Equals(CaptureStatusCode)

        /// <summary>
        /// Compares two CaptureStatusCodes for equality.
        /// </summary>
        /// <param name="CaptureStatusCode">A CaptureStatusCode to compare with.</param>
        public Boolean Equals(Capture_StatusCode CaptureStatusCode)

            => String.Equals(InternalId,
                             CaptureStatusCode.InternalId,
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
