using System;

using LowLevelSystems.CharacterSystems.NpcSystems.Components.InteractionsSystems;

using Sirenix.OdinInspector;

using UnityEngine;

namespace LowLevelSystems.CharacterSystems.NpcSystems
{
[Serializable]
public class Npc : Character
{
    [SerializeField]
    private bool _isStillOnLiving;
    public bool IsStillOnLivingPy => this._isStillOnLiving;
    public void SetIsStillOnLiving(bool isStillOnLiving)
    {
        this._isStillOnLiving = isStillOnLiving;
    }

    [ShowInInspector]
    private Vector3Int _respawnCoordInCity;
    public Vector3Int RespawnCoordInCityPy => this._respawnCoordInCity;

    public void SetRespawnCoordInCity(Vector3Int respawnCoordInCity)
    {
        this._respawnCoordInCity = respawnCoordInCity;
    }

    [ShowInInspector]
    private Interactions _interactions;
    public Interactions InteractionsPy => this._interactions;

    public void SetExtraInteractions(Interactions interactions)
    {
        this._interactions = interactions;
    }

    [ShowInInspector]
    public string NamePy
    {
        get
        {
            string name;
            SpecialNpcConfig characterIdSpecialNpcConfigWithoutError = this.CharacterIdPy.SpecialNpcConfigWithoutErrorPy;
            if (characterIdSpecialNpcConfigWithoutError != null
             && characterIdSpecialNpcConfigWithoutError.NamePy.Id != 0)
            {
                name = characterIdSpecialNpcConfigWithoutError.NamePy.TextPy;
            }
            else
            {
                name = this.CharacterEnumPy.CharacterConfig().CharacterNamePy;
            }
            return name;
        }
    }
}
}