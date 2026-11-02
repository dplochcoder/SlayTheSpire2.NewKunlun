using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Modifiers;

[CardModifierLocalization(
    title: "Toxic",
    description: "Gain {Amount} [gold]Poison[/gold] the first time played this combat."
)]
public class ToxicModifier : NewKunlunModifier
{
    public override ModifierStackType StackType => ModifierStackType.Counter;

    public override bool CanModify(CardModel cardModel) =>
        cardModel.Type is CardType.Attack or CardType.Skill or CardType.Power;

    public override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<PoisonPower>()];

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<PoisonPower>(
            choiceContext,
            cardPlay.Card.Owner.Creature,
            Amount,
            cardPlay.Card.Owner.Creature,
            cardPlay.Card
        );
        RemoveModifier(cardPlay.Card, this);
    }

    public override void ModifyDescriptionPost(Creature? target, ref string description) =>
        description = $"{description}\nGain {Amount} [gold]Poison[/gold].";
}
