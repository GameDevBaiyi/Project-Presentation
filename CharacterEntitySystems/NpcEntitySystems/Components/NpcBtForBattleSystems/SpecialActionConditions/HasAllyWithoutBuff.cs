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
public class HasAllyWithoutBuff : SpecialActionCondition
{
    public override SpecialActionConditionEnum SpecialActionConditionEnumPy => SpecialActionConditionEnum.HasAllyWithoutBuff;

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
    private static bool HasAllyWithoutBuff(NpcEntity npcEntity,HasAllyWithoutBuff hasAllyWithoutBuff)
    {
        Npc npc = npcEntity.NpcPy;
        Vector3Int npcCoord = npc.CoordSystemPy.CurrentCoordPy;
        IEnumerable<CharacterEntity> aliveAllies = _battleManager.FindAllAlliesOf(npcEntity.NpcPy.CampRelationsPy)
                                                                 .Where(t => t.CharacterPy.PropertySystemPy.IsAlivePy)
                                                                 .Where(t => OffsetUtilities.CalculateSteps(t.CharacterPy.CoordSystemPy.CurrentCoordPy,npcCoord)
                                                                          <= SettingsSo.NpcTargetSelectionRange)
                                                                 .Where(t => !t.BuffPoolPy.HasBuffOf(hasAllyWithoutBuff.BuffEnumPy));
        return aliveAllies.Any();
    }
}
}