using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Powers;
using NewKunlun.NewKunlunCode.Tips;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Transformer",
    description: "At the end of your turn, transform unspent {1:energyIcons()} into [gold]Qi Charges[/gold]."
)]
public partial class TransformerCard()
    : NewKunlunCard(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tip.QiCharge()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<TransformerPower>(
            choiceContext,
            Owner.Creature,
            1M,
            Owner.Creature,
            this
        );
    }
}
