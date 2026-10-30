using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Tips;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(CurseCardPool))]
[CardLocalization(
    title: "Tianhuo",
    description: "On draw, add a [gold]Tianhuo[/gold] to your discard pile."
)]
public class TianhuoCard() : NewKunlunCard(1, CardType.Curse, CardRarity.Curse, TargetType.None)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    public override int MaxUpgradeLevel => 0;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromCard<TianhuoCard>()];

    private static readonly IReadOnlyList<PileType> PileTypes =
    [
        PileType.Hand,
        PileType.Draw,
        PileType.Discard,
    ];

    public override async Task AfterCardDrawn(
        PlayerChoiceContext choiceContext,
        CardModel card,
        bool fromHandDraw
    )
    {
        if (card != this)
            return;

        await Owner.AddGeneratedCardToPile<TianhuoCard>(PileType.Discard);
    }
}
