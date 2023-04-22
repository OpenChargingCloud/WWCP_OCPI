/*
 * Copyright (c) 2015-2023 GraphDefined GmbH
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
using org.GraphDefined.Vanaheimr.Aegir;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_1_1.UnitTests.Datastructures
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

            #region Define session1

            var session1 = new Session(
                               CountryCode.Parse("DE"),   // Note: The country code is just internal!
                               Party_Id.   Parse("GEF"),  // Note: The party identification is just internal!
                               Session_Id. Parse("Session0001"),
                               DateTime.Parse("2020-08-21T00:00:00.000Z").ToUniversalTime(), // Start
                               1.11M, // KWh
                               Auth_Id.   Parse("1234"),
                               AuthMethods.AUTH_REQUEST,

                               new Location(
                                   CountryCode. Parse("DE"),
                                   Party_Id.    Parse("GEF"),
                                   Location_Id. Parse("LOC0001"),
                                   LocationType.UNDERGROUND_GARAGE,
                                   "Biberweg 18",
                                   "Jena",
                                   "07749",
                                   Country.Germany,
                                   GeoCoordinate.Parse(10, 20)
                               ),

                               Currency.EUR,
                               SessionStatusTypes.ACTIVE,
                               DateTime.Parse("2020-08-22T00:00:00.000Z").ToUniversalTime(), // End
                               Meter_Id.Parse("Meter0001"),

                               new ChargingPeriod[] {
                                   new ChargingPeriod(
                                       DateTime.Parse("2020-04-12T18:21:49Z").ToUniversalTime(),
                                       new CDRDimension[] {
                                           new CDRDimension(
                                               CDRDimensionType.ENERGY,
                                               1.33M
                                           )
                                       }
                                   ),
                                   new ChargingPeriod(
                                       DateTime.Parse("2020-04-12T18:21:50Z").ToUniversalTime(),
                                       new CDRDimension[] {
                                           new CDRDimension(
                                               CDRDimensionType.TIME,
                                               5.12M
                                           )
                                       }
                                   )
                               },

                               // Total Costs
                               1.12M,

                               DateTime.Parse("2020-09-21T00:00:00.000Z").ToUniversalTime()

                           );

            #endregion

            var json = session1.ToJSON();

            Assert.AreEqual("DE",                              json["country_code"].                    Value<String>());
            Assert.AreEqual("GEF",                             json["party_id"].                        Value<String>());
            Assert.AreEqual("Session0001",                     json["id"].                              Value<String>());
            Assert.AreEqual("2020-08-21T00:00:00.000Z",        json["start_date_time"].                 Value<String>());
            Assert.AreEqual("2020-08-22T00:00:00.000Z",        json["end_date_time"].                   Value<String>());
            Assert.AreEqual(1.11,                              json["kwh"].                             Value<Decimal>());
            Assert.AreEqual("1234",                            json["cdr_token"]["uid"].                Value<String>());
            Assert.AreEqual("RFID",                            json["cdr_token"]["type"].               Value<String>());
            Assert.AreEqual("Contract0815",                    json["cdr_token"]["contract_id"].        Value<String>());
            Assert.AreEqual("AUTH_REQUEST",                    json["auth_method"].                     Value<String>());
            Assert.AreEqual("Auth1234",                        json["authorization_reference"].         Value<String>());
            Assert.AreEqual("LOC0001",                         json["location_id"].                     Value<String>());
            Assert.AreEqual("EVSE0001",                        json["evse_uid"].                        Value<String>());
            Assert.AreEqual("C1",                              json["connector_id"].                    Value<String>());
            Assert.AreEqual("Meter0001",                       json["meter_id"].                        Value<String>());
            Assert.AreEqual("EUR",                             json["currency"].                        Value<String>());
            //Assert.AreEqual("Stadtwerke Jena-Ost",             JSON["charging_periods"]["xxx"].Value<String>());
            Assert.AreEqual(1.12,                              json["total_cost"]["excl_vat"].          Value<Decimal>());
            Assert.AreEqual(2.24,                              json["total_cost"]["incl_vat"].          Value<Decimal>());
            Assert.AreEqual("ACTIVE",                          json["status"].                          Value<String>());
            Assert.AreEqual("2020-09-21T00:00:00.000Z",        json["last_updated"].                    Value<String>());

            Assert.IsTrue(Session.TryParse(json, out var session2, out var errorResponse));
            Assert.IsNull(errorResponse);

            Assert.AreEqual(session1.CountryCode,              session2.CountryCode);
            Assert.AreEqual(session1.PartyId,                  session2.PartyId);
            Assert.AreEqual(session1.Id,                       session2.Id);
            Assert.AreEqual(session1.Start.    ToIso8601(),    session2.Start.    ToIso8601());
            Assert.AreEqual(session1.End.Value.ToIso8601(),    session2.End.Value.ToIso8601());
            Assert.AreEqual(session1.kWh,                      session2.kWh);
            Assert.AreEqual(session1.AuthId,                   session2.AuthId);
            Assert.AreEqual(session1.AuthMethod,               session2.AuthMethod);
            Assert.AreEqual(session1.Location,                 session2.Location);
            Assert.AreEqual(session1.MeterId,                  session2.MeterId);
            Assert.AreEqual(session1.Currency,                 session2.Currency);
            Assert.AreEqual(session1.ChargingPeriods,          session2.ChargingPeriods);
            Assert.AreEqual(session1.TotalCost,                session2.TotalCost);
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

            Assert.IsTrue(Session.TryParse(JObject.Parse(JSON), out var parsedSession, out var errorResponse));
            Assert.IsNull(errorResponse);

            Assert.AreEqual(Session_Id. Parse("101"),                  parsedSession.Id);
            //Assert.AreEqual(session1.Start.    ToIso8601(),    parsedSession.Start.    ToIso8601());
            //Assert.AreEqual(session1.End.Value.ToIso8601(),    parsedSession.End.Value.ToIso8601());
            //Assert.AreEqual(session1.kWh,                      parsedSession.kWh);
            //Assert.AreEqual(session1.CDRToken,                 parsedSession.CDRToken);
            //Assert.AreEqual(session1.AuthMethod,               parsedSession.AuthMethod);
            //Assert.AreEqual(session1.AuthorizationReference,   parsedSession.AuthorizationReference);
            //Assert.AreEqual(session1.LocationId,               parsedSession.LocationId);
            //Assert.AreEqual(session1.EVSEUId,                  parsedSession.EVSEUId);
            //Assert.AreEqual(session1.ConnectorId,              parsedSession.ConnectorId);
            //Assert.AreEqual(session1.MeterId,                  parsedSession.MeterId);
            //Assert.AreEqual(session1.EnergyMeter,              parsedSession.EnergyMeter);
            //Assert.AreEqual(session1.TransparencySoftwares,    parsedSession.TransparencySoftwares);
            //Assert.AreEqual(session1.Currency,                 parsedSession.Currency);
            //Assert.AreEqual(session1.ChargingPeriods,          parsedSession.ChargingPeriods);
            //Assert.AreEqual(session1.TotalCosts,               parsedSession.TotalCosts);
            //Assert.AreEqual(session1.Status,                   parsedSession.Status);
            //Assert.AreEqual(session1.LastUpdated.ToIso8601(),  parsedSession.LastUpdated.ToIso8601());

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

            Assert.IsTrue(Session.TryParse(JObject.Parse(JSON), out var parsedSession, out var errorResponse));
            Assert.IsNull(errorResponse);

            Assert.AreEqual(Session_Id. Parse("101"),                  parsedSession.Id);
            //Assert.AreEqual(session1.Start.    ToIso8601(),    parsedSession.Start.    ToIso8601());
            //Assert.AreEqual(session1.End.Value.ToIso8601(),    parsedSession.End.Value.ToIso8601());
            //Assert.AreEqual(session1.kWh,                      parsedSession.kWh);
            //Assert.AreEqual(session1.CDRToken,                 parsedSession.CDRToken);
            //Assert.AreEqual(session1.AuthMethod,               parsedSession.AuthMethod);
            //Assert.AreEqual(session1.AuthorizationReference,   parsedSession.AuthorizationReference);
            //Assert.AreEqual(session1.LocationId,               parsedSession.LocationId);
            //Assert.AreEqual(session1.EVSEUId,                  parsedSession.EVSEUId);
            //Assert.AreEqual(session1.ConnectorId,              parsedSession.ConnectorId);
            //Assert.AreEqual(session1.MeterId,                  parsedSession.MeterId);
            //Assert.AreEqual(session1.EnergyMeter,              parsedSession.EnergyMeter);
            //Assert.AreEqual(session1.TransparencySoftwares,    parsedSession.TransparencySoftwares);
            //Assert.AreEqual(session1.Currency,                 parsedSession.Currency);
            //Assert.AreEqual(session1.ChargingPeriods,          parsedSession.ChargingPeriods);
            //Assert.AreEqual(session1.TotalCosts,               parsedSession.TotalCosts);
            //Assert.AreEqual(session1.Status,                   parsedSession.Status);
            //Assert.AreEqual(session1.LastUpdated.ToIso8601(),  parsedSession.LastUpdated.ToIso8601());

        }

        #endregion

    }

}
