using LowLevelSystems.CharacterSystems.Components.PropertySystems;

using Sirenix.OdinInspector;

namespace LowLevelSystems.CharacterEntitySystems.PcEntitySystems.Components.PcFSMSystems
{
public class NormalState : InputState
{
    public NormalState(InputFSM.InputStateEnum stateEnum) : base(stateEnum)
    {
    }

    [Title("Methods")]
    public override bool CanEnter()
    {
        return true;
    }
    public override void OnEnter()
    {
        //DoNothing. 
    }
    public override bool CanExit()
    {
        if (MechanicsOfDeath.CheckHasDeadForInputFsm(HeronTeam.CurrentPcInControlPy.PropertySystemPy)) return false;

        return true;
    }
    public override void OnExit()
    {
        //DoNothing.
    }
}
}