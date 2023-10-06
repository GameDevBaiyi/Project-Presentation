#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

using Common.Extensions;
using Common.Utilities;

using DevTools.ProgrammerTools;

using JetBrains.Annotations;

using LitJson;

using LowLevelSystems.CharacterEntitySystems.PcEntitySystems.Components.SkillBarSystems;
using LowLevelSystems.CharacterSystems.Components.PropertySystems;
using LowLevelSystems.CharacterSystems.NpcSystems.Components.InteractionsSystems;
using LowLevelSystems.CharacterSystems.NpcSystems.Components.InteractionsSystems.CurrencyExchangeSystems;
using LowLevelSystems.CharacterSystems.NpcSystems.Components.InteractionsSystems.RestSystems;
using LowLevelSystems.CharacterSystems.NpcSystems.Components.InteractionsSystems.TalkSystems;
using LowLevelSystems.CharacterSystems.NpcSystems.Components.InteractionsSystems.TradeSystems;
using LowLevelSystems.CharacterSystems.PcSystems;
using LowLevelSystems.CharacterSystems.PcSystems.Components.TalentSystems;
using LowLevelSystems.Common;
using LowLevelSystems.ItemSystems.Base;
using LowLevelSystems.LocalizationSystems;

using UnityEditor;

using UnityEngine;

namespace LowLevelSystems.CharacterSystems
{
public static class CharacterConfigBaker
{
    public const string TbRoleData = "tbRoleData";
    public const string TbRolePropertyData = "tbRolePropertyData";
    public const string TbRoleInteractionData = "tbRoleInteraction";

    public static void GenerateCharacterEnum()
    {
        string path = Path.Combine(Application.dataPath,"Scripts/Scripts/LowLevelSystems/CharacterSystems/CharacterEnum.cs");
        JsonData tbRolePropertyData = JsonUtilities.GetJsonData(CharacterConfigBaker.TbRolePropertyData);
        List<string> characterEnumStrings = tbRolePropertyData["RoleID"].Keys.ToList();
        CodeGenerateUtilities.GenerateEnumFile(path,$"{typeof(CharacterEnum).Namespace}",$"{nameof(CharacterEnum)}",characterEnumStrings,Enum.GetNames(typeof(CharacterEnum)));
        AssetDatabase.ImportAsset(@"Assets\Scripts\Scripts\LowLevelSystems\CharacterSystems\CharacterEnum.cs");
    }

    public static void BakePcAndNpcConfig()
    {
        BakeTbRoleDataAndTbRolePropertyDataToPcConfig();
        BakeTbRolePropertyDataAndTbRoleInteractionToNpcConfig();
    }

