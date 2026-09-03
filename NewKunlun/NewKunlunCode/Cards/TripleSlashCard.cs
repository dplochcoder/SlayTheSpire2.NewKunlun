using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Hooks;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Tips;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Triple Slash",
    description: "Deal {SmallHitDamage:diff()} damage. Return to your hand the first two times played this turn. On the third play, deal {BigHitDamage:diff()} damage, or spend 1 [gold]Qi Charge[/gold] to deal double."
)]
public partial class TripleSlashCard()
    : NewKunlunCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy),
        ILateModifyResultLocation
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DamageVar(nameof(SmallHitDamage), 5M, ValueProp.Move),
            new DamageVar(nameof(BigHitDamage), 13M, ValueProp.Move),
        ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tip.QiCharge()];

    private bool IsBigHitTurn => _playsThisTurn % 3 == 2;

    protected override bool ShouldGlowGoldInternal => IsBigHitTurn;

    private int _playsThisTurn;

    protected override void OnUpgrade()
    {
        SmallHitDamage.UpgradeValueTo(8M);
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

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var attack = DamageCmd
            .Attack(IsBigHitTurn ? BigHitDamage.BaseValue : SmallHitDamage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target!);
        attack = IsBigHitTurn ? attack.WithHeavySlashVfx() : attack.WithSlashVfx();
        await attack.Execute(choiceContext);

        ++_playsThisTurn;
    }
}
