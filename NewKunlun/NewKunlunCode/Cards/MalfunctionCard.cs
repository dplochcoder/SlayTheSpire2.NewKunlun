using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Commands;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Powers;
using NewKunlun.NewKunlunCode.Variables;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    "Malfunction",
    "Take {ExhaustDamage:diff()} [gold]Internal Damage[/gold]. If this is in your hand at the end of your turn, take {EndTurnDamage:diff()} [gold]Internal Damage[/gold] and increase damage values by {DamageIncrement}."
)]
public partial class MalfunctionCard()
    : NewKunlunCard(1, CardType.Status, CardRarity.Status, TargetType.Self)
{
    public override int MaxUpgradeLevel => 0;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new InternalDamageVar(nameof(EndTurnDamage), 4M),
            new InternalDamageVar(nameof(ExhaustDamage), 8M),
            new DynamicVar(nameof(DamageIncrement), 4M),
        ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [InternalDamagePower.HoverTip()];

    public override bool HasTurnEndInHandEffect => true;

    protected override async Task OnTurnEndInHand(PlayerChoiceContext choiceContext)
    {
        await InternalDamageCmd.Apply(choiceContext, Owner.Creature, EndTurnDamage, null, this);

        EndTurnDamage.BaseValue += DamageIncrement.BaseValue;
        ExhaustDamage.BaseValue += DamageIncrement.BaseValue;
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await InternalDamageCmd.Apply(choiceContext, Owner.Creature, ExhaustDamage, null, this);
        await CardCmd.Exhaust(choiceContext, this);
    }
}
