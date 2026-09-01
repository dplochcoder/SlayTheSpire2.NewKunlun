using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using NewKunlun.NewKunlunCode.Cards;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Powers;

[PowerLocalization(
    title: "Full Control",
    description: "[gold]Talisman Detonate[/gold] deals {Amount} additional damage per [gold]Qi Charge[/gold]. You can choose how many [gold]Qi Charges[/gold] to spend on detonation, and can spend any number.",
    smartDescription: ""
)]
public class FullControlPower : NewKunlunPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            Tips.Card<TalismanDetonateCard>(TalismanDashCard.IsUpgradedAnywhere(Owner.Player)),
            Tips.Power<QiChargePower>(),
        ];

    public async Task<decimal> ConsumeQiCharges(
        PlayerChoiceContext choiceContext,
        Player player,
        CardModel? cardSource
    )
    {
        var available = player.Creature.GetPowerAmount<QiChargePower>();
        if (available <= 0)
            return 0;

        List<CardModel> cards = [];
        for (var i = 0; i < available; i++)
        {
            var card = CombatState.CreateCard<QiChargeCard>(player);
            card.QiCharges.BaseValue = i + 1;
            cards.Add(card);
        }

        var selected = await CardSelectCmd.FromChooseACardScreen(choiceContext, cards, player);
        var toSpend = ((QiChargeCard)selected!).QiCharges.BaseValue;

        var actualSpent = await QiChargeCmd.ConsumeQiCharges(
            choiceContext,
            player.Creature,
            toSpend,
            player.Creature,
            cardSource
        );
        return actualSpent;
    }
}
