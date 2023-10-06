using System.Collections.Generic;
using System.Linq;

using LowLevelSystems.CharacterSystems;
using LowLevelSystems.CharacterSystems.NpcSystems;
using LowLevelSystems.CharacterSystems.PcSystems;
using LowLevelSystems.Common;
using LowLevelSystems.SceneSystems.Base;

namespace LowLevelSystems.SceneSystems.BlankSpaceSystems
{
public abstract class BlankSpaceDetails : Details
{
    /// <summary>
    /// 将指定场景中的, 非战斗选中的 Pc 和 所有 Npc 放到避战空间中.
    /// </summary>
    public static void AvoidBattleFrom(Scene scene,IEnumerable<CharacterEnum> pcEnumsInBattle)
    {
        Scene spaceAvoidingWar = SceneHub.SpaceAvoidingWarSceneIdPy.ScenePy;
        HashSet<CharacterEnum> pcEnumsSetInBattle = pcEnumsInBattle.ToHashSet();
        List<Pc> pcsOnScene = scene.PcsPy.ToList();
        foreach (Pc pc in pcsOnScene)
        {
            //只处理 非战斗选中的 Pc.
            if (pcEnumsSetInBattle.Contains(pc.CharacterEnumPy)) continue;

            //放到避战空间中.
            SceneDetails.MoveCharacterTo(pc,spaceAvoidingWar);
        }
        List<Npc> npcsOnScene = scene.NpcsPy.ToList();
        foreach (Npc npc in npcsOnScene)
        {
            //所有 Npc 放到避战空间中.
            SceneDetails.MoveCharacterTo(npc,spaceAvoidingWar);
        }
    }

    /// <summary>
    /// 将避战空间中的 Pc 和 Npc 恢复到指定场景中. 
    /// </summary>
    public static void RestoreTo(Scene targetScene)
    {
        Scene spaceAvoidingWar = SceneHub.SpaceAvoidingWarSceneIdPy.ScenePy;
        List<Pc> pcsOnScene = spaceAvoidingWar.PcsPy.ToList();
        foreach (Pc pc in pcsOnScene)
        {
            //放到目标场景中.
            SceneDetails.MoveCharacterTo(pc,targetScene);
        }
        List<Npc> npcsOnScene = spaceAvoidingWar.NpcsPy.ToList();
        foreach (Npc npc in npcsOnScene)
        {
            //所有 Npc 放到目标场景中.
            SceneDetails.MoveCharacterTo(npc,targetScene);
        }
    }
}
}