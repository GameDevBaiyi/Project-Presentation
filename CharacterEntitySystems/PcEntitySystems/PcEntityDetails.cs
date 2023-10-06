using System.Collections.Generic;
using System.Linq;

using Cysharp.Threading.Tasks;

using LowLevelSystems.CharacterSystems.Components.CoordSystems;
using LowLevelSystems.CharacterSystems.PcSystems;
using LowLevelSystems.Common;

using UnityEngine;

namespace LowLevelSystems.CharacterEntitySystems.PcEntitySystems
{
public abstract class PcEntityDetails : Details
{
    public static void Show(PcEntity pcEntity)
    {
        //Debug.
        if (pcEntity.EntityMoverPy.IsMovingPy)
        {
            Debug.LogError($"显示 PcEntity 时, 发现其仍在移动, 是否上面的流程有问题? ");
        }

        CoordSystem pcCoordSystem = pcEntity.PcPy.CoordSystemPy;
        pcEntity.SelfTransformPy.position = _grid.GetCellCenterWorld(pcCoordSystem.CurrentCoordPy);
        pcEntity.CharacterAnimationSystemPy.DoIdleAnime();
        bool isInBattle = _battleManager.IsInBattlePy;
#pragma warning disable CS4014
        //不需要 await 到主线程.
        pcEntity.CharacterPanelControllerPy.SelfAdaptionAsync();
#pragma warning restore CS4014
        pcEntity.CharacterPanelControllerPy.ChangeBuffsUIVisible(isInBattle);
        pcEntity.CharacterPanelControllerPy.HideInteraction();
        pcEntity.CharacterPanelControllerPy.ChangeDirectionUIVisible(false);
        pcEntity.ChangeDirection(pcEntity.PcPy.CoordSystemPy.DirectionIndexPy);
        pcEntity.HpUiSystemPy.ChangeHpVisible(isInBattle);
        pcEntity.BuffPoolPy.Clear();

        if (isInBattle)
        {
            pcEntity.CharacterPanelControllerPy.RefreshBuffs();
            pcEntity.HpUiSystemPy.RefreshAllHpInstantly();
            pcEntity.SkillDrawPilePy.ResetDrawPile();
            pcEntity.SkillBarPy.ResetSkillBar();
        }

        pcEntity.SelfGoPy.SetActive(true);
    }

    public static async UniTask HidePcEntitiesAsync()
    {
        IEnumerable<UniTask> changeVisibleTasks = _characterEntityHub.AllPcEntitiesPy.Select(t => t.HideAsync());
        await UniTask.WhenAll(changeVisibleTasks);
    }

    public static async UniTask ShowPcEntitiesAsync()
    {
        foreach (Pc pc in SceneHub.CurrentSceneIdPy.ScenePy.PcsPy)
        {
            //先生成或者拿到已有的对应的 PcEntity.
            if (!_characterEntityHub.InstanceId_InstancePy.TryGetValue(pc.InstanceIdPy,out CharacterEntity characterEntity))
            {
                characterEntity = await PcEntityFactory.GeneratePcEntityAsync(pc);
            }
            PcEntity pcEntity = (PcEntity)characterEntity;
            pcEntity.SetPc(pc);
            PcEntityDetails.Show(pcEntity);
        }
    }

    public static async UniTask DestroyAllPcEntitiesAsync()
    {
        List<UniTask> destroyAllPcEntitiesTasks = _characterEntityHub.AllPcEntitiesPy.Select(t => PcEntityFactory.DestroyPcEntityAsync(t.CharacterPy.CharacterIdPy)).ToList();
        await UniTask.WhenAll(destroyAllPcEntitiesTasks);
    }
}
}