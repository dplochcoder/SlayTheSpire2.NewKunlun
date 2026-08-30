using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using NewKunlun.NewKunlunCode.Powers;
using NewKunlun.NewKunlunCode.Variables;

namespace NewKunlun.NewKunlunCode.Commands;

public static class InternalDamageCmd
{
    public static async Task Apply(
        PlayerChoiceContext choiceContext,
        Creature target,
        InternalDamageVar amount,
        Creature? applier,
        CardModel? cardSource,
        bool silent = false
    )
    {
        if (amount.BaseValue <= 0)
            return;

        // TODO: Hook modifiers.
        await PowerCmd.Apply<InternalDamagePower>(
            choiceContext,
            target,
            amount.BaseValue,
            applier,
            cardSource,
            silent
        );
    }
}
