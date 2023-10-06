using System.Collections.Generic;

using LowLevelSystems.Common;

namespace LowLevelSystems.CharacterSystems.PcSystems.Components.LuggageSystems
{
public abstract class LuggageFactory : Details
{
    public static Luggage GenerateLuggage(int characterIdParam)
    {
        Luggage luggage = new Luggage();

        //CharacterId _characterId
        luggage.SetCharacterId(characterIdParam);

        //List<CellOfLuggage> _cellsOfLuggage
        int initialCountOfLuggageSlots = luggage.CharacterIdPy.PcPy.CharacterEnumPy.PcConfig().InitialCountOfLuggageSlotsPy;

        List<CellOfLuggage> cellsOfLuggage = new List<CellOfLuggage>(Luggage.MaxNumberOfSlots);
        for (int i = 0; i < initialCountOfLuggageSlots; i++)
        {
            cellsOfLuggage.Add(new CellOfLuggage());
        }
        luggage.SetCellsOfLuggage(cellsOfLuggage);

        return luggage;
    }
}
}