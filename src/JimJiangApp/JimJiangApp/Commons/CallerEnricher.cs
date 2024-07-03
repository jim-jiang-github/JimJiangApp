using Serilog.Core;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JimJiangApp.Commons
{
    public class CallerEnricher : ILogEventEnricher
    {
        private object obj;
        private string callerFilePath;
        private string callerMemberName;
        private int callerLineNumber;
        public  CallerEnricher(object obj, string callerFilePath, string callerMemberName, int callerLineNumber)
        {
            this.obj = obj;
            this.callerFilePath = callerFilePath;
            this.callerMemberName = callerMemberName;
            this.callerLineNumber = callerLineNumber;
        }
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            if (obj != null)
            {
                logEvent.AddPropertyIfAbsent(new LogEventProperty("ObjHash", new ScalarValue(obj.GetHashCode())));
            }
            logEvent.AddPropertyIfAbsent(new LogEventProperty("CallerFilePath", new ScalarValue(Path.GetFileName(callerFilePath))));
            logEvent.AddPropertyIfAbsent(new LogEventProperty("CallerMemberName", new ScalarValue(callerMemberName)));
            logEvent.AddPropertyIfAbsent(new LogEventProperty("CallerLineNumber", new ScalarValue(callerLineNumber)));
        }
    }
}
