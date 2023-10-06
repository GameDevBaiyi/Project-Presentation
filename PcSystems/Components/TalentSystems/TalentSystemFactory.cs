using System.Collections.Generic;

using LowLevelSystems.Common;
using LowLevelSystems.SkillSystems.Base;

namespace LowLevelSystems.CharacterSystems.PcSystems.Components.TalentSystems
{
public abstract class TalentSystemFactory : Details
{
    /// <summary>
    /// 会先生成通用 
    /// </summary>
    public static TalentSystem GenerateTalentSystem(List<TalentBookConfig> talentBookConfigsFromPc)
    {
        List<TalentBookConfig> commonBookConfigs = CommonDesignSO.CharacterConfigHubPy.TalentBookConfigsPy;
        Dictionary<SkillMainTypeEnum,TalentBook> mainSkillTypeEnum_book = new Dictionary<SkillMainTypeEnum,TalentBook>(commonBookConfigs.Count + talentBookConfigsFromPc.Count);
        foreach (SkillMainTypeEnum mainSkillTypeEnum in PublicConst.MainSkillTypeEnums)
        {
            TalentBookConfig bookConfig = talentBookConfigsFromPc.Find(t => t.SkillMainTypeEnumPy == mainSkillTypeEnum)
                                       ?? commonBookConfigs.Find(t => t.SkillMainTypeEnumPy == mainSkillTypeEnum);
            if (bookConfig == null) continue;
            List<TalentPage> talentPages = new List<TalentPage>(bookConfig.TalentPageConfigsPy.Count);
            foreach (TalentBookConfig.TalentPageConfig talentPageConfig in bookConfig.TalentPageConfigsPy)
            {
                Dictionary<int,TalentNode> nodeId_node = new Dictionary<int,TalentNode>(talentPageConfig.TalentNodeIdsPy.Count);
                foreach (TalentNodeId talentNodeId in talentPageConfig.TalentNodeIdsPy)
                {
                    nodeId_node[talentNodeId.NodeId] = new TalentNode(talentNodeId);
                }
                TalentPage talentPage = new TalentPage(talentPageConfig.SkillSubTypeEnumPy,nodeId_node);
                talentPages.Add(talentPage);
            }
            TalentBook talentBook = new TalentBook(mainSkillTypeEnum,talentPages);
            mainSkillTypeEnum_book[mainSkillTypeEnum] = talentBook;
        }
        TalentSystem talentSystem = new TalentSystem(mainSkillTypeEnum_book);

        return talentSystem;
    }
}
}