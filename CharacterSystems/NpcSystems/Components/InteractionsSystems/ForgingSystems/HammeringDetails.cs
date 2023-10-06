using JetBrains.Annotations;

using LowLevelSystems.CharacterSystems.Components.PropertySystems;
using LowLevelSystems.Common;
using LowLevelSystems.ItemSystems.Base;
using LowLevelSystems.ItemSystems.EquipmentSystems.AccessorySystems;
using LowLevelSystems.ItemSystems.EquipmentSystems.Base;
using LowLevelSystems.ItemSystems.EquipmentSystems.WeaponSystems;
using LowLevelSystems.QualitySystems;

using UnityEngine;

namespace LowLevelSystems.CharacterSystems.NpcSystems.Components.InteractionsSystems.ForgingSystems
{
public abstract class HammeringDetails : Details
{
    public static void Reset(Hammering hammering,Smelting smelting,EquipmentEmbryo equipmentEmbryo,
                             CharacterId pcId)
    {
        //如果传入的是 null, 就重置各种数据为0.
        if (equipmentEmbryo == null)
        {
            //int _feng
            hammering.SetFeng(default(float));
            //Vector2Int _perfectRangeForFeng
            hammering.SetPerfectRangeForFeng(default(Vector2));
            //Vector2Int _excellentRangeForFeng
            hammering.SetExcellentRangeForFeng(default(Vector2));
            //int _ren
            hammering.SetRen(default(float));
            //Vector2Int _perfectRangeForRen
            hammering.SetPerfectRangeForRen(default(Vector2));
            //Vector2Int _excellentRangeForRen
            hammering.SetExcellentRangeForRen(default(Vector2));
            hammering.SetHammerTimes(0);

            return;
        }

        //如果传入的是未完成的, 报错. 
        if (equipmentEmbryo.LeftHoursPy > 0f)
        {
            Debug.LogError("传入的胚胎, 未完成");
            return;
        }

        //如果传入的是 胚胎, 就计算各种区间等.
        //从熔铸中移除该 胚胎.
        smelting.EquipmentEmbryosPy.Remove(equipmentEmbryo);
        //	起始值范围=（40-A）~-（60+A）；A=（图纸稀有度-1）*10-锻造属性值*5
        //int _feng
        int qualityLevel = 1;
        if (equipmentEmbryo is WeaponEmbryo weaponEmbryo)
        {
            qualityLevel = weaponEmbryo.WeaponBlueprintPy.ItemConfigIdAndQualityEnumPy.QualityEnumPy.QualityConfig().LevelPy;
        }
        else if (equipmentEmbryo is AccessoryEmbryo accessoryEmbryo)
        {
            qualityLevel = accessoryEmbryo.AccessoryBlueprintPy.ItemConfigIdAndQualityEnumPy.QualityEnumPy.QualityConfig().LevelPy;
        }
        int forgingProperty = (int)pcId.PcPy.PropertySystemPy[PropertyEnum.Forging];
        float a = (qualityLevel - 1f) * 10f - forgingProperty * 5f;
        float feng = Random.Range(40f - a,60f + a);
        hammering.SetFeng(feng);

        //Vector2Int _perfectRangeForFeng
        //	区间中点位置：（40-A）~-（60+A）；
        float rangeCenter = Random.Range(40f - a,60f + a);
        //	完美区间一半范围 1.5f +（7f-稀有度）
        float halfPerfectRange = 1.5f + (7f - qualityLevel);
        Vector2 perfectRangeForFeng = new Vector2(rangeCenter - halfPerfectRange,rangeCenter + halfPerfectRange);
        hammering.SetPerfectRangeForFeng(perfectRangeForFeng);

        //Vector2Int _excellentRangeForFeng
        //	区间中点位置：（40-A）~-（60+A）；
        //	精良区间一半范围 5f+（7f-稀有度）
        float halfExcellentRange = 5f + (7f - qualityLevel);
        Vector2 excellentRangeForFeng = new Vector2(rangeCenter - halfExcellentRange,rangeCenter + halfExcellentRange);
        hammering.SetExcellentRangeForFeng(excellentRangeForFeng);

        //int _ren
        float ren = Random.Range(40f - a,60f + a);
        hammering.SetRen(ren);

        //Vector2Int _perfectRangeForRen
        float rangeCenterForRen = Random.Range(40f - a,60f + a);
        Vector2 perfectRangeForRen = new Vector2(rangeCenterForRen - halfPerfectRange,rangeCenterForRen + halfPerfectRange);
        hammering.SetPerfectRangeForRen(perfectRangeForRen);

        //Vector2Int _excellentRangeForRen
        Vector2 excellentRangeForRen = new Vector2(rangeCenterForRen - halfExcellentRange,rangeCenterForRen + halfExcellentRange);
        hammering.SetExcellentRangeForRen(excellentRangeForRen);

        hammering.SetHammerTimes(forgingProperty + 2);
    }

    [CanBeNull]
    public static Equipment GenerateEquipment(Hammering hammering)
    {
        //BaiyiTODO. 待分析.
        if (!hammering.ForgingScorePy.ForgingConfigPy.IsSuccessful) return null;
        if (hammering.EquipmentEmbryoPy is WeaponEmbryo weaponEmbryo)
        {
            return GenerateWeapon(hammering);
        }
        else
        {
            return GenerateAccessory(hammering);
        }
    }

    public static Weapon GenerateWeapon(Hammering hammering)
    {
        if (hammering.EquipmentEmbryoPy is not WeaponEmbryo weaponEmbryo)
        {
            Debug.LogError("打造的不是武器, 却调用了打造武器的方法.");
            return null;
        }

        QualityEnum qualityEnum = weaponEmbryo.WeaponBlueprintPy.ItemConfigIdAndQualityEnumPy.QualityEnumPy;
        qualityEnum = qualityEnum.Subtract(hammering.ForgingScorePy.ForgingConfigPy.NumberOfReducedRarities);
        Weapon weapon = WeaponFactory.GenerateWeapon(new ItemConfigIdAndQualityEnum(weaponEmbryo.WeaponBlueprintPy.ConfigIdOfCraftedItemPy,qualityEnum));

        return weapon;
    }

    public static Accessory GenerateAccessory(Hammering hammering)
    {
        if (hammering.EquipmentEmbryoPy is not AccessoryEmbryo accessoryEmbryo)
        {
            Debug.LogError("打造的不是饰品, 却调用了打造饰品的方法.");
            return null;
        }

        QualityEnum qualityEnum = accessoryEmbryo.AccessoryBlueprintPy.ItemConfigIdAndQualityEnumPy.QualityEnumPy;
        qualityEnum = qualityEnum.Subtract(hammering.ForgingScorePy.ForgingConfigPy.NumberOfReducedRarities);
        Accessory accessory = AccessoryFactory.GenerateAccessory(new ItemConfigIdAndQualityEnum(accessoryEmbryo.AccessoryBlueprintPy.ConfigIdOfCraftedItemPy,qualityEnum));

        return accessory;
    }
}
}