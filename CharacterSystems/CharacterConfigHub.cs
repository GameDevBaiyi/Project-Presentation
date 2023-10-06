using System;
using System.Collections.Generic;

using LowLevelSystems.CharacterSystems.Components.PropertySystems;
using LowLevelSystems.CharacterSystems.NpcSystems;
using LowLevelSystems.CharacterSystems.PcSystems;
using LowLevelSystems.CharacterSystems.PcSystems.Components.TalentSystems;

using Sirenix.OdinInspector;

using UnityEngine;

namespace LowLevelSystems.CharacterSystems
{
[Serializable]
public class CharacterConfigHub
{
    [Searchable]
    [SerializeField]
    private List<CharacterConfig> _npcConfigs;
    public List<CharacterConfig> NpcConfigsPy => this._npcConfigs;
    private Dictionary<CharacterEnum,CharacterConfig> _characterEnum_config;
    public Dictionary<CharacterEnum,CharacterConfig> CharacterEnum_ConfigPy => this._characterEnum_config;
    public void SetNpcConfigs(List<CharacterConfig> npcConfigs)
    {
        this._npcConfigs = npcConfigs;
    }

    [Searchable]
    [SerializeField]
    private List<PcConfig> _pcConfigs;
    private Dictionary<CharacterEnum,PcConfig> _pcEnum_config;
    public Dictionary<CharacterEnum,PcConfig> PCEnum_ConfigPy => this._pcEnum_config;
    public void SetPlayerCharacterConfigRows(List<PcConfig> playerCharacterConfigRows)
    {
        this._pcConfigs = playerCharacterConfigRows;
    }

    [Searchable]
    [SerializeField]
    private List<SpecialNpcConfig> _specialNpcConfigs;
    public List<SpecialNpcConfig> SpecialNpcConfigsPy => this._specialNpcConfigs;
    private Dictionary<int,SpecialNpcConfig> _characterId_specialNpcConfig;
    public Dictionary<int,SpecialNpcConfig> CharacterId_SpecialNpcConfigPy => this._characterId_specialNpcConfig;
    public void SetSpecialNpcConfigs(List<SpecialNpcConfig> specialNpcConfigs)
    {
        this._specialNpcConfigs = specialNpcConfigs;
    }

    [SerializeField]
    private AIConfig _commonAIConfig;
    public AIConfig CommonAIConfigPy => this._commonAIConfig;
    public void SetCommonAIConfig(AIConfig commonAIConfig)
    {
        this._commonAIConfig = commonAIConfig;
    }

    [SerializeField]
    private List<AIConfig> _aiConfigs;
    private Dictionary<CharacterEnum,AIConfig> _characterEnum_aiConfig;
    public Dictionary<CharacterEnum,AIConfig> CharacterEnum_AIConfigPy => this._characterEnum_aiConfig;
    public void SetAiConfigs(List<AIConfig> aiConfigs)
    {
        this._aiConfigs = aiConfigs;
    }

    [SerializeField]
    private List<PropertyConfig> _propertyConfigs = new List<PropertyConfig>();
    private Dictionary<PropertyEnum,PropertyConfig> _propertyEnum_config;
    public Dictionary<PropertyEnum,PropertyConfig> PropertyEnum_ConfigPy => this._propertyEnum_config;
    public void SetPropertyConfigs(List<PropertyConfig> propertyConfigs)
    {
        this._propertyConfigs = propertyConfigs;
    }

    [SerializeField]
    private List<CharacteristicPropertyConfig> _characteristicPropertyConfigs = new List<CharacteristicPropertyConfig>();
    private Dictionary<PropertyEnum,Dictionary<CharacterEnum,CharacteristicPropertyConfig>> _characteristicPropertyEnum_characterEnum_config;
    public Dictionary<PropertyEnum,Dictionary<CharacterEnum,CharacteristicPropertyConfig>> CharacteristicPropertyEnum_CharacterEnum_ConfigPy =>
        this._characteristicPropertyEnum_characterEnum_config;
    public void SetCharacteristicPropertyConfigs(List<CharacteristicPropertyConfig> characteristicPropertyConfigs)
    {
        this._characteristicPropertyConfigs = characteristicPropertyConfigs;
    }

    [SerializeField]
    private List<TalentBookConfig> _talentBookConfigs;
    public List<TalentBookConfig> TalentBookConfigsPy => this._talentBookConfigs;
    public void SetTalentBookConfigs(List<TalentBookConfig> talentBookConfigs)
    {
        this._talentBookConfigs = talentBookConfigs;
    }

