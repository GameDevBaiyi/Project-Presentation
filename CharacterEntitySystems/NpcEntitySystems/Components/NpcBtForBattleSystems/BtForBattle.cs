using System.Collections.Generic;
using System.Linq;

using Common.BehaviourTree;
using Common.Utilities;

using Cysharp.Threading.Tasks;

using LowLevelSystems.BattleSystems.Components.RoundControllerSystems;
using LowLevelSystems.CharacterEntitySystems.NpcEntitySystems.Components.NpcAIForLivingSystems;
using LowLevelSystems.CharacterEntitySystems.NpcEntitySystems.Components.NpcBtForBattleSystems.SpecialActionConditions;
using LowLevelSystems.CharacterSystems;
using LowLevelSystems.CharacterSystems.NpcSystems;
using LowLevelSystems.PathfindingSystems;
using LowLevelSystems.SkillSystems.Base;
using LowLevelSystems.SkillSystems.Config;

using Sirenix.OdinInspector;

using UnityEngine;

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

namespace LowLevelSystems.CharacterEntitySystems.NpcEntitySystems.Components
{
public class BtForBattle
{
    private readonly Sequence _moveAndAttack;

    private readonly NpcEntity _npcEntity;

    [ShowInInspector]
    private readonly List<AIConfig.NormalAction> _normalActions;
    [ShowInInspector]
    private int _currentNormalActionIndex;
    public void IncreaseCurrentNormalActionIndex()
    {
        this._currentNormalActionIndex++;
        if (this._currentNormalActionIndex >= this._normalActions.Count)
        {
            this._currentNormalActionIndex = 0;
        }
    }
    [ShowInInspector]
    public SkillSugarConfig CurrentSkillSugarConfigPy
    {
        get
        {
            //Debug.
            if (this._normalActions.Count == 0)
            {
                Debug.LogError($"该 Npc: {this._npcEntity.NpcPy.CharacterEnumPy} 的 普通技能轴 为空.");
                return null;
            }

            AIConfig.NormalAction normalAction = this._normalActions[this._currentNormalActionIndex];

            if (normalAction.CanBeReplacedPy)
            {
                SkillSugarConfig skillSugarConfig = DetailsOfSpecialActionCondition.CalculateSpecialSkillSugarConfig(this._npcEntity,this._specialActions);
                if (skillSugarConfig != null) return skillSugarConfig;
            }

            return normalAction.SkillMainIdAndQualityEnumPy.SkillSugarConfigPy;
        }
    }

    [ShowInInspector]
    private readonly List<AIConfig.SpecialAction> _specialActions;

    [ShowInInspector]
    private Vector3Int _skillEffectCenterCoord;
    public Vector3Int SkillEffectCenterCoordPy => this._skillEffectCenterCoord;
    [ShowInInspector]
    private int _skillEffectDirection;
    public int SkillEffectDirectionPy => this._skillEffectDirection;
    [ShowInInspector]
    private Vector3Int _targetCoord;
    public Vector3Int TargetCoordPy => this._targetCoord;

    public BtForBattle(NpcEntity npcEntity)
    {
        TaskNode moveToTargetCoord = new TaskNode(() => DetailsOfBtForBattle.MoveToTargetCoord(this._npcEntity));
        TaskNode attack = new TaskNode(() => DetailsOfBtForBattle.Attack(this._npcEntity));
        this._moveAndAttack = new Sequence(moveToTargetCoord,attack);
        this._npcEntity = npcEntity;
        AIConfig aiConfig = npcEntity.NpcPy.CharacterEnumPy.AIConfig();
        this._normalActions = aiConfig.NormalActionsPy;
        this._specialActions = aiConfig.SpecialActionsPy;
    }

