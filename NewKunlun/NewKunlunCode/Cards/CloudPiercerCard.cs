using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Powers;
using NewKunlun.NewKunlunCode.Tips;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Cloud Piercer",
    description: "Remove all enemy block. Apply {Weak} [gold]Weak[/gold] and {Vulnerable} [gold]Vulnerable[/gold]. Deal {CalculatedDamage} damage. Deals {ExtraDamage} additional damage for every [icon]Dark Steel[/icon]."
)]
public partial class CloudPiercerCard()
    : NewKunlunCard(0, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy),
        IAzureBowArrow
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DynamicVar(nameof(Weak), 3M),
            new DynamicVar(nameof(Vulnerable), 3M),
            new CalculationBaseVar(25M),
            new ExtraDamageVar(5M),
            new CalculatedDamageVar(ValueProp.Move).WithMultiplier(
                (card, _) => card.Owner.Creature.GetPowerAmount<DarkSteelPower>()
            ),
        ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [Tip.Weak(), Tip.Vulnerable(), Tip.DarkSteelPower()];

    public async Task OnPlayArrow(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var target = cardPlay.Target!;
        await CreatureCmd.LoseBlock(choiceContext, target, target.Block, Owner.Creature);
        await PowerCmd.Apply<WeakPower>(
            choiceContext,
            target,
            Weak.BaseValue,
            Owner.Creature,
            cardPlay.Card
        );
        await PowerCmd.Apply<VulnerablePower>(
            choiceContext,
            target,
            Vulnerable.BaseValue,
            Owner.Creature,
            cardPlay.Card
        );
        await DamageCmd
            .Attack(CalculatedDamage.Calculate(target))
            .FromCard(cardPlay.Card, cardPlay)
            .Targeting(target)
            .WithHeavySlashVfx()
            .Execute(choiceContext);
    }
}
