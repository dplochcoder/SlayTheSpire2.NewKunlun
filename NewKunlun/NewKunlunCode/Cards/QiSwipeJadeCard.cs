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

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Qi Swipe Jade",
    description: "Increase [gold]Qi Charge[/gold] capacity by {CapacityIncrease:diff()}.\nWhenever you successfully [gold]Parry[/gold], gain an extra [gold]Qi Charge[/gold]."
)]
public partial class QiSwipeJadeCard()
    : NewKunlunCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar(nameof(CapacityIncrease), 1M)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tip.QiCharge(), Tip.Parry()];

    protected override void OnUpgrade() => CapacityIncrease.UpgradeValueTo(2M);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await QiChargeCmd.IncreaseQiChargeCapacity(
            choiceContext,
            Owner.Creature,
            2M,
            Owner.Creature,
            this
        );
        await PowerCmd.Apply<QiSwipeJadePower>(
            choiceContext,
            Owner.Creature,
            1M,
            Owner.Creature,
            this
        );
    }
}
