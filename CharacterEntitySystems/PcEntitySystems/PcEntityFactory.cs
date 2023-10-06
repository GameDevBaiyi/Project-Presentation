using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using LowLevelSystems.CharacterEntitySystems.Components;
using LowLevelSystems.CharacterEntitySystems.Components.CharacterAnimationSystems;
using LowLevelSystems.CharacterEntitySystems.Components.EntityMoverSystems;
using LowLevelSystems.CharacterEntitySystems.Components.HpUiSystems;
using LowLevelSystems.CharacterEntitySystems.PcEntitySystems.Components.PcFSMSystems;
using LowLevelSystems.CharacterEntitySystems.PcEntitySystems.Components.SkillBarSystems;
using LowLevelSystems.CharacterEntitySystems.PcEntitySystems.Components.SkillDrawPileSystems;
using LowLevelSystems.CharacterSystems;
using LowLevelSystems.CharacterSystems.PcSystems;
using LowLevelSystems.Common;
using LowLevelSystems.SkillSystems.SkillBuffSystems;

using UnityEngine;
using UnityEngine.AddressableAssets;

namespace LowLevelSystems.CharacterEntitySystems.PcEntitySystems
{
public abstract class PcEntityFactory : Details
{
    public static async UniTask<PcEntity> GeneratePcEntityAsync(Pc pc)
    {
        GameObject pcGo = await Addressables.InstantiateAsync(PcEntity.EntityAddress,Vector3.zero,Quaternion.identity,_hierarchyManager.PCEntitiesPy);
        if (!pcGo.TryGetComponent(out PcEntity pcEntity))
        {
            Debug.LogError($"从 {PcEntity.EntityAddress} 生成的 Go 不含有 {typeof(PcEntity)}");
            return pcEntity;
        }

        pcEntity.SetPc(pc);

        //int _instanceId
        int instanceId = pc.InstanceIdPy;
        pcEntity.SetInstanceId(instanceId);

        //CharacterTypeEnum _characterTypeEnum
        Character.CharacterTypeEnum characterTypeEnum = Character.CharacterTypeEnum.Pc;
        pcEntity.SetCharacterTypeEnum(characterTypeEnum);

        _characterEntityHub.RecordInstance(pcEntity);

        //CharacterAnimationSystem _characterAnimationSystem
        pcEntity.SetCharacterAnimationSystem(new CharacterAnimationSystem(pcEntity));

        //EntityMover _entityMover
        EntityMover entityMover = new EntityMover(pcEntity,SettingsSo.PcMoveSpeed,pcEntity.SelfTransformPy);
        pcEntity.SetEntityMover(entityMover);

        //CharacterPanelController _characterPanelController
        CharacterPanelController characterPanelController = new CharacterPanelController(pcEntity);
        pcEntity.SetCharacterPanelController(characterPanelController);
        //需要处理一部分 SpineController 的数据, 才能调整正确 Panel 的宽度.
        pcEntity.SpineControllerPy.Initialize();
        await pcEntity.SpineControllerPy.LoadSpineAsync(pc.CharacterEnumPy.ToString(),pc.SkinDataPy);
        characterPanelController.HideInteraction();
        characterPanelController.ChangeDirectionUIVisible(false);
        bool isInBattle = _battleManager.IsInBattlePy;
        if (isInBattle)
        {
            characterPanelController.RefreshDirection();
        }

        //HpShower _hpShower
        HpUiSystem hpUiSystem = new HpUiSystem(pcEntity);
        pcEntity.SetHpUiSystem(hpUiSystem);

        //BuffPool _buffPool
        BuffPool buffPool = new BuffPool(pcEntity);
        pcEntity.SetBuffPool(buffPool);

        //EfxHolder _efxHolder
        CharacterEfxHolder characterEfxHolder = new CharacterEfxHolder(pcEntity.ParticleHolderPy);
        pcEntity.SetEfxHolder(characterEfxHolder);

        //SpineController _spineController
        //序列化而来, 只做检测.
        if (pcEntity.SpineControllerPy == null)
        {
            Debug.LogError($"未 Serialize {pcEntity.SpineControllerPy.GetType()}");
            return pcEntity;
        }
        pcEntity.SpineControllerPy.SetCharacterEntity(pcEntity);
        pcEntity.CharacterAnimationSystemPy.DoIdleAnime();

        //UIPanel _uiPanel
        //序列化而来, 只做检测.
        if (pcEntity.UiPanelPy == null)
        {
            Debug.LogError($"未 Serialize {pcEntity.UiPanelPy.GetType()}");
            return pcEntity;
        }

        //PcFSM _pcFSM
        if (PcEntity.InputFSMPy == null)
        {
            InputFSM inputFSM = new InputFSM();
            pcEntity.SetPcFSM(inputFSM);
        }

        //SkillDrawPile _skillDrawPile
        SkillDrawPile skillDrawPile = new SkillDrawPile(pcEntity);
        pcEntity.SetSkillDrawPile(skillDrawPile);

        //SkillBar _skillBar
        SkillBar skillBar = new SkillBar(pcEntity);
        pcEntity.SetSkillBar(skillBar);

        pcEntity.SetHasInitialized();

        return pcEntity;
    }

    public static async UniTask DestroyPcEntityAsync(CharacterId pcId)
    {
        if (!_characterEntityHub.InstanceId_InstancePy.ContainsKey(pcId.InstanceId)) return;
        PcEntity pcEntity = pcId.PcEntityPy;

        List<UniTask> uniTasks = new List<UniTask>(10);
        uniTasks.Add(pcEntity.HideAsync());
        await UniTask.WhenAll(uniTasks);

        //从记录中移除.
        _characterEntityHub.RemoveInstance(pcId.InstanceId);

        //然后 Destroy.
        Object.Destroy(pcEntity.SelfGoPy);
    }
}
}