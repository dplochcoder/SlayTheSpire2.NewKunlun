using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using NewKunlun.NewKunlunCode.Afflictions;
using NewKunlun.NewKunlunCode.Cards;
using NewKunlun.NewKunlunCode.Collections;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Powers;
using NewKunlun.NewKunlunCode.Tips;

namespace NewKunlun.NewKunlunCode.Relics;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Orpiment",
    description: "Add one [gold]Medicinal Citrine[/gold] to your deck.",
    flavor: ""
)]
public class OrpimentRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromCard<MedicinalCitrineCard>()];

    [SavedProperty]
    public int NumCitrineUses
    {
        get;
        set
        {
            AssertMutable();
            field = value;
            InvokeDisplayAmountChanged();
        }
    }

    public override bool ShowCounter => NumCitrineUses > 0;

    public override int DisplayAmount => NumCitrineUses;

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is not CombatRoom || NumCitrineUses == 0 || Owner.PlayerCombatState == null)
            return;

        Flash();
        var count = NumCitrineUses;
        await PowerCmd.Apply<ToxinPower>(
            new ThrowingPlayerChoiceContext(),
            Owner.Creature,
            NumCitrineUses,
            Owner.Creature,
            null
        );

        HashMultiset<CardModel> cards = new();
        List<CardModel> draw =
        [
            .. Owner.PlayerCombatState.DrawPile.Cards.Where(
                ModelDb.Affliction<ToxicAffliction>().CanAfflict
            ),
        ];
        if (draw.Count == 0)
            return;

        for (var i = 0; i < count; i++)
            cards.Add(Owner.RunState.Rng.CombatCardSelection.NextItem(draw)!);
        HashSet<CardModel> afflicted = [];
        foreach (var (card, amount) in cards.Counts)
        {
            if (await CardCmd.Afflict<ToxicAffliction>(card, amount) != null)
                afflicted.Add(card);
        }
        CardCmd.Preview([.. draw.Where(afflicted.Contains)]);
    }
}
