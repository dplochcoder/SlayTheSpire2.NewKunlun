using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Gold;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using Printer3DRelic = NewKunlun.NewKunlunCode.Relics.Ancients.Kuafu.Printer3DRelic;

namespace NewKunlun.NewKunlunCode.RestSiteOptions;

public class Printer3DRestSiteOption(Player owner) : RestSiteOption(owner)
{
    public override string OptionId => "NEW_KUNLUN-PRINTER_3D";

    public override bool IsEnabled =>
        PileType
            .Deck.GetPile(Owner)
            .Cards.Any(c =>
                Printer3DRelic.TryGetCost(c.Rarity, out var cost) && Owner.Gold >= cost
            );

    public override async Task<bool> OnSelect()
    {
        var card = (
            await CardSelectCmd.FromDeckGeneric(
                Owner,
                new(ModelDb.Relic<DollysMirror>().SelectionScreenPrompt, minCount: 0, maxCount: 1)
                {
                    Cancelable = true,
                    RequireManualConfirmation = true,
                },
                c => Printer3DRelic.TryGetCost(c.Rarity, out var cost) && Owner.Gold >= cost
            )
        ).FirstOrDefault();
        if (card == null)
            return false;

        await PlayerCmd.LoseGold(Printer3DRelic.GetCost(card.Rarity), Owner, GoldLossType.Spent);
        CardCmd.PreviewCardPileAdd(
            await CardPileCmd.Add(Owner.RunState.CloneCard(card), PileType.Deck)
        );
        return true;
    }
}
