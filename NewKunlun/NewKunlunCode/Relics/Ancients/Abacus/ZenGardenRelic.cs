using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models.RelicPools;
using NewKunlun.NewKunlunCode.Localization;
using MeditateCard = NewKunlun.NewKunlunCode.Cards.Ancients.MeditateCard;

namespace NewKunlun.NewKunlunCode.Relics.Ancients.Abacus;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Zen Garden",
    description: "On pickup, add one [gold]Meditate[/gold] to your deck.",
    flavor: ""
)]
public class ZenGardenRelic : AddCardsToDeckRelic<MeditateCard>
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override int NumCards => 1;
}
