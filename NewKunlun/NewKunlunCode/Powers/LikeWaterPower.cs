using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using NewKunlun.NewKunlunCode.Cards;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Variables;

namespace NewKunlun.NewKunlunCode.Powers;

[PowerLocalization(
    title: "Like Water",
    description: "{TalismanDetonate:cardName} costs {Amount} less energy."
)]
public partial class LikeWaterPower : NewKunlunPower
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new TalismanDetonateVar<LikeWaterPower>(power =>
                TalismanDetonateCard.IsUpgradedAnywhere(power.Owner.Player)
            ),
        ];

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool TryModifyEnergyCostInCombatLate(
        CardModel card,
        decimal originalCost,
        out decimal modifiedCost
    )
    {
        modifiedCost = originalCost;
        if (card.Owner.Creature != Owner || card is not TalismanDetonateCard)
            return false;

        if (card.Pile?.Type is PileType.Hand or PileType.Play)
        {
            modifiedCost = Math.Max(originalCost - Amount, 0);
            return true;
        }

        return false;
    }
}
