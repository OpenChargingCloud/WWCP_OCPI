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

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_1_1
{

    /// <summary>
    /// What an exchange of credentials with the other side came to: its
    /// answer, and whether the file of the remote parties took what that
    /// changed.
    /// </summary>
    /// <param name="Response">The other side's answer - or, where nothing was sent, why not.</param>
    /// <param name="NotSaved">Whether the file of the remote parties refused a line: before anything was sent, where the response has no data - nothing has changed; or after the other side accepted - the change is in effect, and written down later.</param>
    /// <param name="Reason">Why the file refused, as AddResult and RemoveResult say it.</param>
    public sealed record CredentialsResult(OCPIResponse<Credentials>  Response,
                                           Boolean                    NotSaved   = false,
                                           String?                    Reason     = null);

}
