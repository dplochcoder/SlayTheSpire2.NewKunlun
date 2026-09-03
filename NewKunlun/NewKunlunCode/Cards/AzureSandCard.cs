using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Cards;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Powers;
using NewKunlun.NewKunlunCode.Tips;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Azure Sand",
    description: "Gain 1 [gold]Azure Sand[/gold].{IfUpgraded:show: [green]Draw 1 card.[/green]|}"
)]
public class AzureSandCard() : NewKunlunCard(1, CardType.Skill, CardRarity.Token, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tip.AzureSandPower()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<AzureSandPower>(
            choiceContext,
            Owner.Creature,
            1M,
            Owner.Creature,
            this
        );

        var bow = Owner.FindCard<AzureBowCard>([PileType.Draw, PileType.Hand, PileType.Discard]);
        if (bow == null)
            await this.AddGeneratedCardToPile<AzureBowCard>(
                PileType.Hand,
                position: CardPilePosition.Top
            );

        if (IsUpgraded)
            await CardPileCmd.Draw(choiceContext, Owner);
    }
}
