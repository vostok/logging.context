using System;
using System.Collections.Immutable;
using JetBrains.Annotations;
using Vostok.Context;

namespace Vostok.Logging.Context
{
    public class ContextualLogPrefix : IDisposable
    {
        public const string PrefixKey = "logging.prefix";
        private readonly bool needRestoreContext;
        private readonly ImmutableArray<string> oldPrefix;

        public ContextualLogPrefix([NotNull] string prefix, bool needRestoreContext = true)
        {
            FlowingContext.SetOverwriteMode(true);
            this.needRestoreContext = needRestoreContext;
            oldPrefix = FlowingContext.Get<ImmutableArray<string>>(PrefixKey);
            var newPrefix = oldPrefix.IsDefault ? ImmutableArray<string>.Empty.Add(prefix) : oldPrefix.Add(prefix);
            FlowingContext.Set(PrefixKey, newPrefix);
        }

        public void Dispose()
        {
            if (needRestoreContext)
            {
                FlowingContext.SetOverwriteMode(true);
                FlowingContext.Set(PrefixKey, oldPrefix);
            }
        }
    }
}