#if UNITY_EDITOR

using System.Collections.Generic;

using Common.Extensions;
using Common.Utilities;

using DevTools.ProgrammerTools;

using LitJson;

using LowLevelSystems.CharacterSystems.NpcSystems.Components.InteractionsSystems;
using LowLevelSystems.CharacterSystems.NpcSystems.Components.InteractionsSystems.RestSystems;
using LowLevelSystems.CharacterSystems.NpcSystems.Components.InteractionsSystems.TalkSystems;
using LowLevelSystems.CharacterSystems.NpcSystems.Components.InteractionsSystems.TradeSystems;
using LowLevelSystems.Common;
using LowLevelSystems.ItemSystems.Base;
using LowLevelSystems.LocalizationSystems;

using UnityEditor;

using UnityEngine;

namespace LowLevelSystems.CharacterSystems.NpcSystems
{
public static class SpecialNpcConfigBaker
{
    public const string TbSpecialNpc = "tbSpecialNpc";
    public static void BakeTbSpecialNpcToSpecialNpcConfigs()
    {
        //List<SpecialNpcConfig> _specialNpcConfigs
        List<SpecialNpcConfig> specialNpcConfigs = new List<SpecialNpcConfig>();
        JsonData tbSpecialNpcJson = JsonUtilities.GetJsonData(TbSpecialNpc);

        foreach (string characterIdString in tbSpecialNpcJson["NpcID"].Keys)
        {
            SpecialNpcConfig specialNpcConfig = new SpecialNpcConfig();
            JsonData specialNpcConfigJson = tbSpecialNpcJson["NpcID"][characterIdString];

            //int _characterId
            int characterId = int.Parse(characterIdString);
            specialNpcConfig.SetCharacterId(characterId);

            //CharacterEnum _characterEnum
            CharacterEnum characterEnum = default(CharacterEnum);
            if (tbSpecialNpcJson.TryGetNestedJson(out JsonData characterEnumJson,"NpcID",characterIdString,"RoleID"))
            {
                characterEnum = characterEnumJson.ToEnum<CharacterEnum>();
            }
            specialNpcConfig.SetCharacterEnum(characterEnum);
            
            //TextId _name
            if (tbSpecialNpcJson.TryGetNestedJson(out JsonData nameJson,"NpcID",characterIdString,"Name"))
            {
                TextId textId = new TextId(nameJson.ToInt());
                specialNpcConfig.SetCharacterName(textId);
            }

            //List<InteractionConfig> _extraInteractionConfigs
            List<InteractionConfig> interactionConfigs = new List<InteractionConfig>();
            if (specialNpcConfigJson.TryGetNestedJson(out JsonData _,"Interaction"))
            {
                JsonData interactionConfigsJson = specialNpcConfigJson["Interaction"];
                foreach (string interactionEnumString in interactionConfigsJson.Keys)
                {
                    switch (interactionEnumString)
                    {
                    case "Trade":
                    {
                        TradeConfig trade = new TradeConfig();
                        List<Item.ItemSubTypeEnum> itemTypeEnums = new List<Item.ItemSubTypeEnum>();
                        if (interactionConfigsJson.TryGetNestedJson(out JsonData itemTypeEnumsJson,interactionEnumString,"InteractionValue"))
                        {
                            List<int> itemTypeInt = itemTypeEnumsJson.ToList<int>();
                            foreach (int id in itemTypeInt)
                            {
                                itemTypeEnums.Add((Item.ItemSubTypeEnum)id);
                            }
                        }
                        itemTypeEnums.TrimExcess();
                        trade.SetItemTypeEnums(itemTypeEnums);
                        interactionConfigs.Add(trade);
                        break;
                    }

                    case "Talk":
                    {
                        TalkConfig talkConfig = new TalkConfig();
                        List<TextId> textIds = new List<TextId>();
                        if (interactionConfigsJson.TryGetNestedJson(out JsonData textIdJson,interactionEnumString,"InteractionValue"))
                        {
                            List<int> textsString = textIdJson.ToList<int>();
                            foreach (int text in textsString)
                            {
                                TextId textId = new TextId(text);
                                textIds.Add(textId);
                            }
                        }
                        textIds.TrimExcess();
                        talkConfig.SetTextIds(textIds);
                        interactionConfigs.Add(talkConfig);
                        break;
                    }

                    case "Rest":
                    {
                        RestConfig restConfig = new RestConfig();
                        interactionConfigs.Add(restConfig);
                        break;
                    }
                    
                    
                    }
                }
            }
            interactionConfigs.TrimExcess();
            specialNpcConfig.SetInteractionConfigs(interactionConfigs);

            specialNpcConfigs.Add(specialNpcConfig);
        }

        CommonDesignSO commonDesignSO = DevUtilities.GetCommonConfigSO();
        commonDesignSO.CharacterConfigHubPy.SetSpecialNpcConfigs(specialNpcConfigs);
        EditorUtility.SetDirty(commonDesignSO);
        AssetDatabase.SaveAssetIfDirty(commonDesignSO);

        Debug.Log("tbSpecialNpc 转化成功. ");
    }
}
}
#endif