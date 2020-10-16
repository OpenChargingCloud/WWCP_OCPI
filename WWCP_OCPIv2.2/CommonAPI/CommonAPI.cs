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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using System.Linq;
using System.Threading.Tasks;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2.HTTP
{

    /// <summary>
    /// Extention methods for the Common API.
    /// </summary>
    public static class CommonAPIExtentions
    {

        //#region ParseParseCountryCodePartyId(this HTTPRequest, CommonAPI, out CountryCode, out PartyId,                                                         out HTTPResponse)

        ///// <summary>
        ///// Parse the given HTTP request and return the location identification
        ///// for the given HTTP hostname and HTTP query parameter
        ///// or an HTTP error response.
        ///// </summary>
        ///// <param name="HTTPRequest">A HTTP request.</param>
        ///// <param name="CommonAPI">The Common API.</param>
        ///// <param name="CountryCode">The parsed country code.</param>
        ///// <param name="PartyId">The parsed party identification.</param>
        ///// <param name="HTTPResponse">A HTTP error response.</param>
        ///// <returns>True, when user identification was found; false else.</returns>
        //public static Boolean ParseHTTPRequest(this HTTPRequest  HTTPRequest,
        //                                       CommonAPI         CommonAPI,
        //                                       out CountryCode?  CountryCode,
        //                                       out Party_Id?     PartyId,
        //                                       out HTTPResponse  HTTPResponse)
        //{

        //    #region Initial checks

        //    if (HTTPRequest == null)
        //        throw new ArgumentNullException(nameof(HTTPRequest),  "The given HTTP request must not be null!");

        //    if (CommonAPI    == null)
        //        throw new ArgumentNullException(nameof(CommonAPI),    "The given Common API must not be null!");

        //    #endregion

        //    CountryCode   = null;
        //    PartyId       = null;
        //    HTTPResponse  = null;

        //    if (HTTPRequest.ParsedURLParameters.Length < 2)
        //    {

        //        HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
        //            HTTPStatusCode  = HTTPStatusCode.BadRequest,
        //            Server          = CommonAPI.HTTPServer.DefaultServerName,
        //            Date            = DateTime.UtcNow,
        //            Connection      = "close"
        //        };

        //        return false;

        //    }

        //    CountryCode = OCPIv2_2.CountryCode.TryParse(HTTPRequest.ParsedURLParameters[0]);

        //    if (!CountryCode.HasValue)
        //    {

        //        HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
        //            HTTPStatusCode  = HTTPStatusCode.BadRequest,
        //            Server          = CommonAPI.HTTPServer.DefaultServerName,
        //            Date            = DateTime.UtcNow,
        //            ContentType     = HTTPContentType.JSON_UTF8,
        //            Content         = @"{ ""description"": ""Invalid country code!"" }".ToUTF8Bytes(),
        //            Connection      = "close"
        //        };

        //        return false;

        //    }

        //    PartyId = Party_Id.TryParse(HTTPRequest.ParsedURLParameters[1]);

        //    if (!PartyId.HasValue)
        //    {

        //        HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
        //            HTTPStatusCode  = HTTPStatusCode.BadRequest,
        //            Server          = CommonAPI.HTTPServer.DefaultServerName,
        //            Date            = DateTime.UtcNow,
        //            ContentType     = HTTPContentType.JSON_UTF8,
        //            Content         = @"{ ""description"": ""Invalid party identification!"" }".ToUTF8Bytes(),
        //            Connection      = "close"
        //        };

        //        return false;

        //    }

        //    return true;

        //}

        //#endregion



        //#region ParseParseCountryCodePartyId(this HTTPRequest, CommonAPI, out CountryCode, out PartyId,                                                         out HTTPResponse)

        ///// <summary>
        ///// Parse the given HTTP request and return the location identification
        ///// for the given HTTP hostname and HTTP query parameter
        ///// or an HTTP error response.
        ///// </summary>
        ///// <param name="HTTPRequest">A HTTP request.</param>
        ///// <param name="CommonAPI">The Common API.</param>
        ///// <param name="CountryCode">The parsed country code.</param>
        ///// <param name="PartyId">The parsed party identification.</param>
        ///// <param name="HTTPResponse">A HTTP error response.</param>
        ///// <returns>True, when user identification was found; false else.</returns>
        //public static Boolean ParseParseCountryCodePartyId(this HTTPRequest  HTTPRequest,
        //                                                   CommonAPI         CommonAPI,
        //                                                   out CountryCode?  CountryCode,
        //                                                   out Party_Id?     PartyId,
        //                                                   out HTTPResponse  HTTPResponse)
        //{

        //    #region Initial checks

        //    if (HTTPRequest == null)
        //        throw new ArgumentNullException(nameof(HTTPRequest),  "The given HTTP request must not be null!");

        //    if (CommonAPI    == null)
        //        throw new ArgumentNullException(nameof(CommonAPI),    "The given Common API must not be null!");

        //    #endregion

        //    CountryCode   = null;
        //    PartyId       = null;
        //    HTTPResponse  = null;

        //    if (HTTPRequest.ParsedURLParameters.Length < 2)
        //    {

        //        HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
        //            HTTPStatusCode  = HTTPStatusCode.BadRequest,
        //            Server          = CommonAPI.HTTPServer.DefaultServerName,
        //            Date            = DateTime.UtcNow,
        //            Connection      = "close"
        //        };

        //        return false;

        //    }

        //    CountryCode = OCPIv2_2.CountryCode.TryParse(HTTPRequest.ParsedURLParameters[0]);

        //    if (!CountryCode.HasValue)
        //    {

        //        HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
        //            HTTPStatusCode  = HTTPStatusCode.BadRequest,
        //            Server          = CommonAPI.HTTPServer.DefaultServerName,
        //            Date            = DateTime.UtcNow,
        //            ContentType     = HTTPContentType.JSON_UTF8,
        //            Content         = @"{ ""description"": ""Invalid country code!"" }".ToUTF8Bytes(),
        //            Connection      = "close"
        //        };

        //        return false;

        //    }

        //    PartyId = Party_Id.TryParse(HTTPRequest.ParsedURLParameters[1]);

        //    if (!PartyId.HasValue)
        //    {

        //        HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
        //            HTTPStatusCode  = HTTPStatusCode.BadRequest,
        //            Server          = CommonAPI.HTTPServer.DefaultServerName,
        //            Date            = DateTime.UtcNow,
        //            ContentType     = HTTPContentType.JSON_UTF8,
        //            Content         = @"{ ""description"": ""Invalid party identification!"" }".ToUTF8Bytes(),
        //            Connection      = "close"
        //        };

        //        return false;

        //    }

        //    return true;

        //}

        //#endregion


        //#region ParseLocationId             (this HTTPRequest, CommonAPI, out CountryCode, out PartyId, out LocationId,                                                                      out HTTPResponse)

        ///// <summary>
        ///// Parse the given HTTP request and return the location identification
        ///// for the given HTTP hostname and HTTP query parameter
        ///// or an HTTP error response.
        ///// </summary>
        ///// <param name="HTTPRequest">A HTTP request.</param>
        ///// <param name="CommonAPI">The Common API.</param>
        ///// <param name="CountryCode">The parsed country code.</param>
        ///// <param name="PartyId">The parsed party identification.</param>
        ///// <param name="LocationId">The parsed unique location identification.</param>
        ///// <param name="HTTPResponse">A HTTP error response.</param>
        ///// <returns>True, when user identification was found; false else.</returns>
        //public static Boolean ParseLocationId(this HTTPRequest  HTTPRequest,
        //                                      CommonAPI         CommonAPI,
        //                                      out CountryCode?  CountryCode,
        //                                      out Party_Id?     PartyId,
        //                                      out Location_Id?  LocationId,
        //                                      out HTTPResponse  HTTPResponse)
        //{

        //    #region Initial checks

        //    if (HTTPRequest == null)
        //        throw new ArgumentNullException(nameof(HTTPRequest),  "The given HTTP request must not be null!");

        //    if (CommonAPI    == null)
        //        throw new ArgumentNullException(nameof(CommonAPI),    "The given Common API must not be null!");

        //    #endregion

        //    CountryCode   = null;
        //    PartyId       = null;
        //    LocationId    = null;
        //    HTTPResponse  = null;

        //    if (HTTPRequest.ParsedURLParameters.Length < 3)
        //    {

        //        HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
        //            HTTPStatusCode  = HTTPStatusCode.BadRequest,
        //            Server          = CommonAPI.HTTPServer.DefaultServerName,
        //            Date            = DateTime.UtcNow,
        //            Connection      = "close"
        //        };

        //        return false;

        //    }

        //    CountryCode = OCPIv2_2.CountryCode.TryParse(HTTPRequest.ParsedURLParameters[0]);

        //    if (!CountryCode.HasValue)
        //    {

        //        HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
        //            HTTPStatusCode  = HTTPStatusCode.BadRequest,
        //            Server          = CommonAPI.HTTPServer.DefaultServerName,
        //            Date            = DateTime.UtcNow,
        //            ContentType     = HTTPContentType.JSON_UTF8,
        //            Content         = @"{ ""description"": ""Invalid country code!"" }".ToUTF8Bytes(),
        //            Connection      = "close"
        //        };

        //        return false;

        //    }

        //    PartyId = Party_Id.TryParse(HTTPRequest.ParsedURLParameters[1]);

        //    if (!PartyId.HasValue)
        //    {

        //        HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
        //            HTTPStatusCode  = HTTPStatusCode.BadRequest,
        //            Server          = CommonAPI.HTTPServer.DefaultServerName,
        //            Date            = DateTime.UtcNow,
        //            ContentType     = HTTPContentType.JSON_UTF8,
        //            Content         = @"{ ""description"": ""Invalid party identification!"" }".ToUTF8Bytes(),
        //            Connection      = "close"
        //        };

        //        return false;

        //    }

        //    LocationId = Location_Id.TryParse(HTTPRequest.ParsedURLParameters[2]);

        //    if (!LocationId.HasValue)
        //    {

        //        HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
        //            HTTPStatusCode  = HTTPStatusCode.BadRequest,
        //            Server          = CommonAPI.HTTPServer.DefaultServerName,
        //            Date            = DateTime.UtcNow,
        //            ContentType     = HTTPContentType.JSON_UTF8,
        //            Content         = @"{ ""description"": ""Invalid location identification!"" }".ToUTF8Bytes(),
        //            Connection      = "close"
        //        };

        //        return false;

        //    }

        //    return true;

        //}

        //#endregion

        //#region ParseLocation               (this HTTPRequest, CommonAPI, out CountryCode, out PartyId, out LocationId, out Location,                                                        out HTTPResponse)

        ///// <summary>
        ///// Parse the given HTTP request and return the location identification
        ///// for the given HTTP hostname and HTTP query parameter
        ///// or an HTTP error response.
        ///// </summary>
        ///// <param name="HTTPRequest">A HTTP request.</param>
        ///// <param name="CommonAPI">The Users API.</param>
        ///// <param name="CountryCode">The parsed country code.</param>
        ///// <param name="PartyId">The parsed party identification.</param>
        ///// <param name="LocationId">The parsed unique location identification.</param>
        ///// <param name="Location">The resolved user.</param>
        ///// <param name="HTTPResponse">A HTTP error response.</param>
        ///// <returns>True, when user identification was found; false else.</returns>
        //public static Boolean ParseLocation(this HTTPRequest  HTTPRequest,
        //                                    CommonAPI         CommonAPI,
        //                                    out CountryCode?  CountryCode,
        //                                    out Party_Id?     PartyId,
        //                                    out Location_Id?  LocationId,
        //                                    out Location      Location,
        //                                    out HTTPResponse  HTTPResponse)
        //{

        //    #region Initial checks

        //    if (HTTPRequest == null)
        //        throw new ArgumentNullException(nameof(HTTPRequest),  "The given HTTP request must not be null!");

        //    if (CommonAPI    == null)
        //        throw new ArgumentNullException(nameof(CommonAPI),    "The given Common API must not be null!");

        //    #endregion

        //    CountryCode   = null;
        //    PartyId       = null;
        //    LocationId    = null;
        //    Location      = null;
        //    HTTPResponse  = null;

        //    if (HTTPRequest.ParsedURLParameters.Length < 3) {

        //        HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
        //            HTTPStatusCode  = HTTPStatusCode.BadRequest,
        //            Server          = CommonAPI.HTTPServer.DefaultServerName,
        //            Date            = DateTime.UtcNow,
        //            Connection      = "close"
        //        };

        //        return false;

        //    }

        //    CountryCode = OCPIv2_2.CountryCode.TryParse(HTTPRequest.ParsedURLParameters[0]);

        //    if (!CountryCode.HasValue)
        //    {

        //        HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
        //            HTTPStatusCode  = HTTPStatusCode.BadRequest,
        //            Server          = CommonAPI.HTTPServer.DefaultServerName,
        //            Date            = DateTime.UtcNow,
        //            ContentType     = HTTPContentType.JSON_UTF8,
        //            Content         = @"{ ""description"": ""Invalid country code!"" }".ToUTF8Bytes(),
        //            Connection      = "close"
        //        };

        //        return false;

        //    }

        //    PartyId = Party_Id.TryParse(HTTPRequest.ParsedURLParameters[1]);

        //    if (!PartyId.HasValue)
        //    {

        //        HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
        //            HTTPStatusCode  = HTTPStatusCode.BadRequest,
        //            Server          = CommonAPI.HTTPServer.DefaultServerName,
        //            Date            = DateTime.UtcNow,
        //            ContentType     = HTTPContentType.JSON_UTF8,
        //            Content         = @"{ ""description"": ""Invalid party identification!"" }".ToUTF8Bytes(),
        //            Connection      = "close"
        //        };

        //        return false;

        //    }

        //    LocationId = Location_Id.TryParse(HTTPRequest.ParsedURLParameters[2]);

        //    if (!LocationId.HasValue) {

        //        HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
        //            HTTPStatusCode  = HTTPStatusCode.BadRequest,
        //            Server          = CommonAPI.HTTPServer.DefaultServerName,
        //            Date            = DateTime.UtcNow,
        //            ContentType     = HTTPContentType.JSON_UTF8,
        //            Content         = @"{ ""description"": ""Invalid location identification!"" }".ToUTF8Bytes(),
        //            Connection      = "close"
        //        };

        //        return false;

        //    }


        //    if (!CommonAPI.TryGetLocation(CountryCode.Value, PartyId.Value, LocationId.Value, out Location)) {

        //        HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
        //            HTTPStatusCode  = HTTPStatusCode.NotFound,
        //            Server          = CommonAPI.HTTPServer.DefaultServerName,
        //            Date            = DateTime.UtcNow,
        //            ContentType     = HTTPContentType.JSON_UTF8,
        //            Content         = @"{ ""description"": ""Unknown location identification!"" }".ToUTF8Bytes(),
        //            Connection      = "close"
        //        };

        //        return false;

        //    }

        //    return true;

        //}

        //#endregion


        //#region ParseLocationEVSEId         (this HTTPRequest, CommonAPI, out CountryCode, out PartyId, out LocationId,               out EVSEUId,                                           out HTTPResponse)

        ///// <summary>
        ///// Parse the given HTTP request and return the location identification
        ///// for the given HTTP hostname and HTTP query parameter
        ///// or an HTTP error response.
        ///// </summary>
        ///// <param name="HTTPRequest">A HTTP request.</param>
        ///// <param name="CommonAPI">The Common API.</param>
        ///// <param name="CountryCode">The parsed country code.</param>
        ///// <param name="PartyId">The parsed party identification.</param>
        ///// <param name="LocationId">The parsed unique location identification.</param>
        ///// <param name="EVSEUId">The parsed unique EVSE identification.</param>
        ///// <param name="HTTPResponse">A HTTP error response.</param>
        ///// <returns>True, when user identification was found; false else.</returns>
        //public static Boolean ParseLocationEVSEId(this HTTPRequest  HTTPRequest,
        //                                          CommonAPI         CommonAPI,
        //                                          out CountryCode?  CountryCode,
        //                                          out Party_Id?     PartyId,
        //                                          out Location_Id?  LocationId,
        //                                          out EVSE_UId?     EVSEUId,
        //                                          out HTTPResponse  HTTPResponse)
        //{

        //    #region Initial checks

        //    if (HTTPRequest == null)
        //        throw new ArgumentNullException(nameof(HTTPRequest),  "The given HTTP request must not be null!");

        //    if (CommonAPI    == null)
        //        throw new ArgumentNullException(nameof(CommonAPI),    "The given Common API must not be null!");

        //    #endregion

        //    CountryCode   = null;
        //    PartyId       = null;
        //    LocationId    = null;
        //    EVSEUId       = null;
        //    HTTPResponse  = null;

        //    if (HTTPRequest.ParsedURLParameters.Length < 4)
        //    {

        //        HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
        //            HTTPStatusCode  = HTTPStatusCode.BadRequest,
        //            Server          = CommonAPI.HTTPServer.DefaultServerName,
        //            Date            = DateTime.UtcNow,
        //            Connection      = "close"
        //        };

        //        return false;

        //    }

        //    CountryCode = OCPIv2_2.CountryCode.TryParse(HTTPRequest.ParsedURLParameters[0]);

        //    if (!CountryCode.HasValue)
        //    {

        //        HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
        //            HTTPStatusCode  = HTTPStatusCode.BadRequest,
        //            Server          = CommonAPI.HTTPServer.DefaultServerName,
        //            Date            = DateTime.UtcNow,
        //            ContentType     = HTTPContentType.JSON_UTF8,
        //            Content         = @"{ ""description"": ""Invalid country code!"" }".ToUTF8Bytes(),
        //            Connection      = "close"
        //        };

        //        return false;

        //    }

        //    PartyId = Party_Id.TryParse(HTTPRequest.ParsedURLParameters[1]);

        //    if (!PartyId.HasValue)
        //    {

        //        HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
        //            HTTPStatusCode  = HTTPStatusCode.BadRequest,
        //            Server          = CommonAPI.HTTPServer.DefaultServerName,
        //            Date            = DateTime.UtcNow,
        //            ContentType     = HTTPContentType.JSON_UTF8,
        //            Content         = @"{ ""description"": ""Invalid party identification!"" }".ToUTF8Bytes(),
        //            Connection      = "close"
        //        };

        //        return false;

        //    }

        //    LocationId = Location_Id.TryParse(HTTPRequest.ParsedURLParameters[0]);

        //    if (!LocationId.HasValue)
        //    {

        //        HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
        //            HTTPStatusCode  = HTTPStatusCode.BadRequest,
        //            Server          = CommonAPI.HTTPServer.DefaultServerName,
        //            Date            = DateTime.UtcNow,
        //            ContentType     = HTTPContentType.JSON_UTF8,
        //            Content         = @"{ ""description"": ""Invalid location identification!"" }".ToUTF8Bytes(),
        //            Connection      = "close"
        //        };

        //        return false;

        //    }

        //    EVSEUId = EVSE_UId.TryParse(HTTPRequest.ParsedURLParameters[1]);

        //    if (!EVSEUId.HasValue)
        //    {

        //        HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
        //            HTTPStatusCode  = HTTPStatusCode.BadRequest,
        //            Server          = CommonAPI.HTTPServer.DefaultServerName,
        //            Date            = DateTime.UtcNow,
        //            ContentType     = HTTPContentType.JSON_UTF8,
        //            Content         = @"{ ""description"": ""Invalid EVSE identification!"" }".ToUTF8Bytes(),
        //            Connection      = "close"
        //        };

        //        return false;

        //    }

        //    return true;

        //}

        //#endregion

        //#region ParseLocationEVSE           (this HTTPRequest, CommonAPI, out CountryCode, out PartyId, out LocationId, out Location, out EVSEUId, out EVSE,                                 out HTTPResponse, FailOnMissingEVSE = true)

        ///// <summary>
        ///// Parse the given HTTP request and return the location identification
        ///// for the given HTTP hostname and HTTP query parameter
        ///// or an HTTP error response.
        ///// </summary>
        ///// <param name="HTTPRequest">A HTTP request.</param>
        ///// <param name="CommonAPI">The Users API.</param>
        ///// <param name="CountryCode">The parsed country code.</param>
        ///// <param name="PartyId">The parsed party identification.</param>
        ///// <param name="LocationId">The parsed unique location identification.</param>
        ///// <param name="Location">The resolved user.</param>
        ///// <param name="EVSEUId">The parsed unique EVSE identification.</param>
        ///// <param name="EVSE">The resolved EVSE.</param>
        ///// <param name="HTTPResponse">A HTTP error response.</param>
        ///// <returns>True, when user identification was found; false else.</returns>
        //public static Boolean ParseLocationEVSE(this HTTPRequest  HTTPRequest,
        //                                        CommonAPI         CommonAPI,
        //                                        out CountryCode?  CountryCode,
        //                                        out Party_Id?     PartyId,
        //                                        out Location_Id?  LocationId,
        //                                        out Location      Location,
        //                                        out EVSE_UId?     EVSEUId,
        //                                        out EVSE          EVSE,
        //                                        out HTTPResponse  HTTPResponse)
        //{

        //    #region Initial checks

        //    if (HTTPRequest == null)
        //        throw new ArgumentNullException(nameof(HTTPRequest),  "The given HTTP request must not be null!");

        //    if (CommonAPI    == null)
        //        throw new ArgumentNullException(nameof(CommonAPI),    "The given Common API must not be null!");

        //    #endregion

        //    CountryCode   = null;
        //    PartyId       = null;
        //    LocationId    = null;
        //    Location      = null;
        //    EVSEUId       = null;
        //    EVSE          = null;
        //    HTTPResponse  = null;

        //    if (HTTPRequest.ParsedURLParameters.Length < 4) {

        //        HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
        //            HTTPStatusCode  = HTTPStatusCode.BadRequest,
        //            Server          = CommonAPI.HTTPServer.DefaultServerName,
        //            Date            = DateTime.UtcNow,
        //            Connection      = "close"
        //        };

        //        return false;

        //    }

        //    CountryCode = OCPIv2_2.CountryCode.TryParse(HTTPRequest.ParsedURLParameters[0]);

        //    if (!CountryCode.HasValue)
        //    {

        //        HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
        //            HTTPStatusCode  = HTTPStatusCode.BadRequest,
        //            Server          = CommonAPI.HTTPServer.DefaultServerName,
        //            Date            = DateTime.UtcNow,
        //            ContentType     = HTTPContentType.JSON_UTF8,
        //            Content         = @"{ ""description"": ""Invalid country code!"" }".ToUTF8Bytes(),
        //            Connection      = "close"
        //        };

        //        return false;

        //    }

        //    PartyId = Party_Id.TryParse(HTTPRequest.ParsedURLParameters[1]);

        //    if (!PartyId.HasValue)
        //    {

        //        HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
        //            HTTPStatusCode  = HTTPStatusCode.BadRequest,
        //            Server          = CommonAPI.HTTPServer.DefaultServerName,
        //            Date            = DateTime.UtcNow,
        //            ContentType     = HTTPContentType.JSON_UTF8,
        //            Content         = @"{ ""description"": ""Invalid party identification!"" }".ToUTF8Bytes(),
        //            Connection      = "close"
        //        };

        //        return false;

        //    }

        //    LocationId = Location_Id.TryParse(HTTPRequest.ParsedURLParameters[0]);

        //    if (!LocationId.HasValue) {

        //        HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
        //            HTTPStatusCode  = HTTPStatusCode.BadRequest,
        //            Server          = CommonAPI.HTTPServer.DefaultServerName,
        //            Date            = DateTime.UtcNow,
        //            ContentType     = HTTPContentType.JSON_UTF8,
        //            Content         = @"{ ""description"": ""Invalid location identification!"" }".ToUTF8Bytes(),
        //            Connection      = "close"
        //        };

        //        return false;

        //    }

        //    EVSEUId = EVSE_UId.TryParse(HTTPRequest.ParsedURLParameters[1]);

        //    if (!EVSEUId.HasValue)
        //    {

        //        HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
        //            HTTPStatusCode  = HTTPStatusCode.BadRequest,
        //            Server          = CommonAPI.HTTPServer.DefaultServerName,
        //            Date            = DateTime.UtcNow,
        //            ContentType     = HTTPContentType.JSON_UTF8,
        //            Content         = @"{ ""description"": ""Invalid EVSE identification!"" }".ToUTF8Bytes(),
        //            Connection      = "close"
        //        };

        //        return false;

        //    }


        //    if (!CommonAPI.TryGetLocation(CountryCode.Value,
        //                                  PartyId.    Value,
        //                                  LocationId. Value, out Location)) {

        //        HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
        //            HTTPStatusCode  = HTTPStatusCode.NotFound,
        //            Server          = CommonAPI.HTTPServer.DefaultServerName,
        //            Date            = DateTime.UtcNow,
        //            ContentType     = HTTPContentType.JSON_UTF8,
        //            Content         = @"{ ""description"": ""Unknown location identification!"" }".ToUTF8Bytes(),
        //            Connection      = "close"
        //        };

        //        return false;

        //    }

        //    if (!Location.TryGetEVSE(EVSEUId.Value, out EVSE)) {

        //        HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
        //            HTTPStatusCode  = HTTPStatusCode.NotFound,
        //            Server          = CommonAPI.HTTPServer.DefaultServerName,
        //            Date            = DateTime.UtcNow,
        //            ContentType     = HTTPContentType.JSON_UTF8,
        //            Content         = @"{ ""description"": ""Unknown EVSE identification!"" }".ToUTF8Bytes(),
        //            Connection      = "close"
        //        };

        //        return false;

        //    }

        //    return true;

        //}

        //#endregion


        //#region ParseLocationEVSEConnectorId(this HTTPRequest, CommonAPI, out CountryCode, out PartyId, out LocationId,               out EVSEUId,           out ConnectorId,                out HTTPResponse)

        ///// <summary>
        ///// Parse the given HTTP request and return the location identification
        ///// for the given HTTP hostname and HTTP query parameter
        ///// or an HTTP error response.
        ///// </summary>
        ///// <param name="HTTPRequest">A HTTP request.</param>
        ///// <param name="CommonAPI">The Common API.</param>
        ///// <param name="CountryCode">The parsed country code.</param>
        ///// <param name="PartyId">The parsed party identification.</param>
        ///// <param name="LocationId">The parsed unique location identification.</param>
        ///// <param name="EVSEUId">The parsed unique EVSE identification.</param>
        ///// <param name="ConnectorId">The parsed unique connector identification.</param>
        ///// <param name="HTTPResponse">A HTTP error response.</param>
        ///// <returns>True, when user identification was found; false else.</returns>
        //public static Boolean ParseLocationEVSEConnectorId(this HTTPRequest   HTTPRequest,
        //                                                   CommonAPI          CommonAPI,
        //                                                   out CountryCode?   CountryCode,
        //                                                   out Party_Id?      PartyId,
        //                                                   out Location_Id?   LocationId,
        //                                                   out EVSE_UId?      EVSEUId,
        //                                                   out Connector_Id?  ConnectorId,
        //                                                   out HTTPResponse   HTTPResponse)
        //{

        //    #region Initial checks

        //    if (HTTPRequest == null)
        //        throw new ArgumentNullException(nameof(HTTPRequest),  "The given HTTP request must not be null!");

        //    if (CommonAPI    == null)
        //        throw new ArgumentNullException(nameof(CommonAPI),    "The given Common API must not be null!");

        //    #endregion

        //    CountryCode   = null;
        //    PartyId       = null;
        //    LocationId    = null;
        //    EVSEUId       = null;
        //    ConnectorId   = null;
        //    HTTPResponse  = null;

        //    if (HTTPRequest.ParsedURLParameters.Length < 5)
        //    {

        //        HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
        //            HTTPStatusCode  = HTTPStatusCode.BadRequest,
        //            Server          = CommonAPI.HTTPServer.DefaultServerName,
        //            Date            = DateTime.UtcNow,
        //            Connection      = "close"
        //        };

        //        return false;

        //    }

        //    CountryCode = OCPIv2_2.CountryCode.TryParse(HTTPRequest.ParsedURLParameters[0]);

        //    if (!CountryCode.HasValue)
        //    {

        //        HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
        //            HTTPStatusCode  = HTTPStatusCode.BadRequest,
        //            Server          = CommonAPI.HTTPServer.DefaultServerName,
        //            Date            = DateTime.UtcNow,
        //            ContentType     = HTTPContentType.JSON_UTF8,
        //            Content         = @"{ ""description"": ""Invalid country code!"" }".ToUTF8Bytes(),
        //            Connection      = "close"
        //        };

        //        return false;

        //    }

        //    PartyId = Party_Id.TryParse(HTTPRequest.ParsedURLParameters[1]);

        //    if (!PartyId.HasValue)
        //    {

        //        HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
        //            HTTPStatusCode  = HTTPStatusCode.BadRequest,
        //            Server          = CommonAPI.HTTPServer.DefaultServerName,
        //            Date            = DateTime.UtcNow,
        //            ContentType     = HTTPContentType.JSON_UTF8,
        //            Content         = @"{ ""description"": ""Invalid party identification!"" }".ToUTF8Bytes(),
        //            Connection      = "close"
        //        };

        //        return false;

        //    }

        //    LocationId = Location_Id.TryParse(HTTPRequest.ParsedURLParameters[0]);

        //    if (!LocationId.HasValue)
        //    {

        //        HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
        //            HTTPStatusCode  = HTTPStatusCode.BadRequest,
        //            Server          = CommonAPI.HTTPServer.DefaultServerName,
        //            Date            = DateTime.UtcNow,
        //            ContentType     = HTTPContentType.JSON_UTF8,
        //            Content         = @"{ ""description"": ""Invalid location identification!"" }".ToUTF8Bytes(),
        //            Connection      = "close"
        //        };

        //        return false;

        //    }

        //    EVSEUId = EVSE_UId.TryParse(HTTPRequest.ParsedURLParameters[1]);

        //    if (!EVSEUId.HasValue)
        //    {

        //        HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
        //            HTTPStatusCode  = HTTPStatusCode.BadRequest,
        //            Server          = CommonAPI.HTTPServer.DefaultServerName,
        //            Date            = DateTime.UtcNow,
        //            ContentType     = HTTPContentType.JSON_UTF8,
        //            Content         = @"{ ""description"": ""Invalid EVSE identification!"" }".ToUTF8Bytes(),
        //            Connection      = "close"
        //        };

        //        return false;

        //    }

        //    ConnectorId = Connector_Id.TryParse(HTTPRequest.ParsedURLParameters[2]);

        //    if (!EVSEUId.HasValue)
        //    {

        //        HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
        //            HTTPStatusCode  = HTTPStatusCode.BadRequest,
        //            Server          = CommonAPI.HTTPServer.DefaultServerName,
        //            Date            = DateTime.UtcNow,
        //            ContentType     = HTTPContentType.JSON_UTF8,
        //            Content         = @"{ ""description"": ""Invalid connector identification!"" }".ToUTF8Bytes(),
        //            Connection      = "close"
        //        };

        //        return false;

        //    }

        //    return true;

        //}

        //#endregion

        //#region ParseLocationEVSEConnector  (this HTTPRequest, CommonAPI, out CountryCode, out PartyId, out LocationId, out Location, out EVSEUId, out EVSE, out ConnectorId, out Connector, out HTTPResponse)

        ///// <summary>
        ///// Parse the given HTTP request and return the location identification
        ///// for the given HTTP hostname and HTTP query parameter
        ///// or an HTTP error response.
        ///// </summary>
        ///// <param name="HTTPRequest">A HTTP request.</param>
        ///// <param name="CommonAPI">The Users API.</param>
        ///// <param name="CountryCode">The parsed country code.</param>
        ///// <param name="PartyId">The parsed party identification.</param>
        ///// <param name="LocationId">The parsed unique location identification.</param>
        ///// <param name="Location">The resolved user.</param>
        ///// <param name="EVSEUId">The parsed unique EVSE identification.</param>
        ///// <param name="EVSE">The resolved EVSE.</param>
        ///// <param name="ConnectorId">The parsed unique connector identification.</param>
        ///// <param name="Connector">The resolved connector.</param>
        ///// <param name="HTTPResponse">A HTTP error response.</param>
        ///// <returns>True, when user identification was found; false else.</returns>
        //public static Boolean ParseLocationEVSEConnector(this HTTPRequest   HTTPRequest,
        //                                                 CommonAPI          CommonAPI,
        //                                                 out CountryCode?   CountryCode,
        //                                                 out Party_Id?      PartyId,
        //                                                 out Location_Id?   LocationId,
        //                                                 out Location       Location,
        //                                                 out EVSE_UId?      EVSEUId,
        //                                                 out EVSE           EVSE,
        //                                                 out Connector_Id?  ConnectorId,
        //                                                 out Connector      Connector,
        //                                                 out HTTPResponse   HTTPResponse)
        //{

        //    #region Initial checks

        //    if (HTTPRequest == null)
        //        throw new ArgumentNullException(nameof(HTTPRequest),  "The given HTTP request must not be null!");

        //    if (CommonAPI    == null)
        //        throw new ArgumentNullException(nameof(CommonAPI),    "The given Common API must not be null!");

        //    #endregion

        //    CountryCode   = null;
        //    PartyId       = null;
        //    LocationId    = null;
        //    Location      = null;
        //    EVSEUId       = null;
        //    EVSE          = null;
        //    ConnectorId   = null;
        //    Connector     = null;
        //    HTTPResponse  = null;

        //    if (HTTPRequest.ParsedURLParameters.Length < 5) {

        //        HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
        //            HTTPStatusCode  = HTTPStatusCode.BadRequest,
        //            Server          = CommonAPI.HTTPServer.DefaultServerName,
        //            Date            = DateTime.UtcNow,
        //            Connection      = "close"
        //        };

        //        return false;

        //    }

        //    CountryCode = OCPIv2_2.CountryCode.TryParse(HTTPRequest.ParsedURLParameters[0]);

        //    if (!CountryCode.HasValue)
        //    {

        //        HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
        //            HTTPStatusCode  = HTTPStatusCode.BadRequest,
        //            Server          = CommonAPI.HTTPServer.DefaultServerName,
        //            Date            = DateTime.UtcNow,
        //            ContentType     = HTTPContentType.JSON_UTF8,
        //            Content         = @"{ ""description"": ""Invalid country code!"" }".ToUTF8Bytes(),
        //            Connection      = "close"
        //        };

        //        return false;

        //    }

        //    PartyId = Party_Id.TryParse(HTTPRequest.ParsedURLParameters[1]);

        //    if (!PartyId.HasValue)
        //    {

        //        HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
        //            HTTPStatusCode  = HTTPStatusCode.BadRequest,
        //            Server          = CommonAPI.HTTPServer.DefaultServerName,
        //            Date            = DateTime.UtcNow,
        //            ContentType     = HTTPContentType.JSON_UTF8,
        //            Content         = @"{ ""description"": ""Invalid party identification!"" }".ToUTF8Bytes(),
        //            Connection      = "close"
        //        };

        //        return false;

        //    }

        //    LocationId = Location_Id.TryParse(HTTPRequest.ParsedURLParameters[0]);

        //    if (!LocationId.HasValue) {

        //        HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
        //            HTTPStatusCode  = HTTPStatusCode.BadRequest,
        //            Server          = CommonAPI.HTTPServer.DefaultServerName,
        //            Date            = DateTime.UtcNow,
        //            ContentType     = HTTPContentType.JSON_UTF8,
        //            Content         = @"{ ""description"": ""Invalid location identification!"" }".ToUTF8Bytes(),
        //            Connection      = "close"
        //        };

        //        return false;

        //    }

        //    EVSEUId = EVSE_UId.TryParse(HTTPRequest.ParsedURLParameters[1]);

        //    if (!EVSEUId.HasValue)
        //    {

        //        HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
        //            HTTPStatusCode  = HTTPStatusCode.BadRequest,
        //            Server          = CommonAPI.HTTPServer.DefaultServerName,
        //            Date            = DateTime.UtcNow,
        //            ContentType     = HTTPContentType.JSON_UTF8,
        //            Content         = @"{ ""description"": ""Invalid EVSE identification!"" }".ToUTF8Bytes(),
        //            Connection      = "close"
        //        };

        //        return false;

        //    }

        //    ConnectorId = Connector_Id.TryParse(HTTPRequest.ParsedURLParameters[2]);

        //    if (!EVSEUId.HasValue)
        //    {

        //        HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
        //            HTTPStatusCode  = HTTPStatusCode.BadRequest,
        //            Server          = CommonAPI.HTTPServer.DefaultServerName,
        //            Date            = DateTime.UtcNow,
        //            ContentType     = HTTPContentType.JSON_UTF8,
        //            Content         = @"{ ""description"": ""Invalid connector identification!"" }".ToUTF8Bytes(),
        //            Connection      = "close"
        //        };

        //        return false;

        //    }


        //    if (!CommonAPI.TryGetLocation(CountryCode.Value, PartyId.Value, LocationId.Value, out Location)) {

        //        HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
        //            HTTPStatusCode  = HTTPStatusCode.NotFound,
        //            Server          = CommonAPI.HTTPServer.DefaultServerName,
        //            Date            = DateTime.UtcNow,
        //            ContentType     = HTTPContentType.JSON_UTF8,
        //            Content         = @"{ ""description"": ""Unknown location identification!"" }".ToUTF8Bytes(),
        //            Connection      = "close"
        //        };

        //        return false;

        //    }

        //    if (!Location.TryGetEVSE(EVSEUId.Value, out EVSE)) {

        //        HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
        //            HTTPStatusCode  = HTTPStatusCode.NotFound,
        //            Server          = CommonAPI.HTTPServer.DefaultServerName,
        //            Date            = DateTime.UtcNow,
        //            ContentType     = HTTPContentType.JSON_UTF8,
        //            Content         = @"{ ""description"": ""Unknown EVSE identification!"" }".ToUTF8Bytes(),
        //            Connection      = "close"
        //        };

        //        return false;

        //    }

        //    if (!EVSE.TryGetConnector(ConnectorId.Value, out Connector)) {

        //        HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
        //            HTTPStatusCode  = HTTPStatusCode.NotFound,
        //            Server          = CommonAPI.HTTPServer.DefaultServerName,
        //            Date            = DateTime.UtcNow,
        //            ContentType     = HTTPContentType.JSON_UTF8,
        //            Content         = @"{ ""description"": ""Unknown connector identification!"" }".ToUTF8Bytes(),
        //            Connection      = "close"
        //        };

        //        return false;

        //    }

        //    return true;

        //}

        //#endregion

    }


    /// <summary>
    /// The common HTTP API.
    /// </summary>
    public class CommonAPI : HTTPAPI
    {

        #region Data

        private static readonly    Random    _Random                   = new Random();

        protected internal const   String    __DefaultHTTPRoot         = "cloud.charging.open.protocols.OCPIv2_2.HTTPAPI.CommonAPI.HTTPRoot";

        //private readonly Func<String, Stream>  _GetRessources;


        /// <summary>
        /// The default HTTP server name.
        /// </summary>
        public new const           String    DefaultHTTPServerName     = "GraphDefined OCPI HTTP API v0.1";

        /// <summary>
        /// The default HTTP server name.
        /// </summary>
        public new const           String    DefaultHTTPServiceName    = "GraphDefined OCPI HTTP API v0.1";

        /// <summary>
        /// The default HTTP server TCP port.
        /// </summary>
        public new static readonly IPPort    DefaultHTTPServerPort     = IPPort.Parse(8080);

        /// <summary>
        /// The default HTTP URL path prefix.
        /// </summary>
        public new static readonly HTTPPath  DefaultURLPathPrefix      = HTTPPath.Parse("io/OCPI/");


        private readonly Dictionary<AccessToken, AccessInfo> _AccessTokens;

        #endregion

        #region Properties

        public HTTPPath?  AdditionalURLPathPrefix    { get; }

        #endregion

        #region Events

        #endregion

        #region Constructor(s)

        #region CommonAPI(HTTPServerName, ...)

        /// <summary>
        /// Create a new common HTTP API.
        /// </summary>
        /// <param name="HTTPHostname">An optional HTTP hostname.</param>
        /// <param name="HTTPServerPort">An optional HTTP TCP port.</param>
        /// <param name="HTTPServerName">An optional HTTP server name.</param>
        /// <param name="ExternalDNSName">The offical URL/DNS name of this service, e.g. for sending e-mails.</param>
        /// <param name="URLPathPrefix">An optional HTTP URL path prefix.</param>
        /// <param name="ServiceName">An optional HTTP service name.</param>
        /// <param name="DNSClient">An optional DNS client.</param>
        public CommonAPI(HTTPHostname?   HTTPHostname              = null,
                         IPPort?         HTTPServerPort            = null,
                         String          HTTPServerName            = DefaultHTTPServerName,
                         String          ExternalDNSName           = null,
                         HTTPPath?       URLPathPrefix             = null,
                         String          ServiceName               = DefaultHTTPServiceName,
                         HTTPPath?       AdditionalURLPathPrefix   = null,
                         DNSClient       DNSClient                 = null)

            : base(HTTPHostname,
                   HTTPServerPort ?? DefaultHTTPServerPort,
                   HTTPServerName ?? DefaultHTTPServerName,
                   ExternalDNSName,
                   URLPathPrefix  ?? DefaultURLPathPrefix,
                   ServiceName    ?? DefaultHTTPServiceName,
                   DNSClient)

        {

            this.AdditionalURLPathPrefix  = AdditionalURLPathPrefix;

            this._AccessTokens            = new Dictionary<AccessToken, AccessInfo>();
            this._Locations               = new Dictionary<CountryCode, Dictionary<Party_Id, Dictionary<Location_Id, Location>>>();

            RegisterURLTemplates();

        }

        #endregion

        #region CommonAPI(HTTPServer, ...)

        /// <summary>
        /// Create a new common HTTP API.
        /// </summary>
        /// <param name="HTTPServer">A HTTP server.</param>
        /// <param name="HTTPHostname">An optional HTTP hostname.</param>
        /// <param name="ExternalDNSName">The offical URL/DNS name of this service, e.g. for sending e-mails.</param>
        /// <param name="URLPathPrefix">An optional URL path prefix.</param>
        /// <param name="ServiceName">An optional name of the HTTP API service.</param>
        public CommonAPI(HTTPServer      HTTPServer,
                         HTTPHostname?   HTTPHostname              = null,
                         String          ExternalDNSName           = null,
                         HTTPPath?       URLPathPrefix             = null,
                         String          ServiceName               = DefaultHTTPServerName,
                         HTTPPath?       AdditionalURLPathPrefix   = null)

            : base(HTTPServer,
                   HTTPHostname,
                   ExternalDNSName,
                   URLPathPrefix,
                   ServiceName)

        {

            this.AdditionalURLPathPrefix  = AdditionalURLPathPrefix;

            this._AccessTokens            = new Dictionary<AccessToken, AccessInfo>();
            this._Locations               = new Dictionary<CountryCode, Dictionary<Party_Id, Dictionary<Location_Id, Location>>>();
            this._Tariffs                 = new Dictionary<CountryCode, Dictionary<Party_Id, Dictionary<Tariff_Id,   Tariff>>>();
            this._Sessions                = new Dictionary<CountryCode, Dictionary<Party_Id, Dictionary<Session_Id,  Session>>>();
            this._Tokens                  = new Dictionary<CountryCode, Dictionary<Party_Id, Dictionary<Token_Id,    TokenStatus>>>();
            this._CDRs                    = new Dictionary<CountryCode, Dictionary<Party_Id, Dictionary<CDR_Id,      CDR>>>();

            // Link HTTP events...
            HTTPServer.RequestLog        += (HTTPProcessor, ServerTimestamp, Request)                                 => RequestLog. WhenAll(HTTPProcessor, ServerTimestamp, Request);
            HTTPServer.ResponseLog       += (HTTPProcessor, ServerTimestamp, Request, Response)                       => ResponseLog.WhenAll(HTTPProcessor, ServerTimestamp, Request, Response);
            HTTPServer.ErrorLog          += (HTTPProcessor, ServerTimestamp, Request, Response, Error, LastException) => ErrorLog.   WhenAll(HTTPProcessor, ServerTimestamp, Request, Response, Error, LastException);

            RegisterURLTemplates();

        }

        #endregion

        #endregion


        #region (private) RegisterURLTemplates()

        private void RegisterURLTemplates()
        {

            #region GET    /

            //HTTPServer.RegisterResourcesFolder(HTTPHostname.Any,
            //                                   URLPrefix + "/", "cloud.charging.open.protocols.OCPIv2_2.HTTPAPI.CommonAPI.HTTPRoot",
            //                                   Assembly.GetCallingAssembly());

            //HTTPServer.AddMethodCallback(HTTPHostname.Any,
            //                             HTTPMethod.GET,
            //                             new HTTPPath[] {
            //                                 URLPrefix + "/index.html",
            //                                 URLPrefix + "/"
            //                             },
            //                             HTTPContentType.HTML_UTF8,
            //                             HTTPDelegate: async Request => {

            //                                 var _MemoryStream = new MemoryStream();
            //                                 typeof(CommonAPI).Assembly.GetManifestResourceStream("cloud.charging.open.protocols.OCPIv2_2.HTTPAPI.CommonAPI.HTTPRoot._header.html").SeekAndCopyTo(_MemoryStream, 3);
            //                                 typeof(CommonAPI).Assembly.GetManifestResourceStream("cloud.charging.open.protocols.OCPIv2_2.HTTPAPI.CommonAPI.HTTPRoot._footer.html").SeekAndCopyTo(_MemoryStream, 3);

            //                                 return new HTTPResponse.Builder(Request) {
            //                                     HTTPStatusCode  = HTTPStatusCode.OK,
            //                                     ContentType     = HTTPContentType.HTML_UTF8,
            //                                     Content         = _MemoryStream.ToArray(),
            //                                     Connection      = "close"
            //                                 };

            //                             });

            HTTPServer.AddMethodCallback(HTTPHostname.Any,
                                         HTTPMethod.GET,
                                         URLPathPrefix,
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: async Request => {

                                             return new HTTPResponse.Builder(Request) {
                                                 HTTPStatusCode  = HTTPStatusCode.OK,
                                                 Server          = DefaultHTTPServerName,
                                                 Date            = DateTime.UtcNow,
                                                 ContentType     = HTTPContentType.TEXT_UTF8,
                                                 Content         = ("This is an Open Charge Point Interface HTTP service!\nPlease check /versions!").ToUTF8Bytes(),
                                                 Connection      = "close"
                                             };

                                         });

            #endregion


            #region GET         ~/versions

            // ----------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2000/versions
            // ----------------------------------------------------------------------
            HTTPServer.AddMethodCallback(Hostname,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "versions",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: Request => {

                                             var OCPIRequest = HTTP.OCPIRequest.Parse(Request);

                                             #region Check access token

                                             if (OCPIRequest.AccessToken.HasValue &&
                                                 _AccessTokens.TryGetValue(OCPIRequest.AccessToken.Value, out AccessInfo accessInfo) &&
                                                 accessInfo.Status != AccessStatus.ALLOWED)
                                             {

                                                 // Invalid or blocked access token!
                                                 return Task.FromResult(
                                                     new HTTPResponse.Builder(Request) {
                                                         HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                         Server                     = HTTPServer.DefaultServerName,
                                                         Date                       = DateTime.UtcNow,
                                                         AccessControlAllowOrigin   = "*",
                                                         AccessControlAllowMethods  = "GET",
                                                         AccessControlAllowHeaders  = "Authorization",
                                                         Connection                 = "close"
                                                     }.AsImmutable);

                                             }

                                             #endregion


                                             return Task.FromResult(
                                                 new HTTPResponse.Builder(Request) {
                                                     HTTPStatusCode                = HTTPStatusCode.OK,
                                                     Server                        = HTTPServer.DefaultServerName,
                                                     Date                          = DateTime.UtcNow,
                                                     AccessControlAllowOrigin      = "*",
                                                     AccessControlAllowMethods     = "GET",
                                                     AccessControlAllowHeaders     = "Authorization",
                                                     ContentType                   = HTTPContentType.JSON_UTF8,
                                                     Content                       = OCPIResponse<IEnumerable<Version>>.Create(
                                                                                         new Version[] {
                                                                                             new Version(
                                                                                                 Version_Id.Parse("2.2"),
                                                                                                 "https://" + (Request.Host + URLPathPrefix + AdditionalURLPathPrefix + "/versions/2.2").Replace("//", "/")
                                                                                             )
                                                                                         },
                                                                                         versions => new JArray(versions.Select(version => version.ToJSON())),
                                                                                         1000,
                                                                                         "Hello world!"
                                                                                     ).ToUTF8Bytes(),
                                                     Connection                    = "close"
                                                 }.AsImmutable);


                                         });

            #endregion

            #region GET         ~/versions/{id}

            // ---------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2000/versions/{id}
            // ---------------------------------------------------------------------------
            HTTPServer.AddMethodCallback(Hostname,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "versions/{id}",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: Request => {

                                             #region Get version identification URL parameter

                                             if (Request.ParsedURLParameters.Length < 1)
                                             {

                                                 return Task.FromResult(
                                                     new HTTPResponse.Builder(Request) {
                                                         HTTPStatusCode  = HTTPStatusCode.BadRequest,
                                                         Server          = HTTPServer.DefaultServerName,
                                                         Date            = DateTime.UtcNow,
                                                         Connection      = "close"
                                                     }.AsImmutable);

                                             }

                                             if (!Version_Id.TryParse(Request.ParsedURLParameters[0], out Version_Id versionId))
                                             {

                                                 return Task.FromResult(
                                                     new HTTPResponse.Builder(Request) {
                                                         HTTPStatusCode  = HTTPStatusCode.BadRequest,
                                                         Server          = HTTPServer.DefaultServerName,
                                                         Date            = DateTime.UtcNow,
                                                         Connection      = "close"
                                                     }.AsImmutable);

                                             }

                                             #endregion

                                             #region Check access token

                                             AccessInfo accessInfo = default;

                                             if (Request.Authorization is HTTPTokenAuthentication TokenAuth &&
                                                  AccessToken. TryParse   (StringExtensions.FromBase64(TokenAuth.Token), out AccessToken accessToken) &&
                                                 _AccessTokens.TryGetValue(accessToken,                                  out             accessInfo)  &&
                                                 accessInfo.Status != AccessStatus.ALLOWED)
                                             {

                                                 // Invalid or blocked access token!
                                                 return Task.FromResult(
                                                     new HTTPResponse.Builder(Request) {
                                                         HTTPStatusCode                = HTTPStatusCode.Forbidden,
                                                         Server                        = HTTPServer.DefaultServerName,
                                                         Date                          = DateTime.UtcNow,
                                                         AccessControlAllowOrigin      = "*",
                                                         AccessControlAllowMethods     = "GET",
                                                         AccessControlAllowHeaders     = "Authorization",
                                                         Connection                    = "close"
                                                     }.AsImmutable);

                                             }

                                             #endregion

                                             #region Only allow versionId == "2.2"

                                             if (versionId.ToString() != "2.2")
                                             {

                                                 return Task.FromResult(
                                                     new HTTPResponse.Builder(Request) {
                                                         HTTPStatusCode  = HTTPStatusCode.NotFound,
                                                         Server          = HTTPServer.DefaultServerName,
                                                         Date            = DateTime.UtcNow,
                                                         Connection      = "close"
                                                     }.AsImmutable);

                                             }

                                             #endregion


                                             var endpoints  = new List<VersionEndpoint>() {

                                                                  new VersionEndpoint(ModuleIDs.Credentials,
                                                                                      InterfaceRoles.SENDER,
                                                                                      "https://" + (Request.Host + URLPathPrefix + AdditionalURLPathPrefix + "2.2/credentials").Replace("//", "/")),

                                                                  new VersionEndpoint(ModuleIDs.Credentials,
                                                                                      InterfaceRoles.RECEIVER,
                                                                                      "https://" + (Request.Host + URLPathPrefix + AdditionalURLPathPrefix + "2.2/credentials").Replace("//", "/"))

                                                              };


                                             if (accessInfo.Role == Roles.CPO)
                                             {

                                                 endpoints.Add(new VersionEndpoint(ModuleIDs.Locations,
                                                                                   InterfaceRoles.RECEIVER,
                                                                                   "https://" + (Request.Host + URLPathPrefix + AdditionalURLPathPrefix + "2.2/emsp/locations").Replace("//", "/")));

                                                 endpoints.Add(new VersionEndpoint(ModuleIDs.Tariffs,
                                                                                   InterfaceRoles.RECEIVER,
                                                                                   "https://" + (Request.Host + URLPathPrefix + AdditionalURLPathPrefix + "2.2/emsp/tariffs").Replace("//", "/")));

                                                 endpoints.Add(new VersionEndpoint(ModuleIDs.Sessions,
                                                                                   InterfaceRoles.RECEIVER,
                                                                                   "https://" + (Request.Host + URLPathPrefix + AdditionalURLPathPrefix + "2.2/emsp/sessions").Replace("//", "/")));

                                                 endpoints.Add(new VersionEndpoint(ModuleIDs.CDRs,
                                                                                   InterfaceRoles.RECEIVER,
                                                                                   "https://" + (Request.Host + URLPathPrefix + AdditionalURLPathPrefix + "2.2/emsp/cdrs").Replace("//", "/")));


                                                 endpoints.Add(new VersionEndpoint(ModuleIDs.Commands,
                                                                                   InterfaceRoles.SENDER,
                                                                                   "https://" + (Request.Host + URLPathPrefix + AdditionalURLPathPrefix + "2.2/emsp/commands").Replace("//", "/")));

                                                 endpoints.Add(new VersionEndpoint(ModuleIDs.Tokens,
                                                                                   InterfaceRoles.SENDER,
                                                                                   "https://" + (Request.Host + URLPathPrefix + AdditionalURLPathPrefix + "2.2/emsp/tokens").Replace("//", "/")));

                                                 // hubclientinfo

                                             }


                                             if (accessInfo.Role == Roles.EMSP || accessInfo.Role == Roles.OpenData)
                                             {

                                                 endpoints.Add(new VersionEndpoint(ModuleIDs.Locations,
                                                                                   InterfaceRoles.SENDER,
                                                                                   "https://" + (Request.Host + URLPathPrefix + AdditionalURLPathPrefix + "2.2/cpo/locations").Replace("//", "/")));

                                             }

                                             if (accessInfo.Role == Roles.EMSP)
                                             {

                                                 endpoints.Add(new VersionEndpoint(ModuleIDs.CDRs,
                                                                                   InterfaceRoles.SENDER,
                                                                                   "https://" + (Request.Host + URLPathPrefix + AdditionalURLPathPrefix + "2.2/cpo/cdrs").Replace("//", "/")));

                                                 endpoints.Add(new VersionEndpoint(ModuleIDs.Sessions,
                                                                                   InterfaceRoles.SENDER,
                                                                                   "https://" + (Request.Host + URLPathPrefix + AdditionalURLPathPrefix + "2.2/cpo/sessions").Replace("//", "/")));


                                                 endpoints.Add(new VersionEndpoint(ModuleIDs.Locations,
                                                                                   InterfaceRoles.SENDER,
                                                                                   "https://" + (Request.Host + URLPathPrefix + AdditionalURLPathPrefix + "2.2/cpo/locations").Replace("//", "/")));

                                                 endpoints.Add(new VersionEndpoint(ModuleIDs.Tariffs,
                                                                                   InterfaceRoles.SENDER,
                                                                                   "https://" + (Request.Host + URLPathPrefix + AdditionalURLPathPrefix + "2.2/cpo/tariffs").Replace("//", "/")));

                                                 endpoints.Add(new VersionEndpoint(ModuleIDs.Sessions,
                                                                                   InterfaceRoles.SENDER,
                                                                                   "https://" + (Request.Host + URLPathPrefix + AdditionalURLPathPrefix + "2.2/cpo/sessions").Replace("//", "/")));

                                                 endpoints.Add(new VersionEndpoint(ModuleIDs.CDRs,
                                                                                   InterfaceRoles.SENDER,
                                                                                   "https://" + (Request.Host + URLPathPrefix + AdditionalURLPathPrefix + "2.2/cpo/cdrs").Replace("//", "/")));


                                                 endpoints.Add(new VersionEndpoint(ModuleIDs.Commands,
                                                                                   InterfaceRoles.RECEIVER,
                                                                                   "https://" + (Request.Host + URLPathPrefix + AdditionalURLPathPrefix + "2.2/cpo/commands").Replace("//", "/")));

                                                 endpoints.Add(new VersionEndpoint(ModuleIDs.Tokens,
                                                                                   InterfaceRoles.RECEIVER,
                                                                                   "https://" + (Request.Host + URLPathPrefix + AdditionalURLPathPrefix + "2.2/cpo/tokens").Replace("//", "/")));

                                                 // hubclientinfo

                                             }


                                             return Task.FromResult(
                                                 new HTTPResponse.Builder(Request) {
                                                     HTTPStatusCode             = HTTPStatusCode.OK,
                                                     Server                     = HTTPServer.DefaultServerName,
                                                     Date                       = DateTime.UtcNow,
                                                     AccessControlAllowOrigin   = "*",
                                                     AccessControlAllowMethods  = "GET",
                                                     AccessControlAllowHeaders  = "Authorization",
                                                     ContentType                = HTTPContentType.JSON_UTF8,
                                                     Content                    = OCPIResponse<VersionDetail>.Create(
                                                                                      new VersionDetail(
                                                                                          Version_Id.Parse("2.2"),
                                                                                          endpoints),
                                                                                      version => version.ToJSON(),
                                                                                      1000,
                                                                                      "Hello world!"
                                                                                  ).ToUTF8Bytes(),
                                                     Connection                 = "close"
                                                 }.AsImmutable);

                                         });

            #endregion



            #region GET         ~/2.2/credentials

            // ---------------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2000/2.2/credentials
            // ---------------------------------------------------------------------------------
            HTTPServer.AddMethodCallback(Hostname,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "2.2/credentials",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: Request => {

                                             if (Request.Authorization is HTTPTokenAuthentication TokenAuth &&
                                                 AccessToken. TryParse   (StringExtensions.FromBase64(TokenAuth.Token), out AccessToken accessToken) &&
                                                _AccessTokens.TryGetValue(accessToken,                                  out AccessInfo  accessInfo)  &&
                                                 accessInfo.Status == AccessStatus.ALLOWED)
                                             {

                                                 return Task.FromResult(
                                                     new HTTPResponse.Builder(Request) {
                                                         HTTPStatusCode             = HTTPStatusCode.OK,
                                                         Server                     = HTTPServer.DefaultServerName,
                                                         Date                       = DateTime.UtcNow,
                                                         AccessControlAllowOrigin   = "*",
                                                         AccessControlAllowMethods  = "GET, POST, PUT, DELETE",
                                                         AccessControlAllowHeaders  = "Authorization",
                                                         ContentType                = HTTPContentType.JSON_UTF8,
                                                         Content                    = OCPIResponse<OCPIv2_2.Credentials>.Create(
                                                                                          accessInfo.Credentials,
                                                                                          credentials => credentials.ToJSON(),
                                                                                          1000,
                                                                                          "Hello world!"
                                                                                      ).ToUTF8Bytes(),
                                                         Connection                 = "close"
                                                     }.AsImmutable);

                                             }

                                             return Task.FromResult(
                                                 new HTTPResponse.Builder(Request) {
                                                     HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                     Server                     = HTTPServer.DefaultServerName,
                                                     Date                       = DateTime.UtcNow,
                                                     AccessControlAllowOrigin   = "*",
                                                     AccessControlAllowMethods  = "GET",
                                                     AccessControlAllowHeaders  = "Authorization",
                                                     Connection                 = "close"
                                                 }.AsImmutable);

                                         });

            #endregion

            #region POST        ~/2.2/credentials

            // REGISTER new OCPI party!

            // ---------------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2000/2.2/credentials
            // ---------------------------------------------------------------------------------
            HTTPServer.AddMethodCallback(Hostname,
                                         HTTPMethod.POST,
                                         URLPathPrefix + "cpo/2.2/credentials",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: Request => {

                                             if (Request.Authorization is HTTPTokenAuthentication TokenAuth &&
                                                 AccessToken. TryParse   (StringExtensions.FromBase64(TokenAuth.Token), out AccessToken  accessToken) &&
                                                _AccessTokens.TryGetValue(accessToken,                                  out AccessInfo   accessInfo)  &&
                                                 accessInfo.Status == AccessStatus.ALLOWED)
                                             {

                                                 if (accessInfo.Credentials != null)
                                                     return Task.FromResult(
                                                         new HTTPResponse.Builder(Request) {
                                                             HTTPStatusCode                = HTTPStatusCode.MethodNotAllowed,
                                                             Server                        = HTTPServer.DefaultServerName,
                                                             Date                          = DateTime.UtcNow,
                                                             AccessControlAllowOrigin      = "*",
                                                             AccessControlAllowMethods     = "GET, POST, PUT, DELETE",
                                                             AccessControlAllowHeaders     = "Authorization",
                                                             Connection                    = "close"
                                                         }.AsImmutable);


                                                 if (!Request.TryParseJObjectRequestBody(out JObject JSONObj, out HTTPResponse.Builder HTTPResp, AllowEmptyHTTPBody: false) ||
                                                     !Credentials.TryParse(JSONObj, out Credentials credentials, out String ErrorResponse))
                                                    return Task.FromResult(
                                                         new HTTPResponse.Builder(Request) {
                                                             HTTPStatusCode                = HTTPStatusCode.BadRequest,
                                                             Server                        = HTTPServer.DefaultServerName,
                                                             Date                          = DateTime.UtcNow,
                                                             AccessControlAllowOrigin      = "*",
                                                             AccessControlAllowMethods     = "GET, POST, PUT, DELETE",
                                                             AccessControlAllowHeaders     = "Authorization",
                                                             Connection                    = "close"
                                                         }.AsImmutable);


                                                 // Here we should check the other side!!!


                                                 SetAccessToken(accessToken,
                                                                accessInfo.Role,
                                                                credentials,
                                                                AccessStatus.ALLOWED);


                                                 return Task.FromResult(
                                                     new HTTPResponse.Builder(Request) {
                                                         HTTPStatusCode                = HTTPStatusCode.OK,
                                                         Server                        = HTTPServer.DefaultServerName,
                                                         Date                          = DateTime.UtcNow,
                                                         AccessControlAllowOrigin      = "*",
                                                         AccessControlAllowMethods     = "GET, POST, PUT, DELETE",
                                                         AccessControlAllowHeaders     = "Authorization",
                                                         ContentType                   = HTTPContentType.JSON_UTF8,
                                                         Content                       = OCPIResponse<OCPIv2_2.Credentials>.Create(
                                                                                             credentials,
                                                                                             credential => credential.ToJSON(),
                                                                                             1000,
                                                                                             "Hello world!"
                                                                                         ).ToUTF8Bytes(),
                                                         Connection                    = "close"
                                                     }.AsImmutable);

                                             }

                                             return Task.FromResult(
                                                 new HTTPResponse.Builder(Request) {
                                                     HTTPStatusCode                = HTTPStatusCode.Forbidden,
                                                     Server                        = HTTPServer.DefaultServerName,
                                                     Date                          = DateTime.UtcNow,
                                                     AccessControlAllowOrigin      = "*",
                                                     AccessControlAllowMethods     = "GET",
                                                     AccessControlAllowHeaders     = "Authorization",
                                                     Connection                    = "close"
                                                 }.AsImmutable);

                                         });

            #endregion

            #region PUT         ~/2.2/credentials

            // UPDATE the registration of an existing OCPI party!

            // ---------------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2000/2.2/credentials
            // ---------------------------------------------------------------------------------
            HTTPServer.AddMethodCallback(Hostname,
                                         HTTPMethod.PUT,
                                         URLPathPrefix + "2.2/credentials",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: Request => {

                                             if (Request.Authorization is HTTPTokenAuthentication TokenAuth &&
                                                 AccessToken. TryParse   (StringExtensions.FromBase64(TokenAuth.Token), out AccessToken  accessToken) &&
                                                _AccessTokens.TryGetValue(accessToken,                                  out AccessInfo   accessInfo)  &&
                                                 accessInfo.Status == AccessStatus.ALLOWED)
                                             {

                                                 if (accessInfo.Credentials == null)
                                                     return Task.FromResult(
                                                         new HTTPResponse.Builder(Request) {
                                                             HTTPStatusCode                = HTTPStatusCode.MethodNotAllowed,
                                                             Server                        = HTTPServer.DefaultServerName,
                                                             Date                          = DateTime.UtcNow,
                                                             AccessControlAllowOrigin      = "*",
                                                             AccessControlAllowMethods     = "GET, POST, PUT, DELETE",
                                                             AccessControlAllowHeaders     = "Authorization",
                                                             Connection                    = "close"
                                                         }.AsImmutable);


                                                 if (!Request.TryParseJObjectRequestBody(out JObject JSONObj, out HTTPResponse.Builder HTTPResp, AllowEmptyHTTPBody: false) ||
                                                     !Credentials.TryParse(JSONObj, out Credentials credentials, out String ErrorResponse))
                                                    return Task.FromResult(
                                                         new HTTPResponse.Builder(Request) {
                                                             HTTPStatusCode                = HTTPStatusCode.BadRequest,
                                                             Server                        = HTTPServer.DefaultServerName,
                                                             Date                          = DateTime.UtcNow,
                                                             AccessControlAllowOrigin      = "*",
                                                             AccessControlAllowMethods     = "GET, POST, PUT, DELETE",
                                                             AccessControlAllowHeaders     = "Authorization",
                                                             Connection                    = "close"
                                                         }.AsImmutable);


                                                 // Here we should check the other side!!!


                                                 SetAccessToken(accessToken,
                                                                accessInfo.Role,
                                                                credentials,
                                                                AccessStatus.ALLOWED);


                                                 return Task.FromResult(
                                                     new HTTPResponse.Builder(Request) {
                                                         HTTPStatusCode                = HTTPStatusCode.OK,
                                                         Server                        = HTTPServer.DefaultServerName,
                                                         Date                          = DateTime.UtcNow,
                                                         AccessControlAllowOrigin      = "*",
                                                         AccessControlAllowMethods     = "GET, POST, PUT, DELETE",
                                                         AccessControlAllowHeaders     = "Authorization",
                                                         ContentType                   = HTTPContentType.JSON_UTF8,
                                                         Content                       = OCPIResponse<OCPIv2_2.Credentials>.Create(
                                                                                             accessInfo.Credentials,
                                                                                             credential => credential.ToJSON(),
                                                                                             1000,
                                                                                             "Hello world!"
                                                                                         ).ToUTF8Bytes(),
                                                         Connection                    = "close"
                                                     }.AsImmutable);

                                             }

                                             return Task.FromResult(
                                                 new HTTPResponse.Builder(Request) {
                                                     HTTPStatusCode                = HTTPStatusCode.Forbidden,
                                                     Server                        = HTTPServer.DefaultServerName,
                                                     Date                          = DateTime.UtcNow,
                                                     AccessControlAllowOrigin      = "*",
                                                     AccessControlAllowMethods     = "GET",
                                                     AccessControlAllowHeaders     = "Authorization",
                                                     Connection                    = "close"
                                                 }.AsImmutable);

                                         });

            #endregion

            #region DELETE      ~/2.2/credentials

            // UNREGISTER an existing OCPI party!

            // ---------------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2000/2.2/credentials
            // ---------------------------------------------------------------------------------
            HTTPServer.AddMethodCallback(Hostname,
                                         HTTPMethod.DELETE,
                                         URLPathPrefix + "2.2/credentials",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: Request => {

                                             if (Request.Authorization is HTTPTokenAuthentication TokenAuth &&
                                                 AccessToken. TryParse   (StringExtensions.FromBase64(TokenAuth.Token), out AccessToken  accessToken) &&
                                                _AccessTokens.TryGetValue(accessToken,                                  out AccessInfo   accessInfo)  &&
                                                 accessInfo.Status == AccessStatus.ALLOWED)
                                             {

                                                 if (accessInfo.Credentials == null)
                                                     return Task.FromResult(
                                                         new HTTPResponse.Builder(Request) {
                                                             HTTPStatusCode                = HTTPStatusCode.MethodNotAllowed,
                                                             Server                        = HTTPServer.DefaultServerName,
                                                             Date                          = DateTime.UtcNow,
                                                             AccessControlAllowOrigin      = "*",
                                                             AccessControlAllowMethods     = "GET, POST, PUT, DELETE",
                                                             AccessControlAllowHeaders     = "Authorization",
                                                             Connection                    = "close"
                                                         }.AsImmutable);


                                                 RemoveAccessToken(accessToken);


                                                 return Task.FromResult(
                                                     new HTTPResponse.Builder(Request) {
                                                         HTTPStatusCode                = HTTPStatusCode.OK,
                                                         Server                        = HTTPServer.DefaultServerName,
                                                         Date                          = DateTime.UtcNow,
                                                         AccessControlAllowOrigin      = "*",
                                                         AccessControlAllowMethods     = "GET, POST, PUT, DELETE",
                                                         AccessControlAllowHeaders     = "Authorization",
                                                         Connection                    = "close"
                                                     }.AsImmutable);

                                             }

                                             return Task.FromResult(
                                                 new HTTPResponse.Builder(Request) {
                                                     HTTPStatusCode                = HTTPStatusCode.Forbidden,
                                                     Server                        = HTTPServer.DefaultServerName,
                                                     Date                          = DateTime.UtcNow,
                                                     AccessControlAllowOrigin      = "*",
                                                     AccessControlAllowMethods     = "GET",
                                                     AccessControlAllowHeaders     = "Authorization",
                                                     Connection                    = "close"
                                                 }.AsImmutable);

                                         });

            #endregion


        }

        #endregion


        #region AccessTokens

        public CommonAPI SetAccessToken(AccessToken   AccessToken,
                                        Roles         Role,
                                        AccessStatus  AccessStatus   = AccessStatus.ALLOWED)
        {
            lock (_AccessTokens)
            {

                _AccessTokens.Remove(AccessToken);

                _AccessTokens.Add(AccessToken,
                                  new AccessInfo(Role,
                                                 AccessStatus,
                                                 null));

                return this;

            }
        }

        public CommonAPI SetAccessToken(AccessToken   AccessToken,
                                        Roles         Role,
                                        Credentials   Credentials,
                                        AccessStatus  AccessStatus = AccessStatus.ALLOWED)
        {
            lock (_AccessTokens)
            {

                _AccessTokens.Remove(AccessToken);

                _AccessTokens.Add(AccessToken,
                                  new AccessInfo(Role,
                                                 AccessStatus,
                                                 Credentials));

                return this;

            }
        }

        public CommonAPI RemoveAccessToken(AccessToken AccessToken)
        {
            lock (_AccessTokens)
            {
                _AccessTokens.Remove(AccessToken);
                return this;
            }
        }

        #endregion


        //ToDo: Wrap this into an plugable interface!

        #region Locations

        private readonly Dictionary<CountryCode, Dictionary<Party_Id, Dictionary<Location_Id , Location>>> _Locations;


        #region AddLocation           (Location)

        public Location AddLocation(Location Location)
        {

            if (Location is null)
                throw new ArgumentNullException(nameof(Location), "The given location must not be null!");

            lock (_Locations)
            {

                if (!_Locations.TryGetValue(Location.CountryCode, out Dictionary<Party_Id, Dictionary<Location_Id, Location>> parties))
                {
                    parties = new Dictionary<Party_Id, Dictionary<Location_Id, Location>>();
                    _Locations.Add(Location.CountryCode, parties);
                }

                if (!parties.TryGetValue(Location.PartyId, out Dictionary<Location_Id, Location> locations))
                {
                    locations = new Dictionary<Location_Id, Location>();
                    parties.Add(Location.PartyId, locations);
                }

                if (!locations.ContainsKey(Location.Id))
                {
                    locations.Add(Location.Id, Location);
                    return Location;
                }

                throw new ArgumentException("The given location already exists!");

            }

        }

        #endregion

        #region AddLocationIfNotExists(Location)

        public Location AddLocationIfNotExists(Location Location)
        {

            if (Location is null)
                throw new ArgumentNullException(nameof(Location), "The given location must not be null!");

            lock (_Locations)
            {

                if (!_Locations.TryGetValue(Location.CountryCode, out Dictionary<Party_Id, Dictionary<Location_Id, Location>> parties))
                {
                    parties = new Dictionary<Party_Id, Dictionary<Location_Id, Location>>();
                    _Locations.Add(Location.CountryCode, parties);
                }

                if (!parties.TryGetValue(Location.PartyId, out Dictionary<Location_Id, Location> locations))
                {
                    locations = new Dictionary<Location_Id, Location>();
                    parties.Add(Location.PartyId, locations);
                }

                if (!locations.ContainsKey(Location.Id))
                    locations.Add(Location.Id, Location);

                return Location;

            }

        }

        #endregion

        #region AddOrUpdateLocation   (Location)

        public Location AddOrUpdateLocation(Location Location)
        {

            if (Location is null)
                throw new ArgumentNullException(nameof(Location), "The given location must not be null!");

            lock (_Locations)
            {

                if (!_Locations.TryGetValue(Location.CountryCode, out Dictionary<Party_Id, Dictionary<Location_Id, Location>> parties))
                {
                    parties = new Dictionary<Party_Id, Dictionary<Location_Id, Location>>();
                    _Locations.Add(Location.CountryCode, parties);
                }

                if (!parties.TryGetValue(Location.PartyId, out Dictionary<Location_Id, Location> locations))
                {
                    locations = new Dictionary<Location_Id, Location>();
                    parties.Add(Location.PartyId, locations);
                }

                if (locations.ContainsKey(Location.Id))
                {
                    locations.Remove(Location.Id);
                }

                locations.Add(Location.Id, Location);
                return Location;

            }

        }

        #endregion

        #region UpdateLocation        (Location)

        public Location UpdateLocation(Location Location)
        {

            if (Location is null)
                throw new ArgumentNullException(nameof(Location), "The given location must not be null!");

            lock (_Locations)
            {

                if (_Locations.TryGetValue(Location.CountryCode, out Dictionary<Party_Id, Dictionary<Location_Id, Location>> parties)   &&
                    parties.   TryGetValue(Location.PartyId,     out                      Dictionary<Location_Id, Location>  locations) &&
                    locations.ContainsKey(Location.Id))
                {
                    locations[Location.Id] = Location;
                    return Location;
                }

                return null;

            }

        }

        #endregion


        #region TryGetLocation(CountryCode, PartyId, LocationId,, out Location)

        public Boolean TryGetLocation(CountryCode   CountryCode,
                                      Party_Id      PartyId,
                                      Location_Id   LocationId,
                                      out Location  Location)
        {

            lock (_Locations)
            {

                if (_Locations.TryGetValue(CountryCode, out Dictionary<Party_Id, Dictionary<Location_Id, Location>> parties))
                {
                    if (parties.TryGetValue(PartyId, out Dictionary<Location_Id, Location> locations))
                    {
                        if (locations.TryGetValue(LocationId, out Location))
                            return true;
                    }
                }

                Location = null;
                return false;

            }

        }

        #endregion

        #region GetLocations(CountryCode = null, PartyId = null)

        public IEnumerable<Location> GetLocations(CountryCode? CountryCode  = null,
                                                  Party_Id?    PartyId      = null)
        {

            lock (_Locations)
            {

                if (CountryCode.HasValue && PartyId.HasValue)
                {
                    if (_Locations.TryGetValue(CountryCode.Value, out Dictionary<Party_Id, Dictionary<Location_Id, Location>> parties))
                    {
                        if (parties.TryGetValue(PartyId.Value, out Dictionary<Location_Id, Location> locations))
                        {
                            return locations.Values.ToArray();
                        }
                    }
                }

                else if (!CountryCode.HasValue && PartyId.HasValue)
                {

                    var allLocations = new List<Location>();

                    foreach (var party in _Locations.Values)
                    {
                        if (party.TryGetValue(PartyId.Value, out Dictionary<Location_Id, Location> locations))
                        {
                            allLocations.AddRange(locations.Values);
                        }
                    }

                    return allLocations;

                }

                else if (CountryCode.HasValue && !PartyId.HasValue)
                {
                    if (_Locations.TryGetValue(CountryCode.Value, out Dictionary<Party_Id, Dictionary<Location_Id, Location>> parties))
                    {

                        var allLocations = new List<Location>();

                        foreach (var locations in parties.Values)
                        {
                            allLocations.AddRange(locations.Values);
                        }

                        return allLocations;

                    }
                }

                else
                {

                    var allLocations = new List<Location>();

                    foreach (var party in _Locations.Values)
                    {
                        foreach (var locations in party.Values)
                        {
                            allLocations.AddRange(locations.Values);
                        }
                    }

                    return allLocations;

                }

                return new Location[0];

            }

        }

        #endregion


        #region RemoveLocation(Location)

        public Location RemoveLocation(Location Location)
        {

            if (Location is null)
                throw new ArgumentNullException(nameof(Location), "The given location must not be null!");

            lock (_Locations)
            {

                if (_Locations.TryGetValue(Location.CountryCode, out Dictionary<Party_Id, Dictionary<Location_Id, Location>> parties))
                {

                    if (parties.TryGetValue(Location.PartyId, out Dictionary<Location_Id, Location> locations))
                    {

                        if (locations.ContainsKey(Location.Id))
                        {
                            locations.Remove(Location.Id);
                        }

                        if (!locations.Any())
                            parties.Remove(Location.PartyId);

                    }

                    if (!parties.Any())
                        _Locations.Remove(Location.CountryCode);

                }

                return Location;

            }

        }

        #endregion

        #region RemoveAllLocations()

        /// <summary>
        /// Remove all locations.
        /// </summary>
        public void RemoveAllLocations()
        {

            lock (_Locations)
            {
                _Locations.Clear();
            }

        }

        #endregion

        #region RemoveAllLocations(CountryCode, PartyId)

        /// <summary>
        /// Remove all locations owned by the given party.
        /// </summary>
        /// <param name="CountryCode">The country code of the party.</param>
        /// <param name="PartyId">The identification of the party.</param>
        public void RemoveAllLocations(CountryCode  CountryCode,
                                       Party_Id     PartyId)
        {

            lock (_Locations)
            {

                if (_Locations.TryGetValue(CountryCode, out Dictionary<Party_Id, Dictionary<Location_Id, Location>> parties))
                {
                    if (parties.TryGetValue(PartyId, out Dictionary<Location_Id, Location> locations))
                    {
                        locations.Clear();
                    }
                }

            }

        }

        #endregion

        #endregion

        #region Tariffs

        private readonly Dictionary<CountryCode, Dictionary<Party_Id, Dictionary<Tariff_Id , Tariff>>> _Tariffs;


        #region AddTariff           (Tariff)

        public Tariff AddTariff(Tariff Tariff)
        {

            if (Tariff is null)
                throw new ArgumentNullException(nameof(Tariff), "The given tariff must not be null!");

            lock (_Tariffs)
            {

                if (!_Tariffs.TryGetValue(Tariff.CountryCode, out Dictionary<Party_Id, Dictionary<Tariff_Id, Tariff>> parties))
                {
                    parties = new Dictionary<Party_Id, Dictionary<Tariff_Id, Tariff>>();
                    _Tariffs.Add(Tariff.CountryCode, parties);
                }

                if (!parties.TryGetValue(Tariff.PartyId, out Dictionary<Tariff_Id, Tariff> tariffs))
                {
                    tariffs = new Dictionary<Tariff_Id, Tariff>();
                    parties.Add(Tariff.PartyId, tariffs);
                }

                if (!tariffs.ContainsKey(Tariff.Id))
                {
                    tariffs.Add(Tariff.Id, Tariff);
                    return Tariff;
                }

                throw new ArgumentException("The given tariff already exists!");

            }

        }

        #endregion

        #region AddTariffIfNotExists(Tariff)

        public Tariff AddTariffIfNotExists(Tariff Tariff)
        {

            if (Tariff is null)
                throw new ArgumentNullException(nameof(Tariff), "The given tariff must not be null!");

            lock (_Tariffs)
            {

                if (!_Tariffs.TryGetValue(Tariff.CountryCode, out Dictionary<Party_Id, Dictionary<Tariff_Id, Tariff>> parties))
                {
                    parties = new Dictionary<Party_Id, Dictionary<Tariff_Id, Tariff>>();
                    _Tariffs.Add(Tariff.CountryCode, parties);
                }

                if (!parties.TryGetValue(Tariff.PartyId, out Dictionary<Tariff_Id, Tariff> tariffs))
                {
                    tariffs = new Dictionary<Tariff_Id, Tariff>();
                    parties.Add(Tariff.PartyId, tariffs);
                }

                if (!tariffs.ContainsKey(Tariff.Id))
                    tariffs.Add(Tariff.Id, Tariff);

                return Tariff;

            }

        }

        #endregion

        #region AddOrUpdateTariff   (Tariff)

        public Tariff AddOrUpdateTariff(Tariff Tariff)
        {

            if (Tariff is null)
                throw new ArgumentNullException(nameof(Tariff), "The given tariff must not be null!");

            lock (_Tariffs)
            {

                if (!_Tariffs.TryGetValue(Tariff.CountryCode, out Dictionary<Party_Id, Dictionary<Tariff_Id, Tariff>> parties))
                {
                    parties = new Dictionary<Party_Id, Dictionary<Tariff_Id, Tariff>>();
                    _Tariffs.Add(Tariff.CountryCode, parties);
                }

                if (!parties.TryGetValue(Tariff.PartyId, out Dictionary<Tariff_Id, Tariff> tariffs))
                {
                    tariffs = new Dictionary<Tariff_Id, Tariff>();
                    parties.Add(Tariff.PartyId, tariffs);
                }

                if (tariffs.ContainsKey(Tariff.Id))
                {
                    tariffs.Remove(Tariff.Id);
                }

                tariffs.Add(Tariff.Id, Tariff);
                return Tariff;

            }

        }

        #endregion

        #region UpdateTariff        (Tariff)

        public Tariff UpdateTariff(Tariff Tariff)
        {

            if (Tariff is null)
                throw new ArgumentNullException(nameof(Tariff), "The given tariff must not be null!");

            lock (_Tariffs)
            {

                if (_Tariffs.TryGetValue(Tariff.CountryCode, out Dictionary<Party_Id, Dictionary<Tariff_Id, Tariff>> parties) &&
                    parties. TryGetValue(Tariff.PartyId,     out                      Dictionary<Tariff_Id, Tariff>  tariffs) &&
                    tariffs.ContainsKey(Tariff.Id))
                {
                    tariffs[Tariff.Id] = Tariff;
                    return Tariff;
                }

                return null;

            }

        }

        #endregion


        #region TryGetTariff(CountryCode, PartyId, TariffId,, out Tariff)

        public Boolean TryGetTariff(CountryCode   CountryCode,
                                      Party_Id      PartyId,
                                      Tariff_Id   TariffId,
                                      out Tariff  Tariff)
        {

            lock (_Tariffs)
            {

                if (_Tariffs.TryGetValue(CountryCode, out Dictionary<Party_Id, Dictionary<Tariff_Id, Tariff>> parties))
                {
                    if (parties.TryGetValue(PartyId, out Dictionary<Tariff_Id, Tariff> tariffs))
                    {
                        if (tariffs.TryGetValue(TariffId, out Tariff))
                            return true;
                    }
                }

                Tariff = null;
                return false;

            }

        }

        #endregion

        #region GetTariffs(CountryCode = null, PartyId = null)

        public IEnumerable<Tariff> GetTariffs(CountryCode? CountryCode  = null,
                                                  Party_Id?    PartyId      = null)
        {

            lock (_Tariffs)
            {

                if (CountryCode.HasValue && PartyId.HasValue)
                {
                    if (_Tariffs.TryGetValue(CountryCode.Value, out Dictionary<Party_Id, Dictionary<Tariff_Id, Tariff>> parties))
                    {
                        if (parties.TryGetValue(PartyId.Value, out Dictionary<Tariff_Id, Tariff> tariffs))
                        {
                            return tariffs.Values.ToArray();
                        }
                    }
                }

                else if (!CountryCode.HasValue && PartyId.HasValue)
                {

                    var allTariffs = new List<Tariff>();

                    foreach (var party in _Tariffs.Values)
                    {
                        if (party.TryGetValue(PartyId.Value, out Dictionary<Tariff_Id, Tariff> tariffs))
                        {
                            allTariffs.AddRange(tariffs.Values);
                        }
                    }

                    return allTariffs;

                }

                else if (CountryCode.HasValue && !PartyId.HasValue)
                {
                    if (_Tariffs.TryGetValue(CountryCode.Value, out Dictionary<Party_Id, Dictionary<Tariff_Id, Tariff>> parties))
                    {

                        var allTariffs = new List<Tariff>();

                        foreach (var tariffs in parties.Values)
                        {
                            allTariffs.AddRange(tariffs.Values);
                        }

                        return allTariffs;

                    }
                }

                else
                {

                    var allTariffs = new List<Tariff>();

                    foreach (var party in _Tariffs.Values)
                    {
                        foreach (var tariffs in party.Values)
                        {
                            allTariffs.AddRange(tariffs.Values);
                        }
                    }

                    return allTariffs;

                }

                return new Tariff[0];

            }

        }

        #endregion


        #region RemoveTariff(Tariff)

        public Tariff RemoveTariff(Tariff Tariff)
        {

            if (Tariff is null)
                throw new ArgumentNullException(nameof(Tariff), "The given tariff must not be null!");

            lock (_Tariffs)
            {

                if (_Tariffs.TryGetValue(Tariff.CountryCode, out Dictionary<Party_Id, Dictionary<Tariff_Id, Tariff>> parties))
                {

                    if (parties.TryGetValue(Tariff.PartyId, out Dictionary<Tariff_Id, Tariff> tariffs))
                    {

                        if (tariffs.ContainsKey(Tariff.Id))
                        {
                            tariffs.Remove(Tariff.Id);
                        }

                        if (!tariffs.Any())
                            parties.Remove(Tariff.PartyId);

                    }

                    if (!parties.Any())
                        _Tariffs.Remove(Tariff.CountryCode);

                }

                return Tariff;

            }

        }

        #endregion

        #region RemoveAllTariffs()

        /// <summary>
        /// Remove all tariffs.
        /// </summary>
        public void RemoveAllTariffs()
        {

            lock (_Tariffs)
            {
                _Tariffs.Clear();
            }

        }

        #endregion

        #region RemoveAllTariffs(CountryCode, PartyId)

        /// <summary>
        /// Remove all tariffs owned by the given party.
        /// </summary>
        /// <param name="CountryCode">The country code of the party.</param>
        /// <param name="PartyId">The identification of the party.</param>
        public void RemoveAllTariffs(CountryCode  CountryCode,
                                     Party_Id     PartyId)
        {

            lock (_Tariffs)
            {

                if (_Tariffs.TryGetValue(CountryCode, out Dictionary<Party_Id, Dictionary<Tariff_Id, Tariff>> parties))
                {
                    if (parties.TryGetValue(PartyId, out Dictionary<Tariff_Id, Tariff> tariffs))
                    {
                        tariffs.Clear();
                    }
                }

            }

        }

        #endregion

        #endregion

        #region Sessions

        private readonly Dictionary<CountryCode, Dictionary<Party_Id, Dictionary<Session_Id , Session>>> _Sessions;


        #region AddSession           (Session)

        public Session AddSession(Session Session)
        {

            if (Session is null)
                throw new ArgumentNullException(nameof(Session), "The given session must not be null!");

            lock (_Sessions)
            {

                if (!_Sessions.TryGetValue(Session.CountryCode, out Dictionary<Party_Id, Dictionary<Session_Id, Session>> parties))
                {
                    parties = new Dictionary<Party_Id, Dictionary<Session_Id, Session>>();
                    _Sessions.Add(Session.CountryCode, parties);
                }

                if (!parties.TryGetValue(Session.PartyId, out Dictionary<Session_Id, Session> sessions))
                {
                    sessions = new Dictionary<Session_Id, Session>();
                    parties.Add(Session.PartyId, sessions);
                }

                if (!sessions.ContainsKey(Session.Id))
                {
                    sessions.Add(Session.Id, Session);
                    return Session;
                }

                throw new ArgumentException("The given session already exists!");

            }

        }

        #endregion

        #region AddSessionIfNotExists(Session)

        public Session AddSessionIfNotExists(Session Session)
        {

            if (Session is null)
                throw new ArgumentNullException(nameof(Session), "The given session must not be null!");

            lock (_Sessions)
            {

                if (!_Sessions.TryGetValue(Session.CountryCode, out Dictionary<Party_Id, Dictionary<Session_Id, Session>> parties))
                {
                    parties = new Dictionary<Party_Id, Dictionary<Session_Id, Session>>();
                    _Sessions.Add(Session.CountryCode, parties);
                }

                if (!parties.TryGetValue(Session.PartyId, out Dictionary<Session_Id, Session> sessions))
                {
                    sessions = new Dictionary<Session_Id, Session>();
                    parties.Add(Session.PartyId, sessions);
                }

                if (!sessions.ContainsKey(Session.Id))
                    sessions.Add(Session.Id, Session);

                return Session;

            }

        }

        #endregion

        #region AddOrUpdateSession   (Session)

        public Session AddOrUpdateSession(Session Session)
        {

            if (Session is null)
                throw new ArgumentNullException(nameof(Session), "The given session must not be null!");

            lock (_Sessions)
            {

                if (!_Sessions.TryGetValue(Session.CountryCode, out Dictionary<Party_Id, Dictionary<Session_Id, Session>> parties))
                {
                    parties = new Dictionary<Party_Id, Dictionary<Session_Id, Session>>();
                    _Sessions.Add(Session.CountryCode, parties);
                }

                if (!parties.TryGetValue(Session.PartyId, out Dictionary<Session_Id, Session> sessions))
                {
                    sessions = new Dictionary<Session_Id, Session>();
                    parties.Add(Session.PartyId, sessions);
                }

                if (sessions.ContainsKey(Session.Id))
                {
                    sessions.Remove(Session.Id);
                }

                sessions.Add(Session.Id, Session);
                return Session;

            }

        }

        #endregion

        #region UpdateSession        (Session)

        public Session UpdateSession(Session Session)
        {

            if (Session is null)
                throw new ArgumentNullException(nameof(Session), "The given session must not be null!");

            lock (_Sessions)
            {

                if (_Sessions.TryGetValue(Session.CountryCode, out Dictionary<Party_Id, Dictionary<Session_Id, Session>> parties)  &&
                    parties.  TryGetValue(Session.PartyId,     out                      Dictionary<Session_Id, Session>  sessions) &&
                    sessions.ContainsKey(Session.Id))
                {
                    sessions[Session.Id] = Session;
                    return Session;
                }

                return null;

            }

        }

        #endregion


        #region TryGetSession(CountryCode, PartyId, SessionId,, out Session)

        public Boolean TryGetSession(CountryCode   CountryCode,
                                      Party_Id      PartyId,
                                      Session_Id   SessionId,
                                      out Session  Session)
        {

            lock (_Sessions)
            {

                if (_Sessions.TryGetValue(CountryCode, out Dictionary<Party_Id, Dictionary<Session_Id, Session>> parties))
                {
                    if (parties.TryGetValue(PartyId, out Dictionary<Session_Id, Session> sessions))
                    {
                        if (sessions.TryGetValue(SessionId, out Session))
                            return true;
                    }
                }

                Session = null;
                return false;

            }

        }

        #endregion

        #region GetSessions(CountryCode = null, PartyId = null)

        public IEnumerable<Session> GetSessions(CountryCode? CountryCode  = null,
                                                  Party_Id?    PartyId      = null)
        {

            lock (_Sessions)
            {

                if (CountryCode.HasValue && PartyId.HasValue)
                {
                    if (_Sessions.TryGetValue(CountryCode.Value, out Dictionary<Party_Id, Dictionary<Session_Id, Session>> parties))
                    {
                        if (parties.TryGetValue(PartyId.Value, out Dictionary<Session_Id, Session> sessions))
                        {
                            return sessions.Values.ToArray();
                        }
                    }
                }

                else if (!CountryCode.HasValue && PartyId.HasValue)
                {

                    var allSessions = new List<Session>();

                    foreach (var party in _Sessions.Values)
                    {
                        if (party.TryGetValue(PartyId.Value, out Dictionary<Session_Id, Session> sessions))
                        {
                            allSessions.AddRange(sessions.Values);
                        }
                    }

                    return allSessions;

                }

                else if (CountryCode.HasValue && !PartyId.HasValue)
                {
                    if (_Sessions.TryGetValue(CountryCode.Value, out Dictionary<Party_Id, Dictionary<Session_Id, Session>> parties))
                    {

                        var allSessions = new List<Session>();

                        foreach (var sessions in parties.Values)
                        {
                            allSessions.AddRange(sessions.Values);
                        }

                        return allSessions;

                    }
                }

                else
                {

                    var allSessions = new List<Session>();

                    foreach (var party in _Sessions.Values)
                    {
                        foreach (var sessions in party.Values)
                        {
                            allSessions.AddRange(sessions.Values);
                        }
                    }

                    return allSessions;

                }

                return new Session[0];

            }

        }

        #endregion


        #region RemoveSession(Session)

        public Session RemoveSession(Session Session)
        {

            if (Session is null)
                throw new ArgumentNullException(nameof(Session), "The given session must not be null!");

            lock (_Sessions)
            {

                if (_Sessions.TryGetValue(Session.CountryCode, out Dictionary<Party_Id, Dictionary<Session_Id, Session>> parties))
                {

                    if (parties.TryGetValue(Session.PartyId, out Dictionary<Session_Id, Session> sessions))
                    {

                        if (sessions.ContainsKey(Session.Id))
                        {
                            sessions.Remove(Session.Id);
                        }

                        if (!sessions.Any())
                            parties.Remove(Session.PartyId);

                    }

                    if (!parties.Any())
                        _Sessions.Remove(Session.CountryCode);

                }

                return Session;

            }

        }

        #endregion

        #region RemoveAllSessions()

        /// <summary>
        /// Remove all sessions.
        /// </summary>
        public void RemoveAllSessions()
        {

            lock (_Sessions)
            {
                _Sessions.Clear();
            }

        }

        #endregion

        #region RemoveAllSessions(CountryCode, PartyId)

        /// <summary>
        /// Remove all sessions owned by the given party.
        /// </summary>
        /// <param name="CountryCode">The country code of the party.</param>
        /// <param name="PartyId">The identification of the party.</param>
        public void RemoveAllSessions(CountryCode  CountryCode,
                                      Party_Id     PartyId)
        {

            lock (_Sessions)
            {

                if (_Sessions.TryGetValue(CountryCode, out Dictionary<Party_Id, Dictionary<Session_Id, Session>> parties))
                {
                    if (parties.TryGetValue(PartyId, out Dictionary<Session_Id, Session> sessions))
                    {
                        sessions.Clear();
                    }
                }

            }

        }

        #endregion

        #endregion

        #region Tokens

        private readonly Dictionary<CountryCode, Dictionary<Party_Id, Dictionary<Token_Id, TokenStatus>>> _Tokens;


        #region AddToken           (Token, Status = AllowedTypes.ALLOWED)

        public Token AddToken(Token         Token,
                              AllowedTypes  Status = AllowedTypes.ALLOWED)
        {

            if (Token is null)
                throw new ArgumentNullException(nameof(Token), "The given token must not be null!");

            lock (_Tokens)
            {

                if (!_Tokens.TryGetValue(Token.CountryCode, out Dictionary<Party_Id, Dictionary<Token_Id, TokenStatus>> parties))
                {
                    parties = new Dictionary<Party_Id, Dictionary<Token_Id, TokenStatus>>();
                    _Tokens.Add(Token.CountryCode, parties);
                }

                if (!parties.TryGetValue(Token.PartyId, out Dictionary<Token_Id, TokenStatus> tokens))
                {
                    tokens = new Dictionary<Token_Id, TokenStatus>();
                    parties.Add(Token.PartyId, tokens);
                }

                if (!tokens.ContainsKey(Token.Id))
                {
                    tokens.Add(Token.Id, new TokenStatus(Token, Status));
                    return Token;
                }

                throw new ArgumentException("The given token already exists!");

            }

        }

        #endregion

        #region AddTokenIfNotExists(Token, Status = AllowedTypes.ALLOWED)

        public Token AddTokenIfNotExists(Token         Token,
                                         AllowedTypes  Status = AllowedTypes.ALLOWED)
        {

            if (Token is null)
                throw new ArgumentNullException(nameof(Token), "The given token must not be null!");

            lock (_Tokens)
            {

                if (!_Tokens.TryGetValue(Token.CountryCode, out Dictionary<Party_Id, Dictionary<Token_Id, TokenStatus>> parties))
                {
                    parties = new Dictionary<Party_Id, Dictionary<Token_Id, TokenStatus>>();
                    _Tokens.Add(Token.CountryCode, parties);
                }

                if (!parties.TryGetValue(Token.PartyId, out Dictionary<Token_Id, TokenStatus> tokens))
                {
                    tokens = new Dictionary<Token_Id, TokenStatus>();
                    parties.Add(Token.PartyId, tokens);
                }

                if (!tokens.ContainsKey(Token.Id))
                    tokens.Add(Token.Id, new TokenStatus(Token, Status));

                return Token;

            }

        }

        #endregion

        #region AddOrUpdateToken   (Token, Status = AllowedTypes.ALLOWED)

        public Token AddOrUpdateToken(Token         Token,
                                      AllowedTypes  Status = AllowedTypes.ALLOWED)
        {

            if (Token is null)
                throw new ArgumentNullException(nameof(Token), "The given token must not be null!");

            lock (_Tokens)
            {

                if (!_Tokens.TryGetValue(Token.CountryCode, out Dictionary<Party_Id, Dictionary<Token_Id, TokenStatus>> parties))
                {
                    parties = new Dictionary<Party_Id, Dictionary<Token_Id, TokenStatus>>();
                    _Tokens.Add(Token.CountryCode, parties);
                }

                if (!parties.TryGetValue(Token.PartyId, out Dictionary<Token_Id, TokenStatus> tokens))
                {
                    tokens = new Dictionary<Token_Id, TokenStatus>();
                    parties.Add(Token.PartyId, tokens);
                }

                if (tokens.ContainsKey(Token.Id))
                {
                    tokens.Remove(Token.Id);
                }

                tokens.Add(Token.Id, new TokenStatus(Token, Status));
                return Token;

            }

        }

        #endregion


        #region TryGetToken(CountryCode, PartyId, TokenId, out TokenWithStatus)

        public Boolean TryGetToken(CountryCode      CountryCode,
                                   Party_Id         PartyId,
                                   Token_Id         TokenId,
                                   out TokenStatus  TokenWithStatus)
        {

            lock (_Tokens)
            {

                if (_Tokens.TryGetValue(CountryCode, out Dictionary<Party_Id, Dictionary<Token_Id, TokenStatus>> parties))
                {
                    if (parties.TryGetValue(PartyId, out Dictionary<Token_Id, TokenStatus> tokens))
                    {
                        if (tokens.TryGetValue(TokenId, out TokenWithStatus))
                            return true;
                    }
                }

                TokenWithStatus = default;
                return false;

            }

        }

        #endregion

        #region GetTokens(CountryCode = null, PartyId = null)

        public IEnumerable<TokenStatus> GetTokens(CountryCode?  CountryCode   = null,
                                                  Party_Id?     PartyId       = null)
        {

            lock (_Tokens)
            {

                if (CountryCode.HasValue && PartyId.HasValue)
                {
                    if (_Tokens.TryGetValue(CountryCode.Value, out Dictionary<Party_Id, Dictionary<Token_Id, TokenStatus>> parties))
                    {
                        if (parties.TryGetValue(PartyId.Value, out Dictionary<Token_Id, TokenStatus> tokens))
                        {
                            return tokens.Values.ToArray();
                        }
                    }
                }

                else if (!CountryCode.HasValue && PartyId.HasValue)
                {

                    var allTokens = new List<TokenStatus>();

                    foreach (var party in _Tokens.Values)
                    {
                        if (party.TryGetValue(PartyId.Value, out Dictionary<Token_Id, TokenStatus> tokens))
                        {
                            allTokens.AddRange(tokens.Values);
                        }
                    }

                    return allTokens;

                }

                else if (CountryCode.HasValue && !PartyId.HasValue)
                {
                    if (_Tokens.TryGetValue(CountryCode.Value, out Dictionary<Party_Id, Dictionary<Token_Id, TokenStatus>> parties))
                    {

                        var allTokens = new List<TokenStatus>();

                        foreach (var tokens in parties.Values)
                        {
                            allTokens.AddRange(tokens.Values);
                        }

                        return allTokens;

                    }
                }

                else
                {

                    var allTokens = new List<TokenStatus>();

                    foreach (var party in _Tokens.Values)
                    {
                        foreach (var tokens in party.Values)
                        {
                            allTokens.AddRange(tokens.Values);
                        }
                    }

                    return allTokens;

                }

                return new TokenStatus[0];

            }

        }

        #endregion


        #region RemoveToken(TokenId)

        public Token RemoveToken(Token_Id TokenId)
        {

            lock (_Tokens)
            {

                Token foundToken = null;

                foreach (var parties in _Tokens.Values)
                {

                    foreach (var tokens in parties.Values)
                    {
                        if (tokens.TryGetValue(TokenId, out TokenStatus tokenStatus))
                        {
                            foundToken = tokenStatus.Token;
                            break;
                        }
                    }

                    if (foundToken != null)
                        break;

                }

                return foundToken != null
                           ? RemoveToken(foundToken)
                           : null;

            }

        }

        #endregion

        #region RemoveToken(Token)

        public Token RemoveToken(Token Token)
        {

            if (Token is null)
                throw new ArgumentNullException(nameof(Token), "The given token must not be null!");

            lock (_Tokens)
            {

                if (_Tokens.TryGetValue(Token.CountryCode, out Dictionary<Party_Id, Dictionary<Token_Id, TokenStatus>> parties))
                {

                    if (parties.TryGetValue(Token.PartyId, out Dictionary<Token_Id, TokenStatus> tokens))
                    {

                        if (tokens.ContainsKey(Token.Id))
                        {
                            tokens.Remove(Token.Id);
                        }

                        if (!tokens.Any())
                            parties.Remove(Token.PartyId);

                    }

                    if (!parties.Any())
                        _Tokens.Remove(Token.CountryCode);

                }

                return Token;

            }

        }

        #endregion

        #region RemoveAllTokens()

        /// <summary>
        /// Remove all tokens.
        /// </summary>
        public void RemoveAllTokens()
        {

            lock (_Tokens)
            {
                _Tokens.Clear();
            }

        }

        #endregion

        #region RemoveAllTokens(CountryCode, PartyId)

        /// <summary>
        /// Remove all tokens owned by the given party.
        /// </summary>
        /// <param name="CountryCode">The country code of the party.</param>
        /// <param name="PartyId">The identification of the party.</param>
        public void RemoveAllTokens(CountryCode  CountryCode,
                                    Party_Id     PartyId)
        {

            lock (_Tokens)
            {

                if (_Tokens.TryGetValue(CountryCode, out Dictionary<Party_Id, Dictionary<Token_Id, TokenStatus>> parties))
                {
                    if (parties.TryGetValue(PartyId, out Dictionary<Token_Id, TokenStatus> tokens))
                    {
                        tokens.Clear();
                    }
                }

            }

        }

        #endregion

        #endregion

        #region CDRs

        private readonly Dictionary<CountryCode, Dictionary<Party_Id, Dictionary<CDR_Id, CDR>>> _CDRs;


        #region AddCDR           (CDR)

        public CDR AddCDR(CDR CDR)
        {

            if (CDR is null)
                throw new ArgumentNullException(nameof(CDR), "The given charge detail record must not be null!");

            lock (_CDRs)
            {

                if (!_CDRs.TryGetValue(CDR.CountryCode, out Dictionary<Party_Id, Dictionary<CDR_Id, CDR>> parties))
                {
                    parties = new Dictionary<Party_Id, Dictionary<CDR_Id, CDR>>();
                    _CDRs.Add(CDR.CountryCode, parties);
                }

                if (!parties.TryGetValue(CDR.PartyId, out Dictionary<CDR_Id, CDR> CDRs))
                {
                    CDRs = new Dictionary<CDR_Id, CDR>();
                    parties.Add(CDR.PartyId, CDRs);
                }

                if (!CDRs.ContainsKey(CDR.Id))
                {
                    CDRs.Add(CDR.Id, CDR);
                    return CDR;
                }

                throw new ArgumentException("The given charge detail record already exists!");

            }

        }

        #endregion

        #region AddCDRIfNotExists(CDR)

        public CDR AddCDRIfNotExists(CDR CDR)
        {

            if (CDR is null)
                throw new ArgumentNullException(nameof(CDR), "The given charge detail record must not be null!");

            lock (_CDRs)
            {

                if (!_CDRs.TryGetValue(CDR.CountryCode, out Dictionary<Party_Id, Dictionary<CDR_Id, CDR>> parties))
                {
                    parties = new Dictionary<Party_Id, Dictionary<CDR_Id, CDR>>();
                    _CDRs.Add(CDR.CountryCode, parties);
                }

                if (!parties.TryGetValue(CDR.PartyId, out Dictionary<CDR_Id, CDR> CDRs))
                {
                    CDRs = new Dictionary<CDR_Id, CDR>();
                    parties.Add(CDR.PartyId, CDRs);
                }

                if (!CDRs.ContainsKey(CDR.Id))
                    CDRs.Add(CDR.Id, CDR);

                return CDR;

            }

        }

        #endregion

        #region AddOrUpdateCDR   (CDR)

        public CDR AddOrUpdateCDR(CDR CDR)
        {

            if (CDR is null)
                throw new ArgumentNullException(nameof(CDR), "The given charge detail record must not be null!");

            lock (_CDRs)
            {

                if (!_CDRs.TryGetValue(CDR.CountryCode, out Dictionary<Party_Id, Dictionary<CDR_Id, CDR>> parties))
                {
                    parties = new Dictionary<Party_Id, Dictionary<CDR_Id, CDR>>();
                    _CDRs.Add(CDR.CountryCode, parties);
                }

                if (!parties.TryGetValue(CDR.PartyId, out Dictionary<CDR_Id, CDR> CDRs))
                {
                    CDRs = new Dictionary<CDR_Id, CDR>();
                    parties.Add(CDR.PartyId, CDRs);
                }

                if (CDRs.ContainsKey(CDR.Id))
                {
                    CDRs.Remove(CDR.Id);
                }

                CDRs.Add(CDR.Id, CDR);
                return CDR;

            }

        }

        #endregion

        #region UpdateCDR        (CDR)

        public CDR UpdateCDR(CDR CDR)
        {

            if (CDR is null)
                throw new ArgumentNullException(nameof(CDR), "The given charge detail record must not be null!");

            lock (_CDRs)
            {

                if (_CDRs.  TryGetValue(CDR.CountryCode, out Dictionary<Party_Id, Dictionary<CDR_Id, CDR>> parties) &&
                    parties.TryGetValue(CDR.PartyId,     out                      Dictionary<CDR_Id, CDR>  CDRs)    &&
                    CDRs.ContainsKey(CDR.Id))
                {
                    CDRs[CDR.Id] = CDR;
                    return CDR;
                }

                return null;

            }

        }

        #endregion


        #region TryGetCDR(CountryCode, PartyId, CDRId,, out CDR)

        public Boolean TryGetCDR(CountryCode   CountryCode,
                                      Party_Id      PartyId,
                                      CDR_Id   CDRId,
                                      out CDR  CDR)
        {

            lock (_CDRs)
            {

                if (_CDRs.TryGetValue(CountryCode, out Dictionary<Party_Id, Dictionary<CDR_Id, CDR>> parties))
                {
                    if (parties.TryGetValue(PartyId, out Dictionary<CDR_Id, CDR> CDRs))
                    {
                        if (CDRs.TryGetValue(CDRId, out CDR))
                            return true;
                    }
                }

                CDR = null;
                return false;

            }

        }

        #endregion

        #region GetCDRs(CountryCode = null, PartyId = null)

        public IEnumerable<CDR> GetCDRs(CountryCode? CountryCode  = null,
                                                  Party_Id?    PartyId      = null)
        {

            lock (_CDRs)
            {

                if (CountryCode.HasValue && PartyId.HasValue)
                {
                    if (_CDRs.TryGetValue(CountryCode.Value, out Dictionary<Party_Id, Dictionary<CDR_Id, CDR>> parties))
                    {
                        if (parties.TryGetValue(PartyId.Value, out Dictionary<CDR_Id, CDR> CDRs))
                        {
                            return CDRs.Values.ToArray();
                        }
                    }
                }

                else if (!CountryCode.HasValue && PartyId.HasValue)
                {

                    var allCDRs = new List<CDR>();

                    foreach (var party in _CDRs.Values)
                    {
                        if (party.TryGetValue(PartyId.Value, out Dictionary<CDR_Id, CDR> CDRs))
                        {
                            allCDRs.AddRange(CDRs.Values);
                        }
                    }

                    return allCDRs;

                }

                else if (CountryCode.HasValue && !PartyId.HasValue)
                {
                    if (_CDRs.TryGetValue(CountryCode.Value, out Dictionary<Party_Id, Dictionary<CDR_Id, CDR>> parties))
                    {

                        var allCDRs = new List<CDR>();

                        foreach (var CDRs in parties.Values)
                        {
                            allCDRs.AddRange(CDRs.Values);
                        }

                        return allCDRs;

                    }
                }

                else
                {

                    var allCDRs = new List<CDR>();

                    foreach (var party in _CDRs.Values)
                    {
                        foreach (var CDRs in party.Values)
                        {
                            allCDRs.AddRange(CDRs.Values);
                        }
                    }

                    return allCDRs;

                }

                return new CDR[0];

            }

        }

        #endregion


        #region RemoveCDR(CDR)

        public CDR RemoveCDR(CDR CDR)
        {

            if (CDR is null)
                throw new ArgumentNullException(nameof(CDR), "The given charge detail record must not be null!");

            lock (_CDRs)
            {

                if (_CDRs.TryGetValue(CDR.CountryCode, out Dictionary<Party_Id, Dictionary<CDR_Id, CDR>> parties))
                {

                    if (parties.TryGetValue(CDR.PartyId, out Dictionary<CDR_Id, CDR> CDRs))
                    {

                        if (CDRs.ContainsKey(CDR.Id))
                        {
                            CDRs.Remove(CDR.Id);
                        }

                        if (!CDRs.Any())
                            parties.Remove(CDR.PartyId);

                    }

                    if (!parties.Any())
                        _CDRs.Remove(CDR.CountryCode);

                }

                return CDR;

            }

        }

        #endregion

        #region RemoveAllCDRs()

        /// <summary>
        /// Remove all CDRs.
        /// </summary>
        public void RemoveAllCDRs()
        {

            lock (_CDRs)
            {
                _CDRs.Clear();
            }

        }

        #endregion

        #region RemoveAllCDRs(CountryCode, PartyId)

        /// <summary>
        /// Remove all CDRs owned by the given party.
        /// </summary>
        /// <param name="CountryCode">The country code of the party.</param>
        /// <param name="PartyId">The identification of the party.</param>
        public void RemoveAllCDRs(CountryCode  CountryCode,
                                  Party_Id     PartyId)
        {

            lock (_CDRs)
            {

                if (_CDRs.TryGetValue(CountryCode, out Dictionary<Party_Id, Dictionary<CDR_Id, CDR>> parties))
                {
                    if (parties.TryGetValue(PartyId, out Dictionary<CDR_Id, CDR> CDRs))
                    {
                        CDRs.Clear();
                    }
                }

            }

        }

        #endregion

        #endregion



        #region Start()

        public void Start()
        {

            lock (HTTPServer)
            {

                if (!HTTPServer.IsStarted)
                    HTTPServer.Start();

                #region Send 'Open Data API restarted'-e-mail...

                //var Message0 = new HTMLEMailBuilder() {
                //    From        = _APIEMailAddress,
                //    To          = _APIAdminEMail,
                //    Subject     = "Open Data API '" + _ServiceName + "' restarted! at " + DateTime.Now.ToString(),
                //    PlainText   = "Open Data API '" + _ServiceName + "' restarted! at " + DateTime.Now.ToString(),
                //    HTMLText    = "Open Data API <b>'" + _ServiceName + "'</b> restarted! at " + DateTime.Now.ToString(),
                //    Passphrase  = _APIPassphrase
                //};
                //
                //var SMTPTask = _APISMTPClient.Send(Message0);
                //SMTPTask.Wait();

                //var r = SMTPTask.Result;

                #endregion

                //SendStarted(this, DateTime.Now);

            }

        }

        #endregion

        #region Shutdown(Message = null, Wait = true)

        public void Shutdown(String Message = null, Boolean Wait = true)
        {

            lock (HTTPServer)
            {

                HTTPServer.Shutdown(Message, Wait);

                //SendCompleted(this, DateTime.UtcNow, Message);

            }

        }

        #endregion

    }

}
