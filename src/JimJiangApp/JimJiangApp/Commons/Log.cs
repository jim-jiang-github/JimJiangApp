using Serilog;
using Serilog.Core;
using Serilog.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace JimJiangApp.Commons
{
    /// <summary>
    /// An optional static entry point for logging that can be easily referenced
    /// by different parts of an application. To configure the <see cref="Log"/>
    /// set the Logger static property to a logger instance.
    /// </summary>
    /// <example>
    /// Log.GetLogger(this).Logger = new LoggerConfiguration()
    ///     .WithConsoleSink()
    ///     .CreateLogger();
    ///
    /// var thing = "World";
    /// Log.GetLogger(this).Logger.Information("Hello, {Thing}!", thing);
    /// </example>
    /// <remarks>
    /// The methods on <see cref="Log"/> (and its dynamic sibling <see cref="ILogger"/>) are guaranteed
    /// never to throw exceptions. Methods on all other types may.
    /// </remarks>
    public class Log
    {
        public readonly struct Logger
        {
            private readonly CallerEnricher _callerEnricher;
            public Logger(object obj, string callerFilePath, string callerMemberName, int callerLineNumber)
            {
                _callerEnricher = new CallerEnricher(obj, callerFilePath, callerMemberName, callerLineNumber);
            }

            [MessageTemplateFormatMethod("messageTemplate")]
            public void Verbose(string messageTemplate, params object[] args)
            {
                Serilog.Log
                .ForContext(_callerEnricher)
                .Verbose(messageTemplate, args);
            }

            [MessageTemplateFormatMethod("messageTemplate")]
            public void Verbose(Exception exception, string messageTemplate, params object[] args)
            {
                Serilog.Log
                .ForContext(_callerEnricher)
                .Verbose(exception, messageTemplate, args);
            }

            [MessageTemplateFormatMethod("messageTemplate")]
            public void Debug(string messageTemplate, params object[] args)
            {
                Serilog.Log
                .ForContext(_callerEnricher)
                .Debug(messageTemplate, args);
            }

            [MessageTemplateFormatMethod("messageTemplate")]
            public void Debug(Exception exception, string messageTemplate, params object[] args)
            {
                Serilog.Log
                .ForContext(_callerEnricher)
                .Debug(exception, messageTemplate, args);
            }

            [MessageTemplateFormatMethod("messageTemplate")]
            public void Information(string messageTemplate, params object[] args)
            {
                Serilog.Log
                .ForContext(_callerEnricher)
                .Information(messageTemplate, args);
            }

            [MessageTemplateFormatMethod("messageTemplate")]
            public void Information(Exception exception, string messageTemplate, params object[] args)
            {
                Serilog.Log
                .ForContext(_callerEnricher)
                .Information(exception, messageTemplate, args);
            }

            [MessageTemplateFormatMethod("messageTemplate")]
            public void Warning(string messageTemplate, params object[] args)
            {
                Serilog.Log
                .ForContext(_callerEnricher)
                .Warning(messageTemplate, args);
            }

            [MessageTemplateFormatMethod("messageTemplate")]
            public void Warning(Exception exception, string messageTemplate, params object[] args)
            {
                Serilog.Log
                .ForContext(_callerEnricher)
                .Warning(exception, messageTemplate, args);
            }

            [MessageTemplateFormatMethod("messageTemplate")]
            public void Warning(Exception exception)
            {
                Serilog.Log
                .ForContext(_callerEnricher)
                .Warning(exception, "");
            }

            [MessageTemplateFormatMethod("messageTemplate")]
            public void Error(string messageTemplate, params object[] args)
            {
                Serilog.Log
                .ForContext(_callerEnricher)
                .Error(messageTemplate, args);
            }

            [MessageTemplateFormatMethod("messageTemplate")]
            public void Error(Exception exception, string messageTemplate, params object[] args)
            {
                Serilog.Log
                .ForContext(_callerEnricher)
                .Error(exception, messageTemplate, args);
            }

            [MessageTemplateFormatMethod("messageTemplate")]
            public void Error(Exception exception)
            {
                Serilog.Log
                .ForContext(_callerEnricher)
                .Error(exception, "");
            }

            [MessageTemplateFormatMethod("messageTemplate")]
            public void Fatal(string messageTemplate, params object[] args)
            {
                Serilog.Log
                .ForContext(_callerEnricher)
                .Fatal(messageTemplate, args);
            }

            [MessageTemplateFormatMethod("messageTemplate")]
            public void Fatal(Exception exception, string messageTemplate, params object[] args)
            {
                Serilog.Log
                .ForContext(_callerEnricher)
                .Fatal(exception, messageTemplate, args);
            }
        }

        public static List<string> CurrentLogFilePaths { get; } = new List<string>();
        private static ILogger _lastLogger;

        static Log()
        {
            CleanupLogFiles(Global.LogsDirectory);
            var outputTemplate = "[{Timestamp:yyyy-MM-dd HH:mm:ss:fff}][{Level:u3}][{ThreadId}][Hash:{ObjHash}][{CallerFilePath}->{CallerMemberName}:{CallerLineNumber}]{Exception} {Message}{NewLine}";
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH.mm.ss");
            string logFileName = $"{timestamp}.log";
            _lastLogger = new LoggerConfiguration()
                .Enrich.WithExceptionDetails()
                .Enrich.FromLogContext()
                .Enrich.WithThreadId()
                .MinimumLevel.Information()
                .WriteTo.File(Path.Combine(Global.LogsDirectory, logFileName),
                rollOnFileSizeLimit: true,
                fileSizeLimitBytes: 1024 * 1024 * 100,
                outputTemplate: outputTemplate,
                rollingInterval: RollingInterval.Infinite)
                .CreateLogger();
            Serilog.Log.Logger = _lastLogger;
            GetLogger().Information("Serilog is inited");
        }

        public static void SetLogEnable(bool enable)
        {
            if (enable)
            {
                Serilog.Log.Logger = _lastLogger;
            }
            else
            {
                Serilog.Log.Logger = Serilog.Core.Logger.None;
            }
        }

        internal static void UpdateCurrentLogFilePath(string logFilePath)
        {
            lock (CurrentLogFilePaths)
            {
                CurrentLogFilePaths.Add(logFilePath);
            }
        }

        static void CleanupLogFiles(string logFolder)
        {
            if (!Directory.Exists(logFolder))
            {
                return;
            }
            var files = new DirectoryInfo(logFolder).GetFiles()
                .Where(f => f.LastWriteTime < DateTime.Now.AddDays(-30))
                .Select(f => f.FullName);

            foreach (var file in files)
            {
                File.Delete(file);
            }
        }

        public static Logger GetLogger(object obj, [CallerFilePath] string callerFilePath = null, [CallerMemberName] string callerMemberName = null, [CallerLineNumber] int callerLineNumber = 0)
        {
            return new Logger(obj, callerFilePath, callerMemberName, callerLineNumber);
        }

        public static Logger GetLogger([CallerFilePath] string callerFilePath = null, [CallerMemberName] string callerMemberName = null, [CallerLineNumber] int callerLineNumber = 0)
        {
            return new Logger(null, callerFilePath, callerMemberName, callerLineNumber);
        }

        public static void Flush()
        {
            Serilog.Log.CloseAndFlush();
        }
    }
}