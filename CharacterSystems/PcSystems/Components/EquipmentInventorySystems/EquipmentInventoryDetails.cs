using LowLevelSystems.CharacterEntitySystems.PcEntitySystems;
using LowLevelSystems.Common;
using LowLevelSystems.ItemSystems.BackpackSystems;
using LowLevelSystems.ItemSystems.Base;
using LowLevelSystems.ItemSystems.EquipmentSystems.AccessorySystems;
using LowLevelSystems.ItemSystems.EquipmentSystems.Base;
using LowLevelSystems.ItemSystems.EquipmentSystems.WeaponSystems;
using LowLevelSystems.ItemSystems.ItemPileSystems;

using UnityEngine;

namespace LowLevelSystems.CharacterSystems.PcSystems.Components.EquipmentInventorySystems
{
public abstract class EquipmentInventoryDetails : Details
{
    public static void EquipWeapon(Pc pc,ItemPileInBackpack itemPileInBackpack)
    {
        //先检测该道具堆装的是否是装备. 如果不是, 报错.
        Item item = itemPileInBackpack.ItemPy;
        if (item.ItemSubTypeEnumPy != Item.ItemSubTypeEnum.Weapon)
        {
            Debug.LogError($"装备武器时, 指定的道具堆的道具不是 武器, 而是: {item.ItemSubTypeEnumPy}, Id: {itemPileInBackpack.InstanceIdPy}");
            return;
        }

        //如果 Pc 的武器栏没有武器, 那么就是放入该武器.
        EquipmentInventory pcEquipmentInventory = pc.EquipmentInventoryPy;
        Backpack heronTeamBackpack = HeronTeam.BackpackPy;
        if (pcEquipmentInventory.WeaponPy == null)
        {
            pcEquipmentInventory.SetWeapon((Weapon)item);
            heronTeamBackpack.RemoveItemsFromPile(itemPileInBackpack,1,out int _);
            //交换武器可能会影响 Entity.
            if (pc.CharacterIdPy.TryGetPcEntity(out PcEntity pcEntity))
            {
                pcEntity.SpineControllerPy.SetWeaponSkinData();
            }
            return;
        }

        //如果 Pc 的武器栏有武器, 那么就是交换该武器.
        Weapon previousWeapon = pcEquipmentInventory.WeaponPy;
        pcEquipmentInventory.SetWeapon((Weapon)item);
        heronTeamBackpack.RemoveItemsFromPile(itemPileInBackpack,1,out int _);
        heronTeamBackpack.AddItems(previousWeapon,1);

        //交换武器可能会影响 Entity.
        if (pc.CharacterIdPy.TryGetPcEntity(out PcEntity pcEntity1))
        {
            pcEntity1.SpineControllerPy.SetWeaponSkinData();
        }
    }

    public static void EquipWeapon(Pc pc,Weapon weapon)
    {
        //如果 Pc 的武器栏没有武器, 那么就是放入该武器.
        EquipmentInventory pcEquipmentInventory = pc.EquipmentInventoryPy;
        Backpack heronTeamBackpack = HeronTeam.BackpackPy;
        if (pcEquipmentInventory.WeaponPy == null)
        {
            pcEquipmentInventory.SetWeapon(weapon);
            //交换武器可能会影响 Entity.
            if (pc.CharacterIdPy.TryGetPcEntity(out PcEntity pcEntity))
            {
                pcEntity.SpineControllerPy.SetWeaponSkinData();
            }
            return;
        }

        //如果 Pc 的武器栏有武器, 那么就是交换该武器.
        Weapon previousWeapon = pcEquipmentInventory.WeaponPy;
        pcEquipmentInventory.SetWeapon(weapon);
        heronTeamBackpack.AddItems(previousWeapon,1);

        //交换武器可能会影响 Entity.
        if (pc.CharacterIdPy.TryGetPcEntity(out PcEntity pcEntity1))
        {
            pcEntity1.SpineControllerPy.SetWeaponSkinData();
        }
    }

    public static void RemoveWeapon(Pc pc)
    {
        EquipmentInventory pcEquipmentInventory = pc.EquipmentInventoryPy;
        Weapon weapon = pcEquipmentInventory.WeaponPy;
        if (weapon == null)
        {
            Debug.LogError("当前角色的武器栏是空的, 不能卸下.");
            return;
        }
        HeronTeam.BackpackPy.AddItems(weapon,1);
        pcEquipmentInventory.SetWeapon(null);

        //交换武器可能会影响 Entity.
        if (pc.CharacterIdPy.TryGetPcEntity(out PcEntity pcEntity))
        {
            pcEntity.SpineControllerPy.SetWeaponSkinData();
        }
    }

