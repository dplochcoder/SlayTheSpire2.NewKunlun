using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using NewKunlun.NewKunlunCode.Cards;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Powers;

[PowerLocalization(
    title: "Parrying Strike",
    description: "The next {Amount:cond:>1?{Amount} [gold]Parry Cards[/gold]|[gold]Parry Card[/gold]} you play {Amount:cond:>1?are|is} free."
)]
public class ParryingStrikePower : NewKunlunPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tips.ParryCardKeyword()];

    private bool ShouldCardBeFree(CardModel card) =>
        card.Pile?.Type is PileType.Hand or PileType.Play
        && card.Owner.Creature == Owner
        && card is NewKunlunCard { IsParryCard: true };

    public override bool TryModifyEnergyCostInCombatLate(
        CardModel card,
        decimal originalCost,
        out decimal modifiedCost
    )
    {
        if (ShouldCardBeFree(card))
        {
            modifiedCost = 0M;
            return true;
        }

        modifiedCost = originalCost;
        return false;
    }

    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (ShouldCardBeFree(cardPlay.Card))
            await PowerCmd.Decrement(this);
    }
}
