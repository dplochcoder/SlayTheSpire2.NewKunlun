using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using NewKunlun.NewKunlunCode.Commands;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Potions;

[PotionLocalization(title: "Chaos Flask", description: "Randomly enchant all cards in your hand.")]
public class ChaosFlaskPotion : NewKunlunPotion
{
    public override PotionRarity Rarity => PotionRarity.Rare;
    public override PotionUsage Usage => PotionUsage.CombatOnly;
    public override TargetType TargetType => TargetType.AnyPlayer;

    protected override Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        if (target?.Player?.PlayerCombatState == null || target.CombatState == null)
            return Task.CompletedTask;

        List<CardModel> cards = [.. target.Player.PlayerCombatState.Hand.Cards];
        foreach (var card in cards)
            RandomEnchantCmd.ApplyEnchantment(
                target.Player,
                target.CombatState.RunState.Rng.CombatCardGeneration,
                card
            );

        return Task.CompletedTask;
    }
}