    public static void EquipAccessory(Pc pc,ItemPileInBackpack itemPileInBackpack,EquipmentSubTypeEnum equipmentSubTypeEnum)
    {
        //先检测该道具堆装的是否是 Accessory 且匹配 具体类型. 如果不是, 报错.
        Item item = itemPileInBackpack.ItemPy;
        if (item.ItemSubTypeEnumPy != Item.ItemSubTypeEnum.Accessory)
        {
            Debug.LogError($"装备武器时, 指定的道具堆的道具不是 武器, 而是: {item.ItemSubTypeEnumPy}, Id: {itemPileInBackpack.InstanceIdPy}");
            return;
        }
        Accessory accessory = item as Accessory;
        if (accessory.ItemConfigIdAndQualityEnumPy.GetItemConfig<AccessoryConfig>().EquipmentSubTypeEnumPy != equipmentSubTypeEnum)
        {
            Debug.LogError($"装备饰品时, 饰品和栏不匹配. 道具堆的饰品为: {accessory.ItemConfigIdAndQualityEnumPy.GetItemConfig<AccessoryConfig>().EquipmentSubTypeEnumPy}, 指定的饰品为: {equipmentSubTypeEnum}");
            return;
        }

        //如果 Pc 的武器栏没有武器, 那么就是放入该武器.
        EquipmentInventory pcEquipmentInventory = pc.EquipmentInventoryPy;
        Backpack heronTeamBackpack = HeronTeam.BackpackPy;
        Accessory previousAccessory = null;
        switch (equipmentSubTypeEnum)
        {
            case EquipmentSubTypeEnum.SP01:
                previousAccessory = pcEquipmentInventory.MoBaoPy;
                break;

            case EquipmentSubTypeEnum.SP02:
                previousAccessory = pcEquipmentInventory.LingShiPy;
                break;

            case EquipmentSubTypeEnum.SP03:
                previousAccessory = pcEquipmentInventory.CaoNangPy;
                break;

            default:
                Debug.LogError($"不应出现: {equipmentSubTypeEnum}");
                break;
        }
        if (previousAccessory == null)
        {
            switch (equipmentSubTypeEnum)
            {
                case EquipmentSubTypeEnum.SP01:
                    pcEquipmentInventory.SetMoBao(accessory);
                    break;

                case EquipmentSubTypeEnum.SP02:
                    pcEquipmentInventory.SetLingShi(accessory);
                    break;

                case EquipmentSubTypeEnum.SP03:
                    pcEquipmentInventory.SetCaoNang(accessory);
                    break;

                default:
                    Debug.LogError($"不应出现: {equipmentSubTypeEnum}");
                    break;
            }
            heronTeamBackpack.RemoveItemsFromPile(itemPileInBackpack,1,out int _);
            return;
        }

        //如果 Pc 的武器栏有武器, 那么就是交换该武器.
        switch (equipmentSubTypeEnum)
        {
            case EquipmentSubTypeEnum.SP01:
                pcEquipmentInventory.SetMoBao(accessory);
                break;

            case EquipmentSubTypeEnum.SP02:
                pcEquipmentInventory.SetLingShi(accessory);
                break;

            case EquipmentSubTypeEnum.SP03:
                pcEquipmentInventory.SetCaoNang(accessory);
                break;

            default:
                Debug.LogError($"不应出现: {equipmentSubTypeEnum}");
                break;
        }
        heronTeamBackpack.RemoveItemsFromPile(itemPileInBackpack,1,out int _);
        heronTeamBackpack.AddItems(previousAccessory,1);
    }

    public static void RemoveAccessory(Pc pc,EquipmentSubTypeEnum equipmentSubTypeEnum)
    {
        EquipmentInventory pcEquipmentInventory = pc.EquipmentInventoryPy;
        Accessory previousAccessory = null;
        switch (equipmentSubTypeEnum)
        {
            case EquipmentSubTypeEnum.SP01:
                previousAccessory = pcEquipmentInventory.MoBaoPy;
                break;

            case EquipmentSubTypeEnum.SP02:
                previousAccessory = pcEquipmentInventory.LingShiPy;
                break;

            case EquipmentSubTypeEnum.SP03:
                previousAccessory = pcEquipmentInventory.CaoNangPy;
                break;

            default:
                Debug.LogError($"不应出现: {equipmentSubTypeEnum}");
                break;
        }
        if (previousAccessory == null)
        {
            Debug.LogError($"当前角色的饰品栏: {equipmentSubTypeEnum} 是空的, 不能卸下.");
            return;
        }
        HeronTeam.BackpackPy.AddItems(previousAccessory,1);
        switch (equipmentSubTypeEnum)
        {
            case EquipmentSubTypeEnum.SP01:
                pcEquipmentInventory.SetMoBao(null);
                break;

            case EquipmentSubTypeEnum.SP02:
                pcEquipmentInventory.SetLingShi(null);
                break;

            case EquipmentSubTypeEnum.SP03:
                pcEquipmentInventory.SetCaoNang(null);
                break;

            default:
                Debug.LogError($"不应出现: {equipmentSubTypeEnum}");
                break;
        }
    }
}
}