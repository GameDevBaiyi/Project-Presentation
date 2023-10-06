using System.Collections.Generic;

using LowLevelSystems.Common;
using LowLevelSystems.DateSystems;
using LowLevelSystems.ItemSystems.CurrencySystems;
using LowLevelSystems.MissionSystems.PlotGuideSystems;

using UnityEngine;

namespace LowLevelSystems.CharacterSystems.PcSystems.Components.InterestSystems
{
public abstract class DetailsOfInterestSystem : Details
{
    private static readonly List<PlotGuideConfig> _penaltyGuideConfigs = new List<PlotGuideConfig>() { new MovePcsToHotel(), };

    public static bool HasEnoughInterestAndPromptIfNot(InterestSystem interestSystem,float neededInterest)
    {
        if (interestSystem.HasEnoughInterestValue(neededInterest)) return true;

        UiManager.PromptOnMousePosPy.Show(InterestSystem.HasNoEnoughInterestsPromptTextId.TextPy);
        return false;
    }

    public static void ChangeLimitedValue(InterestSystem interestSystem,float addend,bool isIgnoringEvents = false,
                                          bool willChangeTime = true)
    {
        float currentInterestValue = interestSystem.CurrentInterestValuePy;
        float targetValue = Mathf.Clamp(currentInterestValue + addend,0f,interestSystem.MaxInterestValuePy);
        //计算实际改变值.
        float realValue = targetValue - currentInterestValue;
        //确定改变后的值.
        interestSystem.SetCurrentInterestValue(targetValue);

        //如果兴致值实际减少了.
        if (willChangeTime && realValue < 0f)
        {
            DateSystemDetails.ChangeValue(-realValue / SettingsSo.InterestCostPerHour);
        }

        if (isIgnoringEvents) return;
        InterestSystem.InvokeOnInterestChanged();
    }

    /// <summary>
    /// 仅 移动 导致兴致值降到 0, 会有惩罚. 
    /// </summary>
    public static void PenalizePcWhenInterestDropsTo0(Pc pc)
    {
        MovePcsToHotel movePcsToHotel = (MovePcsToHotel)_penaltyGuideConfigs[0];
        movePcsToHotel.CharacterEnumsPy.Clear();
        movePcsToHotel.CharacterEnumsPy.Add(pc.CharacterEnumPy);
        PlotGuideManager.EnqueuePlotGuides(_penaltyGuideConfigs);
        DetailsOfCurrency.DeductCurrency(SettingsSo.HotelChargesWhenPassedOut);
        float interestAddend = pc.InterestSystemPy.MaxInterestValuePy * SettingsSo.InterestRecoveryPctWhenPassedOut;
        DetailsOfInterestSystem.ChangeLimitedValue(pc.InterestSystemPy,interestAddend);
    }
}
}