using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Powers;

namespace NewKunlun.NewKunlunCode.Cards.Ancients;

[Pool(typeof(EventCardPool))]
[CardLocalization(
    title: "Verse",
    description: "Gain 1 [gold]Quarter-Note[/gold].\nIf this is the first [gold]Verse[/gold] played this turn, draw {DrawCards:diff()} {DrawCards:plural:card|cards}."
)]
public partial class VerseCard()
    : NewKunlunCard(0, CardType.Skill, CardRarity.Ancient, TargetType.None)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new(nameof(DrawCards), 1)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<QuarterNotePower>()];

    protected override void OnUpgrade() => DrawCards.UpgradeValueTo(2);

    private int NotesThisTurn =>
        CombatManager
            .Instance.History.Entries.OfType<CardPlayFinishedEntry>()
            .Where(e =>
                e.HappenedThisTurn(Owner.Creature.CombatState) && e.CardPlay.Card is VerseCard
            )
            .Select(e => e.CardPlay.PlayCount)
            .DefaultIfEmpty()
            .Sum();

    protected override bool ShouldGlowGoldInternal => NotesThisTurn == 0;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<QuarterNotePower>(
            choiceContext,
            Owner.Creature,
            1,
            Owner.Creature,
            this
        );
        if (NotesThisTurn == 0)
            await CardPileCmd.Draw(choiceContext, DrawCards.IntValue, Owner);
    }
}
