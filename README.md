# Project-Presentation
独立游戏: 星之命痕 部分代码及功能演示. 包含 Gif 动图, 注意流量. 
1. 场景, 寻路 与 Npc. 
	世界包含多个城镇, 一个城镇包含多个建筑, 一个建筑有多层房间. 
  A* 寻路, 人物的操作逻辑是按住左键会重新寻路. 
![SceenGif](https://github.com/GameDevBaiyi/Project-Presentation/assets/100526832/de877d64-d40e-4fd1-bfcc-9b290a896b0a)

2. Npc 交互. 
	交谈, 商店, 偷窃, 等交互功能, 操作逻辑是点击到 Npc 即移动到其最近位置并打开交互 Ui. 
![NpcGif](https://github.com/GameDevBaiyi/Project-Presentation/assets/100526832/2b3c2e76-0fb0-4212-ace3-cea52924a28e)
![StealGif](https://github.com/GameDevBaiyi/Project-Presentation/assets/100526832/298af43c-3798-4133-8bab-8ff9613eea9a)

3. 道具. 
	道具分为若干大类, 每个大类又分成若干小类, 大类用于 背包的分类, 小类决定各自的功能实现. 
  如食物可以增加属性, 技能书绘制后可以学到新技能. 
![ItemsGif](https://github.com/GameDevBaiyi/Project-Presentation/assets/100526832/e2cd7585-2dac-4c1b-bcc0-b5a5b9847427)

4. Ui. 
	Editor 工具为 IMGUI, UiToolkit 和 Odin, 用于给策划制作编辑器和 Debug 等.  
	Runtime 工具为 FGUI, Ui 的资源加载为 AssetBundle(其他的资源加载为 Addressables 异步加载). 层级抽象出 7 层, 比如第一层是 伤害数字 在最下层, 最高层为 提示框 等等. 
![UiGif](https://github.com/GameDevBaiyi/Project-Presentation/assets/100526832/3f1aa624-5169-421d-8b4c-98afd52630e1)

5. 漫画演出. 
	异步的实现和检测使用 UniTask 作为工具, Unity 本身的 Coroutine 缺陷太多, .NET 的 Task 功耗太大, 所以 UniTask 是第一选择. 
![DialogueGif](https://github.com/GameDevBaiyi/Project-Presentation/assets/100526832/78c2e12e-0c8f-4348-ae4d-575c5991be69)

6. 剧情引导. 
![PlotGuideGif](https://github.com/GameDevBaiyi/Project-Presentation/assets/100526832/6a6e07e2-a62e-4885-9007-ee89dd7ac2fb)
![PlotGuide2Gif](https://github.com/GameDevBaiyi/Project-Presentation/assets/100526832/9278cd7b-576d-49ac-9eef-2024359b88cb)

7. 布告栏, 悬赏任务和副本. 
	悬赏任务也可以有剧情引导, 任务条件有 完成条件, 失败条件, 额外挑战, 但从程序的角度而言, 被抽象成可任意组合 "且" 关系的小条件, 比如 "有指定阵营存活指定数量" "回合数来到 a~b 回合", 可以组合出客观意义上的 "玩家阵营存活到 10 回合", "第 10 回合某敌方阵营仍有 5 个以上存活", 分别作为完成条件和失败条件等, 所谓的 完成条件和失败条件结构相同, 只是检测满足条件后对战斗的结果赋值不同. 
![BountyTaskGif](https://github.com/GameDevBaiyi/Project-Presentation/assets/100526832/1c8def57-9caf-4a28-9405-a581edfb6df6)

8. 技能, Buff, 技能栏. 
	技能可以选择多轮目标, 每一轮目标各自实现多个效果, 效果由程序实现最小单位, 策划自由组合, 比如 伤害, 治疗, 加 Buff, 可以自由配置参数加在一个技能中. 
Buff 的组成更为复杂, 除了要让效果自由组合, 触发时机和并列触发条件也是参数, 比如一个 Buff 在回合开始触发, 一个在移动时触发, 同时还有一些并列触发条件, 比如 Hp 低于 50 pct, 即使效果一样, 也是不同的 Buff, 但程序只需要实现触发时机即可, Buff 名字和具体参数交给策划配置即可, 最大程度的保证灵活性. 
技能栏动画如果交给前端做较为复杂, 因为理论上每个角色的栏位动画都是不同的, 耗时耗力, 所以我设计了一个简易的编辑器, 将技能栏看成一个迷宫, 每个 节点由 x,y 两个坐标作为 key 确定, 当技能在节点上发生流动时, 寻路然后使用 DoTween 挨个执行动画即可(规定节点之间都是直线). 
![Skill2Gif](https://github.com/GameDevBaiyi/Project-Presentation/assets/100526832/021af305-7ad1-47ad-b052-778febdb2aba)
![SkillGif](https://github.com/GameDevBaiyi/Project-Presentation/assets/100526832/aceb7950-d0d0-4ef7-938e-39086357c205)

9. NpcAi 和 战斗. 
	无论是 Npc 的城镇 Ai 还是战斗 Ai 都由 行为树 实现, 城镇 Ai 较为简单, 只有两个 行为节点, 一个随机移动, 一个原地休息. 战斗 Ai 除行为树的实现外, 还有一个普通行动轴和特殊行动轴, 普通行动轴是一些依次释放的常规技能, 多为各种普通攻击, 特殊行动轴是满足一定条件下优先顶替部分普通行动轴的技能, 比如如果自身血量过低, 释放特殊技能治疗. 比如周围友军没有某 Buff, 施加该 Buff. 所以战斗 Ai 有三个节点, 一个逻辑节点, 没有实际的行为, 但会"思考"接下来要释放哪个技能, 这个技能的朝向, 和要走到哪个位置对哪个点释放等等数据. 另外两个行为节点会根据该逻辑节点的思考结果进行移动和释放技能. 最后由一个 async method 循环上述行为, 直到 Ap 不足以进行任何行为. 
![NpcAiGif](https://github.com/GameDevBaiyi/Project-Presentation/assets/100526832/e17bcc58-e3ea-4c9a-8601-690cb0390d70)





