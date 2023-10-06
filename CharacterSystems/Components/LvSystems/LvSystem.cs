using System;

using LowLevelSystems.Common;

using Sirenix.OdinInspector;

namespace LowLevelSystems.CharacterSystems.Components.LvSystems
{
[Serializable]
public class LvSystem
{
    [Serializable]
    public struct Level : ICanDoLevelAnime
    {
        [ShowInInspector]
        public int CurrentLv;
        public int CurrentLvPy
        {
            get => this.CurrentLv;
            set => this.CurrentLv = value;
        }

        [ShowInInspector]
        public float CurrentExp;
        public float CurrentExpPy
        {
            get => this.CurrentExp;
            set => this.CurrentExp = value;
        }

        public Level(int currentLv,float currentExp)
        {
            this.CurrentLv = currentLv;
            this.CurrentExp = currentExp;
        }

        [ShowInInspector]
        public int MaxLvPy => int.MaxValue;
        [ShowInInspector]
        public float MaxExpPy => _maxExp;

        [ShowInInspector]
        public float AllExpPy => this.CurrentLv * _maxExp + this.CurrentExp;

        public static Level operator +(Level lvA,int lvAddend)
        {
            return new Level(lvA.CurrentLv + lvAddend,0f);
        }

        public static Level operator +(Level lvA,float expAddend)
        {
            float allExp = lvA.AllExpPy + expAddend;
            int targetLv = (int)allExp / _maxExp;
            float targetExp = allExp - targetLv * _maxExp;
            return new Level(targetLv,targetExp);
        }

        public ICanDoLevelAnime AddExp(float expAddend)
        {
            return this + expAddend;
        }
    }

    [Title("Config")]
    private const int _maxExp = 100;

    [Title("Data")]
    [ShowInInspector]
    private Level _lv;
    public Level LvPy => this._lv;

    public void AddLv(int lvAddend)
    {
        this._lv += lvAddend;
    }
    public void AddExp(float expAddend)
    {
        this._lv += expAddend;
    }

    public void SetLv(int lv)
    {
        this._lv = new Level(lv,0f);
    }
}
}