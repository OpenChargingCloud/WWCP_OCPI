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

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_3_0.UnitTests.Datastructures
{

    /// <summary>
    /// Unit tests for booking tokens.
    /// </summary>
    [TestFixture]
    public static class BookingTokenTests
    {

        #region BookingTokensWithoutALicensePlateDoNotShareOneHashCode()

        /// <summary>
        /// Booking tokens without a license plate have hash codes of their
        /// own. The missing plate's 0 was put in after the other parts were
        /// combined, not as one of them, and so made every such hash code 0.
        /// </summary>
        [Test]
        public static void BookingTokensWithoutALicensePlateDoNotShareOneHashCode()
        {

            var hashCodes = new[] { "TOKEN0001", "TOKEN0002", "TOKEN0003" }.
                                Select(uid => ABookingToken(uid).GetHashCode()).
                                ToArray();

            Assert.Multiple(() => {

                Assert.That(hashCodes.Distinct().Count(), Is.EqualTo(3),
                            $"Booking tokens without a license plate share a hash code: {String.Join(", ", hashCodes)}");

                Assert.That(ABookingToken("TOKEN0001").GetHashCode(), Is.EqualTo(hashCodes[0]),
                            "Two equal booking tokens have different hash codes.");

            });

        }

        #endregion


        #region (private static) ABookingToken(UID)

        /// <summary>
        /// A booking token without a license plate.
        /// </summary>
        private static BookingToken ABookingToken(String UID)

            => new (
                   CountryCode.Parse("DE"),
                   Party_Id.   Parse("GEF"),
                   Token_Id.   Parse(UID),
                   TokenType.  RFID,
                   Contract_Id.Parse("DE-GEF-C12345678-X")
               );

        #endregion

    }

}
