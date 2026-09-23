# 2D Mini Game Collection（Unity）

用 Unity 实现的 2D 小游戏合集，**每个小游戏是一个独立场景**：

| 小游戏 | 场景 | 状态 | 说明 |
| --- | --- | --- | --- |
| **Snake（贪吃蛇）** | `Assets/Scenes/Snake.unity` | ✅ 完成 | 网格移动、蛇身跟随、吃食物变长、撞边界重置 |
| **Pong（乒乓对战）** | `Assets/Scenes/Pong.unity` | ✅ 完成 | 玩家 vs AI 对手，反弹面 + 计分区 + 实时比分 |
| Mario | `Assets/Scenes/Mario.unity` | ⬜ 占位 | 仅一个空场景，玩法尚未实现 |

## 操作

| 小游戏 | 按键 | 行为 |
| --- | --- | --- |
| Snake | <kbd>W</kbd> <kbd>A</kbd> <kbd>S</kbd> <kbd>D</kbd> | 改变蛇的移动方向（不能 180° 反向） |
| Pong | <kbd>W</kbd> / <kbd>↑</kbd>、<kbd>S</kbd> / <kbd>↓</kbd> | 控制左侧挡板上/下移动（右侧是 AI） |

## 玩法

- **Snake**：蛇头按格子前进，吃到食物在尾部增长一节；撞到边界（`Barrier`）则销毁蛇身、蛇头归零、方向复位，重新开始一局。
- **Pong**：小球在两侧挡板与上下反弹面之间往返；球越过挡板进入计分区即得分，比分用 TextMeshPro 实时显示，得分后小球与双方挡板复位并重新发球（发球方向随机）。

## 技术栈

| 分类 | 内容 |
| --- | --- |
| 引擎 | Unity 2022.3.62f1c1（LTS） |
| 语言 | C# |
| 2D | `com.unity.feature.2d` 2.0.1（Sprite / Tilemap 等 2D 包集） |
| UI 文本 | TextMeshPro 3.0.7 |
| 输入 | 旧版 Input（`Input.GetKey`，WASD / 方向键） |
| 物理 | Unity 2D 物理（Rigidbody2D / Collider2D / 触发器） |

## 核心实现

### Snake —— 蛇身跟随与网格移动

```csharp
private List<Transform> segments;   // segments[0] 是蛇头

private void FixedUpdate()
{
    // 从尾到头倒序遍历：每一节跟上它前面那一节
    for (int i = segments.Count - 1; i > 0; i--)
        segments[i].position = segments[i - 1].position;

    // 先把蛇头取整对齐到网格，再按方向走一格
    transform.position = new Vector3(
        Mathf.Round(transform.position.x) + direction.x,
        Mathf.Round(transform.position.y) + direction.y, 0f);
}
```

- **倒序遍历是必须的**：正序遍历会让每一节读到已经被覆盖的位置，整条蛇瞬间重叠成一节。
- **先取整再步进**：保证蛇头永远落在格点上，不会因为浮点误差越走越偏。
- **转向做合法性判断**：按 <kbd>W</kbd> 要求当前方向不是向下、按 <kbd>D</kbd> 要求不是向左，避免 180° 反向直接撞死自己。
- **碰撞用触发器 + Tag 分派**：项目自定义了 `Snake` / `Food` / `Barrier` 三个 Tag，`food.cs` 被蛇碰到后在自己身上重新随机位置，`Snake.cs` 按 Tag 决定「增长」还是「重置」。
- **食物生成范围取自场景里的 `BoxCollider2D.bounds`**，并在范围内取整点，调整棋盘大小只需在 Scene 里拖这个 Collider。

### Pong —— 基类继承 + AI 对手 + 事件驱动计分

**1）挡板用继承复用**：`Paddle` 基类只放 `speed` 与 `Rigidbody2D`，`PlayerPaddle` 与 `RobotPaddle` 各自实现移动策略。

**2）玩家挡板用物理力驱动**：

