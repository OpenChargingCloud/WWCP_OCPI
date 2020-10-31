/*
 * Copyright (c) 2015-2020 GraphDefined GmbH
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

using NUnit.Framework;
using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2.UnitTests
{

    /// <summary>
    /// Unit tests for connectors.
    /// https://github.com/ocpi/ocpi/blob/master/mod_locations.asciidoc#mod_locations_connector_object
    /// </summary>
    [TestFixture]
    public static class ConnectorTests
    {

        #region Connector_SerializeDeserialize_Test01()

        /// <summary>
        /// Connector serialize, deserialize and compare test.
        /// </summary>
        [Test]
        public static void Connector_SerializeDeserialize_Test01()
        {

            var Connector1 = new Connector(
                                 Connector_Id.Parse("1"),
                                 ConnectorTypes.IEC_62196_T2,
                                 ConnectorFormats.SOCKET,
                                 PowerTypes.AC_3_PHASE,
                                 400,
                                 30,
                                 12,
                                 new Tariff_Id[] {
                                     Tariff_Id.Parse("DE*GEF*T0001"),
                                     Tariff_Id.Parse("DE*GEF*T0002")
                                 },
                                 URL.Parse("https://open.charging.cloud/terms"),
                                 DateTime.Parse("2020-09-21T00:00:00Z")
                             );

            var JSON = Connector1.ToJSON();

            Assert.AreEqual("1",                                  JSON["id"].                  Value<String>());
            Assert.AreEqual("IEC_62196_T2",                       JSON["standard"].            Value<String>());
            Assert.AreEqual("SOCKET",                             JSON["format"].              Value<String>());
            Assert.AreEqual("AC_3_PHASE",                         JSON["power_type"].          Value<String>());
            Assert.AreEqual(400,                                  JSON["max_voltage"].         Value<UInt16>());
            Assert.AreEqual(30,                                   JSON["max_amperage"].        Value<UInt16>());
            Assert.AreEqual(12,                                   JSON["max_electric_power"].  Value<UInt16>());
            Assert.AreEqual("DE*GEF*T0001",                       JSON["tariff_ids"][0].       Value<String>());
            Assert.AreEqual("DE*GEF*T0002",                       JSON["tariff_ids"][1].       Value<String>());
            Assert.AreEqual("https://open.charging.cloud/terms",  JSON["terms_and_conditions"].Value<String>());
            Assert.AreEqual("2020-09-21T00:00:00.000Z",           JSON["last_updated"].        Value<String>());

            Assert.IsTrue(Connector.TryParse(JSON, out Connector Connector2, out String ErrorResponse));
            Assert.IsNull(ErrorResponse);

            Assert.AreEqual(new Tariff_Id[] {
                                Tariff_Id.Parse("DE*GEF*T0001"),
                                Tariff_Id.Parse("DE*GEF*T0002")
                            },
                            Connector2.TariffIds);

            Assert.AreEqual(Connector1.Id,                        Connector2.Id);
            Assert.AreEqual(Connector1.Standard,                  Connector2.Standard);
            Assert.AreEqual(Connector1.Format,                    Connector2.Format);
            Assert.AreEqual(Connector1.PowerType,                 Connector2.PowerType);
            Assert.AreEqual(Connector1.MaxVoltage,                Connector2.MaxVoltage);
            Assert.AreEqual(Connector1.MaxAmperage,               Connector2.MaxAmperage);
            Assert.AreEqual(Connector1.MaxElectricPower,          Connector2.MaxElectricPower);
            Assert.AreEqual(Connector1.TariffIds,                 Connector2.TariffIds);
            Assert.AreEqual(Connector1.TermsAndConditionsURL,     Connector2.TermsAndConditionsURL);
            Assert.AreEqual(Connector1.LastUpdated.ToIso8601(),   Connector2.LastUpdated.ToIso8601());

        }

        #endregion


        #region Connector_PATCH_ConnectorId()

        /// <summary>
        /// Try to PATCH the connector identification.
        /// </summary>
        [Test]
        public static void Connector_PATCH_ConnectorId()
        {

            var Connector1 = new Connector(
                                 Connector_Id.Parse("1"),
                                 ConnectorTypes.IEC_62196_T2,
                                 ConnectorFormats.SOCKET,
                                 PowerTypes.AC_3_PHASE,
                                 400,
                                 30,
                                 12,
                                 new Tariff_Id[] {
                                     Tariff_Id.Parse("DE*GEF*T0001"),
                                     Tariff_Id.Parse("DE*GEF*T0002")
                                 },
                                 URL.Parse("https://open.charging.cloud/terms"),
                                 DateTime.Parse("2020-09-21T00:00:00Z")
                             );

            var patchResult = Connector1.TryPatch(JObject.Parse(@"{ ""id"": ""2"", ""last_updated"": ""2020-10-15T00:00:00Z"" }"));

            Assert.IsNotNull(patchResult);
            Assert.IsFalse  (patchResult.IsSuccess);
            Assert.IsTrue   (patchResult.IsFailed);
            Assert.IsNotNull(patchResult.ErrorResponse);
            Assert.AreEqual ("Patching the 'identification' of a connector is not allowed!",  patchResult.ErrorResponse);
            Assert.IsNotNull(patchResult.PatchedData);

            Assert.AreEqual (Connector_Id.Parse("1"),                                         patchResult.PatchedData.Id);
            Assert.AreEqual (ConnectorTypes.IEC_62196_T2,                                     patchResult.PatchedData.Standard);
            Assert.AreEqual (ConnectorFormats.SOCKET,                                         patchResult.PatchedData.Format);
            Assert.AreEqual (PowerTypes.AC_3_PHASE,                                           patchResult.PatchedData.PowerType);
            Assert.AreEqual (400,                                                             patchResult.PatchedData.MaxVoltage);
            Assert.AreEqual (30,                                                              patchResult.PatchedData.MaxAmperage);
            Assert.AreEqual (12,                                                              patchResult.PatchedData.MaxElectricPower);
            Assert.AreEqual (2,                                                               patchResult.PatchedData.TariffIds.        Count());
            Assert.AreEqual (Tariff_Id.Parse("DE*GEF*T0001"),                                 patchResult.PatchedData.TariffIds.        First());
            Assert.AreEqual (Tariff_Id.Parse("DE*GEF*T0002"),                                 patchResult.PatchedData.TariffIds.Skip(1).First());
            Assert.AreEqual (URL.Parse("https://open.charging.cloud/terms"),                  patchResult.PatchedData.TermsAndConditionsURL);
            Assert.AreEqual ("2020-09-21T00:00:00.000Z",                                      patchResult.PatchedData.LastUpdated.ToIso8601());

        }

        #endregion

        #region Connector_PATCH_minimal()

        /// <summary>
        /// Minimal connector PATCH test.
        /// </summary>
        [Test]
        public static void Connector_PATCH_minimal()
        {

            var Connector1 = new Connector(
                                 Connector_Id.Parse("1"),
                                 ConnectorTypes.IEC_62196_T2,
                                 ConnectorFormats.SOCKET,
                                 PowerTypes.AC_3_PHASE,
                                 400,
                                 30,
                                 12,
                                 new Tariff_Id[] {
                                     Tariff_Id.Parse("DE*GEF*T0001"),
                                     Tariff_Id.Parse("DE*GEF*T0002")
                                 },
                                 URL.Parse("https://open.charging.cloud/terms"),
                                 DateTime.Parse("2020-09-21T00:00:00Z")
                             );

            var patchResult = Connector1.TryPatch(JObject.Parse(@"{ ""standard"": ""TESLA_S"" }"));

            Assert.IsNotNull(patchResult);
            Assert.IsTrue   (patchResult.IsSuccess);
            Assert.IsFalse  (patchResult.IsFailed);
            Assert.IsNull   (patchResult.ErrorResponse);
            Assert.IsNotNull(patchResult.PatchedData);

            Assert.AreEqual   (Connector_Id.Parse("1"),                         patchResult.PatchedData.Id);
            Assert.AreEqual   (ConnectorTypes.TESLA_S,                          patchResult.PatchedData.Standard);
            Assert.AreEqual   (ConnectorFormats.SOCKET,                         patchResult.PatchedData.Format);
            Assert.AreEqual   (PowerTypes.AC_3_PHASE,                           patchResult.PatchedData.PowerType);
            Assert.AreEqual   (400,                                             patchResult.PatchedData.MaxVoltage);
            Assert.AreEqual   (30,                                              patchResult.PatchedData.MaxAmperage);
            Assert.AreEqual   (12,                                              patchResult.PatchedData.MaxElectricPower);
            Assert.AreEqual   (2,                                               patchResult.PatchedData.TariffIds.        Count());
            Assert.AreEqual   (Tariff_Id.Parse("DE*GEF*T0001"),                 patchResult.PatchedData.TariffIds.        First());
            Assert.AreEqual   (Tariff_Id.Parse("DE*GEF*T0002"),                 patchResult.PatchedData.TariffIds.Skip(1).First());
            Assert.AreEqual   (URL.Parse("https://open.charging.cloud/terms"),  patchResult.PatchedData.TermsAndConditionsURL);
            Assert.AreNotEqual(DateTime.Parse("2020-09-21T00:00:00Z"),          patchResult.PatchedData.LastUpdated);

            Assert.IsTrue     (DateTime.UtcNow - patchResult.PatchedData.LastUpdated < TimeSpan.FromSeconds(5));

        }

        #endregion

        #region Connector_PATCH_withLastUpdated()

        /// <summary>
        /// Minimal connector PATCH test, but with last_updated parameter.
        /// </summary>
        [Test]
        public static void Connector_PATCH_withLastUpdated()
        {

            var Connector1 = new Connector(
                                 Connector_Id.Parse("1"),
                                 ConnectorTypes.IEC_62196_T2,
                                 ConnectorFormats.SOCKET,
                                 PowerTypes.AC_3_PHASE,
                                 400,
                                 30,
                                 12,
                                 new Tariff_Id[] {
                                     Tariff_Id.Parse("DE*GEF*T0001"),
                                     Tariff_Id.Parse("DE*GEF*T0002")
                                 },
                                 URL.Parse("https://open.charging.cloud/terms"),
                                 DateTime.Parse("2020-09-21T00:00:00Z")
                             );

            var patchResult = Connector1.TryPatch(JObject.Parse(@"{ ""format"": ""CABLE"", ""last_updated"": ""2020-10-15T00:00:00Z"" }"));

            Assert.IsNotNull(patchResult);
            Assert.IsTrue   (patchResult.IsSuccess);
            Assert.IsFalse  (patchResult.IsFailed);
            Assert.IsNull   (patchResult.ErrorResponse);
            Assert.IsNotNull(patchResult.PatchedData);

            Assert.AreEqual (Connector_Id.Parse("1"),                         patchResult.PatchedData.Id);
            Assert.AreEqual (ConnectorTypes.IEC_62196_T2,                     patchResult.PatchedData.Standard);
            Assert.AreEqual (ConnectorFormats.CABLE,                          patchResult.PatchedData.Format);
            Assert.AreEqual (PowerTypes.AC_3_PHASE,                           patchResult.PatchedData.PowerType);
            Assert.AreEqual (400,                                             patchResult.PatchedData.MaxVoltage);
            Assert.AreEqual (30,                                              patchResult.PatchedData.MaxAmperage);
            Assert.AreEqual (12,                                              patchResult.PatchedData.MaxElectricPower);
            Assert.AreEqual (2,                                               patchResult.PatchedData.TariffIds.        Count());
            Assert.AreEqual (Tariff_Id.Parse("DE*GEF*T0001"),                 patchResult.PatchedData.TariffIds.        First());
            Assert.AreEqual (Tariff_Id.Parse("DE*GEF*T0002"),                 patchResult.PatchedData.TariffIds.Skip(1).First());
            Assert.AreEqual (URL.Parse("https://open.charging.cloud/terms"),  patchResult.PatchedData.TermsAndConditionsURL);
            Assert.AreEqual ("2020-10-15T00:00:00.000Z",                      patchResult.PatchedData.LastUpdated.ToIso8601());

        }

        #endregion

        #region Connector_PATCH_TariffIdArray()

        /// <summary>
        /// Try to PATCH the tariff_id array of a connector.
        /// </summary>
        [Test]
        public static void Connector_PATCH_TariffIdArray()
        {

            var Connector1 = new Connector(
                                 Connector_Id.Parse("1"),
                                 ConnectorTypes.IEC_62196_T2,
                                 ConnectorFormats.SOCKET,
                                 PowerTypes.AC_3_PHASE,
                                 400,
                                 30,
                                 12,
                                 new Tariff_Id[] {
                                     Tariff_Id.Parse("DE*GEF*T0001"),
                                     Tariff_Id.Parse("DE*GEF*T0002")
                                 },
                                 URL.Parse("https://open.charging.cloud/terms"),
                                 DateTime.Parse("2020-09-21T00:00:00Z")
                             );

            var patchResult = Connector1.TryPatch(JObject.Parse(@"{ ""tariff_ids"": [ ""DE*GEF*T0003"" ], ""last_updated"": ""2020-10-15T00:00:00Z"" }"));

            Assert.IsNotNull(patchResult);
            Assert.IsTrue   (patchResult.IsSuccess);
            Assert.IsFalse  (patchResult.IsFailed);
            Assert.IsNull   (patchResult.ErrorResponse);
            Assert.IsNotNull(patchResult.PatchedData);

            Assert.AreEqual (Connector_Id.Parse("1"),                         patchResult.PatchedData.Id);
            Assert.AreEqual (ConnectorTypes.IEC_62196_T2,                     patchResult.PatchedData.Standard);
            Assert.AreEqual (ConnectorFormats.SOCKET,                         patchResult.PatchedData.Format);
            Assert.AreEqual (PowerTypes.AC_3_PHASE,                           patchResult.PatchedData.PowerType);
            Assert.AreEqual (400,                                             patchResult.PatchedData.MaxVoltage);
            Assert.AreEqual (30,                                              patchResult.PatchedData.MaxAmperage);
            Assert.AreEqual (12,                                              patchResult.PatchedData.MaxElectricPower);
            Assert.AreEqual (1,                                               patchResult.PatchedData.TariffIds.Count());
            Assert.AreEqual (Tariff_Id.Parse("DE*GEF*T0003"),                 patchResult.PatchedData.TariffIds.First());
            Assert.AreEqual (URL.Parse("https://open.charging.cloud/terms"),  patchResult.PatchedData.TermsAndConditionsURL);
            Assert.AreEqual ("2020-10-15T00:00:00.000Z",                      patchResult.PatchedData.LastUpdated.ToIso8601());

        }

        #endregion

        #region Connector_PATCH_RemoveTariffIdArray()

        /// <summary>
        /// Try to remove the tariff_id array of a connector via PATCH.
        /// </summary>
        [Test]
        public static void Connector_PATCH_RemoveTariffIdArray()
        {

            var Connector1 = new Connector(
                                 Connector_Id.Parse("1"),
                                 ConnectorTypes.IEC_62196_T2,
                                 ConnectorFormats.SOCKET,
                                 PowerTypes.AC_3_PHASE,
                                 400,
                                 30,
                                 12,
                                 new Tariff_Id[] {
                                     Tariff_Id.Parse("DE*GEF*T0001"),
                                     Tariff_Id.Parse("DE*GEF*T0002")
                                 },
                                 URL.Parse("https://open.charging.cloud/terms"),
                                 DateTime.Parse("2020-09-21T00:00:00Z")
                             );

            var patchResult = Connector1.TryPatch(JObject.Parse(@"{ ""tariff_ids"": null, ""last_updated"": ""2020-10-15T00:00:00Z"" }"));

            Assert.IsNotNull(patchResult);
            Assert.IsTrue   (patchResult.IsSuccess);
            Assert.IsFalse  (patchResult.IsFailed);
            Assert.IsNull   (patchResult.ErrorResponse);
            Assert.IsNotNull(patchResult.PatchedData);

            Assert.AreEqual (Connector_Id.Parse("1"),                         patchResult.PatchedData.Id);
            Assert.AreEqual (ConnectorTypes.IEC_62196_T2,                     patchResult.PatchedData.Standard);
            Assert.AreEqual (ConnectorFormats.SOCKET,                         patchResult.PatchedData.Format);
            Assert.AreEqual (PowerTypes.AC_3_PHASE,                           patchResult.PatchedData.PowerType);
            Assert.AreEqual (400,                                             patchResult.PatchedData.MaxVoltage);
            Assert.AreEqual (30,                                              patchResult.PatchedData.MaxAmperage);
            Assert.AreEqual (12,                                              patchResult.PatchedData.MaxElectricPower);
            Assert.AreEqual (0,                                               patchResult.PatchedData.TariffIds.Count());
            Assert.AreEqual (URL.Parse("https://open.charging.cloud/terms"),  patchResult.PatchedData.TermsAndConditionsURL);
            Assert.AreEqual ("2020-10-15T00:00:00.000Z",                      patchResult.PatchedData.LastUpdated.ToIso8601());

        }

        #endregion

        #region Connector_PATCH_RemoveTermsAndConditionsURL()

        /// <summary>
        /// Try to remove the 'Terms and Conditions' of a connector via PATCH.
        /// </summary>
        [Test]
        public static void Connector_PATCH_RemoveTermsAndConditionsURL()
        {

            var Connector1 = new Connector(
                                 Connector_Id.Parse("1"),
                                 ConnectorTypes.IEC_62196_T2,
                                 ConnectorFormats.SOCKET,
                                 PowerTypes.AC_3_PHASE,
                                 400,
                                 30,
                                 12,
                                 new Tariff_Id[] {
                                     Tariff_Id.Parse("DE*GEF*T0001"),
                                     Tariff_Id.Parse("DE*GEF*T0002")
                                 },
                                 URL.Parse("https://open.charging.cloud/terms"),
                                 DateTime.Parse("2020-09-21T00:00:00Z")
                             );

            var patchResult = Connector1.TryPatch(JObject.Parse(@"{ ""terms_and_conditions"": null, ""last_updated"": ""2020-10-15T00:00:00Z"" }"));

            Assert.IsNotNull(patchResult);
            Assert.IsTrue   (patchResult.IsSuccess);
            Assert.IsFalse  (patchResult.IsFailed);
            Assert.IsNull   (patchResult.ErrorResponse);
            Assert.IsNotNull(patchResult.PatchedData);

            Assert.AreEqual (Connector_Id.Parse("1"),          patchResult.PatchedData.Id);
            Assert.AreEqual (ConnectorTypes.IEC_62196_T2,      patchResult.PatchedData.Standard);
            Assert.AreEqual (ConnectorFormats.SOCKET,          patchResult.PatchedData.Format);
            Assert.AreEqual (PowerTypes.AC_3_PHASE,            patchResult.PatchedData.PowerType);
            Assert.AreEqual (400,                              patchResult.PatchedData.MaxVoltage);
            Assert.AreEqual (30,                               patchResult.PatchedData.MaxAmperage);
            Assert.AreEqual (12,                               patchResult.PatchedData.MaxElectricPower);
            Assert.AreEqual (2,                                patchResult.PatchedData.TariffIds.        Count());
            Assert.AreEqual (Tariff_Id.Parse("DE*GEF*T0001"),  patchResult.PatchedData.TariffIds.        First());
            Assert.AreEqual (Tariff_Id.Parse("DE*GEF*T0002"),  patchResult.PatchedData.TariffIds.Skip(1).First());
            Assert.AreEqual (null,                             patchResult.PatchedData.TermsAndConditionsURL);
            Assert.AreEqual ("2020-10-15T00:00:00.000Z",       patchResult.PatchedData.LastUpdated.ToIso8601());

        }

        #endregion

        #region Connector_PATCH_InvalidPatch()

        /// <summary>
        /// Invalid connector PATCH.
        /// </summary>
        [Test]
        public static void Connector_PATCH_InvalidPatch()
        {

            var Connector1 = new Connector(
                                 Connector_Id.Parse("1"),
                                 ConnectorTypes.IEC_62196_T2,
                                 ConnectorFormats.SOCKET,
                                 PowerTypes.AC_3_PHASE,
                                 400,
                                 30,
                                 12,
                                 new Tariff_Id[] {
                                     Tariff_Id.Parse("DE*GEF*T0001"),
                                     Tariff_Id.Parse("DE*GEF*T0002")
                                 },
                                 URL.Parse("https://open.charging.cloud/terms"),
                                 DateTime.Parse("2020-09-21T00:00:00Z")
                             );

            var patchResult = Connector1.TryPatch(JObject.Parse(@"{ ""standard"": ""I-N-V-A-L-I-D!"" }"));

            Assert.IsNotNull(patchResult);
            Assert.IsFalse  (patchResult.IsSuccess);
            Assert.IsTrue   (patchResult.IsFailed);
            Assert.IsNotNull(patchResult.ErrorResponse);
            Assert.AreEqual ("Invalid JSON merge patch of a connector: Invalid 'connector standard'!",  patchResult.ErrorResponse);
            Assert.IsNotNull(patchResult.PatchedData);

            Assert.AreEqual (Connector_Id.Parse("1"),                                                   patchResult.PatchedData.Id);
            Assert.AreEqual (ConnectorTypes.IEC_62196_T2,                                               patchResult.PatchedData.Standard);
            Assert.AreEqual (ConnectorFormats.SOCKET,                                                   patchResult.PatchedData.Format);
            Assert.AreEqual (PowerTypes.AC_3_PHASE,                                                     patchResult.PatchedData.PowerType);
            Assert.AreEqual (400,                                                                       patchResult.PatchedData.MaxVoltage);
            Assert.AreEqual (30,                                                                        patchResult.PatchedData.MaxAmperage);
            Assert.AreEqual (12,                                                                        patchResult.PatchedData.MaxElectricPower);
            Assert.AreEqual (2,                                                                         patchResult.PatchedData.TariffIds.        Count());
            Assert.AreEqual (Tariff_Id.Parse("DE*GEF*T0001"),                                           patchResult.PatchedData.TariffIds.        First());
            Assert.AreEqual (Tariff_Id.Parse("DE*GEF*T0002"),                                           patchResult.PatchedData.TariffIds.Skip(1).First());
            Assert.AreEqual (URL.Parse("https://open.charging.cloud/terms"),                            patchResult.PatchedData.TermsAndConditionsURL);
            Assert.AreEqual ("2020-09-21T00:00:00.000Z",                                                patchResult.PatchedData.LastUpdated.ToIso8601());

        }

        #endregion

        #region Connector_PATCH_InvalidLastUpdatedPatch()

        /// <summary>
        /// Invalid 'last_updated' PATCH of a connector.
        /// </summary>
        [Test]
        public static void Connector_PATCH_InvalidLastUpdatedPatch()
        {

            var Connector1 = new Connector(
                                 Connector_Id.Parse("1"),
                                 ConnectorTypes.IEC_62196_T2,
                                 ConnectorFormats.SOCKET,
                                 PowerTypes.AC_3_PHASE,
                                 400,
                                 30,
                                 12,
                                 new Tariff_Id[] {
                                     Tariff_Id.Parse("DE*GEF*T0001"),
                                     Tariff_Id.Parse("DE*GEF*T0002")
                                 },
                                 URL.Parse("https://open.charging.cloud/terms"),
                                 DateTime.Parse("2020-09-21T00:00:00Z")
                             );

            var patchResult = Connector1.TryPatch(JObject.Parse(@"{ ""last_updated"": ""I-N-V-A-L-I-D!"" }"));

            Assert.IsNotNull(patchResult);
            Assert.IsFalse  (patchResult.IsSuccess);
            Assert.IsTrue   (patchResult.IsFailed);
            Assert.IsNotNull(patchResult.ErrorResponse);
            Assert.AreEqual ("Invalid JSON merge patch of a connector: Invalid 'last updated'!",  patchResult.ErrorResponse);
            Assert.IsNotNull(patchResult.PatchedData);

            Assert.AreEqual (Connector_Id.Parse("1"),                                             patchResult.PatchedData.Id);
            Assert.AreEqual (ConnectorTypes.IEC_62196_T2,                                         patchResult.PatchedData.Standard);
            Assert.AreEqual (ConnectorFormats.SOCKET,                                             patchResult.PatchedData.Format);
            Assert.AreEqual (PowerTypes.AC_3_PHASE,                                               patchResult.PatchedData.PowerType);
            Assert.AreEqual (400,                                                                 patchResult.PatchedData.MaxVoltage);
            Assert.AreEqual (30,                                                                  patchResult.PatchedData.MaxAmperage);
            Assert.AreEqual (12,                                                                  patchResult.PatchedData.MaxElectricPower);
            Assert.AreEqual (2,                                                                   patchResult.PatchedData.TariffIds.        Count());
            Assert.AreEqual (Tariff_Id.Parse("DE*GEF*T0001"),                                     patchResult.PatchedData.TariffIds.        First());
            Assert.AreEqual (Tariff_Id.Parse("DE*GEF*T0002"),                                     patchResult.PatchedData.TariffIds.Skip(1).First());
            Assert.AreEqual (URL.Parse("https://open.charging.cloud/terms"),                      patchResult.PatchedData.TermsAndConditionsURL);
            Assert.AreEqual ("2020-09-21T00:00:00.000Z",                                          patchResult.PatchedData.LastUpdated.ToIso8601());

        }

        #endregion

    }

}
