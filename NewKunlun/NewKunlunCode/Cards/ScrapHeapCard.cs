using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Tips;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Scrap Heap",
    description: "Choose 1 of 3 random cards in your discard pile to transform into {IfUpgraded:show:[green]Dark Steel+[/green]|[gold]Dark Steel[/gold}}."
)]
public class ScrapHeapCard()
    : NewKunlunCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tip.DarkSteelPower()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        IReadOnlyList<CardModel> options =
        [
            .. PileType
                .Discard.GetPile(Owner)
                .Cards.Where(c => c.IsTransformable)
                .ToList()
                .StableShuffle(Owner.RunState.Rng.CombatCardSelection)
                .Take(3),
        ];
        var card = await CardSelectCmd.FromChooseACardScreen(choiceContext, options, Owner);
        if (card == null)
            return;

        var replacement = CombatState!.CreateCard<DarkSteelCard>(Owner);
        if (IsUpgraded)
            CardCmd.Upgrade(replacement);

        await CardCmd.Transform(card, replacement);
    }
}
