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

namespace cloud.charging.open.protocols.OCPI
{

    /// <summary>
    /// The remote access information.
    /// </summary>
    public sealed class RemoteAccessInfo : IEquatable<RemoteAccessInfo>,
                                           IComparable<RemoteAccessInfo>,
                                           IComparable
    {

        #region Properties

        /// <summary>
        /// The access token for accessing the remote party.
        /// </summary>
        [Mandatory]
        public AccessToken              AccessToken                   { get; }

        /// <summary>
        /// The versions URL of the remote party.
        /// </summary>
        [Mandatory]
        public URL                      VersionsURL                   { get; }

        /// <summary>
        /// All available OCPI versions of the remote party.
        /// </summary>
        [Optional]
        public IEnumerable<Version_Id>  VersionIds                    { get; }

        /// <summary>
        /// The selected OCPI version at the remote party.
        /// </summary>
        [Optional]
        public Version_Id?              SelectedVersionId             { get; }

        /// <summary>
        /// The remote access status.
        /// </summary>
        [Mandatory]
        public RemoteAccessStatus       Status                        { get; }

        /// <summary>
        /// This remote access information should not be used before this timestamp.
        /// </summary>
        [Optional]
        public DateTime?                NotBefore                     { get; }

        /// <summary>
        /// This remote access information should not be used after this timestamp.
        /// </summary>
        [Optional]
        public DateTime?                NotAfter                      { get; }

        /// <summary>
        /// Whether the access token is base64 encoded or not.
        /// </summary>
        [Mandatory]
        public Boolean                  AccessTokenIsBase64Encoded    { get; }

        /// <summary>
        /// (Dis-)allow PUTting of object having an earlier 'LastUpdated'-timestamp then already existing objects.
        /// OCPI does not define any behaviour for this.
        /// </summary>
        [Mandatory]
        public Boolean                  AllowDowngrades               { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new remote access information.
        /// </summary>
        /// <param name="AccessToken">A remote access token.</param>
        /// <param name="VersionsURL">An OCPI vesions URL.</param>
        /// <param name="VersionIds">An optional enumeration of version identifications.</param>
        /// <param name="SelectedVersionId">A optional selected version identification.</param>
        /// <param name="Status">A remote access status.</param>
        /// <param name="NotBefore">This remote access information should not be used before this timestamp.</param>
        /// <param name="NotAfter">This remote access information should not be used after this timestamp.</param>
        /// <param name="AccessTokenIsBase64Encoded">Whether the access token is base64 encoded or not.</param>
        /// <param name="AllowDowngrades">(Dis-)allow PUTting of object having an earlier 'LastUpdated'-timestamp then already existing objects.</param>
        public RemoteAccessInfo(AccessToken               AccessToken,
                                URL                       VersionsURL,
                                IEnumerable<Version_Id>?  VersionIds                   = null,
                                Version_Id?               SelectedVersionId            = null,
                                RemoteAccessStatus?       Status                       = RemoteAccessStatus.ONLINE,
                                DateTime?                 NotBefore                    = null,
                                DateTime?                 NotAfter                     = null,
                                Boolean?                  AccessTokenIsBase64Encoded   = false,
                                Boolean?                  AllowDowngrades              = false)
        {

            this.AccessToken                 = AccessToken;
            this.VersionsURL                 = VersionsURL;
            this.VersionIds                  = VersionIds?.Distinct()     ?? Array.Empty<Version_Id>();
            this.SelectedVersionId           = SelectedVersionId;
            this.Status                      = Status                     ?? RemoteAccessStatus.ONLINE;
            this.NotBefore                   = NotBefore;
            this.NotAfter                    = NotAfter;
            this.AccessTokenIsBase64Encoded  = AccessTokenIsBase64Encoded ?? false;
            this.AllowDowngrades             = AllowDowngrades            ?? false;

            unchecked
            {

                this.hashCode = this.AccessToken.               GetHashCode()       * 23 ^
                                this.VersionsURL.               GetHashCode()       * 19 ^
                                this.VersionIds.                CalcHashCode()      * 17 ^
                               (this.SelectedVersionId?.        GetHashCode() ?? 0) * 13 ^
                                this.Status.                    GetHashCode()       * 11 ^
                               (this.NotBefore?.                GetHashCode() ?? 0) *  7 ^
                               (this.NotAfter?.                 GetHashCode() ?? 0) *  5 ^
                                this.AccessTokenIsBase64Encoded.GetHashCode()       *  3 ^
                                this.AllowDowngrades.           GetHashCode();

            }

        }

        #endregion


        #region (static) Parse   (JSON, CustomRemoteAccessInfoParser = null)

        /// <summary>
        /// Parse the given JSON representation of a remote access information.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomRemoteAccessInfoParser">A delegate to parse custom remote access information JSON objects.</param>
        public static RemoteAccessInfo Parse(JObject                                         JSON,
                                             CustomJObjectParserDelegate<RemoteAccessInfo>?  CustomRemoteAccessInfoParser   = null)
        {
            if (TryParse(JSON,
                         out var remoteAccessInfo,
                         out var errorResponse,
                         CustomRemoteAccessInfoParser))
            {
                return remoteAccessInfo!;
            }

            throw new ArgumentException("The given JSON representation of a remote access information is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out RemoteAccessInfo, out ErrorResponse, CustomRemoteAccessInfoParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a remote access information.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="RemoteAccessInfo">The parsed remote access information.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                JSON,
                                       out RemoteAccessInfo?  RemoteAccessInfo,
                                       out String?            ErrorResponse)

            => TryParse(JSON,
                        out RemoteAccessInfo,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a remote access information.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="RemoteAccessInfo">The parsed remote access information.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomRemoteAccessInfoParser">A delegate to parse custom remote access information JSON objects.</param>
        public static Boolean TryParse(JObject                                         JSON,
                                       out RemoteAccessInfo?                           RemoteAccessInfo,
                                       out String?                                     ErrorResponse,
                                       CustomJObjectParserDelegate<RemoteAccessInfo>?  CustomRemoteAccessInfoParser   = null)
        {

            try
            {

                RemoteAccessInfo = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse AccessToken                   [mandatory]

                if (!JSON.ParseMandatory("accessToken",
                                         "access token",
                                         OCPI.AccessToken.TryParse,
                                         out AccessToken AccessToken,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse VersionsURL                   [mandatory]

                if (!JSON.ParseMandatory("versionsURL",
                                         "versions URL",
                                         URL.TryParse,
                                         out URL VersionsURL,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                #region Parse SelectedVersionId             [optional]

                if (JSON.ParseOptional("selectedVersionId",
                                       "selected version identification",
                                       Version_Id.TryParse,
                                       out Version_Id? SelectedVersionId,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse Status                        [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "remote access status",
                                         RemoteAccessStatus.TryParse,
                                         out RemoteAccessStatus Status,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse NotBefore                     [optional]

                if (JSON.ParseOptional("notBefore",
                                       "not before",
                                       out DateTime? NotBefore,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse NotAfter                      [optional]

                if (JSON.ParseOptional("notAfter",
                                       "not after",
                                       out DateTime? NotAfter,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse AccessTokenIsBase64Encoded    [mandatory]

                if (!JSON.ParseMandatory("accessTokenIsBase64Encoded",
                                         "access token is base64 encoded",
                                         out Boolean AccessTokenIsBase64Encoded,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse AllowDowngrades               [mandatory]

                if (!JSON.ParseMandatory("allowDowngrades",
                                         "allow downgrades",
                                         out Boolean AllowDowngrades,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                RemoteAccessInfo = new RemoteAccessInfo(AccessToken,
                                                        VersionsURL,
                                                        null,
                                                        SelectedVersionId,
                                                        Status,
                                                        NotBefore,
                                                        NotAfter,
                                                        AccessTokenIsBase64Encoded,
                                                        AllowDowngrades);


                if (CustomRemoteAccessInfoParser is not null)
                    RemoteAccessInfo = CustomRemoteAccessInfoParser(JSON,
                                                          RemoteAccessInfo);

                return true;

            }
            catch (Exception e)
            {
                RemoteAccessInfo  = default;
                ErrorResponse     = "The given JSON representation of a remote access information is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomBusinessDetailsSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomRemoteAccessInfoSerializer">A delegate to serialize custom remote access information JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<RemoteAccessInfo>?  CustomRemoteAccessInfoSerializer   = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("accessToken",                  AccessToken.      ToString()),
                                 new JProperty("versionsURL",                  VersionsURL.      ToString()),

                           VersionIds.IsNeitherNullNorEmpty()
                               ? new JProperty("versionIds",                   new JArray(VersionIds.Select(versionId => versionId.ToString())))
                               : null,

                           SelectedVersionId.HasValue
                               ? new JProperty("selectedVersionId",            SelectedVersionId.ToString())
                               : null,

                                 new JProperty("status",                       Status.           ToString()),

                           NotBefore.HasValue
                               ? new JProperty("notBefore",                    NotBefore.Value.ToIso8601())
                               : null,

                           NotAfter.HasValue
                               ? new JProperty("notAfter",                     NotAfter. Value.ToIso8601())
                               : null,

                                 new JProperty("accessTokenIsBase64Encoded",   AccessTokenIsBase64Encoded),
                                 new JProperty("allowDowngrades",              AllowDowngrades)

                       );

            return CustomRemoteAccessInfoSerializer is not null
                       ? CustomRemoteAccessInfoSerializer(this, json)
                       : json;

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
                   Status,
                   NotBefore,
                   NotAfter,
                   AccessTokenIsBase64Encoded,
                   AllowDowngrades);

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

            var c = AccessToken.               CompareTo(RemoteAccessInfo.AccessToken);

            if (c == 0)
                c = VersionsURL.               CompareTo(RemoteAccessInfo.VersionsURL);

            if (c == 0)
                c = Status.                    CompareTo(RemoteAccessInfo.Status);

            if (c == 0)
                c = AccessTokenIsBase64Encoded.CompareTo(RemoteAccessInfo.AccessTokenIsBase64Encoded);

            if (c == 0)
                c = AllowDowngrades.           CompareTo(RemoteAccessInfo.AllowDowngrades);

            if (c == 0 && NotBefore.HasValue && RemoteAccessInfo.NotBefore.HasValue)
                c = NotBefore.        Value.   CompareTo(RemoteAccessInfo.NotBefore.        Value);

            if (c == 0 && NotAfter. HasValue && RemoteAccessInfo.NotAfter. HasValue)
                c = NotAfter.         Value.   CompareTo(RemoteAccessInfo.NotAfter.         Value);

            if (c == 0 && SelectedVersionId.HasValue && RemoteAccessInfo.SelectedVersionId.HasValue)
                c = SelectedVersionId.Value.   CompareTo(RemoteAccessInfo.SelectedVersionId.Value);

            if (c == 0)
                c = VersionIds.Count().        CompareTo(RemoteAccessInfo.VersionIds.Count());

            if (c == 0)
                c = VersionIds.OrderBy(versionId => versionId.ToString()).AggregateWith(",").CompareTo(RemoteAccessInfo.VersionIds.OrderBy(versionId => versionId.ToString()).AggregateWith(","));

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

               AccessToken.               Equals(RemoteAccessInfo.AccessToken)                &&
               VersionsURL.               Equals(RemoteAccessInfo.VersionsURL)                &&
               Status.                    Equals(RemoteAccessInfo.Status)                     &&
               AccessTokenIsBase64Encoded.Equals(RemoteAccessInfo.AccessTokenIsBase64Encoded) &&
               AllowDowngrades.           Equals(RemoteAccessInfo.AllowDowngrades)            &&

            ((!NotBefore.        HasValue && !RemoteAccessInfo.NotBefore.        HasValue) ||
              (NotBefore.        HasValue &&  RemoteAccessInfo.NotBefore.        HasValue && NotBefore.        Value.Equals(RemoteAccessInfo.NotBefore.        Value))) &&

            ((!NotAfter.         HasValue && !RemoteAccessInfo.NotAfter.         HasValue) ||
              (NotAfter.         HasValue &&  RemoteAccessInfo.NotAfter.         HasValue && NotAfter.         Value.Equals(RemoteAccessInfo.NotAfter.         Value))) &&

            ((!SelectedVersionId.HasValue && !RemoteAccessInfo.SelectedVersionId.HasValue) ||
              (SelectedVersionId.HasValue &&  RemoteAccessInfo.SelectedVersionId.HasValue && SelectedVersionId.Value.Equals(RemoteAccessInfo.SelectedVersionId.Value))) &&

               VersionIds.Count().Equals(RemoteAccessInfo.VersionIds.Count()) &&
               VersionIds.All(RemoteAccessInfo.VersionIds.Contains);

        #endregion

        #endregion

        #region (override) GetHashCode()

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

            => $"{AccessToken}{(AccessTokenIsBase64Encoded ? "[base64}" : "")} @ {VersionsURL} {Status}";

        #endregion


    }

}
