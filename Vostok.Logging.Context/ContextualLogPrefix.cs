using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Vostok.Context;

namespace Vostok.Logging.Context
{
    public class ContextualLogPrefix : IDisposable
    {
        public const string PrefixKey = "logging.prefix";
        private readonly bool needRestoreContext;
        private readonly IReadOnlyList<string> oldPrefix;

        public ContextualLogPrefix([NotNull] string prefix, bool needRestoreContext = true)
        {
            this.needRestoreContext = needRestoreContext;
            oldPrefix = FlowingContext.Properties.Get<IReadOnlyList<string>>(PrefixKey);
            IReadOnlyList<string> newPrefix = null;
            if (oldPrefix == null || oldPrefix.Count == 0)
                newPrefix = new[] {prefix};
            else
            {
                var list = oldPrefix.ToList();
                list.Add(prefix);
                newPrefix = list;
            }
            FlowingContext.Properties.Set(PrefixKey, newPrefix);
        }

        public void Dispose()
        {
            if (needRestoreContext && oldPrefix != null && oldPrefix.Count > 0)
                FlowingContext.Properties.Set(PrefixKey, oldPrefix);
            else
                FlowingContext.Properties.Remove(PrefixKey);
        }
    }
}