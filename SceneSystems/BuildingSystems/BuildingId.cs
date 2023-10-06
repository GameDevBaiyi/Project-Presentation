using System;

using LowLevelSystems.Common;
using LowLevelSystems.SceneSystems.CitySystems.Base;

using Sirenix.OdinInspector;

namespace LowLevelSystems.SceneSystems.BuildingSystems
{
[Serializable]
public struct BuildingId
{
    public int Id;
    public CityEnum CityEnum;

    public BuildingId(int id,CityEnum cityEnum)
    {
        this.Id = id;
        this.CityEnum = cityEnum;
    }

    [ShowInInspector]
    public Building BuildingPy => BuildingIdDetails.GetBuilding(this);
}

public abstract class BuildingIdDetails : Details
{
    public static Building GetBuilding(BuildingId buildingId)
    {
        buildingId.CityEnum.City().BuildingHubPy.TryGetInstance(buildingId.Id,out Building building);
        return building;
    }
}
}