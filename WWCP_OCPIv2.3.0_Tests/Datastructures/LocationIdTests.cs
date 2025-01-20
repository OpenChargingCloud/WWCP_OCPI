/*
 * Copyright (c) 2015-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
using NUnit.Framework.Legacy;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_3_0.UnitTests.Datastructures
{

    /// <summary>
    /// Location identification tests.
    /// </summary>
    [TestFixture]
    public class LocationIdTests
    {

        [Test]
        public void Parse_Null()
        {
            Assert.Throws<ArgumentNullException>(() => Location_Id.Parse(null));
        }

        [Test]
        public void Parse_Empty()
        {
            Assert.Throws<ArgumentNullException>(() => Location_Id.Parse(""));
        }

        [Test]
        public void Parse_Whitespace()
        {
            Assert.Throws<ArgumentNullException>(() => Location_Id.Parse("   "));
        }

        [Test]
        public void TryParse_Null()
        {

            ClassicAssert.IsNull  (Location_Id.TryParse(null));
            ClassicAssert.IsFalse (Location_Id.TryParse(null).HasValue);

            ClassicAssert.IsFalse (Location_Id.TryParse(null, out Location_Id LocationId));
            ClassicAssert.IsTrue  (LocationId.IsNullOrEmpty);
            ClassicAssert.AreEqual(0,  LocationId.Length);
            ClassicAssert.AreEqual("", LocationId.ToString());

        }

        [Test]
        public void TryParse_Empty()
        {

            ClassicAssert.IsNull  (Location_Id.TryParse(""));
            ClassicAssert.IsFalse (Location_Id.TryParse("").HasValue);

            ClassicAssert.IsFalse (Location_Id.TryParse("", out Location_Id LocationId));
            ClassicAssert.IsTrue  (LocationId.IsNullOrEmpty);
            ClassicAssert.AreEqual(0,  LocationId.Length);
            ClassicAssert.AreEqual("", LocationId.ToString());

        }

        [Test]
        public void TryParse_Whitespace()
        {

            ClassicAssert.IsNull  (Location_Id.TryParse("   "));
            ClassicAssert.IsFalse (Location_Id.TryParse("   ").HasValue);

            ClassicAssert.IsFalse (Location_Id.TryParse("   ", out Location_Id LocationId));
            ClassicAssert.IsTrue  (LocationId.IsNullOrEmpty);
            ClassicAssert.AreEqual(0,  LocationId.Length);
            ClassicAssert.AreEqual("", LocationId.ToString());

        }

        [Test]
        public void Length()
        {
            ClassicAssert.AreEqual(3, Location_Id.Parse("abc").Length);
        }

        [Test]
        public void Equality()
        {

            ClassicAssert.AreEqual(Location_Id.Parse("abc"), Location_Id.Parse("abc"));
            ClassicAssert.AreEqual(Location_Id.Parse("abc"), Location_Id.Parse("aBc"));

            ClassicAssert.IsTrue  (Location_Id.Parse("abc").Equals(Location_Id.Parse("abc")));
            ClassicAssert.IsTrue  (Location_Id.Parse("abc").Equals(Location_Id.Parse("aBc")));

        }

        [Test]
        public void OperatorEquality()
        {
            ClassicAssert.IsTrue(Location_Id.Parse("abc") == Location_Id.Parse("abc"));
            ClassicAssert.IsTrue(Location_Id.Parse("abc") == Location_Id.Parse("aBc"));
        }

        [Test]
        public void OperatorInequality()
        {
            ClassicAssert.IsFalse(Location_Id.Parse("abc") != Location_Id.Parse("abc"));
            ClassicAssert.IsFalse(Location_Id.Parse("abc") != Location_Id.Parse("aBc"));
        }

        [Test]
        public void OperatorSmaller()
        {
            ClassicAssert.IsFalse(Location_Id.Parse("abc") < Location_Id.Parse("abc"));
            ClassicAssert.IsFalse(Location_Id.Parse("abc") < Location_Id.Parse("aBc"));
            ClassicAssert.IsTrue (Location_Id.Parse("abc") < Location_Id.Parse("abc2"));
        }

        [Test]
        public void OperatorSmallerOrEquals()
        {
            ClassicAssert.IsTrue(Location_Id.Parse("abc") <= Location_Id.Parse("abc"));
            ClassicAssert.IsTrue(Location_Id.Parse("abc") <= Location_Id.Parse("aBc"));
            ClassicAssert.IsTrue(Location_Id.Parse("abc") <= Location_Id.Parse("abc2"));
        }

        [Test]
        public void OperatorBigger()
        {
            ClassicAssert.IsFalse(Location_Id.Parse("abc")  > Location_Id.Parse("abc"));
            ClassicAssert.IsFalse(Location_Id.Parse("abc")  > Location_Id.Parse("aBc"));
            ClassicAssert.IsTrue (Location_Id.Parse("abc2") > Location_Id.Parse("abc"));
        }

        [Test]
        public void OperatorBiggerOrEquals()
        {
            ClassicAssert.IsTrue(Location_Id.Parse("abc")  >= Location_Id.Parse("abc"));
            ClassicAssert.IsTrue(Location_Id.Parse("abc")  >= Location_Id.Parse("aBc"));
            ClassicAssert.IsTrue(Location_Id.Parse("abc2") >= Location_Id.Parse("abc"));
        }

        [Test]
        public void HashCodeEquality()
        {
            ClassicAssert.AreEqual   (Location_Id.Parse("abc").GetHashCode(), Location_Id.Parse("abc"). GetHashCode());
            ClassicAssert.AreEqual   (Location_Id.Parse("abc").GetHashCode(), Location_Id.Parse("aBc"). GetHashCode());
            ClassicAssert.AreNotEqual(Location_Id.Parse("abc").GetHashCode(), Location_Id.Parse("abc2").GetHashCode());
        }

        [Test]
        public void DifferentCases_DictionaryKeyEquality()
        {

            var Lookup = new Dictionary<Location_Id, String> {
                             { Location_Id.Parse("abc01"), "DifferentCases_DictionaryKeyEquality()" }
                         };

            ClassicAssert.IsTrue(Lookup.ContainsKey(Location_Id.Parse("abc01")));
            ClassicAssert.IsTrue(Lookup.ContainsKey(Location_Id.Parse("aBc01")));

        }

        [Test]
        public void Test_ToString()
        {
            ClassicAssert.AreEqual("abc", Location_Id.Parse("abc").ToString());
            ClassicAssert.AreEqual("aBc", Location_Id.Parse("aBc").ToString());
        }

    }

}
