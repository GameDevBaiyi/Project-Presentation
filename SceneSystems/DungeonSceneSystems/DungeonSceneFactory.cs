using LowLevelSystems.Common;
using LowLevelSystems.SceneSystems.Base;

namespace LowLevelSystems.SceneSystems.DungeonSceneSystems
{
public abstract class DungeonSceneFactory : Details
{
    public static DungeonScene GenerateDungeonScene(ScenePrefabEnum scenePrefabEnum)
    {
        int instanceId = SceneHub.GetNextInstanceId();
        DungeonScene dungeonScene = new DungeonScene(instanceId,scenePrefabEnum);

        SceneHub.RecordInstance(dungeonScene);

        return dungeonScene;
    }
}
}