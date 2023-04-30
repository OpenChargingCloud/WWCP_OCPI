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

#endregion

namespace cloud.charging.open.protocols.OCPIv2_1_1.UnitTests.WebAPI
{

    [TestFixture]
    public class WebAPI_Tests : ANodeTests
    {

        #region WebAPI_Root_Test()

        /// <summary>
        /// WebAPI Get Root.
        /// </summary>
        [Test]
        public async Task WebAPI_Root_Test()
        {

            Assert.IsNotNull(cpoWebAPI);
            Assert.IsNotNull(emsp1WebAPI);
            Assert.IsNotNull(emsp2WebAPI);

            Assert.IsNotNull(cpoVersionsAPIURL);
            Assert.IsNotNull(emsp1VersionsAPIURL);
            Assert.IsNotNull(emsp2VersionsAPIURL);

            if (cpoWebAPI is not null &&
                cpoVersionsAPIURL.HasValue)
            {

                var baseURL   = cpoVersionsAPIURL.Value.ToString().Replace(cpoVersionsAPIURL.Value.Path.ToString(), cpoWebAPI.URLPathPrefix.ToString());
                var response  = await TestHelpers.GetHTMLRequest(URL.Parse(baseURL) + "clients");

                Assert.IsNotNull(response);
                Assert.AreEqual (200,            response.HTTPStatusCode.Code);
                Assert.IsTrue   (Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10));

                var json      = JArray.Parse(response.HTTPBodyAsUTF8String!);
                Assert.IsNotNull(json);
                Assert.AreEqual (0, json.Count);

            }

        }

        #endregion

    }

}
