using System;
using System.Collections.Generic;
using System.Linq;

using Cysharp.Threading.Tasks;

using FairyGUI;

using LowLevelSystems.CharacterEntitySystems.NpcEntitySystems;
using LowLevelSystems.CharacterEntitySystems.PcEntitySystems;
using LowLevelSystems.CharacterSystems.NpcSystems.Components.InteractionsSystems;
using LowLevelSystems.CharacterSystems.NpcSystems.Components.InteractionsSystems.RestSystems;
using LowLevelSystems.CharacterSystems.NpcSystems.Components.InteractionsSystems.StealSystems;
using LowLevelSystems.CharacterSystems.NpcSystems.Components.InteractionsSystems.TalkSystems;
using LowLevelSystems.Common;
using LowLevelSystems.HeronTeamSystems.Components;
using LowLevelSystems.LocalizationSystems;
using LowLevelSystems.MissionSystems.Conditions;
using LowLevelSystems.SkillSystems.SkillBuffSystems;
using LowLevelSystems.UISystems;

using Scripts.FGUIPartialScripts.NpcInteractPackage;

using Sirenix.OdinInspector;

using UICommon;

using UnityEngine;
using UnityEngine.Rendering;

using Object = System.Object;

#pragma warning disable CS4014

namespace LowLevelSystems.CharacterEntitySystems.Components
{
public class CharacterPanelController
{
    [Title("References")]
    private readonly CharacterEntity _characterEntity;
    [ShowInInspector]
    private UIPanel _uiPanel;
    [ShowInInspector]
    private GameObject _uiPanelGo;
    [ShowInInspector]
    private UI_GCom_CharacterPanel _gComponent;
    [ShowInInspector]
    private GImage _directionGImage;
    [ShowInInspector]
    private UI_GCom_CharacterInteraction _characterInteraction;
    [ShowInInspector]
    private SortingGroup _sortingGroup;
    [ShowInInspector]
    private SortingGroup _npcSortingGroup;
    [ShowInInspector]
    private GList _buffs;
    [ShowInInspector]
    private UI_GCom_Introduction _name;

    private int _index;
    private List<List<Interaction>> _pages = new List<List<Interaction>>();

    public CharacterPanelController(CharacterEntity characterEntity)
    {
        this._characterEntity = characterEntity;
        this._uiPanel = characterEntity.UiPanelPy;
        this._uiPanelGo = this._uiPanel.gameObject;
        this._gComponent = this._uiPanel.ui as UI_GCom_CharacterPanel;
        // ReSharper disable once PossibleNullReferenceException
        this._directionGImage = this._gComponent.GImage_Direction;
        this._characterInteraction = this._gComponent.CharacterInteraction;
        this._buffs = this._gComponent.GList_Buffs;
        this._name = this._gComponent.GCom_Name;
        this._sortingGroup = this._uiPanel.GetComponent<SortingGroup>();

        //Event Add.
        CursorUi.InteractableCursorHidden += this.HideNameAndTipUIVisible;
        CursorUi.InteractableCursorShowed += this.RefreshName;
    }

#region UI自适应
    private const float _offsetTimesOfHpWidth = 40;
    public async UniTask SelfAdaptionAsync()
    {
        MeshRenderer meshRenderer = this._characterEntity.MeshRendererPy;
        await UniTask.WaitUntil(() => meshRenderer != null && meshRenderer.localBounds.size != Vector3.zero);
        Bounds localBounds = meshRenderer.localBounds;

        //标准长度
        float standardWidth = 2;
        //求出人物的宽长
        float width = localBounds.max.x - localBounds.min.x;
        //根据标准算出倍数
        float times = width / standardWidth;
        //根据倍数改变血条的宽度
        float uiWidth = 287 + (times - 1) * _offsetTimesOfHpWidth;
        this._gComponent.GCom_Slider.SetWidthAndDoAdaption(uiWidth);

        //标准高度
        float standardHeight = 3.75f;
        //得到人物的高长
        float height = localBounds.max.y - localBounds.min.y;
        //根据标准算出倍数
        float times_1 = height / standardHeight;
        //根据倍数改变互动UI的高度
        this._characterInteraction.SetXY(this._characterInteraction.x,60f / times_1);
    }
#endregion

#region Directions Control
    public void ChangeDirectionUIVisible(bool isVisible)
    {
        this._directionGImage.visible = isVisible;
    }

