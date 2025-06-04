using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace BreakTimer
{
    public class WorkTimer
    {
        private TimeSpan timeLeft;
        private DispatcherTimer timer;
        public TimeSpan TimeLeft => timeLeft;

        public event Action OnBreakTime;
        public event Action<TimeSpan> OnTick;

        public void Start(int minutes)
        {
            timer?.Stop();
            timeLeft = TimeSpan.Zero;
            timeLeft = TimeSpan.FromMinutes(minutes);
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Tick;
            timer.Start();
        }

        public void Stop()
        {
            timer?.Stop();
            timeLeft = TimeSpan.Zero;
        }
        private void Tick(object sender, EventArgs e)
        {
            timeLeft -= TimeSpan.FromSeconds(1);
            OnTick?.Invoke(timeLeft);

            if (timeLeft <= TimeSpan.Zero)
            {
                timer.Stop();
                OnBreakTime?.Invoke();
            }
        }
        public void AddTime(TimeSpan time)
        {
            timeLeft += time;
        }
    }

    public class RestTimer
    {
        public event Action OnBreakOver;
        private TimeSpan timeLeft;
        private DispatcherTimer timer;

        public void Start(int minutes)
        {
            timeLeft = TimeSpan.FromMinutes(minutes);
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Tick;
            timer.Start();
        }

        private void Tick(object sender, EventArgs e)
        {
            timeLeft -= TimeSpan.FromSeconds(1);

            if (timeLeft <= TimeSpan.Zero)
            {
                timer.Stop();
                OnBreakOver?.Invoke();
            }
        }
    }
}
