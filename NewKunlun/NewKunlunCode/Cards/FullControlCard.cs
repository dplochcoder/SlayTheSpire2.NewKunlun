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
    title: "Full Control",
    description: "[gold]Boost[/gold] {Boost:diff()}.\nChoose how many [gold]Qi Charges[/gold] to [gold]Discharge[/gold] when [gold]Detonating[/gold], without limit."
)]
public partial class FullControlCard()
    : NewKunlunCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DynamicVar(nameof(Boost), 1M),
            new CardNameVar<TalismanDetonateCard>(() =>
                TalismanDetonateCard.IsUpgradedAnywhere(Owner)
            ),
        ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [Tip.Boost(), Tip.QiCharge(), Tip.Discharge(), Tip.Detonate()];

    protected override void OnUpgrade() => Boost.UpgradeValueTo(3M);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<BoostPower>(
            choiceContext,
            Owner.Creature,
            Boost.BaseValue,
            Owner.Creature,
            this
        );
        await PowerCmd.Apply<FullControlPower>(
            choiceContext,
            Owner.Creature,
            1M,
            Owner.Creature,
            this
        );
    }
}
