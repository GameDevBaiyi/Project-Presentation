using System.Collections.Generic;
using System.Linq;

using Common.Utilities;

using Cysharp.Threading.Tasks;

using LowLevelSystems.CharacterEntitySystems.PcEntitySystems;
using LowLevelSystems.CharacterEntitySystems.PcEntitySystems.Components.PcFSMSystems;
using LowLevelSystems.CharacterSystems.NpcSystems;
using LowLevelSystems.CharacterSystems.PcSystems;
using LowLevelSystems.Common;

using UnityEngine;

namespace LowLevelSystems.CharacterEntitySystems.NpcEntitySystems
{
public abstract class DetailsOfNpcEntity : Details
{
    public static async UniTask DestroyAllNpcEntitiesAsync()
    {
        List<UniTask> destroyAllNpcEntitiesTasks = _characterEntityHub.AllNpcEntitiesPy.Select(t => NpcEntityFactory.DestroyNpcEntityAsync(t.CharacterPy.CharacterIdPy)).ToList();
        await UniTask.WhenAll(destroyAllNpcEntitiesTasks);
    }

    public static async UniTask GenerateNpcEntitiesAsync()
    {
        foreach (Npc npc in SceneHub.CurrentSceneIdPy.ScenePy.NpcsPy)
        {
            await NpcEntityFactory.GenerateNpcEntityAsync(npc);
        }
    }

    public static Vector3Int CalculateTargetCoord(NpcEntity npcEntity)
    {
        Pc currentPc = HeronTeam.CurrentPcInControlPy;
        if (!currentPc.CharacterIdPy.TryGetPcEntity(out PcEntity _))
        {
            Debug.LogError($"与 NpcEntity 交互时, 发现当前 PcEntity 不存在 : {currentPc.CharacterEnumPy} ");
            return Vector3Int.zero;
        }

        Vector3Int pcCoord = currentPc.CoordSystemPy.CurrentCoordPy;
        Vector3Int npcCoord = npcEntity.EntityMoverPy.CoordStoppedAtPy;
        // 计算 PcEntity 要走到的点
        int stepsFromPcToNpc = OffsetUtilities.CalculateSteps(pcCoord,npcCoord);
        // 从 1 到 steps 的环找出 可移动的, 并且有路径的点. 
        Vector3Int pcTargetCoord = Vector3Int.zero;
        List<Vector3Int> ringCache = new List<Vector3Int>(6 * stepsFromPcToNpc);
        for (int range = 1; range < stepsFromPcToNpc; range++)
        {
            OffsetUtilities.GetRing(npcCoord,range,ringCache);
            ringCache.Sort((a,b) => OffsetUtilities.CalculateSteps(pcCoord,a).CompareTo(OffsetUtilities.CalculateSteps(pcCoord,b)));
            foreach (Vector3Int coord in ringCache)
            {
                if (_pathfindingManager.CheckIfInRangeAndWalkable(coord,null,true)
                 && _pathfindingManager.TryFindPath(pcCoord,coord))
                {
                    pcTargetCoord = coord;
                    break;
                }
            }
            if (pcTargetCoord != Vector3Int.zero) break;
        }
        if (pcTargetCoord == Vector3Int.zero)
        {
            pcTargetCoord = pcCoord;
        }

        return pcTargetCoord;
    }

    public static void InteractWithCurrentPcEntity(NpcEntity npcEntity)
    {
        // //Npc 收到信息: 有人尝试叫住
        // npcEntity.NpcAIForLivingPy.SetIsBeingCalledOff(true);
        // npcEntity.NpcAIForLivingPy.SetPcEntityIdCallingOff(currentPc.CharacterIdPy);

        Pc currentPc = HeronTeam.CurrentPcInControlPy;
        if (!currentPc.CharacterIdPy.TryGetPcEntity(out PcEntity currentPcEntity))
        {
            Debug.LogError($"与 NpcEntity 交互时, 发现当前 PcEntity 不存在 : {currentPc.CharacterEnumPy} ");
            return;
        }

        npcEntity.CharacterPanelControllerPy.ShowInteractionsUi(currentPcEntity);
    }
}
}