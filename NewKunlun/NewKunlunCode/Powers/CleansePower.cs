using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using NewKunlun.NewKunlunCode.Commands;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Tips;
using NewKunlun.NewKunlunCode.Variables;

namespace NewKunlun.NewKunlunCode.Powers;

[PowerLocalization(
    title: "Cleanse",
    description: "",
    smartDescription: "At the end of your next {Amount:plural:turn|{Amount} turns}, heal {InternalDamageHeal} [gold]Internal Damage[/gold] and [gold]Exhaust[/gold] 1 card from your hand.",
    selectionScreenPrompt: "Choose a card to Exhaust."
)]
public partial class CleansePower : NewKunlunPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new InternalDamageHealVar(0M)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [Tip.InternalDamage(), Tip.Exhaust()];

    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants
    )
    {
        if (!participants.Contains(Owner))
            return;

        Flash();
        await InternalDamageCmd.Heal(choiceContext, Owner, InternalDamageHeal, Owner, null);

        var cards = await CardSelectCmd.FromHand(
            choiceContext,
            Owner.Player!,
            new CardSelectorPrefs(SelectionScreenPrompt, 1),
            c => true,
            this
        );
        foreach (var card in cards)
            await CardCmd.Exhaust(choiceContext, card);

        await PowerCmd.Decrement(this);
    }
}
