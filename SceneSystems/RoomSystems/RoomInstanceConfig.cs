using System;
using System.Collections.Generic;

using LowLevelSystems.CharacterSystems;
using LowLevelSystems.SceneSystems.Base;

using UnityEngine;

namespace LowLevelSystems.SceneSystems.RoomSystems
{
[Serializable]
public class RoomInstanceConfig
{
    [SerializeField]
    private ScenePrefabEnum _scenePrefabEnum;
    public ScenePrefabEnum ScenePrefabEnumPy => this._scenePrefabEnum;
    public void SetScenePrefabEnum(ScenePrefabEnum scenePrefabEnum)
    {
        this._scenePrefabEnum = scenePrefabEnum;
    }

    [SerializeField]
    private List<InitialCharacterData> _initialCharacters;
    public List<InitialCharacterData> InitialCharactersPy => this._initialCharacters;
    public void SetInitialCharacters(List<InitialCharacterData> initialCharacters)
    {
        this._initialCharacters = initialCharacters;
    }
}
}