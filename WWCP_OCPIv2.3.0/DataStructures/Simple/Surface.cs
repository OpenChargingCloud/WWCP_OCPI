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

namespace cloud.charging.open.protocols.OCPIv2_3_0
{

    /// <summary>
    /// Extension methods for surfaces.
    /// </summary>
    public static class SurfaceExtensions
    {

        /// <summary>
        /// Indicates whether this surface is null or empty.
        /// </summary>
        /// <param name="Surface">A surface.</param>
        public static Boolean IsNullOrEmpty(this Surface? Surface)
            => !Surface.HasValue || Surface.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this surface is NOT null or empty.
        /// </summary>
        /// <param name="Surface">A surface.</param>
        public static Boolean IsNotNullOrEmpty(this Surface? Surface)
            => Surface.HasValue && Surface.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A surface: the surface of a parking bay (OCPI Accessibility Extension 1.0.0).
    /// </summary>
    public readonly struct Surface : IId<Surface>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this surface is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this surface is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the surface.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new surface based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a surface.</param>
        private Surface(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as a surface.
        /// </summary>
        /// <param name="Text">A text representation of a surface.</param>
        public static Surface Parse(String Text)
        {

            ArgumentNullException.ThrowIfNull(Text);

            if (TryParse(Text, out var surface))
                return surface;

            throw new ArgumentException($"Invalid text representation of a surface: '" + Text + "'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a surface.
        /// </summary>
        /// <param name="Text">A text representation of a surface.</param>
        public static Surface? TryParse(String Text)
        {

            if (TryParse(Text, out var surface))
                return surface;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out Surface)

        /// <summary>
        /// Try to parse the given text as a surface.
        /// </summary>
        /// <param name="Text">A text representation of a surface.</param>
        /// <param name="Surface">The parsed surface.</param>
        public static Boolean TryParse(String Text, out Surface Surface)
        {

            Text = Text?.Trim() ?? "";

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    Surface = new Surface(Text);
                    return true;
                }
                catch
                { }
            }

            Surface = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this surface.
        /// </summary>
        public Surface Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// Hard asphalt surface.
        /// </summary>
        public static Surface  ASPHALT     { get; }
            = new ("ASPHALT");

        /// <summary>
        /// Hard concrete surface.
        /// </summary>
        public static Surface  CONCRETE    { get; }
            = new ("CONCRETE");

        /// <summary>
        /// Firm surface made of paving tiles or pavers.
        /// </summary>
        public static Surface  TILES       { get; }
            = new ("TILES");

        /// <summary>
        /// Loose gravel surface.
        /// </summary>
        public static Surface  GRAVEL      { get; }
            = new ("GRAVEL");

        /// <summary>
        /// Natural grass surface.
        /// </summary>
        public static Surface  GRASS       { get; }
            = new ("GRASS");

        /// <summary>
        /// Bare or compacted earth surface.
        /// </summary>
        public static Surface  SOIL        { get; }
            = new ("SOIL");

        #endregion


        #region Operator overloading

        #region Operator == (Surface1, Surface2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Surface1">A surface.</param>
        /// <param name="Surface2">Another surface.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Surface Surface1,
                                           Surface Surface2)

            => Surface1.Equals(Surface2);

        #endregion

        #region Operator != (Surface1, Surface2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Surface1">A surface.</param>
        /// <param name="Surface2">Another surface.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Surface Surface1,
                                           Surface Surface2)

            => !Surface1.Equals(Surface2);

        #endregion

        #region Operator <  (Surface1, Surface2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Surface1">A surface.</param>
        /// <param name="Surface2">Another surface.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Surface Surface1,
                                          Surface Surface2)

            => Surface1.CompareTo(Surface2) < 0;

        #endregion

        #region Operator <= (Surface1, Surface2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Surface1">A surface.</param>
        /// <param name="Surface2">Another surface.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Surface Surface1,
                                           Surface Surface2)

            => Surface1.CompareTo(Surface2) <= 0;

        #endregion

        #region Operator >  (Surface1, Surface2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Surface1">A surface.</param>
        /// <param name="Surface2">Another surface.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Surface Surface1,
                                          Surface Surface2)

            => Surface1.CompareTo(Surface2) > 0;

        #endregion

        #region Operator >= (Surface1, Surface2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Surface1">A surface.</param>
        /// <param name="Surface2">Another surface.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Surface Surface1,
                                           Surface Surface2)

            => Surface1.CompareTo(Surface2) >= 0;

        #endregion

        #endregion

        #region IComparable<Surface> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two surfaces.
        /// </summary>
        /// <param name="Object">A surface to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Surface surface
                   ? CompareTo(surface)
                   : throw new ArgumentException("The given object is not a surface!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(Surface)

        /// <summary>
        /// Compares two surfaces.
        /// </summary>
        /// <param name="Surface">A surface to compare with.</param>
        public Int32 CompareTo(Surface Surface)

            => String.Compare(InternalId,
                              Surface.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<Surface> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two surfaces for equality.
        /// </summary>
        /// <param name="Object">A surface to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Surface surface &&
                   Equals(surface);

        #endregion

        #region Equals(Surface)

        /// <summary>
        /// Compares two surfaces for equality.
        /// </summary>
        /// <param name="Surface">A surface to compare with.</param>
        public Boolean Equals(Surface Surface)

            => String.Equals(InternalId,
                             Surface.InternalId,
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
