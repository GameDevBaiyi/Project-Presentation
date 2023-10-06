using System;
using System.Collections.Generic;
using System.Linq;

using Common.Utilities;

using JetBrains.Annotations;

using LowLevelSystems.CharacterSystems.Components.PropertySystems;
using LowLevelSystems.CharacterSystems.NpcSystems;

using UnityEngine;

namespace LowLevelSystems.CharacterEntitySystems.NpcEntitySystems.Components
{
[Serializable]
public class LowestHpAlly : NpcTargetConfig
{
    public override SkillTargetTypeEnum SkillTargetTypeEnumPy => SkillTargetTypeEnum.LowestHpAlly;
}
}

namespace LowLevelSystems.CharacterEntitySystems.NpcEntitySystems.Components.NpcAIForLivingSystems
{
public partial class DetailsOfBtForBattle
{
    [CanBeNull]
    private static CharacterEntity GetLowestHpAlly(NpcEntity npcEntity)
    {
        Npc npc = npcEntity.NpcPy;
        Vector3Int npcCoord = npc.CoordSystemPy.CurrentCoordPy;
        IEnumerable<CharacterEntity> aliveAllies = _battleManager.FindAllAlliesOf(npcEntity.NpcPy.CampRelationsPy)
                                                                 .Where(t => t.CharacterPy.PropertySystemPy.IsAlivePy)
                                                                 .Where(t => OffsetUtilities.CalculateSteps(t.CharacterPy.CoordSystemPy.CurrentCoordPy,npcCoord)
                                                                          <= SettingsSo.NpcTargetSelectionRange)
                                                                 .OrderBy(t => t.CharacterPy.PropertySystemPy.CurrentHpPy / t.CharacterPy.PropertySystemPy[PropertyEnum.MaxHP]);
        return aliveAllies.FirstOrDefault();
    }
}
}