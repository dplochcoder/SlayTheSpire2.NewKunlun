using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Powers;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(TokenCardPool))]
[CardLocalization(
    title: "Dragon's Whip",
    description: "Apply {Poison:diff()} [gold]Poison[/gold].\nIf the target dies to [gold]Poison[/gold], permanently increase this value by {Increase:diff()}."
)]
public partial class DragonsWhipCard()
    : NewKunlunCard(2, CardType.Skill, CardRarity.Ancient, TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<PoisonPower>(nameof(Poison), 8), new(nameof(Increase), 3)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<PoisonPower>()];

    protected override void OnUpgrade() => Increase.UpgradeValueTo(4);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target == null)
            return;

        await PowerCmd.Apply<PoisonPower>(
            choiceContext,
            cardPlay.Target,
            Poison.IntValue,
            Owner.Creature,
            this
        );
        await PowerCmd.Apply<DragonsWhipPower>(
            choiceContext,
            cardPlay.Target,
            1,
            Owner.Creature,
            this
        );
    }
}
