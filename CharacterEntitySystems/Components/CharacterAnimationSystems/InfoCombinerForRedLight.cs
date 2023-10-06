using System.Collections.Generic;

using JetBrains.Annotations;

namespace LowLevelSystems.CharacterEntitySystems.Components.CharacterAnimationSystems
{
public static class InfoCombinerForRedLight
{
    // 设计: 思路和 DamageUi 相同. 见 InfoCombinerForDamageUi.

    public static void CombineRedLightInfo(ref List<CharacterEntity> infoOfRedLight,CharacterEntity characterEntity,bool hasHpChanged)
    {
        if (!hasHpChanged) return;
        infoOfRedLight ??= new List<CharacterEntity>(2);
        if (infoOfRedLight.Exists(t => t.InstanceIdPy == characterEntity.InstanceIdPy)) return;
        infoOfRedLight.Add(characterEntity);
    }

    public static void CombineRedLightInfo(ref Dictionary<int,CharacterEntity> infoOfRedLight,[CanBeNull] List<CharacterEntity> lowLevelInfoOfRedLight)
    {
        if (lowLevelInfoOfRedLight == null) return;

        infoOfRedLight ??= new Dictionary<int,CharacterEntity>(lowLevelInfoOfRedLight.Count);
        foreach (CharacterEntity characterEntity in lowLevelInfoOfRedLight)
        {
            infoOfRedLight.TryAdd(characterEntity.InstanceIdPy,characterEntity);
        }
    }
}
}