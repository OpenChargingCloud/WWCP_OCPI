/*
 * Copyright (c) 2015-2023 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

namespace cloud.charging.open.protocols.OCPIv2_1_1
{

    /// <summary>
    /// Extension methods for Open Source license identifications.
    /// </summary>
    public static class OpenSourceLicenseIdExtensions
    {

        /// <summary>
        /// Indicates whether this Open Source license identification is null or empty.
        /// </summary>
        /// <param name="OpenSourceLicenseId">A Open Source license identification.</param>
        public static Boolean IsNullOrEmpty(this OpenSourceLicense_Id? OpenSourceLicenseId)
            => !OpenSourceLicenseId.HasValue || OpenSourceLicenseId.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this Open Source license identification is null or empty.
        /// </summary>
        /// <param name="OpenSourceLicenseId">A Open Source license identification.</param>
        public static Boolean IsNotNullOrEmpty(this OpenSourceLicense_Id? OpenSourceLicenseId)
            => OpenSourceLicenseId.HasValue && OpenSourceLicenseId.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The unique identification of an Open Source license.
    /// </summary>
    [NonStandard]
    public readonly struct OpenSourceLicense_Id : IId<OpenSourceLicense_Id>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this Open Source license identification is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this Open Source license identification is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the Open Source license identificator.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new OICP Open Source license identification based on the given string.
        /// </summary>
        private OpenSourceLicense_Id(String Text)
        {
            InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as an Open Source license identification.
        /// </summary>
        /// <param name="Text">A text representation of an Open Source license identification.</param>
        public static OpenSourceLicense_Id Parse(String Text)
        {

            if (TryParse(Text, out var openSourceLicenseId))
                return openSourceLicenseId;

            throw new ArgumentException("Invalid text representation of an Open Source license identification: '" + Text + "'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given string as an Open Source license identification.
        /// </summary>
        /// <param name="Text">A text representation of an Open Source license identification.</param>
        public static OpenSourceLicense_Id? TryParse(String Text)
        {

            if (TryParse(Text, out var openSourceLicenseId))
                return openSourceLicenseId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out OpenSourceLicenseId)

        /// <summary>
        /// Try to parse the given string as an Open Source license identification.
        /// </summary>
        /// <param name="Text">A text representation of an Open Source license identification.</param>
        /// <param name="OpenSourceLicenseId">The parsed Open Source license identification.</param>
        public static Boolean TryParse(String Text, out OpenSourceLicense_Id OpenSourceLicenseId)
        {

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    OpenSourceLicenseId = new OpenSourceLicense_Id(Text.Trim());
                    return true;
                }
                catch
                { }
            }

            OpenSourceLicenseId = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this Open Source license identification.
        /// </summary>
        public OpenSourceLicense_Id Clone

            => new (
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Operator overloading

        #region Operator == (OpenSourceLicenseIdId1, OpenSourceLicenseIdId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OpenSourceLicenseIdId1">A Open Source license identification.</param>
        /// <param name="OpenSourceLicenseIdId2">Another Open Source license identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (OpenSourceLicense_Id OpenSourceLicenseIdId1,
                                           OpenSourceLicense_Id OpenSourceLicenseIdId2)

            => OpenSourceLicenseIdId1.Equals(OpenSourceLicenseIdId2);

        #endregion

        #region Operator != (OpenSourceLicenseIdId1, OpenSourceLicenseIdId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OpenSourceLicenseIdId1">A Open Source license identification.</param>
        /// <param name="OpenSourceLicenseIdId2">Another Open Source license identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (OpenSourceLicense_Id OpenSourceLicenseIdId1,
                                           OpenSourceLicense_Id OpenSourceLicenseIdId2)

            => !OpenSourceLicenseIdId1.Equals(OpenSourceLicenseIdId2);

        #endregion

        #region Operator <  (OpenSourceLicenseIdId1, OpenSourceLicenseIdId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OpenSourceLicenseIdId1">A Open Source license identification.</param>
        /// <param name="OpenSourceLicenseIdId2">Another Open Source license identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (OpenSourceLicense_Id OpenSourceLicenseIdId1,
                                          OpenSourceLicense_Id OpenSourceLicenseIdId2)

            => OpenSourceLicenseIdId1.CompareTo(OpenSourceLicenseIdId2) < 0;

        #endregion

        #region Operator <= (OpenSourceLicenseIdId1, OpenSourceLicenseIdId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OpenSourceLicenseIdId1">A Open Source license identification.</param>
        /// <param name="OpenSourceLicenseIdId2">Another Open Source license identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (OpenSourceLicense_Id OpenSourceLicenseIdId1,
                                           OpenSourceLicense_Id OpenSourceLicenseIdId2)

            => OpenSourceLicenseIdId1.CompareTo(OpenSourceLicenseIdId2) <= 0;

        #endregion

        #region Operator >  (OpenSourceLicenseIdId1, OpenSourceLicenseIdId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OpenSourceLicenseIdId1">A Open Source license identification.</param>
        /// <param name="OpenSourceLicenseIdId2">Another Open Source license identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (OpenSourceLicense_Id OpenSourceLicenseIdId1,
                                          OpenSourceLicense_Id OpenSourceLicenseIdId2)

            => OpenSourceLicenseIdId1.CompareTo(OpenSourceLicenseIdId2) > 0;

        #endregion

        #region Operator >= (OpenSourceLicenseIdId1, OpenSourceLicenseIdId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OpenSourceLicenseIdId1">A Open Source license identification.</param>
        /// <param name="OpenSourceLicenseIdId2">Another Open Source license identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (OpenSourceLicense_Id OpenSourceLicenseIdId1,
                                           OpenSourceLicense_Id OpenSourceLicenseIdId2)

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

            => Object is OpenSourceLicense_Id openSourceLicenseId
                   ? CompareTo(openSourceLicenseId)
                   : throw new ArgumentException("The given object is not an Open Source license identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(OpenSourceLicenseId)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OpenSourceLicenseId">An object to compare with.</param>
        public Int32 CompareTo(OpenSourceLicense_Id OpenSourceLicenseId)

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

            => Object is OpenSourceLicense_Id openSourceLicenseId &&
                   Equals(openSourceLicenseId);

        #endregion

        #region Equals(OpenSourceLicenseId)

        /// <summary>
        /// Compares two OpenSourceLicenseIds for equality.
        /// </summary>
        /// <param name="OpenSourceLicenseId">A Open Source license identification to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(OpenSourceLicense_Id OpenSourceLicenseId)

            => String.Equals(InternalId,
                             OpenSourceLicenseId.InternalId,
                             StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region GetHashCode()

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
