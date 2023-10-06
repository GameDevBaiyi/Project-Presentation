using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using LowLevelSystems.Common;
using LowLevelSystems.SceneSystems.Base;
using LowLevelSystems.SceneSystems.CitySystems.Base;

using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering.Universal;

namespace LowLevelSystems.SceneSystems.Entity
{
public abstract class SceneEntityFactory : Details
{
    [ShowInInspector]
    private static Dictionary<ScenePrefabEnum,SceneEntity> _scenePrefabEnum_sceneEntity = new Dictionary<ScenePrefabEnum,SceneEntity>(30);

    /// <summary>
    /// 会生成一个新的, 或者从 记录中 拿出一个.  
    /// </summary>
    public static async UniTask<SceneEntity> GetSceneEntityAndShowAsync(ScenePrefabEnum scenePrefabEnum,City city = null)
    {
        //看看是否有已经生成的.
        if (_scenePrefabEnum_sceneEntity.TryGetValue(scenePrefabEnum,out SceneEntity sceneEntity))
        {
            await sceneEntity.ShowAsync(DateSystem.IsDayTimePy,city);
            return sceneEntity;
        }

        //生成新的.
        GameObject sceneGameObject = await Addressables.InstantiateAsync(scenePrefabEnum.ScenePrefabConfig().SceneEntityAddressPy,Vector3.zero,Quaternion.identity,
                                                                         _hierarchyManager.SceneEntitiesPy);
        sceneEntity = sceneGameObject.GetComponent<SceneEntity>();

        //初次生成销毁一些 Editor 使用的组件.
        foreach (GameObject editorTilemap in sceneEntity.EditorTilemapGOsToDestroyPy)
        {
            Object.Destroy(editorTilemap);
        }

        //处理 Data.
        //ScenePrefabEnum _scenePrefabEnum
        sceneEntity.SetScenePrefabEnum(scenePrefabEnum);

        //不论新旧, 都要 Show 一次.
        await sceneEntity.ShowAsync(DateSystem.IsDayTimePy,city);

        _scenePrefabEnum_sceneEntity[scenePrefabEnum] = sceneEntity;

        return sceneEntity;
    }
}
}