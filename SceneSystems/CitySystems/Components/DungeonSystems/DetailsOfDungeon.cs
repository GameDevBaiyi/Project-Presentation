using LowLevelSystems.BattleSystems.Config;
using LowLevelSystems.Common;
using LowLevelSystems.MissionSystems;
using LowLevelSystems.SceneSystems.CitySystems.Base;

namespace LowLevelSystems.SceneSystems.CitySystems.Components.DungeonSystems
{
public abstract class DetailsOfDungeon : Details
{
    public static Dungeon CreateRandomDungeonForCurrentCity(MissionId correspondingMissionId,BattleConfig.BattleTypeEnum battleTypeEnum,int npcLvAddend)
    {
        CityEnum currentCityEnum = SceneHub.CurrentCityEnumPy;
        City currentCity = currentCityEnum.City();
        BattleConfigId randomBattleConfigId = currentCity.CityConfigPy.GetRandomBattleConfigId(currentCity.CityJurisdictionSystemPy.CurrentCampPy,battleTypeEnum);

        DungeonHub dungeonHub = currentCity.DungeonHubPy;
        return dungeonHub.CreateDungeon(randomBattleConfigId,correspondingMissionId,npcLvAddend);
    }
}
}