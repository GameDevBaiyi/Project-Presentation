using System;
using System.Collections.Generic;
using System.Linq;

using Common.Utilities;

using JetBrains.Annotations;

using LowLevelSystems.CharacterSystems.NpcSystems;
using LowLevelSystems.SkillSystems.SkillBuffSystems;

using UnityEngine;

namespace LowLevelSystems.CharacterEntitySystems.NpcEntitySystems.Components
{
[Serializable]
public class NoBuffEnemy : NpcTargetConfig
{
    public override SkillTargetTypeEnum SkillTargetTypeEnumPy => SkillTargetTypeEnum.NoBuffEnemy;

    [SerializeField]
    private BuffEnum _buffEnum;
    public BuffEnum BuffEnumPy => this._buffEnum;
    public void SetBuffEnum(BuffEnum buffEnum)
    {
        this._buffEnum = buffEnum;
    }
}
}

namespace LowLevelSystems.CharacterEntitySystems.NpcEntitySystems.Components.NpcAIForLivingSystems
{
public partial class DetailsOfBtForBattle
{
    [CanBeNull]
    private static CharacterEntity GetNoBuffEnemy(NpcEntity npcEntity,NpcTargetConfig npcTargetConfig)
    {
        NoBuffAlly noBuffAlly = (NoBuffAlly)npcTargetConfig;
        BuffEnum buffEnum = noBuffAlly.BuffEnumPy;
        Npc npc = npcEntity.NpcPy;
        Vector3Int npcCoord = npc.CoordSystemPy.CurrentCoordPy;
        IEnumerable<CharacterEntity> characterEntities = _battleManager.FindAllEnemiesOf(npcEntity.NpcPy.CampRelationsPy)
                                                                       .Where(t => t.CharacterPy.PropertySystemPy.IsAlivePy)
                                                                       .Where(t => OffsetUtilities.CalculateSteps(t.CharacterPy.CoordSystemPy.CurrentCoordPy,npcCoord)
                                                                                <= SettingsSo.NpcTargetSelectionRange)
                                                                       .Where(t => !t.BuffPoolPy.HasBuffOf(buffEnum))
                                                                       .OrderBy(t => OffsetUtilities.CalculateSteps(t.CharacterPy.CoordSystemPy.CurrentCoordPy,npcCoord));
        return characterEntities.FirstOrDefault();
    }
}
}