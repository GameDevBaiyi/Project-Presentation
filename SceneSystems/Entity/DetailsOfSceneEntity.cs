using Cysharp.Threading.Tasks;

using LowLevelSystems.Common;
using LowLevelSystems.SceneSystems.Base;
using LowLevelSystems.SceneSystems.CitySystems.Base;

using UnityEngine;

namespace LowLevelSystems.SceneSystems.Entity
{
public abstract class DetailsOfSceneEntity : Details
{
    public static void HideCurrentEntities()
    {
        if (EntityManager.CurrentSceneEntityPy == null) return;
        EntityManager.CurrentSceneEntityPy.Hide();
    }

    public static async UniTask ShowSceneEntitiesAsync()
    {
        Scene currentScene = SceneHub.CurrentSceneIdPy.ScenePy;

        //Debug.
        if (currentScene is not IHasScenePrefab hasScenePrefab)
        {
            Debug.LogError($"未实现无 Prefab 的 {nameof(SceneEntity)} 显示.");
            return;
        }

        SceneEntity sceneEntity = await SceneEntityFactory.GetSceneEntityAndShowAsync(hasScenePrefab.ScenePrefabEnumPy,currentScene as City);
        EntityManager.SetCurrentSceneEntity(sceneEntity);
    }
}
}