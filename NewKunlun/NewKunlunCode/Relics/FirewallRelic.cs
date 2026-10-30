using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Firewall",
    description: "Lose {MaxHpLoss} max HP.\nAt start of combat, gain 3 [gold]Dexterity[/gold].\nThe first time you take unblocked damage, lose 3 [gold]Dexterity[/gold].",
    flavor: ""
)]
public partial class FirewallRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new(nameof(Dexterity), 3), new(nameof(MaxHpLoss), 16)];

    public override bool HasUponPickupEffect => true;

    public override Task AfterObtained() =>
        CreatureCmd.LoseMaxHp(
            new ThrowingPlayerChoiceContext(),
            Owner.Creature,
            MaxHpLoss.IntValue,
            false
        );

    [SavedProperty]
    public bool BrokenThisCombat
    {
        get => field;
        set
        {
            AssertMutable();
            field = value;
        }
    }

    public override bool ShowCounter => true;

    public override int DisplayAmount => BrokenThisCombat ? 0 : Dexterity.IntValue;

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        Status = RelicStatus.Active;
        if (room is not CombatRoom)
            return;

        BrokenThisCombat = false;
        Flash();
        await PowerCmd.Apply<DexterityPower>(
            new ThrowingPlayerChoiceContext(),
            Owner.Creature,
            Dexterity.IntValue,
            Owner.Creature,
            null
        );
    }

    public override async Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource
    )
    {
        if (BrokenThisCombat || result.UnblockedDamage <= 0 || !result.Props.IsCardOrMonsterMove())
            return;

        BrokenThisCombat = true;
        Flash();
        Status = RelicStatus.Disabled;
        await PowerCmd.Apply<DexterityPower>(
            choiceContext,
            Owner.Creature,
            -Dexterity.IntValue,
            Owner.Creature,
            null
        );
    }
}
