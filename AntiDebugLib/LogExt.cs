
namespace AntiDebugLib
{
    /// <summary>
    /// Logger abstraction layer factory.
    /// </summary>
    public static class LogExt
    {
        internal static ILogger ForModule(string moduleName) => AntiDebug.Logger?.ForContext("Module", moduleName) ?? new DummyLogger();
    }
}
