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
using NewKunlun.NewKunlunCode.Variables;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Shadow Hunter",
    description: "Deal {Damage:diff()} damage {HitCount:diff()} times. Deals damage one additional time for every [gold]Dark Steel[/gold]."
)]
public partial class ShadowHunterCard()
    : NewKunlunCard(0, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy),
        IAzureBowArrow
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DamageVar(13M, ValueProp.Move),
            new CustomVar<ShadowHunterCard>(
                nameof(HitCount),
                5M,
                (card, _) => 5M + card.Owner.Creature.GetPowerAmount<DarkSteelPower>()
            ),
        ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tip.DarkSteelPower()];

    public async Task OnPlayArrow(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd
            .Attack(Damage.BaseValue)
            .FromCard(cardPlay.Card, cardPlay)
            .WithHitCount((int)HitCount.Calculate(cardPlay.Target!))
            .WithHeavySlashVfx()
            .Targeting(cardPlay.Target!)
            .Execute(choiceContext);
    }
}
