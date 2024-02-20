namespace AntiDebugLib
{
    public enum PreventionResultType
    {
        Failed = 0,
        Applied = 1,
        Error = 2,
        Incompatible = 3,
        NotImplemented = 4,
    }

    public readonly struct PreventionResult
    {
        public string PreventionName { get; }
        public PreventionResultType Type { get; }
        public object AdditionalInfo { get; }

        public PreventionResult(string checkName, PreventionResultType type, object additionalInfo)
        {
            PreventionName = checkName;
            Type = type;
            AdditionalInfo = additionalInfo;
        }
    }
}
