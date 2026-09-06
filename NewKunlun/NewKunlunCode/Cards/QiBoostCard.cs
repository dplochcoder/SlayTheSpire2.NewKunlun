using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Powers;
using NewKunlun.NewKunlunCode.Tips;
using NewKunlun.NewKunlunCode.Variables;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Qi Boost",
    description: "[gold]Boost[/gold] {Boost:diff()}.\nNext turn, pull {TalismanDash:cardName()} into your hand."
)]
public partial class QiBoostCard()
    : NewKunlunCard(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DynamicVar(nameof(Boost), 7),
            new CardNameVar<TalismanDashCard>(() => TalismanDashCard.IsUpgradedAnywhere(Owner)),
        ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [Tip.Boost(), Tip.TalismanDashCard(Owner)];

    protected override void OnUpgrade() => Boost.UpgradeValueTo(10M);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<BoostPower>(
            choiceContext,
            Owner.Creature,
            Boost.BaseValue,
            Owner.Creature,
            this
        );
    }
}
