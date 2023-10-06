using System;
using System.Collections.Generic;

using LowLevelSystems.MissionSystems.Inheritors.MainMissionSystems;

using Sirenix.OdinInspector;

namespace LowLevelSystems.CharacterSystems.PcSystems.Components.MissionActionsRecorderSystems
{
[Serializable]
public class MissionActionsRecorder
{
    [Title("Data")]
    [ShowInInspector]
    private readonly CharacterId _characterId;
    public CharacterId CharacterIdPy => this._characterId;

    [ShowInInspector]
    private readonly List<MissionRecord> _missionRecords = new List<MissionRecord>();
    public List<MissionRecord> MissionRecordsPy => this._missionRecords;

    public MissionActionsRecorder(CharacterId characterId)
    {
        this._characterId = characterId;
    }

    [Title("Methods")]
    [ShowInInspector]
    public List<MissionActionsRecorderDetails.StageDescriptionUIData> StageDescriptionUIDataListPy => MissionActionsRecorderDetails.GetStageDescriptionUIDataList(this);
    [ShowInInspector]
    public List<MissionActionsRecorderDetails.ResultDescriptionUIData> ResultDescriptionUIDataListPy => MissionActionsRecorderDetails.GetResultDescriptionUIDataList(this);
    public void AddDescription(MissionEnum missionEnum,int stageId,int? resultIndex)
    {
        MissionRecord missionRecord = this._missionRecords.Find(t => t.MissionEnumPy == missionEnum);
        if (missionRecord == null)
        {
            missionRecord = new MissionRecord();
            missionRecord.SetMissionEnum(missionEnum);
            missionRecord.SetMissionDescriptionPointers(new List<MissionDescriptionPointer>());
            this._missionRecords.Add(missionRecord);
        }

        // 如果对应的任务记录已经记录了该阶段或者结果描述, 那么不重复记录.
        if (missionRecord.MissionDescriptionPointersPy.Exists(t => t.MissionStageIdPy == stageId && t.MissionResultIndexPy == resultIndex)) return;

        // 添加新纪录.
        MissionDescriptionPointer missionDescriptionPointer = new MissionDescriptionPointer(stageId,resultIndex);
        missionRecord.MissionDescriptionPointersPy.Add(missionDescriptionPointer);
    }
}

[Serializable]
public class MissionRecord
{
    [Title("Data")]
    [ShowInInspector]
    private MissionEnum _missionEnum;
    public MissionEnum MissionEnumPy => this._missionEnum;
    public void SetMissionEnum(MissionEnum missionEnum)
    {
        this._missionEnum = missionEnum;
    }

    [ShowInInspector]
    private List<MissionDescriptionPointer> _missionDescriptionPointers;
    public List<MissionDescriptionPointer> MissionDescriptionPointersPy => this._missionDescriptionPointers;
    public void SetMissionDescriptionPointers(List<MissionDescriptionPointer> missionDescriptionPointers)
    {
        this._missionDescriptionPointers = missionDescriptionPointers;
    }
}

[Serializable]
public struct MissionDescriptionPointer
{
    [ShowInInspector]
    private int _missionStageId;
    public int MissionStageIdPy => this._missionStageId;

    [ShowInInspector]
    private int? _missionResultIndex;
    public int? MissionResultIndexPy => this._missionResultIndex;

    public MissionDescriptionPointer(int missionStageId,int? missionResultIndex)
    {
        this._missionStageId = missionStageId;
        this._missionResultIndex = missionResultIndex;
    }
}
}