# CAD Project Manager V0.1 实施计划

## 目标

建立 AutoCAD CAD Project Manager 的 V0.1 安全加载器骨架。首轮只实现可编译的 Solution、Core/AutoCAD/Infrastructure 三个项目、最小 `IExtensionApplication`、安全日志、统一命令异常边界、`CADPM_HEALTH` 与 `CADPM_INFO`。

## 实施边界

- V0.1 只做安全加载器；不提前实现 Palette、数据库、压缩包、文件监视、DWG 扫描、AI、网络、常驻线程或修改 DWG/系统变量。
- Core 不引用 AutoCAD API；AutoCAD API 集中在 AutoCAD adapter。
- Autodesk Managed DLL 只作为构建期外部引用，不复制进发布目录。
- AutoCAD Managed API 路径通过 `AUTOCAD_MANAGED_API_DIR` 或 `AutoCADManagedApiDir` 显式提供，仓库不硬编码开发机路径。

## 阶段

| 阶段 | 状态 | 验收 |
|---|---|---|
| 1. 环境与计划恢复 | complete | 已明确 V0.1 边界并记录 AutoCAD/.NET 环境 |
| 2. Solution 与项目骨架 | complete | Core、AutoCAD、Infrastructure、docs、diagnostics、tools 结构存在 |
| 3. V0.1 实现 | complete | 初始化轻量；命令带异常边界；日志失败静默降级；无 UI/网络/后台任务 |
| 4. 本地验证 | complete for source/static | SDK 构建和静态检查通过；输出不包含 Autodesk Managed DLL |
| 5. AutoCAD 宿主验收 | blocked | 无插件 AutoCAD 2025 GUI 基线启动失败，尚未进入 NETLOAD |

## 当前验收标准

- [x] `CADProjectManager.sln` 可完成 SDK 构建。
- [x] Core 不引用 Autodesk Managed DLL / AutoCAD API。
- [x] AutoCAD 引用使用外部 SDK 路径，`Private=false`，不打包 Autodesk DLL。
- [x] `Initialize()` 只做安全日志 bootstrap，不创建窗口、不联网、不读 DWG、不启动后台工作。
- [x] `CADPM_HEALTH` 与 `CADPM_INFO` 由统一异常边界包装，且只读。
- [x] 日志写入插件私有目录；LocalApplicationData 不可用时不得退化到相对路径。
- [x] 静态验证器对文件缺失、引用策略、Core 隔离、禁止行为和输出 DLL 做显式检查。
- [ ] AutoCAD 2025 无插件 GUI 启动基线 PASS。
- [ ] AutoCAD 2025 GUI 完成真实 NETLOAD 与 V0.1 生命周期验收。

## 当前结论

源码、构建和静态安全层 V0.1 已完成；宿主层保持 `source-ready / host-unverified`。

当前阻塞发生在插件加载前：AutoCAD 2025 GUI/Core Console 均出现宿主启动错误，现有证据只证明 AutoCAD 本体未通过 Gate A，不能归因于 CADProjectManager，也不足以确认具体 DBX 根因。

在 Gate A 恢复之前，不进入 V0.2 Palette 开发。
