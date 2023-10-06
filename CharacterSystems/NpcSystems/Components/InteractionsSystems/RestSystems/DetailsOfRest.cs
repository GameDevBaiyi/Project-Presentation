using Cysharp.Threading.Tasks;

using LoadingPackage;

using LowLevelSystems.CharacterEntitySystems.PcEntitySystems;
using LowLevelSystems.CharacterSystems.Components.PropertySystems;
using LowLevelSystems.CharacterSystems.PcSystems;
using LowLevelSystems.CharacterSystems.PcSystems.Components.InterestSystems;
using LowLevelSystems.Common;
using LowLevelSystems.HeronTeamSystems.Components;

using UnityEngine;

namespace LowLevelSystems.CharacterSystems.NpcSystems.Components.InteractionsSystems.RestSystems
{
public abstract class DetailsOfRest : Details
{
    private static bool _isResting;
    public static async UniTask Rest(PcEntity pcEntity)
    {
        if (_isResting) return;
        if (!DetailsOfWallet.HasEnoughMoney(SettingsSo.InnCost))
        {
            Debug.LogError($"钱不够, Ui 需要提示, 不能调用该方法.");
            return;
        }
        _isResting = true;
        DetailsOfWallet.ChangeLimitedMoney(-SettingsSo.InnCost);

        await BlackLoading.OpenAsync();

        // 恢复 兴致值.
        Pc pc = pcEntity.PcPy;
        InterestSystem interestSystem = pc.InterestSystemPy;
        float interestValue = interestSystem.MaxInterestValuePy * (SettingsSo.InterestRecoveryPctWhenRestingAtInn / 100f);
        DetailsOfInterestSystem.ChangeLimitedValue(interestSystem,interestValue);

        // 恢复三种 Hp.
        PropertySystem propertySystem = pc.PropertySystemPy;
        float hp = propertySystem[PropertyEnum.MaxHP] * (SettingsSo.HpRecoveryPctWhenRestingAtInn / 100f);
        float bodyVeinHp = propertySystem[PropertyEnum.MaxBodyVeinHp] * (SettingsSo.HpRecoveryPctWhenRestingAtInn / 100f);
        float spiritVeinHp = propertySystem[PropertyEnum.MaxSpiritVeinHp] * (SettingsSo.HpRecoveryPctWhenRestingAtInn / 100f);
        propertySystem.ChangeHp(hp,bodyVeinHp,spiritVeinHp);

        await BlackLoading.CloseAsync();

        _isResting = false;
    }
}
}