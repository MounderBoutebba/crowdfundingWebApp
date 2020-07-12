using log4net;
using System;
using System.Reflection;

namespace Coiner.Business.LoggerService
{
    public static class Logger
    {
            public static void LogException(ILog Log, Exception ex, MethodBase method)
            {
                var useCase = string.Concat((method.DeclaringType ?? method.ReflectedType).Name, "_", method.Name);
                Log.Error(useCase, ex);
            }
            public static void LogError(ILog log, string CurrentSiteId, string message, MethodBase method)
            {
                log.Error(msgBuilder(CurrentSiteId, message, method));
            }
            public static void LogWarn(ILog log, string CurrentSiteId, string message, MethodBase method)
            {
                log.Warn(msgBuilder(CurrentSiteId, message, method));
            }
            public static void LogInfo(ILog log, string CurrentSiteId, string message, MethodBase method)
            {
                log.Info(msgBuilder(CurrentSiteId, message, method));
            }

            #region private
            private static string msgBuilder(string CurrentSiteId, string message, MethodBase method)
            {
            if (CurrentSiteId == string.Empty)
            {
                return string.Concat((method.DeclaringType ?? method.ReflectedType).Name, "_", method.Name, "\r\n", message);

            }
            else
            {
                return string.Concat("CurrentSiteId = ", CurrentSiteId, "\r\n", (method.DeclaringType ?? method.ReflectedType).Name, "_", method.Name, "\r\n", message);
            }
        }
            #endregion

        
    }
}