    //Npc.
    [CanBeNull]
    private static CharacterConfig RecordSingleCharacterConfigFromTbRolePropertyData(JsonData singleTbRolePropertyData,string roleId)
    {
        CharacterConfig characterConfig = new CharacterConfig();

        //CharacterEnum _characterEnum.
        CharacterEnum characterEnum = roleId.ToEnum<CharacterEnum>();
        characterConfig.SetCharacterEnum(characterEnum);

        //string _characterName.
        string characterName = "";
        if (singleTbRolePropertyData.TryGetNestedJson(out JsonData nameJson,"Name"))
        {
            characterName = nameJson.ToString();
        }
        characterConfig.SetCharacterName(characterName);

        //Vector2Int _headIconPosInBattle.
        Vector2Int headIconPosInBattle = default(Vector2Int);
        if (singleTbRolePropertyData.TryGetNestedJson(out JsonData battleIconJson,"BattleIcon"))
        {
            headIconPosInBattle = (Vector2Int)battleIconJson.ToString().SplitToCoord('_');
        }
        characterConfig.SetHeadIconPosInBattle(headIconPosInBattle);

        //Vector2Int _headIconPosInPreparation.
        Vector2Int headIconPosInPreparation = default(Vector2Int);
        if (singleTbRolePropertyData.TryGetNestedJson(out JsonData goBattleIconJson,"GoBattleIcon"))
        {
            headIconPosInPreparation = (Vector2Int)goBattleIconJson.ToString().SplitToCoord('_');
        }
        characterConfig.SetHeadIconPosInPreparation(headIconPosInPreparation);

        //List<PropertyEnumAndValue> _propertyEnumAndValueList
        List<CharacterConfig.PropertyEnumAndValue> propertyEnumAndValues = new List<CharacterConfig.PropertyEnumAndValue>();
        if (!singleTbRolePropertyData.Keys.Contains("Property"))
        {
            Debug.LogError($"该角色 {roleId} 没有 Property 属性");
            return null;
        }
        JsonData propertyJsonData = singleTbRolePropertyData["Property"]["1"];
        foreach (string property in propertyJsonData.Keys)
        {
            if (property == "FOV") continue;
            if (property != "ApRecoverySpeed"
             && property.ToEnum<PropertyEnum>() == PropertyEnum.None) continue;

            CharacterConfig.PropertyEnumAndValue propertyEnumAndValue = new CharacterConfig.PropertyEnumAndValue();
            PropertyEnum propertyEnum = property == "ApRecoverySpeed" ? PropertyEnum.TimesToRecoverToMaxAp : property.ToEnum<PropertyEnum>();
            propertyEnumAndValue.SetPropertyEnum(propertyEnum);
            float propertyValue = 0;
            if (propertyJsonData.TryGetNestedJson(out JsonData propertyValueJson,property))
            {
                propertyValue = property == "ApRecoverySpeed" ? propertyValueJson.ToInt() / 100f : propertyValueJson.ToInt();
            }
            propertyEnumAndValue.SetValue(propertyValue);
            propertyEnumAndValues.Add(propertyEnumAndValue);
        }
        characterConfig.SetPropertyEnumAndValueList(propertyEnumAndValues);

        //int _manualCardType
        int manualCardType = default(int);
        if (singleTbRolePropertyData.TryGetNestedJson(out JsonData manualCardTypeJson,"TypeLevel"))
        {
            manualCardType = manualCardTypeJson.ToInt();
        }
        characterConfig.SetManualCardType(manualCardType);

        return characterConfig;
    }
    private static Dictionary<CharacterEnum,List<InteractionConfig>> GetCharacterEnum_InteractionsFromTbRoleInteraction()
    {
        JsonData tbRoleInteractionJson = JsonUtilities.GetJsonData(TbRoleInteractionData);
        Dictionary<CharacterEnum,List<InteractionConfig>> characterEnum_interactions
            = new Dictionary<CharacterEnum,List<InteractionConfig>>(tbRoleInteractionJson["RoleID"].Keys.Count);
        foreach (string characterEnumString in tbRoleInteractionJson["RoleID"].Keys)
        {
            CharacterEnum characterEnum = characterEnumString.ToEnum<CharacterEnum>();
            List<InteractionConfig> interactionConfigs = new List<InteractionConfig>();
            if (characterEnum != CharacterEnum.None)
            {
                characterEnum_interactions[characterEnum] = interactionConfigs;
            }
            JsonData interactionConfigsJson = tbRoleInteractionJson["RoleID"][characterEnumString]["Interaction"];
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

                case "CurrencyExchange":
                {
                    CurrencyExchangeConfig currencyExchangeConfig = new CurrencyExchangeConfig();
                    interactionConfigs.Add(currencyExchangeConfig);
                    break;
                }
                }
            }
        }
        return characterEnum_interactions;
    }
    private static void BakeTbRolePropertyDataAndTbRoleInteractionToNpcConfig()
    {
        List<CharacterConfig> npcConfigs = new List<CharacterConfig>();

        JsonData tbRolePropertyData = JsonUtilities.GetJsonData(TbRolePropertyData);
        foreach (string characterEnumString in tbRolePropertyData["RoleID"].Keys)
        {
            CharacterConfig characterConfig = RecordSingleCharacterConfigFromTbRolePropertyData(tbRolePropertyData["RoleID"][characterEnumString],characterEnumString);
            if (characterConfig != null)
            {
                npcConfigs.Add(characterConfig);
            }
        }

        Dictionary<CharacterEnum,List<InteractionConfig>> characterEnum_interactions = GetCharacterEnum_InteractionsFromTbRoleInteraction();
        foreach (CharacterConfig characterConfig in npcConfigs)
        {
            if (!characterEnum_interactions.TryGetValue(characterConfig.CharacterEnumPy,out List<InteractionConfig> interactionConfigs)) continue;
            characterConfig.SetInteractionConfigs(interactionConfigs);
        }

        CommonDesignSO commonDesignSO = DevUtilities.GetCommonConfigSO();
        commonDesignSO.CharacterConfigHubPy.SetNpcConfigs(npcConfigs);
        EditorUtility.SetDirty(commonDesignSO);
        AssetDatabase.SaveAssetIfDirty(commonDesignSO);

        Debug.Log("tbRolePropertyData 数据转 Npc Config 成功.");
    }

    //Pc
    [SuppressMessage("ReSharper","StringLiteralTypo")]
    [CanBeNull]
    private static PcConfig RecordSinglePcConfig(string characterEnumString,JsonData singleTbRoleData,JsonData singleTbRolePropertyData)
    {
        //数据储存.
        PcConfig pcConfig = new PcConfig();

        CharacterConfig characterConfig = RecordSingleCharacterConfigFromTbRolePropertyData(singleTbRolePropertyData,characterEnumString);
        if (characterConfig == null) return null;

        List<CharacterConfig.PropertyEnumAndValue> propertyEnumAndValueList = characterConfig.PropertyEnumAndValueListPy.ToList();

        //int _initialHealth.
        if (singleTbRoleData.TryGetNestedJson(out JsonData healthJson,"Health"))
        {
            int initialHealth = healthJson.ToInt();
            propertyEnumAndValueList.Add(new CharacterConfig.PropertyEnumAndValue(PropertyEnum.Health,initialHealth));
        }

        //int _initialBalance.
        if (singleTbRoleData.TryGetNestedJson(out JsonData balanceJson,"Balance"))
        {
            int initialBalance = balanceJson.ToInt();
            propertyEnumAndValueList.Add(new CharacterConfig.PropertyEnumAndValue(PropertyEnum.Balance,initialBalance));
        }

        //int _initialStrength.
        if (singleTbRoleData.TryGetNestedJson(out JsonData strengthJson,"Strength"))
        {
            int initialStrength = strengthJson.ToInt();
            propertyEnumAndValueList.Add(new CharacterConfig.PropertyEnumAndValue(PropertyEnum.Strength,initialStrength));
        }

        //int _initialAgility.
        if (singleTbRoleData.TryGetNestedJson(out JsonData agilityJson,"Agility"))
        {
            int initialAgility = agilityJson.ToInt();
            propertyEnumAndValueList.Add(new CharacterConfig.PropertyEnumAndValue(PropertyEnum.Agility,initialAgility));
        }

        //int _initialDexterity.
        if (singleTbRoleData.TryGetNestedJson(out JsonData dexterityJson,"Dexterity"))
        {
            int initialDexterity = dexterityJson.ToInt();
            propertyEnumAndValueList.Add(new CharacterConfig.PropertyEnumAndValue(PropertyEnum.Dexterity,initialDexterity));
        }

        //int _initialPrecision.
        if (singleTbRoleData.TryGetNestedJson(out JsonData precisionJson,"Precision"))
        {
            int initialPrecision = precisionJson.ToInt();
            propertyEnumAndValueList.Add(new CharacterConfig.PropertyEnumAndValue(PropertyEnum.Precision,initialPrecision));
        }

        //int _initialToughness.
        if (singleTbRoleData.TryGetNestedJson(out JsonData toughnessJson,"Toughness"))
        {
            int initialToughness = toughnessJson.ToInt();
            propertyEnumAndValueList.Add(new CharacterConfig.PropertyEnumAndValue(PropertyEnum.Toughness,initialToughness));
        }
        pcConfig.SetPropertyEnumAndValueList(propertyEnumAndValueList);

        //int _initialMaxInterest.
        int initialMaxInterest = 0;

        if (singleTbRoleData.TryGetNestedJson(out JsonData maxInterestJson,"Interest"))
        {
            initialMaxInterest = maxInterestJson.ToInt();
        }

        //int _initialCellCountOnLearnedSkillBag.
        int initialCellCountOnLearnedSkillBag = 0;

        if (singleTbRoleData.TryGetNestedJson(out JsonData initialCellCountOnLearnedSkillBagJson,"MaxSugarNum"))
        {
            initialCellCountOnLearnedSkillBag = initialCellCountOnLearnedSkillBagJson.ToInt();
        }

        //List<int> _maxCellCountPerRowOnEquippedSkillBag.
        List<int> maxCellCountPerRowOnEquippedSkillBag = new List<int>();

        foreach (string rowIndexPlus1 in singleTbRoleData["VeinSkeleton"].Keys)
        {
            maxCellCountPerRowOnEquippedSkillBag.Add(singleTbRoleData["VeinSkeleton"][rowIndexPlus1]["TotalCount"].ToInt());
        }
        //List<int> _initialUnlockedCellCountPerRowOnEquippedSkillBag.
        List<int> initialUnlockedCellCountPerRowOnEquippedSkillBag = new List<int>();

        foreach (string rowIndexPlus1 in singleTbRoleData["VeinSkeleton"].Keys)
        {
            initialUnlockedCellCountPerRowOnEquippedSkillBag.Add(singleTbRoleData["VeinSkeleton"][rowIndexPlus1]["UnLockedNum"].ToInt());
        }
        //int _initialUnlockedRowCountOnEquippedSkillBag.
        int initialUnlockedRowCountOnEquippedSkillBag = 0;

        foreach (string rowIndexPlus1 in singleTbRoleData["VeinSkeleton"].Keys)
        {
            if (singleTbRoleData.TryGetNestedJson(out JsonData unLockedJsonData,"VeinSkeleton",rowIndexPlus1,"UnLocked")
             && unLockedJsonData.ToInt() == 1)
            {
                initialUnlockedRowCountOnEquippedSkillBag++;
            }
            else
            {
                break;
            }
        }

        //抽技能堆的配置. _initialPredicatedSlotCount
        int initialPredicatedSlotCount = singleTbRoleData["PreSlotNum"].ToInt();

        //int _initialCountOfLuggageSlots
        int initialCountOfLuggageSlots = 0;

        if (singleTbRoleData.TryGetNestedJson(out JsonData initialCountOfLuggageSlotsJsonData,"RoleBag"))
        {
            initialCountOfLuggageSlots = initialCountOfLuggageSlotsJsonData.ToInt();
        }

        //List<ModuleOfSkillBarEnumsWrapper> _moduleOfSkillBarConfig
        List<PcConfig.ModuleOfSkillBarEnumsWrapper> moduleOfSkillBarConfig = new List<PcConfig.ModuleOfSkillBarEnumsWrapper>();

        if (singleTbRoleData.TryGetNestedJson(out JsonData moduleOfSkillBarConfigJsonData1,"VeinSlot1"))
        {
            PcConfig.ModuleOfSkillBarEnumsWrapper moduleOfSkillBarEnumsWrapper = new PcConfig.ModuleOfSkillBarEnumsWrapper();
            List<ModuleOfSkillBarEnum> moduleOfSkillBarEnums = new List<ModuleOfSkillBarEnum>();
            List<string> moduleOfSkillBar = moduleOfSkillBarConfigJsonData1.ToList<string>();

            foreach (string moduleOfString in moduleOfSkillBar)
            {
                ModuleOfSkillBarEnum moduleOfSkillBarEnum = moduleOfString.ToEnum<ModuleOfSkillBarEnum>();
                moduleOfSkillBarEnums.Add(moduleOfSkillBarEnum);
                moduleOfSkillBarEnumsWrapper.SetModuleOfSkillBarEnums(moduleOfSkillBarEnums);
            }
            moduleOfSkillBarConfig.Add(moduleOfSkillBarEnumsWrapper);
        }

        if (singleTbRoleData.TryGetNestedJson(out JsonData moduleOfSkillBarConfigJsonData2,"VeinSlot2"))
        {
            PcConfig.ModuleOfSkillBarEnumsWrapper moduleOfSkillBarEnumsWrapper = new PcConfig.ModuleOfSkillBarEnumsWrapper();
            List<ModuleOfSkillBarEnum> moduleOfSkillBarEnums = new List<ModuleOfSkillBarEnum>();
            List<string> moduleOfSkillBar = moduleOfSkillBarConfigJsonData2.ToList<string>();

            foreach (string moduleOfString in moduleOfSkillBar)
            {
                ModuleOfSkillBarEnum moduleOfSkillBarEnum = moduleOfString.ToEnum<ModuleOfSkillBarEnum>();
                moduleOfSkillBarEnums.Add(moduleOfSkillBarEnum);
                moduleOfSkillBarEnumsWrapper.SetModuleOfSkillBarEnums(moduleOfSkillBarEnums);
            }
            moduleOfSkillBarConfig.Add(moduleOfSkillBarEnumsWrapper);
        }

        if (singleTbRoleData.TryGetNestedJson(out JsonData moduleOfSkillBarConfigJsonData3,"VeinSlot3"))
        {
            PcConfig.ModuleOfSkillBarEnumsWrapper moduleOfSkillBarEnumsWrapper = new PcConfig.ModuleOfSkillBarEnumsWrapper();
            List<ModuleOfSkillBarEnum> moduleOfSkillBarEnums = new List<ModuleOfSkillBarEnum>();
            List<string> moduleOfSkillBar = moduleOfSkillBarConfigJsonData3.ToList<string>();

            foreach (string moduleOfString in moduleOfSkillBar)
            {
                ModuleOfSkillBarEnum moduleOfSkillBarEnum = moduleOfString.ToEnum<ModuleOfSkillBarEnum>();
                moduleOfSkillBarEnums.Add(moduleOfSkillBarEnum);
                moduleOfSkillBarEnumsWrapper.SetModuleOfSkillBarEnums(moduleOfSkillBarEnums);
            }
            moduleOfSkillBarConfig.Add(moduleOfSkillBarEnumsWrapper);
        }

        //角色性格属性
        for (int i = 1; i < 6; i++)
        {
            if (singleTbRoleData.TryGetNestedJson(out JsonData _,$"P{i}"))
            {
                int initialToughness = 5;
                PropertyEnum propertyEnum = (PropertyEnum)400 + i;
                propertyEnumAndValueList.Add(new CharacterConfig.PropertyEnumAndValue(propertyEnum,initialToughness));
            }
        }

        //记录完了所有的数据, 现在填进 playerCharacterConfigRow 中.
        pcConfig.SetCharacterEnum(characterConfig.CharacterEnumPy);
        pcConfig.SetCharacterName(characterConfig.CharacterNamePy);
        pcConfig.SetHeadIconPosInBattle(characterConfig.HeadIconPosInBattlePy);
        pcConfig.SetHeadIconPosInPreparation(characterConfig.HeadIconPosInPreparationPy);
        pcConfig.SetInitialMaxInterest(initialMaxInterest);
        pcConfig.SetInitialCellCountOnLearnedSkillBag(initialCellCountOnLearnedSkillBag);
        pcConfig.SetMaxCellCountPerRowOnEquippedSkillBag(maxCellCountPerRowOnEquippedSkillBag);
        pcConfig.SetInitialUnlockedCellCountPerRowOnEquippedSkillBag(initialUnlockedCellCountPerRowOnEquippedSkillBag);
        pcConfig.SetInitialUnlockedRowCountOnEquippedSkillBag(initialUnlockedRowCountOnEquippedSkillBag);
        pcConfig.SetInitialPredicatedSlotCount(initialPredicatedSlotCount);
        pcConfig.SetInitialCountOfLuggageSlots(initialCountOfLuggageSlots);
        pcConfig.SetModuleOfSkillBarConfig(moduleOfSkillBarConfig);
        pcConfig.SetTalentBookConfigs(TalentConfigBaker.BakeTbTalentTreeToPcConfigTalentBookConfig(characterEnumString));

        return pcConfig;
    }
    private static void BakeTbRoleDataAndTbRolePropertyDataToPcConfig()
    {
        List<PcConfig> playerCharacterConfigRows = new List<PcConfig>();

        JsonData tbRoleData = JsonUtilities.GetJsonData(TbRoleData);
        JsonData tbRolePropertyData = JsonUtilities.GetJsonData(TbRolePropertyData);

        foreach (string characterEnumString in tbRoleData["RoleID"].Keys)
        {
            PcConfig pcConfig = RecordSinglePcConfig(characterEnumString,tbRoleData["RoleID"][characterEnumString],tbRolePropertyData["RoleID"][characterEnumString]);
            if (pcConfig != null)
            {
                playerCharacterConfigRows.Add(pcConfig);
            }
        }

        CommonDesignSO commonDesignSO = DevUtilities.GetCommonConfigSO();
        commonDesignSO.CharacterConfigHubPy.SetPlayerCharacterConfigRows(playerCharacterConfigRows);
        EditorUtility.SetDirty(commonDesignSO);
        AssetDatabase.SaveAssetIfDirty(commonDesignSO);

        Debug.Log($"tbRoleData tbRolePropertyData 数据转 {nameof(PcConfig)} 成功.");
    }
}
}
#endif