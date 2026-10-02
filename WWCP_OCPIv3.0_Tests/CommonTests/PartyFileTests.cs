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
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv3_0.UnitTests.CommonTests
{

    /// <summary>
    /// A party added is there at the next start, and so is every asset filed
    /// under it.
    /// </summary>
    /// <remarks>
    /// <para>
    /// AddParty wrote down only the party's identification, and the next
    /// start passed over that line: the party was gone, and every asset of it
    /// with it, as the replay drops what belongs to a party it does not know.
    /// In 3.0 no party is given at the start, so every party was gone at
    /// every start, and every asset with it.
    /// </para>
    /// <para>
    /// The next start is a Common API made anew on the same directory, which
    /// reads the files back as a node's start does - see RemovedAtOnceTests.
    /// Nothing here listens: the HTTP servers are made and never started.
    /// </para>
    /// </remarks>
    [TestFixture]
    public class PartyFileTests
    {

        #region Data

        private static readonly Party_Idv3  ourPartyId    = Party_Idv3.From(CountryCode.Parse("DE"), Party_Id.Parse("GEF"));
        private static readonly Party_Idv3  addedPartyId  = Party_Idv3.From(CountryCode.Parse("DE"), Party_Id.Parse("XYZ"));

        private String     directory  = default!;
        private CommonAPI  api        = default!;

        #endregion

        #region SetUp / TearDown

        [SetUp]
        public void SetUp()
        {

            directory  = Path.Combine(Path.GetTempPath(), $"WWCP_OCPI_Tests-{Guid.NewGuid():N}");

            Directory.CreateDirectory(directory);

            api        = ACommonAPI();

        }

        [TearDown]
        public async Task TearDown()
        {

            // Its queue written out, before its directory goes.
            await api.BaseAPI.DisposeAsync();

            try
            {
                Directory.Delete(directory, true);
            }
            catch (IOException)
            { }
            catch (UnauthorizedAccessException)
            { }

        }

        #endregion


        #region APartyAddedIsThereAtTheNextStart()

        [Test]
        public async Task APartyAddedIsThereAtTheNextStart()
        {

            var added = await api.AddParty(
                                  addedPartyId,
                                  Role.EMSP,
                                  new BusinessDetails(
                                      "XYZ Mobility",
                                      URL.Parse("https://xyz.example")
                                  ),
                                  AllowDowngrades: true
                              );

            Assert.That(added.IsSuccess, Is.True, $"The party could not be added: {added.ErrorResponse}");

            Assert.That((await NextStart()).Parties.Select(AsText), Is.EquivalentTo(new[] { AsText(added.Data!) }),
                        "The next start does not know the party added as it was added.");

        }

        #endregion

        #region TheAssetsOfAPartyAddedAreThereAtTheNextStart()

        /// <summary>
        /// A token filed under a party added is there at the next start: the
        /// party it belongs to is known again before the token is read back.
        /// </summary>
        [Test]
        public async Task TheAssetsOfAPartyAddedAreThereAtTheNextStart()
        {

            var added  = await api.AddParty(addedPartyId, Role.CPO, new BusinessDetails("XYZ Charging"));
            var token  = await api.AddToken(AToken());

            Assert.Multiple(() => {
                Assert.That(added.IsSuccess, Is.True, $"The party could not be added: {added.ErrorResponse}");
                Assert.That(token.IsSuccess, Is.True, $"The token could not be added: {token.ErrorResponse}");
            });

            Assert.That((await NextStart()).GetTokens(addedPartyId).Select(token => token.Id.ToString()), Is.EquivalentTo(new[] { "TOKEN0001" }),
                        "The next start does not know the token of the party added.");

        }

        #endregion

        #region ALineWithOnlyAPartysIdentificationIsPassedOver()

        /// <summary>
        /// A party added before was written down with its identification only.
        /// Such a line cannot make the party again: the next start passes over
        /// it, and starts all the same.
        /// </summary>
        [Test]
        public async Task ALineWithOnlyAPartysIdentificationIsPassedOver()
        {

            await api.LogAsset(
                      CommonHTTPAPI.addParty,
                      JSONObject.Create(
                          new JProperty("id", addedPartyId.ToString())
                      ),
                      EventTracking_Id.New
                  );

            Assert.That((await NextStart()).Parties.Select(AsText), Is.Empty,
                        "The next start made a party from its identification alone.");

        }

        #endregion


        #region (private static) AToken() / AsText(PartyData)

        /// <summary>
        /// A token of the party added.
        /// </summary>
        private static Token AToken()

            => new (
                   PartyId:     addedPartyId,
                   Id:          Token_Id.   Parse("TOKEN0001"),
                   VersionId:   1,
                   Type:        TokenType.RFID,
                   ContractId:  Contract_Id.Parse("DE-XYZ-C12345678-X"),
                   Issuer:      "XYZ Charging",
                   ValidFrom:   new DateTimeOffset(2026, 10, 1, 8, 0, 0, TimeSpan.Zero),
                   Whitelist:   WhitelistType.NEVER
               );

        /// <summary>
        /// All a party says, as one text.
        /// </summary>
        private static String AsText(PartyData Party)

            => $"{Party.Id} {Party.Role} {Party.AllowDowngrades} {Party.BusinessDetails.ToJSON().ToString(Newtonsoft.Json.Formatting.None)}";

        #endregion

        #region (private) NextStart()

        /// <summary>
        /// The next start of a node: the Common API so far disposed - its assets
        /// go through a queue, and are all in their file once it is - and one
        /// made anew on the same directory, which reads back what it wrote.
        /// </summary>
        private async Task<CommonAPI> NextStart()
        {

            await api.BaseAPI.DisposeAsync();

            api = ACommonAPI();

            return api;

        }

        #endregion

        #region (private) ACommonAPI()

        /// <summary>
        /// A Common API on this test's directory. Made anew, it reads back what
        /// the one before it wrote, as the next start of a node does.
        /// </summary>
        private CommonAPI ACommonAPI()

            => new (

                   OurCredentialRoles:  [
                                            new CredentialsRole(
                                                ourPartyId,
                                                Role.CPO,
                                                new BusinessDetails("GraphDefined CSO")
                                            )
                                        ],
                   DefaultPartyId:      ourPartyId,

                   BaseAPI:           new CommonHTTPAPI(
                                          HTTPAPI:          new HTTPExtAPI(
                                                                HTTPServer: new HTTPServer(TCPPort: IPPort.Parse(3999))
                                                            ),
                                          OurBaseURL:       URL.Parse("http://127.0.0.1:3999/ocpi"),
                                          OurVersionsURL:   URL.Parse("http://127.0.0.1:3999/ocpi/versions"),
                                          RootPath:         HTTPPath.Parse("/ocpi"),
                                          DisableLogging:   true,
                                          LoggingPath:      directory
                                      ),

                   URLPathPrefix:     HTTPPath.Parse("/ocpi/v3.0"),
                   DatabaseFilePath:  directory,
                   DisableLogging:    true,
                   LoggingPath:       directory

               );

        #endregion

    }

}
