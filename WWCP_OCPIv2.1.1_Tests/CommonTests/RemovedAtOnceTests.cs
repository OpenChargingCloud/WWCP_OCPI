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

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_1_1.UnitTests.CommonTests
{

    /// <summary>
    /// What is removed at once - every remote party, every token - is still
    /// removed at the next start.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Such a command carries no data, and it was written as a line without
    /// its command: the line left out every property whose value was null,
    /// the command's as well. The next start passed over it, and everything
    /// removed at once was there again. Every token removed at once was
    /// replayed under another name besides, and so was not removed either.
    /// </para>
    /// <para>
    /// The next start is a Common API made anew on the same directory, which
    /// reads the files back as a node's start does - see RemotePartyFileTests.
    /// Nothing here listens: the HTTP servers are made and never started.
    /// </para>
    /// </remarks>
    [TestFixture]
    public class RemovedAtOnceTests
    {

        #region Data

        private static readonly CountryCode     countryCode  = CountryCode.Parse("DE");
        private static readonly Party_Id        partyId      = Party_Id.   Parse("BBB");
        private static readonly RemoteParty_Id  id           = RemoteParty_Id.Parse("DE-BBB_CPO");
        private static readonly BusinessDetails business     = new ("Test CPO");
        private static readonly URL             theirURL     = URL.Parse("https://peer.example/ocpi/versions");

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
        public void TearDown()
        {

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


        #region EveryRemotePartyRemovedAtOnceIsGoneAtTheNextStart()

        [Test]
        public async Task EveryRemotePartyRemovedAtOnceIsGoneAtTheNextStart()
        {

            var added = await api.AddRemoteParty(countryCode, partyId, Role.CPO, business, AccessToken.NewRandom(), theirURL, AccessToken.NewRandom());

            Assert.That(added.IsSuccess, Is.True, $"The remote party to remove could not be added: {added.ErrorResponse}");

            await api.RemoveAllRemoteParties();

            Assert.Multiple(() => {
                Assert.That(api.         RemoteParties.Select(party => party.Id), Is.Empty, "The remote parties removed at once are still there.");
                Assert.That(ACommonAPI().RemoteParties.Select(party => party.Id), Is.Empty, "The next start knows the remote parties removed at once.");
            });

        }

        #endregion

        #region EveryTokenRemovedAtOnceIsGoneAtTheNextStart()

        [Test]
        public async Task EveryTokenRemovedAtOnceIsGoneAtTheNextStart()
        {

            var added    = await api.AddToken(AToken("TOKEN0001"));

            Assert.That(added.IsSuccess, Is.True, $"The token to remove could not be added: {added.ErrorResponse}");

            var removed  = await api.RemoveAllTokens();

            Assert.That(removed.IsSuccess, Is.True, $"The tokens could not be removed: {removed.ErrorResponse}");

            // The assets go through a queue, and are all in their file once it
            // is disposed.
            await api.BaseAPI.DisposeAsync();

            Assert.That(ACommonAPI().GetTokenStatus().Select(tokenStatus => tokenStatus.Token.Id.ToString()), Is.Empty, "The next start knows the tokens removed at once.");

        }

        #endregion


        #region (private static) AToken(Id)

        /// <summary>
        /// A token of this Common API's own party, which it keeps tokens for.
        /// </summary>
        private static Token AToken(String Id)

            => new (
                   CountryCode:     CountryCode.Parse("DE"),
                   PartyId:         Party_Id.   Parse("GEF"),
                   Id:              Token_Id.   Parse(Id),
                   Type:            TokenType.RFID,
                   AuthId:          Auth_Id.    Parse("DE-GEF-C12345678-X"),
                   Issuer:          "GraphDefined CA",
                   IsValid:         true,
                   WhitelistType:   WhitelistType.NEVER
               );

        #endregion

        #region (private) ACommonAPI()

        /// <summary>
        /// A Common API on this test's directory. Made anew, it reads back what
        /// the one before it wrote, as the next start of a node does.
        /// </summary>
        private CommonAPI ACommonAPI()

            => new (

                   OurBusinessDetails:  new BusinessDetails("GraphDefined CSO"),
                   OurCountryCode:      CountryCode.Parse("DE"),
                   OurPartyId:          Party_Id.   Parse("GEF"),
                   OurRole:             Role.CPO,

                   BaseAPI:             new CommonHTTPAPI(
                                          HTTPAPI:          new HTTPExtAPI(
                                                                HTTPServer: new HTTPServer(TCPPort: IPPort.Parse(3999))
                                                            ),
                                          OurBaseURL:       URL.Parse("http://127.0.0.1:3999/ocpi"),
                                          OurVersionsURL:   URL.Parse("http://127.0.0.1:3999/ocpi/versions"),
                                          RootPath:         HTTPPath.Parse("/ocpi"),
                                          DisableLogging:   true,
                                          LoggingPath:      directory
                                      ),

                   URLPathPrefix:       HTTPPath.Parse("/ocpi/v2.1.1"),
                   DatabaseFilePath:    directory,
                   DisableLogging:      true,
                   LoggingPath:         directory

               );

        #endregion

    }

}
