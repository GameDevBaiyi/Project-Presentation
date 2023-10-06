using System;
using System.Collections.Generic;

using LowLevelSystems.CharacterSystems.NpcSystems.Components.InteractionsSystems;
using LowLevelSystems.LocalizationSystems;

using UnityEngine;

namespace LowLevelSystems.CharacterSystems.NpcSystems
{
[Serializable]
public class SpecialNpcConfig
{
    [SerializeField]
    private int _characterId;
    public int CharacterIdPy => this._characterId;
    public void SetCharacterId(int characterId)
    {
        this._characterId = characterId;
    }

    [SerializeField]
    private CharacterEnum _characterEnum;
    public CharacterEnum CharacterEnumPy => this._characterEnum;
    public void SetCharacterEnum(CharacterEnum characterEnum)
    {
        this._characterEnum = characterEnum;
    }

    [SerializeField]
    private TextId _name;
    public TextId NamePy => this._name;
    public void SetCharacterName(TextId name)
    {
        this._name = name;
    }
    

    [SerializeReference]
    private List<InteractionConfig> _extraInteractionConfigs = new List<InteractionConfig>();
    public List<InteractionConfig> ExtraInteractionConfigsPy => this._extraInteractionConfigs;
    public void SetInteractionConfigs(List<InteractionConfig> extraInteractionConfigs)
    {
        this._extraInteractionConfigs = extraInteractionConfigs;
    }
}
}