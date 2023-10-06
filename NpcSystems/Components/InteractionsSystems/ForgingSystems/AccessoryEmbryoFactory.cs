using System.Collections.Generic;

using LowLevelSystems.Common;
using LowLevelSystems.DateSystems;
using LowLevelSystems.HeronTeamSystems.Components;
using LowLevelSystems.ItemSystems.CurrencySystems;
using LowLevelSystems.ItemSystems.EquipmentSystems.AccessorySystems;

namespace LowLevelSystems.CharacterSystems.NpcSystems.Components.InteractionsSystems.ForgingSystems
{
public abstract class AccessoryEmbryoFactory : Details
{
    /// <summary>
    /// 检测材料是否足够, 并尝试消耗材料生成武器胚胎.
    /// </summary>
    public static bool TryToGenerateAccessoryEmbryo(AccessoryBlueprint accessoryBlueprint,CampEnum moneyCampEnum,out AccessoryEmbryo accessoryEmbryo)
    {
        accessoryEmbryo = null;

        //检测 钱 和 曜石 够不够.
        //钱.
        int requiredMoney = (int)new Currency(CampEnum.Sun,accessoryBlueprint.RequiredSunMoneyPy).ToOtherCurrency(moneyCampEnum).NumberPy;
        if (!HeronTeam.WalletPy.HasEnoughMoney(moneyCampEnum,requiredMoney)) return false;
        //曜石.
        foreach (KeyValuePair<ObsidianEnum,int> keyValuePair in accessoryBlueprint.ObsidianEnum_ValuePy)
        {
            if (!HeronTeam.ObsidianBagPy.HasEnoughObsidian(keyValuePair.Key,keyValuePair.Value)) return false;
        }

        //生成 饰品胚胎.
        accessoryEmbryo = new AccessoryEmbryo();
        //Date _dateSmeltingWasCompleted
        Date dateSmeltingWasCompleted = DateSystem.DatePy + accessoryBlueprint.RequiredHoursPy;
        accessoryEmbryo.SetDateSmeltingWasCompleted(dateSmeltingWasCompleted);
        //AccessoryBlueprint _accessoryBlueprint
        accessoryEmbryo.SetAccessoryBlueprint(accessoryBlueprint);

        //消耗材料.
        HeronTeam.WalletPy.ChangeLimitedMoney(moneyCampEnum,-requiredMoney);
        foreach (KeyValuePair<ObsidianEnum,int> keyValuePair in accessoryBlueprint.ObsidianEnum_ValuePy)
        {
            HeronTeam.ObsidianBagPy.ChangeObsidian(keyValuePair.Key,-keyValuePair.Value);
        }

        return true;
    }
}
}