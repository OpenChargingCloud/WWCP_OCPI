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

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv3_0.UnitTests.Datastructures
{

    /// <summary>
    /// An optional value that is there but not valid is refused, and one that
    /// is not there is no reason to refuse.
    /// </summary>
    /// <remarks>
    /// <para>
    /// These parsers asked for their optional value with a negation: that an
    /// optional value is not valid is said only while it is there, and the
    /// negation looked for it only while it was not. A value not valid was
    /// passed over, and the rest read as if it were not there.
    /// </para>
    /// <para>
    /// EVSEStatus gave up on a status without a timestamp, which is optional,
    /// and said nothing - and read one with a timestamp not valid.
    /// </para>
    /// </remarks>
    [TestFixture]
    public static class OptionalValueTests
    {

        #region EVSEStatus: timestamp

        [Test]
        public static void AnEVSEStatusWithATimestampNotValidIsRefused()

            => AssertRefused(
                   new EVSEStatus(
                       EVSE_UId.Parse("EVSE0001"),
                       StatusType.AVAILABLE,
                       new DateTimeOffset(2026, 10, 1, 8, 0, 0, TimeSpan.Zero)
                   ).ToJSON(),
                   "timestamp",
                   "not a time",
                   (JObject json, out String? error) => EVSEStatus.TryParse(json, out _, out error)
               );

        #endregion

        #region PlatformParties: hub_party_id

        [Test]
        public static void PlatformPartiesWithAHubPartyIdNotValidAreRefused()

            => AssertRefused(
                   SomePlatformParties(),
                   "hub_party_id",
                   "not a party",
                   (JObject json, out String? error) => PlatformParties.TryParse(json, out _, out error)
               );

        #endregion

        #region PlatformParties: parties

        [Test]
        public static void PlatformPartiesWithPartiesNotValidAreRefused()

            => AssertRefused(
                   SomePlatformParties(),
                   "parties",
                   "not a list of parties",
                   (JObject json, out String? error) => PlatformParties.TryParse(json, out _, out error)
               );

        #endregion

        #region SubscriptionParameterProposal: parallelism_limit

        [Test]
        public static void ASubscriptionParameterProposalWithAParallelismLimitNotValidIsRefused()

            => AssertRefused(
                   new SubscriptionParameterProposal(
                       TimeSpan.FromSeconds(30),
                       100
                   ).ToJSON(),
                   "parallelism_limit",
                   "not a number",
                   (JObject json, out String? error) => SubscriptionParameterProposal.TryParse(json, out _, out error)
               );

        #endregion

        #region SubscriptionRequest: parallelism_limit

        [Test]
        public static void ASubscriptionRequestWithAParallelismLimitNotValidIsRefused()

            => AssertRefused(
                   new SubscriptionRequest(
                       TimeSpan.FromSeconds(30),
                       100
                   ).ToJSON(),
                   "parallelism_limit",
                   "not a number",
                   (JObject json, out String? error) => SubscriptionRequest.TryParse(json, out _, out error)
               );

        #endregion


        #region (private static) SomePlatformParties()

        /// <summary>
        /// The parties of a platform with its hub, as JSON.
        /// </summary>
        private static JObject SomePlatformParties()

            => new PlatformParties(
                   Party_Idv3.Parse("DEHUB"),
                   [
                       new PlatformParty(
                           Party_Idv3.Parse("DEGEF"),
                           new BusinessDetails("GraphDefined CSO"),
                           [ new PartyRole(Module_Id.CDRs, InterfaceRoles.SENDER) ]
                       )
                   ]
               ).ToJSON();

        #endregion

        #region (private) AssertRefused(JSON, PropertyName, NotValid, TryParse)

        private delegate Boolean TryParser(JObject JSON, out String? ErrorResponse);

        /// <summary>
        /// The given JSON, without the optional property, is read; with a value
        /// not valid for it, it is refused, and said why.
        /// </summary>
        private static void AssertRefused(JObject    JSON,
                                          String     PropertyName,
                                          JToken     NotValid,
                                          TryParser  TryParse)
        {

            JSON.Remove(PropertyName);

            Assert.That(TryParse(JSON, out var error), Is.True, $"Without '{PropertyName}' it is not read: {error}");

            JSON[PropertyName] = NotValid;

            var parsed = TryParse(JSON, out error);

            Assert.Multiple(() => {
                Assert.That(parsed, Is.False,   $"With '{PropertyName}' not valid it is read.");
                Assert.That(error,  Is.Not.Null, $"With '{PropertyName}' not valid it is refused without a reason.");
            });

        }

        #endregion

    }

}
