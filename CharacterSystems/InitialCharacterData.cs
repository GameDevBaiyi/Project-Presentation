using System;

using UnityEngine;

namespace LowLevelSystems.CharacterSystems
{
[Serializable]
public class InitialCharacterData
{
    [SerializeField]
    private int _instanceId;
    public int InstanceIdPy => this._instanceId;
    public void SetInstanceId(int instanceId)
    {
        this._instanceId = instanceId;
    }

    [SerializeField]
    private Vector3Int _coord;
    public Vector3Int CoordPy => this._coord;
    public void SetCoord(Vector3Int coord)
    {
        this._coord = coord;
    }

    [SerializeField]
    private int _direction;
    public int DirectionPy => this._direction;
    public void SetDirection(int direction)
    {
        this._direction = direction;
    }

    [SerializeField]
    private bool _isStill;
    public bool IsStillPy => this._isStill;
    public void SetIsStill(bool isStill)
    {
        this._isStill = isStill;
    }
}
}