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

using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_3_0.UnitTests.Datastructures
{

    /// <summary>
    /// An optional value that is there but not valid is refused, and one that
    /// is not there is no reason to refuse.
    /// </summary>
    /// <remarks>
    /// These parsers asked for their optional value with a negation: that an
    /// optional value is not valid is said only while it is there, and the
    /// negation looked for it only while it was not. A value not valid was
    /// passed over, and the rest read as if it were not there.
    /// </remarks>
    [TestFixture]
    public static class OptionalValueTests
    {

        #region NotifyWebPaymentsFailedCommand: error_message

        [Test]
        public static void ANotifyWebPaymentsFailedCommandWithAnErrorMessageNotValidIsRefused()

            => AssertRefused(
                   new NotifyWebPaymentsFailedCommand(
                       URL.        Parse("https://emsp.example/ocpi/commands/1"),
                       Location_Id.Parse("LOCATION0001"),
                       EVSE_UId.   Parse("EVSE0001")
                   ).ToJSON(),
                   "error_message",
                   "not a list of texts",
                   (JObject json, out String? error) => NotifyWebPaymentsFailedCommand.TryParse(json, out _, out error)
               );

        #endregion

        #region Calendar: step_size

        [Test]
        public static void ACalendarWithAStepSizeNotValidIsRefused()

            => AssertRefused(
                   new Calendar(
                       Calendar_Id.Parse("CALENDAR0001"),
                       new DateTimeOffset(2026, 10, 1,  8, 0, 0, TimeSpan.Zero),
                       new DateTimeOffset(2026, 10, 1, 18, 0, 0, TimeSpan.Zero),
                       [
                           new TimeSlot(
                               new DateTimeOffset(2026, 10, 1,  8,  0, 0, TimeSpan.Zero),
                               new DateTimeOffset(2026, 10, 1,  8, 30, 0, TimeSpan.Zero)
                           )
                       ],
                       new DateTimeOffset(2026, 10, 1,  8, 0, 0, TimeSpan.Zero)
                   ).ToJSON(),
                   "step_size",
                   "not a number",
                   (JObject json, out String? error) => Calendar.TryParse(json, out _, out error)
               );

        #endregion

        #region AccessMethod: value

        [Test]
        public static void AnAccessMethodWithAValueNotValidIsRefused()

            => AssertRefused(
                   new AccessMethod(
                       LocationAccess.OPEN
                   ).ToJSON(),
                   "value",
                   new JArray("not a text"),
                   (JObject json, out String? error) => AccessMethod.TryParse(json, out _, out error)
               );

        #endregion

        #region Bookable: ad_hoc

        [Test]
        public static void ABookableWithAnAdHocNotValidIsRefused()

            => AssertRefused(
                   new Bookable(
                       true
                   ).ToJSON(),
                   "ad_hoc",
                   "not a number",
                   (JObject json, out String? error) => Bookable.TryParse(json, out _, out error)
               );

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
