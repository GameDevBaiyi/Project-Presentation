using System;
using System.Collections.Generic;
using System.Linq;

using LowLevelSystems.Common;

using Sirenix.OdinInspector;

// ReSharper disable InconsistentNaming

namespace LowLevelSystems.CharacterSystems.NpcSystems.Components.InteractionsSystems
{
public interface ICanRefreshOnDateChanged
{
    public int RefreshCyclePy { get; }
    public int LastDayRefreshedPy { get; }
    public static bool IsTimeToRefresh(ICanRefreshOnDateChanged canRefreshOnDateChanged)
    {
        int productsRefreshCycle = canRefreshOnDateChanged.RefreshCyclePy;
        int today = Details.DateSystem.DaysPy;
        return canRefreshOnDateChanged.LastDayRefreshedPy < today / productsRefreshCycle * productsRefreshCycle;
    }
    public void CheckAndRefresh();
}

[Serializable]
public abstract class Interaction
{
    public static List<InteractionEnum> InteractionEnums = ((InteractionEnum[])Enum.GetValues(typeof(InteractionEnum))).Where(t => t != InteractionEnum.None).ToList();

    public abstract InteractionEnum InteractionEnumPy { get; }

    [ShowInInspector]
    protected readonly CharacterId _characterId;
    public CharacterId CharacterIdPy => this._characterId;

    protected Interaction(CharacterId characterId)
    {
        this._characterId = characterId;
    }
}
}