/*
 * Copyright (c) 2015-2021 GraphDefined GmbH
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
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_1_1
{

    public delegate Boolean RemotePartyProviderDelegate(RemoteParty_Id RemotePartyId, out RemoteParty RemoteParty);

    public delegate JObject RemotePartyToJSONDelegate(RemoteParty                                       RemoteParty,
                                                      Boolean                                           Embedded                          = false,
                                                      CustomJObjectSerializerDelegate<RemoteParty>      CustomRemotePartySerializer       = null,
                                                      CustomJObjectSerializerDelegate<BusinessDetails>  CustomBusinessDetailsSerializer   = null,
                                                      Boolean                                           IncludeCryptoHash                 = true);


    /// <summary>
    /// Extention methods for remote parties.
    /// </summary>
    public static partial class CardiRemotePartyExtentions
    {

        #region ToJSON(this RemoteParties, Skip = null, Take = null, Embedded = false, ...)

        /// <summary>
        /// Return a JSON representation for the given enumeration of remote parties.
        /// </summary>
        /// <param name="RemoteParties">An enumeration of remote parties.</param>
        /// <param name="Skip">The optional number of remote parties to skip.</param>
        /// <param name="Take">The optional number of remote parties to return.</param>
        /// <param name="Embedded">Whether this data is embedded into another data structure, e.g. into a remote party.</param>
        public static JArray ToJSON(this IEnumerable<RemoteParty>                     RemoteParties,
                                    UInt64?                                           Skip                              = null,
                                    UInt64?                                           Take                              = null,
                                    Boolean                                           Embedded                          = false,
                                    CustomJObjectSerializerDelegate<RemoteParty>      CustomRemotePartySerializer       = null,
                                    CustomJObjectSerializerDelegate<BusinessDetails>  CustomBusinessDetailsSerializer   = null,
                                    RemotePartyToJSONDelegate                         RemotePartyToJSON                 = null,
                                    Boolean                                           IncludeCryptoHash                 = true)


            => RemoteParties?.Any() != true

                   ? new JArray()

                   : new JArray(RemoteParties.
                                    Where         (remoteParty => remoteParty != null).
                                    OrderBy       (remoteParty => remoteParty.Id).
                                    SkipTakeFilter(Skip, Take).
                                    SafeSelect    (remoteParty => RemotePartyToJSON != null
                                                                      ? RemotePartyToJSON (remoteParty,
                                                                                           Embedded,
                                                                                           CustomRemotePartySerializer,
                                                                                           CustomBusinessDetailsSerializer,
                                                                                           IncludeCryptoHash)

                                                                      : remoteParty.ToJSON(Embedded,
                                                                                           CustomRemotePartySerializer,
                                                                                           CustomBusinessDetailsSerializer,
                                                                                           IncludeCryptoHash)));

        #endregion

    }


    /// <summary>
    /// A remote party.
    /// </summary>
    public class RemoteParty : IHasId<RemoteParty_Id>,
                               IEquatable<RemoteParty>,
                               IComparable<RemoteParty>,
                               IComparable
    {

        #region Data

        /// <summary>
        /// The default JSON-LD context of this object.
        /// </summary>
        public static readonly JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/contexts/OCPI/remoteParty");

        #endregion

        #region Properties

        /// <summary>
        /// The unique identification of this remote party
        /// (country code + party identification + role).
        /// </summary>
        [Mandatory]
        public RemoteParty_Id           Id                   { get; }

        /// <summary>
        /// ISO-3166 alpha-2 country code of the country this party is operating in.
        /// </summary>
        [Mandatory]
        public CountryCode              CountryCode          { get; }

        /// <summary>
        /// CPO, eMSP (or other role) ID of this party (following the ISO-15118 standard).
        /// </summary>
        [Mandatory]
        public Party_Id                 PartyId              { get; }

        /// <summary>
        /// The type of the role.
        /// </summary>
        [Mandatory]
        public Roles                    Role                 { get; }

        /// <summary>
        /// Business details of this party.
        /// </summary>
        [Mandatory]
        public BusinessDetails          BusinessDetails      { get; }

        /// <summary>
        /// The current status of the party.
        /// </summary>
        public PartyStatus              Status               { get; }

        /// <summary>
        /// Timestamp when this remote party was last updated (or created).
        /// </summary>
        public DateTime                 LastUpdated          { get; }

        /// <summary>
        /// The SHA256 hash of the JSON representation of this remote party.
        /// </summary>
        public String                   SHA256Hash           { get; private set; }



        private readonly List<AccessInfo2> _AccessInfo;

        public IEnumerable<AccessInfo2> AccessInfo
            => _AccessInfo;



        private readonly List<RemoteAccessInfo> _RemoteAccessInfos;

        public IEnumerable<RemoteAccessInfo> RemoteAccessInfos
            => _RemoteAccessInfos;

        #endregion

        #region Constructor(s)

        #region RemoteParty(..., AccessToken, AccessStatus = ALLOWED, ...)

        public RemoteParty(CountryCode      CountryCode,
                           Party_Id         PartyId,
                           Roles            Role,
                           BusinessDetails  BusinessDetails,

                           AccessToken      AccessToken,
                           AccessStatus     AccessStatus   = AccessStatus.ALLOWED,
                           PartyStatus      Status         = PartyStatus.ENABLED,

                           DateTime?        LastUpdated    = null)

            : this(CountryCode,
                   PartyId,
                   Role,
                   BusinessDetails,
                   new AccessInfo2[] {
                       new AccessInfo2(
                           AccessToken,
                           AccessStatus
                       )
                   },
                   null,
                   Status,
                   LastUpdated)

        { }

        #endregion

        #region RemoteParty(..., RemoteAccessToken, RemoteVersionsURL, ...)

        public RemoteParty(CountryCode              CountryCode,
                           Party_Id                 PartyId,
                           Roles                    Role,
                           BusinessDetails          BusinessDetails,

                           AccessToken              RemoteAccessToken,
                           URL                      RemoteVersionsURL,
                           IEnumerable<Version_Id>  RemoteVersionIds    = null,
                           Version_Id?              SelectedVersionId   = null,

                           RemoteAccessStatus?      RemoteStatus        = RemoteAccessStatus.ONLINE,
                           PartyStatus              Status              = PartyStatus.ENABLED,

                           DateTime?                LastUpdated         = null)

            : this(CountryCode,
                   PartyId,
                   Role,
                   BusinessDetails,
                   null,
                   new RemoteAccessInfo[] {
                       new RemoteAccessInfo(
                           RemoteAccessToken,
                           RemoteVersionsURL,
                           RemoteVersionIds,
                           SelectedVersionId,
                           RemoteStatus
                       )
                   },
                   Status,
                   LastUpdated)

        { }

        #endregion

        #region RemoteParty(...)

        public RemoteParty(CountryCode              CountryCode,
                           Party_Id                 PartyId,
                           Roles                    Role,
                           BusinessDetails          BusinessDetails,

                           AccessToken              AccessToken,

                           AccessToken              RemoteAccessToken,
                           URL                      RemoteVersionsURL,
                           IEnumerable<Version_Id>  RemoteVersionIds    = null,
                           Version_Id?              SelectedVersionId   = null,

                           AccessStatus             AccessStatus        = AccessStatus.ALLOWED,
                           RemoteAccessStatus?      RemoteStatus        = RemoteAccessStatus.ONLINE,
                           PartyStatus              Status              = PartyStatus.ENABLED,

                           DateTime?                LastUpdated         = null)

            : this(CountryCode,
                   PartyId,
                   Role,
                   BusinessDetails,
                   new AccessInfo2[] {
                       new AccessInfo2(
                           AccessToken,
                           AccessStatus
                       )
                   },
                   new RemoteAccessInfo[] {
                       new RemoteAccessInfo(
                           RemoteAccessToken,
                           RemoteVersionsURL,
                           RemoteVersionIds,
                           SelectedVersionId,
                           RemoteStatus
                       )
                   },
                   Status,
                   LastUpdated)

        { }

        #endregion

        #region RemoteParty(...)

        public RemoteParty(CountryCode                    CountryCode,
                           Party_Id                       PartyId,
                           Roles                          Role,
                           BusinessDetails                BusinessDetails,

                           IEnumerable<AccessInfo2>       AccessInfos,
                           IEnumerable<RemoteAccessInfo>  RemoteAccessInfos,

                           PartyStatus                    Status        = PartyStatus.ENABLED,
                           DateTime?                      LastUpdated   = null)

        {

            this.Id                  = RemoteParty_Id.Parse(
                                           String.Concat(CountryCode.ToString(),
                                                         "-",
                                                         PartyId.    ToString(),
                                                         "_",
                                                         Role.       ToString()));

            this.CountryCode         = CountryCode;
            this.PartyId             = PartyId;
            this.Role                = Role;
            this.BusinessDetails     = BusinessDetails;
            this.Status              = Status;
            this.LastUpdated         = LastUpdated ?? DateTime.UtcNow;

            this._AccessInfo         = AccessInfos.      IsNeitherNullNorEmpty() ? new List<AccessInfo2>     (AccessInfos)       : new List<AccessInfo2>();
            this._RemoteAccessInfos  = RemoteAccessInfos.IsNeitherNullNorEmpty() ? new List<RemoteAccessInfo>(RemoteAccessInfos) : new List<RemoteAccessInfo>();

            CalcSHA256Hash();

        }

        #endregion

        #endregion


        #region ToJSON(Embedded, CustomRemotePartySerializer = null, CustomBusinessDetailsSerializer = null, IncludeCryptoHash = false)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="Embedded">Whether this data is embedded into another data structure.</param>
        /// <param name="CustomRemotePartySerializer">A delegate to serialize custom remote party JSON objects.</param>
        /// <param name="CustomBusinessDetailsSerializer">A delegate to serialize custom business details JSON objects.</param>
        /// <param name="IncludeCryptoHash">Include the crypto hash value of this object.</param>
        public JObject ToJSON(Boolean                                           Embedded,
                              CustomJObjectSerializerDelegate<RemoteParty>      CustomRemotePartySerializer       = null,
                              CustomJObjectSerializerDelegate<BusinessDetails>  CustomBusinessDetailsSerializer   = null,
                              Boolean                                           IncludeCryptoHash                 = false)
        {

            var JSON = JSONObject.Create(

                           Id.ToJSON("@id"),

                           Embedded
                               ? new JProperty("@context",           DefaultJSONLDContext.ToString())
                               : null,

                           new JProperty("countryCode",              CountryCode.         ToString()),
                           new JProperty("partyId",                  PartyId.             ToString()),
                           new JProperty("role",                     Role.                ToString()),
                           new JProperty("partyStatus",              Status.         ToString()),

                           BusinessDetails != null
                               ? new JProperty("businessDetails",    BusinessDetails.     ToJSON(CustomBusinessDetailsSerializer))
                               : null,

                           _AccessInfo.SafeAny()
                               ? new JProperty("accessInfos",        new JArray(_AccessInfo.       SafeSelect(accessInfo       => accessInfo.      ToJSON())))
                               : null,

                           _RemoteAccessInfos.SafeAny()
                               ? new JProperty("remoteAccessInfos",  new JArray(_RemoteAccessInfos.SafeSelect(remoteAccessInfo => remoteAccessInfo.ToJSON())))
                               : null,

                           new JProperty("last_updated",             LastUpdated.         ToIso8601()),

                           IncludeCryptoHash
                               ? new JProperty("sha256Hash",         SHA256Hash)
                               : null

                       );

            return CustomRemotePartySerializer != null
                       ? CustomRemotePartySerializer(this, JSON)
                       : JSON;

        }

        #endregion



        //public Credentials AsCredentials()

        //    => new Credentials(Token,
        //                       VersionsURL.Value,
        //                       Roles.Select(role => new CredentialsRole(role.CountryCode,
        //                                                                role.PartyId,
        //                                                                role.Role,
        //                                                                role.BusinessDetails)));




        #region CalcSHA256Hash(CustomRemotePartySerializer = null, CustomBusinessDetailsSerializer = null, ...)

        /// <summary>
        /// Calculate the SHA256 hash of the JSON representation of this remote party in HEX.
        /// </summary>
        /// <param name="CustomRemotePartySerializer">A delegate to serialize custom remote party JSON objects.</param>
        /// <param name="CustomBusinessDetailsSerializer">A delegate to serialize custom business details JSON objects.</param>
        public String CalcSHA256Hash(CustomJObjectSerializerDelegate<RemoteParty>      CustomRemotePartySerializer       = null,
                                     CustomJObjectSerializerDelegate<BusinessDetails>  CustomBusinessDetailsSerializer   = null)
        {

            using (var SHA256 = new SHA256Managed())
            {

                return SHA256Hash = "0x" + SHA256.ComputeHash(Encoding.Unicode.GetBytes(
                                                                  ToJSON(false, // alway with @context!
                                                                         CustomRemotePartySerializer,
                                                                         CustomBusinessDetailsSerializer,
                                                                         false).
                                                                  ToString(Newtonsoft.Json.Formatting.None)
                                                              )).
                                                  Select(value => String.Format("{0:x2}", value)).
                                                  Aggregate();

            }

        }

        #endregion


        #region Operator overloading

        #region Operator == (RemoteParty1, RemoteParty2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RemoteParty1">A remote party identification.</param>
        /// <param name="RemoteParty2">Another remote party identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (RemoteParty RemoteParty1,
                                           RemoteParty RemoteParty2)
        {

            if (Object.ReferenceEquals(RemoteParty1, RemoteParty2))
                return true;

            if ((RemoteParty1 is null) || (RemoteParty2 is null))
                return false;

            return RemoteParty1.Equals(RemoteParty2);

        }

        #endregion

        #region Operator != (RemoteParty1, RemoteParty2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RemoteParty1">A remote party identification.</param>
        /// <param name="RemoteParty2">Another remote party identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (RemoteParty RemoteParty1,
                                           RemoteParty RemoteParty2)

            => !(RemoteParty1 == RemoteParty2);

        #endregion

        #region Operator <  (RemoteParty1, RemoteParty2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RemoteParty1">A remote party identification.</param>
        /// <param name="RemoteParty2">Another remote party identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (RemoteParty RemoteParty1,
                                          RemoteParty RemoteParty2)

            => RemoteParty1 is null
                   ? throw new ArgumentNullException(nameof(RemoteParty1), "The given remote party must not be null!")
                   : RemoteParty1.CompareTo(RemoteParty2) < 0;

        #endregion

        #region Operator <= (RemoteParty1, RemoteParty2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RemoteParty1">A remote party identification.</param>
        /// <param name="RemoteParty2">Another remote party identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (RemoteParty RemoteParty1,
                                           RemoteParty RemoteParty2)

            => !(RemoteParty1 > RemoteParty2);

        #endregion

        #region Operator >  (RemoteParty1, RemoteParty2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RemoteParty1">A remote party identification.</param>
        /// <param name="RemoteParty2">Another remote party identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (RemoteParty RemoteParty1,
                                          RemoteParty RemoteParty2)

            => RemoteParty1 is null
                   ? throw new ArgumentNullException(nameof(RemoteParty1), "The given remote party must not be null!")
                   : RemoteParty1.CompareTo(RemoteParty2) > 0;

        #endregion

        #region Operator >= (RemoteParty1, RemoteParty2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RemoteParty1">A remote party identification.</param>
        /// <param name="RemoteParty2">Another remote party identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (RemoteParty RemoteParty1,
                                           RemoteParty RemoteParty2)

            => !(RemoteParty1 < RemoteParty2);

        #endregion

        #endregion

        #region IComparable<RemoteParty> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is RemoteParty remoteParty
                   ? CompareTo(remoteParty)
                   : throw new ArgumentException("The given object is not a remote party!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(RemoteParty)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RemoteParty">A remote party object to compare with.</param>
        public Int32 CompareTo(RemoteParty RemoteParty)

            => RemoteParty is null
                   ? throw new ArgumentNullException(nameof(RemoteParty), "The given remote party must not be null!")
                   : Id.CompareTo(RemoteParty.Id);

        #endregion

        #endregion

        #region IEquatable<RemoteParty> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is RemoteParty remoteParty &&
                   Equals(remoteParty);

        #endregion

        #region Equals(RemoteParty)

        /// <summary>
        /// Compares two remote partys for equality.
        /// </summary>
        /// <param name="RemoteParty">A remote party to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(RemoteParty RemoteParty)

            => !(RemoteParty is null) &&
                 Id.               Equals(RemoteParty.Id) &&
                 CountryCode.      Equals(RemoteParty.Id) &&
                 PartyId.          Equals(RemoteParty.Id) &&
                 Role.             Equals(RemoteParty.Id) &&
                 BusinessDetails.  Equals(RemoteParty.Id);
                 //Status.           Equals(RemoteParty.Id);
                 //LastUpdated.      Equals(RemoteParty.Id);

                 //AccessInfos.      Equals(RemoteParty.Id) &&
                 //RemoteAccessInfos.Equals(RemoteParty.Id);

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Get the hashcode of this object.
        /// </summary>
        public override Int32 GetHashCode()

            => Id.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => Id.ToString();

        #endregion


    }

}
