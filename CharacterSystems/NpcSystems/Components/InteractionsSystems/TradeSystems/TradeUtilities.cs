namespace LowLevelSystems.CharacterSystems.NpcSystems.Components.InteractionsSystems.TradeSystems
{
public static class TradeUtilities
{
    /// <summary>
    /// 天权: 购买物品花费=物品价值*(1-角色交易属性*3%)
    /// </summary>
    public static int PurchaseFormulaOnAuthorityOfSky(float productValue,int dealPropertyValueOfPc)
    {
        return (int)(productValue * (1f - dealPropertyValueOfPc * 0.03f));
    }

    /// <summary>
    /// 天权: 出售物品收益=物品价值* (1-税率)* (1+角色交易属性*2%)
    /// </summary>
    public static int SellFormulaOnAuthorityOfSky(float productValue,int dealPropertyValueOfPc,float tax)
    {
        return (int)(productValue * (1f - tax) * (1f + dealPropertyValueOfPc * 0.02f));
    }

    /// <summary>
    /// 隼月: 购买物品花费=物品价值*(1-角色交易属性*3%)*(80-150%)
    /// </summary>
    public static int PurchaseFormulaOnFalconMoon(float productValue,int dealPropertyValueOfPc,float currentPricePercent)
    {
        return (int)(productValue * currentPricePercent * (1f - dealPropertyValueOfPc * 0.03f));
    }

    /// <summary>
    /// 隼月: 出售物品收益=物品价值* (1-税率)* (1+角色交易属性*2%)
    /// </summary>
    public static int SellFormulaOnFalconMoon(float productValue,int dealPropertyValueOfPc,float tax)
    {
        return (int)(productValue * (1f - tax) * (1f + dealPropertyValueOfPc * 0.02f));
    }
}
}