using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Cards.Ancients;

[Pool(typeof(EventCardPool))]
[CardLocalization(
    title: "Mysterious Seed",
    description: "Transform this card into a copy of the last card you played this combat, then play it."
)]
public class MysteriousSeedCard()
    : NewKunlunCard(0, CardType.Skill, CardRarity.Ancient, TargetType.None)
{
    private CardModel LastCardPlayed() =>
        CombatManager
            .Instance.History.Entries.OfType<CardPlayFinishedEntry>()
            .Where(e => e.CardPlay.Player == Owner)
            .Select(e => e.CardPlay.Card)
            .FirstOrDefault()
        ?? ModelDb.Card<NothingCard>();

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromCard(LastCardPlayed())];

    protected override void OnUpgrade() => AddKeyword(CardKeyword.Retain);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var clone = LastCardPlayed().CreateClone();
        CardCmd.PreviewCardPileAdd((await CardCmd.Transform(this, clone))!.Value);
        await CardCmd.AutoPlay(choiceContext, clone, null);
    }
}
