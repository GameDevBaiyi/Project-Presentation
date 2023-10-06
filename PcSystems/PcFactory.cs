using System.Collections.Generic;

using LowLevelSystems.CharacterSystems.Components.CoordSystems;
using LowLevelSystems.CharacterSystems.Components.PropertySystems;
using LowLevelSystems.CharacterSystems.PcSystems.Components.BagOfEquippedSkillSystems;
using LowLevelSystems.CharacterSystems.PcSystems.Components.BagOfLearnedSkillSystems;
using LowLevelSystems.CharacterSystems.PcSystems.Components.EquipmentInventorySystems;
using LowLevelSystems.CharacterSystems.PcSystems.Components.InterestSystems;
using LowLevelSystems.CharacterSystems.PcSystems.Components.LuggageSystems;
using LowLevelSystems.CharacterSystems.PcSystems.Components.MissionActionsRecorderSystems;
using LowLevelSystems.CharacterSystems.PcSystems.Components.SkillBarAbstractDataSystems;
using LowLevelSystems.CharacterSystems.PcSystems.Components.TalentSystems;
using LowLevelSystems.Common;
using LowLevelSystems.SceneSystems.Base;

using UnityEngine;

// ReSharper disable ConditionIsAlwaysTrueOrFalse

namespace LowLevelSystems.CharacterSystems.PcSystems
{
public abstract class PcFactory : Details
{
    /// <summary>
    /// 生成的角色未加入队伍, 且都在休息空间中. 需要自行入队. 
    /// </summary>
    public static Pc GeneratePc(PcConfig pcConfig)
    {
        Pc pc = new Pc();

        //int _instanceId
        int instanceId = CharacterHub.GetNextInstanceId();
        pc.SetInstanceId(instanceId);

        //CharacterTypeEnum _characterTypeEnum
        Character.CharacterTypeEnum characterTypeEnum = Character.CharacterTypeEnum.Pc;
        pc.SetCharacterTypeEnum(characterTypeEnum);

        //CharacterEnum _characterEnum
        CharacterEnum characterEnum = pcConfig.CharacterEnumPy;
        pc.SetCharacterEnum(characterEnum);
        CharacterHub.RecordInstance(pc);

        //int _currentSceneId
        Scene currentScene = SceneHub.LoungeIdPy.ScenePy;
        pc.SetCurrentSceneId(currentScene.InstanceIdPy);

        //PropertySystem _propertySystem
        PropertySystem propertySystem = new PropertySystem(instanceId,characterTypeEnum,characterEnum);
        pc.SetPropertySystem(propertySystem);
        //恢复 Hp.
        propertySystem.HealAll(true);

        //CoordSystem _coordSystem
        CoordSystem coordSystem = CoordSystemFactory.GenerateCoordSystem(instanceId,Vector3Int.zero);
        pc.SetCoordSystem(coordSystem);

        //Dictionary<string,string> _skinData
        Dictionary<string,string> skinData = new Dictionary<string,string>(30);
        pc.SetSkinData(skinData);

        //bool _hasJoined 
        bool hasJoined = false;
        pc.SetHasJoined(hasJoined);

        //Dictionary<int,Vector3Int> _sceneId_entranceCoord
        Dictionary<int,Vector3Int> sceneId_entranceCoord = new Dictionary<int,Vector3Int>(30);
        pc.SetSceneId_EntranceCoord(sceneId_entranceCoord);

        //MissionActionsRecorder _missionActionsRecorder
        MissionActionsRecorder missionActionsRecorder = new MissionActionsRecorder(new CharacterId(instanceId));
        pc.SetMissionActionsRecorder(missionActionsRecorder);

        //InterestSystem _interestSystem
        InterestSystem interestSystem = InterestSystemFactory.GenerateInterestSystem(instanceId);
        pc.SetInterestSystem(interestSystem);

        //LearnedSkillBag _learnedSkillBag
        BagOfLearnedSkill bagOfLearnedSkill = new BagOfLearnedSkill(new CharacterId(instanceId));
        //初始时添加一些背包格子.
        bagOfLearnedSkill.AddCells(pcConfig.InitialCellCountOnLearnedSkillBagPy);
        pc.SetLearnedSkillBag(bagOfLearnedSkill);

        //EquippedSkillBag _equippedSkillBag
        BagOfEquippedSkill bagOfEquippedSkill = new BagOfEquippedSkill(new CharacterId(instanceId),pcConfig.MaxCellCountPerRowOnEquippedSkillBagPy.Count);
        pc.SetEquippedSkillBag(bagOfEquippedSkill);

        //SkillBarAbstractData _skillBarAbstractData
        SkillBarAbstractData skillBarAbstractData = new SkillBarAbstractData(instanceId,pcConfig.InitialPredicatedSlotCountPy);
        pc.SetSkillBarAbstractData(skillBarAbstractData);

        //Luggage _luggage
        Luggage luggage = LuggageFactory.GenerateLuggage(instanceId);
        pc.SetLuggage(luggage);

        //EquipmentInventory _equipmentInventory
        EquipmentInventory equipmentInventory = EquipmentInventoryFactory.GenerateEquipmentInventory(instanceId);
        pc.SetEquipmentInventory(equipmentInventory);

        //TalentSystem _talentSystem
        TalentSystem talentSystem = TalentSystemFactory.GenerateTalentSystem(pcConfig.TalentBookConfigsPy);
        pc.SetTalentSystem(talentSystem);

        //记录进对应的 Scene 中.
        SceneDetails.MoveCharacterTo(pc,currentScene);

        //此时可以恢复 兴致.
        DetailsOfInterestSystem.ChangeLimitedValue(interestSystem,interestSystem.MaxInterestValuePy,true);

        //初始解锁几行背包.
        for (int i = 0; i < pcConfig.InitialUnlockedRowCountOnEquippedSkillBagPy; i++)
        {
            bagOfEquippedSkill.UnlockSkillBagRow();
        }

        return pc;
    }
}
}