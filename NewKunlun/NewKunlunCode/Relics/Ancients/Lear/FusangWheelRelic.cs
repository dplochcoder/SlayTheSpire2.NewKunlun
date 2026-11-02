using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics.Ancients.Lear;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Fusang Wheel",
    description: "Gain {Energy:energyIcons()} each turn.\nThe first eligible card you play each combat is removed from your deck.",
    flavor: ""
)]
public partial class FusangWheelRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(1)];

    public override decimal ModifyMaxEnergy(Player player, decimal amount) =>
        player == Owner ? amount + Energy.IntValue : amount;

    [SavedProperty]
    public bool TriggeredThisCombat
    {
        get;
        set
        {
            AssertMutable();
            field = value;
            Status =
                !TriggeredThisCombat && CombatManager.Instance.IsInProgress
                    ? RelicStatus.Active
                    : RelicStatus.Normal;
        }
    }

    public override Task AfterRoomEntered(AbstractRoom room)
    {
        TriggeredThisCombat = false;
        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (
            cardPlay.Card.Owner != Owner
            || TriggeredThisCombat
            || cardPlay.Card.DeckVersion is not { IsRemovable: true } deck
        )
            return;

        Flash();
        TriggeredThisCombat = true;
        await CardPileCmd.RemoveFromDeck(deck);
    }
}
