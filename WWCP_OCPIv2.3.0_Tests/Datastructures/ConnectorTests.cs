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

using NUnit.Framework;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_3_0.UnitTests.Datastructures
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
                                 Volt.  FromV(400),
                                 Ampere.FromA (30),
                                 Watt.  FromKW(12),
                                 [
                                     Tariff_Id.Parse("DE*GEF*T0001"),
                                     Tariff_Id.Parse("DE*GEF*T0002")
                                 ],
                                 URL.Parse("https://open.charging.cloud/terms"),
                                 [ ConnectorCapability.ISO_15118_2_PLUG_AND_CHARGE ],
                                 DateTime.Parse("2020-09-21T00:00:00Z"),
                                 DateTime.Parse("2020-09-21T00:00:00Z")
                             );

            var JSON = Connector1.ToJSON();

            Assert.That(JSON["id"].                  Value<String>(), Is.EqualTo("1"));
            Assert.That(JSON["standard"].            Value<String>(), Is.EqualTo("IEC_62196_T2"));
            Assert.That(JSON["format"].              Value<String>(), Is.EqualTo("SOCKET"));
            Assert.That(JSON["power_type"].          Value<String>(), Is.EqualTo("AC_3_PHASE"));
            Assert.That(JSON["max_voltage"].         Value<UInt16>(), Is.EqualTo(400));
            Assert.That(JSON["max_amperage"].        Value<UInt16>(), Is.EqualTo(30));
            Assert.That(JSON["max_electric_power"].  Value<UInt16>(), Is.EqualTo(12));
            Assert.That(JSON["tariff_ids"][0].       Value<String>(), Is.EqualTo("DE*GEF*T0001"));
            Assert.That(JSON["tariff_ids"][1].       Value<String>(), Is.EqualTo("DE*GEF*T0002"));
            Assert.That(JSON["terms_and_conditions"].Value<String>(), Is.EqualTo("https://open.charging.cloud/terms"));
            Assert.That(JSON["last_updated"].        Value<String>(), Is.EqualTo("2020-09-21T00:00:00.000Z"));

            Assert.That(Connector.TryParse(JSON, out var Connector2, out var ErrorResponse), Is.True);
            Assert.That(ErrorResponse,                                                       Is.Null);

            Assert.That(Connector2.TariffIds, Is.EqualTo(new[] {
                            Tariff_Id.Parse("DE*GEF*T0001"),
                            Tariff_Id.Parse("DE*GEF*T0002")
                        }));

            Assert.That(Connector2.Id,                      Is.EqualTo(Connector1.Id));
            Assert.That(Connector2.Standard,                Is.EqualTo(Connector1.Standard));
            Assert.That(Connector2.Format,                  Is.EqualTo(Connector1.Format));
            Assert.That(Connector2.PowerType,               Is.EqualTo(Connector1.PowerType));
            Assert.That(Connector2.MaxVoltage,              Is.EqualTo(Connector1.MaxVoltage));
            Assert.That(Connector2.MaxAmperage,             Is.EqualTo(Connector1.MaxAmperage));
            Assert.That(Connector2.MaxElectricPower,        Is.EqualTo(Connector1.MaxElectricPower));
            Assert.That(Connector2.TariffIds,               Is.EqualTo(Connector1.TariffIds));
            Assert.That(Connector2.TermsAndConditionsURL,   Is.EqualTo(Connector1.TermsAndConditionsURL));
            Assert.That(Connector2.LastUpdated.ToISO8601(), Is.EqualTo(Connector1.LastUpdated.ToISO8601()));

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
                                 Ampere.FromA (30),
                                 Watt.  FromKW(12),
                                 [
                                     Tariff_Id.Parse("DE*GEF*T0001"),
                                     Tariff_Id.Parse("DE*GEF*T0002")
                                 ],
                                 URL.Parse("https://open.charging.cloud/terms"),
                                 [ ConnectorCapability.ISO_15118_2_PLUG_AND_CHARGE ],
                                 DateTime.Parse("2020-09-21T00:00:00Z"),
                                 DateTime.Parse("2020-09-21T00:00:00Z")
                             );

            var patchResult = Connector1.TryPatch(JObject.Parse(@"{ ""id"": ""2"", ""last_updated"": ""2020-10-15T00:00:00Z"" }"));

            Assert.That(patchResult.IsSuccess,     Is.False);
            Assert.That(patchResult.IsFailed,      Is.True);
            Assert.That(patchResult.ErrorResponse, Is.Not.Null);
            Assert.That(patchResult.ErrorResponse, Is.EqualTo("Patching the 'identification' of a connector is not allowed!"));
            Assert.That(patchResult.PatchedData,   Is.Not.Null);

            Assert.That(patchResult.PatchedData.Id,                        Is.EqualTo(Connector_Id.Parse("1")));
            Assert.That(patchResult.PatchedData.Standard,                  Is.EqualTo(ConnectorType.IEC_62196_T2));
            Assert.That(patchResult.PatchedData.Format,                    Is.EqualTo(ConnectorFormats.SOCKET));
            Assert.That(patchResult.PatchedData.PowerType,                 Is.EqualTo(PowerTypes.AC_3_PHASE));
            Assert.That(patchResult.PatchedData.MaxVoltage,                Is.EqualTo(Volt.FromV(400)));
            Assert.That(patchResult.PatchedData.MaxAmperage,               Is.EqualTo(Ampere.FromA(30)));
            Assert.That(patchResult.PatchedData.MaxElectricPower,          Is.EqualTo(Watt.FromKW(12)));
            Assert.That(patchResult.PatchedData.TariffIds.        Count(), Is.EqualTo(2));
            Assert.That(patchResult.PatchedData.TariffIds.        First(), Is.EqualTo(Tariff_Id.Parse("DE*GEF*T0001")));
            Assert.That(patchResult.PatchedData.TariffIds.Skip(1).First(), Is.EqualTo(Tariff_Id.Parse("DE*GEF*T0002")));
            Assert.That(patchResult.PatchedData.TermsAndConditionsURL,     Is.EqualTo(URL.Parse("https://open.charging.cloud/terms")));
            Assert.That(patchResult.PatchedData.LastUpdated.ToISO8601(),   Is.EqualTo("2020-09-21T00:00:00.000Z"));

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
                                 Ampere.FromA (30),
                                 Watt.  FromKW(12),
                                 [
                                     Tariff_Id.Parse("DE*GEF*T0001"),
                                     Tariff_Id.Parse("DE*GEF*T0002")
                                 ],
                                 URL.Parse("https://open.charging.cloud/terms"),
                                 [ ConnectorCapability.ISO_15118_2_PLUG_AND_CHARGE ],
                                 DateTime.Parse("2020-09-21T00:00:00Z"),
                                 DateTime.Parse("2020-09-21T00:00:00Z")
                             );

            var patchResult = Connector1.TryPatch(JObject.Parse(@"{ ""standard"": ""TESLA_S"" }"));

            Assert.That(patchResult.IsSuccess,     Is.True);
            Assert.That(patchResult.IsFailed,      Is.False);
            Assert.That(patchResult.ErrorResponse, Is.Null);
            Assert.That(patchResult.PatchedData,   Is.Not.Null);

            Assert.That(patchResult.PatchedData.Id,                        Is.EqualTo(Connector_Id.Parse("1")));
            Assert.That(patchResult.PatchedData.Standard,                  Is.EqualTo(ConnectorType.TESLA_S));
            Assert.That(patchResult.PatchedData.Format,                    Is.EqualTo(ConnectorFormats.SOCKET));
            Assert.That(patchResult.PatchedData.PowerType,                 Is.EqualTo(PowerTypes.AC_3_PHASE));
            Assert.That(patchResult.PatchedData.MaxVoltage,                Is.EqualTo(Volt.FromV(400)));
            Assert.That(patchResult.PatchedData.MaxAmperage,               Is.EqualTo(Ampere.FromA(30)));
            Assert.That(patchResult.PatchedData.MaxElectricPower,          Is.EqualTo(Watt.FromKW(12)));
            Assert.That(patchResult.PatchedData.TariffIds.        Count(), Is.EqualTo(2));
            Assert.That(patchResult.PatchedData.TariffIds.        First(), Is.EqualTo(Tariff_Id.Parse("DE*GEF*T0001")));
            Assert.That(patchResult.PatchedData.TariffIds.Skip(1).First(), Is.EqualTo(Tariff_Id.Parse("DE*GEF*T0002")));
            Assert.That(patchResult.PatchedData.TermsAndConditionsURL,     Is.EqualTo(URL.Parse("https://open.charging.cloud/terms")));
            Assert.That(patchResult.PatchedData.LastUpdated,               Is.Not.EqualTo(DateTimeOffset.Parse("2020-09-21T00:00:00Z")));

            Assert.That(Timestamp.Now - patchResult.PatchedData.LastUpdated < TimeSpan.FromSeconds(5), Is.True);

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
                                 Ampere.FromA (30),
                                 Watt.  FromKW(12),
                                 [
                                     Tariff_Id.Parse("DE*GEF*T0001"),
                                     Tariff_Id.Parse("DE*GEF*T0002")
                                 ],
                                 URL.Parse("https://open.charging.cloud/terms"),
                                 [ ConnectorCapability.ISO_15118_2_PLUG_AND_CHARGE ],
                                 DateTime.Parse("2020-09-21T00:00:00Z"),
                                 DateTime.Parse("2020-09-21T00:00:00Z")
                             );

            var patchResult = Connector1.TryPatch(JObject.Parse(@"{ ""format"": ""CABLE"", ""last_updated"": ""2020-10-15T00:00:00Z"" }"));

            Assert.That(patchResult.IsSuccess,     Is.True);
            Assert.That(patchResult.IsFailed,      Is.False);
            Assert.That(patchResult.ErrorResponse, Is.Null);
            Assert.That(patchResult.PatchedData,   Is.Not.Null);

            Assert.That(patchResult.PatchedData.Id,                        Is.EqualTo(Connector_Id.Parse("1")));
            Assert.That(patchResult.PatchedData.Standard,                  Is.EqualTo(ConnectorType.IEC_62196_T2));
            Assert.That(patchResult.PatchedData.Format,                    Is.EqualTo(ConnectorFormats.CABLE));
            Assert.That(patchResult.PatchedData.PowerType,                 Is.EqualTo(PowerTypes.AC_3_PHASE));
            Assert.That(patchResult.PatchedData.MaxVoltage,                Is.EqualTo(Volt.FromV(400)));
            Assert.That(patchResult.PatchedData.MaxAmperage,               Is.EqualTo(Ampere.FromA(30)));
            Assert.That(patchResult.PatchedData.MaxElectricPower,          Is.EqualTo(Watt.FromKW(12)));
            Assert.That(patchResult.PatchedData.TariffIds.        Count(), Is.EqualTo(2));
            Assert.That(patchResult.PatchedData.TariffIds.        First(), Is.EqualTo(Tariff_Id.Parse("DE*GEF*T0001")));
            Assert.That(patchResult.PatchedData.TariffIds.Skip(1).First(), Is.EqualTo(Tariff_Id.Parse("DE*GEF*T0002")));
            Assert.That(patchResult.PatchedData.TermsAndConditionsURL,     Is.EqualTo(URL.Parse("https://open.charging.cloud/terms")));
            Assert.That(patchResult.PatchedData.LastUpdated.ToISO8601(),   Is.EqualTo("2020-10-15T00:00:00.000Z"));

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
                                 Volt.  FromV(400),
                                 Ampere.FromA (30),
                                 Watt.  FromKW(12),
                                 [
                                     Tariff_Id.Parse("DE*GEF*T0001"),
                                     Tariff_Id.Parse("DE*GEF*T0002")
                                 ],
                                 URL.Parse("https://open.charging.cloud/terms"),
                                 [ ConnectorCapability.ISO_15118_2_PLUG_AND_CHARGE ],
                                 DateTime.Parse("2020-09-21T00:00:00Z"),
                                 DateTime.Parse("2020-09-21T00:00:00Z")
                             );

            var patchResult = Connector1.TryPatch(JObject.Parse(@"{ ""tariff_ids"": [ ""DE*GEF*T0003"" ], ""last_updated"": ""2020-10-15T00:00:00Z"" }"));

            Assert.That(patchResult.IsSuccess,     Is.True);
            Assert.That(patchResult.IsFailed,      Is.False);
            Assert.That(patchResult.ErrorResponse, Is.Null);
            Assert.That(patchResult.PatchedData,   Is.Not.Null);

            Assert.That(patchResult.PatchedData.Id,                      Is.EqualTo(Connector_Id.Parse("1")));
            Assert.That(patchResult.PatchedData.Standard,                Is.EqualTo(ConnectorType.IEC_62196_T2));
            Assert.That(patchResult.PatchedData.Format,                  Is.EqualTo(ConnectorFormats.SOCKET));
            Assert.That(patchResult.PatchedData.PowerType,               Is.EqualTo(PowerTypes.AC_3_PHASE));
            Assert.That(patchResult.PatchedData.MaxVoltage,              Is.EqualTo(Volt.FromV(400)));
            Assert.That(patchResult.PatchedData.MaxAmperage,             Is.EqualTo(Ampere.FromA(30)));
            Assert.That(patchResult.PatchedData.MaxElectricPower,        Is.EqualTo(Watt.FromKW(12)));
            Assert.That(patchResult.PatchedData.TariffIds.Count(),       Is.EqualTo(1));
            Assert.That(patchResult.PatchedData.TariffIds.First(),       Is.EqualTo(Tariff_Id.Parse("DE*GEF*T0003")));
            Assert.That(patchResult.PatchedData.TermsAndConditionsURL,   Is.EqualTo(URL.Parse("https://open.charging.cloud/terms")));
            Assert.That(patchResult.PatchedData.LastUpdated.ToISO8601(), Is.EqualTo("2020-10-15T00:00:00.000Z"));

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
                                 Volt.  FromV(400),
                                 Ampere.FromA (30),
                                 Watt.  FromKW(12),
                                 [
                                     Tariff_Id.Parse("DE*GEF*T0001"),
                                     Tariff_Id.Parse("DE*GEF*T0002")
                                 ],
                                 URL.Parse("https://open.charging.cloud/terms"),
                                 [ ConnectorCapability.ISO_15118_2_PLUG_AND_CHARGE ],
                                 DateTime.Parse("2020-09-21T00:00:00Z"),
                                 DateTime.Parse("2020-09-21T00:00:00Z")
                             );

            var patchResult = Connector1.TryPatch(JObject.Parse(@"{ ""tariff_ids"": null, ""last_updated"": ""2020-10-15T00:00:00Z"" }"));

            Assert.That(patchResult.IsSuccess,     Is.True);
            Assert.That(patchResult.IsFailed,      Is.False);
            Assert.That(patchResult.ErrorResponse, Is.Null);
            Assert.That(patchResult.PatchedData,   Is.Not.Null);

            Assert.That(patchResult.PatchedData.Id,                      Is.EqualTo(Connector_Id.Parse("1")));
            Assert.That(patchResult.PatchedData.Standard,                Is.EqualTo(ConnectorType.IEC_62196_T2));
            Assert.That(patchResult.PatchedData.Format,                  Is.EqualTo(ConnectorFormats.SOCKET));
            Assert.That(patchResult.PatchedData.PowerType,               Is.EqualTo(PowerTypes.AC_3_PHASE));
            Assert.That(patchResult.PatchedData.MaxVoltage,              Is.EqualTo(Volt.FromV(400)));
            Assert.That(patchResult.PatchedData.MaxAmperage,             Is.EqualTo(Ampere.FromA(30)));
            Assert.That(patchResult.PatchedData.MaxElectricPower,        Is.EqualTo(Watt.FromKW(12)));
            Assert.That(patchResult.PatchedData.TariffIds.Count(),       Is.EqualTo(0));
            Assert.That(patchResult.PatchedData.TermsAndConditionsURL,   Is.EqualTo(URL.Parse("https://open.charging.cloud/terms")));
            Assert.That(patchResult.PatchedData.LastUpdated.ToISO8601(), Is.EqualTo("2020-10-15T00:00:00.000Z"));

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
                                 Ampere.FromA (30),
                                 Watt.  FromKW(12),
                                 [
                                     Tariff_Id.Parse("DE*GEF*T0001"),
                                     Tariff_Id.Parse("DE*GEF*T0002")
                                 ],
                                 URL.Parse("https://open.charging.cloud/terms"),
                                 [ ConnectorCapability.ISO_15118_2_PLUG_AND_CHARGE ],
                                 DateTime.Parse("2020-09-21T00:00:00Z"),
                                 DateTime.Parse("2020-09-21T00:00:00Z")
                             );

            var patchResult = Connector1.TryPatch(JObject.Parse(@"{ ""terms_and_conditions"": null, ""last_updated"": ""2020-10-15T00:00:00Z"" }"));

            Assert.That(patchResult.IsSuccess,     Is.True);
            Assert.That(patchResult.IsFailed,      Is.False);
            Assert.That(patchResult.ErrorResponse, Is.Null);
            Assert.That(patchResult.PatchedData,   Is.Not.Null);

            Assert.That(patchResult.PatchedData.Id,                        Is.EqualTo(Connector_Id.Parse("1")));
            Assert.That(patchResult.PatchedData.Standard,                  Is.EqualTo(ConnectorType.IEC_62196_T2));
            Assert.That(patchResult.PatchedData.Format,                    Is.EqualTo(ConnectorFormats.SOCKET));
            Assert.That(patchResult.PatchedData.PowerType,                 Is.EqualTo(PowerTypes.AC_3_PHASE));
            Assert.That(patchResult.PatchedData.MaxVoltage,                Is.EqualTo(Volt.FromV(400)));
            Assert.That(patchResult.PatchedData.MaxAmperage,               Is.EqualTo(Ampere.FromA(30)));
            Assert.That(patchResult.PatchedData.MaxElectricPower,          Is.EqualTo(Watt.FromKW(12)));
            Assert.That(patchResult.PatchedData.TariffIds.        Count(), Is.EqualTo(2));
            Assert.That(patchResult.PatchedData.TariffIds.        First(), Is.EqualTo(Tariff_Id.Parse("DE*GEF*T0001")));
            Assert.That(patchResult.PatchedData.TariffIds.Skip(1).First(), Is.EqualTo(Tariff_Id.Parse("DE*GEF*T0002")));
            Assert.That(patchResult.PatchedData.TermsAndConditionsURL,     Is.EqualTo(null));
            Assert.That(patchResult.PatchedData.LastUpdated.ToISO8601(),   Is.EqualTo("2020-10-15T00:00:00.000Z"));

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
                                 Volt.  FromV(400),
                                 Ampere.FromA (30),
                                 Watt.  FromKW(12),
                                 [
                                     Tariff_Id.Parse("DE*GEF*T0001"),
                                     Tariff_Id.Parse("DE*GEF*T0002")
                                 ],
                                 URL.Parse("https://open.charging.cloud/terms"),
                                 [ ConnectorCapability.ISO_15118_2_PLUG_AND_CHARGE ],
                                 DateTime.Parse("2020-09-21T00:00:00Z"),
                                 DateTime.Parse("2020-09-21T00:00:00Z").ToUniversalTime()
                             );

            var patchResult = Connector1.TryPatch(JObject.Parse(@"{ ""max_amperage"": ""I-N-V-A-L-I-D!"" }"));

            Assert.That(patchResult.IsSuccess,     Is.False);
            Assert.That(patchResult.IsFailed,      Is.True);
            Assert.That(patchResult.ErrorResponse, Is.Not.Null);
            Assert.That(patchResult.ErrorResponse, Is.EqualTo("Invalid JSON merge patch of a connector: Invalid 'max amperage'!"));
            Assert.That(patchResult.PatchedData,   Is.Not.Null);

            Assert.That(patchResult.PatchedData.Id,                        Is.EqualTo(Connector_Id.Parse("1")));
            Assert.That(patchResult.PatchedData.Standard,                  Is.EqualTo(ConnectorType.IEC_62196_T2));
            Assert.That(patchResult.PatchedData.Format,                    Is.EqualTo(ConnectorFormats.SOCKET));
            Assert.That(patchResult.PatchedData.PowerType,                 Is.EqualTo(PowerTypes.AC_3_PHASE));
            Assert.That(patchResult.PatchedData.MaxVoltage,                Is.EqualTo(Volt.FromV(400)));
            Assert.That(patchResult.PatchedData.MaxAmperage,               Is.EqualTo(Ampere.FromA(30)));
            Assert.That(patchResult.PatchedData.MaxElectricPower,          Is.EqualTo(Watt.FromKW(12)));
            Assert.That(patchResult.PatchedData.TariffIds.        Count(), Is.EqualTo(2));
            Assert.That(patchResult.PatchedData.TariffIds.        First(), Is.EqualTo(Tariff_Id.Parse("DE*GEF*T0001")));
            Assert.That(patchResult.PatchedData.TariffIds.Skip(1).First(), Is.EqualTo(Tariff_Id.Parse("DE*GEF*T0002")));
            Assert.That(patchResult.PatchedData.TermsAndConditionsURL,     Is.EqualTo(URL.Parse("https://open.charging.cloud/terms")));
            Assert.That(patchResult.PatchedData.LastUpdated.ToISO8601(),   Is.EqualTo("2020-09-21T00:00:00.000Z"));

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
                                 Ampere.FromA (30),
                                 Watt.  FromKW(12),
                                 [
                                     Tariff_Id.Parse("DE*GEF*T0001"),
                                     Tariff_Id.Parse("DE*GEF*T0002")
                                 ],
                                 URL.Parse("https://open.charging.cloud/terms"),
                                 [ ConnectorCapability.ISO_15118_2_PLUG_AND_CHARGE ],
                                 DateTime.Parse("2020-09-21T00:00:00Z"),
                                 DateTime.Parse("2020-09-21T00:00:00Z")
                             );

            var patchResult = Connector1.TryPatch(JObject.Parse(@"{ ""last_updated"": ""I-N-V-A-L-I-D!"" }"));

            Assert.That(patchResult.IsSuccess,     Is.False);
            Assert.That(patchResult.IsFailed,      Is.True);
            Assert.That(patchResult.ErrorResponse, Is.Not.Null);
            Assert.That(patchResult.ErrorResponse, Is.EqualTo("Invalid JSON merge patch of a connector: Invalid 'last updated'!"));
            Assert.That(patchResult.PatchedData,   Is.Not.Null);

            Assert.That(patchResult.PatchedData.Id,                        Is.EqualTo(Connector_Id.Parse("1")));
            Assert.That(patchResult.PatchedData.Standard,                  Is.EqualTo(ConnectorType.IEC_62196_T2));
            Assert.That(patchResult.PatchedData.Format,                    Is.EqualTo(ConnectorFormats.SOCKET));
            Assert.That(patchResult.PatchedData.PowerType,                 Is.EqualTo(PowerTypes.AC_3_PHASE));
            Assert.That(patchResult.PatchedData.MaxVoltage,                Is.EqualTo(Volt.FromV(400)));
            Assert.That(patchResult.PatchedData.MaxAmperage,               Is.EqualTo(Ampere.FromA(30)));
            Assert.That(patchResult.PatchedData.MaxElectricPower,          Is.EqualTo(Watt.FromKW(12)));
            Assert.That(patchResult.PatchedData.TariffIds.        Count(), Is.EqualTo(2));
            Assert.That(patchResult.PatchedData.TariffIds.        First(), Is.EqualTo(Tariff_Id.Parse("DE*GEF*T0001")));
            Assert.That(patchResult.PatchedData.TariffIds.Skip(1).First(), Is.EqualTo(Tariff_Id.Parse("DE*GEF*T0002")));
            Assert.That(patchResult.PatchedData.TermsAndConditionsURL,     Is.EqualTo(URL.Parse("https://open.charging.cloud/terms")));
            Assert.That(patchResult.PatchedData.LastUpdated.ToISO8601(),   Is.EqualTo("2020-09-21T00:00:00.000Z"));

        }

        #endregion

    }

}
