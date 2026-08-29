using System.Diagnostics.CodeAnalysis;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SlayTheSpire2.NewKunlun.SlayTheSpire2.NewKunlunCode.Character;

namespace SlayTheSpire2.NewKunlun.SlayTheSpire2.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
public class TripleSlashCard()
    : NewKunlunCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    private const string SmallHitDamage = nameof(SmallHitDamage);
    private const string BigHitDamage = nameof(BigHitDamage);

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DamageVar(8M, ValueProp.Move),
            new DamageVar(SmallHitDamage, 8M, ValueProp.Move),
            new DamageVar(BigHitDamage, 17M, ValueProp.Move),
        ];

    private void UpdateDamage() =>
        DynamicVars.Damage.BaseValue = DynamicVars[
            PlaysThisTurn == 2 ? BigHitDamage : SmallHitDamage
        ].BaseValue;

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
        DynamicVars[SmallHitDamage].UpgradeValueBy(3M);
        DynamicVars[BigHitDamage].UpgradeValueBy(6M);
        UpdateDamage();
    }

    protected override void AfterDowngraded() => UpdateDamage();

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

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var attack = DamageCmd
            .Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target!);
        attack =
            DynamicVars.Damage.BaseValue == DynamicVars[BigHitDamage].BaseValue
                ? attack.WithHitFx("vfx/vfx_heavy_blunt", tmpSfx: "heavy_attack.mp3")
                : attack.WithHitFx("vfx/vfx_attack_slash");
        await attack.Execute(choiceContext);

        ++PlaysThisTurn;
    }
}
