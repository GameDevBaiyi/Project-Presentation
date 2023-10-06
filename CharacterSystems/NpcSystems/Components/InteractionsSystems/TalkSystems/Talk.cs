using System;
using System.Collections.Generic;

using LowLevelSystems.LocalizationSystems;

using Sirenix.OdinInspector;

namespace LowLevelSystems.CharacterSystems.NpcSystems.Components.InteractionsSystems.TalkSystems
{
[Serializable]
public class Talk : Interaction
{
    public override InteractionEnum InteractionEnumPy => InteractionEnum.Talk;

    [ShowInInspector]
    private readonly List<TextId> _textIds;
    public List<TextId> TextIdsPy => this._textIds;

    public Talk(CharacterId characterId,IEnumerable<TextId> initialTextIds) : base(characterId)
    {
        this._textIds = new List<TextId>(initialTextIds);
    }
}
}