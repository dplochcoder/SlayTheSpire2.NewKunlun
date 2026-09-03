using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Commands;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Powers;
using NewKunlun.NewKunlunCode.Variables;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Regroup",
    description: "Heal {InternalDamageHeal:diff()} [gold]Internal Damage[/gold]. Keep your energy this turn."
)]
public partial class RegroupCard()
    : NewKunlunCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new InternalDamageHealVar(6M)];

    protected override void OnUpgrade() => RemoveKeyword(CardKeyword.Exhaust);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await InternalDamageCmd.Heal(
            choiceContext,
            Owner.Creature,
            InternalDamageHeal,
            Owner.Creature,
            this
        );
        await PowerCmd.Apply<RegroupPower>(choiceContext, Owner.Creature, 1M, Owner.Creature, this);
    }
}
