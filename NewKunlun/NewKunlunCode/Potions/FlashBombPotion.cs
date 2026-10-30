using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models.PotionPools;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Potions;

[Pool(typeof(SharedPotionPool))]
[PotionLocalization(title: "Flash Bomb", description: "Stun all enemies.")]
public class FlashBombPotion : NewKunlunPotion
{
    public override PotionRarity Rarity => PotionRarity.Token;
    public override PotionUsage Usage => PotionUsage.CombatOnly;
    public override TargetType TargetType => TargetType.AllEnemies;

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        List<Creature> enemies = [.. Owner.Creature.CombatState?.HittableEnemies ?? []];
        if (enemies.Count == 0)
            return;

        // TODO: Flash fx
        foreach (var enemy in enemies)
            NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(
                NFireSmokePuffVfx.Create(enemy)
            );

        foreach (var enemy in enemies)
            await CreatureCmd.Stun(enemy);
    }
}
