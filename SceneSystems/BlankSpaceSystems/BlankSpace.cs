using System;

using LowLevelSystems.SceneSystems.Base;

namespace LowLevelSystems.SceneSystems.BlankSpaceSystems
{
/// <summary>
/// 专门用来躲避战斗的空间. 如果某个场景发生了战斗, 将所有 Npc 和 Pc 都放到此场景.
/// </summary>
[Serializable]
public class BlankSpace : Scene
{
    public BlankSpace(int instanceId) : base(instanceId,SceneTypeEnum.BlankSpace)
    {
    }
}
}