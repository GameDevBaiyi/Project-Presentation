using System;
using System.Collections.Generic;
using System.Linq;

using Common.Utilities;

using LowLevelSystems.CharacterSystems.Components.PropertySystems;
using LowLevelSystems.CharacterSystems.NpcSystems;

using UnityEngine;

namespace LowLevelSystems.CharacterEntitySystems.NpcEntitySystems.Components.NpcBtForBattleSystems.SpecialActionConditions
{
[Serializable]
public class HasLowHpAlly : SpecialActionCondition
{
    public override SpecialActionConditionEnum SpecialActionConditionEnumPy => SpecialActionConditionEnum.HasLowHpAlly;

    [SerializeField]
    private float _hpPercent;
    public float HpPercentPy => this._hpPercent;
    public void SetHpPercent(float hpPercent)
    {
        this._hpPercent = hpPercent;
    }
}

public abstract partial class DetailsOfSpecialActionCondition
{
    private static bool HasLowHpAlly(NpcEntity npcEntity,HasLowHpAlly hasLowHpAlly)
    {
        Npc npc = npcEntity.NpcPy;
        Vector3Int npcCoord = npc.CoordSystemPy.CurrentCoordPy;
        IEnumerable<CharacterEntity> aliveAllies = _battleManager.FindAllAlliesOf(npcEntity.NpcPy.CampRelationsPy)
                                                                 .Where(t => t.CharacterPy.PropertySystemPy.IsAlivePy)
                                                                 .Where(t => OffsetUtilities.CalculateSteps(t.CharacterPy.CoordSystemPy.CurrentCoordPy,npcCoord)
                                                                          <= SettingsSo.NpcTargetSelectionRange)
                                                                 .Where(t => t.CharacterPy.PropertySystemPy.CurrentHpPy / t.CharacterPy.PropertySystemPy[PropertyEnum.MaxHP]
                                                                          <= hasLowHpAlly.HpPercentPy);
        return aliveAllies.Any();
    }
}
}