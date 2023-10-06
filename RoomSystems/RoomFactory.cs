using LowLevelSystems.Common;
using LowLevelSystems.SceneSystems.Base;
using LowLevelSystems.SceneSystems.BuildingSystems;

namespace LowLevelSystems.SceneSystems.RoomSystems
{
public abstract class RoomFactory : Details
{
    /// <summary>
    /// 未确定角色和场景连接点. 
    /// </summary>
    public static Room GenerateRoom(RoomInstanceConfig roomInstanceConfig,BuildingId buildingIdParam,int floorIndexParam,
                                    int floorCount)
    {
        int instanceId = SceneHub.GetNextInstanceId();
        ScenePrefabEnum scenePrefabEnum = roomInstanceConfig.ScenePrefabEnumPy;
        BuildingId buildingId = buildingIdParam;
        int floorIndex = floorIndexParam;
        bool isFinalRoom = floorIndexParam >= floorCount - 1;
        Room room = new Room(instanceId,scenePrefabEnum,buildingId,floorIndex,isFinalRoom);

        SceneHub.RecordInstance(room);

        return room;
    }
}
}