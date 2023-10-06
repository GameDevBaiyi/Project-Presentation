using System;
using System.Collections.Generic;

using Common.Template;

using CommonPromptPackage;

using Cysharp.Threading.Tasks;

using FairyGUI;

using HighLevelManagers.AbstractDataManagers;

using LowLevelSystems.Common;
using LowLevelSystems.UISystems.DamagePromptSystems;
using LowLevelSystems.UISystems.WindowSystems;

using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.Rendering.Universal;

// ReSharper disable ConvertToAutoPropertyWithPrivateSetter

namespace LowLevelSystems.UISystems
{
public enum UiLayerEnum
{
    None = 0,
    Effect = 1,
    Hud = 2,
    Popup = 3,
    TopHud = 4,
    Guide = 5,
    Loading = 6,
    Prompt = 7,
}

public static class UILayerEnumExtensions
{
    public static BaseUiWindow BaseUiWindow(this UiLayerEnum uiLayerEnum)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) return null;
#endif
        if (uiLayerEnum == UiLayerEnum.None) return null;

        if (!Details.UiManager.UIWindowHubPy.UiLayerEnum_BaseWindowPy.TryGetValue(uiLayerEnum,out BaseUiWindow uiWindow))
        {
            Debug.LogError($"未找到: {uiLayerEnum} 的 {typeof(BaseUiWindow)}.");
        }
        return uiWindow;
    }
}

public class UiManager : Singleton<UiManager>
{
    //功能: UiPackage 管理器.
    [Title("Data")]
    [ShowInInspector]
    private UiPackageManager _uiPackageManager;
    public UiPackageManager UIPackageManagerPy => this._uiPackageManager;

    //功能: UiWindow.
    [ShowInInspector]
    private UiWindowHub _uiWindowHub;
    public UiWindowHub UIWindowHubPy => this._uiWindowHub;

    //功能: 提示框.
    [ShowInInspector]
    private PromptOnMousePos _promptOnMousePos;
    public PromptOnMousePos PromptOnMousePosPy => this._promptOnMousePos;

    //功能: 技能选择范围显示.
    private const string _addressOfSkillUsingRangeUI = "Sign_UseRange";
    [ShowInInspector]
    private CellUiShower _cellUiShowerForSkillUsingRange;
    public CellUiShower CellUiShowerForSkillUsingRangePy => this._cellUiShowerForSkillUsingRange;

    //功能: 技能生效范围显示.
    [ShowInInspector]
    private const string _addressOfSkillEffectRangeUI = "Sign_EffectRange";
    [ShowInInspector]
    private CellUiShower _cellUiShowerForSkillEffectRange;
    public CellUiShower CellUiShowerForSkillEffectRangePy => this._cellUiShowerForSkillEffectRange;

    //功能: 可移动范围显示.
    private const string _addressOfCellUiForMovableRange = "Sign_BattleMove";
    [ShowInInspector]
    private CellUiShower _cellUiShowerForMovableRange;
    public CellUiShower CellUiShowerForMovableRangePy => this._cellUiShowerForMovableRange;

    private const string _addressOfSpawnTileInBattleGo = "SpawnTileInBattleGo";
    [ShowInInspector]
    private CellUiShower _spawnTileInBattleUiShower;
    public CellUiShower SpawnTileInBattleUiShowerPy => this._spawnTileInBattleUiShower;

    private const string _addressOfInteractionTileGo = "InteractionTileGo";
    [ShowInInspector]
    private CellUiShower _interactionTileUiShower;
    public CellUiShower InteractionTileUiShowerPy => this._interactionTileUiShower;

    private const string _addressOfSpawnTileGo = "SpawnTileGo";
    [ShowInInspector]
    private CellUiShower _spawnTileUiShower;
    public CellUiShower SpawnTileUiShowerPy => this._spawnTileUiShower;
    
    // 战斗中的 "区域 Tile" 显示.
    private const string _addressOfAreaTileGo = "AreaTileGo";
    [ShowInInspector]
    private CellUiShower _areaTileUiShower;
    public CellUiShower AreaTileUiShowerPy => this._areaTileUiShower;

    //功能: 伤害数字显示.
    [ShowInInspector]
    private DamagePromptSystem _damagePromptSystem;
    public DamagePromptSystem DamagePromptSystemPy => this._damagePromptSystem;

    [ShowInInspector]
    private CursorUi _cursorUi;
    public CursorUi CursorUiPy => this._cursorUi;

    //功能: 路径显示.
    [ShowInInspector]
    private PathDrawer _pathDrawer;
    public PathDrawer PathDrawerPy => this._pathDrawer;

