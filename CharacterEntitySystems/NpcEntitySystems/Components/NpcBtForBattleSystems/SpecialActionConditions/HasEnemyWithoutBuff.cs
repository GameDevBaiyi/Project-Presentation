using System;
using System.Collections.Generic;
using System.Linq;

using Common.Utilities;

using LowLevelSystems.CharacterSystems.NpcSystems;
using LowLevelSystems.SkillSystems.SkillBuffSystems;

using UnityEngine;

namespace LowLevelSystems.CharacterEntitySystems.NpcEntitySystems.Components.NpcBtForBattleSystems.SpecialActionConditions
{
[Serializable]
public class HasEnemyWithoutBuff : SpecialActionCondition
{
    public override SpecialActionConditionEnum SpecialActionConditionEnumPy => SpecialActionConditionEnum.HasEnemyWithoutBuff;

    [SerializeField]
    private BuffEnum _buffEnum;
    public BuffEnum BuffEnumPy => this._buffEnum;
    public void SetBuffEnum(BuffEnum buffEnum)
    {
        this._buffEnum = buffEnum;
    }
}

public abstract partial class DetailsOfSpecialActionCondition
{
    private static bool HasEnemyWithoutBuff(NpcEntity npcEntity,HasEnemyWithoutBuff hasEnemyWithoutBuff)
    {
        Npc npc = npcEntity.NpcPy;
        Vector3Int npcCoord = npc.CoordSystemPy.CurrentCoordPy;
        IEnumerable<CharacterEntity> aliveEnemies = _battleManager.FindAllEnemiesOf(npcEntity.NpcPy.CampRelationsPy)
                                                                  .Where(t => t.CharacterPy.PropertySystemPy.IsAlivePy)
                                                                  .Where(t => OffsetUtilities.CalculateSteps(t.CharacterPy.CoordSystemPy.CurrentCoordPy,npcCoord)
                                                                           <= SettingsSo.NpcTargetSelectionRange)
                                                                  .Where(t => !t.BuffPoolPy.HasBuffOf(hasEnemyWithoutBuff.BuffEnumPy));
        return aliveEnemies.Any();
    }
}
}