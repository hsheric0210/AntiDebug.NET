using System;

namespace AntiDebugLib
{
    public class DebuggerDetectedEventArgs : EventArgs
    {
        public string CheckName { get; }
        public CheckReliability Reliability { get; }

        public DebuggerDetectedEventArgs(string checkName, CheckReliability reliability)
        {
            CheckName = checkName;
            Reliability = reliability;
        }
    }
}
