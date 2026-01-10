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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPI
{

    /// <summary>
    /// A remote party.
    /// In OCPI v2.1.x this is a single CPO or EMSP.
    /// Since OCPI v2.2.x this is a roaming network operator serving multiple CPOs and/or EMSPs.
    /// </summary>
    public interface IRemoteParty : IHasId<RemoteParty_Id>,
                                    IComparable<RemoteParty_Id>
    {

        /// <summary>
        /// The current status of the party.
        /// </summary>
        [Mandatory]
        PartyStatus                    Status               { get; }

        /// <summary>
        /// Timestamp when this remote party was last updated (or created).
        /// </summary>
        [Mandatory]
        DateTimeOffset                 LastUpdated          { get; }

        /// <summary>
        /// The SHA256 hash of the JSON representation of this remote party.
        /// </summary>
        [Mandatory]
        String                         ETag                 { get; }

        /// <summary>
        /// Local access information.
        /// </summary>
        IEnumerable<LocalAccessInfo>   LocalAccessInfos     { get; }

        /// <summary>
        /// Remote access information.
        /// </summary>
        IEnumerable<RemoteAccessInfo>  RemoteAccessInfos    { get; }

        /// <summary>
        /// Optional incoming request and response modifiers.
        /// </summary>
        RemoteParty.IOModifiers?      IN                   { get; set; }

        /// <summary>
        /// Optional outgoing request and response modifiers.
        /// </summary>
        RemoteParty.IOModifiers?      OUT                  { get; set; }

    }

}
