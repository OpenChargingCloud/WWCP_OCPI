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

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_3_0
{

    public static class AccessInfo2Extensions
    {

        public static Boolean Is(this LocalAccessInfo2?  AccessInfo,
                                 Role                    Role)

            => AccessInfo?.Is(Role) == true;

        public static Boolean IsNot(this LocalAccessInfo2?  AccessInfo,
                                    Role                    Role)

            => AccessInfo is null ||
               AccessInfo.IsNot(Role);

    }


    /// <summary>
    /// Local Access information.
    /// </summary>
    public sealed class LocalAccessInfo2 : IEquatable<LocalAccessInfo2>,
                                           IComparable<LocalAccessInfo2>,
                                           IComparable
    {

        #region Properties

        /// <summary>
        /// The access token.
        /// </summary>
        [Mandatory]
        public AccessToken                   AccessToken                   { get; }

        /// <summary>
        /// The access status.
        /// </summary>
        [Mandatory]
        public AccessStatus                  Status                        { get; }

        /// <summary>
        /// This local access information should not be used before this timestamp.
        /// </summary>
        [Optional]
        public DateTimeOffset?               NotBefore                     { get; }

        /// <summary>
        /// This local access information should not be used after this timestamp.
        /// </summary>
        [Optional]
        public DateTimeOffset?               NotAfter                      { get; }

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

        /// <summary>
        /// Whether the access token is base64 encoded or not.
        /// </summary>
        [Mandatory]
        public Boolean                       AccessTokenIsBase64Encoded    { get; }

        /// <summary>
        /// (Dis-)allow PUTting of object having an earlier 'LastUpdated'-timestamp then already existing objects.
        /// OCPI does not define any behaviour for this.
        /// </summary>
        [Mandatory]
        public Boolean                       AllowDowngrades               { get; }


        //public Boolean Is(Role Role)
        //    =>  Roles.Any(role => role.Role == Role);

        //public Boolean IsNot(Role Role)
        //    => !Roles.Any(role => role.Role == Role);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new Local access information.
        /// </summary>
        /// <param name="AccessToken">An access token.</param>
        /// <param name="Status">An access status.</param>
        /// <param name="Roles">an enumeration of credential roles.</param>
        /// <param name="NotBefore">This local access information should not be used before this timestamp.</param>
        /// <param name="NotAfter">This local access information should not be used after this timestamp.</param>
        /// <param name="VersionsURL">An optional URL to get the remote OCPI versions information.</param>
        /// <param name="AccessTokenIsBase64Encoded">Whether the access token is base64 encoded or not.</param>
        public LocalAccessInfo2(AccessToken                    AccessToken,
                               AccessStatus                   Status,
                               IEnumerable<CredentialsRole>?  Roles                        = null,
                               DateTimeOffset?                NotBefore                    = null,
                               DateTimeOffset?                NotAfter                     = null,
                               URL?                           VersionsURL                  = null,
                               Boolean?                       AccessTokenIsBase64Encoded   = false,
                               Boolean?                       AllowDowngrades              = false)
        {

            this.AccessToken                 = AccessToken;
            this.Status                      = Status;
            this.Roles                       = Roles?.Distinct()          ?? Array.Empty<CredentialsRole>();
            this.NotBefore                   = NotBefore;
            this.NotAfter                    = NotAfter;
            this.VersionsURL                 = VersionsURL;
            this.AccessTokenIsBase64Encoded  = AccessTokenIsBase64Encoded ?? false;
            this.AllowDowngrades             = AllowDowngrades            ?? false;

            unchecked
            {

                this.hashCode = this.AccessToken.               GetHashCode()       * 19 ^
                                this.Status.                    GetHashCode()       * 17 ^
                                this.Roles.                     CalcHashCode()      * 13 ^
                               (this.NotBefore?.                GetHashCode() ?? 0) * 11 ^
                               (this.NotAfter?.                 GetHashCode() ?? 0) *  7 ^
                               (this.VersionsURL?.              GetHashCode() ?? 0) *  5 ^
                                this.AccessTokenIsBase64Encoded.GetHashCode()       *  3 ^
                                this.AllowDowngrades.           GetHashCode();

            }

        }

        #endregion


        #region (static) Parse   (JSON, CustomLocalAccessInfo2Parser = null)

        /// <summary>
        /// Parse the given JSON representation of a local access information.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomLocalAccessInfo2Parser">A delegate to parse custom local access information JSON objects.</param>
        public static LocalAccessInfo2 Parse(JObject                                        JSON,
                                            CustomJObjectParserDelegate<LocalAccessInfo2>?  CustomLocalAccessInfo2Parser   = null)
        {
            if (TryParse(JSON,
                         out var localAccessInfo,
                         out var errorResponse,
                         CustomLocalAccessInfo2Parser))
            {
                return localAccessInfo;
            }

            throw new ArgumentException("The given JSON representation of a local access information is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out LocalAccessInfo2, out ErrorResponse, CustomLocalAccessInfo2Parser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a local access information.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="LocalAccessInfo2">The parsed local access information.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                     JSON,
                                       [NotNullWhen(true)]  out LocalAccessInfo2?  LocalAccessInfo2,
                                       [NotNullWhen(false)] out String?            ErrorResponse)

            => TryParse(JSON,
                        out LocalAccessInfo2,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a local access information.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="LocalAccessInfo2">The parsed local access information.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomLocalAccessInfo2Parser">A delegate to parse custom local access information JSON objects.</param>
        public static Boolean TryParse(JObject                                         JSON,
                                       [NotNullWhen(true)]  out LocalAccessInfo2?      LocalAccessInfo2,
                                       [NotNullWhen(false)] out String?                ErrorResponse,
                                       CustomJObjectParserDelegate<LocalAccessInfo2>?  CustomLocalAccessInfo2Parser   = null)
        {

            try
            {

                LocalAccessInfo2 = default;

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

                #region Parse CredentialsRoles              [mandatory]

                if (!JSON.ParseMandatoryHashSet("businessDetails",
                                                "business details",
                                                CredentialsRole.TryParse,
                                                out HashSet<CredentialsRole> CredentialsRoles,
                                                out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse NotBefore                     [optional]

                if (JSON.ParseOptional("notBefore",
                                       "not before",
                                       out DateTimeOffset? NotBefore,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse NotAfter                      [optional]

                if (JSON.ParseOptional("notAfter",
                                       "not after",
                                       out DateTimeOffset? NotAfter,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
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


                LocalAccessInfo2 = new LocalAccessInfo2(AccessToken,
                                                      Status,
                                                      CredentialsRoles,
                                                      NotBefore,
                                                      NotAfter,
                                                      VersionsURL,
                                                      AccessTokenIsBase64Encoded,
                                                      AllowDowngrades);


                if (CustomLocalAccessInfo2Parser is not null)
                    LocalAccessInfo2 = CustomLocalAccessInfo2Parser(JSON,
                                                          LocalAccessInfo2);

                return true;

            }
            catch (Exception e)
            {
                LocalAccessInfo2    = default;
                ErrorResponse  = "The given JSON representation of a local access information is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomLocalAccessInfo2Serializer = null, CustomCredentialsRoleSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomLocalAccessInfo2Serializer">A delegate to serialize custom local access information JSON objects.</param>
        /// <param name="CustomCredentialsRoleSerializer">A delegate to serialize custom credentials role JSON objects.</param>
        /// <param name="CustomBusinessDetailsSerializer">A delegate to serialize custom business details JSON objects.</param>
        /// <param name="CustomImageSerializer">A delegate to serialize custom image JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<LocalAccessInfo2>?  CustomLocalAccessInfo2Serializer   = null,
                              CustomJObjectSerializerDelegate<CredentialsRole>?  CustomCredentialsRoleSerializer   = null,
                              CustomJObjectSerializerDelegate<BusinessDetails>?  CustomBusinessDetailsSerializer   = null,
                              CustomJObjectSerializerDelegate<Image>?            CustomImageSerializer             = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("accessToken",                  AccessToken.ToString()),
                                 new JProperty("status",                       Status.     ToString()),
                                 new JProperty("roles",                        new JArray(Roles.Select(role => role.ToJSON(CustomCredentialsRoleSerializer,
                                                                                                                           CustomBusinessDetailsSerializer,
                                                                                                                           CustomImageSerializer)))),

                           NotBefore.HasValue
                               ? new JProperty("notBefore",                    NotBefore.Value.ToISO8601())
                               : null,

                           NotAfter.HasValue
                               ? new JProperty("notAfter",                     NotAfter. Value.ToISO8601())
                               : null,

                           VersionsURL.HasValue
                               ? new JProperty("versionsURL",                  VersionsURL.ToString())
                               : null,

                                 new JProperty("accessTokenIsBase64Encoded",   AccessTokenIsBase64Encoded),
                                 new JProperty("allowDowngrades",              AllowDowngrades)

                       );

            return CustomLocalAccessInfo2Serializer is not null
                       ? CustomLocalAccessInfo2Serializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this object.
        /// </summary>
        public LocalAccessInfo2 Clone()

            => new(AccessToken,
                   Status,
                   Roles.Select(credentialsRole => credentialsRole.Clone()).ToArray(),
                   NotBefore,
                   NotAfter,
                   VersionsURL,
                   AccessTokenIsBase64Encoded,
                   AllowDowngrades);

        #endregion


        #region Operator overloading

        #region Operator == (LocalAccessInfo21, LocalAccessInfo22)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LocalAccessInfo21">A local access information.</param>
        /// <param name="LocalAccessInfo22">Another local access information.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (LocalAccessInfo2 LocalAccessInfo21,
                                           LocalAccessInfo2 LocalAccessInfo22)
        {

            if (Object.ReferenceEquals(LocalAccessInfo21, LocalAccessInfo22))
                return true;

            if ((LocalAccessInfo21 is null) || (LocalAccessInfo22 is null))
                return false;

            return LocalAccessInfo21.Equals(LocalAccessInfo22);

        }

        #endregion

        #region Operator != (LocalAccessInfo21, LocalAccessInfo22)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LocalAccessInfo21">A local access information.</param>
        /// <param name="LocalAccessInfo22">Another local access information.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (LocalAccessInfo2 LocalAccessInfo21,
                                           LocalAccessInfo2 LocalAccessInfo22)

            => !(LocalAccessInfo21 == LocalAccessInfo22);

        #endregion

        #region Operator <  (LocalAccessInfo21, LocalAccessInfo22)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LocalAccessInfo21">A local access information.</param>
        /// <param name="LocalAccessInfo22">Another local access information.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (LocalAccessInfo2 LocalAccessInfo21,
                                          LocalAccessInfo2 LocalAccessInfo22)

            => LocalAccessInfo21 is null
                   ? throw new ArgumentNullException(nameof(LocalAccessInfo21), "The given local access information must not be null!")
                   : LocalAccessInfo21.CompareTo(LocalAccessInfo22) < 0;

        #endregion

        #region Operator <= (LocalAccessInfo21, LocalAccessInfo22)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LocalAccessInfo21">A local access information.</param>
        /// <param name="LocalAccessInfo22">Another local access information.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (LocalAccessInfo2 LocalAccessInfo21,
                                           LocalAccessInfo2 LocalAccessInfo22)

            => !(LocalAccessInfo21 > LocalAccessInfo22);

        #endregion

        #region Operator >  (LocalAccessInfo21, LocalAccessInfo22)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LocalAccessInfo21">A local access information.</param>
        /// <param name="LocalAccessInfo22">Another local access information.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (LocalAccessInfo2 LocalAccessInfo21,
                                          LocalAccessInfo2 LocalAccessInfo22)

            => LocalAccessInfo21 is null
                   ? throw new ArgumentNullException(nameof(LocalAccessInfo21), "The given local access information must not be null!")
                   : LocalAccessInfo21.CompareTo(LocalAccessInfo22) > 0;

        #endregion

        #region Operator >= (LocalAccessInfo21, LocalAccessInfo22)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LocalAccessInfo21">A local access information.</param>
        /// <param name="LocalAccessInfo22">Another local access information.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (LocalAccessInfo2 LocalAccessInfo21,
                                           LocalAccessInfo2 LocalAccessInfo22)

            => !(LocalAccessInfo21 < LocalAccessInfo22);

        #endregion

        #endregion

        #region IComparable<LocalAccessInfo2> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two access information.
        /// </summary>
        /// <param name="Object">An access information to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is LocalAccessInfo2 localAccessInfo
                   ? CompareTo(localAccessInfo)
                   : throw new ArgumentException("The given object is not a local access information!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(LocalAccessInfo2)

        /// <summary>
        /// Compares two access information.
        /// </summary>
        /// <param name="LocalAccessInfo2">An access information to compare with.</param>
        public Int32 CompareTo(LocalAccessInfo2? LocalAccessInfo2)
        {

            if (LocalAccessInfo2 is null)
                throw new ArgumentNullException(nameof(LocalAccessInfo2), "The given local access information must not be null!");

            var c = AccessToken.               CompareTo(LocalAccessInfo2.AccessToken);

            if (c == 0)
                c = Status.                    CompareTo(LocalAccessInfo2.Status);

            if (c == 0)
                c = Roles.Count().             CompareTo(LocalAccessInfo2.Roles.Count());

            if (c == 0 && NotBefore.HasValue && LocalAccessInfo2.NotBefore.HasValue)
                c = NotBefore.  Value.         CompareTo(LocalAccessInfo2.NotBefore.  Value);

            if (c == 0 && NotAfter. HasValue && LocalAccessInfo2.NotAfter. HasValue)
                c = NotAfter.   Value.         CompareTo(LocalAccessInfo2.NotAfter.   Value);

            if (c == 0)
            {
                for (var i = 0; i < Roles.Count(); i++)
                {

                    c = Roles.ElementAt(i).CompareTo(LocalAccessInfo2.Roles.ElementAt(i));

                    if (c != 0)
                        break;

                }
            }

            if (c == 0 && VersionsURL.HasValue && LocalAccessInfo2.VersionsURL.HasValue)
                c = VersionsURL.Value.         CompareTo(LocalAccessInfo2.VersionsURL.Value);

            if (c == 0)
                c = AccessTokenIsBase64Encoded.CompareTo(LocalAccessInfo2.AccessTokenIsBase64Encoded);

            if (c == 0)
                c = AllowDowngrades.           CompareTo(LocalAccessInfo2.AllowDowngrades);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<LocalAccessInfo2> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two access information for equality.
        /// </summary>
        /// <param name="Object">An access information to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is LocalAccessInfo2 localAccessInfo &&
                   Equals(localAccessInfo);

        #endregion

        #region Equals(LocalAccessInfo2)

        /// <summary>
        /// Compares two access information for equality.
        /// </summary>
        /// <param name="LocalAccessInfo2">An access information to compare with.</param>
        public Boolean Equals(LocalAccessInfo2? LocalAccessInfo2)

            => LocalAccessInfo2 is not null &&

               AccessToken.               Equals(LocalAccessInfo2.AccessToken)   &&
               Status.                    Equals(LocalAccessInfo2.Status)        &&

               Roles.Count().             Equals(LocalAccessInfo2.Roles.Count()) &&
               Roles.All(LocalAccessInfo2.Roles.Contains) &&

            ((!NotBefore.  HasValue && !LocalAccessInfo2.NotBefore.  HasValue) ||
              (NotBefore.  HasValue &&  LocalAccessInfo2.NotBefore.  HasValue    && NotBefore.  Value.Equals(LocalAccessInfo2.NotBefore.  Value))) &&

            ((!NotAfter.   HasValue && !LocalAccessInfo2.NotAfter.   HasValue) ||
              (NotAfter.   HasValue &&  LocalAccessInfo2.NotAfter.   HasValue    && NotAfter.   Value.Equals(LocalAccessInfo2.NotAfter.   Value))) &&

            ((!VersionsURL.HasValue && !LocalAccessInfo2.VersionsURL.HasValue) ||
              (VersionsURL.HasValue &&  LocalAccessInfo2.VersionsURL.HasValue    && VersionsURL.Value.Equals(LocalAccessInfo2.VersionsURL.Value))) &&

               AccessTokenIsBase64Encoded.Equals(LocalAccessInfo2.AccessTokenIsBase64Encoded) &&
               AllowDowngrades.           Equals(LocalAccessInfo2.AllowDowngrades);

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

            //=> $"{AccessToken}{(AccessTokenBase64Encoding ? "[base64}" : "")} @ {VersionsURL} {Status}";
            => $"'{AccessToken}' {Status} @ '{VersionsURL}'";
            //=> $"{AccessToken}{(AccessTokenIsBase64Encoded ? "[base64}" : "")} for {BusinessDetails.Name} ({CountryCode}{PartyId} {Role})";

        #endregion


    }

}
