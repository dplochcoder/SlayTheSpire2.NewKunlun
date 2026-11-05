using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Cards.Statuses;

[Pool(typeof(StatusCardPool))]
[CardLocalization(
    title: "Weary",
    description: "When drawn, gain {Weak} [gold]Weak[/gold] and {Frail} [gold]Frail[/gold]."
)]
public partial class WearyCard()
    : NewKunlunCard(0, CardType.Status, CardRarity.Status, TargetType.None)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [CardKeyword.Retain, CardKeyword.Unplayable];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<WeakPower>(nameof(Weak), 1), new PowerVar<FrailPower>(nameof(Frail), 1)];

    public override int MaxUpgradeLevel => 0;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<WeakPower>(), HoverTipFactory.FromPower<FrailPower>()];

    public override async Task AfterCardDrawn(
        PlayerChoiceContext choiceContext,
        CardModel card,
        bool fromHandDraw
    )
    {
        if (card != this)
            return;

        await PowerCmd.Apply<WeakPower>(
            choiceContext,
            Owner.Creature,
            Weak.IntValue,
            Owner.Creature,
            this
        );
        await PowerCmd.Apply<FrailPower>(
            choiceContext,
            Owner.Creature,
            Frail.IntValue,
            Owner.Creature,
            this
        );
    }
}
