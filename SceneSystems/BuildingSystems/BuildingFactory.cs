using System.Collections.Generic;

using LowLevelSystems.Common;
using LowLevelSystems.SceneSystems.Base;
using LowLevelSystems.SceneSystems.CitySystems.Base;
using LowLevelSystems.SceneSystems.RoomSystems;

namespace LowLevelSystems.SceneSystems.BuildingSystems
{
public abstract class BuildingFactory : Details
{
    /// <summary>
    /// Building 自身数据完成, 但其内的 Room 只实现了 RoomFactory 的部分.  
    /// </summary>
    public static Building GenerateBuilding(City city,(bool IsCommon,int IdInCommonList,int StateId,int StageId,int IdInStage) buildingInstanceConfigKey,
                                            BuildingInstanceConfig buildingInstanceConfig)
    {
        int instanceId = city.BuildingHubPy.GetNextInstanceId();
        CityEnum cityEnum = city.CityEnumPy;
        List<SceneId> roomSceneIds = new List<SceneId>();
        int floorIndex = 0;
        foreach (RoomInstanceConfig roomInstanceConfig in buildingInstanceConfig.RoomConfigsPy)
        {
            Room room = RoomFactory.GenerateRoom(roomInstanceConfig,new BuildingId(instanceId,cityEnum),floorIndex,buildingInstanceConfig.RoomConfigsPy.Count);
            roomSceneIds.Add(room.SceneIdPy);
            floorIndex++;
        }
        roomSceneIds.TrimExcess();
        Building building = new Building(instanceId,cityEnum,buildingInstanceConfigKey,roomSceneIds);

        city.BuildingHubPy.RecordInstance(building);

        return building;
    }
}
}