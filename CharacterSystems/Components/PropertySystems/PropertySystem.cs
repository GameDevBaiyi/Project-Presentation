using System;
using System.Collections.Generic;
using System.Linq;

using LowLevelSystems.CharacterSystems.Components.LvSystems;
using LowLevelSystems.CharacterSystems.NpcSystems;
using LowLevelSystems.CharacterSystems.PcSystems;
using LowLevelSystems.Common;

using Sirenix.OdinInspector;

using UnityEngine;

using Random = UnityEngine.Random;

namespace LowLevelSystems.CharacterSystems.Components.PropertySystems
{
[Serializable]
public class PropertySystem
{
#region EditorOnly
#if UNITY_EDITOR
    [Title("EditorOnly")]
    [ShowInInspector]
    // ReSharper disable once InconsistentNaming
    private Dictionary<PropertyEnum,float> _allPropertiesEditorOnlyPy
    {
        get
        {
            PropertyEnum[] propertyEnums = (PropertyEnum[])Enum.GetValues(typeof(PropertyEnum));
            Dictionary<PropertyEnum,float> propertyEnum_value = new Dictionary<PropertyEnum,float>(propertyEnums.Length);
            foreach (PropertyEnum propertyEnum in propertyEnums)
            {
                propertyEnum_value[propertyEnum] = this[propertyEnum];
            }
            return propertyEnum_value;
        }
    }

#endif
#endregion

    public static List<PropertyEnum> AllLv2PropertyEnumsPy
    {
        get
        {
            List<PropertyEnum> propertyEnums = new List<PropertyEnum>(208 - 201);
            for (int i = 201; i < 208; i++)
            {
                propertyEnums.Add((PropertyEnum)i);
            }
            return propertyEnums;
        }
    }
    public static PropertyEnum GetRandomLv2PropertyEnum()
    {
        return (PropertyEnum)Random.Range(201,208);
    }

    [Title("Data")]
    [ShowInInspector]
    private CharacterId _characterId;
    public CharacterId CharacterIdPy => this._characterId;
    public void SetCharacterId(int characterId)
    {
        this._characterId.InstanceId = characterId;
    }

    [ShowInInspector]
    private readonly Character.CharacterTypeEnum _characterTypeEnum;
    public Character.CharacterTypeEnum CharacterTypeEnumPy => this._characterTypeEnum;

    [ShowInInspector]
    private readonly CharacterEnum _characterEnum;
    public CharacterEnum CharacterEnumPy => this._characterEnum;

    [ShowInInspector]
    private readonly List<LibraryOfExtra4Values> _librariesOfExtra4Values = new List<LibraryOfExtra4Values>(5);
    public List<LibraryOfExtra4Values> LibrariesOfExtra4ValuesPy => this._librariesOfExtra4Values;

    [ShowInInspector]
    private bool _isAlive = true;
    public bool IsAlivePy => this._isAlive;
    public void SetIsAlive(bool isAlive)
    {
        this._isAlive = isAlive;
    }

    [Title("当前值")]
    [ShowInInspector]
    private float _currentHp;
    public float CurrentHpPy => this._currentHp;
    public void SetCurrentHp(float currentAp)
    {
        this._currentHp = currentAp;
    }
    [ShowInInspector]
    private float _currentBodyVeinHp;
    public float CurrentBodyVeinHpPy => this._currentBodyVeinHp;
    public void SetCurrentBodyVeinHp(float currentBodyVeinHp)
    {
        this._currentBodyVeinHp = currentBodyVeinHp;
    }
    [ShowInInspector]
    private float _currentSpiritVeinHp;
    public float CurrentSpiritVeinHpPy => this._currentSpiritVeinHp;
    public void SetCurrentSpiritVeinHp(float currentSpiritVeinHp)
    {
        this._currentSpiritVeinHp = currentSpiritVeinHp;
    }
    [ShowInInspector]
    private float _currentAp;
    public float CurrentApPy => this._currentAp;
    public void SetCurrentAp(float currentAp)
    {
        this._currentAp = currentAp;
    }

    [SerializeField]
    private int _propertyPoints;
    public int PropertyPointsPy => this._propertyPoints;
    public void AddPropertyPoints(int addend)
    {
        this._propertyPoints += addend;
    }

    [Title("References")]
    [NonSerialized]
    private LvSystem _lvSystem;
    public LvSystem LvSystemPy => this._lvSystem ??= this._characterId.CharacterPy.LvSystemPy;

