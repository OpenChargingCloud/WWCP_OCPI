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

using System.Text.RegularExpressions;

using NUnit.Framework;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPI.UnitTests.Datastructures
{

    /// <summary>
    /// An optional URL that is there but not valid is refused, and one that is
    /// not there is no reason to refuse.
    /// </summary>
    /// <remarks>
    /// ParseOptional of Illias with URL.TryParse took it as a mapper, as
    /// URL.TryParse(String) returns a URL? - and a mapper's null is no URL at
    /// all, not one that is not valid: a URL not valid was passed over, and
    /// the rest read as if it were not there. ParseOptionalURL refuses it.
    /// </remarks>
    [TestFixture]
    public static class OptionalURLTests
    {

        /// <summary>
        /// A URL not valid: its scheme is no scheme. URL.TryParse takes most
        /// of what it is given.
        /// </summary>
        private const String notValid = "no scheme://peer.example/ocpi/versions";


        #region ParseOptionalURL

        [Test]
        public static void AnOptionalURLNotThereIsNoneAndNoError()
        {

            var there = new JObject().ParseOptionalURL("url", "URL", out var url, out var error);

            Assert.Multiple(() => {
                Assert.That(there, Is.False);
                Assert.That(url,   Is.Null);
                Assert.That(error, Is.Null);
            });

        }

        [Test]
        public static void AnOptionalURLThatIsValidIsRead()
        {

            var there = new JObject(new JProperty("url", "https://peer.example/ocpi/versions")).ParseOptionalURL("url", "URL", out var url, out var error);

            Assert.Multiple(() => {
                Assert.That(there,           Is.True);
                Assert.That(url?.ToString(), Is.EqualTo("https://peer.example/ocpi/versions"));
                Assert.That(error,           Is.Null);
            });

        }

        [Test]
        public static void AnOptionalURLNotValidIsRefused()
        {

            var there = new JObject(new JProperty("url", notValid)).ParseOptionalURL("url", "URL", out var url, out var error);

            Assert.Multiple(() => {
                Assert.That(there, Is.True);
                Assert.That(url,   Is.Null);
                Assert.That(error, Is.Not.Null);
            });

        }

        [Test]
        public static void AnOptionalURLThatIsNoTextIsRefused()
        {

            new JObject(new JProperty("url", new JArray("https://peer.example/ocpi/versions"))).ParseOptionalURL("url", "URL", out _, out var error);

            Assert.That(error, Is.Not.Null);

        }

        #endregion

        #region BusinessDetails: website

        [Test]
        public static void BusinessDetailsWithAWebsiteNotValidAreRefused()
        {

            var json = new BusinessDetails("GraphDefined CSO").ToJSON();

            Assert.That(BusinessDetails.TryParse(json, out _, out var error), Is.True, $"Without 'website' they are not read: {error}");

            json["website"] = notValid;

            Assert.That(BusinessDetails.TryParse(json, out _, out error), Is.False, "With 'website' not valid they are read.");

        }

        #endregion

        #region RemoteAccessInfo: versionsURL

        [Test]
        public static void RemoteAccessInfoWithAVersionsURLNotValidIsRefused()
        {

            var json = new RemoteAccessInfo(
                           VersionsURL:  URL.Parse("https://peer.example/ocpi/versions"),
                           AccessToken:  AccessToken.Parse("peer-token")
                       ).ToJSON();

            json.Remove("versionsURL");

            Assert.That(RemoteAccessInfo.TryParse(json, out _, out var error), Is.True, $"Without 'versionsURL' it is not read: {error}");

            json["versionsURL"] = notValid;

            Assert.That(RemoteAccessInfo.TryParse(json, out _, out error), Is.False, "With 'versionsURL' not valid it is read.");

        }

        #endregion

        #region NoOptionalURLIsParsedWithTheMapper

        /// <summary>
        /// No parser of WWCP_OCPI asks for an optional URL with URL.TryParse
        /// any more, but with ParseOptionalURL.
        /// </summary>
        [Test]
        public static void NoOptionalURLIsParsedWithTheMapper()
        {

            var root = new DirectoryInfo(TestContext.CurrentContext.TestDirectory);

            while (root is not null && !Directory.Exists(Path.Combine(root.FullName, "WWCP_OCPI_Common")))
                root = root.Parent;

            Assert.That(root, Is.Not.Null, "The sources of WWCP_OCPI are not found.");

            var call  = new Regex(@"^[^/\r\n]*\.ParseOptional\(\s*""[^""]*"",\s*""[^""]*"",\s*(?:[\w.]+\.)?URL\.TryParse,", RegexOptions.Multiline);

            var found = Directory.GetFiles(root!.FullName, "*.cs", SearchOption.AllDirectories).
                            Where (path => !path.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}") &&
                                           !path.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}") &&
                                           !path.Contains("_Tests")).
                            Where (path => call.IsMatch(File.ReadAllText(path))).
                            Select(path => Path.GetRelativePath(root.FullName, path)).
                            ToArray();

            Assert.That(found, Is.Empty, "An optional URL is parsed with URL.TryParse, which ParseOptional takes as a mapper.");

        }

        #endregion

    }

}
