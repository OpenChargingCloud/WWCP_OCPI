/*
 * Copyright (c) 2015-2016 GraphDefined GmbH
 * This file is part of WWCP OCPI <https://github.com/GraphDefined/WWCP_OCPI>
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

using System;

#endregion

namespace org.GraphDefined.WWCP.OCPIv2_2
{

    /// <summary>
    /// This class references images related to a EVSE in terms of a file name or uri.
    /// </summary>
    public class Image
    {

        #region Properties

        #region Url

        private readonly Uri _Url;

        /// <summary>
        /// URL from where the image data can be fetched through a web browser (about 512 pixels).
        /// </summary>
        public Uri Url
        {
            get
            {
                return _Url;
            }
        }

        #endregion

        #region Thumbnail

        private readonly Uri _Thumbnail;

        /// <summary>
        /// URL from where a thumbnail of the image can be fetched through a webbrowser (about 128 pixels).
        /// </summary>
        public Uri Thumbnail
        {
            get
            {
                return _Thumbnail;
            }
        }

        #endregion

        #region Category

        private readonly ImageCategoryType _Category;

        /// <summary>
        /// Describes what the image is used for.
        /// </summary>
        public ImageCategoryType Category
        {
            get
            {
                return _Category;
            }
        }

        #endregion

        #region Type

        private readonly ImageFileType _Type;

        /// <summary>
        /// Image type like: gif, jpeg, png, svg.
        /// </summary>
        public ImageFileType Type
        {
            get
            {
                return _Type;
            }
        }

        #endregion

        #region Width

        private readonly UInt32 _Width;

        /// <summary>
        /// Width of the full scale image.
        /// </summary>
        public UInt32 Width
        {
            get
            {
                return _Width;
            }
        }

        #endregion

        #region Height

        private readonly UInt32 _Height;

        /// <summary>
        /// Height of the full scale image.
        /// </summary>
        public UInt32 Height
        {
            get
            {
                return _Height;
            }
        }

        #endregion

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new image.
        /// </summary>
        /// <param name="Url">URL from where the image data can be fetched through a web browser (about 512 pixels).</param>
        /// <param name="Thumbnail">URL from where a thumbnail of the image can be fetched through a webbrowser (about 128 pixels).</param>
        /// <param name="Category">Describes what the image is used for.</param>
        /// <param name="Type">Image type like: gif, jpeg, png, svg.</param>
        /// <param name="Width">Width of the full scale image.</param>
        /// <param name="Height">Height of the full scale image.</param>
        public Image(Uri                Url,
                     Uri                Thumbnail,
                     ImageCategoryType  Category,
                     ImageFileType      Type,
                     UInt32             Width,
                     UInt32             Height)
        {

            #region Initial checks

            if (Url == null)
                throw new ArgumentNullException("Url", "The given parameter must not be null!");

            #endregion

            this._Url        = Url;
            this._Thumbnail  = Thumbnail;
            this._Category   = Category;
            this._Type       = Type;
            this._Width      = Width;
            this._Height     = Height;

        }

        #endregion

    }

}
