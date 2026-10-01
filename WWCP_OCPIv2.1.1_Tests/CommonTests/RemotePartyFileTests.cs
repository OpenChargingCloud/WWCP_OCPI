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
    /// A remote party added, changed or removed while the file the remote
    /// parties are kept in cannot be written: not added, changed or removed,
    /// neither now nor at the next start, and said to be the file's doing.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Both were answered as done. The line went into a queue that told the
    /// debug log alone that it could not be written, and the next start read a
    /// file without it: an added party was gone again, a removed one back, its
    /// token opening the API again.
    /// </para>
    /// <para>
    /// Where a change is kept all the same - a registration the other side
    /// has answered already - it is in effect, and written down later: with
    /// the next line the file takes, when the base API is disposed, or when
    /// asked to.
    /// </para>
    /// <para>
    /// The file is made unwritable the way that stops root as well: a
    /// directory where it would be. The next start is a Common API made anew
    /// on the same directory, which reads the file back as a node's start
    /// does. Nothing here listens: the HTTP servers are made and never started.
    /// </para>
    /// </remarks>
    [TestFixture]
    public class RemotePartyFileTests
    {

        #region Data

        private static readonly CountryCode     countryCode  = CountryCode.Parse("DE");
        private static readonly Party_Id        partyId      = Party_Id.   Parse("BBB");
        private static readonly RemoteParty_Id  id           = RemoteParty_Id.Parse("DE-BBB_CPO");
        private static readonly BusinessDetails business     = new ("Test CPO");
        private static readonly URL             theirURL     = URL.Parse("https://peer.example/ocpi/versions");

        private String       directory   = default!;
        private CommonAPI    api         = default!;
        private AccessToken  theirToken;

        #endregion

        #region SetUp / TearDown

        [SetUp]
        public void SetUp()
        {

            directory   = Path.Combine(Path.GetTempPath(), $"WWCP_OCPI_Tests-{Guid.NewGuid():N}");

            Directory.CreateDirectory(directory);

            theirToken  = AccessToken.NewRandom();
            api         = ACommonAPI();

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


        #region Adds

        /// <summary>
        /// Each way to add a remote party, by what it is handed.
        /// </summary>
        private static readonly Dictionary<String, Func<CommonAPI, AccessToken, Task<AddResult<RemoteParty>>>> adds = new() {

            ["LocalAccessToken"]                                        = (api, theirToken) => api.AddRemoteParty(countryCode, partyId, Role.CPO, business, AccessToken.NewRandom()),
            ["RemoteVersionsURL, RemoteAccessToken"]                    = (api, theirToken) => api.AddRemoteParty(countryCode, partyId, Role.CPO, business, theirURL, theirToken),
            ["LocalAccessToken, RemoteVersionsURL, RemoteAccessToken"]  = (api, theirToken) => api.AddRemoteParty(countryCode, partyId, Role.CPO, business, AccessToken.NewRandom(), theirURL, theirToken),
            ["LocalAccessInfos, RemoteAccessInfos"]                     = (api, theirToken) => api.AddRemoteParty(countryCode, partyId, Role.CPO, business, [ new LocalAccessInfo(AccessToken.NewRandom()) ],
                                                                                                                                                         [ new RemoteAccessInfo(VersionsURL: theirURL, AccessToken: theirToken) ])

        };

        public static IEnumerable<String> Adds
            => adds.Keys;

        #endregion

        #region Removes

        /// <summary>
        /// Each way to remove a remote party, by what it is handed.
        /// </summary>
        private static readonly Dictionary<String, Func<CommonAPI, RemoteParty, AccessToken, Task<Boolean>>> removes = new() {

            ["RemoteParty"]                              = (api, party, theirToken) => api.RemoveRemoteParty(party),
            ["RemotePartyId"]                            = (api, party, theirToken) => api.RemoveRemoteParty(party.Id),
            ["CountryCode, PartyId, Role"]               = (api, party, theirToken) => api.RemoveRemoteParty(countryCode, partyId, Role.CPO),
            ["CountryCode, PartyId, AccessToken"]        = (api, party, theirToken) => api.RemoveRemoteParty(countryCode, partyId, theirToken)

        };

        public static IEnumerable<String> Removes
            => removes.Keys;

        #endregion

        #region AddsIfNotExists, AddsOrUpdates, Updates

        /// <summary>
        /// Each way to add a remote party where there is none of its
        /// identification, by what it is handed.
        /// </summary>
        private static readonly Dictionary<String, Func<CommonAPI, AccessToken, Task<AddResult<RemoteParty>>>> addsIfNotExists = new() {

            ["LocalAccessToken"]                                        = (api, theirToken) => api.AddRemotePartyIfNotExists(countryCode, partyId, Role.CPO, business, AccessToken.NewRandom()),
            ["RemoteVersionsURL, RemoteAccessToken"]                    = (api, theirToken) => api.AddRemotePartyIfNotExists(countryCode, partyId, Role.CPO, business, theirURL, theirToken),
            ["LocalAccessToken, RemoteVersionsURL, RemoteAccessToken"]  = (api, theirToken) => api.AddRemotePartyIfNotExists(countryCode, partyId, Role.CPO, business, AccessToken.NewRandom(), theirURL, theirToken),
            ["LocalAccessInfos, RemoteAccessInfos"]                     = (api, theirToken) => api.AddRemotePartyIfNotExists(countryCode, partyId, Role.CPO, business, [ new LocalAccessInfo(AccessToken.NewRandom()) ],
                                                                                                                                        [ new RemoteAccessInfo(VersionsURL: theirURL, AccessToken: theirToken) ])

        };

        public static IEnumerable<String> AddsIfNotExists
            => addsIfNotExists.Keys;

        /// <summary>
        /// Each way to add a remote party or replace the one of its
        /// identification, by what it is handed.
        /// </summary>
        private static readonly Dictionary<String, Func<CommonAPI, AccessToken, Task<AddOrUpdateResult<RemoteParty>>>> addsOrUpdates = new() {

            ["LocalAccessToken"]                                        = (api, theirToken) => api.AddOrUpdateRemoteParty(countryCode, partyId, Role.CPO, business, AccessToken.NewRandom()),
            ["RemoteVersionsURL, RemoteAccessToken"]                    = (api, theirToken) => api.AddOrUpdateRemoteParty(countryCode, partyId, Role.CPO, business, theirURL, theirToken),
            ["LocalAccessToken, RemoteVersionsURL, RemoteAccessToken"]  = (api, theirToken) => api.AddOrUpdateRemoteParty(countryCode, partyId, Role.CPO, business, AccessToken.NewRandom(), theirURL, theirToken),
            ["LocalAccessInfos, RemoteAccessInfos"]                     = (api, theirToken) => api.AddOrUpdateRemoteParty(countryCode, partyId, Role.CPO, business, [ new LocalAccessInfo(AccessToken.NewRandom()) ],
                                                                                                                                     [ new RemoteAccessInfo(VersionsURL: theirURL, AccessToken: theirToken) ])

        };

        public static IEnumerable<String> AddsOrUpdates
            => addsOrUpdates.Keys;

        /// <summary>
        /// Each way to replace a remote party that is there, by what it is
        /// handed.
        /// </summary>
        private static readonly Dictionary<String, Func<CommonAPI, RemoteParty, AccessToken, Task<UpdateResult<RemoteParty>>>> updates = new() {

            ["LocalAccessToken"]                                        = (api, party, theirToken) => api.UpdateRemoteParty(party, business, AccessToken.NewRandom()),
            ["RemoteVersionsURL, RemoteAccessToken"]                    = (api, party, theirToken) => api.UpdateRemoteParty(party, business, theirURL, theirToken),
            ["LocalAccessToken, RemoteVersionsURL, RemoteAccessToken"]  = (api, party, theirToken) => api.UpdateRemoteParty(party, business, AccessToken.NewRandom(), theirURL, theirToken),
            ["LocalAccessInfos, RemoteAccessInfos"]                     = (api, party, theirToken) => api.UpdateRemoteParty(party, business, [ new LocalAccessInfo(AccessToken.NewRandom()) ],
                                                                                                                                         [ new RemoteAccessInfo(VersionsURL: theirURL, AccessToken: theirToken) ])

        };

        public static IEnumerable<String> Updates
            => updates.Keys;

        #endregion


        #region AnAddItsFileCannotTakeIsNotMade(Add)

        [TestCaseSource(nameof(Adds))]
        public async Task AnAddItsFileCannotTakeIsNotMade(String Add)
        {

            BlockTheFile();

            var added = await adds[Add](api, theirToken);

            Assert.Multiple(() => {
                Assert.That(added.IsSuccess,                              Is.False,             "The file refused the remote party, and it was added all the same.");
                Assert.That(api.RemoteParties.Select(party => party.Id),  Does.Not.Contain(id), "The remote party the file refused is kept all the same.");
            });

            UnblockTheFile();

            Assert.That(ACommonAPI().RemoteParties.Select(party => party.Id), Does.Not.Contain(id), "The next start knows the remote party its file refused.");

        }

        #endregion

        #region ARemovalItsFileCannotTakeIsNotMade(Remove)

        [TestCaseSource(nameof(Removes))]
        public async Task ARemovalItsFileCannotTakeIsNotMade(String Remove)
        {

            var added = await api.AddRemoteParty(countryCode, partyId, Role.CPO, business, AccessToken.NewRandom(), theirURL, theirToken);

            Assert.That(added.IsSuccess, Is.True, $"The remote party to remove could not be added: {added.ErrorResponse}");

            // What the next start below reads the remote party back from.
            Assert.That(await File.ReadAllTextAsync(api.RemotePartyDBFileName), Does.Contain(CommonHTTPAPI.addRemoteParty), "The remote party that was added is not in its file.");

            BlockTheFile();

            var removed = await removes[Remove](api, added.Data!, theirToken);

            Assert.Multiple(() => {
                Assert.That(removed,                                      Is.False,         "The file refused the removal, and the remote party was removed all the same.");
                Assert.That(api.RemoteParties.Select(party => party.Id),  Does.Contain(id), "The remote party the file would not let go of is gone all the same.");
            });

            UnblockTheFile();

            Assert.That(ACommonAPI().RemoteParties.Select(party => party.Id), Does.Contain(id), "The next start has lost the remote party whose removal the file refused.");

        }

        #endregion

        #region ARemotePartysLineDoesNotWaitBehindTheAssets()

        /// <summary>
        /// A change of the remote parties is in their file when the call that
        /// made it returns - also one that does not yet say what its file did,
        /// and however much else is waiting to be written.
        /// </summary>
        /// <remarks>
        /// The assets are written through a queue, and the remote parties went
        /// through the same one: behind a thousand lines of it, their line was
        /// not written yet when the call had long returned. The file of the
        /// remote parties also has one writer this way, not two that could
        /// open it at the same moment.
        /// </remarks>
        [Test]
        public async Task ARemotePartysLineDoesNotWaitBehindTheAssets()
        {

            var queued = Path.Combine(directory, "Queued.db");

            for (var line = 0; line < 1000; line++)
                await api.BaseAPI.WriteToDatabase(queued, $"line {line}");

            await api.AddOrUpdateRemoteParty(countryCode, partyId, Role.CPO, business, AccessToken.NewRandom());
            // Each line is found by its event tracking identification: a command
            // without data is written without its name.
            var probes = Enumerable.Range(0, 4).Select(_ => EventTracking_Id.New).ToArray();

            await api.LogRemoteParty       ("probe",               probes[0]);
            await api.LogRemoteParty       ("probe", "\"a text\"", probes[1]);
            await api.LogRemoteParty       ("probe", 42,           probes[2]);
            await api.LogRemotePartyComment("probe",               probes[3]);

            var written = File.Exists(api.RemotePartyDBFileName)
                              ? await File.ReadAllTextAsync(api.RemotePartyDBFileName)
                              : "";

            Assert.Multiple(() => {
                Assert.That(written, Does.Contain(CommonHTTPAPI.addOrUpdateRemoteParty), "The remote party that was added or updated is not in its file yet.");
                Assert.That(written, Does.Contain(probes[0].ToString()),                 "A command without data is not in the file yet.");
                Assert.That(written, Does.Contain(probes[1].ToString()),                 "A command with a text is not in the file yet.");
                Assert.That(written, Does.Contain(probes[2].ToString()),                 "A command with a number is not in the file yet.");
                Assert.That(written, Does.Contain(probes[3].ToString()),                 "A comment is not in the file yet.");
            });

            // The queue is let finish before the directory is removed.
            for (var waited = 0; waited < 300 && LinesIn(queued) < 1000; waited++)
                await Task.Delay(100);

        }

        #endregion

        #region RemotePartiesAddedAtOnceAreAllInTheirFile()

        /// <summary>
        /// A hundred remote parties added at the same moment are all added, and
        /// all in their file when the calls have returned: one line after the
        /// other, none refused because another one was being written.
        /// </summary>
        [Test]
        public async Task RemotePartiesAddedAtOnceAreAllInTheirFile()
        {

            var partyIds  = Enumerable.Range(0, 100).Select(number => Party_Id.Parse($"{number:D3}")).ToArray();

            var added     = await Task.WhenAll(
                                      partyIds.Select(partyId => Task.Run(() => api.AddRemoteParty(countryCode, partyId, Role.CPO, business, AccessToken.NewRandom())))
                                  );

            var written   = await File.ReadAllLinesAsync(api.RemotePartyDBFileName);

            Assert.Multiple(() => {
                Assert.That(added.Where(result => !result.IsSuccess).Select(result => result.ErrorResponse), Is.Empty,
                            "Remote parties added at the same moment were refused.");
                Assert.That(written.Count(line => line.Contains(CommonHTTPAPI.addRemoteParty)),                Is.EqualTo(partyIds.Length),
                            "Not every remote party added at the same moment is in its file.");
            });

        }

        #endregion

        #region ARefusalOfTheFileIsSaidToBeTheFiles()

        /// <summary>
        /// Where the file refused a change, the result says so, and which file
        /// it was: a caller answers that differently from a change that was
        /// wrong in itself.
        /// </summary>
        [Test]
        public async Task ARefusalOfTheFileIsSaidToBeTheFiles()
        {

            var kept     = await api.AddRemoteParty(countryCode, Party_Id.Parse("CCC"), Role.CPO, business, AccessToken.NewRandom());

            Assert.That(kept.IsSuccess, Is.True, $"The remote party to remove could not be added: {kept.ErrorResponse}");

            BlockTheFile();

            var fileName = Path.GetFileName(api.RemotePartyDBFileName);
            var added    = await api.AddRemoteParty      (countryCode, partyId, Role.CPO, business, AccessToken.NewRandom());
            var byId     = await api.TryRemoveRemoteParty(kept.Data!.Id);
            var byParty  = await api.TryRemoveRemoteParty(kept.Data!);

            Assert.Multiple(() => {

                Assert.That(added.  IsSuccess,      Is.False);
                Assert.That(added.  NotSaved,       Is.True,                 "The file refused the remote party, and the result does not say so.");
                Assert.That(added.  ErrorResponse,  Does.Contain(fileName),  "The result does not name the file that refused the remote party.");

                Assert.That(byId.   IsSuccess,      Is.False);
                Assert.That(byId.   NotSaved,       Is.True,                 "The file refused the removal, and the result does not say so.");
                Assert.That(byId.   ErrorResponse,  Does.Contain(fileName),  "The result does not name the file that refused the removal.");

                Assert.That(byParty.IsSuccess,      Is.False);
                Assert.That(byParty.NotSaved,       Is.True,                 "The file refused the removal, and the result does not say so.");
                Assert.That(byParty.ErrorResponse,  Does.Contain(fileName),  "The result does not name the file that refused the removal.");

                Assert.That(api.RemoteParties.Select(party => party.Id), Does.Contain(kept.Data!.Id), "The remote party the file would not let go of is gone all the same.");

            });

        }

        #endregion

        #region ARefusalOfTheChangeIsNotSaidToBeTheFiles()

        /// <summary>
        /// A change that was refused for what it is - a remote party that is
        /// there already, or one nobody knows - is not said to be the file's.
        /// </summary>
        [Test]
        public async Task ARefusalOfTheChangeIsNotSaidToBeTheFiles()
        {

            var first    = await api.AddRemoteParty      (countryCode, partyId, Role.CPO, business, AccessToken.NewRandom());
            var again    = await api.AddRemoteParty      (countryCode, partyId, Role.CPO, business, AccessToken.NewRandom());
            var unknown  = await api.TryRemoveRemoteParty(RemoteParty_Id.Parse("DE-ZZZ_CPO"));

            Assert.Multiple(() => {
                Assert.That(first.  IsSuccess,  Is.True,   $"The remote party could not be added: {first.ErrorResponse}");
                Assert.That(again.  IsSuccess,  Is.False,  "The same remote party was added twice.");
                Assert.That(again.  NotSaved,   Is.False,  "A remote party that is there already was said to be refused by the file.");
                Assert.That(unknown.IsSuccess,  Is.False,  "A remote party nobody knows was removed.");
                Assert.That(unknown.NotSaved,   Is.False,  "A remote party nobody knows was said to be refused by the file.");
            });

        }

        #endregion

        #region ARemovalItsFileTakesIsMade()

        /// <summary>
        /// A remote party removed while its file can be written is gone, now
        /// and at the next start - by both ways that say what the file did.
        /// </summary>
        [Test]
        public async Task ARemovalItsFileTakesIsMade()
        {

            var other    = RemoteParty_Id.Parse("DE-CCC_CPO");

            var added    = await api.AddRemoteParty(countryCode, partyId,              Role.CPO, business, AccessToken.NewRandom());
            var added2   = await api.AddRemoteParty(countryCode, Party_Id.Parse("CCC"), Role.CPO, business, AccessToken.NewRandom());

            Assert.That(added.IsSuccess && added2.IsSuccess, Is.True, "The remote parties to remove could not be added.");

            var byId     = await api.TryRemoveRemoteParty(id);
            var byParty  = await api.TryRemoveRemoteParty(added2.Data!);

            Assert.Multiple(() => {
                Assert.That(byId.   IsSuccess,  Is.True,  $"The remote party could not be removed: {byId.ErrorResponse}");
                Assert.That(byId.   Data?.Id,   Is.EqualTo(id));
                Assert.That(byParty.IsSuccess,  Is.True,  $"The remote party could not be removed: {byParty.ErrorResponse}");
                Assert.That(byParty.Data?.Id,   Is.EqualTo(other));
                Assert.That(api.RemoteParties,  Is.Empty, "A removed remote party is still there.");
            });

            Assert.That(ACommonAPI().RemoteParties, Is.Empty, "The next start knows a remote party that was removed.");

        }

        #endregion


        #region AnAddIfNotExistsItsFileCannotTakeIsNotMade(Add)

        [TestCaseSource(nameof(AddsIfNotExists))]
        public async Task AnAddIfNotExistsItsFileCannotTakeIsNotMade(String Add)
        {

            BlockTheFile();

            var added = await addsIfNotExists[Add](api, theirToken);

            Assert.Multiple(() => {
                Assert.That(added.IsSuccess,                              Is.False,             "The file refused the remote party, and it was added all the same.");
                Assert.That(added.NotSaved,                               Is.True,              "The file refused the remote party, and the result does not say so.");
                Assert.That(added.ErrorResponse,                          Does.Contain(FileName), "The result does not name the file that refused the remote party.");
                Assert.That(api.RemoteParties.Select(party => party.Id),  Does.Not.Contain(id), "The remote party the file refused is kept all the same.");
            });

            UnblockTheFile();

            Assert.That(ACommonAPI().RemoteParties.Select(party => party.Id), Does.Not.Contain(id), "The next start knows the remote party its file refused.");

        }

        #endregion

        #region AnAddOrUpdateItsFileCannotTakeIsNotMade(AddOrUpdate)

        /// <summary>
        /// Where there is no remote party of its identification yet: it is not
        /// added.
        /// </summary>
        [TestCaseSource(nameof(AddsOrUpdates))]
        public async Task AnAddOrUpdateItsFileCannotTakeIsNotMade(String AddOrUpdate)
        {

            BlockTheFile();

            var added = await addsOrUpdates[AddOrUpdate](api, theirToken);

            Assert.Multiple(() => {
                Assert.That(added.IsSuccess,                              Is.False,             "The file refused the remote party, and it was added all the same.");
                Assert.That(added.NotSaved,                               Is.True,              "The file refused the remote party, and the result does not say so.");
                Assert.That(added.ErrorResponse,                          Does.Contain(FileName), "The result does not name the file that refused the remote party.");
                Assert.That(api.RemoteParties.Select(party => party.Id),  Does.Not.Contain(id), "The remote party the file refused is kept all the same.");
            });

            UnblockTheFile();

            Assert.That(ACommonAPI().RemoteParties.Select(party => party.Id), Does.Not.Contain(id), "The next start knows the remote party its file refused.");

        }

        #endregion

        #region AReplacementItsFileCannotTakeIsNotMade(AddOrUpdate)

        /// <summary>
        /// Where there is a remote party of its identification already: it
        /// stays as it was, its token with it.
        /// </summary>
        [TestCaseSource(nameof(AddsOrUpdates))]
        public async Task AReplacementItsFileCannotTakeIsNotMade(String AddOrUpdate)
        {

            var token     = AccessToken.NewRandom();
            var existing  = await AddAParty(partyId, token);

            Assert.That(existing.IsSuccess, Is.True, $"The remote party to replace could not be added: {existing.ErrorResponse}");

            BlockTheFile();

            var replaced  = await addsOrUpdates[AddOrUpdate](api, theirToken);

            Assert.Multiple(() => {
                Assert.That(replaced.IsSuccess,  Is.False,               "The file refused the replacement, and it was made all the same.");
                Assert.That(replaced.NotSaved,   Is.True,                "The file refused the replacement, and the result does not say so.");
                Assert.That(LocalTokensOf(api),  Is.EqualTo(new[] { token }), "The remote party the file would not let go of is replaced all the same.");
            });

            UnblockTheFile();

            Assert.That(LocalTokensOf(ACommonAPI()), Is.EqualTo(new[] { token }), "The next start knows the replacement its file refused.");

        }

        #endregion

        #region AnUpdateItsFileCannotTakeIsNotMade(Update)

        [TestCaseSource(nameof(Updates))]
        public async Task AnUpdateItsFileCannotTakeIsNotMade(String Update)
        {

            var token     = AccessToken.NewRandom();
            var existing  = await AddAParty(partyId, token);

            Assert.That(existing.IsSuccess, Is.True, $"The remote party to update could not be added: {existing.ErrorResponse}");

            BlockTheFile();

            var updated   = await updates[Update](api, existing.Data!, theirToken);

            Assert.Multiple(() => {
                Assert.That(updated.IsSuccess,       Is.False,               "The file refused the update, and it was made all the same.");
                Assert.That(updated.NotSaved,        Is.True,                "The file refused the update, and the result does not say so.");
                Assert.That(updated.ErrorResponse,   Does.Contain(FileName), "The result does not name the file that refused the update.");
                Assert.That(LocalTokensOf(api),      Is.EqualTo(new[] { token }), "The remote party the file would not let go of is updated all the same.");
            });

            UnblockTheFile();

            Assert.That(LocalTokensOf(ACommonAPI()), Is.EqualTo(new[] { token }), "The next start knows the update its file refused.");

        }

        #endregion

        #region ATokenTakenWhileItsFileCannotTakeItStays(Others)

        /// <summary>
        /// A token taken from a remote party that has no other, which removes
        /// the party, and from one that has another, which keeps it: neither
        /// is changed while the file cannot be written, and the token still
        /// opens what it opened.
        /// </summary>
        [TestCase(0)]
        [TestCase(1)]
        public async Task ATokenTakenWhileItsFileCannotTakeItStays(Int32 Others)
        {

            var token   = AccessToken.NewRandom();
            var tokens  = new[] { token }.Concat(Enumerable.Range(0, Others).Select(_ => AccessToken.NewRandom())).ToArray();
            var added   = await AddAParty(partyId, tokens);

            Assert.That(added.IsSuccess, Is.True, $"The remote party could not be added: {added.ErrorResponse}");

            BlockTheFile();

            var taken   = await api.TryRemoveAccessToken(token);

            Assert.Multiple(() => {
                Assert.That(taken.IsSuccess,      Is.False,               "The file refused the token's removal, and it was removed all the same.");
                Assert.That(taken.NotSaved,       Is.True,                "The file refused the token's removal, and the result does not say so.");
                Assert.That(taken.ErrorResponse,  Does.Contain(FileName), "The result does not name the file that refused the token's removal.");
                Assert.That(LocalTokensOf(api),   Is.EqualTo(tokens),     "The token the file would not let go of is gone all the same.");
            });

            UnblockTheFile();

            Assert.That(LocalTokensOf(ACommonAPI()), Is.EqualTo(tokens), "The next start has lost the token whose removal the file refused.");

        }

        #endregion

        #region ATokenTakenIsGoneAtTheNextStart(Others)

        /// <summary>
        /// A token taken while the file can be written is gone, now and at the
        /// next start: with the remote party that had no other, from the one
        /// that has another.
        /// </summary>
        [TestCase(0)]
        [TestCase(1)]
        public async Task ATokenTakenIsGoneAtTheNextStart(Int32 Others)
        {

            var token   = AccessToken.NewRandom();
            var others  = Enumerable.Range(0, Others).Select(_ => AccessToken.NewRandom()).ToArray();
            var added   = await AddAParty(partyId, [ token, .. others ]);

            Assert.That(added.IsSuccess, Is.True, $"The remote party could not be added: {added.ErrorResponse}");

            var taken   = await api.TryRemoveAccessToken(token);

            Assert.Multiple(() => {
                Assert.That(taken.IsSuccess,                    Is.True,           $"The token could not be removed: {taken.ErrorResponse}");
                Assert.That(taken.Data?.Select(party => party.Id), Is.EqualTo(new[] { added.Data!.Id }), "what the token was taken from");
                Assert.That(LocalTokensOf(api),                 Is.EqualTo(others), "The token taken is still there.");
            });

            Assert.That(LocalTokensOf(ACommonAPI()), Is.EqualTo(others), "The next start knows the token that was taken.");

        }

        #endregion

        #region EveryRemotePartyRemovedAtOnceWhileItsFileCannotTakeItIsBack()

        [Test]
        public async Task EveryRemotePartyRemovedAtOnceWhileItsFileCannotTakeItIsBack()
        {

            var first   = await AddAParty(partyId,              AccessToken.NewRandom());
            var second  = await AddAParty(Party_Id.Parse("CCC"), AccessToken.NewRandom());

            Assert.That(first.IsSuccess && second.IsSuccess, Is.True, "The remote parties to remove could not be added.");

            var ids     = new[] { first.Data!.Id, second.Data!.Id }.OrderBy(remotePartyId => remotePartyId.ToString()).ToArray();

            BlockTheFile();

            var removed = await api.RemoveAllRemoteParties();

            Assert.Multiple(() => {
                Assert.That(removed.IsSuccess,                                     Is.False,               "The file refused the removal of every remote party, and they were removed all the same.");
                Assert.That(removed.NotSaved,                                      Is.True,                "The file refused the removal of every remote party, and the result does not say so.");
                Assert.That(removed.ErrorResponse,                                 Does.Contain(FileName), "The result does not name the file that refused the removal.");
                Assert.That(api.RemoteParties.Select(party => party.Id).OrderBy(remotePartyId => remotePartyId.ToString()),  Is.EqualTo(ids),        "The remote parties the file would not let go of are gone all the same.");
            });

            UnblockTheFile();

            Assert.That(ACommonAPI().RemoteParties.Select(party => party.Id).OrderBy(remotePartyId => remotePartyId.ToString()), Is.EqualTo(ids), "The next start has lost the remote parties whose removal the file refused.");

        }

        #endregion


        #region ARefusedLineIsTold()

        /// <summary>
        /// Every line the file refuses is told to OnRemotePartyNotSaved: its
        /// command, the file and why - for whoever made the API to log.
        /// </summary>
        [Test]
        public async Task ARefusedLineIsTold()
        {

            var told = new List<(String Command, String FileName, Exception Exception)>();

            api.OnRemotePartyNotSaved += (timestamp, command, fileName, exception) => {
                told.Add((command, fileName, exception));
                return Task.CompletedTask;
            };

            BlockTheFile();

            var added = await AddAParty(partyId, AccessToken.NewRandom());

            Assert.Multiple(() => {
                Assert.That(added.NotSaved,                    Is.True,                                "The file refused the remote party, and the result does not say so.");
                Assert.That(told,                              Has.Count.EqualTo(1),                   "The line the file refused is not told once.");
                Assert.That(told.FirstOrDefault().Command,     Is.EqualTo(CommonHTTPAPI.addRemoteParty), "The command told is not the one refused.");
                Assert.That(told.FirstOrDefault().FileName,    Is.EqualTo(api.RemotePartyDBFileName),  "The file told is not the one that refused.");
                Assert.That(told.FirstOrDefault().Exception,   Is.Not.Null,                            "Why the file refused is not told.");
            });

        }

        #endregion

        #region AChangeKeptWhereItsFileRefusedItIsInEffect()

        /// <summary>
        /// A change kept where its file refused it - a registration the other
        /// side has answered - is in effect, and said to be the file's.
        /// </summary>
        [Test]
        public async Task AChangeKeptWhereItsFileRefusedItIsInEffect()
        {

            var token = AccessToken.NewRandom();

            BlockTheFile();

            var kept = await Keep(token);

            Assert.Multiple(() => {
                Assert.That(kept.IsSuccess,            Is.True,                      $"The change to keep was not made: {kept.ErrorResponse}");
                Assert.That(kept.NotSaved,             Is.True,                      "The file refused the change, and the result does not say so.");
                Assert.That(kept.ErrorResponse,        Does.Contain(FileName),       "The result does not name the file that refused the change.");
                Assert.That(LocalTokensOf(api),        Is.EqualTo(new[] { token }),  "The change kept is not in effect.");
                Assert.That(api.UnsavedRemoteParties,  Is.EqualTo(new[] { id }),     "The remote party kept is not said to be unsaved.");
            });

        }

        #endregion

        #region AKeptChangeIsWrittenDownWithTheNextLineTheFileTakes()

        /// <summary>
        /// Once the file takes a line again - of any remote party - the change
        /// kept goes in after it.
        /// </summary>
        [Test]
        public async Task AKeptChangeIsWrittenDownWithTheNextLineTheFileTakes()
        {

            var token = AccessToken.NewRandom();

            BlockTheFile();
            await Keep(token);
            UnblockTheFile();

            var other = await AddAParty(Party_Id.Parse("CCC"), AccessToken.NewRandom());

            Assert.Multiple(() => {
                Assert.That(other.IsSuccess,           Is.True,   $"The next change could not be made: {other.ErrorResponse}");
                Assert.That(api.UnsavedRemoteParties,  Is.Empty,  "The remote party kept is still said to be unsaved.");
            });

            Assert.That(LocalTokensOf(ACommonAPI()), Is.EqualTo(new[] { token }), "The next start does not know the change kept, though the file took a line after it.");

        }

        #endregion

        #region AKeptChangeIsWrittenDownWhenTheBaseAPIIsDisposed()

        /// <summary>
        /// A change kept is written down when the base API is disposed, where
        /// the file takes it by then.
        /// </summary>
        [Test]
        public async Task AKeptChangeIsWrittenDownWhenTheBaseAPIIsDisposed()
        {

            var token = AccessToken.NewRandom();

            BlockTheFile();
            await Keep(token);
            UnblockTheFile();

            await api.BaseAPI.DisposeAsync();

            Assert.That(LocalTokensOf(ACommonAPI()), Is.EqualTo(new[] { token }), "The next start does not know the change kept, though the file took lines when the API was disposed.");

        }

        #endregion

        #region AKeptChangeIsWrittenDownWhenAsked()

        /// <summary>
        /// Asked to write down what was kept: refused and still unsaved while
        /// the file refuses, written down once it takes it.
        /// </summary>
        [Test]
        public async Task AKeptChangeIsWrittenDownWhenAsked()
        {

            var token = AccessToken.NewRandom();

            BlockTheFile();
            await Keep(token);

            var refused = await api.WriteDownUnsavedRemoteParties();

            Assert.Multiple(() => {
                Assert.That(refused,                   Does.Contain(FileName),    "Writing down what was kept does not say which file still refuses it.");
                Assert.That(api.UnsavedRemoteParties,  Is.EqualTo(new[] { id }),  "The remote party kept is no longer said to be unsaved, and its file still refuses it.");
            });

            UnblockTheFile();

            var written = await api.WriteDownUnsavedRemoteParties();

            Assert.Multiple(() => {
                Assert.That(written,                   Is.Null,   $"Writing down what was kept failed, though its file takes it again: {written}");
                Assert.That(api.UnsavedRemoteParties,  Is.Empty,  "The remote party kept is still said to be unsaved.");
            });

            Assert.That(LocalTokensOf(ACommonAPI()), Is.EqualTo(new[] { token }), "The next start does not know the change kept and written down.");

        }

        #endregion

        #region AKeptRemotePartyChangedSinceIsWrittenDownAsItIsNow()

        /// <summary>
        /// A remote party kept and changed since is written down as it is now:
        /// the change after it is what the next start is to know.
        /// </summary>
        [Test]
        public async Task AKeptRemotePartyChangedSinceIsWrittenDownAsItIsNow()
        {

            var kept     = AccessToken.NewRandom();
            var changed  = AccessToken.NewRandom();

            BlockTheFile();
            await Keep(kept);
            UnblockTheFile();

            var replaced = await Replace(changed);

            Assert.That(replaced.IsSuccess, Is.True, $"The remote party kept could not be changed: {replaced.ErrorResponse}");

            await api.BaseAPI.DisposeAsync();

            Assert.That(LocalTokensOf(ACommonAPI()), Is.EqualTo(new[] { changed }), "The next start knows the remote party as it was kept, not as it was changed since.");

        }

        #endregion

        #region AKeptRemotePartyRemovedSinceStaysRemoved()

        /// <summary>
        /// A remote party kept and removed since is not written down again: it
        /// stays removed, at the next start as well.
        /// </summary>
        [Test]
        public async Task AKeptRemotePartyRemovedSinceStaysRemoved()
        {

            BlockTheFile();
            await Keep(AccessToken.NewRandom());
            UnblockTheFile();

            var removed = await api.TryRemoveRemoteParty(id);

            Assert.Multiple(() => {
                Assert.That(removed.IsSuccess,         Is.True,   $"The remote party kept could not be removed: {removed.ErrorResponse}");
                Assert.That(api.UnsavedRemoteParties,  Is.Empty,  "The remote party removed is still said to be unsaved.");
            });

            await api.BaseAPI.DisposeAsync();

            Assert.That(ACommonAPI().RemoteParties.Select(party => party.Id), Does.Not.Contain(id), "The remote party removed after it was kept is back at the next start.");

        }

        #endregion


        #region (private) FileName

        /// <summary>
        /// The name of the file of the remote parties, which a refusal names.
        /// </summary>
        private String FileName
            => Path.GetFileName(api.RemotePartyDBFileName);

        #endregion

        #region (private) AddAParty(PartyId, params LocalTokens)

        /// <summary>
        /// A remote party of the given party identification, with the given
        /// local tokens and no remote access.
        /// </summary>
        private Task<AddResult<RemoteParty>> AddAParty(Party_Id PartyId, params AccessToken[] LocalTokens)

            => api.AddRemoteParty(countryCode, PartyId, Role.CPO, business,
                                  [ .. LocalTokens.Select(token => new LocalAccessInfo(token)) ],
                                  []);

        #endregion

        #region (private) Keep(LocalToken) / Replace(LocalToken)

        /// <summary>
        /// The remote party of this test's identification, with the given local
        /// token and remote access, as a registration the other side has
        /// answered leaves it: kept where its file refuses it.
        /// </summary>
        private Task<AddOrUpdateResult<RemoteParty>> Keep(AccessToken LocalToken)

            => api.AddOrUpdateRemoteParty(countryCode, partyId, Role.CPO, business, LocalToken, theirURL, theirToken, KeepWhereNotSaved: true);

        /// <summary>
        /// The same, taken back where its file refuses it.
        /// </summary>
        private Task<AddOrUpdateResult<RemoteParty>> Replace(AccessToken LocalToken)

            => api.AddOrUpdateRemoteParty(countryCode, partyId, Role.CPO, business, LocalToken, theirURL, theirToken);

        #endregion

        #region (private static) LocalTokensOf(API)

        /// <summary>
        /// The local tokens of the remote party of this test's identification.
        /// </summary>
        private static AccessToken[] LocalTokensOf(CommonAPI API)

            => API.RemoteParties.
                   Where     (party => party.Id == id).
                   SelectMany(party => party.LocalAccessInfos.Select(info => info.AccessToken)).
                   ToArray();

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

        #region (private static) LinesIn(FileName)

        /// <summary>
        /// The lines of a file the queue may be writing to at this moment: none
        /// where it is not there yet.
        /// </summary>
        private static Int32 LinesIn(String FileName)
        {

            try
            {

                using var stream = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
                using var reader = new StreamReader(stream);

                var lines = 0;

                while (reader.ReadLine() is not null)
                    lines++;

                return lines;

            }
            catch (IOException)
            {
                return 0;
            }

        }

        #endregion

        #region (private) BlockTheFile() / UnblockTheFile()

        /// <summary>
        /// Make the file of the remote parties unwritable: a directory where it
        /// is, which stops root as well. What it held is put aside.
        /// </summary>
        private void BlockTheFile()
        {

            if (File.Exists(api.RemotePartyDBFileName))
                File.Move(api.RemotePartyDBFileName, api.RemotePartyDBFileName + ".aside");

            Directory.CreateDirectory(api.RemotePartyDBFileName);

        }

        /// <summary>
        /// Give the file of the remote parties back what it held.
        /// </summary>
        private void UnblockTheFile()
        {

            Directory.Delete(api.RemotePartyDBFileName);

            if (File.Exists(api.RemotePartyDBFileName + ".aside"))
                File.Move(api.RemotePartyDBFileName + ".aside", api.RemotePartyDBFileName);

        }

        #endregion

    }

}
