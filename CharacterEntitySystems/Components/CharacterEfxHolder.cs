using System.Collections.Generic;

using LowLevelSystems.EfxSystems;
using LowLevelSystems.SkillSystems.SkillBuffSystems;

using Sirenix.OdinInspector;

using UnityEngine;

namespace LowLevelSystems.CharacterEntitySystems.Components
{
public class CharacterEfxHolder : EfxHolder
{
    [Title("Data")]
    [ShowInInspector]
    private readonly Dictionary<BuffEnum,EfxWrapper> _buffEnum_efxWrapper = new Dictionary<BuffEnum,EfxWrapper>(5);
    public Dictionary<BuffEnum,EfxWrapper> BuffEnum_EfxWrapperPy => this._buffEnum_efxWrapper;

    public CharacterEfxHolder(Transform efxHolderTransform) : base(efxHolderTransform)
    {
    }

    /// <summary>
    /// 添加特效.
    /// </summary>
    [Title("Methods")]
    public bool HasEfxOf(BuffEnum buffEnum)
    {
        return this._buffEnum_efxWrapper.ContainsKey(buffEnum);
    }

    public void RecordBuffEnum(BuffEnum buffEnum)
    {
        this._buffEnum_efxWrapper[buffEnum] = null;
    }

    public void AddEfx(BuffEnum buffEnum,EfxWrapper efxWrapper)
    {
        efxWrapper.SelfTransformPy.SetParent(this._efxHolderTransform);
        this._buffEnum_efxWrapper[buffEnum] = efxWrapper;
    }

    public void RemoveEfxToPool(BuffEnum buffEnum)
    {
        if (!this._buffEnum_efxWrapper.TryGetValue(buffEnum,out EfxWrapper efxWrapper)) return;
        if (efxWrapper == null)
        {
            Debug.LogError($"回收 Efx 时, Efx 为 null. 是否有 0 回合的特效存在? ");
            return;
        }
        efxWrapper.SelfGoPy.SetActive(false);
        efxWrapper.SelfTransformPy.SetParent(_efxCommonParentTransform);
        _efxManager.ReturnEfxToPool(efxWrapper);
        this._buffEnum_efxWrapper.Remove(buffEnum);
    }
}
}