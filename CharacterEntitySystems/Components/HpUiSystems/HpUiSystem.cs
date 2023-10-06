using System.Collections.Generic;

using FairyGUI;

using JetBrains.Annotations;

using LowLevelSystems.CharacterSystems;
using LowLevelSystems.CharacterSystems.Components.PropertySystems;

using Sirenix.OdinInspector;

using UICommon;

using UnityEngine;

namespace LowLevelSystems.CharacterEntitySystems.Components.HpUiSystems
{
public class HpUiSystem
{
    [Title("Config")]
    private const float _hpAnimeDuration = 1f;

    [Title("References")]
    [ShowInInspector]
    private readonly CharacterEntity _characterEntity;
    [ShowInInspector]
    private readonly UI_GCom_ProgressBar_Slider _progressBarSlider;
    [ShowInInspector]
    private readonly UI_GCom_Battle_Hp _hpBar;
    [ShowInInspector]
    private readonly UI_GCom_Battle_BodyVeinHp _bodyVeinHpBar;
    [ShowInInspector]
    private readonly UI_GCom_Battle_SpiritVeinHp _spiritVeinHpBar;

    public HpUiSystem(CharacterEntity characterEntity)
    {
        this._characterEntity = characterEntity;
        UI_GCom_CharacterPanel uiGComCharacterPanel = characterEntity.UiPanelPy.ui as UI_GCom_CharacterPanel;
        // ReSharper disable once PossibleNullReferenceException
        this._progressBarSlider = uiGComCharacterPanel.GCom_Slider;
        this._hpBar = uiGComCharacterPanel.GCom_Slider.GCom_Hp;
        this._bodyVeinHpBar = uiGComCharacterPanel.GCom_Slider.GCom_BodyVein;
        this._spiritVeinHpBar = uiGComCharacterPanel.GCom_Slider.GCom_SpiritVein;
    }

    [Title("Methods")]
    public void ChangeHpVisible(bool isVisible)
    {
        this._progressBarSlider.visible = isVisible;
    }

    // 机制1. 立刻刷新.
    private void RefreshHpAndBackGroundBarInstantly(GProgressBar hpBar,GProgressBar backgroundBar,int maxHp,
                                                    int currentHp)
    {
        hpBar.max = maxHp;
        hpBar.value = currentHp;
        backgroundBar.max = maxHp;
        backgroundBar.value = currentHp;
    }

    private void RefreshHpInstantly(int maxHp,int currentHp)
    {
        if (maxHp <= 0)
        {
            this._hpBar.visible = false;
        }
        else
        {
            this._hpBar.visible = true;
            this.RefreshHpAndBackGroundBarInstantly(this._hpBar.ProgressBar_Hp,this._hpBar.ProgressBar_HpBackground,maxHp,currentHp);
        }
    }

    private void RefreshBodyVeinHpInstantly(int maxBodyVeinHp,int currentBodyVeinHp)
    {
        if (maxBodyVeinHp <= 0)
        {
            this._bodyVeinHpBar.visible = false;
        }
        else
        {
            this._bodyVeinHpBar.visible = true;
            this.RefreshHpAndBackGroundBarInstantly(this._bodyVeinHpBar.ProgressBar_BodyVein,this._bodyVeinHpBar.ProgressBar_BodyVeinBackground,maxBodyVeinHp,currentBodyVeinHp);
        }
    }

    private void RefreshSpiritVeinHpInstantly(int maxSpiritVeinHp,int currentSpiritVeinHp)
    {
        if (maxSpiritVeinHp <= 0)
        {
            this._spiritVeinHpBar.visible = false;
        }
        else
        {
            this._spiritVeinHpBar.visible = true;
            this.RefreshHpAndBackGroundBarInstantly(this._spiritVeinHpBar.ProgressBar_SpiritVein,this._spiritVeinHpBar.ProgressBar_SpiritVeinBackground,maxSpiritVeinHp,
                                                    currentSpiritVeinHp);
        }
    }

