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

using Newtonsoft.Json.Linq;

using NUnit.Framework;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv3_0.UnitTests.SetupTests
{

    [TestFixture]
    public class SetupTests : A_2CPOs2EMSPs_TestDefaults
    {

        #region Add_ChargingLocationsAndEVSEs_Test1()

        /// <summary>
        /// Add WWCP charging locations, stations and EVSEs.
        /// Validate that they had been sent to the OCPI module.
        /// Validate via HTTP that they are present within the remote OCPI module.
        /// </summary>
        [Test]
        public async Task Add_ChargingLocationsAndEVSEs_Test1()
        {

            if (cpo1CPOAPI   is not null &&
                emsp1EMSPAPI is not null)
            {

                var pki           = new OCPI_PKI();

                var cpo1KeyPair   = pki.GenerateECCKeyPair();
                var cpo1CSR       = pki.GenerateServerCSR (PublicKey:            cpo1KeyPair.Public,
                                                           KeySerialNumber:      UUIDv7.Generate().ToString(),
                                                           PartyIds:             ["DEGEF", "DEGE2"],
                                                           SubCPOIds:            ["DE*GEF", "DE*GE2"],
                                                           NotBefore:            Timestamp.Now - TimeSpan.FromDays(1),
                                                           NotAfter:             Timestamp.Now + TimeSpan.FromDays(13),
                                                           CommonName:           "open.charging.cloud",
                                                           Organization:         "GraphDefined GmbH",
                                                           OrganizationalUnit:   "CPO Services",
                                                           EMailAddress:         "roaming-cpo@charging.cloud",
                                                           TelephoneNumber:      "+49 1234 56789012",
                                                           PostalCode:           "07749",
                                                           Locality:             "Jena",
                                                           Country:              "DE",
                                                           Description:          "Main CPO key...");

                var emsp1KeyPair  = pki.GenerateECCKeyPair();
                var emsp1CSR      = pki.GenerateServerCSR (PublicKey:            emsp1KeyPair.Public,
                                                           KeySerialNumber:      UUIDv7.Generate().ToString(),
                                                           PartyIds:             ["DEGDF", "DEGD2"],
                                                           SubEMSPIds:           ["DE-GDF", "DE-GD2"],
                                                           NotBefore:            Timestamp.Now - TimeSpan.FromDays(1),
                                                           NotAfter:             Timestamp.Now + TimeSpan.FromDays(13),
                                                           CommonName:           "open.charging.cloud",
                                                           Organization:         "GraphDefined GmbH",
                                                           OrganizationalUnit:   "EMSP Services",
                                                           EMailAddress:         "roaming-emsp@charging.cloud",
                                                           TelephoneNumber:      "+49 5678 90123456",
                                                           PostalCode:           "07749",
                                                           Locality:             "Jena",
                                                           Country:              "DE",
                                                           Description:          "Main EMSP key...");


                // openssl req -in csr.txt -noout -text (will not print out the dates correctly!)
                // https://ssl-trust.com/SSL-Zertifikate/csr-decoder
                // https://lapo.it/asn1js
                //var pcsr1      = pki.ParsePEMEncodedCSR(cpo1CSR);
                //var pcsr2      = pki.ParsePEMEncodedCSR(emsp1CSR);

            }

        }

        #endregion

    }

}
