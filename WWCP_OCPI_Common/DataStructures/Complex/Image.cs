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

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPI
{

    /// <summary>
    /// This class references images related to an EVSE in terms of a file name or uri.
    /// </summary>
    public class Image : IEquatable<Image>,
                         IComparable<Image>,
                         IComparable
    {

        #region Properties

        /// <summary>
        /// The URL from where the image data can be fetched.
        /// </summary>
        [Mandatory]
        public URL              URL          { get; }

        /// <summary>
        /// The image type like: gif, jpeg, png, svg.
        /// </summary>
        [Mandatory]
        public ImageFileType    Type         { get; }

        /// <summary>
        /// The description for what the image can be used for.
        /// </summary>
        [Mandatory]
        public ImageCategory    Category     { get; }

        /// <summary>
        /// The optional width of the full scale image.
        /// </summary>
        [Optional]
        public UInt16?          Width        { get; }

        /// <summary>
        /// The optional height of the full scale image.
        /// </summary>
        [Optional]
        public UInt16?          Height       { get; }

        /// <summary>
        /// The optional URL from where a thumbnail of the image can be fetched.
        /// </summary>
        [Optional]
        public URL?             Thumbnail    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new image.
        /// </summary>
        /// <param name="URL">An URL from where the image data can be fetched.</param>
        /// <param name="Type">An image type like: gif, jpeg, png, svg.</param>
        /// <param name="Category">An description for what the image can be used for.</param>
        /// <param name="Width">An optional width of the full scale image.</param>
        /// <param name="Height">An optional height of the full scale image.</param>
        /// <param name="Thumbnail">An optional URL from where a thumbnail of the image can be fetched.</param>
        public Image(URL            URL,
                     ImageFileType  Type,
                     ImageCategory  Category,
                     UInt16?        Width       = null,
                     UInt16?        Height      = null,
                     URL?           Thumbnail   = null)
        {

            this.URL        = URL;
            this.Type       = Type;
            this.Category   = Category;
            this.Width      = Width;
            this.Height     = Height;
            this.Thumbnail  = Thumbnail;

            unchecked
            {

                hashCode = URL.       GetHashCode()       * 13 ^
                           Type.      GetHashCode()       * 11 ^
                           Category.  GetHashCode()       *  7 ^
                          (Thumbnail?.GetHashCode() ?? 0) *  5 ^
                          (Width?.    GetHashCode() ?? 0) *  3 ^
                          (Height?.   GetHashCode() ?? 0);

            }

        }

        #endregion


        #region (static) Parse   (JSON, CustomImageParser = null)

        /// <summary>
        /// Parse the given JSON representation of an image.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomImageParser">A delegate to parse custom images JSON objects.</param>
        public static Image Parse(JObject                              JSON,
                                  CustomJObjectParserDelegate<Image>?  CustomImageParser   = null)
        {

            if (TryParse(JSON,
                         out var additionalGeoLocation,
                         out var errorResponse,
                         CustomImageParser))
            {
                return additionalGeoLocation;
            }

            throw new ArgumentException("The given JSON representation of an image is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out Image, out ErrorResponse, CustomImageParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of an image.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="Image">The parsed connector.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                           JSON,
                                       [NotNullWhen(true)]  out Image?   Image,
                                       [NotNullWhen(false)] out String?  ErrorResponse)

            => TryParse(JSON,
                        out Image,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of an image.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="Image">The parsed connector.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomImageParser">A delegate to parse custom images JSON objects.</param>
        public static Boolean TryParse(JObject                              JSON,
                                       [NotNullWhen(true)]  out Image?      Image,
                                       [NotNullWhen(false)] out String?     ErrorResponse,
                                       CustomJObjectParserDelegate<Image>?  CustomImageParser)
        {

            try
            {

                Image = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse URL         [mandatory]

                if (!JSON.ParseMandatory("url",
                                         "url",
                                         org.GraphDefined.Vanaheimr.Hermod.HTTP.URL.TryParse,
                                         out URL URL,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Category    [mandatory]

                if (!JSON.ParseMandatory("category",
                                         "image category",
                                         ImageCategory.TryParse,
                                         out ImageCategory Category,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Type        [mandatory]

                if (!JSON.ParseMandatory("type",
                                         "image type",
                                         ImageFileType.TryParse,
                                         out ImageFileType Type,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Width       [optional]

                if (JSON.ParseOptional("width",
                                       "image width",
                                       out UInt16? Width,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse Height      [optional]

                if (JSON.ParseOptional("height",
                                       "image height",
                                       out UInt16? Height,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse Thumbnail   [optional]

                if (JSON.ParseOptional("thumbnail",
                                       "image thumbnail",
                                       URL.TryParse,
                                       out URL? Thumbnail,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                Image = new Image(
                            URL,
                            Type,
                            Category,
                            Width,
                            Height,
                            Thumbnail
                        );


                if (CustomImageParser is not null)
                    Image = CustomImageParser(JSON,
                                              Image);

                return true;

            }
            catch (Exception e)
            {
                Image          = default;
                ErrorResponse  = "The given JSON representation of an image is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomImageSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomImageSerializer">A delegate to serialize custom image JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<Image>? CustomImageSerializer = null)
        {

            var JSON = JSONObject.Create(

                                 new JProperty("url",         URL.            ToString()),
                                 new JProperty("category",    Category.       ToString()),
                                 new JProperty("type",        Type.           ToString()),

                           Thumbnail.HasValue
                               ? new JProperty("thumbnail",   Thumbnail.Value.ToString())
                               : null,

                           Width.HasValue
                               ? new JProperty("width",       Width. Value)
                               : null,

                           Height.HasValue
                               ? new JProperty("height",      Height.Value)
                               : null

                       );

            return CustomImageSerializer is not null
                       ? CustomImageSerializer(this, JSON)
                       : JSON;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this object.
        /// </summary>
        public Image Clone()

            => new (
                   URL.     Clone(),
                   Type.    Clone(),
                   Category.Clone(),
                   Width.    HasValue ? Width.    Value : null,
                   Height.   HasValue ? Height.   Value : null,
                   Thumbnail.HasValue ? Thumbnail.Value : null
               );

        #endregion


        #region Operator overloading

        #region Operator == (Image1, Image2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Image1">An image.</param>
        /// <param name="Image2">Another image.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Image Image1,
                                           Image Image2)
        {

            if (Object.ReferenceEquals(Image1, Image2))
                return true;

            if (Image1 is null || Image2 is null)
                return false;

            return Image1.Equals(Image2);

        }

        #endregion

        #region Operator != (Image1, Image2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Image1">An image.</param>
        /// <param name="Image2">Another image.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Image Image1,
                                           Image Image2)

            => !(Image1 == Image2);

        #endregion

        #region Operator <  (Image1, Image2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Image1">A image.</param>
        /// <param name="Image2">Another image.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Image Image1,
                                          Image Image2)

            => Image1 is null
                   ? throw new ArgumentNullException(nameof(Image1), "The given image must not be null!")
                   : Image1.CompareTo(Image2) < 0;

        #endregion

        #region Operator <= (Image1, Image2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Image1">A image.</param>
        /// <param name="Image2">Another image.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Image Image1,
                                           Image Image2)

            => !(Image1 > Image2);

        #endregion

        #region Operator >  (Image1, Image2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Image1">A image.</param>
        /// <param name="Image2">Another image.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Image Image1,
                                          Image Image2)

            => Image1 is null
                   ? throw new ArgumentNullException(nameof(Image1), "The given image must not be null!")
                   : Image1.CompareTo(Image2) > 0;

        #endregion

        #region Operator >= (Image1, Image2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Image1">A image.</param>
        /// <param name="Image2">Another image.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Image Image1,
                                           Image Image2)

            => !(Image1 < Image2);

        #endregion

        #endregion

        #region IComparable<Image> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two images.
        /// </summary>
        /// <param name="Object">An image to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Image image
                   ? CompareTo(image)
                   : throw new ArgumentException("The given object is not an image!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(Image)

        /// <summary>
        /// Compares two images.
        /// </summary>
        /// <param name="Image">An image to compare with.</param>
        public Int32 CompareTo(Image? Image)
        {

            if (Image is null)
                throw new ArgumentNullException(nameof(Image),
                                                "The given image must not be null!");

            var c = URL.     CompareTo(Image.URL);

            if (c == 0)
                c = Type.    CompareTo(Image.Type);

            if (c == 0)
                c = Category.CompareTo(Image.Category);

            if (c == 0)
                c = Thumbnail.HasValue && Image.Thumbnail.HasValue
                        ? Thumbnail.Value.CompareTo(Image.Thumbnail.Value)
                        : 0;

            if (c == 0)
                c = Width. HasValue && Image.Width. HasValue
                        ? Width.    Value.CompareTo(Image.Width.    Value)
                        : 0;

            if (c == 0)
                c = Height.HasValue && Image.Height.HasValue
                        ? Height.   Value.CompareTo(Image.Height.   Value)
                        : 0;

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<Image> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two images for equality.
        /// </summary>
        /// <param name="Object">An image to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Image Image &&
                   Equals(Image);

        #endregion

        #region Equals(Image)

        /// <summary>
        /// Compares two images for equality.
        /// </summary>
        /// <param name="Image">An image to compare with.</param>
        public Boolean Equals(Image? Image)

            => Image is not null &&

               URL.     Equals(Image.URL)      &&
               Type.    Equals(Image.Type)     &&
               Category.Equals(Image.Category)  &&

            ((!Thumbnail.HasValue && !Image.Thumbnail.HasValue) ||
              (Thumbnail.HasValue &&  Image.Thumbnail.HasValue && Thumbnail.Value.Equals(Image.Thumbnail.Value))) &&

            ((!Width.    HasValue && !Image.Width.    HasValue) ||
              (Width.    HasValue &&  Image.Width.    HasValue && Width.    Value.Equals(Image.Width.    Value))) &&

            ((!Height.   HasValue && !Image.Height.   HasValue) ||
              (Height.   HasValue &&  Image.Height.   HasValue && Height.   Value.Equals(Image.Height.   Value)));

        #endregion

        #endregion

        #region (override) GetHashCode()

        private readonly Int32 hashCode;

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()
            => hashCode;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   URL,      "; ",
                   Type,     "; ",
                   Category, "; ",

                   Width.    HasValue &&
                   Height.   HasValue
                       ? "; " + Width + " x " + Height + " px"
                       : "",

                   Thumbnail.HasValue
                       ? "; " + Thumbnail
                       : ""

               );

        #endregion

    }

}
