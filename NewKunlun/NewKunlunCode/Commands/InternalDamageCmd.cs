using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using NewKunlun.NewKunlunCode.Powers;

namespace NewKunlun.NewKunlunCode.Commands;

public static class InternalDamageCmd
{
    public static async Task Apply(
        PlayerChoiceContext choiceContext,
        Creature target,
        decimal amount,
        Creature? applier,
        CardModel? cardSource,
        bool silent = false
    )
    {
        if (amount <= 0)
            return;

        // TODO: Hook modifiers.
        await PowerCmd.Apply<InternalDamagePower>(
            choiceContext,
            target,
            amount,
            applier,
            cardSource,
            silent
        );
    }
}
