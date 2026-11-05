using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Powers;

namespace NewKunlun.NewKunlunCode.Cards.Ancients;

[Pool(typeof(EventCardPool))]
[CardLocalization(
    title: "Unto Legend",
    description: "Become [gold]Heroic[/gold].\nShuffle a copy of this card into a random ally's draw pile.\nIf this card is exhausted or transformed, all players lose [gold]Heroic[/gold]."
)]
public class UntoLegendCard()
    : NewKunlunCard(1, CardType.Power, CardRarity.Ancient, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Ethereal];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            HoverTipFactory.FromPower<HeroicPower>(),
            HoverTipFactory.FromPower<StrengthPower>(),
            HoverTipFactory.FromPower<DexterityPower>(),
        ];

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (Owner.Creature.CombatState == null)
            return;

        await PowerCmd.Apply<HeroicPower>(choiceContext, Owner.Creature, 1, Owner.Creature, this);

        List<Player> allies =
        [
            .. Owner.Creature.CombatState.Players.Where(p => p != Owner && p.Creature.IsAlive),
        ];
        var ally = Owner.Creature.CombatState.RunState.Rng.CombatTargets.NextItem(allies);
        if (ally == null)
            return;
        await CardPileCmd.Add(
            CreateCloneForPlayer(ally),
            PileType.Draw.GetPile(ally),
            CardPilePosition.Random,
            clonedBy: this
        );
    }

    public override async Task AfterTransformedFromAsync()
    {
        if (CombatState == null)
            return;

        foreach (var player in CombatState.Players)
            await PowerCmd.Remove<HeroicPower>(player.Creature);
    }

    public override async Task AfterCardExhausted(
        PlayerChoiceContext choiceContext,
        CardModel card,
        bool causedByEthereal
    )
    {
        if (card != this || CombatState == null)
            return;

        foreach (var player in CombatState.Players)
            await PowerCmd.Remove<HeroicPower>(player.Creature);
    }
}
