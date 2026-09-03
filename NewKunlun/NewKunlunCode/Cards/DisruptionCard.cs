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
using NewKunlun.NewKunlunCode.Tips;
using NewKunlun.NewKunlunCode.Variables;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Disruption",
    description: "Inflict {InternalDamageInflict:diff()} [gold]Internal Damage[/gold] and {Imperfect:diff()} [gold]Imperfect[/gold] to all enemies."
)]
public partial class DisruptionCard()
    : NewKunlunCard(1, CardType.Skill, CardRarity.Common, TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new InternalDamageInflictVar(6M), new DynamicVar(nameof(Imperfect), 5M)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [Tip.InternalDamage(), Tip.Imperfect()];

    protected override void OnUpgrade()
    {
        InternalDamageInflict.UpgradeValueTo(8M);
        Imperfect.UpgradeValueTo(7M);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await InternalDamageCmd.Inflict(
            choiceContext,
            CombatState!.HittableEnemies,
            InternalDamageInflict,
            Owner.Creature,
            this
        );
        await PowerCmd.Apply<ImperfectPower>(
            choiceContext,
            CombatState!.HittableEnemies,
            Imperfect.BaseValue,
            Owner.Creature,
            this
        );
    }
}
