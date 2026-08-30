using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Hooks;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    "Triple Slash",
    "Deal {Damage:diff()} damage. Return to your hand the first two times played this turn. On the third play, deal {BigHitDamage:diff()} damage."
)]
public partial class TripleSlashCard()
    : NewKunlunCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy),
        ILateModifyResultLocation
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DamageVar(9M, ValueProp.Move),
            new DamageVar(nameof(SmallHitDamage), 9M, ValueProp.Move),
            new DamageVar(nameof(BigHitDamage), 19M, ValueProp.Move),
        ];

    private void UpdateDamage() =>
        Damage.BaseValue = (PlaysThisTurn % 3 == 2 ? BigHitDamage : SmallHitDamage).BaseValue;

    private int PlaysThisTurn
    {
        get;
        set
        {
            field = value;
            UpdateDamage();
        }
    } = 0;

    protected override void OnUpgrade()
    {
        SmallHitDamage.UpgradeValueBy(4M);
        BigHitDamage.UpgradeValueBy(8M);
        UpdateDamage();
    }

    protected override void AfterDowngraded() => UpdateDamage();

    protected override void AfterCloned() => PlaysThisTurn = 0;

    public override Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants
    )
    {
        PlaysThisTurn = 0;
        return Task.CompletedTask;
    }

    protected override CardLocation GetResultLocationForCardPlay()
    {
        var loc = base.GetResultLocationForCardPlay();
        if (PlaysThisTurn < 2 && loc.pileType == PileType.Discard)
            loc.pileType = PileType.Hand;
        return loc;
    }

    public void LateModifyResultLocation(ref CardLocation resultLocation)
    {
        if (PlaysThisTurn > 2 && resultLocation.pileType == PileType.Hand)
            resultLocation.pileType = PileType.Discard;
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var attack = DamageCmd
            .Attack(Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target!);
        attack =
            Damage.BaseValue == BigHitDamage.BaseValue
                ? attack.WithHitFx("vfx/vfx_heavy_blunt", tmpSfx: "heavy_attack.mp3")
                : attack.WithHitFx("vfx/vfx_attack_slash");
        await attack.Execute(choiceContext);

        ++PlaysThisTurn;
    }
}
