using LowLevelSystems.Common;

using UnityEngine;

namespace LowLevelSystems.CharacterSystems.Components.CoordSystems
{
public abstract class CoordSystemFactory : Details
{
    public static CoordSystem GenerateCoordSystem(int characterIdParam,Vector3Int currentCoordParam)
    {
        CoordSystem coordSystem = new CoordSystem();

        //int _characterId
        int characterId = characterIdParam;

        //Vector3Int _currentCoord
        Vector3Int currentCoord = currentCoordParam;

        //int _directionIndex
        int directionIndex = Random.Range(0,6);

        coordSystem.SetCharacterId(characterId);
        coordSystem.SetCurrentCoord(currentCoord);
        coordSystem.SetDirectionIndex(directionIndex);

        return coordSystem;
    }
}
}