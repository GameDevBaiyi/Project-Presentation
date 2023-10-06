using System;
using System.Collections.Generic;
using System.Linq;

using Common.Utilities;

using LowLevelSystems.CharacterSystems.NpcSystems;

using UnityEngine;

namespace LowLevelSystems.CharacterEntitySystems.NpcEntitySystems.Components.NpcBtForBattleSystems.SpecialActionConditions
{
[Serializable]
public class HasMultipleEnemies : SpecialActionCondition
{
    public override SpecialActionConditionEnum SpecialActionConditionEnumPy => SpecialActionConditionEnum.HasMultipleEnemies;

    [SerializeField]
    private int _enemyCount;
    public int EnemyCountPy => this._enemyCount;
    public void SetEnemyCount(int enemyCount)
    {
        this._enemyCount = enemyCount;
    }
}

public abstract partial class DetailsOfSpecialActionCondition
{
    private static bool HasMultipleEnemies(NpcEntity npcEntity,HasMultipleEnemies hasMultipleEnemies)
    {
        Npc npc = npcEntity.NpcPy;
        Vector3Int npcCoord = npc.CoordSystemPy.CurrentCoordPy;
        IEnumerable<CharacterEntity> aliveEnemies = _battleManager.FindAllEnemiesOf(npcEntity.NpcPy.CampRelationsPy)
                                                                  .Where(t => t.CharacterPy.PropertySystemPy.IsAlivePy)
                                                                  .Where(t => OffsetUtilities.CalculateSteps(t.CharacterPy.CoordSystemPy.CurrentCoordPy,npcCoord)
                                                                           <= SettingsSo.NpcTargetSelectionRange);
        return aliveEnemies.Count() >= hasMultipleEnemies.EnemyCountPy;
    }
}
}