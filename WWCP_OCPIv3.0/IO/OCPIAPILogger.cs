/*
 * Copyright (c) 2015-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using System.Text;
using System.Collections.Concurrent;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.Logging;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv3_0.HTTP
{

    //public delegate String OCPILogfileCreatorDelegate(String LoggingPath, RemoteParty? RemoteParty, String Context, String LogfileName);

    public static class OCPIAPILoggerExtensions
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

        #region (class) OCPIAPIRequestLogger

        /// <summary>
        /// A wrapper class to manage OCPI API event subscriptions
        /// for logging purposes.
        /// </summary>
        public sealed class OCPIAPIRequestLogger
        {

            #region Data

            private readonly Dictionary<LogTargets, OCPIRequestLogHandler>  subscriptionDelegates;
            private readonly HashSet<LogTargets>                            subscriptionStatus;

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
                    throw new ArgumentNullException(nameof(LogEventName), "The given log event name must not be null or empty!");

                #endregion

                this.LoggingPath                   = LoggingPath ?? AppContext.BaseDirectory;
                this.Context                       = Context     ?? "";
                this.LogEventName                  = LogEventName;
                this.SubscribeToEventDelegate      = SubscribeToEventDelegate;
                this.UnsubscribeFromEventDelegate  = UnsubscribeFromEventDelegate;
                this.subscriptionDelegates         = new Dictionary<LogTargets, OCPIRequestLogHandler>();
                this.subscriptionStatus            = new HashSet<LogTargets>();

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

                if (subscriptionDelegates.ContainsKey(LogTarget))
                    throw new ArgumentException("Duplicate log target!", nameof(LogTarget));

                subscriptionDelegates.Add(LogTarget,
                                          (timestamp, httpAPI, request) => RequestDelegate(LoggingPath, Context, LogEventName, request));

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

                if (subscriptionDelegates.TryGetValue(LogTarget,
                                                      out var requestLogHandler))
                {
                    SubscribeToEventDelegate(requestLogHandler);
                    subscriptionStatus.Add(LogTarget);
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

                => subscriptionStatus.Contains(LogTarget);

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

                if (subscriptionDelegates.TryGetValue(LogTarget,
                                                      out var requestLogHandler))
                {
                    UnsubscribeFromEventDelegate(requestLogHandler);
                    subscriptionStatus.Remove(LogTarget);
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
        public sealed class OCPIAPIResponseLogger
        {

            #region Data

            private readonly Dictionary<LogTargets, OCPIResponseLogHandler>  subscriptionDelegates;
            private readonly HashSet<LogTargets>                             subscriptionStatus;

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
                    throw new ArgumentNullException(nameof(LogEventName), "The given log event name must not be null or empty!");

                #endregion

                this.LoggingPath                   = LoggingPath ?? AppContext.BaseDirectory;
                this.Context                       = Context     ?? "";
                this.LogEventName                  = LogEventName;
                this.SubscribeToEventDelegate      = SubscribeToEventDelegate;
                this.UnsubscribeFromEventDelegate  = UnsubscribeFromEventDelegate;
                this.subscriptionDelegates         = new Dictionary<LogTargets, OCPIResponseLogHandler>();
                this.subscriptionStatus            = new HashSet<LogTargets>();

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

                if (subscriptionDelegates.ContainsKey(LogTarget))
                    throw new ArgumentException("Duplicate log target!", nameof(LogTarget));

                subscriptionDelegates.Add(LogTarget,
                                          (timestamp, httpAPI, request, response) => HTTPResponseDelegate(LoggingPath, Context, LogEventName, request, response));

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

                if (subscriptionDelegates.TryGetValue(LogTarget,
                                                      out var responseLogHandler))
                {
                    SubscribeToEventDelegate(responseLogHandler);
                    subscriptionStatus.Add(LogTarget);
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

                => subscriptionStatus.Contains(LogTarget);

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

                if (subscriptionDelegates.TryGetValue(LogTarget,
                                                      out var responseLogHandler))
                {
                    SubscribeToEventDelegate(responseLogHandler);
                    subscriptionStatus.Remove(LogTarget);
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
        public Task Default_LogOCPIRequest_toConsole(String       LoggingPath,
                                                     String       Context,
                                                     String       LogEventName,
                                                     OCPIRequest  Request)
        {

            lock (lockObject)
            {

                var previousColor = Console.ForegroundColor;

                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("[" + Request.HTTPRequest.Timestamp.ToLocalTime() + " T:" + Environment.CurrentManagedThreadId.ToString() + "] ");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write(Context + "/");
                Console.ForegroundColor = ConsoleColor.Yellow;

                Console.Write(LogEventName);
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine(" from " + Request.HTTPRequest.HTTPSource);

                Console.ForegroundColor = previousColor;

            }

            return Task.CompletedTask;

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
        public Task Default_LogOCPIResponse_toConsole(String        LoggingPath,
                                                      String        Context,
                                                      String        LogEventName,
                                                      OCPIRequest   Request,
                                                      OCPIResponse  Response)
        {

            lock (lockObject)
            {

                var PreviousColor = Console.ForegroundColor;

                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("[" + Request.HTTPRequest.Timestamp.ToLocalTime() + " T:" + Environment.CurrentManagedThreadId.ToString() + "] ");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write(Context + "/");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(LogEventName);
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write(String.Concat($" from {Request.HTTPRequest.HTTPSource} => "));

                if (Response.HTTPResponse is not null)
                {

                    if (Response.HTTPResponse.HTTPStatusCode == HTTPStatusCode.OK ||
                        Response.HTTPResponse.HTTPStatusCode == HTTPStatusCode.Created)
                        Console.ForegroundColor = ConsoleColor.Green;

                    else if (Response.HTTPResponse.HTTPStatusCode == HTTPStatusCode.NoContent)
                        Console.ForegroundColor = ConsoleColor.Yellow;

                    else
                        Console.ForegroundColor = ConsoleColor.Red;

                }

                Console.Write((Response?.HTTPResponse?.HTTPStatusCode?.ToString()) ?? "Exception: OCPI Response is null!");

                Console.ForegroundColor = ConsoleColor.Gray;

                if (Response != null)
                    Console.WriteLine(String.Concat(" in ", Math.Round((Response.Timestamp - Request.HTTPRequest.Timestamp).TotalMilliseconds), "ms"));

                Console.ForegroundColor = PreviousColor;

            }

            return Task.CompletedTask;

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

            //ToDo: Can we have a lock per logfile?
            var LockTaken = await logHTTPRequest_toDisc_Lock.WaitAsync(MaxWaitingForALock);

            try
            {

                if (LockTaken)
                {

                    var retry = 0;

                    do
                    {

                        try
                        {

                            File.AppendAllText(LogfileCreator(LoggingPath, Request.RemoteParty, Context, LogEventName),
                                               String.Concat(Request.HTTPRequest.HTTPSource, " -> ", Request.HTTPRequest.LocalSocket,             Environment.NewLine,
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
                                DebugX.LogT("File access error while logging to '" + LogfileCreator(LoggingPath, Request.RemoteParty, Context, LogEventName) + "' (retry: " + retry + "): " + e.Message);
                                Thread.Sleep(100);
                            }

                            else
                            {
                                DebugX.LogT("Could not log to '" + LogfileCreator(LoggingPath, Request.RemoteParty, Context, LogEventName) + "': " + e.Message);
                                break;
                            }

                        }
                        catch (Exception e)
                        {
                            DebugX.LogT("Could not log to '" + LogfileCreator(LoggingPath, Request.RemoteParty, Context, LogEventName) + "': " + e.Message);
                            break;
                        }

                    }
                    while (retry++ < MaxRetries);

                    if (retry >= MaxRetries)
                        DebugX.LogT("Could not write to logfile '"      + LogfileCreator(LoggingPath, Request.RemoteParty, Context, LogEventName) + "' for "   + retry + " retries!");

                    else if (retry > 0)
                        DebugX.LogT("Successfully written to logfile '" + LogfileCreator(LoggingPath, Request.RemoteParty, Context, LogEventName) + "' after " + retry + " retries!");

                }

                else
                    DebugX.LogT("Could not get lock to log to '" + LogfileCreator(LoggingPath, Request.RemoteParty, Context, LogEventName) + "'!");

            }
            finally
            {
                if (LockTaken)
                    logHTTPRequest_toDisc_Lock.Release();
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

            //ToDo: Can we have a lock per logfile?
            var LockTaken = await logHTTPResponse_toDisc_Lock.WaitAsync(MaxWaitingForALock);

            try
            {

                if (LockTaken)
                {

                    var retry = 0;

                    do
                    {

                        try
                        {

                            File.AppendAllText(LogfileCreator(LoggingPath, Request.RemoteParty, Context, LogEventName),
                                               String.Concat(Request.HTTPRequest.HTTPSource, " -> ", Request.HTTPRequest.LocalSocket,             Environment.NewLine,
                                                             ">>>>>>--Request----->>>>>>------>>>>>>------>>>>>>------>>>>>>------>>>>>>------",  Environment.NewLine,
                                                             Request.HTTPRequest.Timestamp.ToIso8601(),                                                       Environment.NewLine,
                                                             Request.HTTPRequest.EntirePDU,                                                                   Environment.NewLine,
                                                             "<<<<<<--Response----<<<<<<------<<<<<<------<<<<<<------<<<<<<------<<<<<<------",  Environment.NewLine,
                                                             Response.Timestamp.ToIso8601(),
                                                                 " -> ",
                                                                 (Response.Timestamp - Request.HTTPRequest.Timestamp).TotalMilliseconds, "ms runtime",        Environment.NewLine,
                                                             Response.HTTPResponse?.EntirePDU ?? "",                                              Environment.NewLine,
                                                             "--------------------------------------------------------------------------------",  Environment.NewLine),
                                               Encoding.UTF8);

                            break;

                        }
                        catch (IOException e)
                        {

                            if (e.HResult != -2147024864)
                            {
                                DebugX.LogT("File access error while logging to '" + LogfileCreator(LoggingPath, Request.RemoteParty, Context, LogEventName) + "' (retry: " + retry + "): " + e.Message);
                                Thread.Sleep(100);
                            }

                            else
                            {
                                DebugX.LogT("Could not log to '" + LogfileCreator(LoggingPath, Request.RemoteParty, Context, LogEventName) + "': " + e.Message);
                                break;
                            }

                        }
                        catch (Exception e)
                        {
                            DebugX.LogT("Could not log to '" + LogfileCreator(LoggingPath, Request.RemoteParty, Context, LogEventName) + "': " + e.Message);
                            break;
                        }

                    }
                    while (retry++ < MaxRetries);

                    if (retry >= MaxRetries)
                        DebugX.LogT("Could not write to logfile '"      + LogfileCreator(LoggingPath, Request.RemoteParty, Context, LogEventName) + "' for "   + retry + " retries!");

                    else if (retry > 0)
                        DebugX.LogT("Successfully written to logfile '" + LogfileCreator(LoggingPath, Request.RemoteParty, Context, LogEventName) + "' after " + retry + " retries!");

                }

                else
                    DebugX.LogT("Could not get lock to log to '" + LogfileCreator(LoggingPath, Request.RemoteParty, Context, LogEventName) + "'!");

            }
            finally
            {
                if (LockTaken)
                    logHTTPResponse_toDisc_Lock.Release();
            }

        }

        #endregion



        #region Data

        private static readonly  Object                                               lockObject                    = new ();
        private static readonly  SemaphoreSlim                                        logHTTPRequest_toDisc_Lock    = new (1,1);
        private static readonly  SemaphoreSlim                                        logHTTPResponse_toDisc_Lock   = new (1,1);

        /// <summary>
        /// The maximum number of retries to write to a logfile.
        /// </summary>
        public  static readonly  Byte                                                 MaxRetries                    = 5;

        /// <summary>
        /// Maximum waiting time to enter a lock around a logfile.
        /// </summary>
        public  static readonly  TimeSpan                                             MaxWaitingForALock            = TimeSpan.FromSeconds(15);


        protected readonly       ConcurrentDictionary<String, HashSet<String>>        groupTags;
        private   readonly       ConcurrentDictionary<String, OCPIAPIRequestLogger>   requestLoggers;
        private   readonly       ConcurrentDictionary<String, OCPIAPIResponseLogger>  responseLoggers;

        /// <summary>
        /// The default context of this logger.
        /// </summary>
        public    const          String                                               DefaultContext                = "OCPIAPI";

        #endregion

        #region Properties

        /// <summary>
        /// The HTTP server of this logger.
        /// </summary>
        public IHTTPServer                 HTTPServer        { get; }

        public String                      LoggingPath       { get; }

        /// <summary>
        /// The context of this HTTP logger.
        /// </summary>
        public String                      Context           { get; }

        /// <summary>
        /// A delegate for the default ToDisc logger returning a
        /// valid logfile name based on the given log event name.
        /// </summary>
        public OCPILogfileCreatorDelegate  LogfileCreator    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new HTTP API logger using the given logging delegates.
        /// </summary>
        /// <param name="HTTPServer">A HTTP server.</param>
        /// <param name="Context">A context of this API.</param>
        /// <param name="LogfileCreator">A delegate to create a log file from the given context and log file name.</param>
        public OCPIAPILogger(IHTTPServer                  HTTPServer,
                             String                       Context,
                             String?                      LoggingPath      = null,
                             OCPILogfileCreatorDelegate?  LogfileCreator   = null)
        {

            this.HTTPServer       = HTTPServer  ?? throw new ArgumentNullException(nameof(HTTPServer), "The given HTTP API must not be null!");
            this.Context          = Context     ?? $"OCPI{Version.String}_Logger";
            this.LoggingPath      = LoggingPath ?? AppContext.BaseDirectory;

            this.requestLoggers   = new ConcurrentDictionary<String, OCPIAPIRequestLogger>();
            this.responseLoggers  = new ConcurrentDictionary<String, OCPIAPIResponseLogger>();
            this.groupTags        = new ConcurrentDictionary<String, HashSet<String>>();

            this.LogfileCreator   = LogfileCreator ?? ((loggingPath,
                                                        remoteParty,
                                                        context,
                                                        logfilename) => String.Concat(
                                                                            loggingPath,
                                                                            remoteParty is not null
                                                                                ? remoteParty.Id.ToString() + Path.DirectorySeparatorChar
                                                                                : null,
                                                                            context is not null ? context + "_" : "",
                                                                            logfilename, "_",
                                                                            Timestamp.Now.Year, "-",
                                                                            Timestamp.Now.Month.ToString("D2"),
                                                                            ".log"
                                                                        ));

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
                throw new ArgumentNullException(nameof(LogEventName), "The given log event name must not be null or empty!");

            #endregion

            if (!requestLoggers. TryGetValue(LogEventName, out var httpRequestLogger) &&
                !responseLoggers.ContainsKey(LogEventName))
            {

                httpRequestLogger = new OCPIAPIRequestLogger(LoggingPath, Context, LogEventName, SubscribeToEventDelegate, UnsubscribeFromEventDelegate);
                requestLoggers.TryAdd(LogEventName, httpRequestLogger);

                #region Register group tag mapping

                foreach (var groupTag in GroupTags.Distinct())
                {

                    if (groupTags.TryGetValue(groupTag, out var logEventNames))
                        logEventNames.Add(LogEventName);

                    else
                        groupTags.TryAdd(groupTag, new HashSet<String>(new String[] { LogEventName }));

                }

                #endregion

                return httpRequestLogger;

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
                throw new ArgumentNullException(nameof(LogEventName), "The given log event name must not be null or empty!");

            #endregion

            if (!responseLoggers.TryGetValue(LogEventName, out var httpResponseLogger) &&
                !requestLoggers. ContainsKey(LogEventName))
            {

                httpResponseLogger = new OCPIAPIResponseLogger(LoggingPath, Context, LogEventName, SubscribeToEventDelegate, UnsubscribeFromEventDelegate);
                responseLoggers.TryAdd(LogEventName, httpResponseLogger);

                #region Register group tag mapping

                foreach (var GroupTag in GroupTags.Distinct())
                {

                    if (groupTags.TryGetValue(GroupTag, out var logEventNames))
                        logEventNames.Add(LogEventName);

                    else
                        groupTags.TryAdd(GroupTag, new HashSet<String>(new String[] { LogEventName }));

                }

                #endregion

                return httpResponseLogger;

            }

            throw new Exception("Duplicate log event name!");

        }

        #endregion


        #region RegisterLogTarget(LogTarget, RequestLogHandler)

        public void RegisterLogTarget(LogTargets                 LogTarget,
                                      OCPIRequestLoggerDelegate  RequestLogHandler)
        {

            foreach (var ocpiRequestLogger in requestLoggers.Values)
            {
                ocpiRequestLogger.RegisterLogTarget(LogTarget, RequestLogHandler);
            }

        }

        #endregion

        #region RegisterLogTarget(LogTarget, ResponseLogHandler)

        public void RegisterLogTarget(LogTargets                  LogTarget,
                                      OCPIResponseLoggerDelegate  ResponseLogHandler)
        {

            foreach (var ocpiResponseLogger in responseLoggers.Values)
            {
                ocpiResponseLogger.RegisterLogTarget(LogTarget, ResponseLogHandler);
            }

        }

        #endregion


        #region Debug  (LogEventOrGroupName, LogTarget)

        /// <summary>
        /// Start debugging the given log event.
        /// </summary>
        /// <param name="LogEventOrGroupName">A log event of group name.</param>
        /// <param name="LogTarget">The log target.</param>
        public Boolean Debug(String      LogEventOrGroupName,
                             LogTargets  LogTarget)
        {

            if (groupTags.TryGetValue(LogEventOrGroupName,
                                      out var logEventNames))
            {

                return logEventNames.
                           Select(logname => InternalDebug(logname, LogTarget)).
                           All   (result  => result == true);

            }

            return InternalDebug(LogEventOrGroupName, LogTarget);

        }

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

            if (groupTags.TryGetValue(LogEventOrGroupName,
                                      out var logEventNames))
            {

                return logEventNames.
                           Select(logname => InternalUndebug(logname, LogTarget)).
                           All   (result  => result == true);

            }

            return InternalUndebug(LogEventOrGroupName, LogTarget);

        }

        #endregion


        #region (protected) InternalDebug  (LogEventName, LogTarget)

        protected Boolean InternalDebug(String      LogEventName,
                                        LogTargets  LogTarget)
        {

            var found = false;

            if (requestLoggers. TryGetValue(LogEventName, out var httpServerRequestLogger))
                found |= httpServerRequestLogger. Subscribe(LogTarget);

            if (responseLoggers.TryGetValue(LogEventName, out var httpServerResponseLogger))
                found |= httpServerResponseLogger.Subscribe(LogTarget);

            return found;

        }

        #endregion

        #region (protected) InternalUndebug(LogEventName, LogTarget)

        protected Boolean InternalUndebug(String      LogEventName,
                                          LogTargets  LogTarget)
        {

            var found = false;

            if (requestLoggers. TryGetValue(LogEventName, out var httpServerRequestLogger))
                found |= httpServerRequestLogger. Unsubscribe(LogTarget);

            if (responseLoggers.TryGetValue(LogEventName, out var httpServerResponseLogger))
                found |= httpServerResponseLogger.Unsubscribe(LogTarget);

            return found;

        }

        #endregion


    }

}
