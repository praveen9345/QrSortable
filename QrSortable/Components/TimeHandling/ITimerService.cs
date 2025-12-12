namespace QrSortable.Components.TimeHandling
{
    using System;

    /// <summary>
    ///     An interface providing the execution of timer functionality.
    /// </summary>
    public interface ITimerService
    {
        /// <summary>
        ///     Starts a timer with the specified interval and callback function.
        /// </summary>
        /// <param name="interval">The interval of the timer to trigger the callback function.</param>
        /// <param name="callback">The callback function that gets executed as soon as the interval is passed.</param>
        /// <returns>The identifier for the created timer.</returns>
        Guid StartTimer(TimeSpan interval, Func<bool> callback);

        /// <summary>
        ///     Stops the timer with the given ID.
        /// </summary>
        /// <param name="identifier">The identifier of the timer that shall be stopped.</param>
        void StopTimer(Guid identifier);

        /// <summary>
        ///     Starts a periodic timer that triggers a callback function at specified intervals.
        /// </summary>
        /// <param name="timerCallback">The callback function to be invoked on each timer tick.</param>
        /// <param name="interval">The time interval between each invocation of the callback function.</param>
        /// <returns>A Timer object that can be used to control the periodic timer.</returns>
        Timer StartPeriodicTimer(TimerCallback timerCallback, TimeSpan interval);

        /// <summary>
        ///     Stops the periodic timer with the given ID.
        /// </summary>
        /// <param name="identifier">The identifier of the timer that shall be stopped.</param>
        void StopPeriodicTimer(Timer identifier);

    }
}