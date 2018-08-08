using System;
using System.Linq;
using JetBrains.Annotations;
using Vostok.Context;
using Vostok.Logging.Abstractions;

namespace Vostok.Logging.Context
{
    // TODO(iloktionov): allow overwrite properties?
    // TODO(iloktionov): maybe we shouldn't add null properties to log events?
    // TODO(iloktionov): xml docs
    // TODO(iloktionov): unit tests

    [PublicAPI]
    public static class ContextualLogExtensions
    {
        public static ILog WithFlowingContextGlobal<T>([NotNull] this ILog log, [NotNull] string logPropertyName)
        {
            return log.WithProperty(logPropertyName, () => FlowingContext.Globals.Get<T>());
        }

        public static ILog WithFlowingContextProperty([NotNull] this ILog log, [NotNull] string contextPropertyName, [CanBeNull] string logPropertyName = null)
        {
            throw new NotImplementedException();
        }

        public static ILog WithFlowingContextProperties([NotNull] this ILog log, [NotNull] params string[] names)
        {
            throw new NotImplementedException();
        }

        public static ILog WithAllFlowingContextProperties([NotNull] this ILog log)
        {
            return log.WithProperties(() => FlowingContext.Properties.Current.Select(pair => (pair.Key, pair.Value)));
        }

        public static ILog WithContextualPrefix([NotNull] this ILog log)
        {
            return log.WithProperty(WellKnownProperties.ContextualPrefix, () => ContextualLogPrefix.Current, true);
        }
    }
}
