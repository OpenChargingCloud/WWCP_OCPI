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

namespace cloud.charging.open.protocols.OCPIv2_2_1.UnitTests.Datastructures
{

    /// <summary>
    /// Objects whose last optional part is missing have hash codes of their
    /// own.
    /// </summary>
    /// <remarks>
    /// That part's hash code was combined as "^ x?.GetHashCode() ?? 0", and
    /// "??" binds after "^": a missing part made the whole combination null,
    /// and the hash code 0 - the same for every such object. Equal objects
    /// still had equal hash codes, so nothing was wrong, but every set of
    /// them was a list. A few of them stand for all.
    /// </remarks>
    [TestFixture]
    public static class HashCodeTests
    {

        #region PricesWithoutVATIncludedDoNotShareOneHashCode()

        [Test]
        public static void PricesWithoutVATIncludedDoNotShareOneHashCode()

            => AssertOwnHashCodes(
                   number => new Price(number)
               );

        #endregion


        #region (private static) AssertOwnHashCodes(Make)

        /// <summary>
        /// Three objects made from 1, 2 and 3 have three hash codes, and the
        /// one made from 1 again has the hash code of the first.
        /// </summary>
        private static void AssertOwnHashCodes<T>(Func<Int32, T> Make)
            where T : notnull
        {

            var hashCodes = new[] { 1, 2, 3 }.Select(number => Make(number).GetHashCode()).ToArray();

            Assert.Multiple(() => {

                Assert.That(hashCodes.Distinct().Count(), Is.EqualTo(3),
                            $"Three different objects of {typeof(T).Name} share a hash code: {String.Join(", ", hashCodes)}");

                Assert.That(Make(1).GetHashCode(), Is.EqualTo(hashCodes[0]),
                            $"Two equal objects of {typeof(T).Name} have different hash codes.");

            });

        }

        #endregion

    }

}
