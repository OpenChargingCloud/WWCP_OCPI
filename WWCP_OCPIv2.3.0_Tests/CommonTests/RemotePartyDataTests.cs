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

using org.GraphDefined.Vanaheimr.Aegir;
using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_3_0.UnitTests.CommonTests
{

    /// <summary>
    /// What a remote CPO pushes to an EMSP is taken in where the CPO is a
    /// remote party of the EMSP - nothing else has to say so - and is kept
    /// apart from the EMSP's own parties.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The Common API took a location, tariff, session or CDR only for one of
    /// its own parties: an EMSP refused every push of a CPO it had registered
    /// with, unless it had made the CPO a party of its own as well. And its own
    /// parties are what CredentialsRoles() hands a peer as our roles, and what
    /// the version details offer the modules of: an EMSP that did so said it
    /// was a CPO too.
    /// </para>
    /// <para>
    /// The next start is a Common API made anew on the same directory, which
    /// reads back the remote parties first and the assets after them, as a
    /// node's start does. Nothing here listens: the HTTP server is made and
    /// never started.
    /// </para>
    /// </remarks>
    [TestFixture]
    public class RemotePartyDataTests
    {

        #region Data

        private static readonly Party_Idv3      ourPartyId   = Party_Idv3.From(CountryCode.Parse("DE"), Party_Id.Parse("GDF"));
        private static readonly Party_Idv3      remoteCPOId  = Party_Idv3.From(CountryCode.Parse("DE"), Party_Id.Parse("XYZ"));
        private static readonly RemoteParty_Id  remoteId     = RemoteParty_Id.Parse("DE-XYZ_CPO");

        /// <summary>
        /// When the location was last updated: a time of its own, so that it
        /// does not read the clock.
        /// </summary>
        private static readonly DateTimeOffset  start        = new (2026, 10, 1, 8, 0, 0, TimeSpan.Zero);

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


        #region ALocationOfARegisteredCPOIsTakenAndTheCPOIsNoneOfOurParties()

        [Test]
        public async Task ALocationOfARegisteredCPOIsTakenAndTheCPOIsNoneOfOurParties()
        {

            var registered  = await api.AddRemoteParty(remoteId, Roles, AccessToken.NewRandom());
            var pushed      = await api.AddOrUpdateLocation(ALocation());

            Assert.Multiple(() => {

                Assert.That(registered.IsSuccess, Is.True, $"The remote CPO could not be registered: {registered.ErrorResponse}");
                Assert.That(pushed.    IsSuccess, Is.True, $"The location of the remote CPO was refused: {pushed.ErrorResponse}");

                Assert.That(api.TryGetLocation(remoteCPOId, Location_Id.Parse("LOCATION0001"), out _), Is.True,
                            "The location of the remote CPO is not there.");

                Assert.That(api.Parties.Select(party => party.Id), Is.EquivalentTo(new[] { ourPartyId }),
                            "The remote CPO became one of our own parties.");

                Assert.That(api.CredentialsRoles().Select(role => $"{role.PartyId} {role.Role}"), Is.EquivalentTo(new[] { $"{ourPartyId} {Role.EMSP}" }),
                            "Our credentials hand the remote CPO's role to a peer as ours.");

            });

        }

        #endregion

        #region ALocationOfAPartyNobodyRegisteredIsRefused()

        [Test]
        public async Task ALocationOfAPartyNobodyRegisteredIsRefused()
        {

            var pushed = await api.AddOrUpdateLocation(ALocation());

            Assert.Multiple(() => {
                Assert.That(pushed.IsSuccess,     Is.False, "The location of a party nobody registered was taken.");
                Assert.That(pushed.ErrorResponse, Is.EqualTo($"The party identification '{remoteCPOId}' of the location is unknown!"));
            });

        }

        #endregion

        #region ALocationOfARegisteredCPOIsThereAtTheNextStart()

        [Test]
        public async Task ALocationOfARegisteredCPOIsThereAtTheNextStart()
        {

            var registered  = await api.AddRemoteParty(remoteId, Roles, AccessToken.NewRandom());
            var pushed      = await api.AddOrUpdateLocation(ALocation());

            Assert.Multiple(() => {
                Assert.That(registered.IsSuccess, Is.True, $"The remote CPO could not be registered: {registered.ErrorResponse}");
                Assert.That(pushed.    IsSuccess, Is.True, $"The location of the remote CPO was refused: {pushed.ErrorResponse}");
            });

            await api.BaseAPI.DisposeAsync();

            api = ACommonAPI();

            Assert.Multiple(() => {

                Assert.That(api.TryGetLocation(remoteCPOId, Location_Id.Parse("LOCATION0001"), out _), Is.True,
                            "The next start does not know the location of the remote CPO.");

                Assert.That(api.Parties.Select(party => party.Id), Is.EquivalentTo(new[] { ourPartyId }),
                            "At the next start the remote CPO is one of our own parties.");

            });

        }

        #endregion


        #region (private static) Roles / ALocation()

        /// <summary>
        /// The roles of the remote CPO.
        /// </summary>
        private static IEnumerable<CredentialsRole> Roles

            => [
                   new CredentialsRole(
                       remoteCPOId.CountryCode,
                       remoteCPOId.PartyId,
                       Role.CPO,
                       new BusinessDetails("XYZ Charging")
                   )
               ];

        /// <summary>
        /// A location of the remote CPO.
        /// </summary>
        private static Location ALocation()

            => new (
                   CountryCode:  remoteCPOId.CountryCode,
                   PartyId:      remoteCPOId.PartyId,
                   Id:           Location_Id.Parse("LOCATION0001"),
                   Publish:      true,
                   Address:      "Biberweg 18",
                   City:         "Jena",
                   Country:      Country.Germany,
                   Coordinates:  GeoCoordinate.Parse(50.9, 11.6),
                   TimeZone:     "Europe/Berlin",
                   LastUpdated:  start
               );

        #endregion

        #region (private) ACommonAPI()

        /// <summary>
        /// The Common API of an EMSP on this test's directory. Made anew, it
        /// reads back what the one before it wrote, as the next start of a node
        /// does.
        /// </summary>
        private CommonAPI ACommonAPI()

            => new (

                   OurPartyData:      [
                                          new PartyData(
                                              ourPartyId,
                                              Role.EMSP,
                                              new BusinessDetails("GraphDefined EMSP")
                                          )
                                      ],
                   DefaultPartyId:    ourPartyId,

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

                   URLPathPrefix:     HTTPPath.Parse("/ocpi/v2.3.0"),
                   DatabaseFilePath:  directory,
                   DisableLogging:    true,
                   LoggingPath:       directory

               );

        #endregion

    }

}
