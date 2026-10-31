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
using NewKunlun.NewKunlunCode.Variables;

namespace NewKunlun.NewKunlunCode.Cards.Yi;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Cloud Piercer",
    description: "Remove all enemy [gold]Block[/gold].\nApply {Weak} [gold]Weak[/gold] and {Vulnerable} [gold]Vulnerable[/gold].\nDeal {Damage:diff()} damage.\n[gold]Sharpening[/gold] deals {ExtraDamage:diff()} additional damage."
)]
public partial class CloudPiercerCard()
    : NewKunlunCard(0, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy),
        IAzureBowArrow
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new(nameof(Weak), 2M),
            new(nameof(Vulnerable), 2M),
            new CustomDamageVar<CloudPiercerCard>(
                nameof(Damage),
                36M,
                ValueProp.Move,
                _ => 36M + ExtraDamage.BaseValue * Owner.Creature.GetPowerAmount<SharpenedPower>()
            ),
            new(nameof(ExtraDamage), 8M),
        ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            HoverTipFactory.Static(StaticHoverTip.Block),
            HoverTipFactory.FromPower<WeakPower>(),
            HoverTipFactory.FromPower<VulnerablePower>(),
            HoverTipFactory.Static(CustomStaticHoverTip.Sharpen),
        ];

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
            .Attack((decimal)Damage.Calculate(target))
            .FromCard(cardPlay.Card, cardPlay)
            .Targeting(target)
            .WithHeavySlashVfx()
            .Execute(choiceContext);
    }
}
