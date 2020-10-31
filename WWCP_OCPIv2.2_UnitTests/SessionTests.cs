/*
 * Copyright (c) 2015-2020 GraphDefined GmbH
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

using System;

using NUnit.Framework;
using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

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

            var Session1 = new Session(
                               CountryCode.Parse("DE"),
                               Party_Id.   Parse("GEF"),
                               Session_Id. Parse("Session0001"),
                               DateTime.Parse("2020-08-21T00:00:00.000Z"), // Start
                               1.11M, // KWh
                               new CDRToken(
                                   Token_Id.Parse("1234"),
                                   TokenTypes.RFID,
                                   Contract_Id.Parse("Contract0815")
                               ),
                               AuthMethods.AUTH_REQUEST,
                               Location_Id.Parse("LOC0001"),
                               EVSE_UId.Parse("EVSE0001"),
                               Connector_Id.Parse("C1"),
                               Currency.EUR,
                               SessionStatusTypes.ACTIVE,
                               DateTime.Parse("2020-08-22T00:00:00.000Z"), // End
                               AuthorizationReference.Parse("Auth1234"),

                               Meter_Id.Parse("Meter0001"),

                               // OCPI Computer Science Extentions
                               new EnergyMeter(
                                   Meter_Id.Parse("Meter0815"),
                                   "EnergyMeter Model #1",
                                   "hw. v1.80",
                                   "fw. v1.20",
                                   "Energy Metering Services",
                                   null,
                                   null
                               ),

                               // OCPI Computer Science Extentions
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
                                       DateTime.Parse("2020-04-12T18:21:49Z"),
                                       new CDRDimension[] {
                                           new CDRDimension(
                                               TariffDimensions.ENERGY,
                                               1.33M
                                           )
                                       },
                                       Tariff_Id.Parse("DE*GEF*T0001")
                                   ),
                                   new ChargingPeriod(
                                       DateTime.Parse("2020-04-12T18:21:50Z"),
                                       new CDRDimension[] {
                                           new CDRDimension(
                                               TariffDimensions.TIME,
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

                               DateTime.Parse("2020-09-21T00:00:00.000Z")
                           );

            #endregion

            var JSON = Session1.ToJSON();

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

            Assert.IsTrue(Session.TryParse(JSON, out Session Session2, out String ErrorResponse));
            Assert.IsNull(ErrorResponse);

            Assert.AreEqual(Session1.CountryCode,              Session2.CountryCode);
            Assert.AreEqual(Session1.PartyId,                  Session2.PartyId);
            Assert.AreEqual(Session1.Id,                       Session2.Id);
            Assert.AreEqual(Session1.Start.    ToIso8601(),    Session2.Start.    ToIso8601());
            Assert.AreEqual(Session1.End.Value.ToIso8601(),    Session2.End.Value.ToIso8601());
            Assert.AreEqual(Session1.kWh,                      Session2.kWh);
            Assert.AreEqual(Session1.CDRToken,                 Session2.CDRToken);
            Assert.AreEqual(Session1.AuthMethod,               Session2.AuthMethod);
            Assert.AreEqual(Session1.AuthorizationReference,   Session2.AuthorizationReference);
            Assert.AreEqual(Session1.LocationId,               Session2.LocationId);
            Assert.AreEqual(Session1.EVSEUId,                  Session2.EVSEUId);
            Assert.AreEqual(Session1.ConnectorId,              Session2.ConnectorId);
            Assert.AreEqual(Session1.MeterId,                  Session2.MeterId);
            Assert.AreEqual(Session1.EnergyMeter,              Session2.EnergyMeter);
            Assert.AreEqual(Session1.TransparencySoftwares,    Session2.TransparencySoftwares);
            Assert.AreEqual(Session1.Currency,                 Session2.Currency);
            Assert.AreEqual(Session1.ChargingPeriods,          Session2.ChargingPeriods);
            Assert.AreEqual(Session1.TotalCosts,               Session2.TotalCosts);
            Assert.AreEqual(Session1.Status,                   Session2.Status);
            Assert.AreEqual(Session1.LastUpdated.ToIso8601(),  Session2.LastUpdated.ToIso8601());

        }

        #endregion

    }

}