    public void RefreshDirection()
    {
        this._directionGImage.rotation = Details.FGUIDirectionAngles[this._characterEntity.CharacterPy.CoordSystemPy.DirectionIndexPy];
    }

    public void PlayTransition()
    {
        if (!this._directionGImage.visible) return;
        this._sortingGroup.sortingOrder = 2;
        //方向选择动效播放
        this._gComponent.Transition_SelectDirection.Play(-1,0,0,0.5f,() => { });
        this._gComponent.GImage_Direction.SetScale(1.2f,1.2f);
    }

    public void RefreshCloseTransition()
    {
        this._sortingGroup.sortingOrder = 1;
        this._gComponent.Transition_SelectDirection.Play(1,0,0,0,() => { });
        this._gComponent.GImage_Direction.SetScale(1,1);
    }
#endregion

#region Interaction Control
    private async UniTask CheckToCloseInteractionAsync()
    {
        while (true)
        {
            if (!this._characterInteraction.visible) break;

            await UniTask.NextFrame();

            if (!Input.GetKeyDown(KeyCode.Mouse0)
             && !Input.GetKeyDown(KeyCode.Mouse1)) continue;
            if (this._characterInteraction == GRoot._inst.touchTarget
             || this._characterInteraction.IsAncestorOf(GRoot._inst.touchTarget)) continue;

            this.HideInteraction();
        }
    }

    public void ShowInteractionsUi(PcEntity pcEntity)
    {
        if (this._characterEntity is not NpcEntity npcEntity)
        {
            Debug.LogError($"只有 Npc 才有交互功能. ");
            return;
        }

        //Cache.
        this._characterInteraction.visible = true;
        this._characterInteraction.MoveToRight.Play();
        Dictionary<InteractionEnum,Interaction> extraInteractionsExtraInteractionEnumInteraction = npcEntity.NpcPy.InteractionsPy.InteractionEnum_InteractionPy;
        this._npcSortingGroup = npcEntity.UiPanelPy.GetComponent<SortingGroup>();
        this._npcSortingGroup.sortingOrder = 2;

        //UIShow
        this._index = 0;
        this._pages.Clear();
        List<Interaction> page = new List<Interaction>();
        //第一次遍历将所有完整的页面(交互数量大于5的)添加进入
        foreach (KeyValuePair<InteractionEnum,Interaction> interactionEnum_interaction in extraInteractionsExtraInteractionEnumInteraction)
        {
            Interaction interaction = interactionEnum_interaction.Value;
            page.Add(interaction);
            this._index++;
            if (this._index > 5)
            {
                this._index -= 5;
                this._pages.Add(page);
                page.Clear();
            }
        }
        //将不足一页的一组添加进入
        this._pages.Add(page);
        //显示第一页交互功能的UI
        this.ShowOnePageInteractionUI(0,pcEntity,npcEntity);
        //翻页
        UI_GButton_ChatSelect pageButton = this._characterInteraction.GButton_Page;
        if (this._pages.Count > 1)
        {
            pageButton.visible = true;
        }
        else
        {
            pageButton.visible = false;
        }
        pageButton.GLoader_IconFd.url = UiConst.GetLoaderUrl(UiConst.TexturesPackage,"NPC_Page");
        // pageButton.GTextField_TextFd.text = "翻页";
        int currentPage = 0;
        pageButton.onClick.Set(() =>
                               {
                                   if (currentPage < this._pages.Count - 1)
                                   {
                                       currentPage++;
                                   }
                                   else
                                   {
                                       currentPage = 0;
                                   }
                                   this.ShowOnePageInteractionUI(currentPage,pcEntity,npcEntity);
                               });

        this.CheckToCloseInteractionAsync();
    }

