﻿/*
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

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv3_0.HTTP
{

    /// <summary>
    /// An async event notifying about OCPI responses.
    /// </summary>
    public sealed class OCPIResponseLogEvent
    {

        #region Data

        private readonly List<OCPIResponseLogHandler> subscribers;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new async event notifying about OCPI responses.
        /// </summary>
        public OCPIResponseLogEvent()
        {
            subscribers = new List<OCPIResponseLogHandler>();
        }

        #endregion


        #region + / Add

        public static OCPIResponseLogEvent operator + (OCPIResponseLogEvent    ResponseLogEvent,
                                                       OCPIResponseLogHandler  ResponseLogHandler)
        {

            ResponseLogEvent.Add(ResponseLogHandler);
            //ResponseLogEvent.Add((timestamp, api, request, response) => ResponseLogHandler(timestamp, api, request, response));

            return ResponseLogEvent;

        }

        public OCPIResponseLogEvent Add(OCPIResponseLogHandler ResponseLogHandler)
        {

            lock (subscribers)
            {
                subscribers.Add(ResponseLogHandler);
            }

            return this;

        }

        #endregion

        #region - / Remove

        public static OCPIResponseLogEvent operator - (OCPIResponseLogEvent    ResponseLogEvent,
                                                       OCPIResponseLogHandler  ResponseLogHandler)
        {

            ResponseLogEvent.Remove(ResponseLogHandler);

            return ResponseLogEvent;

        }

        public OCPIResponseLogEvent Remove(OCPIResponseLogHandler ResponseLogHandler)
        {

            lock (subscribers)
            {
                subscribers.Remove(ResponseLogHandler);
            }

            return this;

        }

        #endregion


        #region InvokeAsync(ServerTimestamp, OCPIAPI, Request, Response)

        /// <summary>
        /// Call all subscribers sequentially.
        /// </summary>
        /// <param name="ServerTimestamp">The timestamp of the event.</param>
        /// <param name="OCPIAPI">The sending OCPI/HTTP API.</param>
        /// <param name="Request">The incoming request.</param>
        /// <param name="Response">The outgoing response.</param>
        public async Task InvokeAsync(DateTime      ServerTimestamp,
                                      HTTPAPI       OCPIAPI,
                                      OCPIRequest   Request,
                                      OCPIResponse  Response)
        {

            OCPIResponseLogHandler[] invocationList;

            lock (subscribers)
            {
                invocationList = subscribers.ToArray();
            }

            foreach (var callback in invocationList)
                await callback(ServerTimestamp, OCPIAPI, Request, Response).ConfigureAwait(false);

        }

        #endregion

        #region WhenAny    (ServerTimestamp, OCPIAPI, Request, Response,               Timeout = null)

        /// <summary>
        /// Call all subscribers in parallel and wait for any to complete.
        /// </summary>
        /// <param name="ServerTimestamp">The timestamp of the event.</param>
        /// <param name="OCPIAPI">The sending OCPI/HTTP API.</param>
        /// <param name="Request">The incoming request.</param>
        /// <param name="Response">The outgoing response.</param>
        /// <param name="Timeout">A timeout for this operation.</param>
        public Task WhenAny(DateTime      ServerTimestamp,
                            HTTPAPI       OCPIAPI,
                            OCPIRequest   Request,
                            OCPIResponse  Response,
                            TimeSpan?     Timeout = null)
        {

            List<Task> invocationList;

            lock (subscribers)
            {

                invocationList = subscribers.
                                     Select(callback => callback(ServerTimestamp, OCPIAPI, Request, Response)).
                                     ToList();

                if (Timeout.HasValue)
                    invocationList.Add(Task.Delay(Timeout.Value));

            }

            return Task.WhenAny(invocationList);

        }

        #endregion

        #region WhenFirst  (ServerTimestamp, OCPIAPI, Request, Response, VerifyResult, Timeout = null, DefaultResult = null)

        /// <summary>
        /// Call all subscribers in parallel and wait for all to complete.
        /// </summary>
        /// <typeparam name="T">The type of the results.</typeparam>
        /// <param name="ServerTimestamp">The timestamp of the event.</param>
        /// <param name="OCPIAPI">The sending OCPI/HTTP API.</param>
        /// <param name="Request">The incoming request.</param>
        /// <param name="Response">The outgoing response.</param>
        /// <param name="VerifyResult">A delegate to verify and filter results.</param>
        /// <param name="DefaultResult">A default result in case of errors or a timeout.</param>
        /// <param name="Timeout">A timeout for this operation.</param>
        public Task<T> WhenFirst<T>(DateTime           ServerTimestamp,
                                    HTTPAPI            OCPIAPI,
                                    OCPIRequest        Request,
                                    OCPIResponse       Response,
                                    Func<T, Boolean>   VerifyResult,
                                    Func<TimeSpan, T>  DefaultResult,
                                    TimeSpan?          Timeout   = null)
        {

            #region Data

            List<Task>  invocationList;
            Task?       WorkDone;
            Task<T>     Result;
            DateTime    StartTime     = Timestamp.Now;
            Task?       TimeoutTask   = null;

            #endregion

            lock (subscribers)
            {

                invocationList = subscribers.
                                     Select(callback => callback(ServerTimestamp, OCPIAPI, Request, Response)).
                                     ToList();

                if (Timeout.HasValue)
                    invocationList.Add(TimeoutTask = Task.Run(() => Thread.Sleep(Timeout.Value)));

            }

            do
            {

                try
                {

                    WorkDone = Task.WhenAny(invocationList);

                    invocationList.Remove(WorkDone);

                    if (WorkDone != TimeoutTask)
                    {

                        Result = WorkDone as Task<T>;

                        if (Result != null &&
                            !EqualityComparer<T>.Default.Equals(Result.Result, default(T)) &&
                            VerifyResult(Result.Result))
                        {
                            return Result;
                        }

                    }

                }
                catch (Exception e)
                {
                    DebugX.LogT(e.Message);
                    WorkDone = null;
                }

            }
            while (!(WorkDone == TimeoutTask || invocationList.Count == 0));

            return Task.FromResult(DefaultResult(Timestamp.Now - StartTime));

        }

        #endregion

        #region WhenAll    (ServerTimestamp, OCPIAPI, Request, Response)

        /// <summary>
        /// Call all subscribers in parallel and wait for all to complete.
        /// </summary>
        /// <param name="ServerTimestamp">The timestamp of the event.</param>
        /// <param name="OCPIAPI">The sending OCPI/HTTP API.</param>
        /// <param name="Request">The incoming request.</param>
        /// <param name="Response">The outgoing response.</param>
        public Task WhenAll(DateTime      ServerTimestamp,
                            HTTPAPI       OCPIAPI,
                            OCPIRequest   Request,
                            OCPIResponse  Response)
        {

            Task[] invocationList;

            lock (subscribers)
            {
                invocationList = subscribers.
                                     Select (callback => callback(ServerTimestamp, OCPIAPI, Request, Response)).
                                     ToArray();
            }

            return Task.WhenAll(invocationList);

        }

        #endregion

    }

}
