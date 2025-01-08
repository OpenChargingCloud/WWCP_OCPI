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
    /// Extension methods for image categories.
    /// </summary>
    public static class ImageCategoryExtensions
    {

        /// <summary>
        /// Indicates whether this image category is null or empty.
        /// </summary>
        /// <param name="ImageCategory">An image category.</param>
        public static Boolean IsNullOrEmpty(this ImageCategory? ImageCategory)
            => !ImageCategory.HasValue || ImageCategory.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this image category is NOT null or empty.
        /// </summary>
        /// <param name="ImageCategory">An image category.</param>
        public static Boolean IsNotNullOrEmpty(this ImageCategory? ImageCategory)
            => ImageCategory.HasValue && ImageCategory.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The unique identification of an image category.
    /// </summary>
    public readonly struct ImageCategory : IId<ImageCategory>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this image category is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this image category is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the image category.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new image category based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of an image category.</param>
        private ImageCategory(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as an image category.
        /// </summary>
        /// <param name="Text">A text representation of an image category.</param>
        public static ImageCategory Parse(String Text)
        {

            if (TryParse(Text, out var imageCategory))
                return imageCategory;

            throw new ArgumentException($"Invalid text representation of an image category: '" + Text + "'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as an image category.
        /// </summary>
        /// <param name="Text">A text representation of an image category.</param>
        public static ImageCategory? TryParse(String Text)
        {

            if (TryParse(Text, out var imageCategory))
                return imageCategory;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out ImageCategory)

        /// <summary>
        /// Try to parse the given text as an image category.
        /// </summary>
        /// <param name="Text">A text representation of an image category.</param>
        /// <param name="ImageCategory">The parsed Image category.</param>
        public static Boolean TryParse(String Text, out ImageCategory ImageCategory)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    ImageCategory = new ImageCategory(Text);
                    return true;
                }
                catch
                { }
            }

            ImageCategory = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this image category.
        /// </summary>
        public ImageCategory Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// Photo of the physical device that contains one or more EVSEs.
        /// </summary>
        public static ImageCategory CHARGER
            => new ("CHARGER");

        /// <summary>
        /// Location entrance photo. Should show the car entrance to the location from street side.
        /// </summary>
        public static ImageCategory ENTRANCE
            => new ("ENTRANCE");

        /// <summary>
        /// Location overview photo.
        /// </summary>
        public static ImageCategory LOCATION
            => new ("LOCATION");

        /// <summary>
        /// Logo of a associated roaming network to be displayed with the EVSE for example in lists, maps and detailed information view.
        /// </summary>
        public static ImageCategory NETWORK
            => new ("NETWORK");

        /// <summary>
        /// Logo of the charge points operator, for example a municipal, to be displayed with the EVSEs detailed information view or in lists and maps, if no networkLogo is present.
        /// </summary>
        public static ImageCategory OPERATOR
            => new ("OPERATOR");

        /// <summary>
        /// Other.
        /// </summary>
        public static ImageCategory OTHER
            => new ("OTHER");

        /// <summary>
        /// Logo of the charge points owner, for example a local store, to be displayed with the EVSEs detailed information view.
        /// </summary>
        public static ImageCategory OWNER
            => new ("OWNER");

        #endregion


        #region Operator overloading

        #region Operator == (ImageCategory1, ImageCategory2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ImageCategory1">An image category.</param>
        /// <param name="ImageCategory2">Another Image category.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ImageCategory ImageCategory1,
                                           ImageCategory ImageCategory2)

            => ImageCategory1.Equals(ImageCategory2);

        #endregion

        #region Operator != (ImageCategory1, ImageCategory2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ImageCategory1">An image category.</param>
        /// <param name="ImageCategory2">Another Image category.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ImageCategory ImageCategory1,
                                           ImageCategory ImageCategory2)

            => !ImageCategory1.Equals(ImageCategory2);

        #endregion

        #region Operator <  (ImageCategory1, ImageCategory2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ImageCategory1">An image category.</param>
        /// <param name="ImageCategory2">Another Image category.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ImageCategory ImageCategory1,
                                          ImageCategory ImageCategory2)

            => ImageCategory1.CompareTo(ImageCategory2) < 0;

        #endregion

        #region Operator <= (ImageCategory1, ImageCategory2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ImageCategory1">An image category.</param>
        /// <param name="ImageCategory2">Another Image category.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ImageCategory ImageCategory1,
                                           ImageCategory ImageCategory2)

            => ImageCategory1.CompareTo(ImageCategory2) <= 0;

        #endregion

        #region Operator >  (ImageCategory1, ImageCategory2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ImageCategory1">An image category.</param>
        /// <param name="ImageCategory2">Another Image category.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ImageCategory ImageCategory1,
                                          ImageCategory ImageCategory2)

            => ImageCategory1.CompareTo(ImageCategory2) > 0;

        #endregion

        #region Operator >= (ImageCategory1, ImageCategory2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ImageCategory1">An image category.</param>
        /// <param name="ImageCategory2">Another Image category.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ImageCategory ImageCategory1,
                                           ImageCategory ImageCategory2)

            => ImageCategory1.CompareTo(ImageCategory2) >= 0;

        #endregion

        #endregion

        #region IComparable<ImageCategory> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two Image categories.
        /// </summary>
        /// <param name="Object">An image category to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is ImageCategory imageCategory
                   ? CompareTo(imageCategory)
                   : throw new ArgumentException("The given object is not an image category!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(ImageCategory)

        /// <summary>
        /// Compares two Image categories.
        /// </summary>
        /// <param name="ImageCategory">An image category to compare with.</param>
        public Int32 CompareTo(ImageCategory ImageCategory)

            => String.Compare(InternalId,
                              ImageCategory.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<ImageCategory> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two Image categories for equality.
        /// </summary>
        /// <param name="Object">An image category to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ImageCategory imageCategory &&
                   Equals(imageCategory);

        #endregion

        #region Equals(ImageCategory)

        /// <summary>
        /// Compares two Image categories for equality.
        /// </summary>
        /// <param name="ImageCategory">An image category to compare with.</param>
        public Boolean Equals(ImageCategory ImageCategory)

            => String.Equals(InternalId,
                             ImageCategory.InternalId,
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
