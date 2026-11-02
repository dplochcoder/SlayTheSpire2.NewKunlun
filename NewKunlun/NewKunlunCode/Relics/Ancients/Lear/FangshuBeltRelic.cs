using BaseLib.Hooks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics.Ancients.Lear;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Fangshu Belt",
    description: "Gain {Energy:energyIcons()} energy each turn.\nYour maximum hand size is reduced by {MaxHandSizeLoss}.",
    flavor: ""
)]
public partial class FangshuBeltRelic : NewKunlunRelic, IMaxHandSizeModifier
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new EnergyVar(1), new(nameof(MaxHandSizeLoss), 2)];

    public override decimal ModifyMaxEnergy(Player player, decimal amount) =>
        player == Owner ? amount + Energy.IntValue : amount;

    int IMaxHandSizeModifier.ModifyMaxHandSize(Player player, int currentMaxHandSize) =>
        player == Owner
            ? Math.Max(0, currentMaxHandSize - MaxHandSizeLoss.IntValue)
            : currentMaxHandSize;
}
