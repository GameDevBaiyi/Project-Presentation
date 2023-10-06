namespace LowLevelSystems.SceneSystems.Base
{
public static class TerrainStaticFlagsExtensions
{
    public static TerrainStaticFlags AddFlags(this TerrainStaticFlags terrainStaticFlags,TerrainStaticFlags flags)
    {
        return terrainStaticFlags | flags;
    }

    public static TerrainStaticFlags RemoveFlags(this TerrainStaticFlags terrainStaticFlags,TerrainStaticFlags flags)
    {
        return terrainStaticFlags & ~flags;
    }
}
}