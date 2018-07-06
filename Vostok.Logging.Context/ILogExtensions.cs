using Vostok.Logging.Abstractions;

namespace Vostok.Logging.Context
{
    public static class ILogExtensions
    {
        public static ILog WithContextualPrefix(this ILog log) =>
            log.GetBaseLog() is ContextualPrefixedILogWrapper ? log : new ContextualPrefixedILogWrapper(log);

        public static ILog GetBaseLog(this ILog log, bool unwrapContextualWrapper = false)
        {
            if (!(log is IILogWrapper wrapper) || !unwrapContextualWrapper && wrapper is ContextualPrefixedILogWrapper)
                return log;

            return GetBaseLog(wrapper.BaseLog);
        }
    }
}