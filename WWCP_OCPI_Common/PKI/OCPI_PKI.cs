/*
 * Copyright (c) 2015-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using Org.BouncyCastle.Math;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.X509.Extension;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.PKI;

#endregion

namespace cloud.charging.open.protocols.OCPI
{

    /// <summary>
    /// Helper methods to map OCPI data structures to
    /// WWCP data structures and vice versa.
    /// </summary>
    public class OCPI_PKI
    {

        // Design:
        //   Expected automatic key/certificate change: 24 hours to 30 days!
        //   UseCase 1: One client certificates per CPO-EMSP tuple and role: Better security with role-based access control!
        //   UseCase 2. One client certificate per CPO-EMSP tuple:           Better security, but less scalable: n(n-1)/2 certificates (n=100 => 4950 certificates)!
        //   UseCase 3: One client certificate per CPO/EMSP role:            Easier scaling, maybe less secure as key/certificate is reused, but still role-based access control!
        //   UseCase 4. One client certificate per CPO/EMSP:                 Easier scaling, maybe less secure as key/certificate is reused!


        #region Data

        // Using the attribute OID 1.2.840.113549.1.9.14 (extensionRequest) is how the PKI ecosystem expects you to pass X.509 extensions in a CSR.
        public const String KeyGroupIdAttribute  = "1.2.3.4.5.6.7.6.1";
        public const String NotBeforeAttribute   = "1.2.3.4.5.6.7.7.1";
        public const String NotAfterAttribute    = "1.2.3.4.5.6.7.7.2";
        public const String PartyIdAttribute     = "1.2.3.4.5.6.7.8.1";
        public const String SubCPOsIdAttribute   = "1.2.3.4.5.6.7.8.2";
        public const String SubEMSPsIdAttribute  = "1.2.3.4.5.6.7.8.3";

        #endregion

        #region Properties

        public UInt16                                    DefaultRSASize                      { get; set; } = 4096;
        public String                                    DefaultECCAlgorithm                 { get; set; } = "secp256r1";
        public String                                    DefaultMLKEMAlgorithm               { get; set; } = "ml_kem_768";
        public String                                    DefaultMLDSAAlgorithm               { get; set; } = "ml_dsa_65";
        public DefaultMaxCSRCertificateLifeTimeDelegate  DefaultMaxCSRCertificateLifeTime    { get; set; } = (csr) => TimeSpan.FromDays(30);
        public DefaultMaxCertificateLifeTimeDelegate     DefaultMaxCertificateLifeTime       { get; set; } = (csr) => TimeSpan.FromDays(30);

        #endregion


        #region GenerateRSAKeyPair   (NumberOfBits    = 4096)

        /// <summary>
        /// Generate a new RSA key pair.
        /// </summary>
        /// <param name="NumberOfBits">The optional number of RSA bits to use.</param>
        public AsymmetricCipherKeyPair GenerateRSAKeyPair(UInt16? NumberOfBits = null)

            => PKIFactory.GenerateRSAKeyPair(NumberOfBits ?? DefaultRSASize);

        #endregion

        #region GenerateECCKeyPair   (ECCAlgorithm    = "secp256r1")

        /// <summary>
        /// Generate a new ECC key pair.
        /// </summary>
        /// <param name="ECCAlgorithm">The optional ECC curve to use.</param>
        public AsymmetricCipherKeyPair GenerateECCKeyPair(String? ECCAlgorithm = null)

            => PKIFactory.GenerateECCKeyPair(ECCAlgorithm ?? DefaultECCAlgorithm);

        #endregion

        #region GenerateMLKEMKeyPair (MLKEMAlgorithm  = "ml_kem_768")

        /// <summary>
        /// Generate a new ML-KEM key pair.
        /// </summary>
        /// <param name="MLKEMAlgorithm">The optional ML-KEM algorithm to use.</param>
        public AsymmetricCipherKeyPair GenerateMLKEMKeyPair(String? MLKEMAlgorithm = null)

            => PKIFactory.GenerateMLKEMKeyPair(MLKEMAlgorithm ?? DefaultMLKEMAlgorithm);

        #endregion

        #region GenerateMLDSAKeyPair (MLDSAAlgorithm  = "ml_kem_768")

        /// <summary>
        /// Generate a new ML-DSA key pair.
        /// </summary>
        /// <param name="DSAName">The optional ML-DSA algorithm to use.</param>
        public AsymmetricCipherKeyPair GenerateMLDSAKeyPair(String? MLDSAAlgorithm = null)

            => PKIFactory.GenerateMLKEMKeyPair(MLDSAAlgorithm ?? DefaultMLDSAAlgorithm);

        #endregion


        #region GenerateServerCSR(...)

        /// <summary>
        /// Generate a new Server Certificate Signing Request.
        /// </summary>
        /// <param name="PublicKey">The public key.</param>
        /// <param name="KeyGroupId">An optional key group identification. CSRs with the same keyGroupId replace each other.</param>
        /// <param name="KeySerialNumber">An optional key serial number.</param>
        /// <param name="NotBefore">An optional timestamp when the final certificate should become valid.</param>
        /// <param name="NotAfter">An optional timestamp when the final certificate should expire.</param>
        /// 
        /// <param name="PartyIds"></param>
        /// <param name="SubCPOIds"></param>
        /// <param name="SubEMSPIds"></param>
        /// 
        /// <param name="CommonName"></param>
        /// <param name="Organization"></param>
        /// <param name="OrganizationalUnit"></param>
        /// <param name="Street"></param>
        /// <param name="PostalCode"></param>
        /// <param name="Locality"></param>
        /// <param name="Country"></param>
        /// <param name="State"></param>
        /// <param name="TelephoneNumber"></param>
        /// <param name="EMailAddress"></param>
        /// <param name="Description"></param>
        public CSRInfo GenerateServerCSR(AsymmetricKeyParameter  PublicKey,
                                         String?                 KeyGroupId           = null,
                                         String?                 KeySerialNumber      = null,
                                         DateTimeOffset?         NotBefore            = null,
                                         DateTimeOffset?         NotAfter             = null,

                                         IEnumerable<String>?    PartyIds             = null,
                                         IEnumerable<String>?    SubCPOIds            = null,
                                         IEnumerable<String>?    SubEMSPIds           = null,

                                         String?                 CommonName           = null,
                                         String?                 Organization         = null,
                                         String?                 OrganizationalUnit   = null,
                                         String?                 Street               = null,
                                         String?                 PostalCode           = null,
                                         String?                 Locality             = null,
                                         String?                 Country              = null,
                                         String?                 State                = null,
                                         String?                 TelephoneNumber      = null,
                                         String?                 EMailAddress         = null,
                                         String?                 Description          = null)

        {

            #region Create the Subject

            var oidList    = new List<DerObjectIdentifier>();
            var valueList  = new List<String>();

            if (CommonName is not null)
            {
                oidList.  Add(X509Name.CN);
                valueList.Add(CommonName);
            }

            if (Organization is not null)
            {
                oidList.  Add(X509Name.O);
                valueList.Add(Organization);
            }

            if (OrganizationalUnit is not null)
            {
                oidList.  Add(X509Name.OU);
                valueList.Add(OrganizationalUnit);
            }

            if (Street is not null)
            {
                oidList.  Add(X509Name.Street);
                valueList.Add(Street);
            }

            if (PostalCode is not null)
            {
                oidList.  Add(X509Name.PostalCode);
                valueList.Add(PostalCode);
            }

            if (Country is not null)
            {
                oidList.  Add(X509Name.C);
                valueList.Add(Country);
            }

            if (State is not null)
            {
                oidList.  Add(X509Name.ST);
                valueList.Add(State);
            }

            if (Locality is not null)
            {
                oidList.  Add(X509Name.L);
                valueList.Add(Locality);
            }

            if (TelephoneNumber is not null)
            {
                oidList.  Add(X509Name.TelephoneNumber);
                valueList.Add(TelephoneNumber);
            }

            if (EMailAddress is not null)
            {
                oidList.  Add(X509Name.EmailAddress);
                valueList.Add(EMailAddress);
            }

            if (Description is not null)
            {
                oidList.  Add(X509Name.Description);
                valueList.Add(Description);
            }

            var subject = new X509Name(oidList, valueList);

            #endregion

            #region Set optional attributes

            var csrAttributes = new List<Org.BouncyCastle.Asn1.Cms.Attribute>();

            if (KeyGroupId.SafeAny())
                csrAttributes.Add(
                    new Org.BouncyCastle.Asn1.Cms.Attribute(
                        new DerObjectIdentifier(KeyGroupIdAttribute),
                        new DerSet(new DerUtf8String(KeyGroupId))
                    )
                );

            if (KeySerialNumber.SafeAny())
                csrAttributes.Add(
                    new Org.BouncyCastle.Asn1.Cms.Attribute(
                        X509Name.SerialNumber,
                        new DerSet(new DerUtf8String(KeySerialNumber))
                    )
                );

            if (NotBefore.HasValue)
                csrAttributes.Add(
                    new Org.BouncyCastle.Asn1.Cms.Attribute(
                        new DerObjectIdentifier(NotBeforeAttribute),
                        new DerSet(new DerUtcTime(NotBefore.Value.DateTime, 2049)) // 2049 for X.509 standards
                    )
                );

            if (NotAfter.HasValue)
                csrAttributes.Add(
                    new Org.BouncyCastle.Asn1.Cms.Attribute(
                        new DerObjectIdentifier(NotAfterAttribute),
                        new DerSet(new DerUtcTime(NotAfter.Value.DateTime, 2049)) // 2049 for X.509 standards
                    )
                );


            if (PartyIds.SafeAny())
                csrAttributes.Add(
                    new Org.BouncyCastle.Asn1.Cms.Attribute(
                        new DerObjectIdentifier(PartyIdAttribute),
                        new DerSet(PartyIds.Select(partyId => new DerUtf8String(partyId)).ToArray())
                    )
                );

            if (SubCPOIds.SafeAny())
                csrAttributes.Add(
                    new Org.BouncyCastle.Asn1.Cms.Attribute(
                        new DerObjectIdentifier(SubCPOsIdAttribute),
                        new DerSet(SubCPOIds.Select(subCPOId => new DerUtf8String(subCPOId)).ToArray())
                    )
                );

            if (SubEMSPIds.SafeAny())
                csrAttributes.Add(
                    new Org.BouncyCastle.Asn1.Cms.Attribute(
                        new DerObjectIdentifier(SubEMSPsIdAttribute),
                        new DerSet(SubEMSPIds.Select(subEMSPId => new DerUtf8String(subEMSPId)).ToArray())
                    )
                );

            #endregion

            #region (Extended)KeyUsage Extension

            var extgen = new X509ExtensionsGenerator();

            extgen.AddExtension(
                X509Extensions.KeyUsage,
                true,
                new KeyUsage(KeyUsage.DigitalSignature | KeyUsage.KeyEncipherment)
            );

            //extgen.AddExtension(
            //    X509Extensions.ExtendedKeyUsage,
            //    true, // false would be more standard conform, but most CAs don't care...
            //    new ExtendedKeyUsage(KeyPurposeID.id_kp_clientAuth)
            //);

            #endregion

            #region Subject Alternative Names

            extgen.AddExtension(
                X509Extensions.SubjectAlternativeName,
                true, // false would be more standard conform, but most CAs don't care...
                new DerOctetString(
                    new GeneralNames([
                        new (GeneralName.DnsName,                     CommonName),
                        new (GeneralName.IPAddress,                  "192.168.10.10"),
                        new (GeneralName.Rfc822Name,                 "admin@example.com"),
                        new (GeneralName.Rfc822Name,                 "tech@example.com"),
                        new (GeneralName.UniformResourceIdentifier,  "https://example.com/api/"),
                        new (GeneralName.UniformResourceIdentifier,  "urn:acme:device:1234")
                    ]).GetEncoded()
                )
            );

            #endregion

            #region Check max CSR Certificate Lifetime

            csrAttributes.Add(
                new Org.BouncyCastle.Asn1.Cms.Attribute(
                    PkcsObjectIdentifiers.Pkcs9AtExtensionRequest,
                    new DerSet(extgen.Generate())
                )
            );

            #endregion

            var csrInfo = new CSRInfo(
                              PublicKey:         PublicKey,
                              Subject:           subject,
                              //ParsedSubject:     [],
                              Attributes:        new DerSet(csrAttributes.ToArray()),
                              //ParsedAttributes:  [],
                              KeyId:             KeyGroupId,
                              NotBefore:         NotBefore,
                              NotAfter:          NotAfter,
                              PartyIds:          PartyIds,
                              SubCPOIds:         SubCPOIds,
                              SubEMSPIds:        SubEMSPIds
                          );

            if (NotAfter.HasValue && NotAfter.Value - (NotBefore ?? Timestamp.Now) > DefaultMaxCSRCertificateLifeTime(csrInfo))
                throw new ArgumentException("The requested CSR certificate lifetime is too long!");


            //var signatureFactory    = new Asn1SignatureFactory(
            //                              KeyPair.Private is RsaKeyParameters
            //                                  ? PkcsObjectIdentifiers.Sha256WithRsaEncryption.Id
            //                                  : X9ObjectIdentifiers.  ECDsaWithSha256.Id,
            //                              KeyPair.Private,
            //                              new SecureRandom()
            //                          );

            //var csrGenerator        = new Pkcs10CertificationRequest(
            //                              signatureFactory,
            //                              subject,
            //                              KeyPair.Public,
            //                              new DerSet(csrAttributes.ToArray())
            //                          );

            return csrInfo;

        }

        #endregion

        #region GenerateClientCSR(...)

        /// <summary>
        /// Generate a new Server Certificate Signing Request.
        /// </summary>
        /// <param name="PublicKey">The public key.</param>
        /// <param name="KeyGroupId">An optional key group identification. CSRs with the same keyGroupId replace each other.</param>
        /// <param name="KeySerialNumber">An optional key serial number.</param>
        /// <param name="NotBefore">An optional timestamp when the final certificate should become valid.</param>
        /// <param name="NotAfter">An optional timestamp when the final certificate should expire.</param>
        /// 
        /// <param name="PartyIds"></param>
        /// <param name="SubCPOIds"></param>
        /// <param name="SubEMSPIds"></param>
        /// 
        /// <param name="CommonName"></param>
        /// <param name="Organization"></param>
        /// <param name="OrganizationalUnit"></param>
        /// <param name="Street"></param>
        /// <param name="PostalCode"></param>
        /// <param name="Locality"></param>
        /// <param name="Country"></param>
        /// <param name="State"></param>
        /// <param name="TelephoneNumber"></param>
        /// <param name="EMailAddress"></param>
        /// <param name="Description"></param>
        public CSRInfo GenerateClientCSR(AsymmetricKeyParameter  PublicKey,
                                         String?                 KeyGroupId           = null,
                                         String?                 KeySerialNumber      = null,
                                         DateTimeOffset?         NotBefore            = null,
                                         DateTimeOffset?         NotAfter             = null,

                                         IEnumerable<String>?    PartyIds             = null,
                                         IEnumerable<String>?    SubCPOIds            = null,
                                         IEnumerable<String>?    SubEMSPIds           = null,

                                         String?                 CommonName           = null,
                                         String?                 Organization         = null,
                                         String?                 OrganizationalUnit   = null,
                                         String?                 Street               = null,
                                         String?                 PostalCode           = null,
                                         String?                 Locality             = null,
                                         String?                 Country              = null,
                                         String?                 State                = null,
                                         String?                 TelephoneNumber      = null,
                                         String?                 EMailAddress         = null,
                                         String?                 Description          = null)

        {

            #region Create the Subject

            var oidList    = new List<DerObjectIdentifier>();
            var valueList  = new List<String>();

            if (CommonName is not null)
            {
                oidList.  Add(X509Name.CN);
                valueList.Add(CommonName);
            }

            if (Organization is not null)
            {
                oidList.  Add(X509Name.O);
                valueList.Add(Organization);
            }

            if (OrganizationalUnit is not null)
            {
                oidList.  Add(X509Name.OU);
                valueList.Add(OrganizationalUnit);
            }

            if (Street is not null)
            {
                oidList.  Add(X509Name.Street);
                valueList.Add(Street);
            }

            if (PostalCode is not null)
            {
                oidList.  Add(X509Name.PostalCode);
                valueList.Add(PostalCode);
            }

            if (Country is not null)
            {
                oidList.  Add(X509Name.C);
                valueList.Add(Country);
            }

            if (State is not null)
            {
                oidList.  Add(X509Name.ST);
                valueList.Add(State);
            }

            if (Locality is not null)
            {
                oidList.  Add(X509Name.L);
                valueList.Add(Locality);
            }

            if (TelephoneNumber is not null)
            {
                oidList.  Add(X509Name.TelephoneNumber);
                valueList.Add(TelephoneNumber);
            }

            if (EMailAddress is not null)
            {
                oidList.  Add(X509Name.EmailAddress);
                valueList.Add(EMailAddress);
            }

            if (Description is not null)
            {
                oidList.  Add(X509Name.Description);
                valueList.Add(Description);
            }

            var subject = new X509Name(
                              oidList,
                              valueList
                          );

            #endregion

            #region Set optional attributes

            var csrAttributes = new List<Org.BouncyCastle.Asn1.Cms.Attribute>();

            if (KeyGroupId.SafeAny())
                csrAttributes.Add(
                    new Org.BouncyCastle.Asn1.Cms.Attribute(
                        new DerObjectIdentifier(KeyGroupIdAttribute),
                        new DerSet(new DerUtf8String(KeyGroupId))
                    )
                );

            if (KeySerialNumber.SafeAny())
                csrAttributes.Add(
                    new Org.BouncyCastle.Asn1.Cms.Attribute(
                        X509Name.SerialNumber,
                        new DerSet(new DerUtf8String(KeySerialNumber))
                    )
                );

            if (NotBefore.HasValue)
                csrAttributes.Add(
                    new Org.BouncyCastle.Asn1.Cms.Attribute(
                        new DerObjectIdentifier(NotBeforeAttribute),
                        new DerSet(new DerUtcTime(NotBefore.Value.DateTime, 2049)) // 2049 for X.509 standards
                    )
                );

            if (NotAfter.HasValue)
                csrAttributes.Add(
                    new Org.BouncyCastle.Asn1.Cms.Attribute(
                        new DerObjectIdentifier(NotAfterAttribute),
                        new DerSet(new DerUtcTime(NotAfter.Value.DateTime, 2049)) // 2049 for X.509 standards
                    )
                );


            if (PartyIds.SafeAny())
                csrAttributes.Add(
                    new Org.BouncyCastle.Asn1.Cms.Attribute(
                        new DerObjectIdentifier(PartyIdAttribute),
                        new DerSet(PartyIds.Select(partyId => new DerUtf8String(partyId)).ToArray())
                    )
                );

            if (SubCPOIds.SafeAny())
                csrAttributes.Add(
                    new Org.BouncyCastle.Asn1.Cms.Attribute(
                        new DerObjectIdentifier(SubCPOsIdAttribute),
                        new DerSet(SubCPOIds.Select(subCPOId => new DerUtf8String(subCPOId)).ToArray())
                    )
                );

            if (SubEMSPIds.SafeAny())
                csrAttributes.Add(
                    new Org.BouncyCastle.Asn1.Cms.Attribute(
                        new DerObjectIdentifier(SubEMSPsIdAttribute),
                        new DerSet(SubEMSPIds.Select(subEMSPId => new DerUtf8String(subEMSPId)).ToArray())
                    )
                );

            #endregion


            var extgen = new X509ExtensionsGenerator();

            #region (Extended)KeyUsage Extension

            extgen.AddExtension(
                X509Extensions.KeyUsage,
                true,
                new KeyUsage(KeyUsage.DigitalSignature | KeyUsage.KeyEncipherment)
            );

            extgen.AddExtension(
                X509Extensions.ExtendedKeyUsage,
                true, // false would be more standard conform, but most CAs don't care...
                new ExtendedKeyUsage(KeyPurposeID.id_kp_clientAuth)
            );

            #endregion

            #region Subject Alternative Names

            extgen.AddExtension(
                X509Extensions.SubjectAlternativeName,
                true, // false would be more standard conform, but most CAs don't care...
                new DerOctetString(
                    new GeneralNames([
                        new (GeneralName.DnsName,                     CommonName),
                        new (GeneralName.IPAddress,                  "192.168.10.10"),
                        new (GeneralName.Rfc822Name,                 "admin@example.com"),
                        new (GeneralName.Rfc822Name,                 "tech@example.com"),
                        new (GeneralName.UniformResourceIdentifier,  "https://example.com/api/"),
                        new (GeneralName.UniformResourceIdentifier,  "urn:acme:device:1234")
                    ]).GetEncoded()
                )
            );

            #endregion

            #region Check max CSR Certificate Lifetime

            csrAttributes.Add(
                new Org.BouncyCastle.Asn1.Cms.Attribute(
                    PkcsObjectIdentifiers.Pkcs9AtExtensionRequest,
                    new DerSet(extgen.Generate())
                )
            );

            #endregion


            var csrInfo = new CSRInfo(
                              PublicKey:         PublicKey,
                              Subject:           subject,
                              //ParsedSubject:     [],
                              Attributes:        new DerSet(csrAttributes.ToArray()),
                              //ParsedAttributes:  [],
                              KeyId:             KeyGroupId,
                              NotBefore:         NotBefore,
                              NotAfter:          NotAfter,
                              PartyIds:          PartyIds,
                              SubCPOIds:         SubCPOIds,
                              SubEMSPIds:        SubEMSPIds
                          );

            if (NotAfter.HasValue && NotAfter.Value - (NotBefore ?? Timestamp.Now) > DefaultMaxCSRCertificateLifeTime(csrInfo))
                throw new ArgumentException("The requested CSR certificate lifetime is too long!");

            return csrInfo;

        }

        #endregion


        #region ParsePEMEncodedCSR(CSR_PEMEncoded)

        public CSRInfo ParsePEMEncodedCSR(String CSR_PEMEncoded)
        {

            if (TryParsePEMEncodedCSR(CSR_PEMEncoded,
                                      out var parsedCSR,
                                      out var errorResponse))
            {
                return parsedCSR;
            }

            throw new ArgumentException("The given PEM-encoded certificate signing request is invalid: " + errorResponse,
                                        nameof(CSR_PEMEncoded));

        }

        #endregion

        #region TryParsePEMEncodedCSR(CSR_PEMEncoded)

        /// <summary>
        /// Try to parse the given PEM-encoded certificate signing request.
        /// </summary>
        /// <param name="CSR_PEMEncoded">A PEM-encoded certificate signing request</param>
        public CSRInfo? TryParsePEMEncodedCSR(String CSR_PEMEncoded)
        {

            if (TryParsePEMEncodedCSR(CSR_PEMEncoded,
                                      out var parsedCSR,
                                      out _))
            {
                return parsedCSR;
            }

            return null;

        }

        #endregion

        #region TryParsePEMEncodedCSR(CSR_PEMEncoded, out ParsedCSR, out ErrorResponse)

        /// <summary>
        /// Try to parse the given PEM-encoded certificate signing request.
        /// </summary>
        /// <param name="CSR_PEMEncoded">A PEM-encoded certificate signing request</param>
        /// <param name="ParsedCSR">The parsed certificate signing request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public Boolean TryParsePEMEncodedCSR(String                             CSR_PEMEncoded,
                                             [NotNullWhen(true)]  out CSRInfo?  ParsedCSR,
                                             [NotNullWhen(false)] out String?   ErrorResponse)
        {

            ParsedCSR      = null;
            ErrorResponse  = null;

            try
            {

                Pkcs10CertificationRequest parsedCSR;
                using (var strReader = new StringReader(CSR_PEMEncoded))
                {
                    var pemReader = new PemReader(strReader);
                    parsedCSR = (Pkcs10CertificationRequest) pemReader.ReadObject();
                }

                var reqInfo           = parsedCSR.GetCertificationRequestInfo();

                var subjectFields     = new Dictionary<String, String>();
                var oidList           = reqInfo.Subject.GetOidList();
                var valueList         = reqInfo.Subject.GetValueList();

                for (var i = 0; i < oidList.Count; i++)
                {
                    var oid   = (DerObjectIdentifier) oidList[i];
                    var value = valueList[i].ToString();
                    subjectFields[oid.Id] = value;
                }


                #region Parse CSR Attributes

                var parsedAttributes  = new Dictionary<String, ParsedAttribute>();

                if (reqInfo.Attributes is not null)
                {
                    foreach (var asn1Enc in reqInfo.Attributes)
                    {

                        var bcAttr       = Org.BouncyCastle.Asn1.Cms.Attribute.GetInstance(asn1Enc);
                        var oid          = bcAttr.AttrType;
                        var asn1ValueSet = bcAttr.AttrValues;
                        var values       = new List<String>();

                        foreach (var asn1Value in asn1ValueSet)
                        {

                            if (asn1Value is DLSequence dlSequence)
                            {
                                foreach (var element in dlSequence)
                                {

                                    var text = element?.ToString();

                                    if (text is not null)
                                        values.Add(text);

                                }
                            }

                            else if (asn1Value is not null)
                            {

                                var text = asn1Value?.ToString();

                                if (text is not null)
                                    values.Add(text);

                            }

                        }

                        parsedAttributes.Add(
                            oid.Id,
                            new ParsedAttribute(
                                OID:          oid.Id,
                                DecodedData:  values
                            )
                        );

                    }
                }

                #endregion

                ParsedCSR = new CSRInfo(
                                Subject:           reqInfo.Subject,
                                ParsedSubject:     subjectFields,
                                PublicKey:         parsedCSR.GetPublicKey(),
                                Attributes:        reqInfo.Attributes,
                                ParsedAttributes:  parsedAttributes.Values,
                                PartyIds:          parsedAttributes.TryGet(PartyIdAttribute)?.   DecodedData ?? [],
                                SubCPOIds:         parsedAttributes.TryGet(SubCPOsIdAttribute)?. DecodedData ?? [],
                                SubEMSPIds:        parsedAttributes.TryGet(SubEMSPsIdAttribute)?.DecodedData ?? []
                            );

                return true;

            }
            catch (Exception e)
            {
                ErrorResponse = "Could not parse the given PEM-encoded certificate signing request: " + e.Message;
                return false;
            }

        }

        #endregion


        #region GenerateCertificate(CertificateType, SubjectName, SubjectKeyPair, Issuer = null, LifeTime = null)

        /// <summary>
        /// Generate a new certificate.
        /// </summary>
        /// <param name="SubjectName">A friendly name for the owner of the crypto keys.</param>
        /// <param name="SubjectKeyPair">The crypto keys.</param>
        /// <param name="Issuer">The (optional) crypto key pair signing this certificate. Optional means that this certificate will be self-signed!</param>
        /// <param name="LifeTime">The life time of the certificate.</param>
        public static X509Certificate

            GenerateCertificate(String                                           SubjectName,
                                AsymmetricCipherKeyPair                          SubjectKeyPair,
                                Tuple<AsymmetricKeyParameter, X509Certificate>?  Issuer     = null,
                                TimeSpan?                                        LifeTime   = null)

        {

            var now           = Timestamp.Now;
            var x509v3        = new X509V3CertificateGenerator();

            x509v3.SetSerialNumber(BigIntegers.CreateRandomInRange(BigInteger.One, BigInteger.ValueOf(Int64.MaxValue), new SecureRandom()));
            x509v3.SetSubjectDN   (new X509Name($"CN={SubjectName}, O=GraphDefined GmbH, OU=GraphDefined PKI Services"));
            x509v3.SetPublicKey   (SubjectKeyPair.Public);
            x509v3.SetNotBefore   (now.DateTime);
            x509v3.SetNotAfter   ((now + (LifeTime ?? TimeSpan.FromDays(365))).DateTime);

            if (Issuer is null)
                x509v3.SetIssuerDN(new X509Name($"CN={SubjectName}")); // self-signed

            else
            {

                x509v3.SetIssuerDN (Issuer.Item2.SubjectDN);

                x509v3.AddExtension(
                    X509Extensions.AuthorityKeyIdentifier.Id,
                    false,
                    X509ExtensionUtilities.CreateAuthorityKeyIdentifier(Issuer.Item2)
                    //new AuthorityKeyIdentifierStructure(Issuer.Item2)
                );

            }

            // https://jamielinux.com/docs/openssl-certificate-authority/appendix/root-configuration-file.html
            // https://jamielinux.com/docs/openssl-certificate-authority/appendix/intermediate-configuration-file.html

            // Set Key Usage for client certificates
            x509v3.AddExtension(
                X509Extensions.KeyUsage.Id,
                true,
                new KeyUsage(
                    KeyUsage.NonRepudiation   |
                    KeyUsage.DigitalSignature |
                    KeyUsage.KeyEncipherment
                )
            );

            // Set Extended Key Usage for client authentication
            x509v3.AddExtension(X509Extensions.ExtendedKeyUsage.Id,
                                false,
                                new ExtendedKeyUsage(KeyPurposeID.id_kp_clientAuth));

            //x509v3.AddExtension(X509Extensions.ExtendedKeyUsage.Id,
            //                    false,
            //                    new ExtendedKeyUsage(KeyPurposeID.id_kp_serverAuth));


            return x509v3.Generate(
                new Asn1SignatureFactory(
                    SubjectKeyPair.Public is RsaKeyParameters
                        ? "SHA256WITHRSA"
                        : "SHA256WITHECDSA",
                    Issuer?.Item1 ?? SubjectKeyPair.Private
                )
            );

        }

        #endregion

        #region SignCSR(CertificateType, SubjectName, SubjectKeyPair, Issuer = null, LifeTime = null)

        /// <summary>
        /// Sign an incoming certificate signing request.
        /// </summary>
        /// <param name="CertificateType">The type of the certificate.</param>
        /// <param name="SubjectName">A friendly name for the owner of the crypto keys.</param>
        /// <param name="SubjectKeyPair">The crypto keys.</param>
        /// <param name="Issuer">The (optional) crypto key pair signing this certificate. Optional means that this certificate will be self-signed!</param>
        /// <param name="LifeTime">The life time of the certificate.</param>
        public static X509Certificate

            SignCSR(CSRInfo                 CSR,
                    X509Certificate         IssuerCertificate,
                    AsymmetricKeyParameter  IssuerPrivateKey,
                    TimeSpan?               LifeTime   = null)

        {

            var now           = Timestamp.Now;
            var x509v3        = new X509V3CertificateGenerator();

            x509v3.SetSerialNumber(BigIntegers.CreateRandomInRange(BigInteger.One, BigInteger.ValueOf(Int64.MaxValue), new SecureRandom()));
            x509v3.SetSubjectDN   (new X509Name($"CN={CSR.ParsedSubject["CN"]}, O=GraphDefined GmbH, OU=GraphDefined PKI Services"));
            x509v3.SetPublicKey   (CSR.PublicKey);
            x509v3.SetNotBefore   (now.DateTime);
            x509v3.SetNotAfter   ((now + (LifeTime ?? TimeSpan.FromDays(365))).DateTime);

            x509v3.SetIssuerDN    (IssuerCertificate.SubjectDN);

            x509v3.AddExtension   (
                X509Extensions.AuthorityKeyIdentifier.Id,
                false,
                X509ExtensionUtilities.CreateAuthorityKeyIdentifier(IssuerCertificate)
                //new AuthorityKeyIdentifierStructure(IssuerCertificate)
            );

            // Set Key Usage for client certificates
            x509v3.AddExtension(
                X509Extensions.KeyUsage.Id,
                true,
                new KeyUsage(
                    KeyUsage.NonRepudiation   |
                    KeyUsage.DigitalSignature |
                    KeyUsage.KeyEncipherment
                )
            );

            // Set Extended Key Usage for client authentication
            x509v3.AddExtension(
                X509Extensions.ExtendedKeyUsage.Id,
                false,
                new ExtendedKeyUsage(KeyPurposeID.id_kp_clientAuth)
            );

            return x509v3.Generate(
                new Asn1SignatureFactory(
                    CSR.PublicKey is RsaKeyParameters
                        ? "SHA256WITHRSA"
                        : "SHA256WITHECDSA",
                    IssuerPrivateKey
                )
            );

        }

        #endregion



        public X509Certificate? SignServerCertificate(Pkcs10CertificationRequest  csr,
                                                      X509Name                    issuer,
                                                      AsymmetricKeyParameter      issuerPrivateKey)
        {

            try
            {

                if (!csr.Verify())
                    throw new InvalidOperationException("CSR signature verification failed!");

                var csrInfo = csr.GetCertificationRequestInfo();
                var subject = csrInfo.Subject;
                var clientPublicKey = csr.GetPublicKey();

                // === CORRECT SAN EXTRACTION (BouncyCastle 2.2+) ===
                //GeneralNames? subjectAltNames = null;

                //foreach (AttributePkcs attr in csrInfo.Attributes)
                //{
                //    if (attr.AttrType.Equals(PkcsObjectIdentifiers.Pkcs9AtExtensionRequest))
                //    {
                //        var set = (Asn1Set)attr.AttrValues[0];
                //        var extensions = Extensions.GetInstance(set);

                //        if (extensions.GetExtensionOids().Contains(X509Extensions.SubjectAlternativeName))
                //        {
                //            subjectAltNames = GeneralNames.FromExtensions(
                //                extensions, X509Extensions.SubjectAlternativeName);
                //        }
                //        break;
                //    }
                //}

                //// Build SAN list
                //var sanList = new List<GeneralName>();
                //if (subjectAltNames != null)
                //{
                //    foreach (var name in subjectAltNames.GetNames())
                //        sanList.Add(name);
                //}

                // Fallback: use CN as DNS name (legacy)
                //if (sanList.Count == 0)
                //{
                //    var cn = subject.GetValueList(X509Name.CN);
                //    if (cn.Count > 0)
                //    {
                //        sanList.Add(new GeneralName(GeneralName.DnsName, cn[0].ToString()));
                //        Console.WriteLine("Warning: No SAN in CSR, using CN as DNS name (deprecated)");
                //    }
                //}

                //if (sanList.Count == 0)
                //    throw new InvalidOperationException("No DNS name found in CSR (SAN or CN required)");

                // === Certificate generation ===
                var serialNumber = new BigInteger(64, new SecureRandom()); // Better randomness

                var certGen = new X509V3CertificateGenerator();
                certGen.SetSerialNumber(serialNumber);
                certGen.SetIssuerDN(issuer);
                certGen.SetSubjectDN(subject);
                certGen.SetNotBefore(DateTime.UtcNow.AddHours(-2));
                certGen.SetNotAfter(DateTime.UtcNow.AddYears(1));
                certGen.SetPublicKey(clientPublicKey);

                string sigAlg = issuerPrivateKey is RsaKeyParameters
                    ? "SHA384withRSA"
                    : issuerPrivateKey is ECPrivateKeyParameters
                        ? "SHA384withECDSA"
                        : throw new NotSupportedException($"Key type not supported: {issuerPrivateKey.GetType().Name}");

                var sigFactory = new Asn1SignatureFactory(sigAlg, issuerPrivateKey);

                // Extensions
                certGen.AddExtension(X509Extensions.BasicConstraints, true, new BasicConstraints(false));
                certGen.AddExtension(X509Extensions.KeyUsage, true,
                    new KeyUsage(KeyUsage.DigitalSignature | KeyUsage.KeyEncipherment));
                certGen.AddExtension(X509Extensions.ExtendedKeyUsage, false,
                    new ExtendedKeyUsage(KeyPurposeID.IdKPServerAuth));
                //certGen.AddExtension(X509Extensions.SubjectAlternativeName, false,
                //    new GeneralNames(sanList.ToArray()));
                certGen.AddExtension(X509Extensions.SubjectKeyIdentifier, false,
                    new SubjectKeyIdentifierStructure(clientPublicKey));

                //// Authority Key Identifier
                //AuthorityKeyIdentifier aki = issuerCertificate != null
                //    ? new AuthorityKeyIdentifierStructure(issuerCertificate)
                //    : new AuthorityKeyIdentifierStructure(DotNetUtilities.GetKeyPair(issuerPrivateKey).Public);

                //certGen.AddExtension(X509Extensions.AuthorityKeyIdentifier, false, aki);

                var certificate = certGen.Generate(sigFactory);
                //certificate.Verify(issuerPrivateKey.Public);

                return certificate;


            }
            catch (Exception ex)
            {
                // Log exception as needed
                Console.Error.WriteLine($"Failed to sign certificate: {ex.Message}");
                return null;
            }

        }

        public X509Certificate? SignClientCertificate(Pkcs10CertificationRequest  csr,
                                                      X509Name                    issuer,
                                                      AsymmetricKeyParameter      issuerPrivateKey)
        {

            try
            {

                if (!csr.Verify())
                    throw new InvalidOperationException("CSR signature verification failed!");

                // Extract subject and public key from CSR
                X509Name subject = csr.GetCertificationRequestInfo().Subject;
                AsymmetricKeyParameter clientPublicKey = csr.GetPublicKey();

                // Generate a serial number (must be unique per CA)
                BigInteger serialNumber = BigInteger.ValueOf(DateTime.UtcNow.Ticks + new Random().Next(1, 10000));

                // Create certificate generator
                X509V3CertificateGenerator certGen = new X509V3CertificateGenerator();

                // Set issuer and subject
                certGen.SetIssuerDN(issuer);
                certGen.SetSubjectDN(subject);

                // Set validity period (e.g., 1 year)
                var notBefore  = Timestamp.Now.AddDays(-1); // Slight backdating to avoid clock skew issues
                var notAfter   = notBefore.AddYears(1);

                certGen.SetNotBefore(notBefore.DateTime);
                certGen.SetNotAfter (notAfter. DateTime);

                // Set serial number and public key
                certGen.SetSerialNumber(serialNumber);
                certGen.SetPublicKey(clientPublicKey);

                // Set signature algorithm (match issuer key type)
                string signatureAlgorithm = "";

                // Determine signature algorithm based on issuer key type
                if (issuerPrivateKey is RsaKeyParameters)
                {
                    signatureAlgorithm = "SHA256withRSA";
                }
                else if (issuerPrivateKey is ECPrivateKeyParameters ecPrivate)
                {
                    signatureAlgorithm = "SHA256withECDSA";
                }
                else if (issuerPrivateKey is ECPublicKeyParameters ecPublic)
                {
                    throw new InvalidOperationException($"Expected private key for signing, but got public key: {issuerPrivateKey.GetType().Name}. Use the corresponding ECPrivateKeyParameters.");
                }
                else
                {
                    throw new NotSupportedException($"Unsupported private key type: {issuerPrivateKey.GetType().Name}");
                }


                var sigFactory = new Asn1SignatureFactory(signatureAlgorithm, issuerPrivateKey);

                // Optional: Add basic extensions (recommended for client certs)

                // Basic Constraints: not a CA
                certGen.AddExtension(
                    X509Extensions.BasicConstraints.Id,
                    true,
                    new BasicConstraints(false));

                // Key Usage: Digital Signature + Key Encipherment (typical for client auth)
                certGen.AddExtension(
                    X509Extensions.KeyUsage.Id,
                    true,
                    new KeyUsage(KeyUsage.DigitalSignature | KeyUsage.KeyEncipherment | KeyUsage.NonRepudiation));

                // Extended Key Usage: Client Authentication
                certGen.AddExtension(
                    X509Extensions.ExtendedKeyUsage.Id,
                    false,
                    new ExtendedKeyUsage(KeyPurposeID.id_kp_clientAuth));

                // Subject Key Identifier (optional but good practice)
                var skiGenerator = new SubjectKeyIdentifierStructure(clientPublicKey);
                certGen.AddExtension(
                    X509Extensions.SubjectKeyIdentifier.Id,
                    false,
                    skiGenerator);

                // Authority Key Identifier (if you have issuer cert)
                // You'd need issuer certificate to compute AKID properly
                // Skipping here for simplicity — can be added if needed

                // Generate the certificate using the signature factory
                X509Certificate certificate = certGen.Generate(sigFactory);

                // Optional: Verify the generated certificate
                //certificate.Verify(issuerPrivateKey.Public); // This checks signature only

                return certificate;

            }
            catch (Exception ex)
            {
                // Log exception as needed
                Console.Error.WriteLine($"Failed to sign certificate: {ex.Message}");
                return null;
            }

        }


    }

}
