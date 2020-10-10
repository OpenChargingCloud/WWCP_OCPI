/*
 * Copyright (c) 2014-2020 GraphDefined GmbH
 * This file is part of WWCP Core <https://github.com/OpenChargingCloud/WWCP_Core>
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#region Usings

using System;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2
{

    /// <summary>
    /// An uniform resource location (URL).
    /// </summary>
    public readonly struct URL : IId<URL>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this identification is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty

            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// The length of the uniform resource location.
        /// </summary>
        public UInt64 Length

            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new uniform resource location based on the given string.
        /// </summary>
        /// <param name="String">The string representation of the uniform resource location.</param>
        private URL(String String)
        {
            this.InternalId  = String;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as an uniform resource location.
        /// </summary>
        /// <param name="Text">A text representation of an uniform resource location.</param>
        public static URL Parse(String Text)
        {

            if (TryParse(Text, out URL url))
                return url;

            if (Text.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Text), "The given text representation of an uniform resource location must not be null or empty!");

            throw new ArgumentException("The given text representation of an uniform resource location is invalid!", nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as an uniform resource location.
        /// </summary>
        /// <param name="Text">A text representation of an uniform resource location.</param>
        public static URL? TryParse(String Text)
        {

            if (TryParse(Text, out URL url))
                return url;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out URL)

        /// <summary>
        /// Try to parse the given text as an uniform resource location.
        /// </summary>
        /// <param name="Text">A text representation of an uniform resource location.</param>
        /// <param name="URL">The parsed uniform resource location.</param>
        public static Boolean TryParse(String Text, out URL URL)
        {

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    URL = new URL(Text.Trim());
                    return true;
                }
                catch (Exception)
                { }
            }

            URL = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this uniform resource location.
        /// </summary>
        public URL Clone

            => new URL(
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Operator overloading

        #region Operator == (URL1, URL2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="URL1">A uniform resource location.</param>
        /// <param name="URL2">Another uniform resource location.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (URL URL1,
                                           URL URL2)

            => URL1.Equals(URL2);

        #endregion

        #region Operator != (URL1, URL2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="URL1">A uniform resource location.</param>
        /// <param name="URL2">Another uniform resource location.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (URL URL1,
                                           URL URL2)

            => !(URL1 == URL2);

        #endregion

        #region Operator <  (URL1, URL2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="URL1">A uniform resource location.</param>
        /// <param name="URL2">Another uniform resource location.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (URL URL1,
                                          URL URL2)

            => URL1.CompareTo(URL2) < 0;

        #endregion

        #region Operator <= (URL1, URL2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="URL1">A uniform resource location.</param>
        /// <param name="URL2">Another uniform resource location.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (URL URL1,
                                           URL URL2)

            => !(URL1 > URL2);

        #endregion

        #region Operator >  (URL1, URL2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="URL1">A uniform resource location.</param>
        /// <param name="URL2">Another uniform resource location.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (URL URL1,
                                          URL URL2)

            => URL1.CompareTo(URL2) > 0;

        #endregion

        #region Operator >= (URL1, URL2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="URL1">A uniform resource location.</param>
        /// <param name="URL2">Another uniform resource location.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (URL URL1,
                                           URL URL2)

            => !(URL1 < URL2);

        #endregion

        #endregion

        #region IComparable<URL> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is URL url
                   ? CompareTo(url)
                   : throw new ArgumentException("The given object is not an uniform resource location!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(URL)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="URL">An object to compare with.</param>
        public Int32 CompareTo(URL URL)

            => String.Compare(InternalId,
                              URL.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<URL> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is URL url &&
                   Equals(url);

        #endregion

        #region Equals(URL)

        /// <summary>
        /// Compares two uniform resource locations for equality.
        /// </summary>
        /// <param name="URL">An uniform resource location to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(URL URL)

            => String.Equals(InternalId,
                             URL.InternalId,
                             StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
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
