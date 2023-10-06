using System.Collections.Generic;

using JetBrains.Annotations;

using LowLevelSystems.CharacterEntitySystems;

namespace LowLevelSystems.UISystems.DamagePromptSystems
{
public static class InfoCombinerForDamageUi
{
    // 设计: 1. 一个触发效果保证其 out 出来的 Ui 数据都是 List<List<(Tuple)>>
    // 2. 触发效果往往是多个一起触发. 将上面的数据整合为: List<(CharacterEntity,List<List<(Tuple)>>)>
    // 3. 有时候触发效果会不仅多个一起触发, 还会遍历多个角色. 将上面的数据整合为: Dictionary<int,(CharacterEntity,List<List<(Tuple)>>)>

    public static void CombineDamageUiInfo(ref List<(CharacterEntity,List<List<(DamagePromptSystem.DamageUiTypeEnum DamageUiTypeEnum,bool IsCritical,int Value)>>)> infoOfDamageUi,
                                           CharacterEntity characterEntity,
                                           List<List<(DamagePromptSystem.DamageUiTypeEnum DamageUiTypeEnum,bool IsCritical,int Value)>> damageUiDataGroups)
    {
        infoOfDamageUi ??= new List<(CharacterEntity,List<List<(DamagePromptSystem.DamageUiTypeEnum DamageUiTypeEnum,bool IsCritical,int Value)>>)>(2);
        int index = infoOfDamageUi.FindIndex(t => t.Item1.InstanceIdPy == characterEntity.InstanceIdPy);
        if (index != -1)
        {
            infoOfDamageUi[index].Item2.AddRange(damageUiDataGroups);
        }
        else
        {
            infoOfDamageUi.Add(new(characterEntity,new(damageUiDataGroups)));
        }
    }

    public static void CombineDamageUiInfo(
        ref Dictionary<int,(CharacterEntity,List<List<(DamagePromptSystem.DamageUiTypeEnum DamageUiTypeEnum,bool IsCritical,int Value)>>)> infoOfDamageUi,
        [CanBeNull] List<(CharacterEntity,List<List<(DamagePromptSystem.DamageUiTypeEnum DamageUiTypeEnum,bool IsCritical,int Value)>>)> lowLevelInfoOfDamageUi)
    {
        if (lowLevelInfoOfDamageUi == null) return;

        infoOfDamageUi
            ??= new Dictionary<int,(CharacterEntity,List<List<(DamagePromptSystem.DamageUiTypeEnum DamageUiTypeEnum,bool IsCritical,int Value)>>)>(lowLevelInfoOfDamageUi.Count);
        foreach (var tuple in lowLevelInfoOfDamageUi)
        {
            if (infoOfDamageUi.TryGetValue(tuple.Item1.InstanceIdPy,out var outTuple))
            {
                outTuple.Item2.AddRange(tuple.Item2);
            }
            else
            {
                infoOfDamageUi[tuple.Item1.InstanceIdPy] = tuple;
            }
        }
    }
}
}