using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models.RelicPools;
using NewKunlun.NewKunlunCode.Cards.Ancients;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics.Ancients.Abacus;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Reconnaissance",
    description: "Add one [gold]Intelligence Report[/gold] to your deck.",
    flavor: ""
)]
public class ReconnaissanceRelic : AddCardsToDeckRelic<IntelligenceReportCard>
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override int NumCards => 1;
}
