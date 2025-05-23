﻿/*
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

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPI
{

    /// <summary>
    /// Local Access information.
    /// </summary>
    public sealed class LocalAccessInfo : IEquatable<LocalAccessInfo>,
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
        /// The access status.
        /// </summary>
        [Mandatory]
        public AccessStatus      Status                        { get; }

        /// <summary>
        /// This local access information should not be used before this timestamp.
        /// </summary>
        [Optional]
        public DateTime?         NotBefore                     { get; }

        /// <summary>
        /// This local access information should not be used after this timestamp.
        /// </summary>
        [Optional]
        public DateTime?         NotAfter                      { get; }

        /// <summary>
        /// Whether the access token is base64 encoded or not.
        /// </summary>
        [Mandatory]
        public Boolean           AccessTokenIsBase64Encoded    { get; }

        /// <summary>
        /// (Dis-)allow PUTting of object having an earlier 'LastUpdated'-timestamp then already existing objects.
        /// OCPI does not define any behaviour for this.
        /// </summary>
        [Mandatory]
        public Boolean           AllowDowngrades               { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new local access information.
        /// </summary>
        /// <param name="AccessToken">An access token.</param>
        /// <param name="Status">An access status.</param>
        /// <param name="NotBefore">This local access information should not be used before this timestamp.</param>
        /// <param name="NotAfter">This local access information should not be used after this timestamp.</param>
        /// <param name="AccessTokenIsBase64Encoded">Whether the access token is base64 encoded or not.</param>
        /// <param name="AllowDowngrades">(Dis-)allow PUTting of object having an earlier 'LastUpdated'-timestamp then already existing objects.</param>
        public LocalAccessInfo(AccessToken       AccessToken,
                               AccessStatus      Status,
                               DateTime?         NotBefore                    = null,
                               DateTime?         NotAfter                     = null,
                               Boolean?          AccessTokenIsBase64Encoded   = true,
                               Boolean?          AllowDowngrades              = false)
        {

            this.AccessToken                 = AccessToken;
            this.Status                      = Status;
            this.NotBefore                   = NotBefore;
            this.NotAfter                    = NotAfter;
            this.AccessTokenIsBase64Encoded  = AccessTokenIsBase64Encoded ?? true;
            this.AllowDowngrades             = AllowDowngrades            ?? false;

            unchecked
            {

                this.hashCode = this.AccessToken.               GetHashCode()       * 29 ^
                                this.Status.                    GetHashCode()       * 27 ^
                               (this.NotBefore?.                GetHashCode() ?? 0) * 11 ^
                               (this.NotAfter?.                 GetHashCode() ?? 0) *  7 ^
                                this.AccessTokenIsBase64Encoded.GetHashCode()       *  3 ^
                                this.AllowDowngrades.           GetHashCode();

            }

        }

        #endregion


        #region (static) Parse   (JSON, CustomLocalAccessInfoParser = null)

        /// <summary>
        /// Parse the given JSON representation of a local access information.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomLocalAccessInfoParser">A delegate to parse custom local access information JSON objects.</param>
        public static LocalAccessInfo Parse(JObject                                        JSON,
                                            CustomJObjectParserDelegate<LocalAccessInfo>?  CustomLocalAccessInfoParser   = null)
        {
            if (TryParse(JSON,
                         out var localAccessInfo,
                         out var errorResponse,
                         CustomLocalAccessInfoParser))
            {
                return localAccessInfo;
            }

            throw new ArgumentException("The given JSON representation of a local access information is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out LocalAccessInfo, out ErrorResponse, CustomLocalAccessInfoParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a local access information.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="LocalAccessInfo">The parsed local access information.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                    JSON,
                                       [NotNullWhen(true)]  out LocalAccessInfo?  LocalAccessInfo,
                                       [NotNullWhen(false)] out String?           ErrorResponse)

            => TryParse(JSON,
                        out LocalAccessInfo,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a local access information.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="LocalAccessInfo">The parsed local access information.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomLocalAccessInfoParser">A delegate to parse custom local access information JSON objects.</param>
        public static Boolean TryParse(JObject                                        JSON,
                                       [NotNullWhen(true)]  out LocalAccessInfo?      LocalAccessInfo,
                                       [NotNullWhen(false)] out String?               ErrorResponse,
                                       CustomJObjectParserDelegate<LocalAccessInfo>?  CustomLocalAccessInfoParser   = null)
        {

            try
            {

                LocalAccessInfo = default;

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

                #region Parse Status                        [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "access status",
                                         AccessStatus.TryParse,
                                         out AccessStatus Status,
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


                LocalAccessInfo = new LocalAccessInfo(
                                      AccessToken,
                                      Status,
                                      NotBefore,
                                      NotAfter,
                                      AccessTokenIsBase64Encoded,
                                      AllowDowngrades
                                  );


                if (CustomLocalAccessInfoParser is not null)
                    LocalAccessInfo = CustomLocalAccessInfoParser(JSON,
                                                                  LocalAccessInfo);

                return true;

            }
            catch (Exception e)
            {
                LocalAccessInfo  = default;
                ErrorResponse    = "The given JSON representation of a local access information is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomLocalAccessInfoSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomLocalAccessInfoSerializer">A delegate to serialize custom local access information JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<LocalAccessInfo>? CustomLocalAccessInfoSerializer = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("accessToken",                  AccessToken.    ToString()),
                                 new JProperty("status",                       Status.         ToString()),

                           NotBefore.HasValue
                               ? new JProperty("notBefore",                    NotBefore.Value.ToISO8601())
                               : null,

                           NotAfter.HasValue
                               ? new JProperty("notAfter",                     NotAfter. Value.ToISO8601())
                               : null,

                                 new JProperty("accessTokenIsBase64Encoded",   AccessTokenIsBase64Encoded),
                                 new JProperty("allowDowngrades",              AllowDowngrades)

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
                   NotBefore,
                   NotAfter,
                   AccessTokenIsBase64Encoded,
                   AllowDowngrades);

        #endregion


        #region Operator overloading

        #region Operator == (LocalAccessInfo1, LocalAccessInfo2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LocalAccessInfo1">A local access information.</param>
        /// <param name="LocalAccessInfo2">Another local access information.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (LocalAccessInfo LocalAccessInfo1,
                                           LocalAccessInfo LocalAccessInfo2)
        {

            if (Object.ReferenceEquals(LocalAccessInfo1, LocalAccessInfo2))
                return true;

            if ((LocalAccessInfo1 is null) || (LocalAccessInfo2 is null))
                return false;

            return LocalAccessInfo1.Equals(LocalAccessInfo2);

        }

        #endregion

        #region Operator != (LocalAccessInfo1, LocalAccessInfo2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LocalAccessInfo1">A local access information.</param>
        /// <param name="LocalAccessInfo2">Another local access information.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (LocalAccessInfo LocalAccessInfo1,
                                           LocalAccessInfo LocalAccessInfo2)

            => !(LocalAccessInfo1 == LocalAccessInfo2);

        #endregion

        #region Operator <  (LocalAccessInfo1, LocalAccessInfo2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LocalAccessInfo1">A local access information.</param>
        /// <param name="LocalAccessInfo2">Another local access information.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (LocalAccessInfo LocalAccessInfo1,
                                          LocalAccessInfo LocalAccessInfo2)

            => LocalAccessInfo1 is null
                   ? throw new ArgumentNullException(nameof(LocalAccessInfo1), "The given local access information must not be null!")
                   : LocalAccessInfo1.CompareTo(LocalAccessInfo2) < 0;

        #endregion

        #region Operator <= (LocalAccessInfo1, LocalAccessInfo2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LocalAccessInfo1">A local access information.</param>
        /// <param name="LocalAccessInfo2">Another local access information.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (LocalAccessInfo LocalAccessInfo1,
                                           LocalAccessInfo LocalAccessInfo2)

            => !(LocalAccessInfo1 > LocalAccessInfo2);

        #endregion

        #region Operator >  (LocalAccessInfo1, LocalAccessInfo2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LocalAccessInfo1">A local access information.</param>
        /// <param name="LocalAccessInfo2">Another local access information.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (LocalAccessInfo LocalAccessInfo1,
                                          LocalAccessInfo LocalAccessInfo2)

            => LocalAccessInfo1 is null
                   ? throw new ArgumentNullException(nameof(LocalAccessInfo1), "The given local access information must not be null!")
                   : LocalAccessInfo1.CompareTo(LocalAccessInfo2) > 0;

        #endregion

        #region Operator >= (LocalAccessInfo1, LocalAccessInfo2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LocalAccessInfo1">A local access information.</param>
        /// <param name="LocalAccessInfo2">Another local access information.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (LocalAccessInfo LocalAccessInfo1,
                                           LocalAccessInfo LocalAccessInfo2)

            => !(LocalAccessInfo1 < LocalAccessInfo2);

        #endregion

        #endregion

        #region IComparable<LocalAccessInfo> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two access information.
        /// </summary>
        /// <param name="Object">An access information to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is LocalAccessInfo localAccessInfo
                   ? CompareTo(localAccessInfo)
                   : throw new ArgumentException("The given object is not a local access information!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(LocalAccessInfo)

        /// <summary>
        /// Compares two access information.
        /// </summary>
        /// <param name="LocalAccessInfo">An access information to compare with.</param>
        public Int32 CompareTo(LocalAccessInfo? LocalAccessInfo)
        {

            if (LocalAccessInfo is null)
                throw new ArgumentNullException(nameof(LocalAccessInfo), "The given local access information must not be null!");

            var c = AccessToken.               CompareTo(LocalAccessInfo.AccessToken);

            if (c == 0)
                c = Status.                    CompareTo(LocalAccessInfo.Status);

            if (c == 0 && NotBefore.HasValue && LocalAccessInfo.NotBefore.HasValue)
                c = NotBefore.  Value.         CompareTo(LocalAccessInfo.NotBefore.  Value);

            if (c == 0 && NotAfter. HasValue && LocalAccessInfo.NotAfter. HasValue)
                c = NotAfter.   Value.         CompareTo(LocalAccessInfo.NotAfter.   Value);

            if (c == 0)
                c = AccessTokenIsBase64Encoded.CompareTo(LocalAccessInfo.AccessTokenIsBase64Encoded);

            if (c == 0)
                c = AllowDowngrades.           CompareTo(LocalAccessInfo.AllowDowngrades);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<LocalAccessInfo> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two access information for equality.
        /// </summary>
        /// <param name="Object">An access information to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is LocalAccessInfo localAccessInfo &&
                   Equals(localAccessInfo);

        #endregion

        #region Equals(LocalAccessInfo)

        /// <summary>
        /// Compares two access information for equality.
        /// </summary>
        /// <param name="LocalAccessInfo">An access information to compare with.</param>
        public Boolean Equals(LocalAccessInfo? LocalAccessInfo)

            => LocalAccessInfo is not null &&

               AccessToken.               Equals(LocalAccessInfo.AccessToken)                &&
               Status.                    Equals(LocalAccessInfo.Status)                     &&
               AccessTokenIsBase64Encoded.Equals(LocalAccessInfo.AccessTokenIsBase64Encoded) &&
               AllowDowngrades.           Equals(LocalAccessInfo.AllowDowngrades)            &&

            ((!NotBefore.HasValue && !LocalAccessInfo.NotBefore.HasValue) ||
              (NotBefore.HasValue &&  LocalAccessInfo.NotBefore.HasValue && NotBefore.Value.Equals(LocalAccessInfo.NotBefore.Value))) &&

            ((!NotAfter. HasValue && !LocalAccessInfo.NotAfter. HasValue) ||
              (NotAfter. HasValue &&  LocalAccessInfo.NotAfter. HasValue && NotAfter. Value.Equals(LocalAccessInfo.NotAfter. Value)));

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

            => $"'{AccessToken}' ({Status}) {(AllowDowngrades ? "[Downgrades allowed]" : "[No downgrades]")}";

        #endregion


    }

}
