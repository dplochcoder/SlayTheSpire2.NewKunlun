using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Orbs;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Cards.Ancients;

[Pool(typeof(DefectCardPool))]
[CardLocalization(
    title: "Fork Bomb",
    description: "Channel 1 [gold]Lightning[/gold] for each orb slot.\nAdd a copy of this card to your draw and discard piles."
)]
public class ForkBombCard() : NewKunlunCard(1, CardType.Skill, CardRarity.Ancient, TargetType.None)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromOrb<LightningOrb>()];

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (Owner.PlayerCombatState == null)
            return;

        var orbs = Owner.PlayerCombatState.OrbQueue.Capacity;
        for (var i = 0; i < orbs; i++)
            await OrbCmd.Channel<LightningOrb>(choiceContext, Owner);

        await Owner.AddGeneratedCardToPile(
            CreateClone(),
            PileType.Draw,
            position: CardPilePosition.Random
        );
        await Owner.AddGeneratedCardToPile(CreateClone(), PileType.Discard);
    }
}
