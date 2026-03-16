# macOS 史努比屏幕保护

素材是从 tvOS 中提取的，开源了，视频素材没放到版本库，可以在 Release 总下载安装包，在里边的 Resouces 中就能找到。

应用没有签名，所以打开的时候会提示危险什么的，在系统设置-隐私和安全性-滚到最后，仍然打开。

这个屏保的史努比出现顺序还没有摸透，因为 bugOS 的原因，昨晚浪费了极长时间进行调试，累了，随便了。

后续有空会研究研究到底怎么播放的。

成品在 release 里边直接下就行。

@刚修好数码 就是我本人😂

---
## v0.2.1

拖了好久，终于有时间搞一搞，解决了，都解决了！

这个版本大家可以放心使用了，解决了 legacyScreenSaver 的内存问题。

原来 macOS 屏幕保护关闭的时候不会调用 stopAnimation() 函数，非常神奇，而是响应了一个 com.apple.screensaver.willstop 通知。


## v0.1.1

这个版本有写问题，有时候打开会黑屏，时间久了会卡顿，等过阵子有空搞一下。我估计是 avqueueplayer 的队列问题。

由于 macOS 系统的 bug，安装新版本屏幕保护之后，需要重启一下电脑才可以生效。

改用 SpriteKit 播放视频，支持 HEVC 的 alpha 通道，以显示背景。

---
## Windows 版本（WPF）

我补了一个 Windows 版本的实现，目录在 `windows/`，目标是尽量还原 tvOS / macOS 版本的播放逻辑与视觉层级：

- 随机纯色背景
- 半色调纹理叠加（`halftone_pattern.png`）
- 随机背景图
- 前景视频播放（按 Intro / Loop / Outro 规则组装）

### 目录结构

- `windows/Snoopy.Windows.sln`
- `windows/Snoopy.Windows/`
  - `MainWindow.xaml` / `MainWindow.xaml.cs`
  - `Models/Clip.cs`
  - `Services/ClipService.cs`
  - `Services/PlaybackSequenceBuilder.cs`
  - `Assets/`
    - `Videos/`（放 `.mov` / `.mp4`）
    - `Backgrounds/`（放 `.jpg/.jpeg/.png/.webp`）
    - `Patterns/halftone_pattern.png`

### 运行方式

1. 安装 .NET 8 SDK（Windows）
2. 在仓库根目录执行：

```bash
cd windows/Snoopy.Windows
dotnet run
```

### 素材放置

从你已经打包好的 macOS Release 素材中提取资源并放入：

- 视频 -> `windows/Snoopy.Windows/Assets/Videos`
- 背景图 -> `windows/Snoopy.Windows/Assets/Backgrounds`
- 纹理图 -> `windows/Snoopy.Windows/Assets/Patterns/halftone_pattern.png`

> 注意：Windows 上对带 alpha 的 HEVC `.mov` 兼容性受系统编解码器影响。如果出现透明通道无法显示，可先转码为你机器可播放的格式，或改用支持 alpha 的渲染管线（如 FFmpeg + D3D 方案）。


### Release（推荐）

如果你要稳定产出 Windows 可分发包，建议直接用 **Windows 环境**：

#### 方案 A：GitHub Actions（自动 Windows 环境）

仓库已添加工作流：`.github/workflows/windows-release.yml`

- 手动触发：`Actions -> Windows Release Build -> Run workflow`
- 打 tag（如 `v0.3.0`）后会自动：
  1) 在 `windows-latest` 上构建
  2) `dotnet publish -r win-x64 --self-contained true`
  3) 打包 `Snoopy.Windows-win-x64.zip`
  4) 上传 artifact；tag 构建时自动附加到 GitHub Release

#### 方案 B：本地 Windows 机器

安装 .NET 8 SDK 后，在 PowerShell 执行：

```powershell
cd windows
./build-release.ps1 -Runtime win-x64
```

或者 CMD：

```cmd
cd windows
build-release.cmd win-x64
```

输出包路径：

- `windows/release/Snoopy.Windows-win-x64.zip`
