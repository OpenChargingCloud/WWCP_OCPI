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
    /// Extension methods for phases.
    /// </summary>
    public static class PhaseExtensions
    {

        /// <summary>
        /// Indicates whether this phase is null or empty.
        /// </summary>
        /// <param name="Phase">A phase.</param>
        public static Boolean IsNullOrEmpty(this Phase? Phase)
            => !Phase.HasValue || Phase.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this phase is NOT null or empty.
        /// </summary>
        /// <param name="Phase">A phase.</param>
        public static Boolean IsNotNullOrEmpty(this Phase? Phase)
            => Phase.HasValue && Phase.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// Indicates to which phase or phases of the power supply a meter reading applies.
    /// </summary>
    public readonly struct Phase : IId<Phase>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this phase is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this phase is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the phase.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new phase based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a phase.</param>
        private Phase(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as a phase.
        /// </summary>
        /// <param name="Text">A text representation of a phase.</param>
        public static Phase Parse(String Text)
        {

            if (TryParse(Text, out var phase))
                return phase;

            throw new ArgumentException($"Invalid text representation of a phase: '" + Text + "'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a phase.
        /// </summary>
        /// <param name="Text">A text representation of a phase.</param>
        public static Phase? TryParse(String Text)
        {

            if (TryParse(Text, out var phase))
                return phase;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out Phase)

        /// <summary>
        /// Try to parse the given text as a phase.
        /// </summary>
        /// <param name="Text">A text representation of a phase.</param>
        /// <param name="Phase">The parsed phase.</param>
        public static Boolean TryParse(String Text, out Phase Phase)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    Phase = new Phase(Text);
                    return true;
                }
                catch
                { }
            }

            Phase = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this phase.
        /// </summary>
        public Phase Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// Measured on L1
        /// </summary>
        public static Phase  L1       { get; }
            = new ("L1");

        /// <summary>
        /// Measured on L2
        /// </summary>
        public static Phase  L2       { get; }
            = new ("L2");

        /// <summary>
        /// Measured on L3
        /// </summary>
        public static Phase  L3       { get; }
            = new ("L3");

        /// <summary>
        /// Measured on Neutral
        /// </summary>
        public static Phase  N        { get; }
            = new ("N");

        /// <summary>
        /// Measured on L1 with respect to Neutral conductor
        /// </summary>
        public static Phase  L1_N     { get; }
            = new ("L1-N");

        /// <summary>
        /// Measured on L2 with respect to Neutral conductor
        /// </summary>
        public static Phase  L2_N     { get; }
            = new("L2-N");

        //// <summary>
        /// Measured on L3 with respect to Neutral conductor
        /// </summary>
        public static Phase  L3_N     { get; }
            = new("L3-N");

        /// <summary>
        /// Measured between L1 and L2
        /// </summary>
        public static Phase  L1_L2    { get; }
            = new ("L1-L2");

        /// <summary>
        /// Measured between L2 and L3
        /// </summary>
        public static Phase  L2_L3    { get; }
            = new ("L2-L3");

        /// <summary>
        /// Measured between L3 and L1
        /// </summary>
        public static Phase  L3_L1    { get; }
            = new ("L3-L1");

        #endregion


        #region Operator overloading

        #region Operator == (Phase1, Phase2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Phase1">A phase.</param>
        /// <param name="Phase2">Another phase.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Phase Phase1,
                                           Phase Phase2)

            => Phase1.Equals(Phase2);

        #endregion

        #region Operator != (Phase1, Phase2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Phase1">A phase.</param>
        /// <param name="Phase2">Another phase.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Phase Phase1,
                                           Phase Phase2)

            => !Phase1.Equals(Phase2);

        #endregion

        #region Operator <  (Phase1, Phase2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Phase1">A phase.</param>
        /// <param name="Phase2">Another phase.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Phase Phase1,
                                          Phase Phase2)

            => Phase1.CompareTo(Phase2) < 0;

        #endregion

        #region Operator <= (Phase1, Phase2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Phase1">A phase.</param>
        /// <param name="Phase2">Another phase.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Phase Phase1,
                                           Phase Phase2)

            => Phase1.CompareTo(Phase2) <= 0;

        #endregion

        #region Operator >  (Phase1, Phase2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Phase1">A phase.</param>
        /// <param name="Phase2">Another phase.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Phase Phase1,
                                          Phase Phase2)

            => Phase1.CompareTo(Phase2) > 0;

        #endregion

        #region Operator >= (Phase1, Phase2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Phase1">A phase.</param>
        /// <param name="Phase2">Another phase.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Phase Phase1,
                                           Phase Phase2)

            => Phase1.CompareTo(Phase2) >= 0;

        #endregion

        #endregion

        #region IComparable<Phase> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two phases.
        /// </summary>
        /// <param name="Object">A phase to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Phase phase
                   ? CompareTo(phase)
                   : throw new ArgumentException("The given object is not a phase!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(Phase)

        /// <summary>
        /// Compares two phases.
        /// </summary>
        /// <param name="Phase">A phase to compare with.</param>
        public Int32 CompareTo(Phase Phase)

            => String.Compare(InternalId,
                              Phase.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<Phase> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two phases for equality.
        /// </summary>
        /// <param name="Object">A phase to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Phase phase &&
                   Equals(phase);

        #endregion

        #region Equals(Phase)

        /// <summary>
        /// Compares two phases for equality.
        /// </summary>
        /// <param name="Phase">A phase to compare with.</param>
        public Boolean Equals(Phase Phase)

            => String.Equals(InternalId,
                             Phase.InternalId,
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
