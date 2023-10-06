using System.Collections.Generic;
using System.Linq;

using Common.Utilities;

using LowLevelSystems.CharacterSystems.PcSystems.Components.BagOfEquippedSkillSystems;
using LowLevelSystems.CharacterSystems.PcSystems.Components.BagOfLearnedSkillSystems;
using LowLevelSystems.SkillSystems.Base;
using LowLevelSystems.SkillSystems.Config;

using Sirenix.OdinInspector;

namespace LowLevelSystems.CharacterEntitySystems.PcEntitySystems.Components.SkillDrawPileSystems
{
public class SkillDrawPile
{
    [Title("Data")]
    [ShowInInspector]
    private PcEntity _pcEntity;
    public PcEntity PcEntityPy => this._pcEntity;
    public void SetPcEntity(PcEntity pcEntity)
    {
        this._pcEntity = pcEntity;
    }

    //一场战斗中生成的所有 SkillSugar. 用于统一改变 weight.
    [ShowInInspector]
    private readonly List<SkillSugar> _allSkillSugars = new List<SkillSugar>(40);
    public List<SkillSugar> AllSkillSugarsPy => this._allSkillSugars;
    //需要计算权重的 Sugars.
    [ShowInInspector]
    private readonly List<SkillSugar> _sugarsToWeight = new List<SkillSugar>(40);
    public List<SkillSugar> SugarsToWeightPy => this._sugarsToWeight;
    //预示槽, 凝练槽.
    [ShowInInspector]
    private readonly List<SkillSugar> _predicatedSugars = new List<SkillSugar>(5);
    public List<SkillSugar> PredicatedSugarsPy => this._predicatedSugars;

    public SkillDrawPile(PcEntity pcEntity)
    {
        this._pcEntity = pcEntity;
    }

    [Title("Methods")]
    [ShowInInspector]
    public int CountOfPredicatingSlotsPy => this._pcEntity.PcPy.SkillBarAbstractDataPy.CountOfPredicatingSlotsPy;

    /// <summary>
    /// 进入战斗时, 玩家角色会重置抽技能堆. 
    /// </summary>
    public void ResetDrawPile()
    {
        //功能: 将之前的 SkillSugar 都清理掉.
        this._allSkillSugars.Clear();
        this._sugarsToWeight.Clear();
        this._predicatedSugars.Clear();

        //功能: 已装备的技能背包中, 遍历, 每个格子都生成一个技能, 没有技能的空格子就用使用补充技能, 都添加到 _weightedSugars.
        BagOfEquippedSkill bagOfEquippedSkill = this._pcEntity.PcPy.BagOfEquippedSkillPy;
        foreach (RowOfSkillBag bagRow in bagOfEquippedSkill.RowsPy)
        {
            foreach (CellOfSkillBag bagCell in bagRow.CellsOfSkillBagPy)
            {
                //功能: 没有技能的空格子就使用补充技能, 装了技能就使用指定的.
                SkillMainIdAndQualityEnum skillMainIdAndQualityEnumParam
                    = !bagCell.HasSkillSugarStringPy ? SkillConfigHub.SubstituteSkillMainIdAndQualityEnum : bagCell.SkillMainIdAndQualityEnumPy;
                SkillSugar skillSugar = SkillSugarFactory.GenerateSkillSugar(skillMainIdAndQualityEnumParam,this);
                this._sugarsToWeight.Add(skillSugar);
            }
        }

        //功能: 添加满 凝脉槽. 即 调用几次 添加凝练槽.
        for (int i = 0; i < this.CountOfPredicatingSlotsPy; i++)
        {
            this.AddSugarToPredicatedSlots();
        }
    }

    /// <summary>
    /// 功能: 从 权重堆 中移除一个 Sugar 进凝脉槽.
    /// </summary>
    private void AddSugarToPredicatedSlots()
    {
        //任何原因导致技能糖不够了, 都生成一个替补的.
        if (this._sugarsToWeight.Count <= 0)
        {
            SkillSugar skillSugar = SkillSugarFactory.GenerateSkillSugar(SkillConfigHub.SubstituteSkillMainIdAndQualityEnum,this);
            this._sugarsToWeight.Add(skillSugar);
        }

        //根据权重拿出一个随机的 Sugar, 从权重堆移除并添加到 预示槽.
        int targetIndex = MathUtilities.RandomizeWithWeights(this._sugarsToWeight.Select(t => t.WeightPy).ToList());
        SkillSugar targetSkillSugar = this._sugarsToWeight[targetIndex];
        this._predicatedSugars.Add(targetSkillSugar);
        //功能: 然后从 _weightedSugars 中移除该 index.
        this._sugarsToWeight.RemoveAt(targetIndex);

        //功能: 从 _weightedSugars 中拿出一个到凝练槽时, 会将这一个的权重除以 2, 如果当前的权重已经等于1, 那么就是其他所有的权重乘以2. 
        int weightOfTargetSugar = targetSkillSugar.WeightPy;
        if (weightOfTargetSugar == 1)
        {
            foreach (SkillSugar skillSugar in this._allSkillSugars)
            {
                skillSugar.SetWeight(skillSugar.WeightPy * 2);
            }
        }
        targetSkillSugar.SetWeight(weightOfTargetSugar / 2);
    }

    /// <summary>
    /// 功能: 从凝脉槽中移除一个 Sugar.
    /// </summary>
    public SkillSugar RemoveFromPredicatedSugars()
    {
        //功能: 移除后, 返回该 Int, 并且调用 添加凝脉槽 来补充.
        SkillSugar sugar = this._predicatedSugars[0];
        this._predicatedSugars.RemoveAt(0);
        this.AddSugarToPredicatedSlots();
        return sugar;
    }

    public void ReturnSugarToDrawPile(List<SkillSugar> returnedSugars)
    {
        this._sugarsToWeight.AddRange(returnedSugars);
    }
}
}