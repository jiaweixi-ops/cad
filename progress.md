# Progress

## 2026-09-08

- [x] 明确 V0.1 安全加载器边界：不提前实现 Palette、数据库、压缩包、DWG 扫描、AI、网络或常驻后台任务。
- [x] 创建 Solution 与 Core / Infrastructure / AutoCAD 三项目骨架。
- [x] 实现 V0.1 加载器、`CADPM_HEALTH`、`CADPM_INFO`、日志和统一异常边界。
- [x] 使用 .NET 8 SDK 完成 Debug 构建验证。
- [x] 构建输出未复制 `AcMgd.dll`、`AcDbMgd.dll`、`AcCoreMgd.dll`。
- [x] `GlobalExceptionBoundary` 已将 Document/Editor 获取纳入完整异常边界。
- [x] `tools/verify-v01.ps1` 已修复缺文件失败路径：必需文件缺失时记录 FAIL，而不是由 `Get-Content` / XML 解析提前终止。
- [x] 静态验证器只读取 `.cs` / `.csproj` 源文件并排除 `bin` / `obj`；失败项逐条输出，成功显式 `exit 0`。
- [x] `BootstrapLogger` 已保证一次 `Error()` 只生成一条物理日志记录，并支持跨零点切换到新日期日志文件。
- [x] 新建日志使用 UTF-8 without BOM。
- [x] `PluginPaths.LocalRoot` 已防止 `LocalApplicationData` 异常时退化为相对路径；回退到系统临时目录。
- [x] `CADPM_INFO` 现在区分“配置目录尚未创建”和“logging disabled”，不再把推导路径冒充为实际可用日志。
- [x] `SafeMode` 常量已更名为 `BuildLabel`，避免误读为运行时模式。
- [x] AutoCAD Managed API 构建路径不再硬编码任何开发机目录，只接受环境变量/MSBuild 属性。
- [x] Core Console smoke 脚本改用 ASCII staging 路径 `D:\Temp\CADProjectManager.V0.1`。
- [x] README、Findings、Task Plan 和 Acceptance 文档已移除/纠正机器专属路径与宿主错误归因。

## Host acceptance

- [x] 已尝试 AutoCAD 2025 无插件启动基线。
- [ ] Gate A PASS：当前 AutoCAD 2025 在进入正常绘图工作区前发生宿主错误中断。
- [ ] 在真实 AutoCAD 2025 GUI 中执行 `NETLOAD`。
- [ ] `CADPM_HEALTH` 连续执行 100 次无异常。
- [ ] 执行 `CADPM_INFO` 并核对插件/config/log 状态。
- [ ] 完成新建、打开、关闭、保存、另存为、Undo/Redo、Plot Preview、多文档切换生命周期测试。
- [ ] 对比加载前后关键系统变量，确认无插件引起的变化。
- [ ] 对比测试图纸，确认无插件引起的业务数据变化。
- [ ] 正常关闭 AutoCAD 并确认无退出卡死或异常。

## 当前状态

**source-ready / host-unverified**

当前阻塞发生在 `NETLOAD` 之前。现有 `AcGeomentObj.dbx` 延迟加载错误只作为 AutoCAD 宿主失败证据保存，不能据此归因到 CADProjectManager，也不能在缺少进一步宿主诊断时断言具体 DBX 文件根因。

在 AutoCAD 2025 Gate A 和后续 GUI 宿主测试全部完成之前，V0.1 不得标记为 `PASS`，也不进入 V0.2 Palette 功能开发。
