using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loggers
{
    public interface ILoggers
    {
        void LogError(Exception message);
        void LogInfo(string message);
    }
}
