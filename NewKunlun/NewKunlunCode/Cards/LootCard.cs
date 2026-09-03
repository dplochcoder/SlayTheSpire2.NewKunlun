using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Tips;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Loot",
    description: "Add 3 {IfUpgraded:show:[green]Azure Sand+[/green]|[gold]Azure Sand[/gold]} and 1 [IfUpgraded:show:[green]Dark Steel+[/green]|[gold]Dark Steel[/gold]} into your discard pile."
)]
public class LootCard() : NewKunlunCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tip.AzureSandPower()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        List<CardModel> cards = [];
        for (var i = 0; i < 3; i++)
            cards.Add(CombatState!.CreateCard<AzureSandCard>(Owner, upgrade: IsUpgraded));
        cards.Add(CombatState!.CreateCard<DarkSteelCard>(Owner, upgrade: IsUpgraded));

        foreach (
            var preview in await CardPileCmd.AddGeneratedCardsToCombat(
                cards,
                PileType.Discard,
                Owner,
                CardPilePosition.Random
            )
        )
            CardCmd.PreviewCardPileAdd(preview);
    }
}
