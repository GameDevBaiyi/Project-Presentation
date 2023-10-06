using LowLevelSystems.Common;
using LowLevelSystems.SkillSystems.Base;

namespace LowLevelSystems.CharacterSystems.PcSystems.Components.BagOfLearnedSkillSystems
{
public abstract class CellOfSkillBagFactory : Details
{
    public static CellOfSkillBag GenerateCellOfSkillBag(int rowIndexParam,int columnIndexParam,int characterId)
    {
        CellOfSkillBag cellOfSkillBag = new CellOfSkillBag();

        //int _rowIndex
        int rowIndex = rowIndexParam;

        //int _columnIndex
        int columnIndex = columnIndexParam;

        //SkillSugarStringId _sugarStringId
        SkillSugarStringId sugarStringId = default(SkillSugarStringId);
        sugarStringId.CharacterId.InstanceId = characterId;

        //MainSkillIdAndQualityEnum _mainSkillIdAndQualityEnum
        SkillMainIdAndQualityEnum skillMainIdAndQualityEnum = default(SkillMainIdAndQualityEnum);

        cellOfSkillBag.SetRowIndex(rowIndex);
        cellOfSkillBag.SetColumnIndex(columnIndex);
        cellOfSkillBag.SetSugarStringId(sugarStringId);
        cellOfSkillBag.SetMainSkillIdAndQualityEnum(skillMainIdAndQualityEnum);

        return cellOfSkillBag;
    }
}
}