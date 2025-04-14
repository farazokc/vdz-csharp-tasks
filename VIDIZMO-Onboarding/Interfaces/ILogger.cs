using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Onboarding.Interfaces;

namespace Onboarding.Interfaces
{
    public interface ILogger
    {
        public abstract void Log(string message);
        public abstract void LogError(string message);
    }
}
