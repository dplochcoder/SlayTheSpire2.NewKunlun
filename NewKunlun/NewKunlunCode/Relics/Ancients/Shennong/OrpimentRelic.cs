using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using NewKunlun.NewKunlunCode.Cards.Ancients;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics.Ancients.Shennong;

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
        if (room is not CombatRoom || NumCitrineUses <= 0 || Owner.PlayerCombatState == null)
            return;

        Flash();
        var count = NumCitrineUses;
        await PowerCmd.Apply<PoisonPower>(
            new ThrowingPlayerChoiceContext(),
            Owner.Creature,
            NumCitrineUses,
            Owner.Creature,
            null
        );
    }
}
