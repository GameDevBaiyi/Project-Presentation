using System;
using System.Collections.Generic;

using LowLevelSystems.SkillSystems.Base;

using Sirenix.OdinInspector;

using UnityEngine;

namespace LowLevelSystems.CharacterSystems.PcSystems.Components.TalentSystems
{
[Serializable]
public class TalentSystem
{
    [ShowInInspector]
    private readonly Dictionary<SkillMainTypeEnum,TalentBook> _mainSkillTypeEnum_book;
    public Dictionary<SkillMainTypeEnum,TalentBook> MainSkillTypeEnum_BookPy => this._mainSkillTypeEnum_book;

    public TalentSystem(Dictionary<SkillMainTypeEnum,TalentBook> mainSkillTypeEnumBook)
    {
        this._mainSkillTypeEnum_book = mainSkillTypeEnumBook;
    }

    [Title("Methods")]
    public void AddTalentPoints(SkillMainTypeEnum skillMainTypeEnum,int addend)
    {
        if (!this._mainSkillTypeEnum_book.TryGetValue(skillMainTypeEnum,out TalentBook talentBook))
        {
            Debug.LogError($"添加天赋点时, 未找到: {skillMainTypeEnum} 对应的天赋书. ");
            return;
        }

        talentBook.AddTalentPoints(addend);
    }
}
}