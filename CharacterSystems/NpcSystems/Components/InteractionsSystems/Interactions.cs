using System;
using System.Collections.Generic;

using Sirenix.OdinInspector;

using UnityEngine;

namespace LowLevelSystems.CharacterSystems.NpcSystems.Components.InteractionsSystems
{
[Serializable]
public class Interactions
{
    [Title("Data")]
    [ShowInInspector]
    private readonly CharacterId _characterId;
    public CharacterId CharacterIdPy => this._characterId;

    [ShowInInspector]
    private readonly Dictionary<InteractionEnum,Interaction> _interactionEnum_interaction = new Dictionary<InteractionEnum,Interaction>(Interaction.InteractionEnums.Count);
    public Dictionary<InteractionEnum,Interaction> InteractionEnum_InteractionPy => this._interactionEnum_interaction;

    public Interactions(CharacterId characterId)
    {
        this._characterId = characterId;
    }

    [Title("Methods")]
    public T GetInteraction<T>(InteractionEnum interactionEnum) where T : Interaction
    {
        if (!this._interactionEnum_interaction.TryGetValue(interactionEnum,out Interaction interaction))
        {
            Debug.LogError($"该 Npc: {this._characterId.InstanceId} 的该交互功能: {interactionEnum} 不存在. ");
            return null;
        }
        return (T)interaction;
    }
}
}