using System;
using System.Collections.Generic;
using System.Linq;

using Common.Utilities;

using JetBrains.Annotations;

using LowLevelSystems.CharacterSystems.NpcSystems;

using UnityEngine;

namespace LowLevelSystems.CharacterEntitySystems.NpcEntitySystems.Components
{
[Serializable]
public class NearestEnemy : NpcTargetConfig
{
    public override SkillTargetTypeEnum SkillTargetTypeEnumPy => SkillTargetTypeEnum.NearestEnemy;
}
}

namespace LowLevelSystems.CharacterEntitySystems.NpcEntitySystems.Components.NpcAIForLivingSystems
{
public partial class DetailsOfBtForBattle
{
    [CanBeNull]
    private static CharacterEntity GetNearestEnemy(NpcEntity npcEntity)
    {
        Npc npc = npcEntity.NpcPy;
        Vector3Int npcCoord = npc.CoordSystemPy.CurrentCoordPy;
        IEnumerable<CharacterEntity> aliveEnemies = _battleManager.FindAllEnemiesOf(npcEntity.NpcPy.CampRelationsPy)
                                                                  .Where(t => t.CharacterPy.PropertySystemPy.IsAlivePy)
                                                                  .OrderBy(t => OffsetUtilities.CalculateSteps(t.CharacterPy.CoordSystemPy.CurrentCoordPy,npcCoord));

        return aliveEnemies.FirstOrDefault();
    }
}
}