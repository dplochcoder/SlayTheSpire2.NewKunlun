using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Keywords;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Tips;
using NewKunlun.NewKunlunCode.Variables;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Loot",
    description: "Add 2 {AzureSand:cardName()} and 1 {DarkSteel:cardName()} into your discard pile."
)]
public partial class LootCard() : NewKunlunCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CustomCardKeyword.Ephemeral];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new CardNameVar<AzureSandCard>(() => IsUpgraded),
            new CardNameVar<DarkSteelCard>(() => IsUpgraded),
        ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            Tip.AzureSand(upgrade: false),
            Tip.AzureSand(upgrade: true),
            Tip.DarkSteel(upgrade: IsUpgraded),
        ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CombatState == null)
            return;

        foreach (
            var preview in await CardPileCmd.AddGeneratedCardsToCombat(
                [
                    CombatState.CreateUpgradedCard<AzureSandCard>(Owner, IsUpgraded),
                    CombatState.CreateUpgradedCard<AzureSandCard>(Owner, IsUpgraded),
                    CombatState.CreateUpgradedCard<DarkSteelCard>(Owner, upgrade: IsUpgraded),
                ],
                PileType.Discard,
                Owner,
                CardPilePosition.Random
            )
        )
            CardCmd.PreviewCardPileAdd(preview);
    }
}
