using System;

using Sirenix.OdinInspector;

namespace LowLevelSystems.CharacterSystems.PcSystems.Components.TalentSystems
{
[Serializable]
public class TalentNode
{
    [ShowInInspector]
    private readonly TalentNodeId _talentNodeId;
    public TalentNodeId TalentNodeIdPy => this._talentNodeId;

    [ShowInInspector]
    private bool _isLocked = true;
    public bool IsLockedPy => this._isLocked;
    public void Unlock()
    {
        this._isLocked = false;
    }

    public TalentNode(TalentNodeId talentNodeId)
    {
        this._talentNodeId = talentNodeId;
    }
}
}