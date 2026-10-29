using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Tips;

namespace NewKunlun.NewKunlunCode.Afflictions;

[AfflictionLocalization(
    title: "Toxic",
    description: "The first time played this combat, gain {Amount} [gold]Poison[/gold].",
    extraCardText: "Gain {Amount} [gold]Poison[/gold]."
)]
public class ToxicAffliction : NewKunlunAffliction
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tip.Poison()];

    public override bool CanAfflictCardType(CardType cardType) =>
        cardType is CardType.Attack or CardType.Skill or CardType.Power;

    public override async Task OnPlay(PlayerChoiceContext choiceContext, Creature? target)
    {
        await PowerCmd.Apply<PoisonPower>(
            choiceContext,
            Card.Owner.Creature,
            Amount,
            Card.Owner.Creature,
            Card
        );
        CardCmd.ClearAffliction(Card);
    }
}
