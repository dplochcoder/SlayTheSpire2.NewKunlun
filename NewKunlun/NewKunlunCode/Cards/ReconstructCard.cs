using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Gold;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(TokenCardPool))]
[CardLocalization(
    title: "Reconstruct",
    description: "Spend {Cost:inverseDiff()} gold.\nGain {Energy:energyIcons()}.\nReturn to your hand."
)]
public partial class ReconstructCard()
    : NewKunlunCard(0, CardType.Skill, CardRarity.Ancient, TargetType.None)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new GoldVar(nameof(Cost), 25), new EnergyVar(1)];

    protected override void OnUpgrade() => Cost.UpgradeValueTo(20);

    protected override CardLocation GetResultLocationForCardPlay() =>
        new(Owner, PileType.Hand, CardPilePosition.Top);

    protected override bool ShouldGlowRedInternal => Owner.Gold < Cost.IntValue;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (Owner.Gold < Cost.IntValue)
            return;

        await PlayerCmd.LoseGold(Cost.IntValue, Owner, GoldLossType.Spent);
        await PlayerCmd.GainEnergy(Energy.IntValue, Owner);
    }
}
