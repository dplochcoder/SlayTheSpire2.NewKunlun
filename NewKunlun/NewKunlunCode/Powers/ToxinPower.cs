using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Modifiers;

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
        NewKunlunModifier.HoverTips<ToxicModifier>();

    public override Task BeforeSideTurnStart(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState
    )
    {
        if (
            side != Owner.Side
            || Owner.CombatState?.RoundNumber != 1
            || Owner.Player?.PlayerCombatState == null
        )
            return Task.CompletedTask;

        var modifier = ModelDb.Get<ToxicModifier>();
        List<CardModel> cards =
        [
            .. Owner.Player.PlayerCombatState.DrawPile.Cards.Where(modifier.CanModify),
        ];
        if (cards.Count == 0)
            return Task.CompletedTask;

        HashSet<CardModel> modified = [];
        for (var i = 0; i < Amount; i++)
        {
            var next = combatState.RunState.Rng.CombatCardSelection.NextItem(cards)!;
            modified.Add(next);
            CardModifier.AddModifier<ToxicModifier>(next);
        }

        Flash();
        CardCmd.Preview([.. cards.Where(modified.Contains)]);
        return Task.CompletedTask;
    }
}
