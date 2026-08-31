using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using NewKunlun.NewKunlunCode.Powers;

public static class ParryCmd
{
    public static async Task GainParry(
        PlayerChoiceContext choiceContext,
        Creature target,
        decimal amount,
        Creature? applier,
        CardModel? cardSource
    )
    {
        if (CombatManager.Instance.IsOverOrEnding || amount <= 0)
            return;

        await PowerCmd.Apply<ParryPower>(choiceContext, target, amount, applier, cardSource);
    }
}