    private async UniTask<BaseNode.StatusEnum> UpdateOneAttackAsync()
    {
        //Debug. 
        Npc npc = this._npcEntity.NpcPy;
        if (this.CurrentSkillSugarConfigPy == null) return BaseNode.StatusEnum.Failure;

        CharacterEntity targetEntity = DetailsOfBtForBattle.CalculateSkillTargetInBattle(this._npcEntity,this.CurrentSkillSugarConfigPy);
        if (targetEntity == null) return BaseNode.StatusEnum.Failure;

        // 开始前, 计算好本次要 走到的点, 和 技能范围的中心点 和 方向, 和实际的生效范围. 
        // 技能范围中心点 就是 目标点.
        this._skillEffectCenterCoord = targetEntity.CharacterPy.CoordSystemPy.CurrentCoordPy;
        // 旋转方向使得该技能尽可能的覆盖到目标点.
        SkillSugarConfig.SkillEffectConfigForOneRound skillEffectConfigForOneRound = this.CurrentSkillSugarConfigPy.SkillEffectConfigPy.First();
        List<Vector3Int> rangeInCubeCoord = skillEffectConfigForOneRound.EffectRangeEnumPy.RangeInCubeCoord();
        int KeySelector(int direction)
        {
            List<Vector3Int> realEffectRange = OffsetUtilities.Convert0DirectionRelativeCubesToOffset(rangeInCubeCoord,this._skillEffectCenterCoord,direction);
            List<CharacterEntity> selectTargets = DetailsOfSkillEffects.SelectTargets(npc,realEffectRange,skillEffectConfigForOneRound.TargetCampEnumsPy);
            return selectTargets.Count;
        }
        int direction = Enumerable.Range(0,6).OrderByDescending(KeySelector).First();
        this._skillEffectDirection = direction;

        // 尝试寻路到最近的能 释放技能的点.
        Vector3Int npcCoord = npc.CoordSystemPy.CurrentCoordPy;
        SkillMainConfig skillMainConfig = this.CurrentSkillSugarConfigPy.SkillMainIdPy.SkillMainConfigPy;
        int skillSelectingRangeForNpc = skillMainConfig.SkillSelectingRangeForNpcPy;
        bool isInRange = OffsetUtilities.CalculateSteps(npcCoord,this._skillEffectCenterCoord) <= skillSelectingRangeForNpc;
        Vector3Int targetCoord;
        if (isInRange)
        {
            targetCoord = npcCoord;
        }
        else
        {
            PathfindingManager pathfindingManager = PathfindingManager.InstancePy;
            int Selector(Vector3Int coord)
            {
                bool hasPath = pathfindingManager.TryFindPath(npcCoord,coord,this._npcEntity.EntityMoverPy.CoordPathPy,npc.CampRelationsPy);
                int pathCount = this._npcEntity.EntityMoverPy.CoordPathPy.Count;
                return hasPath ? pathCount : int.MaxValue;
            }
            targetCoord = OffsetUtilities.GetCoordsInRange(this._skillEffectCenterCoord,skillSelectingRangeForNpc).OrderBy(Selector).First();
        }
        this._targetCoord = targetCoord;

        BaseNode.StatusEnum statusEnum;
        while (true)
        {
            statusEnum = this._moveAndAttack.Update();
            if (statusEnum != BaseNode.StatusEnum.Running) break;
            await UniTask.NextFrame();
        }

        return statusEnum;
    }

    private bool _isUpdating;
    public async UniTask UpdateAsync()
    {
        if (this._isUpdating)
        {
            Debug.LogError($"该 Npc: {this._npcEntity.InstanceIdPy} BtForBattle.UpdateAsync() 正在运行中, 再次尝试 Update.");
            return;
        }

        this._isUpdating = true;
        while (true)
        {
            BaseNode.StatusEnum statusEnum = await this.UpdateOneAttackAsync();
            if (statusEnum == BaseNode.StatusEnum.Failure) break;
        }
        this._isUpdating = false;

        RoundControllerDetails.SwitchRoundAsync();
    }
}
}