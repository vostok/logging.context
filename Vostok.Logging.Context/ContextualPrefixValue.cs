namespace Vostok.Logging.Context
{
    // TODO(iloktionov): unit tests

    internal class ContextualPrefixValue
    {
        private ContextualPrefixValue(string value)
            => Value = value;

        public string Value { get; }

        public override string ToString() => Value;

        public static ContextualPrefixValue operator+(ContextualPrefixValue prefix, string append)
           => new ContextualPrefixValue(string.Concat(prefix?.Value ?? string.Empty, "[", append ?? string.Empty, "] "));
    }
}
