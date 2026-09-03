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
    title: "Dark Steel",
    description: "Gain {Amount:diff()} [gold]Dark Steel[/gold]."
)]
public partial class DarkSteelCard()
    : NewKunlunCard(2, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar(nameof(Amount), 2M)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tip.DarkSteelPower()];

    protected override void OnUpgrade() => Amount.UpgradeValueTo(3M);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<DarkSteelPower>(
            choiceContext,
            Owner.Creature,
            Amount.BaseValue,
            Owner.Creature,
            this
        );
    }
}
