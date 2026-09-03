using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Powers;
using NewKunlun.NewKunlunCode.Tips;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Shadow Hunter",
    description: "Deal {CalculatedDamage:diff()} damage {HitCount} times. Deals damage an additional {ExtraDamage:diff()} times for every [gold]Dark Steel[/gold]."
)]
public partial class ShadowHunterCard()
    : NewKunlunCard(0, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy),
        IAzureBowArrow
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new CalculationBaseVar(12M),
            new ExtraDamageVar(3M),
            new CalculatedDamageVar(ValueProp.Move).WithMultiplier(
                (card, _) => card.Owner.Creature.GetPowerAmount<DarkSteelPower>()
            ),
            new DynamicVar(nameof(HitCount), 5M),
        ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tip.DarkSteelPower()];

    public async Task OnPlayArrow(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd
            .Attack(CalculatedDamage.Calculate(cardPlay.Target!))
            .FromCard(cardPlay.Card, cardPlay)
            .WithHitCount((int)HitCount.BaseValue)
            .WithHeavySlashVfx()
            .Targeting(cardPlay.Target!)
            .Execute(choiceContext);
    }
}
