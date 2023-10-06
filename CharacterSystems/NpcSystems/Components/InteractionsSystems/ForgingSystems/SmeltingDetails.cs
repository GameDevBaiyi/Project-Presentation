using LowLevelSystems.Common;
using LowLevelSystems.ItemSystems.Base;
using LowLevelSystems.ItemSystems.EquipmentSystems.AccessorySystems;
using LowLevelSystems.ItemSystems.EquipmentSystems.WeaponSystems;
using LowLevelSystems.ItemSystems.ItemPileSystems;

using UnityEngine;

namespace LowLevelSystems.CharacterSystems.NpcSystems.Components.InteractionsSystems.ForgingSystems
{
public abstract class SmeltingDetails : Details
{
    /// <summary>
    /// 将 熔铸 界面上的蓝图, 放回背包中. 
    /// </summary>
    public static void RecycleBlueprintIntoBackpack(Smelting smelting)
    {
        HeronTeam.BackpackPy.AddItems(smelting.BlueprintPy,1);
        smelting.SetBlueprint(null);
    }

    /// <summary>
    /// 放入或者置换一个蓝图. 
    /// </summary>
    public static void PlaceBlueprint(ItemPileInBackpack itemPileInBackpack,Smelting smelting)
    {
        if (itemPileInBackpack == null)
        {
            Debug.LogError("不应传入 null 道具堆.");
            return;
        }

        //如果指定的道具不是 武器或者饰品蓝图, 报错, 并返回.
        Item item = itemPileInBackpack.ItemPy;
        Item.ItemSubTypeEnum itemSubItemSubTypeEnum = item.ItemSubTypeEnumPy;
        if (itemSubItemSubTypeEnum != Item.ItemSubTypeEnum.WeaponBlueprint
         && itemSubItemSubTypeEnum != Item.ItemSubTypeEnum.AccessoryBlueprint)
        {
            Debug.LogError("指定的道具不是 武器蓝图 或者 饰品蓝图.");
            return;
        }

        //如果栏位上有蓝图, 那么就先放回蓝图于背包中.
        if (smelting.BlueprintPy != null)
        {
            RecycleBlueprintIntoBackpack(smelting);
        }

        //将蓝图放到栏位上, 并从背包中移除
        smelting.SetBlueprint((Blueprint)item);
        HeronTeam.BackpackPy.RemoveItemsFromPile(itemPileInBackpack,1,out _);
    }

    /// <summary>
    /// 开始熔炼. 
    /// </summary>
    public static void StartSmelting(Smelting smelting)
    {
        //未放入蓝图.
        Blueprint smeltingBlueprint = smelting.BlueprintPy;
        if (smeltingBlueprint == null) return;

        Item.ItemSubTypeEnum smeltingBlueprintItemSubTypeEnum = smeltingBlueprint.ItemSubTypeEnumPy;
        if (smeltingBlueprintItemSubTypeEnum != Item.ItemSubTypeEnum.WeaponBlueprint
         && smeltingBlueprintItemSubTypeEnum != Item.ItemSubTypeEnum.AccessoryBlueprint)
        {
            Debug.LogError("不应该即不是武器蓝图也不是饰品蓝图.");
            return;
        }
        
        //如果胚胎库满了, 不能继续熔炼.
        if(smelting.EquipmentEmbryosPy.Count>=smelting.MaxNumberOfEmbryosPy) return;
            
        //根据蓝图尝试生成对应的胚胎. 并添加到当前的 装备胚胎库 中.
        EquipmentEmbryo equipmentEmbryo;
        if (smeltingBlueprintItemSubTypeEnum == Item.ItemSubTypeEnum.WeaponBlueprint)
        {
            WeaponBlueprint weaponBlueprint = smeltingBlueprint as WeaponBlueprint;
            if (!WeaponEmbryoFactory.TryToGenerateWeaponEmbryo(weaponBlueprint,smelting.CharacterIdPy.NpcPy.SceneIdPy.CityOrParentCityPy.CityJurisdictionSystemPy.CurrentCampPy,
                                                               out WeaponEmbryo weaponEmbryo)) return;
            equipmentEmbryo = weaponEmbryo;
        }
        else
        {
            AccessoryBlueprint accessoryBlueprint = smeltingBlueprint as AccessoryBlueprint;
            if (!AccessoryEmbryoFactory.TryToGenerateAccessoryEmbryo(accessoryBlueprint,
                                                                     smelting.CharacterIdPy.NpcPy.SceneIdPy.CityOrParentCityPy.CityJurisdictionSystemPy.CurrentCampPy,
                                                                     out AccessoryEmbryo accessoryEmbryo)) return;
            equipmentEmbryo = accessoryEmbryo;
        }
        smelting.EquipmentEmbryosPy.Add(equipmentEmbryo);

        smelting.SetBlueprint(null);
    }
}
}