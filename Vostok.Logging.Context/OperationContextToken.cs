using System;
using JetBrains.Annotations;
using Vostok.Context;
using Vostok.Logging.Abstractions.Values;

namespace Vostok.Logging.Context
{
    /// <summary>
    /// <para>Creating an instance of <see cref="OperationContextToken"/> sets a new global value of <see cref="OperationContextValue"/> in <see cref="FlowingContext"/> by adding a new value to the current ones.</para>
    /// <para>A later <see cref="Dispose"/> call restores old value back, so the usage pattern resembles building a stack of operation contexts.</para>
    /// <para>One should typically want to put created token into <c>using</c> block:</para>
    /// <para><c>using (new OperationContextValue("query-1")) { ... }</c></para>
    /// <para>Usage of operation contexts can be enabled by decorating log instance with <see cref="OperationContextLogExtensions.WithOperationContext"/> extension.</para>
    /// </summary>
    [PublicAPI]
    public struct OperationContextToken : IDisposable
    {
        private readonly OperationContextValue old;

        /// <summary>
        /// <para>Sets a new global <see cref="OperationContextValue"/> in <see cref="FlowingContext"/> by adding given <paramref name="value"/> to the current one.</para>
        /// <para>This constructor also captures current <see cref="OperationContextValue"/> which will be restored later when calling <see cref="Dispose"/>.</para>
        /// </summary>
        public OperationContextToken([NotNull] string value)
            => FlowingContext.Globals.Set((old = FlowingContext.Globals.Get<OperationContextValue>()) + value);

        /// <summary>
        /// Restores global <see cref="OperationContextValue"/> in <see cref="FlowingContext"/> to the one captured in constructor.
        /// </summary>
        public void Dispose()
            => FlowingContext.Globals.Set(old);
    }
}