```csharp
private void Update()        // 采样输入 → 方向
{
    if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))      direction = Vector2.up;
    else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) direction = Vector2.down;
    else direction = Vector2.zero;
}

private void FixedUpdate()   // 在固定步长里施加力
{
    if (direction != Vector2.zero) rb.AddForce(direction * speed);
}
```

**3）AI 对手不做「上帝视角」**：只有当球朝自己飞来（`ball.velocity.x > 0`）时才追踪球的高度；球飞向对面时，挡板缓慢回到中位——对手看起来会「反应」而不是无脑同步。

```csharp
if (ball.velocity.x > 0)   // 球朝我飞来 → 追球
{
    if (ball.position.y > transform.position.y)      rb.AddForce(Vector2.up   * speed);
    else if (ball.position.y < transform.position.y) rb.AddForce(Vector2.down * speed);
}
else                       // 球飞走了 → 回中位
{
    if (0f > transform.position.y) rb.AddForce(Vector2.up   * speed);
    if (0f < transform.position.y) rb.AddForce(Vector2.down * speed);
}
```

**4）发球方向随机、且不会水平直飞**：左右随机，纵向在 `[0.5, 1]` 或 `[-1, -0.5]` 之间随机，归一化后乘速度——每次开局方向都不同，同时保证有足够的纵向分量。

**5）反弹面按接触法线补力**：`ReboundSurface` 在碰撞时取 `collision.GetContact(0).normal`，用 `-normal * elasticity` 施加一次力，反弹手感由 `elasticity` 参数控制，而不是依赖物理材质的默认反弹。

```csharp
Vector2 normal = collision.GetContact(0).normal;
ball.AddForce(-normal * elasticity);
```

**6）计分区用事件解耦**：`ScoringArea` 只负责「球碰到我了」这件事，通过 `EventTrigger.TriggerEvent`（UnityEvent）在 Inspector 里绑定到 `GameManager.PlayerGetScore` / `RobotGetScore`，计分逻辑完全不用知道球和场地结构。

**7）回合复位集中在一处**：`GameManager.ResetStatus()` 统一把球归中并重新发球、双方挡板回到中线，得分与复位流程只有一条路径。

## 工程结构

```
Assets/
├── Scenes/
│   ├── Snake.unity          # 贪吃蛇（已完成）
│   ├── Pong.unity           # 乒乓对战（已完成）
│   └── Mario.unity          # 占位场景（未实现）
├── scripts/
│   ├── Snake/
│   │   ├── Snake.cs         # 蛇头移动 / 蛇身跟随 / 增长 / 重置
│   │   └── food.cs          # 食物随机位置
│   └── Pong/
│       ├── Paddle.cs        # 挡板基类（speed + Rigidbody2D）
│       ├── PlayerPaddle.cs  # 玩家挡板（键盘输入）
│       ├── RobotPaddle.cs   # AI 挡板
│       ├── Ball.cs          # 小球：随机发球 / 位置与速度复位
│       ├── ReboundSurface.cs# 反弹面：按接触法线补力
│       ├── ScoringArea.cs   # 计分区：触发 UnityEvent
│       └── GameManager.cs   # 比分显示 + 回合复位
├── prefabs/                 # Segment.prefab（蛇身）
├── Sprites/ materials/      # 美术资源
└── TextMesh Pro/            # TMP 必需资源
```

## 如何运行

1. 安装 **Unity 2022.3.62f1c1（LTS）**（Unity Hub → 安装编辑器）。
2. Unity Hub → `Add` → `Add project from disk` → 选择本项目根目录。
3. 打开 `Assets/Scenes/Snake.unity` 或 `Assets/Scenes/Pong.unity`，点击 Play。

> `Library/`、`Temp/`、`Logs/`、`.vs/` 等目录已通过 `.gitignore` 排除，首次打开会重新导入资源。

## 后续计划

- [ ] Snake：计分、随长度加速、穿墙模式
- [ ] Pong：先到 N 分的回合制胜负、AI 难度递增、音效
- [ ] Mario：补齐关卡（Tilemap + 角色控制器）
- [ ] 统一到一个主菜单场景，做成合集入口
