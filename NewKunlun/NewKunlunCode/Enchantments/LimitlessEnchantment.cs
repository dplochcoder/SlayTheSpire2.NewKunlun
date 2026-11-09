using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Enchantments;

[EnchantmentLocalization(
    title: "Limitless",
    description: "This card gains double [gold]Block[/gold]."
)]
public class LimitlessEnchantment : NewKunlunEnchantment
{
    public override bool CanEnchant(CardModel card) => base.CanEnchant(card) && card.GainsBlock;

    public override decimal ModifyBlockMultiplicative(
        Creature target,
        decimal block,
        ValueProp props,
        CardModel? cardSource,
        CardPlay? cardPlay
    ) => (props.IsCardOrMonsterMove() && cardPlay?.Card == Card) ? block * 2 : block;
}
