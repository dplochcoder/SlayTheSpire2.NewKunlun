using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.RelicPools;
using NewKunlun.NewKunlunCode.Cards.Ancients;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;
using Void = MegaCrit.Sts2.Core.Models.Cards.Void;

namespace NewKunlun.NewKunlunCode.Relics.Ancients.Kuafu;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Rhizomatic Battery",
    description: "Add two [gold]Rhizomatic Capacitor[/gold]s to your deck.\nWhenever you shuffle your deck, shuffle a [gold]Void[/gold] into your draw pile.",
    flavor: ""
)]
public class RhizomaticBatteryRelic : AddCardsToDeckRelic<RhizomaticCapacitorCard>
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override int NumCards => 2;

    public override async Task AfterShuffle(PlayerChoiceContext choiceContext, Player shuffler)
    {
        if (shuffler != Owner)
            return;

        Flash();
        await Owner.AddGeneratedCardToPile<Void>(PileType.Draw, position: CardPilePosition.Random);
    }
}