    //功能: 对话泡泡.
    [ShowInInspector]
    private DialogueBubblePool _dialogueBubblePool;
    public DialogueBubblePool DialogueBubblePoolPy => this._dialogueBubblePool;

    [Title("Methods")]
    public async UniTask InitializeAsync(Transform gameObjectUIParentTransform,Camera mainCamera)
    {
        //相机设置.
        void InitializeStageCamera()
        {
            //处理相机堆叠.
            Camera stageCamera = StageCamera.main;
            UniversalAdditionalCameraData cameraData = stageCamera.GetUniversalAdditionalCameraData();
            cameraData.renderType = CameraRenderType.Overlay;
            Camera.main.GetUniversalAdditionalCameraData().cameraStack.Add(stageCamera);

            //设置摄像机自动大小.
            StageCamera.main.GetComponent<StageCamera>().constantSize = false;
        }

        //相机设置.
        InitializeStageCamera();
        //设置自适应分辨率.
        GRoot.inst.SetContentScaleFactor(2560,1440,UIContentScaler.ScreenMatchMode.MatchHeight);

        this._uiPackageManager = new UiPackageManager();
        BindHelper.BindAll();
        await this._uiPackageManager.InitializeAsync();
        this._uiWindowHub = new UiWindowHub();

        await CommonPrompt.InitializeAsync();
        this._promptOnMousePos = new PromptOnMousePos(UiLayerEnum.Prompt.BaseUiWindow().SelfGComPy);
        HierarchyManager hierarchyManager = HierarchyManager.InstancePy;
        this._cellUiShowerForSkillUsingRange = new CellUiShower(hierarchyManager.CellsForSkillUsingRangePy,_addressOfSkillUsingRangeUI);
        this._cellUiShowerForSkillEffectRange = new CellUiShower(hierarchyManager.CellsForSkillEffectRangePy,_addressOfSkillEffectRangeUI);
        this._cellUiShowerForMovableRange = new CellUiShower(hierarchyManager.CellsForMovableRangePy,_addressOfCellUiForMovableRange);
        this._areaTileUiShower = new CellUiShower(hierarchyManager.AreaTileUisPy,_addressOfAreaTileGo);
        this._spawnTileInBattleUiShower = new CellUiShower(hierarchyManager.SpawnTileInBattleUisPy,_addressOfSpawnTileInBattleGo);
        this._interactionTileUiShower = new CellUiShower(hierarchyManager.InteractionTileUisPy,_addressOfInteractionTileGo);
        this._spawnTileUiShower = new CellUiShower(hierarchyManager.SpawnTileUisPy,_addressOfSpawnTileGo);
        this._damagePromptSystem = new DamagePromptSystem(UiLayerEnum.Effect.BaseUiWindow().SelfGComPy);
        this._cursorUi = new CursorUi();
        await this._cursorUi.InitializeAsync(gameObjectUIParentTransform);

        this._pathDrawer = new PathDrawer();
        await this._pathDrawer.InitializeAsync(gameObjectUIParentTransform);

        this._dialogueBubblePool = new DialogueBubblePool(UiLayerEnum.Effect.BaseUiWindow().SelfGComPy,mainCamera,GRoot._inst);
        this._popupType_popup = new Dictionary<Type,GObject>(50);
    }

    public void CloseUis()
    {
        this._uiWindowHub.CloseAllButLoadingsAndPromptWindows();
        this._pathDrawer.HidePath();
        this._interactionTileUiShower.Hide();
        this._spawnTileUiShower.Hide();
    }

    //未优化的组件.
    //功能: 显示 Popup.
    [ShowInInspector]
    private Dictionary<Type,GObject> _popupType_popup;

    public async UniTask<T> ShowPopup<T>(GObject target,string packageName) where T : GObject
    {
        //如果没有, 就生成一个.
        Type type = typeof(T);
        if (!this._popupType_popup.TryGetValue(type,out GObject popup))
        {
            await this._uiPackageManager.AddPackageAsync(packageName);
            string resourceName = type.Name.Substring(3);

            //创建 Popup 并记录.
            popup = UIPackage.CreateObject(packageName,resourceName);
            if (popup == null)
            {
                Debug.LogError($"创建 UI 失败: {packageName},{resourceName}");
                return null;
            }

            this._popupType_popup[type] = popup;
        }

        GRoot.inst.ShowPopup(popup,target);

        return (T)popup;
    }

    public void HidePopup(GObject popup)
    {
        popup.visible = false;
    }
}
}