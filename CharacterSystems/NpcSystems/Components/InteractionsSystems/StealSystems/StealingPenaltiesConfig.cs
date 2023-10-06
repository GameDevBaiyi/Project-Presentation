using System;
using System.Collections.Generic;

using LowLevelSystems.Common;

using Sirenix.OdinInspector;

using UnityEngine;

namespace LowLevelSystems.CharacterSystems.NpcSystems.Components.InteractionsSystems.StealSystems
{
[Serializable]
public class StealingPenaltiesConfig
{
    [Serializable]
    public class PenaltiesConfig
    {
        [LabelText("罚金")]
        [SerializeField]
        private int _fine;
        public int FinePy => this._fine;
        public void SetFine(int fine)
        {
            this._fine = fine;
        }

        [LabelText("入狱天数公式的倍数")]
        [SerializeField]
        private int _times;
        public int TimesPy => this._times;
        public void SetTimes(int times)
        {
            this._times = times;
        }
    }

    [SerializeField]
    private CampEnum _campEnum;
    public CampEnum CampEnumPy => this._campEnum;
    public void SetCampEnum(CampEnum campEnum)
    {
        this._campEnum = campEnum;
    }

    [SerializeField]
    private List<PenaltiesConfig> _penaltiesConfigs;
    public List<PenaltiesConfig> PenaltiesConfigsPy => this._penaltiesConfigs;
    public void SetPenaltiesConfigs(List<PenaltiesConfig> penaltiesConfigs)
    {
        this._penaltiesConfigs = penaltiesConfigs;
    }
}
}