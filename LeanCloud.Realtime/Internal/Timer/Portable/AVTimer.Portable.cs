using System;
using System.Threading.Tasks;
using System.Threading;

namespace LeanCloud.Realtime.Internal
{
    internal delegate void TimerCallback(object state);

    internal sealed class Timer : CancellationTokenSource, IDisposable
    {
        internal Timer(TimerCallback callback, object state, int dueTime, int period)
        {
            Task.Delay(dueTime, Token).ContinueWith((t, s) =>
            {
                var tuple = (Tuple<TimerCallback, object>)s;

                while (Enabled)
                {
                    if (IsCancellationRequested)
                        break;
                    Task.Run(() => tuple.Item1(tuple.Item2));
                    Task.Delay(period);
                }
            },
            Tuple.Create(callback, state),
            CancellationToken.None,
            TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion,
            TaskScheduler.Default);
        }

        public new void Dispose() { base.Cancel(); }

        public bool Enabled
        {
            get; set;
        }
    }

    public class AVTimer : IAVTimer
    {
        public AVTimer()
        {

        }

        Timer timer;

        public bool Enabled
        {
            get
            {
                return timer.Enabled;
            }
            set
            {
                timer.Enabled = value;
            }
        }

        public double Interval
        {
            get; set;
        }

        public void Start()
        {
            if (timer == null) timer = new Timer((state) =>
            {
                Elapsed(this, new TimerEventArgs(DateTime.Now));
            }, this, (int)Interval, (int)Interval);
        }

        public void Stop()
        {
            if (timer != null) timer.Enabled = false;
        }

        public event EventHandler<TimerEventArgs> Elapsed;
    }
}
