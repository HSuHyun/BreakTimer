using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace BreakTimer
{
    /// <summary>
    /// SnoozeWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SnoozeWindow : Window
    {
        public Action OnSnoozeRequested;
        public Action OnDismissed;

        private int remainingSeconds;
        private DispatcherTimer countdownTimer;

        public SnoozeWindow(int snoozeMin, TimeSpan timeUntilBreak)
        {
            InitializeComponent();

            snoozeButton.Content = $"스누즈 ({snoozeMin})분";

            remainingSeconds = (int)timeUntilBreak.TotalSeconds;
            txtCountdown.Text = FormatTime(remainingSeconds);

            countdownTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            countdownTimer.Tick += CountdownTimer_Tick;
            countdownTimer.Start();
        }

        private void CountdownTimer_Tick(object sender, EventArgs e)
        {
            remainingSeconds--;
            if (remainingSeconds <= 0)
            {
                countdownTimer.Stop();
                OnDismissed?.Invoke();
                this.Close(); // 자동 종료
            }
            else
            {
                txtCountdown.Text = FormatTime(remainingSeconds);
            }
        }

        private string FormatTime(int seconds)
        {
            TimeSpan ts = TimeSpan.FromSeconds(seconds);
            return $"휴식까지 남은 시간: {ts.Minutes:D2}:{ts.Seconds:D2}";
        }

        private void Snooze_Click(object sender, RoutedEventArgs e)
        {
            countdownTimer.Stop();
            OnSnoozeRequested?.Invoke();
            this.Close();
        }

        private void Dismiss_Click(object sender, RoutedEventArgs e)
        {
            countdownTimer.Stop();
            OnDismissed?.Invoke();
            this.Close();
        }
    }

}
