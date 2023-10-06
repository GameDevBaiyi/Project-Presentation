using LowLevelSystems.Common;

namespace LowLevelSystems.CharacterSystems.PcSystems.Components.EquipmentInventorySystems
{
public abstract class EquipmentInventoryFactory : Details
{
    public static EquipmentInventory GenerateEquipmentInventory(int characterIdParam)
    {
        EquipmentInventory equipmentInventory = new EquipmentInventory();

        //CharacterId _characterId
        equipmentInventory.SetCharacterId(characterIdParam);

        return equipmentInventory;
    }
}
}