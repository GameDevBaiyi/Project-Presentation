using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using DG.Tweening;

using FairyGUI;

using UnityEngine;

using UI_GButton_SkillItem = UICommon.UI_GButton_SkillItem;

namespace LowLevelSystems.CharacterEntitySystems.PcEntitySystems.Components.SkillBarSystems
{
public static class ModuleOfSkillBarUtilities
{
    private const float _singleTransitionTimespan = 0.2f;

    public static async UniTask DoUIAnimeAsync(List<Vector3Int> coordsToHide,Dictionary<Vector3Int,Vector3> coord_uiPos,Dictionary<Vector3Int,UI_GButton_SkillItem> coord_ui,
                                               Dictionary<Vector3Int,List<Vector3Int>> coord_path)
    {
        foreach (Vector3Int coordToHide in coordsToHide)
        {
            coord_ui[coordToHide].visible = false;
        }

        List<UniTask> uniTasks = new List<UniTask>(coord_path.Count);
        foreach (KeyValuePair<Vector3Int,List<Vector3Int>> pair in coord_path)
        {
            uniTasks.Add(DoSinglePathAnimeAsync(coord_ui[pair.Key],coord_path[pair.Key],coord_uiPos));
        }

        await UniTask.WhenAll(uniTasks);
    }

    private static async UniTask DoSinglePathAnimeAsync(GObject ui,List<Vector3Int> path,Dictionary<Vector3Int,Vector3> coord_uiPos)
    {
        Sequence sequence = DOTween.Sequence();
        foreach (Vector3Int wayPoint in path)
        {
            if (!coord_uiPos.TryGetValue(wayPoint,out Vector3 uiPos)) continue;

            sequence.Append(DOTween.To(() => ui.position,t => ui.position = t,uiPos,_singleTransitionTimespan));
        }

        bool hasDone = false;
        sequence.AppendCallback(() => hasDone = true);
        sequence.Play();

        await UniTask.WaitUntil(() => hasDone);
    }
}
}