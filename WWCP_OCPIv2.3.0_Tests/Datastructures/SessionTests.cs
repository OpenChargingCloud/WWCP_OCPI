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
using NUnit.Framework.Legacy;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_3_0.UnitTests.Datastructures
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
                               WattHour.ParseKWh(1.11M),
                               new CDRToken(
                                   CountryCode.Parse("DE"),
                                   Party_Id.   Parse("GEF"),
                                   Token_Id.   Parse("1234"),
                                   TokenType.RFID,
                                   Contract_Id.Parse("Contract0815")
                               ),
                               AuthMethod.AUTH_REQUEST,
                               Location_Id. Parse("LOC0001"),
                               EVSE_UId.    Parse("EVSE0001"),
                               Connector_Id.Parse("C1"),
                               Currency.EUR,
                               SessionStatusType.ACTIVE,
                               DateTime.Parse("2020-08-22T00:00:00.000Z").ToUniversalTime(), // End
                               AuthorizationReference.Parse("Auth1234"),
                               EnergyMeter_Id.Parse("Meter0001"),
                               [
                                   ChargingPeriod.Create(
                                       DateTime.Parse("2020-04-12T18:21:49Z").ToUniversalTime(),
                                       [
                                           CDRDimension.Create(
                                               CDRDimensionType.ENERGY,
                                               1.33M
                                           )
                                       ],
                                       Tariff_Id.Parse("DE*GEF*T0001")
                                   ),
                                   ChargingPeriod.Create(
                                       DateTime.Parse("2020-04-12T18:21:50Z").ToUniversalTime(),
                                       [
                                           CDRDimension.Create(
                                               CDRDimensionType.TIME,
                                               5.12M
                                           )
                                       ],
                                       Tariff_Id.Parse("DE*GEF*T0002")
                                   )
                               ],

                               // Total Costs
                               new Price(
                                   1.12m,
                                   [new TaxAmount("VAT", 2.24m)]
                               ),

                               DateTime.Parse("2020-09-21T00:00:00.000Z").ToUniversalTime()
                           );

            #endregion

            var JSON = session1.ToJSON();

            ClassicAssert.AreEqual("DE",                              JSON["country_code"].                    Value<String>());
            ClassicAssert.AreEqual("GEF",                             JSON["party_id"].                        Value<String>());
            ClassicAssert.AreEqual("Session0001",                     JSON["id"].                              Value<String>());
            ClassicAssert.AreEqual("2020-08-21T00:00:00.000Z",        JSON["start_date_time"].                 Value<String>());
            ClassicAssert.AreEqual("2020-08-22T00:00:00.000Z",        JSON["end_date_time"].                   Value<String>());
            ClassicAssert.AreEqual(1.11,                              JSON["kwh"].                             Value<Decimal>());
            ClassicAssert.AreEqual("1234",                            JSON["cdr_token"]["uid"].                Value<String>());
            ClassicAssert.AreEqual("RFID",                            JSON["cdr_token"]["type"].               Value<String>());
            ClassicAssert.AreEqual("Contract0815",                    JSON["cdr_token"]["contract_id"].        Value<String>());
            ClassicAssert.AreEqual("AUTH_REQUEST",                    JSON["auth_method"].                     Value<String>());
            ClassicAssert.AreEqual("Auth1234",                        JSON["authorization_reference"].         Value<String>());
            ClassicAssert.AreEqual("LOC0001",                         JSON["location_id"].                     Value<String>());
            ClassicAssert.AreEqual("EVSE0001",                        JSON["evse_uid"].                        Value<String>());
            ClassicAssert.AreEqual("C1",                              JSON["connector_id"].                    Value<String>());
            ClassicAssert.AreEqual("Meter0001",                       JSON["meter_id"].                        Value<String>());
            ClassicAssert.AreEqual("EUR",                             JSON["currency"].                        Value<String>());
            //ClassicAssert.AreEqual("Stadtwerke Jena-Ost",             JSON["charging_periods"]["xxx"].Value<String>());
            ClassicAssert.AreEqual(1.12,                              JSON["total_cost"]["excl_vat"].          Value<Decimal>());
            ClassicAssert.AreEqual(2.24,                              JSON["total_cost"]["incl_vat"].          Value<Decimal>());
            ClassicAssert.AreEqual("ACTIVE",                          JSON["status"].                          Value<String>());
            ClassicAssert.AreEqual("2020-09-21T00:00:00.000Z",        JSON["last_updated"].                    Value<String>());

            ClassicAssert.IsTrue(Session.TryParse(JSON, out var session2, out var errorResponse));
            ClassicAssert.IsNull(errorResponse);

            ClassicAssert.AreEqual(session1.CountryCode,              session2.CountryCode);
            ClassicAssert.AreEqual(session1.PartyId,                  session2.PartyId);
            ClassicAssert.AreEqual(session1.Id,                       session2.Id);
            ClassicAssert.AreEqual(session1.Start.    ToISO8601(),    session2.Start.    ToISO8601());
            ClassicAssert.AreEqual(session1.End.Value.ToISO8601(),    session2.End.Value.ToISO8601());
            ClassicAssert.AreEqual(session1.kWh,                      session2.kWh);
            ClassicAssert.AreEqual(session1.CDRToken,                 session2.CDRToken);
            ClassicAssert.AreEqual(session1.AuthMethod,               session2.AuthMethod);
            ClassicAssert.AreEqual(session1.AuthorizationReference,   session2.AuthorizationReference);
            ClassicAssert.AreEqual(session1.LocationId,               session2.LocationId);
            ClassicAssert.AreEqual(session1.EVSEUId,                  session2.EVSEUId);
            ClassicAssert.AreEqual(session1.ConnectorId,              session2.ConnectorId);
            ClassicAssert.AreEqual(session1.EnergyMeterId,                  session2.EnergyMeterId);
            ClassicAssert.AreEqual(session1.Currency,                 session2.Currency);
            ClassicAssert.AreEqual(session1.ChargingPeriods,          session2.ChargingPeriods);
            ClassicAssert.AreEqual(session1.TotalCosts,               session2.TotalCosts);
            ClassicAssert.AreEqual(session1.Status,                   session2.Status);
            ClassicAssert.AreEqual(session1.LastUpdated.ToISO8601(),  session2.LastUpdated.ToISO8601());

        }

        #endregion


        #region Session_DeserializeGitHub_Test01()

        /// <summary>
        /// Tries to deserialize a session example from GitHub.
        /// https://github.com/ocpi/ocpi/blob/release-2.3.0.1-bugfixes/examples/session_example_1_simple_start.json
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
                               ""country_code"":  ""NL"",
                               ""party_id"":      ""TST"",
                               ""uid"":           ""123abc"",
                               ""type"":          ""RFID"",
                               ""contract_id"":   ""NL-TST-C12345678-S""
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

            var result = Session.TryParse(JObject.Parse(JSON), out var parsedSession, out var errorResponse);
            ClassicAssert.IsTrue   (result, errorResponse);
            ClassicAssert.IsNotNull(parsedSession);
            ClassicAssert.IsNull   (errorResponse);

            ClassicAssert.AreEqual(CountryCode.Parse("NL"),                   parsedSession.CountryCode);
            ClassicAssert.AreEqual(Party_Id.   Parse("STK"),                  parsedSession.PartyId);
            ClassicAssert.AreEqual(Session_Id. Parse("101"),                  parsedSession.Id);
            //ClassicAssert.AreEqual(Session1.Start.    ToISO8601(),    parsedSession.Start.    ToISO8601());
            //ClassicAssert.AreEqual(Session1.End.Value.ToISO8601(),    parsedSession.End.Value.ToISO8601());
            //ClassicAssert.AreEqual(Session1.kWh,                      parsedSession.kWh);
            //ClassicAssert.AreEqual(Session1.CDRToken,                 parsedSession.CDRToken);
            //ClassicAssert.AreEqual(Session1.AuthMethod,               parsedSession.AuthMethod);
            //ClassicAssert.AreEqual(Session1.AuthorizationReference,   parsedSession.AuthorizationReference);
            //ClassicAssert.AreEqual(Session1.LocationId,               parsedSession.LocationId);
            //ClassicAssert.AreEqual(Session1.EVSEUId,                  parsedSession.EVSEUId);
            //ClassicAssert.AreEqual(Session1.ConnectorId,              parsedSession.ConnectorId);
            //ClassicAssert.AreEqual(Session1.MeterId,                  parsedSession.MeterId);
            //ClassicAssert.AreEqual(Session1.EnergyMeter,              parsedSession.EnergyMeter);
            //ClassicAssert.AreEqual(Session1.TransparencySoftware,    parsedSession.TransparencySoftware);
            //ClassicAssert.AreEqual(Session1.Currency,                 parsedSession.Currency);
            //ClassicAssert.AreEqual(Session1.ChargingPeriods,          parsedSession.ChargingPeriods);
            //ClassicAssert.AreEqual(Session1.TotalCosts,               parsedSession.TotalCosts);
            //ClassicAssert.AreEqual(Session1.Status,                   parsedSession.Status);
            //ClassicAssert.AreEqual(Session1.LastUpdated.ToISO8601(),  parsedSession.LastUpdated.ToISO8601());

        }

        #endregion

        #region Session_DeserializeGitHub_Test02()

        /// <summary>
        /// Tries to deserialize a session example from GitHub.
        /// https://github.com/ocpi/ocpi/blob/release-2.3.0.1-bugfixes/examples/session_example_2_short_finished.json
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
                                 ""country_code"":  ""NL"",
                                 ""party_id"":      ""TST"",
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

            var result = Session.TryParse(JObject.Parse(JSON), out var parsedSession, out var errorResponse);
            ClassicAssert.IsTrue   (result, errorResponse);
            ClassicAssert.IsNotNull(parsedSession);
            ClassicAssert.IsNull   (errorResponse);

            ClassicAssert.AreEqual(CountryCode.Parse("BE"),                   parsedSession.CountryCode);
            ClassicAssert.AreEqual(Party_Id.   Parse("BEC"),                  parsedSession.PartyId);
            ClassicAssert.AreEqual(Session_Id. Parse("101"),                  parsedSession.Id);
            //ClassicAssert.AreEqual(Session1.Start.    ToISO8601(),    parsedSession.Start.    ToISO8601());
            //ClassicAssert.AreEqual(Session1.End.Value.ToISO8601(),    parsedSession.End.Value.ToISO8601());
            //ClassicAssert.AreEqual(Session1.kWh,                      parsedSession.kWh);
            //ClassicAssert.AreEqual(Session1.CDRToken,                 parsedSession.CDRToken);
            //ClassicAssert.AreEqual(Session1.AuthMethod,               parsedSession.AuthMethod);
            //ClassicAssert.AreEqual(Session1.AuthorizationReference,   parsedSession.AuthorizationReference);
            //ClassicAssert.AreEqual(Session1.LocationId,               parsedSession.LocationId);
            //ClassicAssert.AreEqual(Session1.EVSEUId,                  parsedSession.EVSEUId);
            //ClassicAssert.AreEqual(Session1.ConnectorId,              parsedSession.ConnectorId);
            //ClassicAssert.AreEqual(Session1.MeterId,                  parsedSession.MeterId);
            //ClassicAssert.AreEqual(Session1.EnergyMeter,              parsedSession.EnergyMeter);
            //ClassicAssert.AreEqual(Session1.TransparencySoftware,    parsedSession.TransparencySoftware);
            //ClassicAssert.AreEqual(Session1.Currency,                 parsedSession.Currency);
            //ClassicAssert.AreEqual(Session1.ChargingPeriods,          parsedSession.ChargingPeriods);
            //ClassicAssert.AreEqual(Session1.TotalCosts,               parsedSession.TotalCosts);
            //ClassicAssert.AreEqual(Session1.Status,                   parsedSession.Status);
            //ClassicAssert.AreEqual(Session1.LastUpdated.ToISO8601(),  parsedSession.LastUpdated.ToISO8601());

        }

        #endregion

    }

}
