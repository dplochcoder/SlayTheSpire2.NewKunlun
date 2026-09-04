using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using NewKunlun.NewKunlunCode.Cards;
using NewKunlun.NewKunlunCode.Commands;
using NewKunlun.NewKunlunCode.Hooks;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Tips;
using NewKunlun.NewKunlunCode.Variables;

namespace NewKunlun.NewKunlunCode.Powers;

[PowerLocalization(
    title: "Full Control",
    description: "{TalismanDetonate:cardName} deals {Amount} additional damage per [gold]Qi Charge[/gold]. You can choose how many [gold]Qi Charges[/gold] to spend on detonation, and can spend any number."
)]
public partial class FullControlPower : NewKunlunPower, ITalismanDetonateListener
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new TalismanDetonateVar<FullControlPower>(power =>
                TalismanDetonateCard.IsUpgradedAnywhere(power.Owner.Player)
            ),
        ];

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        Tip.TalismanDetonateCardWithTips(Owner.Player);

    public override decimal ModifyDamageAdditive(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        CardPlay? cardPlay
    ) => Owner == dealer && cardSource is TalismanDetonateCard ? Amount : 0;

    public async Task<decimal> ConsumeQiCharges(
        PlayerChoiceContext choiceContext,
        Player player,
        CardModel? cardSource
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
            cards.Add(card);
        }

        var selected = await CardSelectCmd.FromChooseACardScreen(choiceContext, cards, player);
        var toSpend = ((QiChargesCard)selected!).QiCharges.BaseValue;

        var actualSpent = await QiChargeCmd.ConsumeQiCharges(
            choiceContext,
            player.Creature,
            toSpend,
            player.Creature,
            cardSource
        );
        return actualSpent;
    }

    decimal ITalismanDetonateListener.BaseDamageAdditiveModifier(
        decimal amount,
        Creature? dealer
    ) => dealer == Owner ? Amount : 0;
}
