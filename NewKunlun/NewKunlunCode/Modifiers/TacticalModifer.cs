using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Modifiers;

[CardModifierLocalization(
    title: "Tactical",
    description: "Next turn, play this card.\nIf played, lose [gold]Tactical[/gold]."
)]
public class TacticalModifier : NewKunlunModifier
{
    public override ModifierStackType StackType => ModifierStackType.Single;

    public override async Task AfterAutoPrePlayPhaseEntered(
        PlayerChoiceContext choiceContext,
        Player player
    )
    {
        if (Owner == null || player != Owner.Owner)
            return;

        await CardCmd.AutoPlay(choiceContext, Owner, null);
    }

    public override Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        RemoveModifier(cardPlay.Card, this);
        return Task.CompletedTask;
    }
}
