using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Powers;
using NewKunlun.NewKunlunCode.Tips;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Azure Bow",
    description: "Spend 1 [gold]Azure Sand Magazine[/gold] to fire an [gold]Arrow[/gold].\nReturn to your hand."
)]
public class AzureBowCard()
    : NewKunlunCard(2, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            HoverTipFactory.FromPower<AzureSandMagazinePower>(),
            HoverTipFactory.FromCard<CloudPiercerCard>(),
            HoverTipFactory.FromCard<ShadowHunterCard>(),
            HoverTipFactory.FromCard<ThunderBusterCard>(),
        ];

    protected override bool IsPlayable =>
        Owner.Creature.GetPowerAmount<AzureSandMagazinePower>() > 0;

    protected override bool ShouldGlowGoldInternal => IsPlayable;

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);

    protected override CardLocation GetResultLocationForCardPlay()
    {
        var loc = base.GetResultLocationForCardPlay();
        if (loc.pileType == PileType.Discard)
            loc.pileType = PileType.Hand;
        return loc;
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (
            CombatState == null
            || Owner.Creature.GetPower<AzureSandMagazinePower>() is not { } azureSandMagazinePower
        )
            return;

        await PowerCmd.Decrement(azureSandMagazinePower);
        azureSandMagazinePower.Flash();

        var card = await CardSelectCmd.FromChooseACardScreen(
            choiceContext,
            [
                CombatState.CreateCard<CloudPiercerCard>(Owner),
                CombatState.CreateCard<ThunderBusterCard>(Owner),
                CombatState.CreateCard<ShadowHunterCard>(Owner),
            ],
            Owner
        );
        if (card is not IAzureBowArrow arrow)
            return;

        await arrow.OnPlayArrow(choiceContext, cardPlay);
    }
}
