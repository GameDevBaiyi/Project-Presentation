using System;
using System.Collections.Generic;

using LowLevelSystems.CharacterSystems.NpcSystems.Components.InteractionsSystems.ForgingSystems;
using LowLevelSystems.CharacterSystems.NpcSystems.Components.InteractionsSystems.TradeSystems;

using UnityEngine;

namespace LowLevelSystems.CharacterSystems.NpcSystems.Components.InteractionsSystems
{
[Serializable]
public class InteractionConfigHub
{
    [SerializeField]
    private List<StoreLevelConfig> _storeLevelConfigs;
    private Dictionary<int,StoreLevelConfig> _storeLevel_storeLevelConfig;
    public Dictionary<int,StoreLevelConfig> StoreLevel_StoreLevelConfigPy => this._storeLevel_storeLevelConfig;

    [SerializeField]
    private List<HammerActionConfig> _hammerActionConfigs;
    private Dictionary<Hammering.HammerActionEnum,Dictionary<Hammering.IntensityEnum,HammerActionConfig>> _hammerActionEnum_intensityEnum_hammerActionConfig;
    public Dictionary<Hammering.HammerActionEnum,Dictionary<Hammering.IntensityEnum,HammerActionConfig>> HammerActionEnum_IntensityEnum_HammerActionConfigPy =>
        this._hammerActionEnum_intensityEnum_hammerActionConfig;

    public void Initialize()
    {
        this._storeLevel_storeLevelConfig = new Dictionary<int,StoreLevelConfig>(this._storeLevelConfigs.Count);
        foreach (StoreLevelConfig storeLevelConfig in this._storeLevelConfigs)
        {
            this._storeLevel_storeLevelConfig[storeLevelConfig.StoreLevelPy] = storeLevelConfig;
        }

        this._hammerActionEnum_intensityEnum_hammerActionConfig = new Dictionary<Hammering.HammerActionEnum,Dictionary<Hammering.IntensityEnum,HammerActionConfig>>(3);
        foreach (HammerActionConfig hammerActionConfig in this._hammerActionConfigs)
        {
            if (this._hammerActionEnum_intensityEnum_hammerActionConfig.TryGetValue(hammerActionConfig.HammerActionEnumPy,
                                                                                    out Dictionary<Hammering.IntensityEnum,HammerActionConfig> intensityEnum_hammerActionConfig))
            {
                intensityEnum_hammerActionConfig[hammerActionConfig.IntensityEnumPy] = hammerActionConfig;
            }
            else
            {
                this._hammerActionEnum_intensityEnum_hammerActionConfig[hammerActionConfig.HammerActionEnumPy]
                    = new Dictionary<Hammering.IntensityEnum,HammerActionConfig>() { { hammerActionConfig.IntensityEnumPy,hammerActionConfig }, };
            }
        }
    }

    public void SetStoreLevelConfigs(List<StoreLevelConfig> storeLevelConfigs)
    {
        this._storeLevelConfigs = storeLevelConfigs;
    }
    public void SetHammerActionConfigs(List<HammerActionConfig> hammerActionConfigs)
    {
        this._hammerActionConfigs = hammerActionConfigs;
    }
}
}