using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Gold;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using NewKunlun.NewKunlunCode.Relics;

namespace NewKunlun.NewKunlunCode.RestSiteOptions;

public class Printer3DRestSiteOption(Player owner) : RestSiteOption(owner)
{
    public override string OptionId => "NEW_KUNLUN-PRINTER_3D";

    public override bool IsEnabled =>
        PileType.Deck.GetPile(Owner).Cards.Count > 0 && Owner.Gold >= Printer3DRelic.GoldCost;

    public override async Task<bool> OnSelect()
    {
        var card = (
            await CardSelectCmd.FromDeckGeneric(
                Owner,
                new CardSelectorPrefs(ModelDb.Relic<DollysMirror>().SelectionScreenPrompt, 1)
                {
                    Cancelable = true,
                }
            )
        ).FirstOrDefault();
        if (card == null)
            return false;

        await PlayerCmd.LoseGold(Printer3DRelic.GoldCost, Owner, GoldLossType.Spent);
        CardCmd.PreviewCardPileAdd(
            await CardPileCmd.Add(Owner.RunState.CloneCard(card), PileType.Deck)
        );
        return true;
    }
}
