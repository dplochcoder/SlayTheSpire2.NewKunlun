using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using NewKunlun.NewKunlunCode.Afflictions;
using NewKunlun.NewKunlunCode.Collections;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Powers;

[PowerLocalization(
    title: "Toxin",
    description: "At start of combat, afflict random cards in your deck with [gold]Toxic[/gold] {Amount} {Amount:plural:time|times}."
)]
public class ToxinPower : NewKunlunPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        HoverTipFactory.FromAffliction<ToxicAffliction>();

    public override async Task BeforeSideTurnStart(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState
    )
    {
        if (side != Owner.Side || Owner.CombatState?.RoundNumber != 1)
            return;

        HashMultiset<CardModel> deckCards = new();
        for (var i = 0; i < Amount; i++) { }
    }
}
