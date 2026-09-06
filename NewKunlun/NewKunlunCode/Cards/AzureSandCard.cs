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
using NewKunlun.NewKunlunCode.Powers;
using NewKunlun.NewKunlunCode.Tips;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Azure Sand",
    description: "[gold]Reload[/gold] {ReloadCount:diff()}.\nDraw 1 card."
)]
public partial class AzureSandCard()
    : NewKunlunCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [CardKeyword.Ethereal, CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar(nameof(ReloadCount), 1M)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tip.Reload(), Tip.AzureBow()];

    protected override void OnUpgrade() => ReloadCount.UpgradeValueTo(2M);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<AzureSandMagazinePower>(
            choiceContext,
            Owner.Creature,
            ReloadCount.BaseValue,
            Owner.Creature,
            this
        );

        var bow = Owner.FindCard<AzureBowCard>([PileType.Draw, PileType.Hand, PileType.Discard]);
        if (bow == null)
            await this.AddGeneratedCardToPile<AzureBowCard>(
                PileType.Hand,
                position: CardPilePosition.Top
            );

        await CardPileCmd.Draw(choiceContext, Owner);
    }
}
