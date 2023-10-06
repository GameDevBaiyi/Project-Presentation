using System.Collections.Generic;

using LowLevelSystems.Common;
using LowLevelSystems.MissionSystems;
using LowLevelSystems.MissionSystems.Inheritors.MainMissionSystems;

namespace LowLevelSystems.CharacterSystems.PcSystems.Components.MissionActionsRecorderSystems
{
public abstract class MissionActionsRecorderDetails : Details
{
    public class StageDescriptionUIData
    {
        public string MissionName;
        public List<StageDescription> StageDescriptions;
    }
    public class StageDescription
    {
        public bool IsCurrentStage;
        public string Description;
    }

    public class ResultDescriptionUIData
    {
        public string MissionName;
        public List<string> ResultDescriptions;
    }

    private static readonly List<StageDescriptionUIData> _stageDescriptionUIDataListCache = new List<StageDescriptionUIData>(5);
    public static List<StageDescriptionUIData> GetStageDescriptionUIDataList(MissionActionsRecorder missionActionsRecorder)
    {
        _stageDescriptionUIDataListCache.Clear();

        CharacterEnum characterEnum = missionActionsRecorder.CharacterIdPy.CharacterPy.CharacterEnumPy;
        foreach (MissionRecord missionRecord in missionActionsRecorder.MissionRecordsPy)
        {
            StageDescriptionUIData stageDescriptionUIData = new StageDescriptionUIData();
            bool isOngoing = HeronTeam.MissionHubPy.MissionEnum_MissionIdPy.ContainsKey(missionRecord.MissionEnumPy);
            if (!isOngoing) continue;
            // string MissionName
            //添加任务名.
            MissionConfig missionConfig = missionRecord.MissionEnumPy.MissionConfig();
            stageDescriptionUIData.MissionName = missionConfig.MissionNamePy;

            // List<StageDescription> StageDescriptions
            List<StageDescription> stageDescriptions = new List<StageDescription>();
            foreach (MissionDescriptionPointer descriptionPointer in missionRecord.MissionDescriptionPointersPy)
            {
                // 只显示阶段性描述
                if (descriptionPointer.MissionResultIndexPy != null) continue;
                //根据指针找到对应的阶段, 对应的阶段还需要找到对应的角色.
                StageDescription stageDescription = new StageDescription();
                stageDescription.IsCurrentStage = missionRecord.MissionEnumPy.Mission().CurrentStageIdPy == descriptionPointer.MissionStageIdPy;
                stageDescription.Description = missionConfig.MissionStagesPy[descriptionPointer.MissionStageIdPy]
                                                            .CharacterAndDescriptionListPy.Find(t => t.CharacterEnumPy == characterEnum)
                                                            .DescriptionPy;

                stageDescriptions.Add(stageDescription);
            }
            stageDescriptionUIData.StageDescriptions = stageDescriptions;

            _stageDescriptionUIDataListCache.Add(stageDescriptionUIData);
        }

        return _stageDescriptionUIDataListCache;
    }

    private static readonly List<ResultDescriptionUIData> _resultDescriptionUIDataListCache = new List<ResultDescriptionUIData>(10);
    public static List<ResultDescriptionUIData> GetResultDescriptionUIDataList(MissionActionsRecorder missionActionsRecorder)
    {
        _resultDescriptionUIDataListCache.Clear();

        CharacterEnum characterEnum = missionActionsRecorder.CharacterIdPy.CharacterPy.CharacterEnumPy;
        foreach (MissionRecord missionRecord in missionActionsRecorder.MissionRecordsPy)
        {
            MissionConfig missionConfig = missionRecord.MissionEnumPy.MissionConfig();

            // List<string> ResultDescriptions
            List<string> resultStrings = new List<string>();
            //添加任务描述.
            foreach (MissionDescriptionPointer descriptionPointer in missionRecord.MissionDescriptionPointersPy)
            {
                // 只看结果性描述
                if (descriptionPointer.MissionResultIndexPy == null) continue;
                resultStrings.Add(missionConfig.MissionStagesPy[descriptionPointer.MissionStageIdPy]
                                               .CompletionConditionsAndResultsListPy[descriptionPointer.MissionResultIndexPy.Value]
                                               .CharacterAndDescriptionListPy.Find(t => t.CharacterEnumPy == characterEnum)
                                               .DescriptionPy);
            }
            //无内容就不显示. 
            if (resultStrings.Count <= 0) continue;

            ResultDescriptionUIData resultDescriptionUIData = new ResultDescriptionUIData();
            resultDescriptionUIData.ResultDescriptions = resultStrings;

            // string MissionName
            //添加任务名.
            resultDescriptionUIData.MissionName = missionConfig.MissionNamePy;

            _resultDescriptionUIDataListCache.Add(resultDescriptionUIData);
        }

        return _resultDescriptionUIDataListCache;
    }
}
}