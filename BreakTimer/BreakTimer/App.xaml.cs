using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents.DocumentStructures;
using System.Windows.Forms;
using System.Windows.Threading;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace BreakTimer
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private NotifyIcon notifyIcon;

        private SettingView settingsWindow;
        private SnoozeWindow snoozeWindow;

        private bool snoozeAlreadyShown = false;

        private WorkTimer workTimer;
        private RestTimer breakTimer;
        private bool isPaused = false;
        private int workMinutes = 90;
        private int breakMinutes = 5; 
        private bool isAway = false;

        private bool isWorking = true;

        private int snoozeMinutes = 5;
        private string stretchText = "";

        string settingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "setting.ini");

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            TraySetting();
            RegisterStartupShortcut();
            InitTimers();

            if (SettingCheck(settingsPath))
            {
                LoadSetting(settingsPath);
                workTimer.Start(workMinutes);
            }
        }
        private void InitTimers()
        {
            workTimer = new WorkTimer();
            breakTimer = new RestTimer();

            workTimer.OnTick += HandleWorkTick;
            workTimer.OnBreakTime += StartBreak;

            breakTimer.OnBreakOver += EndBreak;
        }

        private void StartBreak()
        {
            CloseSnoozeWindow();

            breakTimer.Start(breakMinutes);
            workTimer.Stop();
            ShowOverlay(false);
        }

        private void EndBreak()
        {
            CloseOverlay();
            snoozeAlreadyShown = false;
            workTimer.Start(workMinutes);
        }


        private bool SettingCheck(string settingsPath)
        {
            if (!File.Exists(settingsPath))
            {
                System.Windows.MessageBox.Show("처음 실행하는 것으로 보입니다.\n작업/휴식 시간 설정을 먼저 진행해주세요.",
                    "Break Timer",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                settingsWindow = new SettingView();

                settingsWindow.OnSettingsSaved = () =>
                {
                    LoadSetting(settingsPath);  // 새로 저장된 설정값 읽기
                    workTimer.Start(workMinutes);
                };

                settingsWindow.Show(); // 설정 창 표시
                return false;
            }
            else
            {
                return true;
            }
        }
        private void TraySetting()
        {
            settingsWindow = new SettingView();
            settingsWindow.Hide();  // 기본으로 숨김

            notifyIcon = new NotifyIcon();

            string iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "owlicon.ico");
            notifyIcon.Icon = new Icon(iconPath);
            notifyIcon.Visible = true;
            notifyIcon.Text = "Break Timer";

            // ContextMenuStrip 생성
            var contextMenu = new ContextMenuStrip();

            var awayItem = new ToolStripMenuItem("자리 비움");
            awayItem.Click += (s, e) =>
            {
                if (isAway) return; // 이미 자리 비움 상태면 무시

                isAway = true;
                workTimer.Stop();
                ShowOverlay(true); // ← isAway = true 전달
            };
            contextMenu.Items.Add(awayItem);

            contextMenu.Items.Add(new ToolStripMenuItem("타이머 초기화", null, (s, e) =>
            {
                CloseSnoozeWindow();
                CloseOverlay();

                isAway = false;
                workTimer.Start(workMinutes);
                snoozeAlreadyShown = false;
            }));

            // [설정 열기] 메뉴
            var settingsItem = new ToolStripMenuItem("설정 열기");
            settingsItem.Click += (s, e) =>
            {
                settingsWindow.Show();
                settingsWindow.WindowState = WindowState.Normal;

                settingsWindow.OnSettingsSaved = () =>
                {
                    LoadSetting(settingsPath);  // 새로 저장된 설정값 읽기
                    workTimer.Start(workMinutes);
                };
                settingsWindow.Activate();
            };
            contextMenu.Items.Add(settingsItem);

            // [종료] 메뉴
            var exitItem = new ToolStripMenuItem("종료");
            exitItem.Click += (s, e) =>
            {
                notifyIcon.Visible = false;
                Shutdown();
            };
            contextMenu.Items.Add(exitItem);

            // 연결
            notifyIcon.ContextMenuStrip = contextMenu;
        }

        private bool LoadSetting(string settingsPath)
        {
            var lines = File.ReadAllLines(settingsPath);
            foreach (var line in lines)
            {
                var parts = line.Split('=');
                if (parts.Length != 2) continue;

                string key = parts[0].Trim().ToLower();
                string value = parts[1].Trim();

                switch (key)
                {
                    case "worktime": int.TryParse(value, out workMinutes); break;
                    case "breaktime": int.TryParse(value, out breakMinutes); break;
                    case "snoozetime": int.TryParse(value, out snoozeMinutes); break;
                    case "stretchroutine": stretchText = value.Replace("\\n", "\n"); break;
                }
            }

            return true;
        }

        private void HandleWorkTick(TimeSpan time)
        {
            if (notifyIcon != null && !isAway)
            {
                string text = $"남은 작업 시간: {time.Hours:D2}:{time.Minutes:D2}:{time.Seconds:D2}";
                notifyIcon.Text = text.Length > 63 ? text.Substring(0, 63) : text;
            }

            if (isAway)
            {
                notifyIcon.Text = "자리 비움 중입니다";
            }

            if (!snoozeAlreadyShown && time <= TimeSpan.FromMinutes(1))
            {
                ShowSnoozeWindow();
                snoozeAlreadyShown = true;
            }
        }

        private OverlayWindow overlay;

        private void ShowOverlay(bool isAway)
        {
            if (snoozeWindow != null)
            {
                snoozeWindow.Close();
                snoozeWindow = null;
            }

            if (overlay == null)
            {
                if (isAway)
                {
                    overlay = new OverlayWindow("자리 비움 중입니다", snoozeMinutes, true);
                }
                else
                {
                    overlay = new OverlayWindow(stretchText, snoozeMinutes, false);
                }

                overlay.OnSnoozeRequested = () => workTimer.AddTime(TimeSpan.FromMinutes(snoozeMinutes));
                overlay.OnClosedByUser = () => {
                    CloseOverlay();
                    workTimer.Start(workMinutes);
                    snoozeAlreadyShown = false;
                    this.isAway = false;
                };
                overlay.Show();
            }
        }

        private void CloseOverlay()
        {
            if (overlay != null)
            {
                overlay.Close();
                overlay = null;
            }
        }

        private void ShowSnoozeWindow()
        {
            if (snoozeWindow != null) return;

            snoozeWindow = new SnoozeWindow(snoozeMinutes, workTimer.TimeLeft);

            snoozeWindow.OnSnoozeRequested = () =>
            {
                workTimer.AddTime(TimeSpan.FromMinutes(snoozeMinutes));
                CloseSnoozeWindow();
            };

            snoozeWindow.OnDismissed = () => CloseSnoozeWindow();

            snoozeWindow.Closed += (s, e) => snoozeWindow = null;
            var screen = System.Windows.SystemParameters.WorkArea;
            snoozeWindow.Left = screen.Right - snoozeWindow.Width - 20;
            snoozeWindow.Top = screen.Bottom - snoozeWindow.Height - 20;
            snoozeWindow.Show();
        }

        private void CloseSnoozeWindow()
        {
            if (snoozeWindow != null)
            {
                snoozeWindow.Close();
                snoozeWindow = null;
            }
        }

        private void RegisterStartupShortcut()
        {
            string startupFolder = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            string shortcutPath = Path.Combine(startupFolder, "BreakTimer.lnk");

            if (File.Exists(shortcutPath)) return; // 이미 등록된 경우 패스

            string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;

            // COM 라이브러리 사용
            IWshRuntimeLibrary.WshShell shell = new IWshRuntimeLibrary.WshShell();
            IWshRuntimeLibrary.IWshShortcut shortcut = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(shortcutPath);

            shortcut.TargetPath = exePath;
            shortcut.WorkingDirectory = Path.GetDirectoryName(exePath);
            shortcut.WindowStyle = 1;
            shortcut.Description = "BreakTimer 자동 시작";
            shortcut.Save();
        }
    }
}
