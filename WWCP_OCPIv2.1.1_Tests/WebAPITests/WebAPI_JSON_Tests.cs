/*
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

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_1_1.UnitTests.WebAPI
{

    [TestFixture]
    public class WebAPI_JSON_Tests : ANodeTests
    {

        #region WebAPI_GetRemoteParties_Test()

        /// <summary>
        /// WebAPI JSON getting all configured remote parties.
        /// </summary>
        [Test]
        public async Task WebAPI_GetRemoteParties_Test()
        {

            ClassicAssert.IsNotNull(cpoWebAPI);
            ClassicAssert.IsNotNull(emsp1WebAPI);
            ClassicAssert.IsNotNull(emsp2WebAPI);

            ClassicAssert.IsNotNull(cpoVersionsAPIURL);
            ClassicAssert.IsNotNull(emsp1VersionsAPIURL);
            ClassicAssert.IsNotNull(emsp2VersionsAPIURL);

            if (cpoWebAPI is not null &&
                cpoVersionsAPIURL.HasValue)
            {

                var baseURL   = cpoVersionsAPIURL.Value.ToString().Replace(cpoVersionsAPIURL.Value.Path.ToString(), cpoWebAPI.HTTPBaseAPI.RootPath.ToString());
                var response  = await TestHelpers.GetJSONArrayRequest(URL.Parse(baseURL) + "remoteParties");

                #region Documentation

                // [{
                //         "@id":               "DE-GD2_EMSP",
                //         "countryCode":       "DE",
                //         "partyId":           "GD2",
                //         "role":              "EMSP",
                //         "partyStatus":       "ENABLED",
                //         "businessDetails": {
                //             "name":          "GraphDefined EMSP #2 Services",
                //             "website":       "https://www.graphdefined.com/emsp2"
                //         },
                //         "accessInfos": [{
                //             "token":         "emsp2_accessing_cpo++token",
                //             "status":        "ALLOWED"
                //         }],
                //         "remoteAccessInfos": [{
                //             "token":         "cpo_accessing_emsp2++token",
                //             "versionsURL":   "http://localhost:3402/ocpi/v2.1/versions",
                //             "status":        "ONLINE",
                //             "lastUpdate":    "2023-04-30T05:49:16.139Z"
                //         }],
                //         "last_updated":      "2023-04-30T05:49:16.139Z"
                //     },

                //     {
                //         "@id":               "DE-GDF_EMSP",
                //         "countryCode":       "DE",
                //         "partyId":           "GDF",
                //         "role":              "EMSP",
                //         "partyStatus":       "ENABLED",
                //         "businessDetails": {
                //             "name":          "GraphDefined EMSP #1 Services",
                //             "website":       "https://www.graphdefined.com/emsp1"
                //         },
                //         "accessInfos": [{
                //             "token":         "emsp1_accessing_cpo++token",
                //             "status":        "ALLOWED"
                //         }],
                //         "remoteAccessInfos": [{
                //             "token":         "cpo_accessing_emsp1++token",
                //             "versionsURL":   "http://localhost:3401/ocpi/v2.1/versions",
                //             "status":        "ONLINE",
                //             "lastUpdate":    "2023-04-30T05:49:16.123Z"
                //         }],
                //         "last_updated":      "2023-04-30T05:49:16.125Z"
                //     },

                //     {
                //         "@id":               "XX-BLE_EMSP",
                //         "countryCode":       "XX",
                //         "partyId":           "BLE",
                //         "role":              "EMSP",
                //         "partyStatus":       "ENABLED",
                //         "businessDetails": {
                //             "name":          "Blocked EMSP"
                //         },
                //         "accessInfos": [{
                //             "token":         "blocked-emsp",
                //             "status":        "BLOCKED"
                //         }],
                //         "last_updated":      "2023-04-30T05:49:16.153Z"
                //     }]

                #endregion

                ClassicAssert.IsNotNull(response);
                ClassicAssert.AreEqual (200,            response.HTTPStatusCode.Code);
                ClassicAssert.IsTrue   (Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10));

                var json      = response.Content;
                ClassicAssert.IsNotNull(json);
                ClassicAssert.AreEqual (3, json.Count);

            }

            if (emsp1WebAPI is not null &&
                emsp1VersionsAPIURL.HasValue)
            {

                var baseURL   = emsp1VersionsAPIURL.Value.ToString().Replace(emsp1VersionsAPIURL.Value.Path.ToString(), emsp1WebAPI.HTTPBaseAPI.RootPath.ToString());
                var response  = await TestHelpers.GetJSONArrayRequest(URL.Parse(baseURL) + "remoteParties");

                ClassicAssert.IsNotNull(response);
                ClassicAssert.AreEqual (200,            response.HTTPStatusCode.Code);
                ClassicAssert.IsTrue   (Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10));

                var json      = response.Content;
                ClassicAssert.IsNotNull(json);
                ClassicAssert.AreEqual (2, json.Count);

            }

            if (emsp2WebAPI is not null &&
                emsp2VersionsAPIURL.HasValue)
            {

                var baseURL   = emsp2VersionsAPIURL.Value.ToString().Replace(emsp2VersionsAPIURL.Value.Path.ToString(), emsp2WebAPI.HTTPBaseAPI.RootPath.ToString());
                var response  = await TestHelpers.GetJSONArrayRequest(URL.Parse(baseURL) + "remoteParties");

                ClassicAssert.IsNotNull(response);
                ClassicAssert.AreEqual (200,            response.HTTPStatusCode.Code);
                ClassicAssert.IsTrue   (Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10));

                var json      = response.Content;
                ClassicAssert.IsNotNull(json);
                ClassicAssert.AreEqual (2, json.Count);

            }

        }

        #endregion

        #region WebAPI_GetClients_Test()

        /// <summary>
        /// WebAPI JSON GetClients.
        /// </summary>
        [Test]
        public async Task WebAPI_GetClients_Test()
        {

            ClassicAssert.IsNotNull(cpoWebAPI);
            ClassicAssert.IsNotNull(emsp1WebAPI);
            ClassicAssert.IsNotNull(emsp2WebAPI);

            ClassicAssert.IsNotNull(cpoVersionsAPIURL);
            ClassicAssert.IsNotNull(emsp1VersionsAPIURL);
            ClassicAssert.IsNotNull(emsp2VersionsAPIURL);

            if (cpoWebAPI is not null &&
                cpoVersionsAPIURL.HasValue)
            {

                var baseURL   = cpoVersionsAPIURL.Value.ToString().Replace(cpoVersionsAPIURL.Value.Path.ToString(), cpoWebAPI.HTTPBaseAPI.RootPath.ToString());
                var response  = await TestHelpers.GetJSONArrayRequest(URL.Parse(baseURL) + "clients");

                ClassicAssert.IsNotNull(response);
                ClassicAssert.AreEqual (200,            response.HTTPStatusCode.Code);
                ClassicAssert.IsTrue   (Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10));

                var json      = response.Content;
                ClassicAssert.IsNotNull(json);
                ClassicAssert.AreEqual (0, json.Count);

            }

            if (emsp1WebAPI is not null &&
                emsp1VersionsAPIURL.HasValue)
            {

                var baseURL   = emsp1VersionsAPIURL.Value.ToString().Replace(emsp1VersionsAPIURL.Value.Path.ToString(), emsp1WebAPI.HTTPBaseAPI.RootPath.ToString());
                var response  = await TestHelpers.GetJSONArrayRequest(URL.Parse(baseURL) + "clients");

                ClassicAssert.IsNotNull(response);
                ClassicAssert.AreEqual (200,            response.HTTPStatusCode.Code);
                ClassicAssert.IsTrue   (Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10));

                var json      = response.Content;
                ClassicAssert.IsNotNull(json);
                ClassicAssert.AreEqual (0, json.Count);

            }

            if (emsp2WebAPI is not null &&
                emsp2VersionsAPIURL.HasValue)
            {

                var baseURL   = emsp2VersionsAPIURL.Value.ToString().Replace(emsp2VersionsAPIURL.Value.Path.ToString(), emsp2WebAPI.HTTPBaseAPI.RootPath.ToString());
                var response  = await TestHelpers.GetJSONArrayRequest(URL.Parse(baseURL) + "clients");

                ClassicAssert.IsNotNull(response);
                ClassicAssert.AreEqual (200,            response.HTTPStatusCode.Code);
                ClassicAssert.IsTrue   (Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10));

                var json      = response.Content;
                ClassicAssert.IsNotNull(json);
                ClassicAssert.AreEqual (0, json.Count);

            }

        }

        #endregion

    }

}
