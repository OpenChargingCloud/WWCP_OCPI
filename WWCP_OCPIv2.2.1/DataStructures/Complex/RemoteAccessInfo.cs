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

    /// <summary>
    /// The remote access information.
    /// </summary>
    public class RemoteAccessInfo : IEquatable<RemoteAccessInfo>,
                                    IComparable<RemoteAccessInfo>,
                                    IComparable
    {

        #region Properties

        [Mandatory]
        public AccessToken              AccessToken                  { get; }

        [Mandatory]
        public URL                      VersionsURL                  { get; }

        [Optional]
        public IEnumerable<Version_Id>  VersionIds                   { get; }

        [Optional]
        public Version_Id?              SelectedVersionId            { get; }

        [Mandatory]
        public Boolean                  AccessTokenBase64Encoding    { get; }

        [Mandatory]
        public RemoteAccessStatus       Status                       { get; }

        [Mandatory]
        public DateTime                 LastUpdated                  { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new remote access information.
        /// </summary>
        /// <param name="AccessToken">A remote access token.</param>
        /// <param name="VersionsURL">An OCPI vesions URL.</param>
        /// <param name="VersionIds">An optional enumeration of version identifications.</param>
        /// <param name="SelectedVersionId">A optional selected version identification.</param>
        /// <param name="AccessTokenBase64Encoding">Whether the access token is base64 encoded or not.</param>
        /// <param name="Status">A remote access status.</param>
        /// <param name="LastUpdate">The optional timestamp of the last update.</param>
        public RemoteAccessInfo(AccessToken               AccessToken,
                                URL                       VersionsURL,
                                IEnumerable<Version_Id>?  VersionIds                  = null,
                                Version_Id?               SelectedVersionId           = null,
                                Boolean?                  AccessTokenBase64Encoding   = null,
                                RemoteAccessStatus?       Status                      = RemoteAccessStatus.ONLINE,
                                DateTime?                 LastUpdate                  = null)
        {

            this.AccessToken                = AccessToken;
            this.VersionsURL                = VersionsURL;
            this.VersionIds                 = VersionIds?.Distinct()    ?? Array.Empty<Version_Id>();
            this.SelectedVersionId          = SelectedVersionId;
            this.AccessTokenBase64Encoding  = AccessTokenBase64Encoding ?? true;
            this.Status                     = Status                    ?? RemoteAccessStatus.ONLINE;
            this.LastUpdated                 = LastUpdate                ?? Timestamp.Now;

        }

        #endregion


        #region ToJSON(CustomBusinessDetailsSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomRemoteAccessInfoSerializer">A delegate to serialize custom remote access information JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<RemoteAccessInfo>?  CustomRemoteAccessInfoSerializer   = null)
        {

            var JSON = JSONObject.Create(

                                 new JProperty("token",               AccessToken.      ToString()),
                                 new JProperty("versionsURL",         VersionsURL.      ToString()),

                           VersionIds.IsNeitherNullNorEmpty()
                               ? new JProperty("versionIds",          new JArray(VersionIds.Select(versionId => versionId.ToString())))
                               : null,

                           SelectedVersionId.HasValue
                               ? new JProperty("selectedVersionId",   SelectedVersionId.ToString())
                               : null,

                                 new JProperty("status",              Status.           ToString()),
                                 new JProperty("lastUpdate",          LastUpdated.      ToIso8601())

                       );

            return CustomRemoteAccessInfoSerializer is not null
                       ? CustomRemoteAccessInfoSerializer(this, JSON)
                       : JSON;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this object.
        /// </summary>
        public RemoteAccessInfo Clone()

            => new(AccessToken.Clone,
                   VersionsURL.Clone,
                   VersionIds.Select(versionId => versionId.Clone).ToArray(),
                   SelectedVersionId,
                   AccessTokenBase64Encoding,
                   Status);

        #endregion


        #region Operator overloading

        #region Operator == (RemoteAccessInfo1, RemoteAccessInfo2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RemoteAccessInfo1">A remote access information.</param>
        /// <param name="RemoteAccessInfo2">Another remote access information.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (RemoteAccessInfo RemoteAccessInfo1,
                                           RemoteAccessInfo RemoteAccessInfo2)
        {

            if (Object.ReferenceEquals(RemoteAccessInfo1, RemoteAccessInfo2))
                return true;

            if ((RemoteAccessInfo1 is null) || (RemoteAccessInfo2 is null))
                return false;

            return RemoteAccessInfo1.Equals(RemoteAccessInfo2);

        }

        #endregion

        #region Operator != (RemoteAccessInfo1, RemoteAccessInfo2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RemoteAccessInfo1">A remote access information.</param>
        /// <param name="RemoteAccessInfo2">Another remote access information.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (RemoteAccessInfo RemoteAccessInfo1,
                                           RemoteAccessInfo RemoteAccessInfo2)

            => !(RemoteAccessInfo1 == RemoteAccessInfo2);

        #endregion

        #region Operator <  (RemoteAccessInfo1, RemoteAccessInfo2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RemoteAccessInfo1">A remote access information.</param>
        /// <param name="RemoteAccessInfo2">Another remote access information.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (RemoteAccessInfo RemoteAccessInfo1,
                                          RemoteAccessInfo RemoteAccessInfo2)

            => RemoteAccessInfo1 is null
                   ? throw new ArgumentNullException(nameof(RemoteAccessInfo1), "The given remote access information must not be null!")
                   : RemoteAccessInfo1.CompareTo(RemoteAccessInfo2) < 0;

        #endregion

        #region Operator <= (RemoteAccessInfo1, RemoteAccessInfo2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RemoteAccessInfo1">A remote access information.</param>
        /// <param name="RemoteAccessInfo2">Another remote access information.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (RemoteAccessInfo RemoteAccessInfo1,
                                           RemoteAccessInfo RemoteAccessInfo2)

            => !(RemoteAccessInfo1 > RemoteAccessInfo2);

        #endregion

        #region Operator >  (RemoteAccessInfo1, RemoteAccessInfo2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RemoteAccessInfo1">A remote access information.</param>
        /// <param name="RemoteAccessInfo2">Another remote access information.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (RemoteAccessInfo RemoteAccessInfo1,
                                          RemoteAccessInfo RemoteAccessInfo2)

            => RemoteAccessInfo1 is null
                   ? throw new ArgumentNullException(nameof(RemoteAccessInfo1), "The given remote access information must not be null!")
                   : RemoteAccessInfo1.CompareTo(RemoteAccessInfo2) > 0;

        #endregion

        #region Operator >= (RemoteAccessInfo1, RemoteAccessInfo2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RemoteAccessInfo1">A remote access information.</param>
        /// <param name="RemoteAccessInfo2">Another remote access information.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (RemoteAccessInfo RemoteAccessInfo1,
                                           RemoteAccessInfo RemoteAccessInfo2)

            => !(RemoteAccessInfo1 < RemoteAccessInfo2);

        #endregion

        #endregion

        #region IComparable<RemoteAccessInfo> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two remote access information.
        /// </summary>
        /// <param name="Object">A remote access information to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is RemoteAccessInfo remoteAccessInfo
                   ? CompareTo(remoteAccessInfo)
                   : throw new ArgumentException("The given object is not a remote access information!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(RemoteAccessInfo)

        /// <summary>
        /// Compares two remote access information.
        /// </summary>
        /// <param name="RemoteAccessInfo">A remote access information to compare with.</param>
        public Int32 CompareTo(RemoteAccessInfo? RemoteAccessInfo)
        {

            if (RemoteAccessInfo is null)
                throw new ArgumentNullException(nameof(RemoteAccessInfo), "The given remote access information must not be null!");

            var c = AccessToken.              CompareTo(RemoteAccessInfo.AccessToken);

            if (c == 0)
                c = VersionsURL.              CompareTo(RemoteAccessInfo.VersionsURL);

            if (c == 0)
                c = AccessTokenBase64Encoding.CompareTo(RemoteAccessInfo.AccessTokenBase64Encoding);

            if (c == 0)
                c = Status.                   CompareTo(RemoteAccessInfo.Status);

            if (c == 0)
                c = LastUpdated.              CompareTo(RemoteAccessInfo.LastUpdated);

            if (c == 0 && SelectedVersionId.HasValue && RemoteAccessInfo.SelectedVersionId.HasValue)
                c = SelectedVersionId.Value.  CompareTo(RemoteAccessInfo.SelectedVersionId.Value);

            // VersionIds

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<RemoteAccessInfo> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two remote access information for equality.
        /// </summary>
        /// <param name="Object">A remote access information to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is RemoteAccessInfo remoteAccessInfo &&
                   Equals(remoteAccessInfo);

        #endregion

        #region Equals(RemoteAccessInfo)

        /// <summary>
        /// Compares two remote access information for equality.
        /// </summary>
        /// <param name="RemoteAccessInfo">A remote access information to compare with.</param>
        public Boolean Equals(RemoteAccessInfo? RemoteAccessInfo)

            => RemoteAccessInfo is not null &&

               AccessToken.              Equals(RemoteAccessInfo.AccessToken)               &&
               VersionsURL.              Equals(RemoteAccessInfo.VersionsURL)               &&
               AccessTokenBase64Encoding.Equals(RemoteAccessInfo.AccessTokenBase64Encoding) &&
               Status.                   Equals(RemoteAccessInfo.Status)                    &&
               LastUpdated.              Equals(RemoteAccessInfo.LastUpdated)               &&

            ((!SelectedVersionId.HasValue && !RemoteAccessInfo.SelectedVersionId.HasValue) ||
              (SelectedVersionId.HasValue &&  RemoteAccessInfo.SelectedVersionId.HasValue && SelectedVersionId.Value.Equals(RemoteAccessInfo.SelectedVersionId.Value))) &&

               VersionIds.Count().Equals(RemoteAccessInfo.VersionIds.Count()) &&
               VersionIds.All(RemoteAccessInfo.VersionIds.Contains);

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

            => $"{AccessToken}{(AccessTokenBase64Encoding ? "[base64}" : "")} @ {VersionsURL} {Status}";

        #endregion

    }

}
