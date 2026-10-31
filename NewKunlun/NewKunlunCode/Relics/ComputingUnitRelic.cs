using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Runs;
using NewKunlun.NewKunlunCode.Commands;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Computing Unit",
    description: "On pickup, gain one Uncommon Power reward.",
    flavor: ""
)]
public partial class ComputingUnitRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override bool HasUponPickupEffect => true;

    public override async Task AfterObtained()
    {
        List<CardModel> options =
        [
            .. CardFactory
                .CreateForReward(
                    Owner,
                    3,
                    new CardCreationOptions(
                        [Owner.Character.CardPool],
                        CardCreationSource.Other,
                        CardRarityOddsType.Uniform,
                        c => c is { Rarity: CardRarity.Uncommon, Type: CardType.Power }
                    ).WithFlags(CardCreationFlags.NoUpgradeRoll)
                )
                .Select(r => r.Card),
        ];
        await ExtraCardSelectCmds.FromChooseACardRewardScreen(Owner, options);
    }

    public override async Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState
    )
    {
        if (side != Owner.Creature.Side || Owner.PlayerCombatState!.TurnNumber != 3)
            return;

        Flash();
        await PlayerCmd.LoseEnergy(StackCount, Owner);
    }
}
