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
                               WattHour.FromKWh(1.11M),
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
                                           CDRDimension.ENERGY(
                                               WattHour.FromKWh(1.33M)
                                           )
                                       ],
                                       Tariff_Id.Parse("DE*GEF*T0001")
                                   ),
                                   ChargingPeriod.Create(
                                       DateTime.Parse("2020-04-12T18:21:50Z").ToUniversalTime(),
                                       [
                                           CDRDimension.TIME(
                                               TimeSpan.FromHours(5.12)
                                           )
                                       ],
                                       Tariff_Id.Parse("DE*GEF*T0002")
                                   )
                               ],

                               // Total Costs
                               new Price(
                                   1.12m,
                                   [ TaxAmount.VAT(2.24m) ]
                               ),

                               DateTime.Parse("2020-09-21T00:00:00.000Z").ToUniversalTime()
                           );

            #endregion

            var JSON = session1.ToJSON();

            Assert.That(JSON["country_code"].                    Value<String>(),  Is.EqualTo("DE"));
            Assert.That(JSON["party_id"].                        Value<String>(),  Is.EqualTo("GEF"));
            Assert.That(JSON["id"].                              Value<String>(),  Is.EqualTo("Session0001"));
            Assert.That(JSON["start_date_time"].                 Value<String>(),  Is.EqualTo("2020-08-21T00:00:00.000Z"));
            Assert.That(JSON["end_date_time"].                   Value<String>(),  Is.EqualTo("2020-08-22T00:00:00.000Z"));
            Assert.That(JSON["kwh"].                             Value<Decimal>(), Is.EqualTo(1.11));
            Assert.That(JSON["cdr_token"]["uid"].                Value<String>(),  Is.EqualTo("1234"));
            Assert.That(JSON["cdr_token"]["type"].               Value<String>(),  Is.EqualTo("RFID"));
            Assert.That(JSON["cdr_token"]["contract_id"].        Value<String>(),  Is.EqualTo("Contract0815"));
            Assert.That(JSON["auth_method"].                     Value<String>(),  Is.EqualTo("AUTH_REQUEST"));
            Assert.That(JSON["authorization_reference"].         Value<String>(),  Is.EqualTo("Auth1234"));
            Assert.That(JSON["location_id"].                     Value<String>(),  Is.EqualTo("LOC0001"));
            Assert.That(JSON["evse_uid"].                        Value<String>(),  Is.EqualTo("EVSE0001"));
            Assert.That(JSON["connector_id"].                    Value<String>(),  Is.EqualTo("C1"));
            Assert.That(JSON["meter_id"].                        Value<String>(),  Is.EqualTo("Meter0001"));
            Assert.That(JSON["currency"].                        Value<String>(),  Is.EqualTo("EUR"));
            //ClassicAssert.AreEqual("Stadtwerke Jena-Ost",             JSON["charging_periods"]["xxx"].Value<String>());
            Assert.That(JSON["total_cost"]["before_taxes"].      Value<Decimal>(), Is.EqualTo(1.12));
            Assert.That(JSON["total_cost"]["taxes"][0]["name"].  Value<String>(),  Is.EqualTo("VAT"));
            Assert.That(JSON["total_cost"]["taxes"][0]["amount"].Value<Decimal>(), Is.EqualTo(2.24));
            Assert.That(JSON["status"].                          Value<String>(),  Is.EqualTo("ACTIVE"));
            Assert.That(JSON["last_updated"].                    Value<String>(),  Is.EqualTo("2020-09-21T00:00:00.000Z"));

            Assert.That(Session.TryParse(JSON, out var session2, out var errorResponse), Is.True);
            Assert.That(errorResponse,                                                   Is.Null);

            Assert.That(session2.CountryCode,             Is.EqualTo(session1.CountryCode));
            Assert.That(session2.PartyId,                 Is.EqualTo(session1.PartyId));
            Assert.That(session2.Id,                      Is.EqualTo(session1.Id));
            Assert.That(session2.Start.    ToISO8601(),   Is.EqualTo(session1.Start.    ToISO8601()));
            Assert.That(session2.End.Value.ToISO8601(),   Is.EqualTo(session1.End.Value.ToISO8601()));
            Assert.That(session2.kWh,                     Is.EqualTo(session1.kWh));
            Assert.That(session2.CDRToken,                Is.EqualTo(session1.CDRToken));
            Assert.That(session2.AuthMethod,              Is.EqualTo(session1.AuthMethod));
            Assert.That(session2.AuthorizationReference,  Is.EqualTo(session1.AuthorizationReference));
            Assert.That(session2.LocationId,              Is.EqualTo(session1.LocationId));
            Assert.That(session2.EVSEUId,                 Is.EqualTo(session1.EVSEUId));
            Assert.That(session2.ConnectorId,             Is.EqualTo(session1.ConnectorId));
            Assert.That(session2.EnergyMeterId,           Is.EqualTo(session1.EnergyMeterId));
            Assert.That(session2.Currency,                Is.EqualTo(session1.Currency));
            Assert.That(session2.ChargingPeriods,         Is.EqualTo(session1.ChargingPeriods));
            Assert.That(session2.TotalCosts,              Is.EqualTo(session1.TotalCosts));
            Assert.That(session2.Status,                  Is.EqualTo(session1.Status));
            Assert.That(session2.LastUpdated.ToISO8601(), Is.EqualTo(session1.LastUpdated.ToISO8601()));

        }

        #endregion


        #region Session_DeserializeGitHub_Test01()

        /// <summary>
        /// Tries to deserialize a session example from GitHub.
        /// https://github.com/ocpi/ocpi/blob/release-2.3.0.1-bugfixes/examples/session_example_1_simple_start.json
        /// Its total cost is written as a price of 2.3.0 is - with before_taxes:
        /// the example writes it as 2.2.1 did, with excl_vat, which is refused -
        /// see Session_TotalCostAsBefore_IsRefused.
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
                               ""before_taxes"":  2.5
                           },
                           ""status"":          ""PENDING"",
                           ""last_updated"":    ""2020-03-09T10:17:09Z""
                         }";

            #endregion

            var result = Session.TryParse(JObject.Parse(JSON), out var parsedSession, out var errorResponse);
            Assert.That(result,        Is.True, errorResponse);
            Assert.That(parsedSession, Is.Not.Null);
            Assert.That(errorResponse, Is.Null);

            Assert.That(parsedSession.CountryCode, Is.EqualTo(CountryCode.Parse("NL")));
            Assert.That(parsedSession.PartyId,     Is.EqualTo(Party_Id.   Parse("STK")));
            Assert.That(parsedSession.Id,          Is.EqualTo(Session_Id. Parse("101")));
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
        /// Its total cost is written as a price of 2.3.0 is - with before_taxes
        /// and taxes: the example writes it as 2.2.1 did, with excl_vat and
        /// incl_vat, which is refused - see Session_TotalCostAsBefore_IsRefused.
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
                             ""before_taxes"":        8.50,
                             ""taxes"": [{
                               ""name"":              ""VAT"",
                               ""amount"":            0.85
                             }]
                           },
                           ""status"":              ""COMPLETED"",
                           ""last_updated"":        ""2015-06-29T23:50:17Z""
                         }";

            #endregion

            var result = Session.TryParse(JObject.Parse(JSON), out var parsedSession, out var errorResponse);
            Assert.That(result,        Is.True, errorResponse);
            Assert.That(parsedSession, Is.Not.Null);
            Assert.That(errorResponse, Is.Null);

            Assert.That(parsedSession?.TotalCosts?.BeforeTaxes, Is.EqualTo(8.50M));

            Assert.That(parsedSession.CountryCode, Is.EqualTo(CountryCode.Parse("BE")));
            Assert.That(parsedSession.PartyId,     Is.EqualTo(Party_Id.   Parse("BEC")));
            Assert.That(parsedSession.Id,          Is.EqualTo(Session_Id. Parse("101")));
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

        #region Session_TotalCostAsBefore_IsRefused()

        /// <summary>
        /// A session whose total cost is written as 2.2.1 wrote a price - with
        /// excl_vat and incl_vat, as the examples of GitHub still do - is
        /// refused, and said why: a price of 2.3.0 has before_taxes and taxes.
        /// It was read before, its total cost passed over without a word.
        /// </summary>
        [Test]
        public static void Session_TotalCostAsBefore_IsRefused()
        {

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
                               ""excl_vat"":      8.50,
                               ""incl_vat"":      9.35
                           },
                           ""status"":          ""PENDING"",
                           ""last_updated"":    ""2020-03-09T10:17:09Z""
                         }";

            var result = Session.TryParse(JObject.Parse(JSON), out var parsedSession, out var errorResponse);

            Assert.That(result,              Is.False, "A session with a total cost as before is read.");
            Assert.That(parsedSession,       Is.Null);
            Assert.That(errorResponse ?? "", Does.Contain("total_cost"), "Why the session is refused is not said.");

        }

        #endregion

    }

}
