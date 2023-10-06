using System;

namespace LowLevelSystems.CharacterEntitySystems.PcEntitySystems.Components.SkillBarSystems
{
[Flags]
public enum NodeOfModuleFlags
{
    None,
    IsWalkable,
    IsSugarPoint,
}

public static class NodeOfModuleFlagsExtensions
{
    public static NodeOfModuleFlags AddFlags(this NodeOfModuleFlags nodeOfModuleFlags,NodeOfModuleFlags flags)
    {
        return nodeOfModuleFlags | flags;
    }

    public static NodeOfModuleFlags RemoveFlags(this NodeOfModuleFlags nodeOfModuleFlags,NodeOfModuleFlags flags)
    {
        return nodeOfModuleFlags & ~flags;
    }
}
}