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

namespace BreakTimer
{
    /// <summary>
    /// OverlayWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class OverlayWindow : Window
    {
        public Action OnSnoozeRequested;
        public Action OnClosedByUser;

        private int snoozeMinutes;

        public OverlayWindow(string stretchText, int snoozeMin, bool isAwayMode)
        {
            InitializeComponent();
            txtRoutine.Text = stretchText;
            snoozeMinutes = snoozeMin;
            if (isAwayMode)
            {
                snoozeBtn.Visibility = Visibility.Collapsed;
            }
            else
            {
                snoozeBtn.Visibility = Visibility.Visible;
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            OnClosedByUser?.Invoke();
            this.Close();
        }

        private void Snooze_Click(object sender, RoutedEventArgs e)
        {
            OnSnoozeRequested?.Invoke();
            this.Close();
        }
    }
}
