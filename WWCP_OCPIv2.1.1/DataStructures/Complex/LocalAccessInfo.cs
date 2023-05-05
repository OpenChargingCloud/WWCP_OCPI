/*
 * Copyright (c) 2015-2023 GraphDefined GmbH
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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using System.Runtime.Intrinsics.X86;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_1_1
{

    /// <summary>
    /// Local Access information.
    /// </summary>
    public readonly struct LocalAccessInfo : IEquatable<LocalAccessInfo>,
                                             IComparable<LocalAccessInfo>,
                                             IComparable
    {

        #region Properties

        /// <summary>
        /// The access token.
        /// </summary>
        [Mandatory]
        public AccessToken       AccessToken                   { get; }

        /// <summary>
        /// Whether the access token is base64 encoded or not.
        /// </summary>
        [Mandatory]
        public Boolean           AccessTokenIsBase64Encoded    { get; }

        /// <summary>
        /// The access status.
        /// </summary>
        [Mandatory]
        public AccessStatus      Status                        { get; }

        /// <summary>
        /// The country code.
        /// </summary>
        [Mandatory]
        public CountryCode       CountryCode                   { get; }

        /// <summary>
        /// The party identification.
        /// </summary>
        [Mandatory]
        public Party_Id          PartyId                       { get; }

        /// <summary>
        /// The role.
        /// </summary>
        [Mandatory]
        public Roles             Role                          { get; }

        /// <summary>
        /// The optional URL to get the remote OCPI versions information.
        /// </summary>
        [Optional]
        public URL?              VersionsURL                   { get; }

        /// <summary>
        /// Optional business details.
        /// </summary>
        [Optional]
        public BusinessDetails?  BusinessDetails               { get; }

        /// <summary>
        /// (Dis-)allow PUTting of object having an earlier 'LastUpdated'-timestamp then already existing objects.
        /// OCPI does not define any behaviour for this.
        /// </summary>
        [Optional]
        public Boolean           AllowDowngrades               { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new local access information.
        /// </summary>
        /// <param name="AccessToken">An access token.</param>
        /// <param name="Status">An access status.</param>
        /// <param name="CountryCode">A country code.</param>
        /// <param name="PartyId">A party identification.</param>
        /// <param name="Role">A role.</param>
        /// <param name="VersionsURL">An optional URL to get the remote OCPI versions information.</param>
        /// <param name="BusinessDetails">Optional business details.</param>
        /// <param name="AccessTokenIsBase64Encoded">Whether the access token is base64 encoded or not.</param>
        /// <param name="AllowDowngrades">(Dis-)allow PUTting of object having an earlier 'LastUpdated'-timestamp then already existing objects.</param>
        public LocalAccessInfo(AccessToken       AccessToken,
                               AccessStatus      Status,
                               CountryCode       CountryCode,
                               Party_Id          PartyId,
                               Roles             Role,
                               URL?              VersionsURL                  = null,
                               BusinessDetails?  BusinessDetails              = null,
                               Boolean?          AccessTokenIsBase64Encoded   = false,
                               Boolean?          AllowDowngrades              = false)
        {

            this.AccessToken                 = AccessToken;
            this.Status                      = Status;
            this.CountryCode                 = CountryCode;
            this.PartyId                     = PartyId;
            this.Role                        = Role;
            this.VersionsURL                 = VersionsURL;
            this.BusinessDetails             = BusinessDetails;
            this.AccessTokenIsBase64Encoded  = AccessTokenIsBase64Encoded ?? false;
            this.AllowDowngrades             = AllowDowngrades            ?? false;

            unchecked
            {

                this.hashCode = this.AccessToken.               GetHashCode()       * 23 ^
                                this.AccessTokenIsBase64Encoded.GetHashCode()       * 19 ^
                                this.Status.                    GetHashCode()       * 17 ^
                                this.CountryCode.               GetHashCode()       * 13 ^
                                this.PartyId.                   GetHashCode()       * 11 ^
                                this.Role.                      GetHashCode()       *  7 ^
                               (this.VersionsURL?.              GetHashCode() ?? 0) *  5 ^
                               (this.BusinessDetails?.          GetHashCode() ?? 0) *  3 ^
                                this.AllowDowngrades.           GetHashCode();

            }

        }

        #endregion


        #region ToJSON(CustomLocalAccessInfoSerializer = null, CustomBusinessDetailsSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomLocalAccessInfoSerializer">A delegate to serialize custom local access information JSON objects.</param>
        /// <param name="CustomBusinessDetailsSerializer">A delegate to serialize custom business details JSON objects.</param>
        /// <param name="CustomImageSerializer">A delegate to serialize custom image JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<LocalAccessInfo>?  CustomLocalAccessInfoSerializer   = null,
                              CustomJObjectSerializerDelegate<BusinessDetails>?  CustomBusinessDetailsSerializer   = null,
                              CustomJObjectSerializerDelegate<Image>?            CustomImageSerializer             = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("accesstoken",                  AccessToken.    ToString()),
                                 new JProperty("accessTokenIsBase64Encoded",   AccessTokenIsBase64Encoded),
                                 new JProperty("status",                       Status.         ToString()),
                                 new JProperty("countryCode",                  CountryCode.    ToString()),
                                 new JProperty("partyId",                      PartyId.        ToString()),
                                 new JProperty("role",                         Role.           ToString()),

                           VersionsURL.HasValue
                               ? new JProperty("versionsURL",                  VersionsURL.    ToString())
                               : null,

                           BusinessDetails is not null
                               ? new JProperty("businessDetails",              BusinessDetails.ToJSON(CustomBusinessDetailsSerializer,
                                                                                                      CustomImageSerializer))
                               : null,

                           AllowDowngrades
                               ? new JProperty("allow_downgrades",             AllowDowngrades)
                               : null

                       );

            return CustomLocalAccessInfoSerializer is not null
                       ? CustomLocalAccessInfoSerializer(this, json)
                       : json;

        }

        #endregion


        #region Clone()

        /// <summary>
        /// Clone this object.
        /// </summary>
        public LocalAccessInfo Clone()

            => new(AccessToken,
                   Status,
                   CountryCode,
                   PartyId,
                   Role,
                   VersionsURL,
                   BusinessDetails,
                   AccessTokenIsBase64Encoded,
                   AllowDowngrades);

        #endregion


        //public Credentials? AsCredentials

        //    => VersionsURL.    HasValue &&
        //       BusinessDetails is not null

        //           ? new Credentials(AccessToken,
        //                             VersionsURL.Value,
        //                             BusinessDetails,
        //                             CountryCode,
        //                             PartyId)

        //           : null;


        #region Operator overloading

        #region Operator == (AccessInfo1, AccessInfo2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AccessInfo1">An access information.</param>
        /// <param name="AccessInfo2">Another access information.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (LocalAccessInfo AccessInfo1,
                                           LocalAccessInfo AccessInfo2)

            => AccessInfo1.Equals(AccessInfo2);

        #endregion

        #region Operator != (AccessInfo1, AccessInfo2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AccessInfo1">An access information.</param>
        /// <param name="AccessInfo2">Another access information.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (LocalAccessInfo AccessInfo1,
                                           LocalAccessInfo AccessInfo2)

            => !AccessInfo1.Equals(AccessInfo2);

        #endregion

        #region Operator <  (AccessInfo1, AccessInfo2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AccessInfo1">An access information.</param>
        /// <param name="AccessInfo2">Another access information.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (LocalAccessInfo AccessInfo1,
                                          LocalAccessInfo AccessInfo2)

            => AccessInfo1.CompareTo(AccessInfo2) < 0;

        #endregion

        #region Operator <= (AccessInfo1, AccessInfo2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AccessInfo1">An access information.</param>
        /// <param name="AccessInfo2">Another access information.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (LocalAccessInfo AccessInfo1,
                                           LocalAccessInfo AccessInfo2)

            => AccessInfo1.CompareTo(AccessInfo2) <= 0;

        #endregion

        #region Operator >  (AccessInfo1, AccessInfo2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AccessInfo1">An access information.</param>
        /// <param name="AccessInfo2">Another access information.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (LocalAccessInfo AccessInfo1,
                                          LocalAccessInfo AccessInfo2)

            => AccessInfo1.CompareTo(AccessInfo2) > 0;

        #endregion

        #region Operator >= (AccessInfo1, AccessInfo2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AccessInfo1">An access information.</param>
        /// <param name="AccessInfo2">Another access information.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (LocalAccessInfo AccessInfo1,
                                           LocalAccessInfo AccessInfo2)

            => AccessInfo1.CompareTo(AccessInfo2) >= 0;

        #endregion

        #endregion

        #region IComparable<AccessInfo> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two access information.
        /// </summary>
        /// <param name="Object">An access information to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is LocalAccessInfo accessInfo
                   ? CompareTo(accessInfo)
                   : throw new ArgumentException("The given object is not a access information!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(AccessInfo)

        /// <summary>
        /// Compares two access information.
        /// </summary>
        /// <param name="AccessInfo">An access information to compare with.</param>
        public Int32 CompareTo(LocalAccessInfo AccessInfo)
        {

            var c = AccessToken.               CompareTo(AccessInfo.AccessToken);

            if (c == 0)
                c = AccessTokenIsBase64Encoded.CompareTo(AccessInfo.AccessTokenIsBase64Encoded);

            if (c == 0)
                c = Status.                    CompareTo(AccessInfo.Status);

            if (c == 0)
                c = CountryCode.               CompareTo(AccessInfo.CountryCode);

            if (c == 0)
                c = PartyId.                   CompareTo(AccessInfo.PartyId);

            if (c == 0)
                c = Role.                      CompareTo(AccessInfo.Role);

            //if (c == 0)
            //    c = LastUpdated.               CompareTo(AccessInfo.LastUpdated);

            if (c == 0 && VersionsURL.HasValue && AccessInfo.VersionsURL.HasValue)
                c = VersionsURL.Value.         CompareTo(AccessInfo.VersionsURL.Value);

            if (c == 0 && BusinessDetails is not null && AccessInfo.BusinessDetails is not null)
                c = BusinessDetails.           CompareTo(AccessInfo.BusinessDetails);

            if (c == 0)
                c = AllowDowngrades.           CompareTo(AccessInfo.AllowDowngrades);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<AccessInfo> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two access information for equality.
        /// </summary>
        /// <param name="Object">An access information to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is LocalAccessInfo accessInfo &&
                   Equals(accessInfo);

        #endregion

        #region Equals(AccessInfo)

        /// <summary>
        /// Compares two access information for equality.
        /// </summary>
        /// <param name="AccessInfo">An access information to compare with.</param>
        public Boolean Equals(LocalAccessInfo AccessInfo)

            => AccessToken.               Equals(AccessInfo.AccessToken)                &&
               AccessTokenIsBase64Encoded.Equals(AccessInfo.AccessTokenIsBase64Encoded) &&
               Status.                    Equals(AccessInfo.Status)                     &&
               CountryCode.               Equals(AccessInfo.CountryCode)                &&
               PartyId.                   Equals(AccessInfo.PartyId)                    &&
               Role.                      Equals(AccessInfo.Role)                       &&
               AllowDowngrades.           Equals(AccessInfo.AllowDowngrades)            &&

            ((!VersionsURL.    HasValue &&   !AccessInfo.VersionsURL.    HasValue) ||
              (VersionsURL.    HasValue &&    AccessInfo.VersionsURL.    HasValue    && VersionsURL.Value.Equals(AccessInfo.VersionsURL.Value))) &&

             ((BusinessDetails is null     && AccessInfo.BusinessDetails is null)  ||
              (BusinessDetails is not null && AccessInfo.BusinessDetails is not null && BusinessDetails.  Equals(AccessInfo.BusinessDetails)));

        #endregion

        #endregion

        #region GetHashCode()

        private readonly Int32 hashCode;

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        public override Int32 GetHashCode()
            => hashCode;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"'{BusinessDetails?.Name ?? "<unknown>"}' ({CountryCode}{(Role == Roles.EMSP ? "-" : "*")}{PartyId} {Role.AsText()}) [{Status}] using '{AccessToken}' @ '{VersionsURL}' {(AllowDowngrades ? "[Downgrades allowed]" : "[No downgrades]")}";

        #endregion

    }

}
