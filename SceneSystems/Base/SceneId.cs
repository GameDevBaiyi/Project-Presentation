using System;

using LowLevelSystems.Common;
using LowLevelSystems.SceneSystems.CitySystems.Base;
using LowLevelSystems.SceneSystems.RoomSystems;

using Sirenix.OdinInspector;

using UnityEngine;

namespace LowLevelSystems.SceneSystems.Base
{
[Serializable]
public struct SceneId
{
    public int Id;
    public SceneId(int id)
    {
        this.Id = id;
    }

    [ShowInInspector]
    public Scene ScenePy => SceneIdDetails.GetScene(this);

    public Room RoomPy => SceneIdDetails.GetRoom(this);
    public City CityPy => SceneIdDetails.GetCity(this);
    public City CityOrParentCityPy => SceneIdDetails.GetCityOrParentCity(this);
}
public abstract class SceneIdDetails : Details
{
    public static Scene GetScene(SceneId sceneId)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) return null;
#endif

        int id = sceneId.Id;
        if (id == 0) return null;

        SceneHub.TryGetInstance(id,out Scene scene);
        return scene;
    }

    public static Room GetRoom(SceneId sceneId)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) return null;
#endif

        int id = sceneId.Id;
        if (id == 0) return null;

        SceneHub.TryGetInstance(id,out Room scene);
        return scene;
    }

    public static City GetCity(SceneId sceneId)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) return null;
#endif

        int id = sceneId.Id;
        if (id == 0) return null;

        SceneHub.TryGetInstance(id,out City scene);
        return scene;
    }

    public static City GetCityOrParentCity(SceneId sceneId)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) return null;
#endif

        int id = sceneId.Id;
        if (id == 0) return null;

        SceneHub.TryGetInstance(id,out Scene scene);
        if (scene is Room room)
        {
            return room.BuildingIdPy.BuildingPy.CityEnumPy.City();
        }
        else if (scene is City city)
        {
            return city;
        }
        else
        {
            Debug.LogError($"未能找到该场景所在的 City. : SceneId: {id}");
            return null;
        }
    }
}
}