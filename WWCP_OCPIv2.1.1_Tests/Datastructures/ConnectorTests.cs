/*
 * Copyright (c) 2015-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPI <https://github.com/OpenChargingCloud/WWCP_OCPI>
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
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

using NUnit.Framework;

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
                                 Volt.  FromV(400),
                                 Ampere.FromA(30),
                                 Tariff_Id.Parse("DE*GEF*T0001"),
                                 URL.Parse("https://open.charging.cloud/terms"),
                                 DateTime.Parse("2020-09-21T00:00:00Z")
                             );

            var json = Connector1.ToJSON();

            Assert.That(json["id"]?.                  Value<String>(), Is.EqualTo("1"));
            Assert.That(json["standard"]?.            Value<String>(), Is.EqualTo("IEC_62196_T2"));
            Assert.That(json["format"]?.              Value<String>(), Is.EqualTo("SOCKET"));
            Assert.That(json["power_type"]?.          Value<String>(), Is.EqualTo("AC_3_PHASE"));
            Assert.That(json["voltage"]?.             Value<UInt16>(), Is.EqualTo(400));
            Assert.That(json["amperage"]?.            Value<UInt16>(), Is.EqualTo(30));
            Assert.That(json["tariff_id"]?.           Value<String>(), Is.EqualTo("DE*GEF*T0001"));
            Assert.That(json["terms_and_conditions"]?.Value<String>(), Is.EqualTo("https://open.charging.cloud/terms"));
            Assert.That(json["last_updated"]?.        Value<String>(), Is.EqualTo("2020-09-21T00:00:00.000Z"));

            if (Connector.TryParse(json, out var connector2, out var errorResponse))
            {

                Assert.That(connector2,    Is.Not.Null);
                Assert.That(errorResponse, Is.Null);

                Assert.That(connector2.Id,                      Is.EqualTo(Connector1.Id));
                Assert.That(connector2.Standard,                Is.EqualTo(Connector1.Standard));
                Assert.That(connector2.Format,                  Is.EqualTo(Connector1.Format));
                Assert.That(connector2.PowerType,               Is.EqualTo(Connector1.PowerType));
                Assert.That(connector2.Voltage,                 Is.EqualTo(Connector1.Voltage));
                Assert.That(connector2.Amperage,                Is.EqualTo(Connector1.Amperage));
                Assert.That(connector2.GetTariffId(),           Is.EqualTo(Connector1.GetTariffId()));
                Assert.That(connector2.TermsAndConditionsURL,   Is.EqualTo(Connector1.TermsAndConditionsURL));
                Assert.That(connector2.LastUpdated.ToISO8601(), Is.EqualTo(Connector1.LastUpdated.ToISO8601()));

            }
            else
            {
                Assert.Fail("Failed to parse JSON: " + errorResponse);
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
                                 Volt.  FromV(400),
                                 Ampere.FromA(30),
                                 new Dictionary<EMSP_Id, Tariff_Id>() {
                                     { EMSP_Id.Parse("DE-GDF"), Tariff_Id.Parse("DE*GEF*T0001") },
                                     { EMSP_Id.Parse("DE-GD2"), Tariff_Id.Parse("DE*GEF*T0002") }
                                 },
                                 URL.Parse("https://open.charging.cloud/terms"),
                                 DateTime.Parse("2020-09-21T00:00:00Z")
                             );

            var json = Connector1.ToJSON();

            Assert.That(json["id"]?.                  Value<String>(), Is.EqualTo("1"));
            Assert.That(json["standard"]?.            Value<String>(), Is.EqualTo("IEC_62196_T2"));
            Assert.That(json["format"]?.              Value<String>(), Is.EqualTo("SOCKET"));
            Assert.That(json["power_type"]?.          Value<String>(), Is.EqualTo("AC_3_PHASE"));
            Assert.That(json["voltage"]?.             Value<UInt16>(), Is.EqualTo(400));
            Assert.That(json["amperage"]?.            Value<UInt16>(), Is.EqualTo(30));
            Assert.That(json["tariff_id"]?.           Value<String>(), Is.EqualTo(null));
            Assert.That(json["terms_and_conditions"]?.Value<String>(), Is.EqualTo("https://open.charging.cloud/terms"));
            Assert.That(json["last_updated"]?.        Value<String>(), Is.EqualTo("2020-09-21T00:00:00.000Z"));

            var emspTariffIds = json["emsp_tariff_ids"] as JObject;
            Assert.That(emspTariffIds, Is.Not.Null);
            if (emspTariffIds is not null) {
                Assert.That(emspTariffIds["DE-GDF"]?.     Value<String>(), Is.EqualTo("DE*GEF*T0001"));
                Assert.That(emspTariffIds["DE-GD2"]?.     Value<String>(), Is.EqualTo("DE*GEF*T0002"));
            }

            if (Connector.TryParse(json, out var connector2, out var errorResponse))
            {

                Assert.That(connector2,    Is.Not.Null);
                Assert.That(errorResponse, Is.Null);

                Assert.That(connector2.Id,                      Is.EqualTo(Connector1.Id));
                Assert.That(connector2.Standard,                Is.EqualTo(Connector1.Standard));
                Assert.That(connector2.Format,                  Is.EqualTo(Connector1.Format));
                Assert.That(connector2.PowerType,               Is.EqualTo(Connector1.PowerType));
                Assert.That(connector2.Voltage,                 Is.EqualTo(Connector1.Voltage));
                Assert.That(connector2.Amperage,                Is.EqualTo(Connector1.Amperage));
                Assert.That(connector2.GetTariffId(),           Is.EqualTo(Connector1.GetTariffId()));
                Assert.That(connector2.TermsAndConditionsURL,   Is.EqualTo(Connector1.TermsAndConditionsURL));
                Assert.That(connector2.LastUpdated.ToISO8601(), Is.EqualTo(Connector1.LastUpdated.ToISO8601()));

            }
            else
            {
                Assert.Fail("Failed to parse JSON: " + errorResponse);
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
                                 Volt.  FromV(400),
                                 Ampere.FromA(30),
                                 new Dictionary<EMSP_Id, Tariff_Id>() {
                                     { EMSP_Id.Parse("DE-GDF"), Tariff_Id.Parse("DE*GEF*T0001") },
                                     { EMSP_Id.Parse("DE-GD2"), Tariff_Id.Parse("DE*GEF*T0002") }
                                 },
                                 URL.Parse("https://open.charging.cloud/terms"),
                                 DateTime.Parse("2020-09-21T00:00:00Z")
                             );

            var json = Connector1.ToJSON(EMSP_Id.Parse("DE-GDF"));

            Assert.That(json["id"]?.                  Value<String>(), Is.EqualTo("1"));
            Assert.That(json["standard"]?.            Value<String>(), Is.EqualTo("IEC_62196_T2"));
            Assert.That(json["format"]?.              Value<String>(), Is.EqualTo("SOCKET"));
            Assert.That(json["power_type"]?.          Value<String>(), Is.EqualTo("AC_3_PHASE"));
            Assert.That(json["voltage"]?.             Value<UInt16>(), Is.EqualTo(400));
            Assert.That(json["amperage"]?.            Value<UInt16>(), Is.EqualTo(30));
            Assert.That(json["tariff_id"]?.           Value<String>(), Is.EqualTo("DE*GEF*T0001"));
            Assert.That(json["emsp_tariff_ids"]?.     Value<String>(), Is.EqualTo(null));
            Assert.That(json["terms_and_conditions"]?.Value<String>(), Is.EqualTo("https://open.charging.cloud/terms"));
            Assert.That(json["last_updated"]?.        Value<String>(), Is.EqualTo("2020-09-21T00:00:00.000Z"));

            if (Connector.TryParse(json, out var connector2, out var errorResponse))
            {

                Assert.That(connector2,    Is.Not.Null);
                Assert.That(errorResponse, Is.Null);

                Assert.That(connector2.Id,        Is.EqualTo(Connector1.Id));
                Assert.That(connector2.Standard,  Is.EqualTo(Connector1.Standard));
                Assert.That(connector2.Format,    Is.EqualTo(Connector1.Format));
                Assert.That(connector2.PowerType, Is.EqualTo(Connector1.PowerType));
                Assert.That(connector2.Voltage,   Is.EqualTo(Connector1.Voltage));
                Assert.That(connector2.Amperage,  Is.EqualTo(Connector1.Amperage));
                //ClassicAssert.AreEqual(Connector1.GetTariffId(),             connector2.GetTariffId());
                Assert.That(connector2.TermsAndConditionsURL,   Is.EqualTo(Connector1.TermsAndConditionsURL));
                Assert.That(connector2.LastUpdated.ToISO8601(), Is.EqualTo(Connector1.LastUpdated.ToISO8601()));

            }
            else
            {
                Assert.Fail("Failed to parse JSON: " + errorResponse);
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
                                 Volt.  FromV(400),
                                 Ampere.FromA(30),
                                 Tariff_Id.Parse("DE*GEF*T0001"),
                                 URL.Parse("https://open.charging.cloud/terms"),
                                 DateTime.Parse("2020-09-21T00:00:00Z")
                             );

            var patchResult = Connector1.TryPatch(JObject.Parse(@"{ ""id"": ""2"", ""last_updated"": ""2020-10-15T00:00:00Z"" }"));

            Assert.That(patchResult.IsSuccess,     Is.False);
            Assert.That(patchResult.IsFailed,      Is.True);
            Assert.That(patchResult.ErrorResponse, Is.Not.Null);
            Assert.That(patchResult.ErrorResponse, Is.EqualTo("Patching the 'identification' of a connector is not allowed!"));
            Assert.That(patchResult.PatchedData,   Is.Not.Null);

            if (patchResult.PatchedData is not null)
            {
                Assert.That(patchResult.PatchedData.Id,                      Is.EqualTo(Connector_Id.Parse("1")));
                Assert.That(patchResult.PatchedData.Standard,                Is.EqualTo(ConnectorType.IEC_62196_T2));
                Assert.That(patchResult.PatchedData.Format,                  Is.EqualTo(ConnectorFormats.SOCKET));
                Assert.That(patchResult.PatchedData.PowerType,               Is.EqualTo(PowerTypes.AC_3_PHASE));
                Assert.That(patchResult.PatchedData.Voltage. Value,          Is.EqualTo(400));
                Assert.That(patchResult.PatchedData.Amperage.Value,          Is.EqualTo(30));
                Assert.That(patchResult.PatchedData.GetTariffId(),           Is.EqualTo(Tariff_Id.Parse("DE*GEF*T0001")));
                Assert.That(patchResult.PatchedData.TermsAndConditionsURL,   Is.EqualTo(URL.Parse("https://open.charging.cloud/terms")));
                Assert.That(patchResult.PatchedData.LastUpdated.ToISO8601(), Is.EqualTo("2020-09-21T00:00:00.000Z"));
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
                                 Volt.  FromV(400),
                                 Ampere.FromA(30),
                                 Tariff_Id.Parse("DE*GEF*T0001"),
                                 URL.Parse("https://open.charging.cloud/terms"),
                                 DateTime.Parse("2020-09-21T00:00:00Z")
                             );

            var patchResult = Connector1.TryPatch(JObject.Parse(@"{ ""standard"": ""TESLA_S"" }"));

            Assert.That(patchResult.IsSuccess,     Is.True);
            Assert.That(patchResult.IsFailed,      Is.False);
            Assert.That(patchResult.ErrorResponse, Is.Null);
            Assert.That(patchResult.PatchedData,   Is.Not.Null);

            if (patchResult.PatchedData is not null)
            {
                Assert.That(patchResult.PatchedData.Id,                    Is.EqualTo(Connector_Id.Parse("1")));
                Assert.That(patchResult.PatchedData.Standard,              Is.EqualTo(ConnectorType.TESLA_S));
                Assert.That(patchResult.PatchedData.Format,                Is.EqualTo(ConnectorFormats.SOCKET));
                Assert.That(patchResult.PatchedData.PowerType,             Is.EqualTo(PowerTypes.AC_3_PHASE));
                Assert.That(patchResult.PatchedData.Voltage. Value,        Is.EqualTo(400));
                Assert.That(patchResult.PatchedData.Amperage.Value,        Is.EqualTo(30));
                Assert.That(patchResult.PatchedData.GetTariffId(),         Is.EqualTo(Tariff_Id.Parse("DE*GEF*T0001")));
                Assert.That(patchResult.PatchedData.TermsAndConditionsURL, Is.EqualTo(URL.Parse("https://open.charging.cloud/terms")));
                Assert.That(patchResult.PatchedData.LastUpdated,           Is.Not.EqualTo(DateTimeOffset.Parse("2020-09-21T00:00:00Z")));

                Assert.That(Timestamp.Now - patchResult.PatchedData.LastUpdated < TimeSpan.FromSeconds(5), Is.True);
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
                                 Volt.  FromV(400),
                                 Ampere.FromA(30),
                                 Tariff_Id.Parse("DE*GEF*T0001"),
                                 URL.Parse("https://open.charging.cloud/terms"),
                                 DateTime.Parse("2020-09-21T00:00:00Z")
                             );

            var patchResult = Connector1.TryPatch(JObject.Parse(@"{ ""format"": ""CABLE"", ""last_updated"": ""2020-10-15T00:00:00Z"" }"));

            Assert.That(patchResult.IsSuccess,     Is.True);
            Assert.That(patchResult.IsFailed,      Is.False);
            Assert.That(patchResult.ErrorResponse, Is.Null);
            Assert.That(patchResult.PatchedData,   Is.Not.Null);

            if (patchResult.PatchedData is not null)
            {
                Assert.That(patchResult.PatchedData.Id,                      Is.EqualTo(Connector_Id.Parse("1")));
                Assert.That(patchResult.PatchedData.Standard,                Is.EqualTo(ConnectorType.IEC_62196_T2));
                Assert.That(patchResult.PatchedData.Format,                  Is.EqualTo(ConnectorFormats.CABLE));
                Assert.That(patchResult.PatchedData.PowerType,               Is.EqualTo(PowerTypes.AC_3_PHASE));
                Assert.That(patchResult.PatchedData.Voltage. Value,          Is.EqualTo(400));
                Assert.That(patchResult.PatchedData.Amperage.Value,          Is.EqualTo(30));
                Assert.That(patchResult.PatchedData.GetTariffId(),           Is.EqualTo(Tariff_Id.Parse("DE*GEF*T0001")));
                Assert.That(patchResult.PatchedData.TermsAndConditionsURL,   Is.EqualTo(URL.Parse("https://open.charging.cloud/terms")));
                Assert.That(patchResult.PatchedData.LastUpdated.ToISO8601(), Is.EqualTo("2020-10-15T00:00:00.000Z"));
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
                                 Volt.  FromV(400),
                                 Ampere.FromA(30),
                                 Tariff_Id.Parse("DE*GEF*T0001"),
                                 URL.Parse("https://open.charging.cloud/terms"),
                                 DateTime.Parse("2020-09-21T00:00:00Z")
                             );

            var patchResult = Connector1.TryPatch(JObject.Parse(@"{ ""tariff_id"": ""DE*GEF*T0003"", ""last_updated"": ""2020-10-15T00:00:00Z"" }"));

            Assert.That(patchResult.IsSuccess,     Is.True);
            Assert.That(patchResult.IsFailed,      Is.False);
            Assert.That(patchResult.ErrorResponse, Is.Null);
            Assert.That(patchResult.PatchedData,   Is.Not.Null);

            if (patchResult.PatchedData is not null)
            {
                Assert.That(patchResult.PatchedData.Id,                      Is.EqualTo(Connector_Id.Parse("1")));
                Assert.That(patchResult.PatchedData.Standard,                Is.EqualTo(ConnectorType.IEC_62196_T2));
                Assert.That(patchResult.PatchedData.Format,                  Is.EqualTo(ConnectorFormats.SOCKET));
                Assert.That(patchResult.PatchedData.PowerType,               Is.EqualTo(PowerTypes.AC_3_PHASE));
                Assert.That(patchResult.PatchedData.Voltage. Value,          Is.EqualTo(400));
                Assert.That(patchResult.PatchedData.Amperage.Value,          Is.EqualTo(30));
                Assert.That(patchResult.PatchedData.GetTariffId(),           Is.EqualTo(Tariff_Id.Parse("DE*GEF*T0003")));
                Assert.That(patchResult.PatchedData.TermsAndConditionsURL,   Is.EqualTo(URL.Parse("https://open.charging.cloud/terms")));
                Assert.That(patchResult.PatchedData.LastUpdated.ToISO8601(), Is.EqualTo("2020-10-15T00:00:00.000Z"));
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
                                 Volt.  FromV(400),
                                 Ampere.FromA(30),
                                 Tariff_Id.Parse("DE*GEF*T0001"),
                                 URL.Parse("https://open.charging.cloud/terms"),
                                 DateTime.Parse("2020-09-21T00:00:00Z")
                             );

            var patchResult = Connector1.TryPatch(JObject.Parse(@"{ ""tariff_id"": null, ""last_updated"": ""2020-10-15T00:00:00Z"" }"));

            Assert.That(patchResult.IsSuccess,     Is.True);
            Assert.That(patchResult.IsFailed,      Is.False);
            Assert.That(patchResult.ErrorResponse, Is.Null);
            Assert.That(patchResult.PatchedData,   Is.Not.Null);

            if (patchResult.PatchedData is not null)
            {
                Assert.That(patchResult.PatchedData.Id,                      Is.EqualTo(Connector_Id.Parse("1")));
                Assert.That(patchResult.PatchedData.Standard,                Is.EqualTo(ConnectorType.IEC_62196_T2));
                Assert.That(patchResult.PatchedData.Format,                  Is.EqualTo(ConnectorFormats.SOCKET));
                Assert.That(patchResult.PatchedData.PowerType,               Is.EqualTo(PowerTypes.AC_3_PHASE));
                Assert.That(patchResult.PatchedData.Voltage. Value,          Is.EqualTo(400));
                Assert.That(patchResult.PatchedData.Amperage.Value,          Is.EqualTo(30));
                Assert.That(patchResult.PatchedData.GetTariffId(),           Is.Null);
                Assert.That(patchResult.PatchedData.TermsAndConditionsURL,   Is.EqualTo(URL.Parse("https://open.charging.cloud/terms")));
                Assert.That(patchResult.PatchedData.LastUpdated.ToISO8601(), Is.EqualTo("2020-10-15T00:00:00.000Z"));
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
                                 Volt.  FromV(400),
                                 Ampere.FromA(30),
                                 Tariff_Id.Parse("DE*GEF*T0001"),
                                 URL.Parse("https://open.charging.cloud/terms"),
                                 DateTime.Parse("2020-09-21T00:00:00Z")
                             );

            var patchResult = Connector1.TryPatch(JObject.Parse(@"{ ""terms_and_conditions"": null, ""last_updated"": ""2020-10-15T00:00:00Z"" }"));

            Assert.That(patchResult.IsSuccess,     Is.True);
            Assert.That(patchResult.IsFailed,      Is.False);
            Assert.That(patchResult.ErrorResponse, Is.Null);
            Assert.That(patchResult.PatchedData,   Is.Not.Null);

            if (patchResult.PatchedData is not null)
            {
                Assert.That(patchResult.PatchedData.Id,                      Is.EqualTo(Connector_Id.Parse("1")));
                Assert.That(patchResult.PatchedData.Standard,                Is.EqualTo(ConnectorType.IEC_62196_T2));
                Assert.That(patchResult.PatchedData.Format,                  Is.EqualTo(ConnectorFormats.SOCKET));
                Assert.That(patchResult.PatchedData.PowerType,               Is.EqualTo(PowerTypes.AC_3_PHASE));
                Assert.That(patchResult.PatchedData.Voltage. Value,          Is.EqualTo(400));
                Assert.That(patchResult.PatchedData.Amperage.Value,          Is.EqualTo(30));
                Assert.That(patchResult.PatchedData.GetTariffId(),           Is.EqualTo(Tariff_Id.Parse("DE*GEF*T0001")));
                Assert.That(patchResult.PatchedData.TermsAndConditionsURL,   Is.EqualTo(null));
                Assert.That(patchResult.PatchedData.LastUpdated.ToISO8601(), Is.EqualTo("2020-10-15T00:00:00.000Z"));
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
                                 Volt.  FromV(400),
                                 Ampere.FromA(30),
                                 Tariff_Id.Parse("DE*GEF*T0001"),
                                 URL.Parse("https://open.charging.cloud/terms"),
                                 DateTime.Parse("2020-09-21T00:00:00Z")
                             );

            var patchResult = Connector1.TryPatch(JObject.Parse(@"{ ""standard"": null }"));

            Assert.That(patchResult.IsSuccess,     Is.False);
            Assert.That(patchResult.IsFailed,      Is.True);
            Assert.That(patchResult.ErrorResponse, Is.Not.Null);
            Assert.That(patchResult.ErrorResponse, Is.EqualTo("Invalid JSON merge patch of a connector: Invalid 'connector standard'!"));
            Assert.That(patchResult.PatchedData,   Is.Not.Null);

            if (patchResult.PatchedData is not null)
            {
                Assert.That(patchResult.PatchedData.Id,                      Is.EqualTo(Connector_Id.Parse("1")));
                Assert.That(patchResult.PatchedData.Standard,                Is.EqualTo(ConnectorType.IEC_62196_T2));
                Assert.That(patchResult.PatchedData.Format,                  Is.EqualTo(ConnectorFormats.SOCKET));
                Assert.That(patchResult.PatchedData.PowerType,               Is.EqualTo(PowerTypes.AC_3_PHASE));
                Assert.That(patchResult.PatchedData.Voltage. Value,          Is.EqualTo(400));
                Assert.That(patchResult.PatchedData.Amperage.Value,          Is.EqualTo(30));
                Assert.That(patchResult.PatchedData.GetTariffId(),           Is.EqualTo(Tariff_Id.Parse("DE*GEF*T0001")));
                Assert.That(patchResult.PatchedData.TermsAndConditionsURL,   Is.EqualTo(URL.Parse("https://open.charging.cloud/terms")));
                Assert.That(patchResult.PatchedData.LastUpdated.ToISO8601(), Is.EqualTo("2020-09-21T00:00:00.000Z"));
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
                                 Volt.  FromV(400),
                                 Ampere.FromA(30),
                                 Tariff_Id.Parse("DE*GEF*T0001"),
                                 URL.Parse("https://open.charging.cloud/terms"),
                                 DateTime.Parse("2020-09-21T00:00:00Z")
                             );

            var patchResult = Connector1.TryPatch(JObject.Parse(@"{ ""last_updated"": ""I-N-V-A-L-I-D!"" }"));

            Assert.That(patchResult.IsSuccess,     Is.False);
            Assert.That(patchResult.IsFailed,      Is.True);
            Assert.That(patchResult.ErrorResponse, Is.Not.Null);
            Assert.That(patchResult.ErrorResponse, Is.EqualTo("Invalid JSON merge patch of a connector: Invalid 'last updated'!"));
            Assert.That(patchResult.PatchedData,   Is.Not.Null);

            if (patchResult.PatchedData is not null)
            {
                Assert.That(patchResult.PatchedData.Id,                      Is.EqualTo(Connector_Id.Parse("1")));
                Assert.That(patchResult.PatchedData.Standard,                Is.EqualTo(ConnectorType.IEC_62196_T2));
                Assert.That(patchResult.PatchedData.Format,                  Is.EqualTo(ConnectorFormats.SOCKET));
                Assert.That(patchResult.PatchedData.PowerType,               Is.EqualTo(PowerTypes.AC_3_PHASE));
                Assert.That(patchResult.PatchedData.Voltage. Value,          Is.EqualTo(400));
                Assert.That(patchResult.PatchedData.Amperage.Value,          Is.EqualTo(30));
                Assert.That(patchResult.PatchedData.GetTariffId(),           Is.EqualTo(Tariff_Id.Parse("DE*GEF*T0001")));
                Assert.That(patchResult.PatchedData.TermsAndConditionsURL,   Is.EqualTo(URL.Parse("https://open.charging.cloud/terms")));
                Assert.That(patchResult.PatchedData.LastUpdated.ToISO8601(), Is.EqualTo("2020-09-21T00:00:00.000Z"));
            }

        }

        #endregion


    }

}
