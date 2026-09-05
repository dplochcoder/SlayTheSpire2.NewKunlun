using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Powers;
using NewKunlun.NewKunlunCode.Tips;
using NewKunlun.NewKunlunCode.Variables;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Thunder Buster",
    description: "At the end of your next {TurnCount:diff()} turns, deal {Damage:diff()} damage {HitCount:diff()} times to all enemies. Deals damage an additional time each turn for every [gold]Dark Steel[/gold]."
)]
public partial class ThunderBusterCard()
    : NewKunlunCard(0, CardType.Attack, CardRarity.Token, TargetType.AllEnemies),
        IAzureBowArrow
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DynamicVar(nameof(TurnCount), 3M),
            new DamageVar(8M, ValueProp.Unpowered),
            new CustomVar(
                nameof(HitCount),
                3M,
                _ => 3M + Owner.Creature.GetPowerAmount<DarkSteelPower>()
            ),
        ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tip.DarkSteelPower()];

    public async Task OnPlayArrow(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var power = await PowerCmd.Apply<ThunderBusterPower>(
            choiceContext,
            Owner.Creature,
            TurnCount.BaseValue,
            Owner.Creature,
            cardPlay.Card
        );
        if (power == null)
            return;

        power.Damage.BaseValue = Damage.BaseValue;
        power.HitCount.BaseValue = HitCount.Calculate(null);
    }
}
