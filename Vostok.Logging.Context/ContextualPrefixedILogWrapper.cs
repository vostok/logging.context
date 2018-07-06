using System.Collections.Immutable;
using System.Linq;
using Vostok.Context;
using Vostok.Logging.Abstractions;

namespace Vostok.Logging.Context
{
    /// <summary>
    /// This only works well on .NET 4.5 and later. Avoid using contextual prefixes with older runtime versions.
    /// </summary>
    public class ContextualPrefixedILogWrapper : IILogWrapper
    {
        public ContextualPrefixedILogWrapper(ILog log) => BaseLog = log;

        public ILog BaseLog { get; }

        // public string Prefix => LoggingContext.Prefix;

        public void Log(LogEvent @event)
        {
            var prefix = FlowingContext.Get<ImmutableArray<string>>(ContextualLogPrefix.PrefixKey);

            if (!prefix.IsEmpty)
                @event = @event.WithProperty("prefix", prefix.Last());

            BaseLog.Log(@event);

            //

            /*using (new ContextualLogPrefix("P1"))
            {
                log.Info("msg"); // [P1] msg

                using (new ContextualLogPrefix("P2"))
                {
                    log.Info("msg"); // [P1] [P2] msg
                }
            }*/
        }

        public bool IsEnabledFor(LogLevel level) => BaseLog.IsEnabledFor(level);
    }
}