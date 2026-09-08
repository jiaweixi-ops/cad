# Progress

## 2026-09-08

- [x] 确认用户指定产品目录为 `E:\CAD插件开发`。
- [x] 读取并区分附件中的 V0.1 实施边界与用户目录请求。
- [x] 完成历史会话恢复检查；目标目录无既有项目文件。
- [x] 完成环境初检并确认 AutoCAD 2025 与三份 Managed API。
- [x] 创建 Solution 与三项目骨架。
- [x] 实现 V0.1 加载器、命令与日志。
- [x] 添加文档和静态安全边界验证脚本。
- [x] 使用隔离的 .NET SDK 8.0.424 完成 Debug 构建验证。
- [x] 最终 `dotnet build --no-restore` 通过，输出未复制 Autodesk Managed DLL。
- [x] 静态 V0.1 检查通过。
- [x] Core Console smoke test 已尝试，但宿主在插件脚本执行前因 `AcGeomentObj.dbx` 延迟加载错误退出；该错误当前不归因于插件。
- [x] AutoCAD 生成的 `acad.err` 已保留到 `diagnostics/accoreconsole-acad.err`。
- [x] V0.1 已提交并推送到 `main`。
- [x] 2026-09-08 验收整改：`GlobalExceptionBoundary` 已将 Document/Editor 获取纳入完整异常边界。
- [x] 2026-09-08 验收整改：`tools/verify-v01.ps1` 已逐项验证 AutoCAD 引用 `Private=false`，并检查现有 build 输出不得包含 Autodesk Managed DLL。
- [x] 2026-09-08 验收整改：README 已纠正 AutoCAD 2025 为 .NET 8 构建前提，不再把 .NET Framework 4.8 Developer Pack 列为当前 adapter 必需项。
- [ ] 在真实 AutoCAD 2025 GUI 中执行 `NETLOAD`。
- [ ] `CADPM_HEALTH` 连续执行 100 次无异常。
- [ ] 执行 `CADPM_INFO` 并核对插件/config/log 路径。
- [ ] 完成新建、打开、关闭、保存、另存为、Undo/Redo、Plot Preview、多文档切换生命周期测试。
- [ ] 对比加载前后关键系统变量，确认无插件引起的变化。
- [ ] 对比测试图纸，确认无插件引起的业务数据变化。
- [ ] 正常关闭 AutoCAD 并确认无退出卡死或异常。

## 当前状态

**source-ready / host-unverified**

在上面的 AutoCAD 2025 GUI 宿主测试全部完成之前，V0.1 不得标记为 `PASS`，也不应进入 V0.2 Palette 功能开发。
