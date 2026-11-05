using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.RelicPools;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics.Ancients.Shennong;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Zeng-zi",
    description: "Add {Cards} [gold]Alchemize[/gold] to your deck.",
    flavor: ""
)]
public partial class ZengZiRelic : AddCardsToDeckRelic<Alchemize>
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new(nameof(Cards), 2)];

    protected override int NumCards => Cards.IntValue;
}
