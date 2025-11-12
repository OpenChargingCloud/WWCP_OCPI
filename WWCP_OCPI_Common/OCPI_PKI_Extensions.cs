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

    public static class OCPI_PKI_Extensions
    {

        public static Pkcs10CertificationRequest ToCSR(this CSRInfo             CertificateSigningRequest,
                                                       AsymmetricCipherKeyPair  KeyPair)
        {

            var signatureFactory    = new Asn1SignatureFactory(
                                          KeyPair.Private is RsaKeyParameters
                                              ? PkcsObjectIdentifiers.Sha256WithRsaEncryption.Id
                                              : X9ObjectIdentifiers.  ECDsaWithSha256.Id,
                                          KeyPair.Private,
                                          new SecureRandom()
                                      );

            var csrGenerator        = new Pkcs10CertificationRequest(
                                          signatureFactory,
                                          CertificateSigningRequest.Subject,
                                          KeyPair.Public,
                                          new DerSet(
                                              CertificateSigningRequest.Attributes.ToArray()
                                          )
                                      );

            return csrGenerator;

        }

    }

}
