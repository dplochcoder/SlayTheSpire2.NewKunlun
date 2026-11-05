using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Runs;
using NewKunlun.NewKunlunCode.Cards.Ancients;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics.Ancients.Shuanshuan;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "The Legend of the Porky Heroes",
    description: "On pickup, add one [gold]Unto Legend[/gold] to your deck.",
    flavor: ""
)]
public class TheLegendOfThePorkyHeroesRelic : AddCardsToDeckRelic<UntoLegendCard>
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override bool IsAllowed(IRunState runState) => runState.Players.Count > 1;

    protected override int NumCards => 1;
}
