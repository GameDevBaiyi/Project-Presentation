using System.Collections.Generic;

using UnityEngine;

namespace LowLevelSystems.SceneSystems.Base
{
public interface IHasScenePrefab
{
    public ScenePrefabEnum ScenePrefabEnumPy { get; }
}

public interface IHasEntranceToScene
{
    public Dictionary<Vector3Int,SceneId> EntranceCoord_SceneIdPy { get; }
}

public interface IHasSceneName
{
    public string SceneNamePy { get; }
}
}