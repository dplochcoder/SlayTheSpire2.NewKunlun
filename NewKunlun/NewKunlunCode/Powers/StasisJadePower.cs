using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Tips;

namespace NewKunlun.NewKunlunCode.Powers;

[PowerLocalization(
    title: "Stasis Jade",
    description: "Take half damage from enemies who are [gold]Marked[/gold] or were [gold]Detonated[/gold] this turn."
)]
public class StasisJadePower : NewKunlunPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tip.Mark(), Tip.Talisman()];

    public override decimal ModifyDamageMultiplicative(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        CardPlay? cardPlay
    )
    {
        if (
            target == Owner
            && props.IsPoweredAttack()
            && dealer != null
            && (
                dealer.HasPower<TalismanPower>()
                || dealer.HasPower<TalismanDetonatedThisTurnPower>()
            )
        )
            return amount / 2;

        return amount;
    }
}
