# 《撤离边界》

一个使用 Unity 2022.3 LTS 制作的俯视角 3D 生存撤离 Demo。玩家需要在小型场景中移动、战斗、收集补给与实验样本，并携带任务物品到撤离点完成结算。

## 当前状态

- 本地可演示 MVP 已完成。
- 基础MVP已通过Unity Play Mode、Windows Development Player和Windows Release Player验证，Release全流程冒烟测试通过。
- 后续新增的近战共享配置、物品ID字典读档、CSV配置导入及命中帧距离复查已通过用户Play Mode验证；不代表这些新增功能已完成新一轮Development/Release构建验证。

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
- 近战敌人的`Idle/Chase/Attack`代码状态机，以及接口驱动的近战/远程攻击策略组件。
- 玩家与敌人共享的子弹对象池，支持池容量不足时动态扩充。
- `IInteractable`统一交互规则，支持世界物品和撤离点使用同一条E键调用链。
- 使用`ScriptableObject ItemData`配置的六种物品，以及六格背包、堆叠、消耗和UI刷新。
- 编辑器菜单支持导入固定路径的物品CSV，按物品ID更新已有`ItemData`资产，保留资产GUID及已有引用。
- 饮水、治疗、任务样本、撤离条件、失败提示、成功结算和重新开始。
- F5/F9 JSON玩家状态存档，保存生命、饮水和背包物品ID/数量。
- 实时生命/饮水UI、六格背包UI和中文TextMeshPro字体。
- `Health`在数据真实变化后发布`Damaged`、`HealthChanged`和`Died`事件；生命UI、受伤闪红和死亡处理分别按需订阅。
- `TargetDummy`已完成一个程序化倒地样品：先关闭碰撞，再让视觉子对象平滑倒下、短暂停留，最后让根对象失活。
- `TargetDummy`已接入对象池伤害飘字：按实际伤害显示3D TMP数字，完成上升和渐隐后回池复用，池不足时支持动态扩容。
- 玩家治疗成功后由`Health.Healed`发布实际恢复量，并复用同一浮动文字对象池显示绿色`+数值`；满血治疗不通知也不消耗物品，已通过Play Mode验证。
- `ChaserEnemy`已建立无外部角色素材的Animator占位样品：玩法根对象与`VisualRoot`分离，Idle/Chase按AI状态自动切换；`TryAttack成功 → Attacked → Trigger → Animation Event → ApplyPendingHit`已通过Play Mode验证，伤害会在动画命中帧结算。
- 近战命中帧会复查本次保存目标的存活、激活状态与水平距离；超出命中范围则挥空，保留攻击冷却，不临时换目标。已于2026-09-11获用户Play Mode验证确认。
- 新增`FactionMember`与`HostileTargetFinder`：阵营身份独立于Health，近战和远程AI定时使用`OverlapSphereNonAlloc`筛选最近的存活敌对目标；Dummy临时设为Player后，敌人先选Dummy、其后重新选择玩家的流程已验证。

## 主要设计

- 交互层只查找`IInteractable`，具体对象自己实现拾取或撤离行为。
- `ItemData`保存共享且稳定的物品定义；`InventorySlot`保存运行时物品引用和数量。
- 背包通过`InventoryChanged`事件通知UI，背包不直接依赖具体UI组件。
- 子弹由`Queue`对象池复用，减少高频射击中的重复创建和销毁。
- 敌人AI负责判断何时攻击，`IEnemyAttack`统一调用入口，具体策略组件负责近战扣血或远程发射。
- `MeleeAttackData`共享配置保存伤害和冷却时长；各敌人组件独立保存下一次攻击时间和待命中目标，不把运行时状态写回共享资产。
- JSON只保存稳定ID和普通数值；`Awake`将Known Items数组整理为`Dictionary<string, ItemData>`，读档时按ID查回已有资产引用，再恢复各槽位数量。
- 配置导入由`MenuItem`注册菜单，按约定列解析并校验CSV；通过`AssetDatabase`查找已有资产、建立ID字典，再用`SerializedObject`修改字段并保存，不删除重建资产。
- 状态UI先比较已显示整数，再用TMP `SetText`复用字符缓冲区，减少无效刷新和托管字符串分配。
- `Health`只负责生命数据与规则，攻击者只请求伤害，UI、反馈和死亡后果由独立组件响应事件，避免战斗脚本直接认识具体表现。
- 死亡样品把根对象与`VisualRoot`分开：根对象保留逻辑和碰撞职责，视觉子对象负责旋转与位移表现。
- 伤害飘字按`Health.Damaged → DamageNumberEmitter → DamageNumberPool → DamageNumberView`传递；池使用`Queue`管理空闲完整文字对象，每次借出重置运行状态。
- 治疗飘字按`Health.TryHeal → Healed(actualHeal) → HealingNumberEmitter → DamageNumberPool → DamageNumberView`传递；伤害与治疗共享池，借出时分别设置文字格式和颜色。
- 敌人代码状态机负责距离决策、移动、朝向和攻击请求，Animator状态机只负责`VisualRoot`表现；`State`整数表达持续状态，`Attack` Trigger表达一次成功攻击。
- AI只通过`IEnemyAttack`请求“开始一次攻击”：近战把目标暂存到命中帧再结算，远程从对象池发射子弹；目标搜索按照候选、筛选、比较、执行的统一流程选择最近敌对目标。

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
- 程序化倒地已在`TargetDummy`和`ChaserEnemy`验证；ChaserEnemy死亡时会关闭AI、攻击、Animator与CharacterController，再旋转视觉子对象并失活。尚未接入骨骼角色、正式死亡动画、布娃娃或复活重置。
- 伤害飘字已在`TargetDummy`验证，绿色治疗飘字已在玩家验证；暴击、准确命中点、容量上限和飘字性能对比仍未实现或验证。
- 同阵营子弹过滤代码已接入Projectile；最近敌对目标选择已通过Dummy与Player切换验证，但友军挡枪场景仍保留为一次专项回归测试。当前目标选择只按阵营、存活和距离判断，尚未加入视线遮挡、仇恨值或目标优先级。

## 项目文档

- [项目状态与验证](创建项目.md)
- [实际操作与测试证据](操作与作用.md)
- [设计思路与逻辑链](思路.md)
- [Unity与C#知识整理](知识.md)
- [复习与自测](通过项目理解.md)
- [CSV配置导入说明](配置导入说明.md)
