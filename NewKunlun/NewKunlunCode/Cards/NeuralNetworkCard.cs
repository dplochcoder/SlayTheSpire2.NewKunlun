using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Powers;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Neural Network",
    description: "Whenever you take [gold]Internal Damage[/gold], gain that much block."
)]
public class NeuralNetworkCard()
    : NewKunlunCard(2, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tips.Power<InternalDamagePower>()];

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<NeuralNetworkPower>(
            choiceContext,
            Owner.Creature,
            1M,
            Owner.Creature,
            this
        );
    }
}
