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

            Assert.That(result,        Is.True, errorResponse);
            Assert.That(parsedSession, Is.Not.Null);
            Assert.That(errorResponse, Is.Null);

            if (parsedSession is not null)
            {

                Assert.That(parsedSession.Id,                      Is.EqualTo(Session_Id.Parse("101")));
                Assert.That(parsedSession.Start.ToISO8601(),       Is.EqualTo("2015-06-29T22:39:09.000Z"));
                Assert.That(parsedSession.kWh,                     Is.EqualTo(WattHour.FromKWh(0.0M)));
                Assert.That(parsedSession.AuthId,                  Is.EqualTo(Auth_Id.Parse("DE8ACC12E46L89")));
                Assert.That(parsedSession.AuthMethod,              Is.EqualTo(AuthMethods.WHITELIST));
                Assert.That(parsedSession.Currency,                Is.EqualTo(Currency.EUR));
                Assert.That(parsedSession.TotalCost,               Is.EqualTo(2.50M));
                Assert.That(parsedSession.Status,                  Is.EqualTo(SessionStatusTypes.PENDING));
                Assert.That(parsedSession.LastUpdated.ToISO8601(), Is.EqualTo("2015-06-29T22:39:09.000Z"));

                Assert.That(parsedSession.Location,                             Is.Not.Null);
                Assert.That(parsedSession.Location.Id,                          Is.EqualTo(Location_Id.Parse("LOC1")));
                Assert.That(parsedSession.Location.LocationType,                Is.EqualTo(LocationType.ON_STREET));
                Assert.That(parsedSession.Location.Name,                        Is.EqualTo("Gent Zuid"));
                Assert.That(parsedSession.Location.Address,                     Is.EqualTo("F.Rooseveltlaan 3A"));
                Assert.That(parsedSession.Location.City,                        Is.EqualTo("Gent"));
                Assert.That(parsedSession.Location.PostalCode,                  Is.EqualTo("9000"));
                Assert.That(parsedSession.Location.Country,                     Is.EqualTo(Country.Belgium));
                Assert.That(parsedSession.Location.Coordinates.Latitude. Value, Is.EqualTo(3.729944));
                Assert.That(parsedSession.Location.Coordinates.Longitude.Value, Is.EqualTo(51.047599));
                Assert.That(parsedSession.Location.LastUpdated.ToISO8601(),     Is.EqualTo("2015-06-29T22:39:09.000Z"));

                Assert.That(parsedSession.Location.EVSEs,                                 Is.Not.Null);
                Assert.That(parsedSession.Location.EVSEs.First().UId,                     Is.EqualTo(EVSE_UId.Parse("3256")));
                Assert.That(parsedSession.Location.EVSEs.First().EVSEId,                  Is.EqualTo(EVSE_Id.Parse("BE-BEC-E041503003")));
                Assert.That(parsedSession.Location.EVSEs.First().Status,                  Is.EqualTo(StatusType.AVAILABLE));
                Assert.That(parsedSession.Location.EVSEs.First().LastUpdated.ToISO8601(), Is.EqualTo("2015-06-29T22:39:09.000Z"));

                Assert.That(parsedSession.Location.EVSEs.First().Connectors,                                 Is.Not.Null);
                Assert.That(parsedSession.Location.EVSEs.First().Connectors.First().Id,                      Is.EqualTo(Connector_Id.    Parse("1")));
                Assert.That(parsedSession.Location.EVSEs.First().Connectors.First().Standard,                Is.EqualTo(ConnectorType.   IEC_62196_T2));
                Assert.That(parsedSession.Location.EVSEs.First().Connectors.First().Format,                  Is.EqualTo(ConnectorFormats.SOCKET));
                Assert.That(parsedSession.Location.EVSEs.First().Connectors.First().PowerType,               Is.EqualTo(PowerTypes.      AC_1_PHASE));
                Assert.That(parsedSession.Location.EVSEs.First().Connectors.First().Voltage,                 Is.EqualTo(Volt.FromV(230)));
                Assert.That(parsedSession.Location.EVSEs.First().Connectors.First().Amperage,                Is.EqualTo(Ampere.FromA(64)));
                Assert.That(parsedSession.Location.EVSEs.First().Connectors.First().GetTariffId(),           Is.EqualTo(Tariff_Id.       Parse("11")));
                Assert.That(parsedSession.Location.EVSEs.First().Connectors.First().LastUpdated.ToISO8601(), Is.EqualTo("2015-06-29T22:39:09.000Z"));

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

            Assert.That(result,        Is.True, errorResponse);
            Assert.That(parsedSession, Is.Not.Null);
            Assert.That(errorResponse, Is.Null);

            if (parsedSession is not null)
            {

                Assert.That(parsedSession.Id,                      Is.EqualTo(Session_Id.Parse("101")));
                Assert.That(parsedSession.Start.ToISO8601(),       Is.EqualTo("2015-06-29T22:39:09.000Z"));
                Assert.That(parsedSession.End?. ToISO8601(),       Is.EqualTo("2015-06-29T23:50:16.000Z"));
                Assert.That(parsedSession.kWh,                     Is.EqualTo(WattHour.FromKWh(41.0M)));
                Assert.That(parsedSession.AuthId,                  Is.EqualTo(Auth_Id.Parse("DE8ACC12E46L89")));
                Assert.That(parsedSession.AuthMethod,              Is.EqualTo(AuthMethods.WHITELIST));
                Assert.That(parsedSession.Currency,                Is.EqualTo(Currency.EUR));
                Assert.That(parsedSession.TotalCost,               Is.EqualTo(8.50M));
                Assert.That(parsedSession.Status,                  Is.EqualTo(SessionStatusTypes.COMPLETED));
                Assert.That(parsedSession.LastUpdated.ToISO8601(), Is.EqualTo("2015-06-29T23:09:10.000Z"));

                Assert.That(parsedSession.ChargingPeriods,                                             Is.Not.Null);
                Assert.That(parsedSession.ChargingPeriods.ElementAt(0).StartTimestamp.ToISO8601(),     Is.EqualTo("2015-06-29T22:39:09.000Z"));
                Assert.That(parsedSession.ChargingPeriods.ElementAt(0).Dimensions.ElementAt(0).Type,   Is.EqualTo(CDRDimensionType.ENERGY));
                Assert.That(parsedSession.ChargingPeriods.ElementAt(0).Dimensions.ElementAt(0).Volume, Is.EqualTo(120M));
                Assert.That(parsedSession.ChargingPeriods.ElementAt(0).Dimensions.ElementAt(1).Type,   Is.EqualTo(CDRDimensionType.MAX_CURRENT));
                Assert.That(parsedSession.ChargingPeriods.ElementAt(0).Dimensions.ElementAt(1).Volume, Is.EqualTo(30M));

                Assert.That(parsedSession.ChargingPeriods.ElementAt(1).StartTimestamp.ToISO8601(),     Is.EqualTo("2015-06-29T22:40:54.000Z"));
                Assert.That(parsedSession.ChargingPeriods.ElementAt(1).Dimensions.ElementAt(0).Type,   Is.EqualTo(CDRDimensionType.ENERGY));
                Assert.That(parsedSession.ChargingPeriods.ElementAt(1).Dimensions.ElementAt(0).Volume, Is.EqualTo(41000M));
                Assert.That(parsedSession.ChargingPeriods.ElementAt(1).Dimensions.ElementAt(1).Type,   Is.EqualTo(CDRDimensionType.MIN_CURRENT));
                Assert.That(parsedSession.ChargingPeriods.ElementAt(1).Dimensions.ElementAt(1).Volume, Is.EqualTo(34M));

                Assert.That(parsedSession.ChargingPeriods.ElementAt(2).StartTimestamp.ToISO8601(),     Is.EqualTo("2015-06-29T23:07:09.000Z"));
                Assert.That(parsedSession.ChargingPeriods.ElementAt(2).Dimensions.ElementAt(0).Type,   Is.EqualTo(CDRDimensionType.PARKING_TIME));
                Assert.That(parsedSession.ChargingPeriods.ElementAt(2).Dimensions.ElementAt(0).Volume, Is.EqualTo(0.718M));

                Assert.That(parsedSession.Location,                             Is.Not.Null);
                Assert.That(parsedSession.Location.Id,                          Is.EqualTo(Location_Id.Parse("LOC1")));
                Assert.That(parsedSession.Location.LocationType,                Is.EqualTo(LocationType.ON_STREET));
                Assert.That(parsedSession.Location.Name,                        Is.EqualTo("Gent Zuid"));
                Assert.That(parsedSession.Location.Address,                     Is.EqualTo("F.Rooseveltlaan 3A"));
                Assert.That(parsedSession.Location.City,                        Is.EqualTo("Gent"));
                Assert.That(parsedSession.Location.PostalCode,                  Is.EqualTo("9000"));
                Assert.That(parsedSession.Location.Country,                     Is.EqualTo(Country.Belgium));
                Assert.That(parsedSession.Location.Coordinates.Latitude. Value, Is.EqualTo(3.729944));
                Assert.That(parsedSession.Location.Coordinates.Longitude.Value, Is.EqualTo(51.047599));
                Assert.That(parsedSession.Location.LastUpdated.ToISO8601(),     Is.EqualTo("2015-06-29T23:09:10.000Z"));

                Assert.That(parsedSession.Location.EVSEs,                                 Is.Not.Null);
                Assert.That(parsedSession.Location.EVSEs.First().UId,                     Is.EqualTo(EVSE_UId.Parse("3256")));
                Assert.That(parsedSession.Location.EVSEs.First().EVSEId,                  Is.EqualTo(EVSE_Id.Parse("BE-BEC-E041503003")));
                Assert.That(parsedSession.Location.EVSEs.First().Status,                  Is.EqualTo(StatusType.AVAILABLE));
                Assert.That(parsedSession.Location.EVSEs.First().LastUpdated.ToISO8601(), Is.EqualTo("2015-06-29T23:09:10.000Z"));

                Assert.That(parsedSession.Location.EVSEs.First().Connectors,                                 Is.Not.Null);
                Assert.That(parsedSession.Location.EVSEs.First().Connectors.First().Id,                      Is.EqualTo(Connector_Id.    Parse("1")));
                Assert.That(parsedSession.Location.EVSEs.First().Connectors.First().Standard,                Is.EqualTo(ConnectorType.   IEC_62196_T2));
                Assert.That(parsedSession.Location.EVSEs.First().Connectors.First().Format,                  Is.EqualTo(ConnectorFormats.SOCKET));
                Assert.That(parsedSession.Location.EVSEs.First().Connectors.First().PowerType,               Is.EqualTo(PowerTypes.      AC_1_PHASE));
                Assert.That(parsedSession.Location.EVSEs.First().Connectors.First().Voltage,                 Is.EqualTo(Volt.FromV(230)));
                Assert.That(parsedSession.Location.EVSEs.First().Connectors.First().Amperage,                Is.EqualTo(Ampere.FromA(64)));
                Assert.That(parsedSession.Location.EVSEs.First().Connectors.First().GetTariffId(),           Is.EqualTo(Tariff_Id.       Parse("11")));
                Assert.That(parsedSession.Location.EVSEs.First().Connectors.First().LastUpdated.ToISO8601(), Is.EqualTo("2015-06-29T23:09:10.000Z"));

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
                               WattHour.   FromKWh(1.11M),
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

                               Currency.EUR,
                               SessionStatusTypes.ACTIVE,
                               DateTime.Parse("2020-08-22T00:00:00.000Z").ToUniversalTime(), // End
                               EnergyMeter_Id.Parse("Meter0001"),

                               [
                                   ChargingPeriod.Create(
                                       DateTime.Parse("2020-04-12T18:21:49Z").ToUniversalTime(),
                                       [
                                           CDRDimension.Create(
                                               CDRDimensionType.ENERGY,
                                               1.33M
                                           )
                                       ]
                                   ),
                                   ChargingPeriod.Create(
                                       DateTime.Parse("2020-04-12T18:21:50Z").ToUniversalTime(),
                                       [
                                           CDRDimension.Create(
                                               CDRDimensionType.TIME,
                                               5.12M
                                           )
                                       ]
                                   )
                               ],

                               1.12M, // Total Costs

                               DateTime.Parse("2020-09-21T00:00:00.000Z").ToUniversalTime()

                           );

            #endregion

            var json = session1.ToJSON();

            Assert.That(json["id"]?.                              Value<String>(),  Is.EqualTo("Session0001"));
            Assert.That(json["start_datetime"]?.                  Value<String>(),  Is.EqualTo("2020-08-21T00:00:00.000Z"));
            Assert.That(json["end_datetime"]?.                    Value<String>(),  Is.EqualTo("2020-08-22T00:00:00.000Z"));
            Assert.That(json["kwh"]?.                             Value<Decimal>(), Is.EqualTo(1.11));
            Assert.That(json["auth_id"]?.                         Value<String>(),  Is.EqualTo("1234"));
            Assert.That(json["auth_method"]?.                     Value<String>(),  Is.EqualTo("AUTH_REQUEST"));
            Assert.That(json["meter_id"]?.                        Value<String>(),  Is.EqualTo("Meter0001"));
            Assert.That(json["currency"]?.                        Value<String>(),  Is.EqualTo("EUR"));

            //ClassicAssert.AreEqual("LOC0001",                         json["location_id"]?.                     Value<String>());
            // charging_periods

            Assert.That(json["total_cost"]?.                      Value<Decimal>(), Is.EqualTo(1.12));
            Assert.That(json["status"]?.                          Value<String>(),  Is.EqualTo("ACTIVE"));
            Assert.That(json["last_updated"]?.                    Value<String>(),  Is.EqualTo("2020-09-21T00:00:00.000Z"));


            var result = Session.TryParse(json,
                                          out var session2,
                                          out var errorResponse,
                                          session1.CountryCode,
                                          session1.PartyId);

            Assert.That(result,        Is.True, errorResponse);
            Assert.That(session2,      Is.Not.Null);
            Assert.That(errorResponse, Is.Null);

            if (session2 is not null)
            {

                Assert.That(session2.CountryCode,             Is.EqualTo(session1.CountryCode));
                Assert.That(session2.PartyId,                 Is.EqualTo(session1.PartyId));
                Assert.That(session2.Id,                      Is.EqualTo(session1.Id));
                Assert.That(session2.Start.ToISO8601(),       Is.EqualTo(session1.Start.ToISO8601()));
                Assert.That(session2.End?. ToISO8601(),       Is.EqualTo(session1.End?. ToISO8601()));
                Assert.That(session2.kWh,                     Is.EqualTo(session1.kWh));
                Assert.That(session2.AuthId,                  Is.EqualTo(session1.AuthId));
                Assert.That(session2.AuthMethod,              Is.EqualTo(session1.AuthMethod));
                Assert.That(session2.Location,                Is.EqualTo(session1.Location));
                Assert.That(session2.MeterId,                 Is.EqualTo(session1.MeterId));
                Assert.That(session2.Currency,                Is.EqualTo(session1.Currency));
                Assert.That(session2.ChargingPeriods,         Is.EqualTo(session1.ChargingPeriods));
                Assert.That(session2.TotalCost,               Is.EqualTo(session1.TotalCost));
                Assert.That(session2.Status,                  Is.EqualTo(session1.Status));
                Assert.That(session2.LastUpdated.ToISO8601(), Is.EqualTo(session1.LastUpdated.ToISO8601()));

            }

        }

        #endregion


    }

}
