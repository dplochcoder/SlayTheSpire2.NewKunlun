using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Relics;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics;

[Pool(typeof(YiRelicPool))]
[RelicLocalization(
    title: "Parallel Processor",
    description: "You can no longer [gold]Smith[/gold] at rest sites.\nWhenever you [gold]Rest[/gold], upgrade a random card.",
    flavor: ""
)]
public class ParallelProcessorRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;
}
