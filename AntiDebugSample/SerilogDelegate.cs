using AntiDebugLib;
using System;

namespace AntiDebugSample
{
    internal class SerilogDelegate : ILogger
    {
        public Serilog.ILogger BaseLogger { get; }

        public SerilogDelegate(Serilog.ILogger logger) => BaseLogger = logger;

        public ILogger ForContext(string propertyName, object value, bool destructureObjects = false) => new SerilogDelegate(BaseLogger.ForContext(propertyName, value, destructureObjects));
        public void Verbose(string messageTemplate) => BaseLogger.Verbose(messageTemplate);
        public void Verbose<T>(string messageTemplate, T propertyValue) => BaseLogger.Verbose(messageTemplate, propertyValue);
        public void Verbose<T0, T1>(string messageTemplate, T0 propertyValue0, T1 propertyValue1) => BaseLogger.Verbose(messageTemplate, propertyValue0, propertyValue1);
        public void Verbose<T0, T1, T2>(string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2) => BaseLogger.Verbose(messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        public void Verbose(string messageTemplate, params object[] propertyValues) => BaseLogger.Verbose(messageTemplate, propertyValues);
        public void Verbose(Exception exception, string messageTemplate) => BaseLogger.Verbose(exception, messageTemplate);
        public void Verbose<T>(Exception exception, string messageTemplate, T propertyValue) => BaseLogger.Verbose(exception, messageTemplate, propertyValue);
        public void Verbose<T0, T1>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1) => BaseLogger.Verbose(exception, messageTemplate, propertyValue0, propertyValue1);
        public void Verbose<T0, T1, T2>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2) => BaseLogger.Verbose(exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        public void Verbose(Exception exception, string messageTemplate, params object[] propertyValues) => BaseLogger.Verbose(exception, messageTemplate, propertyValues);
        public void Debug(string messageTemplate) => BaseLogger.Debug(messageTemplate);
        public void Debug<T>(string messageTemplate, T propertyValue) => BaseLogger.Debug(messageTemplate, propertyValue);
        public void Debug<T0, T1>(string messageTemplate, T0 propertyValue0, T1 propertyValue1) => BaseLogger.Debug(messageTemplate, propertyValue0, propertyValue1);
        public void Debug<T0, T1, T2>(string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2) => BaseLogger.Debug(messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        public void Debug(string messageTemplate, params object[] propertyValues) => BaseLogger.Debug(messageTemplate, propertyValues);
        public void Debug(Exception exception, string messageTemplate) => BaseLogger.Debug(exception, messageTemplate);
        public void Debug<T>(Exception exception, string messageTemplate, T propertyValue) => BaseLogger.Debug(exception, messageTemplate, propertyValue);
        public void Debug<T0, T1>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1) => BaseLogger.Debug(exception, messageTemplate, propertyValue0, propertyValue1);
        public void Debug<T0, T1, T2>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2) => BaseLogger.Debug(exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        public void Debug(Exception exception, string messageTemplate, params object[] propertyValues) => BaseLogger.Debug(exception, messageTemplate, propertyValues);
        public void Information(string messageTemplate) => BaseLogger.Information(messageTemplate);
        public void Information<T>(string messageTemplate, T propertyValue) => BaseLogger.Information(messageTemplate, propertyValue);
        public void Information<T0, T1>(string messageTemplate, T0 propertyValue0, T1 propertyValue1) => BaseLogger.Information(messageTemplate, propertyValue0, propertyValue1);
        public void Information<T0, T1, T2>(string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2) => BaseLogger.Information(messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        public void Information(string messageTemplate, params object[] propertyValues) => BaseLogger.Information(messageTemplate, propertyValues);
        public void Information(Exception exception, string messageTemplate) => BaseLogger.Information(exception, messageTemplate);
        public void Information<T>(Exception exception, string messageTemplate, T propertyValue) => BaseLogger.Information(exception, messageTemplate, propertyValue);
        public void Information<T0, T1>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1) => BaseLogger.Information(exception, messageTemplate, propertyValue0, propertyValue1);
        public void Information<T0, T1, T2>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2) => BaseLogger.Information(exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        public void Information(Exception exception, string messageTemplate, params object[] propertyValues) => BaseLogger.Information(exception, messageTemplate, propertyValues);
        public void Warning(string messageTemplate) => BaseLogger.Warning(messageTemplate);
        public void Warning<T>(string messageTemplate, T propertyValue) => BaseLogger.Warning(messageTemplate, propertyValue);
        public void Warning<T0, T1>(string messageTemplate, T0 propertyValue0, T1 propertyValue1) => BaseLogger.Warning(messageTemplate, propertyValue0, propertyValue1);
        public void Warning<T0, T1, T2>(string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2) => BaseLogger.Warning(messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        public void Warning(string messageTemplate, params object[] propertyValues) => BaseLogger.Warning(messageTemplate, propertyValues);
        public void Warning(Exception exception, string messageTemplate) => BaseLogger.Warning(exception, messageTemplate);
        public void Warning<T>(Exception exception, string messageTemplate, T propertyValue) => BaseLogger.Warning(exception, messageTemplate, propertyValue);
        public void Warning<T0, T1>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1) => BaseLogger.Warning(exception, messageTemplate, propertyValue0, propertyValue1);
        public void Warning<T0, T1, T2>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2) => BaseLogger.Warning(exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        public void Warning(Exception exception, string messageTemplate, params object[] propertyValues) => BaseLogger.Warning(exception, messageTemplate, propertyValues);
        public void Error(string messageTemplate) => BaseLogger.Error(messageTemplate);
        public void Error<T>(string messageTemplate, T propertyValue) => BaseLogger.Error(messageTemplate, propertyValue);
        public void Error<T0, T1>(string messageTemplate, T0 propertyValue0, T1 propertyValue1) => BaseLogger.Error(messageTemplate, propertyValue0, propertyValue1);
        public void Error<T0, T1, T2>(string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2) => BaseLogger.Error(messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        public void Error(string messageTemplate, params object[] propertyValues) => BaseLogger.Error(messageTemplate, propertyValues);
        public void Error(Exception exception, string messageTemplate) => BaseLogger.Error(exception, messageTemplate);
        public void Error<T>(Exception exception, string messageTemplate, T propertyValue) => BaseLogger.Error(exception, messageTemplate, propertyValue);
        public void Error<T0, T1>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1) => BaseLogger.Error(exception, messageTemplate, propertyValue0, propertyValue1);
        public void Error<T0, T1, T2>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2) => BaseLogger.Error(exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        public void Error(Exception exception, string messageTemplate, params object[] propertyValues) => BaseLogger.Error(exception, messageTemplate, propertyValues);
        public void Fatal(string messageTemplate) => BaseLogger.Fatal(messageTemplate);
        public void Fatal<T>(string messageTemplate, T propertyValue) => BaseLogger.Fatal(messageTemplate, propertyValue);
        public void Fatal<T0, T1>(string messageTemplate, T0 propertyValue0, T1 propertyValue1) => BaseLogger.Fatal(messageTemplate, propertyValue0, propertyValue1);
        public void Fatal<T0, T1, T2>(string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2) => BaseLogger.Fatal(messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        public void Fatal(string messageTemplate, params object[] propertyValues) => BaseLogger.Fatal(messageTemplate, propertyValues);
        public void Fatal(Exception exception, string messageTemplate) => BaseLogger.Fatal(exception, messageTemplate);
        public void Fatal<T>(Exception exception, string messageTemplate, T propertyValue) => BaseLogger.Fatal(exception, messageTemplate, propertyValue);
        public void Fatal<T0, T1>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1) => BaseLogger.Fatal(exception, messageTemplate, propertyValue0, propertyValue1);
        public void Fatal<T0, T1, T2>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2) => BaseLogger.Fatal(exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        public void Fatal(Exception exception, string messageTemplate, params object[] propertyValues) => BaseLogger.Fatal(exception, messageTemplate, propertyValues);
    }
}
