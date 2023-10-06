using LowLevelSystems.Common;
using LowLevelSystems.SceneSystems.BuildingSystems;
using LowLevelSystems.SceneSystems.CitySystems.Base;
using LowLevelSystems.SceneSystems.RoomSystems;

using UnityEngine;

namespace LowLevelSystems.CharacterSystems.NpcSystems
{
public abstract class NpcDetails : Details
{
    public static void GenerateNpcsForNewGame()
    {
        //先生成 City Prefab 中的 Npc. 
        void GenerateNpcsInCity(City city)
        {
            CityConfig cityConfig = city.CityEnumPy.CityConfig();
            foreach (InitialCharacterData initialCharacterData in cityConfig.InitialCharactersPy)
            {
                SpecialNpcConfig specialNpcConfig = new CharacterId(initialCharacterData.InstanceIdPy).SpecialNpcConfigPy;
                if (specialNpcConfig == null) continue;
                CharacterConfig characterConfig = specialNpcConfig.CharacterEnumPy.CharacterConfig();
                NpcFactory.GenerateNpc(characterConfig,city,initialCharacterData.CoordPy,initialCharacterData.CoordPy,specialNpcConfig.CharacterIdPy,
                                       null,initialCharacterData.IsStillPy);
            }
        }
        //再生成 Room Prefab 中的 Npc. 
        void GenerateNpcsInRoom(City city)
        {
            foreach (Building building in city.BuildingHubPy.InstanceId_InstancePy.Values)
            {
                int floorCount = building.BuildingInstanceConfigPy.RoomConfigsPy.Count;
                for (int i = 0; i < floorCount; i++)
                {
                    RoomInstanceConfig roomInstanceConfig = building.BuildingInstanceConfigPy.RoomConfigsPy[i];
                    Room room = building.RoomSceneIdsPy[i].RoomPy;
                    foreach (InitialCharacterData initialCharacterData in roomInstanceConfig.InitialCharactersPy)
                    {
                        SpecialNpcConfig specialNpcConfig = new CharacterId(initialCharacterData.InstanceIdPy).SpecialNpcConfigPy;
                        if (specialNpcConfig == null) continue;
                        CharacterConfig characterConfig = specialNpcConfig.CharacterEnumPy.CharacterConfig();
                        NpcFactory.GenerateNpc(characterConfig,room,initialCharacterData.CoordPy,initialCharacterData.CoordPy,specialNpcConfig.CharacterIdPy,
                                               null,initialCharacterData.IsStillPy);
                    }
                }
            }
        }

        foreach (City city in SceneHub.AllCitiesPy)
        {
            GenerateNpcsInCity(city);
            GenerateNpcsInRoom(city);
        }

        foreach (SpecialNpcConfig specialNpcConfig in CommonDesignSO.CharacterConfigHubPy.SpecialNpcConfigsPy)
        {
            if (CharacterHub.InstanceId_InstancePy.ContainsKey(specialNpcConfig.CharacterIdPy)) continue;
            CharacterConfig characterConfig = specialNpcConfig.CharacterEnumPy.CharacterConfig();
            NpcFactory.GenerateNpc(characterConfig,SceneHub.LoungeIdPy.ScenePy,Vector3Int.zero,Vector3Int.zero,specialNpcConfig.CharacterIdPy);
        }
    }
}
}