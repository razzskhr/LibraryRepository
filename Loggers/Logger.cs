using Microsoft.ApplicationInsights;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loggers
{
    public class Logger : ILoggers
    {
        private TelemetryClient telemetryClient= new TelemetryClient();
       
        public void LogError(Exception ex)
        {
            telemetryClient.TrackException(ex);
        }

        public void LogInfo(string message)
        {
            telemetryClient.TrackTrace(message);
        }
    }
}
