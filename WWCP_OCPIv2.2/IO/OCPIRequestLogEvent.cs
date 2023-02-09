/*
 * Copyright (c) 2015-2023 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2.HTTP
{

    /// <summary>
    /// An async event notifying about OCPI requests.
    /// </summary>
    public class OCPIRequestLogEvent
    {

        #region Data

        private readonly List<OCPIRequestLogHandler> subscribers;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new async event notifying about incoming HTTP requests.
        /// </summary>
        public OCPIRequestLogEvent()
        {
            subscribers = new List<OCPIRequestLogHandler>();
        }

        #endregion


        #region + / Add

        public static OCPIRequestLogEvent operator + (OCPIRequestLogEvent e, OCPIRequestLogHandler callback)
        {

            if (callback == null)
                throw new NullReferenceException("callback is null");

            if (e == null)
                e = new OCPIRequestLogEvent();

            lock (e.subscribers)
            {
                e.subscribers.Add(callback);
            }

            return e;

        }

        public OCPIRequestLogEvent Add(OCPIRequestLogHandler callback)
        {

            if (callback == null)
                throw new NullReferenceException("callback is null");

            lock (subscribers)
            {
                subscribers.Add(callback);
            }

            return this;

        }

        #endregion

        #region - / Remove

        public static OCPIRequestLogEvent operator - (OCPIRequestLogEvent e, OCPIRequestLogHandler callback)
        {

            if (callback == null)
                throw new NullReferenceException("callback is null");

            if (e == null)
                return null;

            lock (e.subscribers)
            {
                e.subscribers.Remove(callback);
            }

            return e;

        }

        public OCPIRequestLogEvent Remove(OCPIRequestLogHandler callback)
        {

            if (callback == null)
                throw new NullReferenceException("callback is null");

            lock (subscribers)
            {
                subscribers.Remove(callback);
            }

            return this;

        }

        #endregion


        #region InvokeAsync(ServerTimestamp, OCPIAPI, Request)

        /// <summary>
        /// Call all subscribers sequentially.
        /// </summary>
        /// <param name="ServerTimestamp">The timestamp of the event.</param>
        /// <param name="OCPIAPI">The sending OCPI/HTTP API.</param>
        /// <param name="Request">The incoming request.</param>
        public async Task InvokeAsync(DateTime     ServerTimestamp,
                                      HTTPAPI      OCPIAPI,
                                      OCPIRequest  Request)
        {

            OCPIRequestLogHandler[] _invocationList;

            lock (subscribers)
            {
                _invocationList = subscribers.ToArray();
            }

            foreach (var callback in _invocationList)
                await callback(ServerTimestamp, OCPIAPI, Request).ConfigureAwait(false);

        }

        #endregion

        #region WhenAny    (ServerTimestamp, OCPIAPI, Request,               Timeout = null)

        /// <summary>
        /// Call all subscribers in parallel and wait for any to complete.
        /// </summary>
        /// <param name="ServerTimestamp">The timestamp of the event.</param>
        /// <param name="OCPIAPI">The sending OCPI/HTTP API.</param>
        /// <param name="Request">The incoming request.</param>
        /// <param name="Timeout">A timeout for this operation.</param>
        public Task WhenAny(DateTime     ServerTimestamp,
                            HTTPAPI      OCPIAPI,
                            OCPIRequest  Request,
                            TimeSpan?    Timeout = null)
        {

            List<Task> _invocationList;

            lock (subscribers)
            {

                _invocationList = subscribers.
                                        Select(callback => callback(ServerTimestamp, OCPIAPI, Request)).
                                        ToList();

                if (Timeout.HasValue)
                    _invocationList.Add(Task.Delay(Timeout.Value));

            }

            return Task.WhenAny(_invocationList);

        }

        #endregion

        #region WhenFirst  (ServerTimestamp, OCPIAPI, Request, VerifyResult, Timeout = null, DefaultResult = null)

        /// <summary>
        /// Call all subscribers in parallel and wait for all to complete.
        /// </summary>
        /// <typeparam name="T">The type of the results.</typeparam>
        /// <param name="ServerTimestamp">The timestamp of the event.</param>
        /// <param name="OCPIAPI">The sending OCPI/HTTP API.</param>
        /// <param name="Request">The incoming request.</param>
        /// <param name="VerifyResult">A delegate to verify and filter results.</param>
        /// <param name="Timeout">A timeout for this operation.</param>
        /// <param name="DefaultResult">A default result in case of errors or a timeout.</param>
        public Task<T> WhenFirst<T>(DateTime           ServerTimestamp,
                                    HTTPAPI            OCPIAPI,
                                    OCPIRequest        Request,
                                    Func<T, Boolean>   VerifyResult,
                                    TimeSpan?          Timeout        = null,
                                    Func<TimeSpan, T>  DefaultResult  = null)
        {

            #region Data

            List<Task>  _invocationList;
            Task        WorkDone;
            Task<T>     Result;
            DateTime    StartTime    = Timestamp.Now;
            Task        TimeoutTask  = null;

            #endregion

            lock (subscribers)
            {

                _invocationList = subscribers.
                                        Select(callback => callback(ServerTimestamp, OCPIAPI, Request)).
                                        ToList();

                if (Timeout.HasValue)
                    _invocationList.Add(TimeoutTask = Task.Run(() => System.Threading.Thread.Sleep(Timeout.Value)));

            }

            do
            {

                try
                {

                    WorkDone = Task.WhenAny(_invocationList);

                    _invocationList.Remove(WorkDone);

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
            while (!(WorkDone == TimeoutTask || _invocationList.Count == 0));

            return Task.FromResult(DefaultResult(Timestamp.Now - StartTime));

        }

        #endregion

        #region WhenAll    (ServerTimestamp, OCPIAPI, Request)

        /// <summary>
        /// Call all subscribers in parallel and wait for all to complete.
        /// </summary>
        /// <param name="ServerTimestamp">The timestamp of the event.</param>
        /// <param name="OCPIAPI">The sending OCPI/HTTP API.</param>
        /// <param name="Request">The incoming request.</param>
        public Task WhenAll(DateTime     ServerTimestamp,
                            HTTPAPI      OCPIAPI,
                            OCPIRequest  Request)
        {

            Task[] _invocationList;

            lock (subscribers)
            {
                _invocationList = subscribers.
                                        Select(callback => callback(ServerTimestamp, OCPIAPI, Request)).
                                        ToArray();
            }

            return Task.WhenAll(_invocationList);

        }

        #endregion

    }

}
