using System.Collections.Generic;

using Common.DataTypes;
using Common.Utilities;

using CommonPromptPackage;

using Cysharp.Threading.Tasks;

using FairyGUI;

using JetBrains.Annotations;

using LowLevelSystems.CharacterEntitySystems;
using LowLevelSystems.Common;

using Sirenix.OdinInspector;

using UnityEngine;

namespace LowLevelSystems.UISystems.DamagePromptSystems
{
public class DamagePromptSystem
{
    public enum DamageUiTypeEnum
    {
        None,
        HpDamage = 1,
        BodyVeinHpDamage = 3,
        SpiritVeinHpDamage = 5,
        HpHeal = 0,
        BodyVeinHpHeal = 2,
        SpiritVeinHpHeal = 4,
        Miss = 6,
    }

    [Title("Config")]
    private const int _damageNumberInterval = 200; //单位 ms.

    [Title("References")]
    private readonly GComponent _parentGCom;

    [Title("Data")]
    [ShowInInspector]
    private ObjectPool<UI_DamagePrompt> _damagePromptUIPool;
    public ObjectPool<UI_DamagePrompt> DamagePromptUIPoolPy => this._damagePromptUIPool;

    public DamagePromptSystem(GComponent parentGCom)
    {
        this._parentGCom = parentGCom;
        this._damagePromptUIPool = new ObjectPool<UI_DamagePrompt>(50,this.GenerateDamagePromptUI);
    }

    /// <summary>
    /// 机制1. 第一次生成的伤害数字, 会给其添加 Parent. 其他属性不处理.
    /// </summary>
    [Title("Methods")]
    private UI_DamagePrompt GenerateDamagePromptUI()
    {
        UI_DamagePrompt damagePromptUI = UI_DamagePrompt.CreateInstance();
        this._parentGCom.AddChild(damagePromptUI);
        return damagePromptUI;
    }

    //机制2. 单个 UI 的播放.
    /// <summary>
    /// 做单个 UI 的动画.
    /// </summary>
    private async UniTask DoAnimeAsync(UI_DamagePrompt uiDamagePrompt)
    {
        uiDamagePrompt.t0.Play(1,0f,0f,0f,() => { });
        uiDamagePrompt.t0.Play();

        while (true)
        {
            if (!uiDamagePrompt.t0.playing) break;

            await UniTask.NextFrame();
        }

        uiDamagePrompt.t0.Stop();
    }

    /// <summary>
    /// 做完动画隐藏并返回 Pool 中. 
    /// </summary>
    private async UniTask DoAnimeAndReturnToPoolAsync(UI_DamagePrompt uiDamagePrompt)
    {
        await this.DoAnimeAsync(uiDamagePrompt);
        uiDamagePrompt.visible = false;
        this._damagePromptUIPool.ReturnItemToPool(uiDamagePrompt);
    }

    /// <summary>
    /// 播放一组 UIs. 以 DealDamage 为例, 多段伤害, 每一段都可能产生多个 UIs. 所以称之为一组. 
    /// 这些 UIs 的位置会出现在以 centerByFguiCoord 为中心, 以 _radiusByScreenCoord 为半径确定一个位置.
    /// 一组 UI 的播放时长为: _damageNumberInterval. 无视动画时间.
    /// </summary>
    private async UniTask PlayUiGroupAsync(List<(DamageUiTypeEnum DamageUiTypeEnum,bool IsCritical,int Value)> tuples,Vector2 centerByFguiCoord)
    {
        foreach ((DamageUiTypeEnum DamageUiTypeEnum,bool IsCritical,int Value) tuple in tuples)
        {
            UI_DamagePrompt damagePromptUi = this._damagePromptUIPool.GetItemFromPool();
            damagePromptUi.Controller_DamageType.selectedIndex = (int)tuple.DamageUiTypeEnum;
            damagePromptUi.Controller_IsCrit.selectedIndex = tuple.IsCritical ? 1 : 0;
            damagePromptUi.GTextField_Number.text = tuple.DamageUiTypeEnum == DamageUiTypeEnum.Miss ? "miss" : tuple.Value.ToString();
            Vector2 fguiPos = centerByFguiCoord + Random.insideUnitCircle * Details.SettingsSo.RadiusByScreenCoord;
            damagePromptUi.SetXY(fguiPos.x,fguiPos.y);
            damagePromptUi.visible = true;

            //做动画不要 await, 并排播放即可.
#pragma warning disable CS4014
            this.DoAnimeAndReturnToPoolAsync(damagePromptUi);
#pragma warning restore CS4014
        }
        await UniTask.Delay(_damageNumberInterval);
    }

    /// <summary>
    /// 播放多组 UI Groups.  
    /// </summary>
    private async UniTask PlayUiGroupsAsync(List<List<(DamageUiTypeEnum DamageUiTypeEnum,bool IsCritical,int Value)>> tuplesList,Vector3 centerByWorldCoord)
    {
        Vector2 centerByFguiCoord = await FGUIUtilities.WorldPosToFGUI(centerByWorldCoord);
        foreach (List<(DamageUiTypeEnum DamageUiTypeEnum,bool IsCritical,int Value)> tuples in tuplesList)
        {
            await this.PlayUiGroupAsync(tuples,centerByFguiCoord);
        }
    }

    // Wrappers. 上游一般是 触发效果 之类的, 执行后会筹集出 DamageUi 需要的数据.
    public void PlayDamageUi([CanBeNull] IEnumerable<(CharacterEntity,List<List<(DamageUiTypeEnum DamageUiTypeEnum,bool IsCritical,int Value)>>)> tupleListForDamageUi)
    {
        if (tupleListForDamageUi == null) return;
        foreach ((CharacterEntity,List<List<(DamageUiTypeEnum DamageUiTypeEnum,bool IsCritical,int Value)>>) tuple in tupleListForDamageUi)
        {
#pragma warning disable CS4014
            this.PlayUiGroupsAsync(tuple.Item2,tuple.Item1.HeadPosPy);
#pragma warning restore CS4014
        }
    }
}
}