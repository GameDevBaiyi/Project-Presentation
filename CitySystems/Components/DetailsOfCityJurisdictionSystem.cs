using Cysharp.Threading.Tasks;

using LowLevelSystems.Common;
using LowLevelSystems.SceneSystems.CitySystems.Base;

using UnityEngine;

namespace LowLevelSystems.SceneSystems.CitySystems.Components
{
public abstract class DetailsOfCityJurisdictionSystem : Details
{
    private static bool _isSwitchingCamp;
    public static async UniTask SwitchCampAsync(CityJurisdictionSystem cityJurisdictionSystem)
    {
        if (_isSwitchingCamp)
        {
            Debug.LogError("正在切换阵营, 请勿重复调用此方法.");
        }

        _isSwitchingCamp = true;

        cityJurisdictionSystem.ChangeCamp();
        await DetailsOfCity.SwitchBuildingStageAsync(cityJurisdictionSystem.CityEnumPy.City(),0,(int)cityJurisdictionSystem.CurrentCampPy - 1);

        _isSwitchingCamp = false;
    }
}
}