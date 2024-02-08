namespace Shuttle.Esb.Throttle
{
    public interface IThrottlePolicy
    {
        bool ShouldAbort();
    }
}