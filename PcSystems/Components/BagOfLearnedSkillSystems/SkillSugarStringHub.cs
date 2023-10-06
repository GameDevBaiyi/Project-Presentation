using System;
using System.Collections.Generic;

using Common.Template;

using LowLevelSystems.SkillSystems.SkillSugarStringSystems;

namespace LowLevelSystems.CharacterSystems.PcSystems.Components.BagOfLearnedSkillSystems
{
[Serializable]
public class SkillSugarStringHub : InstanceHub<SkillSugarString>
{
    public SkillSugarStringHub()
    {
        this._instanceId_instance = new Dictionary<int,SkillSugarString>(20);
    }
}
}