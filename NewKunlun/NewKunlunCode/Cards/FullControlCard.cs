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
    title: "Full Control",
    description: "[gold]Boost[/gold] {Boost:diff()}.\nYou choose how many [gold]Qi Charges[/gold] to [gold]Discharge[/gold], without limit."
)]
public partial class FullControlCard()
    : NewKunlunCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DamageVar(nameof(Boost), 2M, ValueProp.Unpowered),
            new CardNameVar<TalismanDetonateCard>(() =>
                TalismanDetonateCard.IsUpgradedAnywhere(Owner)
            ),
        ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [Tip.TalismanDetonateCard(Owner), Tip.Discharge(), Tip.QiCharge()];

    protected override void OnUpgrade() => Boost.UpgradeValueTo(4M);

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
