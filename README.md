# CAD Project Manager

AutoCAD CAD 图纸管家，当前实现边界为 V0.1 安全加载器。

本仓库位于 `E:\CAD插件开发`。V0.1 的唯一目标是：插件能够被 AutoCAD 加载，并提供不修改图纸、不修改 AutoCAD 系统设置的健康检查命令。

## 当前包含

- `CADProjectManager.Core`：与 AutoCAD 无关的版本和路径模型。
- `CADProjectManager.Infrastructure`：失败静默降级的本地日志 bootstrap。
- `CADProjectManager.AutoCAD`：`IExtensionApplication`、异常边界、`CADPM_HEALTH`、`CADPM_INFO`。
- `tools/verify-v01.ps1`：不依赖 AutoCAD 的静态安全边界检查。
- `docs/V0.1_ACCEPTANCE.md`：构建、NETLOAD 和零影响验收清单。

## 构建前提

本项目第一适配目标是 AutoCAD 2025，使用 .NET 8 Managed API。编译 `CADProjectManager.AutoCAD` 前，需要：

1. Visual Studio 2022 / MSBuild，或可用的 .NET 8 SDK。
2. 将环境变量 `AUTOCAD_MANAGED_API_DIR` 指向当前 AutoCAD 2025 的 Managed API 目录，该目录应包含 `AcMgd.dll`、`AcDbMgd.dll`、`AcCoreMgd.dll`。
3. 使用与开发机实际 AutoCAD 版本匹配的 adapter 和引用，不把 Autodesk DLL 复制到本项目或发布目录。

> `.NET Framework 4.8 Developer Pack` 不属于当前 AutoCAD 2025 / `net8.0` adapter 的必要前提；只有未来适配 AutoCAD 2021–2024 等 .NET Framework 目标时才单独处理。

示例：

```powershell
$env:AUTOCAD_MANAGED_API_DIR = 'D:\Autodesk\CAD2025\AutoCAD 2025'
dotnet build .\CADProjectManager.sln -c Debug
```

项目会自动使用已确认的本机目录 `D:\Autodesk\CAD2025\AutoCAD 2025`；通过环境变量可以覆盖该默认值。构建通过只代表源码/依赖层可用，不能替代真实 AutoCAD 2025 GUI 中的 `NETLOAD` 与生命周期验收。

## V0.1 命令

- `CADPM_HEALTH`：输出插件存活状态、版本、AutoCAD 版本和运行时版本。
- `CADPM_INFO`：输出插件路径、配置根目录和日志路径。

`CADPM_SHOW`、`CADPM_HIDE`、Palette、SQLite、压缩包导入、文件索引、DWG 读取和 AI 均留在后续阶段。

## 当前验收状态

当前状态：**source-ready / host-unverified**。

在真实 AutoCAD 2025 GUI 中完成以下项目之前，不得将 V0.1 标记为 PASS：

- `NETLOAD` 成功加载 `CADProjectManager.AutoCAD.dll`；
- `CADPM_HEALTH` 连续执行 100 次无异常；
- `CADPM_INFO` 正常输出；
- 新建、打开、关闭、保存、另存为、Undo/Redo、Plot Preview、多文档切换正常；
- AutoCAD 正常退出；
- 加载前后相关系统变量无插件引起的变化；
- 测试图纸无插件引起的业务数据变化。
