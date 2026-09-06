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

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Regenerate",
    description: "{IfUpgraded:show:[gold]Boost[/gold] {Boost:diff()}.\n|}Whenever you [gold]Detonate[/gold], gain 1 [gold]Qi Charge[/gold]."
)]
public partial class RegenerateCard()
    : NewKunlunCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar(nameof(Boost), 0M)];

    private IEnumerable<IHoverTip> BoostTip() => IsUpgraded ? [Tip.Boost()] : [];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [.. BoostTip(), Tip.Detonate(), Tip.QiCharge()];

    protected override void OnUpgrade() => Boost.UpgradeValueTo(2M);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<BoostPower>(
            choiceContext,
            Owner.Creature,
            Boost.BaseValue,
            Owner.Creature,
            this
        );
        await PowerCmd.Apply<RegeneratePower>(
            choiceContext,
            Owner.Creature,
            1M,
            Owner.Creature,
            this
        );
    }
}
