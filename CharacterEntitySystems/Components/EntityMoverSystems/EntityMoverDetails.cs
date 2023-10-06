using LowLevelSystems.BattleSystems.Base;
using LowLevelSystems.BattleSystems.Conditions;
using LowLevelSystems.CharacterEntitySystems.NpcEntitySystems;
using LowLevelSystems.CharacterEntitySystems.PcEntitySystems;
using LowLevelSystems.Common;
using LowLevelSystems.InteractableSystems;
using LowLevelSystems.SkillSystems.SkillBuffSystems;

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

namespace LowLevelSystems.CharacterEntitySystems.Components.EntityMoverSystems
{
public abstract class EntityMoverDetails : Details
{
    public static void WhenJourneyStartReached(EntityMover entityMover)
    {
        CharacterEntity characterEntity = entityMover.CharacterEntityPy;
        characterEntity.ChangeDirection(entityMover.DirectionToNextWaypointPy);
        MechanicsOfBuffTriggerTiming.TriggerOnMove(characterEntity);
        MechanicsOfAutoRemoveBuff.RecordHasMovedSomeDistance(characterEntity.BuffPoolPy);
        switch (characterEntity)
        {
        case PcEntity pcEntity:
            EntityMoverDetails_Pc.WhenJourneyStartReached(pcEntity,entityMover);
            break;

        case NpcEntity npcEntity:
            EntityMoverDetails_Npc.WhenJourneyStartReached(npcEntity,entityMover);
            break;
        }
    }

    public static void WhenJourneyEndReached(EntityMover entityMover)
    {
        CharacterEntity characterEntity = entityMover.CharacterEntityPy;

        switch (characterEntity)
        {
        case PcEntity pcEntity:
            EntityMoverDetails_Pc.WhenJourneyEndReached(pcEntity,entityMover);
            break;

        case NpcEntity npcEntity:
            EntityMoverDetails_Npc.WhenJourneyEndReached(npcEntity,entityMover);
            break;
        }
    }

    public static void WhenDestinationReached(EntityMover entityMover)
    {
        CharacterEntity characterEntity = entityMover.CharacterEntityPy;

        switch (characterEntity)
        {
        case PcEntity pcEntity:
            EntityMoverDetails_Pc.WhenDestinationReached(pcEntity,entityMover);
            break;

        case NpcEntity:
            //DoNothing
            break;
        }
    }

    public static void WhenBeginToMove(EntityMover entityMover)
    {
        CharacterEntity characterEntity = entityMover.CharacterEntityPy;

        switch (characterEntity)
        {
        case PcEntity pcEntity:
            EntityMoverDetails_Pc.WhenBeginToMove(pcEntity,entityMover);
            break;

        case NpcEntity npcEntity:
            EntityMoverDetails_Npc.WhenBeginToMove(npcEntity,entityMover);
            break;
        }
    }

    public static void WhenStoppedMove(EntityMover entityMover)
    {
        CharacterEntity characterEntity = entityMover.CharacterEntityPy;

        switch (characterEntity)
        {
        case PcEntity pcEntity:
            EntityMoverDetails_Pc.WhenStoppedMove(pcEntity,entityMover);
            break;

        case NpcEntity npcEntity:
            EntityMoverDetails_Npc.WhenStoppedMove(npcEntity,entityMover);
            break;
        }

        if (!_battleManager.IsInBattlePy) return;
        MechanicsOfConditions.UpdateConditionsRelatedToEnterArea();
        BattleDetails.CheckAndSettleBattleAsync();
    }
}
}