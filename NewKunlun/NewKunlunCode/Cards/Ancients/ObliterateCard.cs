using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Powers;

namespace NewKunlun.NewKunlunCode.Cards.Ancients;

[Pool(typeof(IroncladCardPool))]
[CardLocalization(
    title: "Obliterate",
    description: "Deal {Damage:diff()} damage and apply {Vulnerable:diff()} [gold]Vulnerable[/gold] to all enemies.\nYour attacks deal double damage this turn."
)]
public partial class ObliterateCard()
    : NewKunlunCard(1, CardType.Attack, CardRarity.Ancient, TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(8, ValueProp.Move), new PowerVar<VulnerablePower>(nameof(Vulnerable), 2)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<VulnerablePower>()];

    protected override void OnUpgrade()
    {
        Damage.UpgradeValueTo(10);
        Vulnerable.UpgradeValueTo(3);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (Owner.Creature.CombatState == null)
            return;

        await DamageCmd
            .Attack(Damage.BaseValue)
            .FromCard(this, cardPlay)
            .WithHeavySlashVfx()
            .TargetingAllOpponents(Owner.Creature.CombatState)
            .Execute(choiceContext);
        if (Owner.Creature.CombatState == null)
            return;

        await PowerCmd.Apply<VulnerablePower>(
            choiceContext,
            Owner.Creature.CombatState.HittableEnemies,
            Vulnerable.IntValue,
            Owner.Creature,
            this
        );

        var current = Owner.Creature.GetPower<ObliteratePower>();
        await PowerCmd.Apply<ObliteratePower>(
            choiceContext,
            Owner.Creature,
            current?.Amount ?? 2,
            Owner.Creature,
            this
        );
    }
}
