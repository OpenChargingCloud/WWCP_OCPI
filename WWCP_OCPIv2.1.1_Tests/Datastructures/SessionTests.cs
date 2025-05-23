﻿/*
 * Copyright (c) 2015-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
using NUnit.Framework.Legacy;

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

            ClassicAssert.IsTrue   (result, errorResponse);
            ClassicAssert.IsNotNull(parsedSession);
            ClassicAssert.IsNull   (errorResponse);

            if (parsedSession is not null)
            {

                ClassicAssert.AreEqual (Session_Id.Parse("101"),              parsedSession.Id);
                ClassicAssert.AreEqual ("2015-06-29T22:39:09.000Z",           parsedSession.Start.ToISO8601());
                ClassicAssert.AreEqual (0.0M,                                 parsedSession.kWh);
                ClassicAssert.AreEqual (Auth_Id.Parse("DE8ACC12E46L89"),      parsedSession.AuthId);
                ClassicAssert.AreEqual (AuthMethods.WHITELIST,                parsedSession.AuthMethod);
                ClassicAssert.AreEqual (Currency.EUR,                    parsedSession.Currency);
                ClassicAssert.AreEqual (2.50M,                                parsedSession.TotalCost);
                ClassicAssert.AreEqual (SessionStatusTypes.PENDING,           parsedSession.Status);
                ClassicAssert.AreEqual ("2015-06-29T22:39:09.000Z",           parsedSession.LastUpdated.ToISO8601());

                ClassicAssert.IsNotNull(                                      parsedSession.Location);
                ClassicAssert.AreEqual (Location_Id.Parse("LOC1"),            parsedSession.Location.Id);
                ClassicAssert.AreEqual (LocationType.ON_STREET,               parsedSession.Location.LocationType);
                ClassicAssert.AreEqual ("Gent Zuid",                          parsedSession.Location.Name);
                ClassicAssert.AreEqual ("F.Rooseveltlaan 3A",                 parsedSession.Location.Address);
                ClassicAssert.AreEqual ("Gent",                               parsedSession.Location.City);
                ClassicAssert.AreEqual ("9000",                               parsedSession.Location.PostalCode);
                ClassicAssert.AreEqual (Country.Belgium,                      parsedSession.Location.Country);
                ClassicAssert.AreEqual (3.729944,                             parsedSession.Location.Coordinates.Latitude. Value);
                ClassicAssert.AreEqual (51.047599,                            parsedSession.Location.Coordinates.Longitude.Value);
                ClassicAssert.AreEqual ("2015-06-29T22:39:09.000Z",           parsedSession.Location.LastUpdated.ToISO8601());

                ClassicAssert.IsNotNull(                                      parsedSession.Location.EVSEs);
                ClassicAssert.AreEqual (EVSE_UId.Parse("3256"),               parsedSession.Location.EVSEs.First().UId);
                ClassicAssert.AreEqual (EVSE_Id.Parse("BE-BEC-E041503003"),   parsedSession.Location.EVSEs.First().EVSEId);
                ClassicAssert.AreEqual (StatusType.AVAILABLE,                 parsedSession.Location.EVSEs.First().Status);
                ClassicAssert.AreEqual ("2015-06-29T22:39:09.000Z",           parsedSession.Location.EVSEs.First().LastUpdated.ToISO8601());

                ClassicAssert.IsNotNull(                                      parsedSession.Location.EVSEs.First().Connectors);
                ClassicAssert.AreEqual (Connector_Id.    Parse("1"),          parsedSession.Location.EVSEs.First().Connectors.First().Id);
                ClassicAssert.AreEqual (ConnectorType.   IEC_62196_T2,        parsedSession.Location.EVSEs.First().Connectors.First().Standard);
                ClassicAssert.AreEqual (ConnectorFormats.SOCKET,              parsedSession.Location.EVSEs.First().Connectors.First().Format);
                ClassicAssert.AreEqual (PowerTypes.      AC_1_PHASE,          parsedSession.Location.EVSEs.First().Connectors.First().PowerType);
                ClassicAssert.AreEqual (230,                                  parsedSession.Location.EVSEs.First().Connectors.First().Voltage);
                ClassicAssert.AreEqual (64,                                   parsedSession.Location.EVSEs.First().Connectors.First().Amperage);
                ClassicAssert.AreEqual (Tariff_Id.       Parse("11"),         parsedSession.Location.EVSEs.First().Connectors.First().GetTariffId());
                ClassicAssert.AreEqual ("2015-06-29T22:39:09.000Z",           parsedSession.Location.EVSEs.First().Connectors.First().LastUpdated.ToISO8601());

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

            ClassicAssert.IsTrue   (result, errorResponse);
            ClassicAssert.IsNotNull(parsedSession);
            ClassicAssert.IsNull   (errorResponse);

            if (parsedSession is not null)
            {

                ClassicAssert.AreEqual (Session_Id.Parse("101"),              parsedSession.Id);
                ClassicAssert.AreEqual ("2015-06-29T22:39:09.000Z",           parsedSession.Start.ToISO8601());
                ClassicAssert.AreEqual ("2015-06-29T23:50:16.000Z",           parsedSession.End?. ToISO8601());
                ClassicAssert.AreEqual (41.0M,                                parsedSession.kWh);
                ClassicAssert.AreEqual (Auth_Id.Parse("DE8ACC12E46L89"),      parsedSession.AuthId);
                ClassicAssert.AreEqual (AuthMethods.WHITELIST,                parsedSession.AuthMethod);
                ClassicAssert.AreEqual (Currency.EUR,                    parsedSession.Currency);
                ClassicAssert.AreEqual (8.50M,                                parsedSession.TotalCost);
                ClassicAssert.AreEqual (SessionStatusTypes.COMPLETED,         parsedSession.Status);
                ClassicAssert.AreEqual ("2015-06-29T23:09:10.000Z",           parsedSession.LastUpdated.ToISO8601());

                ClassicAssert.IsNotNull(                                      parsedSession.ChargingPeriods);
                ClassicAssert.AreEqual ("2015-06-29T22:39:09.000Z",           parsedSession.ChargingPeriods.ElementAt(0).StartTimestamp.ToISO8601());
                ClassicAssert.AreEqual (CDRDimensionType.ENERGY,              parsedSession.ChargingPeriods.ElementAt(0).Dimensions.ElementAt(0).Type);
                ClassicAssert.AreEqual (120M,                                 parsedSession.ChargingPeriods.ElementAt(0).Dimensions.ElementAt(0).Volume);
                ClassicAssert.AreEqual (CDRDimensionType.MAX_CURRENT,         parsedSession.ChargingPeriods.ElementAt(0).Dimensions.ElementAt(1).Type);
                ClassicAssert.AreEqual (30M,                                  parsedSession.ChargingPeriods.ElementAt(0).Dimensions.ElementAt(1).Volume);

                ClassicAssert.AreEqual ("2015-06-29T22:40:54.000Z",           parsedSession.ChargingPeriods.ElementAt(1).StartTimestamp.ToISO8601());
                ClassicAssert.AreEqual (CDRDimensionType.ENERGY,              parsedSession.ChargingPeriods.ElementAt(1).Dimensions.ElementAt(0).Type);
                ClassicAssert.AreEqual (41000M,                               parsedSession.ChargingPeriods.ElementAt(1).Dimensions.ElementAt(0).Volume);
                ClassicAssert.AreEqual (CDRDimensionType.MIN_CURRENT,         parsedSession.ChargingPeriods.ElementAt(1).Dimensions.ElementAt(1).Type);
                ClassicAssert.AreEqual (34M,                                  parsedSession.ChargingPeriods.ElementAt(1).Dimensions.ElementAt(1).Volume);

                ClassicAssert.AreEqual ("2015-06-29T23:07:09.000Z",           parsedSession.ChargingPeriods.ElementAt(2).StartTimestamp.ToISO8601());
                ClassicAssert.AreEqual (CDRDimensionType.PARKING_TIME,        parsedSession.ChargingPeriods.ElementAt(2).Dimensions.ElementAt(0).Type);
                ClassicAssert.AreEqual (0.718M,                               parsedSession.ChargingPeriods.ElementAt(2).Dimensions.ElementAt(0).Volume);

                ClassicAssert.IsNotNull(                                      parsedSession.Location);
                ClassicAssert.AreEqual (Location_Id.Parse("LOC1"),            parsedSession.Location.Id);
                ClassicAssert.AreEqual (LocationType.ON_STREET,               parsedSession.Location.LocationType);
                ClassicAssert.AreEqual ("Gent Zuid",                          parsedSession.Location.Name);
                ClassicAssert.AreEqual ("F.Rooseveltlaan 3A",                 parsedSession.Location.Address);
                ClassicAssert.AreEqual ("Gent",                               parsedSession.Location.City);
                ClassicAssert.AreEqual ("9000",                               parsedSession.Location.PostalCode);
                ClassicAssert.AreEqual (Country.Belgium,                      parsedSession.Location.Country);
                ClassicAssert.AreEqual (3.729944,                             parsedSession.Location.Coordinates.Latitude. Value);
                ClassicAssert.AreEqual (51.047599,                            parsedSession.Location.Coordinates.Longitude.Value);
                ClassicAssert.AreEqual ("2015-06-29T23:09:10.000Z",           parsedSession.Location.LastUpdated.ToISO8601());

                ClassicAssert.IsNotNull(                                      parsedSession.Location.EVSEs);
                ClassicAssert.AreEqual (EVSE_UId.Parse("3256"),               parsedSession.Location.EVSEs.First().UId);
                ClassicAssert.AreEqual (EVSE_Id.Parse("BE-BEC-E041503003"),   parsedSession.Location.EVSEs.First().EVSEId);
                ClassicAssert.AreEqual (StatusType.AVAILABLE,                 parsedSession.Location.EVSEs.First().Status);
                ClassicAssert.AreEqual ("2015-06-29T23:09:10.000Z",           parsedSession.Location.EVSEs.First().LastUpdated.ToISO8601());

                ClassicAssert.IsNotNull(                                      parsedSession.Location.EVSEs.First().Connectors);
                ClassicAssert.AreEqual (Connector_Id.    Parse("1"),          parsedSession.Location.EVSEs.First().Connectors.First().Id);
                ClassicAssert.AreEqual (ConnectorType.   IEC_62196_T2,        parsedSession.Location.EVSEs.First().Connectors.First().Standard);
                ClassicAssert.AreEqual (ConnectorFormats.SOCKET,              parsedSession.Location.EVSEs.First().Connectors.First().Format);
                ClassicAssert.AreEqual (PowerTypes.      AC_1_PHASE,          parsedSession.Location.EVSEs.First().Connectors.First().PowerType);
                ClassicAssert.AreEqual (230,                                  parsedSession.Location.EVSEs.First().Connectors.First().Voltage);
                ClassicAssert.AreEqual (64,                                   parsedSession.Location.EVSEs.First().Connectors.First().Amperage);
                ClassicAssert.AreEqual (Tariff_Id.       Parse("11"),         parsedSession.Location.EVSEs.First().Connectors.First().GetTariffId());
                ClassicAssert.AreEqual ("2015-06-29T23:09:10.000Z",           parsedSession.Location.EVSEs.First().Connectors.First().LastUpdated.ToISO8601());

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
                               WattHour.   ParseKWh(1.11M),
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
                                   new ChargingPeriod(
                                       DateTime.Parse("2020-04-12T18:21:49Z").ToUniversalTime(),
                                       [
                                           CDRDimension.Create(
                                               CDRDimensionType.ENERGY,
                                               1.33M
                                           )
                                       ]
                                   ),
                                   new ChargingPeriod(
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

            ClassicAssert.AreEqual("Session0001",                     json["id"]?.                              Value<String>());
            ClassicAssert.AreEqual("2020-08-21T00:00:00.000Z",        json["start_datetime"]?.                  Value<String>());
            ClassicAssert.AreEqual("2020-08-22T00:00:00.000Z",        json["end_datetime"]?.                    Value<String>());
            ClassicAssert.AreEqual(1.11,                              json["kwh"]?.                             Value<Decimal>());
            ClassicAssert.AreEqual("1234",                            json["auth_id"]?.                         Value<String>());
            ClassicAssert.AreEqual("AUTH_REQUEST",                    json["auth_method"]?.                     Value<String>());
            ClassicAssert.AreEqual("Meter0001",                       json["meter_id"]?.                        Value<String>());
            ClassicAssert.AreEqual("EUR",                             json["currency"]?.                        Value<String>());

            //ClassicAssert.AreEqual("LOC0001",                         json["location_id"]?.                     Value<String>());
            // charging_periods

            ClassicAssert.AreEqual(1.12,                              json["total_cost"]?.                      Value<Decimal>());
            ClassicAssert.AreEqual("ACTIVE",                          json["status"]?.                          Value<String>());
            ClassicAssert.AreEqual("2020-09-21T00:00:00.000Z",        json["last_updated"]?.                    Value<String>());


            var result = Session.TryParse(json,
                                          out var session2,
                                          out var errorResponse,
                                          session1.CountryCode,
                                          session1.PartyId);

            ClassicAssert.IsTrue   (result, errorResponse);
            ClassicAssert.IsNotNull(session2);
            ClassicAssert.IsNull   (errorResponse);

            if (session2 is not null)
            {

                ClassicAssert.AreEqual(session1.CountryCode,               session2.CountryCode);
                ClassicAssert.AreEqual(session1.PartyId,                   session2.PartyId);
                ClassicAssert.AreEqual(session1.Id,                        session2.Id);
                ClassicAssert.AreEqual(session1.Start.ToISO8601(),         session2.Start.ToISO8601());
                ClassicAssert.AreEqual(session1.End?. ToISO8601(),         session2.End?. ToISO8601());
                ClassicAssert.AreEqual(session1.kWh,                       session2.kWh);
                ClassicAssert.AreEqual(session1.AuthId,                    session2.AuthId);
                ClassicAssert.AreEqual(session1.AuthMethod,                session2.AuthMethod);
                ClassicAssert.AreEqual(session1.Location,                  session2.Location);
                ClassicAssert.AreEqual(session1.MeterId,                   session2.MeterId);
                ClassicAssert.AreEqual(session1.Currency,                  session2.Currency);
                ClassicAssert.AreEqual(session1.ChargingPeriods,           session2.ChargingPeriods);
                ClassicAssert.AreEqual(session1.TotalCost,                 session2.TotalCost);
                ClassicAssert.AreEqual(session1.Status,                    session2.Status);
                ClassicAssert.AreEqual(session1.LastUpdated.ToISO8601(),   session2.LastUpdated.ToISO8601());

            }

        }

        #endregion


    }

}