    [NonSerialized]
    private static Dictionary<PropertyEnum,float> _propertyEnum_lvMultiplier;
    public static Dictionary<PropertyEnum,float> PropertyEum_LvMultiplierPy
    {
        get
        {
            if (_propertyEnum_lvMultiplier != null) return _propertyEnum_lvMultiplier;
            List<NpcPropertyLvConfig> npcPropertyLvConfigs = Details.CommonDesignSO.CharacterConfigHubPy.NpcPropertyLvConfigsPy;
            _propertyEnum_lvMultiplier = new Dictionary<PropertyEnum,float>(npcPropertyLvConfigs.Count);
            foreach (NpcPropertyLvConfig npcPropertyLvConfig in npcPropertyLvConfigs)
            {
                _propertyEnum_lvMultiplier[npcPropertyLvConfig.PropertyEnumPy] = npcPropertyLvConfig.LvIncreaseFactorPy;
            }
            return _propertyEnum_lvMultiplier;
        }
    }

    public PropertySystem(int characterId,Character.CharacterTypeEnum characterTypeEnum,CharacterEnum characterEnum)
    {
        this.SetCharacterId(characterId);
        this._characterTypeEnum = characterTypeEnum;
        this._characterEnum = characterEnum;

        if (this._characterTypeEnum == Character.CharacterTypeEnum.Pc)
        {
            this._librariesOfExtra4Values.Add(new LibraryOfExtra4Values(characterId));
        }
    }

    [Title("Events")]
    [ShowInInspector]
    public static event Action<PropertySystem,float,float,float> OnHpChanged;
    [ShowInInspector]
    public static event Action<PropertySystem,float> OnApChanged;

    [Title("Methods")]
    public float this[PropertyEnum propertyEnum]
    {
        get
        {
            float resultOfCommonPropertyFormula = CommonPropertyFormula(this.CalculateBaseValue(propertyEnum),
                                                                        this._librariesOfExtra4Values.Sum(t => t[propertyEnum,PosOfPropertyFormulaEnum.baseAdd]),
                                                                        this._librariesOfExtra4Values.Sum(t => t[propertyEnum,PosOfPropertyFormulaEnum.pct]),
                                                                        this._librariesOfExtra4Values.Sum(t => t[propertyEnum,PosOfPropertyFormulaEnum.finalAdd]),
                                                                        this._librariesOfExtra4Values.Sum(t => t[propertyEnum,PosOfPropertyFormulaEnum.finalPct]));
            // Npc 的属性需要乘以等级系数. （1f + 等级 * 递增值）
            if (this._characterTypeEnum == Character.CharacterTypeEnum.Npc)
            {
                PropertyEum_LvMultiplierPy.TryGetValue(propertyEnum,out float lvMultiplier);
                resultOfCommonPropertyFormula *= (1f + this.LvSystemPy.LvPy.CurrentLvPy * lvMultiplier);
            }
            return resultOfCommonPropertyFormula;
        }
    }

