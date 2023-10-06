using System;

using LowLevelSystems.SceneSystems.Base;

using Sirenix.OdinInspector;

namespace LowLevelSystems.SceneSystems.DungeonSceneSystems
{
[Serializable]
public class DungeonScene : Scene,IHasScenePrefab
{
    [Title("Data")]
    [ShowInInspector]
    private readonly ScenePrefabEnum _scenePrefabEnum;

    public ScenePrefabEnum ScenePrefabEnumPy => this._scenePrefabEnum;

    public DungeonScene(int instanceId,ScenePrefabEnum scenePrefabEnum) : base(instanceId,SceneTypeEnum.Dungeon)
    {
        this._scenePrefabEnum = scenePrefabEnum;
    }
}
}