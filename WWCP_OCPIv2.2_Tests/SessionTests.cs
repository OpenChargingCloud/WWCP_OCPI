/*
 * Copyright (c) 2015-2022 GraphDefined GmbH
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
using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2.UnitTests
{

    /// <summary>
    /// Unit tests for sessions.
    /// https://github.com/ocpi/ocpi/blob/master/mod_sessions.asciidoc
    /// </summary>
    [TestFixture]
    public static class SessionTests
    {

        #region Session_SerializeDeserialize_Test01()

        /// <summary>
        /// Session serialize, deserialize and compare test.
        /// </summary>
        [Test]
        public static void Session_SerializeDeserialize_Test01()
        {

            #region Define Session1

            var session1 = new Session(
                               CountryCode.Parse("DE"),
                               Party_Id.   Parse("GEF"),
                               Session_Id. Parse("Session0001"),
                               DateTime.Parse("2020-08-21T00:00:00.000Z").ToUniversalTime(), // Start
                               1.11M, // KWh
                               new CDRToken(
                                   CountryCode.Parse("DE"),
                                   Party_Id.   Parse("GEF"),
                                   Token_Id.   Parse("1234"),
                                   TokenType.RFID,
                                   Contract_Id.Parse("Contract0815")
                               ),
                               AuthMethods.AUTH_REQUEST,
                               Location_Id. Parse("LOC0001"),
                               EVSE_UId.    Parse("EVSE0001"),
                               Connector_Id.Parse("C1"),
                               Currency.EUR,
                               SessionStatusTypes.ACTIVE,
                               DateTime.Parse("2020-08-22T00:00:00.000Z").ToUniversalTime(), // End
                               AuthorizationReference.Parse("Auth1234"),

                               Meter_Id.Parse("Meter0001"),

                               // OCPI Computer Science Extensions
                               new EnergyMeter(
                                   Meter_Id.Parse("Meter0815"),
                                   "EnergyMeter Model #1",
                                   "hw. v1.80",
                                   "fw. v1.20",
                                   "Energy Metering Services",
                                   null,
                                   null
                               ),

                               // OCPI Computer Science Extensions
                               new TransparencySoftware[] {
                                   new TransparencySoftware(
                                       "Chargy Transparency Software Desktop Application",
                                       "v1.00",
                                       LegalStatus.LegallyBinding,
                                       OpenSourceLicenses.GPL3,
                                       "GraphDefined GmbH",
                                       URL.Parse("https://open.charging.cloud/logo.svg"),
                                       URL.Parse("https://open.charging.cloud/Chargy/howto"),
                                       URL.Parse("https://open.charging.cloud/Chargy"),
                                       URL.Parse("https://github.com/OpenChargingCloud/ChargyDesktopApp")
                                   ),
                                   new TransparencySoftware(
                                       "Chargy Transparency Software Mobile Application",
                                       "v1.00",
                                       LegalStatus.ForInformationOnly,
                                       OpenSourceLicenses.GPL3,
                                       "GraphDefined GmbH",
                                       URL.Parse("https://open.charging.cloud/logo.svg"),
                                       URL.Parse("https://open.charging.cloud/Chargy/howto"),
                                       URL.Parse("https://open.charging.cloud/Chargy"),
                                       URL.Parse("https://github.com/OpenChargingCloud/ChargyMobileApp")
                                   )
                               },

                               new ChargingPeriod[] {
                                   new ChargingPeriod(
                                       DateTime.Parse("2020-04-12T18:21:49Z").ToUniversalTime(),
                                       new CDRDimension[] {
                                           new CDRDimension(
                                               CDRDimensionType.ENERGY,
                                               1.33M
                                           )
                                       },
                                       Tariff_Id.Parse("DE*GEF*T0001")
                                   ),
                                   new ChargingPeriod(
                                       DateTime.Parse("2020-04-12T18:21:50Z").ToUniversalTime(),
                                       new CDRDimension[] {
                                           new CDRDimension(
                                               CDRDimensionType.TIME,
                                               5.12M
                                           )
                                       },
                                       Tariff_Id.Parse("DE*GEF*T0002")
                                   )
                               },

                               // Total Costs
                               new Price(
                                   1.12,
                                   2.24
                               ),

                               DateTime.Parse("2020-09-21T00:00:00.000Z").ToUniversalTime()
                           );

            #endregion

            var JSON = session1.ToJSON();

            Assert.AreEqual("DE",                              JSON["country_code"].                    Value<String>());
            Assert.AreEqual("GEF",                             JSON["party_id"].                        Value<String>());
            Assert.AreEqual("Session0001",                     JSON["id"].                              Value<String>());
            Assert.AreEqual("2020-08-21T00:00:00.000Z",        JSON["start_date_time"].                 Value<String>());
            Assert.AreEqual("2020-08-22T00:00:00.000Z",        JSON["end_date_time"].                   Value<String>());
            Assert.AreEqual(1.11,                              JSON["kwh"].                             Value<Decimal>());
            Assert.AreEqual("1234",                            JSON["cdr_token"]["uid"].                Value<String>());
            Assert.AreEqual("RFID",                            JSON["cdr_token"]["type"].               Value<String>());
            Assert.AreEqual("Contract0815",                    JSON["cdr_token"]["contract_id"].        Value<String>());
            Assert.AreEqual("AUTH_REQUEST",                    JSON["auth_method"].                     Value<String>());
            Assert.AreEqual("Auth1234",                        JSON["authorization_reference"].         Value<String>());
            Assert.AreEqual("LOC0001",                         JSON["location_id"].                     Value<String>());
            Assert.AreEqual("EVSE0001",                        JSON["evse_uid"].                        Value<String>());
            Assert.AreEqual("C1",                              JSON["connector_id"].                    Value<String>());
            Assert.AreEqual("Meter0001",                       JSON["meter_id"].                        Value<String>());
            Assert.AreEqual("EUR",                             JSON["currency"].                        Value<String>());
            //Assert.AreEqual("Stadtwerke Jena-Ost",             JSON["charging_periods"]["xxx"].Value<String>());
            Assert.AreEqual(1.12,                              JSON["total_cost"]["excl_vat"].          Value<Decimal>());
            Assert.AreEqual(2.24,                              JSON["total_cost"]["incl_vat"].          Value<Decimal>());
            Assert.AreEqual("ACTIVE",                          JSON["status"].                          Value<String>());
            Assert.AreEqual("2020-09-21T00:00:00.000Z",        JSON["last_updated"].                    Value<String>());

            Assert.IsTrue(Session.TryParse(JSON, out var session2, out var errorResponse));
            Assert.IsNull(errorResponse);

            Assert.AreEqual(session1.CountryCode,              session2.CountryCode);
            Assert.AreEqual(session1.PartyId,                  session2.PartyId);
            Assert.AreEqual(session1.Id,                       session2.Id);
            Assert.AreEqual(session1.Start.    ToIso8601(),    session2.Start.    ToIso8601());
            Assert.AreEqual(session1.End.Value.ToIso8601(),    session2.End.Value.ToIso8601());
            Assert.AreEqual(session1.kWh,                      session2.kWh);
            Assert.AreEqual(session1.CDRToken,                 session2.CDRToken);
            Assert.AreEqual(session1.AuthMethod,               session2.AuthMethod);
            Assert.AreEqual(session1.AuthorizationReference,   session2.AuthorizationReference);
            Assert.AreEqual(session1.LocationId,               session2.LocationId);
            Assert.AreEqual(session1.EVSEUId,                  session2.EVSEUId);
            Assert.AreEqual(session1.ConnectorId,              session2.ConnectorId);
            Assert.AreEqual(session1.MeterId,                  session2.MeterId);
            Assert.AreEqual(session1.EnergyMeter,              session2.EnergyMeter);
            Assert.AreEqual(session1.TransparencySoftwares,    session2.TransparencySoftwares);
            Assert.AreEqual(session1.Currency,                 session2.Currency);
            Assert.AreEqual(session1.ChargingPeriods,          session2.ChargingPeriods);
            Assert.AreEqual(session1.TotalCosts,               session2.TotalCosts);
            Assert.AreEqual(session1.Status,                   session2.Status);
            Assert.AreEqual(session1.LastUpdated.ToIso8601(),  session2.LastUpdated.ToIso8601());

        }

        #endregion


        #region Session_DeserializeGitHub_Test01()

        /// <summary>
        /// Tries to deserialize a session example from GitHub.
        /// https://github.com/ocpi/ocpi/blob/release-2.2-bugfixes/examples/session_example_1_simple_start.json
        /// </summary>
        [Test]
        public static void Session_DeserializeGitHub_Test01()
        {

            #region Define JSON

            var JSON = @"{
                           ""country_code"":    ""NL"",
                           ""party_id"":        ""STK"",
                           ""id"":              ""101"",
                           ""start_date_time"": ""2020-03-09T10:17:09Z"",
                           ""kwh"":               0.0,
                           ""cdr_token"": {
                               ""uid"":         ""123abc"",
                               ""type"":        ""RFID"",
                               ""contract_id"": ""NL-TST-C12345678-S""
                           },
                           ""auth_method"":     ""WHITELIST"",
                           ""location_id"":     ""LOC1"",
                           ""evse_uid"":        ""3256"",
                           ""connector_id"":    ""1"",
                           ""currency"":        ""EUR"",
                           ""total_cost"": {
                               ""excl_vat"":      2.5
                           },
                           ""status"":          ""PENDING"",
                           ""last_updated"":    ""2020-03-09T10:17:09Z""
                         }";

            #endregion

            Assert.IsTrue(Session.TryParse(JSON, out var parsedSession, out var errorResponse));
            Assert.IsNull(errorResponse);

            Assert.AreEqual(CountryCode.Parse("NL"),                   parsedSession.CountryCode);
            Assert.AreEqual(Party_Id.   Parse("STK"),                  parsedSession.PartyId);
            Assert.AreEqual(Session_Id. Parse("101"),                  parsedSession.Id);
            //Assert.AreEqual(Session1.Start.    ToIso8601(),    parsedSession.Start.    ToIso8601());
            //Assert.AreEqual(Session1.End.Value.ToIso8601(),    parsedSession.End.Value.ToIso8601());
            //Assert.AreEqual(Session1.kWh,                      parsedSession.kWh);
            //Assert.AreEqual(Session1.CDRToken,                 parsedSession.CDRToken);
            //Assert.AreEqual(Session1.AuthMethod,               parsedSession.AuthMethod);
            //Assert.AreEqual(Session1.AuthorizationReference,   parsedSession.AuthorizationReference);
            //Assert.AreEqual(Session1.LocationId,               parsedSession.LocationId);
            //Assert.AreEqual(Session1.EVSEUId,                  parsedSession.EVSEUId);
            //Assert.AreEqual(Session1.ConnectorId,              parsedSession.ConnectorId);
            //Assert.AreEqual(Session1.MeterId,                  parsedSession.MeterId);
            //Assert.AreEqual(Session1.EnergyMeter,              parsedSession.EnergyMeter);
            //Assert.AreEqual(Session1.TransparencySoftwares,    parsedSession.TransparencySoftwares);
            //Assert.AreEqual(Session1.Currency,                 parsedSession.Currency);
            //Assert.AreEqual(Session1.ChargingPeriods,          parsedSession.ChargingPeriods);
            //Assert.AreEqual(Session1.TotalCosts,               parsedSession.TotalCosts);
            //Assert.AreEqual(Session1.Status,                   parsedSession.Status);
            //Assert.AreEqual(Session1.LastUpdated.ToIso8601(),  parsedSession.LastUpdated.ToIso8601());

        }

        #endregion

        #region Session_DeserializeGitHub_Test02()

        /// <summary>
        /// Tries to deserialize a session example from GitHub.
        /// https://github.com/ocpi/ocpi/blob/release-2.2-bugfixes/examples/session_example_2_short_finished.json
        /// </summary>
        [Test]
        public static void Session_DeserializeGitHub_Test02()
        {

            #region Define JSON

            var JSON = @"{
                           ""country_code"":        ""BE"",
                           ""party_id"":            ""BEC"",
                           ""id"":                  ""101"",
                           ""start_date_time"":     ""2015-06-29T22:39:09Z"",
                           ""end_date_time"":       ""2015-06-29T23:50:16Z"",
                           ""kwh"": 41.00,
                            ""cdr_token"": {
                                 ""uid"":           ""123abc"",
                                 ""type"":          ""RFID"",
                                 ""contract_id"":   ""NL-TST-C12345678-S""
                             },
                           ""auth_method"":         ""WHITELIST"",
                           ""location_id"":         ""LOC1"",
                           ""evse_uid"":            ""3256"",
                           ""connector_id"":        ""1"",
                           ""currency"":            ""EUR"",
                           ""charging_periods"": [{
                             ""start_date_time"":   ""2015-06-29T22:39:09Z"",
                             ""dimensions"": [{
                               ""type"":            ""ENERGY"",
                               ""volume"":            120
                             }, {
                               ""type"":            ""MAX_CURRENT"",
                               ""volume"":            30
                             }]
                           }, {
                             ""start_date_time"":   ""2015-06-29T22:40:54Z"",
                             ""dimensions"": [{
                               ""type"":            ""ENERGY"",
                               ""volume"":            41000
                             }, {
                               ""type"":            ""MIN_CURRENT"",
                               ""volume"":            34
                             }]
                           }, {
                             ""start_date_time"":   ""2015-06-29T23:07:09Z"",
                             ""dimensions"": [{
                               ""type"":            ""PARKING_TIME"",
                               ""volume"":            0.718
                             }],
                             ""tariff_id"":         ""12""
                           }],
                           ""total_cost"": {
                             ""excl_vat"":            8.50,
                             ""incl_vat"":            9.35
                           },
                           ""status"":              ""COMPLETED"",
                           ""last_updated"":        ""2015-06-29T23:50:17Z""
                         }";

            #endregion

            Assert.IsTrue(Session.TryParse(JSON, out var parsedSession, out var errorResponse));
            Assert.IsNull(errorResponse);

            Assert.AreEqual(CountryCode.Parse("BE"),                   parsedSession.CountryCode);
            Assert.AreEqual(Party_Id.   Parse("BEC"),                  parsedSession.PartyId);
            Assert.AreEqual(Session_Id. Parse("101"),                  parsedSession.Id);
            //Assert.AreEqual(Session1.Start.    ToIso8601(),    parsedSession.Start.    ToIso8601());
            //Assert.AreEqual(Session1.End.Value.ToIso8601(),    parsedSession.End.Value.ToIso8601());
            //Assert.AreEqual(Session1.kWh,                      parsedSession.kWh);
            //Assert.AreEqual(Session1.CDRToken,                 parsedSession.CDRToken);
            //Assert.AreEqual(Session1.AuthMethod,               parsedSession.AuthMethod);
            //Assert.AreEqual(Session1.AuthorizationReference,   parsedSession.AuthorizationReference);
            //Assert.AreEqual(Session1.LocationId,               parsedSession.LocationId);
            //Assert.AreEqual(Session1.EVSEUId,                  parsedSession.EVSEUId);
            //Assert.AreEqual(Session1.ConnectorId,              parsedSession.ConnectorId);
            //Assert.AreEqual(Session1.MeterId,                  parsedSession.MeterId);
            //Assert.AreEqual(Session1.EnergyMeter,              parsedSession.EnergyMeter);
            //Assert.AreEqual(Session1.TransparencySoftwares,    parsedSession.TransparencySoftwares);
            //Assert.AreEqual(Session1.Currency,                 parsedSession.Currency);
            //Assert.AreEqual(Session1.ChargingPeriods,          parsedSession.ChargingPeriods);
            //Assert.AreEqual(Session1.TotalCosts,               parsedSession.TotalCosts);
            //Assert.AreEqual(Session1.Status,                   parsedSession.Status);
            //Assert.AreEqual(Session1.LastUpdated.ToIso8601(),  parsedSession.LastUpdated.ToIso8601());

        }

        #endregion

    }

}