    private float CalculateBaseValue(PropertyEnum propertyEnum)
    {
        CharacterConfig characterConfig = this._characterTypeEnum == Character.CharacterTypeEnum.Npc ? this._characterEnum.CharacterConfig() : this._characterEnum.PcConfig();
        switch (propertyEnum)
        {
        // Pa=Strength+Balance/2
        // baseHp = InitialMaxHP*(1+Pa/(0.5*Pa+50))+20*Health
        case PropertyEnum.MaxHP:
            float pa = this[PropertyEnum.Strength] + this[PropertyEnum.Balance] / 2f;
            return characterConfig.GetInitialPropertyValue(PropertyEnum.MaxHP) * (1f + pa / (0.5f * pa + 50f)) + 20f * this[PropertyEnum.Health];

        // 体魄(health) 小于或者等于 20 时, baseBodyVeinHp =  initialBodyVeinHp * (1f + 力量 / (0.5f * 力量 + 50f)),
        // 体魄(health) > 20 时,再多运算一步公式: baseBodyVeinHp =  baseBodyVeinHp * (1f + (体魄 - 20f) / (0.5f * 体魄 + 10f)),
        case PropertyEnum.MaxBodyVeinHp:
            float strength = this[PropertyEnum.Strength];
            float health = this[PropertyEnum.Health];
            float baseBodyVeinHp = characterConfig.GetInitialPropertyValue(PropertyEnum.MaxBodyVeinHp) * (1f + strength / (0.5f * strength + 50f));
            if (health > 20f)
            {
                baseBodyVeinHp *= 1f + (health - 20f) / (0.5f * health + 10f);
            }
            return baseBodyVeinHp;

        // 体魄(health) 小于或者等于 20 时, baseBodyVeinHp =  initialSpiritVeinHp,
        // 体魄(health) > 20 时, 再多运算一步公式: baseBodyVeinHp =  baseBodyVeinHp * (1f + (体魄 - 20f) / (0.5f * 体魄 + 10f)),
        case PropertyEnum.MaxSpiritVeinHp:
            health = this[PropertyEnum.Health];
            float baseSpiritVeinHp = characterConfig.GetInitialPropertyValue(PropertyEnum.MaxSpiritVeinHp);
            if (health > 20f)
            {
                baseSpiritVeinHp *= 1f + (health - 20f) / (0.5f * health + 10f);
            }
            return baseSpiritVeinHp;

        // base体脉抗性 = IniBodyVeinResistance+100*Toughness/(Toughness+200)
        case PropertyEnum.BodyVeinResistance:
            float toughness = this[PropertyEnum.Toughness];
            return characterConfig.GetInitialPropertyValue(PropertyEnum.BodyVeinResistance) + 100f * toughness / (toughness + 200f);

        // base灵脉抗性 = IniSpiritVeinResistance
        case PropertyEnum.SpiritVeinResistance:
            return characterConfig.GetInitialPropertyValue(PropertyEnum.SpiritVeinResistance);

        // 基础暴击率 = IniCriticalRate+70*Agility/(Agility+50)
        case PropertyEnum.CriticalRate:
            float agility = this[PropertyEnum.Agility];
            return characterConfig.GetInitialPropertyValue(PropertyEnum.CriticalRate) + 70f * agility / (agility + 50f);

        // base暴击抵抗 = IniCriticalResistance+70*Strength/(Strength+50)
        case PropertyEnum.CriticalResistance:
            strength = this[PropertyEnum.Strength];
            return characterConfig.GetInitialPropertyValue(PropertyEnum.CriticalResistance) + 70f * strength / (strength + 50f);

        // base暴击伤害 = IniCriticalDamage+15*floor(Precision/5)
        case PropertyEnum.CriticalDamage:
            float precision = this[PropertyEnum.Precision];
            return characterConfig.GetInitialPropertyValue(PropertyEnum.CriticalDamage) + 15f * Mathf.Floor(precision / 5f);

        // base闪避率 = IniDodgeRate+100*Agility/(Agility*2+60)
        case PropertyEnum.DodgeRate:
            agility = this[PropertyEnum.Agility];
            return characterConfig.GetInitialPropertyValue(PropertyEnum.DodgeRate) + 100f * agility / (agility * 2f + 60f);

        // base命中 = IniHitRate+100*Balance/(Balance+100)
        case PropertyEnum.HitRate:
            float balance = this[PropertyEnum.Balance];
            return characterConfig.GetInitialPropertyValue(PropertyEnum.HitRate) + 100f * balance / (balance + 100f);

        // base限制抵抗 = IniControlResistance+1.5*floor(Strength/3)
        case PropertyEnum.ControlResistance:
            strength = this[PropertyEnum.Strength];
            return characterConfig.GetInitialPropertyValue(PropertyEnum.ControlResistance) + 1.5f * Mathf.Floor(strength / 3f);

        // base伤害抵抗 = IniDamageResistance+0.75*floor(Strength/3)
        case PropertyEnum.DamageResistance:
            strength = this[PropertyEnum.Strength];
            return characterConfig.GetInitialPropertyValue(PropertyEnum.DamageResistance) + 0.75f * Mathf.Floor(strength / 3f);

        // base减益抵抗 = IniDeBuffResistance+1.5*floor(Balance/3)
        case PropertyEnum.DeBuffResistance:
            balance = this[PropertyEnum.Balance];
            return characterConfig.GetInitialPropertyValue(PropertyEnum.DeBuffResistance) + 1.5f * Mathf.Floor(balance / 3f);

        default:
            return characterConfig.GetInitialPropertyValue(propertyEnum);
        }
    }

    private static float CommonPropertyFormula(float baseValue,float baseAdd,float pct,
                                               float finalAdd,float finalPct)
    {
        return ((baseValue + baseAdd) * (1f + pct / 100f) + finalAdd) * (1f + finalPct / 100f);
    }

