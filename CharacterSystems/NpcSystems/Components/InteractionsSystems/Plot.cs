using System;
using System.Collections.Generic;

using LowLevelSystems.LocalizationSystems;

using Sirenix.OdinInspector;

using UnityEngine;

namespace LowLevelSystems.CharacterSystems.NpcSystems.Components.InteractionsSystems
{
[Serializable]
public class Plot : Interaction
{
    private const InteractionEnum _interactionEnum = InteractionEnum.Plot;
    public override InteractionEnum InteractionEnumPy => _interactionEnum;

    [ShowInInspector]
    private readonly List<(Vector3Int MissionConditionsKey,TextId TextId)> _missionConditionsKeyAndTextId = new List<(Vector3Int,TextId)>(5);
    public List<(Vector3Int,TextId)> MissionConditionsKeyAndTextIdPy => this._missionConditionsKeyAndTextId;

    public Plot(CharacterId characterId) : base(characterId)
    {
    }
}
}