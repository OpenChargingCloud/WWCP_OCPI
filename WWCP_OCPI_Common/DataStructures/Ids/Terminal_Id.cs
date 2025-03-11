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
    /// Extension methods for terminal identifications.
    /// </summary>
    public static class TerminalIdExtensions
    {

        /// <summary>
        /// Indicates whether this terminal identification is null or empty.
        /// </summary>
        /// <param name="TerminalId">A terminal identification.</param>
        public static Boolean IsNullOrEmpty(this Terminal_Id? TerminalId)
            => !TerminalId.HasValue || TerminalId.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this terminal identification is NOT null or empty.
        /// </summary>
        /// <param name="TerminalId">A terminal identification.</param>
        public static Boolean IsNotNullOrEmpty(this Terminal_Id? TerminalId)
            => TerminalId.HasValue && TerminalId.Value.IsNotNullOrEmpty;


        #region Matches(TerminalIds, Match, IgnoreCase = true)

        /// <summary>
        /// Checks whether the given enumeration of EVSE identifications matches the given text.
        /// </summary>
        /// <param name="TerminalIds">An enumeration of EVSE identifications.</param>
        /// <param name="Match">A text to match.</param>
        /// <param name="IgnoreCase">Whether to ignore the case of the text.</param>
        public static Boolean Matches(this IEnumerable<Terminal_Id>  TerminalIds,
                                      String                     Match,
                                      Boolean                    IgnoreCase  = true)

            => TerminalIds.Any(terminalId => IgnoreCase
                                          ? terminalId.ToString().Contains(Match, StringComparison.OrdinalIgnoreCase)
                                          : terminalId.ToString().Contains(Match, StringComparison.Ordinal));

        #endregion


    }


    /// <summary>
    /// The unique official identification of an EVSE.
    /// CiString(36)
    /// </summary>
    public readonly struct Terminal_Id : IId<Terminal_Id>
    {

        #region Data

        /// <summary>
        /// The official identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this terminal identification is null or empty.
        /// </summary>
        public Boolean  IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this terminal identification is NOT null or empty.
        /// </summary>
        public Boolean  IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the terminal identification.
        /// </summary>
        public UInt64   Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new terminal identification based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a terminal identification.</param>
        private Terminal_Id(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as a terminal identification.
        /// </summary>
        /// <param name="Text">A text representation of a terminal identification.</param>
        public static Terminal_Id Parse(String Text)
        {

            if (TryParse(Text, out var terminalId))
                return terminalId;

            throw new ArgumentException($"Invalid text representation of a terminal identification: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a terminal identification.
        /// </summary>
        /// <param name="Text">A text representation of a terminal identification.</param>
        public static Terminal_Id? TryParse(String Text)
        {

            if (TryParse(Text, out var terminalId))
                return terminalId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out TerminalId)

        /// <summary>
        /// Try to parse the given text as a terminal identification.
        /// </summary>
        /// <param name="Text">A text representation of a terminal identification.</param>
        /// <param name="TerminalId">The parsed terminal identification.</param>
        public static Boolean TryParse(String Text, out Terminal_Id TerminalId)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    TerminalId = new Terminal_Id(Text);
                    return true;
                }
                catch
                { }
            }

            TerminalId = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this terminal identification.
        /// </summary>
        public Terminal_Id Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Operator overloading

        #region Operator == (TerminalId1, TerminalId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TerminalId1">A terminal identification.</param>
        /// <param name="TerminalId2">Another terminal identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Terminal_Id TerminalId1,
                                           Terminal_Id TerminalId2)

            => TerminalId1.Equals(TerminalId2);

        #endregion

        #region Operator != (TerminalId1, TerminalId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TerminalId1">A terminal identification.</param>
        /// <param name="TerminalId2">Another terminal identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Terminal_Id TerminalId1,
                                           Terminal_Id TerminalId2)

            => !TerminalId1.Equals(TerminalId2);

        #endregion

        #region Operator <  (TerminalId1, TerminalId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TerminalId1">A terminal identification.</param>
        /// <param name="TerminalId2">Another terminal identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Terminal_Id TerminalId1,
                                          Terminal_Id TerminalId2)

            => TerminalId1.CompareTo(TerminalId2) < 0;

        #endregion

        #region Operator <= (TerminalId1, TerminalId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TerminalId1">A terminal identification.</param>
        /// <param name="TerminalId2">Another terminal identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Terminal_Id TerminalId1,
                                           Terminal_Id TerminalId2)

            => TerminalId1.CompareTo(TerminalId2) <= 0;

        #endregion

        #region Operator >  (TerminalId1, TerminalId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TerminalId1">A terminal identification.</param>
        /// <param name="TerminalId2">Another terminal identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Terminal_Id TerminalId1,
                                          Terminal_Id TerminalId2)

            => TerminalId1.CompareTo(TerminalId2) > 0;

        #endregion

        #region Operator >= (TerminalId1, TerminalId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TerminalId1">A terminal identification.</param>
        /// <param name="TerminalId2">Another terminal identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Terminal_Id TerminalId1,
                                           Terminal_Id TerminalId2)

            => TerminalId1.CompareTo(TerminalId2) >= 0;

        #endregion

        #endregion

        #region IComparable<TerminalId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two terminal identifications.
        /// </summary>
        /// <param name="Object">A terminal identification to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Terminal_Id TerminalId
                   ? CompareTo(TerminalId)
                   : throw new ArgumentException("The given object is not a terminal identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(TerminalId)

        /// <summary>
        /// Compares two terminal identifications.
        /// </summary>
        /// <param name="TerminalId">A terminal identification to compare with.</param>
        public Int32 CompareTo(Terminal_Id TerminalId)

            => String.Compare(InternalId,
                              TerminalId.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<TerminalId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two terminal identifications for equality.
        /// </summary>
        /// <param name="Object">A terminal identification to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Terminal_Id TerminalId &&
                   Equals(TerminalId);

        #endregion

        #region Equals(TerminalId)

        /// <summary>
        /// Compares two terminal identifications for equality.
        /// </summary>
        /// <param name="TerminalId">A terminal identification to compare with.</param>
        public Boolean Equals(Terminal_Id TerminalId)

            => String.Equals(InternalId,
                             TerminalId.InternalId,
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
