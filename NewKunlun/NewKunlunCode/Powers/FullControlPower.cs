using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using NewKunlun.NewKunlunCode.Cards;
using NewKunlun.NewKunlunCode.Commands;
using NewKunlun.NewKunlunCode.Hooks;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Tips;

namespace NewKunlun.NewKunlunCode.Powers;

[PowerLocalization(
    title: "Full Control",
    description: "[gold]Discharge[/gold] any number of [gold]Qi Charges[/gold] when you [gold]Detonate[/gold]."
)]
public partial class FullControlPower : NewKunlunPower, ITalismanDetonateListener
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [Tip.Discharge(), Tip.QiCharge(), Tip.Detonate()];

    public async Task<int> Discharge(
        PlayerChoiceContext choiceContext,
        Player player,
        TalismanDetonateCard cardSource
    )
    {
        var available = player.Creature.GetPowerAmount<QiChargePower>();
        if (available <= 1)
            return available;

        List<CardModel> cards = [];
        for (var i = 0; i < available; i++)
        {
            var card = CombatState.CreateCard<QiChargesCard>(player);
            card.QiCharges.BaseValue = i + 1;
            card.DetonateDamage.BaseValue =
                (i + 1) * cardSource.TalismanDetonateBaseDamage.Calculate(cardSource);
            cards.Add(card);
        }

        var selected = await CardSelectCmd.FromChooseACardScreen(choiceContext, cards, player);
        var toSpend = ((QiChargesCard)selected!).QiCharges.IntValue;

        var actualSpent = await QiChargeCmd.Discharge(
            choiceContext,
            player.Creature,
            toSpend,
            player.Creature,
            cardSource
        );
        return actualSpent;
    }
}
