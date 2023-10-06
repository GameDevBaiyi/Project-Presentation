using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using LowLevelSystems.CharacterEntitySystems.Components;
using LowLevelSystems.CharacterEntitySystems.Components.CharacterAnimationSystems;
using LowLevelSystems.CharacterEntitySystems.Components.EntityMoverSystems;
using LowLevelSystems.CharacterEntitySystems.Components.HpUiSystems;
using LowLevelSystems.CharacterEntitySystems.NpcEntitySystems.Components;
using LowLevelSystems.CharacterEntitySystems.NpcEntitySystems.Components.NpcAIForLivingSystems;
using LowLevelSystems.CharacterSystems;
using LowLevelSystems.CharacterSystems.NpcSystems;
using LowLevelSystems.Common;
using LowLevelSystems.SkillSystems.SkillBuffSystems;

using UnityEngine;
using UnityEngine.AddressableAssets;

namespace LowLevelSystems.CharacterEntitySystems.NpcEntitySystems
{
public abstract class NpcEntityFactory : Details
{
    public static async UniTask<NpcEntity> GenerateNpcEntityAsync(Npc npc)
    {
        GameObject npcGo = await Addressables.InstantiateAsync(NpcEntity.EntityAddress,Vector3.zero,Quaternion.identity,_hierarchyManager.NpcEntitiesPy);
        //Debug.
        if (!npcGo.TryGetComponent(out NpcEntity npcEntity))
        {
            Debug.LogError($"从 {NpcEntity.EntityAddress} 生成的 Go 不含有 {typeof(NpcEntity)}");
            return npcEntity;
        }

        npcEntity.SetNpc(npc);
        npcEntity.SetInstanceId(npc.InstanceIdPy);
        npcEntity.SetCharacterTypeEnum(Character.CharacterTypeEnum.Npc);
        _characterEntityHub.RecordInstance(npcEntity);
        npcEntity.SetCharacterAnimationSystem(new CharacterAnimationSystem(npcEntity));
        bool isInBattle = _battleManager.IsInBattlePy;
        EntityMover entityMover = new EntityMover(npcEntity,isInBattle ? NpcEntity.RunSpeed : NpcEntity.WalkSpeed,npcEntity.SelfTransformPy);
        npcEntity.SetEntityMover(entityMover);

        //CharacterPanelController _characterPanelController
        CharacterPanelController characterPanelController = new CharacterPanelController(npcEntity);
        npcEntity.SetCharacterPanelController(characterPanelController);
        npcEntity.SpineControllerPy.Initialize();
        await npcEntity.SpineControllerPy.LoadSpineAsync(npc.CharacterEnumPy.ToString(),npc.SkinDataPy);
        //不要 await 到主线程.
        characterPanelController.SelfAdaptionAsync();
        characterPanelController.HideInteraction();
        characterPanelController.ChangeDirectionUIVisible(false);
        npcEntity.ChangeDirection(npc.CoordSystemPy.DirectionIndexPy);

        //HpShower _hpShower
        HpUiSystem hpUiSystem = new HpUiSystem(npcEntity);
        npcEntity.SetHpUiSystem(hpUiSystem);
        hpUiSystem.ChangeHpVisible(isInBattle);

        //BuffShower
        characterPanelController.ChangeBuffsUIVisible(isInBattle);

        //BuffPool _buffPool
        BuffPool buffPool = new BuffPool(npcEntity);
        npcEntity.SetBuffPool(buffPool);

        //EfxHolder _efxHolder
        CharacterEfxHolder characterEfxHolder = new CharacterEfxHolder(npcEntity.ParticleHolderPy);
        npcEntity.SetEfxHolder(characterEfxHolder);

        //SpineController _spineController
        //序列化而来, 只做检测.
        if (npcEntity.SpineControllerPy == null)
        {
            Debug.LogError($"未 Serialize {npcEntity.SpineControllerPy.GetType()}");
            return npcEntity;
        }
        npcEntity.SpineControllerPy.SetCharacterEntity(npcEntity);
        npcEntity.CharacterAnimationSystemPy.DoIdleAnime();

        //UIPanel _uiPanel
        //序列化而来, 只做检测.
        if (npcEntity.UiPanelPy == null)
        {
            Debug.LogError($"未 Serialize {npcEntity.UiPanelPy.GetType()}");
            return npcEntity;
        }

        //NpcAIForLiving _npcAIForLiving
        NpcAIForLiving npcAIForLiving = new NpcAIForLiving(npcEntity);
        npcEntity.SetNpcAIForLiving(npcAIForLiving);

        // _btForBattle
        BtForBattle btForBattle = new BtForBattle(npcEntity);
        npcEntity.SetBtForBattle(btForBattle);

        //GameObject _entity
        //序列化而来, 只做检测.
        if (npcEntity.SelfGoPy == null)
        {
            Debug.LogError($"未 Serialize {npcEntity.SelfGoPy.GetType()}");
            return npcEntity;
        }
        npcEntity.SelfGoPy.SetActive(true);

        //Transform _transform
        //序列化而来, 只做检测.
        if (npcEntity.SelfTransformPy == null)
        {
            Debug.LogError($"未 Serialize {npcEntity.SelfTransformPy.GetType()}");
            return npcEntity;
        }
        npcEntity.SelfTransformPy.position = _grid.GetCellCenterWorld(npc.CoordSystemPy.CurrentCoordPy);

        //设置好值后, 开启 AI 等.
        hpUiSystem.RefreshAllHpInstantly();
        characterPanelController.RefreshBuffs();
        if (!isInBattle)
        {
#pragma warning disable CS4014
            npcAIForLiving.ProcessAsync();
#pragma warning restore CS4014
        }

        npcEntity.SetHasInitialized();

        return npcEntity;
    }

    public static async UniTask DestroyNpcEntityAsync(CharacterId npcId)
    {
        if (!_characterEntityHub.InstanceId_InstancePy.ContainsKey(npcId.InstanceId)) return;
        NpcEntity npcEntity = npcId.NpcEntityPy;

        List<UniTask> uniTasks = new List<UniTask>(10);
        //结束 AI.
        uniTasks.Add(npcEntity.NpcAIForLivingPy.StopAIAndFormatAsync());
        await UniTask.WhenAll(uniTasks);
        //结束 移动.
        await npcEntity.EntityMoverPy.FormatAsync();

        //从记录中移除.
        _characterEntityHub.RemoveInstance(npcId.InstanceId);

        //然后 Destroy.
        Object.Destroy(npcEntity.SelfGoPy);
    }
}
}