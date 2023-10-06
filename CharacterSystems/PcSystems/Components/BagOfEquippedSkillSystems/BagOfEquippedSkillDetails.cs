using System.Collections.Generic;

using LowLevelSystems.Common;

namespace LowLevelSystems.CharacterSystems.PcSystems.Components.BagOfEquippedSkillSystems
{
public abstract class BagOfEquippedSkillDetails : Details
{
    public static List<int> GetMaxNumberOfCellsPerRow(BagOfEquippedSkill bagOfEquippedSkill)
    {
        return bagOfEquippedSkill.CharacterIdPy.PcPy.CharacterEnumPy.PcConfig().MaxCellCountPerRowOnEquippedSkillBagPy;
    }

    public static List<int> GetInitialCountOfCellsPerRow(BagOfEquippedSkill bagOfEquippedSkill)
    {
        return bagOfEquippedSkill.CharacterIdPy.PcPy.CharacterEnumPy.PcConfig().InitialUnlockedCellCountPerRowOnEquippedSkillBagPy;
    }
}
}