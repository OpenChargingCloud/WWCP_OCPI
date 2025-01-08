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

using NUnit.Framework;
using NUnit.Framework.Legacy;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2_1.UnitTests.Datastructures
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
                                 ConnectorType.IEC_62196_T2,
                                 ConnectorFormats.SOCKET,
                                 PowerTypes.AC_3_PHASE,
                                 Volt.  ParseV(400),
                                 Ampere.ParseA(30),
                                 Watt.  ParseW(12),
                                 new[] {
                                     Tariff_Id.Parse("DE*GEF*T0001"),
                                     Tariff_Id.Parse("DE*GEF*T0002")
                                 },
                                 URL.Parse("https://open.charging.cloud/terms"),
                                 DateTime.Parse("2020-09-21T00:00:00Z")
                             );

            var JSON = Connector1.ToJSON();

            ClassicAssert.AreEqual("1",                                  JSON["id"].                  Value<String>());
            ClassicAssert.AreEqual("IEC_62196_T2",                       JSON["standard"].            Value<String>());
            ClassicAssert.AreEqual("SOCKET",                             JSON["format"].              Value<String>());
            ClassicAssert.AreEqual("AC_3_PHASE",                         JSON["power_type"].          Value<String>());
            ClassicAssert.AreEqual(400,                                  JSON["max_voltage"].         Value<UInt16>());
            ClassicAssert.AreEqual(30,                                   JSON["max_amperage"].        Value<UInt16>());
            ClassicAssert.AreEqual(12,                                   JSON["max_electric_power"].  Value<UInt16>());
            ClassicAssert.AreEqual("DE*GEF*T0001",                       JSON["tariff_ids"][0].       Value<String>());
            ClassicAssert.AreEqual("DE*GEF*T0002",                       JSON["tariff_ids"][1].       Value<String>());
            ClassicAssert.AreEqual("https://open.charging.cloud/terms",  JSON["terms_and_conditions"].Value<String>());
            ClassicAssert.AreEqual("2020-09-21T00:00:00.000Z",           JSON["last_updated"].        Value<String>());

            ClassicAssert.IsTrue(Connector.TryParse(JSON, out Connector Connector2, out String ErrorResponse));
            ClassicAssert.IsNull(ErrorResponse);

            ClassicAssert.AreEqual(new[] {
                                Tariff_Id.Parse("DE*GEF*T0001"),
                                Tariff_Id.Parse("DE*GEF*T0002")
                            },
                            Connector2.TariffIds);

            ClassicAssert.AreEqual(Connector1.Id,                        Connector2.Id);
            ClassicAssert.AreEqual(Connector1.Standard,                  Connector2.Standard);
            ClassicAssert.AreEqual(Connector1.Format,                    Connector2.Format);
            ClassicAssert.AreEqual(Connector1.PowerType,                 Connector2.PowerType);
            ClassicAssert.AreEqual(Connector1.MaxVoltage,                Connector2.MaxVoltage);
            ClassicAssert.AreEqual(Connector1.MaxAmperage,               Connector2.MaxAmperage);
            ClassicAssert.AreEqual(Connector1.MaxElectricPower,          Connector2.MaxElectricPower);
            ClassicAssert.AreEqual(Connector1.TariffIds,                 Connector2.TariffIds);
            ClassicAssert.AreEqual(Connector1.TermsAndConditionsURL,     Connector2.TermsAndConditionsURL);
            ClassicAssert.AreEqual(Connector1.LastUpdated.ToIso8601(),   Connector2.LastUpdated.ToIso8601());

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
                                 Watt.  ParseW(12),
                                 new[] {
                                     Tariff_Id.Parse("DE*GEF*T0001"),
                                     Tariff_Id.Parse("DE*GEF*T0002")
                                 },
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

            ClassicAssert.AreEqual (Connector_Id.Parse("1"),                                         patchResult.PatchedData.Id);
            ClassicAssert.AreEqual (ConnectorType.IEC_62196_T2,                                      patchResult.PatchedData.Standard);
            ClassicAssert.AreEqual (ConnectorFormats.SOCKET,                                         patchResult.PatchedData.Format);
            ClassicAssert.AreEqual (PowerTypes.AC_3_PHASE,                                           patchResult.PatchedData.PowerType);
            ClassicAssert.AreEqual (400,                                                             patchResult.PatchedData.MaxVoltage);
            ClassicAssert.AreEqual (30,                                                              patchResult.PatchedData.MaxAmperage);
            ClassicAssert.AreEqual (12,                                                              patchResult.PatchedData.MaxElectricPower);
            ClassicAssert.AreEqual (2,                                                               patchResult.PatchedData.TariffIds.        Count());
            ClassicAssert.AreEqual (Tariff_Id.Parse("DE*GEF*T0001"),                                 patchResult.PatchedData.TariffIds.        First());
            ClassicAssert.AreEqual (Tariff_Id.Parse("DE*GEF*T0002"),                                 patchResult.PatchedData.TariffIds.Skip(1).First());
            ClassicAssert.AreEqual (URL.Parse("https://open.charging.cloud/terms"),                  patchResult.PatchedData.TermsAndConditionsURL);
            ClassicAssert.AreEqual ("2020-09-21T00:00:00.000Z",                                      patchResult.PatchedData.LastUpdated.ToIso8601());

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
                                 Watt.  ParseW(12),
                                 new[] {
                                     Tariff_Id.Parse("DE*GEF*T0001"),
                                     Tariff_Id.Parse("DE*GEF*T0002")
                                 },
                                 URL.Parse("https://open.charging.cloud/terms"),
                                 DateTime.Parse("2020-09-21T00:00:00Z")
                             );

            var patchResult = Connector1.TryPatch(JObject.Parse(@"{ ""standard"": ""TESLA_S"" }"));

            ClassicAssert.IsNotNull(patchResult);
            ClassicAssert.IsTrue   (patchResult.IsSuccess);
            ClassicAssert.IsFalse  (patchResult.IsFailed);
            ClassicAssert.IsNull   (patchResult.ErrorResponse);
            ClassicAssert.IsNotNull(patchResult.PatchedData);

            ClassicAssert.AreEqual   (Connector_Id.Parse("1"),                         patchResult.PatchedData.Id);
            ClassicAssert.AreEqual   (ConnectorType.TESLA_S,                          patchResult.PatchedData.Standard);
            ClassicAssert.AreEqual   (ConnectorFormats.SOCKET,                         patchResult.PatchedData.Format);
            ClassicAssert.AreEqual   (PowerTypes.AC_3_PHASE,                           patchResult.PatchedData.PowerType);
            ClassicAssert.AreEqual   (400,                                             patchResult.PatchedData.MaxVoltage);
            ClassicAssert.AreEqual   (30,                                              patchResult.PatchedData.MaxAmperage);
            ClassicAssert.AreEqual   (12,                                              patchResult.PatchedData.MaxElectricPower);
            ClassicAssert.AreEqual   (2,                                               patchResult.PatchedData.TariffIds.        Count());
            ClassicAssert.AreEqual   (Tariff_Id.Parse("DE*GEF*T0001"),                 patchResult.PatchedData.TariffIds.        First());
            ClassicAssert.AreEqual   (Tariff_Id.Parse("DE*GEF*T0002"),                 patchResult.PatchedData.TariffIds.Skip(1).First());
            ClassicAssert.AreEqual   (URL.Parse("https://open.charging.cloud/terms"),  patchResult.PatchedData.TermsAndConditionsURL);
            ClassicAssert.AreNotEqual(DateTime.Parse("2020-09-21T00:00:00Z"),          patchResult.PatchedData.LastUpdated);

            ClassicAssert.IsTrue     (Timestamp.Now - patchResult.PatchedData.LastUpdated < TimeSpan.FromSeconds(5));

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
                                 Watt.  ParseW(12),
                                 new[] {
                                     Tariff_Id.Parse("DE*GEF*T0001"),
                                     Tariff_Id.Parse("DE*GEF*T0002")
                                 },
                                 URL.Parse("https://open.charging.cloud/terms"),
                                 DateTime.Parse("2020-09-21T00:00:00Z")
                             );

            var patchResult = Connector1.TryPatch(JObject.Parse(@"{ ""format"": ""CABLE"", ""last_updated"": ""2020-10-15T00:00:00Z"" }"));

            ClassicAssert.IsNotNull(patchResult);
            ClassicAssert.IsTrue   (patchResult.IsSuccess);
            ClassicAssert.IsFalse  (patchResult.IsFailed);
            ClassicAssert.IsNull   (patchResult.ErrorResponse);
            ClassicAssert.IsNotNull(patchResult.PatchedData);

            ClassicAssert.AreEqual (Connector_Id.Parse("1"),                         patchResult.PatchedData.Id);
            ClassicAssert.AreEqual (ConnectorType.IEC_62196_T2,                     patchResult.PatchedData.Standard);
            ClassicAssert.AreEqual (ConnectorFormats.CABLE,                          patchResult.PatchedData.Format);
            ClassicAssert.AreEqual (PowerTypes.AC_3_PHASE,                           patchResult.PatchedData.PowerType);
            ClassicAssert.AreEqual (400,                                             patchResult.PatchedData.MaxVoltage);
            ClassicAssert.AreEqual (30,                                              patchResult.PatchedData.MaxAmperage);
            ClassicAssert.AreEqual (12,                                              patchResult.PatchedData.MaxElectricPower);
            ClassicAssert.AreEqual (2,                                               patchResult.PatchedData.TariffIds.        Count());
            ClassicAssert.AreEqual (Tariff_Id.Parse("DE*GEF*T0001"),                 patchResult.PatchedData.TariffIds.        First());
            ClassicAssert.AreEqual (Tariff_Id.Parse("DE*GEF*T0002"),                 patchResult.PatchedData.TariffIds.Skip(1).First());
            ClassicAssert.AreEqual (URL.Parse("https://open.charging.cloud/terms"),  patchResult.PatchedData.TermsAndConditionsURL);
            ClassicAssert.AreEqual ("2020-10-15T00:00:00.000Z",                      patchResult.PatchedData.LastUpdated.ToIso8601());

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
                                 ConnectorType.IEC_62196_T2,
                                 ConnectorFormats.SOCKET,
                                 PowerTypes.AC_3_PHASE,
                                 Volt.  ParseV(400),
                                 Ampere.ParseA(30),
                                 Watt.  ParseW(12),
                                 new[] {
                                     Tariff_Id.Parse("DE*GEF*T0001"),
                                     Tariff_Id.Parse("DE*GEF*T0002")
                                 },
                                 URL.Parse("https://open.charging.cloud/terms"),
                                 DateTime.Parse("2020-09-21T00:00:00Z")
                             );

            var patchResult = Connector1.TryPatch(JObject.Parse(@"{ ""tariff_ids"": [ ""DE*GEF*T0003"" ], ""last_updated"": ""2020-10-15T00:00:00Z"" }"));

            ClassicAssert.IsNotNull(patchResult);
            ClassicAssert.IsTrue   (patchResult.IsSuccess);
            ClassicAssert.IsFalse  (patchResult.IsFailed);
            ClassicAssert.IsNull   (patchResult.ErrorResponse);
            ClassicAssert.IsNotNull(patchResult.PatchedData);

            ClassicAssert.AreEqual (Connector_Id.Parse("1"),                         patchResult.PatchedData.Id);
            ClassicAssert.AreEqual (ConnectorType.IEC_62196_T2,                     patchResult.PatchedData.Standard);
            ClassicAssert.AreEqual (ConnectorFormats.SOCKET,                         patchResult.PatchedData.Format);
            ClassicAssert.AreEqual (PowerTypes.AC_3_PHASE,                           patchResult.PatchedData.PowerType);
            ClassicAssert.AreEqual (400,                                             patchResult.PatchedData.MaxVoltage);
            ClassicAssert.AreEqual (30,                                              patchResult.PatchedData.MaxAmperage);
            ClassicAssert.AreEqual (12,                                              patchResult.PatchedData.MaxElectricPower);
            ClassicAssert.AreEqual (1,                                               patchResult.PatchedData.TariffIds.Count());
            ClassicAssert.AreEqual (Tariff_Id.Parse("DE*GEF*T0003"),                 patchResult.PatchedData.TariffIds.First());
            ClassicAssert.AreEqual (URL.Parse("https://open.charging.cloud/terms"),  patchResult.PatchedData.TermsAndConditionsURL);
            ClassicAssert.AreEqual ("2020-10-15T00:00:00.000Z",                      patchResult.PatchedData.LastUpdated.ToIso8601());

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
                                 ConnectorType.IEC_62196_T2,
                                 ConnectorFormats.SOCKET,
                                 PowerTypes.AC_3_PHASE,
                                 Volt.  ParseV(400),
                                 Ampere.ParseA(30),
                                 Watt.  ParseW(12),
                                 new[] {
                                     Tariff_Id.Parse("DE*GEF*T0001"),
                                     Tariff_Id.Parse("DE*GEF*T0002")
                                 },
                                 URL.Parse("https://open.charging.cloud/terms"),
                                 DateTime.Parse("2020-09-21T00:00:00Z")
                             );

            var patchResult = Connector1.TryPatch(JObject.Parse(@"{ ""tariff_ids"": null, ""last_updated"": ""2020-10-15T00:00:00Z"" }"));

            ClassicAssert.IsNotNull(patchResult);
            ClassicAssert.IsTrue   (patchResult.IsSuccess);
            ClassicAssert.IsFalse  (patchResult.IsFailed);
            ClassicAssert.IsNull   (patchResult.ErrorResponse);
            ClassicAssert.IsNotNull(patchResult.PatchedData);

            ClassicAssert.AreEqual (Connector_Id.Parse("1"),                         patchResult.PatchedData.Id);
            ClassicAssert.AreEqual (ConnectorType.IEC_62196_T2,                     patchResult.PatchedData.Standard);
            ClassicAssert.AreEqual (ConnectorFormats.SOCKET,                         patchResult.PatchedData.Format);
            ClassicAssert.AreEqual (PowerTypes.AC_3_PHASE,                           patchResult.PatchedData.PowerType);
            ClassicAssert.AreEqual (400,                                             patchResult.PatchedData.MaxVoltage);
            ClassicAssert.AreEqual (30,                                              patchResult.PatchedData.MaxAmperage);
            ClassicAssert.AreEqual (12,                                              patchResult.PatchedData.MaxElectricPower);
            ClassicAssert.AreEqual (0,                                               patchResult.PatchedData.TariffIds.Count());
            ClassicAssert.AreEqual (URL.Parse("https://open.charging.cloud/terms"),  patchResult.PatchedData.TermsAndConditionsURL);
            ClassicAssert.AreEqual ("2020-10-15T00:00:00.000Z",                      patchResult.PatchedData.LastUpdated.ToIso8601());

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
                                 Watt.  ParseW(12),
                                 new[] {
                                     Tariff_Id.Parse("DE*GEF*T0001"),
                                     Tariff_Id.Parse("DE*GEF*T0002")
                                 },
                                 URL.Parse("https://open.charging.cloud/terms"),
                                 DateTime.Parse("2020-09-21T00:00:00Z")
                             );

            var patchResult = Connector1.TryPatch(JObject.Parse(@"{ ""terms_and_conditions"": null, ""last_updated"": ""2020-10-15T00:00:00Z"" }"));

            ClassicAssert.IsNotNull(patchResult);
            ClassicAssert.IsTrue   (patchResult.IsSuccess);
            ClassicAssert.IsFalse  (patchResult.IsFailed);
            ClassicAssert.IsNull   (patchResult.ErrorResponse);
            ClassicAssert.IsNotNull(patchResult.PatchedData);

            ClassicAssert.AreEqual (Connector_Id.Parse("1"),          patchResult.PatchedData.Id);
            ClassicAssert.AreEqual (ConnectorType.IEC_62196_T2,      patchResult.PatchedData.Standard);
            ClassicAssert.AreEqual (ConnectorFormats.SOCKET,          patchResult.PatchedData.Format);
            ClassicAssert.AreEqual (PowerTypes.AC_3_PHASE,            patchResult.PatchedData.PowerType);
            ClassicAssert.AreEqual (400,                              patchResult.PatchedData.MaxVoltage);
            ClassicAssert.AreEqual (30,                               patchResult.PatchedData.MaxAmperage);
            ClassicAssert.AreEqual (12,                               patchResult.PatchedData.MaxElectricPower);
            ClassicAssert.AreEqual (2,                                patchResult.PatchedData.TariffIds.        Count());
            ClassicAssert.AreEqual (Tariff_Id.Parse("DE*GEF*T0001"),  patchResult.PatchedData.TariffIds.        First());
            ClassicAssert.AreEqual (Tariff_Id.Parse("DE*GEF*T0002"),  patchResult.PatchedData.TariffIds.Skip(1).First());
            ClassicAssert.AreEqual (null,                             patchResult.PatchedData.TermsAndConditionsURL);
            ClassicAssert.AreEqual ("2020-10-15T00:00:00.000Z",       patchResult.PatchedData.LastUpdated.ToIso8601());

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
                                 ConnectorType.   IEC_62196_T2,
                                 ConnectorFormats.SOCKET,
                                 PowerTypes.      AC_3_PHASE,
                                 Volt.  ParseV(400),
                                 Ampere.ParseA(30),
                                 Watt.  ParseW(12),
                                 new[] {
                                     Tariff_Id.Parse("DE*GEF*T0001"),
                                     Tariff_Id.Parse("DE*GEF*T0002")
                                 },
                                 URL.Parse("https://open.charging.cloud/terms"),
                                 DateTime.Parse("2020-09-21T00:00:00Z").ToUniversalTime()
                             );

            var patchResult = Connector1.TryPatch(JObject.Parse(@"{ ""max_amperage"": ""I-N-V-A-L-I-D!"" }"));

            ClassicAssert.IsNotNull(patchResult);
            ClassicAssert.IsFalse  (patchResult.IsSuccess);
            ClassicAssert.IsTrue   (patchResult.IsFailed);
            ClassicAssert.IsNotNull(patchResult.ErrorResponse);
            ClassicAssert.AreEqual ("Invalid JSON merge patch of a connector: Invalid 'max amperage'!",  patchResult.ErrorResponse);
            ClassicAssert.IsNotNull(patchResult.PatchedData);

            ClassicAssert.AreEqual (Connector_Id.Parse("1"),                                             patchResult.PatchedData.Id);
            ClassicAssert.AreEqual (ConnectorType.IEC_62196_T2,                                          patchResult.PatchedData.Standard);
            ClassicAssert.AreEqual (ConnectorFormats.SOCKET,                                             patchResult.PatchedData.Format);
            ClassicAssert.AreEqual (PowerTypes.AC_3_PHASE,                                               patchResult.PatchedData.PowerType);
            ClassicAssert.AreEqual (400,                                                                 patchResult.PatchedData.MaxVoltage);
            ClassicAssert.AreEqual (30,                                                                  patchResult.PatchedData.MaxAmperage);
            ClassicAssert.AreEqual (12,                                                                  patchResult.PatchedData.MaxElectricPower);
            ClassicAssert.AreEqual (2,                                                                   patchResult.PatchedData.TariffIds.        Count());
            ClassicAssert.AreEqual (Tariff_Id.Parse("DE*GEF*T0001"),                                     patchResult.PatchedData.TariffIds.        First());
            ClassicAssert.AreEqual (Tariff_Id.Parse("DE*GEF*T0002"),                                     patchResult.PatchedData.TariffIds.Skip(1).First());
            ClassicAssert.AreEqual (URL.Parse("https://open.charging.cloud/terms"),                      patchResult.PatchedData.TermsAndConditionsURL);
            ClassicAssert.AreEqual ("2020-09-21T00:00:00.000Z",                                          patchResult.PatchedData.LastUpdated.ToIso8601());

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
                                 Watt.  ParseW(12),
                                 new[] {
                                     Tariff_Id.Parse("DE*GEF*T0001"),
                                     Tariff_Id.Parse("DE*GEF*T0002")
                                 },
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

            ClassicAssert.AreEqual (Connector_Id.Parse("1"),                                             patchResult.PatchedData.Id);
            ClassicAssert.AreEqual (ConnectorType.IEC_62196_T2,                                         patchResult.PatchedData.Standard);
            ClassicAssert.AreEqual (ConnectorFormats.SOCKET,                                             patchResult.PatchedData.Format);
            ClassicAssert.AreEqual (PowerTypes.AC_3_PHASE,                                               patchResult.PatchedData.PowerType);
            ClassicAssert.AreEqual (400,                                                                 patchResult.PatchedData.MaxVoltage);
            ClassicAssert.AreEqual (30,                                                                  patchResult.PatchedData.MaxAmperage);
            ClassicAssert.AreEqual (12,                                                                  patchResult.PatchedData.MaxElectricPower);
            ClassicAssert.AreEqual (2,                                                                   patchResult.PatchedData.TariffIds.        Count());
            ClassicAssert.AreEqual (Tariff_Id.Parse("DE*GEF*T0001"),                                     patchResult.PatchedData.TariffIds.        First());
            ClassicAssert.AreEqual (Tariff_Id.Parse("DE*GEF*T0002"),                                     patchResult.PatchedData.TariffIds.Skip(1).First());
            ClassicAssert.AreEqual (URL.Parse("https://open.charging.cloud/terms"),                      patchResult.PatchedData.TermsAndConditionsURL);
            ClassicAssert.AreEqual ("2020-09-21T00:00:00.000Z",                                          patchResult.PatchedData.LastUpdated.ToIso8601());

        }

        #endregion

    }

}
