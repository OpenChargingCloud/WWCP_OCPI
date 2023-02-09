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

using System.Security.Cryptography;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_1_1
{

    public delegate Boolean RemotePartyProviderDelegate(RemoteParty_Id RemotePartyId, out RemoteParty RemoteParty);

    public delegate JObject RemotePartyToJSONDelegate(RemoteParty                                         RemoteParty,
                                                      Boolean                                             Embedded                           = false,
                                                      CustomJObjectSerializerDelegate<RemoteParty>?       CustomRemotePartySerializer        = null,
                                                      CustomJObjectSerializerDelegate<BusinessDetails>?   CustomBusinessDetailsSerializer    = null,
                                                      CustomJObjectSerializerDelegate<AccessInfoStatus>?  CustomAccessInfoStatusSerializer   = null,
                                                      CustomJObjectSerializerDelegate<RemoteAccessInfo>?  CustomRemoteAccessInfoSerializer   = null);


    /// <summary>
    /// Extension methods for remote parties.
    /// </summary>
    public static partial class RemotePartyExtensions
    {

        #region ToJSON(this RemoteParties, Skip = null, Take = null, Embedded = false, ...)

        /// <summary>
        /// Return a JSON representation for the given enumeration of remote parties.
        /// </summary>
        /// <param name="RemoteParties">An enumeration of remote parties.</param>
        /// <param name="Skip">The optional number of remote parties to skip.</param>
        /// <param name="Take">The optional number of remote parties to return.</param>
        /// <param name="Embedded">Whether this data is embedded into another data structure, e.g. into a remote party.</param>
        public static JArray ToJSON(this IEnumerable<RemoteParty>                       RemoteParties,
                                    UInt64?                                             Skip                               = null,
                                    UInt64?                                             Take                               = null,
                                    Boolean                                             Embedded                           = false,
                                    CustomJObjectSerializerDelegate<RemoteParty>?       CustomRemotePartySerializer        = null,
                                    CustomJObjectSerializerDelegate<BusinessDetails>?   CustomBusinessDetailsSerializer    = null,
                                    CustomJObjectSerializerDelegate<AccessInfoStatus>?  CustomAccessInfoStatusSerializer   = null,
                                    CustomJObjectSerializerDelegate<RemoteAccessInfo>?  CustomRemoteAccessInfoSerializer   = null,
                                    RemotePartyToJSONDelegate?                          RemotePartyToJSON                  = null)


            => RemoteParties?.Any() != true

                   ? new JArray()

                   : new JArray(RemoteParties.
                                    Where         (remoteParty => remoteParty is not null).
                                    OrderBy       (remoteParty => remoteParty.Id).
                                    SkipTakeFilter(Skip, Take).
                                    SafeSelect    (remoteParty => RemotePartyToJSON is not null
                                                                      ? RemotePartyToJSON (remoteParty,
                                                                                           Embedded,
                                                                                           CustomRemotePartySerializer,
                                                                                           CustomBusinessDetailsSerializer,
                                                                                           CustomAccessInfoStatusSerializer,
                                                                                           CustomRemoteAccessInfoSerializer)

                                                                      : remoteParty.ToJSON(Embedded,
                                                                                           CustomRemotePartySerializer,
                                                                                           CustomBusinessDetailsSerializer,
                                                                                           CustomAccessInfoStatusSerializer,
                                                                                           CustomRemoteAccessInfoSerializer)));

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
        [Mandatory]
        public PartyStatus              Status               { get; }

        /// <summary>
        /// Timestamp when this remote party was last updated (or created).
        /// </summary>
        [Mandatory]
        public DateTime                 LastUpdated          { get; }

        /// <summary>
        /// The SHA256 hash of the JSON representation of this remote party.
        /// </summary>
        [Mandatory]
        public String                   ETag                 { get; private set; }



        private readonly List<AccessInfoStatus> accessInfoStatus;

        public IEnumerable<AccessInfoStatus> AccessInfoStatus
            => accessInfoStatus;



        private readonly List<RemoteAccessInfo> remoteAccessInfos;

        public IEnumerable<RemoteAccessInfo> RemoteAccessInfos
            => remoteAccessInfos;

        #endregion

        #region Constructor(s)

        #region RemoteParty(..., AccessToken, AccessStatus = ALLOWED, ...)

        public RemoteParty(CountryCode      CountryCode,
                           Party_Id         PartyId,
                           Roles            Role,
                           BusinessDetails  BusinessDetails,

                           AccessToken      AccessToken,
                           AccessStatus     AccessStatus   = AccessStatus.ALLOWED,
                           PartyStatus      Status         = PartyStatus. ENABLED,

                           DateTime?        LastUpdated    = null)

            : this(CountryCode,
                   PartyId,
                   Role,
                   BusinessDetails,
                   new AccessInfoStatus[] {
                       new AccessInfoStatus(
                           AccessToken,
                           AccessStatus
                       )
                   },
                   Array.Empty<RemoteAccessInfo>(),
                   Status,
                   LastUpdated)

        { }

        #endregion

        #region RemoteParty(..., RemoteAccessToken, RemoteVersionsURL, ...)

        public RemoteParty(CountryCode               CountryCode,
                           Party_Id                  PartyId,
                           Roles                     Role,
                           BusinessDetails           BusinessDetails,

                           AccessToken               RemoteAccessToken,
                           URL                       RemoteVersionsURL,
                           IEnumerable<Version_Id>?  RemoteVersionIds    = null,
                           Version_Id?               SelectedVersionId   = null,

                           RemoteAccessStatus?       RemoteStatus        = RemoteAccessStatus.ONLINE,
                           PartyStatus               Status              = PartyStatus.       ENABLED,

                           DateTime?                 LastUpdated         = null)

            : this(CountryCode,
                   PartyId,
                   Role,
                   BusinessDetails,
                   Array.Empty<AccessInfoStatus>(),
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

        public RemoteParty(CountryCode               CountryCode,
                           Party_Id                  PartyId,
                           Roles                     Role,
                           BusinessDetails           BusinessDetails,

                           AccessToken               AccessToken,

                           AccessToken               RemoteAccessToken,
                           URL                       RemoteVersionsURL,
                           IEnumerable<Version_Id>?  RemoteVersionIds    = null,
                           Version_Id?               SelectedVersionId   = null,

                           AccessStatus              AccessStatus        = AccessStatus.      ALLOWED,
                           RemoteAccessStatus?       RemoteStatus        = RemoteAccessStatus.ONLINE,
                           PartyStatus               Status              = PartyStatus.       ENABLED,

                           DateTime?                 LastUpdated         = null)

            : this(CountryCode,
                   PartyId,
                   Role,
                   BusinessDetails,
                   new AccessInfoStatus[] {
                       new AccessInfoStatus(
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

                           IEnumerable<AccessInfoStatus>  AccessInfoStatus,
                           IEnumerable<RemoteAccessInfo>  RemoteAccessInfos,

                           PartyStatus                    Status        = PartyStatus.ENABLED,
                           DateTime?                      LastUpdated   = null)

        {

            this.Id                 = RemoteParty_Id.Parse(
                                          String.Concat(CountryCode,
                                                        "-",
                                                        PartyId,
                                                        "_",
                                                        Role));

            this.CountryCode        = CountryCode;
            this.PartyId            = PartyId;
            this.Role               = Role;
            this.BusinessDetails    = BusinessDetails;
            this.Status             = Status;
            this.LastUpdated        = LastUpdated ?? Timestamp.Now;

            this.accessInfoStatus   = AccessInfoStatus. IsNeitherNullNorEmpty() ? new List<AccessInfoStatus>(AccessInfoStatus)  : new List<AccessInfoStatus>();
            this.remoteAccessInfos  = RemoteAccessInfos.IsNeitherNullNorEmpty() ? new List<RemoteAccessInfo>(RemoteAccessInfos) : new List<RemoteAccessInfo>();

            this.ETag               = CalcSHA256Hash();

        }

        #endregion

        #endregion


        #region ToJSON(Embedded, CustomRemotePartySerializer = null, CustomBusinessDetailsSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="Embedded">Whether this data is embedded into another data structure.</param>
        /// <param name="CustomRemotePartySerializer">A delegate to serialize custom remote party JSON objects.</param>
        /// <param name="CustomBusinessDetailsSerializer">A delegate to serialize custom business details JSON objects.</param>
        /// <param name="CustomAccessInfoStatusSerializer">A delegate to serialize custom access information status JSON objects.</param>
        /// <param name="CustomRemoteAccessInfoSerializer">A delegate to serialize custom remote access information JSON objects.</param>
        public JObject ToJSON(Boolean                                             Embedded,
                              CustomJObjectSerializerDelegate<RemoteParty>?       CustomRemotePartySerializer        = null,
                              CustomJObjectSerializerDelegate<BusinessDetails>?   CustomBusinessDetailsSerializer    = null,
                              CustomJObjectSerializerDelegate<AccessInfoStatus>?  CustomAccessInfoStatusSerializer   = null,
                              CustomJObjectSerializerDelegate<RemoteAccessInfo>?  CustomRemoteAccessInfoSerializer   = null)
        {

            var JSON = JSONObject.Create(

                           new JProperty("@id",                      Id.                  ToString()),

                           Embedded
                               ? new JProperty("@context",           DefaultJSONLDContext.ToString())
                               : null,

                           new JProperty("countryCode",              CountryCode.         ToString()),
                           new JProperty("partyId",                  PartyId.             ToString()),
                           new JProperty("role",                     Role.                ToString()),
                           new JProperty("partyStatus",              Status.              ToString()),

                           BusinessDetails is not null
                               ? new JProperty("businessDetails",    BusinessDetails.     ToJSON(CustomBusinessDetailsSerializer))
                               : null,

                           accessInfoStatus.Any()
                               ? new JProperty("accessInfos",        new JArray(accessInfoStatus. SafeSelect(_accessInfoStatus => _accessInfoStatus.ToJSON(CustomAccessInfoStatusSerializer))))
                               : null,

                           remoteAccessInfos.Any()
                               ? new JProperty("remoteAccessInfos",  new JArray(remoteAccessInfos.SafeSelect(remoteAccessInfo  => remoteAccessInfo. ToJSON(CustomRemoteAccessInfoSerializer))))
                               : null,

                           new JProperty("last_updated",             LastUpdated.         ToIso8601())

                       );

            return CustomRemotePartySerializer is not null
                       ? CustomRemotePartySerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region CalcSHA256Hash(CustomRemotePartySerializer = null, CustomBusinessDetailsSerializer = null, ...)

        /// <summary>
        /// Calculate the SHA256 hash of the JSON representation of this remote party in HEX.
        /// </summary>
        /// <param name="CustomRemotePartySerializer">A delegate to serialize custom remote party JSON objects.</param>
        /// <param name="CustomBusinessDetailsSerializer">A delegate to serialize custom business details JSON objects.</param>
        /// <param name="CustomAccessInfoStatusSerializer">A delegate to serialize custom access information status JSON objects.</param>
        /// <param name="CustomRemoteAccessInfoSerializer">A delegate to serialize custom remote access information JSON objects.</param>
        public String CalcSHA256Hash(CustomJObjectSerializerDelegate<RemoteParty>?       CustomRemotePartySerializer        = null,
                                     CustomJObjectSerializerDelegate<BusinessDetails>?   CustomBusinessDetailsSerializer    = null,
                                     CustomJObjectSerializerDelegate<AccessInfoStatus>?  CustomAccessInfoStatusSerializer   = null,
                                     CustomJObjectSerializerDelegate<RemoteAccessInfo>?  CustomRemoteAccessInfoSerializer   = null)
        {

            this.ETag = SHA256.Create().ComputeHash(ToJSON(false, // always with @context!
                                                           CustomRemotePartySerializer,
                                                           CustomBusinessDetailsSerializer,
                                                           CustomAccessInfoStatusSerializer,
                                                           CustomRemoteAccessInfoSerializer).ToUTF8Bytes()).ToBase64();

            return this.ETag;

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
        /// Compares two remote parties.
        /// </summary>
        /// <param name="Object">A remote party to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is RemoteParty remoteParty
                   ? CompareTo(remoteParty)
                   : throw new ArgumentException("The given object is not a remote party!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(RemoteParty)

        /// <summary>
        /// Compares two remote parties.
        /// </summary>
        /// <param name="RemoteParty">A remote party to compare with.</param>
        public Int32 CompareTo(RemoteParty? RemoteParty)
        {

            if (RemoteParty is null)
                throw new ArgumentNullException(nameof(RemoteParty), "The given remote party must not be null!");

            var c = Id.             CompareTo(RemoteParty.Id);

            if (c == 0)
                c = CountryCode.    CompareTo(RemoteParty.CountryCode);

            if (c == 0)
                c = PartyId.        CompareTo(RemoteParty.PartyId);

            if (c == 0)
                c = Role.           CompareTo(RemoteParty.Role);

            if (c == 0)
                c = BusinessDetails.CompareTo(RemoteParty.BusinessDetails);

            if (c == 0)
                c = Status.         CompareTo(RemoteParty.Status);

            if (c == 0)
                c = LastUpdated.    CompareTo(RemoteParty.LastUpdated);

            if (c == 0)
                c = ETag.           CompareTo(RemoteParty.ETag);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<RemoteParty> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two remote parties for equality.
        /// </summary>
        /// <param name="Object">A remote party to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is RemoteParty remoteParty &&
                   Equals(remoteParty);

        #endregion

        #region Equals(RemoteParty)

        /// <summary>
        /// Compares two remote parties for equality.
        /// </summary>
        /// <param name="RemoteParty">A remote party to compare with.</param>
        public Boolean Equals(RemoteParty? RemoteParty)

            => RemoteParty is not null &&

               Id.             Equals(RemoteParty.Id)              &&
               CountryCode.    Equals(RemoteParty.CountryCode)     &&
               PartyId.        Equals(RemoteParty.PartyId)         &&
               Role.           Equals(RemoteParty.Role)            &&
               BusinessDetails.Equals(RemoteParty.BusinessDetails) &&
               Status.         Equals(RemoteParty.Status)          &&
               LastUpdated.    Equals(RemoteParty.LastUpdated)     &&
               ETag.           Equals(RemoteParty.ETag)            &&

               accessInfoStatus.Count.Equals(RemoteParty.accessInfoStatus.Count)     &&
               accessInfoStatus. All(_accessInfoStatus => RemoteParty.accessInfoStatus. Contains(_accessInfoStatus)) &&

               remoteAccessInfos.Count.Equals(RemoteParty.remoteAccessInfos.Count)     &&
               remoteAccessInfos.All(remoteAccessInfo  => RemoteParty.remoteAccessInfos.Contains(remoteAccessInfo));

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Get the hashcode of this object.
        /// </summary>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return Id.               GetHashCode()  * 29 ^
                       CountryCode.      GetHashCode()  * 23 ^
                       PartyId.          GetHashCode()  * 19 ^
                       Role.             GetHashCode()  * 17 ^
                       BusinessDetails.  GetHashCode()  * 13 ^
                       Status.           GetHashCode()  * 11 ^
                       LastUpdated.      GetHashCode()  *  7 ^
                       ETag.             GetHashCode()  *  5 ^
                       accessInfoStatus. CalcHashCode() *  3 ^
                       remoteAccessInfos.CalcHashCode();

            }
        }

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
