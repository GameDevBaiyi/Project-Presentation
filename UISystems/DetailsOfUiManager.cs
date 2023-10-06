using System.Collections.Generic;
using System.Linq;

using LowLevelSystems.BattleSystems.Conditions;
using LowLevelSystems.Common;
using LowLevelSystems.SceneSystems.Base;
using LowLevelSystems.SceneSystems.CitySystems.Base;
using LowLevelSystems.SceneSystems.RoomSystems;

using UnityEngine;

namespace LowLevelSystems.UISystems
{
public abstract class DetailsOfUiManager : Details
{
    public static void ShowSceneUis()
    {
        //显示 Scene Uis.
        Scene currentScene = SceneHub.CurrentSceneIdPy.ScenePy;
        if (currentScene is City city)
        {
            // 当前所有建筑的交互格.
            UiManager.InteractionTileUiShowerPy.Show(city.BuildingHubPy.AllCurrentBuildingsPy.SelectMany(building => building.BuildingInstanceConfigPy.InteractionsPy));
            // EditorTileEnum.SpawnPoint.
            UiManager.SpawnTileUiShowerPy.Show(city.ScenePrefabEnumPy.ScenePrefabConfig().EditorTileEnum_CoordsPy[ScenePrefabConfig.EditorTileEnum.SpawnPoint]);
        }
        else if (currentScene is Room room)
        {
            // EditorTileEnum.SpawnPoint 和 EditorTileEnum.Upstairs.
            Dictionary<ScenePrefabConfig.EditorTileEnum,List<Vector3Int>> editorTileEnum_coords = room.ScenePrefabEnumPy.ScenePrefabConfig().EditorTileEnum_CoordsPy;
            UiManager.SpawnTileUiShowerPy.Show(editorTileEnum_coords[ScenePrefabConfig.EditorTileEnum.SpawnPoint]
                                                  .Concat(editorTileEnum_coords[ScenePrefabConfig.EditorTileEnum.Upstairs]));
        }

        // 如果是战斗中, 检测当前条件中, 所有的 指定区域, 并将其 Ui 显示出来.
        if (_battleManager.IsInBattlePy)
        {
            IEnumerable<int> areaIndexesOfVictoryConditions = _battleManager.VictoryConditionsPy.SelectMany(t => t.ConditionsPy).OfType<IHasAreaIndex>().Select(t => t.AreaIndexPy);
            IEnumerable<int> areaIndexesOfDefeatConditions = _battleManager.FailureConditionsPy.SelectMany(t => t.ConditionsPy).OfType<IHasAreaIndex>().Select(t => t.AreaIndexPy);
            IEnumerable<int> areaIndexesOfExtraConditions = _battleManager.ExtraChallengesPy.SelectMany(t => t.ConditionsPy).OfType<IHasAreaIndex>().Select(t => t.AreaIndexPy);
            IEnumerable<int> areaIndexes = areaIndexesOfVictoryConditions.Concat(areaIndexesOfDefeatConditions).Concat(areaIndexesOfExtraConditions).Distinct();

            UiManager.AreaTileUiShowerPy.Show(areaIndexes.SelectMany(t => _battleManager.SpecialCoordsPy.AreaCoordsGroupsPy[t].CoordsPy));
        }
        else
        {
            UiManager.AreaTileUiShowerPy.Hide();
        }
    }
}
}