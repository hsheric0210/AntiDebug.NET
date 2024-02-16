
namespace AntiDebugLib
{
    /// <summary>
    /// Logger abstraction layer factory.
    /// </summary>
    public static class LogExt
    {
        public static ILogger BaseLogger { get; set; } = new DummyLogger();

        public static ILogger ForModule(string moduleName) => BaseLogger?.ForContext("Module", moduleName) ?? new DummyLogger();
    }
}
