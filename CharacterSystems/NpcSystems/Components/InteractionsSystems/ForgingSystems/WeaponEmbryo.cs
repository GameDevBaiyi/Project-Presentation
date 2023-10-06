using System;

using LowLevelSystems.ItemSystems.EquipmentSystems.WeaponSystems;

using Sirenix.OdinInspector;

namespace LowLevelSystems.CharacterSystems.NpcSystems.Components.InteractionsSystems.ForgingSystems
{
[Serializable]
public class WeaponEmbryo : EquipmentEmbryo
{
    [ShowInInspector]
    private WeaponBlueprint _weaponBlueprint;
    public WeaponBlueprint WeaponBlueprintPy => this._weaponBlueprint;
    public void SetWeaponBlueprint(WeaponBlueprint weaponBlueprint)
    {
        this._weaponBlueprint = weaponBlueprint;
    }
}
}