using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Extensions;
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
        foreach (
            var card in Owner.PlayerRng.Rewards.NextItems(
                2,
                Owner.Deck.Cards.Where(c => c.IsUpgradable)
            )
        )
            CardCmd.Upgrade(card);

        return Task.CompletedTask;
    }
}
