# Korean Railway Track Editor (한국 철도 배선 편집기)

![License](https://img.shields.io/badge/license-MIT-blue.svg)
![.NET](https://img.shields.io/badge/.NET-10.0-blueviolet)

A powerful, visual railway track layout editor built with WPF and .NET 10. This tool allows users to design complex railway track diagrams with signals, switches, and track circuits using a modern, intuitive interface.

WPF와 .NET 10을 기반으로 제작된 강력한 시각적 철도 배선 편집기입니다. 이 도구를 사용하면 현대적이고 직관적인 인터페이스를 통해 신호기, 분기기, 궤도 회로를 포함한 복잡한 철도 배선도를 설계할 수 있습니다.

## Key Features (주요 기능)

### 🛤 Component Management (컴포넌트 관리)
- **Signals (신호기)**: Supports various signal types including Main (3/45/5) and Shunt signals. Highly customizable with Route indicators, Guide icons, TTB, and Siso status icons.
- **Switches (분기기)**: Flexible branching track components with Normal and Reverse state visualization. 
- **Track Circuits (궤도 회로)**: Segmented track parts with real-time occupancy (Gray/Red) visualization.

### 🖱 Intuitive Editing (직관적인 편집)
- **Drag & Drop**: Easily place and move components on the canvas.
- **Grid Snapping**: Automated 10px grid snapping for precise alignment.
- **Handle-based Resizing**: Manipulate switch points (A, S, R, N) and track segments directly via interactive handles.
- **Rubber-band Selection**: Multi-select components by dragging a selection box.
- **Keyboard Shortcuts**: Support for Ctrl+C (Copy), Ctrl+V (Paste), and Delete.

### 🔍 Navigation (탐색 및 뷰 제어)
- **Pan & Zoom**: Smooth canvas navigation using middle-click panning and Ctrl+Wheel zooming.
- **Property Panel**: Detailed property editing for every selected component.

### 💾 Data Persistence (데이터 저장 및 불러오기)
- **Save/Load**: Persist your layouts in JSON format for later editing or integration.

## Tech Stack (기술 스택)

- **Framework**: .NET 10.0 Windows (WPF)
- **Architecture**: MVVM (Model-View-ViewModel)
- **Graphics**: Pure XAML Vector Drawings for high-quality, scalable icons.
- **Serialization**: System.Text.Json

## Getting Started (시작하기)

### Prerequisites (사전 요구 사항)
- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) or later.
- Visual Studio 2022 (latest version) or JetBrains Rider.

### Installation & Run (설치 및 실행)
1. Clone the repository:
   ```bash
   git clone https://github.com/your-username/KoreanRailwayTrackEditor.git
   ```
2. Navigate to the project directory:
   ```bash
   cd KoreanRailwayTrackEditor
   ```
3. Run the application:
   ```bash
   dotnet run
   ```

## Folder Structure (폴더 구조)
- `Models/`: Data models for railway components (Signal, Switch, etc.).
- `ViewModels/`: Application logic and data binding.
- `Converters/`: XAML data converters for visual logic.
- `Resources/`: Icons and static assets.
- `MainWindow.xaml`: Core UI layout and interaction logic.

## License (라이선스)

Distributed under the MIT License. See `LICENSE` for more information.

Copyright (c) **Sehwa Co., Ltd.** All rights reserved.
