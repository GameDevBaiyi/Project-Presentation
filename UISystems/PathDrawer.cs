using System;
using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using Sirenix.OdinInspector;

using TMPro;

using UnityEngine;
using UnityEngine.AddressableAssets;

namespace LowLevelSystems.UISystems
{
public class PathDrawer
{
    [Title("Config")]
    private const string _linePrefabAddress = "LineDrawer";
    private const string _moveCostTextPrefabAddress = "MoveCostText";
    private const string _normalColorHtml = "#426976";
    private const string _specialColorHtml = "#FF0006";

    [Title("Data")]
    //画线.
    [ShowInInspector]
    private GameObject _lineDrawer;
    [ShowInInspector]
    private LineRenderer _lineRenderer;

    //消耗显示.
    [ShowInInspector]
    private GameObject _moveCostText;
    [ShowInInspector]
    private Transform _moveCostTransform;
    [ShowInInspector]
    private TextMeshPro _textMeshPro;

    [ShowInInspector]
    private Color _normalColor;
    private Color _specialColor;

    public async UniTask InitializeAsync(Transform parentTransform)
    {
        this._lineDrawer = await Addressables.InstantiateAsync(_linePrefabAddress,parentTransform);
        this._lineRenderer = this._lineDrawer.GetComponent<LineRenderer>();
        this._moveCostText = await Addressables.InstantiateAsync(_moveCostTextPrefabAddress,parentTransform);
        this._moveCostTransform = this._moveCostText.transform;
        this._textMeshPro = this._moveCostText.GetComponentInChildren<TextMeshPro>();

        ColorUtility.TryParseHtmlString(_normalColorHtml,out this._normalColor);
        ColorUtility.TryParseHtmlString(_specialColorHtml,out this._specialColor);
    }

    public void ShowLine(List<Vector3> worldPath)
    {
        this._lineRenderer.enabled = true;
        this._lineRenderer.positionCount = worldPath.Count;
        Vector3[] linePoints = worldPath.ToArray();
        Array.Reverse(linePoints);
        this._lineRenderer.SetPositions(linePoints);
    }
    public void HideLine()
    {
        this._lineRenderer.enabled = false;
    }
    public void ShowCircle(int numberToShow,bool hasEnoughValue,Vector3 worldPos)
    {
        this._moveCostText.SetActive(true);
        this._textMeshPro.text = numberToShow.ToString();
        this._textMeshPro.color = hasEnoughValue ? this._normalColor : this._specialColor;
        this._moveCostTransform.position = worldPos;
    }
    public void HideCircle()
    {
        this._moveCostText.SetActive(false);
    }

    public void HidePath()
    {
        this._lineRenderer.enabled = false;
        this._moveCostText.SetActive(false);
    }

    public void ChangeLineWaypointCount(int waypointCount)
    {
        this._lineRenderer.positionCount = waypointCount;
    }
}
}