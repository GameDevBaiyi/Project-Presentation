using CityHudPackage;

using Cysharp.Threading.Tasks;

using HighLevelManagers.InputsAndEntityManagers;

using LowLevelSystems.CharacterSystems;
using LowLevelSystems.Common;
using LowLevelSystems.MissionSystems;
using LowLevelSystems.SceneSystems.BlankSpaceSystems;
using LowLevelSystems.SceneSystems.BuildingSystems;
using LowLevelSystems.SceneSystems.CitySystems.Base;
using LowLevelSystems.SceneSystems.RoomSystems;

using UnityEngine;

namespace LowLevelSystems.SceneSystems.Base
{
public abstract class SceneDetails : Details
{
    /// <summary>
    /// 生成了 SpaceAvoidingWar, City, Building, Room 的所有非 Character 相关的数据.
    /// </summary>
    public static void GenerateScenesForNewGame(NewGameConfig newGameGuideConfig)
    {
        //生成 SpaceAvoidingWar
        BlankSpaceFactory.GenerateSpaceAvoidingWar();

        //生成 TeammateRestingSpace
        BlankSpaceFactory.GenerateLounge();

        //生成 City
        foreach (CityConfig cityConfig in CommonDesignSO.SceneConfigHubPy.CityEnum_CityConfigPy.Values)
        {
            if (cityConfig.ScenePrefabEnumPy == ScenePrefabEnum.None) continue;
            CityFactory.GenerateCity(cityConfig);
        }
        //如果是第一个 City, 将其解锁.
        newGameGuideConfig.CityEnumPy.City().CityLockPy.UnlockAll();

        //根据新游戏的配置确定当前 City.
        CityEnum currentCityEnum = newGameGuideConfig.CityEnumPy;
        SceneHub.SetCurrentCityEnum(currentCityEnum);

        //根据新游戏的配置找到对应的初始场景.
        City newGameCity = currentCityEnum.City();
        Vector3Int buildingCoord = newGameGuideConfig.BuildingCoordPy;
        int currentSceneId;
        //如果配置的建筑坐标等于默认值, 代表当前 Scene 就是该 City.
        if (buildingCoord == Vector3Int.zero)
        {
            currentSceneId = newGameCity.InstanceIdPy;
        }
        else
        {
            Building building = newGameCity.BuildingHubPy.GetBuildingByBuildingCoord(buildingCoord);
            Room firstGameRoom = building.RoomSceneIdsPy[0].RoomPy;
            currentSceneId = firstGameRoom.InstanceIdPy;
        }

        SceneHub.SetCurrentSceneId(currentSceneId);
    }

    // BaiyiTODO. 该方法可以省略掉.
    public static async UniTask ChangeCurrentSceneAndRefreshEntitiesAndOpenCityHudAsync(int targetSceneId,bool isForcedToRefresh = false)
    {
        if (!isForcedToRefresh)
        {
            if (SceneHub.CurrentSceneIdPy.Id == targetSceneId) return;
        }
        //功能: 更新本身的数据.
        SceneHub.SetCurrentSceneId(targetSceneId);
        await EntityManagerDetails.RefreshAllEntitiesAsync();
        await CityHud.OpenCityHudWithAnimeAsync();
    }

    /// <summary>
    /// 将目标角色, 从该角色的 当前 Scene 搬运到指定的 Scene, 同时改变 Scene 和 Character 的数据. 
    /// </summary>
    public static void MoveCharacterTo(Character character,Scene targetScene)
    {
        //做一个判定, 因为该角色可能刚生成, 未在任何场景中.
        if (character.SceneIdPy.Id != 0)
        {
            character.SceneIdPy.ScenePy.RemoveCharacterId(character.CharacterIdPy);
        }

        // 将 character 记录进目标 Scene 中.
        targetScene.AddCharacterId(character.CharacterIdPy);

        // character 本身记录.
        character.SetCurrentSceneId(targetScene.InstanceIdPy);
    }

    /// <summary>
    /// 通过 哪个 City 的 哪个建筑 的哪一层 拿到对应的 SceneId.
    /// </summary>
    public static Scene GetSceneBy(CityEnum cityEnum,Vector3Int buildingCoord,int floorIndex)
    {
        City city = cityEnum.City();
        if (buildingCoord == Vector3Int.zero) return city;
        Building building = city.BuildingHubPy.GetBuildingByBuildingCoord(buildingCoord);
        Room room = building.RoomSceneIdsPy[floorIndex].RoomPy;
        return room;
    }
}
}