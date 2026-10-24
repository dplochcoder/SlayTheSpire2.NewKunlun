using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics;

[Pool(typeof(YiRelicPool))]
[RelicLocalization(
    title: "Rhizomatic Stabilizer",
    description: "On pickup, upgrade 2 random cards.",
    flavor: ""
)]
public class RhizomaticStabilizerRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override bool HasUponPickupEffect => true;

    public override Task AfterObtained()
    {
        List<CardModel> deck = [.. PileType.Deck.GetPile(Owner).Cards];
        Owner.PlayerRng.Rewards.Shuffle(deck);
        foreach (var card in deck.Take(2))
            CardCmd.Upgrade(card);

        return Task.CompletedTask;
    }
}
