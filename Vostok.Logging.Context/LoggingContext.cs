/*using Vostok.Context.FlowingContextProvider;

namespace Vostok.Logging.Context
{
    /// <summary>
    /// This only works well on .NET 4.5 and later. Avoid using contextual prefixes with older runtime versions.
    /// </summary>
    public static class LoggingContext
    {
        public static string Prefix
        {
            get
            {
                var internalContext = InternalContext;
                if (internalContext == null)
                    return string.Empty;

                return internalContext.Prefix ?? string.Empty;
            }
            set => InternalContext = string.IsNullOrEmpty(value) ? null : new LoggingContextInternal(value);
        }

        internal static LoggingContextInternal InternalContext
        {
            get => FlowingContextProvider.Get<LoggingContextInternal>();
            set => FlowingContextProvider.Set(value);
        }
    }
}*/