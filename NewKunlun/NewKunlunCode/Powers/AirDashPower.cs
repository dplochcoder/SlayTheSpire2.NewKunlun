using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using NewKunlun.NewKunlunCode.Cards;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Tips;

namespace NewKunlun.NewKunlunCode.Powers;

[PowerLocalization(
    title: "Air Dash",
    description: "Gain {Amount} [gold]Dexterity[/gold] this turn."
)]
public class AirDashPower : NewKunlunPower, ITemporaryPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public AbstractModel OriginModel => ModelDb.Card<AirDashCard>();
    public PowerModel InternallyAppliedPower => ModelDb.Power<DexterityPower>();

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tip.Dexterity()];

    public override Task BeforeApplied(
        Creature target,
        decimal amount,
        Creature? applier,
        CardModel? cardSource
    ) =>
        PowerCmd.Apply<DexterityPower>(
            new ThrowingPlayerChoiceContext(),
            target,
            amount,
            applier,
            cardSource,
            silent: true
        );

    public override async Task AfterPowerAmountChanged(
        PlayerChoiceContext choiceContext,
        PowerModel power,
        Decimal amount,
        Creature? applier,
        CardModel? cardSource
    )
    {
        if (amount == Amount || this != power)
            return;
        await PowerCmd.Apply<DexterityPower>(
            choiceContext,
            Owner,
            amount,
            applier,
            cardSource,
            silent: true
        );
    }

    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants
    )
    {
        if (!participants.Contains(Owner))
            return;

        Flash();
        await PowerCmd.Remove(this);
        await PowerCmd.Apply<DexterityPower>(choiceContext, Owner, -Amount, Owner, null);
    }
}
