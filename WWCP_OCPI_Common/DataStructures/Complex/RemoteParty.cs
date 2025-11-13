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

using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using System.Security.Cryptography;

#endregion

namespace cloud.charging.open.protocols.OCPI
{

    /// <summary>
    /// A remote party serving multiple CPOs and/or EMSPs.
    /// </summary>
    public abstract class ARemoteParty : IRemoteParty
    {

        #region Properties

        /// <summary>
        /// The unique identification of this remote party
        /// (country code + party identification + role).
        /// </summary>
        [Mandatory]
        public RemoteParty_Id                                             Id                            { get; }

        /// <summary>
        /// The current status of the party.
        /// </summary>
        [Mandatory]
        public PartyStatus                                                Status                        { get; }

        /// <summary>
        /// The timestamp when this remote party was created.
        /// </summary>
        [Mandatory, VendorExtension(VE.GraphDefined, VE.Pagination)]
        public DateTimeOffset                                             Created                       { get; }

        /// <summary>
        /// Timestamp when this remote party was last updated (or created).
        /// </summary>
        [Mandatory]
        public DateTimeOffset                                             LastUpdated                   { get; }

        /// <summary>
        /// The SHA256 hash of the JSON representation of this remote party.
        /// </summary>
        [Mandatory]
        public String                                                     ETag                          { get; protected set; } = String.Empty;


        /// <summary>
        /// Prefer IPv4 instead of IPv6.
        /// </summary>
        public Boolean?                                                   PreferIPv4                    { get; }

        /// <summary>
        /// The remote TLS certificate validator.
        /// </summary>
        public RemoteTLSServerCertificateValidationHandler<IHTTPClient>?  RemoteCertificateValidator    { get; set; }

        /// <summary>
        /// A delegate to select a TLS client certificate.
        /// </summary>
        public LocalCertificateSelectionHandler?                          LocalCertificateSelector      { get; }

        /// <summary>
        /// The TLS client certificate and private key to use for HTTP authentication.
        /// </summary>
        public X509Certificate2?                                          ClientCertificate             { get; }

        /// <summary>
        /// The TLS protocol to use.
        /// </summary>
        public SslProtocols?                                              TLSProtocols                  { get; }

        /// <summary>
        /// The optional HTTP accept header.
        /// </summary>
        public HTTPContentType?                                           ContentType                   { get; }

        /// <summary>
        /// The optional HTTP accept header.
        /// </summary>
        public AcceptTypes?                                               Accept                        { get; }

        /// <summary>
        /// The HTTP user agent identification.
        /// </summary>
        public String?                                                    HTTPUserAgent                 { get; }

        /// <summary>
        /// The timeout for upstream requests.
        /// </summary>
        public TimeSpan?                                                  RequestTimeout                { get; set; }

        /// <summary>
        /// The delay between transmission retries.
        /// </summary>
        public TransmissionRetryDelayDelegate?                            TransmissionRetryDelay        { get; }

        /// <summary>
        /// The maximum number of retries when communicating with the remote HTTP service.
        /// </summary>
        public UInt16?                                                    MaxNumberOfRetries            { get; }

        /// <summary>
        /// The size of the internal buffers of HTTP clients.
        /// </summary>
        public UInt32?                                                    InternalBufferSize            { get; }

        /// <summary>
        /// Whether to pipeline multiple HTTP request through a single HTTP/TCP connection.
        /// </summary>
        public Boolean?                                                   UseHTTPPipelining             { get; }



        protected readonly List<LocalAccessInfo> localAccessInfos;

        /// <summary>
        /// Local access information.
        /// </summary>
        public IEnumerable<LocalAccessInfo> LocalAccessInfos
            => localAccessInfos;



        protected readonly List<RemoteAccessInfo> remoteAccessInfos;

        /// <summary>
        /// Remote access information.
        /// </summary>
        public IEnumerable<RemoteAccessInfo> RemoteAccessInfos
            => remoteAccessInfos;

        #endregion

        #region Constructor(s)

        public ARemoteParty(RemoteParty_Id                                             Id,

                            IEnumerable<LocalAccessInfo>                               LocalAccessInfos,
                            IEnumerable<RemoteAccessInfo>                              RemoteAccessInfos,

                            PartyStatus?                                               Status                       = PartyStatus.ENABLED,

                            Boolean?                                                   PreferIPv4                   = null,
                            RemoteTLSServerCertificateValidationHandler<IHTTPClient>?  RemoteCertificateValidator   = null,
                            LocalCertificateSelectionHandler?                          LocalCertificateSelector     = null,
                            X509Certificate2?                                          ClientCertificate            = null,
                            SslProtocols?                                              TLSProtocols                 = null,
                            HTTPContentType?                                           ContentType                  = null,
                            AcceptTypes?                                               Accept                       = null,
                            String?                                                    HTTPUserAgent                = null,
                            TimeSpan?                                                  RequestTimeout               = null,
                            TransmissionRetryDelayDelegate?                            TransmissionRetryDelay       = null,
                            UInt16?                                                    MaxNumberOfRetries           = null,
                            UInt32?                                                    InternalBufferSize           = null,
                            Boolean?                                                   UseHTTPPipelining            = null,

                            DateTimeOffset?                                            Created                      = null,
                            DateTimeOffset?                                            LastUpdated                  = null)

