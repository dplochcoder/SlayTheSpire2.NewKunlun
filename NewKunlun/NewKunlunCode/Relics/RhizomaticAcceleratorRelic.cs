using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Screens;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Rhizomatic Accelerator",
    description: "Transform all your [gold]Strikes[/gold] and [gold]Defends[/gold] into random common cards.",
    flavor: "",
    customPromptA: "Strikes and Defends transformed."
)]
public class RhizomaticAcceleratorRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override bool HasUponPickupEffect => true;

    private CardModel CreateRandomCommonCardForTransform(CardModel original)
    {
        var options = CardFactory
            .GetDefaultTransformationOptions(original, isInCombat: false)
            .Where(c => c.Rarity == CardRarity.Common);
        return original.CardScope!.CreateCard(
            Owner.RunState.Rng.Niche.NextItem(options)!,
            original.Owner
        );
    }

    public override async Task AfterObtained()
    {
        var transformations = PileType
            .Deck.GetPile(Owner)
            .Cards.Where(c => c is { IsBasicStrikeOrDefend: true, IsRemovable: true })
            .Select(c => new CardTransformation(c, CreateRandomCommonCardForTransform(c)));
        List<CardPileAddResult> results =
        [
            .. await CardCmd.Transform(transformations, rng: null, CardPreviewStyle.None),
        ];

        if (results.Count <= 0 || !LocalContext.IsMe(Owner))
            return;

        NSimpleCardsViewScreen.ShowScreen(results, this.CustomPromptA);
    }
}
