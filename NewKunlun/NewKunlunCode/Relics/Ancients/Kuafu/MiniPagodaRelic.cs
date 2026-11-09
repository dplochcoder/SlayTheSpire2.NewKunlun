using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics.Ancients.Kuafu;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Mini Pagoda",
    description: "The first two Powers you play each combat are free.",
    flavor: ""
)]
public class MiniPagodaRelic : FreeCardsThisCombatRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override int MaxUsesPerCombat => 2;

    protected override bool ShouldBeFree(CardModel card) => card.Type is CardType.Power;
}
