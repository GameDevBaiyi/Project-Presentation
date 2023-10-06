using LowLevelSystems.Common;

namespace LowLevelSystems.SceneSystems.BlankSpaceSystems
{
public abstract class BlankSpaceFactory : Details
{
    public static BlankSpace GenerateSpaceAvoidingWar()
    {
        BlankSpace spaceAvoidingWar = GenerateBlankSpace();

        SceneHub.SetSpaceAvoidingWarSceneId(spaceAvoidingWar.InstanceIdPy);

        return spaceAvoidingWar;
    }

    public static BlankSpace GenerateLounge()
    {
        BlankSpace lounge = GenerateBlankSpace();

        SceneHub.SetLoungeId(lounge.InstanceIdPy);

        return lounge;
    }

    private static BlankSpace GenerateBlankSpace()
    {
        int instanceId = SceneHub.GetNextInstanceId();
        BlankSpace blankSpace = new BlankSpace(instanceId);

        SceneHub.RecordInstance(blankSpace);

        return blankSpace;
    }
}
}