using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using NewKunlun.NewKunlunCode.Commands;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Powers;
using NewKunlun.NewKunlunCode.Variables;

namespace NewKunlun.NewKunlunCode.Cards.Statuses;

[Pool(typeof(StatusCardPool))]
[CardLocalization(
    title: "Malfunction",
    description: "Draw 1 card.\nIf this is in your hand at the end of your turn, take {Damage:inverseDiff()} [gold]Internal Damage[/gold] and increase damage by {DamageIncrement:inverseDiff()}."
)]
public partial class MalfunctionCard()
    : NewKunlunCard(1, CardType.Status, CardRarity.Status, TargetType.Self)
{
    public override int MaxUpgradeLevel => 0;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new InternalDamageSelfInflictVar(nameof(Damage), 4M), new(nameof(DamageIncrement), 3M)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<InternalDamagePower>()];

    public override bool HasTurnEndInHandEffect => true;

    protected override async Task OnTurnEndInHand(PlayerChoiceContext choiceContext)
    {
        await InternalDamageCmd.Inflict(choiceContext, Owner.Creature, Damage, null, this);
        Damage.BaseValue += DamageIncrement.BaseValue;
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay) =>
        await CardPileCmd.Draw(choiceContext, Owner);
}
