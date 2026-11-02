using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics.Ancients.Lear;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Lear's Mask",
    description: "The first {Skills} [gold]Skills[/gold] you play each combat are free.",
    flavor: ""
)]
public partial class LearsMaskRelic : FreeCardsThisCombatRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new(nameof(Skills), 3)];
    protected override int MaxUsesPerCombat => Skills.IntValue;

    protected override bool ShouldBeFree(CardModel card) => card.Type == CardType.Skill;
}
