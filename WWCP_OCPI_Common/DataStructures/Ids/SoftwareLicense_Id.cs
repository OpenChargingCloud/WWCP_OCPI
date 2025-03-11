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
    /// Extension methods for software license identifications.
    /// </summary>
    public static class OpenSourceLicenseIdExtensions
    {

        /// <summary>
        /// Indicates whether this software license identification is null or empty.
        /// </summary>
        /// <param name="OpenSourceLicenseId">A software license identification.</param>
        public static Boolean IsNullOrEmpty(this SoftwareLicense_Id? OpenSourceLicenseId)
            => !OpenSourceLicenseId.HasValue || OpenSourceLicenseId.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this software license identification is null or empty.
        /// </summary>
        /// <param name="OpenSourceLicenseId">A software license identification.</param>
        public static Boolean IsNotNullOrEmpty(this SoftwareLicense_Id? OpenSourceLicenseId)
            => OpenSourceLicenseId.HasValue && OpenSourceLicenseId.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The unique identification of a software license.
    /// </summary>
    [VendorExtension(VE.GraphDefined)]
    public readonly struct SoftwareLicense_Id : IId<SoftwareLicense_Id>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this software license identification is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this software license identification is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the software license identifier.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new OCPI software license identification based on the given string.
        /// </summary>
        private SoftwareLicense_Id(String Text)
        {
            InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a software license identification.
        /// </summary>
        /// <param name="Text">A text representation of a software license identification.</param>
        public static SoftwareLicense_Id Parse(String Text)
        {

            if (TryParse(Text, out var softwareLicenseId))
                return softwareLicenseId;

            throw new ArgumentException($"Invalid text representation of a software license identification: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given string as a software license identification.
        /// </summary>
        /// <param name="Text">A text representation of a software license identification.</param>
        public static SoftwareLicense_Id? TryParse(String Text)
        {

            if (TryParse(Text, out var softwareLicenseId))
                return softwareLicenseId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out OpenSourceLicenseId)

        /// <summary>
        /// Try to parse the given string as a software license identification.
        /// </summary>
        /// <param name="Text">A text representation of a software license identification.</param>
        /// <param name="OpenSourceLicenseId">The parsed software license identification.</param>
        public static Boolean TryParse(String Text, out SoftwareLicense_Id OpenSourceLicenseId)
        {

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    OpenSourceLicenseId = new SoftwareLicense_Id(Text.Trim());
                    return true;
                }
                catch
                { }
            }

            OpenSourceLicenseId = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this software license identification.
        /// </summary>
        public SoftwareLicense_Id Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Operator overloading

        #region Operator == (OpenSourceLicenseIdId1, OpenSourceLicenseIdId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OpenSourceLicenseIdId1">A software license identification.</param>
        /// <param name="OpenSourceLicenseIdId2">Another software license identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (SoftwareLicense_Id OpenSourceLicenseIdId1,
                                           SoftwareLicense_Id OpenSourceLicenseIdId2)

            => OpenSourceLicenseIdId1.Equals(OpenSourceLicenseIdId2);

        #endregion

        #region Operator != (OpenSourceLicenseIdId1, OpenSourceLicenseIdId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OpenSourceLicenseIdId1">A software license identification.</param>
        /// <param name="OpenSourceLicenseIdId2">Another software license identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (SoftwareLicense_Id OpenSourceLicenseIdId1,
                                           SoftwareLicense_Id OpenSourceLicenseIdId2)

            => !OpenSourceLicenseIdId1.Equals(OpenSourceLicenseIdId2);

        #endregion

        #region Operator <  (OpenSourceLicenseIdId1, OpenSourceLicenseIdId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OpenSourceLicenseIdId1">A software license identification.</param>
        /// <param name="OpenSourceLicenseIdId2">Another software license identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (SoftwareLicense_Id OpenSourceLicenseIdId1,
                                          SoftwareLicense_Id OpenSourceLicenseIdId2)

            => OpenSourceLicenseIdId1.CompareTo(OpenSourceLicenseIdId2) < 0;

        #endregion

        #region Operator <= (OpenSourceLicenseIdId1, OpenSourceLicenseIdId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OpenSourceLicenseIdId1">A software license identification.</param>
        /// <param name="OpenSourceLicenseIdId2">Another software license identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (SoftwareLicense_Id OpenSourceLicenseIdId1,
                                           SoftwareLicense_Id OpenSourceLicenseIdId2)

            => OpenSourceLicenseIdId1.CompareTo(OpenSourceLicenseIdId2) <= 0;

        #endregion

        #region Operator >  (OpenSourceLicenseIdId1, OpenSourceLicenseIdId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OpenSourceLicenseIdId1">A software license identification.</param>
        /// <param name="OpenSourceLicenseIdId2">Another software license identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (SoftwareLicense_Id OpenSourceLicenseIdId1,
                                          SoftwareLicense_Id OpenSourceLicenseIdId2)

            => OpenSourceLicenseIdId1.CompareTo(OpenSourceLicenseIdId2) > 0;

        #endregion

        #region Operator >= (OpenSourceLicenseIdId1, OpenSourceLicenseIdId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OpenSourceLicenseIdId1">A software license identification.</param>
        /// <param name="OpenSourceLicenseIdId2">Another software license identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (SoftwareLicense_Id OpenSourceLicenseIdId1,
                                           SoftwareLicense_Id OpenSourceLicenseIdId2)

            => OpenSourceLicenseIdId1.CompareTo(OpenSourceLicenseIdId2) >= 0;

        #endregion

        #endregion

        #region IComparable<OpenSourceLicenseId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is SoftwareLicense_Id softwareLicenseId
                   ? CompareTo(softwareLicenseId)
                   : throw new ArgumentException("The given object is not a software license identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(OpenSourceLicenseId)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OpenSourceLicenseId">An object to compare with.</param>
        public Int32 CompareTo(SoftwareLicense_Id OpenSourceLicenseId)

            => String.Compare(InternalId,
                              OpenSourceLicenseId.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<OpenSourceLicenseId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object? Object)

            => Object is SoftwareLicense_Id softwareLicenseId &&
                   Equals(softwareLicenseId);

        #endregion

        #region Equals(OpenSourceLicenseId)

        /// <summary>
        /// Compares two OpenSourceLicenseIds for equality.
        /// </summary>
        /// <param name="OpenSourceLicenseId">A software license identification to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(SoftwareLicense_Id OpenSourceLicenseId)

            => String.Equals(InternalId,
                             OpenSourceLicenseId.InternalId,
                             StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()

            => InternalId?.GetHashCode() ?? 0;

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
