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
    /// Extension methods for slopes.
    /// </summary>
    public static class SlopeExtensions
    {

        /// <summary>
        /// Indicates whether this slope is null or empty.
        /// </summary>
        /// <param name="Slope">A slope.</param>
        public static Boolean IsNullOrEmpty(this Slope? Slope)
            => !Slope.HasValue || Slope.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this slope is NOT null or empty.
        /// </summary>
        /// <param name="Slope">A slope.</param>
        public static Boolean IsNotNullOrEmpty(this Slope? Slope)
            => Slope.HasValue && Slope.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A slope: the slope of a parking bay (OCPI Accessibility Extension 1.0.0).
    /// </summary>
    public readonly struct Slope : IId<Slope>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this slope is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this slope is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the slope.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new slope based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a slope.</param>
        private Slope(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as a slope.
        /// </summary>
        /// <param name="Text">A text representation of a slope.</param>
        public static Slope Parse(String Text)
        {

            ArgumentNullException.ThrowIfNull(Text);

            if (TryParse(Text, out var slope))
                return slope;

            throw new ArgumentException($"Invalid text representation of a slope: '" + Text + "'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a slope.
        /// </summary>
        /// <param name="Text">A text representation of a slope.</param>
        public static Slope? TryParse(String Text)
        {

            if (TryParse(Text, out var slope))
                return slope;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out Slope)

        /// <summary>
        /// Try to parse the given text as a slope.
        /// </summary>
        /// <param name="Text">A text representation of a slope.</param>
        /// <param name="Slope">The parsed slope.</param>
        public static Boolean TryParse(String Text, out Slope Slope)
        {

            Text = Text?.Trim() ?? "";

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    Slope = new Slope(Text);
                    return true;
                }
                catch
                { }
            }

            Slope = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this slope.
        /// </summary>
        public Slope Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// Level surface.
        /// </summary>
        public static Slope  FLAT              { get; }
            = new ("FLAT");

        /// <summary>
        /// Moderately sloped surface.
        /// </summary>
        public static Slope  SLOPED            { get; }
            = new ("SLOPED");

        /// <summary>
        /// Steeply sloped surface.
        /// </summary>
        public static Slope  STEEPLY_SLOPED    { get; }
            = new ("STEEPLY_SLOPED");

        #endregion


        #region Operator overloading

        #region Operator == (Slope1, Slope2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Slope1">A slope.</param>
        /// <param name="Slope2">Another slope.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Slope Slope1,
                                           Slope Slope2)

            => Slope1.Equals(Slope2);

        #endregion

        #region Operator != (Slope1, Slope2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Slope1">A slope.</param>
        /// <param name="Slope2">Another slope.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Slope Slope1,
                                           Slope Slope2)

            => !Slope1.Equals(Slope2);

        #endregion

        #region Operator <  (Slope1, Slope2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Slope1">A slope.</param>
        /// <param name="Slope2">Another slope.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Slope Slope1,
                                          Slope Slope2)

            => Slope1.CompareTo(Slope2) < 0;

        #endregion

        #region Operator <= (Slope1, Slope2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Slope1">A slope.</param>
        /// <param name="Slope2">Another slope.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Slope Slope1,
                                           Slope Slope2)

            => Slope1.CompareTo(Slope2) <= 0;

        #endregion

        #region Operator >  (Slope1, Slope2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Slope1">A slope.</param>
        /// <param name="Slope2">Another slope.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Slope Slope1,
                                          Slope Slope2)

            => Slope1.CompareTo(Slope2) > 0;

        #endregion

        #region Operator >= (Slope1, Slope2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Slope1">A slope.</param>
        /// <param name="Slope2">Another slope.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Slope Slope1,
                                           Slope Slope2)

            => Slope1.CompareTo(Slope2) >= 0;

        #endregion

        #endregion

        #region IComparable<Slope> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two slopes.
        /// </summary>
        /// <param name="Object">A slope to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Slope slope
                   ? CompareTo(slope)
                   : throw new ArgumentException("The given object is not a slope!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(Slope)

        /// <summary>
        /// Compares two slopes.
        /// </summary>
        /// <param name="Slope">A slope to compare with.</param>
        public Int32 CompareTo(Slope Slope)

            => String.Compare(InternalId,
                              Slope.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<Slope> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two slopes for equality.
        /// </summary>
        /// <param name="Object">A slope to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Slope slope &&
                   Equals(slope);

        #endregion

        #region Equals(Slope)

        /// <summary>
        /// Compares two slopes for equality.
        /// </summary>
        /// <param name="Slope">A slope to compare with.</param>
        public Boolean Equals(Slope Slope)

            => String.Equals(InternalId,
                             Slope.InternalId,
                             StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()

            => InternalId?.ToUpper().GetHashCode() ?? 0;

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
