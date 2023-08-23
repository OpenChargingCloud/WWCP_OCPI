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

using org.GraphDefined.Vanaheimr.Aegir;
using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_1_1.UnitTests.Datastructures
{

    /// <summary>
    /// Charging session tests.
    /// https://github.com/ocpi/ocpi/blob/release-2.1.1-bugfixes/mod_sessions.md
    /// </summary>
    [TestFixture]
    public static class SessionTests
    {

        #region Session_DeserializeGitHub_Test01()

        /// <summary>
        /// Tries to deserialize a session example from GitHub.
        /// https://github.com/ocpi/ocpi/blob/release-2.1.1-bugfixes/mod_sessions.md
        /// </summary>
        [Test]
        public static void Session_DeserializeGitHub_Test01()
        {

            #region Define JSON

            var sessionJSON = @"{
                                   ""id"":             ""101"",
                                   ""start_datetime"": ""2015-06-29T22:39:09Z"",
                                   ""kwh"":              0.00,
                                   ""auth_id"":        ""DE8ACC12E46L89"",
                                   ""auth_method"":    ""WHITELIST"",
                                   ""location"": {
                                       ""id"":             ""LOC1"",
                                       ""type"":           ""ON_STREET"",
                                       ""name"":           ""Gent Zuid"",
                                       ""address"":        ""F.Rooseveltlaan 3A"",
                                       ""city"":           ""Gent"",
                                       ""postal_code"":    ""9000"",
                                       ""country"":        ""BE"",
                                       ""coordinates"": {
                                           ""latitude"":       ""3.729944"",
                                           ""longitude"":      ""51.047599""
                                       },
                                       ""evses"": [{
                                           ""uid"":            ""3256"",
                                           ""evse_id"":        ""BE-BEC-E041503003"",
                                           ""status"":         ""AVAILABLE"",
                                           ""connectors"": [{
                                               ""id"":             ""1"",
                                               ""standard"":       ""IEC_62196_T2"",
                                               ""format"":         ""SOCKET"",
                                               ""power_type"":     ""AC_1_PHASE"",
                                               ""voltage"":          230,
                                               ""amperage"":         64,
                                               ""tariff_id"":      ""11"",
                                               ""last_updated"":   ""2015-06-29T22:39:09Z""
                                           }],
                                           ""last_updated"":   ""2015-06-29T22:39:09Z""
                                       }],
                                       ""last_updated"":   ""2015-06-29T22:39:09Z""
                                   },
                                   ""currency"":       ""EUR"",
                                   ""total_cost"":       2.50,
                                   ""status"":         ""PENDING"",
                                   ""last_updated"":   ""2015-06-29T22:39:09Z""
                                }";

            #endregion

            var result = Session.TryParse(JObject.Parse(sessionJSON),
                                          out var parsedSession,
                                          out var errorResponse,
                                          CountryCode.Parse("NL"),
                                          Party_Id.   Parse("STK"));

            Assert.IsTrue   (result, errorResponse);
            Assert.IsNotNull(parsedSession);
            Assert.IsNull   (errorResponse);

            if (parsedSession is not null)
            {

                Assert.AreEqual (Session_Id.Parse("101"),              parsedSession.Id);
                Assert.AreEqual ("2015-06-29T22:39:09.000Z",           parsedSession.Start.ToIso8601());
                Assert.AreEqual (0.0M,                                 parsedSession.kWh);
                Assert.AreEqual (Auth_Id.Parse("DE8ACC12E46L89"),      parsedSession.AuthId);
                Assert.AreEqual (AuthMethods.WHITELIST,                parsedSession.AuthMethod);
                Assert.AreEqual (OCPI.Currency.EUR,                    parsedSession.Currency);
                Assert.AreEqual (2.50M,                                parsedSession.TotalCost);
                Assert.AreEqual (SessionStatusTypes.PENDING,           parsedSession.Status);
                Assert.AreEqual ("2015-06-29T22:39:09.000Z",           parsedSession.LastUpdated.ToIso8601());

                Assert.IsNotNull(                                      parsedSession.Location);
                Assert.AreEqual (Location_Id.Parse("LOC1"),            parsedSession.Location.Id);
                Assert.AreEqual (LocationType.ON_STREET,               parsedSession.Location.LocationType);
                Assert.AreEqual ("Gent Zuid",                          parsedSession.Location.Name);
                Assert.AreEqual ("F.Rooseveltlaan 3A",                 parsedSession.Location.Address);
                Assert.AreEqual ("Gent",                               parsedSession.Location.City);
                Assert.AreEqual ("9000",                               parsedSession.Location.PostalCode);
                Assert.AreEqual (Country.Belgium,                      parsedSession.Location.Country);
                Assert.AreEqual (3.729944,                             parsedSession.Location.Coordinates.Latitude. Value);
                Assert.AreEqual (51.047599,                            parsedSession.Location.Coordinates.Longitude.Value);
                Assert.AreEqual ("2015-06-29T22:39:09.000Z",           parsedSession.Location.LastUpdated.ToIso8601());

                Assert.IsNotNull(                                      parsedSession.Location.EVSEs);
                Assert.AreEqual (EVSE_UId.Parse("3256"),               parsedSession.Location.EVSEs.First().UId);
                Assert.AreEqual (EVSE_Id.Parse("BE-BEC-E041503003"),   parsedSession.Location.EVSEs.First().EVSEId);
                Assert.AreEqual (StatusType.AVAILABLE,                 parsedSession.Location.EVSEs.First().Status);
                Assert.AreEqual ("2015-06-29T22:39:09.000Z",           parsedSession.Location.EVSEs.First().LastUpdated.ToIso8601());

                Assert.IsNotNull(                                      parsedSession.Location.EVSEs.First().Connectors);
                Assert.AreEqual (Connector_Id.    Parse("1"),          parsedSession.Location.EVSEs.First().Connectors.First().Id);
                Assert.AreEqual (ConnectorType.   IEC_62196_T2,        parsedSession.Location.EVSEs.First().Connectors.First().Standard);
                Assert.AreEqual (ConnectorFormats.SOCKET,              parsedSession.Location.EVSEs.First().Connectors.First().Format);
                Assert.AreEqual (PowerTypes.      AC_1_PHASE,          parsedSession.Location.EVSEs.First().Connectors.First().PowerType);
                Assert.AreEqual (230,                                  parsedSession.Location.EVSEs.First().Connectors.First().Voltage);
                Assert.AreEqual (64,                                   parsedSession.Location.EVSEs.First().Connectors.First().Amperage);
                Assert.AreEqual (Tariff_Id.       Parse("11"),         parsedSession.Location.EVSEs.First().Connectors.First().TariffId);
                Assert.AreEqual ("2015-06-29T22:39:09.000Z",           parsedSession.Location.EVSEs.First().Connectors.First().LastUpdated.ToIso8601());

            }

        }

        #endregion

        #region Session_DeserializeGitHub_Test02()

        /// <summary>
        /// Tries to deserialize a session example from GitHub.
        /// https://github.com/ocpi/ocpi/blob/release-2.1.1-bugfixes/mod_sessions.md#simple-session-example-of-a-short-finished-session
        /// </summary>
        [Test]
        public static void Session_DeserializeGitHub_Test02()
        {

            #region Define JSON

            var sessionJSON = @"{
                                   ""id"":             ""101"",
                                   ""start_datetime"": ""2015-06-29T22:39:09Z"",
                                   ""end_datetime"":   ""2015-06-29T23:50:16Z"",
                                   ""kwh"":              41.00,
                                   ""auth_id"":        ""DE8ACC12E46L89"",
                                   ""auth_method"":    ""WHITELIST"",
                                   ""location"": {
                                       ""id"":             ""LOC1"",
                                       ""type"":           ""ON_STREET"",
                                       ""name"":           ""Gent Zuid"",
                                       ""address"":        ""F.Rooseveltlaan 3A"",
                                       ""city"":           ""Gent"",
                                       ""postal_code"":    ""9000"",
                                       ""country"":        ""BE"",
                                       ""coordinates"": {
                                           ""latitude"":       ""3.729944"",
                                           ""longitude"":      ""51.047599""
                                       },
                                       ""evses"": [{
                                           ""uid"":            ""3256"",
                                           ""evse_id"":        ""BE-BEC-E041503003"",
                                           ""status"":         ""AVAILABLE"",
                                           ""connectors"": [{
                                               ""id"":             ""1"",
                                               ""standard"":       ""IEC_62196_T2"",
                                               ""format"":         ""SOCKET"",
                                               ""power_type"":     ""AC_1_PHASE"",
                                               ""voltage"":          230,
                                               ""amperage"":         64,
                                               ""tariff_id"":      ""11"",
                                               ""last_updated"":   ""2015-06-29T23:09:10Z""
                                           }],
                                           ""last_updated"":   ""2015-06-29T23:09:10Z""
                                       }],
                                       ""last_updated"":   ""2015-06-29T23:09:10Z""
                                   },
                                   ""currency"":       ""EUR"",
                                   ""charging_periods"": [{
                                       ""start_date_time"":    ""2015-06-29T22:39:09Z"",
                                       ""dimensions"": [{
                                           ""type"":               ""ENERGY"",
                                           ""volume"":               120
                                       }, {
                                           ""type"":               ""MAX_CURRENT"",
                                           ""volume"":               30
                                       }]
                                   }, {
                                       ""start_date_time"":    ""2015-06-29T22:40:54Z"",
                                       ""dimensions"": [{
                                           ""type"":               ""ENERGY"",
                                           ""volume"":               41000
                                       }, {
                                           ""type"":               ""MIN_CURRENT"",
                                           ""volume"":               34
                                       }]
                                   }, {
                                       ""start_date_time"":    ""2015-06-29T23:07:09Z"",
                                       ""dimensions"": [{
                                           ""type"":               ""PARKING_TIME"",
                                           ""volume"":               0.718
                                       }]
                                   }],
                                   ""total_cost"":       8.50,
                                   ""status"":         ""COMPLETED"",
                                   ""last_updated"":   ""2015-06-29T23:09:10Z""
                               }";

            #endregion

            var result = Session.TryParse(JObject.Parse(sessionJSON),
                                          out var parsedSession,
                                          out var errorResponse,
                                          CountryCode.Parse("NL"),
                                          Party_Id.   Parse("STK"));

            Assert.IsTrue   (result, errorResponse);
            Assert.IsNotNull(parsedSession);
            Assert.IsNull   (errorResponse);

            if (parsedSession is not null)
            {

                Assert.AreEqual (Session_Id.Parse("101"),              parsedSession.Id);
                Assert.AreEqual ("2015-06-29T22:39:09.000Z",           parsedSession.Start.ToIso8601());
                Assert.AreEqual ("2015-06-29T23:50:16.000Z",           parsedSession.End?. ToIso8601());
                Assert.AreEqual (41.0M,                                parsedSession.kWh);
                Assert.AreEqual (Auth_Id.Parse("DE8ACC12E46L89"),      parsedSession.AuthId);
                Assert.AreEqual (AuthMethods.WHITELIST,                parsedSession.AuthMethod);
                Assert.AreEqual (OCPI.Currency.EUR,                    parsedSession.Currency);
                Assert.AreEqual (8.50M,                                parsedSession.TotalCost);
                Assert.AreEqual (SessionStatusTypes.COMPLETED,         parsedSession.Status);
                Assert.AreEqual ("2015-06-29T23:09:10.000Z",           parsedSession.LastUpdated.ToIso8601());

                Assert.IsNotNull(                                      parsedSession.ChargingPeriods);
                Assert.AreEqual ("2015-06-29T22:39:09.000Z",           parsedSession.ChargingPeriods.ElementAt(0).StartTimestamp.ToIso8601());
                Assert.AreEqual (CDRDimensionType.ENERGY,              parsedSession.ChargingPeriods.ElementAt(0).Dimensions.ElementAt(0).Type);
                Assert.AreEqual (120M,                                 parsedSession.ChargingPeriods.ElementAt(0).Dimensions.ElementAt(0).Volume);
                Assert.AreEqual (CDRDimensionType.MAX_CURRENT,         parsedSession.ChargingPeriods.ElementAt(0).Dimensions.ElementAt(1).Type);
                Assert.AreEqual (30M,                                  parsedSession.ChargingPeriods.ElementAt(0).Dimensions.ElementAt(1).Volume);

                Assert.AreEqual ("2015-06-29T22:40:54.000Z",           parsedSession.ChargingPeriods.ElementAt(1).StartTimestamp.ToIso8601());
                Assert.AreEqual (CDRDimensionType.ENERGY,              parsedSession.ChargingPeriods.ElementAt(1).Dimensions.ElementAt(0).Type);
                Assert.AreEqual (41000M,                               parsedSession.ChargingPeriods.ElementAt(1).Dimensions.ElementAt(0).Volume);
                Assert.AreEqual (CDRDimensionType.MIN_CURRENT,         parsedSession.ChargingPeriods.ElementAt(1).Dimensions.ElementAt(1).Type);
                Assert.AreEqual (34M,                                  parsedSession.ChargingPeriods.ElementAt(1).Dimensions.ElementAt(1).Volume);

                Assert.AreEqual ("2015-06-29T23:07:09.000Z",           parsedSession.ChargingPeriods.ElementAt(2).StartTimestamp.ToIso8601());
                Assert.AreEqual (CDRDimensionType.PARKING_TIME,        parsedSession.ChargingPeriods.ElementAt(2).Dimensions.ElementAt(0).Type);
                Assert.AreEqual (0.718M,                               parsedSession.ChargingPeriods.ElementAt(2).Dimensions.ElementAt(0).Volume);

                Assert.IsNotNull(                                      parsedSession.Location);
                Assert.AreEqual (Location_Id.Parse("LOC1"),            parsedSession.Location.Id);
                Assert.AreEqual (LocationType.ON_STREET,               parsedSession.Location.LocationType);
                Assert.AreEqual ("Gent Zuid",                          parsedSession.Location.Name);
                Assert.AreEqual ("F.Rooseveltlaan 3A",                 parsedSession.Location.Address);
                Assert.AreEqual ("Gent",                               parsedSession.Location.City);
                Assert.AreEqual ("9000",                               parsedSession.Location.PostalCode);
                Assert.AreEqual (Country.Belgium,                      parsedSession.Location.Country);
                Assert.AreEqual (3.729944,                             parsedSession.Location.Coordinates.Latitude. Value);
                Assert.AreEqual (51.047599,                            parsedSession.Location.Coordinates.Longitude.Value);
                Assert.AreEqual ("2015-06-29T23:09:10.000Z",           parsedSession.Location.LastUpdated.ToIso8601());

                Assert.IsNotNull(                                      parsedSession.Location.EVSEs);
                Assert.AreEqual (EVSE_UId.Parse("3256"),               parsedSession.Location.EVSEs.First().UId);
                Assert.AreEqual (EVSE_Id.Parse("BE-BEC-E041503003"),   parsedSession.Location.EVSEs.First().EVSEId);
                Assert.AreEqual (StatusType.AVAILABLE,                 parsedSession.Location.EVSEs.First().Status);
                Assert.AreEqual ("2015-06-29T23:09:10.000Z",           parsedSession.Location.EVSEs.First().LastUpdated.ToIso8601());

                Assert.IsNotNull(                                      parsedSession.Location.EVSEs.First().Connectors);
                Assert.AreEqual (Connector_Id.    Parse("1"),          parsedSession.Location.EVSEs.First().Connectors.First().Id);
                Assert.AreEqual (ConnectorType.   IEC_62196_T2,        parsedSession.Location.EVSEs.First().Connectors.First().Standard);
                Assert.AreEqual (ConnectorFormats.SOCKET,              parsedSession.Location.EVSEs.First().Connectors.First().Format);
                Assert.AreEqual (PowerTypes.      AC_1_PHASE,          parsedSession.Location.EVSEs.First().Connectors.First().PowerType);
                Assert.AreEqual (230,                                  parsedSession.Location.EVSEs.First().Connectors.First().Voltage);
                Assert.AreEqual (64,                                   parsedSession.Location.EVSEs.First().Connectors.First().Amperage);
                Assert.AreEqual (Tariff_Id.       Parse("11"),         parsedSession.Location.EVSEs.First().Connectors.First().TariffId);
                Assert.AreEqual ("2015-06-29T23:09:10.000Z",           parsedSession.Location.EVSEs.First().Connectors.First().LastUpdated.ToIso8601());

            }

        }

        #endregion


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
                               DateTime.   Parse("2020-08-21T00:00:00.000Z").ToUniversalTime(), // Start
                               1.11M, // KWh
                               Auth_Id.    Parse("1234"),
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

                               OCPI.Currency.EUR,
                               SessionStatusTypes.ACTIVE,
                               DateTime.Parse("2020-08-22T00:00:00.000Z").ToUniversalTime(), // End
                               Meter_Id.Parse("Meter0001"),

                               new[] {
                                   ChargingPeriod.Create(
                                       DateTime.Parse("2020-04-12T18:21:49Z").ToUniversalTime(),
                                       new[] {
                                           CDRDimension.Create(
                                               CDRDimensionType.ENERGY,
                                               1.33M
                                           )
                                       }
                                   ),
                                   ChargingPeriod.Create(
                                       DateTime.Parse("2020-04-12T18:21:50Z").ToUniversalTime(),
                                       new[] {
                                           CDRDimension.Create(
                                               CDRDimensionType.TIME,
                                               5.12M
                                           )
                                       }
                                   )
                               },

                               1.12M, // Total Costs

                               DateTime.Parse("2020-09-21T00:00:00.000Z").ToUniversalTime()

                           );

            #endregion

            var json = session1.ToJSON();

            Assert.AreEqual("Session0001",                     json["id"]?.                              Value<String>());
            Assert.AreEqual("2020-08-21T00:00:00.000Z",        json["start_datetime"]?.                  Value<String>());
            Assert.AreEqual("2020-08-22T00:00:00.000Z",        json["end_datetime"]?.                    Value<String>());
            Assert.AreEqual(1.11,                              json["kwh"]?.                             Value<Decimal>());
            Assert.AreEqual("1234",                            json["auth_id"]?.                         Value<String>());
            Assert.AreEqual("AUTH_REQUEST",                    json["auth_method"]?.                     Value<String>());
            Assert.AreEqual("Meter0001",                       json["meter_id"]?.                        Value<String>());
            Assert.AreEqual("EUR",                             json["currency"]?.                        Value<String>());

            //Assert.AreEqual("LOC0001",                         json["location_id"]?.                     Value<String>());
            // charging_periods

            Assert.AreEqual(1.12,                              json["total_cost"]?.                      Value<Decimal>());
            Assert.AreEqual("ACTIVE",                          json["status"]?.                          Value<String>());
            Assert.AreEqual("2020-09-21T00:00:00.000Z",        json["last_updated"]?.                    Value<String>());


            var result = Session.TryParse(json,
                                          out var session2,
                                          out var errorResponse,
                                          session1.CountryCode,
                                          session1.PartyId);

            Assert.IsTrue   (result, errorResponse);
            Assert.IsNotNull(session2);
            Assert.IsNull   (errorResponse);

            if (session2 is not null)
            {

                Assert.AreEqual(session1.CountryCode,               session2.CountryCode);
                Assert.AreEqual(session1.PartyId,                   session2.PartyId);
                Assert.AreEqual(session1.Id,                        session2.Id);
                Assert.AreEqual(session1.Start.ToIso8601(),         session2.Start.ToIso8601());
                Assert.AreEqual(session1.End?. ToIso8601(),         session2.End?. ToIso8601());
                Assert.AreEqual(session1.kWh,                       session2.kWh);
                Assert.AreEqual(session1.AuthId,                    session2.AuthId);
                Assert.AreEqual(session1.AuthMethod,                session2.AuthMethod);
                Assert.AreEqual(session1.Location,                  session2.Location);
                Assert.AreEqual(session1.MeterId,                   session2.MeterId);
                Assert.AreEqual(session1.Currency,                  session2.Currency);
                Assert.AreEqual(session1.ChargingPeriods,           session2.ChargingPeriods);
                Assert.AreEqual(session1.TotalCost,                 session2.TotalCost);
                Assert.AreEqual(session1.Status,                    session2.Status);
                Assert.AreEqual(session1.LastUpdated.ToIso8601(),   session2.LastUpdated.ToIso8601());

            }

        }

        #endregion


    }

}
