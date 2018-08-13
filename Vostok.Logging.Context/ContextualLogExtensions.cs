﻿using System.Linq;
using JetBrains.Annotations;
using Vostok.Context;
using Vostok.Logging.Abstractions;

namespace Vostok.Logging.Context
{
    // TODO(iloktionov): unit tests

    [PublicAPI]
    public static class ContextualLogExtensions
    {
        /// <summary>
        /// <para>Returns a wrapper log that adds a value of global property of type <typeparamref name="T"/> from <see cref="FlowingContext"/> to each <see cref="LogEvent"/> before logging.</para>
        /// <para>Uses given <paramref name="logPropertyName"/> as a name for added <see cref="LogEvent"/> property.</para>
        /// <para>By default, existing properties are not overwritten. This can be changed via <paramref name="allowOverwrite"/> parameter.</para>
        /// <para>By default, <c>null</c> values are not added to events. This can be changed via <paramref name="allowNullValues"/> parameter.</para>
        /// </summary>
        public static ILog WithFlowingContextGlobal<T>(
            [NotNull] this ILog log, 
            [NotNull] string logPropertyName, 
            bool allowOverwrite = false,
            bool allowNullValues = false)
        {
            return log.WithProperty(logPropertyName, () => FlowingContext.Globals.Get<T>(), allowOverwrite, allowNullValues);
        }

        /// <summary>
        /// <para>Returns a wrapper log that adds a value of property with name = <paramref name="contextPropertyName"/> from <see cref="FlowingContext"/> to each <see cref="LogEvent"/> before logging.</para>
        /// <para>By default, the value is added to <see cref="LogEvent"/> with <paramref name="contextPropertyName"/>. This can be changed by providing a non-null <paramref name="logPropertyName"/> parameter.</para>
        /// <para>By default, existing properties are not overwritten. This can be changed via <paramref name="allowOverwrite"/> parameter.</para>
        /// <para>By default, <c>null</c> values are not added to events. This can be changed via <paramref name="allowNullValues"/> parameter.</para>
        /// </summary>
        public static ILog WithFlowingContextProperty(
            [NotNull] this ILog log, 
            [NotNull] string contextPropertyName, 
            [CanBeNull] string logPropertyName = null, 
            bool allowOverwrite = false,
            bool allowNullValues = false)
        {
            return log.WithProperty(logPropertyName ?? contextPropertyName, () => GetContextPropertyOrNull(contextPropertyName), allowOverwrite, allowNullValues);
        }

        /// <summary>
        /// <para>Returns a wrapper log that adds values of all properties with names in given <paramref name="names"/> array from <see cref="FlowingContext"/> to each <see cref="LogEvent"/> before logging.</para>
        /// <para>By default, existing properties are not overwritten. This can be changed via <paramref name="allowOverwrite"/> parameter.</para>
        /// <para>By default, <c>null</c> values are not added to events. This can be changed via <paramref name="allowNullValues"/> parameter.</para>
        /// </summary>
        public static ILog WithFlowingContextProperties(
            [NotNull] this ILog log, 
            [NotNull] string[] names, 
            bool allowOverwrite = false,
            bool allowNullValues = false)
        {
            return log.WithProperties(() => names.Select(name => (name, GetContextPropertyOrNull(name))), allowOverwrite, allowNullValues);
        }

        /// <summary>
        /// <para>Returns a wrapper log that adds values of all named properties from <see cref="FlowingContext"/> to each <see cref="LogEvent"/> before logging.</para>
        /// <para>By default, existing properties are not overwritten. This can be changed via <paramref name="allowOverwrite"/> parameter.</para>
        /// <para>By default, <c>null</c> values are not added to events. This can be changed via <paramref name="allowNullValues"/> parameter.</para>
        /// </summary>
        public static ILog WithAllFlowingContextProperties(
            [NotNull] this ILog log, 
            bool allowOverwrite = false,
            bool allowNullValues = false)
        {
            return log.WithProperties(() => FlowingContext.Properties.Current.Select(pair => (pair.Key, pair.Value)), allowOverwrite, allowNullValues);
        }

        /// <summary>
        /// <para>Returns a wrapper log that adds a <see cref="WellKnownProperties.ContextualPrefix"/> property from <see cref="FlowingContext"/> to each <see cref="LogEvent"/> before logging.</para>
        /// <para>See <see cref="ContextualLogPrefix"/> for more info.</para>
        /// </summary>
        public static ILog WithContextualPrefix([NotNull] this ILog log)
        {
            return log.WithProperty(WellKnownProperties.ContextualPrefix, () => ContextualLogPrefix.Current, true);
        }

        private static object GetContextPropertyOrNull(string name)
        {
            return FlowingContext.Properties.Current.TryGetValue(name, out var value) ? value : null;
        } 
    }
}
