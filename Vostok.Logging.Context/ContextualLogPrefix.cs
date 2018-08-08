using System;
using JetBrains.Annotations;
using Vostok.Context;

namespace Vostok.Logging.Context
{
    // TODO(iloktionov): unit tests

    /// <summary>
    /// <para>Creating an instance of <see cref="ContextualLogPrefix"/> sets a new global log prefix property in <see cref="FlowingContext"/> by adding a new segment to the current prefix.</para>
    /// <para>The value is enclosed in square brackets before appending and separated by a space from previous value. A trailing space is also added for nicer formatting.</para>
    /// <para>Example: before = <c>'[v1] [v2] '</c>, after = <c>'[v1] [v2] [v3] '</c>.</para>
    /// <para>A later <see cref="Dispose"/> call then removes the new segment, so the usage pattern resembles building a stack of prefix segments.</para>
    /// <para>One should typically want to put created prefix into <c>using</c> block:</para>
    /// <para><c>using (new ContextualLogPrefix("my-prefix")) { ... }</c></para>
    /// </summary>
    [PublicAPI]
    public struct ContextualLogPrefix : IDisposable
    {
        private readonly ContextualPrefixValue old;

        /// <summary>
        /// <para>Creating an instance of <see cref="ContextualLogPrefix"/> sets a new global log prefix property in <see cref="FlowingContext"/> by adding given <paramref name="value"/> as a new segment to the current prefix.</para>
        /// <para>The value is enclosed in square brackets before appending and separated by a space from previous value. A trailing space is also added for nicer formatting.</para>
        /// <para>Example: before = <c>'[v1] [v2] '</c>, after = <c>'[v1] [v2] [v3] '</c>.</para>
        /// <para>This constructor also captures current prefix value which will be restored later when calling <see cref="Dispose"/>.</para>
        /// </summary>
        public ContextualLogPrefix([NotNull] string value)
            => FlowingContext.Globals.Set((old = FlowingContext.Globals.Get<ContextualPrefixValue>()) + value);

        /// <summary>
        /// Returns the current value of contextual log prefix in <see cref="FlowingContext"/> or <c>null</c> if there's none.
        /// </summary>
        [CanBeNull]
        public static string Current 
            => FlowingContext.Globals.Get<ContextualPrefixValue>().Value;

        /// <summary>
        /// Drops current contextual log prefix in <see cref="FlowingContext"/>.
        /// </summary>
        public static void Drop()
            => FlowingContext.Globals.Set(default(ContextualPrefixValue));

        /// <summary>
        /// Restores the value of contextual log prefix in <see cref="FlowingContext"/> that was captured in constructor.
        /// </summary>
        public void Dispose()
            => FlowingContext.Globals.Set(old);
    }
}
