using Vostok.Logging.Abstractions;

namespace Vostok.Logging.Context
{
    public interface IILogWrapper : ILog
    {
        ILog BaseLog { get; }
    }
}