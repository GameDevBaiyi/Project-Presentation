using System.Collections.Generic;
using System.Linq;

using LowLevelSystems.BattleSystems.Config;
using LowLevelSystems.CharacterSystems.Components.CoordSystems;
using LowLevelSystems.CharacterSystems.Components.PropertySystems;
using LowLevelSystems.CharacterSystems.NpcSystems.Components.InteractionsSystems;
using LowLevelSystems.CharacterSystems.NpcSystems.Components.InteractionsSystems.CurrencyExchangeSystems;
using LowLevelSystems.CharacterSystems.NpcSystems.Components.InteractionsSystems.RestSystems;
using LowLevelSystems.CharacterSystems.NpcSystems.Components.InteractionsSystems.StealSystems;
using LowLevelSystems.CharacterSystems.NpcSystems.Components.InteractionsSystems.TalkSystems;
using LowLevelSystems.CharacterSystems.NpcSystems.Components.InteractionsSystems.TradeSystems;
using LowLevelSystems.Common;
using LowLevelSystems.SceneSystems.Base;

using UnityEngine;

namespace LowLevelSystems.CharacterSystems.NpcSystems
{
public abstract class NpcFactory : Details
{
    /// <summary>
    /// 恢复 Npc 所有数据, 包括 CurrentHp 等计算需求的. 同时还会记录到对应的 Scene 中. 
    /// </summary>
    public static Npc GenerateNpc(CharacterConfig characterConfig,Scene scene,Vector3Int currentCoordParam,
                                  Vector3Int respawnCoordInCityParam,int instanceIdParam = 0,BattleConfig.CampRelations campRelations = null,
                                  bool isStillOnLiving = false,int initialLv = 0)
    {
        Npc npc = new Npc();

        //int _instanceId
        int instanceId = instanceIdParam == 0 ? CharacterHub.GetNextInstanceId() : instanceIdParam;
        npc.SetInstanceId(instanceId);

        //CharacterTypeEnum _characterTypeEnum
        Character.CharacterTypeEnum characterTypeEnum = Character.CharacterTypeEnum.Npc;
        npc.SetCharacterTypeEnum(characterTypeEnum);

        //CharacterEnum _characterEnum
        CharacterEnum characterEnum = characterConfig.CharacterEnumPy;
        npc.SetCharacterEnum(characterEnum);

        CharacterHub.RecordInstance(npc);

        npc.SetCampRelations(campRelations);

        //LvSystem _lvSystem
        npc.LvSystemPy.SetLv(initialLv);

        //PropertySystem _propertySystem
        PropertySystem propertySystem = new PropertySystem(instanceId,characterTypeEnum,characterEnum);
        npc.SetPropertySystem(propertySystem);
        //恢复 Hp.
        propertySystem.HealAll(true);

        //int _currentSceneId
        npc.SetCurrentSceneId(scene.InstanceIdPy);
        //记录到 Scene 中.
        SceneDetails.MoveCharacterTo(npc,scene);

        //CoordSystem _coordSystem
        CoordSystem coordSystem = CoordSystemFactory.GenerateCoordSystem(instanceId,currentCoordParam);
        npc.SetCoordSystem(coordSystem);

        //Dictionary<string,string> _skinData
        Dictionary<string,string> skinData = new Dictionary<string,string>(30);
        npc.SetSkinData(skinData);

        npc.SetIsStillOnLiving(isStillOnLiving);

        //Vector3Int _respawnCoordInCity
        Vector3Int respawnCoordInCity = respawnCoordInCityParam;
        npc.SetRespawnCoordInCity(respawnCoordInCity);

        CharacterId npcCharacterId = npc.CharacterIdPy;
        Interactions interactions = new Interactions(npcCharacterId);
        List<InteractionConfig> interactionConfigsFromNpcConfig = characterConfig.InteractionConfigsPy;
        List<InteractionConfig> interactionConfigsFromSpecialNpcConfig = npcCharacterId.SpecialNpcConfigWithoutErrorPy?.ExtraInteractionConfigsPy;
        foreach (InteractionEnum interactionEnum in Interaction.InteractionEnums)
        {
            InteractionConfig interactionConfig = interactionConfigsFromSpecialNpcConfig?.Find(t => t.InteractionEnumPy == interactionEnum)
                                               ?? interactionConfigsFromNpcConfig.Find(t => t.InteractionEnumPy == interactionEnum);
            if (interactionConfig == null) continue;
            switch (interactionEnum)
            {
            case InteractionEnum.Talk:
                TalkConfig talkConfig = (TalkConfig)interactionConfig;
                Talk talk = new Talk(npcCharacterId,talkConfig.TextIdsPy);
                interactions.InteractionEnum_InteractionPy[InteractionEnum.Talk] = talk;
                break;

            case InteractionEnum.Trade:
                TradeConfig tradeConfig = (TradeConfig)interactionConfig;
                Trade trade = new Trade(npcCharacterId,tradeConfig.ItemTypeEnumsPy.ToList());
                interactions.InteractionEnum_InteractionPy[InteractionEnum.Trade] = trade;
                break;

            case InteractionEnum.Rest:
                Rest rest = new Rest(npcCharacterId);
                interactions.InteractionEnum_InteractionPy[InteractionEnum.Rest] = rest;
                break;

            case InteractionEnum.CurrencyExchange:
                CurrencyExchange currencyExchange = new CurrencyExchange(npcCharacterId);
                interactions.InteractionEnum_InteractionPy[InteractionEnum.CurrencyExchange] = currencyExchange;
                break;
            }
        }
        Steal steal = new Steal(npcCharacterId);
        interactions.InteractionEnum_InteractionPy[InteractionEnum.Steal] = steal;
        npc.SetExtraInteractions(interactions);

        return npc;
    }
}
}