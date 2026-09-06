using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Commands;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Hooks;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Powers;
using NewKunlun.NewKunlunCode.Tips;
using NewKunlun.NewKunlunCode.Variables;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Triple Slash",
    description: "Deal {Damage:diff()} damage.\nReturns to your hand twice this turn.\nOn the third play, deals {BigHitDamage:diff()} damage, [gold]Discharging[/gold] 1 to deal double."
)]
public partial class TripleSlashCard()
    : NewKunlunCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy),
        ILateModifyResultLocation
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new CustomDamageVar<TripleSlashCard>(
                nameof(Damage),
                7M,
                ValueProp.Move,
                _ => CalculateBaseDamage()
            ),
            new DamageVar(nameof(SmallHitDamage), 7M, ValueProp.Move),
            new DamageVar(nameof(BigHitDamage), 13M, ValueProp.Move),
        ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tip.QiCharge()];

    private bool IsBigHitTurn => _playsThisTurn % 3 == 2;

    protected override bool ShouldGlowGoldInternal => IsBigHitTurn;

    private decimal CalculateBaseDamage() =>
        IsBigHitTurn ? BigHitDamage.BaseValue : SmallHitDamage.BaseValue;

    private int _playsThisTurn;

    protected override void OnUpgrade()
    {
        Damage.UpgradeValueTo(9M);
        SmallHitDamage.UpgradeValueTo(9M);
        BigHitDamage.UpgradeValueTo(18M);
    }

    protected override void AfterCloned() => _playsThisTurn = 0;

    public override Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants
    )
    {
        _playsThisTurn = 0;
        return Task.CompletedTask;
    }

    protected override CardLocation GetResultLocationForCardPlay()
    {
        var loc = base.GetResultLocationForCardPlay();
        if (_playsThisTurn < 2 && loc.pileType == PileType.Discard)
            loc.pileType = PileType.Hand;
        return loc;
    }

    public void LateModifyResultLocation(ref CardLocation resultLocation)
    {
        if (_playsThisTurn > 2 && resultLocation.pileType == PileType.Hand)
            resultLocation.pileType = PileType.Discard;
    }

    private bool _justConsumedQiCharge = false;

    public override decimal ModifyDamageMultiplicative(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        CardPlay? cardPlay
    )
    {
        var multiplier = 1;
        if (
            cardSource == this
            && IsBigHitTurn
            && (_justConsumedQiCharge || dealer?.GetPowerAmount<QiChargePower>() > 0)
        )
            multiplier = 2;

        return amount * multiplier;
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (IsBigHitTurn)
        {
            var charge = await QiChargeCmd.Discharge(
                choiceContext,
                Owner.Creature,
                1,
                Owner.Creature,
                this
            );
            _justConsumedQiCharge = charge > 0;
        }

        var attack = DamageCmd
            .Attack(Damage.Calculate(cardPlay.Target))
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target!);
        attack = IsBigHitTurn ? attack.WithHeavySlashVfx() : attack.WithSlashVfx();
        await attack.Execute(choiceContext);

        _justConsumedQiCharge = false;
        ++_playsThisTurn;
    }
}
