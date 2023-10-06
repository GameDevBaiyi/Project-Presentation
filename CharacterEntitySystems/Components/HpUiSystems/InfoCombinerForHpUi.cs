using System.Collections.Generic;

using JetBrains.Annotations;

namespace LowLevelSystems.CharacterEntitySystems.Components.HpUiSystems
{
public static class InfoCombinerForHpUi
{
    // 设计: 思路和 DamageUi 相同. 见 InfoCombinerForDamageUi.

    public static void CombineHpUiInfo(ref List<CharacterEntity> infoOfHpUi,CharacterEntity characterEntity,bool hasHpChanged)
    {
        if (!hasHpChanged) return;
        infoOfHpUi ??= new List<CharacterEntity>(2);
        if (infoOfHpUi.Exists(t => t.InstanceIdPy == characterEntity.InstanceIdPy)) return;
        infoOfHpUi.Add(characterEntity);
    }

    public static void CombineHpUiInfo(ref Dictionary<int,CharacterEntity> infoOfHpUi,[CanBeNull] List<CharacterEntity> lowLevelInfoOfHpUi)
    {
        if (lowLevelInfoOfHpUi == null) return;

        infoOfHpUi ??= new Dictionary<int,CharacterEntity>(lowLevelInfoOfHpUi.Count);
        foreach (CharacterEntity characterEntity in lowLevelInfoOfHpUi)
        {
            infoOfHpUi.TryAdd(characterEntity.InstanceIdPy,characterEntity);
        }
    }
}
}