# 《撤离测试场》

一个使用 Unity 2022.3 LTS 制作的俯视角 3D 生存撤离 Demo。玩家需要在小型场景中移动、战斗、收集补给与实验样本，并携带任务物品到撤离点完成结算。

## 当前状态

- 本地可演示 MVP 已完成。
- Unity Play Mode、Windows Development Player 和 Windows Release Player 均已验证。
- Release 全流程冒烟测试通过，无已知阻断错误。

## 操作

| 输入 | 功能 |
|---|---|
| WASD | 移动 |
| 鼠标 | 瞄准 |
| 鼠标左键 | 射击 |
| E | 与最近的物品或撤离点交互 |
| 数字键 1—6 | 使用对应背包槽位中的物品 |
| F5 | 保存玩家状态 |
| F9 | 读取玩家状态 |

## 已实现功能

- 角色移动、重力、碰撞、俯视相机和鼠标朝向。
- 玩家射击、近战追击敌人、远程射击敌人和生命系统。
- 玩家与敌人共享的子弹对象池，支持池容量不足时动态扩充。
- `IInteractable`统一交互规则，支持世界物品和撤离点使用同一条E键调用链。
- 使用`ScriptableObject ItemData`配置的六种物品，以及六格背包、堆叠、消耗和UI刷新。
- 饮水、治疗、任务样本、撤离条件、失败提示、成功结算和重新开始。
- F5/F9 JSON玩家状态存档，保存生命、饮水和背包物品ID/数量。
- 实时生命/饮水UI、六格背包UI和中文TextMeshPro字体。

## 主要设计

- 交互层只查找`IInteractable`，具体对象自己实现拾取或撤离行为。
- `ItemData`保存共享且稳定的物品定义；`InventorySlot`保存运行时物品引用和数量。
- 背包通过`InventoryChanged`事件通知UI，背包不直接依赖具体UI组件。
- 子弹由`Queue`对象池复用，减少高频射击中的重复创建和销毁。
- JSON只保存稳定ID和普通数值，读取时再通过Known Items把ID还原为`ItemData`资产引用。
- 状态UI先比较已显示整数，再用TMP `SetText`复用字符缓冲区，减少无效刷新和托管字符串分配。

## 已验证的性能改进

- Development Player中，状态UI数值变化帧的`GC Alloc`由78 B降为0 B，周期分配尖峰消失。
- 双方同帧命中时，移除`Health.TakeDamage`高频成功日志后，相关分配由约2.8 KB降至48—72 B，降低97%以上。
- 持续战斗前后总内存约296.6—297.2 MB、Managed Heap约2.5—2.6 MB，Game Objects保持68、Scene Objects保持551，未观察到持续扩容或内存泄漏趋势。

以上数据来自Windows Development Player的受控对比；Release版本另行完成了完整功能冒烟测试。

## 运行与构建

- Unity版本：2022.3.57f1c1。
- 渲染管线：URP 14.0.11。
- 输入：Input System 1.11.2。
- 主场景：`Assets/_Project/Scenes/GameScene.unity`。
- 当前Windows Release输出：`Builds/WindowsRelease/Running.exe`（构建目录不纳入Git）。
- Release存档：可执行文件同级的`SaveData/player-save.json`。

## 当前范围限制

- 场景和敌人AI保持小型Demo规模，没有导航网格、复杂行为树和动态关卡。
- 存档只覆盖玩家生命、饮水和背包，不保存敌人、世界拾取物和撤离状态。
- 当前为单机键鼠版本，不包含联网、手柄和完整设置菜单。

## 项目文档

- [项目状态与验证](创建项目.md)
- [实际操作与测试证据](操作与作用.md)
- [设计思路与逻辑链](思路.md)
- [Unity与C#知识整理](知识.md)
- [复习与自测](通过项目理解.md)
