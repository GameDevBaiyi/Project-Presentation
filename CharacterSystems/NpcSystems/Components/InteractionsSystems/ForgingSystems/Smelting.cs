using System;
using System.Collections.Generic;

using LowLevelSystems.ItemSystems.Base;

using Sirenix.OdinInspector;

namespace LowLevelSystems.CharacterSystems.NpcSystems.Components.InteractionsSystems.ForgingSystems
{
[Serializable]
public class Smelting
{
    [ShowInInspector]
    private CharacterId _characterId;
    public CharacterId CharacterIdPy => this._characterId;
    public void SetCharacterId(int characterId)
    {
        this._characterId.InstanceId = characterId;
    }

    [ShowInInspector]
    private Blueprint _blueprint;
    public Blueprint BlueprintPy => this._blueprint;
    public void SetBlueprint(Blueprint blueprint)
    {
        this._blueprint = blueprint;
    }

    [ShowInInspector]
    private List<EquipmentEmbryo> _equipmentEmbryos;
    public List<EquipmentEmbryo> EquipmentEmbryosPy => this._equipmentEmbryos;
    public void SetEquipmentEmbryos(List<EquipmentEmbryo> equipmentEmbryos)
    {
        this._equipmentEmbryos = equipmentEmbryos;
    }

    [ShowInInspector]
    private int _maxNumberOfEmbryos;
    public int MaxNumberOfEmbryosPy => this._maxNumberOfEmbryos;
    public void SetMaxNumberOfEmbryos(int maxNumberOfEmbryos)
    {
        this._maxNumberOfEmbryos = maxNumberOfEmbryos;
    }
}
}