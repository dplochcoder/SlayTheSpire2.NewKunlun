using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Commands;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Powers;
using NewKunlun.NewKunlunCode.Variables;

namespace NewKunlun.NewKunlunCode.Cards.Yi;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Breathing Exercise",
    description: "Heal {InternalDamageHeal:diff()} [gold]Internal Damage[/gold].\nDraw 1 card."
)]
public partial class BreathingExerciseCard()
    : NewKunlunCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new InternalDamageHealVar(10M)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<InternalDamagePower>()];

    protected override void OnUpgrade() => InternalDamageHeal.UpgradeValueTo(16M);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await InternalDamageCmd.Heal(
            choiceContext,
            Owner.Creature,
            InternalDamageHeal,
            Owner.Creature,
            this
        );
        await CardPileCmd.Draw(choiceContext, Owner);
    }
}
