using LowLevelSystems.Common;

namespace LowLevelSystems.CharacterSystems.PcSystems.Components.InterestSystems
{
public abstract class InterestSystemFactory : Details
{
    public static InterestSystem GenerateInterestSystem(int characterIdParam)
    {
        InterestSystem interestSystem = new InterestSystem();

        //int _characterId
        int characterId = characterIdParam;

        //float _currentInterestValue
        float currentInterestValue = default(float);

        interestSystem.SetCharacterId(characterId);
        //default
        interestSystem.SetCurrentInterestValue(currentInterestValue);

        return interestSystem;
    }
}
}