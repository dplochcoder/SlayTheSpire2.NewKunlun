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
using NewKunlun.NewKunlunCode.Powers;
using NewKunlun.NewKunlunCode.Tips;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Qi Burst",
    description: "[gold]Discharge[/gold] X.\nDeal {DamageMultiplier:diff()}damage X times to all enemies.\n[glow]Boost[/glow] {BoostMultiplier:diff()}X."
)]
public partial class QiBurstCard()
    : NewKunlunCard(1, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DamageVar(nameof(DamageMultiplier), 7M, ValueProp.Move),
            new DynamicVar(nameof(BoostMultiplier), 2M),
        ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tip.Discharge(), Tip.Boost()];

    protected override void OnUpgrade()
    {
        DamageMultiplier.UpgradeValueTo(11M);
        BoostMultiplier.UpgradeValueTo(3M);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var charges = await QiChargeCmd.Discharge(
            choiceContext,
            Owner.Creature,
            int.MaxValue,
            Owner.Creature,
            this
        );
        if (charges == 0)
            return;

        if (Owner.Creature.CombatState == null)
            return;
        await DamageCmd
            .Attack((decimal)DamageMultiplier.BaseValue)
            .FromCard(this, cardPlay)
            .WithSlashVfx()
            .WithHitCount(charges)
            .TargetingAllOpponents(Owner.Creature.CombatState)
            .Execute(choiceContext);

        await PowerCmd.Apply<BoostPower>(
            choiceContext,
            Owner.Creature,
            charges * BoostMultiplier.BaseValue,
            Owner.Creature,
            this
        );
    }
}
