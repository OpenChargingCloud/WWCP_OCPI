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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2_1
{

    public static class AccessInfoextentions
    {

        public static Boolean Is(this LocalAccessInfo?  AccessInfo,
                                 Roles             Role)

            => AccessInfo.HasValue &&
               AccessInfo.Value.Is(Role);

        public static Boolean IsNot(this LocalAccessInfo?  AccessInfo,
                                    Roles             Role)

            => !AccessInfo.HasValue ||
                AccessInfo.Value.IsNot(Role);

    }


    /// <summary>
    /// Local Access information.
    /// </summary>
    public readonly struct LocalAccessInfo
    {

        #region Properties

        /// <summary>
        /// The access token.
        /// </summary>
        [Mandatory]
        public AccessToken                   AccessToken                   { get; }

        /// <summary>
        /// Whether the access token is base64 encoded or not.
        /// </summary>
        [Mandatory]
        public Boolean                       AccessTokenIsBase64Encoded    { get; }

        /// <summary>
        /// The access status.
        /// </summary>
        [Mandatory]
        public AccessStatus                  Status                        { get; }

        /// <summary>
        /// The credential roles.
        /// </summary>
        [Mandatory]
        public IEnumerable<CredentialsRole>  Roles                         { get; }

        /// <summary>
        /// The optional URL to get the remote OCPI versions information.
        /// </summary>
        [Optional]
        public URL?                          VersionsURL                   { get; }


        public Boolean Is(Roles Role)
            =>  Roles.Any(role => role.Role == Role);

        public Boolean IsNot(Roles Role)
            => !Roles.Any(role => role.Role == Role);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new Local access information.
        /// </summary>
        /// <param name="AccessToken">An access token.</param>
        /// <param name="Status">An access status.</param>
        /// <param name="Roles">an enumeration of credential roles.</param>
        /// <param name="VersionsURL">An optional URL to get the remote OCPI versions information.</param>
        /// <param name="AccessTokenIsBase64Encoded">Whether the access token is base64 encoded or not.</param>
        public LocalAccessInfo(AccessToken                    AccessToken,
                               AccessStatus                   Status,
                               IEnumerable<CredentialsRole>?  Roles                        = null,
                               URL?                           VersionsURL                  = null,
                               Boolean?                       AccessTokenIsBase64Encoded   = false)
        {

            this.AccessToken                 = AccessToken;
            this.Status                      = Status;
            this.Roles                       = Roles?.Distinct()          ?? Array.Empty<CredentialsRole>();
            this.VersionsURL                 = VersionsURL;
            this.AccessTokenIsBase64Encoded  = AccessTokenIsBase64Encoded ?? false;

            unchecked
            {

                this.hashCode = this.AccessToken.               GetHashCode()       * 11 ^
                                this.Status.                    GetHashCode()       *  7 ^
                                this.Roles.                     CalcHashCode()      *  5 ^
                               (this.VersionsURL?.              GetHashCode() ?? 0) *  3 ^
                                this.AccessTokenIsBase64Encoded.GetHashCode();

            }

        }

        #endregion


        #region ToJSON(CustomLocalAccessInfoSerializer = null, CustomCredentialsRoleSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomLocalAccessInfoSerializer">A delegate to serialize custom local access information JSON objects.</param>
        /// <param name="CustomCredentialsRoleSerializer">A delegate to serialize custom credentials role JSON objects.</param>
        /// <param name="CustomBusinessDetailsSerializer">A delegate to serialize custom business details JSON objects.</param>
        /// <param name="CustomImageSerializer">A delegate to serialize custom image JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<LocalAccessInfo>?  CustomLocalAccessInfoSerializer   = null,
                              CustomJObjectSerializerDelegate<CredentialsRole>?  CustomCredentialsRoleSerializer   = null,
                              CustomJObjectSerializerDelegate<BusinessDetails>?  CustomBusinessDetailsSerializer   = null,
                              CustomJObjectSerializerDelegate<Image>?            CustomImageSerializer             = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("accesstoken",                  AccessToken.ToString()),
                                 new JProperty("accessTokenIsBase64Encoded",   AccessTokenIsBase64Encoded),
                                 new JProperty("status",                       Status.     ToString()),
                                 new JProperty("roles",                        new JArray(Roles.Select(role => role.ToJSON(CustomCredentialsRoleSerializer,
                                                                                                                           CustomBusinessDetailsSerializer,
                                                                                                                           CustomImageSerializer)))),

                           VersionsURL.HasValue
                               ? new JProperty("versionsURL",                  VersionsURL.ToString())
                               : null

                       );

            return CustomLocalAccessInfoSerializer is not null
                       ? CustomLocalAccessInfoSerializer(this, json)
                       : json;

        }

        #endregion

        //public JObject ToJSON()
        //{

        //    return JSONObject.Create(
        //               new JProperty("accesstoken",   AccessToken.ToString()),
        //               new JProperty("status",        Status.     ToString(),
        //               new JProperty("roles",         new JArray(Roles.Select(role => role.ToJSON()))),
        //               new JProperty("versionsURL",   VersionsURL.ToString()))
        //           );

        //}


        #region Clone()

        /// <summary>
        /// Clone this object.
        /// </summary>
        public LocalAccessInfo Clone()

            => new(AccessToken,
                   Status,
                   Roles.Select(credentialsRole => credentialsRole.Clone()).ToArray(),
                   VersionsURL,
                   AccessTokenIsBase64Encoded);

        #endregion


        //public Credentials AsCredentials()

        //    => new (AccessToken,
        //            VersionsURL.Value,
        //            Roles.Select(role => new CredentialsRole(role.CountryCode,
        //                                                     role.PartyId,
        //                                                     role.Role,
        //                                                     role.BusinessDetails)));


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

            var c = AccessToken.CompareTo(AccessInfo.AccessToken);

            if (c == 0)
                c = Status.     CompareTo(AccessInfo.Status);

            // Roles

            if (c == 0 && VersionsURL.HasValue && AccessInfo.VersionsURL.HasValue)
                c = VersionsURL.Value.CompareTo(AccessInfo.VersionsURL.Value);

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

            => AccessToken.Equals(AccessInfo.AccessToken) &&
               Status.     Equals(AccessInfo.Status)      &&

               Roles.Count().Equals(AccessInfo.Roles.Count()) &&
               Roles.All(AccessInfo.Roles.Contains) &&

            ((!VersionsURL.HasValue && !AccessInfo.VersionsURL.HasValue) ||
              (VersionsURL.HasValue &&  AccessInfo.VersionsURL.HasValue && VersionsURL.Value.Equals(AccessInfo.VersionsURL.Value)));

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

            //=> $"{AccessToken}{(AccessTokenBase64Encoding ? "[base64}" : "")} @ {VersionsURL} {Status}";
            => $"'{AccessToken}' {Status} @ '{VersionsURL}'";
            //=> $"{AccessToken}{(AccessTokenIsBase64Encoded ? "[base64}" : "")} for {BusinessDetails.Name} ({CountryCode}{PartyId} {Role})";

        #endregion

    }

}
