using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Preparation Kit",
    description: "At the start of Boss and Elite combats, gain a random potion, {Strength} [gold]Strength[/gold], and {Dexterity} [gold]Dexterity[/gold].",
    flavor: ""
)]
public partial class PreparationKitRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new PowerVar<StrengthPower>(nameof(Strength), 2),
            new PowerVar<DexterityPower>(nameof(Dexterity), 2),
        ];

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is CombatRoom { RoomType: RoomType.Elite or RoomType.Boss })
        {
            Flash();

            await PotionCmd.TryToProcure(
                PotionFactory
                    .CreateRandomPotionOutOfCombat(Owner, Owner.RunState.Rng.CombatPotionGeneration)
                    .ToMutable(),
                Owner
            );
            await PowerCmd.Apply<StrengthPower>(
                new ThrowingPlayerChoiceContext(),
                Owner.Creature,
                Strength.IntValue,
                Owner.Creature,
                null
            );
            await PowerCmd.Apply<DexterityPower>(
                new ThrowingPlayerChoiceContext(),
                Owner.Creature,
                Dexterity.IntValue,
                Owner.Creature,
                null
            );
        }
    }
}
