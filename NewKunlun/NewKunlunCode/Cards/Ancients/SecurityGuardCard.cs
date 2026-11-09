using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Cards.Ancients;

[Pool(typeof(EventCardPool))]
[CardLocalization(
    title: "Security Guard",
    description: "Gain {Block:diff()} [gold]Block[/gold].\nDraw {Cards:diff()} {Cards:plural:card|cards}."
)]
public partial class SecurityGuardCard()
    : NewKunlunCard(0, CardType.Skill, CardRarity.Ancient, TargetType.None)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [CardKeyword.Exhaust, CardKeyword.Innate, CardKeyword.Retain];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(15, ValueProp.Move), new(nameof(Cards), 1)];

    protected override void OnUpgrade()
    {
        Block.UpgradeValueTo(20);
        Cards.UpgradeValueTo(2);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, Block, cardPlay);
        await CardPileCmd.Draw(choiceContext, Cards.IntValue, Owner);
    }
}
