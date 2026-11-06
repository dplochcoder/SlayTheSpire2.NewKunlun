using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Cards.Ancients;

[Pool(typeof(EventCardPool))]
[CardLocalization(title: "Bloom", description: "Exhaust all your Curses and Statuses.")]
public class BloomCard() : NewKunlunCard(1, CardType.Skill, CardRarity.Ancient, TargetType.None)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [CardKeyword.Exhaust, CardKeyword.Retain];

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        List<CardModel> cards =
        [
            .. Owner.PlayerCombatState?.AllCards.Where(c =>
                c.Type is CardType.Status or CardType.Curse && c.Pile?.Type != PileType.Exhaust
            ) ?? [],
        ];
        foreach (var card in cards)
            await CardCmd.Exhaust(choiceContext, card);
    }
}
