using System;
using System.Collections.Generic;
using System.Linq;

using Common.Template;

using LowLevelSystems.SceneSystems.CitySystems.Base;
using LowLevelSystems.SceneSystems.RoomSystems;

using Sirenix.OdinInspector;

using UnityEngine;

// ReSharper disable InconsistentNaming

namespace LowLevelSystems.SceneSystems.Base
{
[Serializable]
public class SceneHub : SingletonHub<SceneHub,Scene>
{
    [Title("Data")]
    [ShowInInspector]
    private SceneId _currentSceneId;
    public SceneId CurrentSceneIdPy => this._currentSceneId;
    public void SetCurrentSceneId(int currentSceneId)
    {
        this._currentSceneId.Id = currentSceneId;
    }

    [ShowInInspector]
    private SceneId _spaceAvoidingWarSceneId;
    public SceneId SpaceAvoidingWarSceneIdPy => this._spaceAvoidingWarSceneId;
    public void SetSpaceAvoidingWarSceneId(int spaceAvoidingWarSceneId)
    {
        this._spaceAvoidingWarSceneId.Id = spaceAvoidingWarSceneId;
    }

    [ShowInInspector]
    private SceneId _loungeId;
    public SceneId LoungeIdPy => this._loungeId;
    public void SetLoungeId(int loungeId)
    {
        this._loungeId.Id = loungeId;
    }

    [ShowInInspector]
    private CityEnum _currentCityEnum;
    /// <summary>
    /// 表示当前在哪一个城镇, 但当前 Scene 不一定是 City.
    /// </summary>
    public CityEnum CurrentCityEnumPy => this._currentCityEnum;
    public void SetCurrentCityEnum(CityEnum currentCityEnum)
    {
        this._currentCityEnum = currentCityEnum;
    }

    [ShowInInspector]
    private Dictionary<CityEnum,SceneId> _cityEnum_sceneId;
    public Dictionary<CityEnum,SceneId> CityEnum_SceneIdPy => this._cityEnum_sceneId;

    [Title("Methods")]
    [ShowInInspector]
    public IEnumerable<City> AllCitiesPy => this._cityEnum_sceneId.Select(t => t.Value.CityPy);
    [ShowInInspector]
    public IEnumerable<Room> AllRoomsPy => this._instanceId_instance.Values.Where(t => t.SceneTypeEnumPy == SceneTypeEnum.Room).Select(t => (Room)t);

    public override void RecordInstance(Scene instance)
    {
        base.RecordInstance(instance);
        if (instance is City city)
        {
            CityEnum cityEnum = city.CityEnumPy;
            if (this._cityEnum_sceneId.ContainsKey(cityEnum))
            {
                Debug.LogError($"记录 {cityEnum} 时, SceneHub 中已经存在.");
            }
            else
            {
                this._cityEnum_sceneId[cityEnum] = city.SceneIdPy;
            }
        }
    }

    public void Initialize()
    {
        this._instanceId_instance = new Dictionary<int,Scene>(200);
        this._cityEnum_sceneId = new Dictionary<CityEnum,SceneId>(20);
    }
}
}