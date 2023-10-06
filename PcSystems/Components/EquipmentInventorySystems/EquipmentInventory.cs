using System;

using LowLevelSystems.ItemSystems.EquipmentSystems.AccessorySystems;
using LowLevelSystems.ItemSystems.EquipmentSystems.WeaponSystems;

using Sirenix.OdinInspector;

namespace LowLevelSystems.CharacterSystems.PcSystems.Components.EquipmentInventorySystems
{
[Serializable]
public class EquipmentInventory
{
    [Title("Data")]
    [ShowInInspector]
    private CharacterId _characterId;
    public CharacterId CharacterIdPy => this._characterId;
    public void SetCharacterId(int characterId)
    {
        this._characterId.InstanceId = characterId;
    }

    [ShowInInspector]
    private Weapon _weapon;
    public Weapon WeaponPy => this._weapon;
    public void SetWeapon(Weapon weapon)        
    {
        this._weapon = weapon;
    }

    [ShowInInspector]
    private Accessory _moBao;
    public Accessory MoBaoPy => this._moBao;
    public void SetMoBao(Accessory moBao)
    {
        this._moBao = moBao;
    }

    [ShowInInspector]
    private Accessory _lingShi;
    public Accessory LingShiPy => this._lingShi;
    public void SetLingShi(Accessory lingShi)
    {
        this._lingShi = lingShi;
    }

    [ShowInInspector]
    private Accessory _caoNang;
    public Accessory CaoNangPy => this._caoNang;
    public void SetCaoNang(Accessory caoNang)
    {
        this._caoNang = caoNang;
    }
}
}