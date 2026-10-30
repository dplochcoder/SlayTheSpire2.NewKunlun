using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.RelicPools;
using NewKunlun.NewKunlunCode.Cards;
using NewKunlun.NewKunlunCode.Commands;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Rhizomatic Sharpener",
    description: "On pickup, transform 1 attack in your deck into [gold]Yingzhao's Lance[/gold].",
    flavor: ""
)]
public class RhizomaticSharpenerRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromCard<YingzhaosLanceCard>()];

    public override bool IsAllowedForPlayer(Player player) =>
        player.Deck.Cards.Any(c => c is { Type: CardType.Attack, IsTransformable: true });

    public override bool HasUponPickupEffect => true;

    public override async Task AfterObtained()
    {
        await CardSelectCmds.FromDeckForTransformationPreservingMutations<YingzhaosLanceCard>(
            Owner,
            new(CardSelectorPrefs.TransformSelectionPrompt, 1, 1),
            c => c.Type == CardType.Attack
        );
    }
}
