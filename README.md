# Midnight Shift: The Last Lantern

Willow Works 的万圣节最后一班。Unity 6.6 / 6000.6.5f1 / Built-in Render Pipeline。

## 当前版本

v0.7：默认英文，新增统一语言菜单，提供 English、简体中文、Español、Français、Deutsch、日本語、한국어 七种语言。教学、正式关卡待办、界面与剧情对白均有翻译，长对白自动分页；品牌名称与工厂内招牌保留英文。保留逐步教学、大字界面、挂历待办、退出菜单与音频恢复。包含四个从教学到综合运用的线路谜题、可见的灯笼制作阶段，以及可选的 Button 对话和幽灵回应。当前不视为最终课程提交版。未完成陌生玩家试玩，不能声称已达到 5–10 分钟或已验证学习成效。

## 打开游戏

双击本文件夹中的 `开始试玩.app`。它指向本机 `~/Applications/Midnight Shift.app`。

当前开发源文件位于本机 `~/Developer/MidnightShift`，避免 iCloud 卸载源代码。桌面旧项目保留为 v0.5 参考。可玩副本放在本机应用程序文件夹，避开 iCloud 自动加入的程序包元数据。`Builds/MidnightShift-Mac-v0.7.zip` 是保留签名的打包副本；`Builds/Mac` 为 Unity 原始构建输出。

编辑项目：Unity Hub → Projects → Add → Add project from disk → 选择整个 `MidnightShift` 文件夹。打开后在 Project 面板双击 `Assets/Scenes/WillowWorks.unity`，再按编辑器上方 Play。

## 操作

- WASD：移动；鼠标或方向键：视角。K 切换为仅方向键观察。
- E：拾取／插入核心／启动机器／读取说明／交付。
- Q：把物品放在前方一步的位置。货箱靠近压板时会轻微吸附对齐；一次只能拿一件。
- Space 或 Return：继续对白。阅读时世界暂停。
- H：当前任务提示。
- J：值班笔记，查看当前目标、规则和已发现的故事。
- Tab / F1：随时打开菜单、释放鼠标，包括对话与线路板期间。Esc 在对话中也可打开菜单；在线路板中返回工厂。
- 菜单内可退出游戏、试听/恢复声音，并打开七种语言的选择菜单（L）。
- 对话暂停世界；点击继续或按空格/回车推进。右侧挂历与金色箭头逐步指路。

生产机器插入核心后，再按 E 打开线路板。旋转线路，使电源经过标记接点并到达全部出口，再点 Send power 启动。线路板支持鼠标，也支持方向键选格、空格旋转、回车启动、Esc 关闭。休息铃无需线路谜题。完成后按 E 取出核心，用于下一台。核心始终只有一枚，不会消耗。没有全局倒计时。

## 本地试玩记录

游戏自动记录当前局部任务完成、活跃操作时间、阅读、暂停、笔记、提示次数和重试次数，仅保存在本机，不上传。Mac 独立应用位置为 `~/Library/Application Support/com.Willow-Works.Midnight-Shift---The-Last-Lantern/Playtests/`；编辑器检查记录位于 `~/Library/Application Support/Willow Works/Midnight Shift - The Last Lantern/Playtests/`。自动检查及带 --agent-playtest 的操作会标记为 automated，并自动静音；其余记录默认 reviewer=unattributed，不能仅凭日志认定是真人试玩。

计时包含未暂停时的思考与停顿，长时间离开或系统休眠会污染数据；这类记录必须剔除，不能作为课程时长证据。完整开发者试玩见 `Documentation/NATIVE-FULL-REVIEW-v0.5.md`。

## 第一次检查重点

先看能否不依赖旁人指导理解操作与目标，再记录完整游玩时间。请不要把看过答案后的用时当作首次玩家用时。记录卡点和实际策略，不只记录“好玩／不好玩”。

## 开发菜单

Unity 顶部 `Willow Works`：
- Build factory scene：根据生成工具重建场景，会覆盖 `WillowWorks.unity` 中的手工改动。
- Build Mac playable：生成 Mac 通用版本。
- Build Windows / Linux playable：安装相应模块后可用。

`Tools/build-mac.zsh` 提供可重现的构建、本机副本签名、发布包启动检查和 ZIP 打包流程。脚本会重建生成场景，手工改过场景时先保存到 Git 或另存副本。

场景是实际保存的 Unity 场景，正常打开不需要每次重建。
`Assets/Editor/FactoryVerification.cs` 包含引擎集成检查与场景画面检查工具。

## 待验证和后续完善

- 陌生玩家的真实时长、学习迁移、难度及故事理解。
- 巡检教学已要求实际启动休息铃。是否能在首次游玩时顺利理解，仍需真人验证。
- 最终区域已加入巡检，组合能源复用、压力板和机器声音。难度与敌人导航还需真人测试。
- 美术目前为原创程序化 3D 几何，角色与手部动画为程序动画。需要继续打磨动作和角色表现。
- 音乐、脚步与反馈音为原创合成音，尚需人工试听和混音改进。持续机器环境音还可加强。
- 游戏首次启动默认英文。标题或暂停菜单选择 Language（也可按 L），支持七种语言并记住选择。开发说明为中文。
- 尚无跨启动存档。重启或菜单重新开始会重置整晚。
- Mac 构建目标为 Intel + Apple Silicon 通用版；Windows / Linux 模块尚未安装，构建及跨平台完整验证尚未完成。
- GitHub 上传、教师协作者、Box 发布、Blackboard 提交尚未执行。

更多见 Documentation。所有验证记录只描述实际执行的检查，不替代真人试玩。
