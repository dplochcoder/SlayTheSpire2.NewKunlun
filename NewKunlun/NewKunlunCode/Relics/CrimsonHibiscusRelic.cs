using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Commands;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics;

[Pool(typeof(YiRelicPool))]
[RelicLocalization(
    title: "Crimson Hibiscus",
    description: "Remove a [gold]Strike[/gold] or [gold]Defend[/gold].\nUpgrade a [gold]Strike[/gold] or [gold]Defend[/gold].",
    flavor: ""
)]
public class CrimsonHibiscusRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override bool HasUponPickupEffect => true;

    public override async Task AfterObtained()
    {
        var card = (
            await CardSelectCmd.FromDeckForRemoval(
                Owner,
                new CardSelectorPrefs(CardSelectorPrefs.RemoveSelectionPrompt, 1, 1),
                c => c.IsBasicStrikeOrDefend
            )
        ).First();
        await CardPileCmd.RemoveFromDeck(card);
        card = (
            await CardSelectCmds.FromDeckForUpgradeWithFilter(
                Owner,
                new CardSelectorPrefs(CardSelectorPrefs.UpgradeSelectionPrompt, 1, 1),
                c => c.IsBasicStrikeOrDefend
            )
        ).First();
        CardCmd.Upgrade(card);
    }
}
