using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Documents;

namespace BreakTimer
{
    public partial class SettingView : Window
    {
        public Action OnSettingsSaved; // 외부에서 연결 가능

        public SettingView()
        {
            InitializeComponent();
            LoadSettings(); // 생성 시 자동 호출
        }

        private string GetSettingsPath()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "setting.ini");
        }

        private void SaveSettings(object sender, RoutedEventArgs e)
        {
            SaveSettingsToFile();
            MessageBox.Show("설정이 저장되었습니다!", "저장 완료", MessageBoxButton.OK, MessageBoxImage.Information);
            OnSettingsSaved?.Invoke(); // 등록된 콜백 호출
            this.Hide(); // 설정창은 숨김 처리
        }
        private void SaveSettingsToFile()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"worktime={txtWorkTime.Text}");
            sb.AppendLine($"breaktime={txtBreakTime.Text}");
            sb.AppendLine($"snoozetime={txtSnoozeTime.Text}");

            TextRange range = new TextRange(rtbStretchRoutine.Document.ContentStart, rtbStretchRoutine.Document.ContentEnd);
            string routineText = range.Text.Trim().Replace("\r\n", "\\n");
            sb.AppendLine($"stretchroutine={routineText}");

            File.WriteAllText(GetSettingsPath(), sb.ToString());
        }

        private void LoadSettings()
        {
            string path = GetSettingsPath();

            if (File.Exists(path))
            {
                // 기존 설정 불러오기
                var lines = File.ReadAllLines(path);
                foreach (var line in lines)
                {
                    var parts = line.Split('=');
                    if (parts.Length != 2) continue;

                    string key = parts[0].Trim().ToLower();
                    string value = parts[1].Trim();

                    switch (key)
                    {
                        case "worktime": txtWorkTime.Text = value; break;
                        case "breaktime": txtBreakTime.Text = value; break;
                        case "snoozetime": txtSnoozeTime.Text = value; break;
                        case "stretchroutine":
                            string restored = value.Replace("\\n", "\r\n");
                            rtbStretchRoutine.Document.Blocks.Clear();
                            rtbStretchRoutine.Document.Blocks.Add(new Paragraph(new Run(restored)));
                            break;
                    }
                }
            }
        }
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);

            // 윈도우 닫기(X 버튼) 시 종료 방지 + 숨기기
            e.Cancel = true;
            this.Hide();
        }
    }
}
