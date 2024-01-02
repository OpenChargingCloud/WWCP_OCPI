/*
 * Copyright (c) 2015-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

namespace cloud.charging.open.protocols.OCPI
{

    /// <summary>
    /// Extension methods for image file types.
    /// </summary>
    public static class ImageFileTypeExtensions
    {

        /// <summary>
        /// Indicates whether this image file type is null or empty.
        /// </summary>
        /// <param name="ImageFileType">An image file type.</param>
        public static Boolean IsNullOrEmpty(this ImageFileType? ImageFileType)
            => !ImageFileType.HasValue || ImageFileType.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this image file type is NOT null or empty.
        /// </summary>
        /// <param name="ImageFileType">An image file type.</param>
        public static Boolean IsNotNullOrEmpty(this ImageFileType? ImageFileType)
            => ImageFileType.HasValue && ImageFileType.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The unique identification of an image file type.
    /// </summary>
    public readonly struct ImageFileType : IId<ImageFileType>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this image file type is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this image file type is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the image file type.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new image file type based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of an image file type.</param>
        private ImageFileType(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as an image file type.
        /// </summary>
        /// <param name="Text">A text representation of an image file type.</param>
        public static ImageFileType Parse(String Text)
        {

            if (TryParse(Text, out var imageFileType))
                return imageFileType;

            throw new ArgumentException($"Invalid text representation of an image file type: '" + Text + "'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as an image file type.
        /// </summary>
        /// <param name="Text">A text representation of an image file type.</param>
        public static ImageFileType? TryParse(String Text)
        {

            if (TryParse(Text, out var imageFileType))
                return imageFileType;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out ImageFileType)

        /// <summary>
        /// Try to parse the given text as an image file type.
        /// </summary>
        /// <param name="Text">A text representation of an image file type.</param>
        /// <param name="ImageFileType">The parsed image file type.</param>
        public static Boolean TryParse(String Text, out ImageFileType ImageFileType)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    ImageFileType = new ImageFileType(Text);
                    return true;
                }
                catch
                { }
            }

            ImageFileType = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this image file type.
        /// </summary>
        public ImageFileType Clone

            => new (
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Static definitions

#pragma warning disable IDE1006 // Naming Styles

        /// <summary>
        /// gif
        /// </summary>
        public static ImageFileType gif
            => new("gif");

        /// <summary>
        /// jpeg
        /// </summary>
        public static ImageFileType jpeg
            => new("jpeg");

        /// <summary>
        /// jpg
        /// </summary>
        public static ImageFileType jpg
            => new("jpg");

        /// <summary>
        /// png
        /// </summary>
        public static ImageFileType png
            => new("png");

        /// <summary>
        /// svg
        /// </summary>
        public static ImageFileType svg
            => new("svg");

        /// <summary>
        /// webp
        /// </summary>
        public static ImageFileType webp
            => new("webp");

#pragma warning restore IDE1006 // Naming Styles

        #endregion


        #region Operator overloading

        #region Operator == (ImageFileType1, ImageFileType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ImageFileType1">An image file type.</param>
        /// <param name="ImageFileType2">Another image file type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ImageFileType ImageFileType1,
                                           ImageFileType ImageFileType2)

            => ImageFileType1.Equals(ImageFileType2);

        #endregion

        #region Operator != (ImageFileType1, ImageFileType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ImageFileType1">An image file type.</param>
        /// <param name="ImageFileType2">Another image file type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ImageFileType ImageFileType1,
                                           ImageFileType ImageFileType2)

            => !ImageFileType1.Equals(ImageFileType2);

        #endregion

        #region Operator <  (ImageFileType1, ImageFileType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ImageFileType1">An image file type.</param>
        /// <param name="ImageFileType2">Another image file type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ImageFileType ImageFileType1,
                                          ImageFileType ImageFileType2)

            => ImageFileType1.CompareTo(ImageFileType2) < 0;

        #endregion

        #region Operator <= (ImageFileType1, ImageFileType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ImageFileType1">An image file type.</param>
        /// <param name="ImageFileType2">Another image file type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ImageFileType ImageFileType1,
                                           ImageFileType ImageFileType2)

            => ImageFileType1.CompareTo(ImageFileType2) <= 0;

        #endregion

        #region Operator >  (ImageFileType1, ImageFileType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ImageFileType1">An image file type.</param>
        /// <param name="ImageFileType2">Another image file type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ImageFileType ImageFileType1,
                                          ImageFileType ImageFileType2)

            => ImageFileType1.CompareTo(ImageFileType2) > 0;

        #endregion

        #region Operator >= (ImageFileType1, ImageFileType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ImageFileType1">An image file type.</param>
        /// <param name="ImageFileType2">Another image file type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ImageFileType ImageFileType1,
                                           ImageFileType ImageFileType2)

            => ImageFileType1.CompareTo(ImageFileType2) >= 0;

        #endregion

        #endregion

        #region IComparable<ImageFileType> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two image file types.
        /// </summary>
        /// <param name="Object">An image file type to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is ImageFileType imageFileType
                   ? CompareTo(imageFileType)
                   : throw new ArgumentException("The given object is not an image file type!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(ImageFileType)

        /// <summary>
        /// Compares two image file types.
        /// </summary>
        /// <param name="ImageFileType">An image file type to compare with.</param>
        public Int32 CompareTo(ImageFileType ImageFileType)

            => String.Compare(InternalId,
                              ImageFileType.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<ImageFileType> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two image file types for equality.
        /// </summary>
        /// <param name="Object">An image file type to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ImageFileType imageFileType &&
                   Equals(imageFileType);

        #endregion

        #region Equals(ImageFileType)

        /// <summary>
        /// Compares two image file types for equality.
        /// </summary>
        /// <param name="ImageFileType">An image file type to compare with.</param>
        public Boolean Equals(ImageFileType ImageFileType)

            => String.Equals(InternalId,
                             ImageFileType.InternalId,
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
