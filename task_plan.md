# CAD Project Manager V0.1 实施计划

## 目标

在 `E:\CAD插件开发` 建立 AutoCAD CAD Project Manager 的 V0.1 安全加载器骨架。首轮只实现可编译的 Solution、Core/AutoCAD/Infrastructure 三个项目、最小 `IExtensionApplication`、安全日志、统一命令异常边界、`CADPM_HEALTH` 与 `CADPM_INFO`。

## 用户请求与附件边界

- 用户明确请求：产品开发目录为 `E:\CAD插件开发`。
- 附件作为本轮实施约束：只做 V0.1；不提前实现 Palette、数据库、压缩包、文件监视、DWG 扫描、AI、网络、常驻线程或修改 DWG/系统变量。
- 当前没有授权修改 AutoCAD 全局配置、复制 Autodesk Managed DLL 或发布到用户的 AutoCAD 安装目录；用户已提供本机 AutoCAD 2025 路径用于适配与手工验收。

## 阶段

| 阶段 | 状态 | 验收 |
|---|---|---|
| 1. 环境与计划恢复 | complete | 目标目录为空；读取附件；记录 AutoCAD/.NET 环境 |
| 2. Solution 与项目骨架 | complete | Core、AutoCAD、Infrastructure、tests、docs、diagnostics 结构存在 |
| 3. V0.1 实现 | complete | 初始化轻量；命令带异常边界；日志失败静默降级；无 UI/网络/后台任务 |
| 4. 本地验证 | partial | SDK 编译、静态检查、日志 smoke 通过；AutoCAD Core Console 在插件执行前因宿主 `AcGeomentObj.dbx` 错误退出 |

## 当前验收标准

- [x] `CADProjectManager.sln` 可被目标环境打开并完成 SDK 构建。
- [x] Core 不引用 Autodesk Managed DLL。
- [x] AutoCAD 引用使用外部 SDK 路径，`Copy Local=false`，不打包 Autodesk DLL。
- [x] `Initialize()` 只做安全日志 bootstrap，不创建窗口、不联网、不读 DWG、不启动后台工作。
- [x] `CADPM_HEALTH` 与 `CADPM_INFO` 由统一异常边界包装，且只读。
- [x] 日志写入 `%LOCALAPPDATA%\\CADProjectManager\\logs`，独立运行时 smoke 通过。
- [ ] AutoCAD 2025 GUI/Core Console 完成真实 NETLOAD 与 V0.1 生命周期验收；当前宿主在插件执行前因 `AcGeomentObj.dbx` 报错退出，不能标记通过。

## 当前结论

源码和构建层 V0.1 完成；宿主层保持 `HOST-UNVERIFIED`。AutoCAD 2025 GUI/Core Console 均未完成插件命令验证，因为 Core Console 在脚本执行前报告 `AcGeomentObj.dbx` 缺失并退出，相关证据保存在 `diagnostics/accoreconsole-acad.err`。

## 错误记录

| 错误/限制 | 处理 |
|---|---|
| 当前 `dotnet` 只有 runtime，没有 SDK；`msbuild` 不在 PATH | 先生成可审查的项目文件与测试；构建验证需后续补齐 SDK 或使用 Visual Studio/MSBuild |
| 已确认 AutoCAD 2025 Managed API 位于 `D:\Autodesk\CAD2025\AutoCAD 2025`，文件版本 25.0.58.0.0 | 使用 .NET 8 和可配置 `AUTOCAD_MANAGED_API_DIR`；不复制 Autodesk DLL |