        {

            this.Id                          = Id;
            this.Status                      = Status                     ?? PartyStatus.ENABLED;

            this.PreferIPv4                  = PreferIPv4;
            this.RemoteCertificateValidator  = RemoteCertificateValidator ?? ((sender,
                                                                               certificate,
                                                                               certificateChain,
                                                                               tlsClient,
                                                                               policyErrors) => (false, []));
            this.LocalCertificateSelector    = LocalCertificateSelector;
            this.ClientCertificate           = ClientCertificate;
            this.TLSProtocols                = TLSProtocols;
            this.ContentType                 = ContentType                ?? HTTPContentType.Application.JSON_UTF8;
            this.Accept                      = Accept                     ?? AcceptTypes.FromHTTPContentTypes(HTTPContentType.Application.JSON_UTF8);
            this.HTTPUserAgent               = HTTPUserAgent;
            this.RequestTimeout              = RequestTimeout;
            this.TransmissionRetryDelay      = TransmissionRetryDelay;
            this.MaxNumberOfRetries          = MaxNumberOfRetries;
            this.InternalBufferSize          = InternalBufferSize;
            this.UseHTTPPipelining           = UseHTTPPipelining;

            this.Created                     = Created     ?? LastUpdated ?? Timestamp.Now;
            this.LastUpdated                 = LastUpdated ?? Created     ?? Timestamp.Now;

            this.localAccessInfos            = LocalAccessInfos. IsNeitherNullNorEmpty() ? [.. LocalAccessInfos]  : [];
            this.remoteAccessInfos           = RemoteAccessInfos.IsNeitherNullNorEmpty() ? [.. RemoteAccessInfos] : [];

        }

        #endregion


        #region ToJSON(JSONLDContext, CustomLocalAccessInfoSerializer, CustomRemoteAccessInfoSerializer)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="JSONLDContext">Whether to add a JSON-LD @context property, e.g. when this data is embedded into another data structure.</param>
        /// <param name="CustomLocalAccessInfoSerializer">A delegate to serialize custom local access information JSON objects.</param>
        /// <param name="CustomRemoteAccessInfoSerializer">A delegate to serialize custom remote access information JSON objects.</param>
        public JObject ToJSON(JSONLDContext?                                      JSONLDContext,
                              CustomJObjectSerializerDelegate<LocalAccessInfo>?   CustomLocalAccessInfoSerializer,
                              CustomJObjectSerializerDelegate<RemoteAccessInfo>?  CustomRemoteAccessInfoSerializer)
        {

            AsymmetricAlgorithm? clientPrivateKey = ClientCertificate?.GetRSAPrivateKey();
            clientPrivateKey ??= ClientCertificate?.GetDSAPrivateKey();

            var json = JSONObject.Create(

                                 new JProperty("@id",                 Id.ToString()),

                           JSONLDContext.HasValue
                               ? new JProperty("@context",            JSONLDContext.ToString())
                               : null,

                                 new JProperty("partyStatus",         Status.ToString()),

                           localAccessInfos. Count != 0
                               ? new JProperty("localAccessInfos",    new JArray(localAccessInfos. Select(localAccessInfo  => localAccessInfo. ToJSON(CustomLocalAccessInfoSerializer))))
                               : null,

                           remoteAccessInfos.Count != 0
                               ? new JProperty("remoteAccessInfos",   new JArray(remoteAccessInfos.Select(remoteAccessInfo => remoteAccessInfo.ToJSON(CustomRemoteAccessInfoSerializer))))
                               : null,


                           // ...

                           ClientCertificate is not null
                               ? new JProperty("clientCertificate",   ClientCertificate.ExportCertificatePem())
                               : null,

                           ClientCertificate is not null && ClientCertificate.HasPrivateKey && clientPrivateKey is not null
                               ? new JProperty("clientPrivateKey",    clientPrivateKey.ExportPkcs8PrivateKeyPem())
                               : null,

                           // ...

                           HTTPUserAgent.IsNotNullOrEmpty()
                               ? new JProperty("httpUserAgent",       HTTPUserAgent)
                               : null,

                           // ...


                                 new JProperty("created",             Created.    ToISO8601()),
                                 new JProperty("last_updated",        LastUpdated.ToISO8601())

                       );

            return json;

        }

        #endregion


        #region CalcSHA256Hash(CustomARemotePartySerializer = null, CustomCredentialsRoleSerializer = null, ...)

