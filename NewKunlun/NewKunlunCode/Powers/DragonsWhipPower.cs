using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using NewKunlun.NewKunlunCode.Cards;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;
using DragonsWhipCard = NewKunlun.NewKunlunCode.Cards.Ancients.DragonsWhipCard;

namespace NewKunlun.NewKunlunCode.Powers;

[PowerLocalization(
    title: "Dragon's Whip",
    description: "If this creature dies to poison, your [gold]Dragon's Whip[/gold] card permanently increases its effect."
)]
public class DragonsWhipPower : NewKunlunPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    protected override object? InitInternalData() => new ApplyingCard();

    public override Task AfterPowerAmountChanged(
        PlayerChoiceContext choiceContext,
        PowerModel power,
        decimal amount,
        Creature? applier,
        CardModel? cardSource
    )
    {
        if (cardSource is DragonsWhipCard card)
            GetInternalData<ApplyingCard>().CardSource = card;
        return Task.CompletedTask;
    }

    private record ApplyingCard
    {
        public DragonsWhipCard? CardSource;
    }

    [HarmonyPatch]
    private static class Patches
    {
        [HarmonyPatch(typeof(PoisonPower), nameof(PoisonPower.Trigger))]
        [HarmonyPostfix]
        private static void Trigger(
            // ReSharper disable once InconsistentNaming
            PoisonPower __instance,
            // ReSharper disable once InconsistentNaming
            ref Task __result
        ) => __result = Wrap(__instance, __result);

        private static async Task Wrap(PoisonPower poison, Task doPoison)
        {
            var creature = poison.Owner;
            List<DragonsWhipPower> powers = [.. poison.Owner.GetPowerInstances<DragonsWhipPower>()];
            var wasAlive = creature.IsAlive;

            await doPoison;

            if (wasAlive && !creature.IsAlive)
            {
                foreach (var power in powers)
                {
                    var cardSource = power.GetInternalData<ApplyingCard>()?.CardSource;
                    if (cardSource == null)
                        continue;

                    power.Flash();
                    var deck = cardSource.Permanently(c =>
                        c.Poison.BaseValue += c.Increase.IntValue
                    );
                    if (deck != null)
                        CardCmd.Preview(deck);
                }
            }
        }
    }
}