    public void RefreshAllHpInstantly()
    {
        Character character = this._characterEntity.CharacterPy;
        PropertySystem propertySystem = character.PropertySystemPy;

        this.RefreshHpInstantly((int)propertySystem[PropertyEnum.MaxHP],(int)propertySystem.CurrentHpPy);
        this.RefreshBodyVeinHpInstantly((int)propertySystem[PropertyEnum.MaxBodyVeinHp],(int)propertySystem.CurrentBodyVeinHpPy);
        this.RefreshSpiritVeinHpInstantly((int)propertySystem[PropertyEnum.MaxSpiritVeinHp],(int)propertySystem.CurrentSpiritVeinHpPy);
    }

    // 机制2. 刷新但有 动画.
    private void RefreshHpAndBackGroundBarWithAnime(GProgressBar hpBar,GProgressBar backgroundBar,int maxHp,
                                                    int currentHp,float duration)
    {
        hpBar.max = maxHp;
        hpBar.value = currentHp;

        backgroundBar.max = maxHp;
        backgroundBar.TweenValue(currentHp,duration);
    }

    private void RefreshHpWithAnime(int maxHp,int currentHp)
    {
        if (maxHp <= 0)
        {
            this._hpBar.visible = false;
        }
        else
        {
            this._hpBar.visible = true;
            float duration = _hpAnimeDuration * (Mathf.Abs((float)this._hpBar.ProgressBar_Hp.value - currentHp) / maxHp);
            this.RefreshHpAndBackGroundBarWithAnime(this._hpBar.ProgressBar_Hp,this._hpBar.ProgressBar_HpBackground,maxHp,currentHp,duration);
        }
    }

    private void RefreshBodyVeinHpWithAnime(int maxBodyVeinHp,int currentBodyVeinHp)
    {
        if (maxBodyVeinHp <= 0)
        {
            this._bodyVeinHpBar.visible = false;
        }
        else
        {
            this._bodyVeinHpBar.visible = true;
            float duration = _hpAnimeDuration / 2f * (Mathf.Abs((float)this._bodyVeinHpBar.ProgressBar_BodyVein.value - currentBodyVeinHp) / maxBodyVeinHp);
            this.RefreshHpAndBackGroundBarWithAnime(this._bodyVeinHpBar.ProgressBar_BodyVein,this._bodyVeinHpBar.ProgressBar_BodyVeinBackground,maxBodyVeinHp,currentBodyVeinHp,
                                                    duration);
        }
    }

    private void RefreshSpiritVeinHpWithAnime(int maxSpiritVeinHp,int currentSpiritVeinHp)
    {
        if (maxSpiritVeinHp <= 0)
        {
            this._spiritVeinHpBar.visible = false;
        }
        else
        {
            this._spiritVeinHpBar.visible = true;
            float duration = _hpAnimeDuration / 2f * (Mathf.Abs((float)this._spiritVeinHpBar.ProgressBar_SpiritVein.value - currentSpiritVeinHp) / maxSpiritVeinHp);
            this.RefreshHpAndBackGroundBarWithAnime(this._spiritVeinHpBar.ProgressBar_SpiritVein,this._spiritVeinHpBar.ProgressBar_SpiritVeinBackground,maxSpiritVeinHp,
                                                    currentSpiritVeinHp,duration);
        }
    }

    private void RefreshAllHpWithAnime()
    {
        Character character = this._characterEntity.CharacterPy;
        PropertySystem propertySystem = character.PropertySystemPy;

        this.RefreshHpWithAnime((int)propertySystem[PropertyEnum.MaxHP],(int)propertySystem.CurrentHpPy);
        this.RefreshBodyVeinHpWithAnime((int)propertySystem[PropertyEnum.MaxBodyVeinHp],(int)propertySystem.CurrentBodyVeinHpPy);
        this.RefreshSpiritVeinHpWithAnime((int)propertySystem[PropertyEnum.MaxSpiritVeinHp],(int)propertySystem.CurrentSpiritVeinHpPy);
    }

    public static void RefreshUi([CanBeNull] IEnumerable<CharacterEntity> characterEntitiesToRefreshHpUi)
    {
        if (characterEntitiesToRefreshHpUi == null) return;
        foreach (CharacterEntity characterEntity in characterEntitiesToRefreshHpUi)
        {
            characterEntity.HpUiSystemPy.RefreshAllHpWithAnime();
        }
    }
}
}