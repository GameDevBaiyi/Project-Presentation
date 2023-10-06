using System;

using LowLevelSystems.Common;
using LowLevelSystems.DateSystems;

using Sirenix.OdinInspector;

namespace LowLevelSystems.CharacterSystems.NpcSystems.Components.InteractionsSystems.ForgingSystems
{
[Serializable]
public class EquipmentEmbryo
{
    [Title("Data")]
    [ShowInInspector]
    private Date _dateSmeltingWasCompleted;
    public Date DateSmeltingWasCompletedPy => this._dateSmeltingWasCompleted;
    public void SetDateSmeltingWasCompleted(Date dateSmeltingWasCompleted)
    {
        this._dateSmeltingWasCompleted = dateSmeltingWasCompleted;
    }

    [Title("Config")]
    [ShowInInspector]
    public float LeftHoursPy => EquipmentEmbryoDetails.CalculateLeftHours(this);
}
public abstract class EquipmentEmbryoDetails : Details
{
    public static float CalculateLeftHours(EquipmentEmbryo equipmentEmbryo)
    {
        return equipmentEmbryo.DateSmeltingWasCompletedPy - DateSystem.DatePy;
    }
}
}