    //某一页交互功能的UI
    private void ShowOnePageInteractionUI(int pageNumber,PcEntity pcEntity,NpcEntity npcEntity)
    {
        List<Interaction> interactions = this._pages[pageNumber];
        //第一次遍历初始化所有交互按钮
        for (int i = 0; i < 5; i++)
        {
            UI_GButton_ChatSelect interactionUI = (UI_GButton_ChatSelect)this._characterInteraction.GetChild("GButton_Option_" + (i + 1));
            interactionUI.alpha = 0;
            interactionUI.touchable = false;
            interactionUI.GLoader_IconFd.url = null;
        }
        //第二次遍历对应正确显示
        for (int i = 0; i < interactions.Count; i++)
        {
            UI_GButton_ChatSelect interactionUI = (UI_GButton_ChatSelect)this._characterInteraction.GetChild("GButton_Option_" + (i + 1));
            interactionUI.alpha = 1;
            interactionUI.touchable = true;
            interactionUI.GLoader_IconFd.url = UiConst.GetLoaderUrl(UiConst.TexturesPackage,"NPC_" + interactions[i].InteractionEnumPy);
            switch (interactions[i].InteractionEnumPy)
            {
            case InteractionEnum.Talk:
                //将控制器切换按钮类型
                interactionUI.Controller_Type.selectedIndex = 0;
                interactionUI.onRollOver.Set(() => { UI_GCom_NpcIntroduction.Show(interactionUI,new TextId(10300011)); });
                interactionUI.onClick.Set(() =>
                                          {
                                              DetailsOfTalk.Talk(pcEntity,npcEntity);
                                              this.HideInteraction();
                                          });
                break;

            case InteractionEnum.Trade:
                //将控制器切换按钮类型
                interactionUI.Controller_Type.selectedIndex = 1;
                interactionUI.onRollOver.Set(() => { UI_GCom_NpcIntroduction.Show(interactionUI,new TextId(1030002)); });
                interactionUI.onClick.Set(() => NpcInteractionsPopup.OpenAndSteal(InteractionEnum.Trade,npcEntity));
                break;

            case InteractionEnum.Steal:
                //将控制器切换按钮类型
                interactionUI.Controller_Type.selectedIndex = 0;
                interactionUI.onRollOver.Set(() => { UI_GCom_NpcIntroduction.Show(interactionUI,new TextId(10300013)); });
                interactionUI.onClick.Set(() =>
                                          {
                                              NpcInteractionsPopup.OpenAndSteal(InteractionEnum.Steal,npcEntity);
                                              if (!pcEntity.PcPy.HasJoinedPy)
                                              {
                                                  this.HideInteraction();
                                              }
                                          });
                break;

            case InteractionEnum.Plot:
                //将控制器切换按钮类型
                interactionUI.Controller_Type.selectedIndex = 2;
                Plot plot = (Plot)interactions[i];
                List<(Vector3Int,TextId)> plotMissionConditionsKeyAndTextId = plot.MissionConditionsKeyAndTextIdPy;

                List<string> selectNames;
                List<Action> actions;
                interactionUI.onRollOver.Set(() => { UI_GCom_NpcIntroduction.Show(interactionUI,new TextId(1030001)); });
                interactionUI.onClick.Set(() =>
                                          {
                                              selectNames = new List<string>();
                                              actions = new List<Action>();
                                              foreach ((Vector3Int,TextId) vector3Int_textId in plotMissionConditionsKeyAndTextId)
                                              {
                                                  TextId textId = vector3Int_textId.Item2;
                                                  selectNames.Add(textId.TextPy);
                                                  actions.Add(Action);
                                                  void Action()
                                                  {
                                                      DetailsOfMissionConditions.InteractWithNpcOnPlot(vector3Int_textId.Item1);
                                                  }
                                              }
                                              void SelectEvent(EventContext context)
                                              {
                                                  GButton select = (GButton)context.data;
                                                  int childIndex = select.parent.GetChildIndex(select);
                                                  actions.ElementAtOrDefault(childIndex)?.Invoke();
                                                  UI_GCom_SelectPanel.Hide();
                                              }
                                              UI_GCom_SelectPanel.Show(interactionUI.GGraph_placeHolder,selectNames,SelectEvent);
                                          });

                break;

            case InteractionEnum.Rest:
                //将控制器切换按钮类型
                interactionUI.Controller_Type.selectedIndex = 1;
                interactionUI.onRollOver.Set(() => { UI_GCom_NpcIntroduction.Show(interactionUI,new TextId(1030006)); });
                interactionUI.onClick.Set(() =>
                                          {
                                              if (!DetailsOfWallet.HasEnoughMoney(Details.SettingsSo.InnCost))
                                              {
                                                  UiManager.InstancePy.PromptOnMousePosPy.Show(new TextId(1000020).TextPy);
                                                  return;
                                              }
                                              DetailsOfRest.Rest(pcEntity);
                                              this.HideInteraction();
                                          });
                break;
            case InteractionEnum.CurrencyExchange:
                //将控制器切换按钮类型
                interactionUI.Controller_Type.selectedIndex = 1;
                interactionUI.onRollOver.Set(() => { UI_GCom_NpcIntroduction.Show(interactionUI,new TextId(1030007)); });
                interactionUI.onClick.Set(() => NpcInteractionsPopup.OpenAndSteal(InteractionEnum.CurrencyExchange,npcEntity));
                break;
            }
        }
    }

