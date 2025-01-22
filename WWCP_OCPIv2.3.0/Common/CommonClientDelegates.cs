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

using cloud.charging.open.protocols.OCPI;
using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_3_0.HTTP
{

    #region OnGetVersionsRequest/-Response

    /// <summary>
    /// A delegate called whenever a get versions request will be send.
    /// </summary>
    public delegate Task OnGetVersionsRequestDelegate(DateTime                                    LogTimestamp,
                                                      DateTime                                    RequestTimestamp,
                                                      CommonClient                                Sender,
                                                      String                                      SenderId,
                                                      EventTracking_Id                            EventTrackingId,

                                                      Request_Id?                                 RequestId,
                                                      Correlation_Id?                             CorrelationId,

                                                      TimeSpan                                    RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a get versions request had been received.
    /// </summary>
    public delegate Task OnGetVersionsResponseDelegate(DateTime                                    LogTimestamp,
                                                       DateTime                                    RequestTimestamp,
                                                       CommonClient                                Sender,
                                                       String                                      SenderId,
                                                       EventTracking_Id                            EventTrackingId,

                                                       Request_Id?                                 RequestId,
                                                       Correlation_Id?                             CorrelationId,

                                                       TimeSpan                                    RequestTimeout,
                                                       IEnumerable<VersionInformation>             Results,
                                                       TimeSpan                                    Duration);

    #endregion

    #region OnGetVersionDetailsRequest/-Response

    /// <summary>
    /// A delegate called whenever a get version details request will be send.
    /// </summary>
    public delegate Task OnGetVersionDetailsRequestDelegate(DateTime                                    LogTimestamp,
                                                            DateTime                                    RequestTimestamp,
                                                            CommonClient                                Sender,
                                                            String                                      SenderId,
                                                            EventTracking_Id                            EventTrackingId,

                                                            Version_Id?                                 VersionId,
                                                            Boolean                                     SetAsDefaultVersion,
                                                            Request_Id?                                 RequestId,
                                                            Correlation_Id?                             CorrelationId,

                                                            TimeSpan                                    RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a get version details request had been received.
    /// </summary>
    public delegate Task OnGetVersionDetailsResponseDelegate(DateTime                                    LogTimestamp,
                                                             DateTime                                    RequestTimestamp,
                                                             CommonClient                                Sender,
                                                             String                                      SenderId,
                                                             EventTracking_Id                            EventTrackingId,

                                                             Version_Id?                                 VersionId,
                                                             Boolean                                     SetAsDefaultVersion,
                                                             Request_Id?                                 RequestId,
                                                             Correlation_Id?                             CorrelationId,

                                                             TimeSpan                                    RequestTimeout,
                                                             VersionDetail?                              Result,
                                                             TimeSpan                                    Duration);

    #endregion


    #region OnGetCredentialsRequest/-Response

    /// <summary>
    /// A delegate called whenever a get credentials request will be send.
    /// </summary>
    public delegate Task OnGetCredentialsRequestDelegate(DateTime                                    LogTimestamp,
                                                         DateTime                                    RequestTimestamp,
                                                         CommonClient                                Sender,
                                                         String                                      SenderId,
                                                         EventTracking_Id                            EventTrackingId,

                                                         Request_Id?                                 RequestId,
                                                         Correlation_Id?                             CorrelationId,

                                                         Version_Id?                                 VersionId,

                                                         TimeSpan                                    RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a get credentials request had been received.
    /// </summary>
    public delegate Task OnGetCredentialsResponseDelegate(DateTime                                    LogTimestamp,
                                                          DateTime                                    RequestTimestamp,
                                                          CommonClient                                Sender,
                                                          String                                      SenderId,
                                                          EventTracking_Id                            EventTrackingId,

                                                          Request_Id?                                 RequestId,
                                                          Correlation_Id?                             CorrelationId,

                                                          Version_Id?                                 VersionId,

                                                          TimeSpan                                    RequestTimeout,
                                                          Credentials?                                Result,
                                                          TimeSpan                                    Duration);

    #endregion

    #region OnPostCredentialsRequest/-Response

    /// <summary>
    /// A delegate called whenever a put credentials request will be send.
    /// </summary>
    public delegate Task OnPostCredentialsRequestDelegate(DateTime                                    LogTimestamp,
                                                          DateTime                                    RequestTimestamp,
                                                          CommonClient                                Sender,
                                                          String                                      SenderId,
                                                          EventTracking_Id                            EventTrackingId,

                                                          Request_Id?                                 RequestId,
                                                          Correlation_Id?                             CorrelationId,

                                                          Version_Id?                                 VersionId,
                                                          Credentials                                 Credentials,

                                                          TimeSpan                                    RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a put credentials request had been received.
    /// </summary>
    public delegate Task OnPostCredentialsResponseDelegate(DateTime                                    LogTimestamp,
                                                           DateTime                                    RequestTimestamp,
                                                           CommonClient                                Sender,
                                                           String                                      SenderId,
                                                           EventTracking_Id                            EventTrackingId,

                                                           Request_Id?                                 RequestId,
                                                           Correlation_Id?                             CorrelationId,

                                                           Version_Id?                                 VersionId,
                                                           Credentials                                 Credentials,

                                                           TimeSpan                                    RequestTimeout,
                                                           Credentials?                                Result,
                                                           TimeSpan                                    Duration);

    #endregion

    #region OnPutCredentialsRequest/-Response

    /// <summary>
    /// A delegate called whenever a put credentials request will be send.
    /// </summary>
    public delegate Task OnPutCredentialsRequestDelegate(DateTime                                    LogTimestamp,
                                                         DateTime                                    RequestTimestamp,
                                                         CommonClient                                Sender,
                                                         String                                      SenderId,
                                                         EventTracking_Id                            EventTrackingId,

                                                         Request_Id?                                 RequestId,
                                                         Correlation_Id?                             CorrelationId,

                                                         Version_Id?                                 VersionId,
                                                         Credentials                                 Credentials,

                                                         TimeSpan                                    RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a put credentials request had been received.
    /// </summary>
    public delegate Task OnPutCredentialsResponseDelegate(DateTime                                    LogTimestamp,
                                                          DateTime                                    RequestTimestamp,
                                                          CommonClient                                Sender,
                                                          String                                      SenderId,
                                                          EventTracking_Id                            EventTrackingId,

                                                          Request_Id?                                 RequestId,
                                                          Correlation_Id?                             CorrelationId,

                                                          Version_Id?                                 VersionId,
                                                          Credentials                                 Credentials,

                                                          TimeSpan                                    RequestTimeout,
                                                          Credentials?                                Result,
                                                          TimeSpan                                    Duration);

    #endregion

    #region OnDeleteCredentialsRequest/-Response

    /// <summary>
    /// A delegate called whenever a put credentials request will be send.
    /// </summary>
    public delegate Task OnDeleteCredentialsRequestDelegate(DateTime                                    LogTimestamp,
                                                            DateTime                                    RequestTimestamp,
                                                            CommonClient                                Sender,
                                                            String                                      SenderId,
                                                            EventTracking_Id                            EventTrackingId,

                                                            Request_Id?                                 RequestId,
                                                            Correlation_Id?                             CorrelationId,

                                                            Version_Id?                                 VersionId,

                                                            TimeSpan                                    RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a put credentials request had been received.
    /// </summary>
    public delegate Task OnDeleteCredentialsResponseDelegate(DateTime                                    LogTimestamp,
                                                             DateTime                                    RequestTimestamp,
                                                             CommonClient                                Sender,
                                                             String                                      SenderId,
                                                             EventTracking_Id                            EventTrackingId,

                                                             Request_Id?                                 RequestId,
                                                             Correlation_Id?                             CorrelationId,

                                                             Version_Id?                                 VersionId,

                                                             TimeSpan                                    RequestTimeout,
                                                             TimeSpan                                    Duration);

    #endregion


    #region OnRegisterRequest/-Response

    /// <summary>
    /// A delegate called whenever a registration request will be send.
    /// </summary>
    public delegate Task OnRegisterRequestDelegate (DateTime                                    LogTimestamp,
                                                    DateTime                                    RequestTimestamp,
                                                    CommonClient                                Sender,
                                                    String                                      SenderId,
                                                    EventTracking_Id                            EventTrackingId,

                                                    Request_Id?                                 RequestId,
                                                    Correlation_Id?                             CorrelationId,

                                                    Version_Id?                                 VersionId,

                                                    TimeSpan                                    RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a registration request had been received.
    /// </summary>
    public delegate Task OnRegisterResponseDelegate(DateTime                                    LogTimestamp,
                                                    DateTime                                    RequestTimestamp,
                                                    CommonClient                                Sender,
                                                    String                                      SenderId,
                                                    EventTracking_Id                            EventTrackingId,

                                                    Request_Id?                                 RequestId,
                                                    Correlation_Id?                             CorrelationId,

                                                    Version_Id?                                 VersionId,

                                                    TimeSpan                                    RequestTimeout,
                                                    Credentials?                                Result,
                                                    TimeSpan                                    Duration);

    #endregion


}
