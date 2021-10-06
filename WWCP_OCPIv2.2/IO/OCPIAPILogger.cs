/*
 * Copyright (c) 2015-2021 GraphDefined GmbH
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
using System.IO;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2.HTTP
{

    public delegate Task   OCPIRequestLoggerDelegate (String LoggingPath, String Context, String LogEventName, OCPIRequest Request);
    public delegate Task   OCPIResponseLoggerDelegate(String LoggingPath, String Context, String LogEventName, OCPIRequest Request, OCPIResponse Response);

    public static class OCPIAPILoggerExtentions
    {

        #region RegisterDefaultConsoleLogTarget(this RequestLogger,  Logger)

        /// <summary>
        /// Register the default console logger.
        /// </summary>
        /// <param name="RequestLogger">A HTTP request logger.</param>
        /// <param name="Logger">A HTTP logger.</param>
        public static OCPIAPILogger.OCPIAPIRequestLogger RegisterDefaultConsoleLogTarget(this OCPIAPILogger.OCPIAPIRequestLogger  RequestLogger,
                                                                                         OCPIAPILogger                            Logger)

            => RequestLogger.RegisterLogTarget(LogTargets.Console,
                                               Logger.Default_LogOCPIRequest_toConsole);

        #endregion

        #region RegisterDefaultConsoleLogTarget(this ResponseLogger, Logger)

        /// <summary>
        /// Register the default console logger.
        /// </summary>
        /// <param name="ResponseLogger">A HTTP response logger.</param>
        /// <param name="Logger">A HTTP logger.</param>
        public static OCPIAPILogger.OCPIAPIResponseLogger RegisterDefaultConsoleLogTarget(this OCPIAPILogger.OCPIAPIResponseLogger  ResponseLogger,
                                                                                          OCPIAPILogger                             Logger)

            => ResponseLogger.RegisterLogTarget(LogTargets.Console,
                                                Logger.Default_LogOCPIResponse_toConsole);

        #endregion


        #region RegisterDefaultDiscLogTarget(this RequestLogger,  Logger)

        /// <summary>
        /// Register the default disc logger.
        /// </summary>
        /// <param name="RequestLogger">A HTTP request logger.</param>
        /// <param name="Logger">A HTTP logger.</param>
        public static OCPIAPILogger.OCPIAPIRequestLogger RegisterDefaultDiscLogTarget(this OCPIAPILogger.OCPIAPIRequestLogger  RequestLogger,
                                                                                      OCPIAPILogger                            Logger)

            => RequestLogger.RegisterLogTarget(LogTargets.Disc,
                                               Logger.Default_LogOCPIRequest_toDisc);

        #endregion

        #region RegisterDefaultDiscLogTarget(this ResponseLogger, Logger)

        /// <summary>
        /// Register the default disc logger.
        /// </summary>
        /// <param name="ResponseLogger">A HTTP response logger.</param>
        /// <param name="Logger">A HTTP logger.</param>
        public static OCPIAPILogger.OCPIAPIResponseLogger RegisterDefaultDiscLogTarget(this OCPIAPILogger.OCPIAPIResponseLogger  ResponseLogger,
                                                                                       OCPIAPILogger                             Logger)

            => ResponseLogger.RegisterLogTarget(LogTargets.Disc,
                                                Logger.Default_LogOCPIResponse_toDisc);

        #endregion

    }


    /// <summary>
    /// An OCPI/HTTP API logger.
    /// </summary>
    public class OCPIAPILogger
    {

        #region (class) APILogger

        /// <summary>
        /// A wrapper class to manage OCPI API event subscriptions
        /// for logging purposes.
        /// </summary>
        public class APILogger<T, X> // T == OCPIRequestLogHandler, X == OCPIRequestLoggerDelegate
        {

            #region Data

            private readonly Dictionary<LogTargets, T>  _SubscriptionDelegates;
            private readonly HashSet<LogTargets>        _SubscriptionStatus;

            #endregion

            #region Properties

            /// <summary>
            /// The context of the event to be logged.
            /// </summary>
            public String     Context                         { get; }

            /// <summary>
            /// The name of the event to be logged.
            /// </summary>
            public String     LogEventName                    { get; }

            /// <summary>
            /// A delegate called whenever the event is subscriped to.
            /// </summary>
            public Action<T>  SubscribeToEventDelegate        { get; }

            /// <summary>
            /// A delegate called whenever the subscription of the event is stopped.
            /// </summary>
            public Action<T>  UnsubscribeFromEventDelegate    { get; }

            #endregion

            #region Constructor(s)

            /// <summary>
            /// Create a new log event for the linked HTTP API event.
            /// </summary>
            /// <param name="Context">The context of the event.</param>
            /// <param name="LogEventName">The name of the event.</param>
            /// <param name="SubscribeToEventDelegate">A delegate for subscribing to the linked event.</param>
            /// <param name="UnsubscribeFromEventDelegate">A delegate for subscribing from the linked event.</param>
            public APILogger(String     Context,
                             String     LogEventName,
                             Action<T>  SubscribeToEventDelegate,
                             Action<T>  UnsubscribeFromEventDelegate)
            {

                #region Initial checks

                if (LogEventName.IsNullOrEmpty())
                    throw new ArgumentNullException(nameof(LogEventName),                 "The given log event name must not be null or empty!");

                if (SubscribeToEventDelegate == null)
                    throw new ArgumentNullException(nameof(SubscribeToEventDelegate),     "The given delegate for subscribing to the linked HTTP API event must not be null!");

                if (UnsubscribeFromEventDelegate == null)
                    throw new ArgumentNullException(nameof(UnsubscribeFromEventDelegate), "The given delegate for unsubscribing from the linked HTTP API event must not be null!");

                #endregion

                this.Context                       = Context ?? "";
                this.LogEventName                  = LogEventName;
                this.SubscribeToEventDelegate      = SubscribeToEventDelegate;
                this.UnsubscribeFromEventDelegate  = UnsubscribeFromEventDelegate;
                this._SubscriptionDelegates        = new Dictionary<LogTargets, T>();
                this._SubscriptionStatus           = new HashSet<LogTargets>();

            }

            #endregion


            #region RegisterLogTarget(LogTarget, RequestDelegate)

            /// <summary>
            /// Register the given log target and delegate combination.
            /// </summary>
            /// <param name="LogTarget">A log target.</param>
            /// <param name="RequestDelegate">A delegate to call.</param>
            /// <returns>A HTTP request logger.</returns>
            public APILogger<T, X> RegisterLogTarget(LogTargets  LogTarget,
                                                     X           RequestDelegate)
            {

                #region Initial checks

                if (RequestDelegate == null)
                    throw new ArgumentNullException(nameof(RequestDelegate),  "The given delegate must not be null!");

                #endregion

                if (_SubscriptionDelegates.ContainsKey(LogTarget))
                    throw new ArgumentException("Duplicate log target!", nameof(LogTarget));

                //_SubscriptionDelegates.Add(LogTarget,
                //                           (Timestamp, HTTPAPI, Request) => RequestDelegate(Context, LogEventName, Request));

                return this;

            }

            #endregion

            #region Subscribe   (LogTarget)

            /// <summary>
            /// Subscribe the given log target to the linked event.
            /// </summary>
            /// <param name="LogTarget">A log target.</param>
            /// <returns>True, if successful; false else.</returns>
            public Boolean Subscribe(LogTargets LogTarget)
            {

                if (IsSubscribed(LogTarget))
                    return true;

                if (_SubscriptionDelegates.TryGetValue(LogTarget,
                                                       out T _RequestLogHandler))
                {
                    SubscribeToEventDelegate(_RequestLogHandler);
                    _SubscriptionStatus.Add(LogTarget);
                    return true;
                }

                return false;

            }

            #endregion

            #region IsSubscribed(LogTarget)

            /// <summary>
            /// Return the subscription status of the given log target.
            /// </summary>
            /// <param name="LogTarget">A log target.</param>
            public Boolean IsSubscribed(LogTargets LogTarget)

                => _SubscriptionStatus.Contains(LogTarget);

            #endregion

            #region Unsubscribe (LogTarget)

            /// <summary>
            /// Unsubscribe the given log target from the linked event.
            /// </summary>
            /// <param name="LogTarget">A log target.</param>
            /// <returns>True, if successful; false else.</returns>
            public Boolean Unsubscribe(LogTargets LogTarget)
            {

                if (!IsSubscribed(LogTarget))
                    return true;

                if (_SubscriptionDelegates.TryGetValue(LogTarget,
                                                       out T _RequestLogHandler))
                {
                    UnsubscribeFromEventDelegate(_RequestLogHandler);
                    _SubscriptionStatus.Remove(LogTarget);
                    return true;
                }

                return false;

            }

            #endregion

        }

        #endregion

        #region (class) OCPIAPIRequestLogger

        /// <summary>
        /// A wrapper class to manage OCPI API event subscriptions
        /// for logging purposes.
        /// </summary>
        public class OCPIAPIRequestLogger
        {

            #region Data

            private readonly Dictionary<LogTargets, OCPIRequestLogHandler>  _SubscriptionDelegates;
            private readonly HashSet<LogTargets>                            _SubscriptionStatus;

            #endregion

            #region Properties

            public String                         LoggingPath                     { get; }

            /// <summary>
            /// The context of the event to be logged.
            /// </summary>
            public String                         Context                         { get; }

            /// <summary>
            /// The name of the event to be logged.
            /// </summary>
            public String                         LogEventName                    { get; }

            /// <summary>
            /// A delegate called whenever the event is subscriped to.
            /// </summary>
            public Action<OCPIRequestLogHandler>  SubscribeToEventDelegate        { get; }

            /// <summary>
            /// A delegate called whenever the subscription of the event is stopped.
            /// </summary>
            public Action<OCPIRequestLogHandler>  UnsubscribeFromEventDelegate    { get; }

            #endregion

            #region Constructor(s)

            /// <summary>
            /// Create a new log event for the linked HTTP API event.
            /// </summary>
            /// <param name="Context">The context of the event.</param>
            /// <param name="LogEventName">The name of the event.</param>
            /// <param name="SubscribeToEventDelegate">A delegate for subscribing to the linked event.</param>
            /// <param name="UnsubscribeFromEventDelegate">A delegate for subscribing from the linked event.</param>
            public OCPIAPIRequestLogger(String                         LoggingPath,
                                        String                         Context,
                                        String                         LogEventName,
                                        Action<OCPIRequestLogHandler>  SubscribeToEventDelegate,
                                        Action<OCPIRequestLogHandler>  UnsubscribeFromEventDelegate)
            {

                #region Initial checks

                if (LogEventName.IsNullOrEmpty())
                    throw new ArgumentNullException(nameof(LogEventName),                 "The given log event name must not be null or empty!");

                if (SubscribeToEventDelegate == null)
                    throw new ArgumentNullException(nameof(SubscribeToEventDelegate),     "The given delegate for subscribing to the linked HTTP API event must not be null!");

                if (UnsubscribeFromEventDelegate == null)
                    throw new ArgumentNullException(nameof(UnsubscribeFromEventDelegate), "The given delegate for unsubscribing from the linked HTTP API event must not be null!");

                #endregion

                this.Context                       = Context ?? "";
                this.LogEventName                  = LogEventName;
                this.SubscribeToEventDelegate      = SubscribeToEventDelegate;
                this.UnsubscribeFromEventDelegate  = UnsubscribeFromEventDelegate;
                this._SubscriptionDelegates        = new Dictionary<LogTargets, OCPIRequestLogHandler>();
                this._SubscriptionStatus           = new HashSet<LogTargets>();

            }

            #endregion


            #region RegisterLogTarget(LogTarget, RequestDelegate)

            /// <summary>
            /// Register the given log target and delegate combination.
            /// </summary>
            /// <param name="LogTarget">A log target.</param>
            /// <param name="RequestDelegate">A delegate to call.</param>
            /// <returns>A HTTP request logger.</returns>
            public OCPIAPIRequestLogger RegisterLogTarget(LogTargets                 LogTarget,
                                                          OCPIRequestLoggerDelegate  RequestDelegate)
            {

                #region Initial checks

                if (RequestDelegate == null)
                    throw new ArgumentNullException(nameof(RequestDelegate),  "The given delegate must not be null!");

                #endregion

                if (_SubscriptionDelegates.ContainsKey(LogTarget))
                    throw new ArgumentException("Duplicate log target!", nameof(LogTarget));

                _SubscriptionDelegates.Add(LogTarget,
                                           (Timestamp, HTTPAPI, Request) => RequestDelegate(LoggingPath, Context, LogEventName, Request));

                return this;

            }

            #endregion

            #region Subscribe   (LogTarget)

            /// <summary>
            /// Subscribe the given log target to the linked event.
            /// </summary>
            /// <param name="LogTarget">A log target.</param>
            /// <returns>True, if successful; false else.</returns>
            public Boolean Subscribe(LogTargets LogTarget)
            {

                if (IsSubscribed(LogTarget))
                    return true;

                if (_SubscriptionDelegates.TryGetValue(LogTarget,
                                                       out OCPIRequestLogHandler _RequestLogHandler))
                {
                    SubscribeToEventDelegate(_RequestLogHandler);
                    _SubscriptionStatus.Add(LogTarget);
                    return true;
                }

                return false;

            }

            #endregion

            #region IsSubscribed(LogTarget)

            /// <summary>
            /// Return the subscription status of the given log target.
            /// </summary>
            /// <param name="LogTarget">A log target.</param>
            public Boolean IsSubscribed(LogTargets LogTarget)

                => _SubscriptionStatus.Contains(LogTarget);

            #endregion

            #region Unsubscribe (LogTarget)

            /// <summary>
            /// Unsubscribe the given log target from the linked event.
            /// </summary>
            /// <param name="LogTarget">A log target.</param>
            /// <returns>True, if successful; false else.</returns>
            public Boolean Unsubscribe(LogTargets LogTarget)
            {

                if (!IsSubscribed(LogTarget))
                    return true;

                if (_SubscriptionDelegates.TryGetValue(LogTarget,
                                                       out OCPIRequestLogHandler _RequestLogHandler))
                {
                    UnsubscribeFromEventDelegate(_RequestLogHandler);
                    _SubscriptionStatus.Remove(LogTarget);
                    return true;
                }

                return false;

            }

            #endregion

        }

        #endregion

        #region (class) OCPIAPIResponseLogger

        /// <summary>
        /// A wrapper class to manage OCPI API event subscriptions
        /// for logging purposes.
        /// </summary>
        public class OCPIAPIResponseLogger
        {

            #region Data

            private readonly Dictionary<LogTargets, OCPIResponseLogHandler>  _SubscriptionDelegates;
            private readonly HashSet<LogTargets>                             _SubscriptionStatus;

            #endregion

            #region Properties

            public String                          LoggingPath                     { get; }

            /// <summary>
            /// The context of the event to be logged.
            /// </summary>
            public String                          Context                         { get; }

            /// <summary>
            /// The name of the event to be logged.
            /// </summary>
            public String                          LogEventName                    { get; }

            /// <summary>
            /// A delegate called whenever the event is subscriped to.
            /// </summary>
            public Action<OCPIResponseLogHandler>  SubscribeToEventDelegate        { get; }

            /// <summary>
            /// A delegate called whenever the subscription of the event is stopped.
            /// </summary>
            public Action<OCPIResponseLogHandler>  UnsubscribeFromEventDelegate    { get; }

            #endregion

            #region Constructor(s)

            /// <summary>
            /// Create a new log event for the linked HTTP API event.
            /// </summary>
            /// <param name="Context">The context of the event.</param>
            /// <param name="LogEventName">The name of the event.</param>
            /// <param name="SubscribeToEventDelegate">A delegate for subscribing to the linked event.</param>
            /// <param name="UnsubscribeFromEventDelegate">A delegate for subscribing from the linked event.</param>
            public OCPIAPIResponseLogger(String                          LoggingPath,
                                         String                          Context,
                                         String                          LogEventName,
                                         Action<OCPIResponseLogHandler>  SubscribeToEventDelegate,
                                         Action<OCPIResponseLogHandler>  UnsubscribeFromEventDelegate)
            {

                #region Initial checks

                if (LogEventName.IsNullOrEmpty())
                    throw new ArgumentNullException(nameof(LogEventName),                 "The given log event name must not be null or empty!");

                if (SubscribeToEventDelegate == null)
                    throw new ArgumentNullException(nameof(SubscribeToEventDelegate),     "The given delegate for subscribing to the linked  HTTP API event must not be null!");

                if (UnsubscribeFromEventDelegate == null)
                    throw new ArgumentNullException(nameof(UnsubscribeFromEventDelegate), "The given delegate for unsubscribing from the linked HTTP API event must not be null!");

                #endregion

                this.Context                       = Context ?? "";
                this.LogEventName                  = LogEventName;
                this.SubscribeToEventDelegate      = SubscribeToEventDelegate;
                this.UnsubscribeFromEventDelegate  = UnsubscribeFromEventDelegate;
                this._SubscriptionDelegates        = new Dictionary<LogTargets, OCPIResponseLogHandler>();
                this._SubscriptionStatus           = new HashSet<LogTargets>();

            }

            #endregion


            #region RegisterLogTarget(LogTarget, HTTPResponseDelegate)

            /// <summary>
            /// Register the given log target and delegate combination.
            /// </summary>
            /// <param name="LogTarget">A log target.</param>
            /// <param name="HTTPResponseDelegate">A delegate to call.</param>
            /// <returns>A HTTP response logger.</returns>
            public OCPIAPIResponseLogger RegisterLogTarget(LogTargets                  LogTarget,
                                                           OCPIResponseLoggerDelegate  HTTPResponseDelegate)
            {

                #region Initial checks

                if (HTTPResponseDelegate == null)
                    throw new ArgumentNullException(nameof(HTTPResponseDelegate), "The given delegate must not be null!");

                #endregion

                if (_SubscriptionDelegates.ContainsKey(LogTarget))
                    throw new ArgumentException("Duplicate log target!", nameof(LogTarget));

                _SubscriptionDelegates.Add(LogTarget,
                                           (Timestamp, HTTPAPI, Request, Response) => HTTPResponseDelegate(LoggingPath, Context, LogEventName, Request, Response));

                return this;

            }

            #endregion

            #region Subscribe   (LogTarget)

            /// <summary>
            /// Subscribe the given log target to the linked event.
            /// </summary>
            /// <param name="LogTarget">A log target.</param>
            /// <returns>True, if successful; false else.</returns>
            public Boolean Subscribe(LogTargets LogTarget)
            {

                if (IsSubscribed(LogTarget))
                    return true;

                if (_SubscriptionDelegates.TryGetValue(LogTarget,
                                                       out OCPIResponseLogHandler _AccessLogHandler))
                {
                    SubscribeToEventDelegate(_AccessLogHandler);
                    _SubscriptionStatus.Add(LogTarget);
                    return true;
                }

                return false;

            }

            #endregion

            #region IsSubscribed(LogTarget)

            /// <summary>
            /// Return the subscription status of the given log target.
            /// </summary>
            /// <param name="LogTarget">A log target.</param>
            public Boolean IsSubscribed(LogTargets LogTarget)

                => _SubscriptionStatus.Contains(LogTarget);

            #endregion

            #region Unsubscribe (LogTarget)

            /// <summary>
            /// Unsubscribe the given log target from the linked event.
            /// </summary>
            /// <param name="LogTarget">A log target.</param>
            /// <returns>True, if successful; false else.</returns>
            public Boolean Unsubscribe(LogTargets LogTarget)
            {

                if (!IsSubscribed(LogTarget))
                    return true;

                if (_SubscriptionDelegates.TryGetValue(LogTarget,
                                                       out OCPIResponseLogHandler _AccessLogHandler))
                {
                    UnsubscribeFromEventDelegate(_AccessLogHandler);
                    _SubscriptionStatus.Remove(LogTarget);
                    return true;
                }

                return false;

            }

            #endregion

        }

        #endregion


        // Default logging delegates

        #region Default_LogOCPIRequest_toConsole (LoggingPath, Context, LogEventName, Request)

        /// <summary>
        /// A default delegate for logging incoming OCPI requests to console.
        /// </summary>
        /// <param name="Context">The context of the log request.</param>
        /// <param name="LogEventName">The name of the log event.</param>
        /// <param name="Request">The HTTP request to log.</param>
        public async Task Default_LogOCPIRequest_toConsole(String       LoggingPath,
                                                           String       Context,
                                                           String       LogEventName,
                                                           OCPIRequest  Request)
        {

            lock (LockObject)
            {

                var PreviousColor = Console.ForegroundColor;

                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("[" + Request.HTTPRequest.Timestamp.ToLocalTime() + " T:" + Thread.CurrentThread.ManagedThreadId.ToString() + "] ");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write(Context + "/");
                Console.ForegroundColor = ConsoleColor.Yellow;

                if (Request.HTTPRequest.HTTPSource != null)
                {
                    Console.Write(LogEventName);
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine(" from " + Request.HTTPRequest.HTTPSource);
                }

                else
                    Console.WriteLine(LogEventName);

                Console.ForegroundColor = PreviousColor;

            }

        }

        #endregion

        #region Default_LogOCPIResponse_toConsole(LoggingPath, Context, LogEventName, Request, Response)

        /// <summary>
        /// A default delegate for logging OCPI requests/-responses to console.
        /// </summary>
        /// <param name="Context">The context of the log request.</param>
        /// <param name="LogEventName">The name of the log event.</param>
        /// <param name="Request">The OCPI request to log.</param>
        /// <param name="Response">The OCPI response to log.</param>
        public async Task Default_LogOCPIResponse_toConsole(String        LoggingPath,
                                                            String        Context,
                                                            String        LogEventName,
                                                            OCPIRequest   Request,
                                                            OCPIResponse  Response)
        {

            lock (LockObject)
            {

                var PreviousColor = Console.ForegroundColor;

                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("[" + Request.HTTPRequest.Timestamp.ToLocalTime() + " T:" + Thread.CurrentThread.ManagedThreadId.ToString() + "] ");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write(Context + "/");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(LogEventName);
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write(String.Concat(" from ", Request.HTTPRequest.HTTPSource != null ? Request.HTTPRequest.HTTPSource.ToString() : "<local>", " => "));

                if (Response?.HTTPResponse?.HTTPStatusCode == HTTPStatusCode.OK ||
                    Response?.HTTPResponse?.HTTPStatusCode == HTTPStatusCode.Created)
                    Console.ForegroundColor = ConsoleColor.Green;

                else if (Response?.HTTPResponse?.HTTPStatusCode == HTTPStatusCode.NoContent)
                    Console.ForegroundColor = ConsoleColor.Yellow;

                else
                    Console.ForegroundColor = ConsoleColor.Red;

                Console.Write((Response?.HTTPResponse?.HTTPStatusCode?.ToString()) ?? "Exception: OCPI Response is null!");

                Console.ForegroundColor = ConsoleColor.Gray;

                if (Response != null)
                    Console.WriteLine(String.Concat(" in ", Math.Round((Response.Timestamp - Request.HTTPRequest.Timestamp).TotalMilliseconds), "ms"));

                Console.ForegroundColor = PreviousColor;

            }

        }

        #endregion

        #region Default_LogOCPIRequest_toDisc    (LoggingPath, Context, LogEventName, Request)

        /// <summary>
        /// A default delegate for logging incoming OCPI requests to disc.
        /// </summary>
        /// <param name="Context">The context of the log request.</param>
        /// <param name="LogEventName">The name of the log event.</param>
        /// <param name="Request">The HTTP request to log.</param>
        public async Task Default_LogOCPIRequest_toDisc(String       LoggingPath,
                                                        String       Context,
                                                        String       LogEventName,
                                                        OCPIRequest  Request)
        {

            var LoggingPath = "";

            //ToDo: Can we have a lock per logfile?
            var LockTaken = await LogHTTPRequest_toDisc_Lock.WaitAsync(MaxWaitingForALock);

            try
            {

                if (LockTaken)
                {

                    var retry = 0;

                    do
                    {

                        try
                        {

                            File.AppendAllText(LogfileCreator(LoggingPath, Context, LogEventName),
                                               String.Concat(Request.HTTPRequest.HTTPSource != null && Request.HTTPRequest.LocalSocket != null
                                                                 ? String.Concat(Request.HTTPRequest.HTTPSource, " -> ", Request.HTTPRequest.LocalSocket)
                                                                 : "",                                                                            Environment.NewLine,
                                                             ">>>>>>--Request----->>>>>>------>>>>>>------>>>>>>------>>>>>>------>>>>>>------",  Environment.NewLine,
                                                             Request.HTTPRequest.Timestamp.ToIso8601(),                                                       Environment.NewLine,
                                                             Request.HTTPRequest.EntirePDU,                                                                   Environment.NewLine,
                                                             "--------------------------------------------------------------------------------",  Environment.NewLine),
                                               Encoding.UTF8);

                            break;

                        }
                        catch (IOException e)
                        {

                            if (e.HResult != -2147024864)
                            {
                                DebugX.LogT("File access error while logging to '" + LogfileCreator(LoggingPath, Context, LogEventName) + "' (retry: " + retry + "): " + e.Message);
                                Thread.Sleep(100);
                            }

                            else
                            {
                                DebugX.LogT("Could not log to '" + LogfileCreator(LoggingPath, Context, LogEventName) + "': " + e.Message);
                                break;
                            }

                        }
                        catch (Exception e)
                        {
                            DebugX.LogT("Could not log to '" + LogfileCreator(LoggingPath, Context, LogEventName) + "': " + e.Message);
                            break;
                        }

                    }
                    while (retry++ < MaxRetries);

                    if (retry >= MaxRetries)
                        DebugX.LogT("Could not write to logfile '"      + LogfileCreator(LoggingPath, Context, LogEventName) + "' for "   + retry + " retries!");

                    else if (retry > 0)
                        DebugX.LogT("Successfully written to logfile '" + LogfileCreator(LoggingPath, Context, LogEventName) + "' after " + retry + " retries!");

                }

                else
                    DebugX.LogT("Could not get lock to log to '" + LogfileCreator(LoggingPath, Context, LogEventName) + "'!");

            }
            finally
            {
                if (LockTaken)
                    LogHTTPRequest_toDisc_Lock.Release();
            }

        }

        #endregion

        #region Default_LogOCPIResponse_toDisc   (LoggingPath, Context, LogEventName, Request, Response)

        /// <summary>
        /// A default delegate for logging OCPI requests/-responses to disc.
        /// </summary>
        /// <param name="Context">The context of the log request.</param>
        /// <param name="LogEventName">The name of the log event.</param>
        /// <param name="Request">The OCPI request to log.</param>
        /// <param name="Response">The OCPI response to log.</param>
        public async Task Default_LogOCPIResponse_toDisc(String        LoggingPath,
                                                         String        Context,
                                                         String        LogEventName,
                                                         OCPIRequest   Request,
                                                         OCPIResponse  Response)
        {

            var LoggingPath = "";

            //ToDo: Can we have a lock per logfile?
            var LockTaken = await LogHTTPResponse_toDisc_Lock.WaitAsync(MaxWaitingForALock);

            try
            {

                if (LockTaken)
                {

                    var retry = 0;

                    do
                    {

                        try
                        {

                            File.AppendAllText(LogfileCreator(LoggingPath, Context, LogEventName),
                                               String.Concat(Request.HTTPRequest.HTTPSource != null && Request.HTTPRequest.LocalSocket != null
                                                                 ? String.Concat(Request.HTTPRequest.HTTPSource, " -> ", Request.HTTPRequest.LocalSocket)
                                                                 : "",                                                                            Environment.NewLine,
                                                             ">>>>>>--Request----->>>>>>------>>>>>>------>>>>>>------>>>>>>------>>>>>>------",  Environment.NewLine,
                                                             Request.HTTPRequest.Timestamp.ToIso8601(),                                                       Environment.NewLine,
                                                             Request.HTTPRequest.EntirePDU,                                                                   Environment.NewLine,
                                                             "<<<<<<--Response----<<<<<<------<<<<<<------<<<<<<------<<<<<<------<<<<<<------",  Environment.NewLine,
                                                             Response.Timestamp.ToIso8601(),
                                                                 " -> ",
                                                                 (Response.Timestamp - Request.HTTPRequest.Timestamp).TotalMilliseconds, "ms runtime",        Environment.NewLine,
                                                             Response.HTTPResponse.EntirePDU,                                                     Environment.NewLine,
                                                             "--------------------------------------------------------------------------------",  Environment.NewLine),
                                               Encoding.UTF8);

                            break;

                        }
                        catch (IOException e)
                        {

                            if (e.HResult != -2147024864)
                            {
                                DebugX.LogT("File access error while logging to '" + LogfileCreator(LoggingPath, Context, LogEventName) + "' (retry: " + retry + "): " + e.Message);
                                Thread.Sleep(100);
                            }

                            else
                            {
                                DebugX.LogT("Could not log to '" + LogfileCreator(LoggingPath, Context, LogEventName) + "': " + e.Message);
                                break;
                            }

                        }
                        catch (Exception e)
                        {
                            DebugX.LogT("Could not log to '" + LogfileCreator(LoggingPath, Context, LogEventName) + "': " + e.Message);
                            break;
                        }

                    }
                    while (retry++ < MaxRetries);

                    if (retry >= MaxRetries)
                        DebugX.LogT("Could not write to logfile '"      + LogfileCreator(LoggingPath, Context, LogEventName) + "' for "   + retry + " retries!");

                    else if (retry > 0)
                        DebugX.LogT("Successfully written to logfile '" + LogfileCreator(LoggingPath, Context, LogEventName) + "' after " + retry + " retries!");

                }

                else
                    DebugX.LogT("Could not get lock to log to '" + LogfileCreator(LoggingPath, Context, LogEventName) + "'!");

            }
            finally
            {
                if (LockTaken)
                    LogHTTPResponse_toDisc_Lock.Release();
            }

        }

        #endregion



        #region Data

        private static readonly Object         LockObject                   = new Object();
        private static          SemaphoreSlim  LogHTTPRequest_toDisc_Lock   = new SemaphoreSlim(1,1);
        private static          SemaphoreSlim  LogHTTPResponse_toDisc_Lock  = new SemaphoreSlim(1,1);

        /// <summary>
        /// The maximum number of retries to write to a logfile.
        /// </summary>
        public  static readonly Byte           MaxRetries                   = 5;

        /// <summary>
        /// Maximum waiting time to enter a lock around a logfile.
        /// </summary>
        public  static readonly TimeSpan       MaxWaitingForALock           = TimeSpan.FromSeconds(15);

        /// <summary>
        /// A delegate for the default ToDisc logger returning a
        /// valid logfile name based on the given log event name.
        /// </summary>
        public         LogfileCreatorDelegate  LogfileCreator               { get; }


        protected readonly ConcurrentDictionary<String, HashSet<String>>        _GroupTags;
        private   readonly ConcurrentDictionary<String, OCPIAPIRequestLogger>   _RequestLoggers;
        private   readonly ConcurrentDictionary<String, OCPIAPIResponseLogger>  _ResponseLoggers;

        #endregion

        #region Properties

        /// <summary>
        /// The HTTP server of this logger.
        /// </summary>
        public IHTTPServer  HTTPServer     { get; }

        public String       LoggingPath    { get; }

        /// <summary>
        /// The context of this HTTP logger.
        /// </summary>
        public String       Context        { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new HTTP API logger using the given logging delegates.
        /// </summary>
        /// <param name="HTTPServer">A HTTP server.</param>
        /// <param name="Context">A context of this API.</param>
        /// 
        /// <param name="LogHTTPRequest_toConsole">A delegate to log incoming HTTP requests to console.</param>
        /// <param name="LogHTTPResponse_toConsole">A delegate to log HTTP requests/responses to console.</param>
        /// <param name="LogHTTPRequest_toDisc">A delegate to log incoming HTTP requests to disc.</param>
        /// <param name="LogHTTPResponse_toDisc">A delegate to log HTTP requests/responses to disc.</param>
        /// 
        /// <param name="LogHTTPRequest_toNetwork">A delegate to log incoming HTTP requests to a network target.</param>
        /// <param name="LogHTTPResponse_toNetwork">A delegate to log HTTP requests/responses to a network target.</param>
        /// <param name="LogHTTPRequest_toHTTPSSE">A delegate to log incoming HTTP requests to a HTTP server sent events source.</param>
        /// <param name="LogHTTPResponse_toHTTPSSE">A delegate to log HTTP requests/responses to a HTTP server sent events source.</param>
        /// 
        /// <param name="LogHTTPError_toConsole">A delegate to log HTTP errors to console.</param>
        /// <param name="LogHTTPError_toDisc">A delegate to log HTTP errors to disc.</param>
        /// <param name="LogHTTPError_toNetwork">A delegate to log HTTP errors to a network target.</param>
        /// <param name="LogHTTPError_toHTTPSSE">A delegate to log HTTP errors to a HTTP server sent events source.</param>
        /// 
        /// <param name="LogfileCreator">A delegate to create a log file from the given context and log file name.</param>
        public OCPIAPILogger(IHTTPServer                 HTTPServer,
                             String                      Context,

                             OCPIRequestLoggerDelegate   LogHTTPRequest_toConsole,
                             OCPIResponseLoggerDelegate  LogHTTPResponse_toConsole,
                             OCPIRequestLoggerDelegate   LogHTTPRequest_toDisc,
                             OCPIResponseLoggerDelegate  LogHTTPResponse_toDisc,

                             OCPIRequestLoggerDelegate   LogHTTPRequest_toNetwork    = null,
                             OCPIResponseLoggerDelegate  LogHTTPResponse_toNetwork   = null,
                             OCPIRequestLoggerDelegate   LogHTTPRequest_toHTTPSSE    = null,
                             OCPIResponseLoggerDelegate  LogHTTPResponse_toHTTPSSE   = null,

                             OCPIResponseLoggerDelegate  LogHTTPError_toConsole      = null,
                             OCPIResponseLoggerDelegate  LogHTTPError_toDisc         = null,
                             OCPIResponseLoggerDelegate  LogHTTPError_toNetwork      = null,
                             OCPIResponseLoggerDelegate  LogHTTPError_toHTTPSSE      = null,

                             LogfileCreatorDelegate      LogfileCreator              = null)

        {

            this.HTTPServer        = HTTPServer ?? throw new ArgumentNullException(nameof(HTTPServer), "The given HTTP API must not be null!");

            this._RequestLoggers   = new ConcurrentDictionary<String, OCPIAPIRequestLogger>();
            this._ResponseLoggers  = new ConcurrentDictionary<String, OCPIAPIResponseLogger>();


            #region Init data structures

            this.Context     = Context ?? "";
            this._GroupTags  = new ConcurrentDictionary<String, HashSet<String>>();

            #endregion

            #region Set default delegates

            if (LogHTTPRequest_toConsole  == null)
                LogHTTPRequest_toConsole   = Default_LogOCPIRequest_toConsole;

            if (LogHTTPRequest_toDisc     == null)
                LogHTTPRequest_toDisc      = Default_LogOCPIRequest_toDisc;

            if (LogHTTPRequest_toDisc     == null)
                LogHTTPRequest_toDisc      = Default_LogOCPIRequest_toDisc;

            if (LogHTTPResponse_toConsole == null)
                LogHTTPResponse_toConsole  = Default_LogOCPIResponse_toConsole;

            if (LogHTTPResponse_toDisc    == null)
                LogHTTPResponse_toDisc     = Default_LogOCPIResponse_toDisc;

            this.LogfileCreator = LogfileCreator != null
                                      ? LogfileCreator
<<<<<<< HEAD
                                      : (loggingPath, context, logfileName) => String.Concat(context != null ? context + "_" : "",
                                                                                             logfileName, "_",
=======
                                      : (loggingPath, context, logfilename) => String.Concat(loggingPath,
                                                                                             context != null ? context + "_" : "",
                                                                                             logfilename, "_",
>>>>>>> 540fce42031f3f1c3a9b71c9fbc8d6dc74bdc79f
                                                                                             DateTime.UtcNow.Year, "-",
                                                                                             DateTime.UtcNow.Month.ToString("D2"),
                                                                                             ".log");

            #endregion

        }

        #endregion


        #region (protected) RegisterEvent(LogEventName, SubscribeToEventDelegate, UnsubscribeFromEventDelegate, params GroupTags)

        /// <summary>
        /// Register a log event for the linked HTTP API event.
        /// </summary>
        /// <param name="LogEventName">The name of the log event.</param>
        /// <param name="SubscribeToEventDelegate">A delegate for subscribing to the linked event.</param>
        /// <param name="UnsubscribeFromEventDelegate">A delegate for subscribing from the linked event.</param>
        /// <param name="GroupTags">An array of log event groups the given log event name is part of.</param>
        protected OCPIAPIRequestLogger RegisterEvent(String                         LogEventName,
                                                     Action<OCPIRequestLogHandler>  SubscribeToEventDelegate,
                                                     Action<OCPIRequestLogHandler>  UnsubscribeFromEventDelegate,
                                                     params String[]                GroupTags)
        {

            #region Initial checks

            if (LogEventName.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(LogEventName),                  "The given log event name must not be null or empty!");

            if (SubscribeToEventDelegate == null)
                throw new ArgumentNullException(nameof(SubscribeToEventDelegate),      "The given delegate for subscribing to the linked HTTP API event must not be null!");

            if (UnsubscribeFromEventDelegate == null)
                throw new ArgumentNullException(nameof(UnsubscribeFromEventDelegate),  "The given delegate for unsubscribing from the linked HTTP API event must not be null!");

            #endregion

            if (!_RequestLoggers. TryGetValue(LogEventName, out OCPIAPIRequestLogger _HTTPRequestLogger) &&
                !_ResponseLoggers.ContainsKey(LogEventName))
            {

                _HTTPRequestLogger = new OCPIAPIRequestLogger(LoggingPath, Context, LogEventName, SubscribeToEventDelegate, UnsubscribeFromEventDelegate);
                _RequestLoggers.TryAdd(LogEventName, _HTTPRequestLogger);

                #region Register group tag mapping

                HashSet<String> _LogEventNames = null;

                foreach (var GroupTag in GroupTags.Distinct())
                {

                    if (_GroupTags.TryGetValue(GroupTag, out _LogEventNames))
                        _LogEventNames.Add(LogEventName);

                    else
                        _GroupTags.TryAdd(GroupTag, new HashSet<String>(new String[] { LogEventName }));

                }

                #endregion

                return _HTTPRequestLogger;

            }

            throw new Exception("Duplicate log event name!");

        }

        #endregion

        #region (protected) RegisterEvent(LogEventName, SubscribeToEventDelegate, UnsubscribeFromEventDelegate, params GroupTags)

        /// <summary>
        /// Register a log event for the linked HTTP API event.
        /// </summary>
        /// <param name="LogEventName">The name of the log event.</param>
        /// <param name="SubscribeToEventDelegate">A delegate for subscribing to the linked event.</param>
        /// <param name="UnsubscribeFromEventDelegate">A delegate for subscribing from the linked event.</param>
        /// <param name="GroupTags">An array of log event groups the given log event name is part of.</param>
        protected OCPIAPIResponseLogger RegisterEvent(String                          LogEventName,
                                                      Action<OCPIResponseLogHandler>  SubscribeToEventDelegate,
                                                      Action<OCPIResponseLogHandler>  UnsubscribeFromEventDelegate,
                                                      params String[]                 GroupTags)
        {

            #region Initial checks

            if (LogEventName.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(LogEventName),                  "The given log event name must not be null or empty!");

            if (SubscribeToEventDelegate == null)
                throw new ArgumentNullException(nameof(SubscribeToEventDelegate),      "The given delegate for subscribing to the linked HTTP API event must not be null!");

            if (UnsubscribeFromEventDelegate == null)
                throw new ArgumentNullException(nameof(UnsubscribeFromEventDelegate),  "The given delegate for unsubscribing from the linked HTTP API event must not be null!");

            #endregion

            if (!_ResponseLoggers.TryGetValue(LogEventName, out OCPIAPIResponseLogger _HTTPResponseLogger) &&
                !_RequestLoggers. ContainsKey(LogEventName))
            {

                _HTTPResponseLogger = new OCPIAPIResponseLogger(LoggingPath, Context, LogEventName, SubscribeToEventDelegate, UnsubscribeFromEventDelegate);
                _ResponseLoggers.TryAdd(LogEventName, _HTTPResponseLogger);

                #region Register group tag mapping

                HashSet<String> _LogEventNames = null;

                foreach (var GroupTag in GroupTags.Distinct())
                {

                    if (_GroupTags.TryGetValue(GroupTag, out _LogEventNames))
                        _LogEventNames.Add(LogEventName);

                    else
                        _GroupTags.TryAdd(GroupTag, new HashSet<String>(new String[] { LogEventName }));

                }

                #endregion

                return _HTTPResponseLogger;

            }

            throw new Exception("Duplicate log event name!");

        }

        #endregion



        #region Debug(LogEventOrGroupName, LogTarget)

        /// <summary>
        /// Start debugging the given log event.
        /// </summary>
        /// <param name="LogEventOrGroupName">A log event of group name.</param>
        /// <param name="LogTarget">The log target.</param>
        public Boolean Debug(String      LogEventOrGroupName,
                             LogTargets  LogTarget)
        {

            if (_GroupTags.TryGetValue(LogEventOrGroupName,
                                       out HashSet<String> _LogEventNames))

                return _LogEventNames.
                           Select(logname => InternalDebug(logname, LogTarget)).
                           All   (result  => result == true);


            return InternalDebug(LogEventOrGroupName, LogTarget);

        }

        #endregion

        #region (protected) InternalDebug(LogEventName, LogTarget)

        //protected abstract Boolean InternalDebug(String      LogEventName,
        //                                         LogTargets  LogTarget);

        #endregion


        #region Undebug(LogEventOrGroupName, LogTarget)

        /// <summary>
        /// Stop debugging the given log event.
        /// </summary>
        /// <param name="LogEventOrGroupName">A log event of group name.</param>
        /// <param name="LogTarget">The log target.</param>
        public Boolean Undebug(String      LogEventOrGroupName,
                               LogTargets  LogTarget)
        {

            if (_GroupTags.TryGetValue(LogEventOrGroupName,
                                       out HashSet<String> _LogEventNames))

                return _LogEventNames.
                           Select(logname => InternalUndebug(logname, LogTarget)).
                           All   (result  => result == true);


            return InternalUndebug(LogEventOrGroupName, LogTarget);

        }

        #endregion

        #region (private) InternalUndebug(LogEventName, LogTarget)

        //protected abstract Boolean InternalUndebug(String      LogEventName,
        //                                           LogTargets  LogTarget);

        #endregion



        #region (protected) InternalDebug(LogEventName, LogTarget)

        protected Boolean InternalDebug(String      LogEventName,
                                        LogTargets  LogTarget)
        {

            var _Found = false;

            if (_RequestLoggers. TryGetValue(LogEventName, out OCPIAPIRequestLogger   _HTTPServerRequestLogger2))
                _Found |= _HTTPServerRequestLogger2.Subscribe(LogTarget);

            if (_ResponseLoggers.TryGetValue(LogEventName, out OCPIAPIResponseLogger  _HTTPServerResponseLogger2))
                _Found |= _HTTPServerResponseLogger2.Subscribe(LogTarget);

            return _Found;

        }

        #endregion

        #region (protected) InternalUndebug(LogEventName, LogTarget)

        protected Boolean InternalUndebug(String      LogEventName,
                                          LogTargets  LogTarget)
        {

            var _Found = false;

            if (_RequestLoggers. TryGetValue(LogEventName, out OCPIAPIRequestLogger   _HTTPServerRequestLogger2))
                _Found |= _HTTPServerRequestLogger2.Unsubscribe(LogTarget);

            if (_ResponseLoggers.TryGetValue(LogEventName, out OCPIAPIResponseLogger  _HTTPServerResponseLogger2))
                _Found |= _HTTPServerResponseLogger2.Unsubscribe(LogTarget);

            return _Found;

        }

        #endregion


    }

}