    public void HideInteraction()
    {
        if (!this._characterInteraction.visible) return;
        this._characterInteraction.visible = false;

        this._characterInteraction.GButton_Option_1.onClick.Clear();
        this._characterInteraction.GButton_Option_2.onClick.Clear();
        this._characterInteraction.GButton_Option_3.onClick.Clear();
        this._characterInteraction.GButton_Option_4.onClick.Clear();
        this._characterInteraction.GButton_Option_5.onClick.Clear();

        if (!Object.ReferenceEquals(this._npcSortingGroup,null))
        {
            this._npcSortingGroup.sortingOrder = 1;
        }
    }
#endregion

#region Buffs Control
    public void ChangeBuffsUIVisible(bool isVisible)
    {
        this._buffs.visible = isVisible;
    }

    public void RefreshBuffs()
    {
        this._buffs.RemoveChildrenToPool();
        foreach (KeyValuePair<BuffEnum,List<Buff>> buffEnum_buffs in this._characterEntity.BuffPoolPy.BuffEnum_BuffsPy)
        {
            BuffEnum buffEnum = buffEnum_buffs.Key;
            List<Buff> buffs = buffEnum_buffs.Value;

            if (buffs.Count == 0) continue;
            if (buffEnum.BuffConfig().IsTakingMaxValuePy)
            {
                Buff buff = buffs[0];
                UI_GCom_BuffPanel buffPanel = (UI_GCom_BuffPanel)this._gComponent.GList_Buffs.AddItemFromPool();
                GButton buffUI = buffPanel.GCom_Buff;
                buffUI.icon = UiConst.GetLoaderUrl(UiConst.ResIcon,buff.BuffEnumPy.ToString());
                buffUI.title = buff.NumberOfRemainingRoundsPy.ToString();
                buffUI.onRollOver.Set(() => { UI_GCom_NameAndDescribe.Show(buffUI,buff.BuffEnumPy.BuffConfig()); });
                // Debug.Log("取最大");
            }
            else
            {
                foreach (Buff buff in buffs)
                {
                    UI_GCom_BuffPanel buffPanel = (UI_GCom_BuffPanel)this._gComponent.GList_Buffs.AddItemFromPool();
                    GButton buffUI = buffPanel.GCom_Buff;
                    buffUI.icon = UiConst.GetLoaderUrl(UiConst.ResIcon,buff.BuffEnumPy.ToString());
                    buffUI.title = buff.NumberOfRemainingRoundsPy.ToString();
                    buffUI.onRollOver.Set(() => { UI_GCom_NameAndDescribe.Show(buffUI,buff.BuffEnumPy.BuffConfig()); });
                    // Debug.Log("取全部");
                }
            }
        }
    }
#endregion

#region NameAndTipControl
    private void HideNameAndTipUIVisible()
    {
        this._name.visible = false;
    }
    private void RefreshName(NpcEntity npcEntity)
    {
        if (this._characterEntity != npcEntity) return;
        this._name.visible = true;
        this._name.GTextField_Name.text = npcEntity.NpcPy.NamePy;
    }
#endregion
}
}