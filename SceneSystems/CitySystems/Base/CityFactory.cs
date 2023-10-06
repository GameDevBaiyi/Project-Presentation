using System.Collections.Generic;

using LowLevelSystems.Common;
using LowLevelSystems.SceneSystems.Base;
using LowLevelSystems.SceneSystems.BuildingSystems;
using LowLevelSystems.SceneSystems.CitySystems.Components;

namespace LowLevelSystems.SceneSystems.CitySystems.Base
{
public abstract class CityFactory : Details
{
    public static City GenerateCity(CityConfig cityConfig)
    {
        int instanceId = SceneHub.GetNextInstanceId();
        ScenePrefabEnum scenePrefabEnum = cityConfig.ScenePrefabEnumPy;
        CityEnum cityEnum = cityConfig.CityEnumPy;
        CityJurisdictionSystem cityJurisdictionSystem = new CityJurisdictionSystem(cityEnum,cityConfig.InitialCampEnumPy);
        List<int> commonBuildingIds = new List<int>(cityConfig.CommonBuildingInstanceConfigsPy.Count);
        List<BuildingHub.BuildingState> buildingStates = new List<BuildingHub.BuildingState>(cityConfig.BuildingStateConfigsPy.Count);
        foreach (CityConfig.BuildingStateConfig buildingStateConfig in cityConfig.BuildingStateConfigsPy)
        {
            if (buildingStateConfig.ListPy == null)
            {
                buildingStates.Add(new BuildingHub.BuildingState(null));
            }
            else
            {
                List<BuildingHub.BuildingStage> buildingStages = new List<BuildingHub.BuildingStage>(buildingStateConfig.ListPy.Count);
                foreach (CityConfig.BuildingStageConfig buildingStageConfig in buildingStateConfig.ListPy)
                {
                    List<int> buildingIds = new List<int>(buildingStageConfig.ListPy.Count);
                    buildingStages.Add(new BuildingHub.BuildingStage(buildingIds));
                }

                buildingStates.Add(new BuildingHub.BuildingState(buildingStages));
            }
        }
        List<int> currentStagePointers = new List<int>(cityConfig.BuildingStateConfigsPy.Count);
        for (int i = 0; i < cityConfig.BuildingStateConfigsPy.Count; i++)
        {
            currentStagePointers.Add(0);
        }
        currentStagePointers[0] = (int)cityJurisdictionSystem.CurrentCampPy - 1;
        BuildingHub buildingHub = new BuildingHub(cityEnum,commonBuildingIds,buildingStates,currentStagePointers);
        CityExploreSystem cityExploreSystem = new CityExploreSystem();
        CityLock cityLock = new CityLock(cityEnum,cityConfig.BattleListToUnlockLv2Py);

        City city = new City(instanceId,scenePrefabEnum,cityEnum,buildingHub,cityExploreSystem,
                             cityJurisdictionSystem,cityLock);
        SceneHub.RecordInstance(city);

        int idInList = 0;
        foreach (BuildingInstanceConfig buildingInstanceConfig in city.CityConfigPy.CommonBuildingInstanceConfigsPy)
        {
            Building building = BuildingFactory.GenerateBuilding(city,new(true,idInList,0,0,0),buildingInstanceConfig);
            commonBuildingIds.Add(building.InstanceIdPy);
            idInList++;
        }
        int stateId = 0;
        foreach (CityConfig.BuildingStateConfig buildingStateConfig in cityConfig.BuildingStateConfigsPy)
        {
            if (buildingStateConfig.ListPy == null)
            {
                stateId++;
                continue;
            }

            int stageId = 0;
            foreach (CityConfig.BuildingStageConfig buildingStageConfig in buildingStateConfig.ListPy)
            {
                idInList = 0;
                foreach (BuildingInstanceConfig buildingInstanceConfig in buildingStageConfig.ListPy)
                {
                    Building building = BuildingFactory.GenerateBuilding(city,new(false,0,stateId,stageId,idInList),buildingInstanceConfig);
                    buildingStates[stateId].BuildingStagesPy[stageId].BuildingIdsPy.Add(building.InstanceIdPy);
                    idInList++;
                }
                stageId++;
            }
            stateId++;
        }
        buildingHub.UpdateBuildingCoords();

        DetailsOfCity.ConnectScenesInCity(city);

        return city;
    }
}
}