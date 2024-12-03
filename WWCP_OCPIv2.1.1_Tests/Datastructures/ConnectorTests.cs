/*
 * Copyright (c) 2015-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using NUnit.Framework;
using NUnit.Framework.Legacy;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_1_1.UnitTests.Datastructures
{

    /// <summary>
    /// Unit tests for connectors.
    /// https://github.com/ocpi/ocpi/blob/release-2.1.1-bugfixes/mod_locations.md#33-connector-object
    /// </summary>
    [TestFixture]
    public static class ConnectorTests
    {

        #region Connector_SerializeDeserialize_Tariff_Test1()

        /// <summary>
        /// Test serialize, deserialize, and compare a connector with a single tariff.
        /// </summary>
        [Test]
        public static void Connector_SerializeDeserialize_Tariff_Test1()
        {

            var Connector1 = new Connector(
                                 Connector_Id.Parse("1"),
                                 ConnectorType.IEC_62196_T2,
                                 ConnectorFormats.SOCKET,
                                 PowerTypes.AC_3_PHASE,
                                 Volt.  ParseV(400),
                                 Ampere.ParseA(30),
                                 Tariff_Id.Parse("DE*GEF*T0001"),
                                 URL.Parse("https://open.charging.cloud/terms"),
                                 DateTime.Parse("2020-09-21T00:00:00Z")
                             );

            var json = Connector1.ToJSON();

            ClassicAssert.AreEqual("1",                                  json["id"]?.                  Value<String>());
            ClassicAssert.AreEqual("IEC_62196_T2",                       json["standard"]?.            Value<String>());
            ClassicAssert.AreEqual("SOCKET",                             json["format"]?.              Value<String>());
            ClassicAssert.AreEqual("AC_3_PHASE",                         json["power_type"]?.          Value<String>());
            ClassicAssert.AreEqual(400,                                  json["voltage"]?.             Value<UInt16>());
            ClassicAssert.AreEqual(30,                                   json["amperage"]?.            Value<UInt16>());
            ClassicAssert.AreEqual("DE*GEF*T0001",                       json["tariff_id"]?.           Value<String>());
            ClassicAssert.AreEqual("https://open.charging.cloud/terms",  json["terms_and_conditions"]?.Value<String>());
            ClassicAssert.AreEqual("2020-09-21T00:00:00.000Z",           json["last_updated"]?.        Value<String>());

            if (Connector.TryParse(json, out var connector2, out var errorResponse))
            {

                ClassicAssert.IsNotNull(connector2);
                ClassicAssert.IsNull   (errorResponse);

                ClassicAssert.AreEqual(Connector1.Id,                        connector2.Id);
                ClassicAssert.AreEqual(Connector1.Standard,                  connector2.Standard);
                ClassicAssert.AreEqual(Connector1.Format,                    connector2.Format);
                ClassicAssert.AreEqual(Connector1.PowerType,                 connector2.PowerType);
                ClassicAssert.AreEqual(Connector1.Voltage,                   connector2.Voltage);
                ClassicAssert.AreEqual(Connector1.Amperage,                  connector2.Amperage);
                ClassicAssert.AreEqual(Connector1.GetTariffId(),             connector2.GetTariffId());
                ClassicAssert.AreEqual(Connector1.TermsAndConditionsURL,     connector2.TermsAndConditionsURL);
                ClassicAssert.AreEqual(Connector1.LastUpdated.ToIso8601(),   connector2.LastUpdated.ToIso8601());

            }
            else
            {
                ClassicAssert.Fail("Failed to parse JSON: " + errorResponse);
            }

        }

        #endregion

        #region Connector_SerializeDeserialize_EMSPTariffIds_Test1()

        /// <summary>
        /// Test serialize, deserialize, and compare a connector with an EMSP tariff map.
        /// </summary>
        [Test]
        public static void Connector_SerializeDeserialize_EMSPTariffIds_Test1()
        {

            var Connector1 = new Connector(
                                 Connector_Id.Parse("1"),
                                 ConnectorType.IEC_62196_T2,
                                 ConnectorFormats.SOCKET,
                                 PowerTypes.AC_3_PHASE,
                                 Volt.  ParseV(400),
                                 Ampere.ParseA(30),
                                 new Dictionary<EMSP_Id, Tariff_Id>() {
                                     { EMSP_Id.Parse("DE-GDF"), Tariff_Id.Parse("DE*GEF*T0001") },
                                     { EMSP_Id.Parse("DE-GD2"), Tariff_Id.Parse("DE*GEF*T0002") }
                                 },
                                 URL.Parse("https://open.charging.cloud/terms"),
                                 DateTime.Parse("2020-09-21T00:00:00Z")
                             );

            var json = Connector1.ToJSON();

            ClassicAssert.AreEqual("1",                                  json["id"]?.                  Value<String>());
            ClassicAssert.AreEqual("IEC_62196_T2",                       json["standard"]?.            Value<String>());
            ClassicAssert.AreEqual("SOCKET",                             json["format"]?.              Value<String>());
            ClassicAssert.AreEqual("AC_3_PHASE",                         json["power_type"]?.          Value<String>());
            ClassicAssert.AreEqual(400,                                  json["voltage"]?.             Value<UInt16>());
            ClassicAssert.AreEqual(30,                                   json["amperage"]?.            Value<UInt16>());
            ClassicAssert.AreEqual(null,                                 json["tariff_id"]?.           Value<String>());
            ClassicAssert.AreEqual("https://open.charging.cloud/terms",  json["terms_and_conditions"]?.Value<String>());
            ClassicAssert.AreEqual("2020-09-21T00:00:00.000Z",           json["last_updated"]?.        Value<String>());

            var emspTariffIds = json["emsp_tariff_ids"] as JObject;
            ClassicAssert.IsNotNull(emspTariffIds);
            if (emspTariffIds is not null) {
                ClassicAssert.AreEqual("DE*GEF*T0001",                   emspTariffIds["DE-GDF"]?.     Value<String>());
                ClassicAssert.AreEqual("DE*GEF*T0002",                   emspTariffIds["DE-GD2"]?.     Value<String>());
            }

            if (Connector.TryParse(json, out var connector2, out var errorResponse))
            {

                ClassicAssert.IsNotNull(connector2);
                ClassicAssert.IsNull   (errorResponse);

                ClassicAssert.AreEqual(Connector1.Id,                        connector2.Id);
                ClassicAssert.AreEqual(Connector1.Standard,                  connector2.Standard);
                ClassicAssert.AreEqual(Connector1.Format,                    connector2.Format);
                ClassicAssert.AreEqual(Connector1.PowerType,                 connector2.PowerType);
                ClassicAssert.AreEqual(Connector1.Voltage,                   connector2.Voltage);
                ClassicAssert.AreEqual(Connector1.Amperage,                  connector2.Amperage);
                ClassicAssert.AreEqual(Connector1.GetTariffId(),             connector2.GetTariffId());
                ClassicAssert.AreEqual(Connector1.TermsAndConditionsURL,     connector2.TermsAndConditionsURL);
                ClassicAssert.AreEqual(Connector1.LastUpdated.ToIso8601(),   connector2.LastUpdated.ToIso8601());

            }
            else
            {
                ClassicAssert.Fail("Failed to parse JSON: " + errorResponse);
            }

        }

        #endregion

        #region Connector_SerializeDeserialize_EMSPTariffIds_Test2()

        /// <summary>
        /// Test serialize, deserialize, and compare a connector with an EMSP tariff map.
        /// </summary>
        [Test]
        public static void Connector_SerializeDeserialize_EMSPTariffIds_Test2()
        {

            var Connector1 = new Connector(
                                 Connector_Id.Parse("1"),
                                 ConnectorType.IEC_62196_T2,
                                 ConnectorFormats.SOCKET,
                                 PowerTypes.AC_3_PHASE,
                                 Volt.  ParseV(400),
                                 Ampere.ParseA(30),
                                 new Dictionary<EMSP_Id, Tariff_Id>() {
                                     { EMSP_Id.Parse("DE-GDF"), Tariff_Id.Parse("DE*GEF*T0001") },
                                     { EMSP_Id.Parse("DE-GD2"), Tariff_Id.Parse("DE*GEF*T0002") }
                                 },
                                 URL.Parse("https://open.charging.cloud/terms"),
                                 DateTime.Parse("2020-09-21T00:00:00Z")
                             );

            var json = Connector1.ToJSON(EMSP_Id.Parse("DE-GDF"));

            ClassicAssert.AreEqual("1",                                  json["id"]?.                  Value<String>());
            ClassicAssert.AreEqual("IEC_62196_T2",                       json["standard"]?.            Value<String>());
            ClassicAssert.AreEqual("SOCKET",                             json["format"]?.              Value<String>());
            ClassicAssert.AreEqual("AC_3_PHASE",                         json["power_type"]?.          Value<String>());
            ClassicAssert.AreEqual(400,                                  json["voltage"]?.             Value<UInt16>());
            ClassicAssert.AreEqual(30,                                   json["amperage"]?.            Value<UInt16>());
            ClassicAssert.AreEqual("DE*GEF*T0001",                       json["tariff_id"]?.           Value<String>());
            ClassicAssert.AreEqual(null,                                 json["emsp_tariff_ids"]?.     Value<String>());
            ClassicAssert.AreEqual("https://open.charging.cloud/terms",  json["terms_and_conditions"]?.Value<String>());
            ClassicAssert.AreEqual("2020-09-21T00:00:00.000Z",           json["last_updated"]?.        Value<String>());

            if (Connector.TryParse(json, out var connector2, out var errorResponse))
            {

                ClassicAssert.IsNotNull(connector2);
                ClassicAssert.IsNull   (errorResponse);

                ClassicAssert.AreEqual(Connector1.Id,                        connector2.Id);
                ClassicAssert.AreEqual(Connector1.Standard,                  connector2.Standard);
                ClassicAssert.AreEqual(Connector1.Format,                    connector2.Format);
                ClassicAssert.AreEqual(Connector1.PowerType,                 connector2.PowerType);
                ClassicAssert.AreEqual(Connector1.Voltage,                   connector2.Voltage);
                ClassicAssert.AreEqual(Connector1.Amperage,                  connector2.Amperage);
                //ClassicAssert.AreEqual(Connector1.GetTariffId(),             connector2.GetTariffId());
                ClassicAssert.AreEqual(Connector1.TermsAndConditionsURL,     connector2.TermsAndConditionsURL);
                ClassicAssert.AreEqual(Connector1.LastUpdated.ToIso8601(),   connector2.LastUpdated.ToIso8601());

            }
            else
            {
                ClassicAssert.Fail("Failed to parse JSON: " + errorResponse);
            }

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
                                 ConnectorType.IEC_62196_T2,
                                 ConnectorFormats.SOCKET,
                                 PowerTypes.AC_3_PHASE,
                                 Volt.  ParseV(400),
                                 Ampere.ParseA(30),
                                 Tariff_Id.Parse("DE*GEF*T0001"),
                                 URL.Parse("https://open.charging.cloud/terms"),
                                 DateTime.Parse("2020-09-21T00:00:00Z")
                             );

            var patchResult = Connector1.TryPatch(JObject.Parse(@"{ ""id"": ""2"", ""last_updated"": ""2020-10-15T00:00:00Z"" }"));

            ClassicAssert.IsNotNull(patchResult);
            ClassicAssert.IsFalse  (patchResult.IsSuccess);
            ClassicAssert.IsTrue   (patchResult.IsFailed);
            ClassicAssert.IsNotNull(patchResult.ErrorResponse);
            ClassicAssert.AreEqual ("Patching the 'identification' of a connector is not allowed!",  patchResult.ErrorResponse);
            ClassicAssert.IsNotNull(patchResult.PatchedData);

            if (patchResult.PatchedData is not null)
            {
                ClassicAssert.AreEqual (Connector_Id.Parse("1"),                          patchResult.PatchedData.Id);
                ClassicAssert.AreEqual (ConnectorType.IEC_62196_T2,                       patchResult.PatchedData.Standard);
                ClassicAssert.AreEqual (ConnectorFormats.SOCKET,                          patchResult.PatchedData.Format);
                ClassicAssert.AreEqual (PowerTypes.AC_3_PHASE,                            patchResult.PatchedData.PowerType);
                ClassicAssert.AreEqual (400,                                              patchResult.PatchedData.Voltage. Value);
                ClassicAssert.AreEqual (30,                                               patchResult.PatchedData.Amperage.Value);
                ClassicAssert.AreEqual (Tariff_Id.Parse("DE*GEF*T0001"),                  patchResult.PatchedData.GetTariffId());
                ClassicAssert.AreEqual (URL.Parse("https://open.charging.cloud/terms"),   patchResult.PatchedData.TermsAndConditionsURL);
                ClassicAssert.AreEqual ("2020-09-21T00:00:00.000Z",                       patchResult.PatchedData.LastUpdated.ToIso8601());
            }

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
                                 ConnectorType.IEC_62196_T2,
                                 ConnectorFormats.SOCKET,
                                 PowerTypes.AC_3_PHASE,
                                 Volt.  ParseV(400),
                                 Ampere.ParseA(30),
                                 Tariff_Id.Parse("DE*GEF*T0001"),
                                 URL.Parse("https://open.charging.cloud/terms"),
                                 DateTime.Parse("2020-09-21T00:00:00Z")
                             );

            var patchResult = Connector1.TryPatch(JObject.Parse(@"{ ""standard"": ""TESLA_S"" }"));

            ClassicAssert.IsNotNull(patchResult);
            ClassicAssert.IsTrue   (patchResult.IsSuccess);
            ClassicAssert.IsFalse  (patchResult.IsFailed);
            ClassicAssert.IsNull   (patchResult.ErrorResponse);
            ClassicAssert.IsNotNull(patchResult.PatchedData);

            if (patchResult.PatchedData is not null)
            {
                ClassicAssert.AreEqual   (Connector_Id.Parse("1"),                          patchResult.PatchedData.Id);
                ClassicAssert.AreEqual   (ConnectorType.TESLA_S,                            patchResult.PatchedData.Standard);
                ClassicAssert.AreEqual   (ConnectorFormats.SOCKET,                          patchResult.PatchedData.Format);
                ClassicAssert.AreEqual   (PowerTypes.AC_3_PHASE,                            patchResult.PatchedData.PowerType);
                ClassicAssert.AreEqual   (400,                                              patchResult.PatchedData.Voltage. Value);
                ClassicAssert.AreEqual   (30,                                               patchResult.PatchedData.Amperage.Value);
                ClassicAssert.AreEqual   (Tariff_Id.Parse("DE*GEF*T0001"),                  patchResult.PatchedData.GetTariffId());
                ClassicAssert.AreEqual   (URL.Parse("https://open.charging.cloud/terms"),   patchResult.PatchedData.TermsAndConditionsURL);
                ClassicAssert.AreNotEqual(DateTime.Parse("2020-09-21T00:00:00Z"),           patchResult.PatchedData.LastUpdated);

                ClassicAssert.IsTrue     (Timestamp.Now - patchResult.PatchedData.LastUpdated < TimeSpan.FromSeconds(5));
            }

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
                                 ConnectorType.IEC_62196_T2,
                                 ConnectorFormats.SOCKET,
                                 PowerTypes.AC_3_PHASE,
                                 Volt.  ParseV(400),
                                 Ampere.ParseA(30),
                                 Tariff_Id.Parse("DE*GEF*T0001"),
                                 URL.Parse("https://open.charging.cloud/terms"),
                                 DateTime.Parse("2020-09-21T00:00:00Z")
                             );

            var patchResult = Connector1.TryPatch(JObject.Parse(@"{ ""format"": ""CABLE"", ""last_updated"": ""2020-10-15T00:00:00Z"" }"));

            ClassicAssert.IsNotNull(patchResult);
            ClassicAssert.IsTrue   (patchResult.IsSuccess);
            ClassicAssert.IsFalse  (patchResult.IsFailed);
            ClassicAssert.IsNull   (patchResult.ErrorResponse);
            ClassicAssert.IsNotNull(patchResult.PatchedData);

            if (patchResult.PatchedData is not null)
            {
                ClassicAssert.AreEqual (Connector_Id.Parse("1"),                          patchResult.PatchedData.Id);
                ClassicAssert.AreEqual (ConnectorType.IEC_62196_T2,                       patchResult.PatchedData.Standard);
                ClassicAssert.AreEqual (ConnectorFormats.CABLE,                           patchResult.PatchedData.Format);
                ClassicAssert.AreEqual (PowerTypes.AC_3_PHASE,                            patchResult.PatchedData.PowerType);
                ClassicAssert.AreEqual (400,                                              patchResult.PatchedData.Voltage. Value);
                ClassicAssert.AreEqual (30,                                               patchResult.PatchedData.Amperage.Value);
                ClassicAssert.AreEqual (Tariff_Id.Parse("DE*GEF*T0001"),                  patchResult.PatchedData.GetTariffId());
                ClassicAssert.AreEqual (URL.Parse("https://open.charging.cloud/terms"),   patchResult.PatchedData.TermsAndConditionsURL);
                ClassicAssert.AreEqual ("2020-10-15T00:00:00.000Z",                       patchResult.PatchedData.LastUpdated.ToIso8601());
            }

        }

        #endregion

        #region Connector_PATCH_TariffId()

        /// <summary>
        /// Try to PATCH the tariff_id of a connector.
        /// </summary>
        [Test]
        public static void Connector_PATCH_TariffId()
        {

            var Connector1 = new Connector(
                                 Connector_Id.Parse("1"),
                                 ConnectorType.IEC_62196_T2,
                                 ConnectorFormats.SOCKET,
                                 PowerTypes.AC_3_PHASE,
                                 Volt.  ParseV(400),
                                 Ampere.ParseA(30),
                                 Tariff_Id.Parse("DE*GEF*T0001"),
                                 URL.Parse("https://open.charging.cloud/terms"),
                                 DateTime.Parse("2020-09-21T00:00:00Z")
                             );

            var patchResult = Connector1.TryPatch(JObject.Parse(@"{ ""tariff_id"": ""DE*GEF*T0003"", ""last_updated"": ""2020-10-15T00:00:00Z"" }"));

            ClassicAssert.IsNotNull(patchResult);
            ClassicAssert.IsTrue   (patchResult.IsSuccess);
            ClassicAssert.IsFalse  (patchResult.IsFailed);
            ClassicAssert.IsNull   (patchResult.ErrorResponse);
            ClassicAssert.IsNotNull(patchResult.PatchedData);

            if (patchResult.PatchedData is not null)
            {
                ClassicAssert.AreEqual (Connector_Id.Parse("1"),                          patchResult.PatchedData.Id);
                ClassicAssert.AreEqual (ConnectorType.IEC_62196_T2,                       patchResult.PatchedData.Standard);
                ClassicAssert.AreEqual (ConnectorFormats.SOCKET,                          patchResult.PatchedData.Format);
                ClassicAssert.AreEqual (PowerTypes.AC_3_PHASE,                            patchResult.PatchedData.PowerType);
                ClassicAssert.AreEqual (400,                                              patchResult.PatchedData.Voltage. Value);
                ClassicAssert.AreEqual (30,                                               patchResult.PatchedData.Amperage.Value);
                ClassicAssert.AreEqual (Tariff_Id.Parse("DE*GEF*T0003"),                  patchResult.PatchedData.GetTariffId());
                ClassicAssert.AreEqual (URL.Parse("https://open.charging.cloud/terms"),   patchResult.PatchedData.TermsAndConditionsURL);
                ClassicAssert.AreEqual ("2020-10-15T00:00:00.000Z",                       patchResult.PatchedData.LastUpdated.ToIso8601());
            }

        }

        #endregion

        #region Connector_PATCH_RemoveTariffId()

        /// <summary>
        /// Try to remove the tariff_id of a connector via PATCH.
        /// </summary>
        [Test]
        public static void Connector_PATCH_RemoveTariffId()
        {

            var Connector1 = new Connector(
                                 Connector_Id.Parse("1"),
                                 ConnectorType.IEC_62196_T2,
                                 ConnectorFormats.SOCKET,
                                 PowerTypes.AC_3_PHASE,
                                 Volt.  ParseV(400),
                                 Ampere.ParseA(30),
                                 Tariff_Id.Parse("DE*GEF*T0001"),
                                 URL.Parse("https://open.charging.cloud/terms"),
                                 DateTime.Parse("2020-09-21T00:00:00Z")
                             );

            var patchResult = Connector1.TryPatch(JObject.Parse(@"{ ""tariff_id"": null, ""last_updated"": ""2020-10-15T00:00:00Z"" }"));

            ClassicAssert.IsNotNull(patchResult);
            ClassicAssert.IsTrue   (patchResult.IsSuccess);
            ClassicAssert.IsFalse  (patchResult.IsFailed);
            ClassicAssert.IsNull   (patchResult.ErrorResponse);
            ClassicAssert.IsNotNull(patchResult.PatchedData);

            if (patchResult.PatchedData is not null)
            {
                ClassicAssert.AreEqual (Connector_Id.Parse("1"),                          patchResult.PatchedData.Id);
                ClassicAssert.AreEqual (ConnectorType.IEC_62196_T2,                       patchResult.PatchedData.Standard);
                ClassicAssert.AreEqual (ConnectorFormats.SOCKET,                          patchResult.PatchedData.Format);
                ClassicAssert.AreEqual (PowerTypes.AC_3_PHASE,                            patchResult.PatchedData.PowerType);
                ClassicAssert.AreEqual (400,                                              patchResult.PatchedData.Voltage. Value);
                ClassicAssert.AreEqual (30,                                               patchResult.PatchedData.Amperage.Value);
                ClassicAssert.IsNull   (                                                  patchResult.PatchedData.GetTariffId());
                ClassicAssert.AreEqual (URL.Parse("https://open.charging.cloud/terms"),   patchResult.PatchedData.TermsAndConditionsURL);
                ClassicAssert.AreEqual ("2020-10-15T00:00:00.000Z",                       patchResult.PatchedData.LastUpdated.ToIso8601());
            }

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
                                 ConnectorType.IEC_62196_T2,
                                 ConnectorFormats.SOCKET,
                                 PowerTypes.AC_3_PHASE,
                                 Volt.  ParseV(400),
                                 Ampere.ParseA(30),
                                 Tariff_Id.Parse("DE*GEF*T0001"),
                                 URL.Parse("https://open.charging.cloud/terms"),
                                 DateTime.Parse("2020-09-21T00:00:00Z")
                             );

            var patchResult = Connector1.TryPatch(JObject.Parse(@"{ ""terms_and_conditions"": null, ""last_updated"": ""2020-10-15T00:00:00Z"" }"));

            ClassicAssert.IsNotNull(patchResult);
            ClassicAssert.IsTrue   (patchResult.IsSuccess);
            ClassicAssert.IsFalse  (patchResult.IsFailed);
            ClassicAssert.IsNull   (patchResult.ErrorResponse);
            ClassicAssert.IsNotNull(patchResult.PatchedData);

            if (patchResult.PatchedData is not null)
            {
                ClassicAssert.AreEqual (Connector_Id.Parse("1"),           patchResult.PatchedData.Id);
                ClassicAssert.AreEqual (ConnectorType.IEC_62196_T2,        patchResult.PatchedData.Standard);
                ClassicAssert.AreEqual (ConnectorFormats.SOCKET,           patchResult.PatchedData.Format);
                ClassicAssert.AreEqual (PowerTypes.AC_3_PHASE,             patchResult.PatchedData.PowerType);
                ClassicAssert.AreEqual (400,                               patchResult.PatchedData.Voltage. Value);
                ClassicAssert.AreEqual (30,                                patchResult.PatchedData.Amperage.Value);
                ClassicAssert.AreEqual (Tariff_Id.Parse("DE*GEF*T0001"),   patchResult.PatchedData.GetTariffId());
                ClassicAssert.AreEqual (null,                              patchResult.PatchedData.TermsAndConditionsURL);
                ClassicAssert.AreEqual ("2020-10-15T00:00:00.000Z",        patchResult.PatchedData.LastUpdated.ToIso8601());
            }

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
                                 ConnectorType.IEC_62196_T2,
                                 ConnectorFormats.SOCKET,
                                 PowerTypes.AC_3_PHASE,
                                 Volt.  ParseV(400),
                                 Ampere.ParseA(30),
                                 Tariff_Id.Parse("DE*GEF*T0001"),
                                 URL.Parse("https://open.charging.cloud/terms"),
                                 DateTime.Parse("2020-09-21T00:00:00Z")
                             );

            var patchResult = Connector1.TryPatch(JObject.Parse(@"{ ""standard"": null }"));

            ClassicAssert.IsNotNull(patchResult);
            ClassicAssert.IsFalse  (patchResult.IsSuccess);
            ClassicAssert.IsTrue   (patchResult.IsFailed);
            ClassicAssert.IsNotNull(patchResult.ErrorResponse);
            ClassicAssert.AreEqual ("Invalid JSON merge patch of a connector: Invalid 'connector standard'!",   patchResult.ErrorResponse);
            ClassicAssert.IsNotNull(patchResult.PatchedData);

            if (patchResult.PatchedData is not null)
            {
                ClassicAssert.AreEqual (Connector_Id.Parse("1"),                          patchResult.PatchedData.Id);
                ClassicAssert.AreEqual (ConnectorType.IEC_62196_T2,                       patchResult.PatchedData.Standard);
                ClassicAssert.AreEqual (ConnectorFormats.SOCKET,                          patchResult.PatchedData.Format);
                ClassicAssert.AreEqual (PowerTypes.AC_3_PHASE,                            patchResult.PatchedData.PowerType);
                ClassicAssert.AreEqual (400,                                              patchResult.PatchedData.Voltage. Value);
                ClassicAssert.AreEqual (30,                                               patchResult.PatchedData.Amperage.Value);
                ClassicAssert.AreEqual (Tariff_Id.Parse("DE*GEF*T0001"),                  patchResult.PatchedData.GetTariffId());
                ClassicAssert.AreEqual (URL.Parse("https://open.charging.cloud/terms"),   patchResult.PatchedData.TermsAndConditionsURL);
                ClassicAssert.AreEqual ("2020-09-21T00:00:00.000Z",                       patchResult.PatchedData.LastUpdated.ToIso8601());
            }

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
                                 ConnectorType.IEC_62196_T2,
                                 ConnectorFormats.SOCKET,
                                 PowerTypes.AC_3_PHASE,
                                 Volt.  ParseV(400),
                                 Ampere.ParseA(30),
                                 Tariff_Id.Parse("DE*GEF*T0001"),
                                 URL.Parse("https://open.charging.cloud/terms"),
                                 DateTime.Parse("2020-09-21T00:00:00Z")
                             );

            var patchResult = Connector1.TryPatch(JObject.Parse(@"{ ""last_updated"": ""I-N-V-A-L-I-D!"" }"));

            ClassicAssert.IsNotNull(patchResult);
            ClassicAssert.IsFalse  (patchResult.IsSuccess);
            ClassicAssert.IsTrue   (patchResult.IsFailed);
            ClassicAssert.IsNotNull(patchResult.ErrorResponse);
            ClassicAssert.AreEqual ("Invalid JSON merge patch of a connector: Invalid 'last updated'!",  patchResult.ErrorResponse);
            ClassicAssert.IsNotNull(patchResult.PatchedData);

            if (patchResult.PatchedData is not null)
            {
                ClassicAssert.AreEqual (Connector_Id.Parse("1"),                          patchResult.PatchedData.Id);
                ClassicAssert.AreEqual (ConnectorType.IEC_62196_T2,                       patchResult.PatchedData.Standard);
                ClassicAssert.AreEqual (ConnectorFormats.SOCKET,                          patchResult.PatchedData.Format);
                ClassicAssert.AreEqual (PowerTypes.AC_3_PHASE,                            patchResult.PatchedData.PowerType);
                ClassicAssert.AreEqual (400,                                              patchResult.PatchedData.Voltage. Value);
                ClassicAssert.AreEqual (30,                                               patchResult.PatchedData.Amperage.Value);
                ClassicAssert.AreEqual (Tariff_Id.Parse("DE*GEF*T0001"),                  patchResult.PatchedData.GetTariffId());
                ClassicAssert.AreEqual (URL.Parse("https://open.charging.cloud/terms"),   patchResult.PatchedData.TermsAndConditionsURL);
                ClassicAssert.AreEqual ("2020-09-21T00:00:00.000Z",                       patchResult.PatchedData.LastUpdated.ToIso8601());
            }

        }

        #endregion


    }

}
