# 🕒 BreakTimer

> **휴식 주기 알림 데스크톱 앱 (WPF / C#)**  
> 장시간 작업 중인 사용자가 일정 주기로 휴식을 취할 수 있도록 돕는 **Windows용 타이머 유틸리티**입니다.  
> 일정 시간이 지나면 화면 전체에 오버레이 창을 띄워 휴식을 유도하고, 스누즈(일시정지)나 주기 설정 기능을 제공합니다.

---

## 📦 프로젝트 개요

| 항목 | 내용 |
|------|------|
| **개발 언어** | C# (.NET Framework / .NET 6 이상) |
| **프레임워크** | WPF (Windows Presentation Foundation) |
| **UI 구조** | XAML + Code-behind (MVVM 일부 적용) |
| **IDE** | Visual Studio |
| **목적** | 일정 주기마다 휴식 알림을 제공하는 간단한 데스크톱 유틸리티 |

---

## 📁 폴더 구조

BreakTimer/  
┣ BreakTimer/  
┃ ┣ Properties/  
┃ ┣ Resources/ # 아이콘, 이미지, 사운드 등 리소스  
┃ ┣ bin/Debug/ # 빌드 결과물  
┃ ┣ obj/Debug/ # 빌드 캐시  
┃ ┣ App.xaml # 전역 스타일 및 리소스  
┃ ┣ App.xaml.cs # 애플리케이션 진입 로직  
┃ ┣ BreakTimer.csproj # C# 프로젝트 설정  
┃ ┣ MainWindow.xaml # 메인 타이머 화면  
┃ ┣ MainWindow.xaml.cs  
┃ ┣ OverlayWindow.xaml # 휴식 알림 오버레이 UI  
┃ ┣ OverlayWindow.xaml.cs  
┃ ┣ SettingView.xaml # 사용자 설정 화면  
┃ ┣ SettingView.xaml.cs  
┃ ┣ SnoozeWindow.xaml # 스누즈(일시정지) 창  
┃ ┣ SnoozeWindow.xaml.cs  
┃ ┣ Timer.cs # 핵심 타이머 로직  
┃ ┗ App.config  
┣ BreakTimer.sln # Visual Studio 솔루션  
┗ .gitignore  

---

## 💡 주요 기능

| 기능 | 설명 |
|------|------|
| ⏰ **타이머 알림** | 설정된 시간 주기로 휴식 알림 창을 표시합니다. |
| 💤 **스누즈 기능** | “10분 후 다시 알림” 등으로 알림을 미룰 수 있습니다. |
| 🖥️ **오버레이 창** | 전체 화면 반투명 알림창으로 시각적 집중 유도 |
| ⚙️ **설정창 제공** | 타이머 주기, 문구, 알림음 등을 커스터마이징 가능 |
| 🔄 **지속 실행** | 작업 중에도 백그라운드로 동작 가능 (트레이 상주 예정) |

---

## 🔧 주요 로직 요약

### `Timer.cs`
- `DispatcherTimer`를 사용해 주기적인 Tick 이벤트 발생  
- 설정된 시간마다 휴식 알림 트리거  
- 타이머 중복 실행 방지 로직 포함

### `OverlayWindow.xaml`
- 전체 화면 오버레이 형태로 표시  
- “휴식 완료” 클릭 시 닫히고 타이머 재시작  
- UI 비침/포커스 처리로 사용성 확보

### `SnoozeWindow.xaml`
- 일시적으로 알림을 미루는 기능 (예: 10분 후 다시 알림)

### `SettingView.xaml`
- 사용자가 설정한 휴식 간격/메시지/사운드를 로컬에 저장 및 로드

---

## 설계 포인트

- **비침 오버레이 UI** → 집중 흐름을 방해하지 않는 부드러운 알림  
- **DispatcherTimer 기반** → UI 스레드와 안전하게 연동  
- **멀티윈도우 구조** → 메인 / 오버레이 / 스누즈 / 설정 창 분리  
- **MVVM 일부 적용** → UI와 로직의 독립성 향상  

---

## 실행 방법

1️⃣ Visual Studio에서 `BreakTimer.sln` 파일 열기  
2️⃣ `Ctrl + F5` 또는 ▶️ 버튼으로 실행  
3️⃣ 기본 설정  
   - 휴식 주기: 50분  
   - 스누즈 시간: 10분  

빌드 결과물은 `/bin/Debug/` 폴더에 생성됩니다.

---

## 🖼️ 사용 흐름

1. 사용자가 휴식 주기를 설정  
2. 설정된 시간이 지나면 **오버레이 창** 표시  
3. “휴식 완료” 클릭 시 타이머 리셋  
4. “10분 후 다시” 클릭 시 스누즈 적용  

---

## 🗺️ 향후 개선 계획

- [ ] 트레이 아이콘 상주 기능  
- [ ] 설정 자동 저장 / 프로그램 재시작 시 복원  
- [ ] 커스텀 사운드 지원  
- [ ] 다국어 UI 지원 (한국어/영어)  
- [ ] 자동 실행 옵션 추가  

---

## 👤 개발자

**HSuHyun**  
> [🔗 GitHub Profile](https://github.com/HSuHyun)
