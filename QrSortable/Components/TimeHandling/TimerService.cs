namespace QrSortable.Components.TimeHandling
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///     Implementation of a timer service providing timer functionality.
    /// </summary>
    public class TimerService : ITimerService
    {
        /// <summary>
        ///     Holds the identifier of a timer and its current execution state (true, if running; false if stopped).
        /// </summary>
        private readonly Dictionary<Guid, bool> _timerExecutionState = new Dictionary<Guid, bool>();

        /// <summary>
        ///     Starts a timer with the specified interval and callback function.
        /// </summary>
        /// <param name="interval">The interval of the timer to trigger the callback function.</param>
        /// <param name="callback">The callback function that gets executed as soon as the interval is passed.</param>
        /// <returns>The identifier for the created timer.</returns>
        public Guid StartTimer(TimeSpan interval, Func<bool> callback)
        {
            var identifier = Guid.NewGuid();
            _timerExecutionState.Add(identifier, true);

            DispatcherExtensions.StartTimer(Application.Current.Dispatcher, interval, () =>
            {
                if (!_timerExecutionState.ContainsKey(identifier))
                {
                    return false;
                }

                if (_timerExecutionState[identifier])
                {
                    var result = callback.Invoke();
                    if (result)
                    {
                        return true;
                    }
                }

                _timerExecutionState.Remove(identifier);
                return false;
            });
            return identifier;
        }

        /// <summary>
        ///     Stops the timer with the given ID.
        /// </summary>
        /// <param name="identifier">The identifier of the timer that shall be stopped.</param>
        public void StopTimer(Guid identifier)
        {
            if (!_timerExecutionState.ContainsKey(identifier))
            {
                return;
            }

            _timerExecutionState[identifier] = false;
        }

        /// <summary>
        ///     Starts a periodic timer that triggers a callback function at specified intervals.
        /// </summary>
        /// <param name="timerCallback">The callback function to be invoked on each timer tick.</param>
        /// <param name="interval">The time interval between each invocation of the callback function.</param>
        /// <returns>A Timer object that can be used to control the periodic timer.</returns>
        public Timer StartPeriodicTimer(TimerCallback timerCallback, TimeSpan interval)
        {
            return new Timer(timerCallback, null, 0, (int)interval.TotalMilliseconds);
        }

        /// <summary>
        ///     Stops the periodic timer with the given ID.
        /// </summary>
        /// <param name="identifier">The identifier of the timer that shall be stopped.</param>
        public void StopPeriodicTimer(Timer identifier)
        {
            identifier?.Dispose();
        }
    }
}