        ///// <summary>
        ///// Calculate the SHA256 hash of the JSON representation of this remote party in HEX.
        ///// </summary>
        ///// <param name="CustomARemotePartySerializer">A delegate to serialize custom remote party JSON objects.</param>
        ///// <param name="CustomCredentialsRoleSerializer">A delegate to serialize custom credentials roles JSON objects.</param>
        ///// <param name="CustomBusinessDetailsSerializer">A delegate to serialize custom business details JSON objects.</param>
        ///// <param name="CustomLocalAccessInfoSerializer">A delegate to serialize custom local access information JSON objects.</param>
        ///// <param name="CustomRemoteAccessInfoSerializer">A delegate to serialize custom remote access information JSON objects.</param>
        //public String CalcSHA256Hash(CustomJObjectSerializerDelegate<ARemoteParty>?       CustomARemotePartySerializer        = null,
        //                             CustomJObjectSerializerDelegate<CredentialsRole>?   CustomCredentialsRoleSerializer    = null,
        //                             CustomJObjectSerializerDelegate<BusinessDetails>?   CustomBusinessDetailsSerializer    = null,
        //                             CustomJObjectSerializerDelegate<Image>?             CustomImageSerializer              = null,
        //                             CustomJObjectSerializerDelegate<LocalAccessInfo>?   CustomLocalAccessInfoSerializer    = null,
        //                             CustomJObjectSerializerDelegate<RemoteAccessInfo>?  CustomRemoteAccessInfoSerializer   = null)
        //{

        //    this.ETag = SHA256.HashData(ToJSON(false, // always with @context!
        //                                       CustomARemotePartySerializer,
        //                                       CustomCredentialsRoleSerializer,
        //                                       CustomBusinessDetailsSerializer,
        //                                       CustomImageSerializer,
        //                                       CustomLocalAccessInfoSerializer,
        //                                       CustomRemoteAccessInfoSerializer).ToUTF8Bytes()).ToBase64();

        //    return this.ETag;

        //}

        #endregion


        #region IComparable<ARemoteParty> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two remote parties.
        /// </summary>
        /// <param name="Object">A remote party to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is IRemoteParty remoteParty
                   ? CompareTo(remoteParty.Id)
                   : throw new ArgumentException("The given object is not a remote party!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(RemotePartyId)

        /// <summary>
        /// Compares this remote party with another remote party identification.
        /// </summary>
        /// <param name="RemotePartyId">A remote party identification to compare with.</param>
        public Int32 CompareTo(RemoteParty_Id RemotePartyId)
            => Id.CompareTo(RemotePartyId);

        #endregion

        #region CompareTo(RemoteParty)

        /// <summary>
        /// Compares two remote parties.
        /// </summary>
        /// <param name="ARemoteParty">A remote party to compare with.</param>
        public Int32 CompareTo(ARemoteParty? ARemoteParty)
        {

            if (ARemoteParty is null)
                throw new ArgumentNullException(nameof(ARemoteParty), "The given remote party must not be null!");

            var c = Id.CompareTo(ARemoteParty.Id);

            if (c == 0)
                c = Status.CompareTo(ARemoteParty.Status);

            //if (c == 0)
            //    c = Roles.Count().CompareTo(ARemoteParty.Roles.Count());

            //if (c == 0)
            //{
            //    for (var i = 0; i < Roles.Count(); i++)
            //    {

            //        c = Roles.ElementAt(i).CompareTo(ARemoteParty.Roles.ElementAt(i));

            //        if (c != 0)
            //            break;

            //    }
            //}

            if (c == 0)
                c = LastUpdated.CompareTo(ARemoteParty.LastUpdated);

            if (c == 0)
                c = ETag.CompareTo(ARemoteParty.ETag);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<ARemoteParty> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two remote parties for equality.
        /// </summary>
        /// <param name="Object">A remote party to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is IRemoteParty remoteParty &&
                   Equals(remoteParty.Id);

        #endregion

        #region Equals(RemotePartyId)

        /// <summary>
        /// Compares this remote party with another remote party identification for equality.
        /// </summary>
        /// <param name="RemotePartyId">A remote party identification to compare with.</param>
        public Boolean Equals(RemoteParty_Id RemotePartyId)

            => Id.Equals(RemotePartyId);

        #endregion

        #region Equals(RemoteParty)

        /// <summary>
        /// Compares two remote parties for equality.
        /// </summary>
        /// <param name="ARemoteParty">A remote party to compare with.</param>
        public Boolean Equals(ARemoteParty? ARemoteParty)

            => ARemoteParty is not null &&

               Id.Equals(ARemoteParty.Id) &&
               Status.Equals(ARemoteParty.Status) &&

               //Roles.Count().Equals(ARemoteParty.Roles.Count()) &&
               //Roles.All(ARemoteParty.Roles.Contains) &&

               LastUpdated.Equals(ARemoteParty.LastUpdated) &&
               ETag.Equals(ARemoteParty.ETag) &&

               localAccessInfos.Count.Equals(ARemoteParty.localAccessInfos.Count) &&
               localAccessInfos.All(ARemoteParty.localAccessInfos.Contains) &&

               remoteAccessInfos.Count.Equals(ARemoteParty.remoteAccessInfos.Count) &&
               remoteAccessInfos.All(ARemoteParty.remoteAccessInfos.Contains);

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
            => Id.ToString();

        #endregion

    }

}
