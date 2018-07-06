using System;
using System.Collections.Immutable;
using Vostok.Context;

namespace Vostok.Logging.Context
{
    /// <summary>
    /// This only works well on .NET 4.5 and later. Avoid using contextual prefixes with older runtime versions.
    /// </summary>
    public class ContextualLogPrefix : IDisposable
    {
        public const string PrefixKey = "logging.prefix";
        private readonly bool needRestoreContext;
        private readonly ImmutableArray<string> oldPrefix;

        public ContextualLogPrefix(string prefix, bool needRestoreContext = true)
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