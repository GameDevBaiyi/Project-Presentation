using System;

using LowLevelSystems.ItemSystems.EquipmentSystems.AccessorySystems;

using Sirenix.OdinInspector;

namespace LowLevelSystems.CharacterSystems.NpcSystems.Components.InteractionsSystems.ForgingSystems
{
[Serializable]
public class AccessoryEmbryo : EquipmentEmbryo
{
    [ShowInInspector]
    private AccessoryBlueprint _accessoryBlueprint;
    public AccessoryBlueprint AccessoryBlueprintPy => this._accessoryBlueprint;
    public void SetAccessoryBlueprint(AccessoryBlueprint accessoryBlueprint)
    {
        this._accessoryBlueprint = accessoryBlueprint;
    }
}
}