# Findings

## 2026-09-08

- 目标产品目录：`E:\CAD插件开发`，初始为空。
- 设计方案文件：`C:\Users\奚嘉威\Downloads\AutoCAD_CADProjectManager_V0.1_to_V1.0_Safe_Plugin_Plan.md`。
- 附件要求 V0.1 只做安全加载器、日志、异常边界、`CADPM_HEALTH`、`CADPM_INFO`；V0.2 才开始 Palette。
- Core 必须不知道 AutoCAD，AutoCAD API 只能集中在 AutoCAD 项目；Autodesk Managed DLL 不得复制进发布目录。
- 附件规定运行数据写入 `%LOCALAPPDATA%\\CADProjectManager`，不得写 AutoCAD 安装目录、用户 DWG 目录或注册表。
- 本机 `dotnet --info` 显示 .NET runtime 8/10，但没有 .NET SDK；`msbuild` 不在 PATH。
- 用户提供并已确认 AutoCAD 2025 位于 `D:\Autodesk\CAD2025\AutoCAD 2025\acad.exe`，Managed API 为 `acmgd.dll`、`acdbmgd.dll`、`accoremgd.dll`，文件版本均为 `25.0.58.0.0`。
- AutoCAD 当前未运行，因此尚未执行 NETLOAD 或命令验收。
- 首次 `dotnet build --no-restore` 失败为预期的首次构建前置问题：三个项目缺少 `obj/project.assets.json`；应执行一次正常 restore/build，而不是重复 `--no-restore`。
- 第一次 Core Console smoke test 未进入插件：从含中文工作目录启动时，AutoCAD 2025 输出 `Unhandled Delayload "AcGeomentObj.dbx" Module Not Found` 并退出；截图同时显示 AutoCAD 错误报告窗口。后续改用安装目录工作目录，但一次 staging 命令的复制源使用了错误的相对路径，未形成有效插件测试。
- 修正 staging 复制路径后，Core Console 仍在加载脚本前报告同一个 `AcGeomentObj.dbx` 缺失并退出；AutoCAD 目录实际存在的是 `AcGeoLocationObj.dbx`。因此当前宿主 smoke test 是环境/安装层阻塞，不能作为插件失败证据，也不能标记 NETLOAD 通过。

## 设计决定

- 采用 SDK-style .NET 8 项目作为 AutoCAD 2025 第一适配骨架，并把 Autodesk 引用目录外置为 MSBuild 属性；默认仅在检测到用户确认的本机安装目录时使用它。
- 测试先覆盖不依赖 AutoCAD 的 Core/Infrastructure 行为；AutoCAD 真实命令测试保留为手工验收清单。
