using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using NewKunlun.NewKunlunCode.Cards;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Powers;

[PowerLocalization(
    title: "Talisman",
    description: "[gold]Talisman Detonate[/gold] can be activated on this enemy. Removed after {TurnsRemaining:plural:turns|turn} or on detonate.",
    smartDescription: "",
    remoteDescription: "Another player can activate [gold]Talisman Detonate[/gold] on this enemy."
)]
public partial class TalismanPower : NewKunlunPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar(nameof(TurnsRemaining), 2M)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            HoverTipFactory.FromCard<TalismanDetonateCard>(
                upgrade: TalismanDashCard.IsUpgradedAnywhere(Applier?.Player)
            ),
        ];

    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants
    )
    {
        if (!participants.Contains(Owner))
            return;
        if (--TurnsRemaining.BaseValue > 0)
            return;

        await PowerCmd.Remove(this);
        Flash();
    }
}
