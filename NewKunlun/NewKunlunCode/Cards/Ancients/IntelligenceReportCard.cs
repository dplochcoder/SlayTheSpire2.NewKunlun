using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Powers;

namespace NewKunlun.NewKunlunCode.Cards.Ancients;

[Pool(typeof(EventCardPool))]
[CardLocalization(
    title: "Intelligence Report",
    description: "At the start of the Act 1 boss combat, gain {Plating} [gold]Plating[/gold].\nThe boss drops an extra Rare card reward."
)]
public partial class IntelligenceReportCard()
    : NewKunlunCard(0, CardType.Quest, CardRarity.Ancient, TargetType.None)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Unplayable];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<PlatingPower>(nameof(Plating), 12)];

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is not CombatRoom { RoomType: RoomType.Boss })
            return;

        var deck = DeckVersion;
        if (deck != null)
        {
            await CardPileCmd.RemoveFromDeck(deck);
            await CardPileCmd.RemoveFromCombat(this, skipVisuals: true);
        }

        await PowerCmd.Apply<IntelligenceReportPower>(
            new ThrowingPlayerChoiceContext(),
            Owner.Creature,
            1,
            Owner.Creature,
            null
        );
        await PowerCmd.Apply<PlatingPower>(
            new ThrowingPlayerChoiceContext(),
            Owner.Creature,
            Plating.IntValue,
            Owner.Creature,
            null
        );
    }
}