    public LibraryOfExtra4Values PermanentLibraryOfExtra4ValuesPy
    {
        get
        {
            if (this._characterTypeEnum != Character.CharacterTypeEnum.Pc)
            {
                Debug.LogError($"非 Pc 角色不会有永久增加属性的库. ");
                return null;
            }
            return this._librariesOfExtra4Values[0];
        }
    }

    [ShowInInspector]
    public float AttackPy => Random.Range(this[PropertyEnum.MinAttack],this[PropertyEnum.MaxAttack]);
    public float CalculateAttackOfBodyVeinSkill(PropertyEnum lv2PropertyEnum)
    {
        //公式: Ar+Aw*(1+P2/100)
        float weaponAttack = 0f;
        if (this._characterId.CharacterPy is Pc pc)
        {
            weaponAttack = pc.EquipmentInventoryPy.WeaponPy?.AttackPy ?? 0f;
        }

        return this.AttackPy + weaponAttack * (1f + this[lv2PropertyEnum] / 100f);
    }
    [ShowInInspector]
    public float AttackOfSpiritVeinSkillPy => this.AttackPy;

    public void ChangeHp(float hpAddend,float bodyVeinHpAddend,float spiritVeinHpAddend,
                         bool isIgnoringEvents = false)
    {
        this._currentHp += hpAddend;
        this._currentBodyVeinHp += bodyVeinHpAddend;
        this._currentSpiritVeinHp += spiritVeinHpAddend;

        if (this._currentHp <= 0f)
        {
            MechanicsOfDeath.MarkAsDeadAsync(this);
        }
        if (isIgnoringEvents) return;
        OnHpChanged?.Invoke(this,hpAddend,bodyVeinHpAddend,spiritVeinHpAddend);
    }
    public void HealAll(bool isIgnoringEvents = false)
    {
        float hpAddend = this[PropertyEnum.MaxHP] - this._currentHp;
        float bodyVeinHpAddend = this[PropertyEnum.MaxBodyVeinHp] - this._currentBodyVeinHp;
        float spiritVeinHpAddend = this[PropertyEnum.MaxSpiritVeinHp] - this._currentSpiritVeinHp;
        this.ChangeHp(hpAddend,bodyVeinHpAddend,spiritVeinHpAddend,isIgnoringEvents);
    }

    public void ChangeAp(float addend)
    {
        this._currentAp += addend;

        OnApChanged?.Invoke(this,addend);
    }

    /// <summary>
    /// 设计: Pc 的第一个属性库是 Dish, 第二个是 Buff, Npc 的第一个是 Buff. 
    /// </summary>
    public void AddLibraryOfExtra4ValuesForBuff(LibraryOfExtra4Values libraryOfExtra4Values)
    {
        if (this._characterTypeEnum == Character.CharacterTypeEnum.Pc)
        {
            if (this._librariesOfExtra4Values.Count != 1)
            {
                Debug.LogError($"该 Pc 的 属性库在添加 Buff 属性库时有 {this._librariesOfExtra4Values.Count} 个. 是否有问题?");
                this._librariesOfExtra4Values.RemoveAt(1);
            }
            this._librariesOfExtra4Values.Add(libraryOfExtra4Values);
        }
        else
        {
            if (this._librariesOfExtra4Values.Count != 0)
            {
                Debug.LogError($"该 Npc 的 属性库在添加 Buff 属性库时有 {this._librariesOfExtra4Values.Count} 个. 是否有问题?");
                this._librariesOfExtra4Values.RemoveAt(0);
            }
            this._librariesOfExtra4Values.Add(libraryOfExtra4Values);
        }
    }
    public void RemoveLibraryOfExtra4ValuesForBuff()
    {
        if (this._characterTypeEnum == Character.CharacterTypeEnum.Pc)
        {
            this._librariesOfExtra4Values.RemoveAt(1);
        }
        else
        {
            this._librariesOfExtra4Values.RemoveAt(0);
        }
    }

    public void AssignPropertyPoints(PropertyEnum propertyEnum)
    {
        if (this._propertyPoints <= 0)
        {
            Debug.LogError($"没有属性点了, 依然尝试增加属性: {this._propertyPoints}");
        }
        this.PermanentLibraryOfExtra4ValuesPy.ChangeProperty(propertyEnum,PosOfPropertyFormulaEnum.baseAdd,1);
        this._propertyPoints--;
    }

    // BaiyiTODO. 可以节约掉 Methods. 
    public void ResetApTo0()
    {
        this.ChangeAp(-this._currentAp);
    }
}
}