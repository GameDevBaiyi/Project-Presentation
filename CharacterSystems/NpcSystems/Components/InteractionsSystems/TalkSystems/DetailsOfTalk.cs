using Common.Extensions;

using LowLevelSystems.CharacterEntitySystems.NpcEntitySystems;
using LowLevelSystems.CharacterEntitySystems.PcEntitySystems;
using LowLevelSystems.CharacterSystems.PcSystems.Components.InterestSystems;
using LowLevelSystems.Common;

using UICommon;

namespace LowLevelSystems.CharacterSystems.NpcSystems.Components.InteractionsSystems.TalkSystems
{
public abstract class DetailsOfTalk : Details
{
    /// <summary>
    /// 由 交互 Ui 调用. 显示 对话泡泡. 
    /// </summary>
    public static void Talk(PcEntity pcEntity,NpcEntity npcEntity)
    {
        //功能: 如果当前 Pc 的兴致值足够, 才可以进行交谈.
        InterestSystem interestSystem = pcEntity.PcPy.InterestSystemPy;
        if (!DetailsOfInterestSystem.HasEnoughInterestAndPromptIfNot(interestSystem,SettingsSo.InterestCostForTalking)) return;
        DetailsOfInterestSystem.ChangeLimitedValue(interestSystem,-SettingsSo.InterestCostForTalking);

        Talk talk = npcEntity.NpcPy.InteractionsPy.GetInteraction<Talk>(InteractionEnum.Talk);
        if (talk == null) return;
        // 取消之前的 Bubble, 该 Bubble 会自动回到 Pool 中.
        if (npcEntity.Bubble?.visible ?? false)
        {
            npcEntity.Bubble.TypingEffectPy.Cancel();
            npcEntity.Bubble.visible = false;
        }
        UI_Component_DialogueBubble bubble
            = UiManager.DialogueBubblePoolPy.ShowDialogueBubbleAsync(npcEntity.SelfTransformPy,talk.TextIdsPy.GetRandomItem().TextPy,npcEntity.NpcPy.NamePy);
        npcEntity.Bubble = bubble;
    }
}
}