using LowLevelSystems.Common;
using LowLevelSystems.ItemSystems.CurrencySystems;
using LowLevelSystems.WorldSystems;

namespace LowLevelSystems.CharacterSystems.NpcSystems.Components.InteractionsSystems.CurrencyExchangeSystems
{
public abstract class DetailsOfCurrencyExchange : Details
{
    // 金额 = 计算汇率后的金额 * (1f - 税率 * 手续费乘数). 
    public static float CalculateExchangeResult(CampEnum baseCamp,float baseCurrency,CampEnum targetCamp)
    {
        return new Currency(baseCamp,baseCurrency).ToOtherCurrency(targetCamp).NumberPy * (1f - targetCamp.Camp().TaxPy * SettingsSo.ExchangeFeeMultiplier);
    }

    public static void ConfirmExchange(CampEnum baseCamp,float baseCurrency,CampEnum targetCamp)
    {
        float exchangeResult = CalculateExchangeResult(baseCamp,baseCurrency,targetCamp);
        HeronTeam.WalletPy.ChangeLimitedMoney(baseCamp,(int)-baseCurrency);
        HeronTeam.WalletPy.ChangeLimitedMoney(targetCamp,(int)exchangeResult);
    }
}
}