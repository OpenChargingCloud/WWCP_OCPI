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

using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv3_0
{

    /// <summary>
    /// Create a new OCPI rewrite pipeline.
    /// </summary>
    /// <param name="CommonAPI">The OCPI Common API.</param>
    public class OCPIRewritePipeline(CommonAPI CommonAPI) : AHTTPPipeline()
    {

        #region Properties

        /// <summary>
        /// The OCPI Common API.
        /// </summary>
        public CommonAPI  CommonAPI    { get; } = CommonAPI;

        #endregion


        #region (override) ProcessHTTPRequest(Request, Stream, CancellationToken = default)

        /// <summary>
        /// Process the given HTTP request.
        /// </summary>
        /// <param name="Request">The HTTP request.</param>
        /// <param name="CancellationToken">An optional cancellation token.</param>
        public override async Task<(HTTPRequest, HTTPResponse?)>

            ProcessHTTPRequest(HTTPRequest        Request,
                               CancellationToken  CancellationToken   = default)

        {

            try
            {

                OCPI.AccessToken? accessTokenRAW     = null;
                OCPI.AccessToken? accessTokenBASE64  = null;
                String?           totp               = null;
                RemoteParty?      remoteParty        = null;

                if (Request.Authorization is HTTPTokenAuthentication tokenAuth)
                {

                    if (OCPI.AccessToken.TryParse          (tokenAuth.Token, out var parsedAccessToken))
                        accessTokenRAW     = parsedAccessToken;

                    if (OCPI.AccessToken.TryParseFromBASE64(tokenAuth.Token, out var decodedAccessToken))
                        accessTokenBASE64  = decodedAccessToken;

                    totp                   = Request.GetHeaderField<String>("TOTP");

                }

                else if (Request.Authorization is HTTPBasicAuthentication basicAuth)
                {

                    if (OCPI.AccessToken.TryParse          (basicAuth.Username, out var parsedAccessToken))
                        accessTokenRAW     = parsedAccessToken;

                    if (OCPI.AccessToken.TryParseFromBASE64(basicAuth.Username, out var decodedAccessToken))
                        accessTokenBASE64  = decodedAccessToken;

                    totp                   = basicAuth.Password;

                }

                if (accessTokenRAW.HasValue &&
                    CommonAPI.TryGetRemoteParties(accessTokenRAW.Value,
                                                  totp,
                                                  out var partiesAccessInfosRAW))
                {
                    var tuple = partiesAccessInfosRAW.FirstOrDefault();
                    if (tuple is not null)
                    {
                        if (!tuple.Item2.AccessTokenIsBase64Encoded)
                            remoteParty = tuple.Item1;
                    }
                }

                if (accessTokenBASE64.HasValue &&
                    CommonAPI.TryGetRemoteParties(accessTokenBASE64.Value,
                                                  totp,
                                                  out var partiesAccessInfosBASE64))
                {
                    var tuple = partiesAccessInfosBASE64.FirstOrDefault();
                    if (tuple is not null)
                    {
                        if (tuple.Item2.AccessTokenIsBase64Encoded)
                            remoteParty = tuple.Item1;
                    }
                }


                var requestModifier = remoteParty?.IN?.RequestModifier;
                if (requestModifier is not null)
                {
                    try
                    {
                        return (requestModifier(Request), null);
                    }
                    catch (Exception e)
                    {
                        await CommonAPI.LogException(e);
                    }
                }

            }
            catch (Exception e) {
                await CommonAPI.LogException(e);
            }

            return (Request, null);

        }

        #endregion


    }

}
