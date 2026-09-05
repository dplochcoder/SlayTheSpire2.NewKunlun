using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Powers;
using NewKunlun.NewKunlunCode.Tips;
using NewKunlun.NewKunlunCode.Variables;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Rhythm Chop",
    description: "Whenever you have 3 or more [gold]Qi Charges[/gold] at the start of your turn, pull {TalismanDash:cardName()} into your hand."
)]
public partial class RhythmChopCard()
    : NewKunlunCard(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new TalismanDashVar<RhythmChopCard>(card =>
                TalismanDashCard.IsUpgradedAnywhere(card.Owner)
            ),
        ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [Tip.QiCharge(), Tip.TalismanDashCard(Owner)];

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<RhythmChopPower>(
            choiceContext,
            Owner.Creature,
            1M,
            Owner.Creature,
            this
        );
    }
}
