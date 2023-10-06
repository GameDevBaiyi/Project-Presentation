using System;
using System.Collections.Generic;

using LowLevelSystems.LocalizationSystems;

using UnityEngine;

namespace LowLevelSystems.CharacterSystems.NpcSystems.Components.InteractionsSystems.TalkSystems
{
[Serializable]
public class TalkConfig : InteractionConfig
{
    public override InteractionEnum InteractionEnumPy => InteractionEnum.Talk;

    [SerializeField]
    private List<TextId> _textIds = new List<TextId>();
    public List<TextId> TextIdsPy => this._textIds;
    public void SetTextIds(List<TextId> textIds)
    {
        this._textIds = textIds;
    }
}
}