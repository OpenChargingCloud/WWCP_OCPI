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

using Newtonsoft.Json.Linq;

using NUnit.Framework;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Aegir;
using cloud.charging.open.protocols.WWCP;
using System.Drawing;
using System.IO;
using System.Xml.Linq;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_1_1.UnitTests
{

    [TestFixture]
    public class LocationsAndStationsTests : AAdapterTests
    {

        #region Add_ChargingLocation_Test1()

        /// <summary>
        /// Add charging location test1 01.
        /// </summary>
        [Test]
        public async Task Add_ChargingLocation_Test1()
        {

            if (graphDefinedCSO is not null &&
                csoAdapter      is not null)
            {

                var addChargingPoolResult = await graphDefinedCSO.CreateChargingPool(

                                                Id:                   ChargingPool_Id.Parse("DE*GEF*POOL1"),
                                                Name:                 I18NString.Create(Languages.en, "Test pool #1"),
                                                Description:          I18NString.Create(Languages.en, "GraphDefined charging pool for tests #1"),

                                                Address:              new Address(

                                                                          Street:             "Biberweg",
                                                                          PostalCode:         "07749",
                                                                          City:               I18NString.Create(Languages.da, "Jena"),
                                                                          Country:            Country.Germany,

                                                                          HouseNumber:        "18",
                                                                          FloorLevel:         null,
                                                                          Region:             null,
                                                                          PostalCodeSub:      null,
                                                                          TimeZone:           null,
                                                                          OfficialLanguages:  null,
                                                                          Comment:            null,

                                                                          CustomData:         null,
                                                                          InternalData:       null

                                                                      ),
                                                GeoLocation:          GeoCoordinate.Parse(50.93, 11.63),

                                                InitialAdminStatus:   ChargingPoolAdminStatusTypes.Operational,
                                                InitialStatus:        ChargingPoolStatusTypes.Available,

                                                Configurator:         chargingPool => {
                                                                      }

                                            );

                Assert.IsNotNull(addChargingPoolResult);


                var allLocations   = csoAdapter.CommonAPI.GetLocations();

                Assert.IsNotNull(allLocations);
                Assert.AreEqual (1, allLocations.Count());

                var chargingPool1  = addChargingPoolResult.ChargingPool;
                Assert.IsNotNull(chargingPool1);


            //var graphDefinedEMSP = cpoWebAPI?.GetCPOClient(
            //                           CountryCode: CountryCode.Parse("DE"),
            //                           PartyId:     Party_Id.   Parse("GDF")
            //                       );

            //Assert.IsNotNull(graphDefinedEMSP);

            //if (graphDefinedEMSP is not null)
            //{

            //    var response = await graphDefinedEMSP.GetVersions();

            //    // HTTP/1.1 200 OK
            //    // Date:                          Sun, 25 Dec 2022 23:16:31 GMT
            //    // Access-Control-Allow-Methods:  OPTIONS, GET
            //    // Access-Control-Allow-Headers:  Authorization
            //    // Vary:                          Accept
            //    // Server:                        GraphDefined Hermod HTTP Server v1.0
            //    // Access-Control-Allow-Origin:   *
            //    // Connection:                    close
            //    // Content-Type:                  application/json; charset=utf-8
            //    // Content-Length:                165
            //    // X-Request-ID:                  nM7djM37h56hQz8t8hKMznnhGYj3CK
            //    // X-Correlation-ID:              53YKxAnt2zM9bGp2AvjK6t83txbCK3
            //    // 
            //    // {
            //    //     "data": [{
            //    //         "version":  "2.1.1",
            //    //         "url":      "http://127.0.0.1:7135/versions/2.1.1"
            //    //     }],
            //    //     "status_code":      1000,
            //    //     "status_message":  "Hello world!",
            //    //     "timestamp":       "2022-12-25T23:16:31.228Z"
            //    // }

            //    Assert.IsNotNull(response);
            //    Assert.AreEqual (200,             response.HTTPResponse?.HTTPStatusCode.Code);
            //    Assert.AreEqual (1000,            response.StatusCode);
            //    Assert.AreEqual ("Hello world!",  response.StatusMessage);
            //    Assert.IsTrue   (Timestamp.Now -  response.Timestamp < TimeSpan.FromSeconds(10));

            //    //Assert.IsNotNull(response.Request);

            //    var versions = response.Data;
            //    Assert.IsNotNull(versions);
            //    Assert.AreEqual (1, response.Data.Count());

            //    var version = versions.First();
            //    Assert.AreEqual (Version_Id.Parse("2.1.1"),    version.Id);
            //    //Assert.AreEqual (emspVersionsAPIURL + "2.1.1", version.URL);

            }

        }

        #endregion

        #region Add_ChargingStation_Test1()

        /// <summary>
        /// Add charging station test1 01.
        /// </summary>
        [Test]
        public async Task Add_ChargingStation_Test1()
        {

            if (graphDefinedCSO is not null &&
                csoAdapter      is not null)
            {

                var addChargingPoolResult = await graphDefinedCSO.CreateChargingPool(

                                                Id:                   ChargingPool_Id.Parse("DE*GEF*POOL1"),
                                                Name:                 I18NString.Create(Languages.en, "Test pool #1"),
                                                Description:          I18NString.Create(Languages.en, "GraphDefined charging pool for tests #1"),

                                                Address:              new Address(

                                                                          Street:             "Biberweg",
                                                                          PostalCode:         "07749",
                                                                          City:               I18NString.Create(Languages.da, "Jena"),
                                                                          Country:            Country.Germany,

                                                                          HouseNumber:        "18",
                                                                          FloorLevel:         null,
                                                                          Region:             null,
                                                                          PostalCodeSub:      null,
                                                                          TimeZone:           null,
                                                                          OfficialLanguages:  null,
                                                                          Comment:            null,

                                                                          CustomData:         null,
                                                                          InternalData:       null

                                                                      ),
                                                GeoLocation:          GeoCoordinate.Parse(50.93, 11.63),

                                                InitialAdminStatus:   ChargingPoolAdminStatusTypes.Operational,
                                                InitialStatus:        ChargingPoolStatusTypes.Available,

                                                Configurator:         chargingPool => {
                                                                      }

                                            );

                Assert.IsNotNull(addChargingPoolResult);


                var allLocations   = csoAdapter.CommonAPI.GetLocations();

                Assert.IsNotNull(allLocations);
                Assert.AreEqual (1, allLocations.Count());

                var chargingPool1  = addChargingPoolResult.ChargingPool;
                Assert.IsNotNull(chargingPool1);


                // ------------------------------------------------------------


                var addChargingStationResult = await chargingPool1!.CreateChargingStation(

                                                   Id:                   ChargingStation_Id.Parse("DE*GEF*STATION_1_A"),
                                                   Name:                 I18NString.Create(Languages.en, "Test station #1A"),
                                                   Description:          I18NString.Create(Languages.en, "GraphDefined charging station for tests #1A"),

                                                   Address:              new Address(

                                                                             Street:             "Biberweg",
                                                                             PostalCode:         "07749",
                                                                             City:               I18NString.Create(Languages.da, "Jena"),
                                                                             Country:            Country.Germany,

                                                                             HouseNumber:        "18",
                                                                             FloorLevel:         null,
                                                                             Region:             null,
                                                                             PostalCodeSub:      null,
                                                                             TimeZone:           null,
                                                                             OfficialLanguages:  null,
                                                                             Comment:            null,

                                                                             CustomData:         null,
                                                                             InternalData:       null

                                                                         ),
                                                   GeoLocation:          GeoCoordinate.Parse(50.93, 11.63),

                                                   InitialAdminStatus:   ChargingStationAdminStatusTypes.Operational,
                                                   InitialStatus:        ChargingStationStatusTypes.Available,

                                                   Configurator:         chargingStation => {
                                                                         }

                                               );

                Assert.IsNotNull(addChargingPoolResult);




            //var graphDefinedEMSP = cpoWebAPI?.GetCPOClient(
            //                           CountryCode: CountryCode.Parse("DE"),
            //                           PartyId:     Party_Id.   Parse("GDF")
            //                       );

            //Assert.IsNotNull(graphDefinedEMSP);

            //if (graphDefinedEMSP is not null)
            //{

            //    var response = await graphDefinedEMSP.GetVersions();

            //    // HTTP/1.1 200 OK
            //    // Date:                          Sun, 25 Dec 2022 23:16:31 GMT
            //    // Access-Control-Allow-Methods:  OPTIONS, GET
            //    // Access-Control-Allow-Headers:  Authorization
            //    // Vary:                          Accept
            //    // Server:                        GraphDefined Hermod HTTP Server v1.0
            //    // Access-Control-Allow-Origin:   *
            //    // Connection:                    close
            //    // Content-Type:                  application/json; charset=utf-8
            //    // Content-Length:                165
            //    // X-Request-ID:                  nM7djM37h56hQz8t8hKMznnhGYj3CK
            //    // X-Correlation-ID:              53YKxAnt2zM9bGp2AvjK6t83txbCK3
            //    // 
            //    // {
            //    //     "data": [{
            //    //         "version":  "2.1.1",
            //    //         "url":      "http://127.0.0.1:7135/versions/2.1.1"
            //    //     }],
            //    //     "status_code":      1000,
            //    //     "status_message":  "Hello world!",
            //    //     "timestamp":       "2022-12-25T23:16:31.228Z"
            //    // }

            //    Assert.IsNotNull(response);
            //    Assert.AreEqual (200,             response.HTTPResponse?.HTTPStatusCode.Code);
            //    Assert.AreEqual (1000,            response.StatusCode);
            //    Assert.AreEqual ("Hello world!",  response.StatusMessage);
            //    Assert.IsTrue   (Timestamp.Now -  response.Timestamp < TimeSpan.FromSeconds(10));

            //    //Assert.IsNotNull(response.Request);

            //    var versions = response.Data;
            //    Assert.IsNotNull(versions);
            //    Assert.AreEqual (1, response.Data.Count());

            //    var version = versions.First();
            //    Assert.AreEqual (Version_Id.Parse("2.1.1"),    version.Id);
            //    //Assert.AreEqual (emspVersionsAPIURL + "2.1.1", version.URL);

            }

        }

        #endregion


    }

}
