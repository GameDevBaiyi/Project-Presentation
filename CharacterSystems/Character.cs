using System;
using System.Collections.Generic;

using Common.Template;

using LowLevelSystems.BattleSystems.Config;
using LowLevelSystems.CharacterSystems.Components.CoordSystems;
using LowLevelSystems.CharacterSystems.Components.LvSystems;
using LowLevelSystems.CharacterSystems.Components.PropertySystems;
using LowLevelSystems.SceneSystems.Base;

using Sirenix.OdinInspector;

// ReSharper disable InconsistentNaming

namespace LowLevelSystems.CharacterSystems
{
[Serializable]
public abstract class Character : IInstance
{
    public enum CharacterTypeEnum
    {
        None,
        Pc,
        Npc,
    }

    [Title("Data")]
    [ShowInInspector]
    protected int _instanceId;
    public int InstanceIdPy => this._instanceId;
    public void SetInstanceId(int instanceId)
    {
        this._instanceId = instanceId;
    }

    [ShowInInspector]
    protected CharacterTypeEnum _characterTypeEnum;
    public CharacterTypeEnum CharacterTypeEnumPy => this._characterTypeEnum;
    public void SetCharacterTypeEnum(CharacterTypeEnum characterTypeEnum)
    {
        this._characterTypeEnum = characterTypeEnum;
    }

    [ShowInInspector]
    protected CharacterEnum _characterEnum;
    public CharacterEnum CharacterEnumPy => this._characterEnum;
    public void SetCharacterEnum(CharacterEnum characterEnum)
    {
        this._characterEnum = characterEnum;
    }

#if UNITY_EDITOR
    [ShowInInspector]
    private CharacterConfig CharacterConfigPyEditorOnly
    {
        get
        {
            switch (this._characterTypeEnum)
            {
            case CharacterTypeEnum.Pc:
                return this._characterEnum.PcConfig();

            case CharacterTypeEnum.Npc:
                return this._characterEnum.CharacterConfig();
            }
            return null;
        }
    }
#endif

    [ShowInInspector]
    protected SceneId _sceneId;
    public SceneId SceneIdPy => this._sceneId;
    public void SetCurrentSceneId(int currentSceneId)
    {
        this._sceneId.Id = currentSceneId;
    }

    [NonSerialized]
    private BattleConfig.CampRelations _campRelations;
    public BattleConfig.CampRelations CampRelationsPy => this._campRelations;
    public void SetCampRelations(BattleConfig.CampRelations campRelations)
    {
        this._campRelations = campRelations;
    }

    [Title("等级")]
    [ShowInInspector]
    private readonly LvSystem _lvSystem = new LvSystem();
    public LvSystem LvSystemPy => this._lvSystem;

    [Title("属性系统")]
    [ShowInInspector]
    protected PropertySystem _propertySystem;
    public PropertySystem PropertySystemPy => this._propertySystem;
    public void SetPropertySystem(PropertySystem propertySystem)
    {
        this._propertySystem = propertySystem;
    }

    [Title("坐标系统")]
    [ShowInInspector]
    protected CoordSystem _coordSystem;
    public CoordSystem CoordSystemPy => this._coordSystem;
    public void SetCoordSystem(CoordSystem coordSystem)
    {
        this._coordSystem = coordSystem;
    }

    [Title("Spine 数据")]
    [ShowInInspector]
    protected Dictionary<string,string> _skinData;
    public Dictionary<string,string> SkinDataPy => this._skinData;
    public void SetSkinData(Dictionary<string,string> skinData)
    {
        this._skinData = skinData;
    }

    [Title("Methods")]
    [ShowInInspector]
    public CharacterId CharacterIdPy => new CharacterId(this._instanceId);
}
}