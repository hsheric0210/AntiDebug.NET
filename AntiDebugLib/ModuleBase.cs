namespace AntiDebugLib
{
    public abstract class ModuleBase
    {
        /// <summary>
        /// The name of this module.
        /// </summary>
        public abstract string Name { get; }

        private ILogger lazyLogger;

        /// <summary>
        /// The logger dedicated to this module. It's lazy-initialized.
        /// </summary>
        protected ILogger Logger => lazyLogger ?? (lazyLogger = LogExt.ForModule(Name));
    }
}
