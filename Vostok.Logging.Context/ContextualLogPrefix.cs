using System;
using JetBrains.Annotations;
using Vostok.Context;

namespace Vostok.Logging.Context
{
    // TODO(iloktionov): xml docs
    // TODO(iloktionov): unit tests

    [PublicAPI]
    public struct ContextualLogPrefix : IDisposable
    {
        private readonly ContextualPrefixValue old;

        public ContextualLogPrefix([NotNull] string value)
            => FlowingContext.Globals.Set((old = FlowingContext.Globals.Get<ContextualPrefixValue>()) + value);

        [CanBeNull]
        public static string Current 
            => FlowingContext.Globals.Get<ContextualPrefixValue>().Value;

        public static void Drop()
            => FlowingContext.Globals.Set(default(ContextualPrefixValue));

        public void Dispose()
            => FlowingContext.Globals.Set(old);
    }
}