    [SerializeField]
    private List<TalentBookConfig.TalentNodeConfig> _talentNodeConfigs = new List<TalentBookConfig.TalentNodeConfig>();
    private Dictionary<int,TalentBookConfig.TalentNodeConfig> _id_talentNodeConfigs;
    public Dictionary<int,TalentBookConfig.TalentNodeConfig> Id_TalentNodeConfigsPy => this._id_talentNodeConfigs;
    public void SetTalentNodeConfigs(List<TalentBookConfig.TalentNodeConfig> talentNodeConfigs)
    {
        this._talentNodeConfigs = talentNodeConfigs;
    }

    [SerializeField]
    private List<NpcPropertyLvConfig> _npcPropertyLvConfigs = new List<NpcPropertyLvConfig>();
    public List<NpcPropertyLvConfig> NpcPropertyLvConfigsPy => this._npcPropertyLvConfigs;
    public void SetNpcPropertyLvConfigs(List<NpcPropertyLvConfig> npcPropertyLvConfigs)
    {
        this._npcPropertyLvConfigs = npcPropertyLvConfigs;
    }

    public void Initialize()
    {
        foreach (CharacterConfig npcConfig in this._npcConfigs)
        {
            npcConfig.Initialize();
        }
        this._characterEnum_config = new Dictionary<CharacterEnum,CharacterConfig>(this._pcConfigs.Count + this._npcConfigs.Count);
        foreach (CharacterConfig npcConfigRow in this._npcConfigs)
        {
            this._characterEnum_config[npcConfigRow.CharacterEnumPy] = npcConfigRow;
        }
        foreach (PcConfig pcConfig in this._pcConfigs)
        {
            pcConfig.Initialize();
        }
        foreach (PcConfig characterConfigRow in this._pcConfigs)
        {
            this._characterEnum_config[characterConfigRow.CharacterEnumPy] = characterConfigRow;
        }

        this._pcEnum_config = new Dictionary<CharacterEnum,PcConfig>(this._pcConfigs.Count);
        foreach (PcConfig characterConfigRow in this._pcConfigs)
        {
            this._pcEnum_config[characterConfigRow.CharacterEnumPy] = characterConfigRow;
        }

        this._characterId_specialNpcConfig = new Dictionary<int,SpecialNpcConfig>(this._specialNpcConfigs.Count);
        foreach (SpecialNpcConfig specialNpcConfig in this._specialNpcConfigs)
        {
            this._characterId_specialNpcConfig[specialNpcConfig.CharacterIdPy] = specialNpcConfig;
        }

        this._characterEnum_aiConfig = new Dictionary<CharacterEnum,AIConfig>(this._aiConfigs.Count);
        foreach (AIConfig aiConfig in this._aiConfigs)
        {
            this._characterEnum_aiConfig[aiConfig.CharacterEnumPy] = aiConfig;
        }

        this._propertyEnum_config = new Dictionary<PropertyEnum,PropertyConfig>(this._propertyConfigs.Count);
        foreach (PropertyConfig propertyConfig in this._propertyConfigs)
        {
            this._propertyEnum_config[propertyConfig.PropertyEnumPy] = propertyConfig;
        }

        this._characteristicPropertyEnum_characterEnum_config = new Dictionary<PropertyEnum,Dictionary<CharacterEnum,CharacteristicPropertyConfig>>();
        foreach (CharacteristicPropertyConfig characteristicPropertyConfig in this._characteristicPropertyConfigs)
        {
            if (!this._characteristicPropertyEnum_characterEnum_config.TryGetValue(characteristicPropertyConfig.PropertyEnumPy,
                                                                                   out Dictionary<CharacterEnum,CharacteristicPropertyConfig> characterEnum_config))
            {
                characterEnum_config = new Dictionary<CharacterEnum,CharacteristicPropertyConfig>();
                this._characteristicPropertyEnum_characterEnum_config[characteristicPropertyConfig.PropertyEnumPy] = characterEnum_config;
            }
            characterEnum_config[characteristicPropertyConfig.CharacterEnumPy] = characteristicPropertyConfig;
        }

        this._id_talentNodeConfigs = new Dictionary<int,TalentBookConfig.TalentNodeConfig>(this._talentNodeConfigs.Count);
        foreach (TalentBookConfig.TalentNodeConfig talentNodeConfig in this._talentNodeConfigs)
        {
            this._id_talentNodeConfigs[talentNodeConfig.NodeIdPy] = talentNodeConfig;
        }
    }
}
}