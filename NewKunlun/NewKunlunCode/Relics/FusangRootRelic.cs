using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Fusang Root",
    description: "Lose {MaxHpLoss} max hp.\nIf you lose {Heal} hp or less during combat, recover it at the end of combat.",
    flavor: ""
)]
public partial class FusangRootRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new(nameof(MaxHpLoss), 11), new HealVar(9)];

    public override bool ShowCounter => Status == RelicStatus.Active;

    public override int DisplayAmount => Heal.IntValue - HpLostThisRoom;

    private void UpdateStatus()
    {
        Status =
            IsCombatRoom
            && !PlayerDiedThisRoom
            && HpLostThisRoom > 0
            && HpLostThisRoom <= Heal.IntValue
                ? RelicStatus.Active
                : RelicStatus.Normal;
        InvokeDisplayAmountChanged();
    }

    [SavedProperty]
    public bool IsCombatRoom
    {
        get;
        set
        {
            AssertMutable();
            field = value;
            UpdateStatus();
        }
    }

    [SavedProperty]
    public int HpLostThisRoom
    {
        get;
        set
        {
            AssertMutable();
            field = value;
            UpdateStatus();
        }
    }

    [SavedProperty]
    public bool PlayerDiedThisRoom
    {
        get;
        set
        {
            AssertMutable();
            field = value;
            UpdateStatus();
        }
    }

    public override Task AfterObtained() =>
        CreatureCmd.LoseMaxHp(
            new ThrowingPlayerChoiceContext(),
            Owner.Creature,
            MaxHpLoss.IntValue,
            false
        );

    public override Task AfterRoomEntered(AbstractRoom room)
    {
        HpLostThisRoom = 0;
        IsCombatRoom = room is CombatRoom;
        return Task.CompletedTask;
    }

    public override Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource
    )
    {
        if (target == Owner.Creature)
        {
            if (result.WasTargetKilled)
                PlayerDiedThisRoom = true;
            else
                HpLostThisRoom += result.UnblockedDamage;
        }

        return Task.CompletedTask;
    }

    public override async Task AfterCombatVictory(CombatRoom room)
    {
        if (!PlayerDiedThisRoom && HpLostThisRoom > 0 && HpLostThisRoom <= Heal.IntValue)
        {
            Flash();
            await CreatureCmd.Heal(Owner.Creature, HpLostThisRoom);
            HpLostThisRoom = 0;
        }
    }
}
