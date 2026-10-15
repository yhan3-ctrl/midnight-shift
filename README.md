# Midnight Shift: The Last Lantern

Willow Works 的万圣节最后一班。Unity 6.6 / 6000.6.5f1 / Built-in Render Pipeline。

## 当前版本

v0.2：可玩机制与故事原型。当前不视为最终课程提交版。未完成陌生玩家试玩，不能声称已达到 5–10 分钟或已验证学习成效。

## 打开游戏

Mac 测试版如已构建，位于 `Builds/Mac/Midnight Shift.app`，双击运行。

编辑项目：Unity Hub → Projects → Add → Add project from disk → 选择整个 `MidnightShift` 文件夹。打开后在 Project 面板双击 `Assets/Scenes/WillowWorks.unity`，再按编辑器上方 Play。

## 操作

- WASD：移动；鼠标：视角。
- E：拾取／插入核心／启动机器／读取说明／交付。
- Q：放置手中物品。一次只能拿一件。
- Space 或 Return：继续对白。阅读时世界暂停。
- H：当前任务提示。
- J：值班笔记，查看当前目标、规则和已发现的故事。
- Esc：暂停，调节音乐、效果音和鼠标灵敏度。

机器插入核心后需要再按 E 启动。完成后按 E 取出核心，用于下一台。核心始终只有一枚，不会消耗。没有全局倒计时。

## 本地试玩记录

游戏自动记录当前局部任务完成、活跃操作时间、阅读、暂停、笔记、提示次数和重试次数，仅保存在本机，不上传。位置为 `~/Library/Application Support/Willow Works/Midnight Shift - The Last Lantern/Playtests/`。自动检查会标记为 automated，不能当作真人试玩证据。

## 第一次检查重点

先看能否不依赖旁人指导理解操作与目标，再记录完整游玩时间。请不要把看过答案后的用时当作首次玩家用时。记录卡点和实际策略，不只记录“好玩／不好玩”。

## 开发菜单

Unity 顶部 `Willow Works`：
- Build factory scene：根据生成工具重建场景，会覆盖 `WillowWorks.unity` 中的手工改动。
- Build Mac playable：生成 Mac 通用版本。
- Build Windows / Linux playable：安装相应模块后可用。

场景是实际保存的 Unity 场景，正常打开不需要每次重建。
`Assets/Editor/FactoryVerification.cs` 包含引擎集成检查与场景画面检查工具。

## 待验证和后续完善

- 陌生玩家的真实时长、学习迁移、难度及故事理解。
- 巡检教学已要求实际启动休息铃。是否能在首次游玩时顺利理解，仍需真人验证。
- 最终区域已加入巡检，组合能源复用、压力板和机器声音。难度与敌人导航还需真人测试。
- 美术目前为原创程序化 3D 几何，角色与手部动画为程序动画。需要继续打磨动作和角色表现。
- 音乐、脚步与反馈音为原创合成音，尚需人工试听和混音改进。持续机器环境音还可加强。
- 当前为英文游戏文字、中文开发说明。尚未实现语言切换。
- 尚无跨启动存档。重启或菜单重新开始会重置整晚。
- Mac 构建目标为 Intel + Apple Silicon 通用版；Windows / Linux 模块尚未安装，构建及跨平台完整验证尚未完成。
- GitHub 上传、教师协作者、Box 发布、Blackboard 提交尚未执行。

更多见 Documentation。所有验证记录只描述实际执行的检查，不替代真人试玩。
