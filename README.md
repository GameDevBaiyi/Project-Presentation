# Project-Presentation
独立游戏: 星之命痕 部分功能演示. 
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
	Runtime 工具为 FGUI, 资源加载为 AssetBundle. 层级抽象出 7 层, 比如第一层是 伤害数字 在最下层, 最高层为 提示框 等等. 
![UiGif](https://github.com/GameDevBaiyi/Project-Presentation/assets/100526832/3f1aa624-5169-421d-8b4c-98afd52630e1)

5. 漫画演出. 
	异步的实现和检测使用 UniTask 作为工具, Unity 本身的 Coroutine 缺陷太多, .NET 的 Task 功耗太大, 所以 UniTask 是第一选择. 
![DialogueGif](https://github.com/GameDevBaiyi/Project-Presentation/assets/100526832/78c2e12e-0c8f-4348-ae4d-575c5991be69)

6. 剧情引导. 
![PlotGuideGif](https://github.com/GameDevBaiyi/Project-Presentation/assets/100526832/6a6e07e2-a62e-4885-9007-ee89dd7ac2fb)


