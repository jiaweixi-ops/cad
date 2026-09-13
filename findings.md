# Findings

## 2026-09-08

- 目标产品目录最初为 `E:\CAD插件开发`；该路径属于当时开发环境记录，不是仓库运行或构建前提。
- 设计方案文件最初位于本机下载目录；V0.1 实施边界已同步到仓库文档。
- V0.1 只做安全加载器、日志、异常边界、`CADPM_HEALTH`、`CADPM_INFO`；V0.2 才开始 Palette。
- Core 必须不知道 AutoCAD，AutoCAD API 只能集中在 AutoCAD 项目；Autodesk Managed DLL 不得复制进发布目录。
- 运行数据写入 `%LOCALAPPDATA%\CADProjectManager`；如果 LocalApplicationData 无法提供绝对路径，则退到系统临时目录，禁止退化为 AutoCAD 当前工作目录中的相对路径。
- AutoCAD 2025 adapter 使用 .NET 8；Managed API 目录通过 `AUTOCAD_MANAGED_API_DIR` 或 `AutoCADManagedApiDir` 显式提供，仓库不内置机器专属安装路径。
- 首次 `dotnet build --no-restore` 失败为首次构建前置问题：三个项目缺少 `obj/project.assets.json`；应执行一次正常 restore/build，而不是重复 `--no-restore`。
- Core Console smoke test 在插件脚本执行前出现 `Unhandled Delayload "AcGeomentObj.dbx" Module Not Found` 并退出；AutoCAD GUI 基线启动也未进入正常绘图工作区。
- 上述 `AcGeomentObj.dbx` 信息只能证明 AutoCAD 宿主在插件加载前发生启动失败。当前证据不足以确定该模块名对应的具体缺失/损坏文件，也不能仅凭目录中存在 `AcGeoLocationObj.dbx` 推断根因；更不能把该宿主失败归因于 CADProjectManager。
- 因 Gate A（无插件 AutoCAD 启动基线）失败，`NETLOAD`、`CADPM_HEALTH`、`CADPM_INFO` 和完整生命周期验收均未执行。项目状态保持 `source-ready / host-unverified`。

## 设计决定

- 采用 SDK-style .NET 8 项目作为 AutoCAD 2025 第一适配骨架，并把 Autodesk 引用目录外置为环境变量/MSBuild 属性，不在仓库中硬编码开发机安装目录。
- 静态验证只覆盖源码结构与安全边界；它不能替代 AutoCAD 真实宿主测试。
- AutoCAD 真实命令测试保留为宿主验收 Gate；在纯 AutoCAD 基线恢复前不进入 V0.2。
