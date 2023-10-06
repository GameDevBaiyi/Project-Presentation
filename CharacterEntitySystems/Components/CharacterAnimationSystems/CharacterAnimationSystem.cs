using System;
using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using JetBrains.Annotations;

using LowLevelSystems.CharacterSystems.Components.PropertySystems;

using Sirenix.OdinInspector;

using Spine;
using Spine.Unity;

using UnityEngine;

using Animation = Spine.Animation;

namespace LowLevelSystems.CharacterEntitySystems.Components.CharacterAnimationSystems
{
public class CharacterAnimationSystem
{
    [Title("Config")]
    public const string Idle = "idle";
    public const string IdleInBattle = "battle_idle";
    public const string Run = "run";
    public const string RunInBattle = "battle_run";
    public const string Walk = "move";
    public const string Die = "death";

    //事件帧.
    public const string AttackEfxEvent = "atk";
    public const string TrackEfxEvent = "tra";
    public const string GetAttackedEfxEvent = "def";

    [Title("References")]
    private readonly CharacterEntity _characterEntity;
    public CharacterEntity CharacterEntityPy => this._characterEntity;

    [ShowInInspector]
    private readonly SkeletonAnimation _skeletonAnimation;
    public SkeletonAnimation SkeletonAnimationPy => this._skeletonAnimation;

    public CharacterAnimationSystem(CharacterEntity characterEntity)
    {
        this._characterEntity = characterEntity;
        this._skeletonAnimation = characterEntity.SkeletonAnimationPy;
    }

    public void DoIdleAnime()
    {
        Animation animation = CharacterAnimationSystemDetails.GetIdleAnimation(this);
        if (animation == null) return;

        this._skeletonAnimation.AnimationState.SetAnimation(0,animation,true);
    }

    public void DoRunAnime()
    {
        Animation animation = CharacterAnimationSystemDetails.GetRunAnimation(this);
        if (animation == null) return;

        this._skeletonAnimation.AnimationState.SetAnimation(0,animation,true);
    }

    private bool _isDoingDieAnime;
    public bool IsDoingDieAnimePy => this._isDoingDieAnime;
    public async UniTask DoDieAnimeAsync()
    {
        if (this._isDoingDieAnime)
        {
            Debug.LogError($"当前设计下不应该出现重复播放死亡动画的情况. ");
        }
        this._isDoingDieAnime = true;
        Animation die = this._skeletonAnimation.AnimationState.Data.SkeletonData.FindAnimation(Die);
        float animeDuration;
        if (die == null)
        {
            Debug.LogError($"未找到该角色的: {this._characterEntity.CharacterPy.CharacterEnumPy} 该动画: {Die}");
            animeDuration = 0.5f;
        }
        else
        {
            this._skeletonAnimation.AnimationState.SetAnimation(0,die,false);
            animeDuration = die.Duration;
        }

        await UniTask.Delay(TimeSpan.FromSeconds(animeDuration));
        this._isDoingDieAnime = false;
    }

    [ShowInInspector]
    private bool _isAttacking;
    /// <summary>
    /// 周期是一次攻击动画开始到受击特效帧.
    /// </summary>
    public bool IsAttackingPy => this._isAttacking;
    public void DoAttackAnime(string attackAnimeName,out float attackEfxTime,out float trackEfxTime,
                              out float getAttackedEfxTime)
    {
        Animation attackAnime = this._skeletonAnimation.AnimationState.Data.SkeletonData.FindAnimation(attackAnimeName);
        if (attackAnime == null)
        {
            Debug.LogError($"未找到该角色的: {this._characterEntity.CharacterPy.CharacterEnumPy} 该动画: {attackAnimeName}");
            attackEfxTime = 0.5f;
            trackEfxTime = 0.5f;
            getAttackedEfxTime = 0.5f;
            return;
        }
        this._skeletonAnimation.AnimationState.SetAnimation(0,attackAnime,false);
        Animation idleAnimation = CharacterAnimationSystemDetails.GetIdleAnimation(this);
        if (idleAnimation != null)
        {
            this._skeletonAnimation.AnimationState.Data.SetMix(attackAnime,idleAnimation,0.1f);
            this._skeletonAnimation.AnimationState.AddAnimation(0,idleAnimation,true,0);
        }

        Dictionary<string,float> eventString_time = attackAnime.EventString_TimePy;
        if (!eventString_time.TryGetValue(AttackEfxEvent,out attackEfxTime))
        {
            attackEfxTime = 0.5f;
            Debug.LogError($"该角色的: {this._characterEntity.CharacterPy.CharacterEnumPy} 该攻击动画: {attackAnimeName} 没有 攻击特效事件. ");
        }
        if (!eventString_time.TryGetValue(TrackEfxEvent,out trackEfxTime))
        {
            trackEfxTime = 0.5f;
            Debug.LogError($"该角色的: {this._characterEntity.CharacterPy.CharacterEnumPy} 该攻击动画: {attackAnimeName} 没有 弹道特效事件. ");
        }
        if (!eventString_time.TryGetValue(GetAttackedEfxEvent,out getAttackedEfxTime))
        {
            getAttackedEfxTime = 0.5f;
            Debug.LogError($"该角色的: {this._characterEntity.CharacterPy.CharacterEnumPy} 该攻击动画: {attackAnimeName} 没有 受击特效事件. ");
        }

#pragma warning disable CS4014
        RecordIsAttackingAsync(attackAnime.Duration);
#pragma warning restore CS4014
        async UniTask RecordIsAttackingAsync(float attackAnimeDuration)
        {
            this._isAttacking = true;
            await UniTask.Delay(TimeSpan.FromSeconds(attackAnimeDuration));
            this._isAttacking = false;
        }
    }

    // 闪烁红光的机制.
    private async UniTask FlashingRedLightAsync()
    {
        Skeleton skeleton = this._skeletonAnimation.Skeleton;
        skeleton.SetColor(Color.red);
        await UniTask.Delay(TimeSpan.FromSeconds(0.4f));
        skeleton.SetColor(Color.white);
    }

    /// <summary>
    /// 上面的闪烁红光的 Wrapper. 死亡机制目前一定跟红光相关. 且是红光执行完后.
    /// </summary>
    private async UniTask FlashingRedLightAndCheckDieAsync()
    {
        await this.FlashingRedLightAsync();
        await MechanicsOfDeath.DoDieAnimeAfterFlashingRedLightAsync(this._characterEntity);
    }

    public static void FlashingRedLight([CanBeNull] IEnumerable<CharacterEntity> characterEntitiesToFlashingRedLight)
    {
        if (characterEntitiesToFlashingRedLight == null) return;
        foreach (CharacterEntity characterEntity in characterEntitiesToFlashingRedLight)
        {
#pragma warning disable CS4014
            characterEntity.CharacterAnimationSystemPy.FlashingRedLightAndCheckDieAsync();
#pragma warning restore CS4014
        }
    }
}
}