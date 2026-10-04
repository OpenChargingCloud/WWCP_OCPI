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
            Assert.Throws<ArgumentException>    (() => Location_Id.Parse(""));
        }

        [Test]
        public void Parse_Whitespace()
        {
            Assert.Throws<ArgumentException>    (() => Location_Id.Parse("   "));
        }

        [Test]
        public void TryParse_Null()
        {

            Assert.That(Location_Id.TryParse(null),          Is.Null);
            Assert.That(Location_Id.TryParse(null).HasValue, Is.False);

            Assert.That(Location_Id.TryParse(null, out Location_Id LocationId), Is.False);
            Assert.That(LocationId.IsNullOrEmpty,                               Is.True);
            Assert.That(LocationId.Length,                                      Is.EqualTo(0));
            Assert.That(LocationId.ToString(),                                  Is.EqualTo(""));

        }

        [Test]
        public void TryParse_Empty()
        {

            Assert.That(Location_Id.TryParse(""),          Is.Null);
            Assert.That(Location_Id.TryParse("").HasValue, Is.False);

            Assert.That(Location_Id.TryParse("", out Location_Id LocationId), Is.False);
            Assert.That(LocationId.IsNullOrEmpty,                             Is.True);
            Assert.That(LocationId.Length,                                    Is.EqualTo(0));
            Assert.That(LocationId.ToString(),                                Is.EqualTo(""));

        }

        [Test]
        public void TryParse_Whitespace()
        {

            Assert.That(Location_Id.TryParse("   "),          Is.Null);
            Assert.That(Location_Id.TryParse("   ").HasValue, Is.False);

            Assert.That(Location_Id.TryParse("   ", out Location_Id LocationId), Is.False);
            Assert.That(LocationId.IsNullOrEmpty,                                Is.True);
            Assert.That(LocationId.Length,                                       Is.EqualTo(0));
            Assert.That(LocationId.ToString(),                                   Is.EqualTo(""));

        }

        [Test]
        public void Length()
        {
            Assert.That(Location_Id.Parse("abc").Length, Is.EqualTo(3));
        }

        [Test]
        public void Equality()
        {

            var abc = Location_Id.Parse("abc");

            Assert.That(abc,                     Is.EqualTo(Location_Id.Parse("abc")));
            Assert.That(Location_Id.Parse("aBc"), Is.EqualTo(abc));

            Assert.That(Location_Id.Parse("abc").Equals(Location_Id.Parse("abc")), Is.True);
            Assert.That(Location_Id.Parse("abc").Equals(Location_Id.Parse("aBc")), Is.True);

        }

        [Test]
        public void OperatorEquality()
        {
            Assert.That(Location_Id.Parse("abc") == Location_Id.Parse("abc"), Is.True);
            Assert.That(Location_Id.Parse("abc") == Location_Id.Parse("aBc"), Is.True);
        }

        [Test]
        public void OperatorInequality()
        {
            Assert.That(Location_Id.Parse("abc") != Location_Id.Parse("abc"), Is.False);
            Assert.That(Location_Id.Parse("abc") != Location_Id.Parse("aBc"), Is.False);
        }

        [Test]
        public void OperatorSmaller()
        {
            Assert.That(Location_Id.Parse("abc") < Location_Id.Parse("abc"),  Is.False);
            Assert.That(Location_Id.Parse("abc") < Location_Id.Parse("aBc"),  Is.False);
            Assert.That(Location_Id.Parse("abc") < Location_Id.Parse("abc2"), Is.True);
        }

        [Test]
        public void OperatorSmallerOrEquals()
        {
            Assert.That(Location_Id.Parse("abc") <= Location_Id.Parse("abc"),  Is.True);
            Assert.That(Location_Id.Parse("abc") <= Location_Id.Parse("aBc"),  Is.True);
            Assert.That(Location_Id.Parse("abc") <= Location_Id.Parse("abc2"), Is.True);
        }

        [Test]
        public void OperatorBigger()
        {
            Assert.That(Location_Id.Parse("abc")  > Location_Id.Parse("abc"), Is.False);
            Assert.That(Location_Id.Parse("abc")  > Location_Id.Parse("aBc"), Is.False);
            Assert.That(Location_Id.Parse("abc2") > Location_Id.Parse("abc"), Is.True);
        }

        [Test]
        public void OperatorBiggerOrEquals()
        {
            Assert.That(Location_Id.Parse("abc")  >= Location_Id.Parse("abc"), Is.True);
            Assert.That(Location_Id.Parse("abc")  >= Location_Id.Parse("aBc"), Is.True);
            Assert.That(Location_Id.Parse("abc2") >= Location_Id.Parse("abc"), Is.True);
        }

        [Test]
        public void HashCodeEquality()
        {
            Assert.That(Location_Id.Parse("abc"). GetHashCode(), Is.EqualTo(Location_Id.Parse("abc").GetHashCode()));
            Assert.That(Location_Id.Parse("aBc"). GetHashCode(), Is.EqualTo(Location_Id.Parse("abc").GetHashCode()));
            Assert.That(Location_Id.Parse("abc2").GetHashCode(), Is.Not.EqualTo(Location_Id.Parse("abc").GetHashCode()));
        }

        [Test]
        public void DifferentCases_DictionaryKeyEquality()
        {

            var Lookup = new Dictionary<Location_Id, String> {
                             { Location_Id.Parse("abc01"), "DifferentCases_DictionaryKeyEquality()" }
                         };

            Assert.That(Lookup.ContainsKey(Location_Id.Parse("abc01")), Is.True);
            Assert.That(Lookup.ContainsKey(Location_Id.Parse("aBc01")), Is.True);

        }

        [Test]
        public void Test_ToString()
        {
            Assert.That(Location_Id.Parse("abc").ToString(), Is.EqualTo("abc"));
            Assert.That(Location_Id.Parse("aBc").ToString(), Is.EqualTo("aBc"));
        }

    }

}
