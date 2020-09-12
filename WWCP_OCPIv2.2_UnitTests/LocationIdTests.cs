/*
 * Copyright (c) 2015-2020 GraphDefined GmbH
 * This file is part of WWCP OCPI <https://github.com/OpenChargingCloud/WWCP_OCPI>
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#region Usings

using System;
using System.Collections.Generic;

using NUnit.Framework;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2.tests
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

            Assert.IsNull  (Location_Id.TryParse(null));
            Assert.IsFalse (Location_Id.TryParse(null).HasValue);

            Assert.IsFalse (Location_Id.TryParse(null, out Location_Id LocationId));
            Assert.IsTrue  (LocationId.IsNullOrEmpty);
            Assert.AreEqual(0,  LocationId.Length);
            Assert.AreEqual("", LocationId.ToString());

        }

        [Test]
        public void TryParse_Empty()
        {

            Assert.IsNull  (Location_Id.TryParse(""));
            Assert.IsFalse (Location_Id.TryParse("").HasValue);

            Assert.IsFalse (Location_Id.TryParse("", out Location_Id LocationId));
            Assert.IsTrue  (LocationId.IsNullOrEmpty);
            Assert.AreEqual(0,  LocationId.Length);
            Assert.AreEqual("", LocationId.ToString());

        }

        [Test]
        public void TryParse_Whitespace()
        {

            Assert.IsNull  (Location_Id.TryParse("   "));
            Assert.IsFalse (Location_Id.TryParse("   ").HasValue);

            Assert.IsFalse (Location_Id.TryParse("   ", out Location_Id LocationId));
            Assert.IsTrue  (LocationId.IsNullOrEmpty);
            Assert.AreEqual(0,  LocationId.Length);
            Assert.AreEqual("", LocationId.ToString());

        }

        [Test]
        public void Length()
        {
            Assert.AreEqual(3, Location_Id.Parse("abc").Length);
        }

        [Test]
        public void Equality()
        {

            Assert.AreEqual(Location_Id.Parse("abc"), Location_Id.Parse("abc"));
            Assert.AreEqual(Location_Id.Parse("abc"), Location_Id.Parse("aBc"));

            Assert.IsTrue  (Location_Id.Parse("abc").Equals(Location_Id.Parse("abc")));
            Assert.IsTrue  (Location_Id.Parse("abc").Equals(Location_Id.Parse("aBc")));

        }

        [Test]
        public void OperatorEquality()
        {
            Assert.IsTrue(Location_Id.Parse("abc") == Location_Id.Parse("abc"));
            Assert.IsTrue(Location_Id.Parse("abc") == Location_Id.Parse("aBc"));
        }

        [Test]
        public void OperatorInequality()
        {
            Assert.IsFalse(Location_Id.Parse("abc") != Location_Id.Parse("abc"));
            Assert.IsFalse(Location_Id.Parse("abc") != Location_Id.Parse("aBc"));
        }

        [Test]
        public void OperatorSmaller()
        {
            Assert.IsFalse(Location_Id.Parse("abc") < Location_Id.Parse("abc"));
            Assert.IsFalse(Location_Id.Parse("abc") < Location_Id.Parse("aBc"));
            Assert.IsTrue (Location_Id.Parse("abc") < Location_Id.Parse("abc2"));
        }

        [Test]
        public void OperatorSmallerOrEquals()
        {
            Assert.IsTrue(Location_Id.Parse("abc") <= Location_Id.Parse("abc"));
            Assert.IsTrue(Location_Id.Parse("abc") <= Location_Id.Parse("aBc"));
            Assert.IsTrue(Location_Id.Parse("abc") <= Location_Id.Parse("abc2"));
        }

        [Test]
        public void OperatorBigger()
        {
            Assert.IsFalse(Location_Id.Parse("abc")  > Location_Id.Parse("abc"));
            Assert.IsFalse(Location_Id.Parse("abc")  > Location_Id.Parse("aBc"));
            Assert.IsTrue (Location_Id.Parse("abc2") > Location_Id.Parse("abc"));
        }

        [Test]
        public void OperatorBiggerOrEquals()
        {
            Assert.IsTrue(Location_Id.Parse("abc")  >= Location_Id.Parse("abc"));
            Assert.IsTrue(Location_Id.Parse("abc")  >= Location_Id.Parse("aBc"));
            Assert.IsTrue(Location_Id.Parse("abc2") >= Location_Id.Parse("abc"));
        }

        [Test]
        public void HashCodeEquality()
        {
            Assert.AreEqual   (Location_Id.Parse("abc").GetHashCode(), Location_Id.Parse("abc"). GetHashCode());
            Assert.AreEqual   (Location_Id.Parse("abc").GetHashCode(), Location_Id.Parse("aBc"). GetHashCode());
            Assert.AreNotEqual(Location_Id.Parse("abc").GetHashCode(), Location_Id.Parse("abc2").GetHashCode());
        }

        [Test]
        public void DifferentCases_DictionaryKeyEquality()
        {

            var Lookup = new Dictionary<Location_Id, String> {
                             { Location_Id.Parse("abc01"), "DifferentCases_DictionaryKeyEquality()" }
                         };

            Assert.IsTrue(Lookup.ContainsKey(Location_Id.Parse("abc01")));
            Assert.IsTrue(Lookup.ContainsKey(Location_Id.Parse("aBc01")));

        }

        [Test]
        public void Test_ToString()
        {
            Assert.AreEqual("abc", Location_Id.Parse("abc").ToString());
            Assert.AreEqual("aBc", Location_Id.Parse("aBc").ToString());
        }

    }

}
