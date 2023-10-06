using System;

using LowLevelSystems.LocalizationSystems;

using Sirenix.OdinInspector;

namespace LowLevelSystems.CharacterSystems.PcSystems.Components.InterestSystems
{
[Serializable]
public class InterestSystem
{
    [Title("配置")]
    [ShowInInspector]
    public static readonly TextId HasNoEnoughInterestsPromptTextId = new TextId(1000007); // 兴致不足 文本.

    [Title("Data")]
    [ShowInInspector]
    private CharacterId _characterId;
    public CharacterId CharacterIdPy => this._characterId;
    public void SetCharacterId(int characterId)
    {
        this._characterId.InstanceId = characterId;
    }

    [ShowInInspector]
    private float _currentInterestValue;
    public float CurrentInterestValuePy => this._currentInterestValue;
    public void SetCurrentInterestValue(float currentInterestValue)
    {
        this._currentInterestValue = currentInterestValue;
    }

    [ShowInInspector]
    [NonSerialized]
    private float? _maxInterestValue;
    public float MaxInterestValuePy => this._maxInterestValue ??= this._characterId.PcPy.PcConfigPy.InitialMaxInterestPy;

    [Title("Events")]
    [ShowInInspector]
    public static event Action OnInterestChanged;
    public static void InvokeOnInterestChanged()
    {
        OnInterestChanged?.Invoke();
    }

    [Title("Methods")]
    public bool HasEnoughInterestValue(float neededInterest)
    {
        return this._currentInterestValue >= neededInterest;
    }
}
}