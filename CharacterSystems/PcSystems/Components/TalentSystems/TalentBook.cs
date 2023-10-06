using System;
using System.Collections.Generic;
using System.Linq;

using LowLevelSystems.SkillSystems.Base;

using Sirenix.OdinInspector;

using UnityEngine;

namespace LowLevelSystems.CharacterSystems.PcSystems.Components.TalentSystems
{
[Serializable]
public class TalentBook
{
    [Title("Config")]
    private const int _requiredPointsPerNode = 1;

    [Title("Data")]
    [ShowInInspector]
    private readonly SkillMainTypeEnum _skillMainTypeEnum;
    public SkillMainTypeEnum SkillMainTypeEnumPy => this._skillMainTypeEnum;

    [ShowInInspector]
    private readonly List<TalentPage> _pages;
    public List<TalentPage> PagesPy => this._pages;

    [ShowInInspector]
    private int _talentPoints;
    public int TalentPointsPy => this._talentPoints;
    public void AddTalentPoints(int addend)
    {
        if (addend <= 0)
        {
            Debug.LogError($"天赋点不能减少, 尝试的加值是: {addend}");
            return;
        }
        this._talentPoints += addend;
    }

    public TalentBook(SkillMainTypeEnum skillMainTypeEnum,List<TalentPage> pages)
    {
        this._skillMainTypeEnum = skillMainTypeEnum;
        this._pages = pages;
    }

    [Title("Methods")]
    public bool HasEnoughPointsPy => this._talentPoints >= _requiredPointsPerNode;
    public bool CanUnlockNode(TalentPage talentPage,int nodeId)
    {
        return this.HasEnoughPointsPy && !talentPage.HasPrecedingNodeLocked(nodeId);
    }

    public void UnlockNode(int pageId,int nodeId)
    {
        TalentPage talentPage = this._pages.ElementAtOrDefault(pageId);
        //Debug.
        if (talentPage == null)
        {
            Debug.LogError($"未找到该书: {this._skillMainTypeEnum} 第 {pageId} 页天赋页, 总共只有: {this._pages.Count}");
            return;
        }
        //Debug.
        if (!talentPage.Id_TalentNodePy.TryGetValue(nodeId,out TalentNode talentNode))
        {
            Debug.LogError($"未找到该书: {this._skillMainTypeEnum} 第 {pageId} 页天赋页 的该节点: {nodeId}. ");
            return;
        }

        if (!this.CanUnlockNode(talentPage,nodeId)) return;

        talentNode.Unlock();
        this._talentPoints--;
    }
}
}