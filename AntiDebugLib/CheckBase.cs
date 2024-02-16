namespace AntiDebugLib
{
    public abstract class CheckBase
    {
        /// <summary>
        /// The name of this module.
        /// </summary>
        public abstract string Name { get; }

        public abstract CheckReliability Reliability { get; }

        private ILogger lazyLogger;

        /// <summary>
        /// The logger dedicated to this module. It's lazy-initialized.
        /// </summary>
        protected ILogger Logger => lazyLogger ?? (lazyLogger = LogExt.ForModule(Name));

        /// <summary>
        /// Take action to prevent from being debugged.
        /// Only single execution of this function on the startup of the application is enough.
        /// </summary>
        /// <returns><c>true</c> if the action is successfully applied, <c>false</c> otherwise.</returns>
        public virtual bool PreventPassive() => true;

        /// <summary>
        /// Take action to prevent from being debugged.
        /// This function should be executed every seconds to have such effects.
        /// </summary>
        /// <returns><c>true</c> if the action is successfully applied, <c>false</c> otherwise.</returns>
        public virtual bool PreventActive() => true;

        /// <summary>
        /// Check whether a debugger or similar behavior is present.
        /// Only single execution of this function on the startup of the application is enough.
        /// </summary>
        /// <returns><c>true</c> if debugging action is present, <c>false</c> otherwise.</returns>
        public virtual bool CheckPassive() => true;

        /// <summary>
        /// Check whether a debugger or similar behavior is present.
        /// This function should be executed every seconds to have such effects.
        /// </summary>
        /// <returns><c>true</c> if debugging action is present, <c>false</c> otherwise.</returns>
        public virtual bool CheckActive() => true;
    }
}
