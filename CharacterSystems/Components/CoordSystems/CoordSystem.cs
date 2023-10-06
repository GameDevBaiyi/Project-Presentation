using System;

using Sirenix.OdinInspector;

using UnityEngine;

// ReSharper disable InconsistentNaming

namespace LowLevelSystems.CharacterSystems.Components.CoordSystems
{
[Serializable]
public class CoordSystem
{
    [Title("Data")]
    [ShowInInspector]
    private CharacterId _characterId;
    public CharacterId CharacterIdPy => this._characterId;
    public void SetCharacterId(int characterId)
    {
        this._characterId.InstanceId = characterId;
    }

    [ShowInInspector]
    private Vector3Int _currentCoord;
    public Vector3Int CurrentCoordPy => this._currentCoord;
    public void SetCurrentCoord(Vector3Int currentCoord)
    {
        this._currentCoord = currentCoord;
    }

    [ShowInInspector]
    private int _directionIndex;
    public int DirectionIndexPy => this._directionIndex;
    public void SetDirectionIndex(int directionIndex)
    {
        this._directionIndex = directionIndex;
    }
}
}