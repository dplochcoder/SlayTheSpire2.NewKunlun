using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SlayTheSpire2.NewKunlun.SlayTheSpire2.NewKunlunCode.Character;
using SlayTheSpire2.NewKunlun.SlayTheSpire2.NewKunlunCode.Powers;
using SlayTheSpire2.NewKunlun.SlayTheSpire2.NewKunlunCode.Variables;

namespace SlayTheSpire2.NewKunlun.SlayTheSpire2.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
public class MalfuctionCard()
    : NewKunlunCard(1, CardType.Status, CardRarity.Status, TargetType.Self)
{
    private const string EndTurnDamage = nameof(EndTurnDamage);
    private const string ExhaustDamage = nameof(ExhaustDamage);

    public override int MaxUpgradeLevel => 0;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new InternalDamageVar(EndTurnDamage, 4), new InternalDamageVar(ExhaustDamage, 8)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    public override bool HasTurnEndInHandEffect => true;

    protected override async Task OnTurnEndInHand(PlayerChoiceContext choiceContext)
    {
        await InternalDamagePower.Apply(
            choiceContext,
            Owner.Creature,
            DynamicVars[EndTurnDamage].BaseValue,
            null,
            this
        );
        DynamicVars[EndTurnDamage].BaseValue += 4;
        DynamicVars[ExhaustDamage].BaseValue += 4;
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await InternalDamagePower.Apply(
            choiceContext,
            Owner.Creature,
            DynamicVars[ExhaustDamage].BaseValue,
            null,
            this
        );
        await CardCmd.Exhaust(choiceContext, this);
    }
}
