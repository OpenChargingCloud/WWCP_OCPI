/*
 * Copyright (c) 2015-2020 GraphDefined GmbH
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

using System;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2
{

    /// <summary>
    /// The unique identification of a language.
    /// </summary>
    public struct Language_Id : IId<Language_Id>
    {

        #region Data

        // CiString(36)

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
        /// The length of the language identification.
        /// </summary>
        public UInt64 Length

            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new language identification based on the given string.
        /// </summary>
        /// <param name="String">The string representation of the language identification.</param>
        private Language_Id(String String)
        {
            this.InternalId  = String;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a language identification.
        /// </summary>
        /// <param name="Text">A text representation of a language identification.</param>
        public static Language_Id Parse(String Text)
        {

            if (TryParse(Text, out Language_Id languageId))
                return languageId;

            if (Text.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Text), "The given text representation of a language identification must not be null or empty!");

            throw new ArgumentException("The given text representation of a language identification is invalid!", nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a language identification.
        /// </summary>
        /// <param name="Text">A text representation of a language identification.</param>
        public static Language_Id? TryParse(String Text)
        {

            if (TryParse(Text, out Language_Id languageId))
                return languageId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out LanguageId)

        /// <summary>
        /// Try to parse the given text as a language identification.
        /// </summary>
        /// <param name="Text">A text representation of a language identification.</param>
        /// <param name="LanguageId">The parsed language identification.</param>
        public static Boolean TryParse(String Text, out Language_Id LanguageId)
        {

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    LanguageId = new Language_Id(Text.Trim());
                    return true;
                }
                catch (Exception)
                { }
            }

            LanguageId = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this language identification.
        /// </summary>
        public Language_Id Clone

            => new Language_Id(
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Operator overloading

        #region Operator == (LanguageId1, LanguageId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LanguageId1">A language identification.</param>
        /// <param name="LanguageId2">Another language identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Language_Id LanguageId1,
                                           Language_Id LanguageId2)

            => LanguageId1.Equals(LanguageId2);

        #endregion

        #region Operator != (LanguageId1, LanguageId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LanguageId1">A language identification.</param>
        /// <param name="LanguageId2">Another language identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Language_Id LanguageId1,
                                           Language_Id LanguageId2)

            => !(LanguageId1 == LanguageId2);

        #endregion

        #region Operator <  (LanguageId1, LanguageId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LanguageId1">A language identification.</param>
        /// <param name="LanguageId2">Another language identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Language_Id LanguageId1,
                                          Language_Id LanguageId2)

            => LanguageId1.CompareTo(LanguageId2) < 0;

        #endregion

        #region Operator <= (LanguageId1, LanguageId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LanguageId1">A language identification.</param>
        /// <param name="LanguageId2">Another language identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Language_Id LanguageId1,
                                           Language_Id LanguageId2)

            => !(LanguageId1 > LanguageId2);

        #endregion

        #region Operator >  (LanguageId1, LanguageId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LanguageId1">A language identification.</param>
        /// <param name="LanguageId2">Another language identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Language_Id LanguageId1,
                                          Language_Id LanguageId2)

            => LanguageId1.CompareTo(LanguageId2) > 0;

        #endregion

        #region Operator >= (LanguageId1, LanguageId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LanguageId1">A language identification.</param>
        /// <param name="LanguageId2">Another language identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Language_Id LanguageId1,
                                           Language_Id LanguageId2)

            => !(LanguageId1 < LanguageId2);

        #endregion

        #endregion

        #region IComparable<LanguageId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is Language_Id languageId
                   ? CompareTo(languageId)
                   : throw new ArgumentException("The given object is not a language identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(LanguageId)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LanguageId">An object to compare with.</param>
        public Int32 CompareTo(Language_Id LanguageId)

            => String.Compare(InternalId,
                              LanguageId.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<LanguageId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is Language_Id languageId &&
                   Equals(languageId);

        #endregion

        #region Equals(LanguageId)

        /// <summary>
        /// Compares two language identifications for equality.
        /// </summary>
        /// <param name="LanguageId">An language identification to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(Language_Id LanguageId)

            => String.Equals(InternalId,
                             LanguageId.InternalId,
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
