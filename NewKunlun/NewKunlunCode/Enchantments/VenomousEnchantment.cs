using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Tips;

namespace NewKunlun.NewKunlunCode.Enchantments;

[EnchantmentLocalization(
    title: "Venomous",
    description: "Apply {Poison} [gold]Poison[/gold] per hit.",
    extraCardText: "Apply {Poison} [gold]Poison[/gold] per hit."
)]
public partial class VenomousEnchantment : NewKunlunEnchantment
{
    public override bool CanEnchantCardType(CardType cardType) => cardType == CardType.Attack;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<PoisonPower>(nameof(Poison), 4)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tip.Poison()];

    public override async Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource
    )
    {
        if (cardSource != Card || !props.IsPoweredAttack())
            return;

        await PowerCmd.Apply<PoisonPower>(
            choiceContext,
            target,
            Poison.BaseValue,
            dealer,
            cardSource
        );
    }

    public override void RecalculateValues() => Poison.BaseValue = Amount;
}
