using JetBrains.Annotations;
using Vostok.Context;
using Vostok.Logging.Abstractions;
using Vostok.Logging.Abstractions.Values;

namespace Vostok.Logging.Context
{
    [PublicAPI]
    public static class OperationContextLogExtensions
    {
        /// <summary>
        /// <para>Returns a wrapper log that adds an <see cref="WellKnownProperties.OperationContext"/> property from <see cref="FlowingContext"/>'s global <see cref="OperationContextValue"/> to each <see cref="LogEvent"/> before logging.</para>
        /// <para>See <see cref="OperationContextToken"/> for more info on how to leverage operation contexts.</para>
        /// </summary>
        public static ILog WithOperationContext([NotNull] this ILog log)
            => log.WithProperty(WellKnownProperties.OperationContext, () => FlowingContext.Globals.Get<OperationContextValue>(), true);

        /// <summary>
        /// <para>This extension is just a convenience method for creating <see cref="OperationContextToken"/>s and does not depend on provided <see cref="ILog"/> instance.</para>
        /// <para>Use <see cref="WithOperationContext"/> to enable operation contexts on your log.</para>
        /// <para>See <see cref="OperationContextToken"/> for more details on how operation contexts work.</para>
        /// </summary>
        public static OperationContextToken ForOperationContext([NotNull] this ILog log, [NotNull] string operationContext)
            => new OperationContextToken(operationContext);
    }
}