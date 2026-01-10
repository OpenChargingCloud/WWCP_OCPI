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

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;

#endregion

namespace cloud.charging.open.protocols.OCPI
{

    public delegate TimeSpan  DefaultMaxCSRCertificateLifeTimeDelegate (CSRInfo        CSR);
    public delegate TimeSpan  DefaultMaxCertificateLifeTimeDelegate    (CSRInfo        CSR);


    /// <summary>
    /// A structured object capturing all the data we care about from the CSR.
    /// </summary>
    public sealed class CSRInfo(AsymmetricKeyParameter         PublicKey,
                                X509Name                       Subject,
                                Dictionary<String, String>?    ParsedSubject      = null,
                                Asn1Set?                       Attributes         = null,
                                IEnumerable<ParsedAttribute>?  ParsedAttributes   = null,
                                String?                        KeyId              = null,
                                DateTimeOffset?                NotBefore          = null,
                                DateTimeOffset?                NotAfter           = null,
                                IEnumerable<String>?           PartyIds           = null,
                                IEnumerable<String>?           SubCPOIds          = null,
                                IEnumerable<String>?           SubEMSPIds         = null)
    {

        /// <summary>
        /// The subject public key of the CSR.
        /// </summary>
        public AsymmetricKeyParameter        PublicKey           { get; set; } = PublicKey;


        /// <summary>
        /// The Subject of the certificate signing request.
        /// </summary>
        public X509Name                      Subject             { get; set; } = Subject;

        /// <summary>
        /// Subject fields (e.g., CN, O, OU, etc.) as OID -> string.
        /// </summary>
        public Dictionary<String, String>    ParsedSubject       { get; set; } = ParsedSubject    ?? [];


        /// <summary>
        /// The attributes as ASN.1 set.
        /// </summary>
        public Asn1Set?                      Attributes          { get; set; } = Attributes;

        /// <summary>
        /// List of all attributes found in the CSR, with OIDs and decoded data.
        /// </summary>
        public IEnumerable<ParsedAttribute>  ParsedAttributes    { get; set; } = ParsedAttributes ?? [];


        public String?                       KeyId               { get; set; } = KeyId;
        public DateTimeOffset?               NotBefore           { get; set; } = NotBefore;
        public DateTimeOffset?               NotAfter            { get; set; } = NotAfter;

        public IEnumerable<String>           PartyIds            { get; set; } = PartyIds         ?? [];
        public IEnumerable<String>           SubCPOIds           { get; set; } = SubCPOIds        ?? [];
        public IEnumerable<String>           SubEMSPIds          { get; set; } = SubEMSPIds       ?? [];


    }

}
