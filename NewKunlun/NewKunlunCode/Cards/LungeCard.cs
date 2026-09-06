using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Commands;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Tips;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Lunge",
    description: "Deal {Damage:diff()} damage to a random enemy.\n[gold]Discharge[/gold] 1 to draw {NumCards:diff()} cards."
)]
public partial class LungeCard()
    : NewKunlunCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.RandomEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(11M, ValueProp.Move), new DynamicVar(nameof(NumCards), 3M)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tip.Discharge()];

    protected override void OnUpgrade()
    {
        Damage.UpgradeValueTo(14M);
        NumCards.UpgradeValueTo(4M);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (Owner.Creature.CombatState == null)
            return;

        await DamageCmd
            .Attack(Damage.BaseValue)
            .FromCard(this, cardPlay)
            .WithSlashVfx()
            .TargetingRandomOpponents(Owner.Creature.CombatState)
            .Execute(choiceContext);
        var charges = await QiChargeCmd.Discharge(
            choiceContext,
            Owner.Creature,
            1,
            QiChargeCmd.Locked.ExcludeLocked,
            Owner.Creature,
            this
        );
        if (charges > 0)
            await CardPileCmd.Draw(choiceContext, NumCards.IntValue, Owner);
    }
}
