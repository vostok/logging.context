using System.Collections.Immutable;
using System.Linq;
using JetBrains.Annotations;
using Vostok.Context;
using Vostok.Logging.Abstractions;

namespace Vostok.Logging.Context
{
    public class ContextualPrefixedILogWrapper : IILogWrapper
    {
        public const string PrefixPropertyName = "prefix";

        public ContextualPrefixedILogWrapper([NotNull] ILog log) => BaseLog = log;

        public ILog BaseLog { get; }

        public static ImmutableArray<string> Prefix =>
            FlowingContext.Get<ImmutableArray<string>>(ContextualLogPrefix.PrefixKey);

        public void Log(LogEvent @event)
        {
            var prefix = Prefix;

            if (@event != null && prefix != null && !prefix.IsDefaultOrEmpty)
                @event = @event.WithProperty(PrefixPropertyName, prefix);

            BaseLog.Log(@event);
        }

        public bool IsEnabledFor(LogLevel level) => BaseLog.IsEnabledFor(level);

        public ILog ForContext(string context)
        {
            throw new System.NotImplementedException();
        }
    }
}